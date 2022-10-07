Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class ExecutableCodeBinder
		Inherits Binder
		Private ReadOnly _syntaxRoot As SyntaxNode

		Private ReadOnly _descendantBinderFactory As DescendantBinderFactory

		Private _labelsMap As MultiDictionary(Of String, SourceLabelSymbol)

		Private _labels As ImmutableArray(Of SourceLabelSymbol)

		Private ReadOnly Shared s_emptyLabelMap As MultiDictionary(Of String, SourceLabelSymbol)

		Friend ReadOnly Property Labels As ImmutableArray(Of SourceLabelSymbol)
			Get
				If (Me._labels.IsDefault) Then
					Dim sourceLabelSymbols As ImmutableArray(Of SourceLabelSymbol) = Me.BuildLabels()
					Dim sourceLabelSymbols1 As ImmutableArray(Of SourceLabelSymbol) = New ImmutableArray(Of SourceLabelSymbol)()
					ImmutableInterlocked.InterlockedCompareExchange(Of SourceLabelSymbol)(Me._labels, sourceLabelSymbols, sourceLabelSymbols1)
				End If
				Return Me._labels
			End Get
		End Property

		Private ReadOnly Property LabelsMap As MultiDictionary(Of String, SourceLabelSymbol)
			Get
				If (Me._labelsMap Is Nothing) Then
					Interlocked.CompareExchange(Of MultiDictionary(Of String, SourceLabelSymbol))(Me._labelsMap, ExecutableCodeBinder.BuildLabelsMap(Me.Labels), Nothing)
				End If
				Return Me._labelsMap
			End Get
		End Property

		Public ReadOnly Property NodeToBinderMap As ImmutableDictionary(Of SyntaxNode, BlockBaseBinder)
			Get
				Return Me._descendantBinderFactory.NodeToBinderMap
			End Get
		End Property

		Public ReadOnly Property Root As SyntaxNode
			Get
				Return Me._descendantBinderFactory.Root
			End Get
		End Property

		Friend ReadOnly Property StmtListToBinderMap As ImmutableDictionary(Of SyntaxList(Of StatementSyntax), BlockBaseBinder)
			Get
				Return Me._descendantBinderFactory.StmtListToBinderMap
			End Get
		End Property

		Shared Sub New()
			ExecutableCodeBinder.s_emptyLabelMap = New MultiDictionary(Of String, SourceLabelSymbol)(0, CaseInsensitiveComparison.Comparer, Nothing)
		End Sub

		Public Sub New(ByVal root As SyntaxNode, ByVal containingBinder As Binder)
			MyBase.New(containingBinder)
			Me._labels = New ImmutableArray(Of SourceLabelSymbol)()
			Me._syntaxRoot = root
			Me._descendantBinderFactory = New DescendantBinderFactory(Me, root)
		End Sub

		Friend Overrides Sub AddLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			If (Not Me.Labels.IsEmpty AndAlso (options And LookupOptions.LabelsOnly) = LookupOptions.LabelsOnly) Then
				Dim enumerator As ImmutableArray(Of SourceLabelSymbol).Enumerator = Me.Labels.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As SourceLabelSymbol = enumerator.Current
					nameSet.AddSymbol(current, current.Name, 0)
				End While
			End If
		End Sub

		Private Function BuildLabels() As ImmutableArray(Of SourceLabelSymbol)
			Dim empty As ImmutableArray(Of SourceLabelSymbol)
			Dim instance As ArrayBuilder(Of SourceLabelSymbol) = ArrayBuilder(Of SourceLabelSymbol).GetInstance()
			Dim labelVisitor As ExecutableCodeBinder.LabelVisitor = New ExecutableCodeBinder.LabelVisitor(instance, DirectCast(Me.ContainingMember, MethodSymbol), Me)
			Select Case Me._syntaxRoot.Kind()
				Case SyntaxKind.SingleLineFunctionLambdaExpression
				Case SyntaxKind.SingleLineSubLambdaExpression
					labelVisitor.Visit(DirectCast(Me._syntaxRoot, SingleLineLambdaExpressionSyntax).Body)
					Exit Select
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.MidExpression Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.DateLiteralExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.AddressOfExpression
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.TrueLiteralExpression Or SyntaxKind.DateLiteralExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.IsExpression Or SyntaxKind.AddressOfExpression Or SyntaxKind.BinaryConditionalExpression
				Label0:
					labelVisitor.Visit(Me._syntaxRoot)
					Exit Select
				Case SyntaxKind.MultiLineFunctionLambdaExpression
				Case SyntaxKind.MultiLineSubLambdaExpression
					labelVisitor.VisitList(DirectCast(DirectCast(Me._syntaxRoot, MultiLineLambdaExpressionSyntax).Statements, IEnumerable(Of VisualBasicSyntaxNode)))
					Exit Select
				Case Else
					GoTo Label0
			End Select
			If (instance.Count <= 0) Then
				instance.Free()
				empty = ImmutableArray(Of SourceLabelSymbol).Empty
			Else
				empty = instance.ToImmutableAndFree()
			End If
			Return empty
		End Function

		Private Shared Function BuildLabelsMap(ByVal labels As ImmutableArray(Of SourceLabelSymbol)) As MultiDictionary(Of String, SourceLabelSymbol)
			Dim sEmptyLabelMap As MultiDictionary(Of String, SourceLabelSymbol)
			If (labels.IsEmpty) Then
				sEmptyLabelMap = ExecutableCodeBinder.s_emptyLabelMap
			Else
				Dim strs As MultiDictionary(Of String, SourceLabelSymbol) = New MultiDictionary(Of String, SourceLabelSymbol)(labels.Length, CaseInsensitiveComparison.Comparer, Nothing)
				Dim enumerator As ImmutableArray(Of SourceLabelSymbol).Enumerator = labels.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As SourceLabelSymbol = enumerator.Current
					strs.Add(current.Name, current)
				End While
				sEmptyLabelMap = strs
			End If
			Return sEmptyLabelMap
		End Function

		Public Overrides Function GetBinder(ByVal stmtList As SyntaxList(Of StatementSyntax)) As Binder
			Return Me._descendantBinderFactory.GetBinder(stmtList)
		End Function

		Public Overrides Function GetBinder(ByVal node As SyntaxNode) As Binder
			Return Me._descendantBinderFactory.GetBinder(node)
		End Function

		Friend Overrides Sub LookupInSingleBinder(ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal name As String, ByVal arity As Integer, ByVal options As LookupOptions, ByVal originalBinder As Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim enumerator As MultiDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceLabelSymbol).ValueSet.Enumerator = New MultiDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceLabelSymbol).ValueSet.Enumerator()
			If ((options And LookupOptions.LabelsOnly) = LookupOptions.LabelsOnly AndAlso Me.LabelsMap IsNot Nothing) Then
				Dim item As MultiDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceLabelSymbol).ValueSet = Me.LabelsMap(name)
				Dim count As Integer = item.Count
				If (count <> 0) Then
					If (count = 1) Then
						lookupResult.SetFrom(SingleLookupResult.Good(item.[Single]()))
						Return
					End If
					Dim sourceLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceLabelSymbol = Nothing
					Dim location As Microsoft.CodeAnalysis.Location = Nothing
					Try
						enumerator = item.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceLabelSymbol = enumerator.Current
							Dim item1 As Microsoft.CodeAnalysis.Location = current.Locations(0)
							If (sourceLabelSymbol IsNot Nothing AndAlso MyBase.Compilation.CompareSourceLocations(location, item1) <= 0) Then
								Continue While
							End If
							sourceLabelSymbol = current
							location = item1
						End While
					Finally
						DirectCast(enumerator, IDisposable).Dispose()
					End Try
					lookupResult.SetFrom(SingleLookupResult.Good(sourceLabelSymbol))
				End If
			End If
		End Sub

		Friend Overrides Function LookupLabelByNameToken(ByVal labelName As SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol
			Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol
			Dim enumerator As MultiDictionary(Of String, SourceLabelSymbol).ValueSet.Enumerator = New MultiDictionary(Of String, SourceLabelSymbol).ValueSet.Enumerator()
			Dim valueText As String = labelName.ValueText
			Try
				enumerator = Me.LabelsMap(valueText).GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol = enumerator.Current
					If (current.LabelName <> labelName) Then
						Continue While
					End If
					labelSymbol = current
					Return labelSymbol
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			labelSymbol = MyBase.LookupLabelByNameToken(labelName)
			Return labelSymbol
		End Function

		Public Class LabelVisitor
			Inherits StatementSyntaxWalker
			Private ReadOnly _labels As ArrayBuilder(Of SourceLabelSymbol)

			Private ReadOnly _containingMethod As MethodSymbol

			Private ReadOnly _binder As Binder

			Public Sub New(ByVal labels As ArrayBuilder(Of SourceLabelSymbol), ByVal containingMethod As MethodSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder)
				MyBase.New()
				Me._labels = labels
				Me._containingMethod = containingMethod
				Me._binder = binder
			End Sub

			Public Overrides Sub VisitLabelStatement(ByVal node As LabelStatementSyntax)
				Me._labels.Add(New SourceLabelSymbol(node.LabelToken, Me._containingMethod, Me._binder))
			End Sub
		End Class
	End Class
End Namespace