Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class DocumentationCommentTypeParamRefBinder
		Inherits DocumentationCommentTypeParamBinder
		Public Sub New(ByVal containingBinder As Binder, ByVal commentedSymbol As Symbol)
			MyBase.New(containingBinder, commentedSymbol)
		End Sub

		Friend Overrides Function BindXmlNameAttributeValue(ByVal identifier As IdentifierNameSyntax, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of Symbol)
			Dim empty As ImmutableArray(Of Symbol)
			Dim symbols As ImmutableArray(Of Symbol) = MyBase.BindXmlNameAttributeValue(identifier, useSiteInfo)
			If (symbols.IsEmpty) Then
				Dim instance As LookupResult = LookupResult.GetInstance()
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = identifier.Identifier
				Me.Lookup(instance, syntaxToken.ValueText, 0, LookupOptions.MustNotBeReturnValueVariable Or LookupOptions.IgnoreExtensionMethods Or LookupOptions.UseBaseReferenceAccessibility Or LookupOptions.MustNotBeLocalOrParameter, useSiteInfo)
				If (instance.HasSingleSymbol) Then
					Dim singleSymbol As Symbol = instance.SingleSymbol
					instance.Free()
					If (singleSymbol.Kind <> SymbolKind.TypeParameter) Then
						empty = ImmutableArray(Of Symbol).Empty
					Else
						empty = ImmutableArray.Create(Of Symbol)(singleSymbol)
					End If
				Else
					instance.Free()
					empty = New ImmutableArray(Of Symbol)()
				End If
			Else
				empty = symbols
			End If
			Return empty
		End Function
	End Class
End Namespace