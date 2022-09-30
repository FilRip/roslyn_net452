using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class BaseTypeAnalysis
    {
        internal static bool TypeDependsOn(NamedTypeSymbol depends, NamedTypeSymbol on)
        {
            PooledHashSet<Symbol> instance = PooledHashSet<Symbol>.GetInstance();
            TypeDependsClosure(depends, depends.DeclaringCompilation, instance);
            bool result = instance.Contains(on);
            instance.Free();
            return result;
        }

        private static void TypeDependsClosure(NamedTypeSymbol type, CSharpCompilation currentCompilation, HashSet<Symbol> partialClosure)
        {
            if ((object)type == null)
            {
                return;
            }
            type = type.OriginalDefinition;
            if (!partialClosure.Add(type))
            {
                return;
            }
            if (type.IsInterface)
            {
                ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = type.GetDeclaredInterfaces(null).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeDependsClosure(enumerator.Current, currentCompilation, partialClosure);
                }
            }
            else
            {
                TypeDependsClosure(type.GetDeclaredBaseType(null), currentCompilation, partialClosure);
            }
            if (currentCompilation != null && type.IsFromCompilation(currentCompilation))
            {
                TypeDependsClosure(type.ContainingType, currentCompilation, partialClosure);
            }
        }

        internal static bool StructDependsOn(NamedTypeSymbol depends, NamedTypeSymbol on)
        {
            PooledHashSet<Symbol> instance = PooledHashSet<Symbol>.GetInstance();
            StructDependsClosure(depends, instance, on);
            bool result = instance.Contains(on);
            instance.Free();
            return result;
        }

        private static void StructDependsClosure(NamedTypeSymbol type, HashSet<Symbol> partialClosure, NamedTypeSymbol on)
        {
            if ((object)type.OriginalDefinition == on)
            {
                partialClosure.Add(on);
            }
            else
            {
                if (!partialClosure.Add(type))
                {
                    return;
                }
                ImmutableArray<Symbol>.Enumerator enumerator = type.GetMembersUnordered().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    FieldSymbol fieldSymbol = enumerator.Current as FieldSymbol;
                    TypeSymbol typeSymbol = fieldSymbol?.NonPointerType();
                    if ((object)typeSymbol != null && typeSymbol.TypeKind == TypeKind.Struct && !fieldSymbol.IsStatic)
                    {
                        StructDependsClosure((NamedTypeSymbol)typeSymbol, partialClosure, on);
                    }
                }
            }
        }

        internal static ManagedKind GetManagedKind(NamedTypeSymbol type, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            (ThreeState isManaged, bool hasGenerics) tuple = IsManagedTypeHelper(type);
            ThreeState item = tuple.isManaged;
            bool flag = tuple.hasGenerics;
            bool flag2 = item == ThreeState.True;
            if (item == ThreeState.Unknown)
            {
                PooledHashSet<Symbol> instance = PooledHashSet<Symbol>.GetInstance();
                (bool, bool) tuple2 = DependsOnDefinitelyManagedType(type, instance, ref useSiteInfo);
                flag2 = tuple2.Item1;
                flag = flag || tuple2.Item2;
                instance.Free();
            }
            if (flag2)
            {
                return ManagedKind.Managed;
            }
            if (flag)
            {
                return ManagedKind.UnmanagedWithGenerics;
            }
            return ManagedKind.Unmanaged;
        }

        internal static TypeSymbol NonPointerType(this FieldSymbol field)
        {
            if (!field.HasPointerType)
            {
                return field.Type;
            }
            return null;
        }

        private static (bool definitelyManaged, bool hasGenerics) DependsOnDefinitelyManagedType(NamedTypeSymbol type, HashSet<Symbol> partialClosure, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            bool flag = false;
            if (partialClosure.Add(type))
            {
                foreach (Symbol instanceFieldsAndEvent in type.GetInstanceFieldsAndEvents())
                {
                    FieldSymbol fieldSymbol = instanceFieldsAndEvent.Kind switch
                    {
                        SymbolKind.Field => (FieldSymbol)instanceFieldsAndEvent,
                        SymbolKind.Event => ((EventSymbol)instanceFieldsAndEvent).AssociatedField,
                        _ => throw ExceptionUtilities.UnexpectedValue(instanceFieldsAndEvent.Kind),
                    };
                    if ((object)fieldSymbol == null)
                    {
                        continue;
                    }
                    TypeSymbol typeSymbol = fieldSymbol.NonPointerType();
                    if ((object)typeSymbol == null)
                    {
                        continue;
                    }
                    typeSymbol.AddUseSiteInfo(ref useSiteInfo);
                    if (!(typeSymbol is NamedTypeSymbol namedTypeSymbol))
                    {
                        if (typeSymbol.IsManagedType(ref useSiteInfo))
                        {
                            return (true, flag);
                        }
                        continue;
                    }
                    (ThreeState, bool) tuple = IsManagedTypeHelper(namedTypeSymbol);
                    flag = flag || tuple.Item2;
                    switch (tuple.Item1)
                    {
                        case ThreeState.True:
                            return (true, flag);
                        case ThreeState.Unknown:
                            if (!namedTypeSymbol.OriginalDefinition.KnownCircularStruct)
                            {
                                (bool definitelyManaged, bool hasGenerics) tuple2 = DependsOnDefinitelyManagedType(namedTypeSymbol, partialClosure, ref useSiteInfo);
                                bool item = tuple2.definitelyManaged;
                                bool item2 = tuple2.hasGenerics;
                                flag = flag || item2;
                                if (item)
                                {
                                    return (true, flag);
                                }
                            }
                            break;
                    }
                }
            }
            return (false, flag);
        }

        private static (ThreeState isManaged, bool hasGenerics) IsManagedTypeHelper(NamedTypeSymbol type)
        {
            if (type.IsEnumType())
            {
                type = type.GetEnumUnderlyingType();
            }
            switch (type.SpecialType)
            {
                case SpecialType.System_Void:
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_IntPtr:
                case SpecialType.System_UIntPtr:
                case SpecialType.System_TypedReference:
                case SpecialType.System_ArgIterator:
                case SpecialType.System_RuntimeArgumentHandle:
                    return (ThreeState.False, false);
                default:
                    {
                        bool isGenericType = type.IsGenericType;
                        return type.TypeKind switch
                        {
                            TypeKind.Enum => (ThreeState.False, isGenericType),
                            TypeKind.Struct => (ThreeState.Unknown, isGenericType),
                            _ => (ThreeState.True, isGenericType),
                        };
                    }
            }
        }
    }
}
