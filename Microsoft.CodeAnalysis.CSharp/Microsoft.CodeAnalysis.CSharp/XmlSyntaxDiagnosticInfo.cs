using System;
using System.Globalization;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class XmlSyntaxDiagnosticInfo : SyntaxDiagnosticInfo
    {
        private readonly XmlParseErrorCode _xmlErrorCode;

        static XmlSyntaxDiagnosticInfo()
        {
            ObjectBinder.RegisterTypeReader(typeof(XmlSyntaxDiagnosticInfo), (ObjectReader r) => new XmlSyntaxDiagnosticInfo(r));
        }

        internal XmlSyntaxDiagnosticInfo(XmlParseErrorCode code, params object[] args)
            : this(0, 0, code, args)
        {
        }

        internal XmlSyntaxDiagnosticInfo(int offset, int width, XmlParseErrorCode code, params object[] args)
            : base(offset, width, ErrorCode.WRN_XMLParseError, args)
        {
            _xmlErrorCode = code;
        }

        protected override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteUInt32((uint)_xmlErrorCode);
        }

        private XmlSyntaxDiagnosticInfo(ObjectReader reader)
            : base(reader)
        {
            _xmlErrorCode = (XmlParseErrorCode)reader.ReadUInt32();
        }

        public override string GetMessage(IFormatProvider formatProvider = null)
        {
            CultureInfo cultureInfo = formatProvider as CultureInfo;
            string format = base.MessageProvider.LoadMessage(base.Code, cultureInfo);
            string message = ErrorFacts.GetMessage(_xmlErrorCode, cultureInfo);
            if (base.Arguments == null || base.Arguments.Length == 0)
            {
                return string.Format(formatProvider, format, message);
            }
            return string.Format(formatProvider, string.Format(formatProvider, format, message), GetArgumentsToUse(formatProvider));
        }
    }
}
