using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class EventBlockSyntax : DeclarationStatementSyntax
	{
		internal EventStatementSyntax _eventStatement;

		internal SyntaxNode _accessors;

		internal EndBlockStatementSyntax _endEventStatement;

		public EventStatementSyntax EventStatement => GetRedAtZero(ref _eventStatement);

		public SyntaxList<AccessorBlockSyntax> Accessors
		{
			get
			{
				SyntaxNode red = GetRed(ref _accessors, 1);
				return new SyntaxList<AccessorBlockSyntax>(red);
			}
		}

		public EndBlockStatementSyntax EndEventStatement => GetRed(ref _endEventStatement, 2);

		internal EventBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal EventBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, EventStatementSyntax eventStatement, SyntaxNode accessors, EndBlockStatementSyntax endEventStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax)eventStatement.Green, accessors?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endEventStatement.Green), null, 0)
		{
		}

		public EventBlockSyntax WithEventStatement(EventStatementSyntax eventStatement)
		{
			return Update(eventStatement, Accessors, EndEventStatement);
		}

		public EventBlockSyntax WithAccessors(SyntaxList<AccessorBlockSyntax> accessors)
		{
			return Update(EventStatement, accessors, EndEventStatement);
		}

		public EventBlockSyntax AddAccessors(params AccessorBlockSyntax[] items)
		{
			return WithAccessors(Accessors.AddRange(items));
		}

		public EventBlockSyntax WithEndEventStatement(EndBlockStatementSyntax endEventStatement)
		{
			return Update(EventStatement, Accessors, endEventStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _eventStatement, 
				1 => _accessors, 
				2 => _endEventStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => EventStatement, 
				1 => GetRed(ref _accessors, 1), 
				2 => EndEventStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitEventBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitEventBlock(this);
		}

		public EventBlockSyntax Update(EventStatementSyntax eventStatement, SyntaxList<AccessorBlockSyntax> accessors, EndBlockStatementSyntax endEventStatement)
		{
			if (eventStatement != EventStatement || accessors != Accessors || endEventStatement != EndEventStatement)
			{
				EventBlockSyntax eventBlockSyntax = SyntaxFactory.EventBlock(eventStatement, accessors, endEventStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(eventBlockSyntax, annotations);
				}
				return eventBlockSyntax;
			}
			return this;
		}
	}
}
