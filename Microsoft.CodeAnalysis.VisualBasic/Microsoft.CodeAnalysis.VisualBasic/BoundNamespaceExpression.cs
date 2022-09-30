using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundNamespaceExpression : BoundExpression
	{
		private readonly BoundExpression _UnevaluatedReceiverOpt;

		private readonly AliasSymbol _AliasOpt;

		private readonly NamespaceSymbol _NamespaceSymbol;

		public override Symbol ExpressionSymbol => (Symbol)(((object)AliasOpt) ?? ((object)NamespaceSymbol));

		public override LookupResultKind ResultKind
		{
			get
			{
				if (NamespaceSymbol.NamespaceKind == (NamespaceKind)0)
				{
					return LookupResult.WorseResultKind(LookupResultKind.Ambiguous, base.ResultKind);
				}
				return base.ResultKind;
			}
		}

		public BoundExpression UnevaluatedReceiverOpt => _UnevaluatedReceiverOpt;

		public AliasSymbol AliasOpt => _AliasOpt;

		public NamespaceSymbol NamespaceSymbol => _NamespaceSymbol;

		public BoundNamespaceExpression(SyntaxNode syntax, BoundExpression unevaluatedReceiverOpt, NamespaceSymbol namespaceSymbol, bool hasErrors)
			: this(syntax, unevaluatedReceiverOpt, null, namespaceSymbol, hasErrors)
		{
		}

		public BoundNamespaceExpression(SyntaxNode syntax, BoundExpression unevaluatedReceiverOpt, NamespaceSymbol namespaceSymbol)
			: this(syntax, unevaluatedReceiverOpt, null, namespaceSymbol)
		{
		}

		public BoundNamespaceExpression(SyntaxNode syntax, BoundExpression unevaluatedReceiverOpt, AliasSymbol aliasOpt, NamespaceSymbol namespaceSymbol, bool hasErrors = false)
			: base(BoundKind.NamespaceExpression, syntax, null, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(unevaluatedReceiverOpt))
		{
			_UnevaluatedReceiverOpt = unevaluatedReceiverOpt;
			_AliasOpt = aliasOpt;
			_NamespaceSymbol = namespaceSymbol;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitNamespaceExpression(this);
		}

		public BoundNamespaceExpression Update(BoundExpression unevaluatedReceiverOpt, AliasSymbol aliasOpt, NamespaceSymbol namespaceSymbol)
		{
			if (unevaluatedReceiverOpt != UnevaluatedReceiverOpt || (object)aliasOpt != AliasOpt || (object)namespaceSymbol != NamespaceSymbol)
			{
				BoundNamespaceExpression boundNamespaceExpression = new BoundNamespaceExpression(base.Syntax, unevaluatedReceiverOpt, aliasOpt, namespaceSymbol, base.HasErrors);
				boundNamespaceExpression.CopyAttributes(this);
				return boundNamespaceExpression;
			}
			return this;
		}
	}
}
