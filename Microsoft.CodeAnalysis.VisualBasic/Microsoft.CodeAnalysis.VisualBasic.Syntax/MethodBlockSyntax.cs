using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class MethodBlockSyntax : MethodBlockBaseSyntax
	{
		internal MethodStatementSyntax _subOrFunctionStatement;

		internal EndBlockStatementSyntax _endSubOrFunctionStatement;

		public MethodStatementSyntax SubOrFunctionStatement => GetRedAtZero(ref _subOrFunctionStatement);

		public new SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public EndBlockStatementSyntax EndSubOrFunctionStatement => GetRed(ref _endSubOrFunctionStatement, 2);

		public override MethodBaseSyntax BlockStatement => SubOrFunctionStatement;

		public override EndBlockStatementSyntax EndBlockStatement => EndSubOrFunctionStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new MethodStatementSyntax Begin => SubOrFunctionStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new EndBlockStatementSyntax End => EndSubOrFunctionStatement;

		internal MethodBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal MethodBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, MethodStatementSyntax subOrFunctionStatement, SyntaxNode statements, EndBlockStatementSyntax endSubOrFunctionStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax)subOrFunctionStatement.Green, statements?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endSubOrFunctionStatement.Green), null, 0)
		{
		}

		public MethodBlockSyntax WithSubOrFunctionStatement(MethodStatementSyntax subOrFunctionStatement)
		{
			return Update(Kind(), subOrFunctionStatement, Statements, EndSubOrFunctionStatement);
		}

		internal override SyntaxList<StatementSyntax> GetStatementsCore()
		{
			return Statements;
		}

		internal override MethodBlockBaseSyntax WithStatementsCore(SyntaxList<StatementSyntax> statements)
		{
			return WithStatements(statements);
		}

		public new MethodBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(Kind(), SubOrFunctionStatement, statements, EndSubOrFunctionStatement);
		}

		public new MethodBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		internal override MethodBlockBaseSyntax AddStatementsCore(params StatementSyntax[] items)
		{
			return AddStatements(items);
		}

		public MethodBlockSyntax WithEndSubOrFunctionStatement(EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			return Update(Kind(), SubOrFunctionStatement, Statements, endSubOrFunctionStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _subOrFunctionStatement, 
				1 => _statements, 
				2 => _endSubOrFunctionStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => SubOrFunctionStatement, 
				1 => GetRed(ref _statements, 1), 
				2 => EndSubOrFunctionStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitMethodBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitMethodBlock(this);
		}

		public MethodBlockSyntax Update(SyntaxKind kind, MethodStatementSyntax subOrFunctionStatement, SyntaxList<StatementSyntax> statements, EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			if (kind != Kind() || subOrFunctionStatement != SubOrFunctionStatement || statements != Statements || endSubOrFunctionStatement != EndSubOrFunctionStatement)
			{
				MethodBlockSyntax methodBlockSyntax = SyntaxFactory.MethodBlock(kind, subOrFunctionStatement, statements, endSubOrFunctionStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(methodBlockSyntax, annotations);
				}
				return methodBlockSyntax;
			}
			return this;
		}

		public override MethodBlockBaseSyntax WithBlockStatement(MethodBaseSyntax blockStatement)
		{
			return WithSubOrFunctionStatement((MethodStatementSyntax)blockStatement);
		}

		public override MethodBlockBaseSyntax WithEndBlockStatement(EndBlockStatementSyntax endBlockStatement)
		{
			return WithEndSubOrFunctionStatement(endBlockStatement);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public MethodBlockSyntax WithBegin(MethodStatementSyntax begin)
		{
			return WithSubOrFunctionStatement(begin);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new MethodBlockSyntax WithEnd(EndBlockStatementSyntax end)
		{
			return WithEndSubOrFunctionStatement(end);
		}
	}
}
