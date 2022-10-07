Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundArrayCreation
		Inherits BoundExpression
		Private ReadOnly _IsParamArrayArgument As Boolean

		Private ReadOnly _Bounds As ImmutableArray(Of BoundExpression)

		Private ReadOnly _InitializerOpt As BoundArrayInitialization

		Private ReadOnly _ArrayLiteralOpt As BoundArrayLiteral

		Private ReadOnly _ArrayLiteralConversion As ConversionKind

		Public ReadOnly Property ArrayLiteralConversion As ConversionKind
			Get
				Return Me._ArrayLiteralConversion
			End Get
		End Property

		Public ReadOnly Property ArrayLiteralOpt As BoundArrayLiteral
			Get
				Return Me._ArrayLiteralOpt
			End Get
		End Property

		Public ReadOnly Property Bounds As ImmutableArray(Of BoundExpression)
			Get
				Return Me._Bounds
			End Get
		End Property

		Public ReadOnly Property InitializerOpt As BoundArrayInitialization
			Get
				Return Me._InitializerOpt
			End Get
		End Property

		Public ReadOnly Property IsParamArrayArgument As Boolean
			Get
				Return Me._IsParamArrayArgument
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal bounds As ImmutableArray(Of BoundExpression), ByVal initializerOpt As BoundArrayInitialization, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, False, bounds, initializerOpt, Nothing, ConversionKind.DelegateRelaxationLevelNone, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal bounds As ImmutableArray(Of BoundExpression), ByVal initializerOpt As BoundArrayInitialization, ByVal arrayLiteralOpt As BoundArrayLiteral, ByVal arrayLiteralConversion As ConversionKind, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, False, bounds, initializerOpt, arrayLiteralOpt, arrayLiteralConversion, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal isParamArrayArgument As Boolean, ByVal bounds As ImmutableArray(Of BoundExpression), ByVal initializerOpt As BoundArrayInitialization, ByVal arrayLiteralOpt As BoundArrayLiteral, ByVal arrayLiteralConversion As ConversionKind, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ArrayCreation, syntax, type, If(hasErrors OrElse bounds.NonNullAndHasErrors() OrElse initializerOpt.NonNullAndHasErrors(), True, arrayLiteralOpt.NonNullAndHasErrors()))
			Me._IsParamArrayArgument = isParamArrayArgument
			Me._Bounds = bounds
			Me._InitializerOpt = initializerOpt
			Me._ArrayLiteralOpt = arrayLiteralOpt
			Me._ArrayLiteralConversion = arrayLiteralConversion
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitArrayCreation(Me)
		End Function

		Public Function Update(ByVal isParamArrayArgument As Boolean, ByVal bounds As ImmutableArray(Of BoundExpression), ByVal initializerOpt As BoundArrayInitialization, ByVal arrayLiteralOpt As BoundArrayLiteral, ByVal arrayLiteralConversion As ConversionKind, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation
			Dim boundArrayCreation As Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation
			If (isParamArrayArgument <> Me.IsParamArrayArgument OrElse bounds <> Me.Bounds OrElse initializerOpt <> Me.InitializerOpt OrElse arrayLiteralOpt <> Me.ArrayLiteralOpt OrElse arrayLiteralConversion <> Me.ArrayLiteralConversion OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundArrayCreation1 As Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation(MyBase.Syntax, isParamArrayArgument, bounds, initializerOpt, arrayLiteralOpt, arrayLiteralConversion, type, MyBase.HasErrors)
				boundArrayCreation1.CopyAttributes(Me)
				boundArrayCreation = boundArrayCreation1
			Else
				boundArrayCreation = Me
			End If
			Return boundArrayCreation
		End Function
	End Class
End Namespace