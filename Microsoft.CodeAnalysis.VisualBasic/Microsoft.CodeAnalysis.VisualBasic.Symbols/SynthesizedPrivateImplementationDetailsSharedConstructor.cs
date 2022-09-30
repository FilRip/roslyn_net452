using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedPrivateImplementationDetailsSharedConstructor : SynthesizedGlobalMethodBase
	{
		private readonly SourceModuleSymbol _containingModule;

		private readonly PrivateImplementationDetails _privateImplementationType;

		private readonly TypeSymbol _voidType;

		public override bool IsSub => true;

		public override bool ReturnsByRef => false;

		public override TypeSymbol ReturnType => _voidType;

		internal override bool HasSpecialName => true;

		public override MethodKind MethodKind => MethodKind.StaticConstructor;

		public override ModuleSymbol ContainingModule => _containingModule;

		internal SynthesizedPrivateImplementationDetailsSharedConstructor(SourceModuleSymbol containingModule, PrivateImplementationDetails privateImplementationType, NamedTypeSymbol voidType)
			: base(containingModule, ".cctor", privateImplementationType)
		{
			_containingModule = containingModule;
			_privateImplementationType = privateImplementationType;
			_voidType = voidType;
		}

		internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, ref Binder methodBodyBinder = null)
		{
			methodBodyBinder = null;
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(this, this, VisualBasicSyntaxTree.Dummy.GetRoot(), compilationState, diagnostics);
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			foreach (KeyValuePair<int, InstrumentationPayloadRootField> instrumentationPayloadRoot in _privateImplementationType.GetInstrumentationPayloadRoots())
			{
				int key = instrumentationPayloadRoot.Key;
				ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)instrumentationPayloadRoot.Value.Type.GetInternalSymbol();
				instance.Add(syntheticBoundNodeFactory.Assignment(syntheticBoundNodeFactory.InstrumentationPayloadRoot(key, arrayTypeSymbol, isLValue: true), syntheticBoundNodeFactory.Array(arrayTypeSymbol.ElementType, ImmutableArray.Create(syntheticBoundNodeFactory.MaximumMethodDefIndex()), ImmutableArray<BoundExpression>.Empty)));
			}
			MethodSymbol methodSymbol = syntheticBoundNodeFactory.WellKnownMember<MethodSymbol>(WellKnownMember.System_Guid__ctor);
			if ((object)methodSymbol != null)
			{
				instance.Add(syntheticBoundNodeFactory.Assignment(syntheticBoundNodeFactory.ModuleVersionId(isLValue: true), syntheticBoundNodeFactory.New(methodSymbol, syntheticBoundNodeFactory.ModuleVersionIdString())));
			}
			instance.Add(syntheticBoundNodeFactory.Return());
			return syntheticBoundNodeFactory.Block(instance.ToImmutableAndFree());
		}
	}
}
