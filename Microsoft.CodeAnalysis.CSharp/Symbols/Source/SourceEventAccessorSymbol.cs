// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    /// <summary>
    /// Base class for event accessors - synthesized and user defined.
    /// </summary>
    internal abstract class SourceEventAccessorSymbol : SourceMemberMethodSymbol
    {
        private readonly SourceEventSymbol _event;
        private readonly string _name;
        private readonly ImmutableArray<MethodSymbol> _explicitInterfaceImplementations;

        private ImmutableArray<ParameterSymbol> _lazyParameters;
        private TypeWithAnnotations _lazyReturnType;

        public SourceEventAccessorSymbol(
            SourceEventSymbol @event,
            SyntaxReference syntaxReference,
            ImmutableArray<Location> locations,
            EventSymbol explicitlyImplementedEventOpt,
            string aliasQualifierOpt,
            bool isAdder,
            bool isIterator,
            bool isNullableAnalysisEnabled)
            : base(@event.containingType, syntaxReference, locations, isIterator)
        {
            _event = @event;

            string name;
            ImmutableArray<MethodSymbol> explicitInterfaceImplementations;
            if (explicitlyImplementedEventOpt is null)
            {
                name = SourceEventSymbol.GetAccessorName(@event.Name, isAdder);
                explicitInterfaceImplementations = ImmutableArray<MethodSymbol>.Empty;
            }
            else
            {
                MethodSymbol implementedAccessor = isAdder ? explicitlyImplementedEventOpt.AddMethod : explicitlyImplementedEventOpt.RemoveMethod;
                string accessorName = implementedAccessor is not null ? implementedAccessor.Name : SourceEventSymbol.GetAccessorName(explicitlyImplementedEventOpt.Name, isAdder);

                name = ExplicitInterfaceHelpers.GetMemberName(accessorName, explicitlyImplementedEventOpt.ContainingType, aliasQualifierOpt);
                explicitInterfaceImplementations = implementedAccessor is null ? ImmutableArray<MethodSymbol>.Empty : ImmutableArray.Create<MethodSymbol>(implementedAccessor);
            }

            _explicitInterfaceImplementations = explicitInterfaceImplementations;

            this.MakeFlags(
                isAdder ? MethodKind.EventAdd : MethodKind.EventRemove,
                @event.Modifiers,
                returnsVoid: false, // until we learn otherwise (in LazyMethodChecks).
                isExtensionMethod: false,
                isNullableAnalysisEnabled: isNullableAnalysisEnabled,
                isMetadataVirtualIgnoringModifiers: @event.IsExplicitInterfaceImplementation);

            _name = GetOverriddenAccessorName(@event, isAdder) ?? name;
        }

        public override string Name
        {
            get { return _name; }
        }

        internal override bool IsExplicitInterfaceImplementation
        {
            get { return _event.IsExplicitInterfaceImplementation; }
        }

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
        {
            get { return _explicitInterfaceImplementations; }
        }

        public sealed override bool AreLocalsZeroed
            => !_event.HasSkipLocalsInitAttribute && base.AreLocalsZeroed;

        public SourceEventSymbol AssociatedEvent
        {
            get { return _event; }
        }

        public sealed override Symbol AssociatedSymbol
        {
            get { return _event; }
        }

        protected sealed override void MethodChecks(BindingDiagnosticBag diagnostics)
        {

            // CONSIDER: currently, we're copying the custom modifiers of the event overridden
            // by this method's associated event (by using the associated event's type, which is
            // copied from the overridden event).  It would be more correct to copy them from
            // the specific accessor that this method is overriding (as in SourceMemberMethodSymbol).

            if (_lazyReturnType.IsDefault)
            {
                CSharpCompilation compilation = this.DeclaringCompilation;

                // NOTE: LazyMethodChecks calls us within a lock, so we use regular assignments,
                // rather than Interlocked.CompareExchange.
                if (_event.IsWindowsRuntimeEvent)
                {
                    TypeSymbol eventTokenType = compilation.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken);
                    Binder.ReportUseSite(eventTokenType, diagnostics, this.Location);

                    if (this.MethodKind == MethodKind.EventAdd)
                    {
                        // EventRegistrationToken add_E(EventDelegate d);

                        // Leave the returns void bit in this.flags false.
                        _lazyReturnType = TypeWithAnnotations.Create(eventTokenType);

                        var parameter = new SynthesizedAccessorValueParameterSymbol(this, _event.TypeWithAnnotations, 0);
                        _lazyParameters = ImmutableArray.Create<ParameterSymbol>(parameter);
                    }
                    else
                    {

                        // void remove_E(EventRegistrationToken t);

                        TypeSymbol voidType = compilation.GetSpecialType(SpecialType.System_Void);
                        Binder.ReportUseSite(voidType, diagnostics, this.Location);
                        _lazyReturnType = TypeWithAnnotations.Create(voidType);
                        this.SetReturnsVoid(returnsVoid: true);

                        var parameter = new SynthesizedAccessorValueParameterSymbol(this, TypeWithAnnotations.Create(eventTokenType), 0);
                        _lazyParameters = ImmutableArray.Create<ParameterSymbol>(parameter);
                    }
                }
                else
                {
                    // void add_E(EventDelegate d);
                    // void remove_E(EventDelegate d);

                    TypeSymbol voidType = compilation.GetSpecialType(SpecialType.System_Void);
                    Binder.ReportUseSite(voidType, diagnostics, this.Location);
                    _lazyReturnType = TypeWithAnnotations.Create(voidType);
                    this.SetReturnsVoid(returnsVoid: true);

                    var parameter = new SynthesizedAccessorValueParameterSymbol(this, _event.TypeWithAnnotations, 0);
                    _lazyParameters = ImmutableArray.Create<ParameterSymbol>(parameter);
                }
            }
        }

        public sealed override bool ReturnsVoid
        {
            get
            {
                LazyMethodChecks();
                return base.ReturnsVoid;
            }
        }

        public override RefKind RefKind
        {
            get { return RefKind.None; }
        }

        public sealed override TypeWithAnnotations ReturnTypeWithAnnotations
        {
            get
            {
                LazyMethodChecks();
                return _lazyReturnType;
            }
        }

        public sealed override ImmutableArray<CustomModifier> RefCustomModifiers
        {
            get
            {
                return ImmutableArray<CustomModifier>.Empty; // Same as base, but this is clear and explicit.
            }
        }

        public sealed override ImmutableArray<ParameterSymbol> Parameters
        {
            get
            {
                LazyMethodChecks();
                return _lazyParameters;
            }
        }

        public sealed override bool IsVararg
        {
            get { return false; }
        }

        public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters
        {
            get { return ImmutableArray<TypeParameterSymbol>.Empty; }
        }

        public sealed override ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
            => ImmutableArray<ImmutableArray<TypeWithAnnotations>>.Empty;

        public sealed override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
            => ImmutableArray<TypeParameterConstraintKind>.Empty;

        internal Location Location
        {
            get
            {
                return this.Locations[0];
            }
        }

        protected string GetOverriddenAccessorName(SourceEventSymbol @event, bool isAdder)
        {
            if (this.IsOverride)
            {
                // NOTE: What we'd really like to do is ask for the OverriddenMethod of this symbol.
                // Unfortunately, we can't do that, because it would inspect the signature of this
                // method, which depends on whether @event is a WinRT event, which depends on
                // interface implementation, which we can't check during construction of the 
                // member list of the type containing this accessor (infinite recursion).  Instead,
                // we inline part of the implementation of OverriddenMethod - we look for the
                // overridden event (which does not depend on WinRT-ness) and then grab the corresponding
                // accessor.
                EventSymbol overriddenEvent = @event.OverriddenEvent;
                if (overriddenEvent is not null)
                {
                    // If this accessor is overriding an accessor from metadata, it is possible that
                    // the name of the overridden accessor doesn't follow the C# add_X/remove_X pattern.
                    // We should copy the name so that the runtime will recognize this as an override.
                    MethodSymbol overriddenAccessor = overriddenEvent.GetOwnOrInheritedAccessor(isAdder);
                    return overriddenAccessor?.Name;
                }
            }

            return null;
        }

        internal override bool IsExpressionBodied
        {
            // Events cannot be expression-bodied
            get { return false; }
        }
    }
}
