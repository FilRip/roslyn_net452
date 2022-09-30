using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	public sealed class SymbolDisplay
	{
		public static string ToDisplayString(ISymbol symbol, SymbolDisplayFormat format = null)
		{
			return ToDisplayParts(symbol, format).ToDisplayString();
		}

		public static string ToMinimalDisplayString(ISymbol symbol, SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
		{
			return ToMinimalDisplayParts(symbol, semanticModel, position, format).ToDisplayString();
		}

		public static ImmutableArray<SymbolDisplayPart> ToDisplayParts(ISymbol symbol, SymbolDisplayFormat format = null)
		{
			format = format ?? SymbolDisplayFormat.VisualBasicErrorMessageFormat;
			return ToDisplayParts(symbol, null, -1, format, minimal: false);
		}

		public static ImmutableArray<SymbolDisplayPart> ToMinimalDisplayParts(ISymbol symbol, SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
		{
			format = format ?? SymbolDisplayFormat.MinimallyQualifiedFormat;
			return ToDisplayParts(symbol, semanticModel, position, format, minimal: true);
		}

		private static ImmutableArray<SymbolDisplayPart> ToDisplayParts(ISymbol symbol, SemanticModel semanticModelOpt, int positionOpt, SymbolDisplayFormat format, bool minimal)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException("symbol");
			}
			if (minimal)
			{
				if (semanticModelOpt == null)
				{
					throw new ArgumentException(VBResources.SemanticModelMustBeProvided);
				}
				if (positionOpt < 0 || positionOpt > semanticModelOpt.SyntaxTree.Length)
				{
					throw new ArgumentOutOfRangeException(VBResources.PositionNotWithinTree);
				}
			}
			ArrayBuilder<SymbolDisplayPart> instance = ArrayBuilder<SymbolDisplayPart>.GetInstance();
			SymbolDisplayVisitor visitor = new SymbolDisplayVisitor(instance, format, semanticModelOpt, positionOpt);
			symbol.Accept(visitor);
			return instance.ToImmutableAndFree();
		}

		public static string FormatPrimitive(object obj, bool quoteStrings, bool useHexadecimalNumbers)
		{
			ObjectDisplayOptions objectDisplayOptions = ObjectDisplayOptions.None;
			if (quoteStrings)
			{
				objectDisplayOptions = objectDisplayOptions | ObjectDisplayOptions.UseQuotes | ObjectDisplayOptions.EscapeNonPrintableCharacters;
			}
			if (useHexadecimalNumbers)
			{
				objectDisplayOptions |= ObjectDisplayOptions.UseHexadecimalNumbers;
			}
			return Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.FormatPrimitive(RuntimeHelpers.GetObjectValue(obj), objectDisplayOptions);
		}

		internal static void AddSymbolDisplayParts(ArrayBuilder<SymbolDisplayPart> parts, string str)
		{
			PooledStringBuilder instance = PooledStringBuilder.GetInstance();
			StringBuilder builder = instance.Builder;
			int num = -1;
			foreach (int item in Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.TokenizeString(str, ObjectDisplayOptions.UseHexadecimalNumbers | ObjectDisplayOptions.UseQuotes | ObjectDisplayOptions.EscapeNonPrintableCharacters))
			{
				int num2 = item >> 16;
				if (num >= 0 && num != num2)
				{
					parts.Add(new SymbolDisplayPart((SymbolDisplayPartKind)num, null, builder.ToString()));
					builder.Clear();
				}
				num = num2;
				builder.Append(Strings.ChrW(item & 0xFFFF));
			}
			if (num >= 0)
			{
				parts.Add(new SymbolDisplayPart((SymbolDisplayPartKind)num, null, builder.ToString()));
			}
			instance.Free();
		}

		internal static void AddSymbolDisplayParts(ArrayBuilder<SymbolDisplayPart> parts, char c)
		{
			string wellKnownCharacterName = Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.GetWellKnownCharacterName(c);
			if (wellKnownCharacterName != null)
			{
				parts.Add(new SymbolDisplayPart(SymbolDisplayPartKind.FieldName, null, wellKnownCharacterName));
				return;
			}
			if (Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.IsPrintable(c))
			{
				parts.Add(new SymbolDisplayPart(SymbolDisplayPartKind.StringLiteral, null, "\"" + Microsoft.VisualBasic.CompilerServices.Conversions.ToString(c) + "\"c"));
				return;
			}
			int num = c;
			parts.Add(new SymbolDisplayPart(SymbolDisplayPartKind.MethodName, null, "ChrW"));
			parts.Add(new SymbolDisplayPart(SymbolDisplayPartKind.Punctuation, null, "("));
			parts.Add(new SymbolDisplayPart(SymbolDisplayPartKind.NumericLiteral, null, "&H" + num.ToString("X")));
			parts.Add(new SymbolDisplayPart(SymbolDisplayPartKind.Punctuation, null, ")"));
		}
	}
}
