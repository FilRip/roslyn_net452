using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class ErrorFactory
	{
		private const string s_titleSuffix = "_Title";

		private const string s_descriptionSuffix = "_Description";

		private static readonly Lazy<ImmutableDictionary<ERRID, string>> s_categoriesMap = new Lazy<ImmutableDictionary<ERRID, string>>(CreateCategoriesMap);

		public static readonly DiagnosticInfo VoidDiagnosticInfo = ErrorInfo(ERRID.Void);

		public static readonly Func<DiagnosticInfo> GetErrorInfo_ERR_WithEventsRequiresClass = () => ErrorInfo(ERRID.ERR_WithEventsRequiresClass);

		public static readonly Func<DiagnosticInfo> GetErrorInfo_ERR_StrictDisallowImplicitObject = () => ErrorInfo(ERRID.ERR_StrictDisallowImplicitObject);

		public static readonly Func<DiagnosticInfo> GetErrorInfo_WRN_ObjectAssumedVar1_WRN_StaticLocalNoInference = () => ErrorInfo(ERRID.WRN_ObjectAssumedVar1, ErrorInfo(ERRID.WRN_StaticLocalNoInference));

		public static readonly Func<DiagnosticInfo> GetErrorInfo_WRN_ObjectAssumedVar1_WRN_MissingAsClauseinVarDecl = () => ErrorInfo(ERRID.WRN_ObjectAssumedVar1, ErrorInfo(ERRID.WRN_MissingAsClauseinVarDecl));

		public static readonly Func<DiagnosticInfo> GetErrorInfo_ERR_StrictDisallowsImplicitProc = () => ErrorInfo(ERRID.ERR_StrictDisallowsImplicitProc);

		public static readonly Func<DiagnosticInfo> GetErrorInfo_ERR_StrictDisallowsImplicitArgs = () => ErrorInfo(ERRID.ERR_StrictDisallowsImplicitArgs);

		public static readonly Func<DiagnosticInfo> GetErrorInfo_WRN_ObjectAssumed1_WRN_MissingAsClauseinFunction = () => ErrorInfo(ERRID.WRN_ObjectAssumed1, ErrorInfo(ERRID.WRN_MissingAsClauseinFunction));

		public static readonly Func<DiagnosticInfo> GetErrorInfo_WRN_ObjectAssumed1_WRN_MissingAsClauseinOperator = () => ErrorInfo(ERRID.WRN_ObjectAssumed1, ErrorInfo(ERRID.WRN_MissingAsClauseinOperator));

		public static readonly Func<DiagnosticInfo> GetErrorInfo_WRN_ObjectAssumedProperty1_WRN_MissingAsClauseinProperty = () => ErrorInfo(ERRID.WRN_ObjectAssumedProperty1, ErrorInfo(ERRID.WRN_MissingAsClauseinProperty));

		private static ResourceManager s_resourceManager;

		internal static ResourceManager ResourceManager
		{
			get
			{
				if (s_resourceManager == null)
				{
					s_resourceManager = new ResourceManager(typeof(VBResources).FullName, typeof(ERRID).GetTypeInfo().Assembly);
				}
				return s_resourceManager;
			}
		}

		private static ImmutableDictionary<ERRID, string> CreateCategoriesMap()
		{
			return new Dictionary<ERRID, string>().ToImmutableDictionary();
		}

		public static DiagnosticInfo ErrorInfo(ERRID id)
		{
			return new DiagnosticInfo(MessageProvider.Instance, (int)id);
		}

		public static DiagnosticInfo ErrorInfo(ERRID id, params object[] arguments)
		{
			return new DiagnosticInfo(MessageProvider.Instance, (int)id, arguments);
		}

		public static CustomObsoleteDiagnosticInfo ObsoleteErrorInfo(ERRID id, ObsoleteAttributeData data, params object[] arguments)
		{
			return new CustomObsoleteDiagnosticInfo(MessageProvider.Instance, (int)id, data, arguments);
		}

		public static DiagnosticInfo ErrorInfo(ERRID id, ref SyntaxToken syntaxToken)
		{
			return ErrorInfo(id, SyntaxFacts.GetText(VisualBasicExtensions.Kind(syntaxToken)));
		}

		public static DiagnosticInfo ErrorInfo(ERRID id, ref SyntaxKind syntaxTokenKind)
		{
			return ErrorInfo(id, SyntaxFacts.GetText(syntaxTokenKind));
		}

		public static DiagnosticInfo ErrorInfo(ERRID id, ref SyntaxToken syntaxToken, TypeSymbol type)
		{
			return ErrorInfo(id, SyntaxFacts.GetText(VisualBasicExtensions.Kind(syntaxToken)), type);
		}

		public static DiagnosticInfo ErrorInfo(ERRID id, ref SyntaxToken syntaxToken, TypeSymbol type1, TypeSymbol type2)
		{
			return ErrorInfo(id, SyntaxFacts.GetText(VisualBasicExtensions.Kind(syntaxToken)), type1, type2);
		}

		internal static string IdToString(ERRID id)
		{
			return IdToString(id, CultureInfo.CurrentUICulture);
		}

		public static string IdToString(ERRID id, CultureInfo language)
		{
			return ResourceManager.GetString(id.ToString(), language);
		}

		public static LocalizableResourceString GetMessageFormat(ERRID id)
		{
			return new LocalizableResourceString(id.ToString(), ResourceManager, typeof(ErrorFactory));
		}

		public static LocalizableResourceString GetTitle(ERRID id)
		{
			return new LocalizableResourceString(id.ToString() + "_Title", ResourceManager, typeof(ErrorFactory));
		}

		public static LocalizableResourceString GetDescription(ERRID id)
		{
			return new LocalizableResourceString(id.ToString() + "_Description", ResourceManager, typeof(ErrorFactory));
		}

		public static string GetHelpLink(ERRID id)
		{
			string idForErrorCode = MessageProvider.Instance.GetIdForErrorCode((int)id);
			return $"https://msdn.microsoft.com/query/roslyn.query?appId=roslyn&k=k({idForErrorCode})";
		}

		public static string GetCategory(ERRID id)
		{
			string value = null;
			if (s_categoriesMap.Value.TryGetValue(id, out value))
			{
				return value;
			}
			return "Compiler";
		}
	}
}
