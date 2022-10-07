Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class GlobalNameSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax
		Public ReadOnly Property GlobalKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax)._globalKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal globalKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax(kind, errors, annotations, globalKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitGlobalName(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitGlobalName(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal globalKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GlobalNameSyntax
			Dim globalNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.GlobalNameSyntax
			If (globalKeyword = Me.GlobalKeyword) Then
				globalNameSyntax = Me
			Else
				Dim globalNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.GlobalNameSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.GlobalName(globalKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				globalNameSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, globalNameSyntax1, globalNameSyntax1.WithAnnotations(annotations))
			End If
			Return globalNameSyntax
		End Function

		Public Function WithGlobalKeyword(ByVal globalKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GlobalNameSyntax
			Return Me.Update(globalKeyword)
		End Function
	End Class
End Namespace