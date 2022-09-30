using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit.NoPia;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
{
	internal sealed class EmbeddedEvent : EmbeddedTypesManager<PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter>.CommonEmbeddedEvent
	{
		protected override bool IsRuntimeSpecial => base.UnderlyingEvent.AdaptedEventSymbol.HasRuntimeSpecialName;

		protected override bool IsSpecialName => base.UnderlyingEvent.AdaptedEventSymbol.HasSpecialName;

		protected override EmbeddedType ContainingType => base.AnAccessor.ContainingType;

		protected override TypeMemberVisibility Visibility => PEModuleBuilder.MemberVisibility(base.UnderlyingEvent.AdaptedEventSymbol);

		protected override string Name => base.UnderlyingEvent.AdaptedEventSymbol.MetadataName;

		public EmbeddedEvent(EventSymbol underlyingEvent, EmbeddedMethod adder, EmbeddedMethod remover, EmbeddedMethod caller)
			: base(underlyingEvent, adder, remover, caller)
		{
		}

		protected override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
		{
			return base.UnderlyingEvent.AdaptedEventSymbol.GetCustomAttributesToEmit(moduleBuilder.CompilationState);
		}

		protected override ITypeReference GetType(PEModuleBuilder moduleBuilder, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			return moduleBuilder.Translate(base.UnderlyingEvent.AdaptedEventSymbol.Type, syntaxNodeOpt, diagnostics);
		}

		protected override void EmbedCorrespondingComEventInterfaceMethodInternal(SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, bool isUsedForComAwareEventBinding)
		{
			NamedTypeSymbol underlyingNamedType = ContainingType.UnderlyingNamedType;
			ImmutableArray<VisualBasicAttributeData>.Enumerator enumerator = underlyingNamedType.AdaptedNamedTypeSymbol.GetAttributes().GetEnumerator();
			while (enumerator.MoveNext())
			{
				VisualBasicAttributeData current = enumerator.Current;
				if (!current.IsTargetAttribute(underlyingNamedType.AdaptedNamedTypeSymbol, AttributeDescription.ComEventInterfaceAttribute))
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
						EmbeddedTypesManager.ReportDiagnostic(diagnostics, ERRID.ERR_SourceInterfaceMustBeInterface, syntaxNodeOpt, underlyingNamedType.AdaptedNamedTypeSymbol, base.UnderlyingEvent.AdaptedEventSymbol);
						break;
					}
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.DiscardedDependencies;
					namedTypeSymbol.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
					DiagnosticBagExtensions.Add(diagnostics, (syntaxNodeOpt == null) ? NoLocation.Singleton : syntaxNodeOpt.GetLocation(), useSiteInfo.Diagnostics);
					EmbeddedTypesManager.ReportDiagnostic(diagnostics, ERRID.ERR_EventNoPIANoBackingMember, syntaxNodeOpt, namedTypeSymbol, base.UnderlyingEvent.AdaptedEventSymbol.MetadataName, base.UnderlyingEvent.AdaptedEventSymbol);
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
