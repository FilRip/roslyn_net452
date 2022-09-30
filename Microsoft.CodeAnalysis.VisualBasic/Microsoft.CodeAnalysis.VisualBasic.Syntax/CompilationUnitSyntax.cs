using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class CompilationUnitSyntax : VisualBasicSyntaxNode, ICompilationUnitSyntax
	{
		internal SyntaxNode _options;

		internal SyntaxNode _imports;

		internal SyntaxNode _attributes;

		internal SyntaxNode _members;

		public SyntaxList<OptionStatementSyntax> Options
		{
			get
			{
				SyntaxNode redAtZero = GetRedAtZero(ref _options);
				return new SyntaxList<OptionStatementSyntax>(redAtZero);
			}
		}

		public SyntaxList<ImportsStatementSyntax> Imports
		{
			get
			{
				SyntaxNode red = GetRed(ref _imports, 1);
				return new SyntaxList<ImportsStatementSyntax>(red);
			}
		}

		public SyntaxList<AttributesStatementSyntax> Attributes
		{
			get
			{
				SyntaxNode red = GetRed(ref _attributes, 2);
				return new SyntaxList<AttributesStatementSyntax>(red);
			}
		}

		public SyntaxList<StatementSyntax> Members
		{
			get
			{
				SyntaxNode red = GetRed(ref _members, 3);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public SyntaxToken EndOfFileToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax)base.Green)._endOfFileToken, GetChildPosition(4), GetChildIndex(4));

		private SyntaxToken ICompilationUnitSyntax_EndOfFileToken => EndOfFileToken;

		internal CompilationUnitSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal CompilationUnitSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode options, SyntaxNode imports, SyntaxNode attributes, SyntaxNode members, PunctuationSyntax endOfFileToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax(kind, errors, annotations, options?.Green, imports?.Green, attributes?.Green, members?.Green, endOfFileToken), null, 0)
		{
		}

		public CompilationUnitSyntax WithOptions(SyntaxList<OptionStatementSyntax> options)
		{
			return Update(options, Imports, Attributes, Members, EndOfFileToken);
		}

		public CompilationUnitSyntax AddOptions(params OptionStatementSyntax[] items)
		{
			return WithOptions(Options.AddRange(items));
		}

		public CompilationUnitSyntax WithImports(SyntaxList<ImportsStatementSyntax> imports)
		{
			return Update(Options, imports, Attributes, Members, EndOfFileToken);
		}

		public CompilationUnitSyntax AddImports(params ImportsStatementSyntax[] items)
		{
			return WithImports(Imports.AddRange(items));
		}

		public CompilationUnitSyntax WithAttributes(SyntaxList<AttributesStatementSyntax> attributes)
		{
			return Update(Options, Imports, attributes, Members, EndOfFileToken);
		}

		public CompilationUnitSyntax AddAttributes(params AttributesStatementSyntax[] items)
		{
			return WithAttributes(Attributes.AddRange(items));
		}

		public CompilationUnitSyntax WithMembers(SyntaxList<StatementSyntax> members)
		{
			return Update(Options, Imports, Attributes, members, EndOfFileToken);
		}

		public CompilationUnitSyntax AddMembers(params StatementSyntax[] items)
		{
			return WithMembers(Members.AddRange(items));
		}

		public CompilationUnitSyntax WithEndOfFileToken(SyntaxToken endOfFileToken)
		{
			return Update(Options, Imports, Attributes, Members, endOfFileToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _options, 
				1 => _imports, 
				2 => _attributes, 
				3 => _members, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => GetRedAtZero(ref _options), 
				1 => GetRed(ref _imports, 1), 
				2 => GetRed(ref _attributes, 2), 
				3 => GetRed(ref _members, 3), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitCompilationUnit(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitCompilationUnit(this);
		}

		public CompilationUnitSyntax Update(SyntaxList<OptionStatementSyntax> options, SyntaxList<ImportsStatementSyntax> imports, SyntaxList<AttributesStatementSyntax> attributes, SyntaxList<StatementSyntax> members, SyntaxToken endOfFileToken)
		{
			if (options != Options || imports != Imports || attributes != Attributes || members != Members || endOfFileToken != EndOfFileToken)
			{
				CompilationUnitSyntax compilationUnitSyntax = SyntaxFactory.CompilationUnit(options, imports, attributes, members, endOfFileToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(compilationUnitSyntax, annotations);
				}
				return compilationUnitSyntax;
			}
			return this;
		}

		public IList<ReferenceDirectiveTriviaSyntax> GetReferenceDirectives()
		{
			return GetReferenceDirectives(null);
		}

		internal IList<ReferenceDirectiveTriviaSyntax> GetReferenceDirectives(Func<ReferenceDirectiveTriviaSyntax, bool> filter)
		{
			return ((SyntaxNodeOrToken)GetFirstToken(includeZeroWidth: true)).GetDirectives(filter);
		}
	}
}
