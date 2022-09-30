using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Cci;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class LambdaSymbol : MethodSymbol
	{
		internal static readonly TypeSymbol ReturnTypeIsBeingInferred = new ErrorTypeSymbol();

		internal static readonly TypeSymbol ReturnTypeIsUnknown = new ErrorTypeSymbol();

		internal static readonly TypeSymbol ReturnTypePendingDelegate = new ErrorTypeSymbol();

		internal static readonly TypeSymbol ReturnTypeVoidReplacement = new ErrorTypeSymbol();

		internal static readonly TypeSymbol ErrorRecoveryInferenceError = new ErrorTypeSymbol();

		private readonly SyntaxNode _syntaxNode;

		private readonly ImmutableArray<ParameterSymbol> _parameters;

		protected TypeSymbol m_ReturnType;

		private readonly Binder _binder;

		public abstract SynthesizedLambdaKind SynthesizedKind { get; }

		internal sealed override bool IsQueryLambdaMethod => SynthesizedLambdaKindExtensions.IsQueryLambda(SynthesizedKind);

		public override int Arity => 0;

		internal override bool HasSpecialName => false;

		public override Symbol AssociatedSymbol => null;

		internal override CallingConvention CallingConvention => CallingConvention.Default;

		public override Symbol ContainingSymbol => _binder.ContainingMember;

		internal Binder ContainingBinder => _binder;

		public override Accessibility DeclaredAccessibility => Accessibility.Private;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

		public override bool IsExtensionMethod => false;

		public override bool IsExternalMethod => false;

		internal override MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation => null;

		internal override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.IL;

		internal sealed override bool HasDeclarativeSecurity => false;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override bool IsOverloads => false;

		public override bool IsOverridable => false;

		public override bool IsOverrides => false;

		public override bool IsShared
		{
			get
			{
				Symbol containingSymbol = ContainingSymbol;
				SymbolKind kind = containingSymbol.Kind;
				if (kind == SymbolKind.Field || kind == SymbolKind.Method || kind == SymbolKind.Property)
				{
					return containingSymbol.IsShared;
				}
				return true;
			}
		}

		public override bool IsSub => TypeSymbolExtensions.IsVoidType(m_ReturnType);

		public override bool IsVararg => false;

		public sealed override bool IsInitOnly => false;

		public override ImmutableArray<Location> Locations => ImmutableArray.Create(_syntaxNode.GetLocation());

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray.Create(_syntaxNode.GetReference());

		internal override bool IsLambdaMethod => true;

		public override MethodKind MethodKind => MethodKind.AnonymousFunction;

		internal sealed override bool IsMethodKindBasedOnSyntax => true;

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		public override bool ReturnsByRef => false;

		public override TypeSymbol ReturnType => m_ReturnType;

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		internal override SyntaxNode Syntax => _syntaxNode;

		public override ImmutableArray<TypeSymbol> TypeArguments => ImmutableArray<TypeSymbol>.Empty;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

		internal override bool GenerateDebugInfoImpl => true;

		protected LambdaSymbol(SyntaxNode syntaxNode, ImmutableArray<BoundLambdaParameterSymbol> parameters, TypeSymbol returnType, Binder binder)
		{
			_syntaxNode = syntaxNode;
			_parameters = StaticCast<ParameterSymbol>.From(parameters);
			m_ReturnType = returnType;
			_binder = binder;
			ImmutableArray<BoundLambdaParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.SetLambdaSymbol(this);
			}
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return ImmutableArray<string>.Empty;
		}

		internal override bool TryGetMeParameter(out ParameterSymbol meParameter)
		{
			switch (ContainingSymbol.Kind)
			{
			case SymbolKind.Field:
				meParameter = ((FieldSymbol)ContainingSymbol).MeParameter;
				break;
			case SymbolKind.Property:
				meParameter = ((PropertySymbol)ContainingSymbol).MeParameter;
				break;
			case SymbolKind.Method:
				meParameter = ((MethodSymbol)ContainingSymbol).MeParameter;
				break;
			default:
				meParameter = null;
				break;
			}
			return true;
		}

		public override DllImportData GetDllImportData()
		{
			return null;
		}

		internal sealed override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
		{
			return false;
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			return obj is LambdaSymbol lambdaSymbol && lambdaSymbol._syntaxNode == _syntaxNode && object.Equals(lambdaSymbol.ContainingSymbol, ContainingSymbol) && MethodSignatureComparer.AllAspectsSignatureComparer.Equals(lambdaSymbol, this);
		}

		public override int GetHashCode()
		{
			int newKey = Hash.Combine(Syntax.GetHashCode(), _parameters.Length);
			newKey = Hash.Combine(newKey, ReturnType.GetHashCode());
			int num = _parameters.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				newKey = Hash.Combine(newKey, _parameters[i].Type.GetHashCode());
			}
			return newKey;
		}
	}
}
