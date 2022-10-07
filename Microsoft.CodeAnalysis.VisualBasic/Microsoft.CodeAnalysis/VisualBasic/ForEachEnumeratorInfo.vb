Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class ForEachEnumeratorInfo
		Public ReadOnly GetEnumerator As BoundExpression

		Public ReadOnly MoveNext As BoundExpression

		Public ReadOnly Current As BoundExpression

		Public ReadOnly ElementType As TypeSymbol

		Public ReadOnly NeedToDispose As Boolean

		Public ReadOnly IsOrInheritsFromOrImplementsIDisposable As Boolean

		Public ReadOnly DisposeCondition As BoundExpression

		Public ReadOnly DisposeCast As BoundExpression

		Public ReadOnly CurrentConversion As BoundExpression

		Public ReadOnly EnumeratorPlaceholder As BoundLValuePlaceholder

		Public ReadOnly CurrentPlaceholder As BoundRValuePlaceholder

		Public ReadOnly CollectionPlaceholder As BoundRValuePlaceholder

		Public Sub New(ByVal getEnumerator As BoundExpression, ByVal moveNext As BoundExpression, ByVal current As BoundExpression, ByVal elementType As TypeSymbol, ByVal needToDispose As Boolean, ByVal isOrInheritsFromOrImplementsIDisposable As Boolean, ByVal disposeCondition As BoundExpression, ByVal disposeCast As BoundExpression, ByVal currentConversion As BoundExpression, ByVal enumeratorPlaceholder As BoundLValuePlaceholder, ByVal currentPlaceholder As BoundRValuePlaceholder, ByVal collectionPlaceholder As BoundRValuePlaceholder)
			MyBase.New()
			Me.GetEnumerator = getEnumerator
			Me.MoveNext = moveNext
			Me.Current = current
			Me.ElementType = elementType
			Me.NeedToDispose = needToDispose
			Me.IsOrInheritsFromOrImplementsIDisposable = isOrInheritsFromOrImplementsIDisposable
			Me.DisposeCondition = disposeCondition
			Me.DisposeCast = disposeCast
			Me.CurrentConversion = currentConversion
			Me.EnumeratorPlaceholder = enumeratorPlaceholder
			Me.CurrentPlaceholder = currentPlaceholder
			Me.CollectionPlaceholder = collectionPlaceholder
		End Sub
	End Class
End Namespace