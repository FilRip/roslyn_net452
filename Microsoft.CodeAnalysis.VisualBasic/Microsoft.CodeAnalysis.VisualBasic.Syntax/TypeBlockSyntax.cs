using System;
using System.ComponentModel;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class TypeBlockSyntax : DeclarationStatementSyntax
	{
		internal SyntaxNode _inherits;

		internal SyntaxNode _implements;

		internal SyntaxNode _members;

		public SyntaxList<InheritsStatementSyntax> Inherits => GetInheritsCore();

		public SyntaxList<ImplementsStatementSyntax> Implements => GetImplementsCore();

		public SyntaxList<StatementSyntax> Members => GetMembersCore();

		public abstract TypeStatementSyntax BlockStatement { get; }

		public abstract EndBlockStatementSyntax EndBlockStatement { get; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use BlockStatement or a more specific property (e.g. ClassStatement) instead.", true)]
		public TypeStatementSyntax Begin => BlockStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use EndBlockStatement or a more specific property (e.g. EndClassStatement) instead.", true)]
		public EndBlockStatementSyntax End => EndBlockStatement;

		internal TypeBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal virtual SyntaxList<InheritsStatementSyntax> GetInheritsCore()
		{
			SyntaxNode redAtZero = GetRedAtZero(ref _inherits);
			return new SyntaxList<InheritsStatementSyntax>(redAtZero);
		}

		public TypeBlockSyntax WithInherits(SyntaxList<InheritsStatementSyntax> inherits)
		{
			return WithInheritsCore(inherits);
		}

		internal abstract TypeBlockSyntax WithInheritsCore(SyntaxList<InheritsStatementSyntax> inherits);

		public TypeBlockSyntax AddInherits(params InheritsStatementSyntax[] items)
		{
			return AddInheritsCore(items);
		}

		internal abstract TypeBlockSyntax AddInheritsCore(params InheritsStatementSyntax[] items);

		internal virtual SyntaxList<ImplementsStatementSyntax> GetImplementsCore()
		{
			SyntaxNode red = GetRed(ref _implements, 1);
			return new SyntaxList<ImplementsStatementSyntax>(red);
		}

		public TypeBlockSyntax WithImplements(SyntaxList<ImplementsStatementSyntax> implements)
		{
			return WithImplementsCore(implements);
		}

		internal abstract TypeBlockSyntax WithImplementsCore(SyntaxList<ImplementsStatementSyntax> implements);

		public TypeBlockSyntax AddImplements(params ImplementsStatementSyntax[] items)
		{
			return AddImplementsCore(items);
		}

		internal abstract TypeBlockSyntax AddImplementsCore(params ImplementsStatementSyntax[] items);

		internal virtual SyntaxList<StatementSyntax> GetMembersCore()
		{
			SyntaxNode red = GetRed(ref _members, 2);
			return new SyntaxList<StatementSyntax>(red);
		}

		public TypeBlockSyntax WithMembers(SyntaxList<StatementSyntax> members)
		{
			return WithMembersCore(members);
		}

		internal abstract TypeBlockSyntax WithMembersCore(SyntaxList<StatementSyntax> members);

		public TypeBlockSyntax AddMembers(params StatementSyntax[] items)
		{
			return AddMembersCore(items);
		}

		internal abstract TypeBlockSyntax AddMembersCore(params StatementSyntax[] items);

		public abstract TypeBlockSyntax WithBlockStatement(TypeStatementSyntax blockStatement);

		public abstract TypeBlockSyntax WithEndBlockStatement(EndBlockStatementSyntax endBlockStatement);

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use WithBlockStatement or a more specific property (e.g. WithClassStatement) instead.", true)]
		public TypeBlockSyntax WithBegin(TypeStatementSyntax begin)
		{
			return WithBlockStatement(begin);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use WithEndBlockStatement or a more specific property (e.g. WithEndClassStatement) instead.", true)]
		public TypeBlockSyntax WithEnd(EndBlockStatementSyntax end)
		{
			return WithEndBlockStatement(end);
		}
	}
}
