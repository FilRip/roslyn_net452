using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SynthesizedConstructorBase : SynthesizedMethodBase
	{
		protected readonly bool m_isShared;

		protected readonly SyntaxReference m_syntaxReference;

		protected readonly TypeSymbol m_voidType;

		public sealed override string Name
		{
			get
			{
				if (!m_isShared)
				{
					return ".ctor";
				}
				return ".cctor";
			}
		}

		internal sealed override bool HasSpecialName => true;

		public override Accessibility DeclaredAccessibility
		{
			get
			{
				if (IsShared)
				{
					return Accessibility.Private;
				}
				if (m_containingType.IsMustInherit)
				{
					return Accessibility.Protected;
				}
				return Accessibility.Public;
			}
		}

		public sealed override bool IsMustOverride => false;

		public sealed override bool IsNotOverridable => false;

		public override bool IsOverloads => false;

		public sealed override bool IsOverridable => false;

		public sealed override bool IsOverrides => false;

		public sealed override bool IsShared => m_isShared;

		public sealed override bool IsSub => true;

		public override ImmutableArray<Location> Locations => m_containingType.Locations;

		public sealed override MethodKind MethodKind
		{
			get
			{
				if (!m_isShared)
				{
					return MethodKind.Constructor;
				}
				return MethodKind.StaticConstructor;
			}
		}

		public sealed override TypeSymbol ReturnType => m_voidType;

		internal sealed override SyntaxNode Syntax
		{
			get
			{
				if (m_syntaxReference != null)
				{
					return (VisualBasicSyntaxNode)m_syntaxReference.GetSyntax();
				}
				return null;
			}
		}

		public override bool IsExternalMethod => base.ContainingType?.IsComImport ?? false;

		protected SynthesizedConstructorBase(SyntaxReference syntaxReference, NamedTypeSymbol container, bool isShared, Binder binder, BindingDiagnosticBag diagnostics)
			: base(container)
		{
			m_syntaxReference = syntaxReference;
			m_isShared = isShared;
			if (binder != null)
			{
				m_voidType = binder.GetSpecialType(SpecialType.System_Void, syntaxReference.GetSyntax(), diagnostics);
			}
			else
			{
				m_voidType = ContainingAssembly.GetSpecialType(SpecialType.System_Void);
			}
		}

		internal override LexicalSortKey GetLexicalSortKey()
		{
			return m_containingType.GetLexicalSortKey();
		}
	}
}
