using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal struct FieldOrPropertyInitializer
	{
		public readonly ImmutableArray<Symbol> FieldsOrProperties;

		public readonly SyntaxReference Syntax;

		internal readonly int PrecedingInitializersLength;

		internal readonly bool IsMetadataConstant;

		public FieldOrPropertyInitializer(SyntaxReference syntax, int precedingInitializersLength)
		{
			this = default(FieldOrPropertyInitializer);
			Syntax = syntax;
			IsMetadataConstant = false;
			PrecedingInitializersLength = precedingInitializersLength;
		}

		public FieldOrPropertyInitializer(FieldSymbol field, SyntaxReference syntax, int precedingInitializersLength)
		{
			this = default(FieldOrPropertyInitializer);
			FieldsOrProperties = ImmutableArray.Create((Symbol)field);
			Syntax = syntax;
			IsMetadataConstant = field.IsMetadataConstant;
			PrecedingInitializersLength = precedingInitializersLength;
		}

		public FieldOrPropertyInitializer(ImmutableArray<Symbol> fieldsOrProperties, SyntaxReference syntax, int precedingInitializersLength)
		{
			this = default(FieldOrPropertyInitializer);
			FieldsOrProperties = fieldsOrProperties;
			Syntax = syntax;
			IsMetadataConstant = false;
			PrecedingInitializersLength = precedingInitializersLength;
		}

		public FieldOrPropertyInitializer(PropertySymbol property, SyntaxReference syntax, int precedingInitializersLength)
		{
			this = default(FieldOrPropertyInitializer);
			FieldsOrProperties = ImmutableArray.Create((Symbol)property);
			Syntax = syntax;
			IsMetadataConstant = false;
			PrecedingInitializersLength = precedingInitializersLength;
		}
	}
}
