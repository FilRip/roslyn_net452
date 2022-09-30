namespace Microsoft.CodeAnalysis.Diagnostics
{
    public abstract class CompilationEvent
    {
        public Compilation Compilation { get; }

        public CompilationEvent(Compilation compilation)
        {
            Compilation = compilation;
        }
    }
}
