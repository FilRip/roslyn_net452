using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public class SyntaxDiffer
    {
        private enum DiffOp
        {
            None,
            SkipBoth,
            ReduceOld,
            ReduceNew,
            ReduceBoth,
            InsertNew,
            DeleteOld,
            ReplaceOldWithNew
        }

        private struct DiffAction
        {
            public readonly DiffOp Operation;

            public readonly int Count;

            public DiffAction(DiffOp operation, int count)
            {
                Operation = operation;
                Count = count;
            }
        }

        private readonly struct ChangeRecord
        {
            public readonly TextChangeRange Range;

            public readonly Queue<SyntaxNodeOrToken>? OldNodes;

            public readonly Queue<SyntaxNodeOrToken>? NewNodes;

            internal ChangeRecord(TextChangeRange range, Queue<SyntaxNodeOrToken>? oldNodes, Queue<SyntaxNodeOrToken>? newNodes)
            {
                Range = range;
                OldNodes = oldNodes;
                NewNodes = newNodes;
            }
        }

        private readonly struct ChangeRangeWithText
        {
            public readonly TextChangeRange Range;

            public readonly string? NewText;

            public ChangeRangeWithText(TextChangeRange range, string? newText)
            {
                Range = range;
                NewText = newText;
            }
        }

        private const int InitialStackSize = 8;

        private const int MaxSearchLength = 8;

        private readonly Stack<SyntaxNodeOrToken> _oldNodes = new Stack<SyntaxNodeOrToken>(8);

        private readonly Stack<SyntaxNodeOrToken> _newNodes = new Stack<SyntaxNodeOrToken>(8);

        private readonly List<ChangeRecord> _changes = new List<ChangeRecord>();

        private readonly TextSpan _oldSpan;

        private readonly bool _computeNewText;

        private readonly HashSet<GreenNode> _nodeSimilaritySet = new HashSet<GreenNode>();

        private readonly HashSet<string> _tokenTextSimilaritySet = new HashSet<string>();

        private SyntaxDiffer(SyntaxNode oldNode, SyntaxNode newNode, bool computeNewText)
        {
            _oldNodes.Push(oldNode);
            _newNodes.Push(newNode);
            _oldSpan = oldNode.FullSpan;
            _computeNewText = computeNewText;
        }

        public static IList<TextChange> GetTextChanges(SyntaxTree before, SyntaxTree after)
        {
            if (before == after)
            {
                return SpecializedCollections.EmptyList<TextChange>();
            }
            if (before == null)
            {
                return new TextChange[1]
                {
                    new TextChange(new TextSpan(0, 0), after.GetText().ToString())
                };
            }
            if (after == null)
            {
                throw new ArgumentNullException("after");
            }
            return GetTextChanges(before.GetRoot(), after.GetRoot());
        }

        internal static IList<TextChange> GetTextChanges(SyntaxNode oldNode, SyntaxNode newNode)
        {
            return new SyntaxDiffer(oldNode, newNode, computeNewText: true).ComputeTextChangesFromOld();
        }

        private IList<TextChange> ComputeTextChangesFromOld()
        {
            ComputeChangeRecords();
            return (from c in ReduceChanges(_changes)
                    select new TextChange(c.Range.Span, c.NewText)).ToList();
        }

        public static IList<TextSpan> GetPossiblyDifferentTextSpans(SyntaxTree? before, SyntaxTree? after)
        {
            if (before == after)
            {
                return SpecializedCollections.EmptyList<TextSpan>();
            }
            if (before == null)
            {
                return new TextSpan[1]
                {
                    new TextSpan(0, after!.GetText().Length)
                };
            }
            if (after == null)
            {
                throw new ArgumentNullException("after");
            }
            return GetPossiblyDifferentTextSpans(before!.GetRoot(), after!.GetRoot());
        }

        public static IList<TextSpan> GetPossiblyDifferentTextSpans(SyntaxNode oldNode, SyntaxNode newNode)
        {
            return new SyntaxDiffer(oldNode, newNode, computeNewText: false).ComputeSpansInNew();
        }

        private IList<TextSpan> ComputeSpansInNew()
        {
            ComputeChangeRecords();
            List<ChangeRangeWithText> list = ReduceChanges(_changes);
            List<TextSpan> list2 = new List<TextSpan>();
            int num = 0;
            foreach (ChangeRangeWithText item in list)
            {
                if (item.Range.NewLength > 0)
                {
                    int start = item.Range.Span.Start + num;
                    list2.Add(new TextSpan(start, item.Range.NewLength));
                }
                num += item.Range.NewLength - item.Range.Span.Length;
            }
            return list2;
        }

        private void ComputeChangeRecords()
        {
            while (true)
            {
                if (_newNodes.Count == 0)
                {
                    if (_oldNodes.Count > 0)
                    {
                        RecordDeleteOld(_oldNodes.Count);
                    }
                    break;
                }
                if (_oldNodes.Count == 0)
                {
                    if (_newNodes.Count > 0)
                    {
                        RecordInsertNew(_newNodes.Count);
                    }
                    break;
                }
                DiffAction nextAction = GetNextAction();
                switch (nextAction.Operation)
                {
                    case DiffOp.SkipBoth:
                        RemoveFirst(_oldNodes, nextAction.Count);
                        RemoveFirst(_newNodes, nextAction.Count);
                        break;
                    case DiffOp.ReduceOld:
                        ReplaceFirstWithChildren(_oldNodes);
                        break;
                    case DiffOp.ReduceNew:
                        ReplaceFirstWithChildren(_newNodes);
                        break;
                    case DiffOp.ReduceBoth:
                        ReplaceFirstWithChildren(_oldNodes);
                        ReplaceFirstWithChildren(_newNodes);
                        break;
                    case DiffOp.InsertNew:
                        RecordInsertNew(nextAction.Count);
                        break;
                    case DiffOp.DeleteOld:
                        RecordDeleteOld(nextAction.Count);
                        break;
                    case DiffOp.ReplaceOldWithNew:
                        RecordReplaceOldWithNew(nextAction.Count, nextAction.Count);
                        break;
                }
            }
        }

        private DiffAction GetNextAction()
        {
            bool isToken = _oldNodes.Peek().IsToken;
            bool isToken2 = _newNodes.Peek().IsToken;
            Stack<SyntaxNodeOrToken> newNodes = _newNodes;
            SyntaxNodeOrToken node = _oldNodes.Peek();
            FindBestMatch(newNodes, in node, out var index, out var similarity);
            Stack<SyntaxNodeOrToken> oldNodes = _oldNodes;
            node = _newNodes.Peek();
            FindBestMatch(oldNodes, in node, out var index2, out var similarity2);
            if (index == 0 && index2 == 0)
            {
                node = _oldNodes.Peek();
                SyntaxNodeOrToken node2 = _newNodes.Peek();
                if (AreIdentical(in node, in node2))
                {
                    return new DiffAction(DiffOp.SkipBoth, 1);
                }
                if (!isToken && !isToken2)
                {
                    return new DiffAction(DiffOp.ReduceBoth, 1);
                }
                return new DiffAction(DiffOp.ReplaceOldWithNew, 1);
            }
            if (index >= 0 || index2 >= 0)
            {
                if (index2 < 0 || similarity >= similarity2)
                {
                    if (index > 0)
                    {
                        Stack<SyntaxNodeOrToken> oldNodes2 = _oldNodes;
                        node = _oldNodes.Peek();
                        FindBestMatch(oldNodes2, in node, out var index3, out var similarity3, 1);
                        if (index3 < 1 || similarity3 < similarity)
                        {
                            return new DiffAction(DiffOp.InsertNew, index);
                        }
                    }
                    if (!isToken2)
                    {
                        node = _oldNodes.Peek();
                        SyntaxNodeOrToken node2 = _newNodes.Peek();
                        if (AreSimilar(in node, in node2))
                        {
                            return new DiffAction(DiffOp.ReduceBoth, 1);
                        }
                        return new DiffAction(DiffOp.ReduceNew, 1);
                    }
                    return new DiffAction(DiffOp.ReplaceOldWithNew, 1);
                }
                if (index2 > 0)
                {
                    return new DiffAction(DiffOp.DeleteOld, index2);
                }
                if (!isToken)
                {
                    node = _oldNodes.Peek();
                    SyntaxNodeOrToken node2 = _newNodes.Peek();
                    if (AreSimilar(in node, in node2))
                    {
                        return new DiffAction(DiffOp.ReduceBoth, 1);
                    }
                    return new DiffAction(DiffOp.ReduceOld, 1);
                }
                return new DiffAction(DiffOp.ReplaceOldWithNew, 1);
            }
            if (!isToken && !isToken2)
            {
                node = _oldNodes.Peek();
                SyntaxNodeOrToken node2 = _newNodes.Peek();
                if (GetSimilarity(in node, in node2) >= Math.Max(_oldNodes.Peek().FullSpan.Length, _newNodes.Peek().FullSpan.Length))
                {
                    return new DiffAction(DiffOp.ReduceBoth, 1);
                }
            }
            return new DiffAction(DiffOp.ReplaceOldWithNew, 1);
        }

        private static void ReplaceFirstWithChildren(Stack<SyntaxNodeOrToken> stack)
        {
            SyntaxNodeOrToken syntaxNodeOrToken = stack.Pop();
            int num = 0;
            SyntaxNodeOrToken[] array = new SyntaxNodeOrToken[syntaxNodeOrToken.ChildNodesAndTokens().Count];
            ChildSyntaxList.Enumerator enumerator = syntaxNodeOrToken.ChildNodesAndTokens().GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxNodeOrToken current = enumerator.Current;
                if (current.FullSpan.Length > 0)
                {
                    array[num] = current;
                    num++;
                }
            }
            for (int num2 = num - 1; num2 >= 0; num2--)
            {
                stack.Push(array[num2]);
            }
        }

        private void FindBestMatch(Stack<SyntaxNodeOrToken> stack, in SyntaxNodeOrToken node, out int index, out int similarity, int startIndex = 0)
        {
            index = -1;
            similarity = -1;
            int num = 0;
            foreach (SyntaxNodeOrToken item in stack)
            {
                SyntaxNodeOrToken node2 = item;
                if (num >= 8)
                {
                    break;
                }
                if (num >= startIndex)
                {
                    if (AreIdentical(in node2, in node))
                    {
                        int length = node.FullSpan.Length;
                        if (length > similarity)
                        {
                            index = num;
                            similarity = length;
                            break;
                        }
                    }
                    else if (AreSimilar(in node2, in node))
                    {
                        int similarity2 = GetSimilarity(in node2, in node);
                        if (similarity2 == node.FullSpan.Length && node.IsToken && node2.ToFullString() == node.ToFullString())
                        {
                            index = num;
                            similarity = similarity2;
                            break;
                        }
                        if (similarity2 > similarity)
                        {
                            index = num;
                            similarity = similarity2;
                        }
                    }
                    else
                    {
                        int num2 = 0;
                        ChildSyntaxList.Enumerator enumerator2 = node2.ChildNodesAndTokens().GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            SyntaxNodeOrToken node3 = enumerator2.Current;
                            if (num2 >= 8)
                            {
                                break;
                            }
                            num2++;
                            if (AreIdentical(in node3, in node))
                            {
                                index = num;
                                similarity = node.FullSpan.Length;
                                return;
                            }
                            if (AreSimilar(in node3, in node))
                            {
                                int similarity3 = GetSimilarity(in node3, in node);
                                if (similarity3 > similarity)
                                {
                                    index = num;
                                    similarity = similarity3;
                                }
                            }
                        }
                    }
                }
                num++;
            }
        }

        private int GetSimilarity(in SyntaxNodeOrToken node1, in SyntaxNodeOrToken node2)
        {
            int num = 0;
            _nodeSimilaritySet.Clear();
            _tokenTextSimilaritySet.Clear();
            if (node1.IsToken && node2.IsToken)
            {
                string text = node1.ToString();
                string text2 = node2.ToString();
                if (text == text2)
                {
                    num += text.Length;
                }
                SyntaxTriviaList.Enumerator enumerator = node1.GetLeadingTrivia().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxTrivia current = enumerator.Current;
                    _nodeSimilaritySet.Add(current.UnderlyingNode);
                }
                enumerator = node1.GetTrailingTrivia().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxTrivia current2 = enumerator.Current;
                    _nodeSimilaritySet.Add(current2.UnderlyingNode);
                }
                enumerator = node2.GetLeadingTrivia().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxTrivia current3 = enumerator.Current;
                    if (_nodeSimilaritySet.Contains(current3.UnderlyingNode))
                    {
                        num += current3.FullSpan.Length;
                    }
                }
                enumerator = node2.GetTrailingTrivia().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxTrivia current4 = enumerator.Current;
                    if (_nodeSimilaritySet.Contains(current4.UnderlyingNode))
                    {
                        num += current4.FullSpan.Length;
                    }
                }
            }
            else
            {
                ChildSyntaxList.Enumerator enumerator2 = node1.ChildNodesAndTokens().GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    SyntaxNodeOrToken current5 = enumerator2.Current;
                    _nodeSimilaritySet.Add(current5.UnderlyingNode);
                    if (current5.IsToken)
                    {
                        _tokenTextSimilaritySet.Add(current5.ToString());
                    }
                }
                enumerator2 = node2.ChildNodesAndTokens().GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    SyntaxNodeOrToken current6 = enumerator2.Current;
                    if (_nodeSimilaritySet.Contains(current6.UnderlyingNode))
                    {
                        num += current6.FullSpan.Length;
                    }
                    else if (current6.IsToken)
                    {
                        string text3 = current6.ToString();
                        if (_tokenTextSimilaritySet.Contains(text3))
                        {
                            num += text3.Length;
                        }
                    }
                }
            }
            return num;
        }

        private static bool AreIdentical(in SyntaxNodeOrToken node1, in SyntaxNodeOrToken node2)
        {
            return node1.UnderlyingNode == node2.UnderlyingNode;
        }

        private static bool AreSimilar(in SyntaxNodeOrToken node1, in SyntaxNodeOrToken node2)
        {
            return node1.RawKind == node2.RawKind;
        }

        private void RecordDeleteOld(int oldNodeCount)
        {
            TextSpan span = GetSpan(_oldNodes, 0, oldNodeCount);
            Queue<SyntaxNodeOrToken> oldNodes = CopyFirst(_oldNodes, oldNodeCount);
            RemoveFirst(_oldNodes, oldNodeCount);
            RecordChange(new ChangeRecord(new TextChangeRange(span, 0), oldNodes, null));
        }

        private void RecordReplaceOldWithNew(int oldNodeCount, int newNodeCount)
        {
            if (oldNodeCount == 1 && newNodeCount == 1)
            {
                SyntaxNodeOrToken removedNode = _oldNodes.Pop();
                TextSpan fullSpan = removedNode.FullSpan;
                SyntaxNodeOrToken insertedNode = _newNodes.Pop();
                RecordChange(new TextChangeRange(fullSpan, insertedNode.FullSpan.Length), in removedNode, insertedNode);
            }
            else
            {
                TextSpan span = GetSpan(_oldNodes, 0, oldNodeCount);
                Queue<SyntaxNodeOrToken> oldNodes = CopyFirst(_oldNodes, oldNodeCount);
                RemoveFirst(_oldNodes, oldNodeCount);
                TextSpan span2 = GetSpan(_newNodes, 0, newNodeCount);
                Queue<SyntaxNodeOrToken> newNodes = CopyFirst(_newNodes, newNodeCount);
                RemoveFirst(_newNodes, newNodeCount);
                RecordChange(new ChangeRecord(new TextChangeRange(span, span2.Length), oldNodes, newNodes));
            }
        }

        private void RecordInsertNew(int newNodeCount)
        {
            TextSpan span = GetSpan(_newNodes, 0, newNodeCount);
            Queue<SyntaxNodeOrToken> newNodes = CopyFirst(_newNodes, newNodeCount);
            RemoveFirst(_newNodes, newNodeCount);
            int start = ((_oldNodes.Count > 0) ? _oldNodes.Peek().Position : _oldSpan.End);
            RecordChange(new ChangeRecord(new TextChangeRange(new TextSpan(start, 0), span.Length), null, newNodes));
        }

        private void RecordChange(ChangeRecord change)
        {
            if (_changes.Count > 0)
            {
                ChangeRecord changeRecord = _changes[_changes.Count - 1];
                if (changeRecord.Range.Span.End == change.Range.Span.Start)
                {
                    _changes[_changes.Count - 1] = new ChangeRecord(new TextChangeRange(new TextSpan(changeRecord.Range.Span.Start, changeRecord.Range.Span.Length + change.Range.Span.Length), changeRecord.Range.NewLength + change.Range.NewLength), Combine(changeRecord.OldNodes, change.OldNodes), Combine(changeRecord.NewNodes, change.NewNodes));
                    return;
                }
            }
            _changes.Add(change);
        }

        private void RecordChange(TextChangeRange textChangeRange, in SyntaxNodeOrToken removedNode, SyntaxNodeOrToken insertedNode)
        {
            if (_changes.Count > 0)
            {
                ChangeRecord changeRecord = _changes[_changes.Count - 1];
                if (changeRecord.Range.Span.End == textChangeRange.Span.Start)
                {
                    changeRecord.OldNodes?.Enqueue(removedNode);
                    changeRecord.NewNodes?.Enqueue(insertedNode);
                    _changes[_changes.Count - 1] = new ChangeRecord(new TextChangeRange(new TextSpan(changeRecord.Range.Span.Start, changeRecord.Range.Span.Length + textChangeRange.Span.Length), changeRecord.Range.NewLength + textChangeRange.NewLength), changeRecord.OldNodes ?? CreateQueue(removedNode), changeRecord.NewNodes ?? CreateQueue(insertedNode));
                    return;
                }
            }
            _changes.Add(new ChangeRecord(textChangeRange, CreateQueue(removedNode), CreateQueue(insertedNode)));
            static Queue<SyntaxNodeOrToken> CreateQueue(SyntaxNodeOrToken nodeOrToken)
            {
                Queue<SyntaxNodeOrToken> queue = new Queue<SyntaxNodeOrToken>();
                queue.Enqueue(nodeOrToken);
                return queue;
            }
        }

        private static TextSpan GetSpan(Stack<SyntaxNodeOrToken> stack, int first, int length)
        {
            int start = -1;
            int end = -1;
            int num = 0;
            foreach (SyntaxNodeOrToken item in stack)
            {
                if (num == first)
                {
                    start = item.Position;
                }
                if (num == first + length - 1)
                {
                    end = item.EndPosition;
                    break;
                }
                num++;
            }
            return TextSpan.FromBounds(start, end);
        }

        private static TextSpan GetSpan(Queue<SyntaxNodeOrToken> queue, int first, int length)
        {
            int start = -1;
            int end = -1;
            int num = 0;
            foreach (SyntaxNodeOrToken item in queue)
            {
                if (num == first)
                {
                    start = item.Position;
                }
                if (num == first + length - 1)
                {
                    end = item.EndPosition;
                    break;
                }
                num++;
            }
            return TextSpan.FromBounds(start, end);
        }

        private static Queue<SyntaxNodeOrToken>? Combine(Queue<SyntaxNodeOrToken>? first, Queue<SyntaxNodeOrToken>? next)
        {
            if (first == null || first!.Count == 0)
            {
                return next;
            }
            if (next == null || next!.Count == 0)
            {
                return first;
            }
            foreach (SyntaxNodeOrToken item in next!)
            {
                first!.Enqueue(item);
            }
            return first;
        }

        private static Queue<SyntaxNodeOrToken>? CopyFirst(Stack<SyntaxNodeOrToken> stack, int n)
        {
            if (n == 0)
            {
                return null;
            }
            Queue<SyntaxNodeOrToken> queue = new Queue<SyntaxNodeOrToken>(n);
            int num = n;
            foreach (SyntaxNodeOrToken item in stack)
            {
                if (num != 0)
                {
                    queue.Enqueue(item);
                    num--;
                    continue;
                }
                return queue;
            }
            return queue;
        }

        private static SyntaxNodeOrToken[] ToArray(Stack<SyntaxNodeOrToken> stack, int n)
        {
            SyntaxNodeOrToken[] array = new SyntaxNodeOrToken[n];
            int num = n - 1;
            foreach (SyntaxNodeOrToken item in stack)
            {
                SyntaxNodeOrToken syntaxNodeOrToken = (array[num] = item);
                num--;
                if (num < 0)
                {
                    return array;
                }
            }
            return array;
        }

        private static void RemoveFirst(Stack<SyntaxNodeOrToken> stack, int count)
        {
            for (int i = 0; i < count; i++)
            {
                stack.Pop();
            }
        }

        private List<ChangeRangeWithText> ReduceChanges(List<ChangeRecord> changeRecords)
        {
            List<ChangeRangeWithText> list = new List<ChangeRangeWithText>(changeRecords.Count);
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            foreach (ChangeRecord changeRecord in changeRecords)
            {
                if (changeRecord.Range.Span.Length > 0 && changeRecord.Range.NewLength > 0)
                {
                    TextChangeRange range = changeRecord.Range;
                    CopyText(changeRecord.OldNodes, stringBuilder);
                    CopyText(changeRecord.NewNodes, stringBuilder2);
                    GetCommonEdgeLengths(stringBuilder, stringBuilder2, out var commonLeadingCount, out var commonTrailingCount);
                    if (commonLeadingCount > 0 || commonTrailingCount > 0)
                    {
                        range = new TextChangeRange(new TextSpan(range.Span.Start + commonLeadingCount, range.Span.Length - (commonLeadingCount + commonTrailingCount)), range.NewLength - (commonLeadingCount + commonTrailingCount));
                        if (commonTrailingCount > 0)
                        {
                            stringBuilder2.Remove(stringBuilder2.Length - commonTrailingCount, commonTrailingCount);
                        }
                        if (commonLeadingCount > 0)
                        {
                            stringBuilder2.Remove(0, commonLeadingCount);
                        }
                    }
                    if (range.Span.Length > 0 || range.NewLength > 0)
                    {
                        list.Add(new ChangeRangeWithText(range, _computeNewText ? stringBuilder2.ToString() : null));
                    }
                }
                else
                {
                    list.Add(new ChangeRangeWithText(changeRecord.Range, _computeNewText ? GetText(changeRecord.NewNodes) : null));
                }
            }
            return list;
        }

        private static void GetCommonEdgeLengths(StringBuilder oldText, StringBuilder newText, out int commonLeadingCount, out int commonTrailingCount)
        {
            int num = Math.Min(oldText.Length, newText.Length);
            commonLeadingCount = 0;
            while (commonLeadingCount < num && oldText[commonLeadingCount] == newText[commonLeadingCount])
            {
                commonLeadingCount++;
            }
            num -= commonLeadingCount;
            commonTrailingCount = 0;
            while (commonTrailingCount < num && oldText[oldText.Length - commonTrailingCount - 1] == newText[newText.Length - commonTrailingCount - 1])
            {
                commonTrailingCount++;
            }
        }

        private static string GetText(Queue<SyntaxNodeOrToken>? queue)
        {
            if (queue == null || queue!.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder(GetSpan(queue, 0, queue!.Count).Length);
            CopyText(queue, stringBuilder);
            return stringBuilder.ToString();
        }

        private static void CopyText(Queue<SyntaxNodeOrToken>? queue, StringBuilder builder)
        {
            builder.Length = 0;
            if (queue == null || queue!.Count <= 0)
            {
                return;
            }
            StringWriter stringWriter = new StringWriter(builder);
            foreach (SyntaxNodeOrToken item in queue!)
            {
                item.WriteTo(stringWriter);
            }
            stringWriter.Flush();
        }
    }
}
