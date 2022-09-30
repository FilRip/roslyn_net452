using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundDagEvaluation : BoundDagTest
    {
        private Symbol Symbol
        {
            get
            {
                if (!(this is BoundDagFieldEvaluation boundDagFieldEvaluation))
                {
                    if (!(this is BoundDagPropertyEvaluation boundDagPropertyEvaluation))
                    {
                        if (!(this is BoundDagTypeEvaluation boundDagTypeEvaluation))
                        {
                            if (!(this is BoundDagDeconstructEvaluation boundDagDeconstructEvaluation))
                            {
                                if (this is BoundDagIndexEvaluation boundDagIndexEvaluation)
                                {
                                    return boundDagIndexEvaluation.Property;
                                }
                                throw ExceptionUtilities.UnexpectedValue(base.Kind);
                            }
                            return boundDagDeconstructEvaluation.DeconstructMethod;
                        }
                        return boundDagTypeEvaluation.Type;
                    }
                    return boundDagPropertyEvaluation.Property;
                }
                return boundDagFieldEvaluation.Field.CorrespondingTupleField ?? boundDagFieldEvaluation.Field;
            }
        }

        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj)
        {
            if (obj is BoundDagEvaluation other)
            {
                return Equals(other);
            }
            return false;
        }

        public virtual bool Equals(BoundDagEvaluation other)
        {
            if (this != other)
            {
                if (base.Kind == other.Kind && base.Input.Equals(other.Input))
                {
                    return Symbol.Equals(other.Symbol, TypeCompareKind.AllIgnoreOptions);
                }
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(base.Input.GetHashCode(), Symbol?.GetHashCode() ?? 0);
        }

        protected BoundDagEvaluation(BoundKind kind, SyntaxNode syntax, BoundDagTemp input, bool hasErrors = false)
            : base(kind, syntax, input, hasErrors)
        {
        }
    }
}
