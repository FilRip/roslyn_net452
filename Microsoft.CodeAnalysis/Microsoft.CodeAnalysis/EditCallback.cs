namespace Microsoft.CodeAnalysis
{
    public delegate bool EditCallback<T>(GeneratorEditContext context, T edit) where T : PendingEdit;
}
