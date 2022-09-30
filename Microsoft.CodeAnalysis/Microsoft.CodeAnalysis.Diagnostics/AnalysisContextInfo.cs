using System.Text;

using Microsoft.CodeAnalysis.Text;

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    internal readonly struct AnalysisContextInfo
    {
        private readonly Compilation? _compilation;

        private readonly IOperation? _operation;

        private readonly ISymbol? _symbol;

        private readonly SourceOrAdditionalFile? _file;

        private readonly SyntaxNode? _node;

        public AnalysisContextInfo(Compilation compilation)
            : this(compilation, null, null, null, null)
        {
        }

        public AnalysisContextInfo(SemanticModel model)
            : this(model.Compilation, new SourceOrAdditionalFile(model.SyntaxTree))
        {
        }

        public AnalysisContextInfo(Compilation compilation, ISymbol symbol)
            : this(compilation, null, symbol, null, null)
        {
        }

        public AnalysisContextInfo(Compilation compilation, SourceOrAdditionalFile file)
            : this(compilation, null, null, file, null)
        {
        }

        public AnalysisContextInfo(Compilation compilation, SyntaxNode node)
            : this(compilation, null, null, new SourceOrAdditionalFile(node.SyntaxTree), node)
        {
        }

        public AnalysisContextInfo(Compilation compilation, IOperation operation)
            : this(compilation, operation, null, new SourceOrAdditionalFile(operation.Syntax.SyntaxTree), operation.Syntax)
        {
        }

        public AnalysisContextInfo(Compilation compilation, ISymbol symbol, SyntaxNode node)
            : this(compilation, null, symbol, new SourceOrAdditionalFile(node.SyntaxTree), node)
        {
        }

        private AnalysisContextInfo(Compilation? compilation, IOperation? operation, ISymbol? symbol, SourceOrAdditionalFile? file, SyntaxNode? node)
        {
            _compilation = compilation;
            _operation = operation;
            _symbol = symbol;
            _file = file;
            _node = node;
        }

        public string GetContext()
        {
            StringBuilder stringBuilder = new();
            if (_compilation?.AssemblyName != null)
            {
                stringBuilder.AppendLine("Compilation: " + _compilation!.AssemblyName);
            }
            if (_operation != null)
            {
                stringBuilder.AppendLine(string.Format("{0}: {1}", "IOperation", _operation!.Kind));
            }
            if (_symbol?.Name != null)
            {
                stringBuilder.AppendLine(string.Format("{0}: {1} ({2})", "ISymbol", _symbol!.Name, _symbol!.Kind));
            }
            if (_file.HasValue)
            {
                if (_file.Value.SourceTree != null)
                {
                    stringBuilder.AppendLine("SyntaxTree: " + _file.Value.SourceTree!.FilePath);
                }
                else
                {
                    stringBuilder.AppendLine("AdditionalText: " + _file.Value.AdditionalFile!.Path);
                }
            }
            if (_node != null)
            {
                LinePositionSpan? linePositionSpan = _file!.Value.SourceTree!.GetText()?.Lines?.GetLinePositionSpan(_node!.Span);
                stringBuilder.AppendLine(string.Format("{0}: {1} [{2}]@{3} {4}", "SyntaxNode", GetFlattenedNodeText(_node), _node!.GetType().Name, _node!.Span, linePositionSpan.HasValue ? linePositionSpan.Value.ToString() : string.Empty));
            }
            return stringBuilder.ToString();
        }

        private string GetFlattenedNodeText(SyntaxNode node)
        {
            int num = node.Span.Start;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (SyntaxToken item in node.DescendantTokens())
            {
                if (item.Span.Start - num > 0)
                {
                    stringBuilder.Append(" ");
                }
                stringBuilder.Append(item.ToString());
                num = item.Span.End;
                if (stringBuilder.Length > 30)
                {
                    break;
                }
            }
            return stringBuilder.ToString() + ((stringBuilder.Length > 30) ? " ..." : string.Empty);
        }
    }
}
