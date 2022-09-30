using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ConstructorConstraintSyntax : TypeParameterConstraintSyntax
    {
        internal readonly SyntaxToken newKeyword;

        internal readonly SyntaxToken openParenToken;

        internal readonly SyntaxToken closeParenToken;

        public SyntaxToken NewKeyword => newKeyword;

        public SyntaxToken OpenParenToken => openParenToken;

        public SyntaxToken CloseParenToken => closeParenToken;

        public ConstructorConstraintSyntax(SyntaxKind kind, SyntaxToken newKeyword, SyntaxToken openParenToken, SyntaxToken closeParenToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(newKeyword);
            this.newKeyword = newKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public ConstructorConstraintSyntax(SyntaxKind kind, SyntaxToken newKeyword, SyntaxToken openParenToken, SyntaxToken closeParenToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(newKeyword);
            this.newKeyword = newKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public ConstructorConstraintSyntax(SyntaxKind kind, SyntaxToken newKeyword, SyntaxToken openParenToken, SyntaxToken closeParenToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(newKeyword);
            this.newKeyword = newKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => newKeyword,
                1 => openParenToken,
                2 => closeParenToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorConstraintSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitConstructorConstraint(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitConstructorConstraint(this);
        }

        public ConstructorConstraintSyntax Update(SyntaxToken newKeyword, SyntaxToken openParenToken, SyntaxToken closeParenToken)
        {
            if (newKeyword != NewKeyword || openParenToken != OpenParenToken || closeParenToken != CloseParenToken)
            {
                ConstructorConstraintSyntax constructorConstraintSyntax = SyntaxFactory.ConstructorConstraint(newKeyword, openParenToken, closeParenToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    constructorConstraintSyntax = constructorConstraintSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    constructorConstraintSyntax = constructorConstraintSyntax.WithAnnotationsGreen(annotations);
                }
                return constructorConstraintSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ConstructorConstraintSyntax(base.Kind, newKeyword, openParenToken, closeParenToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ConstructorConstraintSyntax(base.Kind, newKeyword, openParenToken, closeParenToken, GetDiagnostics(), annotations);
        }

        public ConstructorConstraintSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            newKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            openParenToken = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            closeParenToken = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(newKeyword);
            writer.WriteValue(openParenToken);
            writer.WriteValue(closeParenToken);
        }

        static ConstructorConstraintSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ConstructorConstraintSyntax), (ObjectReader r) => new ConstructorConstraintSyntax(r));
        }
    }
}
