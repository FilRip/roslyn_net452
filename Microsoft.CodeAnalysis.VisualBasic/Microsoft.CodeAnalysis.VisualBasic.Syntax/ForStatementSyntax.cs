using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ForStatementSyntax : ForOrForEachStatementSyntax
	{
		internal ExpressionSyntax _fromValue;

		internal ExpressionSyntax _toValue;

		internal ForStepClauseSyntax _stepClause;

		public new SyntaxToken ForKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax)base.Green)._forKeyword, base.Position, 0);

		public new VisualBasicSyntaxNode ControlVariable => GetRed(ref _controlVariable, 1);

		public SyntaxToken EqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax)base.Green)._equalsToken, GetChildPosition(2), GetChildIndex(2));

		public ExpressionSyntax FromValue => GetRed(ref _fromValue, 3);

		public SyntaxToken ToKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax)base.Green)._toKeyword, GetChildPosition(4), GetChildIndex(4));

		public ExpressionSyntax ToValue => GetRed(ref _toValue, 5);

		public ForStepClauseSyntax StepClause => GetRed(ref _stepClause, 6);

		internal ForStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ForStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax forKeyword, VisualBasicSyntaxNode controlVariable, PunctuationSyntax equalsToken, ExpressionSyntax fromValue, KeywordSyntax toKeyword, ExpressionSyntax toValue, ForStepClauseSyntax stepClause)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax(kind, errors, annotations, forKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)controlVariable.Green, equalsToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)fromValue.Green, toKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)toValue.Green, (stepClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax)stepClause.Green) : null), null, 0)
		{
		}

		internal override SyntaxToken GetForKeywordCore()
		{
			return ForKeyword;
		}

		internal override ForOrForEachStatementSyntax WithForKeywordCore(SyntaxToken forKeyword)
		{
			return WithForKeyword(forKeyword);
		}

		public new ForStatementSyntax WithForKeyword(SyntaxToken forKeyword)
		{
			return Update(forKeyword, ControlVariable, EqualsToken, FromValue, ToKeyword, ToValue, StepClause);
		}

		internal override VisualBasicSyntaxNode GetControlVariableCore()
		{
			return ControlVariable;
		}

		internal override ForOrForEachStatementSyntax WithControlVariableCore(VisualBasicSyntaxNode controlVariable)
		{
			return WithControlVariable(controlVariable);
		}

		public new ForStatementSyntax WithControlVariable(VisualBasicSyntaxNode controlVariable)
		{
			return Update(ForKeyword, controlVariable, EqualsToken, FromValue, ToKeyword, ToValue, StepClause);
		}

		public ForStatementSyntax WithEqualsToken(SyntaxToken equalsToken)
		{
			return Update(ForKeyword, ControlVariable, equalsToken, FromValue, ToKeyword, ToValue, StepClause);
		}

		public ForStatementSyntax WithFromValue(ExpressionSyntax fromValue)
		{
			return Update(ForKeyword, ControlVariable, EqualsToken, fromValue, ToKeyword, ToValue, StepClause);
		}

		public ForStatementSyntax WithToKeyword(SyntaxToken toKeyword)
		{
			return Update(ForKeyword, ControlVariable, EqualsToken, FromValue, toKeyword, ToValue, StepClause);
		}

		public ForStatementSyntax WithToValue(ExpressionSyntax toValue)
		{
			return Update(ForKeyword, ControlVariable, EqualsToken, FromValue, ToKeyword, toValue, StepClause);
		}

		public ForStatementSyntax WithStepClause(ForStepClauseSyntax stepClause)
		{
			return Update(ForKeyword, ControlVariable, EqualsToken, FromValue, ToKeyword, ToValue, stepClause);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _controlVariable, 
				3 => _fromValue, 
				5 => _toValue, 
				6 => _stepClause, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => ControlVariable, 
				3 => FromValue, 
				5 => ToValue, 
				6 => StepClause, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitForStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitForStatement(this);
		}

		public ForStatementSyntax Update(SyntaxToken forKeyword, VisualBasicSyntaxNode controlVariable, SyntaxToken equalsToken, ExpressionSyntax fromValue, SyntaxToken toKeyword, ExpressionSyntax toValue, ForStepClauseSyntax stepClause)
		{
			if (forKeyword != ForKeyword || controlVariable != ControlVariable || equalsToken != EqualsToken || fromValue != FromValue || toKeyword != ToKeyword || toValue != ToValue || stepClause != StepClause)
			{
				ForStatementSyntax forStatementSyntax = SyntaxFactory.ForStatement(forKeyword, controlVariable, equalsToken, fromValue, toKeyword, toValue, stepClause);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(forStatementSyntax, annotations);
				}
				return forStatementSyntax;
			}
			return this;
		}
	}
}
