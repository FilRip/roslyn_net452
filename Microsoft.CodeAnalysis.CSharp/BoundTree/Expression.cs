// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

using Microsoft.CodeAnalysis.Operations;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class BoundObjectCreationExpression : IBoundInvalidNode
    {
        internal static ImmutableArray<BoundExpression> GetChildInitializers(BoundExpression? objectOrCollectionInitializer)
        {
            if (objectOrCollectionInitializer is BoundObjectInitializerExpression objectInitializerExpression)
            {
                return objectInitializerExpression.Initializers;
            }

            if (objectOrCollectionInitializer is BoundCollectionInitializerExpression collectionInitializerExpression)
            {
                return collectionInitializerExpression.Initializers;
            }

            return ImmutableArray<BoundExpression>.Empty;
        }

        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren => CSharpOperationFactory.CreateInvalidChildrenFromArgumentsExpression(receiverOpt: null, Arguments, InitializerExpressionOpt);
    }

    public sealed partial class BoundObjectInitializerMember : IBoundInvalidNode
    {
        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren => StaticCast<BoundNode>.From(Arguments);
    }

    public sealed partial class BoundCollectionElementInitializer : IBoundInvalidNode
    {
        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren => CSharpOperationFactory.CreateInvalidChildrenFromArgumentsExpression(ImplicitReceiverOpt, Arguments);
    }

    public sealed partial class BoundDeconstructionAssignmentOperator : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create<BoundNode?>(this.Left, this.Right);
    }

    public partial class BoundBadExpression : IBoundInvalidNode
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode?>.From(this.ChildBoundNodes);

        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren => StaticCast<BoundNode>.From(this.ChildBoundNodes);
    }

    public partial class BoundCall : IBoundInvalidNode
    {
        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren => CSharpOperationFactory.CreateInvalidChildrenFromArgumentsExpression(ReceiverOpt, Arguments);
    }

    public partial class BoundIndexerAccess : IBoundInvalidNode
    {
        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren => CSharpOperationFactory.CreateInvalidChildrenFromArgumentsExpression(ReceiverOpt, Arguments);
    }

    public partial class BoundDynamicIndexerAccess
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode?>.From(this.Arguments.Insert(0, this.Receiver));
    }

    public partial class BoundAnonymousObjectCreationExpression
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode?>.From(this.Arguments);
    }

    public partial class BoundAttribute
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode?>.From(this.ConstructorArguments.AddRange(StaticCast<BoundExpression>.From(this.NamedArguments)));
    }

    public partial class BoundQueryClause
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create<BoundNode?>(this.Value);
    }

    public partial class BoundArgListOperator
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode?>.From(this.Arguments);
    }

    public partial class BoundNameOfOperator
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create<BoundNode?>(this.Argument);
    }

    public partial class BoundPointerElementAccess
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create<BoundNode?>(this.Expression, this.Index);
    }

    public partial class BoundRefTypeOperator
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create<BoundNode?>(this.Operand);
    }

    public partial class BoundDynamicMemberAccess
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create<BoundNode?>(this.Receiver);
    }

    public partial class BoundMakeRefOperator
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create<BoundNode?>(this.Operand);
    }

    public partial class BoundRefValueOperator
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create<BoundNode?>(this.Operand);
    }

    public partial class BoundDynamicInvocation
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode?>.From(this.Arguments.Insert(0, this.Expression));
    }

    public partial class BoundFixedLocalCollectionInitializer
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create<BoundNode?>(this.Expression);
    }

    public partial class BoundStackAllocArrayCreationBase
    {
        internal static ImmutableArray<BoundExpression> GetChildInitializers(BoundArrayInitialization? arrayInitializer)
        {
            return arrayInitializer?.Initializers ?? ImmutableArray<BoundExpression>.Empty;
        }
    }

    public partial class BoundStackAllocArrayCreation
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode?>.From(GetChildInitializers(this.InitializerOpt).Insert(0, this.Count));
    }

    public partial class BoundConvertedStackAllocExpression
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode?>.From(GetChildInitializers(this.InitializerOpt).Insert(0, this.Count));
    }

    public partial class BoundDynamicObjectCreationExpression
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode?>.From(this.Arguments.AddRange(BoundObjectCreationExpression.GetChildInitializers(this.InitializerExpressionOpt)));
    }

    public partial class BoundThrowExpression
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create<BoundNode?>(this.Expression);
    }

    public abstract partial class BoundMethodOrPropertyGroup
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create<BoundNode?>(this.ReceiverOpt);
    }

    public partial class BoundSequence
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode?>.From(this.SideEffects.Add(this.Value));
    }

    public partial class BoundStatementList
    {
        protected override ImmutableArray<BoundNode?> Children =>
            (this.Kind == BoundKind.StatementList || this.Kind == BoundKind.Scope) ? StaticCast<BoundNode?>.From(this.Statements) : ImmutableArray<BoundNode?>.Empty;
    }

    public partial class BoundPassByCopy
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create<BoundNode?>(this.Expression);
    }

    public partial class BoundIndexOrRangePatternIndexerAccess
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create<BoundNode?>(Receiver, Argument);
    }

    public partial class BoundFunctionPointerInvocation : IBoundInvalidNode
    {
        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren => CSharpOperationFactory.CreateInvalidChildrenFromArgumentsExpression(receiverOpt: this.InvokedExpression, Arguments);
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode?>.From(((IBoundInvalidNode)this).InvalidNodeChildren);
    }
}
