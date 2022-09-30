using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class MergedTypeDeclaration : MergedNamespaceOrTypeDeclaration
    {
        private readonly ImmutableArray<SingleTypeDeclaration> _declarations;

        private ImmutableArray<MergedTypeDeclaration> _lazyChildren;

        private ICollection<string> _lazyMemberNames;

        public ImmutableArray<SingleTypeDeclaration> Declarations => _declarations;

        public ImmutableArray<SyntaxReference> SyntaxReferences => _declarations.SelectAsArray((SingleTypeDeclaration r) => r.SyntaxReference);

        public override DeclarationKind Kind => Declarations[0].Kind;

        public int Arity => Declarations[0].Arity;

        public bool ContainsExtensionMethods
        {
            get
            {
                ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = Declarations.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.AnyMemberHasExtensionMethodSyntax)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool AnyMemberHasAttributes
        {
            get
            {
                ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = Declarations.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.AnyMemberHasAttributes)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public ImmutableArray<SourceLocation> NameLocations
        {
            get
            {
                if (Declarations.Length == 1)
                {
                    return ImmutableArray.Create(Declarations[0].NameLocation);
                }
                ArrayBuilder<SourceLocation> instance = ArrayBuilder<SourceLocation>.GetInstance();
                ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = Declarations.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SourceLocation nameLocation = enumerator.Current.NameLocation;
                    if (nameLocation != null)
                    {
                        instance.Add(nameLocation);
                    }
                }
                return instance.ToImmutableAndFree();
            }
        }

        public new ImmutableArray<MergedTypeDeclaration> Children
        {
            get
            {
                if (_lazyChildren.IsDefault)
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _lazyChildren, MakeChildren());
                }
                return _lazyChildren;
            }
        }

        public ICollection<string> MemberNames
        {
            get
            {
                if (_lazyMemberNames == null)
                {
                    ICollection<string> value = UnionCollection<string>.Create(Declarations, (SingleTypeDeclaration d) => d.MemberNames);
                    Interlocked.CompareExchange(ref _lazyMemberNames, value, null);
                }
                return _lazyMemberNames;
            }
        }

        internal MergedTypeDeclaration(ImmutableArray<SingleTypeDeclaration> declarations)
            : base(declarations[0].Name)
        {
            _declarations = declarations;
        }

        public ImmutableArray<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            ArrayBuilder<SyntaxList<AttributeListSyntax>> instance = ArrayBuilder<SyntaxList<AttributeListSyntax>>.GetInstance();
            ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = _declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SingleTypeDeclaration current = enumerator.Current;
                if (current.HasAnyAttributes)
                {
                    SyntaxNode syntax = current.SyntaxReference.GetSyntax();
                    SyntaxList<AttributeListSyntax> attributeLists;
                    switch (syntax.Kind())
                    {
                        case SyntaxKind.ClassDeclaration:
                        case SyntaxKind.StructDeclaration:
                        case SyntaxKind.InterfaceDeclaration:
                        case SyntaxKind.RecordDeclaration:
                        case SyntaxKind.RecordStructDeclaration:
                            attributeLists = ((TypeDeclarationSyntax)syntax).AttributeLists;
                            break;
                        case SyntaxKind.DelegateDeclaration:
                            attributeLists = ((DelegateDeclarationSyntax)syntax).AttributeLists;
                            break;
                        case SyntaxKind.EnumDeclaration:
                            attributeLists = ((EnumDeclarationSyntax)syntax).AttributeLists;
                            break;
                        default:
                            throw ExceptionUtilities.UnexpectedValue(syntax.Kind());
                    }
                    instance.Add(attributeLists);
                }
            }
            return instance.ToImmutableAndFree();
        }

        public LexicalSortKey GetLexicalSortKey(CSharpCompilation compilation)
        {
            LexicalSortKey lexicalSortKey = new LexicalSortKey(Declarations[0].NameLocation, compilation);
            for (int i = 1; i < Declarations.Length; i++)
            {
                lexicalSortKey = LexicalSortKey.First(lexicalSortKey, new LexicalSortKey(Declarations[i].NameLocation, compilation));
            }
            return lexicalSortKey;
        }

        private ImmutableArray<MergedTypeDeclaration> MakeChildren()
        {
            ArrayBuilder<SingleTypeDeclaration> arrayBuilder = null;
            ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = Declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator2 = enumerator.Current.Children.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    SingleTypeDeclaration current = enumerator2.Current;
                    if (current != null)
                    {
                        if (arrayBuilder == null)
                        {
                            arrayBuilder = ArrayBuilder<SingleTypeDeclaration>.GetInstance();
                        }
                        arrayBuilder.Add(current);
                    }
                }
            }
            ArrayBuilder<MergedTypeDeclaration> instance = ArrayBuilder<MergedTypeDeclaration>.GetInstance();
            if (arrayBuilder != null)
            {
                Dictionary<SingleTypeDeclaration.TypeDeclarationIdentity, ImmutableArray<SingleTypeDeclaration>> dictionary = arrayBuilder.ToDictionary((SingleTypeDeclaration t) => t.Identity);
                arrayBuilder.Free();
                foreach (ImmutableArray<SingleTypeDeclaration> value in dictionary.Values)
                {
                    instance.Add(new MergedTypeDeclaration(value));
                }
            }
            return instance.ToImmutableAndFree();
        }

        protected override ImmutableArray<Declaration> GetDeclarationChildren()
        {
            return StaticCast<Declaration>.From(Children);
        }

        internal string GetDebuggerDisplay()
        {
            return "MergedTypeDeclaration " + base.Name;
        }
    }
}
