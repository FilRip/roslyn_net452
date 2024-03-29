// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract partial class CommonCompiler
    {
        public sealed class LoggingSourceFileResolver : SourceFileResolver
        {
            private readonly TouchedFileLogger? _logger;

            public LoggingSourceFileResolver(
                ImmutableArray<string> searchPaths,
                string? baseDirectory,
                ImmutableArray<KeyValuePair<string, string>> pathMap,
                TouchedFileLogger? logger)
                : base(searchPaths, baseDirectory, pathMap)
            {
                _logger = logger;
            }

            protected override bool FileExists(string? resolvedPath)
            {
                if (resolvedPath != null)
                {
                    _logger?.AddRead(resolvedPath);
                }

                return base.FileExists(resolvedPath);
            }

            public LoggingSourceFileResolver WithBaseDirectory(string value) =>
                (BaseDirectory == value) ? this : new LoggingSourceFileResolver(SearchPaths, value, PathMap, _logger);

            public LoggingSourceFileResolver WithSearchPaths(ImmutableArray<string> value) =>
                (SearchPaths == value) ? this : new LoggingSourceFileResolver(value, BaseDirectory, PathMap, _logger);
        }
    }
}
