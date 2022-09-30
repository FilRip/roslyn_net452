using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	public sealed class PredefinedPreprocessorSymbols
	{
		internal static double CurrentVersionNumber => double.Parse(LanguageVersionEnumBounds.GetErrorName(LanguageVersionFacts.MapSpecifiedToEffectiveVersion(LanguageVersion.Latest)), CultureInfo.InvariantCulture);

		public static ImmutableArray<KeyValuePair<string, object>> AddPredefinedPreprocessorSymbols(OutputKind kind, IEnumerable<KeyValuePair<string, object>> symbols)
		{
			return AddPredefinedPreprocessorSymbols(kind, symbols.AsImmutableOrNull());
		}

		public static ImmutableArray<KeyValuePair<string, object>> AddPredefinedPreprocessorSymbols(OutputKind kind, params KeyValuePair<string, object>[] symbols)
		{
			return AddPredefinedPreprocessorSymbols(kind, symbols.AsImmutableOrNull());
		}

		public static ImmutableArray<KeyValuePair<string, object>> AddPredefinedPreprocessorSymbols(OutputKind kind, ImmutableArray<KeyValuePair<string, object>> symbols)
		{
			if (!kind.IsValid())
			{
				throw new ArgumentOutOfRangeException("kind");
			}
			if (symbols.IsDefault)
			{
				symbols = ImmutableArray<KeyValuePair<string, object>>.Empty;
			}
			if (symbols.FirstOrDefault((KeyValuePair<string, object> entry) => CaseInsensitiveComparison.Equals(entry.Key, "VBC_VER"))!.Key == null)
			{
				symbols = symbols.Add(new KeyValuePair<string, object>("VBC_VER", CurrentVersionNumber));
			}
			if (symbols.FirstOrDefault((KeyValuePair<string, object> entry) => CaseInsensitiveComparison.Equals(entry.Key, "TARGET"))!.Key == null)
			{
				symbols = symbols.Add(new KeyValuePair<string, object>("TARGET", GetTargetString(kind)));
			}
			return symbols;
		}

		internal static string GetTargetString(OutputKind kind)
		{
			return kind switch
			{
				OutputKind.ConsoleApplication => "exe", 
				OutputKind.DynamicallyLinkedLibrary => "library", 
				OutputKind.NetModule => "module", 
				OutputKind.WindowsApplication => "winexe", 
				OutputKind.WindowsRuntimeApplication => "appcontainerexe", 
				OutputKind.WindowsRuntimeMetadata => "winmdobj", 
				_ => throw ExceptionUtilities.UnexpectedValue(kind), 
			};
		}
	}
}
