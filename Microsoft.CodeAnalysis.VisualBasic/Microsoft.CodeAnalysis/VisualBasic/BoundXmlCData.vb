Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundXmlCData
		Inherits BoundExpression
		Private ReadOnly _Value As BoundLiteral

		Private ReadOnly _ObjectCreation As BoundExpression

		Public ReadOnly Property ObjectCreation As BoundExpression
			Get
				Return Me._ObjectCreation
			End Get
		End Property

		Public ReadOnly Property Value As BoundLiteral
			Get
				Return Me._Value
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal value As BoundLiteral, ByVal objectCreation As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.XmlCData, syntax, type, If(hasErrors OrElse value.NonNullAndHasErrors(), True, objectCreation.NonNullAndHasErrors()))
			Me._Value = value
			Me._ObjectCreation = objectCreation
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitXmlCData(Me)
		End Function

		Public Function Update(ByVal value As BoundLiteral, ByVal objectCreation As BoundExpression, ByVal type As TypeSymbol) As BoundXmlCData
			Dim boundXmlCDatum As BoundXmlCData
			If (value <> Me.Value OrElse objectCreation <> Me.ObjectCreation OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundXmlCDatum1 As BoundXmlCData = New BoundXmlCData(MyBase.Syntax, value, objectCreation, type, MyBase.HasErrors)
				boundXmlCDatum1.CopyAttributes(Me)
				boundXmlCDatum = boundXmlCDatum1
			Else
				boundXmlCDatum = Me
			End If
			Return boundXmlCDatum
		End Function
	End Class
End Namespace