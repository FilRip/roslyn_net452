// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;

using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.SymbolDisplay;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal partial class SymbolDisplayVisitor : AbstractSymbolDisplayVisitor
    {
        private readonly bool _escapeKeywordIdentifiers;
        private IDictionary<INamespaceOrTypeSymbol, IAliasSymbol> _lazyAliasMap;

        internal SymbolDisplayVisitor(
            ArrayBuilder<SymbolDisplayPart> builder,
            SymbolDisplayFormat format,
            SemanticModel semanticModelOpt,
            int positionOpt)
            : base(builder, format, true, semanticModelOpt, positionOpt)
        {
            _escapeKeywordIdentifiers = format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);
        }

        private SymbolDisplayVisitor(
            ArrayBuilder<SymbolDisplayPart> builder,
            SymbolDisplayFormat format,
            SemanticModel semanticModelOpt,
            int positionOpt,
            bool escapeKeywordIdentifiers,
            IDictionary<INamespaceOrTypeSymbol, IAliasSymbol> aliasMap,
            bool isFirstSymbolVisited,
            bool inNamespaceOrType = false)
            : base(builder, format, isFirstSymbolVisited, semanticModelOpt, positionOpt, inNamespaceOrType)
        {
            _escapeKeywordIdentifiers = escapeKeywordIdentifiers;
            _lazyAliasMap = aliasMap;
        }

        protected override AbstractSymbolDisplayVisitor MakeNotFirstVisitor(bool inNamespaceOrType = false)
        {
            return new SymbolDisplayVisitor(
                this.builder,
                this.format,
                this.semanticModelOpt,
                this.positionOpt,
                _escapeKeywordIdentifiers,
                _lazyAliasMap,
                isFirstSymbolVisited: false,
                inNamespaceOrType: inNamespaceOrType);
        }

        internal SymbolDisplayPart CreatePart(SymbolDisplayPartKind kind, ISymbol symbol, string text)
        {
            text = (text == null) ? "?" :
                   (_escapeKeywordIdentifiers && IsEscapable(kind)) ? EscapeIdentifier(text) : text;

            return new SymbolDisplayPart(kind, symbol, text);
        }

        private static bool IsEscapable(SymbolDisplayPartKind kind)
        {
            return kind switch
            {
                SymbolDisplayPartKind.AliasName or SymbolDisplayPartKind.ClassName or SymbolDisplayPartKind.RecordClassName or SymbolDisplayPartKind.StructName or SymbolDisplayPartKind.InterfaceName or SymbolDisplayPartKind.EnumName or SymbolDisplayPartKind.DelegateName or SymbolDisplayPartKind.TypeParameterName or SymbolDisplayPartKind.MethodName or SymbolDisplayPartKind.PropertyName or SymbolDisplayPartKind.FieldName or SymbolDisplayPartKind.LocalName or SymbolDisplayPartKind.NamespaceName or SymbolDisplayPartKind.ParameterName => true,
                _ => false,
            };
        }

        private static string EscapeIdentifier(string identifier)
        {
            var kind = SyntaxFacts.GetKeywordKind(identifier);
            return kind == SyntaxKind.None
                ? identifier
                : $"@{identifier}";
        }

        public override void VisitAssembly(IAssemblySymbol symbol)
        {
            var text = format.TypeQualificationStyle == SymbolDisplayTypeQualificationStyle.NameOnly
                ? symbol.Identity.Name
                : symbol.Identity.GetDisplayName();

            builder.Add(CreatePart(SymbolDisplayPartKind.AssemblyName, symbol, text));
        }

        public override void VisitModule(IModuleSymbol symbol)
        {
            builder.Add(CreatePart(SymbolDisplayPartKind.ModuleName, symbol, symbol.Name));
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            if (this.IsMinimizing)
            {
                if (TryAddAlias(symbol, builder))
                {
                    return;
                }

                MinimallyQualify(symbol);
                return;
            }

            if (isFirstSymbolVisited && format.KindOptions.IncludesOption(SymbolDisplayKindOptions.IncludeNamespaceKeyword))
            {
                AddKeyword(SyntaxKind.NamespaceKeyword);
                AddSpace();
            }

            if (format.TypeQualificationStyle == SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces)
            {
                var containingNamespace = symbol.ContainingNamespace;
                if (ShouldVisitNamespace(containingNamespace))
                {
                    containingNamespace.Accept(this.NotFirstVisitor);
                    AddPunctuation(containingNamespace.IsGlobalNamespace ? SyntaxKind.ColonColonToken : SyntaxKind.DotToken);
                }
            }

            if (symbol.IsGlobalNamespace)
            {
                AddGlobalNamespace(symbol);
            }
            else
            {
                builder.Add(CreatePart(SymbolDisplayPartKind.NamespaceName, symbol, symbol.Name));
            }
        }

        private void AddGlobalNamespace(INamespaceSymbol globalNamespace)
        {
            // Formerly, localized via MessageID.IDS_GlobalNamespace.
            const string standaloneGlobalNamespaceString = "<global namespace>";

            switch (format.GlobalNamespaceStyle)
            {
                case SymbolDisplayGlobalNamespaceStyle.Omitted:
                    break;
                case SymbolDisplayGlobalNamespaceStyle.Included:
                    if (this.isFirstSymbolVisited)
                    {
                        builder.Add(CreatePart(
                            SymbolDisplayPartKind.Text,
                            globalNamespace,
                            standaloneGlobalNamespaceString));
                    }
                    else
                    {
                        builder.Add(CreatePart(SymbolDisplayPartKind.Keyword, globalNamespace,
                            SyntaxFacts.GetText(SyntaxKind.GlobalKeyword)));
                    }
                    break;
                case SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining:
                    builder.Add(CreatePart(
                        SymbolDisplayPartKind.Text,
                        globalNamespace,
                        standaloneGlobalNamespaceString));
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(format.GlobalNamespaceStyle);
            }
        }

        public override void VisitLocal(ILocalSymbol symbol)
        {
            if (symbol.IsRef &&
                format.LocalOptions.IncludesOption(SymbolDisplayLocalOptions.IncludeRef))
            {
                AddKeyword(SyntaxKind.RefKeyword);
                AddSpace();

                if (symbol.RefKind == RefKind.RefReadOnly)
                {
                    AddKeyword(SyntaxKind.ReadOnlyKeyword);
                    AddSpace();
                }
            }

            if (format.LocalOptions.IncludesOption(SymbolDisplayLocalOptions.IncludeType))
            {
                symbol.Type.Accept(this.NotFirstVisitor);
                AddSpace();
            }

            if (symbol.IsConst)
            {
                builder.Add(CreatePart(SymbolDisplayPartKind.ConstantName, symbol, symbol.Name));
            }
            else
            {
                builder.Add(CreatePart(SymbolDisplayPartKind.LocalName, symbol, symbol.Name));
            }

            if (format.LocalOptions.IncludesOption(SymbolDisplayLocalOptions.IncludeConstantValue) &&
                symbol.IsConst &&
                symbol.HasConstantValue &&
                CanAddConstant(symbol.Type, symbol.ConstantValue))
            {
                AddSpace();
                AddPunctuation(SyntaxKind.EqualsToken);
                AddSpace();

                AddConstantValue(symbol.Type, symbol.ConstantValue);
            }
        }

        public override void VisitDiscard(IDiscardSymbol symbol)
        {
            if (format.LocalOptions.IncludesOption(SymbolDisplayLocalOptions.IncludeType))
            {
                symbol.Type.Accept(this.NotFirstVisitor);
                AddSpace();
            }

            builder.Add(CreatePart(SymbolDisplayPartKind.Punctuation, symbol, "_"));
        }

        public override void VisitRangeVariable(IRangeVariableSymbol symbol)
        {
            if (format.LocalOptions.IncludesOption(SymbolDisplayLocalOptions.IncludeType))
            {
                ITypeSymbol type = GetRangeVariableType(symbol);

                if (type != null && type.TypeKind != TypeKind.Error)
                {
                    type.Accept(this);
                }
                else
                {
                    builder.Add(CreatePart(SymbolDisplayPartKind.ErrorTypeName, type, "?"));
                }

                AddSpace();
            }

            builder.Add(CreatePart(SymbolDisplayPartKind.RangeVariableName, symbol, symbol.Name));
        }

        public override void VisitLabel(ILabelSymbol symbol)
        {
            builder.Add(CreatePart(SymbolDisplayPartKind.LabelName, symbol, symbol.Name));
        }

        public override void VisitAlias(IAliasSymbol symbol)
        {
            builder.Add(CreatePart(SymbolDisplayPartKind.AliasName, symbol, symbol.Name));

            if (format.LocalOptions.IncludesOption(SymbolDisplayLocalOptions.IncludeType))
            {
                // ???
                AddPunctuation(SyntaxKind.EqualsToken);
                symbol.Target.Accept(this);
            }
        }

        protected override void AddSpace()
        {
            builder.Add(CreatePart(SymbolDisplayPartKind.Space, null, " "));
        }

        private void AddPunctuation(SyntaxKind punctuationKind)
        {
            builder.Add(CreatePart(SymbolDisplayPartKind.Punctuation, null, SyntaxFacts.GetText(punctuationKind)));
        }

        private void AddKeyword(SyntaxKind keywordKind)
        {
            builder.Add(CreatePart(SymbolDisplayPartKind.Keyword, null, SyntaxFacts.GetText(keywordKind)));
        }

        private void AddAccessibilityIfRequired(ISymbol symbol)
        {
            INamedTypeSymbol containingType = symbol.ContainingType;

            // this method is only called for members and they should have a containingType or a containing symbol should be a TypeSymbol.

            if (format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeAccessibility) &&
                (containingType == null ||
                 (containingType.TypeKind != TypeKind.Interface && !IsEnumMember(symbol) & !IsLocalFunction(symbol))))
            {
                AddAccessibility(symbol);
            }
        }

        private static bool IsLocalFunction(ISymbol symbol)
        {
            if (symbol.Kind != SymbolKind.Method)
            {
                return false;
            }

            return ((IMethodSymbol)symbol).MethodKind == MethodKind.LocalFunction;
        }

        private void AddAccessibility(ISymbol symbol)
        {
            switch (symbol.DeclaredAccessibility)
            {
                case Accessibility.Private:
                    AddKeyword(SyntaxKind.PrivateKeyword);
                    break;
                case Accessibility.Internal:
                    AddKeyword(SyntaxKind.InternalKeyword);
                    break;
                case Accessibility.ProtectedAndInternal:
                    AddKeyword(SyntaxKind.PrivateKeyword);
                    AddSpace();
                    AddKeyword(SyntaxKind.ProtectedKeyword);
                    break;
                case Accessibility.Protected:
                    AddKeyword(SyntaxKind.ProtectedKeyword);
                    break;
                case Accessibility.ProtectedOrInternal:
                    AddKeyword(SyntaxKind.ProtectedKeyword);
                    AddSpace();
                    AddKeyword(SyntaxKind.InternalKeyword);
                    break;
                case Accessibility.Public:
                    AddKeyword(SyntaxKind.PublicKeyword);
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(symbol.DeclaredAccessibility);
            }

            AddSpace();
        }

        private bool ShouldVisitNamespace(ISymbol containingSymbol)
        {
            if (containingSymbol is not INamespaceSymbol namespaceSymbol)
            {
                return false;
            }

            if (format.TypeQualificationStyle != SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces)
            {
                return false;
            }

            return
                !namespaceSymbol.IsGlobalNamespace ||
                format.GlobalNamespaceStyle == SymbolDisplayGlobalNamespaceStyle.Included;
        }

        private bool IncludeNamedType(INamedTypeSymbol namedType)
        {
            if (namedType is null)
            {
                return false;
            }

            if (namedType.IsScriptClass && !format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.IncludeScriptType))
            {
                return false;
            }

            if (namedType == semanticModelOpt?.Compilation.ScriptGlobalsType)
            {
                return false;
            }

            return true;
        }

        private static bool IsEnumMember(ISymbol symbol)
        {
            return symbol != null
                && symbol.Kind == SymbolKind.Field
                && symbol.ContainingType != null
                && symbol.ContainingType.TypeKind == TypeKind.Enum
                && symbol.Name != WellKnownMemberNames.EnumBackingFieldName;
        }
    }
}
