using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlDeclarationSyntax : VisualBasicSyntaxNode
	{
		internal readonly PunctuationSyntax _lessThanQuestionToken;

		internal readonly KeywordSyntax _xmlKeyword;

		internal readonly XmlDeclarationOptionSyntax _version;

		internal readonly XmlDeclarationOptionSyntax _encoding;

		internal readonly XmlDeclarationOptionSyntax _standalone;

		internal readonly PunctuationSyntax _questionGreaterThanToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax LessThanQuestionToken => _lessThanQuestionToken;

		internal KeywordSyntax XmlKeyword => _xmlKeyword;

		internal XmlDeclarationOptionSyntax Version => _version;

		internal XmlDeclarationOptionSyntax Encoding => _encoding;

		internal XmlDeclarationOptionSyntax Standalone => _standalone;

		internal PunctuationSyntax QuestionGreaterThanToken => _questionGreaterThanToken;

		internal XmlDeclarationSyntax(SyntaxKind kind, PunctuationSyntax lessThanQuestionToken, KeywordSyntax xmlKeyword, XmlDeclarationOptionSyntax version, XmlDeclarationOptionSyntax encoding, XmlDeclarationOptionSyntax standalone, PunctuationSyntax questionGreaterThanToken)
			: base(kind)
		{
			base._slotCount = 6;
			AdjustFlagsAndWidth(lessThanQuestionToken);
			_lessThanQuestionToken = lessThanQuestionToken;
			AdjustFlagsAndWidth(xmlKeyword);
			_xmlKeyword = xmlKeyword;
			AdjustFlagsAndWidth(version);
			_version = version;
			if (encoding != null)
			{
				AdjustFlagsAndWidth(encoding);
				_encoding = encoding;
			}
			if (standalone != null)
			{
				AdjustFlagsAndWidth(standalone);
				_standalone = standalone;
			}
			AdjustFlagsAndWidth(questionGreaterThanToken);
			_questionGreaterThanToken = questionGreaterThanToken;
		}

		internal XmlDeclarationSyntax(SyntaxKind kind, PunctuationSyntax lessThanQuestionToken, KeywordSyntax xmlKeyword, XmlDeclarationOptionSyntax version, XmlDeclarationOptionSyntax encoding, XmlDeclarationOptionSyntax standalone, PunctuationSyntax questionGreaterThanToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 6;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(lessThanQuestionToken);
			_lessThanQuestionToken = lessThanQuestionToken;
			AdjustFlagsAndWidth(xmlKeyword);
			_xmlKeyword = xmlKeyword;
			AdjustFlagsAndWidth(version);
			_version = version;
			if (encoding != null)
			{
				AdjustFlagsAndWidth(encoding);
				_encoding = encoding;
			}
			if (standalone != null)
			{
				AdjustFlagsAndWidth(standalone);
				_standalone = standalone;
			}
			AdjustFlagsAndWidth(questionGreaterThanToken);
			_questionGreaterThanToken = questionGreaterThanToken;
		}

		internal XmlDeclarationSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanQuestionToken, KeywordSyntax xmlKeyword, XmlDeclarationOptionSyntax version, XmlDeclarationOptionSyntax encoding, XmlDeclarationOptionSyntax standalone, PunctuationSyntax questionGreaterThanToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 6;
			AdjustFlagsAndWidth(lessThanQuestionToken);
			_lessThanQuestionToken = lessThanQuestionToken;
			AdjustFlagsAndWidth(xmlKeyword);
			_xmlKeyword = xmlKeyword;
			AdjustFlagsAndWidth(version);
			_version = version;
			if (encoding != null)
			{
				AdjustFlagsAndWidth(encoding);
				_encoding = encoding;
			}
			if (standalone != null)
			{
				AdjustFlagsAndWidth(standalone);
				_standalone = standalone;
			}
			AdjustFlagsAndWidth(questionGreaterThanToken);
			_questionGreaterThanToken = questionGreaterThanToken;
		}

		internal XmlDeclarationSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 6;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_lessThanQuestionToken = punctuationSyntax;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_xmlKeyword = keywordSyntax;
			}
			XmlDeclarationOptionSyntax xmlDeclarationOptionSyntax = (XmlDeclarationOptionSyntax)reader.ReadValue();
			if (xmlDeclarationOptionSyntax != null)
			{
				AdjustFlagsAndWidth(xmlDeclarationOptionSyntax);
				_version = xmlDeclarationOptionSyntax;
			}
			XmlDeclarationOptionSyntax xmlDeclarationOptionSyntax2 = (XmlDeclarationOptionSyntax)reader.ReadValue();
			if (xmlDeclarationOptionSyntax2 != null)
			{
				AdjustFlagsAndWidth(xmlDeclarationOptionSyntax2);
				_encoding = xmlDeclarationOptionSyntax2;
			}
			XmlDeclarationOptionSyntax xmlDeclarationOptionSyntax3 = (XmlDeclarationOptionSyntax)reader.ReadValue();
			if (xmlDeclarationOptionSyntax3 != null)
			{
				AdjustFlagsAndWidth(xmlDeclarationOptionSyntax3);
				_standalone = xmlDeclarationOptionSyntax3;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_questionGreaterThanToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_lessThanQuestionToken);
			writer.WriteValue(_xmlKeyword);
			writer.WriteValue(_version);
			writer.WriteValue(_encoding);
			writer.WriteValue(_standalone);
			writer.WriteValue(_questionGreaterThanToken);
		}

		static XmlDeclarationSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlDeclarationSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlDeclarationSyntax), (ObjectReader r) => new XmlDeclarationSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _lessThanQuestionToken, 
				1 => _xmlKeyword, 
				2 => _version, 
				3 => _encoding, 
				4 => _standalone, 
				5 => _questionGreaterThanToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlDeclarationSyntax(base.Kind, newErrors, GetAnnotations(), _lessThanQuestionToken, _xmlKeyword, _version, _encoding, _standalone, _questionGreaterThanToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlDeclarationSyntax(base.Kind, GetDiagnostics(), annotations, _lessThanQuestionToken, _xmlKeyword, _version, _encoding, _standalone, _questionGreaterThanToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlDeclaration(this);
		}
	}
}
