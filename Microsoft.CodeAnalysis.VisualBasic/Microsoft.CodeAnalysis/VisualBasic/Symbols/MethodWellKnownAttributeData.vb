Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class MethodWellKnownAttributeData
		Inherits CommonMethodWellKnownAttributeData
		Private _hasSTAThreadAttribute As Boolean

		Private _hasMTAThreadAttribute As Boolean

		Private _isPropertyAccessorWithDebuggerHiddenAttribute As Boolean

		Friend Property HasMTAThreadAttribute As Boolean
			Get
				Return Me._hasMTAThreadAttribute
			End Get
			Set(ByVal value As Boolean)
				Me._hasMTAThreadAttribute = value
			End Set
		End Property

		Friend Property HasSTAThreadAttribute As Boolean
			Get
				Return Me._hasSTAThreadAttribute
			End Get
			Set(ByVal value As Boolean)
				Me._hasSTAThreadAttribute = value
			End Set
		End Property

		Friend Property IsPropertyAccessorWithDebuggerHiddenAttribute As Boolean
			Get
				Return Me._isPropertyAccessorWithDebuggerHiddenAttribute
			End Get
			Set(ByVal value As Boolean)
				Me._isPropertyAccessorWithDebuggerHiddenAttribute = value
			End Set
		End Property

		Public Sub New()
			MyBase.New(True)
			Me._hasSTAThreadAttribute = False
			Me._hasMTAThreadAttribute = False
			Me._isPropertyAccessorWithDebuggerHiddenAttribute = False
		End Sub
	End Class
End Namespace