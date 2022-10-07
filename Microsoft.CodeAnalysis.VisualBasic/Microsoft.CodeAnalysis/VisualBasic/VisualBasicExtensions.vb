Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Operations
Imports Microsoft.CodeAnalysis.Syntax
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Collections.ObjectModel
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Module VisualBasicExtensions
		<Extension>
		Public Function Add(ByVal list As SyntaxTokenList, ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As SyntaxTokenList
			Return list.Insert(list.Count, items)
		End Function

		<Extension>
		Public Function AliasImports(ByVal compilation As Microsoft.CodeAnalysis.Compilation) As ImmutableArray(Of IAliasSymbol)
			Dim aliasSymbols As ImmutableArray(Of IAliasSymbol)
			Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = TryCast(compilation, Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation)
			aliasSymbols = If(visualBasicCompilation Is Nothing, ImmutableArray.Create(Of IAliasSymbol)(), StaticCast(Of IAliasSymbol).From(Of AliasSymbol)(visualBasicCompilation.AliasImports))
			Return aliasSymbols
		End Function

		<Extension>
		Public Function AnalyzeControlFlow(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal firstStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax, ByVal lastStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) As ControlFlowAnalysis
			Dim controlFlowAnalysi As ControlFlowAnalysis
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				controlFlowAnalysi = Nothing
			Else
				controlFlowAnalysi = vBSemanticModel.AnalyzeControlFlow(firstStatement, lastStatement)
			End If
			Return controlFlowAnalysi
		End Function

		<Extension>
		Public Function AnalyzeControlFlow(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) As ControlFlowAnalysis
			Dim controlFlowAnalysi As ControlFlowAnalysis
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				controlFlowAnalysi = Nothing
			Else
				controlFlowAnalysi = vBSemanticModel.AnalyzeControlFlow(statement)
			End If
			Return controlFlowAnalysi
		End Function

		<Extension>
		Public Function AnalyzeDataFlow(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As DataFlowAnalysis
			Dim dataFlowAnalysi As DataFlowAnalysis
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				dataFlowAnalysi = Nothing
			Else
				dataFlowAnalysi = vBSemanticModel.AnalyzeDataFlow(expression)
			End If
			Return dataFlowAnalysi
		End Function

		<Extension>
		Public Function AnalyzeDataFlow(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal firstStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax, ByVal lastStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) As DataFlowAnalysis
			Dim dataFlowAnalysi As DataFlowAnalysis
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				dataFlowAnalysi = Nothing
			Else
				dataFlowAnalysi = vBSemanticModel.AnalyzeDataFlow(firstStatement, lastStatement)
			End If
			Return dataFlowAnalysi
		End Function

		<Extension>
		Public Function AnalyzeDataFlow(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) As DataFlowAnalysis
			Dim dataFlowAnalysi As DataFlowAnalysis
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				dataFlowAnalysi = Nothing
			Else
				dataFlowAnalysi = vBSemanticModel.AnalyzeDataFlow(statement)
			End If
			Return dataFlowAnalysi
		End Function

		<Extension>
		Friend Function AsSeparatedList(Of TOther As Microsoft.CodeAnalysis.SyntaxNode)(ByVal list As SyntaxNodeOrTokenList) As Microsoft.CodeAnalysis.SeparatedSyntaxList(Of TOther)
			Dim enumerator As SyntaxNodeOrTokenList.Enumerator = New SyntaxNodeOrTokenList.Enumerator()
			Dim separatedSyntaxListBuilder As Microsoft.CodeAnalysis.Syntax.SeparatedSyntaxListBuilder(Of TOther) = Microsoft.CodeAnalysis.Syntax.SeparatedSyntaxListBuilder(Of TOther).Create()
			Try
				enumerator = list.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As SyntaxNodeOrToken = enumerator.Current
					Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = current.AsNode()
					If (syntaxNode Is Nothing) Then
						Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = current.AsToken()
						separatedSyntaxListBuilder.AddSeparator(syntaxToken)
					Else
						separatedSyntaxListBuilder.Add(DirectCast(syntaxNode, TOther))
					End If
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			Return separatedSyntaxListBuilder.ToList()
		End Function

		<Extension>
		Public Function AssociatedField(ByVal eventSymbol As IEventSymbol) As IFieldSymbol
			Dim eventSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = TryCast(eventSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol)
			If (eventSymbol1 Is Nothing) Then
				Return Nothing
			End If
			Return eventSymbol1.AssociatedField
		End Function

		<Extension>
		Public Function ClassifyConversion(ByVal compilation As Microsoft.CodeAnalysis.Compilation, ByVal source As ITypeSymbol, ByVal destination As ITypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = TryCast(compilation, Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation)
			conversion = If(visualBasicCompilation Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.Conversion(), visualBasicCompilation.ClassifyConversion(DirectCast(source, TypeSymbol), DirectCast(destination, TypeSymbol)))
			Return conversion
		End Function

		<Extension>
		Public Function ClassifyConversion(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal destination As ITypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			conversion = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.Conversion(), vBSemanticModel.ClassifyConversion(expression, DirectCast(destination, TypeSymbol)))
			Return conversion
		End Function

		<Extension>
		Public Function ClassifyConversion(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal destination As ITypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			conversion = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.Conversion(), vBSemanticModel.ClassifyConversion(position, expression, DirectCast(destination, TypeSymbol)))
			Return conversion
		End Function

		<Extension>
		Friend Function Errors(ByVal trivia As Microsoft.CodeAnalysis.SyntaxTrivia) As SyntaxDiagnosticInfoList
			Return New SyntaxDiagnosticInfoList(DirectCast(trivia.UnderlyingNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode))
		End Function

		<Extension>
		Friend Function Errors(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As SyntaxDiagnosticInfoList
			Return New SyntaxDiagnosticInfoList(DirectCast(token.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode))
		End Function

		<Extension>
		Public Function GetAggregateClauseSymbolInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal aggregateSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo
			Dim aggregateClauseSymbolInfo As Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			aggregateClauseSymbolInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo(), vBSemanticModel.GetAggregateClauseSymbolInfo(aggregateSyntax, cancellationToken))
			Return aggregateClauseSymbolInfo
		End Function

		<Extension>
		Public Function GetAliasInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IAliasSymbol
			Dim aliasInfo As IAliasSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				aliasInfo = Nothing
			Else
				aliasInfo = vBSemanticModel.GetAliasInfo(nameSyntax, cancellationToken)
			End If
			Return aliasInfo
		End Function

		<Extension>
		Public Function GetAwaitExpressionInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal awaitExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.AwaitExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.AwaitExpressionInfo
			Dim awaitExpressionInfo As Microsoft.CodeAnalysis.VisualBasic.AwaitExpressionInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			awaitExpressionInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.AwaitExpressionInfo(), vBSemanticModel.GetAwaitExpressionInfo(awaitExpression, cancellationToken))
			Return awaitExpressionInfo
		End Function

		<Extension>
		Public Function GetBase(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Nullable(Of LiteralBase)
			Dim nullable As Nullable(Of LiteralBase)
			If (Not token.IsKind(SyntaxKind.IntegerLiteralToken)) Then
				nullable = Nothing
			Else
				nullable = New Nullable(Of LiteralBase)(DirectCast(token.Node, IntegerLiteralTokenSyntax).Base)
			End If
			Return nullable
		End Function

		<Extension>
		Public Function GetCollectionInitializerSymbolInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			symbolInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.SymbolInfo(), vBSemanticModel.GetCollectionInitializerSymbolInfo(expression, cancellationToken))
			Return symbolInfo
		End Function

		<Extension>
		Public Function GetCollectionRangeVariableSymbolInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal variableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.CollectionRangeVariableSymbolInfo
			Dim collectionRangeVariableSymbolInfo As Microsoft.CodeAnalysis.VisualBasic.CollectionRangeVariableSymbolInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			collectionRangeVariableSymbolInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.CollectionRangeVariableSymbolInfo(), vBSemanticModel.GetCollectionRangeVariableSymbolInfo(variableSyntax, cancellationToken))
			Return collectionRangeVariableSymbolInfo
		End Function

		<Extension>
		Public Function GetCompilationUnitRoot(ByVal tree As SyntaxTree) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax
			Return DirectCast(tree.GetRoot(New CancellationToken()), Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax)
		End Function

		<Extension>
		Public Function GetConversion(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal expression As SyntaxNode, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			conversion = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.Conversion(), vBSemanticModel.GetConversion(expression, cancellationToken))
			Return conversion
		End Function

		<Extension>
		Public Function GetConversion(ByVal conversionExpression As IConversionOperation) As Conversion
			If (EmbeddedOperators.CompareString(conversionExpression.Language, "Visual Basic", False) <> 0) Then
				Throw New ArgumentException([String].Format(VBResources.IConversionExpressionIsNotVisualBasicConversion, "IConversionOperation"), "conversionExpression")
			End If
			Return DirectCast(DirectCast(conversionExpression, ConversionOperation).ConversionConvertible, Conversion)
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal identifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ISymbol
			Dim declaredSymbol As ISymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(identifierSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal elementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleElementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ISymbol
			Dim declaredSymbol As ISymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(elementSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal fieldInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IPropertySymbol
			Dim declaredSymbol As IPropertySymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(fieldInitializerSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal anonymousObjectCreationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol
			Dim declaredSymbol As INamedTypeSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(anonymousObjectCreationExpressionSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal rangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IRangeVariableSymbol
			Dim declaredSymbol As IRangeVariableSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal rangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IRangeVariableSymbol
			Dim declaredSymbol As IRangeVariableSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal rangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IRangeVariableSymbol
			Dim declaredSymbol As IRangeVariableSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ILabelSymbol
			Dim declaredSymbol As ILabelSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IFieldSymbol
			Dim declaredSymbol As IFieldSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol
			Dim declaredSymbol As INamedTypeSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol
			Dim declaredSymbol As INamedTypeSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol
			Dim declaredSymbol As INamedTypeSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol
			Dim declaredSymbol As INamedTypeSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamespaceSymbol
			Dim declaredSymbol As INamespaceSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamespaceSymbol
			Dim declaredSymbol As INamespaceSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal parameter As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IParameterSymbol
			Dim declaredSymbol As IParameterSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(parameter, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal typeParameter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ITypeParameterSymbol
			Dim declaredSymbol As ITypeParameterSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(typeParameter, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol
			Dim declaredSymbol As INamedTypeSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IMethodSymbol
			Dim declaredSymbol As IMethodSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IMethodSymbol
			Dim declaredSymbol As IMethodSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IMethodSymbol
			Dim declaredSymbol As IMethodSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IMethodSymbol
			Dim declaredSymbol As IMethodSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IMethodSymbol
			Dim declaredSymbol As IMethodSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IPropertySymbol
			Dim declaredSymbol As IPropertySymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IEventSymbol
			Dim declaredSymbol As IEventSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IPropertySymbol
			Dim declaredSymbol As IPropertySymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IEventSymbol
			Dim declaredSymbol As IEventSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ILocalSymbol
			Dim declaredSymbol As ILocalSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IMethodSymbol
			Dim declaredSymbol As IMethodSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDeclaredSymbol(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal declarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleImportsClauseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IAliasSymbol
			Dim declaredSymbol As IAliasSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				declaredSymbol = Nothing
			Else
				declaredSymbol = vBSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		<Extension>
		Public Function GetDirectives(ByVal node As SyntaxNode, Optional ByVal filter As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax, Boolean) = Nothing) As IList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax)
			Return DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode).GetDirectives(filter)
		End Function

		<Extension>
		Public Function GetFieldAttributes(ByVal eventSymbol As IEventSymbol) As ImmutableArray(Of AttributeData)
			Dim empty As ImmutableArray(Of AttributeData)
			Dim eventSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = TryCast(eventSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol)
			If (eventSymbol1 Is Nothing) Then
				empty = ImmutableArray(Of AttributeData).Empty
			Else
				empty = StaticCast(Of AttributeData).From(Of VisualBasicAttributeData)(eventSymbol1.GetFieldAttributes())
			End If
			Return empty
		End Function

		<Extension>
		Public Function GetFirstDirective(ByVal node As SyntaxNode, Optional ByVal predicate As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax, Boolean) = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Return DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode).GetFirstDirective(predicate)
		End Function

		<Extension>
		Public Function GetForEachStatementInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo
			Dim forEachStatementInfo As Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			forEachStatementInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo(), vBSemanticModel.GetForEachStatementInfo(node))
			Return forEachStatementInfo
		End Function

		<Extension>
		Public Function GetForEachStatementInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo
			Dim forEachStatementInfo As Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			forEachStatementInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo(), vBSemanticModel.GetForEachStatementInfo(node))
			Return forEachStatementInfo
		End Function

		<Extension>
		Public Function GetIdentifierText(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As String
			If (token.Node Is Nothing) Then
				Return [String].Empty
			End If
			If (Not token.IsKind(SyntaxKind.IdentifierToken)) Then
				Return token.ToString()
			End If
			Return DirectCast(token.Node, IdentifierTokenSyntax).IdentifierText
		End Function

		<Extension>
		Public Function GetInConversion(ByVal argument As IArgumentOperation) As Conversion
			If (EmbeddedOperators.CompareString(argument.Language, "Visual Basic", False) <> 0) Then
				Throw New ArgumentException([String].Format(VBResources.IArgumentIsNotVisualBasicArgument, "IArgumentOperation"), "argument")
			End If
			Dim inConversionConvertible As IConvertibleConversion = DirectCast(argument, ArgumentOperation).InConversionConvertible
			Return If(inConversionConvertible IsNot Nothing, DirectCast(inConversionConvertible, Conversion), New Conversion(Microsoft.CodeAnalysis.VisualBasic.Conversions.Identity))
		End Function

		<Extension>
		Public Function GetInConversion(ByVal compoundAssignment As ICompoundAssignmentOperation) As Conversion
			If (compoundAssignment Is Nothing) Then
				Throw New ArgumentNullException("compoundAssignment")
			End If
			If (EmbeddedOperators.CompareString(compoundAssignment.Language, "Visual Basic", False) <> 0) Then
				Throw New ArgumentException([String].Format(VBResources.ICompoundAssignmentOperationIsNotVisualBasicCompoundAssignment, "compoundAssignment"), "compoundAssignment")
			End If
			Return DirectCast(DirectCast(compoundAssignment, CompoundAssignmentOperation).InConversionConvertible, Conversion)
		End Function

		<Extension>
		Public Function GetLastDirective(ByVal node As SyntaxNode, Optional ByVal predicate As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax, Boolean) = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Return DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode).GetLastDirective(predicate)
		End Function

		<Extension>
		Friend Function GetLocation(ByVal syntaxReference As Microsoft.CodeAnalysis.SyntaxReference) As Location
			Dim sourceLocation As Location
			Dim syntaxTree As VisualBasicSyntaxTree = TryCast(syntaxReference.SyntaxTree, VisualBasicSyntaxTree)
			If (syntaxReference.SyntaxTree IsNot Nothing) Then
				If (Not syntaxTree.IsEmbeddedSyntaxTree()) Then
					If (Not syntaxTree.IsMyTemplate) Then
						sourceLocation = New Microsoft.CodeAnalysis.SourceLocation(syntaxReference)
						Return sourceLocation
					End If
					sourceLocation = New MyTemplateLocation(syntaxTree, syntaxReference.Span)
					Return sourceLocation
				Else
					sourceLocation = New EmbeddedTreeLocation(syntaxTree.GetEmbeddedKind(), syntaxReference.Span)
					Return sourceLocation
				End If
			End If
			sourceLocation = New Microsoft.CodeAnalysis.SourceLocation(syntaxReference)
			Return sourceLocation
		End Function

		<Extension>
		Public Function GetMemberGroup(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of ISymbol)
			Dim symbols As ImmutableArray(Of ISymbol)
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			symbols = If(vBSemanticModel Is Nothing, ImmutableArray.Create(Of ISymbol)(), vBSemanticModel.GetMemberGroup(expression, cancellationToken))
			Return symbols
		End Function

		<Extension>
		Public Function GetMemberGroup(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal attribute As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of ISymbol)
			Dim symbols As ImmutableArray(Of ISymbol)
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			symbols = If(vBSemanticModel Is Nothing, ImmutableArray.Create(Of ISymbol)(), vBSemanticModel.GetMemberGroup(attribute, cancellationToken))
			Return symbols
		End Function

		<Extension>
		Public Function GetModuleMembers(ByVal [namespace] As INamespaceSymbol) As ImmutableArray(Of INamedTypeSymbol)
			Dim namedTypeSymbols As ImmutableArray(Of INamedTypeSymbol)
			Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = TryCast([namespace], Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
			namedTypeSymbols = If(namespaceSymbol Is Nothing, ImmutableArray.Create(Of INamedTypeSymbol)(), StaticCast(Of INamedTypeSymbol).From(Of NamedTypeSymbol)(namespaceSymbol.GetModuleMembers()))
			Return namedTypeSymbols
		End Function

		<Extension>
		Public Function GetModuleMembers(ByVal [namespace] As INamespaceSymbol, ByVal name As String) As ImmutableArray(Of INamedTypeSymbol)
			Dim namedTypeSymbols As ImmutableArray(Of INamedTypeSymbol)
			Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = TryCast([namespace], Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
			namedTypeSymbols = If(namespaceSymbol Is Nothing, ImmutableArray.Create(Of INamedTypeSymbol)(), StaticCast(Of INamedTypeSymbol).From(Of NamedTypeSymbol)(namespaceSymbol.GetModuleMembers(name)))
			Return namedTypeSymbols
		End Function

		<Extension>
		Public Function GetOutConversion(ByVal argument As IArgumentOperation) As Conversion
			If (EmbeddedOperators.CompareString(argument.Language, "Visual Basic", False) <> 0) Then
				Throw New ArgumentException([String].Format(VBResources.IArgumentIsNotVisualBasicArgument, "IArgumentOperation"), "argument")
			End If
			Dim outConversionConvertible As IConvertibleConversion = DirectCast(argument, ArgumentOperation).OutConversionConvertible
			Return If(outConversionConvertible IsNot Nothing, DirectCast(outConversionConvertible, Conversion), New Conversion(Microsoft.CodeAnalysis.VisualBasic.Conversions.Identity))
		End Function

		<Extension>
		Public Function GetOutConversion(ByVal compoundAssignment As ICompoundAssignmentOperation) As Conversion
			If (compoundAssignment Is Nothing) Then
				Throw New ArgumentNullException("compoundAssignment")
			End If
			If (EmbeddedOperators.CompareString(compoundAssignment.Language, "Visual Basic", False) <> 0) Then
				Throw New ArgumentException([String].Format(VBResources.ICompoundAssignmentOperationIsNotVisualBasicCompoundAssignment, "compoundAssignment"), "compoundAssignment")
			End If
			Return DirectCast(DirectCast(compoundAssignment, CompoundAssignmentOperation).OutConversionConvertible, Conversion)
		End Function

		<Extension>
		Friend Function GetPreprocessingSymbolInfo(ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree, ByVal identifierNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) As VisualBasicPreprocessingSymbolInfo
			Return DirectCast(syntaxTree, VisualBasicSyntaxTree).GetPreprocessingSymbolInfo(identifierNode)
		End Function

		<Extension>
		Public Function GetPreprocessingSymbolInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) As PreprocessingSymbolInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			Return If(vBSemanticModel Is Nothing, PreprocessingSymbolInfo.None, vBSemanticModel.GetPreprocessingSymbolInfo(node))
		End Function

		<Extension>
		Public Function GetSpecialType(ByVal compilation As Microsoft.CodeAnalysis.Compilation, ByVal typeId As Microsoft.CodeAnalysis.SpecialType) As INamedTypeSymbol
			Dim specialType As INamedTypeSymbol
			Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = TryCast(compilation, Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation)
			If (visualBasicCompilation Is Nothing) Then
				specialType = Nothing
			Else
				specialType = visualBasicCompilation.GetSpecialType(typeId)
			End If
			Return specialType
		End Function

		<Extension>
		Public Function GetSpeculativeAliasInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, ByVal nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax, ByVal bindingOption As SpeculativeBindingOption) As IAliasSymbol
			Dim speculativeAliasInfo As IAliasSymbol
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				speculativeAliasInfo = Nothing
			Else
				speculativeAliasInfo = vBSemanticModel.GetSpeculativeAliasInfo(position, nameSyntax, bindingOption)
			End If
			Return speculativeAliasInfo
		End Function

		<Extension>
		Public Function GetSpeculativeConversion(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal bindingOption As SpeculativeBindingOption) As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			conversion = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.Conversion(), vBSemanticModel.GetSpeculativeConversion(position, expression, bindingOption))
			Return conversion
		End Function

		<Extension>
		Public Function GetSpeculativeMemberGroup(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As ImmutableArray(Of ISymbol)
			Dim symbols As ImmutableArray(Of ISymbol)
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			symbols = If(vBSemanticModel Is Nothing, ImmutableArray.Create(Of ISymbol)(), vBSemanticModel.GetSpeculativeMemberGroup(position, expression))
			Return symbols
		End Function

		<Extension>
		Public Function GetSpeculativeSymbolInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal bindingOption As SpeculativeBindingOption) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			symbolInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.SymbolInfo(), vBSemanticModel.GetSpeculativeSymbolInfo(position, expression, bindingOption))
			Return symbolInfo
		End Function

		<Extension>
		Public Function GetSpeculativeSymbolInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, ByVal attribute As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			symbolInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.SymbolInfo(), vBSemanticModel.GetSpeculativeSymbolInfo(position, attribute))
			Return symbolInfo
		End Function

		<Extension>
		Public Function GetSpeculativeTypeInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal bindingOption As SpeculativeBindingOption) As Microsoft.CodeAnalysis.TypeInfo
			Dim typeInfo As Microsoft.CodeAnalysis.TypeInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			typeInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.TypeInfo(), vBSemanticModel.GetSpeculativeTypeInfo(position, expression, bindingOption))
			Return typeInfo
		End Function

		<Extension>
		Public Function GetSymbolInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			symbolInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.SymbolInfo(), vBSemanticModel.GetSymbolInfo(expression, cancellationToken))
			Return symbolInfo
		End Function

		<Extension>
		Public Function GetSymbolInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal crefReference As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			symbolInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.SymbolInfo(), vBSemanticModel.GetSymbolInfo(crefReference, cancellationToken))
			Return symbolInfo
		End Function

		<Extension>
		Public Function GetSymbolInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal attribute As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			symbolInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.SymbolInfo(), vBSemanticModel.GetSymbolInfo(attribute, cancellationToken))
			Return symbolInfo
		End Function

		<Extension>
		Public Function GetSymbolInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal clauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			symbolInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.SymbolInfo(), vBSemanticModel.GetSymbolInfo(clauseSyntax, cancellationToken))
			Return symbolInfo
		End Function

		<Extension>
		Public Function GetSymbolInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal variableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			symbolInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.SymbolInfo(), vBSemanticModel.GetSymbolInfo(variableSyntax, cancellationToken))
			Return symbolInfo
		End Function

		<Extension>
		Public Function GetSymbolInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal functionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			symbolInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.SymbolInfo(), vBSemanticModel.GetSymbolInfo(functionSyntax, cancellationToken))
			Return symbolInfo
		End Function

		<Extension>
		Public Function GetSymbolInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal orderingSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			symbolInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.SymbolInfo(), vBSemanticModel.GetSymbolInfo(orderingSyntax, cancellationToken))
			Return symbolInfo
		End Function

		<Extension>
		Friend Function GetSyntaxErrors(ByVal token As Microsoft.CodeAnalysis.SyntaxToken, ByVal tree As SyntaxTree) As ReadOnlyCollection(Of Diagnostic)
			Return Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode.DoGetSyntaxErrors(tree, token)
		End Function

		<Extension>
		Public Function GetTypeCharacter(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter
			Dim typeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter
			Select Case token.Kind()
				Case SyntaxKind.IdentifierToken
					typeCharacter = DirectCast(token.Node, IdentifierTokenSyntax).TypeCharacter
					Exit Select
				Case SyntaxKind.IntegerLiteralToken
					typeCharacter = DirectCast(token.Node, IntegerLiteralTokenSyntax).TypeSuffix
					Exit Select
				Case SyntaxKind.FloatingLiteralToken
					typeCharacter = DirectCast(token.Node, FloatingLiteralTokenSyntax).TypeSuffix
					Exit Select
				Case SyntaxKind.DecimalLiteralToken
					typeCharacter = DirectCast(token.Node, DecimalLiteralTokenSyntax).TypeSuffix
					Exit Select
				Case Else
					typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.None
					Exit Select
			End Select
			Return typeCharacter
		End Function

		<Extension>
		Public Function GetTypeInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.TypeInfo
			Dim typeInfo As Microsoft.CodeAnalysis.TypeInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			typeInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.TypeInfo(), vBSemanticModel.GetTypeInfo(expression, cancellationToken))
			Return typeInfo
		End Function

		<Extension>
		Public Function GetTypeInfo(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal attribute As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.TypeInfo
			Dim typeInfo As Microsoft.CodeAnalysis.TypeInfo
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			typeInfo = If(vBSemanticModel Is Nothing, New Microsoft.CodeAnalysis.TypeInfo(), vBSemanticModel.GetTypeInfo(attribute, cancellationToken))
			Return typeInfo
		End Function

		<Extension>
		Friend Function GetVisualBasicRoot(ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
			Return DirectCast(syntaxTree.GetRoot(cancellationToken), Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
		End Function

		<Extension>
		Friend Function GetVisualBasicSyntax(ByVal syntaxReference As Microsoft.CodeAnalysis.SyntaxReference, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
			Return DirectCast(syntaxReference.GetSyntax(cancellationToken), Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
		End Function

		<Extension>
		Friend Function GetWarningState(ByVal tree As SyntaxTree, ByVal id As String, ByVal position As Integer) As ReportDiagnostic
			Return DirectCast(tree, VisualBasicSyntaxTree).GetWarningState(id, position)
		End Function

		<Extension>
		Public Function HandledEvents(ByVal methodSymbol As IMethodSymbol) As ImmutableArray(Of HandledEvent)
			Dim empty As ImmutableArray(Of HandledEvent)
			Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = TryCast(methodSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			If (methodSymbol1 Is Nothing) Then
				empty = ImmutableArray(Of HandledEvent).Empty
			Else
				empty = methodSymbol1.HandledEvents
			End If
			Return empty
		End Function

		<Extension>
		Public Function HasAssociatedField(ByVal eventSymbol As IEventSymbol) As Boolean
			Dim eventSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = TryCast(eventSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol)
			If (eventSymbol1 Is Nothing) Then
				Return False
			End If
			Return eventSymbol1.HasAssociatedField
		End Function

		<Extension>
		Friend Function HasReferenceDirectives(ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree) As Boolean
			Dim visualBasicSyntaxTree As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree = TryCast(syntaxTree, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree)
			If (visualBasicSyntaxTree Is Nothing) Then
				Return False
			End If
			Return visualBasicSyntaxTree.HasReferenceDirectives
		End Function

		<Extension>
		Public Function Insert(ByVal list As SyntaxTokenList, ByVal index As Integer, ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As SyntaxTokenList
			Dim syntaxTokenLists As SyntaxTokenList
			If (index < 0 OrElse index > list.Count) Then
				Throw New ArgumentOutOfRangeException("index")
			End If
			If (items Is Nothing) Then
				Throw New ArgumentNullException("items")
			End If
			If (list.Count <> 0) Then
				Dim syntaxTokenListBuilder As Microsoft.CodeAnalysis.Syntax.SyntaxTokenListBuilder = New Microsoft.CodeAnalysis.Syntax.SyntaxTokenListBuilder(list.Count + CInt(items.Length))
				If (index > 0) Then
					syntaxTokenListBuilder.Add(list, 0, index)
				End If
				syntaxTokenListBuilder.Add(items)
				If (index < list.Count) Then
					syntaxTokenListBuilder.Add(list, index, list.Count - index)
				End If
				syntaxTokenLists = syntaxTokenListBuilder.ToList()
			Else
				syntaxTokenLists = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TokenList(items)
			End If
			Return syntaxTokenLists
		End Function

		<Extension>
		Friend Function IsAnyPreprocessorSymbolDefined(ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree, ByVal conditionalSymbolNames As IEnumerable(Of String), ByVal atNode As SyntaxNodeOrToken) As Boolean
			Dim visualBasicSyntaxTree As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree = TryCast(syntaxTree, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree)
			If (visualBasicSyntaxTree Is Nothing) Then
				Return False
			End If
			Return visualBasicSyntaxTree.IsAnyPreprocessorSymbolDefined(conditionalSymbolNames, atNode)
		End Function

		<Extension>
		Public Function IsBracketed(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Boolean
			Dim flag As Boolean
			flag = If(Not token.IsKind(SyntaxKind.IdentifierToken), False, DirectCast(token.Node, IdentifierTokenSyntax).IsBracketed)
			Return flag
		End Function

		<Extension>
		Public Function IsCatch(ByVal localSymbol As ILocalSymbol) As Boolean
			Dim localSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = TryCast(localSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
			If (localSymbol1 Is Nothing) Then
				Return False
			End If
			Return localSymbol1.IsCatch
		End Function

		<Extension>
		Public Function IsContextualKeyword(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Boolean
			Return SyntaxFacts.IsContextualKeyword(token.Kind())
		End Function

		<Extension>
		Public Function IsDefault(ByVal propertySymbol As IPropertySymbol) As Boolean
			Dim propertySymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = TryCast(propertySymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
			If (propertySymbol1 Is Nothing) Then
				Return False
			End If
			Return propertySymbol1.IsDefault
		End Function

		<Extension>
		Public Function IsFor(ByVal localSymbol As ILocalSymbol) As Boolean
			Dim localSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = TryCast(localSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
			If (localSymbol1 Is Nothing) Then
				Return False
			End If
			Return localSymbol1.IsFor
		End Function

		<Extension>
		Public Function IsForEach(ByVal localSymbol As ILocalSymbol) As Boolean
			Dim localSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = TryCast(localSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
			If (localSymbol1 Is Nothing) Then
				Return False
			End If
			Return localSymbol1.IsForEach
		End Function

		<Extension>
		Public Function IsImplicitlyDeclared(ByVal eventSymbol As IEventSymbol) As Boolean
			Dim eventSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = TryCast(eventSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol)
			If (eventSymbol1 Is Nothing) Then
				Return False
			End If
			Return eventSymbol1.IsImplicitlyDeclared
		End Function

		<Extension>
		Public Function IsKeyword(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Boolean
			Return SyntaxFacts.IsKeywordKind(token.Kind())
		End Function

		<Extension>
		Public Function IsMe(ByVal parameterSymbol As IParameterSymbol) As Boolean
			Return parameterSymbol.IsThis
		End Function

		<Extension>
		Public Function IsMustOverride(ByVal symbol As ISymbol) As Boolean
			Return symbol.IsAbstract
		End Function

		<Extension>
		Friend Function IsMyTemplate(ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree) As Boolean
			Dim visualBasicSyntaxTree As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree = TryCast(syntaxTree, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree)
			If (visualBasicSyntaxTree Is Nothing) Then
				Return False
			End If
			Return visualBasicSyntaxTree.IsMyTemplate
		End Function

		<Extension>
		Public Function IsNotOverridable(ByVal symbol As ISymbol) As Boolean
			Return symbol.IsSealed
		End Function

		<Extension>
		Public Function IsOverloads(ByVal methodSymbol As IMethodSymbol) As Boolean
			Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = TryCast(methodSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			If (methodSymbol1 Is Nothing) Then
				Return False
			End If
			Return methodSymbol1.IsOverloads
		End Function

		<Extension>
		Public Function IsOverloads(ByVal propertySymbol As IPropertySymbol) As Boolean
			Dim propertySymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = TryCast(propertySymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
			If (propertySymbol1 Is Nothing) Then
				Return False
			End If
			Return propertySymbol1.IsOverloads
		End Function

		<Extension>
		Public Function IsOverridable(ByVal symbol As ISymbol) As Boolean
			Return symbol.IsVirtual
		End Function

		<Extension>
		Public Function IsOverrides(ByVal symbol As ISymbol) As Boolean
			Return symbol.IsOverride
		End Function

		<Extension>
		Public Function IsPreprocessorKeyword(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Boolean
			Return SyntaxFacts.IsPreprocessorKeyword(token.Kind())
		End Function

		<Extension>
		Public Function IsReservedKeyword(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Boolean
			Return SyntaxFacts.IsReservedKeyword(token.Kind())
		End Function

		<Extension>
		Public Function IsShared(ByVal symbol As ISymbol) As Boolean
			Return symbol.IsStatic
		End Function

		Friend Function IsVisualBasicKind(ByVal rawKind As Integer) As Boolean
			Return rawKind <= 8192
		End Function

		<Extension>
		Public Function Kind(ByVal trivia As Microsoft.CodeAnalysis.SyntaxTrivia) As SyntaxKind
			Dim rawKind As Integer = trivia.RawKind
			If (Not Microsoft.CodeAnalysis.VisualBasic.VisualBasicExtensions.IsVisualBasicKind(rawKind)) Then
				Return SyntaxKind.None
			End If
			Return DirectCast(CUShort(rawKind), SyntaxKind)
		End Function

		<Extension>
		Public Function Kind(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As SyntaxKind
			Dim rawKind As Integer = token.RawKind
			If (Not Microsoft.CodeAnalysis.VisualBasic.VisualBasicExtensions.IsVisualBasicKind(rawKind)) Then
				Return SyntaxKind.None
			End If
			Return DirectCast(CUShort(rawKind), SyntaxKind)
		End Function

		<Extension>
		Public Function Kind(ByVal node As SyntaxNode) As SyntaxKind
			Dim rawKind As Integer = node.RawKind
			If (Not Microsoft.CodeAnalysis.VisualBasic.VisualBasicExtensions.IsVisualBasicKind(rawKind)) Then
				Return SyntaxKind.None
			End If
			Return DirectCast(CUShort(rawKind), SyntaxKind)
		End Function

		<Extension>
		Public Function Kind(ByVal nodeOrToken As SyntaxNodeOrToken) As SyntaxKind
			Dim rawKind As Integer = nodeOrToken.RawKind
			If (Not Microsoft.CodeAnalysis.VisualBasic.VisualBasicExtensions.IsVisualBasicKind(rawKind)) Then
				Return SyntaxKind.None
			End If
			Return DirectCast(CUShort(rawKind), SyntaxKind)
		End Function

		<Extension>
		Public Function MemberImports(ByVal compilation As Microsoft.CodeAnalysis.Compilation) As ImmutableArray(Of INamespaceOrTypeSymbol)
			Dim namespaceOrTypeSymbols As ImmutableArray(Of INamespaceOrTypeSymbol)
			Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = TryCast(compilation, Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation)
			namespaceOrTypeSymbols = If(visualBasicCompilation Is Nothing, ImmutableArray.Create(Of INamespaceOrTypeSymbol)(), StaticCast(Of INamespaceOrTypeSymbol).From(Of NamespaceOrTypeSymbol)(visualBasicCompilation.MemberImports))
			Return namespaceOrTypeSymbols
		End Function

		<Extension>
		Public Function OptionCompareText(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel) As Boolean
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			Return If(vBSemanticModel Is Nothing, False, vBSemanticModel.OptionCompareText)
		End Function

		<Extension>
		Public Function OptionExplicit(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel) As Boolean
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			Return If(vBSemanticModel Is Nothing, False, vBSemanticModel.OptionExplicit)
		End Function

		<Extension>
		Public Function OptionInfer(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel) As Boolean
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			Return If(vBSemanticModel Is Nothing, False, vBSemanticModel.OptionInfer)
		End Function

		<Extension>
		Public Function OptionStrict(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel) As OptionStrict
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			Return If(vBSemanticModel Is Nothing, OptionStrict.Off, vBSemanticModel.OptionStrict)
		End Function

		<Extension>
		Public Function ReplaceTrivia(ByVal token As Microsoft.CodeAnalysis.SyntaxToken, ByVal oldTrivia As Microsoft.CodeAnalysis.SyntaxTrivia, ByVal newTrivia As Microsoft.CodeAnalysis.SyntaxTrivia) As Microsoft.CodeAnalysis.SyntaxToken
			Return SyntaxReplacer.Replace(token, Nothing, Nothing, Nothing, Nothing, DirectCast((New Microsoft.CodeAnalysis.SyntaxTrivia() { oldTrivia }), IEnumerable(Of Microsoft.CodeAnalysis.SyntaxTrivia)), Function(o As Microsoft.CodeAnalysis.SyntaxTrivia, r As Microsoft.CodeAnalysis.SyntaxTrivia) newTrivia)
		End Function

		<Extension>
		Public Function ReplaceTrivia(ByVal token As Microsoft.CodeAnalysis.SyntaxToken, ByVal trivia As IEnumerable(Of Microsoft.CodeAnalysis.SyntaxTrivia), ByVal computeReplacementTrivia As Func(Of Microsoft.CodeAnalysis.SyntaxTrivia, Microsoft.CodeAnalysis.SyntaxTrivia, Microsoft.CodeAnalysis.SyntaxTrivia)) As Microsoft.CodeAnalysis.SyntaxToken
			Return SyntaxReplacer.Replace(token, Nothing, Nothing, Nothing, Nothing, trivia, computeReplacementTrivia)
		End Function

		<Extension>
		Public Function RootNamespace(ByVal compilation As Microsoft.CodeAnalysis.Compilation) As INamespaceSymbol
			Dim namespaceSymbol As INamespaceSymbol
			Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = TryCast(compilation, Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation)
			If (visualBasicCompilation Is Nothing) Then
				namespaceSymbol = Nothing
			Else
				namespaceSymbol = visualBasicCompilation.RootNamespace
			End If
			Return namespaceSymbol
		End Function

		<Extension>
		Public Function TryGetSpeculativeSemanticModel(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, ByVal rangeArgument As Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeArgumentSyntax, <Out> ByRef speculativeModel As Microsoft.CodeAnalysis.SemanticModel) As Boolean
			Dim flag As Boolean
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				speculativeModel = Nothing
				flag = False
			Else
				flag = vBSemanticModel.TryGetSpeculativeSemanticModel(position, rangeArgument, speculativeModel)
			End If
			Return flag
		End Function

		<Extension>
		Public Function TryGetSpeculativeSemanticModel(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax, <Out> ByRef speculativeModel As Microsoft.CodeAnalysis.SemanticModel) As Boolean
			Dim flag As Boolean
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				speculativeModel = Nothing
				flag = False
			Else
				flag = vBSemanticModel.TryGetSpeculativeSemanticModel(position, statement, speculativeModel)
			End If
			Return flag
		End Function

		<Extension>
		Public Function TryGetSpeculativeSemanticModel(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax, <Out> ByRef speculativeModel As Microsoft.CodeAnalysis.SemanticModel) As Boolean
			Dim flag As Boolean
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				speculativeModel = Nothing
				flag = False
			Else
				flag = vBSemanticModel.TryGetSpeculativeSemanticModel(position, initializer, speculativeModel)
			End If
			Return flag
		End Function

		<Extension>
		Public Function TryGetSpeculativeSemanticModel(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, ByVal attribute As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax, <Out> ByRef speculativeModel As Microsoft.CodeAnalysis.SemanticModel) As Boolean
			Dim flag As Boolean
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				speculativeModel = Nothing
				flag = False
			Else
				flag = vBSemanticModel.TryGetSpeculativeSemanticModel(position, attribute, speculativeModel)
			End If
			Return flag
		End Function

		<Extension>
		Public Function TryGetSpeculativeSemanticModel(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax, <Out> ByRef speculativeModel As Microsoft.CodeAnalysis.SemanticModel, Optional ByVal bindingOption As SpeculativeBindingOption = 0) As Boolean
			Dim flag As Boolean
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				speculativeModel = Nothing
				flag = False
			Else
				flag = vBSemanticModel.TryGetSpeculativeSemanticModel(position, type, speculativeModel, bindingOption)
			End If
			Return flag
		End Function

		<Extension>
		Public Function TryGetSpeculativeSemanticModelForMethodBody(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, ByVal method As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax, <Out> ByRef speculativeModel As Microsoft.CodeAnalysis.SemanticModel) As Boolean
			Dim flag As Boolean
			Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = TryCast(semanticModel, Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel)
			If (vBSemanticModel Is Nothing) Then
				speculativeModel = Nothing
				flag = False
			Else
				flag = vBSemanticModel.TryGetSpeculativeSemanticModelForMethodBody(position, method, speculativeModel)
			End If
			Return flag
		End Function
	End Module
End Namespace