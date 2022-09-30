using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Emit.NoPia;

namespace Microsoft.CodeAnalysis.CSharp.Emit.NoPia
{
    public sealed class EmbeddedEvent : EmbeddedTypesManager<PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, CSharpAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter>.CommonEmbeddedEvent
    {
        protected override bool IsRuntimeSpecial => base.UnderlyingEvent.AdaptedEventSymbol.HasRuntimeSpecialName;

        protected override bool IsSpecialName => base.UnderlyingEvent.AdaptedEventSymbol.HasSpecialName;

        protected override EmbeddedType ContainingType => base.AnAccessor.ContainingType;

        protected override TypeMemberVisibility Visibility => PEModuleBuilder.MemberVisibility(base.UnderlyingEvent.AdaptedEventSymbol);

        protected override string Name => base.UnderlyingEvent.AdaptedEventSymbol.MetadataName;

        public EmbeddedEvent(EventSymbol underlyingEvent, EmbeddedMethod adder, EmbeddedMethod remover)
            : base(underlyingEvent, adder, remover, null)
        {
        }

        protected override IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
        {
            return base.UnderlyingEvent.AdaptedEventSymbol.GetCustomAttributesToEmit(moduleBuilder);
        }

        protected override ITypeReference GetType(PEModuleBuilder moduleBuilder, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            return moduleBuilder.Translate(base.UnderlyingEvent.AdaptedEventSymbol.Type, syntaxNodeOpt, diagnostics);
        }

        protected override void EmbedCorrespondingComEventInterfaceMethodInternal(SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, bool isUsedForComAwareEventBinding)
        {
            NamedTypeSymbol adaptedNamedTypeSymbol = ContainingType.UnderlyingNamedType.AdaptedNamedTypeSymbol;
            ImmutableArray<CSharpAttributeData>.Enumerator enumerator = adaptedNamedTypeSymbol.GetAttributes().GetEnumerator();
            while (enumerator.MoveNext())
            {
                CSharpAttributeData current = enumerator.Current;
                if (!current.IsTargetAttribute(adaptedNamedTypeSymbol, AttributeDescription.ComEventInterfaceAttribute))
                {
                    continue;
                }
                bool flag = false;
                NamedTypeSymbol namedTypeSymbol = null;
                if (current.CommonConstructorArguments.Length == 2)
                {
                    namedTypeSymbol = current.CommonConstructorArguments[0].ValueInternal as NamedTypeSymbol;
                    if ((object)namedTypeSymbol != null)
                    {
                        flag = EmbedMatchingInterfaceMethods(namedTypeSymbol, syntaxNodeOpt, diagnostics);
                        ImmutableArray<NamedTypeSymbol>.Enumerator enumerator2 = namedTypeSymbol.AllInterfacesNoUseSiteDiagnostics.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            NamedTypeSymbol current2 = enumerator2.Current;
                            if (EmbedMatchingInterfaceMethods(current2, syntaxNodeOpt, diagnostics))
                            {
                                flag = true;
                            }
                        }
                    }
                }
                if (!flag && isUsedForComAwareEventBinding)
                {
                    if ((object)namedTypeSymbol == null)
                    {
                        EmbeddedTypesManager.Error(diagnostics, ErrorCode.ERR_MissingSourceInterface, syntaxNodeOpt, adaptedNamedTypeSymbol, base.UnderlyingEvent.AdaptedEventSymbol);
                        break;
                    }
                    CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.DiscardedDependencies;
                    namedTypeSymbol.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
                    diagnostics.Add((syntaxNodeOpt == null) ? NoLocation.Singleton : syntaxNodeOpt.Location, useSiteInfo.Diagnostics);
                    EmbeddedTypesManager.Error(diagnostics, ErrorCode.ERR_MissingMethodOnSourceInterface, syntaxNodeOpt, namedTypeSymbol, base.UnderlyingEvent.AdaptedEventSymbol.MetadataName, base.UnderlyingEvent.AdaptedEventSymbol);
                }
                break;
            }
        }

        private bool EmbedMatchingInterfaceMethods(NamedTypeSymbol sourceInterface, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            bool result = false;
            ImmutableArray<Symbol>.Enumerator enumerator = sourceInterface.GetMembers(base.UnderlyingEvent.AdaptedEventSymbol.MetadataName).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind == SymbolKind.Method)
                {
                    TypeManager.EmbedMethodIfNeedTo(((MethodSymbol)current).GetCciAdapter(), syntaxNodeOpt, diagnostics);
                    result = true;
                }
            }
            return result;
        }
    }
}
