using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class EventBlockSyntax : DeclarationStatementSyntax
	{
		internal readonly EventStatementSyntax _eventStatement;

		internal readonly GreenNode _accessors;

		internal readonly EndBlockStatementSyntax _endEventStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal EventStatementSyntax EventStatement => _eventStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AccessorBlockSyntax> Accessors => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AccessorBlockSyntax>(_accessors);

		internal EndBlockStatementSyntax EndEventStatement => _endEventStatement;

		internal EventBlockSyntax(SyntaxKind kind, EventStatementSyntax eventStatement, GreenNode accessors, EndBlockStatementSyntax endEventStatement)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(eventStatement);
			_eventStatement = eventStatement;
			if (accessors != null)
			{
				AdjustFlagsAndWidth(accessors);
				_accessors = accessors;
			}
			AdjustFlagsAndWidth(endEventStatement);
			_endEventStatement = endEventStatement;
		}

		internal EventBlockSyntax(SyntaxKind kind, EventStatementSyntax eventStatement, GreenNode accessors, EndBlockStatementSyntax endEventStatement, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(eventStatement);
			_eventStatement = eventStatement;
			if (accessors != null)
			{
				AdjustFlagsAndWidth(accessors);
				_accessors = accessors;
			}
			AdjustFlagsAndWidth(endEventStatement);
			_endEventStatement = endEventStatement;
		}

		internal EventBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, EventStatementSyntax eventStatement, GreenNode accessors, EndBlockStatementSyntax endEventStatement)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(eventStatement);
			_eventStatement = eventStatement;
			if (accessors != null)
			{
				AdjustFlagsAndWidth(accessors);
				_accessors = accessors;
			}
			AdjustFlagsAndWidth(endEventStatement);
			_endEventStatement = endEventStatement;
		}

		internal EventBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			EventStatementSyntax eventStatementSyntax = (EventStatementSyntax)reader.ReadValue();
			if (eventStatementSyntax != null)
			{
				AdjustFlagsAndWidth(eventStatementSyntax);
				_eventStatement = eventStatementSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_accessors = greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)reader.ReadValue();
			if (endBlockStatementSyntax != null)
			{
				AdjustFlagsAndWidth(endBlockStatementSyntax);
				_endEventStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_eventStatement);
			writer.WriteValue(_accessors);
			writer.WriteValue(_endEventStatement);
		}

		static EventBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new EventBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(EventBlockSyntax), (ObjectReader r) => new EventBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _eventStatement, 
				1 => _accessors, 
				2 => _endEventStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new EventBlockSyntax(base.Kind, newErrors, GetAnnotations(), _eventStatement, _accessors, _endEventStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new EventBlockSyntax(base.Kind, GetDiagnostics(), annotations, _eventStatement, _accessors, _endEventStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitEventBlock(this);
		}
	}
}
