// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    internal sealed partial class CSharpOperationFactory
    {
        internal ImmutableArray<BoundStatement> ToStatements(BoundStatement? statement)
        {
            if (statement == null)
            {
                return ImmutableArray<BoundStatement>.Empty;
            }

            if (statement.Kind == BoundKind.StatementList)
            {
                return ((BoundStatementList)statement).Statements;
            }

            return ImmutableArray.Create(statement);
        }

        private IInstanceReferenceOperation CreateImplicitReceiver(SyntaxNode syntax, TypeSymbol type) =>
            new InstanceReferenceOperation(InstanceReferenceKind.ImplicitReceiver, _semanticModel, syntax, type.GetPublicSymbol(), isImplicit: true);

        internal IArgumentOperation CreateArgumentOperation(ArgumentKind kind, IParameterSymbol? parameter, BoundExpression expression)
        {
            // put argument syntax to argument operation
            IOperation value = Create(expression);
            (SyntaxNode syntax, bool isImplicit) = expression.Syntax is { Parent: ArgumentSyntax parent } ? (parent, expression.WasCompilerGenerated) : (value.Syntax, true);
            return new ArgumentOperation(
                kind,
                parameter,
                value,
                OperationFactory.IdentityConversion,
                OperationFactory.IdentityConversion,
                _semanticModel,
                syntax,
                isImplicit);
        }

        internal IVariableInitializerOperation? CreateVariableDeclaratorInitializer(BoundLocalDeclaration boundLocalDeclaration, SyntaxNode syntax)
        {
            if (boundLocalDeclaration.InitializerOpt != null)
            {
                SyntaxNode? initializerSyntax = null;
                bool initializerIsImplicit = false;
                if (syntax is VariableDeclaratorSyntax variableDeclarator)
                {
                    initializerSyntax = variableDeclarator.Initializer;
                }
                else
                {
                    Debug.Fail($"Unexpected syntax kind: {syntax.Kind()}");
                }

                if (initializerSyntax == null)
                {
                    // There is no explicit syntax for the initializer, so we use the initializerValue's syntax and mark the operation as implicit.
                    initializerSyntax = boundLocalDeclaration.InitializerOpt.Syntax;
                    initializerIsImplicit = true;
                }

                IOperation value = Create(boundLocalDeclaration.InitializerOpt);
                return new VariableInitializerOperation(locals: ImmutableArray<ILocalSymbol>.Empty, value, _semanticModel, initializerSyntax, initializerIsImplicit);
            }

            return null;
        }

        private IVariableDeclaratorOperation CreateVariableDeclaratorInternal(BoundLocalDeclaration boundLocalDeclaration, SyntaxNode syntax)
        {
            ILocalSymbol symbol = boundLocalDeclaration.LocalSymbol.GetPublicSymbol();
            bool isImplicit = false;

            IVariableInitializerOperation? initializer = CreateVariableDeclaratorInitializer(boundLocalDeclaration, syntax);
            ImmutableArray<IOperation> ignoredDimensions = CreateFromArray<BoundExpression, IOperation>(boundLocalDeclaration.ArgumentsOpt);

            return new VariableDeclaratorOperation(symbol, initializer, ignoredDimensions, _semanticModel, syntax, isImplicit);
        }

        [return: NotNullIfNotNull("boundLocal")]
        internal IVariableDeclaratorOperation? CreateVariableDeclarator(BoundLocal? boundLocal)
        {
            return boundLocal == null ? null : new VariableDeclaratorOperation(boundLocal.LocalSymbol.GetPublicSymbol(), initializer: null, ignoredArguments: ImmutableArray<IOperation>.Empty, semanticModel: _semanticModel, syntax: boundLocal.Syntax, isImplicit: false);
        }

        internal IOperation? CreateReceiverOperation(BoundNode? instance, Symbol? symbol)
        {
            if (instance == null || instance.Kind == BoundKind.TypeExpression)
            {
                return null;
            }

            // Static members cannot have an implicit this receiver
            if (symbol != null && symbol.IsStatic && instance.WasCompilerGenerated && instance.Kind == BoundKind.ThisReference)
            {
                return null;
            }

            return Create(instance);
        }

        private bool IsCallVirtual(MethodSymbol? targetMethod, BoundExpression? receiver)
        {
            return targetMethod is not null && receiver != null &&
                   (targetMethod.IsVirtual || targetMethod.IsAbstract || targetMethod.IsOverride) &&
                   !receiver.SuppressVirtualCalls;
        }

        private bool IsMethodInvalid(LookupResultKind resultKind, MethodSymbol targetMethod) =>
            resultKind == LookupResultKind.OverloadResolutionFailure || targetMethod?.OriginalDefinition is ErrorMethodSymbol;

        internal IEventReferenceOperation CreateBoundEventAccessOperation(BoundEventAssignmentOperator boundEventAssignmentOperator)
        {
            SyntaxNode syntax = boundEventAssignmentOperator.Syntax;
            // BoundEventAssignmentOperator doesn't hold on to BoundEventAccess provided during binding.
            // Based on the implementation of those two bound node types, the following data can be retrieved w/o changing BoundEventAssignmentOperator:
            //  1. the type of BoundEventAccess is the type of the event symbol.
            //  2. the constant value of BoundEventAccess is always null.
            //  3. the syntax of the boundEventAssignmentOperator is always AssignmentExpressionSyntax, so the syntax for the event reference would be the LHS of the assignment.
            IEventSymbol @event = boundEventAssignmentOperator.Event.GetPublicSymbol();
            IOperation? instance = CreateReceiverOperation(boundEventAssignmentOperator.ReceiverOpt, boundEventAssignmentOperator.Event);
            SyntaxNode eventAccessSyntax = ((AssignmentExpressionSyntax)syntax).Left;
            bool isImplicit = boundEventAssignmentOperator.WasCompilerGenerated;

            return new EventReferenceOperation(@event, instance, _semanticModel, eventAccessSyntax, @event.Type, isImplicit);
        }

        internal IOperation CreateDelegateTargetOperation(BoundNode delegateNode)
        {
            if (delegateNode is BoundConversion boundConversion)
            {
                if (boundConversion.ConversionKind == ConversionKind.MethodGroup)
                {
                    // We don't check HasErrors on the conversion here because if we actually have a MethodGroup conversion,
                    // overload resolution succeeded. The resulting method could be invalid for other reasons, but we don't
                    // hide the resolved method.
#nullable restore
                    return CreateBoundMethodGroupSingleMethodOperation((BoundMethodGroup)boundConversion.Operand,
                                                                       boundConversion.SymbolOpt,
                                                                       boundConversion.SuppressVirtualCalls);
                }
                else
                {
                    return Create(boundConversion.Operand);
                }
            }
            else
            {
                var boundDelegateCreationExpression = (BoundDelegateCreationExpression)delegateNode;
                if (boundDelegateCreationExpression.Argument.Kind == BoundKind.MethodGroup &&
                    boundDelegateCreationExpression.MethodOpt != null)
                {
                    // If this is a method binding, and a valid candidate method was found, then we want to expose
                    // this child as an IMethodBindingReference. Otherwise, we want to just delegate to the standard
                    // CSharpOperationFactory behavior. Note we don't check HasErrors here because if we have a method group,
                    // overload resolution succeeded, even if the resulting method isn't valid for some other reason.
                    BoundMethodGroup boundMethodGroup = (BoundMethodGroup)boundDelegateCreationExpression.Argument;
                    return CreateBoundMethodGroupSingleMethodOperation(boundMethodGroup, boundDelegateCreationExpression.MethodOpt, boundMethodGroup.SuppressVirtualCalls);
                }
                else
                {
                    return Create(boundDelegateCreationExpression.Argument);
                }
            }
        }

        internal IOperation CreateMemberInitializerInitializedMember(BoundNode initializedMember)
        {

            return initializedMember switch
            {
                BoundObjectInitializerMember objectInitializer => CreateBoundObjectInitializerMemberOperation(objectInitializer, isObjectOrCollectionInitializer: true),
                BoundDynamicObjectInitializerMember dynamicInitializer => CreateBoundDynamicObjectInitializerMemberOperation(dynamicInitializer),
                _ => Create(initializedMember),
            };
        }

#pragma warning disable IDE0060
        internal ImmutableArray<IArgumentOperation> DeriveArguments(BoundNode containingExpression, bool isObjectOrCollectionInitializer)
#pragma warning restore IDE0060
        {
            switch (containingExpression.Kind)
            {
                case BoundKind.ObjectInitializerMember:
                    {
                        var boundObjectInitializerMember = (BoundObjectInitializerMember)containingExpression;
#nullable enable
                        var property = (PropertySymbol?)boundObjectInitializerMember.MemberSymbol;
                        return DeriveArguments(
#nullable restore
                                    property,
                                    boundObjectInitializerMember.Arguments,
                                    boundObjectInitializerMember.ArgsToParamsOpt,
                                    boundObjectInitializerMember.DefaultArguments,
                                    boundObjectInitializerMember.Expanded,
                                    boundObjectInitializerMember.Syntax);
                    }

                default:
                    return DeriveArguments(containingExpression);
            }
        }

        internal ImmutableArray<IArgumentOperation> DeriveArguments(BoundNode containingExpression)
        {
            switch (containingExpression.Kind)
            {
                case BoundKind.IndexerAccess:
                    {
                        var boundIndexer = (BoundIndexerAccess)containingExpression;
                        return DeriveArguments(boundIndexer.Indexer,
                                               boundIndexer.Arguments,
                                               boundIndexer.ArgsToParamsOpt,
                                               boundIndexer.DefaultArguments,
                                               boundIndexer.Expanded,
                                               boundIndexer.Syntax);
                    }
                case BoundKind.ObjectCreationExpression:
                    {
                        var objectCreation = (BoundObjectCreationExpression)containingExpression;
                        return DeriveArguments(objectCreation.Constructor,
                                               objectCreation.Arguments,
                                               objectCreation.ArgsToParamsOpt,
                                               objectCreation.DefaultArguments,
                                               objectCreation.Expanded,
                                               objectCreation.Syntax);
                    }
                case BoundKind.Call:
                    {
                        var boundCall = (BoundCall)containingExpression;
                        return DeriveArguments(boundCall.Method,
                                               boundCall.Arguments,
                                               boundCall.ArgsToParamsOpt,
                                               boundCall.DefaultArguments,
                                               boundCall.Expanded,
                                               boundCall.Syntax,
                                               boundCall.InvokedAsExtensionMethod);
                    }
                case BoundKind.CollectionElementInitializer:
                    {
                        var boundCollectionElementInitializer = (BoundCollectionElementInitializer)containingExpression;
                        return DeriveArguments(boundCollectionElementInitializer.AddMethod,
                                               boundCollectionElementInitializer.Arguments,
                                               boundCollectionElementInitializer.ArgsToParamsOpt,
                                               boundCollectionElementInitializer.DefaultArguments,
                                               boundCollectionElementInitializer.Expanded,
                                               boundCollectionElementInitializer.Syntax,
                                               boundCollectionElementInitializer.InvokedAsExtensionMethod);
                    }

                default:
                    throw ExceptionUtilities.UnexpectedValue(containingExpression.Kind);
            }
        }

        private ImmutableArray<IArgumentOperation> DeriveArguments(
            Symbol methodOrIndexer,
            ImmutableArray<BoundExpression> boundArguments,
            ImmutableArray<int> argumentsToParametersOpt,
            BitVector defaultArguments,
            bool expanded,
            SyntaxNode invocationSyntax,
            bool invokedAsExtensionMethod = false)
        {
            // We can simply return empty array only if both parameters and boundArguments are empty, because:
            // - if only parameters is empty, there's error in code but we still need to return provided expression.
            // - if boundArguments is empty, then either there's error or we need to provide values for optional/param-array parameters.
            if (methodOrIndexer.GetParameters().IsDefaultOrEmpty && boundArguments.IsDefaultOrEmpty)
            {
                return ImmutableArray<IArgumentOperation>.Empty;
            }

            return LocalRewriter.MakeArgumentsInEvaluationOrder(
                 operationFactory: this,
                 compilation: (CSharpCompilation)_semanticModel.Compilation,
                 syntax: invocationSyntax,
                 arguments: boundArguments,
                 methodOrIndexer: methodOrIndexer,
                 expanded: expanded,
                 argsToParamsOpt: argumentsToParametersOpt,
                 defaultArguments: defaultArguments,
                 invokedAsExtensionMethod: invokedAsExtensionMethod);
        }

#nullable enable

        internal static ImmutableArray<BoundNode> CreateInvalidChildrenFromArgumentsExpression(BoundNode? receiverOpt, ImmutableArray<BoundExpression> arguments, BoundExpression? additionalNodeOpt = null)
        {
            var builder = ArrayBuilder<BoundNode>.GetInstance();

            if (receiverOpt != null
               && (!receiverOpt.WasCompilerGenerated
                   || (receiverOpt.Kind != BoundKind.ThisReference
                      && receiverOpt.Kind != BoundKind.BaseReference
                      && receiverOpt.Kind != BoundKind.ObjectOrCollectionValuePlaceholder)))
            {
                builder.Add(receiverOpt);
            }

            builder.AddRange(StaticCast<BoundNode>.From(arguments));

            builder.AddIfNotNull(additionalNodeOpt);

            return builder.ToImmutableAndFree();
        }

        internal ImmutableArray<IOperation> GetAnonymousObjectCreationInitializers(
            ImmutableArray<BoundExpression> arguments,
            ImmutableArray<BoundAnonymousPropertyDeclaration> declarations,
            SyntaxNode syntax,
            ITypeSymbol type,
            bool isImplicit)
        {
            // For error cases and non-assignment initializers, the binder generates only the argument.

            var builder = ArrayBuilder<IOperation>.GetInstance(arguments.Length);
            var currentDeclarationIndex = 0;
            for (int i = 0; i < arguments.Length; i++)
            {
                IOperation value = Create(arguments[i]);

                IOperation target;
                bool isImplicitAssignment;

                // Synthesize an implicit receiver for property reference being assigned.
                var instance = new InstanceReferenceOperation(
                        referenceKind: InstanceReferenceKind.ImplicitReceiver,
                        semanticModel: _semanticModel,
                        syntax: syntax,
                        type: type,
                        isImplicit: true);

                // Find matching declaration for the current argument.
                PropertySymbol property = AnonymousTypeManager.GetAnonymousTypeProperty(type.GetSymbol<NamedTypeSymbol>(), i);
                BoundAnonymousPropertyDeclaration? anonymousProperty = getDeclaration(declarations, property, ref currentDeclarationIndex);
                if (anonymousProperty is null)
                {
                    // No matching declaration, synthesize a property reference to be assigned.
                    target = new PropertyReferenceOperation(
                        property.GetPublicSymbol(),
                        arguments: ImmutableArray<IArgumentOperation>.Empty,
                        instance,
                        semanticModel: _semanticModel,
                        syntax: value.Syntax,
                        type: property.Type.GetPublicSymbol(),
                        isImplicit: true);
                    isImplicitAssignment = true;
                }
                else
                {
                    target = new PropertyReferenceOperation(anonymousProperty.Property.GetPublicSymbol(),
                                                            ImmutableArray<IArgumentOperation>.Empty,
                                                            instance,
                                                            _semanticModel,
                                                            anonymousProperty.Syntax,
                                                            anonymousProperty.GetPublicTypeSymbol(),
                                                            anonymousProperty.WasCompilerGenerated);
                    isImplicitAssignment = isImplicit;
                }

                var assignmentSyntax = value.Syntax?.Parent ?? syntax;
                ITypeSymbol? assignmentType = target.Type;
                bool isRef = false;
                var assignment = new SimpleAssignmentOperation(isRef, target, value, _semanticModel, assignmentSyntax, assignmentType, value.GetConstantValue(), isImplicitAssignment);
                builder.Add(assignment);
            }

            return builder.ToImmutableAndFree();

            static BoundAnonymousPropertyDeclaration? getDeclaration(ImmutableArray<BoundAnonymousPropertyDeclaration> declarations, PropertySymbol currentProperty, ref int currentDeclarationIndex)
            {
                if (currentDeclarationIndex >= declarations.Length)
                {
                    return null;
                }

                var currentDeclaration = declarations[currentDeclarationIndex];

                if (currentProperty.MemberIndexOpt == currentDeclaration.Property.MemberIndexOpt)
                {
                    currentDeclarationIndex++;
                    return currentDeclaration;
                }

                return null;
            }
        }

        internal class Helper
        {
            internal static bool IsPostfixIncrementOrDecrement(CSharp.UnaryOperatorKind operatorKind)
            {
                return operatorKind.Operator() switch
                {
                    CSharp.UnaryOperatorKind.PostfixIncrement or CSharp.UnaryOperatorKind.PostfixDecrement => true,
                    _ => false,
                };
            }

            internal static bool IsDecrement(CSharp.UnaryOperatorKind operatorKind)
            {
                return operatorKind.Operator() switch
                {
                    CSharp.UnaryOperatorKind.PrefixDecrement or CSharp.UnaryOperatorKind.PostfixDecrement => true,
                    _ => false,
                };
            }

            internal static UnaryOperatorKind DeriveUnaryOperatorKind(CSharp.UnaryOperatorKind operatorKind)
            {
                return operatorKind.Operator() switch
                {
                    CSharp.UnaryOperatorKind.UnaryPlus => UnaryOperatorKind.Plus,
                    CSharp.UnaryOperatorKind.UnaryMinus => UnaryOperatorKind.Minus,
                    CSharp.UnaryOperatorKind.LogicalNegation => UnaryOperatorKind.Not,
                    CSharp.UnaryOperatorKind.BitwiseComplement => UnaryOperatorKind.BitwiseNegation,
                    CSharp.UnaryOperatorKind.True => UnaryOperatorKind.True,
                    CSharp.UnaryOperatorKind.False => UnaryOperatorKind.False,
                    _ => UnaryOperatorKind.None,
                };
            }

            internal static BinaryOperatorKind DeriveBinaryOperatorKind(CSharp.BinaryOperatorKind operatorKind)
            {
                return operatorKind.OperatorWithLogical() switch
                {
                    CSharp.BinaryOperatorKind.Addition => BinaryOperatorKind.Add,
                    CSharp.BinaryOperatorKind.Subtraction => BinaryOperatorKind.Subtract,
                    CSharp.BinaryOperatorKind.Multiplication => BinaryOperatorKind.Multiply,
                    CSharp.BinaryOperatorKind.Division => BinaryOperatorKind.Divide,
                    CSharp.BinaryOperatorKind.Remainder => BinaryOperatorKind.Remainder,
                    CSharp.BinaryOperatorKind.LeftShift => BinaryOperatorKind.LeftShift,
                    CSharp.BinaryOperatorKind.RightShift => BinaryOperatorKind.RightShift,
                    CSharp.BinaryOperatorKind.And => BinaryOperatorKind.And,
                    CSharp.BinaryOperatorKind.Or => BinaryOperatorKind.Or,
                    CSharp.BinaryOperatorKind.Xor => BinaryOperatorKind.ExclusiveOr,
                    CSharp.BinaryOperatorKind.LessThan => BinaryOperatorKind.LessThan,
                    CSharp.BinaryOperatorKind.LessThanOrEqual => BinaryOperatorKind.LessThanOrEqual,
                    CSharp.BinaryOperatorKind.Equal => BinaryOperatorKind.Equals,
                    CSharp.BinaryOperatorKind.NotEqual => BinaryOperatorKind.NotEquals,
                    CSharp.BinaryOperatorKind.GreaterThanOrEqual => BinaryOperatorKind.GreaterThanOrEqual,
                    CSharp.BinaryOperatorKind.GreaterThan => BinaryOperatorKind.GreaterThan,
                    CSharp.BinaryOperatorKind.LogicalAnd => BinaryOperatorKind.ConditionalAnd,
                    CSharp.BinaryOperatorKind.LogicalOr => BinaryOperatorKind.ConditionalOr,
                    _ => BinaryOperatorKind.None,
                };
            }
        }
    }
}
