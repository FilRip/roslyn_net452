// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node)
        {
            var loweredArguments = VisitList(node.Arguments);
            var constructorInvocation = _dynamicFactory.MakeDynamicConstructorInvocation(node.Syntax, node.Type, loweredArguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt).ToExpression();

            if (node.InitializerExpressionOpt == null || node.InitializerExpressionOpt.HasErrors)
            {
                return constructorInvocation;
            }

            return MakeExpressionWithInitializer(node.Syntax, constructorInvocation, node.InitializerExpressionOpt, node.Type);
        }

        public override BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
        {

            // Rewrite the arguments.
            // NOTE: We may need additional argument rewriting such as generating a params array,
            //       re-ordering arguments based on argsToParamsOpt map, etc.
            // NOTE: This is done later by MakeArguments, for now we just lower each argument.
            var rewrittenArguments = VisitList(node.Arguments);

            // We have already lowered each argument, but we may need some additional rewriting for the arguments,
            // such as generating a params array, re-ordering arguments based on argsToParamsOpt map, etc.
            ImmutableArray<RefKind> argumentRefKindsOpt = node.ArgumentRefKindsOpt;
            rewrittenArguments = MakeArguments(
                node.Syntax,
                rewrittenArguments,
                node.Constructor,
                node.Expanded,
                node.ArgsToParamsOpt,
                ref argumentRefKindsOpt,
                out ImmutableArray<LocalSymbol> temps);

            BoundExpression rewrittenObjectCreation;

            if (_inExpressionLambda)
            {
                if (!temps.IsDefaultOrEmpty)
                {
                    throw ExceptionUtilities.UnexpectedValue(temps.Length);
                }

                rewrittenObjectCreation = node.UpdateArgumentsAndInitializer(rewrittenArguments, argumentRefKindsOpt, MakeObjectCreationInitializerForExpressionTree(node.InitializerExpressionOpt), changeTypeOpt: node.Constructor.ContainingType);

                if (node.Type.IsInterfaceType())
                {
                    rewrittenObjectCreation = MakeConversionNode(rewrittenObjectCreation, node.Type, false, false);
                }

                return rewrittenObjectCreation;
            }

            rewrittenObjectCreation = node.UpdateArgumentsAndInitializer(rewrittenArguments, argumentRefKindsOpt, newInitializerExpression: null, changeTypeOpt: node.Constructor.ContainingType);

            // replace "new S()" with a default struct ctor with "default(S)"
            if (node.Constructor.IsDefaultValueTypeConstructor())
            {
                rewrittenObjectCreation = new BoundDefaultExpression(rewrittenObjectCreation.Syntax, rewrittenObjectCreation.Type!);
            }

            if (!temps.IsDefaultOrEmpty)
            {
                rewrittenObjectCreation = new BoundSequence(
                    node.Syntax,
                    temps,
                    ImmutableArray<BoundExpression>.Empty,
                    rewrittenObjectCreation,
                    node.Type);
            }

            if (node.Type.IsInterfaceType())
            {
                rewrittenObjectCreation = MakeConversionNode(rewrittenObjectCreation, node.Type, false, false);
            }

            if (node.InitializerExpressionOpt == null || node.InitializerExpressionOpt.HasErrors)
            {
                return rewrittenObjectCreation;
            }

            return MakeExpressionWithInitializer(node.Syntax, rewrittenObjectCreation, node.InitializerExpressionOpt, node.Type);
        }

        public override BoundNode VisitWithExpression(BoundWithExpression withExpr)
        {

            // for a with expression of the form
            //
            //      receiver with { P1 = e1, P2 = e2 }
            //
            // if the receiver is a struct, duplicate the value, then set the given struct properties:
            //
            //     var tmp = receiver;
            //     tmp.P1 = e1;
            //     tmp.P2 = e2;
            //     tmp
            //
            // otherwise the receiver is a record class and we want to lower it to a call to its `Clone` method, then
            // set the given record properties. i.e.
            //
            //      var tmp = (ReceiverType)receiver.Clone();
            //      tmp.P1 = e1;
            //      tmp.P2 = e2;
            //      tmp

            BoundExpression expression;

            if (withExpr.Type.IsValueType)
            {
                expression = VisitExpression(withExpr.Receiver);
            }
            else
            {

                expression = _factory.Convert(
                    withExpr.Type,
                    _factory.Call(
                        VisitExpression(withExpr.Receiver),
#nullable restore
                        withExpr.CloneMethod));
#nullable enable
            }

            return MakeExpressionWithInitializer(
                withExpr.Syntax,
                expression,
                withExpr.InitializerExpression,
                withExpr.Type);
        }

        [return: NotNullIfNotNull(nameof(initializerExpressionOpt))]
        private BoundObjectInitializerExpressionBase? MakeObjectCreationInitializerForExpressionTree(BoundObjectInitializerExpressionBase? initializerExpressionOpt)
        {
            if (initializerExpressionOpt != null && !initializerExpressionOpt.HasErrors)
            {
                // We may need to MakeArguments for collection initializer add method call if the method has a param array parameter.
                var rewrittenInitializers = MakeObjectOrCollectionInitializersForExpressionTree(initializerExpressionOpt);
                return UpdateInitializers(initializerExpressionOpt, rewrittenInitializers);
            }

            return null;
        }

        // Shared helper for VisitWithExpression, MakeObjectCreationWithInitializer and MakeNewT
        private BoundExpression MakeExpressionWithInitializer(
            SyntaxNode syntax,
            BoundExpression rewrittenExpression,
            BoundExpression initializerExpression,
            TypeSymbol type)
        {

            // Create a temp and assign it with the object creation expression.
            BoundLocal value = _factory.StoreToTemp(rewrittenExpression, out BoundAssignmentOperator boundAssignmentToTemp);

            // Rewrite object/collection initializer expressions
            ArrayBuilder<BoundExpression>? dynamicSiteInitializers = null;
            ArrayBuilder<LocalSymbol>? temps = null;
            ArrayBuilder<BoundExpression>? loweredInitializers = ArrayBuilder<BoundExpression>.GetInstance();

            AddObjectOrCollectionInitializers(ref dynamicSiteInitializers, ref temps, loweredInitializers, value, initializerExpression);

            int dynamicSiteCount = dynamicSiteInitializers?.Count ?? 0;
            var sideEffects = ArrayBuilder<BoundExpression>.GetInstance(1 + dynamicSiteCount + loweredInitializers.Count);
            sideEffects.Add(boundAssignmentToTemp);

            if (dynamicSiteCount > 0)
            {
                sideEffects.AddRange(dynamicSiteInitializers!);
                dynamicSiteInitializers!.Free();
            }

            sideEffects.AddRange(loweredInitializers);
            loweredInitializers.Free();

            ImmutableArray<LocalSymbol> locals;
            if (temps == null)
            {
                locals = ImmutableArray.Create(value.LocalSymbol);
            }
            else
            {
                temps.Insert(0, value.LocalSymbol);
                locals = temps.ToImmutableAndFree();
            }

            return new BoundSequence(
                syntax,
                locals,
                sideEffects.ToImmutableAndFree(),
                value,
                type);
        }

        public override BoundNode VisitNewT(BoundNewT node)
        {
            if (_inExpressionLambda)
            {
                return node.Update(MakeObjectCreationInitializerForExpressionTree(node.InitializerExpressionOpt), node.Type);
            }

            var rewrittenNewT = MakeNewT(node.Syntax, (TypeParameterSymbol)node.Type);
            if (node.InitializerExpressionOpt == null || node.InitializerExpressionOpt.HasErrors)
            {
                return rewrittenNewT;
            }

            return MakeExpressionWithInitializer(node.Syntax, rewrittenNewT, node.InitializerExpressionOpt, rewrittenNewT.Type!);
        }

        private BoundExpression MakeNewT(SyntaxNode syntax, TypeParameterSymbol typeParameter)
        {
            // "new T()" is rewritten as: "Activator.CreateInstance<T>()".

            // NOTE: DIFFERENCE FROM DEV12
            // Dev12 tried to statically optimize this and would emit default(T) if T happens to be struct
            // However semantics of "new" in C# requires that parameterless constructor be called
            // if struct defines one.
            // Since we cannot know if T has a parameterless constructor statically, 
            // we must call Activator.CreateInstance unconditionally.

            if (!this.TryGetWellKnownTypeMember(syntax, WellKnownMember.System_Activator__CreateInstance_T, out MethodSymbol method))
            {
                return new BoundDefaultExpression(syntax, type: typeParameter, hasErrors: true);
            }

            method = method.Construct(ImmutableArray.Create<TypeSymbol>(typeParameter));

            var createInstanceCall = new BoundCall(
                syntax,
                null,
                method,
                ImmutableArray<BoundExpression>.Empty,
                default,
                default,
                isDelegateCall: false,
                expanded: false,
                invokedAsExtensionMethod: false,
                argsToParamsOpt: default,
                defaultArguments: default,
                resultKind: LookupResultKind.Viable,
                type: typeParameter);

            return createInstanceCall;
        }

        public override BoundNode VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node)
        {
            // For the NoPIA feature, we need to gather the GUID from the coclass, and 
            // generate the following:
            //
            // (IPiaType)System.Activator.CreateInstance(System.Runtime.InteropServices.Marshal.GetTypeFromCLSID(new Guid(GUID)))
            //
            // If System.Runtime.InteropServices.Marshal.GetTypeFromCLSID is not available (older framework),
            // System.Type.GetTypeFromCLSID() is used to get the type for the CLSID:
            //
            // (IPiaType)System.Activator.CreateInstance(System.Type.GetTypeFromCLSID(new Guid(GUID)))

            SyntaxNode oldSyntax = _factory.Syntax;
            _factory.Syntax = node.Syntax;

            var ctor = _factory.WellKnownMethod(WellKnownMember.System_Guid__ctor);
            BoundExpression newGuid;

            if (ctor is { })
            {
                newGuid = _factory.New(ctor, _factory.Literal(node.GuidString));
            }
            else
            {
                newGuid = new BoundBadExpression(node.Syntax, LookupResultKind.NotCreatable, ImmutableArray<Symbol?>.Empty, ImmutableArray<BoundExpression>.Empty, ErrorTypeSymbol.UnknownResultType);
            }

            var getTypeFromCLSID = _factory.WellKnownMethod(WellKnownMember.System_Runtime_InteropServices_Marshal__GetTypeFromCLSID, isOptional: true);

            getTypeFromCLSID ??= _factory.WellKnownMethod(WellKnownMember.System_Type__GetTypeFromCLSID);

            BoundExpression callGetTypeFromCLSID;

            if (getTypeFromCLSID is { })
            {
                callGetTypeFromCLSID = _factory.Call(null, getTypeFromCLSID, newGuid);
            }
            else
            {
                callGetTypeFromCLSID = new BoundBadExpression(node.Syntax, LookupResultKind.OverloadResolutionFailure, ImmutableArray<Symbol?>.Empty, ImmutableArray<BoundExpression>.Empty, ErrorTypeSymbol.UnknownResultType);
            }

            var createInstance = _factory.WellKnownMethod(WellKnownMember.System_Activator__CreateInstance);
            BoundExpression rewrittenObjectCreation;

            if (createInstance is not null)
            {
                rewrittenObjectCreation = _factory.Convert(node.Type, _factory.Call(null, createInstance, callGetTypeFromCLSID));
            }
            else
            {
                rewrittenObjectCreation = new BoundBadExpression(node.Syntax, LookupResultKind.OverloadResolutionFailure, ImmutableArray<Symbol?>.Empty, ImmutableArray<BoundExpression>.Empty, node.Type);
            }

            _factory.Syntax = oldSyntax;

            if (node.InitializerExpressionOpt == null || node.InitializerExpressionOpt.HasErrors)
            {
                return rewrittenObjectCreation;
            }

            return MakeExpressionWithInitializer(node.Syntax, rewrittenObjectCreation, node.InitializerExpressionOpt, node.Type);
        }
    }
}
