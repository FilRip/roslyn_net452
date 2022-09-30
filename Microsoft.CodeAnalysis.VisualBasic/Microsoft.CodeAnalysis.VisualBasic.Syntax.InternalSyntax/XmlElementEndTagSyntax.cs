using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlElementEndTagSyntax : XmlNodeSyntax
	{
		internal readonly PunctuationSyntax _lessThanSlashToken;

		internal readonly XmlNameSyntax _name;

		internal readonly PunctuationSyntax _greaterThanToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax LessThanSlashToken => _lessThanSlashToken;

		internal XmlNameSyntax Name => _name;

		internal PunctuationSyntax GreaterThanToken => _greaterThanToken;

		internal XmlElementEndTagSyntax(SyntaxKind kind, PunctuationSyntax lessThanSlashToken, XmlNameSyntax name, PunctuationSyntax greaterThanToken)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(lessThanSlashToken);
			_lessThanSlashToken = lessThanSlashToken;
			if (name != null)
			{
				AdjustFlagsAndWidth(name);
				_name = name;
			}
			AdjustFlagsAndWidth(greaterThanToken);
			_greaterThanToken = greaterThanToken;
		}

		internal XmlElementEndTagSyntax(SyntaxKind kind, PunctuationSyntax lessThanSlashToken, XmlNameSyntax name, PunctuationSyntax greaterThanToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(lessThanSlashToken);
			_lessThanSlashToken = lessThanSlashToken;
			if (name != null)
			{
				AdjustFlagsAndWidth(name);
				_name = name;
			}
			AdjustFlagsAndWidth(greaterThanToken);
			_greaterThanToken = greaterThanToken;
		}

		internal XmlElementEndTagSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanSlashToken, XmlNameSyntax name, PunctuationSyntax greaterThanToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(lessThanSlashToken);
			_lessThanSlashToken = lessThanSlashToken;
			if (name != null)
			{
				AdjustFlagsAndWidth(name);
				_name = name;
			}
			AdjustFlagsAndWidth(greaterThanToken);
			_greaterThanToken = greaterThanToken;
		}

		internal XmlElementEndTagSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_lessThanSlashToken = punctuationSyntax;
			}
			XmlNameSyntax xmlNameSyntax = (XmlNameSyntax)reader.ReadValue();
			if (xmlNameSyntax != null)
			{
				AdjustFlagsAndWidth(xmlNameSyntax);
				_name = xmlNameSyntax;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_greaterThanToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_lessThanSlashToken);
			writer.WriteValue(_name);
			writer.WriteValue(_greaterThanToken);
		}

		static XmlElementEndTagSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlElementEndTagSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlElementEndTagSyntax), (ObjectReader r) => new XmlElementEndTagSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _lessThanSlashToken, 
				1 => _name, 
				2 => _greaterThanToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlElementEndTagSyntax(base.Kind, newErrors, GetAnnotations(), _lessThanSlashToken, _name, _greaterThanToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlElementEndTagSyntax(base.Kind, GetDiagnostics(), annotations, _lessThanSlashToken, _name, _greaterThanToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlElementEndTag(this);
		}
	}
}
