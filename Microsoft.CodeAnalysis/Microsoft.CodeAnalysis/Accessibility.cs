namespace Microsoft.CodeAnalysis
{
    public enum Accessibility
    {
        NotApplicable = 0,
        Private = 1,
        ProtectedAndInternal = 2,
        ProtectedAndFriend = 2,
        Protected = 3,
        Internal = 4,
        Friend = 4,
        ProtectedOrInternal = 5,
        ProtectedOrFriend = 5,
        Public = 6
    }
}
