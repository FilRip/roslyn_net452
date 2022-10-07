Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.CodeGen
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class OverloadResolution
		Private ReadOnly Shared s_operatorNames As Dictionary(Of String, OverloadResolution.OperatorInfo)

		Private Const s_stateSize As Integer = 8

		Shared Sub New()
			OverloadResolution.s_operatorNames = New Dictionary(Of String, OverloadResolution.OperatorInfo)(CaseInsensitiveComparison.Comparer) From
			{
				{ "op_OnesComplement", New OverloadResolution.OperatorInfo(UnaryOperatorKind.[Not]) },
				{ "op_True", New OverloadResolution.OperatorInfo(UnaryOperatorKind.IsTrue) },
				{ "op_False", New OverloadResolution.OperatorInfo(UnaryOperatorKind.IsFalse) },
				{ "op_UnaryPlus", New OverloadResolution.OperatorInfo(UnaryOperatorKind.Plus) },
				{ "op_Addition", New OverloadResolution.OperatorInfo(BinaryOperatorKind.Add) },
				{ "op_UnaryNegation", New OverloadResolution.OperatorInfo(UnaryOperatorKind.Minus) },
				{ "op_Subtraction", New OverloadResolution.OperatorInfo(BinaryOperatorKind.Subtract) },
				{ "op_Multiply", New OverloadResolution.OperatorInfo(BinaryOperatorKind.Multiply) },
				{ "op_Division", New OverloadResolution.OperatorInfo(BinaryOperatorKind.Divide) },
				{ "op_IntegerDivision", New OverloadResolution.OperatorInfo(BinaryOperatorKind.IntegerDivide) },
				{ "op_Modulus", New OverloadResolution.OperatorInfo(BinaryOperatorKind.Modulo) },
				{ "op_Exponent", New OverloadResolution.OperatorInfo(BinaryOperatorKind.Power) },
				{ "op_Equality", New OverloadResolution.OperatorInfo(BinaryOperatorKind.Equals) },
				{ "op_Inequality", New OverloadResolution.OperatorInfo(BinaryOperatorKind.NotEquals) },
				{ "op_LessThan", New OverloadResolution.OperatorInfo(BinaryOperatorKind.LessThan) },
				{ "op_GreaterThan", New OverloadResolution.OperatorInfo(BinaryOperatorKind.GreaterThan) },
				{ "op_LessThanOrEqual", New OverloadResolution.OperatorInfo(BinaryOperatorKind.LessThanOrEqual) },
				{ "op_GreaterThanOrEqual", New OverloadResolution.OperatorInfo(BinaryOperatorKind.GreaterThanOrEqual) },
				{ "op_Like", New OverloadResolution.OperatorInfo(BinaryOperatorKind.[Like]) },
				{ "op_Concatenate", New OverloadResolution.OperatorInfo(BinaryOperatorKind.Concatenate) },
				{ "op_BitwiseAnd", New OverloadResolution.OperatorInfo(BinaryOperatorKind.[And]) },
				{ "op_BitwiseOr", New OverloadResolution.OperatorInfo(BinaryOperatorKind.[Or]) },
				{ "op_ExclusiveOr", New OverloadResolution.OperatorInfo(BinaryOperatorKind.[Xor]) },
				{ "op_LeftShift", New OverloadResolution.OperatorInfo(BinaryOperatorKind.LeftShift) },
				{ "op_RightShift", New OverloadResolution.OperatorInfo(BinaryOperatorKind.RightShift) },
				{ "op_Implicit", New OverloadResolution.OperatorInfo(UnaryOperatorKind.Implicit) },
				{ "op_Explicit", New OverloadResolution.OperatorInfo(UnaryOperatorKind.Explicit) },
				{ "op_LogicalNot", New OverloadResolution.OperatorInfo(UnaryOperatorKind.[Not]) },
				{ "op_LogicalAnd", New OverloadResolution.OperatorInfo(BinaryOperatorKind.[And]) },
				{ "op_LogicalOr", New OverloadResolution.OperatorInfo(BinaryOperatorKind.[Or]) },
				{ "op_UnsignedLeftShift", New OverloadResolution.OperatorInfo(BinaryOperatorKind.LeftShift) },
				{ "op_UnsignedRightShift", New OverloadResolution.OperatorInfo(BinaryOperatorKind.RightShift) }
			}
		End Sub

		Private Sub New()
			MyBase.New()
			Throw ExceptionUtilities.Unreachable
		End Sub

		Friend Shared Sub AdvanceParameterInVirtualSignature(ByRef candidate As OverloadResolution.CandidateAnalysisResult, ByRef paramIndex As Integer)
			If (Not candidate.IsExpandedParamArrayForm OrElse paramIndex <> candidate.Candidate.ParameterCount - 1) Then
				paramIndex = paramIndex + 1
			End If
		End Sub

		Private Shared Function AnalyzeNarrowingCandidates(ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal delegateReturnType As TypeSymbol, ByVal lateBindingIsAllowed As Boolean, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByRef resolutionIsLateBound As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Integer
			' 
			' Current member / type: System.Int32 Microsoft.CodeAnalysis.VisualBasic.OverloadResolution::AnalyzeNarrowingCandidates(Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder`1<Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/CandidateAnalysisResult>,System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.VisualBasic.BoundExpression>,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Boolean,Microsoft.CodeAnalysis.VisualBasic.Binder,System.Boolean&,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Int32 AnalyzeNarrowingCandidates(Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder<Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/CandidateAnalysisResult>,System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.VisualBasic.BoundExpression>,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Boolean,Microsoft.CodeAnalysis.VisualBasic.Binder,System.Boolean&,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function ApplyTieBreakingRules(ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult), ByVal bucket As ArrayBuilder(Of Integer), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal delegateReturnType As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Integer
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			Dim count As Integer = bucket.Count
			Dim num As Integer = bucket.Count - 1
			Dim num1 As Integer = 0
			Do
				Dim item As OverloadResolution.CandidateAnalysisResult = candidates(bucket(num1))
				If (item.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
					Dim count1 As Integer = bucket.Count - 1
					For i As Integer = num1 + 1 To count1
						Dim candidateAnalysisResult As OverloadResolution.CandidateAnalysisResult = candidates(bucket(i))
						If (candidateAnalysisResult.State = OverloadResolution.CandidateAnalysisResultState.Applicable AndAlso OverloadResolution.ShadowBasedOnTieBreakingRules(item, candidateAnalysisResult, arguments, delegateReturnType, flag, flag1, binder, useSiteInfo)) Then
							If (Not flag) Then
								item.State = OverloadResolution.CandidateAnalysisResultState.Shadowed
								candidates(bucket(num1)) = item
								count = count - 1
								Exit For
							Else
								candidateAnalysisResult.State = OverloadResolution.CandidateAnalysisResultState.Shadowed
								candidates(bucket(i)) = candidateAnalysisResult
								count = count - 1
							End If
						End If
					Next

				End If
				num1 = num1 + 1
			Loop While num1 <= num
			Return count
		End Function

		Private Shared Function ApplyTieBreakingRulesToEquallyApplicableCandidates(ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal delegateReturnType As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Integer
			Dim arrayBuilders As ArrayBuilder(Of ArrayBuilder(Of Integer)) = OverloadResolution.GroupEquallyApplicableCandidates(candidates, arguments, binder)
			Dim num As Integer = 0
			Dim count As Integer = arrayBuilders.Count - 1
			Dim num1 As Integer = 0
			Do
				num += OverloadResolution.ApplyTieBreakingRules(candidates, arrayBuilders(num1), arguments, delegateReturnType, binder, useSiteInfo)
				num1 = num1 + 1
			Loop While num1 <= count
			Dim count1 As Integer = arrayBuilders.Count - 1
			Dim num2 As Integer = 0
			Do
				arrayBuilders(num2).Free()
				num2 = num2 + 1
			Loop While num2 <= count1
			arrayBuilders.Free()
			Return num
		End Function

		Private Shared Sub BuildParameterToArgumentMap(ByRef candidate As OverloadResolution.CandidateAnalysisResult, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal argumentNames As ImmutableArray(Of String), ByRef parameterToArgumentMap As ArrayBuilder(Of Integer), ByRef paramArrayItems As ArrayBuilder(Of Integer))
			parameterToArgumentMap = ArrayBuilder(Of Integer).GetInstance(candidate.Candidate.ParameterCount, -1)
			Dim instance As ArrayBuilder(Of Integer) = Nothing
			If (Not argumentNames.IsDefault) Then
				instance = ArrayBuilder(Of Integer).GetInstance(arguments.Length, -1)
			End If
			paramArrayItems = Nothing
			If (candidate.IsExpandedParamArrayForm) Then
				paramArrayItems = ArrayBuilder(Of Integer).GetInstance()
			End If
			Dim num As Integer = 0
			Dim num1 As Integer = 0
			Dim length As Integer = arguments.Length - 1
			Dim num2 As Integer = 0
			Do
				If (Not argumentNames.IsDefault AndAlso argumentNames(num2) IsNot Nothing) Then
					If (candidate.Candidate.TryGetNamedParamIndex(argumentNames(num2), num1)) Then
						If (num1 <> num2) Then
							Exit Do
						End If
						If (num1 = candidate.Candidate.ParameterCount - 1 AndAlso candidate.Candidate.Parameters(num1).IsParamArray) Then
							candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
							If (instance IsNot Nothing) Then
								instance.Free()
								instance = Nothing
							End If
							Return
						End If
					Else
						candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
						If (instance IsNot Nothing) Then
							instance.Free()
							instance = Nothing
						End If
						Return
					End If
				End If
				num = num + 1
				If (instance IsNot Nothing) Then
					instance(num2) = num1
				End If
				If (arguments(num2).Kind = BoundKind.OmittedArgument) Then
					If (num1 <> candidate.Candidate.ParameterCount - 1 OrElse Not candidate.Candidate.Parameters(num1).IsParamArray) Then
						parameterToArgumentMap(num1) = num2
						num1 = num1 + 1
					Else
						candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
						If (instance IsNot Nothing) Then
							instance.Free()
							instance = Nothing
						End If
						Return
					End If
				ElseIf (Not candidate.IsExpandedParamArrayForm OrElse num1 <> candidate.Candidate.ParameterCount - 1) Then
					parameterToArgumentMap(num1) = num2
					num1 = num1 + 1
				Else
					paramArrayItems.Add(num2)
				End If
				num2 = num2 + 1
			Loop While num2 <= length
			Dim length1 As Integer = arguments.Length - 1
			Dim num3 As Integer = num
			While num3 <= length1
				If (argumentNames(num3) Is Nothing) Then
					candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
					If (instance IsNot Nothing) Then
						instance.Free()
						instance = Nothing
					End If
					Return
				ElseIf (candidate.Candidate.TryGetNamedParamIndex(argumentNames(num3), num1)) Then
					If (instance IsNot Nothing) Then
						instance(num3) = num1
					End If
					If (num1 = candidate.Candidate.ParameterCount - 1 AndAlso candidate.Candidate.Parameters(num1).IsParamArray) Then
						candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
						If (instance IsNot Nothing) Then
							instance.Free()
							instance = Nothing
						End If
						Return
					ElseIf (parameterToArgumentMap(num1) <> -1) Then
						candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
						If (instance IsNot Nothing) Then
							instance.Free()
							instance = Nothing
						End If
						Return
					ElseIf (num1 >= num) Then
						parameterToArgumentMap(num1) = num3
						num3 = num3 + 1
					Else
						candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
						If (instance IsNot Nothing) Then
							instance.Free()
							instance = Nothing
						End If
						Return
					End If
				Else
					candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
					If (instance IsNot Nothing) Then
						instance.Free()
						instance = Nothing
					End If
					Return
				End If
			End While
			GoTo Label1
			If (instance IsNot Nothing) Then
				instance.Free()
				instance = Nothing
			End If
			Return
		Label1:
			If (instance IsNot Nothing) Then
				candidate.ArgsToParamsOpt = instance.ToImmutableAndFree()
				instance = Nothing
				If (instance IsNot Nothing) Then
					instance.Free()
					instance = Nothing
				End If
				Return
			Else
				If (instance IsNot Nothing) Then
					instance.Free()
					instance = Nothing
				End If
				Return
			End If
		End Sub

		Private Shared Function CandidatesAreEquallyApplicableToArguments(ByRef left As OverloadResolution.CandidateAnalysisResult, ByRef right As OverloadResolution.CandidateAnalysisResult, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Boolean
			Dim parameterTypeFromVirtualSignature As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim num As Integer = 0
			Dim num1 As Integer = 0
			Dim length As Integer = arguments.Length - 1
			Dim num2 As Integer = 0
			Do
				If (Not left.ArgsToParamsOpt.IsDefault) Then
					parameterTypeFromVirtualSignature = OverloadResolution.GetParameterTypeFromVirtualSignature(left, left.ArgsToParamsOpt(num2))
				Else
					parameterTypeFromVirtualSignature = OverloadResolution.GetParameterTypeFromVirtualSignature(left, num)
					OverloadResolution.AdvanceParameterInVirtualSignature(left, num)
				End If
				If (Not right.ArgsToParamsOpt.IsDefault) Then
					typeSymbol = OverloadResolution.GetParameterTypeFromVirtualSignature(right, right.ArgsToParamsOpt(num2))
				Else
					typeSymbol = OverloadResolution.GetParameterTypeFromVirtualSignature(right, num1)
					OverloadResolution.AdvanceParameterInVirtualSignature(right, num1)
				End If
				If (arguments(num2).Kind <> BoundKind.OmittedArgument AndAlso Not OverloadResolution.ParametersAreEquallyApplicableToArgument(parameterTypeFromVirtualSignature, typeSymbol, arguments(num2), binder)) Then
					Exit Do
				End If
				num2 = num2 + 1
			Loop While num2 <= length
			Return num2 >= arguments.Length
		End Function

		Friend Shared Function CanLiftType(ByVal type As TypeSymbol) As Boolean
			If (type.IsNullableType() OrElse Not type.IsValueType) Then
				Return False
			End If
			Return Not type.IsRestrictedType()
		End Function

		Public Shared Function CanPassToParamArray(ByVal expression As BoundExpression, ByVal targetType As TypeSymbol, <Out> ByRef outConvKind As KeyValuePair(Of ConversionKind, MethodSymbol), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			outConvKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyConversion(expression, targetType, binder, useSiteInfo)
			If (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(outConvKind.Key)) Then
				flag = If(Not expression.IsNothingLiteral(), False, True)
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function ChooseMostSpecificConversionOperator(ByVal opSet As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol), ByVal applicable As BitVector, ByVal mostSpecificSourceType As TypeSymbol, ByVal mostSpecificTargetType As TypeSymbol, <Out> ByRef bestMatchIsAmbiguous As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim num As Integer = -1
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			bestMatchIsAmbiguous = False
			Dim count As Integer = opSet.Count - 1
			Dim num1 As Integer = 0
			Do
				If (applicable(num1)) Then
					Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = opSet(num1)
					If (mostSpecificSourceType.IsSameTypeIgnoringAll(item.Parameters(0).Type) AndAlso mostSpecificTargetType.IsSameTypeIgnoringAll(item.ReturnType)) Then
						If (methodSymbol IsNot Nothing) Then
							methodSymbol = OverloadResolution.LeastGenericConversionOperator(methodSymbol, item, num, bestMatchIsAmbiguous)
							If (bestMatchIsAmbiguous AndAlso num = 0) Then
								Exit Do
							End If
						Else
							methodSymbol = item
						End If
					End If
				End If
				num1 = num1 + 1
			Loop While num1 <= count
			Return methodSymbol
		End Function

		Private Shared Function ClassifyConversionOperatorInOutConversions(ByVal source As TypeSymbol, ByVal destination As TypeSymbol, ByVal method As MethodSymbol, <Out> ByRef conversionIn As ConversionKind, <Out> ByRef conversionOut As ConversionKind, ByVal suppressViabilityChecks As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			Dim type As TypeSymbol = method.Parameters(0).Type
			Dim returnType As TypeSymbol = method.ReturnType
			If (suppressViabilityChecks OrElse OverloadResolution.IsConversionOperatorViableBasedOnTypesInvolved(method, type, returnType, useSiteInfo)) Then
				Dim arrayLiteralTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.ArrayLiteralTypeSymbol = TryCast(source, Microsoft.CodeAnalysis.VisualBasic.ArrayLiteralTypeSymbol)
				If (arrayLiteralTypeSymbol Is Nothing) Then
					conversionIn = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyPredefinedConversion(source, type, useSiteInfo)
				Else
					Dim arrayLiteral As BoundArrayLiteral = arrayLiteralTypeSymbol.ArrayLiteral
					conversionIn = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyArrayLiteralConversion(arrayLiteral, type, arrayLiteral.Binder, useSiteInfo)
					If (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(DirectCast(CInt(conversionIn), ConversionKind)) AndAlso arrayLiteralTypeSymbol.IsSameTypeIgnoringAll(type)) Then
						conversionIn = ConversionKind.Identity
					End If
				End If
				conversionOut = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyPredefinedConversion(returnType, destination, useSiteInfo)
				flag = True
			Else
				conversionIn = ConversionKind.DelegateRelaxationLevelNone
				conversionOut = ConversionKind.DelegateRelaxationLevelNone
				flag = False
			End If
			Return flag
		End Function

		Private Shared Sub CollectOverloadedCandidate(ByVal results As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult), ByVal candidate As OverloadResolution.QuickApplicabilityInfo, ByVal typeArguments As ImmutableArray(Of TypeSymbol), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal argumentNames As ImmutableArray(Of String), ByVal delegateReturnType As TypeSymbol, ByVal delegateReturnTypeReferenceBoundNode As BoundNode, ByVal includeEliminatedCandidates As Boolean, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef asyncLambdaSubToFunctionMismatch As HashSet(Of BoundExpression), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim candidateAnalysisResult As OverloadResolution.CandidateAnalysisResult
			Select Case candidate.State
				Case OverloadResolution.CandidateAnalysisResultState.Applicable
					candidateAnalysisResult = If(typeArguments.Length <= 0, New OverloadResolution.CandidateAnalysisResult(candidate.Candidate), New OverloadResolution.CandidateAnalysisResult(candidate.Candidate.Construct(typeArguments)))
					If (candidate.AppliesToNormalForm) Then
						OverloadResolution.InferTypeArgumentsIfNeedToAndCombineWithExistingCandidates(results, candidateAnalysisResult, typeArguments, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, binder, asyncLambdaSubToFunctionMismatch, useSiteInfo)
					End If
					If (Not candidate.AppliesToParamArrayForm) Then
						Exit Select
					End If
					candidateAnalysisResult.SetIsExpandedParamArrayForm()
					candidateAnalysisResult.ExpandedParamArrayArgumentsUsed = Math.Max(arguments.Length - candidate.Candidate.ParameterCount + 1, 0)
					OverloadResolution.InferTypeArgumentsIfNeedToAndCombineWithExistingCandidates(results, candidateAnalysisResult, typeArguments, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, binder, asyncLambdaSubToFunctionMismatch, useSiteInfo)
					Return
				Case OverloadResolution.CandidateAnalysisResultState.HasUnsupportedMetadata
					If (Not includeEliminatedCandidates) Then
						Exit Select
					End If
					results.Add(New OverloadResolution.CandidateAnalysisResult(candidate.Candidate, OverloadResolution.CandidateAnalysisResultState.HasUnsupportedMetadata))
					Return
				Case OverloadResolution.CandidateAnalysisResultState.HasUseSiteError
					If (Not includeEliminatedCandidates) Then
						Exit Select
					End If
					results.Add(New OverloadResolution.CandidateAnalysisResult(candidate.Candidate, OverloadResolution.CandidateAnalysisResultState.HasUseSiteError))
					Return
				Case OverloadResolution.CandidateAnalysisResultState.Ambiguous
					If (Not includeEliminatedCandidates) Then
						Exit Select
					End If
					results.Add(New OverloadResolution.CandidateAnalysisResult(candidate.Candidate, OverloadResolution.CandidateAnalysisResultState.Ambiguous))
					Return
				Case OverloadResolution.CandidateAnalysisResultState.BadGenericArity
					If (Not includeEliminatedCandidates) Then
						Exit Select
					End If
					results.Add(New OverloadResolution.CandidateAnalysisResult(candidate.Candidate, OverloadResolution.CandidateAnalysisResultState.BadGenericArity))
					Return
				Case OverloadResolution.CandidateAnalysisResultState.ArgumentCountMismatch
					If (Not includeEliminatedCandidates) Then
						Exit Select
					End If
					Dim candidateAnalysisResult1 As OverloadResolution.CandidateAnalysisResult = New OverloadResolution.CandidateAnalysisResult(OverloadResolution.ConstructIfNeedTo(candidate.Candidate, typeArguments), OverloadResolution.CandidateAnalysisResultState.ArgumentCountMismatch)
					If (candidate.AppliesToParamArrayForm) Then
						candidateAnalysisResult1.SetIsExpandedParamArrayForm()
					End If
					results.Add(candidateAnalysisResult1)
					Return
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(candidate.State)
			End Select
		End Sub

		Private Shared Sub CollectOverloadedCandidates(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal results As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult), ByVal group As ArrayBuilder(Of OverloadResolution.Candidate), ByVal typeArguments As ImmutableArray(Of TypeSymbol), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal argumentNames As ImmutableArray(Of String), ByVal delegateReturnType As TypeSymbol, ByVal delegateReturnTypeReferenceBoundNode As BoundNode, ByVal includeEliminatedCandidates As Boolean, ByVal isQueryOperatorInvocation As Boolean, ByVal forceExpandedForm As Boolean, <InAttribute> <Out> ByRef asyncLambdaSubToFunctionMismatch As HashSet(Of BoundExpression), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim quickApplicabilityInfo As OverloadResolution.QuickApplicabilityInfo
			Dim instance As ArrayBuilder(Of OverloadResolution.QuickApplicabilityInfo) = ArrayBuilder(Of OverloadResolution.QuickApplicabilityInfo).GetInstance()
			Dim sourceModule As ModuleSymbol = binder.SourceModule
			Dim count As Integer = group.Count - 1
			Dim num As Integer = 0
			Do
				If (group(num) IsNot Nothing) Then
					Dim item As OverloadResolution.QuickApplicabilityInfo = OverloadResolution.DoQuickApplicabilityCheck(group(num), typeArguments, arguments, isQueryOperatorInvocation, forceExpandedForm, useSiteInfo)
					If (item.Candidate IsNot Nothing) Then
						If (CObj(item.Candidate.UnderlyingSymbol.ContainingModule) = CObj(sourceModule) OrElse item.Candidate.IsExtensionMethod) Then
							OverloadResolution.CollectOverloadedCandidate(results, item, typeArguments, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, includeEliminatedCandidates, binder, asyncLambdaSubToFunctionMismatch, useSiteInfo)
						Else
							Dim containingSymbol As Symbol = item.Candidate.UnderlyingSymbol.ContainingSymbol
							instance.Clear()
							instance.Add(item)
							Dim num1 As Integer = If(item.State = OverloadResolution.CandidateAnalysisResultState.Applicable, 1, 0)
							Dim count1 As Integer = group.Count - 1
							Dim num2 As Integer = num + 1
							Do
								If (group(num2) IsNot Nothing AndAlso Not group(num2).IsExtensionMethod AndAlso containingSymbol = group(num2).UnderlyingSymbol.ContainingSymbol) Then
									item = OverloadResolution.DoQuickApplicabilityCheck(group(num2), typeArguments, arguments, isQueryOperatorInvocation, forceExpandedForm, useSiteInfo)
									group(num2) = Nothing
									If (item.Candidate IsNot Nothing) Then
										If (item.State <> OverloadResolution.CandidateAnalysisResultState.Applicable) Then
											instance.Add(item)
										ElseIf (num1 <> instance.Count) Then
											instance.Add(instance(num1))
											instance(num1) = item
											num1 = num1 + 1
										Else
											instance.Add(item)
											num1 = num1 + 1
										End If
									End If
								End If
								num2 = num2 + 1
							Loop While num2 <= count1
							Dim num3 As Integer = If(num1 > 0 OrElse Not includeEliminatedCandidates, num1, instance.Count) - 1
							For i As Integer = 0 To num3
								item = instance(i)
								If (item.Candidate IsNot Nothing AndAlso item.State <> OverloadResolution.CandidateAnalysisResultState.Ambiguous) Then
									Dim originalDefinition As Symbol = item.Candidate.UnderlyingSymbol.OriginalDefinition
									If (originalDefinition.IsReducedExtensionMethod()) Then
										originalDefinition = DirectCast(originalDefinition, MethodSymbol).ReducedFrom
									End If
									Dim count2 As Integer = instance.Count - 1
									Dim num4 As Integer = i + 1
									Do
										Dim item1 As OverloadResolution.QuickApplicabilityInfo = instance(num4)
										If (item1.Candidate IsNot Nothing AndAlso item1.State <> OverloadResolution.CandidateAnalysisResultState.Ambiguous) Then
											Dim reducedFrom As Symbol = item1.Candidate.UnderlyingSymbol.OriginalDefinition
											If (reducedFrom.IsReducedExtensionMethod()) Then
												reducedFrom = DirectCast(reducedFrom, MethodSymbol).ReducedFrom
											End If
											If (CInt(OverrideHidingHelper.DetailedSignatureCompare(originalDefinition, reducedFrom, SymbolComparisonResults.NameMismatch Or SymbolComparisonResults.ArityMismatch Or SymbolComparisonResults.RequiredExtraParameterMismatch Or SymbolComparisonResults.RequiredParameterTypeMismatch Or SymbolComparisonResults.OptionalParameterTypeMismatch Or SymbolComparisonResults.PropertyInitOnlyMismatch Or SymbolComparisonResults.VarargMismatch Or SymbolComparisonResults.TotalParameterCountMismatch, SymbolComparisonResults.NameMismatch Or SymbolComparisonResults.ArityMismatch Or SymbolComparisonResults.RequiredExtraParameterMismatch Or SymbolComparisonResults.RequiredParameterTypeMismatch Or SymbolComparisonResults.OptionalParameterTypeMismatch Or SymbolComparisonResults.PropertyInitOnlyMismatch Or SymbolComparisonResults.VarargMismatch Or SymbolComparisonResults.TotalParameterCountMismatch)) = 0) Then
												Dim num5 As Integer = LookupResult.CompareAccessibilityOfSymbolsConflictingInSameContainer(originalDefinition, reducedFrom)
												If (num5 > 0) Then
													quickApplicabilityInfo = New OverloadResolution.QuickApplicabilityInfo()
													instance(num4) = quickApplicabilityInfo
												ElseIf (num5 >= 0) Then
													item = New OverloadResolution.QuickApplicabilityInfo(item.Candidate, OverloadResolution.CandidateAnalysisResultState.Ambiguous, True, True)
													instance(i) = item
													instance(num4) = New OverloadResolution.QuickApplicabilityInfo(item1.Candidate, OverloadResolution.CandidateAnalysisResultState.Ambiguous, True, True)
												Else
													instance(i) = item1
													quickApplicabilityInfo = New OverloadResolution.QuickApplicabilityInfo()
													instance(num4) = quickApplicabilityInfo
													originalDefinition = reducedFrom
													item = item1
												End If
											End If
										End If
										num4 = num4 + 1
									Loop While num4 <= count2
									If (item.State <> OverloadResolution.CandidateAnalysisResultState.Ambiguous) Then
										OverloadResolution.CollectOverloadedCandidate(results, item, typeArguments, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, includeEliminatedCandidates, binder, asyncLambdaSubToFunctionMismatch, useSiteInfo)
									ElseIf (includeEliminatedCandidates) Then
										OverloadResolution.CollectOverloadedCandidate(results, item, typeArguments, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, includeEliminatedCandidates, binder, asyncLambdaSubToFunctionMismatch, useSiteInfo)
										Dim count3 As Integer = instance.Count - 1
										For j As Integer = i + 1 To count3
											Dim quickApplicabilityInfo1 As OverloadResolution.QuickApplicabilityInfo = instance(j)
											If (quickApplicabilityInfo1.Candidate IsNot Nothing AndAlso quickApplicabilityInfo1.State = OverloadResolution.CandidateAnalysisResultState.Ambiguous) Then
												quickApplicabilityInfo = New OverloadResolution.QuickApplicabilityInfo()
												instance(j) = quickApplicabilityInfo
												OverloadResolution.CollectOverloadedCandidate(results, quickApplicabilityInfo1, typeArguments, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, includeEliminatedCandidates, binder, asyncLambdaSubToFunctionMismatch, useSiteInfo)
											End If
										Next

									End If
								End If
							Next

						End If
					End If
				End If
				num = num + 1
			Loop While num <= count
			instance.Free()
		End Sub

		Private Shared Sub CollectUserDefinedConversionOperators(ByVal source As TypeSymbol, ByVal destination As TypeSymbol, ByVal opSet As ArrayBuilder(Of MethodSymbol), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			OverloadResolution.CollectUserDefinedOperators(source, destination, MethodKind.Conversion, "op_Implicit", New OverloadResolution.OperatorInfo(UnaryOperatorKind.Implicit), "op_Explicit", New OverloadResolution.OperatorInfo(UnaryOperatorKind.Explicit), opSet, useSiteInfo)
		End Sub

		Friend Shared Sub CollectUserDefinedOperators(ByVal type1 As TypeSymbol, ByVal type2 As TypeSymbol, ByVal opKind As MethodKind, ByVal name1 As String, ByVal name1Info As OverloadResolution.OperatorInfo, ByVal name2Opt As String, ByVal name2InfoOpt As OverloadResolution.OperatorInfo, ByVal opSet As ArrayBuilder(Of MethodSymbol), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			type1 = OverloadResolution.GetTypeToLookForOperatorsIn(type1, useSiteInfo)
			If (type2 IsNot Nothing) Then
				type2 = OverloadResolution.GetTypeToLookForOperatorsIn(type2, useSiteInfo)
			End If
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			If (type1 IsNot Nothing AndAlso type1.Kind = SymbolKind.NamedType AndAlso Not type1.IsInterfaceType()) Then
				Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(type1, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				Do
					If (type2 IsNot Nothing AndAlso namedTypeSymbol Is Nothing AndAlso type2.IsOrDerivedFrom(namedTypeSymbol1, useSiteInfo)) Then
						namedTypeSymbol = namedTypeSymbol1
					End If
					If (OverloadResolution.CollectUserDefinedOperators(namedTypeSymbol1, name1, opKind, name1Info, opSet) Or If(name2Opt Is Nothing, False, OverloadResolution.CollectUserDefinedOperators(namedTypeSymbol1, name2Opt, opKind, name2InfoOpt, opSet))) Then
						Exit Do
					End If
					namedTypeSymbol1 = namedTypeSymbol1.BaseTypeWithDefinitionUseSiteDiagnostics(useSiteInfo)
				Loop While namedTypeSymbol1 IsNot Nothing
			End If
			If (type2 IsNot Nothing AndAlso type2.Kind = SymbolKind.NamedType AndAlso Not type2.IsInterfaceType()) Then
				Dim namedTypeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(type2, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				Do
					If (namedTypeSymbol IsNot Nothing AndAlso namedTypeSymbol.IsSameTypeIgnoringAll(namedTypeSymbol2)) Then
						Exit Do
					End If
					If (OverloadResolution.CollectUserDefinedOperators(namedTypeSymbol2, name1, opKind, name1Info, opSet) Or If(name2Opt Is Nothing, False, OverloadResolution.CollectUserDefinedOperators(namedTypeSymbol2, name2Opt, opKind, name2InfoOpt, opSet))) Then
						Exit Do
					End If
					namedTypeSymbol2 = namedTypeSymbol2.BaseTypeWithDefinitionUseSiteDiagnostics(useSiteInfo)
				Loop While namedTypeSymbol2 IsNot Nothing
			End If
		End Sub

		Private Shared Function CollectUserDefinedOperators(ByVal type As TypeSymbol, ByVal opName As String, ByVal opKind As MethodKind, ByVal opInfo As OverloadResolution.OperatorInfo, ByVal opSet As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)) As Boolean
			Dim flag As Boolean = False
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = type.GetMembers(opName).GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Symbol = enumerator.Current
				If (current.Kind <> SymbolKind.Method) Then
					Continue While
				End If
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
				If (methodSymbol.MethodKind <> opKind) Then
					Continue While
				End If
				If (methodSymbol.IsShadows()) Then
					flag = True
				End If
				If (methodSymbol.IsMethodKindBasedOnSyntax AndAlso Not OverloadResolution.ValidateOverloadedOperator(methodSymbol.OriginalDefinition, opInfo)) Then
					Continue While
				End If
				opSet.Add(methodSymbol)
			End While
			Return flag
		End Function

		Private Shared Sub CombineCandidates(ByVal results As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult), ByVal newCandidate As OverloadResolution.CandidateAnalysisResult, ByVal argumentCount As Integer, ByVal argumentNames As ImmutableArray(Of String), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim isOperator As Boolean = newCandidate.Candidate.IsOperator
			Dim num As Integer = 0
			While True
				If (num < results.Count) Then
					Dim item As OverloadResolution.CandidateAnalysisResult = results(num)
					If (item.State <> OverloadResolution.CandidateAnalysisResultState.ArgumentCountMismatch AndAlso item.State <> OverloadResolution.CandidateAnalysisResultState.BadGenericArity AndAlso item.State <> OverloadResolution.CandidateAnalysisResultState.Ambiguous AndAlso item.Candidate <> newCandidate.Candidate) Then
						Dim flag As Boolean = False
						Dim flag1 As Boolean = False
						If (isOperator OrElse Not OverloadResolution.ShadowBasedOnOverriding(item, newCandidate, flag, flag1)) Then
							If (item.State <> OverloadResolution.CandidateAnalysisResultState.TypeInferenceFailed AndAlso Not item.SomeInferenceFailed AndAlso item.State <> OverloadResolution.CandidateAnalysisResultState.HasUseSiteError AndAlso item.State <> OverloadResolution.CandidateAnalysisResultState.HasUnsupportedMetadata) Then
								If (argumentNames.IsDefault) Then
									Dim num1 As Integer = 0
									Dim num2 As Integer = 0
									Dim num3 As Integer = argumentCount - 1
									For i As Integer = 0 To num3
										If (Not OverloadResolution.GetParameterTypeFromVirtualSignature(item, num1).IsSameTypeIgnoringAll(OverloadResolution.GetParameterTypeFromVirtualSignature(newCandidate, num2))) Then
											GoTo Label0
										End If
										OverloadResolution.AdvanceParameterInVirtualSignature(item, num1)
										OverloadResolution.AdvanceParameterInVirtualSignature(newCandidate, num2)
									Next

								End If
								Dim flag2 As Boolean = True
								If (item.Candidate.ParameterCount <> newCandidate.Candidate.ParameterCount) Then
									flag2 = False
								ElseIf (Not isOperator) Then
									Dim parameterCount As Integer = item.Candidate.ParameterCount - 1
									Dim num4 As Integer = 0
									While num4 <= parameterCount
										If (item.Candidate.Parameters(num4).Type.IsSameTypeIgnoringAll(newCandidate.Candidate.Parameters(num4).Type)) Then
											num4 = num4 + 1
										Else
											flag2 = False
											Exit While
										End If
									End While
								ElseIf (Not item.Candidate.IsLifted) Then
									If (Not newCandidate.Candidate.IsLifted) Then
										GoTo Label2
									End If
									flag = True
									GoTo Label1
								Else
									If (newCandidate.Candidate.IsLifted) Then
										GoTo Label2
									End If
									flag1 = True
									GoTo Label1
								End If
							Label2:
								If (Not argumentNames.IsDefault AndAlso Not flag2) Then
									GoTo Label0
								End If
								If ((flag2 OrElse Not OverloadResolution.ShadowBasedOnParamArrayUsage(item, newCandidate, flag, flag1)) AndAlso (argumentNames.IsDefault OrElse Not item.Candidate.IsExtensionMethod AndAlso Not newCandidate.Candidate.IsExtensionMethod) AndAlso Not OverloadResolution.ShadowBasedOnReceiverType(item, newCandidate, flag, flag1, useSiteInfo)) Then
									OverloadResolution.ShadowBasedOnExtensionMethodTargetTypeGenericity(item, newCandidate, flag, flag1)
								End If
							Else
								GoTo Label0
							End If
						End If
						If (flag1) Then
							results.RemoveAt(num)
							Continue While
						ElseIf (flag) Then
							Exit While
						End If
					End If
				Label0:
					num = num + 1
				Else
					results.Add(newCandidate)
					Exit While
				End If
			End While
		End Sub

		Private Shared Function CompareApplicabilityToTheArguments(ByRef left As OverloadResolution.CandidateAnalysisResult, ByRef right As OverloadResolution.CandidateAnalysisResult, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As OverloadResolution.ApplicabilityComparisonResult
			Dim applicabilityComparisonResult As OverloadResolution.ApplicabilityComparisonResult
			Dim parameterTypeFromVirtualSignature As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim flag As Boolean = True
			Dim flag1 As Boolean = False
			Dim flag2 As Boolean = False
			Dim num As Integer = 0
			Dim num1 As Integer = 0
			Dim length As Integer = arguments.Length - 1
			Dim num2 As Integer = 0
			While True
				If (num2 <= length) Then
					If (Not left.ArgsToParamsOpt.IsDefault) Then
						parameterTypeFromVirtualSignature = OverloadResolution.GetParameterTypeFromVirtualSignature(left, left.ArgsToParamsOpt(num2))
					Else
						parameterTypeFromVirtualSignature = OverloadResolution.GetParameterTypeFromVirtualSignature(left, num)
						OverloadResolution.AdvanceParameterInVirtualSignature(left, num)
					End If
					If (Not right.ArgsToParamsOpt.IsDefault) Then
						typeSymbol = OverloadResolution.GetParameterTypeFromVirtualSignature(right, right.ArgsToParamsOpt(num2))
					Else
						typeSymbol = OverloadResolution.GetParameterTypeFromVirtualSignature(right, num1)
						OverloadResolution.AdvanceParameterInVirtualSignature(right, num1)
					End If
					If (arguments(num2).Kind <> BoundKind.OmittedArgument) Then
						Dim applicabilityComparisonResult1 As OverloadResolution.ApplicabilityComparisonResult = OverloadResolution.CompareParameterTypeApplicability(parameterTypeFromVirtualSignature, typeSymbol, arguments(num2), binder, useSiteInfo)
						If (applicabilityComparisonResult1 = OverloadResolution.ApplicabilityComparisonResult.LeftIsMoreApplicable) Then
							flag1 = True
							If (Not flag2) Then
								flag = False
							Else
								applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.Undefined
								Exit While
							End If
						ElseIf (applicabilityComparisonResult1 = OverloadResolution.ApplicabilityComparisonResult.RightIsMoreApplicable) Then
							flag2 = True
							If (Not flag1) Then
								flag = False
							Else
								applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.Undefined
								Exit While
							End If
						ElseIf (applicabilityComparisonResult1 = OverloadResolution.ApplicabilityComparisonResult.Undefined) Then
							flag = False
						End If
					End If
					num2 = num2 + 1
				ElseIf (flag1) Then
					applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.LeftIsMoreApplicable
					Exit While
				ElseIf (Not flag2) Then
					applicabilityComparisonResult = If(flag, OverloadResolution.ApplicabilityComparisonResult.EquallyApplicable, OverloadResolution.ApplicabilityComparisonResult.Undefined)
					Exit While
				Else
					applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.RightIsMoreApplicable
					Exit While
				End If
			End While
			Return applicabilityComparisonResult
		End Function

		Private Shared Function CompareParameterTypeApplicability(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal argument As BoundExpression, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As OverloadResolution.ApplicabilityComparisonResult
			Dim applicabilityComparisonResult As OverloadResolution.ApplicabilityComparisonResult
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim keyValuePair As KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyConversion(left, right, useSiteInfo)
			If (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(keyValuePair.Key)) Then
				If (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(keyValuePair.Key)) Then
					If (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyConversion(right, left, useSiteInfo).Key)) Then
						GoTo Label1
					End If
					If (argument Is Nothing OrElse Not argument.IsIntegerZeroLiteral() OrElse left.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Enum] OrElse right.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum]) Then
						applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.LeftIsMoreApplicable
						Return applicabilityComparisonResult
					Else
						applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.RightIsMoreApplicable
						Return applicabilityComparisonResult
					End If
				ElseIf (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyConversion(right, left, useSiteInfo).Key)) Then
					If (argument Is Nothing OrElse Not argument.IsIntegerZeroLiteral() OrElse right.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Enum] OrElse left.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum]) Then
						applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.RightIsMoreApplicable
						Return applicabilityComparisonResult
					Else
						applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.LeftIsMoreApplicable
						Return applicabilityComparisonResult
					End If
				ElseIf (left.IsNumericType() AndAlso right.IsNumericType()) Then
					Dim specialType As Microsoft.CodeAnalysis.SpecialType = left.SpecialType
					Dim specialType1 As Microsoft.CodeAnalysis.SpecialType = right.SpecialType
					If (specialType = Microsoft.CodeAnalysis.SpecialType.System_Byte AndAlso specialType1 = Microsoft.CodeAnalysis.SpecialType.System_SByte) Then
						applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.LeftIsMoreApplicable
						Return applicabilityComparisonResult
					ElseIf (specialType = Microsoft.CodeAnalysis.SpecialType.System_SByte AndAlso specialType1 = Microsoft.CodeAnalysis.SpecialType.System_Byte) Then
						applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.RightIsMoreApplicable
						Return applicabilityComparisonResult
					ElseIf (specialType >= specialType1) Then
						applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.RightIsMoreApplicable
						Return applicabilityComparisonResult
					Else
						applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.LeftIsMoreApplicable
						Return applicabilityComparisonResult
					End If
				ElseIf (argument IsNot Nothing) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = left.DelegateOrExpressionDelegate(binder, flag)
					Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = right.DelegateOrExpressionDelegate(binder, flag1)
					If (namedTypeSymbol IsNot Nothing AndAlso namedTypeSymbol1 IsNot Nothing AndAlso (flag = flag1 OrElse argument.IsAnyLambda())) Then
						Dim delegateInvokeMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = namedTypeSymbol.DelegateInvokeMethod
						Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = namedTypeSymbol1.DelegateInvokeMethod
						If (delegateInvokeMethod Is Nothing OrElse delegateInvokeMethod.IsSub OrElse methodSymbol Is Nothing OrElse methodSymbol.IsSub) Then
							GoTo Label1
						End If
						Dim expression As BoundExpression = Nothing
						If (argument.Kind = BoundKind.QueryLambda) Then
							expression = DirectCast(argument, BoundQueryLambda).Expression
						End If
						applicabilityComparisonResult = OverloadResolution.CompareParameterTypeApplicability(delegateInvokeMethod.ReturnType, methodSymbol.ReturnType, expression, binder, useSiteInfo)
						Return applicabilityComparisonResult
					End If
				End If
			Label1:
				If (argument IsNot Nothing) Then
					If (argument.Kind <> BoundKind.ArrayLiteral) Then
						type = argument.Type
					Else
						type = DirectCast(argument, BoundArrayLiteral).InferredType
					End If
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type
					If (typeSymbol IsNot Nothing) Then
						If (Not left.IsSameTypeIgnoringAll(typeSymbol)) Then
							If (Not right.IsSameTypeIgnoringAll(typeSymbol)) Then
								GoTo Label2
							End If
							applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.RightIsMoreApplicable
							Return applicabilityComparisonResult
						Else
							applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.LeftIsMoreApplicable
							Return applicabilityComparisonResult
						End If
					End If
				End If
			Label2:
				applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.Undefined
			Else
				applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.EquallyApplicable
			End If
			Return applicabilityComparisonResult
		End Function

		Private Shared Function CompareParameterTypeGenericDepth(ByVal leftType As TypeSymbol, ByVal rightType As TypeSymbol, ByRef leftWins As Boolean, ByRef rightWins As Boolean) As Boolean
			Dim flag As Boolean
			If (CObj(leftType) = CObj(rightType)) Then
				flag = False
			ElseIf (leftType.IsTypeParameter()) Then
				If (Not rightType.IsTypeParameter()) Then
					rightWins = True
					flag = True
				Else
					flag = False
				End If
			ElseIf (Not rightType.IsTypeParameter()) Then
				If (leftType.IsArrayType() AndAlso rightType.IsArrayType()) Then
					Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(leftType, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
					Dim arrayTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(rightType, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
					If (Not arrayTypeSymbol.HasSameShapeAs(arrayTypeSymbol1)) Then
						GoTo Label1
					End If
					flag = OverloadResolution.CompareParameterTypeGenericDepth(arrayTypeSymbol.ElementType, arrayTypeSymbol1.ElementType, leftWins, rightWins)
					Return flag
				End If
			Label1:
				If (leftType.Kind = SymbolKind.NamedType AndAlso rightType.Kind = SymbolKind.NamedType) Then
					Dim tupleUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(leftType.GetTupleUnderlyingTypeOrSelf(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(rightType.GetTupleUnderlyingTypeOrSelf(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					If (tupleUnderlyingTypeOrSelf.Arity <> namedTypeSymbol.Arity) Then
						GoTo Label2
					End If
					Dim typeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol) = tupleUnderlyingTypeOrSelf.TypeArgumentsNoUseSiteDiagnostics
					Dim typeSymbols As ImmutableArray(Of TypeSymbol) = namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics
					Dim length As Integer = typeArgumentsNoUseSiteDiagnostics.Length - 1
					Dim num As Integer = 0
					Do
						Dim flag1 As Boolean = False
						Dim flag2 As Boolean = False
						If (OverloadResolution.CompareParameterTypeGenericDepth(typeArgumentsNoUseSiteDiagnostics(num), typeSymbols(num), flag1, flag2)) Then
							If (flag1) Then
								If (Not rightWins) Then
									leftWins = True
								Else
									rightWins = False
									flag = False
									Return flag
								End If
							ElseIf (Not leftWins) Then
								rightWins = True
							Else
								leftWins = False
								flag = False
								Return flag
							End If
						End If
						num = num + 1
					Loop While num <= length
					flag = If(leftWins, True, rightWins)
					Return flag
				End If
			Label2:
				flag = False
			Else
				leftWins = True
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function ConstructIfNeedTo(ByVal candidate As OverloadResolution.Candidate, ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As OverloadResolution.Candidate
			Dim candidate1 As OverloadResolution.Candidate
			candidate1 = If(typeArguments.Length <= 0, candidate, candidate.Construct(typeArguments))
			Return candidate1
		End Function

		Private Shared Function CountApplicableCandidates(ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult)) As Integer
			Dim num As Integer = 0
			Dim count As Integer = candidates.Count - 1
			Dim num1 As Integer = 0
			Do
				If (candidates(num1).State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
					num = num + 1
				End If
				num1 = num1 + 1
			Loop While num1 <= count
			Return num
		End Function

		Private Shared Function DetectReferencesToGenericParameters(ByVal symbol As NamedTypeSymbol, ByVal track As OverloadResolution.TypeParameterKind, ByVal methodTypeParametersToTreatAsTypeTypeParameters As BitVector) As OverloadResolution.TypeParameterKind
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/TypeParameterKind Microsoft.CodeAnalysis.VisualBasic.OverloadResolution::DetectReferencesToGenericParameters(Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/TypeParameterKind,Microsoft.CodeAnalysis.BitVector)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/TypeParameterKind DetectReferencesToGenericParameters(Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/TypeParameterKind,Microsoft.CodeAnalysis.BitVector)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function DetectReferencesToGenericParameters(ByVal symbol As TypeParameterSymbol, ByVal track As OverloadResolution.TypeParameterKind, ByVal methodTypeParametersToTreatAsTypeTypeParameters As BitVector) As OverloadResolution.TypeParameterKind
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/TypeParameterKind Microsoft.CodeAnalysis.VisualBasic.OverloadResolution::DetectReferencesToGenericParameters(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol,Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/TypeParameterKind,Microsoft.CodeAnalysis.BitVector)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/TypeParameterKind DetectReferencesToGenericParameters(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol,Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/TypeParameterKind,Microsoft.CodeAnalysis.BitVector)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function DetectReferencesToGenericParameters(ByVal this As TypeSymbol, ByVal track As OverloadResolution.TypeParameterKind, ByVal methodTypeParametersToTreatAsTypeTypeParameters As BitVector) As OverloadResolution.TypeParameterKind
			Dim genericParameters As OverloadResolution.TypeParameterKind
			Dim kind As SymbolKind = this.Kind
			If (kind > SymbolKind.ErrorType) Then
				If (kind = SymbolKind.NamedType) Then
					genericParameters = OverloadResolution.DetectReferencesToGenericParameters(DirectCast(this, NamedTypeSymbol), track, methodTypeParametersToTreatAsTypeTypeParameters)
					Return genericParameters
				End If
				If (kind <> SymbolKind.TypeParameter) Then
					Throw ExceptionUtilities.UnexpectedValue(this.Kind)
				End If
				genericParameters = OverloadResolution.DetectReferencesToGenericParameters(DirectCast(this, TypeParameterSymbol), track, methodTypeParametersToTreatAsTypeTypeParameters)
				Return genericParameters
			Else
				If (kind <> SymbolKind.ArrayType) Then
					If (kind = SymbolKind.ErrorType) Then
						genericParameters = OverloadResolution.DetectReferencesToGenericParameters(DirectCast(this, NamedTypeSymbol), track, methodTypeParametersToTreatAsTypeTypeParameters)
						Return genericParameters
					End If
					Throw ExceptionUtilities.UnexpectedValue(this.Kind)
				End If
				genericParameters = OverloadResolution.DetectReferencesToGenericParameters(DirectCast(this, ArrayTypeSymbol).ElementType, track, methodTypeParametersToTreatAsTypeTypeParameters)
				Return genericParameters
			End If
			Throw ExceptionUtilities.UnexpectedValue(this.Kind)
		End Function

		Private Shared Function DetermineConversionOperatorDegreeOfGenericity(ByVal method As MethodSymbol) As Integer
			Dim num As Integer
			If (method.ContainingType.IsGenericType) Then
				Dim num1 As Integer = 0
				Dim originalDefinition As MethodSymbol = method.OriginalDefinition
				If (OverloadResolution.DetectReferencesToGenericParameters(originalDefinition.Parameters(0).Type, OverloadResolution.TypeParameterKind.Type, BitVector.Null) <> OverloadResolution.TypeParameterKind.None) Then
					num1 = num1 + 1
				End If
				If (OverloadResolution.DetectReferencesToGenericParameters(originalDefinition.ReturnType, OverloadResolution.TypeParameterKind.Type, BitVector.Null) <> OverloadResolution.TypeParameterKind.None) Then
					num1 = num1 + 1
				End If
				num = num1
			Else
				num = 0
			End If
			Return num
		End Function

		Private Shared Function DetermineMostSpecificNarrowingConversion(ByVal source As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal destination As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal opSet As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol), ByVal conversionKinds As ArrayBuilder(Of KeyValuePair(Of ConversionKind, ConversionKind)), <InAttribute> ByRef applicable As BitVector, <Out> ByRef bestMatch As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal suppressViabilityChecks As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			Dim item As Integer
			Dim key As ConversionKind
			Dim value As ConversionKind
			bestMatch = Nothing
			Dim num As Integer = -1
			Dim flag1 As Boolean = False
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
			Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
			Dim num1 As Integer = 0
			Dim num2 As Integer = 0
			Dim num3 As Integer = 0
			Dim num4 As Integer = 0
			Dim count As Integer = opSet.Count - 1
			Dim num5 As Integer = 0
			Do
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = opSet(num5)
				item = num4
				num4 = num4 + 1
				If (item < num5) Then
					opSet(item) = methodSymbol
					conversionKinds(item) = conversionKinds(num5)
				End If
				If (Not OverloadResolution.IsWidening(methodSymbol)) Then
					If (OverloadResolution.ClassifyConversionOperatorInOutConversions(source, destination, methodSymbol, key, value, suppressViabilityChecks, useSiteInfo)) Then
						GoTo Label1
					End If
					num4 = item
					GoTo Label0
				Else
					Dim keyValuePair As KeyValuePair(Of ConversionKind, ConversionKind) = conversionKinds(item)
					key = keyValuePair.Key
					value = keyValuePair.Value
				End If
				If (bestMatch Is Nothing) Then
					If (Microsoft.CodeAnalysis.VisualBasic.Conversions.ConversionExists(key)) Then
						If (Microsoft.CodeAnalysis.VisualBasic.Conversions.ConversionExists(value)) Then
							If (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(key) OrElse Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(value)) Then
								If (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(key)) Then
									If (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(key)) Then
										num2 = num2 + 1
									Else
										typeSymbol = source
									End If
									If (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(value)) Then
										If (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(value)) Then
											num3 = num3 + 1
										Else
											typeSymbol1 = destination
										End If
									End If
								ElseIf (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(value)) Then
									typeSymbol1 = destination
								ElseIf (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsNarrowingConversion(value) OrElse source.IsNullableType() AndAlso destination.IsNullableType() AndAlso methodSymbol.ReturnType.IsIntrinsicType()) Then
									GoTo Label0
								End If
								applicable(item) = True
								num1 = num1 + 1
							Else
								bestMatch = methodSymbol
								applicable.Clear()
								num1 = 0
							End If
						End If
					End If
				ElseIf (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(key) AndAlso Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(value)) Then
					bestMatch = OverloadResolution.LeastGenericConversionOperator(bestMatch, methodSymbol, num, flag1)
					If (flag1 AndAlso num = 0) Then
						Exit Do
					End If
				End If
			Label0:
				num5 = num5 + 1
			Loop While num5 <= count
			opSet.Clip(num4)
			conversionKinds.Clip(num4)
			If (bestMatch Is Nothing) Then
				If (num1 > 0) Then
					If (num1 <= 1) Then
						Dim count1 As Integer = opSet.Count - 1
						Dim num6 As Integer = 0
						While num6 <= count1
							If (Not applicable(num6)) Then
								num6 = num6 + 1
							Else
								bestMatch = opSet(num6)
								Exit While
							End If
						End While
					Else
						Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) = Nothing
						If (typeSymbol Is Nothing) Then
							instance = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol).GetInstance()
							Dim count2 As Integer = opSet.Count - 1
							Dim num7 As Integer = 0
							Do
								If (applicable(num7) AndAlso (num2 = 0 OrElse Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(conversionKinds(num7).Key))) Then
									Dim parameters As ImmutableArray(Of ParameterSymbol) = opSet(num7).Parameters
									instance.Add(parameters(0).Type)
								End If
								num7 = num7 + 1
							Loop While num7 <= count2
							typeSymbol = If(num2 = 0, OverloadResolution.MostEncompassing(instance, useSiteInfo), OverloadResolution.MostEncompassed(instance, useSiteInfo))
						End If
						If (typeSymbol1 Is Nothing AndAlso typeSymbol IsNot Nothing) Then
							If (instance IsNot Nothing) Then
								instance.Clear()
							Else
								instance = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol).GetInstance()
							End If
							Dim count3 As Integer = opSet.Count - 1
							Dim num8 As Integer = 0
							Do
								If (applicable(num8) AndAlso (num3 = 0 OrElse Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(conversionKinds(num8).Value))) Then
									instance.Add(opSet(num8).ReturnType)
								End If
								num8 = num8 + 1
							Loop While num8 <= count3
							typeSymbol1 = If(num3 = 0, OverloadResolution.MostEncompassed(instance, useSiteInfo), OverloadResolution.MostEncompassing(instance, useSiteInfo))
						End If
						If (instance IsNot Nothing) Then
							instance.Free()
						End If
						If (typeSymbol IsNot Nothing AndAlso typeSymbol1 IsNot Nothing) Then
							bestMatch = OverloadResolution.ChooseMostSpecificConversionOperator(opSet, applicable, typeSymbol, typeSymbol1, flag1)
						End If
						If (bestMatch Is Nothing OrElse Not flag1) Then
							GoTo Label3
						End If
						bestMatch = Nothing
						flag = True
						Return flag
					End If
				End If
			Label3:
				flag = CObj(bestMatch) <> CObj(Nothing)
			Else
				If (flag1) Then
					bestMatch = Nothing
				End If
				flag = True
			End If
			Return flag
		Label1:
			conversionKinds(item) = New KeyValuePair(Of ConversionKind, ConversionKind)(key, value)
			GoTo Label4
		End Function

		Private Shared Function DetermineMostSpecificWideningConversion(ByVal source As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal destination As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal opSet As ArrayBuilder(Of MethodSymbol), ByVal conversionKinds As ArrayBuilder(Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.ConversionKind, Microsoft.CodeAnalysis.VisualBasic.ConversionKind)), <InAttribute> ByRef applicable As BitVector, <Out> ByRef bestMatch As MethodSymbol, ByVal suppressViabilityChecks As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			bestMatch = Nothing
			Dim num As Integer = -1
			Dim flag1 As Boolean = False
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
			Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
			Dim num1 As Integer = 0
			Dim num2 As Integer = 0
			Dim count As Integer = opSet.Count - 1
			Dim num3 As Integer = 0
			Do
				Dim item As MethodSymbol = opSet(num3)
				Dim keyValuePair As Integer = num2
				num2 = num2 + 1
				If (keyValuePair < num3) Then
					opSet(keyValuePair) = item
				End If
				If (OverloadResolution.IsWidening(item)) Then
					If (Not OverloadResolution.ClassifyConversionOperatorInOutConversions(source, destination, item, conversionKind, conversionKind1, suppressViabilityChecks, useSiteInfo)) Then
						num2 = keyValuePair
					Else
						conversionKinds(keyValuePair) = New KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.ConversionKind, Microsoft.CodeAnalysis.VisualBasic.ConversionKind)(conversionKind, conversionKind1)
						If (bestMatch Is Nothing) Then
							If (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(conversionKind)) Then
								If (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(conversionKind1)) Then
									If (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(conversionKind) OrElse Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(conversionKind1)) Then
										If (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(conversionKind)) Then
											typeSymbol = source
										End If
										If (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(conversionKind1)) Then
											typeSymbol1 = destination
										End If
										applicable(keyValuePair) = True
										num1 = num1 + 1
									Else
										bestMatch = item
										applicable.Clear()
										num1 = 0
									End If
								End If
							End If
						ElseIf (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(conversionKind) AndAlso Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(conversionKind1)) Then
							bestMatch = OverloadResolution.LeastGenericConversionOperator(bestMatch, item, num, flag1)
							If (flag1 AndAlso num = 0) Then
								Exit Do
							End If
						End If
					End If
				End If
				num3 = num3 + 1
			Loop While num3 <= count
			opSet.Clip(num2)
			conversionKinds.Clip(num2)
			If (bestMatch IsNot Nothing) Then
				If (flag1) Then
					bestMatch = Nothing
				End If
				flag = True
			ElseIf (num1 <= 0) Then
				flag = False
			Else
				If (num1 <= 1) Then
					Dim count1 As Integer = opSet.Count - 1
					Dim num4 As Integer = 0
					While num4 <= count1
						If (Not applicable(num4)) Then
							num4 = num4 + 1
						Else
							bestMatch = opSet(num4)
							Exit While
						End If
					End While
				Else
					Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) = Nothing
					If (typeSymbol Is Nothing) Then
						instance = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol).GetInstance()
						Dim count2 As Integer = opSet.Count - 1
						Dim num5 As Integer = 0
						Do
							If (applicable(num5)) Then
								Dim parameters As ImmutableArray(Of ParameterSymbol) = opSet(num5).Parameters
								instance.Add(parameters(0).Type)
							End If
							num5 = num5 + 1
						Loop While num5 <= count2
						typeSymbol = OverloadResolution.MostEncompassed(instance, useSiteInfo)
					End If
					If (typeSymbol1 Is Nothing AndAlso typeSymbol IsNot Nothing) Then
						If (instance IsNot Nothing) Then
							instance.Clear()
						Else
							instance = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol).GetInstance()
						End If
						Dim count3 As Integer = opSet.Count - 1
						Dim num6 As Integer = 0
						Do
							If (applicable(num6)) Then
								instance.Add(opSet(num6).ReturnType)
							End If
							num6 = num6 + 1
						Loop While num6 <= count3
						typeSymbol1 = OverloadResolution.MostEncompassing(instance, useSiteInfo)
					End If
					If (instance IsNot Nothing) Then
						instance.Free()
					End If
					If (typeSymbol IsNot Nothing AndAlso typeSymbol1 IsNot Nothing) Then
						bestMatch = OverloadResolution.ChooseMostSpecificConversionOperator(opSet, applicable, typeSymbol, typeSymbol1, flag1)
					End If
					If (bestMatch IsNot Nothing AndAlso flag1) Then
						bestMatch = Nothing
					End If
				End If
				flag = True
			End If
			Return flag
		End Function

		Public Shared Function DoesReceiverMatchInstance(ByVal instanceType As TypeSymbol, ByVal receiverType As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Return Microsoft.CodeAnalysis.VisualBasic.Conversions.HasWideningDirectCastConversionButNotEnumTypeConversion(instanceType, receiverType, useSiteInfo)
		End Function

		Private Shared Function DoQuickApplicabilityCheck(ByVal candidate As OverloadResolution.Candidate, ByVal typeArguments As ImmutableArray(Of TypeSymbol), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal isQueryOperatorInvocation As Boolean, ByVal forceExpandedForm As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As OverloadResolution.QuickApplicabilityInfo
			Dim quickApplicabilityInfo As OverloadResolution.QuickApplicabilityInfo
			Dim num As Integer = 0
			Dim num1 As Integer = 0
			Dim flag As Boolean = False
			If (isQueryOperatorInvocation AndAlso DirectCast(candidate.UnderlyingSymbol, MethodSymbol).IsSub) Then
				quickApplicabilityInfo = New OverloadResolution.QuickApplicabilityInfo()
			ElseIf (candidate.UnderlyingSymbol.HasUnsupportedMetadata) Then
				quickApplicabilityInfo = New OverloadResolution.QuickApplicabilityInfo(candidate, OverloadResolution.CandidateAnalysisResultState.HasUnsupportedMetadata, True, True)
			ElseIf (typeArguments.Length <= 0 OrElse candidate.Arity = typeArguments.Length) Then
				candidate.GetAllParameterCounts(num, num1, flag)
				If (Not isQueryOperatorInvocation) Then
					If (arguments.Length >= num AndAlso (flag OrElse arguments.Length <= num1)) Then
						GoTo Label1
					End If
					quickApplicabilityInfo = New OverloadResolution.QuickApplicabilityInfo(candidate, OverloadResolution.CandidateAnalysisResultState.ArgumentCountMismatch, Not flag, flag)
					Return quickApplicabilityInfo
				Else
					If (arguments.Length = num1) Then
						GoTo Label1
					End If
					quickApplicabilityInfo = New OverloadResolution.QuickApplicabilityInfo(candidate, OverloadResolution.CandidateAnalysisResultState.ArgumentCountMismatch, True, False)
					Return quickApplicabilityInfo
				End If
			Label1:
				Dim useSiteInfo1 As UseSiteInfo(Of AssemblySymbol) = candidate.UnderlyingSymbol.GetUseSiteInfo()
				useSiteInfo.Add(useSiteInfo1)
				If (useSiteInfo1.DiagnosticInfo Is Nothing) Then
					Dim flag1 As Boolean = False
					Dim flag2 As Boolean = False
					If (Not flag OrElse arguments.Length = num1 AndAlso Not forceExpandedForm) Then
						flag1 = True
					End If
					If (flag AndAlso Not isQueryOperatorInvocation) Then
						flag2 = True
					End If
					quickApplicabilityInfo = New OverloadResolution.QuickApplicabilityInfo(candidate, OverloadResolution.CandidateAnalysisResultState.Applicable, flag1, flag2)
				Else
					quickApplicabilityInfo = New OverloadResolution.QuickApplicabilityInfo(candidate, OverloadResolution.CandidateAnalysisResultState.HasUseSiteError, True, True)
				End If
			Else
				quickApplicabilityInfo = New OverloadResolution.QuickApplicabilityInfo(candidate, OverloadResolution.CandidateAnalysisResultState.BadGenericArity, True, True)
			End If
			Return quickApplicabilityInfo
		End Function

		Private Shared Function EliminateLessApplicableToTheArguments(ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal delegateReturnType As TypeSymbol, ByVal appliedTieBreakingRules As Boolean, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal mostApplicableMustNarrowOnlyFromNumericConstants As Boolean = False) As Integer
			Dim num As Integer
			Dim instance As ArrayBuilder(Of Integer) = ArrayBuilder(Of Integer).GetInstance()
			If (Not OverloadResolution.FastFindMostApplicableCandidates(candidates, arguments, instance, binder, useSiteInfo) OrElse mostApplicableMustNarrowOnlyFromNumericConstants AndAlso candidates(instance(0)).RequiresNarrowingNotFromNumericConstant AndAlso instance.Count <> OverloadResolution.CountApplicableCandidates(candidates)) Then
				num = If(appliedTieBreakingRules, OverloadResolution.CountApplicableCandidates(candidates), OverloadResolution.ApplyTieBreakingRulesToEquallyApplicableCandidates(candidates, arguments, delegateReturnType, binder, useSiteInfo))
			Else
				Dim num1 As Integer = 0
				Dim item As Integer = instance(num1)
				Dim count As Integer = candidates.Count - 1
				Dim num2 As Integer = 0
				Do
					If (num2 <> item) Then
						Dim candidateAnalysisResult As OverloadResolution.CandidateAnalysisResult = candidates(num2)
						If (candidateAnalysisResult.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
							candidateAnalysisResult.State = OverloadResolution.CandidateAnalysisResultState.LessApplicable
							candidates(num2) = candidateAnalysisResult
						End If
					Else
						num1 = num1 + 1
						item = If(num1 >= instance.Count, candidates.Count, instance(num1))
					End If
					num2 = num2 + 1
				Loop While num2 <= count
				num = If(appliedTieBreakingRules, instance.Count, OverloadResolution.ApplyTieBreakingRules(candidates, instance, arguments, delegateReturnType, binder, useSiteInfo))
			End If
			instance.Free()
			Return num
		End Function

		Private Shared Function EliminateNarrowingCandidates(ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult)) As Integer
			Dim num As Integer = 0
			Dim count As Integer = candidates.Count - 1
			Dim num1 As Integer = 0
			Do
				Dim item As OverloadResolution.CandidateAnalysisResult = candidates(num1)
				If (item.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
					If (Not item.RequiresNarrowingConversion) Then
						num = num + 1
					Else
						item.State = OverloadResolution.CandidateAnalysisResultState.RequiresNarrowing
						candidates(num1) = item
					End If
				End If
				num1 = num1 + 1
			Loop While num1 <= count
			Return num
		End Function

		Private Shared Function EliminateNotApplicableToArguments(ByVal methodOrPropertyGroup As BoundMethodOrPropertyGroup, ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal argumentNames As ImmutableArray(Of String), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <Out> ByRef applicableNarrowingCandidates As Integer, <InAttribute> <Out> ByRef asyncLambdaSubToFunctionMismatch As HashSet(Of BoundExpression), ByVal callerInfoOpt As SyntaxNode, ByVal forceExpandedForm As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Integer
			Dim num As Integer = 0
			Dim num1 As Integer = 0
			applicableNarrowingCandidates = 0
			Dim count As Integer = candidates.Count - 1
			Dim num2 As Integer = 0
			Do
				Dim item As OverloadResolution.CandidateAnalysisResult = candidates(num2)
				If (item.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
					If (Not item.ArgumentMatchingDone) Then
						OverloadResolution.MatchArguments(methodOrPropertyGroup, item, arguments, argumentNames, binder, asyncLambdaSubToFunctionMismatch, callerInfoOpt, forceExpandedForm, useSiteInfo)
						item.SetArgumentMatchingDone()
						candidates(num2) = item
					End If
					If (item.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
						num = num + 1
						If (item.RequiresNarrowingConversion) Then
							applicableNarrowingCandidates = applicableNarrowingCandidates + 1
						End If
						If (item.IsIllegalInAttribute) Then
							num1 = num1 + 1
						End If
					End If
				End If
				num2 = num2 + 1
			Loop While num2 <= count
			If (num1 > 0 AndAlso num > num1) Then
				Dim count1 As Integer = candidates.Count - 1
				For i As Integer = 0 To count1
					Dim candidateAnalysisResult As OverloadResolution.CandidateAnalysisResult = candidates(i)
					If (candidateAnalysisResult.State = OverloadResolution.CandidateAnalysisResultState.Applicable AndAlso candidateAnalysisResult.IsIllegalInAttribute) Then
						num = num - 1
						If (candidateAnalysisResult.RequiresNarrowingConversion) Then
							applicableNarrowingCandidates = applicableNarrowingCandidates - 1
						End If
						candidateAnalysisResult.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
						candidates(i) = candidateAnalysisResult
					End If
				Next

			End If
			Return num
		End Function

		Private Shared Function FastFindMostApplicableCandidates(ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal indexesOfMostApplicableCandidates As ArrayBuilder(Of Integer), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim num As Integer = -1
			Dim candidateAnalysisResult As OverloadResolution.CandidateAnalysisResult = New OverloadResolution.CandidateAnalysisResult()
			indexesOfMostApplicableCandidates.Clear()
			Dim count As Integer = candidates.Count - 1
			Dim num1 As Integer = 0
			Do
				Dim item As OverloadResolution.CandidateAnalysisResult = candidates(num1)
				If (item.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
					If (num <> -1) Then
						Dim theArguments As OverloadResolution.ApplicabilityComparisonResult = OverloadResolution.CompareApplicabilityToTheArguments(candidateAnalysisResult, item, arguments, binder, useSiteInfo)
						If (theArguments = OverloadResolution.ApplicabilityComparisonResult.RightIsMoreApplicable) Then
							num = num1
							candidateAnalysisResult = item
							indexesOfMostApplicableCandidates.Clear()
							indexesOfMostApplicableCandidates.Add(num1)
						ElseIf (theArguments = OverloadResolution.ApplicabilityComparisonResult.Undefined) Then
							num = -1
							indexesOfMostApplicableCandidates.Clear()
						ElseIf (theArguments = OverloadResolution.ApplicabilityComparisonResult.EquallyApplicable) Then
							indexesOfMostApplicableCandidates.Add(num1)
						End If
					Else
						num = num1
						candidateAnalysisResult = item
						indexesOfMostApplicableCandidates.Add(num1)
					End If
				End If
				num1 = num1 + 1
			Loop While num1 <= count
			Dim num2 As Integer = num - 1
			Dim num3 As Integer = 0
			Do
				Dim item1 As OverloadResolution.CandidateAnalysisResult = candidates(num3)
				If (item1.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
					Dim applicabilityComparisonResult As OverloadResolution.ApplicabilityComparisonResult = OverloadResolution.CompareApplicabilityToTheArguments(candidateAnalysisResult, item1, arguments, binder, useSiteInfo)
					If (applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.RightIsMoreApplicable OrElse applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.Undefined OrElse applicabilityComparisonResult = OverloadResolution.ApplicabilityComparisonResult.EquallyApplicable) Then
						num = -1
						Exit Do
					End If
				End If
				num3 = num3 + 1
			Loop While num3 <= num2
			Return num > -1
		End Function

		Private Shared Function FoldBooleanBinaryOperator(ByVal op As BinaryOperatorKind, ByVal left As ConstantValue, ByVal right As ConstantValue) As ConstantValue
			Dim booleanValue As Boolean = left.BooleanValue
			Dim flag As Boolean = right.BooleanValue
			Dim flag1 As Boolean = False
			Select Case op
				Case BinaryOperatorKind.Equals
					flag1 = booleanValue = flag
					Exit Select
				Case BinaryOperatorKind.NotEquals
					flag1 = booleanValue <> flag
					Exit Select
				Case BinaryOperatorKind.LessThanOrEqual
					flag1 = If(booleanValue, True, Not flag)
					Exit Select
				Case BinaryOperatorKind.GreaterThanOrEqual
					flag1 = If(Not booleanValue, True, flag)
					Exit Select
				Case BinaryOperatorKind.LessThan
					flag1 = If(Not booleanValue, False, Not flag)
					Exit Select
				Case BinaryOperatorKind.GreaterThan
					flag1 = If(booleanValue, False, flag)
					Exit Select
				Case BinaryOperatorKind.Subtract
				Case BinaryOperatorKind.Multiply
				Case BinaryOperatorKind.Power
				Case BinaryOperatorKind.Divide
				Case BinaryOperatorKind.Modulo
				Case BinaryOperatorKind.IntegerDivide
				Case BinaryOperatorKind.LeftShift
				Case BinaryOperatorKind.RightShift
					Throw ExceptionUtilities.UnexpectedValue(op)
				Case BinaryOperatorKind.[Xor]
					flag1 = booleanValue Xor flag
					Exit Select
				Case BinaryOperatorKind.[Or]
				Case BinaryOperatorKind.[OrElse]
					flag1 = If(booleanValue, True, flag)
					Exit Select
				Case BinaryOperatorKind.[And]
				Case BinaryOperatorKind.[AndAlso]
					flag1 = If(Not booleanValue, False, flag)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(op)
			End Select
			Return ConstantValue.Create(flag1)
		End Function

		Private Shared Function FoldDecimalBinaryOperator(ByVal op As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind, ByVal left As Microsoft.CodeAnalysis.ConstantValue, ByVal right As Microsoft.CodeAnalysis.ConstantValue, ByVal resultType As TypeSymbol, ByRef divideByZero As Boolean) As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			Dim num As [Decimal] = New Decimal()
			Dim decimalValue As [Decimal] = left.DecimalValue
			Dim decimalValue1 As [Decimal] = right.DecimalValue
			If (Not resultType.IsBooleanType()) Then
				Dim flag As Boolean = False
				Try
					Dim binaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind = op
					If (binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Add) Then
						num = [Decimal].Add(decimalValue, decimalValue1)
					Else
						Select Case binaryOperatorKind
							Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Subtract
								num = [Decimal].Subtract(decimalValue, decimalValue1)
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Multiply
								num = [Decimal].Multiply(decimalValue, decimalValue1)
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Power
								Throw ExceptionUtilities.UnexpectedValue(op)
							Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Divide
								num = [Decimal].Divide(decimalValue, decimalValue1)
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Modulo
								num = [Decimal].Remainder(decimalValue, decimalValue1)
								Exit Select
							Case Else
								Throw ExceptionUtilities.UnexpectedValue(op)
						End Select
					End If
				Catch overflowException As System.OverflowException
					ProjectData.SetProjectError(overflowException)
					flag = True
					ProjectData.ClearProjectError()
				Catch divideByZeroException As System.DivideByZeroException
					ProjectData.SetProjectError(divideByZeroException)
					divideByZero = True
					ProjectData.ClearProjectError()
				End Try
				constantValue = If(flag OrElse divideByZero, Microsoft.CodeAnalysis.ConstantValue.Bad, Microsoft.CodeAnalysis.ConstantValue.Create(num))
			Else
				Dim flag1 As Boolean = False
				Dim num1 As Integer = decimalValue.CompareTo(decimalValue1)
				Select Case op
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Equals
						flag1 = num1 = 0
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.NotEquals
						flag1 = num1 <> 0
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThanOrEqual
						flag1 = num1 <= 0
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThanOrEqual
						flag1 = num1 >= 0
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThan
						flag1 = num1 < 0
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThan
						flag1 = num1 > 0
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(op)
				End Select
				constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(flag1)
			End If
			Return constantValue
		End Function

		Private Shared Function FoldFloatingBinaryOperator(ByVal op As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind, ByVal left As Microsoft.CodeAnalysis.ConstantValue, ByVal right As Microsoft.CodeAnalysis.ConstantValue, ByVal operandType As TypeSymbol, ByVal resultType As TypeSymbol) As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			Dim flag As Boolean
			Dim num As Double = If(operandType.IsSingleType(), CDbl(left.SingleValue), left.DoubleValue)
			Dim num1 As Double = If(operandType.IsSingleType(), CDbl(right.SingleValue), right.DoubleValue)
			If (Not resultType.IsBooleanType()) Then
				Dim num2 As Double = 0
				Dim binaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind = op
				If (binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Add) Then
					num2 = num + num1
				Else
					Select Case binaryOperatorKind
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Subtract
							num2 = num - num1
							GoTo Label0
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Multiply
							num2 = num * num1
							GoTo Label0
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Power
							If (Not [Double].IsInfinity(num1)) Then
								If ([Double].IsNaN(num1)) Then
									num2 = NaN
									GoTo Label0
								End If
							ElseIf (num.Equals(1)) Then
								num2 = num
								GoTo Label0
							ElseIf (num.Equals(-1)) Then
								num2 = NaN
								GoTo Label0
							End If
							num2 = Math.Pow(num, num1)
							GoTo Label0
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Divide
							num2 = num / num1
							GoTo Label0
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Modulo
							num2 = num Mod num1
							GoTo Label0
					End Select
					Throw ExceptionUtilities.UnexpectedValue(op)
				End If
			Label0:
				Dim flag1 As Boolean = False
				num2 = CompileTimeCalculations.NarrowFloatingResult(num2, resultType.GetConstantValueTypeDiscriminator(), flag1)
				constantValue = If(Not resultType.IsSingleType(), Microsoft.CodeAnalysis.ConstantValue.Create(num2), Microsoft.CodeAnalysis.ConstantValue.Create(CSng(num2)))
			Else
				Select Case op
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Equals
						flag = num = num1
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.NotEquals
						flag = num <> num1
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThanOrEqual
						flag = num <= num1
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThanOrEqual
						flag = num >= num1
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThan
						flag = num < num1
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThan
						flag = num > num1
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(op)
				End Select
				constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(flag)
			End If
			Return constantValue
		End Function

		Private Shared Function FoldIntegralCharOrDateTimeBinaryOperator(ByVal op As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind, ByVal left As Microsoft.CodeAnalysis.ConstantValue, ByVal right As Microsoft.CodeAnalysis.ConstantValue, ByVal operandType As TypeSymbol, ByVal resultType As TypeSymbol, ByRef integerOverflow As Boolean, ByRef divideByZero As Boolean) As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			Dim flag As Boolean
			Dim shiftSizeMask As Long = 0L
			Dim constantValueAsInt64 As Long = CompileTimeCalculations.GetConstantValueAsInt64(left)
			Dim num As Long = CompileTimeCalculations.GetConstantValueAsInt64(right)
			If (Not resultType.IsBooleanType()) Then
				Dim constantValueTypeDiscriminator As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator = operandType.GetConstantValueTypeDiscriminator()
				Dim constantValueTypeDiscriminator1 As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator = resultType.GetConstantValueTypeDiscriminator()
				Dim binaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind = op
				If (binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Add) Then
					shiftSizeMask = CompileTimeCalculations.NarrowIntegralResult(constantValueAsInt64 + num, constantValueTypeDiscriminator, constantValueTypeDiscriminator1, integerOverflow)
					If (Not resultType.IsUnsignedIntegralType()) Then
						If (num > CLng(0) AndAlso shiftSizeMask < constantValueAsInt64 OrElse num < CLng(0) AndAlso shiftSizeMask > constantValueAsInt64) Then
							integerOverflow = True
						End If
					ElseIf (CompileTimeCalculations.UncheckedCULng(shiftSizeMask) < CompileTimeCalculations.UncheckedCULng(constantValueAsInt64)) Then
						integerOverflow = True
					End If
				Else
					Select Case binaryOperatorKind
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Subtract
							shiftSizeMask = CompileTimeCalculations.NarrowIntegralResult(constantValueAsInt64 - num, constantValueTypeDiscriminator, constantValueTypeDiscriminator1, integerOverflow)
							If (resultType.IsUnsignedIntegralType()) Then
								If (CompileTimeCalculations.UncheckedCULng(shiftSizeMask) <= CompileTimeCalculations.UncheckedCULng(constantValueAsInt64)) Then
									Exit Select
								End If
								integerOverflow = True
								Exit Select
							Else
								If ((num <= CLng(0) OrElse shiftSizeMask <= constantValueAsInt64) AndAlso (num >= CLng(0) OrElse shiftSizeMask >= constantValueAsInt64)) Then
									Exit Select
								End If
								integerOverflow = True
								Exit Select
							End If
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Multiply
							shiftSizeMask = CompileTimeCalculations.Multiply(constantValueAsInt64, num, constantValueTypeDiscriminator, constantValueTypeDiscriminator1, integerOverflow)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Power
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Divide
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[OrElse]
							Throw ExceptionUtilities.UnexpectedValue(op)
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Modulo
							If (num = 0) Then
								divideByZero = True
								Exit Select
							ElseIf (resultType.IsUnsignedIntegralType()) Then
								shiftSizeMask = CompileTimeCalculations.UncheckedCLng(CompileTimeCalculations.UncheckedCULng(constantValueAsInt64) Mod CompileTimeCalculations.UncheckedCULng(num))
								Exit Select
							ElseIf (num = CLng(-1)) Then
								shiftSizeMask = CLng(0)
								Exit Select
							Else
								shiftSizeMask = constantValueAsInt64 Mod num
								Exit Select
							End If
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.IntegerDivide
							If (num <> 0) Then
								shiftSizeMask = CompileTimeCalculations.NarrowIntegralResult(If(resultType.IsUnsignedIntegralType(), CompileTimeCalculations.UncheckedCLng(CompileTimeCalculations.UncheckedCULng(constantValueAsInt64) / CompileTimeCalculations.UncheckedCULng(num)), CompileTimeCalculations.UncheckedIntegralDiv(constantValueAsInt64, num)), constantValueTypeDiscriminator, constantValueTypeDiscriminator1, integerOverflow)
								If (resultType.IsUnsignedIntegralType() OrElse constantValueAsInt64 <> -9223372036854775808L OrElse num <> CLng(-1)) Then
									Exit Select
								End If
								integerOverflow = True
								Exit Select
							Else
								divideByZero = True
								Exit Select
							End If
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LeftShift
							shiftSizeMask = constantValueAsInt64 << (CInt(num) And CodeGenerator.GetShiftSizeMask(operandType) And 63)
							Dim flag1 As Boolean = False
							shiftSizeMask = CompileTimeCalculations.NarrowIntegralResult(shiftSizeMask, constantValueTypeDiscriminator, constantValueTypeDiscriminator1, flag1)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.RightShift
							If (Not resultType.IsUnsignedIntegralType()) Then
								shiftSizeMask = constantValueAsInt64 >> (CInt(num) And CodeGenerator.GetShiftSizeMask(operandType) And 63)
								Exit Select
							Else
								shiftSizeMask = CompileTimeCalculations.UncheckedCLng(CompileTimeCalculations.UncheckedCULng(constantValueAsInt64) >> (CInt(num) And CodeGenerator.GetShiftSizeMask(operandType) And 63))
								Exit Select
							End If
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Xor]
							shiftSizeMask = constantValueAsInt64 Xor num
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Or]
							shiftSizeMask = constantValueAsInt64 Or num
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[And]
							shiftSizeMask = constantValueAsInt64 And num
							Exit Select
						Case Else
							Throw ExceptionUtilities.UnexpectedValue(op)
					End Select
				End If
				constantValue = If(Not divideByZero, CompileTimeCalculations.GetConstantValue(constantValueTypeDiscriminator1, shiftSizeMask), Microsoft.CodeAnalysis.ConstantValue.Bad)
			Else
				Select Case op
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Equals
						flag = If(operandType.IsUnsignedIntegralType(), CompileTimeCalculations.UncheckedCULng(constantValueAsInt64) = CompileTimeCalculations.UncheckedCULng(num), constantValueAsInt64 = num)
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.NotEquals
						flag = If(operandType.IsUnsignedIntegralType(), CompileTimeCalculations.UncheckedCULng(constantValueAsInt64) <> CompileTimeCalculations.UncheckedCULng(num), constantValueAsInt64 <> num)
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThanOrEqual
						flag = If(operandType.IsUnsignedIntegralType(), CompileTimeCalculations.UncheckedCULng(constantValueAsInt64) <= CompileTimeCalculations.UncheckedCULng(num), constantValueAsInt64 <= num)
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThanOrEqual
						flag = If(operandType.IsUnsignedIntegralType(), CompileTimeCalculations.UncheckedCULng(constantValueAsInt64) >= CompileTimeCalculations.UncheckedCULng(num), constantValueAsInt64 >= num)
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThan
						flag = If(operandType.IsUnsignedIntegralType(), CompileTimeCalculations.UncheckedCULng(constantValueAsInt64) < CompileTimeCalculations.UncheckedCULng(num), constantValueAsInt64 < num)
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThan
						flag = If(operandType.IsUnsignedIntegralType(), CompileTimeCalculations.UncheckedCULng(constantValueAsInt64) > CompileTimeCalculations.UncheckedCULng(num), constantValueAsInt64 > num)
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(op)
				End Select
				constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(flag)
			End If
			Return constantValue
		End Function

		Private Shared Function FoldStringBinaryOperator(ByVal op As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind, ByVal left As Microsoft.CodeAnalysis.ConstantValue, ByVal right As Microsoft.CodeAnalysis.ConstantValue) As Microsoft.CodeAnalysis.ConstantValue
			Dim bad As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			Dim str As String
			Dim binaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind = op
			If (binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Concatenate) Then
				Dim rope As Microsoft.CodeAnalysis.Rope = If(left.IsNothing, Microsoft.CodeAnalysis.Rope.Empty, left.RopeValue)
				Dim rope1 As Microsoft.CodeAnalysis.Rope = If(right.IsNothing, Microsoft.CodeAnalysis.Rope.Empty, right.RopeValue)
				If (CLng(rope.Length) + CLng(rope1.Length) <= CLng(2147483647)) Then
					Try
						constantValue = Microsoft.CodeAnalysis.ConstantValue.CreateFromRope(Microsoft.CodeAnalysis.Rope.Concat(rope, rope1))
					Catch outOfMemoryException As System.OutOfMemoryException
						ProjectData.SetProjectError(outOfMemoryException)
						bad = Microsoft.CodeAnalysis.ConstantValue.Bad
						ProjectData.ClearProjectError()
						Return bad
					End Try
				Else
					bad = Microsoft.CodeAnalysis.ConstantValue.Bad
					Return bad
				End If
			Else
				If (CInt(binaryOperatorKind) - CInt(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Equals) > CInt(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.NotEquals)) Then
					Throw ExceptionUtilities.UnexpectedValue(op)
				End If
				str = If(left.IsNothing, [String].Empty, left.StringValue)
				Dim str1 As String = If(right.IsNothing, [String].Empty, right.StringValue)
				Dim flag As Boolean = False
				Dim num As Integer = [String].Compare(str, str1, StringComparison.Ordinal)
				Select Case op
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Equals
						flag = num = 0
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.NotEquals
						flag = num <> 0
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThanOrEqual
						flag = num <= 0
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThanOrEqual
						flag = num >= 0
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThan
						flag = num < 0
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThan
						flag = num > 0
						Exit Select
				End Select
				constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(flag)
			End If
			bad = constantValue
			Return bad
		End Function

		Friend Shared Function GetOperatorInfo(ByVal name As String) As OverloadResolution.OperatorInfo
			Dim operatorInfo As OverloadResolution.OperatorInfo
			Dim operatorInfo1 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo()
			operatorInfo = If(name.Length <= 3 OrElse Not CaseInsensitiveComparison.Equals("op_", name.Substring(0, 3)) OrElse Not OverloadResolution.s_operatorNames.TryGetValue(name, operatorInfo1), New OverloadResolution.OperatorInfo(), operatorInfo1)
			Return operatorInfo
		End Function

		Friend Shared Function GetOperatorTokenKind(ByVal name As String) As SyntaxKind
			Return OverloadResolution.GetOperatorTokenKind(OverloadResolution.GetOperatorInfo(name))
		End Function

		Friend Shared Function GetOperatorTokenKind(ByVal opInfo As OverloadResolution.OperatorInfo) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			syntaxKind = If(Not opInfo.IsUnary, OverloadResolution.GetOperatorTokenKind(opInfo.BinaryOperatorKind), OverloadResolution.GetOperatorTokenKind(opInfo.UnaryOperatorKind))
			Return syntaxKind
		End Function

		Friend Shared Function GetOperatorTokenKind(ByVal op As Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim unaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind = op
			If (unaryOperatorKind > Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Implicit) Then
				If (unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Explicit) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword
					Return syntaxKind
				End If
				If (unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.IsTrue) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword
				Else
					If (unaryOperatorKind <> Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.IsFalse) Then
						Throw ExceptionUtilities.UnexpectedValue(op)
					End If
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword
				End If
			Else
				Select Case unaryOperatorKind
					Case Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Plus
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Minus
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.[Not]
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
						Exit Select
					Case Else
						If (unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Implicit) Then
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword
							Return syntaxKind
						End If
						Throw ExceptionUtilities.UnexpectedValue(op)
				End Select
			End If
			Return syntaxKind
		End Function

		Friend Shared Function GetOperatorTokenKind(ByVal op As BinaryOperatorKind) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Select Case op
				Case BinaryOperatorKind.Add
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken
					Exit Select
				Case BinaryOperatorKind.Concatenate
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
					Exit Select
				Case BinaryOperatorKind.[Like]
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeKeyword
					Exit Select
				Case BinaryOperatorKind.Equals
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
					Exit Select
				Case BinaryOperatorKind.NotEquals
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanGreaterThanToken
					Exit Select
				Case BinaryOperatorKind.LessThanOrEqual
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanEqualsToken
					Exit Select
				Case BinaryOperatorKind.GreaterThanOrEqual
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanEqualsToken
					Exit Select
				Case BinaryOperatorKind.LessThan
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken
					Exit Select
				Case BinaryOperatorKind.GreaterThan
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
					Exit Select
				Case BinaryOperatorKind.Subtract
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken
					Exit Select
				Case BinaryOperatorKind.Multiply
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskToken
					Exit Select
				Case BinaryOperatorKind.Power
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaretToken
					Exit Select
				Case BinaryOperatorKind.Divide
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashToken
					Exit Select
				Case BinaryOperatorKind.Modulo
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModKeyword
					Exit Select
				Case BinaryOperatorKind.IntegerDivide
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BackslashToken
					Exit Select
				Case BinaryOperatorKind.LeftShift
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanToken
					Exit Select
				Case BinaryOperatorKind.RightShift
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanToken
					Exit Select
				Case BinaryOperatorKind.[Xor]
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword
					Exit Select
				Case BinaryOperatorKind.[Or]
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword
					Exit Select
				Case BinaryOperatorKind.[OrElse]
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword
					Exit Select
				Case BinaryOperatorKind.[And]
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword
					Exit Select
				Case BinaryOperatorKind.[AndAlso]
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndAlsoKeyword
					Exit Select
				Case BinaryOperatorKind.[Is]
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsKeyword
					Exit Select
				Case BinaryOperatorKind.[IsNot]
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsNotKeyword
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(op)
			End Select
			Return syntaxKind
		End Function

		Friend Shared Function GetParameterTypeFromVirtualSignature(ByRef candidate As OverloadResolution.CandidateAnalysisResult, ByVal paramIndex As Integer) As TypeSymbol
			Dim type As TypeSymbol = candidate.Candidate.Parameters(paramIndex).Type
			If (candidate.IsExpandedParamArrayForm AndAlso paramIndex = candidate.Candidate.ParameterCount - 1 AndAlso type.Kind = SymbolKind.ArrayType) Then
				type = DirectCast(type, ArrayTypeSymbol).ElementType
			End If
			Return type
		End Function

		Private Shared Function GetParameterTypeFromVirtualSignature(ByRef candidate As OverloadResolution.CandidateAnalysisResult, ByVal paramIndex As Integer, ByRef typeForGenericityCheck As TypeSymbol) As TypeSymbol
			Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = candidate.Candidate.Parameters(paramIndex)
			Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = parameterSymbol.OriginalDefinition
			If (originalDefinition.ContainingSymbol.Kind = SymbolKind.Method) Then
				Dim containingSymbol As MethodSymbol = DirectCast(originalDefinition.ContainingSymbol, MethodSymbol)
				If (containingSymbol.IsReducedExtensionMethod) Then
					originalDefinition = containingSymbol.ReducedFrom.Parameters(paramIndex + 1)
				End If
			End If
			Dim type As TypeSymbol = parameterSymbol.Type
			typeForGenericityCheck = originalDefinition.Type
			If (candidate.IsExpandedParamArrayForm AndAlso paramIndex = candidate.Candidate.ParameterCount - 1 AndAlso type.Kind = SymbolKind.ArrayType) Then
				type = DirectCast(type, ArrayTypeSymbol).ElementType
				typeForGenericityCheck = DirectCast(typeForGenericityCheck, ArrayTypeSymbol).ElementType
			End If
			Return type
		End Function

		Private Shared Function GetTypeToLookForOperatorsIn(ByVal type As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As TypeSymbol
			type = type.GetNullableUnderlyingTypeOrSelf()
			If (type.Kind = SymbolKind.TypeParameter) Then
				type = DirectCast(type, TypeParameterSymbol).GetNonInterfaceConstraint(useSiteInfo)
			End If
			Return type
		End Function

		Private Shared Function GroupEquallyApplicableCandidates(ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As ArrayBuilder(Of ArrayBuilder(Of Integer))
			Dim instance As ArrayBuilder(Of ArrayBuilder(Of Integer)) = ArrayBuilder(Of ArrayBuilder(Of Integer)).GetInstance()
			Dim count As Integer = candidates.Count - 1
			Dim num As Integer = 0
			Do
				Dim item As OverloadResolution.CandidateAnalysisResult = candidates(num)
				If (item.State = OverloadResolution.CandidateAnalysisResultState.Applicable AndAlso item.EquallyApplicableCandidatesBucket <= 0) Then
					item.EquallyApplicableCandidatesBucket = instance.Count + 1
					candidates(num) = item
					Dim nums As ArrayBuilder(Of Integer) = ArrayBuilder(Of Integer).GetInstance()
					nums.Add(num)
					instance.Add(nums)
					Dim count1 As Integer = candidates.Count - 1
					For i As Integer = num + 1 To count1
						Dim equallyApplicableCandidatesBucket As OverloadResolution.CandidateAnalysisResult = candidates(i)
						If (equallyApplicableCandidatesBucket.State = OverloadResolution.CandidateAnalysisResultState.Applicable AndAlso equallyApplicableCandidatesBucket.EquallyApplicableCandidatesBucket <= 0 AndAlso equallyApplicableCandidatesBucket.Candidate <> item.Candidate AndAlso OverloadResolution.CandidatesAreEquallyApplicableToArguments(item, equallyApplicableCandidatesBucket, arguments, binder)) Then
							equallyApplicableCandidatesBucket.EquallyApplicableCandidatesBucket = item.EquallyApplicableCandidatesBucket
							candidates(i) = equallyApplicableCandidatesBucket
							nums.Add(i)
						End If
					Next

				End If
				num = num + 1
			Loop While num <= count
			Return instance
		End Function

		Private Shared Function HaveNarrowingOnlyFromObjectCandidates(ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult)) As Boolean
			Dim flag As Boolean = False
			Dim count As Integer = candidates.Count - 1
			Dim num As Integer = 0
			While num <= count
				Dim item As OverloadResolution.CandidateAnalysisResult = candidates(num)
				If (item.State <> OverloadResolution.CandidateAnalysisResultState.Applicable OrElse item.RequiresNarrowingNotFromObject OrElse item.Candidate.IsExtensionMethod) Then
					num = num + 1
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Shared Function InferTypeArguments(ByRef candidate As OverloadResolution.CandidateAnalysisResult, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal argumentNames As ImmutableArray(Of String), ByVal delegateReturnType As TypeSymbol, ByVal delegateReturnTypeReferenceBoundNode As BoundNode, <InAttribute> <Out> ByRef asyncLambdaSubToFunctionMismatch As HashSet(Of BoundExpression), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim nums As ArrayBuilder(Of Integer) = Nothing
			Dim nums1 As ArrayBuilder(Of Integer) = Nothing
			OverloadResolution.BuildParameterToArgumentMap(candidate, arguments, argumentNames, nums, nums1)
			If (candidate.State <> OverloadResolution.CandidateAnalysisResultState.Applicable) Then
				candidate.SetSomeInferenceFailed()
			Else
				Dim typeSymbols As ImmutableArray(Of TypeSymbol) = New ImmutableArray(Of TypeSymbol)()
				Dim inferenceLevel As TypeArgumentInference.InferenceLevel = TypeArgumentInference.InferenceLevel.None
				Dim flag As Boolean = False
				Dim flag1 As Boolean = False
				Dim inferenceErrorReason As InferenceErrorReasons = InferenceErrorReasons.Other
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim syntaxNodeOrTokens As ImmutableArray(Of SyntaxNodeOrToken) = New ImmutableArray(Of SyntaxNodeOrToken)()
				Dim underlyingSymbol As MethodSymbol = DirectCast(candidate.Candidate.UnderlyingSymbol, MethodSymbol)
				Dim bitVector1 As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				If (Not TypeArgumentInference.Infer(underlyingSymbol, arguments, nums, nums1, delegateReturnType, delegateReturnTypeReferenceBoundNode, typeSymbols, inferenceLevel, flag, flag1, inferenceErrorReason, bitVector, syntaxNodeOrTokens, asyncLambdaSubToFunctionMismatch, useSiteInfo, candidate.TypeArgumentInferenceDiagnosticsOpt, bitVector1)) Then
					candidate.State = OverloadResolution.CandidateAnalysisResultState.TypeInferenceFailed
					If (flag1) Then
						candidate.SetSomeInferenceFailed()
					End If
					If (flag) Then
						candidate.SetAllFailedInferenceIsDueToObject()
						If (Not candidate.Candidate.IsExtensionMethod) Then
							candidate.IgnoreExtensionMethods = True
						End If
					End If
					candidate.SetInferenceErrorReasons(inferenceErrorReason)
					candidate.NotInferredTypeArguments = Microsoft.CodeAnalysis.BitVector.Create(typeSymbols.Length)
					Dim length As Integer = typeSymbols.Length - 1
					For i As Integer = 0 To length
						If (typeSymbols(i) Is Nothing) Then
							candidate.NotInferredTypeArguments(i) = True
						End If
					Next

				Else
					candidate.SetInferenceLevel(inferenceLevel)
					candidate.Candidate = candidate.Candidate.Construct(typeSymbols)
					If (binder.OptionStrict = OptionStrict.[On] AndAlso Not bitVector.IsNull) Then
						Dim num As Integer = typeSymbols.Length - 1
						For j As Integer = 0 To num
							If (bitVector(j)) Then
								Dim typeArgumentInferenceDiagnosticsOpt As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = candidate.TypeArgumentInferenceDiagnosticsOpt
								If (typeArgumentInferenceDiagnosticsOpt Is Nothing) Then
									typeArgumentInferenceDiagnosticsOpt = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Create(True, useSiteInfo.AccumulatesDependencies)
									candidate.TypeArgumentInferenceDiagnosticsOpt = typeArgumentInferenceDiagnosticsOpt
								End If
								Dim item As SyntaxNodeOrToken = syntaxNodeOrTokens(j)
								Dim originalDefinition(2) As [Object]
								Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = candidate.Candidate.TypeParameters
								originalDefinition(0) = typeParameters(j)
								originalDefinition(1) = DirectCast(candidate.Candidate.UnderlyingSymbol, MethodSymbol).OriginalDefinition
								originalDefinition(2) = typeSymbols(j)
								Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(typeArgumentInferenceDiagnosticsOpt, item, ERRID.WRN_TypeInferenceAssumed3, originalDefinition)
							End If
						Next

					End If
				End If
			End If
			If (nums1 IsNot Nothing) Then
				nums1.Free()
			End If
			If (nums IsNot Nothing) Then
				nums.Free()
			End If
			Return candidate.State = OverloadResolution.CandidateAnalysisResultState.Applicable
		End Function

		Private Shared Sub InferTypeArgumentsIfNeedToAndCombineWithExistingCandidates(ByVal results As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult), ByVal newCandidate As OverloadResolution.CandidateAnalysisResult, ByVal typeArguments As ImmutableArray(Of TypeSymbol), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal argumentNames As ImmutableArray(Of String), ByVal delegateReturnType As TypeSymbol, ByVal delegateReturnTypeReferenceBoundNode As BoundNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef asyncLambdaSubToFunctionMismatch As HashSet(Of BoundExpression), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			If (typeArguments.Length = 0 AndAlso newCandidate.Candidate.Arity > 0 AndAlso Not OverloadResolution.InferTypeArguments(newCandidate, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, asyncLambdaSubToFunctionMismatch, binder, useSiteInfo)) Then
				results.Add(newCandidate)
				Return
			End If
			OverloadResolution.CombineCandidates(results, newCandidate, arguments.Length, argumentNames, useSiteInfo)
		End Sub

		Private Shared Function IsConversionOperatorViableBasedOnTypesInvolved(ByVal method As MethodSymbol, ByVal inputType As TypeSymbol, ByVal outputType As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)
			If (inputType.IsErrorType() OrElse outputType.IsErrorType()) Then
				flag = False
			Else
				If (Not method.ContainingType.IsDefinition) Then
					compoundUseSiteInfo = If(useSiteInfo.AccumulatesDependencies, New CompoundUseSiteInfo(Of AssemblySymbol)(useSiteInfo.AssemblyBeingBuilt), CompoundUseSiteInfo(Of AssemblySymbol).DiscardedDependencies)
					If (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.ConversionExists(Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyPredefinedConversion(inputType, outputType, compoundUseSiteInfo)) AndAlso compoundUseSiteInfo.Diagnostics.IsNullOrEmpty()) Then
						GoTo Label1
					End If
					useSiteInfo.MergeAndClear(compoundUseSiteInfo)
					flag = False
					Return flag
				End If
			Label2:
				flag = True
			End If
			Return flag
		Label1:
			useSiteInfo.MergeAndClear(compoundUseSiteInfo)
			GoTo Label2
		End Function

		Private Shared Function IsUnwrappingNullable(ByVal conv As ConversionKind, ByVal sourceType As TypeSymbol, ByVal targetType As TypeSymbol) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.OverloadResolution::IsUnwrappingNullable(Microsoft.CodeAnalysis.VisualBasic.ConversionKind,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean IsUnwrappingNullable(Microsoft.CodeAnalysis.VisualBasic.ConversionKind,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function IsUnwrappingNullable(ByVal sourceType As TypeSymbol, ByVal targetType As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			If (sourceType Is Nothing) Then
				Return False
			End If
			Return OverloadResolution.IsUnwrappingNullable(Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyPredefinedConversion(sourceType, targetType, useSiteInfo), sourceType, targetType)
		End Function

		Friend Shared Function IsValidInLiftedSignature(ByVal type As TypeSymbol) As Boolean
			Return type.IsNullableType()
		End Function

		Friend Shared Function IsWidening(ByVal method As MethodSymbol) As Boolean
			Dim flag As Boolean
			Dim name As Char = method.Name(3)
			flag = If(name = "I"C OrElse name = "i"C, True, False)
			Return flag
		End Function

		Private Shared Function IsWithinAppliedAttributeName(ByVal syntax As SyntaxNode) As Boolean
			Dim flag As Boolean
			Dim parent As SyntaxNode = syntax.Parent
			While parent IsNot Nothing
				If (parent.Kind() <> SyntaxKind.Attribute) Then
					If (TypeOf parent Is ExpressionSyntax OrElse TypeOf parent Is StatementSyntax) Then
						Exit While
					End If
					parent = parent.Parent
				Else
					Dim span As TextSpan = DirectCast(parent, AttributeSyntax).Name.Span
					flag = span.Contains(syntax.Position)
					Return flag
				End If
			End While
			flag = False
			Return flag
		End Function

		Private Shared Function LeastGenericConversionOperator(ByVal method1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal method2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, <InAttribute> <Out> ByRef bestDegreeOfGenericity As Integer, <InAttribute> <Out> ByRef isAmbiguous As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			If (bestDegreeOfGenericity = -1) Then
				bestDegreeOfGenericity = OverloadResolution.DetermineConversionOperatorDegreeOfGenericity(method1)
			End If
			Dim num As Integer = OverloadResolution.DetermineConversionOperatorDegreeOfGenericity(method2)
			If (bestDegreeOfGenericity < num) Then
				methodSymbol = method1
			ElseIf (num >= bestDegreeOfGenericity) Then
				isAmbiguous = True
				methodSymbol = method1
			Else
				isAmbiguous = False
				bestDegreeOfGenericity = num
				methodSymbol = method2
			End If
			Return methodSymbol
		End Function

		Private Shared Function LiftParameterSymbol(ByVal param As ParameterSymbol, ByVal nullableOfT As NamedTypeSymbol) As ParameterSymbol
			Dim liftedParameterSymbol As ParameterSymbol
			If (Not param.IsDefinition) Then
				Dim originalDefinition As ParameterSymbol = param.OriginalDefinition
				liftedParameterSymbol = SubstitutedParameterSymbol.CreateMethodParameter(DirectCast(param.ContainingSymbol, SubstitutedMethodSymbol), New OverloadResolution.LiftedParameterSymbol(originalDefinition, nullableOfT.Construct(New TypeSymbol() { originalDefinition.Type })))
			Else
				liftedParameterSymbol = New OverloadResolution.LiftedParameterSymbol(param, nullableOfT.Construct(New TypeSymbol() { param.Type }))
			End If
			Return liftedParameterSymbol
		End Function

		Public Shared Function MapBinaryOperatorKind(ByVal opCode As SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind
			Dim binaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind
			Select Case opCode
				Case SyntaxKind.AddExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Add
					Exit Select
				Case SyntaxKind.SubtractExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Subtract
					Exit Select
				Case SyntaxKind.MultiplyExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Multiply
					Exit Select
				Case SyntaxKind.DivideExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Divide
					Exit Select
				Case SyntaxKind.IntegerDivideExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.IntegerDivide
					Exit Select
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.RaiseEventStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.NothingLiteralExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.DirectCastExpression
				Case SyntaxKind.List Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.OptionStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.InheritsStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.TrueLiteralExpression Or SyntaxKind.NothingLiteralExpression Or SyntaxKind.ParenthesizedExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.DirectCastExpression Or SyntaxKind.TryCastExpression
				Case SyntaxKind.IsExpression
				Case SyntaxKind.IsNotExpression
					Throw ExceptionUtilities.UnexpectedValue(opCode)
				Case SyntaxKind.ExponentiateExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Power
					Exit Select
				Case SyntaxKind.LeftShiftExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LeftShift
					Exit Select
				Case SyntaxKind.RightShiftExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.RightShift
					Exit Select
				Case SyntaxKind.ConcatenateExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Concatenate
					Exit Select
				Case SyntaxKind.ModuloExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Modulo
					Exit Select
				Case SyntaxKind.EqualsExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Equals
					Exit Select
				Case SyntaxKind.NotEqualsExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.NotEquals
					Exit Select
				Case SyntaxKind.LessThanExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThan
					Exit Select
				Case SyntaxKind.LessThanOrEqualExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThanOrEqual
					Exit Select
				Case SyntaxKind.GreaterThanOrEqualExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThanOrEqual
					Exit Select
				Case SyntaxKind.GreaterThanExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThan
					Exit Select
				Case SyntaxKind.LikeExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Like]
					Exit Select
				Case SyntaxKind.OrExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Or]
					Exit Select
				Case SyntaxKind.ExclusiveOrExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Xor]
					Exit Select
				Case SyntaxKind.AndExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[And]
					Exit Select
				Case SyntaxKind.OrElseExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[OrElse]
					Exit Select
				Case SyntaxKind.AndAlsoExpression
					binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[AndAlso]
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(opCode)
			End Select
			Return binaryOperatorKind
		End Function

		Public Shared Function MapUnaryOperatorKind(ByVal opCode As SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind
			Dim unaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind
			Select Case opCode
				Case SyntaxKind.UnaryPlusExpression
					unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Plus
					Exit Select
				Case SyntaxKind.UnaryMinusExpression
					unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Minus
					Exit Select
				Case SyntaxKind.NotExpression
					unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.[Not]
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(opCode)
			End Select
			Return unaryOperatorKind
		End Function

		Private Shared Sub MatchArguments(ByVal methodOrPropertyGroup As BoundMethodOrPropertyGroup, ByRef candidate As OverloadResolution.CandidateAnalysisResult, ByVal arguments As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal argumentNames As ImmutableArray(Of String), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef asyncLambdaSubToFunctionMismatch As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal callerInfoOpt As SyntaxNode, ByVal forceExpandedForm As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim item As Integer
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim item1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim nums As ArrayBuilder(Of Integer) = Nothing
			Dim nums1 As ArrayBuilder(Of Integer) = Nothing
			Dim identity As KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)() = Nothing
			Dim keyValuePairArray As KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)() = Nothing
			Dim optionalArgument As OverloadResolution.OptionalArgument() = Nothing
			Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Nothing
			OverloadResolution.BuildParameterToArgumentMap(candidate, arguments, argumentNames, nums, nums1)
			If (candidate.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
				If (Not candidate.Candidate.IsExtensionMethod) Then
					candidate.IgnoreExtensionMethods = True
				End If
				Dim underlyingSymbol As Symbol = candidate.Candidate.UnderlyingSymbol
				If (underlyingSymbol.Kind = SymbolKind.Method) Then
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(underlyingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
					If (methodSymbol.IsGenericMethod) Then
						Dim typeParameterDiagnosticInfos As ArrayBuilder(Of TypeParameterDiagnosticInfo) = ArrayBuilder(Of TypeParameterDiagnosticInfo).GetInstance()
						Dim typeParameterDiagnosticInfos1 As ArrayBuilder(Of TypeParameterDiagnosticInfo) = Nothing
						Dim flag As Boolean = methodSymbol.CheckConstraints(typeParameterDiagnosticInfos, typeParameterDiagnosticInfos1, useSiteInfo)
						typeParameterDiagnosticInfos.Free()
						If (typeParameterDiagnosticInfos1 IsNot Nothing AndAlso typeParameterDiagnosticInfos1.Count > 0) Then
							Dim enumerator As ArrayBuilder(Of TypeParameterDiagnosticInfo).Enumerator = typeParameterDiagnosticInfos1.GetEnumerator()
							While enumerator.MoveNext()
								useSiteInfo.Add(enumerator.Current.UseSiteInfo)
							End While
						End If
						If (flag) Then
							GoTo Label1
						End If
						candidate.State = OverloadResolution.CandidateAnalysisResultState.GenericConstraintsViolated
						If (instance IsNot Nothing) Then
							instance.Free()
						End If
						If (nums1 IsNot Nothing) Then
							nums1.Free()
						End If
						If (identity IsNot Nothing) Then
							candidate.ConversionsOpt = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol))(identity)
						End If
						If (keyValuePairArray IsNot Nothing) Then
							candidate.ConversionsBackOpt = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol))(keyValuePairArray)
						End If
						If (optionalArgument IsNot Nothing) Then
							candidate.OptionalArguments = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of OverloadResolution.OptionalArgument)(optionalArgument)
						End If
						If (nums IsNot Nothing) Then
							nums.Free()
						End If
						Return
					End If
				End If
			Label1:
				Dim kind As Boolean = candidate.Candidate.UnderlyingSymbol.Kind = SymbolKind.[Property]
				Dim parameterCount As Integer = candidate.Candidate.ParameterCount - 1
				For i As Integer = 0 To (candidate.State = OverloadResolution.CandidateAnalysisResultState.Applicable OrElse candidate.IgnoreExtensionMethods)
					Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = candidate.Candidate.Parameters(i)
					Dim isByRef As Boolean = parameterSymbol.IsByRef
					Dim type As TypeSymbol = parameterSymbol.Type
					If (Not parameterSymbol.IsParamArray OrElse i <> candidate.Candidate.ParameterCount - 1) Then
						item = nums(i)
						If (item = -1) Then
							boundExpression = Nothing
						Else
							boundExpression = arguments(item)
						End If
						Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundExpression
						Dim argumentForParameterDefaultValue As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
						If (boundExpression1 Is Nothing OrElse boundExpression1.Kind = BoundKind.OmittedArgument) Then
							If (instance IsNot Nothing) Then
								instance.Clear()
							Else
								instance = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
							End If
							Dim binder1 As Microsoft.CodeAnalysis.VisualBasic.Binder = binder
							Dim parameterSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = parameterSymbol
							Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundExpression1
							If (boundExpression2 Is Nothing) Then
								boundExpression2 = methodOrPropertyGroup
							End If
							argumentForParameterDefaultValue = binder1.GetArgumentForParameterDefaultValue(parameterSymbol1, boundExpression2.Syntax, instance, callerInfoOpt)
							If (argumentForParameterDefaultValue Is Nothing OrElse instance.HasAnyErrors()) Then
								candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
								candidate.IgnoreExtensionMethods = False
								Exit For
							Else
								argumentForParameterDefaultValue.SetWasCompilerGenerated()
								boundExpression1 = argumentForParameterDefaultValue
							End If
						End If
						If (type.Kind = SymbolKind.ErrorType) Then
							candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
						ElseIf (Not boundExpression1.HasErrors) Then
							Dim keyValuePair As KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = New KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)()
							Dim identity1 As KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = New KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)()
							If (Not isByRef OrElse kind OrElse argumentForParameterDefaultValue IsNot Nothing OrElse Not parameterSymbol.IsExplicitByRef AndAlso (boundExpression1.Type Is Nothing OrElse Not boundExpression1.Type.IsStringType())) Then
								identity1 = Microsoft.CodeAnalysis.VisualBasic.Conversions.Identity
								OverloadResolution.MatchArgumentToByValParameter(methodOrPropertyGroup, candidate, boundExpression1, type, binder, keyValuePair, asyncLambdaSubToFunctionMismatch, useSiteInfo, argumentForParameterDefaultValue IsNot Nothing)
							Else
								OverloadResolution.MatchArgumentToByRefParameter(methodOrPropertyGroup, candidate, boundExpression1, type, binder, keyValuePair, identity1, asyncLambdaSubToFunctionMismatch, useSiteInfo)
							End If
							If (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(keyValuePair.Key)) Then
								If (identity Is Nothing) Then
									ReDim identity(arguments.Length - 1 + 1 - 1)
									Dim length As Integer = CInt(identity.Length) - 1
									For j As Integer = 0 To length
										identity(j) = Microsoft.CodeAnalysis.VisualBasic.Conversions.Identity
									Next

								End If
								If (argumentForParameterDefaultValue Is Nothing) Then
									identity(item) = keyValuePair
								End If
							End If
							If (argumentForParameterDefaultValue IsNot Nothing) Then
								If (optionalArgument Is Nothing) Then
									ReDim optionalArgument(candidate.Candidate.ParameterCount - 1 + 1 - 1)
								End If
								optionalArgument(i) = New OverloadResolution.OptionalArgument(argumentForParameterDefaultValue, keyValuePair, instance.DependenciesBag.ToImmutableArray())
							End If
							If (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(identity1.Key)) Then
								If (keyValuePairArray Is Nothing) Then
									ReDim keyValuePairArray(arguments.Length - 1 + 1 - 1)
									Dim num As Integer = CInt(keyValuePairArray.Length) - 1
									For k As Integer = 0 To num
										keyValuePairArray(k) = Microsoft.CodeAnalysis.VisualBasic.Conversions.Identity
									Next

								End If
								keyValuePairArray(item) = identity1
							End If
						Else
							candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
							candidate.IgnoreExtensionMethods = False
							Exit For
						End If
					ElseIf (type.Kind <> SymbolKind.ArrayType) Then
						candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
						candidate.IgnoreExtensionMethods = False
						Exit For
					ElseIf (Not candidate.IsExpandedParamArrayForm) Then
						item = nums(i)
						If (item = -1) Then
							item1 = Nothing
						Else
							item1 = arguments(item)
						End If
						Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = item1
						Dim keyValuePair1 As KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = New KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)()
						If (boundExpression3 Is Nothing OrElse boundExpression3.HasErrors OrElse Not OverloadResolution.CanPassToParamArray(boundExpression3, type, keyValuePair1, binder, useSiteInfo)) Then
							candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
							candidate.IgnoreExtensionMethods = False
							Exit For
						ElseIf (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsNarrowingConversion(keyValuePair1.Key) AndAlso binder.OptionStrict = OptionStrict.[On]) Then
							candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
						ElseIf (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(keyValuePair1.Key)) Then
							If (identity Is Nothing) Then
								ReDim identity(arguments.Length - 1 + 1 - 1)
								Dim length1 As Integer = CInt(identity.Length) - 1
								For l As Integer = 0 To length1
									identity(l) = Microsoft.CodeAnalysis.VisualBasic.Conversions.Identity
								Next

							End If
							identity(item) = keyValuePair1
						End If
					ElseIf (nums1.Count <> 1 OrElse Not arguments(nums1(0)).IsNothingLiteral() OrElse forceExpandedForm) Then
						Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
						If (arrayTypeSymbol.IsSZArray) Then
							type = arrayTypeSymbol.ElementType
							If (type.Kind <> SymbolKind.ErrorType) Then
								Dim count As Integer = nums1.Count - 1
								Dim num1 As Integer = 0
								While num1 <= count
									Dim keyValuePair2 As KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = New KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)()
									If (Not arguments(nums1(num1)).HasErrors) Then
										If (OverloadResolution.MatchArgumentToByValParameter(methodOrPropertyGroup, candidate, arguments(nums1(num1)), type, binder, keyValuePair2, asyncLambdaSubToFunctionMismatch, useSiteInfo, False) AndAlso Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(keyValuePair2.Key)) Then
											If (identity Is Nothing) Then
												ReDim identity(arguments.Length - 1 + 1 - 1)
												Dim length2 As Integer = CInt(identity.Length) - 1
												For m As Integer = 0 To length2
													identity(m) = Microsoft.CodeAnalysis.VisualBasic.Conversions.Identity
												Next

											End If
											identity(nums1(num1)) = keyValuePair2
										End If
										num1 = num1 + 1
									Else
										candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
										candidate.IgnoreExtensionMethods = False
										If (instance IsNot Nothing) Then
											instance.Free()
										End If
										If (nums1 IsNot Nothing) Then
											nums1.Free()
										End If
										If (identity IsNot Nothing) Then
											candidate.ConversionsOpt = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol))(identity)
										End If
										If (keyValuePairArray IsNot Nothing) Then
											candidate.ConversionsBackOpt = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol))(keyValuePairArray)
										End If
										If (optionalArgument IsNot Nothing) Then
											candidate.OptionalArguments = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of OverloadResolution.OptionalArgument)(optionalArgument)
										End If
										If (nums IsNot Nothing) Then
											nums.Free()
										End If
										Return
									End If
								End While
							Else
								candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
							End If
						Else
							candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
							candidate.IgnoreExtensionMethods = False
							Exit For
						End If
					Else
						candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
						candidate.IgnoreExtensionMethods = False
						Exit For
					End If
				Next

			End If
			If (instance IsNot Nothing) Then
				instance.Free()
			End If
			If (nums1 IsNot Nothing) Then
				nums1.Free()
			End If
			If (identity IsNot Nothing) Then
				candidate.ConversionsOpt = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol))(identity)
			End If
			If (keyValuePairArray IsNot Nothing) Then
				candidate.ConversionsBackOpt = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol))(keyValuePairArray)
			End If
			If (optionalArgument IsNot Nothing) Then
				candidate.OptionalArguments = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of OverloadResolution.OptionalArgument)(optionalArgument)
			End If
			If (nums IsNot Nothing) Then
				nums.Free()
			End If
		End Sub

		Private Shared Sub MatchArgumentToByRefParameter(ByVal methodOrPropertyGroup As BoundMethodOrPropertyGroup, ByRef candidate As OverloadResolution.CandidateAnalysisResult, ByVal argument As BoundExpression, ByVal targetType As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <Out> ByRef outConversionKind As KeyValuePair(Of ConversionKind, MethodSymbol), <Out> ByRef outConversionBackKind As KeyValuePair(Of ConversionKind, MethodSymbol), <InAttribute> <Out> ByRef asyncLambdaSubToFunctionMismatch As HashSet(Of BoundExpression), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			If (Not argument.IsSupportingAssignment()) Then
				If (binder.Report_ERRID_ReadOnlyInClosure(argument)) Then
					candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
				End If
				outConversionBackKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.Identity
				OverloadResolution.MatchArgumentToByValParameter(methodOrPropertyGroup, candidate, argument, targetType, binder, outConversionKind, asyncLambdaSubToFunctionMismatch, useSiteInfo, False)
			Else
				If (argument.IsLValue AndAlso targetType.IsSameTypeIgnoringAll(argument.Type)) Then
					outConversionKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.Identity
					outConversionBackKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.Identity
					Return
				End If
				outConversionBackKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.Identity
				If (OverloadResolution.MatchArgumentToByValParameter(methodOrPropertyGroup, candidate, argument, targetType, binder, outConversionKind, asyncLambdaSubToFunctionMismatch, useSiteInfo, False)) Then
					Dim typeOfAssignmentTarget As TypeSymbol = argument.GetTypeOfAssignmentTarget()
					Dim keyValuePair As KeyValuePair(Of ConversionKind, MethodSymbol) = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyConversion(targetType, typeOfAssignmentTarget, useSiteInfo)
					outConversionBackKind = keyValuePair
					If (Microsoft.CodeAnalysis.VisualBasic.Conversions.NoConversion(keyValuePair.Key)) Then
						candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
						candidate.IgnoreExtensionMethods = False
						Return
					End If
					If (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsNarrowingConversion(keyValuePair.Key)) Then
						candidate.SetRequiresNarrowingConversion()
						candidate.SetRequiresNarrowingNotFromNumericConstant()
						If (binder.OptionStrict = OptionStrict.[On]) Then
							candidate.State = OverloadResolution.CandidateAnalysisResultState.ArgumentMismatch
							Return
						End If
						If (targetType.SpecialType <> SpecialType.System_Object) Then
							candidate.SetRequiresNarrowingNotFromObject()
						End If
					End If
					candidate.RegisterDelegateRelaxationLevel(keyValuePair.Key)
					Return
				End If
			End If
		End Sub

		Private Shared Function MatchArgumentToByValParameter(ByVal methodOrPropertyGroup As BoundMethodOrPropertyGroup, ByRef candidate As OverloadResolution.CandidateAnalysisResult, ByVal argument As BoundExpression, ByVal targetType As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <Out> ByRef outConversionKind As KeyValuePair(Of ConversionKind, MethodSymbol), <InAttribute> <Out> ByRef asyncLambdaSubToFunctionMismatch As HashSet(Of BoundExpression), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal isDefaultValueArgument As Boolean = False) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.OverloadResolution::MatchArgumentToByValParameter(Microsoft.CodeAnalysis.VisualBasic.BoundMethodOrPropertyGroup,Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/CandidateAnalysisResult&,Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,System.Collections.Generic.KeyValuePair`2<Microsoft.CodeAnalysis.VisualBasic.ConversionKind,Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol>&,System.Collections.Generic.HashSet`1<Microsoft.CodeAnalysis.VisualBasic.BoundExpression>&,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&,System.Boolean)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean MatchArgumentToByValParameter(Microsoft.CodeAnalysis.VisualBasic.BoundMethodOrPropertyGroup,Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/CandidateAnalysisResult&,Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,System.Collections.Generic.KeyValuePair<Microsoft.CodeAnalysis.VisualBasic.ConversionKind,Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol>&,System.Collections.Generic.HashSet<Microsoft.CodeAnalysis.VisualBasic.BoundExpression>&,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&,System.Boolean)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Shared Function MethodInvocationOverloadResolution(ByVal methodGroup As BoundMethodGroup, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal argumentNames As ImmutableArray(Of String), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal callerInfoOpt As SyntaxNode, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal includeEliminatedCandidates As Boolean = False, Optional ByVal delegateReturnType As TypeSymbol = Nothing, Optional ByVal delegateReturnTypeReferenceBoundNode As BoundNode = Nothing, Optional ByVal lateBindingIsAllowed As Boolean = True, Optional ByVal isQueryOperatorInvocation As Boolean = False, Optional ByVal forceExpandedForm As Boolean = False) As OverloadResolution.OverloadResolutionResult
			Dim overloadResolutionResult As OverloadResolution.OverloadResolutionResult
			Dim empty As ImmutableArray(Of TypeSymbol)
			If (methodGroup.TypeArgumentsOpt IsNot Nothing) Then
				empty = methodGroup.TypeArgumentsOpt.Arguments
			Else
				empty = ImmutableArray(Of TypeSymbol).Empty
			End If
			Dim typeSymbols As ImmutableArray(Of TypeSymbol) = empty
			If (typeSymbols.IsDefault) Then
				typeSymbols = ImmutableArray(Of TypeSymbol).Empty
			End If
			If (arguments.IsDefault) Then
				arguments = ImmutableArray(Of BoundExpression).Empty
			End If
			Dim instance As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult) = ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult).GetInstance()
			Dim candidates As ArrayBuilder(Of OverloadResolution.Candidate) = ArrayBuilder(Of OverloadResolution.Candidate).GetInstance()
			Dim instance1 As ArrayBuilder(Of OverloadResolution.Candidate) = ArrayBuilder(Of OverloadResolution.Candidate).GetInstance()
			Dim methods As ImmutableArray(Of MethodSymbol) = methodGroup.Methods
			If (Not methods.IsDefault) Then
				Dim enumerator As ImmutableArray(Of MethodSymbol).Enumerator = methods.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As MethodSymbol = enumerator.Current
					If (current.ReducedFrom IsNot Nothing) Then
						instance1.Add(New OverloadResolution.ExtensionMethodCandidate(current))
					Else
						candidates.Add(New OverloadResolution.MethodCandidate(current))
					End If
				End While
			End If
			Dim boundExpressions As HashSet(Of BoundExpression) = Nothing
			Dim num As Integer = 0
			Dim num1 As Integer = 0
			If (candidates.Count > 0) Then
				OverloadResolution.CollectOverloadedCandidates(binder, instance, candidates, typeSymbols, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, includeEliminatedCandidates, isQueryOperatorInvocation, forceExpandedForm, boundExpressions, useSiteInfo)
				num1 = OverloadResolution.EliminateNotApplicableToArguments(methodGroup, instance, arguments, argumentNames, binder, num, boundExpressions, callerInfoOpt, forceExpandedForm, useSiteInfo)
			End If
			candidates.Free()
			candidates = Nothing
			Dim flag As Boolean = False
			If (OverloadResolution.ShouldConsiderExtensionMethods(instance)) Then
				If (methodGroup.ResultKind = LookupResultKind.Good) Then
					methods = methodGroup.AdditionalExtensionMethods(useSiteInfo)
					Dim enumerator1 As ImmutableArray(Of MethodSymbol).Enumerator = methods.GetEnumerator()
					While enumerator1.MoveNext()
						instance1.Add(New OverloadResolution.ExtensionMethodCandidate(enumerator1.Current))
					End While
				End If
				If (instance1.Count > 0) Then
					flag = True
					OverloadResolution.CollectOverloadedCandidates(binder, instance, instance1, typeSymbols, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, includeEliminatedCandidates, isQueryOperatorInvocation, forceExpandedForm, boundExpressions, useSiteInfo)
				End If
			End If
			instance1.Free()
			overloadResolutionResult = If(num1 <> 0 OrElse flag, OverloadResolution.ResolveOverloading(methodGroup, instance, arguments, argumentNames, delegateReturnType, lateBindingIsAllowed, binder, boundExpressions, callerInfoOpt, forceExpandedForm, useSiteInfo), OverloadResolution.ReportOverloadResolutionFailedOrLateBound(instance, num1, If(Not lateBindingIsAllowed, False, binder.OptionStrict <> OptionStrict.[On]), boundExpressions))
			instance.Free()
			Return overloadResolutionResult
		End Function

		Public Shared Function MethodOrPropertyInvocationOverloadResolution(ByVal group As BoundMethodOrPropertyGroup, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal argumentNames As ImmutableArray(Of String), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal callerInfoOpt As SyntaxNode, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal includeEliminatedCandidates As Boolean = False, Optional ByVal forceExpandedForm As Boolean = False) As OverloadResolution.OverloadResolutionResult
			Dim overloadResolutionResult As OverloadResolution.OverloadResolutionResult
			overloadResolutionResult = If(group.Kind <> BoundKind.MethodGroup, OverloadResolution.PropertyInvocationOverloadResolution(DirectCast(group, BoundPropertyGroup), arguments, argumentNames, binder, callerInfoOpt, useSiteInfo, includeEliminatedCandidates), OverloadResolution.MethodInvocationOverloadResolution(DirectCast(group, BoundMethodGroup), arguments, argumentNames, binder, callerInfoOpt, useSiteInfo, includeEliminatedCandidates, Nothing, Nothing, True, False, forceExpandedForm))
			Return overloadResolutionResult
		End Function

		Private Shared Function MostEncompassed(ByVal typeSet As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
			Dim count As Integer = typeSet.Count - 1
			Dim num As Integer = 0
			Do
				Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeSet(num)
				If (typeSymbol Is Nothing OrElse Not typeSymbol.IsSameTypeIgnoringAll(item)) Then
					Dim count1 As Integer = typeSet.Count - 1
					Dim num1 As Integer = 0
					Do
						If (num <> num1 AndAlso Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyPredefinedConversion(item, typeSet(num1), useSiteInfo))) Then
							GoTo Label0
						End If
						num1 = num1 + 1
					Loop While num1 <= count1
					If (typeSymbol IsNot Nothing) Then
						typeSymbol = Nothing
						Exit Do
					Else
						typeSymbol = item
					End If
				End If
			Label0:
				num = num + 1
			Loop While num <= count
			Return typeSymbol
		End Function

		Private Shared Function MostEncompassing(ByVal typeSet As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
			Dim count As Integer = typeSet.Count - 1
			Dim num As Integer = 0
			Do
				Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeSet(num)
				If (typeSymbol Is Nothing OrElse Not typeSymbol.IsSameTypeIgnoringAll(item)) Then
					Dim count1 As Integer = typeSet.Count - 1
					Dim num1 As Integer = 0
					Do
						If (num <> num1 AndAlso Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyPredefinedConversion(typeSet(num1), item, useSiteInfo))) Then
							GoTo Label0
						End If
						num1 = num1 + 1
					Loop While num1 <= count1
					If (typeSymbol IsNot Nothing) Then
						typeSymbol = Nothing
						Exit Do
					Else
						typeSymbol = item
					End If
				End If
			Label0:
				num = num + 1
			Loop While num <= count
			Return typeSymbol
		End Function

		Private Shared Function OperatorInvocationOverloadResolution(ByVal opSet As ArrayBuilder(Of MethodSymbol), ByVal argument1 As BoundExpression, ByVal argument2 As BoundExpression, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal lateBindingIsAllowed As Boolean, ByVal includeEliminatedCandidates As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As OverloadResolution.OverloadResolutionResult
			Dim overloadResolutionResult As OverloadResolution.OverloadResolutionResult
			Dim strs As ImmutableArray(Of String)
			Dim boundExpressions As ImmutableArray(Of BoundExpression)
			If (opSet.Count <> 0) Then
				Dim specialType As NamedTypeSymbol = opSet(0).ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Nullable_T)
				Dim diagnosticInfo As Boolean = specialType.GetUseSiteInfo().DiagnosticInfo Is Nothing
				Dim instance As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult) = ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult).GetInstance()
				Dim enumerator As ArrayBuilder(Of MethodSymbol).Enumerator = opSet.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As MethodSymbol = enumerator.Current
					If (Not current.HasUnsupportedMetadata) Then
						Dim useSiteInfo1 As UseSiteInfo(Of AssemblySymbol) = current.GetUseSiteInfo()
						useSiteInfo.Add(useSiteInfo1)
						If (useSiteInfo1.DiagnosticInfo Is Nothing) Then
							Dim candidateAnalysisResult As OverloadResolution.CandidateAnalysisResult = New OverloadResolution.CandidateAnalysisResult(New OverloadResolution.OperatorCandidate(current))
							Dim parameterCount As Integer = current.ParameterCount
							strs = New ImmutableArray(Of String)()
							OverloadResolution.CombineCandidates(instance, candidateAnalysisResult, parameterCount, strs, useSiteInfo)
							If (Not diagnosticInfo) Then
								Continue While
							End If
							Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = current.Parameters(0)
							Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = item.Type
							Dim flag As Boolean = type.IsNullableType()
							Dim flag1 As Boolean = If(flag OrElse Not type.IsValueType, False, Not type.IsRestrictedType())
							Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = Nothing
							Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
							Dim flag2 As Boolean = False
							Dim flag3 As Boolean = False
							If (argument2 IsNot Nothing AndAlso Not flag) Then
								parameterSymbol = current.Parameters(1)
								typeSymbol = parameterSymbol.Type
								flag2 = typeSymbol.IsNullableType()
								flag3 = If(flag2 OrElse Not typeSymbol.IsValueType, False, Not typeSymbol.IsRestrictedType())
							End If
							If (Not flag1 AndAlso Not flag3 OrElse flag OrElse flag2) Then
								Continue While
							End If
							If (flag1) Then
								item = OverloadResolution.LiftParameterSymbol(item, specialType)
							End If
							If (flag3) Then
								parameterSymbol = OverloadResolution.LiftParameterSymbol(parameterSymbol, specialType)
							End If
							Dim returnType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = current.ReturnType
							If (OverloadResolution.CanLiftType(returnType)) Then
								returnType = specialType.Construct(New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol() { returnType })
							End If
							Dim candidateAnalysisResults As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult) = instance
							Dim candidateAnalysisResult1 As OverloadResolution.CandidateAnalysisResult = New OverloadResolution.CandidateAnalysisResult(New OverloadResolution.LiftedOperatorCandidate(current, If(argument2 Is Nothing, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)(item), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)(item, parameterSymbol)), returnType))
							Dim num As Integer = current.ParameterCount
							strs = New ImmutableArray(Of String)()
							OverloadResolution.CombineCandidates(candidateAnalysisResults, candidateAnalysisResult1, num, strs, useSiteInfo)
						Else
							If (Not includeEliminatedCandidates) Then
								Continue While
							End If
							instance.Add(New OverloadResolution.CandidateAnalysisResult(New OverloadResolution.OperatorCandidate(current), OverloadResolution.CandidateAnalysisResultState.HasUseSiteError))
						End If
					Else
						If (Not includeEliminatedCandidates) Then
							Continue While
						End If
						instance.Add(New OverloadResolution.CandidateAnalysisResult(New OverloadResolution.OperatorCandidate(current), OverloadResolution.CandidateAnalysisResultState.HasUnsupportedMetadata))
					End If
				End While
				Dim boundMethodGroup As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup = New Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup(argument1.Syntax, Nothing, ImmutableArray(Of MethodSymbol).Empty, LookupResultKind.Good, Nothing, QualificationKind.Unqualified, False)
				Dim candidateAnalysisResults1 As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult) = instance
				boundExpressions = If(argument2 Is Nothing, ImmutableArray.Create(Of BoundExpression)(argument1), ImmutableArray.Create(Of BoundExpression)(argument1, argument2))
				strs = New ImmutableArray(Of String)()
				Dim boundExpressions1 As HashSet(Of BoundExpression) = Nothing
				Dim overloadResolutionResult1 As OverloadResolution.OverloadResolutionResult = OverloadResolution.ResolveOverloading(boundMethodGroup, candidateAnalysisResults1, boundExpressions, strs, Nothing, lateBindingIsAllowed, binder, boundExpressions1, Nothing, False, useSiteInfo)
				instance.Free()
				overloadResolutionResult = overloadResolutionResult1
			Else
				overloadResolutionResult = If(Not lateBindingIsAllowed, New OverloadResolution.OverloadResolutionResult(ImmutableArray(Of OverloadResolution.CandidateAnalysisResult).Empty, False, False, Nothing), New OverloadResolution.OverloadResolutionResult(ImmutableArray(Of OverloadResolution.CandidateAnalysisResult).Empty, If(argument1.Type.IsObjectType(), True, argument2.Type.IsObjectType()), False, Nothing))
			End If
			Return overloadResolutionResult
		End Function

		Private Shared Function OverloadedOperatorTargetsContainingType(ByVal containingType As NamedTypeSymbol, ByVal typeFromSignature As TypeSymbol) As Boolean
			Dim flag As Boolean
			flag = If(containingType.SpecialType <> SpecialType.System_Nullable_T, TypeSymbol.Equals(typeFromSignature.GetNullableUnderlyingTypeOrSelf().GetTupleUnderlyingTypeOrSelf(), containingType.GetTupleUnderlyingTypeOrSelf(), TypeCompareKind.ConsiderEverything), typeFromSignature = containingType)
			Return flag
		End Function

		Private Shared Function ParametersAreEquallyApplicableToArgument(ByVal leftParamType As TypeSymbol, ByVal rightParamType As TypeSymbol, ByVal argument As BoundExpression, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean = False
			Dim flag2 As Boolean = False
			If (leftParamType.IsSameTypeIgnoringAll(rightParamType)) Then
				flag = True
			Else
				If (argument IsNot Nothing) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = leftParamType.DelegateOrExpressionDelegate(binder, flag1)
					Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = rightParamType.DelegateOrExpressionDelegate(binder, flag2)
					If (namedTypeSymbol IsNot Nothing AndAlso namedTypeSymbol1 IsNot Nothing AndAlso (flag1 = flag2 OrElse argument.IsAnyLambda())) Then
						Dim delegateInvokeMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = namedTypeSymbol.DelegateInvokeMethod
						Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = namedTypeSymbol1.DelegateInvokeMethod
						If (delegateInvokeMethod Is Nothing OrElse delegateInvokeMethod.IsSub OrElse methodSymbol Is Nothing OrElse methodSymbol.IsSub) Then
							GoTo Label1
						End If
						Dim expression As BoundExpression = Nothing
						If (argument.Kind = BoundKind.QueryLambda) Then
							expression = DirectCast(argument, BoundQueryLambda).Expression
						End If
						flag = OverloadResolution.ParametersAreEquallyApplicableToArgument(delegateInvokeMethod.ReturnType, methodSymbol.ReturnType, expression, binder)
						Return flag
					End If
				End If
			Label1:
				flag = False
			End If
			Return flag
		End Function

		Public Shared Function PropertyInvocationOverloadResolution(ByVal propertyGroup As BoundPropertyGroup, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal argumentNames As ImmutableArray(Of String), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal callerInfoOpt As SyntaxNode, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal includeEliminatedCandidates As Boolean = False) As OverloadResolution.OverloadResolutionResult
			Dim properties As ImmutableArray(Of PropertySymbol) = propertyGroup.Properties
			If (arguments.IsDefault) Then
				arguments = ImmutableArray(Of BoundExpression).Empty
			End If
			Dim instance As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult) = ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult).GetInstance()
			Dim candidates As ArrayBuilder(Of OverloadResolution.Candidate) = ArrayBuilder(Of OverloadResolution.Candidate).GetInstance(properties.Length - 1)
			Dim length As Integer = properties.Length - 1
			Dim num As Integer = 0
			Do
				candidates.Add(New OverloadResolution.PropertyCandidate(properties(num)))
				num = num + 1
			Loop While num <= length
			Dim boundExpressions As HashSet(Of BoundExpression) = Nothing
			OverloadResolution.CollectOverloadedCandidates(binder, instance, candidates, ImmutableArray(Of TypeSymbol).Empty, arguments, argumentNames, Nothing, Nothing, includeEliminatedCandidates, False, False, boundExpressions, useSiteInfo)
			candidates.Free()
			Dim overloadResolutionResult As OverloadResolution.OverloadResolutionResult = OverloadResolution.ResolveOverloading(propertyGroup, instance, arguments, argumentNames, Nothing, True, binder, boundExpressions, callerInfoOpt, False, useSiteInfo)
			instance.Free()
			Return overloadResolutionResult
		End Function

		Public Shared Function QueryOperatorInvocationOverloadResolution(ByVal methodGroup As BoundMethodGroup, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal includeEliminatedCandidates As Boolean = False) As OverloadResolution.OverloadResolutionResult
			Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
			Return OverloadResolution.MethodInvocationOverloadResolution(methodGroup, arguments, strs, binder, Nothing, useSiteInfo, includeEliminatedCandidates, Nothing, Nothing, False, True, False)
		End Function

		Private Shared Function ReportOverloadResolutionFailedOrLateBound(ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult), ByVal applicableNarrowingCandidateCount As Integer, ByVal lateBindingIsAllowed As Boolean, ByVal asyncLambdaSubToFunctionMismatch As HashSet(Of BoundExpression)) As OverloadResolution.OverloadResolutionResult
			Dim current As OverloadResolution.CandidateAnalysisResult
			Dim flag As Boolean = False
			If (lateBindingIsAllowed) Then
				Dim enumerator As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult).Enumerator = candidates.GetEnumerator()
				Do
					If (Not enumerator.MoveNext()) Then
						Return New OverloadResolution.OverloadResolutionResult(candidates.ToImmutable(), flag, applicableNarrowingCandidateCount > 0, asyncLambdaSubToFunctionMismatch)
					End If
					current = enumerator.Current
				Loop While current.State <> OverloadResolution.CandidateAnalysisResultState.TypeInferenceFailed OrElse Not current.AllFailedInferenceIsDueToObject OrElse current.Candidate.IsExtensionMethod
				flag = True
			End If
			Return New OverloadResolution.OverloadResolutionResult(candidates.ToImmutable(), flag, applicableNarrowingCandidateCount > 0, asyncLambdaSubToFunctionMismatch)
		End Function

		Public Shared Function ResolveBinaryOperator(ByVal opCode As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind, ByVal left As BoundExpression, ByVal right As BoundExpression, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal considerUserDefinedOrLateBound As Boolean, <Out> ByRef intrinsicOperatorType As Microsoft.CodeAnalysis.SpecialType, <Out> ByRef userDefinedOperator As OverloadResolution.OverloadResolutionResult, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind
			Dim binaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind
			opCode = opCode And Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.OpMask
			intrinsicOperatorType = Microsoft.CodeAnalysis.SpecialType.None
			userDefinedOperator = New OverloadResolution.OverloadResolutionResult()
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
			Dim nullableUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type.GetNullableUnderlyingTypeOrSelf()
			Dim flag As Boolean = CObj(type) <> CObj(nullableUnderlyingTypeOrSelf)
			Dim nullableUnderlyingTypeOrSelf1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeSymbol.GetNullableUnderlyingTypeOrSelf()
			Dim flag1 As Boolean = CObj(typeSymbol) <> CObj(nullableUnderlyingTypeOrSelf1)
			Dim enumUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = nullableUnderlyingTypeOrSelf.GetEnumUnderlyingTypeOrSelf()
			Dim flag2 As Boolean = CObj(enumUnderlyingTypeOrSelf) <> CObj(nullableUnderlyingTypeOrSelf)
			Dim enumUnderlyingTypeOrSelf1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = nullableUnderlyingTypeOrSelf1.GetEnumUnderlyingTypeOrSelf()
			Dim flag3 As Boolean = CObj(enumUnderlyingTypeOrSelf1) <> CObj(nullableUnderlyingTypeOrSelf1)
			If ((flag2 OrElse flag) AndAlso (enumUnderlyingTypeOrSelf.IsStringType() OrElse enumUnderlyingTypeOrSelf.IsObjectType() OrElse enumUnderlyingTypeOrSelf.IsCharSZArray()) OrElse (flag3 OrElse flag1) AndAlso (enumUnderlyingTypeOrSelf1.IsStringType() OrElse enumUnderlyingTypeOrSelf1.IsObjectType() OrElse enumUnderlyingTypeOrSelf1.IsCharSZArray())) Then
				binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Error]
			ElseIf (Not OverloadResolution.UseUserDefinedBinaryOperators(opCode, type, typeSymbol)) Then
				Dim binaryOperatorKind1 As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Error]
				If (Not flag2 OrElse Not flag3 OrElse opCode <> Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Xor] AndAlso opCode <> Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[And] AndAlso opCode <> Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Or] OrElse Not nullableUnderlyingTypeOrSelf.IsSameTypeIgnoringAll(nullableUnderlyingTypeOrSelf1)) Then
					Dim specialType As Microsoft.CodeAnalysis.SpecialType = enumUnderlyingTypeOrSelf.SpecialType
					Dim specialType1 As Microsoft.CodeAnalysis.SpecialType = enumUnderlyingTypeOrSelf1.SpecialType
					If (specialType = Microsoft.CodeAnalysis.SpecialType.None AndAlso enumUnderlyingTypeOrSelf.IsCharSZArray()) Then
						specialType = Microsoft.CodeAnalysis.SpecialType.System_String
					End If
					If (specialType1 = Microsoft.CodeAnalysis.SpecialType.None AndAlso enumUnderlyingTypeOrSelf1.IsCharSZArray()) Then
						specialType1 = Microsoft.CodeAnalysis.SpecialType.System_String
					End If
					intrinsicOperatorType = OverloadResolution.ResolveNotLiftedIntrinsicBinaryOperator(opCode, specialType, specialType1)
					If (CSByte(intrinsicOperatorType) <> 0) Then
						binaryOperatorKind1 = opCode
					End If
					If (binaryOperatorKind1 <> Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Error] AndAlso (flag OrElse flag1) AndAlso CSByte(intrinsicOperatorType) <> 0 AndAlso CSByte(intrinsicOperatorType) <> 20 AndAlso CSByte(intrinsicOperatorType) <> 1 AndAlso opCode <> Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Concatenate AndAlso opCode <> Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Like]) Then
						binaryOperatorKind1 = binaryOperatorKind1 Or Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Lifted
					End If
				Else
					binaryOperatorKind1 = opCode
					If (flag OrElse flag1) Then
						binaryOperatorKind1 = binaryOperatorKind1 Or Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Lifted
					End If
				End If
				binaryOperatorKind = binaryOperatorKind1
			Else
				If (considerUserDefinedOrLateBound) Then
					If (type.CanContainUserDefinedOperators(useSiteInfo) OrElse typeSymbol.CanContainUserDefinedOperators(useSiteInfo) OrElse opCode = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Subtract AndAlso type.GetNullableUnderlyingTypeOrSelf().IsDateTimeType() AndAlso typeSymbol.GetNullableUnderlyingTypeOrSelf().IsDateTimeType()) Then
						userDefinedOperator = OverloadResolution.ResolveUserDefinedBinaryOperator(left, right, opCode, binder, useSiteInfo, False)
						If (Not userDefinedOperator.ResolutionIsLateBound) Then
							If (Not userDefinedOperator.BestResult.HasValue AndAlso userDefinedOperator.Candidates.Length = 0) Then
								Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
								userDefinedOperator = OverloadResolution.ResolveUserDefinedBinaryOperator(left, right, opCode, binder, discarded, True)
								If (userDefinedOperator.Candidates.Length <> 0) Then
									binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.UserDefined
									Return binaryOperatorKind
								End If
								binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Error]
								Return binaryOperatorKind
							End If
							binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.UserDefined
							Return binaryOperatorKind
						Else
							intrinsicOperatorType = Microsoft.CodeAnalysis.SpecialType.System_Object
							binaryOperatorKind = opCode
							Return binaryOperatorKind
						End If
					Else
						Dim flag4 As Boolean = False
						If (type.IsObjectType()) Then
							If (typeSymbol.IsTypeParameter() AndAlso DirectCast(typeSymbol, TypeParameterSymbol).GetNonInterfaceConstraint(useSiteInfo) Is Nothing) Then
								flag4 = True
							End If
						ElseIf (typeSymbol.IsObjectType() AndAlso type.IsTypeParameter() AndAlso DirectCast(type, TypeParameterSymbol).GetNonInterfaceConstraint(useSiteInfo) Is Nothing) Then
							flag4 = True
						End If
						If (Not flag4) Then
							GoTo Label2
						End If
						intrinsicOperatorType = Microsoft.CodeAnalysis.SpecialType.System_Object
						binaryOperatorKind = opCode
						Return binaryOperatorKind
					End If
				End If
			Label2:
				binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Error]
			End If
			Return binaryOperatorKind
		End Function

		Public Shared Function ResolveIsFalseOperator(ByVal argument As BoundExpression, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As OverloadResolution.OverloadResolutionResult
			Dim instance As ArrayBuilder(Of MethodSymbol) = ArrayBuilder(Of MethodSymbol).GetInstance()
			Dim type As TypeSymbol = argument.Type
			Dim operatorInfo As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(UnaryOperatorKind.IsFalse)
			Dim operatorInfo1 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo()
			OverloadResolution.CollectUserDefinedOperators(type, Nothing, MethodKind.UserDefinedOperator, "op_False", operatorInfo, Nothing, operatorInfo1, instance, useSiteInfo)
			Dim overloadResolutionResult As OverloadResolution.OverloadResolutionResult = OverloadResolution.OperatorInvocationOverloadResolution(instance, argument, Nothing, binder, False, False, useSiteInfo)
			instance.Free()
			Return overloadResolutionResult
		End Function

		Public Shared Function ResolveIsTrueOperator(ByVal argument As BoundExpression, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As OverloadResolution.OverloadResolutionResult
			Dim instance As ArrayBuilder(Of MethodSymbol) = ArrayBuilder(Of MethodSymbol).GetInstance()
			Dim type As TypeSymbol = argument.Type
			Dim operatorInfo As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(UnaryOperatorKind.IsTrue)
			Dim operatorInfo1 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo()
			OverloadResolution.CollectUserDefinedOperators(type, Nothing, MethodKind.UserDefinedOperator, "op_True", operatorInfo, Nothing, operatorInfo1, instance, useSiteInfo)
			Dim overloadResolutionResult As OverloadResolution.OverloadResolutionResult = OverloadResolution.OperatorInvocationOverloadResolution(instance, argument, Nothing, binder, False, False, useSiteInfo)
			instance.Free()
			Return overloadResolutionResult
		End Function

		Friend Shared Function ResolveNotLiftedIntrinsicBinaryOperator(ByVal opCode As BinaryOperatorKind, ByVal left As SpecialType, ByVal right As SpecialType) As SpecialType
			Dim table As SpecialType
			Dim tableKind As OverloadResolution.BinaryOperatorTables.TableKind
			Dim index As Nullable(Of Integer) = left.TypeToIndex()
			Dim nullable As Nullable(Of Integer) = right.TypeToIndex()
			If (Not index.HasValue OrElse Not nullable.HasValue) Then
				table = SpecialType.None
			Else
				Select Case opCode
					Case BinaryOperatorKind.Add
						tableKind = OverloadResolution.BinaryOperatorTables.TableKind.Addition
						Exit Select
					Case BinaryOperatorKind.Concatenate
					Case BinaryOperatorKind.[Like]
						tableKind = OverloadResolution.BinaryOperatorTables.TableKind.ConcatenationLike
						Exit Select
					Case BinaryOperatorKind.Equals
					Case BinaryOperatorKind.NotEquals
					Case BinaryOperatorKind.LessThanOrEqual
					Case BinaryOperatorKind.GreaterThanOrEqual
					Case BinaryOperatorKind.LessThan
					Case BinaryOperatorKind.GreaterThan
						tableKind = OverloadResolution.BinaryOperatorTables.TableKind.Relational
						Exit Select
					Case BinaryOperatorKind.Subtract
					Case BinaryOperatorKind.Multiply
					Case BinaryOperatorKind.Modulo
						tableKind = OverloadResolution.BinaryOperatorTables.TableKind.SubtractionMultiplicationModulo
						Exit Select
					Case BinaryOperatorKind.Power
						tableKind = OverloadResolution.BinaryOperatorTables.TableKind.Power
						Exit Select
					Case BinaryOperatorKind.Divide
						tableKind = OverloadResolution.BinaryOperatorTables.TableKind.Division
						Exit Select
					Case BinaryOperatorKind.IntegerDivide
						tableKind = OverloadResolution.BinaryOperatorTables.TableKind.IntegerDivision
						Exit Select
					Case BinaryOperatorKind.LeftShift
					Case BinaryOperatorKind.RightShift
						tableKind = OverloadResolution.BinaryOperatorTables.TableKind.Shift
						Exit Select
					Case BinaryOperatorKind.[Xor]
					Case BinaryOperatorKind.[Or]
					Case BinaryOperatorKind.[And]
						tableKind = OverloadResolution.BinaryOperatorTables.TableKind.Bitwise
						Exit Select
					Case BinaryOperatorKind.[OrElse]
					Case BinaryOperatorKind.[AndAlso]
						tableKind = OverloadResolution.BinaryOperatorTables.TableKind.Logical
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(opCode)
				End Select
				table = OverloadResolution.BinaryOperatorTables.Table(CInt(tableKind), index.Value, nullable.Value)
			End If
			Return table
		End Function

		Friend Shared Function ResolveNotLiftedIntrinsicUnaryOperator(ByVal opCode As UnaryOperatorKind, ByVal operandSpecialType As Microsoft.CodeAnalysis.SpecialType) As Microsoft.CodeAnalysis.SpecialType
			Dim specialType As Microsoft.CodeAnalysis.SpecialType
			Select Case opCode
				Case UnaryOperatorKind.Plus
					Select Case operandSpecialType
						Case Microsoft.CodeAnalysis.SpecialType.System_Object
						Case Microsoft.CodeAnalysis.SpecialType.System_SByte
						Case Microsoft.CodeAnalysis.SpecialType.System_Byte
						Case Microsoft.CodeAnalysis.SpecialType.System_Int16
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt16
						Case Microsoft.CodeAnalysis.SpecialType.System_Int32
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt32
						Case Microsoft.CodeAnalysis.SpecialType.System_Int64
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt64
						Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
						Case Microsoft.CodeAnalysis.SpecialType.System_Single
						Case Microsoft.CodeAnalysis.SpecialType.System_Double
							specialType = operandSpecialType

						Case Microsoft.CodeAnalysis.SpecialType.System_Enum
						Case Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate
						Case Microsoft.CodeAnalysis.SpecialType.System_Delegate
						Case Microsoft.CodeAnalysis.SpecialType.System_ValueType
						Case Microsoft.CodeAnalysis.SpecialType.System_Void
						Case Microsoft.CodeAnalysis.SpecialType.System_Char
						Label1:
							specialType = Microsoft.CodeAnalysis.SpecialType.None

						Case Microsoft.CodeAnalysis.SpecialType.System_Boolean
							specialType = Microsoft.CodeAnalysis.SpecialType.System_Int16

						Case Microsoft.CodeAnalysis.SpecialType.System_String
							specialType = Microsoft.CodeAnalysis.SpecialType.System_Double

						Case Else
							GoTo Label1
					End Select

				Case UnaryOperatorKind.Minus
					Select Case operandSpecialType
						Case Microsoft.CodeAnalysis.SpecialType.System_Object
						Case Microsoft.CodeAnalysis.SpecialType.System_SByte
						Case Microsoft.CodeAnalysis.SpecialType.System_Int16
						Case Microsoft.CodeAnalysis.SpecialType.System_Int32
						Case Microsoft.CodeAnalysis.SpecialType.System_Int64
						Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
						Case Microsoft.CodeAnalysis.SpecialType.System_Single
						Case Microsoft.CodeAnalysis.SpecialType.System_Double
							specialType = operandSpecialType

						Case Microsoft.CodeAnalysis.SpecialType.System_Enum
						Case Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate
						Case Microsoft.CodeAnalysis.SpecialType.System_Delegate
						Case Microsoft.CodeAnalysis.SpecialType.System_ValueType
						Case Microsoft.CodeAnalysis.SpecialType.System_Void
						Case Microsoft.CodeAnalysis.SpecialType.System_Char
						Label2:
							specialType = Microsoft.CodeAnalysis.SpecialType.None

						Case Microsoft.CodeAnalysis.SpecialType.System_Boolean
						Case Microsoft.CodeAnalysis.SpecialType.System_Byte
							specialType = Microsoft.CodeAnalysis.SpecialType.System_Int16

						Case Microsoft.CodeAnalysis.SpecialType.System_UInt16
							specialType = Microsoft.CodeAnalysis.SpecialType.System_Int32

						Case Microsoft.CodeAnalysis.SpecialType.System_UInt32
							specialType = Microsoft.CodeAnalysis.SpecialType.System_Int64

						Case Microsoft.CodeAnalysis.SpecialType.System_UInt64
							specialType = Microsoft.CodeAnalysis.SpecialType.System_Decimal

						Case Microsoft.CodeAnalysis.SpecialType.System_String
							specialType = Microsoft.CodeAnalysis.SpecialType.System_Double

						Case Else
							GoTo Label2
					End Select

				Case UnaryOperatorKind.[Not]
					Select Case operandSpecialType
						Case Microsoft.CodeAnalysis.SpecialType.System_Object
						Case Microsoft.CodeAnalysis.SpecialType.System_Boolean
						Case Microsoft.CodeAnalysis.SpecialType.System_SByte
						Case Microsoft.CodeAnalysis.SpecialType.System_Byte
						Case Microsoft.CodeAnalysis.SpecialType.System_Int16
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt16
						Case Microsoft.CodeAnalysis.SpecialType.System_Int32
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt32
						Case Microsoft.CodeAnalysis.SpecialType.System_Int64
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt64
							specialType = operandSpecialType

						Case Microsoft.CodeAnalysis.SpecialType.System_Enum
						Case Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate
						Case Microsoft.CodeAnalysis.SpecialType.System_Delegate
						Case Microsoft.CodeAnalysis.SpecialType.System_ValueType
						Case Microsoft.CodeAnalysis.SpecialType.System_Void
						Case Microsoft.CodeAnalysis.SpecialType.System_Char
						Label3:
							specialType = Microsoft.CodeAnalysis.SpecialType.None

						Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
						Case Microsoft.CodeAnalysis.SpecialType.System_Single
						Case Microsoft.CodeAnalysis.SpecialType.System_Double
						Case Microsoft.CodeAnalysis.SpecialType.System_String
							specialType = Microsoft.CodeAnalysis.SpecialType.System_Int64

						Case Else
							GoTo Label3
					End Select

				Case Else
					Throw ExceptionUtilities.UnexpectedValue(opCode)
			End Select
			Return specialType
		End Function

		Private Shared Function ResolveOverloading(ByVal methodOrPropertyGroup As BoundMethodOrPropertyGroup, ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal argumentNames As ImmutableArray(Of String), ByVal delegateReturnType As TypeSymbol, ByVal lateBindingIsAllowed As Boolean, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef asyncLambdaSubToFunctionMismatch As HashSet(Of BoundExpression), ByVal callerInfoOpt As SyntaxNode, ByVal forceExpandedForm As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As OverloadResolution.OverloadResolutionResult
			Dim overloadResolutionResult As OverloadResolution.OverloadResolutionResult
			Dim candidateAnalysisResults As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult)
			Dim num As Integer
			Dim flag As Boolean
			Dim flag1 As Boolean = False
			Dim flag2 As Boolean = False
			Dim num1 As Integer = 0
			Dim theArguments As Integer = OverloadResolution.EliminateNotApplicableToArguments(methodOrPropertyGroup, candidates, arguments, argumentNames, binder, num1, asyncLambdaSubToFunctionMismatch, callerInfoOpt, forceExpandedForm, useSiteInfo)
			If (theArguments >= 2) Then
				theArguments = OverloadResolution.ShadowBasedOnDelegateRelaxation(candidates, num1)
				If (theArguments >= 2) Then
					OverloadResolution.ShadowBasedOnInferenceLevel(candidates, arguments, Not argumentNames.IsDefault, delegateReturnType, binder, theArguments, num1, useSiteInfo)
					If (theArguments < 2) Then
						flag2 = num1 > 0
					ElseIf (theArguments <> num1) Then
						If (num1 > 0) Then
							theArguments = OverloadResolution.EliminateNarrowingCandidates(candidates)
							If (theArguments < 2) Then
								If (flag1 OrElse theArguments <> 0) Then
									overloadResolutionResult = New OverloadResolution.OverloadResolutionResult(candidates.ToImmutable(), flag1, flag2, asyncLambdaSubToFunctionMismatch)
								Else
									candidateAnalysisResults = candidates
									num = theArguments
									flag = If(Not lateBindingIsAllowed, False, binder.OptionStrict <> OptionStrict.[On])
									overloadResolutionResult = OverloadResolution.ReportOverloadResolutionFailedOrLateBound(candidateAnalysisResults, num, flag, asyncLambdaSubToFunctionMismatch)
								End If
								Return overloadResolutionResult
							End If
						End If
						theArguments = OverloadResolution.EliminateLessApplicableToTheArguments(candidates, arguments, delegateReturnType, False, binder, useSiteInfo, False)
					Else
						flag2 = True
						theArguments = OverloadResolution.AnalyzeNarrowingCandidates(candidates, arguments, delegateReturnType, If(Not lateBindingIsAllowed, False, binder.OptionStrict <> OptionStrict.[On]), binder, flag1, useSiteInfo)
					End If
				Else
					flag2 = num1 > 0
				End If
			Else
				flag2 = num1 > 0
			End If
			If (flag1 OrElse theArguments <> 0) Then
				overloadResolutionResult = New OverloadResolution.OverloadResolutionResult(candidates.ToImmutable(), flag1, flag2, asyncLambdaSubToFunctionMismatch)
			Else
				candidateAnalysisResults = candidates
				num = theArguments
				flag = If(Not lateBindingIsAllowed, False, binder.OptionStrict <> OptionStrict.[On])
				overloadResolutionResult = OverloadResolution.ReportOverloadResolutionFailedOrLateBound(candidateAnalysisResults, num, flag, asyncLambdaSubToFunctionMismatch)
			End If
			Return overloadResolutionResult
		End Function

		Public Shared Function ResolveUnaryOperator(ByVal opCode As Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind, ByVal operand As BoundExpression, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <Out> ByRef intrinsicOperatorType As SpecialType, <Out> ByRef userDefinedOperator As OverloadResolution.OverloadResolutionResult, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind
			Dim unaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind
			opCode = opCode And Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.[Not]
			intrinsicOperatorType = SpecialType.None
			userDefinedOperator = New OverloadResolution.OverloadResolutionResult()
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = operand.Type
			Dim nullableUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type.GetNullableUnderlyingTypeOrSelf()
			Dim flag As Boolean = CObj(type) <> CObj(nullableUnderlyingTypeOrSelf)
			Dim enumUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = nullableUnderlyingTypeOrSelf.GetEnumUnderlyingTypeOrSelf()
			Dim flag1 As Boolean = CObj(enumUnderlyingTypeOrSelf) <> CObj(nullableUnderlyingTypeOrSelf)
			If ((flag1 OrElse flag) AndAlso (enumUnderlyingTypeOrSelf.IsStringType() OrElse enumUnderlyingTypeOrSelf.IsObjectType())) Then
				unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.[Error]
			Else
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = enumUnderlyingTypeOrSelf
				If (typeSymbol.SpecialType = SpecialType.System_Object OrElse typeSymbol.IsIntrinsicType()) Then
					Dim unaryOperatorKind1 As Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.[Error]
					If (Not flag1 OrElse opCode <> Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.[Not] OrElse Not typeSymbol.IsIntegralType()) Then
						intrinsicOperatorType = OverloadResolution.ResolveNotLiftedIntrinsicUnaryOperator(opCode, typeSymbol.SpecialType)
						If (CSByte(intrinsicOperatorType) <> 0) Then
							unaryOperatorKind1 = opCode
						End If
					Else
						unaryOperatorKind1 = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.[Not]
					End If
					If (unaryOperatorKind1 <> Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.[Error] AndAlso flag) Then
						unaryOperatorKind1 = unaryOperatorKind1 Or Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Lifted
					End If
					unaryOperatorKind = unaryOperatorKind1
				ElseIf (Not type.CanContainUserDefinedOperators(useSiteInfo)) Then
					unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.[Error]
				Else
					userDefinedOperator = OverloadResolution.ResolveUserDefinedUnaryOperator(operand, opCode, binder, useSiteInfo, False)
					If (Not userDefinedOperator.BestResult.HasValue AndAlso userDefinedOperator.Candidates.Length = 0) Then
						userDefinedOperator = OverloadResolution.ResolveUserDefinedUnaryOperator(operand, opCode, binder, useSiteInfo, True)
						If (userDefinedOperator.Candidates.Length <> 0) Then
							GoTo Label1
						End If
						unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.[Error]
						Return unaryOperatorKind
					End If
				Label1:
					unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.UserDefined
				End If
			End If
			Return unaryOperatorKind
		End Function

		Public Shared Function ResolveUserDefinedBinaryOperator(ByVal left As BoundExpression, ByVal right As BoundExpression, ByVal opKind As BinaryOperatorKind, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal includeEliminatedCandidates As Boolean = False) As OverloadResolution.OverloadResolutionResult
			Dim operatorInfo As OverloadResolution.OperatorInfo
			Dim instance As ArrayBuilder(Of MethodSymbol) = ArrayBuilder(Of MethodSymbol).GetInstance()
			Select Case opKind
				Case BinaryOperatorKind.Add
					Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo1 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type, typeSymbol, MethodKind.UserDefinedOperator, "op_Addition", operatorInfo1, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.Concatenate
					Dim type1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo2 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type1, typeSymbol1, MethodKind.UserDefinedOperator, "op_Concatenate", operatorInfo2, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.[Like]
					Dim type2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo3 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type2, typeSymbol2, MethodKind.UserDefinedOperator, "op_Like", operatorInfo3, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.Equals
					Dim type3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo4 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type3, typeSymbol3, MethodKind.UserDefinedOperator, "op_Equality", operatorInfo4, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.NotEquals
					Dim type4 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol4 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo5 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type4, typeSymbol4, MethodKind.UserDefinedOperator, "op_Inequality", operatorInfo5, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.LessThanOrEqual
					Dim type5 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol5 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo6 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type5, typeSymbol5, MethodKind.UserDefinedOperator, "op_LessThanOrEqual", operatorInfo6, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.GreaterThanOrEqual
					Dim type6 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol6 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo7 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type6, typeSymbol6, MethodKind.UserDefinedOperator, "op_GreaterThanOrEqual", operatorInfo7, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.LessThan
					Dim type7 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol7 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo8 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type7, typeSymbol7, MethodKind.UserDefinedOperator, "op_LessThan", operatorInfo8, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.GreaterThan
					Dim type8 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol8 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo9 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type8, typeSymbol8, MethodKind.UserDefinedOperator, "op_GreaterThan", operatorInfo9, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.Subtract
					Dim type9 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol9 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo10 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type9, typeSymbol9, MethodKind.UserDefinedOperator, "op_Subtraction", operatorInfo10, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.Multiply
					Dim type10 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol10 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo11 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type10, typeSymbol10, MethodKind.UserDefinedOperator, "op_Multiply", operatorInfo11, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.Power
					Dim type11 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol11 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo12 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type11, typeSymbol11, MethodKind.UserDefinedOperator, "op_Exponent", operatorInfo12, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.Divide
					Dim type12 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol12 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo13 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type12, typeSymbol12, MethodKind.UserDefinedOperator, "op_Division", operatorInfo13, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.Modulo
					Dim type13 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol13 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo14 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type13, typeSymbol13, MethodKind.UserDefinedOperator, "op_Modulus", operatorInfo14, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.IntegerDivide
					Dim type14 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol14 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo15 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type14, typeSymbol14, MethodKind.UserDefinedOperator, "op_IntegerDivision", operatorInfo15, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.LeftShift
					Dim operatorInfo16 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					OverloadResolution.CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_LeftShift", operatorInfo16, "op_UnsignedLeftShift", operatorInfo16, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.RightShift
					Dim operatorInfo17 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					OverloadResolution.CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_RightShift", operatorInfo17, "op_UnsignedRightShift", operatorInfo17, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.[Xor]
					Dim type15 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
					Dim typeSymbol15 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Type
					Dim operatorInfo18 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type15, typeSymbol15, MethodKind.UserDefinedOperator, "op_ExclusiveOr", operatorInfo18, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.[Or]
				Case BinaryOperatorKind.[OrElse]
					Dim operatorInfo19 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					OverloadResolution.CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_BitwiseOr", operatorInfo19, "op_LogicalOr", operatorInfo19, instance, useSiteInfo)
					Exit Select
				Case BinaryOperatorKind.[And]
				Case BinaryOperatorKind.[AndAlso]
					Dim operatorInfo20 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(opKind)
					OverloadResolution.CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_BitwiseAnd", operatorInfo20, "op_LogicalAnd", operatorInfo20, instance, useSiteInfo)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(opKind)
			End Select
			Dim overloadResolutionResult As OverloadResolution.OverloadResolutionResult = OverloadResolution.OperatorInvocationOverloadResolution(instance, left, right, binder, True, includeEliminatedCandidates, useSiteInfo)
			instance.Free()
			Return overloadResolutionResult
		End Function

		Public Shared Function ResolveUserDefinedConversion(ByVal source As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal destination As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			Dim keyValuePair As KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			Dim keyValuePair1 As KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = New KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)()
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).GetInstance()
			OverloadResolution.CollectUserDefinedConversionOperators(source, destination, instance, useSiteInfo)
			If (instance.Count <> 0) Then
				Dim keyValuePairs As ArrayBuilder(Of KeyValuePair(Of ConversionKind, ConversionKind)) = ArrayBuilder(Of KeyValuePair(Of ConversionKind, ConversionKind)).GetInstance()
				keyValuePairs.ZeroInit(instance.Count)
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = Microsoft.CodeAnalysis.BitVector.Create(instance.Count)
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
				If (OverloadResolution.DetermineMostSpecificWideningConversion(source, destination, instance, keyValuePairs, bitVector, methodSymbol, False, useSiteInfo)) Then
					If (methodSymbol IsNot Nothing) Then
						keyValuePair1 = New KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(ConversionKind.[Widening] Or ConversionKind.UserDefined, methodSymbol)
					End If
				ElseIf (instance.Count <> 0) Then
					If (OverloadResolution.DetermineMostSpecificNarrowingConversion(source, destination, instance, keyValuePairs, bitVector, methodSymbol, False, useSiteInfo)) Then
						If (methodSymbol IsNot Nothing) Then
							keyValuePair1 = New KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(ConversionKind.[Narrowing] Or ConversionKind.UserDefined, methodSymbol)
						End If
					ElseIf (instance.Count <> 0 AndAlso source.IsNullableType() AndAlso destination.IsNullableType()) Then
						bitVector.Clear()
						keyValuePairs.ZeroInit(instance.Count)
						Dim nullableUnderlyingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = source.GetNullableUnderlyingType()
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = destination.GetNullableUnderlyingType()
						If (Not nullableUnderlyingType.IsErrorType() AndAlso Not typeSymbol.IsErrorType()) Then
							If (OverloadResolution.DetermineMostSpecificWideningConversion(nullableUnderlyingType, typeSymbol, instance, keyValuePairs, bitVector, methodSymbol, True, useSiteInfo)) Then
								If (methodSymbol IsNot Nothing) Then
									keyValuePair1 = New KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(ConversionKind.[Widening] Or ConversionKind.Nullable Or ConversionKind.WideningNullable Or ConversionKind.UserDefined, methodSymbol)
								End If
							ElseIf (OverloadResolution.DetermineMostSpecificNarrowingConversion(nullableUnderlyingType, typeSymbol, instance, keyValuePairs, bitVector, methodSymbol, True, useSiteInfo) AndAlso methodSymbol IsNot Nothing) Then
								keyValuePair1 = New KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(ConversionKind.[Narrowing] Or ConversionKind.Nullable Or ConversionKind.NarrowingNullable Or ConversionKind.UserDefined, methodSymbol)
							End If
						End If
					End If
				End If
				keyValuePairs.Free()
				instance.Free()
				keyValuePair = keyValuePair1
			Else
				instance.Free()
				keyValuePair = keyValuePair1
			End If
			Return keyValuePair
		End Function

		Public Shared Function ResolveUserDefinedUnaryOperator(ByVal argument As BoundExpression, ByVal opKind As UnaryOperatorKind, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal includeEliminatedCandidates As Boolean = False) As OverloadResolution.OverloadResolutionResult
			Dim operatorInfo As OverloadResolution.OperatorInfo
			Dim instance As ArrayBuilder(Of MethodSymbol) = ArrayBuilder(Of MethodSymbol).GetInstance()
			Select Case opKind
				Case UnaryOperatorKind.Plus
					Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = argument.Type
					Dim operatorInfo1 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(UnaryOperatorKind.Minus)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(type, Nothing, MethodKind.UserDefinedOperator, "op_UnaryPlus", operatorInfo1, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case UnaryOperatorKind.Minus
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = argument.Type
					Dim operatorInfo2 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(UnaryOperatorKind.Minus)
					operatorInfo = New OverloadResolution.OperatorInfo()
					OverloadResolution.CollectUserDefinedOperators(typeSymbol, Nothing, MethodKind.UserDefinedOperator, "op_UnaryNegation", operatorInfo2, Nothing, operatorInfo, instance, useSiteInfo)
					Exit Select
				Case UnaryOperatorKind.[Not]
					Dim operatorInfo3 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(UnaryOperatorKind.[Not])
					OverloadResolution.CollectUserDefinedOperators(argument.Type, Nothing, MethodKind.UserDefinedOperator, "op_OnesComplement", operatorInfo3, "op_LogicalNot", operatorInfo3, instance, useSiteInfo)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(opKind)
			End Select
			Dim overloadResolutionResult As OverloadResolution.OverloadResolutionResult = OverloadResolution.OperatorInvocationOverloadResolution(instance, argument, Nothing, binder, False, includeEliminatedCandidates, useSiteInfo)
			instance.Free()
			Return overloadResolutionResult
		End Function

		Private Shared Function ShadowBasedOnDelegateRelaxation(ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult), ByRef applicableNarrowingCandidates As Integer) As Integer
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelInvalid
			Dim count As Integer = candidates.Count - 1
			Dim num As Integer = 0
			Do
				Dim item As OverloadResolution.CandidateAnalysisResult = candidates(num)
				If (item.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
					Dim maxDelegateRelaxationLevel As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = item.MaxDelegateRelaxationLevel
					If (maxDelegateRelaxationLevel < conversionKind) Then
						conversionKind = maxDelegateRelaxationLevel
					End If
				End If
				num = num + 1
			Loop While num <= count
			Dim num1 As Integer = 0
			applicableNarrowingCandidates = 0
			Dim count1 As Integer = candidates.Count - 1
			Dim num2 As Integer = 0
			Do
				Dim candidateAnalysisResult As OverloadResolution.CandidateAnalysisResult = candidates(num2)
				If (candidateAnalysisResult.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
					If (candidateAnalysisResult.MaxDelegateRelaxationLevel <= conversionKind) Then
						num1 = num1 + 1
						If (candidateAnalysisResult.RequiresNarrowingConversion) Then
							applicableNarrowingCandidates = applicableNarrowingCandidates + 1
						End If
					Else
						candidateAnalysisResult.State = OverloadResolution.CandidateAnalysisResultState.Shadowed
						candidates(num2) = candidateAnalysisResult
					End If
				End If
				num2 = num2 + 1
			Loop While num2 <= count1
			Return num1
		End Function

		Private Shared Function ShadowBasedOnDepthOfGenericity(ByVal left As OverloadResolution.CandidateAnalysisResult, ByVal right As OverloadResolution.CandidateAnalysisResult, ByRef leftWins As Boolean, ByRef rightWins As Boolean, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Boolean
			Dim flag As Boolean
			Dim parameterTypeFromVirtualSignature As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim num As Integer = 0
			Dim num1 As Integer = 0
			Dim length As Integer = arguments.Length - 1
			Dim num2 As Integer = 0
			While True
				If (num2 <= length) Then
					Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
					If (Not left.ArgsToParamsOpt.IsDefault) Then
						parameterTypeFromVirtualSignature = OverloadResolution.GetParameterTypeFromVirtualSignature(left, left.ArgsToParamsOpt(num2), typeSymbol1)
					Else
						parameterTypeFromVirtualSignature = OverloadResolution.GetParameterTypeFromVirtualSignature(left, num, typeSymbol1)
						OverloadResolution.AdvanceParameterInVirtualSignature(left, num)
					End If
					Dim typeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
					If (Not right.ArgsToParamsOpt.IsDefault) Then
						typeSymbol = OverloadResolution.GetParameterTypeFromVirtualSignature(right, right.ArgsToParamsOpt(num2), typeSymbol2)
					Else
						typeSymbol = OverloadResolution.GetParameterTypeFromVirtualSignature(right, num1, typeSymbol2)
						OverloadResolution.AdvanceParameterInVirtualSignature(right, num1)
					End If
					If (arguments(num2).Kind <> BoundKind.OmittedArgument) Then
						If (Not OverloadResolution.SignatureMismatchForThePurposeOfShadowingBasedOnGenericity(parameterTypeFromVirtualSignature, typeSymbol, arguments(num2), binder)) Then
							Dim flag1 As Boolean = False
							Dim flag2 As Boolean = False
							If (OverloadResolution.CompareParameterTypeGenericDepth(typeSymbol1, typeSymbol2, flag1, flag2)) Then
								If (flag1) Then
									If (Not rightWins) Then
										leftWins = True
									Else
										rightWins = False
										flag = False
										Exit While
									End If
								ElseIf (Not leftWins) Then
									rightWins = True
								Else
									leftWins = False
									flag = False
									Exit While
								End If
							End If
						Else
							flag = False
							Exit While
						End If
					End If
					num2 = num2 + 1
				Else
					flag = If(leftWins, True, rightWins)
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Shared Function ShadowBasedOnExtensionMethodTargetTypeGenericity(ByVal left As OverloadResolution.CandidateAnalysisResult, ByVal right As OverloadResolution.CandidateAnalysisResult, ByRef leftWins As Boolean, ByRef rightWins As Boolean) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.OverloadResolution::ShadowBasedOnExtensionMethodTargetTypeGenericity(Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/CandidateAnalysisResult,Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/CandidateAnalysisResult,System.Boolean&,System.Boolean&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean ShadowBasedOnExtensionMethodTargetTypeGenericity(Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/CandidateAnalysisResult,Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/CandidateAnalysisResult,System.Boolean&,System.Boolean&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function ShadowBasedOnExtensionVsInstanceAndPrecedence(ByVal left As OverloadResolution.CandidateAnalysisResult, ByVal right As OverloadResolution.CandidateAnalysisResult, ByRef leftWins As Boolean, ByRef rightWins As Boolean) As Boolean
			Dim flag As Boolean
			If (Not left.Candidate.IsExtensionMethod) Then
				If (Not right.Candidate.IsExtensionMethod) Then
					flag = False
					Return flag
				End If
				leftWins = True
				flag = True
			ElseIf (Not right.Candidate.IsExtensionMethod) Then
				rightWins = True
				flag = True
			ElseIf (left.Candidate.PrecedenceLevel >= right.Candidate.PrecedenceLevel) Then
				If (left.Candidate.PrecedenceLevel <= right.Candidate.PrecedenceLevel) Then
					flag = False
					Return flag
				End If
				rightWins = True
				flag = True
			Else
				leftWins = True
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function ShadowBasedOnGenericity(ByVal left As OverloadResolution.CandidateAnalysisResult, ByVal right As OverloadResolution.CandidateAnalysisResult, ByRef leftWins As Boolean, ByRef rightWins As Boolean, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.OverloadResolution::ShadowBasedOnGenericity(Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/CandidateAnalysisResult,Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/CandidateAnalysisResult,System.Boolean&,System.Boolean&,System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.VisualBasic.BoundExpression>,Microsoft.CodeAnalysis.VisualBasic.Binder)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean ShadowBasedOnGenericity(Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/CandidateAnalysisResult,Microsoft.CodeAnalysis.VisualBasic.OverloadResolution/CandidateAnalysisResult,System.Boolean&,System.Boolean&,System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.VisualBasic.BoundExpression>,Microsoft.CodeAnalysis.VisualBasic.Binder)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Sub ShadowBasedOnInferenceLevel(ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal haveNamedArguments As Boolean, ByVal delegateReturnType As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByRef applicableCandidates As Integer, ByRef applicableNarrowingCandidates As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim flag As Boolean = False
			Dim inferenceLevel As TypeArgumentInference.InferenceLevel = TypeArgumentInference.InferenceLevel.Orcas Or TypeArgumentInference.InferenceLevel.Invalid
			Dim count As Integer = candidates.Count - 1
			Dim num As Integer = 0
			Do
				Dim item As OverloadResolution.CandidateAnalysisResult = candidates(num)
				If (item.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
					Dim inferenceLevel1 As TypeArgumentInference.InferenceLevel = item.InferenceLevel
					If (inferenceLevel = (TypeArgumentInference.InferenceLevel.Orcas Or TypeArgumentInference.InferenceLevel.Invalid)) Then
						inferenceLevel = inferenceLevel1
					ElseIf (inferenceLevel1 <> inferenceLevel) Then
						flag = True
						Exit Do
					End If
				End If
				num = num + 1
			Loop While num <= count
			If (flag) Then
				If (haveNamedArguments) Then
					Dim instance As ArrayBuilder(Of Integer) = ArrayBuilder(Of Integer).GetInstance(applicableCandidates)
					Dim count1 As Integer = candidates.Count - 1
					Dim num1 As Integer = 0
					Do
						If (candidates(num1).State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
							instance.Add(num1)
						End If
						num1 = num1 + 1
					Loop While num1 <= count1
					instance.Sort(New OverloadResolution.InferenceLevelComparer(candidates))
					Dim count2 As Integer = instance.Count - 2
					For i As Integer = 0 To count2
						Dim candidateAnalysisResult As OverloadResolution.CandidateAnalysisResult = candidates(instance(i))
						If (candidateAnalysisResult.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
							Dim num2 As Integer = instance.Count - 1
							Dim num3 As Integer = i + 1
							Do
								Dim item1 As OverloadResolution.CandidateAnalysisResult = candidates(instance(num3))
								If (item1.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
									Dim flag1 As Boolean = True
									Dim length As Integer = arguments.Length - 1
									Dim num4 As Integer = 0
									While num4 <= length
										If (OverloadResolution.GetParameterTypeFromVirtualSignature(candidateAnalysisResult, candidateAnalysisResult.ArgsToParamsOpt(num4)).IsSameTypeIgnoringAll(OverloadResolution.GetParameterTypeFromVirtualSignature(item1, item1.ArgsToParamsOpt(num4)))) Then
											num4 = num4 + 1
										Else
											flag1 = False
											Exit While
										End If
									End While
									If (flag1) Then
										Dim flag2 As Boolean = True
										If (candidateAnalysisResult.Candidate.ParameterCount = item1.Candidate.ParameterCount) Then
											Dim parameterCount As Integer = candidateAnalysisResult.Candidate.ParameterCount - 1
											Dim num5 As Integer = 0
											While num5 <= parameterCount
												If (candidateAnalysisResult.Candidate.Parameters(num5).Type.IsSameTypeIgnoringAll(item1.Candidate.Parameters(num5).Type)) Then
													num5 = num5 + 1
												Else
													flag2 = False
													Exit While
												End If
											End While
										Else
											flag2 = False
										End If
										Dim flag3 As Boolean = False
										Dim flag4 As Boolean = False
										If (Not flag2 AndAlso OverloadResolution.ShadowBasedOnParamArrayUsage(candidateAnalysisResult, item1, flag3, flag4) OrElse OverloadResolution.ShadowBasedOnReceiverType(candidateAnalysisResult, item1, flag3, flag4, useSiteInfo) OrElse OverloadResolution.ShadowBasedOnExtensionMethodTargetTypeGenericity(candidateAnalysisResult, item1, flag3, flag4)) Then
											If (flag3) Then
												item1.State = OverloadResolution.CandidateAnalysisResultState.Shadowed
												candidates(instance(num3)) = item1
											ElseIf (flag4) Then
												candidateAnalysisResult.State = OverloadResolution.CandidateAnalysisResultState.Shadowed
												candidates(instance(i)) = candidateAnalysisResult
												Exit Do
											End If
										End If
									End If
								End If
								num3 = num3 + 1
							Loop While num3 <= num2
							If (candidateAnalysisResult.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
								Exit For
							End If
						End If
					Next

				End If
				Dim inferenceLevel2 As TypeArgumentInference.InferenceLevel = TypeArgumentInference.InferenceLevel.Invalid
				Dim count3 As Integer = candidates.Count - 1
				Dim num6 As Integer = 0
				Do
					Dim candidateAnalysisResult1 As OverloadResolution.CandidateAnalysisResult = candidates(num6)
					If (candidateAnalysisResult1.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
						Dim inferenceLevel3 As TypeArgumentInference.InferenceLevel = candidateAnalysisResult1.InferenceLevel
						If (inferenceLevel3 < inferenceLevel2) Then
							inferenceLevel2 = inferenceLevel3
						End If
					End If
					num6 = num6 + 1
				Loop While num6 <= count3
				applicableCandidates = 0
				applicableNarrowingCandidates = 0
				Dim count4 As Integer = candidates.Count - 1
				For j As Integer = 0 To count4
					Dim item2 As OverloadResolution.CandidateAnalysisResult = candidates(j)
					If (item2.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
						If (item2.InferenceLevel <= inferenceLevel2) Then
							applicableCandidates = applicableCandidates + 1
							If (item2.RequiresNarrowingConversion) Then
								applicableNarrowingCandidates = applicableNarrowingCandidates + 1
							End If
						Else
							item2.State = OverloadResolution.CandidateAnalysisResultState.Shadowed
							candidates(j) = item2
						End If
					End If
				Next

			End If
		End Sub

		Private Shared Function ShadowBasedOnOptionalParametersDefaultsUsed(ByVal left As OverloadResolution.CandidateAnalysisResult, ByVal right As OverloadResolution.CandidateAnalysisResult, ByRef leftWins As Boolean, ByRef rightWins As Boolean) As Boolean
			Dim flag As Boolean
			Dim usedOptionalParameterDefaultValue As Boolean = left.UsedOptionalParameterDefaultValue
			If (usedOptionalParameterDefaultValue <> right.UsedOptionalParameterDefaultValue) Then
				If (usedOptionalParameterDefaultValue) Then
					rightWins = True
				Else
					leftWins = True
				End If
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function ShadowBasedOnOverriding(ByVal existingCandidate As OverloadResolution.CandidateAnalysisResult, ByVal newCandidate As OverloadResolution.CandidateAnalysisResult, ByRef existingWins As Boolean, ByRef newWins As Boolean) As Boolean
			Dim flag As Boolean
			Dim underlyingSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = existingCandidate.Candidate.UnderlyingSymbol
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = newCandidate.Candidate.UnderlyingSymbol
			Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = underlyingSymbol.ContainingType
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = symbol.ContainingType
			Dim state As Boolean = existingCandidate.State = OverloadResolution.CandidateAnalysisResultState.Applicable
			If (Not state OrElse containingType.IsRestrictedType() OrElse namedTypeSymbol.IsRestrictedType()) Then
				If (CObj(containingType.OriginalDefinition) <> CObj(namedTypeSymbol.OriginalDefinition)) Then
					If (Not newCandidate.Candidate.IsOverriddenBy(underlyingSymbol)) Then
						If (Not state OrElse Not existingCandidate.Candidate.IsOverriddenBy(symbol)) Then
							GoTo Label1
						End If
						newWins = True
						flag = True
						Return flag
					Else
						existingWins = True
						flag = True
						Return flag
					End If
				End If
			Label1:
				flag = False
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function ShadowBasedOnParamArrayUsage(ByVal left As OverloadResolution.CandidateAnalysisResult, ByVal right As OverloadResolution.CandidateAnalysisResult, ByRef leftWins As Boolean, ByRef rightWins As Boolean) As Boolean
			Dim flag As Boolean
			If (Not left.IsExpandedParamArrayForm) Then
				If (Not right.IsExpandedParamArrayForm) Then
					flag = False
					Return flag
				End If
				leftWins = True
				flag = True
			ElseIf (Not right.IsExpandedParamArrayForm) Then
				rightWins = True
				flag = True
			ElseIf (left.ExpandedParamArrayArgumentsUsed <= right.ExpandedParamArrayArgumentsUsed) Then
				If (left.ExpandedParamArrayArgumentsUsed >= right.ExpandedParamArrayArgumentsUsed) Then
					flag = False
					Return flag
				End If
				leftWins = True
				flag = True
			Else
				rightWins = True
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function ShadowBasedOnReceiverType(ByVal left As OverloadResolution.CandidateAnalysisResult, ByVal right As OverloadResolution.CandidateAnalysisResult, ByRef leftWins As Boolean, ByRef rightWins As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			Dim receiverType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Candidate.ReceiverType
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = right.Candidate.ReceiverType
			If (Not receiverType.IsSameTypeIgnoringAll(typeSymbol)) Then
				If (Not OverloadResolution.DoesReceiverMatchInstance(receiverType, typeSymbol, useSiteInfo)) Then
					If (Not OverloadResolution.DoesReceiverMatchInstance(typeSymbol, receiverType, useSiteInfo)) Then
						flag = False
						Return flag
					End If
					rightWins = True
					flag = True
					Return flag
				Else
					leftWins = True
					flag = True
					Return flag
				End If
			End If
			flag = False
			Return flag
		End Function

		Private Shared Function ShadowBasedOnSubOrFunction(ByVal left As OverloadResolution.CandidateAnalysisResult, ByVal right As OverloadResolution.CandidateAnalysisResult, ByVal delegateReturnType As TypeSymbol, ByRef leftWins As Boolean, ByRef rightWins As Boolean) As Boolean
			Dim flag As Boolean
			If (delegateReturnType IsNot Nothing) Then
				Dim flag1 As Boolean = left.Candidate.ReturnType.IsVoidType()
				If (flag1 = right.Candidate.ReturnType.IsVoidType()) Then
					flag = False
				ElseIf (delegateReturnType.IsVoidType() <> flag1) Then
					rightWins = True
					flag = True
				Else
					leftWins = True
					flag = True
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function ShadowBasedOnTieBreakingRules(ByVal left As OverloadResolution.CandidateAnalysisResult, ByVal right As OverloadResolution.CandidateAnalysisResult, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal delegateReturnType As TypeSymbol, ByRef leftWins As Boolean, ByRef rightWins As Boolean, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			leftWins = False
			rightWins = False
			If (OverloadResolution.ShadowBasedOnParamArrayUsage(left, right, leftWins, rightWins)) Then
				flag = True
			ElseIf (OverloadResolution.ShadowBasedOnReceiverType(left, right, leftWins, rightWins, useSiteInfo)) Then
				flag = True
			ElseIf (OverloadResolution.ShadowBasedOnExtensionMethodTargetTypeGenericity(left, right, leftWins, rightWins)) Then
				flag = True
			ElseIf (OverloadResolution.ShadowBasedOnGenericity(left, right, leftWins, rightWins, arguments, binder)) Then
				flag = True
			ElseIf (OverloadResolution.ShadowBasedOnExtensionVsInstanceAndPrecedence(left, right, leftWins, rightWins)) Then
				flag = True
			ElseIf (OverloadResolution.ShadowBasedOnOptionalParametersDefaultsUsed(left, right, leftWins, rightWins)) Then
				flag = True
			ElseIf (Not OverloadResolution.ShadowBasedOnSubOrFunction(left, right, delegateReturnType, leftWins, rightWins)) Then
				flag = If(Not OverloadResolution.ShadowBasedOnDepthOfGenericity(left, right, leftWins, rightWins, arguments, binder), False, True)
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function ShouldConsiderExtensionMethods(ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult)) As Boolean
			Dim flag As Boolean
			Dim count As Integer = candidates.Count - 1
			Dim num As Integer = 0
			While True
				If (num > count) Then
					flag = True
					Exit While
				ElseIf (Not candidates(num).IgnoreExtensionMethods) Then
					num = num + 1
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Shared Function SignatureMismatchForThePurposeOfShadowingBasedOnGenericity(ByVal leftParamType As TypeSymbol, ByVal rightParamType As TypeSymbol, ByVal argument As BoundExpression, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean = False
			Dim flag2 As Boolean = False
			If (Not leftParamType.IsSameTypeIgnoringAll(rightParamType)) Then
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = leftParamType.DelegateOrExpressionDelegate(binder, flag1)
				Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = rightParamType.DelegateOrExpressionDelegate(binder, flag2)
				If (namedTypeSymbol IsNot Nothing AndAlso namedTypeSymbol1 IsNot Nothing AndAlso (flag1 = flag2 OrElse argument.IsAnyLambda())) Then
					Dim delegateInvokeMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = namedTypeSymbol.DelegateInvokeMethod
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = namedTypeSymbol1.DelegateInvokeMethod
					If (delegateInvokeMethod Is Nothing OrElse methodSymbol Is Nothing OrElse Not MethodSignatureComparer.ParametersAndReturnTypeSignatureComparer.Equals(delegateInvokeMethod, methodSymbol)) Then
						GoTo Label1
					End If
					flag = False
					Return flag
				End If
			Label1:
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Shared Function TryFoldConstantBinaryOperator(ByVal operatorKind As BinaryOperatorKind, ByVal left As BoundExpression, ByVal right As BoundExpression, ByVal resultType As TypeSymbol, ByRef integerOverflow As Boolean, ByRef divideByZero As Boolean, ByRef lengthOutOfLimit As Boolean) As ConstantValue
			' 
			' Current member / type: Microsoft.CodeAnalysis.ConstantValue Microsoft.CodeAnalysis.VisualBasic.OverloadResolution::TryFoldConstantBinaryOperator(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind,Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Boolean&,System.Boolean&,System.Boolean&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.ConstantValue TryFoldConstantBinaryOperator(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind,Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Boolean&,System.Boolean&,System.Boolean&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Shared Function TryFoldConstantUnaryOperator(ByVal op As UnaryOperatorKind, ByVal operand As BoundExpression, ByVal resultType As TypeSymbol, ByRef integerOverflow As Boolean) As ConstantValue
			' 
			' Current member / type: Microsoft.CodeAnalysis.ConstantValue Microsoft.CodeAnalysis.VisualBasic.OverloadResolution::TryFoldConstantUnaryOperator(Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind,Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Boolean&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.ConstantValue TryFoldConstantUnaryOperator(Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind,Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Boolean&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 84
			'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 73
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 27
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 198
			'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 79
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
			'    à ..Visit[,]( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 286
			'    à ..Visit( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 317
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 337
			'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 49
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
			'    à ..(IfStatement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 361
			'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 55
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
			'    à ..Visit[,]( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 286
			'    à ..Visit( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 317
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 337
			'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 49
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
			'    à ..(IfStatement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 361
			'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 55
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
			'    à ..Visit[,]( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 286
			'    à ..Visit( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 317
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 337
			'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 49
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
			'    à ..(IfStatement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 361
			'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 55
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
			'    à ..Visit[,]( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 286
			'    à ..Visit( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 317
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 337
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 44
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Friend Shared Function TryGetOperatorName(ByVal op As BinaryOperatorKind) As String
			' 
			' Current member / type: System.String Microsoft.CodeAnalysis.VisualBasic.OverloadResolution::TryGetOperatorName(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.String TryGetOperatorName(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 84
			'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 73
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 27
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 198
			'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 79
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
			'    à ..Visit[,]( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 286
			'    à ..Visit( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 317
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 337
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 44
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Friend Shared Function TryGetOperatorName(ByVal op As Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind) As String
			Dim str As String
			Dim unaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind = op And Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.OpMask
			If (unaryOperatorKind <= Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Implicit) Then
				Select Case unaryOperatorKind
					Case Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Plus
						str = "op_UnaryPlus"
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Minus
						str = "op_UnaryNegation"
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.[Not]
						str = "op_OnesComplement"
						Exit Select
					Case Else
						If (unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Implicit) Then
							str = "op_Implicit"
							Exit Select
						Else
							str = Nothing
							Return str
						End If
				End Select
			ElseIf (unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Explicit) Then
				str = "op_Explicit"
			ElseIf (unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.IsTrue) Then
				str = "op_True"
			Else
				If (unaryOperatorKind <> Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.IsFalse) Then
					str = Nothing
					Return str
				End If
				str = "op_False"
			End If
			Return str
		End Function

		Public Shared Function UseUserDefinedBinaryOperators(ByVal opCode As BinaryOperatorKind, ByVal leftType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal rightType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As Boolean
			Dim flag As Boolean
			Dim enumUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = leftType.GetNullableUnderlyingTypeOrSelf().GetEnumUnderlyingTypeOrSelf()
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = rightType.GetNullableUnderlyingTypeOrSelf().GetEnumUnderlyingTypeOrSelf()
			flag = If((enumUnderlyingTypeOrSelf.SpecialType = SpecialType.System_Object OrElse enumUnderlyingTypeOrSelf.IsIntrinsicType() OrElse enumUnderlyingTypeOrSelf.IsCharSZArray()) AndAlso (typeSymbol.SpecialType = SpecialType.System_Object OrElse typeSymbol.IsIntrinsicType() OrElse typeSymbol.IsCharSZArray()) AndAlso (Not enumUnderlyingTypeOrSelf.IsDateTimeType() OrElse Not typeSymbol.IsDateTimeType() OrElse opCode <> BinaryOperatorKind.Subtract), False, True)
			Return flag
		End Function

		Friend Shared Function ValidateOverloadedOperator(ByVal method As MethodSymbol, ByVal opInfo As OverloadResolution.OperatorInfo) As Boolean
			Return OverloadResolution.ValidateOverloadedOperator(method, opInfo, Nothing, Nothing)
		End Function

		Friend Shared Function ValidateOverloadedOperator(ByVal method As MethodSymbol, ByVal opInfo As OverloadResolution.OperatorInfo, ByVal diagnosticsOpt As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal assemblyBeingBuiltOpt As AssemblySymbol) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Dim locations As ImmutableArray(Of Location)
			Dim returnType As TypeSymbol
			If (method.ParameterCount = opInfo.ParamCount) Then
				flag1 = True
				Dim containingType As NamedTypeSymbol = method.ContainingType
				Dim flag2 As Boolean = False
				Dim flag3 As Boolean = False
				Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = If(diagnosticsOpt IsNot Nothing, New CompoundUseSiteInfo(Of AssemblySymbol)(diagnosticsOpt, assemblyBeingBuiltOpt), CompoundUseSiteInfo(Of AssemblySymbol).Discarded)
				If (Not opInfo.IsUnary) Then
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BinaryParamMustBeContainingType1
					If (CInt(opInfo.BinaryOperatorKind) - CInt(BinaryOperatorKind.LeftShift) <= CInt(BinaryOperatorKind.Add) AndAlso method.Parameters(1).Type.GetNullableUnderlyingTypeOrSelf().SpecialType <> SpecialType.System_Int32) Then
						If (diagnosticsOpt IsNot Nothing) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					End If
				Else
					Dim unaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind = opInfo.UnaryOperatorKind
					If (unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Implicit OrElse unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Explicit) Then
						flag3 = True
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConvParamMustBeContainingType1
						If (OverloadResolution.OverloadedOperatorTargetsContainingType(containingType, method.ReturnType)) Then
							flag2 = True
						End If
					Else
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnaryParamMustBeContainingType1
						If (Not method.ReturnType.IsBooleanType()) Then
							Dim unaryOperatorKind1 As Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind = opInfo.UnaryOperatorKind
							If (unaryOperatorKind1 = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.IsTrue) Then
								If (diagnosticsOpt IsNot Nothing) Then
									GoTo Label2
								End If
								flag = False
								Return flag
							ElseIf (unaryOperatorKind1 = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.IsFalse) Then
								If (diagnosticsOpt IsNot Nothing) Then
									GoTo Label3
								End If
								flag = False
								Return flag
							End If
						End If
					End If
				End If
			Label12:
				Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = method.Parameters.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ParameterSymbol = enumerator.Current
					If (OverloadResolution.OverloadedOperatorTargetsContainingType(containingType, current.Type)) Then
						flag2 = True
					End If
					If (Not current.IsByRef) Then
						Continue While
					End If
					If (diagnosticsOpt Is Nothing) Then
						flag = False
						Return flag
					Else
						flag1 = False
					End If
				End While
				If (Not flag2) Then
					If (diagnosticsOpt IsNot Nothing) Then
						GoTo Label11
					End If
					flag = False
					Return flag
				ElseIf (flag3) Then
					Dim type As TypeSymbol = method.Parameters(0).Type
					returnType = method.ReturnType
					If (type.IsObjectType()) Then
						If (diagnosticsOpt IsNot Nothing) Then
							GoTo Label10
						End If
						flag = False
						Return flag
					ElseIf (returnType.IsObjectType()) Then
						If (diagnosticsOpt IsNot Nothing) Then
							GoTo Label9
						End If
						flag = False
						Return flag
					ElseIf (type.IsInterfaceType()) Then
						If (diagnosticsOpt IsNot Nothing) Then
							GoTo Label8
						End If
						flag = False
						Return flag
					ElseIf (returnType.IsInterfaceType()) Then
						If (diagnosticsOpt IsNot Nothing) Then
							GoTo Label7
						End If
						flag = False
						Return flag
					ElseIf (If(containingType.SpecialType = SpecialType.System_Nullable_T, CObj(type) = CObj(returnType), CObj(type.GetNullableUnderlyingTypeOrSelf()) = CObj(returnType.GetNullableUnderlyingTypeOrSelf()))) Then
						If (diagnosticsOpt IsNot Nothing) Then
							GoTo Label6
						End If
						flag = False
						Return flag
					ElseIf ((type.Kind = SymbolKind.NamedType OrElse type.Kind = SymbolKind.TypeParameter) AndAlso (returnType.Kind = SymbolKind.NamedType OrElse returnType.Kind = SymbolKind.TypeParameter)) Then
						If (Microsoft.CodeAnalysis.VisualBasic.Conversions.HasWideningDirectCastConversionButNotEnumTypeConversion(returnType, type, compoundUseSiteInfo)) Then
							If (diagnosticsOpt IsNot Nothing) Then
								GoTo Label5
							End If
							flag = False
							Return flag
						ElseIf (Microsoft.CodeAnalysis.VisualBasic.Conversions.HasWideningDirectCastConversionButNotEnumTypeConversion(type, returnType, compoundUseSiteInfo)) Then
							If (diagnosticsOpt IsNot Nothing) Then
								GoTo Label4
							End If
							flag = False
							Return flag
						End If
					End If
				End If
			Label13:
				If (flag1 AndAlso diagnosticsOpt IsNot Nothing) Then
					locations = method.Locations
					diagnosticsOpt.Add(locations(0), compoundUseSiteInfo)
				End If
				flag = flag1
			Else
				flag = False
			End If
			Return flag
		Label1:
			Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = diagnosticsOpt
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_OperatorRequiresIntegerParameter1, New [Object]() { SyntaxFacts.GetText(If(opInfo.BinaryOperatorKind = BinaryOperatorKind.LeftShift, SyntaxKind.LessThanLessThanToken, SyntaxKind.GreaterThanGreaterThanToken)) })
			locations = method.Locations
			bindingDiagnosticBag.Add(diagnosticInfo, locations(0))
			flag1 = False
			GoTo Label12
		Label2:
			Dim text() As [Object] = { SyntaxFacts.GetText(SyntaxKind.IsTrueKeyword) }
			Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_OperatorRequiresBoolReturnType1, text)
			locations = method.Locations
			diagnosticsOpt.Add(diagnosticInfo1, locations(0))
			flag1 = False
			GoTo Label12
		Label3:
			Dim objArray() As [Object] = { SyntaxFacts.GetText(SyntaxKind.IsFalseKeyword) }
			Dim diagnosticInfo2 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_OperatorRequiresBoolReturnType1, objArray)
			locations = method.Locations
			diagnosticsOpt.Add(diagnosticInfo2, locations(0))
			flag1 = False
			GoTo Label12
		Label4:
			Dim bindingDiagnosticBag1 As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = diagnosticsOpt
			Dim diagnosticInfo3 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(If(returnType = method.ContainingSymbol, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionFromDerivedType, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToBaseType))
			locations = method.Locations
			bindingDiagnosticBag1.Add(diagnosticInfo3, locations(0))
			flag1 = False
			GoTo Label13
		Label5:
			Dim bindingDiagnosticBag2 As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = diagnosticsOpt
			Dim diagnosticInfo4 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(If(returnType = method.ContainingSymbol, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionFromBaseType, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToDerivedType))
			locations = method.Locations
			bindingDiagnosticBag2.Add(diagnosticInfo4, locations(0))
			flag1 = False
			GoTo Label13
		Label6:
			Dim diagnosticInfo5 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToSameType)
			locations = method.Locations
			diagnosticsOpt.Add(diagnosticInfo5, locations(0))
			flag1 = False
			GoTo Label13
		Label7:
			Dim diagnosticInfo6 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToInterfaceType)
			locations = method.Locations
			diagnosticsOpt.Add(diagnosticInfo6, locations(0))
			flag1 = False
			GoTo Label13
		Label8:
			Dim diagnosticInfo7 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionFromInterfaceType)
			locations = method.Locations
			diagnosticsOpt.Add(diagnosticInfo7, locations(0))
			flag1 = False
			GoTo Label13
		Label9:
			Dim diagnosticInfo8 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToObject)
			locations = method.Locations
			diagnosticsOpt.Add(diagnosticInfo8, locations(0))
			flag1 = False
			GoTo Label13
		Label10:
			Dim diagnosticInfo9 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionFromObject)
			locations = method.Locations
			diagnosticsOpt.Add(diagnosticInfo9, locations(0))
			flag1 = False
			GoTo Label13
		Label11:
			Dim containingSymbol() As [Object] = { method.ContainingSymbol }
			Dim diagnosticInfo10 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(eRRID, containingSymbol)
			locations = method.Locations
			diagnosticsOpt.Add(diagnosticInfo10, locations(0))
			flag1 = False
			GoTo Label13
		End Function

		Private Enum ApplicabilityComparisonResult
			Undefined
			EquallyApplicable
			LeftIsMoreApplicable
			RightIsMoreApplicable
		End Enum

		Private Class BinaryOperatorTables
			Public ReadOnly Shared Table As SByte(,,)

			Shared Sub New()
				OverloadResolution.BinaryOperatorTables.Table = New SByte(,,) { { { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 20, 19, 20, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 20 }, { 1, 19, 11, 0, 9, 11, 13, 15, 11, 13, 15, 17, 18, 19, 17, 0 }, { 1, 20, 0, 20, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 1, 19, 9, 0, 9, 11, 13, 15, 11, 13, 15, 17, 18, 19, 17, 0 }, { 1, 19, 11, 0, 11, 11, 13, 15, 11, 13, 15, 17, 18, 19, 17, 0 }, { 1, 19, 13, 0, 13, 13, 13, 15, 13, 13, 15, 17, 18, 19, 17, 0 }, { 1, 19, 15, 0, 15, 15, 15, 15, 15, 15, 15, 17, 18, 19, 17, 0 }, { 1, 19, 11, 0, 11, 11, 13, 15, 10, 12, 14, 16, 18, 19, 17, 0 }, { 1, 19, 13, 0, 13, 13, 13, 15, 12, 12, 14, 16, 18, 19, 17, 0 }, { 1, 19, 15, 0, 15, 15, 15, 15, 14, 14, 14, 16, 18, 19, 17, 0 }, { 1, 19, 17, 0, 17, 17, 17, 17, 16, 16, 16, 16, 18, 19, 17, 0 }, { 1, 19, 18, 0, 18, 18, 18, 18, 18, 18, 18, 18, 18, 19, 18, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 17, 0, 17, 17, 17, 17, 17, 17, 17, 17, 18, 19, 17, 0 }, { 1, 20, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 20 } }, { { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 11, 0, 9, 11, 13, 15, 11, 13, 15, 17, 18, 19, 17, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 1, 19, 9, 0, 9, 11, 13, 15, 11, 13, 15, 17, 18, 19, 17, 0 }, { 1, 19, 11, 0, 11, 11, 13, 15, 11, 13, 15, 17, 18, 19, 17, 0 }, { 1, 19, 13, 0, 13, 13, 13, 15, 13, 13, 15, 17, 18, 19, 17, 0 }, { 1, 19, 15, 0, 15, 15, 15, 15, 15, 15, 15, 17, 18, 19, 17, 0 }, { 1, 19, 11, 0, 11, 11, 13, 15, 10, 12, 14, 16, 18, 19, 17, 0 }, { 1, 19, 13, 0, 13, 13, 13, 15, 12, 12, 14, 16, 18, 19, 17, 0 }, { 1, 19, 15, 0, 15, 15, 15, 15, 14, 14, 14, 16, 18, 19, 17, 0 }, { 1, 19, 17, 0, 17, 17, 17, 17, 16, 16, 16, 16, 18, 19, 17, 0 }, { 1, 19, 18, 0, 18, 18, 18, 18, 18, 18, 18, 18, 18, 19, 18, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 17, 0, 17, 17, 17, 17, 17, 17, 17, 17, 18, 19, 17, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }, { { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 18, 19, 17, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 18, 19, 17, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 18, 19, 17, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 18, 19, 17, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 18, 19, 17, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 18, 19, 17, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 18, 19, 17, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 18, 19, 17, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 18, 19, 17, 0 }, { 1, 19, 18, 0, 18, 18, 18, 18, 18, 18, 18, 18, 18, 19, 18, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 17, 0, 17, 17, 17, 17, 17, 17, 17, 17, 18, 19, 17, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }, { { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }, { { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 }, { 1, 15, 15, 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0 }, { 1, 15, 11, 0, 9, 11, 13, 15, 11, 13, 15, 15, 15, 15, 15, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 1, 15, 9, 0, 9, 11, 13, 15, 11, 13, 15, 15, 15, 15, 15, 0 }, { 1, 15, 11, 0, 11, 11, 13, 15, 11, 13, 15, 15, 15, 15, 15, 0 }, { 1, 15, 13, 0, 13, 13, 13, 15, 13, 13, 15, 15, 15, 15, 15, 0 }, { 1, 15, 15, 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0 }, { 1, 15, 11, 0, 11, 11, 13, 15, 10, 12, 14, 16, 15, 15, 15, 0 }, { 1, 15, 13, 0, 13, 13, 13, 15, 12, 12, 14, 16, 15, 15, 15, 0 }, { 1, 15, 15, 0, 15, 15, 15, 15, 14, 14, 14, 16, 15, 15, 15, 0 }, { 1, 15, 15, 0, 15, 15, 15, 15, 16, 16, 16, 16, 15, 15, 15, 0 }, { 1, 15, 15, 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0 }, { 1, 15, 15, 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0 }, { 1, 15, 15, 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }, { { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 }, { 1, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 1, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 }, { 1, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11 }, { 1, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13 }, { 1, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 }, { 1, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }, { 1, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12 }, { 1, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14 }, { 1, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16 }, { 1, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 }, { 1, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 }, { 1, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }, { { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 }, { 1, 7, 7, 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0 }, { 1, 7, 7, 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 1, 7, 7, 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0 }, { 1, 7, 7, 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0 }, { 1, 7, 7, 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0 }, { 1, 7, 7, 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0 }, { 1, 7, 7, 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0 }, { 1, 7, 7, 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0 }, { 1, 7, 7, 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0 }, { 1, 7, 7, 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0 }, { 1, 7, 7, 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0 }, { 1, 7, 7, 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0 }, { 1, 7, 7, 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }, { { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 }, { 1, 15, 7, 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0 }, { 1, 7, 7, 0, 9, 11, 13, 15, 11, 13, 15, 15, 15, 15, 15, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 1, 15, 9, 0, 9, 11, 13, 15, 11, 13, 15, 15, 15, 15, 15, 0 }, { 1, 15, 11, 0, 11, 11, 13, 15, 11, 13, 15, 15, 15, 15, 15, 0 }, { 1, 15, 13, 0, 13, 13, 13, 15, 13, 13, 15, 15, 15, 15, 15, 0 }, { 1, 15, 15, 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0 }, { 1, 15, 11, 0, 11, 11, 13, 15, 10, 12, 14, 16, 15, 15, 15, 0 }, { 1, 15, 13, 0, 13, 13, 13, 15, 12, 12, 14, 16, 15, 15, 15, 0 }, { 1, 15, 15, 0, 15, 15, 15, 15, 14, 14, 14, 16, 15, 15, 15, 0 }, { 1, 15, 15, 0, 15, 15, 15, 15, 16, 16, 16, 16, 15, 15, 15, 0 }, { 1, 15, 15, 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0 }, { 1, 15, 15, 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0 }, { 1, 15, 15, 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }, { { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 20, 7, 20, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 33 }, { 1, 7, 7, 0, 9, 11, 13, 15, 11, 13, 15, 17, 18, 19, 17, 0 }, { 1, 20, 0, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 1, 19, 9, 0, 9, 11, 13, 15, 11, 13, 15, 17, 18, 19, 17, 0 }, { 1, 19, 11, 0, 11, 11, 13, 15, 11, 13, 15, 17, 18, 19, 17, 0 }, { 1, 19, 13, 0, 13, 13, 13, 15, 13, 13, 15, 17, 18, 19, 17, 0 }, { 1, 19, 15, 0, 15, 15, 15, 15, 15, 15, 15, 17, 18, 19, 17, 0 }, { 1, 19, 11, 0, 11, 11, 13, 15, 10, 12, 14, 16, 18, 19, 17, 0 }, { 1, 19, 13, 0, 13, 13, 13, 15, 12, 12, 14, 16, 18, 19, 17, 0 }, { 1, 19, 15, 0, 15, 15, 15, 15, 14, 14, 14, 16, 18, 19, 17, 0 }, { 1, 19, 17, 0, 17, 17, 17, 17, 16, 16, 16, 16, 18, 19, 17, 0 }, { 1, 19, 18, 0, 18, 18, 18, 18, 18, 18, 18, 18, 18, 19, 18, 0 }, { 1, 19, 19, 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0 }, { 1, 19, 17, 0, 17, 17, 17, 17, 17, 17, 17, 17, 18, 19, 17, 0 }, { 1, 33, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 33 } }, { { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 }, { 1, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 }, { 1, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 }, { 1, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 }, { 1, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 }, { 1, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 }, { 1, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 }, { 1, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 }, { 1, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 }, { 1, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 }, { 1, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 }, { 1, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 }, { 1, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 }, { 1, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 }, { 1, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 } } }
			End Sub

			Public Sub New()
				MyBase.New()
			End Sub

			Public Enum TableKind
				Addition
				SubtractionMultiplicationModulo
				Division
				Power
				IntegerDivision
				Shift
				Logical
				Bitwise
				Relational
				ConcatenationLike
			End Enum
		End Class

		Public MustInherit Class Candidate
			Public MustOverride ReadOnly Property Arity As Integer

			Public Overridable ReadOnly Property FixedTypeParameters As BitVector
				Get
					Return BitVector.Null
				End Get
			End Property

			Public Overridable ReadOnly Property IsExtensionMethod As Boolean
				Get
					Return False
				End Get
			End Property

			Public MustOverride ReadOnly Property IsGeneric As Boolean

			Public Overridable ReadOnly Property IsLifted As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overridable ReadOnly Property IsOperator As Boolean
				Get
					Return False
				End Get
			End Property

			Public MustOverride ReadOnly Property ParameterCount As Integer

			Public Overridable ReadOnly Property PrecedenceLevel As Integer
				Get
					Return 0
				End Get
			End Property

			Public MustOverride ReadOnly Property ReceiverType As TypeSymbol

			Public MustOverride ReadOnly Property ReceiverTypeDefinition As TypeSymbol

			Public MustOverride ReadOnly Property ReturnType As TypeSymbol

			Public MustOverride ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)

			Public MustOverride ReadOnly Property UnderlyingSymbol As Symbol

			Protected Sub New()
				MyBase.New()
			End Sub

			Friend MustOverride Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As OverloadResolution.Candidate

			Friend Sub GetAllParameterCounts(ByRef requiredCount As Integer, ByRef maxCount As Integer, ByRef hasParamArray As Boolean)
				maxCount = Me.ParameterCount
				hasParamArray = False
				requiredCount = -1
				Dim num As Integer = maxCount - 1
				Dim num1 As Integer = num
				Dim num2 As Integer = 0
				Do
					Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = Me.Parameters(num2)
					If (num2 = num AndAlso parameterSymbol.IsParamArray) Then
						hasParamArray = True
					ElseIf (Not parameterSymbol.IsOptional) Then
						requiredCount = num2
					End If
					num2 = num2 + 1
				Loop While num2 <= num1
				requiredCount = requiredCount + 1
			End Sub

			Friend MustOverride Function IsOverriddenBy(ByVal otherSymbol As Symbol) As Boolean

			Public MustOverride Function Parameters(ByVal index As Integer) As ParameterSymbol

			Friend Function TryGetNamedParamIndex(ByVal name As String, ByRef index As Integer) As Boolean
				Dim flag As Boolean
				Dim parameterCount As Integer = Me.ParameterCount - 1
				Dim num As Integer = 0
				While True
					If (num > parameterCount) Then
						index = -1
						flag = False
						Exit While
					ElseIf (Not CaseInsensitiveComparison.Equals(name, Me.Parameters(num).Name)) Then
						num = num + 1
					Else
						index = num
						flag = True
						Exit While
					End If
				End While
				Return flag
			End Function
		End Class

		Public Structure CandidateAnalysisResult
			Private _smallFields As Integer

			Public Candidate As OverloadResolution.Candidate

			Public ExpandedParamArrayArgumentsUsed As Integer

			Public EquallyApplicableCandidatesBucket As Integer

			Public ArgsToParamsOpt As ImmutableArray(Of Integer)

			Public ConversionsOpt As ImmutableArray(Of KeyValuePair(Of ConversionKind, MethodSymbol))

			Public ConversionsBackOpt As ImmutableArray(Of KeyValuePair(Of ConversionKind, MethodSymbol))

			Public OptionalArguments As ImmutableArray(Of OverloadResolution.OptionalArgument)

			Public NotInferredTypeArguments As BitVector

			Public TypeArgumentInferenceDiagnosticsOpt As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

			Public ReadOnly Property AllFailedInferenceIsDueToObject As Boolean
				Get
					Return (Me._smallFields And 524288) <> 0
				End Get
			End Property

			Public ReadOnly Property ArgumentMatchingDone As Boolean
				Get
					Return (Me._smallFields And 2048) <> 0
				End Get
			End Property

			Public Property IgnoreExtensionMethods As Boolean
				Get
					Return (Me._smallFields And 4194304) <> 0
				End Get
				Set(ByVal value As Boolean)
					If (value) Then
						Me._smallFields = Me._smallFields Or 4194304
						Return
					End If
					Me._smallFields = Me._smallFields And -4194305
				End Set
			End Property

			Public ReadOnly Property InferenceErrorReasons As InferenceErrorReasons
				Get
					Return DirectCast(CByte(((Me._smallFields And 3145728) >> 20)), InferenceErrorReasons)
				End Get
			End Property

			Public ReadOnly Property InferenceLevel As TypeArgumentInference.InferenceLevel
				Get
					Return DirectCast(CByte(((Me._smallFields And 1536) >> 9)), TypeArgumentInference.InferenceLevel)
				End Get
			End Property

			Public ReadOnly Property IsExpandedParamArrayForm As Boolean
				Get
					Return (Me._smallFields And 256) <> 0
				End Get
			End Property

			Public ReadOnly Property IsIllegalInAttribute As Boolean
				Get
					Return (Me._smallFields And 8388608) <> 0
				End Get
			End Property

			Public ReadOnly Property MaxDelegateRelaxationLevel As ConversionKind
				Get
					Return DirectCast((Me._smallFields And 229376), ConversionKind)
				End Get
			End Property

			Public ReadOnly Property RequiresNarrowingConversion As Boolean
				Get
					Return (Me._smallFields And 4096) <> 0
				End Get
			End Property

			Public ReadOnly Property RequiresNarrowingNotFromNumericConstant As Boolean
				Get
					Return (Me._smallFields And 16384) <> 0
				End Get
			End Property

			Public ReadOnly Property RequiresNarrowingNotFromObject As Boolean
				Get
					Return (Me._smallFields And 8192) <> 0
				End Get
			End Property

			Public ReadOnly Property SomeInferenceFailed As Boolean
				Get
					Return (Me._smallFields And 262144) <> 0
				End Get
			End Property

			Public Property State As OverloadResolution.CandidateAnalysisResultState
				Get
					Return DirectCast(CByte((Me._smallFields And 255)), OverloadResolution.CandidateAnalysisResultState)
				End Get
				Set(ByVal value As OverloadResolution.CandidateAnalysisResultState)
					Me._smallFields = Me._smallFields And -256 Or CInt(value)
				End Set
			End Property

			Public ReadOnly Property UsedOptionalParameterDefaultValue As Boolean
				Get
					Return Not Me.OptionalArguments.IsDefault
				End Get
			End Property

			Public Sub New(ByVal candidate As OverloadResolution.Candidate, ByVal state As OverloadResolution.CandidateAnalysisResultState)
				Me = New OverloadResolution.CandidateAnalysisResult() With
				{
					.Candidate = candidate,
					.State = state
				}
			End Sub

			Public Sub New(ByVal candidate As OverloadResolution.Candidate)
				Me = New OverloadResolution.CandidateAnalysisResult() With
				{
					.Candidate = candidate,
					.State = OverloadResolution.CandidateAnalysisResultState.Applicable
				}
			End Sub

			Public Sub RegisterDelegateRelaxationLevel(ByVal conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind)
				Dim num As Integer = CInt((conversionKind And Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelMask))
				If (num > (Me._smallFields And 229376)) Then
					If (num = 131072) Then
						Me.IgnoreExtensionMethods = False
					End If
					Me._smallFields = Me._smallFields And -229377 Or num
				End If
			End Sub

			Public Sub SetAllFailedInferenceIsDueToObject()
				Me._smallFields = Me._smallFields Or 524288
			End Sub

			Public Sub SetArgumentMatchingDone()
				Me._smallFields = Me._smallFields Or 2048
			End Sub

			Public Sub SetIllegalInAttribute()
				Me._smallFields = Me._smallFields Or 8388608
			End Sub

			Public Sub SetInferenceErrorReasons(ByVal reasons As InferenceErrorReasons)
				Dim num As Integer = CByte(reasons) << 20
				Me._smallFields = Me._smallFields And -3145729 Or num And 3145728
			End Sub

			Public Sub SetInferenceLevel(ByVal level As TypeArgumentInference.InferenceLevel)
				Dim num As Integer = CByte(level) << 9
				Me._smallFields = Me._smallFields And -1537 Or num And 1536
			End Sub

			Public Sub SetIsExpandedParamArrayForm()
				Me._smallFields = Me._smallFields Or 256
			End Sub

			Public Sub SetRequiresNarrowingConversion()
				Me._smallFields = Me._smallFields Or 4096
			End Sub

			Public Sub SetRequiresNarrowingNotFromNumericConstant()
				Me.IgnoreExtensionMethods = False
				Me._smallFields = Me._smallFields Or 16384
			End Sub

			Public Sub SetRequiresNarrowingNotFromObject()
				Me._smallFields = Me._smallFields Or 8192
			End Sub

			Public Sub SetSomeInferenceFailed()
				Me._smallFields = Me._smallFields Or 262144
			End Sub
		End Structure

		Public Enum CandidateAnalysisResultState As Byte
			Applicable
			HasUnsupportedMetadata
			HasUseSiteError
			Ambiguous
			BadGenericArity
			ArgumentCountMismatch
			TypeInferenceFailed
			ArgumentMismatch
			GenericConstraintsViolated
			RequiresNarrowing
			RequiresNarrowingNotFromObject
			ExtensionMethodVsInstanceMethod
			Shadowed
			LessApplicable
			ExtensionMethodVsLateBinding
			Count
		End Enum

		Public NotInheritable Class ExtensionMethodCandidate
			Inherits OverloadResolution.MethodCandidate
			Private _fixedTypeParameters As BitVector

			Public Overrides ReadOnly Property FixedTypeParameters As BitVector
				Get
					Return Me._fixedTypeParameters
				End Get
			End Property

			Public Overrides ReadOnly Property IsExtensionMethod As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overrides ReadOnly Property PrecedenceLevel As Integer
				Get
					Return Me.m_Method.Proximity
				End Get
			End Property

			Public Overrides ReadOnly Property ReceiverType As TypeSymbol
				Get
					Return Me.m_Method.ReceiverType
				End Get
			End Property

			Public Overrides ReadOnly Property ReceiverTypeDefinition As TypeSymbol
				Get
					Return Me.m_Method.ReducedFrom.Parameters(0).Type
				End Get
			End Property

			Public Sub New(ByVal method As MethodSymbol)
				MyClass.New(method, OverloadResolution.ExtensionMethodCandidate.GetFixedTypeParameters(method))
			End Sub

			Private Sub New(ByVal method As MethodSymbol, ByVal fixedTypeParameters As BitVector)
				MyBase.New(method)
				Me._fixedTypeParameters = fixedTypeParameters
			End Sub

			Friend Overrides Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As OverloadResolution.Candidate
				Return New OverloadResolution.ExtensionMethodCandidate(Me.m_Method.Construct(typeArguments), Me._fixedTypeParameters)
			End Function

			Private Shared Function GetFixedTypeParameters(ByVal method As MethodSymbol) As Microsoft.CodeAnalysis.BitVector
				Dim bitVector As Microsoft.CodeAnalysis.BitVector
				If (method.FixedTypeParameters.Length <= 0) Then
					bitVector = New Microsoft.CodeAnalysis.BitVector()
				Else
					Dim bitVector1 As Microsoft.CodeAnalysis.BitVector = Microsoft.CodeAnalysis.BitVector.Create(method.ReducedFrom.Arity)
					Dim enumerator As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeSymbol)).Enumerator = method.FixedTypeParameters.GetEnumerator()
					While enumerator.MoveNext()
						bitVector1(enumerator.Current.Key.Ordinal) = True
					End While
					bitVector = bitVector1
				End If
				Return bitVector
			End Function

			Friend Overrides Function IsOverriddenBy(ByVal otherSymbol As Symbol) As Boolean
				Return False
			End Function
		End Class

		Private Class InferenceLevelComparer
			Implements IComparer(Of Integer)
			Private ReadOnly _candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult)

			Public Sub New(ByVal candidates As ArrayBuilder(Of OverloadResolution.CandidateAnalysisResult))
				MyBase.New()
				Me._candidates = candidates
			End Sub

			Public Function Compare(ByVal indexX As Integer, ByVal indexY As Integer) As Integer Implements IComparer(Of Integer).Compare
				Dim item As OverloadResolution.CandidateAnalysisResult = Me._candidates(indexX)
				Dim inferenceLevel As Integer = CInt(item.InferenceLevel)
				item = Me._candidates(indexY)
				Return inferenceLevel.CompareTo(CInt(item.InferenceLevel))
			End Function
		End Class

		Public Class LiftedOperatorCandidate
			Inherits OverloadResolution.OperatorCandidate
			Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

			Private ReadOnly _returnType As TypeSymbol

			Public Overrides ReadOnly Property IsLifted As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overrides ReadOnly Property ParameterCount As Integer
				Get
					Return Me._parameters.Length
				End Get
			End Property

			Public Overrides ReadOnly Property ReturnType As TypeSymbol
				Get
					Return Me._returnType
				End Get
			End Property

			Public Sub New(ByVal method As MethodSymbol, ByVal parameters As ImmutableArray(Of ParameterSymbol), ByVal returnType As TypeSymbol)
				MyBase.New(method)
				Me._parameters = parameters
				Me._returnType = returnType
			End Sub

			Public Overrides Function Parameters(ByVal index As Integer) As ParameterSymbol
				Return Me._parameters(index)
			End Function
		End Class

		Private NotInheritable Class LiftedParameterSymbol
			Inherits ParameterSymbol
			Private ReadOnly _parameterToLift As ParameterSymbol

			Private ReadOnly _type As TypeSymbol

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me._parameterToLift.ContainingSymbol
				End Get
			End Property

			Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
				Get
					Return Me._parameterToLift.CustomModifiers
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
				Get
					Return ImmutableArray(Of SyntaxReference).Empty
				End Get
			End Property

			Friend Overrides ReadOnly Property ExplicitDefaultConstantValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol)) As ConstantValue
				Get
					Return Me._parameterToLift(inProgress)
				End Get
			End Property

			Public Overrides ReadOnly Property HasExplicitDefaultValue As Boolean
				Get
					Return Me._parameterToLift.HasExplicitDefaultValue
				End Get
			End Property

			Friend Overrides ReadOnly Property HasOptionCompare As Boolean
				Get
					Return Me._parameterToLift.HasOptionCompare
				End Get
			End Property

			Public Overrides ReadOnly Property IsByRef As Boolean
				Get
					Return Me._parameterToLift.IsByRef
				End Get
			End Property

			Friend Overrides ReadOnly Property IsCallerFilePath As Boolean
				Get
					Return Me._parameterToLift.IsCallerFilePath
				End Get
			End Property

			Friend Overrides ReadOnly Property IsCallerLineNumber As Boolean
				Get
					Return Me._parameterToLift.IsCallerLineNumber
				End Get
			End Property

			Friend Overrides ReadOnly Property IsCallerMemberName As Boolean
				Get
					Return Me._parameterToLift.IsCallerMemberName
				End Get
			End Property

			Friend Overrides ReadOnly Property IsExplicitByRef As Boolean
				Get
					Return Me._parameterToLift.IsExplicitByRef
				End Get
			End Property

			Friend Overrides ReadOnly Property IsIDispatchConstant As Boolean
				Get
					Return Me._parameterToLift.IsIDispatchConstant
				End Get
			End Property

			Friend Overrides ReadOnly Property IsIUnknownConstant As Boolean
				Get
					Return Me._parameterToLift.IsIUnknownConstant
				End Get
			End Property

			Friend Overrides ReadOnly Property IsMetadataIn As Boolean
				Get
					Return Me._parameterToLift.IsMetadataIn
				End Get
			End Property

			Friend Overrides ReadOnly Property IsMetadataOut As Boolean
				Get
					Return Me._parameterToLift.IsMetadataOut
				End Get
			End Property

			Public Overrides ReadOnly Property IsOptional As Boolean
				Get
					Return Me._parameterToLift.IsOptional
				End Get
			End Property

			Public Overrides ReadOnly Property IsParamArray As Boolean
				Get
					Return Me._parameterToLift.IsParamArray
				End Get
			End Property

			Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
				Get
					Return ImmutableArray(Of Location).Empty
				End Get
			End Property

			Friend Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
				Get
					Return Me._parameterToLift.MarshallingInformation
				End Get
			End Property

			Public Overrides ReadOnly Property Name As String
				Get
					Return Me._parameterToLift.Name
				End Get
			End Property

			Public Overrides ReadOnly Property Ordinal As Integer
				Get
					Return Me._parameterToLift.Ordinal
				End Get
			End Property

			Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
				Get
					Return Me._parameterToLift.RefCustomModifiers
				End Get
			End Property

			Public Overrides ReadOnly Property Type As TypeSymbol
				Get
					Return Me._type
				End Get
			End Property

			Public Sub New(ByVal parameter As ParameterSymbol, ByVal type As TypeSymbol)
				MyBase.New()
				Me._parameterToLift = parameter
				Me._type = type
			End Sub

			Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
				Return Me._parameterToLift.GetUseSiteInfo()
			End Function
		End Class

		Public Class MethodCandidate
			Inherits OverloadResolution.Candidate
			Protected ReadOnly m_Method As MethodSymbol

			Public Overrides ReadOnly Property Arity As Integer
				Get
					Return Me.m_Method.Arity
				End Get
			End Property

			Public Overrides ReadOnly Property IsGeneric As Boolean
				Get
					Return Me.m_Method.IsGenericMethod
				End Get
			End Property

			Public Overrides ReadOnly Property ParameterCount As Integer
				Get
					Return Me.m_Method.ParameterCount
				End Get
			End Property

			Public Overrides ReadOnly Property ReceiverType As TypeSymbol
				Get
					Return Me.m_Method.ContainingType
				End Get
			End Property

			Public Overrides ReadOnly Property ReceiverTypeDefinition As TypeSymbol
				Get
					Return Me.m_Method.ContainingType
				End Get
			End Property

			Public Overrides ReadOnly Property ReturnType As TypeSymbol
				Get
					Return Me.m_Method.ReturnType
				End Get
			End Property

			Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
				Get
					Return Me.m_Method.TypeParameters
				End Get
			End Property

			Public Overrides ReadOnly Property UnderlyingSymbol As Symbol
				Get
					Return Me.m_Method
				End Get
			End Property

			Public Sub New(ByVal method As MethodSymbol)
				MyBase.New()
				Me.m_Method = method
			End Sub

			Friend Overrides Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As OverloadResolution.Candidate
				Return New OverloadResolution.MethodCandidate(Me.m_Method.Construct(typeArguments))
			End Function

			Friend Overrides Function IsOverriddenBy(ByVal otherSymbol As Symbol) As Boolean
				Dim flag As Boolean
				Dim originalDefinition As MethodSymbol = Me.m_Method.OriginalDefinition
				If (originalDefinition.IsOverridable OrElse originalDefinition.IsOverrides OrElse originalDefinition.IsMustOverride) Then
					Dim overriddenMethod As MethodSymbol = DirectCast(otherSymbol, MethodSymbol).OverriddenMethod
					While overriddenMethod IsNot Nothing
						If (Not overriddenMethod.OriginalDefinition.Equals(originalDefinition)) Then
							overriddenMethod = overriddenMethod.OverriddenMethod
						Else
							flag = True
							Return flag
						End If
					End While
				End If
				flag = False
				Return flag
			End Function

			Public Overrides Function Parameters(ByVal index As Integer) As ParameterSymbol
				Return Me.m_Method.Parameters(index)
			End Function
		End Class

		Public Class OperatorCandidate
			Inherits OverloadResolution.MethodCandidate
			Public NotOverridable Overrides ReadOnly Property IsOperator As Boolean
				Get
					Return True
				End Get
			End Property

			Public Sub New(ByVal method As MethodSymbol)
				MyBase.New(method)
			End Sub
		End Class

		Friend Structure OperatorInfo
			Private ReadOnly _Id As Integer

			Public ReadOnly Property BinaryOperatorKind As BinaryOperatorKind
				Get
					Dim binaryOperatorKind1 As BinaryOperatorKind
					If (Me.IsBinary) Then
						binaryOperatorKind1 = Me._Id >> 2
					Else
						binaryOperatorKind1 = BinaryOperatorKind.[Error]
					End If
					Return binaryOperatorKind1
				End Get
			End Property

			Public ReadOnly Property IsBinary As Boolean
				Get
					Return Me.ParamCount = 2
				End Get
			End Property

			Public ReadOnly Property IsUnary As Boolean
				Get
					Return Me.ParamCount = 1
				End Get
			End Property

			Public ReadOnly Property ParamCount As Integer
				Get
					Return Me._Id And 3
				End Get
			End Property

			Public ReadOnly Property UnaryOperatorKind As UnaryOperatorKind
				Get
					Dim unaryOperatorKind1 As UnaryOperatorKind
					If (Me.IsUnary) Then
						unaryOperatorKind1 = Me._Id >> 2
					Else
						unaryOperatorKind1 = UnaryOperatorKind.[Error]
					End If
					Return unaryOperatorKind1
				End Get
			End Property

			Public Sub New(ByVal op As UnaryOperatorKind)
				Me = New OverloadResolution.OperatorInfo() With
				{
					._Id = 1 Or CInt(op) << CInt(UnaryOperatorKind.Minus)
				}
			End Sub

			Public Sub New(ByVal op As BinaryOperatorKind)
				Me = New OverloadResolution.OperatorInfo() With
				{
					._Id = 2 Or CInt(op) << CInt(BinaryOperatorKind.Concatenate)
				}
			End Sub
		End Structure

		Public Structure OptionalArgument
			Public ReadOnly DefaultValue As BoundExpression

			Public ReadOnly Conversion As KeyValuePair(Of ConversionKind, MethodSymbol)

			Public ReadOnly Dependencies As ImmutableArray(Of AssemblySymbol)

			Public Sub New(ByVal value As BoundExpression, ByVal conversion As KeyValuePair(Of ConversionKind, MethodSymbol), ByVal dependencies As ImmutableArray(Of AssemblySymbol))
				Me = New OverloadResolution.OptionalArgument() With
				{
					.DefaultValue = value,
					.Conversion = conversion,
					.Dependencies = Microsoft.CodeAnalysis.ImmutableArrayExtensions.NullToEmpty(Of AssemblySymbol)(dependencies)
				}
			End Sub
		End Structure

		Friend Structure OverloadResolutionResult
			Private ReadOnly _bestResult As Nullable(Of OverloadResolution.CandidateAnalysisResult)

			Private ReadOnly _allResults As ImmutableArray(Of OverloadResolution.CandidateAnalysisResult)

			Private ReadOnly _resolutionIsLateBound As Boolean

			Private ReadOnly _remainingCandidatesRequireNarrowingConversion As Boolean

			Public ReadOnly AsyncLambdaSubToFunctionMismatch As ImmutableArray(Of BoundExpression)

			Public ReadOnly Property BestResult As Nullable(Of OverloadResolution.CandidateAnalysisResult)
				Get
					Return Me._bestResult
				End Get
			End Property

			Public ReadOnly Property Candidates As ImmutableArray(Of OverloadResolution.CandidateAnalysisResult)
				Get
					Return Me._allResults
				End Get
			End Property

			Public ReadOnly Property RemainingCandidatesRequireNarrowingConversion As Boolean
				Get
					Return Me._remainingCandidatesRequireNarrowingConversion
				End Get
			End Property

			Public ReadOnly Property ResolutionIsLateBound As Boolean
				Get
					Return Me._resolutionIsLateBound
				End Get
			End Property

			Public Sub New(ByVal allResults As ImmutableArray(Of OverloadResolution.CandidateAnalysisResult), ByVal resolutionIsLateBound As Boolean, ByVal remainingCandidatesRequireNarrowingConversion As Boolean, ByVal asyncLambdaSubToFunctionMismatch As HashSet(Of BoundExpression))
				Dim empty As ImmutableArray(Of BoundExpression)
				Me = New OverloadResolution.OverloadResolutionResult() With
				{
					._allResults = allResults,
					._resolutionIsLateBound = resolutionIsLateBound,
					._remainingCandidatesRequireNarrowingConversion = remainingCandidatesRequireNarrowingConversion
				}
				If (asyncLambdaSubToFunctionMismatch Is Nothing) Then
					empty = ImmutableArray(Of BoundExpression).Empty
				Else
					empty = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(asyncLambdaSubToFunctionMismatch.ToArray())
				End If
				Me.AsyncLambdaSubToFunctionMismatch = empty
				If (Not resolutionIsLateBound) Then
					Me._bestResult = OverloadResolution.OverloadResolutionResult.GetBestResult(allResults)
				End If
			End Sub

			Private Shared Function GetBestResult(ByVal allResults As ImmutableArray(Of OverloadResolution.CandidateAnalysisResult)) As Nullable(Of OverloadResolution.CandidateAnalysisResult)
				Dim nullable As Nullable(Of OverloadResolution.CandidateAnalysisResult)
				Dim nullable1 As Nullable(Of OverloadResolution.CandidateAnalysisResult) = Nothing
				Dim num As Integer = 0
				While True
					If (num < allResults.Length) Then
						Dim item As OverloadResolution.CandidateAnalysisResult = allResults(num)
						If (item.State = OverloadResolution.CandidateAnalysisResultState.Applicable) Then
							If (Not nullable1.HasValue) Then
								nullable1 = New Nullable(Of OverloadResolution.CandidateAnalysisResult)(item)
							Else
								nullable = Nothing
								Exit While
							End If
						End If
						num = num + 1
					Else
						nullable = nullable1
						Exit While
					End If
				End While
				Return nullable
			End Function
		End Structure

		Public NotInheritable Class PropertyCandidate
			Inherits OverloadResolution.Candidate
			Private ReadOnly _property As PropertySymbol

			Public Overrides ReadOnly Property Arity As Integer
				Get
					Return 0
				End Get
			End Property

			Public Overrides ReadOnly Property IsGeneric As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property ParameterCount As Integer
				Get
					Return Me._property.Parameters.Length
				End Get
			End Property

			Public Overrides ReadOnly Property ReceiverType As TypeSymbol
				Get
					Return Me._property.ContainingType
				End Get
			End Property

			Public Overrides ReadOnly Property ReceiverTypeDefinition As TypeSymbol
				Get
					Return Me._property.ContainingType
				End Get
			End Property

			Public Overrides ReadOnly Property ReturnType As TypeSymbol
				Get
					Return Me._property.Type
				End Get
			End Property

			Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
				Get
					Return ImmutableArray(Of TypeParameterSymbol).Empty
				End Get
			End Property

			Public Overrides ReadOnly Property UnderlyingSymbol As Symbol
				Get
					Return Me._property
				End Get
			End Property

			Public Sub New(ByVal [property] As PropertySymbol)
				MyBase.New()
				Me._property = [property]
			End Sub

			Friend Overrides Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As OverloadResolution.Candidate
				Throw ExceptionUtilities.Unreachable
			End Function

			Friend Overrides Function IsOverriddenBy(ByVal otherSymbol As Symbol) As Boolean
				Dim flag As Boolean
				Dim originalDefinition As PropertySymbol = Me._property.OriginalDefinition
				If (originalDefinition.IsOverridable OrElse originalDefinition.IsOverrides OrElse originalDefinition.IsMustOverride) Then
					Dim overriddenProperty As PropertySymbol = DirectCast(otherSymbol, PropertySymbol).OverriddenProperty
					While overriddenProperty IsNot Nothing
						If (Not overriddenProperty.OriginalDefinition.Equals(originalDefinition)) Then
							overriddenProperty = overriddenProperty.OverriddenProperty
						Else
							flag = True
							Return flag
						End If
					End While
				End If
				flag = False
				Return flag
			End Function

			Public Overrides Function Parameters(ByVal index As Integer) As ParameterSymbol
				Return Me._property.Parameters(index)
			End Function
		End Class

		Private Structure QuickApplicabilityInfo
			Public ReadOnly Candidate As OverloadResolution.Candidate

			Public ReadOnly State As OverloadResolution.CandidateAnalysisResultState

			Public ReadOnly AppliesToNormalForm As Boolean

			Public ReadOnly AppliesToParamArrayForm As Boolean

			Public Sub New(ByVal candidate As OverloadResolution.Candidate, ByVal state As OverloadResolution.CandidateAnalysisResultState, Optional ByVal appliesToNormalForm As Boolean = True, Optional ByVal appliesToParamArrayForm As Boolean = True)
				Me = New OverloadResolution.QuickApplicabilityInfo() With
				{
					.Candidate = candidate,
					.State = state,
					.AppliesToNormalForm = appliesToNormalForm,
					.AppliesToParamArrayForm = appliesToParamArrayForm
				}
			End Sub
		End Structure

		<Flags>
		Private Enum SmallFieldMask
			InferenceLevelShift = 9
			InferenceErrorReasonsShift = 20
			State = 255
			IsExpandedParamArrayForm = 256
			InferenceLevelMask = 1536
			ArgumentMatchingDone = 2048
			RequiresNarrowingConversion = 4096
			RequiresNarrowingNotFromObject = 8192
			RequiresNarrowingNotFromNumericConstant = 16384
			DelegateRelaxationLevelMask = 229376
			SomeInferenceFailed = 262144
			AllFailedInferenceIsDueToObject = 524288
			InferenceErrorReasonsMask = 3145728
			IgnoreExtensionMethods = 4194304
			IllegalInAttribute = 8388608
		End Enum

		<Flags>
		Private Enum TypeParameterKind
			None
			Method
			Type
			Both
		End Enum
	End Class
End Namespace