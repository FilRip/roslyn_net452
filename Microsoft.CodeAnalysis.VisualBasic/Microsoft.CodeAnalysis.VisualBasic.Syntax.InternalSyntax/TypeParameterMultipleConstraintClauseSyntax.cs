using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class TypeParameterMultipleConstraintClauseSyntax : TypeParameterConstraintClauseSyntax
	{
		internal readonly KeywordSyntax _asKeyword;

		internal readonly PunctuationSyntax _openBraceToken;

		internal readonly GreenNode _constraints;

		internal readonly PunctuationSyntax _closeBraceToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax AsKeyword => _asKeyword;

		internal PunctuationSyntax OpenBraceToken => _openBraceToken;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ConstraintSyntax> Constraints => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ConstraintSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ConstraintSyntax>(_constraints));

		internal PunctuationSyntax CloseBraceToken => _closeBraceToken;

		internal TypeParameterMultipleConstraintClauseSyntax(SyntaxKind kind, KeywordSyntax asKeyword, PunctuationSyntax openBraceToken, GreenNode constraints, PunctuationSyntax closeBraceToken)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(asKeyword);
			_asKeyword = asKeyword;
			AdjustFlagsAndWidth(openBraceToken);
			_openBraceToken = openBraceToken;
			if (constraints != null)
			{
				AdjustFlagsAndWidth(constraints);
				_constraints = constraints;
			}
			AdjustFlagsAndWidth(closeBraceToken);
			_closeBraceToken = closeBraceToken;
		}

		internal TypeParameterMultipleConstraintClauseSyntax(SyntaxKind kind, KeywordSyntax asKeyword, PunctuationSyntax openBraceToken, GreenNode constraints, PunctuationSyntax closeBraceToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(asKeyword);
			_asKeyword = asKeyword;
			AdjustFlagsAndWidth(openBraceToken);
			_openBraceToken = openBraceToken;
			if (constraints != null)
			{
				AdjustFlagsAndWidth(constraints);
				_constraints = constraints;
			}
			AdjustFlagsAndWidth(closeBraceToken);
			_closeBraceToken = closeBraceToken;
		}

		internal TypeParameterMultipleConstraintClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax asKeyword, PunctuationSyntax openBraceToken, GreenNode constraints, PunctuationSyntax closeBraceToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(asKeyword);
			_asKeyword = asKeyword;
			AdjustFlagsAndWidth(openBraceToken);
			_openBraceToken = openBraceToken;
			if (constraints != null)
			{
				AdjustFlagsAndWidth(constraints);
				_constraints = constraints;
			}
			AdjustFlagsAndWidth(closeBraceToken);
			_closeBraceToken = closeBraceToken;
		}

		internal TypeParameterMultipleConstraintClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_asKeyword = keywordSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_openBraceToken = punctuationSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_constraints = greenNode;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_closeBraceToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_asKeyword);
			writer.WriteValue(_openBraceToken);
			writer.WriteValue(_constraints);
			writer.WriteValue(_closeBraceToken);
		}

		static TypeParameterMultipleConstraintClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new TypeParameterMultipleConstraintClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(TypeParameterMultipleConstraintClauseSyntax), (ObjectReader r) => new TypeParameterMultipleConstraintClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterMultipleConstraintClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _asKeyword, 
				1 => _openBraceToken, 
				2 => _constraints, 
				3 => _closeBraceToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new TypeParameterMultipleConstraintClauseSyntax(base.Kind, newErrors, GetAnnotations(), _asKeyword, _openBraceToken, _constraints, _closeBraceToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new TypeParameterMultipleConstraintClauseSyntax(base.Kind, GetDiagnostics(), annotations, _asKeyword, _openBraceToken, _constraints, _closeBraceToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitTypeParameterMultipleConstraintClause(this);
		}
	}
}
