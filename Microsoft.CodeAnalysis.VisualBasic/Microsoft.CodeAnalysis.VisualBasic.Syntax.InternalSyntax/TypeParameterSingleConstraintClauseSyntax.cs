using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class TypeParameterSingleConstraintClauseSyntax : TypeParameterConstraintClauseSyntax
	{
		internal readonly KeywordSyntax _asKeyword;

		internal readonly ConstraintSyntax _constraint;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax AsKeyword => _asKeyword;

		internal ConstraintSyntax Constraint => _constraint;

		internal TypeParameterSingleConstraintClauseSyntax(SyntaxKind kind, KeywordSyntax asKeyword, ConstraintSyntax constraint)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(asKeyword);
			_asKeyword = asKeyword;
			AdjustFlagsAndWidth(constraint);
			_constraint = constraint;
		}

		internal TypeParameterSingleConstraintClauseSyntax(SyntaxKind kind, KeywordSyntax asKeyword, ConstraintSyntax constraint, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(asKeyword);
			_asKeyword = asKeyword;
			AdjustFlagsAndWidth(constraint);
			_constraint = constraint;
		}

		internal TypeParameterSingleConstraintClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax asKeyword, ConstraintSyntax constraint)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(asKeyword);
			_asKeyword = asKeyword;
			AdjustFlagsAndWidth(constraint);
			_constraint = constraint;
		}

		internal TypeParameterSingleConstraintClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_asKeyword = keywordSyntax;
			}
			ConstraintSyntax constraintSyntax = (ConstraintSyntax)reader.ReadValue();
			if (constraintSyntax != null)
			{
				AdjustFlagsAndWidth(constraintSyntax);
				_constraint = constraintSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_asKeyword);
			writer.WriteValue(_constraint);
		}

		static TypeParameterSingleConstraintClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new TypeParameterSingleConstraintClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(TypeParameterSingleConstraintClauseSyntax), (ObjectReader r) => new TypeParameterSingleConstraintClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSingleConstraintClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _asKeyword, 
				1 => _constraint, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new TypeParameterSingleConstraintClauseSyntax(base.Kind, newErrors, GetAnnotations(), _asKeyword, _constraint);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new TypeParameterSingleConstraintClauseSyntax(base.Kind, GetDiagnostics(), annotations, _asKeyword, _constraint);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitTypeParameterSingleConstraintClause(this);
		}
	}
}
