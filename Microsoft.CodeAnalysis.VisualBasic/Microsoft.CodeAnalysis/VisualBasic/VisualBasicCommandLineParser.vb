Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Class VisualBasicCommandLineParser
		Inherits CommandLineParser
		Private Const s_win32Manifest As String = "win32manifest"

		Private Const s_win32Icon As String = "win32icon"

		Private Const s_win32Res As String = "win32resource"

		Public ReadOnly Shared Property [Default] As VisualBasicCommandLineParser

		Protected Overrides ReadOnly Property RegularFileExtension As String
			Get
				Return ".vb"
			End Get
		End Property

		Public ReadOnly Shared Property Script As VisualBasicCommandLineParser

		Protected Overrides ReadOnly Property ScriptFileExtension As String
			Get
				Return ".vbx"
			End Get
		End Property

		Shared Sub New()
			VisualBasicCommandLineParser.[Default] = New VisualBasicCommandLineParser(False)
			VisualBasicCommandLineParser.Script = New VisualBasicCommandLineParser(True)
		End Sub

		Friend Sub New(Optional ByVal isScriptCommandLineParser As Boolean = False)
			MyBase.New(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance, isScriptCommandLineParser)
		End Sub

		Private Shared Sub AddDiagnostic(ByVal diagnostics As IList(Of Diagnostic), ByVal errorCode As ERRID, ByVal ParamArray arguments As Object())
			diagnostics.Add(Diagnostic.Create(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance, CInt(errorCode), arguments))
		End Sub

		Private Shared Sub AddInvalidSwitchValueDiagnostic(ByVal diagnostics As IList(Of Diagnostic), ByVal name As String, ByVal nullStringText As String)
			If ([String].IsNullOrEmpty(name)) Then
				name = "(null)"
			End If
			VisualBasicCommandLineParser.AddDiagnostic(diagnostics, ERRID.ERR_InvalidSwitchValue, New [Object]() { name, nullStringText })
		End Sub

		Private Shared Sub AddNormalizedPaths(ByVal builder As ArrayBuilder(Of String), ByVal paths As List(Of String), ByVal baseDirectory As String)
			Dim enumerator As List(Of String).Enumerator = New List(Of String).Enumerator()
			Try
				enumerator = paths.GetEnumerator()
				While enumerator.MoveNext()
					Dim str As String = FileUtilities.NormalizeRelativePath(enumerator.Current, Nothing, baseDirectory)
					If (str Is Nothing) Then
						Continue While
					End If
					builder.Add(str)
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
		End Sub

		Private Shared Sub AddWarnings(ByVal d As IDictionary(Of String, Microsoft.CodeAnalysis.ReportDiagnostic), ByVal kind As Microsoft.CodeAnalysis.ReportDiagnostic, ByVal items As IEnumerable(Of String))
			Dim enumerator As IEnumerator(Of String) = Nothing
			Dim reportDiagnostic As Microsoft.CodeAnalysis.ReportDiagnostic
			Try
				enumerator = items.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As String = enumerator.Current
					If (Not d.TryGetValue(current, reportDiagnostic)) Then
						d.Add(current, kind)
					Else
						If (reportDiagnostic = Microsoft.CodeAnalysis.ReportDiagnostic.Suppress) Then
							Continue While
						End If
						d(current) = kind
					End If
				End While
			Finally
				If (enumerator IsNot Nothing) Then
					enumerator.Dispose()
				End If
			End Try
		End Sub

		Private Shared Function BuildSearchPaths(ByVal baseDirectory As String, ByVal sdkPaths As List(Of String), ByVal responsePaths As List(Of String), ByVal libPaths As List(Of String)) As ImmutableArray(Of String)
			Dim instance As ArrayBuilder(Of String) = ArrayBuilder(Of String).GetInstance()
			VisualBasicCommandLineParser.AddNormalizedPaths(instance, sdkPaths, baseDirectory)
			instance.AddRange(responsePaths)
			VisualBasicCommandLineParser.AddNormalizedPaths(instance, libPaths, baseDirectory)
			Return instance.ToImmutableAndFree()
		End Function

		Friend NotOverridable Overrides Function CommonParse(ByVal args As IEnumerable(Of String), ByVal baseDirectory As String, ByVal sdkDirectoryOpt As String, ByVal additionalReferenceDirectories As String) As CommandLineArguments
			Return Me.Parse(args, baseDirectory, sdkDirectoryOpt, additionalReferenceDirectories)
		End Function

		Private Shared Function FindFileInSdkPath(ByVal sdkPaths As List(Of String), ByVal fileName As String, ByVal baseDirectory As String) As String
			Dim str As String
			Dim enumerator As List(Of String).Enumerator = New List(Of String).Enumerator()
			Try
				enumerator = sdkPaths.GetEnumerator()
				While enumerator.MoveNext()
					Dim str1 As String = FileUtilities.ResolveRelativePath(enumerator.Current, baseDirectory)
					If (str1 Is Nothing) Then
						Continue While
					End If
					Dim str2 As String = PathUtilities.CombineAbsoluteAndRelativePaths(str1, fileName)
					If (Not File.Exists(str2)) Then
						Continue While
					End If
					str = str2
					Return str
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			str = Nothing
			Return str
		End Function

		Friend Overrides Sub GenerateErrorForNoFilesFoundInRecurse(ByVal path As String, ByVal errors As IList(Of Diagnostic))
			VisualBasicCommandLineParser.AddDiagnostic(errors, ERRID.ERR_InvalidSwitchValue, New [Object]() { "recurse", path })
		End Sub

		Private Sub GetCompilationAndModuleNames(ByVal diagnostics As List(Of Diagnostic), ByVal kind As OutputKind, ByVal sourceFiles As List(Of Microsoft.CodeAnalysis.CommandLineSourceFile), ByVal moduleAssemblyName As String, ByRef outputFileName As String, ByRef moduleName As String, <Out> ByRef compilationName As String)
			Dim str As String = Nothing
			If (outputFileName IsNot Nothing) Then
				Dim extension As String = PathUtilities.GetExtension(outputFileName)
				If (Not kind.IsNetModule()) Then
					If (Not extension.Equals(".exe", StringComparison.OrdinalIgnoreCase) And Not extension.Equals(".dll", StringComparison.OrdinalIgnoreCase) And Not extension.Equals(".netmodule", StringComparison.OrdinalIgnoreCase) And Not extension.Equals(".winmdobj", StringComparison.OrdinalIgnoreCase)) Then
						str = outputFileName
						outputFileName = [String].Concat(outputFileName, EnumBounds.GetDefaultExtension(kind))
					End If
					If (str Is Nothing) Then
						str = PathUtilities.RemoveExtension(outputFileName)
						If (str.Length = 0) Then
							VisualBasicCommandLineParser.AddDiagnostic(diagnostics, ERRID.FTL_InvalidInputFileName, New [Object]() { outputFileName })
							str = Nothing
							outputFileName = Nothing
						End If
					End If
				ElseIf (extension.Length = 0) Then
					outputFileName = [String].Concat(outputFileName, ".netmodule")
				End If
			Else
				Dim commandLineSourceFile As Microsoft.CodeAnalysis.CommandLineSourceFile = sourceFiles.FirstOrDefault()
				If (commandLineSourceFile.Path IsNot Nothing) Then
					str = PathUtilities.RemoveExtension(PathUtilities.GetFileName(commandLineSourceFile.Path, True))
					outputFileName = [String].Concat(str, EnumBounds.GetDefaultExtension(kind))
					If (str.Length = 0 AndAlso Not kind.IsNetModule()) Then
						VisualBasicCommandLineParser.AddDiagnostic(diagnostics, ERRID.FTL_InvalidInputFileName, New [Object]() { outputFileName })
						str = Nothing
						outputFileName = Nothing
					End If
				End If
			End If
			If (Not kind.IsNetModule()) Then
				If (moduleAssemblyName IsNot Nothing) Then
					VisualBasicCommandLineParser.AddDiagnostic(diagnostics, ERRID.ERR_NeedModule, New [Object](-1) {})
				End If
				compilationName = str
			Else
				compilationName = moduleAssemblyName
			End If
			If (moduleName Is Nothing) Then
				moduleName = outputFileName
			End If
		End Sub

		Private Shared Sub GetErrorStringForRemainderOfConditionalCompilation(ByVal tokens As IEnumerator(Of Microsoft.CodeAnalysis.SyntaxToken), ByVal remainderErrorLine As StringBuilder, Optional ByVal includeCurrentToken As Boolean = False, Optional ByVal stopTokenKind As SyntaxKind = 636)
			Dim current As Microsoft.CodeAnalysis.SyntaxToken
			If (Not includeCurrentToken) Then
				remainderErrorLine.Append(" ^^ ^^ ")
			Else
				remainderErrorLine.Append(" ^^ ")
				If (tokens.Current.Kind() = SyntaxKind.ColonToken) Then
					current = tokens.Current
					If (current.FullWidth <> 0) Then
						GoTo Label1
					End If
					remainderErrorLine.Append(SyntaxFacts.GetText(SyntaxKind.ColonToken))
					GoTo Label0
				End If
			Label1:
				current = tokens.Current
				remainderErrorLine.Append(current.ToFullString())
			Label0:
				remainderErrorLine.Append(" ^^ ")
			End If
			While tokens.MoveNext() AndAlso tokens.Current.Kind() <> stopTokenKind
				current = tokens.Current
				remainderErrorLine.Append(current.ToFullString())
			End While
		End Sub

		Private Shared Function GetWin32Setting(ByVal arg As String, ByVal value As String, ByVal diagnostics As List(Of Diagnostic)) As String
			Dim str As String
			If (value IsNot Nothing) Then
				Dim str1 As String = CommandLineParser.RemoveQuotesAndSlashes(value)
				If ([String].IsNullOrWhiteSpace(str1)) Then
					VisualBasicCommandLineParser.AddDiagnostic(diagnostics, ERRID.ERR_ArgumentRequired, New [Object]() { arg, ":<file>" })
					str = Nothing
					Return str
				End If
				str = str1
				Return str
			Else
				VisualBasicCommandLineParser.AddDiagnostic(diagnostics, ERRID.ERR_ArgumentRequired, New [Object]() { arg, ":<file>" })
			End If
			str = Nothing
			Return str
		End Function

		Private Shared Function InternalDefinesToPublicSymbols(ByVal defines As ImmutableDictionary(Of String, CConst)) As IReadOnlyDictionary(Of String, Object)
			Dim enumerator As ImmutableDictionary(Of String, CConst).Enumerator = New ImmutableDictionary(Of String, CConst).Enumerator()
			Dim objectValue As ImmutableDictionary(Of String, Object).Builder = ImmutableDictionary.CreateBuilder(Of String, Object)(CaseInsensitiveComparison.Comparer)
			Try
				enumerator = defines.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As KeyValuePair(Of String, CConst) = enumerator.Current
					objectValue(current.Key) = RuntimeHelpers.GetObjectValue(current.Value.ValueAsObject)
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			Return objectValue.ToImmutable()
		End Function

		Private Shared Function IsSeparatorOrEndOfFile(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Boolean
			If (token.Kind() = SyntaxKind.EndOfFileToken OrElse token.Kind() = SyntaxKind.ColonToken) Then
				Return True
			End If
			Return token.Kind() = SyntaxKind.CommaToken
		End Function

		Private Function LoadCoreLibraryReference(ByVal sdkPaths As List(Of String), ByVal baseDirectory As String) As Nullable(Of CommandLineReference)
			Dim nullable As Nullable(Of CommandLineReference)
			Dim strs As ImmutableArray(Of String)
			Dim str As String = VisualBasicCommandLineParser.FindFileInSdkPath(sdkPaths, "mscorlib.dll", baseDirectory)
			Dim str1 As String = VisualBasicCommandLineParser.FindFileInSdkPath(sdkPaths, "System.Runtime.dll", baseDirectory)
			If (str1 IsNot Nothing) Then
				If (str IsNot Nothing) Then
					Try
						Using assemblyMetadatum As AssemblyMetadata = AssemblyMetadata.CreateFromFile(str1)
							If (assemblyMetadatum.GetModules()(0).[Module].IsLinkedModule AndAlso assemblyMetadatum.GetAssembly().AssemblyReferences.Length = 0) Then
								strs = New ImmutableArray(Of String)()
								nullable = New Nullable(Of CommandLineReference)(New CommandLineReference(str1, New MetadataReferenceProperties(MetadataImageKind.Assembly, strs, False)))
								Return nullable
							End If
						End Using
					Catch exception As System.Exception
						ProjectData.SetProjectError(exception)
						ProjectData.ClearProjectError()
					End Try
					strs = New ImmutableArray(Of String)()
					nullable = New Nullable(Of CommandLineReference)(New CommandLineReference(str, New MetadataReferenceProperties(MetadataImageKind.Assembly, strs, False)))
				Else
					strs = New ImmutableArray(Of String)()
					nullable = New Nullable(Of CommandLineReference)(New CommandLineReference(str1, New MetadataReferenceProperties(MetadataImageKind.Assembly, strs, False)))
				End If
			ElseIf (str Is Nothing) Then
				nullable = Nothing
			Else
				strs = New ImmutableArray(Of String)()
				nullable = New Nullable(Of CommandLineReference)(New CommandLineReference(str, New MetadataReferenceProperties(MetadataImageKind.Assembly, strs, False)))
			End If
			Return nullable
		End Function

		Public Shadows Function Parse(ByVal args As IEnumerable(Of String), ByVal baseDirectory As String, ByVal sdkDirectory As String, Optional ByVal additionalReferenceDirectories As String = Nothing) As VisualBasicCommandLineArguments
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.VisualBasicCommandLineArguments Microsoft.CodeAnalysis.VisualBasic.VisualBasicCommandLineParser::Parse(System.Collections.Generic.IEnumerable`1<System.String>,System.String,System.String,System.String)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.VisualBasicCommandLineArguments Parse(System.Collections.Generic.IEnumerable<System.String>,System.String,System.String,System.String)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function ParseAnalyzers(ByVal name As String, ByVal value As String, ByVal diagnostics As IList(Of Diagnostic)) As IEnumerable(Of Microsoft.CodeAnalysis.CommandLineAnalyzerReference)
			Dim commandLineAnalyzerReferences As IEnumerable(Of Microsoft.CodeAnalysis.CommandLineAnalyzerReference)
			Dim commandLineAnalyzerReference As Func(Of String, Microsoft.CodeAnalysis.CommandLineAnalyzerReference)
			If (Not [String].IsNullOrEmpty(value)) Then
				Dim strs As IEnumerable(Of String) = CommandLineParser.ParseSeparatedPaths(value)
				If (VisualBasicCommandLineParser._Closure$__.$I25-0 Is Nothing) Then
					commandLineAnalyzerReference = Function(path As String) New Microsoft.CodeAnalysis.CommandLineAnalyzerReference(path)
					VisualBasicCommandLineParser._Closure$__.$I25-0 = commandLineAnalyzerReference
				Else
					commandLineAnalyzerReference = VisualBasicCommandLineParser._Closure$__.$I25-0
				End If
				commandLineAnalyzerReferences = strs.[Select](Of Microsoft.CodeAnalysis.CommandLineAnalyzerReference)(commandLineAnalyzerReference)
			Else
				VisualBasicCommandLineParser.AddDiagnostic(diagnostics, ERRID.ERR_ArgumentRequired, New [Object]() { name, ":<file_list>" })
				commandLineAnalyzerReferences = SpecializedCollections.EmptyEnumerable(Of Microsoft.CodeAnalysis.CommandLineAnalyzerReference)()
			End If
			Return commandLineAnalyzerReferences
		End Function

		Friend Shared Function ParseAssemblyReferences(ByVal name As String, ByVal value As String, ByVal diagnostics As IList(Of Diagnostic), ByVal embedInteropTypes As Boolean) As IEnumerable(Of CommandLineReference)
			Dim commandLineReferences As IEnumerable(Of CommandLineReference)
			If (Not [String].IsNullOrEmpty(value)) Then
				commandLineReferences = CommandLineParser.ParseSeparatedPaths(value).[Select](Of CommandLineReference)(Function(path As String) New CommandLineReference(path, New MetadataReferenceProperties(MetadataImageKind.Assembly, New ImmutableArray(Of String)(), embedInteropTypes)))
			Else
				VisualBasicCommandLineParser.AddDiagnostic(diagnostics, ERRID.ERR_ArgumentRequired, New [Object]() { name, ":<file_list>" })
				commandLineReferences = SpecializedCollections.EmptyEnumerable(Of CommandLineReference)()
			End If
			Return commandLineReferences
		End Function

		Private Shared Function ParseBaseAddress(ByVal name As String, ByVal value As String, ByVal errors As List(Of Diagnostic)) As ULong
			Dim num As ULong
			Dim num1 As ULong
			If (Not [String].IsNullOrEmpty(value)) Then
				Dim str As String = value
				If (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) Then
					str = value.Substring(2)
				End If
				If (Not UInt64.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, num1)) Then
					VisualBasicCommandLineParser.AddDiagnostic(errors, ERRID.ERR_InvalidSwitchValue, New [Object]() { name, value.ToString() })
					num = CULng(0)
					Return num
				End If
				num = num1
				Return num
			Else
				VisualBasicCommandLineParser.AddDiagnostic(errors, ERRID.ERR_ArgumentRequired, New [Object]() { name, ":<number>" })
			End If
			num = CULng(0)
			Return num
		End Function

		Private Shared Function ParseConditionalCompilationExpression(ByVal symbolList As String, ByVal offset As Integer) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Using parser As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser(Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.MakeSourceText(symbolList, offset), VisualBasicParseOptions.[Default], New CancellationToken())
				parser.GetNextToken(ScannerState.VB)
				expressionSyntax = DirectCast(parser.ParseConditionalCompilationExpression().CreateRed(Nothing, 0), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			End Using
			Return expressionSyntax
		End Function

		Public Shared Function ParseConditionalCompilationSymbols(ByVal symbolList As String, <Out> ByRef diagnostics As IEnumerable(Of Diagnostic), Optional ByVal symbols As IEnumerable(Of KeyValuePair(Of String, Object)) = Nothing) As IReadOnlyDictionary(Of String, Object)
			Dim str As String
			Dim current As Microsoft.CodeAnalysis.SyntaxToken
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Dim flag As Boolean
			Dim enumerator As IEnumerator(Of Microsoft.CodeAnalysis.DiagnosticInfo) = Nothing
			Dim func As Func(Of Microsoft.CodeAnalysis.SyntaxTrivia, Boolean)
			Dim instance As ArrayBuilder(Of Diagnostic) = ArrayBuilder(Of Diagnostic).GetInstance()
			Dim stringBuilder As System.Text.StringBuilder = New System.Text.StringBuilder()
			Dim internalDefines As ImmutableDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst) = VisualBasicCommandLineParser.PublicSymbolsToInternalDefines(symbols, instance)
			Do
				str = symbolList
				symbolList = symbolList.Unquote()
			Loop While Not [String].Equals(symbolList, str, StringComparison.Ordinal)
			symbolList = symbolList.Replace("\""", """")
			Dim str1 As String = symbolList.TrimEnd(New [Char](-1) {})
			If (str1.Length > 0 AndAlso SyntaxFacts.IsConnectorPunctuation(str1(str1.Length - 1))) Then
				symbolList = [String].Concat(symbolList, ",")
			End If
			Using enumerator1 As IEnumerator(Of Microsoft.CodeAnalysis.SyntaxToken) = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ParseTokens(symbolList, 0, 0, Nothing).GetEnumerator()
				If (enumerator1.MoveNext()) Then
					While True
						current = enumerator1.Current
						If (current.Position > 0 AndAlso Not VisualBasicCommandLineParser.IsSeparatorOrEndOfFile(enumerator1.Current)) Then
							stringBuilder.Append(" ^^ ^^ ")
							While Not VisualBasicCommandLineParser.IsSeparatorOrEndOfFile(enumerator1.Current)
								current = enumerator1.Current
								stringBuilder.Append(current.ToFullString())
								enumerator1.MoveNext()
							End While
							instance.Add(New DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, New [Object]() { ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedEOS), stringBuilder.ToString() }), Location.None, False))
							diagnostics = instance.ToArrayAndFree()
							Return VisualBasicCommandLineParser.InternalDefinesToPublicSymbols(internalDefines)
						End If
						Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = New Microsoft.CodeAnalysis.SyntaxToken()
						While enumerator1.Current.Kind() = SyntaxKind.CommaToken OrElse enumerator1.Current.Kind() = SyntaxKind.ColonToken
							If (syntaxToken.Kind() = SyntaxKind.None) Then
								syntaxToken = enumerator1.Current
							ElseIf (syntaxToken.Kind() <> enumerator1.Current.Kind()) Then
								VisualBasicCommandLineParser.GetErrorStringForRemainderOfConditionalCompilation(enumerator1, stringBuilder, True, syntaxToken.Kind())
								instance.Add(New DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, New [Object]() { ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedIdentifier), stringBuilder.ToString() }), Location.None, False))
							End If
							current = enumerator1.Current
							stringBuilder.Append(current.ToString())
							If (enumerator1.Current.Kind() = SyntaxKind.EndOfFileToken) Then
								Continue While
							End If
							enumerator1.MoveNext()
						End While
						stringBuilder.Clear()
						If (enumerator1.Current.Kind() = SyntaxKind.EndOfFileToken) Then
							GoTo Label2
						End If
						current = enumerator1.Current
						stringBuilder.Append(current.ToFullString())
						If (enumerator1.Current.Kind() <> SyntaxKind.IdentifierToken) Then
							VisualBasicCommandLineParser.GetErrorStringForRemainderOfConditionalCompilation(enumerator1, stringBuilder, False, SyntaxKind.CommaToken)
							instance.Add(New DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, New [Object]() { ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedIdentifier), stringBuilder.ToString() }), Location.None, False))
							diagnostics = instance.ToArrayAndFree()
							Return VisualBasicCommandLineParser.InternalDefinesToPublicSymbols(internalDefines)
						End If
						current = enumerator1.Current
						Dim valueText As String = current.ValueText
						enumerator1.MoveNext()
						If (enumerator1.Current.Kind() <> SyntaxKind.EqualsToken) Then
							If (enumerator1.Current.Kind() <> SyntaxKind.CommaToken AndAlso enumerator1.Current.Kind() <> SyntaxKind.ColonToken AndAlso enumerator1.Current.Kind() <> SyntaxKind.EndOfFileToken) Then
								Exit While
							End If
							internalDefines = internalDefines.SetItem(valueText, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(True))
						Else
							current = enumerator1.Current
							stringBuilder.Append(current.ToFullString())
							enumerator1.MoveNext()
							current = enumerator1.Current
							Dim spanStart As Integer = current.SpanStart
							expressionSyntax = VisualBasicCommandLineParser.ParseConditionalCompilationExpression(symbolList, spanStart)
							Dim [end] As Integer = spanStart + expressionSyntax.Span.[End]
							flag = VisualBasicCommandLineParser.IsSeparatorOrEndOfFile(enumerator1.Current)
							While enumerator1.Current.Kind() <> SyntaxKind.EndOfFileToken
								current = enumerator1.Current
								If (current.Span.[End] > [end]) Then
									Exit While
								End If
								current = enumerator1.Current
								stringBuilder.Append(current.ToFullString())
								enumerator1.MoveNext()
								flag = VisualBasicCommandLineParser.IsSeparatorOrEndOfFile(enumerator1.Current)
							End While
							If (expressionSyntax.ContainsDiagnostics) Then
								GoTo Label4
							End If
							Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = ExpressionEvaluator.EvaluateExpression(DirectCast(expressionSyntax.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), internalDefines)
							Dim errorId As ERRID = cConst.ErrorId
							If (errorId <> ERRID.ERR_None) Then
								VisualBasicCommandLineParser.GetErrorStringForRemainderOfConditionalCompilation(enumerator1, stringBuilder, False, SyntaxKind.CommaToken)
								instance.Add(New DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, New [Object]() { ErrorFactory.ErrorInfo(errorId, cConst.ErrorArgs), stringBuilder.ToString() }), Location.None, False))
								diagnostics = instance.ToArrayAndFree()
								Return VisualBasicCommandLineParser.InternalDefinesToPublicSymbols(internalDefines)
							End If
							internalDefines = internalDefines.SetItem(valueText, cConst)
						End If
					End While
					If (enumerator1.Current.Kind() <> SyntaxKind.BadToken) Then
						VisualBasicCommandLineParser.GetErrorStringForRemainderOfConditionalCompilation(enumerator1, stringBuilder, False, SyntaxKind.CommaToken)
						instance.Add(New DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, New [Object]() { ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedEOS), stringBuilder.ToString() }), Location.None, False))
					Else
						VisualBasicCommandLineParser.GetErrorStringForRemainderOfConditionalCompilation(enumerator1, stringBuilder, False, SyntaxKind.CommaToken)
						instance.Add(New DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, New [Object]() { ErrorFactory.ErrorInfo(ERRID.ERR_IllegalChar), stringBuilder.ToString() }), Location.None, False))
						diagnostics = instance.ToArrayAndFree()
						Return VisualBasicCommandLineParser.InternalDefinesToPublicSymbols(internalDefines)
					End If
				End If
			End Using
			diagnostics = instance.ToArrayAndFree()
			Return VisualBasicCommandLineParser.InternalDefinesToPublicSymbols(internalDefines)
		Label2:
			Dim current1 As Microsoft.CodeAnalysis.SyntaxToken = enumerator1.Current
			If (current1.FullWidth > 0) Then
				Dim leadingTrivia As IEnumerable(Of Microsoft.CodeAnalysis.SyntaxTrivia) = DirectCast(current1.LeadingTrivia, IEnumerable(Of Microsoft.CodeAnalysis.SyntaxTrivia))
				If (VisualBasicCommandLineParser._Closure$__.$I31-0 Is Nothing) Then
					func = Function(t As Microsoft.CodeAnalysis.SyntaxTrivia) t.Kind() = SyntaxKind.WhitespaceTrivia
					VisualBasicCommandLineParser._Closure$__.$I31-0 = func
				Else
					func = VisualBasicCommandLineParser._Closure$__.$I31-0
				End If
				If (Not leadingTrivia.All(func)) Then
					VisualBasicCommandLineParser.GetErrorStringForRemainderOfConditionalCompilation(enumerator1, stringBuilder, True, SyntaxKind.CommaToken)
					instance.Add(New DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, New [Object]() { ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedIdentifier), stringBuilder.ToString() }), Location.None, False))
					diagnostics = instance.ToArrayAndFree()
					Return VisualBasicCommandLineParser.InternalDefinesToPublicSymbols(internalDefines)
				Else
					diagnostics = instance.ToArrayAndFree()
					Return VisualBasicCommandLineParser.InternalDefinesToPublicSymbols(internalDefines)
				End If
			Else
				diagnostics = instance.ToArrayAndFree()
				Return VisualBasicCommandLineParser.InternalDefinesToPublicSymbols(internalDefines)
			End If
		Label4:
			stringBuilder.Append(" ^^ ^^ ")
			While Not VisualBasicCommandLineParser.IsSeparatorOrEndOfFile(enumerator1.Current)
				current = enumerator1.Current
				stringBuilder.Append(current.ToFullString())
				enumerator1.MoveNext()
			End While
			Using flag1 As Boolean = False
				enumerator = expressionSyntax.VbGreen.GetSyntaxErrors().GetEnumerator()
				While enumerator.MoveNext()
					Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = enumerator.Current
					If (diagnosticInfo.Code = 30201 OrElse diagnosticInfo.Code = 31427) Then
						flag1 = True
					Else
						instance.Add(New DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, New [Object]() { diagnosticInfo, stringBuilder.ToString() }), Location.None, False))
					End If
				End While
			End Using
			If (flag1) Then
				instance.Add(New DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, New [Object]() { ErrorFactory.ErrorInfo(If(flag, ERRID.ERR_ExpectedExpression, ERRID.ERR_BadCCExpression)), stringBuilder.ToString() }), Location.None, False))
				diagnostics = instance.ToArrayAndFree()
				Return VisualBasicCommandLineParser.InternalDefinesToPublicSymbols(internalDefines)
			Else
				diagnostics = instance.ToArrayAndFree()
				Return VisualBasicCommandLineParser.InternalDefinesToPublicSymbols(internalDefines)
			End If
		End Function

		Private Shared Function ParseFileAlignment(ByVal name As String, ByVal value As String, ByVal errors As List(Of Diagnostic)) As Integer
			Dim num As Integer
			Dim num1 As UShort
			If ([String].IsNullOrEmpty(value)) Then
				VisualBasicCommandLineParser.AddDiagnostic(errors, ERRID.ERR_ArgumentRequired, New [Object]() { name, ":<number>" })
			ElseIf (CommandLineParser.TryParseUInt16(value, num1)) Then
				If (Not CompilationOptions.IsValidFileAlignment(CInt(num1))) Then
					VisualBasicCommandLineParser.AddDiagnostic(errors, ERRID.ERR_InvalidSwitchValue, New [Object]() { name, value })
					num = 0
					Return num
				End If
				num = num1
				Return num
			Else
				VisualBasicCommandLineParser.AddDiagnostic(errors, ERRID.ERR_InvalidSwitchValue, New [Object]() { name, value })
			End If
			num = 0
			Return num
		End Function

		Private Shared Sub ParseGlobalImports(ByVal value As String, ByVal globalImports As List(Of Microsoft.CodeAnalysis.VisualBasic.GlobalImport), ByVal errors As List(Of Diagnostic))
			Dim enumerator As IEnumerator(Of String) = Nothing
			Using strs As IEnumerable(Of String) = CommandLineParser.ParseSeparatedPaths(value)
				enumerator = strs.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As String = enumerator.Current
					Dim diagnostics As ImmutableArray(Of Diagnostic) = New ImmutableArray(Of Diagnostic)()
					Dim globalImport As Microsoft.CodeAnalysis.VisualBasic.GlobalImport = Microsoft.CodeAnalysis.VisualBasic.GlobalImport.Parse(current, diagnostics)
					errors.AddRange(DirectCast(diagnostics, IEnumerable(Of Diagnostic)))
					globalImports.Add(globalImport)
				End While
			End Using
		End Sub

		Private Shared Function ParseInstrumentationKinds(ByVal value As String, ByVal diagnostics As IList(Of Diagnostic)) As IEnumerable(Of InstrumentationKind)
			Return New VisualBasicCommandLineParser.VB$StateMachine_33_ParseInstrumentationKinds(-2) With
			{
				.$P_value = value,
				.$P_diagnostics = diagnostics
			}
		End Function

		Private Shared Function ParsePlatform(ByVal name As String, ByVal value As String, ByVal errors As List(Of Diagnostic)) As Platform
			' 
			' Current member / type: Microsoft.CodeAnalysis.Platform Microsoft.CodeAnalysis.VisualBasic.VisualBasicCommandLineParser::ParsePlatform(System.String,System.String,System.Collections.Generic.List`1<Microsoft.CodeAnalysis.Diagnostic>)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.Platform ParsePlatform(System.String,System.String,System.Collections.Generic.List<Microsoft.CodeAnalysis.Diagnostic>)
			' 
			' L'index était hors limites. Il ne doit pas être négatif et doit être inférieur à la taille de la collection.
			' Nom du paramètre : index
			'    à System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
			'    à ..(Int32 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 364
			'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 74
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 55
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Friend Shared Function ParseResourceDescription(ByVal name As String, ByVal resourceDescriptor As String, ByVal baseDirectory As String, ByVal diagnostics As IList(Of Diagnostic), ByVal embedded As Boolean) As Microsoft.CodeAnalysis.ResourceDescription
			Dim resourceDescription As Microsoft.CodeAnalysis.ResourceDescription
			Dim flag As Boolean
			If (Not [String].IsNullOrEmpty(resourceDescriptor)) Then
				Dim str As String = Nothing
				Dim str1 As String = Nothing
				Dim str2 As String = Nothing
				Dim str3 As String = Nothing
				Dim str4 As String = Nothing
				CommandLineParser.ParseResourceDescription(resourceDescriptor, baseDirectory, True, str, str1, str2, str3, str4)
				If ([String].IsNullOrWhiteSpace(str)) Then
					VisualBasicCommandLineParser.AddInvalidSwitchValueDiagnostic(diagnostics, name, str)
					resourceDescription = Nothing
				ElseIf (PathUtilities.IsValidFilePath(str1)) Then
					If ([String].IsNullOrEmpty(str4)) Then
						flag = True
					ElseIf (Not [String].Equals(str4, "public", StringComparison.OrdinalIgnoreCase)) Then
						If ([String].Equals(str4, "private", StringComparison.OrdinalIgnoreCase)) Then
							GoTo Label1
						End If
						VisualBasicCommandLineParser.AddInvalidSwitchValueDiagnostic(diagnostics, name, str4)
						resourceDescription = Nothing
						Return resourceDescription
					Else
						flag = True
					End If
				Label2:
					Dim fileStream As Func(Of Stream) = Function() New System.IO.FileStream(str1, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
					resourceDescription = New Microsoft.CodeAnalysis.ResourceDescription(str3, str2, fileStream, flag, embedded, False)
				Else
					VisualBasicCommandLineParser.AddDiagnostic(diagnostics, ERRID.FTL_InvalidInputFileName, New [Object]() { str })
					resourceDescription = Nothing
				End If
			Else
				VisualBasicCommandLineParser.AddDiagnostic(diagnostics, ERRID.ERR_ArgumentRequired, New [Object]() { name, ":<resinfo>" })
				resourceDescription = Nothing
			End If
			Return resourceDescription
		Label1:
			flag = False
			GoTo Label2
		End Function

		Private Shared Function ParseTarget(ByVal optionName As String, ByVal value As String, ByVal diagnostics As IList(Of Diagnostic)) As OutputKind
			' 
			' Current member / type: Microsoft.CodeAnalysis.OutputKind Microsoft.CodeAnalysis.VisualBasic.VisualBasicCommandLineParser::ParseTarget(System.String,System.String,System.Collections.Generic.IList`1<Microsoft.CodeAnalysis.Diagnostic>)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.OutputKind ParseTarget(System.String,System.String,System.Collections.Generic.IList<Microsoft.CodeAnalysis.Diagnostic>)
			' 
			' L'index était hors limites. Il ne doit pas être négatif et doit être inférieur à la taille de la collection.
			' Nom du paramètre : index
			'    à System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
			'    à ..(Int32 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 364
			'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 74
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 55
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function ParseWarnings(ByVal value As String) As IEnumerable(Of String)
			Dim enumerator As IEnumerator(Of String) = Nothing
			Dim num As UShort
			Dim strs As IEnumerable(Of String) = CommandLineParser.ParseSeparatedPaths(value)
			Using strs1 As List(Of String) = New List(Of String)()
				enumerator = strs.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As String = enumerator.Current
					If (Not UInt16.TryParse(current, NumberStyles.[Integer], CultureInfo.InvariantCulture, num) OrElse Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance.GetSeverity(CInt(num)) <> DiagnosticSeverity.Warning OrElse Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance.GetWarningLevel(CInt(num)) <> 1) Then
						strs1.Add(current)
					Else
						strs1.Add(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance.GetIdForErrorCode(CInt(num)))
					End If
				End While
			End Using
			Return strs1
		End Function

		Private Shared Function PublicSymbolsToInternalDefines(ByVal symbols As IEnumerable(Of KeyValuePair(Of String, Object)), ByVal diagnosticBuilder As ArrayBuilder(Of Diagnostic)) As ImmutableDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst)
			Dim enumerator As IEnumerator(Of KeyValuePair(Of String, Object)) = Nothing
			Dim strs As ImmutableDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst).Builder = ImmutableDictionary.CreateBuilder(Of String, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst)(CaseInsensitiveComparison.Comparer)
			If (symbols IsNot Nothing) Then
				Try
					enumerator = symbols.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of String, Object) = enumerator.Current
						Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.TryCreate(RuntimeHelpers.GetObjectValue(current.Value))
						If (cConst Is Nothing) Then
							diagnosticBuilder.Add(Diagnostic.Create(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance, 37288, New [Object]() { current.Key, current.Value.[GetType]() }))
						End If
						strs(current.Key) = cConst
					End While
				Finally
					If (enumerator IsNot Nothing) Then
						enumerator.Dispose()
					End If
				End Try
			End If
			Return strs.ToImmutable()
		End Function

		Private Shared Sub UnimplementedSwitch(ByVal diagnostics As IList(Of Diagnostic), ByVal switchName As String)
			VisualBasicCommandLineParser.AddDiagnostic(diagnostics, ERRID.WRN_UnimplementedCommandLineSwitch, New [Object]() { [String].Concat("/", switchName) })
		End Sub

		Private Shared Sub ValidateWin32Settings(ByVal noWin32Manifest As Boolean, ByVal win32ResSetting As String, ByVal win32IconSetting As String, ByVal win32ManifestSetting As String, ByVal outputKind As Microsoft.CodeAnalysis.OutputKind, ByVal diagnostics As List(Of Diagnostic))
			If (noWin32Manifest AndAlso win32ManifestSetting IsNot Nothing) Then
				VisualBasicCommandLineParser.AddDiagnostic(diagnostics, ERRID.ERR_ConflictingManifestSwitches, New [Object](-1) {})
			End If
			If (win32ResSetting IsNot Nothing) Then
				If (win32IconSetting IsNot Nothing) Then
					VisualBasicCommandLineParser.AddDiagnostic(diagnostics, ERRID.ERR_IconFileAndWin32ResFile, New [Object](-1) {})
				End If
				If (win32ManifestSetting IsNot Nothing) Then
					VisualBasicCommandLineParser.AddDiagnostic(diagnostics, ERRID.ERR_CantHaveWin32ResAndManifest, New [Object](-1) {})
				End If
			End If
			If (win32ManifestSetting IsNot Nothing AndAlso outputKind.IsNetModule()) Then
				VisualBasicCommandLineParser.AddDiagnostic(diagnostics, ERRID.WRN_IgnoreModuleManifest, New [Object](-1) {})
			End If
		End Sub
	End Class
End Namespace