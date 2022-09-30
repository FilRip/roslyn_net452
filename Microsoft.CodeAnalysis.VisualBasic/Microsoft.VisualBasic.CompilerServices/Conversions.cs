using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualBasic.CompilerServices
{
	[Embedded]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class Conversions
	{
		private Conversions()
		{
		}

		private static object GetEnumValue(object Value)
		{
			Type underlyingType = Enum.GetUnderlyingType(Value.GetType());
			if (underlyingType.Equals(typeof(sbyte)))
			{
				return (sbyte)Value;
			}
			if (underlyingType.Equals(typeof(byte)))
			{
				return (byte)Value;
			}
			if (underlyingType.Equals(typeof(short)))
			{
				return (short)Value;
			}
			if (underlyingType.Equals(typeof(ushort)))
			{
				return (ushort)Value;
			}
			if (underlyingType.Equals(typeof(int)))
			{
				return (int)Value;
			}
			if (underlyingType.Equals(typeof(uint)))
			{
				return (uint)Value;
			}
			if (underlyingType.Equals(typeof(long)))
			{
				return (long)Value;
			}
			if (underlyingType.Equals(typeof(ulong)))
			{
				return (ulong)Value;
			}
			throw new InvalidCastException();
		}

		public static bool ToBoolean(string Value)
		{
			if (Value == null)
			{
				Value = "";
			}
			try
			{
				CultureInfo cultureInfo = GetCultureInfo();
				if (cultureInfo.CompareInfo.Compare(Value, bool.FalseString, CompareOptions.IgnoreCase) == 0)
				{
					return false;
				}
				if (cultureInfo.CompareInfo.Compare(Value, bool.TrueString, CompareOptions.IgnoreCase) == 0)
				{
					return true;
				}
				long i64Value = default(long);
				if (IsHexOrOctValue(Value, ref i64Value))
				{
					return i64Value != 0;
				}
				return ParseDouble(Value) != 0.0;
			}
			catch (FormatException ex)
			{
				ProjectData.SetProjectError(ex);
				FormatException ex2 = ex;
				throw new InvalidCastException(ex2.Message, ex2);
			}
		}

		public static bool ToBoolean(object Value)
		{
			if (Value == null)
			{
				return false;
			}
			if (Value is Enum)
			{
				Value = RuntimeHelpers.GetObjectValue(GetEnumValue(RuntimeHelpers.GetObjectValue(Value)));
			}
			if (!(Value is bool result))
			{
				if (Value is sbyte)
				{
					return (sbyte)Value != 0;
				}
				if (Value is byte)
				{
					return (byte)Value != 0;
				}
				if (Value is short)
				{
					return (short)Value != 0;
				}
				if (Value is ushort)
				{
					return (ushort)Value != 0;
				}
				if (Value is int)
				{
					return (int)Value != 0;
				}
				if (Value is uint)
				{
					return (uint)Value != 0;
				}
				if (Value is long)
				{
					return (long)Value != 0;
				}
				if (Value is ulong)
				{
					return (ulong)Value != 0;
				}
				if (Value is decimal)
				{
					return Convert.ToBoolean((decimal)Value);
				}
				if (Value is float)
				{
					return (float)Value != 0f;
				}
				if (Value is double)
				{
					return (double)Value != 0.0;
				}
				if (Value is string)
				{
					return ToBoolean((string)Value);
				}
				throw new InvalidCastException();
			}
			return result;
		}

		public static byte ToByte(string Value)
		{
			if (Value == null)
			{
				return 0;
			}
			try
			{
				long i64Value = default(long);
				if (IsHexOrOctValue(Value, ref i64Value))
				{
					return (byte)i64Value;
				}
				return (byte)Math.Round(ParseDouble(Value));
			}
			catch (FormatException ex)
			{
				ProjectData.SetProjectError(ex);
				FormatException ex2 = ex;
				throw new InvalidCastException(ex2.Message, ex2);
			}
		}

		public static byte ToByte(object Value)
		{
			if (Value == null)
			{
				return 0;
			}
			if (Value is Enum)
			{
				Value = RuntimeHelpers.GetObjectValue(GetEnumValue(RuntimeHelpers.GetObjectValue(Value)));
			}
			if (Value is bool)
			{
				return (byte)(0u - (((bool)Value) ? 1u : 0u));
			}
			if (Value is sbyte)
			{
				return (byte)(sbyte)Value;
			}
			if (!(Value is byte result))
			{
				if (Value is short)
				{
					return (byte)(short)Value;
				}
				if (Value is ushort)
				{
					return (byte)(ushort)Value;
				}
				if (Value is int)
				{
					return (byte)(int)Value;
				}
				if (Value is uint)
				{
					return (byte)(uint)Value;
				}
				if (Value is long)
				{
					return (byte)(long)Value;
				}
				if (Value is ulong)
				{
					return (byte)(ulong)Value;
				}
				if (Value is decimal)
				{
					return Convert.ToByte((decimal)Value);
				}
				if (Value is float)
				{
					return (byte)Math.Round((float)Value);
				}
				if (Value is double)
				{
					return (byte)Math.Round((double)Value);
				}
				if (Value is string)
				{
					return ToByte((string)Value);
				}
				throw new InvalidCastException();
			}
			return result;
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(string Value)
		{
			if (Value == null)
			{
				return 0;
			}
			try
			{
				long i64Value = default(long);
				if (IsHexOrOctValue(Value, ref i64Value))
				{
					return (sbyte)i64Value;
				}
				return (sbyte)Math.Round(ParseDouble(Value));
			}
			catch (FormatException ex)
			{
				ProjectData.SetProjectError(ex);
				FormatException ex2 = ex;
				throw new InvalidCastException(ex2.Message, ex2);
			}
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(object Value)
		{
			if (Value == null)
			{
				return 0;
			}
			if (Value is Enum)
			{
				Value = RuntimeHelpers.GetObjectValue(GetEnumValue(RuntimeHelpers.GetObjectValue(Value)));
			}
			if (Value is bool)
			{
				return (sbyte)(0 - (((bool)Value) ? 1 : 0));
			}
			if (!(Value is sbyte result))
			{
				if (Value is byte)
				{
					return (sbyte)(byte)Value;
				}
				if (Value is short)
				{
					return (sbyte)(short)Value;
				}
				if (Value is ushort)
				{
					return (sbyte)(ushort)Value;
				}
				if (Value is int)
				{
					return (sbyte)(int)Value;
				}
				if (Value is uint)
				{
					return (sbyte)(uint)Value;
				}
				if (Value is long)
				{
					return (sbyte)(long)Value;
				}
				if (Value is ulong)
				{
					return (sbyte)(ulong)Value;
				}
				if (Value is decimal)
				{
					return Convert.ToSByte((decimal)Value);
				}
				if (Value is float)
				{
					return (sbyte)Math.Round((float)Value);
				}
				if (Value is double)
				{
					return (sbyte)Math.Round((double)Value);
				}
				if (Value is string)
				{
					return ToSByte((string)Value);
				}
				throw new InvalidCastException();
			}
			return result;
		}

		public static short ToShort(string Value)
		{
			if (Value == null)
			{
				return 0;
			}
			try
			{
				long i64Value = default(long);
				if (IsHexOrOctValue(Value, ref i64Value))
				{
					return (short)i64Value;
				}
				return (short)Math.Round(ParseDouble(Value));
			}
			catch (FormatException ex)
			{
				ProjectData.SetProjectError(ex);
				FormatException ex2 = ex;
				throw new InvalidCastException(ex2.Message, ex2);
			}
		}

		public static short ToShort(object Value)
		{
			if (Value == null)
			{
				return 0;
			}
			if (Value is Enum)
			{
				Value = RuntimeHelpers.GetObjectValue(GetEnumValue(RuntimeHelpers.GetObjectValue(Value)));
			}
			if (Value is bool)
			{
				return (short)(0 - (((bool)Value) ? 1 : 0));
			}
			if (!(Value is short result))
			{
				if (!(Value is short result2))
				{
					if (!(Value is short result3))
					{
						if (Value is ushort)
						{
							return (short)(ushort)Value;
						}
						if (Value is int)
						{
							return (short)(int)Value;
						}
						if (Value is uint)
						{
							return (short)(uint)Value;
						}
						if (Value is long)
						{
							return (short)(long)Value;
						}
						if (Value is ulong)
						{
							return (short)(ulong)Value;
						}
						if (Value is decimal)
						{
							return Convert.ToInt16((decimal)Value);
						}
						if (Value is float)
						{
							return (short)Math.Round((float)Value);
						}
						if (Value is double)
						{
							return (short)Math.Round((double)Value);
						}
						if (Value is string)
						{
							return ToShort((string)Value);
						}
						throw new InvalidCastException();
					}
					return result3;
				}
				return result2;
			}
			return result;
		}

		[CLSCompliant(false)]
		public static ushort ToUShort(string Value)
		{
			if (Value == null)
			{
				return 0;
			}
			try
			{
				long i64Value = default(long);
				if (IsHexOrOctValue(Value, ref i64Value))
				{
					return (ushort)i64Value;
				}
				return (ushort)Math.Round(ParseDouble(Value));
			}
			catch (FormatException ex)
			{
				ProjectData.SetProjectError(ex);
				FormatException ex2 = ex;
				throw new InvalidCastException(ex2.Message, ex2);
			}
		}

		[CLSCompliant(false)]
		public static ushort ToUShort(object Value)
		{
			if (Value == null)
			{
				return 0;
			}
			if (Value is Enum)
			{
				Value = RuntimeHelpers.GetObjectValue(GetEnumValue(RuntimeHelpers.GetObjectValue(Value)));
			}
			if (Value is bool)
			{
				return (ushort)(0u - (((bool)Value) ? 1u : 0u));
			}
			if (Value is sbyte)
			{
				return (ushort)(sbyte)Value;
			}
			if (!(Value is ushort result))
			{
				if (Value is short)
				{
					return (ushort)(short)Value;
				}
				if (!(Value is ushort result2))
				{
					if (Value is int)
					{
						return (ushort)(int)Value;
					}
					if (Value is uint)
					{
						return (ushort)(uint)Value;
					}
					if (Value is long)
					{
						return (ushort)(long)Value;
					}
					if (Value is ulong)
					{
						return (ushort)(ulong)Value;
					}
					if (Value is decimal)
					{
						return Convert.ToUInt16((decimal)Value);
					}
					if (Value is float)
					{
						return (ushort)Math.Round((float)Value);
					}
					if (Value is double)
					{
						return (ushort)Math.Round((double)Value);
					}
					if (Value is string)
					{
						return ToUShort((string)Value);
					}
					throw new InvalidCastException();
				}
				return result2;
			}
			return result;
		}

		public static int ToInteger(string Value)
		{
			if (Value == null)
			{
				return 0;
			}
			try
			{
				long i64Value = default(long);
				if (IsHexOrOctValue(Value, ref i64Value))
				{
					return (int)i64Value;
				}
				return (int)Math.Round(ParseDouble(Value));
			}
			catch (FormatException ex)
			{
				ProjectData.SetProjectError(ex);
				FormatException ex2 = ex;
				throw new InvalidCastException(ex2.Message, ex2);
			}
		}

		public static int ToInteger(object Value)
		{
			if (Value == null)
			{
				return 0;
			}
			if (Value is Enum)
			{
				Value = RuntimeHelpers.GetObjectValue(GetEnumValue(RuntimeHelpers.GetObjectValue(Value)));
			}
			if (Value is bool)
			{
				return 0 - (((bool)Value) ? 1 : 0);
			}
			if (!(Value is int result))
			{
				if (!(Value is int result2))
				{
					if (!(Value is int result3))
					{
						if (!(Value is int result4))
						{
							if (!(Value is int result5))
							{
								if (!(Value is int result6))
								{
									if (Value is long)
									{
										return (int)(long)Value;
									}
									if (Value is ulong)
									{
										return (int)(ulong)Value;
									}
									if (Value is decimal)
									{
										return Convert.ToInt32((decimal)Value);
									}
									if (Value is float)
									{
										return (int)Math.Round((float)Value);
									}
									if (Value is double)
									{
										return (int)Math.Round((double)Value);
									}
									if (Value is string)
									{
										return ToInteger((string)Value);
									}
									throw new InvalidCastException();
								}
								return result6;
							}
							return result5;
						}
						return result4;
					}
					return result3;
				}
				return result2;
			}
			return result;
		}

		[CLSCompliant(false)]
		public static uint ToUInteger(string Value)
		{
			if (Value == null)
			{
				return 0u;
			}
			try
			{
				long i64Value = default(long);
				if (IsHexOrOctValue(Value, ref i64Value))
				{
					return (uint)i64Value;
				}
				return (uint)Math.Round(ParseDouble(Value));
			}
			catch (FormatException ex)
			{
				ProjectData.SetProjectError(ex);
				FormatException ex2 = ex;
				throw new InvalidCastException(ex2.Message, ex2);
			}
		}

		[CLSCompliant(false)]
		public static uint ToUInteger(object Value)
		{
			if (Value == null)
			{
				return 0u;
			}
			if (Value is Enum)
			{
				Value = RuntimeHelpers.GetObjectValue(GetEnumValue(RuntimeHelpers.GetObjectValue(Value)));
			}
			if (Value is bool)
			{
				return 0u - (((bool)Value) ? 1u : 0u);
			}
			if (!(Value is uint result))
			{
				if (!(Value is uint result2))
				{
					if (!(Value is uint result3))
					{
						if (!(Value is uint result4))
						{
							if (!(Value is uint result5))
							{
								if (!(Value is uint result6))
								{
									if (Value is long)
									{
										return (uint)(long)Value;
									}
									if (Value is ulong)
									{
										return (uint)(ulong)Value;
									}
									if (Value is decimal)
									{
										return Convert.ToUInt32((decimal)Value);
									}
									if (Value is float)
									{
										return (uint)Math.Round((float)Value);
									}
									if (Value is double)
									{
										return (uint)Math.Round((double)Value);
									}
									if (Value is string)
									{
										return ToUInteger((string)Value);
									}
									throw new InvalidCastException();
								}
								return result6;
							}
							return result5;
						}
						return result4;
					}
					return result3;
				}
				return result2;
			}
			return result;
		}

		public static long ToLong(string Value)
		{
			if (Value == null)
			{
				return 0L;
			}
			try
			{
				long i64Value = default(long);
				if (IsHexOrOctValue(Value, ref i64Value))
				{
					return i64Value;
				}
				return Convert.ToInt64(ParseDecimal(Value, null));
			}
			catch (FormatException ex)
			{
				ProjectData.SetProjectError(ex);
				FormatException ex2 = ex;
				throw new InvalidCastException(ex2.Message, ex2);
			}
		}

		public static long ToLong(object Value)
		{
			if (Value == null)
			{
				return 0L;
			}
			if (Value is Enum)
			{
				Value = RuntimeHelpers.GetObjectValue(GetEnumValue(RuntimeHelpers.GetObjectValue(Value)));
			}
			if (Value is bool)
			{
				return 0 - (((bool)Value) ? 1 : 0);
			}
			if (Value is sbyte)
			{
				return (sbyte)Value;
			}
			if (Value is byte)
			{
				return (byte)Value;
			}
			if (Value is short)
			{
				return (short)Value;
			}
			if (Value is ushort)
			{
				return (ushort)Value;
			}
			if (Value is int)
			{
				return (int)Value;
			}
			if (Value is uint)
			{
				return (uint)Value;
			}
			if (!(Value is long result))
			{
				if (!(Value is long result2))
				{
					if (Value is decimal)
					{
						return Convert.ToInt64((decimal)Value);
					}
					if (Value is float)
					{
						return (long)Math.Round((float)Value);
					}
					if (Value is double)
					{
						return (long)Math.Round((double)Value);
					}
					if (Value is string)
					{
						return ToLong((string)Value);
					}
					throw new InvalidCastException();
				}
				return result2;
			}
			return result;
		}

		[CLSCompliant(false)]
		public static ulong ToULong(string Value)
		{
			if (Value == null)
			{
				return 0uL;
			}
			try
			{
				ulong ui64Value = default(ulong);
				if (IsHexOrOctValue(Value, ref ui64Value))
				{
					return ui64Value;
				}
				return Convert.ToUInt64(ParseDecimal(Value, null));
			}
			catch (FormatException ex)
			{
				ProjectData.SetProjectError(ex);
				FormatException ex2 = ex;
				throw new InvalidCastException(ex2.Message, ex2);
			}
		}

		[CLSCompliant(false)]
		public static ulong ToULong(object Value)
		{
			if (Value == null)
			{
				return 0uL;
			}
			if (Value is Enum)
			{
				Value = RuntimeHelpers.GetObjectValue(GetEnumValue(RuntimeHelpers.GetObjectValue(Value)));
			}
			if (Value is bool)
			{
				return (ulong)(int)(0u - (((bool)Value) ? 1u : 0u));
			}
			if (Value is sbyte)
			{
				return (ulong)(sbyte)Value;
			}
			if (Value is byte)
			{
				return (byte)Value;
			}
			if (Value is short)
			{
				return (ulong)(short)Value;
			}
			if (Value is ushort)
			{
				return (ushort)Value;
			}
			if (Value is int)
			{
				return (ulong)(int)Value;
			}
			if (Value is uint)
			{
				return (uint)Value;
			}
			if (!(Value is ulong result))
			{
				if (!(Value is ulong result2))
				{
					if (Value is decimal)
					{
						return Convert.ToUInt64((decimal)Value);
					}
					if (Value is float)
					{
						return (ulong)Math.Round((float)Value);
					}
					if (Value is double)
					{
						return (ulong)Math.Round((double)Value);
					}
					if (Value is string)
					{
						return ToULong((string)Value);
					}
					throw new InvalidCastException();
				}
				return result2;
			}
			return result;
		}

		public static decimal ToDecimal(bool Value)
		{
			if (Value)
			{
				return -1m;
			}
			return default(decimal);
		}

		public static decimal ToDecimal(string Value)
		{
			if (Value == null)
			{
				return default(decimal);
			}
			try
			{
				long i64Value = default(long);
				if (IsHexOrOctValue(Value, ref i64Value))
				{
					return new decimal(i64Value);
				}
				return ParseDecimal(Value, null);
			}
			catch (OverflowException ex)
			{
				ProjectData.SetProjectError(ex);
				OverflowException ex2 = ex;
				throw ex2;
			}
			catch (FormatException ex3)
			{
				ProjectData.SetProjectError(ex3);
				FormatException ex4 = ex3;
				throw new InvalidCastException(ex4.Message, ex4);
			}
		}

		public static decimal ToDecimal(object Value)
		{
			decimal result;
			if (Value == null)
			{
				result = default(decimal);
			}
			else
			{
				if (Value is Enum)
				{
					Value = RuntimeHelpers.GetObjectValue(GetEnumValue(RuntimeHelpers.GetObjectValue(Value)));
				}
				if (Value is bool)
				{
					return ToDecimal((bool)Value);
				}
				if (Value is sbyte)
				{
					result = new decimal((sbyte)Value);
				}
				else if (Value is byte)
				{
					result = new decimal((byte)Value);
				}
				else if (Value is short)
				{
					result = new decimal((short)Value);
				}
				else if (Value is ushort)
				{
					result = new decimal((ushort)Value);
				}
				else if (Value is int)
				{
					result = new decimal((int)Value);
				}
				else if (Value is uint)
				{
					result = new decimal((uint)Value);
				}
				else if (Value is long)
				{
					result = new decimal((long)Value);
				}
				else if (Value is ulong)
				{
					result = new decimal((ulong)Value);
				}
				else
				{
					if (Value is decimal result2)
					{
						return result2;
					}
					if (Value is float)
					{
						result = new decimal((float)Value);
					}
					else
					{
						if (!(Value is double))
						{
							if (Value is string)
							{
								return ToDecimal((string)Value);
							}
							throw new InvalidCastException();
						}
						result = new decimal((double)Value);
					}
				}
			}
			return result;
		}

		private static decimal ParseDecimal(string Value, NumberFormatInfo NumberFormat)
		{
			CultureInfo cultureInfo = GetCultureInfo();
			if (NumberFormat == null)
			{
				NumberFormat = cultureInfo.NumberFormat;
			}
			NumberFormatInfo normalizedNumberFormat = GetNormalizedNumberFormat(NumberFormat);
			Value = ToHalfwidthNumbers(Value, cultureInfo);
			try
			{
				return decimal.Parse(Value, NumberStyles.Any, normalizedNumberFormat);
			}
			catch (FormatException projectError) when (((Func<bool>)delegate
			{
				// Could not convert BlockContainer to single expression
				ProjectData.SetProjectError(projectError);
				return NumberFormat != normalizedNumberFormat;
			}).Invoke())
			{
				decimal result = decimal.Parse(Value, NumberStyles.Any, NumberFormat);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				throw ex2;
			}
		}

		private static NumberFormatInfo GetNormalizedNumberFormat(NumberFormatInfo InNumberFormat)
		{
			NumberFormatInfo numberFormatInfo = InNumberFormat;
			if (numberFormatInfo.CurrencyDecimalSeparator != null && numberFormatInfo.NumberDecimalSeparator != null && numberFormatInfo.CurrencyGroupSeparator != null && numberFormatInfo.NumberGroupSeparator != null && numberFormatInfo.CurrencyDecimalSeparator.Length == 1 && numberFormatInfo.NumberDecimalSeparator.Length == 1 && numberFormatInfo.CurrencyGroupSeparator.Length == 1 && numberFormatInfo.NumberGroupSeparator.Length == 1 && numberFormatInfo.CurrencyDecimalSeparator[0] == numberFormatInfo.NumberDecimalSeparator[0] && numberFormatInfo.CurrencyGroupSeparator[0] == numberFormatInfo.NumberGroupSeparator[0] && numberFormatInfo.CurrencyDecimalDigits == numberFormatInfo.NumberDecimalDigits)
			{
				return InNumberFormat;
			}
			numberFormatInfo = null;
			NumberFormatInfo numberFormatInfo2 = InNumberFormat;
			if (numberFormatInfo2.CurrencyDecimalSeparator != null && numberFormatInfo2.NumberDecimalSeparator != null && numberFormatInfo2.CurrencyDecimalSeparator.Length == numberFormatInfo2.NumberDecimalSeparator.Length && numberFormatInfo2.CurrencyGroupSeparator != null && numberFormatInfo2.NumberGroupSeparator != null && numberFormatInfo2.CurrencyGroupSeparator.Length == numberFormatInfo2.NumberGroupSeparator.Length)
			{
				int num = numberFormatInfo2.CurrencyDecimalSeparator.Length - 1;
				int num2 = 0;
				while (true)
				{
					if (num2 <= num)
					{
						if (numberFormatInfo2.CurrencyDecimalSeparator[num2] != numberFormatInfo2.NumberDecimalSeparator[num2])
						{
							break;
						}
						num2++;
						continue;
					}
					int num3 = numberFormatInfo2.CurrencyGroupSeparator.Length - 1;
					num2 = 0;
					while (true)
					{
						if (num2 <= num3)
						{
							if (numberFormatInfo2.CurrencyGroupSeparator[num2] != numberFormatInfo2.NumberGroupSeparator[num2])
							{
								break;
							}
							num2++;
							continue;
						}
						return InNumberFormat;
					}
					break;
				}
			}
			else
			{
				numberFormatInfo2 = null;
			}
			NumberFormatInfo obj = (NumberFormatInfo)InNumberFormat.Clone();
			obj.CurrencyDecimalSeparator = obj.NumberDecimalSeparator;
			obj.CurrencyGroupSeparator = obj.NumberGroupSeparator;
			obj.CurrencyDecimalDigits = obj.NumberDecimalDigits;
			_ = null;
			return obj;
		}

		public static float ToSingle(string Value)
		{
			if (Value == null)
			{
				return 0f;
			}
			try
			{
				long i64Value = default(long);
				if (IsHexOrOctValue(Value, ref i64Value))
				{
					return i64Value;
				}
				double num = ParseDouble(Value);
				if ((num < -3.4028234663852886E+38 || num > 3.4028234663852886E+38) && !double.IsInfinity(num))
				{
					throw new OverflowException();
				}
				return (float)num;
			}
			catch (FormatException ex)
			{
				ProjectData.SetProjectError(ex);
				FormatException ex2 = ex;
				throw new InvalidCastException(ex2.Message, ex2);
			}
		}

		public static float ToSingle(object Value)
		{
			if (Value == null)
			{
				return 0f;
			}
			if (Value is Enum)
			{
				Value = RuntimeHelpers.GetObjectValue(GetEnumValue(RuntimeHelpers.GetObjectValue(Value)));
			}
			if (Value is bool)
			{
				return 0 - (((bool)Value) ? 1 : 0);
			}
			if (Value is sbyte)
			{
				return (sbyte)Value;
			}
			if (Value is byte)
			{
				return (int)(byte)Value;
			}
			if (Value is short)
			{
				return (short)Value;
			}
			if (Value is ushort)
			{
				return (int)(ushort)Value;
			}
			if (Value is int)
			{
				return (int)Value;
			}
			if (Value is uint)
			{
				return (uint)Value;
			}
			if (Value is long)
			{
				return (long)Value;
			}
			if (Value is ulong)
			{
				return (ulong)Value;
			}
			if (Value is decimal)
			{
				return Convert.ToSingle((decimal)Value);
			}
			if (Value is float)
			{
				return (float)Value;
			}
			if (Value is double)
			{
				return (float)(double)Value;
			}
			if (Value is string)
			{
				return ToSingle((string)Value);
			}
			throw new InvalidCastException();
		}

		public static double ToDouble(string Value)
		{
			if (Value == null)
			{
				return 0.0;
			}
			try
			{
				long i64Value = default(long);
				if (IsHexOrOctValue(Value, ref i64Value))
				{
					return i64Value;
				}
				return ParseDouble(Value);
			}
			catch (FormatException ex)
			{
				ProjectData.SetProjectError(ex);
				FormatException ex2 = ex;
				throw new InvalidCastException(ex2.Message, ex2);
			}
		}

		public static double ToDouble(object Value)
		{
			if (Value == null)
			{
				return 0.0;
			}
			if (Value is Enum)
			{
				Value = RuntimeHelpers.GetObjectValue(GetEnumValue(RuntimeHelpers.GetObjectValue(Value)));
			}
			if (Value is bool)
			{
				return 0 - (((bool)Value) ? 1 : 0);
			}
			if (Value is sbyte)
			{
				return (sbyte)Value;
			}
			if (Value is byte)
			{
				return (int)(byte)Value;
			}
			if (Value is short)
			{
				return (short)Value;
			}
			if (Value is ushort)
			{
				return (int)(ushort)Value;
			}
			if (Value is int)
			{
				return (int)Value;
			}
			if (Value is uint)
			{
				return (uint)Value;
			}
			if (Value is long)
			{
				return (long)Value;
			}
			if (Value is ulong)
			{
				return (ulong)Value;
			}
			if (Value is decimal)
			{
				return Convert.ToDouble((decimal)Value);
			}
			if (Value is float)
			{
				return (float)Value;
			}
			if (Value is double)
			{
				return (double)Value;
			}
			if (Value is string)
			{
				return ToDouble((string)Value);
			}
			throw new InvalidCastException();
		}

		private static double ParseDouble(string Value)
		{
			CultureInfo cultureInfo = GetCultureInfo();
			NumberFormatInfo numberFormat = cultureInfo.NumberFormat;
			NumberFormatInfo normalizedNumberFormat = GetNormalizedNumberFormat(numberFormat);
			Value = ToHalfwidthNumbers(Value, cultureInfo);
			try
			{
				return double.Parse(Value, NumberStyles.Any, normalizedNumberFormat);
			}
			catch (FormatException projectError) when (((Func<bool>)delegate
			{
				// Could not convert BlockContainer to single expression
				ProjectData.SetProjectError(projectError);
				return numberFormat != normalizedNumberFormat;
			}).Invoke())
			{
				double result = double.Parse(Value, NumberStyles.Any, numberFormat);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				throw ex2;
			}
		}

		public static DateTime ToDate(string Value)
		{
			CultureInfo cultureInfo = GetCultureInfo();
			if (DateTime.TryParse(ToHalfwidthNumbers(Value, cultureInfo), cultureInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault, out var result))
			{
				return result;
			}
			throw new InvalidCastException();
		}

		public static DateTime ToDate(object Value)
		{
			if (Value == null)
			{
				return DateTime.MinValue;
			}
			if (!(Value is DateTime result))
			{
				if (Value is string)
				{
					return ToDate((string)Value);
				}
				throw new InvalidCastException();
			}
			return result;
		}

		public static char ToChar(string Value)
		{
			if (Value == null || Value.Length == 0)
			{
				return Convert.ToChar(0);
			}
			return Value[0];
		}

		public static char ToChar(object Value)
		{
			if (Value == null)
			{
				return Convert.ToChar(0);
			}
			if (!(Value is char result))
			{
				if (Value is string)
				{
					return ToChar((string)Value);
				}
				throw new InvalidCastException();
			}
			return result;
		}

		public static string ToString(int Value)
		{
			return Value.ToString();
		}

		[CLSCompliant(false)]
		public static string ToString(uint Value)
		{
			return Value.ToString();
		}

		public static string ToString(long Value)
		{
			return Value.ToString();
		}

		[CLSCompliant(false)]
		public static string ToString(ulong Value)
		{
			return Value.ToString();
		}

		public static string ToString(float Value)
		{
			return Value.ToString();
		}

		public static string ToString(double Value)
		{
			return Value.ToString("G");
		}

		public static string ToString(DateTime Value)
		{
			long ticks = Value.TimeOfDay.Ticks;
			if (ticks == Value.Ticks || (Value.Year == 1899 && Value.Month == 12 && Value.Day == 30))
			{
				return Value.ToString("T");
			}
			if (ticks == 0L)
			{
				return Value.ToString("d");
			}
			return Value.ToString("G");
		}

		public static string ToString(decimal Value)
		{
			return Value.ToString("G");
		}

		public static string ToString(object Value)
		{
			if (Value == null)
			{
				return null;
			}
			if (Value is string result)
			{
				return result;
			}
			if (Value is Enum)
			{
				Value = RuntimeHelpers.GetObjectValue(GetEnumValue(RuntimeHelpers.GetObjectValue(Value)));
			}
			if (Value is bool)
			{
				return ToString((bool)Value);
			}
			if (Value is sbyte)
			{
				return ToString((sbyte)Value);
			}
			if (Value is byte)
			{
				return ToString((byte)Value);
			}
			if (Value is short)
			{
				return ToString((short)Value);
			}
			if (Value is ushort)
			{
				return ToString((uint)(ushort)Value);
			}
			if (Value is int)
			{
				return ToString((int)Value);
			}
			if (Value is uint)
			{
				return ToString((uint)Value);
			}
			if (Value is long)
			{
				return ToString((long)Value);
			}
			if (Value is ulong)
			{
				return ToString((ulong)Value);
			}
			if (Value is decimal)
			{
				return ToString((decimal)Value);
			}
			if (Value is float)
			{
				return ToString((float)Value);
			}
			if (Value is double)
			{
				return ToString((double)Value);
			}
			if (Value is char)
			{
				return ToString((char)Value);
			}
			if (Value is DateTime)
			{
				return ToString((DateTime)Value);
			}
			if (Value is char[] value)
			{
				return new string(value);
			}
			throw new InvalidCastException();
		}

		public static string ToString(bool Value)
		{
			if (Value)
			{
				return bool.TrueString;
			}
			return bool.FalseString;
		}

		public static string ToString(byte Value)
		{
			return Value.ToString();
		}

		public static string ToString(char Value)
		{
			return Value.ToString();
		}

		internal static CultureInfo GetCultureInfo()
		{
			return CultureInfo.CurrentCulture;
		}

		internal static string ToHalfwidthNumbers(string s, CultureInfo culture)
		{
			return s;
		}

		internal static bool IsHexOrOctValue(string Value, ref long i64Value)
		{
			int length = Value.Length;
			int num = default(int);
			char c;
			while (true)
			{
				if (num < length)
				{
					c = Value[num];
					if (c == '&' && num + 2 < length)
					{
						break;
					}
					if (c != ' ' && c != '\u3000')
					{
						return false;
					}
					num++;
					continue;
				}
				return false;
			}
			c = char.ToLowerInvariant(Value[num + 1]);
			string value = ToHalfwidthNumbers(Value.Substring(num + 2), GetCultureInfo());
			switch (c)
			{
			case 'h':
				i64Value = Convert.ToInt64(value, 16);
				break;
			case 'o':
				i64Value = Convert.ToInt64(value, 8);
				break;
			default:
				throw new FormatException();
			}
			return true;
		}

		internal static bool IsHexOrOctValue(string Value, ref ulong ui64Value)
		{
			int length = Value.Length;
			int num = default(int);
			char c;
			while (true)
			{
				if (num < length)
				{
					c = Value[num];
					if (c == '&' && num + 2 < length)
					{
						break;
					}
					if (c != ' ' && c != '\u3000')
					{
						return false;
					}
					num++;
					continue;
				}
				return false;
			}
			c = char.ToLowerInvariant(Value[num + 1]);
			string value = ToHalfwidthNumbers(Value.Substring(num + 2), GetCultureInfo());
			switch (c)
			{
			case 'h':
				ui64Value = Convert.ToUInt64(value, 16);
				break;
			case 'o':
				ui64Value = Convert.ToUInt64(value, 8);
				break;
			default:
				throw new FormatException();
			}
			return true;
		}

		public static T ToGenericParameter<T>(object Value)
		{
			if (Value == null)
			{
				return default(T);
			}
			Type typeFromHandle = typeof(T);
			if (object.Equals(typeFromHandle, typeof(bool)))
			{
				return (T)(object)ToBoolean(Value);
			}
			if (object.Equals(typeFromHandle, typeof(sbyte)))
			{
				return (T)(object)ToSByte(Value);
			}
			if (object.Equals(typeFromHandle, typeof(byte)))
			{
				return (T)(object)ToByte(Value);
			}
			if (object.Equals(typeFromHandle, typeof(short)))
			{
				return (T)(object)ToShort(Value);
			}
			if (object.Equals(typeFromHandle, typeof(ushort)))
			{
				return (T)(object)ToUShort(Value);
			}
			if (object.Equals(typeFromHandle, typeof(int)))
			{
				return (T)(object)ToInteger(Value);
			}
			if (object.Equals(typeFromHandle, typeof(uint)))
			{
				return (T)(object)ToUInteger(Value);
			}
			if (object.Equals(typeFromHandle, typeof(long)))
			{
				return (T)(object)ToLong(Value);
			}
			if (object.Equals(typeFromHandle, typeof(ulong)))
			{
				return (T)(object)ToULong(Value);
			}
			if (object.Equals(typeFromHandle, typeof(decimal)))
			{
				return (T)(object)ToDecimal(Value);
			}
			if (object.Equals(typeFromHandle, typeof(float)))
			{
				return (T)(object)ToSingle(Value);
			}
			if (object.Equals(typeFromHandle, typeof(double)))
			{
				return (T)(object)ToDouble(Value);
			}
			if (object.Equals(typeFromHandle, typeof(DateTime)))
			{
				return (T)(object)ToDate(Value);
			}
			if (object.Equals(typeFromHandle, typeof(char)))
			{
				return (T)(object)ToChar(Value);
			}
			if (object.Equals(typeFromHandle, typeof(string)))
			{
				return (T)(object)ToString(Value);
			}
			return (T)Value;
		}
	}
}
