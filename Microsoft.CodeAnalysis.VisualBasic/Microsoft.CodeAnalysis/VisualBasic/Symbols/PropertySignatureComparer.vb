Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class PropertySignatureComparer
		Implements IEqualityComparer(Of PropertySymbol)
		Public ReadOnly Shared RuntimePropertySignatureComparer As PropertySignatureComparer

		Public ReadOnly Shared RetargetedExplicitPropertyImplementationComparer As PropertySignatureComparer

		Public ReadOnly Shared WinRTConflictComparer As PropertySignatureComparer

		Private ReadOnly _considerName As Boolean

		Private ReadOnly _considerType As Boolean

		Private ReadOnly _considerReadWriteModifiers As Boolean

		Private ReadOnly _considerOptionalParameters As Boolean

		Private ReadOnly _considerCustomModifiers As Boolean

		Private ReadOnly _considerTupleNames As Boolean

		Shared Sub New()
			PropertySignatureComparer.RuntimePropertySignatureComparer = New PropertySignatureComparer(True, True, False, True, True, False)
			PropertySignatureComparer.RetargetedExplicitPropertyImplementationComparer = New PropertySignatureComparer(True, True, True, True, True, False)
			PropertySignatureComparer.WinRTConflictComparer = New PropertySignatureComparer(True, False, False, False, False, False)
		End Sub

		Private Sub New(ByVal considerName As Boolean, ByVal considerType As Boolean, ByVal considerReadWriteModifiers As Boolean, ByVal considerOptionalParameters As Boolean, ByVal considerCustomModifiers As Boolean, ByVal considerTupleNames As Boolean)
			MyBase.New()
			Me._considerName = considerName
			Me._considerType = considerType
			Me._considerReadWriteModifiers = considerReadWriteModifiers
			Me._considerOptionalParameters = considerOptionalParameters
			Me._considerCustomModifiers = considerCustomModifiers
			Me._considerTupleNames = considerTupleNames
		End Sub

		Public Shared Function DetailedCompare(ByVal prop1 As PropertySymbol, ByVal prop2 As PropertySymbol, ByVal comparisons As SymbolComparisonResults, Optional ByVal stopIfAny As SymbolComparisonResults = 0) As SymbolComparisonResults
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySignatureComparer::DetailedCompare(Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults DetailedCompare(Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults,Microsoft.CodeAnalysis.VisualBasic.Symbols.SymbolComparisonResults)
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

		Public Function Equals(ByVal prop1 As PropertySymbol, ByVal prop2 As PropertySymbol) As Boolean Implements IEqualityComparer(Of PropertySymbol).Equals
			Dim flag As Boolean
			If (prop1 = prop2) Then
				flag = True
			ElseIf (prop1 Is Nothing OrElse prop2 Is Nothing) Then
				flag = False
			ElseIf (Me._considerName AndAlso Not CaseInsensitiveComparison.Equals(prop1.Name, prop2.Name)) Then
				flag = False
			ElseIf (Me._considerReadWriteModifiers AndAlso (prop1.IsReadOnly <> prop2.IsReadOnly OrElse prop1.IsWriteOnly <> prop2.IsWriteOnly)) Then
				flag = False
			ElseIf (Not Me._considerType OrElse PropertySignatureComparer.HaveSameTypes(prop1, prop2, MethodSignatureComparer.MakeTypeCompareKind(Me._considerCustomModifiers, Me._considerTupleNames))) Then
				flag = If((prop1.ParameterCount > 0 OrElse prop2.ParameterCount > 0) AndAlso Not MethodSignatureComparer.HaveSameParameterTypes(prop1.Parameters, Nothing, prop2.Parameters, Nothing, False, Me._considerCustomModifiers, Me._considerTupleNames), False, True)
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Function GetHashCode(ByVal prop As PropertySymbol) As Integer Implements IEqualityComparer(Of PropertySymbol).GetHashCode
			Dim num As Integer = 1
			If (prop IsNot Nothing) Then
				If (Me._considerName) Then
					num = Hash.Combine(Of String)(prop.Name, num)
				End If
				If (Me._considerType AndAlso Not Me._considerCustomModifiers) Then
					num = Hash.Combine(Of TypeSymbol)(prop.Type, num)
				End If
				num = Hash.Combine(num, prop.ParameterCount)
			End If
			Return num
		End Function

		Private Shared Function HaveSameTypes(ByVal prop1 As PropertySymbol, ByVal prop2 As PropertySymbol, ByVal comparison As TypeCompareKind) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySignatureComparer::HaveSameTypes(Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol,Microsoft.CodeAnalysis.TypeCompareKind)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean HaveSameTypes(Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol,Microsoft.CodeAnalysis.TypeCompareKind)
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
	End Class
End Namespace