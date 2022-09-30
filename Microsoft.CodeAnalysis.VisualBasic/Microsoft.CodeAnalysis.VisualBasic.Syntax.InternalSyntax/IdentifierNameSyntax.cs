using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class IdentifierNameSyntax : SimpleNameSyntax
	{
		internal static Func<ObjectReader, object> CreateInstance;

		internal IdentifierNameSyntax(SyntaxKind kind, IdentifierTokenSyntax identifier)
			: base(kind, identifier)
		{
			base._slotCount = 1;
		}

		internal IdentifierNameSyntax(SyntaxKind kind, IdentifierTokenSyntax identifier, ISyntaxFactoryContext context)
			: base(kind, identifier)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
		}

		internal IdentifierNameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax identifier)
			: base(kind, errors, annotations, identifier)
		{
			base._slotCount = 1;
		}

		internal IdentifierNameSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
		}

		static IdentifierNameSyntax()
		{
			CreateInstance = (ObjectReader o) => new IdentifierNameSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(IdentifierNameSyntax), (ObjectReader r) => new IdentifierNameSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _identifier;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new IdentifierNameSyntax(base.Kind, newErrors, GetAnnotations(), _identifier);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new IdentifierNameSyntax(base.Kind, GetDiagnostics(), annotations, _identifier);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitIdentifierName(this);
		}
	}
}
