using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class MultiLineLambdaExpressionSyntax : LambdaExpressionSyntax
	{
		internal SyntaxNode _statements;

		internal EndBlockStatementSyntax _endSubOrFunctionStatement;

		public new LambdaHeaderSyntax SubOrFunctionHeader => GetRedAtZero(ref _subOrFunctionHeader);

		public SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public EndBlockStatementSyntax EndSubOrFunctionStatement => GetRed(ref _endSubOrFunctionStatement, 2);

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use SubOrFunctionHeader instead.", true)]
		public new LambdaHeaderSyntax Begin => SubOrFunctionHeader;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use EndBlockStatement or a more specific property (e.g. EndClassStatement) instead.", true)]
		public EndBlockStatementSyntax End => EndSubOrFunctionStatement;

		internal MultiLineLambdaExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal MultiLineLambdaExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, LambdaHeaderSyntax subOrFunctionHeader, SyntaxNode statements, EndBlockStatementSyntax endSubOrFunctionStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax)subOrFunctionHeader.Green, statements?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endSubOrFunctionStatement.Green), null, 0)
		{
		}

		internal override LambdaHeaderSyntax GetSubOrFunctionHeaderCore()
		{
			return SubOrFunctionHeader;
		}

		internal override LambdaExpressionSyntax WithSubOrFunctionHeaderCore(LambdaHeaderSyntax subOrFunctionHeader)
		{
			return WithSubOrFunctionHeader(subOrFunctionHeader);
		}

		public new MultiLineLambdaExpressionSyntax WithSubOrFunctionHeader(LambdaHeaderSyntax subOrFunctionHeader)
		{
			return Update(Kind(), subOrFunctionHeader, Statements, EndSubOrFunctionStatement);
		}

		public MultiLineLambdaExpressionSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(Kind(), SubOrFunctionHeader, statements, EndSubOrFunctionStatement);
		}

		public MultiLineLambdaExpressionSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		public MultiLineLambdaExpressionSyntax WithEndSubOrFunctionStatement(EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			return Update(Kind(), SubOrFunctionHeader, Statements, endSubOrFunctionStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _subOrFunctionHeader, 
				1 => _statements, 
				2 => _endSubOrFunctionStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => SubOrFunctionHeader, 
				1 => GetRed(ref _statements, 1), 
				2 => EndSubOrFunctionStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitMultiLineLambdaExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitMultiLineLambdaExpression(this);
		}

		public MultiLineLambdaExpressionSyntax Update(SyntaxKind kind, LambdaHeaderSyntax subOrFunctionHeader, SyntaxList<StatementSyntax> statements, EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			if (kind != Kind() || subOrFunctionHeader != SubOrFunctionHeader || statements != Statements || endSubOrFunctionStatement != EndSubOrFunctionStatement)
			{
				MultiLineLambdaExpressionSyntax multiLineLambdaExpressionSyntax = SyntaxFactory.MultiLineLambdaExpression(kind, subOrFunctionHeader, statements, endSubOrFunctionStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(multiLineLambdaExpressionSyntax, annotations);
				}
				return multiLineLambdaExpressionSyntax;
			}
			return this;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use WithBlockStatement or a more specific property (e.g. WithClassStatement) instead.", true)]
		public MultiLineLambdaExpressionSyntax WithBegin(LambdaHeaderSyntax begin)
		{
			return WithSubOrFunctionHeader(begin);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use WithEndBlockStatement or a more specific property (e.g. WithEndClassStatement) instead.", true)]
		public MultiLineLambdaExpressionSyntax WithEnd(EndBlockStatementSyntax end)
		{
			return WithEndSubOrFunctionStatement(end);
		}
	}
}
