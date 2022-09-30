using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class VisualBasicSymbolVisitor
	{
		public virtual void Visit(Symbol symbol)
		{
			symbol?.Accept(this);
		}

		public virtual void DefaultVisit(Symbol symbol)
		{
		}

		public virtual void VisitAlias(AliasSymbol symbol)
		{
			DefaultVisit(symbol);
		}

		public virtual void VisitArrayType(ArrayTypeSymbol symbol)
		{
			DefaultVisit(symbol);
		}

		public virtual void VisitAssembly(AssemblySymbol symbol)
		{
			DefaultVisit(symbol);
		}

		public virtual void VisitEvent(EventSymbol symbol)
		{
			DefaultVisit(symbol);
		}

		public virtual void VisitField(FieldSymbol symbol)
		{
			DefaultVisit(symbol);
		}

		public virtual void VisitLabel(LabelSymbol symbol)
		{
			DefaultVisit(symbol);
		}

		public virtual void VisitLocal(LocalSymbol symbol)
		{
			DefaultVisit(symbol);
		}

		public virtual void VisitMethod(MethodSymbol symbol)
		{
			DefaultVisit(symbol);
		}

		public virtual void VisitModule(ModuleSymbol symbol)
		{
			DefaultVisit(symbol);
		}

		public virtual void VisitNamedType(NamedTypeSymbol symbol)
		{
			DefaultVisit(symbol);
		}

		public virtual void VisitNamespace(NamespaceSymbol symbol)
		{
			DefaultVisit(symbol);
		}

		public virtual void VisitParameter(ParameterSymbol symbol)
		{
			DefaultVisit(symbol);
		}

		public virtual void VisitProperty(PropertySymbol symbol)
		{
			DefaultVisit(symbol);
		}

		public virtual void VisitRangeVariable(RangeVariableSymbol symbol)
		{
			DefaultVisit(symbol);
		}

		public virtual void VisitTypeParameter(TypeParameterSymbol symbol)
		{
			DefaultVisit(symbol);
		}
	}
	internal abstract class VisualBasicSymbolVisitor<TResult>
	{
		public virtual TResult Visit(Symbol symbol)
		{
			if ((object)symbol != null)
			{
				return symbol.Accept(this);
			}
			return default(TResult);
		}

		public virtual TResult DefaultVisit(Symbol symbol)
		{
			return default(TResult);
		}

		public virtual TResult VisitAlias(AliasSymbol symbol)
		{
			return DefaultVisit(symbol);
		}

		public virtual TResult VisitArrayType(ArrayTypeSymbol symbol)
		{
			return DefaultVisit(symbol);
		}

		public virtual TResult VisitAssembly(AssemblySymbol symbol)
		{
			return DefaultVisit(symbol);
		}

		public virtual TResult VisitEvent(EventSymbol symbol)
		{
			return DefaultVisit(symbol);
		}

		public virtual TResult VisitField(FieldSymbol symbol)
		{
			return DefaultVisit(symbol);
		}

		public virtual TResult VisitLabel(LabelSymbol symbol)
		{
			return DefaultVisit(symbol);
		}

		public virtual TResult VisitLocal(LocalSymbol symbol)
		{
			return DefaultVisit(symbol);
		}

		public virtual TResult VisitMethod(MethodSymbol symbol)
		{
			return DefaultVisit(symbol);
		}

		public virtual TResult VisitModule(ModuleSymbol symbol)
		{
			return DefaultVisit(symbol);
		}

		public virtual TResult VisitNamedType(NamedTypeSymbol symbol)
		{
			return DefaultVisit(symbol);
		}

		public virtual TResult VisitNamespace(NamespaceSymbol symbol)
		{
			return DefaultVisit(symbol);
		}

		public virtual TResult VisitParameter(ParameterSymbol symbol)
		{
			return DefaultVisit(symbol);
		}

		public virtual TResult VisitProperty(PropertySymbol symbol)
		{
			return DefaultVisit(symbol);
		}

		public virtual TResult VisitRangeVariable(RangeVariableSymbol symbol)
		{
			return DefaultVisit(symbol);
		}

		public virtual TResult VisitTypeParameter(TypeParameterSymbol symbol)
		{
			return DefaultVisit(symbol);
		}
	}
	internal abstract class VisualBasicSymbolVisitor<TArgument, TResult>
	{
		public virtual TResult Visit(Symbol symbol, TArgument arg = default(TArgument))
		{
			if ((object)symbol == null)
			{
				return default(TResult);
			}
			return symbol.Accept(this, arg);
		}

		public virtual TResult DefaultVisit(Symbol symbol, TArgument arg)
		{
			return default(TResult);
		}

		public virtual TResult VisitAlias(AliasSymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}

		public virtual TResult VisitAssembly(AssemblySymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}

		public virtual TResult VisitModule(ModuleSymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}

		public virtual TResult VisitNamespace(NamespaceSymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}

		public virtual TResult VisitNamedType(NamedTypeSymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}

		public virtual TResult VisitTypeParameter(TypeParameterSymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}

		public virtual TResult VisitArrayType(ArrayTypeSymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}

		public virtual TResult VisitErrorType(ErrorTypeSymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}

		public virtual TResult VisitMethod(MethodSymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}

		public virtual TResult VisitProperty(PropertySymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}

		public virtual TResult VisitField(FieldSymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}

		public virtual TResult VisitParameter(ParameterSymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}

		public virtual TResult VisitLocal(LocalSymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}

		public virtual TResult VisitRangeVariable(RangeVariableSymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}

		public virtual TResult VisitLabel(LabelSymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}

		public virtual TResult VisitEvent(EventSymbol symbol, TArgument arg)
		{
			return DefaultVisit(symbol, arg);
		}
	}
}
