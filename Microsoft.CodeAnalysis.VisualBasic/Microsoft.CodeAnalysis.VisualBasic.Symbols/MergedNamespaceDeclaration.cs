using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class MergedNamespaceDeclaration : MergedNamespaceOrTypeDeclaration
	{
		private readonly ImmutableArray<SingleNamespaceDeclaration> _declarations;

		private readonly bool _multipleSpellings;

		private ImmutableArray<MergedNamespaceOrTypeDeclaration> _children;

		public override DeclarationKind Kind => DeclarationKind.Namespace;

		public ImmutableArray<SingleNamespaceDeclaration> Declarations => _declarations;

		public ImmutableArray<Location> NameLocations
		{
			get
			{
				if (_declarations.Length == 1)
				{
					Location nameLocation = _declarations[0].NameLocation;
					if ((object)nameLocation == null)
					{
						return ImmutableArray<Location>.Empty;
					}
					return ImmutableArray.Create(nameLocation);
				}
				ArrayBuilder<Location> instance = ArrayBuilder<Location>.GetInstance();
				ImmutableArray<SingleNamespaceDeclaration>.Enumerator enumerator = _declarations.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Location nameLocation2 = enumerator.Current.NameLocation;
					if ((object)nameLocation2 != null)
					{
						instance.Add(nameLocation2);
					}
				}
				return instance.ToImmutableAndFree();
			}
		}

		public ImmutableArray<SyntaxReference> SyntaxReferences
		{
			get
			{
				ArrayBuilder<SyntaxReference> instance = ArrayBuilder<SyntaxReference>.GetInstance();
				ImmutableArray<SingleNamespaceDeclaration>.Enumerator enumerator = _declarations.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SingleNamespaceDeclaration current = enumerator.Current;
					if (current.SyntaxReference != null)
					{
						instance.Add(current.SyntaxReference);
					}
				}
				return instance.ToImmutableAndFree();
			}
		}

		public new ImmutableArray<MergedNamespaceOrTypeDeclaration> Children
		{
			get
			{
				if (_children.IsDefault)
				{
					ImmutableInterlocked.InterlockedInitialize(ref _children, MakeChildren());
				}
				return _children;
			}
		}

		public bool HasMultipleSpellings => _multipleSpellings;

		private MergedNamespaceDeclaration(ImmutableArray<SingleNamespaceDeclaration> declarations)
			: base(string.Empty)
		{
			if (declarations.Any())
			{
				base.Name = SingleNamespaceDeclaration.BestName(declarations, ref _multipleSpellings);
			}
			_declarations = declarations;
		}

		public static MergedNamespaceDeclaration Create(IEnumerable<SingleNamespaceDeclaration> declarations)
		{
			return new MergedNamespaceDeclaration(ImmutableArray.CreateRange(declarations));
		}

		public static MergedNamespaceDeclaration Create(params SingleNamespaceDeclaration[] declarations)
		{
			return new MergedNamespaceDeclaration(declarations.AsImmutableOrNull());
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

		protected override ImmutableArray<Declaration> GetDeclarationChildren()
		{
			return StaticCast<Declaration>.From(Children);
		}

		private ImmutableArray<MergedNamespaceOrTypeDeclaration> MakeChildren()
		{
			ArrayBuilder<SingleNamespaceDeclaration> instance = ArrayBuilder<SingleNamespaceDeclaration>.GetInstance();
			ArrayBuilder<SingleTypeDeclaration> instance2 = ArrayBuilder<SingleTypeDeclaration>.GetInstance();
			ImmutableArray<SingleNamespaceDeclaration>.Enumerator enumerator = _declarations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ImmutableArray<SingleNamespaceOrTypeDeclaration>.Enumerator enumerator2 = enumerator.Current.Children.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					SingleNamespaceOrTypeDeclaration current = enumerator2.Current;
					if (current is SingleNamespaceDeclaration item)
					{
						instance.Add(item);
					}
					else
					{
						instance2.Add((SingleTypeDeclaration)current);
					}
				}
			}
			ArrayBuilder<MergedNamespaceOrTypeDeclaration> instance3 = ArrayBuilder<MergedNamespaceOrTypeDeclaration>.GetInstance();
			switch (instance.Count)
			{
			case 1:
				instance3.Add(Create(instance));
				break;
			case 2:
				if (SingleNamespaceDeclaration.EqualityComparer.Equals(instance[0], instance[1]))
				{
					instance3.Add(Create(instance));
					break;
				}
				instance3.Add(Create(instance[0]));
				instance3.Add(Create(instance[1]));
				break;
			default:
				foreach (IGrouping<SingleNamespaceDeclaration, SingleNamespaceDeclaration> item2 in instance.GroupBy((SingleNamespaceDeclaration n) => n, SingleNamespaceDeclaration.EqualityComparer))
				{
					instance3.Add(Create(item2));
				}
				break;
			case 0:
				break;
			}
			instance.Free();
			if (instance2.Count != 0)
			{
				instance3.AddRange(MergedTypeDeclaration.MakeMergedTypes(instance2));
			}
			instance2.Free();
			return instance3.ToImmutableAndFree();
		}
	}
}
