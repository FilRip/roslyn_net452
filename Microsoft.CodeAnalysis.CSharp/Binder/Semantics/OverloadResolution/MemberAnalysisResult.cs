// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal struct MemberAnalysisResult
    {
        // put these first for better packing
        public readonly ImmutableArray<Conversion> ConversionsOpt;
        public readonly ImmutableArray<int> BadArgumentsOpt;
        public readonly ImmutableArray<int> ArgsToParamsOpt;
        public readonly ImmutableArray<TypeParameterDiagnosticInfo> ConstraintFailureDiagnostics;

        public readonly int BadParameter;
        public readonly MemberResolutionKind Kind;

        /// <summary>
        /// Omit ref feature for COM interop: We can pass arguments by value for ref parameters if we are invoking a method/property on an instance of a COM imported type.
        /// This property returns a flag indicating whether we had any ref omitted argument for the given call.
        /// </summary>
        public readonly bool HasAnyRefOmittedArgument;

        private MemberAnalysisResult(
            MemberResolutionKind kind,
            ImmutableArray<int> badArgumentsOpt = default,
            ImmutableArray<int> argsToParamsOpt = default,
            ImmutableArray<Conversion> conversionsOpt = default,
            int missingParameter = -1,
            bool hasAnyRefOmittedArgument = false,
            ImmutableArray<TypeParameterDiagnosticInfo> constraintFailureDiagnosticsOpt = default)
        {
            this.Kind = kind;
            this.BadArgumentsOpt = badArgumentsOpt;
            this.ArgsToParamsOpt = argsToParamsOpt;
            this.ConversionsOpt = conversionsOpt;
            this.BadParameter = missingParameter;
            this.HasAnyRefOmittedArgument = hasAnyRefOmittedArgument;
            this.ConstraintFailureDiagnostics = constraintFailureDiagnosticsOpt.NullToEmpty();
        }

        public override bool Equals(object obj)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override int GetHashCode()
        {
            throw ExceptionUtilities.Unreachable;
        }

        public Conversion ConversionForArg(int arg)
        {
            if (this.ConversionsOpt.IsDefault)
            {
                return Conversion.Identity;
            }

            return this.ConversionsOpt[arg];
        }

        public int ParameterFromArgument(int arg)
        {
            if (ArgsToParamsOpt.IsDefault)
            {
                return arg;
            }
            return ArgsToParamsOpt[arg];
        }

        // A method may be applicable, but worse than another method.
        public bool IsApplicable
        {
            get
            {
                return this.Kind switch
                {
                    MemberResolutionKind.ApplicableInNormalForm or MemberResolutionKind.ApplicableInExpandedForm or MemberResolutionKind.Worse or MemberResolutionKind.Worst => true,
                    _ => false,
                };
            }
        }

        public bool IsValid
        {
            get
            {
                return this.Kind switch
                {
                    MemberResolutionKind.ApplicableInNormalForm or MemberResolutionKind.ApplicableInExpandedForm => true,
                    _ => false,
                };
            }
        }

        /// <remarks>
        /// Returns false for <see cref="MemberResolutionKind.UnsupportedMetadata"/>
        /// because those diagnostics are only reported if no other candidates are
        /// available.
        /// </remarks>
        internal bool HasUseSiteDiagnosticToReportFor(Symbol symbol)
        {
            // There is a use site diagnostic to report here, but it is not reported
            // just because this member was a candidate - only if it "wins".
            return !SuppressUseSiteDiagnosticsForKind(this.Kind) &&
                symbol is object && symbol.GetUseSiteInfo().DiagnosticInfo != null;
        }

        private static bool SuppressUseSiteDiagnosticsForKind(MemberResolutionKind kind)
        {
            return kind switch
            {
                MemberResolutionKind.UnsupportedMetadata => true,
                MemberResolutionKind.NoCorrespondingParameter or MemberResolutionKind.NoCorrespondingNamedParameter or MemberResolutionKind.DuplicateNamedArgument or MemberResolutionKind.NameUsedForPositional or MemberResolutionKind.RequiredParameterMissing or MemberResolutionKind.LessDerived => true,// Dev12 checks all of these things before considering use site diagnostics.
                                                                                                                                                                                                                                                                                                              // That is, use site diagnostics are suppressed for candidates rejected for these reasons.
                _ => false,
            };
        }

        public static MemberAnalysisResult ArgumentParameterMismatch(ArgumentAnalysisResult argAnalysis)
        {
            return argAnalysis.Kind switch
            {
                ArgumentAnalysisResultKind.NoCorrespondingParameter => NoCorrespondingParameter(argAnalysis.ArgumentPosition),
                ArgumentAnalysisResultKind.NoCorrespondingNamedParameter => NoCorrespondingNamedParameter(argAnalysis.ArgumentPosition),
                ArgumentAnalysisResultKind.DuplicateNamedArgument => DuplicateNamedArgument(argAnalysis.ArgumentPosition),
                ArgumentAnalysisResultKind.RequiredParameterMissing => RequiredParameterMissing(argAnalysis.ParameterPosition),
                ArgumentAnalysisResultKind.NameUsedForPositional => NameUsedForPositional(argAnalysis.ArgumentPosition),
                ArgumentAnalysisResultKind.BadNonTrailingNamedArgument => BadNonTrailingNamedArgument(argAnalysis.ArgumentPosition),
                _ => throw ExceptionUtilities.UnexpectedValue(argAnalysis.Kind),
            };
        }

        public static MemberAnalysisResult NameUsedForPositional(int argumentPosition)
        {
            return new MemberAnalysisResult(
                MemberResolutionKind.NameUsedForPositional,
                badArgumentsOpt: ImmutableArray.Create<int>(argumentPosition));
        }

        public static MemberAnalysisResult BadNonTrailingNamedArgument(int argumentPosition)
        {
            return new MemberAnalysisResult(
                MemberResolutionKind.BadNonTrailingNamedArgument,
                badArgumentsOpt: ImmutableArray.Create<int>(argumentPosition));
        }

        public static MemberAnalysisResult NoCorrespondingParameter(int argumentPosition)
        {
            return new MemberAnalysisResult(
                MemberResolutionKind.NoCorrespondingParameter,
                badArgumentsOpt: ImmutableArray.Create<int>(argumentPosition));
        }

        public static MemberAnalysisResult NoCorrespondingNamedParameter(int argumentPosition)
        {
            return new MemberAnalysisResult(
                MemberResolutionKind.NoCorrespondingNamedParameter,
                badArgumentsOpt: ImmutableArray.Create<int>(argumentPosition));
        }

        public static MemberAnalysisResult DuplicateNamedArgument(int argumentPosition)
        {
            return new MemberAnalysisResult(
                MemberResolutionKind.DuplicateNamedArgument,
                badArgumentsOpt: ImmutableArray.Create<int>(argumentPosition));
        }

        public static MemberAnalysisResult RequiredParameterMissing(int parameterPosition)
        {
            return new MemberAnalysisResult(
                MemberResolutionKind.RequiredParameterMissing,
                missingParameter: parameterPosition);
        }

        public static MemberAnalysisResult UseSiteError()
        {
            return new MemberAnalysisResult(MemberResolutionKind.UseSiteError);
        }

        public static MemberAnalysisResult UnsupportedMetadata()
        {
            return new MemberAnalysisResult(MemberResolutionKind.UnsupportedMetadata);
        }

        public static MemberAnalysisResult BadArgumentConversions(ImmutableArray<int> argsToParamsOpt, ImmutableArray<int> badArguments, ImmutableArray<Conversion> conversions)
        {
            return new MemberAnalysisResult(
                MemberResolutionKind.BadArgumentConversion,
                badArguments,
                argsToParamsOpt,
                conversions);
        }

        public static MemberAnalysisResult InaccessibleTypeArgument()
        {
            return new MemberAnalysisResult(MemberResolutionKind.InaccessibleTypeArgument);
        }

        public static MemberAnalysisResult TypeInferenceFailed()
        {
            return new MemberAnalysisResult(MemberResolutionKind.TypeInferenceFailed);
        }

        public static MemberAnalysisResult TypeInferenceExtensionInstanceArgumentFailed()
        {
            return new MemberAnalysisResult(MemberResolutionKind.TypeInferenceExtensionInstanceArgument);
        }

        public static MemberAnalysisResult StaticInstanceMismatch()
        {
            return new MemberAnalysisResult(MemberResolutionKind.StaticInstanceMismatch);
        }

        public static MemberAnalysisResult ConstructedParameterFailedConstraintsCheck(int parameterPosition)
        {
            return new MemberAnalysisResult(
                MemberResolutionKind.ConstructedParameterFailedConstraintCheck,
                missingParameter: parameterPosition);
        }

        public static MemberAnalysisResult WrongRefKind()
        {
            return new MemberAnalysisResult(MemberResolutionKind.WrongRefKind);
        }

        public static MemberAnalysisResult WrongReturnType()
        {
            return new MemberAnalysisResult(MemberResolutionKind.WrongReturnType);
        }

        public static MemberAnalysisResult LessDerived()
        {
            return new MemberAnalysisResult(MemberResolutionKind.LessDerived);
        }

        public static MemberAnalysisResult NormalForm(ImmutableArray<int> argsToParamsOpt, ImmutableArray<Conversion> conversions, bool hasAnyRefOmittedArgument)
        {
            return new MemberAnalysisResult(MemberResolutionKind.ApplicableInNormalForm, default, argsToParamsOpt, conversions, hasAnyRefOmittedArgument: hasAnyRefOmittedArgument);
        }

        public static MemberAnalysisResult ExpandedForm(ImmutableArray<int> argsToParamsOpt, ImmutableArray<Conversion> conversions, bool hasAnyRefOmittedArgument)
        {
            return new MemberAnalysisResult(MemberResolutionKind.ApplicableInExpandedForm, default, argsToParamsOpt, conversions, hasAnyRefOmittedArgument: hasAnyRefOmittedArgument);
        }

        public static MemberAnalysisResult Worse()
        {
            return new MemberAnalysisResult(MemberResolutionKind.Worse);
        }

        public static MemberAnalysisResult Worst()
        {
            return new MemberAnalysisResult(MemberResolutionKind.Worst);
        }

        internal static MemberAnalysisResult ConstraintFailure(ImmutableArray<TypeParameterDiagnosticInfo> constraintFailureDiagnostics)
        {
            return new MemberAnalysisResult(MemberResolutionKind.ConstraintFailure, constraintFailureDiagnosticsOpt: constraintFailureDiagnostics);
        }

        internal static MemberAnalysisResult WrongCallingConvention()
        {
            return new MemberAnalysisResult(MemberResolutionKind.WrongCallingConvention);
        }
    }
}
