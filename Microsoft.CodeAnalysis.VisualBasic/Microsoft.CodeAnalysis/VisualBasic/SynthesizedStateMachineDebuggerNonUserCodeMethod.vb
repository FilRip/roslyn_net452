Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class SynthesizedStateMachineDebuggerNonUserCodeMethod
		Inherits SynthesizedStateMachineMethod
		Friend Sub New(ByVal stateMachineType As StateMachineTypeSymbol, ByVal name As String, ByVal interfaceMethod As MethodSymbol, ByVal syntax As SyntaxNode, ByVal declaredAccessibility As Accessibility, ByVal hasMethodBodyDependency As Boolean, Optional ByVal associatedProperty As PropertySymbol = Nothing)
			MyBase.New(stateMachineType, name, interfaceMethod, syntax, declaredAccessibility, False, hasMethodBodyDependency, associatedProperty)
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.SynthesizeDebuggerNonUserCodeAttribute())
		End Sub
	End Class
End Namespace