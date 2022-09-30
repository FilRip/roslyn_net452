using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class TypeConstraintSyntax : ConstraintSyntax
	{
		internal readonly TypeSyntax _type;

		internal static Func<ObjectReader, object> CreateInstance;

		internal TypeSyntax Type => _type;

		internal TypeConstraintSyntax(SyntaxKind kind, TypeSyntax type)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(type);
			_type = type;
		}

		internal TypeConstraintSyntax(SyntaxKind kind, TypeSyntax type, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(type);
			_type = type;
		}

		internal TypeConstraintSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, TypeSyntax type)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(type);
			_type = type;
		}

		internal TypeConstraintSyntax(ObjectReader reader)
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

		static TypeConstraintSyntax()
		{
			CreateInstance = (ObjectReader o) => new TypeConstraintSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(TypeConstraintSyntax), (ObjectReader r) => new TypeConstraintSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeConstraintSyntax(this, parent, startLocation);
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
			return new TypeConstraintSyntax(base.Kind, newErrors, GetAnnotations(), _type);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new TypeConstraintSyntax(base.Kind, GetDiagnostics(), annotations, _type);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitTypeConstraint(this);
		}
	}
}
