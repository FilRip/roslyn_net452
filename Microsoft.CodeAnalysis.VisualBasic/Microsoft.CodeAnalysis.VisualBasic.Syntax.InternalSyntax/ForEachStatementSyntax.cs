using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ForEachStatementSyntax : ForOrForEachStatementSyntax
	{
		internal readonly KeywordSyntax _eachKeyword;

		internal readonly KeywordSyntax _inKeyword;

		internal readonly ExpressionSyntax _expression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax EachKeyword => _eachKeyword;

		internal KeywordSyntax InKeyword => _inKeyword;

		internal ExpressionSyntax Expression => _expression;

		internal ForEachStatementSyntax(SyntaxKind kind, KeywordSyntax forKeyword, KeywordSyntax eachKeyword, VisualBasicSyntaxNode controlVariable, KeywordSyntax inKeyword, ExpressionSyntax expression)
			: base(kind, forKeyword, controlVariable)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(eachKeyword);
			_eachKeyword = eachKeyword;
			AdjustFlagsAndWidth(inKeyword);
			_inKeyword = inKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal ForEachStatementSyntax(SyntaxKind kind, KeywordSyntax forKeyword, KeywordSyntax eachKeyword, VisualBasicSyntaxNode controlVariable, KeywordSyntax inKeyword, ExpressionSyntax expression, ISyntaxFactoryContext context)
			: base(kind, forKeyword, controlVariable)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(eachKeyword);
			_eachKeyword = eachKeyword;
			AdjustFlagsAndWidth(inKeyword);
			_inKeyword = inKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal ForEachStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax forKeyword, KeywordSyntax eachKeyword, VisualBasicSyntaxNode controlVariable, KeywordSyntax inKeyword, ExpressionSyntax expression)
			: base(kind, errors, annotations, forKeyword, controlVariable)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(eachKeyword);
			_eachKeyword = eachKeyword;
			AdjustFlagsAndWidth(inKeyword);
			_inKeyword = inKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal ForEachStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_eachKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_inKeyword = keywordSyntax2;
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
			writer.WriteValue(_eachKeyword);
			writer.WriteValue(_inKeyword);
			writer.WriteValue(_expression);
		}

		static ForEachStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ForEachStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ForEachStatementSyntax), (ObjectReader r) => new ForEachStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _forKeyword, 
				1 => _eachKeyword, 
				2 => _controlVariable, 
				3 => _inKeyword, 
				4 => _expression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ForEachStatementSyntax(base.Kind, newErrors, GetAnnotations(), _forKeyword, _eachKeyword, _controlVariable, _inKeyword, _expression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ForEachStatementSyntax(base.Kind, GetDiagnostics(), annotations, _forKeyword, _eachKeyword, _controlVariable, _inKeyword, _expression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitForEachStatement(this);
		}
	}
}
