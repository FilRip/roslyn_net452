using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.CodeAnalysis
{
    internal sealed class ShadowCopyAnalyzerAssemblyLoader : DefaultAnalyzerAssemblyLoader
    {
        private readonly string _baseDirectory;

        internal readonly Task DeleteLeftoverDirectoriesTask;

        private readonly Lazy<(string directory, Mutex)> _shadowCopyDirectoryAndMutex;

        private int _assemblyDirectoryId;

        public ShadowCopyAnalyzerAssemblyLoader(string baseDirectory = null)
        {
            if (baseDirectory != null)
            {
                _baseDirectory = baseDirectory;
            }
            else
            {
                _baseDirectory = Path.Combine(Path.GetTempPath(), "CodeAnalysis", "AnalyzerShadowCopies");
            }
            _shadowCopyDirectoryAndMutex = new Lazy<(string, Mutex)>(() => CreateUniqueDirectoryForProcess(), LazyThreadSafetyMode.ExecutionAndPublication);
            DeleteLeftoverDirectoriesTask = Task.Run(DeleteLeftoverDirectories);
        }

        private void DeleteLeftoverDirectories()
        {
            if (!Directory.Exists(_baseDirectory))
            {
                return;
            }
            IEnumerable<string> enumerable;
            try
            {
                enumerable = Directory.EnumerateDirectories(_baseDirectory);
            }
            catch (DirectoryNotFoundException)
            {
                return;
            }
            foreach (string item in enumerable)
            {
                string name = Path.GetFileName(item).ToLowerInvariant();
                Mutex result = null;
                try
                {
                    if (!Mutex.TryOpenExisting(name, out result))
                    {
                        ClearReadOnlyFlagOnFiles(item);
                        Directory.Delete(item, recursive: true);
                    }
                }
                catch
                {
                }
                finally
                {
                    result?.Dispose();
                }
            }
        }

        protected override Assembly LoadImpl(string fullPath)
        {
            string assemblyDirectory = CreateUniqueDirectoryForAssembly();
            string fullPath2 = CopyFileAndResources(fullPath, assemblyDirectory);
            return base.LoadImpl(fullPath2);
        }

        private static string CopyFileAndResources(string fullPath, string assemblyDirectory)
        {
            string fileName = Path.GetFileName(fullPath);
            string text = Path.Combine(assemblyDirectory, fileName);
            CopyFile(fullPath, text);
            string directoryName = Path.GetDirectoryName(fullPath);
            string text2 = Path.GetFileNameWithoutExtension(fileName) + ".resources";
            string text3 = text2 + ".dll";
            foreach (string item in Directory.EnumerateDirectories(directoryName))
            {
                string fileName2 = Path.GetFileName(item);
                string text4 = Path.Combine(item, text3);
                if (File.Exists(text4))
                {
                    string shadowCopyPath = Path.Combine(assemblyDirectory, fileName2, text3);
                    CopyFile(text4, shadowCopyPath);
                }
                text4 = Path.Combine(item, text2, text3);
                if (File.Exists(text4))
                {
                    string shadowCopyPath2 = Path.Combine(assemblyDirectory, fileName2, text2, text3);
                    CopyFile(text4, shadowCopyPath2);
                }
            }
            return text;
        }

        private static void CopyFile(string originalPath, string shadowCopyPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(shadowCopyPath));
            File.Copy(originalPath, shadowCopyPath);
            ClearReadOnlyFlagOnFile(new FileInfo(shadowCopyPath));
        }

        private static void ClearReadOnlyFlagOnFiles(string directoryPath)
        {
            foreach (FileInfo item in new DirectoryInfo(directoryPath).EnumerateFiles("*", SearchOption.AllDirectories))
            {
                ClearReadOnlyFlagOnFile(item);
            }
        }

        private static void ClearReadOnlyFlagOnFile(FileInfo fileInfo)
        {
            try
            {
                if (fileInfo.IsReadOnly)
                {
                    fileInfo.IsReadOnly = false;
                }
            }
            catch
            {
            }
        }

        private string CreateUniqueDirectoryForAssembly()
        {
            string obj = Path.Combine(path2: Interlocked.Increment(ref _assemblyDirectoryId).ToString(), path1: _shadowCopyDirectoryAndMutex.Value.directory);
            Directory.CreateDirectory(obj);
            return obj;
        }

        private (string directory, Mutex mutex) CreateUniqueDirectoryForProcess()
        {
            string text = Guid.NewGuid().ToString("N").ToLowerInvariant();
            string text2 = Path.Combine(_baseDirectory, text);
            Mutex item = new Mutex(initiallyOwned: false, text);
            Directory.CreateDirectory(text2);
            return (text2, item);
        }
    }
}
