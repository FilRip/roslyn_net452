Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class DocumentationCommentBinder
		Inherits Binder
		Protected ReadOnly CommentedSymbol As Symbol

		Protected Sub New(ByVal containingBinder As Binder, ByVal commentedSymbol As Symbol)
			MyBase.New(containingBinder)
			Me.CommentedSymbol = commentedSymbol
		End Sub

		Friend Overrides Function BinderSpecificLookupOptions(ByVal options As LookupOptions) As LookupOptions
			Return MyBase.ContainingBinder.BinderSpecificLookupOptions(options) Or LookupOptions.UseBaseReferenceAccessibility
		End Function

		Friend Overrides Function BindInsideCrefAttributeValue(ByVal name As TypeSyntax, ByVal preserveAliases As Boolean, ByVal diagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of Symbol)
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function BindInsideCrefAttributeValue(ByVal reference As CrefReferenceSyntax, ByVal preserveAliases As Boolean, ByVal diagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of Symbol)
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function BindXmlNameAttributeValue(ByVal identifier As IdentifierNameSyntax, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of Symbol)
			Throw ExceptionUtilities.Unreachable
		End Function

		<Conditional("DEBUG")>
		Private Shared Sub CheckBinderSymbolRelationship(ByVal containingBinder As Binder, ByVal commentedSymbol As Symbol)
			If (commentedSymbol IsNot Nothing) Then
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(commentedSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				Dim containingMember As Symbol = containingBinder.ContainingMember
				If (namedTypeSymbol Is Nothing OrElse namedTypeSymbol.TypeKind = TypeKind.[Delegate]) Then
					Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = commentedSymbol.ContainingType
				End If
			End If
		End Sub

		Protected Shared Function FindSymbolInSymbolArray(Of TSymbol As Symbol)(ByVal name As String, ByVal symbols As ImmutableArray(Of TSymbol)) As ImmutableArray(Of Symbol)
			Dim empty As ImmutableArray(Of Symbol)
			If (Not symbols.IsEmpty) Then
				Dim enumerator As ImmutableArray(Of TSymbol).Enumerator = symbols.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As TSymbol = enumerator.Current
					If (Not CaseInsensitiveComparison.Equals(name, current.Name)) Then
						Continue While
					End If
					empty = ImmutableArray.Create(Of Symbol)(DirectCast(current, Symbol))
					Return empty
				End While
			End If
			empty = ImmutableArray(Of Symbol).Empty
			Return empty
		End Function

		Friend Shared Function GetBinderTypeForNameAttribute(ByVal node As BaseXmlAttributeSyntax) As DocumentationCommentBinder.BinderType
			Return DocumentationCommentBinder.GetBinderTypeForNameAttribute(DocumentationCommentBinder.GetParentXmlElementName(node))
		End Function

		Friend Shared Function GetBinderTypeForNameAttribute(ByVal parentNodeName As String) As DocumentationCommentBinder.BinderType
			Dim binderType As DocumentationCommentBinder.BinderType
			If (parentNodeName IsNot Nothing) Then
				If (DocumentationCommentXmlNames.ElementEquals(parentNodeName, "param", True) OrElse DocumentationCommentXmlNames.ElementEquals(parentNodeName, "paramref", True)) Then
					binderType = DocumentationCommentBinder.BinderType.NameInParamOrParamRef
					Return binderType
				ElseIf (Not DocumentationCommentXmlNames.ElementEquals(parentNodeName, "typeparam", True)) Then
					If (Not DocumentationCommentXmlNames.ElementEquals(parentNodeName, "typeparamref", True)) Then
						binderType = DocumentationCommentBinder.BinderType.None
						Return binderType
					End If
					binderType = DocumentationCommentBinder.BinderType.NameInTypeParamRef
					Return binderType
				Else
					binderType = DocumentationCommentBinder.BinderType.NameInTypeParam
					Return binderType
				End If
			End If
			binderType = DocumentationCommentBinder.BinderType.None
			Return binderType
		End Function

		Friend Shared Function GetParentXmlElementName(ByVal attr As BaseXmlAttributeSyntax) As String
			Dim valueText As String
			Dim localName As SyntaxToken
			Dim parent As VisualBasicSyntaxNode = attr.Parent
			If (parent IsNot Nothing) Then
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementStartTag) Then
					Dim xmlElementStartTagSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax = DirectCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax)
					If (xmlElementStartTagSyntax.Name.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlName) Then
						localName = DirectCast(xmlElementStartTagSyntax.Name, XmlNameSyntax).LocalName
						valueText = localName.ValueText
					Else
						valueText = Nothing
					End If
				ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlEmptyElement) Then
					valueText = Nothing
				Else
					Dim xmlEmptyElementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmptyElementSyntax = DirectCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmptyElementSyntax)
					If (xmlEmptyElementSyntax.Name.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlName) Then
						localName = DirectCast(xmlEmptyElementSyntax.Name, XmlNameSyntax).LocalName
						valueText = localName.ValueText
					Else
						valueText = Nothing
					End If
				End If
			Else
				valueText = Nothing
			End If
			Return valueText
		End Function

		Public Shared Function IsIntrinsicTypeForDocumentationComment(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleKeyword) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByteKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharKeyword OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BooleanKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByteKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LongKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword AndAlso CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UIntegerKeyword) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) Then
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Protected Shared Sub RemoveOverriddenMethodsAndProperties(ByVal symbols As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))
			If (symbols IsNot Nothing AndAlso symbols.Count >= 2) Then
				Dim symbols1 As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Integer) = Nothing
				Dim count As Integer = symbols.Count - 1
				Dim num As Integer = 0
				Do
					Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbol = symbols(num)
					Dim kind As Microsoft.CodeAnalysis.SymbolKind = item.Kind
					If (kind = Microsoft.CodeAnalysis.SymbolKind.Method OrElse kind = Microsoft.CodeAnalysis.SymbolKind.[Property]) Then
						If (symbols1 Is Nothing) Then
							symbols1 = New Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Integer)()
						End If
						symbols1.Add(item.OriginalDefinition, num)
					End If
					num = num + 1
				Loop While num <= count
				If (symbols1 IsNot Nothing) Then
					Dim instance As ArrayBuilder(Of Integer) = Nothing
					Dim count1 As Integer = symbols.Count - 1
					Dim num1 As Integer = 0
					Do
						Dim num2 As Integer = -1
						Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = symbols(num1)
						Dim symbolKind As Microsoft.CodeAnalysis.SymbolKind = symbol.Kind
						If (symbolKind = Microsoft.CodeAnalysis.SymbolKind.Method) Then
							Dim originalDefinition As MethodSymbol = DirectCast(symbol.OriginalDefinition, MethodSymbol)
							While True
								originalDefinition = originalDefinition.OverriddenMethod
								If (originalDefinition Is Nothing) Then
									Exit While
								End If
								If (symbols1.TryGetValue(originalDefinition, num2)) Then
									If (instance Is Nothing) Then
										instance = ArrayBuilder(Of Integer).GetInstance()
									End If
									instance.Add(num2)
								End If
							End While
						ElseIf (symbolKind = Microsoft.CodeAnalysis.SymbolKind.[Property]) Then
							Dim overriddenProperty As PropertySymbol = DirectCast(symbol.OriginalDefinition, PropertySymbol)
							While True
								overriddenProperty = overriddenProperty.OverriddenProperty
								If (overriddenProperty Is Nothing) Then
									Exit While
								End If
								If (symbols1.TryGetValue(overriddenProperty, num2)) Then
									If (instance Is Nothing) Then
										instance = ArrayBuilder(Of Integer).GetInstance()
									End If
									instance.Add(num2)
								End If
							End While
						End If
						num1 = num1 + 1
					Loop While num1 <= count1
					If (instance IsNot Nothing) Then
						Dim count2 As Integer = instance.Count - 1
						Dim num3 As Integer = 0
						Do
							symbols(instance(num3)) = Nothing
							num3 = num3 + 1
						Loop While num3 <= count2
						Dim num4 As Integer = 0
						Dim count3 As Integer = symbols.Count - 1
						Dim num5 As Integer = 0
						Do
							Dim item1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = symbols(num5)
							If (item1 IsNot Nothing) Then
								symbols(num4) = item1
								num4 = num4 + 1
							End If
							num5 = num5 + 1
						Loop While num5 <= count3
						symbols.Clip(num4)
						instance.Free()
					End If
				End If
			End If
		End Sub

		Friend Enum BinderType
			None
			Cref
			NameInTypeParamRef
			NameInTypeParam
			NameInParamOrParamRef
		End Enum
	End Class
End Namespace