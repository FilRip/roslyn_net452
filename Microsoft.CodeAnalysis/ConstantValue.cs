// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public enum ConstantValueTypeDiscriminator : byte
    {
        Nothing,
        Null = Nothing,
        Bad,
        SByte,
        Byte,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Int64,
        UInt64,
        NInt,
        NUInt,
        Char,
        Boolean,
        Single,
        Double,
        String,
        Decimal,
        DateTime,
    }

    public abstract partial class ConstantValue : IEquatable<ConstantValue?>
    {
        public abstract ConstantValueTypeDiscriminator Discriminator { get; }
        public abstract SpecialType SpecialType { get; }

        public virtual string? StringValue { get { throw new InvalidOperationException(); } }
        public virtual Rope? RopeValue { get { throw new InvalidOperationException(); } }

        public virtual bool BooleanValue { get { throw new InvalidOperationException(); } }

        public virtual sbyte SByteValue { get { throw new InvalidOperationException(); } }
        public virtual byte ByteValue { get { throw new InvalidOperationException(); } }

        // If we can get SByteValue, we can automatically get Int16Value, Int32Value, Int64Value. 
        // This is needed when constant values are reinterpreted during constant folding - 
        // for example a Byte value may be read via UIntValue accessor when folding Byte + Uint
        //
        // I have decided that default implementation of Int16Value in terms of SByteValue is appropriate here.
        // Same pattern is used for providing default implementation of Int32Value in terms of Int16Value and so on.
        //
        // An alternative solution would be to override Int16Value, Int32Value, Int64Value whenever I override SByteValue
        // and so on for Int16Value, Int32Value. That could work mildly faster but would result in a lot more code.

        public virtual short Int16Value { get { return SByteValue; } }
        public virtual ushort UInt16Value { get { return ByteValue; } }

        public virtual int Int32Value { get { return Int16Value; } }
        public virtual uint UInt32Value { get { return UInt16Value; } }

        public virtual long Int64Value { get { return Int32Value; } }
        public virtual ulong UInt64Value { get { return UInt32Value; } }

        public virtual char CharValue { get { throw new InvalidOperationException(); } }
        public virtual decimal DecimalValue { get { throw new InvalidOperationException(); } }
        public virtual DateTime DateTimeValue { get { throw new InvalidOperationException(); } }

        public virtual double DoubleValue { get { throw new InvalidOperationException(); } }
        public virtual float SingleValue { get { throw new InvalidOperationException(); } }

        // returns true if value is in its default (zero-inited) form.
        public virtual bool IsDefaultValue { get { return false; } }

        // NOTE: We do not have IsNumericZero. 
        //       The reason is that integral zeroes are same as default values
        //       and singles, floats and decimals have multiple zero values. 
        //       It appears that in all cases so far we considered isDefaultValue, and not about value being 
        //       arithmetic zero (especially when definition is ambiguous).

        public const ConstantValue NotAvailable = null;

        public static ConstantValue Bad { get { return ConstantValueBad.Instance; } }
        public static ConstantValue Null { get { return ConstantValueNull.Instance; } }
        public static ConstantValue Nothing { get { return Null; } }
        // Null, Nothing and Unset are all ConstantValueNull. Null and Nothing are equivalent and represent the null and
        // nothing constants in C# and VB.  Unset indicates an uninitialized ConstantValue.
        public static ConstantValue Unset { get { return ConstantValueNull.Uninitialized; } }

        public static ConstantValue True { get { return ConstantValueOne.Boolean; } }
        public static ConstantValue False { get { return ConstantValueDefault.Boolean; } }

        public static ConstantValue Create(string? value)
        {
            if (value == null)
            {
                return Null;
            }

            return new ConstantValueString(value);
        }

        public static ConstantValue CreateFromRope(Rope value)
        {
            RoslynDebug.Assert(value != null);
            return new ConstantValueString(value);
        }

        public static ConstantValue Create(char value)
        {
            if (value == default(char))
            {
                return ConstantValueDefault.Char;
            }

            return new ConstantValueI16(value);
        }

        public static ConstantValue Create(sbyte value)
        {
            if (value == 0)
            {
                return ConstantValueDefault.SByte;
            }
            else if (value == 1)
            {
                return ConstantValueOne.SByte;
            }

            return new ConstantValueI8(value);
        }

        public static ConstantValue Create(byte value)
        {
            if (value == 0)
            {
                return ConstantValueDefault.Byte;
            }
            else if (value == 1)
            {
                return ConstantValueOne.Byte;
            }

            return new ConstantValueI8(value);
        }

        public static ConstantValue Create(Int16 value)
        {
            if (value == 0)
            {
                return ConstantValueDefault.Int16;
            }
            else if (value == 1)
            {
                return ConstantValueOne.Int16;
            }

            return new ConstantValueI16(value);
        }

        public static ConstantValue Create(UInt16 value)
        {
            if (value == 0)
            {
                return ConstantValueDefault.UInt16;
            }
            else if (value == 1)
            {
                return ConstantValueOne.UInt16;
            }

            return new ConstantValueI16(value);
        }

        public static ConstantValue Create(Int32 value)
        {
            if (value == 0)
            {
                return ConstantValueDefault.Int32;
            }
            else if (value == 1)
            {
                return ConstantValueOne.Int32;
            }

            return new ConstantValueI32(value);
        }

        public static ConstantValue Create(UInt32 value)
        {
            if (value == 0)
            {
                return ConstantValueDefault.UInt32;
            }
            else if (value == 1)
            {
                return ConstantValueOne.UInt32;
            }

            return new ConstantValueI32(value);
        }

        public static ConstantValue Create(Int64 value)
        {
            if (value == 0)
            {
                return ConstantValueDefault.Int64;
            }
            else if (value == 1)
            {
                return ConstantValueOne.Int64;
            }

            return new ConstantValueI64(value);
        }

        public static ConstantValue Create(UInt64 value)
        {
            if (value == 0)
            {
                return ConstantValueDefault.UInt64;
            }
            else if (value == 1)
            {
                return ConstantValueOne.UInt64;
            }

            return new ConstantValueI64(value);
        }

        public static ConstantValue CreateNativeInt(Int32 value)
        {
            if (value == 0)
            {
                return ConstantValueDefault.NInt;
            }
            else if (value == 1)
            {
                return ConstantValueOne.NInt;
            }

            return new ConstantValueNativeInt(value);
        }

        public static ConstantValue CreateNativeUInt(UInt32 value)
        {
            if (value == 0)
            {
                return ConstantValueDefault.NUInt;
            }
            else if (value == 1)
            {
                return ConstantValueOne.NUInt;
            }

            return new ConstantValueNativeInt(value);
        }

        public static ConstantValue Create(bool value)
        {
            if (value)
            {
                return ConstantValueOne.Boolean;
            }
            else
            {
                return ConstantValueDefault.Boolean;
            }
        }

        public static ConstantValue Create(float value)
        {
            if (BitConverter.DoubleToInt64Bits(value) == 0)
            {
                return ConstantValueDefault.Single;
            }
            else if (value == 1)
            {
                return ConstantValueOne.Single;
            }

            return new ConstantValueSingle(value);
        }

        public static ConstantValue CreateSingle(double value)
        {
            if (BitConverter.DoubleToInt64Bits(value) == 0)
            {
                return ConstantValueDefault.Single;
            }
            else if (value == 1)
            {
                return ConstantValueOne.Single;
            }

            return new ConstantValueSingle(value);
        }

        public static ConstantValue Create(double value)
        {
            if (BitConverter.DoubleToInt64Bits(value) == 0)
            {
                return ConstantValueDefault.Double;
            }
            else if (value == 1)
            {
                return ConstantValueOne.Double;
            }

            return new ConstantValueDouble(value);
        }

        public static ConstantValue Create(decimal value)
        {
            // The significant bits should be preserved even for Zero or One.
            // The fourth element of the returned array contains the scale factor and sign.
            int[] decimalBits = System.Decimal.GetBits(value);
            if (decimalBits[3] == 0)
            {
                if (value == 0)
                {
                    return ConstantValueDefault.Decimal;
                }
                else if (value == 1)
                {
                    return ConstantValueOne.Decimal;
                }
            }

            return new ConstantValueDecimal(value);
        }

        public static ConstantValue Create(DateTime value)
        {
            if (value == default)
            {
                return ConstantValueDefault.DateTime;
            }

            return new ConstantValueDateTime(value);
        }

        public static ConstantValue Create(object value, SpecialType st)
        {
            var discriminator = GetDiscriminator(st);
            Debug.Assert(discriminator != ConstantValueTypeDiscriminator.Bad);
            return Create(value, discriminator);
        }

        public static ConstantValue CreateSizeOf(SpecialType st)
        {
            int size = st.SizeInBytes();
            return (size == 0) ? ConstantValue.NotAvailable : ConstantValue.Create(size);
        }

        public static ConstantValue Create(object value, ConstantValueTypeDiscriminator discriminator)
        {
            Debug.Assert(BitConverter.IsLittleEndian);

            return discriminator switch
            {
                ConstantValueTypeDiscriminator.Null => Null,
                ConstantValueTypeDiscriminator.SByte => Create((sbyte)value),
                ConstantValueTypeDiscriminator.Byte => Create((byte)value),
                ConstantValueTypeDiscriminator.Int16 => Create((short)value),
                ConstantValueTypeDiscriminator.UInt16 => Create((ushort)value),
                ConstantValueTypeDiscriminator.Int32 => Create((int)value),
                ConstantValueTypeDiscriminator.UInt32 => Create((uint)value),
                ConstantValueTypeDiscriminator.Int64 => Create((long)value),
                ConstantValueTypeDiscriminator.UInt64 => Create((ulong)value),
                ConstantValueTypeDiscriminator.NInt => CreateNativeInt((int)value),
                ConstantValueTypeDiscriminator.NUInt => CreateNativeUInt((uint)value),
                ConstantValueTypeDiscriminator.Char => Create((char)value),
                ConstantValueTypeDiscriminator.Boolean => Create((bool)value),
                ConstantValueTypeDiscriminator.Single => value is double ?
CreateSingle((double)value) :
Create((float)value),// values for singles may actually have double precision
                ConstantValueTypeDiscriminator.Double => Create((double)value),
                ConstantValueTypeDiscriminator.Decimal => Create((decimal)value),
                ConstantValueTypeDiscriminator.DateTime => Create((DateTime)value),
                ConstantValueTypeDiscriminator.String => Create((string)value),
                _ => throw new InvalidOperationException(),//Not using ExceptionUtilities.UnexpectedValue() because this failure path is tested.
            };
        }

        public static ConstantValue Default(SpecialType st)
        {
            var discriminator = GetDiscriminator(st);
            Debug.Assert(discriminator != ConstantValueTypeDiscriminator.Bad);
            return Default(discriminator);
        }

        public static ConstantValue Default(ConstantValueTypeDiscriminator discriminator)
        {
            return discriminator switch
            {
                ConstantValueTypeDiscriminator.Bad => Bad,
                ConstantValueTypeDiscriminator.SByte => ConstantValueDefault.SByte,
                ConstantValueTypeDiscriminator.Byte => ConstantValueDefault.Byte,
                ConstantValueTypeDiscriminator.Int16 => ConstantValueDefault.Int16,
                ConstantValueTypeDiscriminator.UInt16 => ConstantValueDefault.UInt16,
                ConstantValueTypeDiscriminator.Int32 => ConstantValueDefault.Int32,
                ConstantValueTypeDiscriminator.UInt32 => ConstantValueDefault.UInt32,
                ConstantValueTypeDiscriminator.Int64 => ConstantValueDefault.Int64,
                ConstantValueTypeDiscriminator.UInt64 => ConstantValueDefault.UInt64,
                ConstantValueTypeDiscriminator.NInt => ConstantValueDefault.NInt,
                ConstantValueTypeDiscriminator.NUInt => ConstantValueDefault.NUInt,
                ConstantValueTypeDiscriminator.Char => ConstantValueDefault.Char,
                ConstantValueTypeDiscriminator.Boolean => ConstantValueDefault.Boolean,
                ConstantValueTypeDiscriminator.Single => ConstantValueDefault.Single,
                ConstantValueTypeDiscriminator.Double => ConstantValueDefault.Double,
                ConstantValueTypeDiscriminator.Decimal => ConstantValueDefault.Decimal,
                ConstantValueTypeDiscriminator.DateTime => ConstantValueDefault.DateTime,
                ConstantValueTypeDiscriminator.Null or ConstantValueTypeDiscriminator.String => Null,
                _ => throw ExceptionUtilities.UnexpectedValue(discriminator),
            };
        }

        public static ConstantValueTypeDiscriminator GetDiscriminator(SpecialType st)
        {
            return st switch
            {
                SpecialType.System_SByte => ConstantValueTypeDiscriminator.SByte,
                SpecialType.System_Byte => ConstantValueTypeDiscriminator.Byte,
                SpecialType.System_Int16 => ConstantValueTypeDiscriminator.Int16,
                SpecialType.System_UInt16 => ConstantValueTypeDiscriminator.UInt16,
                SpecialType.System_Int32 => ConstantValueTypeDiscriminator.Int32,
                SpecialType.System_UInt32 => ConstantValueTypeDiscriminator.UInt32,
                SpecialType.System_Int64 => ConstantValueTypeDiscriminator.Int64,
                SpecialType.System_UInt64 => ConstantValueTypeDiscriminator.UInt64,
                SpecialType.System_IntPtr => ConstantValueTypeDiscriminator.NInt,
                SpecialType.System_UIntPtr => ConstantValueTypeDiscriminator.NUInt,
                SpecialType.System_Char => ConstantValueTypeDiscriminator.Char,
                SpecialType.System_Boolean => ConstantValueTypeDiscriminator.Boolean,
                SpecialType.System_Single => ConstantValueTypeDiscriminator.Single,
                SpecialType.System_Double => ConstantValueTypeDiscriminator.Double,
                SpecialType.System_Decimal => ConstantValueTypeDiscriminator.Decimal,
                SpecialType.System_DateTime => ConstantValueTypeDiscriminator.DateTime,
                SpecialType.System_String => ConstantValueTypeDiscriminator.String,
                _ => ConstantValueTypeDiscriminator.Bad,
            };
        }

        private static SpecialType GetSpecialType(ConstantValueTypeDiscriminator discriminator)
        {
            return discriminator switch
            {
                ConstantValueTypeDiscriminator.SByte => SpecialType.System_SByte,
                ConstantValueTypeDiscriminator.Byte => SpecialType.System_Byte,
                ConstantValueTypeDiscriminator.Int16 => SpecialType.System_Int16,
                ConstantValueTypeDiscriminator.UInt16 => SpecialType.System_UInt16,
                ConstantValueTypeDiscriminator.Int32 => SpecialType.System_Int32,
                ConstantValueTypeDiscriminator.UInt32 => SpecialType.System_UInt32,
                ConstantValueTypeDiscriminator.Int64 => SpecialType.System_Int64,
                ConstantValueTypeDiscriminator.UInt64 => SpecialType.System_UInt64,
                ConstantValueTypeDiscriminator.NInt => SpecialType.System_IntPtr,
                ConstantValueTypeDiscriminator.NUInt => SpecialType.System_UIntPtr,
                ConstantValueTypeDiscriminator.Char => SpecialType.System_Char,
                ConstantValueTypeDiscriminator.Boolean => SpecialType.System_Boolean,
                ConstantValueTypeDiscriminator.Single => SpecialType.System_Single,
                ConstantValueTypeDiscriminator.Double => SpecialType.System_Double,
                ConstantValueTypeDiscriminator.Decimal => SpecialType.System_Decimal,
                ConstantValueTypeDiscriminator.DateTime => SpecialType.System_DateTime,
                ConstantValueTypeDiscriminator.String => SpecialType.System_String,
                _ => SpecialType.None,
            };
        }

        public object? Value
        {
            get
            {
                return this.Discriminator switch
                {
                    ConstantValueTypeDiscriminator.Bad => null,
                    ConstantValueTypeDiscriminator.Null => null,
                    ConstantValueTypeDiscriminator.SByte => Boxes.Box(SByteValue),
                    ConstantValueTypeDiscriminator.Byte => Boxes.Box(ByteValue),
                    ConstantValueTypeDiscriminator.Int16 => Boxes.Box(Int16Value),
                    ConstantValueTypeDiscriminator.UInt16 => Boxes.Box(UInt16Value),
                    ConstantValueTypeDiscriminator.Int32 => Boxes.Box(Int32Value),
                    ConstantValueTypeDiscriminator.UInt32 => Boxes.Box(UInt32Value),
                    ConstantValueTypeDiscriminator.Int64 => Boxes.Box(Int64Value),
                    ConstantValueTypeDiscriminator.UInt64 => Boxes.Box(UInt64Value),
                    ConstantValueTypeDiscriminator.NInt => Boxes.Box(Int32Value),
                    ConstantValueTypeDiscriminator.NUInt => Boxes.Box(UInt32Value),
                    ConstantValueTypeDiscriminator.Char => Boxes.Box(CharValue),
                    ConstantValueTypeDiscriminator.Boolean => Boxes.Box(BooleanValue),
                    ConstantValueTypeDiscriminator.Single => Boxes.Box(SingleValue),
                    ConstantValueTypeDiscriminator.Double => Boxes.Box(DoubleValue),
                    ConstantValueTypeDiscriminator.Decimal => Boxes.Box(DecimalValue),
                    ConstantValueTypeDiscriminator.DateTime => DateTimeValue,
                    ConstantValueTypeDiscriminator.String => StringValue,
                    _ => throw ExceptionUtilities.UnexpectedValue(this.Discriminator),
                };
            }
        }

        public static bool IsIntegralType(ConstantValueTypeDiscriminator discriminator)
        {
            return discriminator switch
            {
                ConstantValueTypeDiscriminator.SByte or ConstantValueTypeDiscriminator.Byte or ConstantValueTypeDiscriminator.Int16 or ConstantValueTypeDiscriminator.UInt16 or ConstantValueTypeDiscriminator.Int32 or ConstantValueTypeDiscriminator.UInt32 or ConstantValueTypeDiscriminator.Int64 or ConstantValueTypeDiscriminator.UInt64 or ConstantValueTypeDiscriminator.NInt or ConstantValueTypeDiscriminator.NUInt => true,
                _ => false,
            };
        }

        public bool IsIntegral
        {
            get
            {
                return IsIntegralType(this.Discriminator);
            }
        }

        public bool IsNegativeNumeric
        {
            get
            {
                return this.Discriminator switch
                {
                    ConstantValueTypeDiscriminator.SByte => SByteValue < 0,
                    ConstantValueTypeDiscriminator.Int16 => Int16Value < 0,
                    ConstantValueTypeDiscriminator.Int32 or ConstantValueTypeDiscriminator.NInt => Int32Value < 0,
                    ConstantValueTypeDiscriminator.Int64 => Int64Value < 0,
                    ConstantValueTypeDiscriminator.Single => SingleValue < 0,
                    ConstantValueTypeDiscriminator.Double => DoubleValue < 0,
                    ConstantValueTypeDiscriminator.Decimal => DecimalValue < 0,
                    _ => false,
                };
            }
        }

        public bool IsNumeric
        {
            get
            {
                return this.Discriminator switch
                {
                    ConstantValueTypeDiscriminator.SByte or ConstantValueTypeDiscriminator.Int16 or ConstantValueTypeDiscriminator.Int32 or ConstantValueTypeDiscriminator.Int64 or ConstantValueTypeDiscriminator.Single or ConstantValueTypeDiscriminator.Double or ConstantValueTypeDiscriminator.Decimal or ConstantValueTypeDiscriminator.Byte or ConstantValueTypeDiscriminator.UInt16 or ConstantValueTypeDiscriminator.UInt32 or ConstantValueTypeDiscriminator.UInt64 or ConstantValueTypeDiscriminator.NInt or ConstantValueTypeDiscriminator.NUInt => true,
                    _ => false,
                };
            }
        }

        public static bool IsUnsignedIntegralType(ConstantValueTypeDiscriminator discriminator)
        {
            return discriminator switch
            {
                ConstantValueTypeDiscriminator.Byte or ConstantValueTypeDiscriminator.UInt16 or ConstantValueTypeDiscriminator.UInt32 or ConstantValueTypeDiscriminator.UInt64 or ConstantValueTypeDiscriminator.NUInt => true,
                _ => false,
            };
        }

        public bool IsUnsigned
        {
            get
            {
                return IsUnsignedIntegralType(this.Discriminator);
            }
        }

        public static bool IsBooleanType(ConstantValueTypeDiscriminator discriminator)
        {
            return discriminator == ConstantValueTypeDiscriminator.Boolean;
        }

        public bool IsBoolean
        {
            get
            {
                return this.Discriminator == ConstantValueTypeDiscriminator.Boolean;
            }
        }

        public static bool IsCharType(ConstantValueTypeDiscriminator discriminator)
        {
            return discriminator == ConstantValueTypeDiscriminator.Char;
        }

        public bool IsChar
        {
            get
            {
                return this.Discriminator == ConstantValueTypeDiscriminator.Char;
            }
        }

        public static bool IsStringType(ConstantValueTypeDiscriminator discriminator)
        {
            return discriminator == ConstantValueTypeDiscriminator.String;
        }

        [MemberNotNullWhen(true, nameof(StringValue))]
        public bool IsString
        {
            get
            {
                return this.Discriminator == ConstantValueTypeDiscriminator.String;
            }
        }

        public static bool IsDecimalType(ConstantValueTypeDiscriminator discriminator)
        {
            return discriminator == ConstantValueTypeDiscriminator.Decimal;
        }

        public bool IsDecimal
        {
            get
            {
                return this.Discriminator == ConstantValueTypeDiscriminator.Decimal;
            }
        }

        public static bool IsDateTimeType(ConstantValueTypeDiscriminator discriminator)
        {
            return discriminator == ConstantValueTypeDiscriminator.DateTime;
        }

        public bool IsDateTime
        {
            get
            {
                return this.Discriminator == ConstantValueTypeDiscriminator.DateTime;
            }
        }

        public static bool IsFloatingType(ConstantValueTypeDiscriminator discriminator)
        {
            return discriminator == ConstantValueTypeDiscriminator.Double ||
                discriminator == ConstantValueTypeDiscriminator.Single;
        }

        public bool IsFloating
        {
            get
            {
                return this.Discriminator == ConstantValueTypeDiscriminator.Double ||
                    this.Discriminator == ConstantValueTypeDiscriminator.Single;
            }
        }

        public bool IsBad
        {
            get
            {
                return this.Discriminator == ConstantValueTypeDiscriminator.Bad;
            }
        }

        public bool IsNull
        {
            get
            {
                return ReferenceEquals(this, Null);
            }
        }

        public bool IsNothing
        {
            get
            {
                return ReferenceEquals(this, Nothing);
            }
        }

        public void Serialize(BlobBuilder writer)
        {
            switch (this.Discriminator)
            {
                case ConstantValueTypeDiscriminator.Boolean:
                    writer.WriteBoolean(this.BooleanValue);
                    break;

                case ConstantValueTypeDiscriminator.SByte:
                    writer.WriteSByte(this.SByteValue);
                    break;

                case ConstantValueTypeDiscriminator.Byte:
                    writer.WriteByte(this.ByteValue);
                    break;

                case ConstantValueTypeDiscriminator.Char:
                case ConstantValueTypeDiscriminator.Int16:
                    writer.WriteInt16(this.Int16Value);
                    break;

                case ConstantValueTypeDiscriminator.UInt16:
                    writer.WriteUInt16(this.UInt16Value);
                    break;

                case ConstantValueTypeDiscriminator.Single:
                    writer.WriteSingle(this.SingleValue);
                    break;

                case ConstantValueTypeDiscriminator.Int32:
                    writer.WriteInt32(this.Int32Value);
                    break;

                case ConstantValueTypeDiscriminator.UInt32:
                    writer.WriteUInt32(this.UInt32Value);
                    break;

                case ConstantValueTypeDiscriminator.Double:
                    writer.WriteDouble(this.DoubleValue);
                    break;

                case ConstantValueTypeDiscriminator.Int64:
                    writer.WriteInt64(this.Int64Value);
                    break;

                case ConstantValueTypeDiscriminator.UInt64:
                    writer.WriteUInt64(this.UInt64Value);
                    break;

                default: throw ExceptionUtilities.UnexpectedValue(this.Discriminator);
            }
        }

        public override string ToString()
        {
            string? valueToDisplay = this.GetValueToDisplay();
            return String.Format("{0}({1}: {2})", this.GetType().Name, valueToDisplay, this.Discriminator);
        }

        public virtual string? GetValueToDisplay()
        {
            return this.Value?.ToString();
        }

        // equal constants must have matching discriminators
        // derived types override this if equivalence is more than just discriminators match. 
        // singletons also override this since they only need a reference compare.
        public virtual bool Equals(ConstantValue? other)
        {
            if (ReferenceEquals(other, this))
            {
                return true;
            }

            if (other is null)
            {
                return false;
            }

            return this.Discriminator == other.Discriminator;
        }

        public static bool operator ==(ConstantValue? left, ConstantValue? right)
        {
            if (ReferenceEquals(right, left))
            {
                return true;
            }

            if (left is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(ConstantValue? left, ConstantValue? right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return this.Discriminator.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return this.Equals(obj as ConstantValue);
        }
    }
}
