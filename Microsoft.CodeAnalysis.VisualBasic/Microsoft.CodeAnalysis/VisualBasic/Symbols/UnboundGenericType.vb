Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class UnboundGenericType
		Inherits NamedTypeSymbol
		Friend ReadOnly Shared UnboundTypeArgument As ErrorTypeSymbol

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return Me.OriginalDefinition.Arity
			End Get
		End Property

		Friend Overrides ReadOnly Property CanConstruct As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property CoClassType As TypeSymbol
			Get
				Return Me.OriginalDefinition.CoClassType
			End Get
		End Property

		Public Overrides ReadOnly Property ConstructedFrom As NamedTypeSymbol

		Public Overrides ReadOnly Property ContainingSymbol As Symbol

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me.OriginalDefinition.DeclaredAccessibility
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me.OriginalDefinition.DeclaringSyntaxReferences
			End Get
		End Property

		Friend Overrides ReadOnly Property DefaultPropertyName As String
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property EnumUnderlyingType As NamedTypeSymbol
			Get
				Return Me.OriginalDefinition.EnumUnderlyingType
			End Get
		End Property

		Friend Overrides ReadOnly Property HasCodeAnalysisEmbeddedAttribute As Boolean
			Get
				Return Me.OriginalDefinition.HasCodeAnalysisEmbeddedAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Return Me.OriginalDefinition.HasDeclarativeSecurity
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me.OriginalDefinition.HasSpecialName
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasTypeArgumentsCustomModifiers As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasVisualBasicEmbeddedAttribute As Boolean
			Get
				Return Me.OriginalDefinition.HasVisualBasicEmbeddedAttribute
			End Get
		End Property

		Public Overrides ReadOnly Property IsAnonymousType As Boolean
			Get
				Return Me.OriginalDefinition.IsAnonymousType
			End Get
		End Property

		Friend Overrides ReadOnly Property IsComImport As Boolean
			Get
				Return Me.OriginalDefinition.IsComImport
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExtensibleInterfaceNoUseSiteDiagnostics As Boolean
			Get
				Return Me.OriginalDefinition.IsExtensibleInterfaceNoUseSiteDiagnostics
			End Get
		End Property

		Friend Overrides ReadOnly Property IsInterface As Boolean
			Get
				Return Me.OriginalDefinition.IsInterface
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustInherit As Boolean
			Get
				Return Me.OriginalDefinition.IsMustInherit
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotInheritable As Boolean
			Get
				Return Me.OriginalDefinition.IsNotInheritable
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsSerializable As Boolean
			Get
				Return Me.OriginalDefinition.IsSerializable
			End Get
		End Property

		Public Overrides ReadOnly Property IsUnboundGenericType As Boolean

		Friend Overrides ReadOnly Property IsWindowsRuntimeImport As Boolean
			Get
				Return Me.OriginalDefinition.IsWindowsRuntimeImport
			End Get
		End Property

		Friend Overrides ReadOnly Property Layout As TypeLayout
			Get
				Return Me.OriginalDefinition.Layout
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me.OriginalDefinition.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MangleName As Boolean
			Get
				Return Me.OriginalDefinition.MangleName
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingCharSet As CharSet
			Get
				Return Me.OriginalDefinition.MarshallingCharSet
			End Get
		End Property

		Public Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me.OriginalDefinition.Name
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Me.OriginalDefinition.ObsoleteAttributeData
			End Get
		End Property

		Public Overrides ReadOnly Property OriginalDefinition As NamedTypeSymbol

		Friend Overrides ReadOnly Property ShouldAddWinRTMembers As Boolean
			Get
				Return Me.OriginalDefinition.ShouldAddWinRTMembers
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)

		Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
			Get
				Return Me.OriginalDefinition.TypeKind
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)

		Friend Overrides ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution

		Shared Sub New()
			UnboundGenericType.UnboundTypeArgument = New ErrorTypeSymbol()
		End Sub

		Private Sub New()
			MyBase.New()
		End Sub

		Public Overrides Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As NamedTypeSymbol
			Throw New InvalidOperationException()
		End Function

		Friend Shared Function Create(ByVal type As NamedTypeSymbol) As NamedTypeSymbol
			Dim constructedSymbol As NamedTypeSymbol
			If (Not type.IsUnboundGenericType) Then
				Dim constructedFromSymbol As UnboundGenericType.ConstructedFromSymbol = TryCast(type, UnboundGenericType.ConstructedFromSymbol)
				If (constructedFromSymbol Is Nothing) Then
					If (Not type.IsGenericType) Then
						Throw New InvalidOperationException()
					End If
					constructedSymbol = New UnboundGenericType.ConstructedSymbol(type.OriginalDefinition)
				Else
					constructedSymbol = constructedFromSymbol.Constructed
				End If
			Else
				constructedSymbol = type
			End If
			Return constructedSymbol
		End Function

		Public NotOverridable Overrides Function Equals(ByVal other As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
			Return Me.Equals(TryCast(other, UnboundGenericType), comparison)
		End Function

		Public Function Equals(ByVal other As UnboundGenericType, ByVal comparison As TypeCompareKind) As Boolean
			Dim flag As Boolean
			If (CObj(Me) <> CObj(other)) Then
				flag = If(other Is Nothing OrElse Not (other.[GetType]() = MyBase.[GetType]()), False, other.OriginalDefinition.Equals(Me.OriginalDefinition, comparison))
			Else
				flag = True
			End If
			Return flag
		End Function

		Friend NotOverridable Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			Throw ExceptionUtilities.Unreachable
		End Sub

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Return Me.OriginalDefinition.GetAppliedConditionalSymbols()
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.OriginalDefinition.GetAttributes()
		End Function

		Friend Overrides Function GetAttributeUsageInfo() As AttributeUsageInfo
			Return Me.OriginalDefinition.GetAttributeUsageInfo()
		End Function

		Friend Overrides Function GetDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved) As NamedTypeSymbol
			Return Nothing
		End Function

		Friend Overrides Function GetDeclaredInterfacesNoUseSiteDiagnostics(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Friend Overrides Function GetDirectBaseTypeNoUseSiteDiagnostics(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved) As NamedTypeSymbol
			Return Nothing
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me.OriginalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Friend NotOverridable Overrides Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)
			Throw ExceptionUtilities.Unreachable
		End Function

		Public NotOverridable Overrides Function GetHashCode() As Integer
			Return Hash.Combine(Of Type)(MyBase.[GetType](), Me.OriginalDefinition.GetHashCode())
		End Function

		Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Return Me.OriginalDefinition.GetSecurityInformation()
		End Function

		Friend NotOverridable Overrides Function GetSynthesizedWithEventsOverrides() As IEnumerable(Of PropertySymbol)
			Return SpecializedCollections.EmptyEnumerable(Of PropertySymbol)()
		End Function

		Public NotOverridable Overrides Function GetTypeArgumentCustomModifiers(ByVal ordinal As Integer) As ImmutableArray(Of CustomModifier)
			Return MyBase.GetEmptyTypeArgumentCustomModifiers(ordinal)
		End Function

		Friend NotOverridable Overrides Function GetUnificationUseSiteDiagnosticRecursive(ByVal owner As Symbol, ByRef checkedTypes As HashSet(Of TypeSymbol)) As DiagnosticInfo
			Return Nothing
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Return Me.OriginalDefinition.GetUseSiteInfo()
		End Function

		Friend Overrides MustOverride Function InternalSubstituteTypeParameters(ByVal additionalSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As TypeWithModifiers

		Friend Overrides Function LookupMetadataType(ByRef emittedTypeName As MetadataTypeName) As NamedTypeSymbol
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Return Nothing
		End Function

		Friend Overrides Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Friend Overrides Function MakeDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Return Nothing
		End Function

		Friend Overrides Function MakeDeclaredInterfaces(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Private NotInheritable Class ConstructedFromSymbol
			Inherits UnboundGenericType
			Public ReadOnly Constructed As UnboundGenericType.ConstructedSymbol

			Private ReadOnly _typeParameters As ImmutableArray(Of TypeParameterSymbol)

			Private ReadOnly _typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution

			Public Overrides ReadOnly Property ConstructedFrom As NamedTypeSymbol
				Get
					Return Me
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me.Constructed.ContainingSymbol
				End Get
			End Property

			Public Overrides ReadOnly Property IsUnboundGenericType As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property MemberNames As IEnumerable(Of String)
				Get
					Return SpecializedCollections.EmptyCollection(Of String)()
				End Get
			End Property

			Public Overrides ReadOnly Property OriginalDefinition As NamedTypeSymbol
				Get
					Return Me.Constructed.OriginalDefinition
				End Get
			End Property

			Friend Overrides ReadOnly Property TypeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
				Get
					Return StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(Me._typeParameters)
				End Get
			End Property

			Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
				Get
					Return Me._typeParameters
				End Get
			End Property

			Friend Overrides ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
				Get
					Return Me._typeSubstitution
				End Get
			End Property

			Public Sub New(ByVal constructed As UnboundGenericType.ConstructedSymbol)
				MyBase.New()
				Me.Constructed = constructed
				Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = constructed.OriginalDefinition.TypeParameters
				Dim substitutedTypeParameterSymbol(typeParameters.Length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedTypeParameterSymbol
				Dim length As Integer = typeParameters.Length - 1
				Dim num As Integer = 0
				Do
					substitutedTypeParameterSymbol(num) = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedTypeParameterSymbol(typeParameters(num))
					num = num + 1
				Loop While num <= length
				Dim substitutedTypeParameterSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedTypeParameterSymbol) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedTypeParameterSymbol)(substitutedTypeParameterSymbol)
				Dim containingSymbol As UnboundGenericType.ConstructedSymbol = DirectCast(constructed.ContainingSymbol, UnboundGenericType.ConstructedSymbol)
				Me._typeParameters = StaticCast(Of TypeParameterSymbol).From(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedTypeParameterSymbol)(substitutedTypeParameterSymbols)
				Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.CreateForAlphaRename(containingSymbol.TypeSubstitution, substitutedTypeParameterSymbols)
				Dim substitutedTypeParameterSymbolArray As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedTypeParameterSymbol() = substitutedTypeParameterSymbol
				Dim num1 As Integer = 0
				Do
					substitutedTypeParameterSymbolArray(num1).SetContainingSymbol(Me)
					num1 = num1 + 1
				Loop While num1 < CInt(substitutedTypeParameterSymbolArray.Length)
				Me._typeSubstitution = typeSubstitution
			End Sub

			Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
				Return ImmutableArray(Of Symbol).Empty
			End Function

			Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
				Return ImmutableArray(Of Symbol).Empty
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

			Friend Overrides Function InternalSubstituteTypeParameters(ByVal additionalSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As TypeWithModifiers
				Throw ExceptionUtilities.Unreachable
			End Function
		End Class

		Private NotInheritable Class ConstructedSymbol
			Inherits UnboundGenericType
			Private ReadOnly _originalDefinition As NamedTypeSymbol

			Private _lazyContainingSymbol As Symbol

			Private _lazyConstructedFrom As NamedTypeSymbol

			Private _lazyTypeArguments As ImmutableArray(Of TypeSymbol)

			Private _lazyTypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution

			Public Overrides ReadOnly Property ConstructedFrom As NamedTypeSymbol
				Get
					Dim originalDefinition As NamedTypeSymbol
					If (Me._lazyConstructedFrom Is Nothing) Then
						Dim containingType As NamedTypeSymbol = Me.OriginalDefinition.ContainingType
						If (containingType Is Nothing OrElse Not containingType.IsGenericType) Then
							originalDefinition = Me.OriginalDefinition
						ElseIf (Me.OriginalDefinition.Arity <> 0) Then
							originalDefinition = New UnboundGenericType.ConstructedFromSymbol(Me)
						Else
							originalDefinition = Me
						End If
						Interlocked.CompareExchange(Of NamedTypeSymbol)(Me._lazyConstructedFrom, originalDefinition, Nothing)
					End If
					Return Me._lazyConstructedFrom
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
					If (Me._lazyContainingSymbol Is Nothing) Then
						Dim containingType As NamedTypeSymbol = Me.OriginalDefinition.ContainingType
						If (containingType Is Nothing) Then
							symbol = Me.OriginalDefinition.ContainingSymbol
						Else
							symbol = If(Not containingType.IsGenericType, containingType, UnboundGenericType.Create(containingType))
						End If
						Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)(Me._lazyContainingSymbol, symbol, Nothing)
					End If
					Return Me._lazyContainingSymbol
				End Get
			End Property

			Public Overrides ReadOnly Property IsUnboundGenericType As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overrides ReadOnly Property MemberNames As IEnumerable(Of String)
				Get
					Dim name As Func(Of NamedTypeSymbol, String)
					Dim typeMembersUnordered As ImmutableArray(Of NamedTypeSymbol) = Me.OriginalDefinition.GetTypeMembersUnordered()
					If (UnboundGenericType.ConstructedSymbol._Closure$__.$I21-0 Is Nothing) Then
						name = Function(t As NamedTypeSymbol) t.Name
						UnboundGenericType.ConstructedSymbol._Closure$__.$I21-0 = name
					Else
						name = UnboundGenericType.ConstructedSymbol._Closure$__.$I21-0
					End If
					Return New List(Of String)(typeMembersUnordered.[Select](Of String)(name).Distinct())
				End Get
			End Property

			Public Overrides ReadOnly Property OriginalDefinition As NamedTypeSymbol
				Get
					Return Me._originalDefinition
				End Get
			End Property

			Friend Overrides ReadOnly Property TypeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
				Get
					If (Me._lazyTypeArguments.IsDefault) Then
						Dim unboundTypeArgument(Me.OriginalDefinition.Arity - 1 + 1 - 1) As TypeSymbol
						Dim length As Integer = CInt(unboundTypeArgument.Length) - 1
						Dim num As Integer = 0
						Do
							unboundTypeArgument(num) = UnboundGenericType.UnboundTypeArgument
							num = num + 1
						Loop While num <= length
						ImmutableInterlocked.InterlockedInitialize(Of TypeSymbol)(Me._lazyTypeArguments, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(unboundTypeArgument))
					End If
					Return Me._lazyTypeArguments
				End Get
			End Property

			Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
				Get
					Dim empty As ImmutableArray(Of TypeParameterSymbol)
					If (Me.OriginalDefinition.Arity <> 0) Then
						empty = Me.ConstructedFrom.TypeParameters
					Else
						empty = ImmutableArray(Of TypeParameterSymbol).Empty
					End If
					Return empty
				End Get
			End Property

			Friend Overrides ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
				Get
					Dim typeSubstitution1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
					If (Me._lazyTypeSubstitution Is Nothing) Then
						Dim containingSymbol As UnboundGenericType.ConstructedSymbol = TryCast(Me.ContainingSymbol, UnboundGenericType.ConstructedSymbol)
						If (containingSymbol Is Nothing) Then
							typeSubstitution1 = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(Me.OriginalDefinition, Me.OriginalDefinition.TypeParameters, Me.TypeArgumentsNoUseSiteDiagnostics, False)
						Else
							typeSubstitution1 = If(Me.OriginalDefinition.Arity <> 0, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(containingSymbol.TypeSubstitution, Me.OriginalDefinition, Me.TypeArgumentsNoUseSiteDiagnostics, False), Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(Me.OriginalDefinition, containingSymbol.TypeSubstitution, Nothing))
						End If
						Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution)(Me._lazyTypeSubstitution, typeSubstitution1, Nothing)
					End If
					Return Me._lazyTypeSubstitution
				End Get
			End Property

			Public Sub New(ByVal originalDefinition As NamedTypeSymbol)
				MyBase.New()
				If (originalDefinition.Arity = 0) Then
					Me._lazyTypeArguments = ImmutableArray(Of TypeSymbol).Empty
				End If
				Me._originalDefinition = originalDefinition
			End Sub

			Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
				Dim instance As ArrayBuilder(Of NamedTypeSymbol) = ArrayBuilder(Of NamedTypeSymbol).GetInstance()
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = Me.OriginalDefinition.GetMembers().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					If (current.Kind <> SymbolKind.NamedType) Then
						Continue While
					End If
					instance.AddRange(New NamedTypeSymbol() { DirectCast(current, NamedTypeSymbol) })
				End While
				Return StaticCast(Of Symbol).From(Of NamedTypeSymbol)(UnboundGenericType.ConstructedSymbol.GetTypeMembers(instance.ToImmutableAndFree()))
			End Function

			Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
				Return StaticCast(Of Symbol).From(Of NamedTypeSymbol)(Me.GetTypeMembers(name))
			End Function

			Public Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
				Return UnboundGenericType.ConstructedSymbol.GetTypeMembers(Me.OriginalDefinition.GetTypeMembers())
			End Function

			Private Shared Function GetTypeMembers(ByVal originalTypeMembers As ImmutableArray(Of NamedTypeSymbol)) As ImmutableArray(Of NamedTypeSymbol)
				Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol)
				If (Not originalTypeMembers.IsEmpty) Then
					Dim constructedFrom(originalTypeMembers.Length - 1 + 1 - 1) As NamedTypeSymbol
					Dim length As Integer = CInt(constructedFrom.Length) - 1
					Dim num As Integer = 0
					Do
						constructedFrom(num) = (New UnboundGenericType.ConstructedSymbol(originalTypeMembers(num))).ConstructedFrom
						num = num + 1
					Loop While num <= length
					namedTypeSymbols = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of NamedTypeSymbol)(constructedFrom)
				Else
					namedTypeSymbols = originalTypeMembers
				End If
				Return namedTypeSymbols
			End Function

			Public Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
				Return UnboundGenericType.ConstructedSymbol.GetTypeMembers(Me.OriginalDefinition.GetTypeMembers(name))
			End Function

			Public Overrides Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
				Dim constructedFrom As Func(Of NamedTypeSymbol, NamedTypeSymbol)
				Dim typeMembers As ImmutableArray(Of NamedTypeSymbol) = Me.OriginalDefinition.GetTypeMembers(name, arity)
				If (UnboundGenericType.ConstructedSymbol._Closure$__.$I28-0 Is Nothing) Then
					constructedFrom = Function(t As NamedTypeSymbol) (New UnboundGenericType.ConstructedSymbol(t)).ConstructedFrom
					UnboundGenericType.ConstructedSymbol._Closure$__.$I28-0 = constructedFrom
				Else
					constructedFrom = UnboundGenericType.ConstructedSymbol._Closure$__.$I28-0
				End If
				Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of NamedTypeSymbol, NamedTypeSymbol)(typeMembers, constructedFrom)
			End Function

			Friend Overrides Function GetTypeMembersUnordered() As ImmutableArray(Of NamedTypeSymbol)
				Return UnboundGenericType.ConstructedSymbol.GetTypeMembers(Me.OriginalDefinition.GetTypeMembersUnordered())
			End Function

			Friend Overrides Function InternalSubstituteTypeParameters(ByVal additionalSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As TypeWithModifiers
				Return New TypeWithModifiers(Me)
			End Function
		End Class
	End Class
End Namespace