Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class VisualBasicSymbolVisitor(Of TArgument, TResult)
		Protected Sub New()
			MyBase.New()
		End Sub

		Public Overridable Function DefaultVisit(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal arg As TArgument) As TResult
			Return Nothing
		End Function

		Public Overridable Function Visit(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, Optional ByVal arg As TArgument = Nothing) As TResult
			Dim tResult As TResult
			tResult = If(symbol IsNot Nothing, symbol.Accept(Of TArgument, TResult)(Me, arg), Nothing)
			Return tResult
		End Function

		Public Overridable Function VisitAlias(ByVal symbol As AliasSymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function

		Public Overridable Function VisitArrayType(ByVal symbol As ArrayTypeSymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function

		Public Overridable Function VisitAssembly(ByVal symbol As AssemblySymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function

		Public Overridable Function VisitErrorType(ByVal symbol As ErrorTypeSymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function

		Public Overridable Function VisitEvent(ByVal symbol As EventSymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function

		Public Overridable Function VisitField(ByVal symbol As FieldSymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function

		Public Overridable Function VisitLabel(ByVal symbol As LabelSymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function

		Public Overridable Function VisitLocal(ByVal symbol As LocalSymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function

		Public Overridable Function VisitMethod(ByVal symbol As MethodSymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function

		Public Overridable Function VisitModule(ByVal symbol As ModuleSymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function

		Public Overridable Function VisitNamedType(ByVal symbol As NamedTypeSymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function

		Public Overridable Function VisitNamespace(ByVal symbol As NamespaceSymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function

		Public Overridable Function VisitParameter(ByVal symbol As ParameterSymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function

		Public Overridable Function VisitProperty(ByVal symbol As PropertySymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function

		Public Overridable Function VisitRangeVariable(ByVal symbol As RangeVariableSymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function

		Public Overridable Function VisitTypeParameter(ByVal symbol As TypeParameterSymbol, ByVal arg As TArgument) As TResult
			Return Me.DefaultVisit(symbol, arg)
		End Function
	End Class
End Namespace