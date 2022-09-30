using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundXmlMemberAccess : BoundExpression
	{
		private readonly BoundExpression _MemberAccess;

		public BoundExpression MemberAccess => _MemberAccess;

		public BoundXmlMemberAccess(SyntaxNode syntax, BoundExpression memberAccess, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.XmlMemberAccess, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(memberAccess))
		{
			_MemberAccess = memberAccess;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitXmlMemberAccess(this);
		}

		public BoundXmlMemberAccess Update(BoundExpression memberAccess, TypeSymbol type)
		{
			if (memberAccess != MemberAccess || (object)type != base.Type)
			{
				BoundXmlMemberAccess boundXmlMemberAccess = new BoundXmlMemberAccess(base.Syntax, memberAccess, type, base.HasErrors);
				boundXmlMemberAccess.CopyAttributes(this);
				return boundXmlMemberAccess;
			}
			return this;
		}
	}
}
