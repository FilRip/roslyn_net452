using Microsoft.CodeAnalysis.Symbols;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class LocalSymbol : Symbol, ILocalSymbolInternal, ISymbolInternal
    {
        internal abstract LocalDeclarationKind DeclarationKind { get; }

        internal abstract SynthesizedLocalKind SynthesizedKind { get; }

        internal abstract SyntaxNode ScopeDesignatorOpt { get; }

        internal abstract bool IsImportedFromMetadata { get; }

        internal virtual bool CanScheduleToStack
        {
            get
            {
                if (!IsConst)
                {
                    return !IsPinned;
                }
                return false;
            }
        }

        internal abstract SyntaxToken IdentifierToken { get; }

        public abstract TypeWithAnnotations TypeWithAnnotations { get; }

        public TypeSymbol Type => TypeWithAnnotations.Type;

        internal abstract bool IsPinned { get; }

        public sealed override bool IsExtern => false;

        public sealed override bool IsSealed => false;

        public sealed override bool IsAbstract => false;

        public sealed override bool IsOverride => false;

        public sealed override bool IsVirtual => false;

        public sealed override bool IsStatic => false;

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

        public sealed override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public sealed override SymbolKind Kind => SymbolKind.Local;

        public bool IsCatch => DeclarationKind == LocalDeclarationKind.CatchVariable;

        public bool IsConst => DeclarationKind == LocalDeclarationKind.Constant;

        public bool IsUsing => DeclarationKind == LocalDeclarationKind.UsingVariable;

        public bool IsFixed => DeclarationKind == LocalDeclarationKind.FixedVariable;

        public bool IsForEach => DeclarationKind == LocalDeclarationKind.ForEachIterationVariable;

        internal virtual bool IsWritableVariable
        {
            get
            {
                LocalDeclarationKind declarationKind = DeclarationKind;
                if (declarationKind - 2 <= LocalDeclarationKind.Constant || declarationKind == LocalDeclarationKind.ForEachIterationVariable)
                {
                    return false;
                }
                return true;
            }
        }

        public bool HasConstantValue
        {
            get
            {
                if (!IsConst)
                {
                    return false;
                }
                ConstantValue constantValue = GetConstantValue(null, null);
                if (constantValue != null)
                {
                    return !constantValue.IsBad;
                }
                return false;
            }
        }

        public object ConstantValue
        {
            get
            {
                if (!IsConst)
                {
                    return null;
                }
                return GetConstantValue(null, null)?.Value;
            }
        }

        internal abstract bool IsCompilerGenerated { get; }

        public bool IsRef => RefKind != RefKind.None;

        public abstract RefKind RefKind { get; }

        internal abstract uint RefEscapeScope { get; }

        internal abstract uint ValEscapeScope { get; }

        internal virtual SyntaxNode ForbiddenZone => null;

        internal virtual ErrorCode ForbiddenDiagnostic => ErrorCode.ERR_VariableUsedBeforeDeclaration;

        SynthesizedLocalKind ILocalSymbolInternal.SynthesizedKind => SynthesizedKind;

        bool ILocalSymbolInternal.IsImportedFromMetadata => IsImportedFromMetadata;

        internal abstract LocalSymbol WithSynthesizedLocalKindAndSyntax(SynthesizedLocalKind kind, SyntaxNode syntax);

        internal sealed override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitLocal(this, argument);
        }

        public sealed override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitLocal(this);
        }

        public sealed override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitLocal(this);
        }

        internal abstract SyntaxNode GetDeclaratorSyntax();

        internal abstract ConstantValue GetConstantValue(SyntaxNode node, LocalSymbol inProgress, BindingDiagnosticBag diagnostics = null);

        internal abstract ImmutableBindingDiagnostic<AssemblySymbol> GetConstantValueDiagnostics(BoundExpression boundInitValue);

        protected sealed override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.LocalSymbol(this);
        }

        SyntaxNode ILocalSymbolInternal.GetDeclaratorSyntax()
        {
            return GetDeclaratorSyntax();
        }
    }
}
