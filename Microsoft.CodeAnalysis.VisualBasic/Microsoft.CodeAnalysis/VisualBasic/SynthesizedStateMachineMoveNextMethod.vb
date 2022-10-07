Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class SynthesizedStateMachineMoveNextMethod
		Inherits SynthesizedStateMachineMethod
		Private _attributes As ImmutableArray(Of VisualBasicAttributeData)

		Friend Sub New(ByVal stateMachineType As StateMachineTypeSymbol, ByVal interfaceMethod As MethodSymbol, ByVal syntax As SyntaxNode, ByVal declaredAccessibility As Accessibility)
			MyBase.New(stateMachineType, "MoveNext", interfaceMethod, syntax, declaredAccessibility, True, True, Nothing)
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
			Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
			Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
			Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor, typedConstants, keyValuePairs, False))
		End Sub

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Dim empty As ImmutableArray(Of VisualBasicAttributeData)
			If (Me._attributes.IsDefault) Then
				Dim instance As ArrayBuilder(Of VisualBasicAttributeData) = Nothing
				Dim kickoffMethod As MethodSymbol = MyBase.StateMachineType.KickoffMethod
				Dim attributes As ImmutableArray(Of VisualBasicAttributeData) = kickoffMethod.GetAttributes()
				Dim enumerator As ImmutableArray(Of VisualBasicAttributeData).Enumerator = attributes.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As VisualBasicAttributeData = enumerator.Current
					If (Not current.IsTargetAttribute(kickoffMethod, AttributeDescription.DebuggerHiddenAttribute) AndAlso Not current.IsTargetAttribute(kickoffMethod, AttributeDescription.DebuggerNonUserCodeAttribute) AndAlso Not current.IsTargetAttribute(kickoffMethod, AttributeDescription.DebuggerStepperBoundaryAttribute) AndAlso Not current.IsTargetAttribute(kickoffMethod, AttributeDescription.DebuggerStepThroughAttribute)) Then
						Continue While
					End If
					If (instance Is Nothing) Then
						instance = ArrayBuilder(Of VisualBasicAttributeData).GetInstance(4)
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
	End Class
End Namespace