using System;
using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class TupleMethodSymbol : WrappedMethodSymbol
	{
		private readonly TupleTypeSymbol _containingType;

		private readonly MethodSymbol _underlyingMethod;

		private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

		private ImmutableArray<ParameterSymbol> _lazyParameters;

		public override bool IsTupleMethod => true;

		public override MethodSymbol TupleUnderlyingMethod => _underlyingMethod.ConstructedFrom;

		public override MethodSymbol UnderlyingMethod => _underlyingMethod;

		public override Symbol AssociatedSymbol => _containingType.GetTupleMemberSymbolForUnderlyingMember(_underlyingMethod.ConstructedFrom.AssociatedSymbol);

		public override Symbol ContainingSymbol => _containingType;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => _underlyingMethod.ConstructedFrom.ExplicitInterfaceImplementations;

		public override ImmutableArray<ParameterSymbol> Parameters
		{
			get
			{
				if (_lazyParameters.IsDefault)
				{
					InterlockedOperations.Initialize(ref _lazyParameters, CreateParameters());
				}
				return _lazyParameters;
			}
		}

		public override bool IsSub => _underlyingMethod.IsSub;

		public override TypeSymbol ReturnType => _underlyingMethod.ReturnType;

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => _underlyingMethod.ReturnTypeCustomModifiers;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => _underlyingMethod.RefCustomModifiers;

		public override ImmutableArray<TypeSymbol> TypeArguments => StaticCast<TypeSymbol>.From(_typeParameters);

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

		public TupleMethodSymbol(TupleTypeSymbol container, MethodSymbol underlyingMethod)
		{
			_containingType = container;
			_underlyingMethod = underlyingMethod;
			_typeParameters = _underlyingMethod.TypeParameters;
		}

		private ImmutableArray<ParameterSymbol> CreateParameters()
		{
			return _underlyingMethod.Parameters.SelectAsArray((Func<ParameterSymbol, ParameterSymbol>)((ParameterSymbol p) => new TupleParameterSymbol(this, p)));
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return _underlyingMethod.GetAttributes();
		}

		public override ImmutableArray<VisualBasicAttributeData> GetReturnTypeAttributes()
		{
			return _underlyingMethod.GetReturnTypeAttributes();
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			UseSiteInfo<AssemblySymbol> useSiteInfo = base.GetUseSiteInfo();
			MergeUseSiteInfo(useSiteInfo, _underlyingMethod.GetUseSiteInfo());
			return useSiteInfo;
		}

		public override int GetHashCode()
		{
			return _underlyingMethod.ConstructedFrom.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as TupleMethodSymbol);
		}

		public bool Equals(TupleMethodSymbol other)
		{
			if ((object)other != this)
			{
				if ((object)other != null && TypeSymbol.Equals(_containingType, other._containingType, TypeCompareKind.ConsiderEverything))
				{
					return _underlyingMethod.ConstructedFrom == other._underlyingMethod.ConstructedFrom;
				}
				return false;
			}
			return true;
		}
	}
}
