Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class CrefReferenceSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax

		Friend _signature As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax

		Friend _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax

		Public ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)(Me._asClause, 2)
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(Me._name)
			End Get
		End Property

		Public ReadOnly Property Signature As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax)(Me._signature, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax, ByVal signature As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax(kind, errors, annotations, DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax), If(signature IsNot Nothing, DirectCast(signature.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax), Nothing), If(asClause IsNot Nothing, DirectCast(asClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitCrefReference(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitCrefReference(Me)
		End Sub

		Public Function AddSignatureArgumentTypes(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax
			Return Me.WithSignature(If(Me.Signature IsNot Nothing, Me.Signature, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CrefSignature(New Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax(-1) {})).AddArgumentTypes(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._name
					Exit Select
				Case 1
					syntaxNode = Me._signature
					Exit Select
				Case 2
					syntaxNode = Me._asClause
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim name As SyntaxNode
			Select Case i
				Case 0
					name = Me.Name
					Exit Select
				Case 1
					name = Me.Signature
					Exit Select
				Case 2
					name = Me.AsClause
					Exit Select
				Case Else
					name = Nothing
					Exit Select
			End Select
			Return name
		End Function

		Public Function Update(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax, ByVal signature As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax
			Dim crefReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax
			If (name <> Me.Name OrElse signature <> Me.Signature OrElse asClause <> Me.AsClause) Then
				Dim crefReferenceSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CrefReference(name, signature, asClause)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				crefReferenceSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, crefReferenceSyntax1, crefReferenceSyntax1.WithAnnotations(annotations))
			Else
				crefReferenceSyntax = Me
			End If
			Return crefReferenceSyntax
		End Function

		Public Function WithAsClause(ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax
			Return Me.Update(Me.Name, Me.Signature, asClause)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax
			Return Me.Update(name, Me.Signature, Me.AsClause)
		End Function

		Public Function WithSignature(ByVal signature As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax
			Return Me.Update(Me.Name, signature, Me.AsClause)
		End Function
	End Class
End Namespace