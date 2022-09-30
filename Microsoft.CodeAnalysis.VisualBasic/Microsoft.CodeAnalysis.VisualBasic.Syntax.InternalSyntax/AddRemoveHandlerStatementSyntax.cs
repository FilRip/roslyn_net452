using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class AddRemoveHandlerStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _addHandlerOrRemoveHandlerKeyword;

		internal readonly ExpressionSyntax _eventExpression;

		internal readonly PunctuationSyntax _commaToken;

		internal readonly ExpressionSyntax _delegateExpression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax AddHandlerOrRemoveHandlerKeyword => _addHandlerOrRemoveHandlerKeyword;

		internal ExpressionSyntax EventExpression => _eventExpression;

		internal PunctuationSyntax CommaToken => _commaToken;

		internal ExpressionSyntax DelegateExpression => _delegateExpression;

		internal AddRemoveHandlerStatementSyntax(SyntaxKind kind, KeywordSyntax addHandlerOrRemoveHandlerKeyword, ExpressionSyntax eventExpression, PunctuationSyntax commaToken, ExpressionSyntax delegateExpression)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(addHandlerOrRemoveHandlerKeyword);
			_addHandlerOrRemoveHandlerKeyword = addHandlerOrRemoveHandlerKeyword;
			AdjustFlagsAndWidth(eventExpression);
			_eventExpression = eventExpression;
			AdjustFlagsAndWidth(commaToken);
			_commaToken = commaToken;
			AdjustFlagsAndWidth(delegateExpression);
			_delegateExpression = delegateExpression;
		}

		internal AddRemoveHandlerStatementSyntax(SyntaxKind kind, KeywordSyntax addHandlerOrRemoveHandlerKeyword, ExpressionSyntax eventExpression, PunctuationSyntax commaToken, ExpressionSyntax delegateExpression, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(addHandlerOrRemoveHandlerKeyword);
			_addHandlerOrRemoveHandlerKeyword = addHandlerOrRemoveHandlerKeyword;
			AdjustFlagsAndWidth(eventExpression);
			_eventExpression = eventExpression;
			AdjustFlagsAndWidth(commaToken);
			_commaToken = commaToken;
			AdjustFlagsAndWidth(delegateExpression);
			_delegateExpression = delegateExpression;
		}

		internal AddRemoveHandlerStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax addHandlerOrRemoveHandlerKeyword, ExpressionSyntax eventExpression, PunctuationSyntax commaToken, ExpressionSyntax delegateExpression)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(addHandlerOrRemoveHandlerKeyword);
			_addHandlerOrRemoveHandlerKeyword = addHandlerOrRemoveHandlerKeyword;
			AdjustFlagsAndWidth(eventExpression);
			_eventExpression = eventExpression;
			AdjustFlagsAndWidth(commaToken);
			_commaToken = commaToken;
			AdjustFlagsAndWidth(delegateExpression);
			_delegateExpression = delegateExpression;
		}

		internal AddRemoveHandlerStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_addHandlerOrRemoveHandlerKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_eventExpression = expressionSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_commaToken = punctuationSyntax;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax2 != null)
			{
				AdjustFlagsAndWidth(expressionSyntax2);
				_delegateExpression = expressionSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_addHandlerOrRemoveHandlerKeyword);
			writer.WriteValue(_eventExpression);
			writer.WriteValue(_commaToken);
			writer.WriteValue(_delegateExpression);
		}

		static AddRemoveHandlerStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new AddRemoveHandlerStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(AddRemoveHandlerStatementSyntax), (ObjectReader r) => new AddRemoveHandlerStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AddRemoveHandlerStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _addHandlerOrRemoveHandlerKeyword, 
				1 => _eventExpression, 
				2 => _commaToken, 
				3 => _delegateExpression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new AddRemoveHandlerStatementSyntax(base.Kind, newErrors, GetAnnotations(), _addHandlerOrRemoveHandlerKeyword, _eventExpression, _commaToken, _delegateExpression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new AddRemoveHandlerStatementSyntax(base.Kind, GetDiagnostics(), annotations, _addHandlerOrRemoveHandlerKeyword, _eventExpression, _commaToken, _delegateExpression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitAddRemoveHandlerStatement(this);
		}
	}
}
