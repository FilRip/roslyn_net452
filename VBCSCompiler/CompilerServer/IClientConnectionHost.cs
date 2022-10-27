using System.Threading.Tasks;

#nullable enable

namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal interface IClientConnectionHost
    {
        bool IsListening { get; }

        void BeginListening();

        Task<IClientConnection> GetNextClientConnectionAsync();

        void EndListening();
    }
}
