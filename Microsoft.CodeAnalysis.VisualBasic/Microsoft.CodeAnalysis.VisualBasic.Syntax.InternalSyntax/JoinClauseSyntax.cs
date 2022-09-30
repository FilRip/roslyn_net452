using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class JoinClauseSyntax : QueryClauseSyntax
	{
		internal readonly KeywordSyntax _joinKeyword;

		internal readonly GreenNode _joinedVariables;

		internal readonly GreenNode _additionalJoins;

		internal readonly KeywordSyntax _onKeyword;

		internal readonly GreenNode _joinConditions;

		internal KeywordSyntax JoinKeyword => _joinKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CollectionRangeVariableSyntax> JoinedVariables => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CollectionRangeVariableSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CollectionRangeVariableSyntax>(_joinedVariables));

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<JoinClauseSyntax> AdditionalJoins => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<JoinClauseSyntax>(_additionalJoins);

		internal KeywordSyntax OnKeyword => _onKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<JoinConditionSyntax> JoinConditions => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<JoinConditionSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<JoinConditionSyntax>(_joinConditions));

		internal JoinClauseSyntax(SyntaxKind kind, KeywordSyntax joinKeyword, GreenNode joinedVariables, GreenNode additionalJoins, KeywordSyntax onKeyword, GreenNode joinConditions)
			: base(kind)
		{
			AdjustFlagsAndWidth(joinKeyword);
			_joinKeyword = joinKeyword;
			if (joinedVariables != null)
			{
				AdjustFlagsAndWidth(joinedVariables);
				_joinedVariables = joinedVariables;
			}
			if (additionalJoins != null)
			{
				AdjustFlagsAndWidth(additionalJoins);
				_additionalJoins = additionalJoins;
			}
			AdjustFlagsAndWidth(onKeyword);
			_onKeyword = onKeyword;
			if (joinConditions != null)
			{
				AdjustFlagsAndWidth(joinConditions);
				_joinConditions = joinConditions;
			}
		}

		internal JoinClauseSyntax(SyntaxKind kind, KeywordSyntax joinKeyword, GreenNode joinedVariables, GreenNode additionalJoins, KeywordSyntax onKeyword, GreenNode joinConditions, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
			AdjustFlagsAndWidth(joinKeyword);
			_joinKeyword = joinKeyword;
			if (joinedVariables != null)
			{
				AdjustFlagsAndWidth(joinedVariables);
				_joinedVariables = joinedVariables;
			}
			if (additionalJoins != null)
			{
				AdjustFlagsAndWidth(additionalJoins);
				_additionalJoins = additionalJoins;
			}
			AdjustFlagsAndWidth(onKeyword);
			_onKeyword = onKeyword;
			if (joinConditions != null)
			{
				AdjustFlagsAndWidth(joinConditions);
				_joinConditions = joinConditions;
			}
		}

		internal JoinClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax joinKeyword, GreenNode joinedVariables, GreenNode additionalJoins, KeywordSyntax onKeyword, GreenNode joinConditions)
			: base(kind, errors, annotations)
		{
			AdjustFlagsAndWidth(joinKeyword);
			_joinKeyword = joinKeyword;
			if (joinedVariables != null)
			{
				AdjustFlagsAndWidth(joinedVariables);
				_joinedVariables = joinedVariables;
			}
			if (additionalJoins != null)
			{
				AdjustFlagsAndWidth(additionalJoins);
				_additionalJoins = additionalJoins;
			}
			AdjustFlagsAndWidth(onKeyword);
			_onKeyword = onKeyword;
			if (joinConditions != null)
			{
				AdjustFlagsAndWidth(joinConditions);
				_joinConditions = joinConditions;
			}
		}

		internal JoinClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_joinKeyword = keywordSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_joinedVariables = greenNode;
			}
			GreenNode greenNode2 = (GreenNode)reader.ReadValue();
			if (greenNode2 != null)
			{
				AdjustFlagsAndWidth(greenNode2);
				_additionalJoins = greenNode2;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_onKeyword = keywordSyntax2;
			}
			GreenNode greenNode3 = (GreenNode)reader.ReadValue();
			if (greenNode3 != null)
			{
				AdjustFlagsAndWidth(greenNode3);
				_joinConditions = greenNode3;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_joinKeyword);
			writer.WriteValue(_joinedVariables);
			writer.WriteValue(_additionalJoins);
			writer.WriteValue(_onKeyword);
			writer.WriteValue(_joinConditions);
		}
	}
}
