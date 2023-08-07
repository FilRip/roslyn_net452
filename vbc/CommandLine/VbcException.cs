using System;
using System.Runtime.Serialization;

namespace Microsoft.CodeAnalysis.CommandLine
{
    [Serializable()]
    public class VbcException : Exception
    {
        public VbcException(string message) : base(message) { }

        public VbcException(string message, Exception innerException) : base(message, innerException) { }

        protected VbcException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
