using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class DirectCastExpressionSyntax : CastExpressionSyntax
	{
		internal static Func<ObjectReader, object> CreateInstance;

		internal DirectCastExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken)
			: base(kind, keyword, openParenToken, expression, commaToken, type, closeParenToken)
		{
			base._slotCount = 6;
		}

		internal DirectCastExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind, keyword, openParenToken, expression, commaToken, type, closeParenToken)
		{
			base._slotCount = 6;
			SetFactoryContext(context);
		}

		internal DirectCastExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations, keyword, openParenToken, expression, commaToken, type, closeParenToken)
		{
			base._slotCount = 6;
		}

		internal DirectCastExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 6;
		}

		static DirectCastExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new DirectCastExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(DirectCastExpressionSyntax), (ObjectReader r) => new DirectCastExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectCastExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _keyword, 
				1 => _openParenToken, 
				2 => _expression, 
				3 => _commaToken, 
				4 => _type, 
				5 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new DirectCastExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _keyword, _openParenToken, _expression, _commaToken, _type, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new DirectCastExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _keyword, _openParenToken, _expression, _commaToken, _type, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitDirectCastExpression(this);
		}
	}
}
