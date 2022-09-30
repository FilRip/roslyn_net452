using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ForStepClauseSyntax : VisualBasicSyntaxNode
	{
		internal readonly KeywordSyntax _stepKeyword;

		internal readonly ExpressionSyntax _stepValue;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax StepKeyword => _stepKeyword;

		internal ExpressionSyntax StepValue => _stepValue;

		internal ForStepClauseSyntax(SyntaxKind kind, KeywordSyntax stepKeyword, ExpressionSyntax stepValue)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(stepKeyword);
			_stepKeyword = stepKeyword;
			AdjustFlagsAndWidth(stepValue);
			_stepValue = stepValue;
		}

		internal ForStepClauseSyntax(SyntaxKind kind, KeywordSyntax stepKeyword, ExpressionSyntax stepValue, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(stepKeyword);
			_stepKeyword = stepKeyword;
			AdjustFlagsAndWidth(stepValue);
			_stepValue = stepValue;
		}

		internal ForStepClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax stepKeyword, ExpressionSyntax stepValue)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(stepKeyword);
			_stepKeyword = stepKeyword;
			AdjustFlagsAndWidth(stepValue);
			_stepValue = stepValue;
		}

		internal ForStepClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_stepKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_stepValue = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_stepKeyword);
			writer.WriteValue(_stepValue);
		}

		static ForStepClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new ForStepClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ForStepClauseSyntax), (ObjectReader r) => new ForStepClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _stepKeyword, 
				1 => _stepValue, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ForStepClauseSyntax(base.Kind, newErrors, GetAnnotations(), _stepKeyword, _stepValue);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ForStepClauseSyntax(base.Kind, GetDiagnostics(), annotations, _stepKeyword, _stepValue);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitForStepClause(this);
		}
	}
}
