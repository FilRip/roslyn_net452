using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class SynthesizedLambdaConstructor : SynthesizedMethod, ISynthesizedMethodBodyImplementationSymbol
	{
		public override MethodKind MethodKind => MethodKind.Constructor;

		internal sealed override bool HasSpecialName => true;

		internal override bool GenerateDebugInfoImpl => false;

		public bool HasMethodBodyDependency => false;

		public IMethodSymbolInternal Method => ((ISynthesizedMethodBodyImplementationSymbol)base.ContainingSymbol).Method;

		internal SynthesizedLambdaConstructor(SyntaxNode syntaxNode, LambdaFrame containingType)
			: base(syntaxNode, containingType, ".ctor", isShared: false)
		{
		}

		internal MethodSymbol AsMember(NamedTypeSymbol frameType)
		{
			if ((object)frameType == base.ContainingType)
			{
				return this;
			}
			return (MethodSymbol)((SubstitutedNamedType)frameType).GetMemberForDefinition(this);
		}

		internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
		{
			return false;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
		}

		internal sealed override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
