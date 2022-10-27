// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Syntax.InternalSyntax
{
    public abstract partial class SyntaxList : GreenNode
    {
        public SyntaxList()
            : base(GreenNode.ListKind)
        {
        }

        public SyntaxList(DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(GreenNode.ListKind, diagnostics, annotations)
        {
        }

        public SyntaxList(ObjectReader reader)
            : base(reader)
        {
        }

        public static GreenNode List(GreenNode child)
        {
            return child;
        }

        public static WithTwoChildren List(GreenNode child0, GreenNode child1)
        {
            RoslynDebug.Assert(child0 != null);
            RoslynDebug.Assert(child1 != null);

            GreenNode? cached = SyntaxNodeCache.TryGetNode(GreenNode.ListKind, child0, child1, out int hash);
            if (cached != null)
                return (WithTwoChildren)cached;

            var result = new WithTwoChildren(child0, child1);
            if (hash >= 0)
            {
                SyntaxNodeCache.AddNode(result, hash);
            }

            return result;
        }

        public static WithThreeChildren List(GreenNode child0, GreenNode child1, GreenNode child2)
        {
            RoslynDebug.Assert(child0 != null);
            RoslynDebug.Assert(child1 != null);
            RoslynDebug.Assert(child2 != null);

            GreenNode? cached = SyntaxNodeCache.TryGetNode(GreenNode.ListKind, child0, child1, child2, out int hash);
            if (cached != null)
                return (WithThreeChildren)cached;

            var result = new WithThreeChildren(child0, child1, child2);
            if (hash >= 0)
            {
                SyntaxNodeCache.AddNode(result, hash);
            }

            return result;
        }

        public static GreenNode List(GreenNode?[] nodes)
        {
            return List(nodes, nodes.Length);
        }

        public static GreenNode List(GreenNode?[] nodes, int count)
        {
            var array = new ArrayElement<GreenNode>[count];
            for (int i = 0; i < count; i++)
            {
                var node = nodes[i];
                Debug.Assert(node is object);
#nullable restore
                array[i].Value = node;
#nullable enable
            }

            return List(array);
        }

        public static SyntaxList List(ArrayElement<GreenNode>[] children)
        {
            // "WithLotsOfChildren" list will allocate a separate array to hold
            // precomputed node offsets. It may not be worth it for smallish lists.
            if (children.Length < 10)
            {
                return new WithManyChildren(children);
            }
            else
            {
                return new WithLotsOfChildren(children);
            }
        }

        internal abstract void CopyTo(ArrayElement<GreenNode>[] array, int offset);

        public static GreenNode? Concat(GreenNode? left, GreenNode? right)
        {
            if (left == null)
            {
                return right;
            }

            if (right == null)
            {
                return left;
            }

            var rightList = right as SyntaxList;
            if (left is SyntaxList leftList)
            {
                if (rightList != null)
                {
                    var tmp = new ArrayElement<GreenNode>[left.SlotCount + right.SlotCount];
                    leftList.CopyTo(tmp, 0);
                    rightList.CopyTo(tmp, left.SlotCount);
                    return List(tmp);
                }
                else
                {
                    var tmp = new ArrayElement<GreenNode>[left.SlotCount + 1];
                    leftList.CopyTo(tmp, 0);
                    tmp[left.SlotCount].Value = right;
                    return List(tmp);
                }
            }
            else if (rightList != null)
            {
                var tmp = new ArrayElement<GreenNode>[rightList.SlotCount + 1];
                tmp[0].Value = left;
                rightList.CopyTo(tmp, 1);
                return List(tmp);
            }
            else
            {
                return List(left, right);
            }
        }

        public sealed override string Language
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public sealed override string KindText
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public sealed override SyntaxNode GetStructure(SyntaxTrivia parentTrivia)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public sealed override SyntaxToken CreateSeparator<TNode>(SyntaxNode element)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public sealed override bool IsTriviaWithEndOfLine()
        {
            return false;
        }
    }
}
