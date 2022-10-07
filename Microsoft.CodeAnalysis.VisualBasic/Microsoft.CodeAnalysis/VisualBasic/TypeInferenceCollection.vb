Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
Imports System
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class TypeInferenceCollection
		Inherits TypeInferenceCollection(Of DominantTypeData)
		Public Sub New()
			MyBase.New()
		End Sub

		Public Sub AddType(ByVal type As TypeSymbol, ByVal conversion As RequiredConversion, ByVal sourceExpression As BoundExpression)
			If (Not type.IsVoidType() AndAlso Not type.IsErrorType()) Then
				Dim flag As Boolean = False
				If (Not TypeOf type Is ArrayLiteralTypeSymbol) Then
					Dim enumerator As ArrayBuilder(Of DominantTypeData).Enumerator = MyBase.GetTypeDataList().GetEnumerator()
					While enumerator.MoveNext()
						Dim current As DominantTypeData = enumerator.Current
						If (TypeOf current.ResultType Is ArrayLiteralTypeSymbol OrElse Not type.IsSameTypeIgnoringAll(current.ResultType)) Then
							Continue While
						End If
						current.ResultType = TypeInferenceCollection.MergeTupleNames(type, current.ResultType)
						current.InferenceRestrictions = Conversions.CombineConversionRequirements(current.InferenceRestrictions, conversion)
						flag = True
					End While
				End If
				If (Not flag) Then
					Dim dominantTypeDatum As DominantTypeData = New DominantTypeData() With
					{
						.ResultType = type,
						.InferenceRestrictions = conversion
					}
					MyBase.GetTypeDataList().Add(dominantTypeDatum)
				End If
			End If
		End Sub

		Friend Shared Function MergeTupleNames(ByVal first As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal second As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim strs As ImmutableArray(Of String)
			Dim func As Func(Of String, String, String)
			Dim func1 As Func(Of String, Boolean)
			If (first.IsSameType(second, TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds) OrElse Not first.ContainsTupleNames()) Then
				typeSymbol = first
			ElseIf (second.ContainsTupleNames()) Then
				Dim strs1 As ImmutableArray(Of String) = VisualBasicCompilation.TupleNamesEncoder.Encode(first)
				Dim strs2 As ImmutableArray(Of String) = VisualBasicCompilation.TupleNamesEncoder.Encode(second)
				If (strs1.IsDefault OrElse strs2.IsDefault) Then
					strs = New ImmutableArray(Of String)()
				Else
					Dim strs3 As ImmutableArray(Of String) = strs1
					Dim strs4 As ImmutableArray(Of String) = strs2
					If (TypeInferenceCollection._Closure$__.$I2-0 Is Nothing) Then
						func = Function(n1 As String, n2 As String)
							If (Not CaseInsensitiveComparison.Equals(n1, n2)) Then
								Return Nothing
							End If
							Return n1
						End Function
						TypeInferenceCollection._Closure$__.$I2-0 = func
					Else
						func = TypeInferenceCollection._Closure$__.$I2-0
					End If
					strs = Microsoft.CodeAnalysis.ImmutableArrayExtensions.ZipAsArray(Of String, String, String)(strs3, strs4, func)
					Dim strs5 As ImmutableArray(Of String) = strs
					If (TypeInferenceCollection._Closure$__.$I2-1 Is Nothing) Then
						func1 = Function(n As String) n Is Nothing
						TypeInferenceCollection._Closure$__.$I2-1 = func1
					Else
						func1 = TypeInferenceCollection._Closure$__.$I2-1
					End If
					If (strs5.All(func1)) Then
						strs = New ImmutableArray(Of String)()
					End If
				End If
				typeSymbol = TupleTypeDecoder.DecodeTupleTypesIfApplicable(first, strs)
			Else
				typeSymbol = second
			End If
			Return typeSymbol
		End Function
	End Class
End Namespace