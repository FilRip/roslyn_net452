Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundInstrumentationPayloadRoot
		Inherits BoundExpression
		Private ReadOnly _AnalysisKind As Integer

		Private ReadOnly _IsLValue As Boolean

		Public ReadOnly Property AnalysisKind As Integer
			Get
				Return Me._AnalysisKind
			End Get
		End Property

		Public Overrides ReadOnly Property IsLValue As Boolean
			Get
				Return Me._IsLValue
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal analysisKind As Integer, ByVal isLValue As Boolean, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.InstrumentationPayloadRoot, syntax, type, hasErrors)
			Me._AnalysisKind = analysisKind
			Me._IsLValue = isLValue
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal analysisKind As Integer, ByVal isLValue As Boolean, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.InstrumentationPayloadRoot, syntax, type)
			Me._AnalysisKind = analysisKind
			Me._IsLValue = isLValue
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitInstrumentationPayloadRoot(Me)
		End Function

		Public Function Update(ByVal analysisKind As Integer, ByVal isLValue As Boolean, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundInstrumentationPayloadRoot
			Dim boundInstrumentationPayloadRoot As Microsoft.CodeAnalysis.VisualBasic.BoundInstrumentationPayloadRoot
			If (analysisKind <> Me.AnalysisKind OrElse isLValue <> Me.IsLValue OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundInstrumentationPayloadRoot1 As Microsoft.CodeAnalysis.VisualBasic.BoundInstrumentationPayloadRoot = New Microsoft.CodeAnalysis.VisualBasic.BoundInstrumentationPayloadRoot(MyBase.Syntax, analysisKind, isLValue, type, MyBase.HasErrors)
				boundInstrumentationPayloadRoot1.CopyAttributes(Me)
				boundInstrumentationPayloadRoot = boundInstrumentationPayloadRoot1
			Else
				boundInstrumentationPayloadRoot = Me
			End If
			Return boundInstrumentationPayloadRoot
		End Function
	End Class
End Namespace