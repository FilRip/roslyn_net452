Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class LambdaFrame
		Inherits SynthesizedContainer
		Implements ISynthesizedMethodBodyImplementationSymbol
		Private ReadOnly _typeParameters As ImmutableArray(Of TypeParameterSymbol)

		Private ReadOnly _topLevelMethod As MethodSymbol

		Private ReadOnly _sharedConstructor As MethodSymbol

		Private ReadOnly _singletonCache As FieldSymbol

		Friend ReadOnly ClosureOrdinal As Integer

		Friend ReadOnly CapturedLocals As ArrayBuilder(Of LambdaCapturedVariable)

		Private ReadOnly _constructor As SynthesizedLambdaConstructor

		Friend ReadOnly TypeMap As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution

		Private ReadOnly _scopeSyntaxOpt As SyntaxNode

		Private ReadOnly Shared s_typeSubstitutionFactory As Func(Of Symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution)

		Friend ReadOnly Shared CreateTypeParameter As Func(Of TypeParameterSymbol, Symbol, TypeParameterSymbol)

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return Me._typeParameters.Length
			End Get
		End Property

		Protected Friend Overrides ReadOnly Property Constructor As MethodSymbol
			Get
				Return Me._constructor
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.Internal
			End Get
		End Property

		Public ReadOnly Property HasMethodBodyDependency As Boolean Implements ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property IsInterface As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsSerializable As Boolean
			Get
				Return CObj(Me._singletonCache) <> CObj(Nothing)
			End Get
		End Property

		Public Overrides ReadOnly Property MemberNames As IEnumerable(Of String)
			Get
				Return SpecializedCollections.EmptyEnumerable(Of String)()
			End Get
		End Property

		Public ReadOnly Property Method As IMethodSymbolInternal Implements ISynthesizedMethodBodyImplementationSymbol.Method
			Get
				Return Me._topLevelMethod
			End Get
		End Property

		Public ReadOnly Property ScopeSyntax As SyntaxNode
			Get
				Return Me._constructor.Syntax
			End Get
		End Property

		Protected Friend ReadOnly Property SharedConstructor As MethodSymbol
			Get
				Return Me._sharedConstructor
			End Get
		End Property

		Friend ReadOnly Property SingletonCache As FieldSymbol
			Get
				Return Me._singletonCache
			End Get
		End Property

		Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
			Get
				Return Microsoft.CodeAnalysis.TypeKind.[Class]
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return Me._typeParameters
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.LambdaFrame.s_typeSubstitutionFactory = Function(container As Symbol)
				Dim lambdaFrame1 As Microsoft.CodeAnalysis.VisualBasic.LambdaFrame = TryCast(container, Microsoft.CodeAnalysis.VisualBasic.LambdaFrame)
				If (lambdaFrame1 IsNot Nothing) Then
					Return lambdaFrame1.TypeMap
				End If
				Return DirectCast(container, SynthesizedMethod).TypeMap
			End Function
			Microsoft.CodeAnalysis.VisualBasic.LambdaFrame.CreateTypeParameter = Function(typeParameter As TypeParameterSymbol, container As Symbol) New SynthesizedClonedTypeParameterSymbol(typeParameter, container, GeneratedNames.MakeDisplayClassGenericParameterName(typeParameter.Ordinal), Microsoft.CodeAnalysis.VisualBasic.LambdaFrame.s_typeSubstitutionFactory)
		End Sub

		Friend Sub New(ByVal topLevelMethod As MethodSymbol, ByVal scopeSyntaxOpt As SyntaxNode, ByVal methodId As DebugId, ByVal closureId As DebugId, ByVal copyConstructor As Boolean, ByVal isStatic As Boolean, ByVal isDelegateRelaxationFrame As Boolean)
			MyBase.New(topLevelMethod, LambdaFrame.MakeName(scopeSyntaxOpt, methodId, closureId, isStatic, isDelegateRelaxationFrame), topLevelMethod.ContainingType, ImmutableArray(Of NamedTypeSymbol).Empty)
			Me.CapturedLocals = New ArrayBuilder(Of LambdaCapturedVariable)()
			If (Not copyConstructor) Then
				Me._constructor = New SynthesizedLambdaConstructor(scopeSyntaxOpt, Me)
			Else
				Me._constructor = New SynthesizedLambdaCopyConstructor(scopeSyntaxOpt, Me)
			End If
			If (Not isStatic) Then
				Me._scopeSyntaxOpt = scopeSyntaxOpt
			Else
				Me._sharedConstructor = New SynthesizedConstructorSymbol(Nothing, Me, True, False, Nothing, Nothing)
				Dim str As String = GeneratedNames.MakeCachedFrameInstanceName()
				Me._singletonCache = New SynthesizedLambdaCacheFieldSymbol(Me, Me, Me, str, topLevelMethod, Accessibility.[Public], True, True, False)
				Me._scopeSyntaxOpt = Nothing
			End If
			Me._typeParameters = SynthesizedClonedTypeParameterSymbol.MakeTypeParameters(topLevelMethod.TypeParameters, Me, LambdaFrame.CreateTypeParameter)
			Me.TypeMap = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(topLevelMethod, topLevelMethod.TypeParameters, MyBase.TypeArgumentsNoUseSiteDiagnostics, False)
			Me._topLevelMethod = topLevelMethod
		End Sub

		<Conditional("DEBUG")>
		Private Shared Sub AssertIsClosureScopeSyntax(ByVal syntaxOpt As SyntaxNode)
			If (syntaxOpt IsNot Nothing AndAlso Not LambdaUtilities.IsClosureScope(syntaxOpt) AndAlso syntaxOpt.Kind() <> SyntaxKind.ObjectMemberInitializer) Then
				ExceptionUtilities.UnexpectedValue(syntaxOpt.Kind())
			End If
		End Sub

		Friend Overrides Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)
			Dim capturedLocals As IEnumerable(Of FieldSymbol)
			If (Me._singletonCache IsNot Nothing) Then
				capturedLocals = Me.CapturedLocals.Concat(Me._singletonCache)
			Else
				capturedLocals = Me.CapturedLocals
			End If
			Return capturedLocals
		End Function

		Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Return ImmutableArray(Of Symbol).Empty
		End Function

		Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
			Dim symbols As ImmutableArray(Of Symbol) = StaticCast(Of Symbol).From(Of LambdaCapturedVariable)(Me.CapturedLocals.AsImmutable())
			symbols = If(Me._sharedConstructor Is Nothing, symbols.Add(Me._constructor), symbols.AddRange(ImmutableArray.Create(Of Symbol)(Me._constructor, Me._sharedConstructor, Me._singletonCache)))
			Return symbols
		End Function

		Friend Overrides Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Return Me.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object)
		End Function

		Friend Overrides Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Friend Overrides Function MakeDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Return Me.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object)
		End Function

		Friend Overrides Function MakeDeclaredInterfaces(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Private Shared Function MakeName(ByVal scopeSyntaxOpt As SyntaxNode, ByVal methodId As DebugId, ByVal closureId As DebugId, ByVal isStatic As Boolean, ByVal isDelegateRelaxation As Boolean) As String
			Dim str As String
			str = If(Not isStatic, GeneratedNames.MakeLambdaDisplayClassName(methodId.Ordinal, methodId.Generation, closureId.Ordinal, closureId.Generation, isDelegateRelaxation), GeneratedNames.MakeStaticLambdaDisplayClassName(methodId.Ordinal, methodId.Generation))
			Return str
		End Function
	End Class
End Namespace