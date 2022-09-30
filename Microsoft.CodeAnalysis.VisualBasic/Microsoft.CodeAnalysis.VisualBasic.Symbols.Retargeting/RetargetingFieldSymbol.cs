using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
{
	internal sealed class RetargetingFieldSymbol : FieldSymbol
	{
		private readonly RetargetingModuleSymbol _retargetingModule;

		private readonly FieldSymbol _underlyingField;

		private ImmutableArray<CustomModifier> _lazyCustomModifiers;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		private CachedUseSiteInfo<AssemblySymbol> _lazyCachedUseSiteInfo;

		private RetargetingModuleSymbol.RetargetingSymbolTranslator RetargetingTranslator => _retargetingModule.RetargetingTranslator;

		public FieldSymbol UnderlyingField => _underlyingField;

		public override bool IsImplicitlyDeclared => _underlyingField.IsImplicitlyDeclared;

		public RetargetingModuleSymbol RetargetingModule => _retargetingModule;

		public override TypeSymbol Type => RetargetingTranslator.Retarget(_underlyingField.Type, RetargetOptions.RetargetPrimitiveTypesByTypeCode);

		public override ImmutableArray<CustomModifier> CustomModifiers => RetargetingTranslator.RetargetModifiers(_underlyingField.CustomModifiers, ref _lazyCustomModifiers);

		public override Symbol ContainingSymbol => RetargetingTranslator.Retarget(_underlyingField.ContainingSymbol);

		public override Accessibility DeclaredAccessibility => _underlyingField.DeclaredAccessibility;

		public override AssemblySymbol ContainingAssembly => _retargetingModule.ContainingAssembly;

		public override ModuleSymbol ContainingModule => _retargetingModule;

		public override string Name => _underlyingField.Name;

		public override string MetadataName => _underlyingField.MetadataName;

		internal override bool HasSpecialName => _underlyingField.HasSpecialName;

		internal override bool HasRuntimeSpecialName => _underlyingField.HasRuntimeSpecialName;

		internal override bool IsNotSerialized => _underlyingField.IsNotSerialized;

		internal override bool IsMarshalledExplicitly => _underlyingField.IsMarshalledExplicitly;

		internal override MarshalPseudoCustomAttributeData MarshallingInformation => RetargetingTranslator.Retarget(UnderlyingField.MarshallingInformation);

		internal override ImmutableArray<byte> MarshallingDescriptor => _underlyingField.MarshallingDescriptor;

		internal override int? TypeLayoutOffset => _underlyingField.TypeLayoutOffset;

		public override Symbol AssociatedSymbol
		{
			get
			{
				Symbol associatedSymbol = _underlyingField.AssociatedSymbol;
				if ((object)associatedSymbol != null)
				{
					return RetargetingTranslator.Retarget(associatedSymbol);
				}
				return null;
			}
		}

		public override bool IsReadOnly => _underlyingField.IsReadOnly;

		public override bool IsConst => _underlyingField.IsConst;

		public override object ConstantValue => _underlyingField.ConstantValue;

		public override ImmutableArray<Location> Locations => _underlyingField.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _underlyingField.DeclaringSyntaxReferences;

		public override bool IsShared => _underlyingField.IsShared;

		internal override ObsoleteAttributeData ObsoleteAttributeData => _underlyingField.ObsoleteAttributeData;

		internal override VisualBasicCompilation DeclaringCompilation => null;

		public RetargetingFieldSymbol(RetargetingModuleSymbol retargetingModule, FieldSymbol underlyingField)
		{
			_lazyCachedUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;
			if (underlyingField is RetargetingFieldSymbol)
			{
				throw new ArgumentException();
			}
			_retargetingModule = retargetingModule;
			_underlyingField = underlyingField;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return RetargetingTranslator.GetRetargetedAttributes(_underlyingField, ref _lazyCustomAttributes);
		}

		internal override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState)
		{
			return RetargetingTranslator.RetargetAttributes(_underlyingField.GetCustomAttributesToEmit(compilationState));
		}

		internal override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress)
		{
			return _underlyingField.GetConstantValue(inProgress);
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
			return _underlyingField.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}
	}
}
