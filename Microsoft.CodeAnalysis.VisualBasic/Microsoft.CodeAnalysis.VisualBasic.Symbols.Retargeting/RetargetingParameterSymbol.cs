using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
{
	internal abstract class RetargetingParameterSymbol : ParameterSymbol
	{
		private sealed class RetargetingMethodParameterSymbol : RetargetingParameterSymbol
		{
			private readonly RetargetingMethodSymbol _retargetingMethod;

			protected override RetargetingModuleSymbol RetargetingModule => _retargetingMethod.RetargetingModule;

			public RetargetingMethodParameterSymbol(RetargetingMethodSymbol retargetingMethod, ParameterSymbol underlyingParameter)
				: base(underlyingParameter)
			{
				_retargetingMethod = retargetingMethod;
			}
		}

		private sealed class RetargetingPropertyParameterSymbol : RetargetingParameterSymbol
		{
			private readonly RetargetingPropertySymbol _retargetingProperty;

			protected override RetargetingModuleSymbol RetargetingModule => _retargetingProperty.RetargetingModule;

			public RetargetingPropertyParameterSymbol(RetargetingPropertySymbol retargetingProperty, ParameterSymbol underlyingParameter)
				: base(underlyingParameter)
			{
				_retargetingProperty = retargetingProperty;
			}
		}

		private readonly ParameterSymbol _underlyingParameter;

		private CustomModifiersTuple _lazyCustomModifiers;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		public ParameterSymbol UnderlyingParameter => _underlyingParameter;

		protected abstract RetargetingModuleSymbol RetargetingModule { get; }

		private RetargetingModuleSymbol.RetargetingSymbolTranslator RetargetingTranslator => RetargetingModule.RetargetingTranslator;

		public override bool IsImplicitlyDeclared => _underlyingParameter.IsImplicitlyDeclared;

		public override TypeSymbol Type => RetargetingTranslator.Retarget(_underlyingParameter.Type, RetargetOptions.RetargetPrimitiveTypesByTypeCode);

		public override ImmutableArray<CustomModifier> CustomModifiers => CustomModifiersTuple.TypeCustomModifiers;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => CustomModifiersTuple.RefCustomModifiers;

		private CustomModifiersTuple CustomModifiersTuple => RetargetingTranslator.RetargetModifiers(_underlyingParameter.CustomModifiers, _underlyingParameter.RefCustomModifiers, ref _lazyCustomModifiers);

		public override bool IsParamArray => _underlyingParameter.IsParamArray;

		public override bool IsByRef => _underlyingParameter.IsByRef;

		internal override bool IsExplicitByRef => _underlyingParameter.IsExplicitByRef;

		public override int Ordinal => _underlyingParameter.Ordinal;

		public override bool IsOptional => _underlyingParameter.IsOptional;

		internal override bool IsMetadataOut => _underlyingParameter.IsMetadataOut;

		internal override bool IsMetadataIn => _underlyingParameter.IsMetadataIn;

		public override bool HasExplicitDefaultValue => _underlyingParameter.HasExplicitDefaultValue;

		internal override ConstantValue ExplicitDefaultConstantValue => _underlyingParameter.get_ExplicitDefaultConstantValue(inProgress);

		internal override MarshalPseudoCustomAttributeData MarshallingInformation => RetargetingTranslator.Retarget(UnderlyingParameter.MarshallingInformation);

		internal override bool HasOptionCompare => _underlyingParameter.HasOptionCompare;

		internal override bool IsIDispatchConstant => _underlyingParameter.IsIDispatchConstant;

		internal override bool IsIUnknownConstant => _underlyingParameter.IsIUnknownConstant;

		internal override bool IsCallerLineNumber => _underlyingParameter.IsCallerLineNumber;

		internal override bool IsCallerMemberName => _underlyingParameter.IsCallerMemberName;

		internal override bool IsCallerFilePath => _underlyingParameter.IsCallerFilePath;

		public override Symbol ContainingSymbol => RetargetingTranslator.Retarget(_underlyingParameter.ContainingSymbol);

		public override ImmutableArray<Location> Locations => _underlyingParameter.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _underlyingParameter.DeclaringSyntaxReferences;

		public override AssemblySymbol ContainingAssembly => RetargetingModule.ContainingAssembly;

		public override ModuleSymbol ContainingModule => RetargetingModule;

		public override string Name => _underlyingParameter.Name;

		public override string MetadataName => _underlyingParameter.MetadataName;

		internal override bool HasMetadataConstantValue => _underlyingParameter.HasMetadataConstantValue;

		internal override bool IsMetadataOptional => _underlyingParameter.IsMetadataOptional;

		internal override bool IsMarshalledExplicitly => _underlyingParameter.IsMarshalledExplicitly;

		internal override ImmutableArray<byte> MarshallingDescriptor => _underlyingParameter.MarshallingDescriptor;

		internal sealed override VisualBasicCompilation DeclaringCompilation => null;

		public static RetargetingParameterSymbol CreateMethodParameter(RetargetingMethodSymbol retargetingMethod, ParameterSymbol underlyingParameter)
		{
			return new RetargetingMethodParameterSymbol(retargetingMethod, underlyingParameter);
		}

		public static RetargetingParameterSymbol CreatePropertyParameter(RetargetingPropertySymbol retargetingProperty, ParameterSymbol underlyingParameter)
		{
			return new RetargetingPropertyParameterSymbol(retargetingProperty, underlyingParameter);
		}

		protected RetargetingParameterSymbol(ParameterSymbol underlyingParameter)
		{
			if (underlyingParameter is RetargetingParameterSymbol)
			{
				throw new ArgumentException();
			}
			_underlyingParameter = underlyingParameter;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return RetargetingTranslator.GetRetargetedAttributes(_underlyingParameter, ref _lazyCustomAttributes);
		}

		internal override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState)
		{
			return RetargetingTranslator.RetargetAttributes(_underlyingParameter.GetCustomAttributesToEmit(compilationState));
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _underlyingParameter.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}
	}
}
