Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module SymbolExtensions
		Friend Const NamespaceKindNamespaceGroup As NamespaceKind = 0

		<Extension>
		Friend Function AsMember(Of T As Symbol)(ByVal origMember As T, ByVal type As NamedTypeSymbol) As T
			Dim t1 As T
			t1 = If(CObj(type) <> CObj(origMember.ContainingType), DirectCast(DirectCast(type, SubstitutedNamedType).GetMemberForDefinition(DirectCast(origMember, Symbol)), T), origMember)
			Return t1
		End Function

		<Extension>
		Friend Function ContainingNonLambdaMember(ByVal member As Symbol) As Symbol
			While True
				If (If(member IsNot Nothing, member.Kind <> SymbolKind.Method, True) OrElse DirectCast(member, MethodSymbol).MethodKind <> MethodKind.AnonymousFunction) Then
					Exit While
				End If
				member = member.ContainingSymbol
			End While
			Return member
		End Function

		<Extension>
		Friend Function ContainsTupleNames(ByVal member As Symbol) As Boolean
			Dim flag As Boolean
			Dim kind As SymbolKind = member.Kind
			If (kind = SymbolKind.[Event]) Then
				flag = SymbolExtensions.ContainsTupleNames(DirectCast(member, EventSymbol).DelegateParameters)
			ElseIf (kind = SymbolKind.Method) Then
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(member, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
				flag = If(methodSymbol.ReturnType.ContainsTupleNames(), True, SymbolExtensions.ContainsTupleNames(methodSymbol.Parameters))
			Else
				If (kind <> SymbolKind.[Property]) Then
					Throw ExceptionUtilities.UnexpectedValue(member.Kind)
				End If
				Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(member, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
				flag = If(propertySymbol.Type.ContainsTupleNames(), True, SymbolExtensions.ContainsTupleNames(propertySymbol.Parameters))
			End If
			Return flag
		End Function

		Private Function ContainsTupleNames(ByVal parameters As ImmutableArray(Of ParameterSymbol)) As Boolean
			Dim func As Func(Of ParameterSymbol, Boolean)
			Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = parameters
			If (SymbolExtensions._Closure$__.$I34-0 Is Nothing) Then
				func = Function(p As ParameterSymbol) p.Type.ContainsTupleNames()
				SymbolExtensions._Closure$__.$I34-0 = func
			Else
				func = SymbolExtensions._Closure$__.$I34-0
			End If
			Return parameterSymbols.Any(func)
		End Function

		<Extension>
		Friend Function EnsureVbSymbolOrNothing(Of TSource As ISymbol, TDestination As {Microsoft.CodeAnalysis.VisualBasic.Symbol, TSource})(ByVal symbol As TSource, ByVal paramName As String) As TDestination
			Dim tDestination1 As TDestination = DirectCast(TryCast(symbol, TDestination), TDestination)
			If (tDestination1 Is Nothing AndAlso symbol IsNot Nothing) Then
				Throw New ArgumentException(VBResources.NotAVbSymbol, paramName)
			End If
			Return tDestination1
		End Function

		<Extension>
		Friend Function GetArity(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Integer
			Dim arity As Integer
			Dim kind As SymbolKind = symbol.Kind
			If (kind <> SymbolKind.ErrorType) Then
				If (kind = SymbolKind.Method) Then
					arity = DirectCast(symbol, MethodSymbol).Arity
					Return arity
				Else
					If (kind = SymbolKind.NamedType) Then
						arity = DirectCast(symbol, NamedTypeSymbol).Arity
						Return arity
					End If
					arity = 0
					Return arity
				End If
			End If
			arity = DirectCast(symbol, NamedTypeSymbol).Arity
			Return arity
		End Function

		<Extension>
		Friend Function GetDeclaringSyntaxNode(Of T As VisualBasicSyntaxNode)(ByVal this As Symbol) As T
			Dim t1 As T
			Dim enumerator As IEnumerator(Of SyntaxNode) = Nothing
			Dim syntax As Func(Of SyntaxReference, SyntaxNode)
			Using enumerator
				Dim declaringSyntaxReferences As ImmutableArray(Of SyntaxReference) = this.DeclaringSyntaxReferences
				If (SymbolExtensions._Closure$__29(Of T).$I29-0 Is Nothing) Then
					syntax = Function(d As SyntaxReference) d.GetSyntax(New CancellationToken())
					SymbolExtensions._Closure$__29(Of T).$I29-0 = syntax
				Else
					syntax = SymbolExtensions._Closure$__29(Of T).$I29-0
				End If
				enumerator = declaringSyntaxReferences.[Select](Of SyntaxNode)(syntax).GetEnumerator()
				While enumerator.MoveNext()
					Dim current As T = DirectCast(TryCast(enumerator.Current, T), T)
					If (current Is Nothing) Then
						Continue While
					End If
					t1 = current
					Return t1
				End While
			End Using
			t1 = DirectCast(System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of VisualBasicSyntaxNode)(Symbol.GetDeclaringSyntaxNodeHelper(Of T)(this.Locations)), T)
			Return t1
		End Function

		<Extension>
		Friend Function GetKindText(ByVal target As Symbol) As String
			Dim str As String
			Select Case target.Kind
				Case SymbolKind.[Event]
					str = "event"
					Exit Select
				Case SymbolKind.Field
				Case SymbolKind.Local
				Case SymbolKind.Parameter
				Case SymbolKind.RangeVariable
					str = "variable"
					Exit Select
				Case SymbolKind.Label
				Case SymbolKind.NetModule
				Case SymbolKind.PointerType
					Throw ExceptionUtilities.UnexpectedValue(target.Kind)
				Case SymbolKind.Method
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(target, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
					Dim methodKind As Microsoft.CodeAnalysis.MethodKind = methodSymbol.MethodKind
					If (methodKind = Microsoft.CodeAnalysis.MethodKind.Conversion OrElse methodKind = Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator OrElse methodKind = Microsoft.CodeAnalysis.MethodKind.BuiltinOperator) Then
						str = "operator"
						Exit Select
					ElseIf (Not methodSymbol.IsSub) Then
						str = "function"
						Exit Select
					Else
						str = "sub"
						Exit Select
					End If
				Case SymbolKind.NamedType
					Select Case DirectCast(target, TypeSymbol).TypeKind
						Case TypeKind.[Class]
							str = "class"

						Case TypeKind.[Delegate]
							str = "delegate Class"

						Case TypeKind.Dynamic
						Case TypeKind.[Error]
						Case TypeKind.Pointer
						Label0:
							str = "type"

						Case TypeKind.[Enum]
							str = "enum"

						Case TypeKind.[Interface]
							str = "interface"

						Case TypeKind.[Module]
							str = "module"

						Case TypeKind.Struct
							str = "structure"

						Case Else
							GoTo Label0
					End Select

				Case SymbolKind.[Namespace]
					str = "namespace"
					Exit Select
				Case SymbolKind.[Property]
					If (Not DirectCast(target, PropertySymbol).IsWithEvents) Then
						str = "property"
						Exit Select
					Else
						str = "WithEvents variable"
						Exit Select
					End If
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(target.Kind)
			End Select
			Return str
		End Function

		<Extension>
		Friend Function GetMeParameter(ByVal sym As Symbol) As ParameterSymbol
			Dim meParameter As ParameterSymbol
			Dim kind As SymbolKind = sym.Kind
			If (kind <= SymbolKind.Method) Then
				If (kind = SymbolKind.Field) Then
					meParameter = DirectCast(sym, FieldSymbol).MeParameter
				Else
					If (kind <> SymbolKind.Method) Then
						Throw ExceptionUtilities.UnexpectedValue(sym.Kind)
					End If
					meParameter = DirectCast(sym, MethodSymbol).MeParameter
				End If
			ElseIf (kind = SymbolKind.Parameter) Then
				meParameter = Nothing
			Else
				If (kind <> SymbolKind.[Property]) Then
					Throw ExceptionUtilities.UnexpectedValue(sym.Kind)
				End If
				meParameter = DirectCast(sym, PropertySymbol).MeParameter
			End If
			Return meParameter
		End Function

		<Extension>
		Friend Function GetPropertyKindText(ByVal target As PropertySymbol) As String
			Dim text As String
			If (Not target.IsWriteOnly) Then
				text = If(Not target.IsReadOnly, "", SyntaxFacts.GetText(SyntaxKind.ReadOnlyKeyword))
			Else
				text = SyntaxFacts.GetText(SyntaxKind.WriteOnlyKeyword)
			End If
			Return text
		End Function

		<Extension>
		Friend Function GetUpperLevelNamedTypeSymbol(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = If(symbol.Kind = SymbolKind.NamedType, DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol), symbol.ContainingType)
			If (containingType IsNot Nothing) Then
				While containingType.ContainingType IsNot Nothing
					containingType = containingType.ContainingType
				End While
				namedTypeSymbol = containingType
			Else
				namedTypeSymbol = Nothing
			End If
			Return namedTypeSymbol
		End Function

		<Extension>
		Public Function IsAccessor(ByVal methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) As Boolean
			Return CObj(methodSymbol.AssociatedSymbol) <> CObj(Nothing)
		End Function

		<Extension>
		Public Function IsAccessor(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			If (symbol.Kind <> SymbolKind.Method) Then
				Return False
			End If
			Return DirectCast(symbol, MethodSymbol).IsAccessor()
		End Function

		<Extension>
		Friend Function IsAnyConstructor(ByVal method As MethodSymbol) As Boolean
			Dim methodKind As Microsoft.CodeAnalysis.MethodKind = method.MethodKind
			If (methodKind = Microsoft.CodeAnalysis.MethodKind.Constructor) Then
				Return True
			End If
			Return methodKind = Microsoft.CodeAnalysis.MethodKind.StaticConstructor
		End Function

		<Extension>
		Friend Function IsCompilationOutputWinMdObj(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			Dim declaringCompilation As VisualBasicCompilation = symbol.DeclaringCompilation
			Return declaringCompilation IsNot Nothing And declaringCompilation.Options.OutputKind = OutputKind.WindowsRuntimeMetadata
		End Function

		<Extension>
		Friend Function IsDefaultValueTypeConstructor(ByVal method As MethodSymbol) As Boolean
			If (Not method.IsImplicitlyDeclared OrElse Not method.ContainingType.IsValueType) Then
				Return False
			End If
			Return method.IsParameterlessConstructor()
		End Function

		<Extension>
		Friend Function IsHiddenByCodeAnalysisEmbeddedAttribute(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			Dim upperLevelNamedTypeSymbol As NamedTypeSymbol = symbol.GetUpperLevelNamedTypeSymbol()
			If (upperLevelNamedTypeSymbol Is Nothing) Then
				Return False
			End If
			Return upperLevelNamedTypeSymbol.HasCodeAnalysisEmbeddedAttribute
		End Function

		<Extension>
		Friend Function IsHiddenByVisualBasicEmbeddedAttribute(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			Dim upperLevelNamedTypeSymbol As NamedTypeSymbol = symbol.GetUpperLevelNamedTypeSymbol()
			If (upperLevelNamedTypeSymbol Is Nothing) Then
				Return False
			End If
			Return upperLevelNamedTypeSymbol.HasVisualBasicEmbeddedAttribute
		End Function

		<Extension>
		Friend Function IsInstanceMember(ByVal sym As Symbol) As Boolean
			Dim flag As Boolean
			Dim kind As SymbolKind = sym.Kind
			flag = If(CInt(kind) - CInt(SymbolKind.[Event]) <= CInt(SymbolKind.ArrayType) OrElse kind = SymbolKind.Method OrElse kind = SymbolKind.[Property], Not sym.IsShared, False)
			Return flag
		End Function

		<Extension>
		Friend Function IsMetadataVirtual(ByVal method As MethodSymbol) As Boolean
			Dim flag As Boolean
			If (method.IsOverridable OrElse method.IsOverrides OrElse method.IsMustOverride OrElse Not method.ExplicitInterfaceImplementations.IsEmpty) Then
				flag = True
			Else
				Dim originalDefinition As MethodSymbol = method.OriginalDefinition
				Dim containingSymbol As SourceNamedTypeSymbol = TryCast(originalDefinition.ContainingSymbol, SourceNamedTypeSymbol)
				flag = If(containingSymbol Is Nothing, False, CObj(containingSymbol.GetCorrespondingComClassInterfaceMethod(originalDefinition)) <> CObj(Nothing))
			End If
			Return flag
		End Function

		<Extension>
		Public Function IsOverloadable(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			Dim flag As Boolean
			Dim kind As SymbolKind = symbol.Kind
			If (kind <> SymbolKind.Method) Then
				flag = If(kind = SymbolKind.[Property], DirectCast(symbol, PropertySymbol).IsOverloadable(), False)
			Else
				flag = True
			End If
			Return flag
		End Function

		<Extension>
		Public Function IsOverloadable(ByVal propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol) As Boolean
			Return Not propertySymbol.IsWithEvents
		End Function

		<Extension>
		Friend Function IsOverloads(ByVal sym As Symbol) As Boolean
			Dim flag As Boolean
			Dim kind As SymbolKind = sym.Kind
			If (kind = SymbolKind.Method) Then
				If (Not DirectCast(sym, MethodSymbol).IsOverloads) Then
					flag = False
					Return flag
				End If
				flag = True
				Return flag
			ElseIf (kind = SymbolKind.[Property]) Then
				If (Not DirectCast(sym, PropertySymbol).IsOverloads) Then
					flag = False
					Return flag
				End If
				flag = True
				Return flag
			End If
			flag = False
			Return flag
		End Function

		<Extension>
		Public Function IsPropertyAndNotWithEvents(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			If (symbol.Kind <> SymbolKind.[Property]) Then
				Return False
			End If
			Return Not DirectCast(symbol, PropertySymbol).IsWithEvents
		End Function

		<Extension>
		Friend Function IsReducedExtensionMethod(ByVal this As Symbol) As Boolean
			If (this.Kind <> SymbolKind.Method) Then
				Return False
			End If
			Return DirectCast(this, MethodSymbol).IsReducedExtensionMethod
		End Function

		<Extension>
		Friend Function IsShadows(ByVal sym As Symbol) As Boolean
			Return Not sym.IsOverloads()
		End Function

		<Extension>
		Friend Function IsUserDefinedOperator(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			If (symbol.Kind <> SymbolKind.Method) Then
				Return False
			End If
			Return DirectCast(symbol, MethodSymbol).IsUserDefinedOperator()
		End Function

		<Extension>
		Public Function IsWithEventsProperty(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			If (symbol.Kind <> SymbolKind.[Property]) Then
				Return False
			End If
			Return DirectCast(symbol, PropertySymbol).IsWithEvents
		End Function

		<Extension>
		Friend Function MatchesAnyName(ByVal this As ImmutableArray(Of TypeParameterSymbol), ByVal name As String) As Boolean
			Dim flag As Boolean
			Dim enumerator As ImmutableArray(Of TypeParameterSymbol).Enumerator = this.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As TypeParameterSymbol = enumerator.Current
					If (CaseInsensitiveComparison.Comparer.Compare(name, current.Name) = 0) Then
						flag = True
						Exit While
					End If
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		<Extension>
		Friend Function OfMinimalArity(ByVal symbols As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol
			Dim enumerator As IEnumerator(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol) = Nothing
			Dim namespaceOrTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = Nothing
			Using num As Integer = 2147483647
				enumerator = symbols.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = enumerator.Current
					Dim arity As Integer = current.GetArity()
					If (arity >= num) Then
						Continue While
					End If
					num = arity
					namespaceOrTypeSymbol = current
				End While
			End Using
			Return namespaceOrTypeSymbol
		End Function

		<Extension>
		Friend Function OverriddenMember(ByVal sym As Symbol) As Symbol
			Dim overriddenEvent As Symbol
			Dim kind As SymbolKind = sym.Kind
			If (kind = SymbolKind.[Event]) Then
				overriddenEvent = DirectCast(sym, EventSymbol).OverriddenEvent
			ElseIf (kind = SymbolKind.Method) Then
				overriddenEvent = DirectCast(sym, MethodSymbol).OverriddenMethod
			ElseIf (kind = SymbolKind.[Property]) Then
				overriddenEvent = DirectCast(sym, PropertySymbol).OverriddenProperty
			Else
				overriddenEvent = Nothing
			End If
			Return overriddenEvent
		End Function

		<Extension>
		Friend Function RequiresImplementation(ByVal sym As Symbol) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean
			Dim kind As SymbolKind = sym.Kind
			If (kind = SymbolKind.[Event] OrElse kind = SymbolKind.Method OrElse kind = SymbolKind.[Property]) Then
				If (Not sym.ContainingType.IsInterfaceType() OrElse sym.IsShared OrElse sym.IsNotOverridable) Then
					flag1 = False
				Else
					flag1 = If(sym.IsMustOverride, True, sym.IsOverridable)
				End If
				flag = flag1
			Else
				flag = False
			End If
			Return flag
		End Function

		<Extension>
		Friend Function ToErrorMessageArgument(ByVal target As Symbol, Optional ByVal errorCode As ERRID = 0) As Object
			Dim obj As Object
			If (target.Kind = SymbolKind.[Namespace] AndAlso DirectCast(target, NamespaceSymbol).IsGlobalNamespace) Then
				obj = "<Default>"
			ElseIf (errorCode <> ERRID.ERR_TypeConflict6) Then
				obj = target
			Else
				obj = CustomSymbolDisplayFormatter.DefaultErrorFormat(target)
			End If
			Return obj
		End Function

		<Extension>
		Friend Function UnwrapAlias(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim aliasSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AliasSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.AliasSymbol)
			If (aliasSymbol Is Nothing) Then
				Return symbol
			End If
			Return aliasSymbol.Target
		End Function
	End Module
End Namespace