using System;
using System.Collections.Specialized;
using System.Configuration;

using Microsoft.CodeAnalysis.CommandLine;

#nullable enable

namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal static class VbCsCompiler
    {
        public static int Main(string[] args)
        {
            using CompilerServerLogger logger = new(nameof(VbCsCompiler));
            NameValueCollection appSettings;
            try
            {
                appSettings = ConfigurationManager.AppSettings;
            }
            catch (Exception ex)
            {
                appSettings = new NameValueCollection();
                logger.LogException(ex, "Error loading application settings");
            }
            try
            {
                return new BuildServerController(appSettings, logger).Run(args);
            }
            catch (Exception ex)
            {
                logger.LogException(ex, "Cannot start server");
            }
            return 1;
        }
    }
}
