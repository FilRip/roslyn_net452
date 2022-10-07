Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedAddAccessorSymbol
		Inherits SynthesizedEventAccessorSymbol
		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Microsoft.CodeAnalysis.MethodKind.EventAdd
			End Get
		End Property

		Public Sub New(ByVal container As SourceMemberContainerTypeSymbol, ByVal [event] As SourceEventSymbol)
			MyBase.New(container, [event])
		End Sub
	End Class
End Namespace