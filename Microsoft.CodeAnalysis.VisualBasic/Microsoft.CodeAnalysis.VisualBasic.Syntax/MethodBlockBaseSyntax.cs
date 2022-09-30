using System;
using System.ComponentModel;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class MethodBlockBaseSyntax : DeclarationStatementSyntax
	{
		internal SyntaxNode _statements;

		public SyntaxList<StatementSyntax> Statements => GetStatementsCore();

		public abstract MethodBaseSyntax BlockStatement { get; }

		public abstract EndBlockStatementSyntax EndBlockStatement { get; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use BlockStatement or a more specific property (e.g. SubOrFunctionStatement) instead.", true)]
		public MethodBaseSyntax Begin => BlockStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use EndBlockStatement or a more specific property (e.g. EndSubOrFunctionStatement) instead.", true)]
		public EndBlockStatementSyntax End => EndBlockStatement;

		internal MethodBlockBaseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal virtual SyntaxList<StatementSyntax> GetStatementsCore()
		{
			SyntaxNode redAtZero = GetRedAtZero(ref _statements);
			return new SyntaxList<StatementSyntax>(redAtZero);
		}

		public MethodBlockBaseSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return WithStatementsCore(statements);
		}

		internal abstract MethodBlockBaseSyntax WithStatementsCore(SyntaxList<StatementSyntax> statements);

		public MethodBlockBaseSyntax AddStatements(params StatementSyntax[] items)
		{
			return AddStatementsCore(items);
		}

		internal abstract MethodBlockBaseSyntax AddStatementsCore(params StatementSyntax[] items);

		public abstract MethodBlockBaseSyntax WithBlockStatement(MethodBaseSyntax blockStatement);

		public abstract MethodBlockBaseSyntax WithEndBlockStatement(EndBlockStatementSyntax endBlockStatement);

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use WithBlockStatement or a more specific property (e.g. WithSubOrFunctionStatement) instead.", true)]
		public MethodBlockBaseSyntax WithBegin(MethodBaseSyntax begin)
		{
			return WithBlockStatement(begin);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use WithEndBlockStatement or a more specific property (e.g. WithEndSubOrFunctionStatement) instead.", true)]
		public MethodBlockBaseSyntax WithEnd(EndBlockStatementSyntax end)
		{
			return WithEndBlockStatement(end);
		}
	}
}
