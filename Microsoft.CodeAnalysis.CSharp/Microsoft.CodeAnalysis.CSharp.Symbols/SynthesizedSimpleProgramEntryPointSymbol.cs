using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedSimpleProgramEntryPointSymbol : SourceMemberMethodSymbol
    {
        private readonly SingleTypeDeclaration _declaration;

        private readonly TypeSymbol _returnType;

        private readonly ImmutableArray<ParameterSymbol> _parameters;

        private WeakReference<ExecutableCodeBinder>? _weakBodyBinder;

        private WeakReference<ExecutableCodeBinder>? _weakIgnoreAccessibilityBodyBinder;

        public override string Name => "<Main>$";

        internal override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.IL;

        public override bool IsVararg => false;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        internal override int ParameterCount => 1;

        public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

        public override Accessibility DeclaredAccessibility => Accessibility.Private;

        public override RefKind RefKind => RefKind.None;

        public override TypeWithAnnotations ReturnTypeWithAnnotations => TypeWithAnnotations.Create(_returnType);

        public override FlowAnalysisAnnotations ReturnTypeFlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        public override ImmutableHashSet<string> ReturnNotNullIfParameterNotNull => ImmutableHashSet<string>.Empty;

        public override FlowAnalysisAnnotations FlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        public sealed override bool IsImplicitlyDeclared => false;

        internal sealed override bool GenerateDebugInfo => true;

        internal override bool IsExpressionBodied => false;

        protected override object MethodChecksLockObject => _declaration;

        internal CompilationUnitSyntax CompilationUnit => (CompilationUnitSyntax)SyntaxNode;

        public SyntaxNode ReturnTypeSyntax => CompilationUnit.Members.First((MemberDeclarationSyntax m) => m.Kind() == SyntaxKind.GlobalStatement);

        internal SynthesizedSimpleProgramEntryPointSymbol(SimpleProgramNamedTypeSymbol containingType, SingleTypeDeclaration declaration, BindingDiagnosticBag diagnostics)
            : base(containingType, declaration.SyntaxReference, ImmutableArray.Create(declaration.SyntaxReference.GetLocation()), declaration.IsIterator)
        {
            _declaration = declaration;
            bool hasAwaitExpressions = declaration.HasAwaitExpressions;
            bool hasReturnWithExpression = declaration.HasReturnWithExpression;
            CSharpCompilation declaringCompilation = containingType.DeclaringCompilation;
            if (hasAwaitExpressions)
            {
                if (!hasReturnWithExpression)
                {
                    _returnType = Binder.GetWellKnownType(declaringCompilation, WellKnownType.System_Threading_Tasks_Task, diagnostics, NoLocation.Singleton);
                }
                else
                {
                    _returnType = Binder.GetWellKnownType(declaringCompilation, WellKnownType.System_Threading_Tasks_Task_T, diagnostics, NoLocation.Singleton).Construct(Binder.GetSpecialType(declaringCompilation, SpecialType.System_Int32, NoLocation.Singleton, diagnostics));
                }
            }
            else if (!hasReturnWithExpression)
            {
                _returnType = Binder.GetSpecialType(declaringCompilation, SpecialType.System_Void, NoLocation.Singleton, diagnostics);
            }
            else
            {
                _returnType = Binder.GetSpecialType(declaringCompilation, SpecialType.System_Int32, NoLocation.Singleton, diagnostics);
            }
            bool isNullableAnalysisEnabled = IsNullableAnalysisEnabled(declaringCompilation, CompilationUnit);
            MakeFlags(MethodKind.Ordinary, DeclarationModifiers.Static | DeclarationModifiers.Private | (hasAwaitExpressions ? DeclarationModifiers.Async : DeclarationModifiers.None), !hasAwaitExpressions && !hasReturnWithExpression, isExtensionMethod: false, isNullableAnalysisEnabled);
            _parameters = ImmutableArray.Create(SynthesizedParameterSymbol.Create(this, TypeWithAnnotations.Create(ArrayTypeSymbol.CreateCSharpArray(declaringCompilation.Assembly, TypeWithAnnotations.Create(Binder.GetSpecialType(declaringCompilation, SpecialType.System_String, NoLocation.Singleton, diagnostics)))), 0, RefKind.None, "args"));
        }

        internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            return localPosition;
        }

        protected override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
        }

        public override ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
        {
            return ImmutableArray<ImmutableArray<TypeWithAnnotations>>.Empty;
        }

        public override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            return ImmutableArray<TypeParameterConstraintKind>.Empty;
        }

        internal override ExecutableCodeBinder TryGetBodyBinder(BinderFactory? binderFactoryOpt = null, bool ignoreAccessibility = false)
        {
            return GetBodyBinder(ignoreAccessibility);
        }

        private ExecutableCodeBinder CreateBodyBinder(bool ignoreAccessibility)
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            Binder next = new BuckStopsHereBinder(declaringCompilation);
            NamespaceSymbol globalNamespace = declaringCompilation.GlobalNamespace;
            SourceNamespaceSymbol declaringSymbol = (SourceNamespaceSymbol)declaringCompilation.SourceModule.GlobalNamespace;
            CSharpSyntaxNode syntaxNode = SyntaxNode;
            next = WithExternAndUsingAliasesBinder.Create(declaringSymbol, syntaxNode, WithUsingNamespacesAndTypesBinder.Create(declaringSymbol, syntaxNode, next));
            next = new InContainerBinder(globalNamespace, next);
            next = new InContainerBinder(ContainingType, next);
            next = new InMethodBinder(this, next);
            next = next.WithAdditionalFlags(ignoreAccessibility ? BinderFlags.IgnoreAccessibility : BinderFlags.None);
            return new ExecutableCodeBinder(syntaxNode, this, next);
        }

        internal ExecutableCodeBinder GetBodyBinder(bool ignoreAccessibility)
        {
            ref WeakReference<ExecutableCodeBinder> reference = ref ignoreAccessibility ? ref _weakIgnoreAccessibilityBodyBinder : ref _weakBodyBinder;
            WeakReference<ExecutableCodeBinder> weakReference;
            ExecutableCodeBinder executableCodeBinder;
            do
            {
                weakReference = reference;
                if (weakReference != null && weakReference.TryGetTarget(out var target))
                {
                    return target;
                }
                executableCodeBinder = CreateBodyBinder(ignoreAccessibility);
            }
            while (Interlocked.CompareExchange(ref reference, new WeakReference<ExecutableCodeBinder>(executableCodeBinder), weakReference) != weakReference);
            return executableCodeBinder;
        }

        internal override bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken)
        {
            if (_declaration.SyntaxReference.SyntaxTree == tree)
            {
                if (!definedWithinSpan.HasValue)
                {
                    return true;
                }
                TextSpan valueOrDefault = definedWithinSpan.GetValueOrDefault();
                foreach (GlobalStatementSyntax item in ((CompilationUnitSyntax)tree.GetRoot(cancellationToken)).Members.OfType<GlobalStatementSyntax>())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (item.Span.IntersectsWith(valueOrDefault))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsNullableAnalysisEnabled(CSharpCompilation compilation, CompilationUnitSyntax syntax)
        {
            SyntaxList<MemberDeclarationSyntax>.Enumerator enumerator = syntax.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MemberDeclarationSyntax current = enumerator.Current;
                if (current.Kind() == SyntaxKind.GlobalStatement && compilation.IsNullableAnalysisEnabledIn(current))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
