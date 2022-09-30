using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class GetTypeExpressionSyntax : ExpressionSyntax
	{
		internal readonly KeywordSyntax _getTypeKeyword;

		internal readonly PunctuationSyntax _openParenToken;

		internal readonly TypeSyntax _type;

		internal readonly PunctuationSyntax _closeParenToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax GetTypeKeyword => _getTypeKeyword;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal TypeSyntax Type => _type;

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal GetTypeExpressionSyntax(SyntaxKind kind, KeywordSyntax getTypeKeyword, PunctuationSyntax openParenToken, TypeSyntax type, PunctuationSyntax closeParenToken)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(getTypeKeyword);
			_getTypeKeyword = getTypeKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(type);
			_type = type;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal GetTypeExpressionSyntax(SyntaxKind kind, KeywordSyntax getTypeKeyword, PunctuationSyntax openParenToken, TypeSyntax type, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(getTypeKeyword);
			_getTypeKeyword = getTypeKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(type);
			_type = type;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal GetTypeExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax getTypeKeyword, PunctuationSyntax openParenToken, TypeSyntax type, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(getTypeKeyword);
			_getTypeKeyword = getTypeKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(type);
			_type = type;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal GetTypeExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_getTypeKeyword = keywordSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_openParenToken = punctuationSyntax;
			}
			TypeSyntax typeSyntax = (TypeSyntax)reader.ReadValue();
			if (typeSyntax != null)
			{
				AdjustFlagsAndWidth(typeSyntax);
				_type = typeSyntax;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_closeParenToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_getTypeKeyword);
			writer.WriteValue(_openParenToken);
			writer.WriteValue(_type);
			writer.WriteValue(_closeParenToken);
		}

		static GetTypeExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new GetTypeExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(GetTypeExpressionSyntax), (ObjectReader r) => new GetTypeExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GetTypeExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _getTypeKeyword, 
				1 => _openParenToken, 
				2 => _type, 
				3 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new GetTypeExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _getTypeKeyword, _openParenToken, _type, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new GetTypeExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _getTypeKeyword, _openParenToken, _type, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitGetTypeExpression(this);
		}
	}
}
