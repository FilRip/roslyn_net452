Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Text

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class DocumentationCommentIdVisitor
		Inherits VisualBasicSymbolVisitor(Of StringBuilder, Object)
		Public ReadOnly Shared Instance As DocumentationCommentIdVisitor

		Shared Sub New()
			DocumentationCommentIdVisitor.Instance = New DocumentationCommentIdVisitor()
		End Sub

		Private Sub New()
			MyBase.New()
		End Sub

		Public Overrides Function DefaultVisit(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal builder As StringBuilder) As Object
			Return Nothing
		End Function

		Public Overrides Function VisitArrayType(ByVal symbol As ArrayTypeSymbol, ByVal builder As StringBuilder) As Object
			Return DocumentationCommentIdVisitor.VisitSymbolUsingPrefix(symbol, builder, "T:")
		End Function

		Public Overrides Function VisitErrorType(ByVal symbol As ErrorTypeSymbol, ByVal builder As StringBuilder) As Object
			Return DocumentationCommentIdVisitor.VisitSymbolUsingPrefix(symbol, builder, "!:")
		End Function

		Public Overrides Function VisitEvent(ByVal symbol As EventSymbol, ByVal builder As StringBuilder) As Object
			Return DocumentationCommentIdVisitor.VisitSymbolUsingPrefix(symbol, builder, "E:")
		End Function

		Public Overrides Function VisitField(ByVal symbol As FieldSymbol, ByVal builder As StringBuilder) As Object
			Return DocumentationCommentIdVisitor.VisitSymbolUsingPrefix(symbol, builder, "F:")
		End Function

		Public Overrides Function VisitMethod(ByVal symbol As MethodSymbol, ByVal builder As StringBuilder) As Object
			Return DocumentationCommentIdVisitor.VisitSymbolUsingPrefix(symbol, builder, "M:")
		End Function

		Public Overrides Function VisitNamedType(ByVal symbol As NamedTypeSymbol, ByVal builder As StringBuilder) As Object
			Return DocumentationCommentIdVisitor.VisitSymbolUsingPrefix(symbol, builder, "T:")
		End Function

		Public Overrides Function VisitNamespace(ByVal symbol As NamespaceSymbol, ByVal builder As StringBuilder) As Object
			Return DocumentationCommentIdVisitor.VisitSymbolUsingPrefix(symbol, builder, "N:")
		End Function

		Public Overrides Function VisitProperty(ByVal symbol As PropertySymbol, ByVal builder As StringBuilder) As Object
			Return DocumentationCommentIdVisitor.VisitSymbolUsingPrefix(symbol, builder, "P:")
		End Function

		Private Shared Function VisitSymbolUsingPrefix(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal builder As StringBuilder, ByVal prefix As String) As Object
			builder.Append(prefix)
			DocumentationCommentIdVisitor.PartVisitor.Instance.Visit(symbol, builder)
			Return Nothing
		End Function

		Public Overrides Function VisitTypeParameter(ByVal symbol As TypeParameterSymbol, ByVal builder As StringBuilder) As Object
			Return DocumentationCommentIdVisitor.VisitSymbolUsingPrefix(symbol, builder, "!:")
		End Function

		Private NotInheritable Class PartVisitor
			Inherits VisualBasicSymbolVisitor(Of StringBuilder, Object)
			Public ReadOnly Shared Instance As DocumentationCommentIdVisitor.PartVisitor

			Private ReadOnly Shared s_parameterOrReturnTypeInstance As DocumentationCommentIdVisitor.PartVisitor

			Private ReadOnly _inParameterOrReturnType As Boolean

			Shared Sub New()
				DocumentationCommentIdVisitor.PartVisitor.Instance = New DocumentationCommentIdVisitor.PartVisitor(False)
				DocumentationCommentIdVisitor.PartVisitor.s_parameterOrReturnTypeInstance = New DocumentationCommentIdVisitor.PartVisitor(True)
			End Sub

			Private Sub New(ByVal inParameterOrReturnType As Boolean)
				MyBase.New()
				Me._inParameterOrReturnType = inParameterOrReturnType
			End Sub

			Public Overrides Function VisitArrayType(ByVal symbol As ArrayTypeSymbol, ByVal builder As StringBuilder) As Object
				Me.Visit(symbol.ElementType, builder)
				If (Not symbol.IsSZArray) Then
					builder.Append("[0:")
					Dim rank As Integer = symbol.Rank - 1
					Dim num As Integer = 1
					Do
						builder.Append(",0:")
						num = num + 1
					Loop While num <= rank
					builder.Append("]"C)
				Else
					builder.Append("[]")
				End If
				Return Nothing
			End Function

			Public Overrides Function VisitErrorType(ByVal symbol As ErrorTypeSymbol, ByVal arg As StringBuilder) As Object
				Return Me.VisitNamedType(symbol, arg)
			End Function

			Public Overrides Function VisitEvent(ByVal symbol As EventSymbol, ByVal builder As StringBuilder) As Object
				Me.Visit(symbol.ContainingType, builder)
				builder.Append("."C)
				builder.Append(symbol.Name)
				Return Nothing
			End Function

			Public Overrides Function VisitField(ByVal symbol As FieldSymbol, ByVal builder As StringBuilder) As Object
				Me.Visit(symbol.ContainingType, builder)
				builder.Append("."C)
				builder.Append(symbol.Name)
				Return Nothing
			End Function

			Public Overrides Function VisitMethod(ByVal symbol As MethodSymbol, ByVal builder As StringBuilder) As Object
				Me.Visit(symbol.ContainingType, builder)
				builder.Append("."C)
				builder.Append(symbol.MetadataName.Replace("."C, "#"C))
				If (symbol.Arity <> 0) Then
					builder.Append("``")
					builder.Append(symbol.Arity)
				End If
				If (System.Linq.ImmutableArrayExtensions.Any(Of ParameterSymbol)(symbol.Parameters)) Then
					DocumentationCommentIdVisitor.PartVisitor.s_parameterOrReturnTypeInstance.VisitParameters(symbol.Parameters, builder)
				End If
				If (symbol.MethodKind = MethodKind.Conversion) Then
					builder.Append("~"C)
					DocumentationCommentIdVisitor.PartVisitor.s_parameterOrReturnTypeInstance.Visit(symbol.ReturnType, builder)
				End If
				Return Nothing
			End Function

			Public Overrides Function VisitNamedType(ByVal symbol As NamedTypeSymbol, ByVal builder As StringBuilder) As Object
				Dim obj As Object
				If (Not symbol.IsTupleType) Then
					If (symbol.ContainingSymbol IsNot Nothing AndAlso symbol.ContainingSymbol.Name.Length <> 0) Then
						Me.Visit(symbol.ContainingSymbol, builder)
						builder.Append("."C)
					End If
					builder.Append(symbol.Name)
					If (symbol.Arity <> 0) Then
						If (Me._inParameterOrReturnType OrElse Not TypeSymbol.Equals(symbol, symbol.ConstructedFrom, TypeCompareKind.ConsiderEverything)) Then
							builder.Append("{"C)
							Dim flag As Boolean = False
							Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = symbol.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator()
							While enumerator.MoveNext()
								Dim current As TypeSymbol = enumerator.Current
								If (flag) Then
									builder.Append(","C)
								End If
								Me.Visit(current, builder)
								flag = True
							End While
							builder.Append("}"C)
						Else
							builder.Append(Strings.ChrW(96))
							builder.Append(symbol.Arity)
						End If
					End If
					obj = Nothing
				Else
					obj = Me.VisitNamedType(DirectCast(symbol, TupleTypeSymbol).UnderlyingNamedType, builder)
				End If
				Return obj
			End Function

			Public Overrides Function VisitNamespace(ByVal symbol As NamespaceSymbol, ByVal builder As StringBuilder) As Object
				If (symbol.ContainingNamespace IsNot Nothing AndAlso symbol.ContainingNamespace.Name.Length <> 0) Then
					Me.Visit(symbol.ContainingNamespace, builder)
					builder.Append("."C)
				End If
				builder.Append(symbol.Name)
				Return Nothing
			End Function

			Public Overrides Function VisitParameter(ByVal symbol As ParameterSymbol, ByVal builder As StringBuilder) As Object
				Me.Visit(symbol.Type, builder)
				If (symbol.IsByRef) Then
					builder.Append("@"C)
				End If
				Return Nothing
			End Function

			Private Sub VisitParameters(ByVal parameters As ImmutableArray(Of ParameterSymbol), ByVal builder As StringBuilder)
				builder.Append("("C)
				Dim flag As Boolean = False
				Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = parameters.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ParameterSymbol = enumerator.Current
					If (flag) Then
						builder.Append(","C)
					End If
					Me.Visit(current, builder)
					flag = True
				End While
				builder.Append(")"C)
			End Sub

			Public Overrides Function VisitProperty(ByVal symbol As PropertySymbol, ByVal builder As StringBuilder) As Object
				Me.Visit(symbol.ContainingType, builder)
				builder.Append("."C)
				builder.Append(symbol.MetadataName)
				If (System.Linq.ImmutableArrayExtensions.Any(Of ParameterSymbol)(symbol.Parameters)) Then
					DocumentationCommentIdVisitor.PartVisitor.s_parameterOrReturnTypeInstance.VisitParameters(symbol.Parameters, builder)
				End If
				Return Nothing
			End Function

			Public Overrides Function VisitTypeParameter(ByVal symbol As TypeParameterSymbol, ByVal builder As StringBuilder) As Object
				Dim arity As Integer = 0
				Dim containingSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = symbol.ContainingSymbol
				If (containingSymbol.Kind <> SymbolKind.NamedType) Then
					If (containingSymbol.Kind <> SymbolKind.Method) Then
						Throw ExceptionUtilities.UnexpectedValue(containingSymbol.Kind)
					End If
					builder.Append("``")
				Else
					Dim containingType As NamedTypeSymbol = containingSymbol.ContainingType
					While containingType IsNot Nothing
						arity += containingType.Arity
						containingType = containingType.ContainingType
					End While
					builder.Append(Strings.ChrW(96))
				End If
				builder.Append(symbol.Ordinal + arity)
				Return Nothing
			End Function
		End Class
	End Class
End Namespace