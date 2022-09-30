using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ArrayTypeSyntax : TypeSyntax
	{
		internal readonly TypeSyntax _elementType;

		internal readonly GreenNode _rankSpecifiers;

		internal static Func<ObjectReader, object> CreateInstance;

		internal TypeSyntax ElementType => _elementType;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax> RankSpecifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax>(_rankSpecifiers);

		internal ArrayTypeSyntax(SyntaxKind kind, TypeSyntax elementType, GreenNode rankSpecifiers)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(elementType);
			_elementType = elementType;
			if (rankSpecifiers != null)
			{
				AdjustFlagsAndWidth(rankSpecifiers);
				_rankSpecifiers = rankSpecifiers;
			}
		}

		internal ArrayTypeSyntax(SyntaxKind kind, TypeSyntax elementType, GreenNode rankSpecifiers, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(elementType);
			_elementType = elementType;
			if (rankSpecifiers != null)
			{
				AdjustFlagsAndWidth(rankSpecifiers);
				_rankSpecifiers = rankSpecifiers;
			}
		}

		internal ArrayTypeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, TypeSyntax elementType, GreenNode rankSpecifiers)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(elementType);
			_elementType = elementType;
			if (rankSpecifiers != null)
			{
				AdjustFlagsAndWidth(rankSpecifiers);
				_rankSpecifiers = rankSpecifiers;
			}
		}

		internal ArrayTypeSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			TypeSyntax typeSyntax = (TypeSyntax)reader.ReadValue();
			if (typeSyntax != null)
			{
				AdjustFlagsAndWidth(typeSyntax);
				_elementType = typeSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_rankSpecifiers = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_elementType);
			writer.WriteValue(_rankSpecifiers);
		}

		static ArrayTypeSyntax()
		{
			CreateInstance = (ObjectReader o) => new ArrayTypeSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ArrayTypeSyntax), (ObjectReader r) => new ArrayTypeSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayTypeSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _elementType, 
				1 => _rankSpecifiers, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ArrayTypeSyntax(base.Kind, newErrors, GetAnnotations(), _elementType, _rankSpecifiers);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ArrayTypeSyntax(base.Kind, GetDiagnostics(), annotations, _elementType, _rankSpecifiers);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitArrayType(this);
		}
	}
}
