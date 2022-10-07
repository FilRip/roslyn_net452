Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlDeclarationSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _version As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax

		Friend _encoding As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax

		Friend _standalone As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax

		Public ReadOnly Property Encoding As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax)(Me._encoding, 3)
			End Get
		End Property

		Public ReadOnly Property LessThanQuestionToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax)._lessThanQuestionToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property QuestionGreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax)._questionGreaterThanToken, Me.GetChildPosition(5), MyBase.GetChildIndex(5))
			End Get
		End Property

		Public ReadOnly Property Standalone As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax)(Me._standalone, 4)
			End Get
		End Property

		Public ReadOnly Property Version As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax)(Me._version, 2)
			End Get
		End Property

		Public ReadOnly Property XmlKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax)._xmlKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lessThanQuestionToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal xmlKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal version As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax, ByVal encoding As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax, ByVal standalone As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax, ByVal questionGreaterThanToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax(kind, errors, annotations, lessThanQuestionToken, xmlKeyword, DirectCast(version.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax), If(encoding IsNot Nothing, DirectCast(encoding.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax), Nothing), If(standalone IsNot Nothing, DirectCast(standalone.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax), Nothing), questionGreaterThanToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlDeclaration(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlDeclaration(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 2
					syntaxNode = Me._version
					Exit Select
				Case 3
					syntaxNode = Me._encoding
					Exit Select
				Case 4
					syntaxNode = Me._standalone
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim version As SyntaxNode
			Select Case i
				Case 2
					version = Me.Version
					Exit Select
				Case 3
					version = Me.Encoding
					Exit Select
				Case 4
					version = Me.Standalone
					Exit Select
				Case Else
					version = Nothing
					Exit Select
			End Select
			Return version
		End Function

		Public Function Update(ByVal lessThanQuestionToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal xmlKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal version As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax, ByVal encoding As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax, ByVal standalone As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax, ByVal questionGreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax
			Dim xmlDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax
			If (lessThanQuestionToken <> Me.LessThanQuestionToken OrElse xmlKeyword <> Me.XmlKeyword OrElse version <> Me.Version OrElse encoding <> Me.Encoding OrElse standalone <> Me.Standalone OrElse questionGreaterThanToken <> Me.QuestionGreaterThanToken) Then
				Dim xmlDeclarationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlDeclaration(lessThanQuestionToken, xmlKeyword, version, encoding, standalone, questionGreaterThanToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlDeclarationSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlDeclarationSyntax1, xmlDeclarationSyntax1.WithAnnotations(annotations))
			Else
				xmlDeclarationSyntax = Me
			End If
			Return xmlDeclarationSyntax
		End Function

		Public Function WithEncoding(ByVal encoding As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax
			Return Me.Update(Me.LessThanQuestionToken, Me.XmlKeyword, Me.Version, encoding, Me.Standalone, Me.QuestionGreaterThanToken)
		End Function

		Public Function WithLessThanQuestionToken(ByVal lessThanQuestionToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax
			Return Me.Update(lessThanQuestionToken, Me.XmlKeyword, Me.Version, Me.Encoding, Me.Standalone, Me.QuestionGreaterThanToken)
		End Function

		Public Function WithQuestionGreaterThanToken(ByVal questionGreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax
			Return Me.Update(Me.LessThanQuestionToken, Me.XmlKeyword, Me.Version, Me.Encoding, Me.Standalone, questionGreaterThanToken)
		End Function

		Public Function WithStandalone(ByVal standalone As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax
			Return Me.Update(Me.LessThanQuestionToken, Me.XmlKeyword, Me.Version, Me.Encoding, standalone, Me.QuestionGreaterThanToken)
		End Function

		Public Function WithVersion(ByVal version As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax
			Return Me.Update(Me.LessThanQuestionToken, Me.XmlKeyword, version, Me.Encoding, Me.Standalone, Me.QuestionGreaterThanToken)
		End Function

		Public Function WithXmlKeyword(ByVal xmlKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax
			Return Me.Update(Me.LessThanQuestionToken, xmlKeyword, Me.Version, Me.Encoding, Me.Standalone, Me.QuestionGreaterThanToken)
		End Function
	End Class
End Namespace