using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

namespace Roslyn.Utilities
{
    internal static class TextChangeRangeExtensions
    {
        private readonly struct UnadjustedNewChange
        {
            public int SpanStart { get; }

            public int SpanLength { get; }

            public int NewLength { get; }

            public int SpanEnd => SpanStart + SpanLength;

            public UnadjustedNewChange(int spanStart, int spanLength, int newLength)
            {
                SpanStart = spanStart;
                SpanLength = spanLength;
                NewLength = newLength;
            }

            public UnadjustedNewChange(TextChangeRange range)
                : this(range.Span.Start, range.Span.Length, range.NewLength)
            {
            }
        }

        public static TextChangeRange? Accumulate(this TextChangeRange? accumulatedTextChangeSoFar, IEnumerable<TextChangeRange> changesInNextVersion)
        {
            if (!changesInNextVersion.Any())
            {
                return accumulatedTextChangeSoFar;
            }
            TextChangeRange value = TextChangeRange.Collapse(changesInNextVersion);
            if (!accumulatedTextChangeSoFar.HasValue)
            {
                return value;
            }
            int start = accumulatedTextChangeSoFar.Value.Span.Start;
            int num = accumulatedTextChangeSoFar.Value.Span.End;
            int num2 = accumulatedTextChangeSoFar.Value.Span.Start + accumulatedTextChangeSoFar.Value.NewLength;
            if (value.Span.Start < start)
            {
                start = value.Span.Start;
            }
            if (num2 > value.Span.End)
            {
                num2 = num2 + value.NewLength - value.Span.Length;
            }
            else
            {
                num = num + value.Span.End - num2;
                num2 = value.Span.Start + value.NewLength;
            }
            return new TextChangeRange(TextSpan.FromBounds(start, num), num2 - start);
        }

        public static TextChangeRange ToTextChangeRange(this TextChange textChange)
        {
            return new TextChangeRange(textChange.Span, textChange.NewText?.Length ?? 0);
        }

