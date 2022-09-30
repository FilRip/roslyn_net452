using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SimpleJoinClauseSyntax : JoinClauseSyntax
	{
		internal static Func<ObjectReader, object> CreateInstance;

		internal SimpleJoinClauseSyntax(SyntaxKind kind, KeywordSyntax joinKeyword, GreenNode joinedVariables, GreenNode additionalJoins, KeywordSyntax onKeyword, GreenNode joinConditions)
			: base(kind, joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions)
		{
			base._slotCount = 5;
		}

		internal SimpleJoinClauseSyntax(SyntaxKind kind, KeywordSyntax joinKeyword, GreenNode joinedVariables, GreenNode additionalJoins, KeywordSyntax onKeyword, GreenNode joinConditions, ISyntaxFactoryContext context)
			: base(kind, joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
		}

		internal SimpleJoinClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax joinKeyword, GreenNode joinedVariables, GreenNode additionalJoins, KeywordSyntax onKeyword, GreenNode joinConditions)
			: base(kind, errors, annotations, joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions)
		{
			base._slotCount = 5;
		}

		internal SimpleJoinClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
		}

		static SimpleJoinClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new SimpleJoinClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SimpleJoinClauseSyntax), (ObjectReader r) => new SimpleJoinClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _joinKeyword, 
				1 => _joinedVariables, 
				2 => _additionalJoins, 
				3 => _onKeyword, 
				4 => _joinConditions, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SimpleJoinClauseSyntax(base.Kind, newErrors, GetAnnotations(), _joinKeyword, _joinedVariables, _additionalJoins, _onKeyword, _joinConditions);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SimpleJoinClauseSyntax(base.Kind, GetDiagnostics(), annotations, _joinKeyword, _joinedVariables, _additionalJoins, _onKeyword, _joinConditions);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSimpleJoinClause(this);
		}
	}
}
