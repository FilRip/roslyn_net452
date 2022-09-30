using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SpecialConstraintSyntax : ConstraintSyntax
	{
		internal readonly KeywordSyntax _constraintKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ConstraintKeyword => _constraintKeyword;

		internal SpecialConstraintSyntax(SyntaxKind kind, KeywordSyntax constraintKeyword)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(constraintKeyword);
			_constraintKeyword = constraintKeyword;
		}

		internal SpecialConstraintSyntax(SyntaxKind kind, KeywordSyntax constraintKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(constraintKeyword);
			_constraintKeyword = constraintKeyword;
		}

		internal SpecialConstraintSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax constraintKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(constraintKeyword);
			_constraintKeyword = constraintKeyword;
		}

		internal SpecialConstraintSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_constraintKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_constraintKeyword);
		}

		static SpecialConstraintSyntax()
		{
			CreateInstance = (ObjectReader o) => new SpecialConstraintSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SpecialConstraintSyntax), (ObjectReader r) => new SpecialConstraintSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SpecialConstraintSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _constraintKeyword;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SpecialConstraintSyntax(base.Kind, newErrors, GetAnnotations(), _constraintKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SpecialConstraintSyntax(base.Kind, GetDiagnostics(), annotations, _constraintKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSpecialConstraint(this);
		}
	}
}
