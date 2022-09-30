using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class SourceEventAccessorSymbol : SourceMemberMethodSymbol
    {
        private readonly SourceEventSymbol _event;

        private readonly string _name;

        private readonly ImmutableArray<MethodSymbol> _explicitInterfaceImplementations;

        private ImmutableArray<ParameterSymbol> _lazyParameters;

        private TypeWithAnnotations _lazyReturnType;

        public override string Name => _name;

        internal override bool IsExplicitInterfaceImplementation => _event.IsExplicitInterfaceImplementation;

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => _explicitInterfaceImplementations;

        public sealed override bool AreLocalsZeroed
        {
            get
            {
                if (!_event.HasSkipLocalsInitAttribute)
                {
                    return base.AreLocalsZeroed;
                }
                return false;
            }
        }

        public SourceEventSymbol AssociatedEvent => _event;

        public sealed override Symbol AssociatedSymbol => _event;

        public sealed override bool ReturnsVoid
        {
            get
            {
                LazyMethodChecks();
                return base.ReturnsVoid;
            }
        }

        public override RefKind RefKind => RefKind.None;

        public sealed override TypeWithAnnotations ReturnTypeWithAnnotations
        {
            get
            {
                LazyMethodChecks();
                return _lazyReturnType;
            }
        }

        public sealed override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        public sealed override ImmutableArray<ParameterSymbol> Parameters
        {
            get
            {
                LazyMethodChecks();
                return _lazyParameters;
            }
        }

        public sealed override bool IsVararg => false;

        public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        internal Location Location => Locations[0];

        internal override bool IsExpressionBodied => false;

        public SourceEventAccessorSymbol(SourceEventSymbol @event, SyntaxReference syntaxReference, ImmutableArray<Location> locations, EventSymbol explicitlyImplementedEventOpt, string aliasQualifierOpt, bool isAdder, bool isIterator, bool isNullableAnalysisEnabled)
            : base(@event.containingType, syntaxReference, locations, isIterator)
        {
            _event = @event;
            string text;
            ImmutableArray<MethodSymbol> explicitInterfaceImplementations;
            if ((object)explicitlyImplementedEventOpt == null)
            {
                text = SourceEventSymbol.GetAccessorName(@event.Name, isAdder);
                explicitInterfaceImplementations = ImmutableArray<MethodSymbol>.Empty;
            }
            else
            {
                MethodSymbol methodSymbol = (isAdder ? explicitlyImplementedEventOpt.AddMethod : explicitlyImplementedEventOpt.RemoveMethod);
                text = ExplicitInterfaceHelpers.GetMemberName(((object)methodSymbol != null) ? methodSymbol.Name : SourceEventSymbol.GetAccessorName(explicitlyImplementedEventOpt.Name, isAdder), explicitlyImplementedEventOpt.ContainingType, aliasQualifierOpt);
                explicitInterfaceImplementations = (((object)methodSymbol == null) ? ImmutableArray<MethodSymbol>.Empty : ImmutableArray.Create(methodSymbol));
            }
            _explicitInterfaceImplementations = explicitInterfaceImplementations;
            MakeFlags(isAdder ? MethodKind.EventAdd : MethodKind.EventRemove, @event.Modifiers, returnsVoid: false, isExtensionMethod: false, isNullableAnalysisEnabled, @event.IsExplicitInterfaceImplementation);
            _name = GetOverriddenAccessorName(@event, isAdder) ?? text;
        }

        protected sealed override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
            if (!_lazyReturnType.IsDefault)
            {
                return;
            }
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            if (_event.IsWindowsRuntimeEvent)
            {
                TypeSymbol wellKnownType = declaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken);
                Binder.ReportUseSite(wellKnownType, diagnostics, Location);
                if (MethodKind == MethodKind.EventAdd)
                {
                    _lazyReturnType = TypeWithAnnotations.Create(wellKnownType);
                    SynthesizedAccessorValueParameterSymbol item = new SynthesizedAccessorValueParameterSymbol(this, _event.TypeWithAnnotations, 0);
                    _lazyParameters = ImmutableArray.Create((ParameterSymbol)item);
                    return;
                }
                TypeSymbol specialType = declaringCompilation.GetSpecialType(SpecialType.System_Void);
                Binder.ReportUseSite(specialType, diagnostics, Location);
                _lazyReturnType = TypeWithAnnotations.Create(specialType);
                SetReturnsVoid(returnsVoid: true);
                SynthesizedAccessorValueParameterSymbol item2 = new SynthesizedAccessorValueParameterSymbol(this, TypeWithAnnotations.Create(wellKnownType), 0);
                _lazyParameters = ImmutableArray.Create((ParameterSymbol)item2);
            }
            else
            {
                TypeSymbol specialType2 = declaringCompilation.GetSpecialType(SpecialType.System_Void);
                Binder.ReportUseSite(specialType2, diagnostics, Location);
                _lazyReturnType = TypeWithAnnotations.Create(specialType2);
                SetReturnsVoid(returnsVoid: true);
                SynthesizedAccessorValueParameterSymbol item3 = new SynthesizedAccessorValueParameterSymbol(this, _event.TypeWithAnnotations, 0);
                _lazyParameters = ImmutableArray.Create((ParameterSymbol)item3);
            }
        }

        public sealed override ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
        {
            return ImmutableArray<ImmutableArray<TypeWithAnnotations>>.Empty;
        }

        public sealed override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            return ImmutableArray<TypeParameterConstraintKind>.Empty;
        }

        protected string GetOverriddenAccessorName(SourceEventSymbol @event, bool isAdder)
        {
            if (IsOverride)
            {
                EventSymbol overriddenEvent = @event.OverriddenEvent;
                if ((object)overriddenEvent != null)
                {
                    return overriddenEvent.GetOwnOrInheritedAccessor(isAdder)?.Name;
                }
            }
            return null;
        }
    }
}
