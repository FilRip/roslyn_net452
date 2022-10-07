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

        /// <summary>
        /// Merges the new change ranges into the old change ranges, adjusting the new ranges to be with respect to the original text
        /// (with neither old or new changes applied) instead of with respect to the original text after "old changes" are applied.
        ///
        /// This may require splitting, concatenation, etc. of individual change ranges.
        /// </summary>
        /// <remarks>
        /// Both `oldChanges` and `newChanges` must contain non-overlapping spans in ascending order.
        /// </remarks>
        public static ImmutableArray<TextChangeRange> Merge(ImmutableArray<TextChangeRange> oldChanges, ImmutableArray<TextChangeRange> newChanges)
        {
            // Earlier steps are expected to prevent us from ever reaching this point with empty change sets.
            if (oldChanges.IsEmpty)
            {
                throw new ArgumentException(nameof(oldChanges));
            }

            if (newChanges.IsEmpty)
            {
                throw new ArgumentException(nameof(newChanges));
            }

            var builder = ArrayBuilder<TextChangeRange>.GetInstance();

            var oldChange = oldChanges[0];
            var newChange = new UnadjustedNewChange(newChanges[0]);

            var oldIndex = 0;
            var newIndex = 0;

            // The sum of characters inserted by old changes minus characters deleted by old changes.
            // This value must be adjusted whenever characters from an old change are added to `builder`.
            var oldDelta = 0;

            // In this loop we "zip" together potentially overlapping old and new changes.
            // It's important that when overlapping changes are found, we don't consume past the end of the overlapping section until the next iteration.
            // so that we don't miss scenarios where the section after the overlap we found itself overlaps with another change
            // e.g.:
            // [-------oldChange1------]
            // [--newChange1--]   [--newChange2--]
            while (true)
            {
                if (oldChange.Span.Length == 0 && oldChange.NewLength == 0)
                {
                    // old change does not insert or delete any characters, so it can be dropped to no effect.
                    if (tryGetNextOldChange()) continue;
                    else break;
                }
                else if (newChange.SpanLength == 0 && newChange.NewLength == 0)
                {
                    // new change does not insert or delete any characters, so it can be dropped to no effect.
                    if (tryGetNextNewChange()) continue;
                    else break;
                }
                else if (newChange.SpanEnd <= oldChange.Span.Start + oldDelta)
                {
                    // new change is entirely before old change, so just take the new change
                    //                old[--------]
                    // new[--------]
                    adjustAndAddNewChange(builder, oldDelta, newChange);
                    if (tryGetNextNewChange()) continue;
                    else break;
                }
                else if (newChange.SpanStart >= oldChange.NewEnd() + oldDelta)
                {
                    // new change is entirely after old change, so just take the old change
                    // old[--------]
                    //                new[--------]
                    addAndAdjustOldDelta(builder, ref oldDelta, oldChange);
                    if (tryGetNextOldChange()) continue;
                    else break;
                }
                else if (newChange.SpanStart < oldChange.Span.Start + oldDelta)
                {
                    // new change starts before old change, but the new change deletion overlaps with the old change insertion
                    // note: 'd' represents a deleted character, 'a' represents a character inserted by an old change, and 'b' represents a character inserted by a new change.
                    //
                    //    old|dddddd|
                    //       |aaaaaa|
                    // ---------------
                    // new|dddddd|
                    //    |bbbbbb|

                    // align the new change and old change start by consuming the part of the new deletion before the old change
                    // (this only deletes characters of the original text)
                    //
                    // old|dddddd|
                    //    |aaaaaa|
                    // ---------------
                    // new|ddd|
                    //    |bbbbbb|
                    var newChangeLeadingDeletion = oldChange.Span.Start + oldDelta - newChange.SpanStart;
                    adjustAndAddNewChange(builder, oldDelta, new UnadjustedNewChange(newChange.SpanStart, newChangeLeadingDeletion, newLength: 0));
                    newChange = new UnadjustedNewChange(oldChange.Span.Start + oldDelta, newChange.SpanLength - newChangeLeadingDeletion, newChange.NewLength);
                    continue;
                }
                else if (newChange.SpanStart > oldChange.Span.Start + oldDelta)
                {
                    // new change starts after old change, but overlaps
                    //
                    // old|dddddd|
                    //    |aaaaaa|
                    // ---------------
                    //    new|dddddd|
                    //       |bbbbbb|

                    // align the old change to the new change by consuming the part of the old change which is before the new change.
                    //
                    //    old|ddd|
                    //       |aaa|
                    // ---------------
                    //    new|dddddd|
                    //       |bbbbbb|

                    var oldChangeLeadingInsertion = newChange.SpanStart - (oldChange.Span.Start + oldDelta);
                    // we must make sure to delete at most as many characters as the entire oldChange deletes
                    var oldChangeLeadingDeletion = Math.Min(oldChange.Span.Length, oldChangeLeadingInsertion);
                    addAndAdjustOldDelta(builder, ref oldDelta, new TextChangeRange(new TextSpan(oldChange.Span.Start, oldChangeLeadingDeletion), oldChangeLeadingInsertion));
                    oldChange = new TextChangeRange(new TextSpan(newChange.SpanStart - oldDelta, oldChange.Span.Length - oldChangeLeadingDeletion), oldChange.NewLength - oldChangeLeadingInsertion);
                    continue;
                }
                else
                {
                    // old and new change start at same adjusted position

                    if (newChange.SpanLength <= oldChange.NewLength)
                    {
                        // new change deletes fewer characters than old change inserted
                        //
                        // old|dddddd|
                        //    |aaaaaa|
                        // ---------------
                        // new|ddd|
                        //    |bbbbbb|

                        // - apply the new change deletion to the old change insertion
                        //
                        //    old|dddddd|
                        //       |aaa|
                        // ---------------
                        // new||
                        //    |bbbbbb|
                        //
                        // - move the new change insertion forward by the same amount as its consumed deletion to remain aligned with the old change.
                        // (because the old change and new change have the same adjusted start position, the new change insertion appears directly before the old change insertion in the final text)
                        //
                        //    old|dddddd|
                        //       |aaa|
                        // ---------------
                        //    new||
                        //       |bbbbbb|

                        oldChange = new TextChangeRange(oldChange.Span, oldChange.NewLength - newChange.SpanLength);

                        // the new change deletion is equal to the subset of the old change insertion that we are consuming this iteration
                        oldDelta = oldDelta + newChange.SpanLength;

                        // since the new change insertion occurs before the old change, consume it now
                        newChange = new UnadjustedNewChange(newChange.SpanEnd, spanLength: 0, newChange.NewLength);
                        adjustAndAddNewChange(builder, oldDelta, newChange);
                        if (tryGetNextNewChange()) continue;
                        else break;
                    }
                    else
                    {
                        // new change deletes more characters than old change inserted
                        //
                        // old|d|
                        //    |aa|
                        // ---------------
                        // new|ddd|
                        //    |bbb|

                        // merge the old change into the new change:
                        // - new change deletion deletes all of the old change insertion. reduce the new change deletion accordingly
                        //
                        //   old|d|
                        //      ||
                        // ---------------
                        // new|d|
                        //    |bbb|
                        //
                        // - old change deletion is simply added to the new change deletion.
                        //
                        //  old||
                        //     ||
                        // ---------------
                        // new|dd|
                        //    |bbb|
                        //
                        // - new change is moved to put its adjusted position equal to the old change we just merged in
                        //
                        //  old||
                        //     ||
                        // ---------------
                        //  new|dd|
                        //     |bbb|

                        // adjust the oldDelta to reflect that the old change has been consumed
                        oldDelta = oldDelta - oldChange.Span.Length + oldChange.NewLength;

                        var newDeletion = newChange.SpanLength + oldChange.Span.Length - oldChange.NewLength;
                        newChange = new UnadjustedNewChange(oldChange.Span.Start + oldDelta, newDeletion, newChange.NewLength);
                        if (tryGetNextOldChange()) continue;
                        else break;
                    }
                }
            }

            // there may be remaining old changes or remaining new changes (not both, and not neither)
            switch (oldIndex == oldChanges.Length, newIndex == newChanges.Length)
            {
                case (true, true):
                case (false, false):
                    throw new InvalidOperationException();
            }

            while (oldIndex < oldChanges.Length)
            {
                addAndAdjustOldDelta(builder, ref oldDelta, oldChange);
                tryGetNextOldChange();
            }

            while (newIndex < newChanges.Length)
            {
                adjustAndAddNewChange(builder, oldDelta, newChange);
                tryGetNextNewChange();
            }

            return builder.ToImmutableAndFree();

            bool tryGetNextOldChange()
            {
                oldIndex++;
                if (oldIndex < oldChanges.Length)
                {
                    oldChange = oldChanges[oldIndex];
                    return true;
                }
                else
                {
                    oldChange = default;
                    return false;
                }
            }

            bool tryGetNextNewChange()
            {
                newIndex++;
                if (newIndex < newChanges.Length)
                {
                    newChange = new UnadjustedNewChange(newChanges[newIndex]);
                    return true;
                }
                else
                {
                    newChange = default;
                    return false;
                }
            }

            static void addAndAdjustOldDelta(ArrayBuilder<TextChangeRange> builder, ref int oldDelta, TextChangeRange oldChange)
            {
                // modify oldDelta to reflect characters deleted and inserted by an old change
                oldDelta = oldDelta - oldChange.Span.Length + oldChange.NewLength;
                add(builder, oldChange);
            }

            static void adjustAndAddNewChange(ArrayBuilder<TextChangeRange> builder, int oldDelta, UnadjustedNewChange newChange)
            {
                // unadjusted new change is relative to the original text with old changes applied. Subtract oldDelta to make it relative to the original text.
                add(builder, new TextChangeRange(new TextSpan(newChange.SpanStart - oldDelta, newChange.SpanLength), newChange.NewLength));
            }

            static void add(ArrayBuilder<TextChangeRange> builder, TextChangeRange change)
            {
                if (builder.Count > 0)
                {
                    var last = builder[^1];
                    if (last.Span.End == change.Span.Start)
                    {
                        // merge changes together if they are adjacent
                        builder[^1] = new TextChangeRange(new TextSpan(last.Span.Start, last.Span.Length + change.Span.Length), last.NewLength + change.NewLength);
                        return;
                    }
                    else if (last.Span.End > change.Span.Start)
                    {
                        throw new ArgumentOutOfRangeException(nameof(change));
                    }

                }

                builder.Add(change);
            }
        }

        private static int NewEnd(this TextChangeRange range)
        {
            return range.Span.Start + range.NewLength;
        }
    }
}
