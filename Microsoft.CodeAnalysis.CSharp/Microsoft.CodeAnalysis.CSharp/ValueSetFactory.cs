using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class ValueSetFactory
    {
        private sealed class BoolValueSet : IValueSet<bool>, IValueSet
        {
            private readonly bool _hasFalse;

            private readonly bool _hasTrue;

            internal static readonly BoolValueSet AllValues = new BoolValueSet(hasFalse: true, hasTrue: true);

            internal static readonly BoolValueSet None = new BoolValueSet(hasFalse: false, hasTrue: false);

            internal static readonly BoolValueSet OnlyTrue = new BoolValueSet(hasFalse: false, hasTrue: true);

            internal static readonly BoolValueSet OnlyFalse = new BoolValueSet(hasFalse: true, hasTrue: false);

            bool IValueSet.IsEmpty
            {
                get
                {
                    if (!_hasFalse)
                    {
                        return !_hasTrue;
                    }
                    return false;
                }
            }

            ConstantValue IValueSet.Sample
            {
                get
                {
                    int value;
                    if (!_hasTrue)
                    {
                        if (!_hasFalse)
                        {
                            throw new ArgumentException();
                        }
                        value = 0;
                    }
                    else
                    {
                        value = 1;
                    }
                    return ConstantValue.Create((byte)value != 0);
                }
            }

            private BoolValueSet(bool hasFalse, bool hasTrue)
            {
                bool hasFalse2 = hasFalse;
                bool hasTrue2 = hasTrue;
                _hasFalse = hasFalse2;
                _hasTrue = hasTrue2;
            }

            public static BoolValueSet Create(bool hasFalse, bool hasTrue)
            {
                if (!hasFalse)
                {
                    if (!hasTrue)
                    {
                        return None;
                    }
                    return OnlyTrue;
                }
                if (!hasTrue)
                {
                    return OnlyFalse;
                }
                return AllValues;
            }

            public bool Any(BinaryOperatorKind relation, bool value)
            {
                if (relation == BinaryOperatorKind.Equal)
                {
                    if (value)
                    {
                        return _hasTrue;
                    }
                    return _hasFalse;
                }
                return true;
            }

            bool IValueSet.Any(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return Any(relation, value.BooleanValue);
                }
                return true;
            }

            public bool All(BinaryOperatorKind relation, bool value)
            {
                if (relation == BinaryOperatorKind.Equal)
                {
                    if (value)
                    {
                        return !_hasFalse;
                    }
                    return !_hasTrue;
                }
                return true;
            }

            bool IValueSet.All(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return All(relation, value.BooleanValue);
                }
                return false;
            }

            public IValueSet<bool> Complement()
            {
                return Create(!_hasFalse, !_hasTrue);
            }

            IValueSet IValueSet.Complement()
            {
                return Complement();
            }

            public IValueSet<bool> Intersect(IValueSet<bool> other)
            {
                if (this == other)
                {
                    return this;
                }
                BoolValueSet boolValueSet = (BoolValueSet)other;
                return Create(_hasFalse & boolValueSet._hasFalse, _hasTrue & boolValueSet._hasTrue);
            }

            public IValueSet Intersect(IValueSet other)
            {
                return Intersect((IValueSet<bool>)other);
            }

            public IValueSet<bool> Union(IValueSet<bool> other)
            {
                if (this == other)
                {
                    return this;
                }
                BoolValueSet boolValueSet = (BoolValueSet)other;
                return Create(_hasFalse | boolValueSet._hasFalse, _hasTrue | boolValueSet._hasTrue);
            }

            IValueSet IValueSet.Union(IValueSet other)
            {
                return Union((IValueSet<bool>)other);
            }

            public override bool Equals(object? obj)
            {
                return this == obj;
            }

            public override int GetHashCode()
            {
                return RuntimeHelpers.GetHashCode(this);
            }

            public override string ToString()
            {
                bool hasFalse = _hasFalse;
                bool hasTrue = _hasTrue;
                if (!hasFalse)
                {
                    if (!hasTrue)
                    {
                        return "{}";
                    }
                    return "{true}";
                }
                if (!hasTrue)
                {
                    return "{false}";
                }
                return "{false,true}";
            }
        }

        private sealed class BoolValueSetFactory : IValueSetFactory<bool>, IValueSetFactory
        {
            public static readonly BoolValueSetFactory Instance = new BoolValueSetFactory();

            IValueSet IValueSetFactory.AllValues => BoolValueSet.AllValues;

            IValueSet IValueSetFactory.NoValues => BoolValueSet.None;

            private BoolValueSetFactory()
            {
            }

            public IValueSet<bool> Related(BinaryOperatorKind relation, bool value)
            {
                if (relation == BinaryOperatorKind.Equal)
                {
                    if (value)
                    {
                        return BoolValueSet.OnlyTrue;
                    }
                    return BoolValueSet.OnlyFalse;
                }
                return BoolValueSet.AllValues;
            }

            IValueSet IValueSetFactory.Random(int expectedSize, Random random)
            {
                return random.Next(4) switch
                {
                    0 => BoolValueSet.None,
                    1 => BoolValueSet.OnlyFalse,
                    2 => BoolValueSet.OnlyTrue,
                    3 => BoolValueSet.AllValues,
                    _ => throw ExceptionUtilities.UnexpectedValue("random"),
                };
            }

            ConstantValue IValueSetFactory.RandomValue(Random random)
            {
                return ConstantValue.Create(random.NextDouble() < 0.5);
            }

            IValueSet IValueSetFactory.Related(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return Related(relation, value.BooleanValue);
                }
                return BoolValueSet.AllValues;
            }

            bool IValueSetFactory.Related(BinaryOperatorKind relation, ConstantValue left, ConstantValue right)
            {
                if (!left.IsBad && !right.IsBad)
                {
                    return left.BooleanValue == right.BooleanValue;
                }
                return true;
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct ByteTC : INumericTC<byte>
        {
            byte INumericTC<byte>.MinValue => 0;

            byte INumericTC<byte>.MaxValue => byte.MaxValue;

            byte INumericTC<byte>.Zero => 0;

            bool INumericTC<byte>.Related(BinaryOperatorKind relation, byte left, byte right)
            {
                return relation switch
                {
                    BinaryOperatorKind.Equal => left == right,
                    BinaryOperatorKind.GreaterThanOrEqual => left >= right,
                    BinaryOperatorKind.GreaterThan => left > right,
                    BinaryOperatorKind.LessThanOrEqual => left <= right,
                    BinaryOperatorKind.LessThan => left < right,
                    _ => throw new ArgumentException("relation"),
                };
            }

            byte INumericTC<byte>.Next(byte value)
            {
                return (byte)(value + 1);
            }

            byte INumericTC<byte>.Prev(byte value)
            {
                return (byte)(value - 1);
            }

            byte INumericTC<byte>.FromConstantValue(ConstantValue constantValue)
            {
                if (!constantValue.IsBad)
                {
                    return constantValue.ByteValue;
                }
                return 0;
            }

            ConstantValue INumericTC<byte>.ToConstantValue(byte value)
            {
                return ConstantValue.Create(value);
            }

            string INumericTC<byte>.ToString(byte value)
            {
                return value.ToString();
            }

            byte INumericTC<byte>.Random(Random random)
            {
                return (byte)random.Next(0, 256);
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct CharTC : INumericTC<char>
        {
            char INumericTC<char>.MinValue => '\0';

            char INumericTC<char>.MaxValue => '\uffff';

            char INumericTC<char>.Zero => '\0';

            bool INumericTC<char>.Related(BinaryOperatorKind relation, char left, char right)
            {
                return relation switch
                {
                    BinaryOperatorKind.Equal => left == right,
                    BinaryOperatorKind.GreaterThanOrEqual => left >= right,
                    BinaryOperatorKind.GreaterThan => left > right,
                    BinaryOperatorKind.LessThanOrEqual => left <= right,
                    BinaryOperatorKind.LessThan => left < right,
                    _ => throw new ArgumentException("relation"),
                };
            }

            char INumericTC<char>.Next(char value)
            {
                return (char)(value + 1);
            }

            char INumericTC<char>.FromConstantValue(ConstantValue constantValue)
            {
                if (!constantValue.IsBad)
                {
                    return constantValue.CharValue;
                }
                return '\0';
            }

            string INumericTC<char>.ToString(char c)
            {
                return ObjectDisplay.FormatPrimitive(c, ObjectDisplayOptions.UseQuotes | ObjectDisplayOptions.EscapeNonPrintableCharacters);
            }

            char INumericTC<char>.Prev(char value)
            {
                return (char)(value - 1);
            }

            char INumericTC<char>.Random(Random random)
            {
                return (char)random.Next(0, 65536);
            }

            ConstantValue INumericTC<char>.ToConstantValue(char value)
            {
                return ConstantValue.Create(value);
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct DecimalTC : INumericTC<decimal>
        {
            private struct DecimalRep
            {
                public readonly uint low;

                public readonly uint mid;

                public readonly uint high;

                public readonly bool isNegative;

                public readonly byte scale;

                public decimal Value => new decimal((int)low, (int)mid, (int)high, isNegative, scale);

                public DecimalRep(uint low, uint mid, uint high, bool isNegative, byte scale)
                {
                    if (scale > 28)
                    {
                        throw new ArgumentException("scale");
                    }
                    this.low = low;
                    this.mid = mid;
                    this.high = high;
                    this.isNegative = isNegative;
                    this.scale = scale;
                }

                public DecimalRep Normalize()
                {
                    if (scale == 28)
                    {
                        return this;
                    }
                    DecimalRep decimalRep = this;
                    decimalRep.Deconstruct(out var num, out var num2, out var num3, out var flag, out var b);
                    uint num4 = num;
                    uint num5 = num2;
                    uint num6 = num3;
                    bool flag2 = flag;
                    byte b2 = b;
                    while (b2 < 28 && num6 * 10L <= uint.MaxValue)
                    {
                        long num7 = 10L * num6;
                        long num8 = 10L * num5;
                        long num9 = 10L * num4;
                        num8 += num9 >> 32;
                        num9 &= 0xFFFFFFFFu;
                        num7 += num8 >> 32;
                        num8 &= 0xFFFFFFFFu;
                        if (num7 > uint.MaxValue)
                        {
                            break;
                        }
                        num4 = (uint)num9;
                        num5 = (uint)num8;
                        num6 = (uint)num7;
                        b2 = (byte)(b2 + 1);
                    }
                    return new DecimalRep(num4, num5, num6, flag2, b2);
                }

                public static DecimalRep FromValue(decimal value)
                {
                    value.GetBits(out var flag, out var b, out var num, out var num2, out var num3);
                    return new DecimalRep(num, num2, num3, flag, b);
                }

                public void Deconstruct(out uint low, out uint mid, out uint high, out bool isNegative, out byte scale)
                {
                    uint num = this.low;
                    uint num2 = this.mid;
                    uint num3 = this.high;
                    bool flag = this.isNegative;
                    byte b = this.scale;
                    low = num;
                    mid = num2;
                    high = num3;
                    isNegative = flag;
                    scale = b;
                }

                public override string ToString()
                {
                    return string.Format("Decimal({0}, 0x{1:08X} 0x{2:08X} 0x{3:08X} *10^-{4})", isNegative ? "-" : "+", high, mid, low, scale);
                }
            }

            private const uint transitionLow = 2576980377u;

            private const uint transitionMid = 2576980377u;

            private const uint transitionHigh = 429496729u;

            private const byte maxScale = 28;

            private static readonly decimal normalZero = 0.0000000000000000000000000000m;

            private static readonly decimal epsilon = 0.0000000000000000000000000001m;

            decimal INumericTC<decimal>.MinValue => decimal.MinValue;

            decimal INumericTC<decimal>.MaxValue => decimal.MaxValue;

            decimal INumericTC<decimal>.Zero => 0m;

            public decimal FromConstantValue(ConstantValue constantValue)
            {
                if (!constantValue.IsBad)
                {
                    return constantValue.DecimalValue;
                }
                return 0m;
            }

            public ConstantValue ToConstantValue(decimal value)
            {
                return ConstantValue.Create(value);
            }

            public decimal Next(decimal value)
            {
                if (value == 0m)
                {
                    return epsilon;
                }
                DecimalRep.FromValue(value).Deconstruct(out var low, out var mid, out var high, out var isNegative, out var scale);
                uint num = low;
                uint num2 = mid;
                uint num3 = high;
                bool flag = isNegative;
                byte b = scale;
                if (flag)
                {
                    if (value == -epsilon)
                    {
                        return normalZero;
                    }
                    if (num != 0)
                    {
                        return new DecimalRep(num - 1, num2, num3, flag, b).Value;
                    }
                    if (num2 != 0)
                    {
                        return new DecimalRep(uint.MaxValue, num2 - 1, num3, flag, b).Value;
                    }
                    return new DecimalRep(uint.MaxValue, uint.MaxValue, num3 - 1, flag, b).Value;
                }
                if (num != uint.MaxValue)
                {
                    return new DecimalRep(num + 1, num2, num3, flag, b).Value;
                }
                if (num2 != uint.MaxValue)
                {
                    return new DecimalRep(0u, num2 + 1, num3, flag, b).Value;
                }
                if (num3 != uint.MaxValue)
                {
                    return new DecimalRep(0u, 0u, num3 + 1, flag, b).Value;
                }
                num = 2576980377u;
                num2 = 2576980377u;
                num3 = 429496729u;
                b = (byte)(b - 1);
                return new DecimalRep(num + 1, num2, num3, flag, b).Value;
            }

            bool INumericTC<decimal>.Related(BinaryOperatorKind relation, decimal left, decimal right)
            {
                return relation switch
                {
                    BinaryOperatorKind.Equal => left == right,
                    BinaryOperatorKind.GreaterThanOrEqual => left >= right,
                    BinaryOperatorKind.GreaterThan => left > right,
                    BinaryOperatorKind.LessThanOrEqual => left <= right,
                    BinaryOperatorKind.LessThan => left < right,
                    _ => throw new ArgumentException("relation"),
                };
            }

            string INumericTC<decimal>.ToString(decimal value)
            {
                return $"{value:G}";
            }

            decimal INumericTC<decimal>.Prev(decimal value)
            {
                return -Next(-value);
            }

            public decimal Random(Random random)
            {
                INumericTC<uint> numericTC = default(UIntTC);
                return new DecimalRep(numericTC.Random(random), numericTC.Random(random), numericTC.Random(random), random.NextDouble() < 0.5, (byte)random.Next(0, 29)).Normalize().Value;
            }

            public static decimal Normalize(decimal value)
            {
                return DecimalRep.FromValue(value).Normalize().Value;
            }
        }

        private sealed class DecimalValueSetFactory : IValueSetFactory<decimal>, IValueSetFactory
        {
            public static readonly DecimalValueSetFactory Instance = new DecimalValueSetFactory();

            private readonly IValueSetFactory<decimal> _underlying = NumericValueSetFactory<decimal, DecimalTC>.Instance;

            IValueSet IValueSetFactory.AllValues => NumericValueSet<decimal, DecimalTC>.AllValues;

            IValueSet IValueSetFactory.NoValues => NumericValueSet<decimal, DecimalTC>.NoValues;

            public IValueSet<decimal> Related(BinaryOperatorKind relation, decimal value)
            {
                return _underlying.Related(relation, DecimalTC.Normalize(value));
            }

            IValueSet IValueSetFactory.Random(int expectedSize, Random random)
            {
                return _underlying.Random(expectedSize, random);
            }

            ConstantValue IValueSetFactory.RandomValue(Random random)
            {
                return ConstantValue.Create(default(DecimalTC).Random(random));
            }

            IValueSet IValueSetFactory.Related(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return Related(relation, default(DecimalTC).FromConstantValue(value));
                }
                return NumericValueSet<decimal, DecimalTC>.AllValues;
            }

            bool IValueSetFactory.Related(BinaryOperatorKind relation, ConstantValue left, ConstantValue right)
            {
                return _underlying.Related(relation, left, right);
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct DoubleTC : IFloatingTC<double>, INumericTC<double>
        {
            double INumericTC<double>.MinValue => double.NegativeInfinity;

            double INumericTC<double>.MaxValue => double.PositiveInfinity;

            double IFloatingTC<double>.NaN => double.NaN;

            double INumericTC<double>.Zero => 0.0;

            public double Next(double value)
            {
                if (value == 0.0)
                {
                    return double.Epsilon;
                }
                if (value < 0.0)
                {
                    if (value == -4.94065645841247E-324)
                    {
                        return 0.0;
                    }
                    if (value == double.NegativeInfinity)
                    {
                        return double.MinValue;
                    }
                    return 0.0 - ULongAsDouble(DoubleAsULong(0.0 - value) - 1);
                }
                if (value == double.MaxValue)
                {
                    return double.PositiveInfinity;
                }
                return ULongAsDouble(DoubleAsULong(value) + 1);
            }

            private static ulong DoubleAsULong(double d)
            {
                if (d == 0.0)
                {
                    return 0uL;
                }
                return (ulong)BitConverter.DoubleToInt64Bits(d);
            }

            private static double ULongAsDouble(ulong l)
            {
                return BitConverter.Int64BitsToDouble((long)l);
            }

            bool INumericTC<double>.Related(BinaryOperatorKind relation, double left, double right)
            {
                switch (relation)
                {
                    case BinaryOperatorKind.Equal:
                        if (left != right)
                        {
                            if (double.IsNaN(left))
                            {
                                return double.IsNaN(right);
                            }
                            return false;
                        }
                        return true;
                    case BinaryOperatorKind.GreaterThanOrEqual:
                        return left >= right;
                    case BinaryOperatorKind.GreaterThan:
                        return left > right;
                    case BinaryOperatorKind.LessThanOrEqual:
                        return left <= right;
                    case BinaryOperatorKind.LessThan:
                        return left < right;
                    default:
                        throw new ArgumentException("relation");
                }
            }

            double INumericTC<double>.FromConstantValue(ConstantValue constantValue)
            {
                if (!constantValue.IsBad)
                {
                    return constantValue.DoubleValue;
                }
                return 0.0;
            }

            ConstantValue INumericTC<double>.ToConstantValue(double value)
            {
                return ConstantValue.Create(value);
            }

            string INumericTC<double>.ToString(double value)
            {
                if (!double.IsNaN(value))
                {
                    if (value != double.NegativeInfinity)
                    {
                        if (value != double.PositiveInfinity)
                        {
                            return $"{value:G17}";
                        }
                        return "Inf";
                    }
                    return "-Inf";
                }
                return "NaN";
            }

            double INumericTC<double>.Prev(double value)
            {
                return 0.0 - Next(0.0 - value);
            }

            double INumericTC<double>.Random(Random random)
            {
                return random.NextDouble() * 100.0 - 50.0;
            }
        }

        private sealed class EnumeratedValueSet<T, TTC> : IValueSet<T>, IValueSet where T : notnull where TTC : struct, IEquatableValueTC<T>
        {
            private readonly bool _included;

            private readonly ImmutableHashSet<T> _membersIncludedOrExcluded;

            public static readonly EnumeratedValueSet<T, TTC> AllValues = new EnumeratedValueSet<T, TTC>(included: false, ImmutableHashSet<T>.Empty);

            public static readonly EnumeratedValueSet<T, TTC> NoValues = new EnumeratedValueSet<T, TTC>(included: true, ImmutableHashSet<T>.Empty);

            public bool IsEmpty
            {
                get
                {
                    if (_included)
                    {
                        return _membersIncludedOrExcluded.IsEmpty;
                    }
                    return false;
                }
            }

            ConstantValue IValueSet.Sample
            {
                get
                {
                    if (IsEmpty)
                    {
                        throw new ArgumentException();
                    }
                    TTC val = default(TTC);
                    if (_included)
                    {
                        return val.ToConstantValue(_membersIncludedOrExcluded.OrderBy((T k) => k).First());
                    }
                    if (typeof(T) == typeof(string))
                    {
                        if (Any(BinaryOperatorKind.Equal, (T)(object)""))
                        {
                            return val.ToConstantValue((T)(object)"");
                        }
                        for (char c = 'A'; c <= 'z'; c = (char)(c + 1))
                        {
                            if (Any(BinaryOperatorKind.Equal, (T)(object)c.ToString()))
                            {
                                return val.ToConstantValue((T)(object)c.ToString());
                            }
                        }
                    }
                    T[] array = val.RandomValues(_membersIncludedOrExcluded.Count + 1, new Random(0), _membersIncludedOrExcluded.Count + 1);
                    foreach (T value in array)
                    {
                        if (Any(BinaryOperatorKind.Equal, value))
                        {
                            return val.ToConstantValue(value);
                        }
                    }
                    throw ExceptionUtilities.Unreachable;
                }
            }

            private EnumeratedValueSet(bool included, ImmutableHashSet<T> membersIncludedOrExcluded)
            {
                bool included2 = included;
                _included = included2;
                _membersIncludedOrExcluded = membersIncludedOrExcluded;
            }

            internal static EnumeratedValueSet<T, TTC> Including(T value)
            {
                return new EnumeratedValueSet<T, TTC>(included: true, ImmutableHashSet<T>.Empty.Add(value));
            }

            public bool Any(BinaryOperatorKind relation, T value)
            {
                if (relation == BinaryOperatorKind.Equal)
                {
                    return _included == _membersIncludedOrExcluded.Contains(value);
                }
                return true;
            }

            bool IValueSet.Any(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return Any(relation, default(TTC).FromConstantValue(value));
                }
                return true;
            }

            public bool All(BinaryOperatorKind relation, T value)
            {
                if (relation == BinaryOperatorKind.Equal)
                {
                    if (!_included)
                    {
                        return false;
                    }
                    return _membersIncludedOrExcluded.Count switch
                    {
                        0 => true,
                        1 => _membersIncludedOrExcluded.Contains(value),
                        _ => false,
                    };
                }
                return false;
            }

            bool IValueSet.All(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return All(relation, default(TTC).FromConstantValue(value));
                }
                return false;
            }

            public IValueSet<T> Complement()
            {
                return new EnumeratedValueSet<T, TTC>(!_included, _membersIncludedOrExcluded);
            }

            IValueSet IValueSet.Complement()
            {
                return Complement();
            }

            public IValueSet<T> Intersect(IValueSet<T> o)
            {
                if (this == o)
                {
                    return this;
                }
                EnumeratedValueSet<T, TTC> enumeratedValueSet = (EnumeratedValueSet<T, TTC>)o;
                EnumeratedValueSet<T, TTC> enumeratedValueSet2;
                EnumeratedValueSet<T, TTC> enumeratedValueSet3;
                if (_membersIncludedOrExcluded.Count <= enumeratedValueSet._membersIncludedOrExcluded.Count)
                {
                    enumeratedValueSet2 = enumeratedValueSet;
                    enumeratedValueSet3 = this;
                }
                else
                {
                    EnumeratedValueSet<T, TTC> enumeratedValueSet4 = enumeratedValueSet;
                    enumeratedValueSet2 = this;
                    enumeratedValueSet3 = enumeratedValueSet4;
                }
                bool included = enumeratedValueSet2._included;
                bool included2 = enumeratedValueSet3._included;
                if (included)
                {
                    if (included2)
                    {
                        return new EnumeratedValueSet<T, TTC>(included: true, enumeratedValueSet2._membersIncludedOrExcluded.Intersect(enumeratedValueSet3._membersIncludedOrExcluded));
                    }
                    return new EnumeratedValueSet<T, TTC>(included: true, enumeratedValueSet2._membersIncludedOrExcluded.Except(enumeratedValueSet3._membersIncludedOrExcluded));
                }
                if (!included2)
                {
                    return new EnumeratedValueSet<T, TTC>(included: false, enumeratedValueSet2._membersIncludedOrExcluded.Union(enumeratedValueSet3._membersIncludedOrExcluded));
                }
                return new EnumeratedValueSet<T, TTC>(included: true, enumeratedValueSet3._membersIncludedOrExcluded.Except(enumeratedValueSet2._membersIncludedOrExcluded));
            }

            IValueSet IValueSet.Intersect(IValueSet other)
            {
                return Intersect((IValueSet<T>)other);
            }

            public IValueSet<T> Union(IValueSet<T> o)
            {
                if (this == o)
                {
                    return this;
                }
                EnumeratedValueSet<T, TTC> enumeratedValueSet = (EnumeratedValueSet<T, TTC>)o;
                EnumeratedValueSet<T, TTC> enumeratedValueSet2;
                EnumeratedValueSet<T, TTC> enumeratedValueSet3;
                if (_membersIncludedOrExcluded.Count <= enumeratedValueSet._membersIncludedOrExcluded.Count)
                {
                    enumeratedValueSet2 = enumeratedValueSet;
                    enumeratedValueSet3 = this;
                }
                else
                {
                    EnumeratedValueSet<T, TTC> enumeratedValueSet4 = enumeratedValueSet;
                    enumeratedValueSet2 = this;
                    enumeratedValueSet3 = enumeratedValueSet4;
                }
                bool included = enumeratedValueSet2._included;
                bool included2 = enumeratedValueSet3._included;
                if (!included)
                {
                    if (!included2)
                    {
                        return new EnumeratedValueSet<T, TTC>(included: false, enumeratedValueSet2._membersIncludedOrExcluded.Intersect(enumeratedValueSet3._membersIncludedOrExcluded));
                    }
                    return new EnumeratedValueSet<T, TTC>(included: false, enumeratedValueSet2._membersIncludedOrExcluded.Except(enumeratedValueSet3._membersIncludedOrExcluded));
                }
                if (included2)
                {
                    return new EnumeratedValueSet<T, TTC>(included: true, enumeratedValueSet2._membersIncludedOrExcluded.Union(enumeratedValueSet3._membersIncludedOrExcluded));
                }
                return new EnumeratedValueSet<T, TTC>(included: false, enumeratedValueSet3._membersIncludedOrExcluded.Except(enumeratedValueSet2._membersIncludedOrExcluded));
            }

            IValueSet IValueSet.Union(IValueSet other)
            {
                return Union((IValueSet<T>)other);
            }

            public override bool Equals(object? obj)
            {
                if (obj is EnumeratedValueSet<T, TTC> enumeratedValueSet && _included == enumeratedValueSet._included)
                {
                    return _membersIncludedOrExcluded.SetEquals(enumeratedValueSet._membersIncludedOrExcluded);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Hash.Combine(_included.GetHashCode(), _membersIncludedOrExcluded.GetHashCode());
            }

            public override string ToString()
            {
                return (_included ? "" : "~") + "{" + string.Join(",", _membersIncludedOrExcluded.Select((T o) => o.ToString())) + "}";
            }
        }

        private sealed class EnumeratedValueSetFactory<T, TTC> : IValueSetFactory<T>, IValueSetFactory where T : notnull where TTC : struct, IEquatableValueTC<T>
        {
            public static readonly EnumeratedValueSetFactory<T, TTC> Instance = new EnumeratedValueSetFactory<T, TTC>();

            IValueSet IValueSetFactory.AllValues => EnumeratedValueSet<T, TTC>.AllValues;

            IValueSet IValueSetFactory.NoValues => EnumeratedValueSet<T, TTC>.NoValues;

            private EnumeratedValueSetFactory()
            {
            }

            public IValueSet<T> Related(BinaryOperatorKind relation, T value)
            {
                if (relation == BinaryOperatorKind.Equal)
                {
                    return EnumeratedValueSet<T, TTC>.Including(value);
                }
                return EnumeratedValueSet<T, TTC>.AllValues;
            }

            IValueSet IValueSetFactory.Related(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad && !value.IsNull)
                {
                    return Related(relation, default(TTC).FromConstantValue(value));
                }
                return EnumeratedValueSet<T, TTC>.AllValues;
            }

            bool IValueSetFactory.Related(BinaryOperatorKind relation, ConstantValue left, ConstantValue right)
            {
                TTC val = default(TTC);
                return val.FromConstantValue(left).Equals(val.FromConstantValue(right));
            }

            public IValueSet Random(int expectedSize, Random random)
            {
                T[] array = default(TTC).RandomValues(expectedSize, random, expectedSize * 2);
                IValueSet<T> valueSet = EnumeratedValueSet<T, TTC>.NoValues;
                T[] array2 = array;
                foreach (T value in array2)
                {
                    valueSet = valueSet.Union(Related(BinaryOperatorKind.Equal, value));
                }
                return valueSet;
            }

            ConstantValue IValueSetFactory.RandomValue(Random random)
            {
                TTC val = default(TTC);
                return val.ToConstantValue(val.RandomValues(1, random, 100)[0]);
            }
        }

        private interface IFloatingTC<T> : INumericTC<T>
        {
            T NaN { get; }
        }

        private sealed class FloatingValueSet<TFloating, TFloatingTC> : IValueSet<TFloating>, IValueSet where TFloatingTC : struct, IFloatingTC<TFloating>
        {
            private readonly IValueSet<TFloating> _numbers;

            private readonly bool _hasNaN;

            internal static readonly IValueSet<TFloating> AllValues = new FloatingValueSet<TFloating, TFloatingTC>(NumericValueSet<TFloating, TFloatingTC>.AllValues, hasNaN: true);

            internal static readonly IValueSet<TFloating> NoValues = new FloatingValueSet<TFloating, TFloatingTC>(NumericValueSet<TFloating, TFloatingTC>.NoValues, hasNaN: false);

            public bool IsEmpty
            {
                get
                {
                    if (!_hasNaN)
                    {
                        return _numbers.IsEmpty;
                    }
                    return false;
                }
            }

            ConstantValue IValueSet.Sample
            {
                get
                {
                    if (IsEmpty)
                    {
                        throw new ArgumentException();
                    }
                    if (!_numbers.IsEmpty)
                    {
                        return _numbers.Sample;
                    }
                    TFloatingTC val = default(TFloatingTC);
                    return val.ToConstantValue(val.NaN);
                }
            }

            private FloatingValueSet(IValueSet<TFloating> numbers, bool hasNaN)
            {
                bool hasNaN2 = hasNaN;
                _numbers = numbers;
                _hasNaN = hasNaN2;
            }

            internal static IValueSet<TFloating> Random(int expectedSize, Random random)
            {
                bool flag = random.NextDouble() < 0.5;
                if (flag)
                {
                    expectedSize--;
                }
                if (expectedSize < 1)
                {
                    expectedSize = 2;
                }
                return new FloatingValueSet<TFloating, TFloatingTC>((IValueSet<TFloating>)NumericValueSetFactory<TFloating, TFloatingTC>.Instance.Random(expectedSize, random), flag);
            }

            public static IValueSet<TFloating> Related(BinaryOperatorKind relation, TFloating value)
            {
                TFloatingTC val = default(TFloatingTC);
                if (val.Related(BinaryOperatorKind.Equal, val.NaN, value))
                {
                    switch (relation)
                    {
                        case BinaryOperatorKind.Equal:
                        case BinaryOperatorKind.GreaterThanOrEqual:
                        case BinaryOperatorKind.LessThanOrEqual:
                            return new FloatingValueSet<TFloating, TFloatingTC>(NumericValueSet<TFloating, TFloatingTC>.NoValues, hasNaN: true);
                        case BinaryOperatorKind.GreaterThan:
                        case BinaryOperatorKind.LessThan:
                            return NoValues;
                        default:
                            throw ExceptionUtilities.UnexpectedValue(relation);
                    }
                }
                return new FloatingValueSet<TFloating, TFloatingTC>(NumericValueSetFactory<TFloating, TFloatingTC>.Instance.Related(relation, value), hasNaN: false);
            }

            public IValueSet<TFloating> Intersect(IValueSet<TFloating> o)
            {
                if (this == o)
                {
                    return this;
                }
                FloatingValueSet<TFloating, TFloatingTC> floatingValueSet = (FloatingValueSet<TFloating, TFloatingTC>)o;
                return new FloatingValueSet<TFloating, TFloatingTC>(_numbers.Intersect(floatingValueSet._numbers), _hasNaN & floatingValueSet._hasNaN);
            }

            IValueSet IValueSet.Intersect(IValueSet other)
            {
                return Intersect((IValueSet<TFloating>)other);
            }

            public IValueSet<TFloating> Union(IValueSet<TFloating> o)
            {
                if (this == o)
                {
                    return this;
                }
                FloatingValueSet<TFloating, TFloatingTC> floatingValueSet = (FloatingValueSet<TFloating, TFloatingTC>)o;
                return new FloatingValueSet<TFloating, TFloatingTC>(_numbers.Union(floatingValueSet._numbers), _hasNaN | floatingValueSet._hasNaN);
            }

            IValueSet IValueSet.Union(IValueSet other)
            {
                return Union((IValueSet<TFloating>)other);
            }

            public IValueSet<TFloating> Complement()
            {
                return new FloatingValueSet<TFloating, TFloatingTC>(_numbers.Complement(), !_hasNaN);
            }

            IValueSet IValueSet.Complement()
            {
                return Complement();
            }

            bool IValueSet.Any(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return Any(relation, default(TFloatingTC).FromConstantValue(value));
                }
                return true;
            }

            public bool Any(BinaryOperatorKind relation, TFloating value)
            {
                TFloatingTC val = default(TFloatingTC);
                if (!_hasNaN || !val.Related(relation, val.NaN, value))
                {
                    return _numbers.Any(relation, value);
                }
                return true;
            }

            bool IValueSet.All(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return All(relation, default(TFloatingTC).FromConstantValue(value));
                }
                return false;
            }

            public bool All(BinaryOperatorKind relation, TFloating value)
            {
                TFloatingTC val = default(TFloatingTC);
                if (!_hasNaN || val.Related(relation, val.NaN, value))
                {
                    return _numbers.All(relation, value);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return _numbers.GetHashCode();
            }

            public override bool Equals(object? obj)
            {
                if (this != obj)
                {
                    if (obj is FloatingValueSet<TFloating, TFloatingTC> floatingValueSet && _hasNaN == floatingValueSet._hasNaN)
                    {
                        return _numbers.Equals(floatingValueSet._numbers);
                    }
                    return false;
                }
                return true;
            }

            public override string ToString()
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (_hasNaN)
                {
                    stringBuilder.Append("NaN");
                }
                string text = _numbers.ToString();
                if (stringBuilder.Length > 1 && text.Length > 1)
                {
                    stringBuilder.Append(",");
                }
                stringBuilder.Append(text);
                return stringBuilder.ToString();
            }
        }

        private sealed class FloatingValueSetFactory<TFloating, TFloatingTC> : IValueSetFactory<TFloating>, IValueSetFactory where TFloatingTC : struct, IFloatingTC<TFloating>
        {
            public static readonly FloatingValueSetFactory<TFloating, TFloatingTC> Instance = new FloatingValueSetFactory<TFloating, TFloatingTC>();

            IValueSet IValueSetFactory.AllValues => FloatingValueSet<TFloating, TFloatingTC>.AllValues;

            IValueSet IValueSetFactory.NoValues => FloatingValueSet<TFloating, TFloatingTC>.NoValues;

            private FloatingValueSetFactory()
            {
            }

            public IValueSet<TFloating> Related(BinaryOperatorKind relation, TFloating value)
            {
                return FloatingValueSet<TFloating, TFloatingTC>.Related(relation, value);
            }

            IValueSet IValueSetFactory.Random(int expectedSize, Random random)
            {
                return FloatingValueSet<TFloating, TFloatingTC>.Random(expectedSize, random);
            }

            ConstantValue IValueSetFactory.RandomValue(Random random)
            {
                TFloatingTC val = default(TFloatingTC);
                return val.ToConstantValue(val.Random(random));
            }

            IValueSet IValueSetFactory.Related(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return FloatingValueSet<TFloating, TFloatingTC>.Related(relation, default(TFloatingTC).FromConstantValue(value));
                }
                return FloatingValueSet<TFloating, TFloatingTC>.AllValues;
            }

            bool IValueSetFactory.Related(BinaryOperatorKind relation, ConstantValue left, ConstantValue right)
            {
                TFloatingTC val = default(TFloatingTC);
                return val.Related(relation, val.FromConstantValue(left), val.FromConstantValue(right));
            }
        }

        private interface IEquatableValueTC<T> where T : notnull
        {
            T FromConstantValue(ConstantValue constantValue);

            ConstantValue ToConstantValue(T value);

            T[] RandomValues(int count, Random random, int scope = 0);
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct IntTC : INumericTC<int>
        {
            int INumericTC<int>.MinValue => int.MinValue;

            int INumericTC<int>.MaxValue => int.MaxValue;

            int INumericTC<int>.Zero => 0;

            public bool Related(BinaryOperatorKind relation, int left, int right)
            {
                return relation switch
                {
                    BinaryOperatorKind.Equal => left == right,
                    BinaryOperatorKind.GreaterThanOrEqual => left >= right,
                    BinaryOperatorKind.GreaterThan => left > right,
                    BinaryOperatorKind.LessThanOrEqual => left <= right,
                    BinaryOperatorKind.LessThan => left < right,
                    _ => throw new ArgumentException("relation"),
                };
            }

            int INumericTC<int>.Next(int value)
            {
                return value + 1;
            }

            int INumericTC<int>.Prev(int value)
            {
                return value - 1;
            }

            public int FromConstantValue(ConstantValue constantValue)
            {
                if (!constantValue.IsBad)
                {
                    return constantValue.Int32Value;
                }
                return 0;
            }

            public ConstantValue ToConstantValue(int value)
            {
                return ConstantValue.Create(value);
            }

            string INumericTC<int>.ToString(int value)
            {
                return value.ToString();
            }

            public int Random(Random random)
            {
                return (random.Next() << 10) ^ random.Next();
            }
        }

        private interface INumericTC<T>
        {
            T MinValue { get; }

            T MaxValue { get; }

            T Zero { get; }

            T FromConstantValue(ConstantValue constantValue);

            ConstantValue ToConstantValue(T value);

            bool Related(BinaryOperatorKind relation, T left, T right);

            T Next(T value);

            T Prev(T value);

            T Random(Random random);

            string ToString(T value);
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct LongTC : INumericTC<long>
        {
            long INumericTC<long>.MinValue => long.MinValue;

            long INumericTC<long>.MaxValue => long.MaxValue;

            long INumericTC<long>.Zero => 0L;

            bool INumericTC<long>.Related(BinaryOperatorKind relation, long left, long right)
            {
                return relation switch
                {
                    BinaryOperatorKind.Equal => left == right,
                    BinaryOperatorKind.GreaterThanOrEqual => left >= right,
                    BinaryOperatorKind.GreaterThan => left > right,
                    BinaryOperatorKind.LessThanOrEqual => left <= right,
                    BinaryOperatorKind.LessThan => left < right,
                    _ => throw new ArgumentException("relation"),
                };
            }

            long INumericTC<long>.Next(long value)
            {
                return value + 1;
            }

            long INumericTC<long>.Prev(long value)
            {
                return value - 1;
            }

            long INumericTC<long>.FromConstantValue(ConstantValue constantValue)
            {
                if (!constantValue.IsBad)
                {
                    return constantValue.Int64Value;
                }
                return 0L;
            }

            ConstantValue INumericTC<long>.ToConstantValue(long value)
            {
                return ConstantValue.Create(value);
            }

            string INumericTC<long>.ToString(long value)
            {
                return value.ToString();
            }

            long INumericTC<long>.Random(Random random)
            {
                return ((long)random.Next() << 35) ^ ((long)random.Next() << 10) ^ random.Next();
            }
        }

        private sealed class NintValueSet : IValueSet<int>, IValueSet
        {
            public static readonly NintValueSet AllValues = new NintValueSet(hasSmall: true, NumericValueSet<int, IntTC>.AllValues, hasLarge: true);

            public static readonly NintValueSet NoValues = new NintValueSet(hasSmall: false, NumericValueSet<int, IntTC>.NoValues, hasLarge: false);

            private readonly IValueSet<int> _values;

            private readonly bool _hasSmall;

            private readonly bool _hasLarge;

            public bool IsEmpty
            {
                get
                {
                    if (!_hasSmall && !_hasLarge)
                    {
                        return _values.IsEmpty;
                    }
                    return false;
                }
            }

            ConstantValue? IValueSet.Sample
            {
                get
                {
                    if (IsEmpty)
                    {
                        throw new ArgumentException();
                    }
                    if (!_values.IsEmpty)
                    {
                        return _values.Sample;
                    }
                    return null;
                }
            }

            internal NintValueSet(bool hasSmall, IValueSet<int> values, bool hasLarge)
            {
                _hasSmall = hasSmall;
                _values = values;
                _hasLarge = hasLarge;
            }

            public bool All(BinaryOperatorKind relation, int value)
            {
                bool flag = _hasLarge;
                if (flag)
                {
                    flag = relation switch
                    {
                        BinaryOperatorKind.LessThan => true,
                        BinaryOperatorKind.LessThanOrEqual => true,
                        _ => false,
                    };
                }
                if (flag)
                {
                    return false;
                }
                flag = _hasSmall;
                if (flag)
                {
                    flag = relation switch
                    {
                        BinaryOperatorKind.GreaterThan => true,
                        BinaryOperatorKind.GreaterThanOrEqual => true,
                        _ => false,
                    };
                }
                if (flag)
                {
                    return false;
                }
                return _values.All(relation, value);
            }

            bool IValueSet.All(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return All(relation, value.Int32Value);
                }
                return true;
            }

            public bool Any(BinaryOperatorKind relation, int value)
            {
                bool flag = _hasSmall;
                if (flag)
                {
                    flag = relation switch
                    {
                        BinaryOperatorKind.LessThan => true,
                        BinaryOperatorKind.LessThanOrEqual => true,
                        _ => false,
                    };
                }
                if (flag)
                {
                    return true;
                }
                flag = _hasLarge;
                if (flag)
                {
                    flag = relation switch
                    {
                        BinaryOperatorKind.GreaterThan => true,
                        BinaryOperatorKind.GreaterThanOrEqual => true,
                        _ => false,
                    };
                }
                if (flag)
                {
                    return true;
                }
                return _values.Any(relation, value);
            }

            bool IValueSet.Any(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return Any(relation, value.Int32Value);
                }
                return true;
            }

            public IValueSet<int> Complement()
            {
                return new NintValueSet(!_hasSmall, _values.Complement(), !_hasLarge);
            }

            IValueSet IValueSet.Complement()
            {
                return Complement();
            }

            public IValueSet<int> Intersect(IValueSet<int> o)
            {
                NintValueSet nintValueSet = (NintValueSet)o;
                return new NintValueSet(_hasSmall && nintValueSet._hasSmall, _values.Intersect(nintValueSet._values), _hasLarge && nintValueSet._hasLarge);
            }

            IValueSet IValueSet.Intersect(IValueSet other)
            {
                return Intersect((NintValueSet)other);
            }

            public IValueSet<int> Union(IValueSet<int> o)
            {
                NintValueSet nintValueSet = (NintValueSet)o;
                return new NintValueSet(_hasSmall || nintValueSet._hasSmall, _values.Union(nintValueSet._values), _hasLarge || nintValueSet._hasLarge);
            }

            IValueSet IValueSet.Union(IValueSet other)
            {
                return Union((NintValueSet)other);
            }

            public override bool Equals(object? obj)
            {
                if (obj is NintValueSet nintValueSet && _hasSmall == nintValueSet._hasSmall && _hasLarge == nintValueSet._hasLarge)
                {
                    return _values.Equals(nintValueSet._values);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Hash.Combine(_hasSmall.GetHashCode(), Hash.Combine(_hasLarge.GetHashCode(), _values.GetHashCode()));
            }

            public override string ToString()
            {
                PooledStringBuilder instance = PooledStringBuilder.GetInstance();
                StringBuilder builder = instance.Builder;
                if (_hasSmall)
                {
                    builder.Append("Small");
                }
                if (_hasSmall && !_values.IsEmpty)
                {
                    builder.Append(",");
                }
                builder.Append(_values.ToString());
                if (_hasLarge && builder.Length > 0)
                {
                    builder.Append(",");
                }
                if (_hasLarge)
                {
                    builder.Append("Large");
                }
                return instance.ToStringAndFree();
            }
        }

        private sealed class NintValueSetFactory : IValueSetFactory<int>, IValueSetFactory
        {
            public static readonly NintValueSetFactory Instance = new NintValueSetFactory();

            IValueSet IValueSetFactory.AllValues => NintValueSet.AllValues;

            IValueSet IValueSetFactory.NoValues => NintValueSet.NoValues;

            private NintValueSetFactory()
            {
            }

            public IValueSet<int> Related(BinaryOperatorKind relation, int value)
            {
                bool hasSmall = relation switch
                {
                    BinaryOperatorKind.LessThan => true,
                    BinaryOperatorKind.LessThanOrEqual => true,
                    _ => false,
                };
                IValueSet<int> values = NumericValueSetFactory<int, IntTC>.Instance.Related(relation, value);
                return new NintValueSet(hasSmall, values, relation switch
                {
                    BinaryOperatorKind.GreaterThan => true,
                    BinaryOperatorKind.GreaterThanOrEqual => true,
                    _ => false,
                });
            }

            IValueSet IValueSetFactory.Random(int expectedSize, Random random)
            {
                return new NintValueSet(random.NextDouble() < 0.25, (IValueSet<int>)NumericValueSetFactory<int, IntTC>.Instance.Random(expectedSize, random), random.NextDouble() < 0.25);
            }

            ConstantValue IValueSetFactory.RandomValue(Random random)
            {
                return ConstantValue.CreateNativeInt(default(IntTC).Random(random));
            }

            IValueSet IValueSetFactory.Related(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return Related(relation, default(IntTC).FromConstantValue(value));
                }
                return NintValueSet.AllValues;
            }

            bool IValueSetFactory.Related(BinaryOperatorKind relation, ConstantValue left, ConstantValue right)
            {
                IntTC intTC = default(IntTC);
                return intTC.Related(relation, intTC.FromConstantValue(left), intTC.FromConstantValue(right));
            }
        }

        private sealed class NuintValueSet : IValueSet<uint>, IValueSet
        {
            public static readonly NuintValueSet AllValues = new NuintValueSet(NumericValueSet<uint, UIntTC>.AllValues, hasLarge: true);

            public static readonly NuintValueSet NoValues = new NuintValueSet(NumericValueSet<uint, UIntTC>.NoValues, hasLarge: false);

            private readonly IValueSet<uint> _values;

            private readonly bool _hasLarge;

            public bool IsEmpty
            {
                get
                {
                    if (!_hasLarge)
                    {
                        return _values.IsEmpty;
                    }
                    return false;
                }
            }

            ConstantValue? IValueSet.Sample
            {
                get
                {
                    if (IsEmpty)
                    {
                        throw new ArgumentException();
                    }
                    if (!_values.IsEmpty)
                    {
                        return _values.Sample;
                    }
                    return null;
                }
            }

            internal NuintValueSet(IValueSet<uint> values, bool hasLarge)
            {
                _values = values;
                _hasLarge = hasLarge;
            }

            public bool All(BinaryOperatorKind relation, uint value)
            {
                bool flag = _hasLarge;
                if (flag)
                {
                    flag = relation switch
                    {
                        BinaryOperatorKind.LessThan => true,
                        BinaryOperatorKind.LessThanOrEqual => true,
                        _ => false,
                    };
                }
                if (flag)
                {
                    return false;
                }
                return _values.All(relation, value);
            }

            bool IValueSet.All(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return All(relation, value.UInt32Value);
                }
                return true;
            }

            public bool Any(BinaryOperatorKind relation, uint value)
            {
                bool flag = _hasLarge;
                if (flag)
                {
                    flag = relation switch
                    {
                        BinaryOperatorKind.GreaterThan => true,
                        BinaryOperatorKind.GreaterThanOrEqual => true,
                        _ => false,
                    };
                }
                if (flag)
                {
                    return true;
                }
                return _values.Any(relation, value);
            }

            bool IValueSet.Any(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return Any(relation, value.UInt32Value);
                }
                return true;
            }

            public IValueSet<uint> Complement()
            {
                return new NuintValueSet(_values.Complement(), !_hasLarge);
            }

            IValueSet IValueSet.Complement()
            {
                return Complement();
            }

            public IValueSet<uint> Intersect(IValueSet<uint> o)
            {
                NuintValueSet nuintValueSet = (NuintValueSet)o;
                return new NuintValueSet(_values.Intersect(nuintValueSet._values), _hasLarge && nuintValueSet._hasLarge);
            }

            IValueSet IValueSet.Intersect(IValueSet other)
            {
                return Intersect((NuintValueSet)other);
            }

            public IValueSet<uint> Union(IValueSet<uint> o)
            {
                NuintValueSet nuintValueSet = (NuintValueSet)o;
                return new NuintValueSet(_values.Union(nuintValueSet._values), _hasLarge || nuintValueSet._hasLarge);
            }

            IValueSet IValueSet.Union(IValueSet other)
            {
                return Union((NuintValueSet)other);
            }

            public override bool Equals(object? obj)
            {
                if (obj is NuintValueSet nuintValueSet && _hasLarge == nuintValueSet._hasLarge)
                {
                    return _values.Equals(nuintValueSet._values);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Hash.Combine(_hasLarge.GetHashCode(), _values.GetHashCode());
            }

            public override string ToString()
            {
                PooledStringBuilder instance = PooledStringBuilder.GetInstance();
                StringBuilder builder = instance.Builder;
                builder.Append(_values.ToString());
                if (_hasLarge && builder.Length > 0)
                {
                    builder.Append(",");
                }
                if (_hasLarge)
                {
                    builder.Append("Large");
                }
                return instance.ToStringAndFree();
            }
        }

        private sealed class NuintValueSetFactory : IValueSetFactory<uint>, IValueSetFactory
        {
            public static readonly NuintValueSetFactory Instance = new NuintValueSetFactory();

            IValueSet IValueSetFactory.AllValues => NuintValueSet.AllValues;

            IValueSet IValueSetFactory.NoValues => NuintValueSet.NoValues;

            private NuintValueSetFactory()
            {
            }

            public IValueSet<uint> Related(BinaryOperatorKind relation, uint value)
            {
                IValueSet<uint> values = NumericValueSetFactory<uint, UIntTC>.Instance.Related(relation, value);
                return new NuintValueSet(values, relation switch
                {
                    BinaryOperatorKind.GreaterThan => true,
                    BinaryOperatorKind.GreaterThanOrEqual => true,
                    _ => false,
                });
            }

            IValueSet IValueSetFactory.Random(int expectedSize, Random random)
            {
                return new NuintValueSet((IValueSet<uint>)NumericValueSetFactory<uint, UIntTC>.Instance.Random(expectedSize, random), random.NextDouble() < 0.25);
            }

            ConstantValue IValueSetFactory.RandomValue(Random random)
            {
                return ConstantValue.CreateNativeUInt(default(UIntTC).Random(random));
            }

            IValueSet IValueSetFactory.Related(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return Related(relation, default(UIntTC).FromConstantValue(value));
                }
                return NuintValueSet.AllValues;
            }

            bool IValueSetFactory.Related(BinaryOperatorKind relation, ConstantValue left, ConstantValue right)
            {
                UIntTC uIntTC = default(UIntTC);
                return uIntTC.Related(relation, uIntTC.FromConstantValue(left), uIntTC.FromConstantValue(right));
            }
        }

        private sealed class NumericValueSet<T, TTC> : IValueSet<T>, IValueSet where TTC : struct, INumericTC<T>
        {
            private readonly ImmutableArray<(T first, T last)> _intervals;

            public static readonly NumericValueSet<T, TTC> AllValues = new NumericValueSet<T, TTC>(default(TTC).MinValue, default(TTC).MaxValue);

            public static readonly NumericValueSet<T, TTC> NoValues = new NumericValueSet<T, TTC>(ImmutableArray<(T, T)>.Empty);

            public bool IsEmpty => _intervals.Length == 0;

            ConstantValue IValueSet.Sample
            {
                get
                {
                    if (IsEmpty)
                    {
                        throw new ArgumentException();
                    }
                    TTC val = default(TTC);
                    IValueSet<T> o = NumericValueSetFactory<T, TTC>.Instance.Related(BinaryOperatorKind.GreaterThanOrEqual, val.Zero);
                    NumericValueSet<T, TTC> numericValueSet = (NumericValueSet<T, TTC>)Intersect(o);
                    if (!numericValueSet.IsEmpty)
                    {
                        return val.ToConstantValue(numericValueSet._intervals[0].first);
                    }
                    return val.ToConstantValue(_intervals[_intervals.Length - 1].last);
                }
            }

            internal NumericValueSet(T first, T last)
                : this(ImmutableArray.Create((first, last)))
            {
            }

            internal NumericValueSet(ImmutableArray<(T first, T last)> intervals)
            {
                _intervals = intervals;
            }

            public bool Any(BinaryOperatorKind relation, T value)
            {
                TTC tc = default(TTC);
                switch (relation)
                {
                    case BinaryOperatorKind.LessThan:
                    case BinaryOperatorKind.LessThanOrEqual:
                        if (_intervals.Length > 0)
                        {
                            return tc.Related(relation, _intervals[0].first, value);
                        }
                        return false;
                    case BinaryOperatorKind.GreaterThan:
                    case BinaryOperatorKind.GreaterThanOrEqual:
                        if (_intervals.Length > 0)
                        {
                            return tc.Related(relation, _intervals[_intervals.Length - 1].last, value);
                        }
                        return false;
                    case BinaryOperatorKind.Equal:
                        return anyIntervalContains(0, _intervals.Length - 1, value);
                    default:
                        throw ExceptionUtilities.UnexpectedValue(relation);
                }
                bool anyIntervalContains(int firstIntervalIndex, int lastIntervalIndex, T value)
                {
                    while (true)
                    {
                        if (lastIntervalIndex < firstIntervalIndex)
                        {
                            return false;
                        }
                        if (lastIntervalIndex == firstIntervalIndex)
                        {
                            break;
                        }
                        int num = firstIntervalIndex + (lastIntervalIndex - firstIntervalIndex) / 2;
                        if (tc.Related(BinaryOperatorKind.LessThanOrEqual, value, _intervals[num].last))
                        {
                            lastIntervalIndex = num;
                        }
                        else
                        {
                            firstIntervalIndex = num + 1;
                        }
                    }
                    if (tc.Related(BinaryOperatorKind.GreaterThanOrEqual, value, _intervals[lastIntervalIndex].first))
                    {
                        return tc.Related(BinaryOperatorKind.LessThanOrEqual, value, _intervals[lastIntervalIndex].last);
                    }
                    return false;
                }
            }

            bool IValueSet.Any(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return Any(relation, default(TTC).FromConstantValue(value));
                }
                return true;
            }

            public bool All(BinaryOperatorKind relation, T value)
            {
                if (_intervals.Length == 0)
                {
                    return true;
                }
                TTC val = default(TTC);
                switch (relation)
                {
                    case BinaryOperatorKind.LessThan:
                    case BinaryOperatorKind.LessThanOrEqual:
                        return val.Related(relation, _intervals[_intervals.Length - 1].last, value);
                    case BinaryOperatorKind.GreaterThan:
                    case BinaryOperatorKind.GreaterThanOrEqual:
                        return val.Related(relation, _intervals[0].first, value);
                    case BinaryOperatorKind.Equal:
                        if (_intervals.Length == 1 && val.Related(BinaryOperatorKind.Equal, _intervals[0].first, value))
                        {
                            return val.Related(BinaryOperatorKind.Equal, _intervals[0].last, value);
                        }
                        return false;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(relation);
                }
            }

            bool IValueSet.All(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return All(relation, default(TTC).FromConstantValue(value));
                }
                return false;
            }

            public IValueSet<T> Complement()
            {
                if (_intervals.Length == 0)
                {
                    return AllValues;
                }
                TTC val = default(TTC);
                ArrayBuilder<(T, T)> instance = ArrayBuilder<(T, T)>.GetInstance();
                if (val.Related(BinaryOperatorKind.LessThan, val.MinValue, _intervals[0].first))
                {
                    instance.Add((val.MinValue, val.Prev(_intervals[0].first)));
                }
                int num = _intervals.Length - 1;
                for (int i = 0; i < num; i++)
                {
                    instance.Add((val.Next(_intervals[i].last), val.Prev(_intervals[i + 1].first)));
                }
                if (val.Related(BinaryOperatorKind.LessThan, _intervals[num].last, val.MaxValue))
                {
                    instance.Add((val.Next(_intervals[num].last), val.MaxValue));
                }
                return new NumericValueSet<T, TTC>(instance.ToImmutableAndFree());
            }

            IValueSet IValueSet.Complement()
            {
                return Complement();
            }

            public IValueSet<T> Intersect(IValueSet<T> o)
            {
                NumericValueSet<T, TTC> obj = (NumericValueSet<T, TTC>)o;
                TTC val = default(TTC);
                ArrayBuilder<(T, T)> instance = ArrayBuilder<(T, T)>.GetInstance();
                ImmutableArray<(T, T)> intervals = _intervals;
                ImmutableArray<(T, T)> intervals2 = obj._intervals;
                int num = 0;
                int num2 = 0;
                while (num < intervals.Length && num2 < intervals2.Length)
                {
                    (T, T) tuple = intervals[num];
                    (T, T) tuple2 = intervals2[num2];
                    if (val.Related(BinaryOperatorKind.LessThan, tuple.Item2, tuple2.Item1))
                    {
                        num++;
                        continue;
                    }
                    if (val.Related(BinaryOperatorKind.LessThan, tuple2.Item2, tuple.Item1))
                    {
                        num2++;
                        continue;
                    }
                    Add(instance, Max(tuple.Item1, tuple2.Item1), Min(tuple.Item2, tuple2.Item2));
                    if (val.Related(BinaryOperatorKind.LessThan, tuple.Item2, tuple2.Item2))
                    {
                        num++;
                        continue;
                    }
                    if (val.Related(BinaryOperatorKind.LessThan, tuple2.Item2, tuple.Item2))
                    {
                        num2++;
                        continue;
                    }
                    num++;
                    num2++;
                }
                return new NumericValueSet<T, TTC>(instance.ToImmutableAndFree());
            }

            private static void Add(ArrayBuilder<(T first, T last)> builder, T first, T last)
            {
                TTC val = default(TTC);
                if (builder.Count > 0 && (val.Related(BinaryOperatorKind.Equal, val.MinValue, first) || val.Related(BinaryOperatorKind.GreaterThanOrEqual, builder.Last().last, val.Prev(first))))
                {
                    (T, T) e = builder.Pop();
                    e.Item2 = Max(last, e.Item2);
                    builder.Push(e);
                }
                else
                {
                    builder.Add((first, last));
                }
            }

            private static T Min(T a, T b)
            {
                if (!default(TTC).Related(BinaryOperatorKind.LessThan, a, b))
                {
                    return b;
                }
                return a;
            }

            private static T Max(T a, T b)
            {
                if (!default(TTC).Related(BinaryOperatorKind.LessThan, a, b))
                {
                    return a;
                }
                return b;
            }

            IValueSet IValueSet.Intersect(IValueSet other)
            {
                return Intersect((IValueSet<T>)other);
            }

            public IValueSet<T> Union(IValueSet<T> o)
            {
                NumericValueSet<T, TTC> obj = (NumericValueSet<T, TTC>)o;
                TTC val = default(TTC);
                ArrayBuilder<(T, T)> instance = ArrayBuilder<(T, T)>.GetInstance();
                ImmutableArray<(T, T)> intervals = _intervals;
                ImmutableArray<(T, T)> intervals2 = obj._intervals;
                int i = 0;
                int j = 0;
                while (i < intervals.Length && j < intervals2.Length)
                {
                    (T, T) tuple = intervals[i];
                    (T, T) tuple2 = intervals2[j];
                    if (val.Related(BinaryOperatorKind.LessThan, tuple.Item2, tuple2.Item1))
                    {
                        Add(instance, tuple.Item1, tuple.Item2);
                        i++;
                    }
                    else if (val.Related(BinaryOperatorKind.LessThan, tuple2.Item2, tuple.Item1))
                    {
                        Add(instance, tuple2.Item1, tuple2.Item2);
                        j++;
                    }
                    else
                    {
                        Add(instance, Min(tuple.Item1, tuple2.Item1), Max(tuple.Item2, tuple2.Item2));
                        i++;
                        j++;
                    }
                }
                for (; i < intervals.Length; i++)
                {
                    (T, T) tuple3 = intervals[i];
                    Add(instance, tuple3.Item1, tuple3.Item2);
                }
                for (; j < intervals2.Length; j++)
                {
                    (T, T) tuple4 = intervals2[j];
                    Add(instance, tuple4.Item1, tuple4.Item2);
                }
                return new NumericValueSet<T, TTC>(instance.ToImmutableAndFree());
            }

            IValueSet IValueSet.Union(IValueSet other)
            {
                return Union((IValueSet<T>)other);
            }

            internal static IValueSet<T> Random(int expectedSize, Random random)
            {
                TTC val = default(TTC);
                T[] array = new T[expectedSize * 2];
                int i = 0;
                for (int num = expectedSize * 2; i < num; i++)
                {
                    array[i] = val.Random(random);
                }
                Array.Sort(array);
                ArrayBuilder<(T, T)> instance = ArrayBuilder<(T, T)>.GetInstance();
                int j = 0;
                for (int num2 = array.Length; j < num2; j += 2)
                {
                    T first = array[j];
                    T last = array[j + 1];
                    Add(instance, first, last);
                }
                return new NumericValueSet<T, TTC>(instance.ToImmutableAndFree());
            }

            public override string ToString()
            {
                TTC tc = default(TTC);
                return string.Join(",", _intervals.Select(((T first, T last) p) => "[" + tc.ToString(p.first) + ".." + tc.ToString(p.last) + "]"));
            }

            public override bool Equals(object? obj)
            {
                if (obj is NumericValueSet<T, TTC> numericValueSet)
                {
                    return _intervals.SequenceEqual(numericValueSet._intervals);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Hash.Combine(Hash.CombineValues(_intervals), _intervals.Length);
            }
        }

        private sealed class NumericValueSetFactory<T, TTC> : IValueSetFactory<T>, IValueSetFactory where TTC : struct, INumericTC<T>
        {
            public static readonly NumericValueSetFactory<T, TTC> Instance = new NumericValueSetFactory<T, TTC>();

            IValueSet IValueSetFactory.AllValues => NumericValueSet<T, TTC>.AllValues;

            IValueSet IValueSetFactory.NoValues => NumericValueSet<T, TTC>.NoValues;

            private NumericValueSetFactory()
            {
            }

            public IValueSet<T> Related(BinaryOperatorKind relation, T value)
            {
                TTC val = default(TTC);
                switch (relation)
                {
                    case BinaryOperatorKind.LessThan:
                        if (val.Related(BinaryOperatorKind.LessThanOrEqual, value, val.MinValue))
                        {
                            return NumericValueSet<T, TTC>.NoValues;
                        }
                        return new NumericValueSet<T, TTC>(val.MinValue, val.Prev(value));
                    case BinaryOperatorKind.LessThanOrEqual:
                        return new NumericValueSet<T, TTC>(val.MinValue, value);
                    case BinaryOperatorKind.GreaterThan:
                        if (val.Related(BinaryOperatorKind.GreaterThanOrEqual, value, val.MaxValue))
                        {
                            return NumericValueSet<T, TTC>.NoValues;
                        }
                        return new NumericValueSet<T, TTC>(val.Next(value), val.MaxValue);
                    case BinaryOperatorKind.GreaterThanOrEqual:
                        return new NumericValueSet<T, TTC>(value, val.MaxValue);
                    case BinaryOperatorKind.Equal:
                        return new NumericValueSet<T, TTC>(value, value);
                    default:
                        throw ExceptionUtilities.UnexpectedValue(relation);
                }
            }

            IValueSet IValueSetFactory.Related(BinaryOperatorKind relation, ConstantValue value)
            {
                if (!value.IsBad)
                {
                    return Related(relation, default(TTC).FromConstantValue(value));
                }
                return NumericValueSet<T, TTC>.AllValues;
            }

            public IValueSet Random(int expectedSize, Random random)
            {
                return NumericValueSet<T, TTC>.Random(expectedSize, random);
            }

            ConstantValue IValueSetFactory.RandomValue(Random random)
            {
                TTC val = default(TTC);
                return val.ToConstantValue(val.Random(random));
            }

            bool IValueSetFactory.Related(BinaryOperatorKind relation, ConstantValue left, ConstantValue right)
            {
                TTC val = default(TTC);
                return val.Related(relation, val.FromConstantValue(left), val.FromConstantValue(right));
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct SByteTC : INumericTC<sbyte>
        {
            sbyte INumericTC<sbyte>.MinValue => sbyte.MinValue;

            sbyte INumericTC<sbyte>.MaxValue => sbyte.MaxValue;

            sbyte INumericTC<sbyte>.Zero => 0;

            bool INumericTC<sbyte>.Related(BinaryOperatorKind relation, sbyte left, sbyte right)
            {
                return relation switch
                {
                    BinaryOperatorKind.Equal => left == right,
                    BinaryOperatorKind.GreaterThanOrEqual => left >= right,
                    BinaryOperatorKind.GreaterThan => left > right,
                    BinaryOperatorKind.LessThanOrEqual => left <= right,
                    BinaryOperatorKind.LessThan => left < right,
                    _ => throw new ArgumentException("relation"),
                };
            }

            sbyte INumericTC<sbyte>.Next(sbyte value)
            {
                return (sbyte)(value + 1);
            }

            sbyte INumericTC<sbyte>.Prev(sbyte value)
            {
                return (sbyte)(value - 1);
            }

            sbyte INumericTC<sbyte>.FromConstantValue(ConstantValue constantValue)
            {
                if (!constantValue.IsBad)
                {
                    return constantValue.SByteValue;
                }
                return 0;
            }

            public ConstantValue ToConstantValue(sbyte value)
            {
                return ConstantValue.Create(value);
            }

            string INumericTC<sbyte>.ToString(sbyte value)
            {
                return value.ToString();
            }

            sbyte INumericTC<sbyte>.Random(Random random)
            {
                return (sbyte)random.Next();
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct ShortTC : INumericTC<short>
        {
            short INumericTC<short>.MinValue => short.MinValue;

            short INumericTC<short>.MaxValue => short.MaxValue;

            short INumericTC<short>.Zero => 0;

            bool INumericTC<short>.Related(BinaryOperatorKind relation, short left, short right)
            {
                return relation switch
                {
                    BinaryOperatorKind.Equal => left == right,
                    BinaryOperatorKind.GreaterThanOrEqual => left >= right,
                    BinaryOperatorKind.GreaterThan => left > right,
                    BinaryOperatorKind.LessThanOrEqual => left <= right,
                    BinaryOperatorKind.LessThan => left < right,
                    _ => throw new ArgumentException("relation"),
                };
            }

            short INumericTC<short>.Next(short value)
            {
                return (short)(value + 1);
            }

            short INumericTC<short>.Prev(short value)
            {
                return (short)(value - 1);
            }

            short INumericTC<short>.FromConstantValue(ConstantValue constantValue)
            {
                if (!constantValue.IsBad)
                {
                    return constantValue.Int16Value;
                }
                return 0;
            }

            ConstantValue INumericTC<short>.ToConstantValue(short value)
            {
                return ConstantValue.Create(value);
            }

            string INumericTC<short>.ToString(short value)
            {
                return value.ToString();
            }

            short INumericTC<short>.Random(Random random)
            {
                return (short)random.Next();
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct SingleTC : IFloatingTC<float>, INumericTC<float>
        {
            float INumericTC<float>.MinValue => float.NegativeInfinity;

            float INumericTC<float>.MaxValue => float.PositiveInfinity;

            float IFloatingTC<float>.NaN => float.NaN;

            float INumericTC<float>.Zero => 0f;

            public float Next(float value)
            {
                if (value == 0f)
                {
                    return float.Epsilon;
                }
                if (value < 0f)
                {
                    if (value == -1.401298E-45f)
                    {
                        return 0f;
                    }
                    if (value == float.NegativeInfinity)
                    {
                        return float.MinValue;
                    }
                    return 0f - UintAsFloat(FloatAsUint(0f - value) - 1);
                }
                if (value == float.MaxValue)
                {
                    return float.PositiveInfinity;
                }
                return UintAsFloat(FloatAsUint(value) + 1);
            }

            private unsafe static uint FloatAsUint(float d)
            {
                if (d == 0f)
                {
                    return 0u;
                }
                uint* ptr = (uint*)(&d);
                return *ptr;
            }

            private unsafe static float UintAsFloat(uint l)
            {
                float* ptr = (float*)(&l);
                return *ptr;
            }

            bool INumericTC<float>.Related(BinaryOperatorKind relation, float left, float right)
            {
                switch (relation)
                {
                    case BinaryOperatorKind.Equal:
                        if (left != right)
                        {
                            if (float.IsNaN(left))
                            {
                                return float.IsNaN(right);
                            }
                            return false;
                        }
                        return true;
                    case BinaryOperatorKind.GreaterThanOrEqual:
                        return left >= right;
                    case BinaryOperatorKind.GreaterThan:
                        return left > right;
                    case BinaryOperatorKind.LessThanOrEqual:
                        return left <= right;
                    case BinaryOperatorKind.LessThan:
                        return left < right;
                    default:
                        throw new ArgumentException("relation");
                }
            }

            float INumericTC<float>.FromConstantValue(ConstantValue constantValue)
            {
                if (!constantValue.IsBad)
                {
                    return constantValue.SingleValue;
                }
                return 0f;
            }

            ConstantValue INumericTC<float>.ToConstantValue(float value)
            {
                return ConstantValue.Create(value);
            }

            string INumericTC<float>.ToString(float value)
            {
                if (!float.IsNaN(value))
                {
                    if (value != float.NegativeInfinity)
                    {
                        if (value != float.PositiveInfinity)
                        {
                            return $"{value:G9}";
                        }
                        return "Inf";
                    }
                    return "-Inf";
                }
                return "NaN";
            }

            float INumericTC<float>.Prev(float value)
            {
                return 0f - Next(0f - value);
            }

            float INumericTC<float>.Random(Random random)
            {
                return (float)(random.NextDouble() * 100.0 - 50.0);
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct StringTC : IEquatableValueTC<string>
        {
            string IEquatableValueTC<string>.FromConstantValue(ConstantValue constantValue)
            {
                if (!constantValue.IsBad)
                {
                    return constantValue.StringValue;
                }
                return string.Empty;
            }

            string[] IEquatableValueTC<string>.RandomValues(int count, Random random, int scope)
            {
                string[] array = new string[count];
                int num = 0;
                for (int i = 0; i < scope; i++)
                {
                    int num2 = count - num;
                    int num3 = scope - i;
                    if (random.NextDouble() * num3 < num2)
                    {
                        array[num++] = i.ToString();
                    }
                }
                return array;
            }

            ConstantValue IEquatableValueTC<string>.ToConstantValue(string value)
            {
                return ConstantValue.Create(value);
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct UIntTC : INumericTC<uint>
        {
            uint INumericTC<uint>.MinValue => 0u;

            uint INumericTC<uint>.MaxValue => uint.MaxValue;

            uint INumericTC<uint>.Zero => 0u;

            public bool Related(BinaryOperatorKind relation, uint left, uint right)
            {
                return relation switch
                {
                    BinaryOperatorKind.Equal => left == right,
                    BinaryOperatorKind.GreaterThanOrEqual => left >= right,
                    BinaryOperatorKind.GreaterThan => left > right,
                    BinaryOperatorKind.LessThanOrEqual => left <= right,
                    BinaryOperatorKind.LessThan => left < right,
                    _ => throw new ArgumentException("relation"),
                };
            }

            uint INumericTC<uint>.Next(uint value)
            {
                return value + 1;
            }

            public uint FromConstantValue(ConstantValue constantValue)
            {
                if (!constantValue.IsBad)
                {
                    return constantValue.UInt32Value;
                }
                return 0u;
            }

            public ConstantValue ToConstantValue(uint value)
            {
                return ConstantValue.Create(value);
            }

            string INumericTC<uint>.ToString(uint value)
            {
                return value.ToString();
            }

            uint INumericTC<uint>.Prev(uint value)
            {
                return value - 1;
            }

            public uint Random(Random random)
            {
                return (uint)((random.Next() << 10) ^ random.Next());
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct ULongTC : INumericTC<ulong>
        {
            ulong INumericTC<ulong>.MinValue => 0uL;

            ulong INumericTC<ulong>.MaxValue => ulong.MaxValue;

            ulong INumericTC<ulong>.Zero => 0uL;

            bool INumericTC<ulong>.Related(BinaryOperatorKind relation, ulong left, ulong right)
            {
                return relation switch
                {
                    BinaryOperatorKind.Equal => left == right,
                    BinaryOperatorKind.GreaterThanOrEqual => left >= right,
                    BinaryOperatorKind.GreaterThan => left > right,
                    BinaryOperatorKind.LessThanOrEqual => left <= right,
                    BinaryOperatorKind.LessThan => left < right,
                    _ => throw new ArgumentException("relation"),
                };
            }

            ulong INumericTC<ulong>.Next(ulong value)
            {
                return value + 1;
            }

            ulong INumericTC<ulong>.Prev(ulong value)
            {
                return value - 1;
            }

            ulong INumericTC<ulong>.FromConstantValue(ConstantValue constantValue)
            {
                if (!constantValue.IsBad)
                {
                    return constantValue.UInt64Value;
                }
                return 0uL;
            }

            ConstantValue INumericTC<ulong>.ToConstantValue(ulong value)
            {
                return ConstantValue.Create(value);
            }

            string INumericTC<ulong>.ToString(ulong value)
            {
                return value.ToString();
            }

            ulong INumericTC<ulong>.Random(Random random)
            {
                return (ulong)(((long)random.Next() << 35) ^ ((long)random.Next() << 10) ^ random.Next());
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct UShortTC : INumericTC<ushort>
        {
            ushort INumericTC<ushort>.MinValue => 0;

            ushort INumericTC<ushort>.MaxValue => ushort.MaxValue;

            ushort INumericTC<ushort>.Zero => 0;

            bool INumericTC<ushort>.Related(BinaryOperatorKind relation, ushort left, ushort right)
            {
                return relation switch
                {
                    BinaryOperatorKind.Equal => left == right,
                    BinaryOperatorKind.GreaterThanOrEqual => left >= right,
                    BinaryOperatorKind.GreaterThan => left > right,
                    BinaryOperatorKind.LessThanOrEqual => left <= right,
                    BinaryOperatorKind.LessThan => left < right,
                    _ => throw new ArgumentException("relation"),
                };
            }

            ushort INumericTC<ushort>.Next(ushort value)
            {
                return (ushort)(value + 1);
            }

            ushort INumericTC<ushort>.FromConstantValue(ConstantValue constantValue)
            {
                if (!constantValue.IsBad)
                {
                    return constantValue.UInt16Value;
                }
                return 0;
            }

            ConstantValue INumericTC<ushort>.ToConstantValue(ushort value)
            {
                return ConstantValue.Create(value);
            }

            string INumericTC<ushort>.ToString(ushort value)
            {
                return value.ToString();
            }

            ushort INumericTC<ushort>.Prev(ushort value)
            {
                return (ushort)(value - 1);
            }

            ushort INumericTC<ushort>.Random(Random random)
            {
                return (ushort)random.Next();
            }
        }

        internal static readonly IValueSetFactory<byte> ForByte = NumericValueSetFactory<byte, ByteTC>.Instance;

        internal static readonly IValueSetFactory<sbyte> ForSByte = NumericValueSetFactory<sbyte, SByteTC>.Instance;

        internal static readonly IValueSetFactory<char> ForChar = NumericValueSetFactory<char, CharTC>.Instance;

        internal static readonly IValueSetFactory<short> ForShort = NumericValueSetFactory<short, ShortTC>.Instance;

        internal static readonly IValueSetFactory<ushort> ForUShort = NumericValueSetFactory<ushort, UShortTC>.Instance;

        internal static readonly IValueSetFactory<int> ForInt = NumericValueSetFactory<int, IntTC>.Instance;

        internal static readonly IValueSetFactory<uint> ForUInt = NumericValueSetFactory<uint, UIntTC>.Instance;

        internal static readonly IValueSetFactory<long> ForLong = NumericValueSetFactory<long, LongTC>.Instance;

        internal static readonly IValueSetFactory<ulong> ForULong = NumericValueSetFactory<ulong, ULongTC>.Instance;

        internal static readonly IValueSetFactory<bool> ForBool = BoolValueSetFactory.Instance;

        internal static readonly IValueSetFactory<float> ForFloat = FloatingValueSetFactory<float, SingleTC>.Instance;

        internal static readonly IValueSetFactory<double> ForDouble = FloatingValueSetFactory<double, DoubleTC>.Instance;

        internal static readonly IValueSetFactory<string> ForString = EnumeratedValueSetFactory<string, StringTC>.Instance;

        internal static readonly IValueSetFactory<decimal> ForDecimal = DecimalValueSetFactory.Instance;

        internal static readonly IValueSetFactory<int> ForNint = NintValueSetFactory.Instance;

        internal static readonly IValueSetFactory<uint> ForNuint = NuintValueSetFactory.Instance;

        public static IValueSetFactory? ForSpecialType(SpecialType specialType, bool isNative = false)
        {
            switch (specialType)
            {
                case SpecialType.System_Byte:
                    return ForByte;
                case SpecialType.System_SByte:
                    return ForSByte;
                case SpecialType.System_Char:
                    return ForChar;
                case SpecialType.System_Int16:
                    return ForShort;
                case SpecialType.System_UInt16:
                    return ForUShort;
                case SpecialType.System_Int32:
                    return ForInt;
                case SpecialType.System_UInt32:
                    return ForUInt;
                case SpecialType.System_Int64:
                    return ForLong;
                case SpecialType.System_UInt64:
                    return ForULong;
                case SpecialType.System_Boolean:
                    return ForBool;
                case SpecialType.System_Single:
                    return ForFloat;
                case SpecialType.System_Double:
                    return ForDouble;
                case SpecialType.System_String:
                    return ForString;
                case SpecialType.System_Decimal:
                    return ForDecimal;
                case SpecialType.System_IntPtr:
                    if (isNative)
                    {
                        return ForNint;
                    }
                    break;
                case SpecialType.System_UIntPtr:
                    if (isNative)
                    {
                        return ForNuint;
                    }
                    break;
            }
            return null;
        }

        public static IValueSetFactory? ForType(TypeSymbol type)
        {
            type = type.EnumUnderlyingTypeOrSelf();
            return ForSpecialType(type.SpecialType, type.IsNativeIntegerType);
        }
    }
}
