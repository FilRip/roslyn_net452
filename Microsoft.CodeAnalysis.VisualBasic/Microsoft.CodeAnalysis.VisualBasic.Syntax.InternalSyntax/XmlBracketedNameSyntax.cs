using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlBracketedNameSyntax : XmlNodeSyntax
	{
		internal readonly PunctuationSyntax _lessThanToken;

		internal readonly XmlNameSyntax _name;

		internal readonly PunctuationSyntax _greaterThanToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax LessThanToken => _lessThanToken;

		internal XmlNameSyntax Name => _name;

		internal PunctuationSyntax GreaterThanToken => _greaterThanToken;

		internal XmlBracketedNameSyntax(SyntaxKind kind, PunctuationSyntax lessThanToken, XmlNameSyntax name, PunctuationSyntax greaterThanToken)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(lessThanToken);
			_lessThanToken = lessThanToken;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(greaterThanToken);
			_greaterThanToken = greaterThanToken;
		}

		internal XmlBracketedNameSyntax(SyntaxKind kind, PunctuationSyntax lessThanToken, XmlNameSyntax name, PunctuationSyntax greaterThanToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(lessThanToken);
			_lessThanToken = lessThanToken;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(greaterThanToken);
			_greaterThanToken = greaterThanToken;
		}

		internal XmlBracketedNameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanToken, XmlNameSyntax name, PunctuationSyntax greaterThanToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(lessThanToken);
			_lessThanToken = lessThanToken;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(greaterThanToken);
			_greaterThanToken = greaterThanToken;
		}

		internal XmlBracketedNameSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_lessThanToken = punctuationSyntax;
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
			writer.WriteValue(_lessThanToken);
			writer.WriteValue(_name);
			writer.WriteValue(_greaterThanToken);
		}

		static XmlBracketedNameSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlBracketedNameSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlBracketedNameSyntax), (ObjectReader r) => new XmlBracketedNameSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlBracketedNameSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _lessThanToken, 
				1 => _name, 
				2 => _greaterThanToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlBracketedNameSyntax(base.Kind, newErrors, GetAnnotations(), _lessThanToken, _name, _greaterThanToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlBracketedNameSyntax(base.Kind, GetDiagnostics(), annotations, _lessThanToken, _name, _greaterThanToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlBracketedName(this);
		}
	}
}
