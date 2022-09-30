using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundLateMemberAccess : BoundExpression
	{
		private readonly string _NameOpt;

		private readonly TypeSymbol _ContainerTypeOpt;

		private readonly BoundExpression _ReceiverOpt;

		private readonly BoundTypeArguments _TypeArgumentsOpt;

		private readonly LateBoundAccessKind _AccessKind;

		public string NameOpt => _NameOpt;

		public TypeSymbol ContainerTypeOpt => _ContainerTypeOpt;

		public BoundExpression ReceiverOpt => _ReceiverOpt;

		public BoundTypeArguments TypeArgumentsOpt => _TypeArgumentsOpt;

		public LateBoundAccessKind AccessKind => _AccessKind;

		public BoundLateMemberAccess SetAccessKind(LateBoundAccessKind newAccessKind)
		{
			return Update(NameOpt, ContainerTypeOpt, ReceiverOpt, TypeArgumentsOpt, newAccessKind, base.Type);
		}

		public BoundLateMemberAccess(SyntaxNode syntax, string nameOpt, TypeSymbol containerTypeOpt, BoundExpression receiverOpt, BoundTypeArguments typeArgumentsOpt, LateBoundAccessKind accessKind, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.LateMemberAccess, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(receiverOpt) || BoundNodeExtensions.NonNullAndHasErrors(typeArgumentsOpt))
		{
			_NameOpt = nameOpt;
			_ContainerTypeOpt = containerTypeOpt;
			_ReceiverOpt = receiverOpt;
			_TypeArgumentsOpt = typeArgumentsOpt;
			_AccessKind = accessKind;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitLateMemberAccess(this);
		}

		public BoundLateMemberAccess Update(string nameOpt, TypeSymbol containerTypeOpt, BoundExpression receiverOpt, BoundTypeArguments typeArgumentsOpt, LateBoundAccessKind accessKind, TypeSymbol type)
		{
			if ((object)nameOpt != NameOpt || (object)containerTypeOpt != ContainerTypeOpt || receiverOpt != ReceiverOpt || typeArgumentsOpt != TypeArgumentsOpt || accessKind != AccessKind || (object)type != base.Type)
			{
				BoundLateMemberAccess boundLateMemberAccess = new BoundLateMemberAccess(base.Syntax, nameOpt, containerTypeOpt, receiverOpt, typeArgumentsOpt, accessKind, type, base.HasErrors);
				boundLateMemberAccess.CopyAttributes(this);
				return boundLateMemberAccess;
			}
			return this;
		}
	}
}
