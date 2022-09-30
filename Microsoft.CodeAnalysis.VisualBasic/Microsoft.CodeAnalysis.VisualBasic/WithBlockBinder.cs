using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class WithBlockBinder : BlockBaseBinder
	{
		internal class WithBlockInfo
		{
			public readonly BoundExpression OriginalExpression;

			public readonly BoundValuePlaceholderBase ExpressionPlaceholder;

			public readonly BindingDiagnosticBag Diagnostics;

			public readonly ImmutableArray<BoundExpression> DraftInitializers;

			public readonly BoundExpression DraftSubstitute;

			private int _exprAccessedFromNestedLambda;

			private int _exprHasByRefMeReference;

			public bool ExpressionIsAccessedFromNestedLambda => _exprAccessedFromNestedLambda == 2;

			public WithBlockInfo(BoundExpression originalExpression, BoundValuePlaceholderBase expressionPlaceholder, BoundExpression draftSubstitute, ImmutableArray<BoundExpression> draftInitializers, BindingDiagnosticBag diagnostics)
			{
				_exprAccessedFromNestedLambda = 0;
				_exprHasByRefMeReference = 0;
				OriginalExpression = originalExpression;
				ExpressionPlaceholder = expressionPlaceholder;
				DraftSubstitute = draftSubstitute;
				DraftInitializers = draftInitializers;
				Diagnostics = diagnostics;
			}

			public void RegisterAccessFromNestedLambda()
			{
				if (_exprAccessedFromNestedLambda != 2)
				{
					Interlocked.CompareExchange(ref _exprAccessedFromNestedLambda, 2, 0);
				}
			}

			public bool ExpressionHasByRefMeReference(int recursionDepth)
			{
				if (_exprHasByRefMeReference == 0)
				{
					int value = ((!ValueTypedMeReferenceFinder.HasByRefMeReference(DraftSubstitute, recursionDepth)) ? 1 : 2);
					Interlocked.CompareExchange(ref _exprHasByRefMeReference, value, 0);
				}
				return _exprHasByRefMeReference == 2;
			}
		}

		private class ValueTypedMeReferenceFinder : BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
		{
			private bool _found;

			private ValueTypedMeReferenceFinder(int recursionDepth)
				: base(recursionDepth)
			{
				_found = false;
			}

			public static bool HasByRefMeReference(BoundExpression expression, int recursionDepth)
			{
				ValueTypedMeReferenceFinder valueTypedMeReferenceFinder = new ValueTypedMeReferenceFinder(recursionDepth);
				valueTypedMeReferenceFinder.Visit(expression);
				return valueTypedMeReferenceFinder._found;
			}

			public override BoundNode Visit(BoundNode node)
			{
				if (!_found)
				{
					return base.Visit(node);
				}
				return null;
			}

			public override BoundNode VisitMeReference(BoundMeReference node)
			{
				_ = node.Type;
				_found = true;
				return null;
			}

			public override BoundNode VisitMyClassReference(BoundMyClassReference node)
			{
				_ = node.Type;
				_found = true;
				return null;
			}
		}

		private readonly WithBlockSyntax _withBlockSyntax;

		private WithBlockInfo _withBlockInfo;

		private ExpressionSyntax Expression => _withBlockSyntax.WithStatement.Expression;

		internal WithBlockInfo Info => _withBlockInfo;

		internal bool ExpressionIsAccessedFromNestedLambda => _withBlockInfo.ExpressionIsAccessedFromNestedLambda;

		internal BoundValuePlaceholderBase ExpressionPlaceholder => _withBlockInfo.ExpressionPlaceholder;

		internal ImmutableArray<BoundExpression> DraftInitializers => _withBlockInfo.DraftInitializers;

		internal BoundExpression DraftPlaceholderSubstitute => _withBlockInfo.DraftSubstitute;

		internal override ImmutableArray<LocalSymbol> Locals => ImmutableArray<LocalSymbol>.Empty;

		public WithBlockBinder(Binder enclosing, WithBlockSyntax syntax)
			: base(enclosing)
		{
			_withBlockInfo = null;
			_withBlockSyntax = syntax;
		}

		internal override BoundExpression GetWithStatementPlaceholderSubstitute(BoundValuePlaceholderBase placeholder)
		{
			EnsureExpressionAndPlaceholder();
			if (placeholder == ExpressionPlaceholder)
			{
				return DraftPlaceholderSubstitute;
			}
			return base.GetWithStatementPlaceholderSubstitute(placeholder);
		}

		private void EnsureExpressionAndPlaceholder()
		{
			if (_withBlockInfo == null)
			{
				BindingDiagnosticBag diagnostics = new BindingDiagnosticBag();
				BoundExpression boundExpression = base.ContainingBinder.BindValue(Expression, diagnostics);
				if (!boundExpression.IsLValue)
				{
					boundExpression = MakeRValue(boundExpression, diagnostics);
				}
				WithExpressionRewriter.Result result = new WithExpressionRewriter(_withBlockSyntax.WithStatement).AnalyzeWithExpression(ContainingMember, boundExpression, doNotUseByRefLocal: true, base.ContainingBinder, preserveIdentityOfLValues: true);
				BoundValuePlaceholderBase boundValuePlaceholderBase = null;
				boundValuePlaceholderBase = ((!boundExpression.IsLValue && !BoundExpressionExtensions.IsMeReference(boundExpression)) ? ((BoundValuePlaceholderBase)new BoundWithRValueExpressionPlaceholder(Expression, boundExpression.Type)) : ((BoundValuePlaceholderBase)new BoundWithLValueExpressionPlaceholder(Expression, boundExpression.Type)));
				boundValuePlaceholderBase.SetWasCompilerGenerated();
				Interlocked.CompareExchange(ref _withBlockInfo, new WithBlockInfo(boundExpression, boundValuePlaceholderBase, result.Expression, result.Initializers, diagnostics), null);
			}
		}

		protected override BoundStatement CreateBoundWithBlock(WithBlockSyntax node, Binder boundBlockBinder, BindingDiagnosticBag diagnostics)
		{
			EnsureExpressionAndPlaceholder();
			diagnostics.AddRange(_withBlockInfo.Diagnostics, allowMismatchInDependencyAccumulation: true);
			return new BoundWithStatement(node, _withBlockInfo.OriginalExpression, BoundNodeExtensions.MakeCompilerGenerated(boundBlockBinder.BindBlock(node, node.Statements, diagnostics)), this);
		}

		[Conditional("DEBUG")]
		private void AssertExpressionIsNotFromStatementExpression(SyntaxNode node)
		{
			while (node != null)
			{
				node = node.Parent;
			}
		}

		private void PrepareBindingOfOmittedLeft(VisualBasicSyntaxNode node, BindingDiagnosticBag diagnostics, Binder accessingBinder)
		{
			EnsureExpressionAndPlaceholder();
			WithBlockInfo withBlockInfo = _withBlockInfo;
			if ((object)ContainingMember != accessingBinder.ContainingMember)
			{
				withBlockInfo.RegisterAccessFromNestedLambda();
			}
		}

		protected internal override BoundExpression TryBindOmittedLeftForMemberAccess(MemberAccessExpressionSyntax node, BindingDiagnosticBag diagnostics, Binder accessingBinder, out bool wholeMemberAccessExpressionBound)
		{
			PrepareBindingOfOmittedLeft(node, diagnostics, accessingBinder);
			wholeMemberAccessExpressionBound = false;
			return _withBlockInfo.ExpressionPlaceholder;
		}

		protected override BoundExpression TryBindOmittedLeftForDictionaryAccess(MemberAccessExpressionSyntax node, Binder accessingBinder, BindingDiagnosticBag diagnostics)
		{
			PrepareBindingOfOmittedLeft(node, diagnostics, accessingBinder);
			return _withBlockInfo.ExpressionPlaceholder;
		}

		protected override BoundExpression TryBindOmittedLeftForConditionalAccess(ConditionalAccessExpressionSyntax node, Binder accessingBinder, BindingDiagnosticBag diagnostics)
		{
			PrepareBindingOfOmittedLeft(node, diagnostics, accessingBinder);
			return _withBlockInfo.ExpressionPlaceholder;
		}

		protected internal override BoundExpression TryBindOmittedLeftForXmlMemberAccess(XmlMemberAccessExpressionSyntax node, BindingDiagnosticBag diagnostics, Binder accessingBinder)
		{
			PrepareBindingOfOmittedLeft(node, diagnostics, accessingBinder);
			return _withBlockInfo.ExpressionPlaceholder;
		}
	}
}
