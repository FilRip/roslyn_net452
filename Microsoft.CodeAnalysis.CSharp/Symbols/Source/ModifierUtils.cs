// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class ModifierUtils
    {
        internal static DeclarationModifiers MakeAndCheckNontypeMemberModifiers(
            SyntaxTokenList modifiers,
            DeclarationModifiers defaultAccess,
            DeclarationModifiers allowedModifiers,
            Location errorLocation,
            BindingDiagnosticBag diagnostics,
            out bool modifierErrors)
        {
            var result = modifiers.ToDeclarationModifiers(diagnostics.DiagnosticBag ?? new DiagnosticBag());
            result = CheckModifiers(result, allowedModifiers, errorLocation, diagnostics, modifiers, out modifierErrors);

            if ((result & DeclarationModifiers.AccessibilityMask) == 0)
            {
                result |= defaultAccess;
            }

            return result;
        }

        internal static DeclarationModifiers CheckModifiers(
            DeclarationModifiers modifiers,
            DeclarationModifiers allowedModifiers,
            Location errorLocation,
            BindingDiagnosticBag diagnostics,
            SyntaxTokenList? modifierTokens,
            out bool modifierErrors)
        {
            modifierErrors = false;
            DeclarationModifiers errorModifiers = modifiers & ~allowedModifiers;
            DeclarationModifiers result = modifiers & allowedModifiers;

            while (errorModifiers != DeclarationModifiers.None)
            {
                DeclarationModifiers oneError = errorModifiers & ~(errorModifiers - 1);
                errorModifiers &= ~oneError;

                switch (oneError)
                {
                    case DeclarationModifiers.Partial:
                        // Provide a specialized error message in the case of partial.
                        ReportPartialError(errorLocation, diagnostics, modifierTokens);
                        break;

                    default:
                        diagnostics.Add(ErrorCode.ERR_BadMemberFlag, errorLocation, ConvertSingleModifierToSyntaxText(oneError));
                        break;
                }

                modifierErrors = true;
            }

            if ((result & DeclarationModifiers.PrivateProtected) != 0)
            {
                modifierErrors |= !Binder.CheckFeatureAvailability(errorLocation.SourceTree, MessageID.IDS_FeaturePrivateProtected, diagnostics, errorLocation);
            }

            return result;
        }

        private static void ReportPartialError(Location errorLocation, BindingDiagnosticBag diagnostics, SyntaxTokenList? modifierTokens)
        {
            // If we can find the 'partial' token, report it on that.
            if (modifierTokens != null)
            {
                var partialToken = modifierTokens.Value.FirstOrDefault(SyntaxKind.PartialKeyword);
                if (partialToken != default)
                {
                    diagnostics.Add(ErrorCode.ERR_PartialMisplaced, partialToken.GetLocation());
                    return;
                }
            }

            diagnostics.Add(ErrorCode.ERR_PartialMisplaced, errorLocation);
        }

        internal static void ReportDefaultInterfaceImplementationModifiers(
            bool hasBody,
            DeclarationModifiers modifiers,
            DeclarationModifiers defaultInterfaceImplementationModifiers,
            Location errorLocation,
            BindingDiagnosticBag diagnostics)
        {
            if (!hasBody && (modifiers & defaultInterfaceImplementationModifiers) != 0)
            {
                LanguageVersion availableVersion = ((CSharpParseOptions)errorLocation.SourceTree.Options).LanguageVersion;
                LanguageVersion requiredVersion = MessageID.IDS_DefaultInterfaceImplementation.RequiredVersion();
                if (availableVersion < requiredVersion)
                {
                    DeclarationModifiers errorModifiers = modifiers & defaultInterfaceImplementationModifiers;
                    var requiredVersionArgument = new CSharpRequiredLanguageVersion(requiredVersion);
                    var availableVersionArgument = availableVersion.ToDisplayString();
                    while (errorModifiers != DeclarationModifiers.None)
                    {
                        DeclarationModifiers oneError = errorModifiers & ~(errorModifiers - 1);
                        errorModifiers &= ~oneError;
                        diagnostics.Add(ErrorCode.ERR_InvalidModifierForLanguageVersion, errorLocation,
                                        ConvertSingleModifierToSyntaxText(oneError),
                                        availableVersionArgument,
                                        requiredVersionArgument);
                    }
                }
            }
        }

        internal static DeclarationModifiers AdjustModifiersForAnInterfaceMember(DeclarationModifiers mods, bool hasBody, bool isExplicitInterfaceImplementation)
        {
            if (isExplicitInterfaceImplementation)
            {
                if ((mods & DeclarationModifiers.Abstract) != 0)
                {
                    mods |= DeclarationModifiers.Sealed;
                }
            }
            else if ((mods & (DeclarationModifiers.Static | DeclarationModifiers.Private | DeclarationModifiers.Partial | DeclarationModifiers.Virtual | DeclarationModifiers.Abstract)) == 0)
            {

                if (hasBody || (mods & (DeclarationModifiers.Extern | DeclarationModifiers.Sealed)) != 0)
                {
                    if ((mods & DeclarationModifiers.Sealed) == 0)
                    {
                        mods |= DeclarationModifiers.Virtual;
                    }
                    else
                    {
                        mods &= ~DeclarationModifiers.Sealed;
                    }
                }
                else
                {
                    mods |= DeclarationModifiers.Abstract;
                }
            }

            if ((mods & DeclarationModifiers.AccessibilityMask) == 0)
            {
                if ((mods & DeclarationModifiers.Partial) == 0 && !isExplicitInterfaceImplementation)
                {
                    mods |= DeclarationModifiers.Public;
                }
                else
                {
                    mods |= DeclarationModifiers.Private;
                }
            }

            return mods;
        }

        private static string ConvertSingleModifierToSyntaxText(DeclarationModifiers modifier)
        {
            return modifier switch
            {
                DeclarationModifiers.Abstract => SyntaxFacts.GetText(SyntaxKind.AbstractKeyword),
                DeclarationModifiers.Sealed => SyntaxFacts.GetText(SyntaxKind.SealedKeyword),
                DeclarationModifiers.Static => SyntaxFacts.GetText(SyntaxKind.StaticKeyword),
                DeclarationModifiers.New => SyntaxFacts.GetText(SyntaxKind.NewKeyword),
                DeclarationModifiers.Public => SyntaxFacts.GetText(SyntaxKind.PublicKeyword),
                DeclarationModifiers.Protected => SyntaxFacts.GetText(SyntaxKind.ProtectedKeyword),
                DeclarationModifiers.Internal => SyntaxFacts.GetText(SyntaxKind.InternalKeyword),
                DeclarationModifiers.ProtectedInternal => SyntaxFacts.GetText(SyntaxKind.ProtectedKeyword) + " " + SyntaxFacts.GetText(SyntaxKind.InternalKeyword),
                DeclarationModifiers.Private => SyntaxFacts.GetText(SyntaxKind.PrivateKeyword),
                DeclarationModifiers.PrivateProtected => SyntaxFacts.GetText(SyntaxKind.PrivateKeyword) + " " + SyntaxFacts.GetText(SyntaxKind.ProtectedKeyword),
                DeclarationModifiers.ReadOnly => SyntaxFacts.GetText(SyntaxKind.ReadOnlyKeyword),
                DeclarationModifiers.Const => SyntaxFacts.GetText(SyntaxKind.ConstKeyword),
                DeclarationModifiers.Volatile => SyntaxFacts.GetText(SyntaxKind.VolatileKeyword),
                DeclarationModifiers.Extern => SyntaxFacts.GetText(SyntaxKind.ExternKeyword),
                DeclarationModifiers.Partial => SyntaxFacts.GetText(SyntaxKind.PartialKeyword),
                DeclarationModifiers.Unsafe => SyntaxFacts.GetText(SyntaxKind.UnsafeKeyword),
                DeclarationModifiers.Fixed => SyntaxFacts.GetText(SyntaxKind.FixedKeyword),
                DeclarationModifiers.Virtual => SyntaxFacts.GetText(SyntaxKind.VirtualKeyword),
                DeclarationModifiers.Override => SyntaxFacts.GetText(SyntaxKind.OverrideKeyword),
                DeclarationModifiers.Async => SyntaxFacts.GetText(SyntaxKind.AsyncKeyword),
                DeclarationModifiers.Ref => SyntaxFacts.GetText(SyntaxKind.RefKeyword),
                DeclarationModifiers.Data => SyntaxFacts.GetText(SyntaxKind.DataKeyword),
                _ => throw ExceptionUtilities.UnexpectedValue(modifier),
            };
        }

        private static DeclarationModifiers ToDeclarationModifier(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.AbstractKeyword => DeclarationModifiers.Abstract,
                SyntaxKind.AsyncKeyword => DeclarationModifiers.Async,
                SyntaxKind.SealedKeyword => DeclarationModifiers.Sealed,
                SyntaxKind.StaticKeyword => DeclarationModifiers.Static,
                SyntaxKind.NewKeyword => DeclarationModifiers.New,
                SyntaxKind.PublicKeyword => DeclarationModifiers.Public,
                SyntaxKind.ProtectedKeyword => DeclarationModifiers.Protected,
                SyntaxKind.InternalKeyword => DeclarationModifiers.Internal,
                SyntaxKind.PrivateKeyword => DeclarationModifiers.Private,
                SyntaxKind.ExternKeyword => DeclarationModifiers.Extern,
                SyntaxKind.ReadOnlyKeyword => DeclarationModifiers.ReadOnly,
                SyntaxKind.PartialKeyword => DeclarationModifiers.Partial,
                SyntaxKind.UnsafeKeyword => DeclarationModifiers.Unsafe,
                SyntaxKind.VirtualKeyword => DeclarationModifiers.Virtual,
                SyntaxKind.OverrideKeyword => DeclarationModifiers.Override,
                SyntaxKind.ConstKeyword => DeclarationModifiers.Const,
                SyntaxKind.FixedKeyword => DeclarationModifiers.Fixed,
                SyntaxKind.VolatileKeyword => DeclarationModifiers.Volatile,
                SyntaxKind.RefKeyword => DeclarationModifiers.Ref,
                SyntaxKind.DataKeyword => DeclarationModifiers.Data,
                _ => throw ExceptionUtilities.UnexpectedValue(kind),
            };
        }

        public static DeclarationModifiers ToDeclarationModifiers(
            this SyntaxTokenList modifiers, DiagnosticBag diagnostics)
        {
            var result = DeclarationModifiers.None;
            bool seenNoDuplicates = true;
            bool seenNoAccessibilityDuplicates = true;

            foreach (var modifier in modifiers)
            {
                DeclarationModifiers one = ToDeclarationModifier(modifier.ContextualKind());

                ReportDuplicateModifiers(
                    modifier, one, result,
                    ref seenNoDuplicates, ref seenNoAccessibilityDuplicates,
                    diagnostics);

                result |= one;
            }

            switch (result & DeclarationModifiers.AccessibilityMask)
            {
                case DeclarationModifiers.Protected | DeclarationModifiers.Internal:
                    // the two keywords "protected" and "internal" together are treated as one modifier.
                    result &= ~DeclarationModifiers.AccessibilityMask;
                    result |= DeclarationModifiers.ProtectedInternal;
                    break;

                case DeclarationModifiers.Private | DeclarationModifiers.Protected:
                    // the two keywords "private" and "protected" together are treated as one modifier.
                    result &= ~DeclarationModifiers.AccessibilityMask;
                    result |= DeclarationModifiers.PrivateProtected;
                    break;
            }

            return result;
        }

        private static void ReportDuplicateModifiers(
            SyntaxToken modifierToken,
            DeclarationModifiers modifierKind,
            DeclarationModifiers allModifiers,
            ref bool seenNoDuplicates,
            ref bool seenNoAccessibilityDuplicates,
            DiagnosticBag diagnostics)
        {
            if ((allModifiers & modifierKind) != 0)
            {
                if (seenNoDuplicates)
                {
                    diagnostics.Add(
                        ErrorCode.ERR_DuplicateModifier,
                        modifierToken.GetLocation(),
                        SyntaxFacts.GetText(modifierToken.Kind()));
                    seenNoDuplicates = false;
                }
            }
        }

        internal static CSDiagnosticInfo CheckAccessibility(DeclarationModifiers modifiers, Symbol symbol, bool isExplicitInterfaceImplementation)
        {
            if (!IsValidAccessibility(modifiers))
            {
                // error CS0107: More than one protection modifier
                return new CSDiagnosticInfo(ErrorCode.ERR_BadMemberProtection);
            }

            if (!isExplicitInterfaceImplementation &&
                (symbol.Kind != SymbolKind.Method || (modifiers & DeclarationModifiers.Partial) == 0) &&
                (modifiers & DeclarationModifiers.Static) == 0)
            {
                switch (modifiers & DeclarationModifiers.AccessibilityMask)
                {
                    case DeclarationModifiers.Protected:
                    case DeclarationModifiers.ProtectedInternal:
                    case DeclarationModifiers.PrivateProtected:

                        if (symbol.ContainingType?.IsInterface == true && !symbol.ContainingAssembly.RuntimeSupportsDefaultInterfaceImplementation)
                        {
                            return new CSDiagnosticInfo(ErrorCode.ERR_RuntimeDoesNotSupportProtectedAccessForInterfaceMember);
                        }
                        break;
                }
            }

            return null;
        }

        // Returns declared accessibility.
        // In a case of bogus accessibility (i.e. "public private"), defaults to public.
        internal static Accessibility EffectiveAccessibility(DeclarationModifiers modifiers)
        {
            return (modifiers & DeclarationModifiers.AccessibilityMask) switch
            {
                DeclarationModifiers.None => Accessibility.NotApplicable,// for explicit interface implementation
                DeclarationModifiers.Private => Accessibility.Private,
                DeclarationModifiers.Protected => Accessibility.Protected,
                DeclarationModifiers.Internal => Accessibility.Internal,
                DeclarationModifiers.Public => Accessibility.Public,
                DeclarationModifiers.ProtectedInternal => Accessibility.ProtectedOrInternal,
                DeclarationModifiers.PrivateProtected => Accessibility.ProtectedAndInternal,
                _ => Accessibility.Public,// This happens when you have a mix of accessibilities.
                                          //
                                          // i.e.: public private void Goo()
            };
        }

        internal static bool IsValidAccessibility(DeclarationModifiers modifiers)
        {
            return (modifiers & DeclarationModifiers.AccessibilityMask) switch
            {
                DeclarationModifiers.None or DeclarationModifiers.Private or DeclarationModifiers.Protected or DeclarationModifiers.Internal or DeclarationModifiers.Public or DeclarationModifiers.ProtectedInternal or DeclarationModifiers.PrivateProtected => true,
                _ => false,// This happens when you have a mix of accessibilities.
                           //
                           // i.e.: public private void Goo()
            };
        }
    }
}
