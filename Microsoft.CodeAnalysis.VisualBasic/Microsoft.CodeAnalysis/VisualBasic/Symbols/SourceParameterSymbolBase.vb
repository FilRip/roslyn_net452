Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SourceParameterSymbolBase
		Inherits ParameterSymbol
		Private ReadOnly _containingSymbol As Symbol

		Private ReadOnly _ordinal As UShort

		Public NotOverridable Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingSymbol
			End Get
		End Property

		Friend MustOverride ReadOnly Property HasDefaultValueAttribute As Boolean

		Friend MustOverride ReadOnly Property HasParamArrayAttribute As Boolean

		Public NotOverridable Overrides ReadOnly Property Ordinal As Integer
			Get
				Return Me._ordinal
			End Get
		End Property

		Friend Sub New(ByVal containingSymbol As Symbol, ByVal ordinal As Integer)
			MyBase.New()
			Me._containingSymbol = containingSymbol
			Me._ordinal = CUShort(ordinal)
		End Sub

		Friend NotOverridable Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			If (Me.IsParamArray AndAlso Not Me.HasParamArrayAttribute) Then
				Dim declaringCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = Me.DeclaringCompilation
				Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
				keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
				Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_ParamArrayAttribute__ctor, typedConstants, keyValuePairs, False))
			End If
			If (Me.HasExplicitDefaultValue AndAlso Not Me.HasDefaultValueAttribute) Then
				Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = Me.DeclaringCompilation
				Dim explicitDefaultConstantValue As ConstantValue = MyBase.ExplicitDefaultConstantValue
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = explicitDefaultConstantValue.SpecialType
				If (specialType = Microsoft.CodeAnalysis.SpecialType.System_Decimal) Then
					Symbol.AddSynthesizedAttribute(attributes, visualBasicCompilation.SynthesizeDecimalConstantAttribute(explicitDefaultConstantValue.DecimalValue))
				ElseIf (specialType = Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = visualBasicCompilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int64)
					Dim dateTimeValue As DateTime = explicitDefaultConstantValue.DateTimeValue
					Dim typedConstants1 As ImmutableArray(Of TypedConstant) = ImmutableArray.Create(Of TypedConstant)(New TypedConstant(namedTypeSymbol, TypedConstantKind.Primitive, dateTimeValue.Ticks))
					keyValuePairs = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
					Symbol.AddSynthesizedAttribute(attributes, visualBasicCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_DateTimeConstantAttribute__ctor, typedConstants1, keyValuePairs, False))
				End If
			End If
			If (Me.Type.ContainsTupleNames()) Then
				Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.SynthesizeTupleNamesAttribute(Me.Type))
			End If
		End Sub

		Friend MustOverride Function WithTypeAndCustomModifiers(ByVal type As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal refCustomModifiers As ImmutableArray(Of CustomModifier)) As ParameterSymbol
	End Class
End Namespace