namespace Microsoft.CodeAnalysis.CSharp
{
    public enum LookupResultKind : byte
    {
        Empty,
        NotATypeOrNamespace,
        NotAnAttributeType,
        WrongArity,
        NotCreatable,
        Inaccessible,
        NotReferencable,
        NotAValue,
        NotAVariable,
        NotInvocable,
        NotLabel,
        StaticInstanceMismatch,
        OverloadResolutionFailure,
        Ambiguous,
        MemberGroup,
        Viable
    }
}
