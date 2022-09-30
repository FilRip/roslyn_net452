using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SelectStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _selectKeyword;

		internal readonly KeywordSyntax _caseKeyword;

		internal readonly ExpressionSyntax _expression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax SelectKeyword => _selectKeyword;

		internal KeywordSyntax CaseKeyword => _caseKeyword;

		internal ExpressionSyntax Expression => _expression;

		internal SelectStatementSyntax(SyntaxKind kind, KeywordSyntax selectKeyword, KeywordSyntax caseKeyword, ExpressionSyntax expression)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(selectKeyword);
			_selectKeyword = selectKeyword;
			if (caseKeyword != null)
			{
				AdjustFlagsAndWidth(caseKeyword);
				_caseKeyword = caseKeyword;
			}
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal SelectStatementSyntax(SyntaxKind kind, KeywordSyntax selectKeyword, KeywordSyntax caseKeyword, ExpressionSyntax expression, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(selectKeyword);
			_selectKeyword = selectKeyword;
			if (caseKeyword != null)
			{
				AdjustFlagsAndWidth(caseKeyword);
				_caseKeyword = caseKeyword;
			}
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal SelectStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax selectKeyword, KeywordSyntax caseKeyword, ExpressionSyntax expression)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(selectKeyword);
			_selectKeyword = selectKeyword;
			if (caseKeyword != null)
			{
				AdjustFlagsAndWidth(caseKeyword);
				_caseKeyword = caseKeyword;
			}
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal SelectStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_selectKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_caseKeyword = keywordSyntax2;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_expression = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_selectKeyword);
			writer.WriteValue(_caseKeyword);
			writer.WriteValue(_expression);
		}

		static SelectStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new SelectStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SelectStatementSyntax), (ObjectReader r) => new SelectStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _selectKeyword, 
				1 => _caseKeyword, 
				2 => _expression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SelectStatementSyntax(base.Kind, newErrors, GetAnnotations(), _selectKeyword, _caseKeyword, _expression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SelectStatementSyntax(base.Kind, GetDiagnostics(), annotations, _selectKeyword, _caseKeyword, _expression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSelectStatement(this);
		}
	}
}
