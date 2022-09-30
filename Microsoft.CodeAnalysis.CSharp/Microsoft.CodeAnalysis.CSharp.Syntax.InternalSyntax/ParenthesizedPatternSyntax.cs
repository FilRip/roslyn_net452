using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ParenthesizedPatternSyntax : PatternSyntax
    {
        internal readonly SyntaxToken openParenToken;

        internal readonly PatternSyntax pattern;

        internal readonly SyntaxToken closeParenToken;

        public SyntaxToken OpenParenToken => openParenToken;

        public PatternSyntax Pattern => pattern;

        public SyntaxToken CloseParenToken => closeParenToken;

        public ParenthesizedPatternSyntax(SyntaxKind kind, SyntaxToken openParenToken, PatternSyntax pattern, SyntaxToken closeParenToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public ParenthesizedPatternSyntax(SyntaxKind kind, SyntaxToken openParenToken, PatternSyntax pattern, SyntaxToken closeParenToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public ParenthesizedPatternSyntax(SyntaxKind kind, SyntaxToken openParenToken, PatternSyntax pattern, SyntaxToken closeParenToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => openParenToken,
                1 => pattern,
                2 => closeParenToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedPatternSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitParenthesizedPattern(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitParenthesizedPattern(this);
        }

        public ParenthesizedPatternSyntax Update(SyntaxToken openParenToken, PatternSyntax pattern, SyntaxToken closeParenToken)
        {
            if (openParenToken != OpenParenToken || pattern != Pattern || closeParenToken != CloseParenToken)
            {
                ParenthesizedPatternSyntax parenthesizedPatternSyntax = SyntaxFactory.ParenthesizedPattern(openParenToken, pattern, closeParenToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    parenthesizedPatternSyntax = parenthesizedPatternSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    parenthesizedPatternSyntax = parenthesizedPatternSyntax.WithAnnotationsGreen(annotations);
                }
                return parenthesizedPatternSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ParenthesizedPatternSyntax(base.Kind, openParenToken, pattern, closeParenToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ParenthesizedPatternSyntax(base.Kind, openParenToken, pattern, closeParenToken, GetDiagnostics(), annotations);
        }

        public ParenthesizedPatternSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            openParenToken = node;
            PatternSyntax node2 = (PatternSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            pattern = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            closeParenToken = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(openParenToken);
            writer.WriteValue(pattern);
            writer.WriteValue(closeParenToken);
        }

        static ParenthesizedPatternSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ParenthesizedPatternSyntax), (ObjectReader r) => new ParenthesizedPatternSyntax(r));
        }
    }
}
