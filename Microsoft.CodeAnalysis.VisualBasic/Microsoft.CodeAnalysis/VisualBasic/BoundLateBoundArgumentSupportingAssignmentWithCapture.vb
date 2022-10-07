Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundLateBoundArgumentSupportingAssignmentWithCapture
		Inherits BoundExpression
		Private ReadOnly _OriginalArgument As BoundExpression

		Private ReadOnly _LocalSymbol As SynthesizedLocal

		Public ReadOnly Property LocalSymbol As SynthesizedLocal
			Get
				Return Me._LocalSymbol
			End Get
		End Property

		Public ReadOnly Property OriginalArgument As BoundExpression
			Get
				Return Me._OriginalArgument
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal originalArgument As BoundExpression, ByVal localSymbol As SynthesizedLocal, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.LateBoundArgumentSupportingAssignmentWithCapture, syntax, type, If(hasErrors, True, originalArgument.NonNullAndHasErrors()))
			Me._OriginalArgument = originalArgument
			Me._LocalSymbol = localSymbol
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitLateBoundArgumentSupportingAssignmentWithCapture(Me)
		End Function

		Public Function Update(ByVal originalArgument As BoundExpression, ByVal localSymbol As SynthesizedLocal, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundLateBoundArgumentSupportingAssignmentWithCapture
			Dim boundLateBoundArgumentSupportingAssignmentWithCapture As Microsoft.CodeAnalysis.VisualBasic.BoundLateBoundArgumentSupportingAssignmentWithCapture
			If (originalArgument <> Me.OriginalArgument OrElse CObj(localSymbol) <> CObj(Me.LocalSymbol) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundLateBoundArgumentSupportingAssignmentWithCapture1 As Microsoft.CodeAnalysis.VisualBasic.BoundLateBoundArgumentSupportingAssignmentWithCapture = New Microsoft.CodeAnalysis.VisualBasic.BoundLateBoundArgumentSupportingAssignmentWithCapture(MyBase.Syntax, originalArgument, localSymbol, type, MyBase.HasErrors)
				boundLateBoundArgumentSupportingAssignmentWithCapture1.CopyAttributes(Me)
				boundLateBoundArgumentSupportingAssignmentWithCapture = boundLateBoundArgumentSupportingAssignmentWithCapture1
			Else
				boundLateBoundArgumentSupportingAssignmentWithCapture = Me
			End If
			Return boundLateBoundArgumentSupportingAssignmentWithCapture
		End Function
	End Class
End Namespace