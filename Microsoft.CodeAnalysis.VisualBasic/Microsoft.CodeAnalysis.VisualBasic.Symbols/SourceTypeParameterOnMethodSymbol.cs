using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SourceTypeParameterOnMethodSymbol : SourceTypeParameterSymbol
	{
		private readonly SourceMemberMethodSymbol _container;

		private readonly SyntaxReference _syntaxRef;

		public override Symbol ContainingSymbol => _container;

		public override TypeParameterKind TypeParameterKind => TypeParameterKind.Method;

		public override VarianceKind Variance => VarianceKind.None;

		public override ImmutableArray<Location> Locations => ImmutableArray.Create(SourceTypeParameterSymbol.GetSymbolLocation(_syntaxRef));

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper(_syntaxRef);

		protected override ImmutableArray<TypeParameterSymbol> ContainerTypeParameters => _container.TypeParameters;

		public SourceTypeParameterOnMethodSymbol(SourceMemberMethodSymbol container, int ordinal, string name, SyntaxReference syntaxRef)
			: base(ordinal, name)
		{
			_container = container;
			_syntaxRef = syntaxRef;
		}

		protected override ImmutableArray<TypeParameterConstraint> GetDeclaredConstraints(BindingDiagnosticBag diagnostics)
		{
			TypeParameterSyntax syntax = (TypeParameterSyntax)_syntaxRef.GetSyntax();
			return _container.BindTypeParameterConstraints(syntax, diagnostics);
		}

		protected override bool ReportRedundantConstraints()
		{
			if (_container.IsOverrides)
			{
				return false;
			}
			if (_container.DeclaredAccessibility == Accessibility.Private && _container.HasExplicitInterfaceImplementations())
			{
				return false;
			}
			return true;
		}
	}
}
