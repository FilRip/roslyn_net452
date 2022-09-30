using System;
using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedIntrinsicOperatorSymbol : SynthesizedMethodBase
	{
		private sealed class SynthesizedOperatorParameterSymbol : SynthesizedParameterSimpleSymbol
		{
			public SynthesizedOperatorParameterSymbol(MethodSymbol container, TypeSymbol type, int ordinal, string name)
				: base(container, type, ordinal, name)
			{
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
				{
					return true;
				}
				if (!(obj is SynthesizedOperatorParameterSymbol synthesizedOperatorParameterSymbol))
				{
					return false;
				}
				return base.Ordinal == synthesizedOperatorParameterSymbol.Ordinal && base.ContainingSymbol == synthesizedOperatorParameterSymbol.ContainingSymbol;
			}

			public override int GetHashCode()
			{
				return Hash.Combine(base.ContainingSymbol, base.Ordinal.GetHashCode());
			}
		}

		private readonly string _name;

		private readonly ImmutableArray<ParameterSymbol> _parameters;

		private readonly TypeSymbol _returnType;

		private readonly bool _isCheckedBuiltin;

		public override string Name => _name;

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		public override TypeSymbol ReturnType => _returnType;

		public override Accessibility DeclaredAccessibility => Accessibility.Public;

		internal override bool HasSpecialName => true;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override bool IsOverloads => false;

		public override bool IsOverridable => false;

		public override bool IsOverrides => false;

		public override bool IsShared => true;

		public override bool IsSub => false;

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override MethodKind MethodKind => MethodKind.BuiltinOperator;

		public override bool IsCheckedBuiltin => _isCheckedBuiltin;

		internal override VisualBasicCompilation DeclaringCompilation => null;

		internal override bool GenerateDebugInfoImpl => false;

		public SynthesizedIntrinsicOperatorSymbol(NamedTypeSymbol container, string name, TypeSymbol rightType, TypeSymbol returnType, bool isCheckedBuiltin)
			: base(container)
		{
			_name = name;
			_returnType = returnType;
			_parameters = new ParameterSymbol[2]
			{
				new SynthesizedOperatorParameterSymbol(this, container, 0, "left"),
				new SynthesizedOperatorParameterSymbol(this, rightType, 1, "right")
			}.AsImmutableOrNull();
			_isCheckedBuiltin = isCheckedBuiltin;
		}

		public SynthesizedIntrinsicOperatorSymbol(NamedTypeSymbol container, string name, TypeSymbol returnType, bool isCheckedBuiltin)
			: base(container)
		{
			_name = name;
			_returnType = returnType;
			_parameters = new ParameterSymbol[1]
			{
				new SynthesizedOperatorParameterSymbol(this, container, 0, "value")
			}.AsImmutableOrNull();
			_isCheckedBuiltin = isCheckedBuiltin;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (!(obj is SynthesizedIntrinsicOperatorSymbol synthesizedIntrinsicOperatorSymbol))
			{
				return false;
			}
			if (_isCheckedBuiltin == synthesizedIntrinsicOperatorSymbol._isCheckedBuiltin && _parameters.Length == synthesizedIntrinsicOperatorSymbol._parameters.Length && string.Equals(_name, synthesizedIntrinsicOperatorSymbol._name, StringComparison.Ordinal) && TypeSymbol.Equals(m_containingType, synthesizedIntrinsicOperatorSymbol.m_containingType, TypeCompareKind.ConsiderEverything) && TypeSymbol.Equals(_returnType, synthesizedIntrinsicOperatorSymbol._returnType, TypeCompareKind.ConsiderEverything))
			{
				int num = _parameters.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					if (!TypeSymbol.Equals(_parameters[i].Type, synthesizedIntrinsicOperatorSymbol._parameters[i].Type, TypeCompareKind.ConsiderEverything))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Hash.Combine(_name, Hash.Combine(m_containingType, _parameters.Length));
		}

		public override string GetDocumentationCommentId()
		{
			return null;
		}

		internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
		{
			return false;
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
