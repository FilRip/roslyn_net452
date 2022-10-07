Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class IteratorStateMachine
		Inherits StateMachineTypeSymbol
		Private ReadOnly _constructor As SynthesizedSimpleConstructorSymbol

		Private ReadOnly _iteratorMethod As MethodSymbol

		Protected Friend Overrides ReadOnly Property Constructor As MethodSymbol
			Get
				Return Me._constructor
			End Get
		End Property

		Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
			Get
				Return Microsoft.CodeAnalysis.TypeKind.[Class]
			End Get
		End Property

		Protected Friend Sub New(ByVal slotAllocatorOpt As VariableSlotAllocator, ByVal compilationState As TypeCompilationState, ByVal iteratorMethod As MethodSymbol, ByVal iteratorMethodOrdinal As Integer, ByVal valueTypeSymbol As TypeSymbol, ByVal isEnumerable As Boolean)
			MyBase.New(slotAllocatorOpt, compilationState, iteratorMethod, iteratorMethodOrdinal, iteratorMethod.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object), IteratorStateMachine.GetIteratorInterfaces(valueTypeSymbol, isEnumerable, iteratorMethod.ContainingAssembly))
			Dim specialType As NamedTypeSymbol = Me.DeclaringCompilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32)
			Me._constructor = New SynthesizedSimpleConstructorSymbol(Me)
			Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = ImmutableArray.Create(Of ParameterSymbol)(New SynthesizedParameterSymbol(Me._constructor, specialType, 0, False, GeneratedNames.MakeStateMachineStateFieldName()))
			Me._constructor.SetParameters(parameterSymbols)
			Me._iteratorMethod = iteratorMethod
		End Sub

		Private Shared Function GetIteratorInterfaces(ByVal elementType As TypeSymbol, ByVal isEnumerable As Boolean, ByVal containingAssembly As AssemblySymbol) As ImmutableArray(Of NamedTypeSymbol)
			Dim instance As ArrayBuilder(Of NamedTypeSymbol) = ArrayBuilder(Of NamedTypeSymbol).GetInstance()
			If (isEnumerable) Then
				instance.Add(containingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IEnumerable_T).Construct(New TypeSymbol() { elementType }))
				instance.Add(containingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Collections_IEnumerable))
			End If
			instance.Add(containingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IEnumerator_T).Construct(New TypeSymbol() { elementType }))
			instance.Add(containingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_IDisposable))
			instance.Add(containingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Collections_IEnumerator))
			Return instance.ToImmutableAndFree()
		End Function
	End Class
End Namespace