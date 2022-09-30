using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class InterpolationSyntax : InterpolatedStringContentSyntax
	{
		internal readonly PunctuationSyntax _openBraceToken;

		internal readonly ExpressionSyntax _expression;

		internal readonly InterpolationAlignmentClauseSyntax _alignmentClause;

		internal readonly InterpolationFormatClauseSyntax _formatClause;

		internal readonly PunctuationSyntax _closeBraceToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax OpenBraceToken => _openBraceToken;

		internal ExpressionSyntax Expression => _expression;

		internal InterpolationAlignmentClauseSyntax AlignmentClause => _alignmentClause;

		internal InterpolationFormatClauseSyntax FormatClause => _formatClause;

		internal PunctuationSyntax CloseBraceToken => _closeBraceToken;

		internal InterpolationSyntax(SyntaxKind kind, PunctuationSyntax openBraceToken, ExpressionSyntax expression, InterpolationAlignmentClauseSyntax alignmentClause, InterpolationFormatClauseSyntax formatClause, PunctuationSyntax closeBraceToken)
			: base(kind)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(openBraceToken);
			_openBraceToken = openBraceToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			if (alignmentClause != null)
			{
				AdjustFlagsAndWidth(alignmentClause);
				_alignmentClause = alignmentClause;
			}
			if (formatClause != null)
			{
				AdjustFlagsAndWidth(formatClause);
				_formatClause = formatClause;
			}
			AdjustFlagsAndWidth(closeBraceToken);
			_closeBraceToken = closeBraceToken;
		}

		internal InterpolationSyntax(SyntaxKind kind, PunctuationSyntax openBraceToken, ExpressionSyntax expression, InterpolationAlignmentClauseSyntax alignmentClause, InterpolationFormatClauseSyntax formatClause, PunctuationSyntax closeBraceToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(openBraceToken);
			_openBraceToken = openBraceToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			if (alignmentClause != null)
			{
				AdjustFlagsAndWidth(alignmentClause);
				_alignmentClause = alignmentClause;
			}
			if (formatClause != null)
			{
				AdjustFlagsAndWidth(formatClause);
				_formatClause = formatClause;
			}
			AdjustFlagsAndWidth(closeBraceToken);
			_closeBraceToken = closeBraceToken;
		}

		internal InterpolationSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openBraceToken, ExpressionSyntax expression, InterpolationAlignmentClauseSyntax alignmentClause, InterpolationFormatClauseSyntax formatClause, PunctuationSyntax closeBraceToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(openBraceToken);
			_openBraceToken = openBraceToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			if (alignmentClause != null)
			{
				AdjustFlagsAndWidth(alignmentClause);
				_alignmentClause = alignmentClause;
			}
			if (formatClause != null)
			{
				AdjustFlagsAndWidth(formatClause);
				_formatClause = formatClause;
			}
			AdjustFlagsAndWidth(closeBraceToken);
			_closeBraceToken = closeBraceToken;
		}

		internal InterpolationSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_openBraceToken = punctuationSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_expression = expressionSyntax;
			}
			InterpolationAlignmentClauseSyntax interpolationAlignmentClauseSyntax = (InterpolationAlignmentClauseSyntax)reader.ReadValue();
			if (interpolationAlignmentClauseSyntax != null)
			{
				AdjustFlagsAndWidth(interpolationAlignmentClauseSyntax);
				_alignmentClause = interpolationAlignmentClauseSyntax;
			}
			InterpolationFormatClauseSyntax interpolationFormatClauseSyntax = (InterpolationFormatClauseSyntax)reader.ReadValue();
			if (interpolationFormatClauseSyntax != null)
			{
				AdjustFlagsAndWidth(interpolationFormatClauseSyntax);
				_formatClause = interpolationFormatClauseSyntax;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_closeBraceToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_openBraceToken);
			writer.WriteValue(_expression);
			writer.WriteValue(_alignmentClause);
			writer.WriteValue(_formatClause);
			writer.WriteValue(_closeBraceToken);
		}

		static InterpolationSyntax()
		{
			CreateInstance = (ObjectReader o) => new InterpolationSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(InterpolationSyntax), (ObjectReader r) => new InterpolationSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _openBraceToken, 
				1 => _expression, 
				2 => _alignmentClause, 
				3 => _formatClause, 
				4 => _closeBraceToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new InterpolationSyntax(base.Kind, newErrors, GetAnnotations(), _openBraceToken, _expression, _alignmentClause, _formatClause, _closeBraceToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new InterpolationSyntax(base.Kind, GetDiagnostics(), annotations, _openBraceToken, _expression, _alignmentClause, _formatClause, _closeBraceToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitInterpolation(this);
		}
	}
}
