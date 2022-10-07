Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class SingleLineLambdaExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaExpressionSyntax
		Friend _body As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use SubOrFunctionHeader instead.", True)>
		Public Shadows ReadOnly Property Begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax
			Get
				Return Me.SubOrFunctionHeader
			End Get
		End Property

		Public ReadOnly Property Body As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(Me._body, 1)
			End Get
		End Property

		Friend ReadOnly Property Statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(Me.Body)
			End Get
		End Property

		Public Shadows ReadOnly Property SubOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax)(Me._subOrFunctionHeader)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax, ByVal body As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax(kind, errors, annotations, DirectCast(subOrFunctionHeader.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax), DirectCast(body.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitSingleLineLambdaExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitSingleLineLambdaExpression(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._subOrFunctionHeader
			ElseIf (num = 1) Then
				syntaxNode = Me._body
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim subOrFunctionHeader As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				subOrFunctionHeader = Me.SubOrFunctionHeader
			ElseIf (num = 1) Then
				subOrFunctionHeader = Me.Body
			Else
				subOrFunctionHeader = Nothing
			End If
			Return subOrFunctionHeader
		End Function

		Friend Overrides Function GetSubOrFunctionHeaderCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax
			Return Me.SubOrFunctionHeader
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax, ByVal body As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineLambdaExpressionSyntax
			Dim singleLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineLambdaExpressionSyntax
			If (kind <> MyBase.Kind() OrElse subOrFunctionHeader <> Me.SubOrFunctionHeader OrElse body <> Me.Body) Then
				Dim singleLineLambdaExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineLambdaExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SingleLineLambdaExpression(kind, subOrFunctionHeader, body)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				singleLineLambdaExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, singleLineLambdaExpressionSyntax1, singleLineLambdaExpressionSyntax1.WithAnnotations(annotations))
			Else
				singleLineLambdaExpressionSyntax = Me
			End If
			Return singleLineLambdaExpressionSyntax
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use WithSubOrFunctionHeader instead.", True)>
		Public Function WithBegin(ByVal begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineLambdaExpressionSyntax
			Return Me.WithSubOrFunctionHeader(begin)
		End Function

		Public Function WithBody(ByVal body As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineLambdaExpressionSyntax
			Return Me.Update(MyBase.Kind(), Me.SubOrFunctionHeader, body)
		End Function

		Public Shadows Function WithSubOrFunctionHeader(ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineLambdaExpressionSyntax
			Return Me.Update(MyBase.Kind(), subOrFunctionHeader, Me.Body)
		End Function

		Friend Overrides Function WithSubOrFunctionHeaderCore(ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaExpressionSyntax
			Return Me.WithSubOrFunctionHeader(subOrFunctionHeader)
		End Function
	End Class
End Namespace