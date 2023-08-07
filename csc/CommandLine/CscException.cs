using System;
using System.Runtime.Serialization;

namespace Microsoft.CodeAnalysis.CommandLine
{
    [Serializable()]
    public class CscException : Exception
    {
        public CscException(string message) : base(message) { }

        public CscException(string message, Exception innerException) : base(message, innerException) { }

        protected CscException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
