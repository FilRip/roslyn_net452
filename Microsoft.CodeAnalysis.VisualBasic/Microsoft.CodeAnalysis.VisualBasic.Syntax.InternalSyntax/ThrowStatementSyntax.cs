using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ThrowStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _throwKeyword;

		internal readonly ExpressionSyntax _expression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ThrowKeyword => _throwKeyword;

		internal ExpressionSyntax Expression => _expression;

		internal ThrowStatementSyntax(SyntaxKind kind, KeywordSyntax throwKeyword, ExpressionSyntax expression)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(throwKeyword);
			_throwKeyword = throwKeyword;
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
		}

		internal ThrowStatementSyntax(SyntaxKind kind, KeywordSyntax throwKeyword, ExpressionSyntax expression, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(throwKeyword);
			_throwKeyword = throwKeyword;
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
		}

		internal ThrowStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax throwKeyword, ExpressionSyntax expression)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(throwKeyword);
			_throwKeyword = throwKeyword;
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
		}

		internal ThrowStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_throwKeyword = keywordSyntax;
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
			writer.WriteValue(_throwKeyword);
			writer.WriteValue(_expression);
		}

		static ThrowStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ThrowStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ThrowStatementSyntax), (ObjectReader r) => new ThrowStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ThrowStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _throwKeyword, 
				1 => _expression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ThrowStatementSyntax(base.Kind, newErrors, GetAnnotations(), _throwKeyword, _expression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ThrowStatementSyntax(base.Kind, GetDiagnostics(), annotations, _throwKeyword, _expression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitThrowStatement(this);
		}
	}
}
