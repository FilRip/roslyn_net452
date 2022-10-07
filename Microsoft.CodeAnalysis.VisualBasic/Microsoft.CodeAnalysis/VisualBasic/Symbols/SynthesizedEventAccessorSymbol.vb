Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Reflection
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SynthesizedEventAccessorSymbol
		Inherits SynthesizedAccessor(Of SourceEventSymbol)
		Private _lazyReturnType As TypeSymbol

		Private _lazyParameters As ImmutableArray(Of ParameterSymbol)

		Private _lazyExplicitImplementations As ImmutableArray(Of MethodSymbol)

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				If (Me._lazyExplicitImplementations.IsDefault) Then
					ImmutableInterlocked.InterlockedInitialize(Of MethodSymbol)(Me._lazyExplicitImplementations, Me.SourceEvent.GetAccessorImplementations(Me.MethodKind))
				End If
				Return Me._lazyExplicitImplementations
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
			Get
				Dim methodImplAttribute As MethodImplAttributes = MyBase.ImplementationAttributes
				If (Not MyBase.IsMustOverride AndAlso Not Me.SourceEvent.IsWindowsRuntimeEvent AndAlso Not MyBase.ContainingType.IsStructureType() AndAlso Me.DeclaringCompilation.GetWellKnownTypeMember(WellKnownMember.System_Threading_Interlocked__CompareExchange_T) Is Nothing) Then
					methodImplAttribute = methodImplAttribute Or MethodImplAttributes.Synchronized
				End If
				Return methodImplAttribute
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				If (Me.MethodKind <> Microsoft.CodeAnalysis.MethodKind.EventAdd) Then
					Return True
				End If
				Return Not Me.m_propertyOrEvent.IsWindowsRuntimeEvent
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Dim type As TypeSymbol
				If (Me._lazyParameters.IsDefault) Then
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					If (Me.MethodKind <> Microsoft.CodeAnalysis.MethodKind.EventRemove OrElse Not Me.m_propertyOrEvent.IsWindowsRuntimeEvent) Then
						type = Me.SourceEvent.Type
					Else
						type = Me.DeclaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken)
						Dim useSiteInfoForWellKnownType As UseSiteInfo(Of AssemblySymbol) = Microsoft.CodeAnalysis.VisualBasic.Binder.GetUseSiteInfoForWellKnownType(type)
						Dim locations As ImmutableArray(Of Location) = MyBase.Locations
						instance.Add(useSiteInfoForWellKnownType, locations(0))
					End If
					Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = ImmutableArray.Create(Of ParameterSymbol)(New SynthesizedParameterSymbol(Me, type, 0, False, "obj"))
					DirectCast(Me.ContainingModule, SourceModuleSymbol).AtomicStoreArrayAndDiagnostics(Of ParameterSymbol)(Me._lazyParameters, parameterSymbols, instance)
					instance.Free()
				End If
				Return Me._lazyParameters
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Dim wellKnownType As TypeSymbol
				Dim useSiteInfoForWellKnownType As UseSiteInfo(Of AssemblySymbol)
				If (Me._lazyReturnType Is Nothing) Then
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
					If (Not Me.IsSub) Then
						wellKnownType = declaringCompilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken)
						useSiteInfoForWellKnownType = Microsoft.CodeAnalysis.VisualBasic.Binder.GetUseSiteInfoForWellKnownType(wellKnownType)
					Else
						wellKnownType = declaringCompilation.GetSpecialType(SpecialType.System_Void)
						useSiteInfoForWellKnownType = If(Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.EventRemove, Microsoft.CodeAnalysis.VisualBasic.Binder.GetUseSiteInfoForSpecialType(wellKnownType, False), New UseSiteInfo(Of AssemblySymbol)())
					End If
					Dim locations As ImmutableArray(Of Location) = MyBase.Locations
					instance.Add(useSiteInfoForWellKnownType, locations(0))
					DirectCast(Me.ContainingModule, SourceModuleSymbol).AtomicStoreReferenceAndDiagnostics(Of TypeSymbol)(Me._lazyReturnType, wellKnownType, instance, Nothing)
					instance.Free()
				End If
				Return Me._lazyReturnType
			End Get
		End Property

		Private ReadOnly Property SourceEvent As SourceEventSymbol
			Get
				Return Me.m_propertyOrEvent
			End Get
		End Property

		Protected Sub New(ByVal container As SourceMemberContainerTypeSymbol, ByVal [event] As SourceEventSymbol)
			MyBase.New(container, [event])
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
			Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
			Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
			Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor, typedConstants, keyValuePairs, False))
		End Sub

		Friend NotOverridable Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Protected Shared Function ConstructFieldLikeEventAccessorBody(ByVal eventSymbol As SourceEventSymbol, ByVal isAddMethod As Boolean, ByVal compilation As VisualBasicCompilation, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			boundBlock = If(eventSymbol.IsWindowsRuntimeEvent, SynthesizedEventAccessorSymbol.ConstructFieldLikeEventAccessorBody_WinRT(eventSymbol, isAddMethod, compilation, diagnostics), SynthesizedEventAccessorSymbol.ConstructFieldLikeEventAccessorBody_Regular(eventSymbol, isAddMethod, compilation, diagnostics))
			If (boundBlock Is Nothing) Then
				Dim syntax As VisualBasicSyntaxNode = DirectCast(eventSymbol.SyntaxReference.GetSyntax(New CancellationToken()), VisualBasicSyntaxNode)
				Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
				boundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray(Of BoundStatement).Empty, True)
			End If
			Return boundBlock
		End Function

		Private Shared Function ConstructFieldLikeEventAccessorBody_Regular(ByVal eventSymbol As SourceEventSymbol, ByVal isAddMethod As Boolean, ByVal compilation As VisualBasicCompilation, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax)
			Dim parameters As ImmutableArray(Of ParameterSymbol)
			Dim bitVector As Microsoft.CodeAnalysis.BitVector
			Dim boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference
			If (eventSymbol.Type.IsDelegateType()) Then
				Dim visualBasicSyntax As VisualBasicSyntaxNode = eventSymbol.SyntaxReference.GetVisualBasicSyntax(New CancellationToken())
				Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = eventSymbol.Type
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = If(isAddMethod, eventSymbol.AddMethod, eventSymbol.RemoveMethod)
				Dim meParameter As ParameterSymbol = methodSymbol.MeParameter
				Dim specialType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = compilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean)
				Dim specialMember As Microsoft.CodeAnalysis.SpecialMember = If(isAddMethod, Microsoft.CodeAnalysis.SpecialMember.System_Delegate__Combine, Microsoft.CodeAnalysis.SpecialMember.System_Delegate__Remove)
				Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)()
				Dim specialTypeMember As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Binder.GetSpecialTypeMember(compilation.Assembly, specialMember, useSiteInfo), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
				diagnostics.Add(useSiteInfo, visualBasicSyntax.GetLocation())
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = (New BoundReturnStatement(visualBasicSyntax, Nothing, Nothing, Nothing, False)).MakeCompilerGenerated()
				If (specialTypeMember IsNot Nothing) Then
					useSiteInfo = New UseSiteInfo(Of AssemblySymbol)()
					Dim wellKnownTypeMember As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Binder.GetWellKnownTypeMember(compilation, WellKnownMember.System_Threading_Interlocked__CompareExchange_T, useSiteInfo), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
					If (eventSymbol.IsShared) Then
						boundMeReference = Nothing
					Else
						boundMeReference = (New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(visualBasicSyntax, meParameter.Type)).MakeCompilerGenerated()
					End If
					Dim boundMeReference1 As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference = boundMeReference
					Dim associatedField As FieldSymbol = eventSymbol.AssociatedField
					Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = (New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(visualBasicSyntax, boundMeReference1, associatedField, True, associatedField.Type, False)).MakeCompilerGenerated()
					Dim item As ParameterSymbol = methodSymbol.Parameters(0)
					Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = (New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(visualBasicSyntax, item, False, item.Type)).MakeCompilerGenerated()
					Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(diagnostics, compilation.Assembly)
					If (wellKnownTypeMember IsNot Nothing) Then
						diagnostics.Add(useSiteInfo, visualBasicSyntax.GetLocation())
						wellKnownTypeMember = wellKnownTypeMember.Construct(ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)(type))
						Dim generatedLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("LOOP")
						Dim synthesizedLocal(2) As LocalSymbol
						Dim boundLocal(2) As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
						Dim num As Integer = 0
						Do
							synthesizedLocal(num) = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(methodSymbol, type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
							boundLocal(num) = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(visualBasicSyntax, synthesizedLocal(num), type)
							num = num + 1
						Loop While num < CInt(synthesizedLocal.Length)
						Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = (New BoundExpressionStatement(visualBasicSyntax, (New BoundAssignmentOperator(visualBasicSyntax, boundLocal(0), boundFieldAccess.MakeRValue(), True, type, False)).MakeCompilerGenerated(), False)).MakeCompilerGenerated()
						Dim boundStatement2 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = (New BoundLabelStatement(visualBasicSyntax, generatedLabelSymbol)).MakeCompilerGenerated()
						Dim boundStatement3 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = (New BoundExpressionStatement(visualBasicSyntax, (New BoundAssignmentOperator(visualBasicSyntax, boundLocal(1), boundLocal(0).MakeRValue(), True, type, False)).MakeCompilerGenerated(), False)).MakeCompilerGenerated()
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = boundLocal(1).Type
						parameters = specialTypeMember.Parameters
						Dim type1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = parameters(0).Type
						Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
						Conversions.ClassifyDirectCastConversion(typeSymbol, type1, discarded)
						Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = boundParameter.Type
						parameters = specialTypeMember.Parameters
						Dim type2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = parameters(1).Type
						discarded = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
						Conversions.ClassifyDirectCastConversion(typeSymbol1, type2, discarded)
						diagnostics.Add(visualBasicSyntax.GetLocation(), compoundUseSiteInfo)
						Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = boundLocal(1).MakeRValue()
						parameters = specialTypeMember.Parameters
						Dim boundDirectCast As Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast = New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(visualBasicSyntax, boundLocal1, ConversionKind.WideningReference, parameters(0).Type, False)
						parameters = specialTypeMember.Parameters
						Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundDirectCast, New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(visualBasicSyntax, boundParameter, ConversionKind.WideningReference, parameters(1).Type, False))
						Dim returnType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = specialTypeMember.ReturnType
						bitVector = New Microsoft.CodeAnalysis.BitVector()
						boundExpression = (New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(visualBasicSyntax, New Microsoft.CodeAnalysis.VisualBasic.BoundCall(visualBasicSyntax, specialTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector), ConversionKind.NarrowingReference, type, type.IsErrorType())).MakeCompilerGenerated()
						Dim boundStatement4 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = (New BoundExpressionStatement(visualBasicSyntax, (New BoundAssignmentOperator(visualBasicSyntax, boundLocal(2), boundExpression, True, type, False)).MakeCompilerGenerated(), False)).MakeCompilerGenerated()
						Dim boundExpressions1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundFieldAccess, boundLocal(2).MakeRValue(), boundLocal(1).MakeRValue())
						Dim returnType1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = wellKnownTypeMember.ReturnType
						bitVector = New Microsoft.CodeAnalysis.BitVector()
						Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(visualBasicSyntax, wellKnownTypeMember, Nothing, Nothing, boundExpressions1, Nothing, returnType1, False, False, bitVector)
						Dim boundStatement5 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = (New BoundExpressionStatement(visualBasicSyntax, (New BoundAssignmentOperator(visualBasicSyntax, boundLocal(0), boundCall, True, type, False)).MakeCompilerGenerated(), False)).MakeCompilerGenerated()
						Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = (New BoundBinaryOperator(visualBasicSyntax, BinaryOperatorKind.[Is], boundLocal(0).MakeRValue(), boundLocal(1).MakeRValue(), False, specialType, False)).MakeCompilerGenerated()
						Dim boundStatement6 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = (New BoundConditionalGoto(visualBasicSyntax, boundExpression1, False, generatedLabelSymbol, False)).MakeCompilerGenerated()
						statementSyntaxes = New SyntaxList(Of StatementSyntax)()
						boundBlock = (New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(visualBasicSyntax, statementSyntaxes, synthesizedLocal.AsImmutable(Of LocalSymbol)(), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { boundStatement1, boundStatement2, boundStatement3, boundStatement4, boundStatement5, boundStatement6, boundStatement }), False)).MakeCompilerGenerated()
					Else
						Dim typeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = associatedField.Type
						parameters = specialTypeMember.Parameters
						Conversions.ClassifyDirectCastConversion(typeSymbol2, parameters(0).Type, compoundUseSiteInfo)
						Dim type3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = boundParameter.Type
						parameters = specialTypeMember.Parameters
						Conversions.ClassifyDirectCastConversion(type3, parameters(1).Type, compoundUseSiteInfo)
						diagnostics.Add(visualBasicSyntax.GetLocation(), compoundUseSiteInfo)
						Dim boundFieldAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = boundFieldAccess.MakeRValue()
						parameters = specialTypeMember.Parameters
						Dim boundDirectCast1 As Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast = New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(visualBasicSyntax, boundFieldAccess1, ConversionKind.WideningReference, parameters(0).Type, False)
						parameters = specialTypeMember.Parameters
						Dim boundExpressions2 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundDirectCast1, New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(visualBasicSyntax, boundParameter, ConversionKind.WideningReference, parameters(1).Type, False))
						Dim returnType2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = specialTypeMember.ReturnType
						bitVector = New Microsoft.CodeAnalysis.BitVector()
						boundExpression = (New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(visualBasicSyntax, New Microsoft.CodeAnalysis.VisualBasic.BoundCall(visualBasicSyntax, specialTypeMember, Nothing, Nothing, boundExpressions2, Nothing, returnType2, False, False, bitVector), ConversionKind.NarrowingReference, type, type.IsErrorType())).MakeCompilerGenerated()
						Dim boundStatement7 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = (New BoundExpressionStatement(visualBasicSyntax, (New BoundAssignmentOperator(visualBasicSyntax, boundFieldAccess, boundExpression, True, type, False)).MakeCompilerGenerated(), False)).MakeCompilerGenerated()
						statementSyntaxes = New SyntaxList(Of StatementSyntax)()
						boundBlock = (New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(visualBasicSyntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundStatement7, boundStatement), False)).MakeCompilerGenerated()
					End If
				Else
					statementSyntaxes = New SyntaxList(Of StatementSyntax)()
					boundBlock = (New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(visualBasicSyntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundStatement), False)).MakeCompilerGenerated()
				End If
			Else
				boundBlock = Nothing
			End If
			Return boundBlock
		End Function

		Private Shared Function ConstructFieldLikeEventAccessorBody_WinRT(ByVal eventSymbol As SourceEventSymbol, ByVal isAddMethod As Boolean, ByVal compilation As VisualBasicCompilation, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax)
			Dim boundMeReference As BoundExpression
			Dim visualBasicSyntax As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = eventSymbol.SyntaxReference.GetVisualBasicSyntax(New CancellationToken())
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = If(isAddMethod, eventSymbol.AddMethod, eventSymbol.RemoveMethod)
			Dim associatedField As FieldSymbol = eventSymbol.AssociatedField
			Dim type As NamedTypeSymbol = DirectCast(associatedField.Type, NamedTypeSymbol)
			If (Not type.IsErrorType()) Then
				Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)()
				Dim wellKnownTypeMember As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Binder.GetWellKnownTypeMember(compilation, Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__GetOrCreateEventRegistrationTokenTable, useSiteInfo), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
				diagnostics.Add(useSiteInfo, visualBasicSyntax.GetLocation())
				If (wellKnownTypeMember IsNot Nothing) Then
					wellKnownTypeMember = wellKnownTypeMember.AsMember(type)
					Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember = If(isAddMethod, Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__AddEventHandler, Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__RemoveEventHandler)
					useSiteInfo = New UseSiteInfo(Of AssemblySymbol)()
					Dim wellKnownTypeMember1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Binder.GetWellKnownTypeMember(compilation, wellKnownMember, useSiteInfo), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
					diagnostics.Add(useSiteInfo, visualBasicSyntax.GetLocation())
					If (wellKnownTypeMember1 IsNot Nothing) Then
						wellKnownTypeMember1 = wellKnownTypeMember1.AsMember(type)
						Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = visualBasicSyntax
						If (associatedField.IsShared) Then
							boundMeReference = Nothing
						Else
							boundMeReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(visualBasicSyntax, methodSymbol.MeParameter.Type)
						End If
						Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = (New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(visualBasicSyntaxNode, boundMeReference, associatedField, True, associatedField.Type, False)).MakeCompilerGenerated()
						Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(boundFieldAccess)
						Dim returnType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = wellKnownTypeMember.ReturnType
						Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
						Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = (New Microsoft.CodeAnalysis.VisualBasic.BoundCall(visualBasicSyntax, wellKnownTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)).MakeCompilerGenerated()
						Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = methodSymbol.Parameters.[Single]()
						Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = (New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(visualBasicSyntax, parameterSymbol, False, parameterSymbol.Type)).MakeCompilerGenerated()
						Dim boundExpressions1 As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(boundParameter)
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = wellKnownTypeMember1.ReturnType
						bitVector = New Microsoft.CodeAnalysis.BitVector()
						Dim boundCall1 As Microsoft.CodeAnalysis.VisualBasic.BoundCall = (New Microsoft.CodeAnalysis.VisualBasic.BoundCall(visualBasicSyntax, wellKnownTypeMember1, Nothing, boundCall, boundExpressions1, Nothing, typeSymbol, False, False, bitVector)).MakeCompilerGenerated()
						If (Not isAddMethod) Then
							Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = (New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(visualBasicSyntax, boundCall1, False)).MakeCompilerGenerated()
							Dim boundReturnStatement As Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement(visualBasicSyntax, Nothing, Nothing, Nothing, False)
							statementSyntaxes = New SyntaxList(Of StatementSyntax)()
							boundBlock = (New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(visualBasicSyntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundStatement)(boundExpressionStatement, boundReturnStatement), False)).MakeCompilerGenerated()
						Else
							Dim boundReturnStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement(visualBasicSyntax, boundCall1, Nothing, Nothing, False)
							statementSyntaxes = New SyntaxList(Of StatementSyntax)()
							boundBlock = (New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(visualBasicSyntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundStatement)(boundReturnStatement1), False)).MakeCompilerGenerated()
						End If
					Else
						boundBlock = Nothing
					End If
				Else
					boundBlock = Nothing
				End If
			Else
				boundBlock = Nothing
			End If
			Return boundBlock
		End Function

		Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			MyBase.GenerateDeclarationErrors(cancellationToken)
			cancellationToken.ThrowIfCancellationRequested()
			Dim parameters As ImmutableArray(Of ParameterSymbol) = Me.Parameters
			Dim returnType As TypeSymbol = Me.ReturnType
		End Sub

		Friend Overrides Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByRef methodBodyBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing) As BoundBlock
			Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
			Return SynthesizedEventAccessorSymbol.ConstructFieldLikeEventAccessorBody(Me.m_propertyOrEvent, Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.EventAdd, declaringCompilation, diagnostics)
		End Function
	End Class
End Namespace