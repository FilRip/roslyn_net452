using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundAnonymousTypePropertyAccess : BoundExpression
	{
		private readonly Lazy<PropertySymbol> _lazyPropertySymbol;

		private readonly Binder.AnonymousTypeCreationBinder _Binder;

		private readonly int _PropertyIndex;

		public override Symbol ExpressionSymbol => _lazyPropertySymbol.Value;

		public Binder.AnonymousTypeCreationBinder Binder => _Binder;

		public int PropertyIndex => _PropertyIndex;

		private PropertySymbol LazyGetProperty()
		{
			return Binder.GetAnonymousTypePropertySymbol(PropertyIndex);
		}

		public BoundAnonymousTypePropertyAccess(SyntaxNode syntax, Binder.AnonymousTypeCreationBinder binder, int propertyIndex, TypeSymbol type, bool hasErrors)
			: base(BoundKind.AnonymousTypePropertyAccess, syntax, type, hasErrors)
		{
			_lazyPropertySymbol = new Lazy<PropertySymbol>(LazyGetProperty);
			_Binder = binder;
			_PropertyIndex = propertyIndex;
		}

		public BoundAnonymousTypePropertyAccess(SyntaxNode syntax, Binder.AnonymousTypeCreationBinder binder, int propertyIndex, TypeSymbol type)
			: base(BoundKind.AnonymousTypePropertyAccess, syntax, type)
		{
			_lazyPropertySymbol = new Lazy<PropertySymbol>(LazyGetProperty);
			_Binder = binder;
			_PropertyIndex = propertyIndex;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitAnonymousTypePropertyAccess(this);
		}

		public BoundAnonymousTypePropertyAccess Update(Binder.AnonymousTypeCreationBinder binder, int propertyIndex, TypeSymbol type)
		{
			if (binder != Binder || propertyIndex != PropertyIndex || (object)type != base.Type)
			{
				BoundAnonymousTypePropertyAccess boundAnonymousTypePropertyAccess = new BoundAnonymousTypePropertyAccess(base.Syntax, binder, propertyIndex, type, base.HasErrors);
				boundAnonymousTypePropertyAccess.CopyAttributes(this);
				return boundAnonymousTypePropertyAccess;
			}
			return this;
		}
	}
}
