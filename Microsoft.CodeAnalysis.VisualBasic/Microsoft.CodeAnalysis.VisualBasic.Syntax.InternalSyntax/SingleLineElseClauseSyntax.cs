using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SingleLineElseClauseSyntax : VisualBasicSyntaxNode
	{
		internal readonly KeywordSyntax _elseKeyword;

		internal readonly GreenNode _statements;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ElseKeyword => _elseKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal SingleLineElseClauseSyntax(SyntaxKind kind, KeywordSyntax elseKeyword, GreenNode statements)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(elseKeyword);
			_elseKeyword = elseKeyword;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal SingleLineElseClauseSyntax(SyntaxKind kind, KeywordSyntax elseKeyword, GreenNode statements, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(elseKeyword);
			_elseKeyword = elseKeyword;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal SingleLineElseClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax elseKeyword, GreenNode statements)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(elseKeyword);
			_elseKeyword = elseKeyword;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal SingleLineElseClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_elseKeyword = keywordSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_statements = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_elseKeyword);
			writer.WriteValue(_statements);
		}

		static SingleLineElseClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new SingleLineElseClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SingleLineElseClauseSyntax), (ObjectReader r) => new SingleLineElseClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _elseKeyword, 
				1 => _statements, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SingleLineElseClauseSyntax(base.Kind, newErrors, GetAnnotations(), _elseKeyword, _statements);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SingleLineElseClauseSyntax(base.Kind, GetDiagnostics(), annotations, _elseKeyword, _statements);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSingleLineElseClause(this);
		}
	}
}
