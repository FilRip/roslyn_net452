using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class InterpolatedStringTextSyntax : InterpolatedStringContentSyntax
	{
		internal readonly InterpolatedStringTextTokenSyntax _textToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal InterpolatedStringTextTokenSyntax TextToken => _textToken;

		internal InterpolatedStringTextSyntax(SyntaxKind kind, InterpolatedStringTextTokenSyntax textToken)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(textToken);
			_textToken = textToken;
		}

		internal InterpolatedStringTextSyntax(SyntaxKind kind, InterpolatedStringTextTokenSyntax textToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(textToken);
			_textToken = textToken;
		}

		internal InterpolatedStringTextSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, InterpolatedStringTextTokenSyntax textToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(textToken);
			_textToken = textToken;
		}

		internal InterpolatedStringTextSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			InterpolatedStringTextTokenSyntax interpolatedStringTextTokenSyntax = (InterpolatedStringTextTokenSyntax)reader.ReadValue();
			if (interpolatedStringTextTokenSyntax != null)
			{
				AdjustFlagsAndWidth(interpolatedStringTextTokenSyntax);
				_textToken = interpolatedStringTextTokenSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_textToken);
		}

		static InterpolatedStringTextSyntax()
		{
			CreateInstance = (ObjectReader o) => new InterpolatedStringTextSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(InterpolatedStringTextSyntax), (ObjectReader r) => new InterpolatedStringTextSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringTextSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _textToken;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new InterpolatedStringTextSyntax(base.Kind, newErrors, GetAnnotations(), _textToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new InterpolatedStringTextSyntax(base.Kind, GetDiagnostics(), annotations, _textToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitInterpolatedStringText(this);
		}
	}
}
