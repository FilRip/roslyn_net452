using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class MemberAccessExpressionSyntax : ExpressionSyntax
    {
        internal readonly ExpressionSyntax expression;

        internal readonly SyntaxToken operatorToken;

        internal readonly SimpleNameSyntax name;

        public ExpressionSyntax Expression => expression;

        public SyntaxToken OperatorToken => operatorToken;

        public SimpleNameSyntax Name => name;

        public MemberAccessExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken operatorToken, SimpleNameSyntax name, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(name);
            this.name = name;
        }

        public MemberAccessExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken operatorToken, SimpleNameSyntax name, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(name);
            this.name = name;
        }

        public MemberAccessExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken operatorToken, SimpleNameSyntax name)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(name);
            this.name = name;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => expression,
                1 => operatorToken,
                2 => name,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitMemberAccessExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitMemberAccessExpression(this);
        }

        public MemberAccessExpressionSyntax Update(ExpressionSyntax expression, SyntaxToken operatorToken, SimpleNameSyntax name)
        {
            if (expression != Expression || operatorToken != OperatorToken || name != Name)
            {
                MemberAccessExpressionSyntax memberAccessExpressionSyntax = SyntaxFactory.MemberAccessExpression(base.Kind, expression, operatorToken, name);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    memberAccessExpressionSyntax = memberAccessExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    memberAccessExpressionSyntax = memberAccessExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return memberAccessExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new MemberAccessExpressionSyntax(base.Kind, expression, operatorToken, name, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new MemberAccessExpressionSyntax(base.Kind, expression, operatorToken, name, GetDiagnostics(), annotations);
        }

        public MemberAccessExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            ExpressionSyntax node = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            expression = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            operatorToken = node2;
            SimpleNameSyntax node3 = (SimpleNameSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            name = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(expression);
            writer.WriteValue(operatorToken);
            writer.WriteValue(name);
        }

        static MemberAccessExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(MemberAccessExpressionSyntax), (ObjectReader r) => new MemberAccessExpressionSyntax(r));
        }
    }
}
