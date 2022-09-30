using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SourceWithEventsBackingFieldSymbol : SourceMemberFieldSymbol
	{
		private readonly SourcePropertySymbol _property;

		public override Symbol AssociatedSymbol => _property;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override bool IsImplicitlyDeclared => true;

		internal override Symbol ImplicitlyDefinedBy => _property;

		public SourceWithEventsBackingFieldSymbol(SourcePropertySymbol property, SyntaxReference syntaxRef, string name)
			: base(property.ContainingSourceType, syntaxRef, name, SourceMemberFlags.AccessibilityPrivate | (property.IsShared ? SourceMemberFlags.Shared : SourceMemberFlags.None))
		{
			_property = property;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			VisualBasicCompilation declaringCompilation = _property.DeclaringCompilation;
			Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
			Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeDebuggerBrowsableNeverAttribute());
			Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_AccessedThroughPropertyAttribute__ctor, ImmutableArray.Create(new TypedConstant(declaringCompilation.GetSpecialType(SpecialType.System_String), TypedConstantKind.Primitive, AssociatedSymbol.Name))));
		}
	}
}
