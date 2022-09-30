using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class MergedTypeDeclaration : MergedNamespaceOrTypeDeclaration
	{
		private ImmutableArray<SingleTypeDeclaration> _declarations;

		private MergedTypeDeclaration[] _children;

		private ICollection<string> _memberNames;

		private static readonly Func<SingleTypeDeclaration, SingleTypeDeclaration> s_identityFunc = (SingleTypeDeclaration t) => t;

		private static readonly Func<IEnumerable<SingleTypeDeclaration>, MergedTypeDeclaration> s_mergeFunc = (IEnumerable<SingleTypeDeclaration> g) => new MergedTypeDeclaration(ImmutableArray.CreateRange(g));

		public ImmutableArray<SingleTypeDeclaration> Declarations
		{
			get
			{
				return _declarations;
			}
			private set
			{
				_declarations = value;
			}
		}

		public ImmutableArray<SyntaxReference> SyntaxReferences
		{
			get
			{
				ArrayBuilder<SyntaxReference> instance = ArrayBuilder<SyntaxReference>.GetInstance();
				ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = Declarations.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SyntaxReference syntaxReference = enumerator.Current.SyntaxReference;
					instance.Add(syntaxReference);
				}
				return instance.ToImmutableAndFree();
			}
		}

		public override DeclarationKind Kind => Declarations[0].Kind;

		public int Arity => Declarations[0].Arity;

		public ImmutableArray<Location> NameLocations
		{
			get
			{
				if (Declarations.Length == 1)
				{
					return ImmutableArray.Create(Declarations[0].NameLocation);
				}
				ArrayBuilder<Location> instance = ArrayBuilder<Location>.GetInstance();
				ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = Declarations.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Location nameLocation = enumerator.Current.NameLocation;
					if ((object)nameLocation != null)
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
				if (_children == null)
				{
					Interlocked.CompareExchange(ref _children, MakeChildren(), null);
				}
				return _children.AsImmutableOrNull();
			}
		}

		public ICollection<string> MemberNames
		{
			get
			{
				if (_memberNames == null)
				{
					ICollection<string> value = UnionCollection<string>.Create(Declarations, (SingleTypeDeclaration d) => d.MemberNames);
					Interlocked.CompareExchange(ref _memberNames, value, null);
				}
				return _memberNames;
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

		internal MergedTypeDeclaration(ImmutableArray<SingleTypeDeclaration> declarations)
			: base(SingleNamespaceOrTypeDeclaration.BestName(declarations))
		{
			Declarations = declarations;
		}

		public ImmutableArray<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
		{
			ArrayBuilder<SyntaxList<AttributeListSyntax>> instance = ArrayBuilder<SyntaxList<AttributeListSyntax>>.GetInstance();
			ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = Declarations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SingleTypeDeclaration current = enumerator.Current;
				if (current.HasAnyAttributes)
				{
					SyntaxNode syntax = current.SyntaxReference.GetSyntax();
					SyntaxList<AttributeListSyntax> attributeLists;
					switch (VisualBasicExtensions.Kind(syntax))
					{
					case SyntaxKind.ModuleBlock:
					case SyntaxKind.StructureBlock:
					case SyntaxKind.InterfaceBlock:
					case SyntaxKind.ClassBlock:
						attributeLists = ((TypeBlockSyntax)syntax).BlockStatement.AttributeLists;
						break;
					case SyntaxKind.DelegateSubStatement:
					case SyntaxKind.DelegateFunctionStatement:
						attributeLists = ((DelegateStatementSyntax)syntax).AttributeLists;
						break;
					case SyntaxKind.EnumBlock:
						attributeLists = ((EnumBlockSyntax)syntax).EnumStatement.AttributeLists;
						break;
					default:
						throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(syntax));
					}
					instance.Add(attributeLists);
				}
			}
			return instance.ToImmutableAndFree();
		}

		public LexicalSortKey GetLexicalSortKey(VisualBasicCompilation compilation)
		{
			LexicalSortKey lexicalSortKey = new LexicalSortKey(_declarations[0].NameLocation, compilation);
			int num = _declarations.Length - 1;
			for (int i = 1; i <= num; i++)
			{
				lexicalSortKey = LexicalSortKey.First(lexicalSortKey, new LexicalSortKey(_declarations[i].NameLocation, compilation));
			}
			return lexicalSortKey;
		}

		private MergedTypeDeclaration[] MakeChildren()
		{
			IEnumerable<SingleTypeDeclaration> types = ((Declarations.Length != 1) ? Declarations.SelectMany((SingleTypeDeclaration d) => d.Children.OfType<SingleTypeDeclaration>()) : Declarations[0].Children.OfType<SingleTypeDeclaration>());
			return MakeMergedTypes(types).ToArray();
		}

		internal static IEnumerable<MergedTypeDeclaration> MakeMergedTypes(IEnumerable<SingleTypeDeclaration> types)
		{
			return types.GroupBy(s_identityFunc, SingleTypeDeclaration.EqualityComparer).Select((Func<IGrouping<SingleTypeDeclaration, SingleTypeDeclaration>, MergedTypeDeclaration>)s_mergeFunc);
		}

		protected override ImmutableArray<Declaration> GetDeclarationChildren()
		{
			return StaticCast<Declaration>.From(Children);
		}
	}
}
