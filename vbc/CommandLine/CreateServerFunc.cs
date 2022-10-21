#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal delegate bool CreateServerFunc(
    string clientDir,
    string pipeName,
    ICompilerServerLogger logger);
}
