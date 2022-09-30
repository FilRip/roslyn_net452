using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class PrintStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly PunctuationSyntax _questionToken;

		internal readonly ExpressionSyntax _expression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax QuestionToken => _questionToken;

		internal ExpressionSyntax Expression => _expression;

		internal PrintStatementSyntax(SyntaxKind kind, PunctuationSyntax questionToken, ExpressionSyntax expression)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(questionToken);
			_questionToken = questionToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal PrintStatementSyntax(SyntaxKind kind, PunctuationSyntax questionToken, ExpressionSyntax expression, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(questionToken);
			_questionToken = questionToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal PrintStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax questionToken, ExpressionSyntax expression)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(questionToken);
			_questionToken = questionToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal PrintStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_questionToken = punctuationSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_expression = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_questionToken);
			writer.WriteValue(_expression);
		}

		static PrintStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new PrintStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(PrintStatementSyntax), (ObjectReader r) => new PrintStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.PrintStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _questionToken, 
				1 => _expression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new PrintStatementSyntax(base.Kind, newErrors, GetAnnotations(), _questionToken, _expression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new PrintStatementSyntax(base.Kind, GetDiagnostics(), annotations, _questionToken, _expression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitPrintStatement(this);
		}
	}
}
