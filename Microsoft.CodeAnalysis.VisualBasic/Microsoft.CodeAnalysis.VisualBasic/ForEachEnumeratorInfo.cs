using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class ForEachEnumeratorInfo
	{
		public readonly BoundExpression GetEnumerator;

		public readonly BoundExpression MoveNext;

		public readonly BoundExpression Current;

		public readonly TypeSymbol ElementType;

		public readonly bool NeedToDispose;

		public readonly bool IsOrInheritsFromOrImplementsIDisposable;

		public readonly BoundExpression DisposeCondition;

		public readonly BoundExpression DisposeCast;

		public readonly BoundExpression CurrentConversion;

		public readonly BoundLValuePlaceholder EnumeratorPlaceholder;

		public readonly BoundRValuePlaceholder CurrentPlaceholder;

		public readonly BoundRValuePlaceholder CollectionPlaceholder;

		public ForEachEnumeratorInfo(BoundExpression getEnumerator, BoundExpression moveNext, BoundExpression current, TypeSymbol elementType, bool needToDispose, bool isOrInheritsFromOrImplementsIDisposable, BoundExpression disposeCondition, BoundExpression disposeCast, BoundExpression currentConversion, BoundLValuePlaceholder enumeratorPlaceholder, BoundRValuePlaceholder currentPlaceholder, BoundRValuePlaceholder collectionPlaceholder)
		{
			GetEnumerator = getEnumerator;
			MoveNext = moveNext;
			Current = current;
			ElementType = elementType;
			NeedToDispose = needToDispose;
			IsOrInheritsFromOrImplementsIDisposable = isOrInheritsFromOrImplementsIDisposable;
			DisposeCondition = disposeCondition;
			DisposeCast = disposeCast;
			CurrentConversion = currentConversion;
			EnumeratorPlaceholder = enumeratorPlaceholder;
			CurrentPlaceholder = currentPlaceholder;
			CollectionPlaceholder = collectionPlaceholder;
		}
	}
}
