using System;
using System.Collections.Generic;

#nullable enable

namespace Roslyn.Utilities
{
    internal static class CompilerOptionParseUtilities
    {
        public static IList<string> ParseFeatureFromMSBuild(string? features)
        {
            if (RoslynString.IsNullOrEmpty(features))
            {
                return new List<string>(0);
            }
            return features!.Split(new char[3] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static void ParseFeatures(IDictionary<string, string> builder, List<string> values)
        {
            foreach (string value in values)
            {
                string[] array = value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string feature in array)
                {
                    ParseFeatureCore(builder, feature);
                }
            }
        }

        private static void ParseFeatureCore(IDictionary<string, string> builder, string feature)
        {
            int num = feature.IndexOf('=');
            if (num > 0)
            {
                string key = feature.Substring(0, num);
                string text2 = (builder[key] = feature.Substring(num + 1));
            }
            else
            {
                builder[feature] = "true";
            }
        }
    }
}
