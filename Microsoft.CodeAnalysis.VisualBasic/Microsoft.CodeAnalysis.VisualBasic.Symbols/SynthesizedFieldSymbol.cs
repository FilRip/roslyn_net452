using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SynthesizedFieldSymbol : FieldSymbol
	{
		protected readonly NamedTypeSymbol _containingType;

		protected readonly Symbol _implicitlyDefinedBy;

		protected readonly TypeSymbol _type;

		protected readonly string _name;

		protected readonly SourceMemberFlags _flags;

		protected readonly bool _isSpecialNameAndRuntimeSpecial;

		public override TypeSymbol Type => _type;

		public override ImmutableArray<CustomModifier> CustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override Symbol AssociatedSymbol => null;

		public override bool IsReadOnly => (_flags & SourceMemberFlags.ReadOnly) != 0;

		public override bool IsConst => false;

		public override Symbol ContainingSymbol => _containingType;

		public override NamedTypeSymbol ContainingType => _containingType;

		public override ImmutableArray<Location> Locations => _implicitlyDefinedBy.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override Accessibility DeclaredAccessibility => (Accessibility)(_flags & SourceMemberFlags.AccessibilityMask);

		public override bool IsShared => (_flags & SourceMemberFlags.Shared) != 0;

		public override bool IsImplicitlyDeclared => true;

		internal override Symbol ImplicitlyDefinedBy => _implicitlyDefinedBy;

		public override string Name => _name;

		internal override bool HasSpecialName => _isSpecialNameAndRuntimeSpecial;

		internal override bool HasRuntimeSpecialName => _isSpecialNameAndRuntimeSpecial;

		internal override bool IsNotSerialized => false;

		internal override MarshalPseudoCustomAttributeData MarshallingInformation => null;

		internal override int? TypeLayoutOffset => null;

		internal override ObsoleteAttributeData ObsoleteAttributeData => null;

		public SynthesizedFieldSymbol(NamedTypeSymbol containingType, Symbol implicitlyDefinedBy, TypeSymbol type, string name, Accessibility accessibility = Accessibility.Private, bool isReadOnly = false, bool isShared = false, bool isSpecialNameAndRuntimeSpecial = false)
		{
			_containingType = containingType;
			_implicitlyDefinedBy = implicitlyDefinedBy;
			_type = type;
			_name = name;
			_flags = (SourceMemberFlags)((int)accessibility | (int)(isReadOnly ? SourceMemberFlags.ReadOnly : SourceMemberFlags.None) | (int)(isShared ? SourceMemberFlags.Shared : SourceMemberFlags.None));
			_isSpecialNameAndRuntimeSpecial = isSpecialNameAndRuntimeSpecial;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			if (!_isSpecialNameAndRuntimeSpecial)
			{
				VisualBasicCompilation declaringCompilation = DeclaringCompilation;
				if (TypeSymbolExtensions.ContainsTupleNames(Type) && declaringCompilation.HasTupleNamesAttributes)
				{
					Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeTupleNamesAttribute(Type));
				}
				if (ContainingSymbol is SourceMemberContainerTypeSymbol)
				{
					Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
				}
			}
		}

		internal override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress)
		{
			return null;
		}

		internal override LexicalSortKey GetLexicalSortKey()
		{
			return _implicitlyDefinedBy.GetLexicalSortKey();
		}
	}
}
