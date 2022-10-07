Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SignatureOnlyParameterSymbol
		Inherits ParameterSymbol
		Private ReadOnly _type As TypeSymbol

		Private ReadOnly _customModifiers As ImmutableArray(Of CustomModifier)

		Private ReadOnly _refCustomModifiers As ImmutableArray(Of CustomModifier)

		Private ReadOnly _defaultValue As ConstantValue

		Private ReadOnly _isParamArray As Boolean

		Private ReadOnly _isByRef As Boolean

		Private ReadOnly _isOut As Boolean

		Private ReadOnly _isOptional As Boolean

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._customModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property ExplicitDefaultConstantValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol)) As ConstantValue
			Get
				Return Me._defaultValue
			End Get
		End Property

		Public Overrides ReadOnly Property HasExplicitDefaultValue As Boolean
			Get
				Return CObj(Me._defaultValue) <> CObj(Nothing)
			End Get
		End Property

		Friend Overrides ReadOnly Property HasOptionCompare As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsByRef As Boolean
			Get
				Return Me._isByRef
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerFilePath As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerLineNumber As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerMemberName As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExplicitByRef As Boolean
			Get
				Return Me._isByRef
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIDispatchConstant As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIUnknownConstant As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataIn As Boolean
			Get
				Return Not Me._isOut
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataOut As Boolean
			Get
				Return Me._isOut
			End Get
		End Property

		Public Overrides ReadOnly Property IsOptional As Boolean
			Get
				Return Me._isOptional
			End Get
		End Property

		Public Overrides ReadOnly Property IsParamArray As Boolean
			Get
				Return Me._isParamArray
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return ""
			End Get
		End Property

		Public Overrides ReadOnly Property Ordinal As Integer
			Get
				Throw ExceptionUtilities.Unreachable
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

		Public Sub New(ByVal type As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal refCustomModifiers As ImmutableArray(Of CustomModifier), ByVal defaultConstantValue As ConstantValue, ByVal isParamArray As Boolean, ByVal isByRef As Boolean, ByVal isOut As Boolean, ByVal isOptional As Boolean)
			MyBase.New()
			Me._type = type
			Me._customModifiers = customModifiers
			Me._refCustomModifiers = refCustomModifiers
			Me._defaultValue = defaultConstantValue
			Me._isParamArray = isParamArray
			Me._isByRef = isByRef
			Me._isOut = isOut
			Me._isOptional = isOptional
		End Sub
	End Class
End Namespace