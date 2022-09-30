using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundAsNewLocalDeclarations : BoundLocalDeclarationBase
	{
		private readonly ImmutableArray<BoundLocalDeclaration> _LocalDeclarations;

		private readonly BoundExpression _Initializer;

		public ImmutableArray<BoundLocalDeclaration> LocalDeclarations => _LocalDeclarations;

		public BoundExpression Initializer => _Initializer;

		public BoundAsNewLocalDeclarations(SyntaxNode syntax, ImmutableArray<BoundLocalDeclaration> localDeclarations, BoundExpression initializer, bool hasErrors = false)
			: base(BoundKind.AsNewLocalDeclarations, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(localDeclarations) || BoundNodeExtensions.NonNullAndHasErrors(initializer))
		{
			_LocalDeclarations = localDeclarations;
			_Initializer = initializer;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitAsNewLocalDeclarations(this);
		}

		public BoundAsNewLocalDeclarations Update(ImmutableArray<BoundLocalDeclaration> localDeclarations, BoundExpression initializer)
		{
			if (localDeclarations != LocalDeclarations || initializer != Initializer)
			{
				BoundAsNewLocalDeclarations boundAsNewLocalDeclarations = new BoundAsNewLocalDeclarations(base.Syntax, localDeclarations, initializer, base.HasErrors);
				boundAsNewLocalDeclarations.CopyAttributes(this);
				return boundAsNewLocalDeclarations;
			}
			return this;
		}
	}
}
