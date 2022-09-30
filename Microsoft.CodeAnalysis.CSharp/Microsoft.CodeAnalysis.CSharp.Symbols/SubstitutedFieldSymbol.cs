using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Emit;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SubstitutedFieldSymbol : WrappedFieldSymbol
    {
        private readonly SubstitutedNamedTypeSymbol _containingType;

        private TypeWithAnnotations.Boxed _lazyType;

        public override Symbol ContainingSymbol => _containingType;

        public override NamedTypeSymbol ContainingType => _containingType;

        public override FieldSymbol OriginalDefinition => _underlyingField;

        public override bool IsImplicitlyDeclared
        {
            get
            {
                if (ContainingType.IsTupleType && IsDefaultTupleElement)
                {
                    return true;
                }
                return base.IsImplicitlyDeclared;
            }
        }

        public override Symbol AssociatedSymbol => OriginalDefinition.AssociatedSymbol?.SymbolAsMember(ContainingType);

        internal SubstitutedFieldSymbol(SubstitutedNamedTypeSymbol containingType, FieldSymbol substitutedFrom)
            : base(substitutedFrom.OriginalDefinition)
        {
            _containingType = containingType;
        }

        internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            if (_lazyType == null)
            {
                TypeWithAnnotations value = _containingType.TypeSubstitution.SubstituteType(OriginalDefinition.GetFieldType(fieldsBeingBound));
                Interlocked.CompareExchange(ref _lazyType, new TypeWithAnnotations.Boxed(value), null);
            }
            return _lazyType.Value;
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return OriginalDefinition.GetAttributes();
        }

        internal override NamedTypeSymbol FixedImplementationType(PEModuleBuilder emitModule)
        {
            return (NamedTypeSymbol)_containingType.TypeSubstitution.SubstituteType(OriginalDefinition.FixedImplementationType(emitModule)).Type;
        }

        public override bool Equals(Symbol obj, TypeCompareKind compareKind)
        {
            if ((object)this == obj)
            {
                return true;
            }
            if (obj is FieldSymbol fieldSymbol && TypeSymbol.Equals(_containingType, fieldSymbol.ContainingType, compareKind))
            {
                return OriginalDefinition == fieldSymbol.OriginalDefinition;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int num = OriginalDefinition.GetHashCode();
            int hashCode = _containingType.GetHashCode();
            if (hashCode != OriginalDefinition.ContainingType.GetHashCode())
            {
                num = Hash.Combine(hashCode, num);
            }
            return num;
        }
    }
}
