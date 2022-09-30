using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection.Metadata;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	[StandardModule]
	internal sealed class EmitHelpers
	{
		internal static EmitDifferenceResult EmitDifference(VisualBasicCompilation compilation, EmitBaseline baseline, IEnumerable<SemanticEdit> edits, Func<ISymbol, bool> isAddedSymbol, Stream metadataStream, Stream ilStream, Stream pdbStream, ICollection<MethodDefinitionHandle> updatedMethods, CompilationTestData testData, CancellationToken cancellationToken)
		{
			FileNameUtilities.ChangeExtension(compilation.SourceModule.Name, "pdb");
			DiagnosticBag instance = DiagnosticBag.GetInstance();
			EmitOptions emitOptions = EmitOptions.Default.WithDebugInformationFormat((!baseline.HasPortablePdb) ? DebugInformationFormat.Pdb : DebugInformationFormat.PortablePdb);
			string runtimeMetadataVersion = compilation.GetRuntimeMetadataVersion();
			ModulePropertiesForSerialization serializationProperties = compilation.ConstructModuleSerializationProperties(emitOptions, runtimeMetadataVersion, baseline.ModuleVersionId);
			IEnumerable<ResourceDescription> manifestResources = SpecializedCollections.EmptyEnumerable<ResourceDescription>();
			PEDeltaAssemblyBuilder pEDeltaAssemblyBuilder;
			try
			{
				pEDeltaAssemblyBuilder = new PEDeltaAssemblyBuilder(compilation.SourceAssembly, emitOptions, compilation.Options.OutputKind, serializationProperties, manifestResources, baseline, edits, isAddedSymbol);
			}
			catch (NotSupportedException ex)
			{
				ProjectData.SetProjectError(ex);
				NotSupportedException ex2 = ex;
				DiagnosticBagExtensions.Add(instance, ERRID.ERR_ModuleEmitFailure, NoLocation.Singleton, compilation.AssemblyName, ex2.Message);
				EmitDifferenceResult result = new EmitDifferenceResult(success: false, instance.ToReadOnlyAndFree(), null);
				ProjectData.ClearProjectError();
				return result;
			}
			if (testData != null)
			{
				pEDeltaAssemblyBuilder.SetMethodTestData(testData.Methods);
				testData.Module = pEDeltaAssemblyBuilder;
			}
			VisualBasicDefinitionMap previousDefinitions = pEDeltaAssemblyBuilder.PreviousDefinitions;
			SymbolChanges changes = pEDeltaAssemblyBuilder.Changes;
			EmitBaseline emitBaseline = null;
			if (compilation.Compile(pEDeltaAssemblyBuilder, emittingPdb: true, instance, (ISymbolInternal s) => changes.RequiresCompilation(s.GetISymbol()), cancellationToken))
			{
				EmitBaseline baseline2 = MapToCompilation(compilation, pEDeltaAssemblyBuilder);
				emitBaseline = compilation.SerializeToDeltaStreams(pEDeltaAssemblyBuilder, baseline2, previousDefinitions, changes, metadataStream, ilStream, pdbStream, updatedMethods, instance, testData?.SymWriterFactory, emitOptions.PdbFilePath, cancellationToken);
			}
			return new EmitDifferenceResult(emitBaseline != null, instance.ToReadOnlyAndFree(), emitBaseline);
		}

		internal static EmitBaseline MapToCompilation(VisualBasicCompilation compilation, PEDeltaAssemblyBuilder moduleBeingBuilt)
		{
			EmitBaseline previousGeneration = moduleBeingBuilt.PreviousGeneration;
			if (previousGeneration.Ordinal == 0)
			{
				return previousGeneration;
			}
			ImmutableDictionary<ISymbolInternal, ImmutableArray<ISymbolInternal>> allSynthesizedMembers = moduleBeingBuilt.GetAllSynthesizedMembers();
			IReadOnlyDictionary<AnonymousTypeKey, AnonymousTypeValue> anonymousTypeMap = moduleBeingBuilt.GetAnonymousTypeMap();
			SourceAssemblySymbol sourceAssembly = ((VisualBasicCompilation)previousGeneration.Compilation).SourceAssembly;
			EmitContext sourceContext = new EmitContext((PEModuleBuilder)previousGeneration.PEModuleBuilder, null, new DiagnosticBag(), metadataOnly: false, includePrivateMembers: true);
			EmitContext otherContext = new EmitContext(moduleBeingBuilt, null, new DiagnosticBag(), metadataOnly: false, includePrivateMembers: true);
			ImmutableDictionary<ISymbolInternal, ImmutableArray<ISymbolInternal>> immutableDictionary = new VisualBasicSymbolMatcher(anonymousTypeMap, sourceAssembly, sourceContext, compilation.SourceAssembly, otherContext, allSynthesizedMembers).MapSynthesizedMembers(previousGeneration.SynthesizedMembers, allSynthesizedMembers);
			return new VisualBasicSymbolMatcher(anonymousTypeMap, sourceAssembly, sourceContext, compilation.SourceAssembly, otherContext, immutableDictionary).MapBaselineToCompilation(previousGeneration, compilation, moduleBeingBuilt, immutableDictionary);
		}
	}
}
