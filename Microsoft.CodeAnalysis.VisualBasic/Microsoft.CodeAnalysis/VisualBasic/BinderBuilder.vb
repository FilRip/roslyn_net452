Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class BinderBuilder
		Public Sub New()
			MyBase.New()
		End Sub

		Public Shared Function CreateBinderForAttribute(ByVal moduleSymbol As SourceModuleSymbol, ByVal tree As SyntaxTree, ByVal target As Symbol) As AttributeBinder
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			namedTypeSymbol = If(target.Kind <> SymbolKind.Parameter, target.ContainingType, target.ContainingSymbol.ContainingType)
			If (namedTypeSymbol Is Nothing) Then
				binder = BinderBuilder.CreateBinderForNamespace(moduleSymbol, tree, target.ContainingNamespace)
			Else
				binder = BinderBuilder.CreateBinderForType(moduleSymbol, tree, namedTypeSymbol)
			End If
			Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = TryCast(target, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol)
			If (sourceMethodSymbol IsNot Nothing) Then
				binder = BinderBuilder.CreateBinderForMethodDeclaration(sourceMethodSymbol, binder)
			End If
			Return New AttributeBinder(binder, tree, Nothing)
		End Function

		Public Shared Function CreateBinderForAttribute(ByVal tree As SyntaxTree, ByVal containingBinder As Binder, ByVal node As VisualBasicSyntaxNode) As AttributeBinder
			Return New AttributeBinder(containingBinder, tree, node)
		End Function

		Public Shared Function CreateBinderForDocumentationComment(ByVal containingBinder As Binder, ByVal commentedSymbol As Symbol, ByVal binderType As DocumentationCommentBinder.BinderType) As Binder
			Dim documentationCommentCrefBinder As Binder
			Select Case binderType
				Case DocumentationCommentBinder.BinderType.Cref
					documentationCommentCrefBinder = New Microsoft.CodeAnalysis.VisualBasic.DocumentationCommentCrefBinder(containingBinder, commentedSymbol)
					Exit Select
				Case DocumentationCommentBinder.BinderType.NameInTypeParamRef
					documentationCommentCrefBinder = New DocumentationCommentTypeParamRefBinder(containingBinder, commentedSymbol)
					Exit Select
				Case DocumentationCommentBinder.BinderType.NameInTypeParam
					documentationCommentCrefBinder = New DocumentationCommentTypeParamBinder(containingBinder, commentedSymbol)
					Exit Select
				Case DocumentationCommentBinder.BinderType.NameInParamOrParamRef
					documentationCommentCrefBinder = New DocumentationCommentParamBinder(containingBinder, commentedSymbol)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(binderType)
			End Select
			Return documentationCommentCrefBinder
		End Function

		Public Shared Function CreateBinderForGenericMethodDeclaration(ByVal methodSymbol As SourceMethodSymbol, ByVal containingBinder As Binder) As Binder
			Return New MethodTypeParametersBinder(containingBinder, methodSymbol.TypeParameters)
		End Function

		Public Shared Function CreateBinderForInitializer(ByVal containingBinder As Binder, ByVal fieldOrProperty As Symbol, ByVal additionalFieldsOrProperties As ImmutableArray(Of Symbol)) As Binder
			Dim declarationSyntax As VisualBasicSyntaxNode
			If (fieldOrProperty.Kind <> SymbolKind.Field) Then
				declarationSyntax = DirectCast(fieldOrProperty, SourcePropertySymbol).DeclarationSyntax
			Else
				declarationSyntax = DirectCast(fieldOrProperty, SourceFieldSymbol).DeclarationSyntax
			End If
			Return New DeclarationInitializerBinder(fieldOrProperty, additionalFieldsOrProperties, containingBinder, declarationSyntax)
		End Function

		Public Shared Function CreateBinderForMethodBody(ByVal methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal root As SyntaxNode, ByVal containingBinder As Binder) As Binder
			Dim implicitVariableBinder As Binder = BinderBuilder.CreateBinderForMethodDeclaration(methodSymbol, containingBinder)
			If (Not implicitVariableBinder.OptionExplicit) Then
				implicitVariableBinder = New Microsoft.CodeAnalysis.VisualBasic.ImplicitVariableBinder(implicitVariableBinder, methodSymbol)
			End If
			Return New MethodBodyBinder(methodSymbol, root, implicitVariableBinder)
		End Function

		Public Shared Function CreateBinderForMethodBody(ByVal moduleSymbol As SourceModuleSymbol, ByVal tree As SyntaxTree, ByVal methodSymbol As SourceMethodSymbol) As Binder
			Return BinderBuilder.CreateBinderForMethodBody(methodSymbol, methodSymbol.Syntax, BinderBuilder.CreateBinderForType(moduleSymbol, tree, methodSymbol.ContainingType))
		End Function

		Public Shared Function CreateBinderForMethodDeclaration(ByVal methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal containingBinder As Binder) As Binder
			Dim methodTypeParametersBinder As Binder
			If (Not methodSymbol.IsGenericMethod) Then
				methodTypeParametersBinder = containingBinder
			Else
				methodTypeParametersBinder = New Microsoft.CodeAnalysis.VisualBasic.MethodTypeParametersBinder(containingBinder, methodSymbol.TypeParameters)
			End If
			Return methodTypeParametersBinder
		End Function

		Public Shared Function CreateBinderForMethodDeclaration(ByVal moduleSymbol As SourceModuleSymbol, ByVal tree As SyntaxTree, ByVal methodSymbol As SourceMethodSymbol) As Binder
			Return BinderBuilder.CreateBinderForMethodDeclaration(methodSymbol, BinderBuilder.CreateBinderForType(moduleSymbol, tree, methodSymbol.ContainingType))
		End Function

		Public Shared Function CreateBinderForNamespace(ByVal moduleSymbol As SourceModuleSymbol, ByVal tree As SyntaxTree, ByVal nsSymbol As NamespaceSymbol) As Microsoft.CodeAnalysis.VisualBasic.NamespaceBinder
			Dim namespaceBinder As Microsoft.CodeAnalysis.VisualBasic.NamespaceBinder
			Dim kind As Func(Of Symbol, Boolean)
			Dim containingNamespace As NamespaceSymbol = nsSymbol.ContainingNamespace
			If (containingNamespace IsNot Nothing) Then
				Dim instance As ArrayBuilder(Of NamespaceSymbol) = ArrayBuilder(Of NamespaceSymbol).GetInstance()
				While containingNamespace IsNot Nothing
					instance.Push(nsSymbol)
					nsSymbol = containingNamespace
					containingNamespace = nsSymbol.ContainingNamespace
				End While
				Dim namespaceBinder1 As Microsoft.CodeAnalysis.VisualBasic.NamespaceBinder = BinderBuilder.CreateBinderForNamespace(moduleSymbol, tree, nsSymbol)
				While instance.Count > 0
					nsSymbol = instance.Pop()
					containingNamespace = nsSymbol.ContainingNamespace
					If (namespaceBinder1.NamespaceSymbol.Extent.Kind <> nsSymbol.Extent.Kind) Then
						Dim members As ImmutableArray(Of Symbol) = namespaceBinder1.NamespaceSymbol.GetMembers(nsSymbol.Name)
						If (BinderBuilder._Closure$__.$I5-0 Is Nothing) Then
							kind = Function(s As Symbol) s.Kind = SymbolKind.[Namespace]
							BinderBuilder._Closure$__.$I5-0 = kind
						Else
							kind = BinderBuilder._Closure$__.$I5-0
						End If
						nsSymbol = DirectCast(members.First(kind), NamespaceSymbol)
					End If
					namespaceBinder1 = New Microsoft.CodeAnalysis.VisualBasic.NamespaceBinder(namespaceBinder1, nsSymbol)
				End While
				instance.Free()
				namespaceBinder = namespaceBinder1
			Else
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForSourceFile(moduleSymbol, tree)
				namespaceBinder = New Microsoft.CodeAnalysis.VisualBasic.NamespaceBinder(binder, moduleSymbol.ContainingSourceAssembly.DeclaringCompilation.GlobalNamespace)
			End If
			Return namespaceBinder
		End Function

		Public Shared Function CreateBinderForParameterDefaultValue(ByVal moduleSymbol As SourceModuleSymbol, ByVal tree As SyntaxTree, ByVal parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol, ByVal node As VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim containingSymbol As Symbol = parameterSymbol.ContainingSymbol
			Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = TryCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol)
			binder = If(sourceMethodSymbol Is Nothing, BinderBuilder.CreateBinderForType(moduleSymbol, tree, containingSymbol.ContainingType), BinderBuilder.CreateBinderForMethodDeclaration(moduleSymbol, tree, sourceMethodSymbol))
			Return New DeclarationInitializerBinder(parameterSymbol, ImmutableArray(Of Symbol).Empty, binder, node)
		End Function

		Public Shared Function CreateBinderForParameterDefaultValue(ByVal parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol, ByVal containingBinder As Binder, ByVal node As VisualBasicSyntaxNode) As Binder
			Dim containingSymbol As SourceMethodSymbol = TryCast(parameterSymbol.ContainingSymbol, SourceMethodSymbol)
			If (containingSymbol IsNot Nothing) Then
				containingBinder = BinderBuilder.CreateBinderForMethodDeclaration(containingSymbol, containingBinder)
			End If
			Return New DeclarationInitializerBinder(parameterSymbol, ImmutableArray(Of Symbol).Empty, containingBinder, node)
		End Function

		Public Shared Function CreateBinderForProjectImports(ByVal moduleSymbol As SourceModuleSymbol, ByVal tree As SyntaxTree) As Binder
			Dim ignoreBaseClassesBinder As Binder = New Microsoft.CodeAnalysis.VisualBasic.IgnoreBaseClassesBinder(New NamespaceBinder(New ProjectImportsBinder(BinderBuilder.CreateSourceModuleBinder(moduleSymbol), tree), moduleSymbol.ContainingSourceAssembly.DeclaringCompilation.GlobalNamespace))
			Return New LocationSpecificBinder(BindingLocation.ProjectImportsDeclaration, ignoreBaseClassesBinder)
		End Function

		Public Shared Function CreateBinderForProjectLevelNamespace(ByVal moduleSymbol As SourceModuleSymbol, ByVal tree As SyntaxTree) As Binder
			Return BinderBuilder.CreateBinderForNamespace(moduleSymbol, tree, moduleSymbol.RootNamespace)
		End Function

		Private Shared Function CreateBinderForSourceFile(ByVal moduleSymbol As SourceModuleSymbol, ByVal tree As SyntaxTree) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim typesOfImportedNamespacesMembersBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateSourceModuleBinder(moduleSymbol)
			Dim memberImports As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition) = moduleSymbol.MemberImports
			If (memberImports.Length > 0) Then
				typesOfImportedNamespacesMembersBinder = New Microsoft.CodeAnalysis.VisualBasic.TypesOfImportedNamespacesMembersBinder(typesOfImportedNamespacesMembersBinder, memberImports)
				typesOfImportedNamespacesMembersBinder = New ImportedTypesAndNamespacesMembersBinder(typesOfImportedNamespacesMembersBinder, memberImports)
			End If
			Dim aliasImportsMap As Dictionary(Of String, AliasAndImportsClausePosition) = moduleSymbol.AliasImportsMap
			If (aliasImportsMap IsNot Nothing) Then
				typesOfImportedNamespacesMembersBinder = New ImportAliasesBinder(typesOfImportedNamespacesMembersBinder, aliasImportsMap)
			End If
			Dim xmlNamespaces As Dictionary(Of String, XmlNamespaceAndImportsClausePosition) = moduleSymbol.XmlNamespaces
			If (xmlNamespaces IsNot Nothing) Then
				typesOfImportedNamespacesMembersBinder = New XmlNamespaceImportsBinder(typesOfImportedNamespacesMembersBinder, xmlNamespaces)
			End If
			Dim sourceFile As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFile = moduleSymbol.TryGetSourceFile(tree)
			If (sourceFile IsNot Nothing) Then
				Dim sourceFileBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = New Microsoft.CodeAnalysis.VisualBasic.SourceFileBinder(typesOfImportedNamespacesMembersBinder, sourceFile, tree)
				Dim namespaceOrTypeAndImportsClausePositions As ImmutableArray(Of NamespaceOrTypeAndImportsClausePosition) = sourceFile.MemberImports
				If (Not namespaceOrTypeAndImportsClausePositions.IsEmpty) Then
					sourceFileBinder = New Microsoft.CodeAnalysis.VisualBasic.TypesOfImportedNamespacesMembersBinder(sourceFileBinder, namespaceOrTypeAndImportsClausePositions)
					sourceFileBinder = New ImportedTypesAndNamespacesMembersBinder(sourceFileBinder, namespaceOrTypeAndImportsClausePositions)
				End If
				Dim aliasImportsOpt As IReadOnlyDictionary(Of String, AliasAndImportsClausePosition) = sourceFile.AliasImportsOpt
				If (aliasImportsOpt IsNot Nothing) Then
					sourceFileBinder = New ImportAliasesBinder(sourceFileBinder, aliasImportsOpt)
				End If
				Dim xmlNamespacesOpt As IReadOnlyDictionary(Of String, XmlNamespaceAndImportsClausePosition) = sourceFile.XmlNamespacesOpt
				If (xmlNamespacesOpt IsNot Nothing) Then
					sourceFileBinder = New XmlNamespaceImportsBinder(sourceFileBinder, xmlNamespacesOpt)
				End If
				binder = sourceFileBinder
			Else
				binder = typesOfImportedNamespacesMembersBinder
			End If
			Return binder
		End Function

		Public Shared Function CreateBinderForSourceFileImports(ByVal moduleSymbol As SourceModuleSymbol, ByVal tree As SyntaxTree) As Binder
			Dim ignoreBaseClassesBinder As Binder = New Microsoft.CodeAnalysis.VisualBasic.IgnoreBaseClassesBinder(New NamespaceBinder(New SourceFileBinder(BinderBuilder.CreateSourceModuleBinder(moduleSymbol), moduleSymbol.TryGetSourceFile(tree), tree), moduleSymbol.ContainingSourceAssembly.DeclaringCompilation.GlobalNamespace))
			Return New LocationSpecificBinder(BindingLocation.SourceFileImportsDeclaration, ignoreBaseClassesBinder)
		End Function

		Public Shared Function CreateBinderForType(ByVal moduleSymbol As SourceModuleSymbol, ByVal tree As SyntaxTree, ByVal typeSymbol As NamedTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim namedTypeBinder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim containingSymbol As Symbol = typeSymbol.ContainingSymbol
			If (containingSymbol.Kind <> SymbolKind.[Namespace]) Then
				Dim instance As ArrayBuilder(Of NamedTypeSymbol) = ArrayBuilder(Of NamedTypeSymbol).GetInstance()
				instance.Push(typeSymbol)
				While containingSymbol.Kind <> SymbolKind.[Namespace]
					typeSymbol = DirectCast(containingSymbol, NamedTypeSymbol)
					containingSymbol = typeSymbol.ContainingSymbol
					instance.Push(typeSymbol)
				End While
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForNamespace(moduleSymbol, tree, DirectCast(containingSymbol, NamespaceSymbol))
				While instance.Count > 0
					typeSymbol = instance.Pop()
					binder = New Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder(binder, typeSymbol)
				End While
				instance.Free()
				namedTypeBinder = binder
			Else
				namedTypeBinder = New Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder(BinderBuilder.CreateBinderForNamespace(moduleSymbol, tree, DirectCast(containingSymbol, NamespaceSymbol)), typeSymbol)
			End If
			Return namedTypeBinder
		End Function

		Public Shared Function CreateSourceModuleBinder(ByVal moduleSymbol As SourceModuleSymbol) As Binder
			Return New SourceModuleBinder(New BackstopBinder(), moduleSymbol)
		End Function
	End Class
End Namespace