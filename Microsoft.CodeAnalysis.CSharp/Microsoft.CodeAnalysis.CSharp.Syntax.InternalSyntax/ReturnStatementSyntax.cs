using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ReturnStatementSyntax : StatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken returnKeyword;

        internal readonly ExpressionSyntax? expression;

        internal readonly SyntaxToken semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public SyntaxToken ReturnKeyword => returnKeyword;

        public ExpressionSyntax? Expression => expression;

        public SyntaxToken SemicolonToken => semicolonToken;

        public ReturnStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken returnKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(returnKeyword);
            this.returnKeyword = returnKeyword;
            if (expression != null)
            {
                AdjustFlagsAndWidth(expression);
                this.expression = expression;
            }
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public ReturnStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken returnKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(returnKeyword);
            this.returnKeyword = returnKeyword;
            if (expression != null)
            {
                AdjustFlagsAndWidth(expression);
                this.expression = expression;
            }
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public ReturnStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken returnKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
            : base(kind)
        {
            base.SlotCount = 4;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(returnKeyword);
            this.returnKeyword = returnKeyword;
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
                1 => returnKeyword,
                2 => expression,
                3 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ReturnStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitReturnStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitReturnStatement(this);
        }

        public ReturnStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken returnKeyword, ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || returnKeyword != ReturnKeyword || expression != Expression || semicolonToken != SemicolonToken)
            {
                ReturnStatementSyntax returnStatementSyntax = SyntaxFactory.ReturnStatement(attributeLists, returnKeyword, expression, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    returnStatementSyntax = returnStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    returnStatementSyntax = returnStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return returnStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ReturnStatementSyntax(base.Kind, attributeLists, returnKeyword, expression, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ReturnStatementSyntax(base.Kind, attributeLists, returnKeyword, expression, semicolonToken, GetDiagnostics(), annotations);
        }

        public ReturnStatementSyntax(ObjectReader reader)
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
            returnKeyword = node;
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
            writer.WriteValue(returnKeyword);
            writer.WriteValue(expression);
            writer.WriteValue(semicolonToken);
        }

        static ReturnStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ReturnStatementSyntax), (ObjectReader r) => new ReturnStatementSyntax(r));
        }
    }
}
