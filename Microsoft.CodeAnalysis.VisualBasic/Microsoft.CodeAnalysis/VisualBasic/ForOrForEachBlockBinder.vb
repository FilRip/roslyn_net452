Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class ForOrForEachBlockBinder
		Inherits ExitableStatementBinder
		Private ReadOnly _syntax As ForOrForEachBlockSyntax

		Private _locals As ImmutableArray(Of LocalSymbol)

		Friend Overrides ReadOnly Property Locals As ImmutableArray(Of LocalSymbol)
			Get
				If (Me._locals.IsDefault) Then
					Dim localSymbols As ImmutableArray(Of LocalSymbol) = Me.BuildLocals()
					Dim localSymbols1 As ImmutableArray(Of LocalSymbol) = New ImmutableArray(Of LocalSymbol)()
					ImmutableInterlocked.InterlockedCompareExchange(Of LocalSymbol)(Me._locals, localSymbols, localSymbols1)
				End If
				Return Me._locals
			End Get
		End Property

		Public Sub New(ByVal enclosing As Binder, ByVal syntax As ForOrForEachBlockSyntax)
			MyBase.New(enclosing, SyntaxKind.ContinueForStatement, SyntaxKind.ExitForStatement)
			Me._locals = New ImmutableArray(Of LocalSymbol)()
			Me._syntax = syntax
		End Sub

		Private Function BuildLocals() As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
			Dim empty As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Nothing
			visualBasicSyntaxNode = If(Me._syntax.Kind() <> SyntaxKind.ForBlock, DirectCast(Me._syntax.ForOrForEachStatement, ForEachStatementSyntax).ControlVariable, DirectCast(Me._syntax.ForOrForEachStatement, ForStatementSyntax).ControlVariable)
			Dim variableDeclaratorSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax = TryCast(visualBasicSyntaxNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)
			If (variableDeclaratorSyntax Is Nothing) Then
				Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = TryCast(visualBasicSyntaxNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
				If (identifierNameSyntax IsNot Nothing) Then
					Dim identifier As SyntaxToken = identifierNameSyntax.Identifier
					If (Me.OptionInfer) Then
						Dim instance As LookupResult = LookupResult.GetInstance()
						Dim containingBinder As Binder = MyBase.ContainingBinder
						Dim valueText As String = identifier.ValueText
						Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
						containingBinder.Lookup(instance, valueText, 0, LookupOptions.AllMethodsOfAnyArity, discarded)
						If (Not instance.IsGoodOrAmbiguous OrElse instance.Symbols(0).Kind = SymbolKind.NamedType OrElse instance.Symbols(0).Kind = SymbolKind.TypeParameter) Then
							localSymbol = Me.CreateLocalSymbol(identifier)
						End If
						instance.Free()
					End If
				End If
			Else
				Dim item As ModifiedIdentifierSyntax = variableDeclaratorSyntax.Names(0)
				localSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol.Create(Me.ContainingMember, Me, item.Identifier, item, variableDeclaratorSyntax.AsClause, variableDeclaratorSyntax.Initializer, If(Me._syntax.Kind() = SyntaxKind.ForEachBlock, LocalDeclarationKind.ForEach, LocalDeclarationKind.[For]))
			End If
			If (localSymbol Is Nothing) Then
				empty = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).Empty
			Else
				empty = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)(localSymbol)
			End If
			Return empty
		End Function

		Private Function CreateLocalSymbol(ByVal identifier As SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol
			If (Me._syntax.Kind() <> SyntaxKind.ForBlock) Then
				Dim forOrForEachStatement As ForEachStatementSyntax = DirectCast(Me._syntax.ForOrForEachStatement, ForEachStatementSyntax)
				localSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol.CreateInferredForEach(Me.ContainingMember, Me, identifier, forOrForEachStatement.Expression)
			Else
				Dim forStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax = DirectCast(Me._syntax.ForOrForEachStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax)
				localSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol.CreateInferredForFromTo(Me.ContainingMember, Me, identifier, forStatementSyntax.FromValue, forStatementSyntax.ToValue, forStatementSyntax.StepClause)
			End If
			Return localSymbol
		End Function
	End Class
End Namespace