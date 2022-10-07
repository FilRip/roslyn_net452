Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.VisualBasic.CompilerServices
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Module VisualBasicSyntaxNodeCache
		Private Function GetNodeFlags(ByVal context As ISyntaxFactoryContext) As GreenNode.NodeFlags
			Dim defaultNodeFlags As GreenNode.NodeFlags = SyntaxNodeCache.GetDefaultNodeFlags()
			If (context.IsWithinAsyncMethodOrLambda) Then
				defaultNodeFlags = defaultNodeFlags Or GreenNode.NodeFlags.FactoryContextIsInAsync
			End If
			If (context.IsWithinIteratorContext) Then
				defaultNodeFlags = defaultNodeFlags Or GreenNode.NodeFlags.FactoryContextIsInQuery
			End If
			Return defaultNodeFlags
		End Function

		Friend Function TryGetNode(ByVal kind As Integer, ByVal child1 As GreenNode, ByVal context As ISyntaxFactoryContext, ByRef hash As Integer) As GreenNode
			Return SyntaxNodeCache.TryGetNode(kind, child1, VisualBasicSyntaxNodeCache.GetNodeFlags(context), hash)
		End Function

		Friend Function TryGetNode(ByVal kind As Integer, ByVal child1 As GreenNode, ByVal child2 As GreenNode, ByVal context As ISyntaxFactoryContext, ByRef hash As Integer) As GreenNode
			Return SyntaxNodeCache.TryGetNode(kind, child1, child2, VisualBasicSyntaxNodeCache.GetNodeFlags(context), hash)
		End Function

		Friend Function TryGetNode(ByVal kind As Integer, ByVal child1 As GreenNode, ByVal child2 As GreenNode, ByVal child3 As GreenNode, ByVal context As ISyntaxFactoryContext, ByRef hash As Integer) As GreenNode
			Return SyntaxNodeCache.TryGetNode(kind, child1, child2, child3, VisualBasicSyntaxNodeCache.GetNodeFlags(context), hash)
		End Function
	End Module
End Namespace