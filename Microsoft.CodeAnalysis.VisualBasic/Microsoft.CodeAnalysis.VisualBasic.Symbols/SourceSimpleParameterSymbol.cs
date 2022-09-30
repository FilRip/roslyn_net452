using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SourceSimpleParameterSymbol : SourceParameterSymbol
	{
		internal sealed class SourceSimpleParameterSymbolWithCustomModifiers : SourceSimpleParameterSymbol
		{
			private readonly ImmutableArray<CustomModifier> _customModifiers;

			private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

			public override ImmutableArray<CustomModifier> CustomModifiers => _customModifiers;

			public override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

			internal SourceSimpleParameterSymbolWithCustomModifiers(Symbol container, string name, int ordinal, TypeSymbol type, Location location, ImmutableArray<CustomModifier> customModifiers, ImmutableArray<CustomModifier> refCustomModifiers)
				: base(container, name, ordinal, type, location)
			{
				_customModifiers = customModifiers;
				_refCustomModifiers = refCustomModifiers;
			}

			internal override ParameterSymbol WithTypeAndCustomModifiers(TypeSymbol type, ImmutableArray<CustomModifier> customModifiers, ImmutableArray<CustomModifier> refCustomModifiers)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override SyntaxList<AttributeListSyntax> AttributeDeclarationList => default(SyntaxList<AttributeListSyntax>);

		public override bool HasExplicitDefaultValue => false;

		internal override ConstantValue ExplicitDefaultConstantValue => null;

		public override bool IsOptional => false;

		public override bool IsParamArray => GetEarlyDecodedWellKnownAttributeData()?.HasParamArrayAttribute ?? false;

		internal override bool IsCallerLineNumber => GetEarlyDecodedWellKnownAttributeData()?.HasCallerLineNumberAttribute ?? false;

		internal override bool IsCallerMemberName => GetEarlyDecodedWellKnownAttributeData()?.HasCallerMemberNameAttribute ?? false;

		internal override bool IsCallerFilePath => GetEarlyDecodedWellKnownAttributeData()?.HasCallerFilePathAttribute ?? false;

		internal override bool IsExplicitByRef => false;

		public override ImmutableArray<CustomModifier> CustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		internal SourceSimpleParameterSymbol(Symbol container, string name, int ordinal, TypeSymbol type, Location location)
			: base(container, name, ordinal, type, location)
		{
		}

		internal override ParameterSymbol ChangeOwner(Symbol newContainingSymbol)
		{
			return new SourceSimpleParameterSymbol(newContainingSymbol, base.Name, base.Ordinal, base.Type, base.Location);
		}

		private SourceComplexParameterSymbol GetCorrespondingPartialParameter()
		{
			if (base.ContainingSymbol is SourceMemberMethodSymbol sourceMemberMethodSymbol && sourceMemberMethodSymbol.IsPartialImplementation)
			{
				return (SourceComplexParameterSymbol)sourceMemberMethodSymbol.SourcePartialDefinition.Parameters[base.Ordinal];
			}
			return null;
		}

		internal override CustomAttributesBag<VisualBasicAttributeData> GetAttributesBag()
		{
			SourceComplexParameterSymbol correspondingPartialParameter = GetCorrespondingPartialParameter();
			if ((object)correspondingPartialParameter != null)
			{
				return correspondingPartialParameter.GetAttributesBag();
			}
			return CustomAttributesBag<VisualBasicAttributeData>.Empty;
		}

		internal override ParameterEarlyWellKnownAttributeData GetEarlyDecodedWellKnownAttributeData()
		{
			return GetCorrespondingPartialParameter()?.GetEarlyDecodedWellKnownAttributeData();
		}

		internal override CommonParameterWellKnownAttributeData GetDecodedWellKnownAttributeData()
		{
			return GetCorrespondingPartialParameter()?.GetDecodedWellKnownAttributeData();
		}

		internal override ParameterSymbol WithTypeAndCustomModifiers(TypeSymbol type, ImmutableArray<CustomModifier> customModifiers, ImmutableArray<CustomModifier> refCustomModifiers)
		{
			if (customModifiers.IsEmpty && refCustomModifiers.IsEmpty)
			{
				return new SourceSimpleParameterSymbol(base.ContainingSymbol, base.Name, base.Ordinal, type, base.Location);
			}
			return new SourceSimpleParameterSymbolWithCustomModifiers(base.ContainingSymbol, base.Name, base.Ordinal, type, base.Location, customModifiers, refCustomModifiers);
		}
	}
}
