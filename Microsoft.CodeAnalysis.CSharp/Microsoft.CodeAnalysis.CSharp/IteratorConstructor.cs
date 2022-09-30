using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class IteratorConstructor : SynthesizedInstanceConstructor, ISynthesizedMethodBodyImplementationSymbol, ISymbolInternal
    {
        private readonly ImmutableArray<ParameterSymbol> _parameters;

        public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

        public override Accessibility DeclaredAccessibility => Accessibility.Public;

        IMethodSymbolInternal ISynthesizedMethodBodyImplementationSymbol.Method => ((ISynthesizedMethodBodyImplementationSymbol)ContainingSymbol).Method;

        bool ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency => false;

        internal IteratorConstructor(StateMachineTypeSymbol container)
            : base(container)
        {
            NamedTypeSymbol specialType = container.DeclaringCompilation.GetSpecialType(SpecialType.System_Int32);
            _parameters = ImmutableArray.Create(SynthesizedParameterSymbol.Create(this, TypeWithAnnotations.Create(specialType), 0, RefKind.None, GeneratedNames.MakeStateMachineStateFieldName()));
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerHiddenAttribute__ctor));
        }
    }
}
