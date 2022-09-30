using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal struct TypeParameterConstraint
	{
		public readonly TypeParameterConstraintKind Kind;

		public readonly TypeSymbol TypeConstraint;

		public readonly Location LocationOpt;

		public bool IsConstructorConstraint => Kind == TypeParameterConstraintKind.Constructor;

		public bool IsReferenceTypeConstraint => Kind == TypeParameterConstraintKind.ReferenceType;

		public bool IsValueTypeConstraint => Kind == TypeParameterConstraintKind.ValueType;

		public TypeParameterConstraint(TypeParameterConstraintKind kind, Location loc)
			: this(kind, null, loc)
		{
		}

		public TypeParameterConstraint(TypeSymbol type, Location loc)
			: this(TypeParameterConstraintKind.None, type, loc)
		{
		}

		private TypeParameterConstraint(TypeParameterConstraintKind kind, TypeSymbol type, Location loc)
		{
			this = default(TypeParameterConstraint);
			Kind = kind;
			TypeConstraint = type;
			LocationOpt = loc;
		}

		public TypeParameterConstraint AtLocation(Location loc)
		{
			return new TypeParameterConstraint(Kind, TypeConstraint, loc);
		}

		public object ToDisplayFormat()
		{
			if ((object)TypeConstraint != null)
			{
				return CustomSymbolDisplayFormatter.ErrorNameWithKind(TypeConstraint);
			}
			return SyntaxFacts.GetText(ToSyntaxKind(Kind));
		}

		public override string ToString()
		{
			return ToDisplayFormat().ToString();
		}

		private static SyntaxKind ToSyntaxKind(TypeParameterConstraintKind kind)
		{
			return kind switch
			{
				TypeParameterConstraintKind.Constructor => SyntaxKind.NewKeyword, 
				TypeParameterConstraintKind.ReferenceType => SyntaxKind.ClassKeyword, 
				TypeParameterConstraintKind.ValueType => SyntaxKind.StructureKeyword, 
				_ => throw ExceptionUtilities.UnexpectedValue(kind), 
			};
		}
	}
}
