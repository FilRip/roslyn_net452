Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class AttributeSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _target As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax

		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax

		Friend _argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax

		Public ReadOnly Property ArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)(Me._argumentList, 2)
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(Me._name, 1)
			End Get
		End Property

		Public ReadOnly Property Target As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax)(Me._target)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal target As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax(kind, errors, annotations, If(target IsNot Nothing, DirectCast(target.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax), Nothing), DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax), If(argumentList IsNot Nothing, DirectCast(argumentList.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitAttribute(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitAttribute(Me)
		End Sub

		Public Function AddArgumentListArguments(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax
			Return Me.WithArgumentList(If(Me.ArgumentList IsNot Nothing, Me.ArgumentList, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ArgumentList()).AddArguments(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._target
					Exit Select
				Case 1
					syntaxNode = Me._name
					Exit Select
				Case 2
					syntaxNode = Me._argumentList
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim target As SyntaxNode
			Select Case i
				Case 0
					target = Me.Target
					Exit Select
				Case 1
					target = Me.Name
					Exit Select
				Case 2
					target = Me.ArgumentList
					Exit Select
				Case Else
					target = Nothing
					Exit Select
			End Select
			Return target
		End Function

		Public Function Update(ByVal target As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax
			Dim attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax
			If (target <> Me.Target OrElse name <> Me.Name OrElse argumentList <> Me.ArgumentList) Then
				Dim attributeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.Attribute(target, name, argumentList)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				attributeSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, attributeSyntax1, attributeSyntax1.WithAnnotations(annotations))
			Else
				attributeSyntax = Me
			End If
			Return attributeSyntax
		End Function

		Public Function WithArgumentList(ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax
			Return Me.Update(Me.Target, Me.Name, argumentList)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax
			Return Me.Update(Me.Target, name, Me.ArgumentList)
		End Function

		Public Function WithTarget(ByVal target As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax
			Return Me.Update(target, Me.Name, Me.ArgumentList)
		End Function
	End Class
End Namespace