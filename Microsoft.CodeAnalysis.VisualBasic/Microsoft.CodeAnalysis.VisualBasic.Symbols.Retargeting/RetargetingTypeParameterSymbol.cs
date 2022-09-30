using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
{
	internal sealed class RetargetingTypeParameterSymbol : SubstitutableTypeParameterSymbol
	{
		private readonly RetargetingModuleSymbol _retargetingModule;

		private readonly TypeParameterSymbol _underlyingTypeParameter;

		private RetargetingModuleSymbol.RetargetingSymbolTranslator RetargetingTranslator => _retargetingModule.RetargetingTranslator;

		public TypeParameterSymbol UnderlyingTypeParameter => _underlyingTypeParameter;

		public override TypeParameterKind TypeParameterKind => _underlyingTypeParameter.TypeParameterKind;

		public override bool IsImplicitlyDeclared => _underlyingTypeParameter.IsImplicitlyDeclared;

		public override int Ordinal => _underlyingTypeParameter.Ordinal;

		internal override ImmutableArray<TypeSymbol> ConstraintTypesNoUseSiteDiagnostics => RetargetingTranslator.Retarget(_underlyingTypeParameter.ConstraintTypesNoUseSiteDiagnostics);

		public override bool HasConstructorConstraint => _underlyingTypeParameter.HasConstructorConstraint;

		public override bool HasReferenceTypeConstraint => _underlyingTypeParameter.HasReferenceTypeConstraint;

		public override bool HasValueTypeConstraint => _underlyingTypeParameter.HasValueTypeConstraint;

		public override VarianceKind Variance => _underlyingTypeParameter.Variance;

		public override Symbol ContainingSymbol => RetargetingTranslator.Retarget(_underlyingTypeParameter.ContainingSymbol);

		public override ImmutableArray<Location> Locations => _underlyingTypeParameter.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _underlyingTypeParameter.DeclaringSyntaxReferences;

		public override AssemblySymbol ContainingAssembly => _retargetingModule.ContainingAssembly;

		public override ModuleSymbol ContainingModule => _retargetingModule;

		public override string Name => _underlyingTypeParameter.Name;

		public override string MetadataName => _underlyingTypeParameter.MetadataName;

		internal override VisualBasicCompilation DeclaringCompilation => null;

		public RetargetingTypeParameterSymbol(RetargetingModuleSymbol retargetingModule, TypeParameterSymbol underlyingTypeParameter)
		{
			if (underlyingTypeParameter is RetargetingTypeParameterSymbol)
			{
				throw new ArgumentException();
			}
			_retargetingModule = retargetingModule;
			_underlyingTypeParameter = underlyingTypeParameter;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return _underlyingTypeParameter.GetAttributes();
		}

		internal override void EnsureAllConstraintsAreResolved()
		{
			_underlyingTypeParameter.EnsureAllConstraintsAreResolved();
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _underlyingTypeParameter.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}
	}
}
