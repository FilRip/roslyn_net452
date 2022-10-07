Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SynthesizedPropertyAccessorBase(Of T As PropertySymbol)
		Inherits SynthesizedAccessor(Of T)
		Friend MustOverride ReadOnly Property BackingFieldSymbol As FieldSymbol

		Protected Sub New(ByVal container As NamedTypeSymbol, ByVal [property] As T)
			MyBase.New(container, [property])
		End Sub

		Friend Overrides Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByRef methodBodyBinder As Binder = Nothing) As BoundBlock
			Return SynthesizedPropertyAccessorHelper.GetBoundMethodBody(Me, Me.BackingFieldSymbol, methodBodyBinder)
		End Function
	End Class
End Namespace