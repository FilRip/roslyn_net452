using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
{
	internal sealed class RetargetingAssemblySymbol : NonMissingAssemblySymbol
	{
		private readonly SourceAssemblySymbol _underlyingAssembly;

		private readonly ImmutableArray<ModuleSymbol> _modules;

		private ImmutableArray<AssemblySymbol> _noPiaResolutionAssemblies;

		private ImmutableArray<AssemblySymbol> _linkedReferencedAssemblies;

		internal readonly ConcurrentDictionary<NamedTypeSymbol, NamedTypeSymbol> m_NoPiaUnificationMap;

		private readonly bool _isLinked;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		public SourceAssemblySymbol UnderlyingAssembly => _underlyingAssembly;

		public override AssemblyIdentity Identity => _underlyingAssembly.Identity;

		public override Version AssemblyVersionPattern => _underlyingAssembly.AssemblyVersionPattern;

		internal override ImmutableArray<byte> PublicKey => _underlyingAssembly.PublicKey;

		internal RetargetingModuleSymbol.RetargetingSymbolTranslator RetargetingTranslator => ((RetargetingModuleSymbol)Modules[0]).RetargetingTranslator;

		public override ImmutableArray<Location> Locations => _underlyingAssembly.Locations;

		public override ImmutableArray<ModuleSymbol> Modules => _modules;

		internal override bool KeepLookingForDeclaredSpecialTypes => false;

		internal override bool IsLinked => _isLinked;

		public override ICollection<string> TypeNames => _underlyingAssembly.TypeNames;

		public override ICollection<string> NamespaceNames => _underlyingAssembly.NamespaceNames;

		public override bool MightContainExtensionMethods => _underlyingAssembly.MightContainExtensionMethods;

		internal override VisualBasicCompilation DeclaringCompilation => null;

		public RetargetingAssemblySymbol(SourceAssemblySymbol underlyingAssembly, bool isLinked)
		{
			m_NoPiaUnificationMap = new ConcurrentDictionary<NamedTypeSymbol, NamedTypeSymbol>();
			_underlyingAssembly = underlyingAssembly;
			ModuleSymbol[] array = new ModuleSymbol[underlyingAssembly.Modules.Length - 1 + 1];
			array[0] = new RetargetingModuleSymbol(this, (SourceModuleSymbol)underlyingAssembly.Modules[0]);
			int num = underlyingAssembly.Modules.Length - 1;
			for (int i = 1; i <= num; i++)
			{
				PEModuleSymbol pEModuleSymbol = (PEModuleSymbol)underlyingAssembly.Modules[i];
				array[i] = new PEModuleSymbol(this, pEModuleSymbol.Module, pEModuleSymbol.ImportOptions, i);
			}
			_modules = array.AsImmutableOrNull();
			_isLinked = isLinked;
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _underlyingAssembly.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return RetargetingTranslator.GetRetargetedAttributes(_underlyingAssembly, ref _lazyCustomAttributes);
		}

		internal override NamedTypeSymbol GetDeclaredSpecialType(SpecialType type)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override ImmutableArray<AssemblySymbol> GetNoPiaResolutionAssemblies()
		{
			return _noPiaResolutionAssemblies;
		}

		internal override void SetNoPiaResolutionAssemblies(ImmutableArray<AssemblySymbol> assemblies)
		{
			_noPiaResolutionAssemblies = assemblies;
		}

		internal override void SetLinkedReferencedAssemblies(ImmutableArray<AssemblySymbol> assemblies)
		{
			_linkedReferencedAssemblies = assemblies;
		}

		internal override ImmutableArray<AssemblySymbol> GetLinkedReferencedAssemblies()
		{
			return _linkedReferencedAssemblies;
		}

		internal override IEnumerable<ImmutableArray<byte>> GetInternalsVisibleToPublicKeys(string simpleName)
		{
			return _underlyingAssembly.GetInternalsVisibleToPublicKeys(simpleName);
		}

		internal override bool AreInternalsVisibleToThisAssembly(AssemblySymbol potentialGiverOfAccess)
		{
			return _underlyingAssembly.AreInternalsVisibleToThisAssembly(potentialGiverOfAccess);
		}

		internal override bool GetGuidString(ref string guidString)
		{
			return _underlyingAssembly.GetGuidString(ref guidString);
		}

		internal override NamedTypeSymbol TryLookupForwardedMetadataTypeWithCycleDetection(ref MetadataTypeName emittedName, ConsList<AssemblySymbol> visitedAssemblies, bool ignoreCase)
		{
			NamedTypeSymbol namedTypeSymbol = UnderlyingAssembly.TryLookupForwardedMetadataType(ref emittedName, ignoreCase);
			if ((object)namedTypeSymbol == null)
			{
				return null;
			}
			return RetargetingTranslator.Retarget(namedTypeSymbol, RetargetOptions.RetargetPrimitiveTypesByName);
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_45_GetAllTopLevelForwardedTypes))]
		internal override IEnumerable<NamedTypeSymbol> GetAllTopLevelForwardedTypes()
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_45_GetAllTopLevelForwardedTypes(-2)
			{
				_0024VB_0024Me = this
			};
		}

		public override AssemblyMetadata GetMetadata()
		{
			return _underlyingAssembly.GetMetadata();
		}
	}
}
