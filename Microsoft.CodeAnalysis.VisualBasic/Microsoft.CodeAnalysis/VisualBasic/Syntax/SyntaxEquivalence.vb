Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Friend Module SyntaxEquivalence
		Friend Function AreEquivalent(ByVal before As SyntaxTree, ByVal after As SyntaxTree, ByVal ignoreChildNode As Func(Of SyntaxKind, Boolean), ByVal topLevel As Boolean) As Boolean
			Dim flag As Boolean
			If (before = after) Then
				flag = True
			ElseIf (before Is Nothing OrElse after Is Nothing) Then
				flag = False
			Else
				Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
				Dim root As SyntaxNode = before.GetRoot(cancellationToken)
				cancellationToken = New System.Threading.CancellationToken()
				flag = SyntaxEquivalence.AreEquivalent(root, after.GetRoot(cancellationToken), ignoreChildNode, topLevel)
			End If
			Return flag
		End Function

		Public Function AreEquivalent(ByVal before As SyntaxNode, ByVal after As SyntaxNode, ByVal ignoreChildNode As Func(Of SyntaxKind, Boolean), ByVal topLevel As Boolean) As Boolean
			Dim flag As Boolean
			flag = If(before Is Nothing OrElse after Is Nothing, before = after, SyntaxEquivalence.AreEquivalentRecursive(before.Green, after.Green, SyntaxKind.None, ignoreChildNode, topLevel))
			Return flag
		End Function

		Public Function AreEquivalent(ByVal before As SyntaxTokenList, ByVal after As SyntaxTokenList) As Boolean
			Return SyntaxEquivalence.AreEquivalentRecursive(before.Node, after.Node, SyntaxKind.None, Nothing, False)
		End Function

		Public Function AreEquivalent(ByVal before As Microsoft.CodeAnalysis.SyntaxToken, ByVal after As Microsoft.CodeAnalysis.SyntaxToken) As Boolean
			If (before.RawKind <> after.RawKind) Then
				Return False
			End If
			If (before.Node Is Nothing) Then
				Return True
			End If
			Return SyntaxEquivalence.AreTokensEquivalent(before.Node, after.Node)
		End Function

		Private Function AreEquivalentRecursive(ByVal before As Microsoft.CodeAnalysis.GreenNode, ByVal after As Microsoft.CodeAnalysis.GreenNode, ByVal parentKind As SyntaxKind, ByVal ignoreChildNode As Func(Of SyntaxKind, Boolean), ByVal topLevel As Boolean) As Boolean
			Dim flag As Boolean
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode1 As Microsoft.CodeAnalysis.GreenNode
			Dim func As Func(Of SyntaxKind, Boolean)
			If (before = after) Then
				flag = True
			ElseIf (before Is Nothing OrElse after Is Nothing) Then
				flag = False
			ElseIf (before.RawKind <> after.RawKind) Then
				flag = False
			ElseIf (Not before.IsToken) Then
				Dim rawKind As SyntaxKind = CUShort(before.RawKind)
				If (SyntaxEquivalence.AreModifiersEquivalent(before, after, rawKind)) Then
					If (topLevel) Then
						If (CUShort(rawKind) - CUShort(SyntaxKind.SubBlock) <= CUShort(SyntaxKind.EndSelectStatement)) Then
							flag = SyntaxEquivalence.AreEquivalentRecursive(DirectCast(before, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockBaseSyntax).Begin, DirectCast(after, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockBaseSyntax).Begin, rawKind, Nothing, True)
							Return flag
						ElseIf (rawKind = SyntaxKind.FieldDeclaration) Then
							Dim fieldDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax = DirectCast(before, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax)
							Dim fieldDeclarationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax = DirectCast(after, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax)
							If (Not fieldDeclarationSyntax.Modifiers.Any(441) AndAlso Not fieldDeclarationSyntax1.Modifiers.Any(441)) Then
								If (SyntaxEquivalence._Closure$__.$I5-0 Is Nothing) Then
									func = Function(childKind As SyntaxKind)
										If (childKind = SyntaxKind.EqualsValue) Then
											Return True
										End If
										Return childKind = SyntaxKind.AsNewClause
									End Function
									SyntaxEquivalence._Closure$__.$I5-0 = func
								Else
									func = SyntaxEquivalence._Closure$__.$I5-0
								End If
								ignoreChildNode = func
							End If
						ElseIf (rawKind = SyntaxKind.EqualsValue) Then
							If (parentKind <> SyntaxKind.PropertyStatement) Then
								GoTo Label1
							End If
							flag = True
							Return flag
						End If
					End If
				Label1:
					If (ignoreChildNode Is Nothing) Then
						Dim slotCount As Integer = before.SlotCount
						If (slotCount = after.SlotCount) Then
							Dim num As Integer = slotCount - 1
							Dim num1 As Integer = 0
							While num1 <= num
								If (SyntaxEquivalence.AreEquivalentRecursive(before.GetSlot(num1), after.GetSlot(num1), rawKind, ignoreChildNode, topLevel)) Then
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
					Else
						Dim enumerator As Microsoft.CodeAnalysis.Syntax.InternalSyntax.ChildSyntaxList.Enumerator = DirectCast(before, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode).ChildNodesAndTokens().GetEnumerator()
						Dim enumerator1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.ChildSyntaxList.Enumerator = DirectCast(after, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode).ChildNodesAndTokens().GetEnumerator()
						Do
							greenNode = Nothing
							greenNode1 = Nothing
							While True
								If (enumerator.MoveNext()) Then
									Dim current As Microsoft.CodeAnalysis.GreenNode = enumerator.Current
									If (current IsNot Nothing AndAlso (current.IsToken OrElse Not ignoreChildNode(CUShort(current.RawKind)))) Then
										greenNode = current
										Exit While
									End If
								Else
									Exit While
								End If
							End While
							While enumerator1.MoveNext()
								Dim current1 As Microsoft.CodeAnalysis.GreenNode = enumerator1.Current
								If (current1 Is Nothing OrElse Not current1.IsToken AndAlso ignoreChildNode(CUShort(current1.RawKind))) Then
									Continue While
								End If
								greenNode1 = current1
								Exit While
							End While
							If (greenNode IsNot Nothing AndAlso greenNode1 IsNot Nothing) Then
								Continue Do
							End If
							flag = greenNode = greenNode1
							Return flag
						Loop While SyntaxEquivalence.AreEquivalentRecursive(greenNode, greenNode1, rawKind, ignoreChildNode, topLevel)
						flag = False
					End If
				Else
					flag = False
				End If
			Else
				flag = SyntaxEquivalence.AreTokensEquivalent(before, after)
			End If
			Return flag
		End Function

		Private Function AreModifiersEquivalent(ByVal before As GreenNode, ByVal after As GreenNode, ByVal kind As SyntaxKind) As Boolean
			Dim flag As Boolean
			If (CUShort(kind) - CUShort(SyntaxKind.SubBlock) <= CUShort(SyntaxKind.List)) Then
				Dim modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax) = DirectCast(before, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockBaseSyntax).Begin.Modifiers
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax) = DirectCast(after, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockBaseSyntax).Begin.Modifiers
				If (modifiers.Count = syntaxList.Count) Then
					Dim count As Integer = modifiers.Count - 1
					Dim num As Integer = 0
					While num <= count
						If (modifiers.Any(CInt(syntaxList(num).Kind))) Then
							num = num + 1
						Else
							flag = False
							Return flag
						End If
					End While
				Else
					flag = False
					Return flag
				End If
			End If
			flag = True
			Return flag
		End Function

		Private Function AreTokensEquivalent(ByVal before As GreenNode, ByVal after As GreenNode) As Boolean
			Dim flag As Boolean
			If (before.IsMissing = after.IsMissing) Then
				Dim rawKind As SyntaxKind = CUShort(before.RawKind)
				flag = If(CUShort(rawKind) - CUShort(SyntaxKind.IdentifierToken) <= CUShort(SyntaxKind.EndUsingStatement) OrElse rawKind = SyntaxKind.InterpolatedStringTextToken, [String].Equals(DirectCast(before, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken).Text, DirectCast(after, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken).Text, StringComparison.Ordinal), True)
			Else
				flag = False
			End If
			Return flag
		End Function
	End Module
End Namespace