Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class EmbeddedSymbolManager
		Friend ReadOnly IsReferencedPredicate As Func(Of Symbol, Boolean)

		Private ReadOnly _embedded As EmbeddedSymbolKind

		Private ReadOnly _symbols As ConcurrentDictionary(Of Symbol, Boolean)

		Private _sealed As Integer

		Private _standardModuleAttributeReferenced As Boolean

		Private Shared s_embeddedSyntax As SyntaxTree

		Private Shared s_vbCoreSyntax As SyntaxTree

		Private Shared s_internalXmlHelperSyntax As SyntaxTree

		Public ReadOnly Property Embedded As EmbeddedSymbolKind
			Get
				Return Me._embedded
			End Get
		End Property

		Public ReadOnly Shared Property EmbeddedSyntax As SyntaxTree
			Get
				If (EmbeddedSymbolManager.s_embeddedSyntax Is Nothing) Then
					Dim embedded As String = EmbeddedResources.Embedded
					Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
					Interlocked.CompareExchange(Of SyntaxTree)(EmbeddedSymbolManager.s_embeddedSyntax, VisualBasicSyntaxTree.ParseText(embedded, Nothing, "", DirectCast(Nothing, Encoding), Nothing, cancellationToken), Nothing)
					Dim sEmbeddedSyntax As SyntaxTree = EmbeddedSymbolManager.s_embeddedSyntax
					cancellationToken = New System.Threading.CancellationToken()
					If (sEmbeddedSyntax.GetDiagnostics(cancellationToken).Any()) Then
						Throw ExceptionUtilities.Unreachable
					End If
				End If
				Return EmbeddedSymbolManager.s_embeddedSyntax
			End Get
		End Property

		Public ReadOnly Shared Property InternalXmlHelperSyntax As SyntaxTree
			Get
				If (EmbeddedSymbolManager.s_internalXmlHelperSyntax Is Nothing) Then
					Dim internalXmlHelper As String = EmbeddedResources.InternalXmlHelper
					Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
					Interlocked.CompareExchange(Of SyntaxTree)(EmbeddedSymbolManager.s_internalXmlHelperSyntax, VisualBasicSyntaxTree.ParseText(internalXmlHelper, Nothing, "", DirectCast(Nothing, Encoding), Nothing, cancellationToken), Nothing)
					Dim sInternalXmlHelperSyntax As SyntaxTree = EmbeddedSymbolManager.s_internalXmlHelperSyntax
					cancellationToken = New System.Threading.CancellationToken()
					If (sInternalXmlHelperSyntax.GetDiagnostics(cancellationToken).Any()) Then
						Throw ExceptionUtilities.Unreachable
					End If
				End If
				Return EmbeddedSymbolManager.s_internalXmlHelperSyntax
			End Get
		End Property

		Public ReadOnly Property IsAnySymbolReferenced As Boolean
			Get
				If (Me._symbols Is Nothing) Then
					Return False
				End If
				Return Not Me._symbols.IsEmpty
			End Get
		End Property

		Public ReadOnly Shared Property VbCoreSyntaxTree As SyntaxTree
			Get
				If (EmbeddedSymbolManager.s_vbCoreSyntax Is Nothing) Then
					Dim vbCoreSourceText As String = EmbeddedResources.VbCoreSourceText
					Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
					Interlocked.CompareExchange(Of SyntaxTree)(EmbeddedSymbolManager.s_vbCoreSyntax, VisualBasicSyntaxTree.ParseText(vbCoreSourceText, Nothing, "", DirectCast(Nothing, Encoding), Nothing, cancellationToken), Nothing)
					Dim sVbCoreSyntax As SyntaxTree = EmbeddedSymbolManager.s_vbCoreSyntax
					cancellationToken = New System.Threading.CancellationToken()
					If (sVbCoreSyntax.GetDiagnostics(cancellationToken).Any()) Then
						Throw ExceptionUtilities.Unreachable
					End If
				End If
				Return EmbeddedSymbolManager.s_vbCoreSyntax
			End Get
		End Property

		Shared Sub New()
			EmbeddedSymbolManager.s_embeddedSyntax = Nothing
			EmbeddedSymbolManager.s_vbCoreSyntax = Nothing
			EmbeddedSymbolManager.s_internalXmlHelperSyntax = Nothing
		End Sub

		Public Sub New(ByVal embedded As EmbeddedSymbolKind)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolManager::.ctor(Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void .ctor(Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind)
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

		End Sub

		Private Sub AddReferencedSymbolRaw(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal allSymbols As ConcurrentSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))
			If (allSymbols.Add(symbol)) Then
				If (Me._sealed = 0) Then
					Me._symbols.TryAdd(symbol, True)
				End If
				Dim enumerator As ImmutableArray(Of VisualBasicAttributeData).Enumerator = symbol.GetAttributes().GetEnumerator()
				While enumerator.MoveNext()
					Me.AddReferencedSymbolWithDependents(enumerator.Current.AttributeClass, allSymbols)
				End While
			End If
		End Sub

		Private Sub AddReferencedSymbolWithDependents(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal allSymbols As ConcurrentSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))
			If (symbol.IsEmbedded AndAlso Not allSymbols.Contains(symbol)) Then
				Dim kind As Microsoft.CodeAnalysis.SymbolKind = symbol.Kind
				If (kind <= Microsoft.CodeAnalysis.SymbolKind.Method) Then
					If (kind = Microsoft.CodeAnalysis.SymbolKind.Field) Then
						Me.AddReferencedSymbolRaw(symbol, allSymbols)
						Me.AddReferencedSymbolWithDependents(symbol.ContainingType, allSymbols)
						Return
					End If
					If (kind <> Microsoft.CodeAnalysis.SymbolKind.Method) Then
						Return
					End If
					Me.AddReferencedSymbolRaw(symbol, allSymbols)
					Me.AddReferencedSymbolWithDependents(symbol.ContainingType, allSymbols)
					Dim methodKind As Microsoft.CodeAnalysis.MethodKind = DirectCast(symbol, MethodSymbol).MethodKind
					If (methodKind <> Microsoft.CodeAnalysis.MethodKind.Constructor) Then
						Select Case methodKind
							Case Microsoft.CodeAnalysis.MethodKind.Ordinary
							Case Microsoft.CodeAnalysis.MethodKind.StaticConstructor
								Exit Select
							Case Microsoft.CodeAnalysis.MethodKind.PropertyGet
							Case Microsoft.CodeAnalysis.MethodKind.PropertySet
								Me.AddReferencedSymbolWithDependents(DirectCast(symbol, MethodSymbol).AssociatedSymbol, allSymbols)
								Return
							Case Microsoft.CodeAnalysis.MethodKind.ReducedExtension
								Throw ExceptionUtilities.UnexpectedValue(methodKind)
							Case Else
								Throw ExceptionUtilities.UnexpectedValue(methodKind)
						End Select
					End If
				ElseIf (kind = Microsoft.CodeAnalysis.SymbolKind.NamedType) Then
					Me.AddReferencedSymbolRaw(symbol, allSymbols)
					Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = DirectCast(symbol, NamedTypeSymbol).GetMembers().GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator.Current
						Dim symbolKind As Microsoft.CodeAnalysis.SymbolKind = current.Kind
						If (symbolKind = Microsoft.CodeAnalysis.SymbolKind.Field) Then
							If (DirectCast(current, FieldSymbol).IsConst) Then
								Continue While
							End If
							Me.AddReferencedSymbolRaw(current, allSymbols)
						ElseIf (symbolKind = Microsoft.CodeAnalysis.SymbolKind.Method) Then
							Dim methodKind1 As Microsoft.CodeAnalysis.MethodKind = DirectCast(current, MethodSymbol).MethodKind
							If (methodKind1 <> Microsoft.CodeAnalysis.MethodKind.Constructor AndAlso methodKind1 <> Microsoft.CodeAnalysis.MethodKind.StaticConstructor) Then
								Continue While
							End If
							Me.AddReferencedSymbolRaw(current, allSymbols)
						End If
					End While
					If (symbol.ContainingType IsNot Nothing) Then
						Me.AddReferencedSymbolWithDependents(symbol.ContainingType, allSymbols)
					End If
				Else
					If (kind <> Microsoft.CodeAnalysis.SymbolKind.[Property]) Then
						Return
					End If
					Me.AddReferencedSymbolRaw(symbol, allSymbols)
					Me.AddReferencedSymbolWithDependents(symbol.ContainingType, allSymbols)
					Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
					If (propertySymbol.GetMethod IsNot Nothing) Then
						Me.AddReferencedSymbolWithDependents(propertySymbol.GetMethod, allSymbols)
					End If
					If (propertySymbol.SetMethod IsNot Nothing) Then
						Me.AddReferencedSymbolWithDependents(propertySymbol.SetMethod, allSymbols)
						Return
					End If
				End If
			End If
		End Sub

		<Conditional("DEBUG")>
		Friend Sub AssertMarkAllDeferredSymbolsAsReferencedIsCalled()
		End Sub

		Friend Sub GetCurrentReferencedSymbolsSnapshot(ByVal builder As ArrayBuilder(Of Symbol), ByVal filter As ConcurrentSet(Of Symbol))
			Dim array As KeyValuePair(Of Symbol, Boolean)() = Me._symbols.ToArray()
			For i As Integer = 0 To CInt(array.Length) Step 1
				Dim keyValuePair As KeyValuePair(Of Symbol, Boolean) = array(i)
				If (Not filter.Contains(keyValuePair.Key)) Then
					builder.Add(keyValuePair.Key)
				End If
			Next

		End Sub

		Friend Shared Function GetEmbeddedKind(ByVal tree As SyntaxTree) As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind
			Dim embeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind
			If (tree Is Nothing) Then
				embeddedSymbolKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind.None
			ElseIf (tree = EmbeddedSymbolManager.s_embeddedSyntax) Then
				embeddedSymbolKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind.EmbeddedAttribute
			ElseIf (tree <> EmbeddedSymbolManager.s_vbCoreSyntax) Then
				embeddedSymbolKind = If(tree <> EmbeddedSymbolManager.s_internalXmlHelperSyntax, Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind.None, Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind.XmlHelper)
			Else
				embeddedSymbolKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind.VbCore
			End If
			Return embeddedSymbolKind
		End Function

		Friend Shared Function GetEmbeddedTree(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind) As SyntaxTree
			Dim embeddedSyntax As SyntaxTree
			Dim embeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind = kind
			If (embeddedSymbolKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind.EmbeddedAttribute) Then
				embeddedSyntax = EmbeddedSymbolManager.EmbeddedSyntax
			ElseIf (embeddedSymbolKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind.VbCore) Then
				embeddedSyntax = EmbeddedSymbolManager.VbCoreSyntaxTree
			Else
				If (embeddedSymbolKind <> Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind.XmlHelper) Then
					Throw ExceptionUtilities.UnexpectedValue(kind)
				End If
				embeddedSyntax = EmbeddedSymbolManager.InternalXmlHelperSyntax
			End If
			Return embeddedSyntax
		End Function

		Public Function IsSymbolReferenced(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			Dim flag As Boolean = False
			Return Me._symbols.TryGetValue(symbol, flag)
		End Function

		Public Sub MarkAllDeferredSymbolsAsReferenced(ByVal compilation As VisualBasicCompilation)
			If (Me._standardModuleAttributeReferenced) Then
				Me.MarkSymbolAsReferenced(compilation.GetWellKnownType(WellKnownType.Microsoft_VisualBasic_CompilerServices_StandardModuleAttribute))
			End If
		End Sub

		Public Sub MarkSymbolAsReferenced(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal allSymbols As ConcurrentSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))
			If (Me._sealed = 0) Then
				Me.AddReferencedSymbolWithDependents(symbol, allSymbols)
			End If
		End Sub

		Public Sub MarkSymbolAsReferenced(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Me.MarkSymbolAsReferenced(symbol, New ConcurrentSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)(ReferenceEqualityComparer.Instance))
		End Sub

		Public Sub RegisterModuleDeclaration()
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolManager::RegisterModuleDeclaration()
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void RegisterModuleDeclaration()
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

		End Sub

		Public Sub SealCollection()
			Interlocked.CompareExchange(Me._sealed, 1, 0)
		End Sub

		<Conditional("DEBUG")>
		Private Shared Sub ValidateField(ByVal field As FieldSymbol)
			Dim type As TypeSymbol = field.Type
		End Sub

		<Conditional("DEBUG")>
		Friend Shared Sub ValidateMethod(ByVal method As MethodSymbol)
			Dim methodKind As Microsoft.CodeAnalysis.MethodKind = method.MethodKind
			Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = method.Parameters.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As ParameterSymbol = enumerator.Current
			End While
		End Sub

		<Conditional("DEBUG")>
		Private Shared Sub ValidateType(ByVal type As NamedTypeSymbol)
			Dim current As Symbol
			Dim kind As SymbolKind
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = type.GetMembers().GetEnumerator()
			Do
			Label0:
				If (Not enumerator.MoveNext()) Then
					Return
				End If
				current = enumerator.Current
				kind = current.Kind
				If (kind > SymbolKind.Method) Then
					Continue Do
				End If
				If (kind <> SymbolKind.Field AndAlso kind <> SymbolKind.Method) Then
					Exit Do
				Else
					GoTo Label0
				End If
			Loop While kind = SymbolKind.NamedType OrElse kind = SymbolKind.[Property]
			Throw ExceptionUtilities.UnexpectedValue(current.Kind)
		End Sub

		Friend NotInheritable Class EmbeddedNamedTypeSymbol
			Inherits SourceNamedTypeSymbol
			Private ReadOnly _kind As EmbeddedSymbolKind

			Friend Overrides ReadOnly Property AreMembersImplicitlyDeclared As Boolean
				Get
					Return True
				End Get
			End Property

			Friend Overrides ReadOnly Property EmbeddedSymbolKind As EmbeddedSymbolKind
				Get
					Return Me._kind
				End Get
			End Property

			Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
				Get
					Return True
				End Get
			End Property

			Public Sub New(ByVal decl As MergedTypeDeclaration, ByVal containingSymbol As NamespaceOrTypeSymbol, ByVal containingModule As SourceModuleSymbol, ByVal kind As EmbeddedSymbolKind)
				MyBase.New(decl, containingSymbol, containingModule)
				Me._kind = kind
			End Sub

			Friend Overrides Function GetMembersForCci() As ImmutableArray(Of Symbol)
				Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
				Dim embeddedSymbolManager As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolManager = Me.DeclaringCompilation.EmbeddedSymbolManager
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = Me.GetMembers().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					If (Not embeddedSymbolManager.IsSymbolReferenced(current)) Then
						Continue While
					End If
					instance.Add(current)
				End While
				Return instance.ToImmutableAndFree()
			End Function
		End Class
	End Class
End Namespace