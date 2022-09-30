using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedRecordCopyCtor : SynthesizedInstanceConstructor
    {
        private readonly int _memberOffset;

        public override ImmutableArray<ParameterSymbol> Parameters { get; }

        public override Accessibility DeclaredAccessibility
        {
            get
            {
                if (!ContainingType.IsSealed)
                {
                    return Accessibility.Protected;
                }
                return Accessibility.Private;
            }
        }

        public SynthesizedRecordCopyCtor(SourceMemberContainerTypeSymbol containingType, int memberOffset)
            : base(containingType)
        {
            _memberOffset = memberOffset;
            Parameters = ImmutableArray.Create(SynthesizedParameterSymbol.Create(this, TypeWithAnnotations.Create(isNullableEnabled: true, ContainingType), 0, RefKind.None, "original"));
        }

        internal override LexicalSortKey GetLexicalSortKey()
        {
            return LexicalSortKey.GetSynthesizedMemberKey(_memberOffset);
        }

        internal override void GenerateMethodBodyStatements(SyntheticBoundNodeFactory F, ArrayBuilder<BoundStatement> statements, BindingDiagnosticBag diagnostics)
        {
            BoundParameter receiver = F.Parameter(Parameters[0]);
            foreach (FieldSymbol item in ContainingType.GetFieldsToEmit())
            {
                if (!item.IsStatic)
                {
                    statements.Add(F.Assignment(F.Field(F.This(), item), F.Field(receiver, item)));
                }
            }
        }

        internal static MethodSymbol? FindCopyConstructor(NamedTypeSymbol containingType, NamedTypeSymbol within, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            MethodSymbol methodSymbol = null;
            int num = -1;
            ImmutableArray<MethodSymbol>.Enumerator enumerator = containingType.InstanceConstructors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MethodSymbol current = enumerator.Current;
                if (!HasCopyConstructorSignature(current) || current.HasUnsupportedMetadata || !AccessCheck.IsSymbolAccessible(current, within, ref useSiteInfo))
                {
                    continue;
                }
                if ((object)methodSymbol == null && num < 0)
                {
                    methodSymbol = current;
                    continue;
                }
                if (num < 0)
                {
                    num = methodSymbol.CustomModifierCount();
                }
                int num2 = current.CustomModifierCount();
                if (num2 <= num)
                {
                    if (num2 == num)
                    {
                        methodSymbol = null;
                        continue;
                    }
                    methodSymbol = current;
                    num = num2;
                }
            }
            return methodSymbol;
        }

        internal static bool IsCopyConstructor(Symbol member)
        {
            if (member is MethodSymbol methodSymbol && methodSymbol.MethodKind == MethodKind.Constructor)
            {
                return HasCopyConstructorSignature(methodSymbol);
            }
            return false;
        }

        internal static bool HasCopyConstructorSignature(MethodSymbol member)
        {
            NamedTypeSymbol containingType = member.ContainingType;
            if ((object)member != null && !member.IsStatic && member.ParameterCount == 1 && member.Arity == 0)
            {
                if (member.Parameters[0].Type.Equals(containingType, TypeCompareKind.AllIgnoreOptions))
                {
                    return member.Parameters[0].RefKind == RefKind.None;
                }
            }
            return false;
        }
    }
}
