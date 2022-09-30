namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class TypeParameterBoundsExtensions
    {
        internal static bool IsSet(this TypeParameterBounds boundsOpt)
        {
            return boundsOpt != TypeParameterBounds.Unset;
        }
    }
}
