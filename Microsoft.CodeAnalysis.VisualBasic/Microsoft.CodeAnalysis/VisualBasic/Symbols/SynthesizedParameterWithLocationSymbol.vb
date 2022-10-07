Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedParameterWithLocationSymbol
		Inherits SynthesizedParameterSymbol
		Private ReadOnly _locations As ImmutableArray(Of Location)

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._locations
			End Get
		End Property

		Public Sub New(ByVal container As MethodSymbol, ByVal type As TypeSymbol, ByVal ordinal As Integer, ByVal isByRef As Boolean, ByVal name As String, ByVal location As Microsoft.CodeAnalysis.Location)
			MyBase.New(container, type, ordinal, isByRef, name)
			Me._locations = ImmutableArray.Create(Of Microsoft.CodeAnalysis.Location)(location)
		End Sub
	End Class
End Namespace