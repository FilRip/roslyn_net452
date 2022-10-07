using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.CodeGen
{
    public class SequencePointList
    {
        private struct OffsetAndSpan
        {
            public readonly int Offset;

            public readonly TextSpan Span;

            public OffsetAndSpan(int offset, TextSpan span)
            {
                Offset = offset;
                Span = span;
            }
        }

        private readonly SyntaxTree _tree;

        private readonly OffsetAndSpan[] _points;

        private SequencePointList _next;

        private static readonly SequencePointList s_empty = new SequencePointList();

        public bool IsEmpty
        {
            get
            {
                if (_next == null)
                {
                    return _points.Length == 0;
                }
                return false;
            }
        }

        private SequencePointList()
        {
            _points = new OffsetAndSpan[0];
        }

        private SequencePointList(SyntaxTree tree, OffsetAndSpan[] points)
        {
            _tree = tree;
            _points = points;
        }

        public static SequencePointList Create(ArrayBuilder<RawSequencePoint> seqPointBuilder, ILBuilder builder)
        {
            if (seqPointBuilder.Count == 0)
            {
                return s_empty;
            }
            SequencePointList result = null;
            SequencePointList sequencePointList = null;
            int count = seqPointBuilder.Count;
            int num = 0;
            for (int i = 1; i <= count; i++)
            {
                if (i == count || seqPointBuilder[i].SyntaxTree != seqPointBuilder[i - 1].SyntaxTree)
                {
                    SequencePointList sequencePointList2 = new SequencePointList(seqPointBuilder[i - 1].SyntaxTree, GetSubArray(seqPointBuilder, num, i - num, builder));
                    num = i;
                    if (sequencePointList == null)
                    {
                        result = (sequencePointList = sequencePointList2);
                        continue;
                    }
                    sequencePointList._next = sequencePointList2;
                    sequencePointList = sequencePointList2;
                }
            }
            return result;
        }

        private static OffsetAndSpan[] GetSubArray(ArrayBuilder<RawSequencePoint> seqPointBuilder, int start, int length, ILBuilder builder)
        {
            OffsetAndSpan[] array = new OffsetAndSpan[length];
            for (int i = 0; i < array.Length; i++)
            {
                RawSequencePoint rawSequencePoint = seqPointBuilder[i + start];
                int iLOffsetFromMarker = builder.GetILOffsetFromMarker(rawSequencePoint.ILMarker);
                array[i] = new OffsetAndSpan(iLOffsetFromMarker, rawSequencePoint.Span);
            }
            return array;
        }

        public void GetSequencePoints(DebugDocumentProvider documentProvider, ArrayBuilder<SequencePoint> builder)
        {
            bool flag = false;
            string text = null;
            DebugSourceDocument debugSourceDocument = null;
            FileLinePositionSpan? fileLinePositionSpan = FindFirstRealSequencePoint();
            if (!fileLinePositionSpan.HasValue)
            {
                return;
            }
            text = fileLinePositionSpan.Value.Path;
            flag = fileLinePositionSpan.Value.HasMappedPath;
            debugSourceDocument = documentProvider(text, flag ? _tree.FilePath : null);
            for (SequencePointList sequencePointList = this; sequencePointList != null; sequencePointList = sequencePointList._next)
            {
                SyntaxTree tree = sequencePointList._tree;
                OffsetAndSpan[] points = sequencePointList._points;
                for (int i = 0; i < points.Length; i++)
                {
                    OffsetAndSpan offsetAndSpan = points[i];
                    TextSpan span = offsetAndSpan.Span;
                    bool isHiddenPosition = span == RawSequencePoint.HiddenSequencePointSpan;
                    FileLinePositionSpan fileLinePositionSpan2 = default;
                    if (!isHiddenPosition)
                    {
                        fileLinePositionSpan2 = tree.GetMappedLineSpanAndVisibility(span, out isHiddenPosition);
                    }
                    if (isHiddenPosition)
                    {
                        if (text == null)
                        {
                            text = tree.FilePath;
                            debugSourceDocument = documentProvider(text, null);
                        }
                        if (debugSourceDocument != null)
                        {
                            builder.Add(new SequencePoint(debugSourceDocument, offsetAndSpan.Offset, 16707566, 0, 16707566, 0));
                        }
                        continue;
                    }
                    if (text != fileLinePositionSpan2.Path || flag != fileLinePositionSpan2.HasMappedPath)
                    {
                        text = fileLinePositionSpan2.Path;
                        flag = fileLinePositionSpan2.HasMappedPath;
                        debugSourceDocument = documentProvider(text, flag ? tree.FilePath : null);
                    }
                    if (debugSourceDocument != null)
                    {
                        int num = ((fileLinePositionSpan2.StartLinePosition.Line != -1) ? (fileLinePositionSpan2.StartLinePosition.Line + 1) : 0);
                        int num2 = ((fileLinePositionSpan2.EndLinePosition.Line != -1) ? (fileLinePositionSpan2.EndLinePosition.Line + 1) : 0);
                        int num3 = fileLinePositionSpan2.StartLinePosition.Character + 1;
                        int num4 = fileLinePositionSpan2.EndLinePosition.Character + 1;
                        if (num3 > 65534)
                        {
                            num3 = ((num == num2) ? 65533 : 65534);
                        }
                        if (num4 > 65534)
                        {
                            num4 = 65534;
                        }
                        builder.Add(new SequencePoint(debugSourceDocument, offsetAndSpan.Offset, num, (ushort)num3, num2, (ushort)num4));
                    }
                }
            }
        }

        private FileLinePositionSpan? FindFirstRealSequencePoint()
        {
            for (SequencePointList sequencePointList = this; sequencePointList != null; sequencePointList = sequencePointList._next)
            {
                OffsetAndSpan[] points = sequencePointList._points;
                for (int i = 0; i < points.Length; i++)
                {
                    TextSpan span = points[i].Span;
                    bool isHiddenPosition = span == RawSequencePoint.HiddenSequencePointSpan;
                    if (!isHiddenPosition)
                    {
                        FileLinePositionSpan mappedLineSpanAndVisibility = sequencePointList._tree.GetMappedLineSpanAndVisibility(span, out isHiddenPosition);
                        if (!isHiddenPosition)
                        {
                            return mappedLineSpanAndVisibility;
                        }
                    }
                }
            }
            return null;
        }
    }
}
