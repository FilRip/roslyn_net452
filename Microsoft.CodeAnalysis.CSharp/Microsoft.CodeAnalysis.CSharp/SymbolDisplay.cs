using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public static class SymbolDisplay
    {
        public static string ToDisplayString(ISymbol symbol, SymbolDisplayFormat? format = null)
        {
            return ToDisplayParts(symbol, format).ToDisplayString();
        }

        public static string ToDisplayString(ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableFlowState nullableFlowState, SymbolDisplayFormat? format = null)
        {
            return ToDisplayParts(symbol, nullableFlowState, format).ToDisplayString();
        }

        public static string ToDisplayString(ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation, SymbolDisplayFormat? format = null)
        {
            return ToDisplayParts(symbol, nullableAnnotation, format).ToDisplayString();
        }

        public static string ToMinimalDisplayString(ISymbol symbol, SemanticModel semanticModel, int position, SymbolDisplayFormat? format = null)
        {
            return ToMinimalDisplayParts(symbol, semanticModel, position, format).ToDisplayString();
        }

        public static string ToMinimalDisplayString(ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableFlowState nullableFlowState, SemanticModel semanticModel, int position, SymbolDisplayFormat? format = null)
        {
            return ToMinimalDisplayParts(symbol, nullableFlowState, semanticModel, position, format).ToDisplayString();
        }

        public static string ToMinimalDisplayString(ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation, SemanticModel semanticModel, int position, SymbolDisplayFormat? format = null)
        {
            return ToMinimalDisplayParts(symbol, nullableAnnotation, semanticModel, position, format).ToDisplayString();
        }

        public static ImmutableArray<SymbolDisplayPart> ToDisplayParts(ISymbol symbol, SymbolDisplayFormat? format = null)
        {
            format = format ?? SymbolDisplayFormat.CSharpErrorMessageFormat;
            return ToDisplayParts(symbol, null, -1, format, minimal: false);
        }

        public static ImmutableArray<SymbolDisplayPart> ToDisplayParts(ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableFlowState nullableFlowState, SymbolDisplayFormat? format = null)
        {
            format = format ?? SymbolDisplayFormat.CSharpErrorMessageFormat;
            return ToDisplayParts(symbol, nullableFlowState, null, -1, format, minimal: false);
        }

        public static ImmutableArray<SymbolDisplayPart> ToDisplayParts(ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation, SymbolDisplayFormat? format = null)
        {
            if (format == null)
            {
                format = SymbolDisplayFormat.CSharpErrorMessageFormat;
            }
            return ToDisplayParts(symbol.WithNullableAnnotation(nullableAnnotation), null, -1, format, minimal: false);
        }

        public static ImmutableArray<SymbolDisplayPart> ToMinimalDisplayParts(ISymbol symbol, SemanticModel semanticModel, int position, SymbolDisplayFormat? format = null)
        {
            if (format == null)
            {
                format = SymbolDisplayFormat.MinimallyQualifiedFormat;
            }
            return ToDisplayParts(symbol, semanticModel, position, format, minimal: true);
        }

        public static ImmutableArray<SymbolDisplayPart> ToMinimalDisplayParts(ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableFlowState nullableFlowState, SemanticModel semanticModel, int position, SymbolDisplayFormat? format = null)
        {
            if (format == null)
            {
                format = SymbolDisplayFormat.MinimallyQualifiedFormat;
            }
            return ToDisplayParts(symbol, nullableFlowState, semanticModel, position, format, minimal: true);
        }

        public static ImmutableArray<SymbolDisplayPart> ToMinimalDisplayParts(ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation, SemanticModel semanticModel, int position, SymbolDisplayFormat? format = null)
        {
            if (format == null)
            {
                format = SymbolDisplayFormat.MinimallyQualifiedFormat;
            }
            return ToDisplayParts(symbol.WithNullableAnnotation(nullableAnnotation), semanticModel, position, format, minimal: true);
        }

        private static ImmutableArray<SymbolDisplayPart> ToDisplayParts(ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableFlowState nullableFlowState, SemanticModel? semanticModelOpt, int positionOpt, SymbolDisplayFormat format, bool minimal)
        {
            return ToDisplayParts(symbol.WithNullableAnnotation(nullableFlowState.ToAnnotation()), semanticModelOpt, positionOpt, format, minimal);
        }

        private static ImmutableArray<SymbolDisplayPart> ToDisplayParts(ISymbol symbol, SemanticModel? semanticModelOpt, int positionOpt, SymbolDisplayFormat format, bool minimal)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException("symbol");
            }
            if (minimal)
            {
                if (semanticModelOpt == null)
                {
                    throw new ArgumentException(CSharpResources.SyntaxTreeSemanticModelMust);
                }
                if (positionOpt < 0 || positionOpt > semanticModelOpt!.SyntaxTree.Length)
                {
                    throw new ArgumentOutOfRangeException(CSharpResources.PositionNotWithinTree);
                }
            }
            if ((symbol as Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.MethodSymbol)?.UnderlyingMethodSymbol is SynthesizedSimpleProgramEntryPointSymbol)
            {
                return ImmutableArray.Create(new SymbolDisplayPart(SymbolDisplayPartKind.MethodName, symbol, "<top-level-statements-entry-point>"));
            }
            ArrayBuilder<SymbolDisplayPart> instance = ArrayBuilder<SymbolDisplayPart>.GetInstance();
            SymbolDisplayVisitor visitor = new SymbolDisplayVisitor(instance, format, semanticModelOpt, positionOpt);
            symbol.Accept(visitor);
            return instance.ToImmutableAndFree();
        }

        public static string FormatPrimitive(object obj, bool quoteStrings, bool useHexadecimalNumbers)
        {
            ObjectDisplayOptions objectDisplayOptions = ObjectDisplayOptions.EscapeNonPrintableCharacters;
            if (quoteStrings)
            {
                objectDisplayOptions |= ObjectDisplayOptions.UseQuotes;
            }
            if (useHexadecimalNumbers)
            {
                objectDisplayOptions |= ObjectDisplayOptions.UseHexadecimalNumbers;
            }
            return ObjectDisplay.FormatPrimitive(obj, objectDisplayOptions);
        }

        public static string FormatLiteral(string value, bool quote)
        {
            ObjectDisplayOptions options = ObjectDisplayOptions.EscapeNonPrintableCharacters | (quote ? ObjectDisplayOptions.UseQuotes : ObjectDisplayOptions.None);
            return ObjectDisplay.FormatLiteral(value, options);
        }

        public static string FormatLiteral(char c, bool quote)
        {
            ObjectDisplayOptions options = ObjectDisplayOptions.EscapeNonPrintableCharacters | (quote ? ObjectDisplayOptions.UseQuotes : ObjectDisplayOptions.None);
            return ObjectDisplay.FormatLiteral(c, options);
        }
    }
}
