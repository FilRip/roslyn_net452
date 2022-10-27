using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CommandLine;

#nullable enable

namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal interface IClientConnection : IDisposable
    {
        Task DisconnectTask { get; }

        Task<BuildRequest> ReadBuildRequestAsync(CancellationToken cancellationToken);

        Task WriteBuildResponseAsync(BuildResponse response, CancellationToken cancellationToken);
    }
}
