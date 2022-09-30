using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class MethodToClassRewriter<TProxy> : BoundTreeRewriterWithStackGuard
	{
		internal sealed class SynthesizedWrapperMethod : SynthesizedMethod
		{
			private readonly MethodSymbol _wrappedMethod;

			private readonly TypeSubstitution _typeMap;

			private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

			private readonly ImmutableArray<ParameterSymbol> _parameters;

			private readonly TypeSymbol _returnType;

			private readonly ImmutableArray<Location> _locations;

			internal override TypeSubstitution TypeMap => _typeMap;

			public MethodSymbol WrappedMethod => _wrappedMethod;

			public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

			public override ImmutableArray<TypeSymbol> TypeArguments
			{
				get
				{
					if (Arity > 0)
					{
						return StaticCast<TypeSymbol>.From(TypeParameters);
					}
					return ImmutableArray<TypeSymbol>.Empty;
				}
			}

			public override ImmutableArray<Location> Locations => _locations;

			public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

			public override TypeSymbol ReturnType => _returnType;

			public override bool IsShared => false;

			public override bool IsSub => _wrappedMethod.IsSub;

			public override bool IsVararg => _wrappedMethod.IsVararg;

			public override int Arity => _typeParameters.Length;

			public override Accessibility DeclaredAccessibility => Accessibility.Private;

			internal override int ParameterCount => _parameters.Length;

			internal override bool HasSpecialName => false;

			internal override bool GenerateDebugInfoImpl => false;

			internal SynthesizedWrapperMethod(InstanceTypeSymbol containingType, MethodSymbol methodToWrap, string wrapperName, SyntaxNode syntax)
				: base(syntax, containingType, wrapperName, isShared: false)
			{
				_locations = ImmutableArray.Create(syntax.GetLocation());
				_typeMap = null;
				if (!methodToWrap.IsGenericMethod)
				{
					_typeParameters = ImmutableArray<TypeParameterSymbol>.Empty;
					_wrappedMethod = methodToWrap;
				}
				else
				{
					_typeParameters = SynthesizedClonedTypeParameterSymbol.MakeTypeParameters(methodToWrap.OriginalDefinition.TypeParameters, this, SynthesizedMethod.CreateTypeParameter);
					TypeSymbol[] array = new TypeSymbol[_typeParameters.Length - 1 + 1];
					int num = _typeParameters.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						array[i] = _typeParameters[i];
					}
					MethodSymbol methodSymbol = methodToWrap.Construct(array.AsImmutableOrNull());
					_typeMap = TypeSubstitution.Create(methodSymbol.OriginalDefinition, methodSymbol.OriginalDefinition.TypeParameters, array.AsImmutableOrNull());
					_wrappedMethod = methodSymbol;
				}
				ParameterSymbol[] array2 = new ParameterSymbol[_wrappedMethod.ParameterCount - 1 + 1];
				int num2 = array2.Length - 1;
				for (int j = 0; j <= num2; j++)
				{
					ParameterSymbol parameterSymbol = _wrappedMethod.Parameters[j];
					array2[j] = SynthesizedMethod.WithNewContainerAndType(this, parameterSymbol.Type.InternalSubstituteTypeParameters(_typeMap).Type, parameterSymbol);
				}
				_parameters = array2.AsImmutableOrNull();
				_returnType = _wrappedMethod.ReturnType.InternalSubstituteTypeParameters(_typeMap).Type;
			}

			internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
			{
				base.AddSynthesizedAttributes(compilationState, ref attributes);
				VisualBasicCompilation declaringCompilation = DeclaringCompilation;
				Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
				Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeDebuggerHiddenAttribute());
			}

			internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
			{
				return false;
			}

			internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		protected readonly Dictionary<Symbol, TProxy> Proxies;

		protected readonly Dictionary<LocalSymbol, LocalSymbol> LocalMap;

		protected readonly Dictionary<ParameterSymbol, ParameterSymbol> ParameterMap;

		protected readonly Dictionary<BoundValuePlaceholderBase, BoundExpression> PlaceholderReplacementMap;

		protected readonly TypeCompilationState CompilationState;

		protected readonly BindingDiagnosticBag Diagnostics;

		protected readonly VariableSlotAllocator SlotAllocatorOpt;

		protected readonly bool PreserveOriginalLocals;

		protected abstract TypeSubstitution TypeMap { get; }

		protected abstract MethodSymbol CurrentMethod { get; }

		protected abstract MethodSymbol TopLevelMethod { get; }

		protected abstract bool IsInExpressionLambda { get; }

		private MethodSymbol SubstituteMethodForMyBaseOrMyClassCall(BoundExpression receiverOpt, MethodSymbol originalMethodBeingCalled)
		{
			if ((SymbolExtensions.IsMetadataVirtual(originalMethodBeingCalled) || IsInExpressionLambda) && receiverOpt != null && (receiverOpt.Kind == BoundKind.MyBaseReference || receiverOpt.Kind == BoundKind.MyClassReference))
			{
				NamedTypeSymbol containingType = CurrentMethod.ContainingType;
				TypeSymbol containingType2 = TopLevelMethod.ContainingType;
				if ((object)containingType != containingType2 || IsInExpressionLambda)
				{
					MethodSymbol methodSymbol = GetOrCreateMyBaseOrMyClassWrapperFunction(receiverOpt, originalMethodBeingCalled);
					if (methodSymbol.IsGenericMethod)
					{
						ImmutableArray<TypeSymbol> typeArguments = originalMethodBeingCalled.TypeArguments;
						TypeSymbol[] array = new TypeSymbol[typeArguments.Length - 1 + 1];
						int num = typeArguments.Length - 1;
						for (int i = 0; i <= num; i++)
						{
							array[i] = VisitType(typeArguments[i]);
						}
						methodSymbol = methodSymbol.Construct(array.AsImmutableOrNull());
					}
					return methodSymbol;
				}
			}
			return originalMethodBeingCalled;
		}

		private MethodSymbol GetOrCreateMyBaseOrMyClassWrapperFunction(BoundExpression receiver, MethodSymbol method)
		{
			method = method.ConstructedFrom;
			MethodSymbol methodWrapper = CompilationState.GetMethodWrapper(method);
			if ((object)methodWrapper != null)
			{
				return methodWrapper;
			}
			NamedTypeSymbol containingType = TopLevelMethod.ContainingType;
			bool flag = !method.ContainingType.Equals(containingType);
			SyntaxNode syntax = CurrentMethod.Syntax;
			string wrapperName = GeneratedNames.MakeBaseMethodWrapperName(method.Name, flag);
			SynthesizedWrapperMethod synthesizedWrapperMethod = new SynthesizedWrapperMethod((InstanceTypeSymbol)containingType, method, wrapperName, syntax);
			if (CompilationState.ModuleBuilderOpt != null)
			{
				CompilationState.ModuleBuilderOpt.AddSynthesizedDefinition(containingType, synthesizedWrapperMethod.GetCciAdapter());
			}
			MethodSymbol wrappedMethod = synthesizedWrapperMethod.WrappedMethod;
			BoundExpression[] array = new BoundExpression[synthesizedWrapperMethod.ParameterCount - 1 + 1];
			int num = synthesizedWrapperMethod.ParameterCount - 1;
			for (int i = 0; i <= num; i++)
			{
				ParameterSymbol parameterSymbol = synthesizedWrapperMethod.Parameters[i];
				array[i] = new BoundParameter(syntax, parameterSymbol, parameterSymbol.IsByRef, parameterSymbol.Type);
			}
			ParameterSymbol meParameter = synthesizedWrapperMethod.MeParameter;
			BoundExpression boundExpression = null;
			boundExpression = ((!flag) ? ((BoundExpression)new BoundMyClassReference(syntax, meParameter.Type)) : ((BoundExpression)new BoundMyBaseReference(syntax, meParameter.Type)));
			BoundCall boundCall = new BoundCall(syntax, wrappedMethod, null, boundExpression, array.AsImmutableOrNull(), null, wrappedMethod.ReturnType);
			BoundStatement body = ((!TypeSymbolExtensions.IsVoidType(wrappedMethod.ReturnType)) ? ((BoundStatement)new BoundReturnStatement(syntax, boundCall, null, null)) : ((BoundStatement)new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundStatement)new BoundExpressionStatement(syntax, boundCall), (BoundStatement)new BoundReturnStatement(syntax, null, null, null)))));
			CompilationState.AddMethodWrapper(method, synthesizedWrapperMethod, body);
			return synthesizedWrapperMethod;
		}

		internal abstract BoundExpression FramePointer(SyntaxNode syntax, NamedTypeSymbol frameClass);

		protected MethodToClassRewriter(VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, bool preserveOriginalLocals)
		{
			Proxies = new Dictionary<Symbol, TProxy>();
			LocalMap = new Dictionary<LocalSymbol, LocalSymbol>();
			ParameterMap = new Dictionary<ParameterSymbol, ParameterSymbol>();
			PlaceholderReplacementMap = new Dictionary<BoundValuePlaceholderBase, BoundExpression>();
			CompilationState = compilationState;
			Diagnostics = diagnostics;
			SlotAllocatorOpt = slotAllocatorOpt;
			PreserveOriginalLocals = preserveOriginalLocals;
		}

		public override BoundNode VisitLocalDeclaration(BoundLocalDeclaration node)
		{
			LocalSymbol localSymbol = node.LocalSymbol;
			TProxy value = default(TProxy);
			if (Proxies.TryGetValue(localSymbol, out value))
			{
				return null;
			}
			return node;
		}

		public sealed override TypeSymbol VisitType(TypeSymbol type)
		{
			if ((object)type == null)
			{
				return type;
			}
			return type.InternalSubstituteTypeParameters(TypeMap).Type;
		}

		public sealed override BoundNode VisitMethodInfo(BoundMethodInfo node)
		{
			return node.Update(VisitMethodSymbol(node.Method), VisitType(node.Type));
		}

		public override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
		{
			PropertySymbol propertySymbol = VisitPropertySymbol(node.PropertySymbol);
			BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
			ImmutableArray<BoundExpression> arguments = node.Arguments;
			BoundExpression[] array = new BoundExpression[arguments.Length - 1 + 1];
			int num = arguments.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = (BoundExpression)Visit(arguments[i]);
			}
			return node.Update(propertySymbol, null, node.AccessKind, node.IsWriteable, node.IsLValue, receiverOpt, array.AsImmutableOrNull(), node.DefaultArguments, VisitType(node.Type));
		}

		public override BoundNode VisitCall(BoundCall node)
		{
			BoundExpression receiverOpt = node.ReceiverOpt;
			BoundExpression boundExpression = (BoundExpression)Visit(receiverOpt);
			MethodSymbol methodSymbol = node.Method;
			ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
			TypeSymbol type = VisitType(node.Type);
			if (ShouldRewriteMethodSymbol(receiverOpt, boundExpression, methodSymbol))
			{
				MethodSymbol method = SubstituteMethodForMyBaseOrMyClassCall(receiverOpt, node.Method);
				methodSymbol = VisitMethodSymbol(method);
			}
			return node.Update(methodSymbol, null, boundExpression, arguments, node.DefaultArguments, node.ConstantValueOpt, node.IsLValue, node.SuppressObjectClone, type);
		}

		private bool ShouldRewriteMethodSymbol(BoundExpression originalReceiver, BoundExpression rewrittenReceiverOpt, MethodSymbol newMethod)
		{
			if (originalReceiver == rewrittenReceiverOpt && newMethod.IsDefinition && (TypeMap == null || !TypeMap.TargetGenericDefinition.Equals(newMethod)))
			{
				if (IsInExpressionLambda && rewrittenReceiverOpt != null)
				{
					if (!BoundExpressionExtensions.IsMyClassReference(rewrittenReceiverOpt))
					{
						return BoundExpressionExtensions.IsMyBaseReference(rewrittenReceiverOpt);
					}
					return true;
				}
				return false;
			}
			return true;
		}

		public sealed override BoundNode VisitParameter(BoundParameter node)
		{
			TProxy value = default(TProxy);
			if (Proxies.TryGetValue(node.ParameterSymbol, out value))
			{
				return MaterializeProxy(node, value);
			}
			ParameterSymbol value2 = null;
			if (ParameterMap.TryGetValue(node.ParameterSymbol, out value2))
			{
				return new BoundParameter(node.Syntax, value2, node.IsLValue, value2.Type, node.HasErrors);
			}
			return base.VisitParameter(node);
		}

		protected abstract BoundNode MaterializeProxy(BoundExpression origExpression, TProxy proxy);

		public sealed override BoundNode VisitLocal(BoundLocal node)
		{
			LocalSymbol localSymbol = node.LocalSymbol;
			if (localSymbol.IsConst)
			{
				return base.VisitLocal(node);
			}
			TProxy value = default(TProxy);
			if (Proxies.TryGetValue(localSymbol, out value))
			{
				return MaterializeProxy(node, value);
			}
			LocalSymbol value2 = null;
			if (LocalMap.TryGetValue(localSymbol, out value2))
			{
				return new BoundLocal(node.Syntax, value2, node.IsLValue, value2.Type, node.HasErrors);
			}
			return base.VisitLocal(node);
		}

		public override BoundNode VisitFieldInfo(BoundFieldInfo node)
		{
			return node.Update(VisitFieldSymbol(node.Field), VisitType(node.Type));
		}

		public override BoundNode VisitFieldAccess(BoundFieldAccess node)
		{
			return node.Update((BoundExpression)Visit(node.ReceiverOpt), VisitFieldSymbol(node.FieldSymbol), node.IsLValue, node.SuppressVirtualCalls, null, VisitType(node.Type));
		}

		public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
		{
			BoundDelegateCreationExpression boundDelegateCreationExpression = (BoundDelegateCreationExpression)base.VisitDelegateCreationExpression(node);
			MethodSymbol methodSymbol = boundDelegateCreationExpression.Method;
			BoundExpression receiverOpt = boundDelegateCreationExpression.ReceiverOpt;
			if (ShouldRewriteMethodSymbol(node.ReceiverOpt, receiverOpt, methodSymbol))
			{
				MethodSymbol method = SubstituteMethodForMyBaseOrMyClassCall(node.ReceiverOpt, node.Method);
				methodSymbol = VisitMethodSymbol(method);
			}
			return node.Update(receiverOpt, methodSymbol, boundDelegateCreationExpression.RelaxationLambdaOpt, boundDelegateCreationExpression.RelaxationReceiverPlaceholderOpt, null, boundDelegateCreationExpression.Type);
		}

		public override BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
		{
			BoundObjectCreationExpression boundObjectCreationExpression = (BoundObjectCreationExpression)base.VisitObjectCreationExpression(node);
			MethodSymbol constructorOpt = boundObjectCreationExpression.ConstructorOpt;
			if ((object)constructorOpt != null && ((object)node.Type != boundObjectCreationExpression.Type || !constructorOpt.IsDefinition))
			{
				MethodSymbol constructorOpt2 = VisitMethodSymbol(constructorOpt);
				boundObjectCreationExpression = node.Update(constructorOpt2, boundObjectCreationExpression.Arguments, boundObjectCreationExpression.DefaultArguments, boundObjectCreationExpression.InitializerOpt, boundObjectCreationExpression.Type);
			}
			return boundObjectCreationExpression;
		}

		private MethodSymbol VisitMethodSymbol(MethodSymbol method)
		{
			TypeSubstitution typeMap = TypeMap;
			if (typeMap != null)
			{
				MethodSymbol methodSymbol = method.OriginalDefinition;
				TypeSymbol typeSymbol = method.ContainingType.InternalSubstituteTypeParameters(typeMap).AsTypeSymbolOnly();
				if (typeSymbol is SubstitutedNamedType substitutedNamedType)
				{
					methodSymbol = (MethodSymbol)substitutedNamedType.GetMemberForDefinition(methodSymbol);
				}
				else if (typeSymbol is AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol anonymousTypeOrDelegatePublicSymbol)
				{
					methodSymbol = anonymousTypeOrDelegatePublicSymbol.FindSubstitutedMethodSymbol(methodSymbol);
				}
				if (methodSymbol.IsGenericMethod)
				{
					ImmutableArray<TypeSymbol> typeArguments = method.TypeArguments;
					TypeSymbol[] array = new TypeSymbol[typeArguments.Length - 1 + 1];
					int num = typeArguments.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						array[i] = VisitType(typeArguments[i]);
					}
					methodSymbol = methodSymbol.Construct(array);
				}
				return methodSymbol;
			}
			return method;
		}

		private PropertySymbol VisitPropertySymbol(PropertySymbol property)
		{
			TypeSubstitution typeMap = TypeMap;
			if (typeMap != null)
			{
				PropertySymbol propertySymbol = property.OriginalDefinition;
				TypeSymbol typeSymbol = property.ContainingType.InternalSubstituteTypeParameters(typeMap).AsTypeSymbolOnly();
				if (typeSymbol is SubstitutedNamedType substitutedNamedType)
				{
					propertySymbol = (PropertySymbol)substitutedNamedType.GetMemberForDefinition(propertySymbol);
				}
				else if (typeSymbol is AnonymousTypeManager.AnonymousTypePublicSymbol anonymousTypePublicSymbol)
				{
					AnonymousTypeManager.AnonymousTypePropertyPublicSymbol anonymousTypePropertyPublicSymbol = propertySymbol as AnonymousTypeManager.AnonymousTypePropertyPublicSymbol;
					propertySymbol = anonymousTypePublicSymbol.Properties[anonymousTypePropertyPublicSymbol.PropertyIndex];
				}
				return propertySymbol;
			}
			return property;
		}

		private FieldSymbol VisitFieldSymbol(FieldSymbol field)
		{
			TypeSubstitution typeMap = TypeMap;
			if (typeMap != null)
			{
				FieldSymbol fieldSymbol = field.OriginalDefinition;
				if (field.ContainingType.InternalSubstituteTypeParameters(typeMap).AsTypeSymbolOnly() is SubstitutedNamedType substitutedNamedType)
				{
					fieldSymbol = (FieldSymbol)substitutedNamedType.GetMemberForDefinition(fieldSymbol);
				}
				return fieldSymbol;
			}
			return field;
		}

		public override BoundNode VisitBlock(BoundBlock node)
		{
			return RewriteBlock(node);
		}

		public override BoundNode VisitSequence(BoundSequence node)
		{
			return RewriteSequence(node);
		}

		public abstract override BoundNode VisitCatchBlock(BoundCatchBlock node);

		protected BoundBlock RewriteBlock(BoundBlock node, ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals)
		{
			ImmutableArray<LocalSymbol>.Enumerator enumerator = node.Locals.GetEnumerator();
			while (enumerator.MoveNext())
			{
				LocalSymbol current = enumerator.Current;
				if (!PreserveOriginalLocals && Proxies.ContainsKey(current))
				{
					continue;
				}
				TypeSymbol typeSymbol = VisitType(current.Type);
				if (TypeSymbol.Equals(typeSymbol, current.Type, TypeCompareKind.ConsiderEverything))
				{
					LocalSymbol value = null;
					bool wasReplaced = false;
					if (!LocalMap.TryGetValue(current, out value))
					{
						value = CreateReplacementLocalOrReturnSelf(current, typeSymbol, onlyReplaceIfFunctionValue: true, out wasReplaced);
					}
					if (wasReplaced)
					{
						LocalMap.Add(current, value);
					}
					newLocals.Add(value);
				}
				else
				{
					bool wasReplaced2 = false;
					LocalSymbol localSymbol = CreateReplacementLocalOrReturnSelf(current, typeSymbol, onlyReplaceIfFunctionValue: false, out wasReplaced2);
					newLocals.Add(localSymbol);
					LocalMap.Add(current, localSymbol);
				}
			}
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			int num = 0;
			ImmutableArray<BoundStatement> statements = node.Statements;
			if (prologue.Count > 0)
			{
				if (statements.Length > 0 && statements[0].Syntax != null)
				{
					bool flag = false;
					switch (statements[0].Kind)
					{
					case BoundKind.SequencePoint:
						flag = ((BoundSequencePoint)statements[0]).StatementOpt == null;
						break;
					case BoundKind.SequencePointWithSpan:
						flag = ((BoundSequencePointWithSpan)statements[0]).StatementOpt == null;
						break;
					}
					if (flag)
					{
						BoundStatement boundStatement = (BoundStatement)Visit(statements[0]);
						if (boundStatement != null)
						{
							instance.Add(boundStatement);
						}
						num = 1;
					}
				}
				instance.Add(BoundNodeExtensions.MakeCompilerGenerated(new BoundSequencePoint(null, null)));
			}
			ArrayBuilder<BoundExpression>.Enumerator enumerator2 = prologue.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				BoundExpression current2 = enumerator2.Current;
				instance.Add(new BoundExpressionStatement(current2.Syntax, current2));
			}
			prologue.Free();
			int num2 = num;
			int num3 = statements.Length - 1;
			for (int i = num2; i <= num3; i++)
			{
				BoundStatement boundStatement2 = (BoundStatement)Visit(statements[i]);
				if (boundStatement2 != null)
				{
					instance.Add(boundStatement2);
				}
			}
			return node.Update(node.StatementListSyntax, newLocals.ToImmutableAndFree(), instance.ToImmutableAndFree());
		}

		protected BoundBlock RewriteBlock(BoundBlock node)
		{
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
			ArrayBuilder<LocalSymbol> instance2 = ArrayBuilder<LocalSymbol>.GetInstance();
			return RewriteBlock(node, instance, instance2);
		}

		protected static LocalSymbol CreateReplacementLocalOrReturnSelf(LocalSymbol originalLocal, TypeSymbol newType, bool onlyReplaceIfFunctionValue = false, out bool wasReplaced = false)
		{
			if (!onlyReplaceIfFunctionValue || originalLocal.IsFunctionValue)
			{
				wasReplaced = true;
				return LocalSymbol.Create(originalLocal, newType);
			}
			wasReplaced = false;
			return originalLocal;
		}

		protected BoundSequence RewriteSequence(BoundSequence node)
		{
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
			ArrayBuilder<LocalSymbol> instance2 = ArrayBuilder<LocalSymbol>.GetInstance();
			return RewriteSequence(node, instance, instance2);
		}

		protected BoundSequence RewriteSequence(BoundSequence node, ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals)
		{
			ImmutableArray<LocalSymbol>.Enumerator enumerator = node.Locals.GetEnumerator();
			while (enumerator.MoveNext())
			{
				LocalSymbol current = enumerator.Current;
				if (!Proxies.ContainsKey(current))
				{
					TypeSymbol typeSymbol = VisitType(current.Type);
					if (TypeSymbol.Equals(typeSymbol, current.Type, TypeCompareKind.ConsiderEverything))
					{
						newLocals.Add(current);
						continue;
					}
					bool wasReplaced = false;
					LocalSymbol localSymbol = CreateReplacementLocalOrReturnSelf(current, typeSymbol, onlyReplaceIfFunctionValue: false, out wasReplaced);
					newLocals.Add(localSymbol);
					LocalMap.Add(current, localSymbol);
				}
			}
			ImmutableArray<BoundExpression>.Enumerator enumerator2 = node.SideEffects.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				BoundExpression current2 = enumerator2.Current;
				BoundExpression boundExpression = (BoundExpression)Visit(current2);
				if (boundExpression != null)
				{
					prologue.Add(boundExpression);
				}
			}
			BoundExpression boundExpression2 = (BoundExpression)Visit(node.ValueOpt);
			return node.Update(newLocals.ToImmutableAndFree(), prologue.ToImmutableAndFree(), boundExpression2, (boundExpression2 == null) ? node.Type : boundExpression2.Type);
		}

		public override BoundNode VisitRValuePlaceholder(BoundRValuePlaceholder node)
		{
			return PlaceholderReplacementMap[node];
		}

		public override BoundNode VisitLValuePlaceholder(BoundLValuePlaceholder node)
		{
			return PlaceholderReplacementMap[node];
		}

		public override BoundNode VisitAwaitOperator(BoundAwaitOperator node)
		{
			BoundRValuePlaceholder awaitableInstancePlaceholder = node.AwaitableInstancePlaceholder;
			PlaceholderReplacementMap.Add(awaitableInstancePlaceholder, awaitableInstancePlaceholder.Update(VisitType(awaitableInstancePlaceholder.Type)));
			BoundLValuePlaceholder awaiterInstancePlaceholder = node.AwaiterInstancePlaceholder;
			PlaceholderReplacementMap.Add(awaiterInstancePlaceholder, awaiterInstancePlaceholder.Update(VisitType(awaiterInstancePlaceholder.Type)));
			BoundNode result = base.VisitAwaitOperator(node);
			PlaceholderReplacementMap.Remove(awaitableInstancePlaceholder);
			PlaceholderReplacementMap.Remove(awaiterInstancePlaceholder);
			return result;
		}

		public override BoundNode VisitSelectStatement(BoundSelectStatement node)
		{
			BoundRValuePlaceholder exprPlaceholderOpt = node.ExprPlaceholderOpt;
			if (exprPlaceholderOpt != null)
			{
				PlaceholderReplacementMap.Add(exprPlaceholderOpt, exprPlaceholderOpt.Update(VisitType(exprPlaceholderOpt.Type)));
			}
			BoundNode result = base.VisitSelectStatement(node);
			if (exprPlaceholderOpt != null)
			{
				PlaceholderReplacementMap.Remove(exprPlaceholderOpt);
			}
			return result;
		}

		public override BoundNode VisitUserDefinedShortCircuitingOperator(BoundUserDefinedShortCircuitingOperator node)
		{
			BoundRValuePlaceholder leftOperandPlaceholder = node.LeftOperandPlaceholder;
			PlaceholderReplacementMap.Add(leftOperandPlaceholder, leftOperandPlaceholder.Update(VisitType(leftOperandPlaceholder.Type)));
			BoundNode result = base.VisitUserDefinedShortCircuitingOperator(node);
			PlaceholderReplacementMap.Remove(leftOperandPlaceholder);
			return result;
		}
	}
}
