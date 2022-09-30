using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundUsingStatement : BoundStatement
	{
		private readonly ImmutableArray<BoundLocalDeclarationBase> _ResourceList;

		private readonly BoundExpression _ResourceExpressionOpt;

		private readonly BoundBlock _Body;

		private readonly UsingInfo _UsingInfo;

		private readonly ImmutableArray<LocalSymbol> _Locals;

		public ImmutableArray<BoundLocalDeclarationBase> ResourceList => _ResourceList;

		public BoundExpression ResourceExpressionOpt => _ResourceExpressionOpt;

		public BoundBlock Body => _Body;

		public UsingInfo UsingInfo => _UsingInfo;

		public ImmutableArray<LocalSymbol> Locals => _Locals;

		public BoundUsingStatement(SyntaxNode syntax, ImmutableArray<BoundLocalDeclarationBase> resourceList, BoundExpression resourceExpressionOpt, BoundBlock body, UsingInfo usingInfo, ImmutableArray<LocalSymbol> locals, bool hasErrors = false)
			: base(BoundKind.UsingStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(resourceList) || BoundNodeExtensions.NonNullAndHasErrors(resourceExpressionOpt) || BoundNodeExtensions.NonNullAndHasErrors(body))
		{
			_ResourceList = resourceList;
			_ResourceExpressionOpt = resourceExpressionOpt;
			_Body = body;
			_UsingInfo = usingInfo;
			_Locals = locals;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitUsingStatement(this);
		}

		public BoundUsingStatement Update(ImmutableArray<BoundLocalDeclarationBase> resourceList, BoundExpression resourceExpressionOpt, BoundBlock body, UsingInfo usingInfo, ImmutableArray<LocalSymbol> locals)
		{
			if (resourceList != ResourceList || resourceExpressionOpt != ResourceExpressionOpt || body != Body || usingInfo != UsingInfo || locals != Locals)
			{
				BoundUsingStatement boundUsingStatement = new BoundUsingStatement(base.Syntax, resourceList, resourceExpressionOpt, body, usingInfo, locals, base.HasErrors);
				boundUsingStatement.CopyAttributes(this);
				return boundUsingStatement;
			}
			return this;
		}
	}
}
