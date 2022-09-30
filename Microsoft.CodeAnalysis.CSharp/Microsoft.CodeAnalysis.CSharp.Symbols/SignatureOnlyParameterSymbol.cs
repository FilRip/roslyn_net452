using System.Collections.Immutable;
using System.Linq;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SignatureOnlyParameterSymbol : ParameterSymbol
    {
        private readonly TypeWithAnnotations _type;

        private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

        private readonly bool _isParams;

        private readonly RefKind _refKind;

        public override TypeWithAnnotations TypeWithAnnotations => _type;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

        public override bool IsParams => _isParams;

        public override RefKind RefKind => _refKind;

        public override string Name => "";

        public override bool IsImplicitlyDeclared => true;

        public override bool IsDiscard => false;

        internal override bool IsMetadataIn
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override bool IsMetadataOut
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override MarshalPseudoCustomAttributeData MarshallingInformation
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override int Ordinal
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override bool IsMetadataOptional
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override ConstantValue ExplicitDefaultConstantValue
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override bool IsIDispatchConstant
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override bool IsIUnknownConstant
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override bool IsCallerFilePath
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override bool IsCallerLineNumber
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override bool IsCallerMemberName
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override FlowAnalysisAnnotations FlowAnalysisAnnotations
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override ImmutableHashSet<string> NotNullIfParameterNotNull
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override Symbol ContainingSymbol
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

        public SignatureOnlyParameterSymbol(TypeWithAnnotations type, ImmutableArray<CustomModifier> refCustomModifiers, bool isParams, RefKind refKind)
        {
            _type = type;
            _refCustomModifiers = refCustomModifiers;
            _isParams = isParams;
            _refKind = refKind;
        }

        public override bool Equals(Symbol obj, TypeCompareKind compareKind)
        {
            if ((object)this == obj)
            {
                return true;
            }
            if (obj is SignatureOnlyParameterSymbol signatureOnlyParameterSymbol && TypeSymbol.Equals(_type.Type, signatureOnlyParameterSymbol._type.Type, compareKind) && _type.CustomModifiers.Equals(signatureOnlyParameterSymbol._type.CustomModifiers) && _refCustomModifiers.SequenceEqual(signatureOnlyParameterSymbol._refCustomModifiers) && _isParams == signatureOnlyParameterSymbol._isParams)
            {
                return _refKind == signatureOnlyParameterSymbol._refKind;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(_type.Type.GetHashCode(), Hash.Combine(Hash.CombineValues(_type.CustomModifiers), Hash.Combine(_isParams.GetHashCode(), _refKind.GetHashCode())));
        }
    }
}
