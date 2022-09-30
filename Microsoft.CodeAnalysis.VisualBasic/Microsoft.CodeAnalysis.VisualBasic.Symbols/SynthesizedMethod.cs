using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SynthesizedMethod : SynthesizedMethodBase
	{
		private readonly bool _isShared;

		private readonly string _name;

		private readonly SyntaxNode _syntaxNodeOpt;

		private static readonly Func<Symbol, TypeSubstitution> s_typeSubstitutionFactory = (Symbol container) => ((SynthesizedMethod)container).TypeMap;

		internal static readonly Func<TypeParameterSymbol, Symbol, TypeParameterSymbol> CreateTypeParameter = (TypeParameterSymbol typeParameter, Symbol container) => new SynthesizedClonedTypeParameterSymbol(typeParameter, container, typeParameter.Name, s_typeSubstitutionFactory);

		public override string Name => _name;

		public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

		public override TypeSymbol ReturnType => ContainingAssembly.GetSpecialType(SpecialType.System_Void);

		public override Accessibility DeclaredAccessibility => Accessibility.Public;

		internal sealed override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.IL;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override bool IsOverloads => false;

		public override bool IsOverridable => false;

		public override bool IsOverrides => false;

		public override bool IsShared => _isShared;

		public override bool IsSub => TypeSymbolExtensions.IsVoidType(ReturnType);

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
		{
			get
			{
				SyntaxNode syntaxNode = Syntax;
				if (syntaxNode is LambdaExpressionSyntax lambdaExpressionSyntax)
				{
					syntaxNode = lambdaExpressionSyntax.SubOrFunctionHeader;
				}
				else if (syntaxNode is MethodBlockBaseSyntax methodBlockBaseSyntax)
				{
					syntaxNode = methodBlockBaseSyntax.BlockStatement;
				}
				return ImmutableArray.Create(syntaxNode.GetReference());
			}
		}

		public override MethodKind MethodKind => MethodKind.Ordinary;

		internal override SyntaxNode Syntax => _syntaxNodeOpt;

		internal virtual TypeSubstitution TypeMap
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal SynthesizedMethod(SyntaxNode syntaxNode, NamedTypeSymbol containingSymbol, string name, bool isShared)
			: base(containingSymbol)
		{
			_syntaxNodeOpt = syntaxNode;
			_isShared = isShared;
			_name = name;
		}

		internal static ParameterSymbol WithNewContainerAndType(Symbol newContainer, TypeSymbol newType, ParameterSymbol origParameter)
		{
			SourceParameterFlags sourceParameterFlags = (SourceParameterFlags)0;
			sourceParameterFlags = ((!origParameter.IsByRef) ? (sourceParameterFlags | SourceParameterFlags.ByVal) : (sourceParameterFlags | SourceParameterFlags.ByRef));
			if (origParameter.IsParamArray)
			{
				sourceParameterFlags |= SourceParameterFlags.ParamArray;
			}
			if (origParameter.IsOptional)
			{
				sourceParameterFlags |= SourceParameterFlags.Optional;
			}
			return SourceComplexParameterSymbol.Create(newContainer, origParameter.Name, origParameter.Ordinal, newType, origParameter.Locations.FirstOrDefault(), null, sourceParameterFlags, origParameter.ExplicitDefaultConstantValue);
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			if (base.ContainingSymbol is SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol)
			{
				Symbol.AddSynthesizedAttribute(ref attributes, sourceMemberContainerTypeSymbol.DeclaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
			}
		}
	}
}
