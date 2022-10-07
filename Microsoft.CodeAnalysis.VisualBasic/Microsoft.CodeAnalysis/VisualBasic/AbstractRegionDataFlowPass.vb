Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class AbstractRegionDataFlowPass
		Inherits DataFlowPass
		Protected Overrides ReadOnly Property EnableBreakingFlowAnalysisFeatures As Boolean
			Get
				Return True
			End Get
		End Property

		Protected Overrides ReadOnly Property IgnoreOutSemantics As Boolean
			Get
				Return False
			End Get
		End Property

		Protected Overrides ReadOnly Property SuppressRedimOperandRvalueOnPreserve As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Sub New(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo, Optional ByVal initiallyAssignedVariables As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = Nothing, Optional ByVal trackUnassignments As Boolean = False, Optional ByVal trackStructsWithIntrinsicTypedFields As Boolean = False)
			MyBase.New(info, region, False, initiallyAssignedVariables, trackUnassignments, trackStructsWithIntrinsicTypedFields)
		End Sub

		Protected Overrides Function CreateLocalSymbolForVariables(ByVal declarations As ImmutableArray(Of BoundLocalDeclaration)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol
			If (declarations.Length <> 1) Then
				Dim localSymbolArray(declarations.Length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol
				Dim length As Integer = declarations.Length - 1
				Dim num As Integer = 0
				Do
					localSymbolArray(num) = declarations(num).LocalSymbol
					num = num + 1
				Loop While num <= length
				localSymbol = DataFlowPass.AmbiguousLocalsPseudoSymbol.Create(ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)(localSymbolArray))
			Else
				localSymbol = declarations(0).LocalSymbol
			End If
			Return localSymbol
		End Function

		Private Sub MakeSlots(ByVal parameters As ImmutableArray(Of ParameterSymbol))
			Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = parameters.GetEnumerator()
			While enumerator.MoveNext()
				MyBase.GetOrCreateSlot(enumerator.Current, 0)
			End While
		End Sub

		Public Overrides Function VisitLambda(ByVal node As BoundLambda) As BoundNode
			Me.MakeSlots(node.LambdaSymbol.Parameters)
			Return MyBase.VisitLambda(node)
		End Function

		Public Overrides Function VisitParameter(ByVal node As BoundParameter) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (Not node.ParameterSymbol.ContainingSymbol.IsQueryLambdaMethod) Then
				boundNode = MyBase.VisitParameter(node)
			Else
				boundNode = Nothing
			End If
			Return boundNode
		End Function
	End Class
End Namespace