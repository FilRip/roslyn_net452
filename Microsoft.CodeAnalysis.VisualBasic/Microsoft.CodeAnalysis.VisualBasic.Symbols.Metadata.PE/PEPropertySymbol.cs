using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal class PEPropertySymbol : PropertySymbol
	{
		private sealed class PEPropertySymbolWithCustomModifiers : PEPropertySymbol
		{
			private readonly ImmutableArray<CustomModifier> _typeCustomModifiers;

			private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

			public override ImmutableArray<CustomModifier> TypeCustomModifiers => _typeCustomModifiers;

			public override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

			public PEPropertySymbolWithCustomModifiers(PEModuleSymbol moduleSymbol, PENamedTypeSymbol containingType, PropertyDefinitionHandle handle, PEMethodSymbol getMethod, PEMethodSymbol setMethod, MetadataDecoder metadataDecoder, SignatureHeader signatureHeader, ParamInfo<TypeSymbol>[] propertyParams)
				: base(moduleSymbol, containingType, handle, getMethod, setMethod, metadataDecoder, signatureHeader, propertyParams)
			{
				ParamInfo<TypeSymbol> paramInfo = propertyParams[0];
				_typeCustomModifiers = VisualBasicCustomModifier.Convert(paramInfo.CustomModifiers);
				_refCustomModifiers = VisualBasicCustomModifier.Convert(paramInfo.RefCustomModifiers);
			}
		}

		private readonly string _name;

		private readonly PropertyAttributes _flags;

		private readonly PENamedTypeSymbol _containingType;

		private readonly SignatureHeader _signatureHeader;

		private readonly ImmutableArray<ParameterSymbol> _parameters;

		private readonly bool _returnsByRef;

		private readonly TypeSymbol _propertyType;

		private readonly PEMethodSymbol _getMethod;

		private readonly PEMethodSymbol _setMethod;

		private readonly PropertyDefinitionHandle _handle;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		private Tuple<CultureInfo, string> _lazyDocComment;

		private CachedUseSiteInfo<AssemblySymbol> _lazyCachedUseSiteInfo;

		private int _isWithEvents;

		private const int s_unsetAccessibility = -1;

		private int _lazyDeclaredAccessibility;

		private ObsoleteAttributeData _lazyObsoleteAttributeData;

		public override Symbol ContainingSymbol => _containingType;

		public override NamedTypeSymbol ContainingType => _containingType;

		public override string Name => _name;

		internal PropertyAttributes PropertyFlags => _flags;

		internal override bool HasSpecialName => (_flags & PropertyAttributes.SpecialName) != 0;

		public override Accessibility DeclaredAccessibility
		{
			get
			{
				if (_lazyDeclaredAccessibility == -1)
				{
					Interlocked.CompareExchange(ref _lazyDeclaredAccessibility, (int)GetDeclaredAccessibility(this), -1);
				}
				return (Accessibility)_lazyDeclaredAccessibility;
			}
		}

		public override bool IsMustOverride
		{
			get
			{
				if ((object)_getMethod == null || !_getMethod.IsMustOverride)
				{
					if ((object)_setMethod != null)
					{
						return _setMethod.IsMustOverride;
					}
					return false;
				}
				return true;
			}
		}

		public override bool IsNotOverridable
		{
			get
			{
				if ((object)_getMethod == null || _getMethod.IsNotOverridable)
				{
					if ((object)_setMethod != null)
					{
						return _setMethod.IsNotOverridable;
					}
					return true;
				}
				return false;
			}
		}

		public override bool IsOverridable
		{
			get
			{
				if (!IsMustOverride && !IsOverrides)
				{
					if ((object)_getMethod == null || !_getMethod.IsOverridable)
					{
						if ((object)_setMethod != null)
						{
							return _setMethod.IsOverridable;
						}
						return false;
					}
					return true;
				}
				return false;
			}
		}

		public override bool IsOverrides
		{
			get
			{
				if ((object)_getMethod == null || !_getMethod.IsOverrides)
				{
					if ((object)_setMethod != null)
					{
						return _setMethod.IsOverrides;
					}
					return false;
				}
				return true;
			}
		}

		public override bool IsOverloads
		{
			get
			{
				if ((object)_getMethod == null || !_getMethod.IsOverloads)
				{
					if ((object)_setMethod != null)
					{
						return _setMethod.IsOverloads;
					}
					return false;
				}
				return true;
			}
		}

		public override bool IsShared
		{
			get
			{
				if ((object)_getMethod == null || _getMethod.IsShared)
				{
					if ((object)_setMethod != null)
					{
						return _setMethod.IsShared;
					}
					return true;
				}
				return false;
			}
		}

		public override bool IsDefault
		{
			get
			{
				string defaultPropertyName = _containingType.DefaultPropertyName;
				if (!string.IsNullOrEmpty(defaultPropertyName))
				{
					return CaseInsensitiveComparison.Equals(defaultPropertyName, _name);
				}
				return false;
			}
		}

		public override bool IsWithEvents
		{
			get
			{
				if (_isWithEvents == 0)
				{
					SetIsWithEvents(base.IsWithEvents);
				}
				return _isWithEvents == 2;
			}
		}

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		public override bool ReturnsByRef => _returnsByRef;

		public override TypeSymbol Type => _propertyType;

		public override ImmutableArray<CustomModifier> TypeCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override MethodSymbol GetMethod => _getMethod;

		public override MethodSymbol SetMethod => _setMethod;

		internal override FieldSymbol AssociatedField => null;

		internal override CallingConvention CallingConvention => (CallingConvention)_signatureHeader.RawValue;

		public override ImmutableArray<Location> Locations => _containingType.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		internal override ObsoleteAttributeData ObsoleteAttributeData
		{
			get
			{
				ObsoleteAttributeHelpers.InitializeObsoleteDataFromMetadata(ref _lazyObsoleteAttributeData, _handle, (PEModuleSymbol)ContainingModule);
				return _lazyObsoleteAttributeData;
			}
		}

		public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations
		{
			get
			{
				if (((object)_getMethod == null || _getMethod.ExplicitInterfaceImplementations.Length == 0) && ((object)_setMethod == null || _setMethod.ExplicitInterfaceImplementations.Length == 0))
				{
					return ImmutableArray<PropertySymbol>.Empty;
				}
				ISet<PropertySymbol> propertiesForExplicitlyImplementedAccessor = PEPropertyOrEventHelpers.GetPropertiesForExplicitlyImplementedAccessor(_getMethod);
				ISet<PropertySymbol> propertiesForExplicitlyImplementedAccessor2 = PEPropertyOrEventHelpers.GetPropertiesForExplicitlyImplementedAccessor(_setMethod);
				ArrayBuilder<PropertySymbol> instance = ArrayBuilder<PropertySymbol>.GetInstance();
				foreach (PropertySymbol item in propertiesForExplicitlyImplementedAccessor)
				{
					MethodSymbol setMethod = item.SetMethod;
					if ((object)setMethod == null || !SymbolExtensions.RequiresImplementation(setMethod) || propertiesForExplicitlyImplementedAccessor2.Contains(item))
					{
						instance.Add(item);
					}
				}
				foreach (PropertySymbol item2 in propertiesForExplicitlyImplementedAccessor2)
				{
					MethodSymbol getMethod = item2.GetMethod;
					if ((object)getMethod == null || !SymbolExtensions.RequiresImplementation(getMethod))
					{
						instance.Add(item2);
					}
				}
				return instance.ToImmutableAndFree();
			}
		}

		internal PropertyDefinitionHandle Handle => _handle;

		internal override bool IsMyGroupCollectionProperty => false;

		internal override bool HasRuntimeSpecialName => (_flags & PropertyAttributes.RTSpecialName) != 0;

		internal override VisualBasicCompilation DeclaringCompilation => null;

		internal static PEPropertySymbol Create(PEModuleSymbol moduleSymbol, PENamedTypeSymbol containingType, PropertyDefinitionHandle handle, PEMethodSymbol getMethod, PEMethodSymbol setMethod)
		{
			MetadataDecoder metadataDecoder = new MetadataDecoder(moduleSymbol, containingType);
			BadImageFormatException BadImageFormatException = null;
			SignatureHeader signatureHeader;
			ParamInfo<TypeSymbol>[] signatureForProperty = metadataDecoder.GetSignatureForProperty(handle, out signatureHeader, out BadImageFormatException);
			ParamInfo<TypeSymbol> paramInfo = signatureForProperty[0];
			PEPropertySymbol pEPropertySymbol = ((!paramInfo.CustomModifiers.IsDefaultOrEmpty || !paramInfo.RefCustomModifiers.IsDefaultOrEmpty) ? new PEPropertySymbolWithCustomModifiers(moduleSymbol, containingType, handle, getMethod, setMethod, metadataDecoder, signatureHeader, signatureForProperty) : new PEPropertySymbol(moduleSymbol, containingType, handle, getMethod, setMethod, metadataDecoder, signatureHeader, signatureForProperty));
			if (BadImageFormatException != null)
			{
				pEPropertySymbol._lazyCachedUseSiteInfo.Initialize(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedProperty1, CustomSymbolDisplayFormatter.QualifiedName(pEPropertySymbol)));
			}
			return pEPropertySymbol;
		}

		private PEPropertySymbol(PEModuleSymbol moduleSymbol, PENamedTypeSymbol containingType, PropertyDefinitionHandle handle, PEMethodSymbol getMethod, PEMethodSymbol setMethod, MetadataDecoder metadataDecoder, SignatureHeader signatureHeader, ParamInfo<TypeSymbol>[] propertyParams)
		{
			_lazyCachedUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;
			_isWithEvents = 0;
			_lazyDeclaredAccessibility = -1;
			_lazyObsoleteAttributeData = ObsoleteAttributeData.Uninitialized;
			_signatureHeader = signatureHeader;
			_containingType = containingType;
			_handle = handle;
			PEModule module = moduleSymbol.Module;
			BadImageFormatException ex = null;
			try
			{
				module.GetPropertyDefPropsOrThrow(handle, out _name, out _flags);
			}
			catch (BadImageFormatException ex2)
			{
				ProjectData.SetProjectError(ex2);
				ex = ex2;
				if (_name == null)
				{
					_name = string.Empty;
				}
				ProjectData.ClearProjectError();
			}
			_getMethod = getMethod;
			_setMethod = setMethod;
			SignatureHeader signatureHeader2 = default(SignatureHeader);
			BadImageFormatException metadataException = null;
			ParamInfo<TypeSymbol>[] getMethodParamsOpt = (((object)_getMethod == null) ? null : metadataDecoder.GetSignatureForMethod(_getMethod.Handle, out signatureHeader2, out metadataException));
			BadImageFormatException metadataException2 = null;
			ParamInfo<TypeSymbol>[] setMethodParamsOpt = (((object)_setMethod == null) ? null : metadataDecoder.GetSignatureForMethod(_setMethod.Handle, out signatureHeader2, out metadataException2));
			bool num = DoSignaturesMatch(metadataDecoder, propertyParams, _getMethod, getMethodParamsOpt, _setMethod, setMethodParamsOpt);
			bool parametersMatch = true;
			_parameters = GetParameters(this, _getMethod, _setMethod, propertyParams, ref parametersMatch);
			if (!num || !parametersMatch || metadataException != null || metadataException2 != null || ex != null || propertyParams.Any((ParamInfo<TypeSymbol> p) => p.RefCustomModifiers.AnyRequired() || p.CustomModifiers.AnyRequired()))
			{
				_lazyCachedUseSiteInfo.Initialize(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedProperty1, CustomSymbolDisplayFormatter.QualifiedName(this)));
			}
			if ((object)_getMethod != null)
			{
				_getMethod.SetAssociatedProperty(this, MethodKind.PropertyGet);
			}
			if ((object)_setMethod != null)
			{
				_setMethod.SetAssociatedProperty(this, MethodKind.PropertySet);
			}
			ParamInfo<TypeSymbol> paramInfo = propertyParams[0];
			_returnsByRef = paramInfo.IsByRef;
			_propertyType = paramInfo.Type;
			_propertyType = TupleTypeDecoder.DecodeTupleTypesIfApplicable(_propertyType, handle, moduleSymbol);
		}

		internal void SetIsWithEvents(bool value)
		{
			ThreeState value2 = ((!value) ? ThreeState.False : ThreeState.True);
			Interlocked.CompareExchange(ref _isWithEvents, (int)value2, 0);
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			if (_lazyCustomAttributes.IsDefault)
			{
				((PEModuleSymbol)ContainingModule).LoadCustomAttributes(_handle, ref _lazyCustomAttributes);
			}
			return _lazyCustomAttributes;
		}

		internal override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState)
		{
			return GetAttributes();
		}

		private static bool DoSignaturesMatch(MetadataDecoder metadataDecoder, ParamInfo<TypeSymbol>[] propertyParams, PEMethodSymbol getMethodOpt, ParamInfo<TypeSymbol>[] getMethodParamsOpt, PEMethodSymbol setMethodOpt, ParamInfo<TypeSymbol>[] setMethodParamsOpt)
		{
			if ((object)getMethodOpt != null)
			{
				if (!metadataDecoder.DoPropertySignaturesMatch(propertyParams, getMethodParamsOpt, comparingToSetter: false, compareParamByRef: false, compareReturnType: false))
				{
					return false;
				}
			}
			else if (!metadataDecoder.DoPropertySignaturesMatch(propertyParams, setMethodParamsOpt, comparingToSetter: true, compareParamByRef: false, compareReturnType: false))
			{
				return false;
			}
			if ((object)getMethodOpt == null || (object)setMethodOpt == null)
			{
				return true;
			}
			if (!metadataDecoder.DoPropertySignaturesMatch(getMethodParamsOpt, setMethodParamsOpt, comparingToSetter: true, compareParamByRef: true, compareReturnType: false))
			{
				return false;
			}
			if (getMethodOpt.IsMustOverride != setMethodOpt.IsMustOverride || getMethodOpt.IsNotOverridable != setMethodOpt.IsNotOverridable || getMethodOpt.IsOverrides != setMethodOpt.IsOverrides || getMethodOpt.IsShared != setMethodOpt.IsShared)
			{
				return false;
			}
			return true;
		}

		private static Accessibility GetDeclaredAccessibility(PropertySymbol property)
		{
			MethodSymbol getMethod = property.GetMethod;
			MethodSymbol setMethod = property.SetMethod;
			if ((object)getMethod == null)
			{
				return setMethod?.DeclaredAccessibility ?? Accessibility.NotApplicable;
			}
			if ((object)setMethod == null)
			{
				return getMethod.DeclaredAccessibility;
			}
			Accessibility declaredAccessibility = getMethod.DeclaredAccessibility;
			Accessibility declaredAccessibility2 = setMethod.DeclaredAccessibility;
			Accessibility num = ((declaredAccessibility > declaredAccessibility2) ? declaredAccessibility2 : declaredAccessibility);
			Accessibility accessibility = ((declaredAccessibility > declaredAccessibility2) ? declaredAccessibility : declaredAccessibility2);
			return (num == Accessibility.Protected && accessibility == Accessibility.Internal) ? Accessibility.ProtectedOrInternal : accessibility;
		}

		private static ImmutableArray<ParameterSymbol> GetParameters(PEPropertySymbol property, PEMethodSymbol getMethod, PEMethodSymbol setMethod, ParamInfo<TypeSymbol>[] propertyParams, ref bool parametersMatch)
		{
			parametersMatch = true;
			if (propertyParams.Length < 2)
			{
				return ImmutableArray<ParameterSymbol>.Empty;
			}
			ParameterSymbol[] array = new ParameterSymbol[propertyParams.Length - 2 + 1];
			int num = propertyParams.Length - 2;
			for (int i = 0; i <= num; i++)
			{
				ParamInfo<TypeSymbol> paramInfo = propertyParams[i + 1];
				PEParameterSymbol accessorParameter = GetAccessorParameter(getMethod, i);
				PEParameterSymbol accessorParameter2 = GetAccessorParameter(setMethod, i);
				PEParameterSymbol pEParameterSymbol = accessorParameter ?? accessorParameter2;
				string text = null;
				bool isByRef = false;
				ConstantValue constantValue = null;
				ParameterAttributes parameterAttributes = ParameterAttributes.None;
				ParameterHandle handle = paramInfo.Handle;
				bool isParamArray = false;
				bool flag = false;
				if ((object)pEParameterSymbol != null)
				{
					text = pEParameterSymbol.Name;
					isByRef = pEParameterSymbol.IsByRef;
					constantValue = ((ParameterSymbol)pEParameterSymbol).ExplicitDefaultConstantValue;
					parameterAttributes = pEParameterSymbol.ParamFlags;
					handle = pEParameterSymbol.Handle;
					isParamArray = pEParameterSymbol.IsParamArray;
					flag = pEParameterSymbol.HasOptionCompare;
				}
				if ((object)accessorParameter != null && (object)accessorParameter2 != null)
				{
					if ((parameterAttributes & ParameterAttributes.Optional) != (accessorParameter2.ParamFlags & ParameterAttributes.Optional))
					{
						parameterAttributes &= ~ParameterAttributes.Optional;
						constantValue = null;
					}
					else if (constantValue != ((ParameterSymbol)accessorParameter2).ExplicitDefaultConstantValue)
					{
						constantValue = null;
						parameterAttributes &= ~ParameterAttributes.Optional;
					}
					if (!CaseInsensitiveComparison.Equals(text, accessorParameter2.Name))
					{
						text = null;
					}
					if (accessorParameter2.IsByRef)
					{
						isByRef = true;
					}
					if (!accessorParameter2.IsParamArray)
					{
						isParamArray = false;
					}
					if (flag != accessorParameter2.HasOptionCompare)
					{
						flag = false;
						parametersMatch = false;
					}
				}
				array[i] = PEParameterSymbol.Create(property, text, isByRef, VisualBasicCustomModifier.Convert(paramInfo.RefCustomModifiers), paramInfo.Type, handle, parameterAttributes, isParamArray, flag, i, constantValue, VisualBasicCustomModifier.Convert(paramInfo.CustomModifiers));
			}
			return array.AsImmutableOrNull();
		}

		private static PEParameterSymbol GetAccessorParameter(PEMethodSymbol accessor, int index)
		{
			if ((object)accessor != null)
			{
				ImmutableArray<ParameterSymbol> parameters = accessor.Parameters;
				if (index < parameters.Length)
				{
					return (PEParameterSymbol)parameters[index];
				}
			}
			return null;
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
				_lazyCachedUseSiteInfo.Initialize(primaryDependency, CalculateUseSiteInfo());
			}
			return _lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency);
		}
	}
}
