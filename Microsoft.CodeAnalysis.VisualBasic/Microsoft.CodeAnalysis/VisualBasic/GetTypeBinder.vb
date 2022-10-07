Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class GetTypeBinder
		Inherits Binder
		Private ReadOnly _allowedMap As Dictionary(Of GenericNameSyntax, Boolean)

		Private ReadOnly _isTypeExpressionOpen As Boolean

		Friend ReadOnly Property IsTypeExpressionOpen As Boolean
			Get
				Return Me._isTypeExpressionOpen
			End Get
		End Property

		Friend Sub New(ByVal typeExpression As ExpressionSyntax, ByVal containingBinder As Binder)
			MyBase.New(containingBinder)
			GetTypeBinder.OpenTypeVisitor.Visit(typeExpression, Me._allowedMap, Me._isTypeExpressionOpen)
		End Sub

		Public Overrides Function IsUnboundTypeAllowed(ByVal Syntax As GenericNameSyntax) As Boolean
			Dim flag As Boolean
			If (Me._allowedMap IsNot Nothing AndAlso Me._allowedMap.TryGetValue(Syntax, flag)) Then
				Return flag
			End If
			Return False
		End Function

		Private Class OpenTypeVisitor
			Inherits VisualBasicSyntaxVisitor
			Private _allowedMap As Dictionary(Of GenericNameSyntax, Boolean)

			Private _seenConstructed As Boolean

			Private _seenGeneric As Boolean

			Public Sub New()
				MyBase.New()
				Me._allowedMap = Nothing
				Me._seenConstructed = False
				Me._seenGeneric = False
			End Sub

			Public Shared Sub Visit(ByVal typeSyntax As ExpressionSyntax, <Out> ByRef allowedMap As Dictionary(Of GenericNameSyntax, Boolean), <Out> ByVal isOpenType As Boolean)
				Dim openTypeVisitor As GetTypeBinder.OpenTypeVisitor = New GetTypeBinder.OpenTypeVisitor()
				openTypeVisitor.Visit(typeSyntax)
				allowedMap = openTypeVisitor._allowedMap
				isOpenType = If(Not openTypeVisitor._seenGeneric, False, Not openTypeVisitor._seenConstructed)
			End Sub

			Public Overrides Sub VisitArrayType(ByVal node As ArrayTypeSyntax)
				Me._seenConstructed = True
				Me.Visit(node.ElementType)
			End Sub

			Public Overrides Sub VisitGenericName(ByVal node As GenericNameSyntax)
				Me._seenGeneric = True
				Dim arguments As SeparatedSyntaxList(Of TypeSyntax) = node.TypeArgumentList.Arguments
				If (DirectCast(arguments, IEnumerable(Of VisualBasicSyntaxNode)).AllAreMissingIdentifierName()) Then
					If (Me._allowedMap Is Nothing) Then
						Me._allowedMap = New Dictionary(Of GenericNameSyntax, Boolean)()
					End If
					Me._allowedMap(node) = Not Me._seenConstructed
					Return
				End If
				Me._seenConstructed = True
				Dim enumerator As SeparatedSyntaxList(Of TypeSyntax).Enumerator = arguments.GetEnumerator()
				While enumerator.MoveNext()
					Me.Visit(enumerator.Current)
				End While
			End Sub

			Public Overrides Sub VisitNullableType(ByVal node As NullableTypeSyntax)
				Me._seenConstructed = True
				Me.Visit(node.ElementType)
			End Sub

			Public Overrides Sub VisitQualifiedName(ByVal node As QualifiedNameSyntax)
				Dim flag As Boolean = Me._seenConstructed
				Me.Visit(node.Right)
				Dim flag1 As Boolean = Me._seenConstructed
				Me.Visit(node.Left)
				If (Not flag AndAlso Not flag1 AndAlso Me._seenConstructed) Then
					Me.Visit(node.Right)
				End If
			End Sub
		End Class
	End Class
End Namespace