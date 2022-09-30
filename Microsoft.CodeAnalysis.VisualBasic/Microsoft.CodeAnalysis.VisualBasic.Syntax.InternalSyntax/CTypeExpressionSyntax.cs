using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CTypeExpressionSyntax : CastExpressionSyntax
	{
		internal static Func<ObjectReader, object> CreateInstance;

		internal CTypeExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken)
			: base(kind, keyword, openParenToken, expression, commaToken, type, closeParenToken)
		{
			base._slotCount = 6;
		}

		internal CTypeExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind, keyword, openParenToken, expression, commaToken, type, closeParenToken)
		{
			base._slotCount = 6;
			SetFactoryContext(context);
		}

		internal CTypeExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations, keyword, openParenToken, expression, commaToken, type, closeParenToken)
		{
			base._slotCount = 6;
		}

		internal CTypeExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 6;
		}

		static CTypeExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new CTypeExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(CTypeExpressionSyntax), (ObjectReader r) => new CTypeExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CTypeExpressionSyntax(this, parent, startLocation);
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
			return new CTypeExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _keyword, _openParenToken, _expression, _commaToken, _type, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new CTypeExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _keyword, _openParenToken, _expression, _commaToken, _type, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitCTypeExpression(this);
		}
	}
}
