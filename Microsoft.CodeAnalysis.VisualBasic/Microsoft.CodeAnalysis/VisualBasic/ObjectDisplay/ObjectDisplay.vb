Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text

Namespace Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay
	Friend Module ObjectDisplay
		Private Const s_nullChar As Char = Strings.ChrW(0)

		Private Const s_back As Char = Strings.ChrW(8)

		Private Const s_Cr As Char = Strings.ChrW(13)

		Private Const s_formFeed As Char = Strings.ChrW(12)

		Private Const s_Lf As Char = Strings.ChrW(10)

		Private Const s_tab As Char = Strings.ChrW(9)

		Private Const s_verticalTab As Char = Strings.ChrW(11)

		Friend ReadOnly Property NullLiteral As String
			Get
				Return "Nothing"
			End Get
		End Property

		Private Function Character(ByVal c As Char) As Integer
			Return 851968 Or c
		End Function

		Private Function EscapeQuote(ByVal c As Char) As String
			If (EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(c), """", False) = 0) Then
				Return """"""
			End If
			Return Microsoft.VisualBasic.CompilerServices.Conversions.ToString(c)
		End Function

		Friend Function FormatLiteral(ByVal value As Boolean) As String
			If (Not value) Then
				Return "False"
			End If
			Return "True"
		End Function

		Friend Function FormatLiteral(ByVal value As String, ByVal options As ObjectDisplayOptions) As String
			Dim enumerator As IEnumerator(Of Integer) = Nothing
			If (value Is Nothing) Then
				Throw New ArgumentNullException()
			End If
			Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
			Using builder As StringBuilder = instance.Builder
				enumerator = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.TokenizeString(value, options).GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Integer = enumerator.Current
					builder.Append(Strings.ChrW(current And 65535))
				End While
			End Using
			Return instance.ToStringAndFree()
		End Function

		Friend Function FormatLiteral(ByVal c As Char, ByVal options As ObjectDisplayOptions) As String
			Dim str As String
			If (Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.IsPrintable(c) OrElse Not options.IncludesOption(ObjectDisplayOptions.EscapeNonPrintableCharacters)) Then
				str = If(options.IncludesOption(ObjectDisplayOptions.UseQuotes), [String].Concat("""", Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.EscapeQuote(c), """c"), c.ToString())
			Else
				Dim wellKnownCharacterName As String = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.GetWellKnownCharacterName(c)
				If (wellKnownCharacterName Is Nothing) Then
					Dim num As Integer = c
					str = [String].Concat(If(options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers), [String].Concat("ChrW(&H", num.ToString("X")), [String].Concat("ChrW(", num.ToString())), ")")
				Else
					str = wellKnownCharacterName
				End If
			End If
			Return str
		End Function

		Friend Function FormatLiteral(ByVal value As SByte, ByVal options As ObjectDisplayOptions, Optional ByVal cultureInfo As System.Globalization.CultureInfo = Nothing) As String
			Dim str As String
			str = If(Not options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers), value.ToString(Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.GetFormatCulture(cultureInfo)), [String].Concat("&H", If(value >= 0, value.ToString("X2"), value.ToString("X8"))))
			Return str
		End Function

		Friend Function FormatLiteral(ByVal value As Byte, ByVal options As ObjectDisplayOptions, Optional ByVal cultureInfo As System.Globalization.CultureInfo = Nothing) As String
			Dim str As String
			str = If(Not options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers), value.ToString(Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.GetFormatCulture(cultureInfo)), [String].Concat("&H", value.ToString("X2")))
			Return str
		End Function

		Friend Function FormatLiteral(ByVal value As Short, ByVal options As ObjectDisplayOptions, Optional ByVal cultureInfo As System.Globalization.CultureInfo = Nothing) As String
			Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
			Dim builder As StringBuilder = instance.Builder
			If (Not options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers)) Then
				builder.Append(value.ToString(Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.GetFormatCulture(cultureInfo)))
			Else
				builder.Append("&H")
				builder.Append(If(value >= 0, value.ToString("X4"), value.ToString("X8")))
			End If
			If (options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix)) Then
				builder.Append("S"C)
			End If
			Return instance.ToStringAndFree()
		End Function

		Friend Function FormatLiteral(ByVal value As UShort, ByVal options As ObjectDisplayOptions, Optional ByVal cultureInfo As System.Globalization.CultureInfo = Nothing) As String
			Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
			Dim builder As StringBuilder = instance.Builder
			If (Not options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers)) Then
				builder.Append(value.ToString(Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.GetFormatCulture(cultureInfo)))
			Else
				builder.Append("&H")
				builder.Append(value.ToString("X4"))
			End If
			If (options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix)) Then
				builder.Append("US")
			End If
			Return instance.ToStringAndFree()
		End Function

		Friend Function FormatLiteral(ByVal value As Integer, ByVal options As ObjectDisplayOptions, Optional ByVal cultureInfo As System.Globalization.CultureInfo = Nothing) As String
			Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
			Dim builder As StringBuilder = instance.Builder
			If (Not options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers)) Then
				builder.Append(value.ToString(Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.GetFormatCulture(cultureInfo)))
			Else
				builder.Append("&H")
				builder.Append(value.ToString("X8"))
			End If
			If (options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix)) Then
				builder.Append("I"C)
			End If
			Return instance.ToStringAndFree()
		End Function

		Friend Function FormatLiteral(ByVal value As UInteger, ByVal options As ObjectDisplayOptions, Optional ByVal cultureInfo As System.Globalization.CultureInfo = Nothing) As String
			Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
			Dim builder As StringBuilder = instance.Builder
			If (Not options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers)) Then
				builder.Append(value.ToString(Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.GetFormatCulture(cultureInfo)))
			Else
				builder.Append("&H")
				builder.Append(value.ToString("X8"))
			End If
			If (options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix)) Then
				builder.Append("UI")
			End If
			Return instance.ToStringAndFree()
		End Function

		Friend Function FormatLiteral(ByVal value As Long, ByVal options As ObjectDisplayOptions, Optional ByVal cultureInfo As System.Globalization.CultureInfo = Nothing) As String
			Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
			Dim builder As StringBuilder = instance.Builder
			If (Not options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers)) Then
				builder.Append(value.ToString(Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.GetFormatCulture(cultureInfo)))
			Else
				builder.Append("&H")
				builder.Append(value.ToString("X16"))
			End If
			If (options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix)) Then
				builder.Append("L"C)
			End If
			Return instance.ToStringAndFree()
		End Function

		Friend Function FormatLiteral(ByVal value As ULong, ByVal options As ObjectDisplayOptions, Optional ByVal cultureInfo As System.Globalization.CultureInfo = Nothing) As String
			Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
			Dim builder As StringBuilder = instance.Builder
			If (Not options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers)) Then
				builder.Append(value.ToString(Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.GetFormatCulture(cultureInfo)))
			Else
				builder.Append("&H")
				builder.Append(value.ToString("X16"))
			End If
			If (options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix)) Then
				builder.Append("UL")
			End If
			Return instance.ToStringAndFree()
		End Function

		Friend Function FormatLiteral(ByVal value As Double, ByVal options As ObjectDisplayOptions, Optional ByVal cultureInfo As System.Globalization.CultureInfo = Nothing) As String
			Dim str As String = value.ToString("R", Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.GetFormatCulture(cultureInfo))
			If (Not options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix)) Then
				Return str
			End If
			Return [String].Concat(str, "R")
		End Function

		Friend Function FormatLiteral(ByVal value As Single, ByVal options As ObjectDisplayOptions, Optional ByVal cultureInfo As System.Globalization.CultureInfo = Nothing) As String
			Dim str As String = value.ToString("R", Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.GetFormatCulture(cultureInfo))
			If (Not options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix)) Then
				Return str
			End If
			Return [String].Concat(str, "F")
		End Function

		Friend Function FormatLiteral(ByVal value As [Decimal], ByVal options As ObjectDisplayOptions, Optional ByVal cultureInfo As System.Globalization.CultureInfo = Nothing) As String
			Dim str As String = value.ToString(Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.GetFormatCulture(cultureInfo))
			If (Not options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix)) Then
				Return str
			End If
			Return [String].Concat(str, "D")
		End Function

		Friend Function FormatLiteral(ByVal value As DateTime) As String
			Return value.ToString("#M/d/yyyy hh:mm:ss tt#", CultureInfo.InvariantCulture)
		End Function

		Public Function FormatPrimitive(ByVal obj As Object, ByVal options As ObjectDisplayOptions) As String
			Dim nullLiteral As String
			If (obj IsNot Nothing) Then
				Dim type As System.Type = obj.[GetType]()
				If (type.GetTypeInfo().IsEnum) Then
					type = [Enum].GetUnderlyingType(type)
				End If
				If (CObj(type) = CObj(GetType(Int32))) Then
					nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatLiteral(CInt(obj), options, Nothing)
				ElseIf (CObj(type) = CObj(GetType(String))) Then
					nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatLiteral(CStr(obj), options)
				ElseIf (CObj(type) = CObj(GetType(Boolean))) Then
					nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatLiteral(CBool(obj))
				ElseIf (CObj(type) = CObj(GetType(Char))) Then
					nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatLiteral(CChar(obj), options)
				ElseIf (CObj(type) = CObj(GetType(Byte))) Then
					nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatLiteral(CByte(obj), options, Nothing)
				ElseIf (CObj(type) = CObj(GetType(Int16))) Then
					nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatLiteral(CShort(obj), options, Nothing)
				ElseIf (CObj(type) = CObj(GetType(Int64))) Then
					nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatLiteral(CLng(obj), options, Nothing)
				ElseIf (CObj(type) = CObj(GetType(Double))) Then
					nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatLiteral(CDbl(obj), options, Nothing)
				ElseIf (CObj(type) = CObj(GetType(UInt64))) Then
					nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatLiteral(CULng(obj), options, Nothing)
				ElseIf (CObj(type) = CObj(GetType(UInt32))) Then
					nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatLiteral(CUInt(obj), options, Nothing)
				ElseIf (CObj(type) = CObj(GetType(UInt16))) Then
					nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatLiteral(CUShort(obj), options, Nothing)
				ElseIf (CObj(type) = CObj(GetType(SByte))) Then
					nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatLiteral(CSByte(obj), options, Nothing)
				ElseIf (CObj(type) = CObj(GetType(Single))) Then
					nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatLiteral(CSng(obj), options, Nothing)
				ElseIf (CObj(type) = CObj(GetType(Decimal))) Then
					nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatLiteral(CDec(obj), options, Nothing)
				ElseIf (CObj(type) <> CObj(GetType(DateTime))) Then
					nullLiteral = Nothing
				Else
					nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatLiteral(DirectCast(obj, DateTime))
				End If
			Else
				nullLiteral = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.NullLiteral
			End If
			Return nullLiteral
		End Function

		Private Function GetFormatCulture(ByVal cultureInfo As System.Globalization.CultureInfo) As System.Globalization.CultureInfo
			Return If(cultureInfo, System.Globalization.CultureInfo.InvariantCulture)
		End Function

		Friend Function GetWellKnownCharacterName(ByVal c As Char) As String
			Dim str As String
			Dim chr As Char = c
			If (chr = 0) Then
				str = "vbNullChar"
			Else
				Select Case chr
					Case Strings.ChrW(8)
						str = "vbBack"
						Exit Select
					Case Strings.ChrW(9)
						str = "vbTab"
						Exit Select
					Case Strings.ChrW(10)
						str = "vbLf"
						Exit Select
					Case Strings.ChrW(11)
						str = "vbVerticalTab"
						Exit Select
					Case Strings.ChrW(12)
						str = "vbFormFeed"
						Exit Select
					Case Strings.ChrW(13)
						str = "vbCr"
						Exit Select
					Case Else
						str = Nothing
						Exit Select
				End Select
			End If
			Return str
		End Function

		Private Function Identifier(ByVal c As Char) As Integer
			Return 983040 Or c
		End Function

		Friend Function IsPrintable(ByVal c As Char) As Boolean
			Return Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.IsPrintable(CharUnicodeInfo.GetUnicodeCategory(c))
		End Function

		Private Function IsPrintable(ByVal category As System.Globalization.UnicodeCategory) As Boolean
			Dim flag As Boolean
			Dim unicodeCategory As System.Globalization.UnicodeCategory = category
			flag = If(CInt(unicodeCategory) - CInt(System.Globalization.UnicodeCategory.LineSeparator) <= CInt(System.Globalization.UnicodeCategory.TitlecaseLetter) OrElse unicodeCategory = System.Globalization.UnicodeCategory.Surrogate OrElse unicodeCategory = System.Globalization.UnicodeCategory.OtherNotAssigned, False, True)
			Return flag
		End Function

		Private Function Number(ByVal c As Char) As Integer
			Return 786432 Or c
		End Function

		Private Function [Operator](ByVal c As Char) As Integer
			Return 1179648 Or c
		End Function

		Private Function Punctuation(ByVal c As Char) As Integer
			Return 1376256 Or c
		End Function

		Private Function Quotes() As Integer
			Return 852002
		End Function

		Private Function Space() As Integer
			Return 1441824
		End Function

		Friend Function TokenizeString(ByVal str As String, ByVal options As ObjectDisplayOptions) As IEnumerable(Of Integer)
			Return New Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.VB$StateMachine_33_TokenizeString(-2) With
			{
				.$P_str = str,
				.$P_options = options
			}
		End Function

		<Conditional("DEBUG")>
		Private Sub ValidateOptions(ByVal options As ObjectDisplayOptions)
		End Sub
	End Module
End Namespace