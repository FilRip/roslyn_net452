using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SynthesizedBackingFieldBase<T> : FieldSymbol where T : Symbol
	{
		protected readonly T _propertyOrEvent;

		protected readonly string _name;

		protected readonly bool _isShared;

		public override ImmutableArray<CustomModifier> CustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public sealed override Symbol AssociatedSymbol => _propertyOrEvent;

		internal sealed override bool ShadowsExplicitly => _propertyOrEvent.ShadowsExplicitly;

		public override bool IsReadOnly => false;

		public override bool IsConst => false;

		public sealed override Symbol ContainingSymbol => _propertyOrEvent.ContainingType;

		public sealed override NamedTypeSymbol ContainingType => _propertyOrEvent.ContainingType;

		public sealed override ImmutableArray<Location> Locations => _propertyOrEvent.Locations;

		public sealed override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public sealed override Accessibility DeclaredAccessibility => Accessibility.Private;

		public sealed override bool IsShared => _isShared;

		public sealed override bool IsImplicitlyDeclared => true;

		internal sealed override Symbol ImplicitlyDefinedBy => _propertyOrEvent;

		public sealed override string Name => _name;

		internal override bool HasSpecialName => false;

		internal override bool HasRuntimeSpecialName => false;

		internal override bool IsNotSerialized => false;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

		internal override MarshalPseudoCustomAttributeData MarshallingInformation => null;

		internal override int? TypeLayoutOffset => null;

		public SynthesizedBackingFieldBase(T propertyOrEvent, string name, bool isShared)
		{
			_propertyOrEvent = propertyOrEvent;
			_name = name;
			_isShared = isShared;
		}

		internal override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress)
		{
			return null;
		}

		internal sealed override LexicalSortKey GetLexicalSortKey()
		{
			return _propertyOrEvent.GetLexicalSortKey();
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			VisualBasicCompilation declaringCompilation = DeclaringCompilation;
			if (!ContainingType.IsImplicitlyDeclared)
			{
				Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
			}
			Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeDebuggerBrowsableNeverAttribute());
			if (TypeSymbolExtensions.ContainsTupleNames(Type) && declaringCompilation.HasTupleNamesAttributes)
			{
				Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeTupleNamesAttribute(Type));
			}
		}
	}
}
