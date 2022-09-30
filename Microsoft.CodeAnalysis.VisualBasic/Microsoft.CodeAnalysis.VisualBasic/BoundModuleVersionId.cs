using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundModuleVersionId : BoundExpression
	{
		private readonly bool _IsLValue;

		public override bool IsLValue => _IsLValue;

		public BoundModuleVersionId(SyntaxNode syntax, bool isLValue, TypeSymbol type, bool hasErrors)
			: base(BoundKind.ModuleVersionId, syntax, type, hasErrors)
		{
			_IsLValue = isLValue;
		}

		public BoundModuleVersionId(SyntaxNode syntax, bool isLValue, TypeSymbol type)
			: base(BoundKind.ModuleVersionId, syntax, type)
		{
			_IsLValue = isLValue;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitModuleVersionId(this);
		}

		public BoundModuleVersionId Update(bool isLValue, TypeSymbol type)
		{
			if (isLValue != IsLValue || (object)type != base.Type)
			{
				BoundModuleVersionId boundModuleVersionId = new BoundModuleVersionId(base.Syntax, isLValue, type, base.HasErrors);
				boundModuleVersionId.CopyAttributes(this);
				return boundModuleVersionId;
			}
			return this;
		}
	}
}
