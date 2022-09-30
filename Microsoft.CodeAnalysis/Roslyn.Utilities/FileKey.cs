using System;

#nullable enable

namespace Roslyn.Utilities
{
    internal struct FileKey : IEquatable<FileKey>
    {
        public readonly string FullPath;

        public readonly DateTime Timestamp;

        public FileKey(string fullPath, DateTime timestamp)
        {
            FullPath = fullPath;
            Timestamp = timestamp;
        }

        public static FileKey Create(string fullPath)
        {
            return new FileKey(fullPath, FileUtilities.GetFileTimeStamp(fullPath));
        }

        public override int GetHashCode()
        {
            int hashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(FullPath);
            DateTime timestamp = Timestamp;
            return Hash.Combine(hashCode, timestamp.GetHashCode());
        }

        public override bool Equals(object? obj)
        {
            if (obj is FileKey)
            {
                return Equals((FileKey)obj);
            }
            return false;
        }

        public override string ToString()
        {
            return $"'{FullPath}'@{Timestamp}";
        }

        public bool Equals(FileKey other)
        {
            if (Timestamp == other.Timestamp)
            {
                return string.Equals(FullPath, other.FullPath, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}
