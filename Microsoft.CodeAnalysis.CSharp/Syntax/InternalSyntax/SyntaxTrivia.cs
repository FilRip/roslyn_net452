// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    internal class SyntaxTrivia : CSharpSyntaxNode
    {
        public readonly string Text;

        internal SyntaxTrivia(SyntaxKind kind, string text, DiagnosticInfo[]? diagnostics = null, SyntaxAnnotation[]? annotations = null)
            : base(kind, diagnostics, annotations, text.Length)
        {
            this.Text = text;
            if (kind == SyntaxKind.PreprocessingMessageTrivia)
            {
                this.flags |= NodeFlags.ContainsSkippedText;
            }
        }

        internal SyntaxTrivia(ObjectReader reader)
            : base(reader)
        {
            this.Text = reader.ReadString();
            this.FullWidth = this.Text.Length;
        }

        static SyntaxTrivia()
        {
            ObjectBinder.RegisterTypeReader(typeof(SyntaxTrivia), r => new SyntaxTrivia(r));
        }

        public override bool IsTrivia => true;

        public override bool ShouldReuseInSerialization => this.Kind == SyntaxKind.WhitespaceTrivia &&
                                                             FullWidth < Lexer.MaxCachedTokenSize;

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteString(this.Text);
        }

        internal static SyntaxTrivia Create(SyntaxKind kind, string text)
        {
            return new SyntaxTrivia(kind, text);
        }

        public override string ToFullString()
        {
            return this.Text;
        }

        public override string ToString()
        {
            return this.Text;
        }

        public override GreenNode GetSlot(int index)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override int Width
        {
            get
            {
                return this.FullWidth;
            }
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
            return new SyntaxTrivia(this.Kind, this.Text, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new SyntaxTrivia(this.Kind, this.Text, GetDiagnostics(), annotations);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitTrivia(this);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitTrivia(this);
        }

        protected override void WriteTriviaTo(System.IO.TextWriter writer)
        {
            writer.Write(Text);
        }

        public static implicit operator CodeAnalysis.SyntaxTrivia(SyntaxTrivia trivia)
        {
            return new CodeAnalysis.SyntaxTrivia(token: default, trivia, position: 0, index: 0);
        }

        public override bool IsEquivalentTo(GreenNode? other)
        {
            if (!base.IsEquivalentTo(other))
            {
                return false;
            }

            if (this.Text != ((SyntaxTrivia)other).Text)
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
