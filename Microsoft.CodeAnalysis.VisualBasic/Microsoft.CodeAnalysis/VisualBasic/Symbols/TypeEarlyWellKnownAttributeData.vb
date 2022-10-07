Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class TypeEarlyWellKnownAttributeData
		Inherits CommonTypeEarlyWellKnownAttributeData
		Private _hasVisualBasicEmbeddedAttribute As Boolean

		Private _hasAttributeForExtensibleInterface As Boolean

		Friend Property HasAttributeForExtensibleInterface As Boolean
			Get
				Return Me._hasAttributeForExtensibleInterface
			End Get
			Set(ByVal value As Boolean)
				Me._hasAttributeForExtensibleInterface = value
			End Set
		End Property

		Friend Property HasVisualBasicEmbeddedAttribute As Boolean
			Get
				Return Me._hasVisualBasicEmbeddedAttribute
			End Get
			Set(ByVal value As Boolean)
				Me._hasVisualBasicEmbeddedAttribute = value
			End Set
		End Property

		Public Sub New()
			MyBase.New()
			Me._hasVisualBasicEmbeddedAttribute = False
			Me._hasAttributeForExtensibleInterface = False
		End Sub
	End Class
End Namespace