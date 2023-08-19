// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class Binder
    {
        /// <summary>
        /// Represents a small change from the enclosing/next binder.
        /// Can specify a BindingLocation and a ContainingMemberOrLambda.
        /// </summary>
        private sealed class BinderWithContainingMemberOrLambda : Binder
        {
            private readonly Symbol _containingMemberOrLambda;

            internal BinderWithContainingMemberOrLambda(Binder next, Symbol containingMemberOrLambda)
                : base(next)
            {

                _containingMemberOrLambda = containingMemberOrLambda;
            }

            internal BinderWithContainingMemberOrLambda(Binder next, EBinder flags, Symbol containingMemberOrLambda)
                : base(next, flags)
            {

                _containingMemberOrLambda = containingMemberOrLambda;
            }

            internal override Symbol ContainingMemberOrLambda
            {
                get { return _containingMemberOrLambda; }
            }
        }

        /// <summary>
        /// Represents a small change from the enclosing/next binder.
        /// Can specify a receiver Expression for containing conditional member access.
        /// </summary>
        private sealed class BinderWithConditionalReceiver : Binder
        {
            private readonly BoundExpression _receiverExpression;

            internal BinderWithConditionalReceiver(Binder next, BoundExpression receiverExpression)
                : base(next)
            {

                _receiverExpression = receiverExpression;
            }

            internal override BoundExpression ConditionalReceiverExpression
            {
                get { return _receiverExpression; }
            }
        }

        internal Binder WithFlags(EBinder flags)
        {
            return this.Flags == flags
                ? this
                : new Binder(this, flags);
        }

        internal Binder WithAdditionalFlags(EBinder flags)
        {
            return this.Flags.Includes(flags)
                ? this
                : new Binder(this, this.Flags | flags);
        }

        internal Binder WithContainingMemberOrLambda(Symbol containing)
        {
            return new BinderWithContainingMemberOrLambda(this, containing);
        }

        /// <remarks>
        /// It seems to be common to do both of these things at once, so provide a way to do so
        /// without adding two links to the binder chain.
        /// </remarks>
        internal Binder WithAdditionalFlagsAndContainingMemberOrLambda(EBinder flags, Symbol containing)
        {
            return new BinderWithContainingMemberOrLambda(this, this.Flags | flags, containing);
        }

        internal Binder WithUnsafeRegionIfNecessary(SyntaxTokenList modifiers)
        {
            return (this.Flags.Includes(EBinder.UnsafeRegion) || !modifiers.Any(SyntaxKind.UnsafeKeyword))
                ? this
                : new Binder(this, this.Flags | EBinder.UnsafeRegion);
        }

        internal Binder WithCheckedOrUncheckedRegion(bool @checked)
        {

            EBinder added = @checked ? EBinder.CheckedRegion : EBinder.UncheckedRegion;
            EBinder removed = @checked ? EBinder.UncheckedRegion : EBinder.CheckedRegion;

            return this.Flags.Includes(added)
                ? this
                : new Binder(this, (this.Flags & ~removed) | added);
        }
    }
}
