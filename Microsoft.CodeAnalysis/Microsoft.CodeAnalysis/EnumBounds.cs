namespace Microsoft.CodeAnalysis
{
    public static class EnumBounds
    {
        public static bool IsValid(this OptimizationLevel value)
        {
            if (value >= OptimizationLevel.Debug)
            {
                return value <= OptimizationLevel.Release;
            }
            return false;
        }

        public static bool IsValid(this Platform value)
        {
            if (value >= Platform.AnyCpu)
            {
                return value <= Platform.Arm64;
            }
            return false;
        }

        public static bool Requires64Bit(this Platform value)
        {
            if (value != Platform.X64 && value != Platform.Itanium)
            {
                return value == Platform.Arm64;
            }
            return true;
        }

        public static bool Requires32Bit(this Platform value)
        {
            return value == Platform.X86;
        }

        public static bool IsValid(this MetadataImportOptions value)
        {
            if ((int)value >= 0)
            {
                return (int)value <= 2;
            }
            return false;
        }

        public static bool IsValid(this MetadataImageKind kind)
        {
            if ((int)kind >= 0)
            {
                return (int)kind <= 1;
            }
            return false;
        }

        public static bool IsValid(this OutputKind value)
        {
            if (value >= OutputKind.ConsoleApplication)
            {
                return value <= OutputKind.WindowsRuntimeApplication;
            }
            return false;
        }

        public static string GetDefaultExtension(this OutputKind kind)
        {
            switch (kind)
            {
                case OutputKind.ConsoleApplication:
                case OutputKind.WindowsApplication:
                case OutputKind.WindowsRuntimeApplication:
                    return ".exe";
                case OutputKind.DynamicallyLinkedLibrary:
                    return ".dll";
                case OutputKind.NetModule:
                    return ".netmodule";
                case OutputKind.WindowsRuntimeMetadata:
                    return ".winmdobj";
                default:
                    return ".dll";
            }
        }

        public static bool IsApplication(this OutputKind kind)
        {
            switch (kind)
            {
                case OutputKind.ConsoleApplication:
                case OutputKind.WindowsApplication:
                case OutputKind.WindowsRuntimeApplication:
                    return true;
                case OutputKind.DynamicallyLinkedLibrary:
                case OutputKind.NetModule:
                case OutputKind.WindowsRuntimeMetadata:
                    return false;
                default:
                    return false;
            }
        }

        public static bool IsNetModule(this OutputKind kind)
        {
            return kind == OutputKind.NetModule;
        }

        public static bool IsWindowsRuntime(this OutputKind kind)
        {
            return kind == OutputKind.WindowsRuntimeMetadata;
        }

        public static bool IsValid(this SymbolDisplayPartKind value)
        {
            if (value < SymbolDisplayPartKind.AliasName || value > SymbolDisplayPartKind.RecordStructName)
            {
                if (value >= (SymbolDisplayPartKind)33)
                {
                    return value <= (SymbolDisplayPartKind)34;
                }
                return false;
            }
            return true;
        }
    }
}
