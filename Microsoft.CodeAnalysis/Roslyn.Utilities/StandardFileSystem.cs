using System.IO;

namespace Roslyn.Utilities
{
    internal sealed class StandardFileSystem : ICommonCompilerFileSystem
    {
        public static StandardFileSystem Instance { get; } = new StandardFileSystem();


        private StandardFileSystem()
        {
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public Stream OpenFile(string filePath, FileMode mode, FileAccess access, FileShare share)
        {
            return new FileStream(filePath, mode, access, share);
        }

        public Stream OpenFileEx(string filePath, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options, out string normalizedFilePath)
        {
            FileStream fileStream = new FileStream(filePath, mode, access, share, bufferSize, options);
            normalizedFilePath = fileStream.Name;
            return fileStream;
        }
    }
}
