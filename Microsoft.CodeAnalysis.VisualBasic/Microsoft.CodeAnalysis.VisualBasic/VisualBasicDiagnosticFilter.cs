using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class VisualBasicDiagnosticFilter
	{
		private static readonly ERRID[] s_alinkWarnings = new ERRID[3]
		{
			ERRID.WRN_ConflictingMachineAssembly,
			ERRID.WRN_RefCultureMismatch,
			ERRID.WRN_InvalidVersionFormat
		};

		public static Diagnostic Filter(Diagnostic diagnostic, ReportDiagnostic generalDiagnosticOption, IDictionary<string, ReportDiagnostic> specificDiagnosticOptions, SyntaxTreeOptionsProvider syntaxTreeOptions, CancellationToken cancellationToken)
		{
			ImmutableDictionary<string, ReportDiagnostic> immutableDictionary = ImmutableDictionary.Create<string, ReportDiagnostic>(CaseInsensitiveComparison.Comparer).AddRange(specificDiagnosticOptions);
			if (diagnostic.Severity == (DiagnosticSeverity)(-2))
			{
				return null;
			}
			if (diagnostic.IsNotConfigurable())
			{
				if (diagnostic.IsEnabledByDefault)
				{
					return diagnostic;
				}
				return null;
			}
			bool hasDisableDirectiveSuppression = false;
			ReportDiagnostic reportAction = ((!s_alinkWarnings.Contains((ERRID)diagnostic.Code) || !immutableDictionary.Keys.Contains(MessageProvider.Instance.GetIdForErrorCode(40010))) ? GetDiagnosticReport(diagnostic.Severity, diagnostic.IsEnabledByDefault, diagnostic.Id, diagnostic.Location, diagnostic.Category, generalDiagnosticOption, immutableDictionary, syntaxTreeOptions, cancellationToken, out hasDisableDirectiveSuppression) : GetDiagnosticReport(MessageProvider.Instance.GetSeverity(40010), diagnostic.IsEnabledByDefault, MessageProvider.Instance.GetIdForErrorCode(40010), diagnostic.Location, diagnostic.Category, generalDiagnosticOption, immutableDictionary, syntaxTreeOptions, cancellationToken, out hasDisableDirectiveSuppression));
			if (hasDisableDirectiveSuppression)
			{
				diagnostic = diagnostic.WithIsSuppressed(isSuppressed: true);
			}
			return diagnostic.WithReportDiagnostic(reportAction);
		}

		internal static ReportDiagnostic GetDiagnosticReport(DiagnosticSeverity severity, bool isEnabledByDefault, string id, Location location, string category, ReportDiagnostic generalDiagnosticOption, IDictionary<string, ReportDiagnostic> caseInsensitiveSpecificDiagnosticOptions, SyntaxTreeOptionsProvider syntaxTreeOptions, CancellationToken cancellationToken, out bool hasDisableDirectiveSuppression)
		{
			hasDisableDirectiveSuppression = false;
			SyntaxTree syntaxTree = location?.SourceTree;
			bool flag = false;
			bool flag2 = false;
			if (caseInsensitiveSpecificDiagnosticOptions.TryGetValue(id, out var value))
			{
				flag = true;
				if (value == ReportDiagnostic.Default)
				{
					flag2 = true;
				}
			}
			if (syntaxTreeOptions != null && (!flag || flag2) && ((syntaxTree != null && syntaxTreeOptions.TryGetDiagnosticValue(syntaxTree, id, cancellationToken, out var severity2)) || syntaxTreeOptions.TryGetGlobalDiagnosticValue(id, cancellationToken, out severity2)) && (!flag2 || severity != DiagnosticSeverity.Warning || severity2 != ReportDiagnostic.Error))
			{
				flag = true;
				value = severity2;
				if (!flag2 && value == ReportDiagnostic.Warn && generalDiagnosticOption == ReportDiagnostic.Error)
				{
					value = ReportDiagnostic.Error;
				}
			}
			if (!flag)
			{
				value = ((!isEnabledByDefault) ? ReportDiagnostic.Suppress : ReportDiagnostic.Default);
			}
			if (value == ReportDiagnostic.Suppress)
			{
				return ReportDiagnostic.Suppress;
			}
			if ((object)location != null && location.SourceTree != null && VisualBasicExtensions.GetWarningState(location.SourceTree, id, location.SourceSpan.Start) == ReportDiagnostic.Suppress)
			{
				hasDisableDirectiveSuppression = true;
			}
			if (generalDiagnosticOption == ReportDiagnostic.Suppress && (severity == DiagnosticSeverity.Warning || severity == DiagnosticSeverity.Info))
			{
				return ReportDiagnostic.Suppress;
			}
			if (generalDiagnosticOption == ReportDiagnostic.Error && severity == DiagnosticSeverity.Warning && !flag && value == ReportDiagnostic.Default)
			{
				return ReportDiagnostic.Error;
			}
			return value;
		}
	}
}
