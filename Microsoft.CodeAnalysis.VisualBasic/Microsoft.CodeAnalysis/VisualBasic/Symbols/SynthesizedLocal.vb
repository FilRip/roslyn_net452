Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Text

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	<DebuggerDisplay("{GetDebuggerDisplay(), nq}")>
	Friend Class SynthesizedLocal
		Inherits LocalSymbol
		Private ReadOnly _kind As SynthesizedLocalKind

		Private ReadOnly _isByRef As Boolean

		Private ReadOnly _syntaxOpt As SyntaxNode

		Friend Overrides ReadOnly Property DeclarationKind As LocalDeclarationKind
			Get
				Return LocalDeclarationKind.None
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				If (Me._syntaxOpt Is Nothing) Then
					Return ImmutableArray(Of SyntaxReference).Empty
				End If
				Return ImmutableArray.Create(Of SyntaxReference)(Me._syntaxOpt.GetReference())
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IdentifierLocation As Location
			Get
				Return NoLocation.Singleton
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IdentifierToken As SyntaxToken
			Get
				Return New SyntaxToken()
			End Get
		End Property

		Friend Overrides ReadOnly Property IsByRef As Boolean
			Get
				Return Me._isByRef
			End Get
		End Property

		Public Overrides ReadOnly Property IsFunctionValue As Boolean
			Get
				Return Me._kind = SynthesizedLocalKind.FunctionReturnValue
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				If (Me._syntaxOpt Is Nothing) Then
					Return ImmutableArray(Of Location).Empty
				End If
				Return ImmutableArray.Create(Of Location)(Me._syntaxOpt.GetLocation())
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property SynthesizedKind As SynthesizedLocalKind
			Get
				Return Me._kind
			End Get
		End Property

		Friend Sub New(ByVal container As Symbol, ByVal type As TypeSymbol, ByVal kind As SynthesizedLocalKind, Optional ByVal syntaxOpt As SyntaxNode = Nothing, Optional ByVal isByRef As Boolean = False)
			MyBase.New(container, type)
			Me._kind = kind
			Me._syntaxOpt = syntaxOpt
			Me._isByRef = isByRef
		End Sub

		Private Function GetDebuggerDisplay() As String
			Dim stringBuilder As System.Text.StringBuilder = New System.Text.StringBuilder()
			stringBuilder.Append(Me._kind.ToString())
			stringBuilder.Append(" ")
			stringBuilder.Append(Me.Type.ToDisplayString(SymbolDisplayFormat.TestFormat))
			Return stringBuilder.ToString()
		End Function

		Friend Overrides Function GetDeclaratorSyntax() As SyntaxNode
			Return Me._syntaxOpt
		End Function
	End Class
End Namespace