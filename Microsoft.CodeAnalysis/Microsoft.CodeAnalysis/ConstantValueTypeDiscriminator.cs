namespace Microsoft.CodeAnalysis
{
    public enum ConstantValueTypeDiscriminator : byte
    {
        Nothing = 0,
        Null = 0,
        Bad = 1,
        SByte = 2,
        Byte = 3,
        Int16 = 4,
        UInt16 = 5,
        Int32 = 6,
        UInt32 = 7,
        Int64 = 8,
        UInt64 = 9,
        NInt = 10,
        NUInt = 11,
        Char = 12,
        Boolean = 13,
        Single = 14,
        Double = 15,
        String = 16,
        Decimal = 17,
        DateTime = 18
    }
}
