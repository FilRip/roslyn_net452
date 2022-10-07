Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class DocumentationCommentCrefBinder
		Inherits DocumentationCommentBinder
		Private _typeParameterBinder As DocumentationCommentCrefBinder.TypeParametersBinder

		Public Sub New(ByVal containingBinder As Binder, ByVal commentedSymbol As Symbol)
			MyBase.New(containingBinder, commentedSymbol)
		End Sub

		Friend Overrides Function BindInsideCrefAttributeValue(ByVal reference As CrefReferenceSyntax, ByVal preserveAliases As Boolean, ByVal diagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of Symbol)
			Dim empty As ImmutableArray(Of Symbol)
			If (DocumentationCommentCrefBinder.HasTrailingSkippedTokensAndShouldReportError(reference)) Then
				empty = ImmutableArray(Of Symbol).Empty
			ElseIf (reference.Signature Is Nothing) Then
				empty = Me.BindNameInsideCrefReferenceInLegacyMode(reference.Name, preserveAliases, useSiteInfo)
			ElseIf (Not DocumentationCommentCrefBinder.NameSyntaxHasComplexGenericArguments(reference.Name)) Then
				Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
				Dim strs As Dictionary(Of String, CrefTypeParameterSymbol) = New Dictionary(Of String, CrefTypeParameterSymbol)(CaseInsensitiveComparison.Comparer)
				Dim name As TypeSyntax = reference.Name
				Dim argumentTypes As SeparatedSyntaxList(Of CrefSignaturePartSyntax) = reference.Signature.ArgumentTypes
				Me.CollectCrefNameSymbolsStrict(name, argumentTypes.Count, strs, instance, preserveAliases, useSiteInfo)
				If (instance.Count <> 0) Then
					DocumentationCommentBinder.RemoveOverriddenMethodsAndProperties(instance)
					Dim signatureElements As ArrayBuilder(Of DocumentationCommentCrefBinder.SignatureElement) = Nothing
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
					Me.BindSignatureAndReturnValue(reference, strs, signatureElements, typeSymbol, diagnosticBag)
					Dim num As Integer = If(signatureElements Is Nothing, 0, signatureElements.Count)
					Dim num1 As Integer = 0
					Dim num2 As Integer = 0
					While num1 < instance.Count
						Dim item As Symbol = instance(num1)
						Dim kind As SymbolKind = item.Kind
						If (kind = SymbolKind.Method) Then
							Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							If (methodSymbol.ParameterCount = num) Then
								Dim parameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = methodSymbol.Parameters
								Dim num3 As Integer = num - 1
								Dim num4 As Integer = 0
								Do
									Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = parameters(num4)
									If (parameterSymbol.IsByRef <> signatureElements(num4).IsByRef OrElse Not parameterSymbol.Type.IsSameTypeIgnoringAll(signatureElements(num4).Type)) Then
										GoTo Label0
									End If
									num4 = num4 + 1
								Loop While num4 <= num3
								If (typeSymbol Is Nothing OrElse Not methodSymbol.IsSub AndAlso methodSymbol.ReturnType.IsSameTypeIgnoringAll(typeSymbol)) Then
									instance(num2) = item
									num2 = num2 + 1
									num1 = num1 + 1
									Continue While
								End If
							End If
						ElseIf (kind = SymbolKind.[Property]) Then
							Dim parameterSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = DirectCast(item, PropertySymbol).Parameters
							If (parameterSymbols.Length = num) Then
								Dim num5 As Integer = num - 1
								Dim num6 As Integer = 0
								Do
									Dim item1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = parameterSymbols(num6)
									If (item1.IsByRef <> signatureElements(num6).IsByRef OrElse Not item1.Type.IsSameTypeIgnoringAll(signatureElements(num6).Type)) Then
										GoTo Label0
									End If
									num6 = num6 + 1
								Loop While num6 <= num5
								instance(num2) = item
								num2 = num2 + 1
								num1 = num1 + 1
								Continue While
							End If
						End If
					Label0:
						num1 = num1 + 1
					End While
					If (signatureElements IsNot Nothing) Then
						signatureElements.Free()
					End If
					If (num2 < num1) Then
						instance.Clip(num2)
					End If
					empty = instance.ToImmutableAndFree()
				Else
					instance.Free()
					empty = ImmutableArray(Of Symbol).Empty
				End If
			Else
				empty = ImmutableArray(Of Symbol).Empty
			End If
			Return empty
		End Function

		Friend Overrides Function BindInsideCrefAttributeValue(ByVal name As TypeSyntax, ByVal preserveAliases As Boolean, ByVal diagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of Symbol)
			Dim empty As ImmutableArray(Of Symbol)
			Dim flag As Boolean = False
			Dim enclosingCrefReference As CrefReferenceSyntax = DocumentationCommentCrefBinder.GetEnclosingCrefReference(name, flag)
			If (enclosingCrefReference Is Nothing) Then
				empty = ImmutableArray(Of Symbol).Empty
			ElseIf (enclosingCrefReference.Signature Is Nothing) Then
				empty = Me.BindNameInsideCrefReferenceInLegacyMode(name, preserveAliases, useSiteInfo)
			ElseIf (Not flag) Then
				Dim argumentTypes As SeparatedSyntaxList(Of CrefSignaturePartSyntax) = enclosingCrefReference.Signature.ArgumentTypes
				empty = Me.BindInsideCrefReferenceName(name, argumentTypes.Count, preserveAliases, useSiteInfo)
			Else
				empty = Me.BindInsideCrefSignatureOrReturnType(enclosingCrefReference, name, preserveAliases, diagnosticBag)
			End If
			Return empty
		End Function

		Private Function BindInsideCrefReferenceName(ByVal name As TypeSyntax, ByVal argCount As Integer, ByVal preserveAliases As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of Symbol)
			Dim immutableAndFree As ImmutableArray(Of Symbol)
			Dim arguments As SeparatedSyntaxList(Of TypeSyntax)
			Dim parent As VisualBasicSyntaxNode = name.Parent
			If (parent Is Nothing OrElse parent.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeArgumentList) Then
				Dim flag As Boolean = False
				Dim valueText As String = Nothing
				Dim count As Integer = -1
				While True
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = name.Kind()
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierName
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GenericName
							If (parent Is Nothing OrElse parent.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedName) Then
								GoTo Label2
							End If
							Dim qualifiedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax = DirectCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax)
							If (qualifiedNameSyntax.Right <> name) Then
								GoTo Label2
							End If
							name = qualifiedNameSyntax
							parent = name.Parent
							Continue While
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedName
							GoTo Label0
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GlobalName
							immutableAndFree = ImmutableArray.Create(Of Symbol)(MyBase.Compilation.GlobalNamespace)
							Return immutableAndFree
						Case Else
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CrefOperatorReference) Then
								If (parent Is Nothing OrElse parent.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedCrefOperatorReference) Then
									GoTo Label0
								End If
								name = DirectCast(parent, QualifiedCrefOperatorReferenceSyntax)
								parent = name.Parent
								Continue While
							Else
								If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedCrefOperatorReference) Then
									GoTo Label0
								End If
								Throw ExceptionUtilities.UnexpectedValue(name.Kind())
							End If
					End Select
				End While
			Label2:
				flag = True
				If (name.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierName) Then
					Dim genericNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax = DirectCast(name, Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax)
					valueText = genericNameSyntax.Identifier.ValueText
					arguments = genericNameSyntax.TypeArgumentList.Arguments
					count = arguments.Count
				Else
					valueText = DirectCast(name, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax).Identifier.ValueText
					count = 0
				End If
			Label0:
				Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
				Me.CollectCrefNameSymbolsStrict(name, argCount, New Dictionary(Of String, CrefTypeParameterSymbol)(CaseInsensitiveComparison.Comparer), instance, preserveAliases, useSiteInfo)
				DocumentationCommentBinder.RemoveOverriddenMethodsAndProperties(instance)
				If (instance.Count = 1 AndAlso flag) Then
					Dim item As Symbol = instance(0)
					Dim type As TypeSymbol = Nothing
					Dim kind As SymbolKind = item.Kind
					If (kind = SymbolKind.Field) Then
						type = DirectCast(item, FieldSymbol).Type
					ElseIf (kind = SymbolKind.Method) Then
						type = DirectCast(item, MethodSymbol).ReturnType
					ElseIf (kind = SymbolKind.[Property]) Then
						type = DirectCast(item, PropertySymbol).Type
					End If
					Dim flag1 As Boolean = False
					If (type IsNot Nothing AndAlso CaseInsensitiveComparison.Equals(type.Name, valueText)) Then
						Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
						flag1 = If(namedTypeSymbol Is Nothing, count = 0, namedTypeSymbol.Arity = count)
					End If
					If (flag1) Then
						instance(0) = type
					End If
				End If
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				arguments = DirectCast(parent, TypeArgumentListSyntax).Arguments
				Dim num As Integer = arguments.IndexOf(name)
				If (name.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierName) Then
					immutableAndFree = ImmutableArray.Create(Of Symbol)(New CrefTypeParameterSymbol(num, "?", name))
				Else
					Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = DirectCast(name, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
					Dim identifier As SyntaxToken = identifierNameSyntax.Identifier
					immutableAndFree = ImmutableArray.Create(Of Symbol)(New CrefTypeParameterSymbol(num, identifier.ValueText, identifierNameSyntax))
				End If
			End If
			Return immutableAndFree
		End Function

		Private Function BindInsideCrefSignatureOrReturnType(ByVal crefReference As CrefReferenceSyntax, ByVal name As TypeSyntax, ByVal preserveAliases As Boolean, ByVal diagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of Symbol)
			Dim orCreateTypeParametersAwareBinder As Binder = Me.GetOrCreateTypeParametersAwareBinder(crefReference)
			Dim target As Symbol = orCreateTypeParametersAwareBinder.BindNamespaceOrTypeOrAliasSyntax(name, If(diagnosticBag, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded), False)
			target = orCreateTypeParametersAwareBinder.BindNamespaceOrTypeOrAliasSyntax(name, If(diagnosticBag, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded), False)
			If (target IsNot Nothing AndAlso target.Kind = SymbolKind.[Alias] AndAlso Not preserveAliases) Then
				target = DirectCast(target, AliasSymbol).Target
			End If
			If (target Is Nothing) Then
				Return ImmutableArray(Of Symbol).Empty
			End If
			Return ImmutableArray.Create(Of Symbol)(target)
		End Function

		Private Function BindNameInsideCrefReferenceInLegacyMode(ByVal nameFromCref As TypeSyntax, ByVal preserveAliases As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
			If (DocumentationCommentCrefBinder.CrefReferenceIsLegalForLegacyMode(nameFromCref)) Then
				Dim parent As VisualBasicSyntaxNode = nameFromCref.Parent
				While parent IsNot Nothing And parent.Kind() <> SyntaxKind.CrefReference
					If (parent.Kind() = SyntaxKind.QualifiedName) Then
						parent = parent.Parent
					Else
						If (preserveAliases) Then
							symbol = MyBase.BindTypeOrAliasSyntax(nameFromCref, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, False)
						Else
							symbol = MyBase.BindTypeSyntax(nameFromCref, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, False, False, False)
						End If
						immutableAndFree = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)(symbol)
						Return immutableAndFree
					End If
				End While
				If (Not nameFromCref.ContainsDiagnostics) Then
					Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).GetInstance()
					Select Case nameFromCref.Kind()
						Case SyntaxKind.PredefinedType
							Me.BindPredefinedTypeForCref(DirectCast(nameFromCref, PredefinedTypeSyntax), instance)
							Exit Select
						Case SyntaxKind.IdentifierName
						Case SyntaxKind.GenericName
							Me.BindSimpleNameForCref(DirectCast(nameFromCref, SimpleNameSyntax), instance, preserveAliases, useSiteInfo, False)
							Exit Select
						Case SyntaxKind.QualifiedName
							Me.BindQualifiedNameForCref(DirectCast(nameFromCref, QualifiedNameSyntax), instance, preserveAliases, useSiteInfo)
							Exit Select
						Case Else
							Throw ExceptionUtilities.UnexpectedValue(nameFromCref.Kind())
					End Select
					DocumentationCommentBinder.RemoveOverriddenMethodsAndProperties(instance)
					immutableAndFree = instance.ToImmutableAndFree()
				Else
					immutableAndFree = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Empty
				End If
			Else
				immutableAndFree = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Empty
			End If
			Return immutableAndFree
		End Function

		Private Sub BindPredefinedTypeForCref(ByVal node As PredefinedTypeSyntax, ByVal symbols As ArrayBuilder(Of Symbol))
			Dim specialType As Microsoft.CodeAnalysis.SpecialType
			If (Not node.ContainsDiagnostics) Then
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Keyword.Kind()
				If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerKeyword) Then
					If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharKeyword) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BooleanKeyword) Then
							specialType = Microsoft.CodeAnalysis.SpecialType.System_Boolean
						ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByteKeyword) Then
							specialType = Microsoft.CodeAnalysis.SpecialType.System_Byte
						Else
							If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharKeyword) Then
								Throw ExceptionUtilities.UnexpectedValue(node.Keyword.Kind())
							End If
							specialType = Microsoft.CodeAnalysis.SpecialType.System_Char
						End If
					ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DecimalKeyword) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateKeyword) Then
							specialType = Microsoft.CodeAnalysis.SpecialType.System_DateTime
						Else
							If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DecimalKeyword) Then
								Throw ExceptionUtilities.UnexpectedValue(node.Keyword.Kind())
							End If
							specialType = Microsoft.CodeAnalysis.SpecialType.System_Decimal
						End If
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleKeyword) Then
						specialType = Microsoft.CodeAnalysis.SpecialType.System_Double
					Else
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerKeyword) Then
							Throw ExceptionUtilities.UnexpectedValue(node.Keyword.Kind())
						End If
						specialType = Microsoft.CodeAnalysis.SpecialType.System_Int32
					End If
				ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LongKeyword) Then
						specialType = Microsoft.CodeAnalysis.SpecialType.System_Int64
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword) Then
						specialType = Microsoft.CodeAnalysis.SpecialType.System_Object
					Else
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword) Then
							Throw ExceptionUtilities.UnexpectedValue(node.Keyword.Kind())
						End If
						specialType = Microsoft.CodeAnalysis.SpecialType.System_SByte
					End If
				ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword) Then
						specialType = Microsoft.CodeAnalysis.SpecialType.System_Int16
					Else
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword) Then
							Throw ExceptionUtilities.UnexpectedValue(node.Keyword.Kind())
						End If
						specialType = Microsoft.CodeAnalysis.SpecialType.System_Single
					End If
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword) Then
					specialType = Microsoft.CodeAnalysis.SpecialType.System_String
				Else
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UIntegerKeyword
							specialType = Microsoft.CodeAnalysis.SpecialType.System_UInt32
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ULongKeyword
							specialType = Microsoft.CodeAnalysis.SpecialType.System_UInt64
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UShortKeyword
							specialType = Microsoft.CodeAnalysis.SpecialType.System_UInt16
							Exit Select
						Case Else
							Throw ExceptionUtilities.UnexpectedValue(node.Keyword.Kind())
					End Select
				End If
				symbols.Add(MyBase.GetSpecialType(specialType, node, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded))
			End If
		End Sub

		Private Sub BindQualifiedNameForCref(ByVal node As QualifiedNameSyntax, ByVal symbols As ArrayBuilder(Of Symbol), ByVal preserveAliases As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim flag As Boolean = True
			Dim left As NameSyntax = node.Left
			Select Case left.Kind()
				Case SyntaxKind.IdentifierName
				Case SyntaxKind.GenericName
					Me.BindSimpleNameForCref(DirectCast(left, SimpleNameSyntax), symbols, preserveAliases, useSiteInfo, True)
					Exit Select
				Case SyntaxKind.QualifiedName
					Me.BindQualifiedNameForCref(DirectCast(left, QualifiedNameSyntax), symbols, preserveAliases, useSiteInfo)
					flag = False
					Exit Select
				Case SyntaxKind.GlobalName
					symbols.Add(MyBase.Compilation.GlobalNamespace)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(left.Kind())
			End Select
			If (symbols.Count <> 1) Then
				symbols.Clear()
				Return
			End If
			Dim item As Symbol = symbols(0)
			symbols.Clear()
			Dim right As SimpleNameSyntax = node.Right
			If (right.Kind() <> SyntaxKind.GenericName) Then
				Dim valueText As String = DirectCast(right, IdentifierNameSyntax).Identifier.ValueText
				Me.BindSimpleNameForCref(valueText, 0, symbols, preserveAliases, useSiteInfo, item, flag, False)
				If (symbols.Count <= 0) Then
					Me.BindSimpleNameForCref(valueText, -1, symbols, preserveAliases, useSiteInfo, item, flag, False)
				End If
			Else
				Dim genericNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax = DirectCast(right, Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax)
				Dim str As String = genericNameSyntax.Identifier.ValueText
				Dim arguments As SeparatedSyntaxList(Of TypeSyntax) = genericNameSyntax.TypeArgumentList.Arguments
				Me.BindSimpleNameForCref(str, arguments.Count, symbols, preserveAliases, useSiteInfo, item, flag, False)
				If (symbols.Count = 1) Then
					symbols(0) = Me.ConstructGenericSymbolWithTypeArgumentsForCref(symbols(0), genericNameSyntax)
					Return
				End If
			End If
		End Sub

		Private Sub BindSignatureAndReturnValue(ByVal reference As CrefReferenceSyntax, ByVal typeParameters As Dictionary(Of String, CrefTypeParameterSymbol), <Out> ByRef signatureTypes As ArrayBuilder(Of DocumentationCommentCrefBinder.SignatureElement), <Out> ByRef returnType As TypeSymbol, ByVal diagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			signatureTypes = Nothing
			returnType = Nothing
			Dim orCreateTypeParametersAwareBinder As Binder = Me.GetOrCreateTypeParametersAwareBinder(typeParameters)
			Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = If(diagnosticBag, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded)
			Dim signature As CrefSignatureSyntax = reference.Signature
			If (signature.ArgumentTypes.Count > 0) Then
				signatureTypes = ArrayBuilder(Of DocumentationCommentCrefBinder.SignatureElement).GetInstance()
				Dim enumerator As SeparatedSyntaxList(Of CrefSignaturePartSyntax).Enumerator = signature.ArgumentTypes.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As CrefSignaturePartSyntax = enumerator.Current
					signatureTypes.Add(New DocumentationCommentCrefBinder.SignatureElement(orCreateTypeParametersAwareBinder.BindTypeSyntax(current.Type, bindingDiagnosticBag, False, False, False), current.Modifier.Kind() = SyntaxKind.ByRefKeyword))
				End While
			End If
			If (reference.AsClause IsNot Nothing) Then
				returnType = orCreateTypeParametersAwareBinder.BindTypeSyntax(reference.AsClause.Type, bindingDiagnosticBag, False, False, False)
			End If
		End Sub

		Private Sub BindSimpleNameForCref(ByVal name As String, ByVal arity As Integer, ByVal symbols As ArrayBuilder(Of Symbol), ByVal preserveAliases As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal containingSymbol As Symbol = Nothing, Optional ByVal allowColorColor As Boolean = False, Optional ByVal typeOrNamespaceOnly As Boolean = False)
			If (Not [String].IsNullOrEmpty(name)) Then
				Dim lookupOption As LookupOptions = LookupOptions.MustNotBeReturnValueVariable Or LookupOptions.IgnoreAccessibility Or LookupOptions.IgnoreExtensionMethods Or LookupOptions.UseBaseReferenceAccessibility Or LookupOptions.MustNotBeLocalOrParameter Or LookupOptions.NoSystemObjectLookupForInterfaces
				If (arity < 0) Then
					lookupOption = lookupOption Or LookupOptions.AllMethodsOfAnyArity
				End If
				If (typeOrNamespaceOnly) Then
					lookupOption = lookupOption Or LookupOptions.NamespacesOrTypesOnly
				End If
				Dim instance As LookupResult = LookupResult.GetInstance()
				If (containingSymbol IsNot Nothing) Then
					Me.LookupSimpleNameInContainingSymbol(containingSymbol, allowColorColor, name, arity, preserveAliases, instance, lookupOption, useSiteInfo)
				Else
					Me.Lookup(instance, name, arity, lookupOption, useSiteInfo)
				End If
				If (Not instance.IsGoodOrAmbiguous OrElse Not instance.HasSymbol) Then
					instance.Free()
					Return
				End If
				DocumentationCommentCrefBinder.CreateGoodOrAmbiguousFromLookupResultAndFree(instance, symbols, preserveAliases)
			End If
		End Sub

		Private Sub BindSimpleNameForCref(ByVal node As SimpleNameSyntax, ByVal symbols As ArrayBuilder(Of Symbol), ByVal preserveAliases As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), ByVal typeOrNamespaceOnly As Boolean)
			If (Not node.ContainsDiagnostics) Then
				If (node.Kind() <> SyntaxKind.GenericName) Then
					Dim valueText As String = DirectCast(node, IdentifierNameSyntax).Identifier.ValueText
					Me.BindSimpleNameForCref(valueText, 0, symbols, preserveAliases, useSiteInfo, Nothing, False, typeOrNamespaceOnly)
					If (symbols.Count <= 0) Then
						Me.BindSimpleNameForCref(valueText, -1, symbols, preserveAliases, useSiteInfo, Nothing, False, typeOrNamespaceOnly)
					End If
				Else
					Dim genericNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax)
					Dim str As String = genericNameSyntax.Identifier.ValueText
					Dim arguments As SeparatedSyntaxList(Of TypeSyntax) = genericNameSyntax.TypeArgumentList.Arguments
					Me.BindSimpleNameForCref(str, arguments.Count, symbols, preserveAliases, useSiteInfo, Nothing, False, typeOrNamespaceOnly)
					If (symbols.Count = 1) Then
						symbols(0) = Me.ConstructGenericSymbolWithTypeArgumentsForCref(symbols(0), genericNameSyntax)
						Return
					End If
				End If
			End If
		End Sub

		Private Function BingTypeArgumentsForCref(ByVal args As SeparatedSyntaxList(Of TypeSyntax)) As ImmutableArray(Of TypeSymbol)
			Dim typeSymbolArray(args.Count - 1 + 1 - 1) As TypeSymbol
			Dim count As Integer = args.Count - 1
			Dim num As Integer = 0
			Do
				typeSymbolArray(num) = MyBase.BindTypeSyntax(args(num), Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, False, False, False)
				num = num + 1
			Loop While num <= count
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(typeSymbolArray)
		End Function

		Private Sub CollectConstructorsSymbolsStrict(ByVal symbols As ArrayBuilder(Of Symbol))
			Dim containingMember As Symbol = Me.ContainingMember
			If (containingMember IsNot Nothing) Then
				If (containingMember.Kind <> SymbolKind.NamedType) Then
					containingMember = containingMember.ContainingType
				End If
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(containingMember, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (namedTypeSymbol IsNot Nothing) Then
					symbols.AddRange(Of MethodSymbol)(namedTypeSymbol.InstanceConstructors)
				End If
			End If
		End Sub

		Private Shared Sub CollectConstructorsSymbolsStrict(ByVal containingSymbol As Symbol, ByVal symbols As ArrayBuilder(Of Symbol))
			If (containingSymbol.Kind = SymbolKind.NamedType) Then
				symbols.AddRange(Of MethodSymbol)(DirectCast(containingSymbol, NamedTypeSymbol).InstanceConstructors)
			End If
		End Sub

		Private Sub CollectCrefNameSymbolsStrict(ByVal nameFromCref As TypeSyntax, ByVal argsCount As Integer, ByVal typeParameters As Dictionary(Of String, CrefTypeParameterSymbol), ByVal symbols As ArrayBuilder(Of Symbol), ByVal preserveAlias As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = nameFromCref.Kind()
			If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedName) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CrefOperatorReference) Then
					Me.CollectTopLevelOperatorReferenceStrict(DirectCast(nameFromCref, CrefOperatorReferenceSyntax), argsCount, symbols, useSiteInfo)
					Return
				End If
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedCrefOperatorReference) Then
					Me.CollectQualifiedOperatorReferenceSymbolsStrict(DirectCast(nameFromCref, QualifiedCrefOperatorReferenceSyntax), argsCount, typeParameters, symbols, useSiteInfo)
					Return
				End If
			Else
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierName) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					Me.CollectSimpleNameSymbolsStrict(DirectCast(nameFromCref, SimpleNameSyntax), typeParameters, symbols, preserveAlias, useSiteInfo, False)
					Return
				End If
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedName) Then
					Me.CollectQualifiedNameSymbolsStrict(DirectCast(nameFromCref, QualifiedNameSyntax), typeParameters, symbols, preserveAlias, useSiteInfo)
					Return
				End If
			End If
			Throw ExceptionUtilities.UnexpectedValue(nameFromCref.Kind())
		End Sub

		Private Shared Sub CollectGoodOrAmbiguousFromLookupResult(ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal symbols As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByVal preserveAlias As Boolean)
			Dim diagnostic As DiagnosticInfo = lookupResult.Diagnostic
			If (Not TypeOf diagnostic Is AmbiguousSymbolDiagnostic) Then
				Dim enumerator As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = lookupResult.Symbols.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator.Current
					symbols.Add(If(preserveAlias, current, current.UnwrapAlias()))
				End While
				Return
			End If
			Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = DirectCast(diagnostic, AmbiguousSymbolDiagnostic).AmbiguousSymbols.GetEnumerator()
			While enumerator1.MoveNext()
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator1.Current
				symbols.Add(If(preserveAlias, symbol, symbol.UnwrapAlias()))
			End While
		End Sub

		Private Shared Sub CollectOperatorsAndConversionsInType(ByVal crefOperator As CrefOperatorReferenceSyntax, ByVal argCount As Integer, ByVal type As TypeSymbol, ByVal symbols As ArrayBuilder(Of Symbol), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim operatorInfo As OverloadResolution.OperatorInfo
			If (type IsNot Nothing AndAlso argCount <= 2 AndAlso argCount >= 1) Then
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = crefOperator.OperatorToken.Kind()
				If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword) Then
					If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeKeyword) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword) Then
							If (argCount = 2) Then
								Dim operatorInfo1 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.[And])
								DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_BitwiseAnd", operatorInfo1, useSiteInfo, "op_LogicalAnd", operatorInfo1)
								Return
							Else
								Return
							End If
						ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword) Then
							If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeKeyword) Then
								Throw ExceptionUtilities.UnexpectedValue(crefOperator.OperatorToken.Kind())
							End If
							If (argCount = 2) Then
								Dim operatorInfo2 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.[Like])
								operatorInfo = New OverloadResolution.OperatorInfo()
								DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Like", operatorInfo2, useSiteInfo, Nothing, operatorInfo)
								Return
							Else
								Return
							End If
						ElseIf (argCount = 1) Then
							Dim operatorInfo3 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.RightShift)
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.Conversion, "op_Implicit", New OverloadResolution.OperatorInfo(UnaryOperatorKind.Implicit), useSiteInfo, "op_Explicit", New OverloadResolution.OperatorInfo(UnaryOperatorKind.Explicit))
							Return
						Else
							Return
						End If
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModKeyword) Then
						If (argCount = 2) Then
							Dim operatorInfo4 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.Modulo)
							operatorInfo = New OverloadResolution.OperatorInfo()
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Modulus", operatorInfo4, useSiteInfo, Nothing, operatorInfo)
							Return
						Else
							Return
						End If
					ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword) Then
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword) Then
							Throw ExceptionUtilities.UnexpectedValue(crefOperator.OperatorToken.Kind())
						End If
						If (argCount = 2) Then
							Dim operatorInfo5 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.[Or])
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_BitwiseOr", operatorInfo5, useSiteInfo, "op_LogicalOr", operatorInfo5)
							Return
						Else
							Return
						End If
					ElseIf (argCount = 1) Then
						Dim operatorInfo6 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(UnaryOperatorKind.[Not])
						DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_OnesComplement", operatorInfo6, useSiteInfo, "op_LogicalNot", operatorInfo6)
						Return
					Else
						Return
					End If
				ElseIf (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword) Then
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
							If (argCount <> 2) Then
								Return
							End If
							Dim operatorInfo7 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.Concatenate)
							operatorInfo = New OverloadResolution.OperatorInfo()
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Concatenate", operatorInfo7, useSiteInfo, Nothing, operatorInfo)
							Return
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterMultipleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeywordEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClauseItem Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableDeclarator Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectMemberInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferredFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionalKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.REMKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThrowKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ToKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UIntegerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ULongKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GosubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WendKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AllKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DescendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalChecksumKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OffKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PreserveKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StrictKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TextKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WarningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AwaitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.YieldKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AtToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseParenToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SemicolonToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GoToStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskToken
							If (argCount <> 2) Then
								Return
							End If
							Dim operatorInfo8 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.Multiply)
							operatorInfo = New OverloadResolution.OperatorInfo()
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Multiply", operatorInfo8, useSiteInfo, Nothing, operatorInfo)
							Return
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken
							If (argCount = 1) Then
								Dim operatorInfo9 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(UnaryOperatorKind.Plus)
								operatorInfo = New OverloadResolution.OperatorInfo()
								DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_UnaryPlus", operatorInfo9, useSiteInfo, Nothing, operatorInfo)
								Return
							End If
							Dim operatorInfo10 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.Add)
							operatorInfo = New OverloadResolution.OperatorInfo()
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Addition", operatorInfo10, useSiteInfo, Nothing, operatorInfo)
							Return
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken
							If (argCount = 1) Then
								Dim operatorInfo11 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(UnaryOperatorKind.Minus)
								operatorInfo = New OverloadResolution.OperatorInfo()
								DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_UnaryNegation", operatorInfo11, useSiteInfo, Nothing, operatorInfo)
								Return
							End If
							Dim operatorInfo12 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.Subtract)
							operatorInfo = New OverloadResolution.OperatorInfo()
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Subtraction", operatorInfo12, useSiteInfo, Nothing, operatorInfo)
							Return
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashToken
							If (argCount <> 2) Then
								Return
							End If
							Dim operatorInfo13 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.Divide)
							operatorInfo = New OverloadResolution.OperatorInfo()
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Division", operatorInfo13, useSiteInfo, Nothing, operatorInfo)
							Return
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken
							If (argCount <> 2) Then
								Return
							End If
							Dim operatorInfo14 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.LessThan)
							operatorInfo = New OverloadResolution.OperatorInfo()
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_LessThan", operatorInfo14, useSiteInfo, Nothing, operatorInfo)
							Return
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanEqualsToken
							If (argCount <> 2) Then
								Return
							End If
							Dim operatorInfo15 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.LessThanOrEqual)
							operatorInfo = New OverloadResolution.OperatorInfo()
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_LessThanOrEqual", operatorInfo15, useSiteInfo, Nothing, operatorInfo)
							Return
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanGreaterThanToken
							If (argCount <> 2) Then
								Return
							End If
							Dim operatorInfo16 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.NotEquals)
							operatorInfo = New OverloadResolution.OperatorInfo()
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Inequality", operatorInfo16, useSiteInfo, Nothing, operatorInfo)
							Return
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
							If (argCount <> 2) Then
								Return
							End If
							Dim operatorInfo17 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.Equals)
							operatorInfo = New OverloadResolution.OperatorInfo()
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Equality", operatorInfo17, useSiteInfo, Nothing, operatorInfo)
							Return
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
							If (argCount <> 2) Then
								Return
							End If
							Dim operatorInfo18 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.GreaterThan)
							operatorInfo = New OverloadResolution.OperatorInfo()
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_GreaterThan", operatorInfo18, useSiteInfo, Nothing, operatorInfo)
							Return
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanEqualsToken
							If (argCount <> 2) Then
								Return
							End If
							Dim operatorInfo19 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.GreaterThanOrEqual)
							operatorInfo = New OverloadResolution.OperatorInfo()
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_GreaterThanOrEqual", operatorInfo19, useSiteInfo, Nothing, operatorInfo)
							Return
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BackslashToken
							If (argCount <> 2) Then
								Return
							End If
							Dim operatorInfo20 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.IntegerDivide)
							operatorInfo = New OverloadResolution.OperatorInfo()
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_IntegerDivision", operatorInfo20, useSiteInfo, Nothing, operatorInfo)
							Return
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaretToken
							If (argCount <> 2) Then
								Return
							End If
							Dim operatorInfo21 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.Power)
							operatorInfo = New OverloadResolution.OperatorInfo()
							DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_Exponent", operatorInfo21, useSiteInfo, Nothing, operatorInfo)
							Return
						Case Else
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanToken) Then
								If (argCount <> 2) Then
									Return
								End If
								Dim operatorInfo22 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.LeftShift)
								DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_LeftShift", operatorInfo22, useSiteInfo, "op_UnsignedLeftShift", operatorInfo22)
								Return
							ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanToken) Then
								If (argCount <> 2) Then
									Return
								End If
								Dim operatorInfo23 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.RightShift)
								DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_RightShift", operatorInfo23, useSiteInfo, "op_UnsignedRightShift", operatorInfo23)
								Return
							Else
								Exit Select
							End If
					End Select
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword) Then
					If (argCount = 2) Then
						Dim operatorInfo24 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(BinaryOperatorKind.[Xor])
						operatorInfo = New OverloadResolution.OperatorInfo()
						DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_ExclusiveOr", operatorInfo24, useSiteInfo, Nothing, operatorInfo)
						Return
					Else
						Return
					End If
				ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword) Then
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword) Then
						Throw ExceptionUtilities.UnexpectedValue(crefOperator.OperatorToken.Kind())
					End If
					If (argCount = 1) Then
						Dim operatorInfo25 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(UnaryOperatorKind.IsTrue)
						operatorInfo = New OverloadResolution.OperatorInfo()
						DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_True", operatorInfo25, useSiteInfo, Nothing, operatorInfo)
						Return
					Else
						Return
					End If
				ElseIf (argCount = 1) Then
					Dim operatorInfo26 As OverloadResolution.OperatorInfo = New OverloadResolution.OperatorInfo(UnaryOperatorKind.IsFalse)
					operatorInfo = New OverloadResolution.OperatorInfo()
					DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(type, symbols, MethodKind.UserDefinedOperator, "op_False", operatorInfo26, useSiteInfo, Nothing, operatorInfo)
					Return
				Else
					Return
				End If
				Throw ExceptionUtilities.UnexpectedValue(crefOperator.OperatorToken.Kind())
			End If
		End Sub

		Private Shared Sub CollectOperatorsAndConversionsInType(ByVal type As TypeSymbol, ByVal symbols As ArrayBuilder(Of Symbol), ByVal kind As MethodKind, ByVal name1 As String, ByVal info1 As OverloadResolution.OperatorInfo, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal name2 As String = Nothing, Optional ByVal info2 As OverloadResolution.OperatorInfo = Nothing)
			Dim instance As ArrayBuilder(Of MethodSymbol) = ArrayBuilder(Of MethodSymbol).GetInstance()
			OverloadResolution.CollectUserDefinedOperators(type, Nothing, kind, name1, info1, name2, info2, instance, useSiteInfo)
			symbols.AddRange(Of MethodSymbol)(instance)
			instance.Free()
		End Sub

		Private Sub CollectQualifiedNameSymbolsStrict(ByVal node As QualifiedNameSyntax, ByVal typeParameters As Dictionary(Of String, CrefTypeParameterSymbol), ByVal symbols As ArrayBuilder(Of Symbol), ByVal preserveAlias As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			If (Not node.ContainsDiagnostics) Then
				Dim flag As Boolean = True
				Dim left As NameSyntax = node.Left
				Select Case left.Kind()
					Case SyntaxKind.IdentifierName
						Me.CollectSimpleNameSymbolsStrict(DirectCast(left, SimpleNameSyntax), typeParameters, symbols, False, useSiteInfo, True)
						Exit Select
					Case SyntaxKind.GenericName
						Me.CollectSimpleNameSymbolsStrict(DirectCast(left, SimpleNameSyntax), typeParameters, symbols, False, useSiteInfo, True)
						Exit Select
					Case SyntaxKind.QualifiedName
						Me.CollectQualifiedNameSymbolsStrict(DirectCast(left, QualifiedNameSyntax), typeParameters, symbols, False, useSiteInfo)
						flag = False
						Exit Select
					Case SyntaxKind.GlobalName
						symbols.Add(MyBase.Compilation.GlobalNamespace)
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(left.Kind())
				End Select
				If (symbols.Count <> 1) Then
					typeParameters.Clear()
					symbols.Clear()
					Return
				End If
				Dim item As Symbol = symbols(0)
				symbols.Clear()
				Dim right As SimpleNameSyntax = node.Right
				If (right.Kind() = SyntaxKind.GenericName) Then
					Dim genericNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax = DirectCast(right, Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax)
					Dim valueText As String = genericNameSyntax.Identifier.ValueText
					Dim arguments As SeparatedSyntaxList(Of TypeSyntax) = genericNameSyntax.TypeArgumentList.Arguments
					Me.CollectSimpleNameSymbolsStrict(item, flag, valueText, arguments.Count, symbols, preserveAlias, useSiteInfo)
					DocumentationCommentCrefBinder.CreateTypeParameterSymbolsAndConstructSymbols(genericNameSyntax, symbols, typeParameters)
					Return
				End If
				Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = DirectCast(right, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
				Dim identifier As Microsoft.CodeAnalysis.SyntaxToken = identifierNameSyntax.Identifier
				If (CaseInsensitiveComparison.Equals(identifierNameSyntax.Identifier.ValueText, SyntaxFacts.GetText(SyntaxKind.NewKeyword)) AndAlso Not identifier.IsBracketed()) Then
					DocumentationCommentCrefBinder.CollectConstructorsSymbolsStrict(item, symbols)
					Return
				End If
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = identifierNameSyntax.Identifier
				Me.CollectSimpleNameSymbolsStrict(item, flag, syntaxToken.ValueText, 0, symbols, preserveAlias, useSiteInfo)
			End If
		End Sub

		Private Sub CollectQualifiedOperatorReferenceSymbolsStrict(ByVal node As QualifiedCrefOperatorReferenceSyntax, ByVal argCount As Integer, ByVal typeParameters As Dictionary(Of String, CrefTypeParameterSymbol), ByVal symbols As ArrayBuilder(Of Symbol), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			If (Not node.ContainsDiagnostics) Then
				Dim left As NameSyntax = node.Left
				Select Case left.Kind()
					Case SyntaxKind.IdentifierName
						Me.CollectSimpleNameSymbolsStrict(DirectCast(left, SimpleNameSyntax), typeParameters, symbols, False, useSiteInfo, True)
						Exit Select
					Case SyntaxKind.GenericName
						Me.CollectSimpleNameSymbolsStrict(DirectCast(left, SimpleNameSyntax), typeParameters, symbols, False, useSiteInfo, True)
						Exit Select
					Case SyntaxKind.QualifiedName
						Me.CollectQualifiedNameSymbolsStrict(DirectCast(left, QualifiedNameSyntax), typeParameters, symbols, False, useSiteInfo)
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(left.Kind())
				End Select
				If (symbols.Count <> 1) Then
					typeParameters.Clear()
					symbols.Clear()
					Return
				End If
				Dim item As Symbol = symbols(0)
				symbols.Clear()
				If (item.Kind = SymbolKind.[Alias]) Then
					item = DirectCast(item, AliasSymbol).Target
				End If
				DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(node.Right, argCount, TryCast(item, TypeSymbol), symbols, useSiteInfo)
			End If
		End Sub

		Private Sub CollectSimpleNameSymbolsStrict(ByVal node As SimpleNameSyntax, ByVal typeParameters As Dictionary(Of String, CrefTypeParameterSymbol), ByVal symbols As ArrayBuilder(Of Symbol), ByVal preserveAlias As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), ByVal typeOrNamespaceOnly As Boolean)
			If (Not node.ContainsDiagnostics) Then
				If (node.Kind() = SyntaxKind.GenericName) Then
					Dim genericNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax)
					Dim valueText As String = genericNameSyntax.Identifier.ValueText
					Dim arguments As SeparatedSyntaxList(Of TypeSyntax) = genericNameSyntax.TypeArgumentList.Arguments
					Me.CollectSimpleNameSymbolsStrict(valueText, arguments.Count, symbols, preserveAlias, useSiteInfo, typeOrNamespaceOnly)
					DocumentationCommentCrefBinder.CreateTypeParameterSymbolsAndConstructSymbols(genericNameSyntax, symbols, typeParameters)
					Return
				End If
				Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
				Dim identifier As Microsoft.CodeAnalysis.SyntaxToken = identifierNameSyntax.Identifier
				If (CaseInsensitiveComparison.Equals(identifierNameSyntax.Identifier.ValueText, SyntaxFacts.GetText(SyntaxKind.NewKeyword)) AndAlso Not identifier.IsBracketed()) Then
					Me.CollectConstructorsSymbolsStrict(symbols)
					Return
				End If
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = identifierNameSyntax.Identifier
				Me.CollectSimpleNameSymbolsStrict(syntaxToken.ValueText, 0, symbols, preserveAlias, useSiteInfo, typeOrNamespaceOnly)
			End If
		End Sub

		Private Sub CollectSimpleNameSymbolsStrict(ByVal name As String, ByVal arity As Integer, ByVal symbols As ArrayBuilder(Of Symbol), ByVal preserveAlias As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), ByVal typeOrNamespaceOnly As Boolean)
			Dim instance As LookupResult = LookupResult.GetInstance()
			Me.Lookup(instance, name, arity, If(typeOrNamespaceOnly, LookupOptions.NamespacesOrTypesOnly Or LookupOptions.MustNotBeReturnValueVariable Or LookupOptions.IgnoreExtensionMethods Or LookupOptions.UseBaseReferenceAccessibility Or LookupOptions.MustNotBeLocalOrParameter Or LookupOptions.NoSystemObjectLookupForInterfaces, LookupOptions.MustNotBeReturnValueVariable Or LookupOptions.IgnoreExtensionMethods Or LookupOptions.UseBaseReferenceAccessibility Or LookupOptions.MustNotBeLocalOrParameter Or LookupOptions.NoSystemObjectLookupForInterfaces), useSiteInfo)
			If (Not instance.IsGoodOrAmbiguous OrElse Not instance.HasSymbol) Then
				instance.Free()
				Return
			End If
			DocumentationCommentCrefBinder.CollectGoodOrAmbiguousFromLookupResult(instance, symbols, preserveAlias)
			instance.Free()
		End Sub

		Private Sub CollectSimpleNameSymbolsStrict(ByVal containingSymbol As Symbol, ByVal allowColorColor As Boolean, ByVal name As String, ByVal arity As Integer, ByVal symbols As ArrayBuilder(Of Symbol), ByVal preserveAlias As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim instance As LookupResult = LookupResult.GetInstance()
			Dim lookupOption As LookupOptions = LookupOptions.MustNotBeReturnValueVariable Or LookupOptions.IgnoreExtensionMethods Or LookupOptions.UseBaseReferenceAccessibility Or LookupOptions.MustNotBeLocalOrParameter Or LookupOptions.NoSystemObjectLookupForInterfaces
			While True
				Dim kind As SymbolKind = containingSymbol.Kind
				If (kind > SymbolKind.ArrayType) Then
					Select Case kind
						Case SymbolKind.Field
							If (Not allowColorColor) Then
								If (Not instance.IsGoodOrAmbiguous OrElse Not instance.HasSymbol) Then
									instance.Free()
									Return
								End If
								DocumentationCommentCrefBinder.CollectGoodOrAmbiguousFromLookupResult(instance, symbols, preserveAlias)
								instance.Free()
								Return
							End If
							Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = DirectCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
							Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = fieldSymbol.Type
							If (Not CaseInsensitiveComparison.Equals(fieldSymbol.Name, type.Name)) Then
								If (Not instance.IsGoodOrAmbiguous OrElse Not instance.HasSymbol) Then
									instance.Free()
									Return
								End If
								DocumentationCommentCrefBinder.CollectGoodOrAmbiguousFromLookupResult(instance, symbols, preserveAlias)
								instance.Free()
								Return
							End If
							containingSymbol = type
							Continue While
						Case SymbolKind.Label
						Case SymbolKind.Local
						Case SymbolKind.NetModule
							Exit Select
						Case SymbolKind.Method
							If (Not allowColorColor) Then
								If (Not instance.IsGoodOrAmbiguous OrElse Not instance.HasSymbol) Then
									instance.Free()
									Return
								End If
								DocumentationCommentCrefBinder.CollectGoodOrAmbiguousFromLookupResult(instance, symbols, preserveAlias)
								instance.Free()
								Return
							End If
							Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							If (methodSymbol.IsSub) Then
								If (Not instance.IsGoodOrAmbiguous OrElse Not instance.HasSymbol) Then
									instance.Free()
									Return
								End If
								DocumentationCommentCrefBinder.CollectGoodOrAmbiguousFromLookupResult(instance, symbols, preserveAlias)
								instance.Free()
								Return
							End If
							Dim returnType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = methodSymbol.ReturnType
							If (Not CaseInsensitiveComparison.Equals(methodSymbol.Name, returnType.Name)) Then
								If (Not instance.IsGoodOrAmbiguous OrElse Not instance.HasSymbol) Then
									instance.Free()
									Return
								End If
								DocumentationCommentCrefBinder.CollectGoodOrAmbiguousFromLookupResult(instance, symbols, preserveAlias)
								instance.Free()
								Return
							End If
							containingSymbol = returnType
							Continue While
						Case SymbolKind.NamedType
							MyBase.LookupMember(instance, DirectCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol), name, arity, lookupOption, useSiteInfo)
							If (Not instance.IsGoodOrAmbiguous OrElse Not instance.HasSymbol) Then
								instance.Free()
								Return
							End If
							DocumentationCommentCrefBinder.CollectGoodOrAmbiguousFromLookupResult(instance, symbols, preserveAlias)
							instance.Free()
							Return
						Case SymbolKind.[Namespace]
							MyBase.LookupMember(instance, DirectCast(containingSymbol, NamespaceSymbol), name, arity, lookupOption, useSiteInfo)

						Case Else
							If (kind = SymbolKind.[Property]) Then
								If (Not allowColorColor) Then
									If (Not instance.IsGoodOrAmbiguous OrElse Not instance.HasSymbol) Then
										instance.Free()
										Return
									End If
									DocumentationCommentCrefBinder.CollectGoodOrAmbiguousFromLookupResult(instance, symbols, preserveAlias)
									instance.Free()
									Return
								End If
								Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
								Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = propertySymbol.Type
								If (Not CaseInsensitiveComparison.Equals(propertySymbol.Name, typeSymbol.Name)) Then
									If (Not instance.IsGoodOrAmbiguous OrElse Not instance.HasSymbol) Then
										instance.Free()
										Return
									End If
									DocumentationCommentCrefBinder.CollectGoodOrAmbiguousFromLookupResult(instance, symbols, preserveAlias)
									instance.Free()
									Return
								End If
								containingSymbol = typeSymbol
								Continue While
							End If

					End Select
				ElseIf (kind = SymbolKind.[Alias]) Then
					containingSymbol = DirectCast(containingSymbol, AliasSymbol).Target
				Else
					If (kind = SymbolKind.ArrayType) Then
						MyBase.LookupMember(instance, DirectCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol), name, arity, lookupOption, useSiteInfo)
						If (Not instance.IsGoodOrAmbiguous OrElse Not instance.HasSymbol) Then
							instance.Free()
							Return
						End If
						DocumentationCommentCrefBinder.CollectGoodOrAmbiguousFromLookupResult(instance, symbols, preserveAlias)
						instance.Free()
						Return
					End If
					Exit While
				End If
			End While
			If (Not instance.IsGoodOrAmbiguous OrElse Not instance.HasSymbol) Then
				instance.Free()
				Return
			End If
			DocumentationCommentCrefBinder.CollectGoodOrAmbiguousFromLookupResult(instance, symbols, preserveAlias)
			instance.Free()
		End Sub

		Private Sub CollectTopLevelOperatorReferenceStrict(ByVal reference As CrefOperatorReferenceSyntax, ByVal argCount As Integer, ByVal symbols As ArrayBuilder(Of Symbol), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			DocumentationCommentCrefBinder.CollectOperatorsAndConversionsInType(reference, argCount, Me.ContainingType, symbols, useSiteInfo)
		End Sub

		Private Function ConstructGenericSymbolWithTypeArgumentsForCref(ByVal genericSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal genericName As GenericNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim kind As SymbolKind = genericSymbol.Kind
			If (kind > SymbolKind.ErrorType) Then
				If (kind <> SymbolKind.Method) Then
					If (kind = SymbolKind.NamedType) Then
						symbol = DirectCast(genericSymbol, NamedTypeSymbol).Construct(Me.BingTypeArgumentsForCref(genericName.TypeArgumentList.Arguments))
						Return symbol
					End If
					Throw ExceptionUtilities.UnexpectedValue(genericSymbol.Kind)
				End If
				symbol = DirectCast(genericSymbol, MethodSymbol).Construct(Me.BingTypeArgumentsForCref(genericName.TypeArgumentList.Arguments))
				Return symbol
			Else
				If (kind <> SymbolKind.[Alias]) Then
					If (kind = SymbolKind.ErrorType) Then
						symbol = DirectCast(genericSymbol, NamedTypeSymbol).Construct(Me.BingTypeArgumentsForCref(genericName.TypeArgumentList.Arguments))
						Return symbol
					End If
					Throw ExceptionUtilities.UnexpectedValue(genericSymbol.Kind)
				End If
				symbol = Me.ConstructGenericSymbolWithTypeArgumentsForCref(DirectCast(genericSymbol, AliasSymbol).Target, genericName)
				Return symbol
			End If
			Throw ExceptionUtilities.UnexpectedValue(genericSymbol.Kind)
			Return symbol
		End Function

		Private Shared Sub CreateGoodOrAmbiguousFromLookupResultAndFree(ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal result As ArrayBuilder(Of Symbol), ByVal preserveAliases As Boolean)
			Dim diagnostic As DiagnosticInfo = lookupResult.Diagnostic
			If (TypeOf diagnostic Is AmbiguousSymbolDiagnostic) Then
				Dim ambiguousSymbols As ImmutableArray(Of Symbol) = DirectCast(diagnostic, AmbiguousSymbolDiagnostic).AmbiguousSymbols
				If (Not preserveAliases) Then
					Dim enumerator As ImmutableArray(Of Symbol).Enumerator = ambiguousSymbols.GetEnumerator()
					While enumerator.MoveNext()
						result.Add(enumerator.Current.UnwrapAlias())
					End While
				Else
					result.AddRange(ambiguousSymbols)
				End If
			ElseIf (Not preserveAliases) Then
				Dim enumerator1 As ArrayBuilder(Of Symbol).Enumerator = lookupResult.Symbols.GetEnumerator()
				While enumerator1.MoveNext()
					result.Add(enumerator1.Current.UnwrapAlias())
				End While
			Else
				result.AddRange(lookupResult.Symbols)
			End If
			lookupResult.Free()
		End Sub

		Private Shared Sub CreateTypeParameterSymbolsAndConstructSymbols(ByVal genericName As GenericNameSyntax, ByVal symbols As ArrayBuilder(Of Symbol), ByVal typeParameters As Dictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.CrefTypeParameterSymbol))
			Dim typeSymbols As ImmutableArray(Of TypeSymbol)
			Dim arguments As SeparatedSyntaxList(Of TypeSyntax) = genericName.TypeArgumentList.Arguments
			Dim typeSymbolArray(arguments.Count - 1 + 1 - 1) As TypeSymbol
			Dim count As Integer = arguments.Count - 1
			Dim num As Integer = 0
			Do
				Dim item As TypeSyntax = arguments(num)
				Dim crefTypeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.CrefTypeParameterSymbol = Nothing
				If (item.Kind() <> SyntaxKind.IdentifierName) Then
					crefTypeParameterSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.CrefTypeParameterSymbol(num, "?", item)
					typeSymbolArray(num) = crefTypeParameterSymbol
				Else
					Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
					crefTypeParameterSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.CrefTypeParameterSymbol(num, identifierNameSyntax.Identifier.ValueText, identifierNameSyntax)
					typeSymbolArray(num) = crefTypeParameterSymbol
				End If
				typeParameters(crefTypeParameterSymbol.Name) = crefTypeParameterSymbol
				num = num + 1
			Loop While num <= count
			Dim count1 As Integer = symbols.Count - 1
			For i As Integer = 0 To count1
				Dim target As Symbol = symbols(i)
				While True
					Dim kind As SymbolKind = target.Kind
					If (kind <= SymbolKind.ErrorType) Then
						If (kind = SymbolKind.[Alias]) Then
							target = DirectCast(target, AliasSymbol).Target
						Else
							If (kind = SymbolKind.ErrorType) Then
								Exit While
							End If
							GoTo Label0
						End If
					ElseIf (kind = SymbolKind.Method) Then
						Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(target, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						typeSymbols = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(typeSymbolArray)
						symbols(i) = methodSymbol.Construct(typeSymbols.[As](Of TypeSymbol)())
						GoTo Label0
					Else
						If (kind = SymbolKind.NamedType) Then
							Exit While
						End If
						GoTo Label0
					End If
				End While
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(target, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				typeSymbols = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(typeSymbolArray)
				symbols(i) = namedTypeSymbol.Construct(typeSymbols.[As](Of TypeSymbol)())
			Label0:
			Next

		End Sub

		Private Shared Function CrefReferenceIsLegalForLegacyMode(ByVal nameFromCref As TypeSyntax) As Boolean
			Dim flag As Boolean
			flag = If(CUShort(nameFromCref.Kind()) - CUShort(SyntaxKind.PredefinedType) > CUShort((SyntaxKind.List Or SyntaxKind.EmptyStatement)), False, True)
			Return flag
		End Function

		Private Shared Function GetEnclosingCrefReference(ByVal nameFromCref As TypeSyntax, <Out> ByRef partOfSignatureOrReturnType As Boolean) As CrefReferenceSyntax
			partOfSignatureOrReturnType = False
			Dim parent As VisualBasicSyntaxNode = nameFromCref
			While parent IsNot Nothing
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause) Then
					partOfSignatureOrReturnType = True
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CrefReference) Then
						Exit While
					End If
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CrefSignature) Then
						partOfSignatureOrReturnType = True
					End If
				End If
				parent = parent.Parent
			End While
			Return DirectCast(parent, CrefReferenceSyntax)
		End Function

		Private Function GetOrCreateTypeParametersAwareBinder(ByVal typeParameters As Dictionary(Of String, CrefTypeParameterSymbol)) As Binder
			If (Me._typeParameterBinder Is Nothing) Then
				Interlocked.CompareExchange(Of DocumentationCommentCrefBinder.TypeParametersBinder)(Me._typeParameterBinder, New DocumentationCommentCrefBinder.TypeParametersBinder(Me, typeParameters), Nothing)
			End If
			Return Me._typeParameterBinder
		End Function

		Private Function GetOrCreateTypeParametersAwareBinder(ByVal crefReference As CrefReferenceSyntax) As Binder
			Dim orCreateTypeParametersAwareBinder As Binder
			If (Me._typeParameterBinder Is Nothing) Then
				Dim strs As Dictionary(Of String, CrefTypeParameterSymbol) = New Dictionary(Of String, CrefTypeParameterSymbol)(CaseInsensitiveComparison.Comparer)
				Dim name As TypeSyntax = crefReference.Name
				Dim right As GenericNameSyntax = Nothing
				While name IsNot Nothing
					Select Case name.Kind()
						Case SyntaxKind.PredefinedType
						Case SyntaxKind.IdentifierName
						Case SyntaxKind.GlobalName
						Case SyntaxKind.CrefOperatorReference
							GoTo Label0
						Case SyntaxKind.GenericName
							right = DirectCast(name, GenericNameSyntax)
							name = Nothing
							Exit Select
						Case SyntaxKind.QualifiedName
							Dim qualifiedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax = DirectCast(name, Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax)
							name = qualifiedNameSyntax.Left
							If (qualifiedNameSyntax.Right.Kind() <> SyntaxKind.GenericName) Then
								Exit Select
							End If
							right = DirectCast(qualifiedNameSyntax.Right, GenericNameSyntax)
							Exit Select
						Case SyntaxKind.TypeArgumentList
						Case SyntaxKind.CrefReference
						Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.TrueLiteralExpression Or SyntaxKind.DateLiteralExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlName Or SyntaxKind.GenericName Or SyntaxKind.QualifiedName Or SyntaxKind.CrefReference
						Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.LabelStatement Or SyntaxKind.IdentifierLabel Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.AddHandlerStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.FalseLiteralExpression Or SyntaxKind.DateLiteralExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlBracketedName Or SyntaxKind.GenericName Or SyntaxKind.GlobalName Or SyntaxKind.CrefReference
						Case SyntaxKind.CrefSignature
						Case SyntaxKind.CrefSignaturePart
							Throw ExceptionUtilities.UnexpectedValue(name.Kind())
						Case SyntaxKind.QualifiedCrefOperatorReference
							name = DirectCast(name, QualifiedCrefOperatorReferenceSyntax).Left
							Exit Select
						Case Else
							Throw ExceptionUtilities.UnexpectedValue(name.Kind())
					End Select
					If (right Is Nothing) Then
						Continue While
					End If
					Dim arguments As SeparatedSyntaxList(Of TypeSyntax) = right.TypeArgumentList.Arguments
					Dim count As Integer = arguments.Count - 1
					For i As Integer = 0 To count
						Dim item As TypeSyntax = arguments(i)
						If (item.Kind() = SyntaxKind.IdentifierName) Then
							Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
							Dim valueText As String = identifierNameSyntax.Identifier.ValueText
							If (Not strs.ContainsKey(valueText)) Then
								strs(valueText) = New CrefTypeParameterSymbol(i, valueText, identifierNameSyntax)
							End If
						End If
					Next

				End While
			Label0:
				orCreateTypeParametersAwareBinder = Me.GetOrCreateTypeParametersAwareBinder(strs)
			Else
				orCreateTypeParametersAwareBinder = Me._typeParameterBinder
			End If
			Return orCreateTypeParametersAwareBinder
		End Function

		Private Shared Function HasTrailingSkippedTokensAndShouldReportError(ByVal reference As CrefReferenceSyntax) As Boolean
			Dim flag As Boolean
			Dim name As TypeSyntax
			Dim enumerator As SyntaxTriviaList.Enumerator = reference.GetTrailingTrivia().GetEnumerator()
			Do
				If (Not enumerator.MoveNext()) Then
					flag = False
					Return flag
				ElseIf (enumerator.Current.Kind() = SyntaxKind.SkippedTokensTrivia) Then
					name = reference.Name
					If (name.Kind() <> SyntaxKind.IdentifierName) Then
						Continue Do
					End If
					Dim identifier As SyntaxToken = DirectCast(name, IdentifierNameSyntax).Identifier
					If (identifier.IsBracketed() OrElse Not DocumentationCommentBinder.IsIntrinsicTypeForDocumentationComment(SyntaxFacts.GetKeywordKind(identifier.ValueText))) Then
						Exit Do
					End If
					GoTo Label1
				Else
					GoTo Label1
				End If
			Loop While name.Kind() = SyntaxKind.PredefinedType
			flag = True
			Return flag
		End Function

		Private Sub LookupSimpleNameInContainingSymbol(ByVal containingSymbol As Symbol, ByVal allowColorColor As Boolean, ByVal name As String, ByVal arity As Integer, ByVal preserveAliases As Boolean, ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal options As LookupOptions, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			While True
				Dim kind As SymbolKind = containingSymbol.Kind
				If (kind > SymbolKind.ArrayType) Then
					Select Case kind
						Case SymbolKind.Field
							If (Not allowColorColor) Then
								Return
							End If
							Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = DirectCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
							Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = fieldSymbol.Type
							If (Not CaseInsensitiveComparison.Equals(fieldSymbol.Name, type.Name)) Then
								Return
							End If
							containingSymbol = type
							Continue While
						Case SymbolKind.Label
						Case SymbolKind.Local
						Case SymbolKind.NetModule
							Return
						Case SymbolKind.Method
							If (Not allowColorColor) Then
								Return
							End If
							Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							If (methodSymbol.IsSub) Then
								Return
							End If
							Dim returnType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = methodSymbol.ReturnType
							If (Not CaseInsensitiveComparison.Equals(methodSymbol.Name, returnType.Name)) Then
								Return
							End If
							containingSymbol = returnType
							Continue While
						Case SymbolKind.NamedType
							Exit Select
						Case SymbolKind.[Namespace]
							MyBase.LookupMember(lookupResult, DirectCast(containingSymbol, NamespaceSymbol), name, arity, options, useSiteInfo)
							Return
						Case Else
							If (kind <> SymbolKind.[Property]) Then
								Return
							End If
							If (Not allowColorColor) Then
								Return
							End If
							Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
							Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = propertySymbol.Type
							If (Not CaseInsensitiveComparison.Equals(propertySymbol.Name, typeSymbol.Name)) Then
								Return
							End If
							containingSymbol = typeSymbol
							Continue While
					End Select
				ElseIf (kind = SymbolKind.[Alias]) Then
					If (preserveAliases) Then
						Exit While
					End If
					containingSymbol = DirectCast(containingSymbol, AliasSymbol).Target
					Continue While
				ElseIf (kind <> SymbolKind.ArrayType) Then
					Return
				End If
				MyBase.LookupMember(lookupResult, DirectCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol), name, arity, options, useSiteInfo)
				Return
			End While
		End Sub

		Private Shared Function NameSyntaxHasComplexGenericArguments(ByVal name As TypeSyntax) As Boolean
			Dim flag As Boolean
			Select Case name.Kind()
				Case SyntaxKind.PredefinedType
				Case SyntaxKind.IdentifierName
				Case SyntaxKind.GlobalName
				Case SyntaxKind.CrefOperatorReference
					flag = False
					Exit Select
				Case SyntaxKind.GenericName
					Dim arguments As SeparatedSyntaxList(Of TypeSyntax) = DirectCast(name, GenericNameSyntax).TypeArgumentList.Arguments
					Dim count As Integer = arguments.Count - 1
					Dim num As Integer = 0
					While num <= count
						If (arguments(num).Kind() = SyntaxKind.IdentifierName) Then
							num = num + 1
						Else
							flag = True
							Return flag
						End If
					End While
					flag = False
					Exit Select
				Case SyntaxKind.QualifiedName
					Dim qualifiedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax = DirectCast(name, Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax)
					flag = If(DocumentationCommentCrefBinder.NameSyntaxHasComplexGenericArguments(qualifiedNameSyntax.Left), True, DocumentationCommentCrefBinder.NameSyntaxHasComplexGenericArguments(qualifiedNameSyntax.Right))
					Exit Select
				Case SyntaxKind.TypeArgumentList
				Case SyntaxKind.CrefReference
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.TrueLiteralExpression Or SyntaxKind.DateLiteralExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlName Or SyntaxKind.GenericName Or SyntaxKind.QualifiedName Or SyntaxKind.CrefReference
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.LabelStatement Or SyntaxKind.IdentifierLabel Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.AddHandlerStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.FalseLiteralExpression Or SyntaxKind.DateLiteralExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlBracketedName Or SyntaxKind.GenericName Or SyntaxKind.GlobalName Or SyntaxKind.CrefReference
				Case SyntaxKind.CrefSignature
				Case SyntaxKind.CrefSignaturePart
					Throw ExceptionUtilities.UnexpectedValue(name.Kind())
				Case SyntaxKind.QualifiedCrefOperatorReference
					flag = DocumentationCommentCrefBinder.NameSyntaxHasComplexGenericArguments(DirectCast(name, QualifiedCrefOperatorReferenceSyntax).Left)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(name.Kind())
			End Select
			Return flag
		End Function

		Private Structure SignatureElement
			Public ReadOnly Type As TypeSymbol

			Public ReadOnly IsByRef As Boolean

			Public Sub New(ByVal type As TypeSymbol, ByVal isByRef As Boolean)
				Me = New DocumentationCommentCrefBinder.SignatureElement() With
				{
					.Type = type,
					.IsByRef = isByRef
				}
			End Sub
		End Structure

		Private NotInheritable Class TypeParametersBinder
			Inherits Binder
			Friend ReadOnly _typeParameters As Dictionary(Of String, CrefTypeParameterSymbol)

			Public Sub New(ByVal containingBinder As Binder, ByVal typeParameters As Dictionary(Of String, CrefTypeParameterSymbol))
				MyBase.New(containingBinder)
				Me._typeParameters = typeParameters
			End Sub

			Friend Overrides Sub AddLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
				Dim enumerator As Dictionary(Of String, CrefTypeParameterSymbol).ValueCollection.Enumerator = New Dictionary(Of String, CrefTypeParameterSymbol).ValueCollection.Enumerator()
				Try
					enumerator = Me._typeParameters.Values.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As CrefTypeParameterSymbol = enumerator.Current
						If (Not originalBinder.CanAddLookupSymbolInfo(current, options, nameSet, Nothing)) Then
							Continue While
						End If
						nameSet.AddSymbol(current, current.Name, 0)
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			End Sub

			Friend Overrides Sub LookupInSingleBinder(ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal name As String, ByVal arity As Integer, ByVal options As LookupOptions, ByVal originalBinder As Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
				Dim crefTypeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.CrefTypeParameterSymbol = Nothing
				If (Me._typeParameters.TryGetValue(name, crefTypeParameterSymbol)) Then
					lookupResult.SetFrom(MyBase.CheckViability(crefTypeParameterSymbol, arity, options Or LookupOptions.IgnoreAccessibility, Nothing, useSiteInfo))
				End If
			End Sub
		End Class
	End Class
End Namespace