Imports Microsoft.CodeAnalysis
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module SpecialTypeExtensions
		<Extension>
		Public Function GetDisplayName(ByVal this As SpecialType) As String
			Return this.TryGetKeywordText()
		End Function

		<Extension>
		Public Function GetNativeCompilerVType(ByVal this As SpecialType) As String
			Dim str As String
			Select Case this
				Case SpecialType.System_Void
					str = "t_void"
					Exit Select
				Case SpecialType.System_Boolean
					str = "t_bool"
					Exit Select
				Case SpecialType.System_Char
					str = "t_char"
					Exit Select
				Case SpecialType.System_SByte
					str = "t_i1"
					Exit Select
				Case SpecialType.System_Byte
					str = "t_ui1"
					Exit Select
				Case SpecialType.System_Int16
					str = "t_i2"
					Exit Select
				Case SpecialType.System_UInt16
					str = "t_ui2"
					Exit Select
				Case SpecialType.System_Int32
					str = "t_i4"
					Exit Select
				Case SpecialType.System_UInt32
					str = "t_ui4"
					Exit Select
				Case SpecialType.System_Int64
					str = "t_i8"
					Exit Select
				Case SpecialType.System_UInt64
					str = "t_ui8"
					Exit Select
				Case SpecialType.System_Decimal
					str = "t_decimal"
					Exit Select
				Case SpecialType.System_Single
					str = "t_single"
					Exit Select
				Case SpecialType.System_Double
					str = "t_double"
					Exit Select
				Case SpecialType.System_String
					str = "t_string"
					Exit Select
				Case SpecialType.System_IntPtr
				Case SpecialType.System_UIntPtr
					str = "t_ptr"
					Exit Select
				Case SpecialType.System_Array
					str = "t_array"
					Exit Select
				Case SpecialType.System_Collections_IEnumerable
				Case SpecialType.System_Collections_Generic_IEnumerable_T
				Case SpecialType.System_Collections_Generic_IList_T
				Case SpecialType.System_Collections_Generic_ICollection_T
				Case SpecialType.System_Collections_IEnumerator
				Case SpecialType.System_Collections_Generic_IEnumerator_T
				Case SpecialType.System_Collections_Generic_IReadOnlyList_T
				Case SpecialType.System_Collections_Generic_IReadOnlyCollection_T
				Case SpecialType.System_Nullable_T
				Label0:
					str = Nothing
					Exit Select
				Case SpecialType.System_DateTime
					str = "t_date"
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return str
		End Function

		<Extension>
		Friend Function GetShiftSizeMask(ByVal this As SpecialType) As Integer
			Dim num As Integer
			Select Case this
				Case SpecialType.System_SByte
				Case SpecialType.System_Byte
					num = 7
					Exit Select
				Case SpecialType.System_Int16
				Case SpecialType.System_UInt16
					num = 15
					Exit Select
				Case SpecialType.System_Int32
				Case SpecialType.System_UInt32
					num = 31
					Exit Select
				Case SpecialType.System_Int64
				Case SpecialType.System_UInt64
					num = 63
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(this)
			End Select
			Return num
		End Function

		<Extension>
		Public Function IsFloatingType(ByVal this As SpecialType) As Boolean
			Return If(CSByte(this) - CSByte(SpecialType.System_Single) > CSByte(SpecialType.System_Object), False, True)
		End Function

		<Extension>
		Public Function IsIntrinsicType(ByVal this As SpecialType) As Boolean
			If (this = SpecialType.System_String) Then
				Return True
			End If
			Return this.IsIntrinsicValueType()
		End Function

		<Extension>
		Public Function IsIntrinsicValueType(ByVal this As Microsoft.CodeAnalysis.SpecialType) As Boolean
			Dim flag As Boolean
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = this
			flag = If(CSByte(specialType) - CSByte(Microsoft.CodeAnalysis.SpecialType.System_Boolean) <= CSByte(Microsoft.CodeAnalysis.SpecialType.System_UInt16) OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_DateTime, True, False)
			Return flag
		End Function

		<Extension>
		Public Function IsPrimitiveType(ByVal this As SpecialType) As Boolean
			Dim flag As Boolean
			Select Case this
				Case SpecialType.System_Boolean
				Case SpecialType.System_Char
				Case SpecialType.System_SByte
				Case SpecialType.System_Byte
				Case SpecialType.System_Int16
				Case SpecialType.System_UInt16
				Case SpecialType.System_Int32
				Case SpecialType.System_UInt32
				Case SpecialType.System_Int64
				Case SpecialType.System_UInt64
				Case SpecialType.System_Single
				Case SpecialType.System_Double
				Case SpecialType.System_IntPtr
				Case SpecialType.System_UIntPtr
					flag = True
					Exit Select
				Case SpecialType.System_Decimal
				Case SpecialType.System_String
				Label0:
					flag = False
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return flag
		End Function

		<Extension>
		Public Function IsRestrictedType(ByVal this As SpecialType) As Boolean
			Return If(CSByte(this) - CSByte(SpecialType.System_TypedReference) > CSByte(SpecialType.System_Enum), False, True)
		End Function

		<Extension>
		Public Function IsStrictSupertypeOfConcreteDelegate(ByVal this As Microsoft.CodeAnalysis.SpecialType) As Boolean
			Dim flag As Boolean
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = this
			flag = If(specialType = Microsoft.CodeAnalysis.SpecialType.System_Object OrElse CSByte(specialType) - CSByte(Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate) <= CSByte(Microsoft.CodeAnalysis.SpecialType.System_Object), True, False)
			Return flag
		End Function

		<Extension>
		Public Function IsValidTypeForAttributeArgument(ByVal this As SpecialType) As Boolean
			Dim flag As Boolean
			Select Case this
				Case SpecialType.System_Object
				Case SpecialType.System_Boolean
				Case SpecialType.System_Char
				Case SpecialType.System_SByte
				Case SpecialType.System_Byte
				Case SpecialType.System_Int16
				Case SpecialType.System_UInt16
				Case SpecialType.System_Int32
				Case SpecialType.System_UInt32
				Case SpecialType.System_Int64
				Case SpecialType.System_UInt64
				Case SpecialType.System_Single
				Case SpecialType.System_Double
				Case SpecialType.System_String
					flag = True
					Exit Select
				Case SpecialType.System_Enum
				Case SpecialType.System_MulticastDelegate
				Case SpecialType.System_Delegate
				Case SpecialType.System_ValueType
				Case SpecialType.System_Void
				Case SpecialType.System_Decimal
				Label0:
					flag = False
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return flag
		End Function

		<Extension>
		Public Function IsValidTypeForSwitchTable(ByVal this As Microsoft.CodeAnalysis.SpecialType) As Boolean
			Dim flag As Boolean
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = this
			flag = If(CSByte(specialType) - CSByte(Microsoft.CodeAnalysis.SpecialType.System_Boolean) <= CSByte(Microsoft.CodeAnalysis.SpecialType.System_SByte) OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_String, True, False)
			Return flag
		End Function

		<Extension>
		Friend Function ToConstantValueDiscriminator(ByVal this As SpecialType) As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator
			Dim constantValueTypeDiscriminator As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator
			Select Case this
				Case SpecialType.System_Boolean
					constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Boolean]
					Exit Select
				Case SpecialType.System_Char
					constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Char]
					Exit Select
				Case SpecialType.System_SByte
					constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[SByte]
					Exit Select
				Case SpecialType.System_Byte
					constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Byte]
					Exit Select
				Case SpecialType.System_Int16
					constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.Int16
					Exit Select
				Case SpecialType.System_UInt16
					constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.UInt16
					Exit Select
				Case SpecialType.System_Int32
					constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.Int32
					Exit Select
				Case SpecialType.System_UInt32
					constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.UInt32
					Exit Select
				Case SpecialType.System_Int64
					constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.Int64
					Exit Select
				Case SpecialType.System_UInt64
					constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.UInt64
					Exit Select
				Case SpecialType.System_Decimal
					constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Decimal]
					Exit Select
				Case SpecialType.System_Single
					constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Single]
					Exit Select
				Case SpecialType.System_Double
					constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Double]
					Exit Select
				Case SpecialType.System_String
					constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[String]
					Exit Select
				Case SpecialType.System_IntPtr
				Case SpecialType.System_UIntPtr
				Case SpecialType.System_Array
				Case SpecialType.System_Collections_IEnumerable
				Case SpecialType.System_Collections_Generic_IEnumerable_T
				Case SpecialType.System_Collections_Generic_IList_T
				Case SpecialType.System_Collections_Generic_ICollection_T
				Case SpecialType.System_Collections_IEnumerator
				Case SpecialType.System_Collections_Generic_IEnumerator_T
				Case SpecialType.System_Collections_Generic_IReadOnlyList_T
				Case SpecialType.System_Collections_Generic_IReadOnlyCollection_T
				Case SpecialType.System_Nullable_T
					Throw ExceptionUtilities.UnexpectedValue(this)
				Case SpecialType.System_DateTime
					constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.DateTime
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(this)
			End Select
			Return constantValueTypeDiscriminator
		End Function

		<Extension>
		Public Function ToRuntimeType(ByVal this As SpecialType) As System.Type
			Dim type As System.Type
			Select Case this
				Case SpecialType.System_Object
					type = GetType(Object)
					Exit Select
				Case SpecialType.System_Enum
				Case SpecialType.System_MulticastDelegate
				Case SpecialType.System_Delegate
				Case SpecialType.System_ValueType
				Case SpecialType.System_IntPtr
				Case SpecialType.System_UIntPtr
				Case SpecialType.System_Array
				Case SpecialType.System_Collections_IEnumerable
				Case SpecialType.System_Collections_Generic_IEnumerable_T
				Case SpecialType.System_Collections_Generic_IList_T
				Case SpecialType.System_Collections_Generic_ICollection_T
				Case SpecialType.System_Collections_IEnumerator
				Case SpecialType.System_Collections_Generic_IEnumerator_T
				Case SpecialType.System_Collections_Generic_IReadOnlyList_T
				Case SpecialType.System_Collections_Generic_IReadOnlyCollection_T
				Case SpecialType.System_Nullable_T
					Throw ExceptionUtilities.UnexpectedValue(this)
				Case SpecialType.System_Void
					type = GetType(Void)
					Exit Select
				Case SpecialType.System_Boolean
					type = GetType(Boolean)
					Exit Select
				Case SpecialType.System_Char
					type = GetType(Char)
					Exit Select
				Case SpecialType.System_SByte
					type = GetType(SByte)
					Exit Select
				Case SpecialType.System_Byte
					type = GetType(Byte)
					Exit Select
				Case SpecialType.System_Int16
					type = GetType(Int16)
					Exit Select
				Case SpecialType.System_UInt16
					type = GetType(UInt16)
					Exit Select
				Case SpecialType.System_Int32
					type = GetType(Int32)
					Exit Select
				Case SpecialType.System_UInt32
					type = GetType(UInt32)
					Exit Select
				Case SpecialType.System_Int64
					type = GetType(Int64)
					Exit Select
				Case SpecialType.System_UInt64
					type = GetType(UInt64)
					Exit Select
				Case SpecialType.System_Decimal
					type = GetType(Decimal)
					Exit Select
				Case SpecialType.System_Single
					type = GetType(Single)
					Exit Select
				Case SpecialType.System_Double
					type = GetType(Double)
					Exit Select
				Case SpecialType.System_String
					type = GetType(String)
					Exit Select
				Case SpecialType.System_DateTime
					type = GetType(DateTime)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(this)
			End Select
			Return type
		End Function

		<Extension>
		Public Function TryGetKeywordText(ByVal this As SpecialType) As String
			Dim str As String
			Select Case this
				Case SpecialType.System_Object
					str = "Object"
					Exit Select
				Case SpecialType.System_Enum
				Case SpecialType.System_MulticastDelegate
				Case SpecialType.System_Delegate
				Case SpecialType.System_ValueType
				Case SpecialType.System_IntPtr
				Case SpecialType.System_UIntPtr
				Case SpecialType.System_Array
				Case SpecialType.System_Collections_IEnumerable
				Case SpecialType.System_Collections_Generic_IEnumerable_T
				Case SpecialType.System_Collections_Generic_IList_T
				Case SpecialType.System_Collections_Generic_ICollection_T
				Case SpecialType.System_Collections_IEnumerator
				Case SpecialType.System_Collections_Generic_IEnumerator_T
				Case SpecialType.System_Collections_Generic_IReadOnlyList_T
				Case SpecialType.System_Collections_Generic_IReadOnlyCollection_T
				Case SpecialType.System_Nullable_T
				Label0:
					str = Nothing
					Exit Select
				Case SpecialType.System_Void
					str = "Void"
					Exit Select
				Case SpecialType.System_Boolean
					str = "Boolean"
					Exit Select
				Case SpecialType.System_Char
					str = "Char"
					Exit Select
				Case SpecialType.System_SByte
					str = "SByte"
					Exit Select
				Case SpecialType.System_Byte
					str = "Byte"
					Exit Select
				Case SpecialType.System_Int16
					str = "Short"
					Exit Select
				Case SpecialType.System_UInt16
					str = "UShort"
					Exit Select
				Case SpecialType.System_Int32
					str = "Integer"
					Exit Select
				Case SpecialType.System_UInt32
					str = "UInteger"
					Exit Select
				Case SpecialType.System_Int64
					str = "Long"
					Exit Select
				Case SpecialType.System_UInt64
					str = "ULong"
					Exit Select
				Case SpecialType.System_Decimal
					str = "Decimal"
					Exit Select
				Case SpecialType.System_Single
					str = "Single"
					Exit Select
				Case SpecialType.System_Double
					str = "Double"
					Exit Select
				Case SpecialType.System_String
					str = "String"
					Exit Select
				Case SpecialType.System_DateTime
					str = "Date"
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return str
		End Function

		<Extension>
		Public Function TypeToIndex(ByVal type As Microsoft.CodeAnalysis.SpecialType) As Nullable(Of Integer)
			Dim nullable As Nullable(Of Integer)
			Dim num As Integer
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = type
			Select Case specialType
				Case Microsoft.CodeAnalysis.SpecialType.System_Object
					num = 0
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Enum
				Case Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate
				Case Microsoft.CodeAnalysis.SpecialType.System_Delegate
				Case Microsoft.CodeAnalysis.SpecialType.System_ValueType
				Case Microsoft.CodeAnalysis.SpecialType.System_Void
					nullable = Nothing
					Return nullable
				Case Microsoft.CodeAnalysis.SpecialType.System_Boolean
					num = 2
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Char
					num = 3
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_SByte
					num = 4
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Byte
					num = 8
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Int16
					num = 5
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_UInt16
					num = 9
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Int32
					num = 6
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_UInt32
					num = 10
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Int64
					num = 7
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_UInt64
					num = 11
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
					num = 14
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Single
					num = 12
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Double
					num = 13
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_String
					num = 1
					Exit Select
				Case Else
					If (specialType = Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
						num = 15
						Exit Select
					Else
						nullable = Nothing
						Return nullable
					End If
			End Select
			nullable = New Nullable(Of Integer)(num)
			Return nullable
		End Function
	End Module
End Namespace