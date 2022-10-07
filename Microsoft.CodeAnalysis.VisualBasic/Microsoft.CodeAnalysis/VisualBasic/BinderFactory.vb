Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class BinderFactory
		Private ReadOnly _sourceModule As SourceModuleSymbol

		Private ReadOnly _tree As SyntaxTree

		Private ReadOnly _cache As ConcurrentDictionary(Of ValueTuple(Of VisualBasicSyntaxNode, Byte), Binder)

		Private ReadOnly _binderFactoryVisitorPool As ObjectPool(Of BinderFactory.BinderFactoryVisitor)

		Private ReadOnly Property InScript As Boolean
			Get
				Return Me._tree.Options.Kind = SourceCodeKind.Script
			End Get
		End Property

		Public Sub New(ByVal sourceModule As SourceModuleSymbol, ByVal tree As SyntaxTree)
			MyBase.New()
			Me._sourceModule = sourceModule
			Me._tree = tree
			Me._cache = New ConcurrentDictionary(Of ValueTuple(Of VisualBasicSyntaxNode, Byte), Binder)()
			Me._binderFactoryVisitorPool = New ObjectPool(Of BinderFactory.BinderFactoryVisitor)(Function() New BinderFactory.BinderFactoryVisitor(Me))
		End Sub

		Private Function BuildAttributeBinder(ByVal containingBinder As Binder, ByVal node As VisualBasicSyntaxNode) As Binder
			If (containingBinder IsNot Nothing AndAlso node.Parent IsNot Nothing) Then
				Dim parent As VisualBasicSyntaxNode = node.Parent
				If (parent.Parent IsNot Nothing AndAlso CUShort(parent.Parent.Kind()) - CUShort(SyntaxKind.ModuleStatement) <= 4 AndAlso TypeOf containingBinder Is NamedTypeBinder) Then
					containingBinder = containingBinder.ContainingBinder
				End If
			End If
			Return BinderBuilder.CreateBinderForAttribute(Me._tree, containingBinder, node)
		End Function

		Private Function BuildInitializerBinder(ByVal containingBinder As Binder, ByVal fieldOrProperty As Symbol, ByVal additionalFieldsOrProperties As ImmutableArray(Of Symbol)) As Binder
			Return BinderBuilder.CreateBinderForInitializer(containingBinder, fieldOrProperty, additionalFieldsOrProperties)
		End Function

		Private Function BuildMethodBinder(ByVal containingBinder As NamedTypeBinder, ByVal methodSyntax As MethodBaseSyntax, ByVal forBody As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim containingType As NamedTypeSymbol = containingBinder.ContainingType
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol.FindSymbolFromSyntax(methodSyntax, Me._tree, containingType)
			If (symbol Is Nothing OrElse symbol.Kind <> SymbolKind.Method) Then
				binder = containingBinder
			Else
				Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol)
				binder = If(Not forBody, BinderBuilder.CreateBinderForMethodDeclaration(sourceMethodSymbol, containingBinder), BinderBuilder.CreateBinderForMethodBody(sourceMethodSymbol, sourceMethodSymbol.Syntax, containingBinder))
			End If
			Return binder
		End Function

		Private Function BuildNamespaceBinder(ByVal containingBinder As Microsoft.CodeAnalysis.VisualBasic.NamespaceBinder, ByVal childName As NameSyntax, ByVal globalNamespaceAllowed As Boolean) As Microsoft.CodeAnalysis.VisualBasic.NamespaceBinder
			Dim namespaceBinder As Microsoft.CodeAnalysis.VisualBasic.NamespaceBinder
			Dim valueText As String
			Select Case childName.Kind()
				Case SyntaxKind.IdentifierName
					valueText = DirectCast(childName, IdentifierNameSyntax).Identifier.ValueText
					Exit Select
				Case SyntaxKind.GenericName
					Throw ExceptionUtilities.UnexpectedValue(childName.Kind())
				Case SyntaxKind.QualifiedName
					Dim qualifiedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax = DirectCast(childName, Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax)
					containingBinder = Me.BuildNamespaceBinder(containingBinder, qualifiedNameSyntax.Left, globalNamespaceAllowed)
					valueText = qualifiedNameSyntax.Right.Identifier.ValueText
					Exit Select
				Case SyntaxKind.GlobalName
					If (Not globalNamespaceAllowed) Then
						valueText = "Global"
						Exit Select
					Else
						namespaceBinder = BinderBuilder.CreateBinderForNamespace(Me._sourceModule, Me._tree, Me._sourceModule.GlobalNamespace)
						Return namespaceBinder
					End If
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(childName.Kind())
			End Select
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = containingBinder.NamespaceSymbol.GetMembers(valueText).GetEnumerator()
			While enumerator.MoveNext()
				Dim current As NamespaceSymbol = TryCast(DirectCast(enumerator.Current, NamespaceOrTypeSymbol), NamespaceSymbol)
				If (current Is Nothing) Then
					Continue While
				End If
				namespaceBinder = New Microsoft.CodeAnalysis.VisualBasic.NamespaceBinder(containingBinder, current)
				Return namespaceBinder
			End While
			Throw ExceptionUtilities.Unreachable
			Return namespaceBinder
		End Function

		Private Function CreateBinderForNodeAndUsage(ByVal node As VisualBasicSyntaxNode, ByVal usage As BinderFactory.NodeUsage, ByVal containingBinder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim namedTypeBinder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim sourceScriptClass As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim empty As BasesBeingResolved
			Dim containingNamedTypeBinderForMemberNode As Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder
			Dim enumerator As IEnumerator(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax) = Nothing
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim namedTypeBinder1 As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder1 As Microsoft.CodeAnalysis.VisualBasic.Binder
			Select Case usage
				Case BinderFactory.NodeUsage.CompilationUnit
					namedTypeBinder = BinderBuilder.CreateBinderForNamespace(Me._sourceModule, Me._tree, Me._sourceModule.RootNamespace)
					Exit Select
				Case BinderFactory.NodeUsage.ImplicitClass
					If (node.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit OrElse Me._tree.Options.Kind = SourceCodeKind.Regular) Then
						sourceScriptClass = DirectCast(containingBinder.ContainingNamespaceOrType.GetMembers("<invalid-global-code>").[Single](), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					Else
						sourceScriptClass = Me._sourceModule.ContainingSourceAssembly.DeclaringCompilation.SourceScriptClass
					End If
					namedTypeBinder = New Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder(containingBinder, sourceScriptClass)
					Exit Select
				Case BinderFactory.NodeUsage.ScriptCompilationUnit
					namedTypeBinder = New Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder(Me.GetBinderForNodeAndUsage(node, BinderFactory.NodeUsage.CompilationUnit, Nothing, -1, Nothing), Me._sourceModule.ContainingSourceAssembly.DeclaringCompilation.SourceScriptClass)
					Exit Select
				Case BinderFactory.NodeUsage.TopLevelExecutableStatement
					namedTypeBinder = New TopLevelCodeBinder(containingBinder.ContainingType.InstanceConstructors.[Single](), containingBinder)
					Exit Select
				Case BinderFactory.NodeUsage.ImportsStatement
					namedTypeBinder = BinderBuilder.CreateBinderForSourceFileImports(Me._sourceModule, Me._tree)
					Exit Select
				Case BinderFactory.NodeUsage.NamespaceBlockInterior
					Dim namespaceBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax)
					Dim binderForNodeAndUsage As NamespaceBinder = TryCast(containingBinder, NamespaceBinder)
					If (binderForNodeAndUsage Is Nothing) Then
						Dim namedTypeBinder2 As Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder = TryCast(containingBinder, Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder)
						If (namedTypeBinder2 IsNot Nothing AndAlso namedTypeBinder2.ContainingType.IsScriptClass) Then
							binderForNodeAndUsage = DirectCast(Me.GetBinderForNodeAndUsage(node, BinderFactory.NodeUsage.CompilationUnit, Nothing, -1, Nothing), NamespaceBinder)
						End If
					End If
					If (binderForNodeAndUsage Is Nothing) Then
						namedTypeBinder = containingBinder
						Exit Select
					Else
						namedTypeBinder = Me.BuildNamespaceBinder(binderForNodeAndUsage, namespaceBlockSyntax.NamespaceStatement.Name, namespaceBlockSyntax.Parent.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit)
						Exit Select
					End If
				Case BinderFactory.NodeUsage.TypeBlockFull
					Dim sourceNamedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol = SourceMemberContainerTypeSymbol.FindSymbolFromSyntax(DirectCast(node, TypeStatementSyntax), containingBinder.ContainingNamespaceOrType, Me._sourceModule)
					If (sourceNamedTypeSymbol IsNot Nothing) Then
						binder = New Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder(containingBinder, sourceNamedTypeSymbol)
					Else
						binder = containingBinder
					End If
					namedTypeBinder = binder
					Exit Select
				Case BinderFactory.NodeUsage.EnumBlockFull
					Dim sourceNamedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol = SourceMemberContainerTypeSymbol.FindSymbolFromSyntax(DirectCast(node, EnumStatementSyntax), containingBinder.ContainingNamespaceOrType, Me._sourceModule)
					If (sourceNamedTypeSymbol1 IsNot Nothing) Then
						namedTypeBinder1 = New Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder(containingBinder, sourceNamedTypeSymbol1)
					Else
						namedTypeBinder1 = containingBinder
					End If
					namedTypeBinder = namedTypeBinder1
					Exit Select
				Case BinderFactory.NodeUsage.DelegateDeclaration
					Dim sourceNamedTypeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol = SourceMemberContainerTypeSymbol.FindSymbolFromSyntax(DirectCast(node, DelegateStatementSyntax), containingBinder.ContainingNamespaceOrType, Me._sourceModule)
					If (sourceNamedTypeSymbol2 IsNot Nothing) Then
						binder1 = New Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder(containingBinder, sourceNamedTypeSymbol2)
					Else
						binder1 = containingBinder
					End If
					namedTypeBinder = binder1
					Exit Select
				Case BinderFactory.NodeUsage.InheritsStatement
					Dim namedTypeBinder3 As Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder = TryCast(containingBinder, Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder)
					If (namedTypeBinder3 Is Nothing) Then
						namedTypeBinder = containingBinder
						Exit Select
					Else
						empty = BasesBeingResolved.Empty
						namedTypeBinder = New BasesBeingResolvedBinder(containingBinder, empty.PrependInheritsBeingResolved(namedTypeBinder3.ContainingType))
						Exit Select
					End If
				Case BinderFactory.NodeUsage.ImplementsStatement
					Dim namedTypeBinder4 As Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder = TryCast(containingBinder, Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder)
					If (namedTypeBinder4 Is Nothing) Then
						namedTypeBinder = containingBinder
						Exit Select
					Else
						empty = BasesBeingResolved.Empty
						namedTypeBinder = New BasesBeingResolvedBinder(containingBinder, empty.PrependImplementsBeingResolved(namedTypeBinder4.ContainingType))
						Exit Select
					End If
				Case BinderFactory.NodeUsage.MethodFull
				Case BinderFactory.NodeUsage.MethodInterior
					Dim methodBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax)
					Dim containingNamedTypeBinderForMemberNode1 As Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder = Me.GetContainingNamedTypeBinderForMemberNode(node.Parent.Parent, containingBinder)
					If (containingNamedTypeBinderForMemberNode1 IsNot Nothing) Then
						Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = methodBaseSyntax.Kind()
						If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) Then
							Select Case syntaxKind
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement
									Exit Select
								Case Else
									namedTypeBinder = containingBinder
									Return namedTypeBinder
							End Select
						End If
						namedTypeBinder = Me.BuildMethodBinder(containingNamedTypeBinderForMemberNode1, methodBaseSyntax, usage = BinderFactory.NodeUsage.MethodInterior)
						Exit Select
					Else
						namedTypeBinder = containingBinder
						Exit Select
					End If
				Case BinderFactory.NodeUsage.FieldOrPropertyInitializer
					Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Nothing
					Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Empty
					Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
					If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration) Then
						Dim enumMemberDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax)
						containingNamedTypeBinderForMemberNode = DirectCast(containingBinder, Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder)
						Dim identifier As Microsoft.CodeAnalysis.SyntaxToken = enumMemberDeclarationSyntax.Identifier
						symbol = containingNamedTypeBinderForMemberNode.ContainingType.FindMember(identifier.ValueText, SymbolKind.Field, identifier.Span, Me._tree)
					ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement) Then
						Dim propertyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax)
						containingNamedTypeBinderForMemberNode = Me.GetContainingNamedTypeBinderForMemberNode(node.Parent, containingBinder)
						If (containingNamedTypeBinderForMemberNode IsNot Nothing) Then
							Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = propertyStatementSyntax.Identifier
							symbol = containingNamedTypeBinderForMemberNode.ContainingType.FindMember(syntaxToken.ValueText, SymbolKind.[Property], syntaxToken.Span, Me._tree)
						Else
							namedTypeBinder = Nothing
							Exit Select
						End If
					Else
						If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableDeclarator) Then
							Throw ExceptionUtilities.UnexpectedValue(node.Kind())
						End If
						Dim variableDeclaratorSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)
						containingNamedTypeBinderForMemberNode = Me.GetContainingNamedTypeBinderForMemberNode(node.Parent.Parent, containingBinder)
						If (containingNamedTypeBinderForMemberNode IsNot Nothing) Then
							Dim identifier1 As Microsoft.CodeAnalysis.SyntaxToken = variableDeclaratorSyntax.Names(0).Identifier
							symbol = containingNamedTypeBinderForMemberNode.ContainingType.FindFieldOrProperty(identifier1.ValueText, identifier1.Span, Me._tree)
							If (variableDeclaratorSyntax.Names.Count > 1) Then
								Using instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).GetInstance()
									enumerator = DirectCast(variableDeclaratorSyntax.Names, IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)).Skip(1).GetEnumerator()
									While enumerator.MoveNext()
										identifier1 = enumerator.Current.Identifier
										Dim symbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = containingNamedTypeBinderForMemberNode.ContainingType.FindFieldOrProperty(identifier1.ValueText, identifier1.Span, Me._tree)
										instance.Add(symbol1)
									End While
								End Using
								immutableAndFree = instance.ToImmutableAndFree()
							End If
						Else
							namedTypeBinder = Nothing
							Exit Select
						End If
					End If
					If (symbol Is Nothing) Then
						namedTypeBinder = Nothing
						Exit Select
					Else
						namedTypeBinder = Me.BuildInitializerBinder(containingNamedTypeBinderForMemberNode, symbol, immutableAndFree)
						Exit Select
					End If
				Case BinderFactory.NodeUsage.FieldArrayBounds
					Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)
					Dim namedTypeBinder5 As Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder = TryCast(containingBinder, Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder)
					If (namedTypeBinder5 IsNot Nothing) Then
						Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = namedTypeBinder5.ContainingType
						Dim syntaxToken1 As Microsoft.CodeAnalysis.SyntaxToken = modifiedIdentifierSyntax.Identifier
						Dim symbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbol = containingType.FindMember(syntaxToken1.ValueText, SymbolKind.Field, syntaxToken1.Span, Me._tree)
						If (symbol2 IsNot Nothing) Then
							namedTypeBinder = Me.BuildInitializerBinder(namedTypeBinder5, symbol2, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Empty)
							Exit Select
						End If
					End If
					namedTypeBinder = Nothing
					Exit Select
				Case BinderFactory.NodeUsage.Attribute
					namedTypeBinder = Me.BuildAttributeBinder(containingBinder, node)
					Exit Select
				Case BinderFactory.NodeUsage.ParameterDefaultValue
					Dim parameterSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax)
					If (parameterSyntax.[Default] IsNot Nothing) Then
						Dim parent As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax = DirectCast(DirectCast(parameterSyntax.Parent, ParameterListSyntax).Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax)
						Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = Nothing
						Dim syntaxKind2 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
						Select Case syntaxKind2
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement
								Dim parameterDeclarationContainingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = BinderFactory.GetParameterDeclarationContainingType(containingBinder)
								If (parameterDeclarationContainingType Is Nothing) Then
									Exit Select
								End If
								Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol.FindSymbolFromSyntax(parent, Me._tree, parameterDeclarationContainingType), Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol)
								If (sourceMethodSymbol Is Nothing) Then
									Exit Select
								End If
								parameterSymbol = sourceMethodSymbol.Parameters.GetParameterSymbol(parameterSyntax)
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement
								Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = BinderFactory.GetParameterDeclarationContainingType(containingBinder)
								If (namedTypeSymbol Is Nothing OrElse namedTypeSymbol.TypeKind <> TypeKind.[Delegate]) Then
									Exit Select
								End If
								parameterSymbol = namedTypeSymbol.DelegateInvokeMethod.Parameters.GetParameterSymbol(parameterSyntax)
								Exit Select
							Case 100
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
								Throw ExceptionUtilities.UnexpectedValue(parent.Kind())
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement
								Dim parameterDeclarationContainingType1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = BinderFactory.GetParameterDeclarationContainingType(containingBinder)
								If (parameterDeclarationContainingType1 Is Nothing) Then
									Exit Select
								End If
								Dim sourceEventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEventSymbol = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol.FindSymbolFromSyntax(parent, Me._tree, parameterDeclarationContainingType1), Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEventSymbol)
								If (sourceEventSymbol Is Nothing) Then
									Exit Select
								End If
								parameterSymbol = sourceEventSymbol.DelegateParameters.GetParameterSymbol(parameterSyntax)
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement
								Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = BinderFactory.GetParameterDeclarationContainingType(containingBinder)
								If (namedTypeSymbol1 Is Nothing) Then
									Exit Select
								End If
								Dim sourcePropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol.FindSymbolFromSyntax(parent, Me._tree, namedTypeSymbol1), Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol)
								If (sourcePropertySymbol Is Nothing) Then
									Exit Select
								End If
								parameterSymbol = sourcePropertySymbol.Parameters.GetParameterSymbol(parameterSyntax)
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement
								namedTypeBinder = Nothing
								Return namedTypeBinder
							Case Else
								If (CUShort(syntaxKind2) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubLambdaHeader) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
									namedTypeBinder = Nothing
									Return namedTypeBinder
								End If
								Throw ExceptionUtilities.UnexpectedValue(parent.Kind())
						End Select
						If (parameterSymbol IsNot Nothing) Then
							namedTypeBinder = BinderBuilder.CreateBinderForParameterDefaultValue(parameterSymbol, containingBinder, parameterSyntax)
							Exit Select
						End If
					End If
					namedTypeBinder = Nothing
					Exit Select
				Case BinderFactory.NodeUsage.PropertyFull
					namedTypeBinder = Me.GetContainingNamedTypeBinderForMemberNode(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax).Parent.Parent, containingBinder)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(usage)
			End Select
			Return namedTypeBinder
		End Function

		Private Function CreateDocumentationCommentBinder(ByVal node As DocumentationCommentTriviaSyntax, ByVal binderType As DocumentationCommentBinder.BinderType) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binderAtOrAbove As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim parent As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = DirectCast(node.ParentTrivia.Token.Parent, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = Nothing
			While True
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration) Then
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement
							visualBasicSyntaxNode = parent.Parent
							If (visualBasicSyntaxNode Is Nothing OrElse Not TypeOf visualBasicSyntaxNode Is MethodBlockBaseSyntax) Then
								GoTo Label1
							End If
							visualBasicSyntaxNode = visualBasicSyntaxNode.Parent

						Case 100
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement
							visualBasicSyntaxNode = parent.Parent
							If (visualBasicSyntaxNode Is Nothing OrElse visualBasicSyntaxNode.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement) Then
								GoTo Label1
							End If
							visualBasicSyntaxNode = visualBasicSyntaxNode.Parent

						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement
							visualBasicSyntaxNode = parent.Parent
							If (visualBasicSyntaxNode Is Nothing OrElse visualBasicSyntaxNode.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock) Then
								GoTo Label1
							End If
							visualBasicSyntaxNode = visualBasicSyntaxNode.Parent

						Case Else
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration) Then
								GoTo Label0
							End If
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeList) Then
								visualBasicSyntaxNode = parent.Parent
								If (visualBasicSyntaxNode Is Nothing) Then
									GoTo Label1
								End If
								parent = visualBasicSyntaxNode
								visualBasicSyntaxNode = Nothing
								Continue While
							End If

					End Select
				ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement) <= 4) Then
					visualBasicSyntaxNode = parent
					Exit While
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration) Then
						GoTo Label0
					End If
					Exit While
				End If
			End While
		Label1:
			If (visualBasicSyntaxNode IsNot Nothing) Then
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.GetBinderAtOrAbove(visualBasicSyntaxNode, parent.SpanStart)
				Dim containingNamespaceOrType As Symbol = Nothing
				Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
				If (syntaxKind1 > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration) Then
					Select Case syntaxKind1
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement
							If (binder.ContainingType Is Nothing) Then
								Exit Select
							End If
							containingNamespaceOrType = SourceMethodSymbol.FindSymbolFromSyntax(DirectCast(parent, MethodBaseSyntax), Me._tree, binder.ContainingType)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement
							If (binder.ContainingType Is Nothing) Then
								containingNamespaceOrType = SourceMemberContainerTypeSymbol.FindSymbolFromSyntax(DirectCast(parent, DelegateStatementSyntax), binder.ContainingNamespaceOrType, Me._sourceModule)
								Exit Select
							Else
								containingNamespaceOrType = SourceMethodSymbol.FindSymbolFromSyntax(DirectCast(parent, MethodBaseSyntax), Me._tree, binder.ContainingType)
								Exit Select
							End If
						Case 100
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
							Throw ExceptionUtilities.UnexpectedValue(parent.Kind())
						Case Else
							If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration) Then
								Exit Select
							End If
							Throw ExceptionUtilities.UnexpectedValue(parent.Kind())
					End Select
				Else
					Select Case syntaxKind1
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement
							containingNamespaceOrType = binder.ContainingNamespaceOrType
							Exit Select
						Case Else
							If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration) Then
								Exit Select
							End If
							Throw ExceptionUtilities.UnexpectedValue(parent.Kind())
					End Select
				End If
				binderAtOrAbove = BinderBuilder.CreateBinderForDocumentationComment(binder, containingNamespaceOrType, binderType)
			Else
				binderAtOrAbove = Me.GetBinderAtOrAbove(parent, parent.SpanStart)
			End If
			Return binderAtOrAbove
		Label0:
			visualBasicSyntaxNode = parent.Parent
			GoTo Label1
		End Function

		Private Function GetBinderAtOrAbove(ByVal node As SyntaxNode, ByVal position As Integer) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			While True
				binder = Me.MakeBinder(node, position)
				If (binder IsNot Nothing) Then
					Exit While
				End If
				If (node.Kind() <> SyntaxKind.DocumentationCommentTrivia) Then
					node = node.Parent
				Else
					node = DirectCast(DirectCast(node, StructuredTriviaSyntax).ParentTrivia.Token.Parent, VisualBasicSyntaxNode)
				End If
			End While
			Return binder
		End Function

		Private Function GetBinderForNodeAndUsage(ByVal node As VisualBasicSyntaxNode, ByVal usage As BinderFactory.NodeUsage, Optional ByVal parentNode As VisualBasicSyntaxNode = Nothing, Optional ByVal position As Integer = -1, Optional ByVal containingBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing
			Dim valueTuple As ValueTuple(Of VisualBasicSyntaxNode, Byte) = New ValueTuple(Of VisualBasicSyntaxNode, Byte)(node, usage)
			If (Not Me._cache.TryGetValue(valueTuple, binder)) Then
				If (containingBinder Is Nothing AndAlso parentNode IsNot Nothing) Then
					containingBinder = Me.GetBinderAtOrAbove(parentNode, position)
				End If
				binder = Me.CreateBinderForNodeAndUsage(node, usage, containingBinder)
				Me._cache.TryAdd(valueTuple, binder)
			End If
			Return binder
		End Function

		Public Function GetBinderForPosition(ByVal node As SyntaxNode, ByVal position As Integer) As Binder
			Return Me.GetBinderAtOrAbove(node, position)
		End Function

		Private Function GetContainingNamedTypeBinderForMemberNode(ByVal node As VisualBasicSyntaxNode, ByVal containingBinder As Binder) As Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder
			Dim binderForNodeAndUsage As Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder
			Dim namedTypeBinder As Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder = TryCast(containingBinder, Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder)
			If (namedTypeBinder IsNot Nothing) Then
				binderForNodeAndUsage = namedTypeBinder
			ElseIf (node Is Nothing OrElse node.Kind() <> SyntaxKind.NamespaceBlock AndAlso node.Kind() <> SyntaxKind.CompilationUnit) Then
				binderForNodeAndUsage = Nothing
			Else
				binderForNodeAndUsage = DirectCast(Me.GetBinderForNodeAndUsage(node, BinderFactory.NodeUsage.ImplicitClass, Nothing, -1, containingBinder), Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder)
			End If
			Return binderForNodeAndUsage
		End Function

		Public Function GetNamedTypeBinder(ByVal node As TypeStatementSyntax) As Binder
			Dim parent As TypeBlockSyntax = TryCast(node.Parent, TypeBlockSyntax)
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = If(parent IsNot Nothing, parent.Parent, node.Parent)
			Return Me.GetBinderForNodeAndUsage(node, BinderFactory.NodeUsage.TypeBlockFull, visualBasicSyntaxNode, node.SpanStart, Nothing)
		End Function

		Public Function GetNamedTypeBinder(ByVal node As EnumStatementSyntax) As Binder
			Dim parent As EnumBlockSyntax = TryCast(node.Parent, EnumBlockSyntax)
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = If(parent IsNot Nothing, parent.Parent, node.Parent)
			Return Me.GetBinderForNodeAndUsage(node, BinderFactory.NodeUsage.EnumBlockFull, visualBasicSyntaxNode, node.SpanStart, Nothing)
		End Function

		Public Function GetNamedTypeBinder(ByVal node As DelegateStatementSyntax) As Binder
			Return Me.GetBinderForNodeAndUsage(node, BinderFactory.NodeUsage.DelegateDeclaration, node.Parent, node.SpanStart, Nothing)
		End Function

		Public Function GetNamespaceBinder(ByVal node As NamespaceBlockSyntax) As Binder
			Return Me.GetBinderForNodeAndUsage(node, BinderFactory.NodeUsage.NamespaceBlockInterior, node.Parent, node.SpanStart, Nothing)
		End Function

		Private Shared Function GetParameterDeclarationContainingType(ByVal containingBinder As Binder) As NamedTypeSymbol
			Dim containingType As NamedTypeSymbol
			Dim namedTypeBinder As Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder = TryCast(containingBinder, Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder)
			If (namedTypeBinder Is Nothing) Then
				Dim methodTypeParametersBinder As Microsoft.CodeAnalysis.VisualBasic.MethodTypeParametersBinder = TryCast(containingBinder, Microsoft.CodeAnalysis.VisualBasic.MethodTypeParametersBinder)
				If (methodTypeParametersBinder Is Nothing) Then
					containingType = Nothing
					Return containingType
				End If
				namedTypeBinder = DirectCast(methodTypeParametersBinder.ContainingBinder, Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder)
			End If
			containingType = namedTypeBinder.ContainingType
			Return containingType
		End Function

		Private Function MakeBinder(ByVal node As SyntaxNode, ByVal position As Integer) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			If (SyntaxFacts.InSpanOrEffectiveTrailingOfNode(node, position) OrElse node.Kind() = SyntaxKind.CompilationUnit) Then
				Dim binderFactoryVisitor As BinderFactory.BinderFactoryVisitor = Me._binderFactoryVisitorPool.Allocate()
				binderFactoryVisitor.Position = position
				Dim binder1 As Microsoft.CodeAnalysis.VisualBasic.Binder = binderFactoryVisitor.Visit(node)
				Me._binderFactoryVisitorPool.Free(binderFactoryVisitor)
				binder = binder1
			Else
				binder = Nothing
			End If
			Return binder
		End Function

		Private NotInheritable Class BinderFactoryVisitor
			Inherits VisualBasicSyntaxVisitor(Of Binder)
			Private _position As Integer

			Private ReadOnly _factory As BinderFactory

			Friend WriteOnly Property Position As Integer
				Set(ByVal value As Integer)
					Me._position = value
				End Set
			End Property

			Public Sub New(ByVal factory As BinderFactory)
				MyBase.New()
				Me._factory = factory
			End Sub

			Public Overrides Function DefaultVisit(ByVal node As SyntaxNode) As Binder
				Dim binderForNodeAndUsage As Binder
				If (Not Me._factory.InScript OrElse node.Parent.Kind() <> SyntaxKind.CompilationUnit) Then
					binderForNodeAndUsage = Nothing
				Else
					binderForNodeAndUsage = Me.GetBinderForNodeAndUsage(DirectCast(node.Parent, VisualBasicSyntaxNode), BinderFactory.NodeUsage.TopLevelExecutableStatement, DirectCast(node.Parent, VisualBasicSyntaxNode), Me._position)
				End If
				Return binderForNodeAndUsage
			End Function

			Private Function GetBinderForNodeAndUsage(ByVal node As VisualBasicSyntaxNode, ByVal usage As BinderFactory.NodeUsage, Optional ByVal parentNode As VisualBasicSyntaxNode = Nothing, Optional ByVal position As Integer = -1) As Binder
				Return Me._factory.GetBinderForNodeAndUsage(node, usage, parentNode, position, Nothing)
			End Function

			Private Shared Function IsNotNothingAndContains(ByVal nodeOpt As VisualBasicSyntaxNode, ByVal position As Integer) As Boolean
				If (nodeOpt Is Nothing) Then
					Return False
				End If
				Return SyntaxFacts.InSpanOrEffectiveTrailingOfNode(nodeOpt, position)
			End Function

			Public Overrides Function VisitAccessorBlock(ByVal node As AccessorBlockSyntax) As Binder
				Return Me.VisitMethodBlockBase(node, node.BlockStatement)
			End Function

			Public Overrides Function VisitAccessorStatement(ByVal node As AccessorStatementSyntax) As Binder
				Return Me.VisitMethodBaseDeclaration(node)
			End Function

			Public Overrides Function VisitAttribute(ByVal node As AttributeSyntax) As Binder
				Return Me.GetBinderForNodeAndUsage(node, BinderFactory.NodeUsage.Attribute, node.Parent, Me._position)
			End Function

			Public Overrides Function VisitClassBlock(ByVal classSyntax As ClassBlockSyntax) As Binder
				Return Me.GetBinderForNodeAndUsage(classSyntax.BlockStatement, BinderFactory.NodeUsage.TypeBlockFull, classSyntax.Parent, Me._position)
			End Function

			Public Overrides Function VisitCompilationUnit(ByVal node As CompilationUnitSyntax) As Binder
				Return Me.GetBinderForNodeAndUsage(node, If(Me._factory.InScript, BinderFactory.NodeUsage.ScriptCompilationUnit, BinderFactory.NodeUsage.CompilationUnit), Nothing, -1)
			End Function

			Public Overrides Function VisitConstructorBlock(ByVal node As ConstructorBlockSyntax) As Binder
				Return Me.VisitMethodBlockBase(node, node.BlockStatement)
			End Function

			Public Overrides Function VisitDeclareStatement(ByVal node As DeclareStatementSyntax) As Binder
				Return Me.VisitMethodBaseDeclaration(node)
			End Function

			Public Overrides Function VisitDelegateStatement(ByVal delegateSyntax As DelegateStatementSyntax) As Binder
				Return Me.GetBinderForNodeAndUsage(delegateSyntax, BinderFactory.NodeUsage.DelegateDeclaration, delegateSyntax.Parent, Me._position)
			End Function

			Public Overrides Function VisitEnumBlock(ByVal enumBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax) As Binder
				Return Me.GetBinderForNodeAndUsage(enumBlockSyntax.EnumStatement, BinderFactory.NodeUsage.EnumBlockFull, enumBlockSyntax.Parent, Me._position)
			End Function

			Public Overrides Function VisitEnumMemberDeclaration(ByVal node As EnumMemberDeclarationSyntax) As Binder
				Dim binderForNodeAndUsage As Binder
				If (Not BinderFactory.BinderFactoryVisitor.IsNotNothingAndContains(node.Initializer, Me._position)) Then
					binderForNodeAndUsage = Nothing
				Else
					binderForNodeAndUsage = Me.GetBinderForNodeAndUsage(node, BinderFactory.NodeUsage.FieldOrPropertyInitializer, node.Parent, Me._position)
				End If
				Return binderForNodeAndUsage
			End Function

			Public Overrides Function VisitImplementsStatement(ByVal implementsSyntax As ImplementsStatementSyntax) As Binder
				Return Me.GetBinderForNodeAndUsage(implementsSyntax, BinderFactory.NodeUsage.ImplementsStatement, implementsSyntax.Parent, Me._position)
			End Function

			Public Overrides Function VisitImportsStatement(ByVal node As ImportsStatementSyntax) As Binder
				Return BinderBuilder.CreateBinderForSourceFileImports(Me._factory._sourceModule, Me._factory._tree)
			End Function

			Public Overrides Function VisitInheritsStatement(ByVal inheritsSyntax As InheritsStatementSyntax) As Binder
				Return Me.GetBinderForNodeAndUsage(inheritsSyntax, BinderFactory.NodeUsage.InheritsStatement, inheritsSyntax.Parent, Me._position)
			End Function

			Public Overrides Function VisitInterfaceBlock(ByVal interfaceSyntax As InterfaceBlockSyntax) As Binder
				Return Me.GetBinderForNodeAndUsage(interfaceSyntax.BlockStatement, BinderFactory.NodeUsage.TypeBlockFull, interfaceSyntax.Parent, Me._position)
			End Function

			Private Function VisitMethodBaseDeclaration(ByVal methodBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax) As Binder
				Dim parent As MethodBlockBaseSyntax = TryCast(methodBaseSyntax.Parent, MethodBlockBaseSyntax)
				Return Me.GetBinderForNodeAndUsage(methodBaseSyntax, BinderFactory.NodeUsage.MethodFull, If(parent IsNot Nothing, parent.Parent, methodBaseSyntax.Parent), Me._position)
			End Function

			Public Overrides Function VisitMethodBlock(ByVal node As MethodBlockSyntax) As Binder
				Return Me.VisitMethodBlockBase(node, node.BlockStatement)
			End Function

			Private Function VisitMethodBlockBase(ByVal methodBlockSyntax As MethodBlockBaseSyntax, ByVal begin As MethodBaseSyntax) As Binder
				Dim nodeUsage As BinderFactory.NodeUsage
				nodeUsage = If(Not SyntaxFacts.InBlockInterior(methodBlockSyntax, Me._position), BinderFactory.NodeUsage.MethodFull, BinderFactory.NodeUsage.MethodInterior)
				Return Me.GetBinderForNodeAndUsage(begin, nodeUsage, methodBlockSyntax.Parent, Me._position)
			End Function

			Public Overrides Function VisitMethodStatement(ByVal node As MethodStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Binder
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
				binder = If(Not node.ContainsDiagnostics OrElse node.Parent.Kind() <> SyntaxKind.SingleLineSubLambdaExpression, Me.VisitMethodBaseDeclaration(node), Me.DefaultVisit(node))
				Return binder
			End Function

			Public Overrides Function VisitModuleBlock(ByVal moduleSyntax As ModuleBlockSyntax) As Binder
				Return Me.GetBinderForNodeAndUsage(moduleSyntax.BlockStatement, BinderFactory.NodeUsage.TypeBlockFull, moduleSyntax.Parent, Me._position)
			End Function

			Public Overrides Function VisitNamespaceBlock(ByVal nsBlockSyntax As NamespaceBlockSyntax) As Binder
				Dim binderForNodeAndUsage As Binder
				If (Not SyntaxFacts.InBlockInterior(nsBlockSyntax, Me._position)) Then
					binderForNodeAndUsage = Nothing
				Else
					binderForNodeAndUsage = Me.GetBinderForNodeAndUsage(nsBlockSyntax, BinderFactory.NodeUsage.NamespaceBlockInterior, nsBlockSyntax.Parent, Me._position)
				End If
				Return binderForNodeAndUsage
			End Function

			Public Overrides Function VisitOperatorBlock(ByVal node As OperatorBlockSyntax) As Binder
				Return Me.VisitMethodBlockBase(node, node.BlockStatement)
			End Function

			Public Overrides Function VisitOperatorStatement(ByVal node As OperatorStatementSyntax) As Binder
				Return Me.VisitMethodBaseDeclaration(node)
			End Function

			Public Overrides Function VisitParameter(ByVal node As ParameterSyntax) As Binder
				Dim binderForNodeAndUsage As Binder
				If (Not BinderFactory.BinderFactoryVisitor.IsNotNothingAndContains(node.[Default], Me._position)) Then
					binderForNodeAndUsage = Nothing
				Else
					binderForNodeAndUsage = Me.GetBinderForNodeAndUsage(node, BinderFactory.NodeUsage.ParameterDefaultValue, node.Parent, Me._position)
				End If
				Return binderForNodeAndUsage
			End Function

			Public Overrides Function VisitPropertyBlock(ByVal node As PropertyBlockSyntax) As Binder
				Return Me.GetBinderForNodeAndUsage(node.PropertyStatement, BinderFactory.NodeUsage.PropertyFull, node.Parent, Me._position)
			End Function

			Public Overrides Function VisitPropertyStatement(ByVal node As PropertyStatementSyntax) As Binder
				Dim binderForNodeAndUsage As Binder
				If (BinderFactory.BinderFactoryVisitor.IsNotNothingAndContains(node.Initializer, Me._position) OrElse BinderFactory.BinderFactoryVisitor.IsNotNothingAndContains(TryCast(node.AsClause, AsNewClauseSyntax), Me._position)) Then
					binderForNodeAndUsage = Me.GetBinderForNodeAndUsage(node, BinderFactory.NodeUsage.FieldOrPropertyInitializer, node.Parent, Me._position)
				Else
					binderForNodeAndUsage = Nothing
				End If
				Return binderForNodeAndUsage
			End Function

			Public Overrides Function VisitStructureBlock(ByVal structureSyntax As StructureBlockSyntax) As Binder
				Return Me.GetBinderForNodeAndUsage(structureSyntax.BlockStatement, BinderFactory.NodeUsage.TypeBlockFull, structureSyntax.Parent, Me._position)
			End Function

			Public Overrides Function VisitSubNewStatement(ByVal node As SubNewStatementSyntax) As Binder
				Return Me.VisitMethodBaseDeclaration(node)
			End Function

			Public Overrides Function VisitVariableDeclarator(ByVal node As VariableDeclaratorSyntax) As Binder
				Dim binderForNodeAndUsage As Binder
				If (BinderFactory.BinderFactoryVisitor.IsNotNothingAndContains(node.Initializer, Me._position) OrElse BinderFactory.BinderFactoryVisitor.IsNotNothingAndContains(TryCast(node.AsClause, AsNewClauseSyntax), Me._position)) Then
					binderForNodeAndUsage = Me.GetBinderForNodeAndUsage(node, BinderFactory.NodeUsage.FieldOrPropertyInitializer, node.Parent, Me._position)
				Else
					Dim enumerator As SeparatedSyntaxList(Of ModifiedIdentifierSyntax).Enumerator = node.Names.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As ModifiedIdentifierSyntax = enumerator.Current
						If (Not BinderFactory.BinderFactoryVisitor.IsNotNothingAndContains(current.ArrayBounds, Me._position)) Then
							Continue While
						End If
						binderForNodeAndUsage = Me.GetBinderForNodeAndUsage(current, BinderFactory.NodeUsage.FieldArrayBounds, node.Parent, Me._position)
						Return binderForNodeAndUsage
					End While
					binderForNodeAndUsage = Nothing
				End If
				Return binderForNodeAndUsage
			End Function

			Public Overrides Function VisitXmlAttribute(ByVal node As XmlAttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Binder
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
				If (node.Name.Kind() = SyntaxKind.XmlName) Then
					Dim valueText As String = DirectCast(node.Name, XmlNameSyntax).LocalName.ValueText
					If (DocumentationCommentXmlNames.AttributeEquals(valueText, "cref")) Then
						Dim structuredTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax = node.EnclosingStructuredTrivia()
						If (structuredTriviaSyntax Is Nothing OrElse structuredTriviaSyntax.Kind() <> SyntaxKind.DocumentationCommentTrivia) Then
							binder = MyBase.VisitXmlAttribute(node)
							Return binder
						End If
						binder = Me._factory.CreateDocumentationCommentBinder(DirectCast(structuredTriviaSyntax, DocumentationCommentTriviaSyntax), DocumentationCommentBinder.BinderType.Cref)
						Return binder
					ElseIf (DocumentationCommentXmlNames.AttributeEquals(valueText, "name")) Then
						Dim structuredTriviaSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax = node.EnclosingStructuredTrivia()
						If (structuredTriviaSyntax1 IsNot Nothing AndAlso structuredTriviaSyntax1.Kind() = SyntaxKind.DocumentationCommentTrivia) Then
							Dim binderTypeForNameAttribute As DocumentationCommentBinder.BinderType = DocumentationCommentBinder.GetBinderTypeForNameAttribute(node)
							If (binderTypeForNameAttribute = DocumentationCommentBinder.BinderType.None) Then
								binder = MyBase.VisitXmlAttribute(node)
								Return binder
							End If
							binder = Me._factory.CreateDocumentationCommentBinder(DirectCast(structuredTriviaSyntax1, DocumentationCommentTriviaSyntax), binderTypeForNameAttribute)
							Return binder
						End If
					End If
				End If
				binder = MyBase.VisitXmlAttribute(node)
				Return binder
			End Function

			Public Overrides Function VisitXmlCrefAttribute(ByVal node As XmlCrefAttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Binder
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
				Dim structuredTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax = node.EnclosingStructuredTrivia()
				binder = If(structuredTriviaSyntax Is Nothing OrElse structuredTriviaSyntax.Kind() <> SyntaxKind.DocumentationCommentTrivia, MyBase.VisitXmlCrefAttribute(node), Me._factory.CreateDocumentationCommentBinder(DirectCast(structuredTriviaSyntax, DocumentationCommentTriviaSyntax), DocumentationCommentBinder.BinderType.Cref))
				Return binder
			End Function

			Public Overrides Function VisitXmlNameAttribute(ByVal node As XmlNameAttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Binder
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
				Dim structuredTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax = node.EnclosingStructuredTrivia()
				If (structuredTriviaSyntax IsNot Nothing AndAlso structuredTriviaSyntax.Kind() = SyntaxKind.DocumentationCommentTrivia) Then
					Dim binderTypeForNameAttribute As DocumentationCommentBinder.BinderType = DocumentationCommentBinder.GetBinderTypeForNameAttribute(node)
					If (binderTypeForNameAttribute = DocumentationCommentBinder.BinderType.None) Then
						binder = MyBase.VisitXmlNameAttribute(node)
						Return binder
					End If
					binder = Me._factory.CreateDocumentationCommentBinder(DirectCast(structuredTriviaSyntax, DocumentationCommentTriviaSyntax), binderTypeForNameAttribute)
					Return binder
				End If
				binder = MyBase.VisitXmlNameAttribute(node)
				Return binder
			End Function
		End Class

		Private Enum NodeUsage As Byte
			CompilationUnit
			ImplicitClass
			ScriptCompilationUnit
			TopLevelExecutableStatement
			ImportsStatement
			NamespaceBlockInterior
			TypeBlockFull
			EnumBlockFull
			DelegateDeclaration
			InheritsStatement
			ImplementsStatement
			MethodFull
			MethodInterior
			FieldOrPropertyInitializer
			FieldArrayBounds
			Attribute
			ParameterDefaultValue
			PropertyFull
		End Enum
	End Class
End Namespace