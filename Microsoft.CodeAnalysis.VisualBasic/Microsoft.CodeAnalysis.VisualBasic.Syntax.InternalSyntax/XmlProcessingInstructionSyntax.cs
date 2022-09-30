using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlProcessingInstructionSyntax : XmlNodeSyntax
	{
		internal readonly PunctuationSyntax _lessThanQuestionToken;

		internal readonly XmlNameTokenSyntax _name;

		internal readonly GreenNode _textTokens;

		internal readonly PunctuationSyntax _questionGreaterThanToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax LessThanQuestionToken => _lessThanQuestionToken;

		internal XmlNameTokenSyntax Name => _name;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlTextTokenSyntax> TextTokens => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>(_textTokens);

		internal PunctuationSyntax QuestionGreaterThanToken => _questionGreaterThanToken;

		internal XmlProcessingInstructionSyntax(SyntaxKind kind, PunctuationSyntax lessThanQuestionToken, XmlNameTokenSyntax name, GreenNode textTokens, PunctuationSyntax questionGreaterThanToken)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(lessThanQuestionToken);
			_lessThanQuestionToken = lessThanQuestionToken;
			AdjustFlagsAndWidth(name);
			_name = name;
			if (textTokens != null)
			{
				AdjustFlagsAndWidth(textTokens);
				_textTokens = textTokens;
			}
			AdjustFlagsAndWidth(questionGreaterThanToken);
			_questionGreaterThanToken = questionGreaterThanToken;
		}

		internal XmlProcessingInstructionSyntax(SyntaxKind kind, PunctuationSyntax lessThanQuestionToken, XmlNameTokenSyntax name, GreenNode textTokens, PunctuationSyntax questionGreaterThanToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(lessThanQuestionToken);
			_lessThanQuestionToken = lessThanQuestionToken;
			AdjustFlagsAndWidth(name);
			_name = name;
			if (textTokens != null)
			{
				AdjustFlagsAndWidth(textTokens);
				_textTokens = textTokens;
			}
			AdjustFlagsAndWidth(questionGreaterThanToken);
			_questionGreaterThanToken = questionGreaterThanToken;
		}

		internal XmlProcessingInstructionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanQuestionToken, XmlNameTokenSyntax name, GreenNode textTokens, PunctuationSyntax questionGreaterThanToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(lessThanQuestionToken);
			_lessThanQuestionToken = lessThanQuestionToken;
			AdjustFlagsAndWidth(name);
			_name = name;
			if (textTokens != null)
			{
				AdjustFlagsAndWidth(textTokens);
				_textTokens = textTokens;
			}
			AdjustFlagsAndWidth(questionGreaterThanToken);
			_questionGreaterThanToken = questionGreaterThanToken;
		}

		internal XmlProcessingInstructionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_lessThanQuestionToken = punctuationSyntax;
			}
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)reader.ReadValue();
			if (xmlNameTokenSyntax != null)
			{
				AdjustFlagsAndWidth(xmlNameTokenSyntax);
				_name = xmlNameTokenSyntax;
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
				_questionGreaterThanToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_lessThanQuestionToken);
			writer.WriteValue(_name);
			writer.WriteValue(_textTokens);
			writer.WriteValue(_questionGreaterThanToken);
		}

		static XmlProcessingInstructionSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlProcessingInstructionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlProcessingInstructionSyntax), (ObjectReader r) => new XmlProcessingInstructionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlProcessingInstructionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _lessThanQuestionToken, 
				1 => _name, 
				2 => _textTokens, 
				3 => _questionGreaterThanToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlProcessingInstructionSyntax(base.Kind, newErrors, GetAnnotations(), _lessThanQuestionToken, _name, _textTokens, _questionGreaterThanToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlProcessingInstructionSyntax(base.Kind, GetDiagnostics(), annotations, _lessThanQuestionToken, _name, _textTokens, _questionGreaterThanToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlProcessingInstruction(this);
		}
	}
}
