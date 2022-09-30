using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal struct AnonymousTypeField
	{
		public readonly string Name;

		public readonly Location Location;

		private TypeSymbol _type;

		public readonly bool IsKey;

		public TypeSymbol Type => _type;

		public bool IsByRef => IsKey;

		public AnonymousTypeField(string name, TypeSymbol type, Location location, bool isKeyOrByRef = false)
		{
			this = default(AnonymousTypeField);
			Name = (string.IsNullOrWhiteSpace(name) ? "<Empty Name>" : name);
			_type = type;
			IsKey = isKeyOrByRef;
			Location = location;
		}

		public AnonymousTypeField(string name, Location location, bool isKey)
			: this(name, null, location, isKey)
		{
		}

		[Conditional("DEBUG")]
		internal void AssertGood()
		{
		}

		internal void AssignFieldType(TypeSymbol newType)
		{
			_type = newType;
		}
	}
}
