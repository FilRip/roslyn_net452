Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class LexicalOrderSymbolComparer
		Implements IComparer(Of Symbol)
		Public ReadOnly Shared Instance As LexicalOrderSymbolComparer

		Shared Sub New()
			LexicalOrderSymbolComparer.Instance = New LexicalOrderSymbolComparer()
		End Sub

		Private Sub New()
			MyBase.New()
		End Sub

		Public Function Compare(ByVal x As Symbol, ByVal y As Symbol) As Integer Implements IComparer(Of Symbol).Compare
			Dim num As Integer
			If (CObj(x) <> CObj(y)) Then
				Dim lexicalSortKey As Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey = x.GetLexicalSortKey()
				Dim lexicalSortKey1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey = y.GetLexicalSortKey()
				Dim sortOrder As Integer = Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey.Compare(lexicalSortKey, lexicalSortKey1)
				If (sortOrder = 0) Then
					sortOrder = DirectCast(x, ISymbol).Kind.ToSortOrder() - DirectCast(y, ISymbol).Kind.ToSortOrder()
					If (sortOrder = 0) Then
						sortOrder = CaseInsensitiveComparison.Compare(x.Name, y.Name)
						num = sortOrder
					Else
						num = sortOrder
					End If
				Else
					num = sortOrder
				End If
			Else
				num = 0
			End If
			Return num
		End Function
	End Class
End Namespace