Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class MethodBodyBinder
		Inherits SubOrFunctionBodyBinder
		Private ReadOnly _functionValue As LocalSymbol

		Public Overrides ReadOnly Property IsInQuery As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property SuppressCallerInfo As Boolean
			Get
				If (Not DirectCast(Me.ContainingMember, MethodSymbol).IsImplicitlyDeclared) Then
					Return False
				End If
				Return TypeOf Me.ContainingMember Is SynthesizedMyGroupCollectionPropertyAccessorSymbol
			End Get
		End Property

		Public Sub New(ByVal methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal root As SyntaxNode, ByVal containingBinder As Binder)
			MyBase.New(methodSymbol, root, containingBinder)
			Me._functionValue = Me.CreateFunctionValueLocal(methodSymbol, root)
			If (Me._functionValue IsNot Nothing AndAlso Not methodSymbol.IsUserDefinedOperator()) Then
				Dim name As String = Me._functionValue.Name
				If (Not [String].IsNullOrEmpty(name)) Then
					Me._parameterMap(name) = Me._functionValue
				End If
			End If
		End Sub

		Private Function CreateFunctionValueLocal(ByVal methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal root As SyntaxNode) As LocalSymbol
			Dim synthesizedLocal As LocalSymbol
			Dim unknownResultType As TypeSymbol
			Dim methodBlockBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax = TryCast(root, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax)
			If (methodBlockBaseSyntax IsNot Nothing) Then
				Select Case methodBlockBaseSyntax.Kind()
					Case SyntaxKind.FunctionBlock
						Dim subOrFunctionStatement As MethodStatementSyntax = DirectCast(methodBlockBaseSyntax, MethodBlockSyntax).SubOrFunctionStatement
						Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = methodSymbol
						Dim identifier As Microsoft.CodeAnalysis.SyntaxToken = subOrFunctionStatement.Identifier
						If (methodSymbol.ReturnType.IsVoidType()) Then
							unknownResultType = ErrorTypeSymbol.UnknownResultType
						Else
							unknownResultType = methodSymbol.ReturnType
						End If
						synthesizedLocal = LocalSymbol.Create(methodSymbol1, Me, identifier, LocalDeclarationKind.FunctionValue, unknownResultType)
						Exit Select
					Case SyntaxKind.ConstructorBlock
					Case SyntaxKind.SetAccessorBlock
					Label0:
						synthesizedLocal = Nothing
						Exit Select
					Case SyntaxKind.OperatorBlock
						synthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(methodSymbol, methodSymbol.ReturnType, SynthesizedLocalKind.FunctionReturnValue, DirectCast(methodBlockBaseSyntax, OperatorBlockSyntax).BlockStatement, False)
						Exit Select
					Case SyntaxKind.GetAccessorBlock
						If (methodBlockBaseSyntax.Parent Is Nothing OrElse methodBlockBaseSyntax.Parent.Kind() <> SyntaxKind.PropertyBlock) Then
							GoTo Label0
						End If
						Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = DirectCast(methodBlockBaseSyntax.Parent, PropertyBlockSyntax).PropertyStatement.Identifier
						synthesizedLocal = LocalSymbol.Create(methodSymbol, Me, syntaxToken, LocalDeclarationKind.FunctionValue, methodSymbol.ReturnType)
						Exit Select
					Case SyntaxKind.AddHandlerAccessorBlock
						If (Not DirectCast(methodSymbol.AssociatedSymbol, EventSymbol).IsWindowsRuntimeEvent OrElse methodBlockBaseSyntax.Parent Is Nothing OrElse methodBlockBaseSyntax.Parent.Kind() <> SyntaxKind.EventBlock) Then
							GoTo Label0
						End If
						Dim identifier1 As Microsoft.CodeAnalysis.SyntaxToken = DirectCast(methodBlockBaseSyntax.Parent, EventBlockSyntax).EventStatement.Identifier
						synthesizedLocal = LocalSymbol.Create(methodSymbol, Me, identifier1, LocalDeclarationKind.FunctionValue, methodSymbol.ReturnType, methodSymbol.Name)
						Exit Select
					Case Else
						GoTo Label0
				End Select
			Else
				synthesizedLocal = Nothing
			End If
			Return synthesizedLocal
		End Function

		Public Overrides Function GetLocalForFunctionValue() As LocalSymbol
			Return Me._functionValue
		End Function
	End Class
End Namespace