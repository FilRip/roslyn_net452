using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ParameterSyntax : VisualBasicSyntaxNode
	{
		internal SyntaxNode _attributeLists;

		internal ModifiedIdentifierSyntax _identifier;

		internal SimpleAsClauseSyntax _asClause;

		internal EqualsValueSyntax _default;

		public SyntaxList<AttributeListSyntax> AttributeLists
		{
			get
			{
				SyntaxNode redAtZero = GetRedAtZero(ref _attributeLists);
				return new SyntaxList<AttributeListSyntax>(redAtZero);
			}
		}

		public SyntaxTokenList Modifiers
		{
			get
			{
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public ModifiedIdentifierSyntax Identifier => GetRed(ref _identifier, 2);

		public SimpleAsClauseSyntax AsClause => GetRed(ref _asClause, 3);

		public EqualsValueSyntax Default => GetRed(ref _default, 4);

		internal ParameterSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ParameterSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, GreenNode modifiers, ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, EqualsValueSyntax @default)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax(kind, errors, annotations, attributeLists?.Green, modifiers, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)identifier.Green, (asClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)asClause.Green) : null, (@default != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)@default.Green) : null), null, 0)
		{
		}

		public ParameterSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(attributeLists, Modifiers, Identifier, AsClause, Default);
		}

		public ParameterSyntax AddAttributeLists(params AttributeListSyntax[] items)
		{
			return WithAttributeLists(AttributeLists.AddRange(items));
		}

		public ParameterSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(AttributeLists, modifiers, Identifier, AsClause, Default);
		}

		public ParameterSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		public ParameterSyntax WithIdentifier(ModifiedIdentifierSyntax identifier)
		{
			return Update(AttributeLists, Modifiers, identifier, AsClause, Default);
		}

		public ParameterSyntax WithAsClause(SimpleAsClauseSyntax asClause)
		{
			return Update(AttributeLists, Modifiers, Identifier, asClause, Default);
		}

		public ParameterSyntax WithDefault(EqualsValueSyntax @default)
		{
			return Update(AttributeLists, Modifiers, Identifier, AsClause, @default);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				2 => _identifier, 
				3 => _asClause, 
				4 => _default, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => GetRedAtZero(ref _attributeLists), 
				2 => Identifier, 
				3 => AsClause, 
				4 => Default, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitParameter(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitParameter(this);
		}

		public ParameterSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, EqualsValueSyntax @default)
		{
			if (attributeLists != AttributeLists || modifiers != Modifiers || identifier != Identifier || asClause != AsClause || @default != Default)
			{
				ParameterSyntax parameterSyntax = SyntaxFactory.Parameter(attributeLists, modifiers, identifier, asClause, @default);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(parameterSyntax, annotations);
				}
				return parameterSyntax;
			}
			return this;
		}
	}
}
