using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class CSharpDiagnosticFilter
    {
        private static readonly ErrorCode[] s_alinkWarnings = new ErrorCode[3]
        {
            ErrorCode.WRN_ConflictingMachineAssembly,
            ErrorCode.WRN_RefCultureMismatch,
            ErrorCode.WRN_InvalidVersionFormat
        };

        internal static Diagnostic? Filter(Diagnostic d, int warningLevelOption, NullableContextOptions nullableOption, ReportDiagnostic generalDiagnosticOption, IDictionary<string, ReportDiagnostic> specificDiagnosticOptions, SyntaxTreeOptionsProvider? syntaxTreeOptions, CancellationToken cancellationToken)
        {
            if (d == null)
            {
                return d;
            }
            else if (d.IsNotConfigurable())
            {
                if (d.IsEnabledByDefault)
                {
                    // Enabled NotConfigurable should always be reported as it is.
                    return d;
                }
                else
                {
                    // Disabled NotConfigurable should never be reported.
                    return null;
                }
            }
            else if (d.Severity == InternalDiagnosticSeverity.Void)
            {
                return null;
            }

            //In the native compiler, all warnings originating from alink.dll were issued
            //under the id WRN_ALinkWarn - 1607. If a customer used nowarn:1607 they would get
            //none of those warnings. In Roslyn, we've given each of these warnings their
            //own number, so that they may be configured independently. To preserve compatibility
            //if a user has specifically configured 1607 and we are reporting one of the alink warnings, use
            //the configuration specified for 1607. As implemented, this could result in customers 
            //specifying warnaserror:1607 and getting a message saying "warning as error CS8012..."
            //We don't permit configuring 1607 and independently configuring the new warnings.
            ReportDiagnostic reportAction;
            bool hasPragmaSuppression;
            if (s_alinkWarnings.Contains((ErrorCode)d.Code) &&
                specificDiagnosticOptions.Keys.Contains(CSharp.MessageProvider.Instance.GetIdForErrorCode((int)ErrorCode.WRN_ALinkWarn)))
            {
                reportAction = GetDiagnosticReport(ErrorFacts.GetSeverity(ErrorCode.WRN_ALinkWarn),
                    d.IsEnabledByDefault,
                    CSharp.MessageProvider.Instance.GetIdForErrorCode((int)ErrorCode.WRN_ALinkWarn),
                    ErrorFacts.GetWarningLevel(ErrorCode.WRN_ALinkWarn),
                    d.Location,
                    d.Category,
                    warningLevelOption,
                    nullableOption,
                    generalDiagnosticOption,
                    specificDiagnosticOptions,
                    syntaxTreeOptions,
                    cancellationToken,
                    out hasPragmaSuppression);
            }
            else
            {
                reportAction = GetDiagnosticReport(d.Severity,
                    d.IsEnabledByDefault,
                    d.Id,
                    d.WarningLevel,
                    d.Location,
                    d.Category,
                    warningLevelOption,
                    nullableOption,
                    generalDiagnosticOption,
                    specificDiagnosticOptions,
                    syntaxTreeOptions,
                    cancellationToken,
                    out hasPragmaSuppression);
            }

            if (hasPragmaSuppression)
            {
                d = d.WithIsSuppressed(true);
            }

            return d.WithReportDiagnostic(reportAction);
        }

        internal static ReportDiagnostic GetDiagnosticReport(DiagnosticSeverity severity, bool isEnabledByDefault, string id, int diagnosticWarningLevel, Location location, string category, int warningLevelOption, NullableContextOptions nullableOption, ReportDiagnostic generalDiagnosticOption, IDictionary<string, ReportDiagnostic> specificDiagnosticOptions, SyntaxTreeOptionsProvider? syntaxTreeOptions, CancellationToken cancellationToken, out bool hasPragmaSuppression)
        {
            hasPragmaSuppression = false;
            CSharpSyntaxTree cSharpSyntaxTree = location.SourceTree as CSharpSyntaxTree;
            int start = location.SourceSpan.Start;
            if (ErrorFacts.NullableWarnings.Contains(id))
            {
                NullableContextState.State? state = cSharpSyntaxTree?.GetNullableContextState(start).WarningsState;
                if (state switch
                {
                    NullableContextState.State.Enabled => 1,
                    NullableContextState.State.Disabled => 0,
                    NullableContextState.State.ExplicitlyRestored => nullableOption.WarningsEnabled() ? 1 : 0,
                    NullableContextState.State.Unknown => ((cSharpSyntaxTree == null || !cSharpSyntaxTree.IsGeneratedCode(syntaxTreeOptions, cancellationToken)) && nullableOption.WarningsEnabled()) ? 1 : 0,
                    null => nullableOption.WarningsEnabled() ? 1 : 0,
                    _ => throw ExceptionUtilities.UnexpectedValue(state),
                } == 0)
                {
                    return ReportDiagnostic.Suppress;
                }
            }
            if (diagnosticWarningLevel > warningLevelOption)
            {
                return ReportDiagnostic.Suppress;
            }
            bool isSpecified = false;
            bool flag = false;
            if (specificDiagnosticOptions.TryGetValue(id, out var value))
            {
                isSpecified = true;
                if (value == ReportDiagnostic.Default)
                {
                    flag = true;
                }
            }
            if (syntaxTreeOptions != null && (!isSpecified || flag) && ((cSharpSyntaxTree != null && syntaxTreeOptions!.TryGetDiagnosticValue(cSharpSyntaxTree, id, cancellationToken, out var severity2)) || syntaxTreeOptions!.TryGetGlobalDiagnosticValue(id, cancellationToken, out severity2)) && (!flag || severity != DiagnosticSeverity.Warning || severity2 != ReportDiagnostic.Error))
            {
                isSpecified = true;
                value = severity2;
                if (!flag && value == ReportDiagnostic.Warn && generalDiagnosticOption == ReportDiagnostic.Error)
                {
                    value = ReportDiagnostic.Error;
                }
            }
            if (!isSpecified)
            {
                value = ((!isEnabledByDefault) ? ReportDiagnostic.Suppress : ReportDiagnostic.Default);
            }
            if (value == ReportDiagnostic.Suppress)
            {
                return ReportDiagnostic.Suppress;
            }
            PragmaWarningState num = cSharpSyntaxTree?.GetPragmaDirectiveWarningState(id, start) ?? PragmaWarningState.Default;
            if (num == PragmaWarningState.Disabled)
            {
                hasPragmaSuppression = true;
            }
            if (num == PragmaWarningState.Enabled)
            {
                switch (value)
                {
                    case ReportDiagnostic.Error:
                    case ReportDiagnostic.Warn:
                    case ReportDiagnostic.Info:
                    case ReportDiagnostic.Hidden:
                        return value;
                    case ReportDiagnostic.Suppress:
                        return ReportDiagnostic.Default;
                    case ReportDiagnostic.Default:
                        if (generalDiagnosticOption == ReportDiagnostic.Error && promoteToAnError())
                        {
                            return ReportDiagnostic.Error;
                        }
                        return ReportDiagnostic.Default;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(value);
                }
            }
            switch (value)
            {
                case ReportDiagnostic.Suppress:
                    return ReportDiagnostic.Suppress;
                case ReportDiagnostic.Default:
                    switch (generalDiagnosticOption)
                    {
                        case ReportDiagnostic.Error:
                            if (promoteToAnError())
                            {
                                return ReportDiagnostic.Error;
                            }
                            break;
                        case ReportDiagnostic.Suppress:
                            if (severity == DiagnosticSeverity.Warning || severity == DiagnosticSeverity.Info)
                            {
                                value = ReportDiagnostic.Suppress;
                                isSpecified = true;
                            }
                            break;
                    }
                    break;
            }
            return value;
            bool promoteToAnError()
            {
                if (severity == DiagnosticSeverity.Warning)
                {
                    return !isSpecified;
                }
                return false;
            }
        }
    }
}
