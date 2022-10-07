Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundNamespaceExpression
		Inherits BoundExpression
		Private ReadOnly _UnevaluatedReceiverOpt As BoundExpression

		Private ReadOnly _AliasOpt As AliasSymbol

		Private ReadOnly _NamespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol

		Public ReadOnly Property AliasOpt As AliasSymbol
			Get
				Return Me._AliasOpt
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Dim aliasOpt As [Object] = Me.AliasOpt
				If (aliasOpt Is Nothing) Then
					aliasOpt = Me.NamespaceSymbol
				End If
				Return aliasOpt
			End Get
		End Property

		Public ReadOnly Property NamespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
			Get
				Return Me._NamespaceSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ResultKind As LookupResultKind
			Get
				Dim lookupResultKind As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind
				lookupResultKind = If(CInt(Me.NamespaceSymbol.NamespaceKind) <> 0, MyBase.ResultKind, LookupResult.WorseResultKind(Microsoft.CodeAnalysis.VisualBasic.LookupResultKind.Ambiguous, MyBase.ResultKind))
				Return lookupResultKind
			End Get
		End Property

		Public ReadOnly Property UnevaluatedReceiverOpt As BoundExpression
			Get
				Return Me._UnevaluatedReceiverOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal unevaluatedReceiverOpt As BoundExpression, ByVal namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol, ByVal hasErrors As Boolean)
			MyClass.New(syntax, unevaluatedReceiverOpt, Nothing, namespaceSymbol, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal unevaluatedReceiverOpt As BoundExpression, ByVal namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
			MyClass.New(syntax, unevaluatedReceiverOpt, Nothing, namespaceSymbol, False)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal unevaluatedReceiverOpt As BoundExpression, ByVal aliasOpt As AliasSymbol, ByVal namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.NamespaceExpression, syntax, Nothing, If(hasErrors, True, unevaluatedReceiverOpt.NonNullAndHasErrors()))
			Me._UnevaluatedReceiverOpt = unevaluatedReceiverOpt
			Me._AliasOpt = aliasOpt
			Me._NamespaceSymbol = namespaceSymbol
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitNamespaceExpression(Me)
		End Function

		Public Function Update(ByVal unevaluatedReceiverOpt As BoundExpression, ByVal aliasOpt As AliasSymbol, ByVal namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundNamespaceExpression
			Dim boundNamespaceExpression As Microsoft.CodeAnalysis.VisualBasic.BoundNamespaceExpression
			If (unevaluatedReceiverOpt <> Me.UnevaluatedReceiverOpt OrElse CObj(aliasOpt) <> CObj(Me.AliasOpt) OrElse CObj(namespaceSymbol) <> CObj(Me.NamespaceSymbol)) Then
				Dim boundNamespaceExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundNamespaceExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundNamespaceExpression(MyBase.Syntax, unevaluatedReceiverOpt, aliasOpt, namespaceSymbol, MyBase.HasErrors)
				boundNamespaceExpression1.CopyAttributes(Me)
				boundNamespaceExpression = boundNamespaceExpression1
			Else
				boundNamespaceExpression = Me
			End If
			Return boundNamespaceExpression
		End Function
	End Class
End Namespace