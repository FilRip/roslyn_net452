using System.IO;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public class StrongNameFileSystem
    {
        internal static readonly StrongNameFileSystem Instance = new StrongNameFileSystem();

        internal readonly string? _customTempPath;

        internal StrongNameFileSystem(string? customTempPath = null)
        {
            _customTempPath = customTempPath;
        }

        internal virtual FileStream CreateFileStream(string filePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            return new FileStream(filePath, fileMode, fileAccess, fileShare);
        }

        internal virtual byte[] ReadAllBytes(string fullPath)
        {
            return File.ReadAllBytes(fullPath);
        }

        internal virtual bool FileExists(string? fullPath)
        {
            return File.Exists(fullPath);
        }

        internal virtual string GetTempPath()
        {
            return _customTempPath ?? Path.GetTempPath();
        }
    }
}
