using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class ObjectInitializerBinder : Binder
	{
		private readonly BoundExpression _receiver;

		public ObjectInitializerBinder(Binder containingBinder, BoundExpression receiver)
			: base(containingBinder)
		{
			_receiver = receiver;
		}

		protected internal override BoundExpression TryBindOmittedLeftForMemberAccess(MemberAccessExpressionSyntax node, BindingDiagnosticBag diagnostics, Binder accessingBinder, ref bool wholeMemberAccessExpressionBound)
		{
			return _receiver;
		}

		protected internal override BoundExpression TryBindOmittedLeftForXmlMemberAccess(XmlMemberAccessExpressionSyntax node, BindingDiagnosticBag diagnostics, Binder accessingBinder)
		{
			return _receiver;
		}

		protected override BoundExpression TryBindOmittedLeftForDictionaryAccess(MemberAccessExpressionSyntax node, Binder accessingBinder, BindingDiagnosticBag diagnostics)
		{
			return _receiver;
		}

		protected override BoundExpression TryBindOmittedLeftForConditionalAccess(ConditionalAccessExpressionSyntax node, Binder accessingBinder, BindingDiagnosticBag diagnostics)
		{
			return null;
		}
	}
}
