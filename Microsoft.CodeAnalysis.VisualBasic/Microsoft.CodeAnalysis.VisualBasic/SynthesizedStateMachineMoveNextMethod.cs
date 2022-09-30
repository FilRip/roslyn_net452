using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class SynthesizedStateMachineMoveNextMethod : SynthesizedStateMachineMethod
	{
		private ImmutableArray<VisualBasicAttributeData> _attributes;

		internal SynthesizedStateMachineMoveNextMethod(StateMachineTypeSymbol stateMachineType, MethodSymbol interfaceMethod, SyntaxNode syntax, Accessibility declaredAccessibility)
			: base(stateMachineType, "MoveNext", interfaceMethod, syntax, declaredAccessibility, generateDebugInfo: true, hasMethodBodyDependency: true)
		{
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			VisualBasicCompilation declaringCompilation = DeclaringCompilation;
			Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			if (_attributes.IsDefault)
			{
				ArrayBuilder<VisualBasicAttributeData> arrayBuilder = null;
				MethodSymbol kickoffMethod = base.StateMachineType.KickoffMethod;
				ImmutableArray<VisualBasicAttributeData>.Enumerator enumerator = kickoffMethod.GetAttributes().GetEnumerator();
				while (enumerator.MoveNext())
				{
					VisualBasicAttributeData current = enumerator.Current;
					if (current.IsTargetAttribute(kickoffMethod, AttributeDescription.DebuggerHiddenAttribute) || current.IsTargetAttribute(kickoffMethod, AttributeDescription.DebuggerNonUserCodeAttribute) || current.IsTargetAttribute(kickoffMethod, AttributeDescription.DebuggerStepperBoundaryAttribute) || current.IsTargetAttribute(kickoffMethod, AttributeDescription.DebuggerStepThroughAttribute))
					{
						if (arrayBuilder == null)
						{
							arrayBuilder = ArrayBuilder<VisualBasicAttributeData>.GetInstance(4);
						}
						arrayBuilder.Add(current);
					}
				}
				ImmutableInterlocked.InterlockedCompareExchange(ref _attributes, arrayBuilder?.ToImmutableAndFree() ?? ImmutableArray<VisualBasicAttributeData>.Empty, default(ImmutableArray<VisualBasicAttributeData>));
			}
			return _attributes;
		}
	}
}
