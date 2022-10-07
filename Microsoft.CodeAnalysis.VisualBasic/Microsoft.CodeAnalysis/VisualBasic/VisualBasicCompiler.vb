Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Collections
Imports Microsoft.CodeAnalysis.Diagnostics
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class VisualBasicCompiler
		Inherits CommonCompiler
		Friend Const ResponseFileName As String = "vbc.rsp"

		Friend Const VbcCommandLinePrefix As String = "vbc : "

		Private ReadOnly Shared s_responseFileName As String

		Private ReadOnly _responseFile As String

		Private ReadOnly _diagnosticFormatter As CommandLineDiagnosticFormatter

		Private ReadOnly _tempDirectory As String

		Private _additionalTextFiles As ImmutableArray(Of AdditionalTextFile)

		Friend Shadows ReadOnly Property Arguments As VisualBasicCommandLineArguments
			Get
				Return DirectCast(MyBase.Arguments, VisualBasicCommandLineArguments)
			End Get
		End Property

		Public Overrides ReadOnly Property DiagnosticFormatter As Microsoft.CodeAnalysis.DiagnosticFormatter
			Get
				Return Me._diagnosticFormatter
			End Get
		End Property

		Friend Overrides ReadOnly Property Type As System.Type
			Get
				Return GetType(VisualBasicCompiler)
			End Get
		End Property

		Protected Sub New(ByVal parser As VisualBasicCommandLineParser, ByVal responseFile As String, ByVal args As String(), ByVal buildPaths As Microsoft.CodeAnalysis.BuildPaths, ByVal additionalReferenceDirectories As String, ByVal analyzerLoader As IAnalyzerAssemblyLoader)
			MyBase.New(parser, responseFile, args, buildPaths, additionalReferenceDirectories, analyzerLoader)
			Me._diagnosticFormatter = New CommandLineDiagnosticFormatter(buildPaths.WorkingDirectory, New Func(Of ImmutableArray(Of AdditionalTextFile))(AddressOf Me.GetAdditionalTextFiles))
			Me._additionalTextFiles = New ImmutableArray(Of AdditionalTextFile)()
			Me._tempDirectory = buildPaths.TempDirectory
		End Sub

		Public Overrides Function CreateCompilation(ByVal consoleOutput As TextWriter, ByVal touchedFilesLogger As TouchedFileLogger, ByVal errorLogger As Microsoft.CodeAnalysis.ErrorLogger, ByVal analyzerConfigOptions As ImmutableArray(Of AnalyzerConfigOptionsResult), ByVal globalAnalyzerConfigOptions As AnalyzerConfigOptionsResult) As Microsoft.CodeAnalysis.Compilation
			Dim variable As VisualBasicCompiler._Closure$__15-0 = Nothing
			Dim compilation As Microsoft.CodeAnalysis.Compilation
			variable = New VisualBasicCompiler._Closure$__15-0(variable) With
			{
				.$VB$Me = Me,
				.$VB$Local_consoleOutput = consoleOutput,
				.$VB$Local_errorLogger = errorLogger,
				.$VB$Local_parseOptions = Me.Arguments.ParseOptions,
				.$VB$Local_scriptParseOptions = variable.$VB$Local_parseOptions.WithKind(SourceCodeKind.Script),
				.$VB$Local_hadErrors = False,
				.$VB$Local_sourceFiles = Me.Arguments.SourceFiles,
				.$VB$Local_trees = New SyntaxTree(variable.$VB$Local_sourceFiles.Length - 1 + 1 - 1) {}
			}
			If (Not Me.Arguments.CompilationOptions.ConcurrentBuild) Then
				Dim length As Integer = variable.$VB$Local_sourceFiles.Length - 1
				For num As Integer = 0 To length
					variable.$VB$Local_trees(num) = Me.ParseFile(variable.$VB$Local_consoleOutput, variable.$VB$Local_parseOptions, variable.$VB$Local_scriptParseOptions, variable.$VB$Local_hadErrors, variable.$VB$Local_sourceFiles(num), variable.$VB$Local_errorLogger)
				Next

			Else
				RoslynParallel.[For](0, variable.$VB$Local_sourceFiles.Length, UICultureUtilities.WithCurrentUICulture(Of Integer)(Sub(i As Integer) Me.$VB$Local_trees(i) = Me.$VB$Me.ParseFile(Me.$VB$Local_consoleOutput, Me.$VB$Local_parseOptions, Me.$VB$Local_scriptParseOptions, Me.$VB$Local_hadErrors, Me.$VB$Local_sourceFiles(i), Me.$VB$Local_errorLogger)), CancellationToken.None)
			End If
			If (Not variable.$VB$Local_hadErrors) Then
				If (Me.Arguments.TouchedFilesPath IsNot Nothing) Then
					Dim enumerator As ImmutableArray(Of CommandLineSourceFile).Enumerator = variable.$VB$Local_sourceFiles.GetEnumerator()
					While enumerator.MoveNext()
						touchedFilesLogger.AddRead(enumerator.Current.Path)
					End While
				End If
				Dim diagnosticInfos As List(Of DiagnosticInfo) = New List(Of DiagnosticInfo)()
				Dim [default] As DesktopAssemblyIdentityComparer = DesktopAssemblyIdentityComparer.[Default]
				Dim metadataReferenceResolver As Microsoft.CodeAnalysis.MetadataReferenceResolver = Nothing
				Dim metadataReferences As List(Of MetadataReference) = MyBase.ResolveMetadataReferences(diagnosticInfos, touchedFilesLogger, metadataReferenceResolver)
				If (Not MyBase.ReportDiagnostics(diagnosticInfos, variable.$VB$Local_consoleOutput, variable.$VB$Local_errorLogger, Nothing)) Then
					If (Me.Arguments.OutputLevel = OutputLevel.Verbose) Then
						Me.PrintReferences(metadataReferences, variable.$VB$Local_consoleOutput)
					End If
					Dim loggingXmlFileResolver As CommonCompiler.LoggingXmlFileResolver = New CommonCompiler.LoggingXmlFileResolver(Me.Arguments.BaseDirectory, touchedFilesLogger)
					Dim loggingSourceFileResolver As CommonCompiler.LoggingSourceFileResolver = New CommonCompiler.LoggingSourceFileResolver(ImmutableArray(Of String).Empty, Me.Arguments.BaseDirectory, Me.Arguments.PathMap, touchedFilesLogger)
					Dim loggingStrongNameFileSystem As CommonCompiler.LoggingStrongNameFileSystem = New CommonCompiler.LoggingStrongNameFileSystem(touchedFilesLogger, Me._tempDirectory)
					Dim compilerSyntaxTreeOptionsProvider As Microsoft.CodeAnalysis.CompilerSyntaxTreeOptionsProvider = New Microsoft.CodeAnalysis.CompilerSyntaxTreeOptionsProvider(variable.$VB$Local_trees, analyzerConfigOptions, globalAnalyzerConfigOptions)
					compilation = VisualBasicCompilation.Create(Me.Arguments.CompilationName, variable.$VB$Local_trees, metadataReferences, Me.Arguments.CompilationOptions.WithMetadataReferenceResolver(metadataReferenceResolver).WithAssemblyIdentityComparer([default]).WithXmlReferenceResolver(loggingXmlFileResolver).WithStrongNameProvider(Me.Arguments.GetStrongNameProvider(loggingStrongNameFileSystem)).WithSourceReferenceResolver(loggingSourceFileResolver).WithSyntaxTreeOptionsProvider(compilerSyntaxTreeOptionsProvider))
				Else
					compilation = Nothing
				End If
			Else
				compilation = Nothing
			End If
			Return compilation
		End Function

		Private Function GetAdditionalTextFiles() As ImmutableArray(Of AdditionalTextFile)
			Return Me._additionalTextFiles
		End Function

		Protected Overrides Function GetOutputFileName(ByVal compilation As Microsoft.CodeAnalysis.Compilation, ByVal cancellationToken As System.Threading.CancellationToken) As String
			Return Me.Arguments.OutputFileName
		End Function

		Friend Overrides Function GetToolName() As String
			Return ErrorFactory.IdToString(ERRID.IDS_ToolName, Me.Culture)
		End Function

		Private Function ParseFile(ByVal consoleOutput As TextWriter, ByVal parseOptions As VisualBasicParseOptions, ByVal scriptParseOptions As VisualBasicParseOptions, ByRef hadErrors As Boolean, ByVal file As CommandLineSourceFile, ByVal errorLogger As Microsoft.CodeAnalysis.ErrorLogger) As Microsoft.CodeAnalysis.SyntaxTree
			Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree
			Dim flag As Boolean
			Dim visualBasicParseOption As VisualBasicParseOptions
			Dim diagnosticInfos As List(Of DiagnosticInfo) = New List(Of DiagnosticInfo)()
			Dim sourceText As Microsoft.CodeAnalysis.Text.SourceText = MyBase.TryReadFileContent(file, diagnosticInfos)
			If (sourceText IsNot Nothing) Then
				Dim sourceText1 As Microsoft.CodeAnalysis.Text.SourceText = sourceText
				visualBasicParseOption = If(file.IsScript, scriptParseOptions, parseOptions)
				Dim path As String = file.Path
				Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
				Dim syntaxTree1 As Microsoft.CodeAnalysis.SyntaxTree = VisualBasicSyntaxTree.ParseText(sourceText1, visualBasicParseOption, path, DirectCast(Nothing, ImmutableDictionary(Of String, ReportDiagnostic)), cancellationToken)
				syntaxTree1.GetMappedLineSpanAndVisibility(New TextSpan(), flag)
				syntaxTree = syntaxTree1
			Else
				MyBase.ReportDiagnostics(diagnosticInfos, consoleOutput, errorLogger, Nothing)
				diagnosticInfos.Clear()
				hadErrors = True
				syntaxTree = Nothing
			End If
			Return syntaxTree
		End Function

		Public Overrides Sub PrintHelp(ByVal consoleOutput As TextWriter)
			consoleOutput.WriteLine(ErrorFactory.IdToString(ERRID.IDS_VBCHelp, Me.Culture))
		End Sub

		Public Overrides Sub PrintLangVersions(ByVal consoleOutput As TextWriter)
			Dim enumerator As IEnumerator = Nothing
			consoleOutput.WriteLine(ErrorFactory.IdToString(ERRID.IDS_LangVersions, Me.Culture))
			Dim effectiveVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.[Default].MapSpecifiedToEffectiveVersion()
			Dim languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.Latest.MapSpecifiedToEffectiveVersion()
			Try
				enumerator = [Enum].GetValues(GetType(Microsoft.CodeAnalysis.VisualBasic.LanguageVersion)).GetEnumerator()
				While enumerator.MoveNext()
					Dim [integer] As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion = Microsoft.VisualBasic.CompilerServices.Conversions.ToInteger(enumerator.Current)
					If ([integer] = effectiveVersion) Then
						consoleOutput.WriteLine([String].Format("{0} (default)", [integer].ToDisplayString()))
					ElseIf ([integer] <> languageVersion) Then
						consoleOutput.WriteLine([integer].ToDisplayString())
					Else
						consoleOutput.WriteLine([String].Format("{0} (latest)", [integer].ToDisplayString()))
					End If
				End While
			Finally
				If (TypeOf enumerator Is IDisposable) Then
					TryCast(enumerator, IDisposable).Dispose()
				End If
			End Try
			consoleOutput.WriteLine()
		End Sub

		Public Overrides Sub PrintLogo(ByVal consoleOutput As TextWriter)
			consoleOutput.WriteLine(ErrorFactory.IdToString(ERRID.IDS_LogoLine1, Me.Culture), Me.GetToolName(), MyBase.GetCompilerVersion())
			consoleOutput.WriteLine(ErrorFactory.IdToString(ERRID.IDS_LogoLine2, Me.Culture))
			consoleOutput.WriteLine()
		End Sub

		Private Sub PrintReferences(ByVal resolvedReferences As List(Of MetadataReference), ByVal consoleOutput As TextWriter)
			Dim enumerator As List(Of MetadataReference).Enumerator = New List(Of MetadataReference).Enumerator()
			Try
				enumerator = resolvedReferences.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As MetadataReference = enumerator.Current
					If (current.Properties.Kind = MetadataImageKind.[Module]) Then
						consoleOutput.WriteLine(ErrorFactory.IdToString(ERRID.IDS_MSG_ADDMODULE, Me.Culture), current.Display)
					ElseIf (Not current.Properties.EmbedInteropTypes) Then
						consoleOutput.WriteLine(ErrorFactory.IdToString(ERRID.IDS_MSG_ADDREFERENCE, Me.Culture), current.Display)
					Else
						consoleOutput.WriteLine(ErrorFactory.IdToString(ERRID.IDS_MSG_ADDLINKREFERENCE, Me.Culture), current.Display)
					End If
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			consoleOutput.WriteLine()
		End Sub

		Protected Overrides Function ResolveAdditionalFilesFromArguments(ByVal diagnostics As List(Of DiagnosticInfo), ByVal messageProvider As CommonMessageProvider, ByVal touchedFilesLogger As TouchedFileLogger) As ImmutableArray(Of AdditionalTextFile)
			Me._additionalTextFiles = MyBase.ResolveAdditionalFilesFromArguments(diagnostics, messageProvider, touchedFilesLogger)
			Return Me._additionalTextFiles
		End Function

		Protected Overrides Sub ResolveAnalyzersFromArguments(ByVal diagnostics As List(Of DiagnosticInfo), ByVal messageProvider As CommonMessageProvider, ByVal skipAnalyzers As Boolean, ByRef analyzers As ImmutableArray(Of DiagnosticAnalyzer), ByRef generators As ImmutableArray(Of ISourceGenerator))
			Me.Arguments.ResolveAnalyzersFromArguments("Visual Basic", diagnostics, messageProvider, MyBase.AssemblyLoader, skipAnalyzers, analyzers, generators)
		End Sub

		Protected Overrides Sub ResolveEmbeddedFilesFromExternalSourceDirectives(ByVal tree As SyntaxTree, ByVal resolver As SourceReferenceResolver, ByVal embeddedFiles As OrderedSet(Of String), ByVal diagnostics As DiagnosticBag)
			Dim enumerator As IEnumerator(Of DirectiveTriviaSyntax) = Nothing
			Dim func As Func(Of DirectiveTriviaSyntax, Boolean)
			Try
				Dim root As SyntaxNode = tree.GetRoot(New CancellationToken())
				If (VisualBasicCompiler._Closure$__.$I27-0 Is Nothing) Then
					func = Function(d As DirectiveTriviaSyntax) d.Kind() = SyntaxKind.ExternalSourceDirectiveTrivia
					VisualBasicCompiler._Closure$__.$I27-0 = func
				Else
					func = VisualBasicCompiler._Closure$__.$I27-0
				End If
				enumerator = root.GetDirectives(func).GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ExternalSourceDirectiveTriviaSyntax = DirectCast(enumerator.Current, ExternalSourceDirectiveTriviaSyntax)
					If (current.ExternalSource.IsMissing) Then
						Continue While
					End If
					Dim str As String = Microsoft.VisualBasic.CompilerServices.Conversions.ToString(current.ExternalSource.Value)
					If (str Is Nothing) Then
						Continue While
					End If
					Dim str1 As String = resolver.ResolveReference(str, tree.FilePath)
					If (str1 IsNot Nothing) Then
						embeddedFiles.Add(str1)
					Else
						diagnostics.Add(MyBase.MessageProvider.CreateDiagnostic(MyBase.MessageProvider.ERR_FileNotFound, current.ExternalSource.GetLocation(), New [Object]() { str }))
					End If
				End While
			Finally
				If (enumerator IsNot Nothing) Then
					enumerator.Dispose()
				End If
			End Try
		End Sub

		Protected Friend Overrides Function RunGenerators(ByVal input As Microsoft.CodeAnalysis.Compilation, ByVal parseOptions As Microsoft.CodeAnalysis.ParseOptions, ByVal generators As ImmutableArray(Of ISourceGenerator), ByVal analyzerConfigOptionsProvider As Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider, ByVal additionalTexts As ImmutableArray(Of AdditionalText), ByVal diagnostics As DiagnosticBag) As Microsoft.CodeAnalysis.Compilation
			Dim visualBasicGeneratorDriver As Microsoft.CodeAnalysis.VisualBasic.VisualBasicGeneratorDriver = Microsoft.CodeAnalysis.VisualBasic.VisualBasicGeneratorDriver.Create(generators, additionalTexts, DirectCast(parseOptions, VisualBasicParseOptions), analyzerConfigOptionsProvider)
			Dim compilation As Microsoft.CodeAnalysis.Compilation = Nothing
			Dim diagnostics1 As ImmutableArray(Of Diagnostic) = New ImmutableArray(Of Diagnostic)()
			Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
			visualBasicGeneratorDriver.RunGeneratorsAndUpdateCompilation(input, compilation, diagnostics1, cancellationToken)
			diagnostics.AddRange(Of Diagnostic)(diagnostics1)
			Return compilation
		End Function

		Friend Overrides Function SuppressDefaultResponseFile(ByVal args As IEnumerable(Of String)) As Boolean
			Dim flag As Boolean
			Dim enumerator As IEnumerator(Of String) = Nothing
			Using enumerator
				enumerator = args.GetEnumerator()
				While enumerator.MoveNext()
					Dim lowerInvariant As String = enumerator.Current.ToLowerInvariant()
					If (EmbeddedOperators.CompareString(lowerInvariant, "/noconfig", False) <> 0 AndAlso EmbeddedOperators.CompareString(lowerInvariant, "-noconfig", False) <> 0 AndAlso EmbeddedOperators.CompareString(lowerInvariant, "/nostdlib", False) <> 0 AndAlso EmbeddedOperators.CompareString(lowerInvariant, "-nostdlib", False) <> 0) Then
						Continue While
					End If
					flag = True
					Return flag
				End While
			End Using
			flag = False
			Return flag
		End Function

		Protected Overrides Function TryGetCompilerDiagnosticCode(ByVal diagnosticId As String, ByRef code As UInteger) As Boolean
			Return CommonCompiler.TryGetCompilerDiagnosticCode(diagnosticId, "BC", code)
		End Function
	End Class
End Namespace