using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDagTemp : BoundNode
    {
        public bool IsOriginalInput => Source == null;

        public TypeSymbol Type { get; }

        public BoundDagEvaluation? Source { get; }

        public int Index { get; }

        public static BoundDagTemp ForOriginalInput(SyntaxNode syntax, TypeSymbol type)
        {
            return new BoundDagTemp(syntax, type, null, 0);
        }

        public override bool Equals(object? obj)
        {
            if (obj is BoundDagTemp other)
            {
                return Equals(other);
            }
            return false;
        }

        public bool Equals(BoundDagTemp other)
        {
            if (other != null && Type.Equals(other.Type, TypeCompareKind.AllIgnoreOptions) && object.Equals(Source, other.Source))
            {
                return Index == other.Index;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Type.GetHashCode(), Hash.Combine(Source?.GetHashCode() ?? 0, Index));
        }

        public BoundDagTemp(SyntaxNode syntax, TypeSymbol type, BoundDagEvaluation? source)
            : this(syntax, type, source, 0)
        {
        }

        public static BoundDagTemp ForOriginalInput(BoundExpression expr)
        {
            return new BoundDagTemp(expr.Syntax, expr.Type, null);
        }

        public BoundDagTemp(SyntaxNode syntax, TypeSymbol type, BoundDagEvaluation? source, int index, bool hasErrors = false)
            : base(BoundKind.DagTemp, syntax, hasErrors || source.HasErrors())
        {
            Type = type;
            Source = source;
            Index = index;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDagTemp(this);
        }

        public BoundDagTemp Update(TypeSymbol type, BoundDagEvaluation? source, int index)
        {
            if (!TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything) || source != Source || index != Index)
            {
                BoundDagTemp boundDagTemp = new BoundDagTemp(Syntax, type, source, index, base.HasErrors);
                boundDagTemp.CopyAttributes(this);
                return boundDagTemp;
            }
            return this;
        }
    }
}
