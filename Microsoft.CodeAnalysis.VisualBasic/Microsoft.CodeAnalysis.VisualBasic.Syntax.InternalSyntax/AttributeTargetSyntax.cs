using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class AttributeTargetSyntax : VisualBasicSyntaxNode
	{
		internal readonly KeywordSyntax _attributeModifier;

		internal readonly PunctuationSyntax _colonToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax AttributeModifier => _attributeModifier;

		internal PunctuationSyntax ColonToken => _colonToken;

		internal AttributeTargetSyntax(SyntaxKind kind, KeywordSyntax attributeModifier, PunctuationSyntax colonToken)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(attributeModifier);
			_attributeModifier = attributeModifier;
			AdjustFlagsAndWidth(colonToken);
			_colonToken = colonToken;
		}

		internal AttributeTargetSyntax(SyntaxKind kind, KeywordSyntax attributeModifier, PunctuationSyntax colonToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(attributeModifier);
			_attributeModifier = attributeModifier;
			AdjustFlagsAndWidth(colonToken);
			_colonToken = colonToken;
		}

		internal AttributeTargetSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax attributeModifier, PunctuationSyntax colonToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(attributeModifier);
			_attributeModifier = attributeModifier;
			AdjustFlagsAndWidth(colonToken);
			_colonToken = colonToken;
		}

		internal AttributeTargetSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_attributeModifier = keywordSyntax;
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
			writer.WriteValue(_attributeModifier);
			writer.WriteValue(_colonToken);
		}

		static AttributeTargetSyntax()
		{
			CreateInstance = (ObjectReader o) => new AttributeTargetSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(AttributeTargetSyntax), (ObjectReader r) => new AttributeTargetSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeModifier, 
				1 => _colonToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new AttributeTargetSyntax(base.Kind, newErrors, GetAnnotations(), _attributeModifier, _colonToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new AttributeTargetSyntax(base.Kind, GetDiagnostics(), annotations, _attributeModifier, _colonToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitAttributeTarget(this);
		}
	}
}
