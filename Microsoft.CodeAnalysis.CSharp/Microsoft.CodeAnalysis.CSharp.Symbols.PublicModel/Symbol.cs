using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable disable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal abstract class Symbol : ISymbol
    {
        internal abstract Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol { get; }

        ISymbol ISymbol.OriginalDefinition => UnderlyingSymbol.OriginalDefinition.GetPublicSymbol();

        ISymbol ISymbol.ContainingSymbol => UnderlyingSymbol.ContainingSymbol.GetPublicSymbol();

        INamedTypeSymbol ISymbol.ContainingType => UnderlyingSymbol.ContainingType.GetPublicSymbol();

        ImmutableArray<Location> ISymbol.Locations => UnderlyingSymbol.Locations;

        ImmutableArray<SyntaxReference> ISymbol.DeclaringSyntaxReferences => UnderlyingSymbol.DeclaringSyntaxReferences;

        Accessibility ISymbol.DeclaredAccessibility => UnderlyingSymbol.DeclaredAccessibility;

        SymbolKind ISymbol.Kind => UnderlyingSymbol.Kind;

        string ISymbol.Language => "C#";

        string ISymbol.Name => UnderlyingSymbol.Name;

        string ISymbol.MetadataName => UnderlyingSymbol.MetadataName;

        IAssemblySymbol ISymbol.ContainingAssembly => UnderlyingSymbol.ContainingAssembly.GetPublicSymbol();

        IModuleSymbol ISymbol.ContainingModule => UnderlyingSymbol.ContainingModule.GetPublicSymbol();

        INamespaceSymbol ISymbol.ContainingNamespace => UnderlyingSymbol.ContainingNamespace.GetPublicSymbol();

        bool ISymbol.IsDefinition => UnderlyingSymbol.IsDefinition;

        bool ISymbol.IsStatic => UnderlyingSymbol.IsStatic;

        bool ISymbol.IsVirtual => UnderlyingSymbol.IsVirtual;

        bool ISymbol.IsOverride => UnderlyingSymbol.IsOverride;

        bool ISymbol.IsAbstract => UnderlyingSymbol.IsAbstract;

        bool ISymbol.IsSealed => UnderlyingSymbol.IsSealed;

        bool ISymbol.IsExtern => UnderlyingSymbol.IsExtern;

        bool ISymbol.IsImplicitlyDeclared => UnderlyingSymbol.IsImplicitlyDeclared;

        bool ISymbol.CanBeReferencedByName => UnderlyingSymbol.CanBeReferencedByName;

        bool ISymbol.HasUnsupportedMetadata => UnderlyingSymbol.HasUnsupportedMetadata;

        protected static ImmutableArray<TypeWithAnnotations> ConstructTypeArguments(ITypeSymbol[] typeArguments)
        {
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(typeArguments.Length);
            foreach (ITypeSymbol typeSymbol in typeArguments)
            {
                Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol typeSymbol2 = typeSymbol.EnsureCSharpSymbolOrNull("typeArguments");
                instance.Add(TypeWithAnnotations.Create(typeSymbol2, typeSymbol?.NullableAnnotation.ToInternalAnnotation() ?? NullableAnnotation.NotAnnotated));
            }
            return instance.ToImmutableAndFree();
        }

        protected static ImmutableArray<TypeWithAnnotations> ConstructTypeArguments(ImmutableArray<ITypeSymbol> typeArguments, ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation> typeArgumentNullableAnnotations)
        {
            if (typeArguments.IsDefault)
            {
                throw new ArgumentException("typeArguments");
            }
            int length = typeArguments.Length;
            if (!typeArgumentNullableAnnotations.IsDefault && typeArgumentNullableAnnotations.Length != length)
            {
                throw new ArgumentException("typeArgumentNullableAnnotations");
            }
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(length);
            for (int i = 0; i < length; i++)
            {
                Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol typeSymbol = typeArguments[i].EnsureCSharpSymbolOrNull("typeArguments");
                NullableAnnotation nullableAnnotation = (typeArgumentNullableAnnotations.IsDefault ? NullableAnnotation.Oblivious : typeArgumentNullableAnnotations[i].ToInternalAnnotation());
                instance.Add(TypeWithAnnotations.Create(typeSymbol, nullableAnnotation));
            }
            return instance.ToImmutableAndFree();
        }

        public sealed override int GetHashCode()
        {
            return UnderlyingSymbol.GetHashCode();
        }

        public sealed override bool Equals(object obj)
        {
            return Equals(obj as Symbol, Microsoft.CodeAnalysis.SymbolEqualityComparer.Default);
        }

        bool IEquatable<ISymbol>.Equals(ISymbol other)
        {
            return Equals(other as Symbol, Microsoft.CodeAnalysis.SymbolEqualityComparer.Default);
        }

        bool ISymbol.Equals(ISymbol other, Microsoft.CodeAnalysis.SymbolEqualityComparer equalityComparer)
        {
            return Equals(other as Symbol, equalityComparer);
        }

        protected bool Equals(Symbol other, Microsoft.CodeAnalysis.SymbolEqualityComparer equalityComparer)
        {
            if (other != null)
            {
                return UnderlyingSymbol.Equals(other.UnderlyingSymbol, equalityComparer.CompareKind);
            }
            return false;
        }

        ImmutableArray<AttributeData> ISymbol.GetAttributes()
        {
            return StaticCast<AttributeData>.From(UnderlyingSymbol.GetAttributes());
        }

        void ISymbol.Accept(SymbolVisitor visitor)
        {
            Accept(visitor);
        }

        protected abstract void Accept(SymbolVisitor visitor);

        TResult ISymbol.Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return Accept(visitor);
        }

        protected abstract TResult Accept<TResult>(SymbolVisitor<TResult> visitor);

        string ISymbol.GetDocumentationCommentId()
        {
            return UnderlyingSymbol.GetDocumentationCommentId();
        }

        string ISymbol.GetDocumentationCommentXml(CultureInfo preferredCulture, bool expandIncludes, CancellationToken cancellationToken)
        {
            return UnderlyingSymbol.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
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
            return SymbolDisplay.ToMinimalDisplayString(this, GetCSharpSemanticModel(semanticModel), position, format);
        }

        ImmutableArray<SymbolDisplayPart> ISymbol.ToMinimalDisplayParts(SemanticModel semanticModel, int position, SymbolDisplayFormat format)
        {
            return SymbolDisplay.ToMinimalDisplayParts(this, GetCSharpSemanticModel(semanticModel), position, format);
        }

        internal static CSharpSemanticModel GetCSharpSemanticModel(SemanticModel semanticModel)
        {
            return (semanticModel as CSharpSemanticModel) ?? throw new ArgumentException(CSharpResources.WrongSemanticModelType, "C#");
        }

        public sealed override string ToString()
        {
            return SymbolDisplay.ToDisplayString(this);
        }
    }
}
