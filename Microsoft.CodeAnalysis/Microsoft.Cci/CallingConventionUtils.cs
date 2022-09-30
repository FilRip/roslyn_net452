using System.Reflection.Metadata;

using Microsoft.CodeAnalysis;

namespace Microsoft.Cci
{
    public static class CallingConventionUtils
    {
        private const SignatureCallingConvention SignatureCallingConventionMask = (SignatureCallingConvention)15;

        private const SignatureAttributes SignatureAttributesMask = SignatureAttributes.Generic | SignatureAttributes.Instance | SignatureAttributes.ExplicitThis;

        public static CallingConvention FromSignatureConvention(this SignatureCallingConvention convention)
        {
            if (!convention.IsValid())
            {
                throw new UnsupportedSignatureContent();
            }
            return (CallingConvention)(convention & (SignatureCallingConvention)15);
        }

        public static bool IsValid(this SignatureCallingConvention convention)
        {
            if ((int)convention > 5)
            {
                return convention == SignatureCallingConvention.Unmanaged;
            }
            return true;
        }

        public static SignatureCallingConvention ToSignatureConvention(this CallingConvention convention)
        {
            return (SignatureCallingConvention)convention & (SignatureCallingConvention)15;
        }

        public static bool IsCallingConvention(this CallingConvention original, CallingConvention compare)
        {
            return (original & (CallingConvention)15) == compare;
        }

        public static bool HasUnknownCallingConventionAttributeBits(this CallingConvention convention)
        {
            return (convention & (CallingConvention)(-128)) != 0;
        }
    }
}
