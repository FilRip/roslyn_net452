Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundLocalDeclaration
		Inherits BoundLocalDeclarationBase
		Implements IBoundLocalDeclarations
		Private ReadOnly _LocalSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol

		Private ReadOnly _DeclarationInitializerOpt As BoundExpression

		Private ReadOnly _IdentifierInitializerOpt As BoundArrayCreation

		Private ReadOnly _InitializedByAsNew As Boolean

		Public ReadOnly Property DeclarationInitializerOpt As BoundExpression
			Get
				Return Me._DeclarationInitializerOpt
			End Get
		End Property

		ReadOnly Property IBoundLocalDeclarations_Declarations As ImmutableArray(Of BoundLocalDeclarationBase) Implements IBoundLocalDeclarations.Declarations
			Get
				Return ImmutableArray.Create(Of BoundLocalDeclarationBase)(Me)
			End Get
		End Property

		Public ReadOnly Property IdentifierInitializerOpt As BoundArrayCreation
			Get
				Return Me._IdentifierInitializerOpt
			End Get
		End Property

		Public ReadOnly Property InitializedByAsNew As Boolean
			Get
				Return Me._InitializedByAsNew
			End Get
		End Property

		Public ReadOnly Property InitializerOpt As BoundExpression
			Get
				Dim declarationInitializerOpt As BoundExpression = Me.DeclarationInitializerOpt
				If (declarationInitializerOpt Is Nothing) Then
					declarationInitializerOpt = Me.IdentifierInitializerOpt
				End If
				Return declarationInitializerOpt
			End Get
		End Property

		Public ReadOnly Property LocalSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol
			Get
				Return Me._LocalSymbol
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal initializerOpt As BoundExpression)
			MyClass.New(syntax, localSymbol, initializerOpt, Nothing, False, False)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal declarationInitializerOpt As BoundExpression, ByVal identifierInitializerOpt As BoundArrayCreation, ByVal initializedByAsNew As Boolean, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.LocalDeclaration, syntax, If(hasErrors OrElse declarationInitializerOpt.NonNullAndHasErrors(), True, identifierInitializerOpt.NonNullAndHasErrors()))
			Me._LocalSymbol = localSymbol
			Me._DeclarationInitializerOpt = declarationInitializerOpt
			Me._IdentifierInitializerOpt = identifierInitializerOpt
			Me._InitializedByAsNew = initializedByAsNew
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitLocalDeclaration(Me)
		End Function

		Public Function Update(ByVal localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal declarationInitializerOpt As BoundExpression, ByVal identifierInitializerOpt As BoundArrayCreation, ByVal initializedByAsNew As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration
			Dim boundLocalDeclaration As Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration
			If (CObj(localSymbol) <> CObj(Me.LocalSymbol) OrElse declarationInitializerOpt <> Me.DeclarationInitializerOpt OrElse identifierInitializerOpt <> Me.IdentifierInitializerOpt OrElse initializedByAsNew <> Me.InitializedByAsNew) Then
				Dim boundLocalDeclaration1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration = New Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration(MyBase.Syntax, localSymbol, declarationInitializerOpt, identifierInitializerOpt, initializedByAsNew, MyBase.HasErrors)
				boundLocalDeclaration1.CopyAttributes(Me)
				boundLocalDeclaration = boundLocalDeclaration1
			Else
				boundLocalDeclaration = Me
			End If
			Return boundLocalDeclaration
		End Function
	End Class
End Namespace