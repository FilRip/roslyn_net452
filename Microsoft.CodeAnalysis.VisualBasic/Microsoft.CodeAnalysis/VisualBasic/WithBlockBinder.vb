Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class WithBlockBinder
		Inherits BlockBaseBinder
		Private ReadOnly _withBlockSyntax As WithBlockSyntax

		Private _withBlockInfo As WithBlockBinder.WithBlockInfo

		Friend ReadOnly Property DraftInitializers As ImmutableArray(Of BoundExpression)
			Get
				Return Me._withBlockInfo.DraftInitializers
			End Get
		End Property

		Friend ReadOnly Property DraftPlaceholderSubstitute As BoundExpression
			Get
				Return Me._withBlockInfo.DraftSubstitute
			End Get
		End Property

		Private ReadOnly Property Expression As ExpressionSyntax
			Get
				Return Me._withBlockSyntax.WithStatement.Expression
			End Get
		End Property

		Friend ReadOnly Property ExpressionIsAccessedFromNestedLambda As Boolean
			Get
				Return Me._withBlockInfo.ExpressionIsAccessedFromNestedLambda
			End Get
		End Property

		Friend ReadOnly Property ExpressionPlaceholder As BoundValuePlaceholderBase
			Get
				Return Me._withBlockInfo.ExpressionPlaceholder
			End Get
		End Property

		Friend ReadOnly Property Info As WithBlockBinder.WithBlockInfo
			Get
				Return Me._withBlockInfo
			End Get
		End Property

		Friend Overrides ReadOnly Property Locals As ImmutableArray(Of LocalSymbol)
			Get
				Return ImmutableArray(Of LocalSymbol).Empty
			End Get
		End Property

		Public Sub New(ByVal enclosing As Binder, ByVal syntax As WithBlockSyntax)
			MyBase.New(enclosing)
			Me._withBlockInfo = Nothing
			Me._withBlockSyntax = syntax
		End Sub

		<Conditional("DEBUG")>
		Private Sub AssertExpressionIsNotFromStatementExpression(ByVal node As SyntaxNode)
			While node IsNot Nothing
				node = node.Parent
			End While
		End Sub

		Protected Overrides Function CreateBoundWithBlock(ByVal node As WithBlockSyntax, ByVal boundBlockBinder As Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundStatement
			Me.EnsureExpressionAndPlaceholder()
			diagnostics.AddRange(Me._withBlockInfo.Diagnostics, True)
			Return New BoundWithStatement(node, Me._withBlockInfo.OriginalExpression, boundBlockBinder.BindBlock(node, node.Statements, diagnostics).MakeCompilerGenerated(), Me, False)
		End Function

		Private Sub EnsureExpressionAndPlaceholder()
			If (Me._withBlockInfo Is Nothing) Then
				Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag()
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = MyBase.ContainingBinder.BindValue(Me.Expression, bindingDiagnosticBag, False)
				If (Not boundExpression.IsLValue) Then
					boundExpression = MyBase.MakeRValue(boundExpression, bindingDiagnosticBag)
				End If
				Dim result As WithExpressionRewriter.Result = (New WithExpressionRewriter(Me._withBlockSyntax.WithStatement)).AnalyzeWithExpression(Me.ContainingMember, boundExpression, True, MyBase.ContainingBinder, True)
				Dim boundWithLValueExpressionPlaceholder As BoundValuePlaceholderBase = Nothing
				If (boundExpression.IsLValue OrElse boundExpression.IsMeReference()) Then
					boundWithLValueExpressionPlaceholder = New Microsoft.CodeAnalysis.VisualBasic.BoundWithLValueExpressionPlaceholder(Me.Expression, boundExpression.Type)
				Else
					boundWithLValueExpressionPlaceholder = New BoundWithRValueExpressionPlaceholder(Me.Expression, boundExpression.Type)
				End If
				boundWithLValueExpressionPlaceholder.SetWasCompilerGenerated()
				Interlocked.CompareExchange(Of WithBlockBinder.WithBlockInfo)(Me._withBlockInfo, New WithBlockBinder.WithBlockInfo(boundExpression, boundWithLValueExpressionPlaceholder, result.Expression, result.Initializers, bindingDiagnosticBag), Nothing)
			End If
		End Sub

		Friend Overrides Function GetWithStatementPlaceholderSubstitute(ByVal placeholder As BoundValuePlaceholderBase) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Me.EnsureExpressionAndPlaceholder()
			boundExpression = If(placeholder <> Me.ExpressionPlaceholder, MyBase.GetWithStatementPlaceholderSubstitute(placeholder), Me.DraftPlaceholderSubstitute)
			Return boundExpression
		End Function

		Private Sub PrepareBindingOfOmittedLeft(ByVal node As VisualBasicSyntaxNode, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal accessingBinder As Binder)
			Me.EnsureExpressionAndPlaceholder()
			Dim withBlockInfo As WithBlockBinder.WithBlockInfo = Me._withBlockInfo
			If (CObj(Me.ContainingMember) <> CObj(accessingBinder.ContainingMember)) Then
				withBlockInfo.RegisterAccessFromNestedLambda()
			End If
		End Sub

		Protected Overrides Function TryBindOmittedLeftForConditionalAccess(ByVal node As ConditionalAccessExpressionSyntax, ByVal accessingBinder As Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundExpression
			Me.PrepareBindingOfOmittedLeft(node, diagnostics, accessingBinder)
			Return Me._withBlockInfo.ExpressionPlaceholder
		End Function

		Protected Overrides Function TryBindOmittedLeftForDictionaryAccess(ByVal node As MemberAccessExpressionSyntax, ByVal accessingBinder As Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundExpression
			Me.PrepareBindingOfOmittedLeft(node, diagnostics, accessingBinder)
			Return Me._withBlockInfo.ExpressionPlaceholder
		End Function

		Protected Friend Overrides Function TryBindOmittedLeftForMemberAccess(ByVal node As MemberAccessExpressionSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal accessingBinder As Binder, <Out> ByRef wholeMemberAccessExpressionBound As Boolean) As BoundExpression
			Me.PrepareBindingOfOmittedLeft(node, diagnostics, accessingBinder)
			wholeMemberAccessExpressionBound = False
			Return Me._withBlockInfo.ExpressionPlaceholder
		End Function

		Protected Friend Overrides Function TryBindOmittedLeftForXmlMemberAccess(ByVal node As XmlMemberAccessExpressionSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal accessingBinder As Binder) As BoundExpression
			Me.PrepareBindingOfOmittedLeft(node, diagnostics, accessingBinder)
			Return Me._withBlockInfo.ExpressionPlaceholder
		End Function

		Private Class ValueTypedMeReferenceFinder
			Inherits BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
			Private _found As Boolean

			Private Sub New(ByVal recursionDepth As Integer)
				MyBase.New(recursionDepth)
				Me._found = False
			End Sub

			Public Shared Function HasByRefMeReference(ByVal expression As BoundExpression, ByVal recursionDepth As Integer) As Boolean
				Dim valueTypedMeReferenceFinder As WithBlockBinder.ValueTypedMeReferenceFinder = New WithBlockBinder.ValueTypedMeReferenceFinder(recursionDepth)
				valueTypedMeReferenceFinder.Visit(expression)
				Return valueTypedMeReferenceFinder._found
			End Function

			Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				If (Me._found) Then
					boundNode = Nothing
				Else
					boundNode = MyBase.Visit(node)
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitMeReference(ByVal node As BoundMeReference) As BoundNode
				Dim type As TypeSymbol = node.Type
				Me._found = True
				Return Nothing
			End Function

			Public Overrides Function VisitMyClassReference(ByVal node As BoundMyClassReference) As BoundNode
				Dim type As TypeSymbol = node.Type
				Me._found = True
				Return Nothing
			End Function
		End Class

		Friend Class WithBlockInfo
			Public ReadOnly OriginalExpression As BoundExpression

			Public ReadOnly ExpressionPlaceholder As BoundValuePlaceholderBase

			Public ReadOnly Diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

			Public ReadOnly DraftInitializers As ImmutableArray(Of BoundExpression)

			Public ReadOnly DraftSubstitute As BoundExpression

			Private _exprAccessedFromNestedLambda As Integer

			Private _exprHasByRefMeReference As Integer

			Public ReadOnly Property ExpressionIsAccessedFromNestedLambda As Boolean
				Get
					Return Me._exprAccessedFromNestedLambda = 2
				End Get
			End Property

			Public Sub New(ByVal originalExpression As BoundExpression, ByVal expressionPlaceholder As BoundValuePlaceholderBase, ByVal draftSubstitute As BoundExpression, ByVal draftInitializers As ImmutableArray(Of BoundExpression), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
				MyBase.New()
				Me._exprAccessedFromNestedLambda = 0
				Me._exprHasByRefMeReference = 0
				Me.OriginalExpression = originalExpression
				Me.ExpressionPlaceholder = expressionPlaceholder
				Me.DraftSubstitute = draftSubstitute
				Me.DraftInitializers = draftInitializers
				Me.Diagnostics = diagnostics
			End Sub

			Public Function ExpressionHasByRefMeReference(ByVal recursionDepth As Integer) As Boolean
				If (Me._exprHasByRefMeReference = 0) Then
					Interlocked.CompareExchange(Me._exprHasByRefMeReference, If(WithBlockBinder.ValueTypedMeReferenceFinder.HasByRefMeReference(Me.DraftSubstitute, recursionDepth), 2, 1), 0)
				End If
				Return Me._exprHasByRefMeReference = 2
			End Function

			Public Sub RegisterAccessFromNestedLambda()
				If (Me._exprAccessedFromNestedLambda <> 2) Then
					Interlocked.CompareExchange(Me._exprAccessedFromNestedLambda, 2, 0)
				End If
			End Sub
		End Class
	End Class
End Namespace