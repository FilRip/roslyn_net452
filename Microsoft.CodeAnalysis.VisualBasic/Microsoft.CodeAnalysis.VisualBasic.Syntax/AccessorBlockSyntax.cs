using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class AccessorBlockSyntax : MethodBlockBaseSyntax
	{
		internal AccessorStatementSyntax _accessorStatement;

		internal EndBlockStatementSyntax _endAccessorStatement;

		public AccessorStatementSyntax AccessorStatement => GetRedAtZero(ref _accessorStatement);

		public new SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public EndBlockStatementSyntax EndAccessorStatement => GetRed(ref _endAccessorStatement, 2);

		public override MethodBaseSyntax BlockStatement => AccessorStatement;

		public override EndBlockStatementSyntax EndBlockStatement => EndAccessorStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use AccessorStatement instead.", true)]
		public new AccessorStatementSyntax Begin => AccessorStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use EndAccessorStatement instead.", true)]
		public new EndBlockStatementSyntax End => EndAccessorStatement;

		internal AccessorBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal AccessorBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, AccessorStatementSyntax accessorStatement, SyntaxNode statements, EndBlockStatementSyntax endAccessorStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax)accessorStatement.Green, statements?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endAccessorStatement.Green), null, 0)
		{
		}

		public AccessorBlockSyntax WithAccessorStatement(AccessorStatementSyntax accessorStatement)
		{
			return Update(Kind(), accessorStatement, Statements, EndAccessorStatement);
		}

		internal override SyntaxList<StatementSyntax> GetStatementsCore()
		{
			return Statements;
		}

		internal override MethodBlockBaseSyntax WithStatementsCore(SyntaxList<StatementSyntax> statements)
		{
			return WithStatements(statements);
		}

		public new AccessorBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(Kind(), AccessorStatement, statements, EndAccessorStatement);
		}

		public new AccessorBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		internal override MethodBlockBaseSyntax AddStatementsCore(params StatementSyntax[] items)
		{
			return AddStatements(items);
		}

		public AccessorBlockSyntax WithEndAccessorStatement(EndBlockStatementSyntax endAccessorStatement)
		{
			return Update(Kind(), AccessorStatement, Statements, endAccessorStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _accessorStatement, 
				1 => _statements, 
				2 => _endAccessorStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => AccessorStatement, 
				1 => GetRed(ref _statements, 1), 
				2 => EndAccessorStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitAccessorBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitAccessorBlock(this);
		}

		public AccessorBlockSyntax Update(SyntaxKind kind, AccessorStatementSyntax accessorStatement, SyntaxList<StatementSyntax> statements, EndBlockStatementSyntax endAccessorStatement)
		{
			if (kind != Kind() || accessorStatement != AccessorStatement || statements != Statements || endAccessorStatement != EndAccessorStatement)
			{
				AccessorBlockSyntax accessorBlockSyntax = SyntaxFactory.AccessorBlock(kind, accessorStatement, statements, endAccessorStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(accessorBlockSyntax, annotations);
				}
				return accessorBlockSyntax;
			}
			return this;
		}

		public override MethodBlockBaseSyntax WithBlockStatement(MethodBaseSyntax blockStatement)
		{
			return WithAccessorStatement((AccessorStatementSyntax)blockStatement);
		}

		public override MethodBlockBaseSyntax WithEndBlockStatement(EndBlockStatementSyntax endBlockStatement)
		{
			return WithEndAccessorStatement(endBlockStatement);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use WithAccessorStatement instead.", true)]
		public AccessorBlockSyntax WithBegin(AccessorStatementSyntax begin)
		{
			return WithAccessorStatement(begin);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use WithEndAccessorStatement instead.", true)]
		public new AccessorBlockSyntax WithEnd(EndBlockStatementSyntax end)
		{
			return WithEndAccessorStatement(end);
		}
	}
}
