Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BlockBaseBinder
		Inherits BlockBaseBinder(Of LocalSymbol)
		Public Sub New(ByVal enclosing As Binder)
			MyBase.New(enclosing)
		End Sub
	End Class
End Namespace