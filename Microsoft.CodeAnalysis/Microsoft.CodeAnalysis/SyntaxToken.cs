using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    [StructLayout(LayoutKind.Auto)]public readonly struct SyntaxToken : IEquatable<SyntaxToken>
    {
        private static readonly Func<DiagnosticInfo, Diagnostic> s_createDiagnosticWithoutLocation = Diagnostic.Create;

        public static readonly Func<SyntaxToken, bool> NonZeroWidth = (SyntaxToken t) => t.Width > 0;

        public static readonly Func<SyntaxToken, bool> Any = (SyntaxToken t) => true;

        public int RawKind => Node?.RawKind ?? 0;

        public string Language => Node?.Language ?? string.Empty;

        public int RawContextualKind => Node?.RawContextualKind ?? 0;

        public SyntaxNode? Parent { get; }

        public GreenNode? Node { get; }

        internal GreenNode RequiredNode => Node;

        internal int Index { get; }

        public int Position { get; }

        public int Width => Node?.Width ?? 0;

        public int FullWidth => Node?.FullWidth ?? 0;

        public TextSpan Span
        {
            get
            {
                if (Node == null)
                {
                    return default(TextSpan);
                }
                return new TextSpan(Position + Node!.GetLeadingTriviaWidth(), Node!.Width);
            }
        }

        public int EndPosition
        {
            get
            {
                if (Node == null)
                {
                    return 0;
                }
                return Position + Node!.FullWidth;
            }
        }

        public int SpanStart
        {
            get
            {
                if (Node == null)
                {
                    return 0;
                }
                return Position + Node!.GetLeadingTriviaWidth();
            }
        }

        public TextSpan FullSpan => new TextSpan(Position, FullWidth);

        public bool IsMissing => Node?.IsMissing ?? false;

        public object? Value => Node?.GetValue();

        public string ValueText => Node?.GetValueText() ?? string.Empty;

        public string Text => ToString();

        public bool HasLeadingTrivia => LeadingTrivia.Count > 0;

        public bool HasTrailingTrivia => TrailingTrivia.Count > 0;

        public int LeadingWidth => Node?.GetLeadingTriviaWidth() ?? 0;

        internal int TrailingWidth => Node?.GetTrailingTriviaWidth() ?? 0;

        public bool ContainsDiagnostics => Node?.ContainsDiagnostics ?? false;

        public bool ContainsDirectives => Node?.ContainsDirectives ?? false;

        public bool HasStructuredTrivia => Node?.ContainsStructuredTrivia ?? false;

        public bool ContainsAnnotations => Node?.ContainsAnnotations ?? false;

        public SyntaxTriviaList LeadingTrivia
        {
            get
            {
                if (Node == null)
                {
                    return default(SyntaxTriviaList);
                }
                return new SyntaxTriviaList(in this, Node!.GetLeadingTriviaCore(), Position);
            }
        }

        public SyntaxTriviaList TrailingTrivia
        {
            get
            {
                if (Node == null)
                {
                    return default(SyntaxTriviaList);
                }
                GreenNode leadingTriviaCore = Node!.GetLeadingTriviaCore();
                int index = 0;
                if (leadingTriviaCore != null)
                {
                    index = ((!leadingTriviaCore.IsList) ? 1 : leadingTriviaCore.SlotCount);
                }
                GreenNode trailingTriviaCore = Node!.GetTrailingTriviaCore();
                int num = Position + FullWidth;
                if (trailingTriviaCore != null)
                {
                    num -= trailingTriviaCore.FullWidth;
                }
                return new SyntaxTriviaList(in this, trailingTriviaCore, num, index);
            }
        }

        public SyntaxTree? SyntaxTree => Parent?.SyntaxTree;

        public SyntaxToken(SyntaxNode? parent, GreenNode? token, int position, int index)
        {
            Parent = parent;
            Node = token;
            Position = position;
            Index = index;
        }

        public SyntaxToken(GreenNode? token)
        {
            this = default(SyntaxToken);
            Node = token;
        }

        private string GetDebuggerDisplay()
        {
            return GetType().Name + " " + ((Node != null) ? Node!.KindText : "None") + " " + ToString();
        }

        public override string ToString()
        {
            if (Node == null)
            {
                return string.Empty;
            }
            return Node!.ToString();
        }

        public string ToFullString()
        {
            if (Node == null)
            {
                return string.Empty;
            }
            return Node!.ToFullString();
        }

        public void WriteTo(TextWriter writer)
        {
            Node?.WriteTo(writer);
        }

        public void WriteTo(TextWriter writer, bool leading, bool trailing)
        {
            Node?.WriteTo(writer, leading, trailing);
        }

        public bool IsPartOfStructuredTrivia()
        {
            return Parent?.IsPartOfStructuredTrivia() ?? false;
        }

        public bool HasAnnotations(string annotationKind)
        {
            return Node?.HasAnnotations(annotationKind) ?? false;
        }

        public bool HasAnnotations(params string[] annotationKinds)
        {
            return Node?.HasAnnotations(annotationKinds) ?? false;
        }

        public bool HasAnnotation([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] SyntaxAnnotation? annotation)
        {
            return Node?.HasAnnotation(annotation) ?? false;
        }

        public IEnumerable<SyntaxAnnotation> GetAnnotations(string annotationKind)
        {
            return Node?.GetAnnotations(annotationKind) ?? SpecializedCollections.EmptyEnumerable<SyntaxAnnotation>();
        }

        public IEnumerable<SyntaxAnnotation> GetAnnotations(params string[] annotationKinds)
        {
            return GetAnnotations((IEnumerable<string>)annotationKinds);
        }

        public IEnumerable<SyntaxAnnotation> GetAnnotations(IEnumerable<string> annotationKinds)
        {
            return Node?.GetAnnotations(annotationKinds) ?? SpecializedCollections.EmptyEnumerable<SyntaxAnnotation>();
        }

        public SyntaxToken WithAdditionalAnnotations(params SyntaxAnnotation[] annotations)
        {
            return WithAdditionalAnnotations((IEnumerable<SyntaxAnnotation>)annotations);
        }

        public SyntaxToken WithAdditionalAnnotations(IEnumerable<SyntaxAnnotation> annotations)
        {
            if (annotations == null)
            {
                throw new ArgumentNullException("annotations");
            }
            if (Node != null)
            {
                return new SyntaxToken(null, Node.WithAdditionalAnnotationsGreen(annotations), 0, 0);
            }
            return default(SyntaxToken);
        }

        public SyntaxToken WithoutAnnotations(params SyntaxAnnotation[] annotations)
        {
            return WithoutAnnotations((IEnumerable<SyntaxAnnotation>)annotations);
        }

        public SyntaxToken WithoutAnnotations(IEnumerable<SyntaxAnnotation> annotations)
        {
            if (annotations == null)
            {
                throw new ArgumentNullException("annotations");
            }
            if (Node != null)
            {
                return new SyntaxToken(null, Node.WithoutAnnotationsGreen(annotations), 0, 0);
            }
            return default(SyntaxToken);
        }

        public SyntaxToken WithoutAnnotations(string annotationKind)
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

        public SyntaxToken CopyAnnotationsTo(SyntaxToken token)
        {
            if (token.Node == null)
            {
                return default(SyntaxToken);
            }
            if (Node == null)
            {
                return token;
            }
            SyntaxAnnotation[] annotations = Node!.GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                return new SyntaxToken(null, token.Node.WithAdditionalAnnotationsGreen(annotations), 0, 0);
            }
            return token;
        }

        public SyntaxToken WithTriviaFrom(SyntaxToken token)
        {
            return WithLeadingTrivia(token.LeadingTrivia).WithTrailingTrivia(token.TrailingTrivia);
        }

        public SyntaxToken WithLeadingTrivia(SyntaxTriviaList trivia)
        {
            return WithLeadingTrivia((IEnumerable<SyntaxTrivia>?)trivia);
        }

        public SyntaxToken WithLeadingTrivia(params SyntaxTrivia[]? trivia)
        {
            return WithLeadingTrivia((IEnumerable<SyntaxTrivia>?)trivia);
        }

        public SyntaxToken WithLeadingTrivia(IEnumerable<SyntaxTrivia>? trivia)
        {
            if (Node == null)
            {
                return default(SyntaxToken);
            }
            return new SyntaxToken(null, Node!.WithLeadingTrivia(GreenNode.CreateList(trivia, (SyntaxTrivia t) => t.RequiredUnderlyingNode)), 0, 0);
        }

        public SyntaxToken WithTrailingTrivia(SyntaxTriviaList trivia)
        {
            return WithTrailingTrivia((IEnumerable<SyntaxTrivia>?)trivia);
        }

        public SyntaxToken WithTrailingTrivia(params SyntaxTrivia[]? trivia)
        {
            return WithTrailingTrivia((IEnumerable<SyntaxTrivia>?)trivia);
        }

        public SyntaxToken WithTrailingTrivia(IEnumerable<SyntaxTrivia>? trivia)
        {
            if (Node == null)
            {
                return default(SyntaxToken);
            }
            return new SyntaxToken(null, Node!.WithTrailingTrivia(GreenNode.CreateList(trivia, (SyntaxTrivia t) => t.RequiredUnderlyingNode)), 0, 0);
        }

        public IEnumerable<SyntaxTrivia> GetAllTrivia()
        {
            if (HasLeadingTrivia)
            {
                if (HasTrailingTrivia)
                {
                    return LeadingTrivia.Concat(TrailingTrivia);
                }
                return LeadingTrivia;
            }
            if (HasTrailingTrivia)
            {
                return TrailingTrivia;
            }
            return SpecializedCollections.EmptyEnumerable<SyntaxTrivia>();
        }

        public static bool operator ==(SyntaxToken left, SyntaxToken right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SyntaxToken left, SyntaxToken right)
        {
            return !left.Equals(right);
        }

        public bool Equals(SyntaxToken other)
        {
            if (Parent == other.Parent && Node == other.Node && Position == other.Position)
            {
                return Index == other.Index;
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is SyntaxToken)
            {
                return Equals((SyntaxToken)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Parent, Hash.Combine(Node, Hash.Combine(Position, Index)));
        }

        public SyntaxToken GetNextToken(bool includeZeroWidth = false, bool includeSkipped = false, bool includeDirectives = false, bool includeDocumentationComments = false)
        {
            if (Node == null)
            {
                return default(SyntaxToken);
            }
            return SyntaxNavigator.Instance.GetNextToken(in this, includeZeroWidth, includeSkipped, includeDirectives, includeDocumentationComments);
        }

        public SyntaxToken GetNextToken(Func<SyntaxToken, bool> predicate, Func<SyntaxTrivia, bool>? stepInto = null)
        {
            if (Node == null)
            {
                return default(SyntaxToken);
            }
            return SyntaxNavigator.Instance.GetNextToken(in this, predicate, stepInto);
        }

        public SyntaxToken GetPreviousToken(bool includeZeroWidth = false, bool includeSkipped = false, bool includeDirectives = false, bool includeDocumentationComments = false)
        {
            if (Node == null)
            {
                return default(SyntaxToken);
            }
            return SyntaxNavigator.Instance.GetPreviousToken(in this, includeZeroWidth, includeSkipped, includeDirectives, includeDocumentationComments);
        }

        public SyntaxToken GetPreviousToken(Func<SyntaxToken, bool> predicate, Func<SyntaxTrivia, bool>? stepInto = null)
        {
            return SyntaxNavigator.Instance.GetPreviousToken(in this, predicate, stepInto);
        }

        public Location GetLocation()
        {
            SyntaxTree syntaxTree = SyntaxTree;
            if (syntaxTree != null)
            {
                return syntaxTree.GetLocation(Span);
            }
            return Location.None;
        }

        public IEnumerable<Diagnostic> GetDiagnostics()
        {
            if (Node == null)
            {
                return SpecializedCollections.EmptyEnumerable<Diagnostic>();
            }
            SyntaxTree syntaxTree = SyntaxTree;
            if (syntaxTree == null)
            {
                DiagnosticInfo[] diagnostics = Node!.GetDiagnostics();
                if (diagnostics.Length != 0)
                {
                    return diagnostics.Select<DiagnosticInfo, Diagnostic>(s_createDiagnosticWithoutLocation);
                }
                return SpecializedCollections.EmptyEnumerable<Diagnostic>();
            }
            return syntaxTree.GetDiagnostics(this);
        }

        public bool IsEquivalentTo(SyntaxToken token)
        {
            if (Node != null || token.Node != null)
            {
                if (Node != null && token.Node != null)
                {
                    return Node!.IsEquivalentTo(token.Node);
                }
                return false;
            }
            return true;
        }

        public bool IsIncrementallyIdenticalTo(SyntaxToken token)
        {
            if (Node != null)
            {
                return Node == token.Node;
            }
            return false;
        }
    }
}
