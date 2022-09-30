using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    [DebuggerDisplay("{Value,nq}")]
    public struct ArrayElement<T>
    {
        public T Value;

        public static implicit operator T(ArrayElement<T> element)
        {
            return element.Value;
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static ArrayElement<T>[]? MakeElementArray(T[]? items)
        {
            if (items == null)
            {
                return null;
            }
            ArrayElement<T>[] array = new ArrayElement<T>[items!.Length];
            for (int i = 0; i < items!.Length; i++)
            {
                array[i].Value = items[i];
            }
            return array;
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static T[]? MakeArray(ArrayElement<T>[]? items)
        {
            if (items == null)
            {
                return null;
            }
            T[] array = new T[items!.Length];
            for (int i = 0; i < items!.Length; i++)
            {
                array[i] = items[i].Value;
            }
            return array;
        }
    }
}
