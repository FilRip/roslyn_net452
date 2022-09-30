using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class PlainUnboundLambdaState : UnboundLambdaState
    {
        private readonly ImmutableArray<SyntaxList<AttributeListSyntax>> _parameterAttributes;

        private readonly ImmutableArray<string> _parameterNames;

        private readonly ImmutableArray<bool> _parameterIsDiscardOpt;

        private readonly ImmutableArray<TypeWithAnnotations> _parameterTypesWithAnnotations;

        private readonly ImmutableArray<RefKind> _parameterRefKinds;

        private readonly bool _isAsync;

        private readonly bool _isStatic;

        public override bool HasNames => !_parameterNames.IsDefault;

        public override bool HasSignature => !_parameterNames.IsDefault;

        public override bool HasExplicitlyTypedParameterList => !_parameterTypesWithAnnotations.IsDefault;

        public override int ParameterCount
        {
            get
            {
                if (!_parameterNames.IsDefault)
                {
                    return _parameterNames.Length;
                }
                return 0;
            }
        }

        public override bool IsAsync => _isAsync;

        public override bool IsStatic => _isStatic;

        public override MessageID MessageID
        {
            get
            {
                if (base.UnboundLambda.Syntax.Kind() != SyntaxKind.AnonymousMethodExpression)
                {
                    return MessageID.IDS_Lambda;
                }
                return MessageID.IDS_AnonMethod;
            }
        }

        private CSharpSyntaxNode Body => base.UnboundLambda.Syntax.AnonymousFunctionBody();

        private bool IsExpressionLambda => Body.Kind() != SyntaxKind.Block;

        internal PlainUnboundLambdaState(Binder binder, ImmutableArray<SyntaxList<AttributeListSyntax>> parameterAttributes, ImmutableArray<string> parameterNames, ImmutableArray<bool> parameterIsDiscardOpt, ImmutableArray<TypeWithAnnotations> parameterTypesWithAnnotations, ImmutableArray<RefKind> parameterRefKinds, bool isAsync, bool isStatic, bool includeCache)
            : base(binder, includeCache)
        {
            _parameterAttributes = parameterAttributes;
            _parameterNames = parameterNames;
            _parameterIsDiscardOpt = parameterIsDiscardOpt;
            _parameterTypesWithAnnotations = parameterTypesWithAnnotations;
            _parameterRefKinds = parameterRefKinds;
            _isAsync = isAsync;
            _isStatic = isStatic;
        }

        public override Location ParameterLocation(int index)
        {
            SyntaxNode syntax = base.UnboundLambda.Syntax;
            return syntax.Kind() switch
            {
                SyntaxKind.ParenthesizedLambdaExpression => ((ParenthesizedLambdaExpressionSyntax)syntax).ParameterList.Parameters[index].Identifier.GetLocation(),
                SyntaxKind.AnonymousMethodExpression => ((AnonymousMethodExpressionSyntax)syntax).ParameterList!.Parameters[index].Identifier.GetLocation(),
                _ => ((SimpleLambdaExpressionSyntax)syntax).Parameter.Identifier.GetLocation(),
            };
        }

        public override SyntaxList<AttributeListSyntax> ParameterAttributes(int index)
        {
            if (!_parameterAttributes.IsDefault)
            {
                return _parameterAttributes[index];
            }
            return default(SyntaxList<AttributeListSyntax>);
        }

        public override string ParameterName(int index)
        {
            return _parameterNames[index];
        }

        public override bool ParameterIsDiscard(int index)
        {
            if (!_parameterIsDiscardOpt.IsDefault)
            {
                return _parameterIsDiscardOpt[index];
            }
            return false;
        }

        public override RefKind RefKind(int index)
        {
            if (!_parameterRefKinds.IsDefault)
            {
                return _parameterRefKinds[index];
            }
            return Microsoft.CodeAnalysis.RefKind.None;
        }

        public override TypeWithAnnotations ParameterTypeWithAnnotations(int index)
        {
            return _parameterTypesWithAnnotations[index];
        }

        protected override UnboundLambdaState WithCachingCore(bool includeCache)
        {
            return new PlainUnboundLambdaState(Binder, _parameterAttributes, _parameterNames, _parameterIsDiscardOpt, _parameterTypesWithAnnotations, _parameterRefKinds, _isAsync, _isStatic, includeCache);
        }

        protected override BoundExpression? GetLambdaExpressionBody(BoundBlock body)
        {
            if (IsExpressionLambda)
            {
                ImmutableArray<BoundStatement> statements = body.Statements;
                if (statements.Length == 1 && statements[0] is BoundReturnStatement boundReturnStatement && boundReturnStatement.RefKind == Microsoft.CodeAnalysis.RefKind.None)
                {
                    BoundExpression expressionOpt = boundReturnStatement.ExpressionOpt;
                    if (expressionOpt != null)
                    {
                        return expressionOpt;
                    }
                }
            }
            return null;
        }

        protected override BoundBlock CreateBlockFromLambdaExpressionBody(Binder lambdaBodyBinder, BoundExpression expression, BindingDiagnosticBag diagnostics)
        {
            return lambdaBodyBinder.CreateBlockFromExpression((ExpressionSyntax)Body, expression, diagnostics);
        }

        protected override BoundBlock BindLambdaBody(LambdaSymbol lambdaSymbol, Binder lambdaBodyBinder, BindingDiagnosticBag diagnostics)
        {
            if (IsExpressionLambda)
            {
                return lambdaBodyBinder.BindLambdaExpressionAsBlock((ExpressionSyntax)Body, diagnostics);
            }
            return lambdaBodyBinder.BindEmbeddedBlock((BlockSyntax)Body, diagnostics);
        }
    }
}
