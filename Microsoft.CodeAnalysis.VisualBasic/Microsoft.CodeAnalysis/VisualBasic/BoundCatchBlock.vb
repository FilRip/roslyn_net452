Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundCatchBlock
		Inherits BoundNode
		Private ReadOnly _LocalOpt As LocalSymbol

		Private ReadOnly _ExceptionSourceOpt As BoundExpression

		Private ReadOnly _ErrorLineNumberOpt As BoundExpression

		Private ReadOnly _ExceptionFilterOpt As BoundExpression

		Private ReadOnly _Body As BoundBlock

		Private ReadOnly _IsSynthesizedAsyncCatchAll As Boolean

		Public ReadOnly Property Body As BoundBlock
			Get
				Return Me._Body
			End Get
		End Property

		Public ReadOnly Property ErrorLineNumberOpt As BoundExpression
			Get
				Return Me._ErrorLineNumberOpt
			End Get
		End Property

		Public ReadOnly Property ExceptionFilterOpt As BoundExpression
			Get
				Return Me._ExceptionFilterOpt
			End Get
		End Property

		Public ReadOnly Property ExceptionSourceOpt As BoundExpression
			Get
				Return Me._ExceptionSourceOpt
			End Get
		End Property

		Public ReadOnly Property IsSynthesizedAsyncCatchAll As Boolean
			Get
				Return Me._IsSynthesizedAsyncCatchAll
			End Get
		End Property

		Public ReadOnly Property LocalOpt As LocalSymbol
			Get
				Return Me._LocalOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal localOpt As LocalSymbol, ByVal exceptionSourceOpt As BoundExpression, ByVal errorLineNumberOpt As BoundExpression, ByVal exceptionFilterOpt As BoundExpression, ByVal body As BoundBlock, ByVal isSynthesizedAsyncCatchAll As Boolean, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.CatchBlock, syntax, If(hasErrors OrElse exceptionSourceOpt.NonNullAndHasErrors() OrElse errorLineNumberOpt.NonNullAndHasErrors() OrElse exceptionFilterOpt.NonNullAndHasErrors(), True, body.NonNullAndHasErrors()))
			Me._LocalOpt = localOpt
			Me._ExceptionSourceOpt = exceptionSourceOpt
			Me._ErrorLineNumberOpt = errorLineNumberOpt
			Me._ExceptionFilterOpt = exceptionFilterOpt
			Me._Body = body
			Me._IsSynthesizedAsyncCatchAll = isSynthesizedAsyncCatchAll
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitCatchBlock(Me)
		End Function

		Public Function Update(ByVal localOpt As LocalSymbol, ByVal exceptionSourceOpt As BoundExpression, ByVal errorLineNumberOpt As BoundExpression, ByVal exceptionFilterOpt As BoundExpression, ByVal body As BoundBlock, ByVal isSynthesizedAsyncCatchAll As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundCatchBlock
			Dim boundCatchBlock As Microsoft.CodeAnalysis.VisualBasic.BoundCatchBlock
			If (CObj(localOpt) <> CObj(Me.LocalOpt) OrElse exceptionSourceOpt <> Me.ExceptionSourceOpt OrElse errorLineNumberOpt <> Me.ErrorLineNumberOpt OrElse exceptionFilterOpt <> Me.ExceptionFilterOpt OrElse body <> Me.Body OrElse isSynthesizedAsyncCatchAll <> Me.IsSynthesizedAsyncCatchAll) Then
				Dim boundCatchBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundCatchBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundCatchBlock(MyBase.Syntax, localOpt, exceptionSourceOpt, errorLineNumberOpt, exceptionFilterOpt, body, isSynthesizedAsyncCatchAll, MyBase.HasErrors)
				boundCatchBlock1.CopyAttributes(Me)
				boundCatchBlock = boundCatchBlock1
			Else
				boundCatchBlock = Me
			End If
			Return boundCatchBlock
		End Function
	End Class
End Namespace