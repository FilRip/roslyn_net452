Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class SpeculativeMemberSemanticModel
		Inherits MemberSemanticModel
		Public Sub New(ByVal parentSemanticModel As SyntaxTreeSemanticModel, ByVal root As VisualBasicSyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal position As Integer)
			MyBase.New(root, binder, Nothing, parentSemanticModel, position, False)
		End Sub

		Friend Overrides Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal statement As ExecutableStatementSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal initializer As EqualsValueSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelForMethodBodyCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal method As MethodBlockBaseSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Throw ExceptionUtilities.Unreachable
		End Function
	End Class
End Namespace