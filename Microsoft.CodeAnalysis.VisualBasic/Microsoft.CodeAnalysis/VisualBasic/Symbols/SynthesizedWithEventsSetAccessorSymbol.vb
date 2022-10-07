Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports System
Imports System.Collections.Immutable
Imports System.Reflection

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedWithEventsSetAccessorSymbol
		Inherits SynthesizedWithEventsAccessorSymbol
		Private ReadOnly _returnType As TypeSymbol

		Private ReadOnly _valueParameterName As String

		Friend Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
			Get
				Dim methodImplAttribute As MethodImplAttributes = MyBase.ImplementationAttributes
				If (DirectCast(MyBase.AssociatedSymbol, PropertySymbol).IsWithEvents) Then
					methodImplAttribute = methodImplAttribute Or MethodImplAttributes.Synchronized
				End If
				Return methodImplAttribute
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Microsoft.CodeAnalysis.MethodKind.PropertySet
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me._returnType
			End Get
		End Property

		Public Sub New(ByVal container As SourceMemberContainerTypeSymbol, ByVal propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol, ByVal returnType As TypeSymbol, ByVal valueParameterName As String)
			MyBase.New(container, propertySymbol)
			Me._returnType = returnType
			Me._valueParameterName = valueParameterName
		End Sub

		Protected Overrides Function GetParameters() As ImmutableArray(Of ParameterSymbol)
			Dim containingProperty As PropertySymbol = MyBase.ContainingProperty
			Dim instance As ArrayBuilder(Of ParameterSymbol) = ArrayBuilder(Of ParameterSymbol).GetInstance(containingProperty.ParameterCount + 1)
			containingProperty.CloneParameters(Me, instance)
			instance.Add(SynthesizedParameterSymbol.CreateSetAccessorValueParameter(Me, containingProperty, Me._valueParameterName))
			Return instance.ToImmutableAndFree()
		End Function
	End Class
End Namespace