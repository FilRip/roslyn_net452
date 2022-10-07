Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class AttributeSemanticModel
		Inherits MemberSemanticModel
		Private Sub New(ByVal root As VisualBasicSyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, Optional ByVal containingSemanticModelOpt As SyntaxTreeSemanticModel = Nothing, Optional ByVal parentSemanticModelOpt As SyntaxTreeSemanticModel = Nothing, Optional ByVal speculatedPosition As Integer = 0, Optional ByVal ignoreAccessibility As Boolean = False)
			MyBase.New(root, binder, containingSemanticModelOpt, parentSemanticModelOpt, speculatedPosition, ignoreAccessibility)
		End Sub

		Friend Overrides Function Bind(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal node As SyntaxNode, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute) Then
				boundNode = binder.BindAttribute(DirectCast(node, AttributeSyntax), diagnostics)
			Else
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierName OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedName) Then
					If (Not SyntaxFacts.IsAttributeName(node)) Then
						GoTo Label1
					End If
					boundNode = binder.BindNamespaceOrTypeExpression(DirectCast(node, NameSyntax), diagnostics)
					Return boundNode
				End If
			Label1:
				boundNode = MyBase.Bind(binder, node, diagnostics)
			End If
			Return boundNode
		End Function

		Friend Shared Function Create(ByVal containingSemanticModel As SyntaxTreeSemanticModel, ByVal binder As AttributeBinder, Optional ByVal ignoreAccessibility As Boolean = False) As AttributeSemanticModel
			Return New AttributeSemanticModel(binder.Root, binder, containingSemanticModel, Nothing, 0, ignoreAccessibility)
		End Function

		Friend Shared Function CreateSpeculative(ByVal parentSemanticModel As SyntaxTreeSemanticModel, ByVal root As VisualBasicSyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal position As Integer) As AttributeSemanticModel
			Return New AttributeSemanticModel(root, binder, Nothing, parentSemanticModel, position, False)
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal initializer As EqualsValueSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			speculativeModel = Nothing
			Return False
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal statement As ExecutableStatementSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			speculativeModel = Nothing
			Return False
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelForMethodBodyCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal method As MethodBlockBaseSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			speculativeModel = Nothing
			Return False
		End Function
	End Class
End Namespace