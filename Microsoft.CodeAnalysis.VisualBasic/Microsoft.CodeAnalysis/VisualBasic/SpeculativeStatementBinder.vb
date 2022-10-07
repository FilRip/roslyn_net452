Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class SpeculativeStatementBinder
		Inherits ExecutableCodeBinder
		Public Overrides ReadOnly Property IsSemanticModelBinder As Boolean
			Get
				Return True
			End Get
		End Property

		Public Sub New(ByVal root As VisualBasicSyntaxNode, ByVal containingBinder As Binder)
			MyBase.New(root, containingBinder)
		End Sub
	End Class
End Namespace