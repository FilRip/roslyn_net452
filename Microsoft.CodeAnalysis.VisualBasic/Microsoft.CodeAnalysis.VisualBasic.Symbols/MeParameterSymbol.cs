using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class MeParameterSymbol : ParameterSymbol
	{
		private readonly Symbol _container;

		private readonly TypeSymbol _type;

		public override string Name => "Me";

		public override TypeSymbol Type => _type;

		public override ImmutableArray<Location> Locations
		{
			get
			{
				if ((object)_container != null)
				{
					return _container.Locations;
				}
				return ImmutableArray<Location>.Empty;
			}
		}

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override Symbol ContainingSymbol => _container;

		internal override ConstantValue ExplicitDefaultConstantValue => null;

		public override bool HasExplicitDefaultValue => false;

		public override bool IsOptional => false;

		public override bool IsParamArray => false;

		public override int Ordinal => -1;

		public override ImmutableArray<CustomModifier> CustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override bool IsByRef => Type.IsValueType;

		internal override bool IsExplicitByRef => IsByRef;

		internal override bool IsMetadataOut => false;

		internal override bool IsMetadataIn => false;

		internal override MarshalPseudoCustomAttributeData MarshallingInformation => null;

		internal override bool HasOptionCompare => false;

		internal override bool IsIDispatchConstant => false;

		internal override bool IsIUnknownConstant => false;

		internal override bool IsCallerLineNumber => false;

		internal override bool IsCallerMemberName => false;

		internal override bool IsCallerFilePath => false;

		public override bool IsMe => true;

		public override bool IsImplicitlyDeclared => true;

		internal MeParameterSymbol(Symbol memberSymbol)
		{
			_container = memberSymbol;
			_type = _container.ContainingType;
		}

		internal MeParameterSymbol(Symbol memberSymbol, TypeSymbol type)
		{
			_container = memberSymbol;
			_type = type;
		}
	}
}
