Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class MergedNamespaceOrTypeDeclaration
		Inherits Declaration
		Protected Sub New(ByVal name As String)
			MyBase.New(name)
		End Sub
	End Class
End Namespace