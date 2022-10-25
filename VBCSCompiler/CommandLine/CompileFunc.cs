using System.IO;

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal delegate int CompileFunc(
    string[] arguments,
    BuildPaths buildPaths,
    TextWriter textWriter,
    IAnalyzerAssemblyLoader analyzerAssemblyLoader);
}
