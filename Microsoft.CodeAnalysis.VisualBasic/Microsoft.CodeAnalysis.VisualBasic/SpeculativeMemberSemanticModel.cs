using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class SpeculativeMemberSemanticModel : MemberSemanticModel
	{
		public SpeculativeMemberSemanticModel(SyntaxTreeSemanticModel parentSemanticModel, VisualBasicSyntaxNode root, Binder binder, int position)
			: base(root, binder, null, parentSemanticModel, position)
		{
		}

		internal override bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, MethodBlockBaseSyntax method, out SemanticModel speculativeModel)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ExecutableStatementSyntax statement, out SemanticModel speculativeModel)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, EqualsValueSyntax initializer, out SemanticModel speculativeModel)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
