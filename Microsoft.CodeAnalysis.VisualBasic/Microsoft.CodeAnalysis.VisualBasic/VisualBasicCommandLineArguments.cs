using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	public sealed class VisualBasicCommandLineArguments : CommandLineArguments
	{
		internal OutputLevel OutputLevel;

		public new VisualBasicCompilationOptions CompilationOptions { get; set; }

		public new VisualBasicParseOptions ParseOptions { get; set; }

		protected override ParseOptions ParseOptionsCore => ParseOptions;

		protected override CompilationOptions CompilationOptionsCore => CompilationOptions;

		internal CommandLineReference? DefaultCoreLibraryReference { get; set; }

		internal VisualBasicCommandLineArguments()
		{
		}

		internal override bool ResolveMetadataReferences(MetadataReferenceResolver metadataResolver, List<DiagnosticInfo> diagnostics, CommonMessageProvider messageProvider, List<MetadataReference> resolved)
		{
			bool flag = base.ResolveMetadataReferences(metadataResolver, diagnostics, messageProvider, resolved);
			if (DefaultCoreLibraryReference.HasValue && resolved.Count > 0)
			{
				foreach (MetadataReference item in resolved)
				{
					if (item.IsUnresolved)
					{
						continue;
					}
					MetadataReferenceProperties properties = item.Properties;
					if (properties.EmbedInteropTypes || properties.Kind != 0)
					{
						continue;
					}
					try
					{
						if (!(((PortableExecutableReference)item).GetMetadataNoCopy() is AssemblyMetadata assemblyMetadata) || !assemblyMetadata.IsValidAssembly())
						{
							return flag;
						}
						PEAssembly assembly = assemblyMetadata.GetAssembly();
						if (assembly.AssemblyReferences.Length == 0 && !assembly.ContainsNoPiaLocalTypes() && assembly.DeclaresTheObjectClass)
						{
							return flag;
						}
					}
					catch (BadImageFormatException ex)
					{
						ProjectData.SetProjectError(ex);
						BadImageFormatException ex2 = ex;
						bool result = flag;
						ProjectData.ClearProjectError();
						return result;
					}
					catch (IOException ex3)
					{
						ProjectData.SetProjectError(ex3);
						IOException ex4 = ex3;
						bool result = flag;
						ProjectData.ClearProjectError();
						return result;
					}
				}
				PortableExecutableReference portableExecutableReference = CommandLineArguments.ResolveMetadataReference(DefaultCoreLibraryReference.Value, metadataResolver, diagnostics, messageProvider).FirstOrDefault();
				if (portableExecutableReference == null || portableExecutableReference.IsUnresolved)
				{
					return false;
				}
				resolved.Insert(0, portableExecutableReference);
				return flag;
			}
			return flag;
		}
	}
}
