using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SynthesizedParameterSymbol : SynthesizedParameterSimpleSymbol
	{
		private readonly bool _isByRef;

		private readonly bool _isOptional;

		private readonly ConstantValue _defaultValue;

		public sealed override bool IsByRef => _isByRef;

		public sealed override bool IsOptional => _isOptional;

		internal sealed override ConstantValue ExplicitDefaultConstantValue => _defaultValue;

		public sealed override bool HasExplicitDefaultValue => (object)_defaultValue != null;

		public SynthesizedParameterSymbol(MethodSymbol container, TypeSymbol type, int ordinal, bool isByRef, string name = "")
			: this(container, type, ordinal, isByRef, name, isOptional: false, null)
		{
		}

		public SynthesizedParameterSymbol(MethodSymbol container, TypeSymbol type, int ordinal, bool isByRef, string name, bool isOptional, ConstantValue defaultValue)
			: base(container, type, ordinal, name)
		{
			_isByRef = isByRef;
			_isOptional = isOptional;
			_defaultValue = defaultValue;
		}

		public static SynthesizedParameterSymbol Create(MethodSymbol container, TypeSymbol type, int ordinal, bool isByRef, string name, ImmutableArray<CustomModifier> customModifiers, ImmutableArray<CustomModifier> refCustomModifiers)
		{
			if (customModifiers.IsEmpty && refCustomModifiers.IsEmpty)
			{
				return new SynthesizedParameterSymbol(container, type, ordinal, isByRef, name, isOptional: false, null);
			}
			return new SynthesizedParameterSymbolWithCustomModifiers(container, type, ordinal, isByRef, name, customModifiers, refCustomModifiers);
		}

		internal static ParameterSymbol CreateSetAccessorValueParameter(MethodSymbol setter, PropertySymbol propertySymbol, string parameterName)
		{
			TypeSymbol type = propertySymbol.Type;
			ImmutableArray<CustomModifier> customModifiers = propertySymbol.TypeCustomModifiers;
			MethodSymbol overriddenMethod = setter.OverriddenMethod;
			if ((object)overriddenMethod != null)
			{
				ParameterSymbol parameterSymbol = overriddenMethod.Parameters[propertySymbol.ParameterCount];
				if (TypeSymbolExtensions.IsSameTypeIgnoringAll(parameterSymbol.Type, type))
				{
					type = parameterSymbol.Type;
					customModifiers = parameterSymbol.CustomModifiers;
				}
			}
			if (customModifiers.IsEmpty)
			{
				return new SynthesizedParameterSimpleSymbol(setter, type, propertySymbol.ParameterCount, parameterName);
			}
			return new SynthesizedParameterSymbolWithCustomModifiers(setter, type, propertySymbol.ParameterCount, isByRef: false, parameterName, customModifiers, ImmutableArray<CustomModifier>.Empty);
		}
	}
}
