using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class SynthesizedPropertyAccessorHelper
	{
		internal static BoundBlock GetBoundMethodBody(MethodSymbol accessor, FieldSymbol backingField, ref Binder methodBodyBinder = null)
		{
			methodBodyBinder = null;
			PropertySymbol propertySymbol = (PropertySymbol)accessor.AssociatedSymbol;
			VisualBasicSyntaxNode root = VisualBasicSyntaxTree.Dummy.GetRoot();
			if (TypeSymbolExtensions.IsVoidType(propertySymbol.Type))
			{
				return BoundNodeExtensions.MakeCompilerGenerated(new BoundBlock(root, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray<BoundStatement>.Empty, hasErrors: true));
			}
			ParameterSymbol parameterSymbol = null;
			BoundExpression receiverOpt = null;
			if (!accessor.IsShared)
			{
				parameterSymbol = accessor.MeParameter;
				receiverOpt = new BoundMeReference(root, parameterSymbol.Type);
			}
			bool flag = propertySymbol.IsWithEvents && propertySymbol.IsOverrides;
			FieldSymbol fieldSymbol = null;
			BoundFieldAccess boundFieldAccess = null;
			BoundExpression receiverOpt2 = null;
			BoundExpression boundExpression = null;
			if (flag)
			{
				receiverOpt2 = new BoundMyBaseReference(root, parameterSymbol.Type);
				MethodSymbol overriddenMethod = propertySymbol.GetMethod.OverriddenMethod;
				boundExpression = new BoundCall(root, overriddenMethod, null, receiverOpt2, ImmutableArray<BoundExpression>.Empty, null, overriddenMethod.ReturnType, suppressObjectClone: true);
			}
			else
			{
				fieldSymbol = backingField;
				boundFieldAccess = new BoundFieldAccess(root, receiverOpt, fieldSymbol, isLValue: true, fieldSymbol.Type);
			}
			GeneratedLabelSymbol generatedLabelSymbol = new GeneratedLabelSymbol("exit");
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			ImmutableArray<LocalSymbol> locals;
			BoundLocal expressionOpt2;
			if (accessor.MethodKind == MethodKind.PropertyGet)
			{
				SynthesizedLocal synthesizedLocal = new SynthesizedLocal(accessor, accessor.ReturnType, SynthesizedLocalKind.LoweringTemp);
				BoundExpression expressionOpt = ((!flag) ? boundFieldAccess.MakeRValue() : boundExpression);
				instance.Add(BoundNodeExtensions.MakeCompilerGenerated(new BoundReturnStatement(root, expressionOpt, synthesizedLocal, generatedLabelSymbol)));
				locals = ImmutableArray.Create((LocalSymbol)synthesizedLocal);
				expressionOpt2 = new BoundLocal(root, synthesizedLocal, isLValue: false, synthesizedLocal.Type);
			}
			else
			{
				ParameterSymbol parameterSymbol2 = accessor.Parameters[accessor.ParameterCount - 1];
				BoundParameter boundParameter = new BoundParameter(root, parameterSymbol2, isLValue: false, parameterSymbol2.Type);
				ArrayBuilder<(EventSymbol, PropertySymbol)> arrayBuilder = null;
				ArrayBuilder<LocalSymbol> arrayBuilder2 = null;
				ArrayBuilder<BoundLocal> arrayBuilder3 = null;
				if (propertySymbol.IsWithEvents)
				{
					ImmutableArray<Symbol>.Enumerator enumerator = accessor.ContainingType.GetMembers().GetEnumerator();
					while (enumerator.MoveNext())
					{
						Symbol current = enumerator.Current;
						if (current.Kind != SymbolKind.Method)
						{
							continue;
						}
						MethodSymbol methodSymbol = (MethodSymbol)current;
						ImmutableArray<HandledEvent> first = methodSymbol.HandledEvents;
						if (MethodSymbolExtensions.IsPartial(methodSymbol))
						{
							MethodSymbol partialImplementationPart = methodSymbol.PartialImplementationPart;
							if ((object)partialImplementationPart == null)
							{
								continue;
							}
							first = first.Concat(partialImplementationPart.HandledEvents);
						}
						if (first.IsEmpty)
						{
							continue;
						}
						ImmutableArray<HandledEvent>.Enumerator enumerator2 = first.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							HandledEvent current2 = enumerator2.Current;
							if (current2.hookupMethod == accessor)
							{
								if (arrayBuilder == null)
								{
									arrayBuilder = ArrayBuilder<(EventSymbol, PropertySymbol)>.GetInstance();
									arrayBuilder2 = ArrayBuilder<LocalSymbol>.GetInstance();
									arrayBuilder3 = ArrayBuilder<BoundLocal>.GetInstance();
								}
								arrayBuilder.Add(((EventSymbol)current2.EventSymbol, (PropertySymbol)current2.WithEventsSourceProperty));
								SynthesizedLocal synthesizedLocal2 = new SynthesizedLocal(accessor, current2.delegateCreation.Type, SynthesizedLocalKind.LoweringTemp);
								arrayBuilder2.Add(synthesizedLocal2);
								BoundLocal boundLocal = new BoundLocal(root, synthesizedLocal2, synthesizedLocal2.Type);
								arrayBuilder3.Add(boundLocal.MakeRValue());
								BoundExpressionStatement item = new BoundExpressionStatement(root, new BoundAssignmentOperator(root, boundLocal, current2.delegateCreation, suppressObjectClone: false, boundLocal.Type));
								instance.Add(item);
							}
						}
					}
				}
				BoundLocal boundLocal2 = null;
				BoundExpressionStatement item2 = null;
				if (arrayBuilder != null)
				{
					BoundExpression boundExpression2 = ((!flag) ? boundFieldAccess.MakeRValue() : boundExpression);
					SynthesizedLocal synthesizedLocal3 = new SynthesizedLocal(accessor, boundExpression2.Type, SynthesizedLocalKind.LoweringTemp);
					arrayBuilder2.Add(synthesizedLocal3);
					boundLocal2 = new BoundLocal(root, synthesizedLocal3, synthesizedLocal3.Type);
					item2 = new BoundExpressionStatement(root, new BoundAssignmentOperator(root, boundLocal2, boundExpression2, suppressObjectClone: true, synthesizedLocal3.Type));
					instance.Add(item2);
					ArrayBuilder<BoundStatement> instance2 = ArrayBuilder<BoundStatement>.GetInstance();
					int num = arrayBuilder.Count - 1;
					for (int i = 0; i <= num; i++)
					{
						EventSymbol item3 = arrayBuilder[i].Item1;
						BoundExpression receiverOpt3 = boundLocal2;
						PropertySymbol item4 = arrayBuilder[i].Item2;
						if ((object)item4 != null)
						{
							receiverOpt3 = new BoundPropertyAccess(root, item4, null, PropertyAccessKind.Get, isWriteable: false, item4.IsShared ? null : boundLocal2, ImmutableArray<BoundExpression>.Empty);
						}
						instance2.Add(new BoundRemoveHandlerStatement(root, new BoundEventAccess(root, receiverOpt3, item3, item3.Type), arrayBuilder3[i]));
					}
					BoundStatementList consequence = new BoundStatementList(root, instance2.ToImmutableAndFree());
					BoundIfStatement @this = new BoundIfStatement(root, BoundNodeExtensions.MakeCompilerGenerated(new BoundBinaryOperator(root, BinaryOperatorKind.IsNot, boundLocal2.MakeRValue(), new BoundLiteral(root, ConstantValue.Nothing, accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Object)), @checked: false, accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Boolean))), consequence, null);
					instance.Add(BoundNodeExtensions.MakeCompilerGenerated(@this));
				}
				BoundExpression expression;
				if (flag)
				{
					MethodSymbol overriddenMethod2 = accessor.OverriddenMethod;
					expression = new BoundCall(root, overriddenMethod2, null, receiverOpt2, ImmutableArray.Create((BoundExpression)boundParameter), null, overriddenMethod2.ReturnType, suppressObjectClone: true);
				}
				else
				{
					expression = new BoundAssignmentOperator(root, boundFieldAccess, boundParameter, suppressObjectClone: false, propertySymbol.Type);
				}
				instance.Add(BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(root, expression)));
				if (arrayBuilder != null)
				{
					instance.Add(item2);
					ArrayBuilder<BoundStatement> instance3 = ArrayBuilder<BoundStatement>.GetInstance();
					int num2 = arrayBuilder.Count - 1;
					for (int j = 0; j <= num2; j++)
					{
						EventSymbol item5 = arrayBuilder[j].Item1;
						BoundExpression receiverOpt4 = boundLocal2;
						PropertySymbol item6 = arrayBuilder[j].Item2;
						if ((object)item6 != null)
						{
							receiverOpt4 = new BoundPropertyAccess(root, item6, null, PropertyAccessKind.Get, isWriteable: false, item6.IsShared ? null : boundLocal2, ImmutableArray<BoundExpression>.Empty);
						}
						instance3.Add(new BoundAddHandlerStatement(root, new BoundEventAccess(root, receiverOpt4, item5, item5.Type), arrayBuilder3[j]));
					}
					BoundStatementList consequence2 = new BoundStatementList(root, instance3.ToImmutableAndFree());
					BoundIfStatement this2 = new BoundIfStatement(root, BoundNodeExtensions.MakeCompilerGenerated(new BoundBinaryOperator(root, BinaryOperatorKind.IsNot, boundLocal2.MakeRValue(), new BoundLiteral(root, ConstantValue.Nothing, accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Object)), @checked: false, accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Boolean))), consequence2, null);
					instance.Add(BoundNodeExtensions.MakeCompilerGenerated(this2));
				}
				locals = arrayBuilder2?.ToImmutableAndFree() ?? ImmutableArray<LocalSymbol>.Empty;
				expressionOpt2 = null;
				if (arrayBuilder != null)
				{
					arrayBuilder.Free();
					arrayBuilder3.Free();
				}
			}
			instance.Add(BoundNodeExtensions.MakeCompilerGenerated(new BoundLabelStatement(root, generatedLabelSymbol)));
			instance.Add(BoundNodeExtensions.MakeCompilerGenerated(new BoundReturnStatement(root, expressionOpt2, null, null)));
			return BoundNodeExtensions.MakeCompilerGenerated(new BoundBlock(root, default(SyntaxList<StatementSyntax>), locals, instance.ToImmutableAndFree()));
		}
	}
}
