using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;

using Microsoft.Cci;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SignatureOnlyMethodSymbol : MethodSymbol
    {
        private readonly string _name;

        private readonly TypeSymbol _containingType;

        private readonly MethodKind _methodKind;

        private readonly CallingConvention _callingConvention;

        private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

        private readonly ImmutableArray<ParameterSymbol> _parameters;

        private readonly RefKind _refKind;

        private readonly bool _isInitOnly;

        private readonly TypeWithAnnotations _returnType;

        private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

        private readonly ImmutableArray<MethodSymbol> _explicitInterfaceImplementations;

        internal override CallingConvention CallingConvention => _callingConvention;

        public override bool IsVararg => new SignatureHeader((byte)_callingConvention).CallingConvention == SignatureCallingConvention.VarArgs;

        public override bool IsGenericMethod => Arity > 0;

        public override int Arity => _typeParameters.Length;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

        public override bool ReturnsVoid => _returnType.IsVoidType();

        public override RefKind RefKind => _refKind;

        public override TypeWithAnnotations ReturnTypeWithAnnotations => _returnType;

        public override FlowAnalysisAnnotations ReturnTypeFlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        public override ImmutableHashSet<string> ReturnNotNullIfParameterNotNull => ImmutableHashSet<string>.Empty;

        public override FlowAnalysisAnnotations FlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

        public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => _explicitInterfaceImplementations;

        public override Symbol ContainingSymbol => _containingType;

        public override MethodKind MethodKind => _methodKind;

        public override string Name => _name;

        internal override bool GenerateDebugInfo
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override bool HasSpecialName
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override MethodImplAttributes ImplementationAttributes
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override bool RequiresSecurityObject
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override MarshalPseudoCustomAttributeData ReturnValueMarshallingInformation
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override bool HasDeclarativeSecurity
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

        public override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotations
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override Symbol AssociatedSymbol
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override bool IsExtensionMethod
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override bool HidesBaseMethodsByName
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

        public override bool IsStatic
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override bool IsAsync
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

        public override bool IsOverride
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

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

        public override bool AreLocalsZeroed
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

        internal override bool IsMetadataFinal
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override bool IsDeclaredReadOnly => false;

        internal override bool IsInitOnly => _isInitOnly;

        public SignatureOnlyMethodSymbol(string name, TypeSymbol containingType, MethodKind methodKind, CallingConvention callingConvention, ImmutableArray<TypeParameterSymbol> typeParameters, ImmutableArray<ParameterSymbol> parameters, RefKind refKind, bool isInitOnly, TypeWithAnnotations returnType, ImmutableArray<CustomModifier> refCustomModifiers, ImmutableArray<MethodSymbol> explicitInterfaceImplementations)
        {
            _callingConvention = callingConvention;
            _typeParameters = typeParameters;
            _refKind = refKind;
            _isInitOnly = isInitOnly;
            _returnType = returnType;
            _refCustomModifiers = refCustomModifiers;
            _parameters = parameters;
            _explicitInterfaceImplementations = explicitInterfaceImplementations.NullToEmpty();
            _containingType = containingType;
            _methodKind = methodKind;
            _name = name;
        }

        internal sealed override bool IsNullableAnalysisEnabled()
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override DllImportData GetDllImportData()
        {
            return null;
        }

        internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal sealed override UnmanagedCallersOnlyAttributeData GetUnmanagedCallersOnlyAttributeData(bool forceComplete)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal sealed override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
