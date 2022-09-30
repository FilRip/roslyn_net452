using System.Collections.Generic;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal class GlobalExpressionVariable : SourceMemberFieldSymbol
    {
        private class InferrableGlobalExpressionVariable : GlobalExpressionVariable
        {
            private readonly FieldSymbol _containingFieldOpt;

            private readonly SyntaxReference _nodeToBind;

            internal InferrableGlobalExpressionVariable(SourceMemberContainerTypeSymbol containingType, DeclarationModifiers modifiers, TypeSyntax typeSyntax, string name, SyntaxReference syntax, Location location, FieldSymbol containingFieldOpt, SyntaxNode nodeToBind)
                : base(containingType, modifiers, typeSyntax, name, syntax, location)
            {
                _containingFieldOpt = containingFieldOpt;
                _nodeToBind = nodeToBind.GetReference();
            }

            protected override void InferFieldType(ConsList<FieldSymbol> fieldsBeingBound, Binder binder)
            {
                SyntaxNode syntax = _nodeToBind.GetSyntax();
                if ((object)_containingFieldOpt != null && syntax.Kind() != SyntaxKind.VariableDeclarator)
                {
                    binder = binder.WithContainingMemberOrLambda(_containingFieldOpt).WithAdditionalFlags(BinderFlags.FieldInitializer);
                }
                fieldsBeingBound = new ConsList<FieldSymbol>(this, fieldsBeingBound);
                binder = new ImplicitlyTypedFieldBinder(binder, fieldsBeingBound);
                if (syntax.Kind() == SyntaxKind.VariableDeclarator)
                {
                    binder.BindDeclaratorArguments((VariableDeclaratorSyntax)syntax, BindingDiagnosticBag.Discarded);
                }
                else
                {
                    binder.BindExpression((ExpressionSyntax)syntax, BindingDiagnosticBag.Discarded);
                }
            }
        }

        private TypeWithAnnotations.Boxed _lazyType;

        private readonly SyntaxReference _typeSyntaxOpt;

        protected override SyntaxList<AttributeListSyntax> AttributeDeclarationSyntaxList => default(SyntaxList<AttributeListSyntax>);

        protected override TypeSyntax TypeSyntax => (TypeSyntax)(_typeSyntaxOpt?.GetSyntax());

        protected override SyntaxTokenList ModifiersTokenList => default(SyntaxTokenList);

        public override bool HasInitializer => false;

        internal GlobalExpressionVariable(SourceMemberContainerTypeSymbol containingType, DeclarationModifiers modifiers, TypeSyntax typeSyntax, string name, SyntaxReference syntax, Location location)
            : base(containingType, modifiers, name, syntax, location)
        {
            _typeSyntaxOpt = typeSyntax?.GetReference();
        }

        internal static GlobalExpressionVariable Create(SourceMemberContainerTypeSymbol containingType, DeclarationModifiers modifiers, TypeSyntax typeSyntax, string name, SyntaxNode syntax, Location location, FieldSymbol containingFieldOpt, SyntaxNode nodeToBind)
        {
            SyntaxReference reference = syntax.GetReference();
            if (typeSyntax != null && !typeSyntax.IsVar)
            {
                return new GlobalExpressionVariable(containingType, modifiers, typeSyntax, name, reference, location);
            }
            return new InferrableGlobalExpressionVariable(containingType, modifiers, typeSyntax, name, reference, location, containingFieldOpt, nodeToBind);
        }

        protected override ConstantValue MakeConstantValue(HashSet<SourceFieldSymbolWithSyntaxReference> dependencies, bool earlyDecodingWellKnownAttributes, BindingDiagnosticBag diagnostics)
        {
            return null;
        }

        internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            if (_lazyType != null)
            {
                return _lazyType.Value;
            }
            TypeSyntax typeSyntax = TypeSyntax;
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
            Binder binder = declaringCompilation.GetBinderFactory(base.SyntaxTree).GetBinder(typeSyntax ?? base.SyntaxNode);
            TypeWithAnnotations type;
            bool isVar;
            if (typeSyntax != null)
            {
                type = binder.BindTypeOrVarKeyword(typeSyntax, instance, out isVar);
            }
            else
            {
                isVar = true;
                type = default(TypeWithAnnotations);
            }
            if (isVar && !fieldsBeingBound.ContainsReference(this))
            {
                InferFieldType(fieldsBeingBound, binder);
            }
            else
            {
                if (isVar)
                {
                    instance.Add(ErrorCode.ERR_RecursivelyTypedVariable, ErrorLocation, this);
                    type = TypeWithAnnotations.Create(binder.CreateErrorType("var"));
                }
                SetType(declaringCompilation, instance, type);
            }
            instance.Free();
            return _lazyType.Value;
        }

        private TypeWithAnnotations SetType(CSharpCompilation compilation, BindingDiagnosticBag diagnostics, TypeWithAnnotations type)
        {
            _ = _lazyType?.Value;
            if (Interlocked.CompareExchange(ref _lazyType, new TypeWithAnnotations.Boxed(type), null) == null)
            {
                TypeChecks(type.Type, diagnostics);
                AddDeclarationDiagnostics(diagnostics);
                state.NotePartComplete(CompletionPart.Type);
            }
            return _lazyType.Value;
        }

        internal TypeWithAnnotations SetTypeWithAnnotations(TypeWithAnnotations type, BindingDiagnosticBag diagnostics)
        {
            return SetType(DeclaringCompilation, diagnostics, type);
        }

        protected virtual void InferFieldType(ConsList<FieldSymbol> fieldsBeingBound, Binder binder)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
