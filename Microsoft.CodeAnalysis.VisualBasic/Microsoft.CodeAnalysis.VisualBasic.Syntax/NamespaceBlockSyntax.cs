using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class NamespaceBlockSyntax : DeclarationStatementSyntax
	{
		internal NamespaceStatementSyntax _namespaceStatement;

		internal SyntaxNode _members;

		internal EndBlockStatementSyntax _endNamespaceStatement;

		public NamespaceStatementSyntax NamespaceStatement => GetRedAtZero(ref _namespaceStatement);

		public SyntaxList<StatementSyntax> Members
		{
			get
			{
				SyntaxNode red = GetRed(ref _members, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public EndBlockStatementSyntax EndNamespaceStatement => GetRed(ref _endNamespaceStatement, 2);

		internal NamespaceBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal NamespaceBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, NamespaceStatementSyntax namespaceStatement, SyntaxNode members, EndBlockStatementSyntax endNamespaceStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax)namespaceStatement.Green, members?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endNamespaceStatement.Green), null, 0)
		{
		}

		public NamespaceBlockSyntax WithNamespaceStatement(NamespaceStatementSyntax namespaceStatement)
		{
			return Update(namespaceStatement, Members, EndNamespaceStatement);
		}

		public NamespaceBlockSyntax WithMembers(SyntaxList<StatementSyntax> members)
		{
			return Update(NamespaceStatement, members, EndNamespaceStatement);
		}

		public NamespaceBlockSyntax AddMembers(params StatementSyntax[] items)
		{
			return WithMembers(Members.AddRange(items));
		}

		public NamespaceBlockSyntax WithEndNamespaceStatement(EndBlockStatementSyntax endNamespaceStatement)
		{
			return Update(NamespaceStatement, Members, endNamespaceStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _namespaceStatement, 
				1 => _members, 
				2 => _endNamespaceStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => NamespaceStatement, 
				1 => GetRed(ref _members, 1), 
				2 => EndNamespaceStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitNamespaceBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitNamespaceBlock(this);
		}

		public NamespaceBlockSyntax Update(NamespaceStatementSyntax namespaceStatement, SyntaxList<StatementSyntax> members, EndBlockStatementSyntax endNamespaceStatement)
		{
			if (namespaceStatement != NamespaceStatement || members != Members || endNamespaceStatement != EndNamespaceStatement)
			{
				NamespaceBlockSyntax namespaceBlockSyntax = SyntaxFactory.NamespaceBlock(namespaceStatement, members, endNamespaceStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(namespaceBlockSyntax, annotations);
				}
				return namespaceBlockSyntax;
			}
			return this;
		}
	}
}
