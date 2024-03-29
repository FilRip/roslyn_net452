namespace System.Text
{
    internal class InternalEncoderBestFitFallback : EncoderFallback
    {
        internal BaseCodePageEncoding encoding;

        internal char[] arrayBestFit;

        public override int MaxCharCount => 1;

        internal InternalEncoderBestFitFallback(BaseCodePageEncoding _encoding)
        {
            encoding = _encoding;
        }

        public override EncoderFallbackBuffer CreateFallbackBuffer()
        {
            return new InternalEncoderBestFitFallbackBuffer(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is InternalEncoderBestFitFallback internalEncoderBestFitFallback)
            {
                return encoding.CodePage == internalEncoderBestFitFallback.encoding.CodePage;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return encoding.CodePage.GetHashCode();
        }
    }
}
