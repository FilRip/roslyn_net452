Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.DiaSymReader
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.IO
Imports System.Reflection.Metadata
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend Module EmitHelpers
		Friend Function EmitDifference(ByVal compilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation, ByVal baseline As Microsoft.CodeAnalysis.Emit.EmitBaseline, ByVal edits As IEnumerable(Of SemanticEdit), ByVal isAddedSymbol As Func(Of ISymbol, Boolean), ByVal metadataStream As System.IO.Stream, ByVal ilStream As System.IO.Stream, ByVal pdbStream As System.IO.Stream, ByVal updatedMethods As ICollection(Of MethodDefinitionHandle), ByVal testData As CompilationTestData, ByVal cancellationToken As System.Threading.CancellationToken) As Microsoft.CodeAnalysis.Emit.EmitDifferenceResult
			Dim emitDifferenceResult As Microsoft.CodeAnalysis.Emit.EmitDifferenceResult
			Dim pEDeltaAssemblyBuilder As Microsoft.CodeAnalysis.VisualBasic.Emit.PEDeltaAssemblyBuilder
			Dim symWriterFactory As Func(Of ISymWriterMetadataProvider, SymUnmanagedWriter)
			FileNameUtilities.ChangeExtension(compilation.SourceModule.Name, "pdb")
			Dim instance As Microsoft.CodeAnalysis.DiagnosticBag = Microsoft.CodeAnalysis.DiagnosticBag.GetInstance()
			Dim emitOption As EmitOptions = EmitOptions.[Default].WithDebugInformationFormat(If(baseline.HasPortablePdb, DebugInformationFormat.PortablePdb, DebugInformationFormat.Pdb))
			Dim runtimeMetadataVersion As String = compilation.GetRuntimeMetadataVersion()
			Dim modulePropertiesForSerialization As Microsoft.Cci.ModulePropertiesForSerialization = compilation.ConstructModuleSerializationProperties(emitOption, runtimeMetadataVersion, baseline.ModuleVersionId)
			Dim resourceDescriptions As IEnumerable(Of ResourceDescription) = SpecializedCollections.EmptyEnumerable(Of ResourceDescription)()
			Try
				pEDeltaAssemblyBuilder = New Microsoft.CodeAnalysis.VisualBasic.Emit.PEDeltaAssemblyBuilder(compilation.SourceAssembly, emitOption, compilation.Options.OutputKind, modulePropertiesForSerialization, resourceDescriptions, baseline, edits, isAddedSymbol)
			Catch notSupportedException1 As System.NotSupportedException
				ProjectData.SetProjectError(notSupportedException1)
				Dim notSupportedException As System.NotSupportedException = notSupportedException1
				instance.Add(ERRID.ERR_ModuleEmitFailure, NoLocation.Singleton, New [Object]() { compilation.AssemblyName, notSupportedException.Message })
				emitDifferenceResult = New Microsoft.CodeAnalysis.Emit.EmitDifferenceResult(False, instance.ToReadOnlyAndFree(), Nothing)
				ProjectData.ClearProjectError()
				Return emitDifferenceResult
			End Try
			If (testData IsNot Nothing) Then
				pEDeltaAssemblyBuilder.SetMethodTestData(testData.Methods)
				testData.[Module] = pEDeltaAssemblyBuilder
			End If
			Dim previousDefinitions As Microsoft.CodeAnalysis.VisualBasic.Emit.VisualBasicDefinitionMap = pEDeltaAssemblyBuilder.PreviousDefinitions
			Dim changes As SymbolChanges = pEDeltaAssemblyBuilder.Changes
			Dim deltaStreams As Microsoft.CodeAnalysis.Emit.EmitBaseline = Nothing
			If (compilation.Compile(pEDeltaAssemblyBuilder, True, instance, Function(s As ISymbolInternal) changes.RequiresCompilation(s.GetISymbol()), cancellationToken)) Then
				Dim emitBaseline As Microsoft.CodeAnalysis.Emit.EmitBaseline = EmitHelpers.MapToCompilation(compilation, pEDeltaAssemblyBuilder)
				Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation = compilation
				Dim pEDeltaAssemblyBuilder1 As Microsoft.CodeAnalysis.VisualBasic.Emit.PEDeltaAssemblyBuilder = pEDeltaAssemblyBuilder
				Dim emitBaseline1 As Microsoft.CodeAnalysis.Emit.EmitBaseline = emitBaseline
				Dim visualBasicDefinitionMap As Microsoft.CodeAnalysis.VisualBasic.Emit.VisualBasicDefinitionMap = previousDefinitions
				Dim symbolChange As SymbolChanges = changes
				Dim stream As System.IO.Stream = metadataStream
				Dim stream1 As System.IO.Stream = ilStream
				Dim stream2 As System.IO.Stream = pdbStream
				Dim methodDefinitionHandles As ICollection(Of MethodDefinitionHandle) = updatedMethods
				Dim diagnosticBag As Microsoft.CodeAnalysis.DiagnosticBag = instance
				If (testData IsNot Nothing) Then
					symWriterFactory = testData.SymWriterFactory
				Else
					symWriterFactory = Nothing
				End If
				deltaStreams = visualBasicCompilation.SerializeToDeltaStreams(pEDeltaAssemblyBuilder1, emitBaseline1, visualBasicDefinitionMap, symbolChange, stream, stream1, stream2, methodDefinitionHandles, diagnosticBag, symWriterFactory, emitOption.PdbFilePath, cancellationToken)
			End If
			emitDifferenceResult = New Microsoft.CodeAnalysis.Emit.EmitDifferenceResult(deltaStreams IsNot Nothing, instance.ToReadOnlyAndFree(), deltaStreams)
			Return emitDifferenceResult
		End Function

		Friend Function MapToCompilation(ByVal compilation As VisualBasicCompilation, ByVal moduleBeingBuilt As PEDeltaAssemblyBuilder) As Microsoft.CodeAnalysis.Emit.EmitBaseline
			Dim emitBaseline As Microsoft.CodeAnalysis.Emit.EmitBaseline
			Dim previousGeneration As Microsoft.CodeAnalysis.Emit.EmitBaseline = moduleBeingBuilt.PreviousGeneration
			If (previousGeneration.Ordinal <> 0) Then
				Dim allSynthesizedMembers As ImmutableDictionary(Of ISymbolInternal, ImmutableArray(Of ISymbolInternal)) = moduleBeingBuilt.GetAllSynthesizedMembers()
				Dim anonymousTypeMap As IReadOnlyDictionary(Of AnonymousTypeKey, AnonymousTypeValue) = moduleBeingBuilt.GetAnonymousTypeMap()
				Dim sourceAssembly As SourceAssemblySymbol = DirectCast(previousGeneration.Compilation, VisualBasicCompilation).SourceAssembly
				Dim emitContext As Microsoft.CodeAnalysis.Emit.EmitContext = New Microsoft.CodeAnalysis.Emit.EmitContext(DirectCast(previousGeneration.PEModuleBuilder, PEModuleBuilder), Nothing, New DiagnosticBag(), False, True)
				Dim emitContext1 As Microsoft.CodeAnalysis.Emit.EmitContext = New Microsoft.CodeAnalysis.Emit.EmitContext(moduleBeingBuilt, Nothing, New DiagnosticBag(), False, True)
				Dim symbolInternals As ImmutableDictionary(Of ISymbolInternal, ImmutableArray(Of ISymbolInternal)) = (New VisualBasicSymbolMatcher(anonymousTypeMap, sourceAssembly, emitContext, compilation.SourceAssembly, emitContext1, allSynthesizedMembers)).MapSynthesizedMembers(previousGeneration.SynthesizedMembers, allSynthesizedMembers)
				emitBaseline = (New VisualBasicSymbolMatcher(anonymousTypeMap, sourceAssembly, emitContext, compilation.SourceAssembly, emitContext1, symbolInternals)).MapBaselineToCompilation(previousGeneration, compilation, moduleBeingBuilt, symbolInternals)
			Else
				emitBaseline = previousGeneration
			End If
			Return emitBaseline
		End Function
	End Module
End Namespace