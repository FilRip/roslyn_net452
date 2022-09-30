using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class CustomEventAccessorSymbol : SourceNonPropertyAccessorMethodSymbol
	{
		private readonly SourceEventSymbol _event;

		private readonly string _name;

		private ImmutableArray<MethodSymbol> _lazyExplicitImplementations;

		private static readonly Binder.CheckParameterModifierDelegate s_checkAddRemoveParameterModifierCallback = CheckAddRemoveParameterModifier;

		private static readonly Binder.CheckParameterModifierDelegate s_checkRaiseParameterModifierCallback = CheckEventMethodParameterModifier;

		public override string Name => _name;

		public override string MetadataName => Binder.GetAccessorName(_event.MetadataName, MethodKind, isWinMd: false);

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

		public override Accessibility DeclaredAccessibility
		{
			get
			{
				if (MethodKind == MethodKind.EventRaise)
				{
					return Accessibility.Private;
				}
				return _event.DeclaredAccessibility;
			}
		}

		public override Symbol AssociatedSymbol => _event;

		internal override bool ShadowsExplicitly => _event.ShadowsExplicitly;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
		{
			get
			{
				if (_lazyExplicitImplementations.IsDefault)
				{
					ImmutableInterlocked.InterlockedCompareExchange(ref _lazyExplicitImplementations, _event.GetAccessorImplementations(MethodKind), default(ImmutableArray<MethodSymbol>));
				}
				return _lazyExplicitImplementations;
			}
		}

		public override MethodSymbol OverriddenMethod => null;

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override bool IsExtensionMethod => false;

		internal override bool MayBeReducibleExtensionMethod => false;

		public override bool IsSub
		{
			get
			{
				if (MethodKind == MethodKind.EventAdd)
				{
					return !_event.IsWindowsRuntimeEvent;
				}
				return true;
			}
		}

		internal CustomEventAccessorSymbol(SourceMemberContainerTypeSymbol container, SourceEventSymbol @event, string name, SourceMemberFlags flags, SyntaxReference syntaxRef, Location location)
			: base(container, flags, syntaxRef, ImmutableArray.Create(location))
		{
			_event = @event;
			_name = name;
		}

		protected override ImmutableArray<ParameterSymbol> GetParameters(SourceModuleSymbol sourceModule, BindingDiagnosticBag diagBag)
		{
			SourceMemberContainerTypeSymbol typeSymbol = (SourceMemberContainerTypeSymbol)ContainingType;
			Binder containingBinder = BinderBuilder.CreateBinderForType(sourceModule, base.SyntaxTree, typeSymbol);
			containingBinder = new LocationSpecificBinder(BindingLocation.EventAccessorSignature, this, containingBinder);
			return BindParameters(Locations.FirstOrDefault(), containingBinder, base.BlockSyntax.BlockStatement.ParameterList, diagBag);
		}

		protected override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
		{
			return OneOrMany.Create(base.AttributeDeclarationSyntaxList);
		}

		protected override OneOrMany<SyntaxList<AttributeListSyntax>> GetReturnTypeAttributeDeclarations()
		{
			return default(OneOrMany<SyntaxList<AttributeListSyntax>>);
		}

		private ImmutableArray<ParameterSymbol> BindParameters(Location location, Binder binder, ParameterListSyntax parameterListOpt, BindingDiagnosticBag diagnostics)
		{
			SeparatedSyntaxList<ParameterSyntax> syntax = parameterListOpt?.Parameters ?? default(SeparatedSyntaxList<ParameterSyntax>);
			ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance(syntax.Count);
			binder.DecodeParameterList(this, isFromLambda: false, SourceMemberFlags.None, syntax, instance, (MethodKind == MethodKind.EventRaise) ? s_checkRaiseParameterModifierCallback : s_checkAddRemoveParameterModifierCallback, diagnostics);
			ImmutableArray<ParameterSymbol> immutableArray = instance.ToImmutableAndFree();
			if (MethodKind == MethodKind.EventRaise)
			{
				if (_event.Type is NamedTypeSymbol namedTypeSymbol && !TypeSymbolExtensions.IsErrorType(namedTypeSymbol))
				{
					MethodSymbol delegateInvokeMethod = namedTypeSymbol.DelegateInvokeMethod;
					if ((object)delegateInvokeMethod != null && delegateInvokeMethod.IsSub)
					{
						CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = binder.GetNewCompoundUseSiteInfo(diagnostics);
						MethodConversionKind methodConversion = Conversions.ClassifyMethodConversionForEventRaise(delegateInvokeMethod, immutableArray, ref useSiteInfo);
						if (!diagnostics.Add(location, useSiteInfo) && (!Conversions.IsDelegateRelaxationSupportedFor(methodConversion) || (binder.OptionStrict == OptionStrict.On && Conversions.IsNarrowingMethodConversion(methodConversion, isForAddressOf: false))))
						{
							diagnostics.Add(ERRID.ERR_RaiseEventShapeMismatch1, location, namedTypeSymbol);
						}
					}
				}
			}
			else if (immutableArray.Length != 1)
			{
				diagnostics.Add(ERRID.ERR_EventAddRemoveHasOnlyOneParam, location);
			}
			else
			{
				TypeSymbol type = _event.Type;
				TypeSymbol type2 = immutableArray[0].Type;
				if (MethodKind == MethodKind.EventAdd)
				{
					if (!TypeSymbolExtensions.IsErrorType(type) && !TypeSymbol.Equals(type, type2, TypeCompareKind.ConsiderEverything))
					{
						ERRID code = (_event.IsWindowsRuntimeEvent ? ERRID.ERR_AddParamWrongForWinRT : ERRID.ERR_AddRemoveParamNotEventType);
						diagnostics.Add(code, location);
					}
				}
				else if (_event.ExplicitInterfaceImplementations.Any())
				{
					NamedTypeSymbol wellKnownType = binder.Compilation.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken);
					EventSymbol eventSymbol = _event.ExplicitInterfaceImplementations[0];
					if (!TypeSymbolExtensions.IsErrorType(wellKnownType) && eventSymbol.IsWindowsRuntimeEvent != TypeSymbol.Equals(type2, wellKnownType, TypeCompareKind.ConsiderEverything))
					{
						diagnostics.Add(ERRID.ERR_EventImplRemoveHandlerParamWrong, location, _event.Name, eventSymbol.Name, eventSymbol.ContainingType);
					}
				}
				else if (_event.IsWindowsRuntimeEvent)
				{
					NamedTypeSymbol wellKnownType2 = binder.Compilation.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken);
					if (!TypeSymbolExtensions.IsErrorType(wellKnownType2) && !TypeSymbol.Equals(type2, wellKnownType2, TypeCompareKind.ConsiderEverything))
					{
						diagnostics.Add(ERRID.ERR_RemoveParamWrongForWinRT, location);
					}
				}
				else if (!TypeSymbolExtensions.IsErrorType(type) && !TypeSymbol.Equals(type, type2, TypeCompareKind.ConsiderEverything))
				{
					diagnostics.Add(ERRID.ERR_AddRemoveParamNotEventType, location);
				}
			}
			return immutableArray;
		}

		private static SourceParameterFlags CheckEventMethodParameterModifier(Symbol container, SyntaxToken token, SourceParameterFlags flag, BindingDiagnosticBag diagnostics)
		{
			if ((flag & SourceParameterFlags.Optional) != 0)
			{
				Location location = token.GetLocation();
				diagnostics.Add(ERRID.ERR_EventMethodOptionalParamIllegal1, location, token.ToString());
				flag &= (SourceParameterFlags)251;
			}
			if ((flag & SourceParameterFlags.ParamArray) != 0)
			{
				Location location2 = token.GetLocation();
				diagnostics.Add(ERRID.ERR_EventMethodOptionalParamIllegal1, location2, token.ToString());
				flag &= (SourceParameterFlags)247;
			}
			return flag;
		}

		private static SourceParameterFlags CheckAddRemoveParameterModifier(Symbol container, SyntaxToken token, SourceParameterFlags flag, BindingDiagnosticBag diagnostics)
		{
			if ((flag & SourceParameterFlags.ByRef) != 0)
			{
				Location location = token.GetLocation();
				diagnostics.Add(ERRID.ERR_EventAddRemoveByrefParamIllegal, location, token.ToString());
				flag &= (SourceParameterFlags)253;
			}
			return CheckEventMethodParameterModifier(container, token, flag, diagnostics);
		}
	}
}
