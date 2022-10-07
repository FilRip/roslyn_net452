Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SubstitutedParameterSymbol
		Inherits ParameterSymbol
		Private ReadOnly _originalDefinition As ParameterSymbol

		Public Overrides ReadOnly Property ContainingSymbol As Symbol

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.TypeSubstitution.SubstituteCustomModifiers(Me._originalDefinition.Type, Me._originalDefinition.CustomModifiers)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._originalDefinition.DeclaringSyntaxReferences
			End Get
		End Property

		Friend Overrides ReadOnly Property ExplicitDefaultConstantValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol)) As ConstantValue
			Get
				Return Me._originalDefinition(inProgress)
			End Get
		End Property

		Public Overrides ReadOnly Property HasExplicitDefaultValue As Boolean
			Get
				Return Me._originalDefinition.HasExplicitDefaultValue
			End Get
		End Property

		Friend Overrides ReadOnly Property HasOptionCompare As Boolean
			Get
				Return Me._originalDefinition.HasOptionCompare
			End Get
		End Property

		Public Overrides ReadOnly Property IsByRef As Boolean
			Get
				Return Me._originalDefinition.IsByRef
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerFilePath As Boolean
			Get
				Return Me._originalDefinition.IsCallerFilePath
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerLineNumber As Boolean
			Get
				Return Me._originalDefinition.IsCallerLineNumber
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerMemberName As Boolean
			Get
				Return Me._originalDefinition.IsCallerMemberName
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExplicitByRef As Boolean
			Get
				Return Me._originalDefinition.IsExplicitByRef
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIDispatchConstant As Boolean
			Get
				Return Me._originalDefinition.IsIDispatchConstant
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._originalDefinition.IsImplicitlyDeclared
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIUnknownConstant As Boolean
			Get
				Return Me._originalDefinition.IsIUnknownConstant
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataIn As Boolean
			Get
				Return Me._originalDefinition.IsMetadataIn
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataOut As Boolean
			Get
				Return Me._originalDefinition.IsMetadataOut
			End Get
		End Property

		Public Overrides ReadOnly Property IsOptional As Boolean
			Get
				Return Me._originalDefinition.IsOptional
			End Get
		End Property

		Public Overrides ReadOnly Property IsParamArray As Boolean
			Get
				Return Me._originalDefinition.IsParamArray
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._originalDefinition.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Me._originalDefinition.MarshallingInformation
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._originalDefinition.MetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._originalDefinition.Name
			End Get
		End Property

		Public Overrides ReadOnly Property Ordinal As Integer
			Get
				Return Me._originalDefinition.Ordinal
			End Get
		End Property

		Public Overrides ReadOnly Property OriginalDefinition As ParameterSymbol
			Get
				Return Me._originalDefinition
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.TypeSubstitution.SubstituteCustomModifiers(Me._originalDefinition.RefCustomModifiers)
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._originalDefinition.Type.InternalSubstituteTypeParameters(Me.TypeSubstitution).Type
			End Get
		End Property

		Protected MustOverride ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution

		Protected Sub New(ByVal originalDefinition As ParameterSymbol)
			MyBase.New()
			Me._originalDefinition = originalDefinition
		End Sub

		Public Shared Function CreateMethodParameter(ByVal container As SubstitutedMethodSymbol, ByVal originalDefinition As ParameterSymbol) As SubstitutedParameterSymbol
			Return New SubstitutedParameterSymbol.SubstitutedMethodParameterSymbol(container, originalDefinition)
		End Function

		Public Shared Function CreatePropertyParameter(ByVal container As SubstitutedPropertySymbol, ByVal originalDefinition As ParameterSymbol) As SubstitutedParameterSymbol
			Return New SubstitutedParameterSymbol.SubstitutedPropertyParameterSymbol(container, originalDefinition)
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Dim flag As Boolean
			If (Me <> obj) Then
				Dim substitutedParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedParameterSymbol = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedParameterSymbol)
				If (substitutedParameterSymbol Is Nothing) Then
					flag = False
				ElseIf (Me._originalDefinition.Equals(substitutedParameterSymbol._originalDefinition)) Then
					flag = If(Me.ContainingSymbol.Equals(substitutedParameterSymbol.ContainingSymbol), True, False)
				Else
					flag = False
				End If
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me._originalDefinition.GetAttributes()
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._originalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim hashCode As Integer = Me._originalDefinition.GetHashCode()
			Return Hash.Combine(Of Symbol)(Me.ContainingSymbol, hashCode)
		End Function

		Friend Class SubstitutedMethodParameterSymbol
			Inherits SubstitutedParameterSymbol
			Private ReadOnly _container As SubstitutedMethodSymbol

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me._container
				End Get
			End Property

			Protected Overrides ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
				Get
					Return Me._container.TypeSubstitution
				End Get
			End Property

			Public Sub New(ByVal container As SubstitutedMethodSymbol, ByVal originalDefinition As ParameterSymbol)
				MyBase.New(originalDefinition)
				Me._container = container
			End Sub
		End Class

		Private NotInheritable Class SubstitutedPropertyParameterSymbol
			Inherits SubstitutedParameterSymbol
			Private ReadOnly _container As SubstitutedPropertySymbol

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me._container
				End Get
			End Property

			Protected Overrides ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
				Get
					Return Me._container.TypeSubstitution
				End Get
			End Property

			Public Sub New(ByVal container As SubstitutedPropertySymbol, ByVal originalDefinition As ParameterSymbol)
				MyBase.New(originalDefinition)
				Me._container = container
			End Sub
		End Class
	End Class
End Namespace