Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SynthesizedAccessor(Of T As Symbol)
		Inherits SynthesizedMethodBase
		Protected ReadOnly m_propertyOrEvent As T

		Private _lazyMetadataName As String

		Public NotOverridable Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return DirectCast(Me.m_propertyOrEvent, Symbol)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me.m_propertyOrEvent.DeclaredAccessibility
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return True
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ImplicitlyDefinedBy(ByVal membersInProgress As Dictionary(Of String, ArrayBuilder(Of Symbol))) As Symbol
			Get
				Return DirectCast(Me.m_propertyOrEvent, Symbol)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return Me.m_propertyOrEvent.IsMustOverride
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return Me.m_propertyOrEvent.IsNotOverridable
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOverloads As Boolean
			Get
				Return DirectCast(Me.m_propertyOrEvent, Symbol).IsOverloads()
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return Me.m_propertyOrEvent.IsOverridable
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return Me.m_propertyOrEvent.IsOverrides
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsShared As Boolean
			Get
				Return Me.m_propertyOrEvent.IsShared
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me.m_propertyOrEvent.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				If (Me._lazyMetadataName Is Nothing) Then
					Interlocked.CompareExchange(Of String)(Me._lazyMetadataName, Me.GenerateMetadataName(), Nothing)
				End If
				Return Me._lazyMetadataName
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Name As String
			Get
				Return Binder.GetAccessorName(Me.m_propertyOrEvent.Name, Me.MethodKind, Me.IsCompilationOutputWinMdObj())
			End Get
		End Property

		Public ReadOnly Property PropertyOrEvent As T
			Get
				Return Me.m_propertyOrEvent
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ShadowsExplicitly As Boolean
			Get
				Return Me.m_propertyOrEvent.ShadowsExplicitly
			End Get
		End Property

		Protected Sub New(ByVal container As NamedTypeSymbol, ByVal propertyOrEvent As T)
			MyBase.New(container)
			Me.m_propertyOrEvent = propertyOrEvent
		End Sub

		Protected Overridable Function GenerateMetadataName() As String
			Dim overriddenMethod As MethodSymbol = Me.OverriddenMethod
			Return If(overriddenMethod Is Nothing, Me.Name, overriddenMethod.MetadataName)
		End Function

		Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
			Return Me.m_propertyOrEvent.GetLexicalSortKey()
		End Function
	End Class
End Namespace