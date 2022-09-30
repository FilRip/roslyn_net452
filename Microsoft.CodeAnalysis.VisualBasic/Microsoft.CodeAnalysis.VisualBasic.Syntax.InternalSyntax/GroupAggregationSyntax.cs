using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class GroupAggregationSyntax : AggregationSyntax
	{
		internal readonly KeywordSyntax _groupKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax GroupKeyword => _groupKeyword;

		internal GroupAggregationSyntax(SyntaxKind kind, KeywordSyntax groupKeyword)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(groupKeyword);
			_groupKeyword = groupKeyword;
		}

		internal GroupAggregationSyntax(SyntaxKind kind, KeywordSyntax groupKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(groupKeyword);
			_groupKeyword = groupKeyword;
		}

		internal GroupAggregationSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax groupKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(groupKeyword);
			_groupKeyword = groupKeyword;
		}

		internal GroupAggregationSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_groupKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_groupKeyword);
		}

		static GroupAggregationSyntax()
		{
			CreateInstance = (ObjectReader o) => new GroupAggregationSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(GroupAggregationSyntax), (ObjectReader r) => new GroupAggregationSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupAggregationSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _groupKeyword;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new GroupAggregationSyntax(base.Kind, newErrors, GetAnnotations(), _groupKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new GroupAggregationSyntax(base.Kind, GetDiagnostics(), annotations, _groupKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitGroupAggregation(this);
		}
	}
}
