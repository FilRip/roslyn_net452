Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SynthesizedMyGroupCollectionPropertyBackingFieldSymbol
		Inherits SynthesizedFieldSymbol
		Public Sub New(ByVal containingType As NamedTypeSymbol, ByVal implicitlyDefinedBy As Symbol, ByVal type As TypeSymbol, ByVal name As String)
			MyBase.New(containingType, implicitlyDefinedBy, type, name, Accessibility.[Public], False, False, False)
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.SynthesizeEditorBrowsableNeverAttribute())
		End Sub

		Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
			Return LexicalSortKey.NotInSource
		End Function
	End Class
End Namespace