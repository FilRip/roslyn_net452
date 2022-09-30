using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class PragmaChecksumDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {
        internal readonly SyntaxToken hashToken;

        internal readonly SyntaxToken pragmaKeyword;

        internal readonly SyntaxToken checksumKeyword;

        internal readonly SyntaxToken file;

        internal readonly SyntaxToken guid;

        internal readonly SyntaxToken bytes;

        internal readonly SyntaxToken endOfDirectiveToken;

        internal readonly bool isActive;

        public override SyntaxToken HashToken => hashToken;

        public SyntaxToken PragmaKeyword => pragmaKeyword;

        public SyntaxToken ChecksumKeyword => checksumKeyword;

        public SyntaxToken File => file;

        public SyntaxToken Guid => guid;

        public SyntaxToken Bytes => bytes;

        public override SyntaxToken EndOfDirectiveToken => endOfDirectiveToken;

        public override bool IsActive => isActive;

        public PragmaChecksumDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken pragmaKeyword, SyntaxToken checksumKeyword, SyntaxToken file, SyntaxToken guid, SyntaxToken bytes, SyntaxToken endOfDirectiveToken, bool isActive, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 7;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(pragmaKeyword);
            this.pragmaKeyword = pragmaKeyword;
            AdjustFlagsAndWidth(checksumKeyword);
            this.checksumKeyword = checksumKeyword;
            AdjustFlagsAndWidth(file);
            this.file = file;
            AdjustFlagsAndWidth(guid);
            this.guid = guid;
            AdjustFlagsAndWidth(bytes);
            this.bytes = bytes;
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
        }

        public PragmaChecksumDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken pragmaKeyword, SyntaxToken checksumKeyword, SyntaxToken file, SyntaxToken guid, SyntaxToken bytes, SyntaxToken endOfDirectiveToken, bool isActive, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 7;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(pragmaKeyword);
            this.pragmaKeyword = pragmaKeyword;
            AdjustFlagsAndWidth(checksumKeyword);
            this.checksumKeyword = checksumKeyword;
            AdjustFlagsAndWidth(file);
            this.file = file;
            AdjustFlagsAndWidth(guid);
            this.guid = guid;
            AdjustFlagsAndWidth(bytes);
            this.bytes = bytes;
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
        }

        public PragmaChecksumDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken pragmaKeyword, SyntaxToken checksumKeyword, SyntaxToken file, SyntaxToken guid, SyntaxToken bytes, SyntaxToken endOfDirectiveToken, bool isActive)
            : base(kind)
        {
            base.SlotCount = 7;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(pragmaKeyword);
            this.pragmaKeyword = pragmaKeyword;
            AdjustFlagsAndWidth(checksumKeyword);
            this.checksumKeyword = checksumKeyword;
            AdjustFlagsAndWidth(file);
            this.file = file;
            AdjustFlagsAndWidth(guid);
            this.guid = guid;
            AdjustFlagsAndWidth(bytes);
            this.bytes = bytes;
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
                2 => checksumKeyword,
                3 => file,
                4 => guid,
                5 => bytes,
                6 => endOfDirectiveToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.PragmaChecksumDirectiveTriviaSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitPragmaChecksumDirectiveTrivia(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitPragmaChecksumDirectiveTrivia(this);
        }

        public PragmaChecksumDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken pragmaKeyword, SyntaxToken checksumKeyword, SyntaxToken file, SyntaxToken guid, SyntaxToken bytes, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != HashToken || pragmaKeyword != PragmaKeyword || checksumKeyword != ChecksumKeyword || file != File || guid != Guid || bytes != Bytes || endOfDirectiveToken != EndOfDirectiveToken)
            {
                PragmaChecksumDirectiveTriviaSyntax pragmaChecksumDirectiveTriviaSyntax = SyntaxFactory.PragmaChecksumDirectiveTrivia(hashToken, pragmaKeyword, checksumKeyword, file, guid, bytes, endOfDirectiveToken, isActive);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    pragmaChecksumDirectiveTriviaSyntax = pragmaChecksumDirectiveTriviaSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    pragmaChecksumDirectiveTriviaSyntax = pragmaChecksumDirectiveTriviaSyntax.WithAnnotationsGreen(annotations);
                }
                return pragmaChecksumDirectiveTriviaSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new PragmaChecksumDirectiveTriviaSyntax(base.Kind, hashToken, pragmaKeyword, checksumKeyword, file, guid, bytes, endOfDirectiveToken, isActive, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new PragmaChecksumDirectiveTriviaSyntax(base.Kind, hashToken, pragmaKeyword, checksumKeyword, file, guid, bytes, endOfDirectiveToken, isActive, GetDiagnostics(), annotations);
        }

        public PragmaChecksumDirectiveTriviaSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 7;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            hashToken = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            pragmaKeyword = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            checksumKeyword = node3;
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            file = node4;
            SyntaxToken node5 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node5);
            guid = node5;
            SyntaxToken node6 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node6);
            bytes = node6;
            SyntaxToken node7 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node7);
            endOfDirectiveToken = node7;
            isActive = reader.ReadBoolean();
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(hashToken);
            writer.WriteValue(pragmaKeyword);
            writer.WriteValue(checksumKeyword);
            writer.WriteValue(file);
            writer.WriteValue(guid);
            writer.WriteValue(bytes);
            writer.WriteValue(endOfDirectiveToken);
            writer.WriteBoolean(isActive);
        }

        static PragmaChecksumDirectiveTriviaSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(PragmaChecksumDirectiveTriviaSyntax), (ObjectReader r) => new PragmaChecksumDirectiveTriviaSyntax(r));
        }
    }
}
