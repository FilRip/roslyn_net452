Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class TypeCompilationState
		Public ReadOnly Compilation As VisualBasicCompilation

		Public staticLambdaFrame As LambdaFrame

		Public ReadOnly ModuleBuilderOpt As PEModuleBuilder

		Private _synthesizedMethods As ArrayBuilder(Of TypeCompilationState.MethodWithBody)

		Public ReadOnly InitializeComponentOpt As MethodSymbol

		Public ReadOnly StateMachineImplementationClass As Dictionary(Of MethodSymbol, NamedTypeSymbol)

		Private _methodWrappers As Dictionary(Of MethodSymbol, MethodSymbol)

		Private _initializeComponentCallTree As Dictionary(Of MethodSymbol, ImmutableArray(Of MethodSymbol))

		Public ReadOnly Property HasSynthesizedMethods As Boolean
			Get
				Return Me._synthesizedMethods IsNot Nothing
			End Get
		End Property

		Public ReadOnly Property SynthesizedMethods As ArrayBuilder(Of TypeCompilationState.MethodWithBody)
			Get
				Return Me._synthesizedMethods
			End Get
		End Property

		Public Sub New(ByVal compilation As VisualBasicCompilation, ByVal moduleBuilderOpt As PEModuleBuilder, ByVal initializeComponentOpt As MethodSymbol)
			MyBase.New()
			Me._synthesizedMethods = Nothing
			Me.StateMachineImplementationClass = New Dictionary(Of MethodSymbol, NamedTypeSymbol)(ReferenceEqualityComparer.Instance)
			Me._methodWrappers = Nothing
			Me._initializeComponentCallTree = Nothing
			Me.Compilation = compilation
			Me.ModuleBuilderOpt = moduleBuilderOpt
			Me.InitializeComponentOpt = initializeComponentOpt
		End Sub

		Public Sub AddMethodWrapper(ByVal method As MethodSymbol, ByVal wrapper As MethodSymbol, ByVal body As BoundStatement)
			If (Me._methodWrappers Is Nothing) Then
				Me._methodWrappers = New Dictionary(Of MethodSymbol, MethodSymbol)()
			End If
			Me._methodWrappers(method) = wrapper
			Me.AddSynthesizedMethod(wrapper, body)
		End Sub

		Public Sub AddSynthesizedMethod(ByVal method As MethodSymbol, ByVal body As BoundStatement)
			If (Me._synthesizedMethods Is Nothing) Then
				Me._synthesizedMethods = ArrayBuilder(Of TypeCompilationState.MethodWithBody).GetInstance()
			End If
			Me._synthesizedMethods.Add(New TypeCompilationState.MethodWithBody(method, body))
		End Sub

		Public Sub AddToInitializeComponentCallTree(ByVal method As MethodSymbol, ByVal callees As ImmutableArray(Of MethodSymbol))
			If (Me._initializeComponentCallTree Is Nothing) Then
				Me._initializeComponentCallTree = New Dictionary(Of MethodSymbol, ImmutableArray(Of MethodSymbol))(ReferenceEqualityComparer.Instance)
			End If
			Me._initializeComponentCallTree.Add(method, callees)
		End Sub

		Public Function CallsInitializeComponent(ByVal method As MethodSymbol) As Boolean
			Dim flag As Boolean
			flag = If(Me._initializeComponentCallTree IsNot Nothing, Me.CallsInitializeComponent(method, New HashSet(Of MethodSymbol)(ReferenceEqualityComparer.Instance)), False)
			Return flag
		End Function

		Private Function CallsInitializeComponent(ByVal method As MethodSymbol, ByVal visited As HashSet(Of MethodSymbol)) As Boolean
			Dim flag As Boolean
			visited.Add(method)
			Dim methodSymbols As ImmutableArray(Of MethodSymbol) = New ImmutableArray(Of MethodSymbol)()
			If (Me._initializeComponentCallTree.TryGetValue(method, methodSymbols)) Then
				Dim enumerator As ImmutableArray(Of MethodSymbol).Enumerator = methodSymbols.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As MethodSymbol = enumerator.Current
					If (CObj(current) <> CObj(Me.InitializeComponentOpt)) Then
						If (visited.Contains(current) OrElse Not Me.CallsInitializeComponent(current, visited)) Then
							Continue While
						End If
						flag = True
						Return flag
					Else
						flag = True
						Return flag
					End If
				End While
			End If
			flag = False
			Return flag
		End Function

		Public Sub Free()
			If (Me._synthesizedMethods IsNot Nothing) Then
				Me._synthesizedMethods.Free()
				Me._synthesizedMethods = Nothing
			End If
			If (Me._methodWrappers IsNot Nothing) Then
				Me._methodWrappers = Nothing
			End If
		End Sub

		Public Function GetMethodWrapper(ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (Me._methodWrappers IsNot Nothing AndAlso Me._methodWrappers.TryGetValue(method, methodSymbol)) Then
				Return methodSymbol
			End If
			Return Nothing
		End Function

		Public Function HasMethodWrapper(ByVal method As MethodSymbol) As Boolean
			If (Me._methodWrappers Is Nothing) Then
				Return False
			End If
			Return Me._methodWrappers.ContainsKey(method)
		End Function

		Public Structure MethodWithBody
			Public ReadOnly Method As MethodSymbol

			Public ReadOnly Body As BoundStatement

			Friend Sub New(ByVal _method As MethodSymbol, ByVal _body As BoundStatement)
				Me = New TypeCompilationState.MethodWithBody() With
				{
					.Method = _method,
					.Body = _body
				}
			End Sub
		End Structure
	End Class
End Namespace