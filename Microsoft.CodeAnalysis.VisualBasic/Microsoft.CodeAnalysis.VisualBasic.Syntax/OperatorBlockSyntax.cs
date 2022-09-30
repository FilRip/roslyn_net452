using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class OperatorBlockSyntax : MethodBlockBaseSyntax
	{
		internal OperatorStatementSyntax _operatorStatement;

		internal EndBlockStatementSyntax _endOperatorStatement;

		public OperatorStatementSyntax OperatorStatement => GetRedAtZero(ref _operatorStatement);

		public new SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public EndBlockStatementSyntax EndOperatorStatement => GetRed(ref _endOperatorStatement, 2);

		public override MethodBaseSyntax BlockStatement => OperatorStatement;

		public override EndBlockStatementSyntax EndBlockStatement => EndOperatorStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new OperatorStatementSyntax Begin => OperatorStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new EndBlockStatementSyntax End => EndOperatorStatement;

		internal OperatorBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal OperatorBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, OperatorStatementSyntax operatorStatement, SyntaxNode statements, EndBlockStatementSyntax endOperatorStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax)operatorStatement.Green, statements?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endOperatorStatement.Green), null, 0)
		{
		}

		public OperatorBlockSyntax WithOperatorStatement(OperatorStatementSyntax operatorStatement)
		{
			return Update(operatorStatement, Statements, EndOperatorStatement);
		}

		internal override SyntaxList<StatementSyntax> GetStatementsCore()
		{
			return Statements;
		}

		internal override MethodBlockBaseSyntax WithStatementsCore(SyntaxList<StatementSyntax> statements)
		{
			return WithStatements(statements);
		}

		public new OperatorBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(OperatorStatement, statements, EndOperatorStatement);
		}

		public new OperatorBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		internal override MethodBlockBaseSyntax AddStatementsCore(params StatementSyntax[] items)
		{
			return AddStatements(items);
		}

		public OperatorBlockSyntax WithEndOperatorStatement(EndBlockStatementSyntax endOperatorStatement)
		{
			return Update(OperatorStatement, Statements, endOperatorStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _operatorStatement, 
				1 => _statements, 
				2 => _endOperatorStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => OperatorStatement, 
				1 => GetRed(ref _statements, 1), 
				2 => EndOperatorStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitOperatorBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitOperatorBlock(this);
		}

		public OperatorBlockSyntax Update(OperatorStatementSyntax operatorStatement, SyntaxList<StatementSyntax> statements, EndBlockStatementSyntax endOperatorStatement)
		{
			if (operatorStatement != OperatorStatement || statements != Statements || endOperatorStatement != EndOperatorStatement)
			{
				OperatorBlockSyntax operatorBlockSyntax = SyntaxFactory.OperatorBlock(operatorStatement, statements, endOperatorStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(operatorBlockSyntax, annotations);
				}
				return operatorBlockSyntax;
			}
			return this;
		}

		public override MethodBlockBaseSyntax WithBlockStatement(MethodBaseSyntax blockStatement)
		{
			return WithOperatorStatement((OperatorStatementSyntax)blockStatement);
		}

		public override MethodBlockBaseSyntax WithEndBlockStatement(EndBlockStatementSyntax endBlockStatement)
		{
			return WithEndOperatorStatement(endBlockStatement);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public OperatorBlockSyntax WithBegin(OperatorStatementSyntax begin)
		{
			return WithOperatorStatement(begin);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new OperatorBlockSyntax WithEnd(EndBlockStatementSyntax end)
		{
			return WithEndOperatorStatement(end);
		}
	}
}
