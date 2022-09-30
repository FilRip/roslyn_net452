using System;

using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.Cci
{
    public struct UsedNamespaceOrType : IEquatable<UsedNamespaceOrType>
    {
        public readonly string? AliasOpt;

        public readonly IAssemblyReference? TargetAssemblyOpt;

        public readonly INamespace? TargetNamespaceOpt;

        public readonly ITypeReference? TargetTypeOpt;

        public readonly string? TargetXmlNamespaceOpt;

        private UsedNamespaceOrType(string? alias = null, IAssemblyReference? targetAssembly = null, INamespace? targetNamespace = null, ITypeReference? targetType = null, string? targetXmlNamespace = null)
        {
            AliasOpt = alias;
            TargetAssemblyOpt = targetAssembly;
            TargetNamespaceOpt = targetNamespace;
            TargetTypeOpt = targetType;
            TargetXmlNamespaceOpt = targetXmlNamespace;
        }

        public static UsedNamespaceOrType CreateType(ITypeReference type, string? aliasOpt = null)
        {
            return new UsedNamespaceOrType(aliasOpt, null, null, type);
        }

        public static UsedNamespaceOrType CreateNamespace(INamespace @namespace, IAssemblyReference? assemblyOpt = null, string? aliasOpt = null)
        {
            return new UsedNamespaceOrType(aliasOpt, assemblyOpt, @namespace);
        }

        public static UsedNamespaceOrType CreateExternAlias(string alias)
        {
            return new UsedNamespaceOrType(alias);
        }

        public static UsedNamespaceOrType CreateXmlNamespace(string prefix, string xmlNamespace)
        {
            return new UsedNamespaceOrType(prefix, null, null, null, xmlNamespace);
        }

        public override bool Equals(object? obj)
        {
            if (obj is UsedNamespaceOrType other)
            {
                return Equals(other);
            }
            return false;
        }

        public bool Equals(UsedNamespaceOrType other)
        {
            if (AliasOpt == other.AliasOpt && object.Equals(TargetAssemblyOpt, other.TargetAssemblyOpt) && Equals(TargetNamespaceOpt, other.TargetNamespaceOpt) && Equals(TargetTypeOpt, other.TargetTypeOpt))
            {
                return TargetXmlNamespaceOpt == other.TargetXmlNamespaceOpt;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(AliasOpt, Hash.Combine((object)TargetAssemblyOpt, Hash.Combine(GetHashCode(TargetNamespaceOpt), Hash.Combine(GetHashCode(TargetTypeOpt), Hash.Combine(TargetXmlNamespaceOpt, 0)))));
        }

        private static bool Equals(ITypeReference? x, ITypeReference? y)
        {
            if (x == y)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            ISymbolInternal? internalSymbol = x!.GetInternalSymbol();
            ISymbolInternal? internalSymbol2 = y!.GetInternalSymbol();
            if (internalSymbol != null && internalSymbol2 != null)
            {
                return internalSymbol.Equals(internalSymbol2);
            }
            if (internalSymbol != null || internalSymbol2 != null)
            {
                return false;
            }
            return x!.Equals(y);
        }

        private static int GetHashCode(ITypeReference? obj)
        {
            return (obj?.GetInternalSymbol())?.GetHashCode() ?? obj?.GetHashCode() ?? 0;
        }

        private static bool Equals(INamespace? x, INamespace? y)
        {
            if (x == y)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            INamespaceSymbolInternal internalSymbol = x!.GetInternalSymbol();
            INamespaceSymbolInternal internalSymbol2 = y!.GetInternalSymbol();
            if (internalSymbol != null && internalSymbol2 != null)
            {
                return internalSymbol.Equals(internalSymbol2);
            }
            if (internalSymbol != null || internalSymbol2 != null)
            {
                return false;
            }
            return x!.Equals(y);
        }

        private static int GetHashCode(INamespace? obj)
        {
            return (obj?.GetInternalSymbol())?.GetHashCode() ?? obj?.GetHashCode() ?? 0;
        }
    }
}
