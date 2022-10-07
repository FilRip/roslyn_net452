Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Module SyntaxExtensions
		<Extension>
		Public Function EndsWithEndOfLineOrColonTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Boolean
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = node.LastTriviaIfAny()
			If (visualBasicSyntaxNode Is Nothing) Then
				Return False
			End If
			If (visualBasicSyntaxNode.Kind = SyntaxKind.EndOfLineTrivia) Then
				Return True
			End If
			Return visualBasicSyntaxNode.Kind = SyntaxKind.ColonTrivia
		End Function

		<Extension>
		Public Function LastTriviaIfAny(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim last As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim trailingTrivia As GreenNode = node.GetTrailingTrivia()
			If (trailingTrivia IsNot Nothing) Then
				last = (New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(trailingTrivia)).Last
			Else
				last = Nothing
			End If
			Return last
		End Function

		<Extension>
		Public Function WithAdditionalAnnotations(Of TNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TNode, ByVal ParamArray annotations As SyntaxAnnotation()) As TNode
			If (annotations Is Nothing) Then
				Throw New ArgumentNullException("annotations")
			End If
			Return DirectCast(node.SetAnnotations(node.GetAnnotations().Concat(annotations).ToArray()), TNode)
		End Function

		<Extension>
		Public Function WithAdditionalDiagnostics(Of TNode As GreenNode)(ByVal node As TNode, ByVal ParamArray diagnostics As DiagnosticInfo()) As TNode
			Dim tNode1 As TNode
			Dim diagnosticInfoArray As DiagnosticInfo() = node.GetDiagnostics()
			tNode1 = If(diagnosticInfoArray Is Nothing, node.WithDiagnostics(diagnostics), DirectCast(node.SetDiagnostics(diagnosticInfoArray.Concat(diagnostics).ToArray()), TNode))
			Return tNode1
		End Function

		<Extension>
		Public Function WithAnnotations(Of TNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TNode, ByVal ParamArray annotations As SyntaxAnnotation()) As TNode
			If (annotations Is Nothing) Then
				Throw New ArgumentNullException("annotations")
			End If
			Return DirectCast(node.SetAnnotations(annotations), TNode)
		End Function

		<Extension>
		Public Function WithDiagnostics(Of TNode As GreenNode)(ByVal node As TNode, ByVal ParamArray diagnostics As DiagnosticInfo()) As TNode
			Return DirectCast(node.SetDiagnostics(diagnostics), TNode)
		End Function

		<Extension>
		Public Function WithoutAnnotations(Of TNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TNode, ByVal ParamArray removalAnnotations As Microsoft.CodeAnalysis.SyntaxAnnotation()) As TNode
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.SyntaxAnnotation) = ArrayBuilder(Of Microsoft.CodeAnalysis.SyntaxAnnotation).GetInstance()
			Dim annotations As Microsoft.CodeAnalysis.SyntaxAnnotation() = node.GetAnnotations()
			Dim num As Integer = 0
			Do
				Dim syntaxAnnotation As Microsoft.CodeAnalysis.SyntaxAnnotation = annotations(num)
				If (Array.IndexOf(Of Microsoft.CodeAnalysis.SyntaxAnnotation)(removalAnnotations, syntaxAnnotation) < 0) Then
					instance.Add(syntaxAnnotation)
				End If
				num = num + 1
			Loop While num < CInt(annotations.Length)
			Return DirectCast(node.SetAnnotations(instance.ToArrayAndFree()), TNode)
		End Function

		<Extension>
		Public Function WithoutDiagnostics(Of TNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TNode) As TNode
			Dim tNode1 As TNode
			Dim diagnostics As DiagnosticInfo() = node.GetDiagnostics()
			tNode1 = If(diagnostics Is Nothing OrElse CInt(diagnostics.Length) = 0, node, DirectCast(node.SetDiagnostics(Nothing), TNode))
			Return tNode1
		End Function
	End Module
End Namespace