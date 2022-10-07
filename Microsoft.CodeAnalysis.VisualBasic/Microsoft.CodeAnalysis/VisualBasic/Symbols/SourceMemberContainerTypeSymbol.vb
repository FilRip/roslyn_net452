Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
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
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SourceMemberContainerTypeSymbol
		Inherits InstanceTypeSymbol
		Private ReadOnly _flags As SourceMemberContainerTypeSymbol.SourceTypeFlags

		Protected m_lazyState As Integer

		Private ReadOnly _containingSymbol As NamespaceOrTypeSymbol

		Protected ReadOnly m_containingModule As SourceModuleSymbol

		Private ReadOnly _declaration As MergedTypeDeclaration

		Private ReadOnly _name As String

		Private _defaultPropertyName As String

		Private _lazyMembersAndInitializers As SourceMemberContainerTypeSymbol.MembersAndInitializers

		Private ReadOnly Shared s_emptyTypeMembers As Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol))

		Private _lazyTypeMembers As Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol))

		Private _lazyMembersFlattened As ImmutableArray(Of Symbol)

		Private _lazyTypeParameters As ImmutableArray(Of TypeParameterSymbol)

		Private _lazyEmitExtensionAttribute As ThreeState

		Private _lazyContainsExtensionMethods As ThreeState

		Private _lazyAnyMemberHasAttributes As ThreeState

		Private _lazyStructureCycle As Integer

		Private _lazyLexicalSortKey As LexicalSortKey

		Friend ReadOnly Property AnyMemberHasAttributes As Boolean
			Get
				If (Not Me._lazyAnyMemberHasAttributes.HasValue()) Then
					Me._lazyAnyMemberHasAttributes = Me._declaration.AnyMemberHasAttributes.ToThreeState()
				End If
				Return Me._lazyAnyMemberHasAttributes.Value()
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Arity As Integer
			Get
				Return Me._declaration.Arity
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingModule As ModuleSymbol
			Get
				Return Me.m_containingModule
			End Get
		End Property

		Public ReadOnly Property ContainingSourceModule As SourceModuleSymbol
			Get
				Return Me.m_containingModule
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return TryCast(Me._containingSymbol, NamedTypeSymbol)
			End Get
		End Property

		Friend ReadOnly Property DeclarationKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind
			Get
				Return Me._declaration.Kind
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return DirectCast((Me._flags And SourceMemberContainerTypeSymbol.SourceTypeFlags.AccessibilityMask), Accessibility)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Symbol.GetDeclaringSyntaxReferenceHelper(Me.SyntaxReferences)
			End Get
		End Property

		Friend Overrides ReadOnly Property DefaultPropertyName As String
			Get
				If (Me.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Delegate]) Then
					Me.GetMembersAndInitializers()
				End If
				Return Me._defaultPropertyName
			End Get
		End Property

		Private ReadOnly Property EmitExtensionAttribute As Boolean
			Get
				If (Me._lazyEmitExtensionAttribute = ThreeState.Unknown) Then
					Me.BindAllMemberAttributes(New CancellationToken())
				End If
				Return Me._lazyEmitExtensionAttribute = ThreeState.[True]
			End Get
		End Property

		Friend Overrides ReadOnly Property ExplicitInterfaceImplementationMap As MultiDictionary(Of Symbol, Symbol)
			Get
				If (Me.m_lazyExplicitInterfaceImplementationMap Is Nothing) Then
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					Dim symbols As MultiDictionary(Of Symbol, Symbol) = Me.MakeExplicitInterfaceImplementationMap(instance)
					OverrideHidingHelper.CheckHidingAndOverridingForType(Me, instance)
					Me.CheckForOverloadsErrors(instance)
					Me.m_containingModule.AtomicStoreReferenceAndDiagnostics(Of MultiDictionary(Of Symbol, Symbol))(Me.m_lazyExplicitInterfaceImplementationMap, symbols, instance, Nothing)
					instance.Free()
				End If
				Return Me.m_lazyExplicitInterfaceImplementationMap
			End Get
		End Property

		Public ReadOnly Property InstanceInitializers As ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer))
			Get
				Return Me.MemberAndInitializerLookup.InstanceInitializers
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsImplicitClass As Boolean
			Get
				Return Me._declaration.Declarations(0).Kind = Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind.ImplicitClass
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsInterface As Boolean
			Get
				Return Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Interface]
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustInherit As Boolean
			Get
				Return CInt((Me._flags And SourceMemberContainerTypeSymbol.SourceTypeFlags.[MustInherit])) <> 0
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotInheritable As Boolean
			Get
				Return CInt((Me._flags And SourceMemberContainerTypeSymbol.SourceTypeFlags.[NotInheritable])) <> 0
			End Get
		End Property

		Friend ReadOnly Property IsPartial As Boolean
			Get
				Return CInt((Me._flags And SourceMemberContainerTypeSymbol.SourceTypeFlags.[Partial])) <> 0
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsScriptClass As Boolean
			Get
				Dim kind As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind = Me._declaration.Declarations(0).Kind
				If (kind = Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind.Script) Then
					Return True
				End If
				Return kind = Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind.Submission
			End Get
		End Property

		Friend Overrides ReadOnly Property KnownCircularStruct As Boolean
			Get
				If (Me._lazyStructureCycle = 0) Then
					If (Me.IsStructureType()) Then
						Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
						Dim flag As Boolean = Me.CheckStructureCircularity(instance)
						Me.m_containingModule.AtomicStoreIntegerAndDiagnostics(Me._lazyStructureCycle, If(flag, 2, 1), 0, instance)
						instance.Free()
					Else
						Me._lazyStructureCycle = 1
					End If
				End If
				Return Me._lazyStructureCycle = 2
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._declaration.NameLocations
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property MangleName As Boolean
			Get
				Return Me.Arity > 0
			End Get
		End Property

		Private ReadOnly Property MemberAndInitializerLookup As SourceMemberContainerTypeSymbol.MembersAndInitializers
			Get
				Return Me.GetMembersAndInitializers()
			End Get
		End Property

		Public Overrides ReadOnly Property MemberNames As IEnumerable(Of String)
			Get
				Return Me._declaration.MemberNames
			End Get
		End Property

		Friend ReadOnly Property MembersHaveBeenCreated As Boolean
			Get
				Return Me._lazyMembersAndInitializers IsNot Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				If (Me._lazyContainsExtensionMethods = ThreeState.Unknown AndAlso (Me._containingSymbol.Kind <> SymbolKind.[Namespace] OrElse Not Me.AllowsExtensionMethods() OrElse Not Me.AnyMemberHasAttributes)) Then
					Me._lazyContainsExtensionMethods = ThreeState.[False]
				End If
				Return Me._lazyContainsExtensionMethods <> ThreeState.[False]
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property ShadowsExplicitly As Boolean
			Get
				Return CInt((Me._flags And SourceMemberContainerTypeSymbol.SourceTypeFlags.[Shadows])) <> 0
			End Get
		End Property

		Public ReadOnly Property StaticInitializers As ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer))
			Get
				Return Me.MemberAndInitializerLookup.StaticInitializers
			End Get
		End Property

		Public ReadOnly Property SyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._declaration.SyntaxReferences
			End Get
		End Property

		Friend ReadOnly Property TypeDeclaration As MergedTypeDeclaration
			Get
				Return Me._declaration
			End Get
		End Property

		Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
			Get
				Return DirectCast(CByte(CUShort((CUShort((Me._flags And SourceMemberContainerTypeSymbol.SourceTypeFlags.TypeKindMask)) >> CUShort(SourceMemberContainerTypeSymbol.SourceTypeFlags.[Friend])))), Microsoft.CodeAnalysis.TypeKind)
			End Get
		End Property

		Shared Sub New()
			SourceMemberContainerTypeSymbol.s_emptyTypeMembers = New Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol))(CaseInsensitiveComparison.Comparer)
		End Sub

		Protected Sub New(ByVal declaration As MergedTypeDeclaration, ByVal containingSymbol As NamespaceOrTypeSymbol, ByVal containingModule As SourceModuleSymbol)
			MyBase.New()
			Me._lazyEmitExtensionAttribute = ThreeState.Unknown
			Me._lazyContainsExtensionMethods = ThreeState.Unknown
			Me._lazyAnyMemberHasAttributes = ThreeState.Unknown
			Me._lazyStructureCycle = 0
			Me._lazyLexicalSortKey = LexicalSortKey.NotInitialized
			Me.m_containingModule = containingModule
			Me._containingSymbol = containingSymbol
			Me._declaration = declaration
			Me._name = SourceMemberContainerTypeSymbol.GetBestName(declaration, containingModule.ContainingSourceAssembly.DeclaringCompilation)
			Me._flags = Me.ComputeTypeFlags(declaration, containingSymbol.IsNamespace)
		End Sub

		Protected MustOverride Sub AddDeclaredNonTypeMembers(ByVal membersBuilder As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)

		Private Sub AddDefaultConstructorIfNeeded(ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByVal isShared As Boolean, ByVal initializers As ArrayBuilder(Of ImmutableArray(Of FieldOrPropertyInitializer)), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.Submission) Then
				If (Not isShared OrElse Me.AnyInitializerToBeInjectedIntoConstructor(initializers, False)) Then
					Dim syntaxReference As Microsoft.CodeAnalysis.SyntaxReference = Me.SyntaxReferences.[Single]()
					Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(Me.m_containingModule, syntaxReference.SyntaxTree, Me)
					Dim synthesizedSubmissionConstructorSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedSubmissionConstructorSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedSubmissionConstructorSymbol(syntaxReference, Me, isShared, binder, diagnostics)
					Me.AddMember(synthesizedSubmissionConstructorSymbol, binder, members, False)
				End If
			ElseIf (Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Class] OrElse Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.Struct OrElse Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum] OrElse Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Module] AndAlso isShared) Then
				Dim beInjectedIntoConstructor As Boolean = Me.AnyInitializerToBeInjectedIntoConstructor(initializers, Not isShared)
				If (isShared AndAlso Not beInjectedIntoConstructor) Then
					Return
				End If
				Me.EnsureCtor(members, isShared, beInjectedIntoConstructor, diagnostics)
			End If
			If (Not isShared AndAlso Me.IsScriptClass) Then
				Dim syntaxReference1 As Microsoft.CodeAnalysis.SyntaxReference = Me.SyntaxReferences.[Single]()
				Dim synthesizedInteractiveInitializerMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedInteractiveInitializerMethod = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedInteractiveInitializerMethod(syntaxReference1, Me, diagnostics)
				Me.AddSymbolToMembers(synthesizedInteractiveInitializerMethod, members.Members)
				Me.AddSymbolToMembers(SynthesizedEntryPointSymbol.Create(synthesizedInteractiveInitializerMethod, diagnostics), members.Members)
			End If
		End Sub

		Protected Overridable Sub AddEntryPointIfNeeded(ByVal membersBuilder As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder)
		End Sub

		Private Sub AddEventAndAccessors(ByVal eventSymbol As SourceEventSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder)
			Me.AddMember(eventSymbol, binder, members, False)
			If (eventSymbol.AddMethod IsNot Nothing) Then
				Me.AddMember(eventSymbol.AddMethod, binder, members, False)
			End If
			If (eventSymbol.RemoveMethod IsNot Nothing) Then
				Me.AddMember(eventSymbol.RemoveMethod, binder, members, False)
			End If
			If (eventSymbol.RaiseMethod IsNot Nothing) Then
				Me.AddMember(eventSymbol.RaiseMethod, binder, members, False)
			End If
			If (eventSymbol.AssociatedField IsNot Nothing) Then
				Me.AddMember(eventSymbol.AssociatedField, binder, members, False)
			End If
		End Sub

		Friend Overrides Sub AddExtensionMethodLookupSymbolsInfo(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder, ByVal appendThrough As NamedTypeSymbol)
			If (Me.MightContainExtensionMethods AndAlso Not appendThrough.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, Me.MemberAndInitializerLookup.Members)) Then
				Me._lazyContainsExtensionMethods = ThreeState.[False]
			End If
		End Sub

		Protected Overridable Sub AddGroupClassMembersIfNeeded(ByVal membersBuilder As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
		End Sub

		Friend Shared Sub AddInitializer(ByRef initializers As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer), ByVal computeInitializer As Func(Of Integer, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer), ByRef aggregateSyntaxLength As Integer)
			Dim fieldOrPropertyInitializer As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer = computeInitializer(aggregateSyntaxLength)
			If (initializers Is Nothing) Then
				initializers = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer).GetInstance()
			End If
			initializers.Add(fieldOrPropertyInitializer)
			If (Not fieldOrPropertyInitializer.IsMetadataConstant) Then
				aggregateSyntaxLength += fieldOrPropertyInitializer.Syntax.Span.Length
			End If
		End Sub

		Friend Shared Sub AddInitializers(ByRef allInitializers As ArrayBuilder(Of ImmutableArray(Of FieldOrPropertyInitializer)), ByVal siblings As ArrayBuilder(Of FieldOrPropertyInitializer))
			If (siblings IsNot Nothing) Then
				If (allInitializers Is Nothing) Then
					allInitializers = New ArrayBuilder(Of ImmutableArray(Of FieldOrPropertyInitializer))()
				End If
				allInitializers.Add(siblings.ToImmutableAndFree())
			End If
		End Sub

		Protected Sub AddMember(ByVal memberSyntax As StatementSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByRef staticInitializers As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer), ByRef instanceInitializers As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer), ByVal reportAsInvalid As Boolean)
			Dim func As Func(Of Integer, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = memberSyntax.Kind()
			Select Case syntaxKind
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock
					Dim blockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax = DirectCast(memberSyntax, MethodBlockBaseSyntax).BlockStatement
					If (reportAsInvalid) Then
						diagBag.Add(ERRID.ERR_InvalidInNamespace, blockStatement.GetLocation())
					End If
					Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = Me.CreateMethodMember(blockStatement, binder, diagBag.DiagnosticBag)
					If (sourceMethodSymbol Is Nothing) Then
						Exit Select
					End If
					Me.AddMember(sourceMethodSymbol, binder, members, False)
					Return
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
				Label0:
					If (memberSyntax.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement AndAlso Not TypeOf memberSyntax Is ExecutableStatementSyntax) Then
						Exit Select
					End If
					If (binder.BindingTopLevelScriptCode) Then
						Dim fieldOrPropertyInitializer As VB$AnonymousDelegate_0(Of Integer, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer) = Function(precedingInitializersLength As Integer) New Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer(binder.GetSyntaxReference(memberSyntax), precedingInitializersLength)
						Dim variable As VB$AnonymousDelegate_0(Of Integer, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer) = fieldOrPropertyInitializer
						If (variable Is Nothing) Then
							func = Nothing
						Else
							func = New Func(Of Integer, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer)(AddressOf variable.Invoke)
						End If
						SourceMemberContainerTypeSymbol.AddInitializer(instanceInitializers, func, members.InstanceSyntaxLength)
						Return
					End If
					If (Not reportAsInvalid) Then
						Exit Select
					End If
					diagBag.Add(ERRID.ERR_InvalidInNamespace, memberSyntax.GetLocation())
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock
					Dim propertyBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax = DirectCast(memberSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax)
					If (reportAsInvalid) Then
						diagBag.Add(ERRID.ERR_InvalidInNamespace, propertyBlockSyntax.PropertyStatement.GetLocation())
					End If
					Me.CreateProperty(propertyBlockSyntax.PropertyStatement, propertyBlockSyntax, binder, diagBag.DiagnosticBag, members, staticInitializers, instanceInitializers)
					Return
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock
					Dim eventBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax = DirectCast(memberSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax)
					Me.CreateEvent(eventBlockSyntax.EventStatement, eventBlockSyntax, binder, diagBag.DiagnosticBag, members)
					Return
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement
					Dim methodBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax = DirectCast(memberSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax)
					If (reportAsInvalid) Then
						diagBag.Add(ERRID.ERR_InvalidInNamespace, methodBaseSyntax.GetLocation())
					End If
					Dim sourceMethodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = Me.CreateMethodMember(DirectCast(memberSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax), binder, diagBag.DiagnosticBag)
					If (sourceMethodSymbol1 Is Nothing) Then
						Exit Select
					End If
					Me.AddMember(sourceMethodSymbol1, binder, members, False)
					Return
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement
					Dim eventStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax = DirectCast(memberSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax)
					Me.CreateEvent(eventStatementSyntax, Nothing, binder, diagBag.DiagnosticBag, members)
					Return
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement
					Dim propertyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax = DirectCast(memberSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax)
					If (reportAsInvalid) Then
						diagBag.Add(ERRID.ERR_InvalidInNamespace, propertyStatementSyntax.GetLocation())
					End If
					Me.CreateProperty(propertyStatementSyntax, Nothing, binder, diagBag.DiagnosticBag, members, staticInitializers, instanceInitializers)
					Return
				Case Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration) Then
						Dim fieldDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax = DirectCast(memberSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax)
						If (reportAsInvalid) Then
							diagBag.Add(ERRID.ERR_InvalidInNamespace, fieldDeclarationSyntax.GetLocation())
						End If
						SourceMemberFieldSymbol.Create(Me, fieldDeclarationSyntax, binder, members, staticInitializers, instanceInitializers, diagBag)
						Return
					End If
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement) Then
						Exit Select
					End If
					GoTo Label0
			End Select
		End Sub

		Friend Sub AddMember(ByVal sym As Symbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByVal omitDiagnostics As Boolean)
			If (Not omitDiagnostics) Then
				members.DeferredMemberDiagnostic.Add(sym)
			End If
			Me.AddSymbolToMembers(sym, members.Members)
		End Sub

		Private Sub AddPropertyAndAccessors(ByVal propertySymbol As SourcePropertySymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder)
			Me.AddMember(propertySymbol, binder, members, False)
			If (propertySymbol.GetMethod IsNot Nothing) Then
				Me.AddMember(propertySymbol.GetMethod, binder, members, False)
			End If
			If (propertySymbol.SetMethod IsNot Nothing) Then
				Me.AddMember(propertySymbol.SetMethod, binder, members, False)
			End If
			If (propertySymbol.AssociatedField IsNot Nothing) Then
				Me.AddMember(propertySymbol.AssociatedField, binder, members, False)
			End If
		End Sub

		Friend Sub AddSymbolToMembers(ByVal memberSymbol As Symbol, ByVal members As Dictionary(Of String, ArrayBuilder(Of Symbol)))
			Dim symbols As ArrayBuilder(Of Symbol) = Nothing
			If (members.TryGetValue(memberSymbol.Name, symbols)) Then
				symbols.Add(memberSymbol)
				Return
			End If
			symbols = New ArrayBuilder(Of Symbol)() From
			{
				memberSymbol
			}
			members(memberSymbol.Name) = symbols
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			If (Me.EmitExtensionAttribute) Then
				Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.SynthesizeExtensionAttribute())
			End If
		End Sub

		Private Sub AddWithEventsHookupConstructorsIfNeeded(ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim enumerator As Dictionary(Of String, ArrayBuilder(Of Symbol)).ValueCollection.Enumerator = New Dictionary(Of String, ArrayBuilder(Of Symbol)).ValueCollection.Enumerator()
			If (Me.TypeKind <> Microsoft.CodeAnalysis.TypeKind.Submission AndAlso (Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Class] OrElse Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Module])) Then
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol) = Nothing
				Try
					enumerator = members.Members.Values.GetEnumerator()
					While enumerator.MoveNext()
						Dim enumerator1 As ArrayBuilder(Of Symbol).Enumerator = enumerator.Current.GetEnumerator()
						While enumerator1.MoveNext()
							Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = TryCast(enumerator1.Current, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol)
							If (current Is Nothing OrElse Not current.HandlesEvents) Then
								Continue While
							End If
							If (instance Is Nothing) Then
								instance = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol).GetInstance()
							End If
							instance.Add(current)
						End While
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
				If (instance IsNot Nothing) Then
					Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing
					Dim enumerator2 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol).Enumerator = instance.GetEnumerator()
					While enumerator2.MoveNext()
						Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = enumerator2.Current
						Dim declarationSyntax As MethodStatementSyntax = DirectCast(sourceMethodSymbol.DeclarationSyntax, MethodStatementSyntax)
						Dim enumerator3 As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax).Enumerator = declarationSyntax.HandlesClause.Events.GetEnumerator()
						While enumerator3.MoveNext()
							Dim handlesClauseItemSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax = enumerator3.Current
							If (handlesClauseItemSyntax.EventContainer.Kind() <> SyntaxKind.KeywordEventContainer) Then
								Continue While
							End If
							If (sourceMethodSymbol.IsShared) Then
								Dim valueText As String = handlesClauseItemSyntax.EventMember.Identifier.ValueText
								Dim item As EventSymbol = Nothing
								If (handlesClauseItemSyntax.EventContainer.Kind() <> SyntaxKind.MyBaseKeyword) Then
									Dim symbols As ArrayBuilder(Of Symbol) = Nothing
									If (members.Members.TryGetValue(valueText, symbols) AndAlso symbols.Count = 1 AndAlso symbols(0).Kind = SymbolKind.[Event]) Then
										item = DirectCast(symbols(0), EventSymbol)
									End If
								End If
								If (item Is Nothing) Then
									binder = If(binder, BinderBuilder.CreateBinderForType(Me.m_containingModule, declarationSyntax.SyntaxTree, Me))
									Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(diagBag, Me.m_containingModule.ContainingAssembly)
									Dim lookupResultKind As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind = Microsoft.CodeAnalysis.VisualBasic.LookupResultKind.Empty
									item = SourceMemberMethodSymbol.FindEvent(MyBase.BaseTypeNoUseSiteDiagnostics, binder, valueText, True, compoundUseSiteInfo, Nothing, lookupResultKind)
									diagBag.Add(handlesClauseItemSyntax.EventMember, compoundUseSiteInfo)
								End If
								If (item Is Nothing) Then
									Continue While
								End If
								Me.EnsureCtor(members, item.IsShared, False, diagBag)
							Else
								Me.EnsureCtor(members, False, False, diagBag)
							End If
						End While
					End While
					instance.Free()
				End If
			End If
		End Sub

		Friend Function AnyInitializerToBeInjectedIntoConstructor(ByVal initializerSet As IEnumerable(Of ImmutableArray(Of FieldOrPropertyInitializer)), ByVal includingNonMetadataConstants As Boolean) As Boolean
			Dim flag As Boolean
			Dim enumerator As IEnumerator(Of ImmutableArray(Of FieldOrPropertyInitializer)) = Nothing
			If (initializerSet IsNot Nothing) Then
				Try
					enumerator = initializerSet.GetEnumerator()
					While enumerator.MoveNext()
						Dim enumerator1 As ImmutableArray(Of FieldOrPropertyInitializer).Enumerator = enumerator.Current.GetEnumerator()
						While enumerator1.MoveNext()
							Dim fieldsOrProperties As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = enumerator1.Current.FieldsOrProperties
							If (fieldsOrProperties.IsDefault) Then
								Continue While
							End If
							Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = fieldsOrProperties.First()
							If (symbol.Kind <> SymbolKind.[Property]) Then
								Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
								If (fieldSymbol.IsConst AndAlso (Not includingNonMetadataConstants OrElse Not fieldSymbol.IsConstButNotMetadataConstant)) Then
									Continue While
								End If
								flag = True
								Return flag
							Else
								flag = True
								Return flag
							End If
						End While
					End While
				Finally
					If (enumerator IsNot Nothing) Then
						enumerator.Dispose()
					End If
				End Try
			End If
			flag = False
			Return flag
		End Function

		Private Shared Sub AppendVarianceDiagnosticInfo(<InAttribute> <Out> ByRef diagnostics As ArrayBuilder(Of DiagnosticInfo), ByVal info As DiagnosticInfo)
			If (diagnostics Is Nothing) Then
				diagnostics = ArrayBuilder(Of DiagnosticInfo).GetInstance()
			End If
			diagnostics.Add(info)
		End Sub

		Private Sub BindAllMemberAttributes(ByVal cancellationToken As System.Threading.CancellationToken)
			Dim enumerator As Dictionary(Of String, ImmutableArray(Of Symbol)).ValueCollection.Enumerator = New Dictionary(Of String, ImmutableArray(Of Symbol)).ValueCollection.Enumerator()
			Dim memberAndInitializerLookup As SourceMemberContainerTypeSymbol.MembersAndInitializers = Me.MemberAndInitializerLookup
			Dim flag As Boolean = False
			Try
				enumerator = memberAndInitializerLookup.Members.Values.GetEnumerator()
				While enumerator.MoveNext()
					Dim enumerator1 As ImmutableArray(Of Symbol).Enumerator = enumerator.Current.GetEnumerator()
					While enumerator1.MoveNext()
						Dim current As Symbol = enumerator1.Current
						current.GetAttributes()
						If (Not flag) Then
							flag = If(current.Kind <> SymbolKind.Method, False, DirectCast(current, MethodSymbol).IsExtensionMethod)
						End If
						cancellationToken.ThrowIfCancellationRequested()
					End While
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			If (Not flag) Then
				Me._lazyContainsExtensionMethods = ThreeState.[False]
				Me._lazyEmitExtensionAttribute = ThreeState.[False]
			Else
				Me.m_containingModule.RecordPresenceOfExtensionMethods()
				Me._lazyContainsExtensionMethods = ThreeState.[True]
				If (Me._lazyEmitExtensionAttribute = ThreeState.Unknown) Then
					Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)()
					Me.m_containingModule.ContainingSourceAssembly.DeclaringCompilation.GetExtensionAttributeConstructor(useSiteInfo)
					If (useSiteInfo.DiagnosticInfo Is Nothing) Then
						Me._lazyEmitExtensionAttribute = ThreeState.[True]
						Return
					End If
					Me._lazyEmitExtensionAttribute = ThreeState.[False]
					Me.m_containingModule.ContainingSourceAssembly.AnErrorHasBeenReportedAboutExtensionAttribute()
					Return
				End If
			End If
		End Sub

		Friend Overrides Sub BuildExtensionMethodsMap(ByVal map As Dictionary(Of String, ArrayBuilder(Of MethodSymbol)), ByVal appendThrough As NamespaceSymbol)
			If (Me.MightContainExtensionMethods AndAlso Not appendThrough.BuildExtensionMethodsMap(map, Me.MemberAndInitializerLookup.Members)) Then
				Me._lazyContainsExtensionMethods = ThreeState.[False]
			End If
		End Sub

		Private Function BuildMembersAndInitializers(ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As SourceMemberContainerTypeSymbol.MembersAndInitializers
			Dim enumerator As Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol)).ValueCollection.Enumerator = New Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol)).ValueCollection.Enumerator()
			Dim typeMembersDictionary As Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol)) = Me.GetTypeMembersDictionary()
			Dim membersAndInitializer As SourceMemberContainerTypeSymbol.MembersAndInitializers = Me.BuildNonTypeMembers(diagBag)
			Me._defaultPropertyName = Me.DetermineDefaultPropertyName(membersAndInitializer.Members, diagBag)
			Me.ProcessPartialMethodsIfAny(membersAndInitializer.Members, diagBag)
			Try
				enumerator = typeMembersDictionary.Values.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ImmutableArray(Of NamedTypeSymbol) = enumerator.Current
					Dim symbols As ImmutableArray(Of Symbol) = New ImmutableArray(Of Symbol)()
					Dim name As String = current(0).Name
					If (membersAndInitializer.Members.TryGetValue(name, symbols)) Then
						membersAndInitializer.Members(name) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.Concat(Of Symbol)(symbols, StaticCast(Of Symbol).From(Of NamedTypeSymbol)(current))
					Else
						membersAndInitializer.Members.Add(name, StaticCast(Of Symbol).From(Of NamedTypeSymbol)(current))
					End If
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			Return membersAndInitializer
		End Function

		Private Function BuildNonTypeMembers(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As SourceMemberContainerTypeSymbol.MembersAndInitializers
			Dim membersAndInitializersBuilder As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder = New SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder()
			Me.AddDeclaredNonTypeMembers(membersAndInitializersBuilder, diagnostics)
			Me.AddDefaultConstructorIfNeeded(membersAndInitializersBuilder, False, membersAndInitializersBuilder.InstanceInitializers, diagnostics)
			Me.AddDefaultConstructorIfNeeded(membersAndInitializersBuilder, True, membersAndInitializersBuilder.StaticInitializers, diagnostics)
			Me.AddWithEventsHookupConstructorsIfNeeded(membersAndInitializersBuilder, diagnostics)
			Me.AddGroupClassMembersIfNeeded(membersAndInitializersBuilder, diagnostics)
			Me.AddEntryPointIfNeeded(membersAndInitializersBuilder)
			Me.CheckMemberDiagnostics(membersAndInitializersBuilder, diagnostics)
			Dim readOnlyAndFree As SourceMemberContainerTypeSymbol.MembersAndInitializers = membersAndInitializersBuilder.ToReadOnlyAndFree()
			Me.CheckForOverloadOverridesShadowsClashesInSameType(readOnlyAndFree, diagnostics)
			Return readOnlyAndFree
		End Function

		Friend Function CalculateSyntaxOffsetInSynthesizedConstructor(ByVal position As Integer, ByVal tree As SyntaxTree, ByVal isShared As Boolean) As Integer
			Dim num As Integer
			Dim num1 As Integer = 0
			If (Me.IsScriptClass AndAlso Not isShared) Then
				Dim length As Integer = 0
				Dim enumerator As ImmutableArray(Of SingleTypeDeclaration).Enumerator = Me._declaration.Declarations.GetEnumerator()
				While enumerator.MoveNext()
					Dim syntaxReference As Microsoft.CodeAnalysis.SyntaxReference = enumerator.Current.SyntaxReference
					If (tree <> syntaxReference.SyntaxTree) Then
						length += syntaxReference.Span.Length
					Else
						num = length + position
						Return num
					End If
				End While
				Throw ExceptionUtilities.Unreachable
			ElseIf (Not Me.TryCalculateSyntaxOffsetOfPositionInInitializer(position, tree, isShared, num1)) Then
				If (Me._declaration.Declarations.Length < 1 OrElse position <> Me._declaration.Declarations(0).Location.SourceSpan.Start) Then
					Throw ExceptionUtilities.Unreachable
				End If
				num = 0
			Else
				num = num1
			End If
			Return num
		End Function

		Private Sub CheckDefaultPropertyAgainstAllBases(ByVal namedType As NamedTypeSymbol, ByVal defaultPropertyName As String, ByVal location As Microsoft.CodeAnalysis.Location, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (Not namedType.IsInterfaceType()) Then
				Me.CheckDefaultPropertyAgainstBase(defaultPropertyName, namedType.BaseTypeNoUseSiteDiagnostics, location, diagBag)
				Return
			End If
			Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = namedType.InterfacesNoUseSiteDiagnostics.GetEnumerator()
			While enumerator.MoveNext()
				Me.CheckDefaultPropertyAgainstBase(defaultPropertyName, enumerator.Current, location, diagBag)
			End While
		End Sub

		Private Sub CheckDefaultPropertyAgainstBase(ByVal defaultPropertyName As String, ByVal baseType As NamedTypeSymbol, ByVal location As Microsoft.CodeAnalysis.Location, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (baseType IsNot Nothing) Then
				Dim str As String = baseType.DefaultPropertyName
				If (str Is Nothing) Then
					Me.CheckDefaultPropertyAgainstAllBases(baseType, defaultPropertyName, location, diagBag)
				ElseIf (Not CaseInsensitiveComparison.Equals(defaultPropertyName, str)) Then
					diagBag.Add(ERRID.WRN_DefaultnessShadowed4, location, New [Object]() { defaultPropertyName, str, baseType.GetKindText(), CustomSymbolDisplayFormatter.ShortErrorName(baseType) })
					Return
				End If
			End If
		End Sub

		Private Function CheckForOperatorOverloadingErrors(ByVal memberList As ImmutableArray(Of Symbol), ByVal memberIndex As Integer, ByVal membersEnumerator As Dictionary(Of String, ImmutableArray(Of Symbol)).Enumerator, <InAttribute> <Out> ByRef operatorsKnownToHavePair As HashSet(Of MethodSymbol), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol::CheckForOperatorOverloadingErrors(System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.VisualBasic.Symbol>,System.Int32,System.Collections.Generic.Dictionary`2/Enumerator<System.String,System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.VisualBasic.Symbol>>,System.Collections.Generic.HashSet`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol>&,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean CheckForOperatorOverloadingErrors(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.VisualBasic.Symbol>,System.Int32,System.Collections.Generic.Dictionary<u00210, u00211>/Enumerator<System.String,System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.VisualBasic.Symbol>>,System.Collections.Generic.HashSet<Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol>&,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Sub CheckForOverloadOverridesShadowsClashesInSameType(ByVal membersAndInitializers As SourceMemberContainerTypeSymbol.MembersAndInitializers, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim enumerator As Dictionary(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)).Enumerator = New Dictionary(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)).Enumerator()
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			Dim flag2 As Boolean = False
			Try
				enumerator = membersAndInitializers.Members.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As KeyValuePair(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)) = enumerator.Current
					Dim flag3 As Boolean = True
					Dim flag4 As Boolean = True
					Dim flag5 As Boolean = False
					Dim flag6 As Boolean = False
					Dim value As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = current.Value
					Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = value.GetEnumerator()
					While enumerator1.MoveNext()
						Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator1.Current
						Dim kind As SymbolKind = symbol.Kind
						If (kind = SymbolKind.Method) Then
							If (Not flag4) Then
								Continue While
							End If
							flag3 = False
						ElseIf (kind = SymbolKind.[Property]) Then
							If (Not flag3) Then
								Continue While
							End If
							flag4 = False
						Else
							flag5 = False
							flag6 = False
							Exit While
						End If
						If (Not Me.GetExplicitSymbolFlags(symbol, flag, flag1, flag2)) Then
							Continue While
						End If
						If (Not flag) Then
							If (Not flag1 AndAlso Not flag2) Then
								Continue While
							End If
							flag6 = True
						Else
							flag5 = True
							Exit While
						End If
					End While
					If (Not flag5 AndAlso Not flag6) Then
						Continue While
					End If
					value = current.Value
					Dim enumerator2 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = value.GetEnumerator()
					While enumerator2.MoveNext()
						Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator2.Current
						If ((current1.Kind <> SymbolKind.Method OrElse Not flag4) AndAlso (Not current1.IsPropertyAndNotWithEvents() OrElse Not flag3) OrElse Not Me.GetExplicitSymbolFlags(current1, flag, flag1, flag2)) Then
							Continue While
						End If
						If (Not flag5) Then
							If (Not flag6 OrElse flag2 OrElse flag1) Then
								Continue While
							End If
							diagBag.Add(ERRID.ERR_MustBeOverloads2, current1.Locations(0), New [Object]() { current1.GetKindText(), current1.Name })
						Else
							If (flag) Then
								Continue While
							End If
							diagBag.Add(ERRID.ERR_MustShadow2, current1.Locations(0), New [Object]() { current1.GetKindText(), current1.Name })
						End If
					End While
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
		End Sub

		Private Sub CheckForOverloadsErrors(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol::CheckForOverloadsErrors(Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void CheckForOverloadsErrors(Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Sub

		Private Function CheckIfMemberNameConflictsWithTypeMember(ByVal sym As Symbol, ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Boolean
			Dim flag As Boolean
			Dim typeMembers As ImmutableArray(Of NamedTypeSymbol) = Me.GetTypeMembers(sym.Name)
			If (typeMembers.Length > 0) Then
				Dim item As NamedTypeSymbol = typeMembers(0)
				If (TypeSymbol.Equals(TryCast(sym, TypeSymbol), item, TypeCompareKind.ConsiderEverything)) Then
					flag = False
					Return flag
				End If
				flag = Me.CheckIfMemberNameIsDuplicate(sym, item, members, diagBag, True)
				Return flag
			End If
			flag = False
			Return flag
		End Function

		Private Function CheckIfMemberNameIsDuplicate(ByVal sym As Symbol, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder) As Boolean
			Dim flag As Boolean
			Dim symbols As ArrayBuilder(Of Symbol) = Nothing
			If (members.Members.TryGetValue(sym.Name, symbols)) Then
				Dim item As Symbol = symbols(0)
				If (sym = item) Then
					flag = False
					Return flag
				End If
				flag = Me.CheckIfMemberNameIsDuplicate(sym, item, members, diagBag, False)
				Return flag
			End If
			flag = False
			Return flag
		End Function

		Private Function CheckIfMemberNameIsDuplicate(ByVal firstSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal secondSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal includeKind As Boolean) As Boolean
			Dim flag As Boolean
			Dim obj As [Object]
			Dim implicitlyDefinedBy As Microsoft.CodeAnalysis.VisualBasic.Symbol = secondSymbol(members.Members)
			If (implicitlyDefinedBy Is Nothing AndAlso secondSymbol.IsUserDefinedOperator()) Then
				implicitlyDefinedBy = secondSymbol
			End If
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = firstSymbol(members.Members)
			If (symbol Is Nothing AndAlso firstSymbol.IsUserDefinedOperator()) Then
				symbol = firstSymbol
			End If
			If (implicitlyDefinedBy IsNot Nothing) Then
				If (symbol Is Nothing) Then
					GoTo Label2
				End If
				If (Not CaseInsensitiveComparison.Equals(implicitlyDefinedBy.Name, symbol.Name)) Then
					Binder.ReportDiagnostic(diagBag, implicitlyDefinedBy.Locations(0), ERRID.ERR_SynthMemberClashesWithSynth7, New [Object]() { implicitlyDefinedBy.GetKindText(), OverrideHidingHelper.AssociatedSymbolName(implicitlyDefinedBy), secondSymbol.Name, symbol.GetKindText(), OverrideHidingHelper.AssociatedSymbolName(symbol), Me.GetKindText(), Me.Name })
				End If
			ElseIf (symbol Is Nothing) Then
				If ((firstSymbol.Kind = SymbolKind.Method OrElse firstSymbol.IsPropertyAndNotWithEvents()) AndAlso firstSymbol.Kind = secondSymbol.Kind) Then
					flag = False
					Return flag
				End If
				If (Not Me.IsEnumType()) Then
					Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = diagBag
					Dim item As Location = firstSymbol.Locations(0)
					Dim name() As [Object] = { firstSymbol.Name, Nothing, Nothing }
					If (includeKind) Then
						obj = CustomSymbolDisplayFormatter.ErrorNameWithKind(secondSymbol)
					Else
						obj = secondSymbol
					End If
					name(1) = obj
					name(2) = Me.GetKindText()
					Binder.ReportDiagnostic(bindingDiagnosticBag, item, ERRID.ERR_MultiplyDefinedType3, name)
				Else
					Binder.ReportDiagnostic(diagBag, firstSymbol.Locations(0), ERRID.ERR_MultiplyDefinedEnumMember2, New [Object]() { firstSymbol.Name, Me.GetKindText() })
				End If
				flag = True
				Return flag
			Else
				Binder.ReportDiagnostic(diagBag, secondSymbol.Locations(0), ERRID.ERR_MemberClashesWithSynth6, New [Object]() { secondSymbol.GetKindText(), secondSymbol.Name, symbol.GetKindText(), OverrideHidingHelper.AssociatedSymbolName(symbol), Me.GetKindText(), Me.Name })
				flag = True
				Return flag
			End If
			flag = False
			Return flag
		Label2:
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = TryCast(implicitlyDefinedBy, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
			If (typeSymbol Is Nothing OrElse Not typeSymbol.IsEnumType()) Then
				Binder.ReportDiagnostic(diagBag, implicitlyDefinedBy.Locations(0), ERRID.ERR_SynthMemberClashesWithMember5, New [Object]() { implicitlyDefinedBy.GetKindText(), OverrideHidingHelper.AssociatedSymbolName(implicitlyDefinedBy), secondSymbol.Name, Me.GetKindText(), Me.Name })
				flag = True
				Return flag
			Else
				flag = True
				Return flag
			End If
		End Function

		Private Sub CheckInterfaceUnificationAndVariance(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim value As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet
			Dim enumerator As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet).Enumerator = New Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet).Enumerator()
			Dim enumerator1 As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet.Enumerator = New MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet.Enumerator()
			Dim enumerator2 As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet).KeyCollection.Enumerator = New Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet).KeyCollection.Enumerator()
			Dim enumerator3 As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet).Enumerator = New Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet).Enumerator()
			Dim enumerator4 As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet.Enumerator = New MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet.Enumerator()
			Dim enumerator5 As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet.Enumerator = New MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet.Enumerator()
			Dim interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) = MyBase.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics
			If (Not interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.IsEmpty AndAlso (interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.Count <> 1 OrElse interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.Values.[Single]().Count <> 1)) Then
				If (Me.GetDeclaringSyntaxNode(Of VisualBasicSyntaxNode)() IsNot Nothing) Then
					Try
						enumerator = interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet) = enumerator.Current
							If (current.Value.Count <= 1) Then
								Continue While
							End If
							Dim key As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = current.Key
							Try
								value = current.Value
								enumerator1 = value.GetEnumerator()
								While enumerator1.MoveNext()
									Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator1.Current
									If (CObj(key) = CObj(namedTypeSymbol)) Then
										Continue While
									End If
									Me.ReportDuplicateInterfaceWithDifferentTupleNames(diagnostics, namedTypeSymbol, key)
								End While
							Finally
								DirectCast(enumerator1, IDisposable).Dispose()
							End Try
						End While
					Finally
						DirectCast(enumerator, IDisposable).Dispose()
					End Try
				End If
				Dim namedTypeSymbols As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) = New MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)()
				Try
					enumerator2 = interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.Keys.GetEnumerator()
					While enumerator2.MoveNext()
						Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator2.Current
						If (Not current1.IsGenericType) Then
							Continue While
						End If
						namedTypeSymbols.Add(current1.OriginalDefinition, current1)
					End While
				Finally
					DirectCast(enumerator2, IDisposable).Dispose()
				End Try
				Try
					enumerator3 = namedTypeSymbols.GetEnumerator()
					While enumerator3.MoveNext()
						Dim keyValuePair As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet) = enumerator3.Current
						If (keyValuePair.Value.Count < 2) Then
							Continue While
						End If
						Dim num As Integer = 0
						Try
							value = keyValuePair.Value
							enumerator4 = value.GetEnumerator()
							While enumerator4.MoveNext()
								Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator4.Current
								Dim num1 As Integer = 0
								Try
									value = keyValuePair.Value
									enumerator5 = value.GetEnumerator()
									While enumerator5.MoveNext()
										Dim current2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator5.Current
										If (num1 > num) Then
											If (Not TypeUnification.CanUnify(Me, namedTypeSymbol1, current2)) Then
												Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
												If (VarianceAmbiguity.HasVarianceAmbiguity(Me, namedTypeSymbol1, current2, discarded)) Then
													Me.ReportVarianceAmbiguityWarning(diagnostics, namedTypeSymbol1, current2)
												End If
											Else
												Me.ReportInterfaceUnificationError(diagnostics, namedTypeSymbol1, current2)
											End If
										End If
										num1 = num1 + 1
									End While
								Finally
									DirectCast(enumerator5, IDisposable).Dispose()
								End Try
								num = num + 1
							End While
						Finally
							DirectCast(enumerator4, IDisposable).Dispose()
						End Try
					End While
				Finally
					DirectCast(enumerator3, IDisposable).Dispose()
				End Try
			End If
		End Sub

		Private Sub CheckMemberDiagnostics(ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (Me.Locations.Length <= 1 OrElse Me.IsPartial) Then
				Dim enumerator As ArrayBuilder(Of Symbol).Enumerator = members.DeferredMemberDiagnostic.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					If (Not Me.CheckIfMemberNameConflictsWithTypeMember(current, members, diagBag)) Then
						Me.CheckIfMemberNameIsDuplicate(current, diagBag, members)
					End If
					If (Not current.CanBeReferencedByName OrElse Not Me.TypeParameters.MatchesAnyName(current.Name)) Then
						Continue While
					End If
					If (Not current.IsImplicitlyDeclared) Then
						Binder.ReportDiagnostic(diagBag, current.Locations(0), ERRID.ERR_ShadowingGenericParamWithMember1, New [Object]() { current.Name })
					Else
						Dim implicitlyDefinedBy As Symbol = current(members.Members)
						Binder.ReportDiagnostic(diagBag, implicitlyDefinedBy.Locations(0), ERRID.ERR_SyntMemberShadowsGenericParam3, New [Object]() { implicitlyDefinedBy.GetKindText(), implicitlyDefinedBy.Name, current.Name })
					End If
				End While
			End If
		End Sub

		Private Function CheckStructureCircularity(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Boolean
			Dim instance As SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet = SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet.GetInstance()
			instance.Queue.Enqueue(New SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet.QueueElement(Me, ConsList(Of FieldSymbol).Empty))
			Dim flag As Boolean = False
			Try
				While instance.Queue.Count > 0
					Dim queueElement As SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet.QueueElement = instance.Queue.Dequeue()
					If (Not instance.ProcessedTypes.Add(queueElement.Type)) Then
						Continue While
					End If
					Dim flag1 As Boolean = False
					Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = queueElement.Type.GetMembers().GetEnumerator()
					While enumerator.MoveNext()
						Dim current As FieldSymbol = TryCast(enumerator.Current, FieldSymbol)
						If (current Is Nothing OrElse current.IsShared) Then
							Continue While
						End If
						Dim type As NamedTypeSymbol = TryCast(current.Type, NamedTypeSymbol)
						If (type Is Nothing OrElse Not type.IsValueType OrElse Not current.IsDefinition AndAlso current.Type.Equals(current.OriginalDefinition.Type)) Then
							Continue While
						End If
						If (Not type.OriginalDefinition.Equals(Me)) Then
							If (instance.ProcessedTypes.Contains(type)) Then
								Continue While
							End If
							If (Not type.IsDefinition) Then
								instance.Queue.Enqueue(New SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet.QueueElement(type, New ConsList(Of FieldSymbol)(current, queueElement.Path)))
								type = type.OriginalDefinition
							End If
							If (Not Me.DetectTypeCircularity_ShouldStepIntoType(type)) Then
								instance.ProcessedTypes.Add(type)
							Else
								instance.Queue.Enqueue(New SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet.QueueElement(type, New ConsList(Of FieldSymbol)(current, queueElement.Path)))
							End If
						Else
							If (flag1) Then
								Continue While
							End If
							Dim fieldSymbols As ConsList(Of FieldSymbol) = New ConsList(Of FieldSymbol)(current, queueElement.Path)
							Dim diagnosticInfos As ArrayBuilder(Of DiagnosticInfo) = ArrayBuilder(Of DiagnosticInfo).GetInstance()
							Dim head As FieldSymbol = Nothing
							While Not fieldSymbols.IsEmpty()
								head = fieldSymbols.Head
								diagnosticInfos.Add(ErrorFactory.ErrorInfo(ERRID.ERR_RecordEmbeds2, New [Object]() { head.ContainingType, head.Type, head.Name }))
								fieldSymbols = fieldSymbols.Tail
							End While
							diagnosticInfos.ReverseContents()
							Dim associatedSymbol As [Object] = head.AssociatedSymbol
							If (associatedSymbol Is Nothing) Then
								associatedSymbol = head
							End If
							Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = DirectCast(associatedSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbol)
							diagnostics.Add(ERRID.ERR_RecordCycle2, symbol.Locations(0), New [Object]() { head.ContainingType.Name, New CompoundDiagnosticInfo(diagnosticInfos.ToArrayAndFree()) })
							flag1 = True
							flag = True
						End If
					End While
				End While
			Finally
				instance.Free()
			End Try
			Return flag
		End Function

		Private Function ComparePartialMethodSignatures(ByVal partialDeclaration As SourceMethodSymbol, ByVal candidate As SourceMethodSymbol) As Boolean
			Dim flag As Boolean
			If (CInt(MethodSignatureComparer.DetailedCompare(partialDeclaration, candidate, SymbolComparisonResults.NameMismatch Or SymbolComparisonResults.ReturnTypeMismatch Or SymbolComparisonResults.ArityMismatch Or SymbolComparisonResults.CustomModifierMismatch Or SymbolComparisonResults.RequiredExtraParameterMismatch Or SymbolComparisonResults.OptionalParameterMismatch Or SymbolComparisonResults.RequiredParameterTypeMismatch Or SymbolComparisonResults.OptionalParameterTypeMismatch Or SymbolComparisonResults.ParameterByrefMismatch Or SymbolComparisonResults.PropertyAccessorMismatch Or SymbolComparisonResults.PropertyInitOnlyMismatch Or SymbolComparisonResults.VarargMismatch Or SymbolComparisonResults.TotalParameterCountMismatch Or SymbolComparisonResults.TupleNamesMismatch, 0)) = 0) Then
				flag = If(partialDeclaration.IsShared <> candidate.IsShared OrElse partialDeclaration.IsOverrides <> candidate.IsOverrides, False, partialDeclaration.IsMustOverride = candidate.IsMustOverride)
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function ComputeTypeFlags(ByVal declaration As MergedTypeDeclaration, ByVal isTopLevel As Boolean) As SourceMemberContainerTypeSymbol.SourceTypeFlags
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol/SourceTypeFlags Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol::ComputeTypeFlags(Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedTypeDeclaration,System.Boolean)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol/SourceTypeFlags ComputeTypeFlags(Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedTypeDeclaration,System.Boolean)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Shared Function Create(ByVal declaration As MergedTypeDeclaration, ByVal containingSymbol As NamespaceOrTypeSymbol, ByVal containingModule As SourceModuleSymbol) As SourceMemberContainerTypeSymbol
			Dim embeddedNamedTypeSymbol As SourceMemberContainerTypeSymbol
			Dim embeddedKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind = declaration.SyntaxReferences.First().SyntaxTree.GetEmbeddedKind()
			If (embeddedKind <> Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind.None) Then
				embeddedNamedTypeSymbol = New EmbeddedSymbolManager.EmbeddedNamedTypeSymbol(declaration, containingSymbol, containingModule, embeddedKind)
			ElseIf (CByte(declaration.Kind) - CByte(Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind.Script) > CByte(Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind.[Interface])) Then
				Dim sourceNamedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol(declaration, containingSymbol, containingModule)
				If (sourceNamedTypeSymbol.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Module]) Then
					sourceNamedTypeSymbol.DeclaringCompilation.EmbeddedSymbolManager.RegisterModuleDeclaration()
				End If
				embeddedNamedTypeSymbol = sourceNamedTypeSymbol
			Else
				embeddedNamedTypeSymbol = New ImplicitNamedTypeSymbol(declaration, containingSymbol, containingModule)
			End If
			Return embeddedNamedTypeSymbol
		End Function

		Private Sub CreateEvent(ByVal syntax As EventStatementSyntax, ByVal blockSyntaxOpt As EventBlockSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag, ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder)
			Dim sourceEventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEventSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEventSymbol(Me, binder, syntax, blockSyntaxOpt, diagBag)
			Me.AddEventAndAccessors(sourceEventSymbol, binder, members)
		End Sub

		Private Function CreateMethodMember(ByVal methodBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol
			Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol
			Select Case methodBaseSyntax.Kind()
				Case SyntaxKind.SubStatement
				Case SyntaxKind.FunctionStatement
					sourceMethodSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol.CreateRegularMethod(Me, DirectCast(methodBaseSyntax, MethodStatementSyntax), binder, diagBag)
					Exit Select
				Case SyntaxKind.SubNewStatement
					sourceMethodSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol.CreateConstructor(Me, DirectCast(methodBaseSyntax, SubNewStatementSyntax), binder, diagBag)
					Exit Select
				Case SyntaxKind.DeclareSubStatement
				Case SyntaxKind.DeclareFunctionStatement
					sourceMethodSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol.CreateDeclareMethod(Me, DirectCast(methodBaseSyntax, DeclareStatementSyntax), binder, diagBag)
					Exit Select
				Case SyntaxKind.DelegateSubStatement
				Case SyntaxKind.DelegateFunctionStatement
				Case 100
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement
				Case SyntaxKind.EventStatement
					Throw ExceptionUtilities.UnexpectedValue(methodBaseSyntax.Kind())
				Case SyntaxKind.OperatorStatement
					sourceMethodSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol.CreateOperator(Me, DirectCast(methodBaseSyntax, OperatorStatementSyntax), binder, diagBag)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(methodBaseSyntax.Kind())
			End Select
			Return sourceMethodSymbol
		End Function

		Private Function CreateNestedType(ByVal declaration As MergedTypeDeclaration) As NamedTypeSymbol
			Dim sourceNamedTypeSymbol As NamedTypeSymbol
			If (declaration.Kind = Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind.[Delegate]) Then
				sourceNamedTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol(declaration, Me, Me.m_containingModule)
			ElseIf (declaration.Kind <> Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind.EventSyntheticDelegate) Then
				sourceNamedTypeSymbol = SourceMemberContainerTypeSymbol.Create(declaration, Me, Me.m_containingModule)
			Else
				sourceNamedTypeSymbol = New SynthesizedEventDelegateSymbol(declaration.SyntaxReferences(0), Me)
			End If
			Return sourceNamedTypeSymbol
		End Function

		Private Sub CreateProperty(ByVal syntax As PropertyStatementSyntax, ByVal blockSyntaxOpt As PropertyBlockSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag, ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByRef staticInitializers As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer), ByRef instanceInitializers As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer))
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
			Dim variable As VB$AnonymousDelegate_0(Of Integer, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer)
			Dim func As Func(Of Integer, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer)
			Dim func1 As Func(Of Integer, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer)
			Dim sourcePropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol.Create(Me, binder, syntax, blockSyntaxOpt, diagBag)
			Me.AddPropertyAndAccessors(sourcePropertySymbol, binder, members)
			Dim initializer As EqualsValueSyntax = syntax.Initializer
			Dim asClause As AsClauseSyntax = syntax.AsClause
			If (asClause Is Nothing OrElse asClause.Kind() <> SyntaxKind.AsNewClause) Then
				visualBasicSyntaxNode = initializer
			Else
				visualBasicSyntaxNode = asClause
			End If
			If (visualBasicSyntaxNode IsNot Nothing) Then
				Dim syntaxReference As Microsoft.CodeAnalysis.SyntaxReference = binder.GetSyntaxReference(visualBasicSyntaxNode)
				Dim fieldOrPropertyInitializer As VB$AnonymousDelegate_0(Of Integer, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer) = Function(precedingInitializersLength As Integer) New Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer(sourcePropertySymbol, syntaxReference, precedingInitializersLength)
				If (sourcePropertySymbol.IsShared) Then
					variable = fieldOrPropertyInitializer
					If (variable Is Nothing) Then
						func1 = Nothing
					Else
						func1 = New Func(Of Integer, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer)(AddressOf variable.Invoke)
					End If
					SourceMemberContainerTypeSymbol.AddInitializer(staticInitializers, func1, members.StaticSyntaxLength)
					Return
				End If
				If (sourcePropertySymbol.IsAutoProperty AndAlso sourcePropertySymbol.ContainingType.TypeKind = Microsoft.CodeAnalysis.TypeKind.Struct) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, syntax.Identifier, ERRID.ERR_AutoPropertyInitializedInStructure)
				End If
				variable = fieldOrPropertyInitializer
				If (variable Is Nothing) Then
					func = Nothing
				Else
					func = New Func(Of Integer, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer)(AddressOf variable.Invoke)
				End If
				SourceMemberContainerTypeSymbol.AddInitializer(instanceInitializers, func, members.InstanceSyntaxLength)
			End If
		End Sub

		Friend Function CreateSharedConstructorsForConstFieldsIfRequired(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As MethodSymbol
			Dim synthesizedConstructorSymbol As MethodSymbol
			Dim staticInitializers As ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer)) = Me.MemberAndInitializerLookup.StaticInitializers
			If (Not staticInitializers.IsDefaultOrEmpty) Then
				Dim symbols As ImmutableArray(Of Symbol) = New ImmutableArray(Of Symbol)()
				If (Me.MemberAndInitializerLookup.Members.TryGetValue(".cctor", symbols) OrElse Not Me.AnyInitializerToBeInjectedIntoConstructor(DirectCast(staticInitializers, IEnumerable(Of ImmutableArray(Of FieldOrPropertyInitializer))), True)) Then
					synthesizedConstructorSymbol = Nothing
					Return synthesizedConstructorSymbol
				End If
				synthesizedConstructorSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedConstructorSymbol(Me.SyntaxReferences.First(), Me, True, True, binder, diagnostics)
				Return synthesizedConstructorSymbol
			End If
			synthesizedConstructorSymbol = Nothing
			Return synthesizedConstructorSymbol
		End Function

		Friend Function DetectTypeCircularity_ShouldStepIntoType(ByVal typeToTest As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean
			If (typeToTest.ContainingModule Is Nothing OrElse Not typeToTest.ContainingModule.Equals(Me.ContainingModule)) Then
				flag = True
			ElseIf (Not typeToTest.Locations.IsEmpty) Then
				Dim item As Microsoft.CodeAnalysis.Location = typeToTest.Locations(0)
				Dim location As Microsoft.CodeAnalysis.Location = Me.Locations(0)
				Dim num As Integer = Me.DeclaringCompilation.CompareSourceLocations(item, location)
				If (num > 0) Then
					flag1 = True
				Else
					flag1 = If(num <> 0, False, item.SourceSpan.Start >= location.SourceSpan.Start)
				End If
				flag = flag1
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Function DetermineDefaultPropertyName(ByVal membersByName As Dictionary(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)), ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As String
			Dim enumerator As Dictionary(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)).Enumerator = New Dictionary(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)).Enumerator()
			Dim str As String = Nothing
			Try
				enumerator = membersByName.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As KeyValuePair(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)) = enumerator.Current
					Dim key As String = current.Key
					Dim value As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = current.Value
					Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = Nothing
					Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = value.GetEnumerator()
					While enumerator1.MoveNext()
						Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator1.Current
						If (symbol.Kind <> SymbolKind.[Property]) Then
							Continue While
						End If
						Dim propertySymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
						If (Not propertySymbol1.IsDefault) Then
							Continue While
						End If
						If (str IsNot Nothing) Then
							diagBag.Add(ERRID.ERR_DuplicateDefaultProps1, propertySymbol1.Locations(0), New [Object]() { Me.GetKindText() })
							Exit While
						Else
							propertySymbol = propertySymbol1
							str = key
							If (propertySymbol.ShadowsExplicitly) Then
								Exit While
							End If
							Dim locations As ImmutableArray(Of Location) = propertySymbol1.Locations
							Me.CheckDefaultPropertyAgainstAllBases(Me, str, locations(0), diagBag)
							Exit While
						End If
					End While
					If (str Is Nothing OrElse EmbeddedOperators.CompareString(str, key, False) <> 0) Then
						Continue While
					End If
					Dim enumerator2 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = value.GetEnumerator()
					While enumerator2.MoveNext()
						Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator2.Current
						If (current1.Kind <> SymbolKind.[Property]) Then
							Continue While
						End If
						Dim sourcePropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol = DirectCast(current1, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol)
						If (sourcePropertySymbol.IsDefault) Then
							Continue While
						End If
						diagBag.Add(ERRID.ERR_DefaultMissingFromProperty2, sourcePropertySymbol.Locations(0), New [Object]() { propertySymbol, sourcePropertySymbol })
					End While
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			Return str
		End Function

		Private Sub EnsureCtor(ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByVal isShared As Boolean, ByVal isDebuggable As Boolean, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim str As String = If(isShared, ".cctor", ".ctor")
			Dim symbols As ArrayBuilder(Of Symbol) = Nothing
			If (members.Members.TryGetValue(str, symbols)) Then
				Dim enumerator As ArrayBuilder(Of Symbol).Enumerator = symbols.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As MethodSymbol = DirectCast(enumerator.Current, MethodSymbol)
					If (current.MethodKind = MethodKind.Constructor AndAlso current.ParameterCount = 0) Then
						Return
					End If
				End While
				If (Me.TypeKind <> Microsoft.CodeAnalysis.TypeKind.Struct OrElse isShared) Then
					Return
				End If
			End If
			Dim syntaxReference As Microsoft.CodeAnalysis.SyntaxReference = Me.SyntaxReferences.First()
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(Me.m_containingModule, syntaxReference.SyntaxTree, Me)
			Dim synthesizedConstructorSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedConstructorSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedConstructorSymbol(syntaxReference, Me, isShared, isDebuggable, binder, diagBag)
			Me.AddMember(synthesizedConstructorSymbol, binder, members, False)
		End Sub

		Private Function FindPartialMethodDeclarations(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal members As Dictionary(Of String, ImmutableArray(Of Symbol))) As HashSet(Of SourceMemberMethodSymbol)
			Dim enumerator As Dictionary(Of String, ImmutableArray(Of Symbol)).Enumerator = New Dictionary(Of String, ImmutableArray(Of Symbol)).Enumerator()
			Dim sourceMemberMethodSymbols As HashSet(Of SourceMemberMethodSymbol) = Nothing
			Try
				enumerator = members.GetEnumerator()
				While enumerator.MoveNext()
					Dim enumerator1 As ImmutableArray(Of Symbol).Enumerator = enumerator.Current.Value.GetEnumerator()
					While enumerator1.MoveNext()
						Dim current As SourceMemberMethodSymbol = TryCast(enumerator1.Current, SourceMemberMethodSymbol)
						If (current Is Nothing OrElse Not current.IsPartial OrElse current.MethodKind <> MethodKind.Ordinary) Then
							Continue While
						End If
						If (current.IsSub) Then
							If (sourceMemberMethodSymbols Is Nothing) Then
								sourceMemberMethodSymbols = New HashSet(Of SourceMemberMethodSymbol)(ReferenceEqualityComparer.Instance)
							End If
							sourceMemberMethodSymbols.Add(current)
						Else
							diagnostics.Add(ERRID.ERR_PartialMethodsMustBeSub1, current.NonMergedLocation, New [Object]() { current.Name })
						End If
					End While
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			Return sourceMemberMethodSymbols
		End Function

		Friend Shared Function FindSymbolFromSyntax(ByVal declarationSyntax As TypeStatementSyntax, ByVal container As NamespaceOrTypeSymbol, ByVal sourceModule As ModuleSymbol) As SourceNamedTypeSymbol
			Dim valueText As String = declarationSyntax.Identifier.ValueText
			Dim arity As Integer = DeclarationTreeBuilder.GetArity(declarationSyntax.TypeParameterList)
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind = DeclarationTreeBuilder.GetKind(declarationSyntax.Kind())
			Return SourceMemberContainerTypeSymbol.FindSymbolInContainer(valueText, arity, kind, container, sourceModule)
		End Function

		Friend Shared Function FindSymbolFromSyntax(ByVal declarationSyntax As EnumStatementSyntax, ByVal container As NamespaceOrTypeSymbol, ByVal sourceModule As ModuleSymbol) As SourceNamedTypeSymbol
			Dim valueText As String = declarationSyntax.Identifier.ValueText
			Dim num As Integer = 0
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind = DeclarationTreeBuilder.GetKind(declarationSyntax.Kind())
			Return SourceMemberContainerTypeSymbol.FindSymbolInContainer(valueText, num, kind, container, sourceModule)
		End Function

		Friend Shared Function FindSymbolFromSyntax(ByVal declarationSyntax As DelegateStatementSyntax, ByVal container As NamespaceOrTypeSymbol, ByVal sourceModule As ModuleSymbol) As SourceNamedTypeSymbol
			Dim valueText As String = declarationSyntax.Identifier.ValueText
			Dim arity As Integer = DeclarationTreeBuilder.GetArity(declarationSyntax.TypeParameterList)
			Return SourceMemberContainerTypeSymbol.FindSymbolInContainer(valueText, arity, Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind.[Delegate], container, sourceModule)
		End Function

		Private Shared Function FindSymbolInContainer(ByVal childName As String, ByVal childArity As Integer, ByVal childDeclKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.DeclarationKind, ByVal container As NamespaceOrTypeSymbol, ByVal sourceModule As ModuleSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol
			Dim sourceNamedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol
			Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = container.GetTypeMembers(childName, childArity).GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol = TryCast(enumerator.Current, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol)
					If (current IsNot Nothing AndAlso CObj(current.ContainingModule) = CObj(sourceModule) AndAlso current.DeclarationKind = childDeclKind) Then
						sourceNamedTypeSymbol = current
						Exit While
					End If
				Else
					sourceNamedTypeSymbol = Nothing
					Exit While
				End If
			End While
			Return sourceNamedTypeSymbol
		End Function

		Protected Overridable Sub GenerateAllDeclarationErrorsImpl(ByVal cancellationToken As System.Threading.CancellationToken)
			cancellationToken.ThrowIfCancellationRequested()
			Me.GetMembersAndInitializers()
			cancellationToken.ThrowIfCancellationRequested()
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = Me.GetMembers().GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Symbol = enumerator.Current
				If (current.Kind = SymbolKind.NamedType) Then
					Continue While
				End If
				current.GenerateDeclarationErrors(cancellationToken)
			End While
			cancellationToken.ThrowIfCancellationRequested()
			Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = MyBase.BaseTypeNoUseSiteDiagnostics
			cancellationToken.ThrowIfCancellationRequested()
			Dim interfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol) = MyBase.InterfacesNoUseSiteDiagnostics
			cancellationToken.ThrowIfCancellationRequested()
			Dim explicitInterfaceImplementationMap As MultiDictionary(Of Symbol, Symbol) = Me.ExplicitInterfaceImplementationMap
			cancellationToken.ThrowIfCancellationRequested()
			Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = Me.TypeParameters
			If (Not typeParameters.IsEmpty) Then
				TypeParameterSymbol.EnsureAllConstraintsAreResolved(typeParameters)
			End If
			cancellationToken.ThrowIfCancellationRequested()
			Me.GetAttributes()
			cancellationToken.ThrowIfCancellationRequested()
			Me.BindAllMemberAttributes(cancellationToken)
			cancellationToken.ThrowIfCancellationRequested()
			Me.GenerateVarianceDiagnostics()
		End Sub

		Friend NotOverridable Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			Me.GenerateAllDeclarationErrorsImpl(cancellationToken)
		End Sub

		Private Sub GenerateVarianceDiagnostics()
			Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag
			If ((Me.m_lazyState And 2) = 0) Then
				Dim diagnosticBag As Microsoft.CodeAnalysis.DiagnosticBag = Nothing
				Dim diagnosticInfos As ArrayBuilder(Of DiagnosticInfo) = Nothing
				Select Case Me.TypeKind
					Case Microsoft.CodeAnalysis.TypeKind.[Class]
					Case Microsoft.CodeAnalysis.TypeKind.[Enum]
					Case Microsoft.CodeAnalysis.TypeKind.Struct
						Me.ReportNestingIntoVariantInterface(diagnosticBag)
						GoTo Label0
					Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
						Me.GenerateVarianceDiagnosticsForDelegate(diagnosticBag, diagnosticInfos)
						GoTo Label0
					Case Microsoft.CodeAnalysis.TypeKind.Dynamic
					Case Microsoft.CodeAnalysis.TypeKind.[Error]
					Case Microsoft.CodeAnalysis.TypeKind.Pointer
					Case Microsoft.CodeAnalysis.TypeKind.TypeParameter
						Throw ExceptionUtilities.UnexpectedValue(Me.TypeKind)
					Case Microsoft.CodeAnalysis.TypeKind.[Interface]
						Me.GenerateVarianceDiagnosticsForInterface(diagnosticBag, diagnosticInfos)
						GoTo Label0
					Case Microsoft.CodeAnalysis.TypeKind.[Module]
					Case Microsoft.CodeAnalysis.TypeKind.Submission
					Label0:
						Dim mContainingModule As SourceModuleSymbol = Me.m_containingModule
						If (diagnosticBag IsNot Nothing) Then
							bindingDiagnosticBag = New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(diagnosticBag)
						Else
							bindingDiagnosticBag = Nothing
						End If
						mContainingModule.AtomicSetFlagAndStoreDiagnostics(Me.m_lazyState, 2, 0, bindingDiagnosticBag)
						If (diagnosticBag IsNot Nothing) Then
							diagnosticBag.Free()
						End If
						If (diagnosticInfos Is Nothing) Then
							Exit Select
						End If
						diagnosticInfos.Free()
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(Me.TypeKind)
				End Select
			End If
		End Sub

		Private Sub GenerateVarianceDiagnosticsForConstraints(ByVal parameters As ImmutableArray(Of TypeParameterSymbol), <InAttribute> <Out> ByRef diagnostics As DiagnosticBag, <InAttribute> <Out> ByRef infosBuffer As ArrayBuilder(Of DiagnosticInfo))
			Dim enumerator As ImmutableArray(Of TypeParameterSymbol).Enumerator = parameters.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As TypeParameterSymbol = enumerator.Current
				Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol).Enumerator = current.ConstraintTypesNoUseSiteDiagnostics.GetEnumerator()
				While enumerator1.MoveNext()
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = enumerator1.Current
					Me.GenerateVarianceDiagnosticsForType(typeSymbol, VarianceKind.[In], SourceMemberContainerTypeSymbol.VarianceContext.Constraint, infosBuffer)
					If (Not SourceMemberContainerTypeSymbol.HaveDiagnostics(infosBuffer)) Then
						Continue While
					End If
					Dim item As Location = current.Locations(0)
					Dim enumerator2 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterConstraint).Enumerator = current.GetConstraints().GetEnumerator()
					While enumerator2.MoveNext()
						Dim typeParameterConstraint As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterConstraint = enumerator2.Current
						If (typeParameterConstraint.TypeConstraint Is Nothing OrElse Not typeParameterConstraint.TypeConstraint.IsSameTypeIgnoringAll(typeSymbol)) Then
							Continue While
						End If
						item = typeParameterConstraint.LocationOpt
						Exit While
					End While
					SourceMemberContainerTypeSymbol.ReportDiagnostics(diagnostics, item, infosBuffer)
				End While
			End While
		End Sub

		Private Sub GenerateVarianceDiagnosticsForDelegate(<InAttribute> <Out> ByRef diagnostics As DiagnosticBag, <InAttribute> <Out> ByRef infosBuffer As ArrayBuilder(Of DiagnosticInfo))
			If (Me.HasVariance()) Then
				Dim delegateInvokeMethod As MethodSymbol = Me.DelegateInvokeMethod
				If (delegateInvokeMethod IsNot Nothing) Then
					Me.GenerateVarianceDiagnosticsForMethod(delegateInvokeMethod, diagnostics, infosBuffer)
				End If
			End If
		End Sub

		Private Sub GenerateVarianceDiagnosticsForEvent(ByVal [event] As EventSymbol, <InAttribute> <Out> ByRef diagnostics As DiagnosticBag, <InAttribute> <Out> ByRef infosBuffer As ArrayBuilder(Of DiagnosticInfo))
			Dim location As Microsoft.CodeAnalysis.Location
			Dim type As TypeSymbol = [event].Type
			If (type.IsDelegateType() AndAlso type(Nothing) <> [event]) Then
				Me.GenerateVarianceDiagnosticsForType(type, VarianceKind.[In], SourceMemberContainerTypeSymbol.VarianceContext.Complex, infosBuffer)
				If (SourceMemberContainerTypeSymbol.HaveDiagnostics(infosBuffer)) Then
					Dim declaringSyntaxNode As EventStatementSyntax = [event].GetDeclaringSyntaxNode(Of EventStatementSyntax)()
					location = If(declaringSyntaxNode Is Nothing OrElse declaringSyntaxNode.AsClause Is Nothing, [event].Locations(0), declaringSyntaxNode.AsClause.Type.GetLocation())
					SourceMemberContainerTypeSymbol.ReportDiagnostics(diagnostics, location, infosBuffer)
				End If
			End If
		End Sub

		Private Sub GenerateVarianceDiagnosticsForInterface(<InAttribute> <Out> ByRef diagnostics As DiagnosticBag, <InAttribute> <Out> ByRef infosBuffer As ArrayBuilder(Of DiagnosticInfo))
			Dim enumerator As Dictionary(Of String, ImmutableArray(Of Symbol)).ValueCollection.Enumerator = New Dictionary(Of String, ImmutableArray(Of Symbol)).ValueCollection.Enumerator()
			If (Me.HasVariance()) Then
				Try
					enumerator = Me.GetMembersAndInitializers().Members.Values.GetEnumerator()
					While enumerator.MoveNext()
						Dim enumerator1 As ImmutableArray(Of Symbol).Enumerator = enumerator.Current.GetEnumerator()
						While enumerator1.MoveNext()
							Dim current As Symbol = enumerator1.Current
							If (current.IsImplicitlyDeclared) Then
								Continue While
							End If
							Dim kind As SymbolKind = current.Kind
							If (kind = SymbolKind.[Event]) Then
								Me.GenerateVarianceDiagnosticsForEvent(DirectCast(current, EventSymbol), diagnostics, infosBuffer)
							ElseIf (kind = SymbolKind.Method) Then
								Me.GenerateVarianceDiagnosticsForMethod(DirectCast(current, MethodSymbol), diagnostics, infosBuffer)
							ElseIf (kind = SymbolKind.[Property]) Then
								Me.GenerateVarianceDiagnosticsForProperty(DirectCast(current, PropertySymbol), diagnostics, infosBuffer)
							End If
						End While
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
				Dim enumerator2 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Enumerator = MyBase.InterfacesNoUseSiteDiagnostics.GetEnumerator()
				While enumerator2.MoveNext()
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator2.Current
					If (namedTypeSymbol.IsErrorType()) Then
						Continue While
					End If
					Me.GenerateVarianceDiagnosticsForType(namedTypeSymbol, VarianceKind.Out, SourceMemberContainerTypeSymbol.VarianceContext.Complex, infosBuffer)
					If (Not SourceMemberContainerTypeSymbol.HaveDiagnostics(infosBuffer)) Then
						Continue While
					End If
					SourceMemberContainerTypeSymbol.ReportDiagnostics(diagnostics, Me.GetInheritsOrImplementsLocation(namedTypeSymbol, True), infosBuffer)
				End While
			End If
		End Sub

		Private Sub GenerateVarianceDiagnosticsForMethod(ByVal method As MethodSymbol, <InAttribute> <Out> ByRef diagnostics As DiagnosticBag, <InAttribute> <Out> ByRef infosBuffer As ArrayBuilder(Of DiagnosticInfo))
			Dim location As Microsoft.CodeAnalysis.Location
			Dim asClauseInternal As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax
			Dim methodKind As Microsoft.CodeAnalysis.MethodKind = method.MethodKind
			If (methodKind <> Microsoft.CodeAnalysis.MethodKind.EventAdd AndAlso methodKind <> Microsoft.CodeAnalysis.MethodKind.EventRemove AndAlso CInt(methodKind) - CInt(Microsoft.CodeAnalysis.MethodKind.PropertyGet) > CInt(Microsoft.CodeAnalysis.MethodKind.Constructor)) Then
				Me.GenerateVarianceDiagnosticsForParameters(method.Parameters, diagnostics, infosBuffer)
				Me.GenerateVarianceDiagnosticsForType(method.ReturnType, VarianceKind.Out, SourceMemberContainerTypeSymbol.VarianceContext.[Return], infosBuffer)
				If (SourceMemberContainerTypeSymbol.HaveDiagnostics(infosBuffer)) Then
					Dim declaringSyntaxNode As MethodBaseSyntax = method.GetDeclaringSyntaxNode(Of MethodBaseSyntax)()
					If (declaringSyntaxNode Is Nothing AndAlso method.MethodKind = Microsoft.CodeAnalysis.MethodKind.DelegateInvoke) Then
						declaringSyntaxNode = method.ContainingType.GetDeclaringSyntaxNode(Of MethodBaseSyntax)()
					End If
					If (declaringSyntaxNode IsNot Nothing) Then
						asClauseInternal = declaringSyntaxNode.AsClauseInternal
					Else
						asClauseInternal = Nothing
					End If
					Dim asClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax = asClauseInternal
					location = If(asClauseSyntax Is Nothing, method.Locations(0), asClauseSyntax.Type().GetLocation())
					SourceMemberContainerTypeSymbol.ReportDiagnostics(diagnostics, location, infosBuffer)
				End If
				Me.GenerateVarianceDiagnosticsForConstraints(method.TypeParameters, diagnostics, infosBuffer)
			End If
		End Sub

		Private Sub GenerateVarianceDiagnosticsForParameters(ByVal parameters As ImmutableArray(Of ParameterSymbol), <InAttribute> <Out> ByRef diagnostics As DiagnosticBag, <InAttribute> <Out> ByRef infosBuffer As ArrayBuilder(Of DiagnosticInfo))
			Dim varianceKind As Microsoft.CodeAnalysis.VarianceKind
			Dim varianceContext As SourceMemberContainerTypeSymbol.VarianceContext
			Dim location As Microsoft.CodeAnalysis.Location
			Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = parameters.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As ParameterSymbol = enumerator.Current
				If (Not current.IsByRef) Then
					varianceKind = Microsoft.CodeAnalysis.VarianceKind.[In]
					varianceContext = SourceMemberContainerTypeSymbol.VarianceContext.[ByVal]
				Else
					varianceKind = Microsoft.CodeAnalysis.VarianceKind.None
					varianceContext = SourceMemberContainerTypeSymbol.VarianceContext.[ByRef]
				End If
				Me.GenerateVarianceDiagnosticsForType(current.Type, varianceKind, varianceContext, infosBuffer)
				If (Not SourceMemberContainerTypeSymbol.HaveDiagnostics(infosBuffer)) Then
					Continue While
				End If
				Dim declaringSyntaxNode As ParameterSyntax = current.GetDeclaringSyntaxNode(Of ParameterSyntax)()
				location = If(declaringSyntaxNode Is Nothing OrElse declaringSyntaxNode.AsClause Is Nothing, current.Locations(0), declaringSyntaxNode.AsClause.Type.GetLocation())
				SourceMemberContainerTypeSymbol.ReportDiagnostics(diagnostics, location, infosBuffer)
			End While
		End Sub

		Private Sub GenerateVarianceDiagnosticsForProperty(ByVal [property] As PropertySymbol, <InAttribute> <Out> ByRef diagnostics As DiagnosticBag, <InAttribute> <Out> ByRef infosBuffer As ArrayBuilder(Of DiagnosticInfo))
			Dim varianceKind As Microsoft.CodeAnalysis.VarianceKind
			Dim varianceContext As SourceMemberContainerTypeSymbol.VarianceContext
			Dim location As Microsoft.CodeAnalysis.Location
			If ([property].IsReadOnly) Then
				varianceKind = Microsoft.CodeAnalysis.VarianceKind.Out
				varianceContext = SourceMemberContainerTypeSymbol.VarianceContext.ReadOnlyProperty
			ElseIf (Not [property].IsWriteOnly) Then
				varianceKind = Microsoft.CodeAnalysis.VarianceKind.None
				varianceContext = SourceMemberContainerTypeSymbol.VarianceContext.[Property]
			Else
				varianceKind = Microsoft.CodeAnalysis.VarianceKind.[In]
				varianceContext = SourceMemberContainerTypeSymbol.VarianceContext.WriteOnlyProperty
			End If
			Me.GenerateVarianceDiagnosticsForType([property].Type, varianceKind, varianceContext, infosBuffer)
			If (SourceMemberContainerTypeSymbol.HaveDiagnostics(infosBuffer)) Then
				Dim declaringSyntaxNode As PropertyStatementSyntax = [property].GetDeclaringSyntaxNode(Of PropertyStatementSyntax)()
				location = If(declaringSyntaxNode Is Nothing OrElse declaringSyntaxNode.AsClause Is Nothing, [property].Locations(0), declaringSyntaxNode.AsClause.Type().GetLocation())
				SourceMemberContainerTypeSymbol.ReportDiagnostics(diagnostics, location, infosBuffer)
			End If
			Me.GenerateVarianceDiagnosticsForParameters([property].Parameters, diagnostics, infosBuffer)
		End Sub

		Private Sub GenerateVarianceDiagnosticsForType(ByVal type As TypeSymbol, ByVal requiredVariance As VarianceKind, ByVal context As SourceMemberContainerTypeSymbol.VarianceContext, <InAttribute> <Out> ByRef diagnostics As ArrayBuilder(Of DiagnosticInfo))
			Dim varianceDiagnosticsTargetTypeParameter As SourceMemberContainerTypeSymbol.VarianceDiagnosticsTargetTypeParameter = New SourceMemberContainerTypeSymbol.VarianceDiagnosticsTargetTypeParameter()
			Me.GenerateVarianceDiagnosticsForTypeRecursively(type, requiredVariance, context, varianceDiagnosticsTargetTypeParameter, 0, diagnostics)
		End Sub

		Private Sub GenerateVarianceDiagnosticsForTypeRecursively(ByVal type As TypeSymbol, ByVal requiredVariance As Microsoft.CodeAnalysis.VarianceKind, ByVal context As SourceMemberContainerTypeSymbol.VarianceContext, ByVal typeParameterInfo As SourceMemberContainerTypeSymbol.VarianceDiagnosticsTargetTypeParameter, ByVal constructionDepth As Integer, <InAttribute> <Out> ByRef diagnostics As ArrayBuilder(Of DiagnosticInfo))
			Dim typeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
			Dim variance As Microsoft.CodeAnalysis.VarianceKind
			Dim typeParameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol)
			Dim kind As SymbolKind = type.Kind
			If (kind <= SymbolKind.ErrorType) Then
				If (kind = SymbolKind.ArrayType) Then
					Me.GenerateVarianceDiagnosticsForTypeRecursively(DirectCast(type, ArrayTypeSymbol).ElementType, requiredVariance, context, typeParameterInfo, constructionDepth, diagnostics)
					Return
				End If
				If (kind = SymbolKind.ErrorType) Then
					Return
				End If
				Throw ExceptionUtilities.UnexpectedValue(type.Kind)
			ElseIf (kind = SymbolKind.NamedType) Then
				Dim tupleUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(type.GetTupleUnderlyingTypeOrSelf(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (tupleUnderlyingTypeOrSelf.IsGenericType) Then
					If (requiredVariance <> Microsoft.CodeAnalysis.VarianceKind.Out) Then
						Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
						Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = type.ContainingType
						While containingType IsNot Nothing
							If (containingType.TypeParameters.HaveVariance()) Then
								originalDefinition = containingType.OriginalDefinition
							End If
							containingType = containingType.ContainingType
						End While
						Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
						containingType = Me
						Do
							If (containingType.TypeParameters.HaveVariance()) Then
								namedTypeSymbol = containingType
							End If
							containingType = containingType.ContainingType
						Loop While containingType IsNot Nothing
						If (originalDefinition IsNot Nothing AndAlso CObj(originalDefinition) = CObj(namedTypeSymbol)) Then
							If (typeParameterInfo.ConstructedType Is Nothing) Then
								SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceTypeDisallowed2, New [Object]() { CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(type.OriginalDefinition), CustomSymbolDisplayFormatter.QualifiedName(originalDefinition) }))
								Return
							End If
							If (constructionDepth <= 1) Then
								If (typeParameterInfo.ConstructedType.Arity <= 1) Then
									SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceTypeDisallowed2, New [Object]() { CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(type.OriginalDefinition), CustomSymbolDisplayFormatter.QualifiedName(originalDefinition) }))
									Return
								End If
								SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceTypeDisallowedForGeneric4, New [Object]() { CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(type.OriginalDefinition), CustomSymbolDisplayFormatter.QualifiedName(originalDefinition), typeParameterInfo.TypeParameter.Name, CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType.OriginalDefinition) }))
								Return
							End If
							If (typeParameterInfo.ConstructedType.Arity <= 1) Then
								SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceTypeDisallowedHere3, New [Object]() { CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(type.OriginalDefinition), CustomSymbolDisplayFormatter.QualifiedName(originalDefinition), CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType) }))
								Return
							End If
							SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceTypeDisallowedHereForGeneric5, New [Object]() { CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(type.OriginalDefinition), CustomSymbolDisplayFormatter.QualifiedName(originalDefinition), CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType), typeParameterInfo.TypeParameter.Name, CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType.OriginalDefinition) }))
							Return
						End If
					End If
					If (Not tupleUnderlyingTypeOrSelf.IsNullableType()) Then
						Do
							Dim arity As Integer = tupleUnderlyingTypeOrSelf.Arity - 1
							Dim num As Integer = 0
							Do
								Dim varianceKind As Microsoft.CodeAnalysis.VarianceKind = requiredVariance
								If (varianceKind = Microsoft.CodeAnalysis.VarianceKind.Out) Then
									typeParameters = tupleUnderlyingTypeOrSelf.TypeParameters
									variance = typeParameters(num).Variance
								ElseIf (varianceKind <> Microsoft.CodeAnalysis.VarianceKind.[In]) Then
									variance = Microsoft.CodeAnalysis.VarianceKind.None
								Else
									typeParameters = tupleUnderlyingTypeOrSelf.TypeParameters
									Dim variance1 As Microsoft.CodeAnalysis.VarianceKind = typeParameters(num).Variance
									If (variance1 = Microsoft.CodeAnalysis.VarianceKind.Out) Then
										variance = Microsoft.CodeAnalysis.VarianceKind.[In]
									Else
										variance = If(variance1 <> Microsoft.CodeAnalysis.VarianceKind.[In], Microsoft.CodeAnalysis.VarianceKind.None, Microsoft.CodeAnalysis.VarianceKind.Out)
									End If
								End If
								typeArgumentsNoUseSiteDiagnostics = tupleUnderlyingTypeOrSelf.TypeArgumentsNoUseSiteDiagnostics
								Me.GenerateVarianceDiagnosticsForTypeRecursively(typeArgumentsNoUseSiteDiagnostics(num), variance, SourceMemberContainerTypeSymbol.VarianceContext.Complex, New SourceMemberContainerTypeSymbol.VarianceDiagnosticsTargetTypeParameter(tupleUnderlyingTypeOrSelf, num), constructionDepth + 1, diagnostics)
								num = num + 1
							Loop While num <= arity
							tupleUnderlyingTypeOrSelf = tupleUnderlyingTypeOrSelf.ContainingType
						Loop While tupleUnderlyingTypeOrSelf IsNot Nothing
						Return
					End If
					If (tupleUnderlyingTypeOrSelf.TypeArgumentsNoUseSiteDiagnostics(0).IsValueType) Then
						typeArgumentsNoUseSiteDiagnostics = tupleUnderlyingTypeOrSelf.TypeArgumentsNoUseSiteDiagnostics
						Me.GenerateVarianceDiagnosticsForTypeRecursively(typeArgumentsNoUseSiteDiagnostics(0), Microsoft.CodeAnalysis.VarianceKind.None, SourceMemberContainerTypeSymbol.VarianceContext.Nullable, New SourceMemberContainerTypeSymbol.VarianceDiagnosticsTargetTypeParameter(tupleUnderlyingTypeOrSelf, 0), constructionDepth, diagnostics)
						Return
					End If
				End If
			Else
				If (kind <> SymbolKind.TypeParameter) Then
					Throw ExceptionUtilities.UnexpectedValue(type.Kind)
				End If
				Dim typeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol)
				If (typeParameterSymbol.Variance = Microsoft.CodeAnalysis.VarianceKind.Out AndAlso requiredVariance <> Microsoft.CodeAnalysis.VarianceKind.Out OrElse typeParameterSymbol.Variance = Microsoft.CodeAnalysis.VarianceKind.[In] AndAlso requiredVariance <> Microsoft.CodeAnalysis.VarianceKind.[In]) Then
					Dim flag As Boolean = typeParameterSymbol.Variance = Microsoft.CodeAnalysis.VarianceKind.Out
					Select Case context
						Case SourceMemberContainerTypeSymbol.VarianceContext.[ByVal]
							SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceOutByValDisallowed1, New [Object]() { type.Name }))
							Return
						Case SourceMemberContainerTypeSymbol.VarianceContext.[ByRef]
							SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(If(flag, ERRID.ERR_VarianceOutByRefDisallowed1, ERRID.ERR_VarianceInByRefDisallowed1), New [Object]() { type.Name }))
							Return
						Case SourceMemberContainerTypeSymbol.VarianceContext.[Return]
							SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceInReturnDisallowed1, New [Object]() { type.Name }))
							Return
						Case SourceMemberContainerTypeSymbol.VarianceContext.Constraint
							SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceOutConstraintDisallowed1, New [Object]() { type.Name }))
							Return
						Case SourceMemberContainerTypeSymbol.VarianceContext.Nullable
							SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(If(flag, ERRID.ERR_VarianceOutNullableDisallowed2, ERRID.ERR_VarianceInNullableDisallowed2), New [Object]() { type.Name, CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType) }))
							Return
						Case SourceMemberContainerTypeSymbol.VarianceContext.ReadOnlyProperty
							SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceInReadOnlyPropertyDisallowed1, New [Object]() { type.Name }))
							Return
						Case SourceMemberContainerTypeSymbol.VarianceContext.WriteOnlyProperty
							SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceOutWriteOnlyPropertyDisallowed1, New [Object]() { type.Name }))
							Return
						Case SourceMemberContainerTypeSymbol.VarianceContext.[Property]
							SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(If(flag, ERRID.ERR_VarianceOutPropertyDisallowed1, ERRID.ERR_VarianceInPropertyDisallowed1), New [Object]() { type.Name }))
							Return
						Case SourceMemberContainerTypeSymbol.VarianceContext.Complex
							If (typeParameterInfo.ConstructedType Is Nothing) Then
								SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(If(flag, ERRID.ERR_VarianceOutParamDisallowed1, ERRID.ERR_VarianceInParamDisallowed1), New [Object]() { type.Name }))
								Return
							End If
							If (constructionDepth <= 1) Then
								If (typeParameterInfo.ConstructedType.Arity <= 1) Then
									SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(If(flag, ERRID.ERR_VarianceOutParamDisallowed1, ERRID.ERR_VarianceInParamDisallowed1), New [Object]() { type.Name }))
									Return
								End If
								SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(If(flag, ERRID.ERR_VarianceOutParamDisallowedForGeneric3, ERRID.ERR_VarianceInParamDisallowedForGeneric3), New [Object]() { type.Name, typeParameterInfo.TypeParameter.Name, CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType.OriginalDefinition) }))
								Return
							End If
							If (typeParameterInfo.ConstructedType.Arity <= 1) Then
								SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(If(flag, ERRID.ERR_VarianceOutParamDisallowedHere2, ERRID.ERR_VarianceInParamDisallowedHere2), New [Object]() { type.Name, CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType) }))
								Return
							End If
							SourceMemberContainerTypeSymbol.AppendVarianceDiagnosticInfo(diagnostics, ErrorFactory.ErrorInfo(If(flag, ERRID.ERR_VarianceOutParamDisallowedHereForGeneric4, ERRID.ERR_VarianceInParamDisallowedHereForGeneric4), New [Object]() { type.Name, CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType), typeParameterInfo.TypeParameter.Name, CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType.OriginalDefinition) }))
							Return
					End Select
					Throw ExceptionUtilities.UnexpectedValue(context)
				End If
			End If
		End Sub

		Private Shared Function GetBestName(ByVal declaration As MergedTypeDeclaration, ByVal compilation As VisualBasicCompilation) As String
			Dim declarations As ImmutableArray(Of SingleTypeDeclaration) = declaration.Declarations
			Dim item As SingleTypeDeclaration = declarations(0)
			Dim length As Integer = declarations.Length - 1
			Dim num As Integer = 1
			Do
				Dim location As Microsoft.CodeAnalysis.Location = item.Location
				If (CObj(compilation.FirstSourceLocation(Of Microsoft.CodeAnalysis.Location)(location, declarations(num).Location)) <> CObj(location)) Then
					item = declarations(num)
				End If
				num = num + 1
			Loop While num <= length
			Return item.Name
		End Function

		Friend Overrides Function GetEmittedNamespaceName() As String
			Dim declarationSpelling As String
			Dim sourceNamespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamespaceSymbol = TryCast(Me._containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamespaceSymbol)
			If (sourceNamespaceSymbol Is Nothing OrElse Not sourceNamespaceSymbol.HasMultipleSpellings) Then
				declarationSpelling = Nothing
			Else
				Dim location As Microsoft.CodeAnalysis.Location = Me.DeclaringCompilation.FirstSourceLocation(Of Microsoft.CodeAnalysis.Location)(Me.Locations)
				declarationSpelling = sourceNamespaceSymbol.GetDeclarationSpelling(location.SourceTree, location.SourceSpan.Start)
			End If
			Return declarationSpelling
		End Function

		Private Function GetExplicitSymbolFlags(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByRef shadowsExplicitly As Boolean, ByRef overloadsExplicitly As Boolean, ByRef overridesExplicitly As Boolean) As Boolean
			Dim flag As Boolean
			Dim kind As SymbolKind = symbol.Kind
			If (kind = SymbolKind.Method) Then
				Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol)
				If (sourceMethodSymbol IsNot Nothing) Then
					shadowsExplicitly = sourceMethodSymbol.ShadowsExplicitly
					overloadsExplicitly = sourceMethodSymbol.OverloadsExplicitly
					overridesExplicitly = sourceMethodSymbol.OverridesExplicitly
					flag = If(sourceMethodSymbol.MethodKind = MethodKind.Ordinary, True, sourceMethodSymbol.MethodKind = MethodKind.DeclareMethod)
				Else
					flag = False
				End If
			Else
				If (kind <> SymbolKind.[Property]) Then
					Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
				End If
				Dim sourcePropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol)
				If (sourcePropertySymbol IsNot Nothing) Then
					shadowsExplicitly = sourcePropertySymbol.ShadowsExplicitly
					overloadsExplicitly = sourcePropertySymbol.OverloadsExplicitly
					overridesExplicitly = sourcePropertySymbol.OverridesExplicitly
					flag = True
				Else
					flag = False
				End If
			End If
			Return flag
		End Function

		Friend Overrides Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)
			Return New SourceMemberContainerTypeSymbol.VB$StateMachine_156_GetFieldsToEmit(-2) With
			{
				.$VB$Me = Me
			}
		End Function

		Private Function GetImplementsLocation(ByVal implementedInterface As NamedTypeSymbol, ByRef directInterface As NamedTypeSymbol) As Location
			directInterface = Nothing
			Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = MyBase.InterfacesNoUseSiteDiagnostics.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As NamedTypeSymbol = enumerator.Current
				If (Not TypeSymbol.Equals(current, implementedInterface, TypeCompareKind.ConsiderEverything)) Then
					If (directInterface IsNot Nothing) Then
						Continue While
					End If
					Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
					If (Not current.ImplementsInterface(implementedInterface, Nothing, discarded)) Then
						Continue While
					End If
					directInterface = current
				Else
					directInterface = current
					Exit While
				End If
			End While
			Return Me.GetInheritsOrImplementsLocation(directInterface, Me.IsInterfaceType())
		End Function

		Private Function GetImplementsLocation(ByVal implementedInterface As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) As Location
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			Return Me.GetImplementsLocation(implementedInterface, namedTypeSymbol)
		End Function

		Protected MustOverride Function GetInheritsOrImplementsLocation(ByVal base As NamedTypeSymbol, ByVal getInherits As Boolean) As Location

		Private Shared Function GetInitializersInSourceTree(ByVal tree As SyntaxTree, ByVal initializers As ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer))) As ImmutableArray(Of FieldOrPropertyInitializer)
			Dim instance As ArrayBuilder(Of FieldOrPropertyInitializer) = ArrayBuilder(Of FieldOrPropertyInitializer).GetInstance()
			Dim enumerator As ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer)).Enumerator = initializers.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As ImmutableArray(Of FieldOrPropertyInitializer) = enumerator.Current
				If (current.First().Syntax.SyntaxTree <> tree) Then
					Continue While
				End If
				instance.AddRange(current)
			End While
			Return instance.ToImmutableAndFree()
		End Function

		Friend NotOverridable Overrides Function GetLexicalSortKey() As Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey
			If (Not Me._lazyLexicalSortKey.IsInitialized) Then
				Dim lexicalSortKey As Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey = Me._declaration.GetLexicalSortKey(Me.DeclaringCompilation)
				Me._lazyLexicalSortKey.SetFrom(lexicalSortKey)
			End If
			Return Me._lazyLexicalSortKey
		End Function

		Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
			Dim symbols As ImmutableArray(Of Symbol)
			If ((Me.m_lazyState And 1) = 0) Then
				Dim membersUnordered As ImmutableArray(Of Symbol) = Me.GetMembersUnordered()
				If (membersUnordered.Length >= 2) Then
					membersUnordered = membersUnordered.Sort(LexicalOrderSymbolComparer.Instance)
					ImmutableInterlocked.InterlockedExchange(Of Symbol)(Me._lazyMembersFlattened, membersUnordered)
				End If
				ThreadSafeFlagOperations.[Set](Me.m_lazyState, 1)
				symbols = membersUnordered
			Else
				symbols = Me._lazyMembersFlattened
			End If
			Return symbols
		End Function

		Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Dim empty As ImmutableArray(Of Symbol)
			Dim memberAndInitializerLookup As SourceMemberContainerTypeSymbol.MembersAndInitializers = Me.MemberAndInitializerLookup
			Dim symbols As ImmutableArray(Of Symbol) = New ImmutableArray(Of Symbol)()
			If (Not memberAndInitializerLookup.Members.TryGetValue(name, symbols)) Then
				empty = ImmutableArray(Of Symbol).Empty
			Else
				empty = symbols
			End If
			Return empty
		End Function

		Private Function GetMembersAndInitializers() As SourceMemberContainerTypeSymbol.MembersAndInitializers
			If (Me._lazyMembersAndInitializers Is Nothing) Then
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
				Dim membersAndInitializer As SourceMemberContainerTypeSymbol.MembersAndInitializers = Me.BuildMembersAndInitializers(instance)
				Me.m_containingModule.AtomicStoreReferenceAndDiagnostics(Of SourceMemberContainerTypeSymbol.MembersAndInitializers)(Me._lazyMembersAndInitializers, membersAndInitializer, instance, Nothing)
				instance.Free()
				Dim knownCircularStruct As Boolean = Me.KnownCircularStruct
			End If
			Return Me._lazyMembersAndInitializers
		End Function

		Friend Overrides Function GetMembersUnordered() As ImmutableArray(Of Symbol)
			If (Me._lazyMembersFlattened.IsDefault) Then
				Dim symbols As ImmutableArray(Of Symbol) = Me.MemberAndInitializerLookup.Members.Flatten(Nothing)
				ImmutableInterlocked.InterlockedInitialize(Of Symbol)(Me._lazyMembersFlattened, symbols)
			End If
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.ConditionallyDeOrder(Of Symbol)(Me._lazyMembersFlattened)
		End Function

		Friend Overrides Function GetSimpleNonTypeMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Dim members As ImmutableArray(Of Symbol)
			If (Me._lazyMembersAndInitializers IsNot Nothing OrElse Me.MemberNames.Contains(name, CaseInsensitiveComparison.Comparer)) Then
				members = Me.GetMembers(name)
			Else
				members = ImmutableArray(Of Symbol).Empty
			End If
			Return members
		End Function

		Public Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
			Return Me.GetTypeMembersDictionary().Flatten(LexicalOrderSymbolComparer.Instance)
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Dim empty As ImmutableArray(Of NamedTypeSymbol)
			Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol) = New ImmutableArray(Of NamedTypeSymbol)()
			If (Not Me.GetTypeMembersDictionary().TryGetValue(name, namedTypeSymbols)) Then
				empty = ImmutableArray(Of NamedTypeSymbol).Empty
			Else
				empty = namedTypeSymbols
			End If
			Return empty
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
			Dim func As Func(Of NamedTypeSymbol, Integer, Boolean)
			Dim typeMembers As ImmutableArray(Of NamedTypeSymbol) = Me.GetTypeMembers(name)
			If (SourceMemberContainerTypeSymbol._Closure$__.$I106-0 Is Nothing) Then
				func = Function(t As NamedTypeSymbol, arity_ As Integer) t.Arity = arity_
				SourceMemberContainerTypeSymbol._Closure$__.$I106-0 = func
			Else
				func = SourceMemberContainerTypeSymbol._Closure$__.$I106-0
			End If
			Return typeMembers.WhereAsArray(Of Integer)(func, arity)
		End Function

		Protected Function GetTypeMembersDictionary() As Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol))
			If (Me._lazyTypeMembers Is Nothing) Then
				Interlocked.CompareExchange(Of Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol)))(Me._lazyTypeMembers, Me.MakeTypeMembers(), Nothing)
			End If
			Return Me._lazyTypeMembers
		End Function

		Friend Overrides Function GetTypeMembersUnordered() As ImmutableArray(Of NamedTypeSymbol)
			Return Me.GetTypeMembersDictionary().Flatten(Nothing)
		End Function

		Private Shared Function HaveDiagnostics(ByVal diagnostics As ArrayBuilder(Of DiagnosticInfo)) As Boolean
			If (diagnostics Is Nothing) Then
				Return False
			End If
			Return diagnostics.Count > 0
		End Function

		Private Shared Function IndexOfInitializerContainingPosition(ByVal initializers As ImmutableArray(Of FieldOrPropertyInitializer), ByVal position As Integer) As Integer
			Dim num As Integer
			Dim func As Func(Of FieldOrPropertyInitializer, Integer, Integer)
			Dim fieldOrPropertyInitializers As ImmutableArray(Of FieldOrPropertyInitializer) = initializers
			Dim num1 As Integer = position
			If (SourceMemberContainerTypeSymbol._Closure$__.$I164-0 Is Nothing) Then
				func = Function(initializer As FieldOrPropertyInitializer, pos As Integer) initializer.Syntax.Span.Start.CompareTo(pos)
				SourceMemberContainerTypeSymbol._Closure$__.$I164-0 = func
			Else
				func = SourceMemberContainerTypeSymbol._Closure$__.$I164-0
			End If
			Dim num2 As Integer = fieldOrPropertyInitializers.BinarySearch(Of Integer)(num1, func)
			If (num2 < 0) Then
				num2 = Not num2 - 1
				num = If(num2 < 0 OrElse Not initializers(num2).Syntax.Span.Contains(position), -1, num2)
			Else
				num = num2
			End If
			Return num
		End Function

		Private Function IsConflictingOperatorOverloading(ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal significantDiff As SymbolComparisonResults, ByVal memberList As ImmutableArray(Of Symbol), ByVal memberIndex As Integer, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Boolean
			Dim flag As Boolean
			Dim length As Integer = memberList.Length - 1
			Dim num As Integer = memberIndex
			While True
				If (num <= length) Then
					Dim item As Symbol = memberList(num)
					If (item.Kind = SymbolKind.Method) Then
						Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						If (methodSymbol.MethodKind = method.MethodKind) Then
							Dim symbolComparisonResult As SymbolComparisonResults = MethodSignatureComparer.DetailedCompare(method, methodSymbol, SymbolComparisonResults.ReturnTypeMismatch Or SymbolComparisonResults.ArityMismatch Or SymbolComparisonResults.RequiredExtraParameterMismatch Or SymbolComparisonResults.OptionalParameterMismatch Or SymbolComparisonResults.RequiredParameterTypeMismatch Or SymbolComparisonResults.OptionalParameterTypeMismatch Or SymbolComparisonResults.OptionalParameterValueMismatch Or SymbolComparisonResults.ParameterByrefMismatch Or SymbolComparisonResults.ParamArrayMismatch Or SymbolComparisonResults.PropertyAccessorMismatch Or SymbolComparisonResults.PropertyInitOnlyMismatch Or SymbolComparisonResults.VarargMismatch Or SymbolComparisonResults.TotalParameterCountMismatch Or SymbolComparisonResults.TupleNamesMismatch, 0)
							If (CInt((symbolComparisonResult And significantDiff)) = 0) Then
								Dim locations As ImmutableArray(Of Location) = method.Locations
								Me.ReportOverloadsErrors(symbolComparisonResult, method, methodSymbol, locations(0), diagnostics)
								flag = True
								Exit While
							End If
						End If
					End If
					num = num + 1
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Function MakeExplicitInterfaceImplementationMap(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim emptyExplicitImplementationMap As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim enumerator As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet).ValueCollection.Enumerator = New Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet).ValueCollection.Enumerator()
			Dim enumerator1 As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet.Enumerator = New MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet.Enumerator()
			Dim locations As ImmutableArray(Of Location)
			If (Me.IsClassType() OrElse Me.IsStructureType() OrElse Me.IsInterfaceType()) Then
				Me.CheckInterfaceUnificationAndVariance(diagnostics)
			End If
			If (Me.IsClassType() OrElse Me.IsStructureType()) Then
				Dim symbols As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Microsoft.CodeAnalysis.VisualBasic.Symbol) = New MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Microsoft.CodeAnalysis.VisualBasic.Symbol)(TypeSymbol.ExplicitInterfaceImplementationTargetMemberEqualityComparer.Instance)
				Dim enumerator2 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = Me.GetMembers().GetEnumerator()
				While enumerator2.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator2.Current
					Dim enumerator3 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = ImplementsHelper.GetExplicitInterfaceImplementations(current).GetEnumerator()
					While enumerator3.MoveNext()
						Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator3.Current
						If (Me.ShouldReportImplementationError(symbol) AndAlso symbols.ContainsKey(symbol)) Then
							Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_MethodAlreadyImplemented2, New [Object]() { CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(symbol.ContainingType), CustomSymbolDisplayFormatter.ShortErrorName(symbol) })
							diagnostics.Add(New VBDiagnostic(diagnosticInfo, ImplementsHelper.GetImplementingLocation(current, symbol), False))
						End If
						symbols.Add(symbol, current)
					End While
				End While
				Try
					enumerator = MyBase.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.Values.GetEnumerator()
					While enumerator.MoveNext()
						Dim namedTypeSymbols As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).ValueSet = enumerator.Current
						Try
							enumerator1 = namedTypeSymbols.GetEnumerator()
							While enumerator1.MoveNext()
								Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator1.Current
								Dim baseTypeNoUseSiteDiagnostics As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = MyBase.BaseTypeNoUseSiteDiagnostics
								Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
								If (baseTypeNoUseSiteDiagnostics.ImplementsInterface(namedTypeSymbol, Nothing, discarded)) Then
									Continue While
								End If
								Dim enumerator4 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = namedTypeSymbol.GetMembers().GetEnumerator()
								While enumerator4.MoveNext()
									Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator4.Current
									If (Not current1.RequiresImplementation()) Then
										Continue While
									End If
									Dim item As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Microsoft.CodeAnalysis.VisualBasic.Symbol).ValueSet = symbols(current1)
									Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = current1.GetUseSiteInfo()
									If (Not Me.ShouldReportImplementationError(current1)) Then
										If (item.Count <> 1) Then
											Continue While
										End If
										locations = item.[Single]().Locations
										diagnostics.Add(useSiteInfo, locations(0))
									ElseIf (item.Count <> 0) Then
										If (item.Count <> 1) Then
											Continue While
										End If
										locations = item.[Single]().Locations
										diagnostics.Add(useSiteInfo, locations(0))
									Else
										Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = If(useSiteInfo.DiagnosticInfo, ErrorFactory.ErrorInfo(ERRID.ERR_UnimplementedMember3, New [Object]() { If(Me.IsStructureType(), "Structure", "Class"), CustomSymbolDisplayFormatter.ShortErrorName(Me), current1, CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(namedTypeSymbol) }))
										diagnostics.Add(New VBDiagnostic(diagnosticInfo1, Me.GetImplementsLocation(namedTypeSymbol), False))
									End If
								End While
							End While
						Finally
							DirectCast(enumerator1, IDisposable).Dispose()
						End Try
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
				emptyExplicitImplementationMap = If(symbols.Count <= 0, TypeSymbol.EmptyExplicitImplementationMap, symbols)
			Else
				emptyExplicitImplementationMap = TypeSymbol.EmptyExplicitImplementationMap
			End If
			Return emptyExplicitImplementationMap
		End Function

		Private Function MakeTypeMembers() As Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol))
			Dim dictionary As Dictionary(Of String, ImmutableArray(Of NamedTypeSymbol))
			Dim name As Func(Of NamedTypeSymbol, String)
			Dim children As ImmutableArray(Of MergedTypeDeclaration) = Me._declaration.Children
			If (Not children.IsEmpty) Then
				Dim namedTypeSymbols As IEnumerable(Of NamedTypeSymbol) = children.[Select](Of NamedTypeSymbol)(Function(decl As MergedTypeDeclaration) Me.CreateNestedType(decl))
				If (SourceMemberContainerTypeSymbol._Closure$__.$I102-1 Is Nothing) Then
					name = Function(decl As NamedTypeSymbol) decl.Name
					SourceMemberContainerTypeSymbol._Closure$__.$I102-1 = name
				Else
					name = SourceMemberContainerTypeSymbol._Closure$__.$I102-1
				End If
				dictionary = namedTypeSymbols.ToDictionary(Of String)(name, CaseInsensitiveComparison.Comparer)
			Else
				dictionary = SourceMemberContainerTypeSymbol.s_emptyTypeMembers
			End If
			Return dictionary
		End Function

		Private Sub ProcessPartialMethodsIfAny(ByVal members As Dictionary(Of String, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim sourceMemberMethodSymbols As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol) = Me.FindPartialMethodDeclarations(diagnostics, members)
			If (sourceMemberMethodSymbols Is Nothing) Then
				Return
			End If
			While sourceMemberMethodSymbols.Count > 0
				Dim immutableAndFree As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol = sourceMemberMethodSymbols.First()
				sourceMemberMethodSymbols.Remove(immutableAndFree)
				Dim sourceMemberMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol = immutableAndFree
				Dim nonMergedLocation As Microsoft.CodeAnalysis.Location = sourceMemberMethodSymbol.NonMergedLocation
				Dim sourceMemberMethodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol = Nothing
				Dim location As Microsoft.CodeAnalysis.Location = Nothing
				Dim item As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = members(immutableAndFree.Name)
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = item.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol = TryCast(enumerator.Current, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol)
					If (current Is Nothing OrElse CObj(current) = CObj(immutableAndFree) OrElse current.MethodKind <> MethodKind.Ordinary OrElse Not Me.ComparePartialMethodSignatures(immutableAndFree, current)) Then
						Continue While
					End If
					Dim nonMergedLocation1 As Microsoft.CodeAnalysis.Location = current.NonMergedLocation
					If (Not sourceMemberMethodSymbols.Contains(current)) Then
						If (current.IsPartial) Then
							Continue While
						End If
						If (sourceMemberMethodSymbol1 IsNot Nothing) Then
							Dim flag As Boolean = Me.DeclaringCompilation.CompareSourceLocations(location, nonMergedLocation1) < 0
							Dim sourceMemberMethodSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol = If(flag, current, sourceMemberMethodSymbol1)
							Dim name As String = sourceMemberMethodSymbol2.Name
							diagnostics.Add(ERRID.ERR_OnlyOneImplementingMethodAllowed3, If(flag, nonMergedLocation1, location), New [Object]() { name, name, name })
							sourceMemberMethodSymbol2.SuppressDuplicateProcDefDiagnostics = True
							If (flag) Then
								Continue While
							End If
							sourceMemberMethodSymbol1 = current
							location = nonMergedLocation1
						Else
							sourceMemberMethodSymbol1 = current
							location = nonMergedLocation1
						End If
					Else
						sourceMemberMethodSymbols.Remove(current)
						Dim flag1 As Boolean = Me.DeclaringCompilation.CompareSourceLocations(nonMergedLocation, nonMergedLocation1) < 0
						Dim sourceMemberMethodSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol = If(flag1, current, sourceMemberMethodSymbol)
						Dim str As String = sourceMemberMethodSymbol3.Name
						diagnostics.Add(ERRID.ERR_OnlyOnePartialMethodAllowed2, If(flag1, nonMergedLocation1, nonMergedLocation), New [Object]() { str, str })
						sourceMemberMethodSymbol3.SuppressDuplicateProcDefDiagnostics = True
						If (flag1) Then
							Continue While
						End If
						sourceMemberMethodSymbol = current
						nonMergedLocation = nonMergedLocation1
					End If
				End While
				If (sourceMemberMethodSymbol.BlockSyntax IsNot Nothing AndAlso sourceMemberMethodSymbol.BlockSyntax.Statements.Count > 0) Then
					diagnostics.Add(ERRID.ERR_PartialMethodMustBeEmpty, nonMergedLocation)
				End If
				If (sourceMemberMethodSymbol1 Is Nothing) Then
					Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol.InitializePartialMethodParts(sourceMemberMethodSymbol, Nothing)
				Else
					Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).GetInstance()
					Dim length As Integer = item.Length - 1
					Dim num As Integer = 0
					Do
						Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = item(num)
						If (sourceMemberMethodSymbol1 <> symbol) Then
							instance.Add(symbol)
						End If
						num = num + 1
					Loop While num <= length
					members(immutableAndFree.Name) = instance.ToImmutableAndFree()
					Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol.InitializePartialMethodParts(sourceMemberMethodSymbol, sourceMemberMethodSymbol1)
					Me.ReportErrorsOnPartialMethodImplementation(sourceMemberMethodSymbol, sourceMemberMethodSymbol1, location, diagnostics)
				End If
			End While
		End Sub

		Private Shared Sub ReportDiagnostics(<InAttribute> <Out> ByRef diagnostics As DiagnosticBag, ByVal location As Microsoft.CodeAnalysis.Location, ByVal infos As ArrayBuilder(Of DiagnosticInfo))
			If (diagnostics Is Nothing) Then
				diagnostics = DiagnosticBag.GetInstance()
			End If
			Dim enumerator As ArrayBuilder(Of DiagnosticInfo).Enumerator = infos.GetEnumerator()
			While enumerator.MoveNext()
				diagnostics.Add(enumerator.Current, location)
			End While
			infos.Clear()
		End Sub

		Private Sub ReportDuplicateInterfaceWithDifferentTupleNames(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal interface1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal interface2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			If (Me.GetImplementsLocation(interface1).SourceSpan.Start > Me.GetImplementsLocation(interface2).SourceSpan.Start) Then
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = interface1
				interface1 = interface2
				interface2 = namedTypeSymbol
			End If
			Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			Dim namedTypeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			Me.GetImplementsLocation(interface1, namedTypeSymbol1)
			Dim implementsLocation As Location = Me.GetImplementsLocation(interface2, namedTypeSymbol2)
			If (TypeSymbol.Equals(namedTypeSymbol1, interface1, TypeCompareKind.ConsiderEverything) AndAlso TypeSymbol.Equals(namedTypeSymbol2, interface2, TypeCompareKind.ConsiderEverything)) Then
				diagnosticInfo = ErrorFactory.ErrorInfo(If(Me.IsInterface, ERRID.ERR_InterfaceInheritedTwiceWithDifferentTupleNames2, ERRID.ERR_InterfaceImplementedTwiceWithDifferentTupleNames2), New [Object]() { CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1) })
			ElseIf (TypeSymbol.Equals(namedTypeSymbol1, interface1, TypeCompareKind.ConsiderEverything) OrElse Not TypeSymbol.Equals(namedTypeSymbol2, interface2, TypeCompareKind.ConsiderEverything)) Then
				diagnosticInfo = If(Not TypeSymbol.Equals(namedTypeSymbol1, interface1, TypeCompareKind.ConsiderEverything) OrElse TypeSymbol.Equals(namedTypeSymbol2, interface2, TypeCompareKind.ConsiderEverything), ErrorFactory.ErrorInfo(If(Me.IsInterface, ERRID.ERR_InterfaceInheritedTwiceWithDifferentTupleNames4, ERRID.ERR_InterfaceImplementedTwiceWithDifferentTupleNames4), New [Object]() { CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(namedTypeSymbol2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(namedTypeSymbol1) }), ErrorFactory.ErrorInfo(If(Me.IsInterface, ERRID.ERR_InterfaceInheritedTwiceWithDifferentTupleNamesReverse3, ERRID.ERR_InterfaceImplementedTwiceWithDifferentTupleNamesReverse3), New [Object]() { CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(namedTypeSymbol2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1) }))
			Else
				diagnosticInfo = ErrorFactory.ErrorInfo(If(Me.IsInterface, ERRID.ERR_InterfaceInheritedTwiceWithDifferentTupleNames3, ERRID.ERR_InterfaceImplementedTwiceWithDifferentTupleNames3), New [Object]() { CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(namedTypeSymbol1) })
			End If
			diagnostics.Add(New VBDiagnostic(diagnosticInfo, implementsLocation, False))
		End Sub

		Private Sub ReportErrorsOnPartialMethodImplementation(ByVal partialMethod As SourceMethodSymbol, ByVal implMethod As SourceMethodSymbol, ByVal implMethodLocation As Location, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (implMethod.DeclaredAccessibility <> Accessibility.[Private]) Then
				diagnostics.Add(ERRID.ERR_ImplementationMustBePrivate2, implMethodLocation, New [Object]() { implMethod.Name, partialMethod.Name })
			End If
			If (partialMethod.ParameterCount > 0) Then
				Dim parameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = partialMethod.Parameters
				Dim parameterSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = implMethod.Parameters
				Dim length As Integer = parameters.Length - 1
				For i As Integer = 0 To length
					Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = parameters(i)
					Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = parameterSymbols(i)
					If (Not CaseInsensitiveComparison.Equals(item.Name, parameterSymbol.Name)) Then
						diagnostics.Add(ERRID.ERR_PartialMethodParamNamesMustMatch3, parameterSymbol.Locations(0), New [Object]() { parameterSymbol.Name, item.Name, implMethod.Name })
					End If
				Next

			End If
			If (implMethod.Arity > 0) Then
				Dim typeParameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol) = partialMethod.TypeParameters
				Dim typeParameterSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol) = implMethod.TypeParameters
				Dim num As Integer = typeParameters.Length - 1
				Dim num1 As Integer = 0
				Do
					Dim typeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = typeParameters(num1)
					Dim item1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = typeParameterSymbols(num1)
					If (Not CaseInsensitiveComparison.Equals(typeParameterSymbol.Name, item1.Name)) Then
						diagnostics.Add(ERRID.ERR_PartialMethodTypeParamNameMismatch3, item1.Locations(0), New [Object]() { item1.Name, typeParameterSymbol.Name, implMethod.Name })
					End If
					num1 = num1 + 1
				Loop While num1 <= num
				If (CInt(MethodSignatureComparer.DetailedCompare(partialMethod, implMethod, SymbolComparisonResults.ArityMismatch Or SymbolComparisonResults.ConstraintMismatch, 0)) <> 0) Then
					diagnostics.Add(ERRID.ERR_PartialMethodGenericConstraints2, implMethodLocation, New [Object]() { implMethod.Name, partialMethod.Name })
				End If
			End If
		End Sub

		Private Sub ReportInterfaceUnificationError(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal interface1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal interface2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			If (Me.GetImplementsLocation(interface1).SourceSpan.Start > Me.GetImplementsLocation(interface2).SourceSpan.Start) Then
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = interface1
				interface1 = interface2
				interface2 = namedTypeSymbol
			End If
			Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			Dim namedTypeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			Me.GetImplementsLocation(interface1, namedTypeSymbol1)
			Dim implementsLocation As Location = Me.GetImplementsLocation(interface2, namedTypeSymbol2)
			Dim flag As Boolean = Me.IsInterfaceType()
			If (TypeSymbol.Equals(namedTypeSymbol1, interface1, TypeCompareKind.ConsiderEverything) AndAlso TypeSymbol.Equals(namedTypeSymbol2, interface2, TypeCompareKind.ConsiderEverything)) Then
				diagnosticInfo = ErrorFactory.ErrorInfo(If(flag, ERRID.ERR_InterfaceUnifiesWithInterface2, ERRID.ERR_InterfacePossiblyImplTwice2), New [Object]() { CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1) })
			ElseIf (TypeSymbol.Equals(namedTypeSymbol1, interface1, TypeCompareKind.ConsiderEverything) OrElse Not TypeSymbol.Equals(namedTypeSymbol2, interface2, TypeCompareKind.ConsiderEverything)) Then
				diagnosticInfo = If(Not TypeSymbol.Equals(namedTypeSymbol1, interface1, TypeCompareKind.ConsiderEverything) OrElse TypeSymbol.Equals(namedTypeSymbol2, interface2, TypeCompareKind.ConsiderEverything), ErrorFactory.ErrorInfo(If(flag, ERRID.ERR_InterfaceBaseUnifiesWithBase4, ERRID.ERR_ClassInheritsInterfaceBaseUnifiesWithBase4), New [Object]() { CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(namedTypeSymbol2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(namedTypeSymbol1) }), ErrorFactory.ErrorInfo(If(flag, ERRID.ERR_BaseUnifiesWithInterfaces3, ERRID.ERR_ClassInheritsBaseUnifiesWithInterfaces3), New [Object]() { CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(namedTypeSymbol2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1) }))
			Else
				diagnosticInfo = ErrorFactory.ErrorInfo(If(flag, ERRID.ERR_InterfaceUnifiesWithBase3, ERRID.ERR_ClassInheritsInterfaceUnifiesWithBase3), New [Object]() { CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(namedTypeSymbol1) })
			End If
			diagnostics.Add(New VBDiagnostic(diagnosticInfo, implementsLocation, False))
		End Sub

		Private Sub ReportNestingIntoVariantInterface(<InAttribute> <Out> ByRef diagnostics As Microsoft.CodeAnalysis.DiagnosticBag)
			If (Me._containingSymbol.IsType) Then
				Dim containingType As NamedTypeSymbol = DirectCast(Me._containingSymbol, NamedTypeSymbol)
				Do
					If (containingType.IsInterfaceType()) Then
						If (containingType.TypeParameters.HaveVariance()) Then
							Exit Do
						End If
						containingType = containingType.ContainingType
					Else
						containingType = Nothing
						Exit Do
					End If
				Loop While containingType IsNot Nothing
				If (containingType IsNot Nothing) Then
					If (diagnostics Is Nothing) Then
						diagnostics = Microsoft.CodeAnalysis.DiagnosticBag.GetInstance()
					End If
					Dim diagnosticBag As Microsoft.CodeAnalysis.DiagnosticBag = diagnostics
					Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_VarianceInterfaceNesting)
					Dim locations As ImmutableArray(Of Location) = Me.Locations
					diagnosticBag.Add(New VBDiagnostic(diagnosticInfo, locations(0), False))
				End If
			End If
		End Sub

		Private Sub ReportOverloadsErrors(ByVal comparisonResults As SymbolComparisonResults, ByVal firstMember As Symbol, ByVal secondMember As Symbol, ByVal location As Microsoft.CodeAnalysis.Location, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol::ReportOverloadsErrors(Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults,Microsoft.CodeAnalysis.VisualBasic.Symbol,Microsoft.CodeAnalysis.VisualBasic.Symbol,Microsoft.CodeAnalysis.Location,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void ReportOverloadsErrors(Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults,Microsoft.CodeAnalysis.VisualBasic.Symbol,Microsoft.CodeAnalysis.VisualBasic.Symbol,Microsoft.CodeAnalysis.Location,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Sub

		Private Sub ReportVarianceAmbiguityWarning(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal interface1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal interface2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(diagnostics, Me.ContainingAssembly)
			Dim flag As Boolean = VarianceAmbiguity.HasVarianceAmbiguity(Me, interface1, interface2, compoundUseSiteInfo)
			If (flag OrElse Not compoundUseSiteInfo.Diagnostics.IsNullOrEmpty()) Then
				If (Me.GetImplementsLocation(interface1).SourceSpan.Start > Me.GetImplementsLocation(interface2).SourceSpan.Start) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = interface1
					interface1 = interface2
					interface2 = namedTypeSymbol
				End If
				Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
				Dim namedTypeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
				Me.GetImplementsLocation(interface1, namedTypeSymbol1)
				Dim implementsLocation As Location = Me.GetImplementsLocation(interface2, namedTypeSymbol2)
				If (Not diagnostics.Add(implementsLocation, compoundUseSiteInfo) AndAlso flag) Then
					Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.WRN_VarianceDeclarationAmbiguous3, New [Object]() { CustomSymbolDisplayFormatter.QualifiedName(namedTypeSymbol2), CustomSymbolDisplayFormatter.QualifiedName(namedTypeSymbol1), CustomSymbolDisplayFormatter.ErrorNameWithKind(interface1.OriginalDefinition) })
					diagnostics.Add(New VBDiagnostic(diagnosticInfo, implementsLocation, False))
					Return
				End If
			Else
				diagnostics.AddDependencies(compoundUseSiteInfo)
			End If
		End Sub

		Private Function ShouldReportImplementationError(ByVal interfaceMember As Symbol) As Boolean
			Dim flag As Boolean
			flag = If(interfaceMember.Kind <> SymbolKind.Method OrElse DirectCast(interfaceMember, MethodSymbol).MethodKind = MethodKind.Ordinary, True, False)
			Return flag
		End Function

		Protected Sub SuppressExtensionAttributeSynthesis()
			Me._lazyEmitExtensionAttribute = ThreeState.[False]
		End Sub

		Friend Function TryCalculateSyntaxOffsetOfPositionInInitializer(ByVal position As Integer, ByVal tree As SyntaxTree, ByVal isShared As Boolean, ByRef syntaxOffset As Integer) As Boolean
			Dim flag As Boolean
			Dim num As Integer
			Dim membersAndInitializers As SourceMemberContainerTypeSymbol.MembersAndInitializers = Me.GetMembersAndInitializers()
			Dim initializersInSourceTree As ImmutableArray(Of FieldOrPropertyInitializer) = SourceMemberContainerTypeSymbol.GetInitializersInSourceTree(tree, If(isShared, membersAndInitializers.StaticInitializers, membersAndInitializers.InstanceInitializers))
			Dim num1 As Integer = SourceMemberContainerTypeSymbol.IndexOfInitializerContainingPosition(initializersInSourceTree, position)
			If (num1 >= 0) Then
				num = If(isShared, membersAndInitializers.StaticInitializersSyntaxLength, membersAndInitializers.InstanceInitializersSyntaxLength)
				Dim span As TextSpan = initializersInSourceTree(num1).Syntax.Span
				Dim num2 As Integer = position - span.Start
				Dim precedingInitializersLength As Integer = num - (initializersInSourceTree(num1).PrecedingInitializersLength + num2)
				syntaxOffset = -precedingInitializersLength
				flag = True
			Else
				syntaxOffset = 0
				flag = False
			End If
			Return flag
		End Function

		Friend Class MembersAndInitializers
			Friend ReadOnly Members As Dictionary(Of String, ImmutableArray(Of Symbol))

			Friend ReadOnly StaticInitializers As ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer))

			Friend ReadOnly InstanceInitializers As ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer))

			Friend ReadOnly StaticInitializersSyntaxLength As Integer

			Friend ReadOnly InstanceInitializersSyntaxLength As Integer

			Friend Sub New(ByVal members As Dictionary(Of String, ImmutableArray(Of Symbol)), ByVal staticInitializers As ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer)), ByVal instanceInitializers As ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer)), ByVal staticInitializersSyntaxLength As Integer, ByVal instanceInitializersSyntaxLength As Integer)
				MyBase.New()
				Me.Members = members
				Me.StaticInitializers = staticInitializers
				Me.InstanceInitializers = instanceInitializers
				Me.StaticInitializersSyntaxLength = staticInitializersSyntaxLength
				Me.InstanceInitializersSyntaxLength = instanceInitializersSyntaxLength
			End Sub
		End Class

		Friend NotInheritable Class MembersAndInitializersBuilder
			Friend ReadOnly Members As Dictionary(Of String, ArrayBuilder(Of Symbol))

			Friend ReadOnly DeferredMemberDiagnostic As ArrayBuilder(Of Symbol)

			Friend StaticSyntaxLength As Integer

			Friend InstanceSyntaxLength As Integer

			Friend Property InstanceInitializers As ArrayBuilder(Of ImmutableArray(Of FieldOrPropertyInitializer))

			Friend Property StaticInitializers As ArrayBuilder(Of ImmutableArray(Of FieldOrPropertyInitializer))

			Public Sub New()
				MyBase.New()
				Me.Members = New Dictionary(Of String, ArrayBuilder(Of Symbol))(CaseInsensitiveComparison.Comparer)
				Me.DeferredMemberDiagnostic = ArrayBuilder(Of Symbol).GetInstance()
				Me.StaticSyntaxLength = 0
				Me.InstanceSyntaxLength = 0
			End Sub

			Friend Function ToReadOnlyAndFree() As SourceMemberContainerTypeSymbol.MembersAndInitializers
				Dim enumerator As Dictionary(Of String, ArrayBuilder(Of Symbol)).ValueCollection.Enumerator = New Dictionary(Of String, ArrayBuilder(Of Symbol)).ValueCollection.Enumerator()
				Dim immutableArrays As ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer))
				Dim immutableAndFree As ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer))
				Dim immutableAndFree1 As ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer))
				Me.DeferredMemberDiagnostic.Free()
				Dim strs As Dictionary(Of String, ImmutableArray(Of Symbol)) = New Dictionary(Of String, ImmutableArray(Of Symbol))(CaseInsensitiveComparison.Comparer)
				Try
					enumerator = Me.Members.Values.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As ArrayBuilder(Of Symbol) = enumerator.Current
						strs.Add(current(0).Name, current.ToImmutableAndFree())
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
				Dim strs1 As Dictionary(Of String, ImmutableArray(Of Symbol)) = strs
				If (Me.StaticInitializers IsNot Nothing) Then
					immutableAndFree = Me.StaticInitializers.ToImmutableAndFree()
				Else
					immutableArrays = New ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer))()
					immutableAndFree = immutableArrays
				End If
				If (Me.InstanceInitializers IsNot Nothing) Then
					immutableAndFree1 = Me.InstanceInitializers.ToImmutableAndFree()
				Else
					immutableArrays = New ImmutableArray(Of ImmutableArray(Of FieldOrPropertyInitializer))()
					immutableAndFree1 = immutableArrays
				End If
				Return New SourceMemberContainerTypeSymbol.MembersAndInitializers(strs1, immutableAndFree, immutableAndFree1, Me.StaticSyntaxLength, Me.InstanceSyntaxLength)
			End Function
		End Class

		<Flags>
		Friend Enum SourceTypeFlags As UShort
			[Private] = 1
			PrivateProtected = 2
			[Protected] = 3
			[Friend] = 4
			TypeKindShift = 4
			ProtectedFriend = 5
			[Public] = 6
			AccessibilityMask = 7
			[Class] = 32
			[Delegate] = 48
			[Enum] = 80
			[Interface] = 112
			[Module] = 128
			[Structure] = 160
			Submission = 192
			TypeKindMask = 240
			[MustInherit] = 256
			[NotInheritable] = 512
			[Shadows] = 1024
			[Partial] = 2048
		End Enum

		<Flags>
		Protected Enum StateFlags
			FlattenedMembersIsSortedMask = 1
			ReportedVarianceDiagnostics = 2
			ReportedBaseClassConstraintsDiagnostics = 4
			ReportedInterfacesConstraintsDiagnostics = 8
		End Enum

		Private Class StructureCircularityDetectionDataSet
			Private ReadOnly Shared s_pool As ObjectPool(Of SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet)

			Public ReadOnly ProcessedTypes As HashSet(Of NamedTypeSymbol)

			Public ReadOnly Queue As Queue(Of SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet.QueueElement)

			Shared Sub New()
				SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet.s_pool = New ObjectPool(Of SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet)(Function() New SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet(), 32)
			End Sub

			Private Sub New()
				MyBase.New()
				Me.ProcessedTypes = New HashSet(Of NamedTypeSymbol)()
				Me.Queue = New Queue(Of SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet.QueueElement)()
			End Sub

			Public Sub Free()
				Me.Queue.Clear()
				Me.ProcessedTypes.Clear()
				SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet.s_pool.Free(Me)
			End Sub

			Public Shared Function GetInstance() As SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet
				Return SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet.s_pool.Allocate()
			End Function

			Public Structure QueueElement
				Public ReadOnly Type As NamedTypeSymbol

				Public ReadOnly Path As ConsList(Of FieldSymbol)

				Public Sub New(ByVal type As NamedTypeSymbol, ByVal path As ConsList(Of FieldSymbol))
					Me = New SourceMemberContainerTypeSymbol.StructureCircularityDetectionDataSet.QueueElement() With
					{
						.Type = type,
						.Path = path
					}
				End Sub
			End Structure
		End Class

		Private Enum VarianceContext
			[ByVal]
			[ByRef]
			[Return]
			Constraint
			Nullable
			ReadOnlyProperty
			WriteOnlyProperty
			[Property]
			Complex
		End Enum

		Private Structure VarianceDiagnosticsTargetTypeParameter
			Public ReadOnly ConstructedType As NamedTypeSymbol

			Private ReadOnly _typeParameterIndex As Integer

			Public ReadOnly Property TypeParameter As TypeParameterSymbol
				Get
					Return Me.ConstructedType.TypeParameters(Me._typeParameterIndex)
				End Get
			End Property

			Public Sub New(ByVal constructedType As NamedTypeSymbol, ByVal typeParameterIndex As Integer)
				Me = New SourceMemberContainerTypeSymbol.VarianceDiagnosticsTargetTypeParameter() With
				{
					.ConstructedType = constructedType,
					._typeParameterIndex = typeParameterIndex
				}
			End Sub
		End Structure
	End Class
End Namespace