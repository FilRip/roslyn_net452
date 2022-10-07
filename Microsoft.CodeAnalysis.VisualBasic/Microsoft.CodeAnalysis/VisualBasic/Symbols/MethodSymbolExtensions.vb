Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module MethodSymbolExtensions
		<Extension>
		Friend Function CanBeCalledWithNoParameters(ByVal method As MethodSymbol) As Boolean
			Dim flag As Boolean
			Dim parameterCount As Integer = method.ParameterCount
			If (parameterCount <> 0) Then
				Dim parameters As ImmutableArray(Of ParameterSymbol) = method.Parameters
				Dim num As Integer = parameterCount - 1
				Dim num1 As Integer = 0
				Do
					Dim item As ParameterSymbol = parameters(num1)
					If (item.IsParamArray AndAlso num1 = parameterCount - 1) Then
						Dim type As TypeSymbol = item.Type
						If (Not type.IsArrayType() OrElse Not DirectCast(type, ArrayTypeSymbol).IsSZArray) Then
							flag = False
							Return flag
						End If
					ElseIf (Not item.IsOptional) Then
						flag = False
						Return flag
					End If
					num1 = num1 + 1
				Loop While num1 <= num
				flag = True
			Else
				flag = True
			End If
			Return flag
		End Function

		<Extension>
		Friend Function ConstructIfGeneric(ByVal method As MethodSymbol, ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As MethodSymbol
			If (Not method.IsGenericMethod) Then
				Return method
			End If
			Return method.Construct(typeArguments)
		End Function

		<Extension>
		Friend Function GetParameterSymbol(ByVal parameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol), ByVal parameter As ParameterSyntax) As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol
			Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol
			Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = parameter.SyntaxTree
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol).Enumerator = parameters.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = enumerator.Current
					Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.Location).Enumerator = current.Locations.GetEnumerator()
					While enumerator1.MoveNext()
						Dim location As Microsoft.CodeAnalysis.Location = enumerator1.Current
						If (Not location.IsInSource OrElse location.SourceTree <> syntaxTree OrElse Not parameter.Span.Contains(location.SourceSpan)) Then
							Continue While
						End If
						parameterSymbol = current
						Return parameterSymbol
					End While
				Else
					parameterSymbol = Nothing
					Exit While
				End If
			End While
			Return parameterSymbol
		End Function

		<Extension>
		Friend Function IsPartial(ByVal method As MethodSymbol) As Boolean
			Dim sourceMemberMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol = TryCast(method, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol)
			If (sourceMemberMethodSymbol Is Nothing) Then
				Return False
			End If
			Return sourceMemberMethodSymbol.IsPartial
		End Function

		<Extension>
		Friend Function IsPartialWithoutImplementation(ByVal method As MethodSymbol) As Boolean
			Dim sourceMemberMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol = TryCast(method, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol)
			If (sourceMemberMethodSymbol Is Nothing OrElse Not sourceMemberMethodSymbol.IsPartial) Then
				Return False
			End If
			Return sourceMemberMethodSymbol.OtherPartOfPartial Is Nothing
		End Function

		<Extension>
		Friend Function IsUserDefinedOperator(ByVal method As MethodSymbol) As Boolean
			Dim flag As Boolean
			Dim methodKind As Microsoft.CodeAnalysis.MethodKind = method.MethodKind
			flag = If(methodKind = Microsoft.CodeAnalysis.MethodKind.Conversion OrElse methodKind = Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator, True, False)
			Return flag
		End Function
	End Module
End Namespace