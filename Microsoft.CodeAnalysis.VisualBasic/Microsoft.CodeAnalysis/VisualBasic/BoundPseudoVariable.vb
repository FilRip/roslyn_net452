Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundPseudoVariable
		Inherits BoundExpression
		Private ReadOnly _LocalSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol

		Private ReadOnly _IsLValue As Boolean

		Private ReadOnly _EmitExpressions As PseudoVariableExpressions

		Public ReadOnly Property EmitExpressions As PseudoVariableExpressions
			Get
				Return Me._EmitExpressions
			End Get
		End Property

		Public Overrides ReadOnly Property IsLValue As Boolean
			Get
				Return Me._IsLValue
			End Get
		End Property

		Public ReadOnly Property LocalSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol
			Get
				Return Me._LocalSymbol
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal isLValue As Boolean, ByVal emitExpressions As PseudoVariableExpressions, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.PseudoVariable, syntax, type, hasErrors)
			Me._LocalSymbol = localSymbol
			Me._IsLValue = isLValue
			Me._EmitExpressions = emitExpressions
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal isLValue As Boolean, ByVal emitExpressions As PseudoVariableExpressions, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.PseudoVariable, syntax, type)
			Me._LocalSymbol = localSymbol
			Me._IsLValue = isLValue
			Me._EmitExpressions = emitExpressions
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitPseudoVariable(Me)
		End Function

		Protected Overrides Function MakeRValueImpl() As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			boundExpression = If(Me._IsLValue, Me.Update(Me._LocalSymbol, False, Me._EmitExpressions, MyBase.Type), Me)
			Return boundExpression
		End Function

		Public Function Update(ByVal localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal isLValue As Boolean, ByVal emitExpressions As PseudoVariableExpressions, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundPseudoVariable
			Dim boundPseudoVariable As Microsoft.CodeAnalysis.VisualBasic.BoundPseudoVariable
			If (CObj(localSymbol) <> CObj(Me.LocalSymbol) OrElse isLValue <> Me.IsLValue OrElse emitExpressions <> Me.EmitExpressions OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundPseudoVariable1 As Microsoft.CodeAnalysis.VisualBasic.BoundPseudoVariable = New Microsoft.CodeAnalysis.VisualBasic.BoundPseudoVariable(MyBase.Syntax, localSymbol, isLValue, emitExpressions, type, MyBase.HasErrors)
				boundPseudoVariable1.CopyAttributes(Me)
				boundPseudoVariable = boundPseudoVariable1
			Else
				boundPseudoVariable = Me
			End If
			Return boundPseudoVariable
		End Function
	End Class
End Namespace