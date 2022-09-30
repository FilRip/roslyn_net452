using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SkippedTokensTriviaSyntax : StructuredTriviaSyntax
	{
		internal readonly GreenNode _tokens;

		internal static Func<ObjectReader, object> CreateInstance;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Tokens => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>(_tokens);

		internal SkippedTokensTriviaSyntax(SyntaxKind kind, GreenNode tokens)
			: base(kind)
		{
			base._slotCount = 1;
			if (tokens != null)
			{
				AdjustFlagsAndWidth(tokens);
				_tokens = tokens;
			}
		}

		internal SkippedTokensTriviaSyntax(SyntaxKind kind, GreenNode tokens, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			if (tokens != null)
			{
				AdjustFlagsAndWidth(tokens);
				_tokens = tokens;
			}
		}

		internal SkippedTokensTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode tokens)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			if (tokens != null)
			{
				AdjustFlagsAndWidth(tokens);
				_tokens = tokens;
			}
		}

		internal SkippedTokensTriviaSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_tokens = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_tokens);
		}

		static SkippedTokensTriviaSyntax()
		{
			CreateInstance = (ObjectReader o) => new SkippedTokensTriviaSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SkippedTokensTriviaSyntax), (ObjectReader r) => new SkippedTokensTriviaSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SkippedTokensTriviaSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _tokens;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SkippedTokensTriviaSyntax(base.Kind, newErrors, GetAnnotations(), _tokens);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SkippedTokensTriviaSyntax(base.Kind, GetDiagnostics(), annotations, _tokens);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSkippedTokensTrivia(this);
		}
	}
}
