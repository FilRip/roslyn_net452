using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundInterpolatedStringExpression : BoundExpression
	{
		private readonly ImmutableArray<BoundNode> _Contents;

		private readonly Binder _Binder;

		public bool HasInterpolations
		{
			get
			{
				ImmutableArray<BoundNode>.Enumerator enumerator = Contents.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Kind == BoundKind.Interpolation)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsEmpty => Contents.Length == 0;

		public ImmutableArray<BoundNode> Contents => _Contents;

		public Binder Binder => _Binder;

		public BoundInterpolatedStringExpression(SyntaxNode syntax, ImmutableArray<BoundNode> contents, Binder binder, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.InterpolatedStringExpression, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(contents))
		{
			_Contents = contents;
			_Binder = binder;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitInterpolatedStringExpression(this);
		}

		public BoundInterpolatedStringExpression Update(ImmutableArray<BoundNode> contents, Binder binder, TypeSymbol type)
		{
			if (contents != Contents || binder != Binder || (object)type != base.Type)
			{
				BoundInterpolatedStringExpression boundInterpolatedStringExpression = new BoundInterpolatedStringExpression(base.Syntax, contents, binder, type, base.HasErrors);
				boundInterpolatedStringExpression.CopyAttributes(this);
				return boundInterpolatedStringExpression;
			}
			return this;
		}
	}
}
