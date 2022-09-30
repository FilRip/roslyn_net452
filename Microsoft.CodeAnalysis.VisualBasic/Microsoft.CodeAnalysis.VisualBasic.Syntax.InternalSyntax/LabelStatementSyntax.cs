using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class LabelStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly SyntaxToken _labelToken;

		internal readonly PunctuationSyntax _colonToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal SyntaxToken LabelToken => _labelToken;

		internal PunctuationSyntax ColonToken => _colonToken;

		internal LabelStatementSyntax(SyntaxKind kind, SyntaxToken labelToken, PunctuationSyntax colonToken)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(labelToken);
			_labelToken = labelToken;
			AdjustFlagsAndWidth(colonToken);
			_colonToken = colonToken;
		}

		internal LabelStatementSyntax(SyntaxKind kind, SyntaxToken labelToken, PunctuationSyntax colonToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(labelToken);
			_labelToken = labelToken;
			AdjustFlagsAndWidth(colonToken);
			_colonToken = colonToken;
		}

		internal LabelStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxToken labelToken, PunctuationSyntax colonToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(labelToken);
			_labelToken = labelToken;
			AdjustFlagsAndWidth(colonToken);
			_colonToken = colonToken;
		}

		internal LabelStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
			if (syntaxToken != null)
			{
				AdjustFlagsAndWidth(syntaxToken);
				_labelToken = syntaxToken;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_colonToken = punctuationSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_labelToken);
			writer.WriteValue(_colonToken);
		}

		static LabelStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new LabelStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(LabelStatementSyntax), (ObjectReader r) => new LabelStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _labelToken, 
				1 => _colonToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new LabelStatementSyntax(base.Kind, newErrors, GetAnnotations(), _labelToken, _colonToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new LabelStatementSyntax(base.Kind, GetDiagnostics(), annotations, _labelToken, _colonToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitLabelStatement(this);
		}
	}
}
