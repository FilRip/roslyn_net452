// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class MethodSymbolExtensions
    {
        public static bool IsParams(this MethodSymbol method)
        {
            return method.ParameterCount != 0 && method.Parameters[method.ParameterCount - 1].IsParams;
        }

        internal static bool IsSynthesizedLambda(this MethodSymbol method)
        {
            return method.IsImplicitlyDeclared && method.MethodKind == MethodKind.AnonymousFunction;
        }

        /// <summary>
        /// The runtime considers a method to be a finalizer (i.e. a method that should be invoked
        /// by the garbage collector) if it (directly or indirectly) overrides System.Object.Finalize.
        /// </summary>
        /// <remarks>
        /// As an optimization, return true immediately for metadata methods with MethodKind
        /// Destructor - they are guaranteed to be finalizers.
        /// </remarks>
        /// <param name="method">Method to inspect.</param>
        /// <param name="skipFirstMethodKindCheck">This method is used to determine the method kind of
        /// a PEMethodSymbol, so we may need to avoid using MethodKind until we move on to a different
        /// MethodSymbol.</param>
        public static bool IsRuntimeFinalizer(this MethodSymbol method, bool skipFirstMethodKindCheck = false)
        {
            // Note: Flipping the metadata-virtual bit on a method can't change it from not-a-runtime-finalize
            // to runtime-finalizer (since it will also be marked newslot), so it is safe to use
            // IsMetadataVirtualIgnoringInterfaceImplementationChanges.  This also has the advantage of making
            // this method safe to call before declaration diagnostics have been computed.
            if (method is null || method.Name != WellKnownMemberNames.DestructorName ||
                method.ParameterCount != 0 || method.Arity != 0 || !method.IsMetadataVirtual(ignoreInterfaceImplementationChanges: true))
            {
                return false;
            }

            while (method is not null)
            {
                if (!skipFirstMethodKindCheck && method.MethodKind == MethodKind.Destructor)
                {
                    // For metadata symbols (and symbols that wrap them), having MethodKind
                    // Destructor guarantees that the method is a runtime finalizer.
                    // This is also true for source symbols, since we make them explicitly
                    // override System.Object.Finalize.
                    return true;
                }
                else if (method.ContainingType.SpecialType == SpecialType.System_Object)
                {
                    return true;
                }
                else if (method.IsMetadataNewSlot(ignoreInterfaceImplementationChanges: true))
                {
                    // If the method isn't a runtime override, then it can't be a finalizer.
                    return false;
                }

                // At this point, we know method originated with a DestructorDeclarationSyntax in source,
                // so it can't have the "new" modifier.
                // First is fine, since there should only be one, since there are no parameters.
                method = method.GetFirstRuntimeOverriddenMethodIgnoringNewSlot(out _);
                skipFirstMethodKindCheck = false;
            }

            return false;
        }

        /// <summary>
        /// Returns a constructed method symbol if 'method' is generic, otherwise just returns 'method'
        /// </summary>
        public static MethodSymbol ConstructIfGeneric(this MethodSymbol method, ImmutableArray<TypeWithAnnotations> typeArguments)
        {
            return method.IsGenericMethod ? method.Construct(typeArguments) : method;
        }

        /// <summary>
        /// Some kinds of methods are not considered to be hideable by certain kinds of members.
        /// Specifically, methods, properties, and types cannot hide constructors, destructors,
        /// operators, conversions, or accessors.
        /// </summary>
        public static bool CanBeHiddenByMemberKind(this MethodSymbol hiddenMethod, SymbolKind hidingMemberKind)
        {

            // Nothing can hide a destructor (see SymbolPreparer::ReportHiding)
            if (hiddenMethod.MethodKind == MethodKind.Destructor)
            {
                return false;
            }

            return hidingMemberKind switch
            {
                SymbolKind.ErrorType or SymbolKind.NamedType or SymbolKind.Method or SymbolKind.Property => CanBeHiddenByMethodPropertyOrType(hiddenMethod),
                SymbolKind.Field or SymbolKind.Event => true,
                _ => throw ExceptionUtilities.UnexpectedValue(hidingMemberKind),
            };
        }

        /// <summary>
        /// Some kinds of methods are never considered hidden by methods, properties, or types
        /// (constructors, destructors, operators, conversions, and accessors).
        /// </summary>
        private static bool CanBeHiddenByMethodPropertyOrType(MethodSymbol method)
        {
            return method.MethodKind switch
            {
                // See CSemanticChecker::FindSymHiddenByMethPropAgg.
                MethodKind.Destructor or MethodKind.Constructor or MethodKind.StaticConstructor or MethodKind.UserDefinedOperator or MethodKind.Conversion => false,
                MethodKind.EventAdd or MethodKind.EventRemove or MethodKind.PropertyGet or MethodKind.PropertySet => method.IsIndexedPropertyAccessor(),
                _ => true,
            };
        }

        /// <summary>
        /// Returns whether this method is async and returns void.
        /// </summary>
        public static bool IsAsyncReturningVoid(this MethodSymbol method)
        {
            return method.IsAsync && method.ReturnsVoid;
        }

        /// <summary>
        /// Returns whether this method is async and returns a task.
        /// </summary>
        public static bool IsAsyncReturningTask(this MethodSymbol method, CSharpCompilation compilation)
        {
            return method.IsAsync
                && method.ReturnType.IsNonGenericTaskType(compilation);
        }

        /// <summary>
        /// Returns whether this method is async and returns a generic task.
        /// </summary>
        public static bool IsAsyncReturningGenericTask(this MethodSymbol method, CSharpCompilation compilation)
        {
            return method.IsAsync
                && method.ReturnType.IsGenericTaskType(compilation);
        }

        /// <summary>
        /// Returns whether this method is async and returns an IAsyncEnumerable`1.
        /// </summary>
        public static bool IsAsyncReturningIAsyncEnumerable(this MethodSymbol method, CSharpCompilation compilation)
        {
            return method.IsAsync
                && method.ReturnType.IsIAsyncEnumerableType(compilation);
        }

        /// <summary>
        /// Returns whether this method is async and returns an IAsyncEnumerator`1.
        /// </summary>
        public static bool IsAsyncReturningIAsyncEnumerator(this MethodSymbol method, CSharpCompilation compilation)
        {
            return method.IsAsync
                && method.ReturnType.IsIAsyncEnumeratorType(compilation);
        }

        internal static CSharpSyntaxNode ExtractReturnTypeSyntax(this MethodSymbol method)
        {
            if (method is SynthesizedSimpleProgramEntryPointSymbol synthesized)
            {
                return (CSharpSyntaxNode)synthesized.ReturnTypeSyntax;
            }

            method = method.PartialDefinitionPart ?? method;
            foreach (var reference in method.DeclaringSyntaxReferences)
            {
                SyntaxNode node = reference.GetSyntax();
                if (node is MethodDeclarationSyntax methodDeclaration)
                {
                    return methodDeclaration.ReturnType;
                }
                else if (node is LocalFunctionStatementSyntax statement)
                {
                    return statement.ReturnType;
                }
            }

            return (CSharpSyntaxNode)CSharpSyntaxTree.Dummy.GetRoot();
        }
    }
}
