using System;
using System.Runtime.Serialization;

namespace Microsoft.CodeAnalysis.CommandLine
{
    [Serializable()]
    public class VbCsCompilerException : Exception
    {
        public VbCsCompilerException(string message) : base(message) { }

        public VbCsCompilerException(string message, Exception innerException) : base(message, innerException) { }

        protected VbCsCompilerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
