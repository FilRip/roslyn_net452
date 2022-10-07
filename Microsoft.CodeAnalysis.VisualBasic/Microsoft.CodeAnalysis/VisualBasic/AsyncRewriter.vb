Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Collections
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class AsyncRewriter
		Inherits StateMachineRewriter(Of AsyncRewriter.CapturedSymbolOrExpression)
		Private ReadOnly _binder As Binder

		Private ReadOnly _lookupOptions As LookupOptions

		Private ReadOnly _asyncMethodKind As AsyncRewriter.AsyncMethodKind

		Private ReadOnly _builderType As NamedTypeSymbol

		Private ReadOnly _resultType As TypeSymbol

		Private _builderField As FieldSymbol

		Private _lastExpressionCaptureNumber As Integer

		Protected Overrides ReadOnly Property PreserveInitialParameterValues As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeMap As TypeSubstitution
			Get
				Return Me.StateMachineType.TypeSubstitution
			End Get
		End Property

		Public Sub New(ByVal body As BoundStatement, ByVal method As MethodSymbol, ByVal stateMachineType As AsyncStateMachine, ByVal slotAllocatorOpt As VariableSlotAllocator, ByVal asyncKind As AsyncRewriter.AsyncMethodKind, ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			MyBase.New(body, method, stateMachineType, slotAllocatorOpt, compilationState, diagnostics)
			Me._binder = AsyncRewriter.CreateMethodBinder(method)
			Me._lookupOptions = LookupOptions.NoBaseClassLookup Or LookupOptions.AllMethodsOfAnyArity Or LookupOptions.IgnoreExtensionMethods
			If (compilationState.ModuleBuilderOpt.IgnoreAccessibility) Then
				Me._binder = New IgnoreAccessibilityBinder(Me._binder)
				Me._lookupOptions = Me._lookupOptions Or LookupOptions.IgnoreAccessibility
			End If
			Me._asyncMethodKind = asyncKind
			Select Case Me._asyncMethodKind
				Case AsyncRewriter.AsyncMethodKind.[Sub]
					Me._resultType = Me.F.SpecialType(SpecialType.System_Void)
					Me._builderType = Me.F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncVoidMethodBuilder)
					Return
				Case AsyncRewriter.AsyncMethodKind.TaskFunction
					Me._resultType = Me.F.SpecialType(SpecialType.System_Void)
					Me._builderType = Me.F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncTaskMethodBuilder)
					Return
				Case AsyncRewriter.AsyncMethodKind.GenericTaskFunction
					Me._resultType = DirectCast(Me.Method.ReturnType, NamedTypeSymbol).TypeArgumentsNoUseSiteDiagnostics.[Single]().InternalSubstituteTypeParameters(Me.TypeMap).Type
					Me._builderType = Me.F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T).Construct(New TypeSymbol() { Me._resultType })
					Return
			End Select
			Throw ExceptionUtilities.UnexpectedValue(Me._asyncMethodKind)
		End Sub

		Private Function CaptureExpression(ByVal typeMap As TypeSubstitution, ByVal expression As BoundExpression, ByVal initializers As Dictionary(Of LocalSymbol, BoundExpression)) As AsyncRewriter.CapturedSymbolOrExpression
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.AsyncRewriter/CapturedSymbolOrExpression Microsoft.CodeAnalysis.VisualBasic.AsyncRewriter::CaptureExpression(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution,Microsoft.CodeAnalysis.VisualBasic.BoundExpression,System.Collections.Generic.Dictionary`2<Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol,Microsoft.CodeAnalysis.VisualBasic.BoundExpression>)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.AsyncRewriter/CapturedSymbolOrExpression CaptureExpression(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution,Microsoft.CodeAnalysis.VisualBasic.BoundExpression,System.Collections.Generic.Dictionary<Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol,Microsoft.CodeAnalysis.VisualBasic.BoundExpression>)
			' 
			' The unary opperator AddressReference is not supported in VisualBasic
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\DetermineNotSupportedVBCodeStep.cs:ligne 22
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Protected Overrides Function CreateByRefLocalCapture(ByVal typeMap As TypeSubstitution, ByVal local As LocalSymbol, ByVal initializers As Dictionary(Of LocalSymbol, BoundExpression)) As AsyncRewriter.CapturedSymbolOrExpression
			Return Me.CaptureExpression(typeMap, initializers(local), initializers)
		End Function

		Protected Overrides Function CreateByValLocalCapture(ByVal field As FieldSymbol, ByVal local As LocalSymbol) As AsyncRewriter.CapturedSymbolOrExpression
			Return New AsyncRewriter.CapturedLocalSymbol(field, local)
		End Function

		Private Shared Function CreateMethodBinder(ByVal method As MethodSymbol) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim sourceTree As Microsoft.CodeAnalysis.SyntaxTree
			Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = TryCast(method, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol)
			If (sourceMethodSymbol Is Nothing) Then
				Dim containingType As NamedTypeSymbol = method.ContainingType
				While containingType IsNot Nothing
					Dim location As Microsoft.CodeAnalysis.Location = System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Microsoft.CodeAnalysis.Location)(containingType.Locations)
					If (location IsNot Nothing) Then
						sourceTree = location.SourceTree
					Else
						sourceTree = Nothing
					End If
					Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = sourceTree
					If (syntaxTree Is Nothing) Then
						containingType = containingType.ContainingType
					Else
						binder = BinderBuilder.CreateBinderForType(DirectCast(containingType.ContainingModule, SourceModuleSymbol), syntaxTree, containingType)
						Return binder
					End If
				End While
				Throw ExceptionUtilities.Unreachable
			Else
				Dim containingModule As SourceModuleSymbol = DirectCast(sourceMethodSymbol.ContainingModule, SourceModuleSymbol)
				Dim locations As ImmutableArray(Of Microsoft.CodeAnalysis.Location) = sourceMethodSymbol.ContainingType.Locations
				binder = BinderBuilder.CreateBinderForMethodBody(containingModule, locations(0).PossiblyEmbeddedOrMySourceTree(), sourceMethodSymbol)
			End If
			Return binder
		End Function

		Protected Overrides Function CreateParameterCapture(ByVal field As FieldSymbol, ByVal parameter As ParameterSymbol) As AsyncRewriter.CapturedSymbolOrExpression
			Return New AsyncRewriter.CapturedParameterSymbol(field)
		End Function

		Friend Overrides Function EnsureAllSymbolsAndSignature() As Boolean
			Dim flag As Boolean
			If (Not MyBase.EnsureAllSymbolsAndSignature()) Then
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance(True, Me.Diagnostics.AccumulatesDependencies)
				MyBase.EnsureSpecialType(SpecialType.System_Object, instance)
				MyBase.EnsureSpecialType(SpecialType.System_Void, instance)
				MyBase.EnsureSpecialType(SpecialType.System_ValueType, instance)
				MyBase.EnsureWellKnownType(WellKnownType.System_Runtime_CompilerServices_IAsyncStateMachine, instance)
				MyBase.EnsureWellKnownMember(WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_MoveNext, instance)
				MyBase.EnsureWellKnownMember(WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_SetStateMachine, instance)
				Select Case Me._asyncMethodKind
					Case AsyncRewriter.AsyncMethodKind.[Sub]
						MyBase.EnsureWellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncVoidMethodBuilder, instance)
						Exit Select
					Case AsyncRewriter.AsyncMethodKind.TaskFunction
						MyBase.EnsureWellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncTaskMethodBuilder, instance)
						Exit Select
					Case AsyncRewriter.AsyncMethodKind.GenericTaskFunction
						MyBase.EnsureWellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T, instance)
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(Me._asyncMethodKind)
				End Select
				Dim flag1 As Boolean = instance.HasAnyErrors()
				If (flag1) Then
					Me.Diagnostics.AddRange(instance, False)
				End If
				instance.Free()
				flag = flag1
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Function FindMethodAndReturnMethodGroup(ByVal receiver As BoundExpression, ByVal type As TypeSymbol, ByVal methodName As String, ByVal typeArgs As ImmutableArray(Of TypeSymbol)) As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup
			Dim boundMethodGroup As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup = Nothing
			Dim instance As LookupResult = LookupResult.GetInstance()
			Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me._binder.GetNewCompoundUseSiteInfo(Me.Diagnostics)
			Me._binder.LookupMember(instance, type, methodName, 0, Me._lookupOptions, newCompoundUseSiteInfo)
			Me.Diagnostics.Add(Me.F.Syntax, newCompoundUseSiteInfo)
			If (instance.IsGood) Then
				Dim item As Symbol = instance.Symbols(0)
				If (instance.Symbols(0).Kind = SymbolKind.Method) Then
					boundMethodGroup = New Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup(Me.F.Syntax, Me.F.TypeArguments(typeArgs), instance.Symbols.ToDowncastedImmutable(Of MethodSymbol)(), instance.Kind, receiver, QualificationKind.QualifiedViaValue, False)
				End If
			End If
			If (boundMethodGroup Is Nothing) Then
				Me.Diagnostics.Add(If(instance.HasDiagnostic, instance.Diagnostic, ErrorFactory.ErrorInfo(ERRID.ERR_NameNotMember2, New [Object]() { methodName, type })), Me.F.Syntax.GetLocation())
			End If
			instance.Free()
			Return boundMethodGroup
		End Function

		Private Function FindPropertyAndReturnPropertyGroup(ByVal receiver As BoundExpression, ByVal type As TypeSymbol, ByVal propertyName As String) As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyGroup
			Dim boundPropertyGroup As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyGroup = Nothing
			Dim instance As LookupResult = LookupResult.GetInstance()
			Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me._binder.GetNewCompoundUseSiteInfo(Me.Diagnostics)
			Me._binder.LookupMember(instance, type, propertyName, 0, Me._lookupOptions, newCompoundUseSiteInfo)
			Me.Diagnostics.Add(Me.F.Syntax, newCompoundUseSiteInfo)
			If (instance.IsGood) Then
				Dim item As Symbol = instance.Symbols(0)
				If (instance.Symbols(0).Kind = SymbolKind.[Property]) Then
					boundPropertyGroup = New Microsoft.CodeAnalysis.VisualBasic.BoundPropertyGroup(Me.F.Syntax, instance.Symbols.ToDowncastedImmutable(Of PropertySymbol)(), instance.Kind, receiver, QualificationKind.QualifiedViaValue, False)
				End If
			End If
			If (boundPropertyGroup Is Nothing) Then
				Me.Diagnostics.Add(If(instance.HasDiagnostic, instance.Diagnostic, ErrorFactory.ErrorInfo(ERRID.ERR_NameNotMember2, New [Object]() { propertyName, type })), Me.F.Syntax.GetLocation())
			End If
			instance.Free()
			Return boundPropertyGroup
		End Function

		Protected Overrides Sub GenerateControlFields()
			Me.StateField = Me.F.StateMachineField(Me.F.SpecialType(SpecialType.System_Int32), Me.Method, GeneratedNames.MakeStateMachineStateFieldName(), Accessibility.[Public])
			Me._builderField = Me.F.StateMachineField(Me._builderType, Me.Method, GeneratedNames.MakeStateMachineBuilderFieldName(), Accessibility.[Public])
		End Sub

		Private Function GenerateMethodCall(ByVal receiver As BoundExpression, ByVal type As TypeSymbol, ByVal methodName As String, ByVal ParamArray arguments As BoundExpression()) As BoundExpression
			Return Me.GenerateMethodCall(receiver, type, methodName, ImmutableArray(Of TypeSymbol).Empty, arguments)
		End Function

		Private Function GenerateMethodCall(ByVal receiver As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal type As TypeSymbol, ByVal methodName As String, ByVal typeArgs As ImmutableArray(Of TypeSymbol), ByVal ParamArray arguments As Microsoft.CodeAnalysis.VisualBasic.BoundExpression()) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim hasErrors As Func(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression, Boolean)
			Dim boundMethodGroup As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup = Me.FindMethodAndReturnMethodGroup(receiver, type, methodName, typeArgs)
			If (boundMethodGroup IsNot Nothing) Then
				Dim boundExpressionArray As Microsoft.CodeAnalysis.VisualBasic.BoundExpression() = arguments
				If (AsyncRewriter._Closure$__.$I40-0 Is Nothing) Then
					hasErrors = Function(a As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) a.HasErrors
					AsyncRewriter._Closure$__.$I40-0 = hasErrors
				Else
					hasErrors = AsyncRewriter._Closure$__.$I40-0
				End If
				If (DirectCast(boundExpressionArray, IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)).Any(hasErrors) OrElse receiver IsNot Nothing AndAlso receiver.HasErrors) Then
					boundExpression = Me.F.BadExpression(arguments)
					Return boundExpression
				End If
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me._binder
				Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = Me.F.Syntax
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = Me.F.Syntax
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(arguments)
				Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
				boundExpression = binder.BindInvocationExpression(syntax, syntaxNode, TypeCharacter.None, boundMethodGroup, boundExpressions, strs, Me.Diagnostics, Nothing, False, False, False, Nothing, False)
				Return boundExpression
			End If
			boundExpression = Me.F.BadExpression(arguments)
			Return boundExpression
		End Function

		Protected Overrides Sub GenerateMethodImplementations()
			Me.GenerateMoveNext(MyBase.OpenMoveNextMethodImplementation(WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_MoveNext, Accessibility.Internal))
			MyBase.OpenMethodImplementation(WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_SetStateMachine, "System.Runtime.CompilerServices.IAsyncStateMachine.SetStateMachine", Accessibility.[Private], False, Nothing)
			If (Me.F.CurrentType.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Class]) Then
				Dim f As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = Me.F
				Dim boundStatementArray(1) As BoundStatement
				Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = Me.F
				Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = Me.F.Field(Me.F.[Me](), Me._builderField, False)
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._builderType
				Dim boundExpressionArray(0) As BoundExpression
				Dim f1 As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = Me.F
				Dim parameters As ImmutableArray(Of ParameterSymbol) = Me.F.CurrentMethod.Parameters
				boundExpressionArray(0) = f1.Parameter(parameters(0))
				boundStatementArray(0) = syntheticBoundNodeFactory.ExpressionStatement(Me.GenerateMethodCall(boundFieldAccess, namedTypeSymbol, "SetStateMachine", boundExpressionArray))
				boundStatementArray(1) = Me.F.[Return](Nothing)
				MyBase.CloseMethod(f.Block(boundStatementArray))
			Else
				Me.F.CloseMethod(Me.F.[Return](Nothing))
			End If
			If (Me.StateMachineType.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Class]) Then
				Me.F.CurrentMethod = Me.StateMachineType.Constructor
				Me.F.CloseMethod(Me.F.Block(ImmutableArray.Create(Of BoundStatement)(Me.F.BaseInitialization(New BoundExpression(-1) {}), Me.F.[Return](Nothing))))
			End If
		End Sub

		Private Sub GenerateMoveNext(ByVal moveNextMethod As MethodSymbol)
			Dim asyncMethodToClassRewriter As AsyncRewriter.AsyncMethodToClassRewriter = New AsyncRewriter.AsyncMethodToClassRewriter(Me.Method, Me.F, Me.StateField, Me._builderField, Me.hoistedVariables, Me.nonReusableLocalProxies, Me.SynthesizedLocalOrdinals, Me.SlotAllocatorOpt, Me.nextFreeHoistedLocalSlot, Me, Me.Diagnostics)
			asyncMethodToClassRewriter.GenerateMoveNext(Me.Body, moveNextMethod)
		End Sub

		Private Function GeneratePropertyGet(ByVal receiver As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal type As TypeSymbol, ByVal propertyName As String) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundPropertyGroup As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyGroup = Me.FindPropertyAndReturnPropertyGroup(receiver, type, propertyName)
			If (boundPropertyGroup Is Nothing OrElse receiver IsNot Nothing AndAlso receiver.HasErrors) Then
				boundExpression = Me.F.BadExpression(New Microsoft.CodeAnalysis.VisualBasic.BoundExpression(-1) {})
			Else
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me._binder
				Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = Me.F.Syntax
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = Me.F.Syntax
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)()
				Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = binder.BindInvocationExpression(syntax, syntaxNode, TypeCharacter.None, boundPropertyGroup, boundExpressions, strs, Me.Diagnostics, Nothing, False, False, False, Nothing, False)
				If (boundExpression1.Kind = BoundKind.PropertyAccess) Then
					boundExpression1 = DirectCast(boundExpression1, BoundPropertyAccess).SetAccessKind(PropertyAccessKind.[Get])
				End If
				boundExpression = boundExpression1
			End If
			Return boundExpression
		End Function

		Protected Overrides Function GenerateStateMachineCreation(ByVal stateMachineVariable As LocalSymbol, ByVal frameType As NamedTypeSymbol) As BoundStatement
			Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.F.Field(Me.F.Local(stateMachineVariable, True), Me.StateField.AsMember(frameType), True)
			instance.Add(Me.F.Assignment(boundExpression, Me.F.Literal(StateMachineStates.NotStartedStateMachine)))
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = Me._builderField.AsMember(frameType)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.F.Field(Me.F.Local(stateMachineVariable, True), fieldSymbol, True)
			Dim type As TypeSymbol = fieldSymbol.Type
			instance.Add(Me.F.Assignment(boundExpression1, Me.GenerateMethodCall(Nothing, type, "Create", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression(-1) {})))
			instance.Add(Me.F.ExpressionStatement(Me.GenerateMethodCall(boundExpression1, type, "Start", ImmutableArray.Create(Of TypeSymbol)(frameType), New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { Me.F.Local(stateMachineVariable, True) })))
			instance.Add(If(Me._asyncMethodKind = AsyncRewriter.AsyncMethodKind.[Sub], Me.F.[Return](Nothing), Me.F.[Return](Me.GeneratePropertyGet(boundExpression1, type, "Task"))))
			Return Me.RewriteBodyIfNeeded(Me.F.Block(ImmutableArray(Of LocalSymbol).Empty, instance.ToImmutableAndFree()), Me.F.TopLevelMethod, Me.Method)
		End Function

		Friend Shared Function GetAsyncMethodKind(ByVal method As MethodSymbol) As AsyncRewriter.AsyncMethodKind
			Dim asyncMethodKind As AsyncRewriter.AsyncMethodKind
			If (Not method.IsAsync) Then
				asyncMethodKind = AsyncRewriter.AsyncMethodKind.None
			ElseIf (Not method.IsSub) Then
				Dim declaringCompilation As VisualBasicCompilation = method.DeclaringCompilation
				Dim returnType As TypeSymbol = method.ReturnType
				If (Not TypeSymbol.Equals(returnType, declaringCompilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task), TypeCompareKind.ConsiderEverything)) Then
					asyncMethodKind = If(returnType.Kind <> SymbolKind.NamedType OrElse Not TypeSymbol.Equals(returnType.OriginalDefinition, declaringCompilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task_T), TypeCompareKind.ConsiderEverything), AsyncRewriter.AsyncMethodKind.None, AsyncRewriter.AsyncMethodKind.GenericTaskFunction)
				Else
					asyncMethodKind = AsyncRewriter.AsyncMethodKind.TaskFunction
				End If
			Else
				asyncMethodKind = AsyncRewriter.AsyncMethodKind.[Sub]
			End If
			Return asyncMethodKind
		End Function

		Protected Overrides Sub InitializeParameterWithProxy(ByVal parameter As ParameterSymbol, ByVal proxy As AsyncRewriter.CapturedSymbolOrExpression, ByVal stateMachineVariable As LocalSymbol, ByVal initializers As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression))
			Dim stateMachineType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim field As FieldSymbol = DirectCast(proxy, AsyncRewriter.CapturedParameterSymbol).Field
			If (Me.Method.IsGenericMethod) Then
				stateMachineType = Me.StateMachineType.Construct(Me.Method.TypeArguments)
			Else
				stateMachineType = Me.StateMachineType
			End If
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = stateMachineType
			If (parameter.IsMe) Then
				boundExpression = Me.F.[Me]()
			Else
				boundExpression = Me.F.Parameter(parameter).MakeRValue()
			End If
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundExpression
			initializers.Add(Me.F.AssignmentExpression(Me.F.Field(Me.F.Local(stateMachineVariable, True), field.AsMember(namedTypeSymbol), True), boundExpression1))
		End Sub

		Protected Overrides Sub InitializeStateMachine(ByVal bodyBuilder As ArrayBuilder(Of BoundStatement), ByVal frameType As NamedTypeSymbol, ByVal stateMachineLocal As LocalSymbol)
			If (frameType.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Class]) Then
				bodyBuilder.Add(Me.F.Assignment(Me.F.Local(stateMachineLocal, True), Me.F.Null(stateMachineLocal.Type)))
				Return
			End If
			bodyBuilder.Add(Me.F.Assignment(Me.F.Local(stateMachineLocal, True), Me.F.[New](Me.StateMachineType.Constructor.AsMember(frameType))))
		End Sub

		Friend Shared Function Rewrite(ByVal body As Microsoft.CodeAnalysis.VisualBasic.BoundBlock, ByVal method As MethodSymbol, ByVal methodOrdinal As Integer, ByVal slotAllocatorOpt As VariableSlotAllocator, ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <Out> ByRef stateMachineType As AsyncStateMachine) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			If (Not body.HasErrors) Then
				Dim asyncMethodKind As Microsoft.CodeAnalysis.VisualBasic.AsyncRewriter.AsyncMethodKind = Microsoft.CodeAnalysis.VisualBasic.AsyncRewriter.GetAsyncMethodKind(method)
				If (asyncMethodKind <> Microsoft.CodeAnalysis.VisualBasic.AsyncRewriter.AsyncMethodKind.None) Then
					stateMachineType = New AsyncStateMachine(slotAllocatorOpt, compilationState, method, methodOrdinal, If(compilationState.Compilation.Options.EnableEditAndContinue, Microsoft.CodeAnalysis.TypeKind.[Class], Microsoft.CodeAnalysis.TypeKind.Struct))
					compilationState.ModuleBuilderOpt.CompilationState.SetStateMachineType(method, stateMachineType)
					Dim asyncRewriter As Microsoft.CodeAnalysis.VisualBasic.AsyncRewriter = New Microsoft.CodeAnalysis.VisualBasic.AsyncRewriter(body, method, stateMachineType, slotAllocatorOpt, asyncMethodKind, compilationState, diagnostics)
					boundBlock = If(Not asyncRewriter.EnsureAllSymbolsAndSignature(), asyncRewriter.Rewrite(), body)
				Else
					boundBlock = body
				End If
			Else
				boundBlock = body
			End If
			Return boundBlock
		End Function

		Friend Overrides Function RewriteBodyIfNeeded(ByVal body As Microsoft.CodeAnalysis.VisualBasic.BoundStatement, ByVal topMethod As MethodSymbol, ByVal currentMethod As MethodSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			If (Not body.HasErrors) Then
				Dim boundNodes As HashSet(Of BoundNode) = Nothing
				Dim flag As Boolean = False
				Dim symbols As ISet(Of Symbol) = Nothing
				If (body.Kind <> BoundKind.Block) Then
					body = Me.F.Block(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { body })
				End If
				boundStatement = LocalRewriter.Rewrite(DirectCast(body, BoundBlock), topMethod, Me.F.CompilationState, Nothing, Me.Diagnostics, boundNodes, flag, symbols, LocalRewriter.RewritingFlags.AllowSequencePoints Or LocalRewriter.RewritingFlags.AllowEndOfMethodReturnWithExpression Or LocalRewriter.RewritingFlags.AllowCatchWithErrorLineNumberReference, Nothing, currentMethod)
			Else
				boundStatement = body
			End If
			Return boundStatement
		End Function

		Friend Enum AsyncMethodKind
			None
			[Sub]
			TaskFunction
			GenericTaskFunction
		End Enum

		Friend Class AsyncMethodToClassRewriter
			Inherits StateMachineRewriter(Of AsyncRewriter.CapturedSymbolOrExpression).StateMachineMethodToClassRewriter
			Private _conditionalAccessReceiverPlaceholderReplacementInfo As AsyncRewriter.AsyncMethodToClassRewriter.ConditionalAccessReceiverPlaceholderReplacementInfo

			Private ReadOnly _method As MethodSymbol

			Private ReadOnly _builder As FieldSymbol

			Private ReadOnly _exprReturnLabel As LabelSymbol

			Private ReadOnly _exitLabel As LabelSymbol

			Private ReadOnly _exprRetValue As LocalSymbol

			Private ReadOnly _asyncMethodKind As AsyncRewriter.AsyncMethodKind

			Private ReadOnly _awaiterFields As Dictionary(Of TypeSymbol, FieldSymbol)

			Private _nextAwaiterId As Integer

			Private ReadOnly _owner As AsyncRewriter

			Private ReadOnly _spillFieldAllocator As AsyncRewriter.SpillFieldAllocator

			Private ReadOnly _typesNeedingClearingCache As Dictionary(Of TypeSymbol, Boolean)

			Protected Overrides ReadOnly Property IsInExpressionLambda As Boolean
				Get
					Return False
				End Get
			End Property

			Protected Overrides ReadOnly Property ResumeLabelName As String
				Get
					Return "asyncLabel"
				End Get
			End Property

			Friend Sub New(ByVal method As MethodSymbol, ByVal F As SyntheticBoundNodeFactory, ByVal state As FieldSymbol, ByVal builder As FieldSymbol, ByVal hoistedVariables As IReadOnlySet(Of Symbol), ByVal nonReusableLocalProxies As Dictionary(Of Symbol, AsyncRewriter.CapturedSymbolOrExpression), ByVal synthesizedLocalOrdinals As SynthesizedLocalOrdinalsDispenser, ByVal slotAllocatorOpt As VariableSlotAllocator, ByVal nextFreeHoistedLocalSlot As Integer, ByVal owner As AsyncRewriter, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
				MyBase.New(F, state, hoistedVariables, nonReusableLocalProxies, synthesizedLocalOrdinals, slotAllocatorOpt, nextFreeHoistedLocalSlot, diagnostics)
				Me._conditionalAccessReceiverPlaceholderReplacementInfo = Nothing
				Me._exprRetValue = Nothing
				Me._awaiterFields = New Dictionary(Of TypeSymbol, FieldSymbol)()
				Me._typesNeedingClearingCache = New Dictionary(Of TypeSymbol, Boolean)()
				Me._method = method
				Me._builder = builder
				Me._exprReturnLabel = F.GenerateLabel("exprReturn")
				Me._exitLabel = F.GenerateLabel("exitLabel")
				Me._owner = owner
				Me._asyncMethodKind = AsyncRewriter.GetAsyncMethodKind(Me._method)
				Me._spillFieldAllocator = New AsyncRewriter.SpillFieldAllocator(F)
				If (Me._asyncMethodKind = AsyncRewriter.AsyncMethodKind.GenericTaskFunction) Then
					Me._exprRetValue = Me.F.SynthesizedLocal(Me._owner._resultType, SynthesizedLocalKind.AsyncMethodReturnValue, F.Syntax)
				End If
				Me._nextAwaiterId = If(slotAllocatorOpt IsNot Nothing, slotAllocatorOpt.PreviousAwaiterSlotCount, 0)
			End Sub

			Friend Overrides Sub AddProxyFieldsForStateMachineScope(ByVal proxy As AsyncRewriter.CapturedSymbolOrExpression, ByVal proxyFields As ArrayBuilder(Of FieldSymbol))
				proxy.AddProxyFieldsForStateMachineScope(proxyFields)
			End Sub

			Private Shared Function ArrayInitializerNeedsSpill(ByVal node As BoundArrayInitialization) As Boolean
				Dim flag As Boolean
				If (node IsNot Nothing) Then
					Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.Initializers.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As BoundExpression = enumerator.Current
						If (current.Kind <> BoundKind.ArrayInitialization) Then
							If (Not AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(current)) Then
								Continue While
							End If
							flag = True
							Return flag
						Else
							If (Not AsyncRewriter.AsyncMethodToClassRewriter.ArrayInitializerNeedsSpill(DirectCast(current, BoundArrayInitialization))) Then
								Continue While
							End If
							flag = True
							Return flag
						End If
					End While
					flag = False
				Else
					flag = False
				End If
				Return flag
			End Function

			Private Function GenerateAwaitForIncompleteTask(ByVal awaiterTemp As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol) As BoundBlock
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim stateInfo As StateMachineRewriter(Of AsyncRewriter.CapturedSymbolOrExpression).StateMachineMethodToClassRewriter.StateInfo = MyBase.AddState()
				Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = awaiterTemp.Type
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type
				If (typeSymbol.IsVerifierReference()) Then
					typeSymbol = Me.F.SpecialType(SpecialType.System_Object)
				End If
				Dim awaiterField As FieldSymbol = Me.GetAwaiterField(typeSymbol)
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement).GetInstance()
				instance.Add(Me.F.Assignment(Me.F.Field(Me.F.[Me](), Me.StateField, True), Me.F.AssignmentExpression(Me.F.Local(Me.CachedState, True), Me.F.Literal(stateInfo.Number))))
				instance.Add(Me.F.NoOp(NoOpStatementFlavor.AwaitYieldPoint))
				Dim boundStatements As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = instance
				Dim f As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = Me.F
				Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = Me.F.Field(Me.F.[Me](), awaiterField, True)
				If (Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(awaiterField.Type, awaiterTemp.Type, TypeCompareKind.ConsiderEverything)) Then
					boundExpression = Me.F.Local(awaiterTemp, False)
				Else
					boundExpression = Me.F.Convert(typeSymbol, Me.F.Local(awaiterTemp, False), False)
				End If
				boundStatements.Add(f.Assignment(boundFieldAccess, boundExpression))
				Dim flag As Boolean = type.IsObjectType()
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.F.Field(Me.F.[Me](), Me._builder, False)
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_ICriticalNotifyCompletion)
				If (Not namedTypeSymbol.IsErrorType()) Then
					If (Not flag) Then
						Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(Me.F.Diagnostics, Me.CompilationState.Compilation.Assembly)
						Dim flag1 As Boolean = Conversions.IsWideningConversion(Conversions.ClassifyDirectCastConversion(type, namedTypeSymbol, compoundUseSiteInfo))
						Me.F.Diagnostics.Add(Me.F.Syntax, compoundUseSiteInfo)
						instance.Add(Me.F.ExpressionStatement(Me._owner.GenerateMethodCall(boundExpression2, Me._owner._builderType, If(flag1, "AwaitUnsafeOnCompleted", "AwaitOnCompleted"), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)(type, Me.F.[Me]().Type), New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { Me.F.Local(awaiterTemp, True), Me.F.ReferenceOrByrefMe() })))
					Else
						Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Me.F.SynthesizedLocal(namedTypeSymbol, SynthesizedLocalKind.LoweringTemp, Nothing)
						Dim localSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Me.F.SynthesizedLocal(Me.F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_INotifyCompletion), SynthesizedLocalKind.LoweringTemp, Nothing)
						Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me.F.Local(awaiterTemp, False)
						Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me.F.Local(localSymbol, True)
						Dim boundLocal2 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me.F.Local(localSymbol1, True)
						Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.MakeAssignmentStatement(Me.F.[TryCast](boundLocal, localSymbol.Type), localSymbol)
						Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.F.ExpressionStatement(Me._owner.GenerateMethodCall(boundExpression2, Me._owner._builderType, "AwaitUnsafeOnCompleted", ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)(localSymbol.Type, Me.F.[Me]().Type), New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundLocal1, Me.F.ReferenceOrByrefMe() }))
						Dim boundStatement2 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.MakeAssignmentStatement(Me.F.[DirectCast](boundLocal, localSymbol1.Type), localSymbol1)
						Dim boundStatement3 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.F.ExpressionStatement(Me._owner.GenerateMethodCall(boundExpression2, Me._owner._builderType, "AwaitOnCompleted", ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)(localSymbol1.Type, Me.F.[Me]().Type), New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundLocal2, Me.F.ReferenceOrByrefMe() }))
						Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = Me.F
						Dim localSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)(localSymbol, localSymbol1)
						Dim boundStatementArray() As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = { boundStatement, Nothing }
						boundStatementArray(1) = Me.F.[If](Me.F.[Not](Me.F.ReferenceIsNothing(Me.F.Local(localSymbol, False))), boundStatement1, Me.F.Block(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { boundStatement2, boundStatement3 }))
						instance.Add(syntheticBoundNodeFactory.Block(localSymbols, boundStatementArray))
					End If
				End If
				instance.Add(Me.F.[Goto](Me._exitLabel, True))
				instance.Add(Me.F.Label(stateInfo.ResumeLabel))
				instance.Add(Me.F.NoOp(NoOpStatementFlavor.AwaitResumePoint))
				instance.Add(Me.F.Assignment(Me.F.Field(Me.F.[Me](), Me.StateField, True), Me.F.AssignmentExpression(Me.F.Local(Me.CachedState, True), Me.F.Literal(StateMachineStates.NotStartedStateMachine))))
				Dim boundStatements1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = instance
				Dim f1 As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = Me.F
				Dim boundLocal3 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me.F.Local(awaiterTemp, True)
				If (Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(awaiterTemp.Type, awaiterField.Type, TypeCompareKind.ConsiderEverything)) Then
					boundExpression1 = Me.F.Field(Me.F.[Me](), awaiterField, False)
				Else
					boundExpression1 = Me.F.Convert(awaiterTemp.Type, Me.F.Field(Me.F.[Me](), awaiterField, False), False)
				End If
				boundStatements1.Add(f1.Assignment(boundLocal3, boundExpression1))
				instance.Add(Me.F.Assignment(Me.F.Field(Me.F.[Me](), awaiterField, True), Me.F.Null(awaiterField.Type)))
				Return Me.F.Block(instance.ToImmutableAndFree())
			End Function

			Friend Sub GenerateMoveNext(ByVal body As Microsoft.CodeAnalysis.VisualBasic.BoundStatement, ByVal moveNextMethod As MethodSymbol)
				Me.F.CurrentMethod = moveNextMethod
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(body), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
				Dim fieldSymbols As ImmutableArray(Of FieldSymbol) = New ImmutableArray(Of FieldSymbol)()
				MyBase.TryUnwrapBoundStateMachineScope(boundStatement, fieldSymbols)
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement).GetInstance()
				instance.Add(Me.F.Assignment(Me.F.Local(Me.CachedState, True), Me.F.Field(Me.F.[Me](), Me.StateField, False)))
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Me.F.SynthesizedLocal(Me.F.WellKnownType(WellKnownType.System_Exception), SynthesizedLocalKind.LoweringTemp, Nothing)
				Dim f As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = Me.F
				Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = Me.F.Block(ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).Empty, New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory.HiddenSequencePoint(Nothing), MyBase.Dispatch(), boundStatement })
				Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = Me.F
				Dim boundCatchBlockArray(0) As BoundCatchBlock
				Dim f1 As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = Me.F
				Dim syntheticBoundNodeFactory1 As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = Me.F
				Dim boundStatementArray() As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = { Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory.HiddenSequencePoint(Nothing), Me.F.Assignment(Me.F.Field(Me.F.[Me](), Me.StateField, True), Me.F.Literal(StateMachineStates.FinishedStateMachine)), Nothing, Nothing }
				boundStatementArray(2) = Me.F.ExpressionStatement(Me._owner.GenerateMethodCall(Me.F.Field(Me.F.[Me](), Me._builder, False), Me._owner._builderType, "SetException", New BoundExpression() { Me.F.Local(localSymbol, False) }))
				boundStatementArray(3) = Me.F.[Goto](Me._exitLabel, True)
				boundCatchBlockArray(0) = f1.[Catch](localSymbol, syntheticBoundNodeFactory1.Block(boundStatementArray), True)
				instance.Add(f.[Try](boundBlock, syntheticBoundNodeFactory.CatchBlocks(boundCatchBlockArray), Nothing, Nothing))
				instance.Add(Me.F.Label(Me._exprReturnLabel))
				Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = Me.F.Assignment(Me.F.Field(Me.F.[Me](), Me.StateField, True), Me.F.AssignmentExpression(Me.F.Local(Me.CachedState, True), Me.F.Literal(StateMachineStates.FinishedStateMachine)))
				Dim syntax As MethodBlockSyntax = TryCast(body.Syntax, MethodBlockSyntax)
				If (syntax IsNot Nothing) Then
					instance.Add(Me.F.SequencePointWithSpan(syntax, syntax.EndBlockStatement.Span, boundExpressionStatement))
					instance.Add(Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory.HiddenSequencePoint(Nothing))
				Else
					instance.Add(boundExpressionStatement)
				End If
				instance.Add(Me.F.ExpressionStatement(Me._owner.GenerateMethodCall(Me.F.Field(Me.F.[Me](), Me._builder, False), Me._owner._builderType, "SetResult", If(Me._asyncMethodKind = AsyncRewriter.AsyncMethodKind.GenericTaskFunction, New BoundExpression() { Me.F.Local(Me._exprRetValue, False) }, Array.Empty(Of BoundExpression)()))))
				instance.Add(Me.F.Label(Me._exitLabel))
				instance.Add(Me.F.[Return](Nothing))
				Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = instance.ToImmutableAndFree()
				Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = Me.F.Block(If(Me._exprRetValue IsNot Nothing, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)(Me._exprRetValue, Me.CachedState), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)(Me.CachedState)), immutableAndFree)
				If (fieldSymbols.Length > 0) Then
					boundBlock1 = MyBase.MakeStateMachineScope(fieldSymbols, boundBlock1)
				End If
				Me._owner.CloseMethod(boundBlock1)
			End Sub

			Protected Overrides Function GenerateReturn(ByVal finished As Boolean) As BoundStatement
				Return Me.F.[Goto](Me._exitLabel, True)
			End Function

			Private Function GetAwaiterField(ByVal awaiterType As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol
				Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = Nothing
				If (Not Me._awaiterFields.TryGetValue(awaiterType, fieldSymbol)) Then
					Dim num As Integer = -1
					If (Me.SlotAllocatorOpt Is Nothing OrElse Not Me.SlotAllocatorOpt.TryGetPreviousAwaiterSlotIndex(Me.F.CompilationState.ModuleBuilderOpt.Translate(awaiterType, Me.F.Syntax, Me.F.Diagnostics.DiagnosticBag), Me.F.Diagnostics.DiagnosticBag, num)) Then
						num = Me._nextAwaiterId
						Me._nextAwaiterId = Me._nextAwaiterId + 1
					End If
					Dim str As String = GeneratedNames.MakeStateMachineAwaiterFieldName(num)
					fieldSymbol = Me.F.StateMachineField(awaiterType, Me._method, str, SynthesizedLocalKind.AwaiterField, num, Accessibility.Internal)
					Me._awaiterFields.Add(awaiterType, fieldSymbol)
				End If
				Return fieldSymbol
			End Function

			Private Function MakeAssignmentStatement(ByVal expression As BoundExpression, ByVal temp As LocalSymbol, <InAttribute> <Out> ByRef builder As AsyncRewriter.SpillBuilder) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
				If (Not AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(expression)) Then
					boundStatement = Me.F.Assignment(Me.F.Local(temp, True), expression)
				Else
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(expression, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					builder.AssumeFieldsIfNeeded(boundSpillSequence)
					boundStatement = Me.RewriteSpillSequenceIntoBlock(boundSpillSequence, False, New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { Me.F.Assignment(Me.F.Local(temp, True), boundSpillSequence.ValueOpt) })
				End If
				Return boundStatement
			End Function

			Private Function MakeAssignmentStatement(ByVal expression As BoundExpression, ByVal temp As LocalSymbol) As BoundStatement
				Return Me.F.Assignment(Me.F.Local(temp, True), expression)
			End Function

			Private Function MakeExpressionStatement(ByVal expression As BoundExpression, ByRef builder As AsyncRewriter.SpillBuilder) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
				If (Not AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(expression)) Then
					boundStatement = Me.F.ExpressionStatement(expression)
				Else
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(expression, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					builder.AssumeFieldsIfNeeded(boundSpillSequence)
					boundStatement = Me.RewriteSpillSequenceIntoBlock(boundSpillSequence, True)
				End If
				Return boundStatement
			End Function

			Protected Overrides Function MaterializeProxy(ByVal origExpression As BoundExpression, ByVal proxy As AsyncRewriter.CapturedSymbolOrExpression) As BoundNode
				Return proxy.Materialize(Me, origExpression.IsLValue)
			End Function

			Private Shared Function NeedsSpill(ByVal node As BoundExpression) As Boolean
				Dim flag As Boolean
				If (node IsNot Nothing) Then
					Dim kind As BoundKind = node.Kind
					If (kind = BoundKind.ArrayInitialization) Then
						Throw ExceptionUtilities.UnexpectedValue(node.Kind)
					End If
					flag = If(kind <> BoundKind.SpillSequence, False, True)
				Else
					flag = False
				End If
				Return flag
			End Function

			Private Shared Function NeedsSpill(ByVal nodes As ImmutableArray(Of BoundExpression)) As Boolean
				Dim flag As Boolean
				If (Not nodes.IsEmpty) Then
					Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = nodes.GetEnumerator()
					While enumerator.MoveNext()
						If (Not AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(enumerator.Current)) Then
							Continue While
						End If
						flag = True
						Return flag
					End While
				End If
				flag = False
				Return flag
			End Function

			Friend Function ProcessRewrittenAssignmentOperator(ByVal rewritten As BoundAssignmentOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = rewritten.Left
				Dim right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = rewritten.Right
				Dim flag As Boolean = AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(left)
				Dim flag1 As Boolean = AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(right)
				If (Not flag AndAlso Not flag1) Then
					boundExpression = rewritten
				ElseIf (flag1) Then
					Dim spillBuilder As AsyncRewriter.SpillBuilder = New AsyncRewriter.SpillBuilder()
					Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.SpillLValue(left, False, True, spillBuilder, True)
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(right, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					spillBuilder.AddSpill(boundSpillSequence)
					boundExpression = spillBuilder.BuildSequenceAndFree(Me.F, rewritten.Update(boundExpression1, rewritten.LeftOnTheRightOpt, boundSpillSequence.ValueOpt, rewritten.SuppressObjectClone, rewritten.Type))
				Else
					Dim boundSpillSequence1 As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(left, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundExpression = AsyncRewriter.AsyncMethodToClassRewriter.SpillSequenceWithNewValue(boundSpillSequence1, rewritten.Update(boundSpillSequence1.ValueOpt, rewritten.LeftOnTheRightOpt, right, rewritten.SuppressObjectClone, rewritten.Type))
				End If
				Return boundExpression
			End Function

			Private Function RewriteSpillSequenceIntoBlock(ByVal spill As BoundSpillSequence, ByVal addValueAsExpression As Boolean) As BoundBlock
				Return Me.RewriteSpillSequenceIntoBlock(spill, addValueAsExpression, Array.Empty(Of BoundStatement)())
			End Function

			Private Function RewriteSpillSequenceIntoBlock(ByVal spill As BoundSpillSequence, ByVal addValueAsExpression As Boolean, ByVal ParamArray additional As BoundStatement()) As BoundBlock
				Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
				instance.AddRange(spill.Statements)
				If (addValueAsExpression AndAlso spill.ValueOpt IsNot Nothing) Then
					instance.Add(Me.F.ExpressionStatement(spill.ValueOpt))
				End If
				instance.AddRange(additional)
				Dim spillFields As ImmutableArray(Of FieldSymbol) = spill.SpillFields
				Dim length As Integer = spillFields.Length - 1
				Dim num As Integer = 0
				Do
					Dim item As FieldSymbol = spillFields(num)
					If (Me.TypeNeedsClearing(item.Type)) Then
						instance.Add(Me.F.Assignment(Me.F.Field(Me.F.[Me](), item, True), Me.F.Null(item.Type)))
					End If
					Me._spillFieldAllocator.FreeField(item)
					num = num + 1
				Loop While num <= length
				Return Me.F.Block(spill.Locals, instance.ToImmutableAndFree())
			End Function

			Private Function SpillArgumentListInner(ByVal arguments As ImmutableArray(Of BoundExpression), ByVal spillBuilders As ArrayBuilder(Of AsyncRewriter.SpillBuilder), ByVal firstArgumentIsAReceiverOfAMethodCall As Boolean, <InAttribute> <Out> ByRef spilledFirstArg As Boolean) As ImmutableArray(Of BoundExpression)
				Dim valueOpt As BoundExpression
				Dim boundExpressionArray(arguments.Length - 1 + 1 - 1) As BoundExpression
				For i As Integer = arguments.Length - 1 To 0 Step -1
					Dim item As BoundExpression = arguments(i)
					If (item.Kind <> BoundKind.ArrayInitialization) Then
						Dim spillBuilder As AsyncRewriter.SpillBuilder = New AsyncRewriter.SpillBuilder()
						If (spilledFirstArg) Then
							valueOpt = Me.SpillValue(item, If(i <> 0, False, firstArgumentIsAReceiverOfAMethodCall), True, spillBuilder)
						ElseIf (item.Kind <> BoundKind.SpillSequence) Then
							valueOpt = item
						Else
							spilledFirstArg = True
							Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
							spillBuilder.AddSpill(boundSpillSequence)
							valueOpt = boundSpillSequence.ValueOpt
						End If
						boundExpressionArray(i) = valueOpt
						If (Not spillBuilder.IsEmpty) Then
							spillBuilders.Add(spillBuilder)
						End If
					Else
						Dim boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization)
						Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.SpillArgumentListInner(boundArrayInitialization.Initializers, spillBuilders, False, spilledFirstArg)
						boundExpressionArray(i) = boundArrayInitialization.Update(boundExpressions, boundArrayInitialization.Type)
					End If
				Next

				Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundExpressionArray)
			End Function

			Private Function SpillExpressionList(<InAttribute> <Out> ByRef builder As AsyncRewriter.SpillBuilder, ByVal expressions As ImmutableArray(Of BoundExpression), ByVal firstArgumentIsAReceiverOfAMethodCall As Boolean) As ImmutableArray(Of BoundExpression)
				Dim instance As ArrayBuilder(Of AsyncRewriter.SpillBuilder) = ArrayBuilder(Of AsyncRewriter.SpillBuilder).GetInstance()
				Dim flag As Boolean = False
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.SpillArgumentListInner(expressions, instance, firstArgumentIsAReceiverOfAMethodCall, flag)
				For i As Integer = instance.Count - 1 To 0 Step -1
					Dim spillBuilders As ArrayBuilder(Of AsyncRewriter.SpillBuilder) = instance
					Dim spillBuilders1 As ArrayBuilder(Of AsyncRewriter.SpillBuilder) = spillBuilders
					Dim num As Integer = i
					Dim num1 As Integer = num
					Dim item As AsyncRewriter.SpillBuilder = spillBuilders(num)
					builder.AddSpill(item)
					spillBuilders1(num1) = item
					item = instance(i)
					item.Free()
				Next

				instance.Free()
				Return boundExpressions
			End Function

			Private Function SpillExpressionList(<InAttribute> <Out> ByRef builder As AsyncRewriter.SpillBuilder, ByVal ParamArray expressions As BoundExpression()) As ImmutableArray(Of BoundExpression)
				Return Me.SpillExpressionList(builder, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(expressions), False)
			End Function

			Private Function SpillExpressionsWithReceiver(ByVal receiverOpt As BoundExpression, ByVal isReceiverOfAMethodCall As Boolean, ByVal expressions As ImmutableArray(Of BoundExpression), <InAttribute> <Out> ByRef spillBuilder As AsyncRewriter.SpillBuilder) As AsyncRewriter.AsyncMethodToClassRewriter.ExpressionsWithReceiver
				Dim expressionsWithReceiver As AsyncRewriter.AsyncMethodToClassRewriter.ExpressionsWithReceiver
				If (receiverOpt IsNot Nothing) Then
					Dim boundExpressions As ImmutableArray(Of BoundExpression) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.Concat(Of BoundExpression)(ImmutableArray.Create(Of BoundExpression)(receiverOpt), expressions)
					Dim boundExpressions1 As ImmutableArray(Of BoundExpression) = Me.SpillExpressionList(spillBuilder, boundExpressions, isReceiverOfAMethodCall)
					expressionsWithReceiver = New AsyncRewriter.AsyncMethodToClassRewriter.ExpressionsWithReceiver(boundExpressions1.First(), boundExpressions1.RemoveAt(0))
				Else
					expressionsWithReceiver = New AsyncRewriter.AsyncMethodToClassRewriter.ExpressionsWithReceiver(Nothing, Me.SpillExpressionList(spillBuilder, expressions, False))
				End If
				Return expressionsWithReceiver
			End Function

			Private Function SpillLValue(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal isReceiver As Boolean, ByVal evaluateSideEffects As Boolean, <InAttribute> <Out> ByRef builder As AsyncRewriter.SpillBuilder, Optional ByVal isAssignmentTarget As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				If (Not isReceiver OrElse Not expr.Type.IsReferenceType OrElse expr.Type.IsTypeParameter()) Then
					Dim kind As BoundKind = expr.Kind
					If (kind <= BoundKind.Sequence) Then
						If (kind = BoundKind.ArrayAccess) Then
							Dim boundArrayAccess As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess = DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess)
							Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.SpillRValue(boundArrayAccess.Expression, builder)
							Dim indices As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = boundArrayAccess.Indices
							Dim boundExpressionArray(indices.Length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
							Dim length As Integer = indices.Length - 1
							Dim num As Integer = 0
							Do
								boundExpressionArray(num) = Me.SpillRValue(indices(num), builder)
								num = num + 1
							Loop While num <= length
							boundArrayAccess = boundArrayAccess.Update(boundExpression1, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpressionArray), boundArrayAccess.IsLValue, boundArrayAccess.Type)
							If (evaluateSideEffects And Not isAssignmentTarget) Then
								builder.AddStatement(Me.F.ExpressionStatement(boundArrayAccess))
							End If
							boundExpression = boundArrayAccess
							Return boundExpression
						ElseIf (kind = BoundKind.FieldAccess) Then
							Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess)
							If (Not AsyncRewriter.AsyncMethodToClassRewriter.Unspillable(boundFieldAccess.ReceiverOpt)) Then
								Dim flag As Boolean = evaluateSideEffects And Not isAssignmentTarget And boundFieldAccess.FieldSymbol.ContainingType.IsReferenceType
								Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.SpillValue(boundFieldAccess.ReceiverOpt, True, evaluateSideEffects And Not flag, builder)
								boundFieldAccess = boundFieldAccess.Update(boundExpression2, boundFieldAccess.FieldSymbol, boundFieldAccess.IsLValue, boundFieldAccess.SuppressVirtualCalls, Nothing, boundFieldAccess.Type)
								If (flag) Then
									builder.AddStatement(Me.F.ExpressionStatement(boundFieldAccess))
								End If
								boundExpression = boundFieldAccess
								Return boundExpression
							Else
								boundExpression = boundFieldAccess
								Return boundExpression
							End If
						Else
							If (kind <> BoundKind.Sequence) Then
								GoTo Label3
							End If
							Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSequence = DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.BoundSequence)
							builder.AddLocals(boundSequence.Locals)
							Dim sideEffects As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = boundSequence.SideEffects
							If (Not sideEffects.IsEmpty) Then
								Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Enumerator = sideEffects.GetEnumerator()
								While enumerator.MoveNext()
									Dim current As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = enumerator.Current
									If (Not AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(current)) Then
										builder.AddStatement(Me.F.ExpressionStatement(current))
									Else
										Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
										builder.AssumeFieldsIfNeeded(boundSpillSequence)
										builder.AddStatement(Me.RewriteSpillSequenceIntoBlock(boundSpillSequence, True))
									End If
								End While
							End If
							boundExpression = Me.SpillLValue(boundSequence.ValueOpt, evaluateSideEffects, isReceiver, builder, False)
							Return boundExpression
						End If
					ElseIf (kind = BoundKind.Local) Then
						boundExpression = expr
						Return boundExpression
					ElseIf (kind = BoundKind.Parameter) Then
						boundExpression = expr
						Return boundExpression
					Else
						If (kind <> BoundKind.SpillSequence) Then
							GoTo Label3
						End If
						Dim boundSpillSequence1 As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
						builder.AddSpill(boundSpillSequence1)
						boundExpression = Me.SpillLValue(boundSpillSequence1.ValueOpt, isReceiver, evaluateSideEffects, builder, False)
						Return boundExpression
					End If
				Label3:
					boundExpression = Me.SpillRValue(expr, builder)
				Else
					boundExpression = Me.SpillRValue(expr.MakeRValue(), builder)
				End If
				Return boundExpression
			End Function

			Private Function SpillRValue(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, <InAttribute> <Out> ByRef builder As AsyncRewriter.SpillBuilder) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
				Dim boundStatementArray As Microsoft.CodeAnalysis.VisualBasic.BoundStatement()
				Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence
				Dim kind As BoundKind = expr.Kind
				If (kind <= BoundKind.Literal) Then
					If (kind = BoundKind.ArrayInitialization) Then
						Dim boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization = DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization)
						boundExpression = boundArrayInitialization.Update(Me.SpillExpressionList(builder, boundArrayInitialization.Initializers, False), boundArrayInitialization.Type)
					Else
						If (kind <> BoundKind.Literal) Then
							fieldSymbol = Me._spillFieldAllocator.AllocateField(expr.Type)
							boundStatement = Me.F.Assignment(Me.F.Field(Me.F.[Me](), fieldSymbol, True), expr)
							If (expr.Kind = BoundKind.SpillSequence) Then
								boundSpillSequence = DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
								boundStatementArray = New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { boundStatement }
								boundStatement = Me.RewriteSpillSequenceIntoBlock(boundSpillSequence, True, boundStatementArray)
							End If
							builder.AddFieldWithInitialization(fieldSymbol, boundStatement)
							boundExpression = Me.F.Field(Me.F.[Me](), fieldSymbol, False)
							Return boundExpression
						End If
						boundExpression = expr
					End If
				ElseIf (kind = BoundKind.SpillSequence) Then
					Dim boundSpillSequence1 As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					builder.AddSpill(boundSpillSequence1)
					boundExpression = Me.SpillRValue(boundSpillSequence1.ValueOpt, builder)
				Else
					If (kind <> BoundKind.ConditionalAccessReceiverPlaceholder) Then
						If (kind = BoundKind.ComplexConditionalAccessReceiver) Then
							Throw ExceptionUtilities.Unreachable
						End If
						fieldSymbol = Me._spillFieldAllocator.AllocateField(expr.Type)
						boundStatement = Me.F.Assignment(Me.F.Field(Me.F.[Me](), fieldSymbol, True), expr)
						If (expr.Kind = BoundKind.SpillSequence) Then
							boundSpillSequence = DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
							boundStatementArray = New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { boundStatement }
							boundStatement = Me.RewriteSpillSequenceIntoBlock(boundSpillSequence, True, boundStatementArray)
						End If
						builder.AddFieldWithInitialization(fieldSymbol, boundStatement)
						boundExpression = Me.F.Field(Me.F.[Me](), fieldSymbol, False)
						Return boundExpression
					End If
					If (Me._conditionalAccessReceiverPlaceholderReplacementInfo Is Nothing OrElse Me._conditionalAccessReceiverPlaceholderReplacementInfo.PlaceholderId <> DirectCast(expr, BoundConditionalAccessReceiverPlaceholder).PlaceholderId) Then
						Throw ExceptionUtilities.Unreachable
					End If
					Me._conditionalAccessReceiverPlaceholderReplacementInfo.IsSpilled = True
					boundExpression = expr
				End If
				Return boundExpression
			End Function

			Private Shared Function SpillSequenceWithNewValue(ByVal spill As BoundSpillSequence, ByVal newValue As BoundExpression) As BoundSpillSequence
				Return spill.Update(spill.Locals, spill.SpillFields, spill.Statements, newValue, newValue.Type)
			End Function

			Private Function SpillValue(ByVal expr As BoundExpression, <InAttribute> <Out> ByRef builder As AsyncRewriter.SpillBuilder) As BoundExpression
				Return Me.SpillValue(expr, False, True, builder)
			End Function

			Private Function SpillValue(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal isReceiver As Boolean, ByVal evaluateSideEffects As Boolean, <InAttribute> <Out> ByRef builder As AsyncRewriter.SpillBuilder) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				If (Not AsyncRewriter.AsyncMethodToClassRewriter.Unspillable(expr)) Then
					boundExpression = If(isReceiver OrElse expr.IsLValue, Me.SpillLValue(expr, isReceiver, evaluateSideEffects, builder, False), Me.SpillRValue(expr, builder))
				Else
					boundExpression = expr
				End If
				Return boundExpression
			End Function

			Private Function TypeNeedsClearing(ByVal type As TypeSymbol) As Boolean
				Dim flag As Boolean
				Dim flag1 As Boolean = False
				If (Me._typesNeedingClearingCache.TryGetValue(type, flag1)) Then
					flag = flag1
				ElseIf (type.IsArrayType() OrElse type.IsTypeParameter()) Then
					Me._typesNeedingClearingCache.Add(type, True)
					flag = True
				ElseIf (type.IsErrorType() OrElse type.IsEnumType()) Then
					Me._typesNeedingClearingCache.Add(type, False)
					flag = False
				Else
					Select Case type.SpecialType
						Case SpecialType.System_Object
						Case SpecialType.System_String
							flag1 = True
							Exit Select
						Case SpecialType.System_Enum
						Case SpecialType.System_MulticastDelegate
						Case SpecialType.System_Delegate
						Case SpecialType.System_ValueType
						Case SpecialType.System_Array
						Case SpecialType.System_Collections_IEnumerable
						Case SpecialType.System_Collections_Generic_IEnumerable_T
						Case SpecialType.System_Collections_Generic_IList_T
						Case SpecialType.System_Collections_Generic_ICollection_T
						Case SpecialType.System_Collections_IEnumerator
						Case SpecialType.System_Collections_Generic_IEnumerator_T
						Case SpecialType.System_Collections_Generic_IReadOnlyList_T
						Case SpecialType.System_Collections_Generic_IReadOnlyCollection_T
						Case SpecialType.System_Nullable_T
						Case SpecialType.System_DateTime
						Case SpecialType.System_Runtime_CompilerServices_IsVolatile
						Case SpecialType.System_IDisposable
						Label0:
							Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
							If (namedTypeSymbol IsNot Nothing AndAlso namedTypeSymbol.IsGenericType) Then
								flag1 = True
								Exit Select
							ElseIf (type.TypeKind = Microsoft.CodeAnalysis.TypeKind.Struct) Then
								Me._typesNeedingClearingCache.Add(type, True)
								flag1 = False
								Dim enumerator As ImmutableArray(Of Symbol).Enumerator = type.GetMembersUnordered().GetEnumerator()
								While enumerator.MoveNext()
									Dim current As Symbol = enumerator.Current
									If (current.IsShared) Then
										Continue While
									End If
									Dim kind As SymbolKind = current.Kind
									If (kind = SymbolKind.[Event]) Then
										If (Not Me.TypeNeedsClearing(DirectCast(current, EventSymbol).AssociatedField.Type)) Then
											Continue While
										End If
										flag1 = True
									ElseIf (kind = SymbolKind.Field) Then
										If (Not Me.TypeNeedsClearing(DirectCast(current, FieldSymbol).Type)) Then
											Continue While
										End If
										flag1 = True
									End If
								End While
								Me._typesNeedingClearingCache.Remove(type)
								Exit Select
							Else
								flag1 = True
								Exit Select
							End If
						Case SpecialType.System_Void
						Case SpecialType.System_Boolean
						Case SpecialType.System_Char
						Case SpecialType.System_SByte
						Case SpecialType.System_Byte
						Case SpecialType.System_Int16
						Case SpecialType.System_UInt16
						Case SpecialType.System_Int32
						Case SpecialType.System_UInt32
						Case SpecialType.System_Int64
						Case SpecialType.System_UInt64
						Case SpecialType.System_Decimal
						Case SpecialType.System_Single
						Case SpecialType.System_Double
						Case SpecialType.System_IntPtr
						Case SpecialType.System_UIntPtr
						Case SpecialType.System_TypedReference
						Case SpecialType.System_ArgIterator
						Case SpecialType.System_RuntimeArgumentHandle
							flag1 = False
							Exit Select
						Case Else
							GoTo Label0
					End Select
					Me._typesNeedingClearingCache.Add(type, flag1)
					flag = flag1
				End If
				Return flag
			End Function

			Private Shared Function Unspillable(ByVal node As BoundExpression) As Boolean
				Dim flag As Boolean
				If (node IsNot Nothing) Then
					Dim kind As BoundKind = node.Kind
					If (kind = BoundKind.TypeExpression) Then
						flag = True
					Else
						Select Case kind
							Case BoundKind.Literal
								flag = True
								Exit Select
							Case BoundKind.MeReference
								flag = True
								Exit Select
							Case BoundKind.ValueTypeMeReference
							Label0:
								flag = False
								Exit Select
							Case BoundKind.MyBaseReference
							Case BoundKind.MyClassReference
								Throw ExceptionUtilities.UnexpectedValue(node.Kind)
							Case Else
								GoTo Label0
						End Select
					End If
				Else
					flag = True
				End If
				Return flag
			End Function

			Public Overrides Function VisitArrayAccess(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundArrayAccess As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess = DirectCast(MyBase.VisitArrayAccess(node), Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess)
				Dim expression As BoundExpression = boundArrayAccess.Expression
				Dim indices As ImmutableArray(Of BoundExpression) = boundArrayAccess.Indices
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(expression) OrElse AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(indices)) Then
					Dim spillBuilder As AsyncRewriter.SpillBuilder = New AsyncRewriter.SpillBuilder()
					Dim expressionsWithReceiver As AsyncRewriter.AsyncMethodToClassRewriter.ExpressionsWithReceiver = Me.SpillExpressionsWithReceiver(expression, False, indices, spillBuilder)
					boundNode = spillBuilder.BuildSequenceAndFree(Me.F, boundArrayAccess.Update(expressionsWithReceiver.ReceiverOpt, expressionsWithReceiver.Arguments, boundArrayAccess.IsLValue, boundArrayAccess.Type))
				Else
					boundNode = boundArrayAccess
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitArrayCreation(ByVal node As BoundArrayCreation) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.VisitList(Of BoundExpression)(node.Bounds)
				Dim boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization = DirectCast(Me.Visit(node.InitializerOpt), Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization)
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = MyBase.VisitType(node.Type)
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(boundExpressions) OrElse AsyncRewriter.AsyncMethodToClassRewriter.ArrayInitializerNeedsSpill(boundArrayInitialization)) Then
					Dim spillBuilder As AsyncRewriter.SpillBuilder = New AsyncRewriter.SpillBuilder()
					boundExpressions = Me.SpillExpressionList(spillBuilder, boundExpressions, False)
					If (boundArrayInitialization IsNot Nothing) Then
						boundArrayInitialization = boundArrayInitialization.Update(Me.SpillExpressionList(spillBuilder, boundArrayInitialization.Initializers, False), boundArrayInitialization.Type)
					End If
					boundNode = spillBuilder.BuildSequenceAndFree(Me.F, node.Update(node.IsParamArrayArgument, boundExpressions, boundArrayInitialization, Nothing, ConversionKind.DelegateRelaxationLevelNone, typeSymbol))
				Else
					boundNode = node.Update(node.IsParamArrayArgument, boundExpressions, boundArrayInitialization, Nothing, ConversionKind.DelegateRelaxationLevelNone, typeSymbol)
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitArrayInitialization(ByVal node As BoundArrayInitialization) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				If (node IsNot Nothing) Then
					boundNode = Me.VisitArrayInitializationParts(node)
				Else
					boundNode = Nothing
				End If
				Return boundNode
			End Function

			Private Function VisitArrayInitializationParts(ByVal node As BoundArrayInitialization) As BoundExpression
				Dim initializers As ImmutableArray(Of BoundExpression) = node.Initializers
				Dim length As Integer = initializers.Length
				Dim boundExpressionArray(length - 1 + 1 - 1) As BoundExpression
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					Dim item As BoundExpression = initializers(num1)
					boundExpressionArray(num1) = If(item.Kind = BoundKind.ArrayInitialization, Me.VisitArrayInitializationParts(DirectCast(item, BoundArrayInitialization)), Me.VisitExpression(item))
					num1 = num1 + 1
				Loop While num1 <= num
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = MyBase.VisitType(node.Type)
				Return node.Update(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundExpressionArray), typeSymbol)
			End Function

			Public Overrides Function VisitArrayLength(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundArrayLength) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundArrayLength As Microsoft.CodeAnalysis.VisualBasic.BoundArrayLength = DirectCast(MyBase.VisitArrayLength(node), Microsoft.CodeAnalysis.VisualBasic.BoundArrayLength)
				Dim expression As BoundExpression = boundArrayLength.Expression
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(expression)) Then
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(expression, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundNode = AsyncRewriter.AsyncMethodToClassRewriter.SpillSequenceWithNewValue(boundSpillSequence, boundArrayLength.Update(boundSpillSequence.ValueOpt, boundArrayLength.Type))
				Else
					boundNode = boundArrayLength
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitAssignmentOperator(ByVal node As BoundAssignmentOperator) As BoundNode
				Return Me.ProcessRewrittenAssignmentOperator(DirectCast(MyBase.VisitAssignmentOperator(node), BoundAssignmentOperator))
			End Function

			Public Overrides Function VisitAwaitOperator(ByVal node As BoundAwaitOperator) As BoundNode
				Dim spillBuilder As AsyncRewriter.SpillBuilder = New AsyncRewriter.SpillBuilder()
				Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = node.GetAwaiter.Type.InternalSubstituteTypeParameters(Me.TypeMap).Type
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Me.F.SynthesizedLocal(type, SynthesizedLocalKind.Awaiter, node.Syntax)
				spillBuilder.AddLocal(localSymbol)
				Dim awaiterInstancePlaceholder As BoundLValuePlaceholder = node.AwaiterInstancePlaceholder
				Me.PlaceholderReplacementMap.Add(awaiterInstancePlaceholder, Me.F.Local(localSymbol, True))
				Dim awaitableInstancePlaceholder As BoundRValuePlaceholder = node.AwaitableInstancePlaceholder
				Me.PlaceholderReplacementMap.Add(awaitableInstancePlaceholder, Me.VisitExpression(node.Operand))
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(node.GetAwaiter)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(node.IsCompleted)
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(node.GetResult)
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = MyBase.VisitType(node.Type)
				Me.PlaceholderReplacementMap.Remove(awaiterInstancePlaceholder)
				Me.PlaceholderReplacementMap.Remove(awaitableInstancePlaceholder)
				spillBuilder.AddStatement(Me.MakeAssignmentStatement(boundExpression, localSymbol, spillBuilder))
				spillBuilder.AddStatement(SyntheticBoundNodeFactory.HiddenSequencePoint(Nothing))
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.GenerateAwaitForIncompleteTask(localSymbol)
				If (Not type.IsObjectType()) Then
					spillBuilder.AddStatement(Me.F.[If](Me.F.[Not](boundExpression1), boundStatement))
				Else
					spillBuilder.AddStatement(Me.F.[If](Me.F.Convert(Me.F.SpecialType(SpecialType.System_Boolean), boundExpression1, False), Me.F.StatementList(), boundStatement))
				End If
				Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.F.AssignmentExpression(Me.F.Local(localSymbol, True), Me.F.Null(localSymbol.Type))
				If (typeSymbol.SpecialType = SpecialType.System_Void) Then
					boundExpression3 = Me.F.Sequence(New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression2, boundExpression4 })
				Else
					Dim localSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Me.F.SynthesizedLocal(typeSymbol, SynthesizedLocalKind.LoweringTemp, Nothing)
					boundExpression3 = Me.F.Sequence(localSymbol1, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { Me.F.AssignmentExpression(Me.F.Local(localSymbol1, True), boundExpression2), boundExpression4, Me.F.Local(localSymbol1, False) })
				End If
				Return spillBuilder.BuildSequenceAndFree(Me.F, boundExpression3)
			End Function

			Public Overrides Function VisitBinaryConditionalExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryConditionalExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundBinaryConditionalExpression As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryConditionalExpression = DirectCast(MyBase.VisitBinaryConditionalExpression(node), Microsoft.CodeAnalysis.VisualBasic.BoundBinaryConditionalExpression)
				Dim testExpression As BoundExpression = boundBinaryConditionalExpression.TestExpression
				Dim elseExpression As BoundExpression = boundBinaryConditionalExpression.ElseExpression
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(testExpression) OrElse AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(elseExpression)) Then
					Dim spillBuilder As AsyncRewriter.SpillBuilder = New AsyncRewriter.SpillBuilder()
					Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Me.F.SynthesizedLocal(boundBinaryConditionalExpression.Type, SynthesizedLocalKind.LoweringTemp, Nothing)
					spillBuilder.AddLocal(localSymbol)
					spillBuilder.AddStatement(Me.MakeAssignmentStatement(testExpression, localSymbol, spillBuilder))
					spillBuilder.AddStatement(Me.F.[If](Me.F.ReferenceIsNothing(Me.F.Local(localSymbol, False)), Me.MakeAssignmentStatement(elseExpression, localSymbol, spillBuilder)))
					boundNode = spillBuilder.BuildSequenceAndFree(Me.F, Me.F.Local(localSymbol, False))
				Else
					boundNode = boundBinaryConditionalExpression
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitBinaryOperator(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = DirectCast(MyBase.VisitBinaryOperator(node), Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
				Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundBinaryOperator.Left
				Dim right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundBinaryOperator.Right
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(left) OrElse AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(right)) Then
					Dim spillBuilder As AsyncRewriter.SpillBuilder = New AsyncRewriter.SpillBuilder()
					Dim operatorKind As BinaryOperatorKind = boundBinaryOperator.OperatorKind And BinaryOperatorKind.OpMask
					If (operatorKind = BinaryOperatorKind.[AndAlso] OrElse operatorKind = BinaryOperatorKind.[OrElse]) Then
						Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.SpillValue(left, spillBuilder)
						Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Me.F.SynthesizedLocal(boundBinaryOperator.Type, SynthesizedLocalKind.LoweringTemp, Nothing)
						spillBuilder.AddLocal(localSymbol)
						spillBuilder.AddStatement(If(operatorKind = BinaryOperatorKind.[AndAlso], Me.F.[If](boundExpression, Me.MakeAssignmentStatement(right, localSymbol, spillBuilder), Me.MakeAssignmentStatement(Me.F.Literal(False), localSymbol)), Me.F.[If](boundExpression, Me.MakeAssignmentStatement(Me.F.Literal(True), localSymbol), Me.MakeAssignmentStatement(right, localSymbol, spillBuilder))))
						boundNode = spillBuilder.BuildSequenceAndFree(Me.F, Me.F.Local(localSymbol, False))
					Else
						Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Me.SpillExpressionList(spillBuilder, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { left, right })
						boundNode = spillBuilder.BuildSequenceAndFree(Me.F, boundBinaryOperator.Update(boundBinaryOperator.OperatorKind, boundExpressions(0), boundExpressions(1), boundBinaryOperator.Checked, boundBinaryOperator.ConstantValueOpt, boundBinaryOperator.Type))
					End If
				Else
					boundNode = boundBinaryOperator
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitCall(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundCall) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = DirectCast(MyBase.VisitCall(node), Microsoft.CodeAnalysis.VisualBasic.BoundCall)
				Dim receiverOpt As BoundExpression = boundCall.ReceiverOpt
				Dim arguments As ImmutableArray(Of BoundExpression) = boundCall.Arguments
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(arguments) OrElse AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(receiverOpt)) Then
					Dim spillBuilder As AsyncRewriter.SpillBuilder = New AsyncRewriter.SpillBuilder()
					Dim expressionsWithReceiver As AsyncRewriter.AsyncMethodToClassRewriter.ExpressionsWithReceiver = Me.SpillExpressionsWithReceiver(receiverOpt, True, arguments, spillBuilder)
					boundNode = spillBuilder.BuildSequenceAndFree(Me.F, boundCall.Update(boundCall.Method, boundCall.MethodGroupOpt, expressionsWithReceiver.ReceiverOpt, expressionsWithReceiver.Arguments, boundCall.DefaultArguments, boundCall.ConstantValueOpt, boundCall.IsLValue, boundCall.SuppressObjectClone, boundCall.Type))
				Else
					boundNode = boundCall
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitComplexConditionalAccessReceiver(ByVal node As BoundComplexConditionalAccessReceiver) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public Overrides Function VisitConditionalAccessReceiverPlaceholder(ByVal node As BoundConditionalAccessReceiverPlaceholder) As BoundNode
				If (Me._conditionalAccessReceiverPlaceholderReplacementInfo Is Nothing OrElse Me._conditionalAccessReceiverPlaceholderReplacementInfo.PlaceholderId <> node.PlaceholderId) Then
					Throw ExceptionUtilities.Unreachable
				End If
				Return MyBase.VisitConditionalAccessReceiverPlaceholder(node)
			End Function

			Public Overrides Function VisitConditionalGoto(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundConditionalGoto As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto = DirectCast(MyBase.VisitConditionalGoto(node), Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto)
				Dim condition As BoundExpression = boundConditionalGoto.Condition
				If (condition.Kind = BoundKind.SpillSequence) Then
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(condition, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundNode = Me.RewriteSpillSequenceIntoBlock(boundSpillSequence, False, New BoundStatement() { node.Update(boundSpillSequence.ValueOpt, node.JumpIfTrue, node.Label) })
				Else
					boundNode = boundConditionalGoto
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitConversion(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundConversion) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = DirectCast(MyBase.VisitConversion(node), Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
				Dim operand As BoundExpression = boundConversion.Operand
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(operand)) Then
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(operand, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundNode = AsyncRewriter.AsyncMethodToClassRewriter.SpillSequenceWithNewValue(boundSpillSequence, boundConversion.Update(boundSpillSequence.ValueOpt, boundConversion.ConversionKind, boundConversion.Checked, boundConversion.ExplicitCastInCode, boundConversion.ConstantValueOpt, boundConversion.ExtendedInfoOpt, boundConversion.Type))
				Else
					boundNode = boundConversion
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitDelegateCreationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundDelegateCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression = DirectCast(MyBase.VisitDelegateCreationExpression(node), Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression)
				Dim receiverOpt As BoundExpression = boundDelegateCreationExpression.ReceiverOpt
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(receiverOpt)) Then
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(receiverOpt, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundNode = AsyncRewriter.AsyncMethodToClassRewriter.SpillSequenceWithNewValue(boundSpillSequence, boundDelegateCreationExpression.Update(boundSpillSequence.ValueOpt, boundDelegateCreationExpression.Method, boundDelegateCreationExpression.RelaxationLambdaOpt, boundDelegateCreationExpression.RelaxationReceiverPlaceholderOpt, boundDelegateCreationExpression.MethodGroupOpt, boundDelegateCreationExpression.Type))
				Else
					boundNode = boundDelegateCreationExpression
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitDirectCast(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundDirectCast As Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast = DirectCast(MyBase.VisitDirectCast(node), Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast)
				Dim operand As BoundExpression = boundDirectCast.Operand
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(operand)) Then
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(operand, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundNode = AsyncRewriter.AsyncMethodToClassRewriter.SpillSequenceWithNewValue(boundSpillSequence, boundDirectCast.Update(boundSpillSequence.ValueOpt, boundDirectCast.ConversionKind, boundDirectCast.SuppressVirtualCalls, boundDirectCast.ConstantValueOpt, boundDirectCast.RelaxationLambdaOpt, boundDirectCast.Type))
				Else
					boundNode = boundDirectCast
				End If
				Return boundNode
			End Function

			Public Function VisitExpression(ByVal expression As BoundExpression) As BoundExpression
				Return DirectCast(Me.Visit(expression), BoundExpression)
			End Function

			Public Overrides Function VisitExpressionStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = DirectCast(MyBase.VisitExpressionStatement(node), Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement)
				Dim expression As BoundExpression = boundExpressionStatement.Expression
				If (expression.Kind = BoundKind.SpillSequence) Then
					boundNode = Me.RewriteSpillSequenceIntoBlock(DirectCast(expression, BoundSpillSequence), True)
				Else
					boundNode = boundExpressionStatement
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitFieldAccess(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = DirectCast(MyBase.VisitFieldAccess(node), Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess)
				Dim receiverOpt As BoundExpression = boundFieldAccess.ReceiverOpt
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(receiverOpt)) Then
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(receiverOpt, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundNode = AsyncRewriter.AsyncMethodToClassRewriter.SpillSequenceWithNewValue(boundSpillSequence, boundFieldAccess.Update(boundSpillSequence.ValueOpt, boundFieldAccess.FieldSymbol, boundFieldAccess.IsLValue, boundFieldAccess.SuppressVirtualCalls, Nothing, boundFieldAccess.Type))
				Else
					boundNode = boundFieldAccess
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitLoweredConditionalAccess(ByVal node As BoundLoweredConditionalAccess) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim conditionalAccessReceiverPlaceholderReplacementInfo As AsyncRewriter.AsyncMethodToClassRewriter.ConditionalAccessReceiverPlaceholderReplacementInfo
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim boundComplexConditionalAccessReceiver As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = MyBase.VisitType(node.Type)
				Dim valueOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ReceiverOrCondition), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Dim flag As Boolean = AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(valueOpt)
				Dim conditionalAccessReceiverPlaceholderReplacementInfo1 As AsyncRewriter.AsyncMethodToClassRewriter.ConditionalAccessReceiverPlaceholderReplacementInfo = Me._conditionalAccessReceiverPlaceholderReplacementInfo
				If (node.PlaceholderId = 0) Then
					conditionalAccessReceiverPlaceholderReplacementInfo = Nothing
				Else
					conditionalAccessReceiverPlaceholderReplacementInfo = New AsyncRewriter.AsyncMethodToClassRewriter.ConditionalAccessReceiverPlaceholderReplacementInfo(node.PlaceholderId)
				End If
				Me._conditionalAccessReceiverPlaceholderReplacementInfo = conditionalAccessReceiverPlaceholderReplacementInfo
				Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.WhenNotNull), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Dim flag1 As Boolean = AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(boundExpression3)
				Me._conditionalAccessReceiverPlaceholderReplacementInfo = Nothing
				Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.WhenNullOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Dim flag2 As Boolean = If(boundExpression4 IsNot Nothing, AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(boundExpression4), False)
				Me._conditionalAccessReceiverPlaceholderReplacementInfo = conditionalAccessReceiverPlaceholderReplacementInfo1
				If (Not flag AndAlso Not flag1 AndAlso Not flag2) Then
					boundNode = node.Update(valueOpt, node.CaptureReceiver, node.PlaceholderId, boundExpression3, boundExpression4, typeSymbol)
				ElseIf (flag1 OrElse flag2) Then
					Dim spillBuilder As AsyncRewriter.SpillBuilder = New AsyncRewriter.SpillBuilder()
					If (flag) Then
						Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(valueOpt, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
						spillBuilder.AddSpill(boundSpillSequence)
						valueOpt = boundSpillSequence.ValueOpt
					End If
					If (node.PlaceholderId <> 0) Then
						Dim boundExpression5 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = valueOpt
						If (Not node.CaptureReceiver AndAlso Not conditionalAccessReceiverPlaceholderReplacementInfo.IsSpilled) Then
							boundComplexConditionalAccessReceiver = boundExpression5
							boundExpression1 = boundComplexConditionalAccessReceiver.MakeRValue()
						ElseIf (Not node.CaptureReceiver) Then
							boundComplexConditionalAccessReceiver = Me.SpillValue(boundExpression5, True, True, spillBuilder)
							boundExpression1 = boundComplexConditionalAccessReceiver.MakeRValue()
						Else
							If (Not boundExpression5.Type.IsReferenceType) Then
								boundExpression5 = Me.SpillValue(boundExpression5, True, True, spillBuilder)
							End If
							If (Not conditionalAccessReceiverPlaceholderReplacementInfo.IsSpilled) Then
								Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Nothing
								localSymbol = Me.F.SynthesizedLocal(boundExpression5.Type, SynthesizedLocalKind.LoweringTemp, Nothing)
								spillBuilder.AddLocal(localSymbol)
								boundExpression1 = Me.F.AssignmentExpression(Me.F.Local(localSymbol, True), boundExpression5.MakeRValue())
								boundExpression2 = Me.F.Local(localSymbol, True)
							Else
								boundExpression1 = Me.SpillRValue(boundExpression5.MakeRValue(), spillBuilder)
								boundExpression2 = boundExpression1
							End If
							If (Not boundExpression1.Type.IsReferenceType) Then
								boundComplexConditionalAccessReceiver = New Microsoft.CodeAnalysis.VisualBasic.BoundComplexConditionalAccessReceiver(Me.F.Syntax, boundExpression5, boundExpression2, boundExpression5.Type, False)
							Else
								boundComplexConditionalAccessReceiver = boundExpression2
							End If
						End If
						boundExpression3 = DirectCast((New AsyncRewriter.AsyncMethodToClassRewriter.ConditionalAccessReceiverPlaceholderReplacement(node.PlaceholderId, boundComplexConditionalAccessReceiver, MyBase.RecursionDepth)).Visit(boundExpression3), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
						If (Not boundExpression1.Type.IsReferenceType) Then
							Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = Me.F.ReferenceIsNotNothing(Me.F.[DirectCast](Me.F.[DirectCast](Me.F.Null(), boundExpression1.Type), Me.F.SpecialType(SpecialType.System_Object)))
							boundExpression = Me.F.LogicalOrElse(boundBinaryOperator, Me.F.ReferenceIsNotNothing(Me.F.[DirectCast](boundExpression1, Me.F.SpecialType(SpecialType.System_Object))))
						Else
							boundExpression = Me.F.ReferenceIsNotNothing(boundExpression1)
						End If
					Else
						boundExpression = valueOpt
					End If
					If (boundExpression4 IsNot Nothing) Then
						Dim localSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Me.F.SynthesizedLocal(typeSymbol, SynthesizedLocalKind.LoweringTemp, Nothing)
						spillBuilder.AddLocal(localSymbol1)
						spillBuilder.AddStatement(Me.F.[If](boundExpression, Me.MakeAssignmentStatement(boundExpression3, localSymbol1, spillBuilder), Me.MakeAssignmentStatement(boundExpression4, localSymbol1, spillBuilder)))
						boundNode = spillBuilder.BuildSequenceAndFree(Me.F, Me.F.Local(localSymbol1, False))
					Else
						spillBuilder.AddStatement(Me.F.[If](boundExpression, Me.MakeExpressionStatement(boundExpression3, spillBuilder)))
						boundNode = spillBuilder.BuildSequenceAndFree(Me.F, Nothing)
					End If
				Else
					Dim boundSpillSequence1 As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(valueOpt, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundNode = AsyncRewriter.AsyncMethodToClassRewriter.SpillSequenceWithNewValue(boundSpillSequence1, node.Update(boundSpillSequence1.ValueOpt, node.CaptureReceiver, node.PlaceholderId, boundExpression3, boundExpression4, typeSymbol))
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitLValueToRValueWrapper(ByVal node As BoundLValueToRValueWrapper) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim rValueWrapper As BoundLValueToRValueWrapper = DirectCast(MyBase.VisitLValueToRValueWrapper(node), BoundLValueToRValueWrapper)
				Dim underlyingLValue As BoundExpression = rValueWrapper.UnderlyingLValue
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(underlyingLValue)) Then
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(underlyingLValue, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundNode = AsyncRewriter.AsyncMethodToClassRewriter.SpillSequenceWithNewValue(boundSpillSequence, rValueWrapper.Update(boundSpillSequence.ValueOpt, rValueWrapper.Type))
				Else
					boundNode = rValueWrapper
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitObjectCreationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression = DirectCast(MyBase.VisitObjectCreationExpression(node), Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression)
				Dim arguments As ImmutableArray(Of BoundExpression) = boundObjectCreationExpression.Arguments
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(arguments)) Then
					Dim spillBuilder As AsyncRewriter.SpillBuilder = New AsyncRewriter.SpillBuilder()
					arguments = Me.SpillExpressionList(spillBuilder, arguments, False)
					boundNode = spillBuilder.BuildSequenceAndFree(Me.F, boundObjectCreationExpression.Update(boundObjectCreationExpression.ConstructorOpt, arguments, boundObjectCreationExpression.DefaultArguments, boundObjectCreationExpression.InitializerOpt, boundObjectCreationExpression.Type))
				Else
					boundNode = boundObjectCreationExpression
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitReferenceAssignment(ByVal node As BoundReferenceAssignment) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim byRefLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = node.ByRefLocal
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = byRefLocal.LocalSymbol
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = MyBase.VisitType(node.Type)
				If (Me.Proxies.ContainsKey(localSymbol)) Then
					Dim item As AsyncRewriter.CapturedSymbolOrExpression = Me.Proxies(localSymbol)
					Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
					item.CreateCaptureInitializationCode(Me, instance)
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = item.Materialize(Me, node.IsLValue)
					If (instance.Count <> 0) Then
						instance.Add(boundExpression)
						boundNode = Me.F.Sequence(instance.ToArrayAndFree())
					Else
						instance.Free()
						boundNode = boundExpression
					End If
				Else
					Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = DirectCast(Me.VisitExpression(byRefLocal), Microsoft.CodeAnalysis.VisualBasic.BoundLocal)
					Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(node.LValue)
					If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(boundExpression1)) Then
						Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(boundExpression1, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
						boundNode = AsyncRewriter.AsyncMethodToClassRewriter.SpillSequenceWithNewValue(boundSpillSequence, node.Update(boundLocal, boundSpillSequence.ValueOpt, node.IsLValue, typeSymbol))
					Else
						boundNode = node.Update(boundLocal, boundExpression1, node.IsLValue, typeSymbol)
					End If
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitReturnStatement(ByVal node As BoundReturnStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim expressionOpt As BoundExpression = DirectCast(MyBase.VisitReturnStatement(node), BoundReturnStatement).ExpressionOpt
				If (expressionOpt Is Nothing) Then
					boundNode = Me.F.[Goto](Me._exprReturnLabel, True)
				ElseIf (expressionOpt.Kind <> BoundKind.SpillSequence) Then
					boundNode = Me.F.Block(New BoundStatement() { Me.F.Assignment(Me.F.Local(Me._exprRetValue, True), expressionOpt), Me.F.[Goto](Me._exprReturnLabel, True) })
				Else
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(expressionOpt, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundNode = Me.F.Block(New BoundStatement() { Me.RewriteSpillSequenceIntoBlock(boundSpillSequence, False, New BoundStatement() { Me.F.Assignment(Me.F.Local(Me._exprRetValue, True), boundSpillSequence.ValueOpt) }), Me.F.[Goto](Me._exprReturnLabel, True) })
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitSequence(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundSequence) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSequence = DirectCast(MyBase.VisitSequence(node), Microsoft.CodeAnalysis.VisualBasic.BoundSequence)
				Dim locals As ImmutableArray(Of LocalSymbol) = boundSequence.Locals
				Dim sideEffects As ImmutableArray(Of BoundExpression) = boundSequence.SideEffects
				Dim valueOpt As BoundExpression = boundSequence.ValueOpt
				Dim flag As Boolean = AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(sideEffects)
				Dim flag1 As Boolean = AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(valueOpt)
				If (flag OrElse flag1) Then
					Dim spillBuilder As AsyncRewriter.SpillBuilder = New AsyncRewriter.SpillBuilder()
					spillBuilder.AddLocals(locals)
					If (flag) Then
						Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = sideEffects.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As BoundExpression = enumerator.Current
							spillBuilder.AddStatement(Me.MakeExpressionStatement(current, spillBuilder))
						End While
					End If
					If (flag1) Then
						Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(valueOpt, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
						spillBuilder.AddSpill(boundSpillSequence)
						valueOpt = boundSpillSequence.ValueOpt
					End If
					boundNode = spillBuilder.BuildSequenceAndFree(Me.F, valueOpt)
				Else
					boundNode = boundSequence
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitSequencePointExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundSequencePointExpression As Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointExpression = DirectCast(MyBase.VisitSequencePointExpression(node), Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointExpression)
				Dim expression As BoundExpression = boundSequencePointExpression.Expression
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(expression)) Then
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(expression, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundNode = AsyncRewriter.AsyncMethodToClassRewriter.SpillSequenceWithNewValue(boundSpillSequence, boundSequencePointExpression.Update(boundSpillSequence.ValueOpt, boundSequencePointExpression.Type))
				Else
					boundNode = boundSequencePointExpression
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitSpillSequence(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundStatements As ImmutableArray(Of BoundStatement) = Me.VisitList(Of BoundStatement)(node.Statements)
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(node.ValueOpt)
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = MyBase.VisitType(node.Type)
				If (boundExpression Is Nothing OrElse boundExpression.Kind <> BoundKind.SpillSequence) Then
					boundNode = node.Update(node.Locals, node.SpillFields, boundStatements, boundExpression, typeSymbol)
				Else
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(boundExpression, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundNode = node.Update(Microsoft.CodeAnalysis.ImmutableArrayExtensions.Concat(Of LocalSymbol)(node.Locals, boundSpillSequence.Locals), Microsoft.CodeAnalysis.ImmutableArrayExtensions.Concat(Of FieldSymbol)(node.SpillFields, boundSpillSequence.SpillFields), Microsoft.CodeAnalysis.ImmutableArrayExtensions.Concat(Of BoundStatement)(boundStatements, boundSpillSequence.Statements), boundSpillSequence.ValueOpt, typeSymbol)
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitTernaryConditionalExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundTernaryConditionalExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim boundTernaryConditionalExpression As Microsoft.CodeAnalysis.VisualBasic.BoundTernaryConditionalExpression = DirectCast(MyBase.VisitTernaryConditionalExpression(node), Microsoft.CodeAnalysis.VisualBasic.BoundTernaryConditionalExpression)
				Dim condition As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundTernaryConditionalExpression.Condition
				Dim whenTrue As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundTernaryConditionalExpression.WhenTrue
				Dim whenFalse As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundTernaryConditionalExpression.WhenFalse
				Dim flag As Boolean = AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(condition)
				If (flag OrElse AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(whenTrue) OrElse AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(whenFalse)) Then
					Dim spillBuilder As AsyncRewriter.SpillBuilder = New AsyncRewriter.SpillBuilder()
					If (flag) Then
						condition = Me.SpillRValue(condition, spillBuilder)
					End If
					If (boundTernaryConditionalExpression.Type.IsVoidType()) Then
						spillBuilder.AddStatement(Me.F.[If](condition, Me.MakeExpressionStatement(whenTrue, spillBuilder), Me.MakeExpressionStatement(whenFalse, spillBuilder)))
						boundExpression = Nothing
					Else
						Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Me.F.SynthesizedLocal(boundTernaryConditionalExpression.Type, SynthesizedLocalKind.LoweringTemp, Nothing)
						spillBuilder.AddLocal(localSymbol)
						spillBuilder.AddStatement(Me.F.[If](condition, Me.MakeAssignmentStatement(whenTrue, localSymbol, spillBuilder), Me.MakeAssignmentStatement(whenFalse, localSymbol, spillBuilder)))
						boundExpression = Me.F.Local(localSymbol, False)
					End If
					boundNode = spillBuilder.BuildSequenceAndFree(Me.F, boundExpression)
				Else
					boundNode = boundTernaryConditionalExpression
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitThrowStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundThrowStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundThrowStatement As Microsoft.CodeAnalysis.VisualBasic.BoundThrowStatement = DirectCast(MyBase.VisitThrowStatement(node), Microsoft.CodeAnalysis.VisualBasic.BoundThrowStatement)
				Dim expressionOpt As BoundExpression = boundThrowStatement.ExpressionOpt
				If (expressionOpt Is Nothing OrElse expressionOpt.Kind <> BoundKind.SpillSequence) Then
					boundNode = boundThrowStatement
				Else
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(expressionOpt, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundNode = Me.RewriteSpillSequenceIntoBlock(boundSpillSequence, False, New BoundStatement() { boundThrowStatement.Update(boundSpillSequence.ValueOpt) })
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitTryCast(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundTryCast) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundTryCast As Microsoft.CodeAnalysis.VisualBasic.BoundTryCast = DirectCast(MyBase.VisitTryCast(node), Microsoft.CodeAnalysis.VisualBasic.BoundTryCast)
				Dim operand As BoundExpression = boundTryCast.Operand
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(operand)) Then
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(operand, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundNode = AsyncRewriter.AsyncMethodToClassRewriter.SpillSequenceWithNewValue(boundSpillSequence, boundTryCast.Update(boundSpillSequence.ValueOpt, boundTryCast.ConversionKind, boundTryCast.ConstantValueOpt, boundTryCast.RelaxationLambdaOpt, boundTryCast.Type))
				Else
					boundNode = boundTryCast
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitTypeOf(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundTypeOf) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundTypeOf As Microsoft.CodeAnalysis.VisualBasic.BoundTypeOf = DirectCast(MyBase.VisitTypeOf(node), Microsoft.CodeAnalysis.VisualBasic.BoundTypeOf)
				Dim operand As BoundExpression = boundTypeOf.Operand
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(operand)) Then
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(operand, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundNode = AsyncRewriter.AsyncMethodToClassRewriter.SpillSequenceWithNewValue(boundSpillSequence, boundTypeOf.Update(boundSpillSequence.ValueOpt, boundTypeOf.IsTypeOfIsNotExpression, boundTypeOf.TargetType, boundTypeOf.Type))
				Else
					boundNode = boundTypeOf
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitUnaryOperator(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundUnaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator = DirectCast(MyBase.VisitUnaryOperator(node), Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator)
				Dim operand As BoundExpression = boundUnaryOperator.Operand
				If (AsyncRewriter.AsyncMethodToClassRewriter.NeedsSpill(operand)) Then
					Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = DirectCast(operand, Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence)
					boundNode = AsyncRewriter.AsyncMethodToClassRewriter.SpillSequenceWithNewValue(boundSpillSequence, boundUnaryOperator.Update(boundUnaryOperator.OperatorKind, boundSpillSequence.ValueOpt, boundUnaryOperator.Checked, boundUnaryOperator.ConstantValueOpt, boundUnaryOperator.Type))
				Else
					boundNode = boundUnaryOperator
				End If
				Return boundNode
			End Function

			Private NotInheritable Class ConditionalAccessReceiverPlaceholderReplacement
				Inherits BoundTreeRewriterWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
				Private ReadOnly _placeholderId As Integer

				Private ReadOnly _replaceWith As BoundExpression

				Private _replaced As Boolean

				Public ReadOnly Property Replaced As Boolean
					Get
						Return Me._replaced
					End Get
				End Property

				Public Sub New(ByVal placeholderId As Integer, ByVal replaceWith As BoundExpression, ByVal recursionDepth As Integer)
					MyBase.New(recursionDepth)
					Me._placeholderId = placeholderId
					Me._replaceWith = replaceWith
				End Sub

				Public Overrides Function VisitConditionalAccessReceiverPlaceholder(ByVal node As BoundConditionalAccessReceiverPlaceholder) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
					Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
					If (Me._placeholderId <> node.PlaceholderId) Then
						boundNode = node
					Else
						Me._replaced = True
						Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me._replaceWith
						boundNode = If(Not node.IsLValue, boundExpression.MakeRValue(), boundExpression)
					End If
					Return boundNode
				End Function
			End Class

			Private Class ConditionalAccessReceiverPlaceholderReplacementInfo
				Public ReadOnly PlaceholderId As Integer

				Public IsSpilled As Boolean

				Public Sub New(ByVal placeholderId As Integer)
					MyBase.New()
					Me.PlaceholderId = placeholderId
					Me.IsSpilled = False
				End Sub
			End Class

			Private Structure ExpressionsWithReceiver
				Public ReadOnly ReceiverOpt As BoundExpression

				Public ReadOnly Arguments As ImmutableArray(Of BoundExpression)

				Public Sub New(ByVal receiverOpt As BoundExpression, ByVal arguments As ImmutableArray(Of BoundExpression))
					Me = New AsyncRewriter.AsyncMethodToClassRewriter.ExpressionsWithReceiver() With
					{
						.ReceiverOpt = receiverOpt,
						.Arguments = arguments
					}
				End Sub
			End Structure
		End Class

		Private Class CapturedArrayAccessExpression
			Inherits AsyncRewriter.CapturedSymbolOrExpression
			Friend ReadOnly ArrayPointer As AsyncRewriter.CapturedSymbolOrExpression

			Friend ReadOnly Indices As ImmutableArray(Of AsyncRewriter.CapturedSymbolOrExpression)

			Public Sub New(ByVal arrayPointer As AsyncRewriter.CapturedSymbolOrExpression, ByVal indices As ImmutableArray(Of AsyncRewriter.CapturedSymbolOrExpression))
				MyBase.New()
				Me.ArrayPointer = arrayPointer
				Me.Indices = indices
			End Sub

			Friend Overrides Sub CreateCaptureInitializationCode(ByVal rewriter As AsyncRewriter.AsyncMethodToClassRewriter, ByVal prologue As ArrayBuilder(Of BoundExpression))
				Me.ArrayPointer.CreateCaptureInitializationCode(rewriter, prologue)
				Dim enumerator As ImmutableArray(Of AsyncRewriter.CapturedSymbolOrExpression).Enumerator = Me.Indices.GetEnumerator()
				While enumerator.MoveNext()
					enumerator.Current.CreateCaptureInitializationCode(rewriter, prologue)
				End While
			End Sub

			Friend Overrides Function Materialize(ByVal rewriter As AsyncRewriter.AsyncMethodToClassRewriter, ByVal isLValue As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim indices As ImmutableArray(Of AsyncRewriter.CapturedSymbolOrExpression) = Me.Indices
				Dim length As Integer = indices.Length
				Dim boundExpressionArray(length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					boundExpressionArray(num1) = indices(num1).Materialize(rewriter, False)
					num1 = num1 + 1
				Loop While num1 <= num
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.ArrayPointer.Materialize(rewriter, False)
				Dim elementType As TypeSymbol = DirectCast(boundExpression.Type, ArrayTypeSymbol).ElementType
				Return rewriter.F.ArrayAccess(boundExpression, isLValue, boundExpressionArray)
			End Function
		End Class

		Private Class CapturedConstantExpression
			Inherits AsyncRewriter.CapturedSymbolOrExpression
			Private ReadOnly _constValue As ConstantValue

			Private ReadOnly _type As TypeSymbol

			Public Sub New(ByVal constValue As ConstantValue, ByVal type As TypeSymbol)
				MyBase.New()
				Me._constValue = constValue
				Me._type = type
			End Sub

			Friend Overrides Sub CreateCaptureInitializationCode(ByVal rewriter As AsyncRewriter.AsyncMethodToClassRewriter, ByVal prologue As ArrayBuilder(Of BoundExpression))
			End Sub

			Friend Overrides Function Materialize(ByVal rewriter As AsyncRewriter.AsyncMethodToClassRewriter, ByVal isLValue As Boolean) As BoundExpression
				Return New BoundLiteral(rewriter.F.Syntax, Me._constValue, Me._type)
			End Function
		End Class

		Private Class CapturedFieldAccessExpression
			Inherits AsyncRewriter.CapturedSymbolOrExpression
			Friend ReadOnly ReceiverOpt As AsyncRewriter.CapturedSymbolOrExpression

			Friend ReadOnly Field As FieldSymbol

			Public Sub New(ByVal receiverOpt As AsyncRewriter.CapturedSymbolOrExpression, ByVal field As FieldSymbol)
				MyBase.New()
				Me.ReceiverOpt = receiverOpt
				Me.Field = field
			End Sub

			Friend Overrides Sub CreateCaptureInitializationCode(ByVal rewriter As AsyncRewriter.AsyncMethodToClassRewriter, ByVal prologue As ArrayBuilder(Of BoundExpression))
				If (Me.ReceiverOpt IsNot Nothing) Then
					Me.ReceiverOpt.CreateCaptureInitializationCode(rewriter, prologue)
				End If
			End Sub

			Friend Overrides Function Materialize(ByVal rewriter As AsyncRewriter.AsyncMethodToClassRewriter, ByVal isLValue As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				If (Me.ReceiverOpt IsNot Nothing) Then
					boundExpression = Me.ReceiverOpt.Materialize(rewriter, Me.Field.ContainingType.IsValueType)
				End If
				Return rewriter.F.Field(boundExpression, Me.Field, isLValue)
			End Function
		End Class

		Private Class CapturedLocalSymbol
			Inherits AsyncRewriter.SingleFieldCapture
			Friend ReadOnly Local As LocalSymbol

			Public Sub New(ByVal field As FieldSymbol, ByVal local As LocalSymbol)
				MyBase.New(field)
				Me.Local = local
			End Sub

			Friend Overrides Sub AddProxyFieldsForStateMachineScope(ByVal proxyFields As ArrayBuilder(Of FieldSymbol))
				proxyFields.Add(Me.Field)
			End Sub

			Friend Overrides Sub CreateCaptureInitializationCode(ByVal rewriter As AsyncRewriter.AsyncMethodToClassRewriter, ByVal prologue As ArrayBuilder(Of BoundExpression))
			End Sub
		End Class

		Private Class CapturedParameterSymbol
			Inherits AsyncRewriter.SingleFieldCapture
			Public Sub New(ByVal field As FieldSymbol)
				MyBase.New(field)
			End Sub

			Friend Overrides Sub CreateCaptureInitializationCode(ByVal rewriter As AsyncRewriter.AsyncMethodToClassRewriter, ByVal prologue As ArrayBuilder(Of BoundExpression))
			End Sub
		End Class

		Private Class CapturedRValueExpression
			Inherits AsyncRewriter.SingleFieldCapture
			Friend ReadOnly Expression As BoundExpression

			Public Sub New(ByVal field As FieldSymbol, ByVal expr As BoundExpression)
				MyBase.New(field)
				Me.Expression = expr
			End Sub

			Friend Overrides Sub CreateCaptureInitializationCode(ByVal rewriter As AsyncRewriter.AsyncMethodToClassRewriter, ByVal prologue As ArrayBuilder(Of BoundExpression))
				prologue.Add(rewriter.ProcessRewrittenAssignmentOperator(rewriter.F.AssignmentExpression(Me.Materialize(rewriter, True), rewriter.VisitExpression(Me.Expression))))
			End Sub
		End Class

		Friend MustInherit Class CapturedSymbolOrExpression
			Protected Sub New()
				MyBase.New()
			End Sub

			Friend Overridable Sub AddProxyFieldsForStateMachineScope(ByVal proxyFields As ArrayBuilder(Of FieldSymbol))
			End Sub

			Friend MustOverride Sub CreateCaptureInitializationCode(ByVal rewriter As AsyncRewriter.AsyncMethodToClassRewriter, ByVal prologue As ArrayBuilder(Of BoundExpression))

			Friend MustOverride Function Materialize(ByVal rewriter As AsyncRewriter.AsyncMethodToClassRewriter, ByVal isLValue As Boolean) As BoundExpression
		End Class

		Private MustInherit Class SingleFieldCapture
			Inherits AsyncRewriter.CapturedSymbolOrExpression
			Friend ReadOnly Field As FieldSymbol

			Public Sub New(ByVal field As FieldSymbol)
				MyBase.New()
				Me.Field = field
			End Sub

			Friend Overrides Function Materialize(ByVal rewriter As AsyncRewriter.AsyncMethodToClassRewriter, ByVal isLValue As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim syntax As SyntaxNode = rewriter.F.Syntax
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = rewriter.FramePointer(syntax, Me.Field.ContainingType)
				Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = Me.Field.AsMember(DirectCast(boundExpression.Type, NamedTypeSymbol))
				Return rewriter.F.Field(boundExpression, fieldSymbol, isLValue)
			End Function
		End Class

		Private Structure SpillBuilder
			Private _locals As ArrayBuilder(Of LocalSymbol)

			Private _fields As ArrayBuilder(Of FieldSymbol)

			Private _statements As ArrayBuilder(Of BoundStatement)

			Public ReadOnly Property IsEmpty As Boolean
				Get
					If (Me._locals IsNot Nothing OrElse Me._statements IsNot Nothing) Then
						Return False
					End If
					Return Me._fields Is Nothing
				End Get
			End Property

			Private Shared Sub Add(Of T)(<InAttribute> <Out> ByRef array As ArrayBuilder(Of T), ByVal element As T)
				AsyncRewriter.SpillBuilder.EnsureArrayBuilder(Of T)(array)
				array.Add(element)
			End Sub

			Friend Sub AddFieldWithInitialization(ByVal field As FieldSymbol, ByVal init As BoundStatement)
				AsyncRewriter.SpillBuilder.Add(Of FieldSymbol)(Me._fields, field)
				AsyncRewriter.SpillBuilder.Add(Of BoundStatement)(Me._statements, init)
			End Sub

			Friend Sub AddLocal(ByVal local As LocalSymbol)
				AsyncRewriter.SpillBuilder.Add(Of LocalSymbol)(Me._locals, local)
			End Sub

			Friend Sub AddLocals(ByVal locals As ImmutableArray(Of LocalSymbol))
				AsyncRewriter.SpillBuilder.AddRange(Of LocalSymbol)(Me._locals, locals)
			End Sub

			Private Shared Sub AddRange(Of T)(<InAttribute> <Out> ByRef array As ArrayBuilder(Of T), ByVal other As ArrayBuilder(Of T))
				If (other IsNot Nothing AndAlso other.Count <> 0) Then
					AsyncRewriter.SpillBuilder.EnsureArrayBuilder(Of T)(array)
					array.AddRange(other)
				End If
			End Sub

			Private Shared Sub AddRange(Of T)(<InAttribute> <Out> ByRef array As ArrayBuilder(Of T), ByVal other As ImmutableArray(Of T))
				If (Not other.IsEmpty) Then
					AsyncRewriter.SpillBuilder.EnsureArrayBuilder(Of T)(array)
					array.AddRange(other)
				End If
			End Sub

			Friend Sub AddSpill(<InAttribute> ByRef spill As AsyncRewriter.SpillBuilder)
				If (Not spill.IsEmpty) Then
					AsyncRewriter.SpillBuilder.AddRange(Of LocalSymbol)(Me._locals, spill._locals)
					AsyncRewriter.SpillBuilder.AddRange(Of FieldSymbol)(Me._fields, spill._fields)
					AsyncRewriter.SpillBuilder.AddRange(Of BoundStatement)(Me._statements, spill._statements)
				End If
			End Sub

			Friend Sub AddSpill(ByVal spill As BoundSpillSequence)
				AsyncRewriter.SpillBuilder.AddRange(Of LocalSymbol)(Me._locals, spill.Locals)
				AsyncRewriter.SpillBuilder.AddRange(Of FieldSymbol)(Me._fields, spill.SpillFields)
				AsyncRewriter.SpillBuilder.AddRange(Of BoundStatement)(Me._statements, spill.Statements)
			End Sub

			Friend Sub AddStatement(ByVal statement As BoundStatement)
				AsyncRewriter.SpillBuilder.Add(Of BoundStatement)(Me._statements, statement)
			End Sub

			Friend Sub AssumeFieldsIfNeeded(<InAttribute> <Out> ByRef expression As BoundSpillSequence)
				If (Not expression.SpillFields.IsEmpty) Then
					AsyncRewriter.SpillBuilder.AddRange(Of FieldSymbol)(Me._fields, expression.SpillFields)
					expression = expression.Update(expression.Locals, ImmutableArray(Of FieldSymbol).Empty, expression.Statements, expression.ValueOpt, expression.Type)
				End If
			End Sub

			Friend Function BuildSequenceAndFree(ByVal F As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory, ByVal expression As BoundExpression) As BoundExpression
				Dim empty As ImmutableArray(Of LocalSymbol)
				Dim immutableAndFree As ImmutableArray(Of FieldSymbol)
				Dim boundStatements As ImmutableArray(Of BoundStatement)
				If (Not Me.IsEmpty) Then
					Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = F
					If (Me._locals Is Nothing) Then
						empty = ImmutableArray(Of LocalSymbol).Empty
					Else
						empty = Me._locals.ToImmutableAndFree()
					End If
					If (Me._fields Is Nothing) Then
						immutableAndFree = ImmutableArray(Of FieldSymbol).Empty
					Else
						immutableAndFree = Me._fields.ToImmutableAndFree()
					End If
					If (Me._statements Is Nothing) Then
						boundStatements = ImmutableArray(Of BoundStatement).Empty
					Else
						boundStatements = Me._statements.ToImmutableAndFree()
					End If
					expression = syntheticBoundNodeFactory.SpillSequence(empty, immutableAndFree, boundStatements, expression)
					Me._locals = Nothing
					Me._statements = Nothing
					Me._fields = Nothing
				End If
				Return expression
			End Function

			Private Shared Sub EnsureArrayBuilder(Of T)(<InAttribute> <Out> ByRef array As ArrayBuilder(Of T))
				If (array Is Nothing) Then
					array = ArrayBuilder(Of T).GetInstance()
				End If
			End Sub

			Friend Sub Free()
				If (Me._locals IsNot Nothing) Then
					Me._locals.Free()
					Me._locals = Nothing
				End If
				If (Me._statements IsNot Nothing) Then
					Me._statements.Free()
					Me._statements = Nothing
				End If
				If (Me._fields IsNot Nothing) Then
					Me._fields.Free()
					Me._fields = Nothing
				End If
			End Sub
		End Structure

		Private Class SpillFieldAllocator
			Private ReadOnly _F As SyntheticBoundNodeFactory

			Private ReadOnly _allocatedFields As KeyedStack(Of TypeSymbol, FieldSymbol)

			Private ReadOnly _realizedSpills As HashSet(Of FieldSymbol)

			Private _nextHoistedFieldId As Integer

			Friend Sub New(ByVal f As SyntheticBoundNodeFactory)
				MyBase.New()
				Me._allocatedFields = New KeyedStack(Of TypeSymbol, FieldSymbol)()
				Me._realizedSpills = New HashSet(Of FieldSymbol)(ReferenceEqualityComparer.Instance)
				Me._F = f
				Me._nextHoistedFieldId = 0
			End Sub

			Friend Function AllocateField(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol
				Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = Nothing
				If (Not Me._allocatedFields.TryPop(type, fieldSymbol)) Then
					Me._nextHoistedFieldId = Me._nextHoistedFieldId + 1
					fieldSymbol = Me._F.StateMachineField(type, Me._F.CurrentMethod, GeneratedNames.ReusableHoistedLocalFieldName(Me._nextHoistedFieldId), Accessibility.Internal)
				End If
				Me._realizedSpills.Add(fieldSymbol)
				Return fieldSymbol
			End Function

			Friend Sub FreeField(ByVal field As FieldSymbol)
				Me._realizedSpills.Remove(field)
				Me._allocatedFields.Push(field.Type, field)
			End Sub
		End Class
	End Class
End Namespace