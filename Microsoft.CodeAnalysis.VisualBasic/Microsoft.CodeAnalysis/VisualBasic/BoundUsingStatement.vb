Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundUsingStatement
		Inherits BoundStatement
		Private ReadOnly _ResourceList As ImmutableArray(Of BoundLocalDeclarationBase)

		Private ReadOnly _ResourceExpressionOpt As BoundExpression

		Private ReadOnly _Body As BoundBlock

		Private ReadOnly _UsingInfo As Microsoft.CodeAnalysis.VisualBasic.UsingInfo

		Private ReadOnly _Locals As ImmutableArray(Of LocalSymbol)

		Public ReadOnly Property Body As BoundBlock
			Get
				Return Me._Body
			End Get
		End Property

		Public ReadOnly Property Locals As ImmutableArray(Of LocalSymbol)
			Get
				Return Me._Locals
			End Get
		End Property

		Public ReadOnly Property ResourceExpressionOpt As BoundExpression
			Get
				Return Me._ResourceExpressionOpt
			End Get
		End Property

		Public ReadOnly Property ResourceList As ImmutableArray(Of BoundLocalDeclarationBase)
			Get
				Return Me._ResourceList
			End Get
		End Property

		Public ReadOnly Property UsingInfo As Microsoft.CodeAnalysis.VisualBasic.UsingInfo
			Get
				Return Me._UsingInfo
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal resourceList As ImmutableArray(Of BoundLocalDeclarationBase), ByVal resourceExpressionOpt As BoundExpression, ByVal body As BoundBlock, ByVal usingInfo As Microsoft.CodeAnalysis.VisualBasic.UsingInfo, ByVal locals As ImmutableArray(Of LocalSymbol), Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.UsingStatement, syntax, If(hasErrors OrElse resourceList.NonNullAndHasErrors() OrElse resourceExpressionOpt.NonNullAndHasErrors(), True, body.NonNullAndHasErrors()))
			Me._ResourceList = resourceList
			Me._ResourceExpressionOpt = resourceExpressionOpt
			Me._Body = body
			Me._UsingInfo = usingInfo
			Me._Locals = locals
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitUsingStatement(Me)
		End Function

		Public Function Update(ByVal resourceList As ImmutableArray(Of BoundLocalDeclarationBase), ByVal resourceExpressionOpt As BoundExpression, ByVal body As BoundBlock, ByVal usingInfo As Microsoft.CodeAnalysis.VisualBasic.UsingInfo, ByVal locals As ImmutableArray(Of LocalSymbol)) As Microsoft.CodeAnalysis.VisualBasic.BoundUsingStatement
			Dim boundUsingStatement As Microsoft.CodeAnalysis.VisualBasic.BoundUsingStatement
			If (resourceList <> Me.ResourceList OrElse resourceExpressionOpt <> Me.ResourceExpressionOpt OrElse body <> Me.Body OrElse usingInfo <> Me.UsingInfo OrElse locals <> Me.Locals) Then
				Dim boundUsingStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundUsingStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundUsingStatement(MyBase.Syntax, resourceList, resourceExpressionOpt, body, usingInfo, locals, MyBase.HasErrors)
				boundUsingStatement1.CopyAttributes(Me)
				boundUsingStatement = boundUsingStatement1
			Else
				boundUsingStatement = Me
			End If
			Return boundUsingStatement
		End Function
	End Class
End Namespace