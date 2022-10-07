Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundRaiseEventStatement
		Inherits BoundStatement
		Implements IBoundInvalidNode
		Private ReadOnly _EventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol

		Private ReadOnly _EventInvocation As BoundExpression

		Public ReadOnly Property EventInvocation As BoundExpression
			Get
				Return Me._EventInvocation
			End Get
		End Property

		Public ReadOnly Property EventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol
			Get
				Return Me._EventSymbol
			End Get
		End Property

		ReadOnly Property IBoundInvalidNode_InvalidNodeChildren As ImmutableArray(Of BoundNode) Implements IBoundInvalidNode.InvalidNodeChildren
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.EventInvocation)
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol, ByVal eventInvocation As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.RaiseEventStatement, syntax, If(hasErrors, True, eventInvocation.NonNullAndHasErrors()))
			Me._EventSymbol = eventSymbol
			Me._EventInvocation = eventInvocation
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitRaiseEventStatement(Me)
		End Function

		Public Function Update(ByVal eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol, ByVal eventInvocation As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundRaiseEventStatement
			Dim boundRaiseEventStatement As Microsoft.CodeAnalysis.VisualBasic.BoundRaiseEventStatement
			If (CObj(eventSymbol) <> CObj(Me.EventSymbol) OrElse eventInvocation <> Me.EventInvocation) Then
				Dim boundRaiseEventStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundRaiseEventStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundRaiseEventStatement(MyBase.Syntax, eventSymbol, eventInvocation, MyBase.HasErrors)
				boundRaiseEventStatement1.CopyAttributes(Me)
				boundRaiseEventStatement = boundRaiseEventStatement1
			Else
				boundRaiseEventStatement = Me
			End If
			Return boundRaiseEventStatement
		End Function
	End Class
End Namespace