using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class WithStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _withKeyword;

		internal readonly ExpressionSyntax _expression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax WithKeyword => _withKeyword;

		internal ExpressionSyntax Expression => _expression;

		internal WithStatementSyntax(SyntaxKind kind, KeywordSyntax withKeyword, ExpressionSyntax expression)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(withKeyword);
			_withKeyword = withKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal WithStatementSyntax(SyntaxKind kind, KeywordSyntax withKeyword, ExpressionSyntax expression, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(withKeyword);
			_withKeyword = withKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal WithStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax withKeyword, ExpressionSyntax expression)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(withKeyword);
			_withKeyword = withKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal WithStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_withKeyword = keywordSyntax;
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
			writer.WriteValue(_withKeyword);
			writer.WriteValue(_expression);
		}

		static WithStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new WithStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(WithStatementSyntax), (ObjectReader r) => new WithStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _withKeyword, 
				1 => _expression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new WithStatementSyntax(base.Kind, newErrors, GetAnnotations(), _withKeyword, _expression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new WithStatementSyntax(base.Kind, GetDiagnostics(), annotations, _withKeyword, _expression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitWithStatement(this);
		}
	}
}
