using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class DirectiveTriviaSyntax : StructuredTriviaSyntax
	{
		internal readonly PunctuationSyntax _hashToken;

		internal PunctuationSyntax HashToken => _hashToken;

		internal DirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken)
			: base(kind)
		{
			AdjustFlagsAndWidth(hashToken);
			_hashToken = hashToken;
			SetFlags(NodeFlags.ContainsDirectives);
		}

		internal DirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
			AdjustFlagsAndWidth(hashToken);
			_hashToken = hashToken;
			SetFlags(NodeFlags.ContainsDirectives);
		}

		internal DirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken)
			: base(kind, errors, annotations)
		{
			AdjustFlagsAndWidth(hashToken);
			_hashToken = hashToken;
			SetFlags(NodeFlags.ContainsDirectives);
		}

		internal DirectiveTriviaSyntax(ObjectReader reader)
			: base(reader)
		{
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_hashToken = punctuationSyntax;
			}
			SetFlags(NodeFlags.ContainsDirectives);
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_hashToken);
		}
	}
}
