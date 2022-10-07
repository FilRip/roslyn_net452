Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices
Imports System.Text

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Module SymbolDisplay
		Friend Sub AddSymbolDisplayParts(ByVal parts As ArrayBuilder(Of SymbolDisplayPart), ByVal str As String)
			Dim enumerator As IEnumerator(Of Integer) = Nothing
			Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
			Dim builder As StringBuilder = instance.Builder
			Using num As Integer = -1
				enumerator = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.TokenizeString(str, ObjectDisplayOptions.UseHexadecimalNumbers Or ObjectDisplayOptions.UseQuotes Or ObjectDisplayOptions.EscapeNonPrintableCharacters).GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Integer = enumerator.Current
					Dim num1 As Integer = current >> 16
					If (num >= 0 AndAlso num <> num1) Then
						parts.Add(New SymbolDisplayPart(DirectCast(num, SymbolDisplayPartKind), Nothing, builder.ToString()))
						builder.Clear()
					End If
					num = num1
					builder.Append(Strings.ChrW(current And 65535))
				End While
			End Using
			If (num >= 0) Then
				parts.Add(New SymbolDisplayPart(DirectCast(num, SymbolDisplayPartKind), Nothing, builder.ToString()))
			End If
			instance.Free()
		End Sub

		Friend Sub AddSymbolDisplayParts(ByVal parts As ArrayBuilder(Of SymbolDisplayPart), ByVal c As Char)
			Dim wellKnownCharacterName As String = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.GetWellKnownCharacterName(c)
			If (wellKnownCharacterName IsNot Nothing) Then
				parts.Add(New SymbolDisplayPart(SymbolDisplayPartKind.FieldName, Nothing, wellKnownCharacterName))
				Return
			End If
			If (Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.IsPrintable(c)) Then
				parts.Add(New SymbolDisplayPart(SymbolDisplayPartKind.StringLiteral, Nothing, [String].Concat("""", Microsoft.VisualBasic.CompilerServices.Conversions.ToString(c), """c")))
				Return
			End If
			Dim num As Integer = c
			parts.Add(New SymbolDisplayPart(SymbolDisplayPartKind.MethodName, Nothing, "ChrW"))
			parts.Add(New SymbolDisplayPart(SymbolDisplayPartKind.Punctuation, Nothing, "("))
			parts.Add(New SymbolDisplayPart(SymbolDisplayPartKind.NumericLiteral, Nothing, [String].Concat("&H", num.ToString("X"))))
			parts.Add(New SymbolDisplayPart(SymbolDisplayPartKind.Punctuation, Nothing, ")"))
		End Sub

		Public Function FormatPrimitive(ByVal obj As Object, ByVal quoteStrings As Boolean, ByVal useHexadecimalNumbers As Boolean) As String
			Dim objectDisplayOption As ObjectDisplayOptions = ObjectDisplayOptions.None
			If (quoteStrings) Then
				objectDisplayOption = objectDisplayOption Or ObjectDisplayOptions.UseQuotes Or ObjectDisplayOptions.EscapeNonPrintableCharacters
			End If
			If (useHexadecimalNumbers) Then
				objectDisplayOption = objectDisplayOption Or ObjectDisplayOptions.UseHexadecimalNumbers
			End If
			Return Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatPrimitive(RuntimeHelpers.GetObjectValue(obj), objectDisplayOption)
		End Function

		Public Function ToDisplayParts(ByVal symbol As ISymbol, Optional ByVal format As SymbolDisplayFormat = Nothing) As ImmutableArray(Of SymbolDisplayPart)
			format = If(format, SymbolDisplayFormat.VisualBasicErrorMessageFormat)
			Return Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.ToDisplayParts(symbol, Nothing, -1, format, False)
		End Function

		Private Function ToDisplayParts(ByVal symbol As ISymbol, ByVal semanticModelOpt As SemanticModel, ByVal positionOpt As Integer, ByVal format As SymbolDisplayFormat, ByVal minimal As Boolean) As ImmutableArray(Of SymbolDisplayPart)
			If (symbol Is Nothing) Then
				Throw New ArgumentNullException("symbol")
			End If
			If (minimal) Then
				If (semanticModelOpt Is Nothing) Then
					Throw New ArgumentException(VBResources.SemanticModelMustBeProvided)
				End If
				If (positionOpt < 0 OrElse positionOpt > semanticModelOpt.SyntaxTree.Length) Then
					Throw New ArgumentOutOfRangeException(VBResources.PositionNotWithinTree)
				End If
			End If
			Dim instance As ArrayBuilder(Of SymbolDisplayPart) = ArrayBuilder(Of SymbolDisplayPart).GetInstance()
			symbol.Accept(New SymbolDisplayVisitor(instance, format, semanticModelOpt, positionOpt))
			Return instance.ToImmutableAndFree()
		End Function

		Public Function ToDisplayString(ByVal symbol As ISymbol, Optional ByVal format As SymbolDisplayFormat = Nothing) As String
			Return Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.ToDisplayParts(symbol, format).ToDisplayString()
		End Function

		Public Function ToMinimalDisplayParts(ByVal symbol As ISymbol, ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, Optional ByVal format As SymbolDisplayFormat = Nothing) As ImmutableArray(Of SymbolDisplayPart)
			format = If(format, SymbolDisplayFormat.MinimallyQualifiedFormat)
			Return Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.ToDisplayParts(symbol, semanticModel, position, format, True)
		End Function

		Public Function ToMinimalDisplayString(ByVal symbol As ISymbol, ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, Optional ByVal format As SymbolDisplayFormat = Nothing) As String
			Return Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.ToMinimalDisplayParts(symbol, semanticModel, position, format).ToDisplayString()
		End Function
	End Module
End Namespace