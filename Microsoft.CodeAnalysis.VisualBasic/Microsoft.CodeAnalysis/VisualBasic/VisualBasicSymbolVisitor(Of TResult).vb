Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class VisualBasicSymbolVisitor(Of TResult)
		Protected Sub New()
			MyBase.New()
		End Sub

		Public Overridable Function DefaultVisit(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As TResult
			Return Nothing
		End Function

		Public Overridable Function Visit(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As TResult
			If (symbol IsNot Nothing) Then
				Return symbol.Accept(Of TResult)(Me)
			End If
			Return Nothing
		End Function

		Public Overridable Function VisitAlias(ByVal symbol As AliasSymbol) As TResult
			Return Me.DefaultVisit(symbol)
		End Function

		Public Overridable Function VisitArrayType(ByVal symbol As ArrayTypeSymbol) As TResult
			Return Me.DefaultVisit(symbol)
		End Function

		Public Overridable Function VisitAssembly(ByVal symbol As AssemblySymbol) As TResult
			Return Me.DefaultVisit(symbol)
		End Function

		Public Overridable Function VisitEvent(ByVal symbol As EventSymbol) As TResult
			Return Me.DefaultVisit(symbol)
		End Function

		Public Overridable Function VisitField(ByVal symbol As FieldSymbol) As TResult
			Return Me.DefaultVisit(symbol)
		End Function

		Public Overridable Function VisitLabel(ByVal symbol As LabelSymbol) As TResult
			Return Me.DefaultVisit(symbol)
		End Function

		Public Overridable Function VisitLocal(ByVal symbol As LocalSymbol) As TResult
			Return Me.DefaultVisit(symbol)
		End Function

		Public Overridable Function VisitMethod(ByVal symbol As MethodSymbol) As TResult
			Return Me.DefaultVisit(symbol)
		End Function

		Public Overridable Function VisitModule(ByVal symbol As ModuleSymbol) As TResult
			Return Me.DefaultVisit(symbol)
		End Function

		Public Overridable Function VisitNamedType(ByVal symbol As NamedTypeSymbol) As TResult
			Return Me.DefaultVisit(symbol)
		End Function

		Public Overridable Function VisitNamespace(ByVal symbol As NamespaceSymbol) As TResult
			Return Me.DefaultVisit(symbol)
		End Function

		Public Overridable Function VisitParameter(ByVal symbol As ParameterSymbol) As TResult
			Return Me.DefaultVisit(symbol)
		End Function

		Public Overridable Function VisitProperty(ByVal symbol As PropertySymbol) As TResult
			Return Me.DefaultVisit(symbol)
		End Function

		Public Overridable Function VisitRangeVariable(ByVal symbol As RangeVariableSymbol) As TResult
			Return Me.DefaultVisit(symbol)
		End Function

		Public Overridable Function VisitTypeParameter(ByVal symbol As TypeParameterSymbol) As TResult
			Return Me.DefaultVisit(symbol)
		End Function
	End Class
End Namespace