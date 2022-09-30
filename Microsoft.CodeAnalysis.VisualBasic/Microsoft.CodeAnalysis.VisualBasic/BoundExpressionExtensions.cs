using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class BoundExpressionExtensions
	{
		public static bool IsDefaultValue(this BoundExpression node)
		{
			ConstantValue constantValueOpt = node.ConstantValueOpt;
			if ((object)constantValueOpt != null && constantValueOpt.IsDefaultValue)
			{
				return true;
			}
			switch (node.Kind)
			{
			case BoundKind.Conversion:
				return ((BoundConversion)node).Operand.ConstantValueOpt?.IsNothing ?? false;
			case BoundKind.DirectCast:
				if (TypeSymbolExtensions.IsTypeParameter(node.Type) || !node.Type.IsValueType)
				{
					return ((BoundDirectCast)node).Operand.ConstantValueOpt?.IsNothing ?? false;
				}
				break;
			case BoundKind.TryCast:
				return ((BoundTryCast)node).Operand.ConstantValueOpt?.IsNothing ?? false;
			case BoundKind.ObjectCreationExpression:
			{
				MethodSymbol constructorOpt = ((BoundObjectCreationExpression)node).ConstructorOpt;
				return (object)constructorOpt == null || SymbolExtensions.IsDefaultValueTypeConstructor(constructorOpt);
			}
			}
			return false;
		}

		public static bool IsValue(this BoundExpression node)
		{
			switch (node.Kind)
			{
			case BoundKind.Parenthesized:
				return IsValue(((BoundParenthesized)node).Expression);
			case BoundKind.BadExpression:
				return (object)node.Type != null;
			case BoundKind.TypeArguments:
			case BoundKind.TypeExpression:
			case BoundKind.NamespaceExpression:
			case BoundKind.MethodGroup:
			case BoundKind.PropertyGroup:
			case BoundKind.ArrayInitialization:
			case BoundKind.EventAccess:
			case BoundKind.Label:
				return false;
			default:
				return true;
			}
		}

		public static bool IsMeReference(this BoundExpression node)
		{
			return node.Kind == BoundKind.MeReference;
		}

		public static bool IsMyBaseReference(this BoundExpression node)
		{
			return node.Kind == BoundKind.MyBaseReference;
		}

		public static bool IsMyClassReference(this BoundExpression node)
		{
			return node.Kind == BoundKind.MyClassReference;
		}

		public static bool IsInstanceReference(this BoundExpression node)
		{
			if (!IsMeReference(node) && !IsMyBaseReference(node))
			{
				return IsMyClassReference(node);
			}
			return true;
		}

		public static bool IsPropertyOrXmlPropertyAccess(this BoundExpression node)
		{
			return node.Kind switch
			{
				BoundKind.XmlMemberAccess => IsPropertyOrXmlPropertyAccess(((BoundXmlMemberAccess)node).MemberAccess), 
				BoundKind.PropertyAccess => true, 
				_ => false, 
			};
		}

		public static bool IsPropertyReturnsByRef(this BoundExpression node)
		{
			if (node.Kind == BoundKind.PropertyAccess)
			{
				return ((BoundPropertyAccess)node).PropertySymbol.ReturnsByRef;
			}
			return false;
		}

		public static bool IsLateBound(this BoundExpression node)
		{
			BoundKind kind = node.Kind;
			if (kind - 59 <= BoundKind.OmittedArgument)
			{
				return true;
			}
			return false;
		}

		public static TypeSymbol GetTypeOfAssignmentTarget(this BoundExpression node)
		{
			if (node.Kind == BoundKind.PropertyAccess)
			{
				return PropertySymbolExtensions.GetTypeFromSetMethod(((BoundPropertyAccess)node).PropertySymbol);
			}
			return node.Type;
		}

		public static PropertySymbol GetPropertyOrXmlProperty(this BoundExpression node)
		{
			return node.Kind switch
			{
				BoundKind.XmlMemberAccess => GetPropertyOrXmlProperty(((BoundXmlMemberAccess)node).MemberAccess), 
				BoundKind.PropertyAccess => ((BoundPropertyAccess)node).PropertySymbol, 
				_ => null, 
			};
		}

		public static bool IsPropertySupportingAssignment(this BoundExpression node)
		{
			switch (node.Kind)
			{
			case BoundKind.XmlMemberAccess:
				return IsPropertySupportingAssignment(((BoundXmlMemberAccess)node).MemberAccess);
			case BoundKind.PropertyAccess:
			{
				BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)node;
				if (boundPropertyAccess.AccessKind == PropertyAccessKind.Get)
				{
					return false;
				}
				return boundPropertyAccess.IsWriteable;
			}
			default:
				return false;
			}
		}

		public static bool IsSupportingAssignment(this BoundExpression node)
		{
			if (node == null)
			{
				return false;
			}
			if (node.IsLValue)
			{
				return true;
			}
			switch (node.Kind)
			{
			case BoundKind.LateMemberAccess:
			{
				BoundLateMemberAccess boundLateMemberAccess = (BoundLateMemberAccess)node;
				return boundLateMemberAccess.AccessKind != LateBoundAccessKind.Get && boundLateMemberAccess.AccessKind != LateBoundAccessKind.Call;
			}
			case BoundKind.LateInvocation:
			{
				BoundLateInvocation boundLateInvocation = (BoundLateInvocation)node;
				if (boundLateInvocation.AccessKind == LateBoundAccessKind.Unknown)
				{
					BoundMethodOrPropertyGroup methodOrPropertyGroupOpt = boundLateInvocation.MethodOrPropertyGroupOpt;
					if (methodOrPropertyGroupOpt != null && methodOrPropertyGroupOpt.Kind == BoundKind.MethodGroup)
					{
						return false;
					}
				}
				return boundLateInvocation.AccessKind != LateBoundAccessKind.Get && boundLateInvocation.AccessKind != LateBoundAccessKind.Call;
			}
			case BoundKind.LateBoundArgumentSupportingAssignmentWithCapture:
				return true;
			default:
				return IsPropertySupportingAssignment(node);
			}
		}

		public static PropertyAccessKind GetAccessKind(this BoundExpression node)
		{
			return node.Kind switch
			{
				BoundKind.XmlMemberAccess => GetAccessKind(((BoundXmlMemberAccess)node).MemberAccess), 
				BoundKind.PropertyAccess => ((BoundPropertyAccess)node).AccessKind, 
				_ => throw ExceptionUtilities.UnexpectedValue(node.Kind), 
			};
		}

		public static LateBoundAccessKind GetLateBoundAccessKind(this BoundExpression node)
		{
			return node.Kind switch
			{
				BoundKind.LateMemberAccess => ((BoundLateMemberAccess)node).AccessKind, 
				BoundKind.LateInvocation => ((BoundLateInvocation)node).AccessKind, 
				_ => throw ExceptionUtilities.UnexpectedValue(node.Kind), 
			};
		}

		public static BoundExpression SetAccessKind(this BoundExpression node, PropertyAccessKind newAccessKind)
		{
			return node.Kind switch
			{
				BoundKind.XmlMemberAccess => SetAccessKind((BoundXmlMemberAccess)node, newAccessKind), 
				BoundKind.PropertyAccess => ((BoundPropertyAccess)node).SetAccessKind(newAccessKind), 
				_ => throw ExceptionUtilities.UnexpectedValue(node.Kind), 
			};
		}

		public static BoundExpression SetLateBoundAccessKind(this BoundExpression node, LateBoundAccessKind newAccessKind)
		{
			return node.Kind switch
			{
				BoundKind.LateMemberAccess => ((BoundLateMemberAccess)node).SetAccessKind(newAccessKind), 
				BoundKind.LateInvocation => ((BoundLateInvocation)node).SetAccessKind(newAccessKind), 
				_ => throw ExceptionUtilities.UnexpectedValue(node.Kind), 
			};
		}

		public static BoundXmlMemberAccess SetAccessKind(this BoundXmlMemberAccess node, PropertyAccessKind newAccessKind)
		{
			BoundExpression memberAccess = SetAccessKind(node.MemberAccess, newAccessKind);
			return Update(node, memberAccess);
		}

		public static BoundExpression SetGetSetAccessKindIfAppropriate(this BoundExpression node)
		{
			switch (node.Kind)
			{
			case BoundKind.XmlMemberAccess:
				return SetAccessKind((BoundXmlMemberAccess)node, PropertyAccessKind.Get | PropertyAccessKind.Set);
			case BoundKind.PropertyAccess:
			{
				BoundPropertyAccess obj = (BoundPropertyAccess)node;
				PropertyAccessKind accessKind = (obj.PropertySymbol.ReturnsByRef ? PropertyAccessKind.Get : (PropertyAccessKind.Get | PropertyAccessKind.Set));
				return obj.SetAccessKind(accessKind);
			}
			case BoundKind.LateMemberAccess:
				return ((BoundLateMemberAccess)node).SetAccessKind(LateBoundAccessKind.Get | LateBoundAccessKind.Set);
			case BoundKind.LateInvocation:
				return ((BoundLateInvocation)node).SetAccessKind(LateBoundAccessKind.Get | LateBoundAccessKind.Set);
			default:
				return node;
			}
		}

		public static BoundXmlMemberAccess Update(this BoundXmlMemberAccess node, BoundExpression memberAccess)
		{
			return node.Update(memberAccess, memberAccess.Type);
		}

		public static bool IsIntegerZeroLiteral(this BoundExpression node)
		{
			while (node.Kind == BoundKind.Parenthesized)
			{
				node = ((BoundParenthesized)node).Expression;
			}
			if (node.Kind == BoundKind.Literal)
			{
				return IsIntegerZeroLiteral((BoundLiteral)node);
			}
			return false;
		}

		public static bool IsIntegerZeroLiteral(this BoundLiteral node)
		{
			if (node.Value.Discriminator == ConstantValueTypeDiscriminator.Int32 && node.Type.SpecialType == SpecialType.System_Int32)
			{
				return node.Value.Int32Value == 0;
			}
			return false;
		}

		public static bool IsDefaultValueConstant(this BoundExpression expr)
		{
			return expr.ConstantValueOpt?.IsDefaultValue ?? false;
		}

		public static bool IsTrueConstant(this BoundExpression expr)
		{
			return (object)expr.ConstantValueOpt == ConstantValue.True;
		}

		public static bool IsFalseConstant(this BoundExpression expr)
		{
			return (object)expr.ConstantValueOpt == ConstantValue.False;
		}

		public static bool IsNegativeIntegerConstant(this BoundExpression expression)
		{
			int? integerConstantValue = GetIntegerConstantValue(expression);
			if ((integerConstantValue.HasValue ? new bool?(integerConstantValue.GetValueOrDefault() < 0) : null).GetValueOrDefault())
			{
				return true;
			}
			return false;
		}

		public static int? GetIntegerConstantValue(this BoundExpression expression)
		{
			if (expression.HasErrors || !expression.IsConstant)
			{
				goto IL_00ae;
			}
			switch (expression.Type.SpecialType)
			{
			case SpecialType.System_Int16:
				break;
			case SpecialType.System_Int32:
				goto IL_0055;
			case SpecialType.System_Int64:
				goto IL_0069;
			default:
				goto IL_00ae;
			}
			int? result = expression.ConstantValueOpt.Int16Value;
			goto IL_00b6;
			IL_00b6:
			return result;
			IL_0055:
			result = expression.ConstantValueOpt.Int32Value;
			goto IL_00b6;
			IL_0069:
			result = ((expression.ConstantValueOpt.Int64Value > int.MaxValue || expression.ConstantValueOpt.Int64Value < int.MinValue) ? null : new int?((int)expression.ConstantValueOpt.Int64Value));
			goto IL_00b6;
			IL_00ae:
			result = null;
			goto IL_00b6;
		}

		public static bool IsNothingLiteral(this BoundExpression node)
		{
			TypeSymbol type = node.Type;
			if ((object)type == null || type.SpecialType == SpecialType.System_Object)
			{
				ConstantValue constantValueOpt = node.ConstantValueOpt;
				if ((object)constantValueOpt != null && constantValueOpt.IsNothing)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsNothingLiteral(this BoundLiteral node)
		{
			if (node.Value.IsNothing)
			{
				return (object)node.Type == null;
			}
			return false;
		}

		public static bool IsStrictNothingLiteral(this BoundExpression node)
		{
			if (!IsNothingLiteral(node))
			{
				return false;
			}
			while (true)
			{
				switch (node.Kind)
				{
				case BoundKind.Literal:
					return IsNothingLiteral((BoundLiteral)node);
				case BoundKind.Parenthesized:
					node = ((BoundParenthesized)node).Expression;
					break;
				case BoundKind.Conversion:
				{
					BoundConversion boundConversion = (BoundConversion)node;
					ConstantValue constantValueOpt = boundConversion.ConstantValueOpt;
					if (boundConversion.ExplicitCastInCode || (object)constantValueOpt == null || !constantValueOpt.IsNothing)
					{
						return false;
					}
					node = boundConversion.Operand;
					break;
				}
				default:
					return false;
				}
			}
		}

		public static BoundExpression GetMostEnclosedParenthesizedExpression(this BoundExpression expression)
		{
			while (expression.Kind == BoundKind.Parenthesized)
			{
				expression = ((BoundParenthesized)expression).Expression;
			}
			return expression;
		}

		public static bool HasExpressionSymbols(this BoundExpression node)
		{
			switch (node.Kind)
			{
			case BoundKind.TypeExpression:
			case BoundKind.NamespaceExpression:
			case BoundKind.Conversion:
			case BoundKind.MethodGroup:
			case BoundKind.PropertyGroup:
			case BoundKind.Call:
			case BoundKind.ObjectCreationExpression:
			case BoundKind.FieldAccess:
			case BoundKind.PropertyAccess:
			case BoundKind.EventAccess:
			case BoundKind.Local:
			case BoundKind.RangeVariable:
				return true;
			case BoundKind.BadExpression:
				return ((BoundBadExpression)node).Symbols.Length > 0;
			default:
				return false;
			}
		}

		public static void GetExpressionSymbols(this BoundMethodGroup methodGroup, ArrayBuilder<Symbol> symbols)
		{
			int num = 0;
			if (methodGroup.TypeArgumentsOpt != null)
			{
				num = methodGroup.TypeArgumentsOpt.Arguments.Length;
			}
			ImmutableArray<MethodSymbol>.Enumerator enumerator = methodGroup.Methods.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MethodSymbol current = enumerator.Current;
				if (num == 0)
				{
					symbols.Add(current);
				}
				else if (num == current.Arity)
				{
					symbols.Add(current.Construct(methodGroup.TypeArgumentsOpt.Arguments));
				}
			}
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
			ImmutableArray<MethodSymbol>.Enumerator enumerator2 = methodGroup.AdditionalExtensionMethods(ref useSiteInfo).GetEnumerator();
			while (enumerator2.MoveNext())
			{
				MethodSymbol current2 = enumerator2.Current;
				if (num == 0)
				{
					symbols.Add(current2);
				}
				else if (num == current2.Arity)
				{
					symbols.Add(current2.Construct(methodGroup.TypeArgumentsOpt.Arguments));
				}
			}
		}

		public static void GetExpressionSymbols(this BoundExpression node, ArrayBuilder<Symbol> symbols)
		{
			switch (node.Kind)
			{
			case BoundKind.MethodGroup:
				GetExpressionSymbols((BoundMethodGroup)node, symbols);
				return;
			case BoundKind.PropertyGroup:
				symbols.AddRange(((BoundPropertyGroup)node).Properties);
				return;
			case BoundKind.BadExpression:
				symbols.AddRange(((BoundBadExpression)node).Symbols);
				return;
			case BoundKind.QueryClause:
				GetExpressionSymbols(((BoundQueryClause)node).UnderlyingExpression, symbols);
				return;
			case BoundKind.AggregateClause:
				GetExpressionSymbols(((BoundAggregateClause)node).UnderlyingExpression, symbols);
				return;
			case BoundKind.Ordering:
				GetExpressionSymbols(((BoundOrdering)node).UnderlyingExpression, symbols);
				return;
			case BoundKind.QuerySource:
				GetExpressionSymbols(((BoundQuerySource)node).Expression, symbols);
				return;
			case BoundKind.ToQueryableCollectionConversion:
				GetExpressionSymbols(((BoundToQueryableCollectionConversion)node).ConversionCall, symbols);
				return;
			case BoundKind.QueryableSource:
				GetExpressionSymbols(((BoundQueryableSource)node).Source, symbols);
				return;
			}
			Symbol expressionSymbol = node.ExpressionSymbol;
			if ((object)expressionSymbol != null)
			{
				if (expressionSymbol.Kind == SymbolKind.Namespace && ((NamespaceSymbol)expressionSymbol).NamespaceKind == (NamespaceKind)0)
				{
					symbols.AddRange(((NamespaceSymbol)expressionSymbol).ConstituentNamespaces);
				}
				else
				{
					symbols.Add(expressionSymbol);
				}
			}
		}

		public static BoundExpressionStatement ToStatement(this BoundExpression node)
		{
			return new BoundExpressionStatement(node.Syntax, node, node.HasErrors);
		}

		[Conditional("DEBUG")]
		public static void AssertRValue(this BoundExpression node)
		{
			_ = node.HasErrors;
		}

		internal static BoundTypeArguments TypeArguments(this BoundMethodOrPropertyGroup @this)
		{
			if (@this is BoundMethodGroup boundMethodGroup)
			{
				return boundMethodGroup.TypeArgumentsOpt;
			}
			return null;
		}
	}
}
