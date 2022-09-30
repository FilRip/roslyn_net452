using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ThrowStatementSyntax : StatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken throwKeyword;

        internal readonly ExpressionSyntax? expression;

        internal readonly SyntaxToken semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public SyntaxToken ThrowKeyword => throwKeyword;

        public ExpressionSyntax? Expression => expression;

        public SyntaxToken SemicolonToken => semicolonToken;

        public ThrowStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken throwKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(throwKeyword);
            this.throwKeyword = throwKeyword;
            if (expression != null)
            {
                AdjustFlagsAndWidth(expression);
                this.expression = expression;
            }
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public ThrowStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken throwKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(throwKeyword);
            this.throwKeyword = throwKeyword;
            if (expression != null)
            {
                AdjustFlagsAndWidth(expression);
                this.expression = expression;
            }
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public ThrowStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken throwKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
            : base(kind)
        {
            base.SlotCount = 4;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(throwKeyword);
            this.throwKeyword = throwKeyword;
            if (expression != null)
            {
                AdjustFlagsAndWidth(expression);
                this.expression = expression;
            }
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => throwKeyword,
                2 => expression,
                3 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ThrowStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitThrowStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitThrowStatement(this);
        }

        public ThrowStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken throwKeyword, ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || throwKeyword != ThrowKeyword || expression != Expression || semicolonToken != SemicolonToken)
            {
                ThrowStatementSyntax throwStatementSyntax = SyntaxFactory.ThrowStatement(attributeLists, throwKeyword, expression, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    throwStatementSyntax = throwStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    throwStatementSyntax = throwStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return throwStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ThrowStatementSyntax(base.Kind, attributeLists, throwKeyword, expression, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ThrowStatementSyntax(base.Kind, attributeLists, throwKeyword, expression, semicolonToken, GetDiagnostics(), annotations);
        }

        public ThrowStatementSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                attributeLists = greenNode;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            throwKeyword = node;
            ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
            if (expressionSyntax != null)
            {
                AdjustFlagsAndWidth(expressionSyntax);
                expression = expressionSyntax;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            semicolonToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(throwKeyword);
            writer.WriteValue(expression);
            writer.WriteValue(semicolonToken);
        }

        static ThrowStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ThrowStatementSyntax), (ObjectReader r) => new ThrowStatementSyntax(r));
        }
    }
}
