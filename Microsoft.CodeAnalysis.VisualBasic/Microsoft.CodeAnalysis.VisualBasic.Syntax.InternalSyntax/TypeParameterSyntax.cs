using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class TypeParameterSyntax : VisualBasicSyntaxNode
	{
		internal readonly KeywordSyntax _varianceKeyword;

		internal readonly IdentifierTokenSyntax _identifier;

		internal readonly TypeParameterConstraintClauseSyntax _typeParameterConstraintClause;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax VarianceKeyword => _varianceKeyword;

		internal IdentifierTokenSyntax Identifier => _identifier;

		internal TypeParameterConstraintClauseSyntax TypeParameterConstraintClause => _typeParameterConstraintClause;

		internal TypeParameterSyntax(SyntaxKind kind, KeywordSyntax varianceKeyword, IdentifierTokenSyntax identifier, TypeParameterConstraintClauseSyntax typeParameterConstraintClause)
			: base(kind)
		{
			base._slotCount = 3;
			if (varianceKeyword != null)
			{
				AdjustFlagsAndWidth(varianceKeyword);
				_varianceKeyword = varianceKeyword;
			}
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (typeParameterConstraintClause != null)
			{
				AdjustFlagsAndWidth(typeParameterConstraintClause);
				_typeParameterConstraintClause = typeParameterConstraintClause;
			}
		}

		internal TypeParameterSyntax(SyntaxKind kind, KeywordSyntax varianceKeyword, IdentifierTokenSyntax identifier, TypeParameterConstraintClauseSyntax typeParameterConstraintClause, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			if (varianceKeyword != null)
			{
				AdjustFlagsAndWidth(varianceKeyword);
				_varianceKeyword = varianceKeyword;
			}
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (typeParameterConstraintClause != null)
			{
				AdjustFlagsAndWidth(typeParameterConstraintClause);
				_typeParameterConstraintClause = typeParameterConstraintClause;
			}
		}

		internal TypeParameterSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax varianceKeyword, IdentifierTokenSyntax identifier, TypeParameterConstraintClauseSyntax typeParameterConstraintClause)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			if (varianceKeyword != null)
			{
				AdjustFlagsAndWidth(varianceKeyword);
				_varianceKeyword = varianceKeyword;
			}
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (typeParameterConstraintClause != null)
			{
				AdjustFlagsAndWidth(typeParameterConstraintClause);
				_typeParameterConstraintClause = typeParameterConstraintClause;
			}
		}

		internal TypeParameterSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_varianceKeyword = keywordSyntax;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)reader.ReadValue();
			if (identifierTokenSyntax != null)
			{
				AdjustFlagsAndWidth(identifierTokenSyntax);
				_identifier = identifierTokenSyntax;
			}
			TypeParameterConstraintClauseSyntax typeParameterConstraintClauseSyntax = (TypeParameterConstraintClauseSyntax)reader.ReadValue();
			if (typeParameterConstraintClauseSyntax != null)
			{
				AdjustFlagsAndWidth(typeParameterConstraintClauseSyntax);
				_typeParameterConstraintClause = typeParameterConstraintClauseSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_varianceKeyword);
			writer.WriteValue(_identifier);
			writer.WriteValue(_typeParameterConstraintClause);
		}

		static TypeParameterSyntax()
		{
			CreateInstance = (ObjectReader o) => new TypeParameterSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(TypeParameterSyntax), (ObjectReader r) => new TypeParameterSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _varianceKeyword, 
				1 => _identifier, 
				2 => _typeParameterConstraintClause, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new TypeParameterSyntax(base.Kind, newErrors, GetAnnotations(), _varianceKeyword, _identifier, _typeParameterConstraintClause);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new TypeParameterSyntax(base.Kind, GetDiagnostics(), annotations, _varianceKeyword, _identifier, _typeParameterConstraintClause);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitTypeParameter(this);
		}
	}
}
