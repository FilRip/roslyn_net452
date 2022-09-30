using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedConstructorSymbol : SynthesizedConstructorBase
	{
		private readonly bool _debuggable;

		public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

		internal override bool GenerateDebugInfoImpl => _debuggable;

		internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, ref Binder methodBodyBinder = null)
		{
			methodBodyBinder = null;
			BoundReturnStatement boundReturnStatement = new BoundReturnStatement(base.Syntax, null, null, null);
			boundReturnStatement.SetWasCompilerGenerated();
			return new BoundBlock(base.Syntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundStatement)boundReturnStatement));
		}

		internal SynthesizedConstructorSymbol(SyntaxReference syntaxReference, NamedTypeSymbol container, bool isShared, bool isDebuggable, Binder binder, BindingDiagnosticBag diagnostics)
			: base(syntaxReference, container, isShared, binder, diagnostics)
		{
			_debuggable = isDebuggable;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			return ((SourceMemberContainerTypeSymbol)base.ContainingType).CalculateSyntaxOffsetInSynthesizedConstructor(localPosition, localTree, base.IsShared);
		}
	}
}
