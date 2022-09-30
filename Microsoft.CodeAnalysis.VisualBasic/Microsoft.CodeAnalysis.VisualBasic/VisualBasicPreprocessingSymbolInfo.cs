using System;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal struct VisualBasicPreprocessingSymbolInfo : IEquatable<VisualBasicPreprocessingSymbolInfo>
	{
		private readonly PreprocessingSymbol _symbol;

		private readonly object _constantValue;

		private readonly bool _isDefined;

		internal static VisualBasicPreprocessingSymbolInfo None = new VisualBasicPreprocessingSymbolInfo(null, null, isDefined: false);

		public PreprocessingSymbol Symbol => _symbol;

		public bool IsDefined => _isDefined;

		public object ConstantValue => _constantValue;

		public static implicit operator PreprocessingSymbolInfo(VisualBasicPreprocessingSymbolInfo info)
		{
			return new PreprocessingSymbolInfo(info.Symbol, info.IsDefined);
		}

		internal VisualBasicPreprocessingSymbolInfo(PreprocessingSymbol symbol, object constantValueOpt, bool isDefined)
		{
			this = default(VisualBasicPreprocessingSymbolInfo);
			_symbol = symbol;
			_constantValue = RuntimeHelpers.GetObjectValue(constantValueOpt);
			_isDefined = isDefined;
		}

		public bool Equals(VisualBasicPreprocessingSymbolInfo other)
		{
			if (_isDefined == other._isDefined && _symbol == other._symbol)
			{
				return object.Equals(RuntimeHelpers.GetObjectValue(_constantValue), RuntimeHelpers.GetObjectValue(other._constantValue));
			}
			return false;
		}

		bool IEquatable<VisualBasicPreprocessingSymbolInfo>.Equals(VisualBasicPreprocessingSymbolInfo other)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Equals
			return this.Equals(other);
		}

		public override bool Equals(object obj)
		{
			if (obj is VisualBasicTypeInfo)
			{
				return Equals((VisualBasicTypeInfo)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Hash.Combine(_symbol, Hash.Combine(RuntimeHelpers.GetObjectValue(_constantValue), 0 - (_isDefined ? 1 : 0)));
		}
	}
}
