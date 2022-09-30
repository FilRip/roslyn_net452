using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlStringSyntax : XmlNodeSyntax
	{
		internal readonly PunctuationSyntax _startQuoteToken;

		internal readonly GreenNode _textTokens;

		internal readonly PunctuationSyntax _endQuoteToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax StartQuoteToken => _startQuoteToken;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlTextTokenSyntax> TextTokens => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>(_textTokens);

		internal PunctuationSyntax EndQuoteToken => _endQuoteToken;

		internal XmlStringSyntax(SyntaxKind kind, PunctuationSyntax startQuoteToken, GreenNode textTokens, PunctuationSyntax endQuoteToken)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(startQuoteToken);
			_startQuoteToken = startQuoteToken;
			if (textTokens != null)
			{
				AdjustFlagsAndWidth(textTokens);
				_textTokens = textTokens;
			}
			AdjustFlagsAndWidth(endQuoteToken);
			_endQuoteToken = endQuoteToken;
		}

		internal XmlStringSyntax(SyntaxKind kind, PunctuationSyntax startQuoteToken, GreenNode textTokens, PunctuationSyntax endQuoteToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(startQuoteToken);
			_startQuoteToken = startQuoteToken;
			if (textTokens != null)
			{
				AdjustFlagsAndWidth(textTokens);
				_textTokens = textTokens;
			}
			AdjustFlagsAndWidth(endQuoteToken);
			_endQuoteToken = endQuoteToken;
		}

		internal XmlStringSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax startQuoteToken, GreenNode textTokens, PunctuationSyntax endQuoteToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(startQuoteToken);
			_startQuoteToken = startQuoteToken;
			if (textTokens != null)
			{
				AdjustFlagsAndWidth(textTokens);
				_textTokens = textTokens;
			}
			AdjustFlagsAndWidth(endQuoteToken);
			_endQuoteToken = endQuoteToken;
		}

		internal XmlStringSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_startQuoteToken = punctuationSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_textTokens = greenNode;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_endQuoteToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_startQuoteToken);
			writer.WriteValue(_textTokens);
			writer.WriteValue(_endQuoteToken);
		}

		static XmlStringSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlStringSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlStringSyntax), (ObjectReader r) => new XmlStringSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _startQuoteToken, 
				1 => _textTokens, 
				2 => _endQuoteToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlStringSyntax(base.Kind, newErrors, GetAnnotations(), _startQuoteToken, _textTokens, _endQuoteToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlStringSyntax(base.Kind, GetDiagnostics(), annotations, _startQuoteToken, _textTokens, _endQuoteToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlString(this);
		}
	}
}
