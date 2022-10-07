Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SynthesizedParameterSymbolWithCustomModifiers
		Inherits SynthesizedParameterSymbol
		Private ReadOnly _customModifiers As ImmutableArray(Of CustomModifier)

		Private ReadOnly _refCustomModifiers As ImmutableArray(Of CustomModifier)

		Public NotOverridable Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._customModifiers
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._refCustomModifiers
			End Get
		End Property

		Public Sub New(ByVal container As MethodSymbol, ByVal type As TypeSymbol, ByVal ordinal As Integer, ByVal isByRef As Boolean, ByVal name As String, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal refCustomModifiers As ImmutableArray(Of CustomModifier))
			MyBase.New(container, type, ordinal, isByRef, name, False, Nothing)
			Me._customModifiers = customModifiers
			Me._refCustomModifiers = refCustomModifiers
		End Sub
	End Class
End Namespace