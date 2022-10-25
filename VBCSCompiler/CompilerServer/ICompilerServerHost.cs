using System.Threading;

using Microsoft.CodeAnalysis.CommandLine;


#nullable enable
namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal interface ICompilerServerHost
    {
        ICompilerServerLogger Logger { get; }

        BuildResponse RunCompilation(
          in RunRequest request,
          CancellationToken cancellationToken);
    }
}
