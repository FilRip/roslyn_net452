Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class MethodEarlyWellKnownAttributeData
		Inherits CommonMethodEarlyWellKnownAttributeData
		Private _isExtensionMethod As Boolean

		Friend Property IsExtensionMethod As Boolean
			Get
				Return Me._isExtensionMethod
			End Get
			Set(ByVal value As Boolean)
				Me._isExtensionMethod = value
			End Set
		End Property

		Public Sub New()
			MyBase.New()
			Me._isExtensionMethod = False
		End Sub
	End Class
End Namespace