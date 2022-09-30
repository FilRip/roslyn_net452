using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SourceParameterSymbolBase : ParameterSymbol
	{
		private readonly Symbol _containingSymbol;

		private readonly ushort _ordinal;

		public sealed override int Ordinal => _ordinal;

		public sealed override Symbol ContainingSymbol => _containingSymbol;

		internal abstract bool HasParamArrayAttribute { get; }

		internal abstract bool HasDefaultValueAttribute { get; }

		internal SourceParameterSymbolBase(Symbol containingSymbol, int ordinal)
		{
			_containingSymbol = containingSymbol;
			_ordinal = (ushort)ordinal;
		}

		internal sealed override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			if (IsParamArray && !HasParamArrayAttribute)
			{
				VisualBasicCompilation declaringCompilation = DeclaringCompilation;
				Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_ParamArrayAttribute__ctor));
			}
			if (HasExplicitDefaultValue && !HasDefaultValueAttribute)
			{
				VisualBasicCompilation declaringCompilation2 = DeclaringCompilation;
				ConstantValue explicitDefaultConstantValue = base.ExplicitDefaultConstantValue;
				switch (explicitDefaultConstantValue.SpecialType)
				{
				case SpecialType.System_DateTime:
					Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation2.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_DateTimeConstantAttribute__ctor, ImmutableArray.Create(new TypedConstant(declaringCompilation2.GetSpecialType(SpecialType.System_Int64), TypedConstantKind.Primitive, explicitDefaultConstantValue.DateTimeValue.Ticks))));
					break;
				case SpecialType.System_Decimal:
					Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation2.SynthesizeDecimalConstantAttribute(explicitDefaultConstantValue.DecimalValue));
					break;
				}
			}
			if (TypeSymbolExtensions.ContainsTupleNames(Type))
			{
				Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.SynthesizeTupleNamesAttribute(Type));
			}
		}

		internal abstract ParameterSymbol WithTypeAndCustomModifiers(TypeSymbol type, ImmutableArray<CustomModifier> customModifiers, ImmutableArray<CustomModifier> refCustomModifiers);
	}
}
