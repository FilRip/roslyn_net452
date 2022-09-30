using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlTextSyntax : XmlNodeSyntax
	{
		internal readonly GreenNode _textTokens;

		internal static Func<ObjectReader, object> CreateInstance;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlTextTokenSyntax> TextTokens => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>(_textTokens);

		internal XmlTextSyntax(SyntaxKind kind, GreenNode textTokens)
			: base(kind)
		{
			base._slotCount = 1;
			if (textTokens != null)
			{
				AdjustFlagsAndWidth(textTokens);
				_textTokens = textTokens;
			}
		}

		internal XmlTextSyntax(SyntaxKind kind, GreenNode textTokens, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			if (textTokens != null)
			{
				AdjustFlagsAndWidth(textTokens);
				_textTokens = textTokens;
			}
		}

		internal XmlTextSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode textTokens)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			if (textTokens != null)
			{
				AdjustFlagsAndWidth(textTokens);
				_textTokens = textTokens;
			}
		}

		internal XmlTextSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_textTokens = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_textTokens);
		}

		static XmlTextSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlTextSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlTextSyntax), (ObjectReader r) => new XmlTextSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlTextSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _textTokens;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlTextSyntax(base.Kind, newErrors, GetAnnotations(), _textTokens);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlTextSyntax(base.Kind, GetDiagnostics(), annotations, _textTokens);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlText(this);
		}
	}
}
