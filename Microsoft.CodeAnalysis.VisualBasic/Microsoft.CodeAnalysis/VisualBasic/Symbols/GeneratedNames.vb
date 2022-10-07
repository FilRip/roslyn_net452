Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Roslyn.Utilities
Imports System
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Text

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class GeneratedNames
		Friend Const DotReplacementInTypeNames As Char = "-"C

		Private Const s_methodNameSeparator As Char = Strings.ChrW(95)

		Private Const s_idSeparator As Char = "-"C

		Private Const s_generationSeparator As Char = "#"C

		Friend Const AnonymousTypeOrDelegateCommonPrefix As String = "VB$Anonymous"

		Friend Const AnonymousTypeTemplateNamePrefix As String = "VB$AnonymousType_"

		Friend Const AnonymousDelegateTemplateNamePrefix As String = "VB$AnonymousDelegate_"

		Public Sub New()
			MyBase.New()
		End Sub

		Friend Shared Function GetKind(ByVal name As String) As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind
			Dim generatedNameKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind
			If (name.StartsWith("$VB$Me", StringComparison.Ordinal)) Then
				generatedNameKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind.HoistedMeField
			ElseIf (name.StartsWith("$State", StringComparison.Ordinal)) Then
				generatedNameKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind.StateMachineStateField
			ElseIf (name.StartsWith("$STATIC$", StringComparison.Ordinal)) Then
				generatedNameKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind.StaticLocalField
			ElseIf (name.StartsWith("$S", StringComparison.Ordinal)) Then
				generatedNameKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind.HoistedSynthesizedLocalField
			ElseIf (name.StartsWith("$VB$Local_", StringComparison.Ordinal)) Then
				generatedNameKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind.HoistedUserVariableField
			ElseIf (name.StartsWith("$Current", StringComparison.Ordinal)) Then
				generatedNameKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind.IteratorCurrentField
			ElseIf (name.StartsWith("$InitialThreadId", StringComparison.Ordinal)) Then
				generatedNameKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind.IteratorInitialThreadIdField
			ElseIf (name.StartsWith("$P_", StringComparison.Ordinal)) Then
				generatedNameKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind.IteratorParameterProxyField
			ElseIf (name.StartsWith("$A", StringComparison.Ordinal)) Then
				generatedNameKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind.StateMachineAwaiterField
			ElseIf (name.StartsWith("$VB$ResumableLocal_", StringComparison.Ordinal)) Then
				generatedNameKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind.StateMachineHoistedUserVariableField
			ElseIf (name.StartsWith("VB$AnonymousType_", StringComparison.Ordinal)) Then
				generatedNameKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind.AnonymousType
			ElseIf (name.StartsWith("_Closure$__", StringComparison.Ordinal)) Then
				generatedNameKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind.LambdaDisplayClass
			ElseIf (name.Equals("$VB$It", StringComparison.Ordinal) OrElse name.Equals("$VB$It1", StringComparison.Ordinal) OrElse name.Equals("$VB$It2", StringComparison.Ordinal)) Then
				generatedNameKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind.TransparentIdentifier
			Else
				generatedNameKind = If(Not name.Equals("$VB$ItAnonymous", StringComparison.Ordinal), Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind.None, Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedNameKind.AnonymousTransparentIdentifier)
			End If
			Return generatedNameKind
		End Function

		Friend Shared Function MakeAnonymousTypeTemplateName(ByVal prefix As String, ByVal index As Integer, ByVal submissionSlotIndex As Integer, ByVal moduleId As String) As String
			If (submissionSlotIndex < 0) Then
				Return [String].Format("{0}{1}{2}", CObj(prefix), index, moduleId)
			End If
			Return [String].Format("{0}{1}_{2}{3}", New [Object]() { prefix, submissionSlotIndex, index, moduleId })
		End Function

		Public Shared Function MakeBaseMethodWrapperName(ByVal methodName As String, ByVal isMyBase As Boolean) As String
			Return [String].Concat("$VB$ClosureStub_", methodName, If(isMyBase, "_MyBase", "_MyClass"))
		End Function

		Public Shared Function MakeCachedFrameInstanceName() As String
			Return "$I"
		End Function

		Friend Shared Function MakeDelegateRelaxationParameterName(ByVal parameterIndex As Integer) As String
			Return [String].Concat("a", StringExtensions.GetNumeral(parameterIndex))
		End Function

		Friend Shared Function MakeDisplayClassGenericParameterName(ByVal parameterIndex As Integer) As String
			Return [String].Concat("$CLS", StringExtensions.GetNumeral(parameterIndex))
		End Function

		Public Shared Function MakeIteratorCurrentFieldName() As String
			Return "$Current"
		End Function

		Public Shared Function MakeIteratorInitialThreadIdName() As String
			Return "$InitialThreadId"
		End Function

		Public Shared Function MakeIteratorParameterProxyName(ByVal paramName As String) As String
			Return [String].Concat("$P_", paramName)
		End Function

		Friend Shared Function MakeLambdaCacheFieldName(ByVal methodOrdinal As Integer, ByVal generation As Integer, ByVal lambdaOrdinal As Integer, ByVal lambdaGeneration As Integer, ByVal lambdaKind As SynthesizedLambdaKind) As String
			Return GeneratedNames.MakeMethodScopedSynthesizedName(If(lambdaKind = SynthesizedLambdaKind.DelegateRelaxationStub, "$IR", "$I"), methodOrdinal, generation, Nothing, lambdaOrdinal, lambdaGeneration, False)
		End Function

		Friend Shared Function MakeLambdaDisplayClassName(ByVal methodOrdinal As Integer, ByVal generation As Integer, ByVal closureOrdinal As Integer, ByVal closureGeneration As Integer, ByVal isDelegateRelaxation As Boolean) As String
			Return GeneratedNames.MakeMethodScopedSynthesizedName(If(isDelegateRelaxation, "_Closure$__R", "_Closure$__"), methodOrdinal, generation, Nothing, closureOrdinal, closureGeneration, True)
		End Function

		Friend Shared Function MakeLambdaDisplayClassStorageName(ByVal uniqueId As Integer) As String
			Return [String].Concat("$VB$Closure_", StringExtensions.GetNumeral(uniqueId))
		End Function

		Friend Shared Function MakeLambdaMethodName(ByVal methodOrdinal As Integer, ByVal generation As Integer, ByVal lambdaOrdinal As Integer, ByVal lambdaGeneration As Integer, ByVal lambdaKind As SynthesizedLambdaKind) As String
			Return GeneratedNames.MakeMethodScopedSynthesizedName(If(lambdaKind = SynthesizedLambdaKind.DelegateRelaxationStub, "_Lambda$__R", "_Lambda$__"), methodOrdinal, generation, Nothing, lambdaOrdinal, lambdaGeneration, False)
		End Function

		Private Shared Function MakeMethodScopedSynthesizedName(ByVal prefix As String, ByVal methodOrdinal As Integer, ByVal methodGeneration As Integer, Optional ByVal methodNameOpt As String = Nothing, Optional ByVal entityOrdinal As Integer = -1, Optional ByVal entityGeneration As Integer = -1, Optional ByVal isTypeName As Boolean = False) As String
			Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
			Dim builder As StringBuilder = instance.Builder
			builder.Append(prefix)
			If (methodOrdinal >= 0) Then
				builder.Append(methodOrdinal)
				If (methodGeneration > 0) Then
					builder.Append("#"C)
					builder.Append(methodGeneration)
				End If
			End If
			If (entityOrdinal >= 0) Then
				If (methodOrdinal >= 0) Then
					builder.Append("-"C)
				End If
				builder.Append(entityOrdinal)
				If (entityGeneration > 0) Then
					builder.Append("#"C)
					builder.Append(entityGeneration)
				End If
			End If
			If (methodNameOpt IsNot Nothing) Then
				builder.Append(Strings.ChrW(95))
				builder.Append(methodNameOpt)
				If (isTypeName) Then
					builder.Replace("."C, "-"C)
				End If
			End If
			Return instance.ToStringAndFree()
		End Function

		Friend Shared Function MakeSignatureString(ByVal signature As Byte()) As String
			Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
			Dim numArray As Byte() = signature
			Dim num As Integer = 0
			Do
				Dim num1 As Byte = numArray(num)
				instance.Builder.AppendFormat("{0:X}", num1)
				num = num + 1
			Loop While num < CInt(numArray.Length)
			Return instance.ToStringAndFree()
		End Function

		Public Shared Function MakeStateMachineAwaiterFieldName(ByVal index As Integer) As String
			Return [String].Concat("$A", StringExtensions.GetNumeral(index))
		End Function

		Public Shared Function MakeStateMachineBuilderFieldName() As String
			Return "$Builder"
		End Function

		Public Shared Function MakeStateMachineCapturedClosureMeName(ByVal closureName As String) As String
			Return [String].Concat("$VB$NonLocal_", closureName)
		End Function

		Public Shared Function MakeStateMachineCapturedMeName() As String
			Return "$VB$Me"
		End Function

		Public Shared Function MakeStateMachineParameterName(ByVal paramName As String) As String
			Return [String].Concat("$VB$Local_", paramName)
		End Function

		Public Shared Function MakeStateMachineStateFieldName() As String
			Return "$State"
		End Function

		Public Shared Function MakeStateMachineTypeName(ByVal methodName As String, ByVal methodOrdinal As Integer, ByVal generation As Integer) As String
			Return GeneratedNames.MakeMethodScopedSynthesizedName("VB$StateMachine_", methodOrdinal, generation, methodName, -1, -1, True)
		End Function

		Public Shared Function MakeStaticLambdaDisplayClassName(ByVal methodOrdinal As Integer, ByVal generation As Integer) As String
			Return GeneratedNames.MakeMethodScopedSynthesizedName("_Closure$__", methodOrdinal, generation, Nothing, -1, -1, False)
		End Function

		Friend Shared Function MakeStaticLocalFieldName(ByVal methodName As String, ByVal methodSignature As String, ByVal localName As String) As String
			Return [String].Format("$STATIC${0}${1}${2}", CObj(methodName), methodSignature, localName)
		End Function

		Friend Shared Function MakeSynthesizedLocalName(ByVal kind As Microsoft.CodeAnalysis.SynthesizedLocalKind, ByRef uniqueId As Integer) As String
			Dim str As String
			Dim synthesizedLocalKind As Microsoft.CodeAnalysis.SynthesizedLocalKind = kind
			If (synthesizedLocalKind = Microsoft.CodeAnalysis.SynthesizedLocalKind.[With]) Then
				str = [String].Concat("$W", StringExtensions.GetNumeral(uniqueId))
				uniqueId = uniqueId + 1
			ElseIf (synthesizedLocalKind <> Microsoft.CodeAnalysis.SynthesizedLocalKind.LambdaDisplayClass) Then
				str = Nothing
			Else
				str = GeneratedNames.MakeLambdaDisplayClassStorageName(uniqueId)
				uniqueId = uniqueId + 1
			End If
			Return str
		End Function

		Public Shared Function ReusableHoistedLocalFieldName(ByVal number As Integer) As String
			Return [String].Concat("$U", StringExtensions.GetNumeral(number))
		End Function

		Friend Shared Function TryParseAnonymousTypeTemplateName(ByVal prefix As String, ByVal name As String, <Out> ByRef index As Integer) As Boolean
			Dim flag As Boolean
			If (Not name.StartsWith(prefix, StringComparison.Ordinal) OrElse Not Int32.TryParse(name.Substring(prefix.Length), NumberStyles.None, CultureInfo.InvariantCulture, index)) Then
				index = -1
				flag = False
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Shared Function TryParseHoistedUserVariableName(ByVal proxyName As String, <Out> ByRef variableName As String) As Boolean
			Dim flag As Boolean
			variableName = Nothing
			Dim length As Integer = "$VB$Local_".Length
			If (proxyName.Length <= length) Then
				flag = False
			ElseIf (proxyName.StartsWith("$VB$Local_", StringComparison.Ordinal)) Then
				variableName = proxyName.Substring(length)
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Shared Function TryParseSlotIndex(ByVal prefix As String, ByVal fieldName As String, <Out> ByRef slotIndex As Integer) As Boolean
			Dim flag As Boolean
			If (Not fieldName.StartsWith(prefix, StringComparison.Ordinal) OrElse Not Int32.TryParse(fieldName.Substring(prefix.Length), NumberStyles.None, CultureInfo.InvariantCulture, slotIndex)) Then
				slotIndex = -1
				flag = False
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Shared Function TryParseStateMachineHoistedUserVariableName(ByVal proxyName As String, <Out> ByRef variableName As String, <Out> ByRef index As Integer) As Boolean
			Dim flag As Boolean
			variableName = Nothing
			index = 0
			If (proxyName.StartsWith("$VB$ResumableLocal_", StringComparison.Ordinal)) Then
				Dim length As Integer = "$VB$ResumableLocal_".Length
				Dim num As Integer = proxyName.LastIndexOf("$"C)
				If (num > length) Then
					variableName = proxyName.Substring(length, num - length)
					flag = Int32.TryParse(proxyName.Substring(num + 1), NumberStyles.None, CultureInfo.InvariantCulture, index)
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Shared Function TryParseStateMachineTypeName(ByVal stateMachineTypeName As String, <Out> ByRef methodName As String) As Boolean
			Dim flag As Boolean
			If (stateMachineTypeName.StartsWith("VB$StateMachine_", StringComparison.Ordinal)) Then
				Dim num As Integer = stateMachineTypeName.IndexOf(Strings.ChrW(95), "VB$StateMachine_".Length)
				If (num < 0 OrElse num = stateMachineTypeName.Length - 1) Then
					flag = False
				Else
					methodName = stateMachineTypeName.Substring(num + 1)
					flag = True
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Shared Function TryParseStaticLocalFieldName(ByVal fieldName As String, <Out> ByRef methodName As String, <Out> ByRef methodSignature As String, <Out> ByRef localName As String) As Boolean
			Dim flag As Boolean
			If (fieldName.StartsWith("$STATIC$", StringComparison.Ordinal)) Then
				Dim strArray As String() = fieldName.Split(New [Char]() { "$"C })
				If (CInt(strArray.Length) <> 5) Then
					methodName = Nothing
					methodSignature = Nothing
					localName = Nothing
					flag = False
					Return flag
				End If
				methodName = strArray(2)
				methodSignature = strArray(3)
				localName = strArray(4)
				flag = True
				Return flag
			End If
			methodName = Nothing
			methodSignature = Nothing
			localName = Nothing
			flag = False
			Return flag
		End Function
	End Class
End Namespace