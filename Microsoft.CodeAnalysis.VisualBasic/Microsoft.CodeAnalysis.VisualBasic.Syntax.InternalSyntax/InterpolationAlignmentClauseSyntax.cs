using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class InterpolationAlignmentClauseSyntax : VisualBasicSyntaxNode
	{
		internal readonly PunctuationSyntax _commaToken;

		internal readonly ExpressionSyntax _value;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax CommaToken => _commaToken;

		internal ExpressionSyntax Value => _value;

		internal InterpolationAlignmentClauseSyntax(SyntaxKind kind, PunctuationSyntax commaToken, ExpressionSyntax value)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(commaToken);
			_commaToken = commaToken;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal InterpolationAlignmentClauseSyntax(SyntaxKind kind, PunctuationSyntax commaToken, ExpressionSyntax value, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(commaToken);
			_commaToken = commaToken;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal InterpolationAlignmentClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax commaToken, ExpressionSyntax value)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(commaToken);
			_commaToken = commaToken;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal InterpolationAlignmentClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_commaToken = punctuationSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_value = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_commaToken);
			writer.WriteValue(_value);
		}

		static InterpolationAlignmentClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new InterpolationAlignmentClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(InterpolationAlignmentClauseSyntax), (ObjectReader r) => new InterpolationAlignmentClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationAlignmentClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _commaToken, 
				1 => _value, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new InterpolationAlignmentClauseSyntax(base.Kind, newErrors, GetAnnotations(), _commaToken, _value);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new InterpolationAlignmentClauseSyntax(base.Kind, GetDiagnostics(), annotations, _commaToken, _value);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitInterpolationAlignmentClause(this);
		}
	}
}
