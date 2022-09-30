using System;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public struct AwaitExpressionInfo : IEquatable<AwaitExpressionInfo>
    {
        public IMethodSymbol? GetAwaiterMethod { get; }

        public IPropertySymbol? IsCompletedProperty { get; }

        public IMethodSymbol? GetResultMethod { get; }

        public bool IsDynamic { get; }

        internal AwaitExpressionInfo(IMethodSymbol getAwaiter, IPropertySymbol isCompleted, IMethodSymbol getResult, bool isDynamic)
        {
            GetAwaiterMethod = getAwaiter;
            IsCompletedProperty = isCompleted;
            GetResultMethod = getResult;
            IsDynamic = isDynamic;
        }

        public override bool Equals(object? obj)
        {
            if (obj is AwaitExpressionInfo other)
            {
                return Equals(other);
            }
            return false;
        }

        public bool Equals(AwaitExpressionInfo other)
        {
            if (object.Equals(GetAwaiterMethod, other.GetAwaiterMethod) && object.Equals(IsCompletedProperty, other.IsCompletedProperty) && object.Equals(GetResultMethod, other.GetResultMethod))
            {
                return IsDynamic == other.IsDynamic;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(GetAwaiterMethod, Hash.Combine(IsCompletedProperty, Hash.Combine(GetResultMethod, IsDynamic.GetHashCode())));
        }
    }
}
