Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module PropertySymbolExtensions
		<Extension>
		Friend Function GetCanBeCalledWithNoParameters(ByVal prop As PropertySymbol) As Boolean
			Dim flag As Boolean
			Dim parameterCount As Integer = prop.ParameterCount
			If (parameterCount <> 0) Then
				Dim parameters As ImmutableArray(Of ParameterSymbol) = prop.Parameters
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
		Public Function GetTypeFromGetMethod(ByVal [property] As PropertySymbol) As TypeSymbol
			Dim getMethod As MethodSymbol = [property].GetMethod
			If (getMethod IsNot Nothing) Then
				Return getMethod.ReturnType
			End If
			Return [property].Type
		End Function

		<Extension>
		Public Function GetTypeFromSetMethod(ByVal [property] As PropertySymbol) As TypeSymbol
			Dim type As TypeSymbol
			Dim setMethod As MethodSymbol = [property].SetMethod
			If (setMethod IsNot Nothing) Then
				Dim parameters As ImmutableArray(Of ParameterSymbol) = setMethod.Parameters
				type = parameters(parameters.Length - 1).Type
			Else
				type = [property].Type
			End If
			Return type
		End Function
	End Module
End Namespace