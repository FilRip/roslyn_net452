Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class VisualBasicSymbolVisitor
		Protected Sub New()
			MyBase.New()
		End Sub

		Public Overridable Sub DefaultVisit(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol)
		End Sub

		Public Overridable Sub Visit(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol)
			If (symbol IsNot Nothing) Then
				symbol.Accept(Me)
			End If
		End Sub

		Public Overridable Sub VisitAlias(ByVal symbol As AliasSymbol)
			Me.DefaultVisit(symbol)
		End Sub

		Public Overridable Sub VisitArrayType(ByVal symbol As ArrayTypeSymbol)
			Me.DefaultVisit(symbol)
		End Sub

		Public Overridable Sub VisitAssembly(ByVal symbol As AssemblySymbol)
			Me.DefaultVisit(symbol)
		End Sub

		Public Overridable Sub VisitEvent(ByVal symbol As EventSymbol)
			Me.DefaultVisit(symbol)
		End Sub

		Public Overridable Sub VisitField(ByVal symbol As FieldSymbol)
			Me.DefaultVisit(symbol)
		End Sub

		Public Overridable Sub VisitLabel(ByVal symbol As LabelSymbol)
			Me.DefaultVisit(symbol)
		End Sub

		Public Overridable Sub VisitLocal(ByVal symbol As LocalSymbol)
			Me.DefaultVisit(symbol)
		End Sub

		Public Overridable Sub VisitMethod(ByVal symbol As MethodSymbol)
			Me.DefaultVisit(symbol)
		End Sub

		Public Overridable Sub VisitModule(ByVal symbol As ModuleSymbol)
			Me.DefaultVisit(symbol)
		End Sub

		Public Overridable Sub VisitNamedType(ByVal symbol As NamedTypeSymbol)
			Me.DefaultVisit(symbol)
		End Sub

		Public Overridable Sub VisitNamespace(ByVal symbol As NamespaceSymbol)
			Me.DefaultVisit(symbol)
		End Sub

		Public Overridable Sub VisitParameter(ByVal symbol As ParameterSymbol)
			Me.DefaultVisit(symbol)
		End Sub

		Public Overridable Sub VisitProperty(ByVal symbol As PropertySymbol)
			Me.DefaultVisit(symbol)
		End Sub

		Public Overridable Sub VisitRangeVariable(ByVal symbol As RangeVariableSymbol)
			Me.DefaultVisit(symbol)
		End Sub

		Public Overridable Sub VisitTypeParameter(ByVal symbol As TypeParameterSymbol)
			Me.DefaultVisit(symbol)
		End Sub
	End Class
End Namespace