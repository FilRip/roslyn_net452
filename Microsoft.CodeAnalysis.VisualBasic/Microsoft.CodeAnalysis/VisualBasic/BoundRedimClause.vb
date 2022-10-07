Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundRedimClause
		Inherits BoundStatement
		Private ReadOnly _Operand As BoundExpression

		Private ReadOnly _Indices As ImmutableArray(Of BoundExpression)

		Private ReadOnly _ArrayTypeOpt As ArrayTypeSymbol

		Private ReadOnly _Preserve As Boolean

		Public ReadOnly Property ArrayTypeOpt As ArrayTypeSymbol
			Get
				Return Me._ArrayTypeOpt
			End Get
		End Property

		Public ReadOnly Property Indices As ImmutableArray(Of BoundExpression)
			Get
				Return Me._Indices
			End Get
		End Property

		Public ReadOnly Property Operand As BoundExpression
			Get
				Return Me._Operand
			End Get
		End Property

		Public ReadOnly Property Preserve As Boolean
			Get
				Return Me._Preserve
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operand As BoundExpression, ByVal indices As ImmutableArray(Of BoundExpression), ByVal arrayTypeOpt As ArrayTypeSymbol, ByVal preserve As Boolean, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.RedimClause, syntax, If(hasErrors OrElse operand.NonNullAndHasErrors(), True, indices.NonNullAndHasErrors()))
			Me._Operand = operand
			Me._Indices = indices
			Me._ArrayTypeOpt = arrayTypeOpt
			Me._Preserve = preserve
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitRedimClause(Me)
		End Function

		Public Function Update(ByVal operand As BoundExpression, ByVal indices As ImmutableArray(Of BoundExpression), ByVal arrayTypeOpt As ArrayTypeSymbol, ByVal preserve As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundRedimClause
			Dim boundRedimClause As Microsoft.CodeAnalysis.VisualBasic.BoundRedimClause
			If (operand <> Me.Operand OrElse indices <> Me.Indices OrElse CObj(arrayTypeOpt) <> CObj(Me.ArrayTypeOpt) OrElse preserve <> Me.Preserve) Then
				Dim boundRedimClause1 As Microsoft.CodeAnalysis.VisualBasic.BoundRedimClause = New Microsoft.CodeAnalysis.VisualBasic.BoundRedimClause(MyBase.Syntax, operand, indices, arrayTypeOpt, preserve, MyBase.HasErrors)
				boundRedimClause1.CopyAttributes(Me)
				boundRedimClause = boundRedimClause1
			Else
				boundRedimClause = Me
			End If
			Return boundRedimClause
		End Function
	End Class
End Namespace