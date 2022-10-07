Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Collections
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class StateMachineRewriter(Of TProxy)
		Protected ReadOnly Body As BoundStatement

		Protected ReadOnly Method As MethodSymbol

		Protected ReadOnly Diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

		Protected ReadOnly F As SyntheticBoundNodeFactory

		Protected ReadOnly StateMachineType As SynthesizedContainer

		Protected ReadOnly SlotAllocatorOpt As VariableSlotAllocator

		Protected ReadOnly SynthesizedLocalOrdinals As SynthesizedLocalOrdinalsDispenser

		Protected StateField As FieldSymbol

		Protected nonReusableLocalProxies As Dictionary(Of Symbol, TProxy)

		Protected nextFreeHoistedLocalSlot As Integer

		Protected hoistedVariables As IReadOnlySet(Of Symbol)

		Protected InitialParameters As Dictionary(Of Symbol, TProxy)

		Protected MustOverride ReadOnly Property PreserveInitialParameterValues As Boolean

		Friend MustOverride ReadOnly Property TypeMap As TypeSubstitution

		Protected Sub New(ByVal body As BoundStatement, ByVal method As MethodSymbol, ByVal stateMachineType As StateMachineTypeSymbol, ByVal slotAllocatorOpt As VariableSlotAllocator, ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			MyBase.New()
			Me.Body = body
			Me.Method = method
			Me.StateMachineType = stateMachineType
			Me.SlotAllocatorOpt = slotAllocatorOpt
			Me.Diagnostics = diagnostics
			Me.SynthesizedLocalOrdinals = New SynthesizedLocalOrdinalsDispenser()
			Me.nonReusableLocalProxies = New Dictionary(Of Symbol, TProxy)()
			Me.F = New SyntheticBoundNodeFactory(method, method, method.ContainingType, body.Syntax, compilationState, diagnostics)
		End Sub

		Protected Function CaptureLocalSymbol(ByVal typeMap As TypeSubstitution, ByVal local As LocalSymbol, ByVal initializers As Dictionary(Of LocalSymbol, BoundExpression)) As TProxy
			Dim tProxy As TProxy
			Dim tProxy1 As TProxy = Nothing
			If (Me.nonReusableLocalProxies.TryGetValue(local, tProxy1)) Then
				tProxy = tProxy1
			ElseIf (Not local.IsByRef) Then
				Dim type As TypeSymbol = local.Type.InternalSubstituteTypeParameters(typeMap).Type
				Dim none As LocalDebugId = LocalDebugId.None
				Dim num As Integer = -1
				If (Not local.SynthesizedKind.IsSlotReusable(Me.F.Compilation.Options.OptimizationLevel)) Then
					Dim declaratorSyntax As SyntaxNode = local.GetDeclaratorSyntax()
					Dim num1 As Integer = Me.Method.CalculateLocalSyntaxOffset(declaratorSyntax.SpanStart, declaratorSyntax.SyntaxTree)
					Dim num2 As Integer = Me.SynthesizedLocalOrdinals.AssignLocalOrdinal(local.SynthesizedKind, num1)
					none = New LocalDebugId(num1, num2)
					Dim num3 As Integer = -1
					If (Me.SlotAllocatorOpt IsNot Nothing AndAlso Me.SlotAllocatorOpt.TryGetPreviousHoistedLocalSlotIndex(declaratorSyntax, Me.F.CompilationState.ModuleBuilderOpt.Translate(type, declaratorSyntax, Me.Diagnostics.DiagnosticBag), local.SynthesizedKind, none, Me.Diagnostics.DiagnosticBag, num3)) Then
						num = num3
					End If
				End If
				If (num = -1) Then
					num = Me.nextFreeHoistedLocalSlot
					Me.nextFreeHoistedLocalSlot = Me.nextFreeHoistedLocalSlot + 1
				End If
				tProxy1 = Me.CreateByValLocalCapture(Me.MakeHoistedFieldForLocal(local, type, num, none), local)
				Me.nonReusableLocalProxies.Add(local, tProxy1)
				tProxy = tProxy1
			Else
				tProxy1 = Me.CreateByRefLocalCapture(typeMap, local, initializers)
				Me.nonReusableLocalProxies.Add(local, tProxy1)
				tProxy = tProxy1
			End If
			Return tProxy
		End Function

		Protected Function CaptureParameterSymbol(ByVal typeMap As TypeSubstitution, ByVal parameter As ParameterSymbol) As TProxy
			Dim tProxy As TProxy
			Dim tProxy1 As TProxy
			Dim tProxy2 As TProxy = Nothing
			If (Not Me.nonReusableLocalProxies.TryGetValue(parameter, tProxy2)) Then
				If (Not parameter.IsMe) Then
					Dim type As TypeSymbol = parameter.Type.InternalSubstituteTypeParameters(typeMap).Type
					tProxy2 = Me.CreateParameterCapture(Me.F.StateMachineField(type, Me.Method, GeneratedNames.MakeStateMachineParameterName(parameter.Name), Accessibility.Internal), parameter)
					Me.nonReusableLocalProxies.Add(parameter, tProxy2)
					If (Me.PreserveInitialParameterValues) Then
						Me.InitialParameters.Add(parameter, Me.CreateParameterCapture(Me.F.StateMachineField(type, Me.Method, GeneratedNames.MakeIteratorParameterProxyName(parameter.Name), Accessibility.Internal), parameter))
					End If
				Else
					Dim name As String = parameter.ContainingSymbol.ContainingType.Name
					Dim flag As Boolean = name.StartsWith("_Closure$__", StringComparison.Ordinal)
					tProxy2 = Me.CreateParameterCapture(Me.F.StateMachineField(Me.Method.ContainingType, Me.Method, If(flag, GeneratedNames.MakeStateMachineCapturedClosureMeName(name), GeneratedNames.MakeStateMachineCapturedMeName()), Accessibility.Internal), parameter)
					Me.nonReusableLocalProxies.Add(parameter, tProxy2)
					If (Me.PreserveInitialParameterValues) Then
						tProxy1 = If(Me.Method.ContainingType.IsStructureType(), Me.CreateParameterCapture(Me.F.StateMachineField(Me.Method.ContainingType, Me.Method, GeneratedNames.MakeIteratorParameterProxyName(GeneratedNames.MakeStateMachineCapturedMeName()), Accessibility.Internal), parameter), Me.nonReusableLocalProxies(parameter))
						Me.InitialParameters.Add(parameter, tProxy1)
					End If
				End If
				tProxy = tProxy2
			Else
				tProxy = tProxy2
			End If
			Return tProxy
		End Function

		Friend Sub CloseMethod(ByVal body As BoundStatement)
			Me.F.CloseMethod(Me.RewriteBodyIfNeeded(body, Me.F.TopLevelMethod, Me.F.CurrentMethod))
		End Sub

		Protected Overridable Function CreateByRefLocalCapture(ByVal typeMap As TypeSubstitution, ByVal local As LocalSymbol, ByVal initializers As Dictionary(Of LocalSymbol, BoundExpression)) As TProxy
			Throw ExceptionUtilities.Unreachable
		End Function

		Protected MustOverride Function CreateByValLocalCapture(ByVal field As FieldSymbol, ByVal local As LocalSymbol) As TProxy

		Private Sub CreateNonReusableLocalProxies(ByVal captured As IteratorAndAsyncCaptureWalker.Result, ByRef nextFreeHoistedLocalSlot As Integer)
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Me.StateMachineType.TypeSubstitution
			Me.nextFreeHoistedLocalSlot = If(If(Me.F.Compilation.Options.OptimizationLevel <> OptimizationLevel.Debug, False, Me.SlotAllocatorOpt IsNot Nothing), Me.SlotAllocatorOpt.PreviousHoistedLocalSlotCount, 0)
			Dim enumerator As ArrayBuilder(Of Symbol).Enumerator = captured.CapturedLocals.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Symbol = enumerator.Current
				Dim kind As SymbolKind = current.Kind
				If (kind = SymbolKind.Local) Then
					Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
					If (localSymbol.IsConst OrElse localSymbol.SynthesizedKind = SynthesizedLocalKind.ConditionalBranchDiscriminator) Then
						Continue While
					End If
					Me.CaptureLocalSymbol(typeSubstitution, DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol), captured.ByRefLocalsInitializers)
				ElseIf (kind = SymbolKind.Parameter) Then
					Me.CaptureParameterSymbol(typeSubstitution, DirectCast(current, ParameterSymbol))
				End If
			End While
		End Sub

		Protected MustOverride Function CreateParameterCapture(ByVal field As FieldSymbol, ByVal parameter As ParameterSymbol) As TProxy

		Friend Overridable Function EnsureAllSymbolsAndSignature() As Boolean
			Dim flag As Boolean
			If (Not Me.Method.ReturnType.IsErrorType()) Then
				Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = Me.Method.Parameters.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ParameterSymbol = enumerator.Current
					If (Not current.IsByRef AndAlso Not current.Type.IsErrorType()) Then
						Continue While
					End If
					flag = True
					Return flag
				End While
				flag = False
			Else
				flag = True
			End If
			Return flag
		End Function

		Friend Function EnsureSpecialMember(ByVal member As SpecialMember, ByVal bag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Symbol
			Return Binder.GetSpecialTypeMember(Me.F.Compilation.Assembly, member, Me.Body.Syntax, bag)
		End Function

		Friend Sub EnsureSpecialPropertyGetter(ByVal member As SpecialMember, ByVal bag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(Me.EnsureSpecialMember(member, bag), Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
			If (propertySymbol IsNot Nothing) Then
				Dim getMethod As MethodSymbol = propertySymbol.GetMethod
				If (getMethod Is Nothing) Then
					Binder.ReportDiagnostic(bag, Me.Body.Syntax, ERRID.ERR_NoGetProperty1, New [Object]() { CustomSymbolDisplayFormatter.QualifiedName(propertySymbol) })
					Return
				End If
				Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = getMethod.GetUseSiteInfo()
				Binder.ReportUseSite(bag, Me.Body.Syntax, useSiteInfo)
			End If
		End Sub

		Friend Function EnsureSpecialType(ByVal type As SpecialType, ByVal bag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Symbol
			Return Binder.GetSpecialType(Me.F.Compilation, type, Me.Body.Syntax, bag)
		End Function

		Friend Function EnsureWellKnownMember(ByVal member As WellKnownMember, ByVal bag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Symbol
			Return Binder.GetWellKnownTypeMember(Me.F.Compilation, member, Me.Body.Syntax, bag)
		End Function

		Friend Function EnsureWellKnownType(ByVal type As WellKnownType, ByVal bag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Symbol
			Return Binder.GetWellKnownType(Me.F.Compilation, type, Me.Body.Syntax, bag)
		End Function

		Protected MustOverride Sub GenerateControlFields()

		Private Function GenerateKickoffMethodBody() As BoundBlock
			Dim stateMachineType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Me.F.CurrentMethod = Me.Method
			Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
			instance.Add(SyntheticBoundNodeFactory.HiddenSequencePoint(Nothing))
			If (Me.Method.IsGenericMethod) Then
				stateMachineType = Me.StateMachineType.Construct(Me.Method.TypeArguments)
			Else
				stateMachineType = Me.StateMachineType
			End If
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = stateMachineType
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Me.F.SynthesizedLocal(namedTypeSymbol, SynthesizedLocalKind.LoweringTemp, Nothing)
			Me.InitializeStateMachine(instance, namedTypeSymbol, localSymbol)
			Dim symbols As Dictionary(Of Symbol, TProxy) = If(Me.PreserveInitialParameterValues, Me.InitialParameters, Me.nonReusableLocalProxies)
			Dim boundExpressions As ArrayBuilder(Of BoundExpression) = ArrayBuilder(Of BoundExpression).GetInstance()
			If (Not Me.Method.IsShared AndAlso Me.Method.MeParameter IsNot Nothing) Then
				Dim tProxy As TProxy = Nothing
				If (symbols.TryGetValue(Me.Method.MeParameter, tProxy)) Then
					Me.InitializeParameterWithProxy(Me.Method.MeParameter, tProxy, localSymbol, boundExpressions)
				End If
			End If
			Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = Me.Method.Parameters.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As ParameterSymbol = enumerator.Current
				Dim tProxy1 As TProxy = Nothing
				If (Not symbols.TryGetValue(current, tProxy1)) Then
					Continue While
				End If
				Me.InitializeParameterWithProxy(current, tProxy1, localSymbol, boundExpressions)
			End While
			If (boundExpressions.Count > 0) Then
				instance.Add(Me.F.ExpressionStatement(Me.F.Sequence(boundExpressions.ToArray())))
			End If
			boundExpressions.Free()
			instance.Add(Me.GenerateStateMachineCreation(localSymbol, namedTypeSymbol))
			Return Me.F.Block(ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)(localSymbol), instance.ToImmutableAndFree())
		End Function

		Protected MustOverride Sub GenerateMethodImplementations()

		Protected MustOverride Function GenerateStateMachineCreation(ByVal stateMachineVariable As LocalSymbol, ByVal frameType As NamedTypeSymbol) As BoundStatement

		Protected MustOverride Sub InitializeParameterWithProxy(ByVal parameter As ParameterSymbol, ByVal proxy As TProxy, ByVal stateMachineVariable As LocalSymbol, ByVal initializers As ArrayBuilder(Of BoundExpression))

		Protected MustOverride Sub InitializeStateMachine(ByVal bodyBuilder As ArrayBuilder(Of BoundStatement), ByVal frameType As NamedTypeSymbol, ByVal stateMachineLocal As LocalSymbol)

		Protected Function MakeHoistedFieldForLocal(ByVal local As LocalSymbol, ByVal localType As TypeSymbol, ByVal slotIndex As Integer, ByVal id As LocalDebugId) As FieldSymbol
			Dim str As String
			Dim synthesizedKind As SynthesizedLocalKind = local.SynthesizedKind
			If (synthesizedKind = SynthesizedLocalKind.UserDefined) Then
				str = [String].Concat("$VB$ResumableLocal_", local.Name, "$", Microsoft.VisualBasic.CompilerServices.Conversions.ToString(slotIndex))
			ElseIf (synthesizedKind = SynthesizedLocalKind.[With]) Then
				str = [String].Concat("$W", Microsoft.VisualBasic.CompilerServices.Conversions.ToString(slotIndex))
			Else
				str = If(synthesizedKind <> SynthesizedLocalKind.LambdaDisplayClass, [String].Concat("$S", Microsoft.VisualBasic.CompilerServices.Conversions.ToString(slotIndex)), [String].Concat("$VB$ResumableLocal_$VB$Closure_$", Microsoft.VisualBasic.CompilerServices.Conversions.ToString(slotIndex)))
			End If
			Return Me.F.StateMachineField(localType, Me.Method, str, New LocalSlotDebugInfo(local.SynthesizedKind, id), slotIndex, Accessibility.Internal)
		End Function

		Friend Function OpenMethodImplementation(ByVal interfaceMethod As WellKnownMember, ByVal name As String, ByVal accessibility As Microsoft.CodeAnalysis.Accessibility, Optional ByVal hasMethodBodyDependency As Boolean = False, Optional ByVal associatedProperty As PropertySymbol = Nothing) As SynthesizedMethod
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.F.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(interfaceMethod, False)
			Return Me.OpenMethodImplementation(methodSymbol, name, accessibility, hasMethodBodyDependency, associatedProperty)
		End Function

		Friend Function OpenMethodImplementation(ByVal interfaceMethod As SpecialMember, ByVal name As String, ByVal accessibility As Microsoft.CodeAnalysis.Accessibility, Optional ByVal hasMethodBodyDependency As Boolean = False, Optional ByVal associatedProperty As PropertySymbol = Nothing) As SynthesizedMethod
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Me.F.SpecialMember(interfaceMethod), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			Return Me.OpenMethodImplementation(methodSymbol, name, accessibility, hasMethodBodyDependency, associatedProperty)
		End Function

		Friend Function OpenMethodImplementation(ByVal interfaceType As NamedTypeSymbol, ByVal interfaceMethod As SpecialMember, ByVal name As String, ByVal accessibility As Microsoft.CodeAnalysis.Accessibility, Optional ByVal hasMethodBodyDependency As Boolean = False, Optional ByVal associatedProperty As PropertySymbol = Nothing) As SynthesizedMethod
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Me.F.SpecialMember(interfaceMethod), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).AsMember(interfaceType)
			Return Me.OpenMethodImplementation(methodSymbol, name, accessibility, hasMethodBodyDependency, associatedProperty)
		End Function

		Private Function OpenMethodImplementation(ByVal methodToImplement As MethodSymbol, ByVal methodName As String, ByVal accessibility As Microsoft.CodeAnalysis.Accessibility, Optional ByVal hasMethodBodyDependency As Boolean = False, Optional ByVal associatedProperty As PropertySymbol = Nothing) As SynthesizedMethod
			Dim synthesizedStateMachineDebuggerNonUserCodeMethod As Microsoft.CodeAnalysis.VisualBasic.SynthesizedStateMachineDebuggerNonUserCodeMethod = New Microsoft.CodeAnalysis.VisualBasic.SynthesizedStateMachineDebuggerNonUserCodeMethod(DirectCast(Me.F.CurrentType, StateMachineTypeSymbol), methodName, methodToImplement, Me.F.Syntax, accessibility, hasMethodBodyDependency, associatedProperty)
			Me.F.AddMethod(Me.F.CurrentType, synthesizedStateMachineDebuggerNonUserCodeMethod)
			Me.F.CurrentMethod = synthesizedStateMachineDebuggerNonUserCodeMethod
			Return synthesizedStateMachineDebuggerNonUserCodeMethod
		End Function

		Friend Function OpenMoveNextMethodImplementation(ByVal interfaceMethod As WellKnownMember, ByVal accessibility As Microsoft.CodeAnalysis.Accessibility) As SynthesizedMethod
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.F.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(interfaceMethod, False)
			Return Me.OpenMoveNextMethodImplementation(methodSymbol, accessibility)
		End Function

		Friend Function OpenMoveNextMethodImplementation(ByVal interfaceMethod As SpecialMember, ByVal accessibility As Microsoft.CodeAnalysis.Accessibility) As SynthesizedMethod
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Me.F.SpecialMember(interfaceMethod), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			Return Me.OpenMoveNextMethodImplementation(methodSymbol, accessibility)
		End Function

		Private Function OpenMoveNextMethodImplementation(ByVal methodToImplement As MethodSymbol, ByVal accessibility As Microsoft.CodeAnalysis.Accessibility) As SynthesizedMethod
			Dim synthesizedStateMachineMoveNextMethod As Microsoft.CodeAnalysis.VisualBasic.SynthesizedStateMachineMoveNextMethod = New Microsoft.CodeAnalysis.VisualBasic.SynthesizedStateMachineMoveNextMethod(DirectCast(Me.F.CurrentType, StateMachineTypeSymbol), methodToImplement, Me.F.Syntax, accessibility)
			Me.F.AddMethod(Me.F.CurrentType, synthesizedStateMachineMoveNextMethod)
			Me.F.CurrentMethod = synthesizedStateMachineMoveNextMethod
			Return synthesizedStateMachineMoveNextMethod
		End Function

		Friend Function OpenPropertyImplementation(ByVal interfaceProperty As SpecialMember, ByVal name As String, ByVal accessibility As Microsoft.CodeAnalysis.Accessibility) As MethodSymbol
			Dim getMethod As MethodSymbol = DirectCast(Me.F.SpecialMember(interfaceProperty), PropertySymbol).GetMethod
			Return Me.OpenPropertyImplementation(getMethod, name, accessibility)
		End Function

		Friend Function OpenPropertyImplementation(ByVal interfaceType As NamedTypeSymbol, ByVal interfaceMethod As SpecialMember, ByVal name As String, ByVal accessibility As Microsoft.CodeAnalysis.Accessibility) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Me.F.SpecialMember(interfaceMethod), PropertySymbol).GetMethod.AsMember(interfaceType)
			Return Me.OpenPropertyImplementation(methodSymbol, name, accessibility)
		End Function

		Private Function OpenPropertyImplementation(ByVal getterToImplement As MethodSymbol, ByVal name As String, ByVal accessibility As Microsoft.CodeAnalysis.Accessibility) As MethodSymbol
			Dim synthesizedStateMachineProperty As Microsoft.CodeAnalysis.VisualBasic.SynthesizedStateMachineProperty = New Microsoft.CodeAnalysis.VisualBasic.SynthesizedStateMachineProperty(DirectCast(Me.F.CurrentType, StateMachineTypeSymbol), name, getterToImplement, Me.F.Syntax, accessibility)
			Me.F.AddProperty(Me.F.CurrentType, synthesizedStateMachineProperty)
			Dim getMethod As MethodSymbol = synthesizedStateMachineProperty.GetMethod
			Me.F.AddMethod(Me.F.CurrentType, getMethod)
			Me.F.CurrentMethod = getMethod
			Return getMethod
		End Function

		Protected Function Rewrite() As BoundBlock
			Me.F.OpenNestedType(Me.StateMachineType)
			Me.F.CompilationState.StateMachineImplementationClass(Me.Method) = Me.StateMachineType
			Me.GenerateControlFields()
			If (Me.PreserveInitialParameterValues) Then
				Me.InitialParameters = New Dictionary(Of Symbol, TProxy)()
			End If
			Dim result As IteratorAndAsyncCaptureWalker.Result = IteratorAndAsyncCaptureWalker.Analyze(New FlowAnalysisInfo(Me.F.CompilationState.Compilation, Me.Method, Me.Body), Me.Diagnostics.DiagnosticBag)
			Me.CreateNonReusableLocalProxies(result, Me.nextFreeHoistedLocalSlot)
			Me.hoistedVariables = New OrderedSet(Of Symbol)(result.CapturedLocals)
			Me.GenerateMethodImplementations()
			Return Me.GenerateKickoffMethodBody()
		End Function

		Friend Overridable Function RewriteBodyIfNeeded(ByVal body As BoundStatement, ByVal topMethod As MethodSymbol, ByVal currentMethod As MethodSymbol) As BoundStatement
			Return body
		End Function

		Friend MustInherit Class StateMachineMethodToClassRewriter
			Inherits MethodToClassRewriter(Of TProxy)
			Protected Friend ReadOnly F As SyntheticBoundNodeFactory

			Protected NextState As Integer

			Protected ReadOnly StateField As FieldSymbol

			Protected ReadOnly CachedState As LocalSymbol

			Protected Dispatches As Dictionary(Of LabelSymbol, List(Of Integer))

			Protected ReadOnly FinalizerStateMap As Dictionary(Of Integer, Integer)

			Private _hasFinalizerState As Boolean

			Private _currentFinalizerState As Integer

			Private ReadOnly _hoistedVariables As IReadOnlySet(Of Symbol)

			Private ReadOnly _synthesizedLocalOrdinals As SynthesizedLocalOrdinalsDispenser

			Private ReadOnly _nextFreeHoistedLocalSlot As Integer

			Protected Overrides ReadOnly Property CurrentMethod As MethodSymbol
				Get
					Return Me.F.CurrentMethod
				End Get
			End Property

			Protected MustOverride ReadOnly Property ResumeLabelName As String

			Protected Overrides ReadOnly Property TopLevelMethod As MethodSymbol
				Get
					Return Me.F.TopLevelMethod
				End Get
			End Property

			Protected Overrides ReadOnly Property TypeMap As TypeSubstitution
				Get
					Return Me.F.CurrentType.TypeSubstitution
				End Get
			End Property

			Public Sub New(ByVal F As SyntheticBoundNodeFactory, ByVal stateField As FieldSymbol, ByVal hoistedVariables As IReadOnlySet(Of Symbol), ByVal initialProxies As Dictionary(Of Symbol, TProxy), ByVal synthesizedLocalOrdinals As SynthesizedLocalOrdinalsDispenser, ByVal slotAllocatorOpt As VariableSlotAllocator, ByVal nextFreeHoistedLocalSlot As Integer, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
				MyBase.New(slotAllocatorOpt, F.CompilationState, diagnostics, False)
				Dim enumerator As Dictionary(Of Symbol, TProxy).Enumerator = New Dictionary(Of Symbol, TProxy).Enumerator()
				Me.NextState = 0
				Me.Dispatches = New Dictionary(Of LabelSymbol, List(Of Integer))()
				Me.FinalizerStateMap = New Dictionary(Of Integer, Integer)()
				Me._hasFinalizerState = True
				Me._currentFinalizerState = -1
				Me._hoistedVariables = Nothing
				Me.F = F
				Me.StateField = stateField
				Me.CachedState = F.SynthesizedLocal(F.SpecialType(SpecialType.System_Int32), SynthesizedLocalKind.StateMachineCachedState, F.Syntax)
				Me._hoistedVariables = hoistedVariables
				Me._synthesizedLocalOrdinals = synthesizedLocalOrdinals
				Me._nextFreeHoistedLocalSlot = nextFreeHoistedLocalSlot
				Try
					enumerator = initialProxies.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of Symbol, TProxy) = enumerator.Current
						Me.Proxies.Add(current.Key, current.Value)
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			End Sub

			Friend MustOverride Sub AddProxyFieldsForStateMachineScope(ByVal proxy As TProxy, ByVal proxyFields As ArrayBuilder(Of FieldSymbol))

			Protected Function AddState() As StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter.StateInfo
				Dim nextState As Integer = Me.NextState
				Me.NextState = Me.NextState + 1
				If (Me.Dispatches Is Nothing) Then
					Me.Dispatches = New Dictionary(Of LabelSymbol, List(Of Integer))()
				End If
				If (Not Me._hasFinalizerState) Then
					Me._currentFinalizerState = Me.NextState
					Me.NextState = Me.NextState + 1
					Me._hasFinalizerState = True
				End If
				Dim generatedLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = Me.F.GenerateLabel(Me.ResumeLabelName)
				Me.Dispatches.Add(generatedLabelSymbol, New List(Of Integer)() From
				{
					nextState
				})
				Me.FinalizerStateMap.Add(nextState, Me._currentFinalizerState)
				Return New StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter.StateInfo(nextState, generatedLabelSymbol)
			End Function

			Protected Function Dispatch() As BoundStatement
				Dim item As Func(Of KeyValuePair(Of LabelSymbol, List(Of Integer)), Integer)
				Dim f As SyntheticBoundNodeFactory = Me.F
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me.F.Local(Me.CachedState, False)
				Dim dispatches As Dictionary(Of LabelSymbol, List(Of Integer)) = Me.Dispatches
				If (StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter._Closure$__.$I24-0 Is Nothing) Then
					item = Function(kv As KeyValuePair(Of LabelSymbol, List(Of Integer))) kv.Value(0)
					StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter._Closure$__.$I24-0 = item
				Else
					item = StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter._Closure$__.$I24-0
				End If
				Return f.[Select](boundLocal, dispatches.OrderBy(Of Integer)(item).[Select](Of BoundCaseBlock)(Function(kv As KeyValuePair(Of LabelSymbol, List(Of Integer))) Me.F.SwitchSection(kv.Value, New BoundStatement() { Me.F.[Goto](kv.Key, True) })))
			End Function

			Friend Overrides Function FramePointer(ByVal syntax As Microsoft.CodeAnalysis.SyntaxNode, ByVal frameClass As NamedTypeSymbol) As BoundExpression
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = Me.F.Syntax
				Me.F.Syntax = syntax
				Dim boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference = Me.F.[Me]()
				Me.F.Syntax = syntaxNode
				Return boundMeReference
			End Function

			Protected MustOverride Function GenerateReturn(ByVal finished As Boolean) As BoundStatement

			Friend Function MakeStateMachineScope(ByVal hoistedLocals As ImmutableArray(Of FieldSymbol), ByVal statement As BoundStatement) As BoundBlock
				Return Me.F.Block(New BoundStatement() { (New BoundStateMachineScope(Me.F.Syntax, hoistedLocals, statement, False)).MakeCompilerGenerated() })
			End Function

			Private Function NeedsProxy(ByVal localOrParameter As Symbol) As Boolean
				Return Me._hoistedVariables.Contains(localOrParameter)
			End Function

			Private Function PossibleStateMachineScope(ByVal locals As ImmutableArray(Of LocalSymbol), ByVal wrapped As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				If (Not locals.IsEmpty) Then
					Dim instance As ArrayBuilder(Of FieldSymbol) = ArrayBuilder(Of FieldSymbol).GetInstance()
					Dim enumerator As ImmutableArray(Of LocalSymbol).Enumerator = locals.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As LocalSymbol = enumerator.Current
						If (Not Me.NeedsProxy(current) OrElse current.IsByRef OrElse current.SynthesizedKind <> SynthesizedLocalKind.UserDefined AndAlso current.SynthesizedKind <> SynthesizedLocalKind.LambdaDisplayClass) Then
							Continue While
						End If
						Dim tProxy As TProxy = Nothing
						If (Not Me.Proxies.TryGetValue(current, tProxy)) Then
							Continue While
						End If
						Me.AddProxyFieldsForStateMachineScope(tProxy, instance)
					End While
					Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = wrapped
					If (instance.Count > 0) Then
						boundNode1 = Me.MakeStateMachineScope(instance.ToImmutable(), DirectCast(boundNode1, BoundStatement))
					End If
					instance.Free()
					boundNode = boundNode1
				Else
					boundNode = wrapped
				End If
				Return boundNode
			End Function

			Private Function TryRewriteLocal(ByVal local As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol
				If (Not Me.NeedsProxy(local)) Then
					Dim localSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Nothing
					If (Not Me.LocalMap.TryGetValue(local, localSymbol1)) Then
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = MyBase.VisitType(local.Type)
						If (Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(typeSymbol, local.Type, TypeCompareKind.ConsiderEverything)) Then
							localSymbol1 = Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol.Create(local, typeSymbol)
							Me.LocalMap.Add(local, localSymbol1)
						Else
							localSymbol1 = local
						End If
					End If
					localSymbol = localSymbol1
				Else
					localSymbol = Nothing
				End If
				Return localSymbol
			End Function

			Friend Function TryUnwrapBoundStateMachineScope(ByRef statement As BoundStatement, <Out> ByRef hoistedLocals As ImmutableArray(Of FieldSymbol)) As Boolean
				Dim flag As Boolean
				If (statement.Kind = BoundKind.Block) Then
					Dim statements As ImmutableArray(Of BoundStatement) = DirectCast(statement, BoundBlock).Statements
					If (statements.Length <> 1 OrElse statements(0).Kind <> BoundKind.StateMachineScope) Then
						hoistedLocals = ImmutableArray(Of FieldSymbol).Empty
						flag = False
						Return flag
					End If
					Dim item As BoundStateMachineScope = DirectCast(statements(0), BoundStateMachineScope)
					statement = item.Statement
					hoistedLocals = item.Fields
					flag = True
					Return flag
				End If
				hoistedLocals = ImmutableArray(Of FieldSymbol).Empty
				flag = False
				Return flag
			End Function

			Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				If (node IsNot Nothing) Then
					Dim syntax As SyntaxNode = Me.F.Syntax
					Me.F.Syntax = node.Syntax
					Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.Visit(node)
					Me.F.Syntax = syntax
					boundNode = boundNode1
				Else
					boundNode = node
				End If
				Return boundNode
			End Function

			Public NotOverridable Overrides Function VisitAddressOfOperator(ByVal node As BoundAddressOfOperator) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitAnonymousTypeCreationExpression(ByVal node As BoundAnonymousTypeCreationExpression) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitAnonymousTypeFieldInitializer(ByVal node As BoundAnonymousTypeFieldInitializer) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitAnonymousTypePropertyAccess(ByVal node As BoundAnonymousTypePropertyAccess) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitArrayLiteral(ByVal node As BoundArrayLiteral) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitAttribute(ByVal node As BoundAttribute) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitBadExpression(ByVal node As BoundBadExpression) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitBadVariable(ByVal node As BoundBadVariable) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public Overrides Function VisitBlock(ByVal node As BoundBlock) As BoundNode
				Return Me.PossibleStateMachineScope(node.Locals, MyBase.VisitBlock(node))
			End Function

			Public NotOverridable Overrides Function VisitByRefArgumentPlaceholder(ByVal node As BoundByRefArgumentPlaceholder) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitByRefArgumentWithCopyBack(ByVal node As BoundByRefArgumentWithCopyBack) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public Overrides Function VisitCatchBlock(ByVal node As BoundCatchBlock) As BoundNode
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Nothing
				Dim localOpt As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = node.LocalOpt
				If (localOpt IsNot Nothing) Then
					localSymbol = Me.TryRewriteLocal(localOpt)
				End If
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ExceptionSourceOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ErrorLineNumberOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ExceptionFilterOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
				Return node.Update(localSymbol, boundExpression, boundExpression1, boundExpression2, boundBlock, node.IsSynthesizedAsyncCatchAll)
			End Function

			Public NotOverridable Overrides Function VisitCollectionInitializerExpression(ByVal node As BoundCollectionInitializerExpression) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitCompoundAssignmentTargetPlaceholder(ByVal node As BoundCompoundAssignmentTargetPlaceholder) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitDup(ByVal node As BoundDup) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitGroupTypeInferenceLambda(ByVal node As GroupTypeInferenceLambda) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitLambda(ByVal node As BoundLambda) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitLateAddressOfOperator(ByVal node As BoundLateAddressOfOperator) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitLateBoundArgumentSupportingAssignmentWithCapture(ByVal node As BoundLateBoundArgumentSupportingAssignmentWithCapture) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitLateInvocation(ByVal node As BoundLateInvocation) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitLateMemberAccess(ByVal node As BoundLateMemberAccess) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public Overrides Function VisitMeReference(ByVal node As BoundMeReference) As BoundNode
				Return Me.MaterializeProxy(node, Me.Proxies(Me.TopLevelMethod.MeParameter))
			End Function

			Public NotOverridable Overrides Function VisitMethodGroup(ByVal node As BoundMethodGroup) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public Overrides Function VisitMyBaseReference(ByVal node As BoundMyBaseReference) As BoundNode
				Return Me.MaterializeProxy(node, Me.Proxies(Me.TopLevelMethod.MeParameter))
			End Function

			Public Overrides Function VisitMyClassReference(ByVal node As BoundMyClassReference) As BoundNode
				Return Me.MaterializeProxy(node, Me.Proxies(Me.TopLevelMethod.MeParameter))
			End Function

			Public NotOverridable Overrides Function VisitNewT(ByVal node As BoundNewT) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitNullableIsTrueOperator(ByVal node As BoundNullableIsTrueOperator) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitObjectInitializerExpression(ByVal node As BoundObjectInitializerExpression) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitOmittedArgument(ByVal node As BoundOmittedArgument) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitParenthesized(ByVal node As BoundParenthesized) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitPropertyAccess(ByVal node As BoundPropertyAccess) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitPropertyGroup(ByVal node As BoundPropertyGroup) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitQueryLambda(ByVal node As BoundQueryLambda) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitRangeVariable(ByVal node As BoundRangeVariable) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public Overrides Function VisitTryStatement(ByVal node As BoundTryStatement) As BoundNode
				Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
				Dim func As Func(Of List(Of Integer), IEnumerable(Of Integer))
				Dim variable
				Dim func1
				Dim func2
				Dim dispatches As Dictionary(Of LabelSymbol, List(Of Integer)) = Me.Dispatches
				Dim num As Integer = Me._currentFinalizerState
				Dim flag As Boolean = Me._hasFinalizerState
				Me.Dispatches = Nothing
				Me._currentFinalizerState = -1
				Me._hasFinalizerState = False
				Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = Me.F.Block(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { DirectCast(Me.Visit(node.TryBlock), Microsoft.CodeAnalysis.VisualBasic.BoundStatement) })
				Dim generatedLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = Nothing
				If (Me.Dispatches IsNot Nothing) Then
					generatedLabelSymbol = Me.F.GenerateLabel("tryDispatch")
					If (Not Me._hasFinalizerState) Then
						boundBlock1 = Me.F.Block(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { SyntheticBoundNodeFactory.HiddenSequencePoint(Nothing), Me.Dispatch(), boundBlock1 })
					Else
						Dim generatedLabelSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = Me.F.GenerateLabel("finalizer")
						Me.Dispatches.Add(generatedLabelSymbol1, New List(Of Integer)() From
						{
							Me._currentFinalizerState
						})
						Dim generatedLabelSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = Me.F.GenerateLabel("skipFinalizer")
						boundBlock1 = Me.F.Block(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { SyntheticBoundNodeFactory.HiddenSequencePoint(Nothing), Me.Dispatch(), Me.F.[Goto](generatedLabelSymbol2, True), Me.F.Label(generatedLabelSymbol1), Me.F.Assignment(Me.F.Field(Me.F.[Me](), Me.StateField, True), Me.F.AssignmentExpression(Me.F.Local(Me.CachedState, True), Me.F.Literal(StateMachineStates.NotStartedStateMachine))), Me.GenerateReturn(False), Me.F.Label(generatedLabelSymbol2), boundBlock1 })
					End If
					If (dispatches Is Nothing) Then
						dispatches = New Dictionary(Of LabelSymbol, List(Of Integer))()
					End If
					Dim labelSymbols As Dictionary(Of LabelSymbol, List(Of Integer)) = dispatches
					Dim generatedLabelSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = generatedLabelSymbol
					Dim values As Dictionary(Of !0, !1).ValueCollection = Me.Dispatches.Values
					If (StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter._Closure$__.$I32-0 Is Nothing) Then
						func = Function(kv As List(Of Integer)) kv
						StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter._Closure$__.$I32-0 = func
					Else
						func = StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter._Closure$__.$I32-0
					End If
					If (StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter._Closure$__.$I32-1 Is Nothing) Then
						variable = Function(kv As List(Of Integer), n As Integer) New With { Key .kv = kv, Key .n = n }
						StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter._Closure$__.$I32-1 = variable
					Else
						variable = StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter._Closure$__.$I32-1
					End If
					Dim collection = values.SelectMany(func, variable)
					If (StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter._Closure$__.$I32-2 Is Nothing) Then
						func1 = Function(argument0) argument0.n
						StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter._Closure$__.$I32-2 = func1
					Else
						func1 = StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter._Closure$__.$I32-2
					End If
					Dim collection1 = collection.OrderBy(func1)
					If (StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter._Closure$__.$I32-3 Is Nothing) Then
						func2 = Function(argument1) argument1.n
						StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter._Closure$__.$I32-3 = func2
					Else
						func2 = StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter._Closure$__.$I32-3
					End If
					labelSymbols.Add(generatedLabelSymbol3, New List(Of Integer)(collection1.[Select](func2)))
				End If
				Me._hasFinalizerState = flag
				Me._currentFinalizerState = num
				Me.Dispatches = dispatches
				Dim boundCatchBlocks As ImmutableArray(Of BoundCatchBlock) = Me.VisitList(Of BoundCatchBlock)(node.CatchBlocks)
				If (node.FinallyBlockOpt Is Nothing) Then
					boundBlock = Nothing
				Else
					boundBlock = Me.F.Block(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { SyntheticBoundNodeFactory.HiddenSequencePoint(Nothing), Me.F.[If](Me.F.IntLessThan(Me.F.Local(Me.CachedState, False), Me.F.Literal(StateMachineStates.FirstUnusedState)), DirectCast(Me.Visit(node.FinallyBlockOpt), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)), SyntheticBoundNodeFactory.HiddenSequencePoint(Nothing) })
				End If
				Dim boundBlock2 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = boundBlock
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = node.Update(boundBlock1, boundCatchBlocks, boundBlock2, node.ExitLabelOpt)
				If (generatedLabelSymbol IsNot Nothing) Then
					boundStatement = Me.F.Block(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { SyntheticBoundNodeFactory.HiddenSequencePoint(Nothing), Me.F.Label(generatedLabelSymbol), boundStatement })
				End If
				Return boundStatement
			End Function

			Public NotOverridable Overrides Function VisitTypeArguments(ByVal node As BoundTypeArguments) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitTypeOrValueExpression(ByVal node As BoundTypeOrValueExpression) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitUnboundLambda(ByVal node As UnboundLambda) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitUserDefinedBinaryOperator(ByVal node As BoundUserDefinedBinaryOperator) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitUserDefinedConversion(ByVal node As BoundUserDefinedConversion) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitUserDefinedShortCircuitingOperator(ByVal node As BoundUserDefinedShortCircuitingOperator) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitUserDefinedUnaryOperator(ByVal node As BoundUserDefinedUnaryOperator) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitValueTypeMeReference(ByVal node As BoundValueTypeMeReference) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitWithLValueExpressionPlaceholder(ByVal node As BoundWithLValueExpressionPlaceholder) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitWithRValueExpressionPlaceholder(ByVal node As BoundWithRValueExpressionPlaceholder) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitXmlAttribute(ByVal node As BoundXmlAttribute) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitXmlCData(ByVal node As BoundXmlCData) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitXmlComment(ByVal node As BoundXmlComment) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitXmlDeclaration(ByVal node As BoundXmlDeclaration) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitXmlDocument(ByVal node As BoundXmlDocument) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitXmlElement(ByVal node As BoundXmlElement) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitXmlEmbeddedExpression(ByVal node As BoundXmlEmbeddedExpression) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitXmlMemberAccess(ByVal node As BoundXmlMemberAccess) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitXmlName(ByVal node As BoundXmlName) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitXmlNamespace(ByVal node As BoundXmlNamespace) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function VisitXmlProcessingInstruction(ByVal node As BoundXmlProcessingInstruction) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Protected Structure StateInfo
				Public ReadOnly Number As Integer

				Public ReadOnly ResumeLabel As GeneratedLabelSymbol

				Public Sub New(ByVal stateNumber As Integer, ByVal resumeLabel As GeneratedLabelSymbol)
					Me = New StateMachineRewriter(Of TProxy).StateMachineMethodToClassRewriter.StateInfo() With
					{
						.Number = stateNumber,
						.ResumeLabel = resumeLabel
					}
				End Sub
			End Structure
		End Class
	End Class
End Namespace