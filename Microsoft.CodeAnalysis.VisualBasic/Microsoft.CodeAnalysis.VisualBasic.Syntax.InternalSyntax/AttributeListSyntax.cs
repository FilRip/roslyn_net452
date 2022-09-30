using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class AttributeListSyntax : VisualBasicSyntaxNode
	{
		internal readonly PunctuationSyntax _lessThanToken;

		internal readonly GreenNode _attributes;

		internal readonly PunctuationSyntax _greaterThanToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax LessThanToken => _lessThanToken;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AttributeSyntax> Attributes => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AttributeSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeSyntax>(_attributes));

		internal PunctuationSyntax GreaterThanToken => _greaterThanToken;

		internal AttributeListSyntax(SyntaxKind kind, PunctuationSyntax lessThanToken, GreenNode attributes, PunctuationSyntax greaterThanToken)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(lessThanToken);
			_lessThanToken = lessThanToken;
			if (attributes != null)
			{
				AdjustFlagsAndWidth(attributes);
				_attributes = attributes;
			}
			AdjustFlagsAndWidth(greaterThanToken);
			_greaterThanToken = greaterThanToken;
		}

		internal AttributeListSyntax(SyntaxKind kind, PunctuationSyntax lessThanToken, GreenNode attributes, PunctuationSyntax greaterThanToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(lessThanToken);
			_lessThanToken = lessThanToken;
			if (attributes != null)
			{
				AdjustFlagsAndWidth(attributes);
				_attributes = attributes;
			}
			AdjustFlagsAndWidth(greaterThanToken);
			_greaterThanToken = greaterThanToken;
		}

		internal AttributeListSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanToken, GreenNode attributes, PunctuationSyntax greaterThanToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(lessThanToken);
			_lessThanToken = lessThanToken;
			if (attributes != null)
			{
				AdjustFlagsAndWidth(attributes);
				_attributes = attributes;
			}
			AdjustFlagsAndWidth(greaterThanToken);
			_greaterThanToken = greaterThanToken;
		}

		internal AttributeListSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_lessThanToken = punctuationSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_attributes = greenNode;
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
			writer.WriteValue(_attributes);
			writer.WriteValue(_greaterThanToken);
		}

		static AttributeListSyntax()
		{
			CreateInstance = (ObjectReader o) => new AttributeListSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(AttributeListSyntax), (ObjectReader r) => new AttributeListSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _lessThanToken, 
				1 => _attributes, 
				2 => _greaterThanToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new AttributeListSyntax(base.Kind, newErrors, GetAnnotations(), _lessThanToken, _attributes, _greaterThanToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new AttributeListSyntax(base.Kind, GetDiagnostics(), annotations, _lessThanToken, _attributes, _greaterThanToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitAttributeList(this);
		}
	}
}
