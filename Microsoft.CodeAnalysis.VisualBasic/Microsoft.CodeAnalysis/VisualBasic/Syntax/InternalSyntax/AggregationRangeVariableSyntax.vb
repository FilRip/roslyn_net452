Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class AggregationRangeVariableSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend ReadOnly _nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax

		Friend ReadOnly _aggregation As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Aggregation As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax
			Get
				Return Me._aggregation
			End Get
		End Property

		Friend ReadOnly Property NameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax
			Get
				Return Me._nameEquals
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax, ByVal aggregation As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 2
			If (nameEquals IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(nameEquals)
				Me._nameEquals = nameEquals
			End If
			MyBase.AdjustFlagsAndWidth(aggregation)
			Me._aggregation = aggregation
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax, ByVal aggregation As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.SetFactoryContext(context)
			If (nameEquals IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(nameEquals)
				Me._nameEquals = nameEquals
			End If
			MyBase.AdjustFlagsAndWidth(aggregation)
			Me._aggregation = aggregation
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax, ByVal aggregation As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 2
			If (nameEquals IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(nameEquals)
				Me._nameEquals = nameEquals
			End If
			MyBase.AdjustFlagsAndWidth(aggregation)
			Me._aggregation = aggregation
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 2
			Dim variableNameEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax)
			If (variableNameEqualsSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(variableNameEqualsSyntax)
				Me._nameEquals = variableNameEqualsSyntax
			End If
			Dim aggregationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax)
			If (aggregationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(aggregationSyntax)
				Me._aggregation = aggregationSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitAggregationRangeVariable(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim num As Integer = i
			If (num = 0) Then
				greenNode = Me._nameEquals
			ElseIf (num = 1) Then
				greenNode = Me._aggregation
			Else
				greenNode = Nothing
			End If
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._nameEquals, Me._aggregation)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._nameEquals, Me._aggregation)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._nameEquals, IObjectWritable))
			writer.WriteValue(DirectCast(Me._aggregation, IObjectWritable))
		End Sub
	End Class
End Namespace