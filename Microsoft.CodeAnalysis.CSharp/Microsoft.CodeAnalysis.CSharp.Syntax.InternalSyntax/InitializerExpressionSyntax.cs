using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class InitializerExpressionSyntax : ExpressionSyntax
    {
        internal readonly SyntaxToken openBraceToken;

        internal readonly GreenNode? expressions;

        internal readonly SyntaxToken closeBraceToken;

        public SyntaxToken OpenBraceToken => openBraceToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> Expressions => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(expressions));

        public SyntaxToken CloseBraceToken => closeBraceToken;

        public InitializerExpressionSyntax(SyntaxKind kind, SyntaxToken openBraceToken, GreenNode? expressions, SyntaxToken closeBraceToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
            if (expressions != null)
            {
                AdjustFlagsAndWidth(expressions);
                this.expressions = expressions;
            }
            AdjustFlagsAndWidth(closeBraceToken);
            this.closeBraceToken = closeBraceToken;
        }

        public InitializerExpressionSyntax(SyntaxKind kind, SyntaxToken openBraceToken, GreenNode? expressions, SyntaxToken closeBraceToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
            if (expressions != null)
            {
                AdjustFlagsAndWidth(expressions);
                this.expressions = expressions;
            }
            AdjustFlagsAndWidth(closeBraceToken);
            this.closeBraceToken = closeBraceToken;
        }

        public InitializerExpressionSyntax(SyntaxKind kind, SyntaxToken openBraceToken, GreenNode? expressions, SyntaxToken closeBraceToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
            if (expressions != null)
            {
                AdjustFlagsAndWidth(expressions);
                this.expressions = expressions;
            }
            AdjustFlagsAndWidth(closeBraceToken);
            this.closeBraceToken = closeBraceToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => openBraceToken,
                1 => expressions,
                2 => closeBraceToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitInitializerExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitInitializerExpression(this);
        }

        public InitializerExpressionSyntax Update(SyntaxToken openBraceToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> expressions, SyntaxToken closeBraceToken)
        {
            if (openBraceToken == OpenBraceToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> right = Expressions;
                if (!(expressions != right) && closeBraceToken == CloseBraceToken)
                {
                    return this;
                }
            }
            InitializerExpressionSyntax initializerExpressionSyntax = SyntaxFactory.InitializerExpression(base.Kind, openBraceToken, expressions, closeBraceToken);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                initializerExpressionSyntax = initializerExpressionSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                initializerExpressionSyntax = initializerExpressionSyntax.WithAnnotationsGreen(annotations);
            }
            return initializerExpressionSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new InitializerExpressionSyntax(base.Kind, openBraceToken, expressions, closeBraceToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new InitializerExpressionSyntax(base.Kind, openBraceToken, expressions, closeBraceToken, GetDiagnostics(), annotations);
        }

        public InitializerExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            openBraceToken = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                expressions = greenNode;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            closeBraceToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(openBraceToken);
            writer.WriteValue(expressions);
            writer.WriteValue(closeBraceToken);
        }

        static InitializerExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(InitializerExpressionSyntax), (ObjectReader r) => new InitializerExpressionSyntax(r));
        }
    }
}
