using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
{
	internal sealed class RetargetingPropertySymbol : PropertySymbol
	{
		private readonly RetargetingModuleSymbol _retargetingModule;

		private readonly PropertySymbol _underlyingProperty;

		private ImmutableArray<ParameterSymbol> _lazyParameters;

		private CustomModifiersTuple _lazyCustomModifiers;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		private ImmutableArray<PropertySymbol> _lazyExplicitInterfaceImplementations;

		private CachedUseSiteInfo<AssemblySymbol> _lazyCachedUseSiteInfo;

		private RetargetingModuleSymbol.RetargetingSymbolTranslator RetargetingTranslator => _retargetingModule.RetargetingTranslator;

		public PropertySymbol UnderlyingProperty => _underlyingProperty;

		public RetargetingModuleSymbol RetargetingModule => _retargetingModule;

		public override bool IsImplicitlyDeclared => _underlyingProperty.IsImplicitlyDeclared;

		public override bool IsWithEvents => _underlyingProperty.IsWithEvents;

		public override Symbol ContainingSymbol => RetargetingTranslator.Retarget(_underlyingProperty.ContainingSymbol);

		public override Accessibility DeclaredAccessibility => _underlyingProperty.DeclaredAccessibility;

		public override MethodSymbol GetMethod
		{
			get
			{
				if ((object)_underlyingProperty.GetMethod != null)
				{
					return RetargetingTranslator.Retarget(_underlyingProperty.GetMethod);
				}
				return null;
			}
		}

		public override MethodSymbol SetMethod
		{
			get
			{
				if ((object)_underlyingProperty.SetMethod != null)
				{
					return RetargetingTranslator.Retarget(_underlyingProperty.SetMethod);
				}
				return null;
			}
		}

		internal override FieldSymbol AssociatedField
		{
			get
			{
				if ((object)_underlyingProperty.AssociatedField != null)
				{
					return RetargetingTranslator.Retarget(_underlyingProperty.AssociatedField);
				}
				return null;
			}
		}

		public override bool IsDefault => _underlyingProperty.IsDefault;

		public override bool IsMustOverride => _underlyingProperty.IsMustOverride;

		public override bool IsNotOverridable => _underlyingProperty.IsNotOverridable;

		public override bool IsOverridable => _underlyingProperty.IsOverridable;

		public override bool IsOverrides => _underlyingProperty.IsOverrides;

		public override bool IsOverloads => _underlyingProperty.IsOverloads;

		public override bool IsShared => _underlyingProperty.IsShared;

		public override string Name => _underlyingProperty.Name;

		public override string MetadataName => _underlyingProperty.MetadataName;

		internal override bool HasSpecialName => _underlyingProperty.HasSpecialName;

		internal override ObsoleteAttributeData ObsoleteAttributeData => _underlyingProperty.ObsoleteAttributeData;

		public override ImmutableArray<Location> Locations => _underlyingProperty.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _underlyingProperty.DeclaringSyntaxReferences;

		public override ImmutableArray<ParameterSymbol> Parameters
		{
			get
			{
				if (_lazyParameters.IsDefault)
				{
					ImmutableInterlocked.InterlockedCompareExchange(ref _lazyParameters, RetargetParameters(), default(ImmutableArray<ParameterSymbol>));
				}
				return _lazyParameters;
			}
		}

		public override int ParameterCount => _underlyingProperty.ParameterCount;

		public override bool ReturnsByRef => _underlyingProperty.ReturnsByRef;

		public override TypeSymbol Type => RetargetingTranslator.Retarget(_underlyingProperty.Type, RetargetOptions.RetargetPrimitiveTypesByTypeCode);

		public override ImmutableArray<CustomModifier> TypeCustomModifiers => CustomModifiersTuple.TypeCustomModifiers;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => CustomModifiersTuple.RefCustomModifiers;

		private CustomModifiersTuple CustomModifiersTuple => RetargetingTranslator.RetargetModifiers(_underlyingProperty.TypeCustomModifiers, _underlyingProperty.RefCustomModifiers, ref _lazyCustomModifiers);

		internal override CallingConvention CallingConvention => _underlyingProperty.CallingConvention;

		public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations
		{
			get
			{
				if (_lazyExplicitInterfaceImplementations.IsDefault)
				{
					ImmutableInterlocked.InterlockedCompareExchange(ref _lazyExplicitInterfaceImplementations, RetargetExplicitInterfaceImplementations(), default(ImmutableArray<PropertySymbol>));
				}
				return _lazyExplicitInterfaceImplementations;
			}
		}

		internal override bool IsMyGroupCollectionProperty => _underlyingProperty.IsMyGroupCollectionProperty;

		internal override bool HasRuntimeSpecialName => _underlyingProperty.HasRuntimeSpecialName;

		internal override VisualBasicCompilation DeclaringCompilation => null;

		public RetargetingPropertySymbol(RetargetingModuleSymbol retargetingModule, PropertySymbol underlyingProperty)
		{
			_lazyCachedUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;
			if (underlyingProperty is RetargetingPropertySymbol)
			{
				throw new ArgumentException();
			}
			_retargetingModule = retargetingModule;
			_underlyingProperty = underlyingProperty;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return RetargetingTranslator.GetRetargetedAttributes(_underlyingProperty, ref _lazyCustomAttributes);
		}

		internal override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState)
		{
			return RetargetingTranslator.RetargetAttributes(_underlyingProperty.GetCustomAttributesToEmit(compilationState));
		}

		private ImmutableArray<ParameterSymbol> RetargetParameters()
		{
			ImmutableArray<ParameterSymbol> parameters = _underlyingProperty.Parameters;
			int length = parameters.Length;
			if (length == 0)
			{
				return ImmutableArray<ParameterSymbol>.Empty;
			}
			ParameterSymbol[] array = new ParameterSymbol[length - 1 + 1];
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = RetargetingParameterSymbol.CreatePropertyParameter(this, parameters[i]);
			}
			return array.AsImmutableOrNull();
		}

		private ImmutableArray<PropertySymbol> RetargetExplicitInterfaceImplementations()
		{
			ImmutableArray<PropertySymbol> explicitInterfaceImplementations = UnderlyingProperty.ExplicitInterfaceImplementations;
			if (explicitInterfaceImplementations.IsEmpty)
			{
				return explicitInterfaceImplementations;
			}
			ArrayBuilder<PropertySymbol> instance = ArrayBuilder<PropertySymbol>.GetInstance();
			int num = explicitInterfaceImplementations.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				PropertySymbol propertySymbol = RetargetingModule.RetargetingTranslator.Retarget(explicitInterfaceImplementations[i], PropertySignatureComparer.RetargetedExplicitPropertyImplementationComparer);
				if ((object)propertySymbol != null)
				{
					instance.Add(propertySymbol);
				}
			}
			return instance.ToImmutableAndFree();
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

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _underlyingProperty.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}
	}
}
