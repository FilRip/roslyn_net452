using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class SwitchExpressionSyntax : ExpressionSyntax
    {
        internal readonly ExpressionSyntax governingExpression;

        internal readonly SyntaxToken switchKeyword;

        internal readonly SyntaxToken openBraceToken;

        internal readonly GreenNode? arms;

        internal readonly SyntaxToken closeBraceToken;

        public ExpressionSyntax GoverningExpression => governingExpression;

        public SyntaxToken SwitchKeyword => switchKeyword;

        public SyntaxToken OpenBraceToken => openBraceToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SwitchExpressionArmSyntax> Arms => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SwitchExpressionArmSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(arms));

        public SyntaxToken CloseBraceToken => closeBraceToken;

        public SwitchExpressionSyntax(SyntaxKind kind, ExpressionSyntax governingExpression, SyntaxToken switchKeyword, SyntaxToken openBraceToken, GreenNode? arms, SyntaxToken closeBraceToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(governingExpression);
            this.governingExpression = governingExpression;
            AdjustFlagsAndWidth(switchKeyword);
            this.switchKeyword = switchKeyword;
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
            if (arms != null)
            {
                AdjustFlagsAndWidth(arms);
                this.arms = arms;
            }
            AdjustFlagsAndWidth(closeBraceToken);
            this.closeBraceToken = closeBraceToken;
        }

        public SwitchExpressionSyntax(SyntaxKind kind, ExpressionSyntax governingExpression, SyntaxToken switchKeyword, SyntaxToken openBraceToken, GreenNode? arms, SyntaxToken closeBraceToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 5;
            AdjustFlagsAndWidth(governingExpression);
            this.governingExpression = governingExpression;
            AdjustFlagsAndWidth(switchKeyword);
            this.switchKeyword = switchKeyword;
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
            if (arms != null)
            {
                AdjustFlagsAndWidth(arms);
                this.arms = arms;
            }
            AdjustFlagsAndWidth(closeBraceToken);
            this.closeBraceToken = closeBraceToken;
        }

        public SwitchExpressionSyntax(SyntaxKind kind, ExpressionSyntax governingExpression, SyntaxToken switchKeyword, SyntaxToken openBraceToken, GreenNode? arms, SyntaxToken closeBraceToken)
            : base(kind)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(governingExpression);
            this.governingExpression = governingExpression;
            AdjustFlagsAndWidth(switchKeyword);
            this.switchKeyword = switchKeyword;
            AdjustFlagsAndWidth(openBraceToken);
            this.openBraceToken = openBraceToken;
            if (arms != null)
            {
                AdjustFlagsAndWidth(arms);
                this.arms = arms;
            }
            AdjustFlagsAndWidth(closeBraceToken);
            this.closeBraceToken = closeBraceToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => governingExpression,
                1 => switchKeyword,
                2 => openBraceToken,
                3 => arms,
                4 => closeBraceToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSwitchExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitSwitchExpression(this);
        }

        public SwitchExpressionSyntax Update(ExpressionSyntax governingExpression, SyntaxToken switchKeyword, SyntaxToken openBraceToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SwitchExpressionArmSyntax> arms, SyntaxToken closeBraceToken)
        {
            if (governingExpression == GoverningExpression && switchKeyword == SwitchKeyword && openBraceToken == OpenBraceToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SwitchExpressionArmSyntax> right = Arms;
                if (!(arms != right) && closeBraceToken == CloseBraceToken)
                {
                    return this;
                }
            }
            SwitchExpressionSyntax switchExpressionSyntax = SyntaxFactory.SwitchExpression(governingExpression, switchKeyword, openBraceToken, arms, closeBraceToken);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                switchExpressionSyntax = switchExpressionSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                switchExpressionSyntax = switchExpressionSyntax.WithAnnotationsGreen(annotations);
            }
            return switchExpressionSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new SwitchExpressionSyntax(base.Kind, governingExpression, switchKeyword, openBraceToken, arms, closeBraceToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new SwitchExpressionSyntax(base.Kind, governingExpression, switchKeyword, openBraceToken, arms, closeBraceToken, GetDiagnostics(), annotations);
        }

        public SwitchExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 5;
            ExpressionSyntax node = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            governingExpression = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            switchKeyword = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            openBraceToken = node3;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                arms = greenNode;
            }
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            closeBraceToken = node4;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(governingExpression);
            writer.WriteValue(switchKeyword);
            writer.WriteValue(openBraceToken);
            writer.WriteValue(arms);
            writer.WriteValue(closeBraceToken);
        }

        static SwitchExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(SwitchExpressionSyntax), (ObjectReader r) => new SwitchExpressionSyntax(r));
        }
    }
}
