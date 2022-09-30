using System;

#nullable enable

namespace Roslyn.Utilities
{
    public static class ExceptionUtilities
    {
        public static Exception Unreachable => new InvalidOperationException("This program location is thought to be unreachable.");

        public static Exception UnexpectedValue(object? o)
        {
            return new InvalidOperationException(string.Format("Unexpected value '{0}' of type '{1}'", o, (o != null) ? o!.GetType().FullName : "<unknown>"));
        }
    }
}
