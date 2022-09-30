using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class SizeOfExpressionSyntax : ExpressionSyntax
    {
        internal readonly SyntaxToken keyword;

        internal readonly SyntaxToken openParenToken;

        internal readonly TypeSyntax type;

        internal readonly SyntaxToken closeParenToken;

        public SyntaxToken Keyword => keyword;

        public SyntaxToken OpenParenToken => openParenToken;

        public TypeSyntax Type => type;

        public SyntaxToken CloseParenToken => closeParenToken;

        public SizeOfExpressionSyntax(SyntaxKind kind, SyntaxToken keyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(type);
            this.type = type;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public SizeOfExpressionSyntax(SyntaxKind kind, SyntaxToken keyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(type);
            this.type = type;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public SizeOfExpressionSyntax(SyntaxKind kind, SyntaxToken keyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(type);
            this.type = type;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => keyword,
                1 => openParenToken,
                2 => type,
                3 => closeParenToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.SizeOfExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSizeOfExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitSizeOfExpression(this);
        }

        public SizeOfExpressionSyntax Update(SyntaxToken keyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken)
        {
            if (keyword != Keyword || openParenToken != OpenParenToken || type != Type || closeParenToken != CloseParenToken)
            {
                SizeOfExpressionSyntax sizeOfExpressionSyntax = SyntaxFactory.SizeOfExpression(keyword, openParenToken, type, closeParenToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    sizeOfExpressionSyntax = sizeOfExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    sizeOfExpressionSyntax = sizeOfExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return sizeOfExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new SizeOfExpressionSyntax(base.Kind, keyword, openParenToken, type, closeParenToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new SizeOfExpressionSyntax(base.Kind, keyword, openParenToken, type, closeParenToken, GetDiagnostics(), annotations);
        }

        public SizeOfExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            keyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            openParenToken = node2;
            TypeSyntax node3 = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            type = node3;
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            closeParenToken = node4;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(keyword);
            writer.WriteValue(openParenToken);
            writer.WriteValue(type);
            writer.WriteValue(closeParenToken);
        }

        static SizeOfExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(SizeOfExpressionSyntax), (ObjectReader r) => new SizeOfExpressionSyntax(r));
        }
    }
}
