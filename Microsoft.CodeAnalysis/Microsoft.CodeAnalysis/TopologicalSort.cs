using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis
{
    public static class TopologicalSort
    {
        public static bool TryIterativeSort<TNode>(IEnumerable<TNode> nodes, Func<TNode, ImmutableArray<TNode>> successors, out ImmutableArray<TNode> result) where TNode : notnull
        {
            PooledDictionary<TNode, int> pooledDictionary = PredecessorCounts(nodes, successors, out ImmutableArray<TNode> allNodes);
            ArrayBuilder<TNode> instance = ArrayBuilder<TNode>.GetInstance();
            ImmutableArray<TNode>.Enumerator enumerator = allNodes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TNode current = enumerator.Current;
                if (pooledDictionary[current] == 0)
                {
                    instance.Push(current);
                }
            }
            ArrayBuilder<TNode> instance2 = ArrayBuilder<TNode>.GetInstance();
            while (instance.Count != 0)
            {
                TNode val = instance.Pop();
                instance2.Add(val);
                enumerator = successors(val).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TNode current2 = enumerator.Current;
                    if (pooledDictionary[current2]-- == 1)
                    {
                        instance.Push(current2);
                    }
                }
            }
            bool flag = pooledDictionary.Count != instance2.Count;
            result = (flag ? ImmutableArray<TNode>.Empty : instance2.ToImmutable());
            pooledDictionary.Free();
            instance.Free();
            instance2.Free();
            return !flag;
        }

        private static PooledDictionary<TNode, int> PredecessorCounts<TNode>(IEnumerable<TNode> nodes, Func<TNode, ImmutableArray<TNode>> successors, out ImmutableArray<TNode> allNodes) where TNode : notnull
        {
            PooledDictionary<TNode, int> instance = PooledDictionary<TNode, int>.GetInstance();
            PooledHashSet<TNode> instance2 = PooledHashSet<TNode>.GetInstance();
            ArrayBuilder<TNode> instance3 = ArrayBuilder<TNode>.GetInstance();
            ArrayBuilder<TNode> instance4 = ArrayBuilder<TNode>.GetInstance();
            instance3.AddRange(nodes);
            while (instance3.Count != 0)
            {
                TNode val = instance3.Pop();
                if (!instance2.Add(val))
                {
                    continue;
                }
                instance4.Add(val);
                if (!instance.ContainsKey(val))
                {
                    instance.Add(val, 0);
                }
                ImmutableArray<TNode>.Enumerator enumerator = successors(val).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TNode current = enumerator.Current;
                    instance3.Push(current);
                    if (instance.TryGetValue(current, out var value))
                    {
                        instance[current] = value + 1;
                    }
                    else
                    {
                        instance.Add(current, 1);
                    }
                }
            }
            instance2.Free();
            instance3.Free();
            allNodes = instance4.ToImmutableAndFree();
            return instance;
        }
    }
}
