using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

using Microsoft.CodeAnalysis.CommandLine;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal static class AnalyzerConsistencyChecker
    {
        public static bool Check(
            string baseDirectory,
            IEnumerable<CommandLineAnalyzerReference> analyzerReferences,
            IAnalyzerAssemblyLoader loader,
            ICompilerServerLogger? logger = null)
        {
            return AnalyzerConsistencyChecker.Check(baseDirectory, analyzerReferences, loader, logger, out List<string> _);
        }

        public static bool Check(
            string baseDirectory,
            IEnumerable<CommandLineAnalyzerReference> analyzerReferences,
            IAnalyzerAssemblyLoader loader,
            ICompilerServerLogger? logger,
            [NotNullWhen(false)] out List<string>? errorMessages)
        {
            try
            {
                logger?.Log("Begin Analyzer Consistency Check");
                return AnalyzerConsistencyChecker.CheckCore(baseDirectory, analyzerReferences, loader, /*logger, */out errorMessages);
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.LogException(ex, "Analyzer Consistency Check");
                errorMessages = new List<string>()
                {
                    ex.Message
                };
                return false;
            }
            finally
            {
                logger?.Log("End Analyzer Consistency Check");
            }
        }

        private static bool CheckCore(
            string baseDirectory,
            IEnumerable<CommandLineAnalyzerReference> analyzerReferences,
            IAnalyzerAssemblyLoader loader,
            //ICompilerServerLogger? logger,
            [NotNullWhen(false)] out List<string>? errorMessages)
        {
            errorMessages = null;
            List<string> stringList = new();
            foreach (CommandLineAnalyzerReference analyzerReference in analyzerReferences)
            {
#nullable restore
                string path = FileUtilities.ResolveRelativePath(analyzerReference.FilePath, null, baseDirectory, SpecializedCollections.EmptyEnumerable<string>(), new Func<string, bool>(File.Exists));
                if (path != null)
                {
                    string str = FileUtilities.TryNormalizeAbsolutePath(path);
                    if (str != null)
                        stringList.Add(str);
                }
#nullable enable
            }
            foreach (string fullPath in stringList)
                loader.AddDependencyLocation(fullPath);
            List<Assembly> assemblyList = new();
            foreach (string fullPath in stringList)
                assemblyList.Add(loader.LoadFromPath(fullPath));
            for (int index = 0; index < stringList.Count; ++index)
            {
                string filePath = stringList[index];
                Assembly assembly = assemblyList[index];
                Guid guid = AssemblyUtilities.ReadMvid(filePath);
                Guid moduleVersionId = assembly.ManifestModule.ModuleVersionId;
                if (guid != moduleVersionId)
                {
                    string str = string.Format("analyzer assembly '{0}' has MVID '{1}' but loaded assembly '{2}' has MVID '{3}'", filePath, guid, assembly.FullName, moduleVersionId);
                    if (errorMessages == null)
                        errorMessages = new List<string>();
                    errorMessages.Add(str);
                }
            }
            return errorMessages == null;
        }
    }
}
