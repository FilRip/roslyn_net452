using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class LambdaParameterSymbol : ParameterSymbol
	{
		private readonly ImmutableArray<Location> _location;

		private readonly string _name;

		private readonly TypeSymbol _type;

		private readonly ushort _ordinal;

		private readonly bool _isByRef;

		public sealed override string Name => _name;

		public sealed override int Ordinal => _ordinal;

		public sealed override bool HasExplicitDefaultValue => false;

		internal override ConstantValue ExplicitDefaultConstantValue => null;

		public sealed override bool IsOptional => false;

		internal sealed override bool IsMetadataOut => false;

		internal sealed override bool IsMetadataIn => false;

		internal sealed override MarshalPseudoCustomAttributeData MarshallingInformation => null;

		internal override bool HasOptionCompare => false;

		internal override bool IsIDispatchConstant => false;

		internal override bool IsIUnknownConstant => false;

		internal override bool IsCallerLineNumber => false;

		internal override bool IsCallerMemberName => false;

		internal override bool IsCallerFilePath => false;

		public sealed override bool IsParamArray => false;

		public sealed override ImmutableArray<Location> Locations => _location;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper<ParameterSyntax>(Locations);

		public sealed override TypeSymbol Type => _type;

		public sealed override bool IsByRef => _isByRef;

		internal sealed override bool IsExplicitByRef => _isByRef;

		public sealed override ImmutableArray<CustomModifier> CustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public sealed override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		protected LambdaParameterSymbol(string name, int ordinal, TypeSymbol type, bool isByRef, Location location)
		{
			_name = name;
			_ordinal = (ushort)ordinal;
			_type = type;
			if ((object)location != null)
			{
				_location = ImmutableArray.Create(location);
			}
			else
			{
				_location = ImmutableArray<Location>.Empty;
			}
			_isByRef = isByRef;
		}
	}
}
