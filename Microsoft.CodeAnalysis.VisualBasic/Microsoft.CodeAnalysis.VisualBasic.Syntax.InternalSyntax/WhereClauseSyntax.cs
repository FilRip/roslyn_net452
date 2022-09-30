using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class WhereClauseSyntax : QueryClauseSyntax
	{
		internal readonly KeywordSyntax _whereKeyword;

		internal readonly ExpressionSyntax _condition;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax WhereKeyword => _whereKeyword;

		internal ExpressionSyntax Condition => _condition;

		internal WhereClauseSyntax(SyntaxKind kind, KeywordSyntax whereKeyword, ExpressionSyntax condition)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(whereKeyword);
			_whereKeyword = whereKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
		}

		internal WhereClauseSyntax(SyntaxKind kind, KeywordSyntax whereKeyword, ExpressionSyntax condition, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(whereKeyword);
			_whereKeyword = whereKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
		}

		internal WhereClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax whereKeyword, ExpressionSyntax condition)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(whereKeyword);
			_whereKeyword = whereKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
		}

		internal WhereClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_whereKeyword = keywordSyntax;
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
			writer.WriteValue(_whereKeyword);
			writer.WriteValue(_condition);
		}

		static WhereClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new WhereClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(WhereClauseSyntax), (ObjectReader r) => new WhereClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WhereClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _whereKeyword, 
				1 => _condition, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new WhereClauseSyntax(base.Kind, newErrors, GetAnnotations(), _whereKeyword, _condition);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new WhereClauseSyntax(base.Kind, GetDiagnostics(), annotations, _whereKeyword, _condition);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitWhereClause(this);
		}
	}
}
