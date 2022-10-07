Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SynthesizedPropertyBackingFieldSymbol
		Inherits SynthesizedBackingFieldBase(Of SourcePropertySymbol)
		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._propertyOrEvent.Type
			End Get
		End Property

		Public Sub New(ByVal [property] As SourcePropertySymbol, ByVal name As String, ByVal isShared As Boolean)
			MyBase.New([property], name, isShared)
		End Sub
	End Class
End Namespace