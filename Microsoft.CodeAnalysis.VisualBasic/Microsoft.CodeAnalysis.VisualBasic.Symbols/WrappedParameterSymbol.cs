using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class WrappedParameterSymbol : ParameterSymbol
	{
		protected ParameterSymbol _underlyingParameter;

		public ParameterSymbol UnderlyingParameter => _underlyingParameter;

		public override TypeSymbol Type => _underlyingParameter.Type;

		public override bool IsByRef => _underlyingParameter.IsByRef;

		internal override bool IsMetadataIn => _underlyingParameter.IsMetadataIn;

		internal override bool IsMetadataOut => _underlyingParameter.IsMetadataOut;

		public override ImmutableArray<Location> Locations => _underlyingParameter.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _underlyingParameter.DeclaringSyntaxReferences;

		public override int Ordinal => _underlyingParameter.Ordinal;

		public override bool IsParamArray => _underlyingParameter.IsParamArray;

		internal override bool IsMetadataOptional => _underlyingParameter.IsMetadataOptional;

		public override bool IsImplicitlyDeclared => _underlyingParameter.IsImplicitlyDeclared;

		public override string Name => _underlyingParameter.Name;

		public override string MetadataName => _underlyingParameter.MetadataName;

		public override ImmutableArray<CustomModifier> CustomModifiers => _underlyingParameter.CustomModifiers;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => _underlyingParameter.RefCustomModifiers;

		internal override MarshalPseudoCustomAttributeData MarshallingInformation => _underlyingParameter.MarshallingInformation;

		internal override UnmanagedType MarshallingType => _underlyingParameter.MarshallingType;

		internal override bool IsIDispatchConstant => _underlyingParameter.IsIDispatchConstant;

		internal override bool IsIUnknownConstant => _underlyingParameter.IsIUnknownConstant;

		internal override bool IsCallerLineNumber => _underlyingParameter.IsCallerLineNumber;

		internal override bool IsCallerFilePath => _underlyingParameter.IsCallerFilePath;

		internal override bool IsCallerMemberName => _underlyingParameter.IsCallerMemberName;

		internal override bool IsExplicitByRef => _underlyingParameter.IsExplicitByRef;

		public override bool IsOptional => _underlyingParameter.IsOptional;

		public override bool HasExplicitDefaultValue => _underlyingParameter.HasExplicitDefaultValue;

		internal override ConstantValue ExplicitDefaultConstantValue => _underlyingParameter.get_ExplicitDefaultConstantValue(inProgress);

		internal override bool HasOptionCompare => _underlyingParameter.HasOptionCompare;

		protected WrappedParameterSymbol(ParameterSymbol underlyingParameter)
		{
			_underlyingParameter = underlyingParameter;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return _underlyingParameter.GetAttributes();
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			_underlyingParameter.AddSynthesizedAttributes(compilationState, ref attributes);
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _underlyingParameter.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}
	}
}
