using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public abstract class CompilerDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        private class CompilationAnalyzer
        {
            private class CompilerDiagnostic : Diagnostic
            {
                private readonly Diagnostic _original;

                private readonly ImmutableDictionary<string, string?> _properties;

                public override DiagnosticDescriptor Descriptor => _original.Descriptor;

                public override int Code => _original.Code;

                public override IReadOnlyList<object?> Arguments => _original.Arguments;

                public override string Id => _original.Id;

                public override DiagnosticSeverity Severity => _original.Severity;

                public override int WarningLevel => _original.WarningLevel;

                public override Location Location => _original.Location;

                public override IReadOnlyList<Location> AdditionalLocations => _original.AdditionalLocations;

                public override bool IsSuppressed => _original.IsSuppressed;

                public override ImmutableDictionary<string, string?> Properties => _properties;

                public CompilerDiagnostic(Diagnostic original, ImmutableDictionary<string, string?> properties)
                {
                    _original = original;
                    _properties = properties;
                }

                public override string GetMessage(IFormatProvider? formatProvider = null)
                {
                    return _original.GetMessage(formatProvider);
                }

                public override bool Equals(object? obj)
                {
                    return _original.Equals(obj);
                }

                public override int GetHashCode()
                {
                    return _original.GetHashCode();
                }

                public override bool Equals(Diagnostic? obj)
                {
                    return _original.Equals(obj);
                }

                public override Diagnostic WithLocation(Location location)
                {
                    return new CompilerDiagnostic(_original.WithLocation(location), _properties);
                }

                public override Diagnostic WithSeverity(DiagnosticSeverity severity)
                {
                    return new CompilerDiagnostic(_original.WithSeverity(severity), _properties);
                }

                public override Diagnostic WithIsSuppressed(bool isSuppressed)
                {
                    return new CompilerDiagnostic(_original.WithIsSuppressed(isSuppressed), _properties);
                }
            }

            private readonly Compilation _compilation;

            public CompilationAnalyzer(Compilation compilation)
            {
                _compilation = compilation;
            }

            public void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
            {
                SemanticModel semanticModel = _compilation.GetSemanticModel(context.Tree);
                CancellationToken cancellationToken = context.CancellationToken;
                ReportDiagnostics(semanticModel.GetSyntaxDiagnostics(null, cancellationToken), context.ReportDiagnostic, IsSourceLocation, s_syntactic);
            }

            public static void AnalyzeSemanticModel(SemanticModelAnalysisContext context)
            {
                SemanticModel semanticModel = context.SemanticModel;
                CancellationToken cancellationToken = context.CancellationToken;
                ImmutableArray<Diagnostic> declarationDiagnostics = semanticModel.GetDeclarationDiagnostics(null, cancellationToken);
                SemanticModel semanticModel2 = context.SemanticModel;
                cancellationToken = context.CancellationToken;
                ImmutableArray<Diagnostic> methodBodyDiagnostics = semanticModel2.GetMethodBodyDiagnostics(null, cancellationToken);
                ReportDiagnostics(declarationDiagnostics, context.ReportDiagnostic, IsSourceLocation, s_declaration);
                ReportDiagnostics(methodBodyDiagnostics, context.ReportDiagnostic, IsSourceLocation);
            }

            public static void AnalyzeCompilation(CompilationAnalysisContext context)
            {
                ReportDiagnostics(context.Compilation.GetDeclarationDiagnostics(context.CancellationToken), context.ReportDiagnostic, (Location location) => !IsSourceLocation(location), s_declaration);
            }

            private static bool IsSourceLocation(Location location)
            {
                if (location != null)
                {
                    return location.Kind == LocationKind.SourceFile;
                }
                return false;
            }

            private static void ReportDiagnostics(ImmutableArray<Diagnostic> diagnostics, Action<Diagnostic> reportDiagnostic, Func<Location, bool> locationFilter, ImmutableDictionary<string, string?>? properties = null)
            {
                ImmutableArray<Diagnostic>.Enumerator enumerator = diagnostics.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Diagnostic current = enumerator.Current;
                    if (locationFilter(current.Location) && current.Severity != 0)
                    {
                        Diagnostic obj = ((properties == null) ? current : new CompilerDiagnostic(current, properties));
                        reportDiagnostic(obj);
                    }
                }
            }
        }

        private const string Origin = "Origin";

        private const string Syntactic = "Syntactic";

        private const string Declaration = "Declaration";

        private static readonly ImmutableDictionary<string, string?> s_syntactic = ImmutableDictionary<string, string>.Empty.Add("Origin", "Syntactic");

        private static readonly ImmutableDictionary<string, string?> s_declaration = ImmutableDictionary<string, string>.Empty.Add("Origin", "Declaration");

        public abstract CommonMessageProvider MessageProvider { get; }

        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                CommonMessageProvider messageProvider = MessageProvider;
                ImmutableArray<int> supportedErrorCodes = GetSupportedErrorCodes();
                ImmutableArray<DiagnosticDescriptor>.Builder builder = ImmutableArray.CreateBuilder<DiagnosticDescriptor>(supportedErrorCodes.Length);
                ImmutableArray<int>.Enumerator enumerator = supportedErrorCodes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DiagnosticDescriptor descriptor = DiagnosticInfo.GetDescriptor(enumerator.Current, messageProvider);
                    builder.Add(descriptor);
                }
                builder.Add(AnalyzerExecutor.GetAnalyzerExceptionDiagnosticDescriptor());
                return builder.ToImmutable();
            }
        }

        public abstract ImmutableArray<int> GetSupportedErrorCodes();

        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterCompilationStartAction(delegate (CompilationStartAnalysisContext c)
            {
                CompilationAnalyzer @object = new CompilationAnalyzer(c.Compilation);
                c.RegisterSyntaxTreeAction(@object.AnalyzeSyntaxTree);
                c.RegisterSemanticModelAction(CompilationAnalyzer.AnalyzeSemanticModel);
            });
        }
    }
}
