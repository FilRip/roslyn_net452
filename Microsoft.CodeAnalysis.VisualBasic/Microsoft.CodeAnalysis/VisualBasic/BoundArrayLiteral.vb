Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundArrayLiteral
		Inherits BoundExpression
		Private ReadOnly _HasDominantType As Boolean

		Private ReadOnly _NumberOfCandidates As Integer

		Private ReadOnly _InferredType As ArrayTypeSymbol

		Private ReadOnly _Bounds As ImmutableArray(Of BoundExpression)

		Private ReadOnly _Initializer As BoundArrayInitialization

		Private ReadOnly _Binder As Microsoft.CodeAnalysis.VisualBasic.Binder

		Public ReadOnly Property Binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Get
				Return Me._Binder
			End Get
		End Property

		Public ReadOnly Property Bounds As ImmutableArray(Of BoundExpression)
			Get
				Return Me._Bounds
			End Get
		End Property

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Dim bounds As ImmutableArray(Of BoundExpression) = Me.Bounds
				Return StaticCast(Of BoundNode).From(Of BoundExpression)(bounds.Add(Me.Initializer))
			End Get
		End Property

		Public ReadOnly Property HasDominantType As Boolean
			Get
				Return Me._HasDominantType
			End Get
		End Property

		Public ReadOnly Property InferredType As ArrayTypeSymbol
			Get
				Return Me._InferredType
			End Get
		End Property

		Public ReadOnly Property Initializer As BoundArrayInitialization
			Get
				Return Me._Initializer
			End Get
		End Property

		Public ReadOnly Property IsEmptyArrayLiteral As Boolean
			Get
				If (Me.InferredType.Rank <> 1) Then
					Return False
				End If
				Return Me.Initializer.Initializers.Length = 0
			End Get
		End Property

		Public ReadOnly Property NumberOfCandidates As Integer
			Get
				Return Me._NumberOfCandidates
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal hasDominantType As Boolean, ByVal numberOfCandidates As Integer, ByVal inferredType As ArrayTypeSymbol, ByVal bounds As ImmutableArray(Of BoundExpression), ByVal initializer As BoundArrayInitialization, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ArrayLiteral, syntax, Nothing, If(hasErrors OrElse bounds.NonNullAndHasErrors(), True, initializer.NonNullAndHasErrors()))
			Me._HasDominantType = hasDominantType
			Me._NumberOfCandidates = numberOfCandidates
			Me._InferredType = inferredType
			Me._Bounds = bounds
			Me._Initializer = initializer
			Me._Binder = binder
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitArrayLiteral(Me)
		End Function

		Public Function Update(ByVal hasDominantType As Boolean, ByVal numberOfCandidates As Integer, ByVal inferredType As ArrayTypeSymbol, ByVal bounds As ImmutableArray(Of BoundExpression), ByVal initializer As BoundArrayInitialization, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Microsoft.CodeAnalysis.VisualBasic.BoundArrayLiteral
			Dim boundArrayLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundArrayLiteral
			If (hasDominantType <> Me.HasDominantType OrElse numberOfCandidates <> Me.NumberOfCandidates OrElse CObj(inferredType) <> CObj(Me.InferredType) OrElse bounds <> Me.Bounds OrElse initializer <> Me.Initializer OrElse binder <> Me.Binder) Then
				Dim boundArrayLiteral1 As Microsoft.CodeAnalysis.VisualBasic.BoundArrayLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayLiteral(MyBase.Syntax, hasDominantType, numberOfCandidates, inferredType, bounds, initializer, binder, MyBase.HasErrors)
				boundArrayLiteral1.CopyAttributes(Me)
				boundArrayLiteral = boundArrayLiteral1
			Else
				boundArrayLiteral = Me
			End If
			Return boundArrayLiteral
		End Function
	End Class
End Namespace