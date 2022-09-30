using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class SymbolExtensions
	{
		internal const NamespaceKind NamespaceKindNamespaceGroup = (NamespaceKind)0;

		internal static bool IsCompilationOutputWinMdObj(this Symbol symbol)
		{
			VisualBasicCompilation declaringCompilation = symbol.DeclaringCompilation;
			return (declaringCompilation != null) & (declaringCompilation.Options.OutputKind == OutputKind.WindowsRuntimeMetadata);
		}

		internal static string GetKindText(this Symbol target)
		{
			switch (target.Kind)
			{
			case SymbolKind.Namespace:
				return "namespace";
			case SymbolKind.NamedType:
				return ((TypeSymbol)target).TypeKind switch
				{
					TypeKind.Class => "class", 
					TypeKind.Enum => "enum", 
					TypeKind.Interface => "interface", 
					TypeKind.Struct => "structure", 
					TypeKind.Module => "module", 
					TypeKind.Delegate => "delegate Class", 
					_ => "type", 
				};
			case SymbolKind.Field:
			case SymbolKind.Local:
			case SymbolKind.Parameter:
			case SymbolKind.RangeVariable:
				return "variable";
			case SymbolKind.Method:
			{
				MethodSymbol methodSymbol = (MethodSymbol)target;
				MethodKind methodKind = methodSymbol.MethodKind;
				if (methodKind == MethodKind.Conversion || methodKind == MethodKind.UserDefinedOperator || methodKind == MethodKind.BuiltinOperator)
				{
					return "operator";
				}
				if (methodSymbol.IsSub)
				{
					return "sub";
				}
				return "function";
			}
			case SymbolKind.Property:
				if (((PropertySymbol)target).IsWithEvents)
				{
					return "WithEvents variable";
				}
				return "property";
			case SymbolKind.Event:
				return "event";
			default:
				throw ExceptionUtilities.UnexpectedValue(target.Kind);
			}
		}

		internal static string GetPropertyKindText(this PropertySymbol target)
		{
			if (target.IsWriteOnly)
			{
				return SyntaxFacts.GetText(SyntaxKind.WriteOnlyKeyword);
			}
			if (target.IsReadOnly)
			{
				return SyntaxFacts.GetText(SyntaxKind.ReadOnlyKeyword);
			}
			return "";
		}

		internal static object ToErrorMessageArgument(this Symbol target, ERRID errorCode = ERRID.ERR_None)
		{
			if (target.Kind == SymbolKind.Namespace && ((NamespaceSymbol)target).IsGlobalNamespace)
			{
				return "<Default>";
			}
			if (errorCode == ERRID.ERR_TypeConflict6)
			{
				return CustomSymbolDisplayFormatter.DefaultErrorFormat(target);
			}
			return target;
		}

		internal static bool MatchesAnyName(this ImmutableArray<TypeParameterSymbol> @this, string name)
		{
			ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = @this.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeParameterSymbol current = enumerator.Current;
				if (CaseInsensitiveComparison.Comparer.Compare(name, current.Name) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsOverloadable(this Symbol symbol)
		{
			return symbol.Kind switch
			{
				SymbolKind.Method => true, 
				SymbolKind.Property => IsOverloadable((PropertySymbol)symbol), 
				_ => false, 
			};
		}

		public static bool IsOverloadable(this PropertySymbol propertySymbol)
		{
			return !propertySymbol.IsWithEvents;
		}

		internal static bool IsOverloads(this Symbol sym)
		{
			switch (sym.Kind)
			{
			case SymbolKind.Method:
				if (((MethodSymbol)sym).IsOverloads)
				{
					return true;
				}
				break;
			case SymbolKind.Property:
				if (((PropertySymbol)sym).IsOverloads)
				{
					return true;
				}
				break;
			}
			return false;
		}

		internal static bool IsShadows(this Symbol sym)
		{
			return !IsOverloads(sym);
		}

		internal static bool IsInstanceMember(this Symbol sym)
		{
			SymbolKind kind = sym.Kind;
			if ((uint)(kind - 5) <= 1u || kind == SymbolKind.Method || kind == SymbolKind.Property)
			{
				return !sym.IsShared;
			}
			return false;
		}

		internal static bool RequiresImplementation(this Symbol sym)
		{
			SymbolKind kind = sym.Kind;
			if (kind == SymbolKind.Event || kind == SymbolKind.Method || kind == SymbolKind.Property)
			{
				return TypeSymbolExtensions.IsInterfaceType(sym.ContainingType) && !sym.IsShared && !sym.IsNotOverridable && (sym.IsMustOverride || sym.IsOverridable);
			}
			return false;
		}

		internal static bool IsMetadataVirtual(this MethodSymbol method)
		{
			if (method.IsOverridable || method.IsOverrides || method.IsMustOverride || !method.ExplicitInterfaceImplementations.IsEmpty)
			{
				return true;
			}
			MethodSymbol originalDefinition = method.OriginalDefinition;
			return originalDefinition.ContainingSymbol is SourceNamedTypeSymbol sourceNamedTypeSymbol && (object)sourceNamedTypeSymbol.GetCorrespondingComClassInterfaceMethod(originalDefinition) != null;
		}

		public static bool IsAccessor(this MethodSymbol methodSymbol)
		{
			return (object)methodSymbol.AssociatedSymbol != null;
		}

		public static bool IsAccessor(this Symbol symbol)
		{
			if (symbol.Kind == SymbolKind.Method)
			{
				return IsAccessor((MethodSymbol)symbol);
			}
			return false;
		}

		public static bool IsWithEventsProperty(this Symbol symbol)
		{
			if (symbol.Kind == SymbolKind.Property)
			{
				return ((PropertySymbol)symbol).IsWithEvents;
			}
			return false;
		}

		public static bool IsPropertyAndNotWithEvents(this Symbol symbol)
		{
			if (symbol.Kind == SymbolKind.Property)
			{
				return !((PropertySymbol)symbol).IsWithEvents;
			}
			return false;
		}

		internal static bool IsAnyConstructor(this MethodSymbol method)
		{
			MethodKind methodKind = method.MethodKind;
			if (methodKind != MethodKind.Constructor)
			{
				return methodKind == MethodKind.StaticConstructor;
			}
			return true;
		}

		internal static bool IsDefaultValueTypeConstructor(this MethodSymbol method)
		{
			if (method.IsImplicitlyDeclared && method.ContainingType.IsValueType)
			{
				return method.IsParameterlessConstructor();
			}
			return false;
		}

		internal static bool IsReducedExtensionMethod(this Symbol @this)
		{
			if (@this.Kind == SymbolKind.Method)
			{
				return ((MethodSymbol)@this).IsReducedExtensionMethod;
			}
			return false;
		}

		internal static Symbol OverriddenMember(this Symbol sym)
		{
			return sym.Kind switch
			{
				SymbolKind.Method => ((MethodSymbol)sym).OverriddenMethod, 
				SymbolKind.Property => ((PropertySymbol)sym).OverriddenProperty, 
				SymbolKind.Event => ((EventSymbol)sym).OverriddenEvent, 
				_ => null, 
			};
		}

		internal static int GetArity(this Symbol symbol)
		{
			switch (symbol.Kind)
			{
			case SymbolKind.Method:
				return ((MethodSymbol)symbol).Arity;
			case SymbolKind.ErrorType:
			case SymbolKind.NamedType:
				return ((NamedTypeSymbol)symbol).Arity;
			default:
				return 0;
			}
		}

		internal static ParameterSymbol GetMeParameter(this Symbol sym)
		{
			return sym.Kind switch
			{
				SymbolKind.Method => ((MethodSymbol)sym).MeParameter, 
				SymbolKind.Field => ((FieldSymbol)sym).MeParameter, 
				SymbolKind.Property => ((PropertySymbol)sym).MeParameter, 
				SymbolKind.Parameter => null, 
				_ => throw ExceptionUtilities.UnexpectedValue(sym.Kind), 
			};
		}

		internal static NamespaceOrTypeSymbol OfMinimalArity(this IEnumerable<NamespaceOrTypeSymbol> symbols)
		{
			NamespaceOrTypeSymbol result = null;
			int num = int.MaxValue;
			foreach (NamespaceOrTypeSymbol symbol in symbols)
			{
				int arity = GetArity(symbol);
				if (arity < num)
				{
					num = arity;
					result = symbol;
				}
			}
			return result;
		}

		internal static Symbol UnwrapAlias(this Symbol symbol)
		{
			if (symbol is AliasSymbol aliasSymbol)
			{
				return aliasSymbol.Target;
			}
			return symbol;
		}

		internal static bool IsUserDefinedOperator(this Symbol symbol)
		{
			if (symbol.Kind == SymbolKind.Method)
			{
				return MethodSymbolExtensions.IsUserDefinedOperator((MethodSymbol)symbol);
			}
			return false;
		}

		internal static bool IsHiddenByCodeAnalysisEmbeddedAttribute(this Symbol symbol)
		{
			return GetUpperLevelNamedTypeSymbol(symbol)?.HasCodeAnalysisEmbeddedAttribute ?? false;
		}

		internal static bool IsHiddenByVisualBasicEmbeddedAttribute(this Symbol symbol)
		{
			return GetUpperLevelNamedTypeSymbol(symbol)?.HasVisualBasicEmbeddedAttribute ?? false;
		}

		internal static NamedTypeSymbol GetUpperLevelNamedTypeSymbol(this Symbol symbol)
		{
			NamedTypeSymbol namedTypeSymbol = ((symbol.Kind == SymbolKind.NamedType) ? ((NamedTypeSymbol)symbol) : symbol.ContainingType);
			if ((object)namedTypeSymbol == null)
			{
				return null;
			}
			while ((object)namedTypeSymbol.ContainingType != null)
			{
				namedTypeSymbol = namedTypeSymbol.ContainingType;
			}
			return namedTypeSymbol;
		}

		internal static T GetDeclaringSyntaxNode<T>(this Symbol @this) where T : VisualBasicSyntaxNode
		{
			foreach (SyntaxNode item in @this.DeclaringSyntaxReferences.Select((SyntaxReference d) => d.GetSyntax()))
			{
				if (item is T result)
				{
					return result;
				}
			}
			return (T)Symbol.GetDeclaringSyntaxNodeHelper<T>(@this.Locations).FirstOrDefault();
		}

		internal static T AsMember<T>(this T origMember, NamedTypeSymbol type) where T : Symbol
		{
			if ((object)type == origMember.ContainingType)
			{
				return origMember;
			}
			return (T)((SubstitutedNamedType)type).GetMemberForDefinition(origMember);
		}

		internal static TDestination EnsureVbSymbolOrNothing<TSource, TDestination>(this TSource symbol, string paramName) where TSource : ISymbol where TDestination : Symbol, TSource
		{
			TDestination obj = symbol as TDestination;
			if (obj == null && symbol != null)
			{
				throw new ArgumentException(VBResources.NotAVbSymbol, paramName);
			}
			return obj;
		}

		internal static Symbol ContainingNonLambdaMember(this Symbol member)
		{
			while ((object)member != null && member.Kind == SymbolKind.Method && ((MethodSymbol)member).MethodKind == MethodKind.AnonymousFunction)
			{
				member = member.ContainingSymbol;
			}
			return member;
		}

		internal static bool ContainsTupleNames(this Symbol member)
		{
			switch (member.Kind)
			{
			case SymbolKind.Method:
			{
				MethodSymbol methodSymbol = (MethodSymbol)member;
				return TypeSymbolExtensions.ContainsTupleNames(methodSymbol.ReturnType) || ContainsTupleNames(methodSymbol.Parameters);
			}
			case SymbolKind.Property:
			{
				PropertySymbol propertySymbol = (PropertySymbol)member;
				return TypeSymbolExtensions.ContainsTupleNames(propertySymbol.Type) || ContainsTupleNames(propertySymbol.Parameters);
			}
			case SymbolKind.Event:
				return ContainsTupleNames(((EventSymbol)member).DelegateParameters);
			default:
				throw ExceptionUtilities.UnexpectedValue(member.Kind);
			}
		}

		private static bool ContainsTupleNames(ImmutableArray<ParameterSymbol> parameters)
		{
			return parameters.Any((ParameterSymbol p) => TypeSymbolExtensions.ContainsTupleNames(p.Type));
		}
	}
}
