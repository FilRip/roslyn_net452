using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class WhileStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _whileKeyword;

		internal readonly ExpressionSyntax _condition;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax WhileKeyword => _whileKeyword;

		internal ExpressionSyntax Condition => _condition;

		internal WhileStatementSyntax(SyntaxKind kind, KeywordSyntax whileKeyword, ExpressionSyntax condition)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(whileKeyword);
			_whileKeyword = whileKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
		}

		internal WhileStatementSyntax(SyntaxKind kind, KeywordSyntax whileKeyword, ExpressionSyntax condition, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(whileKeyword);
			_whileKeyword = whileKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
		}

		internal WhileStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax whileKeyword, ExpressionSyntax condition)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(whileKeyword);
			_whileKeyword = whileKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
		}

		internal WhileStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_whileKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_condition = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_whileKeyword);
			writer.WriteValue(_condition);
		}

		static WhileStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new WhileStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(WhileStatementSyntax), (ObjectReader r) => new WhileStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _whileKeyword, 
				1 => _condition, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new WhileStatementSyntax(base.Kind, newErrors, GetAnnotations(), _whileKeyword, _condition);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new WhileStatementSyntax(base.Kind, GetDiagnostics(), annotations, _whileKeyword, _condition);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitWhileStatement(this);
		}
	}
}
