using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal sealed class PEFieldSymbol : FieldSymbol
	{
		private readonly FieldDefinitionHandle _handle;

		private readonly string _name;

		private readonly FieldAttributes _flags;

		private readonly PENamedTypeSymbol _containingType;

		private TypeSymbol _lazyType;

		private ImmutableArray<CustomModifier> _lazyCustomModifiers;

		private ConstantValue _lazyConstantValue;

		private Tuple<CultureInfo, string> _lazyDocComment;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		private CachedUseSiteInfo<AssemblySymbol> _lazyCachedUseSiteInfo;

		private ObsoleteAttributeData _lazyObsoleteAttributeData;

		public override string Name => _name;

		internal FieldAttributes FieldFlags => _flags;

		public override Symbol AssociatedSymbol => null;

		public override Symbol ContainingSymbol => _containingType;

		public override NamedTypeSymbol ContainingType => _containingType;

		public override Accessibility DeclaredAccessibility
		{
			get
			{
				Accessibility accessibility = Accessibility.Private;
				switch (_flags & FieldAttributes.FieldAccessMask)
				{
				case FieldAttributes.Assembly:
					return Accessibility.Internal;
				case FieldAttributes.FamORAssem:
					return Accessibility.ProtectedOrInternal;
				case FieldAttributes.FamANDAssem:
					return Accessibility.ProtectedAndInternal;
				case FieldAttributes.PrivateScope:
				case FieldAttributes.Private:
					return Accessibility.Private;
				case FieldAttributes.Public:
					return Accessibility.Public;
				case FieldAttributes.Family:
					return Accessibility.Protected;
				default:
					return Accessibility.Private;
				}
			}
		}

		internal override bool HasSpecialName => (_flags & FieldAttributes.SpecialName) != 0;

		internal override bool HasRuntimeSpecialName => (_flags & FieldAttributes.RTSpecialName) != 0;

		internal override bool IsNotSerialized => (_flags & FieldAttributes.NotSerialized) != 0;

		public override bool IsReadOnly => (_flags & FieldAttributes.InitOnly) != 0;

		public override bool IsConst
		{
			get
			{
				if ((_flags & FieldAttributes.Literal) == 0)
				{
					return (object)GetConstantValue(ConstantFieldsInProgress.Empty) != null;
				}
				return true;
			}
		}

		public override bool IsShared => (_flags & FieldAttributes.Static) != 0;

		internal override MarshalPseudoCustomAttributeData MarshallingInformation => null;

		internal override ObsoleteAttributeData ObsoleteAttributeData
		{
			get
			{
				ObsoleteAttributeHelpers.InitializeObsoleteDataFromMetadata(ref _lazyObsoleteAttributeData, _handle, (PEModuleSymbol)ContainingModule);
				return _lazyObsoleteAttributeData;
			}
		}

		internal override bool IsMarshalledExplicitly => (_flags & FieldAttributes.HasFieldMarshal) != 0;

		internal override UnmanagedType MarshallingType
		{
			get
			{
				if ((_flags & FieldAttributes.HasFieldMarshal) == 0)
				{
					return (UnmanagedType)0;
				}
				return PEModule.GetMarshallingType(_handle);
			}
		}

		internal override ImmutableArray<byte> MarshallingDescriptor
		{
			get
			{
				if ((_flags & FieldAttributes.HasFieldMarshal) == 0)
				{
					return default(ImmutableArray<byte>);
				}
				return PEModule.GetMarshallingDescriptor(_handle);
			}
		}

		internal override int? TypeLayoutOffset => PEModule.GetFieldOffset(_handle);

		public override ImmutableArray<Location> Locations => StaticCast<Location>.From(_containingType.ContainingPEModule.MetadataLocation);

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override TypeSymbol Type
		{
			get
			{
				EnsureSignatureIsLoaded();
				return _lazyType;
			}
		}

		public override ImmutableArray<CustomModifier> CustomModifiers
		{
			get
			{
				EnsureSignatureIsLoaded();
				return _lazyCustomModifiers;
			}
		}

		internal FieldDefinitionHandle Handle => _handle;

		internal override VisualBasicCompilation DeclaringCompilation => null;

		private PEModule PEModule => ((PEModuleSymbol)ContainingModule).Module;

		internal PEFieldSymbol(PEModuleSymbol moduleSymbol, PENamedTypeSymbol containingType, FieldDefinitionHandle handle)
		{
			_lazyConstantValue = Microsoft.CodeAnalysis.ConstantValue.Unset;
			_lazyCachedUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;
			_lazyObsoleteAttributeData = ObsoleteAttributeData.Uninitialized;
			_handle = handle;
			_containingType = containingType;
			try
			{
				moduleSymbol.Module.GetFieldDefPropsOrThrow(handle, out _name, out _flags);
			}
			catch (BadImageFormatException ex)
			{
				ProjectData.SetProjectError(ex);
				BadImageFormatException ex2 = ex;
				if (_name == null)
				{
					_name = string.Empty;
				}
				_lazyCachedUseSiteInfo.Initialize(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedField1, this));
				ProjectData.ClearProjectError();
			}
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			if (_lazyCustomAttributes.IsDefault)
			{
				PEModuleSymbol pEModuleSymbol = (PEModuleSymbol)ContainingModule;
				AttributeDescription constantAttributeDescription = GetConstantAttributeDescription();
				if (constantAttributeDescription.Signatures != null)
				{
					EntityHandle token = _handle;
					AttributeDescription filterOut = constantAttributeDescription;
					CustomAttributeHandle filteredOutAttribute = default(CustomAttributeHandle);
					CustomAttributeHandle filteredOutAttribute2;
					ImmutableArray<VisualBasicAttributeData> customAttributesForToken = pEModuleSymbol.GetCustomAttributesForToken(token, out filteredOutAttribute2, filterOut, out filteredOutAttribute);
					ImmutableInterlocked.InterlockedInitialize(ref _lazyCustomAttributes, customAttributesForToken);
				}
				else
				{
					pEModuleSymbol.LoadCustomAttributes(_handle, ref _lazyCustomAttributes);
				}
			}
			return _lazyCustomAttributes;
		}

		private AttributeDescription GetConstantAttributeDescription()
		{
			if (Type.SpecialType == SpecialType.System_DateTime)
			{
				ConstantValue constantValue = GetConstantValue(ConstantFieldsInProgress.Empty);
				if ((object)constantValue != null && constantValue.Discriminator == ConstantValueTypeDiscriminator.DateTime)
				{
					return AttributeDescription.DateTimeConstantAttribute;
				}
			}
			else if (Type.SpecialType == SpecialType.System_Decimal)
			{
				ConstantValue constantValue = GetConstantValue(ConstantFieldsInProgress.Empty);
				if ((object)constantValue != null && constantValue.Discriminator == ConstantValueTypeDiscriminator.Decimal)
				{
					return AttributeDescription.DecimalConstantAttribute;
				}
			}
			return default(AttributeDescription);
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_26_GetCustomAttributesToEmit))]
		internal override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_26_GetCustomAttributesToEmit(-2)
			{
				_0024VB_0024Me = this,
				_0024P_compilationState = compilationState
			};
		}

		internal override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress)
		{
			if ((object)_lazyConstantValue == Microsoft.CodeAnalysis.ConstantValue.Unset)
			{
				ConstantValue constantValue = null;
				if ((_flags & FieldAttributes.Literal) != 0)
				{
					constantValue = _containingType.ContainingPEModule.Module.GetConstantFieldValue(_handle);
					constantValue = CompileTimeCalculations.AdjustConstantValueFromMetadata(constantValue, Type, isByRefParamValue: false);
					TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(Type);
					SpecialType specialType = enumUnderlyingTypeOrSelf.SpecialType;
					if (constantValue.IsNothing)
					{
						if (TypeSymbolExtensions.IsNumericType(TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(Type)))
						{
							constantValue = Microsoft.CodeAnalysis.ConstantValue.Default(Microsoft.CodeAnalysis.ConstantValue.GetDiscriminator(specialType));
						}
						else
						{
							switch (specialType)
							{
							case SpecialType.System_DateTime:
								constantValue = Microsoft.CodeAnalysis.ConstantValue.Default(ConstantValueTypeDiscriminator.DateTime);
								break;
							case SpecialType.System_Boolean:
								constantValue = Microsoft.CodeAnalysis.ConstantValue.Default(ConstantValueTypeDiscriminator.Boolean);
								break;
							case SpecialType.System_Char:
								constantValue = Microsoft.CodeAnalysis.ConstantValue.Default(ConstantValueTypeDiscriminator.Char);
								break;
							default:
								if (TypeSymbolExtensions.IsErrorType(enumUnderlyingTypeOrSelf) || (object)Conversions.TryFoldNothingReferenceConversion(constantValue, ConversionKind.WideningNothingLiteral, enumUnderlyingTypeOrSelf) == null)
								{
									constantValue = Microsoft.CodeAnalysis.ConstantValue.Bad;
								}
								break;
							}
						}
					}
					else if (constantValue.SpecialType != specialType)
					{
						constantValue = Microsoft.CodeAnalysis.ConstantValue.Bad;
					}
				}
				ConstantValue defaultValue = null;
				if (Type.SpecialType == SpecialType.System_DateTime)
				{
					if (PEModule.HasDateTimeConstantAttribute(Handle, out defaultValue))
					{
						constantValue = defaultValue;
					}
				}
				else if (Type.SpecialType == SpecialType.System_Decimal && PEModule.HasDecimalConstantAttribute(Handle, out defaultValue))
				{
					constantValue = defaultValue;
				}
				Interlocked.CompareExchange(ref _lazyConstantValue, constantValue, Microsoft.CodeAnalysis.ConstantValue.Unset);
			}
			return _lazyConstantValue;
		}

		private void EnsureSignatureIsLoaded()
		{
			if ((object)_lazyType == null)
			{
				PEModuleSymbol containingPEModule = _containingType.ContainingPEModule;
				ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers = default(ImmutableArray<ModifierInfo<TypeSymbol>>);
				TypeSymbol metadataType = new MetadataDecoder(containingPEModule, _containingType).DecodeFieldSignature(_handle, out customModifiers);
				metadataType = TupleTypeDecoder.DecodeTupleTypesIfApplicable(metadataType, _handle, containingPEModule);
				ImmutableInterlocked.InterlockedCompareExchange(ref _lazyCustomModifiers, VisualBasicCustomModifier.Convert(customModifiers), default(ImmutableArray<CustomModifier>));
				Interlocked.CompareExchange(ref _lazyType, metadataType, null);
			}
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return PEDocumentationCommentUtils.GetDocumentationComment(this, _containingType.ContainingPEModule, preferredCulture, cancellationToken, ref _lazyDocComment);
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			AssemblySymbol primaryDependency = base.PrimaryDependency;
			if (!_lazyCachedUseSiteInfo.IsInitialized)
			{
				UseSiteInfo<AssemblySymbol> useSiteInfo = CalculateUseSiteInfo();
				if (useSiteInfo.DiagnosticInfo == null)
				{
					ConstantValue constantValue = GetConstantValue(ConstantFieldsInProgress.Empty);
					if ((object)constantValue != null && constantValue.IsBad)
					{
						useSiteInfo = new UseSiteInfo<AssemblySymbol>(new DiagnosticInfo(MessageProvider.Instance, 30799, ContainingType, Name));
					}
				}
				_lazyCachedUseSiteInfo.Initialize(primaryDependency, useSiteInfo);
			}
			return _lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency);
		}
	}
}
