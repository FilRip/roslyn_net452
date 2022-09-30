using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class PragmaWarningDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {
        internal readonly SyntaxToken hashToken;

        internal readonly SyntaxToken pragmaKeyword;

        internal readonly SyntaxToken warningKeyword;

        internal readonly SyntaxToken disableOrRestoreKeyword;

        internal readonly GreenNode? errorCodes;

        internal readonly SyntaxToken endOfDirectiveToken;

        internal readonly bool isActive;

        public override SyntaxToken HashToken => hashToken;

        public SyntaxToken PragmaKeyword => pragmaKeyword;

        public SyntaxToken WarningKeyword => warningKeyword;

        public SyntaxToken DisableOrRestoreKeyword => disableOrRestoreKeyword;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> ErrorCodes => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(errorCodes));

        public override SyntaxToken EndOfDirectiveToken => endOfDirectiveToken;

        public override bool IsActive => isActive;

        public PragmaWarningDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken pragmaKeyword, SyntaxToken warningKeyword, SyntaxToken disableOrRestoreKeyword, GreenNode? errorCodes, SyntaxToken endOfDirectiveToken, bool isActive, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 6;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(pragmaKeyword);
            this.pragmaKeyword = pragmaKeyword;
            AdjustFlagsAndWidth(warningKeyword);
            this.warningKeyword = warningKeyword;
            AdjustFlagsAndWidth(disableOrRestoreKeyword);
            this.disableOrRestoreKeyword = disableOrRestoreKeyword;
            if (errorCodes != null)
            {
                AdjustFlagsAndWidth(errorCodes);
                this.errorCodes = errorCodes;
            }
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
        }

        public PragmaWarningDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken pragmaKeyword, SyntaxToken warningKeyword, SyntaxToken disableOrRestoreKeyword, GreenNode? errorCodes, SyntaxToken endOfDirectiveToken, bool isActive, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 6;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(pragmaKeyword);
            this.pragmaKeyword = pragmaKeyword;
            AdjustFlagsAndWidth(warningKeyword);
            this.warningKeyword = warningKeyword;
            AdjustFlagsAndWidth(disableOrRestoreKeyword);
            this.disableOrRestoreKeyword = disableOrRestoreKeyword;
            if (errorCodes != null)
            {
                AdjustFlagsAndWidth(errorCodes);
                this.errorCodes = errorCodes;
            }
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
        }

        public PragmaWarningDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken pragmaKeyword, SyntaxToken warningKeyword, SyntaxToken disableOrRestoreKeyword, GreenNode? errorCodes, SyntaxToken endOfDirectiveToken, bool isActive)
            : base(kind)
        {
            base.SlotCount = 6;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(pragmaKeyword);
            this.pragmaKeyword = pragmaKeyword;
            AdjustFlagsAndWidth(warningKeyword);
            this.warningKeyword = warningKeyword;
            AdjustFlagsAndWidth(disableOrRestoreKeyword);
            this.disableOrRestoreKeyword = disableOrRestoreKeyword;
            if (errorCodes != null)
            {
                AdjustFlagsAndWidth(errorCodes);
                this.errorCodes = errorCodes;
            }
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => hashToken,
                1 => pragmaKeyword,
                2 => warningKeyword,
                3 => disableOrRestoreKeyword,
                4 => errorCodes,
                5 => endOfDirectiveToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.PragmaWarningDirectiveTriviaSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitPragmaWarningDirectiveTrivia(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitPragmaWarningDirectiveTrivia(this);
        }

        public PragmaWarningDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken pragmaKeyword, SyntaxToken warningKeyword, SyntaxToken disableOrRestoreKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> errorCodes, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken == HashToken && pragmaKeyword == PragmaKeyword && warningKeyword == WarningKeyword && disableOrRestoreKeyword == DisableOrRestoreKeyword)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> right = ErrorCodes;
                if (!(errorCodes != right) && endOfDirectiveToken == EndOfDirectiveToken)
                {
                    return this;
                }
            }
            PragmaWarningDirectiveTriviaSyntax pragmaWarningDirectiveTriviaSyntax = SyntaxFactory.PragmaWarningDirectiveTrivia(hashToken, pragmaKeyword, warningKeyword, disableOrRestoreKeyword, errorCodes, endOfDirectiveToken, isActive);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                pragmaWarningDirectiveTriviaSyntax = pragmaWarningDirectiveTriviaSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                pragmaWarningDirectiveTriviaSyntax = pragmaWarningDirectiveTriviaSyntax.WithAnnotationsGreen(annotations);
            }
            return pragmaWarningDirectiveTriviaSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new PragmaWarningDirectiveTriviaSyntax(base.Kind, hashToken, pragmaKeyword, warningKeyword, disableOrRestoreKeyword, errorCodes, endOfDirectiveToken, isActive, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new PragmaWarningDirectiveTriviaSyntax(base.Kind, hashToken, pragmaKeyword, warningKeyword, disableOrRestoreKeyword, errorCodes, endOfDirectiveToken, isActive, GetDiagnostics(), annotations);
        }

        public PragmaWarningDirectiveTriviaSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 6;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            hashToken = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            pragmaKeyword = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            warningKeyword = node3;
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            disableOrRestoreKeyword = node4;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                errorCodes = greenNode;
            }
            SyntaxToken node5 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node5);
            endOfDirectiveToken = node5;
            isActive = reader.ReadBoolean();
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(hashToken);
            writer.WriteValue(pragmaKeyword);
            writer.WriteValue(warningKeyword);
            writer.WriteValue(disableOrRestoreKeyword);
            writer.WriteValue(errorCodes);
            writer.WriteValue(endOfDirectiveToken);
            writer.WriteBoolean(isActive);
        }

        static PragmaWarningDirectiveTriviaSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(PragmaWarningDirectiveTriviaSyntax), (ObjectReader r) => new PragmaWarningDirectiveTriviaSyntax(r));
        }
    }
}
