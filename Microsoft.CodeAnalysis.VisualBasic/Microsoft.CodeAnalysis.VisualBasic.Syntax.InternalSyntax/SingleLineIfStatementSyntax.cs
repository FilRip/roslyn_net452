using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SingleLineIfStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _ifKeyword;

		internal readonly ExpressionSyntax _condition;

		internal readonly KeywordSyntax _thenKeyword;

		internal readonly GreenNode _statements;

		internal readonly SingleLineElseClauseSyntax _elseClause;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax IfKeyword => _ifKeyword;

		internal ExpressionSyntax Condition => _condition;

		internal KeywordSyntax ThenKeyword => _thenKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal SingleLineElseClauseSyntax ElseClause => _elseClause;

		internal SingleLineIfStatementSyntax(SyntaxKind kind, KeywordSyntax ifKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword, GreenNode statements, SingleLineElseClauseSyntax elseClause)
			: base(kind)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(ifKeyword);
			_ifKeyword = ifKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
			AdjustFlagsAndWidth(thenKeyword);
			_thenKeyword = thenKeyword;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			if (elseClause != null)
			{
				AdjustFlagsAndWidth(elseClause);
				_elseClause = elseClause;
			}
		}

		internal SingleLineIfStatementSyntax(SyntaxKind kind, KeywordSyntax ifKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword, GreenNode statements, SingleLineElseClauseSyntax elseClause, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(ifKeyword);
			_ifKeyword = ifKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
			AdjustFlagsAndWidth(thenKeyword);
			_thenKeyword = thenKeyword;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			if (elseClause != null)
			{
				AdjustFlagsAndWidth(elseClause);
				_elseClause = elseClause;
			}
		}

		internal SingleLineIfStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax ifKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword, GreenNode statements, SingleLineElseClauseSyntax elseClause)
			: base(kind, errors, annotations)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(ifKeyword);
			_ifKeyword = ifKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
			AdjustFlagsAndWidth(thenKeyword);
			_thenKeyword = thenKeyword;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			if (elseClause != null)
			{
				AdjustFlagsAndWidth(elseClause);
				_elseClause = elseClause;
			}
		}

		internal SingleLineIfStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_ifKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_condition = expressionSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_thenKeyword = keywordSyntax2;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_statements = greenNode;
			}
			SingleLineElseClauseSyntax singleLineElseClauseSyntax = (SingleLineElseClauseSyntax)reader.ReadValue();
			if (singleLineElseClauseSyntax != null)
			{
				AdjustFlagsAndWidth(singleLineElseClauseSyntax);
				_elseClause = singleLineElseClauseSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_ifKeyword);
			writer.WriteValue(_condition);
			writer.WriteValue(_thenKeyword);
			writer.WriteValue(_statements);
			writer.WriteValue(_elseClause);
		}

		static SingleLineIfStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new SingleLineIfStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SingleLineIfStatementSyntax), (ObjectReader r) => new SingleLineIfStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _ifKeyword, 
				1 => _condition, 
				2 => _thenKeyword, 
				3 => _statements, 
				4 => _elseClause, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SingleLineIfStatementSyntax(base.Kind, newErrors, GetAnnotations(), _ifKeyword, _condition, _thenKeyword, _statements, _elseClause);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SingleLineIfStatementSyntax(base.Kind, GetDiagnostics(), annotations, _ifKeyword, _condition, _thenKeyword, _statements, _elseClause);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSingleLineIfStatement(this);
		}
	}
}
