using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundPropertyAccess : BoundExpression
	{
		private readonly PropertySymbol _PropertySymbol;

		private readonly BoundPropertyGroup _PropertyGroupOpt;

		private readonly PropertyAccessKind _AccessKind;

		private readonly bool _IsWriteable;

		private readonly bool _IsLValue;

		private readonly BoundExpression _ReceiverOpt;

		private readonly ImmutableArray<BoundExpression> _Arguments;

		private readonly BitVector _DefaultArguments;

		public override Symbol ExpressionSymbol => PropertySymbol;

		public override LookupResultKind ResultKind
		{
			get
			{
				if (PropertyGroupOpt != null)
				{
					return PropertyGroupOpt.ResultKind;
				}
				return base.ResultKind;
			}
		}

		public PropertySymbol PropertySymbol => _PropertySymbol;

		public BoundPropertyGroup PropertyGroupOpt => _PropertyGroupOpt;

		public PropertyAccessKind AccessKind => _AccessKind;

		public bool IsWriteable => _IsWriteable;

		public override bool IsLValue => _IsLValue;

		public BoundExpression ReceiverOpt => _ReceiverOpt;

		public ImmutableArray<BoundExpression> Arguments => _Arguments;

		public BitVector DefaultArguments => _DefaultArguments;

		public BoundPropertyAccess(SyntaxNode syntax, PropertySymbol propertySymbol, BoundPropertyGroup propertyGroupOpt, PropertyAccessKind accessKind, bool isWriteable, BoundExpression receiverOpt, ImmutableArray<BoundExpression> arguments, BitVector defaultArguments = default(BitVector), bool hasErrors = false)
			: this(syntax, propertySymbol, propertyGroupOpt, accessKind, isWriteable, propertySymbol.ReturnsByRef, receiverOpt, arguments, defaultArguments, GetTypeFromAccessKind(propertySymbol, accessKind), hasErrors)
		{
		}

		public BoundPropertyAccess SetAccessKind(PropertyAccessKind newAccessKind)
		{
			return Update(PropertySymbol, PropertyGroupOpt, newAccessKind, IsWriteable, IsLValue, ReceiverOpt, Arguments, DefaultArguments, GetTypeFromAccessKind(PropertySymbol, newAccessKind));
		}

		protected override BoundExpression MakeRValueImpl()
		{
			return MakeRValue();
		}

		public new BoundPropertyAccess MakeRValue()
		{
			if (_IsLValue)
			{
				return Update(PropertySymbol, PropertyGroupOpt, PropertyAccessKind.Get, IsWriteable, isLValue: false, ReceiverOpt, Arguments, DefaultArguments, base.Type);
			}
			return this;
		}

		private static TypeSymbol GetTypeFromAccessKind(PropertySymbol property, PropertyAccessKind accessKind)
		{
			if ((accessKind & PropertyAccessKind.Set) == 0)
			{
				return PropertySymbolExtensions.GetTypeFromGetMethod(property);
			}
			return PropertySymbolExtensions.GetTypeFromSetMethod(property);
		}

		public BoundPropertyAccess(SyntaxNode syntax, PropertySymbol propertySymbol, BoundPropertyGroup propertyGroupOpt, PropertyAccessKind accessKind, bool isWriteable, bool isLValue, BoundExpression receiverOpt, ImmutableArray<BoundExpression> arguments, BitVector defaultArguments, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.PropertyAccess, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(propertyGroupOpt) || BoundNodeExtensions.NonNullAndHasErrors(receiverOpt) || BoundNodeExtensions.NonNullAndHasErrors(arguments))
		{
			_PropertySymbol = propertySymbol;
			_PropertyGroupOpt = propertyGroupOpt;
			_AccessKind = accessKind;
			_IsWriteable = isWriteable;
			_IsLValue = isLValue;
			_ReceiverOpt = receiverOpt;
			_Arguments = arguments;
			_DefaultArguments = defaultArguments;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitPropertyAccess(this);
		}

		public BoundPropertyAccess Update(PropertySymbol propertySymbol, BoundPropertyGroup propertyGroupOpt, PropertyAccessKind accessKind, bool isWriteable, bool isLValue, BoundExpression receiverOpt, ImmutableArray<BoundExpression> arguments, BitVector defaultArguments, TypeSymbol type)
		{
			if ((object)propertySymbol != PropertySymbol || propertyGroupOpt != PropertyGroupOpt || accessKind != AccessKind || isWriteable != IsWriteable || isLValue != IsLValue || receiverOpt != ReceiverOpt || arguments != Arguments || defaultArguments != DefaultArguments || (object)type != base.Type)
			{
				BoundPropertyAccess boundPropertyAccess = new BoundPropertyAccess(base.Syntax, propertySymbol, propertyGroupOpt, accessKind, isWriteable, isLValue, receiverOpt, arguments, defaultArguments, type, base.HasErrors);
				boundPropertyAccess.CopyAttributes(this);
				return boundPropertyAccess;
			}
			return this;
		}
	}
}
