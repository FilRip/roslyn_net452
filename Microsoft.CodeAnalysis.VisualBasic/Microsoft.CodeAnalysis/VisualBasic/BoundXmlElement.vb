Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundXmlElement
		Inherits BoundExpression
		Private ReadOnly _Argument As BoundExpression

		Private ReadOnly _ChildNodes As ImmutableArray(Of BoundExpression)

		Private ReadOnly _RewriterInfo As BoundXmlContainerRewriterInfo

		Public ReadOnly Property Argument As BoundExpression
			Get
				Return Me._Argument
			End Get
		End Property

		Public ReadOnly Property ChildNodes As ImmutableArray(Of BoundExpression)
			Get
				Return Me._ChildNodes
			End Get
		End Property

		Public ReadOnly Property RewriterInfo As BoundXmlContainerRewriterInfo
			Get
				Return Me._RewriterInfo
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal argument As BoundExpression, ByVal childNodes As ImmutableArray(Of BoundExpression), ByVal rewriterInfo As BoundXmlContainerRewriterInfo, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.XmlElement, syntax, type, If(hasErrors OrElse argument.NonNullAndHasErrors(), True, childNodes.NonNullAndHasErrors()))
			Me._Argument = argument
			Me._ChildNodes = childNodes
			Me._RewriterInfo = rewriterInfo
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitXmlElement(Me)
		End Function

		Public Function Update(ByVal argument As BoundExpression, ByVal childNodes As ImmutableArray(Of BoundExpression), ByVal rewriterInfo As BoundXmlContainerRewriterInfo, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundXmlElement
			Dim boundXmlElement As Microsoft.CodeAnalysis.VisualBasic.BoundXmlElement
			If (argument <> Me.Argument OrElse childNodes <> Me.ChildNodes OrElse rewriterInfo <> Me.RewriterInfo OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundXmlElement1 As Microsoft.CodeAnalysis.VisualBasic.BoundXmlElement = New Microsoft.CodeAnalysis.VisualBasic.BoundXmlElement(MyBase.Syntax, argument, childNodes, rewriterInfo, type, MyBase.HasErrors)
				boundXmlElement1.CopyAttributes(Me)
				boundXmlElement = boundXmlElement1
			Else
				boundXmlElement = Me
			End If
			Return boundXmlElement
		End Function
	End Class
End Namespace