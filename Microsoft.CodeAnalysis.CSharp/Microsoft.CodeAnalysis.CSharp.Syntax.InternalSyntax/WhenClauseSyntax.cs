using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class WhenClauseSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken whenKeyword;

        internal readonly ExpressionSyntax condition;

        public SyntaxToken WhenKeyword => whenKeyword;

        public ExpressionSyntax Condition => condition;

        public WhenClauseSyntax(SyntaxKind kind, SyntaxToken whenKeyword, ExpressionSyntax condition, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(whenKeyword);
            this.whenKeyword = whenKeyword;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
        }

        public WhenClauseSyntax(SyntaxKind kind, SyntaxToken whenKeyword, ExpressionSyntax condition, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(whenKeyword);
            this.whenKeyword = whenKeyword;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
        }

        public WhenClauseSyntax(SyntaxKind kind, SyntaxToken whenKeyword, ExpressionSyntax condition)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(whenKeyword);
            this.whenKeyword = whenKeyword;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => whenKeyword,
                1 => condition,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitWhenClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitWhenClause(this);
        }

        public WhenClauseSyntax Update(SyntaxToken whenKeyword, ExpressionSyntax condition)
        {
            if (whenKeyword != WhenKeyword || condition != Condition)
            {
                WhenClauseSyntax whenClauseSyntax = SyntaxFactory.WhenClause(whenKeyword, condition);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    whenClauseSyntax = whenClauseSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    whenClauseSyntax = whenClauseSyntax.WithAnnotationsGreen(annotations);
                }
                return whenClauseSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new WhenClauseSyntax(base.Kind, whenKeyword, condition, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new WhenClauseSyntax(base.Kind, whenKeyword, condition, GetDiagnostics(), annotations);
        }

        public WhenClauseSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            whenKeyword = node;
            ExpressionSyntax node2 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            condition = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(whenKeyword);
            writer.WriteValue(condition);
        }

        static WhenClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(WhenClauseSyntax), (ObjectReader r) => new WhenClauseSyntax(r));
        }
    }
}
