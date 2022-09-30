using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class ReducedParameterSymbolBase : ParameterSymbol
	{
		protected readonly ParameterSymbol m_CurriedFromParameter;

		public abstract override Symbol ContainingSymbol { get; }

		public abstract override TypeSymbol Type { get; }

		public override bool IsByRef => m_CurriedFromParameter.IsByRef;

		internal override bool IsExplicitByRef => m_CurriedFromParameter.IsExplicitByRef;

		public override ImmutableArray<CustomModifier> CustomModifiers => m_CurriedFromParameter.CustomModifiers;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => m_CurriedFromParameter.RefCustomModifiers;

		public override int Ordinal => m_CurriedFromParameter.Ordinal - 1;

		public override bool IsParamArray => m_CurriedFromParameter.IsParamArray;

		public override bool IsOptional => m_CurriedFromParameter.IsOptional;

		internal override ConstantValue ExplicitDefaultConstantValue => m_CurriedFromParameter.get_ExplicitDefaultConstantValue(inProgress);

		internal override bool HasOptionCompare => m_CurriedFromParameter.HasOptionCompare;

		internal override bool IsIDispatchConstant => m_CurriedFromParameter.IsIDispatchConstant;

		internal override bool IsIUnknownConstant => m_CurriedFromParameter.IsIUnknownConstant;

		internal override bool IsCallerLineNumber => m_CurriedFromParameter.IsCallerLineNumber;

		internal override bool IsCallerMemberName => m_CurriedFromParameter.IsCallerMemberName;

		internal override bool IsCallerFilePath => m_CurriedFromParameter.IsCallerFilePath;

		public override bool HasExplicitDefaultValue => m_CurriedFromParameter.HasExplicitDefaultValue;

		public override ImmutableArray<Location> Locations => m_CurriedFromParameter.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => m_CurriedFromParameter.DeclaringSyntaxReferences;

		public override bool IsImplicitlyDeclared => m_CurriedFromParameter.IsImplicitlyDeclared;

		internal sealed override bool IsMetadataOut => m_CurriedFromParameter.IsMetadataOut;

		internal sealed override bool IsMetadataIn => m_CurriedFromParameter.IsMetadataIn;

		internal sealed override MarshalPseudoCustomAttributeData MarshallingInformation => m_CurriedFromParameter.MarshallingInformation;

		public override string Name => m_CurriedFromParameter.Name;

		public override string MetadataName => m_CurriedFromParameter.MetadataName;

		protected ReducedParameterSymbolBase(ParameterSymbol curriedFromParameter)
		{
			m_CurriedFromParameter = curriedFromParameter;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return m_CurriedFromParameter.GetAttributes();
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return m_CurriedFromParameter.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			return m_CurriedFromParameter.GetUseSiteInfo();
		}

		public override int GetHashCode()
		{
			return Hash.Combine(m_CurriedFromParameter.GetHashCode(), ContainingSymbol.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			return obj is ReducedParameterSymbolBase reducedParameterSymbolBase && reducedParameterSymbolBase.m_CurriedFromParameter.Equals(m_CurriedFromParameter) && reducedParameterSymbolBase.ContainingSymbol.Equals(ContainingSymbol);
		}
	}
}
