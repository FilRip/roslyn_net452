using System.Collections.Immutable;

using Microsoft.Cci;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SignatureOnlyPropertySymbol : PropertySymbol
    {
        private readonly string _name;

        private readonly TypeSymbol _containingType;

        private readonly ImmutableArray<ParameterSymbol> _parameters;

        private readonly RefKind _refKind;

        private readonly TypeWithAnnotations _type;

        private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

        private readonly bool _isStatic;

        private readonly ImmutableArray<PropertySymbol> _explicitInterfaceImplementations;

        public override RefKind RefKind => _refKind;

        public override TypeWithAnnotations TypeWithAnnotations => _type;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

        public override bool IsStatic => _isStatic;

        public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

        public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations => _explicitInterfaceImplementations;

        public override Symbol ContainingSymbol => _containingType;

        public override string Name => _name;

        internal override bool HasSpecialName
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override CallingConvention CallingConvention
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override ImmutableArray<Location> Locations
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override Accessibility DeclaredAccessibility
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override bool IsVirtual
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override bool IsOverride => false;

        public override bool IsAbstract
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override bool IsSealed
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override bool IsExtern
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override ObsoleteAttributeData ObsoleteAttributeData
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override AssemblySymbol ContainingAssembly
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override ModuleSymbol ContainingModule
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override bool MustCallMethodsDirectly
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override MethodSymbol SetMethod
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override MethodSymbol GetMethod
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override bool IsIndexer
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public SignatureOnlyPropertySymbol(string name, TypeSymbol containingType, ImmutableArray<ParameterSymbol> parameters, RefKind refKind, TypeWithAnnotations type, ImmutableArray<CustomModifier> refCustomModifiers, bool isStatic, ImmutableArray<PropertySymbol> explicitInterfaceImplementations)
        {
            _refKind = refKind;
            _type = type;
            _refCustomModifiers = refCustomModifiers;
            _isStatic = isStatic;
            _parameters = parameters;
            _explicitInterfaceImplementations = explicitInterfaceImplementations.NullToEmpty();
            _containingType = containingType;
            _name = name;
        }
    }
}
