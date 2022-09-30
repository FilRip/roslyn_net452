using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class InitializerRewriter
	{
		internal static BoundBlock BuildConstructorBody(TypeCompilationState compilationState, MethodSymbol constructorMethod, BoundStatement constructorInitializerOpt, Binder.ProcessedFieldOrPropertyInitializers processedInitializers, BoundBlock block)
		{
			bool isMyBaseConstructorCall = false;
			NamedTypeSymbol containingType = constructorMethod.ContainingType;
			if (HasExplicitMeConstructorCall(block, containingType, out isMyBaseConstructorCall) && !isMyBaseConstructorCall)
			{
				return block;
			}
			if (processedInitializers.InitializerStatements.IsDefault)
			{
				processedInitializers.InitializerStatements = processedInitializers.BoundInitializers.SelectAsArray(RewriteInitializerAsStatement);
			}
			ImmutableArray<BoundStatement> initializerStatements = processedInitializers.InitializerStatements;
			ImmutableArray<BoundStatement> statements = block.Statements;
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			if (constructorInitializerOpt != null)
			{
				instance.Add(constructorInitializerOpt);
			}
			else if (isMyBaseConstructorCall)
			{
				instance.Add(statements[0]);
			}
			else if (!constructorMethod.IsShared && containingType.IsValueType)
			{
				SyntaxNode syntax = block.Syntax;
				instance.Add(new BoundExpressionStatement(syntax, new BoundAssignmentOperator(syntax, new BoundValueTypeMeReference(syntax, containingType), new BoundConversion(syntax, new BoundLiteral(syntax, ConstantValue.Null, null), ConversionKind.WideningNothingLiteral, @checked: false, explicitCastInCode: false, containingType), suppressObjectClone: true, containingType)));
			}
			ImmutableArray<Symbol>.Enumerator enumerator = containingType.GetMembers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.Kind != SymbolKind.Method)
				{
					continue;
				}
				MethodSymbol methodSymbol = (MethodSymbol)current;
				ImmutableArray<HandledEvent> first = methodSymbol.HandledEvents;
				if (first.IsEmpty)
				{
					continue;
				}
				if (MethodSymbolExtensions.IsPartial(methodSymbol))
				{
					MethodSymbol partialImplementationPart = methodSymbol.PartialImplementationPart;
					if ((object)partialImplementationPart == null)
					{
						continue;
					}
					first = first.Concat(partialImplementationPart.HandledEvents);
				}
				ImmutableArray<HandledEvent>.Enumerator enumerator2 = first.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					HandledEvent current2 = enumerator2.Current;
					if (current2.hookupMethod.MethodKind == constructorMethod.MethodKind)
					{
						EventSymbol eventSymbol = (EventSymbol)current2.EventSymbol;
						MethodSymbol addMethod = eventSymbol.AddMethod;
						BoundExpression delegateCreation = current2.delegateCreation;
						SyntaxNode syntax2 = delegateCreation.Syntax;
						BoundExpression receiverOpt = null;
						if (!addMethod.IsShared)
						{
							ParameterSymbol meParameter = constructorMethod.MeParameter;
							receiverOpt = ((!TypeSymbol.Equals(addMethod.ContainingType, containingType, TypeCompareKind.ConsiderEverything)) ? ((BoundExpression)BoundNodeExtensions.MakeCompilerGenerated(new BoundMyBaseReference(syntax2, meParameter.Type))) : ((BoundExpression)BoundNodeExtensions.MakeCompilerGenerated(new BoundMeReference(syntax2, meParameter.Type))));
						}
						instance.Add(BoundNodeExtensions.MakeCompilerGenerated(new BoundAddHandlerStatement(syntax2, BoundNodeExtensions.MakeCompilerGenerated(new BoundEventAccess(syntax2, receiverOpt, eventSymbol, eventSymbol.Type)), delegateCreation)));
					}
				}
			}
			instance.AddRange(initializerStatements);
			if (!constructorMethod.IsShared && (object)compilationState.InitializeComponentOpt != null && constructorMethod.IsImplicitlyDeclared)
			{
				instance.Add(BoundNodeExtensions.MakeCompilerGenerated(BoundExpressionExtensions.ToStatement(BoundNodeExtensions.MakeCompilerGenerated(new BoundCall(constructorMethod.Syntax, compilationState.InitializeComponentOpt, null, new BoundMeReference(constructorMethod.Syntax, compilationState.InitializeComponentOpt.ContainingType), ImmutableArray<BoundExpression>.Empty, null, compilationState.InitializeComponentOpt.ReturnType)))));
			}
			if (instance.Count == 0)
			{
				instance.Free();
				return block;
			}
			int num = (isMyBaseConstructorCall ? 1 : 0);
			int num2 = statements.Length - 1;
			for (int i = num; i <= num2; i++)
			{
				instance.Add(statements[i]);
			}
			return new BoundBlock(block.Syntax, block.StatementListSyntax, block.Locals, instance.ToImmutableAndFree(), block.HasErrors);
		}

		internal static BoundBlock BuildScriptInitializerBody(SynthesizedInteractiveInitializerMethod initializerMethod, Binder.ProcessedFieldOrPropertyInitializers processedInitializers, BoundBlock block)
		{
			ImmutableArray<BoundStatement> items = (processedInitializers.InitializerStatements = RewriteInitializersAsStatements(initializerMethod, processedInitializers.BoundInitializers));
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			instance.AddRange(items);
			instance.AddRange(block.Statements);
			return new BoundBlock(block.Syntax, block.StatementListSyntax, block.Locals, instance.ToImmutableAndFree(), block.HasErrors);
		}

		private static ImmutableArray<BoundStatement> RewriteInitializersAsStatements(SynthesizedInteractiveInitializerMethod method, ImmutableArray<BoundInitializer> boundInitializers)
		{
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance(boundInitializers.Length);
			TypeSymbol resultType = method.ResultType;
			BoundExpression boundExpression = null;
			ImmutableArray<BoundInitializer>.Enumerator enumerator = boundInitializers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundInitializer current = enumerator.Current;
				if ((object)resultType != null && current == boundInitializers.Last() && current.Kind == BoundKind.GlobalStatementInitializer)
				{
					BoundStatement statement = ((BoundGlobalStatementInitializer)current).Statement;
					if (statement.Kind == BoundKind.ExpressionStatement)
					{
						BoundExpression expression = ((BoundExpressionStatement)statement).Expression;
						if (expression.Type.SpecialType != SpecialType.System_Void)
						{
							boundExpression = expression;
							continue;
						}
					}
				}
				instance.Add(RewriteInitializerAsStatement(current));
			}
			if ((object)resultType != null)
			{
				if (boundExpression == null)
				{
					boundExpression = new BoundLiteral(method.Syntax, ConstantValue.Nothing, resultType);
				}
				instance.Add(new BoundReturnStatement(boundExpression.Syntax, boundExpression, method.FunctionLocal, method.ExitLabel));
			}
			return instance.ToImmutableAndFree();
		}

		private static BoundStatement RewriteInitializerAsStatement(BoundInitializer initializer)
		{
			switch (initializer.Kind)
			{
			case BoundKind.FieldInitializer:
			case BoundKind.PropertyInitializer:
				return initializer;
			case BoundKind.GlobalStatementInitializer:
				return ((BoundGlobalStatementInitializer)initializer).Statement;
			default:
				throw ExceptionUtilities.UnexpectedValue(initializer.Kind);
			}
		}

		internal static bool HasExplicitMeConstructorCall(BoundBlock block, TypeSymbol container, out bool isMyBaseConstructorCall)
		{
			isMyBaseConstructorCall = false;
			if (block.Statements.Any())
			{
				BoundStatement boundStatement = block.Statements.First();
				if (boundStatement.Kind == BoundKind.ExpressionStatement)
				{
					BoundExpression expression = ((BoundExpressionStatement)boundStatement).Expression;
					if (expression.Kind == BoundKind.Call)
					{
						BoundCall boundCall = (BoundCall)expression;
						BoundExpression receiverOpt = boundCall.ReceiverOpt;
						if (receiverOpt != null && BoundExpressionExtensions.IsInstanceReference(receiverOpt))
						{
							MethodSymbol method = boundCall.Method;
							if (method.MethodKind == MethodKind.Constructor)
							{
								isMyBaseConstructorCall = BoundExpressionExtensions.IsMyBaseReference(receiverOpt);
								return TypeSymbol.Equals(method.ContainingType, container, TypeCompareKind.ConsiderEverything);
							}
						}
					}
				}
			}
			return false;
		}
	}
}
