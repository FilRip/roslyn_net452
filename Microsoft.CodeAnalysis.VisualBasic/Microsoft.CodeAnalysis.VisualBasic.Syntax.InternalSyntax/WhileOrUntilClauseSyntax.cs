using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class WhileOrUntilClauseSyntax : VisualBasicSyntaxNode
	{
		internal readonly KeywordSyntax _whileOrUntilKeyword;

		internal readonly ExpressionSyntax _condition;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax WhileOrUntilKeyword => _whileOrUntilKeyword;

		internal ExpressionSyntax Condition => _condition;

		internal WhileOrUntilClauseSyntax(SyntaxKind kind, KeywordSyntax whileOrUntilKeyword, ExpressionSyntax condition)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(whileOrUntilKeyword);
			_whileOrUntilKeyword = whileOrUntilKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
		}

		internal WhileOrUntilClauseSyntax(SyntaxKind kind, KeywordSyntax whileOrUntilKeyword, ExpressionSyntax condition, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(whileOrUntilKeyword);
			_whileOrUntilKeyword = whileOrUntilKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
		}

		internal WhileOrUntilClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax whileOrUntilKeyword, ExpressionSyntax condition)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(whileOrUntilKeyword);
			_whileOrUntilKeyword = whileOrUntilKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
		}

		internal WhileOrUntilClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_whileOrUntilKeyword = keywordSyntax;
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
			writer.WriteValue(_whileOrUntilKeyword);
			writer.WriteValue(_condition);
		}

		static WhileOrUntilClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new WhileOrUntilClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(WhileOrUntilClauseSyntax), (ObjectReader r) => new WhileOrUntilClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _whileOrUntilKeyword, 
				1 => _condition, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new WhileOrUntilClauseSyntax(base.Kind, newErrors, GetAnnotations(), _whileOrUntilKeyword, _condition);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new WhileOrUntilClauseSyntax(base.Kind, GetDiagnostics(), annotations, _whileOrUntilKeyword, _condition);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitWhileOrUntilClause(this);
		}
	}
}
