using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SourceEnumConstantSymbol : SourceFieldSymbol
	{
		private sealed class ZeroValuedEnumConstantSymbol : SourceEnumConstantSymbol
		{
			public ZeroValuedEnumConstantSymbol(SourceNamedTypeSymbol containingEnum, Binder bodyBinder, EnumMemberDeclarationSyntax syntax, BindingDiagnosticBag diagnostics)
				: base(containingEnum, bodyBinder, syntax, diagnostics)
			{
			}

			protected override EvaluatedConstant MakeConstantTuple(ConstantFieldsInProgress.Dependencies dependencies, BindingDiagnosticBag diagnostics)
			{
				NamedTypeSymbol enumUnderlyingType = base.ContainingType.EnumUnderlyingType;
				return new EvaluatedConstant(Microsoft.CodeAnalysis.ConstantValue.Default(enumUnderlyingType.SpecialType), enumUnderlyingType);
			}
		}

		private sealed class ExplicitValuedEnumConstantSymbol : SourceEnumConstantSymbol
		{
			private readonly SyntaxReference _equalsValueNodeRef;

			public ExplicitValuedEnumConstantSymbol(SourceNamedTypeSymbol containingEnum, Binder bodyBinder, EnumMemberDeclarationSyntax syntax, EqualsValueSyntax initializer, BindingDiagnosticBag diagnostics)
				: base(containingEnum, bodyBinder, syntax, diagnostics)
			{
				_equalsValueNodeRef = bodyBinder.GetSyntaxReference(initializer);
			}

			protected override EvaluatedConstant MakeConstantTuple(ConstantFieldsInProgress.Dependencies dependencies, BindingDiagnosticBag diagnostics)
			{
				return ConstantValueUtils.EvaluateFieldConstant(this, _equalsValueNodeRef, dependencies, diagnostics);
			}
		}

		private sealed class ImplicitValuedEnumConstantSymbol : SourceEnumConstantSymbol
		{
			private readonly SourceEnumConstantSymbol _otherConstant;

			private readonly uint _otherConstantOffset;

			public ImplicitValuedEnumConstantSymbol(SourceNamedTypeSymbol containingEnum, Binder bodyBinder, EnumMemberDeclarationSyntax syntax, SourceEnumConstantSymbol otherConstant, uint otherConstantOffset, BindingDiagnosticBag diagnostics)
				: base(containingEnum, bodyBinder, syntax, diagnostics)
			{
				_otherConstant = otherConstant;
				_otherConstantOffset = otherConstantOffset;
			}

			protected override EvaluatedConstant MakeConstantTuple(ConstantFieldsInProgress.Dependencies dependencies, BindingDiagnosticBag diagnostics)
			{
				ConstantValue offsetValue = Microsoft.CodeAnalysis.ConstantValue.Bad;
				ConstantValue constantValue = _otherConstant.GetConstantValue(new ConstantFieldsInProgress(this, dependencies));
				if (!constantValue.IsBad && EnumConstantHelper.OffsetValue(constantValue, _otherConstantOffset, out offsetValue) == EnumOverflowKind.OverflowReport)
				{
					diagnostics.Add(ERRID.ERR_ExpressionOverflow1, base.Locations[0], this);
				}
				return new EvaluatedConstant(offsetValue, base.Type);
			}
		}

		private EvaluatedConstant _constantTuple;

		internal sealed override VisualBasicSyntaxNode DeclarationSyntax => base.Syntax;

		internal sealed override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations => OneOrMany.Create(((EnumMemberDeclarationSyntax)base.Syntax).AttributeLists);

		public sealed override ImmutableArray<Location> Locations => ImmutableArray.Create(((EnumMemberDeclarationSyntax)base.Syntax).Identifier.GetLocation());

		public sealed override TypeSymbol Type => base.ContainingType;

		internal override ParameterSymbol MeParameter => null;

		public static SourceEnumConstantSymbol CreateExplicitValuedConstant(SourceNamedTypeSymbol containingEnum, Binder bodyBinder, EnumMemberDeclarationSyntax syntax, BindingDiagnosticBag diagnostics)
		{
			EqualsValueSyntax initializer = syntax.Initializer;
			return new ExplicitValuedEnumConstantSymbol(containingEnum, bodyBinder, syntax, initializer, diagnostics);
		}

		public static SourceEnumConstantSymbol CreateImplicitValuedConstant(SourceNamedTypeSymbol containingEnum, Binder bodyBinder, EnumMemberDeclarationSyntax syntax, SourceEnumConstantSymbol otherConstant, int otherConstantOffset, BindingDiagnosticBag diagnostics)
		{
			if ((object)otherConstant == null)
			{
				return new ZeroValuedEnumConstantSymbol(containingEnum, bodyBinder, syntax, diagnostics);
			}
			return new ImplicitValuedEnumConstantSymbol(containingEnum, bodyBinder, syntax, otherConstant, (uint)otherConstantOffset, diagnostics);
		}

		protected SourceEnumConstantSymbol(SourceNamedTypeSymbol containingEnum, Binder bodyBinder, EnumMemberDeclarationSyntax syntax, BindingDiagnosticBag diagnostics)
			: base(containingEnum, bodyBinder.GetSyntaxReference(syntax), syntax.Identifier.ValueText, SourceMemberFlags.AccessibilityPublic | SourceMemberFlags.Shared | SourceMemberFlags.Const)
		{
			if (CaseInsensitiveComparison.Equals(base.Name, "value__"))
			{
				diagnostics.Add(ERRID.ERR_ClashWithReservedEnumMember1, syntax.Identifier.GetLocation(), base.Name);
			}
		}

		protected sealed override EvaluatedConstant GetLazyConstantTuple()
		{
			return _constantTuple;
		}

		internal sealed override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress)
		{
			return GetConstantValueImpl(inProgress);
		}

		protected sealed override void SetLazyConstantTuple(EvaluatedConstant constantTuple, BindingDiagnosticBag diagnostics)
		{
			((SourceModuleSymbol)ContainingModule).AtomicStoreReferenceAndDiagnostics(ref _constantTuple, constantTuple, diagnostics);
		}

		protected abstract override EvaluatedConstant MakeConstantTuple(ConstantFieldsInProgress.Dependencies dependencies, BindingDiagnosticBag diagnostics);
	}
}
