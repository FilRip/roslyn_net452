Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SynthesizedMyGroupCollectionPropertySetAccessorSymbol
		Inherits SynthesizedMyGroupCollectionPropertyAccessorSymbol
		Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Microsoft.CodeAnalysis.MethodKind.PropertySet
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._parameters
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me.ContainingAssembly.GetSpecialType(SpecialType.System_Void)
			End Get
		End Property

		Public Sub New(ByVal container As SourceNamedTypeSymbol, ByVal [property] As SynthesizedMyGroupCollectionPropertySymbol, ByVal disposeMethod As String)
			MyBase.New(container, [property], disposeMethod)
			Me._parameters = ImmutableArrayExtensions.AsImmutableOrNull(Of ParameterSymbol)(New ParameterSymbol() { SynthesizedParameterSymbol.CreateSetAccessorValueParameter(Me, [property], "Value") })
		End Sub

		Protected Overrides Function GetMethodBlock(ByVal fieldName As String, ByVal disposeMethodName As String, ByVal targetTypeName As String) As String
			Return [String].Concat(New [String]() { "Set(ByVal Value As ", targetTypeName, ")" & VbCrLf & "If Value Is ", fieldName, "" & VbCrLf & "Return" & VbCrLf & "End If" & VbCrLf & "If Value IsNot Nothing Then" & VbCrLf & "Throw New Global.System.ArgumentException(""Property can only be set to Nothing"")" & VbCrLf & "End If" & VbCrLf & "", disposeMethodName, "(Of ", targetTypeName, ")(", fieldName, ")" & VbCrLf & "End Set" & VbCrLf & "" })
		End Function
	End Class
End Namespace