Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SynthesizedRegularMethodBase
		Inherits SynthesizedMethodBase
		Protected ReadOnly m_name As String

		Protected ReadOnly m_isShared As Boolean

		Protected ReadOnly m_SyntaxNode As VisualBasicSyntaxNode

		Friend NotOverridable Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsShared As Boolean
			Get
				Return Me.m_isShared
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me.m_containingType.Locations
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Microsoft.CodeAnalysis.MethodKind.Ordinary
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Name As String
			Get
				Return Me.m_name
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return ImmutableArray(Of ParameterSymbol).Empty
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property Syntax As SyntaxNode
			Get
				Return Me.m_SyntaxNode
			End Get
		End Property

		Protected Sub New(ByVal syntaxNode As VisualBasicSyntaxNode, ByVal container As NamedTypeSymbol, ByVal name As String, Optional ByVal isShared As Boolean = False)
			MyBase.New(container)
			Me.m_SyntaxNode = syntaxNode
			Me.m_isShared = isShared
			Me.m_name = name
		End Sub

		Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
			Return Me.m_containingType.GetLexicalSortKey()
		End Function
	End Class
End Namespace