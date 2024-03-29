// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Syntax.InternalSyntax
{
    public class SyntaxListPool
    {
        private ArrayElement<SyntaxListBuilder?>[] _freeList = new ArrayElement<SyntaxListBuilder?>[10];
        private int _freeIndex;

#if DEBUG
        private readonly List<SyntaxListBuilder> _allocated = new();
#endif

        public SyntaxListPool()
        {
        }

        public SyntaxListBuilder Allocate()
        {
            SyntaxListBuilder item;
            if (_freeIndex > 0)
            {
                _freeIndex--;
                item = _freeList[_freeIndex].Value!;
                _freeList[_freeIndex].Value = null;
            }
            else
            {
                item = new SyntaxListBuilder(10);
            }

#if DEBUG
            Debug.Assert(!_allocated.Contains(item));
            _allocated.Add(item);
#endif
            return item;
        }

        public SyntaxListBuilder<TNode> Allocate<TNode>() where TNode : GreenNode
        {
            return new SyntaxListBuilder<TNode>(this.Allocate());
        }

        public SeparatedSyntaxListBuilder<TNode> AllocateSeparated<TNode>() where TNode : GreenNode
        {
            return new SeparatedSyntaxListBuilder<TNode>(this.Allocate());
        }

        public void Free<TNode>(in SeparatedSyntaxListBuilder<TNode> item) where TNode : GreenNode
        {
            RoslynDebug.Assert(item.UnderlyingBuilder is not null);
            Free(item.UnderlyingBuilder);
        }

        public void Free(SyntaxListBuilder item)
        {
            item.Clear();
            if (_freeIndex >= _freeList.Length)
            {
                this.Grow();
            }
#if DEBUG
            Debug.Assert(_allocated.Contains(item));

            _allocated.Remove(item);
#endif
            _freeList[_freeIndex].Value = item;
            _freeIndex++;
        }

        private void Grow()
        {
            var tmp = new ArrayElement<SyntaxListBuilder?>[_freeList.Length * 2];
            Array.Copy(_freeList, tmp, _freeList.Length);
            _freeList = tmp;
        }

        public SyntaxList<TNode> ToListAndFree<TNode>(SyntaxListBuilder<TNode> item)
            where TNode : GreenNode
        {
            var list = item.ToList();
            Free(item);
            return list;
        }

        public SeparatedSyntaxList<TNode> ToListAndFree<TNode>(in SeparatedSyntaxListBuilder<TNode> item)
            where TNode : GreenNode
        {
            var list = item.ToList();
            Free(item);
            return list;
        }
    }
}
