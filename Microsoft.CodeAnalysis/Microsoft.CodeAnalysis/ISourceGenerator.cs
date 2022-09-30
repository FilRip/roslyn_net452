namespace Microsoft.CodeAnalysis
{
    public interface ISourceGenerator
    {
        void Initialize(GeneratorInitializationContext context);

        void Execute(GeneratorExecutionContext context);
    }
}
