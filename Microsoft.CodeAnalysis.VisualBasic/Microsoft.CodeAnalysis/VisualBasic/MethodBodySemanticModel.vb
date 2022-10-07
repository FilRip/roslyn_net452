Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class MethodBodySemanticModel
		Inherits MemberSemanticModel
		Private Sub New(ByVal root As SyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, Optional ByVal containingSemanticModelOpt As SyntaxTreeSemanticModel = Nothing, Optional ByVal parentSemanticModelOpt As SyntaxTreeSemanticModel = Nothing, Optional ByVal speculatedPosition As Integer = 0, Optional ByVal ignoreAccessibility As Boolean = False)
			MyBase.New(root, binder, containingSemanticModelOpt, parentSemanticModelOpt, speculatedPosition, ignoreAccessibility)
		End Sub

		Friend Shared Function Create(ByVal containingSemanticModel As SyntaxTreeSemanticModel, ByVal binder As SubOrFunctionBodyBinder, Optional ByVal ignoreAccessibility As Boolean = False) As MethodBodySemanticModel
			Return New MethodBodySemanticModel(binder.Root, binder, containingSemanticModel, Nothing, 0, ignoreAccessibility)
		End Function

		Friend Shared Function CreateSpeculative(ByVal parentSemanticModel As SyntaxTreeSemanticModel, ByVal root As VisualBasicSyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal position As Integer) As MethodBodySemanticModel
			Return New MethodBodySemanticModel(root, binder, Nothing, parentSemanticModel, position, False)
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal statement As ExecutableStatementSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Dim flag As Boolean
			Dim enclosingBinder As Binder = MyBase.GetEnclosingBinder(position)
			If (enclosingBinder IsNot Nothing) Then
				enclosingBinder = New SpeculativeStatementBinder(statement, enclosingBinder)
				enclosingBinder = New StatementListBinder(enclosingBinder, New SyntaxList(Of StatementSyntax)(statement))
				speculativeModel = MethodBodySemanticModel.CreateSpeculative(parentModel, statement, enclosingBinder, position)
				flag = True
			Else
				speculativeModel = Nothing
				flag = False
			End If
			Return flag
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal initializer As EqualsValueSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			speculativeModel = Nothing
			Return False
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelForMethodBodyCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal method As MethodBlockBaseSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Dim namedTypeBinder As Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder
			Dim memberSymbol As MethodSymbol = DirectCast(MyBase.MemberSymbol, MethodSymbol)
			Dim rootBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = MyBase.RootBinder
			While True
				namedTypeBinder = TryCast(rootBinder, Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder)
				If (namedTypeBinder IsNot Nothing) Then
					Exit While
				End If
				rootBinder = rootBinder.ContainingBinder
			End While
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForMethodBody(memberSymbol, method, SemanticModelBinder.Mark(namedTypeBinder, MyBase.IgnoresAccessibility))
			Dim statementListBinder As Microsoft.CodeAnalysis.VisualBasic.StatementListBinder = New Microsoft.CodeAnalysis.VisualBasic.StatementListBinder(binder, method.Statements)
			speculativeModel = MethodBodySemanticModel.CreateSpeculative(parentModel, method, statementListBinder, position)
			Return True
		End Function
	End Class
End Namespace