Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundXmlDocument
		Inherits BoundExpression
		Private ReadOnly _Declaration As BoundExpression

		Private ReadOnly _ChildNodes As ImmutableArray(Of BoundExpression)

		Private ReadOnly _RewriterInfo As BoundXmlContainerRewriterInfo

		Public ReadOnly Property ChildNodes As ImmutableArray(Of BoundExpression)
			Get
				Return Me._ChildNodes
			End Get
		End Property

		Public ReadOnly Property Declaration As BoundExpression
			Get
				Return Me._Declaration
			End Get
		End Property

		Public ReadOnly Property RewriterInfo As BoundXmlContainerRewriterInfo
			Get
				Return Me._RewriterInfo
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal declaration As BoundExpression, ByVal childNodes As ImmutableArray(Of BoundExpression), ByVal rewriterInfo As BoundXmlContainerRewriterInfo, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.XmlDocument, syntax, type, If(hasErrors OrElse declaration.NonNullAndHasErrors(), True, childNodes.NonNullAndHasErrors()))
			Me._Declaration = declaration
			Me._ChildNodes = childNodes
			Me._RewriterInfo = rewriterInfo
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitXmlDocument(Me)
		End Function

		Public Function Update(ByVal declaration As BoundExpression, ByVal childNodes As ImmutableArray(Of BoundExpression), ByVal rewriterInfo As BoundXmlContainerRewriterInfo, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundXmlDocument
			Dim boundXmlDocument As Microsoft.CodeAnalysis.VisualBasic.BoundXmlDocument
			If (declaration <> Me.Declaration OrElse childNodes <> Me.ChildNodes OrElse rewriterInfo <> Me.RewriterInfo OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundXmlDocument1 As Microsoft.CodeAnalysis.VisualBasic.BoundXmlDocument = New Microsoft.CodeAnalysis.VisualBasic.BoundXmlDocument(MyBase.Syntax, declaration, childNodes, rewriterInfo, type, MyBase.HasErrors)
				boundXmlDocument1.CopyAttributes(Me)
				boundXmlDocument = boundXmlDocument1
			Else
				boundXmlDocument = Me
			End If
			Return boundXmlDocument
		End Function
	End Class
End Namespace