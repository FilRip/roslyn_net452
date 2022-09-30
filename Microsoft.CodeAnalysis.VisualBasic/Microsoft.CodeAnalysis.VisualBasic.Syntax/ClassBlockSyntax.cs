using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ClassBlockSyntax : TypeBlockSyntax
	{
		internal ClassStatementSyntax _classStatement;

		internal EndBlockStatementSyntax _endClassStatement;

		public ClassStatementSyntax ClassStatement => GetRedAtZero(ref _classStatement);

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

		public EndBlockStatementSyntax EndClassStatement => GetRed(ref _endClassStatement, 4);

		public override TypeStatementSyntax BlockStatement => ClassStatement;

		public override EndBlockStatementSyntax EndBlockStatement => EndClassStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new ClassStatementSyntax Begin => ClassStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new EndBlockStatementSyntax End => EndClassStatement;

		internal ClassBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ClassBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ClassStatementSyntax classStatement, SyntaxNode inherits, SyntaxNode implements, SyntaxNode members, EndBlockStatementSyntax endClassStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax)classStatement.Green, inherits?.Green, implements?.Green, members?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endClassStatement.Green), null, 0)
		{
		}

		public ClassBlockSyntax WithClassStatement(ClassStatementSyntax classStatement)
		{
			return Update(classStatement, Inherits, Implements, Members, EndClassStatement);
		}

		internal override SyntaxList<InheritsStatementSyntax> GetInheritsCore()
		{
			return Inherits;
		}

		internal override TypeBlockSyntax WithInheritsCore(SyntaxList<InheritsStatementSyntax> inherits)
		{
			return WithInherits(inherits);
		}

		public new ClassBlockSyntax WithInherits(SyntaxList<InheritsStatementSyntax> inherits)
		{
			return Update(ClassStatement, inherits, Implements, Members, EndClassStatement);
		}

		public new ClassBlockSyntax AddInherits(params InheritsStatementSyntax[] items)
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

		public new ClassBlockSyntax WithImplements(SyntaxList<ImplementsStatementSyntax> implements)
		{
			return Update(ClassStatement, Inherits, implements, Members, EndClassStatement);
		}

		public new ClassBlockSyntax AddImplements(params ImplementsStatementSyntax[] items)
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

		public new ClassBlockSyntax WithMembers(SyntaxList<StatementSyntax> members)
		{
			return Update(ClassStatement, Inherits, Implements, members, EndClassStatement);
		}

		public new ClassBlockSyntax AddMembers(params StatementSyntax[] items)
		{
			return WithMembers(Members.AddRange(items));
		}

		internal override TypeBlockSyntax AddMembersCore(params StatementSyntax[] items)
		{
			return AddMembers(items);
		}

		public ClassBlockSyntax WithEndClassStatement(EndBlockStatementSyntax endClassStatement)
		{
			return Update(ClassStatement, Inherits, Implements, Members, endClassStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _classStatement, 
				1 => _inherits, 
				2 => _implements, 
				3 => _members, 
				4 => _endClassStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => ClassStatement, 
				1 => GetRed(ref _inherits, 1), 
				2 => GetRed(ref _implements, 2), 
				3 => GetRed(ref _members, 3), 
				4 => EndClassStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitClassBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitClassBlock(this);
		}

		public ClassBlockSyntax Update(ClassStatementSyntax classStatement, SyntaxList<InheritsStatementSyntax> inherits, SyntaxList<ImplementsStatementSyntax> implements, SyntaxList<StatementSyntax> members, EndBlockStatementSyntax endClassStatement)
		{
			if (classStatement != ClassStatement || inherits != Inherits || implements != Implements || members != Members || endClassStatement != EndClassStatement)
			{
				ClassBlockSyntax classBlockSyntax = SyntaxFactory.ClassBlock(classStatement, inherits, implements, members, endClassStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(classBlockSyntax, annotations);
				}
				return classBlockSyntax;
			}
			return this;
		}

		public override TypeBlockSyntax WithBlockStatement(TypeStatementSyntax blockStatement)
		{
			return WithClassStatement((ClassStatementSyntax)blockStatement);
		}

		public override TypeBlockSyntax WithEndBlockStatement(EndBlockStatementSyntax endBlockStatement)
		{
			return WithEndClassStatement(endBlockStatement);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public ClassBlockSyntax WithBegin(ClassStatementSyntax begin)
		{
			return WithClassStatement(begin);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new ClassBlockSyntax WithEnd(EndBlockStatementSyntax end)
		{
			return WithEndClassStatement(end);
		}
	}
}
