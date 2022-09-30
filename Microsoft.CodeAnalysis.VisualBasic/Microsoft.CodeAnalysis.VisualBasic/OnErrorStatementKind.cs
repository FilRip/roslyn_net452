namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal enum OnErrorStatementKind : byte
	{
		GoToZero,
		GoToMinusOne,
		GoToLabel,
		ResumeNext
	}
}
