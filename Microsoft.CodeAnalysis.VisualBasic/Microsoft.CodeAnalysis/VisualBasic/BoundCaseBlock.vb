Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundCaseBlock
		Inherits BoundStatement
		Private ReadOnly _CaseStatement As BoundCaseStatement

		Private ReadOnly _Body As BoundBlock

		Public ReadOnly Property Body As BoundBlock
			Get
				Return Me._Body
			End Get
		End Property

		Public ReadOnly Property CaseStatement As BoundCaseStatement
			Get
				Return Me._CaseStatement
			End Get
		End Property

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.CaseStatement, Me.Body)
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal caseStatement As BoundCaseStatement, ByVal body As BoundBlock, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.CaseBlock, syntax, If(hasErrors OrElse caseStatement.NonNullAndHasErrors(), True, body.NonNullAndHasErrors()))
			Me._CaseStatement = caseStatement
			Me._Body = body
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitCaseBlock(Me)
		End Function

		Public Function Update(ByVal caseStatement As BoundCaseStatement, ByVal body As BoundBlock) As Microsoft.CodeAnalysis.VisualBasic.BoundCaseBlock
			Dim boundCaseBlock As Microsoft.CodeAnalysis.VisualBasic.BoundCaseBlock
			If (caseStatement <> Me.CaseStatement OrElse body <> Me.Body) Then
				Dim boundCaseBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundCaseBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundCaseBlock(MyBase.Syntax, caseStatement, body, MyBase.HasErrors)
				boundCaseBlock1.CopyAttributes(Me)
				boundCaseBlock = boundCaseBlock1
			Else
				boundCaseBlock = Me
			End If
			Return boundCaseBlock
		End Function
	End Class
End Namespace