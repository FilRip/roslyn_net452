Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class EventBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclarationStatementSyntax
		Friend _eventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax

		Friend _accessors As SyntaxNode

		Friend _endEventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		Public ReadOnly Property Accessors As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax)(MyBase.GetRed(Me._accessors, 1))
			End Get
		End Property

		Public ReadOnly Property EndEventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endEventStatement, 2)
			End Get
		End Property

		Public ReadOnly Property EventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax)(Me._eventStatement)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal eventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax, ByVal accessors As SyntaxNode, ByVal endEventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax(kind, errors, annotations, DirectCast(eventStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax), If(accessors IsNot Nothing, accessors.Green, Nothing), DirectCast(endEventStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitEventBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitEventBlock(Me)
		End Sub

		Public Function AddAccessors(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax
			Return Me.WithAccessors(Me.Accessors.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._eventStatement
					Exit Select
				Case 1
					syntaxNode = Me._accessors
					Exit Select
				Case 2
					syntaxNode = Me._endEventStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim eventStatement As SyntaxNode
			Select Case i
				Case 0
					eventStatement = Me.EventStatement
					Exit Select
				Case 1
					eventStatement = MyBase.GetRed(Me._accessors, 1)
					Exit Select
				Case 2
					eventStatement = Me.EndEventStatement
					Exit Select
				Case Else
					eventStatement = Nothing
					Exit Select
			End Select
			Return eventStatement
		End Function

		Public Function Update(ByVal eventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax, ByVal accessors As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax), ByVal endEventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax
			Dim eventBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax
			If (eventStatement <> Me.EventStatement OrElse accessors <> Me.Accessors OrElse endEventStatement <> Me.EndEventStatement) Then
				Dim eventBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.EventBlock(eventStatement, accessors, endEventStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				eventBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, eventBlockSyntax1, eventBlockSyntax1.WithAnnotations(annotations))
			Else
				eventBlockSyntax = Me
			End If
			Return eventBlockSyntax
		End Function

		Public Function WithAccessors(ByVal accessors As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax
			Return Me.Update(Me.EventStatement, accessors, Me.EndEventStatement)
		End Function

		Public Function WithEndEventStatement(ByVal endEventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax
			Return Me.Update(Me.EventStatement, Me.Accessors, endEventStatement)
		End Function

		Public Function WithEventStatement(ByVal eventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax
			Return Me.Update(eventStatement, Me.Accessors, Me.EndEventStatement)
		End Function
	End Class
End Namespace