Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class SynthesizedLambdaCacheFieldSymbol
		Inherits SynthesizedFieldSymbol
		Implements ISynthesizedMethodBodyImplementationSymbol
		Private ReadOnly _topLevelMethod As MethodSymbol

		Public ReadOnly Property HasMethodBodyDependency As Boolean Implements ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency
			Get
				Return False
			End Get
		End Property

		Public ReadOnly Property Method As IMethodSymbolInternal Implements ISynthesizedMethodBodyImplementationSymbol.Method
			Get
				Return Me._topLevelMethod
			End Get
		End Property

		Public Sub New(ByVal containingType As NamedTypeSymbol, ByVal implicitlyDefinedBy As Symbol, ByVal type As TypeSymbol, ByVal name As String, ByVal topLevelMethod As MethodSymbol, Optional ByVal accessibility As Microsoft.CodeAnalysis.Accessibility = 1, Optional ByVal isReadOnly As Boolean = False, Optional ByVal isShared As Boolean = False, Optional ByVal isSpecialNameAndRuntimeSpecial As Boolean = False)
			MyBase.New(containingType, implicitlyDefinedBy, type, name, accessibility, isReadOnly, isShared, isSpecialNameAndRuntimeSpecial)
			Me._topLevelMethod = topLevelMethod
		End Sub
	End Class
End Namespace