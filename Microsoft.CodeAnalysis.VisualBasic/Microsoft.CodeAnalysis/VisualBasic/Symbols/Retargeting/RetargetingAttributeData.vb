Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
	Friend Class RetargetingAttributeData
		Inherits SourceAttributeData
		Friend Sub New(ByVal applicationNode As SyntaxReference, ByVal attributeClass As NamedTypeSymbol, ByVal attributeConstructor As MethodSymbol, ByVal constructorArguments As ImmutableArray(Of TypedConstant), ByVal namedArguments As ImmutableArray(Of KeyValuePair(Of String, TypedConstant)), ByVal isConditionallyOmitted As Boolean, ByVal hasErrors As Boolean)
			MyBase.New(applicationNode, attributeClass, attributeConstructor, constructorArguments, namedArguments, isConditionallyOmitted, hasErrors)
		End Sub

		Friend Overrides Function GetSystemType(ByVal targetSymbol As Symbol) As TypeSymbol
			Dim containingAssembly As Object
			If (targetSymbol.Kind = SymbolKind.Assembly) Then
				containingAssembly = targetSymbol
			Else
				containingAssembly = targetSymbol.ContainingAssembly
			End If
			Dim retargetingAssemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting.RetargetingAssemblySymbol = DirectCast(containingAssembly, Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting.RetargetingAssemblySymbol)
			Dim wellKnownType As TypeSymbol = retargetingAssemblySymbol.UnderlyingAssembly.DeclaringCompilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Type)
			Dim modules As ImmutableArray(Of ModuleSymbol) = retargetingAssemblySymbol.Modules
			Return DirectCast(modules(0), RetargetingModuleSymbol).RetargetingTranslator.Retarget(wellKnownType, RetargetOptions.RetargetPrimitiveTypesByTypeCode)
		End Function
	End Class
End Namespace