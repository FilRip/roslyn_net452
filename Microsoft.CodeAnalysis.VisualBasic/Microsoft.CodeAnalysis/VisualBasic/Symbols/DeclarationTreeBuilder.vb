Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class DeclarationTreeBuilder
		Inherits VisualBasicSyntaxVisitor(Of SingleNamespaceOrTypeDeclaration)
		Private ReadOnly _rootNamespace As ImmutableArray(Of String)

		Private ReadOnly _scriptClassName As String

		Private ReadOnly _isSubmission As Boolean

		Private ReadOnly _syntaxTree As SyntaxTree

		Private ReadOnly Shared s_memberNameBuilderPool As ObjectPool(Of ImmutableHashSet(Of String).Builder)

		Shared Sub New()
			DeclarationTreeBuilder.s_memberNameBuilderPool = New ObjectPool(Of ImmutableHashSet(Of String).Builder)(Function() ImmutableHashSet.CreateBuilder(Of String)(CaseInsensitiveComparison.Comparer))
		End Sub

		Private Sub New(ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree, ByVal rootNamespace As ImmutableArray(Of String), ByVal scriptClassName As String, ByVal isSubmission As Boolean)
			MyBase.New()
			Me._syntaxTree = syntaxTree
			Me._rootNamespace = rootNamespace
			Me._scriptClassName = scriptClassName
			Me._isSubmission = isSubmission
		End Sub

		Private Sub AddMemberNames(ByVal methodDecl As MethodBaseSyntax, ByVal results As ImmutableHashSet(Of String).Builder)
			results.Add(SourceMethodSymbol.GetMemberNameFromSyntax(methodDecl))
		End Sub

		Private Function BuildRootNamespace(ByVal node As CompilationUnitSyntax, ByVal children As ImmutableArray(Of SingleNamespaceOrTypeDeclaration)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration
			Dim singleNamespaceDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration = Nothing
			Dim reference As SyntaxReference = Me._syntaxTree.GetReference(node)
			Dim location As Microsoft.CodeAnalysis.Location = reference.GetLocation()
			For i As Integer = Me._rootNamespace.Length - 1 To 0 Step -1
				Dim strs As ImmutableArray(Of String) = Me._rootNamespace
				singleNamespaceDeclaration = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration(Me.UnescapeIdentifier(strs(i)), True, reference, location, children, True)
				reference = Nothing
				location = Nothing
				children = ImmutableArray.Create(Of SingleNamespaceOrTypeDeclaration)(singleNamespaceDeclaration)
			Next

			Return singleNamespaceDeclaration
		End Function

		Private Function CreateImplicitClass(ByVal parent As VisualBasicSyntaxNode, ByVal memberNames As ImmutableHashSet(Of String), ByVal children As ImmutableArray(Of SingleTypeDeclaration), ByVal declFlags As SingleTypeDeclaration.TypeDeclarationFlags) As SingleNamespaceOrTypeDeclaration
			Dim reference As SyntaxReference = Me._syntaxTree.GetReference(parent)
			Return New SingleTypeDeclaration(DeclarationKind.ImplicitClass, "<invalid-global-code>", 0, DeclarationModifiers.[Friend] Or DeclarationModifiers.[Partial] Or DeclarationModifiers.[NotInheritable], declFlags, reference, reference.GetLocation(), memberNames, children)
		End Function

		Private Function CreateScriptClass(ByVal parent As VisualBasicSyntaxNode, ByVal children As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration), ByVal memberNames As ImmutableHashSet(Of String), ByVal declFlags As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration.TypeDeclarationFlags) As SingleNamespaceOrTypeDeclaration
			Dim reference As SyntaxReference = Me._syntaxTree.GetReference(parent)
			Dim strArray As String() = Me._scriptClassName.Split(New [Char]() { "."C })
			Dim singleTypeDeclaration As SingleNamespaceOrTypeDeclaration = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration(If(Me._isSubmission, DeclarationKind.Submission, DeclarationKind.Script), strArray.Last(), 0, DeclarationModifiers.[Friend] Or DeclarationModifiers.[Partial] Or DeclarationModifiers.[NotInheritable], declFlags, reference, reference.GetLocation(), memberNames, children)
			For i As Integer = CInt(strArray.Length) - 2 To 0 Step -1
				singleTypeDeclaration = New SingleNamespaceDeclaration(strArray(i), False, reference, reference.GetLocation(), ImmutableArray.Create(Of SingleNamespaceOrTypeDeclaration)(singleTypeDeclaration), False)
			Next

			Return singleTypeDeclaration
		End Function

		Private Sub FindGlobalDeclarations(ByVal declarations As ImmutableArray(Of SingleNamespaceOrTypeDeclaration), ByVal implicitClass As SingleNamespaceOrTypeDeclaration, ByRef globalDeclarations As ImmutableArray(Of SingleNamespaceOrTypeDeclaration), ByRef nonGlobal As ImmutableArray(Of SingleNamespaceOrTypeDeclaration))
			Dim instance As ArrayBuilder(Of SingleNamespaceOrTypeDeclaration) = ArrayBuilder(Of SingleNamespaceOrTypeDeclaration).GetInstance()
			Dim singleNamespaceOrTypeDeclarations As ArrayBuilder(Of SingleNamespaceOrTypeDeclaration) = ArrayBuilder(Of SingleNamespaceOrTypeDeclaration).GetInstance()
			If (implicitClass IsNot Nothing) Then
				singleNamespaceOrTypeDeclarations.Add(implicitClass)
			End If
			Dim enumerator As ImmutableArray(Of SingleNamespaceOrTypeDeclaration).Enumerator = declarations.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As SingleNamespaceOrTypeDeclaration = enumerator.Current
				Dim singleNamespaceDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration = TryCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration)
				If (singleNamespaceDeclaration Is Nothing OrElse Not singleNamespaceDeclaration.IsGlobalNamespace) Then
					singleNamespaceOrTypeDeclarations.Add(current)
				Else
					instance.AddRange(singleNamespaceDeclaration.Children)
				End If
			End While
			globalDeclarations = instance.ToImmutableAndFree()
			nonGlobal = singleNamespaceOrTypeDeclarations.ToImmutableAndFree()
		End Sub

		Private Function ForDeclaration(ByVal node As SyntaxNode) As SingleNamespaceOrTypeDeclaration
			Return Me.Visit(node)
		End Function

		Public Shared Function ForTree(ByVal tree As SyntaxTree, ByVal rootNamespace As ImmutableArray(Of String), ByVal scriptClassName As String, ByVal isSubmission As Boolean) As RootSingleNamespaceDeclaration
			Dim declarationTreeBuilder As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTreeBuilder = New Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationTreeBuilder(tree, rootNamespace, scriptClassName, isSubmission)
			Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
			Return DirectCast(declarationTreeBuilder.ForDeclaration(tree.GetRoot(cancellationToken)), RootSingleNamespaceDeclaration)
		End Function

		Public Shared Function GetArity(ByVal typeParamsSyntax As TypeParameterListSyntax) As Integer
			Return If(typeParamsSyntax IsNot Nothing, typeParamsSyntax.Parameters.Count, 0)
		End Function

		Public Shared Function GetKind(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind
			Dim declarationKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement) Then
				declarationKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind.[Namespace]
			Else
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement
						declarationKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind.[Module]
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement
						declarationKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind.[Structure]
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement
						declarationKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind.[Interface]
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement
						declarationKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind.[Class]
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement
						declarationKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind.[Enum]
						Exit Select
					Case Else
						If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
							Throw ExceptionUtilities.UnexpectedValue(kind)
						End If
						declarationKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind.[Delegate]
						Exit Select
				End Select
			End If
			Return declarationKind
		End Function

		Private Function GetMemberNames(ByVal enumBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax, ByRef declFlags As SingleTypeDeclaration.TypeDeclarationFlags) As ImmutableHashSet(Of String)
			If (enumBlockSyntax.Members.Count <> 0) Then
				declFlags = CByte(declFlags) Or 16
			End If
			Dim strs As ImmutableHashSet(Of String).Builder = DeclarationTreeBuilder.s_memberNameBuilderPool.Allocate()
			Dim flag As Boolean = False
			Dim enumerator As SyntaxList(Of StatementSyntax).Enumerator = enumBlockSyntax.Members.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As StatementSyntax = enumerator.Current
				If (current.Kind() <> SyntaxKind.EnumMemberDeclaration) Then
					Continue While
				End If
				Dim enumMemberDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax)
				strs.Add(enumMemberDeclarationSyntax.Identifier.ValueText)
				If (flag OrElse Not enumMemberDeclarationSyntax.AttributeLists.Any()) Then
					Continue While
				End If
				flag = True
			End While
			If (flag) Then
				declFlags = CByte(declFlags) Or 8
			End If
			Return DeclarationTreeBuilder.ToImmutableAndFree(strs)
		End Function

		Private Shared Function GetModifiers(ByVal modifiers As SyntaxTokenList) As DeclarationModifiers
			Dim current As SyntaxToken
			Dim severity As Func(Of Diagnostic, Boolean)
			Dim declarationModifier As DeclarationModifiers = DeclarationModifiers.None
			Dim enumerator As SyntaxTokenList.Enumerator = modifiers.GetEnumerator()
			While enumerator.MoveNext()
				current = enumerator.Current
				Dim declarationModifier1 As DeclarationModifiers = DeclarationModifiers.None
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = current.Kind()
				If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword) Then
					If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FriendKeyword) Then
						If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DefaultKeyword) Then
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstKeyword) Then
								declarationModifier1 = DeclarationModifiers.[Const]
							Else
								If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DefaultKeyword) Then
									GoTo Label0
								End If
								declarationModifier1 = DeclarationModifiers.[Default]
							End If
						ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DimKeyword) Then
							declarationModifier1 = DeclarationModifiers.[Dim]
						Else
							If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FriendKeyword) Then
								GoTo Label0
							End If
							declarationModifier1 = DeclarationModifiers.[Friend]
						End If
					ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MustOverrideKeyword) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MustInheritKeyword) Then
							declarationModifier1 = DeclarationModifiers.[MustInherit]
						Else
							If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MustOverrideKeyword) Then
								GoTo Label0
							End If
							declarationModifier1 = DeclarationModifiers.[MustOverride]
						End If
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NarrowingKeyword) Then
						declarationModifier1 = DeclarationModifiers.[Narrowing]
					Else
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword) Then
							GoTo Label0
						End If
						declarationModifier1 = DeclarationModifiers.[NotInheritable]
					End If
				ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword) Then
					If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword) Then
						Select Case syntaxKind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword
								declarationModifier1 = DeclarationModifiers.[Shadows]
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword
								declarationModifier1 = DeclarationModifiers.[Shared]
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword
								GoTo Label0
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword
								declarationModifier1 = DeclarationModifiers.[Static]
								Exit Select
							Case Else
								If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword) Then
									declarationModifier1 = DeclarationModifiers.[Widening]
									Exit Select
								Else
									GoTo Label0
								End If
						End Select
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword) Then
						declarationModifier1 = DeclarationModifiers.[NotOverridable]
					Else
						Select Case syntaxKind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword
								declarationModifier1 = DeclarationModifiers.[Overloads]
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword
								declarationModifier1 = DeclarationModifiers.[Overridable]
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword
								declarationModifier1 = DeclarationModifiers.[Overrides]
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword
								declarationModifier1 = DeclarationModifiers.[Partial]
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword
								declarationModifier1 = DeclarationModifiers.[Private]
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword
								declarationModifier1 = DeclarationModifiers.[Protected]
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword
								declarationModifier1 = DeclarationModifiers.[Public]
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword
								declarationModifier1 = DeclarationModifiers.[ReadOnly]
								Exit Select
							Case Else
								GoTo Label0
						End Select
					End If
				ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword) Then
						declarationModifier1 = DeclarationModifiers.[WithEvents]
					Else
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword) Then
							GoTo Label0
						End If
						declarationModifier1 = DeclarationModifiers.[WriteOnly]
					End If
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword) Then
					declarationModifier1 = DeclarationModifiers.Async
				Else
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword) Then
						GoTo Label0
					End If
					declarationModifier1 = DeclarationModifiers.Iterator
				End If
			Label7:
				declarationModifier = declarationModifier Or declarationModifier1
			End While
			Return declarationModifier
		Label0:
			Dim diagnostics As IEnumerable(Of Diagnostic) = current.GetDiagnostics()
			If (DeclarationTreeBuilder._Closure$__.$I35-0 Is Nothing) Then
				severity = Function(d As Diagnostic) d.Severity = DiagnosticSeverity.[Error]
				DeclarationTreeBuilder._Closure$__.$I35-0 = severity
			Else
				severity = DeclarationTreeBuilder._Closure$__.$I35-0
			End If
			If (Not diagnostics.Any(severity)) Then
				Throw ExceptionUtilities.UnexpectedValue(current.Kind())
			Else
				GoTo Label7
			End If
		End Function

		Private Function GetNonTypeMemberNames(ByVal members As SyntaxList(Of StatementSyntax), ByRef declFlags As SingleTypeDeclaration.TypeDeclarationFlags) As ImmutableHashSet(Of String)
			Dim identifier As SyntaxToken
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			Dim strs As ImmutableHashSet(Of String).Builder = DeclarationTreeBuilder.s_memberNameBuilderPool.Allocate()
			Dim enumerator As SyntaxList(Of StatementSyntax).Enumerator = members.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As StatementSyntax = enumerator.Current
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = current.Kind()
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock
						flag1 = True
						Dim blockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax = DirectCast(current, MethodBlockBaseSyntax).BlockStatement
						If (blockStatement.AttributeLists.Any()) Then
							flag = True
						End If
						Me.AddMemberNames(blockStatement, strs)
						Continue While
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParameterList
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement
					Case 100
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
						Continue While
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock
						flag1 = True
						Dim propertyBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax)
						If (Not propertyBlockSyntax.PropertyStatement.AttributeLists.Any()) Then
							Dim enumerator1 As SyntaxList(Of AccessorBlockSyntax).Enumerator = propertyBlockSyntax.Accessors.GetEnumerator()
							While enumerator1.MoveNext()
								If (Not enumerator1.Current.BlockStatement.AttributeLists.Any()) Then
									Continue While
								End If
								flag = True
							End While
						Else
							flag = True
						End If
						Me.AddMemberNames(propertyBlockSyntax.PropertyStatement, strs)
						Continue While
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock
						flag1 = True
						Dim eventBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax)
						If (Not eventBlockSyntax.EventStatement.AttributeLists.Any()) Then
							Dim enumerator2 As SyntaxList(Of AccessorBlockSyntax).Enumerator = eventBlockSyntax.Accessors.GetEnumerator()
							While enumerator2.MoveNext()
								If (Not enumerator2.Current.BlockStatement.AttributeLists.Any()) Then
									Continue While
								End If
								flag = True
							End While
						Else
							flag = True
						End If
						identifier = eventBlockSyntax.EventStatement.Identifier
						strs.Add(identifier.ValueText)
						Continue While
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement
						flag1 = True
						Dim methodBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax)
						If (methodBaseSyntax.AttributeLists.Any()) Then
							flag = True
						End If
						Me.AddMemberNames(methodBaseSyntax, strs)
						Continue While
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement
						flag1 = True
						Dim eventStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax)
						If (eventStatementSyntax.AttributeLists.Any()) Then
							flag = True
						End If
						identifier = eventStatementSyntax.Identifier
						strs.Add(identifier.ValueText)
						Continue While
					Case Else
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration) Then
							Continue While
						Else
							Exit Select
						End If
				End Select
				flag1 = True
				Dim fieldDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax)
				If (fieldDeclarationSyntax.AttributeLists.Any()) Then
					flag = True
				End If
				Dim enumerator3 As SeparatedSyntaxList(Of VariableDeclaratorSyntax).Enumerator = fieldDeclarationSyntax.Declarators.GetEnumerator()
				While enumerator3.MoveNext()
					Dim enumerator4 As SeparatedSyntaxList(Of ModifiedIdentifierSyntax).Enumerator = enumerator3.Current.Names.GetEnumerator()
					While enumerator4.MoveNext()
						identifier = enumerator4.Current.Identifier
						strs.Add(identifier.ValueText)
					End While
				End While
			End While
			If (flag) Then
				declFlags = CByte(declFlags) Or 8
			End If
			If (flag1) Then
				declFlags = CByte(declFlags) Or 16
			End If
			Return DeclarationTreeBuilder.ToImmutableAndFree(strs)
		End Function

		Private Shared Function GetReferenceDirectives(ByVal compilationUnit As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax) As ImmutableArray(Of ReferenceDirective)
			Dim immutableAndFree As ImmutableArray(Of ReferenceDirective)
			Dim enumerator As IEnumerator(Of ReferenceDirectiveTriviaSyntax) = Nothing
			Dim containsDiagnostics As Func(Of ReferenceDirectiveTriviaSyntax, Boolean)
			Dim compilationUnitSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax = compilationUnit
			If (DeclarationTreeBuilder._Closure$__.$I10-0 Is Nothing) Then
				containsDiagnostics = Function(d As ReferenceDirectiveTriviaSyntax)
					If (d.File.ContainsDiagnostics) Then
						Return False
					End If
					Return Not [String].IsNullOrEmpty(d.File.ValueText)
				End Function
				DeclarationTreeBuilder._Closure$__.$I10-0 = containsDiagnostics
			Else
				containsDiagnostics = DeclarationTreeBuilder._Closure$__.$I10-0
			End If
			Dim referenceDirectives As IList(Of ReferenceDirectiveTriviaSyntax) = compilationUnitSyntax.GetReferenceDirectives(containsDiagnostics)
			If (referenceDirectives.Count <> 0) Then
				Using instance As ArrayBuilder(Of ReferenceDirective) = ArrayBuilder(Of ReferenceDirective).GetInstance(referenceDirectives.Count)
					enumerator = referenceDirectives.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As ReferenceDirectiveTriviaSyntax = enumerator.Current
						Dim file As SyntaxToken = current.File
						instance.Add(New ReferenceDirective(file.ValueText, New SourceLocation(current)))
					End While
				End Using
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = ImmutableArray(Of ReferenceDirective).Empty
			End If
			Return immutableAndFree
		End Function

		Private Shared Function ToImmutableAndFree(ByVal builder As ImmutableHashSet(Of String).Builder) As ImmutableHashSet(Of String)
			Dim immutable As ImmutableHashSet(Of String) = builder.ToImmutable()
			builder.Clear()
			DeclarationTreeBuilder.s_memberNameBuilderPool.Free(builder)
			Return immutable
		End Function

		Private Function UnescapeIdentifier(ByVal identifier As String) As String
			Dim str As String
			str = If(EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(identifier(0)), "[", False) <> 0, identifier, identifier.Substring(1, identifier.Length - 2))
			Return str
		End Function

		Public Overrides Function VisitClassBlock(ByVal classBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax) As SingleNamespaceOrTypeDeclaration
			Return Me.VisitTypeBlockNew(classBlockSyntax)
		End Function

		Public Overrides Function VisitCompilationUnit(ByVal node As CompilationUnitSyntax) As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration
			Dim rootSingleNamespaceDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration
			Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration)
			Dim empty As ImmutableArray(Of ReferenceDirective)
			Dim attributes As SyntaxList(Of AttributesStatementSyntax)
			Dim singleNamespaceOrTypeDeclarations As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration)()
			Dim singleNamespaceOrTypeDeclarations1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration)()
			Me._syntaxTree.GetReference(node)
			Dim singleNamespaceOrTypeDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration = Nothing
			If (Me._syntaxTree.Options.Kind = SourceCodeKind.Regular) Then
				immutableAndFree = Me.VisitNamespaceChildren(node, node.Members, singleNamespaceOrTypeDeclaration).ToImmutableAndFree()
				empty = ImmutableArray(Of ReferenceDirective).Empty
			Else
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration).GetInstance()
				Dim singleTypeDeclarations As ArrayBuilder(Of SingleTypeDeclaration) = ArrayBuilder(Of SingleTypeDeclaration).GetInstance()
				Dim enumerator As SyntaxList(Of StatementSyntax).Enumerator = node.Members.GetEnumerator()
				While enumerator.MoveNext()
					Dim singleNamespaceOrTypeDeclaration1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration = Me.Visit(enumerator.Current)
					If (singleNamespaceOrTypeDeclaration1 Is Nothing) Then
						Continue While
					End If
					If (singleNamespaceOrTypeDeclaration1.Kind <> DeclarationKind.[Namespace]) Then
						singleTypeDeclarations.Add(DirectCast(singleNamespaceOrTypeDeclaration1, SingleTypeDeclaration))
					Else
						instance.Add(singleNamespaceOrTypeDeclaration1)
					End If
				End While
				Dim typeDeclarationFlag As SingleTypeDeclaration.TypeDeclarationFlags = SingleTypeDeclaration.TypeDeclarationFlags.None
				Dim nonTypeMemberNames As ImmutableHashSet(Of String) = Me.GetNonTypeMemberNames(node.Members, typeDeclarationFlag)
				singleNamespaceOrTypeDeclaration = Me.CreateScriptClass(node, singleTypeDeclarations.ToImmutableAndFree(), nonTypeMemberNames, typeDeclarationFlag)
				immutableAndFree = instance.ToImmutableAndFree()
				empty = DeclarationTreeBuilder.GetReferenceDirectives(node)
			End If
			Me.FindGlobalDeclarations(immutableAndFree, singleNamespaceOrTypeDeclaration, singleNamespaceOrTypeDeclarations, singleNamespaceOrTypeDeclarations1)
			If (Me._rootNamespace.Length <> 0) Then
				Dim singleNamespaceDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration = Me.BuildRootNamespace(node, singleNamespaceOrTypeDeclarations1)
				singleNamespaceOrTypeDeclarations = singleNamespaceOrTypeDeclarations.Add(singleNamespaceDeclaration)
				Dim singleNamespaceOrTypeDeclarations2 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration) = singleNamespaceOrTypeDeclarations.OfType(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration)().AsImmutable()
				Dim reference As Microsoft.CodeAnalysis.SyntaxReference = Me._syntaxTree.GetReference(node)
				attributes = node.Attributes
				rootSingleNamespaceDeclaration = New Microsoft.CodeAnalysis.VisualBasic.Symbols.RootSingleNamespaceDeclaration(True, reference, singleNamespaceOrTypeDeclarations2, empty, attributes.Any())
			Else
				Dim syntaxReference As Microsoft.CodeAnalysis.SyntaxReference = Me._syntaxTree.GetReference(node)
				Dim singleNamespaceOrTypeDeclarations3 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.Concat(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration)(singleNamespaceOrTypeDeclarations, singleNamespaceOrTypeDeclarations1)
				attributes = node.Attributes
				rootSingleNamespaceDeclaration = New Microsoft.CodeAnalysis.VisualBasic.Symbols.RootSingleNamespaceDeclaration(True, syntaxReference, singleNamespaceOrTypeDeclarations3, empty, attributes.Any())
			End If
			Return rootSingleNamespaceDeclaration
		End Function

		Public Overrides Function VisitDelegateStatement(ByVal node As DelegateStatementSyntax) As SingleNamespaceOrTypeDeclaration
			Dim typeDeclarationFlag As SingleTypeDeclaration.TypeDeclarationFlags = If(node.AttributeLists.Any(), SingleTypeDeclaration.TypeDeclarationFlags.HasAnyAttributes, SingleTypeDeclaration.TypeDeclarationFlags.None) Or SingleTypeDeclaration.TypeDeclarationFlags.HasAnyNontypeMembers
			Dim valueText As String = node.Identifier.ValueText
			Dim arity As Integer = DeclarationTreeBuilder.GetArity(node.TypeParameterList)
			Dim modifiers As DeclarationModifiers = DeclarationTreeBuilder.GetModifiers(node.Modifiers)
			Dim reference As SyntaxReference = Me._syntaxTree.GetReference(node)
			Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = Me._syntaxTree
			Dim identifier As SyntaxToken = node.Identifier
			Return New SingleTypeDeclaration(DeclarationKind.[Delegate], valueText, arity, modifiers, typeDeclarationFlag, reference, syntaxTree.GetLocation(identifier.Span), ImmutableHashSet(Of String).Empty, ImmutableArray(Of SingleTypeDeclaration).Empty)
		End Function

		Public Overrides Function VisitEnumBlock(ByVal enumBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax) As SingleNamespaceOrTypeDeclaration
			Dim enumStatement As EnumStatementSyntax = enumBlockSyntax.EnumStatement
			Dim typeDeclarationFlag As SingleTypeDeclaration.TypeDeclarationFlags = If(enumStatement.AttributeLists.Any(), SingleTypeDeclaration.TypeDeclarationFlags.HasAnyAttributes, SingleTypeDeclaration.TypeDeclarationFlags.None)
			If (enumStatement.UnderlyingType IsNot Nothing) Then
				typeDeclarationFlag = typeDeclarationFlag Or SingleTypeDeclaration.TypeDeclarationFlags.HasBaseDeclarations
			End If
			Dim memberNames As ImmutableHashSet(Of String) = Me.GetMemberNames(enumBlockSyntax, typeDeclarationFlag)
			Dim kind As DeclarationKind = DeclarationTreeBuilder.GetKind(enumStatement.Kind())
			Dim valueText As String = enumStatement.Identifier.ValueText
			Dim modifiers As DeclarationModifiers = DeclarationTreeBuilder.GetModifiers(enumStatement.Modifiers)
			Dim reference As SyntaxReference = Me._syntaxTree.GetReference(enumBlockSyntax)
			Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = Me._syntaxTree
			Dim identifier As SyntaxToken = enumBlockSyntax.EnumStatement.Identifier
			Return New SingleTypeDeclaration(kind, valueText, 0, modifiers, typeDeclarationFlag, reference, syntaxTree.GetLocation(identifier.Span), memberNames, Me.VisitTypeChildren(enumBlockSyntax.Members))
		End Function

		Public Overrides Function VisitEventStatement(ByVal node As EventStatementSyntax) As SingleNamespaceOrTypeDeclaration
			Dim singleTypeDeclaration As SingleNamespaceOrTypeDeclaration
			If (node.AsClause IsNot Nothing OrElse node.ImplementsClause IsNot Nothing) Then
				singleTypeDeclaration = Nothing
			Else
				Dim typeDeclarationFlag As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration.TypeDeclarationFlags = If(node.AttributeLists.Any(), Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration.TypeDeclarationFlags.HasAnyAttributes, Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration.TypeDeclarationFlags.None) Or Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration.TypeDeclarationFlags.HasAnyNontypeMembers
				Dim valueText As String = node.Identifier.ValueText
				Dim modifiers As DeclarationModifiers = DeclarationTreeBuilder.GetModifiers(node.Modifiers)
				Dim reference As SyntaxReference = Me._syntaxTree.GetReference(node)
				Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = Me._syntaxTree
				Dim identifier As SyntaxToken = node.Identifier
				singleTypeDeclaration = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration(DeclarationKind.EventSyntheticDelegate, valueText, 0, modifiers, typeDeclarationFlag, reference, syntaxTree.GetLocation(identifier.Span), ImmutableHashSet(Of String).Empty, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration).Empty)
			End If
			Return singleTypeDeclaration
		End Function

		Public Overrides Function VisitInterfaceBlock(ByVal interfaceBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceBlockSyntax) As SingleNamespaceOrTypeDeclaration
			Return Me.VisitTypeBlockNew(interfaceBlockSyntax)
		End Function

		Public Overrides Function VisitModuleBlock(ByVal moduleBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleBlockSyntax) As SingleNamespaceOrTypeDeclaration
			Return Me.VisitTypeBlockNew(moduleBlockSyntax)
		End Function

		Public Overrides Function VisitNamespaceBlock(ByVal nsBlockSyntax As NamespaceBlockSyntax) As SingleNamespaceOrTypeDeclaration
			Dim singleNamespaceDeclaration As SingleNamespaceOrTypeDeclaration
			Dim identifier As SyntaxToken
			Dim namespaceStatement As NamespaceStatementSyntax = nsBlockSyntax.NamespaceStatement
			Dim singleNamespaceOrTypeDeclarations As ImmutableArray(Of SingleNamespaceOrTypeDeclaration) = Me.VisitNamespaceChildren(nsBlockSyntax, nsBlockSyntax.Members)
			Dim name As NameSyntax = namespaceStatement.Name
			While TypeOf name Is Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax
				Dim qualifiedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax = DirectCast(name, Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax)
				identifier = qualifiedNameSyntax.Right.Identifier
				Dim singleNamespaceDeclaration1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration(identifier.ValueText, True, Me._syntaxTree.GetReference(qualifiedNameSyntax), Me._syntaxTree.GetLocation(qualifiedNameSyntax.Right.Span), singleNamespaceOrTypeDeclarations, False)
				singleNamespaceOrTypeDeclarations = DirectCast((New Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration() { singleNamespaceDeclaration1 }), IEnumerable).OfType(Of SingleNamespaceOrTypeDeclaration)().AsImmutable()
				name = qualifiedNameSyntax.Left
			End While
			If (name.Kind() <> SyntaxKind.GlobalName) Then
				identifier = DirectCast(name, IdentifierNameSyntax).Identifier
				singleNamespaceDeclaration = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration(identifier.ValueText, True, Me._syntaxTree.GetReference(name), Me._syntaxTree.GetLocation(name.Span), singleNamespaceOrTypeDeclarations, False)
			ElseIf (nsBlockSyntax.Parent.Kind() <> SyntaxKind.CompilationUnit) Then
				singleNamespaceDeclaration = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration("Global", True, Me._syntaxTree.GetReference(name), Me._syntaxTree.GetLocation(name.Span), singleNamespaceOrTypeDeclarations, False)
			Else
				singleNamespaceDeclaration = New GlobalNamespaceDeclaration(True, Me._syntaxTree.GetReference(name), Me._syntaxTree.GetLocation(name.Span), singleNamespaceOrTypeDeclarations)
			End If
			Return singleNamespaceDeclaration
		End Function

		Private Function VisitNamespaceChildren(ByVal node As VisualBasicSyntaxNode, ByVal members As SyntaxList(Of StatementSyntax)) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration)
			Dim singleNamespaceOrTypeDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration = Nothing
			Dim singleNamespaceOrTypeDeclarations As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration) = Me.VisitNamespaceChildren(node, members, singleNamespaceOrTypeDeclaration)
			If (singleNamespaceOrTypeDeclaration IsNot Nothing) Then
				singleNamespaceOrTypeDeclarations.Add(singleNamespaceOrTypeDeclaration)
			End If
			Return singleNamespaceOrTypeDeclarations.ToImmutableAndFree()
		End Function

		Private Function VisitNamespaceChildren(ByVal node As VisualBasicSyntaxNode, ByVal members As SyntaxList(Of StatementSyntax), <Out> ByRef implicitClass As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration) As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration)
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration).GetInstance()
			Dim singleTypeDeclarations As ArrayBuilder(Of SingleTypeDeclaration) = ArrayBuilder(Of SingleTypeDeclaration).GetInstance()
			Dim flag As Boolean = False
			Dim enumerator As SyntaxList(Of StatementSyntax).Enumerator = members.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As StatementSyntax = enumerator.Current
				Dim singleNamespaceOrTypeDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceOrTypeDeclaration = Me.Visit(current)
				If (singleNamespaceOrTypeDeclaration Is Nothing) Then
					If (flag) Then
						Continue While
					End If
					flag = If(current.Kind() = SyntaxKind.IncompleteMember, False, current.Kind() <> SyntaxKind.EmptyStatement)
				ElseIf (singleNamespaceOrTypeDeclaration.Kind <> DeclarationKind.EventSyntheticDelegate) Then
					instance.Add(singleNamespaceOrTypeDeclaration)
				Else
					singleTypeDeclarations.Add(DirectCast(singleNamespaceOrTypeDeclaration, SingleTypeDeclaration))
					flag = True
				End If
			End While
			If (Not flag) Then
				implicitClass = Nothing
			Else
				Dim typeDeclarationFlag As SingleTypeDeclaration.TypeDeclarationFlags = SingleTypeDeclaration.TypeDeclarationFlags.None
				Dim nonTypeMemberNames As ImmutableHashSet(Of String) = Me.GetNonTypeMemberNames(members, typeDeclarationFlag)
				implicitClass = Me.CreateImplicitClass(node, nonTypeMemberNames, singleTypeDeclarations.ToImmutable(), typeDeclarationFlag)
			End If
			singleTypeDeclarations.Free()
			Return instance
		End Function

		Public Overrides Function VisitStructureBlock(ByVal structureBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax) As SingleNamespaceOrTypeDeclaration
			Return Me.VisitTypeBlockNew(structureBlockSyntax)
		End Function

		Private Function VisitTypeBlockNew(ByVal topTypeBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax) As SingleNamespaceOrTypeDeclaration
			Dim instance As ArrayBuilder(Of DeclarationTreeBuilder.TypeBlockInfo) = ArrayBuilder(Of DeclarationTreeBuilder.TypeBlockInfo).GetInstance()
			instance.Add(New DeclarationTreeBuilder.TypeBlockInfo(topTypeBlockSyntax))
			Dim num As Integer = 0
			Do
				Dim item As DeclarationTreeBuilder.TypeBlockInfo = instance(num)
				Dim members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = item.TypeBlockSyntax.Members
				If (members.Count > 0) Then
					Dim nums As ArrayBuilder(Of Integer) = Nothing
					Dim enumerator As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax).Enumerator = members.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = enumerator.Current
						If (CUShort(current.Kind()) - CUShort(SyntaxKind.ModuleBlock) > CUShort((SyntaxKind.List Or SyntaxKind.EmptyStatement))) Then
							Continue While
						End If
						If (nums Is Nothing) Then
							nums = ArrayBuilder(Of Integer).GetInstance()
						End If
						nums.Add(instance.Count)
						instance.Add(New DeclarationTreeBuilder.TypeBlockInfo(DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax)))
					End While
					If (nums IsNot Nothing) Then
						instance(num) = item.WithNestedTypes(nums)
					End If
				End If
				num = num + 1
			Loop While num < instance.Count
			Dim singleTypeDeclarations As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration).GetInstance()
			While num > 0
				num = num - 1
				Dim typeBlockInfo As DeclarationTreeBuilder.TypeBlockInfo = instance(num)
				Dim empty As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration) = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration).Empty
				Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = typeBlockInfo.TypeBlockSyntax.Members
				If (statementSyntaxes.Count > 0) Then
					singleTypeDeclarations.Clear()
					Dim enumerator1 As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax).Enumerator = statementSyntaxes.GetEnumerator()
					While enumerator1.MoveNext()
						Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = enumerator1.Current
						If (CUShort(statementSyntax.Kind()) - CUShort(SyntaxKind.ModuleBlock) <= CUShort((SyntaxKind.List Or SyntaxKind.EmptyStatement))) Then
							Continue While
						End If
						Dim singleTypeDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration = TryCast(Me.Visit(statementSyntax), Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration)
						If (singleTypeDeclaration Is Nothing) Then
							Continue While
						End If
						singleTypeDeclarations.Add(singleTypeDeclaration)
					End While
					Dim nestedTypes As ArrayBuilder(Of Integer) = typeBlockInfo.NestedTypes
					If (nestedTypes IsNot Nothing) Then
						Dim count As Integer = nestedTypes.Count - 1
						Dim num1 As Integer = 0
						Do
							singleTypeDeclarations.Add(instance(nestedTypes(num1)).TypeDeclaration)
							num1 = num1 + 1
						Loop While num1 <= count
						nestedTypes.Free()
					End If
					empty = singleTypeDeclarations.ToImmutable()
				End If
				Dim typeBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax = typeBlockInfo.TypeBlockSyntax
				Dim blockStatement As TypeStatementSyntax = typeBlockSyntax.BlockStatement
				Dim arity As Integer = 0
				If (CUShort(typeBlockSyntax.Kind()) - CUShort(SyntaxKind.StructureBlock) <= CUShort(SyntaxKind.EmptyStatement)) Then
					arity = DeclarationTreeBuilder.GetArity(blockStatement.TypeParameterList)
				End If
				Dim typeDeclarationFlag As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration.TypeDeclarationFlags = If(blockStatement.AttributeLists.Any(), Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration.TypeDeclarationFlags.HasAnyAttributes, Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration.TypeDeclarationFlags.None)
				If (typeBlockSyntax.[Inherits].Any()) Then
					typeDeclarationFlag = typeDeclarationFlag Or Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration.TypeDeclarationFlags.HasBaseDeclarations
				End If
				Dim nonTypeMemberNames As ImmutableHashSet(Of String) = Me.GetNonTypeMemberNames(typeBlockSyntax.Members, typeDeclarationFlag)
				Dim kind As DeclarationKind = DeclarationTreeBuilder.GetKind(blockStatement.Kind())
				Dim valueText As String = blockStatement.Identifier.ValueText
				Dim modifiers As DeclarationModifiers = DeclarationTreeBuilder.GetModifiers(blockStatement.Modifiers)
				Dim reference As SyntaxReference = Me._syntaxTree.GetReference(typeBlockSyntax)
				Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = Me._syntaxTree
				Dim identifier As SyntaxToken = typeBlockSyntax.BlockStatement.Identifier
				instance(num) = typeBlockInfo.WithDeclaration(New Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration(kind, valueText, arity, modifiers, typeDeclarationFlag, reference, syntaxTree.GetLocation(identifier.Span), nonTypeMemberNames, empty))
			End While
			singleTypeDeclarations.Free()
			Dim typeDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration = instance(0).TypeDeclaration
			instance.Free()
			Return typeDeclaration
		End Function

		Private Function VisitTypeChildren(ByVal members As SyntaxList(Of StatementSyntax)) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration)
			Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration)
			If (members.Count <> 0) Then
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration).GetInstance()
				Dim enumerator As SyntaxList(Of StatementSyntax).Enumerator = members.GetEnumerator()
				While enumerator.MoveNext()
					Dim singleTypeDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration = TryCast(Me.Visit(enumerator.Current), Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration)
					If (singleTypeDeclaration Is Nothing) Then
						Continue While
					End If
					instance.Add(singleTypeDeclaration)
				End While
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleTypeDeclaration).Empty
			End If
			Return immutableAndFree
		End Function

		Private Structure TypeBlockInfo
			Public ReadOnly TypeBlockSyntax As TypeBlockSyntax

			Public ReadOnly TypeDeclaration As SingleTypeDeclaration

			Public ReadOnly NestedTypes As ArrayBuilder(Of Integer)

			Public Sub New(ByVal typeBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax)
				MyClass.New(typeBlockSyntax, Nothing, Nothing)
			End Sub

			Private Sub New(ByVal typeBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax, ByVal declaration As SingleTypeDeclaration, ByVal nestedTypes As ArrayBuilder(Of Integer))
				Me = New DeclarationTreeBuilder.TypeBlockInfo() With
				{
					.TypeBlockSyntax = typeBlockSyntax,
					.TypeDeclaration = declaration,
					.NestedTypes = nestedTypes
				}
			End Sub

			Public Function WithDeclaration(ByVal declaration As SingleTypeDeclaration) As DeclarationTreeBuilder.TypeBlockInfo
				Return New DeclarationTreeBuilder.TypeBlockInfo(Me.TypeBlockSyntax, declaration, Me.NestedTypes)
			End Function

			Public Function WithNestedTypes(ByVal nested As ArrayBuilder(Of Integer)) As DeclarationTreeBuilder.TypeBlockInfo
				Return New DeclarationTreeBuilder.TypeBlockInfo(Me.TypeBlockSyntax, Nothing, nested)
			End Function
		End Structure
	End Class
End Namespace