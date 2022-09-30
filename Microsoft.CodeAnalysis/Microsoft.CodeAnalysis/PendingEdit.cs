namespace Microsoft.CodeAnalysis
{
    public abstract class PendingEdit
    {
        internal abstract GeneratorDriverState Commit(GeneratorDriverState state);

        internal abstract bool AcceptedBy(GeneratorInfo info);

        internal abstract bool TryApply(GeneratorInfo info, GeneratorEditContext context);
    }
}
