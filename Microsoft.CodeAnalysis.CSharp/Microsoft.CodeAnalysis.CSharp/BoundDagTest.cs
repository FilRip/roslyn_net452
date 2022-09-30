using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundDagTest : BoundNode
    {
        public BoundDagTemp Input { get; }

        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj)
        {
            return Equals(obj as BoundDagTest);
        }

        private bool Equals(BoundDagTest? other)
        {
            if (other == null || base.Kind != other!.Kind)
            {
                return false;
            }
            if (this == other)
            {
                return true;
            }
            if (this is BoundDagTypeTest boundDagTypeTest)
            {
                if (other is BoundDagTypeTest boundDagTypeTest2)
                {
                    return boundDagTypeTest.Type.Equals(boundDagTypeTest2.Type, TypeCompareKind.AllIgnoreOptions);
                }
            }
            else if (this is BoundDagNonNullTest boundDagNonNullTest)
            {
                if (other is BoundDagNonNullTest boundDagNonNullTest2)
                {
                    return boundDagNonNullTest.IsExplicitTest == boundDagNonNullTest2.IsExplicitTest;
                }
            }
            else if (this is BoundDagExplicitNullTest)
            {
                if (other is BoundDagExplicitNullTest)
                {
                    return true;
                }
            }
            else if (this is BoundDagValueTest boundDagValueTest)
            {
                if (other is BoundDagValueTest boundDagValueTest2)
                {
                    return boundDagValueTest.Value.Equals(boundDagValueTest2.Value);
                }
            }
            else if (this is BoundDagRelationalTest boundDagRelationalTest && other is BoundDagRelationalTest boundDagRelationalTest2)
            {
                if (boundDagRelationalTest.Relation == boundDagRelationalTest2.Relation)
                {
                    return boundDagRelationalTest.Value.Equals(boundDagRelationalTest2.Value);
                }
                return false;
            }
            throw ExceptionUtilities.UnexpectedValue(this);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(base.Kind.GetHashCode(), Input.GetHashCode());
        }

        protected BoundDagTest(BoundKind kind, SyntaxNode syntax, BoundDagTemp input, bool hasErrors = false)
            : base(kind, syntax, hasErrors)
        {
            Input = input;
        }
    }
}
