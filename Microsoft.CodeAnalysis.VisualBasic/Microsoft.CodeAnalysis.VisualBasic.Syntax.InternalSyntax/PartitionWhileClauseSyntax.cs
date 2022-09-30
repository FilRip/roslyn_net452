using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class PartitionWhileClauseSyntax : QueryClauseSyntax
	{
		internal readonly KeywordSyntax _skipOrTakeKeyword;

		internal readonly KeywordSyntax _whileKeyword;

		internal readonly ExpressionSyntax _condition;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax SkipOrTakeKeyword => _skipOrTakeKeyword;

		internal KeywordSyntax WhileKeyword => _whileKeyword;

		internal ExpressionSyntax Condition => _condition;

		internal PartitionWhileClauseSyntax(SyntaxKind kind, KeywordSyntax skipOrTakeKeyword, KeywordSyntax whileKeyword, ExpressionSyntax condition)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(skipOrTakeKeyword);
			_skipOrTakeKeyword = skipOrTakeKeyword;
			AdjustFlagsAndWidth(whileKeyword);
			_whileKeyword = whileKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
		}

		internal PartitionWhileClauseSyntax(SyntaxKind kind, KeywordSyntax skipOrTakeKeyword, KeywordSyntax whileKeyword, ExpressionSyntax condition, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(skipOrTakeKeyword);
			_skipOrTakeKeyword = skipOrTakeKeyword;
			AdjustFlagsAndWidth(whileKeyword);
			_whileKeyword = whileKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
		}

		internal PartitionWhileClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax skipOrTakeKeyword, KeywordSyntax whileKeyword, ExpressionSyntax condition)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(skipOrTakeKeyword);
			_skipOrTakeKeyword = skipOrTakeKeyword;
			AdjustFlagsAndWidth(whileKeyword);
			_whileKeyword = whileKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
		}

		internal PartitionWhileClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_skipOrTakeKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_whileKeyword = keywordSyntax2;
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
			writer.WriteValue(_skipOrTakeKeyword);
			writer.WriteValue(_whileKeyword);
			writer.WriteValue(_condition);
		}

		static PartitionWhileClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new PartitionWhileClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(PartitionWhileClauseSyntax), (ObjectReader r) => new PartitionWhileClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _skipOrTakeKeyword, 
				1 => _whileKeyword, 
				2 => _condition, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new PartitionWhileClauseSyntax(base.Kind, newErrors, GetAnnotations(), _skipOrTakeKeyword, _whileKeyword, _condition);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new PartitionWhileClauseSyntax(base.Kind, GetDiagnostics(), annotations, _skipOrTakeKeyword, _whileKeyword, _condition);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitPartitionWhileClause(this);
		}
	}
}
