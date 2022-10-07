Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SubstitutedEventSymbol
		Inherits EventSymbol
		Private ReadOnly _originalDefinition As EventSymbol

		Private ReadOnly _containingType As SubstitutedNamedType

		Private ReadOnly _addMethod As SubstitutedMethodSymbol

		Private ReadOnly _removeMethod As SubstitutedMethodSymbol

		Private ReadOnly _raiseMethod As SubstitutedMethodSymbol

		Private ReadOnly _associatedField As SubstitutedFieldSymbol

		Private _lazyType As TypeSymbol

		Private _lazyExplicitInterfaceImplementations As ImmutableArray(Of EventSymbol)

		Private _lazyOverriddenOrHiddenMembers As OverriddenMembersResult(Of EventSymbol)

		Public Overrides ReadOnly Property AddMethod As MethodSymbol
			Get
				Return Me._addMethod
			End Get
		End Property

		Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
			Get
				Return Me._associatedField
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me.OriginalDefinition.DeclaredAccessibility
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._originalDefinition.DeclaringSyntaxReferences
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of EventSymbol)
			Get
				If (Me._lazyExplicitInterfaceImplementations.IsDefault) Then
					Dim eventSymbols As ImmutableArray(Of EventSymbol) = ImplementsHelper.SubstituteExplicitInterfaceImplementations(Of EventSymbol)(Me._originalDefinition.ExplicitInterfaceImplementations, Me.TypeSubstitution)
					Dim eventSymbols1 As ImmutableArray(Of EventSymbol) = New ImmutableArray(Of EventSymbol)()
					ImmutableInterlocked.InterlockedCompareExchange(Of EventSymbol)(Me._lazyExplicitInterfaceImplementations, eventSymbols, eventSymbols1)
				End If
				Return Me._lazyExplicitInterfaceImplementations
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me._originalDefinition.HasSpecialName
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExplicitInterfaceImplementation As Boolean
			Get
				Return Me._originalDefinition.IsExplicitInterfaceImplementation
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me.OriginalDefinition.IsImplicitlyDeclared
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return Me._originalDefinition.IsMustOverride
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return Me._originalDefinition.IsNotOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return Me._originalDefinition.IsOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return Me.OriginalDefinition.IsOverrides
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return Me._originalDefinition.IsShared
			End Get
		End Property

		Public Overrides ReadOnly Property IsWindowsRuntimeEvent As Boolean
			Get
				Return Me._originalDefinition.IsWindowsRuntimeEvent
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._originalDefinition.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me.OriginalDefinition.Name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Me.OriginalDefinition.ObsoleteAttributeData
			End Get
		End Property

		Public Overrides ReadOnly Property OriginalDefinition As EventSymbol
			Get
				Return Me._originalDefinition
			End Get
		End Property

		Friend Overrides ReadOnly Property OverriddenOrHiddenMembers As OverriddenMembersResult(Of EventSymbol)
			Get
				If (Me._lazyOverriddenOrHiddenMembers Is Nothing) Then
					Interlocked.CompareExchange(Of OverriddenMembersResult(Of EventSymbol))(Me._lazyOverriddenOrHiddenMembers, OverrideHidingHelper(Of EventSymbol).MakeOverriddenMembers(Me), Nothing)
				End If
				Return Me._lazyOverriddenOrHiddenMembers
			End Get
		End Property

		Public Overrides ReadOnly Property RaiseMethod As MethodSymbol
			Get
				Return Me._raiseMethod
			End Get
		End Property

		Public Overrides ReadOnly Property RemoveMethod As MethodSymbol
			Get
				Return Me._removeMethod
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				If (Me._lazyType Is Nothing) Then
					Dim typeWithModifier As TypeWithModifiers = Me._originalDefinition.Type.InternalSubstituteTypeParameters(Me.TypeSubstitution)
					Interlocked.CompareExchange(Of TypeSymbol)(Me._lazyType, typeWithModifier.AsTypeSymbolOnly(), Nothing)
				End If
				Return Me._lazyType
			End Get
		End Property

		Friend ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Get
				Return Me._containingType.TypeSubstitution
			End Get
		End Property

		Friend Sub New(ByVal containingType As SubstitutedNamedType, ByVal originalDefinition As EventSymbol, ByVal addMethod As SubstitutedMethodSymbol, ByVal removeMethod As SubstitutedMethodSymbol, ByVal raiseMethod As SubstitutedMethodSymbol, ByVal associatedField As SubstitutedFieldSymbol)
			MyBase.New()
			Me._containingType = containingType
			Me._originalDefinition = originalDefinition
			Me._associatedField = associatedField
			If (addMethod IsNot Nothing) Then
				addMethod.SetAssociatedPropertyOrEvent(Me)
				Me._addMethod = addMethod
			End If
			If (removeMethod IsNot Nothing) Then
				removeMethod.SetAssociatedPropertyOrEvent(Me)
				Me._removeMethod = removeMethod
			End If
			If (raiseMethod IsNot Nothing) Then
				raiseMethod.SetAssociatedPropertyOrEvent(Me)
				Me._raiseMethod = raiseMethod
			End If
		End Sub

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me._originalDefinition.GetAttributes()
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._originalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function
	End Class
End Namespace