Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SynthesizedWithEventsAccessorSymbol
		Inherits SynthesizedPropertyAccessorBase(Of PropertySymbol)
		Private _lazyParameters As ImmutableArray(Of ParameterSymbol)

		Private _lazyExplicitImplementations As ImmutableArray(Of MethodSymbol)

		Friend Overrides ReadOnly Property BackingFieldSymbol As FieldSymbol
			Get
				Dim associatedField As FieldSymbol
				Dim containingProperty As SourcePropertySymbol = TryCast(Me.ContainingProperty, SourcePropertySymbol)
				If (containingProperty Is Nothing) Then
					associatedField = Nothing
				Else
					associatedField = containingProperty.AssociatedField
				End If
				Return associatedField
			End Get
		End Property

		Protected ReadOnly Property ContainingProperty As PropertySymbol
			Get
				Return Me.m_propertyOrEvent
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				If (Me._lazyExplicitImplementations.IsDefault) Then
					ImmutableInterlocked.InterlockedInitialize(Of MethodSymbol)(Me._lazyExplicitImplementations, Me.GetExplicitInterfaceImplementations())
				End If
				Return Me._lazyExplicitImplementations
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property OverriddenMethod As MethodSymbol
			Get
				Return Me.ContainingProperty.GetAccessorOverride(Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.PropertyGet)
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				If (Me._lazyParameters.IsDefault) Then
					ImmutableInterlocked.InterlockedInitialize(Of ParameterSymbol)(Me._lazyParameters, Me.GetParameters())
				End If
				Return Me._lazyParameters
			End Get
		End Property

		Friend Overrides ReadOnly Property Syntax As SyntaxNode
			Get
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
				Dim containingProperty As SourcePropertySymbol = TryCast(Me.ContainingProperty, SourcePropertySymbol)
				If (containingProperty Is Nothing) Then
					syntaxNode = Nothing
				Else
					syntaxNode = containingProperty.Syntax
				End If
				Return syntaxNode
			End Get
		End Property

		Protected Sub New(ByVal container As SourceMemberContainerTypeSymbol, ByVal [property] As PropertySymbol)
			MyBase.New(container, [property])
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
			Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
			Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
			Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor, typedConstants, keyValuePairs, False))
		End Sub

		Friend NotOverridable Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Private Function GetExplicitInterfaceImplementations() As ImmutableArray(Of MethodSymbol)
			Dim empty As ImmutableArray(Of MethodSymbol)
			Dim containingProperty As SourcePropertySymbol = TryCast(Me.ContainingProperty, SourcePropertySymbol)
			If (containingProperty Is Nothing) Then
				empty = ImmutableArray(Of MethodSymbol).Empty
			Else
				empty = containingProperty.GetAccessorImplementations(Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.PropertyGet)
			End If
			Return empty
		End Function

		Protected MustOverride Function GetParameters() As ImmutableArray(Of ParameterSymbol)
	End Class
End Namespace