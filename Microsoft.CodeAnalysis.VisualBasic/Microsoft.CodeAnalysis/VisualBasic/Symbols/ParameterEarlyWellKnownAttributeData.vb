Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class ParameterEarlyWellKnownAttributeData
		Inherits CommonParameterEarlyWellKnownAttributeData
		Private _hasMarshalAsAttribute As Boolean

		Private _hasParamArrayAttribute As Boolean

		Friend Property HasMarshalAsAttribute As Boolean
			Get
				Return Me._hasMarshalAsAttribute
			End Get
			Set(ByVal value As Boolean)
				Me._hasMarshalAsAttribute = value
			End Set
		End Property

		Friend Property HasParamArrayAttribute As Boolean
			Get
				Return Me._hasParamArrayAttribute
			End Get
			Set(ByVal value As Boolean)
				Me._hasParamArrayAttribute = value
			End Set
		End Property

		Public Sub New()
			MyBase.New()
		End Sub
	End Class
End Namespace