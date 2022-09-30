using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class LineDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {
        internal readonly SyntaxToken hashToken;

        internal readonly SyntaxToken lineKeyword;

        internal readonly SyntaxToken line;

        internal readonly SyntaxToken? file;

        internal readonly SyntaxToken endOfDirectiveToken;

        internal readonly bool isActive;

        public override SyntaxToken HashToken => hashToken;

        public SyntaxToken LineKeyword => lineKeyword;

        public SyntaxToken Line => line;

        public SyntaxToken? File => file;

        public override SyntaxToken EndOfDirectiveToken => endOfDirectiveToken;

        public override bool IsActive => isActive;

        public LineDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken lineKeyword, SyntaxToken line, SyntaxToken? file, SyntaxToken endOfDirectiveToken, bool isActive, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(lineKeyword);
            this.lineKeyword = lineKeyword;
            AdjustFlagsAndWidth(line);
            this.line = line;
            if (file != null)
            {
                AdjustFlagsAndWidth(file);
                this.file = file;
            }
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
        }

        public LineDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken lineKeyword, SyntaxToken line, SyntaxToken? file, SyntaxToken endOfDirectiveToken, bool isActive, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 5;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(lineKeyword);
            this.lineKeyword = lineKeyword;
            AdjustFlagsAndWidth(line);
            this.line = line;
            if (file != null)
            {
                AdjustFlagsAndWidth(file);
                this.file = file;
            }
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
        }

        public LineDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken lineKeyword, SyntaxToken line, SyntaxToken? file, SyntaxToken endOfDirectiveToken, bool isActive)
            : base(kind)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(lineKeyword);
            this.lineKeyword = lineKeyword;
            AdjustFlagsAndWidth(line);
            this.line = line;
            if (file != null)
            {
                AdjustFlagsAndWidth(file);
                this.file = file;
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
                1 => lineKeyword,
                2 => line,
                3 => file,
                4 => endOfDirectiveToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.LineDirectiveTriviaSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitLineDirectiveTrivia(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitLineDirectiveTrivia(this);
        }

        public LineDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken lineKeyword, SyntaxToken line, SyntaxToken file, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != HashToken || lineKeyword != LineKeyword || line != Line || file != File || endOfDirectiveToken != EndOfDirectiveToken)
            {
                LineDirectiveTriviaSyntax lineDirectiveTriviaSyntax = SyntaxFactory.LineDirectiveTrivia(hashToken, lineKeyword, line, file, endOfDirectiveToken, isActive);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    lineDirectiveTriviaSyntax = lineDirectiveTriviaSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    lineDirectiveTriviaSyntax = lineDirectiveTriviaSyntax.WithAnnotationsGreen(annotations);
                }
                return lineDirectiveTriviaSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new LineDirectiveTriviaSyntax(base.Kind, hashToken, lineKeyword, line, file, endOfDirectiveToken, isActive, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new LineDirectiveTriviaSyntax(base.Kind, hashToken, lineKeyword, line, file, endOfDirectiveToken, isActive, GetDiagnostics(), annotations);
        }

        public LineDirectiveTriviaSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 5;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            hashToken = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            lineKeyword = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            line = node3;
            SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
            if (syntaxToken != null)
            {
                AdjustFlagsAndWidth(syntaxToken);
                file = syntaxToken;
            }
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            endOfDirectiveToken = node4;
            isActive = reader.ReadBoolean();
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(hashToken);
            writer.WriteValue(lineKeyword);
            writer.WriteValue(line);
            writer.WriteValue(file);
            writer.WriteValue(endOfDirectiveToken);
            writer.WriteBoolean(isActive);
        }

        static LineDirectiveTriviaSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(LineDirectiveTriviaSyntax), (ObjectReader r) => new LineDirectiveTriviaSyntax(r));
        }
    }
}
