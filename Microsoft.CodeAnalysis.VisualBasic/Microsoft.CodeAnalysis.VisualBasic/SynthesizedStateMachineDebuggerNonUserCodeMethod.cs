using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class SynthesizedStateMachineDebuggerNonUserCodeMethod : SynthesizedStateMachineMethod
	{
		internal SynthesizedStateMachineDebuggerNonUserCodeMethod(StateMachineTypeSymbol stateMachineType, string name, MethodSymbol interfaceMethod, SyntaxNode syntax, Accessibility declaredAccessibility, bool hasMethodBodyDependency, PropertySymbol associatedProperty = null)
			: base(stateMachineType, name, interfaceMethod, syntax, declaredAccessibility, generateDebugInfo: false, hasMethodBodyDependency, associatedProperty)
		{
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			VisualBasicCompilation declaringCompilation = DeclaringCompilation;
			Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeDebuggerNonUserCodeAttribute());
		}
	}
}
