using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class OrderByClauseSyntax : QueryClauseSyntax
	{
		internal readonly KeywordSyntax _orderKeyword;

		internal readonly KeywordSyntax _byKeyword;

		internal readonly GreenNode _orderings;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax OrderKeyword => _orderKeyword;

		internal KeywordSyntax ByKeyword => _byKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<OrderingSyntax> Orderings => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<OrderingSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<OrderingSyntax>(_orderings));

		internal OrderByClauseSyntax(SyntaxKind kind, KeywordSyntax orderKeyword, KeywordSyntax byKeyword, GreenNode orderings)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(orderKeyword);
			_orderKeyword = orderKeyword;
			AdjustFlagsAndWidth(byKeyword);
			_byKeyword = byKeyword;
			if (orderings != null)
			{
				AdjustFlagsAndWidth(orderings);
				_orderings = orderings;
			}
		}

		internal OrderByClauseSyntax(SyntaxKind kind, KeywordSyntax orderKeyword, KeywordSyntax byKeyword, GreenNode orderings, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(orderKeyword);
			_orderKeyword = orderKeyword;
			AdjustFlagsAndWidth(byKeyword);
			_byKeyword = byKeyword;
			if (orderings != null)
			{
				AdjustFlagsAndWidth(orderings);
				_orderings = orderings;
			}
		}

		internal OrderByClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax orderKeyword, KeywordSyntax byKeyword, GreenNode orderings)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(orderKeyword);
			_orderKeyword = orderKeyword;
			AdjustFlagsAndWidth(byKeyword);
			_byKeyword = byKeyword;
			if (orderings != null)
			{
				AdjustFlagsAndWidth(orderings);
				_orderings = orderings;
			}
		}

		internal OrderByClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_orderKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_byKeyword = keywordSyntax2;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_orderings = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_orderKeyword);
			writer.WriteValue(_byKeyword);
			writer.WriteValue(_orderings);
		}

		static OrderByClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new OrderByClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(OrderByClauseSyntax), (ObjectReader r) => new OrderByClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _orderKeyword, 
				1 => _byKeyword, 
				2 => _orderings, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new OrderByClauseSyntax(base.Kind, newErrors, GetAnnotations(), _orderKeyword, _byKeyword, _orderings);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new OrderByClauseSyntax(base.Kind, GetDiagnostics(), annotations, _orderKeyword, _byKeyword, _orderings);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitOrderByClause(this);
		}
	}
}
