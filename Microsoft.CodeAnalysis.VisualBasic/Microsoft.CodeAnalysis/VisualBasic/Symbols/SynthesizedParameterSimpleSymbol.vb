Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SynthesizedParameterSimpleSymbol
		Inherits ParameterSymbol
		Protected ReadOnly _container As MethodSymbol

		Protected ReadOnly _type As TypeSymbol

		Protected ReadOnly _ordinal As Integer

		Protected ReadOnly _name As String

		Public NotOverridable Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._container
			End Get
		End Property

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property ExplicitDefaultConstantValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol)) As ConstantValue
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property HasExplicitDefaultValue As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasOptionCompare As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsByRef As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerFilePath As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerLineNumber As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerMemberName As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExplicitByRef As Boolean
			Get
				Return Me.IsByRef
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIDispatchConstant As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIUnknownConstant As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataIn As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataOut As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOptional As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsParamArray As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray(Of Location).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Dim returnTypeMarshallingInformation As MarshalPseudoCustomAttributeData
				Dim containingSymbol As MethodSymbol = DirectCast(Me.ContainingSymbol, MethodSymbol)
				If (containingSymbol.MethodKind <> MethodKind.PropertySet OrElse Not SynthesizedParameterSimpleSymbol.IsMarshalAsAttributeApplicable(containingSymbol)) Then
					returnTypeMarshallingInformation = Nothing
				Else
					returnTypeMarshallingInformation = DirectCast(containingSymbol.AssociatedSymbol, SourcePropertySymbol).ReturnTypeMarshallingInformation
				End If
				Return returnTypeMarshallingInformation
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Ordinal As Integer
			Get
				Return Me._ordinal
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._type
			End Get
		End Property

		Public Sub New(ByVal container As MethodSymbol, ByVal type As TypeSymbol, ByVal ordinal As Integer, ByVal name As String)
			MyBase.New()
			Me._container = container
			Me._type = type
			Me._ordinal = ordinal
			Me._name = name
		End Sub

		Friend NotOverridable Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
			If (Me.Type.ContainsTupleNames() AndAlso declaringCompilation.HasTupleNamesAttributes) Then
				Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.SynthesizeTupleNamesAttribute(Me.Type))
			End If
		End Sub

		Friend Shared Function IsMarshalAsAttributeApplicable(ByVal propertySetter As MethodSymbol) As Boolean
			Return propertySetter.ContainingType.IsInterface
		End Function
	End Class
End Namespace