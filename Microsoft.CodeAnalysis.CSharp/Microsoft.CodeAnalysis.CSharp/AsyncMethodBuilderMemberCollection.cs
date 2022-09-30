using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.RuntimeMembers;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal struct AsyncMethodBuilderMemberCollection
    {
        internal readonly NamedTypeSymbol BuilderType;

        internal readonly TypeSymbol ResultType;

        internal readonly MethodSymbol CreateBuilder;

        internal readonly MethodSymbol SetException;

        internal readonly MethodSymbol SetResult;

        internal readonly MethodSymbol AwaitOnCompleted;

        internal readonly MethodSymbol AwaitUnsafeOnCompleted;

        internal readonly MethodSymbol Start;

        internal readonly MethodSymbol SetStateMachine;

        internal readonly PropertySymbol Task;

        internal readonly bool CheckGenericMethodConstraints;

        private AsyncMethodBuilderMemberCollection(NamedTypeSymbol builderType, TypeSymbol resultType, MethodSymbol createBuilder, MethodSymbol setException, MethodSymbol setResult, MethodSymbol awaitOnCompleted, MethodSymbol awaitUnsafeOnCompleted, MethodSymbol start, MethodSymbol setStateMachine, PropertySymbol task, bool checkGenericMethodConstraints)
        {
            BuilderType = builderType;
            ResultType = resultType;
            CreateBuilder = createBuilder;
            SetException = setException;
            SetResult = setResult;
            AwaitOnCompleted = awaitOnCompleted;
            AwaitUnsafeOnCompleted = awaitUnsafeOnCompleted;
            Start = start;
            SetStateMachine = setStateMachine;
            Task = task;
            CheckGenericMethodConstraints = checkGenericMethodConstraints;
        }

        internal static bool TryCreate(SyntheticBoundNodeFactory F, MethodSymbol method, TypeMap typeMap, out AsyncMethodBuilderMemberCollection collection)
        {
            if (method.IsIterator)
            {
                NamedTypeSymbol builderType = F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncIteratorMethodBuilder);
                TryGetBuilderMember<MethodSymbol>(F, WellKnownMember.System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__Create, builderType, customBuilder: false, out var symbol);
                if ((object)symbol == null)
                {
                    collection = default(AsyncMethodBuilderMemberCollection);
                    return false;
                }
                return TryCreate(F, customBuilder: false, builderType, F.SpecialType(SpecialType.System_Void), symbol, null, null, WellKnownMember.System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__Complete, WellKnownMember.System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__AwaitOnCompleted, WellKnownMember.System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__AwaitUnsafeOnCompleted, WellKnownMember.System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__MoveNext_T, null, out collection);
            }
            if (method.IsAsyncReturningVoid())
            {
                NamedTypeSymbol builderType2 = F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncVoidMethodBuilder);
                bool customBuilder = false;
                TryGetBuilderMember<MethodSymbol>(F, WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__Create, builderType2, customBuilder, out var symbol2);
                if ((object)symbol2 == null)
                {
                    collection = default(AsyncMethodBuilderMemberCollection);
                    return false;
                }
                return TryCreate(F, customBuilder, builderType2, F.SpecialType(SpecialType.System_Void), symbol2, null, WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetException, WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetResult, WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__AwaitOnCompleted, WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__AwaitUnsafeOnCompleted, WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__Start_T, WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetStateMachine, out collection);
            }
            if (method.IsAsyncReturningTask(F.Compilation))
            {
                NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)method.ReturnType;
                MethodSymbol symbol3 = null;
                PropertySymbol symbol4 = null;
                bool flag = namedTypeSymbol.IsCustomTaskType(out object builderArgument);
                NamedTypeSymbol namedTypeSymbol2;
                if (flag)
                {
                    namedTypeSymbol2 = ValidateBuilderType(F, builderArgument, namedTypeSymbol.DeclaredAccessibility, isGeneric: false);
                    if ((object)namedTypeSymbol2 != null)
                    {
                        symbol4 = GetCustomTaskProperty(F, namedTypeSymbol2, namedTypeSymbol);
                        symbol3 = GetCustomCreateMethod(F, namedTypeSymbol2);
                    }
                }
                else
                {
                    namedTypeSymbol2 = F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncTaskMethodBuilder);
                    TryGetBuilderMember<MethodSymbol>(F, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Create, namedTypeSymbol2, flag, out symbol3);
                    TryGetBuilderMember<PropertySymbol>(F, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Task, namedTypeSymbol2, flag, out symbol4);
                }
                if ((object)namedTypeSymbol2 == null || (object)symbol3 == null || (object)symbol4 == null)
                {
                    collection = default(AsyncMethodBuilderMemberCollection);
                    return false;
                }
                return TryCreate(F, flag, namedTypeSymbol2, F.SpecialType(SpecialType.System_Void), symbol3, symbol4, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetException, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetResult, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__AwaitOnCompleted, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__AwaitUnsafeOnCompleted, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Start_T, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetStateMachine, out collection);
            }
            if (method.IsAsyncReturningGenericTask(F.Compilation))
            {
                NamedTypeSymbol namedTypeSymbol3 = (NamedTypeSymbol)method.ReturnType;
                TypeSymbol typeSymbol = namedTypeSymbol3.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.Single().Type;
                if (typeSymbol.IsDynamic())
                {
                    typeSymbol = F.SpecialType(SpecialType.System_Object);
                }
                if (typeMap != null)
                {
                    typeSymbol = typeMap.SubstituteType(typeSymbol).Type;
                }
                namedTypeSymbol3 = namedTypeSymbol3.ConstructedFrom.Construct(typeSymbol);
                MethodSymbol symbol5 = null;
                PropertySymbol symbol6 = null;
                bool flag2 = namedTypeSymbol3.IsCustomTaskType(out object builderArgument2);
                NamedTypeSymbol namedTypeSymbol4;
                if (flag2)
                {
                    namedTypeSymbol4 = ValidateBuilderType(F, builderArgument2, namedTypeSymbol3.DeclaredAccessibility, isGeneric: true);
                    if ((object)namedTypeSymbol4 != null)
                    {
                        namedTypeSymbol4 = namedTypeSymbol4.ConstructedFrom.Construct(typeSymbol);
                        symbol6 = GetCustomTaskProperty(F, namedTypeSymbol4, namedTypeSymbol3);
                        symbol5 = GetCustomCreateMethod(F, namedTypeSymbol4);
                    }
                }
                else
                {
                    namedTypeSymbol4 = F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T);
                    namedTypeSymbol4 = namedTypeSymbol4.Construct(typeSymbol);
                    TryGetBuilderMember<MethodSymbol>(F, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Create, namedTypeSymbol4, flag2, out symbol5);
                    TryGetBuilderMember<PropertySymbol>(F, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Task, namedTypeSymbol4, flag2, out symbol6);
                }
                if ((object)namedTypeSymbol4 == null || (object)symbol6 == null || (object)symbol5 == null)
                {
                    collection = default(AsyncMethodBuilderMemberCollection);
                    return false;
                }
                return TryCreate(F, flag2, namedTypeSymbol4, typeSymbol, symbol5, symbol6, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetException, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetResult, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__AwaitOnCompleted, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__AwaitUnsafeOnCompleted, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Start_T, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetStateMachine, out collection);
            }
            throw ExceptionUtilities.UnexpectedValue(method);
        }

        private static NamedTypeSymbol ValidateBuilderType(SyntheticBoundNodeFactory F, object builderAttributeArgument, Accessibility desiredAccessibility, bool isGeneric)
        {
            bool num;
            if (builderAttributeArgument is NamedTypeSymbol namedTypeSymbol && !namedTypeSymbol.IsErrorType() && !namedTypeSymbol.IsVoidType() && namedTypeSymbol.DeclaredAccessibility == desiredAccessibility)
            {
                if (!isGeneric)
                {
                    num = !namedTypeSymbol.IsGenericType;
                    goto IL_005c;
                }
                if (namedTypeSymbol.IsUnboundGenericType)
                {
                    NamedTypeSymbol containingType = namedTypeSymbol.ContainingType;
                    if ((object)containingType == null || !containingType.IsGenericType)
                    {
                        num = namedTypeSymbol.Arity == 1;
                        goto IL_005c;
                    }
                }
            }
            goto IL_0060;
        IL_005c:
            if (num)
            {
                return namedTypeSymbol;
            }
            goto IL_0060;
        IL_0060:
            F.Diagnostics.Add(ErrorCode.ERR_BadAsyncReturn, F.Syntax.Location);
            return null;
        }

        private static bool TryCreate(SyntheticBoundNodeFactory F, bool customBuilder, NamedTypeSymbol builderType, TypeSymbol resultType, MethodSymbol createBuilderMethod, PropertySymbol taskProperty, WellKnownMember? setException, WellKnownMember setResult, WellKnownMember awaitOnCompleted, WellKnownMember awaitUnsafeOnCompleted, WellKnownMember start, WellKnownMember? setStateMachine, out AsyncMethodBuilderMemberCollection collection)
        {
            if (TryGetBuilderMember<MethodSymbol>(F, setException, builderType, customBuilder, out var symbol) && TryGetBuilderMember<MethodSymbol>(F, setResult, builderType, customBuilder, out var symbol2) && TryGetBuilderMember<MethodSymbol>(F, awaitOnCompleted, builderType, customBuilder, out var symbol3) && TryGetBuilderMember<MethodSymbol>(F, awaitUnsafeOnCompleted, builderType, customBuilder, out var symbol4) && TryGetBuilderMember<MethodSymbol>(F, start, builderType, customBuilder, out var symbol5) && TryGetBuilderMember<MethodSymbol>(F, setStateMachine, builderType, customBuilder, out var symbol6))
            {
                collection = new AsyncMethodBuilderMemberCollection(builderType, resultType, createBuilderMethod, symbol, symbol2, symbol3, symbol4, symbol5, symbol6, taskProperty, customBuilder);
                return true;
            }
            collection = default(AsyncMethodBuilderMemberCollection);
            return false;
        }

        private static bool TryGetBuilderMember<TSymbol>(SyntheticBoundNodeFactory F, WellKnownMember? member, NamedTypeSymbol builderType, bool customBuilder, out TSymbol symbol) where TSymbol : Symbol
        {
            if (!member.HasValue)
            {
                symbol = null;
                return true;
            }
            WellKnownMember value = member.Value;
            if (customBuilder)
            {
                MemberDescriptor descriptor = WellKnownMembers.GetDescriptor(value);
                Symbol symbol2 = CSharpCompilation.GetRuntimeMember(builderType.OriginalDefinition, in descriptor, F.Compilation.WellKnownMemberSignatureComparer, null);
                if ((object)symbol2 != null)
                {
                    symbol2 = symbol2.SymbolAsMember(builderType);
                }
                symbol = symbol2 as TSymbol;
            }
            else
            {
                symbol = F.WellKnownMember(value, isOptional: true) as TSymbol;
                if (symbol != null)
                {
                    symbol = (TSymbol)symbol.SymbolAsMember(builderType);
                }
            }
            if (symbol == null)
            {
                MemberDescriptor descriptor2 = WellKnownMembers.GetDescriptor(value);
                CSDiagnostic diag = new CSDiagnostic(new CSDiagnosticInfo(ErrorCode.ERR_MissingPredefinedMember, customBuilder ? builderType : descriptor2.DeclaringTypeMetadataName, descriptor2.Name), F.Syntax.Location);
                F.Diagnostics.Add(diag);
                return false;
            }
            return true;
        }

        private static MethodSymbol GetCustomCreateMethod(SyntheticBoundNodeFactory F, NamedTypeSymbol builderType)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = builderType.GetMembers("Create").GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind == SymbolKind.Method)
                {
                    MethodSymbol methodSymbol = (MethodSymbol)current;
                    if (methodSymbol.DeclaredAccessibility == Accessibility.Public && methodSymbol.IsStatic && methodSymbol.ParameterCount == 0 && !methodSymbol.IsGenericMethod && methodSymbol.ReturnType.Equals(builderType, TypeCompareKind.AllIgnoreOptions))
                    {
                        return methodSymbol;
                    }
                }
            }
            F.Diagnostics.Add(ErrorCode.ERR_MissingPredefinedMember, F.Syntax.Location, builderType, "Create");
            return null;
        }

        private static PropertySymbol GetCustomTaskProperty(SyntheticBoundNodeFactory F, NamedTypeSymbol builderType, NamedTypeSymbol returnType)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = builderType.GetMembers("Task").GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind != SymbolKind.Property)
                {
                    continue;
                }
                PropertySymbol propertySymbol = (PropertySymbol)current;
                if (propertySymbol.DeclaredAccessibility == Accessibility.Public && !propertySymbol.IsStatic && propertySymbol.ParameterCount == 0)
                {
                    if (!propertySymbol.Type.Equals(returnType, TypeCompareKind.AllIgnoreOptions))
                    {
                        CSDiagnostic diag = new CSDiagnostic(new CSDiagnosticInfo(ErrorCode.ERR_BadAsyncMethodBuilderTaskProperty, builderType, returnType, propertySymbol.Type), F.Syntax.Location);
                        F.Diagnostics.Add(diag);
                        return null;
                    }
                    return propertySymbol;
                }
            }
            CSDiagnostic diag2 = new CSDiagnostic(new CSDiagnosticInfo(ErrorCode.ERR_MissingPredefinedMember, builderType, "Task"), F.Syntax.Location);
            F.Diagnostics.Add(diag2);
            return null;
        }
    }
}
