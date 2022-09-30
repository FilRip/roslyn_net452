using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class HandlesClauseItemSyntax : VisualBasicSyntaxNode
	{
		internal readonly EventContainerSyntax _eventContainer;

		internal readonly PunctuationSyntax _dotToken;

		internal readonly IdentifierNameSyntax _eventMember;

		internal static Func<ObjectReader, object> CreateInstance;

		internal EventContainerSyntax EventContainer => _eventContainer;

		internal PunctuationSyntax DotToken => _dotToken;

		internal IdentifierNameSyntax EventMember => _eventMember;

		internal HandlesClauseItemSyntax(SyntaxKind kind, EventContainerSyntax eventContainer, PunctuationSyntax dotToken, IdentifierNameSyntax eventMember)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(eventContainer);
			_eventContainer = eventContainer;
			AdjustFlagsAndWidth(dotToken);
			_dotToken = dotToken;
			AdjustFlagsAndWidth(eventMember);
			_eventMember = eventMember;
		}

		internal HandlesClauseItemSyntax(SyntaxKind kind, EventContainerSyntax eventContainer, PunctuationSyntax dotToken, IdentifierNameSyntax eventMember, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(eventContainer);
			_eventContainer = eventContainer;
			AdjustFlagsAndWidth(dotToken);
			_dotToken = dotToken;
			AdjustFlagsAndWidth(eventMember);
			_eventMember = eventMember;
		}

		internal HandlesClauseItemSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, EventContainerSyntax eventContainer, PunctuationSyntax dotToken, IdentifierNameSyntax eventMember)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(eventContainer);
			_eventContainer = eventContainer;
			AdjustFlagsAndWidth(dotToken);
			_dotToken = dotToken;
			AdjustFlagsAndWidth(eventMember);
			_eventMember = eventMember;
		}

		internal HandlesClauseItemSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			EventContainerSyntax eventContainerSyntax = (EventContainerSyntax)reader.ReadValue();
			if (eventContainerSyntax != null)
			{
				AdjustFlagsAndWidth(eventContainerSyntax);
				_eventContainer = eventContainerSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_dotToken = punctuationSyntax;
			}
			IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)reader.ReadValue();
			if (identifierNameSyntax != null)
			{
				AdjustFlagsAndWidth(identifierNameSyntax);
				_eventMember = identifierNameSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_eventContainer);
			writer.WriteValue(_dotToken);
			writer.WriteValue(_eventMember);
		}

		static HandlesClauseItemSyntax()
		{
			CreateInstance = (ObjectReader o) => new HandlesClauseItemSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(HandlesClauseItemSyntax), (ObjectReader r) => new HandlesClauseItemSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _eventContainer, 
				1 => _dotToken, 
				2 => _eventMember, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new HandlesClauseItemSyntax(base.Kind, newErrors, GetAnnotations(), _eventContainer, _dotToken, _eventMember);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new HandlesClauseItemSyntax(base.Kind, GetDiagnostics(), annotations, _eventContainer, _dotToken, _eventMember);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitHandlesClauseItem(this);
		}
	}
}
