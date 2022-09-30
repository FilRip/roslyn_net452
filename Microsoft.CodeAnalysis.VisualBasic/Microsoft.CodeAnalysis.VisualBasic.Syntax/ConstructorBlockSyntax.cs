using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ConstructorBlockSyntax : MethodBlockBaseSyntax
	{
		internal SubNewStatementSyntax _subNewStatement;

		internal EndBlockStatementSyntax _endSubStatement;

		public SubNewStatementSyntax SubNewStatement => GetRedAtZero(ref _subNewStatement);

		public new SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public EndBlockStatementSyntax EndSubStatement => GetRed(ref _endSubStatement, 2);

		public override MethodBaseSyntax BlockStatement => SubNewStatement;

		public override EndBlockStatementSyntax EndBlockStatement => EndSubStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new SubNewStatementSyntax Begin => SubNewStatement;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new EndBlockStatementSyntax End => EndSubStatement;

		internal ConstructorBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ConstructorBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SubNewStatementSyntax subNewStatement, SyntaxNode statements, EndBlockStatementSyntax endSubStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax)subNewStatement.Green, statements?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endSubStatement.Green), null, 0)
		{
		}

		public ConstructorBlockSyntax WithSubNewStatement(SubNewStatementSyntax subNewStatement)
		{
			return Update(subNewStatement, Statements, EndSubStatement);
		}

		internal override SyntaxList<StatementSyntax> GetStatementsCore()
		{
			return Statements;
		}

		internal override MethodBlockBaseSyntax WithStatementsCore(SyntaxList<StatementSyntax> statements)
		{
			return WithStatements(statements);
		}

		public new ConstructorBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(SubNewStatement, statements, EndSubStatement);
		}

		public new ConstructorBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		internal override MethodBlockBaseSyntax AddStatementsCore(params StatementSyntax[] items)
		{
			return AddStatements(items);
		}

		public ConstructorBlockSyntax WithEndSubStatement(EndBlockStatementSyntax endSubStatement)
		{
			return Update(SubNewStatement, Statements, endSubStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _subNewStatement, 
				1 => _statements, 
				2 => _endSubStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => SubNewStatement, 
				1 => GetRed(ref _statements, 1), 
				2 => EndSubStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitConstructorBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitConstructorBlock(this);
		}

		public ConstructorBlockSyntax Update(SubNewStatementSyntax subNewStatement, SyntaxList<StatementSyntax> statements, EndBlockStatementSyntax endSubStatement)
		{
			if (subNewStatement != SubNewStatement || statements != Statements || endSubStatement != EndSubStatement)
			{
				ConstructorBlockSyntax constructorBlockSyntax = SyntaxFactory.ConstructorBlock(subNewStatement, statements, endSubStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(constructorBlockSyntax, annotations);
				}
				return constructorBlockSyntax;
			}
			return this;
		}

		public override MethodBlockBaseSyntax WithBlockStatement(MethodBaseSyntax blockStatement)
		{
			return WithSubNewStatement((SubNewStatementSyntax)blockStatement);
		}

		public override MethodBlockBaseSyntax WithEndBlockStatement(EndBlockStatementSyntax endBlockStatement)
		{
			return WithEndSubStatement(endBlockStatement);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public ConstructorBlockSyntax WithBegin(SubNewStatementSyntax begin)
		{
			return WithSubNewStatement(begin);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new ConstructorBlockSyntax WithEnd(EndBlockStatementSyntax end)
		{
			return WithEndSubStatement(end);
		}
	}
}
