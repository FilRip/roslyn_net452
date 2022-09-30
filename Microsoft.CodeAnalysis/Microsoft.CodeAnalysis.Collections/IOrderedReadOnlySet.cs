using System.Collections;
using System.Collections.Generic;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Collections
{
    public interface IOrderedReadOnlySet<T> : IReadOnlySet<T>, IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>
    {
    }
}
