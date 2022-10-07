Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SourceClonedParameterSymbol
		Inherits SourceParameterSymbolBase
		Private ReadOnly _originalParam As SourceParameterSymbol

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._originalParam.CustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property ExplicitDefaultConstantValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol)) As ConstantValue
			Get
				Return Me._originalParam(inProgress)
			End Get
		End Property

		Friend Overrides ReadOnly Property HasDefaultValueAttribute As Boolean
			Get
				Return Me._originalParam.HasDefaultValueAttribute
			End Get
		End Property

		Public Overrides ReadOnly Property HasExplicitDefaultValue As Boolean
			Get
				Return Me._originalParam.HasExplicitDefaultValue
			End Get
		End Property

		Friend Overrides ReadOnly Property HasOptionCompare As Boolean
			Get
				Return Me._originalParam.HasOptionCompare
			End Get
		End Property

		Friend Overrides ReadOnly Property HasParamArrayAttribute As Boolean
			Get
				Return Me._originalParam.HasParamArrayAttribute
			End Get
		End Property

		Public Overrides ReadOnly Property IsByRef As Boolean
			Get
				Return Me._originalParam.IsByRef
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerFilePath As Boolean
			Get
				Return Me._originalParam.IsCallerFilePath
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerLineNumber As Boolean
			Get
				Return Me._originalParam.IsCallerLineNumber
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerMemberName As Boolean
			Get
				Return Me._originalParam.IsCallerMemberName
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExplicitByRef As Boolean
			Get
				Return Me._originalParam.IsExplicitByRef
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIDispatchConstant As Boolean
			Get
				Return Me._originalParam.IsIDispatchConstant
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIUnknownConstant As Boolean
			Get
				Return Me._originalParam.IsIUnknownConstant
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataIn As Boolean
			Get
				Return Me._originalParam.IsMetadataIn
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataOut As Boolean
			Get
				Return Me._originalParam.IsMetadataOut
			End Get
		End Property

		Public Overrides ReadOnly Property IsOptional As Boolean
			Get
				Return Me._originalParam.IsOptional
			End Get
		End Property

		Public Overrides ReadOnly Property IsParamArray As Boolean
			Get
				Return Me._originalParam.IsParamArray
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._originalParam.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Me._originalParam.MarshallingInformation
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._originalParam.Name
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._originalParam.RefCustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._originalParam.Type
			End Get
		End Property

		Friend Sub New(ByVal originalParam As SourceParameterSymbol, ByVal newOwner As MethodSymbol, ByVal newOrdinal As Integer)
			MyBase.New(newOwner, newOrdinal)
			Me._originalParam = originalParam
		End Sub

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me._originalParam.GetAttributes()
		End Function

		Friend Overrides Function WithTypeAndCustomModifiers(ByVal type As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal refCustomModifiers As ImmutableArray(Of CustomModifier)) As ParameterSymbol
			Return New SourceClonedParameterSymbol.SourceClonedParameterSymbolWithCustomModifiers(Me._originalParam, DirectCast(MyBase.ContainingSymbol, MethodSymbol), MyBase.Ordinal, type, customModifiers, refCustomModifiers)
		End Function

		Friend NotInheritable Class SourceClonedParameterSymbolWithCustomModifiers
			Inherits SourceClonedParameterSymbol
			Private ReadOnly _type As TypeSymbol

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

			Public Overrides ReadOnly Property Type As TypeSymbol
				Get
					Return Me._type
				End Get
			End Property

			Friend Sub New(ByVal originalParam As SourceParameterSymbol, ByVal newOwner As MethodSymbol, ByVal newOrdinal As Integer, ByVal type As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal refCustomModifiers As ImmutableArray(Of CustomModifier))
				MyBase.New(originalParam, newOwner, newOrdinal)
				Me._type = type
				Me._customModifiers = Microsoft.CodeAnalysis.ImmutableArrayExtensions.NullToEmpty(Of CustomModifier)(customModifiers)
				Me._refCustomModifiers = Microsoft.CodeAnalysis.ImmutableArrayExtensions.NullToEmpty(Of CustomModifier)(refCustomModifiers)
			End Sub

			Friend Overrides Function WithTypeAndCustomModifiers(ByVal type As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal refCustomModifiers As ImmutableArray(Of CustomModifier)) As ParameterSymbol
				Throw ExceptionUtilities.Unreachable
			End Function
		End Class
	End Class
End Namespace