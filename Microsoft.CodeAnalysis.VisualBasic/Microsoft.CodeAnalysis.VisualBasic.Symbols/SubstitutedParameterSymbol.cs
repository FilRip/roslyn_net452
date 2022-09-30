using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SubstitutedParameterSymbol : ParameterSymbol
	{
		internal class SubstitutedMethodParameterSymbol : SubstitutedParameterSymbol
		{
			private readonly SubstitutedMethodSymbol _container;

			public override Symbol ContainingSymbol => _container;

			protected override TypeSubstitution TypeSubstitution => _container.TypeSubstitution;

			public SubstitutedMethodParameterSymbol(SubstitutedMethodSymbol container, ParameterSymbol originalDefinition)
				: base(originalDefinition)
			{
				_container = container;
			}
		}

		private sealed class SubstitutedPropertyParameterSymbol : SubstitutedParameterSymbol
		{
			private readonly SubstitutedPropertySymbol _container;

			public override Symbol ContainingSymbol => _container;

			protected override TypeSubstitution TypeSubstitution => _container.TypeSubstitution;

			public SubstitutedPropertyParameterSymbol(SubstitutedPropertySymbol container, ParameterSymbol originalDefinition)
				: base(originalDefinition)
			{
				_container = container;
			}
		}

		private readonly ParameterSymbol _originalDefinition;

		public override string Name => _originalDefinition.Name;

		public override string MetadataName => _originalDefinition.MetadataName;

		public override int Ordinal => _originalDefinition.Ordinal;

		public abstract override Symbol ContainingSymbol { get; }

		internal override ConstantValue ExplicitDefaultConstantValue => _originalDefinition.get_ExplicitDefaultConstantValue(inProgress);

		public override bool HasExplicitDefaultValue => _originalDefinition.HasExplicitDefaultValue;

		public override bool IsOptional => _originalDefinition.IsOptional;

		public override bool IsParamArray => _originalDefinition.IsParamArray;

		public override bool IsImplicitlyDeclared => _originalDefinition.IsImplicitlyDeclared;

		public override ImmutableArray<Location> Locations => _originalDefinition.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _originalDefinition.DeclaringSyntaxReferences;

		public override TypeSymbol Type => _originalDefinition.Type.InternalSubstituteTypeParameters(TypeSubstitution).Type;

		public override bool IsByRef => _originalDefinition.IsByRef;

		internal override bool IsMetadataOut => _originalDefinition.IsMetadataOut;

		internal override bool IsMetadataIn => _originalDefinition.IsMetadataIn;

		internal override MarshalPseudoCustomAttributeData MarshallingInformation => _originalDefinition.MarshallingInformation;

		internal override bool HasOptionCompare => _originalDefinition.HasOptionCompare;

		internal override bool IsIDispatchConstant => _originalDefinition.IsIDispatchConstant;

		internal override bool IsIUnknownConstant => _originalDefinition.IsIUnknownConstant;

		internal override bool IsCallerLineNumber => _originalDefinition.IsCallerLineNumber;

		internal override bool IsCallerMemberName => _originalDefinition.IsCallerMemberName;

		internal override bool IsCallerFilePath => _originalDefinition.IsCallerFilePath;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => TypeSubstitution.SubstituteCustomModifiers(_originalDefinition.RefCustomModifiers);

		internal override bool IsExplicitByRef => _originalDefinition.IsExplicitByRef;

		public override ImmutableArray<CustomModifier> CustomModifiers => TypeSubstitution.SubstituteCustomModifiers(_originalDefinition.Type, _originalDefinition.CustomModifiers);

		public override ParameterSymbol OriginalDefinition => _originalDefinition;

		protected abstract TypeSubstitution TypeSubstitution { get; }

		public static SubstitutedParameterSymbol CreateMethodParameter(SubstitutedMethodSymbol container, ParameterSymbol originalDefinition)
		{
			return new SubstitutedMethodParameterSymbol(container, originalDefinition);
		}

		public static SubstitutedParameterSymbol CreatePropertyParameter(SubstitutedPropertySymbol container, ParameterSymbol originalDefinition)
		{
			return new SubstitutedPropertyParameterSymbol(container, originalDefinition);
		}

		protected SubstitutedParameterSymbol(ParameterSymbol originalDefinition)
		{
			_originalDefinition = originalDefinition;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return _originalDefinition.GetAttributes();
		}

		public override int GetHashCode()
		{
			int hashCode = _originalDefinition.GetHashCode();
			return Hash.Combine(ContainingSymbol, hashCode);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is SubstitutedParameterSymbol substitutedParameterSymbol))
			{
				return false;
			}
			if (!_originalDefinition.Equals(substitutedParameterSymbol._originalDefinition))
			{
				return false;
			}
			if (!ContainingSymbol.Equals(substitutedParameterSymbol.ContainingSymbol))
			{
				return false;
			}
			return true;
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _originalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}
	}
}
