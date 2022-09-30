using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class DocumentationCommentIdVisitor : VisualBasicSymbolVisitor<StringBuilder, object>
	{
		private sealed class PartVisitor : VisualBasicSymbolVisitor<StringBuilder, object>
		{
			public static readonly PartVisitor Instance = new PartVisitor(inParameterOrReturnType: false);

			private static readonly PartVisitor s_parameterOrReturnTypeInstance = new PartVisitor(inParameterOrReturnType: true);

			private readonly bool _inParameterOrReturnType;

			private PartVisitor(bool inParameterOrReturnType)
			{
				_inParameterOrReturnType = inParameterOrReturnType;
			}

			public override object VisitArrayType(ArrayTypeSymbol symbol, StringBuilder builder)
			{
				Visit(symbol.ElementType, builder);
				if (symbol.IsSZArray)
				{
					builder.Append("[]");
				}
				else
				{
					builder.Append("[0:");
					int num = symbol.Rank - 1;
					for (int i = 1; i <= num; i++)
					{
						builder.Append(",0:");
					}
					builder.Append(']');
				}
				return null;
			}

			public override object VisitEvent(EventSymbol symbol, StringBuilder builder)
			{
				Visit(symbol.ContainingType, builder);
				builder.Append('.');
				builder.Append(symbol.Name);
				return null;
			}

			public override object VisitField(FieldSymbol symbol, StringBuilder builder)
			{
				Visit(symbol.ContainingType, builder);
				builder.Append('.');
				builder.Append(symbol.Name);
				return null;
			}

			private void VisitParameters(ImmutableArray<ParameterSymbol> parameters, StringBuilder builder)
			{
				builder.Append('(');
				bool flag = false;
				ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ParameterSymbol current = enumerator.Current;
					if (flag)
					{
						builder.Append(',');
					}
					Visit(current, builder);
					flag = true;
				}
				builder.Append(')');
			}

			public override object VisitMethod(MethodSymbol symbol, StringBuilder builder)
			{
				Visit(symbol.ContainingType, builder);
				builder.Append('.');
				builder.Append(symbol.MetadataName.Replace('.', '#'));
				if (symbol.Arity != 0)
				{
					builder.Append("``");
					builder.Append(symbol.Arity);
				}
				if (symbol.Parameters.Any())
				{
					s_parameterOrReturnTypeInstance.VisitParameters(symbol.Parameters, builder);
				}
				if (symbol.MethodKind == MethodKind.Conversion)
				{
					builder.Append('~');
					s_parameterOrReturnTypeInstance.Visit(symbol.ReturnType, builder);
				}
				return null;
			}

			public override object VisitProperty(PropertySymbol symbol, StringBuilder builder)
			{
				Visit(symbol.ContainingType, builder);
				builder.Append('.');
				builder.Append(symbol.MetadataName);
				if (symbol.Parameters.Any())
				{
					s_parameterOrReturnTypeInstance.VisitParameters(symbol.Parameters, builder);
				}
				return null;
			}

			public override object VisitTypeParameter(TypeParameterSymbol symbol, StringBuilder builder)
			{
				int num = 0;
				Symbol containingSymbol = symbol.ContainingSymbol;
				if (containingSymbol.Kind == SymbolKind.NamedType)
				{
					NamedTypeSymbol containingType = containingSymbol.ContainingType;
					while ((object)containingType != null)
					{
						num += containingType.Arity;
						containingType = containingType.ContainingType;
					}
					builder.Append('`');
				}
				else
				{
					if (containingSymbol.Kind != SymbolKind.Method)
					{
						throw ExceptionUtilities.UnexpectedValue(containingSymbol.Kind);
					}
					builder.Append("``");
				}
				builder.Append(symbol.Ordinal + num);
				return null;
			}

			public override object VisitNamedType(NamedTypeSymbol symbol, StringBuilder builder)
			{
				if (symbol.IsTupleType)
				{
					return VisitNamedType(((TupleTypeSymbol)symbol).UnderlyingNamedType, builder);
				}
				if ((object)symbol.ContainingSymbol != null && symbol.ContainingSymbol.Name.Length != 0)
				{
					Visit(symbol.ContainingSymbol, builder);
					builder.Append('.');
				}
				builder.Append(symbol.Name);
				if (symbol.Arity != 0)
				{
					if (!_inParameterOrReturnType && TypeSymbol.Equals(symbol, symbol.ConstructedFrom, TypeCompareKind.ConsiderEverything))
					{
						builder.Append('`');
						builder.Append(symbol.Arity);
					}
					else
					{
						builder.Append('{');
						bool flag = false;
						ImmutableArray<TypeSymbol>.Enumerator enumerator = symbol.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator();
						while (enumerator.MoveNext())
						{
							TypeSymbol current = enumerator.Current;
							if (flag)
							{
								builder.Append(',');
							}
							Visit(current, builder);
							flag = true;
						}
						builder.Append('}');
					}
				}
				return null;
			}

			public override object VisitNamespace(NamespaceSymbol symbol, StringBuilder builder)
			{
				if ((object)symbol.ContainingNamespace != null && symbol.ContainingNamespace.Name.Length != 0)
				{
					Visit(symbol.ContainingNamespace, builder);
					builder.Append('.');
				}
				builder.Append(symbol.Name);
				return null;
			}

			public override object VisitParameter(ParameterSymbol symbol, StringBuilder builder)
			{
				Visit(symbol.Type, builder);
				if (symbol.IsByRef)
				{
					builder.Append('@');
				}
				return null;
			}

			public override object VisitErrorType(ErrorTypeSymbol symbol, StringBuilder arg)
			{
				return VisitNamedType(symbol, arg);
			}
		}

		public static readonly DocumentationCommentIdVisitor Instance = new DocumentationCommentIdVisitor();

		private DocumentationCommentIdVisitor()
		{
		}

		public override object DefaultVisit(Symbol symbol, StringBuilder builder)
		{
			return null;
		}

		public override object VisitNamespace(NamespaceSymbol symbol, StringBuilder builder)
		{
			return VisitSymbolUsingPrefix(symbol, builder, "N:");
		}

		public override object VisitEvent(EventSymbol symbol, StringBuilder builder)
		{
			return VisitSymbolUsingPrefix(symbol, builder, "E:");
		}

		public override object VisitMethod(MethodSymbol symbol, StringBuilder builder)
		{
			return VisitSymbolUsingPrefix(symbol, builder, "M:");
		}

		public override object VisitField(FieldSymbol symbol, StringBuilder builder)
		{
			return VisitSymbolUsingPrefix(symbol, builder, "F:");
		}

		public override object VisitProperty(PropertySymbol symbol, StringBuilder builder)
		{
			return VisitSymbolUsingPrefix(symbol, builder, "P:");
		}

		public override object VisitNamedType(NamedTypeSymbol symbol, StringBuilder builder)
		{
			return VisitSymbolUsingPrefix(symbol, builder, "T:");
		}

		public override object VisitArrayType(ArrayTypeSymbol symbol, StringBuilder builder)
		{
			return VisitSymbolUsingPrefix(symbol, builder, "T:");
		}

		public override object VisitTypeParameter(TypeParameterSymbol symbol, StringBuilder builder)
		{
			return VisitSymbolUsingPrefix(symbol, builder, "!:");
		}

		public override object VisitErrorType(ErrorTypeSymbol symbol, StringBuilder builder)
		{
			return VisitSymbolUsingPrefix(symbol, builder, "!:");
		}

		private static object VisitSymbolUsingPrefix(Symbol symbol, StringBuilder builder, string prefix)
		{
			builder.Append(prefix);
			PartVisitor.Instance.Visit(symbol, builder);
			return null;
		}
	}
}
