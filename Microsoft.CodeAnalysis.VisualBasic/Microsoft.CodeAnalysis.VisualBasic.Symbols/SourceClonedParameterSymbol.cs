using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SourceClonedParameterSymbol : SourceParameterSymbolBase
	{
		internal sealed class SourceClonedParameterSymbolWithCustomModifiers : SourceClonedParameterSymbol
		{
			private readonly TypeSymbol _type;

			private readonly ImmutableArray<CustomModifier> _customModifiers;

			private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

			public override TypeSymbol Type => _type;

			public override ImmutableArray<CustomModifier> CustomModifiers => _customModifiers;

			public override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

			internal SourceClonedParameterSymbolWithCustomModifiers(SourceParameterSymbol originalParam, MethodSymbol newOwner, int newOrdinal, TypeSymbol type, ImmutableArray<CustomModifier> customModifiers, ImmutableArray<CustomModifier> refCustomModifiers)
				: base(originalParam, newOwner, newOrdinal)
			{
				_type = type;
				_customModifiers = customModifiers.NullToEmpty();
				_refCustomModifiers = refCustomModifiers.NullToEmpty();
			}

			internal override ParameterSymbol WithTypeAndCustomModifiers(TypeSymbol type, ImmutableArray<CustomModifier> customModifiers, ImmutableArray<CustomModifier> refCustomModifiers)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		private readonly SourceParameterSymbol _originalParam;

		public override bool IsImplicitlyDeclared => true;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override TypeSymbol Type => _originalParam.Type;

		internal override bool IsMetadataIn => _originalParam.IsMetadataIn;

		internal override bool IsMetadataOut => _originalParam.IsMetadataOut;

		public override ImmutableArray<Location> Locations => _originalParam.Locations;

		public override string Name => _originalParam.Name;

		public override ImmutableArray<CustomModifier> CustomModifiers => _originalParam.CustomModifiers;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => _originalParam.RefCustomModifiers;

		internal override ConstantValue ExplicitDefaultConstantValue => ((ParameterSymbol)_originalParam).get_ExplicitDefaultConstantValue(inProgress);

		internal override bool HasParamArrayAttribute => _originalParam.HasParamArrayAttribute;

		internal override bool HasDefaultValueAttribute => _originalParam.HasDefaultValueAttribute;

		public override bool HasExplicitDefaultValue => _originalParam.HasExplicitDefaultValue;

		internal override bool HasOptionCompare => _originalParam.HasOptionCompare;

		internal override bool IsIDispatchConstant => _originalParam.IsIDispatchConstant;

		internal override bool IsIUnknownConstant => _originalParam.IsIUnknownConstant;

		internal override bool IsCallerLineNumber => _originalParam.IsCallerLineNumber;

		internal override bool IsCallerMemberName => _originalParam.IsCallerMemberName;

		internal override bool IsCallerFilePath => _originalParam.IsCallerFilePath;

		public override bool IsByRef => _originalParam.IsByRef;

		internal override bool IsExplicitByRef => _originalParam.IsExplicitByRef;

		public override bool IsOptional => _originalParam.IsOptional;

		public override bool IsParamArray => _originalParam.IsParamArray;

		internal override MarshalPseudoCustomAttributeData MarshallingInformation => _originalParam.MarshallingInformation;

		internal SourceClonedParameterSymbol(SourceParameterSymbol originalParam, MethodSymbol newOwner, int newOrdinal)
			: base(newOwner, newOrdinal)
		{
			_originalParam = originalParam;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return _originalParam.GetAttributes();
		}

		internal override ParameterSymbol WithTypeAndCustomModifiers(TypeSymbol type, ImmutableArray<CustomModifier> customModifiers, ImmutableArray<CustomModifier> refCustomModifiers)
		{
			return new SourceClonedParameterSymbolWithCustomModifiers(_originalParam, (MethodSymbol)base.ContainingSymbol, base.Ordinal, type, customModifiers, refCustomModifiers);
		}
	}
}
