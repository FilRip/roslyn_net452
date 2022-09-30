using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlNameSyntax : XmlNodeSyntax
	{
		internal readonly XmlPrefixSyntax _prefix;

		internal readonly XmlNameTokenSyntax _localName;

		internal static Func<ObjectReader, object> CreateInstance;

		internal XmlPrefixSyntax Prefix => _prefix;

		internal XmlNameTokenSyntax LocalName => _localName;

		internal XmlNameSyntax(SyntaxKind kind, XmlPrefixSyntax prefix, XmlNameTokenSyntax localName)
			: base(kind)
		{
			base._slotCount = 2;
			if (prefix != null)
			{
				AdjustFlagsAndWidth(prefix);
				_prefix = prefix;
			}
			AdjustFlagsAndWidth(localName);
			_localName = localName;
		}

		internal XmlNameSyntax(SyntaxKind kind, XmlPrefixSyntax prefix, XmlNameTokenSyntax localName, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			if (prefix != null)
			{
				AdjustFlagsAndWidth(prefix);
				_prefix = prefix;
			}
			AdjustFlagsAndWidth(localName);
			_localName = localName;
		}

		internal XmlNameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlPrefixSyntax prefix, XmlNameTokenSyntax localName)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			if (prefix != null)
			{
				AdjustFlagsAndWidth(prefix);
				_prefix = prefix;
			}
			AdjustFlagsAndWidth(localName);
			_localName = localName;
		}

		internal XmlNameSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			XmlPrefixSyntax xmlPrefixSyntax = (XmlPrefixSyntax)reader.ReadValue();
			if (xmlPrefixSyntax != null)
			{
				AdjustFlagsAndWidth(xmlPrefixSyntax);
				_prefix = xmlPrefixSyntax;
			}
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)reader.ReadValue();
			if (xmlNameTokenSyntax != null)
			{
				AdjustFlagsAndWidth(xmlNameTokenSyntax);
				_localName = xmlNameTokenSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_prefix);
			writer.WriteValue(_localName);
		}

		static XmlNameSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlNameSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlNameSyntax), (ObjectReader r) => new XmlNameSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _prefix, 
				1 => _localName, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlNameSyntax(base.Kind, newErrors, GetAnnotations(), _prefix, _localName);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlNameSyntax(base.Kind, GetDiagnostics(), annotations, _prefix, _localName);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlName(this);
		}
	}
}
