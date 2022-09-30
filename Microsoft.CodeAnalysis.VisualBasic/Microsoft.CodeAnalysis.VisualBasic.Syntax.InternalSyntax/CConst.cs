using System;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class CConst
	{
		protected readonly ERRID _errid;

		protected readonly object[] _diagnosticArguments;

		public abstract SpecialType SpecialType { get; }

		public abstract object ValueAsObject { get; }

		public bool IsBad => SpecialType == SpecialType.None;

		public bool IsBooleanTrue
		{
			get
			{
				if (IsBad)
				{
					return false;
				}
				if (this is CConst<bool> cConst)
				{
					return cConst.Value;
				}
				return false;
			}
		}

		public ERRID ErrorId => _errid;

		public object[] ErrorArgs => _diagnosticArguments ?? Array.Empty<object>();

		public CConst()
		{
		}

		public CConst(ERRID id, params object[] diagnosticArguments)
		{
			_errid = id;
			_diagnosticArguments = diagnosticArguments;
		}

		public abstract CConst WithError(ERRID id);

		internal static CConst CreateChecked(object value)
		{
			return TryCreate(RuntimeHelpers.GetObjectValue(value));
		}

		internal static CConst TryCreate(object value)
		{
			if (value == null)
			{
				return CreateNothing();
			}
			return SpecialTypeExtensions.FromRuntimeTypeOfLiteralValue(RuntimeHelpers.GetObjectValue(value)) switch
			{
				SpecialType.System_Boolean => Create(Convert.ToBoolean(RuntimeHelpers.GetObjectValue(value))), 
				SpecialType.System_Byte => Create(Convert.ToByte(RuntimeHelpers.GetObjectValue(value))), 
				SpecialType.System_Char => Create(Convert.ToChar(RuntimeHelpers.GetObjectValue(value))), 
				SpecialType.System_DateTime => Create(Convert.ToDateTime(RuntimeHelpers.GetObjectValue(value))), 
				SpecialType.System_Decimal => Create(Convert.ToDecimal(RuntimeHelpers.GetObjectValue(value))), 
				SpecialType.System_Double => Create(Convert.ToDouble(RuntimeHelpers.GetObjectValue(value))), 
				SpecialType.System_Int16 => Create(Convert.ToInt16(RuntimeHelpers.GetObjectValue(value))), 
				SpecialType.System_Int32 => Create(Convert.ToInt32(RuntimeHelpers.GetObjectValue(value))), 
				SpecialType.System_Int64 => Create(Convert.ToInt64(RuntimeHelpers.GetObjectValue(value))), 
				SpecialType.System_SByte => Create(Convert.ToSByte(RuntimeHelpers.GetObjectValue(value))), 
				SpecialType.System_Single => Create(Convert.ToSingle(RuntimeHelpers.GetObjectValue(value))), 
				SpecialType.System_String => Create(Convert.ToString(RuntimeHelpers.GetObjectValue(value))), 
				SpecialType.System_UInt16 => Create(Convert.ToUInt16(RuntimeHelpers.GetObjectValue(value))), 
				SpecialType.System_UInt32 => Create(Convert.ToUInt32(RuntimeHelpers.GetObjectValue(value))), 
				SpecialType.System_UInt64 => Create(Convert.ToUInt64(RuntimeHelpers.GetObjectValue(value))), 
				_ => null, 
			};
		}

		internal static CConst<object> CreateNothing()
		{
			return new CConst<object>(null, SpecialType.System_Object);
		}

		internal static CConst<bool> Create(bool value)
		{
			return new CConst<bool>(value, SpecialType.System_Boolean);
		}

		internal static CConst<byte> Create(byte value)
		{
			return new CConst<byte>(value, SpecialType.System_Byte);
		}

		internal static CConst<sbyte> Create(sbyte value)
		{
			return new CConst<sbyte>(value, SpecialType.System_SByte);
		}

		internal static CConst<char> Create(char value)
		{
			return new CConst<char>(value, SpecialType.System_Char);
		}

		internal static CConst<short> Create(short value)
		{
			return new CConst<short>(value, SpecialType.System_Int16);
		}

		internal static CConst<ushort> Create(ushort value)
		{
			return new CConst<ushort>(value, SpecialType.System_UInt16);
		}

		internal static CConst<int> Create(int value)
		{
			return new CConst<int>(value, SpecialType.System_Int32);
		}

		internal static CConst<uint> Create(uint value)
		{
			return new CConst<uint>(value, SpecialType.System_UInt32);
		}

		internal static CConst<long> Create(long value)
		{
			return new CConst<long>(value, SpecialType.System_Int64);
		}

		internal static CConst<ulong> Create(ulong value)
		{
			return new CConst<ulong>(value, SpecialType.System_UInt64);
		}

		internal static CConst<decimal> Create(decimal value)
		{
			return new CConst<decimal>(value, SpecialType.System_Decimal);
		}

		internal static CConst<string> Create(string value)
		{
			return new CConst<string>(value, SpecialType.System_String);
		}

		internal static CConst<float> Create(float value)
		{
			return new CConst<float>(value, SpecialType.System_Single);
		}

		internal static CConst<double> Create(double value)
		{
			return new CConst<double>(value, SpecialType.System_Double);
		}

		internal static CConst<DateTime> Create(DateTime value)
		{
			return new CConst<DateTime>(value, SpecialType.System_DateTime);
		}
	}
	internal class CConst<T> : CConst
	{
		private readonly SpecialType _specialType;

		private readonly T _value;

		public override SpecialType SpecialType => _specialType;

		public override object ValueAsObject => _value;

		public T Value => _value;

		internal CConst(T value, SpecialType specialType)
		{
			_value = value;
			_specialType = specialType;
		}

		private CConst(T value, SpecialType specialType, ERRID id)
			: base(id)
		{
			_value = value;
			_specialType = specialType;
		}

		public override CConst WithError(ERRID id)
		{
			return new CConst<T>(_value, _specialType, id);
		}
	}
}
