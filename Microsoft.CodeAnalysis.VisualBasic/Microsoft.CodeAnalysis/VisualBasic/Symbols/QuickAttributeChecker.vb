Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class QuickAttributeChecker
		Private ReadOnly _nameToAttributeMap As Dictionary(Of String, QuickAttributes)

		Private _sealed As Boolean

		Public Sub New()
			MyBase.New()
			Me._nameToAttributeMap = New Dictionary(Of String, QuickAttributes)(CaseInsensitiveComparison.Comparer)
		End Sub

		Public Sub New(ByVal other As QuickAttributeChecker)
			MyBase.New()
			Me._nameToAttributeMap = New Dictionary(Of String, QuickAttributes)(other._nameToAttributeMap, CaseInsensitiveComparison.Comparer)
		End Sub

		Public Sub AddAlias(ByVal aliasSyntax As SimpleImportsClauseSyntax)
			Dim finalName As String = Me.GetFinalName(aliasSyntax.Name)
			If (finalName IsNot Nothing) Then
				Dim quickAttribute As QuickAttributes = QuickAttributes.None
				If (Me._nameToAttributeMap.TryGetValue(finalName, quickAttribute)) Then
					Me.AddName(aliasSyntax.[Alias].Identifier.ValueText, quickAttribute)
				End If
			End If
		End Sub

		Public Sub AddName(ByVal name As String, ByVal newAttributes As QuickAttributes)
			Dim quickAttribute As QuickAttributes = QuickAttributes.None
			Me._nameToAttributeMap.TryGetValue(name, quickAttribute)
			Me._nameToAttributeMap(name) = newAttributes Or quickAttribute
			If (name.EndsWith("Attribute", StringComparison.OrdinalIgnoreCase)) Then
				Me._nameToAttributeMap(name.Substring(0, name.Length - "Attribute".Length)) = newAttributes Or quickAttribute
			End If
		End Sub

		Public Function CheckAttribute(ByVal attr As AttributeSyntax) As QuickAttributes
			Dim quickAttribute As QuickAttributes
			Dim quickAttribute1 As QuickAttributes
			Dim finalName As String = Me.GetFinalName(attr.Name)
			quickAttribute = If(finalName Is Nothing OrElse Not Me._nameToAttributeMap.TryGetValue(finalName, quickAttribute1), QuickAttributes.None, quickAttribute1)
			Return quickAttribute
		End Function

		Public Function CheckAttributes(ByVal attributeLists As SyntaxList(Of AttributeListSyntax)) As QuickAttributes
			Dim quickAttribute As QuickAttributes = QuickAttributes.None
			If (attributeLists.Count > 0) Then
				Dim enumerator As SyntaxList(Of AttributeListSyntax).Enumerator = attributeLists.GetEnumerator()
				While enumerator.MoveNext()
					Dim enumerator1 As SeparatedSyntaxList(Of AttributeSyntax).Enumerator = enumerator.Current.Attributes.GetEnumerator()
					While enumerator1.MoveNext()
						quickAttribute = quickAttribute Or Me.CheckAttribute(enumerator1.Current)
					End While
				End While
			End If
			Return quickAttribute
		End Function

		Private Function GetFinalName(ByVal typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As String
			Dim valueText As String
			Dim right As VisualBasicSyntaxNode = typeSyntax
			While True
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = right.Kind()
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierName) Then
					valueText = DirectCast(right, IdentifierNameSyntax).Identifier.ValueText
					Exit While
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedName) Then
					right = DirectCast(right, QualifiedNameSyntax).Right
				Else
					valueText = Nothing
					Exit While
				End If
			End While
			Return valueText
		End Function

		Public Sub Seal()
			Me._sealed = True
		End Sub
	End Class
End Namespace