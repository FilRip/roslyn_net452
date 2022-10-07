Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SynthesizedBackingFieldBase(Of T As Symbol)
		Inherits FieldSymbol
		Protected ReadOnly _propertyOrEvent As T

		Protected ReadOnly _name As String

		Protected ReadOnly _isShared As Boolean

		Public NotOverridable Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return DirectCast(Me._propertyOrEvent, Symbol)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._propertyOrEvent.ContainingType
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Me._propertyOrEvent.ContainingType
			End Get
		End Property

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.[Private]
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ImplicitlyDefinedBy(ByVal membersInProgress As Dictionary(Of String, ArrayBuilder(Of Symbol))) As Symbol
			Get
				Return DirectCast(Me._propertyOrEvent, Symbol)
			End Get
		End Property

		Public Overrides ReadOnly Property IsConst As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
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
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsShared As Boolean
			Get
				Return Me._isShared
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._propertyOrEvent.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ShadowsExplicitly As Boolean
			Get
				Return Me._propertyOrEvent.ShadowsExplicitly
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeLayoutOffset As Nullable(Of Integer)
			Get
				Return Nothing
			End Get
		End Property

		Public Sub New(ByVal propertyOrEvent As T, ByVal name As String, ByVal isShared As Boolean)
			MyBase.New()
			Me._propertyOrEvent = propertyOrEvent
			Me._name = name
			Me._isShared = isShared
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
			If (Not Me.ContainingType.IsImplicitlyDeclared) Then
				Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
				Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
				Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor, typedConstants, keyValuePairs, False))
			End If
			Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.SynthesizeDebuggerBrowsableNeverAttribute())
			If (Me.Type.ContainsTupleNames() AndAlso declaringCompilation.HasTupleNamesAttributes) Then
				Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.SynthesizeTupleNamesAttribute(Me.Type))
			End If
		End Sub

		Friend Overrides Function GetConstantValue(ByVal inProgress As ConstantFieldsInProgress) As Microsoft.CodeAnalysis.ConstantValue
			Return Nothing
		End Function

		Friend NotOverridable Overrides Function GetLexicalSortKey() As LexicalSortKey
			Return Me._propertyOrEvent.GetLexicalSortKey()
		End Function
	End Class
End Namespace