using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class InterpolatedStringExpressionSyntax : ExpressionSyntax
	{
		internal readonly PunctuationSyntax _dollarSignDoubleQuoteToken;

		internal readonly GreenNode _contents;

		internal readonly PunctuationSyntax _doubleQuoteToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax DollarSignDoubleQuoteToken => _dollarSignDoubleQuoteToken;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<InterpolatedStringContentSyntax> Contents => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<InterpolatedStringContentSyntax>(_contents);

		internal PunctuationSyntax DoubleQuoteToken => _doubleQuoteToken;

		internal InterpolatedStringExpressionSyntax(SyntaxKind kind, PunctuationSyntax dollarSignDoubleQuoteToken, GreenNode contents, PunctuationSyntax doubleQuoteToken)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(dollarSignDoubleQuoteToken);
			_dollarSignDoubleQuoteToken = dollarSignDoubleQuoteToken;
			if (contents != null)
			{
				AdjustFlagsAndWidth(contents);
				_contents = contents;
			}
			AdjustFlagsAndWidth(doubleQuoteToken);
			_doubleQuoteToken = doubleQuoteToken;
		}

		internal InterpolatedStringExpressionSyntax(SyntaxKind kind, PunctuationSyntax dollarSignDoubleQuoteToken, GreenNode contents, PunctuationSyntax doubleQuoteToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(dollarSignDoubleQuoteToken);
			_dollarSignDoubleQuoteToken = dollarSignDoubleQuoteToken;
			if (contents != null)
			{
				AdjustFlagsAndWidth(contents);
				_contents = contents;
			}
			AdjustFlagsAndWidth(doubleQuoteToken);
			_doubleQuoteToken = doubleQuoteToken;
		}

		internal InterpolatedStringExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax dollarSignDoubleQuoteToken, GreenNode contents, PunctuationSyntax doubleQuoteToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(dollarSignDoubleQuoteToken);
			_dollarSignDoubleQuoteToken = dollarSignDoubleQuoteToken;
			if (contents != null)
			{
				AdjustFlagsAndWidth(contents);
				_contents = contents;
			}
			AdjustFlagsAndWidth(doubleQuoteToken);
			_doubleQuoteToken = doubleQuoteToken;
		}

		internal InterpolatedStringExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_dollarSignDoubleQuoteToken = punctuationSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_contents = greenNode;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_doubleQuoteToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_dollarSignDoubleQuoteToken);
			writer.WriteValue(_contents);
			writer.WriteValue(_doubleQuoteToken);
		}

		static InterpolatedStringExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new InterpolatedStringExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(InterpolatedStringExpressionSyntax), (ObjectReader r) => new InterpolatedStringExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _dollarSignDoubleQuoteToken, 
				1 => _contents, 
				2 => _doubleQuoteToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new InterpolatedStringExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _dollarSignDoubleQuoteToken, _contents, _doubleQuoteToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new InterpolatedStringExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _dollarSignDoubleQuoteToken, _contents, _doubleQuoteToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitInterpolatedStringExpression(this);
		}
	}
}
