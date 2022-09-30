using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Text
{
    [DataContract]
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public readonly struct TextChange : IEquatable<TextChange>
    {
        [DataMember(Order = 0)]
        public TextSpan Span { get; }

        [DataMember(Order = 1)]
        public string? NewText { get; }

        public static IReadOnlyList<TextChange> NoChanges => SpecializedCollections.EmptyReadOnlyList<TextChange>();

        public TextChange(TextSpan span, string newText)
        {
            this = default(TextChange);
            if (newText == null)
            {
                throw new ArgumentNullException("newText");
            }
            Span = span;
            NewText = newText;
        }

        public override string ToString()
        {
            return $"{GetType().Name}: {{ {Span}, \"{NewText}\" }}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is TextChange)
            {
                return Equals((TextChange)obj);
            }
            return false;
        }

        public bool Equals(TextChange other)
        {
            if (EqualityComparer<TextSpan>.Default.Equals(Span, other.Span))
            {
                return EqualityComparer<string>.Default.Equals(NewText, other.NewText);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Span.GetHashCode(), NewText?.GetHashCode() ?? 0);
        }

        public static bool operator ==(TextChange left, TextChange right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextChange left, TextChange right)
        {
            return !(left == right);
        }

        public static implicit operator TextChangeRange(TextChange change)
        {
            return new TextChangeRange(change.Span, change.NewText!.Length);
        }

        internal string GetDebuggerDisplay()
        {
            string newText = NewText;
            string text;
            if (newText != null)
            {
                int length = newText.Length;
                text = ((length >= 10) ? $"(NewLength = {length})" : ("\"" + NewText + "\""));
            }
            else
            {
                text = "null";
            }
            string arg = text;
            return $"new TextChange(new TextSpan({Span.Start}, {Span.Length}), {arg})";
        }
    }
}
