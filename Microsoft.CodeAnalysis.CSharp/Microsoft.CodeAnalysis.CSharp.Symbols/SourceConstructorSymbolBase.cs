using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class SourceConstructorSymbolBase : SourceMemberMethodSymbol
    {
        protected ImmutableArray<ParameterSymbol> _lazyParameters;

        private TypeWithAnnotations _lazyReturnType;

        private bool _lazyIsVararg;

        protected abstract bool AllowRefOrOut { get; }

        public sealed override bool IsVararg
        {
            get
            {
                LazyMethodChecks();
                return _lazyIsVararg;
            }
        }

        public sealed override bool IsImplicitlyDeclared => base.IsImplicitlyDeclared;

        internal sealed override int ParameterCount
        {
            get
            {
                if (!_lazyParameters.IsDefault)
                {
                    return _lazyParameters.Length;
                }
                return GetParameterList().ParameterCount;
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

        public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        public override RefKind RefKind => RefKind.None;

        public sealed override TypeWithAnnotations ReturnTypeWithAnnotations
        {
            get
            {
                LazyMethodChecks();
                return _lazyReturnType;
            }
        }

        public sealed override string Name
        {
            get
            {
                if (!IsStatic)
                {
                    return ".ctor";
                }
                return ".cctor";
            }
        }

        protected sealed override IAttributeTargetSymbol AttributeOwner => base.AttributeOwner;

        internal sealed override bool GenerateDebugInfo => true;

        protected SourceConstructorSymbolBase(SourceMemberContainerTypeSymbol containingType, Location location, CSharpSyntaxNode syntax, bool isIterator)
            : base(containingType, syntax.GetReference(), ImmutableArray.Create(location), isIterator)
        {
        }

        protected sealed override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
            CSharpSyntaxNode cSharpSyntaxNode = (CSharpSyntaxNode)syntaxReferenceOpt.GetSyntax();
            BinderFactory binderFactory = DeclaringCompilation.GetBinderFactory(cSharpSyntaxNode.SyntaxTree);
            ParameterListSyntax parameterList = GetParameterList();
            Binder binder = binderFactory.GetBinder(parameterList, cSharpSyntaxNode, this).WithContainingMemberOrLambda(this);
            Binder binder2 = binder.WithAdditionalFlagsAndContainingMemberOrLambda(BinderFlags.SuppressConstraintChecks, this);
            bool allowRefOrOut = AllowRefOrOut;
            _lazyParameters = ParameterHelpers.MakeParameters(binder2, this, parameterList, out var arglistToken, diagnostics, allowRefOrOut, allowThis: false, addRefReadOnlyModifier: false);
            _lazyIsVararg = arglistToken.Kind() == SyntaxKind.ArgListKeyword;
            _lazyReturnType = TypeWithAnnotations.Create(binder.GetSpecialType(SpecialType.System_Void, diagnostics, cSharpSyntaxNode));
            Location location = Locations[0];
            if (MethodKind == MethodKind.StaticConstructor && _lazyParameters.Length != 0 && ContainingType.Name == ((ConstructorDeclarationSyntax)SyntaxNode).Identifier.ValueText)
            {
                diagnostics.Add(ErrorCode.ERR_StaticConstParam, location, this);
            }
            CheckEffectiveAccessibility(_lazyReturnType, _lazyParameters, diagnostics);
            if (_lazyIsVararg && (IsGenericMethod || ContainingType.IsGenericType || (_lazyParameters.Length > 0 && _lazyParameters[_lazyParameters.Length - 1].IsParams)))
            {
                diagnostics.Add(ErrorCode.ERR_BadVarargs, location);
            }
        }

        protected abstract ParameterListSyntax GetParameterList();

        internal sealed override void AfterAddingTypeMembersChecks(ConversionsBase conversions, BindingDiagnosticBag diagnostics)
        {
            base.AfterAddingTypeMembersChecks(conversions, diagnostics);
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            ParameterHelpers.EnsureIsReadOnlyAttributeExists(declaringCompilation, Parameters, diagnostics, modifyCompilation: true);
            ParameterHelpers.EnsureNativeIntegerAttributeExists(declaringCompilation, Parameters, diagnostics, modifyCompilation: true);
            ParameterHelpers.EnsureNullableAttributeExists(declaringCompilation, this, Parameters, diagnostics, modifyCompilation: true);
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                current.Type.CheckAllConstraints(declaringCompilation, conversions, current.Locations[0], diagnostics);
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

        internal sealed override OneOrMany<SyntaxList<AttributeListSyntax>> GetReturnTypeAttributeDeclarations()
        {
            return OneOrMany.Create(default(SyntaxList<AttributeListSyntax>));
        }

        internal sealed override int CalculateLocalSyntaxOffset(int position, SyntaxTree tree)
        {
            CSharpSyntaxNode cSharpSyntaxNode = (CSharpSyntaxNode)syntaxReferenceOpt.GetSyntax();
            if (tree == cSharpSyntaxNode.SyntaxTree)
            {
                if (IsWithinExpressionOrBlockBody(position, out var offset))
                {
                    return offset;
                }
                if (position == cSharpSyntaxNode.SpanStart)
                {
                    return -1;
                }
            }
            CSharpSyntaxNode initializer = GetInitializer();
            int num;
            if (tree == initializer?.SyntaxTree)
            {
                TextSpan span = initializer.Span;
                num = span.Length;
                if (span.Contains(position))
                {
                    return -num + (position - span.Start);
                }
            }
            else
            {
                num = 0;
            }
            if (((SourceNamedTypeSymbol)ContainingType).TryCalculateSyntaxOffsetOfPositionInInitializer(position, tree, IsStatic, num, out var syntaxOffset))
            {
                return syntaxOffset;
            }
            throw ExceptionUtilities.Unreachable;
        }

        internal abstract override bool IsNullableAnalysisEnabled();

        protected abstract CSharpSyntaxNode GetInitializer();

        protected abstract bool IsWithinExpressionOrBlockBody(int position, out int offset);
    }
}
