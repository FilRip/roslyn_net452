// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    // Shared code for rewriting Object and Collection initializer expressions

    internal sealed partial class LocalRewriter
    {
        private static BoundObjectInitializerExpressionBase UpdateInitializers(BoundObjectInitializerExpressionBase initializerExpression, ImmutableArray<BoundExpression> newInitializers)
        {
            return initializerExpression switch
            {
                BoundObjectInitializerExpression objectInitializer => objectInitializer.Update(objectInitializer.Placeholder, newInitializers, initializerExpression.Type),
                BoundCollectionInitializerExpression collectionInitializer => collectionInitializer.Update(collectionInitializer.Placeholder, newInitializers, initializerExpression.Type),
                _ => throw ExceptionUtilities.UnexpectedValue(initializerExpression.Kind),
            };
        }

        private void AddObjectOrCollectionInitializers(
            ref ArrayBuilder<BoundExpression>? dynamicSiteInitializers,
            ref ArrayBuilder<LocalSymbol>? temps,
            ArrayBuilder<BoundExpression> result,
            BoundExpression rewrittenReceiver,
            BoundExpression initializerExpression)
        {

            switch (initializerExpression)
            {
                case BoundObjectInitializerExpression objectInitializer:
                    {
                        var placeholder = objectInitializer.Placeholder;
                        AddPlaceholderReplacement(placeholder, rewrittenReceiver);
                        AddObjectInitializers(ref dynamicSiteInitializers, ref temps, result, rewrittenReceiver, objectInitializer.Initializers);
                        RemovePlaceholderReplacement(placeholder);
                    }
                    return;

                case BoundCollectionInitializerExpression collectionInitializer:
                    {
                        var placeholder = collectionInitializer.Placeholder;
                        AddPlaceholderReplacement(placeholder, rewrittenReceiver);
                        AddCollectionInitializers(/*ref dynamicSiteInitializers, */result, rewrittenReceiver, collectionInitializer.Initializers);
                        RemovePlaceholderReplacement(placeholder);
                    }
                    return;

                default:
                    throw ExceptionUtilities.UnexpectedValue(initializerExpression.Kind);
            }
        }

        private ImmutableArray<BoundExpression> MakeObjectOrCollectionInitializersForExpressionTree(BoundExpression initializerExpression)
        {

            switch (initializerExpression.Kind)
            {
                case BoundKind.ObjectInitializerExpression:
                    return VisitList(((BoundObjectInitializerExpression)initializerExpression).Initializers);

                case BoundKind.CollectionInitializerExpression:
                    var result = ArrayBuilder<BoundExpression>.GetInstance();
                    //ArrayBuilder<BoundExpression>? dynamicSiteInitializers = null;
                    AddCollectionInitializers(/*ref dynamicSiteInitializers, */result, null, ((BoundCollectionInitializerExpression)initializerExpression).Initializers);

                    // dynamic sites not allowed in ET:

                    return result.ToImmutableAndFree();

                default:
                    throw ExceptionUtilities.UnexpectedValue(initializerExpression.Kind);
            }
        }

        // Rewrite collection initializer add method calls:
        // 2) new List<int> { 1 };
        //                    ~
        private void AddCollectionInitializers(/*ref ArrayBuilder<BoundExpression>? dynamicSiteInitializers, */ArrayBuilder<BoundExpression> result, BoundExpression? rewrittenReceiver, ImmutableArray<BoundExpression> initializers)
        {

            foreach (var initializer in initializers)
            {
                // In general bound initializers may contain bad expressions or element initializers.
                // We don't lower them if they contain errors, so it's safe to assume an element initializer.

                BoundExpression? rewrittenInitializer;
                if (initializer.Kind == BoundKind.CollectionElementInitializer)
                {
                    rewrittenInitializer = MakeCollectionInitializer(rewrittenReceiver, (BoundCollectionElementInitializer)initializer);
                }
                else
                {

                    rewrittenInitializer = MakeDynamicCollectionInitializer(rewrittenReceiver!, (BoundDynamicCollectionElementInitializer)initializer);
                }

                // the call to Add may be omitted
                if (rewrittenInitializer != null)
                {
                    result.Add(rewrittenInitializer);
                }
            }
        }

        private BoundExpression MakeDynamicCollectionInitializer(BoundExpression rewrittenReceiver, BoundDynamicCollectionElementInitializer initializer)
        {
            var rewrittenArguments = VisitList(initializer.Arguments);

            // If we are calling a method on a NoPIA type, we need to embed all methods/properties
            // with the matching name of this dynamic invocation.
            EmbedIfNeedTo(rewrittenReceiver, initializer.ApplicableMethods, initializer.Syntax);

            return _dynamicFactory.MakeDynamicMemberInvocation(
                WellKnownMemberNames.CollectionInitializerAddMethodName,
                rewrittenReceiver,
                ImmutableArray<TypeWithAnnotations>.Empty,
                rewrittenArguments,
                default,
                default,
                hasImplicitReceiver: false,
                resultDiscarded: true).ToExpression();
        }

        // Rewrite collection initializer element Add method call:
        //  new List<int> { 1, 2, 3 };  OR  new List<int> { { 1, 2 }, 3 };
        //                  ~                               ~~~~~~~~
        private BoundExpression? MakeCollectionInitializer(BoundExpression? rewrittenReceiver, BoundCollectionElementInitializer initializer)
        {
            MethodSymbol addMethod = initializer.AddMethod;

            var syntax = initializer.Syntax;

            if (_allowOmissionOfConditionalCalls)
            {
                // NOTE: Calls cannot be omitted within an expression tree (CS0765); this should already
                // have been checked.
                if (addMethod.CallsAreOmitted(initializer.SyntaxTree))
                {
                    return null;
                }
            }

            var rewrittenArguments = VisitList(initializer.Arguments);
            var rewrittenType = VisitType(initializer.Type);

            // We have already lowered each argument, but we may need some additional rewriting for the arguments,
            // such as generating a params array, re-ordering arguments based on argsToParamsOpt map, inserting arguments for optional parameters, etc.
            var argumentRefKindsOpt = default(ImmutableArray<RefKind>);
            if (addMethod.Parameters[0].RefKind == RefKind.Ref)
            {
                // If the Add method is an extension which takes a `ref this` as the first parameter, implicitly add a `ref` to the argument
                // Initializer element syntax cannot have `ref`, `in`, or `out` keywords.
                // Arguments to `in` parameters will be converted to have RefKind.In later on.
                var builder = ArrayBuilder<RefKind>.GetInstance(addMethod.Parameters.Length, RefKind.None);
                builder[0] = RefKind.Ref;
                argumentRefKindsOpt = builder.ToImmutableAndFree();
            }

            rewrittenArguments = MakeArguments(syntax, rewrittenArguments, addMethod, initializer.Expanded, initializer.ArgsToParamsOpt, ref argumentRefKindsOpt, out ImmutableArray<LocalSymbol> temps/*, enableCallerInfo: ThreeState.True*/);

            if (initializer.InvokedAsExtensionMethod)
            {
                rewrittenReceiver = null;
            }

            if (_inExpressionLambda)
            {
                return initializer.Update(addMethod, rewrittenArguments, rewrittenReceiver, expanded: false, argsToParamsOpt: default, defaultArguments: default, initializer.InvokedAsExtensionMethod, initializer.ResultKind, rewrittenType);
            }

            return MakeCall(null, syntax, rewrittenReceiver, addMethod, rewrittenArguments, argumentRefKindsOpt, initializer.InvokedAsExtensionMethod, initializer.ResultKind, addMethod.ReturnType, temps);
        }

        // Rewrite object initializer member assignments and add them to the result.
        private void AddObjectInitializers(
            ref ArrayBuilder<BoundExpression>? dynamicSiteInitializers,
            ref ArrayBuilder<LocalSymbol>? temps,
            ArrayBuilder<BoundExpression> result,
            BoundExpression rewrittenReceiver,
            ImmutableArray<BoundExpression> initializers)
        {

            foreach (var initializer in initializers)
            {
                // In general bound initializers may contain bad expressions or assignments.
                // We don't lower them if they contain errors, so it's safe to assume an assignment.
                AddObjectInitializer(ref dynamicSiteInitializers, ref temps, result, rewrittenReceiver, (BoundAssignmentOperator)initializer);
            }
        }

        // Rewrite object initializer member assignment and add it to the result.
        //  new SomeType { Member = 0 };
        //                 ~~~~~~~~~~
        private void AddObjectInitializer(
            ref ArrayBuilder<BoundExpression>? dynamicSiteInitializers,
            ref ArrayBuilder<LocalSymbol>? temps,
            ArrayBuilder<BoundExpression> result,
            BoundExpression rewrittenReceiver,
            BoundAssignmentOperator assignment)
        {

            // Update the receiver for the field/property access as we might have introduced a temp for the initializer rewrite.

            BoundExpression? rewrittenLeft = null;

            // Do not lower pointer access yet, we'll do it later.
            if (assignment.Left.Kind != BoundKind.PointerElementAccess)
            {
                rewrittenLeft = VisitExpression(assignment.Left);
            }

            BoundKind rhsKind = assignment.Right.Kind;
            bool isRhsNestedInitializer = rhsKind == BoundKind.ObjectInitializerExpression || rhsKind == BoundKind.CollectionInitializerExpression;

            BoundExpression rewrittenAccess;
            switch ((rewrittenLeft ?? assignment.Left).Kind)
            {
                case BoundKind.ObjectInitializerMember:
                    {
                        var memberInit = (BoundObjectInitializerMember?)rewrittenLeft;

#nullable restore
                        if (!memberInit.Arguments.IsDefaultOrEmpty)
                        {
                            var args = EvaluateSideEffectingArgumentsToTemps(
                                memberInit.Arguments,
                                memberInit.MemberSymbol?.GetParameterRefKinds() ?? default,
                                result,
                                ref temps);

                            memberInit = memberInit.Update(
                                memberInit.MemberSymbol,
                                args,
                                memberInit.ArgumentNamesOpt,
                                memberInit.ArgumentRefKindsOpt,
                                memberInit.Expanded,
                                memberInit.ArgsToParamsOpt,
                                memberInit.DefaultArguments,
                                memberInit.ResultKind,
                                memberInit.ReceiverType,
                                memberInit.Type);
                        }

                        if (memberInit.MemberSymbol == null && memberInit.Type.IsDynamic())
                        {
                            dynamicSiteInitializers ??= ArrayBuilder<BoundExpression>.GetInstance();

                            if (!isRhsNestedInitializer)
                            {
                                var rewrittenRight = VisitExpression(assignment.Right);
                                var setMember = _dynamicFactory.MakeDynamicSetIndex(
                                    rewrittenReceiver,
                                    memberInit.Arguments,
                                    memberInit.ArgumentNamesOpt,
                                    memberInit.ArgumentRefKindsOpt,
                                    rewrittenRight);

                                dynamicSiteInitializers.Add(setMember.SiteInitialization);
                                result.Add(setMember.SiteInvocation);
                                return;
                            }

                            var getMember = _dynamicFactory.MakeDynamicGetIndex(
                                rewrittenReceiver,
                                memberInit.Arguments,
                                memberInit.ArgumentNamesOpt,
                                memberInit.ArgumentRefKindsOpt);

                            dynamicSiteInitializers.Add(getMember.SiteInitialization);
                            rewrittenAccess = getMember.SiteInvocation;
                        }
                        else
                        {
                            rewrittenAccess = MakeObjectInitializerMemberAccess(rewrittenReceiver, memberInit, isRhsNestedInitializer);
                            if (!isRhsNestedInitializer)
                            {
                                // Rewrite simple assignment to field/property.
                                var rewrittenRight = VisitExpression(assignment.Right);
                                result.Add(MakeStaticAssignmentOperator(assignment.Syntax, rewrittenAccess, rewrittenRight, false, assignment.Type, used: false));
                                return;
                            }
                        }
                        break;
                    }

                case BoundKind.DynamicObjectInitializerMember:
                    {
                        dynamicSiteInitializers ??= ArrayBuilder<BoundExpression>.GetInstance();

                        var initializerMember = (BoundDynamicObjectInitializerMember)rewrittenLeft;

                        if (!isRhsNestedInitializer)
                        {
                            var rewrittenRight = VisitExpression(assignment.Right);
                            var setMember = _dynamicFactory.MakeDynamicSetMember(rewrittenReceiver, initializerMember.MemberName, rewrittenRight);
                            dynamicSiteInitializers.Add(setMember.SiteInitialization);
                            result.Add(setMember.SiteInvocation);
                            return;
                        }

                        var getMember = _dynamicFactory.MakeDynamicGetMember(rewrittenReceiver, initializerMember.MemberName, resultIndexed: false);
                        dynamicSiteInitializers.Add(getMember.SiteInitialization);
                        rewrittenAccess = getMember.SiteInvocation;
                        break;
                    }

                case BoundKind.ArrayAccess:
                    {
                        var arrayAccess = (BoundArrayAccess)rewrittenLeft;
                        var indices = EvaluateSideEffectingArgumentsToTemps(
                            arrayAccess.Indices,
                            paramRefKindsOpt: default,
                            result,
                            ref temps);
                        rewrittenAccess = arrayAccess.Update(rewrittenReceiver, indices, arrayAccess.Type);

                        if (!isRhsNestedInitializer)
                        {
                            // Rewrite simple assignment to field/property.
                            var rewrittenRight = VisitExpression(assignment.Right);
                            result.Add(MakeStaticAssignmentOperator(assignment.Syntax, rewrittenAccess, rewrittenRight, false, assignment.Type, used: false));
                            return;
                        }

                        break;
                    }

                case BoundKind.PointerElementAccess:
                    {
                        // Remember we haven't lowered this node yet.
                        var pointerAccess = (BoundPointerElementAccess)assignment.Left;
                        var rewrittenIndex = VisitExpression(pointerAccess.Index);

                        if (CanChangeValueBetweenReads(rewrittenIndex))
                        {
                            var temp = _factory.StoreToTemp(rewrittenIndex, out BoundAssignmentOperator store);
                            rewrittenIndex = temp;

                            temps ??= ArrayBuilder<LocalSymbol>.GetInstance();
                            temps.Add(temp.LocalSymbol);
                            result.Add(store);
                        }

                        rewrittenAccess = RewritePointerElementAccess(pointerAccess, rewrittenReceiver, rewrittenIndex);

                        if (!isRhsNestedInitializer)
                        {
                            // Rewrite as simple assignment.
                            var rewrittenRight = VisitExpression(assignment.Right);
                            result.Add(MakeStaticAssignmentOperator(assignment.Syntax, rewrittenAccess, rewrittenRight, false, assignment.Type, used: false));
                            return;
                        }

                        break;
                    }

                default:
                    throw ExceptionUtilities.UnexpectedValue((rewrittenLeft ?? assignment.Left).Kind);
            }

            AddObjectOrCollectionInitializers(ref dynamicSiteInitializers, ref temps, result, rewrittenAccess, assignment.Right);
        }

#nullable enable

        private ImmutableArray<BoundExpression> EvaluateSideEffectingArgumentsToTemps(
                                                 ImmutableArray<BoundExpression> args,
                                                 ImmutableArray<RefKind> paramRefKindsOpt,
                                                 ArrayBuilder<BoundExpression> sideeffects,
                                                 ref ArrayBuilder<LocalSymbol>? temps)
        {
            ArrayBuilder<BoundExpression>? newArgs = null;

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (CanChangeValueBetweenReads(arg))
                {
                    if (newArgs == null)
                    {
                        newArgs = ArrayBuilder<BoundExpression>.GetInstance(args.Length);
                        newArgs.AddRange(args, i);
                    }

                    RefKind refKind = paramRefKindsOpt.RefKinds(i);

                    var temp = _factory.StoreToTemp(arg, out BoundAssignmentOperator store, refKind);
                    newArgs.Add(temp);

                    temps ??= ArrayBuilder<LocalSymbol>.GetInstance();
                    temps.Add(temp.LocalSymbol);
                    sideeffects.Add(store);
                }
                else
                {
                    newArgs?.Add(arg);
                }
            }

            return newArgs?.ToImmutableAndFree() ?? args;
        }

        private BoundExpression MakeObjectInitializerMemberAccess(
            BoundExpression rewrittenReceiver,
            BoundObjectInitializerMember rewrittenLeft,
            bool isRhsNestedInitializer)
        {
            var memberSymbol = rewrittenLeft.MemberSymbol;

#if DEBUG
            var discardedUseSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            // It is possible there are use site diagnostics from the above, but none that we need report as we aren't generating code for the conversion
#endif

#nullable restore
            switch (memberSymbol.Kind)
            {
                case SymbolKind.Field:
                    var fieldSymbol = (FieldSymbol)memberSymbol;
                    return MakeFieldAccess(rewrittenLeft.Syntax, rewrittenReceiver, fieldSymbol, null, rewrittenLeft.ResultKind, fieldSymbol.Type);

                case SymbolKind.Property:
                    var propertySymbol = (PropertySymbol)memberSymbol;
                    var arguments = rewrittenLeft.Arguments;
                    if (!arguments.IsEmpty || propertySymbol.IsIndexedProperty)
                    {
                        return MakeIndexerAccess(
                            rewrittenLeft.Syntax,
                            rewrittenReceiver,
                            propertySymbol,
                            rewrittenLeft.Arguments,
                            rewrittenLeft.ArgumentNamesOpt,
                            rewrittenLeft.ArgumentRefKindsOpt,
                            rewrittenLeft.Expanded,
                            rewrittenLeft.ArgsToParamsOpt,
                            rewrittenLeft.DefaultArguments,
                            type: propertySymbol.Type,
                            oldNodeOpt: null,
                            isLeftOfAssignment: !isRhsNestedInitializer);
                    }
                    else
                    {
                        return MakePropertyAccess(
                            rewrittenLeft.Syntax,
                            rewrittenReceiver,
                            propertySymbol,
                            rewrittenLeft.ResultKind,
                            propertySymbol.Type,
                            isLeftOfAssignment: !isRhsNestedInitializer);
                    }

                case SymbolKind.Event:
                    var eventSymbol = (EventSymbol)memberSymbol;
                    return MakeEventAccess(rewrittenLeft.Syntax, rewrittenReceiver, eventSymbol, null, rewrittenLeft.ResultKind, eventSymbol.Type);

                default:
                    throw ExceptionUtilities.UnexpectedValue(memberSymbol.Kind);
            }
        }
    }
}
