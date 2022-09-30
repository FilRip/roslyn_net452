using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BinaryOperatorOverloadResolutionResult
    {
        public readonly ArrayBuilder<BinaryOperatorAnalysisResult> Results;

        public static readonly ObjectPool<BinaryOperatorOverloadResolutionResult> Pool = CreatePool();

        public BinaryOperatorAnalysisResult Best
        {
            get
            {
                BinaryOperatorAnalysisResult result = default(BinaryOperatorAnalysisResult);
                ArrayBuilder<BinaryOperatorAnalysisResult>.Enumerator enumerator = Results.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BinaryOperatorAnalysisResult current = enumerator.Current;
                    if (current.IsValid)
                    {
                        if (result.IsValid)
                        {
                            return default(BinaryOperatorAnalysisResult);
                        }
                        result = current;
                    }
                }
                return result;
            }
        }

        private BinaryOperatorOverloadResolutionResult()
        {
            Results = new ArrayBuilder<BinaryOperatorAnalysisResult>(10);
        }

        public bool AnyValid()
        {
            ArrayBuilder<BinaryOperatorAnalysisResult>.Enumerator enumerator = Results.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.IsValid)
                {
                    return true;
                }
            }
            return false;
        }

        public bool SingleValid()
        {
            bool flag = false;
            ArrayBuilder<BinaryOperatorAnalysisResult>.Enumerator enumerator = Results.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.IsValid)
                {
                    if (flag)
                    {
                        return false;
                    }
                    flag = true;
                }
            }
            return flag;
        }

        public static BinaryOperatorOverloadResolutionResult GetInstance()
        {
            return Pool.Allocate();
        }

        public void Free()
        {
            Clear();
            Pool.Free(this);
        }

        public void Clear()
        {
            Results.Clear();
        }

        private static ObjectPool<BinaryOperatorOverloadResolutionResult> CreatePool()
        {
            return new ObjectPool<BinaryOperatorOverloadResolutionResult>(() => new BinaryOperatorOverloadResolutionResult(), 10);
        }
    }
}
