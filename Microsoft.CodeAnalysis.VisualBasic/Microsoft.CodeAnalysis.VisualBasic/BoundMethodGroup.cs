using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundMethodGroup : BoundMethodOrPropertyGroup
	{
		private readonly BoundTypeArguments _TypeArgumentsOpt;

		private readonly ImmutableArray<MethodSymbol> _Methods;

		private readonly ExtensionMethodGroup _PendingExtensionMethodsOpt;

		private readonly LookupResultKind _ResultKind;

		public BoundTypeArguments TypeArgumentsOpt => _TypeArgumentsOpt;

		public ImmutableArray<MethodSymbol> Methods => _Methods;

		public ExtensionMethodGroup PendingExtensionMethodsOpt => _PendingExtensionMethodsOpt;

		public override LookupResultKind ResultKind => _ResultKind;

		public BoundMethodGroup(SyntaxNode syntax, BoundTypeArguments typeArgumentsOpt, ImmutableArray<MethodSymbol> methods, LookupResultKind resultKind, BoundExpression receiverOpt, QualificationKind qualificationKind, bool hasErrors = false)
			: this(syntax, typeArgumentsOpt, methods, null, resultKind, receiverOpt, qualificationKind, hasErrors)
		{
		}

		public ImmutableArray<MethodSymbol> AdditionalExtensionMethods([In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (_PendingExtensionMethodsOpt == null)
			{
				return ImmutableArray<MethodSymbol>.Empty;
			}
			return _PendingExtensionMethodsOpt.LazyLookupAdditionalExtensionMethods(this, ref useSiteInfo);
		}

		public BoundMethodGroup(SyntaxNode syntax, BoundTypeArguments typeArgumentsOpt, ImmutableArray<MethodSymbol> methods, ExtensionMethodGroup pendingExtensionMethodsOpt, LookupResultKind resultKind, BoundExpression receiverOpt, QualificationKind qualificationKind, bool hasErrors = false)
			: base(BoundKind.MethodGroup, syntax, receiverOpt, qualificationKind, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(typeArgumentsOpt) || BoundNodeExtensions.NonNullAndHasErrors(receiverOpt))
		{
			_TypeArgumentsOpt = typeArgumentsOpt;
			_Methods = methods;
			_PendingExtensionMethodsOpt = pendingExtensionMethodsOpt;
			_ResultKind = resultKind;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitMethodGroup(this);
		}

		public BoundMethodGroup Update(BoundTypeArguments typeArgumentsOpt, ImmutableArray<MethodSymbol> methods, ExtensionMethodGroup pendingExtensionMethodsOpt, LookupResultKind resultKind, BoundExpression receiverOpt, QualificationKind qualificationKind)
		{
			if (typeArgumentsOpt != TypeArgumentsOpt || methods != Methods || pendingExtensionMethodsOpt != PendingExtensionMethodsOpt || resultKind != ResultKind || receiverOpt != base.ReceiverOpt || qualificationKind != base.QualificationKind)
			{
				BoundMethodGroup boundMethodGroup = new BoundMethodGroup(base.Syntax, typeArgumentsOpt, methods, pendingExtensionMethodsOpt, resultKind, receiverOpt, qualificationKind, base.HasErrors);
				boundMethodGroup.CopyAttributes(this);
				return boundMethodGroup;
			}
			return this;
		}
	}
}
