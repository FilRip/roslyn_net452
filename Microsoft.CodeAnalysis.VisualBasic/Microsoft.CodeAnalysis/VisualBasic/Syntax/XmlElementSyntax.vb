Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlElementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
		Friend _startTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax

		Friend _content As SyntaxNode

		Friend _endTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax

		Public ReadOnly Property Content As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(MyBase.GetRed(Me._content, 1))
			End Get
		End Property

		Public ReadOnly Property EndTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax)(Me._endTag, 2)
			End Get
		End Property

		Public ReadOnly Property StartTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax)(Me._startTag)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal startTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax, ByVal content As SyntaxNode, ByVal endTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax(kind, errors, annotations, DirectCast(startTag.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax), If(content IsNot Nothing, content.Green, Nothing), DirectCast(endTag.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlElement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlElement(Me)
		End Sub

		Public Function AddContent(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementSyntax
			Return Me.WithContent(Me.Content.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._startTag
					Exit Select
				Case 1
					syntaxNode = Me._content
					Exit Select
				Case 2
					syntaxNode = Me._endTag
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim startTag As SyntaxNode
			Select Case i
				Case 0
					startTag = Me.StartTag
					Exit Select
				Case 1
					startTag = MyBase.GetRed(Me._content, 1)
					Exit Select
				Case 2
					startTag = Me.EndTag
					Exit Select
				Case Else
					startTag = Nothing
					Exit Select
			End Select
			Return startTag
		End Function

		Public Function Update(ByVal startTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax, ByVal content As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax), ByVal endTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementSyntax
			Dim xmlElementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementSyntax
			If (startTag <> Me.StartTag OrElse content <> Me.Content OrElse endTag <> Me.EndTag) Then
				Dim xmlElementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlElement(startTag, content, endTag)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlElementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlElementSyntax1, xmlElementSyntax1.WithAnnotations(annotations))
			Else
				xmlElementSyntax = Me
			End If
			Return xmlElementSyntax
		End Function

		Public Function WithContent(ByVal content As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementSyntax
			Return Me.Update(Me.StartTag, content, Me.EndTag)
		End Function

		Public Function WithEndTag(ByVal endTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementSyntax
			Return Me.Update(Me.StartTag, Me.Content, endTag)
		End Function

		Public Function WithStartTag(ByVal startTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementSyntax
			Return Me.Update(startTag, Me.Content, Me.EndTag)
		End Function
	End Class
End Namespace