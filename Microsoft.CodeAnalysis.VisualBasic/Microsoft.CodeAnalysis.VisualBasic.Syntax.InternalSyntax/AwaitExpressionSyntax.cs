using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class AwaitExpressionSyntax : ExpressionSyntax
	{
		internal readonly KeywordSyntax _awaitKeyword;

		internal readonly ExpressionSyntax _expression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax AwaitKeyword => _awaitKeyword;

		internal ExpressionSyntax Expression => _expression;

		internal AwaitExpressionSyntax(SyntaxKind kind, KeywordSyntax awaitKeyword, ExpressionSyntax expression)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(awaitKeyword);
			_awaitKeyword = awaitKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal AwaitExpressionSyntax(SyntaxKind kind, KeywordSyntax awaitKeyword, ExpressionSyntax expression, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(awaitKeyword);
			_awaitKeyword = awaitKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal AwaitExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax awaitKeyword, ExpressionSyntax expression)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(awaitKeyword);
			_awaitKeyword = awaitKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal AwaitExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_awaitKeyword = keywordSyntax;
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
			writer.WriteValue(_awaitKeyword);
			writer.WriteValue(_expression);
		}

		static AwaitExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new AwaitExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(AwaitExpressionSyntax), (ObjectReader r) => new AwaitExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AwaitExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _awaitKeyword, 
				1 => _expression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new AwaitExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _awaitKeyword, _expression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new AwaitExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _awaitKeyword, _expression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitAwaitExpression(this);
		}
	}
}
