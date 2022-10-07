Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SynthesizedParameterSymbol
		Inherits SynthesizedParameterSimpleSymbol
		Private ReadOnly _isByRef As Boolean

		Private ReadOnly _isOptional As Boolean

		Private ReadOnly _defaultValue As ConstantValue

		Friend NotOverridable Overrides ReadOnly Property ExplicitDefaultConstantValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol)) As ConstantValue
			Get
				Return Me._defaultValue
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property HasExplicitDefaultValue As Boolean
			Get
				Return CObj(Me._defaultValue) <> CObj(Nothing)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsByRef As Boolean
			Get
				Return Me._isByRef
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOptional As Boolean
			Get
				Return Me._isOptional
			End Get
		End Property

		Public Sub New(ByVal container As MethodSymbol, ByVal type As TypeSymbol, ByVal ordinal As Integer, ByVal isByRef As Boolean, Optional ByVal name As String = "")
			MyClass.New(container, type, ordinal, isByRef, name, False, Nothing)
		End Sub

		Public Sub New(ByVal container As MethodSymbol, ByVal type As TypeSymbol, ByVal ordinal As Integer, ByVal isByRef As Boolean, ByVal name As String, ByVal isOptional As Boolean, ByVal defaultValue As ConstantValue)
			MyBase.New(container, type, ordinal, name)
			Me._isByRef = isByRef
			Me._isOptional = isOptional
			Me._defaultValue = defaultValue
		End Sub

		Public Shared Function Create(ByVal container As MethodSymbol, ByVal type As TypeSymbol, ByVal ordinal As Integer, ByVal isByRef As Boolean, ByVal name As String, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal refCustomModifiers As ImmutableArray(Of CustomModifier)) As SynthesizedParameterSymbol
			Dim synthesizedParameterSymbolWithCustomModifier As SynthesizedParameterSymbol
			If (Not customModifiers.IsEmpty OrElse Not refCustomModifiers.IsEmpty) Then
				synthesizedParameterSymbolWithCustomModifier = New SynthesizedParameterSymbolWithCustomModifiers(container, type, ordinal, isByRef, name, customModifiers, refCustomModifiers)
			Else
				synthesizedParameterSymbolWithCustomModifier = New SynthesizedParameterSymbol(container, type, ordinal, isByRef, name, False, Nothing)
			End If
			Return synthesizedParameterSymbolWithCustomModifier
		End Function

		Friend Shared Function CreateSetAccessorValueParameter(ByVal setter As MethodSymbol, ByVal propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol, ByVal parameterName As String) As ParameterSymbol
			Dim synthesizedParameterSymbolWithCustomModifier As ParameterSymbol
			Dim type As TypeSymbol = propertySymbol.Type
			Dim typeCustomModifiers As ImmutableArray(Of CustomModifier) = propertySymbol.TypeCustomModifiers
			Dim overriddenMethod As MethodSymbol = setter.OverriddenMethod
			If (overriddenMethod IsNot Nothing) Then
				Dim item As ParameterSymbol = overriddenMethod.Parameters(propertySymbol.ParameterCount)
				If (item.Type.IsSameTypeIgnoringAll(type)) Then
					type = item.Type
					typeCustomModifiers = item.CustomModifiers
				End If
			End If
			If (Not typeCustomModifiers.IsEmpty) Then
				synthesizedParameterSymbolWithCustomModifier = New SynthesizedParameterSymbolWithCustomModifiers(setter, type, propertySymbol.ParameterCount, False, parameterName, typeCustomModifiers, ImmutableArray(Of CustomModifier).Empty)
			Else
				synthesizedParameterSymbolWithCustomModifier = New SynthesizedParameterSimpleSymbol(setter, type, propertySymbol.ParameterCount, parameterName)
			End If
			Return synthesizedParameterSymbolWithCustomModifier
		End Function
	End Class
End Namespace