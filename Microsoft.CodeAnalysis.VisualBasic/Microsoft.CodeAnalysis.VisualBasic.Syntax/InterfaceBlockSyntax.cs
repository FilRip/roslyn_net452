using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class InterfaceBlockSyntax : TypeBlockSyntax
	{
		internal InterfaceStatementSyntax _interfaceStatement;

		internal EndBlockStatementSyntax _endInterfaceStatement;

		public InterfaceStatementSyntax InterfaceStatement => GetRedAtZero(ref _interfaceStatement);

		public new SyntaxList<InheritsStatementSyntax> Inherits
		{
			get
			{
				SyntaxNode red = GetRed(ref _inherits, 1);
				return new SyntaxList<InheritsStatementSyntax>(red);
			}
		}

		public new SyntaxList<ImplementsStatementSyntax> Implements
		{
			get
			{
				SyntaxNode red = GetRed(ref _implements, 2);
				return new SyntaxList<ImplementsStatementSyntax>(red);
			}
		}

		public new SyntaxList<StatementSyntax> Members
		{
			get
			{
				SyntaxNode red = GetRed(ref _members, 3);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public EndBlockStatementSyntax EndInterfaceStatement => GetRed(ref _endInterfaceStatement, 4);

		public override TypeStatementSyntax BlockStatement => InterfaceStatement;

		public override EndBlockStatementSyntax EndBlockStatement => EndInterfaceStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new InterfaceStatementSyntax Begin => InterfaceStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new EndBlockStatementSyntax End => EndInterfaceStatement;

		internal InterfaceBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal InterfaceBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, InterfaceStatementSyntax interfaceStatement, SyntaxNode inherits, SyntaxNode implements, SyntaxNode members, EndBlockStatementSyntax endInterfaceStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax)interfaceStatement.Green, inherits?.Green, implements?.Green, members?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endInterfaceStatement.Green), null, 0)
		{
		}

		public InterfaceBlockSyntax WithInterfaceStatement(InterfaceStatementSyntax interfaceStatement)
		{
			return Update(interfaceStatement, Inherits, Implements, Members, EndInterfaceStatement);
		}

		internal override SyntaxList<InheritsStatementSyntax> GetInheritsCore()
		{
			return Inherits;
		}

		internal override TypeBlockSyntax WithInheritsCore(SyntaxList<InheritsStatementSyntax> inherits)
		{
			return WithInherits(inherits);
		}

		public new InterfaceBlockSyntax WithInherits(SyntaxList<InheritsStatementSyntax> inherits)
		{
			return Update(InterfaceStatement, inherits, Implements, Members, EndInterfaceStatement);
		}

		public new InterfaceBlockSyntax AddInherits(params InheritsStatementSyntax[] items)
		{
			return WithInherits(Inherits.AddRange(items));
		}

		internal override TypeBlockSyntax AddInheritsCore(params InheritsStatementSyntax[] items)
		{
			return AddInherits(items);
		}

		internal override SyntaxList<ImplementsStatementSyntax> GetImplementsCore()
		{
			return Implements;
		}

		internal override TypeBlockSyntax WithImplementsCore(SyntaxList<ImplementsStatementSyntax> implements)
		{
			return WithImplements(implements);
		}

		public new InterfaceBlockSyntax WithImplements(SyntaxList<ImplementsStatementSyntax> implements)
		{
			return Update(InterfaceStatement, Inherits, implements, Members, EndInterfaceStatement);
		}

		public new InterfaceBlockSyntax AddImplements(params ImplementsStatementSyntax[] items)
		{
			return WithImplements(Implements.AddRange(items));
		}

		internal override TypeBlockSyntax AddImplementsCore(params ImplementsStatementSyntax[] items)
		{
			return AddImplements(items);
		}

		internal override SyntaxList<StatementSyntax> GetMembersCore()
		{
			return Members;
		}

		internal override TypeBlockSyntax WithMembersCore(SyntaxList<StatementSyntax> members)
		{
			return WithMembers(members);
		}

		public new InterfaceBlockSyntax WithMembers(SyntaxList<StatementSyntax> members)
		{
			return Update(InterfaceStatement, Inherits, Implements, members, EndInterfaceStatement);
		}

		public new InterfaceBlockSyntax AddMembers(params StatementSyntax[] items)
		{
			return WithMembers(Members.AddRange(items));
		}

		internal override TypeBlockSyntax AddMembersCore(params StatementSyntax[] items)
		{
			return AddMembers(items);
		}

		public InterfaceBlockSyntax WithEndInterfaceStatement(EndBlockStatementSyntax endInterfaceStatement)
		{
			return Update(InterfaceStatement, Inherits, Implements, Members, endInterfaceStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _interfaceStatement, 
				1 => _inherits, 
				2 => _implements, 
				3 => _members, 
				4 => _endInterfaceStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => InterfaceStatement, 
				1 => GetRed(ref _inherits, 1), 
				2 => GetRed(ref _implements, 2), 
				3 => GetRed(ref _members, 3), 
				4 => EndInterfaceStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitInterfaceBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitInterfaceBlock(this);
		}

		public InterfaceBlockSyntax Update(InterfaceStatementSyntax interfaceStatement, SyntaxList<InheritsStatementSyntax> inherits, SyntaxList<ImplementsStatementSyntax> implements, SyntaxList<StatementSyntax> members, EndBlockStatementSyntax endInterfaceStatement)
		{
			if (interfaceStatement != InterfaceStatement || inherits != Inherits || implements != Implements || members != Members || endInterfaceStatement != EndInterfaceStatement)
			{
				InterfaceBlockSyntax interfaceBlockSyntax = SyntaxFactory.InterfaceBlock(interfaceStatement, inherits, implements, members, endInterfaceStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(interfaceBlockSyntax, annotations);
				}
				return interfaceBlockSyntax;
			}
			return this;
		}

		public override TypeBlockSyntax WithBlockStatement(TypeStatementSyntax blockStatement)
		{
			return WithInterfaceStatement((InterfaceStatementSyntax)blockStatement);
		}

		public override TypeBlockSyntax WithEndBlockStatement(EndBlockStatementSyntax endBlockStatement)
		{
			return WithEndInterfaceStatement(endBlockStatement);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public InterfaceBlockSyntax WithBegin(InterfaceStatementSyntax begin)
		{
			return WithInterfaceStatement(begin);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new InterfaceBlockSyntax WithEnd(EndBlockStatementSyntax end)
		{
			return WithEndInterfaceStatement(end);
		}
	}
}
