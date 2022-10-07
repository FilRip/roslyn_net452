Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class PropertyBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclarationStatementSyntax
		Friend _propertyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax

		Friend _accessors As SyntaxNode

		Friend _endPropertyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		Public ReadOnly Property Accessors As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax)(MyBase.GetRed(Me._accessors, 1))
			End Get
		End Property

		Public ReadOnly Property EndPropertyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endPropertyStatement, 2)
			End Get
		End Property

		Public ReadOnly Property PropertyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax)(Me._propertyStatement)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal propertyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax, ByVal accessors As SyntaxNode, ByVal endPropertyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax(kind, errors, annotations, DirectCast(propertyStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax), If(accessors IsNot Nothing, accessors.Green, Nothing), DirectCast(endPropertyStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitPropertyBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitPropertyBlock(Me)
		End Sub

		Public Function AddAccessors(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax
			Return Me.WithAccessors(Me.Accessors.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._propertyStatement
					Exit Select
				Case 1
					syntaxNode = Me._accessors
					Exit Select
				Case 2
					syntaxNode = Me._endPropertyStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim propertyStatement As SyntaxNode
			Select Case i
				Case 0
					propertyStatement = Me.PropertyStatement
					Exit Select
				Case 1
					propertyStatement = MyBase.GetRed(Me._accessors, 1)
					Exit Select
				Case 2
					propertyStatement = Me.EndPropertyStatement
					Exit Select
				Case Else
					propertyStatement = Nothing
					Exit Select
			End Select
			Return propertyStatement
		End Function

		Public Function Update(ByVal propertyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax, ByVal accessors As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax), ByVal endPropertyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax
			Dim propertyBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax
			If (propertyStatement <> Me.PropertyStatement OrElse accessors <> Me.Accessors OrElse endPropertyStatement <> Me.EndPropertyStatement) Then
				Dim propertyBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.PropertyBlock(propertyStatement, accessors, endPropertyStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				propertyBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, propertyBlockSyntax1, propertyBlockSyntax1.WithAnnotations(annotations))
			Else
				propertyBlockSyntax = Me
			End If
			Return propertyBlockSyntax
		End Function

		Public Function WithAccessors(ByVal accessors As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax
			Return Me.Update(Me.PropertyStatement, accessors, Me.EndPropertyStatement)
		End Function

		Public Function WithEndPropertyStatement(ByVal endPropertyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax
			Return Me.Update(Me.PropertyStatement, Me.Accessors, endPropertyStatement)
		End Function

		Public Function WithPropertyStatement(ByVal propertyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax
			Return Me.Update(propertyStatement, Me.Accessors, Me.EndPropertyStatement)
		End Function
	End Class
End Namespace