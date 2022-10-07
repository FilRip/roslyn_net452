Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class MethodSignatureComparer
		Implements IEqualityComparer(Of MethodSymbol)
		Public ReadOnly Shared RuntimeMethodSignatureComparer As MethodSignatureComparer

		Public ReadOnly Shared AllAspectsSignatureComparer As MethodSignatureComparer

		Public ReadOnly Shared ParametersAndReturnTypeSignatureComparer As MethodSignatureComparer

		Public ReadOnly Shared CustomModifiersAndParametersAndReturnTypeSignatureComparer As MethodSignatureComparer

		Public ReadOnly Shared VisualBasicSignatureAndConstraintsAndReturnTypeComparer As MethodSignatureComparer

		Public ReadOnly Shared RetargetedExplicitMethodImplementationComparer As MethodSignatureComparer

		Public ReadOnly Shared WinRTConflictComparer As MethodSignatureComparer

		Private ReadOnly _considerName As Boolean

		Private ReadOnly _considerReturnType As Boolean

		Private ReadOnly _considerTypeConstraints As Boolean

		Private ReadOnly _considerCallingConvention As Boolean

		Private ReadOnly _considerByRef As Boolean

		Private ReadOnly _considerCustomModifiers As Boolean

		Private ReadOnly _considerTupleNames As Boolean

		Shared Sub New()
			MethodSignatureComparer.RuntimeMethodSignatureComparer = New MethodSignatureComparer(True, True, False, True, True, True, False)
			MethodSignatureComparer.AllAspectsSignatureComparer = New MethodSignatureComparer(True, True, True, True, True, True, True)
			MethodSignatureComparer.ParametersAndReturnTypeSignatureComparer = New MethodSignatureComparer(False, True, False, False, True, False, False)
			MethodSignatureComparer.CustomModifiersAndParametersAndReturnTypeSignatureComparer = New MethodSignatureComparer(False, True, False, False, True, True, False)
			MethodSignatureComparer.VisualBasicSignatureAndConstraintsAndReturnTypeComparer = New MethodSignatureComparer(True, True, True, True, True, False, False)
			MethodSignatureComparer.RetargetedExplicitMethodImplementationComparer = New MethodSignatureComparer(True, True, False, True, True, True, False)
			MethodSignatureComparer.WinRTConflictComparer = New MethodSignatureComparer(True, False, False, False, False, False, False)
		End Sub

		Private Sub New(ByVal considerName As Boolean, ByVal considerReturnType As Boolean, ByVal considerTypeConstraints As Boolean, ByVal considerCallingConvention As Boolean, ByVal considerByRef As Boolean, ByVal considerCustomModifiers As Boolean, ByVal considerTupleNames As Boolean)
			MyBase.New()
			Me._considerName = considerName
			Me._considerReturnType = considerReturnType
			Me._considerTypeConstraints = considerTypeConstraints
			Me._considerCallingConvention = considerCallingConvention
			Me._considerByRef = considerByRef
			Me._considerCustomModifiers = considerCustomModifiers
			Me._considerTupleNames = considerTupleNames
		End Sub

		Private Shared Function AreConstraintTypesSubset(ByVal constraintTypes1 As ArrayBuilder(Of TypeSymbol), ByVal constraintTypes2 As ArrayBuilder(Of TypeSymbol)) As Boolean
			Dim flag As Boolean
			Dim enumerator As ArrayBuilder(Of TypeSymbol).Enumerator = constraintTypes1.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As TypeSymbol = enumerator.Current
					If (Not current.IsObjectType() AndAlso Not MethodSignatureComparer.ContainsIgnoringCustomModifiers(constraintTypes2, current)) Then
						flag = False
						Exit While
					End If
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Shared Function ContainsIgnoringCustomModifiers(ByVal types As ArrayBuilder(Of TypeSymbol), ByVal type As TypeSymbol) As Boolean
			Dim flag As Boolean
			Dim enumerator As ArrayBuilder(Of TypeSymbol).Enumerator = types.GetEnumerator()
			While True
				If (Not enumerator.MoveNext()) Then
					flag = False
					Exit While
				ElseIf (enumerator.Current.IsSameTypeIgnoringAll(type)) Then
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Public Shared Function DetailedCompare(ByVal method1 As MethodSymbol, ByVal method2 As MethodSymbol, ByVal comparisons As SymbolComparisonResults, Optional ByVal stopIfAny As SymbolComparisonResults = 0) As SymbolComparisonResults
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSignatureComparer::DetailedCompare(Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults DetailedCompare(Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults)
			' 
			' File d'attente vide.
			'    à System.ThrowHelper.ThrowInvalidOperationException(ExceptionResource resource)
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.(ICollection`1 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 525
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 445
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 363
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 307
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 86
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Shared Function DetailedParameterCompare(ByVal params1 As ImmutableArray(Of ParameterSymbol), <InAttribute> ByRef lazyTypeSubstitution1 As MethodSignatureComparer.LazyTypeSubstitution, ByVal params2 As ImmutableArray(Of ParameterSymbol), <InAttribute> ByRef lazyTypeSubstitution2 As MethodSignatureComparer.LazyTypeSubstitution, ByVal comparisons As SymbolComparisonResults, Optional ByVal stopIfAny As SymbolComparisonResults = 0) As SymbolComparisonResults
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSignatureComparer::DetailedParameterCompare(System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol>,Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSignatureComparer/LazyTypeSubstitution&,System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol>,Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSignatureComparer/LazyTypeSubstitution&,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults DetailedParameterCompare(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol>,Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSignatureComparer/LazyTypeSubstitution&,System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol>,Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSignatureComparer/LazyTypeSubstitution&,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults)
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

		Public Shared Function DetailedReturnTypeCompare(ByVal returnsByRef1 As Boolean, ByVal type1 As TypeWithModifiers, ByVal refCustomModifiers1 As ImmutableArray(Of CustomModifier), ByVal typeSubstitution1 As TypeSubstitution, ByVal returnsByRef2 As Boolean, ByVal type2 As TypeWithModifiers, ByVal refCustomModifiers2 As ImmutableArray(Of CustomModifier), ByVal typeSubstitution2 As TypeSubstitution, ByVal comparisons As SymbolComparisonResults, Optional ByVal stopIfAny As SymbolComparisonResults = 0) As SymbolComparisonResults
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSignatureComparer::DetailedReturnTypeCompare(System.Boolean,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers,System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.CustomModifier>,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution,System.Boolean,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers,System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.CustomModifier>,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults DetailedReturnTypeCompare(System.Boolean,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers,System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.CustomModifier>,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution,System.Boolean,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers,System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.CustomModifier>,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults)
			' 
			' File d'attente vide.
			'    à System.ThrowHelper.ThrowInvalidOperationException(ExceptionResource resource)
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.(ICollection`1 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 525
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 445
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 363
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 307
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 86
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Function Equals(ByVal method1 As MethodSymbol, ByVal method2 As MethodSymbol) As Boolean Implements IEqualityComparer(Of MethodSymbol).Equals
			Dim flag As Boolean
			If (CObj(method1) = CObj(method2)) Then
				flag = True
			ElseIf (method1 Is Nothing OrElse method2 Is Nothing) Then
				flag = False
			ElseIf (method1.Arity <> method2.Arity) Then
				flag = False
			ElseIf (Not Me._considerName OrElse CaseInsensitiveComparison.Equals(method1.Name, method2.Name)) Then
				Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = MethodSignatureComparer.GetTypeSubstitution(method1)
				Dim typeSubstitution1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = MethodSignatureComparer.GetTypeSubstitution(method2)
				If (Me._considerReturnType AndAlso Not MethodSignatureComparer.HaveSameReturnTypes(method1, typeSubstitution, method2, typeSubstitution1, Me._considerCustomModifiers, Me._considerTupleNames)) Then
					flag = False
				ElseIf ((method1.ParameterCount > 0 OrElse method2.ParameterCount > 0) AndAlso Not MethodSignatureComparer.HaveSameParameterTypes(method1.Parameters, typeSubstitution, method2.Parameters, typeSubstitution1, Me._considerByRef, Me._considerCustomModifiers, Me._considerTupleNames)) Then
					flag = False
				Else
					If (Not Me._considerCallingConvention) Then
						If (method1.IsVararg = method2.IsVararg) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					Else
						If (method1.CallingConvention = method2.CallingConvention) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					End If
				Label1:
					flag = If(Not Me._considerTypeConstraints OrElse MethodSignatureComparer.HaveSameConstraints(method1, typeSubstitution, method2, typeSubstitution1), True, False)
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Function GetHashCode(ByVal method As MethodSymbol) As Integer Implements IEqualityComparer(Of MethodSymbol).GetHashCode
			Dim num As Integer = 1
			If (method IsNot Nothing) Then
				If (Me._considerName) Then
					num = Hash.Combine(Of String)(method.Name, num)
				End If
				If (Me._considerReturnType AndAlso Not method.IsGenericMethod AndAlso Not Me._considerCustomModifiers) Then
					num = Hash.Combine(Of TypeSymbol)(method.ReturnType, num)
				End If
				num = Hash.Combine(num, method.Arity)
				num = Hash.Combine(num, method.ParameterCount)
				num = Hash.Combine(method.IsVararg, num)
			End If
			Return num
		End Function

		Private Shared Function GetRefModifiers(ByVal typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution, ByVal param As ParameterSymbol) As ImmutableArray(Of CustomModifier)
			Dim customModifiers As ImmutableArray(Of CustomModifier)
			customModifiers = If(typeSubstitution Is Nothing, param.RefCustomModifiers, typeSubstitution.SubstituteCustomModifiers(param.OriginalDefinition.RefCustomModifiers))
			Return customModifiers
		End Function

		Private Shared Function GetTypeSubstitution(ByVal method As MethodSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim containingType As NamedTypeSymbol = method.ContainingType
			If (method.Arity <> 0) Then
				Dim typeSymbols As ImmutableArray(Of TypeSymbol) = StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(IndexedTypeParameterSymbol.Take(method.Arity))
				typeSubstitution = If(Not method.IsDefinition, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(containingType.TypeSubstitution, method.OriginalDefinition, typeSymbols, False), Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(method, method.TypeParameters, typeSymbols, False))
			ElseIf (containingType Is Nothing OrElse method.IsDefinition) Then
				typeSubstitution = Nothing
			Else
				typeSubstitution = containingType.TypeSubstitution
			End If
			Return typeSubstitution
		End Function

		Private Shared Function GetTypeWithModifiers(ByVal typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution, ByVal param As ParameterSymbol) As TypeWithModifiers
			Dim typeWithModifier As TypeWithModifiers
			typeWithModifier = If(typeSubstitution Is Nothing, New TypeWithModifiers(param.Type, param.CustomModifiers), MethodSignatureComparer.SubstituteType(typeSubstitution, New TypeWithModifiers(param.OriginalDefinition.Type, param.OriginalDefinition.CustomModifiers)))
			Return typeWithModifier
		End Function

		Friend Shared Function HaveSameConstraints(ByVal method1 As MethodSymbol, ByVal method2 As MethodSymbol) As Boolean
			Return MethodSignatureComparer.HaveSameConstraints(method1, MethodSignatureComparer.GetTypeSubstitution(method1), method2, MethodSignatureComparer.GetTypeSubstitution(method2))
		End Function

		Private Shared Function HaveSameConstraints(ByVal method1 As MethodSymbol, ByVal typeSubstitution1 As TypeSubstitution, ByVal method2 As MethodSymbol, ByVal typeSubstitution2 As TypeSubstitution) As Boolean
			Dim flag As Boolean
			Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = method1.OriginalDefinition.TypeParameters
			Dim typeParameterSymbols As ImmutableArray(Of TypeParameterSymbol) = method2.OriginalDefinition.TypeParameters
			Dim length As Integer = typeParameters.Length - 1
			Dim num As Integer = 0
			While True
				If (num > length) Then
					flag = True
					Exit While
				ElseIf (MethodSignatureComparer.HaveSameConstraints(typeParameters(num), typeSubstitution1, typeParameterSymbols(num), typeSubstitution2)) Then
					num = num + 1
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		Friend Shared Function HaveSameConstraints(ByVal typeParameter1 As TypeParameterSymbol, ByVal typeSubstitution1 As TypeSubstitution, ByVal typeParameter2 As TypeParameterSymbol, ByVal typeSubstitution2 As TypeSubstitution) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean
			If (typeParameter1.HasConstructorConstraint <> typeParameter2.HasConstructorConstraint OrElse typeParameter1.HasReferenceTypeConstraint <> typeParameter2.HasReferenceTypeConstraint OrElse typeParameter1.HasValueTypeConstraint <> typeParameter2.HasValueTypeConstraint OrElse typeParameter1.Variance <> typeParameter2.Variance) Then
				flag = False
			Else
				Dim constraintTypesNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol) = typeParameter1.ConstraintTypesNoUseSiteDiagnostics
				Dim typeSymbols As ImmutableArray(Of TypeSymbol) = typeParameter2.ConstraintTypesNoUseSiteDiagnostics
				If (constraintTypesNoUseSiteDiagnostics.Length <> 0 OrElse typeSymbols.Length <> 0) Then
					Dim instance As ArrayBuilder(Of TypeSymbol) = ArrayBuilder(Of TypeSymbol).GetInstance()
					Dim instance1 As ArrayBuilder(Of TypeSymbol) = ArrayBuilder(Of TypeSymbol).GetInstance()
					MethodSignatureComparer.SubstituteConstraintTypes(constraintTypesNoUseSiteDiagnostics, instance, typeSubstitution1)
					MethodSignatureComparer.SubstituteConstraintTypes(typeSymbols, instance1, typeSubstitution2)
					flag1 = If(Not MethodSignatureComparer.AreConstraintTypesSubset(instance, instance1), False, MethodSignatureComparer.AreConstraintTypesSubset(instance1, instance))
					instance.Free()
					instance1.Free()
					flag = flag1
				Else
					flag = True
				End If
			End If
			Return flag
		End Function

		Public Shared Function HaveSameParameterTypes(ByVal params1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol), ByVal typeSubstitution1 As TypeSubstitution, ByVal params2 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol), ByVal typeSubstitution2 As TypeSubstitution, ByVal considerByRef As Boolean, ByVal considerCustomModifiers As Boolean, ByVal considerTupleNames As Boolean) As Boolean
			Dim flag As Boolean
			Dim length As Integer = params1.Length
			If (length = params2.Length) Then
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				While num1 <= num
					Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = params1(num1)
					Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = params2(num1)
					If (Not MethodSignatureComparer.GetTypeWithModifiers(typeSubstitution1, item).IsSameType(MethodSignatureComparer.GetTypeWithModifiers(typeSubstitution2, parameterSymbol), MethodSignatureComparer.MakeTypeCompareKind(considerCustomModifiers, considerTupleNames))) Then
						flag = False
						Return flag
					ElseIf (considerCustomModifiers AndAlso Not System.Linq.ImmutableArrayExtensions.SequenceEqual(Of CustomModifier, CustomModifier)(MethodSignatureComparer.GetRefModifiers(typeSubstitution1, item), MethodSignatureComparer.GetRefModifiers(typeSubstitution2, parameterSymbol), DirectCast(Nothing, IEqualityComparer(Of CustomModifier)))) Then
						flag = False
						Return flag
					ElseIf (Not considerByRef OrElse item.IsByRef = parameterSymbol.IsByRef) Then
						num1 = num1 + 1
					Else
						flag = False
						Return flag
					End If
				End While
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function HaveSameReturnTypes(ByVal method1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal typeSubstitution1 As TypeSubstitution, ByVal method2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal typeSubstitution2 As TypeSubstitution, ByVal considerCustomModifiers As Boolean, ByVal considerTupleNames As Boolean) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean
			Dim isSub As Boolean = method1.IsSub
			If (isSub <> method2.IsSub) Then
				flag = False
			ElseIf (isSub) Then
				flag = True
			ElseIf (method1.ReturnsByRef = method2.ReturnsByRef) Then
				Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = method1.OriginalDefinition
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = method2.OriginalDefinition
				If (Not MethodSignatureComparer.SubstituteType(typeSubstitution1, New TypeWithModifiers(originalDefinition.ReturnType, originalDefinition.ReturnTypeCustomModifiers)).IsSameType(MethodSignatureComparer.SubstituteType(typeSubstitution2, New TypeWithModifiers(methodSymbol.ReturnType, methodSymbol.ReturnTypeCustomModifiers)), MethodSignatureComparer.MakeTypeCompareKind(considerCustomModifiers, considerTupleNames))) Then
					flag1 = False
				Else
					flag1 = If(Not considerCustomModifiers, True, System.Linq.ImmutableArrayExtensions.SequenceEqual(Of CustomModifier, CustomModifier)(MethodSignatureComparer.SubstituteModifiers(typeSubstitution1, originalDefinition.RefCustomModifiers), MethodSignatureComparer.SubstituteModifiers(typeSubstitution2, methodSymbol.RefCustomModifiers), DirectCast(Nothing, IEqualityComparer(Of CustomModifier))))
				End If
				flag = flag1
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Shared Function MakeTypeCompareKind(ByVal considerCustomModifiers As Boolean, ByVal considerTupleNames As Boolean) As Microsoft.CodeAnalysis.TypeCompareKind
			Dim typeCompareKind As Microsoft.CodeAnalysis.TypeCompareKind = Microsoft.CodeAnalysis.TypeCompareKind.ConsiderEverything
			If (Not considerCustomModifiers) Then
				typeCompareKind = typeCompareKind Or Microsoft.CodeAnalysis.TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds
			End If
			If (Not considerTupleNames) Then
				typeCompareKind = typeCompareKind Or Microsoft.CodeAnalysis.TypeCompareKind.IgnoreTupleNames
			End If
			Return typeCompareKind
		End Function

		Private Shared Function ParameterDefaultValueMismatch(ByVal param1 As ParameterSymbol, ByVal param2 As ParameterSymbol) As Boolean
			Dim flag As Boolean
			Dim explicitDefaultConstantValue As Microsoft.CodeAnalysis.ConstantValue = param1.ExplicitDefaultConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = param2.ExplicitDefaultConstantValue
			If (explicitDefaultConstantValue.IsBad OrElse constantValue.IsBad) Then
				flag = True
			Else
				If (explicitDefaultConstantValue.IsNothing) Then
					Dim discriminator As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValue.GetDiscriminator(param1.Type.GetEnumUnderlyingTypeOrSelf().SpecialType)
					If (discriminator <> Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.Bad) Then
						explicitDefaultConstantValue = Microsoft.CodeAnalysis.ConstantValue.[Default](discriminator)
					End If
				End If
				If (constantValue.IsNothing) Then
					Dim constantValueTypeDiscriminator As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValue.GetDiscriminator(param2.Type.GetEnumUnderlyingTypeOrSelf().SpecialType)
					If (constantValueTypeDiscriminator <> Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.Bad) Then
						constantValue = Microsoft.CodeAnalysis.ConstantValue.[Default](constantValueTypeDiscriminator)
					End If
				End If
				flag = Not explicitDefaultConstantValue.Equals(constantValue)
			End If
			Return flag
		End Function

		Private Shared Sub SubstituteConstraintTypes(ByVal constraintTypes As ImmutableArray(Of TypeSymbol), ByVal result As ArrayBuilder(Of TypeSymbol), ByVal substitution As TypeSubstitution)
			Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = constraintTypes.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As TypeSymbol = enumerator.Current
				result.Add(MethodSignatureComparer.SubstituteType(substitution, New TypeWithModifiers(current)).Type)
			End While
		End Sub

		Private Shared Function SubstituteModifiers(ByVal typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution, ByVal customModifiers As ImmutableArray(Of CustomModifier)) As ImmutableArray(Of CustomModifier)
			Dim customModifiers1 As ImmutableArray(Of CustomModifier)
			customModifiers1 = If(typeSubstitution Is Nothing, customModifiers, typeSubstitution.SubstituteCustomModifiers(customModifiers))
			Return customModifiers1
		End Function

		Private Shared Function SubstituteType(ByVal typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution, ByVal typeSymbol As TypeWithModifiers) As TypeWithModifiers
			Return typeSymbol.InternalSubstituteTypeParameters(typeSubstitution)
		End Function

		Public Structure LazyTypeSubstitution
			Private _typeSubstitution As TypeSubstitution

			Private _method As MethodSymbol

			Public ReadOnly Property Value As TypeSubstitution
				Get
					If (Me._typeSubstitution Is Nothing AndAlso Me._method IsNot Nothing) Then
						Me._typeSubstitution = MethodSignatureComparer.GetTypeSubstitution(Me._method)
						Me._method = Nothing
					End If
					Return Me._typeSubstitution
				End Get
			End Property

			Public Sub New(ByVal method As MethodSymbol)
				Me = New MethodSignatureComparer.LazyTypeSubstitution() With
				{
					._method = method
				}
			End Sub
		End Structure
	End Class
End Namespace