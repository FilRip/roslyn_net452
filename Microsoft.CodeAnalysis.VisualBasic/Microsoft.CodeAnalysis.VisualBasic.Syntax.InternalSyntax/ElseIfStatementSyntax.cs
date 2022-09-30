using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ElseIfStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _elseIfKeyword;

		internal readonly ExpressionSyntax _condition;

		internal readonly KeywordSyntax _thenKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ElseIfKeyword => _elseIfKeyword;

		internal ExpressionSyntax Condition => _condition;

		internal KeywordSyntax ThenKeyword => _thenKeyword;

		internal ElseIfStatementSyntax(SyntaxKind kind, KeywordSyntax elseIfKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(elseIfKeyword);
			_elseIfKeyword = elseIfKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
			if (thenKeyword != null)
			{
				AdjustFlagsAndWidth(thenKeyword);
				_thenKeyword = thenKeyword;
			}
		}

		internal ElseIfStatementSyntax(SyntaxKind kind, KeywordSyntax elseIfKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(elseIfKeyword);
			_elseIfKeyword = elseIfKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
			if (thenKeyword != null)
			{
				AdjustFlagsAndWidth(thenKeyword);
				_thenKeyword = thenKeyword;
			}
		}

		internal ElseIfStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax elseIfKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(elseIfKeyword);
			_elseIfKeyword = elseIfKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
			if (thenKeyword != null)
			{
				AdjustFlagsAndWidth(thenKeyword);
				_thenKeyword = thenKeyword;
			}
		}

		internal ElseIfStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_elseIfKeyword = keywordSyntax;
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
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_elseIfKeyword);
			writer.WriteValue(_condition);
			writer.WriteValue(_thenKeyword);
		}

		static ElseIfStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ElseIfStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ElseIfStatementSyntax), (ObjectReader r) => new ElseIfStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _elseIfKeyword, 
				1 => _condition, 
				2 => _thenKeyword, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ElseIfStatementSyntax(base.Kind, newErrors, GetAnnotations(), _elseIfKeyword, _condition, _thenKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ElseIfStatementSyntax(base.Kind, GetDiagnostics(), annotations, _elseIfKeyword, _condition, _thenKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitElseIfStatement(this);
		}
	}
}
