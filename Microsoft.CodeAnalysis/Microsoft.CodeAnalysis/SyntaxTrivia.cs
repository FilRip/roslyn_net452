using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    public readonly struct SyntaxTrivia : IEquatable<SyntaxTrivia>
    {
        public static readonly Func<SyntaxTrivia, bool> Any = (SyntaxTrivia t) => true;

        public int RawKind => UnderlyingNode?.RawKind ?? 0;

        public string Language => UnderlyingNode?.Language ?? string.Empty;

        public SyntaxToken Token { get; }

        public GreenNode? UnderlyingNode { get; }

        internal GreenNode RequiredUnderlyingNode => UnderlyingNode;

        public int Position { get; }

        internal int Index { get; }

        internal int Width => UnderlyingNode?.Width ?? 0;

        public int FullWidth => UnderlyingNode?.FullWidth ?? 0;

        public TextSpan Span
        {
            get
            {
                if (UnderlyingNode == null)
                {
                    return default(TextSpan);
                }
                return new TextSpan(Position + UnderlyingNode!.GetLeadingTriviaWidth(), UnderlyingNode!.Width);
            }
        }

        public int SpanStart
        {
            get
            {
                if (UnderlyingNode == null)
                {
                    return 0;
                }
                return Position + UnderlyingNode!.GetLeadingTriviaWidth();
            }
        }

        public TextSpan FullSpan
        {
            get
            {
                if (UnderlyingNode == null)
                {
                    return default(TextSpan);
                }
                return new TextSpan(Position, UnderlyingNode!.FullWidth);
            }
        }

        public bool ContainsDiagnostics => UnderlyingNode?.ContainsDiagnostics ?? false;

        public bool HasStructure => UnderlyingNode?.IsStructuredTrivia ?? false;

        internal bool ContainsAnnotations => UnderlyingNode?.ContainsAnnotations ?? false;

        public bool IsDirective => UnderlyingNode?.IsDirective ?? false;

        internal bool IsSkippedTokensTrivia => UnderlyingNode?.IsSkippedTokensTrivia ?? false;

        internal bool IsDocumentationCommentTrivia => UnderlyingNode?.IsDocumentationCommentTrivia ?? false;

        public SyntaxTree? SyntaxTree => Token.SyntaxTree;

        public SyntaxTrivia(in SyntaxToken token, GreenNode? triviaNode, int position, int index)
        {
            Token = token;
            UnderlyingNode = triviaNode;
            Position = position;
            Index = index;
        }

        private string GetDebuggerDisplay()
        {
            return GetType().Name + " " + (UnderlyingNode?.KindText ?? "None") + " " + ToString();
        }

        public bool IsPartOfStructuredTrivia()
        {
            return Token.Parent?.IsPartOfStructuredTrivia() ?? false;
        }

        public bool HasAnnotations(string annotationKind)
        {
            return UnderlyingNode?.HasAnnotations(annotationKind) ?? false;
        }

        public bool HasAnnotations(params string[] annotationKinds)
        {
            return UnderlyingNode?.HasAnnotations(annotationKinds) ?? false;
        }

        public bool HasAnnotation([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] SyntaxAnnotation? annotation)
        {
            return UnderlyingNode?.HasAnnotation(annotation) ?? false;
        }

        public IEnumerable<SyntaxAnnotation> GetAnnotations(string annotationKind)
        {
            if (UnderlyingNode == null)
            {
                return SpecializedCollections.EmptyEnumerable<SyntaxAnnotation>();
            }
            return UnderlyingNode!.GetAnnotations(annotationKind);
        }

        public IEnumerable<SyntaxAnnotation> GetAnnotations(params string[] annotationKinds)
        {
            if (UnderlyingNode == null)
            {
                return SpecializedCollections.EmptyEnumerable<SyntaxAnnotation>();
            }
            return UnderlyingNode!.GetAnnotations(annotationKinds);
        }

        public SyntaxNode? GetStructure()
        {
            if (!HasStructure)
            {
                return null;
            }
            return UnderlyingNode!.GetStructure(this);
        }

        internal bool TryGetStructure([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out SyntaxNode? structure)
        {
            structure = GetStructure();
            return structure != null;
        }

        public override string ToString()
        {
            if (UnderlyingNode == null)
            {
                return string.Empty;
            }
            return UnderlyingNode!.ToString();
        }

        public string ToFullString()
        {
            if (UnderlyingNode == null)
            {
                return string.Empty;
            }
            return UnderlyingNode!.ToFullString();
        }

        public void WriteTo(TextWriter writer)
        {
            UnderlyingNode?.WriteTo(writer);
        }

        public static bool operator ==(SyntaxTrivia left, SyntaxTrivia right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SyntaxTrivia left, SyntaxTrivia right)
        {
            return !left.Equals(right);
        }

        public bool Equals(SyntaxTrivia other)
        {
            if (Token == other.Token && UnderlyingNode == other.UnderlyingNode && Position == other.Position)
            {
                return Index == other.Index;
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is SyntaxTrivia other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Token.GetHashCode(), Hash.Combine(UnderlyingNode, Hash.Combine(Position, Index)));
        }

        public SyntaxTrivia WithAdditionalAnnotations(params SyntaxAnnotation[] annotations)
        {
            return WithAdditionalAnnotations((IEnumerable<SyntaxAnnotation>)annotations);
        }

        public SyntaxTrivia WithAdditionalAnnotations(IEnumerable<SyntaxAnnotation> annotations)
        {
            if (annotations == null)
            {
                throw new ArgumentNullException("annotations");
            }
            if (UnderlyingNode != null)
            {
                SyntaxToken token = default(SyntaxToken);
                return new SyntaxTrivia(in token, UnderlyingNode.WithAdditionalAnnotationsGreen(annotations), 0, 0);
            }
            return default(SyntaxTrivia);
        }

        public SyntaxTrivia WithoutAnnotations(params SyntaxAnnotation[] annotations)
        {
            return WithoutAnnotations((IEnumerable<SyntaxAnnotation>)annotations);
        }

        public SyntaxTrivia WithoutAnnotations(IEnumerable<SyntaxAnnotation> annotations)
        {
            if (annotations == null)
            {
                throw new ArgumentNullException("annotations");
            }
            if (UnderlyingNode != null)
            {
                SyntaxToken token = default(SyntaxToken);
                return new SyntaxTrivia(in token, UnderlyingNode.WithoutAnnotationsGreen(annotations), 0, 0);
            }
            return default(SyntaxTrivia);
        }

        public SyntaxTrivia WithoutAnnotations(string annotationKind)
        {
            if (annotationKind == null)
            {
                throw new ArgumentNullException("annotationKind");
            }
            if (HasAnnotations(annotationKind))
            {
                return WithoutAnnotations(GetAnnotations(annotationKind));
            }
            return this;
        }

        public SyntaxTrivia CopyAnnotationsTo(SyntaxTrivia trivia)
        {
            if (trivia.UnderlyingNode == null)
            {
                return default(SyntaxTrivia);
            }
            if (UnderlyingNode == null)
            {
                return trivia;
            }
            SyntaxAnnotation[] annotations = UnderlyingNode!.GetAnnotations();
            if (annotations == null || annotations.Length == 0)
            {
                return trivia;
            }
            SyntaxToken token = default(SyntaxToken);
            return new SyntaxTrivia(in token, trivia.UnderlyingNode.WithAdditionalAnnotationsGreen(annotations), 0, 0);
        }

        public Location GetLocation()
        {
            return SyntaxTree!.GetLocation(Span);
        }

        public IEnumerable<Diagnostic> GetDiagnostics()
        {
            return SyntaxTree!.GetDiagnostics(this);
        }

        public bool IsEquivalentTo(SyntaxTrivia trivia)
        {
            if (UnderlyingNode != null || trivia.UnderlyingNode != null)
            {
                if (UnderlyingNode != null && trivia.UnderlyingNode != null)
                {
                    return UnderlyingNode!.IsEquivalentTo(trivia.UnderlyingNode);
                }
                return false;
            }
            return true;
        }
    }
}
