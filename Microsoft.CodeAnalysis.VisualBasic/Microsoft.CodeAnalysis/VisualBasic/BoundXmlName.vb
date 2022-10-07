Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundXmlName
		Inherits BoundExpression
		Private ReadOnly _XmlNamespace As BoundExpression

		Private ReadOnly _LocalName As BoundExpression

		Private ReadOnly _ObjectCreation As BoundExpression

		Public ReadOnly Property LocalName As BoundExpression
			Get
				Return Me._LocalName
			End Get
		End Property

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

		Public Sub New(ByVal syntax As SyntaxNode, ByVal xmlNamespace As BoundExpression, ByVal localName As BoundExpression, ByVal objectCreation As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.XmlName, syntax, type, If(hasErrors OrElse xmlNamespace.NonNullAndHasErrors() OrElse localName.NonNullAndHasErrors(), True, objectCreation.NonNullAndHasErrors()))
			Me._XmlNamespace = xmlNamespace
			Me._LocalName = localName
			Me._ObjectCreation = objectCreation
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitXmlName(Me)
		End Function

		Public Function Update(ByVal xmlNamespace As BoundExpression, ByVal localName As BoundExpression, ByVal objectCreation As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundXmlName
			Dim boundXmlName As Microsoft.CodeAnalysis.VisualBasic.BoundXmlName
			If (xmlNamespace <> Me.XmlNamespace OrElse localName <> Me.LocalName OrElse objectCreation <> Me.ObjectCreation OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundXmlName1 As Microsoft.CodeAnalysis.VisualBasic.BoundXmlName = New Microsoft.CodeAnalysis.VisualBasic.BoundXmlName(MyBase.Syntax, xmlNamespace, localName, objectCreation, type, MyBase.HasErrors)
				boundXmlName1.CopyAttributes(Me)
				boundXmlName = boundXmlName1
			Else
				boundXmlName = Me
			End If
			Return boundXmlName
		End Function
	End Class
End Namespace