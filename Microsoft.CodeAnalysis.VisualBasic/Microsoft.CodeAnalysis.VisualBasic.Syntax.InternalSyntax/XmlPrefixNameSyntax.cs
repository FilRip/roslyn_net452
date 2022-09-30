using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlPrefixNameSyntax : XmlNodeSyntax
	{
		internal readonly XmlNameTokenSyntax _name;

		internal static Func<ObjectReader, object> CreateInstance;

		internal XmlNameTokenSyntax Name => _name;

		internal XmlPrefixNameSyntax(SyntaxKind kind, XmlNameTokenSyntax name)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal XmlPrefixNameSyntax(SyntaxKind kind, XmlNameTokenSyntax name, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal XmlPrefixNameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlNameTokenSyntax name)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal XmlPrefixNameSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)reader.ReadValue();
			if (xmlNameTokenSyntax != null)
			{
				AdjustFlagsAndWidth(xmlNameTokenSyntax);
				_name = xmlNameTokenSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_name);
		}

		static XmlPrefixNameSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlPrefixNameSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlPrefixNameSyntax), (ObjectReader r) => new XmlPrefixNameSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _name;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlPrefixNameSyntax(base.Kind, newErrors, GetAnnotations(), _name);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlPrefixNameSyntax(base.Kind, GetDiagnostics(), annotations, _name);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlPrefixName(this);
		}
	}
}
