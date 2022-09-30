using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlCommentSyntax : XmlNodeSyntax
	{
		internal readonly PunctuationSyntax _lessThanExclamationMinusMinusToken;

		internal readonly GreenNode _textTokens;

		internal readonly PunctuationSyntax _minusMinusGreaterThanToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax LessThanExclamationMinusMinusToken => _lessThanExclamationMinusMinusToken;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlTextTokenSyntax> TextTokens => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>(_textTokens);

		internal PunctuationSyntax MinusMinusGreaterThanToken => _minusMinusGreaterThanToken;

		internal XmlCommentSyntax(SyntaxKind kind, PunctuationSyntax lessThanExclamationMinusMinusToken, GreenNode textTokens, PunctuationSyntax minusMinusGreaterThanToken)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(lessThanExclamationMinusMinusToken);
			_lessThanExclamationMinusMinusToken = lessThanExclamationMinusMinusToken;
			if (textTokens != null)
			{
				AdjustFlagsAndWidth(textTokens);
				_textTokens = textTokens;
			}
			AdjustFlagsAndWidth(minusMinusGreaterThanToken);
			_minusMinusGreaterThanToken = minusMinusGreaterThanToken;
		}

		internal XmlCommentSyntax(SyntaxKind kind, PunctuationSyntax lessThanExclamationMinusMinusToken, GreenNode textTokens, PunctuationSyntax minusMinusGreaterThanToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(lessThanExclamationMinusMinusToken);
			_lessThanExclamationMinusMinusToken = lessThanExclamationMinusMinusToken;
			if (textTokens != null)
			{
				AdjustFlagsAndWidth(textTokens);
				_textTokens = textTokens;
			}
			AdjustFlagsAndWidth(minusMinusGreaterThanToken);
			_minusMinusGreaterThanToken = minusMinusGreaterThanToken;
		}

		internal XmlCommentSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanExclamationMinusMinusToken, GreenNode textTokens, PunctuationSyntax minusMinusGreaterThanToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(lessThanExclamationMinusMinusToken);
			_lessThanExclamationMinusMinusToken = lessThanExclamationMinusMinusToken;
			if (textTokens != null)
			{
				AdjustFlagsAndWidth(textTokens);
				_textTokens = textTokens;
			}
			AdjustFlagsAndWidth(minusMinusGreaterThanToken);
			_minusMinusGreaterThanToken = minusMinusGreaterThanToken;
		}

		internal XmlCommentSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_lessThanExclamationMinusMinusToken = punctuationSyntax;
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
				_minusMinusGreaterThanToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_lessThanExclamationMinusMinusToken);
			writer.WriteValue(_textTokens);
			writer.WriteValue(_minusMinusGreaterThanToken);
		}

		static XmlCommentSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlCommentSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlCommentSyntax), (ObjectReader r) => new XmlCommentSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCommentSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _lessThanExclamationMinusMinusToken, 
				1 => _textTokens, 
				2 => _minusMinusGreaterThanToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlCommentSyntax(base.Kind, newErrors, GetAnnotations(), _lessThanExclamationMinusMinusToken, _textTokens, _minusMinusGreaterThanToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlCommentSyntax(base.Kind, GetDiagnostics(), annotations, _lessThanExclamationMinusMinusToken, _textTokens, _minusMinusGreaterThanToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlComment(this);
		}
	}
}
