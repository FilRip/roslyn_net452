using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class EnumBlockSyntax : DeclarationStatementSyntax
	{
		internal EnumStatementSyntax _enumStatement;

		internal SyntaxNode _members;

		internal EndBlockStatementSyntax _endEnumStatement;

		public EnumStatementSyntax EnumStatement => GetRedAtZero(ref _enumStatement);

		public SyntaxList<StatementSyntax> Members
		{
			get
			{
				SyntaxNode red = GetRed(ref _members, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public EndBlockStatementSyntax EndEnumStatement => GetRed(ref _endEnumStatement, 2);

		internal EnumBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal EnumBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, EnumStatementSyntax enumStatement, SyntaxNode members, EndBlockStatementSyntax endEnumStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax)enumStatement.Green, members?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endEnumStatement.Green), null, 0)
		{
		}

		public EnumBlockSyntax WithEnumStatement(EnumStatementSyntax enumStatement)
		{
			return Update(enumStatement, Members, EndEnumStatement);
		}

		public EnumBlockSyntax WithMembers(SyntaxList<StatementSyntax> members)
		{
			return Update(EnumStatement, members, EndEnumStatement);
		}

		public EnumBlockSyntax AddMembers(params StatementSyntax[] items)
		{
			return WithMembers(Members.AddRange(items));
		}

		public EnumBlockSyntax WithEndEnumStatement(EndBlockStatementSyntax endEnumStatement)
		{
			return Update(EnumStatement, Members, endEnumStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _enumStatement, 
				1 => _members, 
				2 => _endEnumStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => EnumStatement, 
				1 => GetRed(ref _members, 1), 
				2 => EndEnumStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitEnumBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitEnumBlock(this);
		}

		public EnumBlockSyntax Update(EnumStatementSyntax enumStatement, SyntaxList<StatementSyntax> members, EndBlockStatementSyntax endEnumStatement)
		{
			if (enumStatement != EnumStatement || members != Members || endEnumStatement != EndEnumStatement)
			{
				EnumBlockSyntax enumBlockSyntax = SyntaxFactory.EnumBlock(enumStatement, members, endEnumStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(enumBlockSyntax, annotations);
				}
				return enumBlockSyntax;
			}
			return this;
		}
	}
}
