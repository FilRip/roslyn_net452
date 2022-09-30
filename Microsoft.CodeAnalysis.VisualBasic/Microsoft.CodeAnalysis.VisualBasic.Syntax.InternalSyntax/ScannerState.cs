namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal enum ScannerState
	{
		VB,
		VBAllowLeadingMultilineTrivia,
		Misc,
		DocType,
		Element,
		EndElement,
		SingleQuotedString,
		SmartSingleQuotedString,
		QuotedString,
		SmartQuotedString,
		UnQuotedString,
		Content,
		CData,
		StartProcessingInstruction,
		ProcessingInstruction,
		Comment,
		InterpolatedStringPunctuation,
		InterpolatedStringContent,
		InterpolatedStringFormatString
	}
}
