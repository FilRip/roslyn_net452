using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundCaseBlock : BoundStatement
	{
		private readonly BoundCaseStatement _CaseStatement;

		private readonly BoundBlock _Body;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)CaseStatement, (BoundNode)Body);

		public BoundCaseStatement CaseStatement => _CaseStatement;

		public BoundBlock Body => _Body;

		public BoundCaseBlock(SyntaxNode syntax, BoundCaseStatement caseStatement, BoundBlock body, bool hasErrors = false)
			: base(BoundKind.CaseBlock, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(caseStatement) || BoundNodeExtensions.NonNullAndHasErrors(body))
		{
			_CaseStatement = caseStatement;
			_Body = body;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitCaseBlock(this);
		}

		public BoundCaseBlock Update(BoundCaseStatement caseStatement, BoundBlock body)
		{
			if (caseStatement != CaseStatement || body != Body)
			{
				BoundCaseBlock boundCaseBlock = new BoundCaseBlock(base.Syntax, caseStatement, body, base.HasErrors);
				boundCaseBlock.CopyAttributes(this);
				return boundCaseBlock;
			}
			return this;
		}
	}
}
