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

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class SwitchExpressionArmSyntax : CSharpSyntaxNode
    {
        internal readonly PatternSyntax pattern;

        internal readonly WhenClauseSyntax? whenClause;

        internal readonly SyntaxToken equalsGreaterThanToken;

        internal readonly ExpressionSyntax expression;

        public PatternSyntax Pattern => pattern;

        public WhenClauseSyntax? WhenClause => whenClause;

        public SyntaxToken EqualsGreaterThanToken => equalsGreaterThanToken;

        public ExpressionSyntax Expression => expression;

        public SwitchExpressionArmSyntax(SyntaxKind kind, PatternSyntax pattern, WhenClauseSyntax? whenClause, SyntaxToken equalsGreaterThanToken, ExpressionSyntax expression, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
            if (whenClause != null)
            {
                AdjustFlagsAndWidth(whenClause);
                this.whenClause = whenClause;
            }
            AdjustFlagsAndWidth(equalsGreaterThanToken);
            this.equalsGreaterThanToken = equalsGreaterThanToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public SwitchExpressionArmSyntax(SyntaxKind kind, PatternSyntax pattern, WhenClauseSyntax? whenClause, SyntaxToken equalsGreaterThanToken, ExpressionSyntax expression, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
            if (whenClause != null)
            {
                AdjustFlagsAndWidth(whenClause);
                this.whenClause = whenClause;
            }
            AdjustFlagsAndWidth(equalsGreaterThanToken);
            this.equalsGreaterThanToken = equalsGreaterThanToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public SwitchExpressionArmSyntax(SyntaxKind kind, PatternSyntax pattern, WhenClauseSyntax? whenClause, SyntaxToken equalsGreaterThanToken, ExpressionSyntax expression)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
            if (whenClause != null)
            {
                AdjustFlagsAndWidth(whenClause);
                this.whenClause = whenClause;
            }
            AdjustFlagsAndWidth(equalsGreaterThanToken);
            this.equalsGreaterThanToken = equalsGreaterThanToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => pattern,
                1 => whenClause,
                2 => equalsGreaterThanToken,
                3 => expression,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSwitchExpressionArm(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitSwitchExpressionArm(this);
        }

        public SwitchExpressionArmSyntax Update(PatternSyntax pattern, WhenClauseSyntax whenClause, SyntaxToken equalsGreaterThanToken, ExpressionSyntax expression)
        {
            if (pattern != Pattern || whenClause != WhenClause || equalsGreaterThanToken != EqualsGreaterThanToken || expression != Expression)
            {
                SwitchExpressionArmSyntax switchExpressionArmSyntax = SyntaxFactory.SwitchExpressionArm(pattern, whenClause, equalsGreaterThanToken, expression);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    switchExpressionArmSyntax = switchExpressionArmSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    switchExpressionArmSyntax = switchExpressionArmSyntax.WithAnnotationsGreen(annotations);
                }
                return switchExpressionArmSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new SwitchExpressionArmSyntax(base.Kind, pattern, whenClause, equalsGreaterThanToken, expression, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new SwitchExpressionArmSyntax(base.Kind, pattern, whenClause, equalsGreaterThanToken, expression, GetDiagnostics(), annotations);
        }

        public SwitchExpressionArmSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            PatternSyntax node = (PatternSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            pattern = node;
            WhenClauseSyntax whenClauseSyntax = (WhenClauseSyntax)reader.ReadValue();
            if (whenClauseSyntax != null)
            {
                AdjustFlagsAndWidth(whenClauseSyntax);
                whenClause = whenClauseSyntax;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            equalsGreaterThanToken = node2;
            ExpressionSyntax node3 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            expression = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(pattern);
            writer.WriteValue(whenClause);
            writer.WriteValue(equalsGreaterThanToken);
            writer.WriteValue(expression);
        }

        static SwitchExpressionArmSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(SwitchExpressionArmSyntax), (ObjectReader r) => new SwitchExpressionArmSyntax(r));
        }
    }
}
