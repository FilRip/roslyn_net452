using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlNamespaceImportsClauseSyntax : ImportsClauseSyntax
	{
		internal readonly PunctuationSyntax _lessThanToken;

		internal readonly XmlAttributeSyntax _xmlNamespace;

		internal readonly PunctuationSyntax _greaterThanToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax LessThanToken => _lessThanToken;

		internal XmlAttributeSyntax XmlNamespace => _xmlNamespace;

		internal PunctuationSyntax GreaterThanToken => _greaterThanToken;

		internal XmlNamespaceImportsClauseSyntax(SyntaxKind kind, PunctuationSyntax lessThanToken, XmlAttributeSyntax xmlNamespace, PunctuationSyntax greaterThanToken)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(lessThanToken);
			_lessThanToken = lessThanToken;
			AdjustFlagsAndWidth(xmlNamespace);
			_xmlNamespace = xmlNamespace;
			AdjustFlagsAndWidth(greaterThanToken);
			_greaterThanToken = greaterThanToken;
		}

		internal XmlNamespaceImportsClauseSyntax(SyntaxKind kind, PunctuationSyntax lessThanToken, XmlAttributeSyntax xmlNamespace, PunctuationSyntax greaterThanToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(lessThanToken);
			_lessThanToken = lessThanToken;
			AdjustFlagsAndWidth(xmlNamespace);
			_xmlNamespace = xmlNamespace;
			AdjustFlagsAndWidth(greaterThanToken);
			_greaterThanToken = greaterThanToken;
		}

		internal XmlNamespaceImportsClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanToken, XmlAttributeSyntax xmlNamespace, PunctuationSyntax greaterThanToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(lessThanToken);
			_lessThanToken = lessThanToken;
			AdjustFlagsAndWidth(xmlNamespace);
			_xmlNamespace = xmlNamespace;
			AdjustFlagsAndWidth(greaterThanToken);
			_greaterThanToken = greaterThanToken;
		}

		internal XmlNamespaceImportsClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_lessThanToken = punctuationSyntax;
			}
			XmlAttributeSyntax xmlAttributeSyntax = (XmlAttributeSyntax)reader.ReadValue();
			if (xmlAttributeSyntax != null)
			{
				AdjustFlagsAndWidth(xmlAttributeSyntax);
				_xmlNamespace = xmlAttributeSyntax;
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
			writer.WriteValue(_xmlNamespace);
			writer.WriteValue(_greaterThanToken);
		}

		static XmlNamespaceImportsClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlNamespaceImportsClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlNamespaceImportsClauseSyntax), (ObjectReader r) => new XmlNamespaceImportsClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNamespaceImportsClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _lessThanToken, 
				1 => _xmlNamespace, 
				2 => _greaterThanToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlNamespaceImportsClauseSyntax(base.Kind, newErrors, GetAnnotations(), _lessThanToken, _xmlNamespace, _greaterThanToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlNamespaceImportsClauseSyntax(base.Kind, GetDiagnostics(), annotations, _lessThanToken, _xmlNamespace, _greaterThanToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlNamespaceImportsClause(this);
		}
	}
}
