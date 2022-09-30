using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	public sealed class TypedConstantExtensions
	{
		public static string ToVisualBasicString(this TypedConstant constant)
		{
			if (constant.IsNull)
			{
				return "Nothing";
			}
			if (constant.Kind == TypedConstantKind.Array)
			{
				return "{" + string.Join(", ", constant.Values.Select((TypedConstant v) => ToVisualBasicString(v))) + "}";
			}
			if (constant.Kind == TypedConstantKind.Type || constant.TypeInternal!.SpecialType == SpecialType.System_Object)
			{
				return "GetType(" + constant.Value!.ToString() + ")";
			}
			if (constant.Kind == TypedConstantKind.Enum)
			{
				return DisplayEnumConstant(constant);
			}
			return SymbolDisplay.FormatPrimitive(RuntimeHelpers.GetObjectValue(constant.ValueInternal), quoteStrings: true, useHexadecimalNumbers: false);
		}

		private static string DisplayEnumConstant(TypedConstant constant)
		{
			SpecialType specialType = ((NamedTypeSymbol)constant.TypeInternal).EnumUnderlyingType.SpecialType;
			ConstantValue constantValue = ConstantValue.Create(RuntimeHelpers.GetObjectValue(constant.ValueInternal), specialType);
			string typeName = constant.Type!.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat);
			if (constantValue.IsUnsigned)
			{
				return DisplayUnsignedEnumConstant(constant, specialType, constantValue.UInt64Value, typeName);
			}
			return DisplaySignedEnumConstant(constant, specialType, constantValue.Int64Value, typeName);
		}

		private static string DisplayUnsignedEnumConstant(TypedConstant constant, SpecialType splType, ulong constantToDecode, string typeName)
		{
			ulong num = 0uL;
			PooledStringBuilder pooledStringBuilder = null;
			StringBuilder stringBuilder = null;
			ImmutableArray<ISymbol>.Enumerator enumerator = constant.Type!.GetMembers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!(enumerator.Current is IFieldSymbol fieldSymbol) || !fieldSymbol.HasConstantValue)
				{
					continue;
				}
				ulong uInt64Value = ConstantValue.Create(RuntimeHelpers.GetObjectValue(fieldSymbol.ConstantValue), splType).UInt64Value;
				if (uInt64Value == constantToDecode)
				{
					pooledStringBuilder?.Free();
					return typeName + "." + fieldSymbol.Name;
				}
				if ((uInt64Value & constantToDecode) == uInt64Value)
				{
					num |= uInt64Value;
					if (stringBuilder == null)
					{
						pooledStringBuilder = PooledStringBuilder.GetInstance();
						stringBuilder = pooledStringBuilder.Builder;
					}
					else
					{
						stringBuilder.Append(" Or ");
					}
					stringBuilder.Append(typeName);
					stringBuilder.Append(".");
					stringBuilder.Append(fieldSymbol.Name);
				}
			}
			if (pooledStringBuilder != null)
			{
				if (num == constantToDecode)
				{
					return pooledStringBuilder.ToStringAndFree();
				}
				pooledStringBuilder.Free();
			}
			return constant.ValueInternal!.ToString();
		}

		private static string DisplaySignedEnumConstant(TypedConstant constant, SpecialType splType, long constantToDecode, string typeName)
		{
			long num = 0L;
			PooledStringBuilder pooledStringBuilder = null;
			StringBuilder stringBuilder = null;
			ImmutableArray<ISymbol>.Enumerator enumerator = constant.Type!.GetMembers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!(enumerator.Current is IFieldSymbol fieldSymbol) || !fieldSymbol.HasConstantValue)
				{
					continue;
				}
				long int64Value = ConstantValue.Create(RuntimeHelpers.GetObjectValue(fieldSymbol.ConstantValue), splType).Int64Value;
				if (int64Value == constantToDecode)
				{
					pooledStringBuilder?.Free();
					return typeName + "." + fieldSymbol.Name;
				}
				if ((int64Value & constantToDecode) == int64Value)
				{
					num |= int64Value;
					if (stringBuilder == null)
					{
						pooledStringBuilder = PooledStringBuilder.GetInstance();
						stringBuilder = pooledStringBuilder.Builder;
					}
					else
					{
						stringBuilder.Append(" Or ");
					}
					stringBuilder.Append(typeName);
					stringBuilder.Append(".");
					stringBuilder.Append(fieldSymbol.Name);
				}
			}
			if (pooledStringBuilder != null)
			{
				if (num == constantToDecode)
				{
					return pooledStringBuilder.ToStringAndFree();
				}
				pooledStringBuilder.Free();
			}
			return constant.ValueInternal!.ToString();
		}
	}
}
