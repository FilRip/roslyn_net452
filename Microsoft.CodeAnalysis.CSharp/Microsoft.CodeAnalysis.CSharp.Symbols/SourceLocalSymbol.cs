using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public class SourceLocalSymbol : LocalSymbol
    {
        private sealed class LocalWithInitializer : SourceLocalSymbol
        {
            private readonly EqualsValueClauseSyntax _initializer;

            private readonly Binder _initializerBinder;

            private EvaluatedConstant _constantTuple;

            internal override SyntaxNode ForbiddenZone => _initializer;

            public LocalWithInitializer(Symbol containingSymbol, Binder scopeBinder, TypeSyntax typeSyntax, SyntaxToken identifierToken, EqualsValueClauseSyntax initializer, Binder initializerBinder, LocalDeclarationKind declarationKind)
                : base(containingSymbol, scopeBinder, allowRefKind: true, typeSyntax, identifierToken, declarationKind)
            {
                _initializer = initializer;
                _initializerBinder = initializerBinder;
                _refEscapeScope = _scopeBinder.LocalScopeDepth;
                _valEscapeScope = _scopeBinder.LocalScopeDepth;
            }

            protected override TypeWithAnnotations InferTypeOfVarVariable(BindingDiagnosticBag diagnostics)
            {
                return TypeWithAnnotations.Create(_initializerBinder.BindInferredVariableInitializer(diagnostics, RefKind, _initializer, _initializer)?.Type);
            }

            private void MakeConstantTuple(LocalSymbol inProgress, BoundExpression boundInitValue)
            {
                if (base.IsConst && _constantTuple == null)
                {
                    ConstantValue bad = Microsoft.CodeAnalysis.ConstantValue.Bad;
                    Location location = _initializer.Value.Location;
                    BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                    TypeSymbol type = base.Type;
                    if (boundInitValue == null)
                    {
                        boundInitValue = new LocalInProgressBinder(this, _initializerBinder).BindVariableOrAutoPropInitializerValue(_initializer, RefKind, type, instance);
                    }
                    bad = ConstantValueUtils.GetAndValidateConstantValue(boundInitValue, this, type, location, instance);
                    Interlocked.CompareExchange(ref _constantTuple, new EvaluatedConstant(bad, instance.ToReadOnlyAndFree()), null);
                }
            }

            internal override ConstantValue GetConstantValue(SyntaxNode node, LocalSymbol inProgress, BindingDiagnosticBag diagnostics = null)
            {
                if (base.IsConst && inProgress == this)
                {
                    diagnostics?.Add(ErrorCode.ERR_CircConstValue, node.GetLocation(), this);
                    return Microsoft.CodeAnalysis.ConstantValue.Bad;
                }
                MakeConstantTuple(inProgress, null);
                if (_constantTuple != null)
                {
                    return _constantTuple.Value;
                }
                return null;
            }

            internal override ImmutableBindingDiagnostic<AssemblySymbol> GetConstantValueDiagnostics(BoundExpression boundInitValue)
            {
                MakeConstantTuple(null, boundInitValue);
                if (_constantTuple != null)
                {
                    return _constantTuple.Diagnostics;
                }
                return ImmutableBindingDiagnostic<AssemblySymbol>.Empty;
            }

            internal override void SetRefEscape(uint value)
            {
                _refEscapeScope = value;
            }

            internal override void SetValEscape(uint value)
            {
                _valEscapeScope = value;
            }
        }

        private sealed class ForEachLocalSymbol : SourceLocalSymbol
        {
            private readonly ExpressionSyntax _collection;

            private ForEachLoopBinder ForEachLoopBinder => (ForEachLoopBinder)base.ScopeBinder;

            internal override SyntaxNode ForbiddenZone => null;

            public ForEachLocalSymbol(Symbol containingSymbol, ForEachLoopBinder scopeBinder, TypeSyntax typeSyntax, SyntaxToken identifierToken, ExpressionSyntax collection, LocalDeclarationKind declarationKind)
                : base(containingSymbol, scopeBinder, allowRefKind: true, typeSyntax, identifierToken, declarationKind)
            {
                _collection = collection;
            }

            protected override TypeWithAnnotations InferTypeOfVarVariable(BindingDiagnosticBag diagnostics)
            {
                return ForEachLoopBinder.InferCollectionElementType(diagnostics, _collection);
            }
        }

        private class DeconstructionLocalSymbol : SourceLocalSymbol
        {
            private readonly SyntaxNode _deconstruction;

            private readonly Binder _nodeBinder;

            internal override SyntaxNode ForbiddenZone => _deconstruction.Kind() switch
            {
                SyntaxKind.SimpleAssignmentExpression => _deconstruction,
                SyntaxKind.ForEachVariableStatement => null,
                _ => null,
            };

            public DeconstructionLocalSymbol(Symbol containingSymbol, Binder scopeBinder, Binder nodeBinder, TypeSyntax typeSyntax, SyntaxToken identifierToken, LocalDeclarationKind declarationKind, SyntaxNode deconstruction)
                : base(containingSymbol, scopeBinder, allowRefKind: false, typeSyntax, identifierToken, declarationKind)
            {
                _deconstruction = deconstruction;
                _nodeBinder = nodeBinder;
            }

            protected override TypeWithAnnotations InferTypeOfVarVariable(BindingDiagnosticBag diagnostics)
            {
                switch (_deconstruction.Kind())
                {
                    case SyntaxKind.SimpleAssignmentExpression:
                        {
                            AssignmentExpressionSyntax assignmentExpressionSyntax = (AssignmentExpressionSyntax)_deconstruction;
                            DeclarationExpressionSyntax declaration = null;
                            ExpressionSyntax expression = null;
                            _nodeBinder.BindDeconstruction(assignmentExpressionSyntax, assignmentExpressionSyntax.Left, assignmentExpressionSyntax.Right, diagnostics, ref declaration, ref expression);
                            break;
                        }
                    case SyntaxKind.ForEachVariableStatement:
                        _nodeBinder.BindForEachDeconstruction(diagnostics, _nodeBinder);
                        break;
                    default:
                        return TypeWithAnnotations.Create(_nodeBinder.CreateErrorType());
                }
                return _type.Value;
            }
        }

        private class LocalSymbolWithEnclosingContext : SourceLocalSymbol
        {
            private readonly SyntaxNode _forbiddenZone;

            private readonly Binder _nodeBinder;

            private readonly SyntaxNode _nodeToBind;

            internal override SyntaxNode ForbiddenZone => _forbiddenZone;

            internal override ErrorCode ForbiddenDiagnostic => ErrorCode.ERR_ImplicitlyTypedOutVariableUsedInTheSameArgumentList;

            public LocalSymbolWithEnclosingContext(Symbol containingSymbol, Binder scopeBinder, Binder nodeBinder, TypeSyntax typeSyntax, SyntaxToken identifierToken, LocalDeclarationKind declarationKind, SyntaxNode nodeToBind, SyntaxNode forbiddenZone)
                : base(containingSymbol, scopeBinder, allowRefKind: false, typeSyntax, identifierToken, declarationKind)
            {
                _nodeBinder = nodeBinder;
                _nodeToBind = nodeToBind;
                _forbiddenZone = forbiddenZone;
            }

            protected override TypeWithAnnotations InferTypeOfVarVariable(BindingDiagnosticBag diagnostics)
            {
                switch (_nodeToBind.Kind())
                {
                    case SyntaxKind.BaseConstructorInitializer:
                    case SyntaxKind.ThisConstructorInitializer:
                        {
                            ConstructorInitializerSyntax initializer3 = (ConstructorInitializerSyntax)_nodeToBind;
                            _nodeBinder.BindConstructorInitializer(initializer3, diagnostics);
                            break;
                        }
                    case SyntaxKind.PrimaryConstructorBaseType:
                        _nodeBinder.BindConstructorInitializer((PrimaryConstructorBaseTypeSyntax)_nodeToBind, diagnostics);
                        break;
                    case SyntaxKind.ArgumentList:
                        {
                            SyntaxNode parent = _nodeToBind.Parent;
                            if (!(parent is ConstructorInitializerSyntax initializer))
                            {
                                if (!(parent is PrimaryConstructorBaseTypeSyntax initializer2))
                                {
                                    throw ExceptionUtilities.UnexpectedValue(_nodeToBind.Parent);
                                }
                                _nodeBinder.BindConstructorInitializer(initializer2, diagnostics);
                            }
                            else
                            {
                                _nodeBinder.BindConstructorInitializer(initializer, diagnostics);
                            }
                            break;
                        }
                    case SyntaxKind.CasePatternSwitchLabel:
                        _nodeBinder.BindPatternSwitchLabelForInference((CasePatternSwitchLabelSyntax)_nodeToBind, diagnostics);
                        break;
                    case SyntaxKind.VariableDeclarator:
                        _nodeBinder.BindDeclaratorArguments((VariableDeclaratorSyntax)_nodeToBind, diagnostics);
                        break;
                    case SyntaxKind.SwitchExpressionArm:
                        {
                            SwitchExpressionArmSyntax node = (SwitchExpressionArmSyntax)_nodeToBind;
                            ((SwitchExpressionArmBinder)_nodeBinder).BindSwitchExpressionArm(node, diagnostics);
                            break;
                        }
                    case SyntaxKind.GotoCaseStatement:
                        _nodeBinder.BindStatement((GotoStatementSyntax)_nodeToBind, diagnostics);
                        break;
                    default:
                        _nodeBinder.BindExpression((ExpressionSyntax)_nodeToBind, diagnostics);
                        break;
                }
                if (_type == null)
                {
                    SetTypeWithAnnotations(TypeWithAnnotations.Create(_nodeBinder.CreateErrorType("var")));
                }
                return _type.Value;
            }
        }

        private readonly Binder _scopeBinder;

        private readonly Symbol _containingSymbol;

        private readonly SyntaxToken _identifierToken;

        private readonly ImmutableArray<Location> _locations;

        private readonly RefKind _refKind;

        private readonly TypeSyntax _typeSyntax;

        private readonly LocalDeclarationKind _declarationKind;

        private TypeWithAnnotations.Boxed _type;

        protected uint _refEscapeScope;

        protected uint _valEscapeScope;

        internal Binder ScopeBinder => _scopeBinder;

        internal override SyntaxNode ScopeDesignatorOpt => _scopeBinder.ScopeDesignator;

        internal override uint RefEscapeScope => _refEscapeScope;

        internal override uint ValEscapeScope => _valEscapeScope;

        internal Binder TypeSyntaxBinder => _scopeBinder;

        internal override bool IsImportedFromMetadata => false;

        internal override LocalDeclarationKind DeclarationKind => _declarationKind;

        internal override SynthesizedLocalKind SynthesizedKind => SynthesizedLocalKind.UserDefined;

        internal override bool IsPinned => false;

        public override Symbol ContainingSymbol => _containingSymbol;

        public override string Name => _identifierToken.ValueText;

        internal override SyntaxToken IdentifierToken => _identifierToken;

        public override TypeWithAnnotations TypeWithAnnotations
        {
            get
            {
                if (_type == null)
                {
                    TypeWithAnnotations typeSymbol = GetTypeSymbol();
                    SetTypeWithAnnotations(typeSymbol);
                }
                return _type.Value;
            }
        }

        public bool IsVar
        {
            get
            {
                if (_typeSyntax == null)
                {
                    return true;
                }
                if (_typeSyntax.IsVar)
                {
                    TypeSyntaxBinder.BindTypeOrVarKeyword(_typeSyntax, BindingDiagnosticBag.Discarded, out var isVar);
                    return isVar;
                }
                return false;
            }
        }

        public override ImmutableArray<Location> Locations => _locations;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray.Create(_identifierToken.Parent!.GetReference());

        internal override bool IsCompilerGenerated => false;

        public override RefKind RefKind => _refKind;

        private SourceLocalSymbol(Symbol containingSymbol, Binder scopeBinder, bool allowRefKind, TypeSyntax typeSyntax, SyntaxToken identifierToken, LocalDeclarationKind declarationKind)
        {
            _scopeBinder = scopeBinder;
            _containingSymbol = containingSymbol;
            _identifierToken = identifierToken;
            _typeSyntax = ((!allowRefKind) ? typeSyntax : typeSyntax?.SkipRef(out _refKind));
            _declarationKind = declarationKind;
            _locations = ImmutableArray.Create(identifierToken.GetLocation());
            _refEscapeScope = ((_refKind == RefKind.None) ? scopeBinder.LocalScopeDepth : 0u);
            _valEscapeScope = 0u;
        }

        internal override string GetDebuggerDisplay()
        {
            if (_type == null)
            {
                return $"{Kind} <var> ${Name}";
            }
            return base.GetDebuggerDisplay();
        }

        public static SourceLocalSymbol MakeForeachLocal(MethodSymbol containingMethod, ForEachLoopBinder binder, TypeSyntax typeSyntax, SyntaxToken identifierToken, ExpressionSyntax collection)
        {
            return new ForEachLocalSymbol(containingMethod, binder, typeSyntax, identifierToken, collection, LocalDeclarationKind.ForEachIterationVariable);
        }

        public static SourceLocalSymbol MakeDeconstructionLocal(Symbol containingSymbol, Binder scopeBinder, Binder nodeBinder, TypeSyntax closestTypeSyntax, SyntaxToken identifierToken, LocalDeclarationKind kind, SyntaxNode deconstruction)
        {
            if (!closestTypeSyntax.IsVar)
            {
                return new SourceLocalSymbol(containingSymbol, scopeBinder, allowRefKind: false, closestTypeSyntax, identifierToken, kind);
            }
            return new DeconstructionLocalSymbol(containingSymbol, scopeBinder, nodeBinder, closestTypeSyntax, identifierToken, kind, deconstruction);
        }

        internal static LocalSymbol MakeLocalSymbolWithEnclosingContext(Symbol containingSymbol, Binder scopeBinder, Binder nodeBinder, TypeSyntax typeSyntax, SyntaxToken identifierToken, LocalDeclarationKind kind, SyntaxNode nodeToBind, SyntaxNode forbiddenZone)
        {
            if ((typeSyntax != null && !typeSyntax.IsVar) || kind == LocalDeclarationKind.DeclarationExpressionVariable)
            {
                return new SourceLocalSymbol(containingSymbol, scopeBinder, allowRefKind: false, typeSyntax, identifierToken, kind);
            }
            return new LocalSymbolWithEnclosingContext(containingSymbol, scopeBinder, nodeBinder, typeSyntax, identifierToken, kind, nodeToBind, forbiddenZone);
        }

        public static SourceLocalSymbol MakeLocal(Symbol containingSymbol, Binder scopeBinder, bool allowRefKind, TypeSyntax typeSyntax, SyntaxToken identifierToken, LocalDeclarationKind declarationKind, EqualsValueClauseSyntax initializer = null, Binder initializerBinderOpt = null)
        {
            if (initializer == null)
            {
                return new SourceLocalSymbol(containingSymbol, scopeBinder, allowRefKind, typeSyntax, identifierToken, declarationKind);
            }
            return new LocalWithInitializer(containingSymbol, scopeBinder, typeSyntax, identifierToken, initializer, initializerBinderOpt ?? scopeBinder, declarationKind);
        }

        internal override LocalSymbol WithSynthesizedLocalKindAndSyntax(SynthesizedLocalKind kind, SyntaxNode syntax)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal virtual void SetRefEscape(uint value)
        {
            _refEscapeScope = value;
        }

        internal virtual void SetValEscape(uint value)
        {
            _valEscapeScope = value;
        }

        private TypeWithAnnotations GetTypeSymbol()
        {
            BindingDiagnosticBag discarded = BindingDiagnosticBag.Discarded;
            Binder typeSyntaxBinder = TypeSyntaxBinder;
            bool isVar;
            TypeWithAnnotations result;
            if (_typeSyntax == null)
            {
                isVar = true;
                result = default(TypeWithAnnotations);
            }
            else
            {
                result = typeSyntaxBinder.BindTypeOrVarKeyword(_typeSyntax.SkipRef(out var _), discarded, out isVar);
            }
            if (isVar)
            {
                TypeWithAnnotations typeWithAnnotations = InferTypeOfVarVariable(discarded);
                result = ((!typeWithAnnotations.HasType || typeWithAnnotations.IsVoidType()) ? TypeWithAnnotations.Create(typeSyntaxBinder.CreateErrorType("var")) : typeWithAnnotations);
            }
            return result;
        }

        protected virtual TypeWithAnnotations InferTypeOfVarVariable(BindingDiagnosticBag diagnostics)
        {
            return _type?.Value ?? default(TypeWithAnnotations);
        }

        internal void SetTypeWithAnnotations(TypeWithAnnotations newType)
        {
            _ = _type?.Value;
            if (_type == null)
            {
                Interlocked.CompareExchange(ref _type, new TypeWithAnnotations.Boxed(newType), null);
            }
        }

        internal sealed override SyntaxNode GetDeclaratorSyntax()
        {
            return _identifierToken.Parent;
        }

        internal override ConstantValue GetConstantValue(SyntaxNode node, LocalSymbol inProgress, BindingDiagnosticBag diagnostics)
        {
            return null;
        }

        internal override ImmutableBindingDiagnostic<AssemblySymbol> GetConstantValueDiagnostics(BoundExpression boundInitValue)
        {
            return ImmutableBindingDiagnostic<AssemblySymbol>.Empty;
        }

        public sealed override bool Equals(Symbol obj, TypeCompareKind compareKind)
        {
            if ((object)obj == this)
            {
                return true;
            }
            if (obj is UpdatedContainingSymbolAndNullableAnnotationLocal updatedContainingSymbolAndNullableAnnotationLocal)
            {
                return updatedContainingSymbolAndNullableAnnotationLocal.Equals(this, compareKind);
            }
            if (obj is SourceLocalSymbol sourceLocalSymbol && sourceLocalSymbol._identifierToken.Equals(_identifierToken))
            {
                return sourceLocalSymbol._containingSymbol.Equals(_containingSymbol, compareKind);
            }
            return false;
        }

        public sealed override int GetHashCode()
        {
            return Hash.Combine(_identifierToken.GetHashCode(), _containingSymbol.GetHashCode());
        }
    }
}
