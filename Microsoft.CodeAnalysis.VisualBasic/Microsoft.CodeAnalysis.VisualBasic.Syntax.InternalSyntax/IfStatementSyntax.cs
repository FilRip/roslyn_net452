using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class IfStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _ifKeyword;

		internal readonly ExpressionSyntax _condition;

		internal readonly KeywordSyntax _thenKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax IfKeyword => _ifKeyword;

		internal ExpressionSyntax Condition => _condition;

		internal KeywordSyntax ThenKeyword => _thenKeyword;

		internal IfStatementSyntax(SyntaxKind kind, KeywordSyntax ifKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(ifKeyword);
			_ifKeyword = ifKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
			if (thenKeyword != null)
			{
				AdjustFlagsAndWidth(thenKeyword);
				_thenKeyword = thenKeyword;
			}
		}

		internal IfStatementSyntax(SyntaxKind kind, KeywordSyntax ifKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(ifKeyword);
			_ifKeyword = ifKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
			if (thenKeyword != null)
			{
				AdjustFlagsAndWidth(thenKeyword);
				_thenKeyword = thenKeyword;
			}
		}

		internal IfStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax ifKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(ifKeyword);
			_ifKeyword = ifKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
			if (thenKeyword != null)
			{
				AdjustFlagsAndWidth(thenKeyword);
				_thenKeyword = thenKeyword;
			}
		}

		internal IfStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_ifKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_condition = expressionSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_thenKeyword = keywordSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_ifKeyword);
			writer.WriteValue(_condition);
			writer.WriteValue(_thenKeyword);
		}

		static IfStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new IfStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(IfStatementSyntax), (ObjectReader r) => new IfStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _ifKeyword, 
				1 => _condition, 
				2 => _thenKeyword, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new IfStatementSyntax(base.Kind, newErrors, GetAnnotations(), _ifKeyword, _condition, _thenKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new IfStatementSyntax(base.Kind, GetDiagnostics(), annotations, _ifKeyword, _condition, _thenKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitIfStatement(this);
		}
	}
}
