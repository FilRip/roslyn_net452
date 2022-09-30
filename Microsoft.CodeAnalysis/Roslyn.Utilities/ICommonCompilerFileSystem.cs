using System.IO;

namespace Roslyn.Utilities
{
    internal interface ICommonCompilerFileSystem
    {
        bool FileExists(string filePath);

        Stream OpenFile(string filePath, FileMode mode, FileAccess access, FileShare share);

        Stream OpenFileEx(string filePath, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options, out string normalizedFilePath);
    }
}
