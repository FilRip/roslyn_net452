using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class JoinConditionSyntax : VisualBasicSyntaxNode
	{
		internal readonly ExpressionSyntax _left;

		internal readonly KeywordSyntax _equalsKeyword;

		internal readonly ExpressionSyntax _right;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ExpressionSyntax Left => _left;

		internal KeywordSyntax EqualsKeyword => _equalsKeyword;

		internal ExpressionSyntax Right => _right;

		internal JoinConditionSyntax(SyntaxKind kind, ExpressionSyntax left, KeywordSyntax equalsKeyword, ExpressionSyntax right)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(left);
			_left = left;
			AdjustFlagsAndWidth(equalsKeyword);
			_equalsKeyword = equalsKeyword;
			AdjustFlagsAndWidth(right);
			_right = right;
		}

		internal JoinConditionSyntax(SyntaxKind kind, ExpressionSyntax left, KeywordSyntax equalsKeyword, ExpressionSyntax right, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(left);
			_left = left;
			AdjustFlagsAndWidth(equalsKeyword);
			_equalsKeyword = equalsKeyword;
			AdjustFlagsAndWidth(right);
			_right = right;
		}

		internal JoinConditionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax left, KeywordSyntax equalsKeyword, ExpressionSyntax right)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(left);
			_left = left;
			AdjustFlagsAndWidth(equalsKeyword);
			_equalsKeyword = equalsKeyword;
			AdjustFlagsAndWidth(right);
			_right = right;
		}

		internal JoinConditionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_left = expressionSyntax;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_equalsKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax2 != null)
			{
				AdjustFlagsAndWidth(expressionSyntax2);
				_right = expressionSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_left);
			writer.WriteValue(_equalsKeyword);
			writer.WriteValue(_right);
		}

		static JoinConditionSyntax()
		{
			CreateInstance = (ObjectReader o) => new JoinConditionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(JoinConditionSyntax), (ObjectReader r) => new JoinConditionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _left, 
				1 => _equalsKeyword, 
				2 => _right, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new JoinConditionSyntax(base.Kind, newErrors, GetAnnotations(), _left, _equalsKeyword, _right);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new JoinConditionSyntax(base.Kind, GetDiagnostics(), annotations, _left, _equalsKeyword, _right);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitJoinCondition(this);
		}
	}
}
