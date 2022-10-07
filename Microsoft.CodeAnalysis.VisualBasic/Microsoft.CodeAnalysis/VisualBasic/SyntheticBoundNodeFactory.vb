Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.RuntimeMembers
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class SyntheticBoundNodeFactory
		Private _currentClass As NamedTypeSymbol

		Private _syntax As SyntaxNode

		Public ReadOnly Diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

		Public ReadOnly TopLevelMethod As MethodSymbol

		Public ReadOnly CompilationState As TypeCompilationState

		Public ReadOnly Property Compilation As VisualBasicCompilation
			Get
				Return Me.CompilationState.Compilation
			End Get
		End Property

		Public Property CurrentMethod As MethodSymbol

		Public ReadOnly Property CurrentType As NamedTypeSymbol
			Get
				Return Me._currentClass
			End Get
		End Property

		Private ReadOnly Property EmitModule As PEModuleBuilder
			Get
				If (Me.CompilationState Is Nothing) Then
					Return Nothing
				End If
				Return Me.CompilationState.ModuleBuilderOpt
			End Get
		End Property

		Public Property Syntax As SyntaxNode
			Get
				Return Me._syntax
			End Get
			Set(ByVal value As SyntaxNode)
				Me._syntax = value
			End Set
		End Property

		Public Sub New(ByVal topLevelMethod As MethodSymbol, ByVal currentMethod As MethodSymbol, ByVal node As SyntaxNode, ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			MyClass.New(topLevelMethod, currentMethod, Nothing, node, compilationState, diagnostics)
		End Sub

		Public Sub New(ByVal topLevelMethod As MethodSymbol, ByVal currentMethod As MethodSymbol, ByVal currentClass As NamedTypeSymbol, ByVal node As SyntaxNode, ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			MyBase.New()
			Me.CompilationState = compilationState
			Me.CurrentMethod = currentMethod
			Me.TopLevelMethod = topLevelMethod
			Me._currentClass = currentClass
			Me._syntax = node
			Me.Diagnostics = diagnostics
		End Sub

		Public Sub AddField(ByVal containingType As NamedTypeSymbol, ByVal field As FieldSymbol)
			Dim emitModule As PEModuleBuilder = Me.EmitModule
			If (emitModule IsNot Nothing) Then
				emitModule.AddSynthesizedDefinition(containingType, field.GetCciAdapter())
			End If
		End Sub

		Public Sub AddMethod(ByVal containingType As NamedTypeSymbol, ByVal method As MethodSymbol)
			Dim emitModule As PEModuleBuilder = Me.EmitModule
			If (emitModule IsNot Nothing) Then
				emitModule.AddSynthesizedDefinition(containingType, method.GetCciAdapter())
			End If
		End Sub

		Public Sub AddNestedType(ByVal nestedType As NamedTypeSymbol)
			Dim emitModule As PEModuleBuilder = Me.EmitModule
			If (emitModule IsNot Nothing) Then
				emitModule.AddSynthesizedDefinition(Me._currentClass, nestedType.GetCciAdapter())
			End If
		End Sub

		Public Sub AddProperty(ByVal containingType As NamedTypeSymbol, ByVal prop As PropertySymbol)
			Dim emitModule As PEModuleBuilder = Me.EmitModule
			If (emitModule IsNot Nothing) Then
				emitModule.AddSynthesizedDefinition(containingType, prop.GetCciAdapter())
			End If
		End Sub

		Public Function Array(ByVal elementType As TypeSymbol, ByVal ParamArray elements As BoundExpression()) As BoundExpression
			Return Me.Array(elementType, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(elements))
		End Function

		Public Function Array(ByVal elementType As TypeSymbol, ByVal elements As ImmutableArray(Of BoundExpression)) As BoundExpression
			Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = Me.Compilation.CreateArrayTypeSymbol(elementType, 1)
			Dim boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization(Me._syntax, elements, arrayTypeSymbol, False)
			boundArrayInitialization.SetWasCompilerGenerated()
			Return New BoundArrayCreation(Me._syntax, ImmutableArray.Create(Of BoundExpression)(Me.Literal(elements.Length)), boundArrayInitialization, arrayTypeSymbol, False)
		End Function

		Public Function Array(ByVal elementType As TypeSymbol, ByVal bounds As ImmutableArray(Of BoundExpression), ByVal elements As ImmutableArray(Of BoundExpression)) As BoundExpression
			Dim boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization
			Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = Me.Compilation.CreateArrayTypeSymbol(elementType, 1)
			If (Not elements.IsDefaultOrEmpty) Then
				boundArrayInitialization = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization(Me._syntax, elements, arrayTypeSymbol, False)
			Else
				boundArrayInitialization = Nothing
			End If
			Dim boundArrayInitialization1 As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization = boundArrayInitialization
			If (boundArrayInitialization1 IsNot Nothing) Then
				boundArrayInitialization1.SetWasCompilerGenerated()
			End If
			Dim boundArrayCreation As Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation(Me._syntax, bounds, boundArrayInitialization1, arrayTypeSymbol, False)
			boundArrayCreation.SetWasCompilerGenerated()
			Return boundArrayCreation
		End Function

		Public Function ArrayAccess(ByVal array As BoundExpression, ByVal isLValue As Boolean, ByVal ParamArray indices As BoundExpression()) As BoundArrayAccess
			Return Me.ArrayAccess(array, isLValue, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(indices))
		End Function

		Public Function ArrayAccess(ByVal array As BoundExpression, ByVal isLValue As Boolean, ByVal indices As ImmutableArray(Of BoundExpression)) As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess
			Dim boundArrayAccess As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess(Me._syntax, array, indices, isLValue, DirectCast(array.Type, ArrayTypeSymbol).ElementType, False)
			boundArrayAccess.SetWasCompilerGenerated()
			Return boundArrayAccess
		End Function

		Public Function Assignment(ByVal left As BoundExpression, ByVal right As BoundExpression) As BoundExpressionStatement
			Return Me.ExpressionStatement(Me.AssignmentExpression(left, right))
		End Function

		Public Function AssignmentExpression(ByVal left As BoundExpression, ByVal right As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator
			Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(Me._syntax, left, right, True, False)
			boundAssignmentOperator.SetWasCompilerGenerated()
			Return boundAssignmentOperator
		End Function

		Public Function BadExpression(ByVal ParamArray subExpressions As BoundExpression()) As BoundExpression
			Dim boundBadExpression As Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(Me._syntax, LookupResultKind.Empty, ImmutableArray(Of Symbol).Empty, ImmutableArray.Create(Of BoundExpression)(subExpressions), ErrorTypeSymbol.UnknownResultType, True)
			boundBadExpression.SetWasCompilerGenerated()
			Return boundBadExpression
		End Function

		Public Function Base() As Microsoft.CodeAnalysis.VisualBasic.BoundMyBaseReference
			Dim boundMyBaseReference As Microsoft.CodeAnalysis.VisualBasic.BoundMyBaseReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMyBaseReference(Me._syntax, Me.CurrentMethod.MeParameter.Type.BaseTypeNoUseSiteDiagnostics)
			boundMyBaseReference.SetWasCompilerGenerated()
			Return boundMyBaseReference
		End Function

		Public Function BaseInitialization(ByVal ParamArray args As BoundExpression()) As BoundStatement
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.CurrentMethod.MeParameter.Type.BaseTypeNoUseSiteDiagnostics.InstanceConstructors.[Single](Function(c As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) c.ParameterCount = CInt(args.Length))
			Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(Me._syntax, Me.[Call](Me.Base(), methodSymbol, args), False)
			boundExpressionStatement.SetWasCompilerGenerated()
			Return boundExpressionStatement
		End Function

		Public Function Binary(ByVal kind As BinaryOperatorKind, ByVal type As TypeSymbol, ByVal left As BoundExpression, ByVal right As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator
			Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(Me.Syntax, kind, left, right, False, type, False)
			boundBinaryOperator.SetWasCompilerGenerated()
			Return boundBinaryOperator
		End Function

		Public Function BinaryConditional(ByVal left As BoundExpression, ByVal right As BoundExpression) As BoundBinaryConditionalExpression
			Return New BoundBinaryConditionalExpression(Me.Syntax, left, Nothing, Nothing, right, Nothing, left.Type, False)
		End Function

		Public Function Block(ByVal statements As ImmutableArray(Of BoundStatement)) As BoundBlock
			Return Me.Block(ImmutableArray(Of LocalSymbol).Empty, statements)
		End Function

		Public Function Block(ByVal locals As ImmutableArray(Of LocalSymbol), ByVal statements As ImmutableArray(Of BoundStatement)) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = Me._syntax
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntaxNode, statementSyntaxes, locals, statements, False)
			boundBlock.SetWasCompilerGenerated()
			Return boundBlock
		End Function

		Public Function Block() As BoundBlock
			Return Me.Block(ImmutableArray(Of BoundStatement).Empty)
		End Function

		Public Function Block(ByVal ParamArray statements As BoundStatement()) As BoundBlock
			Return Me.Block(ImmutableArray.Create(Of BoundStatement)(statements))
		End Function

		Public Function Block(ByVal locals As ImmutableArray(Of LocalSymbol), ByVal ParamArray statements As BoundStatement()) As BoundBlock
			Return Me.Block(locals, ImmutableArray.Create(Of BoundStatement)(statements))
		End Function

		Public Function [Call](ByVal receiver As BoundExpression, ByVal method As MethodSymbol) As BoundCall
			Return Me.[Call](receiver, method, ImmutableArray(Of BoundExpression).Empty)
		End Function

		Public Function [Call](ByVal receiver As BoundExpression, ByVal method As MethodSymbol, ByVal ParamArray args As BoundExpression()) As BoundCall
			Return Me.[Call](receiver, method, ImmutableArray.Create(Of BoundExpression)(args))
		End Function

		Public Function [Call](ByVal receiver As BoundExpression, ByVal method As MethodSymbol, ByVal args As ImmutableArray(Of BoundExpression)) As Microsoft.CodeAnalysis.VisualBasic.BoundCall
			Dim syntax As SyntaxNode = Me.Syntax
			Dim returnType As TypeSymbol = method.ReturnType
			Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
			Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, method, Nothing, receiver, args, Nothing, returnType, True, False, bitVector)
			boundCall.SetWasCompilerGenerated()
			Return boundCall
		End Function

		Public Function [Catch](ByVal local As LocalSymbol, ByVal block As BoundBlock, Optional ByVal isSynthesizedAsyncCatchAll As Boolean = False) As BoundCatchBlock
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__SetProjectError, False)
			Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__ClearProjectError, False)
			Return New BoundCatchBlock(Me.Syntax, local, Me.Local(local, False), Nothing, Nothing, block, isSynthesizedAsyncCatchAll, If(methodSymbol Is Nothing, True, methodSymbol1 Is Nothing))
		End Function

		Public Function CatchBlocks(ByVal ParamArray blocks As BoundCatchBlock()) As ImmutableArray(Of BoundCatchBlock)
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundCatchBlock)(blocks)
		End Function

		<Conditional("DEBUG")>
		Private Sub CheckSwitchSections(ByVal sections As ImmutableArray(Of BoundCaseBlock))
			Dim nums As HashSet(Of Integer) = New HashSet(Of Integer)()
			Dim enumerator As ImmutableArray(Of BoundCaseBlock).Enumerator = sections.GetEnumerator()
			While enumerator.MoveNext()
				Dim enumerator1 As ImmutableArray(Of BoundCaseClause).Enumerator = enumerator.Current.CaseStatement.CaseClauses.GetEnumerator()
				While enumerator1.MoveNext()
					Dim int32Value As Integer = DirectCast(enumerator1.Current, BoundSimpleCaseClause).ValueOpt.ConstantValueOpt.Int32Value
					nums.Add(int32Value)
				End While
			End While
		End Sub

		Public Sub CloseMethod(ByVal body As BoundStatement)
			If (body.Kind <> BoundKind.Block) Then
				body = Me.Block(New BoundStatement() { body })
			End If
			Me.CompilationState.AddSynthesizedMethod(Me.CurrentMethod, body)
			Me.CurrentMethod = Nothing
		End Sub

		Public Function Conditional(ByVal condition As BoundExpression, ByVal consequence As BoundExpression, ByVal alternative As BoundExpression, ByVal type As TypeSymbol) As BoundTernaryConditionalExpression
			Return New BoundTernaryConditionalExpression(Me.Syntax, condition, consequence, alternative, Nothing, type, False)
		End Function

		Public Function ConstructorInfo(ByVal meth As WellKnownMember) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(meth, False)
			boundExpression = If(methodSymbol IsNot Nothing, Me.ConstructorInfo(methodSymbol), Me.BadExpression(New Microsoft.CodeAnalysis.VisualBasic.BoundExpression(-1) {}))
			Return boundExpression
		End Function

		Public Function ConstructorInfo(ByVal meth As SpecialMember) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Me.SpecialMember(meth), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			boundExpression = If(methodSymbol IsNot Nothing, Me.ConstructorInfo(methodSymbol), Me.BadExpression(New Microsoft.CodeAnalysis.VisualBasic.BoundExpression(-1) {}))
			Return boundExpression
		End Function

		Public Function ConstructorInfo(ByVal meth As MethodSymbol) As BoundExpression
			Dim boundMethodInfo As Microsoft.CodeAnalysis.VisualBasic.BoundMethodInfo = New Microsoft.CodeAnalysis.VisualBasic.BoundMethodInfo(Me.Syntax, meth, Me.WellKnownType(WellKnownType.System_Reflection_ConstructorInfo))
			boundMethodInfo.SetWasCompilerGenerated()
			Return boundMethodInfo
		End Function

		Public Function Convert(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal arg As BoundExpression, Optional ByVal isChecked As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.BoundConversion
			Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion
			If (arg.IsNothingLiteral()) Then
				boundConversion = Me.Convert(type, arg, ConversionKind.WideningNothingLiteral, isChecked)
			ElseIf (type.IsErrorType() OrElse arg.Type.IsErrorType()) Then
				boundConversion = Me.Convert(type, arg, ConversionKind.WideningReference, isChecked)
			Else
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = arg.Type
				Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
				Dim keyValuePair As KeyValuePair(Of ConversionKind, MethodSymbol) = Conversions.ClassifyConversion(typeSymbol, type, discarded)
				boundConversion = Me.Convert(type, arg, keyValuePair.Key, isChecked)
			End If
			Return boundConversion
		End Function

		Public Function Convert(ByVal type As TypeSymbol, ByVal arg As BoundExpression, ByVal convKind As ConversionKind, Optional ByVal isChecked As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.BoundConversion
			Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = New Microsoft.CodeAnalysis.VisualBasic.BoundConversion(Me._syntax, arg, convKind, isChecked, True, Nothing, type, False)
			boundConversion.SetWasCompilerGenerated()
			Return boundConversion
		End Function

		Public Function [DirectCast](ByVal expression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As BoundDirectCast
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim syntax As SyntaxNode = Me.Syntax
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = expression
			If (expression.IsNothingLiteral()) Then
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNothingLiteral
			Else
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = expression.Type
				Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
				conversionKind = Conversions.ClassifyDirectCastConversion(typeSymbol, type, discarded)
			End If
			Return New BoundDirectCast(syntax, boundExpression, conversionKind, type, False)
		End Function

		Public Function ExpressionStatement(ByVal expr As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement
			Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(Me._syntax, expr, False)
			boundExpressionStatement.SetWasCompilerGenerated()
			Return boundExpressionStatement
		End Function

		Public Function Field(ByVal receiver As BoundExpression, ByVal f As FieldSymbol, ByVal isLValue As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess
			Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(Me._syntax, receiver, f, isLValue, f.Type, False)
			boundFieldAccess.SetWasCompilerGenerated()
			Return boundFieldAccess
		End Function

		Public Function FieldInfo(ByVal field As FieldSymbol) As BoundExpression
			Dim boundFieldInfo As Microsoft.CodeAnalysis.VisualBasic.BoundFieldInfo = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldInfo(Me._syntax, field, Me.WellKnownType(WellKnownType.System_Reflection_FieldInfo))
			Me.WellKnownMember(Of MethodSymbol)(If(field.ContainingType.IsGenericType, WellKnownMember.System_Reflection_FieldInfo__GetFieldFromHandle2, WellKnownMember.System_Reflection_FieldInfo__GetFieldFromHandle), False)
			boundFieldInfo.SetWasCompilerGenerated()
			Return boundFieldInfo
		End Function

		Public Function GenerateLabel(ByVal prefix As String) As GeneratedLabelSymbol
			Return New GeneratedLabelSymbol(prefix)
		End Function

		Public Function [Goto](ByVal label As LabelSymbol, Optional ByVal setWasCompilerGenerated As Boolean = True) As Microsoft.CodeAnalysis.VisualBasic.BoundGotoStatement
			Dim boundGotoStatement As Microsoft.CodeAnalysis.VisualBasic.BoundGotoStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundGotoStatement(Me._syntax, label, Nothing, False)
			If (setWasCompilerGenerated) Then
				boundGotoStatement.SetWasCompilerGenerated()
			End If
			Return boundGotoStatement
		End Function

		Public Shared Function HiddenSequencePoint(Optional ByVal statementOpt As BoundStatement = Nothing) As BoundStatement
			Return (New BoundSequencePoint(Nothing, statementOpt, False)).MakeCompilerGenerated()
		End Function

		Public Function [If](ByVal condition As BoundExpression, ByVal thenClause As BoundStatement, ByVal elseClause As BoundStatement) As BoundStatement
			Dim generatedLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("afterif")
			Dim generatedLabelSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("alternative")
			Dim boundConditionalGoto As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto = New Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto(Me._syntax, condition, False, generatedLabelSymbol1, False)
			boundConditionalGoto.SetWasCompilerGenerated()
			Return Me.Block(New BoundStatement() { boundConditionalGoto, thenClause, Me.[Goto](generatedLabelSymbol, True), Me.Label(generatedLabelSymbol1), elseClause, Me.Label(generatedLabelSymbol) })
		End Function

		Public Function [If](ByVal condition As BoundExpression, ByVal thenClause As BoundStatement) As BoundStatement
			Return Me.[If](condition, thenClause, Me.Block())
		End Function

		Public Function InstrumentationPayloadRoot(ByVal analysisKind As Integer, ByVal payloadType As TypeSymbol, ByVal isLValue As Boolean) As BoundExpression
			Dim boundInstrumentationPayloadRoot As Microsoft.CodeAnalysis.VisualBasic.BoundInstrumentationPayloadRoot = New Microsoft.CodeAnalysis.VisualBasic.BoundInstrumentationPayloadRoot(Me.Syntax, analysisKind, isLValue, payloadType)
			boundInstrumentationPayloadRoot.SetWasCompilerGenerated()
			Return boundInstrumentationPayloadRoot
		End Function

		Public Function IntEqual(ByVal left As BoundExpression, ByVal right As BoundExpression) As BoundBinaryOperator
			Return Me.Binary(BinaryOperatorKind.Equals, Me.SpecialType(SpecialType.System_Boolean), left, right)
		End Function

		Public Function IntLessThan(ByVal left As BoundExpression, ByVal right As BoundExpression) As BoundBinaryOperator
			Return Me.Binary(BinaryOperatorKind.LessThan, Me.SpecialType(SpecialType.System_Boolean), left, right)
		End Function

		Public Function Label(ByVal labelSym As LabelSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement
			Dim boundLabelStatement As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(Me._syntax, labelSym)
			boundLabelStatement.SetWasCompilerGenerated()
			Return boundLabelStatement
		End Function

		Public Function Literal(ByVal value As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(Me._syntax, ConstantValue.Create(value), Me.SpecialType(SpecialType.System_Boolean))
			boundLiteral.SetWasCompilerGenerated()
			Return boundLiteral
		End Function

		Public Function Literal(ByVal value As Integer) As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(Me._syntax, ConstantValue.Create(value), Me.SpecialType(SpecialType.System_Int32))
			boundLiteral.SetWasCompilerGenerated()
			Return boundLiteral
		End Function

		Public Function Literal(ByVal value As String) As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(Me._syntax, ConstantValue.Create(value), Me.SpecialType(SpecialType.System_String))
			boundLiteral.SetWasCompilerGenerated()
			Return boundLiteral
		End Function

		Public Function Local(ByVal localSym As LocalSymbol, ByVal isLValue As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(Me._syntax, localSym, isLValue, localSym.Type)
			boundLocal.SetWasCompilerGenerated()
			Return boundLocal
		End Function

		Public Function LogicalAndAlso(ByVal left As BoundExpression, ByVal right As BoundExpression) As BoundBinaryOperator
			Return Me.Binary(BinaryOperatorKind.[AndAlso], Me.SpecialType(SpecialType.System_Boolean), left, right)
		End Function

		Public Function LogicalOrElse(ByVal left As BoundExpression, ByVal right As BoundExpression) As BoundBinaryOperator
			Return Me.Binary(BinaryOperatorKind.[OrElse], Me.SpecialType(SpecialType.System_Boolean), left, right)
		End Function

		Public Function MaximumMethodDefIndex() As BoundExpression
			Dim boundMaximumMethodDefIndex As Microsoft.CodeAnalysis.VisualBasic.BoundMaximumMethodDefIndex = New Microsoft.CodeAnalysis.VisualBasic.BoundMaximumMethodDefIndex(Me.Syntax, Me.SpecialType(SpecialType.System_Int32))
			boundMaximumMethodDefIndex.SetWasCompilerGenerated()
			Return boundMaximumMethodDefIndex
		End Function

		Public Function [Me]() As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference
			Dim boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(Me._syntax, Me.CurrentMethod.MeParameter.Type)
			boundMeReference.SetWasCompilerGenerated()
			Return boundMeReference
		End Function

		Public Function MethodDefIndex(ByVal method As MethodSymbol) As BoundExpression
			Dim boundMethodDefIndex As Microsoft.CodeAnalysis.VisualBasic.BoundMethodDefIndex = New Microsoft.CodeAnalysis.VisualBasic.BoundMethodDefIndex(Me.Syntax, method, Me.SpecialType(SpecialType.System_Int32))
			boundMethodDefIndex.SetWasCompilerGenerated()
			Return boundMethodDefIndex
		End Function

		Public Function MethodInfo(ByVal meth As WellKnownMember) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(meth, False)
			boundExpression = If(methodSymbol IsNot Nothing, Me.MethodInfo(methodSymbol), Me.BadExpression(New Microsoft.CodeAnalysis.VisualBasic.BoundExpression(-1) {}))
			Return boundExpression
		End Function

		Public Function MethodInfo(ByVal meth As SpecialMember) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Me.SpecialMember(meth), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			boundExpression = If(methodSymbol IsNot Nothing, Me.MethodInfo(methodSymbol), Me.BadExpression(New Microsoft.CodeAnalysis.VisualBasic.BoundExpression(-1) {}))
			Return boundExpression
		End Function

		Public Function MethodInfo(ByVal method As MethodSymbol) As BoundExpression
			Dim boundMethodInfo As Microsoft.CodeAnalysis.VisualBasic.BoundMethodInfo = New Microsoft.CodeAnalysis.VisualBasic.BoundMethodInfo(Me.Syntax, method, Me.WellKnownType(WellKnownType.System_Reflection_MethodInfo))
			Me.WellKnownMember(Of MethodSymbol)(If(method.ContainingType.IsGenericType, WellKnownMember.System_Reflection_MethodBase__GetMethodFromHandle2, WellKnownMember.System_Reflection_MethodBase__GetMethodFromHandle), False)
			boundMethodInfo.SetWasCompilerGenerated()
			Return boundMethodInfo
		End Function

		Public Function ModuleVersionId(ByVal isLValue As Boolean) As BoundExpression
			Dim boundModuleVersionId As Microsoft.CodeAnalysis.VisualBasic.BoundModuleVersionId = New Microsoft.CodeAnalysis.VisualBasic.BoundModuleVersionId(Me.Syntax, isLValue, Me.WellKnownType(WellKnownType.System_Guid))
			boundModuleVersionId.SetWasCompilerGenerated()
			Return boundModuleVersionId
		End Function

		Public Function ModuleVersionIdString() As BoundExpression
			Dim boundModuleVersionIdString As Microsoft.CodeAnalysis.VisualBasic.BoundModuleVersionIdString = New Microsoft.CodeAnalysis.VisualBasic.BoundModuleVersionIdString(Me.Syntax, Me.SpecialType(SpecialType.System_String))
			boundModuleVersionIdString.SetWasCompilerGenerated()
			Return boundModuleVersionIdString
		End Function

		Public Function [New](ByVal type As NamedTypeSymbol) As BoundObjectCreationExpression
			Dim parameterCount As Func(Of MethodSymbol, Boolean)
			Dim instanceConstructors As ImmutableArray(Of MethodSymbol) = type.InstanceConstructors
			If (SyntheticBoundNodeFactory._Closure$__.$I65-0 Is Nothing) Then
				parameterCount = Function(c As MethodSymbol) c.ParameterCount = 0
				SyntheticBoundNodeFactory._Closure$__.$I65-0 = parameterCount
			Else
				parameterCount = SyntheticBoundNodeFactory._Closure$__.$I65-0
			End If
			Return Me.[New](instanceConstructors.[Single](parameterCount))
		End Function

		Public Function [New](ByVal ctor As MethodSymbol, ByVal ParamArray args As BoundExpression()) As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = Me._syntax
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(args)
			Dim containingType As NamedTypeSymbol = ctor.ContainingType
			Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
			Dim boundObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression(syntaxNode, ctor, boundExpressions, Nothing, containingType, False, bitVector)
			boundObjectCreationExpression.SetWasCompilerGenerated()
			Return boundObjectCreationExpression
		End Function

		Public Function [New](ByVal ctor As MethodSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = Me._syntax
			Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of BoundExpression).Empty
			Dim containingType As NamedTypeSymbol = ctor.ContainingType
			Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
			Dim boundObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression(syntaxNode, ctor, empty, Nothing, containingType, False, bitVector)
			boundObjectCreationExpression.SetWasCompilerGenerated()
			Return boundObjectCreationExpression
		End Function

		Public Function NoOp(Optional ByVal flavor As NoOpStatementFlavor = 0) As BoundStatement
			Return (New BoundNoOpStatement(Me.Syntax, flavor)).MakeCompilerGenerated()
		End Function

		Public Function [Not](ByVal expression As BoundExpression) As BoundExpression
			Return New BoundUnaryOperator(expression.Syntax, UnaryOperatorKind.[Not], expression, False, expression.Type, False)
		End Function

		Public Function Null() As BoundExpression
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(Me._syntax, ConstantValue.Null, Nothing)
			boundLiteral.SetWasCompilerGenerated()
			Return boundLiteral
		End Function

		Public Function Null(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (type.IsTypeParameter() OrElse Not type.IsReferenceType) Then
				Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(Me._syntax, ConstantValue.Null, Nothing)
				boundLiteral.SetWasCompilerGenerated()
				boundExpression = Me.Convert(type, boundLiteral, False)
			Else
				Dim boundLiteral1 As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(Me._syntax, ConstantValue.Null, type)
				boundLiteral1.SetWasCompilerGenerated()
				boundExpression = boundLiteral1
			End If
			Return boundExpression
		End Function

		Public Function NullableOf(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.SpecialType(SpecialType.System_Nullable_T)
			namedTypeSymbol = If(Not namedTypeSymbol1.IsErrorType(), namedTypeSymbol1.Construct(ImmutableArray.Create(Of TypeSymbol)(type)), namedTypeSymbol1)
			Return namedTypeSymbol
		End Function

		Public Function ObjectReferenceEqual(ByVal left As BoundExpression, ByVal right As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator
			Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = Me.Binary(BinaryOperatorKind.[Is], Me.SpecialType(SpecialType.System_Boolean), left, right)
			boundBinaryOperator.SetWasCompilerGenerated()
			Return boundBinaryOperator
		End Function

		Public Sub OpenNestedType(ByVal nestedType As NamedTypeSymbol)
			Me.AddNestedType(nestedType)
			Me._currentClass = nestedType
			Me.CurrentMethod = Nothing
		End Sub

		Public Function Parameter(ByVal p As ParameterSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundParameter
			Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(Me._syntax, p, p.Type)
			boundParameter.SetWasCompilerGenerated()
			Return boundParameter
		End Function

		Public Function [Property](ByVal member As WellKnownMember) As BoundExpression
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = Me.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)(member, False)
			Return Me.[Call](Nothing, propertySymbol.GetMethod)
		End Function

		Public Function [Property](ByVal receiver As BoundExpression, ByVal member As WellKnownMember) As BoundExpression
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = Me.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)(member, False)
			Return Me.[Call](receiver, propertySymbol.GetMethod)
		End Function

		Public Function [Property](ByVal receiver As BoundExpression, ByVal name As String) As BoundExpression
			Dim members As ImmutableArray(Of Symbol) = receiver.Type.GetMembers(name)
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = members.OfType(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)().[Single]()
			Return Me.[Call](receiver, propertySymbol.GetMethod)
		End Function

		Public Function [Property](ByVal receiver As NamedTypeSymbol, ByVal name As String) As BoundExpression
			Dim members As ImmutableArray(Of Symbol) = receiver.GetMembers(name)
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = members.OfType(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)().[Single]()
			Return Me.[Call](Nothing, propertySymbol.GetMethod)
		End Function

		Public Function ReferenceAssignment(ByVal byRefLocal As LocalSymbol, ByVal lValue As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundReferenceAssignment
			Dim boundReferenceAssignment As Microsoft.CodeAnalysis.VisualBasic.BoundReferenceAssignment = New Microsoft.CodeAnalysis.VisualBasic.BoundReferenceAssignment(Me._syntax, Me.Local(byRefLocal, True), lValue, True, lValue.Type, False)
			boundReferenceAssignment.SetWasCompilerGenerated()
			Return boundReferenceAssignment
		End Function

		Public Function ReferenceIsNothing(ByVal operand As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator
			Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = Me.Binary(BinaryOperatorKind.[Is], Me.SpecialType(SpecialType.System_Boolean), operand, Me.Null(operand.Type))
			boundBinaryOperator.SetWasCompilerGenerated()
			Return boundBinaryOperator
		End Function

		Public Function ReferenceIsNotNothing(ByVal operand As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator
			Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = Me.Binary(BinaryOperatorKind.[IsNot], Me.SpecialType(SpecialType.System_Boolean), operand, Me.Null(operand.Type))
			boundBinaryOperator.SetWasCompilerGenerated()
			Return boundBinaryOperator
		End Function

		Public Function ReferenceOrByrefMe() As BoundExpression
			Dim boundValueTypeMeReference As BoundExpression
			If (Me.CurrentMethod.MeParameter.Type.IsReferenceType) Then
				boundValueTypeMeReference = Me.[Me]()
			Else
				boundValueTypeMeReference = New Microsoft.CodeAnalysis.VisualBasic.BoundValueTypeMeReference(Me._syntax, Me.CurrentMethod.MeParameter.Type)
			End If
			boundValueTypeMeReference.SetWasCompilerGenerated()
			Return boundValueTypeMeReference
		End Function

		Public Function [Return](Optional ByVal expression As BoundExpression = Nothing) As Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement
			If (expression IsNot Nothing) Then
				Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(Me.Diagnostics, Me.Compilation.Assembly)
				Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyDirectCastConversion(expression.Type, Me.CurrentMethod.ReturnType, compoundUseSiteInfo)
				Me.Diagnostics.Add(expression, compoundUseSiteInfo)
				If (Not Conversions.IsIdentityConversion(conversionKind)) Then
					expression = New BoundDirectCast(Me.Syntax, expression, conversionKind, Me.CurrentMethod.ReturnType, False)
				End If
			End If
			Dim boundReturnStatement As Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement(Me._syntax, expression, Nothing, Nothing, False)
			boundReturnStatement.SetWasCompilerGenerated()
			Return boundReturnStatement
		End Function

		Public Function [Select](ByVal ex As BoundExpression, ByVal sections As IEnumerable(Of BoundCaseBlock)) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundCaseBlocks As ImmutableArray(Of BoundCaseBlock) = ImmutableArray.CreateRange(Of BoundCaseBlock)(sections)
			If (boundCaseBlocks.Length <> 0) Then
				Dim generatedLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("break")
				Dim boundSelectStatement As Microsoft.CodeAnalysis.VisualBasic.BoundSelectStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundSelectStatement(Me._syntax, Me.ExpressionStatement(ex), Nothing, boundCaseBlocks, True, generatedLabelSymbol, False)
				boundSelectStatement.SetWasCompilerGenerated()
				boundStatement = boundSelectStatement
			Else
				boundStatement = Me.ExpressionStatement(ex)
			End If
			Return boundStatement
		End Function

		Public Function Sequence(ByVal temps As ImmutableArray(Of LocalSymbol), ByVal ParamArray parts As Microsoft.CodeAnalysis.VisualBasic.BoundExpression()) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpressionArray(CInt(parts.Length) - 1 - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim length As Integer = CInt(parts.Length) - 1 - 1
			Dim num As Integer = 0
			Do
				boundExpressionArray(num) = parts(num)
				num = num + 1
			Loop While num <= length
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = parts(CInt(parts.Length) - 1)
			Return Me.Sequence(temps, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpressionArray), boundExpression)
		End Function

		Public Function Sequence(ByVal temp As LocalSymbol, ByVal ParamArray parts As BoundExpression()) As BoundExpression
			Return Me.Sequence(ImmutableArray.Create(Of LocalSymbol)(temp), parts)
		End Function

		Public Function Sequence(ByVal ParamArray parts As BoundExpression()) As BoundExpression
			Return Me.Sequence(ImmutableArray(Of LocalSymbol).Empty, parts)
		End Function

		Public Function Sequence(ByVal locals As ImmutableArray(Of LocalSymbol), ByVal sideEffects As ImmutableArray(Of BoundExpression), ByVal result As BoundExpression) As BoundExpression
			Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(Me._syntax, locals, sideEffects, result, result.Type, False)
			boundSequence.SetWasCompilerGenerated()
			Return boundSequence
		End Function

		Public Function SequencePoint(ByVal syntax As SyntaxNode, ByVal statement As BoundStatement) As BoundStatement
			Return New BoundSequencePoint(syntax, statement, False)
		End Function

		Public Function SequencePoint(ByVal syntax As SyntaxNode) As BoundStatement
			Return (New BoundSequencePoint(syntax, Nothing, False)).MakeCompilerGenerated()
		End Function

		Public Function SequencePointWithSpan(ByVal syntax As SyntaxNode, ByVal textSpan As Microsoft.CodeAnalysis.Text.TextSpan, ByVal boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Return New BoundSequencePointWithSpan(syntax, boundStatement, textSpan, False)
		End Function

		Public Function SourceDocumentIndex(ByVal document As DebugSourceDocument) As BoundExpression
			Dim boundSourceDocumentIndex As Microsoft.CodeAnalysis.VisualBasic.BoundSourceDocumentIndex = New Microsoft.CodeAnalysis.VisualBasic.BoundSourceDocumentIndex(Me.Syntax, document, Me.SpecialType(SpecialType.System_Int32))
			boundSourceDocumentIndex.SetWasCompilerGenerated()
			Return boundSourceDocumentIndex
		End Function

		Public Function SpecialMember(ByVal sm As SpecialMember) As Symbol
			Dim useSiteInfoForMemberAndContainingType As UseSiteInfo(Of AssemblySymbol)
			Dim specialTypeMember As Symbol = Me.Compilation.GetSpecialTypeMember(sm)
			If (specialTypeMember IsNot Nothing) Then
				useSiteInfoForMemberAndContainingType = Binder.GetUseSiteInfoForMemberAndContainingType(specialTypeMember)
			Else
				Dim descriptor As MemberDescriptor = SpecialMembers.GetDescriptor(sm)
				useSiteInfoForMemberAndContainingType = New UseSiteInfo(Of AssemblySymbol)(MissingRuntimeMemberDiagnosticHelper.GetDiagnosticForMissingRuntimeHelper(descriptor.DeclaringTypeMetadataName, descriptor.Name, Me.CompilationState.Compilation.Options.EmbedVbCoreRuntime))
			End If
			Me.Diagnostics.Add(useSiteInfoForMemberAndContainingType, Me._syntax)
			Return specialTypeMember
		End Function

		Public Function SpecialType(ByVal st As SpecialType) As NamedTypeSymbol
			Return Binder.GetSpecialType(Me.Compilation, st, Me._syntax, Me.Diagnostics)
		End Function

		Public Function SpillSequence(ByVal locals As ImmutableArray(Of LocalSymbol), ByVal fields As ImmutableArray(Of FieldSymbol), ByVal statements As ImmutableArray(Of BoundStatement), ByVal valueOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As BoundSpillSequence
			Dim type As TypeSymbol
			Dim syntax As SyntaxNode = Me.Syntax
			Dim localSymbols As ImmutableArray(Of LocalSymbol) = locals
			Dim fieldSymbols As ImmutableArray(Of FieldSymbol) = fields
			Dim boundStatements As ImmutableArray(Of BoundStatement) = statements
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = valueOpt
			If (valueOpt Is Nothing) Then
				type = Me.SpecialType(SpecialType.System_Void)
			Else
				type = valueOpt.Type
			End If
			Return (New BoundSpillSequence(syntax, localSymbols, fieldSymbols, boundStatements, boundExpression, type, False)).MakeCompilerGenerated()
		End Function

		Public Function StateMachineField(ByVal type As TypeSymbol, ByVal implicitlyDefinedBy As Symbol, ByVal name As String, Optional ByVal accessibility As Microsoft.CodeAnalysis.Accessibility = 1) As SynthesizedFieldSymbol
			Dim stateMachineFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.StateMachineFieldSymbol = New Microsoft.CodeAnalysis.VisualBasic.StateMachineFieldSymbol(Me.CurrentType, implicitlyDefinedBy, type, name, accessibility, False, False, False)
			Me.AddField(Me.CurrentType, stateMachineFieldSymbol)
			Return stateMachineFieldSymbol
		End Function

		Public Function StateMachineField(ByVal type As TypeSymbol, ByVal implicitlyDefinedBy As Symbol, ByVal name As String, ByVal synthesizedKind As SynthesizedLocalKind, ByVal slotIndex As Integer, Optional ByVal accessibility As Microsoft.CodeAnalysis.Accessibility = 1) As SynthesizedFieldSymbol
			Dim stateMachineFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.StateMachineFieldSymbol = New Microsoft.CodeAnalysis.VisualBasic.StateMachineFieldSymbol(Me.CurrentType, implicitlyDefinedBy, type, name, synthesizedKind, slotIndex, accessibility, False, False, False)
			Me.AddField(Me.CurrentType, stateMachineFieldSymbol)
			Return stateMachineFieldSymbol
		End Function

		Public Function StateMachineField(ByVal type As TypeSymbol, ByVal implicitlyDefinedBy As Symbol, ByVal name As String, ByVal slotDebugInfo As LocalSlotDebugInfo, ByVal slotIndex As Integer, Optional ByVal accessibility As Microsoft.CodeAnalysis.Accessibility = 1) As SynthesizedFieldSymbol
			Dim stateMachineFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.StateMachineFieldSymbol = New Microsoft.CodeAnalysis.VisualBasic.StateMachineFieldSymbol(Me.CurrentType, implicitlyDefinedBy, type, name, slotDebugInfo, slotIndex, accessibility, False, False, False)
			Me.AddField(Me.CurrentType, stateMachineFieldSymbol)
			Return stateMachineFieldSymbol
		End Function

		Public Function StatementList() As BoundStatementList
			Return Me.StatementList(ImmutableArray(Of BoundStatement).Empty)
		End Function

		Public Function StatementList(ByVal statements As ImmutableArray(Of BoundStatement)) As Microsoft.CodeAnalysis.VisualBasic.BoundStatementList
			Dim boundStatementList As Microsoft.CodeAnalysis.VisualBasic.BoundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(Me.Syntax, statements, False)
			boundStatementList.SetWasCompilerGenerated()
			Return boundStatementList
		End Function

		Public Function StatementList(ByVal first As BoundStatement, ByVal second As BoundStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundStatementList
			Dim boundStatementList As Microsoft.CodeAnalysis.VisualBasic.BoundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(Me.Syntax, ImmutableArray.Create(Of BoundStatement)(first, second), False)
			boundStatementList.SetWasCompilerGenerated()
			Return boundStatementList
		End Function

		Public Function StringLiteral(ByVal value As ConstantValue) As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(Me._syntax, value, Me.SpecialType(SpecialType.System_String))
			boundLiteral.SetWasCompilerGenerated()
			Return boundLiteral
		End Function

		Public Function SwitchSection(ByVal values As List(Of Integer), ByVal ParamArray statements As BoundStatement()) As Microsoft.CodeAnalysis.VisualBasic.BoundCaseBlock
			Dim enumerator As List(Of Integer).Enumerator = New List(Of Integer).Enumerator()
			Dim instance As ArrayBuilder(Of BoundCaseClause) = ArrayBuilder(Of BoundCaseClause).GetInstance()
			Try
				enumerator = values.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Integer = enumerator.Current
					Dim boundSimpleCaseClause As Microsoft.CodeAnalysis.VisualBasic.BoundSimpleCaseClause = New Microsoft.CodeAnalysis.VisualBasic.BoundSimpleCaseClause(Me._syntax, Me.Literal(current), Nothing, False)
					boundSimpleCaseClause.SetWasCompilerGenerated()
					instance.Add(boundSimpleCaseClause)
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			Dim boundCaseStatement As Microsoft.CodeAnalysis.VisualBasic.BoundCaseStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundCaseStatement(Me._syntax, instance.ToImmutableAndFree(), Nothing, False)
			boundCaseStatement.SetWasCompilerGenerated()
			Dim boundCaseBlock As Microsoft.CodeAnalysis.VisualBasic.BoundCaseBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundCaseBlock(Me._syntax, boundCaseStatement, Me.Block(ImmutableArray.Create(Of BoundStatement)(statements)), False)
			boundCaseBlock.SetWasCompilerGenerated()
			Return boundCaseBlock
		End Function

		Public Function SynthesizedLocal(ByVal type As TypeSymbol, Optional ByVal kind As SynthesizedLocalKind = -2, Optional ByVal syntax As SyntaxNode = Nothing) As LocalSymbol
			Return New SynthesizedLocal(Me.CurrentMethod, type, kind, syntax, False)
		End Function

		Public Function SynthesizedParameter(ByVal type As TypeSymbol, ByVal name As String, Optional ByVal container As MethodSymbol = Nothing, Optional ByVal ordinal As Integer = 0) As ParameterSymbol
			Return New SynthesizedParameterSymbol(container, type, ordinal, False, name)
		End Function

		Public Function TernaryConditionalExpression(ByVal condition As BoundExpression, ByVal ifTrue As BoundExpression, ByVal ifFalse As BoundExpression) As BoundTernaryConditionalExpression
			Return (New BoundTernaryConditionalExpression(Me.Syntax, condition, ifTrue, ifFalse, Nothing, ifTrue.Type, False)).MakeCompilerGenerated()
		End Function

		Public Function [Throw](Optional ByVal e As BoundExpression = Nothing) As Microsoft.CodeAnalysis.VisualBasic.BoundThrowStatement
			Dim boundThrowStatement As Microsoft.CodeAnalysis.VisualBasic.BoundThrowStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundThrowStatement(Me._syntax, e, False)
			boundThrowStatement.SetWasCompilerGenerated()
			Return boundThrowStatement
		End Function

		Public Function [Try](ByVal tryBlock As BoundBlock, ByVal catchBlocks As ImmutableArray(Of BoundCatchBlock), Optional ByVal finallyBlock As BoundBlock = Nothing, Optional ByVal exitLabel As LabelSymbol = Nothing) As BoundStatement
			Return New BoundTryStatement(Me.Syntax, tryBlock, catchBlocks, finallyBlock, exitLabel, False)
		End Function

		Public Function [TryCast](ByVal expression As BoundExpression, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As BoundTryCast
			Dim syntax As SyntaxNode = Me.Syntax
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = expression.Type
			Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
			Return New BoundTryCast(syntax, expression, Conversions.ClassifyTryCastConversion(typeSymbol, type, discarded), type, False)
		End Function

		Public Function Type(ByVal typeSym As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundTypeExpression
			Dim boundTypeExpression As Microsoft.CodeAnalysis.VisualBasic.BoundTypeExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundTypeExpression(Me._syntax, typeSym, False)
			boundTypeExpression.SetWasCompilerGenerated()
			Return boundTypeExpression
		End Function

		Public Function TypeArguments(ByVal typeArgs As ImmutableArray(Of TypeSymbol)) As BoundTypeArguments
			Dim boundTypeArgument As BoundTypeArguments = New BoundTypeArguments(Me._syntax, typeArgs)
			boundTypeArgument.SetWasCompilerGenerated()
			Return boundTypeArgument
		End Function

		Public Function [Typeof](ByVal type As WellKnownType) As BoundExpression
			Return Me.[Typeof](Me.WellKnownType(type))
		End Function

		Public Function [Typeof](ByVal typeSym As TypeSymbol) As BoundExpression
			Dim boundGetType As Microsoft.CodeAnalysis.VisualBasic.BoundGetType = New Microsoft.CodeAnalysis.VisualBasic.BoundGetType(Me._syntax, Me.Type(typeSym), Me.WellKnownType(WellKnownType.System_Type), False)
			boundGetType.SetWasCompilerGenerated()
			Return boundGetType
		End Function

		Public Function WellKnownMember(Of T As Symbol)(ByVal wm As WellKnownMember, Optional ByVal isOptional As Boolean = False) As T
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)()
			Dim wellKnownTypeMember As T = DirectCast(Binder.GetWellKnownTypeMember(Me.Compilation, wm, useSiteInfo), T)
			If (useSiteInfo.DiagnosticInfo Is Nothing OrElse Not isOptional) Then
				Me.Diagnostics.Add(useSiteInfo, Me._syntax)
			Else
				wellKnownTypeMember = Nothing
			End If
			Return wellKnownTypeMember
		End Function

		Public Function WellKnownType(ByVal wt As WellKnownType) As NamedTypeSymbol
			Return Binder.GetWellKnownType(Me.Compilation, wt, Me._syntax, Me.Diagnostics)
		End Function
	End Class
End Namespace