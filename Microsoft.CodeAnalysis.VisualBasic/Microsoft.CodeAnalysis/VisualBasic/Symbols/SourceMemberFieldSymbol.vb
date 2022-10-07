Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SourceMemberFieldSymbol
		Inherits SourceFieldSymbol
		Private _lazyType As TypeSymbol

		Private _lazyMeParameter As ParameterSymbol

		Friend NotOverridable Overrides ReadOnly Property DeclarationSyntax As VisualBasicSyntaxNode
			Get
				Return MyBase.Syntax.Parent.Parent
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property GetAttributeDeclarations As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			Get
				Return OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(DirectCast(MyBase.Syntax.Parent.Parent, FieldDeclarationSyntax).AttributeLists)
			End Get
		End Property

		Friend Overrides ReadOnly Property MeParameter As ParameterSymbol
			Get
				Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol
				If (Not Me.IsShared) Then
					If (Me._lazyMeParameter Is Nothing) Then
						Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)(Me._lazyMeParameter, New MeParameterSymbol(Me), Nothing)
					End If
					parameterSymbol = Me._lazyMeParameter
				Else
					parameterSymbol = Nothing
				End If
				Return parameterSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				If (Me._lazyType Is Nothing) Then
					Dim containingModule As SourceModuleSymbol = DirectCast(Me.ContainingModule, SourceModuleSymbol)
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.ComputeType(instance)
					containingModule.AtomicStoreReferenceAndDiagnostics(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)(Me._lazyType, typeSymbol, instance, Nothing)
					instance.Free()
				End If
				Return Me._lazyType
			End Get
		End Property

		Protected Sub New(ByVal container As SourceMemberContainerTypeSymbol, ByVal syntaxRef As SyntaxReference, ByVal name As String, ByVal memberFlags As SourceMemberFlags)
			MyBase.New(container, syntaxRef, name, memberFlags)
		End Sub

		Private Shared Function ComputeFieldType(ByVal modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal isConst As Boolean, ByVal isWithEvents As Boolean, ByVal ignoreTypeSyntaxDiagnostics As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim unknownResultType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim parent As VariableDeclaratorSyntax = DirectCast(modifiedIdentifierSyntax.Parent, VariableDeclaratorSyntax)
			Dim asClause As AsClauseSyntax = parent.AsClause
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
			Dim initializer As VisualBasicSyntaxNode = parent.Initializer
			If (asClause IsNot Nothing) Then
				If (asClause.Kind() <> SyntaxKind.AsNewClause OrElse DirectCast(asClause, AsNewClauseSyntax).NewExpression.Kind() <> SyntaxKind.AnonymousObjectCreationExpression) Then
					typeSymbol = binder.BindTypeSyntax(asClause.Type(), If(ignoreTypeSyntaxDiagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, diagnostics), False, False, False)
				End If
				If (asClause.Kind() = SyntaxKind.AsNewClause) Then
					initializer = asClause
				End If
			End If
			Dim flag As Boolean = [String].IsNullOrEmpty(modifiedIdentifierSyntax.Identifier.ValueText)
			If (asClause Is Nothing OrElse asClause.Kind() <> SyntaxKind.AsNewClause OrElse DirectCast(asClause, AsNewClauseSyntax).NewExpression.Kind() <> SyntaxKind.AnonymousObjectCreationExpression) Then
				Dim getErrorInfoERRWithEventsRequiresClass As Func(Of DiagnosticInfo) = Nothing
				If (Not flag AndAlso (Not isConst OrElse Not binder.OptionInfer)) Then
					If (isWithEvents) Then
						getErrorInfoERRWithEventsRequiresClass = ErrorFactory.GetErrorInfo_ERR_WithEventsRequiresClass
					ElseIf (binder.OptionStrict = OptionStrict.[On]) Then
						getErrorInfoERRWithEventsRequiresClass = ErrorFactory.GetErrorInfo_ERR_StrictDisallowImplicitObject
					ElseIf (binder.OptionStrict = OptionStrict.Custom) Then
						getErrorInfoERRWithEventsRequiresClass = ErrorFactory.GetErrorInfo_WRN_ObjectAssumedVar1_WRN_MissingAsClauseinVarDecl
					End If
				End If
				unknownResultType = binder.DecodeModifiedIdentifierType(modifiedIdentifierSyntax, typeSymbol, asClause, initializer, getErrorInfoERRWithEventsRequiresClass, diagnostics, Microsoft.CodeAnalysis.VisualBasic.Binder.ModifiedIdentifierTypeDecoderContext.FieldType)
			Else
				unknownResultType = ErrorTypeSymbol.UnknownResultType
			End If
			Return unknownResultType
		End Function

		Private Function ComputeType(ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim declaredType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.GetDeclaredType(diagBag)
			typeSymbol = If(Me.HasDeclaredType, declaredType, Me.GetInferredType(ConstantFieldsInProgress.Empty))
			Return typeSymbol
		End Function

		Friend Shared Function ComputeWithEventsFieldType(ByVal propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol, ByVal modifiedIdentifier As ModifiedIdentifierSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal ignoreTypeSyntaxDiagnostics As Boolean, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = SourceMemberFieldSymbol.ComputeFieldType(modifiedIdentifier, binder, diagnostics, False, True, ignoreTypeSyntaxDiagnostics)
			If (Not typeSymbol.IsErrorType()) Then
				Dim parent As VariableDeclaratorSyntax = DirectCast(modifiedIdentifier.Parent, VariableDeclaratorSyntax)
				Dim identifier As SyntaxToken = modifiedIdentifier.Identifier
				If (typeSymbol.IsArrayType()) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, modifiedIdentifier, ERRID.ERR_EventSourceIsArray)
				ElseIf (Not typeSymbol.IsClassOrInterfaceType() AndAlso (typeSymbol.Kind <> SymbolKind.TypeParameter OrElse Not typeSymbol.IsReferenceType) AndAlso parent.AsClause IsNot Nothing AndAlso parent.AsClause.Type() IsNot Nothing) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, identifier, ERRID.ERR_WithEventsAsStruct)
				End If
				If (parent.AsClause IsNot Nothing) Then
					Dim asClauseLocation As SyntaxNodeOrToken = SourceSymbolHelpers.GetAsClauseLocation(identifier, parent.AsClause)
					AccessCheck.VerifyAccessExposureForMemberType(propertySymbol, asClauseLocation, typeSymbol, diagnostics, False)
				End If
			End If
			Return typeSymbol
		End Function

		Friend Shared Sub Create(ByVal container As SourceMemberContainerTypeSymbol, ByVal syntax As FieldDeclarationSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal members As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByRef staticInitializers As ArrayBuilder(Of FieldOrPropertyInitializer), ByRef instanceInitializers As ArrayBuilder(Of FieldOrPropertyInitializer), ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberFieldSymbol::Create(Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol/MembersAndInitializersBuilder,Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer>&,Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer>&,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void Create(Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol/MembersAndInitializersBuilder,Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder<Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer>&,Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder<Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldOrPropertyInitializer>&,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
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

		Private Function GetDeclaredType(ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim syntax As ModifiedIdentifierSyntax = DirectCast(MyBase.Syntax, ModifiedIdentifierSyntax)
			Dim parent As VariableDeclaratorSyntax = DirectCast(syntax.Parent, VariableDeclaratorSyntax)
			Dim locationSpecificBinder As Binder = BinderBuilder.CreateBinderForType(DirectCast(Me.ContainingModule, SourceModuleSymbol), MyBase.SyntaxTree, MyBase.ContainingType)
			locationSpecificBinder = New Microsoft.CodeAnalysis.VisualBasic.LocationSpecificBinder(BindingLocation.FieldType, Me, locationSpecificBinder)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = SourceMemberFieldSymbol.ComputeFieldType(syntax, locationSpecificBinder, diagBag, Me.IsConst, False, (Me.m_memberFlags And SourceMemberFlags.FirstFieldDeclarationOfType) = SourceMemberFlags.None)
			If (Not typeSymbol.IsErrorType()) Then
				If (Not Me.IsConst) Then
					Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
					If (typeSymbol.IsRestrictedTypeOrArrayType(typeSymbol1)) Then
						Binder.ReportDiagnostic(diagBag, parent.AsClause.Type(), ERRID.ERR_RestrictedType1, New [Object]() { typeSymbol1 })
					End If
				ElseIf (Not typeSymbol.IsValidTypeForConstField()) Then
					If (Not typeSymbol.IsArrayType()) Then
						Binder.ReportDiagnostic(diagBag, parent.AsClause.Type(), ERRID.ERR_ConstAsNonConstant)
					Else
						Binder.ReportDiagnostic(diagBag, syntax.Identifier, ERRID.ERR_ConstAsNonConstant)
					End If
				ElseIf (parent.Initializer Is Nothing) Then
					Binder.ReportDiagnostic(diagBag, syntax, ERRID.ERR_ConstantWithNoValue)
				End If
				If (Me.HasDeclaredType) Then
					Dim asClauseLocation As SyntaxNodeOrToken = SourceSymbolHelpers.GetAsClauseLocation(syntax.Identifier, parent.AsClause)
					AccessCheck.VerifyAccessExposureForMemberType(Me, asClauseLocation, typeSymbol, diagBag, False)
				End If
			End If
			Return typeSymbol
		End Function

		Protected Overridable Function GetInferredConstantType(ByVal inProgress As ConstantFieldsInProgress) As TypeSymbol
			Return Nothing
		End Function

		Friend Overrides Function GetInferredType(ByVal inProgress As ConstantFieldsInProgress) As TypeSymbol
			Dim type As TypeSymbol
			If (Not Me.HasDeclaredType) Then
				Me.GetConstantValue(inProgress)
				Dim inferredConstantType As TypeSymbol = Me.GetInferredConstantType(inProgress)
				If (inferredConstantType Is Nothing) Then
					inferredConstantType = Me.ContainingAssembly.GetSpecialType(SpecialType.System_Object)
				ElseIf (inferredConstantType.IsValidTypeForConstField()) Then
					inferredConstantType = inferredConstantType.GetEnumUnderlyingTypeOrSelf()
				Else
					inferredConstantType = Me.ContainingAssembly.GetSpecialType(SpecialType.System_Object)
				End If
				type = inferredConstantType
			Else
				type = Me.Type
			End If
			Return type
		End Function

		Friend NotOverridable Overrides Function IsDefinedInSourceTree(ByVal tree As Microsoft.CodeAnalysis.SyntaxTree, ByVal definedWithinSpan As Nullable(Of TextSpan), Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Boolean
			Return Symbol.IsDefinedInSourceTree(Me.DeclarationSyntax, tree, definedWithinSpan, cancellationToken)
		End Function

		Private NotInheritable Class SourceConstFieldSymbolWithInitializer
			Inherits SourceMemberFieldSymbol.SourceFieldSymbolWithInitializer
			Private _constantTuple As EvaluatedConstant

			Public Sub New(ByVal container As SourceMemberContainerTypeSymbol, ByVal syntaxRef As SyntaxReference, ByVal name As String, ByVal memberFlags As SourceMemberFlags, ByVal equalsValueOrAsNewInit As SyntaxReference)
				MyBase.New(container, syntaxRef, name, memberFlags, equalsValueOrAsNewInit)
			End Sub

			Friend Overrides Function GetConstantValue(ByVal inProgress As ConstantFieldsInProgress) As Microsoft.CodeAnalysis.ConstantValue
				Return MyBase.GetConstantValueImpl(inProgress)
			End Function

			Protected Overrides Function GetInferredConstantType(ByVal inProgress As ConstantFieldsInProgress) As TypeSymbol
				Dim errorTypeSymbol As TypeSymbol
				MyBase.GetConstantValueImpl(inProgress)
				Dim lazyConstantTuple As EvaluatedConstant = Me.GetLazyConstantTuple()
				If (lazyConstantTuple Is Nothing) Then
					errorTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.ErrorTypeSymbol()
				Else
					errorTypeSymbol = lazyConstantTuple.Type
				End If
				Return errorTypeSymbol
			End Function

			Protected Overrides Function GetLazyConstantTuple() As EvaluatedConstant
				Return Me._constantTuple
			End Function

			Protected Overrides Function MakeConstantTuple(ByVal dependencies As ConstantFieldsInProgress.Dependencies, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As EvaluatedConstant
				Return ConstantValueUtils.EvaluateFieldConstant(Me, Me._equalsValueOrAsNewInit, dependencies, diagnostics)
			End Function

			Protected Overrides Sub SetLazyConstantTuple(ByVal constantTuple As EvaluatedConstant, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
				DirectCast(Me.ContainingModule, SourceModuleSymbol).AtomicStoreReferenceAndDiagnostics(Of EvaluatedConstant)(Me._constantTuple, constantTuple, diagnostics, Nothing)
			End Sub
		End Class

		Private NotInheritable Class SourceFieldSymbolSiblingInitializer
			Inherits SourceMemberFieldSymbol
			Private ReadOnly _sibling As SourceMemberFieldSymbol

			Friend Overrides ReadOnly Property EqualsValueOrAsNewInitOpt As VisualBasicSyntaxNode
				Get
					Return Me._sibling.EqualsValueOrAsNewInitOpt
				End Get
			End Property

			Public Sub New(ByVal container As SourceMemberContainerTypeSymbol, ByVal syntaxRef As SyntaxReference, ByVal name As String, ByVal memberFlags As SourceMemberFlags, ByVal sibling As SourceMemberFieldSymbol)
				MyBase.New(container, syntaxRef, name, memberFlags)
				Me._sibling = sibling
			End Sub

			Friend Overrides Function GetConstantValue(ByVal inProgress As ConstantFieldsInProgress) As Microsoft.CodeAnalysis.ConstantValue
				Return Me._sibling.GetConstantValue(inProgress)
			End Function

			Protected Overrides Function GetInferredConstantType(ByVal inProgress As ConstantFieldsInProgress) As TypeSymbol
				Return Me._sibling.GetInferredConstantType(inProgress)
			End Function
		End Class

		Private Class SourceFieldSymbolWithInitializer
			Inherits SourceMemberFieldSymbol
			Protected ReadOnly _equalsValueOrAsNewInit As SyntaxReference

			Friend NotOverridable Overrides ReadOnly Property EqualsValueOrAsNewInitOpt As VisualBasicSyntaxNode
				Get
					Return Me._equalsValueOrAsNewInit.GetVisualBasicSyntax(New CancellationToken())
				End Get
			End Property

			Public Sub New(ByVal container As SourceMemberContainerTypeSymbol, ByVal syntaxRef As SyntaxReference, ByVal name As String, ByVal memberFlags As SourceMemberFlags, ByVal equalsValueOrAsNewInit As SyntaxReference)
				MyBase.New(container, syntaxRef, name, memberFlags)
				Me._equalsValueOrAsNewInit = equalsValueOrAsNewInit
			End Sub
		End Class
	End Class
End Namespace