using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class DocumentationCommentTriviaSyntax : StructuredTriviaSyntax
    {
        internal readonly GreenNode? content;

        internal readonly SyntaxToken endOfComment;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> Content => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax>(content);

        public SyntaxToken EndOfComment => endOfComment;

        public DocumentationCommentTriviaSyntax(SyntaxKind kind, GreenNode? content, SyntaxToken endOfComment, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            if (content != null)
            {
                AdjustFlagsAndWidth(content);
                this.content = content;
            }
            AdjustFlagsAndWidth(endOfComment);
            this.endOfComment = endOfComment;
        }

        public DocumentationCommentTriviaSyntax(SyntaxKind kind, GreenNode? content, SyntaxToken endOfComment, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            if (content != null)
            {
                AdjustFlagsAndWidth(content);
                this.content = content;
            }
            AdjustFlagsAndWidth(endOfComment);
            this.endOfComment = endOfComment;
        }

        public DocumentationCommentTriviaSyntax(SyntaxKind kind, GreenNode? content, SyntaxToken endOfComment)
            : base(kind)
        {
            base.SlotCount = 2;
            if (content != null)
            {
                AdjustFlagsAndWidth(content);
                this.content = content;
            }
            AdjustFlagsAndWidth(endOfComment);
            this.endOfComment = endOfComment;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => content,
                1 => endOfComment,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.DocumentationCommentTriviaSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitDocumentationCommentTrivia(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitDocumentationCommentTrivia(this);
        }

        public DocumentationCommentTriviaSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> content, SyntaxToken endOfComment)
        {
            if (content != Content || endOfComment != EndOfComment)
            {
                DocumentationCommentTriviaSyntax documentationCommentTriviaSyntax = SyntaxFactory.DocumentationCommentTrivia(base.Kind, content, endOfComment);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    documentationCommentTriviaSyntax = documentationCommentTriviaSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    documentationCommentTriviaSyntax = documentationCommentTriviaSyntax.WithAnnotationsGreen(annotations);
                }
                return documentationCommentTriviaSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new DocumentationCommentTriviaSyntax(base.Kind, content, endOfComment, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new DocumentationCommentTriviaSyntax(base.Kind, content, endOfComment, GetDiagnostics(), annotations);
        }

        public DocumentationCommentTriviaSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                content = greenNode;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            endOfComment = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(content);
            writer.WriteValue(endOfComment);
        }

        static DocumentationCommentTriviaSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(DocumentationCommentTriviaSyntax), (ObjectReader r) => new DocumentationCommentTriviaSyntax(r));
        }
    }
}
