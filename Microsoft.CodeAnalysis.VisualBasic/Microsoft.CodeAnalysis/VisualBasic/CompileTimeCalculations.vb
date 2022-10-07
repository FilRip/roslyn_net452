Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module CompileTimeCalculations
		Friend Function AdjustConstantValueFromMetadata(ByVal value As Microsoft.CodeAnalysis.ConstantValue, ByVal targetType As TypeSymbol, ByVal isByRefParamValue As Boolean) As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			If (targetType Is Nothing OrElse targetType.IsErrorType()) Then
				constantValue = value
			Else
				Dim discriminator As ConstantValueTypeDiscriminator = value.Discriminator
				If (discriminator <> ConstantValueTypeDiscriminator.Int32) Then
					If (discriminator = ConstantValueTypeDiscriminator.Int64) Then
						If (targetType.IsDateTimeType()) Then
							value = Microsoft.CodeAnalysis.ConstantValue.Create(New DateTime(value.Int64Value))
						End If
					End If
				ElseIf (Not targetType.IsIntrinsicType() AndAlso Not targetType.IsEnumType() AndAlso Not targetType.IsObjectType() AndAlso (Not targetType.IsNullableType() OrElse Not targetType.GetNullableUnderlyingType().IsIntrinsicType()) AndAlso (isByRefParamValue OrElse Not targetType.IsTypeParameter() AndAlso Not targetType.IsArrayType() AndAlso targetType.IsReferenceType)) Then
					If (value.Int32Value <> 0) Then
						value = Microsoft.CodeAnalysis.ConstantValue.Bad
					Else
						value = Microsoft.CodeAnalysis.ConstantValue.[Nothing]
					End If
				End If
				constantValue = value
			End If
			Return constantValue
		End Function

		Friend Function ConvertDecimalValue(ByVal sourceValue As [Decimal], ByVal targetType As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator, ByRef integerOverflow As Boolean) As ConstantValue
			Dim bad As ConstantValue
			Dim flag As Boolean
			Dim num As Byte
			Dim num1 As UInteger
			Dim num2 As UInteger
			Dim num3 As UInteger
			Dim flag1 As Boolean = False
			If (ConstantValue.IsIntegralType(targetType) OrElse ConstantValue.IsCharType(targetType)) Then
				sourceValue.GetBits(flag, num, num1, num2, num3)
				If (num <> 0) Then
					bad = CompileTimeCalculations.ConvertFloatingValue([Decimal].ToDouble(sourceValue), targetType, integerOverflow)
					Return bad
				Else
					flag1 = CULng(num3) <> CLng(0)
					If (Not flag1) Then
						Dim num4 As Long = ' 
						' Current member / type: Microsoft.CodeAnalysis.ConstantValue Microsoft.CodeAnalysis.VisualBasic.CompileTimeCalculations::ConvertDecimalValue(System.Decimal,Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator,System.Boolean&)
						' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
						' 
						' Product version: 2019.1.118.0
						' Exception in: Microsoft.CodeAnalysis.ConstantValue ConvertDecimalValue(System.Decimal,Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator,System.Boolean&)
						' 
						' La référence d'objet n'est pas définie à une instance d'un objet.
						'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
						'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
						'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 827
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 822
						'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 998
						'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2183
						'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 84
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
						'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
						'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(IfStatement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 108
						'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 78
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
						'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
						'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(IfStatement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 119
						'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 78
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
						'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
						'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(IfStatement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 108
						'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 78
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
						'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
						'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1916
						'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1841
						'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 447
						' 
						' mailto: JustDecompilePublicFeedback@telerik.com


		Private Function ConvertFloatingToUI64(ByVal sourceValue As Double) As Long
			Dim num As Long
			Dim num1 As Double = 9.22337203685478E+18
			num = If(sourceValue >= num1, CompileTimeCalculations.UncheckedAdd(CompileTimeCalculations.UncheckedCLng(sourceValue - num1), -9223372036854775808L), CompileTimeCalculations.UncheckedCLng(sourceValue))
			Return num
		End Function

		Friend Function ConvertFloatingValue(ByVal sourceValue As Double, ByVal targetType As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator, ByRef integerOverflow As Boolean) As ConstantValue
			Dim bad As ConstantValue
			Dim num As Long
			Dim constantValueTypeDiscriminator As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator
			Dim num1 As [Decimal] = New Decimal()
			Dim integralOverflow As Boolean = False
			If (Not ConstantValue.IsBooleanType(targetType)) Then
				If (ConstantValue.IsIntegralType(targetType) OrElse ConstantValue.IsCharType(targetType)) Then
					integralOverflow = CompileTimeCalculations.DetectFloatingToIntegralOverflow(sourceValue, ConstantValue.IsUnsignedIntegralType(targetType))
					If (integralOverflow) Then
						GoTo Label1
					End If
					Dim num2 As Double = sourceValue + 0.5
					Dim num3 As Double = Math.Floor(num2)
					If (num3 <> num2 OrElse Math.IEEERemainder(num2, 2) = 0) Then
						num = If(CompileTimeCalculations.IsUnsignedLongType(targetType), CompileTimeCalculations.ConvertFloatingToUI64(num3), CompileTimeCalculations.UncheckedCLng(num3))
					Else
						num = If(CompileTimeCalculations.IsUnsignedLongType(targetType), CompileTimeCalculations.ConvertFloatingToUI64(num3 - 1), CompileTimeCalculations.UncheckedCLng(num3 - 1))
					End If
					constantValueTypeDiscriminator = If(sourceValue >= 0, Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.UInt64, Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.Int64)
					bad = CompileTimeCalculations.ConvertIntegralValue(num, constantValueTypeDiscriminator, targetType, integerOverflow)
					Return bad
				End If
			Label1:
				If (Not ConstantValue.IsFloatingType(targetType)) Then
					If (ConstantValue.IsDecimalType(targetType)) Then
						Try
							num1 = Convert.ToDecimal(sourceValue)
						Catch overflowException As System.OverflowException
							ProjectData.SetProjectError(overflowException)
							integralOverflow = True
							ProjectData.ClearProjectError()
						End Try
						If (integralOverflow) Then
							GoTo Label2
						End If
						bad = CompileTimeCalculations.ConvertDecimalValue(num1, targetType, integerOverflow)
						Return bad
					End If
				Label2:
					If (Not integralOverflow) Then
						Throw ExceptionUtilities.Unreachable
					End If
					bad = ConstantValue.Bad
				Else
					Dim num4 As Double = CompileTimeCalculations.NarrowFloatingResult(sourceValue, targetType, integralOverflow)
					bad = If(targetType <> Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Single], ConstantValue.Create(num4), ConstantValue.Create(CSng(num4)))
				End If
			Else
				bad = CompileTimeCalculations.ConvertIntegralValue(CLng(If(sourceValue = 0, 0, 1)), Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.Int64, targetType, integerOverflow)
			End If
			Return bad
		End Function

		Friend Function ConvertIntegralValue(ByVal sourceValue As Long, ByVal sourceType As ConstantValueTypeDiscriminator, ByVal targetType As ConstantValueTypeDiscriminator, ByRef integerOverflow As Boolean) As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			Dim flag As Boolean
			If (Microsoft.CodeAnalysis.ConstantValue.IsIntegralType(targetType) OrElse Microsoft.CodeAnalysis.ConstantValue.IsBooleanType(targetType) OrElse Microsoft.CodeAnalysis.ConstantValue.IsCharType(targetType)) Then
				constantValue = CompileTimeCalculations.GetConstantValue(targetType, CompileTimeCalculations.NarrowIntegralResult(sourceValue, sourceType, targetType, integerOverflow))
			ElseIf (Microsoft.CodeAnalysis.ConstantValue.IsStringType(targetType) AndAlso Microsoft.CodeAnalysis.ConstantValue.IsCharType(sourceType)) Then
				constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(New [String](Strings.ChrW(CompileTimeCalculations.UncheckedCInt(sourceValue)), 1))
			ElseIf (Not Microsoft.CodeAnalysis.ConstantValue.IsFloatingType(targetType)) Then
				If (Not Microsoft.CodeAnalysis.ConstantValue.IsDecimalType(targetType)) Then
					Throw ExceptionUtilities.Unreachable
				End If
				If (Microsoft.CodeAnalysis.ConstantValue.IsUnsignedIntegralType(sourceType) OrElse sourceValue >= CLng(0)) Then
					flag = False
				Else
					flag = True
					sourceValue = -sourceValue
				End If
				Dim num As Integer = CompileTimeCalculations.UncheckedCInt(sourceValue And CLng(-1))
				Dim num1 As Integer = CompileTimeCalculations.UncheckedCInt((sourceValue And -4294967296L) >> 32)
				Dim num2 As Integer = 0
				Dim num3 As Byte = 0
				constantValue = CompileTimeCalculations.ConvertDecimalValue(New [Decimal](num, num1, num2, flag, num3), targetType, integerOverflow)
			Else
				constantValue = CompileTimeCalculations.ConvertFloatingValue(If(Microsoft.CodeAnalysis.ConstantValue.IsUnsignedIntegralType(sourceType), CDbl(CSng(CompileTimeCalculations.UncheckedCULng(sourceValue))), CDbl(sourceValue)), targetType, integerOverflow)
			End If
			Return constantValue
		End Function

		Private Function DetectFloatingToIntegralOverflow(ByVal sourceValue As Double, ByVal isUnsigned As Boolean) As Boolean
			Dim flag As Boolean
			If (isUnsigned) Then
				If (sourceValue >= 1.72938225691027E+19) Then
					Dim num As Double = sourceValue - 1.72938225691027E+19
					If (num >= 8.07045053224793E+18 OrElse CompileTimeCalculations.UncheckedCLng(num) >= 1152921504606846976L) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (sourceValue <= -1) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (sourceValue < -8.07045053224793E+18) Then
				Dim num1 As Double = sourceValue - -8.07045053224793E+18
				If (num1 <= -8.07045053224793E+18 OrElse CompileTimeCalculations.UncheckedCLng(num1) <= -1152921504606846977L) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (sourceValue <= 8.07045053224793E+18) Then
				flag = False
				Return flag
			Else
				Dim num2 As Double = sourceValue - 8.07045053224793E+18
				If (num2 >= 8.07045053224793E+18 OrElse CompileTimeCalculations.UncheckedCLng(num2) <= 1152921504606846976L) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Friend Function GetConstantValue(ByVal type As ConstantValueTypeDiscriminator, ByVal value As Long) As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			Select Case type
				Case ConstantValueTypeDiscriminator.[SByte]
					constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(CompileTimeCalculations.UncheckedCSByte(value))
					Exit Select
				Case ConstantValueTypeDiscriminator.[Byte]
					constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(CompileTimeCalculations.UncheckedCByte(value))
					Exit Select
				Case ConstantValueTypeDiscriminator.Int16
					constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(CompileTimeCalculations.UncheckedCShort(value))
					Exit Select
				Case ConstantValueTypeDiscriminator.UInt16
					constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(CompileTimeCalculations.UncheckedCUShort(value))
					Exit Select
				Case ConstantValueTypeDiscriminator.Int32
					constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(CompileTimeCalculations.UncheckedCInt(value))
					Exit Select
				Case ConstantValueTypeDiscriminator.UInt32
					constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(CompileTimeCalculations.UncheckedCUInt(value))
					Exit Select
				Case ConstantValueTypeDiscriminator.Int64
					constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(value)
					Exit Select
				Case ConstantValueTypeDiscriminator.UInt64
					constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(CompileTimeCalculations.UncheckedCULng(value))
					Exit Select
				Case ConstantValueTypeDiscriminator.NInt
				Case ConstantValueTypeDiscriminator.NUInt
				Case ConstantValueTypeDiscriminator.[Single]
				Case ConstantValueTypeDiscriminator.[Double]
				Case ConstantValueTypeDiscriminator.[String]
				Case ConstantValueTypeDiscriminator.[Decimal]
					Throw ExceptionUtilities.UnexpectedValue(type)
				Case ConstantValueTypeDiscriminator.[Char]
					constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(Strings.ChrW(CompileTimeCalculations.UncheckedCInt(value)))
					Exit Select
				Case ConstantValueTypeDiscriminator.[Boolean]
					constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(If(value = 0, False, True))
					Exit Select
				Case ConstantValueTypeDiscriminator.DateTime
					constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(New DateTime(value))
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(type)
			End Select
			Return constantValue
		End Function

		Friend Function GetConstantValueAsInt64(ByRef value As ConstantValue) As Long
			Dim sByteValue As Long
			Select Case value.Discriminator
				Case ConstantValueTypeDiscriminator.[SByte]
					sByteValue = CLng(value.SByteValue)
					Exit Select
				Case ConstantValueTypeDiscriminator.[Byte]
					sByteValue = CLng(value.ByteValue)
					Exit Select
				Case ConstantValueTypeDiscriminator.Int16
					sByteValue = CLng(value.Int16Value)
					Exit Select
				Case ConstantValueTypeDiscriminator.UInt16
					sByteValue = CLng(value.UInt16Value)
					Exit Select
				Case ConstantValueTypeDiscriminator.Int32
					sByteValue = CLng(value.Int32Value)
					Exit Select
				Case ConstantValueTypeDiscriminator.UInt32
					sByteValue = CLng(value.UInt32Value)
					Exit Select
				Case ConstantValueTypeDiscriminator.Int64
					sByteValue = value.Int64Value
					Exit Select
				Case ConstantValueTypeDiscriminator.UInt64
					sByteValue = CompileTimeCalculations.UncheckedCLng(value.UInt64Value)
					Exit Select
				Case ConstantValueTypeDiscriminator.NInt
				Case ConstantValueTypeDiscriminator.NUInt
				Case ConstantValueTypeDiscriminator.[Single]
				Case ConstantValueTypeDiscriminator.[Double]
				Case ConstantValueTypeDiscriminator.[String]
				Case ConstantValueTypeDiscriminator.[Decimal]
					Throw ExceptionUtilities.UnexpectedValue(value.Discriminator)
				Case ConstantValueTypeDiscriminator.[Char]
					sByteValue = CLng(value.CharValue)
					Exit Select
				Case ConstantValueTypeDiscriminator.[Boolean]
					sByteValue = CLng(If(value.BooleanValue, 1, 0))
					Exit Select
				Case ConstantValueTypeDiscriminator.DateTime
					sByteValue = value.DateTimeValue.Ticks
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(value.Discriminator)
			End Select
			Return sByteValue
		End Function

		Private Function IsUnsignedLongType(ByVal type As ConstantValueTypeDiscriminator) As Boolean
			Return type = ConstantValueTypeDiscriminator.UInt64
		End Function

		Friend Function Multiply(ByVal leftValue As Long, ByVal rightValue As Long, ByVal sourceType As SpecialType, ByVal resultType As SpecialType, ByRef integerOverflow As Boolean) As Long
			Return CompileTimeCalculations.Multiply(leftValue, rightValue, sourceType.ToConstantValueDiscriminator(), resultType.ToConstantValueDiscriminator(), integerOverflow)
		End Function

		Friend Function Multiply(ByVal leftValue As Long, ByVal rightValue As Long, ByVal sourceType As ConstantValueTypeDiscriminator, ByVal resultType As ConstantValueTypeDiscriminator, ByRef integerOverflow As Boolean) As Long
			Dim num As Long = CompileTimeCalculations.NarrowIntegralResult(CompileTimeCalculations.UncheckedMul(leftValue, rightValue), sourceType, resultType, integerOverflow)
			If (ConstantValue.IsUnsignedIntegralType(resultType)) Then
				If (rightValue <> 0 AndAlso CDbl(CSng(CompileTimeCalculations.UncheckedCULng(num))) / CDbl(CSng(CompileTimeCalculations.UncheckedCULng(rightValue))) <> CDbl(CSng(CompileTimeCalculations.UncheckedCULng(leftValue)))) Then
					integerOverflow = True
				End If
			ElseIf (leftValue > CLng(0) AndAlso rightValue > CLng(0) AndAlso num <= CLng(0) OrElse leftValue < CLng(0) AndAlso rightValue < CLng(0) AndAlso num <= CLng(0) OrElse leftValue > CLng(0) AndAlso rightValue < CLng(0) AndAlso num >= CLng(0) OrElse leftValue < CLng(0) AndAlso rightValue > CLng(0) AndAlso num >= CLng(0) OrElse rightValue <> 0 AndAlso CDbl(num) / CDbl(rightValue) <> CDbl(leftValue)) Then
				integerOverflow = True
			End If
			Return num
		End Function

		Friend Function NarrowFloatingResult(ByVal value As Double, ByVal resultType As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator, ByRef overflow As Boolean) As Double
			Dim num As Double
			If ([Double].IsNaN(value)) Then
				overflow = True
			End If
			Dim constantValueTypeDiscriminator As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator = resultType
			If (constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Single]) Then
				If (value > 3.40282346638529E+38 OrElse value < -3.40282346638529E+38) Then
					overflow = True
				End If
				num = CDbl(CSng(value))
			Else
				If (constantValueTypeDiscriminator <> Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Double]) Then
					Throw ExceptionUtilities.UnexpectedValue(resultType)
				End If
				num = value
			End If
			Return num
		End Function

		Friend Function NarrowFloatingResult(ByVal value As Double, ByVal resultType As Microsoft.CodeAnalysis.SpecialType, ByRef overflow As Boolean) As Double
			Dim num As Double
			If ([Double].IsNaN(value)) Then
				overflow = True
			End If
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = resultType
			If (specialType = Microsoft.CodeAnalysis.SpecialType.System_Single) Then
				If (value > 3.40282346638529E+38 OrElse value < -3.40282346638529E+38) Then
					overflow = True
				End If
				num = CDbl(CSng(value))
			Else
				If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_Double) Then
					Throw ExceptionUtilities.UnexpectedValue(resultType)
				End If
				num = value
			End If
			Return num
		End Function

		Friend Function NarrowIntegralResult(ByVal sourceValue As Long, ByVal sourceType As ConstantValueTypeDiscriminator, ByVal resultType As ConstantValueTypeDiscriminator, ByRef overflow As Boolean) As Long
			Dim num As Long
			Dim num1 As Long = CLng(0)
			Select Case resultType
				Case ConstantValueTypeDiscriminator.[SByte]
					num1 = CLng(CompileTimeCalculations.UncheckedCSByte(sourceValue))
					Exit Select
				Case ConstantValueTypeDiscriminator.[Byte]
					num1 = CLng(CompileTimeCalculations.UncheckedCByte(sourceValue))
					Exit Select
				Case ConstantValueTypeDiscriminator.Int16
					num1 = CLng(CompileTimeCalculations.UncheckedCShort(sourceValue))
					Exit Select
				Case ConstantValueTypeDiscriminator.UInt16
					num1 = CLng(CompileTimeCalculations.UncheckedCUShort(sourceValue))
					Exit Select
				Case ConstantValueTypeDiscriminator.Int32
					num1 = CLng(CompileTimeCalculations.UncheckedCInt(sourceValue))
					Exit Select
				Case ConstantValueTypeDiscriminator.UInt32
					num1 = CLng(CompileTimeCalculations.UncheckedCUInt(sourceValue))
					Exit Select
				Case ConstantValueTypeDiscriminator.Int64
					num1 = sourceValue
					Exit Select
				Case ConstantValueTypeDiscriminator.UInt64
					num1 = sourceValue
					Exit Select
				Case ConstantValueTypeDiscriminator.NInt
				Case ConstantValueTypeDiscriminator.NUInt
					Throw ExceptionUtilities.UnexpectedValue(resultType)
				Case ConstantValueTypeDiscriminator.[Char]
					num1 = CLng(CompileTimeCalculations.UncheckedCUShort(sourceValue))
					Exit Select
				Case ConstantValueTypeDiscriminator.[Boolean]
					num1 = CLng(If(sourceValue = 0, 0, 1))
					num = num1
					Return num
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(resultType)
			End Select
			If (Not ConstantValue.IsBooleanType(sourceType) AndAlso ConstantValue.IsUnsignedIntegralType(sourceType) Xor ConstantValue.IsUnsignedIntegralType(resultType)) Then
				If (Not ConstantValue.IsUnsignedIntegralType(sourceType)) Then
					If (sourceValue < CLng(0)) Then
						overflow = True
					End If
				ElseIf (num1 < CLng(0)) Then
					overflow = True
				End If
			End If
			If (num1 <> sourceValue) Then
				overflow = True
			End If
			num = num1
			Return num
		End Function

		Friend Function NarrowIntegralResult(ByVal sourceValue As Long, ByVal sourceType As SpecialType, ByVal resultType As SpecialType, ByRef overflow As Boolean) As Long
			Return CompileTimeCalculations.NarrowIntegralResult(sourceValue, sourceType.ToConstantValueDiscriminator(), resultType.ToConstantValueDiscriminator(), overflow)
		End Function

		Friend Function NarrowIntegralResult(ByVal sourceValue As Long, ByVal sourceType As TypeSymbol, ByVal resultType As TypeSymbol, ByRef overflow As Boolean) As Long
			Return CompileTimeCalculations.NarrowIntegralResult(sourceValue, sourceType.GetConstantValueTypeDiscriminator(), resultType.GetConstantValueTypeDiscriminator(), overflow)
		End Function

		Friend Function TypeAllowsCompileTimeConversions(ByVal type As ConstantValueTypeDiscriminator) As Boolean
			Return CompileTimeCalculations.TypeAllowsCompileTimeOperations(type)
		End Function

		Friend Function TypeAllowsCompileTimeOperations(ByVal type As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator) As Boolean
			Dim flag As Boolean
			Dim constantValueTypeDiscriminator As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator = type
			flag = If(CByte(constantValueTypeDiscriminator) - CByte(Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[SByte]) <= CByte(Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.UInt32) OrElse CByte(constantValueTypeDiscriminator) - CByte(Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Char]) <= CByte(Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.Int32), True, False)
			Return flag
		End Function

		Private Function UncheckedAdd(ByVal x As Integer, ByVal y As Integer) As Integer
			Return x + y
		End Function

		Private Function UncheckedAdd(ByVal x As Long, ByVal y As Long) As Long
			Return x + y
		End Function

		Private Function UncheckedAdd(ByVal x As ULong, ByVal y As ULong) As ULong
			Return x + y
		End Function

		Friend Function UncheckedCByte(ByVal v As SByte) As Byte
			Return CByte(v)
		End Function

		Friend Function UncheckedCByte(ByVal v As Integer) As Byte
			Return CByte(v)
		End Function

		Friend Function UncheckedCByte(ByVal v As Long) As Byte
			Return CByte(v)
		End Function

		Friend Function UncheckedCByte(ByVal v As UShort) As Byte
			Return CByte(v)
		End Function

		Friend Function UncheckedCInt(ByVal v As ULong) As Integer
			Return CInt(v)
		End Function

		Friend Function UncheckedCInt(ByVal v As Long) As Integer
			Return CInt(v)
		End Function

		Friend Function UncheckedCInt(ByVal v As UInteger) As Integer
			Return CInt(v)
		End Function

		Friend Function UncheckedCLng(ByVal v As ULong) As Long
			Return CLng(v)
		End Function

		Friend Function UncheckedCLng(ByVal v As Double) As Long
			Return CLng(Math.Round(v))
		End Function

		Friend Function UncheckedCSByte(ByVal v As Byte) As SByte
			Return CSByte(v)
		End Function

		Friend Function UncheckedCSByte(ByVal v As Integer) As SByte
			Return CSByte(v)
		End Function

		Friend Function UncheckedCSByte(ByVal v As Long) As SByte
			Return CSByte(v)
		End Function

		Friend Function UncheckedCShort(ByVal v As ULong) As Short
			Return CShort(v)
		End Function

		Friend Function UncheckedCShort(ByVal v As Long) As Short
			Return CShort(v)
		End Function

		Friend Function UncheckedCShort(ByVal v As Integer) As Short
			Return CShort(v)
		End Function

		Friend Function UncheckedCShort(ByVal v As UShort) As Short
			Return CShort(v)
		End Function

		Friend Function UncheckedCShort(ByVal v As UInteger) As Short
			Return CShort(v)
		End Function

		Friend Function UncheckedCUInt(ByVal v As ULong) As UInteger
			Return CUInt(v)
		End Function

		Friend Function UncheckedCUInt(ByVal v As Long) As UInteger
			Return CUInt(v)
		End Function

		Friend Function UncheckedCUInt(ByVal v As Integer) As UInteger
			Return CUInt(v)
		End Function

		Friend Function UncheckedCULng(ByVal v As Long) As ULong
			Return CULng(v)
		End Function

		Friend Function UncheckedCULng(ByVal v As Integer) As ULong
			Return CULng(v)
		End Function

		Friend Function UncheckedCULng(ByVal v As Double) As ULong
			Return CULng(Math.Round(v))
		End Function

		Friend Function UncheckedCUShort(ByVal v As Short) As UShort
			Return CUShort(v)
		End Function

		Friend Function UncheckedCUShort(ByVal v As Integer) As UShort
			Return CUShort(v)
		End Function

		Friend Function UncheckedCUShort(ByVal v As Long) As UShort
			Return CUShort(v)
		End Function

		Friend Function UncheckedIntegralDiv(ByVal x As Long, ByVal y As Long) As Long
			Dim num As Long
			num = If(y <> CLng(-1), x / y, CompileTimeCalculations.UncheckedNegate(x))
			Return num
		End Function

		Friend Function UncheckedMul(ByVal x As Integer, ByVal y As Integer) As Integer
			Return x * y
		End Function

		Friend Function UncheckedMul(ByVal x As Long, ByVal y As Long) As Long
			Return x * y
		End Function

		Private Function UncheckedNegate(ByVal x As Long) As Long
			Return -x
		End Function

		Private Function UncheckedSub(ByVal x As Long, ByVal y As Long) As Long
			Return x - y
		End Function

		Private Function UncheckedSub(ByVal x As UInteger, ByVal y As UInteger) As UInteger
			Return x - y
		End Function
	End Module
End Namespace