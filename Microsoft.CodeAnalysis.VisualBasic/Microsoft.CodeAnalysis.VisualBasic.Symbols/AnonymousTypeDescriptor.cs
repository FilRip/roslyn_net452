using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal struct AnonymousTypeDescriptor : IEquatable<AnonymousTypeDescriptor>
	{
		public const string SubReturnParameterName = "Sub";

		public const string FunctionReturnParameterName = "Function";

		public readonly Location Location;

		public readonly ImmutableArray<AnonymousTypeField> Fields;

		public readonly string Key;

		public readonly bool IsImplicitlyDeclared;

		public ImmutableArray<AnonymousTypeField> Parameters => Fields;

		internal static string GetReturnParameterName(bool isFunction)
		{
			if (!isFunction)
			{
				return "Sub";
			}
			return "Function";
		}

		public AnonymousTypeDescriptor(ImmutableArray<AnonymousTypeField> fields, Location location, bool isImplicitlyDeclared)
		{
			this = default(AnonymousTypeDescriptor);
			Fields = fields;
			Location = location;
			IsImplicitlyDeclared = isImplicitlyDeclared;
			Key = ComputeKey(fields, (AnonymousTypeField f) => f.Name, (AnonymousTypeField f) => f.IsKey);
		}

		internal static string ComputeKey<T>(ImmutableArray<T> fields, Func<T, string> getName, Func<T, bool> getIsKey)
		{
			PooledStringBuilder instance = PooledStringBuilder.GetInstance();
			StringBuilder builder = instance.Builder;
			ImmutableArray<T>.Enumerator enumerator = fields.GetEnumerator();
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				builder.Append('|');
				builder.Append(getName(current));
				builder.Append(getIsKey(current) ? '+' : '-');
			}
			CaseInsensitiveComparison.ToLower(builder);
			return instance.ToStringAndFree();
		}

		[Conditional("DEBUG")]
		internal void AssertGood()
		{
			ImmutableArray<AnonymousTypeField>.Enumerator enumerator = Fields.GetEnumerator();
			while (enumerator.MoveNext())
			{
				_ = enumerator.Current;
			}
		}

		public bool Equals(AnonymousTypeDescriptor other)
		{
			return Equals(other, TypeCompareKind.ConsiderEverything);
		}

		bool IEquatable<AnonymousTypeDescriptor>.Equals(AnonymousTypeDescriptor other)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Equals
			return this.Equals(other);
		}

		public bool Equals(AnonymousTypeDescriptor other, TypeCompareKind compareKind)
		{
			if (!Key.Equals(other.Key))
			{
				return false;
			}
			ImmutableArray<AnonymousTypeField> fields = Fields;
			int length = fields.Length;
			ImmutableArray<AnonymousTypeField> fields2 = other.Fields;
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (!fields[i].Type.Equals(fields2[i].Type, compareKind))
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj is AnonymousTypeDescriptor)
			{
				return Equals((AnonymousTypeDescriptor)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Key.GetHashCode();
		}

		public bool SubstituteTypeParametersIfNeeded(TypeSubstitution substitution, out AnonymousTypeDescriptor newDescriptor)
		{
			int length = Fields.Length;
			AnonymousTypeField[] array = new AnonymousTypeField[length - 1 + 1];
			bool flag = false;
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				AnonymousTypeField anonymousTypeField = Fields[i];
				array[i] = new AnonymousTypeField(anonymousTypeField.Name, anonymousTypeField.Type.InternalSubstituteTypeParameters(substitution).Type, anonymousTypeField.Location, anonymousTypeField.IsKey);
				if (!flag)
				{
					flag = (object)anonymousTypeField.Type != array[i].Type;
				}
			}
			if (flag)
			{
				newDescriptor = new AnonymousTypeDescriptor(array.AsImmutableOrNull(), Location, IsImplicitlyDeclared);
			}
			else
			{
				newDescriptor = default(AnonymousTypeDescriptor);
			}
			return flag;
		}
	}
}
