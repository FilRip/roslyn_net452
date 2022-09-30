using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class GetXmlNamespaceExpressionSyntax : ExpressionSyntax
	{
		internal readonly KeywordSyntax _getXmlNamespaceKeyword;

		internal readonly PunctuationSyntax _openParenToken;

		internal readonly XmlPrefixNameSyntax _name;

		internal readonly PunctuationSyntax _closeParenToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax GetXmlNamespaceKeyword => _getXmlNamespaceKeyword;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal XmlPrefixNameSyntax Name => _name;

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal GetXmlNamespaceExpressionSyntax(SyntaxKind kind, KeywordSyntax getXmlNamespaceKeyword, PunctuationSyntax openParenToken, XmlPrefixNameSyntax name, PunctuationSyntax closeParenToken)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(getXmlNamespaceKeyword);
			_getXmlNamespaceKeyword = getXmlNamespaceKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			if (name != null)
			{
				AdjustFlagsAndWidth(name);
				_name = name;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal GetXmlNamespaceExpressionSyntax(SyntaxKind kind, KeywordSyntax getXmlNamespaceKeyword, PunctuationSyntax openParenToken, XmlPrefixNameSyntax name, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(getXmlNamespaceKeyword);
			_getXmlNamespaceKeyword = getXmlNamespaceKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			if (name != null)
			{
				AdjustFlagsAndWidth(name);
				_name = name;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal GetXmlNamespaceExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax getXmlNamespaceKeyword, PunctuationSyntax openParenToken, XmlPrefixNameSyntax name, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(getXmlNamespaceKeyword);
			_getXmlNamespaceKeyword = getXmlNamespaceKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			if (name != null)
			{
				AdjustFlagsAndWidth(name);
				_name = name;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal GetXmlNamespaceExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_getXmlNamespaceKeyword = keywordSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_openParenToken = punctuationSyntax;
			}
			XmlPrefixNameSyntax xmlPrefixNameSyntax = (XmlPrefixNameSyntax)reader.ReadValue();
			if (xmlPrefixNameSyntax != null)
			{
				AdjustFlagsAndWidth(xmlPrefixNameSyntax);
				_name = xmlPrefixNameSyntax;
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
			writer.WriteValue(_getXmlNamespaceKeyword);
			writer.WriteValue(_openParenToken);
			writer.WriteValue(_name);
			writer.WriteValue(_closeParenToken);
		}

		static GetXmlNamespaceExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new GetXmlNamespaceExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(GetXmlNamespaceExpressionSyntax), (ObjectReader r) => new GetXmlNamespaceExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GetXmlNamespaceExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _getXmlNamespaceKeyword, 
				1 => _openParenToken, 
				2 => _name, 
				3 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new GetXmlNamespaceExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _getXmlNamespaceKeyword, _openParenToken, _name, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new GetXmlNamespaceExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _getXmlNamespaceKeyword, _openParenToken, _name, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitGetXmlNamespaceExpression(this);
		}
	}
}
