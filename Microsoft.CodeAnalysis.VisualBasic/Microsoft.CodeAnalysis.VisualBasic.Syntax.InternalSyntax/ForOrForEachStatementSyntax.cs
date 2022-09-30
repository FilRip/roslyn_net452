using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class ForOrForEachStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _forKeyword;

		internal readonly VisualBasicSyntaxNode _controlVariable;

		internal KeywordSyntax ForKeyword => _forKeyword;

		internal VisualBasicSyntaxNode ControlVariable => _controlVariable;

		internal ForOrForEachStatementSyntax(SyntaxKind kind, KeywordSyntax forKeyword, VisualBasicSyntaxNode controlVariable)
			: base(kind)
		{
			AdjustFlagsAndWidth(forKeyword);
			_forKeyword = forKeyword;
			AdjustFlagsAndWidth(controlVariable);
			_controlVariable = controlVariable;
		}

		internal ForOrForEachStatementSyntax(SyntaxKind kind, KeywordSyntax forKeyword, VisualBasicSyntaxNode controlVariable, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
			AdjustFlagsAndWidth(forKeyword);
			_forKeyword = forKeyword;
			AdjustFlagsAndWidth(controlVariable);
			_controlVariable = controlVariable;
		}

		internal ForOrForEachStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax forKeyword, VisualBasicSyntaxNode controlVariable)
			: base(kind, errors, annotations)
		{
			AdjustFlagsAndWidth(forKeyword);
			_forKeyword = forKeyword;
			AdjustFlagsAndWidth(controlVariable);
			_controlVariable = controlVariable;
		}

		internal ForOrForEachStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_forKeyword = keywordSyntax;
			}
			VisualBasicSyntaxNode visualBasicSyntaxNode = (VisualBasicSyntaxNode)reader.ReadValue();
			if (visualBasicSyntaxNode != null)
			{
				AdjustFlagsAndWidth(visualBasicSyntaxNode);
				_controlVariable = visualBasicSyntaxNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_forKeyword);
			writer.WriteValue(_controlVariable);
		}
	}
}
