using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class IfDirectiveTriviaSyntax : ConditionalDirectiveTriviaSyntax
    {
        internal readonly SyntaxToken hashToken;

        internal readonly SyntaxToken ifKeyword;

        internal readonly ExpressionSyntax condition;

        internal readonly SyntaxToken endOfDirectiveToken;

        internal readonly bool isActive;

        internal readonly bool branchTaken;

        internal readonly bool conditionValue;

        public override SyntaxToken HashToken => hashToken;

        public SyntaxToken IfKeyword => ifKeyword;

        public override ExpressionSyntax Condition => condition;

        public override SyntaxToken EndOfDirectiveToken => endOfDirectiveToken;

        public override bool IsActive => isActive;

        public override bool BranchTaken => branchTaken;

        public override bool ConditionValue => conditionValue;

        public IfDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken ifKeyword, ExpressionSyntax condition, SyntaxToken endOfDirectiveToken, bool isActive, bool branchTaken, bool conditionValue, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(ifKeyword);
            this.ifKeyword = ifKeyword;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
            this.branchTaken = branchTaken;
            this.conditionValue = conditionValue;
        }

        public IfDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken ifKeyword, ExpressionSyntax condition, SyntaxToken endOfDirectiveToken, bool isActive, bool branchTaken, bool conditionValue, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(ifKeyword);
            this.ifKeyword = ifKeyword;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
            this.branchTaken = branchTaken;
            this.conditionValue = conditionValue;
        }

        public IfDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken ifKeyword, ExpressionSyntax condition, SyntaxToken endOfDirectiveToken, bool isActive, bool branchTaken, bool conditionValue)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(ifKeyword);
            this.ifKeyword = ifKeyword;
            AdjustFlagsAndWidth(condition);
            this.condition = condition;
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
            this.branchTaken = branchTaken;
            this.conditionValue = conditionValue;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => hashToken,
                1 => ifKeyword,
                2 => condition,
                3 => endOfDirectiveToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.IfDirectiveTriviaSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitIfDirectiveTrivia(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitIfDirectiveTrivia(this);
        }

        public IfDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken ifKeyword, ExpressionSyntax condition, SyntaxToken endOfDirectiveToken, bool isActive, bool branchTaken, bool conditionValue)
        {
            if (hashToken != HashToken || ifKeyword != IfKeyword || condition != Condition || endOfDirectiveToken != EndOfDirectiveToken)
            {
                IfDirectiveTriviaSyntax ifDirectiveTriviaSyntax = SyntaxFactory.IfDirectiveTrivia(hashToken, ifKeyword, condition, endOfDirectiveToken, isActive, branchTaken, conditionValue);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    ifDirectiveTriviaSyntax = ifDirectiveTriviaSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    ifDirectiveTriviaSyntax = ifDirectiveTriviaSyntax.WithAnnotationsGreen(annotations);
                }
                return ifDirectiveTriviaSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new IfDirectiveTriviaSyntax(base.Kind, hashToken, ifKeyword, condition, endOfDirectiveToken, isActive, branchTaken, conditionValue, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new IfDirectiveTriviaSyntax(base.Kind, hashToken, ifKeyword, condition, endOfDirectiveToken, isActive, branchTaken, conditionValue, GetDiagnostics(), annotations);
        }

        public IfDirectiveTriviaSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            hashToken = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            ifKeyword = node2;
            ExpressionSyntax node3 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            condition = node3;
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            endOfDirectiveToken = node4;
            isActive = reader.ReadBoolean();
            branchTaken = reader.ReadBoolean();
            conditionValue = reader.ReadBoolean();
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(hashToken);
            writer.WriteValue(ifKeyword);
            writer.WriteValue(condition);
            writer.WriteValue(endOfDirectiveToken);
            writer.WriteBoolean(isActive);
            writer.WriteBoolean(branchTaken);
            writer.WriteBoolean(conditionValue);
        }

        static IfDirectiveTriviaSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(IfDirectiveTriviaSyntax), (ObjectReader r) => new IfDirectiveTriviaSyntax(r));
        }
    }
}
