using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class ExplicitInterfaceHelpers
    {
        public static string GetMemberName(Binder binder, ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifierOpt, string name)
        {
            return GetMemberNameAndInterfaceSymbol(binder, explicitInterfaceSpecifierOpt, name, BindingDiagnosticBag.Discarded, out TypeSymbol explicitInterfaceTypeOpt, out string aliasQualifierOpt);
        }

        public static string GetMemberNameAndInterfaceSymbol(Binder binder, ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifierOpt, string name, BindingDiagnosticBag diagnostics, out TypeSymbol explicitInterfaceTypeOpt, out string aliasQualifierOpt)
        {
            if (explicitInterfaceSpecifierOpt == null)
            {
                explicitInterfaceTypeOpt = null;
                aliasQualifierOpt = null;
                return name;
            }
            binder = binder.WithAdditionalFlags(BinderFlags.SuppressConstraintChecks | BinderFlags.SuppressObsoleteChecks);
            NameSyntax name2 = explicitInterfaceSpecifierOpt.Name;
            explicitInterfaceTypeOpt = binder.BindType(name2, diagnostics).Type;
            aliasQualifierOpt = name2.GetAliasQualifierOpt();
            return GetMemberName(name, explicitInterfaceTypeOpt, aliasQualifierOpt);
        }

        public static string GetMemberName(string name, TypeSymbol explicitInterfaceTypeOpt, string aliasQualifierOpt)
        {
            if ((object)explicitInterfaceTypeOpt == null)
            {
                return name;
            }
            string text = explicitInterfaceTypeOpt.ToDisplayString(SymbolDisplayFormat.ExplicitInterfaceImplementationFormat);
            PooledStringBuilder instance = PooledStringBuilder.GetInstance();
            StringBuilder builder = instance.Builder;
            if (!string.IsNullOrEmpty(aliasQualifierOpt))
            {
                builder.Append(aliasQualifierOpt);
                builder.Append("::");
            }
            string text2 = text;
            foreach (char c in text2)
            {
                if (c != ' ')
                {
                    builder.Append(c);
                }
            }
            builder.Append(".");
            builder.Append(name);
            return instance.ToStringAndFree();
        }

        public static string GetMethodNameWithoutInterfaceName(this MethodSymbol method)
        {
            if (method.MethodKind != MethodKind.ExplicitInterfaceImplementation)
            {
                return method.Name;
            }
            return GetMemberNameWithoutInterfaceName(method.Name);
        }

        public static string GetMemberNameWithoutInterfaceName(string fullName)
        {
            int num = fullName.LastIndexOf('.');
            if (num <= 0)
            {
                return fullName;
            }
            return fullName.Substring(num + 1);
        }

        public static ImmutableArray<T> SubstituteExplicitInterfaceImplementations<T>(ImmutableArray<T> unsubstitutedExplicitInterfaceImplementations, TypeMap map) where T : Symbol
        {
            ArrayBuilder<T> instance = ArrayBuilder<T>.GetInstance();
            ImmutableArray<T>.Enumerator enumerator = unsubstitutedExplicitInterfaceImplementations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                NamedTypeSymbol containingType = current.ContainingType;
                NamedTypeSymbol namedTypeSymbol = map.SubstituteNamedType(containingType);
                string name = current.Name;
                T item = null;
                ImmutableArray<Symbol>.Enumerator enumerator2 = namedTypeSymbol.GetMembers(name).GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current2 = enumerator2.Current;
                    if (current2.OriginalDefinition == current.OriginalDefinition)
                    {
                        item = (T)current2;
                        break;
                    }
                }
                instance.Add(item);
            }
            return instance.ToImmutableAndFree();
        }

        internal static MethodSymbol FindExplicitlyImplementedMethod(this MethodSymbol implementingMethod, TypeSymbol explicitInterfaceType, string interfaceMethodName, ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifierSyntax, BindingDiagnosticBag diagnostics)
        {
            return (MethodSymbol)FindExplicitlyImplementedMember(implementingMethod, explicitInterfaceType, interfaceMethodName, explicitInterfaceSpecifierSyntax, diagnostics);
        }

        internal static PropertySymbol FindExplicitlyImplementedProperty(this PropertySymbol implementingProperty, TypeSymbol explicitInterfaceType, string interfacePropertyName, ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifierSyntax, BindingDiagnosticBag diagnostics)
        {
            return (PropertySymbol)FindExplicitlyImplementedMember(implementingProperty, explicitInterfaceType, interfacePropertyName, explicitInterfaceSpecifierSyntax, diagnostics);
        }

        internal static EventSymbol FindExplicitlyImplementedEvent(this EventSymbol implementingEvent, TypeSymbol explicitInterfaceType, string interfaceEventName, ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifierSyntax, BindingDiagnosticBag diagnostics)
        {
            return (EventSymbol)FindExplicitlyImplementedMember(implementingEvent, explicitInterfaceType, interfaceEventName, explicitInterfaceSpecifierSyntax, diagnostics);
        }

        private static Symbol FindExplicitlyImplementedMember(Symbol implementingMember, TypeSymbol explicitInterfaceType, string interfaceMemberName, ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifierSyntax, BindingDiagnosticBag diagnostics)
        {
            if ((object)explicitInterfaceType == null)
            {
                return null;
            }
            Location memberLocation = implementingMember.Locations[0];
            NamedTypeSymbol containingType = implementingMember.ContainingType;
            TypeKind typeKind = containingType.TypeKind;
            if (typeKind != TypeKind.Class && typeKind != TypeKind.Interface && typeKind != TypeKind.Struct)
            {
                diagnostics.Add(ErrorCode.ERR_ExplicitInterfaceImplementationInNonClassOrStruct, memberLocation, implementingMember);
                return null;
            }
            if (!explicitInterfaceType.IsInterfaceType())
            {
                SourceLocation location = new SourceLocation(explicitInterfaceSpecifierSyntax.Name);
                diagnostics.Add(ErrorCode.ERR_ExplicitInterfaceImplementationNotInterface, location, explicitInterfaceType);
                return null;
            }
            NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)explicitInterfaceType;
            MultiDictionary<NamedTypeSymbol, NamedTypeSymbol>.ValueSet valueSet = containingType.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics[namedTypeSymbol];
            int count = valueSet.Count;
            if (count == 0 || !valueSet.Contains(namedTypeSymbol, SymbolEqualityComparer.ObliviousNullableModifierMatchesAny))
            {
                SourceLocation location2 = new SourceLocation(explicitInterfaceSpecifierSyntax.Name);
                if (count > 0 && valueSet.Contains(namedTypeSymbol, SymbolEqualityComparer.IgnoringNullable))
                {
                    diagnostics.Add(ErrorCode.WRN_NullabilityMismatchInExplicitlyImplementedInterface, location2);
                }
                else
                {
                    diagnostics.Add(ErrorCode.ERR_ClassDoesntImplementInterface, location2, implementingMember, namedTypeSymbol);
                }
            }
            if ((object)containingType == namedTypeSymbol.OriginalDefinition)
            {
                return null;
            }
            bool flag = implementingMember.HasParamsParameter();
            bool flag2 = false;
            Symbol symbol = null;
            ImmutableArray<Symbol>.Enumerator enumerator = namedTypeSymbol.GetMembers(interfaceMemberName).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind != implementingMember.Kind || !current.IsImplementableInterfaceMember() || !MemberSignatureComparer.ExplicitImplementationComparer.Equals(implementingMember, current))
                {
                    continue;
                }
                flag2 = true;
                if (current.IsAccessor() && !((MethodSymbol)current).IsIndexedPropertyAccessor())
                {
                    diagnostics.Add(ErrorCode.ERR_ExplicitMethodImplAccessor, memberLocation, implementingMember, current);
                    continue;
                }
                if (current.MustCallMethodsDirectly())
                {
                    diagnostics.Add(ErrorCode.ERR_BogusExplicitImpl, memberLocation, implementingMember, current);
                }
                else if (flag && !current.HasParamsParameter())
                {
                    diagnostics.Add(ErrorCode.ERR_ExplicitImplParams, memberLocation, implementingMember, current);
                }
                symbol = current;
                break;
            }
            if (!flag2)
            {
                diagnostics.Add(ErrorCode.ERR_InterfaceMemberNotFound, memberLocation, implementingMember);
            }
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo;
            if ((object)symbol != null)
            {
                useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, implementingMember.ContainingAssembly);
                if (!AccessCheck.IsSymbolAccessible(symbol, implementingMember.ContainingType, ref useSiteInfo))
                {
                    diagnostics.Add(ErrorCode.ERR_BadAccess, memberLocation, symbol);
                }
                else
                {
                    switch (symbol.Kind)
                    {
                        case SymbolKind.Property:
                            {
                                PropertySymbol obj2 = (PropertySymbol)symbol;
                                checkAccessorIsAccessibleIfImplementable(obj2.GetMethod);
                                checkAccessorIsAccessibleIfImplementable(obj2.SetMethod);
                                break;
                            }
                        case SymbolKind.Event:
                            {
                                EventSymbol obj = (EventSymbol)symbol;
                                checkAccessorIsAccessibleIfImplementable(obj.AddMethod);
                                checkAccessorIsAccessibleIfImplementable(obj.RemoveMethod);
                                break;
                            }
                    }
                }
                diagnostics.Add(memberLocation, useSiteInfo);
            }
            return symbol;
            void checkAccessorIsAccessibleIfImplementable(MethodSymbol accessor)
            {
                if (accessor.IsImplementable() && !AccessCheck.IsSymbolAccessible(accessor, implementingMember.ContainingType, ref useSiteInfo))
                {
                    diagnostics.Add(ErrorCode.ERR_BadAccess, memberLocation, accessor);
                }
            }
        }

        internal static void FindExplicitlyImplementedMemberVerification(this Symbol implementingMember, Symbol implementedMember, BindingDiagnosticBag diagnostics)
        {
            if ((object)implementedMember != null)
            {
                if (implementingMember.ContainsTupleNames() && MemberSignatureComparer.ConsideringTupleNamesCreatesDifference(implementingMember, implementedMember))
                {
                    Location location = implementingMember.Locations[0];
                    diagnostics.Add(ErrorCode.ERR_ImplBadTupleNames, location, implementingMember, implementedMember);
                }
                FindExplicitImplementationCollisions(implementingMember, implementedMember, diagnostics);
            }
        }

        private static void FindExplicitImplementationCollisions(Symbol implementingMember, Symbol implementedMember, BindingDiagnosticBag diagnostics)
        {
            if ((object)implementedMember == null)
            {
                return;
            }
            NamedTypeSymbol containingType = implementedMember.ContainingType;
            bool isDefinition = containingType.IsDefinition;
            ImmutableArray<Symbol>.Enumerator enumerator = containingType.GetMembers(implementedMember.Name).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind != implementingMember.Kind || !(implementedMember != current))
                {
                    continue;
                }
                if (!isDefinition && MemberSignatureComparer.RuntimeSignatureComparer.Equals(implementedMember, current))
                {
                    bool flag = false;
                    ImmutableArray<ParameterSymbol> parameters = implementedMember.GetParameters();
                    ImmutableArray<ParameterSymbol> parameters2 = current.GetParameters();
                    int length = parameters.Length;
                    for (int i = 0; i < length; i++)
                    {
                        if (parameters[i].RefKind != parameters2[i].RefKind)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        diagnostics.Add(ErrorCode.ERR_ExplicitImplCollisionOnRefOut, containingType.Locations[0], containingType, implementedMember);
                    }
                    else
                    {
                        diagnostics.Add(ErrorCode.WRN_ExplicitImplCollision, implementingMember.Locations[0], implementingMember);
                    }
                    break;
                }
                if (MemberSignatureComparer.ExplicitImplementationComparer.Equals(implementedMember, current))
                {
                    diagnostics.Add(ErrorCode.WRN_ExplicitImplCollision, implementingMember.Locations[0], implementingMember);
                }
            }
        }
    }
}
