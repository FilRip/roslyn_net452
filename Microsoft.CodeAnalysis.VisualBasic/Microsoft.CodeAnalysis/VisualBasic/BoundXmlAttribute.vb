Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundXmlAttribute
		Inherits BoundExpression
		Private ReadOnly _Name As BoundExpression

		Private ReadOnly _Value As BoundExpression

		Private ReadOnly _MatchesImport As Boolean

		Private ReadOnly _ObjectCreation As BoundExpression

		Public ReadOnly Property MatchesImport As Boolean
			Get
				Return Me._MatchesImport
			End Get
		End Property

		Public ReadOnly Property Name As BoundExpression
			Get
				Return Me._Name
			End Get
		End Property

		Public ReadOnly Property ObjectCreation As BoundExpression
			Get
				Return Me._ObjectCreation
			End Get
		End Property

		Public ReadOnly Property Value As BoundExpression
			Get
				Return Me._Value
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal name As BoundExpression, ByVal value As BoundExpression, ByVal matchesImport As Boolean, ByVal objectCreation As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.XmlAttribute, syntax, type, If(hasErrors OrElse name.NonNullAndHasErrors() OrElse value.NonNullAndHasErrors(), True, objectCreation.NonNullAndHasErrors()))
			Me._Name = name
			Me._Value = value
			Me._MatchesImport = matchesImport
			Me._ObjectCreation = objectCreation
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitXmlAttribute(Me)
		End Function

		Public Function Update(ByVal name As BoundExpression, ByVal value As BoundExpression, ByVal matchesImport As Boolean, ByVal objectCreation As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundXmlAttribute
			Dim boundXmlAttribute As Microsoft.CodeAnalysis.VisualBasic.BoundXmlAttribute
			If (name <> Me.Name OrElse value <> Me.Value OrElse matchesImport <> Me.MatchesImport OrElse objectCreation <> Me.ObjectCreation OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundXmlAttribute1 As Microsoft.CodeAnalysis.VisualBasic.BoundXmlAttribute = New Microsoft.CodeAnalysis.VisualBasic.BoundXmlAttribute(MyBase.Syntax, name, value, matchesImport, objectCreation, type, MyBase.HasErrors)
				boundXmlAttribute1.CopyAttributes(Me)
				boundXmlAttribute = boundXmlAttribute1
			Else
				boundXmlAttribute = Me
			End If
			Return boundXmlAttribute
		End Function
	End Class
End Namespace