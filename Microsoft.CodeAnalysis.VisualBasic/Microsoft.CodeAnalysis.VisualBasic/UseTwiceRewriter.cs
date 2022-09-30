using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class UseTwiceRewriter
	{
		public struct Result
		{
			public readonly BoundExpression First;

			public readonly BoundExpression Second;

			public Result(BoundExpression first, BoundExpression second)
			{
				this = default(Result);
				First = first;
				Second = second;
			}
		}

		private UseTwiceRewriter()
		{
		}

		public static Result UseTwice(Symbol containingMember, BoundExpression value, ArrayBuilder<SynthesizedLocal> temporaries)
		{
			switch (value.Kind)
			{
			case BoundKind.XmlMemberAccess:
			{
				BoundXmlMemberAccess boundXmlMemberAccess = (BoundXmlMemberAccess)value;
				Result result = UseTwice(containingMember, boundXmlMemberAccess.MemberAccess, temporaries);
				return new Result(BoundExpressionExtensions.Update(boundXmlMemberAccess, result.First), BoundExpressionExtensions.Update(boundXmlMemberAccess, result.Second));
			}
			case BoundKind.PropertyAccess:
				return UseTwicePropertyAccess(containingMember, (BoundPropertyAccess)value, temporaries);
			case BoundKind.LateInvocation:
				return UseTwiceLateInvocation(containingMember, (BoundLateInvocation)value, temporaries);
			case BoundKind.LateMemberAccess:
				return UseTwiceLateMember(containingMember, (BoundLateMemberAccess)value, temporaries);
			default:
				return UseTwiceExpression(containingMember, value, temporaries);
			}
		}

		private static Result UseTwiceLateBoundReceiver(Symbol containingMember, BoundExpression receiverOpt, ArrayBuilder<SynthesizedLocal> temporaries)
		{
			Result result;
			if (receiverOpt == null)
			{
				result = new Result(null, null);
			}
			else if (receiverOpt.IsLValue && receiverOpt.Type.IsReferenceType)
			{
				BoundLocal referToTemp = null;
				BoundAssignmentOperator boundAssignmentOperator = CaptureInATemp(containingMember, receiverOpt.MakeRValue(), temporaries, ref referToTemp);
				referToTemp = referToTemp.Update(referToTemp.LocalSymbol, isLValue: true, referToTemp.Type);
				result = new Result(new BoundSequence(boundAssignmentOperator.Syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundExpression)boundAssignmentOperator), referToTemp, referToTemp.Type), referToTemp);
			}
			else
			{
				if (receiverOpt.IsLValue || receiverOpt.Type.IsReferenceType || receiverOpt.Type.IsValueType)
				{
					return UseTwiceExpression(containingMember, receiverOpt, temporaries);
				}
				BoundLocal referToTemp2 = null;
				BoundAssignmentOperator boundAssignmentOperator2 = CaptureInATemp(containingMember, receiverOpt.MakeRValue(), temporaries, ref referToTemp2);
				referToTemp2 = referToTemp2.Update(referToTemp2.LocalSymbol, isLValue: true, referToTemp2.Type);
				result = new Result(new BoundSequence(boundAssignmentOperator2.Syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundExpression)boundAssignmentOperator2), referToTemp2, referToTemp2.Type), referToTemp2);
			}
			return result;
		}

		private static Result UseTwiceExpression(Symbol containingMember, BoundExpression value, ArrayBuilder<SynthesizedLocal> temporaries)
		{
			if (!value.IsLValue)
			{
				return UseTwiceRValue(containingMember, value, temporaries);
			}
			switch (value.Kind)
			{
			case BoundKind.Call:
				return UseTwiceCall(containingMember, (BoundCall)value, temporaries);
			case BoundKind.ArrayAccess:
				return UseTwiceArrayAccess(containingMember, (BoundArrayAccess)value, temporaries);
			case BoundKind.FieldAccess:
				return UseTwiceFieldAccess(containingMember, (BoundFieldAccess)value, temporaries);
			case BoundKind.WithLValueExpressionPlaceholder:
			case BoundKind.Local:
			case BoundKind.PseudoVariable:
			case BoundKind.Parameter:
				return new Result(value, value);
			default:
				return UseTwiceRValue(containingMember, value, temporaries);
			}
		}

		private static BoundAssignmentOperator CaptureInATemp(Symbol containingMember, BoundExpression value, TypeSymbol type, ArrayBuilder<SynthesizedLocal> temporaries, ref BoundLocal referToTemp)
		{
			SynthesizedLocal synthesizedLocal = new SynthesizedLocal(containingMember, type, SynthesizedLocalKind.LoweringTemp);
			temporaries.Add(synthesizedLocal);
			referToTemp = new BoundLocal(value.Syntax, synthesizedLocal, type);
			referToTemp.SetWasCompilerGenerated();
			BoundAssignmentOperator result = BoundNodeExtensions.MakeCompilerGenerated(new BoundAssignmentOperator(value.Syntax, referToTemp, value, suppressObjectClone: true, type));
			referToTemp = referToTemp.MakeRValue();
			return result;
		}

		private static BoundAssignmentOperator CaptureInATemp(Symbol containingMember, BoundExpression value, ArrayBuilder<SynthesizedLocal> temporaries, ref BoundLocal referToTemp)
		{
			return CaptureInATemp(containingMember, value, value.Type, temporaries, ref referToTemp);
		}

		private static Result UseTwiceRValue(Symbol containingMember, BoundExpression value, ArrayBuilder<SynthesizedLocal> arg)
		{
			BoundKind kind = value.Kind;
			Result result;
			if (kind == BoundKind.BadVariable || kind == BoundKind.MeReference || kind == BoundKind.MyBaseReference || kind == BoundKind.MyClassReference || kind == BoundKind.Literal)
			{
				result = new Result(value, value);
			}
			else
			{
				if (!BoundExpressionExtensions.IsValue(value) || (object)value.Type == null || TypeSymbolExtensions.IsVoidType(value.Type))
				{
					throw ExceptionUtilities.Unreachable;
				}
				ConstantValue constantValueOpt = value.ConstantValueOpt;
				if ((object)constantValueOpt != null)
				{
					BoundLiteral boundLiteral = new BoundLiteral(value.Syntax, constantValueOpt, value.Type);
					boundLiteral.SetWasCompilerGenerated();
					result = new Result(value, boundLiteral);
				}
				else
				{
					BoundLocal referToTemp = null;
					BoundAssignmentOperator first = CaptureInATemp(containingMember, value, arg, ref referToTemp);
					result = new Result(first, referToTemp);
				}
			}
			return result;
		}

		private static Result UseTwiceCall(Symbol containingMember, BoundCall node, ArrayBuilder<SynthesizedLocal> arg)
		{
			return UseTwiceLValue(containingMember, node, arg);
		}

		private static Result UseTwiceArrayAccess(Symbol containingMember, BoundArrayAccess node, ArrayBuilder<SynthesizedLocal> arg)
		{
			if (IsInvariantArray(node.Expression.Type))
			{
				return UseTwiceLValue(containingMember, node, arg);
			}
			BoundLocal referToTemp = null;
			BoundAssignmentOperator expression = CaptureInATemp(containingMember, node.Expression, arg, ref referToTemp);
			int length = node.Indices.Length;
			BoundExpression[] array = new BoundExpression[length - 1 + 1];
			BoundExpression[] array2 = new BoundExpression[length - 1 + 1];
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				Result result = UseTwiceRValue(containingMember, node.Indices[i], arg);
				array[i] = result.First;
				array2[i] = result.Second;
			}
			BoundArrayAccess second = node.Update(referToTemp, array2.AsImmutableOrNull(), node.IsLValue, node.Type);
			BoundArrayAccess first = node.Update(expression, array.AsImmutableOrNull(), node.IsLValue, node.Type);
			return new Result(first, second);
		}

		private static bool IsInvariantArray(TypeSymbol type)
		{
			ArrayTypeSymbol obj = type as ArrayTypeSymbol;
			return (((object)obj != null) ? new bool?(TypeSymbolExtensions.IsNotInheritable(obj.ElementType)) : null).GetValueOrDefault();
		}

		private static Result UseTwiceLValue(Symbol containingMember, BoundExpression lvalue, ArrayBuilder<SynthesizedLocal> temporaries)
		{
			SynthesizedLocal synthesizedLocal = new SynthesizedLocal(containingMember, lvalue.Type, SynthesizedLocalKind.LoweringTemp, null, isByRef: true);
			BoundReferenceAssignment first = BoundNodeExtensions.MakeCompilerGenerated(new BoundReferenceAssignment(lvalue.Syntax, BoundNodeExtensions.MakeCompilerGenerated(new BoundLocal(lvalue.Syntax, synthesizedLocal, synthesizedLocal.Type)), lvalue, isLValue: true, lvalue.Type));
			temporaries.Add(synthesizedLocal);
			BoundLocal second = BoundNodeExtensions.MakeCompilerGenerated(new BoundLocal(lvalue.Syntax, synthesizedLocal, isLValue: true, lvalue.Type));
			return new Result(first, second);
		}

		private static Result UseTwiceFieldAccess(Symbol containingMember, BoundFieldAccess node, ArrayBuilder<SynthesizedLocal> arg)
		{
			FieldSymbol fieldSymbol = node.FieldSymbol;
			Result result;
			if (fieldSymbol.IsShared && node.ReceiverOpt != null)
			{
				BoundFieldAccess second = node.Update(null, fieldSymbol, node.IsLValue, node.SuppressVirtualCalls, null, node.Type);
				result = new Result(node, second);
			}
			else
			{
				if (node.ReceiverOpt != null)
				{
					return UseTwiceLValue(containingMember, node, arg);
				}
				result = new Result(node, node);
			}
			return result;
		}

		private static Result UseTwicePropertyAccess(Symbol containingMember, BoundPropertyAccess node, ArrayBuilder<SynthesizedLocal> arg)
		{
			PropertySymbol propertySymbol = node.PropertySymbol;
			BoundExpression receiverOpt = node.ReceiverOpt;
			Result result;
			if (receiverOpt == null)
			{
				result = new Result(null, null);
			}
			else if (node.PropertySymbol.IsShared)
			{
				result = new Result(receiverOpt, null);
			}
			else if (receiverOpt.IsLValue && receiverOpt.Type.IsReferenceType && !TypeSymbolExtensions.IsTypeParameter(receiverOpt.Type))
			{
				BoundLocal referToTemp = null;
				result = new Result(CaptureInATemp(containingMember, receiverOpt.MakeRValue(), arg, ref referToTemp), referToTemp);
			}
			else if (!receiverOpt.IsLValue && !receiverOpt.Type.IsReferenceType && !receiverOpt.Type.IsValueType)
			{
				BoundLocal referToTemp2 = null;
				BoundAssignmentOperator boundAssignmentOperator = CaptureInATemp(containingMember, receiverOpt.MakeRValue(), arg, ref referToTemp2);
				referToTemp2 = referToTemp2.Update(referToTemp2.LocalSymbol, isLValue: true, referToTemp2.Type);
				result = new Result(new BoundSequence(boundAssignmentOperator.Syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundExpression)boundAssignmentOperator), referToTemp2, referToTemp2.Type), referToTemp2);
			}
			else
			{
				result = UseTwiceExpression(containingMember, receiverOpt, arg);
			}
			ImmutableArray<BoundExpression> arguments;
			ImmutableArray<BoundExpression> arguments2;
			if (node.Arguments.IsEmpty)
			{
				arguments = ImmutableArray<BoundExpression>.Empty;
				arguments2 = ImmutableArray<BoundExpression>.Empty;
			}
			else
			{
				int length = node.Arguments.Length;
				BoundExpression[] array = new BoundExpression[length - 1 + 1];
				BoundExpression[] array2 = new BoundExpression[length - 1 + 1];
				int num = length - 1;
				for (int i = 0; i <= num; i++)
				{
					BoundExpression boundExpression = node.Arguments[i];
					if (boundExpression.Kind == BoundKind.ArrayCreation && ((BoundArrayCreation)boundExpression).IsParamArrayArgument)
					{
						UseTwiceParamArrayArgument(containingMember, (BoundArrayCreation)boundExpression, arg, ref array[i], ref array2[i]);
					}
					else
					{
						UseTwiceRegularArgument(containingMember, boundExpression, arg, ref array[i], ref array2[i]);
					}
				}
				arguments = array.AsImmutableOrNull();
				arguments2 = array2.AsImmutableOrNull();
			}
			BoundPropertyAccess first = node.Update(propertySymbol, node.PropertyGroupOpt, node.AccessKind, node.IsWriteable, node.IsLValue, result.First, arguments, node.DefaultArguments, node.Type);
			BoundPropertyAccess second = node.Update(propertySymbol, node.PropertyGroupOpt, node.AccessKind, node.IsWriteable, node.IsLValue, result.Second, arguments2, node.DefaultArguments, node.Type);
			return new Result(first, second);
		}

		private static Result UseTwiceLateInvocation(Symbol containingMember, BoundLateInvocation node, ArrayBuilder<SynthesizedLocal> arg)
		{
			Result result = ((node.Member.Kind != BoundKind.LateMemberAccess) ? UseTwiceLateBoundReceiver(containingMember, node.Member, arg) : UseTwiceLateMember(containingMember, (BoundLateMemberAccess)node.Member, arg));
			ImmutableArray<BoundExpression> argumentsOpt;
			ImmutableArray<BoundExpression> argumentsOpt2;
			if (node.ArgumentsOpt.IsEmpty)
			{
				argumentsOpt = ImmutableArray<BoundExpression>.Empty;
				argumentsOpt2 = ImmutableArray<BoundExpression>.Empty;
			}
			else
			{
				int length = node.ArgumentsOpt.Length;
				BoundExpression[] array = new BoundExpression[length - 1 + 1];
				BoundExpression[] array2 = new BoundExpression[length - 1 + 1];
				int num = length - 1;
				for (int i = 0; i <= num; i++)
				{
					BoundExpression boundExpression = node.ArgumentsOpt[i];
					if (!BoundExpressionExtensions.IsSupportingAssignment(boundExpression))
					{
						UseTwiceRegularArgument(containingMember, boundExpression, arg, ref array[i], ref array2[i]);
						continue;
					}
					SynthesizedLocal synthesizedLocal = new SynthesizedLocal(containingMember, boundExpression.Type, SynthesizedLocalKind.LoweringTemp);
					arg.Add(synthesizedLocal);
					array[i] = new BoundLateBoundArgumentSupportingAssignmentWithCapture(boundExpression.Syntax, boundExpression, synthesizedLocal, boundExpression.Type);
					array2[i] = new BoundLocal(boundExpression.Syntax, synthesizedLocal, isLValue: false, synthesizedLocal.Type);
				}
				argumentsOpt = array.AsImmutableOrNull();
				argumentsOpt2 = array2.AsImmutableOrNull();
			}
			BoundLateInvocation first = node.Update(result.First, argumentsOpt, node.ArgumentNamesOpt, node.AccessKind, node.MethodOrPropertyGroupOpt, node.Type);
			BoundLateInvocation second = node.Update(result.Second, argumentsOpt2, node.ArgumentNamesOpt, node.AccessKind, node.MethodOrPropertyGroupOpt, node.Type);
			return new Result(first, second);
		}

		private static Result UseTwiceLateMember(Symbol containingMember, BoundLateMemberAccess node, ArrayBuilder<SynthesizedLocal> arg)
		{
			Result result = UseTwiceLateBoundReceiver(containingMember, node.ReceiverOpt, arg);
			BoundLateMemberAccess first = node.Update(node.NameOpt, node.ContainerTypeOpt, result.First, node.TypeArgumentsOpt, node.AccessKind, node.Type);
			BoundLateMemberAccess second = node.Update(node.NameOpt, node.ContainerTypeOpt, result.Second, node.TypeArgumentsOpt, node.AccessKind, node.Type);
			return new Result(first, second);
		}

		private static void UseTwiceRegularArgument(Symbol containingMember, BoundExpression boundArgument, ArrayBuilder<SynthesizedLocal> arg, ref BoundExpression first, ref BoundExpression second)
		{
			Result result = UseTwiceRValue(containingMember, boundArgument, arg);
			first = result.First;
			second = result.Second;
		}

		private static void UseTwiceParamArrayArgument(Symbol containingMember, BoundArrayCreation boundArray, ArrayBuilder<SynthesizedLocal> arg, ref BoundExpression first, ref BoundExpression second)
		{
			BoundArrayInitialization initializerOpt = boundArray.InitializerOpt;
			int length = initializerOpt.Initializers.Length;
			BoundExpression[] array = new BoundExpression[length - 1 + 1];
			BoundExpression[] array2 = new BoundExpression[length - 1 + 1];
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				UseTwiceRegularArgument(containingMember, initializerOpt.Initializers[i], arg, ref array[i], ref array2[i]);
			}
			first = boundArray.Update(boundArray.IsParamArrayArgument, boundArray.Bounds, initializerOpt.Update(array.AsImmutableOrNull(), initializerOpt.Type), null, ConversionKind.DelegateRelaxationLevelNone, boundArray.Type);
			second = boundArray.Update(boundArray.IsParamArrayArgument, boundArray.Bounds, initializerOpt.Update(array2.AsImmutableOrNull(), initializerOpt.Type), null, ConversionKind.DelegateRelaxationLevelNone, boundArray.Type);
		}
	}
}
