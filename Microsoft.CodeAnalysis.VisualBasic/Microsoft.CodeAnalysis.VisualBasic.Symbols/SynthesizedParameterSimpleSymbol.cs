using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SynthesizedParameterSimpleSymbol : ParameterSymbol
	{
		protected readonly MethodSymbol _container;

		protected readonly TypeSymbol _type;

		protected readonly int _ordinal;

		protected readonly string _name;

		public sealed override Symbol ContainingSymbol => _container;

		public override ImmutableArray<CustomModifier> CustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		internal override ConstantValue ExplicitDefaultConstantValue => null;

		public override bool HasExplicitDefaultValue => false;

		public override bool IsByRef => false;

		internal override bool IsMetadataIn => false;

		internal override bool IsMetadataOut => false;

		internal override MarshalPseudoCustomAttributeData MarshallingInformation
		{
			get
			{
				MethodSymbol methodSymbol = (MethodSymbol)ContainingSymbol;
				if (methodSymbol.MethodKind == MethodKind.PropertySet && IsMarshalAsAttributeApplicable(methodSymbol))
				{
					return ((SourcePropertySymbol)methodSymbol.AssociatedSymbol).ReturnTypeMarshallingInformation;
				}
				return null;
			}
		}

		internal override bool IsExplicitByRef => IsByRef;

		internal override bool HasOptionCompare => false;

		internal override bool IsIDispatchConstant => false;

		internal override bool IsIUnknownConstant => false;

		internal override bool IsCallerLineNumber => false;

		internal override bool IsCallerMemberName => false;

		internal override bool IsCallerFilePath => false;

		public override bool IsOptional => false;

		public override bool IsParamArray => false;

		public override bool IsImplicitlyDeclared => true;

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public sealed override int Ordinal => _ordinal;

		public sealed override TypeSymbol Type => _type;

		public sealed override string Name => _name;

		public SynthesizedParameterSimpleSymbol(MethodSymbol container, TypeSymbol type, int ordinal, string name)
		{
			_container = container;
			_type = type;
			_ordinal = ordinal;
			_name = name;
		}

		internal static bool IsMarshalAsAttributeApplicable(MethodSymbol propertySetter)
		{
			return propertySetter.ContainingType.IsInterface;
		}

		internal sealed override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			VisualBasicCompilation declaringCompilation = DeclaringCompilation;
			if (TypeSymbolExtensions.ContainsTupleNames(Type) && declaringCompilation.HasTupleNamesAttributes)
			{
				Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeTupleNamesAttribute(Type));
			}
		}
	}
}
