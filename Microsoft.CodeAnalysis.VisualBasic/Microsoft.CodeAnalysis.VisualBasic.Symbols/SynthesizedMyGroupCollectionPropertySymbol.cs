using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SynthesizedMyGroupCollectionPropertySymbol : SynthesizedPropertyBase
	{
		private readonly string _name;

		private readonly SynthesizedMyGroupCollectionPropertyBackingFieldSymbol _field;

		private readonly SynthesizedMyGroupCollectionPropertyGetAccessorSymbol _getMethod;

		private readonly SynthesizedMyGroupCollectionPropertySetAccessorSymbol _setMethodOpt;

		public readonly SyntaxReference AttributeSyntax;

		public readonly string DefaultInstanceAlias;

		public override string Name => _name;

		public override TypeSymbol Type => _field.Type;

		public override MethodSymbol GetMethod => _getMethod;

		public override MethodSymbol SetMethod => _setMethodOpt;

		public override Symbol ContainingSymbol => _field.ContainingSymbol;

		public override NamedTypeSymbol ContainingType => _field.ContainingType;

		internal override FieldSymbol AssociatedField => _field;

		public override bool IsImplicitlyDeclared => true;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		internal override bool IsMyGroupCollectionProperty => true;

		public SynthesizedMyGroupCollectionPropertySymbol(SourceNamedTypeSymbol container, AttributeSyntax attributeSyntax, string propertyName, string fieldName, NamedTypeSymbol type, string createMethod, string disposeMethod, string defaultInstanceAlias)
		{
			AttributeSyntax = attributeSyntax.SyntaxTree.GetReference(attributeSyntax);
			DefaultInstanceAlias = defaultInstanceAlias;
			_name = propertyName;
			_field = new SynthesizedMyGroupCollectionPropertyBackingFieldSymbol(container, this, type, fieldName);
			_getMethod = new SynthesizedMyGroupCollectionPropertyGetAccessorSymbol(container, this, createMethod);
			if (disposeMethod.Length > 0)
			{
				_setMethodOpt = new SynthesizedMyGroupCollectionPropertySetAccessorSymbol(container, this, disposeMethod);
			}
		}

		internal override LexicalSortKey GetLexicalSortKey()
		{
			return LexicalSortKey.NotInSource;
		}

		public void RelocateDiagnostics(DiagnosticBag source, DiagnosticBag destination)
		{
			if (source.IsEmptyWithoutResolution)
			{
				return;
			}
			Location location = AttributeSyntax.GetLocation();
			foreach (VBDiagnostic item in source.AsEnumerable())
			{
				destination.Add(item.WithLocation(location));
			}
		}
	}
}
