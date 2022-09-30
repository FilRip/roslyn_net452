using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SourceAttributeData : VisualBasicAttributeData
	{
		private readonly NamedTypeSymbol _attributeClass;

		private readonly MethodSymbol _attributeConstructor;

		private readonly ImmutableArray<TypedConstant> _constructorArguments;

		private readonly ImmutableArray<KeyValuePair<string, TypedConstant>> _namedArguments;

		private readonly bool _isConditionallyOmitted;

		private readonly bool _hasErrors;

		private readonly SyntaxReference _applicationNode;

		public override NamedTypeSymbol AttributeClass => _attributeClass;

		public override MethodSymbol AttributeConstructor => _attributeConstructor;

		public override SyntaxReference ApplicationSyntaxReference => _applicationNode;

		protected internal override ImmutableArray<TypedConstant> CommonConstructorArguments
		{
			protected get
			{
				return _constructorArguments;
			}
		}

		protected internal override ImmutableArray<KeyValuePair<string, TypedConstant>> CommonNamedArguments
		{
			protected get
			{
				return _namedArguments;
			}
		}

		internal sealed override bool IsConditionallyOmitted => _isConditionallyOmitted;

		internal sealed override bool HasErrors => _hasErrors;

		internal SourceAttributeData(SyntaxReference applicationNode, NamedTypeSymbol attrClass, MethodSymbol attrMethod, ImmutableArray<TypedConstant> constructorArgs, ImmutableArray<KeyValuePair<string, TypedConstant>> namedArgs, bool isConditionallyOmitted, bool hasErrors)
		{
			_applicationNode = applicationNode;
			_attributeClass = attrClass;
			_attributeConstructor = attrMethod;
			_constructorArguments = constructorArgs.NullToEmpty();
			_namedArguments = (namedArgs.IsDefault ? ImmutableArray.Create<KeyValuePair<string, TypedConstant>>() : namedArgs);
			_isConditionallyOmitted = isConditionallyOmitted;
			_hasErrors = hasErrors;
		}

		internal SourceAttributeData WithOmittedCondition(bool isConditionallyOmitted)
		{
			if (IsConditionallyOmitted == isConditionallyOmitted)
			{
				return this;
			}
			return new SourceAttributeData(ApplicationSyntaxReference, AttributeClass, AttributeConstructor, CommonConstructorArguments, CommonNamedArguments, isConditionallyOmitted, HasErrors);
		}

		internal override int GetTargetAttributeSignatureIndex(Symbol targetSymbol, AttributeDescription description)
		{
			if (!IsTargetAttribute(description.Namespace, description.Name, description.MatchIgnoringCase))
			{
				return -1;
			}
			TypeSymbol typeSymbol = null;
			MethodSymbol attributeConstructor = AttributeConstructor;
			if ((object)attributeConstructor == null)
			{
				return -1;
			}
			ImmutableArray<ParameterSymbol> parameters = attributeConstructor.Parameters;
			bool flag = false;
			int num = description.Signatures.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				byte[] array = description.Signatures[i];
				if (array[0] != 32 || array[1] != parameters.Length || array[2] != 1)
				{
					continue;
				}
				flag = array.Length == 3;
				int num2 = 0;
				int num3 = array.Length - 1;
				for (int j = 3; j <= num3; j++)
				{
					if (num2 >= parameters.Length)
					{
						break;
					}
					TypeSymbol type = parameters[num2].Type;
					SpecialType specialType = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(type).SpecialType;
					byte b = array[j];
					if (b == 64)
					{
						j++;
						if (type.Kind != SymbolKind.NamedType && type.Kind != SymbolKind.ErrorType)
						{
							flag = false;
							break;
						}
						NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)type;
						AttributeDescription.TypeHandleTargetInfo typeHandleTargetInfo = AttributeDescription.TypeHandleTargets[array[j]];
						if (!string.Equals(namedTypeSymbol.MetadataName, typeHandleTargetInfo.Name, StringComparison.Ordinal) || !TypeSymbolExtensions.HasNameQualifier(namedTypeSymbol, typeHandleTargetInfo.Namespace, StringComparison.Ordinal))
						{
							flag = false;
							break;
						}
						b = (byte)typeHandleTargetInfo.Underlying;
					}
					else if (TypeSymbolExtensions.IsArrayType(type))
					{
						specialType = ((ArrayTypeSymbol)type).ElementType.SpecialType;
					}
					switch (b)
					{
					case 2:
						flag = specialType == SpecialType.System_Boolean;
						num2++;
						break;
					case 3:
						flag = specialType == SpecialType.System_Char;
						num2++;
						break;
					case 4:
						flag = specialType == SpecialType.System_SByte;
						num2++;
						break;
					case 5:
						flag = specialType == SpecialType.System_Byte;
						num2++;
						break;
					case 6:
						flag = specialType == SpecialType.System_Int16;
						num2++;
						break;
					case 7:
						flag = specialType == SpecialType.System_UInt16;
						num2++;
						break;
					case 8:
						flag = specialType == SpecialType.System_Int32;
						num2++;
						break;
					case 9:
						flag = specialType == SpecialType.System_UInt32;
						num2++;
						break;
					case 10:
						flag = specialType == SpecialType.System_Int64;
						num2++;
						break;
					case 11:
						flag = specialType == SpecialType.System_UInt64;
						num2++;
						break;
					case 12:
						flag = specialType == SpecialType.System_Single;
						num2++;
						break;
					case 13:
						flag = specialType == SpecialType.System_Double;
						num2++;
						break;
					case 14:
						flag = specialType == SpecialType.System_String;
						num2++;
						break;
					case 28:
						flag = specialType == SpecialType.System_Object;
						num2++;
						break;
					case 80:
						if ((object)typeSymbol == null)
						{
							typeSymbol = GetSystemType(targetSymbol);
						}
						flag = TypeSymbol.Equals(type, typeSymbol, TypeCompareKind.ConsiderEverything);
						num2++;
						break;
					case 29:
						flag = TypeSymbolExtensions.IsArrayType(type);
						break;
					default:
						return -1;
					}
					if (!flag)
					{
						break;
					}
				}
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		internal virtual TypeSymbol GetSystemType(Symbol targetSymbol)
		{
			return targetSymbol.DeclaringCompilation.GetWellKnownType(WellKnownType.System_Type);
		}
	}
}
