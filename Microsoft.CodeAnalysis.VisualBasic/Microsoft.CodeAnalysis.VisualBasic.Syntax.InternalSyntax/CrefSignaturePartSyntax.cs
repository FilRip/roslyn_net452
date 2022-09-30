using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CrefSignaturePartSyntax : VisualBasicSyntaxNode
	{
		internal readonly KeywordSyntax _modifier;

		internal readonly TypeSyntax _type;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax Modifier => _modifier;

		internal TypeSyntax Type => _type;

		internal CrefSignaturePartSyntax(SyntaxKind kind, KeywordSyntax modifier, TypeSyntax type)
			: base(kind)
		{
			base._slotCount = 2;
			if (modifier != null)
			{
				AdjustFlagsAndWidth(modifier);
				_modifier = modifier;
			}
			if (type != null)
			{
				AdjustFlagsAndWidth(type);
				_type = type;
			}
		}

		internal CrefSignaturePartSyntax(SyntaxKind kind, KeywordSyntax modifier, TypeSyntax type, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			if (modifier != null)
			{
				AdjustFlagsAndWidth(modifier);
				_modifier = modifier;
			}
			if (type != null)
			{
				AdjustFlagsAndWidth(type);
				_type = type;
			}
		}

		internal CrefSignaturePartSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax modifier, TypeSyntax type)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			if (modifier != null)
			{
				AdjustFlagsAndWidth(modifier);
				_modifier = modifier;
			}
			if (type != null)
			{
				AdjustFlagsAndWidth(type);
				_type = type;
			}
		}

		internal CrefSignaturePartSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_modifier = keywordSyntax;
			}
			TypeSyntax typeSyntax = (TypeSyntax)reader.ReadValue();
			if (typeSyntax != null)
			{
				AdjustFlagsAndWidth(typeSyntax);
				_type = typeSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_modifier);
			writer.WriteValue(_type);
		}

		static CrefSignaturePartSyntax()
		{
			CreateInstance = (ObjectReader o) => new CrefSignaturePartSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(CrefSignaturePartSyntax), (ObjectReader r) => new CrefSignaturePartSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _modifier, 
				1 => _type, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new CrefSignaturePartSyntax(base.Kind, newErrors, GetAnnotations(), _modifier, _type);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new CrefSignaturePartSyntax(base.Kind, GetDiagnostics(), annotations, _modifier, _type);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitCrefSignaturePart(this);
		}
	}
}
