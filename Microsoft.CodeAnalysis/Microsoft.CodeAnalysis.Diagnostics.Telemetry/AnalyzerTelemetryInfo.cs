using System;
using System.Runtime.Serialization;

namespace Microsoft.CodeAnalysis.Diagnostics.Telemetry
{
    [DataContract]
    public sealed class AnalyzerTelemetryInfo
    {
        [DataMember(Order = 0)]
        public int CompilationStartActionsCount { get; set; }

        [DataMember(Order = 1)]
        public int CompilationEndActionsCount { get; set; }

        [DataMember(Order = 2)]
        public int CompilationActionsCount { get; set; }

        [DataMember(Order = 3)]
        public int SyntaxTreeActionsCount { get; set; }

        [DataMember(Order = 4)]
        public int AdditionalFileActionsCount { get; set; }

        [DataMember(Order = 5)]
        public int SemanticModelActionsCount { get; set; }

        [DataMember(Order = 6)]
        public int SymbolActionsCount { get; set; }

        [DataMember(Order = 7)]
        public int SymbolStartActionsCount { get; set; }

        [DataMember(Order = 8)]
        public int SymbolEndActionsCount { get; set; }

        [DataMember(Order = 9)]
        public int SyntaxNodeActionsCount { get; set; }

        [DataMember(Order = 10)]
        public int CodeBlockStartActionsCount { get; set; }

        [DataMember(Order = 11)]
        public int CodeBlockEndActionsCount { get; set; }

        [DataMember(Order = 12)]
        public int CodeBlockActionsCount { get; set; }

        [DataMember(Order = 13)]
        public int OperationActionsCount { get; set; }

        [DataMember(Order = 14)]
        public int OperationBlockStartActionsCount { get; set; }

        [DataMember(Order = 15)]
        public int OperationBlockEndActionsCount { get; set; }

        [DataMember(Order = 16)]
        public int OperationBlockActionsCount { get; set; }

        [DataMember(Order = 17)]
        public int SuppressionActionsCount { get; set; }

        [DataMember(Order = 18)]
        public TimeSpan ExecutionTime { get; set; } = TimeSpan.Zero;


        [DataMember(Order = 19)]
        public bool Concurrent { get; set; }

        internal AnalyzerTelemetryInfo(AnalyzerActionCounts actionCounts, int suppressionActionCounts, TimeSpan executionTime)
        {
            CompilationStartActionsCount = actionCounts.CompilationStartActionsCount;
            CompilationEndActionsCount = actionCounts.CompilationEndActionsCount;
            CompilationActionsCount = actionCounts.CompilationActionsCount;
            SyntaxTreeActionsCount = actionCounts.SyntaxTreeActionsCount;
            AdditionalFileActionsCount = actionCounts.AdditionalFileActionsCount;
            SemanticModelActionsCount = actionCounts.SemanticModelActionsCount;
            SymbolActionsCount = actionCounts.SymbolActionsCount;
            SymbolStartActionsCount = actionCounts.SymbolStartActionsCount;
            SymbolEndActionsCount = actionCounts.SymbolEndActionsCount;
            SyntaxNodeActionsCount = actionCounts.SyntaxNodeActionsCount;
            CodeBlockStartActionsCount = actionCounts.CodeBlockStartActionsCount;
            CodeBlockEndActionsCount = actionCounts.CodeBlockEndActionsCount;
            CodeBlockActionsCount = actionCounts.CodeBlockActionsCount;
            OperationActionsCount = actionCounts.OperationActionsCount;
            OperationBlockStartActionsCount = actionCounts.OperationBlockStartActionsCount;
            OperationBlockEndActionsCount = actionCounts.OperationBlockEndActionsCount;
            OperationBlockActionsCount = actionCounts.OperationBlockActionsCount;
            SuppressionActionsCount = suppressionActionCounts;
            ExecutionTime = executionTime;
            Concurrent = actionCounts.Concurrent;
        }

        public AnalyzerTelemetryInfo()
        {
        }
    }
}
