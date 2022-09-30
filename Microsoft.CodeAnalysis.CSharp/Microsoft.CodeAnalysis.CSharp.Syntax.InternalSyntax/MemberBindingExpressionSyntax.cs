using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class MemberBindingExpressionSyntax : ExpressionSyntax
    {
        internal readonly SyntaxToken operatorToken;

        internal readonly SimpleNameSyntax name;

        public SyntaxToken OperatorToken => operatorToken;

        public SimpleNameSyntax Name => name;

        public MemberBindingExpressionSyntax(SyntaxKind kind, SyntaxToken operatorToken, SimpleNameSyntax name, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(name);
            this.name = name;
        }

        public MemberBindingExpressionSyntax(SyntaxKind kind, SyntaxToken operatorToken, SimpleNameSyntax name, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(name);
            this.name = name;
        }

        public MemberBindingExpressionSyntax(SyntaxKind kind, SyntaxToken operatorToken, SimpleNameSyntax name)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(name);
            this.name = name;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => operatorToken,
                1 => name,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.MemberBindingExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitMemberBindingExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitMemberBindingExpression(this);
        }

        public MemberBindingExpressionSyntax Update(SyntaxToken operatorToken, SimpleNameSyntax name)
        {
            if (operatorToken != OperatorToken || name != Name)
            {
                MemberBindingExpressionSyntax memberBindingExpressionSyntax = SyntaxFactory.MemberBindingExpression(operatorToken, name);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    memberBindingExpressionSyntax = memberBindingExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    memberBindingExpressionSyntax = memberBindingExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return memberBindingExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new MemberBindingExpressionSyntax(base.Kind, operatorToken, name, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new MemberBindingExpressionSyntax(base.Kind, operatorToken, name, GetDiagnostics(), annotations);
        }

        public MemberBindingExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            operatorToken = node;
            SimpleNameSyntax node2 = (SimpleNameSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            name = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(operatorToken);
            writer.WriteValue(name);
        }

        static MemberBindingExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(MemberBindingExpressionSyntax), (ObjectReader r) => new MemberBindingExpressionSyntax(r));
        }
    }
}
