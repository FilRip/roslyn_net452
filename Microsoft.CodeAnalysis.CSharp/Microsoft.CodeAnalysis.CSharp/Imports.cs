using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    internal sealed class Imports
    {
        private class UsingTargetComparer : IEqualityComparer<NamespaceOrTypeAndUsingDirective>
        {
            public static readonly IEqualityComparer<NamespaceOrTypeAndUsingDirective> Instance = new UsingTargetComparer();

            private UsingTargetComparer()
            {
            }

            bool IEqualityComparer<NamespaceOrTypeAndUsingDirective>.Equals(NamespaceOrTypeAndUsingDirective x, NamespaceOrTypeAndUsingDirective y)
            {
                return x.NamespaceOrType.Equals(y.NamespaceOrType);
            }

            int IEqualityComparer<NamespaceOrTypeAndUsingDirective>.GetHashCode(NamespaceOrTypeAndUsingDirective obj)
            {
                return obj.NamespaceOrType.GetHashCode();
            }
        }

        internal static readonly Imports Empty = new Imports(ImmutableDictionary<string, AliasAndUsingDirective>.Empty, ImmutableArray<NamespaceOrTypeAndUsingDirective>.Empty, ImmutableArray<AliasAndExternAliasDirective>.Empty);

        public readonly ImmutableDictionary<string, AliasAndUsingDirective> UsingAliases;

        public readonly ImmutableArray<NamespaceOrTypeAndUsingDirective> Usings;

        public readonly ImmutableArray<AliasAndExternAliasDirective> ExternAliases;

        private Imports(ImmutableDictionary<string, AliasAndUsingDirective> usingAliases, ImmutableArray<NamespaceOrTypeAndUsingDirective> usings, ImmutableArray<AliasAndExternAliasDirective> externs)
        {
            UsingAliases = usingAliases;
            Usings = usings;
            ExternAliases = externs;
        }

        internal string GetDebuggerDisplay()
        {
            return string.Join("; ", (from x in UsingAliases
                                      orderby x.Value.UsingDirective!.Location.SourceSpan.Start
                                      select x into ua
                                      select $"{ua.Key} = {ua.Value.Alias.Target}").Concat(Usings.Select((NamespaceOrTypeAndUsingDirective u) => u.NamespaceOrType.ToString())).Concat(ExternAliases.Select((AliasAndExternAliasDirective ea) => "extern alias " + ea.Alias.Name)));
        }

        internal static Imports ExpandPreviousSubmissionImports(Imports previousSubmissionImports, CSharpCompilation newSubmission)
        {
            if (previousSubmissionImports == Empty)
            {
                return Empty;
            }
            ImmutableDictionary<string, AliasAndUsingDirective> usingAliases = ImmutableDictionary<string, AliasAndUsingDirective>.Empty;
            if (!previousSubmissionImports.UsingAliases.IsEmpty)
            {
                ImmutableDictionary<string, AliasAndUsingDirective>.Builder builder = ImmutableDictionary.CreateBuilder<string, AliasAndUsingDirective>();
                foreach (KeyValuePair<string, AliasAndUsingDirective> usingAlias in previousSubmissionImports.UsingAliases)
                {
                    string key = usingAlias.Key;
                    AliasAndUsingDirective value = usingAlias.Value;
                    builder.Add(key, new AliasAndUsingDirective(value.Alias.ToNewSubmission(newSubmission), value.UsingDirective));
                }
                usingAliases = builder.ToImmutable();
            }
            ImmutableArray<NamespaceOrTypeAndUsingDirective> usings = ExpandPreviousSubmissionImports(previousSubmissionImports.Usings, newSubmission);
            return Create(usingAliases, usings, previousSubmissionImports.ExternAliases);
        }

        internal static ImmutableArray<NamespaceOrTypeAndUsingDirective> ExpandPreviousSubmissionImports(ImmutableArray<NamespaceOrTypeAndUsingDirective> previousSubmissionUsings, CSharpCompilation newSubmission)
        {
            if (!previousSubmissionUsings.IsEmpty)
            {
                ArrayBuilder<NamespaceOrTypeAndUsingDirective> instance = ArrayBuilder<NamespaceOrTypeAndUsingDirective>.GetInstance(previousSubmissionUsings.Length);
                NamespaceSymbol globalNamespace = newSubmission.GlobalNamespace;
                ImmutableArray<NamespaceOrTypeAndUsingDirective>.Enumerator enumerator = previousSubmissionUsings.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    NamespaceOrTypeAndUsingDirective current = enumerator.Current;
                    NamespaceOrTypeSymbol namespaceOrType = current.NamespaceOrType;
                    if (namespaceOrType.IsType)
                    {
                        instance.Add(current);
                        continue;
                    }
                    NamespaceSymbol namespaceOrType2 = ExpandPreviousSubmissionNamespace((NamespaceSymbol)namespaceOrType, globalNamespace);
                    instance.Add(new NamespaceOrTypeAndUsingDirective(namespaceOrType2, current.UsingDirective, default(ImmutableArray<AssemblySymbol>)));
                }
                return instance.ToImmutableAndFree();
            }
            return previousSubmissionUsings;
        }

        internal static NamespaceSymbol ExpandPreviousSubmissionNamespace(NamespaceSymbol originalNamespace, NamespaceSymbol expandedGlobalNamespace)
        {
            ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance();
            NamespaceSymbol namespaceSymbol = originalNamespace;
            while (!namespaceSymbol.IsGlobalNamespace)
            {
                instance.Add(namespaceSymbol.Name);
                namespaceSymbol = namespaceSymbol.ContainingNamespace;
            }
            NamespaceSymbol namespaceSymbol2 = expandedGlobalNamespace;
            for (int num = instance.Count - 1; num >= 0; num--)
            {
                namespaceSymbol2 = namespaceSymbol2.GetMembers(instance[num]).OfType<NamespaceSymbol>().Single();
            }
            instance.Free();
            return namespaceSymbol2;
        }

        public static Imports Create(ImmutableDictionary<string, AliasAndUsingDirective> usingAliases, ImmutableArray<NamespaceOrTypeAndUsingDirective> usings, ImmutableArray<AliasAndExternAliasDirective> externs)
        {
            if (usingAliases.IsEmpty && usings.IsEmpty && externs.IsEmpty)
            {
                return Empty;
            }
            return new Imports(usingAliases, usings, externs);
        }

        internal Imports Concat(Imports otherImports)
        {
            if (this == Empty)
            {
                return otherImports;
            }
            if (otherImports == Empty)
            {
                return this;
            }
            ImmutableDictionary<string, AliasAndUsingDirective> usingAliases = UsingAliases.SetItems(otherImports.UsingAliases);
            ImmutableArray<NamespaceOrTypeAndUsingDirective> usings = Usings.AddRange(otherImports.Usings).Distinct(UsingTargetComparer.Instance);
            ImmutableArray<AliasAndExternAliasDirective> externs = ConcatExternAliases(ExternAliases, otherImports.ExternAliases);
            return Create(usingAliases, usings, externs);
        }

        private static ImmutableArray<AliasAndExternAliasDirective> ConcatExternAliases(ImmutableArray<AliasAndExternAliasDirective> externs1, ImmutableArray<AliasAndExternAliasDirective> externs2)
        {
            if (externs1.Length == 0)
            {
                return externs2;
            }
            if (externs2.Length == 0)
            {
                return externs1;
            }
            PooledHashSet<string> instance = PooledHashSet<string>.GetInstance();
            instance.AddAll(externs2.Select((AliasAndExternAliasDirective e) => e.Alias.Name));
            return externs1.WhereAsArray((AliasAndExternAliasDirective e, PooledHashSet<string> replacedExternAliases) => !replacedExternAliases.Contains(e.Alias.Name), instance).AddRange(externs2);
        }
    }
}
