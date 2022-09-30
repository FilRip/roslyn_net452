using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class InstanceExpressionSyntax : ExpressionSyntax
	{
		internal readonly KeywordSyntax _keyword;

		internal KeywordSyntax Keyword => _keyword;

		internal InstanceExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword)
			: base(kind)
		{
			AdjustFlagsAndWidth(keyword);
			_keyword = keyword;
		}

		internal InstanceExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
			AdjustFlagsAndWidth(keyword);
			_keyword = keyword;
		}

		internal InstanceExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword)
			: base(kind, errors, annotations)
		{
			AdjustFlagsAndWidth(keyword);
			_keyword = keyword;
		}

		internal InstanceExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_keyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_keyword);
		}
	}
}
