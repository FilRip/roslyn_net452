Imports Microsoft.CodeAnalysis
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class ForLoopVerification
		Public Sub New()
			MyBase.New()
		End Sub

		Friend Shared Function ReferencedSymbol(ByVal expression As BoundExpression) As Symbol
			Dim method As Symbol
			Dim kind As BoundKind = expression.Kind
			If (kind <= BoundKind.FieldAccess) Then
				If (kind <= BoundKind.ArrayAccess) Then
					If (kind = BoundKind.Parenthesized) Then
						method = ForLoopVerification.ReferencedSymbol(DirectCast(expression, BoundParenthesized).Expression)
					Else
						If (kind <> BoundKind.ArrayAccess) Then
							method = Nothing
							Return method
						End If
						method = ForLoopVerification.ReferencedSymbol(DirectCast(expression, BoundArrayAccess).Expression)
					End If
				ElseIf (kind = BoundKind.[Call]) Then
					method = DirectCast(expression, BoundCall).Method
				Else
					If (kind <> BoundKind.FieldAccess) Then
						method = Nothing
						Return method
					End If
					method = DirectCast(expression, BoundFieldAccess).FieldSymbol
				End If
			ElseIf (kind <= BoundKind.Local) Then
				If (kind = BoundKind.PropertyAccess) Then
					method = DirectCast(expression, BoundPropertyAccess).PropertySymbol
				Else
					If (kind <> BoundKind.Local) Then
						method = Nothing
						Return method
					End If
					method = DirectCast(expression, BoundLocal).LocalSymbol
				End If
			ElseIf (kind = BoundKind.Parameter) Then
				method = DirectCast(expression, BoundParameter).ParameterSymbol
			Else
				If (kind <> BoundKind.RangeVariable) Then
					method = Nothing
					Return method
				End If
				method = DirectCast(expression, BoundRangeVariable).RangeVariable
			End If
			Return method
		End Function

		Public Shared Sub VerifyForLoops(ByVal block As BoundBlock, ByVal diagnostics As DiagnosticBag)
			Try
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = (New ForLoopVerification.ForLoopVerificationWalker(diagnostics)).Visit(block)
			Catch cancelledByStackGuardException As BoundTreeVisitor.CancelledByStackGuardException
				ProjectData.SetProjectError(cancelledByStackGuardException)
				cancelledByStackGuardException.AddAnError(diagnostics)
				ProjectData.ClearProjectError()
			End Try
		End Sub

		Private NotInheritable Class ForLoopVerificationWalker
			Inherits BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
			Private ReadOnly _diagnostics As DiagnosticBag

			Private ReadOnly _controlVariables As Stack(Of BoundExpression)

			Public Sub New(ByVal diagnostics As DiagnosticBag)
				MyBase.New()
				Me._diagnostics = diagnostics
				Me._controlVariables = New Stack(Of BoundExpression)()
			End Sub

			Private Sub PostVisitForAndForEachStatement(ByVal boundForStatement As Microsoft.CodeAnalysis.VisualBasic.BoundForStatement)
				If (Not boundForStatement.NextVariablesOpt.IsDefault) Then
					If (boundForStatement.NextVariablesOpt.IsEmpty) Then
						Me._controlVariables.Pop()
						Return
					End If
					Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Enumerator = boundForStatement.NextVariablesOpt.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = enumerator.Current
						Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me._controlVariables.Pop()
						If (boundExpression.HasErrors OrElse current.HasErrors OrElse Not (ForLoopVerification.ReferencedSymbol(current) <> ForLoopVerification.ReferencedSymbol(boundExpression))) Then
							Continue While
						End If
						Me._diagnostics.Add(ERRID.ERR_NextForMismatch1, current.Syntax.GetLocation(), New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(ForLoopVerification.ReferencedSymbol(boundExpression)) })
					End While
				End If
			End Sub

			Private Sub PreVisitForAndForEachStatement(ByVal boundForStatement As Microsoft.CodeAnalysis.VisualBasic.BoundForStatement)
				Dim enumerator As Stack(Of BoundExpression).Enumerator = New Stack(Of BoundExpression).Enumerator()
				Dim controlVariable As BoundExpression = boundForStatement.ControlVariable
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = ForLoopVerification.ReferencedSymbol(controlVariable)
				If (symbol IsNot Nothing) Then
					Try
						enumerator = Me._controlVariables.GetEnumerator()
						While enumerator.MoveNext()
							If (ForLoopVerification.ReferencedSymbol(enumerator.Current) <> symbol) Then
								Continue While
							End If
							Me._diagnostics.Add(ERRID.ERR_ForIndexInUse1, controlVariable.Syntax.GetLocation(), New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(symbol) })
							Me._controlVariables.Push(controlVariable)
							Return
						End While
					Finally
						DirectCast(enumerator, IDisposable).Dispose()
					End Try
				End If
				Me._controlVariables.Push(controlVariable)
			End Sub

			Public Overrides Function VisitForEachStatement(ByVal node As BoundForEachStatement) As BoundNode
				Me.PreVisitForAndForEachStatement(node)
				MyBase.VisitForEachStatement(node)
				Me.PostVisitForAndForEachStatement(node)
				Return Nothing
			End Function

			Public Overrides Function VisitForToStatement(ByVal node As BoundForToStatement) As BoundNode
				Me.PreVisitForAndForEachStatement(node)
				MyBase.VisitForToStatement(node)
				Me.PostVisitForAndForEachStatement(node)
				Return Nothing
			End Function
		End Class
	End Class
End Namespace