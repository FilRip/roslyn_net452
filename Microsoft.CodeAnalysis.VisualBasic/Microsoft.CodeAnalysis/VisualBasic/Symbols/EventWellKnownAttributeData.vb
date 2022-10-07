Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class EventWellKnownAttributeData
		Inherits CommonEventWellKnownAttributeData
		Private _hasNonSerializedAttribute As Boolean

		Friend Property HasNonSerializedAttribute As Boolean
			Get
				Return Me._hasNonSerializedAttribute
			End Get
			Set(ByVal value As Boolean)
				Me._hasNonSerializedAttribute = value
			End Set
		End Property

		Public Sub New()
			MyBase.New()
			Me._hasNonSerializedAttribute = False
		End Sub
	End Class
End Namespace