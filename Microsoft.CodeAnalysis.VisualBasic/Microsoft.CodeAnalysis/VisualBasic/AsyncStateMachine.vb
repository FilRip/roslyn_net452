Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class AsyncStateMachine
		Inherits StateMachineTypeSymbol
		Private ReadOnly _typeKind As Microsoft.CodeAnalysis.TypeKind

		Private ReadOnly _constructor As SynthesizedSimpleConstructorSymbol

		Protected Friend Overrides ReadOnly Property Constructor As MethodSymbol
			Get
				Return Me._constructor
			End Get
		End Property

		Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
			Get
				Return Me._typeKind
			End Get
		End Property

		Protected Friend Sub New(ByVal slotAllocatorOpt As VariableSlotAllocator, ByVal compilationState As TypeCompilationState, ByVal asyncMethod As MethodSymbol, ByVal asyncMethodOrdinal As Integer, ByVal typeKind As Microsoft.CodeAnalysis.TypeKind)
			MyBase.New(slotAllocatorOpt, compilationState, asyncMethod, asyncMethodOrdinal, asyncMethod.ContainingAssembly.GetSpecialType(If(typeKind = Microsoft.CodeAnalysis.TypeKind.Struct, Microsoft.CodeAnalysis.SpecialType.System_ValueType, Microsoft.CodeAnalysis.SpecialType.System_Object)), ImmutableArray.Create(Of NamedTypeSymbol)(asyncMethod.DeclaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_IAsyncStateMachine)))
			Me._constructor = New SynthesizedSimpleConstructorSymbol(Me)
			Me._constructor.SetParameters(ImmutableArray(Of ParameterSymbol).Empty)
			Me._typeKind = typeKind
		End Sub
	End Class
End Namespace