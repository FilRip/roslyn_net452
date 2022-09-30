using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ReturnStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _returnKeyword;

		internal readonly ExpressionSyntax _expression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ReturnKeyword => _returnKeyword;

		internal ExpressionSyntax Expression => _expression;

		internal ReturnStatementSyntax(SyntaxKind kind, KeywordSyntax returnKeyword, ExpressionSyntax expression)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(returnKeyword);
			_returnKeyword = returnKeyword;
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
		}

		internal ReturnStatementSyntax(SyntaxKind kind, KeywordSyntax returnKeyword, ExpressionSyntax expression, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(returnKeyword);
			_returnKeyword = returnKeyword;
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
		}

		internal ReturnStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax returnKeyword, ExpressionSyntax expression)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(returnKeyword);
			_returnKeyword = returnKeyword;
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
		}

		internal ReturnStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_returnKeyword = keywordSyntax;
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
			writer.WriteValue(_returnKeyword);
			writer.WriteValue(_expression);
		}

		static ReturnStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ReturnStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ReturnStatementSyntax), (ObjectReader r) => new ReturnStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ReturnStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _returnKeyword, 
				1 => _expression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ReturnStatementSyntax(base.Kind, newErrors, GetAnnotations(), _returnKeyword, _expression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ReturnStatementSyntax(base.Kind, GetDiagnostics(), annotations, _returnKeyword, _expression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitReturnStatement(this);
		}
	}
}
