namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class CompilationUnitCompletedEvent : CompilationEvent
    {
        public SyntaxTree CompilationUnit { get; }

        public CompilationUnitCompletedEvent(Compilation compilation, SyntaxTree compilationUnit)
            : base(compilation)
        {
            CompilationUnit = compilationUnit;
        }

        public override string ToString()
        {
            return "CompilationUnitCompletedEvent(" + CompilationUnit.FilePath + ")";
        }
    }
}
