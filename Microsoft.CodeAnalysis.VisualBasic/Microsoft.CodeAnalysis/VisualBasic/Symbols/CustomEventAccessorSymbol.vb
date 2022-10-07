Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Linq

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class CustomEventAccessorSymbol
		Inherits SourceNonPropertyAccessorMethodSymbol
		Private ReadOnly _event As SourceEventSymbol

		Private ReadOnly _name As String

		Private _lazyExplicitImplementations As ImmutableArray(Of MethodSymbol)

		Private ReadOnly Shared s_checkAddRemoveParameterModifierCallback As Binder.CheckParameterModifierDelegate

		Private ReadOnly Shared s_checkRaiseParameterModifierCallback As Binder.CheckParameterModifierDelegate

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Me._event
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Dim accessibility As Microsoft.CodeAnalysis.Accessibility
				accessibility = If(Me.MethodKind <> Microsoft.CodeAnalysis.MethodKind.EventRaise, Me._event.DeclaredAccessibility, Microsoft.CodeAnalysis.Accessibility.[Private])
				Return accessibility
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				If (Me._lazyExplicitImplementations.IsDefault) Then
					Dim accessorImplementations As ImmutableArray(Of MethodSymbol) = Me._event.GetAccessorImplementations(Me.MethodKind)
					Dim methodSymbols As ImmutableArray(Of MethodSymbol) = New ImmutableArray(Of MethodSymbol)()
					ImmutableInterlocked.InterlockedCompareExchange(Of MethodSymbol)(Me._lazyExplicitImplementations, accessorImplementations, methodSymbols)
				End If
				Return Me._lazyExplicitImplementations
			End Get
		End Property

		Public Overrides ReadOnly Property IsExtensionMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				If (Me.MethodKind <> Microsoft.CodeAnalysis.MethodKind.EventAdd) Then
					Return True
				End If
				Return Not Me._event.IsWindowsRuntimeEvent
			End Get
		End Property

		Friend Overrides ReadOnly Property MayBeReducibleExtensionMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Binder.GetAccessorName(Me._event.MetadataName, Me.MethodKind, False)
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Public Overrides ReadOnly Property OverriddenMethod As MethodSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property ShadowsExplicitly As Boolean
			Get
				Return Me._event.ShadowsExplicitly
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return ImmutableArray(Of TypeParameterSymbol).Empty
			End Get
		End Property

		Shared Sub New()
			CustomEventAccessorSymbol.s_checkAddRemoveParameterModifierCallback = New Binder.CheckParameterModifierDelegate(AddressOf CustomEventAccessorSymbol.CheckAddRemoveParameterModifier)
			CustomEventAccessorSymbol.s_checkRaiseParameterModifierCallback = New Binder.CheckParameterModifierDelegate(AddressOf CustomEventAccessorSymbol.CheckEventMethodParameterModifier)
		End Sub

		Friend Sub New(ByVal container As SourceMemberContainerTypeSymbol, ByVal [event] As SourceEventSymbol, ByVal name As String, ByVal flags As SourceMemberFlags, ByVal syntaxRef As SyntaxReference, ByVal location As Microsoft.CodeAnalysis.Location)
			MyBase.New(container, flags, syntaxRef, ImmutableArray.Create(Of Microsoft.CodeAnalysis.Location)(location))
			Me._event = [event]
			Me._name = name
		End Sub

		Private Function BindParameters(ByVal location As Microsoft.CodeAnalysis.Location, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal parameterListOpt As ParameterListSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of ParameterSymbol)
			Dim parameterSyntaxes As SeparatedSyntaxList(Of ParameterSyntax) = If(parameterListOpt Is Nothing, New SeparatedSyntaxList(Of ParameterSyntax)(), parameterListOpt.Parameters)
			Dim instance As ArrayBuilder(Of ParameterSymbol) = ArrayBuilder(Of ParameterSymbol).GetInstance(parameterSyntaxes.Count)
			binder.DecodeParameterList(Me, False, SourceMemberFlags.None, parameterSyntaxes, instance, If(Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.EventRaise, CustomEventAccessorSymbol.s_checkRaiseParameterModifierCallback, CustomEventAccessorSymbol.s_checkAddRemoveParameterModifierCallback), diagnostics)
			Dim immutableAndFree As ImmutableArray(Of ParameterSymbol) = instance.ToImmutableAndFree()
			If (Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.EventRaise) Then
				Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(Me._event.Type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (type IsNot Nothing AndAlso Not type.IsErrorType()) Then
					Dim delegateInvokeMethod As MethodSymbol = type.DelegateInvokeMethod
					If (delegateInvokeMethod IsNot Nothing AndAlso delegateInvokeMethod.IsSub) Then
						Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = binder.GetNewCompoundUseSiteInfo(diagnostics)
						Dim methodConversionKind As Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind = Conversions.ClassifyMethodConversionForEventRaise(delegateInvokeMethod, immutableAndFree, newCompoundUseSiteInfo)
						If (Not diagnostics.Add(location, newCompoundUseSiteInfo) AndAlso (Not Conversions.IsDelegateRelaxationSupportedFor(methodConversionKind) OrElse binder.OptionStrict = OptionStrict.[On] AndAlso Conversions.IsNarrowingMethodConversion(methodConversionKind, False))) Then
							diagnostics.Add(ERRID.ERR_RaiseEventShapeMismatch1, location, New [Object]() { type })
						End If
					End If
				End If
			ElseIf (immutableAndFree.Length = 1) Then
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me._event.Type
				Dim type1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = immutableAndFree(0).Type
				If (Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.EventAdd) Then
					If (Not typeSymbol.IsErrorType() AndAlso Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(typeSymbol, type1, TypeCompareKind.ConsiderEverything)) Then
						diagnostics.Add(If(Me._event.IsWindowsRuntimeEvent, ERRID.ERR_AddParamWrongForWinRT, ERRID.ERR_AddRemoveParamNotEventType), location)
					End If
				ElseIf (System.Linq.ImmutableArrayExtensions.Any(Of EventSymbol)(Me._event.ExplicitInterfaceImplementations)) Then
					Dim wellKnownType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = binder.Compilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken)
					Dim item As EventSymbol = Me._event.ExplicitInterfaceImplementations(0)
					If (Not wellKnownType.IsErrorType() AndAlso item.IsWindowsRuntimeEvent <> Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(type1, wellKnownType, TypeCompareKind.ConsiderEverything)) Then
						diagnostics.Add(ERRID.ERR_EventImplRemoveHandlerParamWrong, location, New [Object]() { Me._event.Name, item.Name, item.ContainingType })
					End If
				ElseIf (Me._event.IsWindowsRuntimeEvent) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = binder.Compilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken)
					If (Not namedTypeSymbol.IsErrorType() AndAlso Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(type1, namedTypeSymbol, TypeCompareKind.ConsiderEverything)) Then
						diagnostics.Add(ERRID.ERR_RemoveParamWrongForWinRT, location)
					End If
				ElseIf (Not typeSymbol.IsErrorType() AndAlso Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(typeSymbol, type1, TypeCompareKind.ConsiderEverything)) Then
					diagnostics.Add(ERRID.ERR_AddRemoveParamNotEventType, location)
				End If
			Else
				diagnostics.Add(ERRID.ERR_EventAddRemoveHasOnlyOneParam, location)
			End If
			Return immutableAndFree
		End Function

		Private Shared Function CheckAddRemoveParameterModifier(ByVal container As Symbol, ByVal token As SyntaxToken, ByVal flag As SourceParameterFlags, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As SourceParameterFlags
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceParameterFlags Microsoft.CodeAnalysis.VisualBasic.Symbols.CustomEventAccessorSymbol::CheckAddRemoveParameterModifier(Microsoft.CodeAnalysis.VisualBasic.Symbol,Microsoft.CodeAnalysis.SyntaxToken,Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceParameterFlags,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceParameterFlags CheckAddRemoveParameterModifier(Microsoft.CodeAnalysis.VisualBasic.Symbol,Microsoft.CodeAnalysis.SyntaxToken,Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceParameterFlags,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
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

		Private Shared Function CheckEventMethodParameterModifier(ByVal container As Symbol, ByVal token As SyntaxToken, ByVal flag As SourceParameterFlags, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As SourceParameterFlags
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceParameterFlags Microsoft.CodeAnalysis.VisualBasic.Symbols.CustomEventAccessorSymbol::CheckEventMethodParameterModifier(Microsoft.CodeAnalysis.VisualBasic.Symbol,Microsoft.CodeAnalysis.SyntaxToken,Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceParameterFlags,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceParameterFlags CheckEventMethodParameterModifier(Microsoft.CodeAnalysis.VisualBasic.Symbol,Microsoft.CodeAnalysis.SyntaxToken,Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceParameterFlags,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
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

		Protected Overrides Function GetAttributeDeclarations() As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			Return OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(MyBase.AttributeDeclarationSyntaxList)
		End Function

		Protected Overrides Function GetParameters(ByVal sourceModule As SourceModuleSymbol, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of ParameterSymbol)
			Dim containingType As SourceMemberContainerTypeSymbol = DirectCast(Me.ContainingType, SourceMemberContainerTypeSymbol)
			Dim locationSpecificBinder As Binder = BinderBuilder.CreateBinderForType(sourceModule, MyBase.SyntaxTree, containingType)
			locationSpecificBinder = New Microsoft.CodeAnalysis.VisualBasic.LocationSpecificBinder(BindingLocation.EventAccessorSignature, Me, locationSpecificBinder)
			Return Me.BindParameters(System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Location)(Me.Locations), locationSpecificBinder, MyBase.BlockSyntax.BlockStatement.ParameterList, diagBag)
		End Function

		Protected Overrides Function GetReturnTypeAttributeDeclarations() As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			Return New OneOrMany(Of SyntaxList(Of AttributeListSyntax))()
		End Function
	End Class
End Namespace