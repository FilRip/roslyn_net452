using System;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public struct CommonConversion
    {
        [Flags()]
        private enum ConversionKind
        {
            None = 0,
            Exists = 1,
            IsIdentity = 2,
            IsNumeric = 4,
            IsReference = 8,
            IsImplicit = 0x10,
            IsNullable = 0x20
        }

        private readonly ConversionKind _conversionKind;

        public bool Exists => (_conversionKind & ConversionKind.Exists) == ConversionKind.Exists;

        public bool IsIdentity => (_conversionKind & ConversionKind.IsIdentity) == ConversionKind.IsIdentity;

        public bool IsNullable => (_conversionKind & ConversionKind.IsNullable) == ConversionKind.IsNullable;

        public bool IsNumeric => (_conversionKind & ConversionKind.IsNumeric) == ConversionKind.IsNumeric;

        public bool IsReference => (_conversionKind & ConversionKind.IsReference) == ConversionKind.IsReference;

        public bool IsImplicit => (_conversionKind & ConversionKind.IsImplicit) == ConversionKind.IsImplicit;

        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "MethodSymbol")]
        public bool IsUserDefined
        {
            [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "MethodSymbol")]
            get
            {
                return MethodSymbol != null;
            }
        }

        public IMethodSymbol? MethodSymbol { get; }

        public CommonConversion(bool exists, bool isIdentity, bool isNumeric, bool isReference, bool isImplicit, bool isNullable, IMethodSymbol? methodSymbol)
        {
            _conversionKind = (exists ? ConversionKind.Exists : ConversionKind.None) | (isIdentity ? ConversionKind.IsIdentity : ConversionKind.None) | (isNumeric ? ConversionKind.IsNumeric : ConversionKind.None) | (isReference ? ConversionKind.IsReference : ConversionKind.None) | (isImplicit ? ConversionKind.IsImplicit : ConversionKind.None) | (isNullable ? ConversionKind.IsNullable : ConversionKind.None);
            MethodSymbol = methodSymbol;
        }
    }
}
