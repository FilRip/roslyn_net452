Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Structure FieldOrPropertyInitializer
		Public ReadOnly FieldsOrProperties As ImmutableArray(Of Symbol)

		Public ReadOnly Syntax As SyntaxReference

		Friend ReadOnly PrecedingInitializersLength As Integer

		Friend ReadOnly IsMetadataConstant As Boolean

		Public Sub New(ByVal syntax As SyntaxReference, ByVal precedingInitializersLength As Integer)
			Me = New FieldOrPropertyInitializer() With
			{
				.Syntax = syntax,
				.IsMetadataConstant = False,
				.PrecedingInitializersLength = precedingInitializersLength
			}
		End Sub

		Public Sub New(ByVal field As FieldSymbol, ByVal syntax As SyntaxReference, ByVal precedingInitializersLength As Integer)
			Me = New FieldOrPropertyInitializer() With
			{
				.FieldsOrProperties = ImmutableArray.Create(Of Symbol)(field),
				.Syntax = syntax,
				.IsMetadataConstant = field.IsMetadataConstant,
				.PrecedingInitializersLength = precedingInitializersLength
			}
		End Sub

		Public Sub New(ByVal fieldsOrProperties As ImmutableArray(Of Symbol), ByVal syntax As SyntaxReference, ByVal precedingInitializersLength As Integer)
			Me = New FieldOrPropertyInitializer() With
			{
				.FieldsOrProperties = fieldsOrProperties,
				.Syntax = syntax,
				.IsMetadataConstant = False,
				.PrecedingInitializersLength = precedingInitializersLength
			}
		End Sub

		Public Sub New(ByVal [property] As PropertySymbol, ByVal syntax As SyntaxReference, ByVal precedingInitializersLength As Integer)
			Me = New FieldOrPropertyInitializer() With
			{
				.FieldsOrProperties = ImmutableArray.Create(Of Symbol)([property]),
				.Syntax = syntax,
				.IsMetadataConstant = False,
				.PrecedingInitializersLength = precedingInitializersLength
			}
		End Sub
	End Structure
End Namespace