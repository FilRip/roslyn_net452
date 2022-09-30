using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class WithEventsPropertyEventContainerSyntax : EventContainerSyntax
	{
		internal readonly WithEventsEventContainerSyntax _withEventsContainer;

		internal readonly PunctuationSyntax _dotToken;

		internal readonly IdentifierNameSyntax _property;

		internal static Func<ObjectReader, object> CreateInstance;

		internal WithEventsEventContainerSyntax WithEventsContainer => _withEventsContainer;

		internal PunctuationSyntax DotToken => _dotToken;

		internal IdentifierNameSyntax Property => _property;

		internal WithEventsPropertyEventContainerSyntax(SyntaxKind kind, WithEventsEventContainerSyntax withEventsContainer, PunctuationSyntax dotToken, IdentifierNameSyntax property)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(withEventsContainer);
			_withEventsContainer = withEventsContainer;
			AdjustFlagsAndWidth(dotToken);
			_dotToken = dotToken;
			AdjustFlagsAndWidth(property);
			_property = property;
		}

		internal WithEventsPropertyEventContainerSyntax(SyntaxKind kind, WithEventsEventContainerSyntax withEventsContainer, PunctuationSyntax dotToken, IdentifierNameSyntax property, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(withEventsContainer);
			_withEventsContainer = withEventsContainer;
			AdjustFlagsAndWidth(dotToken);
			_dotToken = dotToken;
			AdjustFlagsAndWidth(property);
			_property = property;
		}

		internal WithEventsPropertyEventContainerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, WithEventsEventContainerSyntax withEventsContainer, PunctuationSyntax dotToken, IdentifierNameSyntax property)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(withEventsContainer);
			_withEventsContainer = withEventsContainer;
			AdjustFlagsAndWidth(dotToken);
			_dotToken = dotToken;
			AdjustFlagsAndWidth(property);
			_property = property;
		}

		internal WithEventsPropertyEventContainerSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			WithEventsEventContainerSyntax withEventsEventContainerSyntax = (WithEventsEventContainerSyntax)reader.ReadValue();
			if (withEventsEventContainerSyntax != null)
			{
				AdjustFlagsAndWidth(withEventsEventContainerSyntax);
				_withEventsContainer = withEventsEventContainerSyntax;
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
				_property = identifierNameSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_withEventsContainer);
			writer.WriteValue(_dotToken);
			writer.WriteValue(_property);
		}

		static WithEventsPropertyEventContainerSyntax()
		{
			CreateInstance = (ObjectReader o) => new WithEventsPropertyEventContainerSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(WithEventsPropertyEventContainerSyntax), (ObjectReader r) => new WithEventsPropertyEventContainerSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsPropertyEventContainerSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _withEventsContainer, 
				1 => _dotToken, 
				2 => _property, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new WithEventsPropertyEventContainerSyntax(base.Kind, newErrors, GetAnnotations(), _withEventsContainer, _dotToken, _property);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new WithEventsPropertyEventContainerSyntax(base.Kind, GetDiagnostics(), annotations, _withEventsContainer, _dotToken, _property);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitWithEventsPropertyEventContainer(this);
		}
	}
}
