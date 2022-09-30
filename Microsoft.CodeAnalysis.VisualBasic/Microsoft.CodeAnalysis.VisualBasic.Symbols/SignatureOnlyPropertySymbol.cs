using System.Collections.Immutable;
using Microsoft.Cci;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SignatureOnlyPropertySymbol : PropertySymbol
	{
		private readonly string _name;

		private readonly NamedTypeSymbol _containingType;

		private readonly bool _isReadOnly;

		private readonly bool _isWriteOnly;

		private readonly ImmutableArray<ParameterSymbol> _parameters;

		private readonly bool _returnsByRef;

		private readonly TypeSymbol _type;

		private readonly ImmutableArray<CustomModifier> _typeCustomModifiers;

		private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

		private readonly bool _isOverrides;

		private readonly bool _isWithEvents;

		public override string Name => _name;

		public override Symbol ContainingSymbol => _containingType;

		public override NamedTypeSymbol ContainingType => _containingType;

		public override bool IsReadOnly => _isReadOnly;

		public override bool IsWriteOnly => _isWriteOnly;

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		public override bool ReturnsByRef => _returnsByRef;

		public override TypeSymbol Type => _type;

		public override ImmutableArray<CustomModifier> TypeCustomModifiers => _typeCustomModifiers;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

		internal override ObsoleteAttributeData ObsoleteAttributeData => null;

		internal override bool HasSpecialName
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override CallingConvention CallingConvention
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override Accessibility DeclaredAccessibility
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override MethodSymbol GetMethod
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override MethodSymbol SetMethod
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override FieldSymbol AssociatedField
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override bool IsDefault
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override bool IsMustOverride
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override bool IsNotOverridable
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override bool IsOverloads
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override bool IsOverridable
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override bool IsOverrides => _isOverrides;

		public override bool IsWithEvents => _isWithEvents;

		public override bool IsShared
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override ImmutableArray<Location> Locations
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override bool IsMyGroupCollectionProperty => false;

		internal override OverriddenMembersResult<PropertySymbol> OverriddenMembers
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public SignatureOnlyPropertySymbol(string name, NamedTypeSymbol containingType, bool isReadOnly, bool isWriteOnly, ImmutableArray<ParameterSymbol> parameters, bool returnsByRef, TypeSymbol type, ImmutableArray<CustomModifier> typeCustomModifiers, ImmutableArray<CustomModifier> refCustomModifiers, bool isOverrides = false, bool isWithEvents = false)
		{
			_name = name;
			_containingType = containingType;
			_isReadOnly = isReadOnly;
			_isWriteOnly = isWriteOnly;
			_parameters = parameters;
			_returnsByRef = returnsByRef;
			_type = type;
			_typeCustomModifiers = typeCustomModifiers;
			_refCustomModifiers = refCustomModifiers;
			_isOverrides = isOverrides;
			_isWithEvents = isWithEvents;
		}
	}
}
