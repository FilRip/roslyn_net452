using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class UnaryOperatorOverloadResolutionResult
    {
        public readonly ArrayBuilder<UnaryOperatorAnalysisResult> Results;

        public static readonly ObjectPool<UnaryOperatorOverloadResolutionResult> Pool = CreatePool();

        public UnaryOperatorAnalysisResult Best
        {
            get
            {
                UnaryOperatorAnalysisResult result = default(UnaryOperatorAnalysisResult);
                ArrayBuilder<UnaryOperatorAnalysisResult>.Enumerator enumerator = Results.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    UnaryOperatorAnalysisResult current = enumerator.Current;
                    if (current.IsValid)
                    {
                        if (result.IsValid)
                        {
                            return default(UnaryOperatorAnalysisResult);
                        }
                        result = current;
                    }
                }
                return result;
            }
        }

        public UnaryOperatorOverloadResolutionResult()
        {
            Results = new ArrayBuilder<UnaryOperatorAnalysisResult>(10);
        }

        public bool AnyValid()
        {
            ArrayBuilder<UnaryOperatorAnalysisResult>.Enumerator enumerator = Results.GetEnumerator();
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
            ArrayBuilder<UnaryOperatorAnalysisResult>.Enumerator enumerator = Results.GetEnumerator();
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

        public static UnaryOperatorOverloadResolutionResult GetInstance()
        {
            return Pool.Allocate();
        }

        public void Free()
        {
            Results.Clear();
            Pool.Free(this);
        }

        private static ObjectPool<UnaryOperatorOverloadResolutionResult> CreatePool()
        {
            return new ObjectPool<UnaryOperatorOverloadResolutionResult>(() => new UnaryOperatorOverloadResolutionResult(), 10);
        }
    }
}
