using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class ModuleSymbol : Symbol, IModuleSymbol, IModuleSymbolInternal
	{
		internal abstract int Ordinal { get; }

		internal abstract Machine Machine { get; }

		internal abstract bool Bit32Required { get; }

		public sealed override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

		public sealed override bool IsMustOverride => false;

		public sealed override bool IsNotOverridable => false;

		public sealed override bool IsOverridable => false;

		public sealed override bool IsOverrides => false;

		public sealed override bool IsShared => false;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public abstract NamespaceSymbol GlobalNamespace { get; }

		public override AssemblySymbol ContainingAssembly => (AssemblySymbol)ContainingSymbol;

		public override ModuleSymbol ContainingModule => null;

		public sealed override SymbolKind Kind => SymbolKind.NetModule;

		public ImmutableArray<AssemblyIdentity> ReferencedAssemblies => GetReferencedAssemblies();

		public ImmutableArray<AssemblySymbol> ReferencedAssemblySymbols => GetReferencedAssemblySymbols();

		internal abstract bool HasUnifiedReferences { get; }

		internal abstract ICollection<string> TypeNames { get; }

		internal abstract ICollection<string> NamespaceNames { get; }

		internal abstract bool HasAssemblyCompilationRelaxationsAttribute { get; }

		internal abstract bool HasAssemblyRuntimeCompatibilityAttribute { get; }

		internal abstract CharSet? DefaultMarshallingCharSet { get; }

		internal abstract bool IsMissing { get; }

		internal abstract bool MightContainExtensionMethods { get; }

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

		private INamespaceSymbol IModuleSymbol_GlobalNamespace => GlobalNamespace;

		private ImmutableArray<IAssemblySymbol> IModuleSymbol_ReferencedAssemblySymbols => ImmutableArray<IAssemblySymbol>.CastUp(ReferencedAssemblySymbols);

		internal ModuleSymbol()
		{
		}

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg)
		{
			return visitor.VisitModule(this, arg);
		}

		public abstract ModuleMetadata GetMetadata();

		ModuleMetadata IModuleSymbol.GetMetadata()
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetMetadata
			return this.GetMetadata();
		}

		internal abstract ImmutableArray<AssemblyIdentity> GetReferencedAssemblies();

		internal abstract ImmutableArray<AssemblySymbol> GetReferencedAssemblySymbols();

		internal AssemblySymbol GetReferencedAssemblySymbol(int referencedAssemblyIndex)
		{
			ImmutableArray<AssemblySymbol> referencedAssemblySymbols = GetReferencedAssemblySymbols();
			if (referencedAssemblyIndex < referencedAssemblySymbols.Length)
			{
				return referencedAssemblySymbols[referencedAssemblyIndex];
			}
			AssemblySymbol containingAssembly = ContainingAssembly;
			if ((object)containingAssembly != containingAssembly.CorLibrary)
			{
				throw new ArgumentOutOfRangeException("referencedAssemblyIndex");
			}
			return null;
		}

		internal abstract void SetReferences(ModuleReferences<AssemblySymbol> moduleReferences, SourceAssemblySymbol originatingSourceAssemblyDebugOnly = null);

		internal abstract DiagnosticInfo GetUnificationUseSiteErrorInfo(TypeSymbol dependentType);

		internal abstract NamedTypeSymbol LookupTopLevelMetadataType(ref MetadataTypeName emittedName);

		internal virtual ImmutableArray<byte> GetHash(AssemblyHashAlgorithm algorithmId)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public NamespaceSymbol GetModuleNamespace(INamespaceSymbol namespaceSymbol)
		{
			if (namespaceSymbol == null)
			{
				throw new ArgumentNullException("namespaceSymbol");
			}
			NamespaceSymbol namespaceSymbol2 = namespaceSymbol as NamespaceSymbol;
			if (((object)namespaceSymbol2 != null) & (namespaceSymbol2.Extent.Kind == NamespaceKind.Module) & (namespaceSymbol2.ContainingModule == this))
			{
				return namespaceSymbol2;
			}
			if (namespaceSymbol.IsGlobalNamespace | (namespaceSymbol.ContainingNamespace == null))
			{
				return GlobalNamespace;
			}
			return GetModuleNamespace(namespaceSymbol.ContainingNamespace)?.GetNestedNamespace(namespaceSymbol.Name);
		}

		private INamespaceSymbol IModuleSymbol_GetModuleNamespace(INamespaceSymbol namespaceSymbol)
		{
			return GetModuleNamespace(namespaceSymbol);
		}

		INamespaceSymbol IModuleSymbol.GetModuleNamespace(INamespaceSymbol namespaceSymbol)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IModuleSymbol_GetModuleNamespace
			return this.IModuleSymbol_GetModuleNamespace(namespaceSymbol);
		}

		public override void Accept(SymbolVisitor visitor)
		{
			visitor.VisitModule(this);
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			return visitor.VisitModule(this);
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			visitor.VisitModule(this);
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			return visitor.VisitModule(this);
		}
	}
}
