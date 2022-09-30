using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class BadDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal static Func<ObjectReader, object> CreateInstance;

		internal BadDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken)
			: base(kind, hashToken)
		{
			base._slotCount = 1;
		}

		internal BadDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, ISyntaxFactoryContext context)
			: base(kind, hashToken)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
		}

		internal BadDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken)
			: base(kind, errors, annotations, hashToken)
		{
			base._slotCount = 1;
		}

		internal BadDirectiveTriviaSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
		}

		static BadDirectiveTriviaSyntax()
		{
			CreateInstance = (ObjectReader o) => new BadDirectiveTriviaSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(BadDirectiveTriviaSyntax), (ObjectReader r) => new BadDirectiveTriviaSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.BadDirectiveTriviaSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _hashToken;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new BadDirectiveTriviaSyntax(base.Kind, newErrors, GetAnnotations(), _hashToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new BadDirectiveTriviaSyntax(base.Kind, GetDiagnostics(), annotations, _hashToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitBadDirectiveTrivia(this);
		}
	}
}
