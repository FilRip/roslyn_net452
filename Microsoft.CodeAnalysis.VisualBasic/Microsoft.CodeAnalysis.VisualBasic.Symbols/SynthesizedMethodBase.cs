using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading;
using Microsoft.Cci;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SynthesizedMethodBase : MethodSymbol
	{
		protected readonly NamedTypeSymbol m_containingType;

		private ParameterSymbol _lazyMeParameter;

		public override int Arity => 0;

		public override Symbol AssociatedSymbol => null;

		internal sealed override CallingConvention CallingConvention => ((!IsShared) ? CallingConvention.HasThis : CallingConvention.Default) | (IsGenericMethod ? CallingConvention.Generic : CallingConvention.Default);

		public sealed override Symbol ContainingSymbol => m_containingType;

		public sealed override NamedTypeSymbol ContainingType => m_containingType;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

		public override MethodSymbol OverriddenMethod => null;

		public sealed override bool IsExtensionMethod => false;

		public override bool IsExternalMethod => false;

		internal sealed override MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation => null;

		internal override MethodImplAttributes ImplementationAttributes
		{
			get
			{
				if (!m_containingType.IsComImport || m_containingType.IsInterface)
				{
					return MethodImplAttributes.IL;
				}
				return (MethodImplAttributes)4099;
			}
		}

		internal override bool HasDeclarativeSecurity => false;

		public override bool ReturnsByRef => false;

		public sealed override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override bool IsVararg => false;

		public override ImmutableArray<TypeSymbol> TypeArguments => ImmutableArray<TypeSymbol>.Empty;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public sealed override bool IsImplicitlyDeclared => true;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		internal override SyntaxNode Syntax => null;

		public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

		public override bool IsAsync => false;

		public override bool IsIterator => false;

		public sealed override bool IsInitOnly => false;

		internal sealed override bool IsMethodKindBasedOnSyntax => false;

		protected SynthesizedMethodBase(NamedTypeSymbol container)
		{
			m_containingType = container;
		}

		public sealed override DllImportData GetDllImportData()
		{
			return null;
		}

		internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetReturnTypeAttributes()
		{
			return ImmutableArray<VisualBasicAttributeData>.Empty;
		}

		internal sealed override bool TryGetMeParameter(out ParameterSymbol meParameter)
		{
			if (IsShared)
			{
				meParameter = null;
			}
			else
			{
				if ((object)_lazyMeParameter == null)
				{
					Interlocked.CompareExchange(ref _lazyMeParameter, new MeParameterSymbol(this), null);
				}
				meParameter = _lazyMeParameter;
			}
			return true;
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return ImmutableArray<string>.Empty;
		}
	}
}
