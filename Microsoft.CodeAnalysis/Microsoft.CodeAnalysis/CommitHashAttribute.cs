using System;

namespace Microsoft.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class CommitHashAttribute : Attribute
    {
        public readonly string Hash;

        public CommitHashAttribute(string hash)
        {
            Hash = hash;
        }
    }
}
