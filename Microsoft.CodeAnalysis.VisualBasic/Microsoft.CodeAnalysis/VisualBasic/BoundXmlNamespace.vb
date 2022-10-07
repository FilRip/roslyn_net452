Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundXmlNamespace
		Inherits BoundExpression
		Private ReadOnly _XmlNamespace As BoundExpression

		Private ReadOnly _ObjectCreation As BoundExpression

		Public ReadOnly Property ObjectCreation As BoundExpression
			Get
				Return Me._ObjectCreation
			End Get
		End Property

		Public ReadOnly Property XmlNamespace As BoundExpression
			Get
				Return Me._XmlNamespace
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal xmlNamespace As BoundExpression, ByVal objectCreation As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.XmlNamespace, syntax, type, If(hasErrors OrElse xmlNamespace.NonNullAndHasErrors(), True, objectCreation.NonNullAndHasErrors()))
			Me._XmlNamespace = xmlNamespace
			Me._ObjectCreation = objectCreation
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitXmlNamespace(Me)
		End Function

		Public Function Update(ByVal xmlNamespace As BoundExpression, ByVal objectCreation As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundXmlNamespace
			Dim boundXmlNamespace As Microsoft.CodeAnalysis.VisualBasic.BoundXmlNamespace
			If (xmlNamespace <> Me.XmlNamespace OrElse objectCreation <> Me.ObjectCreation OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundXmlNamespace1 As Microsoft.CodeAnalysis.VisualBasic.BoundXmlNamespace = New Microsoft.CodeAnalysis.VisualBasic.BoundXmlNamespace(MyBase.Syntax, xmlNamespace, objectCreation, type, MyBase.HasErrors)
				boundXmlNamespace1.CopyAttributes(Me)
				boundXmlNamespace = boundXmlNamespace1
			Else
				boundXmlNamespace = Me
			End If
			Return boundXmlNamespace
		End Function
	End Class
End Namespace