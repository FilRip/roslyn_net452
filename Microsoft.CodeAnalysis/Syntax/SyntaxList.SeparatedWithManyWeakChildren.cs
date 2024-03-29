// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

#nullable enable

namespace Microsoft.CodeAnalysis.Syntax
{
    internal partial class SyntaxList
    {
        internal class SeparatedWithManyWeakChildren : SyntaxList
        {
            private readonly ArrayElement<WeakReference<SyntaxNode>?>[] _children;

            internal SeparatedWithManyWeakChildren(InternalSyntax.SyntaxList green, SyntaxNode parent, int position)
                : base(green, parent, position)
            {
                _children = new ArrayElement<WeakReference<SyntaxNode>?>[(((green.SlotCount + 1) >> 1) - 1)];
            }

            public override SyntaxNode? GetNodeSlot(int slot)
            {
                SyntaxNode? result = null;

                if ((slot & 1) == 0)
                {
                    // not a separator
                    result = GetWeakRedElement(ref this._children[slot >> 1].Value, slot);
                }

                return result;
            }

            public override SyntaxNode? GetCachedSlot(int index)
            {
                SyntaxNode? result = null;

                if ((index & 1) == 0)
                {
                    // not a separator
                    var weak = this._children[index >> 1].Value;
                    weak?.TryGetTarget(out result);
                }

                return result;
            }
        }
    }
}
