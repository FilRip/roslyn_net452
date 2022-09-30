using System;
using System.IO;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public class RuleSetInclude
    {
        private readonly string _includePath;

        private readonly ReportDiagnostic _action;

        public string IncludePath => _includePath;

        public ReportDiagnostic Action => _action;

        public RuleSetInclude(string includePath, ReportDiagnostic action)
        {
            _includePath = includePath;
            _action = action;
        }

        public RuleSet? LoadRuleSet(RuleSet parent)
        {
            RuleSet result = null;
            string includePath = _includePath;
            try
            {
                includePath = GetIncludePath(parent);
                if (includePath == null)
                {
                    return null;
                }
                result = RuleSetProcessor.LoadFromFile(includePath);
                return result;
            }
            catch (FileNotFoundException)
            {
                return result;
            }
            catch (Exception ex2)
            {
                throw new InvalidRuleSetException(string.Format(CodeAnalysisResources.InvalidRuleSetInclude, includePath, ex2.Message));
            }
        }

        private string? GetIncludePath(RuleSet parent)
        {
            string text = resolveIncludePath(_includePath, parent?.FilePath);
            if (text == null)
            {
                return null;
            }
            return Path.GetFullPath(text);
            static string? resolveIncludePath(string includePath, string? parentRulesetPath)
            {
                string text2 = resolveIncludePathCore(includePath, parentRulesetPath);
                if (text2 == null && PathUtilities.IsUnixLikePlatform)
                {
                    includePath = includePath.Replace('\\', Path.DirectorySeparatorChar);
                    text2 = resolveIncludePathCore(includePath, parentRulesetPath);
                }
                return text2;
            }
            static string? resolveIncludePathCore(string includePath, string? parentRulesetPath)
            {
                includePath = Environment.ExpandEnvironmentVariables(includePath);
                if (Path.IsPathRooted(includePath))
                {
                    if (File.Exists(includePath))
                    {
                        return includePath;
                    }
                }
                else if (!string.IsNullOrEmpty(parentRulesetPath))
                {
                    includePath = PathUtilities.CombinePathsUnchecked(Path.GetDirectoryName(parentRulesetPath) ?? "", includePath);
                    if (File.Exists(includePath))
                    {
                        return includePath;
                    }
                }
                return null;
            }
        }
    }
}
