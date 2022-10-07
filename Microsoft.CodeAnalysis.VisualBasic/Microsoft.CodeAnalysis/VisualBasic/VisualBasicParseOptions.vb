Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public NotInheritable Class VisualBasicParseOptions
		Inherits ParseOptions
		Implements IEquatable(Of VisualBasicParseOptions)
		Private Shared s_defaultPreprocessorSymbols As ImmutableArray(Of KeyValuePair(Of String, Object))

		Private _features As ImmutableDictionary(Of String, String)

		Private _preprocessorSymbols As ImmutableArray(Of KeyValuePair(Of String, Object))

		Private _specifiedLanguageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion

		Private _languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion

		Public ReadOnly Shared Property [Default] As VisualBasicParseOptions

		Private ReadOnly Shared Property DefaultPreprocessorSymbols As ImmutableArray(Of KeyValuePair(Of String, Object))
			Get
				If (VisualBasicParseOptions.s_defaultPreprocessorSymbols.IsDefaultOrEmpty) Then
					VisualBasicParseOptions.s_defaultPreprocessorSymbols = ImmutableArray.Create(Of KeyValuePair(Of String, Object))(KeyValuePairUtil.Create(Of String, Object)("_MYTYPE", "Empty"))
				End If
				Return VisualBasicParseOptions.s_defaultPreprocessorSymbols
			End Get
		End Property

		Public Overrides ReadOnly Property Features As IReadOnlyDictionary(Of String, String)
			Get
				Return Me._features
			End Get
		End Property

		Public Overrides ReadOnly Property Language As String
			Get
				Return "Visual Basic"
			End Get
		End Property

		Public ReadOnly Property LanguageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion
			Get
				Return Me._languageVersion
			End Get
		End Property

		Public Overrides ReadOnly Property PreprocessorSymbolNames As IEnumerable(Of String)
			Get
				Dim key As Func(Of KeyValuePair(Of String, Object), String)
				Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of String, Object)) = Me._preprocessorSymbols
				If (VisualBasicParseOptions._Closure$__.$I23-0 Is Nothing) Then
					key = Function(ps As KeyValuePair(Of String, Object)) ps.Key
					VisualBasicParseOptions._Closure$__.$I23-0 = key
				Else
					key = VisualBasicParseOptions._Closure$__.$I23-0
				End If
				Return keyValuePairs.[Select](Of String)(key)
			End Get
		End Property

		Public ReadOnly Property PreprocessorSymbols As ImmutableArray(Of KeyValuePair(Of String, Object))
			Get
				Return Me._preprocessorSymbols
			End Get
		End Property

		Public ReadOnly Property SpecifiedLanguageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion
			Get
				Return Me._specifiedLanguageVersion
			End Get
		End Property

		Shared Sub New()
			VisualBasicParseOptions.[Default] = New VisualBasicParseOptions(Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.[Default], Microsoft.CodeAnalysis.DocumentationMode.Parse, SourceCodeKind.Regular, Nothing)
		End Sub

		Public Sub New(Optional ByVal languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion = 0, Optional ByVal documentationMode As Microsoft.CodeAnalysis.DocumentationMode = 1, Optional ByVal kind As SourceCodeKind = 0, Optional ByVal preprocessorSymbols As IEnumerable(Of KeyValuePair(Of String, Object)) = Nothing)
			MyClass.New(languageVersion, documentationMode, kind, If(preprocessorSymbols Is Nothing, VisualBasicParseOptions.DefaultPreprocessorSymbols, ImmutableArray.CreateRange(Of KeyValuePair(Of String, Object))(preprocessorSymbols)), ImmutableDictionary(Of String, String).Empty)
		End Sub

		Friend Sub New(ByVal languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion, ByVal documentationMode As Microsoft.CodeAnalysis.DocumentationMode, ByVal kind As SourceCodeKind, ByVal preprocessorSymbols As ImmutableArray(Of KeyValuePair(Of String, Object)), ByVal features As ImmutableDictionary(Of String, String))
			MyBase.New(kind, documentationMode)
			Me._specifiedLanguageVersion = languageVersion
			Me._languageVersion = languageVersion.MapSpecifiedToEffectiveVersion()
			Me._preprocessorSymbols = Roslyn.Utilities.ImmutableArrayExtensions.ToImmutableArrayOrEmpty(Of KeyValuePair(Of String, Object))(preprocessorSymbols)
			Dim empty As Object = features
			If (empty Is Nothing) Then
				empty = ImmutableDictionary(Of String, String).Empty
			End If
			Me._features = DirectCast(empty, ImmutableDictionary(Of String, String))
		End Sub

		Private Sub New(ByVal other As VisualBasicParseOptions)
			MyClass.New(other._specifiedLanguageVersion, other.DocumentationMode, other.Kind, other._preprocessorSymbols, other._features)
		End Sub

		Protected Overrides Function CommonWithDocumentationMode(ByVal documentationMode As Microsoft.CodeAnalysis.DocumentationMode) As ParseOptions
			Return Me.WithDocumentationMode(documentationMode)
		End Function

		Protected Overrides Function CommonWithFeatures(ByVal features As IEnumerable(Of KeyValuePair(Of String, String))) As ParseOptions
			Return Me.WithFeatures(features)
		End Function

		Public Overrides Function CommonWithKind(ByVal kind As SourceCodeKind) As ParseOptions
			Return Me.WithKind(kind)
		End Function

		Public Function ExplicitEquals(ByVal other As VisualBasicParseOptions) As Boolean Implements IEquatable(Of VisualBasicParseOptions).Equals
			Dim flag As Boolean
			If (CObj(Me) = CObj(other)) Then
				flag = True
			ElseIf (Not MyBase.EqualsHelper(other)) Then
				flag = False
			ElseIf (Me.SpecifiedLanguageVersion = other.SpecifiedLanguageVersion) Then
				flag = If(System.Linq.ImmutableArrayExtensions.SequenceEqual(Of KeyValuePair(Of String, Object), KeyValuePair(Of String, Object))(Me.PreprocessorSymbols, other.PreprocessorSymbols, DirectCast(Nothing, IEqualityComparer(Of KeyValuePair(Of String, Object)))), True, False)
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean Implements IEquatable(Of VisualBasicParseOptions).Equals
			Return Me.ExplicitEquals(TryCast(obj, VisualBasicParseOptions))
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(MyBase.GetHashCodeHelper(), CInt(Me.SpecifiedLanguageVersion))
		End Function

		Friend Overrides Sub ValidateOptions(ByVal builder As ArrayBuilder(Of Diagnostic))
			MyBase.ValidateOptions(builder, MessageProvider.Instance)
			If (Not Me.LanguageVersion.IsValid()) Then
				builder.Add(Diagnostic.Create(MessageProvider.Instance, 37287, New [Object]() { Me.LanguageVersion.ToString() }))
			End If
			If (Not Me.PreprocessorSymbols.IsDefaultOrEmpty) Then
				Dim enumerator As ImmutableArray(Of KeyValuePair(Of String, Object)).Enumerator = Me.PreprocessorSymbols.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As KeyValuePair(Of String, Object) = enumerator.Current
					If (Not SyntaxFacts.IsValidIdentifier(current.Key) OrElse SyntaxFacts.GetKeywordKind(current.Key) <> SyntaxKind.None) Then
						builder.Add(Diagnostic.Create(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, New [Object]() { ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedIdentifier), current.Key })))
					End If
					If (CConst.TryCreate(RuntimeHelpers.GetObjectValue(current.Value)) IsNot Nothing) Then
						Continue While
					End If
					builder.Add(Diagnostic.Create(MessageProvider.Instance, 37288, New [Object]() { current.Key, current.Value.[GetType]() }))
				End While
			End If
		End Sub

		Public Shadows Function WithDocumentationMode(ByVal documentationMode As Microsoft.CodeAnalysis.DocumentationMode) As VisualBasicParseOptions
			Dim visualBasicParseOption As VisualBasicParseOptions
			visualBasicParseOption = If(documentationMode <> MyBase.DocumentationMode, New VisualBasicParseOptions(Me) With
			{
				.DocumentationMode = documentationMode
			}, Me)
			Return visualBasicParseOption
		End Function

		Public Shadows Function WithFeatures(ByVal features As IEnumerable(Of KeyValuePair(Of String, String))) As VisualBasicParseOptions
			Dim visualBasicParseOption As VisualBasicParseOptions
			visualBasicParseOption = If(features IsNot Nothing, New VisualBasicParseOptions(Me) With
			{
				._features = features.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase)
			}, New VisualBasicParseOptions(Me) With
			{
				._features = ImmutableDictionary(Of String, String).Empty
			})
			Return visualBasicParseOption
		End Function

		Public Shadows Function WithKind(ByVal kind As SourceCodeKind) As VisualBasicParseOptions
			Dim visualBasicParseOption As VisualBasicParseOptions
			If (kind <> MyBase.SpecifiedKind) Then
				Dim effectiveKind As SourceCodeKind = kind.MapSpecifiedToEffectiveKind()
				visualBasicParseOption = New VisualBasicParseOptions(Me) With
				{
					.SpecifiedKind = kind,
					.Kind = effectiveKind
				}
			Else
				visualBasicParseOption = Me
			End If
			Return visualBasicParseOption
		End Function

		Public Function WithLanguageVersion(ByVal version As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion) As VisualBasicParseOptions
			Dim visualBasicParseOption As VisualBasicParseOptions
			If (version <> Me._specifiedLanguageVersion) Then
				Dim effectiveVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion = version.MapSpecifiedToEffectiveVersion()
				visualBasicParseOption = New VisualBasicParseOptions(Me) With
				{
					._specifiedLanguageVersion = version,
					._languageVersion = effectiveVersion
				}
			Else
				visualBasicParseOption = Me
			End If
			Return visualBasicParseOption
		End Function

		Public Function WithPreprocessorSymbols(ByVal symbols As IEnumerable(Of KeyValuePair(Of String, Object))) As VisualBasicParseOptions
			Return Me.WithPreprocessorSymbols(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of KeyValuePair(Of String, Object))(symbols))
		End Function

		Public Function WithPreprocessorSymbols(ByVal ParamArray symbols As KeyValuePair(Of String, Object)()) As VisualBasicParseOptions
			Return Me.WithPreprocessorSymbols(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of KeyValuePair(Of String, Object))(symbols))
		End Function

		Public Function WithPreprocessorSymbols(ByVal symbols As ImmutableArray(Of KeyValuePair(Of String, Object))) As VisualBasicParseOptions
			Dim visualBasicParseOption As VisualBasicParseOptions
			If (symbols.IsDefault) Then
				symbols = ImmutableArray(Of KeyValuePair(Of String, Object)).Empty
			End If
			visualBasicParseOption = If(Not symbols.Equals(Me.PreprocessorSymbols), New VisualBasicParseOptions(Me) With
			{
				._preprocessorSymbols = symbols
			}, Me)
			Return visualBasicParseOption
		End Function
	End Class
End Namespace