using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal abstract class PEAssemblyBuilderBase : PEModuleBuilder, IAssemblyReference
	{
		protected readonly SourceAssemblySymbol m_SourceAssembly;

		private readonly ImmutableArray<NamedTypeSymbol> _additionalTypes;

		private ImmutableArray<IFileReference> _lazyFiles;

		private ImmutableArray<IFileReference> _lazyFilesWithoutManifestResources;

		private readonly string _metadataName;

		public override ISourceAssemblySymbolInternal SourceAssemblyOpt => m_SourceAssembly;

		public override string Name => _metadataName;

		public AssemblyIdentity Identity => m_SourceAssembly.Identity;

		public Version AssemblyVersionPattern => m_SourceAssembly.AssemblyVersionPattern;

		public PEAssemblyBuilderBase(SourceAssemblySymbol sourceAssembly, EmitOptions emitOptions, OutputKind outputKind, ModulePropertiesForSerialization serializationProperties, IEnumerable<ResourceDescription> manifestResources, ImmutableArray<NamedTypeSymbol> additionalTypes)
			: base((SourceModuleSymbol)sourceAssembly.Modules[0], emitOptions, outputKind, serializationProperties, manifestResources)
		{
			m_SourceAssembly = sourceAssembly;
			_additionalTypes = additionalTypes.NullToEmpty();
			_metadataName = ((emitOptions.OutputNameOverride == null) ? sourceAssembly.MetadataName : FileNameUtilities.ChangeExtension(emitOptions.OutputNameOverride, null));
			m_AssemblyOrModuleSymbolToModuleRefMap.Add(sourceAssembly, this);
		}

		public override ImmutableArray<NamedTypeSymbol> GetAdditionalTopLevelTypes()
		{
			return _additionalTypes;
		}

		public override ImmutableArray<NamedTypeSymbol> GetEmbeddedTypes(DiagnosticBag diagnostics)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public sealed override IEnumerable<IFileReference> GetFiles(EmitContext context)
		{
			if (!context.IsRefAssembly)
			{
				return GetFilesCore(context, ref _lazyFiles);
			}
			return GetFilesCore(context, ref _lazyFilesWithoutManifestResources);
		}

		private IEnumerable<IFileReference> GetFilesCore(EmitContext context, ref ImmutableArray<IFileReference> lazyFiles)
		{
			if (lazyFiles.IsDefault)
			{
				ArrayBuilder<IFileReference> instance = ArrayBuilder<IFileReference>.GetInstance();
				try
				{
					ImmutableArray<ModuleSymbol> modules = m_SourceAssembly.Modules;
					int num = modules.Length - 1;
					for (int i = 1; i <= num; i++)
					{
						instance.Add((IFileReference)Translate(modules[i], context.Diagnostics));
					}
					if (!context.IsRefAssembly)
					{
						foreach (ResourceDescription manifestResource in ManifestResources)
						{
							if (!manifestResource.IsEmbedded)
							{
								instance.Add(manifestResource);
							}
						}
					}
					if (ImmutableInterlocked.InterlockedInitialize(ref lazyFiles, instance.ToImmutable()) && lazyFiles.Length > 0 && !CryptographicHashProvider.IsSupportedAlgorithm(m_SourceAssembly.HashAlgorithm))
					{
						context.Diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_CryptoHashFailed), NoLocation.Singleton));
					}
				}
				finally
				{
					instance.Free();
				}
			}
			return lazyFiles;
		}

		private static bool Free(ArrayBuilder<IFileReference> builder)
		{
			builder.Free();
			return false;
		}

		protected override void AddEmbeddedResourcesFromAddedModules(ArrayBuilder<ManagedResource> builder, DiagnosticBag diagnostics)
		{
			ImmutableArray<ModuleSymbol> modules = m_SourceAssembly.Modules;
			int num = modules.Length - 1;
			for (int i = 1; i <= num; i++)
			{
				IFileReference fileReference = (IFileReference)Translate(modules[i], diagnostics);
				try
				{
					ImmutableArray<EmbeddedResource>.Enumerator enumerator = ((PEModuleSymbol)modules[i]).Module.GetEmbeddedResourcesOrThrow().GetEnumerator();
					while (enumerator.MoveNext())
					{
						EmbeddedResource current = enumerator.Current;
						builder.Add(new ManagedResource(current.Name, (current.Attributes & ManifestResourceAttributes.Public) != 0, null, fileReference, current.Offset));
					}
				}
				catch (BadImageFormatException ex)
				{
					ProjectData.SetProjectError(ex);
					BadImageFormatException ex2 = ex;
					DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_UnsupportedModule1, NoLocation.Singleton, modules[i]);
					ProjectData.ClearProjectError();
				}
			}
		}
	}
}
