// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// This is an abstraction over the file system which allows for us to do more thorough unit testing.
    /// </summary>
    public class StrongNameFileSystem
    {
        internal static readonly StrongNameFileSystem Instance = new();
        internal readonly string? _customTempPath;

        public StrongNameFileSystem(string? customTempPath = null)
        {
            _customTempPath = customTempPath;
        }

        internal virtual FileStream CreateFileStream(string filePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            return new FileStream(filePath, fileMode, fileAccess, fileShare);
        }

        internal virtual byte[] ReadAllBytes(string fullPath)
        {
            Debug.Assert(PathUtilities.IsAbsolute(fullPath));
            return File.ReadAllBytes(fullPath);
        }

        internal virtual bool FileExists(string? fullPath)
        {
            Debug.Assert(fullPath == null || PathUtilities.IsAbsolute(fullPath));
            return File.Exists(fullPath);
        }

        internal virtual string GetTempPath() => _customTempPath ?? Path.GetTempPath();
    }
}
