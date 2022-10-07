Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SynthesizedFieldSymbol
		Inherits FieldSymbol
		Protected ReadOnly _containingType As NamedTypeSymbol

		Protected ReadOnly _implicitlyDefinedBy As Symbol

		Protected ReadOnly _type As TypeSymbol

		Protected ReadOnly _name As String

		Protected ReadOnly _flags As SourceMemberFlags

		Protected ReadOnly _isSpecialNameAndRuntimeSpecial As Boolean

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Nothing
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

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return DirectCast((Me._flags And SourceMemberFlags.AccessibilityMask), Accessibility)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return Me._isSpecialNameAndRuntimeSpecial
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me._isSpecialNameAndRuntimeSpecial
			End Get
		End Property

		Friend Overrides ReadOnly Property ImplicitlyDefinedBy(ByVal membersInProgress As Dictionary(Of String, ArrayBuilder(Of Symbol))) As Symbol
			Get
				Return Me._implicitlyDefinedBy
			End Get
		End Property

		Public Overrides ReadOnly Property IsConst As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property IsNotSerialized As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsReadOnly As Boolean
			Get
				Return (Me._flags And SourceMemberFlags.[ReadOnly]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return (Me._flags And SourceMemberFlags.[Shared]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._implicitlyDefinedBy.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._type
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeLayoutOffset As Nullable(Of Integer)
			Get
				Return Nothing
			End Get
		End Property

		Public Sub New(ByVal containingType As NamedTypeSymbol, ByVal implicitlyDefinedBy As Symbol, ByVal type As TypeSymbol, ByVal name As String, Optional ByVal accessibility As Microsoft.CodeAnalysis.Accessibility = 1, Optional ByVal isReadOnly As Boolean = False, Optional ByVal isShared As Boolean = False, Optional ByVal isSpecialNameAndRuntimeSpecial As Boolean = False)
			MyBase.New()
			Dim accessibility1 As Microsoft.CodeAnalysis.Accessibility
			Dim accessibility2 As Microsoft.CodeAnalysis.Accessibility
			Me._containingType = containingType
			Me._implicitlyDefinedBy = implicitlyDefinedBy
			Me._type = type
			Me._name = name
			Dim accessibility3 As Microsoft.CodeAnalysis.Accessibility = accessibility
			If (isReadOnly) Then
				accessibility1 = 256
			Else
				accessibility1 = Microsoft.CodeAnalysis.Accessibility.NotApplicable
			End If
			Dim accessibility4 As Microsoft.CodeAnalysis.Accessibility = accessibility3 Or accessibility1
			If (isShared) Then
				accessibility2 = 128
			Else
				accessibility2 = Microsoft.CodeAnalysis.Accessibility.NotApplicable
			End If
			Me._flags = DirectCast((accessibility4 Or accessibility2), SourceMemberFlags)
			Me._isSpecialNameAndRuntimeSpecial = isSpecialNameAndRuntimeSpecial
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			If (Not Me._isSpecialNameAndRuntimeSpecial) Then
				Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
				If (Me.Type.ContainsTupleNames() AndAlso declaringCompilation.HasTupleNamesAttributes) Then
					Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.SynthesizeTupleNamesAttribute(Me.Type))
				End If
				If (TypeOf Me.ContainingSymbol Is SourceMemberContainerTypeSymbol) Then
					Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
					Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
					Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor, typedConstants, keyValuePairs, False))
				End If
			End If
		End Sub

		Friend Overrides Function GetConstantValue(ByVal inProgress As ConstantFieldsInProgress) As Microsoft.CodeAnalysis.ConstantValue
			Return Nothing
		End Function

		Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
			Return Me._implicitlyDefinedBy.GetLexicalSortKey()
		End Function
	End Class
End Namespace