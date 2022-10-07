Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class ImplicitVariableBinder
		Inherits Binder
		Private ReadOnly _containerOfLocals As Symbol

		Private _frozen As Boolean

		Private _implicitLocals As Dictionary(Of String, LocalSymbol)

		Private _possiblyShadowingVariables As MultiDictionary(Of String, ImplicitVariableBinder.ShadowedVariableInfo)

		Public Overrides ReadOnly Property AllImplicitVariableDeclarationsAreHandled As Boolean
			Get
				Return Me._frozen
			End Get
		End Property

		Public Overrides ReadOnly Property ImplicitlyDeclaredVariables As ImmutableArray(Of LocalSymbol)
			Get
				Dim immutableAndFree As ImmutableArray(Of LocalSymbol)
				If (Me._implicitLocals IsNot Nothing) Then
					Dim instance As ArrayBuilder(Of LocalSymbol) = ArrayBuilder(Of LocalSymbol).GetInstance()
					instance.AddRange(Me._implicitLocals.Values)
					immutableAndFree = instance.ToImmutableAndFree()
				Else
					immutableAndFree = ImmutableArray(Of LocalSymbol).Empty
				End If
				Return immutableAndFree
			End Get
		End Property

		Public Overrides ReadOnly Property ImplicitVariableDeclarationAllowed As Boolean
			Get
				Return True
			End Get
		End Property

		Public Sub New(ByVal containingBinder As Binder, ByVal containerOfLocals As Symbol)
			MyBase.New(containingBinder)
			Me._containerOfLocals = containerOfLocals
			Me._frozen = False
		End Sub

		Friend Overrides Sub AddLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.ImplicitVariableBinder::AddLookupSymbolsInfoInSingleBinder(Microsoft.CodeAnalysis.VisualBasic.LookupSymbolsInfo,Microsoft.CodeAnalysis.VisualBasic.LookupOptions,Microsoft.CodeAnalysis.VisualBasic.Binder)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void AddLookupSymbolsInfoInSingleBinder(Microsoft.CodeAnalysis.VisualBasic.LookupSymbolsInfo,Microsoft.CodeAnalysis.VisualBasic.LookupOptions,Microsoft.CodeAnalysis.VisualBasic.Binder)
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

		Friend Overrides Function BindFunctionAggregationExpression(ByVal [function] As FunctionAggregationSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundExpression
			Return MyBase.ContainingBinder.BindFunctionAggregationExpression([function], diagnostics)
		End Function

		Friend Overrides Function BindGroupAggregationExpression(ByVal group As GroupAggregationSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundExpression
			Return MyBase.ContainingBinder.BindGroupAggregationExpression(group, diagnostics)
		End Function

		Public Overrides Function DeclareImplicitLocalVariable(ByVal nameSyntax As IdentifierNameSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol
			Dim specialTypeForTypeCharacter As SpecialType = SpecialType.System_Object
			If (nameSyntax.Identifier.GetTypeCharacter() <> TypeCharacter.None) Then
				Dim str As String = Nothing
				specialTypeForTypeCharacter = Binder.GetSpecialTypeForTypeCharacter(nameSyntax.Identifier.GetTypeCharacter(), str)
			End If
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol.Create(Me._containerOfLocals, Me, nameSyntax.Identifier, LocalDeclarationKind.ImplicitVariable, MyBase.GetSpecialType(specialTypeForTypeCharacter, nameSyntax, diagnostics))
			If (Me._implicitLocals Is Nothing) Then
				Me._implicitLocals = New Dictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)(CaseInsensitiveComparison.Comparer)
			End If
			Me._implicitLocals.Add(nameSyntax.Identifier.ValueText, localSymbol)
			Return localSymbol
		End Function

		Public Overrides Sub DisallowFurtherImplicitVariableDeclaration(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim enumerator As Dictionary(Of String, LocalSymbol).KeyCollection.Enumerator = New Dictionary(Of String, LocalSymbol).KeyCollection.Enumerator()
			Dim enumerator1 As MultiDictionary(Of String, ImplicitVariableBinder.ShadowedVariableInfo).ValueSet.Enumerator = New MultiDictionary(Of String, ImplicitVariableBinder.ShadowedVariableInfo).ValueSet.Enumerator()
			If (Not Me._frozen) Then
				Me._frozen = True
				If (Me._implicitLocals IsNot Nothing AndAlso Me._possiblyShadowingVariables IsNot Nothing) Then
					Try
						enumerator = Me._implicitLocals.Keys.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As String = enumerator.Current
							Try
								enumerator1 = Me._possiblyShadowingVariables(current).GetEnumerator()
								While enumerator1.MoveNext()
									Dim shadowedVariableInfo As ImplicitVariableBinder.ShadowedVariableInfo = enumerator1.Current
									Binder.ReportDiagnostic(diagnostics, shadowedVariableInfo.Location, shadowedVariableInfo.ErrorId, New [Object]() { shadowedVariableInfo.Name })
								End While
							Finally
								DirectCast(enumerator1, IDisposable).Dispose()
							End Try
						End While
					Finally
						DirectCast(enumerator, IDisposable).Dispose()
					End Try
				End If
			End If
		End Sub

		Friend Overrides Sub LookupInSingleBinder(ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal name As String, ByVal arity As Integer, ByVal options As LookupOptions, ByVal originalBinder As Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.ImplicitVariableBinder::LookupInSingleBinder(Microsoft.CodeAnalysis.VisualBasic.LookupResult,System.String,System.Int32,Microsoft.CodeAnalysis.VisualBasic.LookupOptions,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void LookupInSingleBinder(Microsoft.CodeAnalysis.VisualBasic.LookupResult,System.String,System.Int32,Microsoft.CodeAnalysis.VisualBasic.LookupOptions,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
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

		Public Sub RememberPossibleShadowingVariable(ByVal name As String, ByVal syntax As SyntaxNodeOrToken, ByVal errorId As ERRID)
			If (Me._possiblyShadowingVariables Is Nothing) Then
				Me._possiblyShadowingVariables = New MultiDictionary(Of String, ImplicitVariableBinder.ShadowedVariableInfo)(CaseInsensitiveComparison.Comparer)
			End If
			Me._possiblyShadowingVariables.Add(name, New ImplicitVariableBinder.ShadowedVariableInfo(name, syntax.GetLocation(), errorId))
		End Sub

		Private Structure ShadowedVariableInfo
			Public ReadOnly Name As String

			Public ReadOnly Location As Location

			Public ReadOnly ErrorId As ERRID

			Public Sub New(ByVal name As String, ByVal location As Microsoft.CodeAnalysis.Location, ByVal errorId As ERRID)
				Me = New ImplicitVariableBinder.ShadowedVariableInfo() With
				{
					.Name = name,
					.Location = location,
					.ErrorId = errorId
				}
			End Sub
		End Structure
	End Class
End Namespace