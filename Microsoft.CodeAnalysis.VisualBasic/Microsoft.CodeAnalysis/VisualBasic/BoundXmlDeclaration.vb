Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundXmlDeclaration
		Inherits BoundExpression
		Private ReadOnly _Version As BoundExpression

		Private ReadOnly _Encoding As BoundExpression

		Private ReadOnly _Standalone As BoundExpression

		Private ReadOnly _ObjectCreation As BoundExpression

		Public ReadOnly Property Encoding As BoundExpression
			Get
				Return Me._Encoding
			End Get
		End Property

		Public ReadOnly Property ObjectCreation As BoundExpression
			Get
				Return Me._ObjectCreation
			End Get
		End Property

		Public ReadOnly Property Standalone As BoundExpression
			Get
				Return Me._Standalone
			End Get
		End Property

		Public ReadOnly Property Version As BoundExpression
			Get
				Return Me._Version
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal version As BoundExpression, ByVal encoding As BoundExpression, ByVal standalone As BoundExpression, ByVal objectCreation As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.XmlDeclaration, syntax, type, If(hasErrors OrElse version.NonNullAndHasErrors() OrElse encoding.NonNullAndHasErrors() OrElse standalone.NonNullAndHasErrors(), True, objectCreation.NonNullAndHasErrors()))
			Me._Version = version
			Me._Encoding = encoding
			Me._Standalone = standalone
			Me._ObjectCreation = objectCreation
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitXmlDeclaration(Me)
		End Function

		Public Function Update(ByVal version As BoundExpression, ByVal encoding As BoundExpression, ByVal standalone As BoundExpression, ByVal objectCreation As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundXmlDeclaration
			Dim boundXmlDeclaration As Microsoft.CodeAnalysis.VisualBasic.BoundXmlDeclaration
			If (version <> Me.Version OrElse encoding <> Me.Encoding OrElse standalone <> Me.Standalone OrElse objectCreation <> Me.ObjectCreation OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundXmlDeclaration1 As Microsoft.CodeAnalysis.VisualBasic.BoundXmlDeclaration = New Microsoft.CodeAnalysis.VisualBasic.BoundXmlDeclaration(MyBase.Syntax, version, encoding, standalone, objectCreation, type, MyBase.HasErrors)
				boundXmlDeclaration1.CopyAttributes(Me)
				boundXmlDeclaration = boundXmlDeclaration1
			Else
				boundXmlDeclaration = Me
			End If
			Return boundXmlDeclaration
		End Function
	End Class
End Namespace