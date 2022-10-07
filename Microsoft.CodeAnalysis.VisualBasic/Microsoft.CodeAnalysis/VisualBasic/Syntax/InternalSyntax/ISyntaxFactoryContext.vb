Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Interface ISyntaxFactoryContext
		ReadOnly Property IsWithinAsyncMethodOrLambda As Boolean

		ReadOnly Property IsWithinIteratorContext As Boolean
	End Interface
End Namespace