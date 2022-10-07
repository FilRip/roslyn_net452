Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SourceComplexParameterSymbol
		Inherits SourceParameterSymbol
		Private ReadOnly _syntaxRef As SyntaxReference

		Private ReadOnly _flags As SourceParameterFlags

		Private _lazyDefaultValue As ConstantValue

		Private _lazyCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData)

		Friend Overrides ReadOnly Property AttributeDeclarationList As SyntaxList(Of AttributeListSyntax)
			Get
				If (Me._syntaxRef Is Nothing) Then
					Return New SyntaxList(Of AttributeListSyntax)()
				End If
				Return DirectCast(Me._syntaxRef.GetSyntax(New CancellationToken()), ParameterSyntax).AttributeLists
			End Get
		End Property

		Private ReadOnly Property BoundAttributesSource As SourceParameterSymbol
			Get
				Dim item As SourceParameterSymbol
				Dim containingSymbol As SourceMemberMethodSymbol = TryCast(MyBase.ContainingSymbol, SourceMemberMethodSymbol)
				If (containingSymbol IsNot Nothing) Then
					Dim sourcePartialDefinition As SourceMemberMethodSymbol = containingSymbol.SourcePartialDefinition
					If (sourcePartialDefinition IsNot Nothing) Then
						item = DirectCast(sourcePartialDefinition.Parameters(MyBase.Ordinal), SourceParameterSymbol)
					Else
						item = Nothing
					End If
				Else
					item = Nothing
				End If
				Return item
			End Get
		End Property

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Dim empty As ImmutableArray(Of SyntaxReference)
				If (Not MyBase.IsImplicitlyDeclared) Then
					empty = If(Me._syntaxRef Is Nothing, MyBase.DeclaringSyntaxReferences, Symbol.GetDeclaringSyntaxReferenceHelper(Me._syntaxRef))
				Else
					empty = ImmutableArray(Of SyntaxReference).Empty
				End If
				Return empty
			End Get
		End Property

		Friend Overrides ReadOnly Property ExplicitDefaultConstantValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol)) As ConstantValue
			Get
				If (CObj(Me._lazyDefaultValue) = CObj(ConstantValue.Unset)) Then
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					If (CObj(Interlocked.CompareExchange(Of ConstantValue)(Me._lazyDefaultValue, Me.BindDefaultValue(inProgress, instance), ConstantValue.Unset)) = CObj(ConstantValue.Unset)) Then
						DirectCast(Me.ContainingModule, SourceModuleSymbol).AddDeclarationDiagnostics(instance)
					End If
					instance.Free()
				End If
				Return Me._lazyDefaultValue
			End Get
		End Property

		Public Overrides ReadOnly Property HasExplicitDefaultValue As Boolean
			Get
				Return CObj(Me(SymbolsInProgress(Of ParameterSymbol).Empty)) <> CObj(Nothing)
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerFilePath As Boolean
			Get
				Dim boundAttributesSource As SourceParameterSymbol = Me.BoundAttributesSource
				If (boundAttributesSource Is Nothing) Then
					boundAttributesSource = Me
				End If
				Dim earlyDecodedWellKnownAttributeData As ParameterEarlyWellKnownAttributeData = boundAttributesSource.GetEarlyDecodedWellKnownAttributeData()
				If (earlyDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return earlyDecodedWellKnownAttributeData.HasCallerFilePathAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerLineNumber As Boolean
			Get
				Dim boundAttributesSource As SourceParameterSymbol = Me.BoundAttributesSource
				If (boundAttributesSource Is Nothing) Then
					boundAttributesSource = Me
				End If
				Dim earlyDecodedWellKnownAttributeData As ParameterEarlyWellKnownAttributeData = boundAttributesSource.GetEarlyDecodedWellKnownAttributeData()
				If (earlyDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return earlyDecodedWellKnownAttributeData.HasCallerLineNumberAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerMemberName As Boolean
			Get
				Dim boundAttributesSource As SourceParameterSymbol = Me.BoundAttributesSource
				If (boundAttributesSource Is Nothing) Then
					boundAttributesSource = Me
				End If
				Dim earlyDecodedWellKnownAttributeData As ParameterEarlyWellKnownAttributeData = boundAttributesSource.GetEarlyDecodedWellKnownAttributeData()
				If (earlyDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return earlyDecodedWellKnownAttributeData.HasCallerMemberNameAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExplicitByRef As Boolean
			Get
				Return CInt((Me._flags And SourceParameterFlags.[ByRef])) <> 0
			End Get
		End Property

		Public Overrides ReadOnly Property IsOptional As Boolean
			Get
				Return CInt((Me._flags And SourceParameterFlags.[Optional])) <> 0
			End Get
		End Property

		Public Overrides ReadOnly Property IsParamArray As Boolean
			Get
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceComplexParameterSymbol::get_IsParamArray()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean get_IsParamArray()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à ..(MethodBody , & ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:ligne 345
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Friend ReadOnly Property SyntaxNode As ParameterSyntax
			Get
				If (Me._syntaxRef Is Nothing) Then
					Return Nothing
				End If
				Return DirectCast(Me._syntaxRef.GetSyntax(New CancellationToken()), ParameterSyntax)
			End Get
		End Property

		Private Sub New(ByVal container As Symbol, ByVal name As String, ByVal ordinal As Integer, ByVal type As TypeSymbol, ByVal location As Microsoft.CodeAnalysis.Location, ByVal syntaxRef As SyntaxReference, ByVal flags As SourceParameterFlags, ByVal defaultValueOpt As ConstantValue)
			MyBase.New(container, name, ordinal, type, location)
			Me._flags = flags
			Me._lazyDefaultValue = defaultValueOpt
			Me._syntaxRef = syntaxRef
		End Sub

		Private Function BindDefaultValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			Dim syntaxNode As ParameterSyntax = Me.SyntaxNode
			If (syntaxNode IsNot Nothing) Then
				Dim [default] As EqualsValueSyntax = syntaxNode.[Default]
				If ([default] IsNot Nothing) Then
					Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForParameterDefaultValue(DirectCast(Me.ContainingModule, SourceModuleSymbol), Me._syntaxRef.SyntaxTree, Me, syntaxNode)
					If (Not inProgress.Contains(Me)) Then
						Dim constantValue1 As Microsoft.CodeAnalysis.ConstantValue = Nothing
						Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = (New DefaultParametersInProgressBinder(inProgress.Add(Me), binder)).BindParameterDefaultValue(MyBase.Type, [default], diagnostics, constantValue1)
						If (constantValue1 IsNot Nothing) Then
							MyBase.VerifyParamDefaultValueMatchesAttributeIfAny(constantValue1, [default].Value, diagnostics)
						End If
						constantValue = constantValue1
					Else
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, [default].Value, ERRID.ERR_CircularEvaluation1, New [Object]() { Me })
						constantValue = Nothing
					End If
				Else
					constantValue = Nothing
				End If
			Else
				constantValue = Nothing
			End If
			Return constantValue
		End Function

		Friend Overrides Function ChangeOwner(ByVal newContainingSymbol As Symbol) As ParameterSymbol
			Dim name As String = MyBase.Name
			Dim ordinal As Integer = MyBase.Ordinal
			Dim type As TypeSymbol = MyBase.Type
			Dim locations As ImmutableArray(Of Microsoft.CodeAnalysis.Location) = MyBase.Locations
			Return New SourceComplexParameterSymbol(newContainingSymbol, name, ordinal, type, locations(0), Me._syntaxRef, Me._flags, Me._lazyDefaultValue)
		End Function

		Friend Shared Function Create(ByVal container As Symbol, ByVal name As String, ByVal ordinal As Integer, ByVal type As TypeSymbol, ByVal location As Microsoft.CodeAnalysis.Location, ByVal syntaxRef As SyntaxReference, ByVal flags As SourceParameterFlags, ByVal defaultValueOpt As ConstantValue) As ParameterSymbol
			Dim sourceComplexParameterSymbol As ParameterSymbol
			Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = TryCast(container, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol)
			If (CInt(flags) <> 0 OrElse defaultValueOpt IsNot Nothing OrElse syntaxRef IsNot Nothing OrElse sourceMethodSymbol IsNot Nothing AndAlso sourceMethodSymbol.IsPartial) Then
				sourceComplexParameterSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceComplexParameterSymbol(container, name, ordinal, type, location, syntaxRef, flags, defaultValueOpt)
			Else
				sourceComplexParameterSymbol = New SourceSimpleParameterSymbol(container, name, ordinal, type, location)
			End If
			Return sourceComplexParameterSymbol
		End Function

		Friend Shared Function CreateFromSyntax(ByVal container As Symbol, ByVal syntax As ParameterSyntax, ByVal name As String, ByVal flags As SourceParameterFlags, ByVal ordinal As Integer, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal checkModifier As Microsoft.CodeAnalysis.VisualBasic.Binder.CheckParameterModifierDelegate, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ParameterSymbol
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceComplexParameterSymbol::CreateFromSyntax(Microsoft.CodeAnalysis.VisualBasic.Symbol,Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax,System.String,Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceParameterFlags,System.Int32,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.VisualBasic.Binder/CheckParameterModifierDelegate,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol CreateFromSyntax(Microsoft.CodeAnalysis.VisualBasic.Symbol,Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax,System.String,Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceParameterFlags,System.Int32,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.VisualBasic.Binder/CheckParameterModifierDelegate,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
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

		Private Function GetAttributeDeclarations() As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			Dim oneOrMany As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			Dim attributeListSyntaxes As SyntaxList(Of AttributeListSyntax)
			Dim attributeDeclarationList As SyntaxList(Of AttributeListSyntax) = Me.AttributeDeclarationList
			Dim containingSymbol As SourceMemberMethodSymbol = TryCast(MyBase.ContainingSymbol, SourceMemberMethodSymbol)
			If (containingSymbol IsNot Nothing) Then
				Dim sourcePartialImplementation As SourceMemberMethodSymbol = containingSymbol.SourcePartialImplementation
				attributeListSyntaxes = If(sourcePartialImplementation Is Nothing, New SyntaxList(Of AttributeListSyntax)(), DirectCast(sourcePartialImplementation.Parameters(MyBase.Ordinal), SourceParameterSymbol).AttributeDeclarationList)
				Dim attributeListSyntaxes1 As SyntaxList(Of AttributeListSyntax) = New SyntaxList(Of AttributeListSyntax)()
				If (Not attributeDeclarationList.Equals(attributeListSyntaxes1)) Then
					attributeListSyntaxes1 = New SyntaxList(Of AttributeListSyntax)()
					oneOrMany = If(Not attributeListSyntaxes.Equals(attributeListSyntaxes1), Roslyn.Utilities.OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(ImmutableArray.Create(Of SyntaxList(Of AttributeListSyntax))(attributeDeclarationList, attributeListSyntaxes)), Roslyn.Utilities.OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(attributeDeclarationList))
				Else
					oneOrMany = Roslyn.Utilities.OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(attributeListSyntaxes)
				End If
			Else
				oneOrMany = Roslyn.Utilities.OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(attributeDeclarationList)
			End If
			Return oneOrMany
		End Function

		Friend Overrides Function GetAttributesBag() As CustomAttributesBag(Of VisualBasicAttributeData)
			If (Me._lazyCustomAttributesBag Is Nothing OrElse Not Me._lazyCustomAttributesBag.IsSealed) Then
				Dim boundAttributesSource As SourceParameterSymbol = Me.BoundAttributesSource
				If (boundAttributesSource Is Nothing) Then
					MyBase.LoadAndValidateAttributes(Me.GetAttributeDeclarations(), Me._lazyCustomAttributesBag, AttributeLocation.None)
				Else
					Dim attributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = boundAttributesSource.GetAttributesBag()
					Interlocked.CompareExchange(Of CustomAttributesBag(Of VisualBasicAttributeData))(Me._lazyCustomAttributesBag, attributesBag, Nothing)
				End If
			End If
			Return Me._lazyCustomAttributesBag
		End Function

		Friend Overrides Function GetDecodedWellKnownAttributeData() As CommonParameterWellKnownAttributeData
			Dim attributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me._lazyCustomAttributesBag
			If (attributesBag Is Nothing OrElse Not attributesBag.IsDecodedWellKnownAttributeDataComputed) Then
				attributesBag = Me.GetAttributesBag()
			End If
			Return DirectCast(attributesBag.DecodedWellKnownAttributeData, CommonParameterWellKnownAttributeData)
		End Function

		Friend Overrides Function GetEarlyDecodedWellKnownAttributeData() As ParameterEarlyWellKnownAttributeData
			Dim attributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me._lazyCustomAttributesBag
			If (attributesBag Is Nothing OrElse Not attributesBag.IsEarlyDecodedWellKnownAttributeDataComputed) Then
				attributesBag = Me.GetAttributesBag()
			End If
			Return DirectCast(attributesBag.EarlyDecodedWellKnownAttributeData, ParameterEarlyWellKnownAttributeData)
		End Function

		Friend NotOverridable Overrides Function IsDefinedInSourceTree(ByVal tree As SyntaxTree, ByVal definedWithinSpan As Nullable(Of TextSpan), Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Boolean
			Return Symbol.IsDefinedInSourceTree(Me.SyntaxNode, tree, definedWithinSpan, cancellationToken)
		End Function

		Friend Overrides Function WithTypeAndCustomModifiers(ByVal type As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal refCustomModifiers As ImmutableArray(Of CustomModifier)) As ParameterSymbol
			Dim sourceComplexParameterSymbolWithCustomModifier As ParameterSymbol
			If (Not customModifiers.IsEmpty OrElse Not refCustomModifiers.IsEmpty) Then
				sourceComplexParameterSymbolWithCustomModifier = New SourceComplexParameterSymbol.SourceComplexParameterSymbolWithCustomModifiers(MyBase.ContainingSymbol, MyBase.Name, MyBase.Ordinal, type, MyBase.Location, Me._syntaxRef, Me._flags, Me._lazyDefaultValue, customModifiers, refCustomModifiers)
			Else
				sourceComplexParameterSymbolWithCustomModifier = New SourceComplexParameterSymbol(MyBase.ContainingSymbol, MyBase.Name, MyBase.Ordinal, type, MyBase.Location, Me._syntaxRef, Me._flags, Me._lazyDefaultValue)
			End If
			Return sourceComplexParameterSymbolWithCustomModifier
		End Function

		Private Class SourceComplexParameterSymbolWithCustomModifiers
			Inherits SourceComplexParameterSymbol
			Private ReadOnly _customModifiers As ImmutableArray(Of CustomModifier)

			Private ReadOnly _refCustomModifiers As ImmutableArray(Of CustomModifier)

			Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
				Get
					Return Me._customModifiers
				End Get
			End Property

			Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
				Get
					Return Me._refCustomModifiers
				End Get
			End Property

			Public Sub New(ByVal container As Symbol, ByVal name As String, ByVal ordinal As Integer, ByVal type As TypeSymbol, ByVal location As Microsoft.CodeAnalysis.Location, ByVal syntaxRef As SyntaxReference, ByVal flags As SourceParameterFlags, ByVal defaultValueOpt As ConstantValue, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal refCustomModifiers As ImmutableArray(Of CustomModifier))
				MyBase.New(container, name, ordinal, type, location, syntaxRef, flags, defaultValueOpt)
				Me._customModifiers = customModifiers
				Me._refCustomModifiers = refCustomModifiers
			End Sub

			Friend Overrides Function WithTypeAndCustomModifiers(ByVal type As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal refCustomModifiers As ImmutableArray(Of CustomModifier)) As ParameterSymbol
				Throw ExceptionUtilities.Unreachable
			End Function
		End Class
	End Class
End Namespace