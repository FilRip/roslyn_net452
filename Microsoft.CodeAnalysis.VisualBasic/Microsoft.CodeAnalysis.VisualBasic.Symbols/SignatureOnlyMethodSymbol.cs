using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.Cci;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SignatureOnlyMethodSymbol : MethodSymbol
	{
		private readonly string _name;

		private readonly TypeSymbol _containingType;

		private readonly MethodKind _methodKind;

		private readonly CallingConvention _callingConvention;

		private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

		private readonly ImmutableArray<ParameterSymbol> _parameters;

		private readonly bool _returnsByRef;

		private readonly TypeSymbol _returnType;

		private readonly ImmutableArray<CustomModifier> _returnTypeCustomModifiers;

		private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

		private readonly ImmutableArray<MethodSymbol> _explicitInterfaceImplementations;

		private readonly bool _isOverrides;

		internal override CallingConvention CallingConvention => _callingConvention;

		public override bool IsVararg => new SignatureHeader((byte)_callingConvention).CallingConvention == SignatureCallingConvention.VarArgs;

		public override bool IsGenericMethod => Arity > 0;

		public override int Arity => _typeParameters.Length;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

		public override bool ReturnsByRef => _returnsByRef;

		public override TypeSymbol ReturnType => _returnType;

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => _returnTypeCustomModifiers;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => _explicitInterfaceImplementations;

		public override Symbol ContainingSymbol => _containingType;

		public override MethodKind MethodKind => _methodKind;

		internal override bool IsMethodKindBasedOnSyntax => false;

		public override string Name => _name;

		public override bool IsSub => ReturnType.SpecialType == SpecialType.System_Void;

		public override bool IsAsync => false;

		public override bool IsIterator => false;

		public override bool IsInitOnly => false;

		internal override ObsoleteAttributeData ObsoleteAttributeData => null;

		internal override bool HasSpecialName
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override ImmutableArray<TypeSymbol> TypeArguments
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override Symbol AssociatedSymbol
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override bool IsExtensionMethod
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

		public override Accessibility DeclaredAccessibility
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

		public override bool IsShared
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

		public override bool IsOverrides => _isOverrides;

		internal override SyntaxNode Syntax
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

		public override bool IsExternalMethod
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override MethodImplAttributes ImplementationAttributes
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override bool HasDeclarativeSecurity => false;

		public override bool IsMustOverride
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

		internal override OverriddenMembersResult<MethodSymbol> OverriddenMembers
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override MethodSymbol OverriddenMethod
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override bool GenerateDebugInfoImpl
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public SignatureOnlyMethodSymbol(string name, TypeSymbol m_containingType, MethodKind methodKind, CallingConvention callingConvention, ImmutableArray<TypeParameterSymbol> typeParameters, ImmutableArray<ParameterSymbol> parameters, bool returnsByRef, TypeSymbol returnType, ImmutableArray<CustomModifier> returnTypeCustomModifiers, ImmutableArray<CustomModifier> refCustomModifiers, ImmutableArray<MethodSymbol> explicitInterfaceImplementations, bool isOverrides = false)
		{
			_callingConvention = callingConvention;
			_typeParameters = typeParameters;
			_returnsByRef = returnsByRef;
			_returnType = returnType;
			_returnTypeCustomModifiers = returnTypeCustomModifiers;
			_refCustomModifiers = refCustomModifiers;
			_parameters = parameters;
			_explicitInterfaceImplementations = explicitInterfaceImplementations.NullToEmpty();
			_containingType = m_containingType;
			_methodKind = methodKind;
			_name = name;
			_isOverrides = isOverrides;
		}

		public override DllImportData GetDllImportData()
		{
			return null;
		}

		internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
