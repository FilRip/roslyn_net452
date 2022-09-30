using System.Collections.Immutable;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Emit;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class VisualBasicCustomModifier : CustomModifier, ICustomModifier
	{
		private class OptionalCustomModifier : VisualBasicCustomModifier
		{
			public override bool IsOptional => true;

			public OptionalCustomModifier(NamedTypeSymbol modifier)
				: base(modifier)
			{
			}

			public override int GetHashCode()
			{
				return m_Modifier.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
				{
					return true;
				}
				return obj is OptionalCustomModifier optionalCustomModifier && optionalCustomModifier.m_Modifier.Equals(m_Modifier);
			}
		}

		private class RequiredCustomModifier : VisualBasicCustomModifier
		{
			public override bool IsOptional => false;

			public RequiredCustomModifier(NamedTypeSymbol modifier)
				: base(modifier)
			{
			}

			public override int GetHashCode()
			{
				return m_Modifier.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
				{
					return true;
				}
				return obj is RequiredCustomModifier requiredCustomModifier && requiredCustomModifier.m_Modifier.Equals(m_Modifier);
			}
		}

		protected readonly NamedTypeSymbol m_Modifier;

		private bool CciIsOptional => IsOptional;

		public override INamedTypeSymbol Modifier => m_Modifier;

		public NamedTypeSymbol ModifierSymbol => m_Modifier;

		private ITypeReference CciGetModifier(EmitContext context)
		{
			return ((PEModuleBuilder)context.Module).Translate(ModifierSymbol, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
		}

		ITypeReference ICustomModifier.GetModifier(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in CciGetModifier
			return this.CciGetModifier(context);
		}

		private VisualBasicCustomModifier(NamedTypeSymbol modifier)
		{
			m_Modifier = modifier;
		}

		public abstract override int GetHashCode();

		public abstract override bool Equals(object obj);

		internal static CustomModifier CreateOptional(NamedTypeSymbol modifier)
		{
			return new OptionalCustomModifier(modifier);
		}

		internal static CustomModifier CreateRequired(NamedTypeSymbol modifier)
		{
			return new RequiredCustomModifier(modifier);
		}

		internal static ImmutableArray<CustomModifier> Convert(ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers)
		{
			if (customModifiers.IsDefault)
			{
				return ImmutableArray<CustomModifier>.Empty;
			}
			return customModifiers.SelectAsArray(Convert);
		}

		private static CustomModifier Convert(ModifierInfo<TypeSymbol> customModifier)
		{
			NamedTypeSymbol modifier = (NamedTypeSymbol)customModifier.Modifier;
			if (!customModifier.IsOptional)
			{
				return CreateRequired(modifier);
			}
			return CreateOptional(modifier);
		}
	}
}
