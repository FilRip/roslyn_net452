Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class MissingModuleSymbolWithName
		Inherits MissingModuleSymbol
		Private ReadOnly _name As String

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Public Sub New(ByVal assembly As AssemblySymbol, ByVal name As String)
			MyBase.New(assembly, -1)
			Me._name = name
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Dim flag As Boolean
			If (Me <> obj) Then
				Dim missingModuleSymbolWithName As Microsoft.CodeAnalysis.VisualBasic.Symbols.MissingModuleSymbolWithName = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.MissingModuleSymbolWithName)
				flag = If(missingModuleSymbolWithName Is Nothing OrElse Not Me.m_Assembly.Equals(missingModuleSymbolWithName.m_Assembly), False, [String].Equals(Me._name, missingModuleSymbolWithName._name, StringComparison.OrdinalIgnoreCase))
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(Me.m_Assembly.GetHashCode(), StringComparer.OrdinalIgnoreCase.GetHashCode(Me._name))
		End Function
	End Class
End Namespace