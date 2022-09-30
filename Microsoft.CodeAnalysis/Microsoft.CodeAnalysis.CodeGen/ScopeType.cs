namespace Microsoft.CodeAnalysis.CodeGen
{
    public enum ScopeType
    {
        Variable,
        TryCatchFinally,
        Try,
        Catch,
        Filter,
        Finally,
        Fault,
        StateMachineVariable
    }
}
