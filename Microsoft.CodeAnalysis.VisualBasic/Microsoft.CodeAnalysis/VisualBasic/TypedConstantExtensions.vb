Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Module TypedConstantExtensions
		Private Function DisplayEnumConstant(ByVal constant As TypedConstant) As String
			Dim str As String
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = DirectCast(constant.TypeInternal, NamedTypeSymbol).EnumUnderlyingType.SpecialType
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = Microsoft.CodeAnalysis.ConstantValue.Create(RuntimeHelpers.GetObjectValue(constant.ValueInternal), specialType)
			Dim displayString As String = constant.Type.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat)
			str = If(Not constantValue.IsUnsigned, TypedConstantExtensions.DisplaySignedEnumConstant(constant, specialType, constantValue.Int64Value, displayString), TypedConstantExtensions.DisplayUnsignedEnumConstant(constant, specialType, constantValue.UInt64Value, displayString))
			Return str
		End Function

		Private Function DisplaySignedEnumConstant(ByVal constant As TypedConstant, ByVal splType As SpecialType, ByVal constantToDecode As Long, ByVal typeName As String) As String
			Dim stringAndFree As String
			Dim num As Long = CLng(0)
			Dim instance As PooledStringBuilder = Nothing
			Dim builder As StringBuilder = Nothing
			Dim enumerator As ImmutableArray(Of ISymbol).Enumerator = constant.Type.GetMembers().GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As IFieldSymbol = TryCast(enumerator.Current, IFieldSymbol)
					If (current IsNot Nothing AndAlso current.HasConstantValue) Then
						Dim int64Value As Long = ConstantValue.Create(RuntimeHelpers.GetObjectValue(current.ConstantValue), splType).Int64Value
						If (int64Value = constantToDecode) Then
							If (instance IsNot Nothing) Then
								instance.Free()
							End If
							stringAndFree = [String].Concat(typeName, ".", current.Name)
							Exit While
						ElseIf ((int64Value And constantToDecode) = int64Value) Then
							num = num Or int64Value
							If (builder IsNot Nothing) Then
								builder.Append(" Or ")
							Else
								instance = PooledStringBuilder.GetInstance()
								builder = instance.Builder
							End If
							builder.Append(typeName)
							builder.Append(".")
							builder.Append(current.Name)
						End If
					End If
				Else
					If (instance IsNot Nothing) Then
						If (num <> constantToDecode) Then
							instance.Free()
						Else
							stringAndFree = instance.ToStringAndFree()
							Exit While
						End If
					End If
					stringAndFree = constant.ValueInternal.ToString()
					Exit While
				End If
			End While
			Return stringAndFree
		End Function

		Private Function DisplayUnsignedEnumConstant(ByVal constant As TypedConstant, ByVal splType As SpecialType, ByVal constantToDecode As ULong, ByVal typeName As String) As String
			Dim stringAndFree As String
			Dim num As ULong = CULng(0)
			Dim instance As PooledStringBuilder = Nothing
			Dim builder As StringBuilder = Nothing
			Dim enumerator As ImmutableArray(Of ISymbol).Enumerator = constant.Type.GetMembers().GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As IFieldSymbol = TryCast(enumerator.Current, IFieldSymbol)
					If (current IsNot Nothing AndAlso current.HasConstantValue) Then
						Dim uInt64Value As ULong = ConstantValue.Create(RuntimeHelpers.GetObjectValue(current.ConstantValue), splType).UInt64Value
						If (uInt64Value = constantToDecode) Then
							If (instance IsNot Nothing) Then
								instance.Free()
							End If
							stringAndFree = [String].Concat(typeName, ".", current.Name)
							Exit While
						ElseIf ((uInt64Value And constantToDecode) = uInt64Value) Then
							num = num Or uInt64Value
							If (builder IsNot Nothing) Then
								builder.Append(" Or ")
							Else
								instance = PooledStringBuilder.GetInstance()
								builder = instance.Builder
							End If
							builder.Append(typeName)
							builder.Append(".")
							builder.Append(current.Name)
						End If
					End If
				Else
					If (instance IsNot Nothing) Then
						If (num <> constantToDecode) Then
							instance.Free()
						Else
							stringAndFree = instance.ToStringAndFree()
							Exit While
						End If
					End If
					stringAndFree = constant.ValueInternal.ToString()
					Exit While
				End If
			End While
			Return stringAndFree
		End Function

		<Extension>
		Public Function ToVisualBasicString(ByVal constant As TypedConstant) As String
			Dim str As String
			Dim visualBasicString As Func(Of TypedConstant, String)
			If (constant.IsNull) Then
				str = "Nothing"
			ElseIf (constant.Kind = TypedConstantKind.Array) Then
				Dim values As ImmutableArray(Of TypedConstant) = constant.Values
				If (TypedConstantExtensions._Closure$__.$I0-0 Is Nothing) Then
					visualBasicString = Function(v As TypedConstant) v.ToVisualBasicString()
					TypedConstantExtensions._Closure$__.$I0-0 = visualBasicString
				Else
					visualBasicString = TypedConstantExtensions._Closure$__.$I0-0
				End If
				str = [String].Concat("{", [String].Join(", ", values.[Select](Of String)(visualBasicString)), "}")
			ElseIf (constant.Kind = TypedConstantKind.Type OrElse constant.TypeInternal.SpecialType = SpecialType.System_Object) Then
				str = [String].Concat("GetType(", constant.Value.ToString(), ")")
			Else
				str = If(constant.Kind <> TypedConstantKind.[Enum], Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.FormatPrimitive(RuntimeHelpers.GetObjectValue(constant.ValueInternal), True, False), TypedConstantExtensions.DisplayEnumConstant(constant))
			End If
			Return str
		End Function
	End Module
End Namespace