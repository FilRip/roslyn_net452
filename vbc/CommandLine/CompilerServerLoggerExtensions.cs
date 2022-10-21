using System;
using System.Text;

#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal static class CompilerServerLoggerExtensions
    {
        internal static void Log(
          this ICompilerServerLogger logger,
          string format,
          params object?[] arguments)
        {
            if (!logger.IsLogging)
                return;
            logger.Log(string.Format(format, arguments));
        }

        internal static void LogError(this ICompilerServerLogger logger, string message)
        {
            if (!logger.IsLogging)
                return;
            logger.Log("Error: " + message);
        }

        internal static void LogError(
          this ICompilerServerLogger logger,
          string format,
          params object?[] arguments)
        {
            if (!logger.IsLogging)
                return;
            logger.Log("Error: " + format, arguments);
        }

        internal static void LogException(
          this ICompilerServerLogger logger,
          Exception exception,
          string reason)
        {
            if (!logger.IsLogging)
                return;
            StringBuilder builder = new();
            builder.Append("Error ");
            AppendException(exception);
            int num = 0;
            Exception innerException = exception.InnerException;
            while (innerException != null)
            {
                builder.Append(string.Format("Inner exception[{0}]  ", num));
                AppendException(innerException);
                innerException = innerException.InnerException;
                ++num;
            }
            logger.Log(builder.ToString());

            void AppendException(Exception exception)
            {
                builder.AppendLine("Error: '" + exception.GetType().Name + "' '" + exception.Message + "' occurred during '" + reason + "'");
                builder.AppendLine("Stack trace:");
                builder.AppendLine(exception.StackTrace);
            }
        }
    }
}
