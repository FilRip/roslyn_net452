Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class DescendantBinderFactory
		Private ReadOnly _rootBinder As ExecutableCodeBinder

		Private ReadOnly _root As SyntaxNode

		Private _lazyNodeToBinderMap As ImmutableDictionary(Of SyntaxNode, BlockBaseBinder)

		Private _lazyStmtListToBinderMap As ImmutableDictionary(Of SyntaxList(Of StatementSyntax), BlockBaseBinder)

		Friend ReadOnly Property NodeToBinderMap As ImmutableDictionary(Of SyntaxNode, BlockBaseBinder)
			Get
				If (Me._lazyNodeToBinderMap Is Nothing) Then
					Me.BuildBinderMaps()
				End If
				Return Me._lazyNodeToBinderMap
			End Get
		End Property

		Friend ReadOnly Property Root As SyntaxNode
			Get
				Return Me._root
			End Get
		End Property

		Friend ReadOnly Property RootBinder As ExecutableCodeBinder
			Get
				Return Me._rootBinder
			End Get
		End Property

		Friend ReadOnly Property StmtListToBinderMap As ImmutableDictionary(Of SyntaxList(Of StatementSyntax), BlockBaseBinder)
			Get
				If (Me._lazyStmtListToBinderMap Is Nothing) Then
					Me.BuildBinderMaps()
				End If
				Return Me._lazyStmtListToBinderMap
			End Get
		End Property

		Public Sub New(ByVal binder As ExecutableCodeBinder, ByVal root As SyntaxNode)
			MyBase.New()
			Me._rootBinder = binder
			Me._root = root
		End Sub

		Private Sub BuildBinderMaps()
			Dim localBinderBuilder As Microsoft.CodeAnalysis.VisualBasic.LocalBinderBuilder = New Microsoft.CodeAnalysis.VisualBasic.LocalBinderBuilder(DirectCast(Me._rootBinder.ContainingMember, MethodSymbol))
			localBinderBuilder.MakeBinder(Me.Root, Me.RootBinder)
			Interlocked.CompareExchange(Of ImmutableDictionary(Of SyntaxNode, BlockBaseBinder))(Me._lazyNodeToBinderMap, localBinderBuilder.NodeToBinderMap, Nothing)
			Interlocked.CompareExchange(Of ImmutableDictionary(Of SyntaxList(Of StatementSyntax), BlockBaseBinder))(Me._lazyStmtListToBinderMap, localBinderBuilder.StmtListToBinderMap, Nothing)
		End Sub

		Friend Function GetBinder(ByVal node As SyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim blockBaseBinder As Microsoft.CodeAnalysis.VisualBasic.BlockBaseBinder = Nothing
			If (Not Me.NodeToBinderMap.TryGetValue(node, blockBaseBinder)) Then
				binder = Nothing
			Else
				binder = blockBaseBinder
			End If
			Return binder
		End Function

		Friend Function GetBinder(ByVal statementList As SyntaxList(Of StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim blockBaseBinder As Microsoft.CodeAnalysis.VisualBasic.BlockBaseBinder = Nothing
			If (Not Me.StmtListToBinderMap.TryGetValue(statementList, blockBaseBinder)) Then
				binder = Nothing
			Else
				binder = blockBaseBinder
			End If
			Return binder
		End Function
	End Class
End Namespace