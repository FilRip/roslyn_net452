using System.IO;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public class SyntaxTrivia : CSharpSyntaxNode
    {
        public readonly string Text;

        public override bool IsTrivia => true;

        public override bool ShouldReuseInSerialization
        {
            get
            {
                if (base.Kind == SyntaxKind.WhitespaceTrivia)
                {
                    return base.FullWidth < 42;
                }
                return false;
            }
        }

        public override int Width => base.FullWidth;

        public SyntaxTrivia(SyntaxKind kind, string text, DiagnosticInfo[]? diagnostics = null, SyntaxAnnotation[]? annotations = null)
            : base(kind, diagnostics, annotations, text.Length)
        {
            Text = text;
            if (kind == SyntaxKind.PreprocessingMessageTrivia)
            {
                flags |= NodeFlags.ContainsSkippedText;
            }
        }

        public SyntaxTrivia(ObjectReader reader)
            : base(reader)
        {
            Text = reader.ReadString();
            base.FullWidth = Text.Length;
        }

        static SyntaxTrivia()
        {
            ObjectBinder.RegisterTypeReader(typeof(SyntaxTrivia), (ObjectReader r) => new SyntaxTrivia(r));
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteString(Text);
        }

        public static SyntaxTrivia Create(SyntaxKind kind, string text)
        {
            return new SyntaxTrivia(kind, text);
        }

        public override string ToFullString()
        {
            return Text;
        }

        public override string ToString()
        {
            return Text;
        }

        public override GreenNode GetSlot(int index)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override int GetLeadingTriviaWidth()
        {
            return 0;
        }

        public override int GetTrailingTriviaWidth()
        {
            return 0;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new SyntaxTrivia(base.Kind, Text, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new SyntaxTrivia(base.Kind, Text, GetDiagnostics(), annotations);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitTrivia(this);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitTrivia(this);
        }

        protected override void WriteTriviaTo(TextWriter writer)
        {
            writer.Write(Text);
        }

        public static implicit operator Microsoft.CodeAnalysis.SyntaxTrivia(SyntaxTrivia trivia)
        {
            Microsoft.CodeAnalysis.SyntaxToken token = default(Microsoft.CodeAnalysis.SyntaxToken);
            return new Microsoft.CodeAnalysis.SyntaxTrivia(in token, trivia, 0, 0);
        }

        public override bool IsEquivalentTo(GreenNode? other)
        {
            if (!base.IsEquivalentTo(other))
            {
                return false;
            }
            if (Text != ((SyntaxTrivia)other).Text)
            {
                return false;
            }
            return true;
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
