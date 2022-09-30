using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class LabelSyntax : ExpressionSyntax
	{
		internal readonly SyntaxToken _labelToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal SyntaxToken LabelToken => _labelToken;

		internal LabelSyntax(SyntaxKind kind, SyntaxToken labelToken)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(labelToken);
			_labelToken = labelToken;
		}

		internal LabelSyntax(SyntaxKind kind, SyntaxToken labelToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(labelToken);
			_labelToken = labelToken;
		}

		internal LabelSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxToken labelToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(labelToken);
			_labelToken = labelToken;
		}

		internal LabelSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
			if (syntaxToken != null)
			{
				AdjustFlagsAndWidth(syntaxToken);
				_labelToken = syntaxToken;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_labelToken);
		}

		static LabelSyntax()
		{
			CreateInstance = (ObjectReader o) => new LabelSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(LabelSyntax), (ObjectReader r) => new LabelSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _labelToken;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new LabelSyntax(base.Kind, newErrors, GetAnnotations(), _labelToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new LabelSyntax(base.Kind, GetDiagnostics(), annotations, _labelToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitLabel(this);
		}
	}
}
