using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class AsClauseSyntax : VisualBasicSyntaxNode
	{
		internal readonly KeywordSyntax _asKeyword;

		internal KeywordSyntax AsKeyword => _asKeyword;

		internal AsClauseSyntax(SyntaxKind kind, KeywordSyntax asKeyword)
			: base(kind)
		{
			AdjustFlagsAndWidth(asKeyword);
			_asKeyword = asKeyword;
		}

		internal AsClauseSyntax(SyntaxKind kind, KeywordSyntax asKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
			AdjustFlagsAndWidth(asKeyword);
			_asKeyword = asKeyword;
		}

		internal AsClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax asKeyword)
			: base(kind, errors, annotations)
		{
			AdjustFlagsAndWidth(asKeyword);
			_asKeyword = asKeyword;
		}

		internal AsClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_asKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_asKeyword);
		}
	}
}
