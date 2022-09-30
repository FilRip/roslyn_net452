using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SourceTypeParameterOnTypeSymbol : SourceTypeParameterSymbol
	{
		private readonly SourceNamedTypeSymbol _container;

		private readonly ImmutableArray<SyntaxReference> _syntaxRefs;

		private VarianceKind _lazyVariance;

		public override TypeParameterKind TypeParameterKind => TypeParameterKind.Type;

		public override VarianceKind Variance
		{
			get
			{
				EnsureAllConstraintsAreResolved();
				return _lazyVariance;
			}
		}

		public override ImmutableArray<Location> Locations
		{
			get
			{
				ArrayBuilder<Location> instance = ArrayBuilder<Location>.GetInstance();
				ImmutableArray<SyntaxReference>.Enumerator enumerator = _syntaxRefs.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SyntaxReference current = enumerator.Current;
					instance.Add(SourceTypeParameterSymbol.GetSymbolLocation(current));
				}
				return instance.ToImmutableAndFree();
			}
		}

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper(_syntaxRefs);

		public override Symbol ContainingSymbol => _container;

		protected override ImmutableArray<TypeParameterSymbol> ContainerTypeParameters => _container.TypeParameters;

		public SourceTypeParameterOnTypeSymbol(SourceNamedTypeSymbol container, int ordinal, string name, ImmutableArray<SyntaxReference> syntaxRefs)
			: base(ordinal, name)
		{
			_container = container;
			_syntaxRefs = syntaxRefs;
		}

		protected override ImmutableArray<TypeParameterConstraint> GetDeclaredConstraints(BindingDiagnosticBag diagnostics)
		{
			ImmutableArray<TypeParameterConstraint> constraints = default(ImmutableArray<TypeParameterConstraint>);
			_container.BindTypeParameterConstraints(this, out var variance, out constraints, diagnostics);
			_lazyVariance = variance;
			return constraints;
		}

		protected override bool ReportRedundantConstraints()
		{
			return true;
		}
	}
}
