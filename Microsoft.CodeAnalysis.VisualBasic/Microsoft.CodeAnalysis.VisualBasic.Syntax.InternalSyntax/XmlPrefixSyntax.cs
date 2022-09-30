using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlPrefixSyntax : VisualBasicSyntaxNode
	{
		internal readonly XmlNameTokenSyntax _name;

		internal readonly PunctuationSyntax _colonToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal XmlNameTokenSyntax Name => _name;

		internal PunctuationSyntax ColonToken => _colonToken;

		internal XmlPrefixSyntax(SyntaxKind kind, XmlNameTokenSyntax name, PunctuationSyntax colonToken)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(colonToken);
			_colonToken = colonToken;
		}

		internal XmlPrefixSyntax(SyntaxKind kind, XmlNameTokenSyntax name, PunctuationSyntax colonToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(colonToken);
			_colonToken = colonToken;
		}

		internal XmlPrefixSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlNameTokenSyntax name, PunctuationSyntax colonToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(colonToken);
			_colonToken = colonToken;
		}

		internal XmlPrefixSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)reader.ReadValue();
			if (xmlNameTokenSyntax != null)
			{
				AdjustFlagsAndWidth(xmlNameTokenSyntax);
				_name = xmlNameTokenSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_colonToken = punctuationSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_name);
			writer.WriteValue(_colonToken);
		}

		static XmlPrefixSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlPrefixSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlPrefixSyntax), (ObjectReader r) => new XmlPrefixSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _name, 
				1 => _colonToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlPrefixSyntax(base.Kind, newErrors, GetAnnotations(), _name, _colonToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlPrefixSyntax(base.Kind, GetDiagnostics(), annotations, _name, _colonToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlPrefix(this);
		}
	}
}
