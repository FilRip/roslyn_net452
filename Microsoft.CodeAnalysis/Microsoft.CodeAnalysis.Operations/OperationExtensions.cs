using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public static class OperationExtensions
    {
        internal static bool HasErrors(this IOperation operation, Compilation compilation, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }
            if (compilation == null)
            {
                throw new ArgumentNullException("compilation");
            }
            if (operation.Syntax == null)
            {
                return true;
            }
            SemanticModel semanticModel = operation.SemanticModel;
            if (semanticModel == null || semanticModel.SyntaxTree != operation.Syntax.SyntaxTree)
            {
                semanticModel = compilation.GetSemanticModel(operation.Syntax.SyntaxTree);
            }
            if (semanticModel.IsSpeculativeSemanticModel)
            {
                return false;
            }
            return semanticModel.GetDiagnostics(operation.Syntax.Span, cancellationToken).Any((Diagnostic d) => d.DefaultSeverity == DiagnosticSeverity.Error);
        }

        public static IEnumerable<IOperation> Descendants(this IOperation? operation)
        {
            return Descendants(operation, includeSelf: false);
        }

        public static IEnumerable<IOperation> DescendantsAndSelf(this IOperation? operation)
        {
            return Descendants(operation, includeSelf: true);
        }

        private static IEnumerable<IOperation> Descendants(IOperation? operation, bool includeSelf)
        {
            if (operation == null)
            {
                yield break;
            }
            if (includeSelf)
            {
                yield return operation;
            }
            ArrayBuilder<Operation.Enumerator> stack = ArrayBuilder<Operation.Enumerator>.GetInstance();
            stack.Push(((Operation)operation).ChildOperations.GetEnumerator());
            while (stack.Any())
            {
                Operation.Enumerator e = stack.Pop();
                if (e.MoveNext())
                {
                    IOperation current = e.Current;
                    stack.Push(e);
                    if (current != null)
                    {
                        yield return current;
                        stack.Push(((Operation)current).ChildOperations.GetEnumerator());
                    }
                }
            }
            stack.Free();
        }

        public static ImmutableArray<ILocalSymbol> GetDeclaredVariables(this IVariableDeclarationGroupOperation declarationGroup)
        {
            if (declarationGroup == null)
            {
                throw new ArgumentNullException("declarationGroup");
            }
            ArrayBuilder<ILocalSymbol> instance = ArrayBuilder<ILocalSymbol>.GetInstance();
            ImmutableArray<IVariableDeclarationOperation>.Enumerator enumerator = declarationGroup.Declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.GetDeclaredVariables(instance);
            }
            return instance.ToImmutableAndFree();
        }

        public static ImmutableArray<ILocalSymbol> GetDeclaredVariables(this IVariableDeclarationOperation declaration)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }
            ArrayBuilder<ILocalSymbol> instance = ArrayBuilder<ILocalSymbol>.GetInstance();
            declaration.GetDeclaredVariables(instance);
            return instance.ToImmutableAndFree();
        }

        private static void GetDeclaredVariables(this IVariableDeclarationOperation declaration, ArrayBuilder<ILocalSymbol> arrayBuilder)
        {
            ImmutableArray<IVariableDeclaratorOperation>.Enumerator enumerator = declaration.Declarators.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IVariableDeclaratorOperation current = enumerator.Current;
                arrayBuilder.Add(current.Symbol);
            }
        }

        public static IVariableInitializerOperation? GetVariableInitializer(this IVariableDeclaratorOperation declarationOperation)
        {
            if (declarationOperation == null)
            {
                throw new ArgumentNullException("declarationOperation");
            }
            IVariableInitializerOperation? initializer = declarationOperation.Initializer;
            if (initializer == null)
            {
                IVariableDeclarationOperation obj = declarationOperation.Parent as IVariableDeclarationOperation;
                if (obj == null)
                {
                    return null;
                }
                initializer = obj.Initializer;
            }
            return initializer;
        }

        public static string? GetArgumentName(this IDynamicInvocationOperation dynamicOperation, int index)
        {
            if (dynamicOperation == null)
            {
                throw new ArgumentNullException("dynamicOperation");
            }
            return ((HasDynamicArgumentsExpression)dynamicOperation).GetArgumentName(index);
        }

        public static string? GetArgumentName(this IDynamicIndexerAccessOperation dynamicOperation, int index)
        {
            if (dynamicOperation == null)
            {
                throw new ArgumentNullException("dynamicOperation");
            }
            return ((HasDynamicArgumentsExpression)dynamicOperation).GetArgumentName(index);
        }

        public static string? GetArgumentName(this IDynamicObjectCreationOperation dynamicOperation, int index)
        {
            if (dynamicOperation == null)
            {
                throw new ArgumentNullException("dynamicOperation");
            }
            return ((HasDynamicArgumentsExpression)dynamicOperation).GetArgumentName(index);
        }

        internal static string? GetArgumentName(this HasDynamicArgumentsExpression dynamicOperation, int index)
        {
            if (dynamicOperation.Arguments.IsDefaultOrEmpty)
            {
                throw new InvalidOperationException();
            }
            if (index < 0 || index >= dynamicOperation.Arguments.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            ImmutableArray<string> argumentNames = dynamicOperation.ArgumentNames;
            if (!argumentNames.IsDefaultOrEmpty)
            {
                return argumentNames[index];
            }
            return null;
        }

        public static RefKind? GetArgumentRefKind(this IDynamicInvocationOperation dynamicOperation, int index)
        {
            if (dynamicOperation == null)
            {
                throw new ArgumentNullException("dynamicOperation");
            }
            return ((HasDynamicArgumentsExpression)dynamicOperation).GetArgumentRefKind(index);
        }

        public static RefKind? GetArgumentRefKind(this IDynamicIndexerAccessOperation dynamicOperation, int index)
        {
            if (dynamicOperation == null)
            {
                throw new ArgumentNullException("dynamicOperation");
            }
            return ((HasDynamicArgumentsExpression)dynamicOperation).GetArgumentRefKind(index);
        }

        public static RefKind? GetArgumentRefKind(this IDynamicObjectCreationOperation dynamicOperation, int index)
        {
            if (dynamicOperation == null)
            {
                throw new ArgumentNullException("dynamicOperation");
            }
            return ((HasDynamicArgumentsExpression)dynamicOperation).GetArgumentRefKind(index);
        }

        internal static RefKind? GetArgumentRefKind(this HasDynamicArgumentsExpression dynamicOperation, int index)
        {
            if (dynamicOperation.Arguments.IsDefaultOrEmpty)
            {
                throw new InvalidOperationException();
            }
            if (index < 0 || index >= dynamicOperation.Arguments.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            ImmutableArray<RefKind> argumentRefKinds = dynamicOperation.ArgumentRefKinds;
            if (argumentRefKinds.IsDefault)
            {
                return null;
            }
            if (argumentRefKinds.IsEmpty)
            {
                return RefKind.None;
            }
            return argumentRefKinds[index];
        }

        internal static IOperation GetRootOperation(this IOperation operation)
        {
            while (operation.Parent != null)
            {
                operation = operation.Parent;
            }
            return operation;
        }

        public static IOperation? GetCorrespondingOperation(this IBranchOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }
            if (operation.SemanticModel == null)
            {
                throw new InvalidOperationException(CodeAnalysisResources.OperationMustNotBeControlFlowGraphPart);
            }
            if (operation.BranchKind != BranchKind.Break && operation.BranchKind != BranchKind.Continue)
            {
                return null;
            }
            if (operation.Target == null)
            {
                return null;
            }
            IOperation operation2 = operation;
            while (operation2.Parent != null)
            {
                IOperation operation3 = operation2;
                if (!(operation3 is ILoopOperation loopOperation) || (!operation.Target.Equals(loopOperation.ExitLabel) && !operation.Target.Equals(loopOperation.ContinueLabel)))
                {
                    if (operation3 is ISwitchOperation switchOperation && operation.Target.Equals(switchOperation.ExitLabel))
                    {
                        return switchOperation;
                    }
                    operation2 = operation2.Parent;
                    continue;
                }
                return loopOperation;
            }
            return null;
        }

        public static ConstantValue? GetConstantValue(this IOperation operation)
        {
            return ((Operation)operation).OperationConstantValue;
        }
    }
}
