using System.Collections.Immutable;
using System.Threading;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SubstitutedPropertySymbol : WrappedPropertySymbol
    {
        private readonly SubstitutedNamedTypeSymbol _containingType;

        private TypeWithAnnotations.Boxed _lazyType;

        private ImmutableArray<ParameterSymbol> _lazyParameters;

        private ImmutableArray<PropertySymbol> _lazyExplicitInterfaceImplementations;

        private OverriddenOrHiddenMembersResult _lazyOverriddenOrHiddenMembers;

        public override TypeWithAnnotations TypeWithAnnotations
        {
            get
            {
                if (_lazyType == null)
                {
                    TypeWithAnnotations value = _containingType.TypeSubstitution.SubstituteType(OriginalDefinition.TypeWithAnnotations);
                    Interlocked.CompareExchange(ref _lazyType, new TypeWithAnnotations.Boxed(value), null);
                }
                return _lazyType.Value;
            }
        }

        public override Symbol ContainingSymbol => _containingType;

        public override NamedTypeSymbol ContainingType => _containingType;

        public override PropertySymbol OriginalDefinition => _underlyingProperty;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => _containingType.TypeSubstitution.SubstituteCustomModifiers(OriginalDefinition.RefCustomModifiers);

        public override ImmutableArray<ParameterSymbol> Parameters
        {
            get
            {
                if (_lazyParameters.IsDefault)
                {
                    ImmutableInterlocked.InterlockedCompareExchange(ref _lazyParameters, SubstituteParameters(), default(ImmutableArray<ParameterSymbol>));
                }
                return _lazyParameters;
            }
        }

        public override MethodSymbol GetMethod => OriginalDefinition.GetMethod?.AsMember(_containingType);

        public override MethodSymbol SetMethod => OriginalDefinition.SetMethod?.AsMember(_containingType);

        internal override bool IsExplicitInterfaceImplementation => OriginalDefinition.IsExplicitInterfaceImplementation;

        public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations
        {
            get
            {
                if (_lazyExplicitInterfaceImplementations.IsDefault)
                {
                    ImmutableInterlocked.InterlockedCompareExchange(ref _lazyExplicitInterfaceImplementations, ExplicitInterfaceHelpers.SubstituteExplicitInterfaceImplementations(OriginalDefinition.ExplicitInterfaceImplementations, _containingType.TypeSubstitution), default(ImmutableArray<PropertySymbol>));
                }
                return _lazyExplicitInterfaceImplementations;
            }
        }

        internal override bool MustCallMethodsDirectly => OriginalDefinition.MustCallMethodsDirectly;

        internal override OverriddenOrHiddenMembersResult OverriddenOrHiddenMembers
        {
            get
            {
                if (_lazyOverriddenOrHiddenMembers == null)
                {
                    Interlocked.CompareExchange(ref _lazyOverriddenOrHiddenMembers, this.MakeOverriddenOrHiddenMembers(), null);
                }
                return _lazyOverriddenOrHiddenMembers;
            }
        }

        internal SubstitutedPropertySymbol(SubstitutedNamedTypeSymbol containingType, PropertySymbol originalDefinition)
            : base(originalDefinition)
        {
            _containingType = containingType;
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return OriginalDefinition.GetAttributes();
        }

        private ImmutableArray<ParameterSymbol> SubstituteParameters()
        {
            ImmutableArray<ParameterSymbol> parameters = OriginalDefinition.Parameters;
            if (parameters.IsEmpty)
            {
                return parameters;
            }
            int length = parameters.Length;
            ParameterSymbol[] array = new ParameterSymbol[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = new SubstitutedParameterSymbol(this, _containingType.TypeSubstitution, parameters[i]);
            }
            return array.AsImmutableOrNull();
        }
    }
}
