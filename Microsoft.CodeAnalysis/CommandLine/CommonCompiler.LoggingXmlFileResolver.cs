// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract partial class CommonCompiler
    {
        public sealed class LoggingXmlFileResolver : XmlFileResolver
        {
            private readonly TouchedFileLogger? _logger;

            public LoggingXmlFileResolver(string? baseDirectory, TouchedFileLogger? logger)
                : base(baseDirectory)
            {
                _logger = logger;
            }

            protected override bool FileExists(string? fullPath)
            {
                if (fullPath != null)
                {
                    _logger?.AddRead(fullPath);
                }

                return base.FileExists(fullPath);
            }
        }
    }
}
