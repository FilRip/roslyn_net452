using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class YieldStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _yieldKeyword;

		internal readonly ExpressionSyntax _expression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax YieldKeyword => _yieldKeyword;

		internal ExpressionSyntax Expression => _expression;

		internal YieldStatementSyntax(SyntaxKind kind, KeywordSyntax yieldKeyword, ExpressionSyntax expression)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(yieldKeyword);
			_yieldKeyword = yieldKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal YieldStatementSyntax(SyntaxKind kind, KeywordSyntax yieldKeyword, ExpressionSyntax expression, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(yieldKeyword);
			_yieldKeyword = yieldKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal YieldStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax yieldKeyword, ExpressionSyntax expression)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(yieldKeyword);
			_yieldKeyword = yieldKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal YieldStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_yieldKeyword = keywordSyntax;
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
			writer.WriteValue(_yieldKeyword);
			writer.WriteValue(_expression);
		}

		static YieldStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new YieldStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(YieldStatementSyntax), (ObjectReader r) => new YieldStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.YieldStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _yieldKeyword, 
				1 => _expression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new YieldStatementSyntax(base.Kind, newErrors, GetAnnotations(), _yieldKeyword, _expression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new YieldStatementSyntax(base.Kind, GetDiagnostics(), annotations, _yieldKeyword, _expression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitYieldStatement(this);
		}
	}
}
