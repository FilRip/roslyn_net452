using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.CodeGen;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class ExpressionLambdaRewriter
	{
		private enum ConversionSemantics
		{
			Default,
			DirectCast,
			TryCast
		}

		private readonly Binder _binder;

		private readonly SyntheticBoundNodeFactory _factory;

		private readonly TypeSubstitution _typeMap;

		private readonly NamedTypeSymbol _expressionType;

		private NamedTypeSymbol _int32Type;

		private NamedTypeSymbol _objectType;

		private NamedTypeSymbol _memberInfoType;

		private NamedTypeSymbol _memberBindingType;

		private NamedTypeSymbol _elementInitType;

		private readonly Dictionary<ParameterSymbol, BoundExpression> _parameterMap;

		private int _recursionDepth;

		private const string s_coalesceLambdaParameterName = "CoalesceLHS";

		public NamedTypeSymbol ElementInitType
		{
			get
			{
				if ((object)_elementInitType == null)
				{
					_elementInitType = _factory.WellKnownType(WellKnownType.System_Linq_Expressions_ElementInit);
				}
				return _elementInitType;
			}
		}

		public NamedTypeSymbol MemberBindingType
		{
			get
			{
				if ((object)_memberBindingType == null)
				{
					_memberBindingType = _factory.WellKnownType(WellKnownType.System_Linq_Expressions_MemberBinding);
				}
				return _memberBindingType;
			}
		}

		public NamedTypeSymbol MemberInfoType
		{
			get
			{
				if ((object)_memberInfoType == null)
				{
					_memberInfoType = _factory.WellKnownType(WellKnownType.System_Reflection_MemberInfo);
				}
				return _memberInfoType;
			}
		}

		public NamedTypeSymbol Int32Type
		{
			get
			{
				if ((object)_int32Type == null)
				{
					_int32Type = _factory.SpecialType(SpecialType.System_Int32);
				}
				return _int32Type;
			}
		}

		public NamedTypeSymbol ObjectType
		{
			get
			{
				if ((object)_objectType == null)
				{
					_objectType = _factory.SpecialType(SpecialType.System_Object);
				}
				return _objectType;
			}
		}

		private BindingDiagnosticBag Diagnostics => _factory.Diagnostics;

		private ExpressionLambdaRewriter(MethodSymbol currentMethod, TypeCompilationState compilationState, TypeSubstitution typeMap, Binder binder, SyntaxNode node, int recursionDepth, BindingDiagnosticBag diagnostics)
		{
			_parameterMap = new Dictionary<ParameterSymbol, BoundExpression>();
			_binder = binder;
			_typeMap = typeMap;
			_factory = new SyntheticBoundNodeFactory(null, currentMethod, node, compilationState, diagnostics);
			_expressionType = _factory.WellKnownType(WellKnownType.System_Linq_Expressions_Expression);
			_recursionDepth = recursionDepth;
		}

		internal static BoundExpression RewriteLambda(BoundLambda node, MethodSymbol currentMethod, NamedTypeSymbol delegateType, TypeCompilationState compilationState, TypeSubstitution typeMap, BindingDiagnosticBag diagnostics, HashSet<BoundNode> rewrittenNodes, int recursionDepth)
		{
			BoundExpression boundExpression = new ExpressionLambdaRewriter(currentMethod, compilationState, typeMap, node.LambdaSymbol.ContainingBinder, node.Syntax, recursionDepth, diagnostics).VisitLambdaInternal(node, delegateType);
			if (!boundExpression.HasErrors)
			{
				boundExpression = LocalRewriter.RewriteExpressionTree(boundExpression, currentMethod, compilationState, null, diagnostics, rewrittenNodes, recursionDepth);
			}
			return boundExpression;
		}

		private BoundExpression TranslateLambdaBody(BoundBlock block)
		{
			BoundStatement boundStatement = block.Statements[0];
			while (true)
			{
				BoundBlock boundBlock;
				switch (boundStatement.Kind)
				{
				case BoundKind.ReturnStatement:
				{
					BoundExpression expressionOpt = ((BoundReturnStatement)boundStatement).ExpressionOpt;
					if (expressionOpt != null)
					{
						return Visit(expressionOpt);
					}
					break;
				}
				case BoundKind.ExpressionStatement:
					return Visit(((BoundExpressionStatement)boundStatement).Expression);
				case BoundKind.Block:
					boundBlock = (BoundBlock)boundStatement;
					if (boundBlock.Locals.IsEmpty && boundBlock.Statements.Length == 1)
					{
						goto IL_0084;
					}
					break;
				}
				break;
				IL_0084:
				boundStatement = boundBlock.Statements[0];
			}
			return GenerateDiagnosticAndReturnDummyExpression(ERRID.ERR_StatementLambdaInExpressionTree, block);
		}

		private BoundExpression GenerateDiagnosticAndReturnDummyExpression(ERRID code, BoundNode node, params object[] args)
		{
			Diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(code, args), node.Syntax.GetLocation()));
			return VisitInternal(_factory.Literal("Diagnostics Generated"));
		}

		private BoundExpression Visit(BoundExpression node)
		{
			if (node == null)
			{
				return null;
			}
			SyntaxNode syntax = _factory.Syntax;
			_factory.Syntax = node.Syntax;
			BoundExpression arg = VisitInternal(node);
			_factory.Syntax = syntax;
			return _factory.Convert(_expressionType, arg);
		}

		private BoundExpression VisitExpressionWithoutStackGuard(BoundExpression node)
		{
			switch (node.Kind)
			{
			case BoundKind.ArrayCreation:
				return VisitArrayCreation((BoundArrayCreation)node);
			case BoundKind.ArrayAccess:
				return VisitArrayAccess((BoundArrayAccess)node);
			case BoundKind.ArrayLength:
				return VisitArrayLength((BoundArrayLength)node);
			case BoundKind.BadExpression:
				return VisitBadExpression((BoundBadExpression)node);
			case BoundKind.BinaryConditionalExpression:
				return VisitBinaryConditionalExpression((BoundBinaryConditionalExpression)node);
			case BoundKind.BinaryOperator:
				return VisitBinaryOperator((BoundBinaryOperator)node);
			case BoundKind.Call:
				return VisitCall((BoundCall)node);
			case BoundKind.Conversion:
				return VisitConversion((BoundConversion)node);
			case BoundKind.DelegateCreationExpression:
				return VisitDelegateCreationExpression((BoundDelegateCreationExpression)node);
			case BoundKind.DirectCast:
				return VisitDirectCast((BoundDirectCast)node);
			case BoundKind.FieldAccess:
			{
				BoundFieldAccess boundFieldAccess = (BoundFieldAccess)node;
				if (boundFieldAccess.FieldSymbol.IsCapturedFrame)
				{
					return CreateLiteralExpression(node);
				}
				return VisitFieldAccess(boundFieldAccess);
			}
			case BoundKind.Lambda:
				return VisitLambda((BoundLambda)node);
			case BoundKind.NewT:
				return VisitNewT((BoundNewT)node);
			case BoundKind.NullableIsTrueOperator:
				return VisitNullableIsTrueOperator((BoundNullableIsTrueOperator)node);
			case BoundKind.ObjectCreationExpression:
				return VisitObjectCreationExpression((BoundObjectCreationExpression)node);
			case BoundKind.Parameter:
				return VisitParameter((BoundParameter)node);
			case BoundKind.PropertyAccess:
				return VisitPropertyAccess((BoundPropertyAccess)node);
			case BoundKind.Sequence:
				return VisitSequence((BoundSequence)node);
			case BoundKind.TernaryConditionalExpression:
				return VisitTernaryConditionalExpression((BoundTernaryConditionalExpression)node);
			case BoundKind.TryCast:
				return VisitTryCast((BoundTryCast)node);
			case BoundKind.TypeOf:
				return VisitTypeOf((BoundTypeOf)node);
			case BoundKind.UnaryOperator:
				return VisitUnaryOperator((BoundUnaryOperator)node);
			case BoundKind.UserDefinedBinaryOperator:
				return VisitUserDefinedBinaryOperator((BoundUserDefinedBinaryOperator)node);
			case BoundKind.UserDefinedShortCircuitingOperator:
				return VisitUserDefinedShortCircuitingOperator((BoundUserDefinedShortCircuitingOperator)node);
			case BoundKind.UserDefinedUnaryOperator:
				return VisitUserDefinedUnaryOperator((BoundUserDefinedUnaryOperator)node);
			case BoundKind.GetType:
			case BoundKind.FieldInfo:
			case BoundKind.MethodInfo:
			case BoundKind.Literal:
			case BoundKind.MeReference:
			case BoundKind.MyClassReference:
			case BoundKind.Local:
				return CreateLiteralExpression(node);
			case BoundKind.MyBaseReference:
				throw ExceptionUtilities.UnexpectedValue(node.Kind);
			default:
				throw ExceptionUtilities.UnexpectedValue(node.Kind);
			}
		}

		private BoundExpression VisitInternal(BoundExpression node)
		{
			_recursionDepth++;
			BoundExpression result;
			if (_recursionDepth > 1)
			{
				StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
				result = VisitExpressionWithoutStackGuard(node);
			}
			else
			{
				result = VisitExpressionWithStackGuard(node);
			}
			_recursionDepth--;
			return result;
		}

		private BoundExpression VisitExpressionWithStackGuard(BoundExpression node)
		{
			try
			{
				return VisitExpressionWithoutStackGuard(node);
			}
			catch (InsufficientExecutionStackException ex)
			{
				ProjectData.SetProjectError(ex);
				InsufficientExecutionStackException inner = ex;
				throw new BoundTreeVisitor.CancelledByStackGuardException(inner, node);
			}
		}

		private BoundExpression VisitLambdaInternal(BoundLambda node, NamedTypeSymbol delegateType)
		{
			NamedTypeSymbol namedTypeSymbol = _factory.WellKnownType(WellKnownType.System_Linq_Expressions_ParameterExpression);
			ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
			ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
			ArrayBuilder<BoundExpression> instance3 = ArrayBuilder<BoundExpression>.GetInstance();
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = node.LambdaSymbol.Parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				LocalSymbol localSymbol = _factory.SynthesizedLocal(namedTypeSymbol);
				BoundLocal boundLocal = _factory.Local(localSymbol, isLValue: false);
				BoundLocal left = _factory.Local(localSymbol, isLValue: true);
				instance.Add(localSymbol);
				instance3.Add(boundLocal);
				_parameterMap[current] = boundLocal;
				BoundExpression boundExpression = ConvertRuntimeHelperToExpressionTree("Parameter", _factory.Typeof(current.Type.InternalSubstituteTypeParameters(_typeMap).Type), _factory.Literal(current.Name));
				if (!boundExpression.HasErrors)
				{
					instance2.Add(_factory.AssignmentExpression(left, boundExpression));
				}
			}
			BoundExpression boundExpression2 = TranslateLambdaBody(node.Body);
			BoundExpression result = _factory.Sequence(instance.ToImmutableAndFree(), instance2.ToImmutableAndFree(), ConvertRuntimeHelperToExpressionTree("Lambda", ImmutableArray.Create((TypeSymbol)delegateType), boundExpression2, _factory.Array(namedTypeSymbol, instance3.ToImmutableAndFree())));
			ImmutableArray<ParameterSymbol>.Enumerator enumerator2 = node.LambdaSymbol.Parameters.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				ParameterSymbol current2 = enumerator2.Current;
				_parameterMap.Remove(current2);
			}
			return result;
		}

		private BoundExpression VisitCall(BoundCall node)
		{
			MethodSymbol method = node.Method;
			BoundExpression boundExpression = node.ReceiverOpt;
			if (boundExpression != null)
			{
				boundExpression = ((boundExpression.Kind != BoundKind.MyBaseReference) ? Visit(boundExpression) : CreateLiteralExpression(boundExpression, method.ContainingType));
			}
			if (method.MethodKind == MethodKind.DelegateInvoke)
			{
				return ConvertRuntimeHelperToExpressionTree("Invoke", boundExpression, ConvertArgumentsIntoArray(node.Arguments));
			}
			return ConvertRuntimeHelperToExpressionTree("Call", method.IsShared ? _factory.Null(_expressionType) : boundExpression, _factory.MethodInfo(method), ConvertArgumentsIntoArray(node.Arguments));
		}

		private BoundExpression VisitFieldAccess(BoundFieldAccess node)
		{
			BoundExpression receiverOpt = node.ReceiverOpt;
			FieldSymbol fieldSymbol = node.FieldSymbol;
			bool isShared = fieldSymbol.IsShared;
			BoundExpression boundExpression = null;
			boundExpression = (isShared ? _factory.Null() : ((receiverOpt.Kind != BoundKind.MyBaseReference) ? Visit(receiverOpt) : CreateLiteralExpression(receiverOpt.MakeRValue(), fieldSymbol.ContainingType)));
			return ConvertRuntimeHelperToExpressionTree("Field", boundExpression, _factory.FieldInfo(fieldSymbol));
		}

		private BoundExpression VisitPropertyAccess(BoundPropertyAccess node)
		{
			BoundExpression receiverOpt = node.ReceiverOpt;
			PropertySymbol propertySymbol = node.PropertySymbol;
			bool isShared = propertySymbol.IsShared;
			BoundExpression boundExpression = null;
			boundExpression = ((!isShared) ? Visit(receiverOpt) : _factory.Null());
			MethodSymbol mostDerivedGetMethod = propertySymbol.GetMostDerivedGetMethod();
			return ConvertRuntimeHelperToExpressionTree("Property", boundExpression, _factory.MethodInfo(mostDerivedGetMethod));
		}

		private BoundExpression VisitLambda(BoundLambda node)
		{
			throw ExceptionUtilities.Unreachable;
		}

		private BoundExpression VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
		{
			NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)node.Type;
			MethodSymbol method = node.Method;
			BoundExpression boundExpression = node.ReceiverOpt;
			if (node.Method.IsShared)
			{
				boundExpression = _factory.Convert(ObjectType, _factory.Null());
			}
			else
			{
				if (boundExpression.Kind == BoundKind.MyBaseReference)
				{
					boundExpression = new BoundMeReference(boundExpression.Syntax, method.ContainingType);
				}
				else if (boundExpression.IsLValue)
				{
					boundExpression = boundExpression.MakeRValue();
				}
				if (!TypeSymbolExtensions.IsObjectType(boundExpression.Type))
				{
					boundExpression = _factory.Convert(ObjectType, boundExpression);
				}
			}
			BoundExpression boundExpression2 = _factory.MethodInfo(method.CallsiteReducedFromMethod ?? method);
			UseSiteInfo<AssemblySymbol> useSiteInfo = default(UseSiteInfo<AssemblySymbol>);
			Symbol wellKnownTypeMember = Binder.GetWellKnownTypeMember(_factory.Compilation, WellKnownMember.System_Reflection_MethodInfo__CreateDelegate, out useSiteInfo);
			BoundExpression node2;
			if (((object)wellKnownTypeMember != null) & (useSiteInfo.DiagnosticInfo == null))
			{
				Diagnostics.AddDependencies(useSiteInfo);
				node2 = _factory.Call(boundExpression2, (MethodSymbol)wellKnownTypeMember, _factory.Typeof(namedTypeSymbol), boundExpression);
			}
			else
			{
				wellKnownTypeMember = _factory.WellKnownMember<MethodSymbol>(WellKnownMember.System_Delegate__CreateDelegate4);
				if ((object)wellKnownTypeMember == null)
				{
					return node;
				}
				node2 = _factory.Call(_factory.Null(ObjectType), (MethodSymbol)wellKnownTypeMember, _factory.Typeof(namedTypeSymbol), boundExpression, boundExpression2, _factory.Literal(value: false));
			}
			return Convert(Visit(node2), namedTypeSymbol, isChecked: false);
		}

		private BoundExpression VisitParameter(BoundParameter node)
		{
			return _parameterMap[node.ParameterSymbol];
		}

		private BoundExpression VisitArrayAccess(BoundArrayAccess node)
		{
			BoundExpression boundExpression = Visit(node.Expression);
			if (node.Indices.Length == 1)
			{
				BoundExpression node2 = node.Indices[0];
				BoundExpression boundExpression2 = Visit(node2);
				return ConvertRuntimeHelperToExpressionTree("ArrayIndex", boundExpression, boundExpression2);
			}
			return ConvertRuntimeHelperToExpressionTree("ArrayIndex", boundExpression, BuildIndices(node.Indices));
		}

		private BoundExpression BuildIndices(ImmutableArray<BoundExpression> expressions)
		{
			int length = expressions.Length;
			BoundExpression[] array = new BoundExpression[length - 1 + 1];
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = Visit(expressions[i]);
			}
			return _factory.Array(_expressionType, array.AsImmutableOrNull());
		}

		private BoundExpression VisitBadExpression(BoundBadExpression node)
		{
			return node;
		}

		private BoundExpression VisitObjectCreationExpression(BoundObjectCreationExpression node)
		{
			BoundExpression creation = VisitObjectCreationExpressionInternal(node);
			return VisitObjectCreationContinued(creation, node.InitializerOpt);
		}

		private BoundExpression VisitNewT(BoundNewT node)
		{
			return VisitObjectCreationContinued(ConvertRuntimeHelperToExpressionTree("New", _factory.Typeof(node.Type)), node.InitializerOpt);
		}

		private BoundExpression VisitObjectCreationContinued(BoundExpression creation, BoundExpression initializerOpt)
		{
			if (initializerOpt == null)
			{
				return creation;
			}
			return initializerOpt.Kind switch
			{
				BoundKind.ObjectInitializerExpression => ConvertRuntimeHelperToExpressionTree("MemberInit", creation, VisitObjectInitializer((BoundObjectInitializerExpression)initializerOpt)), 
				BoundKind.CollectionInitializerExpression => ConvertRuntimeHelperToExpressionTree("ListInit", creation, VisitCollectionInitializer((BoundCollectionInitializerExpression)initializerOpt)), 
				_ => throw ExceptionUtilities.UnexpectedValue(initializerOpt.Kind), 
			};
		}

		private BoundExpression VisitObjectInitializer(BoundObjectInitializerExpression initializer)
		{
			ImmutableArray<BoundExpression> initializers = initializer.Initializers;
			int length = initializers.Length;
			BoundExpression[] array = new BoundExpression[length - 1 + 1];
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				BoundAssignmentOperator boundAssignmentOperator = (BoundAssignmentOperator)initializers[i];
				BoundExpression left = boundAssignmentOperator.Left;
				Symbol symbol = null;
				symbol = left.Kind switch
				{
					BoundKind.FieldAccess => ((BoundFieldAccess)boundAssignmentOperator.Left).FieldSymbol, 
					BoundKind.PropertyAccess => ((BoundPropertyAccess)boundAssignmentOperator.Left).PropertySymbol, 
					_ => throw ExceptionUtilities.UnexpectedValue(left.Kind), 
				};
				BoundExpression right = boundAssignmentOperator.Right;
				BoundExpression boundExpression = ((symbol.Kind == SymbolKind.Field) ? _factory.FieldInfo((FieldSymbol)symbol) : _factory.MethodInfo(((PropertySymbol)symbol).SetMethod));
				array[i] = _factory.Convert(MemberBindingType, ConvertRuntimeHelperToExpressionTree("Bind", boundExpression, Visit(right)));
			}
			return _factory.Array(MemberBindingType, array.AsImmutableOrNull());
		}

		private BoundExpression VisitCollectionInitializer(BoundCollectionInitializerExpression initializer)
		{
			ImmutableArray<BoundExpression> initializers = initializer.Initializers;
			int length = initializers.Length;
			BoundExpression[] array = new BoundExpression[length - 1 + 1];
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				BoundCall boundCall = (BoundCall)initializers[i];
				array[i] = _factory.Convert(ElementInitType, ConvertRuntimeHelperToExpressionTree("ElementInit", _factory.MethodInfo(boundCall.Method), ConvertArgumentsIntoArray((boundCall.Method.IsShared && boundCall.Method.IsExtensionMethod) ? boundCall.Arguments.RemoveAt(0) : boundCall.Arguments)));
			}
			return _factory.Array(ElementInitType, array.AsImmutableOrNull());
		}

		private BoundExpression VisitObjectCreationExpressionInternal(BoundObjectCreationExpression node)
		{
			if ((object)node.ConstantValueOpt != null)
			{
				return CreateLiteralExpression(node);
			}
			if ((object)node.ConstructorOpt == null || (node.Arguments.Length == 0 && !TypeSymbolExtensions.IsStructureType(node.Type)) || SymbolExtensions.IsDefaultValueTypeConstructor(node.ConstructorOpt))
			{
				return ConvertRuntimeHelperToExpressionTree("New", _factory.Typeof(node.Type));
			}
			BoundExpression boundExpression = _factory.ConstructorInfo(node.ConstructorOpt);
			BoundExpression boundExpression2 = ConvertArgumentsIntoArray(node.Arguments);
			if (node.Type.IsAnonymousType && node.Arguments.Length != 0)
			{
				ImmutableArray<AnonymousTypeManager.AnonymousTypePropertyPublicSymbol> properties = ((AnonymousTypeManager.AnonymousTypePublicSymbol)node.Type).Properties;
				BoundExpression[] array = new BoundExpression[properties.Length - 1 + 1];
				int num = properties.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					array[i] = _factory.Convert(MemberInfoType, _factory.MethodInfo(properties[i].GetMethod));
				}
				return ConvertRuntimeHelperToExpressionTree("New", boundExpression, boundExpression2, _factory.Array(MemberInfoType, array.AsImmutableOrNull()));
			}
			return ConvertRuntimeHelperToExpressionTree("New", boundExpression, boundExpression2);
		}

		private BoundExpression VisitSequence(BoundSequence node)
		{
			ImmutableArray<LocalSymbol> locals = node.Locals;
			ImmutableArray<BoundExpression> sideEffects = node.SideEffects;
			BoundExpression valueOpt = node.ValueOpt;
			if (locals.IsEmpty && sideEffects.IsEmpty && valueOpt != null)
			{
				return VisitInternal(valueOpt);
			}
			return GenerateDiagnosticAndReturnDummyExpression(ERRID.ERR_ExpressionTreeNotSupported, node);
		}

		private BoundExpression VisitArrayLength(BoundArrayLength node)
		{
			TypeSymbol type = node.Type;
			if (type.SpecialType == SpecialType.System_Int64)
			{
				return VisitCall(new BoundCall(node.Syntax, ((PropertySymbol)_factory.SpecialMember(SpecialMember.System_Array__LongLength)).GetMethod, null, node.Expression, ImmutableArray<BoundExpression>.Empty, null, isLValue: false, suppressObjectClone: true, type));
			}
			return ConvertRuntimeHelperToExpressionTree("ArrayLength", Visit(node.Expression));
		}

		private BoundExpression VisitArrayCreation(BoundArrayCreation node)
		{
			ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)node.Type;
			BoundExpression boundExpression = _factory.Typeof(arrayTypeSymbol.ElementType);
			BoundArrayInitialization initializerOpt = node.InitializerOpt;
			if (initializerOpt != null && !initializerOpt.Initializers.IsEmpty)
			{
				return ConvertRuntimeHelperToExpressionTree("NewArrayInit", boundExpression, ConvertArgumentsIntoArray(node.InitializerOpt.Initializers));
			}
			return ConvertRuntimeHelperToExpressionTree("NewArrayBounds", boundExpression, ConvertArgumentsIntoArray(node.Bounds));
		}

		private BoundExpression ConvertArgumentsIntoArray(ImmutableArray<BoundExpression> exprs)
		{
			BoundExpression[] array = new BoundExpression[exprs.Length - 1 + 1];
			int num = exprs.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = Visit(exprs[i]);
			}
			return _factory.Array(_expressionType, array.AsImmutableOrNull());
		}

		private BoundExpression VisitTypeOf(BoundTypeOf node)
		{
			return ConvertRuntimeHelperToExpressionTree("TypeIs", Visit(node.Operand), _factory.Typeof(node.TargetType));
		}

		private BoundExpression Call(BoundExpression receiver, MethodSymbol method, params BoundExpression[] @params)
		{
			BoundExpression[] array = new BoundExpression[@params.Length + 1 + 1];
			array[0] = receiver;
			array[1] = _factory.MethodInfo(method);
			Array.Copy(@params, 0, array, 2, @params.Length);
			return ConvertRuntimeHelperToExpressionTree("Call", array);
		}

		private BoundExpression Default(TypeSymbol type)
		{
			return ConvertRuntimeHelperToExpressionTree("Default", _factory.Typeof(type));
		}

		private BoundExpression New(SpecialMember helper, BoundExpression argument)
		{
			return ConvertRuntimeHelperToExpressionTree("New", _factory.ConstructorInfo(helper), argument);
		}

		private BoundExpression Negate(BoundExpression expr)
		{
			return ConvertRuntimeHelperToExpressionTree("Negate", expr);
		}

		private BoundExpression InitWithParameterlessValueTypeConstructor(TypeSymbol type)
		{
			return ConvertRuntimeHelperToExpressionTree("New", _factory.Typeof(type));
		}

		private bool IsIntegralType(TypeSymbol type)
		{
			return TypeSymbolExtensions.IsIntegralType(GetUnderlyingType(type));
		}

		private TypeSymbol GetUnderlyingType(TypeSymbol type)
		{
			return TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(type));
		}

		private BoundExpression CreateLiteralExpression(BoundExpression node)
		{
			return CreateLiteralExpression(node, node.Type);
		}

		private BoundExpression CreateLiteralExpression(BoundExpression node, TypeSymbol type)
		{
			return ConvertRuntimeHelperToExpressionTree("Constant", _factory.Convert(ObjectType, node), _factory.Typeof(type));
		}

		private BoundExpression ConvertRuntimeHelperToExpressionTree(string helperMethodName, params BoundExpression[] arguments)
		{
			return ConvertRuntimeHelperToExpressionTree(helperMethodName, ImmutableArray<TypeSymbol>.Empty, arguments);
		}

		private BoundExpression ConvertRuntimeHelperToExpressionTree(string helperMethodName, ImmutableArray<TypeSymbol> typeArgs, params BoundExpression[] arguments)
		{
			bool flag = false;
			for (int i = 0; i < arguments.Length; i = checked(i + 1))
			{
				if (arguments[i].HasErrors)
				{
					flag = true;
				}
			}
			BoundMethodGroup exprFactoryMethodGroup = GetExprFactoryMethodGroup(helperMethodName, typeArgs);
			if (exprFactoryMethodGroup == null || flag)
			{
				return _factory.BadExpression(arguments);
			}
			return _binder.BindInvocationExpression(_factory.Syntax, _factory.Syntax, TypeCharacter.None, exprFactoryMethodGroup, ImmutableArray.Create(arguments), default(ImmutableArray<string>), Diagnostics, null);
		}

		private BoundMethodGroup GetExprFactoryMethodGroup(string methodName, ImmutableArray<TypeSymbol> typeArgs)
		{
			BoundMethodGroup boundMethodGroup = null;
			LookupResult instance = LookupResult.GetInstance();
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = _binder.GetNewCompoundUseSiteInfo(Diagnostics);
			_binder.LookupMember(instance, _expressionType, methodName, 0, LookupOptions.AllMethodsOfAnyArity | LookupOptions.IgnoreExtensionMethods, ref useSiteInfo);
			((BindingDiagnosticBag<AssemblySymbol>)Diagnostics).Add(_factory.Syntax, useSiteInfo);
			if (instance.IsGood)
			{
				_ = instance.Symbols[0];
				if (instance.Symbols[0].Kind == SymbolKind.Method)
				{
					boundMethodGroup = new BoundMethodGroup(_factory.Syntax, _factory.TypeArguments(typeArgs), instance.Symbols.ToDowncastedImmutable<MethodSymbol>(), instance.Kind, null, QualificationKind.QualifiedViaTypeName);
				}
			}
			if (boundMethodGroup == null)
			{
				Diagnostics.Add(instance.HasDiagnostic ? instance.Diagnostic : ErrorFactory.ErrorInfo(ERRID.ERR_NameNotMember2, methodName, _expressionType), _factory.Syntax.GetLocation());
			}
			instance.Free();
			return boundMethodGroup;
		}

		private BoundExpression VisitBinaryOperator(BoundBinaryOperator node)
		{
			switch (node.OperatorKind & BinaryOperatorKind.OpMask)
			{
			case BinaryOperatorKind.Add:
			case BinaryOperatorKind.Concatenate:
			case BinaryOperatorKind.Subtract:
			case BinaryOperatorKind.Multiply:
			case BinaryOperatorKind.Power:
			case BinaryOperatorKind.Divide:
			case BinaryOperatorKind.Modulo:
			case BinaryOperatorKind.IntegerDivide:
			case BinaryOperatorKind.LeftShift:
			case BinaryOperatorKind.RightShift:
			case BinaryOperatorKind.Xor:
			case BinaryOperatorKind.Or:
			case BinaryOperatorKind.And:
				return ConvertBinaryOperator(node);
			case BinaryOperatorKind.Equals:
			case BinaryOperatorKind.NotEquals:
			case BinaryOperatorKind.LessThanOrEqual:
			case BinaryOperatorKind.GreaterThanOrEqual:
			case BinaryOperatorKind.LessThan:
			case BinaryOperatorKind.GreaterThan:
			case BinaryOperatorKind.Is:
			case BinaryOperatorKind.IsNot:
				return ConvertBooleanOperator(node);
			case BinaryOperatorKind.OrElse:
			case BinaryOperatorKind.AndAlso:
				return ConvertShortCircuitedBooleanOperator(node);
			case BinaryOperatorKind.Like:
				throw ExceptionUtilities.UnexpectedValue(node.OperatorKind);
			default:
				throw ExceptionUtilities.UnexpectedValue(node.OperatorKind);
			}
		}

		private BoundExpression VisitUserDefinedBinaryOperator(BoundUserDefinedBinaryOperator node)
		{
			BinaryOperatorKind binaryOperatorKind = node.OperatorKind & BinaryOperatorKind.OpMask;
			bool value = (node.OperatorKind & BinaryOperatorKind.Lifted) != 0;
			bool isChecked = node.Checked && IsIntegralType(node.Call.Method.ReturnType);
			switch (binaryOperatorKind)
			{
			case BinaryOperatorKind.Concatenate:
			case BinaryOperatorKind.Like:
				return ConvertUserDefinedLikeOrConcate(node);
			case BinaryOperatorKind.Equals:
			case BinaryOperatorKind.NotEquals:
			case BinaryOperatorKind.LessThanOrEqual:
			case BinaryOperatorKind.GreaterThanOrEqual:
			case BinaryOperatorKind.LessThan:
			case BinaryOperatorKind.GreaterThan:
			case BinaryOperatorKind.Is:
			case BinaryOperatorKind.IsNot:
				return ConvertRuntimeHelperToExpressionTree(GetBinaryOperatorMethodName(binaryOperatorKind, isChecked), Visit(node.Left), Visit(node.Right), _factory.Literal(value), _factory.MethodInfo(node.Call.Method));
			default:
				return ConvertRuntimeHelperToExpressionTree(GetBinaryOperatorMethodName(binaryOperatorKind, isChecked), Visit(node.Left), Visit(node.Right), _factory.MethodInfo(node.Call.Method));
			}
		}

		private BoundExpression VisitUserDefinedShortCircuitingOperator(BoundUserDefinedShortCircuitingOperator node)
		{
			BoundUserDefinedBinaryOperator bitwiseOperator = node.BitwiseOperator;
			BinaryOperatorKind binaryOperatorKind = bitwiseOperator.OperatorKind & BinaryOperatorKind.OpMask;
			binaryOperatorKind = ((binaryOperatorKind == BinaryOperatorKind.And) ? BinaryOperatorKind.AndAlso : BinaryOperatorKind.OrElse);
			return ConvertRuntimeHelperToExpressionTree(GetBinaryOperatorMethodName(binaryOperatorKind, isChecked: false), Visit(bitwiseOperator.Left), Visit(bitwiseOperator.Right), _factory.MethodInfo(bitwiseOperator.Call.Method));
		}

		private BoundExpression ConvertUserDefinedLikeOrConcate(BoundUserDefinedBinaryOperator node)
		{
			BoundCall call = node.Call;
			BinaryOperatorKind operatorKind = node.OperatorKind;
			if ((operatorKind & BinaryOperatorKind.Lifted) == 0)
			{
				return VisitInternal(call);
			}
			return VisitInternal(AdjustCallForLiftedOperator(operatorKind, call, node.Type));
		}

		private BoundExpression ConvertBooleanOperator(BoundBinaryOperator node)
		{
			TypeSymbol type = node.Type;
			TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(type));
			_ = enumUnderlyingTypeOrSelf.SpecialType;
			BinaryOperatorKind binaryOperatorKind = node.OperatorKind & BinaryOperatorKind.OpMask;
			BoundExpression left = node.Left;
			TypeSymbol type2 = left.Type;
			BoundExpression boundExpression = null;
			BoundExpression right = node.Right;
			BoundExpression boundExpression2 = null;
			bool flag = binaryOperatorKind == BinaryOperatorKind.Is || binaryOperatorKind == BinaryOperatorKind.IsNot;
			if (flag)
			{
				if (BoundExpressionExtensions.IsNothingLiteral(left))
				{
					if (TypeSymbolExtensions.IsNullableType(right.Type))
					{
						boundExpression = CreateLiteralExpression(left, right.Type);
						type2 = right.Type;
					}
				}
				else if (BoundExpressionExtensions.IsNothingLiteral(right) && TypeSymbolExtensions.IsNullableType(left.Type))
				{
					boundExpression2 = CreateLiteralExpression(right, left.Type);
				}
			}
			bool flag2 = TypeSymbolExtensions.IsNullableType(type2);
			TypeSymbol nullableUnderlyingTypeOrSelf = TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(type2);
			TypeSymbol enumUnderlyingTypeOrSelf2 = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(nullableUnderlyingTypeOrSelf);
			SpecialType specialType = enumUnderlyingTypeOrSelf2.SpecialType;
			if (boundExpression == null)
			{
				boundExpression = Visit(left);
			}
			if (boundExpression2 == null)
			{
				boundExpression2 = Visit(right);
			}
			MethodSymbol methodSymbol = null;
			switch (specialType)
			{
			case SpecialType.System_Decimal:
				methodSymbol = GetHelperForDecimalBinaryOperation(binaryOperatorKind);
				break;
			case SpecialType.System_DateTime:
				methodSymbol = GetHelperForDateTimeBinaryOperation(binaryOperatorKind);
				break;
			}
			bool isChecked = node.Checked && IsIntegralType(enumUnderlyingTypeOrSelf);
			string binaryOperatorMethodName = GetBinaryOperatorMethodName(binaryOperatorKind, isChecked);
			if ((object)methodSymbol != null)
			{
				return ConvertRuntimeHelperToExpressionTree(binaryOperatorMethodName, boundExpression, boundExpression2, _factory.Literal(TypeSymbolExtensions.IsNullableType(type)), _factory.MethodInfo(methodSymbol));
			}
			bool flag3 = false;
			if (specialType == SpecialType.System_Boolean)
			{
				switch (binaryOperatorKind)
				{
				case BinaryOperatorKind.LessThan:
					binaryOperatorMethodName = GetBinaryOperatorMethodName(BinaryOperatorKind.GreaterThan, isChecked);
					flag3 = true;
					break;
				case BinaryOperatorKind.LessThanOrEqual:
					binaryOperatorMethodName = GetBinaryOperatorMethodName(BinaryOperatorKind.GreaterThanOrEqual, isChecked);
					flag3 = true;
					break;
				case BinaryOperatorKind.GreaterThan:
					binaryOperatorMethodName = GetBinaryOperatorMethodName(BinaryOperatorKind.LessThan, isChecked);
					flag3 = true;
					break;
				case BinaryOperatorKind.GreaterThanOrEqual:
					binaryOperatorMethodName = GetBinaryOperatorMethodName(BinaryOperatorKind.LessThanOrEqual, isChecked);
					flag3 = true;
					break;
				}
			}
			TypeSymbol typeFrom = type2;
			if (TypeSymbolExtensions.IsEnumType(nullableUnderlyingTypeOrSelf) && !flag)
			{
				TypeSymbol typeSymbol = (flag2 ? _factory.NullableOf(enumUnderlyingTypeOrSelf2) : enumUnderlyingTypeOrSelf2);
				boundExpression = CreateBuiltInConversion(typeFrom, typeSymbol, boundExpression, node.Checked, isExplicit: false, ConversionSemantics.Default);
				boundExpression2 = CreateBuiltInConversion(typeFrom, typeSymbol, boundExpression2, node.Checked, isExplicit: false, ConversionSemantics.Default);
				typeFrom = typeSymbol;
			}
			if (flag3 && !flag)
			{
				TypeSymbol typeSymbol2 = (flag2 ? _factory.NullableOf(Int32Type) : Int32Type);
				boundExpression = Convert(boundExpression, typeSymbol2, node.Checked);
				boundExpression2 = Convert(boundExpression2, typeSymbol2, node.Checked);
				typeFrom = typeSymbol2;
			}
			return ConvertRuntimeHelperToExpressionTree(binaryOperatorMethodName, boundExpression, boundExpression2, _factory.Literal(TypeSymbolExtensions.IsNullableType(type)), _factory.Null());
		}

		private BoundExpression ConvertShortCircuitedBooleanOperator(BoundBinaryOperator node)
		{
			TypeSymbol type = node.Type;
			TypeSymbol underlyingType = GetUnderlyingType(type);
			_ = underlyingType.SpecialType;
			BinaryOperatorKind opKind = node.OperatorKind & BinaryOperatorKind.OpMask;
			BoundExpression left = node.Left;
			BoundExpression boundExpression = Visit(left);
			BoundExpression right = node.Right;
			BoundExpression boundExpression2 = Visit(right);
			if (TypeSymbolExtensions.IsObjectType(underlyingType))
			{
				TypeSymbol typeTo = _factory.SpecialType(SpecialType.System_Boolean);
				boundExpression = CreateBuiltInConversion(type, typeTo, boundExpression, node.Checked, isExplicit: false, ConversionSemantics.Default);
				boundExpression2 = CreateBuiltInConversion(type, typeTo, boundExpression2, node.Checked, isExplicit: false, ConversionSemantics.Default);
			}
			bool isChecked = node.Checked && IsIntegralType(underlyingType);
			string binaryOperatorMethodName = GetBinaryOperatorMethodName(opKind, isChecked);
			BoundExpression boundExpression3 = ConvertRuntimeHelperToExpressionTree(binaryOperatorMethodName, boundExpression, boundExpression2);
			if (TypeSymbolExtensions.IsObjectType(underlyingType))
			{
				boundExpression3 = Convert(boundExpression3, type, isChecked);
			}
			return boundExpression3;
		}

		private BoundExpression ConvertBinaryOperator(BoundBinaryOperator node)
		{
			TypeSymbol type = node.Type;
			TypeSymbol nullableUnderlyingTypeOrSelf = TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(type);
			TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(nullableUnderlyingTypeOrSelf);
			SpecialType specialType = enumUnderlyingTypeOrSelf.SpecialType;
			BinaryOperatorKind binaryOperatorKind = node.OperatorKind & BinaryOperatorKind.OpMask;
			MethodSymbol methodSymbol = null;
			switch (specialType)
			{
			case SpecialType.System_Object:
				methodSymbol = GetHelperForObjectBinaryOperation(binaryOperatorKind);
				break;
			case SpecialType.System_Decimal:
				methodSymbol = GetHelperForDecimalBinaryOperation(binaryOperatorKind);
				break;
			default:
				switch (binaryOperatorKind)
				{
				case BinaryOperatorKind.Concatenate:
					methodSymbol = (MethodSymbol)_factory.SpecialMember(SpecialMember.System_String__ConcatStringString);
					break;
				case BinaryOperatorKind.Power:
					methodSymbol = _factory.WellKnownMember<MethodSymbol>(WellKnownMember.System_Math__PowDoubleDouble);
					break;
				}
				break;
			}
			bool flag = node.Checked && IsIntegralType(enumUnderlyingTypeOrSelf);
			string binaryOperatorMethodName = GetBinaryOperatorMethodName(binaryOperatorKind, flag);
			BoundExpression boundExpression = Visit(node.Left);
			BoundExpression boundExpression2;
			if ((object)methodSymbol != null)
			{
				boundExpression2 = Visit(node.Right);
				return ConvertRuntimeHelperToExpressionTree(binaryOperatorMethodName, boundExpression, boundExpression2, _factory.MethodInfo(methodSymbol));
			}
			bool flag2 = TypeSymbolExtensions.IsNullableType(type);
			bool flag3 = specialType == SpecialType.System_Byte || specialType == SpecialType.System_SByte;
			boundExpression = GenerateCastsForBinaryAndUnaryOperator(boundExpression, flag2, nullableUnderlyingTypeOrSelf, flag && IsIntegralType(enumUnderlyingTypeOrSelf), flag3);
			if (binaryOperatorKind == BinaryOperatorKind.LeftShift || binaryOperatorKind == BinaryOperatorKind.RightShift)
			{
				boundExpression2 = MaskShiftCountOperand(node, type, flag);
				flag = false;
			}
			else
			{
				boundExpression2 = Visit(node.Right);
				boundExpression2 = GenerateCastsForBinaryAndUnaryOperator(boundExpression2, flag2, nullableUnderlyingTypeOrSelf, flag && IsIntegralType(enumUnderlyingTypeOrSelf), flag3);
			}
			BoundExpression boundExpression3 = ConvertRuntimeHelperToExpressionTree(binaryOperatorMethodName, boundExpression, boundExpression2);
			if (flag3)
			{
				boundExpression3 = Convert(boundExpression3, flag2 ? _factory.NullableOf(enumUnderlyingTypeOrSelf) : enumUnderlyingTypeOrSelf, flag);
			}
			if (TypeSymbolExtensions.IsEnumType(nullableUnderlyingTypeOrSelf))
			{
				boundExpression3 = Convert(boundExpression3, type, isChecked: false);
			}
			return boundExpression3;
		}

		private BoundExpression GenerateCastsForBinaryAndUnaryOperator(BoundExpression loweredOperand, bool isNullable, TypeSymbol notNullableType, bool @checked, bool needToCastBackToByteOrSByte)
		{
			if (TypeSymbolExtensions.IsEnumType(notNullableType))
			{
				TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(notNullableType);
				loweredOperand = Convert(loweredOperand, isNullable ? _factory.NullableOf(enumUnderlyingTypeOrSelf) : enumUnderlyingTypeOrSelf, isChecked: false);
			}
			if (needToCastBackToByteOrSByte)
			{
				loweredOperand = Convert(loweredOperand, isNullable ? _factory.NullableOf(Int32Type) : Int32Type, @checked);
			}
			return loweredOperand;
		}

		private BoundExpression MaskShiftCountOperand(BoundBinaryOperator node, TypeSymbol resultType, bool isChecked)
		{
			BoundExpression boundExpression = null;
			bool flag = false;
			BoundExpression right = node.Right;
			int shiftSizeMask = CodeGenerator.GetShiftSizeMask(TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(resultType));
			TypeSymbol shiftedType = resultType;
			if (TypeSymbolExtensions.IsNullableType(resultType) && right.Kind == BoundKind.Conversion)
			{
				BoundConversion boundConversion = (BoundConversion)right;
				BoundExpression operand = boundConversion.Operand;
				TypeSymbol type = operand.Type;
				if ((boundConversion.ConversionKind & ConversionKind.Nullable) != 0 && !boundConversion.ExplicitCastInCode && !TypeSymbolExtensions.IsNullableType(type))
				{
					shiftedType = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(type);
					boundExpression = Visit(operand);
					if (!TypeSymbol.Equals(type, Int32Type, TypeCompareKind.ConsiderEverything))
					{
						boundExpression = CreateBuiltInConversion(type, Int32Type, boundExpression, isChecked, isExplicit: false, ConversionSemantics.Default);
					}
					flag = true;
				}
			}
			if (!flag)
			{
				boundExpression = Visit(right);
			}
			boundExpression = MaskShiftCountOperand(boundExpression, shiftedType, shiftSizeMask, boundExpression.ConstantValueOpt, isChecked);
			if (flag)
			{
				boundExpression = Convert(boundExpression, _factory.NullableOf(Int32Type), isChecked);
			}
			return boundExpression;
		}

		private BoundExpression MaskShiftCountOperand(BoundExpression loweredOperand, TypeSymbol shiftedType, int shiftMask, ConstantValue shiftConst, bool isChecked)
		{
			if ((object)shiftConst == null || shiftConst.UInt32Value > shiftMask)
			{
				BoundExpression boundExpression = ConvertRuntimeHelperToExpressionTree("Constant", _factory.Convert(ObjectType, _factory.Literal(shiftMask)), _factory.Typeof(Int32Type));
				bool num = TypeSymbolExtensions.IsNullableType(shiftedType);
				_ = TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(shiftedType).SpecialType;
				TypeSymbol type = (num ? _factory.NullableOf(Int32Type) : null);
				if (num)
				{
					boundExpression = Convert(boundExpression, type, isChecked);
				}
				loweredOperand = ConvertRuntimeHelperToExpressionTree("And", loweredOperand, boundExpression);
			}
			return loweredOperand;
		}

		private MethodSymbol GetHelperForDecimalBinaryOperation(BinaryOperatorKind opKind)
		{
			opKind &= BinaryOperatorKind.OpMask;
			SpecialMember sm;
			switch (opKind)
			{
			case BinaryOperatorKind.Add:
				sm = SpecialMember.System_Decimal__AddDecimalDecimal;
				break;
			case BinaryOperatorKind.Subtract:
				sm = SpecialMember.System_Decimal__SubtractDecimalDecimal;
				break;
			case BinaryOperatorKind.Multiply:
				sm = SpecialMember.System_Decimal__MultiplyDecimalDecimal;
				break;
			case BinaryOperatorKind.Divide:
				sm = SpecialMember.System_Decimal__DivideDecimalDecimal;
				break;
			case BinaryOperatorKind.Modulo:
				sm = SpecialMember.System_Decimal__ModuloDecimalDecimal;
				break;
			case BinaryOperatorKind.Equals:
			case BinaryOperatorKind.Is:
				sm = SpecialMember.System_Decimal__op_Equality;
				break;
			case BinaryOperatorKind.NotEquals:
			case BinaryOperatorKind.IsNot:
				sm = SpecialMember.System_Decimal__op_Inequality;
				break;
			case BinaryOperatorKind.LessThan:
				sm = SpecialMember.System_Decimal__op_LessThan;
				break;
			case BinaryOperatorKind.LessThanOrEqual:
				sm = SpecialMember.System_Decimal__op_LessThanOrEqual;
				break;
			case BinaryOperatorKind.GreaterThan:
				sm = SpecialMember.System_Decimal__op_GreaterThan;
				break;
			case BinaryOperatorKind.GreaterThanOrEqual:
				sm = SpecialMember.System_Decimal__op_GreaterThanOrEqual;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(opKind);
			}
			return (MethodSymbol)_factory.SpecialMember(sm);
		}

		private MethodSymbol GetHelperForDateTimeBinaryOperation(BinaryOperatorKind opKind)
		{
			opKind &= BinaryOperatorKind.OpMask;
			SpecialMember sm;
			switch (opKind)
			{
			case BinaryOperatorKind.Equals:
			case BinaryOperatorKind.Is:
				sm = SpecialMember.System_DateTime__op_Equality;
				break;
			case BinaryOperatorKind.NotEquals:
			case BinaryOperatorKind.IsNot:
				sm = SpecialMember.System_DateTime__op_Inequality;
				break;
			case BinaryOperatorKind.LessThan:
				sm = SpecialMember.System_DateTime__op_LessThan;
				break;
			case BinaryOperatorKind.LessThanOrEqual:
				sm = SpecialMember.System_DateTime__op_LessThanOrEqual;
				break;
			case BinaryOperatorKind.GreaterThan:
				sm = SpecialMember.System_DateTime__op_GreaterThan;
				break;
			case BinaryOperatorKind.GreaterThanOrEqual:
				sm = SpecialMember.System_DateTime__op_GreaterThanOrEqual;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(opKind);
			}
			return (MethodSymbol)_factory.SpecialMember(sm);
		}

		private MethodSymbol GetHelperForObjectBinaryOperation(BinaryOperatorKind opKind)
		{
			opKind &= BinaryOperatorKind.OpMask;
			WellKnownMember wm = opKind switch
			{
				BinaryOperatorKind.Add => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__AddObjectObjectObject, 
				BinaryOperatorKind.Subtract => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__SubtractObjectObjectObject, 
				BinaryOperatorKind.Multiply => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__MultiplyObjectObjectObject, 
				BinaryOperatorKind.Divide => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__DivideObjectObjectObject, 
				BinaryOperatorKind.IntegerDivide => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__IntDivideObjectObjectObject, 
				BinaryOperatorKind.Modulo => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ModObjectObjectObject, 
				BinaryOperatorKind.Power => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ExponentObjectObjectObject, 
				BinaryOperatorKind.And => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__AndObjectObjectObject, 
				BinaryOperatorKind.Xor => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__XorObjectObjectObject, 
				BinaryOperatorKind.Or => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__OrObjectObjectObject, 
				BinaryOperatorKind.LeftShift => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__LeftShiftObjectObjectObject, 
				BinaryOperatorKind.RightShift => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__RightShiftObjectObjectObject, 
				BinaryOperatorKind.Concatenate => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConcatenateObjectObjectObject, 
				_ => throw ExceptionUtilities.UnexpectedValue(opKind), 
			};
			return _factory.WellKnownMember<MethodSymbol>(wm);
		}

		private static string GetBinaryOperatorMethodName(BinaryOperatorKind opKind, bool isChecked)
		{
			switch (opKind & BinaryOperatorKind.OpMask)
			{
			case BinaryOperatorKind.Add:
				return isChecked ? "AddChecked" : "Add";
			case BinaryOperatorKind.Subtract:
				return isChecked ? "SubtractChecked" : "Subtract";
			case BinaryOperatorKind.Multiply:
				return isChecked ? "MultiplyChecked" : "Multiply";
			case BinaryOperatorKind.Divide:
			case BinaryOperatorKind.IntegerDivide:
				return "Divide";
			case BinaryOperatorKind.Modulo:
				return "Modulo";
			case BinaryOperatorKind.Power:
				return "Power";
			case BinaryOperatorKind.And:
				return "And";
			case BinaryOperatorKind.Or:
				return "Or";
			case BinaryOperatorKind.Xor:
				return "ExclusiveOr";
			case BinaryOperatorKind.LeftShift:
				return "LeftShift";
			case BinaryOperatorKind.RightShift:
				return "RightShift";
			case BinaryOperatorKind.Equals:
			case BinaryOperatorKind.Is:
				return "Equal";
			case BinaryOperatorKind.NotEquals:
			case BinaryOperatorKind.IsNot:
				return "NotEqual";
			case BinaryOperatorKind.LessThan:
				return "LessThan";
			case BinaryOperatorKind.LessThanOrEqual:
				return "LessThanOrEqual";
			case BinaryOperatorKind.GreaterThan:
				return "GreaterThan";
			case BinaryOperatorKind.GreaterThanOrEqual:
				return "GreaterThanOrEqual";
			case BinaryOperatorKind.AndAlso:
				return "AndAlso";
			case BinaryOperatorKind.OrElse:
				return "OrElse";
			default:
				throw ExceptionUtilities.UnexpectedValue(opKind);
			}
		}

		private static BoundExpression AdjustCallArgumentForLiftedOperator(BoundExpression oldArg, TypeSymbol parameterType)
		{
			if (oldArg.Kind == BoundKind.ObjectCreationExpression)
			{
				BoundObjectCreationExpression boundObjectCreationExpression = (BoundObjectCreationExpression)oldArg;
				if (boundObjectCreationExpression.Arguments.Length == 1)
				{
					return boundObjectCreationExpression.Arguments[0];
				}
			}
			return new BoundConversion(oldArg.Syntax, oldArg, ConversionKind.NarrowingNullable, @checked: false, explicitCastInCode: false, parameterType);
		}

		private BoundExpression AdjustCallForLiftedOperator(BinaryOperatorKind opKind, BoundCall call, TypeSymbol resultType)
		{
			return AdjustCallForLiftedOperator_DoNotCallDirectly(call, resultType);
		}

		private BoundExpression AdjustCallForLiftedOperator(UnaryOperatorKind opKind, BoundCall call, TypeSymbol resultType)
		{
			return AdjustCallForLiftedOperator_DoNotCallDirectly(call, resultType);
		}

		private BoundExpression AdjustCallForLiftedOperator_DoNotCallDirectly(BoundCall call, TypeSymbol resultType)
		{
			ImmutableArray<ParameterSymbol> parameters = call.Method.Parameters;
			ImmutableArray<BoundExpression> arguments = call.Arguments;
			BoundExpression[] array = new BoundExpression[arguments.Length - 1 + 1];
			int num = arguments.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = AdjustCallArgumentForLiftedOperator(arguments[i], parameters[i].Type);
			}
			TypeSymbol returnType = call.Method.ReturnType;
			call = call.Update(call.Method, call.MethodGroupOpt, call.ReceiverOpt, array.AsImmutableOrNull(), call.DefaultArguments, call.ConstantValueOpt, call.IsLValue, call.SuppressObjectClone, returnType);
			if (TypeSymbolExtensions.IsNullableType(resultType) != TypeSymbolExtensions.IsNullableType(returnType))
			{
				return _factory.Convert(resultType, call);
			}
			return call;
		}

		private BoundExpression VisitTernaryConditionalExpression(BoundTernaryConditionalExpression node)
		{
			BoundExpression boundExpression = Visit(node.Condition);
			BoundExpression boundExpression2 = Visit(node.WhenTrue);
			BoundExpression boundExpression3 = Visit(node.WhenFalse);
			return ConvertRuntimeHelperToExpressionTree("Condition", boundExpression, boundExpression2, boundExpression3);
		}

		private BoundExpression VisitBinaryConditionalExpression(BoundBinaryConditionalExpression node)
		{
			BoundExpression testExpression = node.TestExpression;
			BoundExpression convertedTestExpression = node.ConvertedTestExpression;
			TypeSymbol type = node.Type;
			TypeSymbol type2 = testExpression.Type;
			BoundExpression boundExpression = Visit(testExpression);
			BoundExpression boundExpression2 = Visit(node.ElseExpression);
			if (convertedTestExpression == null || TypeSymbolExtensions.IsSameTypeIgnoringAll(type, type2) || (TypeSymbolExtensions.IsNullableType(type2) && TypeSymbolExtensions.IsSameTypeIgnoringAll(type, TypeSymbolExtensions.GetNullableUnderlyingType(type2))))
			{
				return ConvertRuntimeHelperToExpressionTree("Coalesce", boundExpression, boundExpression2);
			}
			BoundKind kind = convertedTestExpression.Kind;
			if (kind == BoundKind.Conversion)
			{
				BoundConversion boundConversion = (BoundConversion)convertedTestExpression;
				ParameterSymbol lambdaParameter = CreateCoalesceLambdaParameterSymbol(type2);
				BoundExpression body = BuildLambdaBodyForCoalesce(boundConversion, type, lambdaParameter, boundConversion.Checked);
				BoundExpression boundExpression3 = BuildLambdaForCoalesceCall(type, lambdaParameter, body);
				return ConvertRuntimeHelperToExpressionTree("Coalesce", boundExpression, boundExpression2, boundExpression3);
			}
			throw ExceptionUtilities.UnexpectedValue(convertedTestExpression.Kind);
		}

		private ParameterSymbol CreateCoalesceLambdaParameterSymbol(TypeSymbol paramType)
		{
			return _factory.SynthesizedParameter(paramType, "CoalesceLHS");
		}

		private BoundExpression CreateCoalesceLambdaParameter(ParameterSymbol paramSymbol)
		{
			return _factory.Parameter(paramSymbol).MakeRValue();
		}

		private BoundExpression BuildLambdaForCoalesceCall(TypeSymbol toType, ParameterSymbol lambdaParameter, BoundExpression body)
		{
			TypeSymbol typeSymbol = _factory.WellKnownType(WellKnownType.System_Linq_Expressions_ParameterExpression);
			LocalSymbol localSymbol = _factory.SynthesizedLocal(typeSymbol);
			BoundLocal boundLocal = _factory.Local(localSymbol, isLValue: true);
			BoundExpression right = ConvertRuntimeHelperToExpressionTree("Parameter", _factory.Typeof(lambdaParameter.Type), _factory.Literal("CoalesceLHS"));
			_parameterMap[lambdaParameter] = boundLocal.MakeRValue();
			BoundExpression boundExpression = Visit(body);
			_parameterMap.Remove(lambdaParameter);
			return _factory.Sequence(ImmutableArray.Create(localSymbol), ImmutableArray.Create((BoundExpression)_factory.AssignmentExpression(boundLocal, right)), ConvertRuntimeHelperToExpressionTree("Lambda", boundExpression, _factory.Array(typeSymbol, ImmutableArray.Create((BoundExpression)boundLocal.MakeRValue()))));
		}

		private BoundExpression BuildLambdaBodyForCoalesce(BoundConversion conversion, TypeSymbol toType, ParameterSymbol lambdaParameter, bool isChecked)
		{
			BoundExpression boundExpression = CreateCoalesceLambdaParameter(lambdaParameter);
			if ((conversion.ConversionKind & ConversionKind.UserDefined) == 0)
			{
				TypeSymbol type = boundExpression.Type;
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = _binder.GetNewCompoundUseSiteInfo(Diagnostics);
				ConversionKind num = Conversions.ClassifyPredefinedConversion(type, conversion.Operand.Type, ref useSiteInfo);
				Diagnostics.Add(conversion, useSiteInfo);
				if ((num & ConversionKind.NarrowingNullable) == ConversionKind.NarrowingNullable && !TypeSymbolExtensions.IsNullableType(toType))
				{
					return _factory.Convert(toType, CreateUserDefinedNullableToUnderlyingConversion(boundExpression, type, isChecked), isChecked);
				}
				return _factory.Convert(toType, boundExpression, isChecked);
			}
			return ReplaceArgWithParameterInUserDefinedConversion(conversion, toType, boundExpression, isChecked);
		}

		private BoundExpression CreateUserDefinedNullableToUnderlyingConversion(BoundExpression expression, TypeSymbol nullableType, bool isChecked)
		{
			TypeSymbol nullableUnderlyingType = TypeSymbolExtensions.GetNullableUnderlyingType(nullableType);
			MethodSymbol methodSymbol = (MethodSymbol)_factory.SpecialMember(SpecialMember.System_Nullable_T__op_Explicit_ToT);
			if ((object)methodSymbol == null)
			{
				return _factory.Convert(nullableUnderlyingType, expression, isChecked);
			}
			methodSymbol = (MethodSymbol)((SubstitutedNamedType)nullableType).GetMemberForDefinition(methodSymbol);
			SyntaxNode syntax = expression.Syntax;
			return new BoundConversion(syntax, new BoundUserDefinedConversion(syntax, new BoundCall(syntax, methodSymbol, null, null, ImmutableArray.Create(expression), null, nullableUnderlyingType, suppressObjectClone: true), 0, nullableType), ConversionKind.Narrowing | ConversionKind.UserDefined, isChecked, explicitCastInCode: false, null, nullableUnderlyingType);
		}

		private BoundExpression ReplaceArgWithParameterInUserDefinedConversion(BoundConversion conversion, TypeSymbol toType, BoundExpression parameter, bool isChecked)
		{
			BoundUserDefinedConversion boundUserDefinedConversion = (BoundUserDefinedConversion)conversion.Operand;
			BoundCall call = boundUserDefinedConversion.Call;
			TypeSymbol type = call.Type;
			MethodSymbol method = call.Method;
			BoundConversion boundConversion = boundUserDefinedConversion.OutConversionOpt;
			TypeSymbol type2 = method.Parameters[0].Type;
			TypeSymbol type3 = parameter.Type;
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = _binder.GetNewCompoundUseSiteInfo(Diagnostics);
			ConversionKind key = Conversions.ClassifyConversion(type3, type2, ref useSiteInfo).Key;
			Diagnostics.Add(conversion, useSiteInfo);
			bool flag = !Conversions.IsIdentityConversion(key);
			if (flag)
			{
				parameter = _factory.Convert(type2, parameter, isChecked);
			}
			call = call.Update(method, null, null, ImmutableArray.Create(parameter), default(BitVector), null, isLValue: false, suppressObjectClone: true, type);
			if (boundConversion != null)
			{
				boundConversion = boundConversion.Update(call, boundConversion.ConversionKind, boundConversion.Checked, boundConversion.ExplicitCastInCode, boundConversion.ConstantValueOpt, boundConversion.ExtendedInfoOpt, boundConversion.Type);
			}
			byte inOutConversionFlags = (byte)(((boundConversion != null) ? 2 : 0) + (flag ? 1 : 0));
			boundUserDefinedConversion = boundUserDefinedConversion.Update((BoundExpression)(((object)boundConversion) ?? ((object)call)), inOutConversionFlags, type3);
			ConversionKind conversionKind = conversion.ConversionKind & ~ConversionKind.Nullable;
			if (TypeSymbolExtensions.IsNullableType(type3) && !TypeSymbolExtensions.IsNullableType(method.Parameters[0].Type) && TypeSymbolExtensions.IsNullableType(toType) && !TypeSymbolExtensions.IsNullableType(method.ReturnType))
			{
				conversionKind |= ConversionKind.Nullable;
			}
			return conversion.Update(boundUserDefinedConversion, conversionKind, conversion.Checked, conversion.ExplicitCastInCode, conversion.ConstantValueOpt, conversion.ExtendedInfoOpt, toType);
		}

		private BoundExpression VisitConversion(BoundConversion node)
		{
			if (Conversions.IsIdentityConversion(node.ConversionKind) && !TypeSymbolExtensions.IsFloatingType(node.Type))
			{
				return VisitInternal(node.Operand);
			}
			return ConvertExpression(node.Operand, node.ConversionKind, node.Operand.Type, node.Type, node.Checked, node.ExplicitCastInCode, ConversionSemantics.Default);
		}

		private BoundExpression VisitDirectCast(BoundDirectCast node)
		{
			if (Conversions.IsIdentityConversion(node.ConversionKind))
			{
				return VisitInternal(node.Operand);
			}
			return ConvertExpression(node.Operand, node.ConversionKind, node.Operand.Type, node.Type, isChecked: false, explicitCastInCode: true, ConversionSemantics.DirectCast);
		}

		private BoundExpression VisitTryCast(BoundTryCast node)
		{
			if (Conversions.IsIdentityConversion(node.ConversionKind))
			{
				return VisitInternal(node.Operand);
			}
			return ConvertExpression(node.Operand, node.ConversionKind, node.Operand.Type, node.Type, isChecked: false, explicitCastInCode: true, ConversionSemantics.TryCast);
		}

		private BoundExpression ConvertExpression(BoundExpression operand, ConversionKind conversion, TypeSymbol typeFrom, TypeSymbol typeTo, bool isChecked, bool explicitCastInCode, ConversionSemantics semantics)
		{
			bool flag = TypeSymbolExtensions.IsNullableType(typeTo);
			TypeSymbol nullableUnderlyingTypeOrSelf = TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(typeTo);
			if (BoundExpressionExtensions.IsNothingLiteral(operand))
			{
				if (TypeSymbolExtensions.IsTypeParameter(nullableUnderlyingTypeOrSelf))
				{
					if (flag)
					{
						return Convert(VisitInternal(_factory.Null(ObjectType)), typeTo, isChecked: false);
					}
					return Default(typeTo);
				}
				if (nullableUnderlyingTypeOrSelf.IsReferenceType)
				{
					return CreateLiteralExpression(operand, typeTo);
				}
				if (flag)
				{
					return CreateLiteralExpression(operand, typeTo);
				}
				return InitWithParameterlessValueTypeConstructor(typeTo);
			}
			if (operand.Kind == BoundKind.Lambda)
			{
				return ConvertLambda((BoundLambda)operand, typeTo);
			}
			if ((conversion & ConversionKind.UserDefined) != 0)
			{
				BoundUserDefinedConversion node = (BoundUserDefinedConversion)operand;
				return CreateUserDefinedConversion(node, typeTo, (conversion & ConversionKind.Nullable) != 0, isChecked);
			}
			if (TypeSymbolExtensions.IsInterfaceType(typeTo) && typeFrom.IsValueType)
			{
				BoundExpression rewrittenOperand = CreateBuiltInConversion(typeFrom, ObjectType, Visit(operand), isChecked, explicitCastInCode, semantics);
				return CreateBuiltInConversion(ObjectType, typeTo, rewrittenOperand, isChecked, explicitCastInCode, semantics);
			}
			return CreateBuiltInConversion(typeFrom, typeTo, Visit(operand), isChecked, explicitCastInCode, semantics);
		}

		private BoundExpression ConvertLambda(BoundLambda node, TypeSymbol type)
		{
			if (TypeSymbolExtensions.IsExpressionTree(type, _binder))
			{
				type = TypeSymbolExtensions.ExpressionTargetDelegate(type, _factory.Compilation);
				BoundExpression boundExpression = VisitLambdaInternal(node, (NamedTypeSymbol)type);
				return ConvertRuntimeHelperToExpressionTree("Quote", boundExpression);
			}
			return VisitLambdaInternal(node, (NamedTypeSymbol)type);
		}

		private BoundExpression CreateBuiltInConversion(TypeSymbol typeFrom, TypeSymbol typeTo, BoundExpression rewrittenOperand, bool isChecked, bool isExplicit, ConversionSemantics semantics, bool specialConversionForNullable = false)
		{
			bool flag = TypeSymbolExtensions.IsNullableType(typeFrom);
			bool flag2 = TypeSymbolExtensions.IsNullableType(typeTo);
			TypeSymbol nullableUnderlyingTypeOrSelf = TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(typeTo);
			TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(nullableUnderlyingTypeOrSelf);
			TypeSymbol nullableUnderlyingTypeOrSelf2 = TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(typeFrom);
			TypeSymbol enumUnderlyingTypeOrSelf2 = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(nullableUnderlyingTypeOrSelf2);
			if (flag || flag2)
			{
				if (flag && !flag2 && !TypeSymbolExtensions.IsObjectType(typeTo))
				{
					if (TypeSymbol.Equals(nullableUnderlyingTypeOrSelf2, nullableUnderlyingTypeOrSelf, TypeCompareKind.ConsiderEverything))
					{
						if (specialConversionForNullable)
						{
							return ConvertNullableToUnderlying(rewrittenOperand, typeFrom, isChecked);
						}
						return Convert(rewrittenOperand, typeTo, isChecked && IsIntegralType(nullableUnderlyingTypeOrSelf));
					}
					TypeSymbol typeSymbol = nullableUnderlyingTypeOrSelf2;
					rewrittenOperand = CreateBuiltInConversion(typeFrom, typeSymbol, rewrittenOperand, isChecked, isExplicit, ConversionSemantics.Default, specialConversionForNullable: true);
					return CreateBuiltInConversion(typeSymbol, typeTo, rewrittenOperand, isChecked, isExplicit, ConversionSemantics.Default, specialConversionForNullable: true);
				}
				if (flag2 && !flag && !TypeSymbolExtensions.IsObjectType(typeFrom))
				{
					if (TypeSymbol.Equals(nullableUnderlyingTypeOrSelf2, nullableUnderlyingTypeOrSelf, TypeCompareKind.ConsiderEverything))
					{
						if (specialConversionForNullable)
						{
							return ConvertUnderlyingToNullable(rewrittenOperand, typeTo, isChecked);
						}
						return Convert(rewrittenOperand, typeTo, isChecked && IsIntegralType(nullableUnderlyingTypeOrSelf));
					}
					TypeSymbol typeSymbol2 = nullableUnderlyingTypeOrSelf;
					rewrittenOperand = CreateBuiltInConversion(typeFrom, typeSymbol2, rewrittenOperand, isChecked, isExplicit, ConversionSemantics.Default, specialConversionForNullable: true);
					return CreateBuiltInConversion(typeSymbol2, typeTo, rewrittenOperand, isChecked, isExplicit, ConversionSemantics.Default, specialConversionForNullable: true);
				}
			}
			MethodSymbol methodSymbol = null;
			if (semantics == ConversionSemantics.Default)
			{
				methodSymbol = GetConversionHelperMethod(enumUnderlyingTypeOrSelf2.SpecialType, enumUnderlyingTypeOrSelf.SpecialType);
			}
			if ((object)methodSymbol != null)
			{
				TypeSymbol typeSymbol3 = methodSymbol.Parameters[0].Type;
				if (flag)
				{
					typeSymbol3 = _factory.NullableOf(typeSymbol3);
				}
				TypeSymbol typeSymbol4 = methodSymbol.ReturnType;
				if (flag2)
				{
					typeSymbol4 = _factory.NullableOf(typeSymbol4);
				}
				BoundExpression expr = ConvertIfNeeded(rewrittenOperand, typeFrom, typeSymbol3, isChecked && IsIntegralType(typeSymbol3));
				BoundExpression operand = Convert(expr, typeSymbol4, methodSymbol, isChecked && IsIntegralType(typeSymbol4));
				return ConvertIfNeeded(operand, typeSymbol4, typeTo, isChecked && IsIntegralType(enumUnderlyingTypeOrSelf));
			}
			if (TypeSymbolExtensions.IsObjectType(enumUnderlyingTypeOrSelf2) && TypeSymbolExtensions.IsTypeParameter(enumUnderlyingTypeOrSelf))
			{
				switch (semantics)
				{
				case ConversionSemantics.DirectCast:
					return Convert(rewrittenOperand, typeTo, isChecked: false);
				case ConversionSemantics.TryCast:
					return CreateTypeAs(rewrittenOperand, typeTo);
				default:
				{
					MethodSymbol methodSymbol2 = _factory.WellKnownMember<MethodSymbol>(WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToGenericParameter_T_Object);
					return Call(_factory.Null(), methodSymbol2.Construct(typeTo), rewrittenOperand);
				}
				}
			}
			if (TypeSymbolExtensions.IsTypeParameter(enumUnderlyingTypeOrSelf2))
			{
				if (semantics == ConversionSemantics.TryCast)
				{
					return CreateTypeAs((typeTo.SpecialType == SpecialType.System_Object) ? rewrittenOperand : Convert(rewrittenOperand, ObjectType, isChecked: false), typeTo);
				}
				BoundExpression operand2 = Convert(rewrittenOperand, ObjectType, isChecked: false);
				return ConvertIfNeeded(operand2, _factory.SpecialType(SpecialType.System_Object), typeTo, isChecked: false);
			}
			if (TypeSymbolExtensions.IsStringType(enumUnderlyingTypeOrSelf) && TypeSymbolExtensions.IsCharSZArray(enumUnderlyingTypeOrSelf2))
			{
				return New(SpecialMember.System_String__CtorSZArrayChar, rewrittenOperand);
			}
			if (enumUnderlyingTypeOrSelf2.IsReferenceType && TypeSymbolExtensions.IsCharSZArray(enumUnderlyingTypeOrSelf))
			{
				Symbol symbol;
				TypeSymbol newType;
				if (TypeSymbolExtensions.IsStringType(enumUnderlyingTypeOrSelf2))
				{
					symbol = _factory.WellKnownMember<Symbol>(WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharArrayRankOneString);
					newType = _factory.SpecialType(SpecialType.System_String);
				}
				else
				{
					symbol = _factory.WellKnownMember<Symbol>(WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharArrayRankOneObject);
					newType = _factory.SpecialType(SpecialType.System_Object);
				}
				return Convert(ConvertIfNeeded(rewrittenOperand, typeFrom, newType, isChecked: false), typeTo, (MethodSymbol)symbol, isChecked: false);
			}
			if (TypeSymbolExtensions.IsBooleanType(enumUnderlyingTypeOrSelf2) && TypeSymbolExtensions.IsNumericType(enumUnderlyingTypeOrSelf))
			{
				TypeSymbol typeSymbol5 = GetSignedVersionOfNumericType(enumUnderlyingTypeOrSelf);
				if (typeSymbol5.SpecialType == SpecialType.System_SByte)
				{
					typeSymbol5 = _factory.SpecialType(SpecialType.System_Int32);
				}
				if (isChecked && (object)typeSymbol5 != enumUnderlyingTypeOrSelf)
				{
					isChecked = false;
				}
				if (flag && (object)typeSymbol5 == enumUnderlyingTypeOrSelf)
				{
					typeSymbol5 = _factory.NullableOf(typeSymbol5);
				}
				BoundExpression expr2 = Convert(rewrittenOperand, typeSymbol5, isChecked && IsIntegralType(typeSymbol5));
				BoundExpression operand3 = Negate(expr2);
				return ConvertIfNeeded(operand3, typeSymbol5, typeTo, isChecked && IsIntegralType(typeSymbol5));
			}
			if (isExplicit && TypeSymbolExtensions.IsFloatingType(enumUnderlyingTypeOrSelf))
			{
				return Convert(rewrittenOperand, typeTo, isChecked && IsIntegralType(enumUnderlyingTypeOrSelf));
			}
			if (semantics == ConversionSemantics.TryCast)
			{
				return CreateTypeAsIfNeeded(rewrittenOperand, typeFrom, typeTo);
			}
			return ConvertIfNeeded(rewrittenOperand, typeFrom, typeTo, isChecked && IsIntegralType(enumUnderlyingTypeOrSelf));
		}

		private TypeSymbol GetSignedVersionOfNumericType(TypeSymbol type)
		{
			return type.SpecialType switch
			{
				SpecialType.System_Byte => _factory.SpecialType(SpecialType.System_SByte), 
				SpecialType.System_UInt16 => _factory.SpecialType(SpecialType.System_Int16), 
				SpecialType.System_UInt32 => _factory.SpecialType(SpecialType.System_Int32), 
				SpecialType.System_UInt64 => _factory.SpecialType(SpecialType.System_Int64), 
				_ => type, 
			};
		}

		private BoundExpression ConvertUnderlyingToNullable(BoundExpression operand, TypeSymbol nullableType, bool isChecked)
		{
			if (isChecked && !IsIntegralType(nullableType))
			{
				isChecked = false;
			}
			MethodSymbol methodSymbol = (MethodSymbol)_factory.SpecialMember(SpecialMember.System_Nullable_T__op_Implicit_FromT);
			if ((object)methodSymbol != null)
			{
				SubstitutedNamedType substitutedNamedType = (SubstitutedNamedType)nullableType;
				return Convert(operand, nullableType, (MethodSymbol)substitutedNamedType.GetMemberForDefinition(methodSymbol), isChecked);
			}
			return Convert(operand, nullableType, isChecked);
		}

		private BoundExpression ConvertNullableToUnderlying(BoundExpression operand, TypeSymbol nullableType, bool isChecked)
		{
			TypeSymbol nullableUnderlyingType = TypeSymbolExtensions.GetNullableUnderlyingType(nullableType);
			if (isChecked && !IsIntegralType(nullableUnderlyingType))
			{
				isChecked = false;
			}
			MethodSymbol methodSymbol = (MethodSymbol)_factory.SpecialMember(SpecialMember.System_Nullable_T__op_Explicit_ToT);
			if ((object)methodSymbol != null)
			{
				SubstitutedNamedType substitutedNamedType = (SubstitutedNamedType)nullableType;
				return Convert(operand, nullableUnderlyingType, (MethodSymbol)substitutedNamedType.GetMemberForDefinition(methodSymbol), isChecked);
			}
			return Convert(operand, nullableUnderlyingType, isChecked);
		}

		private BoundExpression CreateUserDefinedConversion(BoundUserDefinedConversion node, TypeSymbol resultType, bool isLifted, bool isChecked)
		{
			BoundCall call = node.Call;
			MethodSymbol method = call.Method;
			TypeSymbol type = call.Type;
			if (isLifted)
			{
				BoundExpression operand = node.Operand;
				if (TypeSymbolExtensions.IsNullableType(operand.Type) == TypeSymbolExtensions.IsNullableType(resultType))
				{
					return Convert(Visit(operand), resultType, method, isChecked && IsIntegralType(resultType));
				}
			}
			BoundExpression expr = Visit(call.Arguments[0]);
			BoundExpression boundExpression = Convert(expr, type, method, isChecked && IsIntegralType(type));
			BoundConversion outConversionOpt = node.OutConversionOpt;
			if (outConversionOpt != null)
			{
				return CreateBuiltInConversion(type, resultType, boundExpression, outConversionOpt.Checked, outConversionOpt.ExplicitCastInCode, ConversionSemantics.Default);
			}
			return boundExpression;
		}

		private BoundExpression CreateTypeAs(BoundExpression expr, TypeSymbol type)
		{
			return ConvertRuntimeHelperToExpressionTree("TypeAs", expr, _factory.Typeof(type));
		}

		private BoundExpression CreateTypeAsIfNeeded(BoundExpression operand, TypeSymbol oldType, TypeSymbol newType)
		{
			if (!TypeSymbol.Equals(oldType, newType, TypeCompareKind.ConsiderEverything))
			{
				return CreateTypeAs(operand, newType);
			}
			return operand;
		}

		private BoundExpression Convert(BoundExpression expr, TypeSymbol type, bool isChecked)
		{
			return ConvertRuntimeHelperToExpressionTree(isChecked ? "ConvertChecked" : "Convert", expr, _factory.Typeof(type));
		}

		private BoundExpression Convert(BoundExpression expr, TypeSymbol type, MethodSymbol helper, bool isChecked)
		{
			return ConvertRuntimeHelperToExpressionTree(isChecked ? "ConvertChecked" : "Convert", expr, _factory.Typeof(type), _factory.MethodInfo(helper));
		}

		private BoundExpression ConvertIfNeeded(BoundExpression operand, TypeSymbol oldType, TypeSymbol newType, bool isChecked)
		{
			if (!TypeSymbol.Equals(oldType, newType, TypeCompareKind.ConsiderEverything))
			{
				return Convert(operand, newType, isChecked);
			}
			return operand;
		}

		private MethodSymbol GetConversionHelperMethod(SpecialType stFrom, SpecialType stTo)
		{
			WellKnownMember wellKnownMember = (WellKnownMember)(-1);
			SpecialMember specialMember = (SpecialMember)(-1);
			switch (stTo)
			{
			case SpecialType.System_Boolean:
				switch (stFrom)
				{
				case SpecialType.System_SByte:
					wellKnownMember = WellKnownMember.System_Convert__ToBooleanInt32;
					break;
				case SpecialType.System_Byte:
					wellKnownMember = WellKnownMember.System_Convert__ToBooleanInt32;
					break;
				case SpecialType.System_Int16:
					wellKnownMember = WellKnownMember.System_Convert__ToBooleanInt32;
					break;
				case SpecialType.System_UInt16:
					wellKnownMember = WellKnownMember.System_Convert__ToBooleanInt32;
					break;
				case SpecialType.System_Int32:
					wellKnownMember = WellKnownMember.System_Convert__ToBooleanInt32;
					break;
				case SpecialType.System_UInt32:
					wellKnownMember = WellKnownMember.System_Convert__ToBooleanUInt32;
					break;
				case SpecialType.System_Int64:
					wellKnownMember = WellKnownMember.System_Convert__ToBooleanInt64;
					break;
				case SpecialType.System_UInt64:
					wellKnownMember = WellKnownMember.System_Convert__ToBooleanUInt64;
					break;
				case SpecialType.System_Single:
					wellKnownMember = WellKnownMember.System_Convert__ToBooleanSingle;
					break;
				case SpecialType.System_Double:
					wellKnownMember = WellKnownMember.System_Convert__ToBooleanDouble;
					break;
				case SpecialType.System_Decimal:
					wellKnownMember = WellKnownMember.System_Convert__ToBooleanDecimal;
					break;
				case SpecialType.System_String:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToBooleanString;
					break;
				case SpecialType.System_Object:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToBooleanObject;
					break;
				}
				break;
			case SpecialType.System_Byte:
				switch (stFrom)
				{
				case SpecialType.System_Double:
					wellKnownMember = WellKnownMember.System_Convert__ToByteDouble;
					break;
				case SpecialType.System_Single:
					wellKnownMember = WellKnownMember.System_Convert__ToByteSingle;
					break;
				case SpecialType.System_Decimal:
					specialMember = SpecialMember.System_Decimal__op_Explicit_ToByte;
					break;
				case SpecialType.System_String:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToByteString;
					break;
				case SpecialType.System_Object:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToByteObject;
					break;
				}
				break;
			case SpecialType.System_SByte:
				switch (stFrom)
				{
				case SpecialType.System_Double:
					wellKnownMember = WellKnownMember.System_Convert__ToSByteDouble;
					break;
				case SpecialType.System_Single:
					wellKnownMember = WellKnownMember.System_Convert__ToSByteSingle;
					break;
				case SpecialType.System_Decimal:
					specialMember = SpecialMember.System_Decimal__op_Explicit_ToSByte;
					break;
				case SpecialType.System_String:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSByteString;
					break;
				case SpecialType.System_Object:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSByteObject;
					break;
				}
				break;
			case SpecialType.System_Int16:
				switch (stFrom)
				{
				case SpecialType.System_Double:
					wellKnownMember = WellKnownMember.System_Convert__ToInt16Double;
					break;
				case SpecialType.System_Single:
					wellKnownMember = WellKnownMember.System_Convert__ToInt16Single;
					break;
				case SpecialType.System_Decimal:
					specialMember = SpecialMember.System_Decimal__op_Explicit_ToInt16;
					break;
				case SpecialType.System_String:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToShortString;
					break;
				case SpecialType.System_Object:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToShortObject;
					break;
				}
				break;
			case SpecialType.System_UInt16:
				switch (stFrom)
				{
				case SpecialType.System_Double:
					wellKnownMember = WellKnownMember.System_Convert__ToUInt16Double;
					break;
				case SpecialType.System_Single:
					wellKnownMember = WellKnownMember.System_Convert__ToUInt16Single;
					break;
				case SpecialType.System_Decimal:
					specialMember = SpecialMember.System_Decimal__op_Explicit_ToUInt16;
					break;
				case SpecialType.System_String:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUShortString;
					break;
				case SpecialType.System_Object:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUShortObject;
					break;
				}
				break;
			case SpecialType.System_Int32:
				switch (stFrom)
				{
				case SpecialType.System_Double:
					wellKnownMember = WellKnownMember.System_Convert__ToInt32Double;
					break;
				case SpecialType.System_Single:
					wellKnownMember = WellKnownMember.System_Convert__ToInt32Single;
					break;
				case SpecialType.System_Decimal:
					specialMember = SpecialMember.System_Decimal__op_Explicit_ToInt32;
					break;
				case SpecialType.System_String:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToIntegerString;
					break;
				case SpecialType.System_Object:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToIntegerObject;
					break;
				}
				break;
			case SpecialType.System_UInt32:
				switch (stFrom)
				{
				case SpecialType.System_Double:
					wellKnownMember = WellKnownMember.System_Convert__ToUInt32Double;
					break;
				case SpecialType.System_Single:
					wellKnownMember = WellKnownMember.System_Convert__ToUInt32Single;
					break;
				case SpecialType.System_Decimal:
					specialMember = SpecialMember.System_Decimal__op_Explicit_ToUInt32;
					break;
				case SpecialType.System_String:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUIntegerString;
					break;
				case SpecialType.System_Object:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUIntegerObject;
					break;
				}
				break;
			case SpecialType.System_Int64:
				switch (stFrom)
				{
				case SpecialType.System_Double:
					wellKnownMember = WellKnownMember.System_Convert__ToInt64Double;
					break;
				case SpecialType.System_Single:
					wellKnownMember = WellKnownMember.System_Convert__ToInt64Single;
					break;
				case SpecialType.System_Decimal:
					specialMember = SpecialMember.System_Decimal__op_Explicit_ToInt64;
					break;
				case SpecialType.System_String:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToLongString;
					break;
				case SpecialType.System_Object:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToLongObject;
					break;
				}
				break;
			case SpecialType.System_UInt64:
				switch (stFrom)
				{
				case SpecialType.System_Double:
					wellKnownMember = WellKnownMember.System_Convert__ToUInt64Double;
					break;
				case SpecialType.System_Single:
					wellKnownMember = WellKnownMember.System_Convert__ToUInt64Single;
					break;
				case SpecialType.System_Decimal:
					specialMember = SpecialMember.System_Decimal__op_Explicit_ToUInt64;
					break;
				case SpecialType.System_String:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToULongString;
					break;
				case SpecialType.System_Object:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToULongObject;
					break;
				}
				break;
			case SpecialType.System_Decimal:
				switch (stFrom)
				{
				case SpecialType.System_SByte:
					specialMember = SpecialMember.System_Decimal__op_Implicit_FromInt32;
					break;
				case SpecialType.System_Byte:
					specialMember = SpecialMember.System_Decimal__op_Implicit_FromInt32;
					break;
				case SpecialType.System_Int16:
					specialMember = SpecialMember.System_Decimal__op_Implicit_FromInt32;
					break;
				case SpecialType.System_UInt16:
					specialMember = SpecialMember.System_Decimal__op_Implicit_FromInt32;
					break;
				case SpecialType.System_Int32:
					specialMember = SpecialMember.System_Decimal__op_Implicit_FromInt32;
					break;
				case SpecialType.System_UInt32:
					specialMember = SpecialMember.System_Decimal__op_Implicit_FromUInt32;
					break;
				case SpecialType.System_Int64:
					specialMember = SpecialMember.System_Decimal__op_Implicit_FromInt64;
					break;
				case SpecialType.System_UInt64:
					specialMember = SpecialMember.System_Decimal__op_Implicit_FromUInt64;
					break;
				case SpecialType.System_Single:
					specialMember = SpecialMember.System_Decimal__op_Explicit_FromSingle;
					break;
				case SpecialType.System_Double:
					specialMember = SpecialMember.System_Decimal__op_Explicit_FromDouble;
					break;
				case SpecialType.System_Boolean:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalBoolean;
					break;
				case SpecialType.System_String:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalString;
					break;
				case SpecialType.System_Object:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalObject;
					break;
				}
				break;
			case SpecialType.System_Single:
				switch (stFrom)
				{
				case SpecialType.System_Decimal:
					specialMember = SpecialMember.System_Decimal__op_Explicit_ToSingle;
					break;
				case SpecialType.System_String:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSingleString;
					break;
				case SpecialType.System_Object:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSingleObject;
					break;
				}
				break;
			case SpecialType.System_Double:
				switch (stFrom)
				{
				case SpecialType.System_Decimal:
					specialMember = SpecialMember.System_Decimal__op_Explicit_ToDouble;
					break;
				case SpecialType.System_String:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDoubleString;
					break;
				case SpecialType.System_Object:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDoubleObject;
					break;
				}
				break;
			case SpecialType.System_Char:
				switch (stFrom)
				{
				case SpecialType.System_String:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharString;
					break;
				case SpecialType.System_Object:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharObject;
					break;
				}
				break;
			case SpecialType.System_String:
				switch (stFrom)
				{
				case SpecialType.System_Boolean:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringBoolean;
					break;
				case SpecialType.System_SByte:
				case SpecialType.System_Int16:
				case SpecialType.System_Int32:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringInt32;
					break;
				case SpecialType.System_Byte:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringByte;
					break;
				case SpecialType.System_UInt16:
				case SpecialType.System_UInt32:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringUInt32;
					break;
				case SpecialType.System_Int64:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringInt64;
					break;
				case SpecialType.System_UInt64:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringUInt64;
					break;
				case SpecialType.System_Single:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringSingle;
					break;
				case SpecialType.System_Double:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringDouble;
					break;
				case SpecialType.System_Decimal:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringDecimal;
					break;
				case SpecialType.System_DateTime:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringDateTime;
					break;
				case SpecialType.System_Char:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringChar;
					break;
				case SpecialType.System_Object:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringObject;
					break;
				}
				break;
			case SpecialType.System_DateTime:
				switch (stFrom)
				{
				case SpecialType.System_String:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDateString;
					break;
				case SpecialType.System_Object:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDateObject;
					break;
				}
				break;
			}
			if (wellKnownMember >= WellKnownMember.System_Math__RoundDouble)
			{
				return _factory.WellKnownMember<MethodSymbol>(wellKnownMember);
			}
			if (specialMember >= SpecialMember.System_String__CtorSZArrayChar)
			{
				return (MethodSymbol)_factory.SpecialMember(specialMember);
			}
			return null;
		}

		private BoundExpression VisitUnaryOperator(BoundUnaryOperator node)
		{
			BoundExpression operand = node.Operand;
			TypeSymbol type = operand.Type;
			TypeSymbol nullableUnderlyingTypeOrSelf = TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(type);
			TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(nullableUnderlyingTypeOrSelf);
			SpecialType specialType = enumUnderlyingTypeOrSelf.SpecialType;
			BoundExpression boundExpression = Visit(operand);
			UnaryOperatorKind unaryOperatorKind = node.OperatorKind & UnaryOperatorKind.OpMask;
			bool flag = node.Checked && TypeSymbolExtensions.IsIntegralType(enumUnderlyingTypeOrSelf);
			string text = null;
			MethodSymbol methodSymbol;
			bool flag2;
			bool flag3;
			BoundExpression boundExpression2;
			switch (unaryOperatorKind)
			{
			case UnaryOperatorKind.Plus:
			{
				if (!type.IsReferenceType)
				{
					return boundExpression;
				}
				MethodSymbol helperForObjectUnaryOperation = GetHelperForObjectUnaryOperation(unaryOperatorKind);
				return ((object)helperForObjectUnaryOperation == null) ? boundExpression : ConvertRuntimeHelperToExpressionTree("UnaryPlus", boundExpression, _factory.MethodInfo(helperForObjectUnaryOperation));
			}
			case UnaryOperatorKind.Minus:
				text = (flag ? "NegateChecked" : "Negate");
				goto IL_00d4;
			case UnaryOperatorKind.Not:
				text = "Not";
				goto IL_00d4;
			default:
				{
					throw ExceptionUtilities.UnexpectedValue(unaryOperatorKind);
				}
				IL_00d4:
				methodSymbol = null;
				if (type.IsReferenceType)
				{
					methodSymbol = GetHelperForObjectUnaryOperation(unaryOperatorKind);
				}
				else if (TypeSymbolExtensions.IsDecimalType(enumUnderlyingTypeOrSelf))
				{
					methodSymbol = GetHelperForDecimalUnaryOperation(unaryOperatorKind);
				}
				if ((object)methodSymbol != null)
				{
					return ConvertRuntimeHelperToExpressionTree(text, boundExpression, _factory.MethodInfo(methodSymbol));
				}
				flag2 = specialType == SpecialType.System_Byte || specialType == SpecialType.System_SByte;
				flag3 = TypeSymbolExtensions.IsNullableType(type);
				boundExpression = GenerateCastsForBinaryAndUnaryOperator(boundExpression, flag3, nullableUnderlyingTypeOrSelf, flag && IsIntegralType(enumUnderlyingTypeOrSelf), flag2);
				boundExpression2 = ConvertRuntimeHelperToExpressionTree(text, boundExpression);
				if (flag2)
				{
					boundExpression2 = Convert(boundExpression2, flag3 ? _factory.NullableOf(enumUnderlyingTypeOrSelf) : enumUnderlyingTypeOrSelf, flag);
				}
				if (TypeSymbolExtensions.IsEnumType(nullableUnderlyingTypeOrSelf))
				{
					boundExpression2 = Convert(boundExpression2, type, isChecked: false);
				}
				return boundExpression2;
			}
		}

		private BoundExpression VisitNullableIsTrueOperator(BoundNullableIsTrueOperator node)
		{
			BoundExpression operand = node.Operand;
			BoundKind kind = operand.Kind;
			if (kind == BoundKind.UserDefinedUnaryOperator)
			{
				BoundUserDefinedUnaryOperator boundUserDefinedUnaryOperator = (BoundUserDefinedUnaryOperator)operand;
				UnaryOperatorKind operatorKind = boundUserDefinedUnaryOperator.OperatorKind;
				if ((operatorKind & UnaryOperatorKind.OpMask) == UnaryOperatorKind.IsTrue && (operatorKind & UnaryOperatorKind.Lifted) != 0)
				{
					BoundCall call = boundUserDefinedUnaryOperator.Call;
					TypeSymbol type = boundUserDefinedUnaryOperator.Operand.Type;
					ParameterSymbol lambdaParameter = CreateCoalesceLambdaParameterSymbol(type);
					BoundExpression body = BuildLambdaBodyForCoalesce(boundUserDefinedUnaryOperator.OperatorKind, call, node.Type, lambdaParameter);
					BoundExpression boundExpression = BuildLambdaForCoalesceCall(node.Type, lambdaParameter, body);
					return ConvertRuntimeHelperToExpressionTree("Coalesce", Visit(boundUserDefinedUnaryOperator.Operand), Visit(_factory.Literal(value: false)), boundExpression);
				}
			}
			if (operand.Kind == BoundKind.ObjectCreationExpression)
			{
				BoundObjectCreationExpression boundObjectCreationExpression = (BoundObjectCreationExpression)operand;
				if (boundObjectCreationExpression.Arguments.Length == 1)
				{
					return VisitInternal(boundObjectCreationExpression.Arguments[0]);
				}
			}
			return ConvertRuntimeHelperToExpressionTree("Coalesce", Visit(operand), Visit(_factory.Literal(value: false)));
		}

		private BoundExpression BuildLambdaBodyForCoalesce(UnaryOperatorKind opKind, BoundCall call, TypeSymbol resultType, ParameterSymbol lambdaParameter)
		{
			return AdjustCallForLiftedOperator(opKind, call.Update(call.Method, null, null, ImmutableArray.Create(CreateCoalesceLambdaParameter(lambdaParameter)), default(BitVector), null, call.IsLValue, suppressObjectClone: true, call.Type), resultType);
		}

		private BoundExpression VisitUserDefinedUnaryOperator(BoundUserDefinedUnaryOperator node)
		{
			UnaryOperatorKind unaryOperatorKind = node.OperatorKind & UnaryOperatorKind.OpMask;
			_ = node.OperatorKind & UnaryOperatorKind.Lifted;
			switch (unaryOperatorKind)
			{
			case UnaryOperatorKind.IsTrue:
			case UnaryOperatorKind.IsFalse:
				return RewriteUserDefinedOperator(node);
			case UnaryOperatorKind.Plus:
			case UnaryOperatorKind.Minus:
			case UnaryOperatorKind.Not:
				return ConvertRuntimeHelperToExpressionTree(GetUnaryOperatorMethodName(unaryOperatorKind, isChecked: false), Visit(node.Operand), _factory.MethodInfo(node.Call.Method));
			default:
				throw ExceptionUtilities.UnexpectedValue(unaryOperatorKind);
			}
		}

		private BoundExpression RewriteUserDefinedOperator(BoundUserDefinedUnaryOperator node)
		{
			BoundCall call = node.Call;
			UnaryOperatorKind operatorKind = node.OperatorKind;
			if ((operatorKind & UnaryOperatorKind.Lifted) == 0)
			{
				return VisitInternal(call);
			}
			return VisitInternal(AdjustCallForLiftedOperator(operatorKind, call, node.Type));
		}

		private MethodSymbol GetHelperForDecimalUnaryOperation(UnaryOperatorKind opKind)
		{
			opKind &= UnaryOperatorKind.OpMask;
			UnaryOperatorKind unaryOperatorKind = opKind;
			if ((uint)(unaryOperatorKind - 2) <= 1u)
			{
				SpecialMember sm = SpecialMember.System_Decimal__NegateDecimal;
				return (MethodSymbol)_factory.SpecialMember(sm);
			}
			throw ExceptionUtilities.UnexpectedValue(opKind);
		}

		private MethodSymbol GetHelperForObjectUnaryOperation(UnaryOperatorKind opKind)
		{
			opKind &= UnaryOperatorKind.OpMask;
			WellKnownMember wm = opKind switch
			{
				UnaryOperatorKind.Plus => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__PlusObjectObject, 
				UnaryOperatorKind.Minus => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__NegateObjectObject, 
				UnaryOperatorKind.Not => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__NotObjectObject, 
				_ => throw ExceptionUtilities.UnexpectedValue(opKind), 
			};
			return _factory.WellKnownMember<MethodSymbol>(wm);
		}

		private static string GetUnaryOperatorMethodName(UnaryOperatorKind opKind, bool isChecked)
		{
			return (opKind & UnaryOperatorKind.OpMask) switch
			{
				UnaryOperatorKind.Not => "Not", 
				UnaryOperatorKind.Plus => "UnaryPlus", 
				UnaryOperatorKind.Minus => isChecked ? "NegateChecked" : "Negate", 
				_ => throw ExceptionUtilities.UnexpectedValue(opKind), 
			};
		}
	}
}
