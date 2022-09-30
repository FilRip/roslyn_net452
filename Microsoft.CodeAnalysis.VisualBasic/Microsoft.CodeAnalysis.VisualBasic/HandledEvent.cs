using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	public sealed class HandledEvent
	{
		private readonly HandledEventKind _kind;

		private readonly EventSymbol _eventSymbol;

		private readonly PropertySymbol _WithEventsContainerOpt;

		private readonly PropertySymbol _WithEventsSourcePropertyOpt;

		internal readonly BoundExpression delegateCreation;

		internal readonly MethodSymbol hookupMethod;

		public HandledEventKind HandlesKind => _kind;

		public IEventSymbol EventSymbol => _eventSymbol;

		public IPropertySymbol EventContainer => _WithEventsContainerOpt;

		public IPropertySymbol WithEventsSourceProperty => _WithEventsSourcePropertyOpt;

		internal HandledEvent(HandledEventKind kind, EventSymbol eventSymbol, PropertySymbol withEventsContainerOpt, PropertySymbol withEventsSourcePropertyOpt, BoundExpression delegateCreation, MethodSymbol hookupMethod)
		{
			_kind = kind;
			_eventSymbol = eventSymbol;
			_WithEventsContainerOpt = withEventsContainerOpt;
			_WithEventsSourcePropertyOpt = withEventsSourcePropertyOpt;
			this.delegateCreation = delegateCreation;
			this.hookupMethod = hookupMethod;
		}
	}
}
