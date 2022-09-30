using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ModuleBlockSyntax : TypeBlockSyntax
	{
		internal ModuleStatementSyntax _moduleStatement;

		internal EndBlockStatementSyntax _endModuleStatement;

		public ModuleStatementSyntax ModuleStatement => GetRedAtZero(ref _moduleStatement);

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

		public EndBlockStatementSyntax EndModuleStatement => GetRed(ref _endModuleStatement, 4);

		public override TypeStatementSyntax BlockStatement => ModuleStatement;

		public override EndBlockStatementSyntax EndBlockStatement => EndModuleStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new ModuleStatementSyntax Begin => ModuleStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new EndBlockStatementSyntax End => EndModuleStatement;

		internal ModuleBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ModuleBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ModuleStatementSyntax moduleStatement, SyntaxNode inherits, SyntaxNode implements, SyntaxNode members, EndBlockStatementSyntax endModuleStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax)moduleStatement.Green, inherits?.Green, implements?.Green, members?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endModuleStatement.Green), null, 0)
		{
		}

		public ModuleBlockSyntax WithModuleStatement(ModuleStatementSyntax moduleStatement)
		{
			return Update(moduleStatement, Inherits, Implements, Members, EndModuleStatement);
		}

		internal override SyntaxList<InheritsStatementSyntax> GetInheritsCore()
		{
			return Inherits;
		}

		internal override TypeBlockSyntax WithInheritsCore(SyntaxList<InheritsStatementSyntax> inherits)
		{
			return WithInherits(inherits);
		}

		public new ModuleBlockSyntax WithInherits(SyntaxList<InheritsStatementSyntax> inherits)
		{
			return Update(ModuleStatement, inherits, Implements, Members, EndModuleStatement);
		}

		public new ModuleBlockSyntax AddInherits(params InheritsStatementSyntax[] items)
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

		public new ModuleBlockSyntax WithImplements(SyntaxList<ImplementsStatementSyntax> implements)
		{
			return Update(ModuleStatement, Inherits, implements, Members, EndModuleStatement);
		}

		public new ModuleBlockSyntax AddImplements(params ImplementsStatementSyntax[] items)
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

		public new ModuleBlockSyntax WithMembers(SyntaxList<StatementSyntax> members)
		{
			return Update(ModuleStatement, Inherits, Implements, members, EndModuleStatement);
		}

		public new ModuleBlockSyntax AddMembers(params StatementSyntax[] items)
		{
			return WithMembers(Members.AddRange(items));
		}

		internal override TypeBlockSyntax AddMembersCore(params StatementSyntax[] items)
		{
			return AddMembers(items);
		}

		public ModuleBlockSyntax WithEndModuleStatement(EndBlockStatementSyntax endModuleStatement)
		{
			return Update(ModuleStatement, Inherits, Implements, Members, endModuleStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _moduleStatement, 
				1 => _inherits, 
				2 => _implements, 
				3 => _members, 
				4 => _endModuleStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => ModuleStatement, 
				1 => GetRed(ref _inherits, 1), 
				2 => GetRed(ref _implements, 2), 
				3 => GetRed(ref _members, 3), 
				4 => EndModuleStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitModuleBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitModuleBlock(this);
		}

		public ModuleBlockSyntax Update(ModuleStatementSyntax moduleStatement, SyntaxList<InheritsStatementSyntax> inherits, SyntaxList<ImplementsStatementSyntax> implements, SyntaxList<StatementSyntax> members, EndBlockStatementSyntax endModuleStatement)
		{
			if (moduleStatement != ModuleStatement || inherits != Inherits || implements != Implements || members != Members || endModuleStatement != EndModuleStatement)
			{
				ModuleBlockSyntax moduleBlockSyntax = SyntaxFactory.ModuleBlock(moduleStatement, inherits, implements, members, endModuleStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(moduleBlockSyntax, annotations);
				}
				return moduleBlockSyntax;
			}
			return this;
		}

		public override TypeBlockSyntax WithBlockStatement(TypeStatementSyntax blockStatement)
		{
			return WithModuleStatement((ModuleStatementSyntax)blockStatement);
		}

		public override TypeBlockSyntax WithEndBlockStatement(EndBlockStatementSyntax endBlockStatement)
		{
			return WithEndModuleStatement(endBlockStatement);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public ModuleBlockSyntax WithBegin(ModuleStatementSyntax begin)
		{
			return WithModuleStatement(begin);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new ModuleBlockSyntax WithEnd(EndBlockStatementSyntax end)
		{
			return WithEndModuleStatement(end);
		}
	}
}
