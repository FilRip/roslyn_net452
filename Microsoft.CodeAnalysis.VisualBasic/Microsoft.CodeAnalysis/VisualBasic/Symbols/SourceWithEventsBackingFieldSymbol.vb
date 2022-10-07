Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SourceWithEventsBackingFieldSymbol
		Inherits SourceMemberFieldSymbol
		Private ReadOnly _property As SourcePropertySymbol

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Me._property
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property ImplicitlyDefinedBy(ByVal membersInProgress As Dictionary(Of String, ArrayBuilder(Of Symbol))) As Symbol
			Get
				Return Me._property
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return True
			End Get
		End Property

		Public Sub New(ByVal [property] As SourcePropertySymbol, ByVal syntaxRef As SyntaxReference, ByVal name As String)
			MyBase.New([property].ContainingSourceType, syntaxRef, name, DirectCast((1 Or If([property].IsShared, 128, 0)), SourceMemberFlags))
			Me._property = [property]
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			Dim declaringCompilation As VisualBasicCompilation = Me._property.DeclaringCompilation
			Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
			Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
			Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor, typedConstants, keyValuePairs, False))
			Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.SynthesizeDebuggerBrowsableNeverAttribute())
			Dim typedConstants1 As ImmutableArray(Of TypedConstant) = ImmutableArray.Create(Of TypedConstant)(New TypedConstant(declaringCompilation.GetSpecialType(SpecialType.System_String), TypedConstantKind.Primitive, Me.AssociatedSymbol.Name))
			keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
			Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_AccessedThroughPropertyAttribute__ctor, typedConstants1, keyValuePairs, False))
		End Sub
	End Class
End Namespace