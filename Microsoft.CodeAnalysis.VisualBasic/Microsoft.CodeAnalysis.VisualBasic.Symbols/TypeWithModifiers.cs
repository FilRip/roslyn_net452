using System;
using System.Collections.Immutable;
using System.Linq;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal struct TypeWithModifiers : IEquatable<TypeWithModifiers>
	{
		public readonly TypeSymbol Type;

		public readonly ImmutableArray<CustomModifier> CustomModifiers;

		public TypeWithModifiers(TypeSymbol type, ImmutableArray<CustomModifier> customModifiers)
		{
			this = default(TypeWithModifiers);
			Type = type;
			CustomModifiers = customModifiers.NullToEmpty();
		}

		public TypeWithModifiers(TypeSymbol type)
		{
			this = default(TypeWithModifiers);
			Type = type;
			CustomModifiers = ImmutableArray<CustomModifier>.Empty;
		}

		[Obsolete("Use the strongly typed overload.", true)]
		public override bool Equals(object obj)
		{
			if (obj is TypeWithModifiers)
			{
				return Equals((TypeWithModifiers)obj);
			}
			return false;
		}

		public bool Equals(TypeWithModifiers other)
		{
			return IsSameType(other, TypeCompareKind.ConsiderEverything);
		}

		bool IEquatable<TypeWithModifiers>.Equals(TypeWithModifiers other)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Equals
			return this.Equals(other);
		}

		internal bool IsSameType(TypeWithModifiers other, TypeCompareKind compareKind)
		{
			if (!TypeSymbolExtensions.IsSameType(Type, other.Type, compareKind))
			{
				return false;
			}
			if ((compareKind & TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds) == 0)
			{
				return CustomModifiers.IsDefault ? other.CustomModifiers.IsDefault : (!other.CustomModifiers.IsDefault && CustomModifiers.SequenceEqual(other.CustomModifiers));
			}
			return true;
		}

		public static bool operator ==(TypeWithModifiers x, TypeWithModifiers y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(TypeWithModifiers x, TypeWithModifiers y)
		{
			return !x.Equals(y);
		}

		public override int GetHashCode()
		{
			return Hash.Combine(Type, Hash.CombineValues(CustomModifiers));
		}

		public bool Is(TypeSymbol other)
		{
			if (TypeSymbol.Equals(Type, other, TypeCompareKind.ConsiderEverything))
			{
				return CustomModifiers.IsEmpty;
			}
			return false;
		}

		[Obsolete("Use Is method.", true)]
		public bool Equals(TypeSymbol other)
		{
			return Is(other);
		}

		public TypeSymbol AsTypeSymbolOnly()
		{
			return Type;
		}

		public TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution substitution)
		{
			ImmutableArray<CustomModifier> immutableArray = substitution?.SubstituteCustomModifiers(CustomModifiers) ?? CustomModifiers;
			TypeWithModifiers typeWithModifiers = Type.InternalSubstituteTypeParameters(substitution);
			if (!typeWithModifiers.Is(Type) || immutableArray != CustomModifiers)
			{
				return new TypeWithModifiers(typeWithModifiers.Type, immutableArray.Concat(typeWithModifiers.CustomModifiers));
			}
			return this;
		}
	}
}
