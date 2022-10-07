Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class DisableWarningDirectiveTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
		Friend _errorCodes As SyntaxNode

		Public ReadOnly Property DisableKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax)._disableKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property ErrorCodes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
			Get
				Dim identifierNameSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._errorCodes, 3)
				identifierNameSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)(red, MyBase.GetChildIndex(3)))
				Return identifierNameSyntaxes
			End Get
		End Property

		Public Shadows ReadOnly Property HashToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax)._hashToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property WarningKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax)._warningKeyword, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal hashToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal disableKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal warningKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal errorCodes As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax(kind, errors, annotations, hashToken, disableKeyword, warningKeyword, If(errorCodes IsNot Nothing, errorCodes.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitDisableWarningDirectiveTrivia(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitDisableWarningDirectiveTrivia(Me)
		End Sub

		Public Function AddErrorCodes(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DisableWarningDirectiveTriviaSyntax
			Return Me.WithErrorCodes(Me.ErrorCodes.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 3) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._errorCodes
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetHashTokenCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.HashToken
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 3) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._errorCodes, 3)
			End If
			Return red
		End Function

		Public Function Update(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal disableKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal warningKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal errorCodes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DisableWarningDirectiveTriviaSyntax
			Dim disableWarningDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DisableWarningDirectiveTriviaSyntax
			If (hashToken <> Me.HashToken OrElse disableKeyword <> Me.DisableKeyword OrElse warningKeyword <> Me.WarningKeyword OrElse errorCodes <> Me.ErrorCodes) Then
				Dim disableWarningDirectiveTriviaSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.DisableWarningDirectiveTriviaSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.DisableWarningDirectiveTrivia(hashToken, disableKeyword, warningKeyword, errorCodes)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				disableWarningDirectiveTriviaSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, disableWarningDirectiveTriviaSyntax1, disableWarningDirectiveTriviaSyntax1.WithAnnotations(annotations))
			Else
				disableWarningDirectiveTriviaSyntax = Me
			End If
			Return disableWarningDirectiveTriviaSyntax
		End Function

		Public Function WithDisableKeyword(ByVal disableKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DisableWarningDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, disableKeyword, Me.WarningKeyword, Me.ErrorCodes)
		End Function

		Public Function WithErrorCodes(ByVal errorCodes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DisableWarningDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.DisableKeyword, Me.WarningKeyword, errorCodes)
		End Function

		Public Shadows Function WithHashToken(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DisableWarningDirectiveTriviaSyntax
			Return Me.Update(hashToken, Me.DisableKeyword, Me.WarningKeyword, Me.ErrorCodes)
		End Function

		Friend Overrides Function WithHashTokenCore(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Return Me.WithHashToken(hashToken)
		End Function

		Public Function WithWarningKeyword(ByVal warningKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DisableWarningDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.DisableKeyword, warningKeyword, Me.ErrorCodes)
		End Function
	End Class
End Namespace