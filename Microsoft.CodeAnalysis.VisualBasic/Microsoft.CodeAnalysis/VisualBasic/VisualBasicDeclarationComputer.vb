Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class VisualBasicDeclarationComputer
		Inherits DeclarationComputer
		Public Sub New()
			MyBase.New()
		End Sub

		Private Shared Sub ComputeDeclarationsCore(ByVal model As SemanticModel, ByVal node As SyntaxNode, ByVal shouldSkip As Func(Of SyntaxNode, Nullable(Of Integer), Boolean), ByVal getSymbol As Boolean, ByVal builder As ArrayBuilder(Of Microsoft.CodeAnalysis.DeclarationInfo), ByVal levelsToCompute As Nullable(Of Integer), ByVal cancellationToken As System.Threading.CancellationToken)
			Dim nullable As Nullable(Of Integer)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim containingNamespace As INamespaceSymbol
			cancellationToken.ThrowIfCancellationRequested()
			If (Not shouldSkip(node, levelsToCompute)) Then
				nullable = VisualBasicDeclarationComputer.DecrementLevel(levelsToCompute)
				syntaxKind = node.Kind()
				If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement) Then
					If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock) Then
							Dim enumBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax)
							Dim enumerator As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax).Enumerator = enumBlockSyntax.Members.GetEnumerator()
							While enumerator.MoveNext()
								Dim current As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = enumerator.Current
								VisualBasicDeclarationComputer.ComputeDeclarationsCore(model, current, shouldSkip, getSymbol, builder, nullable, cancellationToken)
							End While
							Dim attributes As IEnumerable(Of SyntaxNode) = VisualBasicDeclarationComputer.GetAttributes(enumBlockSyntax.EnumStatement.AttributeLists)
							builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, attributes, cancellationToken))
							Return
						End If
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement) Then
							Dim syntaxNodes As IEnumerable(Of SyntaxNode) = VisualBasicDeclarationComputer.GetAttributes(DirectCast(node, EnumStatementSyntax).AttributeLists)
							builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, syntaxNodes, cancellationToken))
							Return
						End If
					Else
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit) Then
							GoTo Label1
						End If
						Dim compilationUnitSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax)
						Dim enumerator1 As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax).Enumerator = compilationUnitSyntax.Members.GetEnumerator()
						While enumerator1.MoveNext()
							Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = enumerator1.Current
							VisualBasicDeclarationComputer.ComputeDeclarationsCore(model, statementSyntax, shouldSkip, getSymbol, builder, nullable, cancellationToken)
						End While
						If (Not DirectCast(compilationUnitSyntax.Attributes, IReadOnlyCollection(Of AttributesStatementSyntax)).IsEmpty()) Then
							Dim attributes1 As IEnumerable(Of SyntaxNode) = VisualBasicDeclarationComputer.GetAttributes(compilationUnitSyntax.Attributes)
							builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, attributes1, cancellationToken))
							Return
						Else
							Return
						End If
					End If
				ElseIf (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock) Then
						Dim eventBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax)
						Dim enumerator2 As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax).Enumerator = eventBlockSyntax.Accessors.GetEnumerator()
						While enumerator2.MoveNext()
							Dim accessorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax = enumerator2.Current
							VisualBasicDeclarationComputer.ComputeDeclarationsCore(model, accessorBlockSyntax, shouldSkip, getSymbol, builder, nullable, cancellationToken)
						End While
						Dim methodBaseCodeBlocks As IEnumerable(Of SyntaxNode) = VisualBasicDeclarationComputer.GetMethodBaseCodeBlocks(eventBlockSyntax.EventStatement)
						builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, methodBaseCodeBlocks, cancellationToken))
						Return
					End If
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement) Then
						Dim propertyStatementCodeBlocks As IEnumerable(Of SyntaxNode) = VisualBasicDeclarationComputer.GetPropertyStatementCodeBlocks(DirectCast(node, PropertyStatementSyntax))
						builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, propertyStatementCodeBlocks, cancellationToken))
						Return
					End If
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration) Then
						Dim fieldDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax)
						Dim syntaxNodes1 As IEnumerable(Of SyntaxNode) = VisualBasicDeclarationComputer.GetAttributes(fieldDeclarationSyntax.AttributeLists)
						Dim enumerator3 As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax).Enumerator = fieldDeclarationSyntax.Declarators.GetEnumerator()
						While enumerator3.MoveNext()
							Dim variableDeclaratorSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax = enumerator3.Current
							Dim syntaxNodes2 As IEnumerable(Of SyntaxNode) = SpecializedCollections.SingletonEnumerable(Of SyntaxNode)(VisualBasicDeclarationComputer.GetInitializerNode(variableDeclaratorSyntax)).Concat(syntaxNodes1)
							Dim enumerator4 As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax).Enumerator = variableDeclaratorSyntax.Names.GetEnumerator()
							While enumerator4.MoveNext()
								Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax = enumerator4.Current
								builder.Add(DeclarationComputer.GetDeclarationInfo(model, modifiedIdentifierSyntax, getSymbol, syntaxNodes2, cancellationToken))
							End While
						End While
						Return
					End If
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration) Then
						Dim enumMemberDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax)
						Dim attributes2 As IEnumerable(Of SyntaxNode) = VisualBasicDeclarationComputer.GetAttributes(enumMemberDeclarationSyntax.AttributeLists)
						Dim syntaxNodes3 As IEnumerable(Of SyntaxNode) = SpecializedCollections.SingletonEnumerable(Of SyntaxNode)(enumMemberDeclarationSyntax.Initializer).Concat(attributes2)
						builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, syntaxNodes3, cancellationToken))
						Return
					End If
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock) Then
						Dim propertyBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax)
						Dim enumerator5 As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax).Enumerator = propertyBlockSyntax.Accessors.GetEnumerator()
						While enumerator5.MoveNext()
							Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax = enumerator5.Current
							VisualBasicDeclarationComputer.ComputeDeclarationsCore(model, current1, shouldSkip, getSymbol, builder, nullable, cancellationToken)
						End While
						Dim propertyStatementCodeBlocks1 As IEnumerable(Of SyntaxNode) = VisualBasicDeclarationComputer.GetPropertyStatementCodeBlocks(propertyBlockSyntax.PropertyStatement)
						builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, propertyStatementCodeBlocks1, cancellationToken))
						Return
					End If
				End If
			Label2:
				Dim typeBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax)
				If (typeBlockSyntax IsNot Nothing) Then
					Dim enumerator6 As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax).Enumerator = typeBlockSyntax.Members.GetEnumerator()
					While enumerator6.MoveNext()
						Dim statementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = enumerator6.Current
						VisualBasicDeclarationComputer.ComputeDeclarationsCore(model, statementSyntax1, shouldSkip, getSymbol, builder, nullable, cancellationToken)
					End While
					Dim attributes3 As IEnumerable(Of SyntaxNode) = VisualBasicDeclarationComputer.GetAttributes(typeBlockSyntax.BlockStatement.AttributeLists)
					builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, attributes3, cancellationToken))
					Return
				End If
				Dim typeStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax)
				If (typeStatementSyntax IsNot Nothing) Then
					Dim attributes4 As IEnumerable(Of SyntaxNode) = VisualBasicDeclarationComputer.GetAttributes(typeStatementSyntax.AttributeLists)
					builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, attributes4, cancellationToken))
					Return
				End If
				Dim methodBlockBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax)
				If (methodBlockBaseSyntax IsNot Nothing) Then
					Dim syntaxNodes4 As IEnumerable(Of SyntaxNode) = SpecializedCollections.SingletonEnumerable(Of SyntaxNode)(methodBlockBaseSyntax).Concat(VisualBasicDeclarationComputer.GetMethodBaseCodeBlocks(methodBlockBaseSyntax.BlockStatement))
					builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, syntaxNodes4, cancellationToken))
					Return
				End If
				Dim methodBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax)
				If (methodBaseSyntax IsNot Nothing) Then
					Dim methodBaseCodeBlocks1 As IEnumerable(Of SyntaxNode) = VisualBasicDeclarationComputer.GetMethodBaseCodeBlocks(methodBaseSyntax)
					builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, methodBaseCodeBlocks1, cancellationToken))
				End If
			End If
			Return
		Label1:
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock) Then
				Dim namespaceBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax)
				Dim enumerator7 As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax).Enumerator = namespaceBlockSyntax.Members.GetEnumerator()
				While enumerator7.MoveNext()
					Dim current2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = enumerator7.Current
					VisualBasicDeclarationComputer.ComputeDeclarationsCore(model, current2, shouldSkip, getSymbol, builder, nullable, cancellationToken)
				End While
				Dim declarationInfo As Microsoft.CodeAnalysis.DeclarationInfo = DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, cancellationToken)
				builder.Add(declarationInfo)
				Dim name As NameSyntax = namespaceBlockSyntax.NamespaceStatement.Name
				Dim declaredSymbol As ISymbol = declarationInfo.DeclaredSymbol
				While name.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedName
					name = DirectCast(name, QualifiedNameSyntax).Left
					If (Not getSymbol) Then
						containingNamespace = Nothing
					ElseIf (declaredSymbol IsNot Nothing) Then
						containingNamespace = declaredSymbol.ContainingNamespace
					Else
						containingNamespace = Nothing
					End If
					Dim namespaceSymbol As INamespaceSymbol = containingNamespace
					builder.Add(New Microsoft.CodeAnalysis.DeclarationInfo(name, ImmutableArray(Of SyntaxNode).Empty, namespaceSymbol))
					declaredSymbol = namespaceSymbol
				End While
				Return
			End If
			GoTo Label2
		End Sub

		Public Shared Sub ComputeDeclarationsInNode(ByVal model As Microsoft.CodeAnalysis.SemanticModel, ByVal node As Microsoft.CodeAnalysis.SyntaxNode, ByVal getSymbol As Boolean, ByVal builder As ArrayBuilder(Of DeclarationInfo), ByVal cancellationToken As System.Threading.CancellationToken, Optional ByVal levelsToCompute As Nullable(Of Integer) = Nothing)
			Dim func As Func(Of Microsoft.CodeAnalysis.SyntaxNode, Nullable(Of Integer), Boolean)
			Dim semanticModel As Microsoft.CodeAnalysis.SemanticModel = model
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node
			If (VisualBasicDeclarationComputer._Closure$__.$I2-0 Is Nothing) Then
				func = Function(n As Microsoft.CodeAnalysis.SyntaxNode, level As Nullable(Of Integer)) VisualBasicDeclarationComputer.InvalidLevel(level)
				VisualBasicDeclarationComputer._Closure$__.$I2-0 = func
			Else
				func = VisualBasicDeclarationComputer._Closure$__.$I2-0
			End If
			VisualBasicDeclarationComputer.ComputeDeclarationsCore(semanticModel, syntaxNode, func, getSymbol, builder, levelsToCompute, cancellationToken)
		End Sub

		Public Shared Sub ComputeDeclarationsInSpan(ByVal model As SemanticModel, ByVal span As TextSpan, ByVal getSymbol As Boolean, ByVal builder As ArrayBuilder(Of DeclarationInfo), ByVal cancellationToken As System.Threading.CancellationToken)
			Dim nullable As Nullable(Of Integer) = Nothing
			VisualBasicDeclarationComputer.ComputeDeclarationsCore(model, model.SyntaxTree.GetRoot(cancellationToken), Function(node As SyntaxNode, level As Nullable(Of Integer))
				If (Not node.Span.OverlapsWith(span)) Then
					Return True
				End If
				Return VisualBasicDeclarationComputer.InvalidLevel(level)
			End Function, getSymbol, builder, nullable, cancellationToken)
		End Sub

		Private Shared Function DecrementLevel(ByVal level As Nullable(Of Integer)) As Nullable(Of Integer)
			If (Not level.HasValue) Then
				Return level
			End If
			Return New Nullable(Of Integer)(level.Value - 1)
		End Function

		Private Shared Function GetAsClause(ByVal methodBase As MethodBaseSyntax) As AsClauseSyntax
			Dim asClause As AsClauseSyntax
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = methodBase.Kind()
			Select Case syntaxKind
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement
					asClause = DirectCast(methodBase, MethodStatementSyntax).AsClause
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement
					asClause = Nothing
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
					asClause = DirectCast(methodBase, DeclareStatementSyntax).AsClause
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement
					asClause = DirectCast(methodBase, DelegateStatementSyntax).AsClause
					Exit Select
				Case 100
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
					Throw ExceptionUtilities.UnexpectedValue(methodBase.Kind())
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement
					asClause = DirectCast(methodBase, EventStatementSyntax).AsClause
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement
					asClause = DirectCast(methodBase, OperatorStatementSyntax).AsClause
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement
					asClause = DirectCast(methodBase, PropertyStatementSyntax).AsClause
					Exit Select
				Case Else
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubLambdaHeader) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						asClause = DirectCast(methodBase, LambdaHeaderSyntax).AsClause
						Exit Select
					Else
						Throw ExceptionUtilities.UnexpectedValue(methodBase.Kind())
					End If
			End Select
			Return asClause
		End Function

		Private Shared Function GetAsNewClauseInitializer(ByVal asClause As AsClauseSyntax) As SyntaxNode
			If (Not asClause.IsKind(SyntaxKind.AsNewClause)) Then
				Return Nothing
			End If
			Return asClause
		End Function

		Private Shared Function GetAttributes(ByVal attributeStatements As SyntaxList(Of AttributesStatementSyntax)) As IEnumerable(Of SyntaxNode)
			Dim syntaxNodes As IEnumerable(Of SyntaxNode) = SpecializedCollections.EmptyEnumerable(Of SyntaxNode)()
			Dim enumerator As SyntaxList(Of AttributesStatementSyntax).Enumerator = attributeStatements.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As AttributesStatementSyntax = enumerator.Current
				syntaxNodes = syntaxNodes.Concat(VisualBasicDeclarationComputer.GetAttributes(current.AttributeLists))
			End While
			Return syntaxNodes
		End Function

		Private Shared Function GetAttributes(ByVal attributeLists As SyntaxList(Of AttributeListSyntax)) As IEnumerable(Of SyntaxNode)
			Return New VisualBasicDeclarationComputer.VB$StateMachine_11_GetAttributes(-2) With
			{
				.$P_attributeLists = attributeLists
			}
		End Function

		Private Shared Function GetInitializerNode(ByVal variableDeclarator As VariableDeclaratorSyntax) As SyntaxNode
			Dim initializer As SyntaxNode = variableDeclarator.Initializer
			If (initializer Is Nothing) Then
				initializer = VisualBasicDeclarationComputer.GetAsNewClauseInitializer(variableDeclarator.AsClause)
			End If
			Return initializer
		End Function

		Private Shared Function GetMethodBaseCodeBlocks(ByVal methodBase As MethodBaseSyntax) As IEnumerable(Of SyntaxNode)
			Dim parameterListInitializersAndAttributes As IEnumerable(Of SyntaxNode) = VisualBasicDeclarationComputer.GetParameterListInitializersAndAttributes(methodBase.ParameterList)
			Dim syntaxNodes As IEnumerable(Of SyntaxNode) = VisualBasicDeclarationComputer.GetAttributes(methodBase.AttributeLists).Concat(VisualBasicDeclarationComputer.GetReturnTypeAttributes(VisualBasicDeclarationComputer.GetAsClause(methodBase)))
			Return parameterListInitializersAndAttributes.Concat(syntaxNodes)
		End Function

		Private Shared Function GetParameterInitializersAndAttributes(ByVal parameter As ParameterSyntax) As IEnumerable(Of SyntaxNode)
			Return SpecializedCollections.SingletonEnumerable(Of SyntaxNode)(parameter.[Default]).Concat(VisualBasicDeclarationComputer.GetAttributes(parameter.AttributeLists))
		End Function

		Private Shared Function GetParameterListInitializersAndAttributes(ByVal parameterList As ParameterListSyntax) As IEnumerable(Of SyntaxNode)
			Dim parameterInitializersAndAttributes As Func(Of ParameterSyntax, IEnumerable(Of SyntaxNode))
			If (parameterList Is Nothing) Then
				Return SpecializedCollections.EmptyEnumerable(Of SyntaxNode)()
			End If
			Dim parameters As IEnumerable(Of ParameterSyntax) = DirectCast(parameterList.Parameters, IEnumerable(Of ParameterSyntax))
			If (VisualBasicDeclarationComputer._Closure$__.$I12-0 Is Nothing) Then
				parameterInitializersAndAttributes = Function(p As ParameterSyntax) VisualBasicDeclarationComputer.GetParameterInitializersAndAttributes(p)
				VisualBasicDeclarationComputer._Closure$__.$I12-0 = parameterInitializersAndAttributes
			Else
				parameterInitializersAndAttributes = VisualBasicDeclarationComputer._Closure$__.$I12-0
			End If
			Return parameters.SelectMany(Of SyntaxNode)(parameterInitializersAndAttributes)
		End Function

		Private Shared Function GetPropertyStatementCodeBlocks(ByVal propertyStatement As PropertyStatementSyntax) As IEnumerable(Of SyntaxNode)
			Dim initializer As SyntaxNode = propertyStatement.Initializer
			If (initializer Is Nothing) Then
				initializer = VisualBasicDeclarationComputer.GetAsNewClauseInitializer(propertyStatement.AsClause)
			End If
			Dim methodBaseCodeBlocks As IEnumerable(Of SyntaxNode) = VisualBasicDeclarationComputer.GetMethodBaseCodeBlocks(propertyStatement)
			If (initializer Is Nothing) Then
				Return methodBaseCodeBlocks
			End If
			Return SpecializedCollections.SingletonEnumerable(Of SyntaxNode)(initializer).Concat(methodBaseCodeBlocks)
		End Function

		Private Shared Function GetReturnTypeAttributes(ByVal asClause As AsClauseSyntax) As IEnumerable(Of SyntaxNode)
			If (asClause Is Nothing OrElse DirectCast(asClause.Attributes(), IReadOnlyCollection(Of AttributeListSyntax)).IsEmpty()) Then
				Return SpecializedCollections.EmptyEnumerable(Of SyntaxNode)()
			End If
			Return VisualBasicDeclarationComputer.GetAttributes(asClause.Attributes())
		End Function

		Private Shared Function InvalidLevel(ByVal level As Nullable(Of Integer)) As Boolean
			If (Not level.HasValue) Then
				Return False
			End If
			Return level.Value <= 0
		End Function
	End Class
End Namespace