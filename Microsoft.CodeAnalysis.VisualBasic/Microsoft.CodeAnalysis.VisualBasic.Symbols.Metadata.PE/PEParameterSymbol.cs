using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal class PEParameterSymbol : ParameterSymbol
	{
		private sealed class PEParameterSymbolWithCustomModifiers : PEParameterSymbol
		{
			private readonly ImmutableArray<CustomModifier> _customModifiers;

			private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

			public override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

			public override ImmutableArray<CustomModifier> CustomModifiers => _customModifiers;

			public PEParameterSymbolWithCustomModifiers(Symbol containingSymbol, string name, bool isByRef, ImmutableArray<CustomModifier> refCustomModifiers, TypeSymbol type, ParameterHandle handle, ParameterAttributes flags, bool isParamArray, bool hasOptionCompare, int ordinal, ConstantValue defaultValue, ImmutableArray<CustomModifier> customModifiers)
				: base(containingSymbol, name, isByRef, type, handle, flags, isParamArray, hasOptionCompare, ordinal, defaultValue)
			{
				_customModifiers = customModifiers;
				_refCustomModifiers = refCustomModifiers;
			}

			public PEParameterSymbolWithCustomModifiers(PEModuleSymbol moduleSymbol, Symbol containingSymbol, int ordinal, bool isByRef, ImmutableArray<ModifierInfo<TypeSymbol>> refCustomModifiers, TypeSymbol type, ParameterHandle handle, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers, out bool isBad)
				: base(moduleSymbol, containingSymbol, ordinal, isByRef, type, handle, out isBad)
			{
				_customModifiers = VisualBasicCustomModifier.Convert(customModifiers);
				_refCustomModifiers = VisualBasicCustomModifier.Convert(refCustomModifiers);
			}
		}

		private readonly Symbol _containingSymbol;

		private readonly string _name;

		private readonly TypeSymbol _type;

		private readonly ParameterHandle _handle;

		private readonly ParameterAttributes _flags;

		private readonly ushort _ordinal;

		private readonly byte _packed;

		private const int s_isByRefMask = 1;

		private const int s_hasNameInMetadataMask = 2;

		private const int s_hasOptionCompareMask = 4;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		private ConstantValue _lazyDefaultValue;

		private ThreeState _lazyHasIDispatchConstantAttribute;

		private ThreeState _lazyHasIUnknownConstantAttribute;

		private ThreeState _lazyHasCallerLineNumberAttribute;

		private ThreeState _lazyHasCallerMemberNameAttribute;

		private ThreeState _lazyHasCallerFilePathAttribute;

		private ThreeState _lazyIsParamArray;

		private ImmutableArray<VisualBasicAttributeData> _lazyHiddenAttributes;

		private bool HasNameInMetadata => (_packed & 2) != 0;

		public override string MetadataName
		{
			get
			{
				if (!HasNameInMetadata)
				{
					return string.Empty;
				}
				return _name;
			}
		}

		public override string Name => _name;

		internal ParameterAttributes ParamFlags => _flags;

		public override int Ordinal => _ordinal;

		public override Symbol ContainingSymbol => _containingSymbol;

		internal override bool HasMetadataConstantValue => (_flags & ParameterAttributes.HasDefault) != 0;

		internal override ConstantValue ExplicitDefaultConstantValue
		{
			get
			{
				if ((object)_lazyDefaultValue == ConstantValue.Unset)
				{
					ConstantValue defaultValue = null;
					PEModule pEModule = PEModule;
					ParameterHandle handle = _handle;
					if ((_flags & ParameterAttributes.HasDefault) != 0)
					{
						defaultValue = pEModule.GetParamDefaultValue(handle);
					}
					else if (IsOptional && !pEModule.HasDateTimeConstantAttribute(handle, out defaultValue))
					{
						pEModule.HasDecimalConstantAttribute(handle, out defaultValue);
					}
					Interlocked.CompareExchange(ref _lazyDefaultValue, defaultValue, ConstantValue.Unset);
				}
				return _lazyDefaultValue;
			}
		}

		internal override bool IsMetadataOptional => (_flags & ParameterAttributes.Optional) != 0;

		public override bool HasExplicitDefaultValue
		{
			get
			{
				if (IsOptional)
				{
					return (object)base.ExplicitDefaultConstantValue != null;
				}
				return false;
			}
		}

		public override bool IsOptional => (_flags & ParameterAttributes.Optional) != 0;

		public override bool IsParamArray
		{
			get
			{
				if (!_lazyIsParamArray.HasValue())
				{
					_lazyIsParamArray = PEModule.HasParamsAttribute(_handle).ToThreeState();
				}
				return _lazyIsParamArray.Value();
			}
		}

		public override ImmutableArray<Location> Locations => _containingSymbol.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override TypeSymbol Type => _type;

		public override bool IsByRef => (_packed & 1) != 0;

		internal override bool IsExplicitByRef => IsByRef;

		internal override bool IsMetadataOut => (_flags & ParameterAttributes.Out) != 0;

		internal override bool IsMetadataIn => (_flags & ParameterAttributes.In) != 0;

		internal override bool HasOptionCompare => (_packed & 4) != 0;

		public override ImmutableArray<CustomModifier> CustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		internal override bool IsMarshalledExplicitly => (_flags & ParameterAttributes.HasFieldMarshal) != 0;

		internal override MarshalPseudoCustomAttributeData MarshallingInformation => null;

		internal override ImmutableArray<byte> MarshallingDescriptor
		{
			get
			{
				if ((_flags & ParameterAttributes.HasFieldMarshal) == 0)
				{
					return default(ImmutableArray<byte>);
				}
				return PEModule.GetMarshallingDescriptor(_handle);
			}
		}

		internal override UnmanagedType MarshallingType
		{
			get
			{
				if ((_flags & ParameterAttributes.HasFieldMarshal) == 0)
				{
					return (UnmanagedType)0;
				}
				return PEModule.GetMarshallingType(_handle);
			}
		}

		internal ParameterHandle Handle => _handle;

		internal override bool IsIDispatchConstant
		{
			get
			{
				if (_lazyHasIDispatchConstantAttribute == ThreeState.Unknown)
				{
					_lazyHasIDispatchConstantAttribute = PEModule.HasAttribute(_handle, AttributeDescription.IDispatchConstantAttribute).ToThreeState();
				}
				return _lazyHasIDispatchConstantAttribute.Value();
			}
		}

		internal override bool IsIUnknownConstant
		{
			get
			{
				if (_lazyHasIUnknownConstantAttribute == ThreeState.Unknown)
				{
					_lazyHasIUnknownConstantAttribute = PEModule.HasAttribute(_handle, AttributeDescription.IUnknownConstantAttribute).ToThreeState();
				}
				return _lazyHasIUnknownConstantAttribute.Value();
			}
		}

		internal override bool IsCallerLineNumber
		{
			get
			{
				if (_lazyHasCallerLineNumberAttribute == ThreeState.Unknown)
				{
					_lazyHasCallerLineNumberAttribute = PEModule.HasAttribute(_handle, AttributeDescription.CallerLineNumberAttribute).ToThreeState();
				}
				return _lazyHasCallerLineNumberAttribute.Value();
			}
		}

		internal override bool IsCallerMemberName
		{
			get
			{
				if (_lazyHasCallerMemberNameAttribute == ThreeState.Unknown)
				{
					_lazyHasCallerMemberNameAttribute = PEModule.HasAttribute(_handle, AttributeDescription.CallerMemberNameAttribute).ToThreeState();
				}
				return _lazyHasCallerMemberNameAttribute.Value();
			}
		}

		internal override bool IsCallerFilePath
		{
			get
			{
				if (_lazyHasCallerFilePathAttribute == ThreeState.Unknown)
				{
					_lazyHasCallerFilePathAttribute = PEModule.HasAttribute(_handle, AttributeDescription.CallerFilePathAttribute).ToThreeState();
				}
				return _lazyHasCallerFilePathAttribute.Value();
			}
		}

		internal override VisualBasicCompilation DeclaringCompilation => null;

		private PEModule PEModule => ((PEModuleSymbol)_containingSymbol.ContainingModule).Module;

		internal static PEParameterSymbol Create(PEModuleSymbol moduleSymbol, PEMethodSymbol containingSymbol, int ordinal, ref ParamInfo<TypeSymbol> parameter, out bool isBad)
		{
			return Create(moduleSymbol, containingSymbol, ordinal, parameter.IsByRef, parameter.RefCustomModifiers, parameter.Type, parameter.Handle, parameter.CustomModifiers, out isBad);
		}

		internal static PEParameterSymbol Create(Symbol containingSymbol, string name, bool isByRef, ImmutableArray<CustomModifier> refCustomModifiers, TypeSymbol type, ParameterHandle handle, ParameterAttributes flags, bool isParamArray, bool hasOptionCompare, int ordinal, ConstantValue defaultValue, ImmutableArray<CustomModifier> customModifiers)
		{
			if (customModifiers.IsEmpty && refCustomModifiers.IsEmpty)
			{
				return new PEParameterSymbol(containingSymbol, name, isByRef, type, handle, flags, isParamArray, hasOptionCompare, ordinal, defaultValue);
			}
			return new PEParameterSymbolWithCustomModifiers(containingSymbol, name, isByRef, refCustomModifiers, type, handle, flags, isParamArray, hasOptionCompare, ordinal, defaultValue, customModifiers);
		}

		private PEParameterSymbol(Symbol containingSymbol, string name, bool isByRef, TypeSymbol type, ParameterHandle handle, ParameterAttributes flags, bool isParamArray, bool hasOptionCompare, int ordinal, ConstantValue defaultValue)
		{
			_lazyDefaultValue = ConstantValue.Unset;
			_lazyHasIDispatchConstantAttribute = ThreeState.Unknown;
			_lazyHasIUnknownConstantAttribute = ThreeState.Unknown;
			_lazyHasCallerLineNumberAttribute = ThreeState.Unknown;
			_lazyHasCallerMemberNameAttribute = ThreeState.Unknown;
			_lazyHasCallerFilePathAttribute = ThreeState.Unknown;
			_containingSymbol = containingSymbol;
			_name = EnsureParameterNameNotEmpty(name, out var hasNameInMetadata);
			_type = TupleTypeDecoder.DecodeTupleTypesIfApplicable(type, handle, (PEModuleSymbol)containingSymbol.ContainingModule);
			_handle = handle;
			_ordinal = (ushort)ordinal;
			_flags = flags;
			_lazyIsParamArray = isParamArray.ToThreeState();
			_lazyDefaultValue = defaultValue;
			_packed = Pack(isByRef, hasNameInMetadata, hasOptionCompare);
		}

		private static PEParameterSymbol Create(PEModuleSymbol moduleSymbol, Symbol containingSymbol, int ordinal, bool isByRef, ImmutableArray<ModifierInfo<TypeSymbol>> refCustomModifiers, TypeSymbol type, ParameterHandle handle, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers, out bool isBad)
		{
			if (customModifiers.IsDefaultOrEmpty && refCustomModifiers.IsDefaultOrEmpty)
			{
				return new PEParameterSymbol(moduleSymbol, containingSymbol, ordinal, isByRef, type, handle, out isBad);
			}
			return new PEParameterSymbolWithCustomModifiers(moduleSymbol, containingSymbol, ordinal, isByRef, refCustomModifiers, type, handle, customModifiers, out isBad);
		}

		private PEParameterSymbol(PEModuleSymbol moduleSymbol, Symbol containingSymbol, int ordinal, bool isByRef, TypeSymbol type, ParameterHandle handle, out bool isBad)
		{
			_lazyDefaultValue = ConstantValue.Unset;
			_lazyHasIDispatchConstantAttribute = ThreeState.Unknown;
			_lazyHasIUnknownConstantAttribute = ThreeState.Unknown;
			_lazyHasCallerLineNumberAttribute = ThreeState.Unknown;
			_lazyHasCallerMemberNameAttribute = ThreeState.Unknown;
			_lazyHasCallerFilePathAttribute = ThreeState.Unknown;
			isBad = false;
			_containingSymbol = containingSymbol;
			_type = TupleTypeDecoder.DecodeTupleTypesIfApplicable(type, handle, moduleSymbol);
			_ordinal = (ushort)ordinal;
			_handle = handle;
			bool hasOptionCompare = false;
			if (handle.IsNil)
			{
				_lazyCustomAttributes = ImmutableArray<VisualBasicAttributeData>.Empty;
				_lazyHiddenAttributes = ImmutableArray<VisualBasicAttributeData>.Empty;
				_lazyHasIDispatchConstantAttribute = ThreeState.False;
				_lazyHasIUnknownConstantAttribute = ThreeState.False;
				_lazyDefaultValue = null;
				_lazyHasCallerLineNumberAttribute = ThreeState.False;
				_lazyHasCallerMemberNameAttribute = ThreeState.False;
				_lazyHasCallerFilePathAttribute = ThreeState.False;
				_lazyIsParamArray = ThreeState.False;
			}
			else
			{
				try
				{
					moduleSymbol.Module.GetParamPropsOrThrow(handle, out _name, out _flags);
				}
				catch (BadImageFormatException ex)
				{
					ProjectData.SetProjectError(ex);
					BadImageFormatException ex2 = ex;
					isBad = true;
					ProjectData.ClearProjectError();
				}
				hasOptionCompare = moduleSymbol.Module.HasAttribute(handle, AttributeDescription.OptionCompareAttribute);
			}
			_name = EnsureParameterNameNotEmpty(_name, out var hasNameInMetadata);
			_packed = Pack(isByRef, hasNameInMetadata, hasOptionCompare);
		}

		private static byte Pack(bool isByRef, bool hasNameInMetadata, bool hasOptionCompare)
		{
			int num = (isByRef ? 1 : 0);
			int num2 = (hasNameInMetadata ? 2 : 0);
			int num3 = (hasOptionCompare ? 4 : 0);
			return (byte)(num | num2 | num3);
		}

		private static string EnsureParameterNameNotEmpty(string name, out bool hasNameInMetadata)
		{
			hasNameInMetadata = !string.IsNullOrEmpty(name);
			if (!hasNameInMetadata)
			{
				return "Param";
			}
			return name;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			if (_lazyCustomAttributes.IsDefault)
			{
				PEModuleSymbol pEModuleSymbol = (PEModuleSymbol)_containingSymbol.ContainingModule;
				bool flag = !_lazyIsParamArray.HasValue() || _lazyIsParamArray.Value();
				ConstantValue explicitDefaultConstantValue = base.ExplicitDefaultConstantValue;
				AttributeDescription filterOut = default(AttributeDescription);
				if ((object)explicitDefaultConstantValue != null)
				{
					if (explicitDefaultConstantValue.Discriminator == ConstantValueTypeDiscriminator.DateTime)
					{
						filterOut = AttributeDescription.DateTimeConstantAttribute;
					}
					else if (explicitDefaultConstantValue.Discriminator == ConstantValueTypeDiscriminator.Decimal)
					{
						filterOut = AttributeDescription.DecimalConstantAttribute;
					}
				}
				if (flag || filterOut.Signatures != null)
				{
					CustomAttributeHandle filteredOutAttribute;
					CustomAttributeHandle filteredOutAttribute2;
					ImmutableArray<VisualBasicAttributeData> customAttributesForToken = pEModuleSymbol.GetCustomAttributesForToken(_handle, out filteredOutAttribute, flag ? AttributeDescription.ParamArrayAttribute : default(AttributeDescription), out filteredOutAttribute2, filterOut);
					if (!filteredOutAttribute.IsNil || !filteredOutAttribute2.IsNil)
					{
						ArrayBuilder<VisualBasicAttributeData> instance = ArrayBuilder<VisualBasicAttributeData>.GetInstance();
						if (!filteredOutAttribute.IsNil)
						{
							instance.Add(new PEAttributeData(pEModuleSymbol, filteredOutAttribute));
						}
						if (!filteredOutAttribute2.IsNil)
						{
							instance.Add(new PEAttributeData(pEModuleSymbol, filteredOutAttribute2));
						}
						ImmutableInterlocked.InterlockedInitialize(ref _lazyHiddenAttributes, instance.ToImmutableAndFree());
					}
					else
					{
						ImmutableInterlocked.InterlockedInitialize(ref _lazyHiddenAttributes, ImmutableArray<VisualBasicAttributeData>.Empty);
					}
					if (!_lazyIsParamArray.HasValue())
					{
						_lazyIsParamArray = (!filteredOutAttribute.IsNil).ToThreeState();
					}
					ImmutableInterlocked.InterlockedInitialize(ref _lazyCustomAttributes, customAttributesForToken);
				}
				else
				{
					ImmutableInterlocked.InterlockedInitialize(ref _lazyHiddenAttributes, ImmutableArray<VisualBasicAttributeData>.Empty);
					pEModuleSymbol.LoadCustomAttributes(_handle, ref _lazyCustomAttributes);
				}
			}
			return _lazyCustomAttributes;
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_46_GetCustomAttributesToEmit))]
		internal override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_46_GetCustomAttributesToEmit(-2)
			{
				_0024VB_0024Me = this,
				_0024P_compilationState = compilationState
			};
		}
	}
}
