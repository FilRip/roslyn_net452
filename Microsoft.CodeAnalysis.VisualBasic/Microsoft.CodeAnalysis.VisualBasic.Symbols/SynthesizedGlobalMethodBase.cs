using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SynthesizedGlobalMethodBase : MethodSymbol
	{
		protected readonly PrivateImplementationDetails m_privateImplType;

		protected readonly SourceModuleSymbol m_containingModule;

		protected readonly string m_name;

		public sealed override bool IsImplicitlyDeclared => true;

		public sealed override string Name => m_name;

		internal override bool HasSpecialName => false;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override bool IsOverloads => false;

		public override bool IsOverridable => false;

		public override bool IsOverrides => false;

		public sealed override bool IsShared => true;

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override MethodKind MethodKind => MethodKind.Ordinary;

		internal sealed override bool IsMethodKindBasedOnSyntax => false;

		public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

		public override Accessibility DeclaredAccessibility => Accessibility.Internal;

		internal sealed override SyntaxNode Syntax => VisualBasicSyntaxTree.Dummy.GetRoot();

		public sealed override int Arity => 0;

		public override Symbol AssociatedSymbol => null;

		internal sealed override CallingConvention CallingConvention
		{
			get
			{
				if (!IsShared)
				{
					return CallingConvention.HasThis;
				}
				return CallingConvention.Default;
			}
		}

		public PrivateImplementationDetails ContainingPrivateImplementationDetailsType => m_privateImplType;

		public sealed override Symbol ContainingSymbol => null;

		public override NamedTypeSymbol ContainingType => null;

		public override ModuleSymbol ContainingModule => m_containingModule;

		public override AssemblySymbol ContainingAssembly => m_containingModule.ContainingAssembly;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

		public override MethodSymbol OverriddenMethod => null;

		public sealed override bool IsExtensionMethod => false;

		public sealed override bool IsExternalMethod => false;

		internal sealed override MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation => null;

		internal sealed override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.IL;

		internal sealed override bool HasDeclarativeSecurity => false;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

		public sealed override bool IsVararg => false;

		public override ImmutableArray<TypeSymbol> TypeArguments => ImmutableArray<TypeSymbol>.Empty;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override bool IsAsync => false;

		public override bool IsIterator => false;

		public sealed override bool IsInitOnly => false;

		internal override bool GenerateDebugInfoImpl => false;

		protected SynthesizedGlobalMethodBase(SourceModuleSymbol containingModule, string name, PrivateImplementationDetails privateImplType)
		{
			m_containingModule = containingModule;
			m_name = name;
			m_privateImplType = privateImplType;
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return ImmutableArray<string>.Empty;
		}

		internal override LexicalSortKey GetLexicalSortKey()
		{
			return LexicalSortKey.NotInSource;
		}

		public sealed override DllImportData GetDllImportData()
		{
			return null;
		}

		internal sealed override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetReturnTypeAttributes()
		{
			return ImmutableArray<VisualBasicAttributeData>.Empty;
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
