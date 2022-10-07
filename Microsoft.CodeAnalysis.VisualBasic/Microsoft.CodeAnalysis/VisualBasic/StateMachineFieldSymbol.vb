Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class StateMachineFieldSymbol
		Inherits SynthesizedFieldSymbol
		Implements ISynthesizedMethodBodyImplementationSymbol
		Friend ReadOnly SlotIndex As Integer

		Friend ReadOnly SlotDebugInfo As LocalSlotDebugInfo

		Public ReadOnly Property HasMethodBodyDependency As Boolean Implements ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency
			Get
				Return True
			End Get
		End Property

		Public ReadOnly Property Method As IMethodSymbolInternal Implements ISynthesizedMethodBodyImplementationSymbol.Method
			Get
				Return DirectCast(Me.ContainingSymbol, ISynthesizedMethodBodyImplementationSymbol).Method
			End Get
		End Property

		Public Sub New(ByVal stateMachineType As NamedTypeSymbol, ByVal implicitlyDefinedBy As Symbol, ByVal type As TypeSymbol, ByVal name As String, Optional ByVal accessibility As Microsoft.CodeAnalysis.Accessibility = 1, Optional ByVal isReadOnly As Boolean = False, Optional ByVal isShared As Boolean = False, Optional ByVal isSpecialNameAndRuntimeSpecial As Boolean = False)
			MyClass.New(stateMachineType, implicitlyDefinedBy, type, name, New LocalSlotDebugInfo(SynthesizedLocalKind.LoweringTemp, LocalDebugId.None), -1, accessibility, isReadOnly, isShared, False)
		End Sub

		Public Sub New(ByVal stateMachineType As NamedTypeSymbol, ByVal implicitlyDefinedBy As Symbol, ByVal type As TypeSymbol, ByVal name As String, ByVal synthesizedKind As SynthesizedLocalKind, ByVal slotindex As Integer, Optional ByVal accessibility As Microsoft.CodeAnalysis.Accessibility = 1, Optional ByVal isReadOnly As Boolean = False, Optional ByVal isShared As Boolean = False, Optional ByVal isSpecialNameAndRuntimeSpecial As Boolean = False)
			MyClass.New(stateMachineType, implicitlyDefinedBy, type, name, New LocalSlotDebugInfo(synthesizedKind, LocalDebugId.None), slotindex, accessibility, isReadOnly, isShared, False)
		End Sub

		Public Sub New(ByVal stateMachineType As NamedTypeSymbol, ByVal implicitlyDefinedBy As Symbol, ByVal type As TypeSymbol, ByVal name As String, ByVal slotDebugInfo As LocalSlotDebugInfo, ByVal slotIndex As Integer, Optional ByVal accessibility As Microsoft.CodeAnalysis.Accessibility = 1, Optional ByVal isReadOnly As Boolean = False, Optional ByVal isShared As Boolean = False, Optional ByVal isSpecialNameAndRuntimeSpecial As Boolean = False)
			MyBase.New(stateMachineType, implicitlyDefinedBy, type, name, accessibility, isReadOnly, isShared, isSpecialNameAndRuntimeSpecial)
			Me.SlotIndex = slotIndex
			Me.SlotDebugInfo = slotDebugInfo
		End Sub
	End Class
End Namespace