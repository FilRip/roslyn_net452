namespace Microsoft.CodeAnalysis.CSharp
{
    public enum DeclarationKind : byte
    {
        Namespace,
        Class,
        Interface,
        Struct,
        Enum,
        Delegate,
        Script,
        Submission,
        ImplicitClass,
        SimpleProgram,
        Record,
        RecordStruct
    }
}
