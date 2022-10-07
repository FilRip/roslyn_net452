Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedWithEventsGetAccessorSymbol
		Inherits SynthesizedWithEventsAccessorSymbol
		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Microsoft.CodeAnalysis.MethodKind.PropertyGet
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me.m_propertyOrEvent.Type
			End Get
		End Property

		Public Sub New(ByVal container As SourceMemberContainerTypeSymbol, ByVal propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
			MyBase.New(container, propertySymbol)
		End Sub

		Protected Overrides Function GetParameters() As ImmutableArray(Of ParameterSymbol)
			Dim empty As ImmutableArray(Of ParameterSymbol)
			Dim containingProperty As PropertySymbol = MyBase.ContainingProperty
			If (containingProperty.ParameterCount <= 0) Then
				empty = ImmutableArray(Of ParameterSymbol).Empty
			Else
				Dim instance As ArrayBuilder(Of ParameterSymbol) = ArrayBuilder(Of ParameterSymbol).GetInstance(containingProperty.ParameterCount)
				containingProperty.CloneParameters(Me, instance)
				empty = instance.ToImmutableAndFree()
			End If
			Return empty
		End Function
	End Class
End Namespace