using System.Collections.Generic;

using Microsoft.CodeAnalysis.CommandLine;

#nullable enable

namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal static class BuildProtocolUtil
    {
        internal static RunRequest GetRunRequest(BuildRequest req)
        {
#nullable restore
            string[] commandLineArguments = GetCommandLineArguments(req, out string currentDirectory, out string tempDirectory, out string libDirectory);
            string language = "";
            switch (req.Language)
            {
                case RequestLanguage.CSharpCompile:
                    language = "C#";
                    break;
                case RequestLanguage.VisualBasicCompile:
                    language = "Visual Basic";
                    break;
            }
            return new RunRequest(req.RequestId, language, currentDirectory, tempDirectory, libDirectory, commandLineArguments);
        }

#nullable enable

        internal static string[] GetCommandLineArguments(
            BuildRequest req,
            out string? currentDirectory,
            out string? tempDirectory,
            out string? libDirectory)
        {
            currentDirectory = null;
            libDirectory = null;
            tempDirectory = null;
            List<string> stringList = new();
            foreach (BuildRequest.Argument obj in req.Arguments)
            {
                if (obj.ArgumentId == BuildProtocolConstants.ArgumentId.CurrentDirectory)
                    currentDirectory = obj.Value;
                else if (obj.ArgumentId == BuildProtocolConstants.ArgumentId.TempDirectory)
                    tempDirectory = obj.Value;
                else if (obj.ArgumentId == BuildProtocolConstants.ArgumentId.LibEnvVariable)
                    libDirectory = obj.Value;
                else if (obj.ArgumentId == BuildProtocolConstants.ArgumentId.CommandLineArgument && obj.Value != null)
                {
                    int argumentIndex = obj.ArgumentIndex;
                    while (argumentIndex >= stringList.Count)
                        stringList.Add("");
                    stringList[argumentIndex] = obj.Value;
                }
            }
            return stringList.ToArray();
        }
    }
}
