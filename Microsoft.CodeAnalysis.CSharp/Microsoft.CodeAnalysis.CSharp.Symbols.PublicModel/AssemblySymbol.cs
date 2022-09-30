using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal abstract class AssemblySymbol : Symbol, IAssemblySymbol, ISymbol, IEquatable<ISymbol?>
    {
        internal abstract Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol UnderlyingAssemblySymbol { get; }

        INamespaceSymbol IAssemblySymbol.GlobalNamespace => UnderlyingAssemblySymbol.GlobalNamespace.GetPublicSymbol();

        IEnumerable<IModuleSymbol> IAssemblySymbol.Modules
        {
            get
            {
                ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.ModuleSymbol>.Enumerator enumerator = UnderlyingAssemblySymbol.Modules.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Microsoft.CodeAnalysis.CSharp.Symbols.ModuleSymbol current = enumerator.Current;
                    yield return current.GetPublicSymbol();
                }
            }
        }

        bool IAssemblySymbol.IsInteractive => UnderlyingAssemblySymbol.IsInteractive;

        AssemblyIdentity IAssemblySymbol.Identity => UnderlyingAssemblySymbol.Identity;

        ICollection<string> IAssemblySymbol.TypeNames => UnderlyingAssemblySymbol.TypeNames;

        ICollection<string> IAssemblySymbol.NamespaceNames => UnderlyingAssemblySymbol.NamespaceNames;

        bool IAssemblySymbol.MightContainExtensionMethods => UnderlyingAssemblySymbol.MightContainExtensionMethods;

        AssemblyMetadata IAssemblySymbol.GetMetadata()
        {
            return UnderlyingAssemblySymbol.GetMetadata();
        }

        INamedTypeSymbol IAssemblySymbol.ResolveForwardedType(string fullyQualifiedMetadataName)
        {
            return UnderlyingAssemblySymbol.ResolveForwardedType(fullyQualifiedMetadataName).GetPublicSymbol();
        }

        ImmutableArray<INamedTypeSymbol> IAssemblySymbol.GetForwardedTypes()
        {
            return (from t in UnderlyingAssemblySymbol.GetAllTopLevelForwardedTypes()
                    select t.GetPublicSymbol() into t
                    orderby t.ToDisplayString(SymbolDisplayFormat.QualifiedNameArityFormat)
                    select t).AsImmutable();
        }

        bool IAssemblySymbol.GivesAccessTo(IAssemblySymbol assemblyWantingAccess)
        {
            if (object.Equals(this, assemblyWantingAccess))
            {
                return true;
            }
            IEnumerable<ImmutableArray<byte>> internalsVisibleToPublicKeys = UnderlyingAssemblySymbol.GetInternalsVisibleToPublicKeys(assemblyWantingAccess.Name);
            if (internalsVisibleToPublicKeys.Any())
            {
                if (assemblyWantingAccess.IsNetModule())
                {
                    return true;
                }
                AssemblyIdentity identity = UnderlyingAssemblySymbol.Identity;
                foreach (ImmutableArray<byte> item in internalsVisibleToPublicKeys)
                {
                    IVTConclusion iVTConclusion = identity.PerformIVTCheck(assemblyWantingAccess.Identity.PublicKey, item);
                    if (iVTConclusion == IVTConclusion.Match || iVTConclusion == IVTConclusion.OneSignedOneNot)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        INamedTypeSymbol IAssemblySymbol.GetTypeByMetadataName(string metadataName)
        {
            return UnderlyingAssemblySymbol.GetTypeByMetadataName(metadataName).GetPublicSymbol();
        }

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitAssembly(this);
        }

        protected override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitAssembly(this);
        }
    }
}
