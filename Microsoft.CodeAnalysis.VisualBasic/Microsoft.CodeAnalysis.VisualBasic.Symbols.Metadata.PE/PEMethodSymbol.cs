using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal sealed class PEMethodSymbol : MethodSymbol
	{
		private struct PackedFlags
		{
			private int _bits;

			private const int s_methodKindOffset = 0;

			private const int s_methodKindMask = 31;

			private const int s_methodKindIsPopulatedBit = 32;

			private const int s_isExtensionMethodBit = 64;

			private const int s_isExtensionMethodIsPopulatedBit = 128;

			private const int s_isObsoleteAttributePopulatedBit = 256;

			private const int s_isCustomAttributesPopulatedBit = 512;

			private const int s_isUseSiteDiagnosticPopulatedBit = 1024;

			private const int s_isConditionalAttributePopulatedBit = 2048;

			private const int s_isInitOnlyBit = 4096;

			private const int s_isInitOnlyPopulatedBit = 8192;

			public MethodKind MethodKind
			{
				get
				{
					return (MethodKind)((_bits >> 0) & 0x1F);
				}
				set
				{
					_bits = (_bits & -32) | ((int)value << 0) | 0x20;
				}
			}

			public bool MethodKindIsPopulated => (_bits & 0x20) != 0;

			public bool IsExtensionMethod => (_bits & 0x40) != 0;

			public bool IsExtensionMethodPopulated => (_bits & 0x80) != 0;

			public bool IsObsoleteAttributePopulated => (_bits & 0x100) != 0;

			public bool IsCustomAttributesPopulated => (_bits & 0x200) != 0;

			public bool IsUseSiteDiagnosticPopulated => (_bits & 0x400) != 0;

			public bool IsConditionalPopulated => (_bits & 0x800) != 0;

			public bool IsInitOnly => (_bits & 0x1000) != 0;

			public bool IsInitOnlyPopulated => (_bits & 0x2000) != 0;

			private static bool BitsAreUnsetOrSame(int bits, int mask)
			{
				if ((bits & mask) != 0)
				{
					return (bits & mask) == mask;
				}
				return true;
			}

			public void InitializeMethodKind(MethodKind methodKind)
			{
				int toSet = ((int)(methodKind & (MethodKind)31) << 0) | 0x20;
				ThreadSafeFlagOperations.Set(ref _bits, toSet);
			}

			public void InitializeIsExtensionMethod(bool isExtensionMethod)
			{
				int toSet = (isExtensionMethod ? 64 : 0) | 0x80;
				ThreadSafeFlagOperations.Set(ref _bits, toSet);
			}

			public void SetIsObsoleteAttributePopulated()
			{
				ThreadSafeFlagOperations.Set(ref _bits, 256);
			}

			public void SetIsCustomAttributesPopulated()
			{
				ThreadSafeFlagOperations.Set(ref _bits, 512);
			}

			public void SetIsUseSiteDiagnosticPopulated()
			{
				ThreadSafeFlagOperations.Set(ref _bits, 1024);
			}

			public void SetIsConditionalAttributePopulated()
			{
				ThreadSafeFlagOperations.Set(ref _bits, 2048);
			}

			public void InitializeIsInitOnly(bool isInitOnly)
			{
				int toSet = (isInitOnly ? 4096 : 0) | 0x2000;
				ThreadSafeFlagOperations.Set(ref _bits, toSet);
			}
		}

		private sealed class UncommonFields
		{
			public ParameterSymbol _lazyMeParameter;

			public Tuple<CultureInfo, string> _lazyDocComment;

			public ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

			public ImmutableArray<string> _lazyConditionalAttributeSymbols;

			public ObsoleteAttributeData _lazyObsoleteAttributeData;

			public CachedUseSiteInfo<AssemblySymbol> _lazyCachedUseSiteInfo;
		}

		private class SignatureData
		{
			public readonly SignatureHeader Header;

			public readonly ImmutableArray<ParameterSymbol> Parameters;

			public readonly PEParameterSymbol ReturnParam;

			public SignatureData(SignatureHeader signatureHeader, ImmutableArray<ParameterSymbol> parameters, PEParameterSymbol returnParam)
			{
				Header = signatureHeader;
				Parameters = parameters;
				ReturnParam = returnParam;
			}
		}

		private readonly MethodDefinitionHandle _handle;

		private readonly string _name;

		private readonly ushort _implFlags;

		private readonly ushort _flags;

		private readonly PENamedTypeSymbol _containingType;

		private Symbol _associatedPropertyOrEventOpt;

		private PackedFlags _packedFlags;

		private ImmutableArray<TypeParameterSymbol> _lazyTypeParameters;

		private ImmutableArray<MethodSymbol> _lazyExplicitMethodImplementations;

		private UncommonFields _uncommonFields;

		private SignatureData _lazySignature;

		public override Symbol ContainingSymbol => _containingType;

		public override NamedTypeSymbol ContainingType => _containingType;

		public override string Name => _name;

		internal override bool HasSpecialName => (_flags & 0x800) != 0;

		internal override bool HasRuntimeSpecialName => (_flags & 0x1000) != 0;

		internal override bool IsMetadataFinal => (_flags & 0x20) != 0;

		internal MethodImplAttributes MethodImplFlags => (MethodImplAttributes)_implFlags;

		internal MethodAttributes MethodFlags => (MethodAttributes)_flags;

		public override MethodKind MethodKind
		{
			get
			{
				if (!_packedFlags.MethodKindIsPopulated)
				{
					_packedFlags.InitializeMethodKind(ComputeMethodKind());
				}
				return _packedFlags.MethodKind;
			}
		}

		internal override bool IsMethodKindBasedOnSyntax => false;

		public override Symbol AssociatedSymbol => _associatedPropertyOrEventOpt;

		public override Accessibility DeclaredAccessibility
		{
			get
			{
				Accessibility accessibility = Accessibility.Private;
				switch (_flags & 7)
				{
				case 3:
					return Accessibility.Internal;
				case 5:
					return Accessibility.ProtectedOrInternal;
				case 2:
					return Accessibility.ProtectedAndInternal;
				case 0:
				case 1:
					return Accessibility.Private;
				case 6:
					return Accessibility.Public;
				case 4:
					return Accessibility.Protected;
				default:
					return Accessibility.Private;
				}
			}
		}

		public override bool IsExtensionMethod
		{
			get
			{
				if (!_packedFlags.IsExtensionMethodPopulated)
				{
					bool isExtensionMethod = false;
					if (IsShared && ParameterCount > 0 && MethodKind == MethodKind.Ordinary && _containingType.MightContainExtensionMethods && _containingType.ContainingPEModule.Module.HasExtensionAttribute(Handle, ignoreCase: true) && ValidateGenericConstraintsOnExtensionMethodDefinition())
					{
						ParameterSymbol parameterSymbol = Parameters[0];
						isExtensionMethod = !parameterSymbol.IsOptional && !parameterSymbol.IsParamArray;
					}
					_packedFlags.InitializeIsExtensionMethod(isExtensionMethod);
				}
				return _packedFlags.IsExtensionMethod;
			}
		}

		public override bool IsExternalMethod
		{
			get
			{
				if ((_flags & 0x2000) == 0)
				{
					return (_implFlags & 0x1003) != 0;
				}
				return true;
			}
		}

		internal override bool IsExternal
		{
			get
			{
				if (!IsExternalMethod)
				{
					return (_implFlags & 3) != 0;
				}
				return true;
			}
		}

		internal override bool IsAccessCheckedOnOverride => (_flags & 0x200) != 0;

		internal override bool ReturnValueIsMarshalledExplicitly => _lazySignature.ReturnParam.IsMarshalledExplicitly;

		internal override MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation => _lazySignature.ReturnParam.MarshallingInformation;

		internal override ImmutableArray<byte> ReturnValueMarshallingDescriptor => _lazySignature.ReturnParam.MarshallingDescriptor;

		internal override MethodImplAttributes ImplementationAttributes => (MethodImplAttributes)_implFlags;

		internal override bool HasDeclarativeSecurity
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		private SignatureData Signature => _lazySignature ?? LoadSignature();

		public override bool IsVararg => Signature.Header.CallingConvention == SignatureCallingConvention.VarArgs;

		public override bool IsGenericMethod => Arity > 0;

		public override int Arity
		{
			get
			{
				if (_lazyTypeParameters.IsDefault)
				{
					try
					{
						int parameterCount = 0;
						int typeParameterCount = 0;
						MetadataDecoder<PEModuleSymbol, TypeSymbol, MethodSymbol, FieldSymbol, Symbol>.GetSignatureCountsOrThrow(_containingType.ContainingPEModule.Module, _handle, out parameterCount, out typeParameterCount);
						return typeParameterCount;
					}
					catch (BadImageFormatException ex)
					{
						ProjectData.SetProjectError(ex);
						BadImageFormatException ex2 = ex;
						int length = TypeParameters.Length;
						ProjectData.ClearProjectError();
						return length;
					}
				}
				return _lazyTypeParameters.Length;
			}
		}

		internal MethodDefinitionHandle Handle => _handle;

		public override bool IsMustOverride
		{
			get
			{
				if ((_flags & 0x40u) != 0)
				{
					return (_flags & 0x400) != 0;
				}
				return false;
			}
		}

		public override bool IsNotOverridable => (_flags & 0x560) == (_containingType.IsInterface ? 1120 : 96);

		public override bool IsOverloads
		{
			get
			{
				if ((_flags & 0x80) == 0)
				{
					return IsOverrides;
				}
				return true;
			}
		}

		internal override bool IsHiddenBySignature => (_flags & 0x80) != 0;

		public override bool IsOverridable
		{
			get
			{
				int num = _flags & 0x560;
				if (num != 320)
				{
					if (!_containingType.IsInterface && num == 64)
					{
						return (object)_containingType.BaseTypeNoUseSiteDiagnostics == null;
					}
					return false;
				}
				return true;
			}
		}

		public override bool IsOverrides
		{
			get
			{
				if (!_containingType.IsInterface && (_flags & 0x40u) != 0 && (_flags & 0x100) == 0)
				{
					return (object)_containingType.BaseTypeNoUseSiteDiagnostics != null;
				}
				return false;
			}
		}

		public override bool IsShared => (_flags & 0x10) != 0;

		public override bool IsSub => ReturnType.SpecialType == SpecialType.System_Void;

		public override bool IsAsync => false;

		public override bool IsIterator => false;

		public override bool IsInitOnly
		{
			get
			{
				if (!_packedFlags.IsInitOnlyPopulated)
				{
					bool isInitOnly = !IsShared && MethodKind == MethodKind.PropertySet && CustomModifierUtils.HasIsExternalInitModifier(ReturnTypeCustomModifiers);
					_packedFlags.InitializeIsInitOnly(isInitOnly);
				}
				return _packedFlags.IsInitOnly;
			}
		}

		public override ImmutableArray<Location> Locations => StaticCast<Location>.From(_containingType.ContainingPEModule.MetadataLocation);

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		internal override int ParameterCount
		{
			get
			{
				if (_lazySignature == null)
				{
					try
					{
						int parameterCount = 0;
						int typeParameterCount = 0;
						MetadataDecoder<PEModuleSymbol, TypeSymbol, MethodSymbol, FieldSymbol, Symbol>.GetSignatureCountsOrThrow(_containingType.ContainingPEModule.Module, _handle, out parameterCount, out typeParameterCount);
						return parameterCount;
					}
					catch (BadImageFormatException ex)
					{
						ProjectData.SetProjectError(ex);
						BadImageFormatException ex2 = ex;
						int length = Parameters.Length;
						ProjectData.ClearProjectError();
						return length;
					}
				}
				return _lazySignature.Parameters.Length;
			}
		}

		public override ImmutableArray<ParameterSymbol> Parameters => Signature.Parameters;

		public override bool ReturnsByRef => Signature.ReturnParam.IsByRef;

		public override TypeSymbol ReturnType => Signature.ReturnParam.Type;

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => Signature.ReturnParam.CustomModifiers;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => Signature.ReturnParam.RefCustomModifiers;

		internal PEParameterSymbol ReturnParam => Signature.ReturnParam;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters
		{
			get
			{
				DiagnosticInfo errorInfo = null;
				EnsureTypeParametersAreLoaded(ref errorInfo);
				ImmutableArray<TypeParameterSymbol> result = EnsureTypeParametersAreLoaded(ref errorInfo);
				if (errorInfo != null)
				{
					InitializeUseSiteInfo(new UseSiteInfo<AssemblySymbol>(errorInfo));
				}
				return result;
			}
		}

		public override ImmutableArray<TypeSymbol> TypeArguments
		{
			get
			{
				if (IsGenericMethod)
				{
					return StaticCast<TypeSymbol>.From(TypeParameters);
				}
				return ImmutableArray<TypeSymbol>.Empty;
			}
		}

		internal override CallingConvention CallingConvention => (CallingConvention)Signature.Header.RawValue;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
		{
			get
			{
				if (!_lazyExplicitMethodImplementations.IsDefault)
				{
					return _lazyExplicitMethodImplementations;
				}
				ImmutableArray<MethodSymbol> explicitlyOverriddenMethods = new MetadataDecoder(_containingType.ContainingPEModule, _containingType).GetExplicitlyOverriddenMethods(_containingType.Handle, _handle, ContainingType);
				bool flag = false;
				ImmutableArray<MethodSymbol>.Enumerator enumerator = explicitlyOverriddenMethods.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.ContainingType.IsInterface)
					{
						flag = true;
						break;
					}
				}
				ImmutableArray<MethodSymbol> initializedValue = explicitlyOverriddenMethods;
				if (flag)
				{
					ArrayBuilder<MethodSymbol> instance = ArrayBuilder<MethodSymbol>.GetInstance();
					ImmutableArray<MethodSymbol>.Enumerator enumerator2 = explicitlyOverriddenMethods.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						MethodSymbol current = enumerator2.Current;
						if (current.ContainingType.IsInterface)
						{
							instance.Add(current);
						}
					}
					initializedValue = instance.ToImmutableAndFree();
				}
				return InterlockedOperations.Initialize(ref _lazyExplicitMethodImplementations, initializedValue);
			}
		}

		internal override SyntaxNode Syntax => null;

		internal override ObsoleteAttributeData ObsoleteAttributeData
		{
			get
			{
				if (!_packedFlags.IsObsoleteAttributePopulated)
				{
					ObsoleteAttributeData obsoleteAttributeData = ObsoleteAttributeHelpers.GetObsoleteDataFromMetadata(_handle, (PEModuleSymbol)ContainingModule);
					if (obsoleteAttributeData != null)
					{
						obsoleteAttributeData = InterlockedOperations.Initialize(ref AccessUncommonFields()._lazyObsoleteAttributeData, obsoleteAttributeData, ObsoleteAttributeData.Uninitialized);
					}
					_packedFlags.SetIsObsoleteAttributePopulated();
					return obsoleteAttributeData;
				}
				UncommonFields uncommonFields = _uncommonFields;
				if (uncommonFields == null)
				{
					return null;
				}
				ObsoleteAttributeData lazyObsoleteAttributeData = uncommonFields._lazyObsoleteAttributeData;
				return (lazyObsoleteAttributeData == ObsoleteAttributeData.Uninitialized) ? InterlockedOperations.Initialize(ref uncommonFields._lazyObsoleteAttributeData, null, ObsoleteAttributeData.Uninitialized) : lazyObsoleteAttributeData;
			}
		}

		internal override VisualBasicCompilation DeclaringCompilation => null;

		internal override bool GenerateDebugInfoImpl => false;

		private UncommonFields CreateUncommonFields()
		{
			UncommonFields uncommonFields = new UncommonFields();
			if (!_packedFlags.IsObsoleteAttributePopulated)
			{
				uncommonFields._lazyObsoleteAttributeData = ObsoleteAttributeData.Uninitialized;
			}
			if (_packedFlags.IsCustomAttributesPopulated)
			{
				uncommonFields._lazyCustomAttributes = ImmutableArray<VisualBasicAttributeData>.Empty;
			}
			if (_packedFlags.IsConditionalPopulated)
			{
				uncommonFields._lazyConditionalAttributeSymbols = ImmutableArray<string>.Empty;
			}
			return uncommonFields;
		}

		private UncommonFields AccessUncommonFields()
		{
			return _uncommonFields ?? InterlockedOperations.Initialize(ref _uncommonFields, CreateUncommonFields());
		}

		internal PEMethodSymbol(PEModuleSymbol moduleSymbol, PENamedTypeSymbol containingType, MethodDefinitionHandle handle)
		{
			_handle = handle;
			_containingType = containingType;
			try
			{
				moduleSymbol.Module.GetMethodDefPropsOrThrow(handle, out _name, out var implFlags, out var flags, out var _);
				_implFlags = (ushort)implFlags;
				_flags = (ushort)flags;
			}
			catch (BadImageFormatException ex)
			{
				ProjectData.SetProjectError(ex);
				BadImageFormatException ex2 = ex;
				if (_name == null)
				{
					_name = string.Empty;
				}
				InitializeUseSiteInfo(new UseSiteInfo<AssemblySymbol>(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedMethod1, CustomSymbolDisplayFormatter.ShortErrorName(this))));
				ProjectData.ClearProjectError();
			}
		}

		private MethodKind ComputeMethodKind()
		{
			string name = Name;
			if (HasSpecialName)
			{
				if (name.StartsWith(".", StringComparison.Ordinal))
				{
					if ((_flags & 0x1040) == 4096 && string.Equals(name, IsShared ? ".cctor" : ".ctor", StringComparison.Ordinal) && IsSub && Arity == 0)
					{
						if (!IsShared)
						{
							return MethodKind.Constructor;
						}
						if (Parameters.Length == 0)
						{
							return MethodKind.StaticConstructor;
						}
					}
					return MethodKind.Ordinary;
				}
				if (IsShared && DeclaredAccessibility == Accessibility.Public && !IsSub && Arity == 0)
				{
					OverloadResolution.OperatorInfo operatorInfo = OverloadResolution.GetOperatorInfo(name);
					if (operatorInfo.ParamCount != 0 && OverloadResolution.ValidateOverloadedOperator(this, operatorInfo))
					{
						return ComputeMethodKindForPotentialOperatorOrConversion(operatorInfo);
					}
					return MethodKind.Ordinary;
				}
			}
			if (!IsShared && string.Equals(name, "Invoke", StringComparison.Ordinal) && _containingType.TypeKind == TypeKind.Delegate)
			{
				return MethodKind.DelegateInvoke;
			}
			return MethodKind.Ordinary;
		}

		internal override bool IsParameterlessConstructor()
		{
			if (_packedFlags.MethodKindIsPopulated)
			{
				return _packedFlags.MethodKind == MethodKind.Constructor && ParameterCount == 0;
			}
			if ((_flags & 0x1850) == 6144 && string.Equals(Name, ".ctor", StringComparison.Ordinal) && ParameterCount == 0 && IsSub && Arity == 0)
			{
				_packedFlags.MethodKind = MethodKind.Constructor;
				return true;
			}
			return false;
		}

		private MethodKind ComputeMethodKindForPotentialOperatorOrConversion(OverloadResolution.OperatorInfo opInfo)
		{
			if (opInfo.IsUnary)
			{
				switch (opInfo.UnaryOperatorKind)
				{
				case UnaryOperatorKind.Implicit:
					return ComputeMethodKindForPotentialOperatorOrConversion(opInfo, MethodKind.Conversion, "op_Explicit", adjustContendersOfAdditionalName: true);
				case UnaryOperatorKind.Explicit:
					return ComputeMethodKindForPotentialOperatorOrConversion(opInfo, MethodKind.Conversion, "op_Implicit", adjustContendersOfAdditionalName: true);
				case UnaryOperatorKind.Plus:
				case UnaryOperatorKind.Minus:
				case UnaryOperatorKind.IsTrue:
				case UnaryOperatorKind.IsFalse:
					return ComputeMethodKindForPotentialOperatorOrConversion(opInfo, MethodKind.UserDefinedOperator, null, adjustContendersOfAdditionalName: false);
				case UnaryOperatorKind.Not:
					if (CaseInsensitiveComparison.Equals(Name, "op_OnesComplement"))
					{
						return ComputeMethodKindForPotentialOperatorOrConversion(opInfo, MethodKind.UserDefinedOperator, null, adjustContendersOfAdditionalName: false);
					}
					return ComputeMethodKindForPotentialOperatorOrConversion(opInfo, MethodKind.UserDefinedOperator, "op_OnesComplement", adjustContendersOfAdditionalName: false);
				default:
					throw ExceptionUtilities.UnexpectedValue(opInfo.UnaryOperatorKind);
				}
			}
			switch (opInfo.BinaryOperatorKind)
			{
			case BinaryOperatorKind.Add:
			case BinaryOperatorKind.Concatenate:
			case BinaryOperatorKind.Like:
			case BinaryOperatorKind.Equals:
			case BinaryOperatorKind.NotEquals:
			case BinaryOperatorKind.LessThanOrEqual:
			case BinaryOperatorKind.GreaterThanOrEqual:
			case BinaryOperatorKind.LessThan:
			case BinaryOperatorKind.GreaterThan:
			case BinaryOperatorKind.Subtract:
			case BinaryOperatorKind.Multiply:
			case BinaryOperatorKind.Power:
			case BinaryOperatorKind.Divide:
			case BinaryOperatorKind.Modulo:
			case BinaryOperatorKind.IntegerDivide:
			case BinaryOperatorKind.Xor:
				return ComputeMethodKindForPotentialOperatorOrConversion(opInfo, MethodKind.UserDefinedOperator, null, adjustContendersOfAdditionalName: false);
			case BinaryOperatorKind.And:
				if (CaseInsensitiveComparison.Equals(Name, "op_BitwiseAnd"))
				{
					return ComputeMethodKindForPotentialOperatorOrConversion(opInfo, MethodKind.UserDefinedOperator, null, adjustContendersOfAdditionalName: false);
				}
				return ComputeMethodKindForPotentialOperatorOrConversion(opInfo, MethodKind.UserDefinedOperator, "op_BitwiseAnd", adjustContendersOfAdditionalName: false);
			case BinaryOperatorKind.Or:
				if (CaseInsensitiveComparison.Equals(Name, "op_BitwiseOr"))
				{
					return ComputeMethodKindForPotentialOperatorOrConversion(opInfo, MethodKind.UserDefinedOperator, null, adjustContendersOfAdditionalName: false);
				}
				return ComputeMethodKindForPotentialOperatorOrConversion(opInfo, MethodKind.UserDefinedOperator, "op_BitwiseOr", adjustContendersOfAdditionalName: false);
			case BinaryOperatorKind.LeftShift:
				if (CaseInsensitiveComparison.Equals(Name, "op_LeftShift"))
				{
					return ComputeMethodKindForPotentialOperatorOrConversion(opInfo, MethodKind.UserDefinedOperator, null, adjustContendersOfAdditionalName: false);
				}
				return ComputeMethodKindForPotentialOperatorOrConversion(opInfo, MethodKind.UserDefinedOperator, "op_LeftShift", adjustContendersOfAdditionalName: false);
			case BinaryOperatorKind.RightShift:
				if (CaseInsensitiveComparison.Equals(Name, "op_RightShift"))
				{
					return ComputeMethodKindForPotentialOperatorOrConversion(opInfo, MethodKind.UserDefinedOperator, null, adjustContendersOfAdditionalName: false);
				}
				return ComputeMethodKindForPotentialOperatorOrConversion(opInfo, MethodKind.UserDefinedOperator, "op_RightShift", adjustContendersOfAdditionalName: false);
			default:
				throw ExceptionUtilities.UnexpectedValue(opInfo.BinaryOperatorKind);
			}
		}

		private bool IsPotentialOperatorOrConversion(OverloadResolution.OperatorInfo opInfo)
		{
			if (HasSpecialName && IsShared && DeclaredAccessibility == Accessibility.Public && !IsSub && Arity == 0)
			{
				return ParameterCount == opInfo.ParamCount;
			}
			return false;
		}

		private MethodKind ComputeMethodKindForPotentialOperatorOrConversion(OverloadResolution.OperatorInfo opInfo, MethodKind potentialMethodKind, string additionalNameOpt, bool adjustContendersOfAdditionalName)
		{
			MethodKind result = potentialMethodKind;
			ImmutableArray<ParameterSymbol> parameters = Parameters;
			TypeSymbol returnType = ReturnType;
			int num = ((additionalNameOpt != null) ? 1 : 0);
			for (int i = 0; i <= num; i++)
			{
				ImmutableArray<Symbol>.Enumerator enumerator = _containingType.GetMembers((i == 0) ? Name : additionalNameOpt).GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if ((object)current == this || current.Kind != SymbolKind.Method || !(current is PEMethodSymbol pEMethodSymbol) || !pEMethodSymbol.IsPotentialOperatorOrConversion(opInfo))
					{
						continue;
					}
					if (pEMethodSymbol._packedFlags.MethodKindIsPopulated)
					{
						MethodKind methodKind = pEMethodSymbol._packedFlags.MethodKind;
						if (methodKind != MethodKind.Ordinary && (methodKind != potentialMethodKind || i == 0 || adjustContendersOfAdditionalName))
						{
							continue;
						}
					}
					if (potentialMethodKind == MethodKind.Conversion && !TypeSymbolExtensions.IsSameTypeIgnoringAll(returnType, pEMethodSymbol.ReturnType))
					{
						continue;
					}
					int num2 = parameters.Length - 1;
					int j;
					for (j = 0; j <= num2 && TypeSymbolExtensions.IsSameTypeIgnoringAll(parameters[j].Type, pEMethodSymbol.Parameters[j].Type); j++)
					{
					}
					if (j >= parameters.Length)
					{
						result = MethodKind.Ordinary;
						if (i == 0 || adjustContendersOfAdditionalName)
						{
							pEMethodSymbol._packedFlags.InitializeMethodKind(MethodKind.Ordinary);
						}
					}
				}
			}
			return result;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			if (!_packedFlags.IsCustomAttributesPopulated)
			{
				ImmutableArray<VisualBasicAttributeData> lazyCustomAttributes = default(ImmutableArray<VisualBasicAttributeData>);
				((PEModuleSymbol)ContainingModule).LoadCustomAttributes(Handle, ref lazyCustomAttributes);
				if (!lazyCustomAttributes.IsEmpty)
				{
					lazyCustomAttributes = InterlockedOperations.Initialize(ref AccessUncommonFields()._lazyCustomAttributes, lazyCustomAttributes);
				}
				_packedFlags.SetIsCustomAttributesPopulated();
				return lazyCustomAttributes;
			}
			UncommonFields uncommonFields = _uncommonFields;
			if (uncommonFields == null)
			{
				return ImmutableArray<VisualBasicAttributeData>.Empty;
			}
			ImmutableArray<VisualBasicAttributeData> lazyCustomAttributes2 = uncommonFields._lazyCustomAttributes;
			return lazyCustomAttributes2.IsDefault ? InterlockedOperations.Initialize(ref uncommonFields._lazyCustomAttributes, ImmutableArray<VisualBasicAttributeData>.Empty) : lazyCustomAttributes2;
		}

		internal override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState)
		{
			return GetAttributes();
		}

		public override DllImportData GetDllImportData()
		{
			if ((_flags & 0x2000) == 0)
			{
				return null;
			}
			return _containingType.ContainingPEModule.Module.GetDllImportData(_handle);
		}

		internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
		{
			return (_flags & 0x100) != 0;
		}

		internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetReturnTypeAttributes()
		{
			return Signature.ReturnParam.GetAttributes();
		}

		internal bool SetAssociatedProperty(PEPropertySymbol propertySymbol, MethodKind methodKind)
		{
			return SetAssociatedPropertyOrEvent(propertySymbol, methodKind);
		}

		internal bool SetAssociatedEvent(PEEventSymbol eventSymbol, MethodKind methodKind)
		{
			return SetAssociatedPropertyOrEvent(eventSymbol, methodKind);
		}

		private bool SetAssociatedPropertyOrEvent(Symbol propertyOrEventSymbol, MethodKind methodKind)
		{
			if ((object)_associatedPropertyOrEventOpt == null)
			{
				_associatedPropertyOrEventOpt = propertyOrEventSymbol;
				_packedFlags.MethodKind = methodKind;
				return true;
			}
			return false;
		}

		private SignatureData LoadSignature()
		{
			PEModuleSymbol containingPEModule = _containingType.ContainingPEModule;
			BadImageFormatException metadataException = null;
			SignatureHeader signatureHeader;
			ParamInfo<TypeSymbol>[] signatureForMethod = new MetadataDecoder(containingPEModule, this).GetSignatureForMethod(_handle, out signatureHeader, out metadataException);
			if (!signatureHeader.IsGeneric && _lazyTypeParameters.IsDefault)
			{
				ImmutableInterlocked.InterlockedInitialize(ref _lazyTypeParameters, ImmutableArray<TypeParameterSymbol>.Empty);
			}
			int num = signatureForMethod.Length - 1;
			bool flag = false;
			bool isBad;
			ImmutableArray<ParameterSymbol> parameters;
			if (num > 0)
			{
				ImmutableArray<ParameterSymbol>.Builder builder = ImmutableArray.CreateBuilder<ParameterSymbol>(num);
				int num2 = num - 1;
				for (int i = 0; i <= num2; i++)
				{
					builder.Add(PEParameterSymbol.Create(containingPEModule, this, i, ref signatureForMethod[i + 1], out isBad));
					if (isBad)
					{
						flag = true;
					}
				}
				parameters = builder.ToImmutable();
			}
			else
			{
				parameters = ImmutableArray<ParameterSymbol>.Empty;
			}
			PEParameterSymbol returnParam = PEParameterSymbol.Create(containingPEModule, this, 0, ref signatureForMethod[0], out isBad);
			if (metadataException != null || flag || isBad)
			{
				InitializeUseSiteInfo(new UseSiteInfo<AssemblySymbol>(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedMethod1, CustomSymbolDisplayFormatter.ShortErrorName(this))));
			}
			SignatureData value = new SignatureData(signatureHeader, parameters, returnParam);
			return InterlockedOperations.Initialize(ref _lazySignature, value);
		}

		private ImmutableArray<TypeParameterSymbol> EnsureTypeParametersAreLoaded(ref DiagnosticInfo errorInfo)
		{
			ImmutableArray<TypeParameterSymbol> lazyTypeParameters = _lazyTypeParameters;
			if (!lazyTypeParameters.IsDefault)
			{
				return lazyTypeParameters;
			}
			return InterlockedOperations.Initialize(ref _lazyTypeParameters, LoadTypeParameters(ref errorInfo));
		}

		private ImmutableArray<TypeParameterSymbol> LoadTypeParameters(ref DiagnosticInfo errorInfo)
		{
			try
			{
				PEModuleSymbol containingPEModule = _containingType.ContainingPEModule;
				GenericParameterHandleCollection genericParametersForMethodOrThrow = containingPEModule.Module.GetGenericParametersForMethodOrThrow(_handle);
				if (genericParametersForMethodOrThrow.Count == 0)
				{
					return ImmutableArray<TypeParameterSymbol>.Empty;
				}
				ImmutableArray<TypeParameterSymbol>.Builder builder = ImmutableArray.CreateBuilder<TypeParameterSymbol>(genericParametersForMethodOrThrow.Count);
				int num = genericParametersForMethodOrThrow.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					builder.Add(new PETypeParameterSymbol(containingPEModule, this, (ushort)i, genericParametersForMethodOrThrow[i]));
				}
				return builder.ToImmutable();
			}
			catch (BadImageFormatException ex)
			{
				ProjectData.SetProjectError(ex);
				BadImageFormatException ex2 = ex;
				errorInfo = ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedMethod1, CustomSymbolDisplayFormatter.ShortErrorName(this));
				ImmutableArray<TypeParameterSymbol> empty = ImmutableArray<TypeParameterSymbol>.Empty;
				ProjectData.ClearProjectError();
				return empty;
			}
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return PEDocumentationCommentUtils.GetDocumentationComment(this, _containingType.ContainingPEModule, preferredCulture, cancellationToken, ref AccessUncommonFields()._lazyDocComment);
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			if (!_packedFlags.IsUseSiteDiagnosticPopulated)
			{
				UseSiteInfo<AssemblySymbol> useSiteInfo = CalculateUseSiteInfo();
				DiagnosticInfo errorInfo = useSiteInfo.DiagnosticInfo;
				EnsureTypeParametersAreLoaded(ref errorInfo);
				CheckUnmanagedCallersOnly(ref errorInfo);
				return InitializeUseSiteInfo(useSiteInfo.AdjustDiagnosticInfo(errorInfo));
			}
			return GetCachedUseSiteInfo();
		}

		private UseSiteInfo<AssemblySymbol> GetCachedUseSiteInfo()
		{
			return (_uncommonFields?._lazyCachedUseSiteInfo ?? default(CachedUseSiteInfo<AssemblySymbol>)).ToUseSiteInfo(base.PrimaryDependency);
		}

		private void CheckUnmanagedCallersOnly(ref DiagnosticInfo errorInfo)
		{
			if ((errorInfo == null || errorInfo.Code != 30657) && ((PEModuleSymbol)ContainingModule).Module.FindTargetAttribute(_handle, AttributeDescription.UnmanagedCallersOnlyAttribute).HasValue)
			{
				errorInfo = ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedMethod1, CustomSymbolDisplayFormatter.ShortErrorName(this));
			}
		}

		private UseSiteInfo<AssemblySymbol> InitializeUseSiteInfo(UseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (_packedFlags.IsUseSiteDiagnosticPopulated)
			{
				return GetCachedUseSiteInfo();
			}
			if (useSiteInfo.DiagnosticInfo != null || !useSiteInfo.SecondaryDependencies.IsNullOrEmpty())
			{
				useSiteInfo = AccessUncommonFields()._lazyCachedUseSiteInfo.InterlockedInitialize(base.PrimaryDependency, useSiteInfo);
			}
			_packedFlags.SetIsUseSiteDiagnosticPopulated();
			return useSiteInfo;
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			if (!_packedFlags.IsConditionalPopulated)
			{
				ImmutableArray<string> immutableArray = _containingType.ContainingPEModule.Module.GetConditionalAttributeValues(_handle);
				if (!immutableArray.IsEmpty)
				{
					immutableArray = InterlockedOperations.Initialize(ref AccessUncommonFields()._lazyConditionalAttributeSymbols, immutableArray);
				}
				_packedFlags.SetIsConditionalAttributePopulated();
				return immutableArray;
			}
			UncommonFields uncommonFields = _uncommonFields;
			if (uncommonFields == null)
			{
				return ImmutableArray<string>.Empty;
			}
			ImmutableArray<string> lazyConditionalAttributeSymbols = uncommonFields._lazyConditionalAttributeSymbols;
			return lazyConditionalAttributeSymbols.IsDefault ? InterlockedOperations.Initialize(ref uncommonFields._lazyConditionalAttributeSymbols, ImmutableArray<string>.Empty) : lazyConditionalAttributeSymbols;
		}

		internal override bool TryGetMeParameter(out ParameterSymbol meParameter)
		{
			if (IsShared)
			{
				meParameter = null;
			}
			else
			{
				meParameter = _uncommonFields?._lazyMeParameter ?? InterlockedOperations.Initialize(ref AccessUncommonFields()._lazyMeParameter, new MeParameterSymbol(this));
			}
			return true;
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
