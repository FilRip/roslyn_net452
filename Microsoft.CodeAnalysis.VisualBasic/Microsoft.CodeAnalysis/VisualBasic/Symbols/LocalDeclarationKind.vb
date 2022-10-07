Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Enum LocalDeclarationKind As Byte
		None
		Variable
		ImplicitVariable
		Constant
		[Static]
		[Using]
		[Catch]
		[For]
		ForEach
		FunctionValue
		AmbiguousLocals
	End Enum
End Namespace