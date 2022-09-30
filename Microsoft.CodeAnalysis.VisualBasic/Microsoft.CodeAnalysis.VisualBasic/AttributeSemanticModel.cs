using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class AttributeSemanticModel : MemberSemanticModel
	{
		private AttributeSemanticModel(VisualBasicSyntaxNode root, Binder binder, SyntaxTreeSemanticModel containingSemanticModelOpt = null, SyntaxTreeSemanticModel parentSemanticModelOpt = null, int speculatedPosition = 0, bool ignoreAccessibility = false)
			: base(root, binder, containingSemanticModelOpt, parentSemanticModelOpt, speculatedPosition, ignoreAccessibility)
		{
		}

		internal static AttributeSemanticModel Create(SyntaxTreeSemanticModel containingSemanticModel, AttributeBinder binder, bool ignoreAccessibility = false)
		{
			return new AttributeSemanticModel(binder.Root, binder, containingSemanticModel, null, 0, ignoreAccessibility);
		}

		internal static AttributeSemanticModel CreateSpeculative(SyntaxTreeSemanticModel parentSemanticModel, VisualBasicSyntaxNode root, Binder binder, int position)
		{
			return new AttributeSemanticModel(root, binder, null, parentSemanticModel, position);
		}

		internal override BoundNode Bind(Binder binder, SyntaxNode node, BindingDiagnosticBag diagnostics)
		{
			switch (VisualBasicExtensions.Kind(node))
			{
			case SyntaxKind.Attribute:
				return binder.BindAttribute((AttributeSyntax)node, diagnostics);
			case SyntaxKind.IdentifierName:
			case SyntaxKind.QualifiedName:
				if (SyntaxFacts.IsAttributeName(node))
				{
					NameSyntax node2 = (NameSyntax)node;
					return binder.BindNamespaceOrTypeExpression(node2, diagnostics);
				}
				break;
			}
			return base.Bind(binder, node, diagnostics);
		}

		internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, EqualsValueSyntax initializer, out SemanticModel speculativeModel)
		{
			speculativeModel = null;
			return false;
		}

		internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ExecutableStatementSyntax statement, out SemanticModel speculativeModel)
		{
			speculativeModel = null;
			return false;
		}

		internal override bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, MethodBlockBaseSyntax method, out SemanticModel speculativeModel)
		{
			speculativeModel = null;
			return false;
		}
	}
}
