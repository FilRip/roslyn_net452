using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class WithEventsEventContainerSyntax : EventContainerSyntax
	{
		internal readonly IdentifierTokenSyntax _identifier;

		internal static Func<ObjectReader, object> CreateInstance;

		internal IdentifierTokenSyntax Identifier => _identifier;

		internal WithEventsEventContainerSyntax(SyntaxKind kind, IdentifierTokenSyntax identifier)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
		}

		internal WithEventsEventContainerSyntax(SyntaxKind kind, IdentifierTokenSyntax identifier, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
		}

		internal WithEventsEventContainerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax identifier)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
		}

		internal WithEventsEventContainerSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)reader.ReadValue();
			if (identifierTokenSyntax != null)
			{
				AdjustFlagsAndWidth(identifierTokenSyntax);
				_identifier = identifierTokenSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_identifier);
		}

		static WithEventsEventContainerSyntax()
		{
			CreateInstance = (ObjectReader o) => new WithEventsEventContainerSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(WithEventsEventContainerSyntax), (ObjectReader r) => new WithEventsEventContainerSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _identifier;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new WithEventsEventContainerSyntax(base.Kind, newErrors, GetAnnotations(), _identifier);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new WithEventsEventContainerSyntax(base.Kind, GetDiagnostics(), annotations, _identifier);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitWithEventsEventContainer(this);
		}
	}
}
