Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class StateMachineTypeSymbol
		Inherits SynthesizedContainer
		Implements ISynthesizedMethodBodyImplementationSymbol
		Private _attributes As ImmutableArray(Of VisualBasicAttributeData)

		Public ReadOnly KickoffMethod As MethodSymbol

		Public ReadOnly Property HasMethodBodyDependency As Boolean Implements ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency
			Get
				Return True
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsInterface As Boolean
			Get
				Return False
			End Get
		End Property

		Public ReadOnly Property Method As IMethodSymbolInternal Implements ISynthesizedMethodBodyImplementationSymbol.Method
			Get
				Return Me.KickoffMethod
			End Get
		End Property

		Public Sub New(ByVal slotAllocatorOpt As VariableSlotAllocator, ByVal compilationState As TypeCompilationState, ByVal kickoffMethod As MethodSymbol, ByVal kickoffMethodOrdinal As Integer, ByVal baseType As NamedTypeSymbol, ByVal originalInterfaces As ImmutableArray(Of NamedTypeSymbol))
			MyBase.New(kickoffMethod, StateMachineTypeSymbol.MakeName(slotAllocatorOpt, compilationState, kickoffMethod, kickoffMethodOrdinal), baseType, originalInterfaces)
			Me.KickoffMethod = kickoffMethod
		End Sub

		Public NotOverridable Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Dim empty As ImmutableArray(Of VisualBasicAttributeData)
			If (Me._attributes.IsDefault) Then
				Dim instance As ArrayBuilder(Of VisualBasicAttributeData) = Nothing
				Dim containingType As NamedTypeSymbol = Me.KickoffMethod.ContainingType
				Dim attributes As ImmutableArray(Of VisualBasicAttributeData) = containingType.GetAttributes()
				Dim enumerator As ImmutableArray(Of VisualBasicAttributeData).Enumerator = attributes.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As VisualBasicAttributeData = enumerator.Current
					If (Not current.IsTargetAttribute(containingType, AttributeDescription.DebuggerNonUserCodeAttribute) AndAlso Not current.IsTargetAttribute(containingType, AttributeDescription.DebuggerStepThroughAttribute)) Then
						Continue While
					End If
					If (instance Is Nothing) Then
						instance = ArrayBuilder(Of VisualBasicAttributeData).GetInstance(2)
					End If
					instance.Add(current)
				End While
				If (instance Is Nothing) Then
					empty = ImmutableArray(Of VisualBasicAttributeData).Empty
				Else
					empty = instance.ToImmutableAndFree()
				End If
				attributes = New ImmutableArray(Of VisualBasicAttributeData)()
				ImmutableInterlocked.InterlockedCompareExchange(Of VisualBasicAttributeData)(Me._attributes, empty, attributes)
			End If
			Return Me._attributes
		End Function

		Private Shared Function MakeName(ByVal slotAllocatorOpt As VariableSlotAllocator, ByVal compilationState As TypeCompilationState, ByVal kickoffMethod As MethodSymbol, ByVal kickoffMethodOrdinal As Integer) As String
			Dim previousStateMachineTypeName As Object
			Dim currentGenerationOrdinal As Integer
			If (slotAllocatorOpt IsNot Nothing) Then
				previousStateMachineTypeName = slotAllocatorOpt.PreviousStateMachineTypeName
			Else
				previousStateMachineTypeName = Nothing
			End If
			If (previousStateMachineTypeName Is Nothing) Then
				Dim name As String = kickoffMethod.Name
				Dim num As Integer = kickoffMethodOrdinal
				Dim moduleBuilderOpt As PEModuleBuilder = compilationState.ModuleBuilderOpt
				If (moduleBuilderOpt IsNot Nothing) Then
					currentGenerationOrdinal = moduleBuilderOpt.CurrentGenerationOrdinal
				Else
					currentGenerationOrdinal = 0
				End If
				previousStateMachineTypeName = GeneratedNames.MakeStateMachineTypeName(name, num, currentGenerationOrdinal)
			End If
			Return previousStateMachineTypeName
		End Function
	End Class
End Namespace