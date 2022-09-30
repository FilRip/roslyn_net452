using System.Collections.Generic;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public class SyntaxNodeLocationComparer : IComparer<SyntaxNode>
    {
        private readonly Compilation _compilation;

        public SyntaxNodeLocationComparer(Compilation compilation)
        {
            _compilation = compilation;
        }

        public int Compare(SyntaxNode? x, SyntaxNode? y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            return _compilation.CompareSourceLocations(x!.GetLocation(), y!.GetLocation());
        }
    }
}
