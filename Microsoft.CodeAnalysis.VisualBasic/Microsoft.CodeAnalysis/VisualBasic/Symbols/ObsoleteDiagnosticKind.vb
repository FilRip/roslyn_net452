Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Enum ObsoleteDiagnosticKind
		NotObsolete
		Suppressed
		Diagnostic
		Lazy
		LazyPotentiallySuppressed
	End Enum
End Namespace