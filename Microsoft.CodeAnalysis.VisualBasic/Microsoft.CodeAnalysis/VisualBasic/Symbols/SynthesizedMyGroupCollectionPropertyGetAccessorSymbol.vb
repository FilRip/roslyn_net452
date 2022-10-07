Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SynthesizedMyGroupCollectionPropertyGetAccessorSymbol
		Inherits SynthesizedMyGroupCollectionPropertyAccessorSymbol
		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Microsoft.CodeAnalysis.MethodKind.PropertyGet
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return MyBase.PropertyOrEvent.Type
			End Get
		End Property

		Public Sub New(ByVal container As SourceNamedTypeSymbol, ByVal [property] As SynthesizedMyGroupCollectionPropertySymbol, ByVal createMethod As String)
			MyBase.New(container, [property], createMethod)
		End Sub

		Protected Overrides Function GetMethodBlock(ByVal fieldName As String, ByVal createMethodName As String, ByVal targetTypeName As String) As String
			Return [String].Concat(New [String]() { "Get" & VbCrLf & "", fieldName, " = ", createMethodName, "(Of ", targetTypeName, ")(", fieldName, ")" & VbCrLf & "Return ", fieldName, "" & VbCrLf & "End Get" & VbCrLf & "" })
		End Function
	End Class
End Namespace