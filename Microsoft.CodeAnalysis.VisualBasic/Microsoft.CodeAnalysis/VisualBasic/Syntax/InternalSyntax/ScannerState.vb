Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Enum ScannerState
		VB
		VBAllowLeadingMultilineTrivia
		Misc
		DocType
		Element
		EndElement
		SingleQuotedString
		SmartSingleQuotedString
		QuotedString
		SmartQuotedString
		UnQuotedString
		Content
		CData
		StartProcessingInstruction
		ProcessingInstruction
		Comment
		InterpolatedStringPunctuation
		InterpolatedStringContent
		InterpolatedStringFormatString
	End Enum
End Namespace