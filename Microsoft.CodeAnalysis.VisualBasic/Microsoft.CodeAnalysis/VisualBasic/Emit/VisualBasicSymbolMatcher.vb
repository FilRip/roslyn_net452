Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend NotInheritable Class VisualBasicSymbolMatcher
		Inherits SymbolMatcher
		Private ReadOnly Shared s_nameComparer As StringComparer

		Private ReadOnly _defs As VisualBasicSymbolMatcher.MatchDefs

		Private ReadOnly _symbols As VisualBasicSymbolMatcher.MatchSymbols

		Shared Sub New()
			VisualBasicSymbolMatcher.s_nameComparer = CaseInsensitiveComparison.Comparer
		End Sub

		Public Sub New(ByVal anonymousTypeMap As IReadOnlyDictionary(Of AnonymousTypeKey, AnonymousTypeValue), ByVal sourceAssembly As SourceAssemblySymbol, ByVal sourceContext As EmitContext, ByVal otherAssembly As SourceAssemblySymbol, ByVal otherContext As EmitContext, ByVal otherSynthesizedMembersOpt As ImmutableDictionary(Of ISymbolInternal, ImmutableArray(Of ISymbolInternal)))
			MyBase.New()
			Me._defs = New VisualBasicSymbolMatcher.MatchDefsToSource(sourceContext, otherContext)
			Me._symbols = New VisualBasicSymbolMatcher.MatchSymbols(anonymousTypeMap, sourceAssembly, otherAssembly, otherSynthesizedMembersOpt, New VisualBasicSymbolMatcher.DeepTranslator(otherAssembly.GetSpecialType(SpecialType.System_Object)))
		End Sub

		Public Sub New(ByVal anonymousTypeMap As IReadOnlyDictionary(Of AnonymousTypeKey, AnonymousTypeValue), ByVal sourceAssembly As SourceAssemblySymbol, ByVal sourceContext As EmitContext, ByVal otherAssembly As PEAssemblySymbol)
			MyBase.New()
			Me._defs = New VisualBasicSymbolMatcher.MatchDefsToMetadata(sourceContext, otherAssembly)
			Me._symbols = New VisualBasicSymbolMatcher.MatchSymbols(anonymousTypeMap, sourceAssembly, otherAssembly, Nothing, Nothing)
		End Sub

		Public Overrides Function MapDefinition(ByVal definition As IDefinition) As IDefinition
			Dim definition1 As IDefinition
			Dim cciAdapter As Object
			Dim internalSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = TryCast(definition.GetInternalSymbol(), Microsoft.CodeAnalysis.VisualBasic.Symbol)
			If (internalSymbol Is Nothing) Then
				definition1 = Me._defs.VisitDef(definition)
			Else
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me._symbols.Visit(internalSymbol)
				If (symbol IsNot Nothing) Then
					cciAdapter = symbol.GetCciAdapter()
				Else
					cciAdapter = Nothing
				End If
				definition1 = DirectCast(cciAdapter, IDefinition)
			End If
			Return definition1
		End Function

		Public Overrides Function MapNamespace(ByVal [namespace] As INamespace) As INamespace
			Dim internalSymbol As Object
			Dim cciAdapter As Object
			Dim matchSymbol As VisualBasicSymbolMatcher.MatchSymbols = Me._symbols
			If ([namespace] IsNot Nothing) Then
				internalSymbol = [namespace].GetInternalSymbol()
			Else
				internalSymbol = Nothing
			End If
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = matchSymbol.Visit(DirectCast(internalSymbol, NamespaceSymbol))
			If (symbol IsNot Nothing) Then
				cciAdapter = symbol.GetCciAdapter()
			Else
				cciAdapter = Nothing
			End If
			Return DirectCast(cciAdapter, INamespace)
		End Function

		Public Overrides Function MapReference(ByVal reference As ITypeReference) As ITypeReference
			Dim typeReference As ITypeReference
			Dim cciAdapter As Object
			Dim internalSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = TryCast(reference.GetInternalSymbol(), Microsoft.CodeAnalysis.VisualBasic.Symbol)
			If (internalSymbol Is Nothing) Then
				typeReference = Nothing
			Else
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me._symbols.Visit(internalSymbol)
				If (symbol IsNot Nothing) Then
					cciAdapter = symbol.GetCciAdapter()
				Else
					cciAdapter = Nothing
				End If
				typeReference = DirectCast(cciAdapter, ITypeReference)
			End If
			Return typeReference
		End Function

		Friend Function TryGetAnonymousTypeName(ByVal template As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol, <Out> ByRef name As String, <Out> ByRef index As Integer) As Boolean
			Return Me._symbols.TryGetAnonymousTypeName(template, name, index)
		End Function

		Friend NotInheritable Class DeepTranslator
			Inherits VisualBasicSymbolVisitor(Of Symbol)
			Private ReadOnly _matches As ConcurrentDictionary(Of Symbol, Symbol)

			Private ReadOnly _systemObject As NamedTypeSymbol

			Public Sub New(ByVal systemObject As NamedTypeSymbol)
				MyBase.New()
				Me._matches = New ConcurrentDictionary(Of Symbol, Symbol)(ReferenceEqualityComparer.Instance)
				Me._systemObject = systemObject
			End Sub

			Public Overrides Function DefaultVisit(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Throw ExceptionUtilities.Unreachable
			End Function

			Public Overrides Function Visit(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me._matches.GetOrAdd(symbol, New Func(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Microsoft.CodeAnalysis.VisualBasic.Symbol)(AddressOf Me.Visit))
			End Function

			Public Overrides Function VisitArrayType(ByVal symbol As ArrayTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim symbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = DirectCast(Me.Visit(symbol.ElementType), Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
				Dim customModifiers As ImmutableArray(Of CustomModifier) = Me.VisitCustomModifiers(symbol.CustomModifiers)
				symbol1 = If(Not symbol.IsSZArray, ArrayTypeSymbol.CreateMDArray(typeSymbol, customModifiers, symbol.Rank, symbol.Sizes, symbol.LowerBounds, symbol.BaseTypeNoUseSiteDiagnostics.ContainingAssembly), ArrayTypeSymbol.CreateSZArray(typeSymbol, customModifiers, symbol.BaseTypeNoUseSiteDiagnostics.ContainingAssembly))
				Return symbol1
			End Function

			Private Function VisitCustomModifier(ByVal modifier As CustomModifier) As CustomModifier
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(Me.Visit(DirectCast(modifier.Modifier, Symbol)), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (Not modifier.IsOptional) Then
					Return VisualBasicCustomModifier.CreateRequired(namedTypeSymbol)
				End If
				Return VisualBasicCustomModifier.CreateOptional(namedTypeSymbol)
			End Function

			Private Function VisitCustomModifiers(ByVal modifiers As ImmutableArray(Of CustomModifier)) As ImmutableArray(Of CustomModifier)
				Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of CustomModifier, CustomModifier)(modifiers, New Func(Of CustomModifier, CustomModifier)(AddressOf Me.VisitCustomModifier))
			End Function

			Public Overrides Function VisitNamedType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim typeWithModifier As Func(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers, VisualBasicSymbolMatcher.DeepTranslator, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers)
				If (type.IsTupleType) Then
					type = type.TupleUnderlyingType
				End If
				Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = type.OriginalDefinition
				If (CObj(originalDefinition) <> CObj(type)) Then
					Dim allTypeArgumentsWithModifiers As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers) = type.GetAllTypeArgumentsWithModifiers()
					If (VisualBasicSymbolMatcher.DeepTranslator._Closure$__.$I6-0 Is Nothing) Then
						typeWithModifier = Function(t As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers, v As VisualBasicSymbolMatcher.DeepTranslator) New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers(DirectCast(v.Visit(t.Type), TypeSymbol), v.VisitCustomModifiers(t.CustomModifiers))
						VisualBasicSymbolMatcher.DeepTranslator._Closure$__.$I6-0 = typeWithModifier
					Else
						typeWithModifier = VisualBasicSymbolMatcher.DeepTranslator._Closure$__.$I6-0
					End If
					Dim typeWithModifiers As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers, VisualBasicSymbolMatcher.DeepTranslator, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers)(allTypeArgumentsWithModifiers, typeWithModifier, Me)
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(Me.Visit(originalDefinition), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(namedTypeSymbol, namedTypeSymbol.GetAllTypeParameters(), typeWithModifiers, False)
					symbol = namedTypeSymbol.Construct(typeSubstitution)
				ElseIf (Not type.IsAnonymousType) Then
					symbol = type
				Else
					symbol = Me.Visit(AnonymousTypeManager.TranslateAnonymousTypeSymbol(type))
				End If
				Return symbol
			End Function

			Public Overrides Function VisitTypeParameter(ByVal symbol As TypeParameterSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return symbol
			End Function
		End Class

		Private MustInherit Class MatchDefs
			Private ReadOnly _sourceContext As EmitContext

			Private ReadOnly _matches As ConcurrentDictionary(Of IDefinition, IDefinition)

			Private _lazyTopLevelTypes As IReadOnlyDictionary(Of String, INamespaceTypeDefinition)

			Public Sub New(ByVal sourceContext As EmitContext)
				MyBase.New()
				Me._sourceContext = sourceContext
				Me._matches = New ConcurrentDictionary(Of IDefinition, IDefinition)(ReferenceEqualityComparer.Instance)
			End Sub

			Protected MustOverride Function GetFields(ByVal def As ITypeDefinition) As IEnumerable(Of IFieldDefinition)

			Protected MustOverride Function GetNestedTypes(ByVal def As ITypeDefinition) As IEnumerable(Of INestedTypeDefinition)

			Protected MustOverride Function GetTopLevelTypes() As IEnumerable(Of INamespaceTypeDefinition)

			Private Function GetTopLevelTypesByName() As IReadOnlyDictionary(Of String, INamespaceTypeDefinition)
				Dim enumerator As IEnumerator(Of INamespaceTypeDefinition) = Nothing
				If (Me._lazyTopLevelTypes Is Nothing) Then
					Using strs As Dictionary(Of String, INamespaceTypeDefinition) = New Dictionary(Of String, INamespaceTypeDefinition)(VisualBasicSymbolMatcher.s_nameComparer)
						enumerator = Me.GetTopLevelTypes().GetEnumerator()
						While enumerator.MoveNext()
							Dim current As INamespaceTypeDefinition = enumerator.Current
							If (Not [String].IsNullOrEmpty(current.NamespaceName)) Then
								Continue While
							End If
							strs.Add(current.Name, current)
						End While
					End Using
					Interlocked.CompareExchange(Of IReadOnlyDictionary(Of String, INamespaceTypeDefinition))(Me._lazyTopLevelTypes, strs, Nothing)
				End If
				Return Me._lazyTopLevelTypes
			End Function

			Public Function VisitDef(ByVal def As IDefinition) As IDefinition
				Return Me._matches.GetOrAdd(def, New Func(Of IDefinition, IDefinition)(AddressOf Me.VisitDefInternal))
			End Function

			Private Function VisitDefInternal(ByVal def As IDefinition) As IDefinition
				Dim definition As IDefinition
				Dim func As Func(Of IFieldDefinition, IFieldDefinition, Boolean)
				Dim func1 As Func(Of INestedTypeDefinition, INestedTypeDefinition, Boolean)
				Dim typeDefinition As ITypeDefinition = TryCast(def, ITypeDefinition)
				If (typeDefinition Is Nothing) Then
					Dim typeDefinitionMember As ITypeDefinitionMember = TryCast(def, ITypeDefinitionMember)
					If (typeDefinitionMember IsNot Nothing) Then
						Dim typeDefinition1 As ITypeDefinition = DirectCast(Me.VisitDef(typeDefinitionMember.ContainingTypeDefinition), ITypeDefinition)
						If (typeDefinition1 IsNot Nothing) Then
							Dim fieldDefinition As IFieldDefinition = TryCast(def, IFieldDefinition)
							If (fieldDefinition Is Nothing) Then
								Throw ExceptionUtilities.UnexpectedValue(def)
							End If
							Dim typeDefinition2 As ITypeDefinition = typeDefinition1
							Dim fieldDefinition1 As IFieldDefinition = fieldDefinition
							Dim matchDef As VisualBasicSymbolMatcher.MatchDefs = Me
							Dim func2 As Func(Of ITypeDefinition, IEnumerable(Of IFieldDefinition)) = New Func(Of ITypeDefinition, IEnumerable(Of IFieldDefinition))(AddressOf matchDef.GetFields)
							If (VisualBasicSymbolMatcher.MatchDefs._Closure$__.$I5-1 Is Nothing) Then
								func = Function(a As IFieldDefinition, b As IFieldDefinition) VisualBasicSymbolMatcher.s_nameComparer.Equals(a.Name, b.Name)
								VisualBasicSymbolMatcher.MatchDefs._Closure$__.$I5-1 = func
							Else
								func = VisualBasicSymbolMatcher.MatchDefs._Closure$__.$I5-1
							End If
							definition = VisualBasicSymbolMatcher.MatchDefs.VisitTypeMembers(Of IFieldDefinition)(typeDefinition2, fieldDefinition1, func2, func)
							Return definition
						Else
							definition = Nothing
							Return definition
						End If
					End If
					Throw ExceptionUtilities.UnexpectedValue(def)
				Else
					Dim namespaceTypeDefinition As INamespaceTypeDefinition = typeDefinition.AsNamespaceTypeDefinition(Me._sourceContext)
					If (namespaceTypeDefinition Is Nothing) Then
						Dim nestedTypeDefinition As INestedTypeDefinition = typeDefinition.AsNestedTypeDefinition(Me._sourceContext)
						Dim typeDefinition3 As ITypeDefinition = DirectCast(Me.VisitDef(nestedTypeDefinition.ContainingTypeDefinition), ITypeDefinition)
						If (typeDefinition3 IsNot Nothing) Then
							Dim typeDefinition4 As ITypeDefinition = typeDefinition3
							Dim nestedTypeDefinition1 As INestedTypeDefinition = nestedTypeDefinition
							Dim matchDef1 As VisualBasicSymbolMatcher.MatchDefs = Me
							Dim func3 As Func(Of ITypeDefinition, IEnumerable(Of INestedTypeDefinition)) = New Func(Of ITypeDefinition, IEnumerable(Of INestedTypeDefinition))(AddressOf matchDef1.GetNestedTypes)
							If (VisualBasicSymbolMatcher.MatchDefs._Closure$__.$I5-0 Is Nothing) Then
								func1 = Function(a As INestedTypeDefinition, b As INestedTypeDefinition) VisualBasicSymbolMatcher.s_nameComparer.Equals(a.Name, b.Name)
								VisualBasicSymbolMatcher.MatchDefs._Closure$__.$I5-0 = func1
							Else
								func1 = VisualBasicSymbolMatcher.MatchDefs._Closure$__.$I5-0
							End If
							definition = VisualBasicSymbolMatcher.MatchDefs.VisitTypeMembers(Of INestedTypeDefinition)(typeDefinition4, nestedTypeDefinition1, func3, func1)
						Else
							definition = Nothing
						End If
					Else
						definition = Me.VisitNamespaceType(namespaceTypeDefinition)
					End If
				End If
				Return definition
			End Function

			Private Function VisitNamespaceType(ByVal def As INamespaceTypeDefinition) As INamespaceTypeDefinition
				Dim namespaceTypeDefinition As INamespaceTypeDefinition
				If ([String].IsNullOrEmpty(def.NamespaceName)) Then
					Dim namespaceTypeDefinition1 As INamespaceTypeDefinition = Nothing
					Me.GetTopLevelTypesByName().TryGetValue(def.Name, namespaceTypeDefinition1)
					namespaceTypeDefinition = namespaceTypeDefinition1
				Else
					namespaceTypeDefinition = Nothing
				End If
				Return namespaceTypeDefinition
			End Function

			Private Shared Function VisitTypeMembers(Of T As {Class, ITypeDefinitionMember})(ByVal otherContainer As ITypeDefinition, ByVal member As T, ByVal getMembers As Func(Of ITypeDefinition, IEnumerable(Of T)), ByVal predicate As Func(Of T, T, Boolean)) As T
				Return getMembers(otherContainer).FirstOrDefault(Function(otherMember As $CLS0) predicate(member, otherMember))
			End Function
		End Class

		Private NotInheritable Class MatchDefsToMetadata
			Inherits VisualBasicSymbolMatcher.MatchDefs
			Private ReadOnly _otherAssembly As PEAssemblySymbol

			Public Sub New(ByVal sourceContext As EmitContext, ByVal otherAssembly As PEAssemblySymbol)
				MyBase.New(sourceContext)
				Me._otherAssembly = otherAssembly
			End Sub

			Protected Overrides Function GetFields(ByVal def As ITypeDefinition) As IEnumerable(Of IFieldDefinition)
				Return DirectCast(def, PENamedTypeSymbol).GetFieldsToEmit().Cast(Of IFieldDefinition)()
			End Function

			Protected Overrides Function GetNestedTypes(ByVal def As ITypeDefinition) As IEnumerable(Of INestedTypeDefinition)
				Return DirectCast(DirectCast(def, PENamedTypeSymbol).GetTypeMembers(), IEnumerable).Cast(Of INestedTypeDefinition)()
			End Function

			Protected Overrides Function GetTopLevelTypes() As IEnumerable(Of INamespaceTypeDefinition)
				Dim instance As ArrayBuilder(Of INamespaceTypeDefinition) = ArrayBuilder(Of INamespaceTypeDefinition).GetInstance()
				VisualBasicSymbolMatcher.MatchDefsToMetadata.GetTopLevelTypes(instance, Me._otherAssembly.GlobalNamespace)
				Return instance.ToArrayAndFree()
			End Function

			Private Shared Sub GetTopLevelTypes(ByVal builder As ArrayBuilder(Of INamespaceTypeDefinition), ByVal [namespace] As NamespaceSymbol)
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = [namespace].GetMembers().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					If (current.Kind <> SymbolKind.[Namespace]) Then
						builder.Add(DirectCast(current.GetCciAdapter(), INamespaceTypeDefinition))
					Else
						VisualBasicSymbolMatcher.MatchDefsToMetadata.GetTopLevelTypes(builder, DirectCast(current, NamespaceSymbol))
					End If
				End While
			End Sub
		End Class

		Private NotInheritable Class MatchDefsToSource
			Inherits VisualBasicSymbolMatcher.MatchDefs
			Private ReadOnly _otherContext As EmitContext

			Public Sub New(ByVal sourceContext As EmitContext, ByVal otherContext As EmitContext)
				MyBase.New(sourceContext)
				Me._otherContext = otherContext
			End Sub

			Protected Overrides Function GetFields(ByVal def As ITypeDefinition) As IEnumerable(Of IFieldDefinition)
				Return def.GetFields(Me._otherContext)
			End Function

			Protected Overrides Function GetNestedTypes(ByVal def As ITypeDefinition) As IEnumerable(Of INestedTypeDefinition)
				Return def.GetNestedTypes(Me._otherContext)
			End Function

			Protected Overrides Function GetTopLevelTypes() As IEnumerable(Of INamespaceTypeDefinition)
				Return Me._otherContext.[Module].GetTopLevelTypeDefinitions(Me._otherContext)
			End Function
		End Class

		Private NotInheritable Class MatchSymbols
			Inherits VisualBasicSymbolVisitor(Of Symbol)
			Private ReadOnly _anonymousTypeMap As IReadOnlyDictionary(Of AnonymousTypeKey, AnonymousTypeValue)

			Private ReadOnly _comparer As VisualBasicSymbolMatcher.MatchSymbols.SymbolComparer

			Private ReadOnly _matches As ConcurrentDictionary(Of Symbol, Symbol)

			Private ReadOnly _sourceAssembly As SourceAssemblySymbol

			Private ReadOnly _otherAssembly As AssemblySymbol

			Private ReadOnly _otherSynthesizedMembersOpt As ImmutableDictionary(Of ISymbolInternal, ImmutableArray(Of ISymbolInternal))

			Private ReadOnly _otherMembers As ConcurrentDictionary(Of ISymbolInternal, IReadOnlyDictionary(Of String, ImmutableArray(Of ISymbolInternal)))

			Public Sub New(ByVal anonymousTypeMap As IReadOnlyDictionary(Of AnonymousTypeKey, AnonymousTypeValue), ByVal sourceAssembly As SourceAssemblySymbol, ByVal otherAssembly As AssemblySymbol, ByVal otherSynthesizedMembersOpt As ImmutableDictionary(Of ISymbolInternal, ImmutableArray(Of ISymbolInternal)), ByVal deepTranslatorOpt As VisualBasicSymbolMatcher.DeepTranslator)
				MyBase.New()
				Me._anonymousTypeMap = anonymousTypeMap
				Me._sourceAssembly = sourceAssembly
				Me._otherAssembly = otherAssembly
				Me._otherSynthesizedMembersOpt = otherSynthesizedMembersOpt
				Me._comparer = New VisualBasicSymbolMatcher.MatchSymbols.SymbolComparer(Me, deepTranslatorOpt)
				Me._matches = New ConcurrentDictionary(Of Symbol, Symbol)(ReferenceEqualityComparer.Instance)
				Me._otherMembers = New ConcurrentDictionary(Of ISymbolInternal, IReadOnlyDictionary(Of String, ImmutableArray(Of ISymbolInternal)))(ReferenceEqualityComparer.Instance)
			End Sub

			Private Function AreArrayTypesEqual(ByVal type As ArrayTypeSymbol, ByVal other As ArrayTypeSymbol) As Boolean
				If (Not type.HasSameShapeAs(other)) Then
					Return False
				End If
				Return Me.AreTypesEqual(type.ElementType, other.ElementType)
			End Function

			Private Function AreEventsEqual(ByVal [event] As EventSymbol, ByVal other As EventSymbol) As Boolean
				Return Me._comparer.Equals([event].Type, other.Type)
			End Function

			Private Function AreFieldsEqual(ByVal field As FieldSymbol, ByVal other As FieldSymbol) As Boolean
				Return Me._comparer.Equals(field.Type, other.Type)
			End Function

			Private Function AreMethodsEqual(ByVal method As MethodSymbol, ByVal other As MethodSymbol) As Boolean
				method = VisualBasicSymbolMatcher.MatchSymbols.SubstituteTypeParameters(method)
				other = VisualBasicSymbolMatcher.MatchSymbols.SubstituteTypeParameters(other)
				If (Not Me._comparer.Equals(method.ReturnType, other.ReturnType) OrElse Not method.Parameters.SequenceEqual(Of ParameterSymbol)(other.Parameters, New Func(Of ParameterSymbol, ParameterSymbol, Boolean)(AddressOf Me.AreParametersEqual))) Then
					Return False
				End If
				Return method.TypeParameters.SequenceEqual(Of TypeParameterSymbol)(other.TypeParameters, New Func(Of TypeParameterSymbol, TypeParameterSymbol, Boolean)(AddressOf Me.AreTypesEqual))
			End Function

			Private Function AreNamedTypesEqual(ByVal type As NamedTypeSymbol, ByVal other As NamedTypeSymbol) As Boolean
				Return type.TypeArgumentsNoUseSiteDiagnostics.SequenceEqual(Of TypeSymbol)(other.TypeArgumentsNoUseSiteDiagnostics, New Func(Of TypeSymbol, TypeSymbol, Boolean)(AddressOf Me.AreTypesEqual))
			End Function

			Private Function AreNamespacesEqual(ByVal [namespace] As NamespaceSymbol, ByVal other As NamespaceSymbol) As Boolean
				Return True
			End Function

			Private Function AreParametersEqual(ByVal parameter As ParameterSymbol, ByVal other As ParameterSymbol) As Boolean
				If (Not VisualBasicSymbolMatcher.s_nameComparer.Equals(parameter.Name, other.Name) OrElse parameter.IsByRef <> other.IsByRef) Then
					Return False
				End If
				Return Me._comparer.Equals(parameter.Type, other.Type)
			End Function

			Private Function ArePropertiesEqual(ByVal [property] As PropertySymbol, ByVal other As PropertySymbol) As Boolean
				If (Not Me._comparer.Equals([property].Type, other.Type)) Then
					Return False
				End If
				Return [property].Parameters.SequenceEqual(Of ParameterSymbol)(other.Parameters, New Func(Of ParameterSymbol, ParameterSymbol, Boolean)(AddressOf Me.AreParametersEqual))
			End Function

			Private Shared Function AreTypeParametersEqual(ByVal type As TypeParameterSymbol, ByVal other As TypeParameterSymbol) As Boolean
				Return True
			End Function

			Private Function AreTypesEqual(ByVal type As TypeSymbol, ByVal other As TypeSymbol) As Boolean
				Dim flag As Boolean
				If (type.Kind = other.Kind) Then
					Dim kind As SymbolKind = type.Kind
					If (kind > SymbolKind.ErrorType) Then
						If (kind = SymbolKind.NamedType) Then
							flag = Me.AreNamedTypesEqual(DirectCast(type, NamedTypeSymbol), DirectCast(other, NamedTypeSymbol))
							Return flag
						End If
						If (kind <> SymbolKind.TypeParameter) Then
							Throw ExceptionUtilities.UnexpectedValue(type.Kind)
						End If
						flag = VisualBasicSymbolMatcher.MatchSymbols.AreTypeParametersEqual(DirectCast(type, TypeParameterSymbol), DirectCast(other, TypeParameterSymbol))
						Return flag
					Else
						If (kind <> SymbolKind.ArrayType) Then
							If (kind = SymbolKind.ErrorType) Then
								flag = Me.AreNamedTypesEqual(DirectCast(type, NamedTypeSymbol), DirectCast(other, NamedTypeSymbol))
								Return flag
							End If
							Throw ExceptionUtilities.UnexpectedValue(type.Kind)
						End If
						flag = Me.AreArrayTypesEqual(DirectCast(type, ArrayTypeSymbol), DirectCast(other, ArrayTypeSymbol))
						Return flag
					End If
					Throw ExceptionUtilities.UnexpectedValue(type.Kind)
				Else
					flag = False
				End If
				Return flag
			End Function

			Public Overrides Function DefaultVisit(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Throw ExceptionUtilities.Unreachable
			End Function

			Private Function FindMatchingMember(Of T As Symbol)(ByVal otherTypeOrNamespace As ISymbolInternal, ByVal sourceMember As T, ByVal predicate As Func(Of T, T, Boolean)) As T
				Dim t1 As T
				Dim orAdd As IReadOnlyDictionary(Of String, ImmutableArray(Of ISymbolInternal)) = Me._otherMembers.GetOrAdd(otherTypeOrNamespace, New Func(Of ISymbolInternal, IReadOnlyDictionary(Of String, ImmutableArray(Of ISymbolInternal)))(AddressOf Me.GetAllEmittedMembers))
				Dim symbolInternals As ImmutableArray(Of ISymbolInternal) = New ImmutableArray(Of ISymbolInternal)()
				If (orAdd.TryGetValue(sourceMember.Name, symbolInternals)) Then
					Dim enumerator As ImmutableArray(Of ISymbolInternal).Enumerator = symbolInternals.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As T = DirectCast(TryCast(enumerator.Current, T), T)
						If (current Is Nothing OrElse Not predicate(sourceMember, current)) Then
							Continue While
						End If
						t1 = current
						Return t1
					End While
				End If
				t1 = Nothing
				Return t1
			End Function

			Private Function GetAllEmittedMembers(ByVal symbol As ISymbolInternal) As IReadOnlyDictionary(Of String, ImmutableArray(Of ISymbolInternal))
				Dim name As Func(Of ISymbolInternal, String)
				Dim instance As ArrayBuilder(Of ISymbolInternal) = ArrayBuilder(Of ISymbolInternal).GetInstance()
				If (symbol.Kind <> SymbolKind.NamedType) Then
					instance.AddRange(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)(DirectCast(symbol, NamespaceSymbol).GetMembers())
				Else
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					instance.AddRange(namedTypeSymbol.GetEventsToEmit())
					instance.AddRange(namedTypeSymbol.GetFieldsToEmit())
					instance.AddRange(namedTypeSymbol.GetMethodsToEmit())
					instance.AddRange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(namedTypeSymbol.GetTypeMembers())
					instance.AddRange(namedTypeSymbol.GetPropertiesToEmit())
				End If
				Dim symbolInternals As ImmutableArray(Of ISymbolInternal) = New ImmutableArray(Of ISymbolInternal)()
				If (Me._otherSynthesizedMembersOpt IsNot Nothing AndAlso Me._otherSynthesizedMembersOpt.TryGetValue(symbol, symbolInternals)) Then
					instance.AddRange(symbolInternals)
				End If
				Dim symbolInternals1 As ArrayBuilder(Of ISymbolInternal) = instance
				If (VisualBasicSymbolMatcher.MatchSymbols._Closure$__.$I39-0 Is Nothing) Then
					name = Function(s As ISymbolInternal) s.Name
					VisualBasicSymbolMatcher.MatchSymbols._Closure$__.$I39-0 = name
				Else
					name = VisualBasicSymbolMatcher.MatchSymbols._Closure$__.$I39-0
				End If
				Dim dictionary As Dictionary(Of String, ImmutableArray(Of !0)) = symbolInternals1.ToDictionary(Of String)(name, VisualBasicSymbolMatcher.s_nameComparer)
				instance.Free()
				Return dictionary
			End Function

			Private Shared Function IdentityEqualIgnoringVersionWildcard(ByVal left As AssemblySymbol, ByVal right As AssemblySymbol) As Boolean
				Dim identity As Microsoft.CodeAnalysis.AssemblyIdentity = left.Identity
				Dim assemblyIdentity As Microsoft.CodeAnalysis.AssemblyIdentity = right.Identity
				If (AssemblyIdentityComparer.SimpleNameComparer.Equals(identity.Name, assemblyIdentity.Name)) Then
					If ((If(left.AssemblyVersionPattern, identity.Version)).Equals(If(right.AssemblyVersionPattern, assemblyIdentity.Version))) Then
						Return Microsoft.CodeAnalysis.AssemblyIdentity.EqualIgnoringNameAndVersion(identity, assemblyIdentity)
					End If
				End If
				Return False
			End Function

			Private Shared Function SubstituteTypeParameters(ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
				Dim length As Integer = method.TypeParameters.Length
				methodSymbol = If(length <> 0, method.Construct(IndexedTypeParameterSymbol.Take(length).Cast(Of TypeSymbol)()), method)
				Return methodSymbol
			End Function

			Friend Function TryFindAnonymousType(ByVal type As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol, <Out> ByRef otherType As AnonymousTypeValue) As Boolean
				Return Me._anonymousTypeMap.TryGetValue(type.GetAnonymousTypeKey(), otherType)
			End Function

			Friend Function TryGetAnonymousTypeName(ByVal type As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol, <Out> ByRef name As String, <Out> ByRef index As Integer) As Boolean
				Dim flag As Boolean
				Dim anonymousTypeValue As Microsoft.CodeAnalysis.Emit.AnonymousTypeValue = New Microsoft.CodeAnalysis.Emit.AnonymousTypeValue()
				If (Not Me.TryFindAnonymousType(type, anonymousTypeValue)) Then
					name = Nothing
					index = -1
					flag = False
				Else
					name = anonymousTypeValue.Name
					index = anonymousTypeValue.UniqueIndex
					flag = True
				End If
				Return flag
			End Function

			Public Overrides Function Visit(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me._matches.GetOrAdd(symbol, New Func(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Microsoft.CodeAnalysis.VisualBasic.Symbol)(AddressOf Me.Visit))
			End Function

			Public Overrides Function VisitArrayType(ByVal symbol As ArrayTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim symbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = DirectCast(Me.Visit(symbol.ElementType), Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
				If (typeSymbol IsNot Nothing) Then
					Dim customModifiers As ImmutableArray(Of CustomModifier) = Me.VisitCustomModifiers(symbol.CustomModifiers)
					symbol1 = If(Not symbol.IsSZArray, ArrayTypeSymbol.CreateMDArray(typeSymbol, customModifiers, symbol.Rank, symbol.Sizes, symbol.LowerBounds, Me._otherAssembly), ArrayTypeSymbol.CreateSZArray(typeSymbol, customModifiers, Me._otherAssembly))
				Else
					symbol1 = Nothing
				End If
				Return symbol1
			End Function

			Public Overrides Function VisitAssembly(ByVal assembly As AssemblySymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
				If (assembly.IsLinked) Then
					symbol = assembly
				ElseIf (Not VisualBasicSymbolMatcher.MatchSymbols.IdentityEqualIgnoringVersionWildcard(assembly, Me._sourceAssembly)) Then
					Dim modules As ImmutableArray(Of ModuleSymbol) = Me._otherAssembly.Modules
					Dim enumerator As ImmutableArray(Of AssemblySymbol).Enumerator = modules(0).ReferencedAssemblySymbols.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As AssemblySymbol = enumerator.Current
						If (Not VisualBasicSymbolMatcher.MatchSymbols.IdentityEqualIgnoringVersionWildcard(assembly, current)) Then
							Continue While
						End If
						symbol = current
						Return symbol
					End While
					symbol = Nothing
				Else
					symbol = Me._otherAssembly
				End If
				Return symbol
			End Function

			Private Function VisitCustomModifier(ByVal modifier As CustomModifier) As CustomModifier
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(Me.Visit(DirectCast(modifier.Modifier, Symbol)), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (Not modifier.IsOptional) Then
					Return VisualBasicCustomModifier.CreateRequired(namedTypeSymbol)
				End If
				Return VisualBasicCustomModifier.CreateOptional(namedTypeSymbol)
			End Function

			Private Function VisitCustomModifiers(ByVal modifiers As ImmutableArray(Of CustomModifier)) As ImmutableArray(Of CustomModifier)
				Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of CustomModifier, CustomModifier)(modifiers, New Func(Of CustomModifier, CustomModifier)(AddressOf Me.VisitCustomModifier))
			End Function

			Public Overrides Function VisitEvent(ByVal symbol As EventSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me.VisitNamedTypeMember(Of EventSymbol)(symbol, New Func(Of EventSymbol, EventSymbol, Boolean)(AddressOf Me.AreEventsEqual))
			End Function

			Public Overrides Function VisitField(ByVal symbol As FieldSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me.VisitNamedTypeMember(Of FieldSymbol)(symbol, New Func(Of FieldSymbol, FieldSymbol, Boolean)(AddressOf Me.AreFieldsEqual))
			End Function

			Public Overrides Function VisitMethod(ByVal symbol As MethodSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me.VisitNamedTypeMember(Of MethodSymbol)(symbol, New Func(Of MethodSymbol, MethodSymbol, Boolean)(AddressOf Me.AreMethodsEqual))
			End Function

			Public Overrides Function VisitModule(ByVal [module] As Microsoft.CodeAnalysis.VisualBasic.Symbols.ModuleSymbol) As Symbol
				Dim item As Symbol
				Dim assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = DirectCast(Me.Visit([module].ContainingAssembly), Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)
				If (assemblySymbol Is Nothing) Then
					item = Nothing
				ElseIf ([module].Ordinal <> 0) Then
					Dim length As Integer = assemblySymbol.Modules.Length - 1
					Dim num As Integer = 1
					While num <= length
						Dim moduleSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ModuleSymbol = assemblySymbol.Modules(num)
						If (Not StringComparer.Ordinal.Equals(moduleSymbol.Name, [module].Name)) Then
							num = num + 1
						Else
							item = moduleSymbol
							Return item
						End If
					End While
					item = Nothing
				Else
					item = assemblySymbol.Modules(0)
				End If
				Return item
			End Function

			Public Overrides Function VisitNamedType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim internalSymbol As Object
				Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = type.OriginalDefinition
				If (CObj(originalDefinition) <> CObj(type)) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(Me.Visit(originalDefinition), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					If (namedTypeSymbol IsNot Nothing) Then
						Dim allTypeParameters As ImmutableArray(Of TypeParameterSymbol) = namedTypeSymbol.GetAllTypeParameters()
						Dim flag As Boolean = False
						Dim typeWithModifiers As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers, VisualBasicSymbolMatcher.MatchSymbols, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers)(type.GetAllTypeArgumentsWithModifiers(), Function(t As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers, v As VisualBasicSymbolMatcher.MatchSymbols)
							Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = DirectCast(v.Visit(t.Type), Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
							If (typeSymbol Is Nothing) Then
								flag = True
								typeSymbol = t.Type
							End If
							Return New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers(typeSymbol, v.VisitCustomModifiers(t.CustomModifiers))
						End Function, Me)
						If (Not flag) Then
							Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(namedTypeSymbol, allTypeParameters, typeWithModifiers, False)
							symbol = namedTypeSymbol.Construct(typeSubstitution)
						Else
							symbol = Nothing
						End If
					Else
						symbol = Nothing
					End If
				ElseIf (Not type.IsTupleType) Then
					Dim symbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me.Visit(type.ContainingSymbol)
					If (symbol1 IsNot Nothing) Then
						Dim kind As SymbolKind = symbol1.Kind
						If (kind = SymbolKind.NamedType) Then
							symbol = Me.FindMatchingMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(symbol1, type, New Func(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Boolean)(AddressOf Me.AreNamedTypesEqual))
						Else
							If (kind <> SymbolKind.[Namespace]) Then
								Throw ExceptionUtilities.UnexpectedValue(symbol1.Kind)
							End If
							Dim anonymousTypeOrDelegateTemplateSymbol As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol = TryCast(type, AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol)
							If (anonymousTypeOrDelegateTemplateSymbol IsNot Nothing) Then
								Dim anonymousTypeValue As Microsoft.CodeAnalysis.Emit.AnonymousTypeValue = New Microsoft.CodeAnalysis.Emit.AnonymousTypeValue()
								Me.TryFindAnonymousType(anonymousTypeOrDelegateTemplateSymbol, anonymousTypeValue)
								Dim typeDefinition As ITypeDefinition = anonymousTypeValue.Type
								If (typeDefinition IsNot Nothing) Then
									internalSymbol = typeDefinition.GetInternalSymbol()
								Else
									internalSymbol = Nothing
								End If
								symbol = DirectCast(internalSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
							ElseIf (Not type.IsAnonymousType) Then
								symbol = Me.FindMatchingMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(symbol1, type, New Func(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Boolean)(AddressOf Me.AreNamedTypesEqual))
							Else
								symbol = Me.Visit(AnonymousTypeManager.TranslateAnonymousTypeSymbol(type))
							End If
						End If
					Else
						symbol = Nothing
					End If
				Else
					Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(Me.Visit(type.TupleUnderlyingType), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					If (namedTypeSymbol1 Is Nothing OrElse Not namedTypeSymbol1.IsTupleOrCompatibleWithTupleOfCardinality(type.TupleElementTypes.Length)) Then
						symbol = Nothing
					Else
						symbol = namedTypeSymbol1
					End If
				End If
				Return symbol
			End Function

			Private Function VisitNamedTypeMember(Of T As Microsoft.CodeAnalysis.VisualBasic.Symbol)(ByVal member As T, ByVal predicate As Func(Of T, T, Boolean)) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(Me.Visit(member.ContainingType), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (namedTypeSymbol IsNot Nothing) Then
					symbol = DirectCast(Me.FindMatchingMember(Of T)(namedTypeSymbol, member, predicate), Microsoft.CodeAnalysis.VisualBasic.Symbol)
				Else
					symbol = Nothing
				End If
				Return symbol
			End Function

			Public Overrides Function VisitNamespace(ByVal [namespace] As NamespaceSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim globalNamespace As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me.Visit([namespace].ContainingSymbol)
				Dim kind As SymbolKind = symbol.Kind
				If (kind = SymbolKind.NetModule) Then
					globalNamespace = DirectCast(symbol, ModuleSymbol).GlobalNamespace
				Else
					If (kind <> SymbolKind.[Namespace]) Then
						Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
					End If
					globalNamespace = Me.FindMatchingMember(Of NamespaceSymbol)(symbol, [namespace], New Func(Of NamespaceSymbol, NamespaceSymbol, Boolean)(AddressOf Me.AreNamespacesEqual))
				End If
				Return globalNamespace
			End Function

			Public Overrides Function VisitParameter(ByVal parameter As ParameterSymbol) As Symbol
				Throw ExceptionUtilities.Unreachable
			End Function

			Public Overrides Function VisitProperty(ByVal symbol As PropertySymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me.VisitNamedTypeMember(Of PropertySymbol)(symbol, New Func(Of PropertySymbol, PropertySymbol, Boolean)(AddressOf Me.ArePropertiesEqual))
			End Function

			Public Overrides Function VisitTypeParameter(ByVal symbol As TypeParameterSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim symbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Dim typeParameters As ImmutableArray(Of TypeParameterSymbol)
				Dim kind As SymbolKind
				Dim indexedTypeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.IndexedTypeParameterSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.IndexedTypeParameterSymbol)
				If (indexedTypeParameterSymbol Is Nothing) Then
					symbol1 = Me.Visit(symbol.ContainingSymbol)
					kind = symbol1.Kind
					If (kind <> SymbolKind.ErrorType) Then
						If (kind <> SymbolKind.Method) Then
							GoTo Label1
						End If
						typeParameters = DirectCast(symbol1, MethodSymbol).TypeParameters
						GoTo Label0
					End If
				Label2:
					typeParameters = DirectCast(symbol1, NamedTypeSymbol).TypeParameters
				Label0:
					item = typeParameters(symbol.Ordinal)
				Else
					item = indexedTypeParameterSymbol
				End If
				Return item
			Label1:
				If (kind <> SymbolKind.NamedType) Then
					Throw ExceptionUtilities.UnexpectedValue(symbol1.Kind)
				Else
					GoTo Label2
				End If
			End Function

			Private Class SymbolComparer
				Private ReadOnly _matcher As VisualBasicSymbolMatcher.MatchSymbols

				Private ReadOnly _deepTranslatorOpt As VisualBasicSymbolMatcher.DeepTranslator

				Public Sub New(ByVal matcher As VisualBasicSymbolMatcher.MatchSymbols, ByVal deepTranslatorOpt As VisualBasicSymbolMatcher.DeepTranslator)
					MyBase.New()
					Me._matcher = matcher
					Me._deepTranslatorOpt = deepTranslatorOpt
				End Sub

				Public Function Equals(ByVal source As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal other As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As Boolean
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = DirectCast(Me._matcher.Visit(source), Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
					Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = If(Me._deepTranslatorOpt IsNot Nothing, DirectCast(Me._deepTranslatorOpt.Visit(other), Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol), other)
					If (typeSymbol Is Nothing OrElse typeSymbol1 Is Nothing) Then
						Return False
					End If
					Return typeSymbol.IsSameType(typeSymbol1, TypeCompareKind.IgnoreTupleNames)
				End Function
			End Class
		End Class
	End Class
End Namespace