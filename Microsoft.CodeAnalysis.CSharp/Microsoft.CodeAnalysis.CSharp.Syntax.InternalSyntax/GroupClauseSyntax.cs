using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class GroupClauseSyntax : SelectOrGroupClauseSyntax
    {
        internal readonly SyntaxToken groupKeyword;

        internal readonly ExpressionSyntax groupExpression;

        internal readonly SyntaxToken byKeyword;

        internal readonly ExpressionSyntax byExpression;

        public SyntaxToken GroupKeyword => groupKeyword;

        public ExpressionSyntax GroupExpression => groupExpression;

        public SyntaxToken ByKeyword => byKeyword;

        public ExpressionSyntax ByExpression => byExpression;

        public GroupClauseSyntax(SyntaxKind kind, SyntaxToken groupKeyword, ExpressionSyntax groupExpression, SyntaxToken byKeyword, ExpressionSyntax byExpression, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(groupKeyword);
            this.groupKeyword = groupKeyword;
            AdjustFlagsAndWidth(groupExpression);
            this.groupExpression = groupExpression;
            AdjustFlagsAndWidth(byKeyword);
            this.byKeyword = byKeyword;
            AdjustFlagsAndWidth(byExpression);
            this.byExpression = byExpression;
        }

        public GroupClauseSyntax(SyntaxKind kind, SyntaxToken groupKeyword, ExpressionSyntax groupExpression, SyntaxToken byKeyword, ExpressionSyntax byExpression, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(groupKeyword);
            this.groupKeyword = groupKeyword;
            AdjustFlagsAndWidth(groupExpression);
            this.groupExpression = groupExpression;
            AdjustFlagsAndWidth(byKeyword);
            this.byKeyword = byKeyword;
            AdjustFlagsAndWidth(byExpression);
            this.byExpression = byExpression;
        }

        public GroupClauseSyntax(SyntaxKind kind, SyntaxToken groupKeyword, ExpressionSyntax groupExpression, SyntaxToken byKeyword, ExpressionSyntax byExpression)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(groupKeyword);
            this.groupKeyword = groupKeyword;
            AdjustFlagsAndWidth(groupExpression);
            this.groupExpression = groupExpression;
            AdjustFlagsAndWidth(byKeyword);
            this.byKeyword = byKeyword;
            AdjustFlagsAndWidth(byExpression);
            this.byExpression = byExpression;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => groupKeyword,
                1 => groupExpression,
                2 => byKeyword,
                3 => byExpression,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.GroupClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitGroupClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitGroupClause(this);
        }

        public GroupClauseSyntax Update(SyntaxToken groupKeyword, ExpressionSyntax groupExpression, SyntaxToken byKeyword, ExpressionSyntax byExpression)
        {
            if (groupKeyword != GroupKeyword || groupExpression != GroupExpression || byKeyword != ByKeyword || byExpression != ByExpression)
            {
                GroupClauseSyntax groupClauseSyntax = SyntaxFactory.GroupClause(groupKeyword, groupExpression, byKeyword, byExpression);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    groupClauseSyntax = groupClauseSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    groupClauseSyntax = groupClauseSyntax.WithAnnotationsGreen(annotations);
                }
                return groupClauseSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new GroupClauseSyntax(base.Kind, groupKeyword, groupExpression, byKeyword, byExpression, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new GroupClauseSyntax(base.Kind, groupKeyword, groupExpression, byKeyword, byExpression, GetDiagnostics(), annotations);
        }

        public GroupClauseSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            groupKeyword = node;
            ExpressionSyntax node2 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            groupExpression = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            byKeyword = node3;
            ExpressionSyntax node4 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            byExpression = node4;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(groupKeyword);
            writer.WriteValue(groupExpression);
            writer.WriteValue(byKeyword);
            writer.WriteValue(byExpression);
        }

        static GroupClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(GroupClauseSyntax), (ObjectReader r) => new GroupClauseSyntax(r));
        }
    }
}
