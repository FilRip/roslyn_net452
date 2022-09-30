using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    internal sealed class MergedAliases
    {
        public ArrayBuilder<string>? AliasesOpt;

        public ArrayBuilder<string>? RecursiveAliasesOpt;

        public ArrayBuilder<MetadataReference>? MergedReferencesOpt;

        internal void Merge(MetadataReference reference)
        {
            ArrayBuilder<string> aliases;
            if (reference.Properties.HasRecursiveAliases)
            {
                if (RecursiveAliasesOpt == null)
                {
                    RecursiveAliasesOpt = ArrayBuilder<string>.GetInstance();
                    RecursiveAliasesOpt!.AddRange(reference.Properties.Aliases);
                    return;
                }
                aliases = RecursiveAliasesOpt;
            }
            else
            {
                if (AliasesOpt == null)
                {
                    AliasesOpt = ArrayBuilder<string>.GetInstance();
                    AliasesOpt!.AddRange(reference.Properties.Aliases);
                    return;
                }
                aliases = AliasesOpt;
            }
            Merge(aliases, reference.Properties.Aliases);
            (MergedReferencesOpt ?? (MergedReferencesOpt = ArrayBuilder<MetadataReference>.GetInstance()))!.Add(reference);
        }

        internal static void Merge(ArrayBuilder<string> aliases, ImmutableArray<string> newAliases)
        {
            if ((aliases.Count == 0) ^ newAliases.IsEmpty)
            {
                AddNonIncluded(aliases, MetadataReferenceProperties.GlobalAlias);
            }
            AddNonIncluded(aliases, newAliases);
        }

        internal static ImmutableArray<string> Merge(ImmutableArray<string> aliasesOpt, ImmutableArray<string> newAliases)
        {
            if (aliasesOpt.IsDefault)
            {
                return newAliases;
            }
            ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance(aliasesOpt.Length);
            instance.AddRange(aliasesOpt);
            Merge(instance, newAliases);
            return instance.ToImmutableAndFree();
        }

        private static void AddNonIncluded(ArrayBuilder<string> builder, string item)
        {
            if (!builder.Contains(item))
            {
                builder.Add(item);
            }
        }

        private static void AddNonIncluded(ArrayBuilder<string> builder, ImmutableArray<string> items)
        {
            int count = builder.Count;
            ImmutableArray<string>.Enumerator enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string current = enumerator.Current;
                if (builder.IndexOf(current, 0, count) < 0)
                {
                    builder.Add(current);
                }
            }
        }
    }
}
