using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class VarianceSafety
    {
        private delegate Location LocationProvider<T>(T arg);

        internal static void CheckInterfaceVarianceSafety(this NamedTypeSymbol interfaceType, BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = interfaceType.InterfacesNoUseSiteDiagnostics().GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                IsVarianceUnsafe(current, requireOutputSafety: true, requireInputSafety: false, current, (NamedTypeSymbol i) => null, current, diagnostics);
            }
            ImmutableArray<Symbol>.Enumerator enumerator2 = interfaceType.GetMembersUnordered().GetEnumerator();
            while (enumerator2.MoveNext())
            {
                Symbol current2 = enumerator2.Current;
                switch (current2.Kind)
                {
                    case SymbolKind.Method:
                        if (!current2.IsAccessor())
                        {
                            ((MethodSymbol)current2).CheckMethodVarianceSafety(diagnostics);
                        }
                        break;
                    case SymbolKind.Property:
                        CheckPropertyVarianceSafety((PropertySymbol)current2, diagnostics);
                        break;
                    case SymbolKind.Event:
                        CheckEventVarianceSafety((EventSymbol)current2, diagnostics);
                        break;
                    case SymbolKind.NamedType:
                        CheckNestedTypeVarianceSafety((NamedTypeSymbol)current2, diagnostics);
                        break;
                }
            }
        }

        private static void CheckNestedTypeVarianceSafety(NamedTypeSymbol member, BindingDiagnosticBag diagnostics)
        {
            switch (member.TypeKind)
            {
                case TypeKind.Delegate:
                case TypeKind.Interface:
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(member.TypeKind);
                case TypeKind.Class:
                case TypeKind.Enum:
                case TypeKind.Struct:
                    if ((object)GetEnclosingVariantInterface(member) != null)
                    {
                        diagnostics.Add(ErrorCode.ERR_VarianceInterfaceNesting, member.Locations[0]);
                    }
                    break;
            }
        }

        internal static NamedTypeSymbol GetEnclosingVariantInterface(Symbol member)
        {
            NamedTypeSymbol containingType = member.ContainingType;
            while ((object)containingType != null && containingType.IsInterfaceType())
            {
                if (containingType.TypeParameters.Any((TypeParameterSymbol tp) => tp.Variance != VarianceKind.None))
                {
                    return containingType;
                }
                containingType = containingType.ContainingType;
            }
            return null;
        }

        internal static void CheckDelegateVarianceSafety(this SourceDelegateMethodSymbol method, BindingDiagnosticBag diagnostics)
        {
            method.CheckMethodVarianceSafety((MethodSymbol m) => m.GetDeclaringSyntax<DelegateDeclarationSyntax>()?.ReturnType.Location, diagnostics);
        }

        private static void CheckMethodVarianceSafety(this MethodSymbol method, BindingDiagnosticBag diagnostics)
        {
            method.CheckMethodVarianceSafety((MethodSymbol m) => m.GetDeclaringSyntax<MethodDeclarationSyntax>()?.ReturnType.Location, diagnostics);
        }

        private static void CheckMethodVarianceSafety(this MethodSymbol method, LocationProvider<MethodSymbol> returnTypeLocationProvider, BindingDiagnosticBag diagnostics)
        {
            if (!SkipVarianceSafetyChecks(method))
            {
                CheckTypeParametersVarianceSafety(method.TypeParameters, method, diagnostics);
                IsVarianceUnsafe(method.ReturnType, requireOutputSafety: true, method.RefKind != RefKind.None, method, returnTypeLocationProvider, method, diagnostics);
                CheckParametersVarianceSafety(method.Parameters, method, diagnostics);
            }
        }

        private static bool SkipVarianceSafetyChecks(Symbol member)
        {
            if (member.IsStatic)
            {
                return MessageID.IDS_FeatureVarianceSafetyForStaticInterfaceMembers.RequiredVersion() <= member.DeclaringCompilation.LanguageVersion;
            }
            return false;
        }

        private static void CheckPropertyVarianceSafety(PropertySymbol property, BindingDiagnosticBag diagnostics)
        {
            if (SkipVarianceSafetyChecks(property))
            {
                return;
            }
            bool flag = (object)property.GetMethod != null;
            bool flag2 = (object)property.SetMethod != null;
            if (flag || flag2)
            {
                TypeSymbol type = property.Type;
                int requireInputSafety;
                if (!flag2)
                {
                    MethodSymbol getMethod = property.GetMethod;
                    requireInputSafety = (((object)getMethod == null || getMethod.RefKind != RefKind.None) ? 1 : 0);
                }
                else
                {
                    requireInputSafety = 1;
                }
                IsVarianceUnsafe(type, flag, (byte)requireInputSafety != 0, property, (PropertySymbol p) => p.GetDeclaringSyntax<BasePropertyDeclarationSyntax>()?.Type.Location, property, diagnostics);
            }
            CheckParametersVarianceSafety(property.Parameters, property, diagnostics);
        }

        private static void CheckEventVarianceSafety(EventSymbol @event, BindingDiagnosticBag diagnostics)
        {
            if (!SkipVarianceSafetyChecks(@event))
            {
                IsVarianceUnsafe(@event.Type, requireOutputSafety: false, requireInputSafety: true, @event, (EventSymbol e) => e.Locations[0], @event, diagnostics);
            }
        }

        private static void CheckParametersVarianceSafety(ImmutableArray<ParameterSymbol> parameters, Symbol context, BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                IsVarianceUnsafe(current.Type, current.RefKind != RefKind.None, requireInputSafety: true, context, (ParameterSymbol p) => p.GetDeclaringSyntax<ParameterSyntax>()?.Type.Location, current, diagnostics);
            }
        }

        private static void CheckTypeParametersVarianceSafety(ImmutableArray<TypeParameterSymbol> typeParameters, MethodSymbol context, BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = typeParameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeParameterSymbol current = enumerator.Current;
                ImmutableArray<TypeWithAnnotations>.Enumerator enumerator2 = current.ConstraintTypesNoUseSiteDiagnostics.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    IsVarianceUnsafe(enumerator2.Current.Type, requireOutputSafety: false, requireInputSafety: true, context, (TypeParameterSymbol t) => t.Locations[0], current, diagnostics);
                }
            }
        }

        private static bool IsVarianceUnsafe<T>(TypeSymbol type, bool requireOutputSafety, bool requireInputSafety, Symbol context, LocationProvider<T> locationProvider, T locationArg, BindingDiagnosticBag diagnostics) where T : Symbol
        {
            switch (type.Kind)
            {
                case SymbolKind.TypeParameter:
                    {
                        TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)type;
                        if (requireInputSafety && requireOutputSafety && typeParameterSymbol.Variance != 0)
                        {
                            diagnostics.AddVarianceError(typeParameterSymbol, context, locationProvider, locationArg, MessageID.IDS_Invariantly);
                            return true;
                        }
                        if (requireOutputSafety && typeParameterSymbol.Variance == VarianceKind.In)
                        {
                            diagnostics.AddVarianceError(typeParameterSymbol, context, locationProvider, locationArg, MessageID.IDS_Covariantly);
                            return true;
                        }
                        if (requireInputSafety && typeParameterSymbol.Variance == VarianceKind.Out)
                        {
                            diagnostics.AddVarianceError(typeParameterSymbol, context, locationProvider, locationArg, MessageID.IDS_Contravariantly);
                            return true;
                        }
                        return false;
                    }
                case SymbolKind.ArrayType:
                    return IsVarianceUnsafe(((ArrayTypeSymbol)type).ElementType, requireOutputSafety, requireInputSafety, context, locationProvider, locationArg, diagnostics);
                case SymbolKind.ErrorType:
                case SymbolKind.NamedType:
                    return IsVarianceUnsafe((NamedTypeSymbol)type, requireOutputSafety, requireInputSafety, context, locationProvider, locationArg, diagnostics);
                default:
                    return false;
            }
        }

        private static bool IsVarianceUnsafe<T>(NamedTypeSymbol namedType, bool requireOutputSafety, bool requireInputSafety, Symbol context, LocationProvider<T> locationProvider, T locationArg, BindingDiagnosticBag diagnostics) where T : Symbol
        {
            switch (namedType.TypeKind)
            {
                default:
                    return false;
                case TypeKind.Class:
                case TypeKind.Delegate:
                case TypeKind.Enum:
                case TypeKind.Error:
                case TypeKind.Interface:
                case TypeKind.Struct:
                    break;
            }
            while ((object)namedType != null)
            {
                for (int i = 0; i < namedType.Arity; i++)
                {
                    TypeParameterSymbol typeParameterSymbol = namedType.TypeParameters[i];
                    TypeSymbol type = namedType.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[i].Type;
                    bool requireOutputSafety2;
                    bool requireInputSafety2;
                    switch (typeParameterSymbol.Variance)
                    {
                        case VarianceKind.Out:
                            requireOutputSafety2 = requireOutputSafety;
                            requireInputSafety2 = requireInputSafety;
                            break;
                        case VarianceKind.In:
                            requireOutputSafety2 = requireInputSafety;
                            requireInputSafety2 = requireOutputSafety;
                            break;
                        case VarianceKind.None:
                            requireInputSafety2 = true;
                            requireOutputSafety2 = true;
                            break;
                        default:
                            throw ExceptionUtilities.UnexpectedValue(typeParameterSymbol.Variance);
                    }
                    if (IsVarianceUnsafe(type, requireOutputSafety2, requireInputSafety2, context, locationProvider, locationArg, diagnostics))
                    {
                        return true;
                    }
                }
                namedType = namedType.ContainingType;
            }
            return false;
        }

        private static void AddVarianceError<T>(this BindingDiagnosticBag diagnostics, TypeParameterSymbol unsafeTypeParameter, Symbol context, LocationProvider<T> locationProvider, T locationArg, MessageID expectedVariance) where T : Symbol
        {
            MessageID id = unsafeTypeParameter.Variance switch
            {
                VarianceKind.In => MessageID.IDS_Contravariant,
                VarianceKind.Out => MessageID.IDS_Covariant,
                _ => throw ExceptionUtilities.UnexpectedValue(unsafeTypeParameter.Variance),
            };
            Location location = locationProvider(locationArg) ?? unsafeTypeParameter.Locations[0];
            if (!(context is TypeSymbol) && context.IsStatic)
            {
                diagnostics.Add(ErrorCode.ERR_UnexpectedVarianceStaticMember, location, context, unsafeTypeParameter, id.Localize(), expectedVariance.Localize(), new CSharpRequiredLanguageVersion(MessageID.IDS_FeatureVarianceSafetyForStaticInterfaceMembers.RequiredVersion()));
            }
            else
            {
                diagnostics.Add(ErrorCode.ERR_UnexpectedVariance, location, context, unsafeTypeParameter, id.Localize(), expectedVariance.Localize());
            }
        }

        private static T GetDeclaringSyntax<T>(this Symbol symbol) where T : SyntaxNode
        {
            ImmutableArray<SyntaxReference> declaringSyntaxReferences = symbol.DeclaringSyntaxReferences;
            if (declaringSyntaxReferences.Length == 0)
            {
                return null;
            }
            return declaringSyntaxReferences[0].GetSyntax() as T;
        }
    }
}
