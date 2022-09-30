using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class HandlesClauseSyntax : VisualBasicSyntaxNode
	{
		internal readonly KeywordSyntax _handlesKeyword;

		internal readonly GreenNode _events;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax HandlesKeyword => _handlesKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<HandlesClauseItemSyntax> Events => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<HandlesClauseItemSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<HandlesClauseItemSyntax>(_events));

		internal HandlesClauseSyntax(SyntaxKind kind, KeywordSyntax handlesKeyword, GreenNode events)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(handlesKeyword);
			_handlesKeyword = handlesKeyword;
			if (events != null)
			{
				AdjustFlagsAndWidth(events);
				_events = events;
			}
		}

		internal HandlesClauseSyntax(SyntaxKind kind, KeywordSyntax handlesKeyword, GreenNode events, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(handlesKeyword);
			_handlesKeyword = handlesKeyword;
			if (events != null)
			{
				AdjustFlagsAndWidth(events);
				_events = events;
			}
		}

		internal HandlesClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax handlesKeyword, GreenNode events)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(handlesKeyword);
			_handlesKeyword = handlesKeyword;
			if (events != null)
			{
				AdjustFlagsAndWidth(events);
				_events = events;
			}
		}

		internal HandlesClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_handlesKeyword = keywordSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_events = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_handlesKeyword);
			writer.WriteValue(_events);
		}

		static HandlesClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new HandlesClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(HandlesClauseSyntax), (ObjectReader r) => new HandlesClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _handlesKeyword, 
				1 => _events, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new HandlesClauseSyntax(base.Kind, newErrors, GetAnnotations(), _handlesKeyword, _events);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new HandlesClauseSyntax(base.Kind, GetDiagnostics(), annotations, _handlesKeyword, _events);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitHandlesClause(this);
		}
	}
}
