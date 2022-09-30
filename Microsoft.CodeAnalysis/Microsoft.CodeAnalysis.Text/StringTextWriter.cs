using System.Collections.Immutable;
using System.Text;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Text
{
    internal class StringTextWriter : SourceTextWriter
    {
        private readonly StringBuilder _builder;

        private readonly Encoding? _encoding;

        private readonly SourceHashAlgorithm _checksumAlgorithm;

        public override Encoding Encoding => _encoding;

        public StringTextWriter(Encoding? encoding, SourceHashAlgorithm checksumAlgorithm, int capacity)
        {
            _builder = new StringBuilder(capacity);
            _encoding = encoding;
            _checksumAlgorithm = checksumAlgorithm;
        }

        public override SourceText ToSourceText()
        {
            string source = _builder.ToString();
            Encoding? encoding = _encoding;
            SourceHashAlgorithm checksumAlgorithm = _checksumAlgorithm;
            return new StringText(source, encoding, default(ImmutableArray<byte>), checksumAlgorithm);
        }

        public override void Write(char value)
        {
            _builder.Append(value);
        }

        public override void Write(string? value)
        {
            _builder.Append(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            _builder.Append(buffer, index, count);
        }
    }
}
