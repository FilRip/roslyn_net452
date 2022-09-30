using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis
{
	[StandardModule]
	internal sealed class MissingRuntimeMemberDiagnosticHelper
	{
		internal const string MyVBNamespace = "My";

		private static readonly Dictionary<string, string> s_metadataNames = new Dictionary<string, string>
		{
			{ "Microsoft.VisualBasic.CompilerServices.Operators", "Late binding" },
			{ "Microsoft.VisualBasic.CompilerServices.NewLateBinding", "Late binding" },
			{ "Microsoft.VisualBasic.CompilerServices.LikeOperator", "Like operator" },
			{ "Microsoft.VisualBasic.CompilerServices.ProjectData", "Unstructured exception handling" },
			{ "Microsoft.VisualBasic.CompilerServices.ProjectData.CreateProjectError", "Unstructured exception handling" }
		};

		internal static DiagnosticInfo GetDiagnosticForMissingRuntimeHelper(string typename, string membername, bool embedVBCoreRuntime)
		{
			string value = "";
			s_metadataNames.TryGetValue(typename, out value);
			if (embedVBCoreRuntime && !string.IsNullOrEmpty(value))
			{
				return ErrorFactory.ErrorInfo(ERRID.ERR_PlatformDoesntSupport, value);
			}
			return ErrorFactory.ErrorInfo(ERRID.ERR_MissingRuntimeHelper, typename + "." + membername);
		}
	}
}
