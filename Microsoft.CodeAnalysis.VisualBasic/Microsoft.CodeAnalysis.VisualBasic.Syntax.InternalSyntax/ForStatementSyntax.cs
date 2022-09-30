using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ForStatementSyntax : ForOrForEachStatementSyntax
	{
		internal readonly PunctuationSyntax _equalsToken;

		internal readonly ExpressionSyntax _fromValue;

		internal readonly KeywordSyntax _toKeyword;

		internal readonly ExpressionSyntax _toValue;

		internal readonly ForStepClauseSyntax _stepClause;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax EqualsToken => _equalsToken;

		internal ExpressionSyntax FromValue => _fromValue;

		internal KeywordSyntax ToKeyword => _toKeyword;

		internal ExpressionSyntax ToValue => _toValue;

		internal ForStepClauseSyntax StepClause => _stepClause;

		internal ForStatementSyntax(SyntaxKind kind, KeywordSyntax forKeyword, VisualBasicSyntaxNode controlVariable, PunctuationSyntax equalsToken, ExpressionSyntax fromValue, KeywordSyntax toKeyword, ExpressionSyntax toValue, ForStepClauseSyntax stepClause)
			: base(kind, forKeyword, controlVariable)
		{
			base._slotCount = 7;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(fromValue);
			_fromValue = fromValue;
			AdjustFlagsAndWidth(toKeyword);
			_toKeyword = toKeyword;
			AdjustFlagsAndWidth(toValue);
			_toValue = toValue;
			if (stepClause != null)
			{
				AdjustFlagsAndWidth(stepClause);
				_stepClause = stepClause;
			}
		}

		internal ForStatementSyntax(SyntaxKind kind, KeywordSyntax forKeyword, VisualBasicSyntaxNode controlVariable, PunctuationSyntax equalsToken, ExpressionSyntax fromValue, KeywordSyntax toKeyword, ExpressionSyntax toValue, ForStepClauseSyntax stepClause, ISyntaxFactoryContext context)
			: base(kind, forKeyword, controlVariable)
		{
			base._slotCount = 7;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(fromValue);
			_fromValue = fromValue;
			AdjustFlagsAndWidth(toKeyword);
			_toKeyword = toKeyword;
			AdjustFlagsAndWidth(toValue);
			_toValue = toValue;
			if (stepClause != null)
			{
				AdjustFlagsAndWidth(stepClause);
				_stepClause = stepClause;
			}
		}

		internal ForStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax forKeyword, VisualBasicSyntaxNode controlVariable, PunctuationSyntax equalsToken, ExpressionSyntax fromValue, KeywordSyntax toKeyword, ExpressionSyntax toValue, ForStepClauseSyntax stepClause)
			: base(kind, errors, annotations, forKeyword, controlVariable)
		{
			base._slotCount = 7;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(fromValue);
			_fromValue = fromValue;
			AdjustFlagsAndWidth(toKeyword);
			_toKeyword = toKeyword;
			AdjustFlagsAndWidth(toValue);
			_toValue = toValue;
			if (stepClause != null)
			{
				AdjustFlagsAndWidth(stepClause);
				_stepClause = stepClause;
			}
		}

		internal ForStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 7;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_equalsToken = punctuationSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_fromValue = expressionSyntax;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_toKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax2 != null)
			{
				AdjustFlagsAndWidth(expressionSyntax2);
				_toValue = expressionSyntax2;
			}
			ForStepClauseSyntax forStepClauseSyntax = (ForStepClauseSyntax)reader.ReadValue();
			if (forStepClauseSyntax != null)
			{
				AdjustFlagsAndWidth(forStepClauseSyntax);
				_stepClause = forStepClauseSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_equalsToken);
			writer.WriteValue(_fromValue);
			writer.WriteValue(_toKeyword);
			writer.WriteValue(_toValue);
			writer.WriteValue(_stepClause);
		}

		static ForStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ForStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ForStatementSyntax), (ObjectReader r) => new ForStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _forKeyword, 
				1 => _controlVariable, 
				2 => _equalsToken, 
				3 => _fromValue, 
				4 => _toKeyword, 
				5 => _toValue, 
				6 => _stepClause, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ForStatementSyntax(base.Kind, newErrors, GetAnnotations(), _forKeyword, _controlVariable, _equalsToken, _fromValue, _toKeyword, _toValue, _stepClause);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ForStatementSyntax(base.Kind, GetDiagnostics(), annotations, _forKeyword, _controlVariable, _equalsToken, _fromValue, _toKeyword, _toValue, _stepClause);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitForStatement(this);
		}
	}
}
