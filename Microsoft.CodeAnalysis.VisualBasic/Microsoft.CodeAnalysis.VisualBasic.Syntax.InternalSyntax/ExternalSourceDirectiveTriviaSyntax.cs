using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ExternalSourceDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal readonly KeywordSyntax _externalSourceKeyword;

		internal readonly PunctuationSyntax _openParenToken;

		internal readonly StringLiteralTokenSyntax _externalSource;

		internal readonly PunctuationSyntax _commaToken;

		internal readonly IntegerLiteralTokenSyntax _lineStart;

		internal readonly PunctuationSyntax _closeParenToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ExternalSourceKeyword => _externalSourceKeyword;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal StringLiteralTokenSyntax ExternalSource => _externalSource;

		internal PunctuationSyntax CommaToken => _commaToken;

		internal IntegerLiteralTokenSyntax LineStart => _lineStart;

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal ExternalSourceDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax externalSourceKeyword, PunctuationSyntax openParenToken, StringLiteralTokenSyntax externalSource, PunctuationSyntax commaToken, IntegerLiteralTokenSyntax lineStart, PunctuationSyntax closeParenToken)
			: base(kind, hashToken)
		{
			base._slotCount = 7;
			AdjustFlagsAndWidth(externalSourceKeyword);
			_externalSourceKeyword = externalSourceKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(externalSource);
			_externalSource = externalSource;
			AdjustFlagsAndWidth(commaToken);
			_commaToken = commaToken;
			AdjustFlagsAndWidth(lineStart);
			_lineStart = lineStart;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal ExternalSourceDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax externalSourceKeyword, PunctuationSyntax openParenToken, StringLiteralTokenSyntax externalSource, PunctuationSyntax commaToken, IntegerLiteralTokenSyntax lineStart, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind, hashToken)
		{
			base._slotCount = 7;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(externalSourceKeyword);
			_externalSourceKeyword = externalSourceKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(externalSource);
			_externalSource = externalSource;
			AdjustFlagsAndWidth(commaToken);
			_commaToken = commaToken;
			AdjustFlagsAndWidth(lineStart);
			_lineStart = lineStart;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal ExternalSourceDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax externalSourceKeyword, PunctuationSyntax openParenToken, StringLiteralTokenSyntax externalSource, PunctuationSyntax commaToken, IntegerLiteralTokenSyntax lineStart, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations, hashToken)
		{
			base._slotCount = 7;
			AdjustFlagsAndWidth(externalSourceKeyword);
			_externalSourceKeyword = externalSourceKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(externalSource);
			_externalSource = externalSource;
			AdjustFlagsAndWidth(commaToken);
			_commaToken = commaToken;
			AdjustFlagsAndWidth(lineStart);
			_lineStart = lineStart;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal ExternalSourceDirectiveTriviaSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 7;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_externalSourceKeyword = keywordSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_openParenToken = punctuationSyntax;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax = (StringLiteralTokenSyntax)reader.ReadValue();
			if (stringLiteralTokenSyntax != null)
			{
				AdjustFlagsAndWidth(stringLiteralTokenSyntax);
				_externalSource = stringLiteralTokenSyntax;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_commaToken = punctuationSyntax2;
			}
			IntegerLiteralTokenSyntax integerLiteralTokenSyntax = (IntegerLiteralTokenSyntax)reader.ReadValue();
			if (integerLiteralTokenSyntax != null)
			{
				AdjustFlagsAndWidth(integerLiteralTokenSyntax);
				_lineStart = integerLiteralTokenSyntax;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax3 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax3);
				_closeParenToken = punctuationSyntax3;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_externalSourceKeyword);
			writer.WriteValue(_openParenToken);
			writer.WriteValue(_externalSource);
			writer.WriteValue(_commaToken);
			writer.WriteValue(_lineStart);
			writer.WriteValue(_closeParenToken);
		}

		static ExternalSourceDirectiveTriviaSyntax()
		{
			CreateInstance = (ObjectReader o) => new ExternalSourceDirectiveTriviaSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ExternalSourceDirectiveTriviaSyntax), (ObjectReader r) => new ExternalSourceDirectiveTriviaSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _hashToken, 
				1 => _externalSourceKeyword, 
				2 => _openParenToken, 
				3 => _externalSource, 
				4 => _commaToken, 
				5 => _lineStart, 
				6 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ExternalSourceDirectiveTriviaSyntax(base.Kind, newErrors, GetAnnotations(), _hashToken, _externalSourceKeyword, _openParenToken, _externalSource, _commaToken, _lineStart, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ExternalSourceDirectiveTriviaSyntax(base.Kind, GetDiagnostics(), annotations, _hashToken, _externalSourceKeyword, _openParenToken, _externalSource, _commaToken, _lineStart, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitExternalSourceDirectiveTrivia(this);
		}
	}
}
