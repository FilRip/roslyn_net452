using System.Collections.Immutable;

using Microsoft.CodeAnalysis.Collections;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public static class ISymbolExtensions
    {
        public static IMethodSymbol? GetConstructedReducedFrom(this IMethodSymbol method)
        {
            if (method.MethodKind != MethodKind.ReducedExtension)
            {
                return null;
            }
            IMethodSymbol reducedFrom = method.ReducedFrom;
            if (!reducedFrom.IsGenericMethod)
            {
                return reducedFrom;
            }
            ITypeSymbol[] array = new ITypeSymbol[reducedFrom.TypeParameters.Length];
            int i = 0;
            for (int length = method.TypeParameters.Length; i < length; i++)
            {
                ITypeSymbol typeSymbol = method.TypeArguments[i];
                ITypeParameterSymbol typeParameterSymbol = method.TypeParameters[i];
                if (typeSymbol.Equals(typeParameterSymbol))
                {
                    typeSymbol = typeParameterSymbol.ReducedFrom;
                }
                array[typeParameterSymbol.ReducedFrom!.Ordinal] = typeSymbol;
            }
            int j = 0;
            for (int length2 = reducedFrom.TypeParameters.Length; j < length2; j++)
            {
                ITypeSymbol typeInferredDuringReduction = method.GetTypeInferredDuringReduction(reducedFrom.TypeParameters[j]);
                if (typeInferredDuringReduction != null)
                {
                    array[j] = typeInferredDuringReduction;
                }
            }
            return reducedFrom.Construct(array);
        }

        public static bool IsDefaultTupleElement(this IFieldSymbol field)
        {
            return field == field.CorrespondingTupleField;
        }

        public static bool IsTupleElement(this IFieldSymbol field)
        {
            return field.CorrespondingTupleField != null;
        }

        public static string? ProvidedTupleElementNameOrNull(this IFieldSymbol field)
        {
            if (!field.IsTupleElement() || field.IsImplicitlyDeclared)
            {
                return null;
            }
            return field.Name;
        }

        public static INamespaceSymbol? GetNestedNamespace(this INamespaceSymbol container, string name)
        {
            foreach (INamespaceOrTypeSymbol member in container.GetMembers(name))
            {
                if (member.Kind == SymbolKind.Namespace)
                {
                    return (INamespaceSymbol)member;
                }
            }
            return null;
        }

        public static bool IsNetModule(this IAssemblySymbol assembly)
        {
            if (assembly is ISourceAssemblySymbol sourceAssemblySymbol)
            {
                return sourceAssemblySymbol.Compilation.Options.OutputKind.IsNetModule();
            }
            return false;
        }

        public static bool IsInSource(this ISymbol symbol)
        {
            ImmutableArray<Location>.Enumerator enumerator = symbol.Locations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.IsInSource)
                {
                    return true;
                }
            }
            return false;
        }

        public static IVTConclusion PerformIVTCheck(this AssemblyIdentity assemblyGrantingAccessIdentity, ImmutableArray<byte> assemblyWantingAccessKey, ImmutableArray<byte> grantedToPublicKey)
        {
            bool isStrongName = assemblyGrantingAccessIdentity.IsStrongName;
            bool num = !grantedToPublicKey.IsDefaultOrEmpty;
            bool flag = !assemblyWantingAccessKey.IsDefaultOrEmpty;
            bool flag2 = num && flag && ByteSequenceComparer.Equals(grantedToPublicKey, assemblyWantingAccessKey);
            if (num && !flag2)
            {
                return IVTConclusion.PublicKeyDoesntMatch;
            }
            if (!isStrongName && flag)
            {
                return IVTConclusion.OneSignedOneNot;
            }
            return IVTConclusion.Match;
        }
    }
}