        public static ImmutableArray<TextChangeRange> Merge(ImmutableArray<TextChangeRange> oldChanges, ImmutableArray<TextChangeRange> newChanges)
        {
            if (oldChanges.IsEmpty)
            {
                throw new ArgumentException("oldChanges");
            }
            if (newChanges.IsEmpty)
            {
                throw new ArgumentException("newChanges");
            }
            ArrayBuilder<TextChangeRange> instance = ArrayBuilder<TextChangeRange>.GetInstance();
            TextChangeRange oldChange2 = oldChanges[0];
            UnadjustedNewChange newChange2 = new UnadjustedNewChange(newChanges[0]);
            int oldIndex = 0;
            int newIndex = 0;
            int oldDelta2 = 0;
            while (true)
            {
                if (oldChange2.Span.Length == 0 && oldChange2.NewLength == 0)
                {
                    if (!tryGetNextOldChange())
                    {
                        break;
                    }
                }
                else if (newChange2.SpanLength == 0 && newChange2.NewLength == 0)
                {
                    if (!tryGetNextNewChange())
                    {
                        break;
                    }
                }
                else if (newChange2.SpanEnd <= oldChange2.Span.Start + oldDelta2)
                {
                    adjustAndAddNewChange(instance, oldDelta2, newChange2);
                    if (!tryGetNextNewChange())
                    {
                        break;
                    }
                }
                else if (newChange2.SpanStart >= oldChange2.NewEnd() + oldDelta2)
                {
                    addAndAdjustOldDelta(instance, ref oldDelta2, oldChange2);
                    if (!tryGetNextOldChange())
                    {
                        break;
                    }
                }
                else if (newChange2.SpanStart < oldChange2.Span.Start + oldDelta2)
                {
                    int num = oldChange2.Span.Start + oldDelta2 - newChange2.SpanStart;
                    adjustAndAddNewChange(instance, oldDelta2, new UnadjustedNewChange(newChange2.SpanStart, num, 0));
                    newChange2 = new UnadjustedNewChange(oldChange2.Span.Start + oldDelta2, newChange2.SpanLength - num, newChange2.NewLength);
                }
                else if (newChange2.SpanStart > oldChange2.Span.Start + oldDelta2)
                {
                    int num2 = newChange2.SpanStart - (oldChange2.Span.Start + oldDelta2);
                    int num3 = Math.Min(oldChange2.Span.Length, num2);
                    addAndAdjustOldDelta(instance, ref oldDelta2, new TextChangeRange(new TextSpan(oldChange2.Span.Start, num3), num2));
                    oldChange2 = new TextChangeRange(new TextSpan(newChange2.SpanStart - oldDelta2, oldChange2.Span.Length - num3), oldChange2.NewLength - num2);
                }
                else if (newChange2.SpanLength <= oldChange2.NewLength)
                {
                    oldChange2 = new TextChangeRange(oldChange2.Span, oldChange2.NewLength - newChange2.SpanLength);
                    oldDelta2 += newChange2.SpanLength;
                    newChange2 = new UnadjustedNewChange(newChange2.SpanEnd, 0, newChange2.NewLength);
                    adjustAndAddNewChange(instance, oldDelta2, newChange2);
                    if (!tryGetNextNewChange())
                    {
                        break;
                    }
                }
                else
                {
                    oldDelta2 = oldDelta2 - oldChange2.Span.Length + oldChange2.NewLength;
                    int spanLength = newChange2.SpanLength + oldChange2.Span.Length - oldChange2.NewLength;
                    newChange2 = new UnadjustedNewChange(oldChange2.Span.Start + oldDelta2, spanLength, newChange2.NewLength);
                    if (!tryGetNextOldChange())
                    {
                        break;
                    }
                }
            }
            bool num4 = oldIndex == oldChanges.Length;
            bool flag = newIndex == newChanges.Length;
            if (num4)
            {
                if (flag)
                {
                    goto IL_044b;
                }
            }
            else if (!flag)
            {
                goto IL_044b;
            }
            while (oldIndex < oldChanges.Length)
            {
                addAndAdjustOldDelta(instance, ref oldDelta2, oldChange2);
                tryGetNextOldChange();
            }
            while (newIndex < newChanges.Length)
            {
                adjustAndAddNewChange(instance, oldDelta2, newChange2);
                tryGetNextNewChange();
            }
            return instance.ToImmutableAndFree();
        IL_044b:
            throw new InvalidOperationException();
            static void add(ArrayBuilder<TextChangeRange> builder, TextChangeRange change)
            {
                if (builder.Count > 0)
                {
                    TextChangeRange textChangeRange = builder[^1];
                    if (textChangeRange.Span.End == change.Span.Start)
                    {
                        builder[^1] = new TextChangeRange(new TextSpan(textChangeRange.Span.Start, textChangeRange.Span.Length + change.Span.Length), textChangeRange.NewLength + change.NewLength);
                        return;
                    }
                    if (textChangeRange.Span.End > change.Span.Start)
                    {
                        throw new ArgumentOutOfRangeException("change");
                    }
                }
                builder.Add(change);
            }
            static void addAndAdjustOldDelta(ArrayBuilder<TextChangeRange> builder, ref int oldDelta, TextChangeRange oldChange)
            {
                oldDelta = oldDelta - oldChange.Span.Length + oldChange.NewLength;
                add(builder, oldChange);
            }
            static void adjustAndAddNewChange(ArrayBuilder<TextChangeRange> builder, int oldDelta, UnadjustedNewChange newChange)
            {
                add(builder, new TextChangeRange(new TextSpan(newChange.SpanStart - oldDelta, newChange.SpanLength), newChange.NewLength));
            }
            bool tryGetNextNewChange()
            {
                newIndex++;
                if (newIndex < newChanges.Length)
                {
                    newChange2 = new UnadjustedNewChange(newChanges[newIndex]);
                    return true;
                }
                newChange2 = default(UnadjustedNewChange);
                return false;
            }
            bool tryGetNextOldChange()
            {
                oldIndex++;
                if (oldIndex < oldChanges.Length)
                {
                    oldChange2 = oldChanges[oldIndex];
                    return true;
                }
                oldChange2 = default(TextChangeRange);
                return false;
            }
        }

        private static int NewEnd(this TextChangeRange range)
        {
            return range.Span.Start + range.NewLength;
        }
    }
}
