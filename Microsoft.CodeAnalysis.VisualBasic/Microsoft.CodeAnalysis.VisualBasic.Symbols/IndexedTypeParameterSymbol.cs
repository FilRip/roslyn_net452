using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class IndexedTypeParameterSymbol : TypeParameterSymbol
	{
		private static TypeParameterSymbol[] s_parameterPool = Array.Empty<TypeParameterSymbol>();

		private readonly int _index;

		public override TypeParameterKind TypeParameterKind => TypeParameterKind.Method;

		public override int Ordinal => _index;

		public override VarianceKind Variance => VarianceKind.None;

		public override bool HasValueTypeConstraint => false;

		public override bool HasReferenceTypeConstraint => false;

		public override bool HasConstructorConstraint => false;

		internal override ImmutableArray<TypeSymbol> ConstraintTypesNoUseSiteDiagnostics => ImmutableArray<TypeSymbol>.Empty;

		public override Symbol ContainingSymbol => null;

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		private IndexedTypeParameterSymbol(int index)
		{
			_index = index;
		}

		internal static TypeParameterSymbol GetTypeParameter(int index)
		{
			if (index >= s_parameterPool.Length)
			{
				GrowPool(index + 1);
			}
			return s_parameterPool[index];
		}

		private static void GrowPool(int count)
		{
			TypeParameterSymbol[] array = s_parameterPool;
			while (count > array.Length)
			{
				TypeParameterSymbol[] array2 = new TypeParameterSymbol[((count + 15) & -16) - 1 + 1];
				Array.Copy(array, array2, array.Length);
				int num = array.Length;
				int num2 = array2.Length - 1;
				for (int i = num; i <= num2; i++)
				{
					array2[i] = new IndexedTypeParameterSymbol(i);
				}
				Interlocked.CompareExchange(ref s_parameterPool, array2, array);
				array = s_parameterPool;
			}
		}

		internal static ImmutableArray<TypeParameterSymbol> Take(int count)
		{
			if (count > s_parameterPool.Length)
			{
				GrowPool(count);
			}
			ArrayBuilder<TypeParameterSymbol> instance = ArrayBuilder<TypeParameterSymbol>.GetInstance();
			int num = count - 1;
			for (int i = 0; i <= num; i++)
			{
				instance.Add(GetTypeParameter(i));
			}
			return instance.ToImmutableAndFree();
		}

		public override bool Equals(TypeSymbol other, TypeCompareKind comparison)
		{
			return (object)this == other;
		}

		public override int GetHashCode()
		{
			return _index;
		}

		internal override void EnsureAllConstraintsAreResolved()
		{
		}
	}
}
