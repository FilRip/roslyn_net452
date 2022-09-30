using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class VisualBasicDeclarationComputer : DeclarationComputer
	{
		public static void ComputeDeclarationsInSpan(SemanticModel model, TextSpan span, bool getSymbol, ArrayBuilder<DeclarationInfo> builder, CancellationToken cancellationToken)
		{
			ComputeDeclarationsCore(model, model.SyntaxTree.GetRoot(cancellationToken), (SyntaxNode node, int? level) => !node.Span.OverlapsWith(span) || InvalidLevel(level), getSymbol, builder, null, cancellationToken);
		}

		public static void ComputeDeclarationsInNode(SemanticModel model, SyntaxNode node, bool getSymbol, ArrayBuilder<DeclarationInfo> builder, CancellationToken cancellationToken, int? levelsToCompute = null)
		{
			ComputeDeclarationsCore(model, node, (SyntaxNode n, int? level) => InvalidLevel(level), getSymbol, builder, levelsToCompute, cancellationToken);
		}

		private static bool InvalidLevel(int? level)
		{
			if (level.HasValue)
			{
				return level.Value <= 0;
			}
			return false;
		}

		private static int? DecrementLevel(int? level)
		{
			if (!level.HasValue)
			{
				return level;
			}
			return level.Value - 1;
		}

		private static void ComputeDeclarationsCore(SemanticModel model, SyntaxNode node, Func<SyntaxNode, int?, bool> shouldSkip, bool getSymbol, ArrayBuilder<DeclarationInfo> builder, int? levelsToCompute, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (shouldSkip(node, levelsToCompute))
			{
				return;
			}
			int? levelsToCompute2 = DecrementLevel(levelsToCompute);
			switch (VisualBasicExtensions.Kind(node))
			{
			case SyntaxKind.NamespaceBlock:
			{
				NamespaceBlockSyntax namespaceBlockSyntax = (NamespaceBlockSyntax)node;
				SyntaxList<StatementSyntax>.Enumerator enumerator7 = namespaceBlockSyntax.Members.GetEnumerator();
				while (enumerator7.MoveNext())
				{
					StatementSyntax current7 = enumerator7.Current;
					ComputeDeclarationsCore(model, current7, shouldSkip, getSymbol, builder, levelsToCompute2, cancellationToken);
				}
				DeclarationInfo declarationInfo = DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, cancellationToken);
				builder.Add(declarationInfo);
				NameSyntax nameSyntax = namespaceBlockSyntax.NamespaceStatement.Name;
				ISymbol symbol = declarationInfo.DeclaredSymbol;
				while (nameSyntax.Kind() == SyntaxKind.QualifiedName)
				{
					nameSyntax = ((QualifiedNameSyntax)nameSyntax).Left;
					INamespaceSymbol namespaceSymbol = (getSymbol ? symbol?.ContainingNamespace : null);
					builder.Add(new DeclarationInfo(nameSyntax, ImmutableArray<SyntaxNode>.Empty, namespaceSymbol));
					symbol = namespaceSymbol;
				}
				return;
			}
			case SyntaxKind.EnumBlock:
			{
				EnumBlockSyntax enumBlockSyntax = (EnumBlockSyntax)node;
				SyntaxList<StatementSyntax>.Enumerator enumerator4 = enumBlockSyntax.Members.GetEnumerator();
				while (enumerator4.MoveNext())
				{
					StatementSyntax current4 = enumerator4.Current;
					ComputeDeclarationsCore(model, current4, shouldSkip, getSymbol, builder, levelsToCompute2, cancellationToken);
				}
				IEnumerable<SyntaxNode> attributes5 = GetAttributes(enumBlockSyntax.EnumStatement.AttributeLists);
				builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, attributes5, cancellationToken));
				return;
			}
			case SyntaxKind.EnumStatement:
			{
				IEnumerable<SyntaxNode> attributes = GetAttributes(((EnumStatementSyntax)node).AttributeLists);
				builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, attributes, cancellationToken));
				return;
			}
			case SyntaxKind.EnumMemberDeclaration:
			{
				EnumMemberDeclarationSyntax obj = (EnumMemberDeclarationSyntax)node;
				IEnumerable<SyntaxNode> executableCodeBlocks = Enumerable.Concat(second: GetAttributes(obj.AttributeLists), first: SpecializedCollections.SingletonEnumerable((SyntaxNode)obj.Initializer));
				builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, executableCodeBlocks, cancellationToken));
				return;
			}
			case SyntaxKind.EventBlock:
			{
				EventBlockSyntax eventBlockSyntax = (EventBlockSyntax)node;
				SyntaxList<AccessorBlockSyntax>.Enumerator enumerator5 = eventBlockSyntax.Accessors.GetEnumerator();
				while (enumerator5.MoveNext())
				{
					AccessorBlockSyntax current5 = enumerator5.Current;
					ComputeDeclarationsCore(model, current5, shouldSkip, getSymbol, builder, levelsToCompute2, cancellationToken);
				}
				IEnumerable<SyntaxNode> methodBaseCodeBlocks = GetMethodBaseCodeBlocks(eventBlockSyntax.EventStatement);
				builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, methodBaseCodeBlocks, cancellationToken));
				return;
			}
			case SyntaxKind.FieldDeclaration:
			{
				FieldDeclarationSyntax obj2 = (FieldDeclarationSyntax)node;
				IEnumerable<SyntaxNode> attributes3 = GetAttributes(obj2.AttributeLists);
				SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator = obj2.Declarators.GetEnumerator();
				while (enumerator.MoveNext())
				{
					VariableDeclaratorSyntax current = enumerator.Current;
					IEnumerable<SyntaxNode> executableCodeBlocks2 = SpecializedCollections.SingletonEnumerable(GetInitializerNode(current)).Concat(attributes3);
					SeparatedSyntaxList<ModifiedIdentifierSyntax>.Enumerator enumerator2 = current.Names.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						ModifiedIdentifierSyntax current2 = enumerator2.Current;
						builder.Add(DeclarationComputer.GetDeclarationInfo(model, current2, getSymbol, executableCodeBlocks2, cancellationToken));
					}
				}
				return;
			}
			case SyntaxKind.PropertyBlock:
			{
				PropertyBlockSyntax propertyBlockSyntax = (PropertyBlockSyntax)node;
				SyntaxList<AccessorBlockSyntax>.Enumerator enumerator6 = propertyBlockSyntax.Accessors.GetEnumerator();
				while (enumerator6.MoveNext())
				{
					AccessorBlockSyntax current6 = enumerator6.Current;
					ComputeDeclarationsCore(model, current6, shouldSkip, getSymbol, builder, levelsToCompute2, cancellationToken);
				}
				IEnumerable<SyntaxNode> propertyStatementCodeBlocks = GetPropertyStatementCodeBlocks(propertyBlockSyntax.PropertyStatement);
				builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, propertyStatementCodeBlocks, cancellationToken));
				return;
			}
			case SyntaxKind.PropertyStatement:
			{
				IEnumerable<SyntaxNode> propertyStatementCodeBlocks2 = GetPropertyStatementCodeBlocks((PropertyStatementSyntax)node);
				builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, propertyStatementCodeBlocks2, cancellationToken));
				return;
			}
			case SyntaxKind.CompilationUnit:
			{
				CompilationUnitSyntax compilationUnitSyntax = (CompilationUnitSyntax)node;
				SyntaxList<StatementSyntax>.Enumerator enumerator3 = compilationUnitSyntax.Members.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					StatementSyntax current3 = enumerator3.Current;
					ComputeDeclarationsCore(model, current3, shouldSkip, getSymbol, builder, levelsToCompute2, cancellationToken);
				}
				if (!compilationUnitSyntax.Attributes.IsEmpty())
				{
					IEnumerable<SyntaxNode> attributes4 = GetAttributes(compilationUnitSyntax.Attributes);
					builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, attributes4, cancellationToken));
				}
				return;
			}
			}
			if (node is TypeBlockSyntax typeBlockSyntax)
			{
				SyntaxList<StatementSyntax>.Enumerator enumerator8 = typeBlockSyntax.Members.GetEnumerator();
				while (enumerator8.MoveNext())
				{
					StatementSyntax current8 = enumerator8.Current;
					ComputeDeclarationsCore(model, current8, shouldSkip, getSymbol, builder, levelsToCompute2, cancellationToken);
				}
				IEnumerable<SyntaxNode> attributes6 = GetAttributes(typeBlockSyntax.BlockStatement.AttributeLists);
				builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, attributes6, cancellationToken));
			}
			else if (node is TypeStatementSyntax typeStatementSyntax)
			{
				IEnumerable<SyntaxNode> attributes7 = GetAttributes(typeStatementSyntax.AttributeLists);
				builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, attributes7, cancellationToken));
			}
			else if (node is MethodBlockBaseSyntax methodBlockBaseSyntax)
			{
				IEnumerable<SyntaxNode> executableCodeBlocks3 = SpecializedCollections.SingletonEnumerable((SyntaxNode)methodBlockBaseSyntax).Concat(GetMethodBaseCodeBlocks(methodBlockBaseSyntax.BlockStatement));
				builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, executableCodeBlocks3, cancellationToken));
			}
			else if (node is MethodBaseSyntax methodBase)
			{
				IEnumerable<SyntaxNode> methodBaseCodeBlocks2 = GetMethodBaseCodeBlocks(methodBase);
				builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, methodBaseCodeBlocks2, cancellationToken));
			}
		}

		private static IEnumerable<SyntaxNode> GetAttributes(SyntaxList<AttributesStatementSyntax> attributeStatements)
		{
			IEnumerable<SyntaxNode> enumerable = SpecializedCollections.EmptyEnumerable<SyntaxNode>();
			SyntaxList<AttributesStatementSyntax>.Enumerator enumerator = attributeStatements.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AttributesStatementSyntax current = enumerator.Current;
				enumerable = enumerable.Concat(GetAttributes(current.AttributeLists));
			}
			return enumerable;
		}

		private static IEnumerable<SyntaxNode> GetPropertyStatementCodeBlocks(PropertyStatementSyntax propertyStatement)
		{
			SyntaxNode syntaxNode = propertyStatement.Initializer;
			if (syntaxNode == null)
			{
				syntaxNode = GetAsNewClauseInitializer(propertyStatement.AsClause);
			}
			IEnumerable<SyntaxNode> methodBaseCodeBlocks = GetMethodBaseCodeBlocks(propertyStatement);
			if (syntaxNode == null)
			{
				return methodBaseCodeBlocks;
			}
			return SpecializedCollections.SingletonEnumerable(syntaxNode).Concat(methodBaseCodeBlocks);
		}

		private static IEnumerable<SyntaxNode> GetMethodBaseCodeBlocks(MethodBaseSyntax methodBase)
		{
			IEnumerable<SyntaxNode> parameterListInitializersAndAttributes = GetParameterListInitializersAndAttributes(methodBase.ParameterList);
			IEnumerable<SyntaxNode> second = GetAttributes(methodBase.AttributeLists).Concat(GetReturnTypeAttributes(GetAsClause(methodBase)));
			return parameterListInitializersAndAttributes.Concat(second);
		}

		private static AsClauseSyntax GetAsClause(MethodBaseSyntax methodBase)
		{
			switch (methodBase.Kind())
			{
			case SyntaxKind.SubStatement:
			case SyntaxKind.FunctionStatement:
				return ((MethodStatementSyntax)methodBase).AsClause;
			case SyntaxKind.SubLambdaHeader:
			case SyntaxKind.FunctionLambdaHeader:
				return ((LambdaHeaderSyntax)methodBase).AsClause;
			case SyntaxKind.DeclareSubStatement:
			case SyntaxKind.DeclareFunctionStatement:
				return ((DeclareStatementSyntax)methodBase).AsClause;
			case SyntaxKind.DelegateSubStatement:
			case SyntaxKind.DelegateFunctionStatement:
				return ((DelegateStatementSyntax)methodBase).AsClause;
			case SyntaxKind.EventStatement:
				return ((EventStatementSyntax)methodBase).AsClause;
			case SyntaxKind.OperatorStatement:
				return ((OperatorStatementSyntax)methodBase).AsClause;
			case SyntaxKind.PropertyStatement:
				return ((PropertyStatementSyntax)methodBase).AsClause;
			case SyntaxKind.SubNewStatement:
			case SyntaxKind.GetAccessorStatement:
			case SyntaxKind.SetAccessorStatement:
			case SyntaxKind.AddHandlerAccessorStatement:
			case SyntaxKind.RemoveHandlerAccessorStatement:
			case SyntaxKind.RaiseEventAccessorStatement:
				return null;
			default:
				throw ExceptionUtilities.UnexpectedValue(methodBase.Kind());
			}
		}

		private static IEnumerable<SyntaxNode> GetReturnTypeAttributes(AsClauseSyntax asClause)
		{
			if (asClause == null || SyntaxExtensions.Attributes(asClause).IsEmpty())
			{
				return SpecializedCollections.EmptyEnumerable<SyntaxNode>();
			}
			return GetAttributes(SyntaxExtensions.Attributes(asClause));
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_11_GetAttributes))]
		private static IEnumerable<SyntaxNode> GetAttributes(SyntaxList<AttributeListSyntax> attributeLists)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_11_GetAttributes(-2)
			{
				_0024P_attributeLists = attributeLists
			};
		}

		private static IEnumerable<SyntaxNode> GetParameterListInitializersAndAttributes(ParameterListSyntax parameterList)
		{
			if (parameterList == null)
			{
				return SpecializedCollections.EmptyEnumerable<SyntaxNode>();
			}
			return parameterList.Parameters.SelectMany((ParameterSyntax p) => GetParameterInitializersAndAttributes(p));
		}

		private static IEnumerable<SyntaxNode> GetParameterInitializersAndAttributes(ParameterSyntax parameter)
		{
			return SpecializedCollections.SingletonEnumerable((SyntaxNode)parameter.Default).Concat(GetAttributes(parameter.AttributeLists));
		}

		private static SyntaxNode GetInitializerNode(VariableDeclaratorSyntax variableDeclarator)
		{
			SyntaxNode syntaxNode = variableDeclarator.Initializer;
			if (syntaxNode == null)
			{
				syntaxNode = GetAsNewClauseInitializer(variableDeclarator.AsClause);
			}
			return syntaxNode;
		}

		private static SyntaxNode GetAsNewClauseInitializer(AsClauseSyntax asClause)
		{
			if (!Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(asClause, SyntaxKind.AsNewClause))
			{
				return null;
			}
			return asClause;
		}
	}
}
