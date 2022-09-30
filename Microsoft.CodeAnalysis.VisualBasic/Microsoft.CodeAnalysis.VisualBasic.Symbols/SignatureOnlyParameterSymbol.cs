using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SignatureOnlyParameterSymbol : ParameterSymbol
	{
		private readonly TypeSymbol _type;

		private readonly ImmutableArray<CustomModifier> _customModifiers;

		private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

		private readonly ConstantValue _defaultValue;

		private readonly bool _isParamArray;

		private readonly bool _isByRef;

		private readonly bool _isOut;

		private readonly bool _isOptional;

		public override TypeSymbol Type => _type;

		public override ImmutableArray<CustomModifier> CustomModifiers => _customModifiers;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

		public override bool IsParamArray => _isParamArray;

		public override string Name => "";

		public override bool IsByRef => _isByRef;

		internal override bool IsExplicitByRef => _isByRef;

		internal override bool IsMetadataOut => _isOut;

		internal override bool IsMetadataIn => !_isOut;

		public override bool IsOptional => _isOptional;

		public override bool HasExplicitDefaultValue => (object)_defaultValue != null;

		internal override ConstantValue ExplicitDefaultConstantValue => _defaultValue;

		internal override bool HasOptionCompare => false;

		internal override MarshalPseudoCustomAttributeData MarshallingInformation
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override int Ordinal
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override Symbol ContainingSymbol
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

		public override AssemblySymbol ContainingAssembly
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override bool IsIDispatchConstant
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override bool IsIUnknownConstant
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override bool IsCallerLineNumber
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override bool IsCallerMemberName
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override bool IsCallerFilePath
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public SignatureOnlyParameterSymbol(TypeSymbol type, ImmutableArray<CustomModifier> customModifiers, ImmutableArray<CustomModifier> refCustomModifiers, ConstantValue defaultConstantValue, bool isParamArray, bool isByRef, bool isOut, bool isOptional)
		{
			_type = type;
			_customModifiers = customModifiers;
			_refCustomModifiers = refCustomModifiers;
			_defaultValue = defaultConstantValue;
			_isParamArray = isParamArray;
			_isByRef = isByRef;
			_isOut = isOut;
			_isOptional = isOptional;
		}
	}
}
