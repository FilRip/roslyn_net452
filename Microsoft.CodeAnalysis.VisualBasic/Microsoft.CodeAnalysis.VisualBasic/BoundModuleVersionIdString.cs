using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundModuleVersionIdString : BoundExpression
	{
		public BoundModuleVersionIdString(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(BoundKind.ModuleVersionIdString, syntax, type, hasErrors)
		{
		}

		public BoundModuleVersionIdString(SyntaxNode syntax, TypeSymbol type)
			: base(BoundKind.ModuleVersionIdString, syntax, type)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitModuleVersionIdString(this);
		}

		public BoundModuleVersionIdString Update(TypeSymbol type)
		{
			if ((object)type != base.Type)
			{
				BoundModuleVersionIdString boundModuleVersionIdString = new BoundModuleVersionIdString(base.Syntax, type, base.HasErrors);
				boundModuleVersionIdString.CopyAttributes(this);
				return boundModuleVersionIdString;
			}
			return this;
		}
	}
}
