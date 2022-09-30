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
    public sealed class CasePatternSwitchLabelSyntax : SwitchLabelSyntax
    {
        internal readonly SyntaxToken keyword;

        internal readonly PatternSyntax pattern;

        internal readonly WhenClauseSyntax? whenClause;

        internal readonly SyntaxToken colonToken;

        public override SyntaxToken Keyword => keyword;

        public PatternSyntax Pattern => pattern;

        public WhenClauseSyntax? WhenClause => whenClause;

        public override SyntaxToken ColonToken => colonToken;

        public CasePatternSwitchLabelSyntax(SyntaxKind kind, SyntaxToken keyword, PatternSyntax pattern, WhenClauseSyntax? whenClause, SyntaxToken colonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
            if (whenClause != null)
            {
                AdjustFlagsAndWidth(whenClause);
                this.whenClause = whenClause;
            }
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
        }

        public CasePatternSwitchLabelSyntax(SyntaxKind kind, SyntaxToken keyword, PatternSyntax pattern, WhenClauseSyntax? whenClause, SyntaxToken colonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
            if (whenClause != null)
            {
                AdjustFlagsAndWidth(whenClause);
                this.whenClause = whenClause;
            }
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
        }

        public CasePatternSwitchLabelSyntax(SyntaxKind kind, SyntaxToken keyword, PatternSyntax pattern, WhenClauseSyntax? whenClause, SyntaxToken colonToken)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
            if (whenClause != null)
            {
                AdjustFlagsAndWidth(whenClause);
                this.whenClause = whenClause;
            }
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => keyword,
                1 => pattern,
                2 => whenClause,
                3 => colonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCasePatternSwitchLabel(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitCasePatternSwitchLabel(this);
        }

        public CasePatternSwitchLabelSyntax Update(SyntaxToken keyword, PatternSyntax pattern, WhenClauseSyntax whenClause, SyntaxToken colonToken)
        {
            if (keyword != Keyword || pattern != Pattern || whenClause != WhenClause || colonToken != ColonToken)
            {
                CasePatternSwitchLabelSyntax casePatternSwitchLabelSyntax = SyntaxFactory.CasePatternSwitchLabel(keyword, pattern, whenClause, colonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    casePatternSwitchLabelSyntax = casePatternSwitchLabelSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    casePatternSwitchLabelSyntax = casePatternSwitchLabelSyntax.WithAnnotationsGreen(annotations);
                }
                return casePatternSwitchLabelSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new CasePatternSwitchLabelSyntax(base.Kind, keyword, pattern, whenClause, colonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new CasePatternSwitchLabelSyntax(base.Kind, keyword, pattern, whenClause, colonToken, GetDiagnostics(), annotations);
        }

        public CasePatternSwitchLabelSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            keyword = node;
            PatternSyntax node2 = (PatternSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            pattern = node2;
            WhenClauseSyntax whenClauseSyntax = (WhenClauseSyntax)reader.ReadValue();
            if (whenClauseSyntax != null)
            {
                AdjustFlagsAndWidth(whenClauseSyntax);
                whenClause = whenClauseSyntax;
            }
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            colonToken = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(keyword);
            writer.WriteValue(pattern);
            writer.WriteValue(whenClause);
            writer.WriteValue(colonToken);
        }

        static CasePatternSwitchLabelSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(CasePatternSwitchLabelSyntax), (ObjectReader r) => new CasePatternSwitchLabelSyntax(r));
        }
    }
}
