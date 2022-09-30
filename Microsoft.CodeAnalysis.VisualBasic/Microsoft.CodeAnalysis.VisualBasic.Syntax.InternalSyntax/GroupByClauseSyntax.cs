using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class GroupByClauseSyntax : QueryClauseSyntax
	{
		internal readonly KeywordSyntax _groupKeyword;

		internal readonly GreenNode _items;

		internal readonly KeywordSyntax _byKeyword;

		internal readonly GreenNode _keys;

		internal readonly KeywordSyntax _intoKeyword;

		internal readonly GreenNode _aggregationVariables;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax GroupKeyword => _groupKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax> Items => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ExpressionRangeVariableSyntax>(_items));

		internal KeywordSyntax ByKeyword => _byKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax> Keys => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ExpressionRangeVariableSyntax>(_keys));

		internal KeywordSyntax IntoKeyword => _intoKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax> AggregationVariables => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AggregationRangeVariableSyntax>(_aggregationVariables));

		internal GroupByClauseSyntax(SyntaxKind kind, KeywordSyntax groupKeyword, GreenNode items, KeywordSyntax byKeyword, GreenNode keys, KeywordSyntax intoKeyword, GreenNode aggregationVariables)
			: base(kind)
		{
			base._slotCount = 6;
			AdjustFlagsAndWidth(groupKeyword);
			_groupKeyword = groupKeyword;
			if (items != null)
			{
				AdjustFlagsAndWidth(items);
				_items = items;
			}
			AdjustFlagsAndWidth(byKeyword);
			_byKeyword = byKeyword;
			if (keys != null)
			{
				AdjustFlagsAndWidth(keys);
				_keys = keys;
			}
			AdjustFlagsAndWidth(intoKeyword);
			_intoKeyword = intoKeyword;
			if (aggregationVariables != null)
			{
				AdjustFlagsAndWidth(aggregationVariables);
				_aggregationVariables = aggregationVariables;
			}
		}

		internal GroupByClauseSyntax(SyntaxKind kind, KeywordSyntax groupKeyword, GreenNode items, KeywordSyntax byKeyword, GreenNode keys, KeywordSyntax intoKeyword, GreenNode aggregationVariables, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 6;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(groupKeyword);
			_groupKeyword = groupKeyword;
			if (items != null)
			{
				AdjustFlagsAndWidth(items);
				_items = items;
			}
			AdjustFlagsAndWidth(byKeyword);
			_byKeyword = byKeyword;
			if (keys != null)
			{
				AdjustFlagsAndWidth(keys);
				_keys = keys;
			}
			AdjustFlagsAndWidth(intoKeyword);
			_intoKeyword = intoKeyword;
			if (aggregationVariables != null)
			{
				AdjustFlagsAndWidth(aggregationVariables);
				_aggregationVariables = aggregationVariables;
			}
		}

		internal GroupByClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax groupKeyword, GreenNode items, KeywordSyntax byKeyword, GreenNode keys, KeywordSyntax intoKeyword, GreenNode aggregationVariables)
			: base(kind, errors, annotations)
		{
			base._slotCount = 6;
			AdjustFlagsAndWidth(groupKeyword);
			_groupKeyword = groupKeyword;
			if (items != null)
			{
				AdjustFlagsAndWidth(items);
				_items = items;
			}
			AdjustFlagsAndWidth(byKeyword);
			_byKeyword = byKeyword;
			if (keys != null)
			{
				AdjustFlagsAndWidth(keys);
				_keys = keys;
			}
			AdjustFlagsAndWidth(intoKeyword);
			_intoKeyword = intoKeyword;
			if (aggregationVariables != null)
			{
				AdjustFlagsAndWidth(aggregationVariables);
				_aggregationVariables = aggregationVariables;
			}
		}

		internal GroupByClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 6;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_groupKeyword = keywordSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_items = greenNode;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_byKeyword = keywordSyntax2;
			}
			GreenNode greenNode2 = (GreenNode)reader.ReadValue();
			if (greenNode2 != null)
			{
				AdjustFlagsAndWidth(greenNode2);
				_keys = greenNode2;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax3 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax3);
				_intoKeyword = keywordSyntax3;
			}
			GreenNode greenNode3 = (GreenNode)reader.ReadValue();
			if (greenNode3 != null)
			{
				AdjustFlagsAndWidth(greenNode3);
				_aggregationVariables = greenNode3;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_groupKeyword);
			writer.WriteValue(_items);
			writer.WriteValue(_byKeyword);
			writer.WriteValue(_keys);
			writer.WriteValue(_intoKeyword);
			writer.WriteValue(_aggregationVariables);
		}

		static GroupByClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new GroupByClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(GroupByClauseSyntax), (ObjectReader r) => new GroupByClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _groupKeyword, 
				1 => _items, 
				2 => _byKeyword, 
				3 => _keys, 
				4 => _intoKeyword, 
				5 => _aggregationVariables, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new GroupByClauseSyntax(base.Kind, newErrors, GetAnnotations(), _groupKeyword, _items, _byKeyword, _keys, _intoKeyword, _aggregationVariables);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new GroupByClauseSyntax(base.Kind, GetDiagnostics(), annotations, _groupKeyword, _items, _byKeyword, _keys, _intoKeyword, _aggregationVariables);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitGroupByClause(this);
		}
	}
}
