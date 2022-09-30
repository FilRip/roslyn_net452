using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class GroupJoinClauseSyntax : JoinClauseSyntax
	{
		internal readonly KeywordSyntax _groupKeyword;

		internal readonly KeywordSyntax _intoKeyword;

		internal readonly GreenNode _aggregationVariables;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax GroupKeyword => _groupKeyword;

		internal KeywordSyntax IntoKeyword => _intoKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax> AggregationVariables => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AggregationRangeVariableSyntax>(_aggregationVariables));

		internal GroupJoinClauseSyntax(SyntaxKind kind, KeywordSyntax groupKeyword, KeywordSyntax joinKeyword, GreenNode joinedVariables, GreenNode additionalJoins, KeywordSyntax onKeyword, GreenNode joinConditions, KeywordSyntax intoKeyword, GreenNode aggregationVariables)
			: base(kind, joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions)
		{
			base._slotCount = 8;
			AdjustFlagsAndWidth(groupKeyword);
			_groupKeyword = groupKeyword;
			AdjustFlagsAndWidth(intoKeyword);
			_intoKeyword = intoKeyword;
			if (aggregationVariables != null)
			{
				AdjustFlagsAndWidth(aggregationVariables);
				_aggregationVariables = aggregationVariables;
			}
		}

		internal GroupJoinClauseSyntax(SyntaxKind kind, KeywordSyntax groupKeyword, KeywordSyntax joinKeyword, GreenNode joinedVariables, GreenNode additionalJoins, KeywordSyntax onKeyword, GreenNode joinConditions, KeywordSyntax intoKeyword, GreenNode aggregationVariables, ISyntaxFactoryContext context)
			: base(kind, joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions)
		{
			base._slotCount = 8;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(groupKeyword);
			_groupKeyword = groupKeyword;
			AdjustFlagsAndWidth(intoKeyword);
			_intoKeyword = intoKeyword;
			if (aggregationVariables != null)
			{
				AdjustFlagsAndWidth(aggregationVariables);
				_aggregationVariables = aggregationVariables;
			}
		}

		internal GroupJoinClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax groupKeyword, KeywordSyntax joinKeyword, GreenNode joinedVariables, GreenNode additionalJoins, KeywordSyntax onKeyword, GreenNode joinConditions, KeywordSyntax intoKeyword, GreenNode aggregationVariables)
			: base(kind, errors, annotations, joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions)
		{
			base._slotCount = 8;
			AdjustFlagsAndWidth(groupKeyword);
			_groupKeyword = groupKeyword;
			AdjustFlagsAndWidth(intoKeyword);
			_intoKeyword = intoKeyword;
			if (aggregationVariables != null)
			{
				AdjustFlagsAndWidth(aggregationVariables);
				_aggregationVariables = aggregationVariables;
			}
		}

		internal GroupJoinClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 8;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_groupKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_intoKeyword = keywordSyntax2;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_aggregationVariables = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_groupKeyword);
			writer.WriteValue(_intoKeyword);
			writer.WriteValue(_aggregationVariables);
		}

		static GroupJoinClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new GroupJoinClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(GroupJoinClauseSyntax), (ObjectReader r) => new GroupJoinClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _groupKeyword, 
				1 => _joinKeyword, 
				2 => _joinedVariables, 
				3 => _additionalJoins, 
				4 => _onKeyword, 
				5 => _joinConditions, 
				6 => _intoKeyword, 
				7 => _aggregationVariables, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new GroupJoinClauseSyntax(base.Kind, newErrors, GetAnnotations(), _groupKeyword, _joinKeyword, _joinedVariables, _additionalJoins, _onKeyword, _joinConditions, _intoKeyword, _aggregationVariables);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new GroupJoinClauseSyntax(base.Kind, GetDiagnostics(), annotations, _groupKeyword, _joinKeyword, _joinedVariables, _additionalJoins, _onKeyword, _joinConditions, _intoKeyword, _aggregationVariables);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitGroupJoinClause(this);
		}
	}
}
