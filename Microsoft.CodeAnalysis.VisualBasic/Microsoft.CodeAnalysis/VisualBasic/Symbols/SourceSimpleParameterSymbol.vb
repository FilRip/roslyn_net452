Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SourceSimpleParameterSymbol
		Inherits SourceParameterSymbol
		Friend Overrides ReadOnly Property AttributeDeclarationList As SyntaxList(Of AttributeListSyntax)
			Get
				Return New SyntaxList(Of AttributeListSyntax)()
			End Get
		End Property

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property ExplicitDefaultConstantValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol)) As ConstantValue
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property HasExplicitDefaultValue As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerFilePath As Boolean
			Get
				Dim earlyDecodedWellKnownAttributeData As ParameterEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
				If (earlyDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return earlyDecodedWellKnownAttributeData.HasCallerFilePathAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerLineNumber As Boolean
			Get
				Dim earlyDecodedWellKnownAttributeData As ParameterEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
				If (earlyDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return earlyDecodedWellKnownAttributeData.HasCallerLineNumberAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerMemberName As Boolean
			Get
				Dim earlyDecodedWellKnownAttributeData As ParameterEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
				If (earlyDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return earlyDecodedWellKnownAttributeData.HasCallerMemberNameAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExplicitByRef As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOptional As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsParamArray As Boolean
			Get
				Dim earlyDecodedWellKnownAttributeData As ParameterEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
				If (earlyDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return earlyDecodedWellKnownAttributeData.HasParamArrayAttribute
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Friend Sub New(ByVal container As Symbol, ByVal name As String, ByVal ordinal As Integer, ByVal type As TypeSymbol, ByVal location As Microsoft.CodeAnalysis.Location)
			MyBase.New(container, name, ordinal, type, location)
		End Sub

		Friend Overrides Function ChangeOwner(ByVal newContainingSymbol As Symbol) As ParameterSymbol
			Return New SourceSimpleParameterSymbol(newContainingSymbol, MyBase.Name, MyBase.Ordinal, MyBase.Type, MyBase.Location)
		End Function

		Friend Overrides Function GetAttributesBag() As CustomAttributesBag(Of VisualBasicAttributeData)
			Dim empty As CustomAttributesBag(Of VisualBasicAttributeData)
			Dim correspondingPartialParameter As SourceComplexParameterSymbol = Me.GetCorrespondingPartialParameter()
			If (correspondingPartialParameter Is Nothing) Then
				empty = CustomAttributesBag(Of VisualBasicAttributeData).Empty
			Else
				empty = correspondingPartialParameter.GetAttributesBag()
			End If
			Return empty
		End Function

		Private Function GetCorrespondingPartialParameter() As SourceComplexParameterSymbol
			Dim item As SourceComplexParameterSymbol
			Dim containingSymbol As SourceMemberMethodSymbol = TryCast(MyBase.ContainingSymbol, SourceMemberMethodSymbol)
			If (containingSymbol Is Nothing OrElse Not containingSymbol.IsPartialImplementation) Then
				item = Nothing
			Else
				item = DirectCast(containingSymbol.SourcePartialDefinition.Parameters(MyBase.Ordinal), SourceComplexParameterSymbol)
			End If
			Return item
		End Function

		Friend Overrides Function GetDecodedWellKnownAttributeData() As CommonParameterWellKnownAttributeData
			Dim decodedWellKnownAttributeData As CommonParameterWellKnownAttributeData
			Dim correspondingPartialParameter As SourceComplexParameterSymbol = Me.GetCorrespondingPartialParameter()
			If (correspondingPartialParameter Is Nothing) Then
				decodedWellKnownAttributeData = Nothing
			Else
				decodedWellKnownAttributeData = correspondingPartialParameter.GetDecodedWellKnownAttributeData()
			End If
			Return decodedWellKnownAttributeData
		End Function

		Friend Overrides Function GetEarlyDecodedWellKnownAttributeData() As ParameterEarlyWellKnownAttributeData
			Dim earlyDecodedWellKnownAttributeData As ParameterEarlyWellKnownAttributeData
			Dim correspondingPartialParameter As SourceComplexParameterSymbol = Me.GetCorrespondingPartialParameter()
			If (correspondingPartialParameter Is Nothing) Then
				earlyDecodedWellKnownAttributeData = Nothing
			Else
				earlyDecodedWellKnownAttributeData = correspondingPartialParameter.GetEarlyDecodedWellKnownAttributeData()
			End If
			Return earlyDecodedWellKnownAttributeData
		End Function

		Friend Overrides Function WithTypeAndCustomModifiers(ByVal type As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal refCustomModifiers As ImmutableArray(Of CustomModifier)) As ParameterSymbol
			Dim sourceSimpleParameterSymbolWithCustomModifier As ParameterSymbol
			If (Not customModifiers.IsEmpty OrElse Not refCustomModifiers.IsEmpty) Then
				sourceSimpleParameterSymbolWithCustomModifier = New SourceSimpleParameterSymbol.SourceSimpleParameterSymbolWithCustomModifiers(MyBase.ContainingSymbol, MyBase.Name, MyBase.Ordinal, type, MyBase.Location, customModifiers, refCustomModifiers)
			Else
				sourceSimpleParameterSymbolWithCustomModifier = New SourceSimpleParameterSymbol(MyBase.ContainingSymbol, MyBase.Name, MyBase.Ordinal, type, MyBase.Location)
			End If
			Return sourceSimpleParameterSymbolWithCustomModifier
		End Function

		Friend NotInheritable Class SourceSimpleParameterSymbolWithCustomModifiers
			Inherits SourceSimpleParameterSymbol
			Private ReadOnly _customModifiers As ImmutableArray(Of CustomModifier)

			Private ReadOnly _refCustomModifiers As ImmutableArray(Of CustomModifier)

			Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
				Get
					Return Me._customModifiers
				End Get
			End Property

			Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
				Get
					Return Me._refCustomModifiers
				End Get
			End Property

			Friend Sub New(ByVal container As Symbol, ByVal name As String, ByVal ordinal As Integer, ByVal type As TypeSymbol, ByVal location As Microsoft.CodeAnalysis.Location, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal refCustomModifiers As ImmutableArray(Of CustomModifier))
				MyBase.New(container, name, ordinal, type, location)
				Me._customModifiers = customModifiers
				Me._refCustomModifiers = refCustomModifiers
			End Sub

			Friend Overrides Function WithTypeAndCustomModifiers(ByVal type As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal refCustomModifiers As ImmutableArray(Of CustomModifier)) As ParameterSymbol
				Throw ExceptionUtilities.Unreachable
			End Function
		End Class
	End Class
End Namespace