using System;
using System.IO;

namespace Roslyn.Utilities
{
    internal static class CommonCompilerFileSystemExtensions
    {
        internal static Stream OpenFileWithNormalizedException(this ICommonCompilerFileSystem fileSystem, string filePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            try
            {
                return fileSystem.OpenFile(filePath, fileMode, fileAccess, fileShare);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (DirectoryNotFoundException ex2)
            {
                throw new FileNotFoundException(ex2.Message, filePath, ex2);
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception ex4)
            {
                throw new IOException(ex4.Message, ex4);
            }
        }
    }
}
