using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

using Microsoft.Cci;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedIntrinsicOperatorSymbol : MethodSymbol
    {
        private sealed class SynthesizedOperatorParameterSymbol : SynthesizedParameterSymbolBase
        {
            public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

            internal override MarshalPseudoCustomAttributeData MarshallingInformation => null;

            public SynthesizedOperatorParameterSymbol(SynthesizedIntrinsicOperatorSymbol container, TypeSymbol type, int ordinal, string name)
                : base(container, TypeWithAnnotations.Create(type), ordinal, RefKind.None, name)
            {
            }

            public override bool Equals(Symbol obj, TypeCompareKind compareKind)
            {
                if ((object)obj == this)
                {
                    return true;
                }
                if (!(obj is SynthesizedOperatorParameterSymbol synthesizedOperatorParameterSymbol))
                {
                    return false;
                }
                if (Ordinal == synthesizedOperatorParameterSymbol.Ordinal)
                {
                    return ContainingSymbol!.Equals(synthesizedOperatorParameterSymbol.ContainingSymbol, compareKind);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Hash.Combine(ContainingSymbol, Ordinal.GetHashCode());
            }
        }

        private readonly TypeSymbol _containingType;

        private readonly string _name;

        private readonly ImmutableArray<ParameterSymbol> _parameters;

        private readonly TypeSymbol _returnType;

        private readonly bool _isCheckedBuiltin;

        public override string Name => _name;

        public override bool IsCheckedBuiltin => _isCheckedBuiltin;

        public override MethodKind MethodKind => MethodKind.BuiltinOperator;

        public override bool IsImplicitlyDeclared => true;

        internal override CSharpCompilation DeclaringCompilation => null;

        internal override bool IsMetadataFinal => false;

        public override int Arity => 0;

        public override bool IsExtensionMethod => false;

        internal override bool HasSpecialName => true;

        internal override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.IL;

        internal override bool HasDeclarativeSecurity => false;

        public override bool AreLocalsZeroed
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override MarshalPseudoCustomAttributeData ReturnValueMarshallingInformation => null;

        internal override bool RequiresSecurityObject => false;

        public override bool HidesBaseMethodsByName => false;

        public override bool IsVararg => false;

        public override bool ReturnsVoid => false;

        public override bool IsAsync => false;

        public override RefKind RefKind => RefKind.None;

        public override TypeWithAnnotations ReturnTypeWithAnnotations => TypeWithAnnotations.Create(_returnType);

        public override FlowAnalysisAnnotations ReturnTypeFlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        public override ImmutableHashSet<string> ReturnNotNullIfParameterNotNull => ImmutableHashSet<string>.Empty;

        public override FlowAnalysisAnnotations FlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        public override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotations => ImmutableArray<TypeWithAnnotations>.Empty;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

        internal override bool IsDeclaredReadOnly => false;

        internal override bool IsInitOnly => false;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        public override Symbol AssociatedSymbol => null;

        internal override CallingConvention CallingConvention => CallingConvention.Default;

        internal override bool GenerateDebugInfo => false;

        public override Symbol ContainingSymbol => _containingType;

        public override NamedTypeSymbol ContainingType => _containingType as NamedTypeSymbol;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override Accessibility DeclaredAccessibility => Accessibility.Public;

        public override bool IsStatic => true;

        public override bool IsVirtual => false;

        public override bool IsOverride => false;

        public override bool IsAbstract => false;

        public override bool IsSealed => false;

        public override bool IsExtern => false;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        public SynthesizedIntrinsicOperatorSymbol(TypeSymbol leftType, string name, TypeSymbol rightType, TypeSymbol returnType, bool isCheckedBuiltin)
        {
            if (leftType.Equals(rightType, TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
            {
                _containingType = leftType;
            }
            else if (rightType.Equals(returnType, TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
            {
                _containingType = rightType;
            }
            else
            {
                _containingType = leftType;
            }
            _name = name;
            _returnType = returnType;
            _parameters = new ParameterSymbol[2]
            {
                new SynthesizedOperatorParameterSymbol(this, leftType, 0, "left"),
                new SynthesizedOperatorParameterSymbol(this, rightType, 1, "right")
            }.AsImmutableOrNull();
            _isCheckedBuiltin = isCheckedBuiltin;
        }

        public SynthesizedIntrinsicOperatorSymbol(TypeSymbol container, string name, TypeSymbol returnType, bool isCheckedBuiltin)
        {
            _containingType = container;
            _name = name;
            _returnType = returnType;
            _parameters = new ParameterSymbol[1]
            {
                new SynthesizedOperatorParameterSymbol(this, container, 0, "value")
            }.AsImmutableOrNull();
            _isCheckedBuiltin = isCheckedBuiltin;
        }

        public override string GetDocumentationCommentId()
        {
            return null;
        }

        internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        internal override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        public override DllImportData GetDllImportData()
        {
            return null;
        }

        internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
        {
            return SpecializedCollections.EmptyEnumerable<SecurityAttribute>();
        }

        internal override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return ImmutableArray<string>.Empty;
        }

        internal sealed override UnmanagedCallersOnlyAttributeData GetUnmanagedCallersOnlyAttributeData(bool forceComplete)
        {
            return null;
        }

        internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal sealed override bool IsNullableAnalysisEnabled()
        {
            return false;
        }

        public override bool Equals(Symbol obj, TypeCompareKind compareKind)
        {
            if ((object)obj == this)
            {
                return true;
            }
            if (!(obj is SynthesizedIntrinsicOperatorSymbol synthesizedIntrinsicOperatorSymbol))
            {
                return false;
            }
            if (_isCheckedBuiltin == synthesizedIntrinsicOperatorSymbol._isCheckedBuiltin && _parameters.Length == synthesizedIntrinsicOperatorSymbol._parameters.Length && string.Equals(_name, synthesizedIntrinsicOperatorSymbol._name, StringComparison.Ordinal) && TypeSymbol.Equals(_containingType, synthesizedIntrinsicOperatorSymbol._containingType, compareKind) && TypeSymbol.Equals(_returnType, synthesizedIntrinsicOperatorSymbol._returnType, compareKind))
            {
                for (int i = 0; i < _parameters.Length; i++)
                {
                    if (!TypeSymbol.Equals(_parameters[i].Type, synthesizedIntrinsicOperatorSymbol._parameters[i].Type, compareKind))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(_name, Hash.Combine(_containingType, _parameters.Length));
        }
    }
}
