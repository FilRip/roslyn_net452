using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class SynthesizedMetadataCompiler : VisualBasicSymbolVisitor
	{
		private readonly PEModuleBuilder _moduleBeingBuilt;

		private readonly CancellationToken _cancellationToken;

		private SynthesizedMetadataCompiler(PEModuleBuilder moduleBeingBuilt, CancellationToken cancellationToken)
		{
			_moduleBeingBuilt = moduleBeingBuilt;
			_cancellationToken = cancellationToken;
		}

		internal static void ProcessSynthesizedMembers(VisualBasicCompilation compilation, PEModuleBuilder moduleBeingBuilt, CancellationToken cancellationToken = default(CancellationToken))
		{
			SynthesizedMetadataCompiler visitor = new SynthesizedMetadataCompiler(moduleBeingBuilt, cancellationToken);
			compilation.SourceModule.GlobalNamespace.Accept(visitor);
		}

		public override void VisitNamespace(NamespaceSymbol symbol)
		{
			CancellationToken cancellationToken = _cancellationToken;
			cancellationToken.ThrowIfCancellationRequested();
			ImmutableArray<Symbol>.Enumerator enumerator = symbol.GetMembers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.Accept(this);
			}
		}

		public override void VisitNamedType(NamedTypeSymbol symbol)
		{
			CancellationToken cancellationToken = _cancellationToken;
			cancellationToken.ThrowIfCancellationRequested();
			ImmutableArray<Symbol>.Enumerator enumerator = symbol.GetMembers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				SymbolKind kind = current.Kind;
				if (kind == SymbolKind.NamedType)
				{
					current.Accept(this);
				}
			}
		}
	}
}
