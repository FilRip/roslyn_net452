using System.Reflection.Metadata;
using System.Threading;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class ObsoleteAttributeHelpers
	{
		internal static void InitializeObsoleteDataFromMetadata(ref ObsoleteAttributeData data, EntityHandle token, PEModuleSymbol containingModule)
		{
			if (data == ObsoleteAttributeData.Uninitialized)
			{
				ObsoleteAttributeData obsoleteDataFromMetadata = GetObsoleteDataFromMetadata(token, containingModule);
				Interlocked.CompareExchange(ref data, obsoleteDataFromMetadata, ObsoleteAttributeData.Uninitialized);
			}
		}

		internal static ObsoleteAttributeData GetObsoleteDataFromMetadata(EntityHandle token, PEModuleSymbol containingModule)
		{
			return containingModule.Module.TryGetDeprecatedOrExperimentalOrObsoleteAttribute(token, new MetadataDecoder(containingModule), ignoreByRefLikeMarker: false);
		}

		private static ThreeState GetObsoleteContextState(Symbol symbol, bool forceComplete)
		{
			while ((object)symbol != null)
			{
				if (forceComplete)
				{
					symbol.ForceCompleteObsoleteAttribute();
				}
				ThreeState obsoleteState = symbol.ObsoleteState;
				if (obsoleteState != ThreeState.False)
				{
					return obsoleteState;
				}
				symbol = ((!SymbolExtensions.IsAccessor(symbol)) ? symbol.ContainingSymbol : ((MethodSymbol)symbol).AssociatedSymbol);
			}
			return ThreeState.False;
		}

		internal static ObsoleteDiagnosticKind GetObsoleteDiagnosticKind(Symbol context, Symbol symbol, bool forceComplete = false)
		{
			return symbol.ObsoleteKind switch
			{
				ObsoleteAttributeKind.None => ObsoleteDiagnosticKind.NotObsolete, 
				ObsoleteAttributeKind.Experimental => ObsoleteDiagnosticKind.Diagnostic, 
				ObsoleteAttributeKind.Uninitialized => ObsoleteDiagnosticKind.Lazy, 
				_ => GetObsoleteContextState(context, forceComplete) switch
				{
					ThreeState.False => ObsoleteDiagnosticKind.Diagnostic, 
					ThreeState.True => ObsoleteDiagnosticKind.Suppressed, 
					_ => ObsoleteDiagnosticKind.LazyPotentiallySuppressed, 
				}, 
			};
		}

		internal static DiagnosticInfo CreateObsoleteDiagnostic(Symbol symbol)
		{
			ObsoleteAttributeData obsoleteAttributeData = symbol.ObsoleteAttributeData;
			if (obsoleteAttributeData == null)
			{
				return null;
			}
			if (obsoleteAttributeData.Kind == ObsoleteAttributeKind.Experimental)
			{
				return ErrorFactory.ErrorInfo(ERRID.WRN_Experimental, new FormattedSymbol(symbol, SymbolDisplayFormat.VisualBasicErrorMessageFormat));
			}
			if (SymbolExtensions.IsAccessor(symbol) && ((MethodSymbol)symbol).AssociatedSymbol.Kind == SymbolKind.Property)
			{
				MethodSymbol methodSymbol = (MethodSymbol)symbol;
				string text = ((methodSymbol.MethodKind == MethodKind.PropertyGet) ? "Get" : "Set");
				if (string.IsNullOrEmpty(obsoleteAttributeData.Message))
				{
					return ErrorFactory.ObsoleteErrorInfo(obsoleteAttributeData.IsError ? ERRID.ERR_UseOfObsoletePropertyAccessor2 : ERRID.WRN_UseOfObsoletePropertyAccessor2, obsoleteAttributeData, text, methodSymbol.AssociatedSymbol);
				}
				return ErrorFactory.ObsoleteErrorInfo(obsoleteAttributeData.IsError ? ERRID.ERR_UseOfObsoletePropertyAccessor3 : ERRID.WRN_UseOfObsoletePropertyAccessor3, obsoleteAttributeData, text, methodSymbol.AssociatedSymbol, obsoleteAttributeData.Message);
			}
			if (string.IsNullOrEmpty(obsoleteAttributeData.Message))
			{
				return ErrorFactory.ObsoleteErrorInfo(obsoleteAttributeData.IsError ? ERRID.ERR_UseOfObsoleteSymbolNoMessage1 : ERRID.WRN_UseOfObsoleteSymbolNoMessage1, obsoleteAttributeData, symbol);
			}
			return ErrorFactory.ObsoleteErrorInfo(obsoleteAttributeData.IsError ? ERRID.ERR_UseOfObsoleteSymbol2 : ERRID.WRN_UseOfObsoleteSymbol2, obsoleteAttributeData, symbol, obsoleteAttributeData.Message);
		}
	}
}
