using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class VariableDeclaratorSyntax : VisualBasicSyntaxNode
	{
		internal SyntaxNode _names;

		internal AsClauseSyntax _asClause;

		internal EqualsValueSyntax _initializer;

		public SeparatedSyntaxList<ModifiedIdentifierSyntax> Names
		{
			get
			{
				SyntaxNode redAtZero = GetRedAtZero(ref _names);
				return (redAtZero == null) ? default(SeparatedSyntaxList<ModifiedIdentifierSyntax>) : new SeparatedSyntaxList<ModifiedIdentifierSyntax>(redAtZero, 0);
			}
		}

		public AsClauseSyntax AsClause => GetRed(ref _asClause, 1);

		public EqualsValueSyntax Initializer => GetRed(ref _initializer, 2);

		internal VariableDeclaratorSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal VariableDeclaratorSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode names, AsClauseSyntax asClause, EqualsValueSyntax initializer)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax(kind, errors, annotations, names?.Green, (asClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax)asClause.Green) : null, (initializer != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)initializer.Green) : null), null, 0)
		{
		}

		public VariableDeclaratorSyntax WithNames(SeparatedSyntaxList<ModifiedIdentifierSyntax> names)
		{
			return Update(names, AsClause, Initializer);
		}

		public VariableDeclaratorSyntax AddNames(params ModifiedIdentifierSyntax[] items)
		{
			return WithNames(Names.AddRange(items));
		}

		public VariableDeclaratorSyntax WithAsClause(AsClauseSyntax asClause)
		{
			return Update(Names, asClause, Initializer);
		}

		public VariableDeclaratorSyntax WithInitializer(EqualsValueSyntax initializer)
		{
			return Update(Names, AsClause, initializer);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _names, 
				1 => _asClause, 
				2 => _initializer, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => GetRedAtZero(ref _names), 
				1 => AsClause, 
				2 => Initializer, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitVariableDeclarator(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitVariableDeclarator(this);
		}

		public VariableDeclaratorSyntax Update(SeparatedSyntaxList<ModifiedIdentifierSyntax> names, AsClauseSyntax asClause, EqualsValueSyntax initializer)
		{
			if (names != Names || asClause != AsClause || initializer != Initializer)
			{
				VariableDeclaratorSyntax variableDeclaratorSyntax = SyntaxFactory.VariableDeclarator(names, asClause, initializer);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(variableDeclaratorSyntax, annotations);
				}
				return variableDeclaratorSyntax;
			}
			return this;
		}
	}
}
