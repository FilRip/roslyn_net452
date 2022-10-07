Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class IgnoreBaseClassesBinder
		Inherits Binder
		Public Sub New(ByVal containingBinder As Binder)
			MyBase.New(containingBinder, Nothing, New Nullable(Of Boolean)(True))
		End Sub
	End Class
End Namespace