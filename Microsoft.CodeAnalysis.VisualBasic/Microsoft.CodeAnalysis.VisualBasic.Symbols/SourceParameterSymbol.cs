using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SourceParameterSymbol : SourceParameterSymbolBase, IAttributeTargetSymbol
	{
		private readonly Location _location;

		private readonly string _name;

		private readonly TypeSymbol _type;

		internal Location Location => _location;

		public sealed override string Name => _name;

		internal sealed override bool HasOptionCompare => false;

		internal override bool IsIDispatchConstant => false;

		internal override bool IsIUnknownConstant => false;

		public sealed override ImmutableArray<Location> Locations
		{
			get
			{
				if ((object)_location != null)
				{
					return ImmutableArray.Create(_location);
				}
				return ImmutableArray<Location>.Empty;
			}
		}

		public sealed override TypeSymbol Type => _type;

		public abstract override ImmutableArray<CustomModifier> CustomModifiers { get; }

		public abstract override ImmutableArray<CustomModifier> RefCustomModifiers { get; }

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
		{
			get
			{
				if (IsImplicitlyDeclared)
				{
					return ImmutableArray<SyntaxReference>.Empty;
				}
				return Symbol.GetDeclaringSyntaxReferenceHelper<ParameterSyntax>(Locations);
			}
		}

		public sealed override bool IsImplicitlyDeclared
		{
			get
			{
				if (base.ContainingSymbol.IsImplicitlyDeclared)
				{
					MethodSymbol obj = base.ContainingSymbol as MethodSymbol;
					if ((object)obj != null && obj.MethodKind == MethodKind.DelegateInvoke)
					{
						bool? flag = ContainingType.AssociatedSymbol?.IsImplicitlyDeclared;
						if (((!flag) ?? flag).GetValueOrDefault())
						{
							return false;
						}
					}
					return true;
				}
				return (object)GetMatchingPropertyParameter() != null;
			}
		}

		internal abstract SyntaxList<AttributeListSyntax> AttributeDeclarationList { get; }

		internal sealed override bool HasParamArrayAttribute => GetEarlyDecodedWellKnownAttributeData()?.HasParamArrayAttribute ?? false;

		internal sealed override bool HasDefaultValueAttribute
		{
			get
			{
				ParameterEarlyWellKnownAttributeData earlyDecodedWellKnownAttributeData = GetEarlyDecodedWellKnownAttributeData();
				if (earlyDecodedWellKnownAttributeData != null)
				{
					return earlyDecodedWellKnownAttributeData.DefaultParameterValue != ConstantValue.Unset;
				}
				return false;
			}
		}

		public AttributeLocation DefaultAttributeLocation => AttributeLocation.Parameter;

		internal sealed override MarshalPseudoCustomAttributeData MarshallingInformation
		{
			get
			{
				CommonParameterWellKnownAttributeData decodedWellKnownAttributeData = GetDecodedWellKnownAttributeData();
				if (decodedWellKnownAttributeData != null)
				{
					return decodedWellKnownAttributeData.MarshallingInformation;
				}
				if (TypeSymbolExtensions.IsStringType(Type))
				{
					Symbol containingSymbol = base.ContainingSymbol;
					if (containingSymbol.Kind == SymbolKind.Method)
					{
						MethodSymbol methodSymbol = (MethodSymbol)containingSymbol;
						if (methodSymbol.MethodKind == MethodKind.DeclareMethod)
						{
							MarshalPseudoCustomAttributeData marshalPseudoCustomAttributeData = new MarshalPseudoCustomAttributeData();
							if (IsExplicitByRef)
							{
								DllImportData dllImportData = methodSymbol.GetDllImportData();
								switch (dllImportData.CharacterSet)
								{
								case CharSet.None:
								case CharSet.Ansi:
									marshalPseudoCustomAttributeData.SetMarshalAsSimpleType(UnmanagedType.AnsiBStr);
									break;
								case CharSet.Auto:
									marshalPseudoCustomAttributeData.SetMarshalAsSimpleType(UnmanagedType.TBStr);
									break;
								case CharSet.Unicode:
									marshalPseudoCustomAttributeData.SetMarshalAsSimpleType(UnmanagedType.BStr);
									break;
								default:
									throw ExceptionUtilities.UnexpectedValue(dllImportData.CharacterSet);
								}
							}
							else
							{
								marshalPseudoCustomAttributeData.SetMarshalAsSimpleType(UnmanagedType.VBByRefStr);
							}
							return marshalPseudoCustomAttributeData;
						}
					}
				}
				return null;
			}
		}

		public sealed override bool IsByRef
		{
			get
			{
				if (IsExplicitByRef)
				{
					return true;
				}
				if (TypeSymbolExtensions.IsStringType(Type) && base.ContainingSymbol.Kind == SymbolKind.Method && ((MethodSymbol)base.ContainingSymbol).MethodKind == MethodKind.DeclareMethod)
				{
					ParameterEarlyWellKnownAttributeData earlyDecodedWellKnownAttributeData = GetEarlyDecodedWellKnownAttributeData();
					return earlyDecodedWellKnownAttributeData == null || !earlyDecodedWellKnownAttributeData.HasMarshalAsAttribute;
				}
				return false;
			}
		}

		internal sealed override bool IsMetadataOut => GetDecodedWellKnownAttributeData()?.HasOutAttribute ?? false;

		internal sealed override bool IsMetadataIn => GetDecodedWellKnownAttributeData()?.HasInAttribute ?? false;

		internal SourceParameterSymbol(Symbol container, string name, int ordinal, TypeSymbol type, Location location)
			: base(container, ordinal)
		{
			_name = name;
			_type = type;
			_location = location;
		}

		private ParameterSymbol GetMatchingPropertyParameter()
		{
			if (base.ContainingSymbol is MethodSymbol methodSymbol && SymbolExtensions.IsAccessor(methodSymbol) && methodSymbol.AssociatedSymbol is PropertySymbol propertySymbol && base.Ordinal < propertySymbol.ParameterCount)
			{
				return propertySymbol.Parameters[base.Ordinal];
			}
			return null;
		}

		internal abstract CustomAttributesBag<VisualBasicAttributeData> GetAttributesBag();

		internal abstract ParameterEarlyWellKnownAttributeData GetEarlyDecodedWellKnownAttributeData();

		internal abstract CommonParameterWellKnownAttributeData GetDecodedWellKnownAttributeData();

		public sealed override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return GetAttributesBag().Attributes;
		}

		internal override VisualBasicAttributeData EarlyDecodeWellKnownAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
		{
			Symbol containingSymbol = base.ContainingSymbol;
			if (containingSymbol.Kind == SymbolKind.Method && ((MethodSymbol)containingSymbol).MethodKind == MethodKind.DeclareMethod && VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.MarshalAsAttribute))
			{
				bool generatedDiagnostics = false;
				SourceAttributeData attribute = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics);
				if (!attribute.HasErrors)
				{
					arguments.GetOrCreateData<ParameterEarlyWellKnownAttributeData>().HasMarshalAsAttribute = true;
					return (!generatedDiagnostics) ? attribute : null;
				}
				return null;
			}
			bool flag = false;
			switch (containingSymbol.Kind)
			{
			case SymbolKind.Property:
				flag = true;
				break;
			case SymbolKind.Method:
				switch (((MethodSymbol)containingSymbol).MethodKind)
				{
				default:
					flag = true;
					break;
				case MethodKind.Conversion:
				case MethodKind.EventAdd:
				case MethodKind.EventRemove:
				case MethodKind.UserDefinedOperator:
					break;
				}
				break;
			}
			if (flag && VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.ParamArrayAttribute))
			{
				bool generatedDiagnostics2 = false;
				SourceAttributeData attribute2 = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics2);
				if (!attribute2.HasErrors)
				{
					arguments.GetOrCreateData<ParameterEarlyWellKnownAttributeData>().HasParamArrayAttribute = true;
					return (!generatedDiagnostics2) ? attribute2 : null;
				}
				return null;
			}
			if (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.DefaultParameterValueAttribute))
			{
				return EarlyDecodeAttributeForDefaultParameterValue(AttributeDescription.DefaultParameterValueAttribute, ref arguments);
			}
			if (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.DecimalConstantAttribute))
			{
				return EarlyDecodeAttributeForDefaultParameterValue(AttributeDescription.DecimalConstantAttribute, ref arguments);
			}
			if (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.DateTimeConstantAttribute))
			{
				return EarlyDecodeAttributeForDefaultParameterValue(AttributeDescription.DateTimeConstantAttribute, ref arguments);
			}
			if (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.CallerLineNumberAttribute))
			{
				arguments.GetOrCreateData<ParameterEarlyWellKnownAttributeData>().HasCallerLineNumberAttribute = true;
			}
			else if (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.CallerFilePathAttribute))
			{
				arguments.GetOrCreateData<ParameterEarlyWellKnownAttributeData>().HasCallerFilePathAttribute = true;
			}
			else if (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.CallerMemberNameAttribute))
			{
				arguments.GetOrCreateData<ParameterEarlyWellKnownAttributeData>().HasCallerMemberNameAttribute = true;
			}
			return base.EarlyDecodeWellKnownAttribute(ref arguments);
		}

		private VisualBasicAttributeData EarlyDecodeAttributeForDefaultParameterValue(AttributeDescription description, ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
		{
			bool generatedDiagnostics = false;
			SourceAttributeData attribute = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics);
			ConstantValue defaultParameterValue;
			if (attribute.HasErrors)
			{
				defaultParameterValue = ConstantValue.Bad;
				generatedDiagnostics = true;
			}
			else
			{
				defaultParameterValue = DecodeDefaultParameterValueAttribute(description, attribute);
			}
			ParameterEarlyWellKnownAttributeData orCreateData = arguments.GetOrCreateData<ParameterEarlyWellKnownAttributeData>();
			if (orCreateData.DefaultParameterValue == ConstantValue.Unset)
			{
				orCreateData.DefaultParameterValue = defaultParameterValue;
			}
			if (!generatedDiagnostics)
			{
				return attribute;
			}
			return null;
		}

		internal override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments)
		{
			VisualBasicAttributeData attribute = arguments.Attribute;
			if (attribute.IsTargetAttribute(this, AttributeDescription.TupleElementNamesAttribute))
			{
				((BindingDiagnosticBag)arguments.Diagnostics).Add(ERRID.ERR_ExplicitTupleElementNamesAttribute, arguments.AttributeSyntaxOpt!.Location);
			}
			if (attribute.IsTargetAttribute(this, AttributeDescription.DefaultParameterValueAttribute))
			{
				DecodeDefaultParameterValueAttribute(AttributeDescription.DefaultParameterValueAttribute, ref arguments);
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.DecimalConstantAttribute))
			{
				DecodeDefaultParameterValueAttribute(AttributeDescription.DecimalConstantAttribute, ref arguments);
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.DateTimeConstantAttribute))
			{
				DecodeDefaultParameterValueAttribute(AttributeDescription.DateTimeConstantAttribute, ref arguments);
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.InAttribute))
			{
				arguments.GetOrCreateData<CommonParameterWellKnownAttributeData>().HasInAttribute = true;
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.OutAttribute))
			{
				arguments.GetOrCreateData<CommonParameterWellKnownAttributeData>().HasOutAttribute = true;
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.MarshalAsAttribute))
			{
				MarshalAsAttributeDecoder<CommonParameterWellKnownAttributeData, AttributeSyntax, VisualBasicAttributeData, AttributeLocation>.Decode(ref arguments, AttributeTargets.Parameter, MessageProvider.Instance);
			}
			base.DecodeWellKnownAttribute(ref arguments);
		}

		private void DecodeDefaultParameterValueAttribute(AttributeDescription description, ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments)
		{
			VisualBasicAttributeData attribute = arguments.Attribute;
			BindingDiagnosticBag diagnostics = (BindingDiagnosticBag)arguments.Diagnostics;
			ConstantValue constantValue = DecodeDefaultParameterValueAttribute(description, attribute);
			if (!constantValue.IsBad)
			{
				VerifyParamDefaultValueMatchesAttributeIfAny(constantValue, arguments.AttributeSyntaxOpt, diagnostics);
			}
		}

		protected void VerifyParamDefaultValueMatchesAttributeIfAny(ConstantValue value, VisualBasicSyntaxNode syntax, BindingDiagnosticBag diagnostics)
		{
			ParameterEarlyWellKnownAttributeData earlyDecodedWellKnownAttributeData = GetEarlyDecodedWellKnownAttributeData();
			if (earlyDecodedWellKnownAttributeData != null)
			{
				ConstantValue defaultParameterValue = earlyDecodedWellKnownAttributeData.DefaultParameterValue;
				if (defaultParameterValue != ConstantValue.Unset && value != defaultParameterValue)
				{
					Binder.ReportDiagnostic(diagnostics, syntax, ERRID.ERR_ParamDefaultValueDiffersFromAttribute);
				}
			}
		}

		private ConstantValue DecodeDefaultParameterValueAttribute(AttributeDescription description, VisualBasicAttributeData attribute)
		{
			if (description.Equals(AttributeDescription.DefaultParameterValueAttribute))
			{
				return DecodeDefaultParameterValueAttribute(attribute);
			}
			if (description.Equals(AttributeDescription.DecimalConstantAttribute))
			{
				return attribute.DecodeDecimalConstantValue();
			}
			return attribute.DecodeDateTimeConstantValue();
		}

		private ConstantValue DecodeDefaultParameterValueAttribute(VisualBasicAttributeData attribute)
		{
			TypedConstant typedConstant = attribute.CommonConstructorArguments[0];
			ConstantValueTypeDiscriminator discriminator = ConstantValue.GetDiscriminator((typedConstant.Kind == TypedConstantKind.Enum) ? ((NamedTypeSymbol)typedConstant.TypeInternal).EnumUnderlyingType.SpecialType : typedConstant.TypeInternal!.SpecialType);
			if (discriminator == ConstantValueTypeDiscriminator.Bad)
			{
				if (typedConstant.Kind != TypedConstantKind.Array && typedConstant.ValueInternal == null && Type.IsReferenceType)
				{
					return ConstantValue.Null;
				}
				return ConstantValue.Bad;
			}
			return ConstantValue.Create(RuntimeHelpers.GetObjectValue(typedConstant.ValueInternal), discriminator);
		}
	}
}
