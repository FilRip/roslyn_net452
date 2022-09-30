using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class TypedTupleElementSyntax : TupleElementSyntax
	{
		internal readonly TypeSyntax _type;

		internal static Func<ObjectReader, object> CreateInstance;

		internal TypeSyntax Type => _type;

		internal TypedTupleElementSyntax(SyntaxKind kind, TypeSyntax type)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(type);
			_type = type;
		}

		internal TypedTupleElementSyntax(SyntaxKind kind, TypeSyntax type, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(type);
			_type = type;
		}

		internal TypedTupleElementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, TypeSyntax type)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(type);
			_type = type;
		}

		internal TypedTupleElementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
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
			writer.WriteValue(_type);
		}

		static TypedTupleElementSyntax()
		{
			CreateInstance = (ObjectReader o) => new TypedTupleElementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(TypedTupleElementSyntax), (ObjectReader r) => new TypedTupleElementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TypedTupleElementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _type;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new TypedTupleElementSyntax(base.Kind, newErrors, GetAnnotations(), _type);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new TypedTupleElementSyntax(base.Kind, GetDiagnostics(), annotations, _type);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitTypedTupleElement(this);
		}
	}
}
