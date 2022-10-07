Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.RuntimeMembers
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class TupleTypeSymbol
		Inherits WrappedNamedTypeSymbol
		Private ReadOnly _locations As ImmutableArray(Of Location)

		Private ReadOnly _elementLocations As ImmutableArray(Of Location)

		Private ReadOnly _providedElementNames As ImmutableArray(Of String)

		Private ReadOnly _errorPositions As ImmutableArray(Of Boolean)

		Private _lazyActualElementNames As ImmutableArray(Of String)

		Private ReadOnly _elementTypes As ImmutableArray(Of TypeSymbol)

		Private _lazyMembers As ImmutableArray(Of Symbol)

		Private _lazyFields As ImmutableArray(Of FieldSymbol)

		Private _lazyUnderlyingDefinitionToMemberMap As SmallDictionary(Of Symbol, Symbol)

		Friend Const RestPosition As Integer = 8

		Friend Const RestIndex As Integer = 7

		Friend Const TupleTypeName As String = "ValueTuple"

		Friend Const RestFieldName As String = "Rest"

		Private ReadOnly Shared tupleTypes As WellKnownType()

		Private ReadOnly Shared tupleCtors As WellKnownMember()

		Private ReadOnly Shared tupleMembers As WellKnownMember()()

		Private ReadOnly Shared ForbiddenNames As HashSet(Of String)

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return 0
			End Get
		End Property

		Friend Overrides ReadOnly Property CanConstruct As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property ConstructedFrom As NamedTypeSymbol
			Get
				Return Me
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._underlyingType.ContainingSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Dim accessibility As Microsoft.CodeAnalysis.Accessibility
				accessibility = If(Not Me._underlyingType.IsErrorType(), Me._underlyingType.DeclaredAccessibility, Microsoft.CodeAnalysis.Accessibility.[Public])
				Return accessibility
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Symbol.GetDeclaringSyntaxReferenceHelper(Of VisualBasicSyntaxNode)(Me._locations)
			End Get
		End Property

		Public Overrides ReadOnly Property EnumUnderlyingType As NamedTypeSymbol
			Get
				Return Me._underlyingType.EnumUnderlyingType
			End Get
		End Property

		Friend Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Return Me._underlyingType.HasDeclarativeSecurity
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasTypeArgumentsCustomModifiers As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsComImport As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExtensibleInterfaceNoUseSiteDiagnostics As Boolean
			Get
				Return Me._underlyingType.IsExtensibleInterfaceNoUseSiteDiagnostics
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsInterface As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustInherit As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotInheritable As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsSerializable As Boolean
			Get
				Return Me._underlyingType.IsSerializable
			End Get
		End Property

		Public Overrides ReadOnly Property IsTupleType As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property IsWindowsRuntimeImport As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Kind As SymbolKind
			Get
				Return SymbolKind.NamedType
			End Get
		End Property

		Friend Overrides ReadOnly Property Layout As TypeLayout
			Get
				Return Me._underlyingType.Layout
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MangleName As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingCharSet As CharSet
			Get
				Return Me._underlyingType.MarshallingCharSet
			End Get
		End Property

		Public Overrides ReadOnly Property MemberNames As IEnumerable(Of String)
			Get
				Return New TupleTypeSymbol.VB$StateMachine_67_get_MemberNames(-2) With
				{
					.$VB$Me = Me
				}
			End Get
		End Property

		Public Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return [String].Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property ShouldAddWinRTMembers As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property TupleElementNames As ImmutableArray(Of String)
			Get
				Dim strs As ImmutableArray(Of String)
				Dim isImplicitlyDeclared As Func(Of FieldSymbol, String)
				If (Not Me._providedElementNames.IsDefault) Then
					If (Me._lazyActualElementNames.IsDefault) Then
						Dim tupleElements As ImmutableArray(Of FieldSymbol) = Me.TupleElements
						If (TupleTypeSymbol._Closure$__.$I24-0 Is Nothing) Then
							isImplicitlyDeclared = Function(e As FieldSymbol)
								If (e.IsImplicitlyDeclared) Then
									Return Nothing
								End If
								Return e.Name
							End Function
							TupleTypeSymbol._Closure$__.$I24-0 = isImplicitlyDeclared
						Else
							isImplicitlyDeclared = TupleTypeSymbol._Closure$__.$I24-0
						End If
						Me._lazyActualElementNames = Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of FieldSymbol, String)(tupleElements, isImplicitlyDeclared)
					End If
					strs = Me._lazyActualElementNames
				Else
					strs = New ImmutableArray(Of String)()
				End If
				Return strs
			End Get
		End Property

		Public Overrides ReadOnly Property TupleElements As ImmutableArray(Of FieldSymbol)
			Get
				If (Me._lazyFields.IsDefault) Then
					ImmutableInterlocked.InterlockedInitialize(Of FieldSymbol)(Me._lazyFields, Me.CollectTupleElementFields())
				End If
				Return Me._lazyFields
			End Get
		End Property

		Public Overrides ReadOnly Property TupleElementTypes As ImmutableArray(Of TypeSymbol)
			Get
				Return Me._elementTypes
			End Get
		End Property

		Public Overrides ReadOnly Property TupleUnderlyingType As NamedTypeSymbol
			Get
				Return Me._underlyingType
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
			Get
				Return ImmutableArray(Of TypeSymbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
			Get
				Return Microsoft.CodeAnalysis.TypeKind.Struct
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return ImmutableArray(Of TypeParameterSymbol).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Get
				Return Nothing
			End Get
		End Property

		Friend ReadOnly Property UnderlyingDefinitionToMemberMap As SmallDictionary(Of Symbol, Symbol)
			Get
				If (Me._lazyUnderlyingDefinitionToMemberMap Is Nothing) Then
					Me._lazyUnderlyingDefinitionToMemberMap = Me.ComputeDefinitionToMemberMap()
				End If
				Return Me._lazyUnderlyingDefinitionToMemberMap
			End Get
		End Property

		Shared Sub New()
			TupleTypeSymbol.tupleTypes = New WellKnownType() { GetType(<PrivateImplementationDetails>).GetField("CAB049684DD1612D446A5591EAD33BEDAEEF9F5B64FE1DE56D387EE523A61425").FieldHandle }
			TupleTypeSymbol.tupleCtors = New WellKnownMember() { GetType(<PrivateImplementationDetails>).GetField("D8A34CE7CF47D91AF9056F6990EA251406D822614AE75ADEC0CA0BE1DC9F2023").FieldHandle }
			TupleTypeSymbol.tupleMembers = New WellKnownMember()() { New WellKnownMember() { WellKnownMember.System_ValueTuple_T1__Item1 }, New WellKnownMember() { WellKnownMember.System_ValueTuple_T2__Item1, WellKnownMember.System_ValueTuple_T2__Item2 }, New WellKnownMember() { GetType(<PrivateImplementationDetails>).GetField("08ADCADF6DCE3F6281D5E007A4DF6DDCA4FCE7241034AF89F28B8FE14224549D").FieldHandle }, New WellKnownMember() { GetType(<PrivateImplementationDetails>).GetField("C9915955BA92C9378EC9B7A75B437011D5D1D1B96C6D86BAD077807136064D4A").FieldHandle }, New WellKnownMember() { GetType(<PrivateImplementationDetails>).GetField("DF8909A4F9819F27A0079B01F126536C6D220BE7A93C66329D4438D57C6F1507").FieldHandle }, New WellKnownMember() { GetType(<PrivateImplementationDetails>).GetField("0D3A26FF1CE6ACDE3D2F019DDD502055F8FFAEEC8E1E5B1E7D57755ED0AFB373").FieldHandle }, New WellKnownMember() { GetType(<PrivateImplementationDetails>).GetField("0FC792481E95868661B2909BDD719ED4DF342A616D14AEA6FF06C3CB92ED2F71").FieldHandle }, New WellKnownMember() { GetType(<PrivateImplementationDetails>).GetField("CBD5E9B7AB36EAC10AEE648DCC4F989E16E79A1447A7ABEFCF54A952685FD959").FieldHandle } }
			TupleTypeSymbol.ForbiddenNames = New HashSet(Of String)(New [String]() { "CompareTo", "Deconstruct", "Equals", "GetHashCode", "Rest", "ToString" }, CaseInsensitiveComparison.Comparer)
		End Sub

		Private Sub New(ByVal locationOpt As Location, ByVal underlyingType As NamedTypeSymbol, ByVal elementLocations As ImmutableArray(Of Location), ByVal elementNames As ImmutableArray(Of String), ByVal elementTypes As ImmutableArray(Of TypeSymbol), ByVal errorPositions As ImmutableArray(Of Boolean))
			MyClass.New(If(locationOpt Is Nothing, ImmutableArray(Of Location).Empty, ImmutableArray.Create(Of Location)(locationOpt)), underlyingType, elementLocations, elementNames, elementTypes, errorPositions)
		End Sub

		Private Sub New(ByVal locations As ImmutableArray(Of Location), ByVal underlyingType As NamedTypeSymbol, ByVal elementLocations As ImmutableArray(Of Location), ByVal elementNames As ImmutableArray(Of String), ByVal elementTypes As ImmutableArray(Of TypeSymbol), ByVal errorPositions As ImmutableArray(Of Boolean))
			MyBase.New(underlyingType)
			Me._elementLocations = elementLocations
			Me._providedElementNames = elementNames
			Me._elementTypes = elementTypes
			Me._locations = locations
			Me._errorPositions = errorPositions
		End Sub

		Friend Shared Sub AddElementTypes(ByVal underlyingTupleType As NamedTypeSymbol, ByVal tupleElementTypes As ArrayBuilder(Of TypeSymbol))
			Dim item As NamedTypeSymbol = underlyingTupleType
			While True
				If (item.IsTupleType) Then
					tupleElementTypes.AddRange(item.TupleElementTypes)
					Exit While
				Else
					Dim num As Integer = Math.Min(item.Arity, 7)
					tupleElementTypes.AddRange(item.TypeArgumentsNoUseSiteDiagnostics, num)
					If (item.Arity <> 8) Then
						Exit While
					End If
					item = DirectCast(item.TypeArgumentsNoUseSiteDiagnostics(7), NamedTypeSymbol)
				End If
			End While
		End Sub

		Private Shared Sub CollectTargetTupleFields(ByVal underlying As NamedTypeSymbol, ByVal fieldsForElements As ArrayBuilder(Of FieldSymbol))
			underlying = underlying.OriginalDefinition
			Dim num As Integer = Math.Min(underlying.Arity, 7) - 1
			For i As Integer = 0 To num
				Dim tupleTypeMember As WellKnownMember = TupleTypeSymbol.GetTupleTypeMember(underlying.Arity, i + 1)
				fieldsForElements.Add(DirectCast(TupleTypeSymbol.GetWellKnownMemberInType(underlying, tupleTypeMember), FieldSymbol))
			Next

		End Sub

		Private Function CollectTupleElementFields() As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol).GetInstance(Me._elementTypes.Length, Nothing)
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = Me.GetMembers().GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Symbol = enumerator.Current
				If (current.Kind <> SymbolKind.Field) Then
					Continue While
				End If
				Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
				Dim tupleElementIndex As Integer = fieldSymbol.TupleElementIndex
				If (tupleElementIndex < 0 OrElse instance(tupleElementIndex) IsNot Nothing AndAlso Not instance(tupleElementIndex).IsDefaultTupleElement) Then
					Continue While
				End If
				instance(tupleElementIndex) = fieldSymbol
			End While
			Return instance.ToImmutableAndFree()
		End Function

		Private Function ComputeDefinitionToMemberMap() As SmallDictionary(Of Symbol, Symbol)
			Dim item As Symbol
			Dim symbols As SmallDictionary(Of Symbol, Symbol) = New SmallDictionary(Of Symbol, Symbol)(ReferenceEqualityComparer.Instance)
			Dim originalDefinition As NamedTypeSymbol = Me._underlyingType.OriginalDefinition
			Dim members As ImmutableArray(Of Symbol) = Me.GetMembers()
			Dim length As Integer = members.Length - 1
			While True
				If (length < 0) Then
					Return symbols
				End If
				item = members(length)
				Dim kind As SymbolKind = item.Kind
				Select Case kind
					Case SymbolKind.[Event]
						Dim tupleUnderlyingEvent As EventSymbol = DirectCast(item, EventSymbol).TupleUnderlyingEvent
						Dim associatedField As FieldSymbol = tupleUnderlyingEvent.AssociatedField
						If (associatedField IsNot Nothing) Then
							symbols.Add(associatedField.OriginalDefinition, New TupleFieldSymbol(Me, associatedField, -length - 1))
						End If
						symbols.Add(tupleUnderlyingEvent.OriginalDefinition, item)
						Exit Select
					Case SymbolKind.Field
						Dim tupleUnderlyingField As FieldSymbol = DirectCast(item, FieldSymbol).TupleUnderlyingField
						If (tupleUnderlyingField Is Nothing) Then
							Exit Select
						End If
						symbols(tupleUnderlyingField.OriginalDefinition) = item
						Exit Select
					Case SymbolKind.Label
					Case SymbolKind.Local
						Throw ExceptionUtilities.UnexpectedValue(item.Kind)
					Case SymbolKind.Method
						symbols.Add(DirectCast(item, MethodSymbol).TupleUnderlyingMethod.OriginalDefinition, item)
						Exit Select
					Case Else
						If (kind = SymbolKind.[Property]) Then
							symbols.Add(DirectCast(item, PropertySymbol).TupleUnderlyingProperty.OriginalDefinition, item)
							Exit Select
						Else
							Throw ExceptionUtilities.UnexpectedValue(item.Kind)
						End If
				End Select
				length = length - 1
			End While
			Throw ExceptionUtilities.UnexpectedValue(item.Kind)
		End Function

		Public Overrides Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As NamedTypeSymbol
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Shared Function Create(ByVal locationOpt As Location, ByVal elementTypes As ImmutableArray(Of TypeSymbol), ByVal elementLocations As ImmutableArray(Of Location), ByVal elementNames As ImmutableArray(Of String), ByVal compilation As VisualBasicCompilation, ByVal shouldCheckConstraints As Boolean, ByVal errorPositions As ImmutableArray(Of Boolean), Optional ByVal syntax As SyntaxNode = Nothing, Optional ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol
			Dim diagnosticBag As Boolean
			If (elementTypes.Length <= 1) Then
				Throw ExceptionUtilities.Unreachable
			End If
			Dim tupleUnderlyingType As NamedTypeSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol.GetTupleUnderlyingType(elementTypes, syntax, compilation, diagnostics)
			If (diagnostics IsNot Nothing) Then
				diagnosticBag = diagnostics.DiagnosticBag
			Else
				diagnosticBag = False
			End If
			If (diagnosticBag AndAlso DirectCast(compilation.SourceModule, SourceModuleSymbol).AnyReferencedAssembliesAreLinked) Then
				EmbeddedTypesManager.IsValidEmbeddableType(tupleUnderlyingType, syntax, diagnostics.DiagnosticBag, Nothing)
			End If
			Dim tupleTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol.Create(locationOpt, tupleUnderlyingType, elementLocations, elementNames, errorPositions)
			If (shouldCheckConstraints) Then
				tupleTypeSymbol.CheckConstraints(syntax, elementLocations, diagnostics, New CompoundUseSiteInfo(Of AssemblySymbol)(diagnostics, compilation.Assembly))
			End If
			Return tupleTypeSymbol
		End Function

		Public Shared Function Create(ByVal tupleCompatibleType As NamedTypeSymbol) As TupleTypeSymbol
			Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of Location).Empty
			Dim locations As ImmutableArray(Of Location) = New ImmutableArray(Of Location)()
			Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
			Dim flags As ImmutableArray(Of Boolean) = New ImmutableArray(Of Boolean)()
			Return TupleTypeSymbol.Create(empty, tupleCompatibleType, locations, strs, flags)
		End Function

		Public Shared Function Create(ByVal tupleCompatibleType As NamedTypeSymbol, ByVal elementNames As ImmutableArray(Of String)) As TupleTypeSymbol
			Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of Location).Empty
			Dim locations As ImmutableArray(Of Location) = New ImmutableArray(Of Location)()
			Dim flags As ImmutableArray(Of Boolean) = New ImmutableArray(Of Boolean)()
			Return TupleTypeSymbol.Create(empty, tupleCompatibleType, locations, elementNames, flags)
		End Function

		Public Shared Function Create(ByVal locationOpt As Location, ByVal tupleCompatibleType As NamedTypeSymbol, ByVal elementLocations As ImmutableArray(Of Location), ByVal elementNames As ImmutableArray(Of String), ByVal errorPositions As ImmutableArray(Of Boolean)) As TupleTypeSymbol
			Dim empty As ImmutableArray(Of Location)
			If (locationOpt Is Nothing) Then
				empty = ImmutableArray(Of Location).Empty
			Else
				empty = ImmutableArray.Create(Of Location)(locationOpt)
			End If
			Return TupleTypeSymbol.Create(empty, tupleCompatibleType, elementLocations, elementNames, errorPositions)
		End Function

		Public Shared Function Create(ByVal locations As ImmutableArray(Of Location), ByVal tupleCompatibleType As NamedTypeSymbol, ByVal elementLocations As ImmutableArray(Of Location), ByVal elementNames As ImmutableArray(Of String), ByVal errorPositions As ImmutableArray(Of Boolean)) As TupleTypeSymbol
			Dim typeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
			If (tupleCompatibleType.Arity <> 8) Then
				typeArgumentsNoUseSiteDiagnostics = tupleCompatibleType.TypeArgumentsNoUseSiteDiagnostics
			Else
				tupleCompatibleType = TupleTypeSymbol.EnsureRestExtensionsAreTuples(tupleCompatibleType)
				Dim tupleElementTypes As ImmutableArray(Of TypeSymbol) = tupleCompatibleType.TypeArgumentsNoUseSiteDiagnostics(7).TupleElementTypes
				Dim instance As ArrayBuilder(Of TypeSymbol) = ArrayBuilder(Of TypeSymbol).GetInstance(7 + tupleElementTypes.Length)
				instance.AddRange(tupleCompatibleType.TypeArgumentsNoUseSiteDiagnostics, 7)
				instance.AddRange(tupleElementTypes)
				typeArgumentsNoUseSiteDiagnostics = instance.ToImmutableAndFree()
			End If
			Return New TupleTypeSymbol(locations, tupleCompatibleType, elementLocations, elementNames, typeArgumentsNoUseSiteDiagnostics, errorPositions)
		End Function

		Private Function CreateMembers() As ImmutableArray(Of Symbol)
			Dim tupleElementFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleElementFieldSymbol
			Dim num As Integer
			Dim item As String
			Dim location As Microsoft.CodeAnalysis.Location
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim str As String
			Dim item1 As Microsoft.CodeAnalysis.Location
			Dim location1 As Microsoft.CodeAnalysis.Location
			Dim typeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol) = Me._elementTypes
			Dim instance As ArrayBuilder(Of Boolean) = ArrayBuilder(Of Boolean).GetInstance(typeArgumentsNoUseSiteDiagnostics.Length, False)
			Dim length As Integer = Me._elementTypes.Length
			Dim members As ImmutableArray(Of Symbol) = Me._underlyingType.OriginalDefinition.GetMembers()
			Dim symbols As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance(Math.Max(length, members.Length))
			Dim tupleUnderlyingType As NamedTypeSymbol = Me._underlyingType
			Dim num1 As Integer = 0
			Dim fieldSymbols As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol).GetInstance(tupleUnderlyingType.Arity)
			TupleTypeSymbol.CollectTargetTupleFields(tupleUnderlyingType, fieldSymbols)
			Dim members1 As ImmutableArray(Of Symbol) = tupleUnderlyingType.OriginalDefinition.GetMembers()
			While True
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = members1.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					Dim kind As SymbolKind = current.Kind
					Select Case kind
						Case SymbolKind.[Event]
							If (num1 <> 0) Then
								Continue While
							End If
							symbols.Add(New TupleEventSymbol(Me, DirectCast(current, EventSymbol).AsMember(tupleUnderlyingType)))
							Continue While
						Case SymbolKind.Field
							Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
							Dim num2 As Integer = fieldSymbols.IndexOf(fieldSymbol, ReferenceEqualityComparer.Instance)
							If (num2 < 0) Then
								If (num1 <> 0) Then
									Continue While
								End If
								symbols.Add(New TupleFieldSymbol(Me, fieldSymbol.AsMember(tupleUnderlyingType), -symbols.Count - 1))
								Continue While
							Else
								If (num1 <> 0) Then
									num2 = num2 + 7 * num1
								End If
								If (Me._providedElementNames.IsDefault) Then
									item = Nothing
								Else
									item = Me._providedElementNames(num2)
								End If
								Dim str1 As String = item
								If (Me._elementLocations.IsDefault) Then
									location = Nothing
								Else
									location = Me._elementLocations(num2)
								End If
								Dim location2 As Microsoft.CodeAnalysis.Location = location
								Dim str2 As String = TupleTypeSymbol.TupleMemberName(num2 + 1)
								Dim flag As Boolean = Not CaseInsensitiveComparison.Equals(str1, str2)
								Dim fieldSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = fieldSymbol.AsMember(tupleUnderlyingType)
								If (num1 = 0) Then
									tupleElementFieldSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleElementFieldSymbol(Me, fieldSymbol1, num2, location2, flag, Nothing)
								Else
									tupleElementFieldSymbol = New TupleVirtualElementFieldSymbol(Me, fieldSymbol1, str2, False, num2, location2, flag, Nothing)
								End If
								symbols.Add(tupleElementFieldSymbol)
								If (flag AndAlso Not [String].IsNullOrEmpty(str1)) Then
									Dim flag1 As Boolean = If(Me._errorPositions.IsDefault, False, Me._errorPositions(num2))
									symbols.Add(New TupleVirtualElementFieldSymbol(Me, fieldSymbol1, str1, flag1, num2, location2, False, tupleElementFieldSymbol))
								End If
								instance(num2) = True
								Continue While
							End If
						Case SymbolKind.Label
						Case SymbolKind.Local
						Case SymbolKind.NetModule
							If (num1 <> 0) Then
								Continue While
							End If
							Throw ExceptionUtilities.UnexpectedValue(current.Kind)
						Case SymbolKind.Method
							If (num1 <> 0) Then
								Continue While
							End If
							symbols.Add(New TupleMethodSymbol(Me, DirectCast(current, MethodSymbol).AsMember(tupleUnderlyingType)))
							Continue While
						Case SymbolKind.NamedType
							Continue While
						Case Else
							If (kind = SymbolKind.[Property]) Then
								If (num1 <> 0) Then
									Continue While
								End If
								symbols.Add(New TuplePropertySymbol(Me, DirectCast(current, PropertySymbol).AsMember(tupleUnderlyingType)))
								Continue While
							Else
								If (num1 <> 0) Then
									Continue While
								End If
								Throw ExceptionUtilities.UnexpectedValue(current.Kind)
							End If
					End Select
				End While
				If (tupleUnderlyingType.Arity <> 8) Then
					Exit While
				End If
				typeArgumentsNoUseSiteDiagnostics = tupleUnderlyingType.TypeArgumentsNoUseSiteDiagnostics
				tupleUnderlyingType = typeArgumentsNoUseSiteDiagnostics(7).TupleUnderlyingType
				num1 = num1 + 1
				If (tupleUnderlyingType.Arity <> 8) Then
					members1 = tupleUnderlyingType.OriginalDefinition.GetMembers()
					fieldSymbols.Clear()
					TupleTypeSymbol.CollectTargetTupleFields(tupleUnderlyingType, fieldSymbols)
				End If
			End While
			fieldSymbols.Free()
			Dim count As Integer = instance.Count - 1
			Dim num3 As Integer = 0
			Do
				If (Not instance(num3)) Then
					Dim num4 As Integer = TupleTypeSymbol.NumberOfValueTuples(num3 + 1, num)
					Dim originalDefinition As NamedTypeSymbol = TupleTypeSymbol.GetNestedTupleUnderlyingType(Me._underlyingType, num4 - 1).OriginalDefinition
					If (originalDefinition.IsErrorType()) Then
						diagnosticInfo = Nothing
					Else
						diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_MissingRuntimeHelper, New [Object]() { [String].Concat(originalDefinition.Name, ".", TupleTypeSymbol.TupleMemberName(num)) })
					End If
					Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = diagnosticInfo
					If (Me._providedElementNames.IsDefault) Then
						str = Nothing
					Else
						str = Me._providedElementNames(num3)
					End If
					Dim str3 As String = str
					If (Me._elementLocations.IsDefault) Then
						item1 = Nothing
					Else
						item1 = Me._elementLocations(num3)
					End If
					Dim location3 As Microsoft.CodeAnalysis.Location = item1
					Dim str4 As String = TupleTypeSymbol.TupleMemberName(num3 + 1)
					Dim flag2 As Boolean = Not CaseInsensitiveComparison.Equals(str3, str4)
					Dim str5 As String = str4
					Dim num5 As Integer = num3
					If (flag2) Then
						location1 = Nothing
					Else
						location1 = location3
					End If
					typeArgumentsNoUseSiteDiagnostics = Me._elementTypes
					Dim tupleErrorFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleErrorFieldSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleErrorFieldSymbol(Me, str5, num5, location1, typeArgumentsNoUseSiteDiagnostics(num3), diagnosticInfo1, flag2, Nothing)
					symbols.Add(tupleErrorFieldSymbol)
					If (flag2 AndAlso Not [String].IsNullOrEmpty(str3)) Then
						typeArgumentsNoUseSiteDiagnostics = Me._elementTypes
						symbols.Add(New Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleErrorFieldSymbol(Me, str3, num3, location3, typeArgumentsNoUseSiteDiagnostics(num3), diagnosticInfo1, False, tupleErrorFieldSymbol))
					End If
				End If
				num3 = num3 + 1
			Loop While num3 <= count
			Return symbols.ToImmutableAndFree()
		End Function

		Private Shared Function EnsureRestExtensionsAreTuples(ByVal tupleCompatibleType As NamedTypeSymbol) As NamedTypeSymbol
			If (Not tupleCompatibleType.TypeArgumentsNoUseSiteDiagnostics(7).IsTupleType) Then
				Dim instance As ArrayBuilder(Of NamedTypeSymbol) = ArrayBuilder(Of NamedTypeSymbol).GetInstance()
				Dim item As NamedTypeSymbol = tupleCompatibleType
				Do
					instance.Add(item)
					item = DirectCast(item.TypeArgumentsNoUseSiteDiagnostics(7), NamedTypeSymbol)
				Loop While item.Arity = 8
				If (Not item.IsTupleType) Then
					instance.Add(item)
				End If
				tupleCompatibleType = instance.Pop()
				Dim typeWithModifiers As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers).GetInstance(8)
				Do
					Dim locations As ImmutableArray(Of Location) = New ImmutableArray(Of Location)()
					Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
					Dim flags As ImmutableArray(Of Boolean) = New ImmutableArray(Of Boolean)()
					Dim tupleTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol.Create(Nothing, tupleCompatibleType, locations, strs, flags)
					tupleCompatibleType = instance.Pop()
					tupleCompatibleType = Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol.ReplaceRestExtensionType(tupleCompatibleType, typeWithModifiers, tupleTypeSymbol)
				Loop While instance.Count <> 0
				typeWithModifiers.Free()
				instance.Free()
			End If
			Return tupleCompatibleType
		End Function

		Public Overrides Function Equals(ByVal obj As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol::Equals(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.TypeCompareKind)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean Equals(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.TypeCompareKind)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			Me._underlyingType.GenerateDeclarationErrors(cancellationToken)
		End Sub

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Return ImmutableArray(Of String).Empty
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me._underlyingType.GetAttributes()
		End Function

		Friend Overrides Function GetAttributeUsageInfo() As AttributeUsageInfo
			Return AttributeUsageInfo.Null
		End Function

		Friend Overrides Function GetCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState) As IEnumerable(Of VisualBasicAttributeData)
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function GetEventsToEmit() As IEnumerable(Of EventSymbol)
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Me._underlyingType.GetHashCode()
		End Function

		Friend Overrides Function GetInterfacesToEmit() As IEnumerable(Of NamedTypeSymbol)
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
			If (Me._lazyMembers.IsDefault) Then
				ImmutableInterlocked.InterlockedInitialize(Of Symbol)(Me._lazyMembers, Me.CreateMembers())
			End If
			Return Me._lazyMembers
		End Function

		Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Dim func As Func(Of Symbol, String, Boolean)
			Dim members As ImmutableArray(Of Symbol) = Me.GetMembers()
			If (TupleTypeSymbol._Closure$__.$I125-0 Is Nothing) Then
				func = Function(member As Symbol, name_ As String) CaseInsensitiveComparison.Equals(member.Name, name_)
				TupleTypeSymbol._Closure$__.$I125-0 = func
			Else
				func = TupleTypeSymbol._Closure$__.$I125-0
			End If
			Return members.WhereAsArray(Of String)(func, name)
		End Function

		Friend Overrides Function GetMethodsToEmit() As IEnumerable(Of MethodSymbol)
			Throw ExceptionUtilities.Unreachable
		End Function

		Private Shared Function GetModifiers(ByVal modifiers As ImmutableArray(Of ImmutableArray(Of CustomModifier)), ByVal i As Integer) As ImmutableArray(Of CustomModifier)
			If (Not modifiers.IsDefaultOrEmpty) Then
				Return modifiers(i)
			End If
			Return New ImmutableArray(Of CustomModifier)()
		End Function

		Private Shared Function GetNestedTupleUnderlyingType(ByVal topLevelUnderlyingType As NamedTypeSymbol, ByVal depth As Integer) As NamedTypeSymbol
			Dim tupleUnderlyingType As NamedTypeSymbol = topLevelUnderlyingType
			Dim num As Integer = depth - 1
			Dim num1 As Integer = 0
			Do
				tupleUnderlyingType = tupleUnderlyingType.TypeArgumentsNoUseSiteDiagnostics(7).TupleUnderlyingType
				num1 = num1 + 1
			Loop While num1 <= num
			Return tupleUnderlyingType
		End Function

		Friend Overrides Function GetPropertiesToEmit() As IEnumerable(Of PropertySymbol)
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function GetSynthesizedWithEventsOverrides() As IEnumerable(Of PropertySymbol)
			Return SpecializedCollections.EmptyEnumerable(Of PropertySymbol)()
		End Function

		Friend Shared Function GetTupleCtor(ByVal arity As Integer) As WellKnownMember
			If (arity > 8) Then
				Throw ExceptionUtilities.Unreachable
			End If
			Return TupleTypeSymbol.tupleCtors(arity - 1)
		End Function

		Public Function GetTupleMemberSymbolForUnderlyingMember(Of TMember As Microsoft.CodeAnalysis.VisualBasic.Symbol)(ByVal underlyingMemberOpt As TMember) As TMember
			Dim tMember1 As TMember
			Dim tMember2 As TMember
			If (underlyingMemberOpt IsNot Nothing) Then
				Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbol = underlyingMemberOpt.OriginalDefinition
				If (CObj(originalDefinition.ContainingType) = CObj(Me._underlyingType.OriginalDefinition)) Then
					Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Nothing
					If (Not Me.UnderlyingDefinitionToMemberMap.TryGetValue(originalDefinition, symbol)) Then
						GoTo Label1
					End If
					tMember2 = DirectCast(symbol, TMember)
					tMember1 = tMember2
					Return tMember1
				End If
			Label1:
				tMember2 = Nothing
			Else
				tMember2 = Nothing
			End If
			tMember1 = tMember2
			Return tMember1
		End Function

		Private Shared Function GetTupleType(ByVal arity As Integer) As WellKnownType
			If (arity > 8) Then
				Throw ExceptionUtilities.Unreachable
			End If
			Return TupleTypeSymbol.tupleTypes(arity - 1)
		End Function

		Friend Shared Function GetTupleTypeMember(ByVal arity As Integer, ByVal position As Integer) As WellKnownMember
			Return TupleTypeSymbol.tupleMembers(arity - 1)(position - 1)
		End Function

		Private Shared Function GetTupleUnderlyingType(ByVal elementTypes As ImmutableArray(Of TypeSymbol), ByVal syntax As SyntaxNode, ByVal compilation As VisualBasicCompilation, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim num As Integer
			Dim num1 As Integer = TupleTypeSymbol.NumberOfValueTuples(elementTypes.Length, num)
			Dim wellKnownType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = compilation.GetWellKnownType(TupleTypeSymbol.GetTupleType(num))
			If (diagnostics IsNot Nothing AndAlso syntax IsNot Nothing) Then
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSite(diagnostics, syntax, wellKnownType)
			End If
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = wellKnownType.Construct(ImmutableArray.Create(Of TypeSymbol)(elementTypes, (num1 - 1) * 7, num))
			Dim num2 As Integer = num1 - 1
			If (num2 > 0) Then
				Dim wellKnownType1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = compilation.GetWellKnownType(TupleTypeSymbol.GetTupleType(8))
				If (diagnostics IsNot Nothing AndAlso syntax IsNot Nothing) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSite(diagnostics, syntax, wellKnownType1)
				End If
				Do
					Dim typeSymbols As ImmutableArray(Of TypeSymbol) = ImmutableArray.Create(Of TypeSymbol)(elementTypes, (num2 - 1) * 7, 7)
					namedTypeSymbol = wellKnownType1.Construct(typeSymbols.Add(namedTypeSymbol))
					num2 = num2 - 1
				Loop While num2 > 0
			End If
			Return namedTypeSymbol
		End Function

		Public Overrides Function GetTypeArgumentCustomModifiers(ByVal ordinal As Integer) As ImmutableArray(Of CustomModifier)
			Return MyBase.GetEmptyTypeArgumentCustomModifiers(ordinal)
		End Function

		Public Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Friend Shared Sub GetUnderlyingTypeChain(ByVal underlyingTupleType As NamedTypeSymbol, ByVal underlyingTupleTypeChain As ArrayBuilder(Of NamedTypeSymbol))
			Dim tupleUnderlyingType As NamedTypeSymbol = underlyingTupleType
			While True
				underlyingTupleTypeChain.Add(tupleUnderlyingType)
				If (tupleUnderlyingType.Arity <> 8) Then
					Exit While
				End If
				tupleUnderlyingType = tupleUnderlyingType.TypeArgumentsNoUseSiteDiagnostics(7).TupleUnderlyingType
			End While
		End Sub

		Friend Overrides Function GetUnificationUseSiteDiagnosticRecursive(ByVal owner As Symbol, ByRef checkedTypes As HashSet(Of TypeSymbol)) As DiagnosticInfo
			Return Me._underlyingType.GetUnificationUseSiteDiagnosticRecursive(owner, checkedTypes)
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Return Me._underlyingType.GetUseSiteInfo()
		End Function

		Private Shared Function GetWellKnownMemberInType(ByVal type As NamedTypeSymbol, ByVal relativeMember As WellKnownMember) As Symbol
			Dim descriptor As MemberDescriptor = WellKnownMembers.GetDescriptor(relativeMember)
			Return VisualBasicCompilation.GetRuntimeMember(type, descriptor, VisualBasicCompilation.SpecialMembersSignatureComparer.Instance, Nothing)
		End Function

		Friend Shared Function GetWellKnownMemberInType(ByVal type As NamedTypeSymbol, ByVal relativeMember As WellKnownMember, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal syntax As SyntaxNode) As Symbol
			Dim wellKnownMemberInType As Symbol = TupleTypeSymbol.GetWellKnownMemberInType(type, relativeMember)
			If (wellKnownMemberInType IsNot Nothing) Then
				Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = wellKnownMemberInType.GetUseSiteInfo()
				diagnostics.Add(useSiteInfo, syntax.GetLocation())
			Else
				Dim descriptor As MemberDescriptor = WellKnownMembers.GetDescriptor(relativeMember)
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, syntax, ERRID.ERR_MissingRuntimeHelper, New [Object]() { [String].Concat(type.Name, ".", descriptor.Name) })
			End If
			Return wellKnownMemberInType
		End Function

		Friend Overrides Function InternalSubstituteTypeParameters(ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As TypeWithModifiers
			Dim type As NamedTypeSymbol = DirectCast(Me.TupleUnderlyingType.InternalSubstituteTypeParameters(substitution).Type, NamedTypeSymbol)
			Dim tupleTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol.Create(Me._locations, type, Me._elementLocations, Me._providedElementNames, Me._errorPositions)
			Return New TypeWithModifiers(tupleTypeSymbol, New ImmutableArray(Of CustomModifier)())
		End Function

		Private Shared Function IsElementNameForbidden(ByVal name As String) As Boolean
			Return TupleTypeSymbol.ForbiddenNames.Contains(name)
		End Function

		Friend Shared Function IsElementNameReserved(ByVal name As String) As Integer
			Dim num As Integer
			Dim num1 As Integer
			Dim num2 As Integer
			If (Not TupleTypeSymbol.IsElementNameForbidden(name)) Then
				If (CaseInsensitiveComparison.StartsWith(name, "Item") AndAlso Int32.TryParse(name.Substring(4), num2) AndAlso num2 > 0 AndAlso CaseInsensitiveComparison.Equals(name, TupleTypeSymbol.TupleMemberName(num2))) Then
					num1 = num2
					num = num1
					Return num
				End If
				num1 = -1
			Else
				num1 = 0
			End If
			num = num1
			Return num
		End Function

		Friend Overrides Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Return Me._underlyingType.MakeAcyclicBaseType(diagnostics)
		End Function

		Friend Overrides Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Return Me._underlyingType.MakeAcyclicInterfaces(diagnostics)
		End Function

		Friend Overrides Function MakeDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Return Me._underlyingType.MakeDeclaredBase(basesBeingResolved, diagnostics)
		End Function

		Friend Overrides Function MakeDeclaredInterfaces(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Return Me._underlyingType.MakeDeclaredInterfaces(basesBeingResolved, diagnostics)
		End Function

		Private Shared Function NumberOfValueTuples(ByVal numElements As Integer, <Out> ByRef remainder As Integer) As Integer
			remainder = (numElements - 1) Mod 7 + 1
			Return (numElements - 1) / 7 + 1
		End Function

		Private Shared Function ReplaceRestExtensionType(ByVal tupleCompatibleType As NamedTypeSymbol, ByVal typeArgumentsBuilder As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers), ByVal extensionTuple As Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol) As NamedTypeSymbol
			Dim customModifiers As ImmutableArray(Of CustomModifier)
			Dim typeArgumentCustomModifiers As ImmutableArray(Of CustomModifier)
			Dim typeArgumentCustomModifiers1 As ImmutableArray(Of CustomModifier)
			Dim hasTypeArgumentsCustomModifiers As Boolean = tupleCompatibleType.HasTypeArgumentsCustomModifiers
			Dim typeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol) = tupleCompatibleType.TypeArgumentsNoUseSiteDiagnostics
			typeArgumentsBuilder.Clear()
			Dim num As Integer = 0
			Do
				Dim typeWithModifiers As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers) = typeArgumentsBuilder
				Dim item As TypeSymbol = typeArgumentsNoUseSiteDiagnostics(num)
				If (hasTypeArgumentsCustomModifiers) Then
					typeArgumentCustomModifiers = tupleCompatibleType.GetTypeArgumentCustomModifiers(num)
				Else
					customModifiers = New ImmutableArray(Of CustomModifier)()
					typeArgumentCustomModifiers = customModifiers
				End If
				typeWithModifiers.Add(New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers(item, typeArgumentCustomModifiers))
				num = num + 1
			Loop While num <= 6
			Dim typeWithModifiers1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers) = typeArgumentsBuilder
			Dim tupleTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol = extensionTuple
			If (hasTypeArgumentsCustomModifiers) Then
				typeArgumentCustomModifiers1 = tupleCompatibleType.GetTypeArgumentCustomModifiers(7)
			Else
				customModifiers = New ImmutableArray(Of CustomModifier)()
				typeArgumentCustomModifiers1 = customModifiers
			End If
			typeWithModifiers1.Add(New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers(tupleTypeSymbol, typeArgumentCustomModifiers1))
			Dim constructedFrom As NamedTypeSymbol = tupleCompatibleType.ConstructedFrom
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(constructedFrom, constructedFrom.TypeParameters, typeArgumentsBuilder.ToImmutable(), False)
			Return constructedFrom.Construct(typeSubstitution)
		End Function

		Friend Shared Sub ReportNamesMismatchesIfAny(ByVal destination As TypeSymbol, ByVal literal As BoundTupleLiteral, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim argumentNamesOpt As ImmutableArray(Of String) = literal.ArgumentNamesOpt
			If (Not argumentNamesOpt.IsDefault) Then
				Dim inferredNamesOpt As ImmutableArray(Of Boolean) = literal.InferredNamesOpt
				Dim isDefault As Boolean = inferredNamesOpt.IsDefault
				Dim tupleElementNames As ImmutableArray(Of String) = destination.TupleElementNames
				Dim length As Integer = argumentNamesOpt.Length
				Dim flag As Boolean = tupleElementNames.IsDefault
				Dim num As Integer = length - 1
				For i As Integer = 0 To num
					Dim item As String = argumentNamesOpt(i)
					Dim flag1 As Boolean = If(isDefault, False, inferredNamesOpt(i))
					If (item IsNot Nothing AndAlso Not flag1 AndAlso (flag OrElse [String].CompareOrdinal(tupleElementNames(i), item) <> 0)) Then
						Dim arguments As ImmutableArray(Of BoundExpression) = literal.Arguments
						diagnostics.Add(ERRID.WRN_TupleLiteralNameMismatch, arguments(i).Syntax.Parent.Location, New [Object]() { item, destination })
					End If
				Next

			End If
		End Sub

		Public Shared Function TransformToTupleIfCompatible(ByVal target As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			If (Not target.IsTupleCompatible()) Then
				typeSymbol = target
			Else
				typeSymbol = TupleTypeSymbol.Create(DirectCast(target, NamedTypeSymbol))
			End If
			Return typeSymbol
		End Function

		Friend Shared Function TupleMemberName(ByVal position As Integer) As String
			Return [String].Concat("Item", Microsoft.VisualBasic.CompilerServices.Conversions.ToString(position))
		End Function

		Friend Shared Sub VerifyTupleTypePresent(ByVal cardinality As Integer, ByVal syntax As VisualBasicSyntaxNode, ByVal compilation As VisualBasicCompilation, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim num As Integer
			Dim num1 As Integer = TupleTypeSymbol.NumberOfValueTuples(cardinality, num)
			Dim wellKnownType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = compilation.GetWellKnownType(TupleTypeSymbol.GetTupleType(num))
			Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSite(diagnostics, syntax, wellKnownType)
			If (num1 > 1) Then
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = compilation.GetWellKnownType(TupleTypeSymbol.GetTupleType(8))
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSite(diagnostics, syntax, namedTypeSymbol)
			End If
		End Sub

		Friend Function WithElementNames(ByVal newElementNames As ImmutableArray(Of String)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol
			Dim tupleTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol
			Dim locations As ImmutableArray(Of Location)
			Dim flags As ImmutableArray(Of Boolean)
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim typeSymbols As ImmutableArray(Of TypeSymbol)
			If (Not Me._providedElementNames.IsDefault) Then
				If (newElementNames.IsDefault OrElse Not System.Linq.ImmutableArrayExtensions.SequenceEqual(Of String, String)(Me._providedElementNames, newElementNames, DirectCast(Nothing, IEqualityComparer(Of String)))) Then
					namedTypeSymbol = Me._underlyingType
					locations = New ImmutableArray(Of Location)()
					typeSymbols = Me._elementTypes
					flags = New ImmutableArray(Of Boolean)()
					tupleTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol(Nothing, namedTypeSymbol, locations, newElementNames, typeSymbols, flags)
					Return tupleTypeSymbol
				End If
				tupleTypeSymbol = Me
				Return tupleTypeSymbol
			Else
				If (Not newElementNames.IsDefault) Then
					namedTypeSymbol = Me._underlyingType
					locations = New ImmutableArray(Of Location)()
					typeSymbols = Me._elementTypes
					flags = New ImmutableArray(Of Boolean)()
					tupleTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol(Nothing, namedTypeSymbol, locations, newElementNames, typeSymbols, flags)
					Return tupleTypeSymbol
				End If
				tupleTypeSymbol = Me
				Return tupleTypeSymbol
			End If
			namedTypeSymbol = Me._underlyingType
			locations = New ImmutableArray(Of Location)()
			typeSymbols = Me._elementTypes
			flags = New ImmutableArray(Of Boolean)()
			tupleTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol(Nothing, namedTypeSymbol, locations, newElementNames, typeSymbols, flags)
			Return tupleTypeSymbol
		End Function

		Friend Function WithUnderlyingType(ByVal newUnderlyingType As NamedTypeSymbol) As TupleTypeSymbol
			Return TupleTypeSymbol.Create(Me._locations, newUnderlyingType, Me._elementLocations, Me._providedElementNames, Me._errorPositions)
		End Function
	End Class
End Namespace