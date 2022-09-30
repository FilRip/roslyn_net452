using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.CodeGen
{
    internal class Optimizer
    {
        public static BoundStatement Optimize(BoundStatement src, bool debugFriendly, out HashSet<LocalSymbol> stackLocals)
        {
            PooledDictionary<LocalSymbol, LocalDefUseInfo> instance = PooledDictionary<LocalSymbol, LocalDefUseInfo>.GetInstance();
            src = (BoundStatement)StackOptimizerPass1.Analyze(src, instance, debugFriendly);
            FilterValidStackLocals(instance);
            BoundStatement result;
            if (instance.Count == 0)
            {
                stackLocals = null;
                result = src;
            }
            else
            {
                stackLocals = new HashSet<LocalSymbol>(instance.Keys);
                result = StackOptimizerPass2.Rewrite(src, instance);
            }
            foreach (LocalDefUseInfo value in instance.Values)
            {
                value.Free();
            }
            instance.Free();
            return result;
        }

        private static void FilterValidStackLocals(Dictionary<LocalSymbol, LocalDefUseInfo> info)
        {
            ArrayBuilder<LocalDefUseInfo> instance = ArrayBuilder<LocalDefUseInfo>.GetInstance();
            LocalSymbol[] array = info.Keys.ToArray();
            foreach (LocalSymbol localSymbol in array)
            {
                LocalDefUseInfo localDefUseInfo = info[localSymbol];
                if (localSymbol.SynthesizedKind == SynthesizedLocalKind.OptimizerTemp)
                {
                    instance.Add(localDefUseInfo);
                    info.Remove(localSymbol);
                }
                else if (localDefUseInfo.CannotSchedule)
                {
                    localDefUseInfo.Free();
                    info.Remove(localSymbol);
                }
            }
            if (info.Count != 0)
            {
                RemoveIntersectingLocals(info, instance);
            }
            ArrayBuilder<LocalDefUseInfo>.Enumerator enumerator = instance.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Free();
            }
            instance.Free();
        }

        private static void RemoveIntersectingLocals(Dictionary<LocalSymbol, LocalDefUseInfo> info, ArrayBuilder<LocalDefUseInfo> dummies)
        {
            ArrayBuilder<LocalDefUseSpan> instance = ArrayBuilder<LocalDefUseSpan>.GetInstance(dummies.Count);
            ArrayBuilder<LocalDefUseInfo>.Enumerator enumerator = dummies.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ArrayBuilder<LocalDefUseSpan>.Enumerator enumerator2 = enumerator.Current.LocalDefs.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    LocalDefUseSpan current = enumerator2.Current;
                    if (current.Start != current.End)
                    {
                        instance.Add(current);
                    }
                }
            }
            int count = instance.Count;
            foreach (var item in from _003C_003Eh__TransparentIdentifier0 in info.SelectMany(delegate (KeyValuePair<LocalSymbol, LocalDefUseInfo> i)
                {
                    KeyValuePair<LocalSymbol, LocalDefUseInfo> keyValuePair = i;
                    return keyValuePair.Value.LocalDefs;
                }, (KeyValuePair<LocalSymbol, LocalDefUseInfo> i, LocalDefUseSpan d) => new { i, d })
                                 orderby _003C_003Eh__TransparentIdentifier0.d.End - _003C_003Eh__TransparentIdentifier0.d.Start, _003C_003Eh__TransparentIdentifier0.d.End
                                 select new
                                 {
                                     i = _003C_003Eh__TransparentIdentifier0.i.Key,
                                     d = _003C_003Eh__TransparentIdentifier0.d
                                 })
            {
                if (!info.ContainsKey(item.i))
                {
                    continue;
                }
                LocalDefUseSpan d2 = item.d;
                int count2 = instance.Count;
                bool flag;
                if (count2 > 5000)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                    for (int j = 0; j < count; j++)
                    {
                        LocalDefUseSpan dummy = instance[j];
                        if (d2.ConflictsWithDummy(dummy))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        for (int k = count; k < count2; k++)
                        {
                            LocalDefUseSpan other = instance[k];
                            if (d2.ConflictsWith(other))
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                }
                if (flag)
                {
                    info[item.i].LocalDefs.Free();
                    info.Remove(item.i);
                }
                else
                {
                    instance.Add(d2);
                }
            }
            instance.Free();
        }
    }
}
