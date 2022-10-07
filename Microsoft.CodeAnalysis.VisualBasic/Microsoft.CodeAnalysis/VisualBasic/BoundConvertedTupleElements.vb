Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundConvertedTupleElements
		Inherits BoundExtendedConversionInfo
		Private ReadOnly _ElementPlaceholders As ImmutableArray(Of BoundRValuePlaceholder)

		Private ReadOnly _ConvertedElements As ImmutableArray(Of BoundExpression)

		Public ReadOnly Property ConvertedElements As ImmutableArray(Of BoundExpression)
			Get
				Return Me._ConvertedElements
			End Get
		End Property

		Public ReadOnly Property ElementPlaceholders As ImmutableArray(Of BoundRValuePlaceholder)
			Get
				Return Me._ElementPlaceholders
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal elementPlaceholders As ImmutableArray(Of BoundRValuePlaceholder), ByVal convertedElements As ImmutableArray(Of BoundExpression), Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ConvertedTupleElements, syntax, If(hasErrors OrElse elementPlaceholders.NonNullAndHasErrors(), True, convertedElements.NonNullAndHasErrors()))
			Me._ElementPlaceholders = elementPlaceholders
			Me._ConvertedElements = convertedElements
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitConvertedTupleElements(Me)
		End Function

		Public Function Update(ByVal elementPlaceholders As ImmutableArray(Of BoundRValuePlaceholder), ByVal convertedElements As ImmutableArray(Of BoundExpression)) As BoundConvertedTupleElements
			Dim boundConvertedTupleElement As BoundConvertedTupleElements
			If (elementPlaceholders <> Me.ElementPlaceholders OrElse convertedElements <> Me.ConvertedElements) Then
				Dim boundConvertedTupleElement1 As BoundConvertedTupleElements = New BoundConvertedTupleElements(MyBase.Syntax, elementPlaceholders, convertedElements, MyBase.HasErrors)
				boundConvertedTupleElement1.CopyAttributes(Me)
				boundConvertedTupleElement = boundConvertedTupleElement1
			Else
				boundConvertedTupleElement = Me
			End If
			Return boundConvertedTupleElement
		End Function
	End Class
End Namespace