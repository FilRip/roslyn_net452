using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ExternalChecksumDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal readonly KeywordSyntax _externalChecksumKeyword;

		internal readonly PunctuationSyntax _openParenToken;

		internal readonly StringLiteralTokenSyntax _externalSource;

		internal readonly PunctuationSyntax _firstCommaToken;

		internal readonly StringLiteralTokenSyntax _guid;

		internal readonly PunctuationSyntax _secondCommaToken;

		internal readonly StringLiteralTokenSyntax _checksum;

		internal readonly PunctuationSyntax _closeParenToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ExternalChecksumKeyword => _externalChecksumKeyword;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal StringLiteralTokenSyntax ExternalSource => _externalSource;

		internal PunctuationSyntax FirstCommaToken => _firstCommaToken;

		internal StringLiteralTokenSyntax Guid => _guid;

		internal PunctuationSyntax SecondCommaToken => _secondCommaToken;

		internal StringLiteralTokenSyntax Checksum => _checksum;

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal ExternalChecksumDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax externalChecksumKeyword, PunctuationSyntax openParenToken, StringLiteralTokenSyntax externalSource, PunctuationSyntax firstCommaToken, StringLiteralTokenSyntax guid, PunctuationSyntax secondCommaToken, StringLiteralTokenSyntax checksum, PunctuationSyntax closeParenToken)
			: base(kind, hashToken)
		{
			base._slotCount = 9;
			AdjustFlagsAndWidth(externalChecksumKeyword);
			_externalChecksumKeyword = externalChecksumKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(externalSource);
			_externalSource = externalSource;
			AdjustFlagsAndWidth(firstCommaToken);
			_firstCommaToken = firstCommaToken;
			AdjustFlagsAndWidth(guid);
			_guid = guid;
			AdjustFlagsAndWidth(secondCommaToken);
			_secondCommaToken = secondCommaToken;
			AdjustFlagsAndWidth(checksum);
			_checksum = checksum;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal ExternalChecksumDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax externalChecksumKeyword, PunctuationSyntax openParenToken, StringLiteralTokenSyntax externalSource, PunctuationSyntax firstCommaToken, StringLiteralTokenSyntax guid, PunctuationSyntax secondCommaToken, StringLiteralTokenSyntax checksum, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind, hashToken)
		{
			base._slotCount = 9;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(externalChecksumKeyword);
			_externalChecksumKeyword = externalChecksumKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(externalSource);
			_externalSource = externalSource;
			AdjustFlagsAndWidth(firstCommaToken);
			_firstCommaToken = firstCommaToken;
			AdjustFlagsAndWidth(guid);
			_guid = guid;
			AdjustFlagsAndWidth(secondCommaToken);
			_secondCommaToken = secondCommaToken;
			AdjustFlagsAndWidth(checksum);
			_checksum = checksum;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal ExternalChecksumDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax externalChecksumKeyword, PunctuationSyntax openParenToken, StringLiteralTokenSyntax externalSource, PunctuationSyntax firstCommaToken, StringLiteralTokenSyntax guid, PunctuationSyntax secondCommaToken, StringLiteralTokenSyntax checksum, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations, hashToken)
		{
			base._slotCount = 9;
			AdjustFlagsAndWidth(externalChecksumKeyword);
			_externalChecksumKeyword = externalChecksumKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(externalSource);
			_externalSource = externalSource;
			AdjustFlagsAndWidth(firstCommaToken);
			_firstCommaToken = firstCommaToken;
			AdjustFlagsAndWidth(guid);
			_guid = guid;
			AdjustFlagsAndWidth(secondCommaToken);
			_secondCommaToken = secondCommaToken;
			AdjustFlagsAndWidth(checksum);
			_checksum = checksum;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal ExternalChecksumDirectiveTriviaSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 9;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_externalChecksumKeyword = keywordSyntax;
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
				_firstCommaToken = punctuationSyntax2;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax2 = (StringLiteralTokenSyntax)reader.ReadValue();
			if (stringLiteralTokenSyntax2 != null)
			{
				AdjustFlagsAndWidth(stringLiteralTokenSyntax2);
				_guid = stringLiteralTokenSyntax2;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax3 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax3);
				_secondCommaToken = punctuationSyntax3;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax3 = (StringLiteralTokenSyntax)reader.ReadValue();
			if (stringLiteralTokenSyntax3 != null)
			{
				AdjustFlagsAndWidth(stringLiteralTokenSyntax3);
				_checksum = stringLiteralTokenSyntax3;
			}
			PunctuationSyntax punctuationSyntax4 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax4 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax4);
				_closeParenToken = punctuationSyntax4;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_externalChecksumKeyword);
			writer.WriteValue(_openParenToken);
			writer.WriteValue(_externalSource);
			writer.WriteValue(_firstCommaToken);
			writer.WriteValue(_guid);
			writer.WriteValue(_secondCommaToken);
			writer.WriteValue(_checksum);
			writer.WriteValue(_closeParenToken);
		}

		static ExternalChecksumDirectiveTriviaSyntax()
		{
			CreateInstance = (ObjectReader o) => new ExternalChecksumDirectiveTriviaSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ExternalChecksumDirectiveTriviaSyntax), (ObjectReader r) => new ExternalChecksumDirectiveTriviaSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _hashToken, 
				1 => _externalChecksumKeyword, 
				2 => _openParenToken, 
				3 => _externalSource, 
				4 => _firstCommaToken, 
				5 => _guid, 
				6 => _secondCommaToken, 
				7 => _checksum, 
				8 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ExternalChecksumDirectiveTriviaSyntax(base.Kind, newErrors, GetAnnotations(), _hashToken, _externalChecksumKeyword, _openParenToken, _externalSource, _firstCommaToken, _guid, _secondCommaToken, _checksum, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ExternalChecksumDirectiveTriviaSyntax(base.Kind, GetDiagnostics(), annotations, _hashToken, _externalChecksumKeyword, _openParenToken, _externalSource, _firstCommaToken, _guid, _secondCommaToken, _checksum, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitExternalChecksumDirectiveTrivia(this);
		}
	}
}
