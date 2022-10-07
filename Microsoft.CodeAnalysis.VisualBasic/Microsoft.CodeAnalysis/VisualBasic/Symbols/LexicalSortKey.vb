Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Structure LexicalSortKey
		Private _embeddedKind As LexicalSortKey.SyntaxTreeKind

		Private _treeOrdinal As Integer

		Private _position As Integer

		Public ReadOnly Shared NotInSource As LexicalSortKey

		Public ReadOnly Shared NotInitialized As LexicalSortKey

		Private ReadOnly Property EmbeddedKind As LexicalSortKey.SyntaxTreeKind
			Get
				Return Me._embeddedKind
			End Get
		End Property

		Public ReadOnly Property IsInitialized As Boolean
			Get
				Return Volatile.Read(Me._position) >= 0
			End Get
		End Property

		Public ReadOnly Property Position As Integer
			Get
				Return Me._position
			End Get
		End Property

		Public ReadOnly Property TreeOrdinal As Integer
			Get
				Return Me._treeOrdinal
			End Get
		End Property

		Shared Sub New()
			LexicalSortKey.NotInSource = New LexicalSortKey(LexicalSortKey.SyntaxTreeKind.None, -1, 0)
			Dim lexicalSortKey1 As LexicalSortKey = New LexicalSortKey() With
			{
				._embeddedKind = LexicalSortKey.SyntaxTreeKind.None,
				._treeOrdinal = -1,
				._position = -1
			}
			LexicalSortKey.NotInitialized = lexicalSortKey1
		End Sub

		Private Sub New(ByVal embeddedKind As LexicalSortKey.SyntaxTreeKind, ByVal treeOrdinal As Integer, ByVal location As Integer)
			Me = New LexicalSortKey() With
			{
				._embeddedKind = embeddedKind,
				._treeOrdinal = treeOrdinal,
				._position = location
			}
		End Sub

		Private Sub New(ByVal embeddedKind As LexicalSortKey.SyntaxTreeKind, ByVal tree As SyntaxTree, ByVal location As Integer, ByVal compilation As VisualBasicCompilation)
			MyClass.New(embeddedKind, If(tree Is Nothing OrElse embeddedKind <> LexicalSortKey.SyntaxTreeKind.None, -1, compilation.GetSyntaxTreeOrdinal(tree)), location)
		End Sub

		Public Sub New(ByVal tree As SyntaxTree, ByVal position As Integer, ByVal compilation As VisualBasicCompilation)
			MyClass.New(LexicalSortKey.GetEmbeddedKind(tree), tree, position, compilation)
		End Sub

		Public Sub New(ByVal syntaxRef As SyntaxReference, ByVal compilation As VisualBasicCompilation)
			MyClass.New(syntaxRef.SyntaxTree, syntaxRef.Span.Start, compilation)
		End Sub

		Public Sub New(ByVal location As Microsoft.CodeAnalysis.Location, ByVal compilation As VisualBasicCompilation)
			Me = New LexicalSortKey()
			If (location Is Nothing) Then
				Me._embeddedKind = LexicalSortKey.SyntaxTreeKind.None
				Me._treeOrdinal = -1
				Me._position = 0
				Return
			End If
			Dim visualBasicSyntaxTree As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree = DirectCast(location.PossiblyEmbeddedOrMySourceTree(), Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree)
			Dim embeddedKind As LexicalSortKey.SyntaxTreeKind = LexicalSortKey.GetEmbeddedKind(visualBasicSyntaxTree)
			If (embeddedKind = LexicalSortKey.SyntaxTreeKind.None) Then
				Me._embeddedKind = LexicalSortKey.SyntaxTreeKind.None
				Me._treeOrdinal = If(visualBasicSyntaxTree Is Nothing, -1, compilation.GetSyntaxTreeOrdinal(visualBasicSyntaxTree))
			Else
				Me._embeddedKind = embeddedKind
				Me._treeOrdinal = -1
			End If
			Me._position = location.PossiblyEmbeddedOrMySourceSpan().Start
		End Sub

		Public Sub New(ByVal node As VisualBasicSyntaxNode, ByVal compilation As VisualBasicCompilation)
			MyClass.New(node.SyntaxTree, node.SpanStart, compilation)
		End Sub

		Public Sub New(ByVal token As SyntaxToken, ByVal compilation As VisualBasicCompilation)
			MyClass.New(DirectCast(token.SyntaxTree, VisualBasicSyntaxTree), token.SpanStart, compilation)
		End Sub

		Public Shared Function Compare(ByRef xSortKey As LexicalSortKey, ByRef ySortKey As LexicalSortKey) As Integer
			Dim position As Integer
			If (xSortKey.EmbeddedKind <> ySortKey.EmbeddedKind) Then
				position = If(ySortKey.EmbeddedKind > xSortKey.EmbeddedKind, 1, -1)
			ElseIf (xSortKey.EmbeddedKind <> LexicalSortKey.SyntaxTreeKind.None OrElse xSortKey.TreeOrdinal = ySortKey.TreeOrdinal) Then
				position = xSortKey.Position - ySortKey.Position
			ElseIf (xSortKey.TreeOrdinal >= 0) Then
				position = If(ySortKey.TreeOrdinal >= 0, xSortKey.TreeOrdinal - ySortKey.TreeOrdinal, -1)
			Else
				position = 1
			End If
			Return position
		End Function

		Public Shared Function Compare(ByVal first As Location, ByVal second As Location, ByVal compilation As VisualBasicCompilation) As Integer
			Dim start As Integer
			If (first.SourceTree Is Nothing OrElse first.SourceTree <> second.SourceTree) Then
				Dim lexicalSortKey As Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey = New Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey(first, compilation)
				Dim lexicalSortKey1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey = New Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey(second, compilation)
				start = Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey.Compare(lexicalSortKey, lexicalSortKey1)
			Else
				start = first.PossiblyEmbeddedOrMySourceSpan().Start - second.PossiblyEmbeddedOrMySourceSpan().Start
			End If
			Return start
		End Function

		Public Shared Function Compare(ByVal first As SyntaxReference, ByVal second As SyntaxReference, ByVal compilation As VisualBasicCompilation) As Integer
			Dim start As Integer
			If (first.SyntaxTree Is Nothing OrElse first.SyntaxTree <> second.SyntaxTree) Then
				Dim lexicalSortKey As Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey = New Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey(first, compilation)
				Dim lexicalSortKey1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey = New Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey(second, compilation)
				start = Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey.Compare(lexicalSortKey, lexicalSortKey1)
			Else
				start = first.Span.Start - second.Span.Start
			End If
			Return start
		End Function

		Public Shared Function First(ByVal xSortKey As LexicalSortKey, ByVal ySortKey As LexicalSortKey) As LexicalSortKey
			If (LexicalSortKey.Compare(xSortKey, ySortKey) <= 0) Then
				Return xSortKey
			End If
			Return ySortKey
		End Function

		Private Shared Function GetEmbeddedKind(ByVal tree As SyntaxTree) As LexicalSortKey.SyntaxTreeKind
			If (tree Is Nothing) Then
				Return LexicalSortKey.SyntaxTreeKind.None
			End If
			If (tree.IsMyTemplate()) Then
				Return LexicalSortKey.SyntaxTreeKind.MyTemplate
			End If
			Return DirectCast(tree.GetEmbeddedKind(), LexicalSortKey.SyntaxTreeKind)
		End Function

		Public Sub SetFrom(ByRef other As LexicalSortKey)
			Me._embeddedKind = other._embeddedKind
			Me._treeOrdinal = other._treeOrdinal
			Volatile.Write(Me._position, other._position)
		End Sub

		<Flags>
		Private Enum SyntaxTreeKind As Byte
			None = 0
			Unset = 1
			EmbeddedAttribute = 2
			VbCore = 4
			XmlHelper = 8
			MyTemplate = 16
		End Enum
	End Structure
End Namespace