using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class UnaryPatternSyntax : PatternSyntax
    {
        internal readonly SyntaxToken operatorToken;

        internal readonly PatternSyntax pattern;

        public SyntaxToken OperatorToken => operatorToken;

        public PatternSyntax Pattern => pattern;

        public UnaryPatternSyntax(SyntaxKind kind, SyntaxToken operatorToken, PatternSyntax pattern, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
        }

        public UnaryPatternSyntax(SyntaxKind kind, SyntaxToken operatorToken, PatternSyntax pattern, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
        }

        public UnaryPatternSyntax(SyntaxKind kind, SyntaxToken operatorToken, PatternSyntax pattern)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => operatorToken,
                1 => pattern,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.UnaryPatternSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitUnaryPattern(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitUnaryPattern(this);
        }

        public UnaryPatternSyntax Update(SyntaxToken operatorToken, PatternSyntax pattern)
        {
            if (operatorToken != OperatorToken || pattern != Pattern)
            {
                UnaryPatternSyntax unaryPatternSyntax = SyntaxFactory.UnaryPattern(operatorToken, pattern);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    unaryPatternSyntax = unaryPatternSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    unaryPatternSyntax = unaryPatternSyntax.WithAnnotationsGreen(annotations);
                }
                return unaryPatternSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new UnaryPatternSyntax(base.Kind, operatorToken, pattern, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new UnaryPatternSyntax(base.Kind, operatorToken, pattern, GetDiagnostics(), annotations);
        }

        public UnaryPatternSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            operatorToken = node;
            PatternSyntax node2 = (PatternSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            pattern = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(operatorToken);
            writer.WriteValue(pattern);
        }

        static UnaryPatternSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(UnaryPatternSyntax), (ObjectReader r) => new UnaryPatternSyntax(r));
        }
    }
}
