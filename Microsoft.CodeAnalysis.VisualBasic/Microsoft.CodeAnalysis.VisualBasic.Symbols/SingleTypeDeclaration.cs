using System.Collections.Generic;
using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SingleTypeDeclaration : SingleNamespaceOrTypeDeclaration
	{
		internal enum TypeDeclarationFlags : byte
		{
			None = 0,
			HasAnyAttributes = 2,
			HasBaseDeclarations = 4,
			AnyMemberHasAttributes = 8,
			HasAnyNontypeMembers = 0x10
		}

		private sealed class Comparer : IEqualityComparer<SingleTypeDeclaration>
		{
			private bool Equals(SingleTypeDeclaration decl1, SingleTypeDeclaration decl2)
			{
				if (CaseInsensitiveComparison.Equals(decl1.Name, decl2.Name) && decl1.Kind == decl2.Kind && decl1.Kind != DeclarationKind.Enum && decl1.Arity == decl2.Arity)
				{
					return decl1.GetEmbeddedSymbolKind() == decl2.GetEmbeddedSymbolKind();
				}
				return false;
			}

			bool IEqualityComparer<SingleTypeDeclaration>.Equals(SingleTypeDeclaration decl1, SingleTypeDeclaration decl2)
			{
				//ILSpy generated this explicit interface implementation from .override directive in Equals
				return this.Equals(decl1, decl2);
			}

			private int GetHashCode(SingleTypeDeclaration decl1)
			{
				return Hash.Combine(CaseInsensitiveComparison.GetHashCode(decl1.Name), Hash.Combine(decl1.Arity.GetHashCode(), (int)decl1.Kind));
			}

			int IEqualityComparer<SingleTypeDeclaration>.GetHashCode(SingleTypeDeclaration decl1)
			{
				//ILSpy generated this explicit interface implementation from .override directive in GetHashCode
				return this.GetHashCode(decl1);
			}
		}

		private readonly ImmutableArray<SingleTypeDeclaration> _children;

		private readonly DeclarationKind _kind;

		private readonly TypeDeclarationFlags _flags;

		private readonly ushort _arity;

		private readonly DeclarationModifiers _modifiers;

		public static readonly IEqualityComparer<SingleTypeDeclaration> EqualityComparer = new Comparer();

		public override DeclarationKind Kind => _kind;

		public int Arity => _arity;

		public bool HasAnyAttributes => (_flags & TypeDeclarationFlags.HasAnyAttributes) != 0;

		public bool HasBaseDeclarations => (_flags & TypeDeclarationFlags.HasBaseDeclarations) != 0;

		public bool HasAnyNontypeMembers => (_flags & TypeDeclarationFlags.HasAnyNontypeMembers) != 0;

		public bool AnyMemberHasAttributes => (_flags & TypeDeclarationFlags.AnyMemberHasAttributes) != 0;

		public new ImmutableArray<SingleTypeDeclaration> Children => _children;

		public DeclarationModifiers Modifiers => _modifiers;

		public ImmutableHashSet<string> MemberNames { get; }

		public SingleTypeDeclaration(DeclarationKind kind, string name, int arity, DeclarationModifiers modifiers, TypeDeclarationFlags declFlags, SyntaxReference syntaxReference, Location nameLocation, ImmutableHashSet<string> memberNames, ImmutableArray<SingleTypeDeclaration> children)
			: base(name, syntaxReference, nameLocation)
		{
			_kind = kind;
			_arity = (ushort)arity;
			_flags = declFlags;
			_modifiers = modifiers;
			MemberNames = memberNames;
			_children = children;
		}

		protected override ImmutableArray<SingleNamespaceOrTypeDeclaration> GetNamespaceOrTypeDeclarationChildren()
		{
			return StaticCast<SingleNamespaceOrTypeDeclaration>.From(_children);
		}

		private EmbeddedSymbolKind GetEmbeddedSymbolKind()
		{
			return EmbeddedSymbolExtensions.GetEmbeddedKind(SyntaxReference.SyntaxTree);
		}
	}
}
