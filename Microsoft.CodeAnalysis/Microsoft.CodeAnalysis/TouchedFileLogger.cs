using System;
using System.IO;
using System.Threading;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    public class TouchedFileLogger
    {
        private ConcurrentSet<string> _readFiles;

        private ConcurrentSet<string> _writtenFiles;

        public TouchedFileLogger()
        {
            _readFiles = new ConcurrentSet<string>();
            _writtenFiles = new ConcurrentSet<string>();
        }

        public void AddRead(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(path);
            }
            _readFiles.Add(path);
        }

        public void AddWritten(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(path);
            }
            _writtenFiles.Add(path);
        }

        public void AddReadWritten(string path)
        {
            AddRead(path);
            AddWritten(path);
        }

        public void WriteReadPaths(TextWriter s)
        {
            string[] array = new string[_readFiles.Count];
            int num = 0;
            ConcurrentSet<string>.KeyEnumerator enumerator = Interlocked.Exchange(ref _readFiles, null).GetEnumerator();
            while (enumerator.MoveNext())
            {
                string current = enumerator.Current;
                array[num] = current.ToUpperInvariant();
                num++;
            }
            Array.Sort(array);
            string[] array2 = array;
            foreach (string value in array2)
            {
                s.WriteLine(value);
            }
        }

        public void WriteWrittenPaths(TextWriter s)
        {
            string[] array = new string[_writtenFiles.Count];
            int num = 0;
            ConcurrentSet<string>.KeyEnumerator enumerator = Interlocked.Exchange(ref _writtenFiles, null).GetEnumerator();
            while (enumerator.MoveNext())
            {
                string current = enumerator.Current;
                array[num] = current.ToUpperInvariant();
                num++;
            }
            Array.Sort(array);
            string[] array2 = array;
            foreach (string value in array2)
            {
                s.WriteLine(value);
            }
        }
    }
}
