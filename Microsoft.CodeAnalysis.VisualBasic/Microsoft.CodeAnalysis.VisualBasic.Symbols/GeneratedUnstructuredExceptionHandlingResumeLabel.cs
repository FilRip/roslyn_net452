using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class GeneratedUnstructuredExceptionHandlingResumeLabel : GeneratedLabelSymbol
	{
		public readonly StatementSyntax ResumeStatement;

		public GeneratedUnstructuredExceptionHandlingResumeLabel(StatementSyntax resumeStmt)
			: base("$VB$UnstructuredExceptionHandling_TargetResumeLabel")
		{
			ResumeStatement = resumeStmt;
		}
	}
}
