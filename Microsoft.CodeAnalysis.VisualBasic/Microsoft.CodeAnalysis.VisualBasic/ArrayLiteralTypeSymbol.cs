using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class ArrayLiteralTypeSymbol : ArrayTypeSymbol
	{
		private readonly BoundArrayLiteral _arrayLiteral;

		internal BoundArrayLiteral ArrayLiteral => _arrayLiteral;

		internal override bool IsSZArray => _arrayLiteral.InferredType.IsSZArray;

		public override int Rank => _arrayLiteral.InferredType.Rank;

		internal override bool HasDefaultSizesAndLowerBounds => _arrayLiteral.InferredType.HasDefaultSizesAndLowerBounds;

		internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics => _arrayLiteral.InferredType.InterfacesNoUseSiteDiagnostics;

		internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => _arrayLiteral.InferredType.BaseTypeNoUseSiteDiagnostics;

		public override ImmutableArray<CustomModifier> CustomModifiers => _arrayLiteral.InferredType.CustomModifiers;

		public override TypeSymbol ElementType => _arrayLiteral.InferredType.ElementType;

		internal ArrayLiteralTypeSymbol(BoundArrayLiteral arrayLiteral)
		{
			_arrayLiteral = arrayLiteral;
		}

		internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution substitution)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override ArrayTypeSymbol WithElementType(TypeSymbol elementType)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
