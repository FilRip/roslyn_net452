using System.Reflection.Metadata;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class ObsoleteAttributeHelpers
    {
        internal static void InitializeObsoleteDataFromMetadata(ref ObsoleteAttributeData data, EntityHandle token, PEModuleSymbol containingModule, bool ignoreByRefLikeMarker)
        {
            if (data == ObsoleteAttributeData.Uninitialized)
            {
                ObsoleteAttributeData obsoleteDataFromMetadata = GetObsoleteDataFromMetadata(token, containingModule, ignoreByRefLikeMarker);
                Interlocked.CompareExchange(ref data, obsoleteDataFromMetadata, ObsoleteAttributeData.Uninitialized);
            }
        }

        internal static ObsoleteAttributeData GetObsoleteDataFromMetadata(EntityHandle token, PEModuleSymbol containingModule, bool ignoreByRefLikeMarker)
        {
            return containingModule.Module.TryGetDeprecatedOrExperimentalOrObsoleteAttribute(token, new MetadataDecoder(containingModule), ignoreByRefLikeMarker);
        }

        private static ThreeState GetObsoleteContextState(Symbol symbol, bool forceComplete)
        {
            while ((object)symbol != null)
            {
                if (symbol.Kind == SymbolKind.Field)
                {
                    Symbol associatedSymbol = ((FieldSymbol)symbol).AssociatedSymbol;
                    if ((object)associatedSymbol != null)
                    {
                        symbol = associatedSymbol;
                    }
                }
                if (forceComplete)
                {
                    symbol.ForceCompleteObsoleteAttribute();
                }
                ThreeState obsoleteState = symbol.ObsoleteState;
                if (obsoleteState != ThreeState.False)
                {
                    return obsoleteState;
                }
                symbol = ((!symbol.IsAccessor()) ? symbol.ContainingSymbol : ((MethodSymbol)symbol).AssociatedSymbol);
            }
            return ThreeState.False;
        }

        internal static ObsoleteDiagnosticKind GetObsoleteDiagnosticKind(Symbol symbol, Symbol containingMember, bool forceComplete = false)
        {
            return symbol.ObsoleteKind switch
            {
                ObsoleteAttributeKind.None => ObsoleteDiagnosticKind.NotObsolete,
                ObsoleteAttributeKind.Experimental => ObsoleteDiagnosticKind.Diagnostic,
                ObsoleteAttributeKind.Uninitialized => ObsoleteDiagnosticKind.Lazy,
                _ => GetObsoleteContextState(containingMember, forceComplete) switch
                {
                    ThreeState.False => ObsoleteDiagnosticKind.Diagnostic,
                    ThreeState.True => ObsoleteDiagnosticKind.Suppressed,
                    _ => ObsoleteDiagnosticKind.LazyPotentiallySuppressed,
                },
            };
        }

        internal static DiagnosticInfo CreateObsoleteDiagnostic(Symbol symbol, BinderFlags location)
        {
            ObsoleteAttributeData obsoleteAttributeData = symbol.ObsoleteAttributeData;
            if (obsoleteAttributeData == null)
            {
                return null;
            }
            if (location.Includes(BinderFlags.SuppressObsoleteChecks))
            {
                return null;
            }
            if (obsoleteAttributeData.Kind == ObsoleteAttributeKind.Experimental)
            {
                return new CSDiagnosticInfo(ErrorCode.WRN_Experimental, new FormattedSymbol(symbol, SymbolDisplayFormat.CSharpErrorMessageFormat));
            }
            bool flag = location.Includes(BinderFlags.CollectionInitializerAddMethod);
            string? message = obsoleteAttributeData.Message;
            bool isError = obsoleteAttributeData.IsError;
            ErrorCode errorCode = ((message == null) ? ((!flag) ? ErrorCode.WRN_DeprecatedSymbol : ErrorCode.WRN_DeprecatedCollectionInitAdd) : (isError ? ((!flag) ? ErrorCode.ERR_DeprecatedSymbolStr : ErrorCode.ERR_DeprecatedCollectionInitAddStr) : ((!flag) ? ErrorCode.WRN_DeprecatedSymbolStr : ErrorCode.WRN_DeprecatedCollectionInitAddStr)));
            ErrorCode errorCode2 = errorCode;
            string message2 = obsoleteAttributeData.Message;
            object[] arguments = ((message2 == null) ? new object[1] { symbol } : new object[2] { symbol, message2 });
            return new CustomObsoleteDiagnosticInfo(MessageProvider.Instance, (int)errorCode2, obsoleteAttributeData, arguments);
        }
    }
}
