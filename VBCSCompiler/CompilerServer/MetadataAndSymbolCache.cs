using System;
using System.Collections.Immutable;
using System.IO;
using System.Reflection.PortableExecutable;

using Microsoft.CodeAnalysis.InternalUtilities;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal class MetadataAndSymbolCache
    {
        private readonly ConcurrentLruCache<FileKey, Metadata> _metadataCache = new(500);

        private ModuleMetadata CreateModuleMetadata(string path, bool prefetchEntireImage)
        {
            Stream peStream = FileUtilities.OpenRead(path);
            PEStreamOptions peStreamOptions = PEStreamOptions.PrefetchMetadata;
            if (prefetchEntireImage)
                peStreamOptions |= PEStreamOptions.PrefetchEntireImage;
            int options = (int)peStreamOptions;
            return ModuleMetadata.CreateFromStream(peStream, (PEStreamOptions)options);
        }

        private ImmutableArray<ModuleMetadata> GetAllModules(
            ModuleMetadata manifestModule,
            string assemblyDir)
        {
#nullable restore
            ArrayBuilder<ModuleMetadata> arrayBuilder = null;
            foreach (string moduleName in manifestModule.GetModuleNames())
            {
                if (arrayBuilder == null)
                {
                    arrayBuilder = ArrayBuilder<ModuleMetadata>.GetInstance();
                    arrayBuilder.Add(manifestModule);
                }
                ModuleMetadata moduleMetadata = this.CreateModuleMetadata(PathUtilities.CombineAbsoluteAndRelativePaths(assemblyDir, moduleName), false);
                arrayBuilder.Add(moduleMetadata);
            }
            return arrayBuilder == null ? ImmutableArray.Create<ModuleMetadata>(manifestModule) : arrayBuilder.ToImmutableAndFree();
        }

        internal Metadata GetMetadata(string fullPath, MetadataReferenceProperties properties)
        {
            FileKey? uniqueFileKey = this.GetUniqueFileKey(fullPath);
            if (uniqueFileKey.HasValue && this._metadataCache.TryGetValue(uniqueFileKey.Value, out Metadata metadata1) && metadata1 != null)
                return metadata1;
            if (properties.Kind == MetadataImageKind.Module)
                return this.CreateModuleMetadata(fullPath, true);
            Metadata metadata2 = AssemblyMetadata.Create(this.GetAllModules(this.CreateModuleMetadata(fullPath, false), Path.GetDirectoryName(fullPath)));
            return this._metadataCache.GetOrAdd(uniqueFileKey.Value, metadata2);
        }

        private FileKey? GetUniqueFileKey(string filePath)
        {
            try
            {
                return new FileKey?(FileKey.Create(filePath));
            }
            catch (Exception)
            {
                return new FileKey?();
            }
        }
    }
}
