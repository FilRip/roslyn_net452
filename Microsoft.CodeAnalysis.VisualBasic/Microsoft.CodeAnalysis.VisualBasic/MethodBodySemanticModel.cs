using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class MethodBodySemanticModel : MemberSemanticModel
	{
		private MethodBodySemanticModel(SyntaxNode root, Binder binder, SyntaxTreeSemanticModel containingSemanticModelOpt = null, SyntaxTreeSemanticModel parentSemanticModelOpt = null, int speculatedPosition = 0, bool ignoreAccessibility = false)
			: base(root, binder, containingSemanticModelOpt, parentSemanticModelOpt, speculatedPosition, ignoreAccessibility)
		{
		}

		internal static MethodBodySemanticModel Create(SyntaxTreeSemanticModel containingSemanticModel, SubOrFunctionBodyBinder binder, bool ignoreAccessibility = false)
		{
			return new MethodBodySemanticModel(binder.Root, binder, containingSemanticModel, null, 0, ignoreAccessibility);
		}

		internal static MethodBodySemanticModel CreateSpeculative(SyntaxTreeSemanticModel parentSemanticModel, VisualBasicSyntaxNode root, Binder binder, int position)
		{
			return new MethodBodySemanticModel(root, binder, null, parentSemanticModel, position);
		}

		internal override bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, MethodBlockBaseSyntax method, out SemanticModel speculativeModel)
		{
			MethodSymbol methodSymbol = (MethodSymbol)base.MemberSymbol;
			Binder binder = base.RootBinder;
			NamedTypeBinder namedTypeBinder;
			while (true)
			{
				namedTypeBinder = binder as NamedTypeBinder;
				if (namedTypeBinder != null)
				{
					break;
				}
				binder = binder.ContainingBinder;
			}
			Binder containing = BinderBuilder.CreateBinderForMethodBody(methodSymbol, method, SemanticModelBinder.Mark(namedTypeBinder, base.IgnoresAccessibility));
			StatementListBinder binder2 = new StatementListBinder(containing, method.Statements);
			speculativeModel = CreateSpeculative(parentModel, method, binder2, position);
			return true;
		}

		internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ExecutableStatementSyntax statement, out SemanticModel speculativeModel)
		{
			Binder enclosingBinder = GetEnclosingBinder(position);
			if (enclosingBinder == null)
			{
				speculativeModel = null;
				return false;
			}
			enclosingBinder = new SpeculativeStatementBinder(statement, enclosingBinder);
			SyntaxList<StatementSyntax> statementList = new SyntaxList<StatementSyntax>(statement);
			enclosingBinder = new StatementListBinder(enclosingBinder, statementList);
			speculativeModel = CreateSpeculative(parentModel, statement, enclosingBinder, position);
			return true;
		}

		internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, EqualsValueSyntax initializer, out SemanticModel speculativeModel)
		{
			speculativeModel = null;
			return false;
		}
	}
}
