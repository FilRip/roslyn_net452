using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlCDataSectionSyntax : XmlNodeSyntax
	{
		internal readonly PunctuationSyntax _beginCDataToken;

		internal readonly GreenNode _textTokens;

		internal readonly PunctuationSyntax _endCDataToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax BeginCDataToken => _beginCDataToken;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlTextTokenSyntax> TextTokens => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>(_textTokens);

		internal PunctuationSyntax EndCDataToken => _endCDataToken;

		internal XmlCDataSectionSyntax(SyntaxKind kind, PunctuationSyntax beginCDataToken, GreenNode textTokens, PunctuationSyntax endCDataToken)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(beginCDataToken);
			_beginCDataToken = beginCDataToken;
			if (textTokens != null)
			{
				AdjustFlagsAndWidth(textTokens);
				_textTokens = textTokens;
			}
			AdjustFlagsAndWidth(endCDataToken);
			_endCDataToken = endCDataToken;
		}

		internal XmlCDataSectionSyntax(SyntaxKind kind, PunctuationSyntax beginCDataToken, GreenNode textTokens, PunctuationSyntax endCDataToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(beginCDataToken);
			_beginCDataToken = beginCDataToken;
			if (textTokens != null)
			{
				AdjustFlagsAndWidth(textTokens);
				_textTokens = textTokens;
			}
			AdjustFlagsAndWidth(endCDataToken);
			_endCDataToken = endCDataToken;
		}

		internal XmlCDataSectionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax beginCDataToken, GreenNode textTokens, PunctuationSyntax endCDataToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(beginCDataToken);
			_beginCDataToken = beginCDataToken;
			if (textTokens != null)
			{
				AdjustFlagsAndWidth(textTokens);
				_textTokens = textTokens;
			}
			AdjustFlagsAndWidth(endCDataToken);
			_endCDataToken = endCDataToken;
		}

		internal XmlCDataSectionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_beginCDataToken = punctuationSyntax;
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
				_endCDataToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_beginCDataToken);
			writer.WriteValue(_textTokens);
			writer.WriteValue(_endCDataToken);
		}

		static XmlCDataSectionSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlCDataSectionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlCDataSectionSyntax), (ObjectReader r) => new XmlCDataSectionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCDataSectionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _beginCDataToken, 
				1 => _textTokens, 
				2 => _endCDataToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlCDataSectionSyntax(base.Kind, newErrors, GetAnnotations(), _beginCDataToken, _textTokens, _endCDataToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlCDataSectionSyntax(base.Kind, GetDiagnostics(), annotations, _beginCDataToken, _textTokens, _endCDataToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlCDataSection(this);
		}
	}
}
