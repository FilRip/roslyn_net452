#nullable enable

namespace Roslyn.Utilities
{
    public static class FileNameUtilities
    {
        public const char DirectorySeparatorChar = '\\';

        public const char AltDirectorySeparatorChar = '/';

        public const char VolumeSeparatorChar = ':';

        public static bool IsFileName([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] string? path)
        {
            return IndexOfFileName(path) == 0;
        }

        private static int IndexOfExtension(string? path)
        {
            if (path == null)
            {
                return -1;
            }
            int length = path!.Length;
            int num = length;
            while (--num >= 0)
            {
                char c = path![num];
                if (c == '.')
                {
                    if (num != length - 1)
                    {
                        return num;
                    }
                    return -1;
                }
                if (c == '\\' || c == '/' || c == ':')
                {
                    break;
                }
            }
            return -1;
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("path")]
        public static string? GetExtension(string? path)
        {
            if (path == null)
            {
                return null;
            }
            int num = IndexOfExtension(path);
            if (num < 0)
            {
                return string.Empty;
            }
            return path!.Substring(num);
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("path")]
        private static string? RemoveExtension(string? path)
        {
            if (path == null)
            {
                return null;
            }
            int num = IndexOfExtension(path);
            if (num >= 0)
            {
                return path!.Substring(0, num);
            }
            if (path!.Length > 0 && path![path!.Length - 1] == '.')
            {
                return path!.Substring(0, path!.Length - 1);
            }
            return path;
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("path")]
        public static string? ChangeExtension(string? path, string? extension)
        {
            if (path == null)
            {
                return null;
            }
            string text = RemoveExtension(path);
            if (extension == null || path!.Length == 0)
            {
                return text;
            }
            if (extension!.Length == 0 || extension![0] != '.')
            {
                return text + "." + extension;
            }
            return text + extension;
        }

        public static int IndexOfFileName(string? path)
        {
            if (path == null)
            {
                return -1;
            }
            for (int num = path!.Length - 1; num >= 0; num--)
            {
                char c = path![num];
                if (c == '\\' || c == '/' || c == ':')
                {
                    return num + 1;
                }
            }
            return 0;
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("path")]
        public static string? GetFileName(string? path, bool includeExtension = true)
        {
            int num = IndexOfFileName(path);
            string text = ((num <= 0) ? path : path!.Substring(num));
            if (!includeExtension)
            {
                return RemoveExtension(text);
            }
            return text;
        }
    }
}
