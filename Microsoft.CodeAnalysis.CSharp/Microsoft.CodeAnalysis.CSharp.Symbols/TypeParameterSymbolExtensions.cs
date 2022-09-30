using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class TypeParameterSymbolExtensions
    {
        public static bool DependsOn(this TypeParameterSymbol typeParameter1, TypeParameterSymbol typeParameter2)
        {
            Stack<TypeParameterSymbol> stack = null;
            HashSet<TypeParameterSymbol> hashSet = null;
            while (true)
            {
                ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = typeParameter1.ConstraintTypesNoUseSiteDiagnostics.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (!(enumerator.Current.Type is TypeParameterSymbol typeParameterSymbol))
                    {
                        continue;
                    }
                    if (typeParameterSymbol.Equals(typeParameter2))
                    {
                        return true;
                    }
                    if (hashSet == null)
                    {
                        hashSet = new HashSet<TypeParameterSymbol>();
                    }
                    if (hashSet.Add(typeParameterSymbol))
                    {
                        if (stack == null)
                        {
                            stack = new Stack<TypeParameterSymbol>();
                        }
                        stack.Push(typeParameterSymbol);
                    }
                }
                if (stack == null || stack.Count == 0)
                {
                    break;
                }
                typeParameter1 = stack.Pop();
            }
            return false;
        }
    }
}
