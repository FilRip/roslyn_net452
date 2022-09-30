namespace Microsoft.CodeAnalysis.Collections.Internal
{
    internal enum InsertionBehavior : byte
    {
        None,
        OverwriteExisting,
        ThrowOnExisting
    }
}
