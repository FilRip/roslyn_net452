using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class FieldInitializerSyntax : VisualBasicSyntaxNode
	{
		internal readonly KeywordSyntax _keyKeyword;

		internal KeywordSyntax KeyKeyword => _keyKeyword;

		internal FieldInitializerSyntax(SyntaxKind kind, KeywordSyntax keyKeyword)
			: base(kind)
		{
			if (keyKeyword != null)
			{
				AdjustFlagsAndWidth(keyKeyword);
				_keyKeyword = keyKeyword;
			}
		}

		internal FieldInitializerSyntax(SyntaxKind kind, KeywordSyntax keyKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
			if (keyKeyword != null)
			{
				AdjustFlagsAndWidth(keyKeyword);
				_keyKeyword = keyKeyword;
			}
		}

		internal FieldInitializerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyKeyword)
			: base(kind, errors, annotations)
		{
			if (keyKeyword != null)
			{
				AdjustFlagsAndWidth(keyKeyword);
				_keyKeyword = keyKeyword;
			}
		}

		internal FieldInitializerSyntax(ObjectReader reader)
			: base(reader)
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_keyKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_keyKeyword);
		}
	}
}
