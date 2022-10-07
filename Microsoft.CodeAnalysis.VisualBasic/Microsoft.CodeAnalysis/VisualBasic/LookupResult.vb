Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class LookupResult
		Private _kind As LookupResultKind

		Private ReadOnly _symList As ArrayBuilder(Of Symbol)

		Private _diagInfo As DiagnosticInfo

		Private ReadOnly _pool As ObjectPool(Of LookupResult)

		Private ReadOnly Shared s_poolInstance As ObjectPool(Of LookupResult)

		Private ReadOnly Shared s_ambiguousInTypeError As Func(Of ImmutableArray(Of Symbol), AmbiguousSymbolDiagnostic)

		Private ReadOnly Shared s_ambiguousInNSError As Func(Of ImmutableArray(Of Symbol), AmbiguousSymbolDiagnostic)

		Public ReadOnly Property Diagnostic As DiagnosticInfo
			Get
				Return Me._diagInfo
			End Get
		End Property

		Public ReadOnly Property HasDiagnostic As Boolean
			Get
				Return Me._diagInfo IsNot Nothing
			End Get
		End Property

		Public ReadOnly Property HasSingleSymbol As Boolean
			Get
				Return Me._symList.Count = 1
			End Get
		End Property

		Public ReadOnly Property HasSymbol As Boolean
			Get
				Return Me._symList.Count > 0
			End Get
		End Property

		Public ReadOnly Property IsAmbiguous As Boolean
			Get
				Return Me._kind = LookupResultKind.Ambiguous
			End Get
		End Property

		Public ReadOnly Property IsClear As Boolean
			Get
				If (Me._kind <> LookupResultKind.Empty OrElse Me._symList.Count <> 0) Then
					Return False
				End If
				Return Me._diagInfo Is Nothing
			End Get
		End Property

		Public ReadOnly Property IsGood As Boolean
			Get
				Return Me._kind = LookupResultKind.Good
			End Get
		End Property

		Public ReadOnly Property IsGoodOrAmbiguous As Boolean
			Get
				If (Me._kind = LookupResultKind.Good) Then
					Return True
				End If
				Return Me._kind = LookupResultKind.Ambiguous
			End Get
		End Property

		Public ReadOnly Property IsWrongArity As Boolean
			Get
				If (Me._kind = LookupResultKind.WrongArity) Then
					Return True
				End If
				Return Me._kind = LookupResultKind.WrongArityAndStopLookup
			End Get
		End Property

		Public ReadOnly Property Kind As LookupResultKind
			Get
				Return Me._kind
			End Get
		End Property

		Public ReadOnly Property SingleSymbol As Symbol
			Get
				Return Me._symList(0)
			End Get
		End Property

		Public ReadOnly Property StopFurtherLookup As Boolean
			Get
				Return Me._kind >= LookupResultKind.EmptyAndStopLookup
			End Get
		End Property

		Public ReadOnly Property Symbols As ArrayBuilder(Of Symbol)
			Get
				Return Me._symList
			End Get
		End Property

		Shared Sub New()
			LookupResult.s_poolInstance = LookupResult.CreatePool()
			LookupResult.s_ambiguousInTypeError = Function(syms As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))
				Dim name As String = syms(0).Name
				Dim symbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = syms(0).ContainingSymbol
				Return New Microsoft.CodeAnalysis.VisualBasic.AmbiguousSymbolDiagnostic(ERRID.ERR_MetadataMembersAmbiguous3, syms, New [Object]() { name, symbol1.GetKindText(), symbol1 })
			End Function
			LookupResult.s_ambiguousInNSError = Function(syms As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))
				Dim ambiguousSymbolDiagnostic As Microsoft.CodeAnalysis.VisualBasic.AmbiguousSymbolDiagnostic
				Dim containingSymbol As Func(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Microsoft.CodeAnalysis.VisualBasic.Symbol)
				Dim displayString As Func(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, String)
				Dim key As Func(Of IGrouping(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbol), String)
				Dim func As Func(Of IGrouping(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbol), Microsoft.CodeAnalysis.VisualBasic.Symbol)
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = syms(0).ContainingSymbol
				If (symbol.Name.Length <= 0) Then
					ambiguousSymbolDiagnostic = New Microsoft.CodeAnalysis.VisualBasic.AmbiguousSymbolDiagnostic(ERRID.ERR_AmbiguousInUnnamedNamespace1, syms, New [Object]() { syms(0).Name })
				Else
					Dim symbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = syms
					If (LookupResult._Closure$__.$I0-2 Is Nothing) Then
						containingSymbol = Function(sym As Microsoft.CodeAnalysis.VisualBasic.Symbol) sym.ContainingSymbol
						LookupResult._Closure$__.$I0-2 = containingSymbol
					Else
						containingSymbol = LookupResult._Closure$__.$I0-2
					End If
					Dim symbols1 As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = symbols.[Select](Of Microsoft.CodeAnalysis.VisualBasic.Symbol)(containingSymbol)
					If (LookupResult._Closure$__.$I0-3 Is Nothing) Then
						displayString = Function(c As Microsoft.CodeAnalysis.VisualBasic.Symbol) c.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat)
						LookupResult._Closure$__.$I0-3 = displayString
					Else
						displayString = LookupResult._Closure$__.$I0-3
					End If
					Dim groupings As IEnumerable(Of IGrouping(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbol)) = symbols1.GroupBy(Of String)(displayString, CaseInsensitiveComparison.Comparer)
					If (LookupResult._Closure$__.$I0-4 Is Nothing) Then
						key = Function(group As IGrouping(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbol)) group.Key
						LookupResult._Closure$__.$I0-4 = key
					Else
						key = LookupResult._Closure$__.$I0-4
					End If
					Dim groupings1 As IOrderedEnumerable(Of IGrouping(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbol)) = groupings.OrderBy(Of String)(key, CaseInsensitiveComparison.Comparer)
					If (LookupResult._Closure$__.$I0-5 Is Nothing) Then
						func = Function(group As IGrouping(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbol)) group.First()
						LookupResult._Closure$__.$I0-5 = func
					Else
						func = LookupResult._Closure$__.$I0-5
					End If
					Dim symbols2 As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = groupings1.[Select](Of Microsoft.CodeAnalysis.VisualBasic.Symbol)(func)
					ambiguousSymbolDiagnostic = If(Not symbols2.Skip(1).Any(), New Microsoft.CodeAnalysis.VisualBasic.AmbiguousSymbolDiagnostic(ERRID.ERR_AmbiguousInNamespace2, syms, New [Object]() { syms(0).Name, symbol }), New Microsoft.CodeAnalysis.VisualBasic.AmbiguousSymbolDiagnostic(ERRID.ERR_AmbiguousInNamespaces2, syms, New [Object]() { syms(0).Name, New FormattedSymbolList(symbols2, Nothing) }))
				End If
				Return ambiguousSymbolDiagnostic
			End Function
		End Sub

		Private Sub New(ByVal pool As ObjectPool(Of LookupResult))
			MyClass.New()
			Me._pool = pool
		End Sub

		Friend Sub New()
			MyBase.New()
			Me._kind = LookupResultKind.Empty
			Me._symList = New ArrayBuilder(Of Symbol)()
			Me._diagInfo = Nothing
		End Sub

		Public Function AllSymbolsHaveOverloads() As Boolean
			Dim flag As Boolean
			Dim enumerator As ArrayBuilder(Of Symbol).Enumerator = Me.Symbols.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Symbol = enumerator.Current
					Dim kind As SymbolKind = current.Kind
					If (kind <> SymbolKind.Method) Then
						If (kind = SymbolKind.[Property]) Then
							If (Not DirectCast(current, PropertySymbol).IsOverloads) Then
								flag = False
								Exit While
							End If
						End If
					ElseIf (Not DirectCast(current, MethodSymbol).IsOverloads) Then
						flag = False
						Exit While
					End If
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Shared Function AreEquivalentEnumConstants(ByVal symbol1 As Symbol, ByVal symbol2 As Symbol) As Boolean
			Dim flag As Boolean
			If (symbol1.Kind <> SymbolKind.Field OrElse symbol2.Kind <> SymbolKind.Field OrElse symbol1.ContainingType.TypeKind <> TypeKind.[Enum]) Then
				flag = False
			Else
				Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = DirectCast(symbol1, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
				Dim fieldSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = DirectCast(symbol2, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
				flag = If(fieldSymbol.ConstantValue Is Nothing, False, fieldSymbol.ConstantValue.Equals(RuntimeHelpers.GetObjectValue(fieldSymbol1.ConstantValue)))
			End If
			Return flag
		End Function

		Public Shared Function CanOverload(ByVal sym1 As Symbol, ByVal sym2 As Symbol) As Boolean
			If (sym1.Kind <> sym2.Kind OrElse Not sym1.IsOverloadable()) Then
				Return False
			End If
			Return sym2.IsOverloadable()
		End Function

		Public Sub Clear()
			Me._kind = LookupResultKind.Empty
			Me._symList.Clear()
			Me._diagInfo = Nothing
		End Sub

		Private Sub CompactSymbols(ByVal lost As Integer)
			If (lost > 0) Then
				Dim count As Integer = Me._symList.Count - 1
				Dim item As Integer = 0
				While item <= count AndAlso Me._symList(item) IsNot Nothing
					item = item + 1
				End While
				Dim num As Integer = Me._symList.Count - lost
				If (num > item) Then
					Dim count1 As Integer = Me._symList.Count - 1
					For i As Integer = item + 1 To count1
						If (Me._symList(i) IsNot Nothing) Then
							Me._symList(item) = Me._symList(i)
							item = item + 1
							If (item = num) Then
								Exit For
							End If
						End If
					Next

				End If
				Me._symList.Clip(num)
			End If
		End Sub

		Public Shared Function CompareAccessibilityOfSymbolsConflictingInSameContainer(ByVal first As Symbol, ByVal second As Symbol) As Integer
			Dim num As Integer
			If (first.DeclaredAccessibility = second.DeclaredAccessibility) Then
				num = 0
			ElseIf (first.DeclaredAccessibility >= second.DeclaredAccessibility) Then
				num = If(second.DeclaredAccessibility <> Accessibility.[Protected] OrElse first.DeclaredAccessibility <> Accessibility.Internal, 1, -1)
			Else
				num = If(first.DeclaredAccessibility <> Accessibility.[Protected] OrElse second.DeclaredAccessibility <> Accessibility.Internal, -1, 1)
			End If
			Return num
		End Function

		Private Shared Function CreatePool() As ObjectPool(Of LookupResult)
			Dim objectPool As ObjectPool(Of LookupResult) = Nothing
			objectPool = New ObjectPool(Of LookupResult)(Function() New LookupResult(Me.$VB$Local_pool), 128)
			Return objectPool
		End Function

		Public Sub Free()
			Me.Clear()
			If (Me._pool IsNot Nothing) Then
				Me._pool.Free(Me)
			End If
		End Sub

		Public Shared Function GetInstance() As LookupResult
			Return LookupResult.s_poolInstance.Allocate()
		End Function

		Private Shared Function GetSymbolLocation(ByVal sym As Symbol, ByVal sourceModule As ModuleSymbol, ByVal options As LookupOptions) As LookupResult.SymbolLocation
			Dim symbolLocation As LookupResult.SymbolLocation
			If (sym.Kind = SymbolKind.[Namespace]) Then
				symbolLocation = If(DirectCast(sym, NamespaceSymbol).IsDeclaredInSourceModule(sourceModule), LookupResult.SymbolLocation.FromSourceModule, LookupResult.SymbolLocation.FromReferencedAssembly)
			ElseIf (CObj(sym.ContainingModule) <> CObj(sourceModule)) Then
				If (sourceModule.DeclaringCompilation.Options.IgnoreCorLibraryDuplicatedTypes) Then
					Dim containingAssembly As AssemblySymbol = sym.ContainingAssembly
					If (CObj(containingAssembly) <> CObj(containingAssembly.CorLibrary)) Then
						GoTo Label1
					End If
					symbolLocation = LookupResult.SymbolLocation.FromCorLibrary
					Return symbolLocation
				End If
			Label1:
				symbolLocation = LookupResult.SymbolLocation.FromReferencedAssembly
			Else
				symbolLocation = LookupResult.SymbolLocation.FromSourceModule
			End If
			Return symbolLocation
		End Function

		Public Sub MergeAmbiguous(ByVal other As LookupResult, ByVal generateAmbiguityDiagnostic As Func(Of ImmutableArray(Of Symbol), AmbiguousSymbolDiagnostic))
			If (Not Me.IsGoodOrAmbiguous OrElse Not other.IsGoodOrAmbiguous) Then
				If (other.Kind > Me.Kind) Then
					Me.SetFrom(other)
				End If
				Return
			End If
			Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
			If (Not TypeOf Me.Diagnostic Is AmbiguousSymbolDiagnostic) Then
				instance.AddRange(Me.Symbols)
			Else
				instance.AddRange(DirectCast(Me.Diagnostic, AmbiguousSymbolDiagnostic).AmbiguousSymbols)
			End If
			If (Not TypeOf other.Diagnostic Is AmbiguousSymbolDiagnostic) Then
				instance.AddRange(other.Symbols)
			Else
				instance.AddRange(DirectCast(other.Diagnostic, AmbiguousSymbolDiagnostic).AmbiguousSymbols)
			End If
			Me.SetFrom(instance.ToImmutableAndFree(), generateAmbiguityDiagnostic)
		End Sub

		Public Sub MergeAmbiguous(ByVal other As SingleLookupResult, ByVal generateAmbiguityDiagnostic As Func(Of ImmutableArray(Of Symbol), AmbiguousSymbolDiagnostic))
			If (Not Me.IsGoodOrAmbiguous OrElse Not other.IsGoodOrAmbiguous) Then
				If (other.Kind > Me.Kind) Then
					Me.SetFrom(other)
				End If
				Return
			End If
			Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
			If (Not TypeOf Me.Diagnostic Is AmbiguousSymbolDiagnostic) Then
				instance.AddRange(Me.Symbols)
			Else
				instance.AddRange(DirectCast(Me.Diagnostic, AmbiguousSymbolDiagnostic).AmbiguousSymbols)
			End If
			If (Not TypeOf other.Diagnostic Is AmbiguousSymbolDiagnostic) Then
				instance.Add(other.Symbol)
			Else
				instance.AddRange(DirectCast(other.Diagnostic, AmbiguousSymbolDiagnostic).AmbiguousSymbols)
			End If
			Me.SetFrom(instance.ToImmutableAndFree(), generateAmbiguityDiagnostic)
		End Sub

		Public Sub MergeMembersOfTheSameNamespace(ByVal other As SingleLookupResult, ByVal sourceModule As ModuleSymbol, ByVal options As LookupOptions)
			Dim num As Integer = Me.ResolveAmbiguityInTheSameNamespace(other, sourceModule, options)
			If (num <= 0) Then
				If (num < 0) Then
					Me.SetFrom(other)
					Return
				End If
				Me.MergeAmbiguous(other, LookupResult.s_ambiguousInNSError)
			End If
		End Sub

		Public Sub MergeMembersOfTheSameType(ByVal other As SingleLookupResult, ByVal imported As Boolean)
			Dim declaredAccessibility As Func(Of Symbol, Symbol, Boolean)
			If (Me.IsGoodOrAmbiguous AndAlso other.IsGood) Then
				Me.MergeOverloadedOrAmbiguousInTheSameType(other, imported)
				Return
			End If
			If (other.Kind > Me.Kind) Then
				Me.SetFrom(other)
				Return
			End If
			If (Me.Kind = LookupResultKind.Inaccessible AndAlso Me.Kind <= other.Kind) Then
				If (LookupResult.CanOverload(Me.Symbols(0), other.Symbol)) Then
					Me._symList.Add(other.Symbol)
				Else
					Dim symbols As ArrayBuilder(Of Symbol) = Me.Symbols
					If (LookupResult._Closure$__.$I52-0 Is Nothing) Then
						declaredAccessibility = Function(candidate As Symbol, otherSymbol As Symbol) candidate.DeclaredAccessibility < otherSymbol.DeclaredAccessibility
						LookupResult._Closure$__.$I52-0 = declaredAccessibility
					Else
						declaredAccessibility = LookupResult._Closure$__.$I52-0
					End If
					If (symbols.All(Of Symbol)(declaredAccessibility, other.Symbol)) Then
						Me.SetFrom(other)
						Return
					End If
				End If
			End If
		End Sub

		Private Sub MergeOverloadedOrAmbiguousInTheSameType(ByVal other As SingleLookupResult, ByVal imported As Boolean)
			If (Me.IsGood) Then
				If (LookupResult.CanOverload(Me.Symbols(0), other.Symbol)) Then
					Me._symList.Add(other.Symbol)
					Return
				End If
				If (imported) Then
					Dim num As Integer = 0
					Dim num1 As Integer = 0
					Dim flag As Boolean = False
					Dim count As Integer = Me._symList.Count - 1
					Dim num2 As Integer = 0
					Do
						Dim num3 As Integer = LookupResult.CompareAccessibilityOfSymbolsConflictingInSameContainer(Me._symList(num2), other.Symbol)
						If (num3 = 0) Then
							num1 = num1 + 1
						ElseIf (num3 >= 0) Then
							flag = True
						Else
							num = num + 1
							Me._symList(num2) = Nothing
						End If
						num2 = num2 + 1
					Loop While num2 <= count
					If (num = Me._symList.Count) Then
						Me.SetFrom(other)
						Return
					End If
					If (flag AndAlso num1 > 0) Then
						num += num1
						num1 = Me.RemoveAmbiguousSymbols(other.Symbol, num1)
					End If
					Me.CompactSymbols(num)
					If (flag) Then
						Return
					End If
					If (Me._symList.Count = 1 AndAlso num1 = 1 AndAlso LookupResult.AreEquivalentEnumConstants(Me._symList(0), other.Symbol)) Then
						Return
					End If
				End If
			ElseIf (imported) Then
				Dim num4 As Integer = LookupResult.CompareAccessibilityOfSymbolsConflictingInSameContainer(Me._symList(0), other.Symbol)
				If (num4 < 0) Then
					Me.SetFrom(other)
					Return
				End If
				If (num4 > 0) Then
					Return
				End If
			End If
			Me.MergeAmbiguous(other, LookupResult.s_ambiguousInTypeError)
		End Sub

		Public Sub MergeOverloadedOrPrioritized(ByVal other As LookupResult, ByVal checkIfCurrentHasOverloads As Boolean)
			If (Not Me.IsGoodOrAmbiguous OrElse Not other.IsGoodOrAmbiguous) Then
				If (other.Kind > Me.Kind) Then
					Me.SetFrom(other)
					Return
				End If
				If (Me.Kind = LookupResultKind.Inaccessible AndAlso Me.Kind <= other.Kind AndAlso LookupResult.CanOverload(Me.Symbols(0), other.Symbols(0)) AndAlso (Not checkIfCurrentHasOverloads OrElse Me.AllSymbolsHaveOverloads())) Then
					Me._symList.AddRange(other.Symbols)
				End If
			ElseIf (Me.IsGood AndAlso other.IsGood AndAlso LookupResult.CanOverload(Me.Symbols(0), other.Symbols(0)) AndAlso (Not checkIfCurrentHasOverloads OrElse Me.AllSymbolsHaveOverloads())) Then
				Me._symList.AddRange(other.Symbols)
				Return
			End If
		End Sub

		Public Sub MergeOverloadedOrPrioritized(ByVal other As SingleLookupResult, ByVal checkIfCurrentHasOverloads As Boolean)
			If (Not Me.IsGoodOrAmbiguous OrElse Not other.IsGoodOrAmbiguous) Then
				If (other.Kind > Me.Kind) Then
					Me.SetFrom(other)
					Return
				End If
				If (Me.Kind = LookupResultKind.Inaccessible AndAlso Me.Kind <= other.Kind AndAlso LookupResult.CanOverload(Me.Symbols(0), other.Symbol) AndAlso (Not checkIfCurrentHasOverloads OrElse Me.AllSymbolsHaveOverloads())) Then
					Me._symList.Add(other.Symbol)
				End If
			ElseIf (Me.IsGood AndAlso other.IsGood AndAlso LookupResult.CanOverload(Me.Symbols(0), other.Symbol) AndAlso (Not checkIfCurrentHasOverloads OrElse Me.AllSymbolsHaveOverloads())) Then
				Me._symList.Add(other.Symbol)
				Return
			End If
		End Sub

		Public Sub MergeOverloadedOrPrioritizedExtensionMethods(ByVal other As SingleLookupResult)
			If (Me.IsGood AndAlso other.IsGood) Then
				Me._symList.Add(other.Symbol)
				Return
			End If
			If (other.Kind > Me.Kind) Then
				Me.SetFrom(other)
				Return
			End If
			If (Me.Kind = LookupResultKind.Inaccessible AndAlso Me.Kind <= other.Kind) Then
				Me._symList.Add(other.Symbol)
			End If
		End Sub

		Public Sub MergePrioritized(ByVal other As LookupResult)
			If (other.Kind > Me.Kind AndAlso Me.Kind < LookupResultKind.Ambiguous) Then
				Me.SetFrom(other)
			End If
		End Sub

		Public Sub MergePrioritized(ByVal other As SingleLookupResult)
			If (other.Kind > Me.Kind AndAlso Me.Kind < LookupResultKind.Ambiguous) Then
				Me.SetFrom(other)
			End If
		End Sub

		Private Function RemoveAmbiguousSymbols(ByVal other As Symbol, ByVal ambiguous As Integer) As Integer
			Dim count As Integer = Me._symList.Count - 1
			Dim num As Integer = 0
			Do
				If (Me._symList(num) IsNot Nothing AndAlso Me._symList(num).DeclaredAccessibility = other.DeclaredAccessibility) Then
					Me._symList(num) = Nothing
					ambiguous = ambiguous - 1
					If (ambiguous = 0) Then
						Exit Do
					End If
				End If
				num = num + 1
			Loop While num <= count
			Return ambiguous
		End Function

		Public Sub ReplaceSymbol(ByVal newSym As Symbol)
			Me._symList.Clear()
			Me._symList.Add(newSym)
		End Sub

		Private Shared Function ResolveAmbiguityBetweenTypeAndMergedNamespaceInTheSameNamespace(ByVal possiblyMergedNamespace As NamespaceSymbol, ByVal type As Symbol) As Integer
			Dim num As Integer
			If (type.DeclaredAccessibility < Accessibility.[Public] AndAlso possiblyMergedNamespace.Extent.Kind <> NamespaceKind.[Module]) Then
				Dim members As ImmutableArray(Of Symbol) = DirectCast(type.ContainingSymbol, NamespaceSymbol).GetMembers(type.Name)
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = members.GetEnumerator()
				While enumerator.MoveNext()
					If (enumerator.Current.Kind <> SymbolKind.[Namespace]) Then
						Continue While
					End If
					num = 1
					Return num
				End While
			End If
			num = 0
			Return num
		End Function

		Private Function ResolveAmbiguityInTheSameNamespace(ByVal other As SingleLookupResult, ByVal sourceModule As ModuleSymbol, ByVal options As LookupOptions) As Integer
			Dim num As Integer
			If (other.StopFurtherLookup AndAlso Me.StopFurtherLookup AndAlso Me.Symbols.Count > 0) Then
				Dim symbolLocation As LookupResult.SymbolLocation = LookupResult.GetSymbolLocation(other.Symbol, sourceModule, options)
				Dim symbolLocation1 As LookupResult.SymbolLocation = LookupResult.GetSymbolLocation(Me.Symbols(0), sourceModule, options)
				Dim num1 As Integer = CInt(symbolLocation) - CInt(symbolLocation1)
				If (num1 = 0) Then
					GoTo Label1
				End If
				num = num1
				Return num
			End If
			If (Not other.IsGood) Then
				num = 0
				Return num
			ElseIf (Not Me.IsGood) Then
				If (Not Me.IsAmbiguous) Then
					num = 0
					Return num
				End If
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = DirectCast(Me.Diagnostic, AmbiguousSymbolDiagnostic).AmbiguousSymbols.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					If (current.Kind <> SymbolKind.[Namespace]) Then
						If (LookupResult.ResolveAmbiguityInTheSameNamespace(current, other.Symbol, sourceModule) < 0) Then
							Continue While
						End If
						num = 0
						Return num
					Else
						num = 0
						Return num
					End If
				End While
				num = -1
			Else
				num = LookupResult.ResolveAmbiguityInTheSameNamespace(Me.Symbols(0), other.Symbol, sourceModule)
			End If
			Return num
		End Function

		Private Shared Function ResolveAmbiguityInTheSameNamespace(ByVal first As Symbol, ByVal second As Symbol, ByVal sourceModule As ModuleSymbol) As Integer
			Dim num As Integer
			If (CObj(first.ContainingSymbol) = CObj(second.ContainingSymbol)) Then
				num = If(CObj(first.ContainingModule) <> CObj(sourceModule), LookupResult.CompareAccessibilityOfSymbolsConflictingInSameContainer(first, second), 0)
			ElseIf (first.Kind = SymbolKind.[Namespace]) Then
				num = If(CObj(second.ContainingModule) <> CObj(sourceModule) OrElse first.IsEmbedded <> second.IsEmbedded, LookupResult.ResolveAmbiguityBetweenTypeAndMergedNamespaceInTheSameNamespace(DirectCast(first, NamespaceSymbol), second), 0)
			ElseIf (second.Kind <> SymbolKind.[Namespace]) Then
				num = 0
			Else
				num = If(CObj(first.ContainingModule) <> CObj(sourceModule) OrElse first.IsEmbedded <> second.IsEmbedded, -1 * LookupResult.ResolveAmbiguityBetweenTypeAndMergedNamespaceInTheSameNamespace(DirectCast(second, NamespaceSymbol), first), 0)
			End If
			Return num
		End Function

		Private Sub SetFrom(ByVal kind As LookupResultKind, ByVal sym As Symbol, ByVal diagInfo As DiagnosticInfo)
			Me._kind = kind
			Me._symList.Clear()
			If (sym IsNot Nothing) Then
				Me._symList.Add(sym)
			End If
			Me._diagInfo = diagInfo
		End Sub

		Public Sub SetFrom(ByVal other As SingleLookupResult)
			Me.SetFrom(other.Kind, other.Symbol, other.Diagnostic)
		End Sub

		Public Sub SetFrom(ByVal other As LookupResult)
			Me._kind = other._kind
			Me._symList.Clear()
			Me._symList.AddRange(other._symList)
			Me._diagInfo = other._diagInfo
		End Sub

		Public Sub SetFrom(ByVal s As Symbol)
			Me.SetFrom(SingleLookupResult.Good(s))
		End Sub

		Public Sub SetFrom(ByVal syms As ImmutableArray(Of Symbol), ByVal generateAmbiguityDiagnostic As Func(Of ImmutableArray(Of Symbol), AmbiguousSymbolDiagnostic))
			If (syms.Length = 0) Then
				Me.Clear()
				Return
			End If
			If (syms.Length <= 1) Then
				Me.SetFrom(SingleLookupResult.Good(syms(0)))
				Return
			End If
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = generateAmbiguityDiagnostic(syms)
			Me.SetFrom(LookupResultKind.Ambiguous, syms(0), diagnosticInfo)
		End Sub

		Friend Shared Function WorseResultKind(ByVal resultKind1 As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind, ByVal resultKind2 As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind) As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind
			Dim lookupResultKind As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind
			If (resultKind1 = Microsoft.CodeAnalysis.VisualBasic.LookupResultKind.Empty) Then
				lookupResultKind = If(resultKind2 = Microsoft.CodeAnalysis.VisualBasic.LookupResultKind.Good, Microsoft.CodeAnalysis.VisualBasic.LookupResultKind.Empty, resultKind2)
			ElseIf (resultKind2 <> Microsoft.CodeAnalysis.VisualBasic.LookupResultKind.Empty) Then
				lookupResultKind = If(resultKind1 >= resultKind2, resultKind2, resultKind1)
			Else
				lookupResultKind = If(resultKind1 = Microsoft.CodeAnalysis.VisualBasic.LookupResultKind.Good, Microsoft.CodeAnalysis.VisualBasic.LookupResultKind.Empty, resultKind1)
			End If
			Return lookupResultKind
		End Function

		Private Enum SymbolLocation
			FromSourceModule
			FromReferencedAssembly
			FromCorLibrary
		End Enum
	End Class
End Namespace