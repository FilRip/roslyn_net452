using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    internal sealed class SynthesizedLocal : LocalSymbol
    {
        private readonly MethodSymbol _containingMethodOpt;

        private readonly TypeWithAnnotations _type;

        private readonly SynthesizedLocalKind _kind;

        private readonly SyntaxNode _syntaxOpt;

        private readonly bool _isPinned;

        private readonly RefKind _refKind;

        public SyntaxNode SyntaxOpt => _syntaxOpt;

        public override RefKind RefKind => _refKind;

        internal override bool IsImportedFromMetadata => false;

        internal override LocalDeclarationKind DeclarationKind => LocalDeclarationKind.None;

        internal override SynthesizedLocalKind SynthesizedKind => _kind;

        internal override SyntaxNode ScopeDesignatorOpt => null;

        internal override SyntaxToken IdentifierToken => default(SyntaxToken);

        public override Symbol ContainingSymbol => _containingMethodOpt;

        public override string Name => null;

        public override TypeWithAnnotations TypeWithAnnotations => _type;

        public override ImmutableArray<Location> Locations
        {
            get
            {
                if (_syntaxOpt != null)
                {
                    return ImmutableArray.Create(_syntaxOpt.GetLocation());
                }
                return ImmutableArray<Location>.Empty;
            }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get
            {
                if (_syntaxOpt != null)
                {
                    return ImmutableArray.Create(_syntaxOpt.GetReference());
                }
                return ImmutableArray<SyntaxReference>.Empty;
            }
        }

        public override bool IsImplicitlyDeclared => true;

        internal override bool IsPinned => _isPinned;

        internal override bool IsCompilerGenerated => true;

        internal override uint ValEscapeScope
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override uint RefEscapeScope
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal SynthesizedLocal(MethodSymbol containingMethodOpt, TypeWithAnnotations type, SynthesizedLocalKind kind, SyntaxNode syntaxOpt = null, bool isPinned = false, RefKind refKind = RefKind.None)
        {
            _containingMethodOpt = containingMethodOpt;
            _type = type;
            _kind = kind;
            _syntaxOpt = syntaxOpt;
            _isPinned = isPinned;
            _refKind = refKind;
        }

        internal override LocalSymbol WithSynthesizedLocalKindAndSyntax(SynthesizedLocalKind kind, SyntaxNode syntax)
        {
            return new SynthesizedLocal(_containingMethodOpt, _type, kind, syntax, _isPinned, _refKind);
        }

        internal override SyntaxNode GetDeclaratorSyntax()
        {
            return _syntaxOpt;
        }

        internal override ConstantValue GetConstantValue(SyntaxNode node, LocalSymbol inProgress, BindingDiagnosticBag diagnostics)
        {
            return null;
        }

        internal override ImmutableBindingDiagnostic<AssemblySymbol> GetConstantValueDiagnostics(BoundExpression boundInitValue)
        {
            return ImmutableBindingDiagnostic<AssemblySymbol>.Empty;
        }

        internal override string GetDebuggerDisplay()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('<');
            stringBuilder.Append(_kind.ToString());
            stringBuilder.Append('>');
            stringBuilder.Append(' ');
            stringBuilder.Append(_type.ToDisplayString(SymbolDisplayFormat.TestFormat));
            return stringBuilder.ToString();
        }
    }
}
