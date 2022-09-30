using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class SynthesizedMethodBaseSymbol : SourceMemberMethodSymbol
    {
        protected readonly MethodSymbol BaseMethod;

        private readonly string _name;

        private ImmutableArray<TypeParameterSymbol> _typeParameters;

        private ImmutableArray<ParameterSymbol> _parameters;

        private TypeWithAnnotations.Boxed _iteratorElementType;

        internal TypeMap TypeMap { get; private set; }

        public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

        internal override int ParameterCount => Parameters.Length;

        public sealed override ImmutableArray<ParameterSymbol> Parameters
        {
            get
            {
                if (_parameters.IsDefault)
                {
                    ImmutableInterlocked.InterlockedCompareExchange(ref _parameters, MakeParameters(), default(ImmutableArray<ParameterSymbol>));
                }
                return _parameters;
            }
        }

        protected virtual ImmutableArray<TypeSymbol> ExtraSynthesizedRefParameters => default(ImmutableArray<TypeSymbol>);

        protected virtual ImmutableArray<ParameterSymbol> BaseMethodParameters => BaseMethod.Parameters;

        internal virtual bool InheritsBaseMethodAttributes => false;

        internal sealed override MethodImplAttributes ImplementationAttributes
        {
            get
            {
                if (!InheritsBaseMethodAttributes)
                {
                    return MethodImplAttributes.IL;
                }
                return BaseMethod.ImplementationAttributes;
            }
        }

        internal sealed override MarshalPseudoCustomAttributeData? ReturnValueMarshallingInformation
        {
            get
            {
                if (!InheritsBaseMethodAttributes)
                {
                    return null;
                }
                return BaseMethod.ReturnValueMarshallingInformation;
            }
        }

        internal sealed override bool HasSpecialName
        {
            get
            {
                if (InheritsBaseMethodAttributes)
                {
                    return BaseMethod.HasSpecialName;
                }
                return false;
            }
        }

        public sealed override bool AreLocalsZeroed
        {
            get
            {
                if (BaseMethod is SourceMethodSymbol sourceMethodSymbol)
                {
                    return sourceMethodSymbol.AreLocalsZeroed;
                }
                return true;
            }
        }

        internal sealed override bool RequiresSecurityObject
        {
            get
            {
                if (InheritsBaseMethodAttributes)
                {
                    return BaseMethod.RequiresSecurityObject;
                }
                return false;
            }
        }

        internal sealed override bool HasDeclarativeSecurity
        {
            get
            {
                if (InheritsBaseMethodAttributes)
                {
                    return BaseMethod.HasDeclarativeSecurity;
                }
                return false;
            }
        }

        public sealed override RefKind RefKind => BaseMethod.RefKind;

        public sealed override TypeWithAnnotations ReturnTypeWithAnnotations => TypeMap.SubstituteType(BaseMethod.OriginalDefinition.ReturnTypeWithAnnotations);

        public sealed override FlowAnalysisAnnotations ReturnTypeFlowAnalysisAnnotations => BaseMethod.ReturnTypeFlowAnalysisAnnotations;

        public sealed override ImmutableHashSet<string> ReturnNotNullIfParameterNotNull => BaseMethod.ReturnNotNullIfParameterNotNull;

        public sealed override FlowAnalysisAnnotations FlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        public sealed override bool IsVararg => BaseMethod.IsVararg;

        public sealed override string Name => _name;

        public sealed override bool IsImplicitlyDeclared => true;

        internal override bool IsExpressionBodied => false;

        internal override TypeWithAnnotations IteratorElementTypeWithAnnotations
        {
            get
            {
                if (_iteratorElementType == null)
                {
                    Interlocked.CompareExchange(ref _iteratorElementType, new TypeWithAnnotations.Boxed(TypeMap.SubstituteType(BaseMethod.IteratorElementTypeWithAnnotations.Type)), null);
                }
                return _iteratorElementType.Value;
            }
            set
            {
                Interlocked.Exchange(ref _iteratorElementType, new TypeWithAnnotations.Boxed(value));
            }
        }

        internal override bool IsIterator => BaseMethod.IsIterator;

        protected SynthesizedMethodBaseSymbol(NamedTypeSymbol containingType, MethodSymbol baseMethod, SyntaxReference syntaxReference, Location location, string name, DeclarationModifiers declarationModifiers)
            : base(containingType, syntaxReference, location, isIterator: false)
        {
            BaseMethod = baseMethod;
            _name = name;
            MakeFlags(MethodKind.Ordinary, declarationModifiers, baseMethod.ReturnsVoid, isExtensionMethod: false, isNullableAnalysisEnabled: false);
        }

        protected void AssignTypeMapAndTypeParameters(TypeMap typeMap, ImmutableArray<TypeParameterSymbol> typeParameters)
        {
            TypeMap = typeMap;
            _typeParameters = typeParameters;
        }

        protected override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            if (!ContainingType.IsImplicitlyDeclared && !(ContainingType is SimpleProgramNamedTypeSymbol))
            {
                CSharpCompilation declaringCompilation = DeclaringCompilation;
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
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

        private ImmutableArray<ParameterSymbol> MakeParameters()
        {
            int num = 0;
            ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance();
            ImmutableArray<ParameterSymbol> baseMethodParameters = BaseMethodParameters;
            bool inheritsBaseMethodAttributes = InheritsBaseMethodAttributes;
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = baseMethodParameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                instance.Add(SynthesizedParameterSymbol.Create(this, TypeMap.SubstituteType(current.OriginalDefinition.TypeWithAnnotations), num++, current.RefKind, current.Name, default(ImmutableArray<CustomModifier>), inheritsBaseMethodAttributes ? (current as SourceComplexParameterSymbol) : null));
            }
            ImmutableArray<TypeSymbol> extraSynthesizedRefParameters = ExtraSynthesizedRefParameters;
            if (!extraSynthesizedRefParameters.IsDefaultOrEmpty)
            {
                ImmutableArray<TypeSymbol>.Enumerator enumerator2 = extraSynthesizedRefParameters.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    TypeSymbol current2 = enumerator2.Current;
                    instance.Add(SynthesizedParameterSymbol.Create(this, TypeMap.SubstituteType(current2), num++, RefKind.Ref));
                }
            }
            return instance.ToImmutableAndFree();
        }

        public sealed override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            if (!InheritsBaseMethodAttributes)
            {
                return ImmutableArray<CSharpAttributeData>.Empty;
            }
            return BaseMethod.GetAttributes();
        }

        public sealed override ImmutableArray<CSharpAttributeData> GetReturnTypeAttributes()
        {
            if (!InheritsBaseMethodAttributes)
            {
                return ImmutableArray<CSharpAttributeData>.Empty;
            }
            return BaseMethod.GetReturnTypeAttributes();
        }

        public sealed override DllImportData? GetDllImportData()
        {
            if (!InheritsBaseMethodAttributes)
            {
                return null;
            }
            return BaseMethod.GetDllImportData();
        }

        internal sealed override IEnumerable<SecurityAttribute> GetSecurityInformation()
        {
            if (!InheritsBaseMethodAttributes)
            {
                return SpecializedCollections.EmptyEnumerable<SecurityAttribute>();
            }
            return BaseMethod.GetSecurityInformation();
        }
    }
}
