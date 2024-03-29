// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract partial class CommonCompiler
    {
        internal sealed class LoggingMetadataFileReferenceResolver : MetadataReferenceResolver, IEquatable<LoggingMetadataFileReferenceResolver>
        {
            private readonly TouchedFileLogger? _logger;
            private readonly RelativePathResolver _pathResolver;
            private readonly Func<string, MetadataReferenceProperties, PortableExecutableReference> _provider;

#nullable restore

            public LoggingMetadataFileReferenceResolver(RelativePathResolver pathResolver,
                Func<string, MetadataReferenceProperties,
                PortableExecutableReference> provider,
#nullable enable
                TouchedFileLogger? logger)
            {
                Debug.Assert(pathResolver != null);
                Debug.Assert(provider != null);

#nullable restore

                _pathResolver = pathResolver;
                _provider = provider;
                _logger = logger;
            }

#nullable enable

            public override ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string? baseFilePath, MetadataReferenceProperties properties)
            {
                string fullPath = _pathResolver.ResolvePath(reference, baseFilePath);

                if (fullPath != null)
                {
                    _logger?.AddRead(fullPath);
                    return ImmutableArray.Create(_provider(fullPath, properties));
                }

                return ImmutableArray<PortableExecutableReference>.Empty;
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            public bool Equals(LoggingMetadataFileReferenceResolver? other)
            {
                throw new NotImplementedException();
            }

            public override bool Equals(object? other) => other is LoggingMetadataFileReferenceResolver otherCast && Equals(otherCast);
        }
    }
}
