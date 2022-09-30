using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class InterpolationFormatClauseSyntax : VisualBasicSyntaxNode
	{
		internal readonly PunctuationSyntax _colonToken;

		internal readonly InterpolatedStringTextTokenSyntax _formatStringToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax ColonToken => _colonToken;

		internal InterpolatedStringTextTokenSyntax FormatStringToken => _formatStringToken;

		internal InterpolationFormatClauseSyntax(SyntaxKind kind, PunctuationSyntax colonToken, InterpolatedStringTextTokenSyntax formatStringToken)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(colonToken);
			_colonToken = colonToken;
			AdjustFlagsAndWidth(formatStringToken);
			_formatStringToken = formatStringToken;
		}

		internal InterpolationFormatClauseSyntax(SyntaxKind kind, PunctuationSyntax colonToken, InterpolatedStringTextTokenSyntax formatStringToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(colonToken);
			_colonToken = colonToken;
			AdjustFlagsAndWidth(formatStringToken);
			_formatStringToken = formatStringToken;
		}

		internal InterpolationFormatClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax colonToken, InterpolatedStringTextTokenSyntax formatStringToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(colonToken);
			_colonToken = colonToken;
			AdjustFlagsAndWidth(formatStringToken);
			_formatStringToken = formatStringToken;
		}

		internal InterpolationFormatClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_colonToken = punctuationSyntax;
			}
			InterpolatedStringTextTokenSyntax interpolatedStringTextTokenSyntax = (InterpolatedStringTextTokenSyntax)reader.ReadValue();
			if (interpolatedStringTextTokenSyntax != null)
			{
				AdjustFlagsAndWidth(interpolatedStringTextTokenSyntax);
				_formatStringToken = interpolatedStringTextTokenSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_colonToken);
			writer.WriteValue(_formatStringToken);
		}

		static InterpolationFormatClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new InterpolationFormatClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(InterpolationFormatClauseSyntax), (ObjectReader r) => new InterpolationFormatClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _colonToken, 
				1 => _formatStringToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new InterpolationFormatClauseSyntax(base.Kind, newErrors, GetAnnotations(), _colonToken, _formatStringToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new InterpolationFormatClauseSyntax(base.Kind, GetDiagnostics(), annotations, _colonToken, _formatStringToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitInterpolationFormatClause(this);
		}
	}
}
