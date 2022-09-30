using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundDimStatement : BoundStatement, IBoundLocalDeclarations
	{
		private readonly ImmutableArray<BoundLocalDeclarationBase> _LocalDeclarations;

		private readonly BoundExpression _InitializerOpt;

		private ImmutableArray<BoundLocalDeclarationBase> IBoundLocalDeclarations_Declarations => LocalDeclarations;

		public ImmutableArray<BoundLocalDeclarationBase> LocalDeclarations => _LocalDeclarations;

		public BoundExpression InitializerOpt => _InitializerOpt;

		public BoundDimStatement(SyntaxNode syntax, ImmutableArray<BoundLocalDeclarationBase> localDeclarations, BoundExpression initializerOpt, bool hasErrors = false)
			: base(BoundKind.DimStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(localDeclarations) || BoundNodeExtensions.NonNullAndHasErrors(initializerOpt))
		{
			_LocalDeclarations = localDeclarations;
			_InitializerOpt = initializerOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitDimStatement(this);
		}

		public BoundDimStatement Update(ImmutableArray<BoundLocalDeclarationBase> localDeclarations, BoundExpression initializerOpt)
		{
			if (localDeclarations != LocalDeclarations || initializerOpt != InitializerOpt)
			{
				BoundDimStatement boundDimStatement = new BoundDimStatement(base.Syntax, localDeclarations, initializerOpt, base.HasErrors);
				boundDimStatement.CopyAttributes(this);
				return boundDimStatement;
			}
			return this;
		}
	}
}
