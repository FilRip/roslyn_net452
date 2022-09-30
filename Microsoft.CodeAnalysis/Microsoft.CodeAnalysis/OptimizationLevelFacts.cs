using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    internal static class OptimizationLevelFacts
    {
        internal static (OptimizationLevel OptimizationLevel, bool DebugPlus) DefaultValues => (OptimizationLevel.Debug, false);

        public static string ToPdbSerializedString(this OptimizationLevel optimization, bool debugPlusMode)
        {
            switch (optimization)
            {
                case OptimizationLevel.Release:
                    if (debugPlusMode)
                    {
                        return "release-debug-plus";
                    }
                    return "release";
                case OptimizationLevel.Debug:
                    if (debugPlusMode)
                    {
                        return "debug-plus";
                    }
                    return "debug";
                default:
                    throw ExceptionUtilities.UnexpectedValue(optimization);
            }
        }

        public static bool TryParsePdbSerializedString(string value, out OptimizationLevel optimizationLevel, out bool debugPlusMode)
        {
            switch (value)
            {
                case "release-debug-plus":
                    optimizationLevel = OptimizationLevel.Release;
                    debugPlusMode = true;
                    return true;
                case "release":
                    optimizationLevel = OptimizationLevel.Release;
                    debugPlusMode = false;
                    return true;
                case "debug-plus":
                    optimizationLevel = OptimizationLevel.Debug;
                    debugPlusMode = true;
                    return true;
                case "debug":
                    optimizationLevel = OptimizationLevel.Debug;
                    debugPlusMode = false;
                    return true;
                default:
                    optimizationLevel = OptimizationLevel.Debug;
                    debugPlusMode = false;
                    return false;
            }
        }
    }
}
