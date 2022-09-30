using System;
using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.Operations;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.FlowAnalysis
{
    public sealed class ControlFlowGraph
    {
        private readonly ControlFlowGraphBuilder.CaptureIdDispenser _captureIdDispenser;

        private readonly ImmutableDictionary<IMethodSymbol, (ControlFlowRegion region, ILocalFunctionOperation operation, int ordinal)> _localFunctionsMap;

        private ControlFlowGraph?[]? _lazyLocalFunctionsGraphs;

        private readonly ImmutableDictionary<IFlowAnonymousFunctionOperation, (ControlFlowRegion region, int ordinal)> _anonymousFunctionsMap;

        private ControlFlowGraph?[]? _lazyAnonymousFunctionsGraphs;

        public IOperation OriginalOperation { get; }

        public ControlFlowGraph? Parent { get; }

        public ImmutableArray<BasicBlock> Blocks { get; }

        public ControlFlowRegion Root { get; }

        public ImmutableArray<IMethodSymbol> LocalFunctions { get; }

        internal ControlFlowGraph(IOperation originalOperation, ControlFlowGraph? parent, ControlFlowGraphBuilder.CaptureIdDispenser captureIdDispenser, ImmutableArray<BasicBlock> blocks, ControlFlowRegion root, ImmutableArray<IMethodSymbol> localFunctions, ImmutableDictionary<IMethodSymbol, (ControlFlowRegion region, ILocalFunctionOperation operation, int ordinal)> localFunctionsMap, ImmutableDictionary<IFlowAnonymousFunctionOperation, (ControlFlowRegion region, int ordinal)> anonymousFunctionsMap)
        {
            OriginalOperation = originalOperation;
            Parent = parent;
            Blocks = blocks;
            Root = root;
            LocalFunctions = localFunctions;
            _localFunctionsMap = localFunctionsMap;
            _anonymousFunctionsMap = anonymousFunctionsMap;
            _captureIdDispenser = captureIdDispenser;
        }

        public static ControlFlowGraph? Create(SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (semanticModel == null)
            {
                throw new ArgumentNullException("semanticModel");
            }
            IOperation operation = semanticModel.GetOperation(node, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (operation != null)
            {
                return CreateCore(operation, "operation", cancellationToken);
            }
            return null;
        }

        public static ControlFlowGraph Create(IBlockOperation body, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CreateCore(body, "body", cancellationToken);
        }

        public static ControlFlowGraph Create(IFieldInitializerOperation initializer, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CreateCore(initializer, "initializer", cancellationToken);
        }

        public static ControlFlowGraph Create(IPropertyInitializerOperation initializer, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CreateCore(initializer, "initializer", cancellationToken);
        }

        public static ControlFlowGraph Create(IParameterInitializerOperation initializer, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CreateCore(initializer, "initializer", cancellationToken);
        }

        public static ControlFlowGraph Create(IConstructorBodyOperation constructorBody, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CreateCore(constructorBody, "constructorBody", cancellationToken);
        }

        public static ControlFlowGraph Create(IMethodBodyOperation methodBody, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CreateCore(methodBody, "methodBody", cancellationToken);
        }

        internal static ControlFlowGraph CreateCore(IOperation operation, string argumentNameForException, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (operation == null)
            {
                throw new ArgumentNullException(argumentNameForException);
            }
            if (operation.Parent != null)
            {
                throw new ArgumentException(CodeAnalysisResources.NotARootOperation, argumentNameForException);
            }
            if (((Operation)operation).OwningSemanticModel == null)
            {
                throw new ArgumentException(CodeAnalysisResources.OperationHasNullSemanticModel, argumentNameForException);
            }
            try
            {
                ControlFlowGraphBuilder.Context context = default(ControlFlowGraphBuilder.Context);
                return ControlFlowGraphBuilder.Create(operation, null, null, null, in context);
            }
            catch (Exception exception) when (FatalError.ReportAndCatchUnlessCanceled(exception, cancellationToken))
            {
            }
            return null;
        }

        public ControlFlowGraph GetLocalFunctionControlFlowGraph(IMethodSymbol localFunction, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (localFunction == null)
            {
                throw new ArgumentNullException("localFunction");
            }
            if (!TryGetLocalFunctionControlFlowGraph(localFunction, cancellationToken, out var controlFlowGraph))
            {
                throw new ArgumentOutOfRangeException("localFunction");
            }
            return controlFlowGraph;
        }

        internal bool TryGetLocalFunctionControlFlowGraph(IMethodSymbol localFunction, CancellationToken cancellationToken, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ControlFlowGraph? controlFlowGraph)
        {
            if (!_localFunctionsMap.TryGetValue(localFunction, out var value))
            {
                controlFlowGraph = null;
                return false;
            }
            if (_lazyLocalFunctionsGraphs == null)
            {
                Interlocked.CompareExchange(ref _lazyLocalFunctionsGraphs, new ControlFlowGraph[LocalFunctions.Length], null);
            }
            ref ControlFlowGraph reference = ref _lazyLocalFunctionsGraphs[value.Item3];
            if (reference == null)
            {
                ILocalFunctionOperation item = value.Item2;
                ControlFlowRegion item2 = value.Item1;
                ControlFlowGraphBuilder.CaptureIdDispenser captureIdDispenser = _captureIdDispenser;
                ControlFlowGraphBuilder.Context context = default(ControlFlowGraphBuilder.Context);
                ControlFlowGraph value2 = ControlFlowGraphBuilder.Create(item, this, item2, captureIdDispenser, in context);
                Interlocked.CompareExchange(ref reference, value2, null);
            }
            controlFlowGraph = reference;
            return true;
        }

        public ControlFlowGraph GetAnonymousFunctionControlFlowGraph(IFlowAnonymousFunctionOperation anonymousFunction, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (anonymousFunction == null)
            {
                throw new ArgumentNullException("anonymousFunction");
            }
            if (!TryGetAnonymousFunctionControlFlowGraph(anonymousFunction, cancellationToken, out var controlFlowGraph))
            {
                throw new ArgumentOutOfRangeException("anonymousFunction");
            }
            return controlFlowGraph;
        }

        internal bool TryGetAnonymousFunctionControlFlowGraph(IFlowAnonymousFunctionOperation anonymousFunction, CancellationToken cancellationToken, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ControlFlowGraph? controlFlowGraph)
        {
            if (!_anonymousFunctionsMap.TryGetValue(anonymousFunction, out var value))
            {
                controlFlowGraph = null;
                return false;
            }
            if (_lazyAnonymousFunctionsGraphs == null)
            {
                Interlocked.CompareExchange(ref _lazyAnonymousFunctionsGraphs, new ControlFlowGraph[_anonymousFunctionsMap.Count], null);
            }
            ref ControlFlowGraph reference = ref _lazyAnonymousFunctionsGraphs[value.Item2];
            if (reference == null)
            {
                FlowAnonymousFunctionOperation flowAnonymousFunctionOperation = (FlowAnonymousFunctionOperation)anonymousFunction;
                ControlFlowGraph value2 = ControlFlowGraphBuilder.Create(flowAnonymousFunctionOperation.Original, this, value.Item1, _captureIdDispenser, in flowAnonymousFunctionOperation.Context);
                Interlocked.CompareExchange(ref reference, value2, null);
            }
            controlFlowGraph = reference;
            return true;
        }
    }
}
