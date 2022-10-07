using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;

#nullable disable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class PreprocessingSymbol : IPreprocessingSymbol
    {
        private readonly string _name;

        ISymbol ISymbol.OriginalDefinition => this;

        ISymbol ISymbol.ContainingSymbol => null;

        INamedTypeSymbol ISymbol.ContainingType => null;

        ImmutableArray<Location> ISymbol.Locations => ImmutableArray<Location>.Empty;

        ImmutableArray<SyntaxReference> ISymbol.DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        Accessibility ISymbol.DeclaredAccessibility => Accessibility.NotApplicable;

        SymbolKind ISymbol.Kind => SymbolKind.Preprocessing;

        string ISymbol.Language => "C#";

        string ISymbol.Name => _name;

        string ISymbol.MetadataName => _name;

        IAssemblySymbol ISymbol.ContainingAssembly => null;

        IModuleSymbol ISymbol.ContainingModule => null;

        INamespaceSymbol ISymbol.ContainingNamespace => null;

        bool ISymbol.IsDefinition => true;

        bool ISymbol.IsStatic => false;

        bool ISymbol.IsVirtual => false;

        bool ISymbol.IsOverride => false;

        bool ISymbol.IsAbstract => false;

        bool ISymbol.IsSealed => false;

        bool ISymbol.IsExtern => false;

        bool ISymbol.IsImplicitlyDeclared => false;

        bool ISymbol.CanBeReferencedByName
        {
            get
            {
                if (SyntaxFacts.IsValidIdentifier(_name))
                {
                    return !SyntaxFacts.ContainsDroppedIdentifierCharacters(_name);
                }
                return false;
            }
        }

        bool ISymbol.HasUnsupportedMetadata => false;

        internal PreprocessingSymbol(string name)
        {
            _name = name;
        }

        public sealed override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (obj is PreprocessingSymbol preprocessingSymbol)
            {
                return _name.Equals(preprocessingSymbol._name);
            }
            return false;
        }

        bool IEquatable<ISymbol>.Equals(ISymbol other)
        {
            return Equals(other);
        }

        bool ISymbol.Equals(ISymbol other, Microsoft.CodeAnalysis.SymbolEqualityComparer equalityComparer)
        {
            return Equals(other);
        }

        ImmutableArray<AttributeData> ISymbol.GetAttributes()
        {
            return ImmutableArray<AttributeData>.Empty;
        }

        void ISymbol.Accept(SymbolVisitor visitor)
        {
            throw new NotSupportedException();
        }

        TResult ISymbol.Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            throw new NotSupportedException();
        }

        string ISymbol.GetDocumentationCommentId()
        {
            return null;
        }

        string ISymbol.GetDocumentationCommentXml(CultureInfo preferredCulture, bool expandIncludes, CancellationToken cancellationToken)
        {
            return null;
        }

        string ISymbol.ToDisplayString(SymbolDisplayFormat format)
        {
            return SymbolDisplay.ToDisplayString(this, format);
        }

        ImmutableArray<SymbolDisplayPart> ISymbol.ToDisplayParts(SymbolDisplayFormat format)
        {
            return SymbolDisplay.ToDisplayParts(this, format);
        }

        string ISymbol.ToMinimalDisplayString(SemanticModel semanticModel, int position, SymbolDisplayFormat format)
        {
            return SymbolDisplay.ToMinimalDisplayString(this, Symbol.GetCSharpSemanticModel(semanticModel), position, format);
        }

        ImmutableArray<SymbolDisplayPart> ISymbol.ToMinimalDisplayParts(SemanticModel semanticModel, int position, SymbolDisplayFormat format)
        {
            return SymbolDisplay.ToMinimalDisplayParts(this, Symbol.GetCSharpSemanticModel(semanticModel), position, format);
        }

        public sealed override string ToString()
        {
            return SymbolDisplay.ToDisplayString(this);
        }
    }
}
