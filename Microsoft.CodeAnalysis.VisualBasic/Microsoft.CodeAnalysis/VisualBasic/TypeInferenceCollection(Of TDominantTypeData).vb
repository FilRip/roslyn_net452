Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class TypeInferenceCollection(Of TDominantTypeData As DominantTypeData)
		Private ReadOnly _dominantTypeDataList As ArrayBuilder(Of TDominantTypeData)

		Public Sub New()
			MyBase.New()
			Me._dominantTypeDataList = New ArrayBuilder(Of TDominantTypeData)()
		End Sub

		Private Shared Sub AppendArrayElements(ByVal source As BoundArrayInitialization, ByVal elements As ArrayBuilder(Of BoundExpression))
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = source.Initializers.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As BoundExpression = enumerator.Current
				If (current.Kind <> BoundKind.ArrayInitialization) Then
					elements.Add(current)
				Else
					TypeInferenceCollection(Of TDominantTypeData).AppendArrayElements(DirectCast(current, BoundArrayInitialization), elements)
				End If
			End While
		End Sub

		Private Function CheckHintSatisfaction(ByVal candidateData As DominantTypeData, ByVal hintData As DominantTypeData, ByVal hintRestrictions As RequiredConversion, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As TypeInferenceCollection(Of TDominantTypeData).HintSatisfaction
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.TypeInferenceCollection`1/HintSatisfaction<TDominantTypeData> Microsoft.CodeAnalysis.VisualBasic.TypeInferenceCollection`1::CheckHintSatisfaction(Microsoft.CodeAnalysis.VisualBasic.DominantTypeData,Microsoft.CodeAnalysis.VisualBasic.DominantTypeData,Microsoft.CodeAnalysis.VisualBasic.RequiredConversion,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.TypeInferenceCollection<TDominantTypeData>/HintSatisfaction<TDominantTypeData> CheckHintSatisfaction(Microsoft.CodeAnalysis.VisualBasic.DominantTypeData,Microsoft.CodeAnalysis.VisualBasic.DominantTypeData,Microsoft.CodeAnalysis.VisualBasic.RequiredConversion,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
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

		Friend Sub FindDominantType(ByVal resultList As ArrayBuilder(Of TDominantTypeData), ByRef inferenceErrorReasons As Microsoft.CodeAnalysis.VisualBasic.InferenceErrorReasons, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim count As Integer
			Dim key As ConversionKind
			Dim keyValuePair As KeyValuePair(Of ConversionKind, MethodSymbol)
			resultList.Clear()
			Dim tDominantTypeDatum As TDominantTypeData = Nothing
			Dim tDominantTypeDatum1 As TDominantTypeData = Nothing
			Dim num As Integer = 0
			Dim num1 As Integer = 0
			Dim instance As ArrayBuilder(Of Integer) = ArrayBuilder(Of Integer).GetInstance(4, 0)
			Dim enumerator As ArrayBuilder(Of TDominantTypeData).Enumerator = Me._dominantTypeDataList.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As TDominantTypeData = enumerator.Current
				If (current.ResultType IsNot Nothing) Then
					instance.ZeroInit(4)
					Dim enumerator1 As ArrayBuilder(Of TDominantTypeData).Enumerator = Me._dominantTypeDataList.GetEnumerator()
					While enumerator1.MoveNext()
						Dim current1 As TDominantTypeData = enumerator1.Current
						Dim hintSatisfaction As TypeInferenceCollection(Of TDominantTypeData).HintSatisfaction = Me.CheckHintSatisfaction(DirectCast(current, DominantTypeData), DirectCast(current1, DominantTypeData), current1.InferenceRestrictions, useSiteInfo)
						Dim item As ArrayBuilder(Of Integer) = instance
						Dim nums As ArrayBuilder(Of Integer) = item
						Dim hintSatisfaction1 As TypeInferenceCollection(Of TDominantTypeData).HintSatisfaction = hintSatisfaction
						count = CInt(hintSatisfaction1)
						item(CInt(hintSatisfaction1)) = nums(count) + 1
					End While
					current.IsStrictCandidate = If(instance(3) <> 0, False, instance(2) = 0)
					current.IsUnstrictCandidate = instance(3) = 0
					If (current.IsStrictCandidate) Then
						num = num + 1
						tDominantTypeDatum = current
					End If
					If (Not current.IsUnstrictCandidate) Then
						Continue While
					End If
					num1 = num1 + 1
					tDominantTypeDatum1 = current
				Else
					current.IsStrictCandidate = False
					current.IsUnstrictCandidate = False
				End If
			End While
			instance.Free()
			If (num = 1) Then
				resultList.Add(tDominantTypeDatum)
				Return
			End If
			If (num = 0 AndAlso num1 = 1) Then
				resultList.Add(tDominantTypeDatum1)
				Return
			End If
			If (num1 = 0) Then
				inferenceErrorReasons = CByte(inferenceErrorReasons) Or 2
				Return
			End If
			If (num = 0) Then
				Dim enumerator2 As ArrayBuilder(Of TDominantTypeData).Enumerator = Me._dominantTypeDataList.GetEnumerator()
				While enumerator2.MoveNext()
					Dim current2 As TDominantTypeData = enumerator2.Current
					If (Not current2.IsUnstrictCandidate) Then
						Continue While
					End If
					resultList.Add(current2)
				End While
				inferenceErrorReasons = CByte(inferenceErrorReasons) Or 1
				Return
			End If
			Dim enumerator3 As ArrayBuilder(Of TDominantTypeData).Enumerator = Me._dominantTypeDataList.GetEnumerator()
			While enumerator3.MoveNext()
				Dim tDominantTypeDatum2 As TDominantTypeData = enumerator3.Current
				If (Not tDominantTypeDatum2.IsStrictCandidate) Then
					Continue While
				End If
				Dim flag As Boolean = True
				Dim enumerator4 As ArrayBuilder(Of TDominantTypeData).Enumerator = Me._dominantTypeDataList.GetEnumerator()
				While enumerator4.MoveNext()
					Dim current3 As TDominantTypeData = enumerator4.Current
					If (Not current3.IsStrictCandidate OrElse tDominantTypeDatum2 = current3 OrElse tDominantTypeDatum2.ResultType Is Nothing OrElse current3.ResultType Is Nothing) Then
						Continue While
					End If
					Dim resultType As Microsoft.CodeAnalysis.VisualBasic.ArrayLiteralTypeSymbol = TryCast(current3.ResultType, Microsoft.CodeAnalysis.VisualBasic.ArrayLiteralTypeSymbol)
					If (resultType IsNot Nothing) Then
						Dim arrayLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundArrayLiteral = resultType.ArrayLiteral
						keyValuePair = Conversions.ClassifyConversion(arrayLiteral, tDominantTypeDatum2.ResultType, arrayLiteral.Binder, useSiteInfo)
						key = keyValuePair.Key
						If (Conversions.IsWideningConversion(key) AndAlso resultType.IsSameTypeIgnoringAll(tDominantTypeDatum2.ResultType)) Then
							key = ConversionKind.Identity
						End If
					Else
						keyValuePair = Conversions.ClassifyConversion(current3.ResultType, tDominantTypeDatum2.ResultType, useSiteInfo)
						key = keyValuePair.Key
					End If
					If (Conversions.IsWideningConversion(key)) Then
						Continue While
					End If
					flag = False
					Exit While
				End While
				If (Not flag) Then
					Continue While
				End If
				resultList.Add(tDominantTypeDatum2)
			End While
			If (resultList.Count > 1) Then
				Dim item1 As Integer = -1
				count = resultList.Count - 1
				Dim num2 As Integer = 0
				Do
					If (Not TypeOf resultList(num2).ResultType Is Microsoft.CodeAnalysis.VisualBasic.ArrayLiteralTypeSymbol) Then
						item1 = item1 + 1
						If (item1 <> num2) Then
							resultList(item1) = resultList(num2)
						End If
					End If
					num2 = num2 + 1
				Loop While num2 <= count
				If (item1 <= -1) Then
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = resultList(0).ResultType
					Dim count1 As Integer = resultList.Count - 1
					Dim num3 As Integer = 1
					While num3 <= count1
						If (resultList(num3).ResultType.IsSameTypeIgnoringAll(typeSymbol)) Then
							num3 = num3 + 1
						Else
							typeSymbol = Nothing
							Exit While
						End If
					End While
					If (typeSymbol Is Nothing) Then
						Dim rank As Integer = DirectCast(resultList(0).ResultType, Microsoft.CodeAnalysis.VisualBasic.ArrayLiteralTypeSymbol).Rank
						Dim count2 As Integer = resultList.Count - 1
						Dim num4 As Integer = 1
						While num4 <= count2
							If (DirectCast(resultList(num4).ResultType, Microsoft.CodeAnalysis.VisualBasic.ArrayLiteralTypeSymbol).Rank = rank) Then
								num4 = num4 + 1
							Else
								rank = -1
								Exit While
							End If
						End While
						If (rank <> -1) Then
							Dim boundExpressions As ArrayBuilder(Of BoundExpression) = ArrayBuilder(Of BoundExpression).GetInstance()
							Dim enumerator5 As ArrayBuilder(Of TDominantTypeData).Enumerator = resultList.GetEnumerator()
							While enumerator5.MoveNext()
								TypeInferenceCollection(Of TDominantTypeData).AppendArrayElements(DirectCast(enumerator5.Current.ResultType, Microsoft.CodeAnalysis.VisualBasic.ArrayLiteralTypeSymbol).ArrayLiteral.Initializer, boundExpressions)
							End While
							Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(Nothing, PooledHashSet(Of AssemblySymbol).GetInstance())
							Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = DirectCast(resultList(0).ResultType, Microsoft.CodeAnalysis.VisualBasic.ArrayLiteralTypeSymbol).ArrayLiteral.Binder
							Dim dummy As VisualBasicSyntaxTree = VisualBasicSyntaxTree.Dummy
							Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
							Dim num5 As Integer = 0
							Dim inferenceErrorReason As Microsoft.CodeAnalysis.VisualBasic.InferenceErrorReasons = Microsoft.CodeAnalysis.VisualBasic.InferenceErrorReasons.Other
							Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = binder.InferDominantTypeOfExpressions(dummy.GetRoot(cancellationToken), boundExpressions, bindingDiagnosticBag, num5, inferenceErrorReason)
							If (typeSymbol1 IsNot Nothing) Then
								useSiteInfo.AddDependencies(bindingDiagnosticBag.DependenciesBag)
								Dim tDominantTypeDatum3 As TDominantTypeData = Nothing
								Dim boundArrayLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundArrayLiteral = Nothing
								Dim enumerator6 As ArrayBuilder(Of TDominantTypeData).Enumerator = resultList.GetEnumerator()
								While enumerator6.MoveNext()
									Dim current4 As TDominantTypeData = enumerator6.Current
									Dim arrayLiteralTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.ArrayLiteralTypeSymbol = DirectCast(current4.ResultType, Microsoft.CodeAnalysis.VisualBasic.ArrayLiteralTypeSymbol)
									If (Not arrayLiteralTypeSymbol.ElementType.IsSameTypeIgnoringAll(typeSymbol1)) Then
										Continue While
									End If
									Dim arrayLiteral1 As Microsoft.CodeAnalysis.VisualBasic.BoundArrayLiteral = arrayLiteralTypeSymbol.ArrayLiteral
									If (tDominantTypeDatum3 IsNot Nothing AndAlso (Not arrayLiteral1.HasDominantType OrElse boundArrayLiteral.HasDominantType AndAlso boundArrayLiteral.NumberOfCandidates >= arrayLiteral1.NumberOfCandidates)) Then
										Continue While
									End If
									tDominantTypeDatum3 = current4
									boundArrayLiteral = arrayLiteralTypeSymbol.ArrayLiteral
								End While
								If (tDominantTypeDatum3 IsNot Nothing) Then
									resultList.Clear()
									resultList.Add(tDominantTypeDatum3)
								End If
							End If
							bindingDiagnosticBag.Free()
						End If
					Else
						resultList.Clip(1)
					End If
				Else
					resultList.Clip(item1 + 1)
				End If
			End If
			If (resultList.Count <> 1) Then
				If (resultList.Count > 1) Then
					inferenceErrorReasons = CByte(inferenceErrorReasons) Or 1
					Return
				End If
				Dim enumerator7 As ArrayBuilder(Of TDominantTypeData).Enumerator = Me._dominantTypeDataList.GetEnumerator()
				While enumerator7.MoveNext()
					Dim tDominantTypeDatum4 As TDominantTypeData = enumerator7.Current
					If (Not tDominantTypeDatum4.IsStrictCandidate) Then
						Continue While
					End If
					resultList.Add(tDominantTypeDatum4)
				End While
				inferenceErrorReasons = CByte(inferenceErrorReasons) Or 1
			End If
		End Sub

		Public Function GetTypeDataList() As ArrayBuilder(Of TDominantTypeData)
			Return Me._dominantTypeDataList
		End Function

		Public Enum HintSatisfaction
			ThroughIdentity
			ThroughWidening
			ThroughNarrowing
			Unsatisfied
			Count
		End Enum
	End Class
End Namespace