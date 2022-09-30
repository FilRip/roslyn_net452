using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class MethodSymbolExtensions
    {
        public static bool IsParams(this MethodSymbol method)
        {
            if (method.ParameterCount != 0)
            {
                return method.Parameters[method.ParameterCount - 1].IsParams;
            }
            return false;
        }

        internal static bool IsSynthesizedLambda(this MethodSymbol method)
        {
            if (method.IsImplicitlyDeclared)
            {
                return method.MethodKind == MethodKind.AnonymousFunction;
            }
            return false;
        }

        public static bool IsRuntimeFinalizer(this MethodSymbol method, bool skipFirstMethodKindCheck = false)
        {
            if ((object)method == null || method.Name != "Finalize" || method.ParameterCount != 0 || method.Arity != 0 || !method.IsMetadataVirtual(ignoreInterfaceImplementationChanges: true))
            {
                return false;
            }
            while ((object)method != null)
            {
                if (!skipFirstMethodKindCheck && method.MethodKind == MethodKind.Destructor)
                {
                    return true;
                }
                if (method.ContainingType.SpecialType == SpecialType.System_Object)
                {
                    return true;
                }
                if (method.IsMetadataNewSlot(ignoreInterfaceImplementationChanges: true))
                {
                    return false;
                }
                method = method.GetFirstRuntimeOverriddenMethodIgnoringNewSlot(out var _);
                skipFirstMethodKindCheck = false;
            }
            return false;
        }

        public static MethodSymbol ConstructIfGeneric(this MethodSymbol method, ImmutableArray<TypeWithAnnotations> typeArguments)
        {
            if (!method.IsGenericMethod)
            {
                return method;
            }
            return method.Construct(typeArguments);
        }

        public static bool CanBeHiddenByMemberKind(this MethodSymbol hiddenMethod, SymbolKind hidingMemberKind)
        {
            if (hiddenMethod.MethodKind == MethodKind.Destructor)
            {
                return false;
            }
            switch (hidingMemberKind)
            {
                case SymbolKind.ErrorType:
                case SymbolKind.Method:
                case SymbolKind.NamedType:
                case SymbolKind.Property:
                    return CanBeHiddenByMethodPropertyOrType(hiddenMethod);
                case SymbolKind.Event:
                case SymbolKind.Field:
                    return true;
                default:
                    throw ExceptionUtilities.UnexpectedValue(hidingMemberKind);
            }
        }

        private static bool CanBeHiddenByMethodPropertyOrType(MethodSymbol method)
        {
            switch (method.MethodKind)
            {
                case MethodKind.Constructor:
                case MethodKind.Conversion:
                case MethodKind.Destructor:
                case MethodKind.UserDefinedOperator:
                case MethodKind.StaticConstructor:
                    return false;
                case MethodKind.EventAdd:
                case MethodKind.EventRemove:
                case MethodKind.PropertyGet:
                case MethodKind.PropertySet:
                    return method.IsIndexedPropertyAccessor();
                default:
                    return true;
            }
        }

        public static bool IsAsyncReturningVoid(this MethodSymbol method)
        {
            if (method.IsAsync)
            {
                return method.ReturnsVoid;
            }
            return false;
        }

        public static bool IsAsyncReturningTask(this MethodSymbol method, CSharpCompilation compilation)
        {
            if (method.IsAsync)
            {
                return method.ReturnType.IsNonGenericTaskType(compilation);
            }
            return false;
        }

        public static bool IsAsyncReturningGenericTask(this MethodSymbol method, CSharpCompilation compilation)
        {
            if (method.IsAsync)
            {
                return method.ReturnType.IsGenericTaskType(compilation);
            }
            return false;
        }

        public static bool IsAsyncReturningIAsyncEnumerable(this MethodSymbol method, CSharpCompilation compilation)
        {
            if (method.IsAsync)
            {
                return method.ReturnType.IsIAsyncEnumerableType(compilation);
            }
            return false;
        }

        public static bool IsAsyncReturningIAsyncEnumerator(this MethodSymbol method, CSharpCompilation compilation)
        {
            if (method.IsAsync)
            {
                return method.ReturnType.IsIAsyncEnumeratorType(compilation);
            }
            return false;
        }

        internal static CSharpSyntaxNode ExtractReturnTypeSyntax(this MethodSymbol method)
        {
            if (method is SynthesizedSimpleProgramEntryPointSymbol synthesizedSimpleProgramEntryPointSymbol)
            {
                return (CSharpSyntaxNode)synthesizedSimpleProgramEntryPointSymbol.ReturnTypeSyntax;
            }
            method = method.PartialDefinitionPart ?? method;
            ImmutableArray<SyntaxReference>.Enumerator enumerator = method.DeclaringSyntaxReferences.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxNode syntax = enumerator.Current.GetSyntax();
                if (syntax is MethodDeclarationSyntax methodDeclarationSyntax)
                {
                    return methodDeclarationSyntax.ReturnType;
                }
                if (syntax is LocalFunctionStatementSyntax localFunctionStatementSyntax)
                {
                    return localFunctionStatementSyntax.ReturnType;
                }
            }
            return (CSharpSyntaxNode)CSharpSyntaxTree.Dummy.GetRoot();
        }
    }
}
