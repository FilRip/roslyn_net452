using System;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class SpeculativeBinder : SemanticModelBinder
	{
		private SpeculativeBinder(Binder containingBinder)
			: base(containingBinder)
		{
		}

		public static SpeculativeBinder Create(Binder containingBinder)
		{
			if (containingBinder.ImplicitVariableDeclarationAllowed)
			{
				containingBinder = new ImplicitVariableBinder(containingBinder, containingBinder.ContainingMember);
			}
			return new SpeculativeBinder(containingBinder);
		}

		public override SyntaxReference GetSyntaxReference(VisualBasicSyntaxNode node)
		{
			throw new NotSupportedException();
		}

		internal override BoundExpression BindGroupAggregationExpression(GroupAggregationSyntax group, BindingDiagnosticBag diagnostics)
		{
			return base.ContainingBinder.BindGroupAggregationExpression(group, diagnostics);
		}

		internal override BoundExpression BindFunctionAggregationExpression(FunctionAggregationSyntax function, BindingDiagnosticBag diagnostics)
		{
			return base.ContainingBinder.BindFunctionAggregationExpression(function, diagnostics);
		}
	}
}
