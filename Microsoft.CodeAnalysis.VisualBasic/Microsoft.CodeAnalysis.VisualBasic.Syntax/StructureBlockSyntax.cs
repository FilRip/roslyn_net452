using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class StructureBlockSyntax : TypeBlockSyntax
	{
		internal StructureStatementSyntax _structureStatement;

		internal EndBlockStatementSyntax _endStructureStatement;

		public StructureStatementSyntax StructureStatement => GetRedAtZero(ref _structureStatement);

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

		public EndBlockStatementSyntax EndStructureStatement => GetRed(ref _endStructureStatement, 4);

		public override TypeStatementSyntax BlockStatement => StructureStatement;

		public override EndBlockStatementSyntax EndBlockStatement => EndStructureStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new StructureStatementSyntax Begin => StructureStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new EndBlockStatementSyntax End => EndStructureStatement;

		internal StructureBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal StructureBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, StructureStatementSyntax structureStatement, SyntaxNode inherits, SyntaxNode implements, SyntaxNode members, EndBlockStatementSyntax endStructureStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax)structureStatement.Green, inherits?.Green, implements?.Green, members?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endStructureStatement.Green), null, 0)
		{
		}

		public StructureBlockSyntax WithStructureStatement(StructureStatementSyntax structureStatement)
		{
			return Update(structureStatement, Inherits, Implements, Members, EndStructureStatement);
		}

		internal override SyntaxList<InheritsStatementSyntax> GetInheritsCore()
		{
			return Inherits;
		}

		internal override TypeBlockSyntax WithInheritsCore(SyntaxList<InheritsStatementSyntax> inherits)
		{
			return WithInherits(inherits);
		}

		public new StructureBlockSyntax WithInherits(SyntaxList<InheritsStatementSyntax> inherits)
		{
			return Update(StructureStatement, inherits, Implements, Members, EndStructureStatement);
		}

		public new StructureBlockSyntax AddInherits(params InheritsStatementSyntax[] items)
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

		public new StructureBlockSyntax WithImplements(SyntaxList<ImplementsStatementSyntax> implements)
		{
			return Update(StructureStatement, Inherits, implements, Members, EndStructureStatement);
		}

		public new StructureBlockSyntax AddImplements(params ImplementsStatementSyntax[] items)
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

		public new StructureBlockSyntax WithMembers(SyntaxList<StatementSyntax> members)
		{
			return Update(StructureStatement, Inherits, Implements, members, EndStructureStatement);
		}

		public new StructureBlockSyntax AddMembers(params StatementSyntax[] items)
		{
			return WithMembers(Members.AddRange(items));
		}

		internal override TypeBlockSyntax AddMembersCore(params StatementSyntax[] items)
		{
			return AddMembers(items);
		}

		public StructureBlockSyntax WithEndStructureStatement(EndBlockStatementSyntax endStructureStatement)
		{
			return Update(StructureStatement, Inherits, Implements, Members, endStructureStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _structureStatement, 
				1 => _inherits, 
				2 => _implements, 
				3 => _members, 
				4 => _endStructureStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => StructureStatement, 
				1 => GetRed(ref _inherits, 1), 
				2 => GetRed(ref _implements, 2), 
				3 => GetRed(ref _members, 3), 
				4 => EndStructureStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitStructureBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitStructureBlock(this);
		}

		public StructureBlockSyntax Update(StructureStatementSyntax structureStatement, SyntaxList<InheritsStatementSyntax> inherits, SyntaxList<ImplementsStatementSyntax> implements, SyntaxList<StatementSyntax> members, EndBlockStatementSyntax endStructureStatement)
		{
			if (structureStatement != StructureStatement || inherits != Inherits || implements != Implements || members != Members || endStructureStatement != EndStructureStatement)
			{
				StructureBlockSyntax structureBlockSyntax = SyntaxFactory.StructureBlock(structureStatement, inherits, implements, members, endStructureStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(structureBlockSyntax, annotations);
				}
				return structureBlockSyntax;
			}
			return this;
		}

		public override TypeBlockSyntax WithBlockStatement(TypeStatementSyntax blockStatement)
		{
			return WithStructureStatement((StructureStatementSyntax)blockStatement);
		}

		public override TypeBlockSyntax WithEndBlockStatement(EndBlockStatementSyntax endBlockStatement)
		{
			return WithEndStructureStatement(endBlockStatement);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public StructureBlockSyntax WithBegin(StructureStatementSyntax begin)
		{
			return WithStructureStatement(begin);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new StructureBlockSyntax WithEnd(EndBlockStatementSyntax end)
		{
			return WithEndStructureStatement(end);
		}
	}
}
