Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class SourceModuleBinder
		Inherits Binder
		Private ReadOnly _sourceModule As SourceModuleSymbol

		Public Overrides ReadOnly Property CheckOverflow As Boolean
			Get
				Return Me._sourceModule.Options.CheckOverflow
			End Get
		End Property

		Public Overrides ReadOnly Property OptionCompareText As Boolean
			Get
				Return Me._sourceModule.Options.OptionCompareText
			End Get
		End Property

		Public Overrides ReadOnly Property OptionExplicit As Boolean
			Get
				Return Me._sourceModule.Options.OptionExplicit
			End Get
		End Property

		Public Overrides ReadOnly Property OptionInfer As Boolean
			Get
				Return Me._sourceModule.Options.OptionInfer
			End Get
		End Property

		Public Overrides ReadOnly Property OptionStrict As Microsoft.CodeAnalysis.VisualBasic.OptionStrict
			Get
				Return Me._sourceModule.Options.OptionStrict
			End Get
		End Property

		Public Overrides ReadOnly Property QuickAttributeChecker As Microsoft.CodeAnalysis.VisualBasic.Symbols.QuickAttributeChecker
			Get
				Return Me._sourceModule.QuickAttributeChecker
			End Get
		End Property

		Public Sub New(ByVal containingBinder As Binder, ByVal sourceModule As SourceModuleSymbol)
			MyBase.New(containingBinder, sourceModule, sourceModule.ContainingSourceAssembly.DeclaringCompilation)
			Me._sourceModule = sourceModule
		End Sub

		Public Overrides Function CheckAccessibility(ByVal sym As Symbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal accessThroughType As TypeSymbol = Nothing, Optional ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = Nothing) As AccessCheckResult
			If (MyBase.IgnoresAccessibility) Then
				Return AccessCheckResult.Accessible
			End If
			Return AccessCheck.CheckSymbolAccessibility(sym, Me._sourceModule.ContainingSourceAssembly, useSiteInfo, basesBeingResolved)
		End Function
	End Class
End Namespace