Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class ExpressionLambdaRewriter
		Private ReadOnly _binder As Binder

		Private ReadOnly _factory As SyntheticBoundNodeFactory

		Private ReadOnly _typeMap As TypeSubstitution

		Private ReadOnly _expressionType As NamedTypeSymbol

		Private _int32Type As NamedTypeSymbol

		Private _objectType As NamedTypeSymbol

		Private _memberInfoType As NamedTypeSymbol

		Private _memberBindingType As NamedTypeSymbol

		Private _elementInitType As NamedTypeSymbol

		Private ReadOnly _parameterMap As Dictionary(Of ParameterSymbol, BoundExpression)

		Private _recursionDepth As Integer

		Private Const s_coalesceLambdaParameterName As String = "CoalesceLHS"

		Private ReadOnly Property Diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag
			Get
				Return Me._factory.Diagnostics
			End Get
		End Property

		Public ReadOnly Property ElementInitType As NamedTypeSymbol
			Get
				If (Me._elementInitType Is Nothing) Then
					Me._elementInitType = Me._factory.WellKnownType(WellKnownType.System_Linq_Expressions_ElementInit)
				End If
				Return Me._elementInitType
			End Get
		End Property

		Public ReadOnly Property Int32Type As NamedTypeSymbol
			Get
				If (Me._int32Type Is Nothing) Then
					Me._int32Type = Me._factory.SpecialType(SpecialType.System_Int32)
				End If
				Return Me._int32Type
			End Get
		End Property

		Public ReadOnly Property MemberBindingType As NamedTypeSymbol
			Get
				If (Me._memberBindingType Is Nothing) Then
					Me._memberBindingType = Me._factory.WellKnownType(WellKnownType.System_Linq_Expressions_MemberBinding)
				End If
				Return Me._memberBindingType
			End Get
		End Property

		Public ReadOnly Property MemberInfoType As NamedTypeSymbol
			Get
				If (Me._memberInfoType Is Nothing) Then
					Me._memberInfoType = Me._factory.WellKnownType(WellKnownType.System_Reflection_MemberInfo)
				End If
				Return Me._memberInfoType
			End Get
		End Property

		Public ReadOnly Property ObjectType As NamedTypeSymbol
			Get
				If (Me._objectType Is Nothing) Then
					Me._objectType = Me._factory.SpecialType(SpecialType.System_Object)
				End If
				Return Me._objectType
			End Get
		End Property

		Private Sub New(ByVal currentMethod As MethodSymbol, ByVal compilationState As TypeCompilationState, ByVal typeMap As TypeSubstitution, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal node As SyntaxNode, ByVal recursionDepth As Integer, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			MyBase.New()
			Me._parameterMap = New Dictionary(Of ParameterSymbol, BoundExpression)()
			Me._binder = binder
			Me._typeMap = typeMap
			Me._factory = New SyntheticBoundNodeFactory(Nothing, currentMethod, node, compilationState, diagnostics)
			Me._expressionType = Me._factory.WellKnownType(WellKnownType.System_Linq_Expressions_Expression)
			Me._recursionDepth = recursionDepth
		End Sub

		Private Shared Function AdjustCallArgumentForLiftedOperator(ByVal oldArg As BoundExpression, ByVal parameterType As TypeSymbol) As BoundExpression
			Dim boundConversion As BoundExpression
			If (oldArg.Kind = BoundKind.ObjectCreationExpression) Then
				Dim boundObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression = DirectCast(oldArg, Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression)
				If (boundObjectCreationExpression.Arguments.Length <> 1) Then
					boundConversion = New Microsoft.CodeAnalysis.VisualBasic.BoundConversion(oldArg.Syntax, oldArg, ConversionKind.NarrowingNullable, False, False, parameterType, False)
					Return boundConversion
				End If
				boundConversion = boundObjectCreationExpression.Arguments(0)
				Return boundConversion
			End If
			boundConversion = New Microsoft.CodeAnalysis.VisualBasic.BoundConversion(oldArg.Syntax, oldArg, ConversionKind.NarrowingNullable, False, False, parameterType, False)
			Return boundConversion
		End Function

		Private Function AdjustCallForLiftedOperator(ByVal opKind As BinaryOperatorKind, ByVal [call] As BoundCall, ByVal resultType As TypeSymbol) As BoundExpression
			Return Me.AdjustCallForLiftedOperator_DoNotCallDirectly([call], resultType)
		End Function

		Private Function AdjustCallForLiftedOperator(ByVal opKind As UnaryOperatorKind, ByVal [call] As BoundCall, ByVal resultType As TypeSymbol) As BoundExpression
			Return Me.AdjustCallForLiftedOperator_DoNotCallDirectly([call], resultType)
		End Function

		Private Function AdjustCallForLiftedOperator_DoNotCallDirectly(ByVal [call] As BoundCall, ByVal resultType As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim parameters As ImmutableArray(Of ParameterSymbol) = [call].Method.Parameters
			Dim arguments As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = [call].Arguments
			Dim boundExpressionArray(arguments.Length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim length As Integer = arguments.Length - 1
			Dim num As Integer = 0
			Do
				boundExpressionArray(num) = ExpressionLambdaRewriter.AdjustCallArgumentForLiftedOperator(arguments(num), parameters(num).Type)
				num = num + 1
			Loop While num <= length
			Dim returnType As TypeSymbol = [call].Method.ReturnType
			[call] = [call].Update([call].Method, [call].MethodGroupOpt, [call].ReceiverOpt, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpressionArray), [call].DefaultArguments, [call].ConstantValueOpt, [call].IsLValue, [call].SuppressObjectClone, returnType)
			If (resultType.IsNullableType() = returnType.IsNullableType()) Then
				boundExpression = [call]
			Else
				boundExpression = Me._factory.Convert(resultType, [call], False)
			End If
			Return boundExpression
		End Function

		Private Function BuildIndices(ByVal expressions As ImmutableArray(Of BoundExpression)) As BoundExpression
			Dim length As Integer = expressions.Length
			Dim boundExpressionArray(length - 1 + 1 - 1) As BoundExpression
			Dim num As Integer = length - 1
			Dim num1 As Integer = 0
			Do
				boundExpressionArray(num1) = Me.Visit(expressions(num1))
				num1 = num1 + 1
			Loop While num1 <= num
			Return Me._factory.Array(Me._expressionType, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundExpressionArray))
		End Function

		Private Function BuildLambdaBodyForCoalesce(ByVal conversion As BoundConversion, ByVal toType As TypeSymbol, ByVal lambdaParameter As ParameterSymbol, ByVal isChecked As Boolean) As BoundExpression
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundExpression Microsoft.CodeAnalysis.VisualBasic.ExpressionLambdaRewriter::BuildLambdaBodyForCoalesce(Microsoft.CodeAnalysis.VisualBasic.BoundConversion,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol,System.Boolean)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundExpression BuildLambdaBodyForCoalesce(Microsoft.CodeAnalysis.VisualBasic.BoundConversion,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol,System.Boolean)
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

		Private Function BuildLambdaBodyForCoalesce(ByVal opKind As UnaryOperatorKind, ByVal [call] As BoundCall, ByVal resultType As TypeSymbol, ByVal lambdaParameter As ParameterSymbol) As BoundExpression
			Dim method As MethodSymbol = [call].Method
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(Me.CreateCoalesceLambdaParameter(lambdaParameter))
			Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
			Return Me.AdjustCallForLiftedOperator(opKind, [call].Update(method, Nothing, Nothing, boundExpressions, bitVector, Nothing, [call].IsLValue, True, [call].Type), resultType)
		End Function

		Private Function BuildLambdaForCoalesceCall(ByVal toType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal lambdaParameter As ParameterSymbol, ByVal body As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me._factory.WellKnownType(WellKnownType.System_Linq_Expressions_ParameterExpression)
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Me._factory.SynthesizedLocal(typeSymbol, SynthesizedLocalKind.LoweringTemp, Nothing)
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me._factory.Local(localSymbol, True)
			Dim expressionTree As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.ConvertRuntimeHelperToExpressionTree("Parameter", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { Me._factory.[Typeof](lambdaParameter.Type), Me._factory.Literal("CoalesceLHS") })
			Me._parameterMap(lambdaParameter) = boundLocal.MakeRValue()
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.Visit(body)
			Me._parameterMap.Remove(lambdaParameter)
			Return Me._factory.Sequence(ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)(localSymbol), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(Me._factory.AssignmentExpression(boundLocal, expressionTree)), Me.ConvertRuntimeHelperToExpressionTree("Lambda", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, Me._factory.Array(typeSymbol, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundLocal.MakeRValue())) }))
		End Function

		Private Function [Call](ByVal receiver As BoundExpression, ByVal method As MethodSymbol, ByVal ParamArray params As BoundExpression()) As BoundExpression
			Dim boundExpressionArray(CInt(params.Length) + 1 + 1 - 1) As BoundExpression
			boundExpressionArray(0) = receiver
			boundExpressionArray(1) = Me._factory.MethodInfo(method)
			Array.Copy(params, 0, boundExpressionArray, 2, CInt(params.Length))
			Return Me.ConvertRuntimeHelperToExpressionTree("Call", boundExpressionArray)
		End Function

		Private Function Convert(ByVal expr As BoundExpression, ByVal type As TypeSymbol, ByVal isChecked As Boolean) As BoundExpression
			Return Me.ConvertRuntimeHelperToExpressionTree(If(isChecked, "ConvertChecked", "Convert"), New BoundExpression() { expr, Me._factory.[Typeof](type) })
		End Function

		Private Function Convert(ByVal expr As BoundExpression, ByVal type As TypeSymbol, ByVal helper As MethodSymbol, ByVal isChecked As Boolean) As BoundExpression
			Return Me.ConvertRuntimeHelperToExpressionTree(If(isChecked, "ConvertChecked", "Convert"), New BoundExpression() { expr, Me._factory.[Typeof](type), Me._factory.MethodInfo(helper) })
		End Function

		Private Function ConvertArgumentsIntoArray(ByVal exprs As ImmutableArray(Of BoundExpression)) As BoundExpression
			Dim boundExpressionArray(exprs.Length - 1 + 1 - 1) As BoundExpression
			Dim length As Integer = exprs.Length - 1
			Dim num As Integer = 0
			Do
				boundExpressionArray(num) = Me.Visit(exprs(num))
				num = num + 1
			Loop While num <= length
			Return Me._factory.Array(Me._expressionType, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundExpressionArray))
		End Function

		Private Function ConvertBinaryOperator(ByVal node As BoundBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim expressionTree As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = node.Type
			Dim nullableUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type.GetNullableUnderlyingTypeOrSelf()
			Dim enumUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = nullableUnderlyingTypeOrSelf.GetEnumUnderlyingTypeOrSelf()
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = enumUnderlyingTypeOrSelf.SpecialType
			Dim operatorKind As BinaryOperatorKind = node.OperatorKind And BinaryOperatorKind.OpMask
			Dim helperForObjectBinaryOperation As MethodSymbol = Nothing
			If (specialType = Microsoft.CodeAnalysis.SpecialType.System_Object) Then
				helperForObjectBinaryOperation = Me.GetHelperForObjectBinaryOperation(operatorKind)
			ElseIf (specialType = Microsoft.CodeAnalysis.SpecialType.System_Decimal) Then
				helperForObjectBinaryOperation = Me.GetHelperForDecimalBinaryOperation(operatorKind)
			ElseIf (operatorKind = BinaryOperatorKind.Concatenate) Then
				helperForObjectBinaryOperation = DirectCast(Me._factory.SpecialMember(SpecialMember.System_String__ConcatStringString), MethodSymbol)
			ElseIf (operatorKind = BinaryOperatorKind.Power) Then
				helperForObjectBinaryOperation = Me._factory.WellKnownMember(Of MethodSymbol)(WellKnownMember.System_Math__PowDoubleDouble, False)
			End If
			Dim flag As Boolean = If(Not node.Checked, False, Me.IsIntegralType(enumUnderlyingTypeOrSelf))
			Dim binaryOperatorMethodName As String = ExpressionLambdaRewriter.GetBinaryOperatorMethodName(operatorKind, flag)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.Visit(node.Left)
			If (helperForObjectBinaryOperation Is Nothing) Then
				Dim flag1 As Boolean = type.IsNullableType()
				Dim flag2 As Boolean = If(specialType = Microsoft.CodeAnalysis.SpecialType.System_Byte, True, specialType = Microsoft.CodeAnalysis.SpecialType.System_SByte)
				boundExpression1 = Me.GenerateCastsForBinaryAndUnaryOperator(boundExpression1, flag1, nullableUnderlyingTypeOrSelf, If(Not flag, False, Me.IsIntegralType(enumUnderlyingTypeOrSelf)), flag2)
				If (operatorKind = BinaryOperatorKind.LeftShift OrElse operatorKind = BinaryOperatorKind.RightShift) Then
					boundExpression = Me.MaskShiftCountOperand(node, type, flag)
					flag = False
				Else
					boundExpression = Me.Visit(node.Right)
					boundExpression = Me.GenerateCastsForBinaryAndUnaryOperator(boundExpression, flag1, nullableUnderlyingTypeOrSelf, If(Not flag, False, Me.IsIntegralType(enumUnderlyingTypeOrSelf)), flag2)
				End If
				Dim expressionTree1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.ConvertRuntimeHelperToExpressionTree(binaryOperatorMethodName, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression1, boundExpression })
				If (flag2) Then
					Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = expressionTree1
					If (flag1) Then
						typeSymbol = Me._factory.NullableOf(enumUnderlyingTypeOrSelf)
					Else
						typeSymbol = enumUnderlyingTypeOrSelf
					End If
					expressionTree1 = Me.Convert(boundExpression2, typeSymbol, flag)
				End If
				If (nullableUnderlyingTypeOrSelf.IsEnumType()) Then
					expressionTree1 = Me.Convert(expressionTree1, type, False)
				End If
				expressionTree = expressionTree1
			Else
				boundExpression = Me.Visit(node.Right)
				expressionTree = Me.ConvertRuntimeHelperToExpressionTree(binaryOperatorMethodName, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression1, boundExpression, Me._factory.MethodInfo(helperForObjectBinaryOperation) })
			End If
			Return expressionTree
		End Function

		Private Function ConvertBooleanOperator(ByVal node As BoundBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim expressionTree As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = node.Type
			Dim enumUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type.GetNullableUnderlyingTypeOrSelf().GetEnumUnderlyingTypeOrSelf()
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = enumUnderlyingTypeOrSelf.SpecialType
			Dim operatorKind As BinaryOperatorKind = node.OperatorKind And BinaryOperatorKind.OpMask
			Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Left
			Dim type1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = left.Type
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
			Dim right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Right
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
			Dim flag As Boolean = If(operatorKind = BinaryOperatorKind.[Is], True, operatorKind = BinaryOperatorKind.[IsNot])
			If (flag) Then
				If (left.IsNothingLiteral()) Then
					If (right.Type.IsNullableType()) Then
						boundExpression = Me.CreateLiteralExpression(left, right.Type)
						type1 = right.Type
					End If
				ElseIf (right.IsNothingLiteral() AndAlso left.Type.IsNullableType()) Then
					boundExpression1 = Me.CreateLiteralExpression(right, left.Type)
				End If
			End If
			Dim flag1 As Boolean = type1.IsNullableType()
			Dim nullableUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type1.GetNullableUnderlyingTypeOrSelf()
			Dim enumUnderlyingTypeOrSelf1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = nullableUnderlyingTypeOrSelf.GetEnumUnderlyingTypeOrSelf()
			Dim specialType1 As Microsoft.CodeAnalysis.SpecialType = enumUnderlyingTypeOrSelf1.SpecialType
			If (boundExpression Is Nothing) Then
				boundExpression = Me.Visit(left)
			End If
			If (boundExpression1 Is Nothing) Then
				boundExpression1 = Me.Visit(right)
			End If
			Dim helperForDecimalBinaryOperation As MethodSymbol = Nothing
			If (specialType1 = Microsoft.CodeAnalysis.SpecialType.System_Decimal) Then
				helperForDecimalBinaryOperation = Me.GetHelperForDecimalBinaryOperation(operatorKind)
			ElseIf (specialType1 = Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
				helperForDecimalBinaryOperation = Me.GetHelperForDateTimeBinaryOperation(operatorKind)
			End If
			Dim flag2 As Boolean = If(Not node.Checked, False, Me.IsIntegralType(enumUnderlyingTypeOrSelf))
			Dim binaryOperatorMethodName As String = ExpressionLambdaRewriter.GetBinaryOperatorMethodName(operatorKind, flag2)
			If (helperForDecimalBinaryOperation Is Nothing) Then
				Dim flag3 As Boolean = False
				If (specialType1 = Microsoft.CodeAnalysis.SpecialType.System_Boolean) Then
					Select Case operatorKind
						Case BinaryOperatorKind.LessThanOrEqual
							binaryOperatorMethodName = ExpressionLambdaRewriter.GetBinaryOperatorMethodName(BinaryOperatorKind.GreaterThanOrEqual, flag2)
							flag3 = True
							Exit Select
						Case BinaryOperatorKind.GreaterThanOrEqual
							binaryOperatorMethodName = ExpressionLambdaRewriter.GetBinaryOperatorMethodName(BinaryOperatorKind.LessThanOrEqual, flag2)
							flag3 = True
							Exit Select
						Case BinaryOperatorKind.LessThan
							binaryOperatorMethodName = ExpressionLambdaRewriter.GetBinaryOperatorMethodName(BinaryOperatorKind.GreaterThan, flag2)
							flag3 = True
							Exit Select
						Case BinaryOperatorKind.GreaterThan
							binaryOperatorMethodName = ExpressionLambdaRewriter.GetBinaryOperatorMethodName(BinaryOperatorKind.LessThan, flag2)
							flag3 = True
							Exit Select
					End Select
				End If
				Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type1
				If (nullableUnderlyingTypeOrSelf.IsEnumType() AndAlso Not flag) Then
					If (flag1) Then
						typeSymbol = Me._factory.NullableOf(enumUnderlyingTypeOrSelf1)
					Else
						typeSymbol = enumUnderlyingTypeOrSelf1
					End If
					Dim typeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeSymbol
					boundExpression = Me.CreateBuiltInConversion(typeSymbol1, typeSymbol2, boundExpression, node.Checked, False, ExpressionLambdaRewriter.ConversionSemantics.[Default], False)
					boundExpression1 = Me.CreateBuiltInConversion(typeSymbol1, typeSymbol2, boundExpression1, node.Checked, False, ExpressionLambdaRewriter.ConversionSemantics.[Default], False)
					typeSymbol1 = typeSymbol2
				End If
				If (flag3 AndAlso Not flag) Then
					Dim typeSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = If(flag1, Me._factory.NullableOf(Me.Int32Type), Me.Int32Type)
					boundExpression = Me.Convert(boundExpression, typeSymbol3, node.Checked)
					boundExpression1 = Me.Convert(boundExpression1, typeSymbol3, node.Checked)
					typeSymbol1 = typeSymbol3
				End If
				expressionTree = Me.ConvertRuntimeHelperToExpressionTree(binaryOperatorMethodName, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, boundExpression1, Me._factory.Literal(type.IsNullableType()), Me._factory.Null() })
			Else
				expressionTree = Me.ConvertRuntimeHelperToExpressionTree(binaryOperatorMethodName, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, boundExpression1, Me._factory.Literal(type.IsNullableType()), Me._factory.MethodInfo(helperForDecimalBinaryOperation) })
			End If
			Return expressionTree
		End Function

		Private Function ConvertExpression(ByVal operand As BoundExpression, ByVal conversion As ConversionKind, ByVal typeFrom As TypeSymbol, ByVal typeTo As TypeSymbol, ByVal isChecked As Boolean, ByVal explicitCastInCode As Boolean, ByVal semantics As ExpressionLambdaRewriter.ConversionSemantics) As BoundExpression
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundExpression Microsoft.CodeAnalysis.VisualBasic.ExpressionLambdaRewriter::ConvertExpression(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.ConversionKind,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Boolean,System.Boolean,Microsoft.CodeAnalysis.VisualBasic.ExpressionLambdaRewriter/ConversionSemantics)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundExpression ConvertExpression(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.ConversionKind,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Boolean,System.Boolean,Microsoft.CodeAnalysis.VisualBasic.ExpressionLambdaRewriter/ConversionSemantics)
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

		Private Function ConvertIfNeeded(ByVal operand As BoundExpression, ByVal oldType As TypeSymbol, ByVal newType As TypeSymbol, ByVal isChecked As Boolean) As BoundExpression
			If (TypeSymbol.Equals(oldType, newType, TypeCompareKind.ConsiderEverything)) Then
				Return operand
			End If
			Return Me.Convert(operand, newType, isChecked)
		End Function

		Private Function ConvertLambda(ByVal node As BoundLambda, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim expressionTree As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (Not type.IsExpressionTree(Me._binder)) Then
				expressionTree = Me.VisitLambdaInternal(node, DirectCast(type, NamedTypeSymbol))
			Else
				type = type.ExpressionTargetDelegate(Me._factory.Compilation)
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitLambdaInternal(node, DirectCast(type, NamedTypeSymbol))
				expressionTree = Me.ConvertRuntimeHelperToExpressionTree("Quote", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression })
			End If
			Return expressionTree
		End Function

		Private Function ConvertNullableToUnderlying(ByVal operand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal nullableType As TypeSymbol, ByVal isChecked As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim nullableUnderlyingType As TypeSymbol = nullableType.GetNullableUnderlyingType()
			If (isChecked AndAlso Not Me.IsIntegralType(nullableUnderlyingType)) Then
				isChecked = False
			End If
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Me._factory.SpecialMember(SpecialMember.System_Nullable_T__op_Explicit_ToT), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			If (methodSymbol Is Nothing) Then
				boundExpression = Me.Convert(operand, nullableUnderlyingType, isChecked)
			Else
				Dim substitutedNamedType As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedNamedType = DirectCast(nullableType, Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedNamedType)
				boundExpression = Me.Convert(operand, nullableUnderlyingType, DirectCast(substitutedNamedType.GetMemberForDefinition(methodSymbol), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol), isChecked)
			End If
			Return boundExpression
		End Function

		Private Function ConvertRuntimeHelperToExpressionTree(ByVal helperMethodName As String, ByVal ParamArray arguments As BoundExpression()) As BoundExpression
			Return Me.ConvertRuntimeHelperToExpressionTree(helperMethodName, ImmutableArray(Of TypeSymbol).Empty, arguments)
		End Function

		Private Function ConvertRuntimeHelperToExpressionTree(ByVal helperMethodName As String, ByVal typeArgs As ImmutableArray(Of TypeSymbol), ByVal ParamArray arguments As Microsoft.CodeAnalysis.VisualBasic.BoundExpression()) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim flag As Boolean = False
			Dim boundExpressionArray As Microsoft.CodeAnalysis.VisualBasic.BoundExpression() = arguments
			Dim num As Integer = 0
			Do
				If (boundExpressionArray(num).HasErrors) Then
					flag = True
				End If
				num = num + 1
			Loop While num < CInt(boundExpressionArray.Length)
			Dim exprFactoryMethodGroup As BoundMethodGroup = Me.GetExprFactoryMethodGroup(helperMethodName, typeArgs)
			If (exprFactoryMethodGroup Is Nothing OrElse flag) Then
				boundExpression = Me._factory.BadExpression(arguments)
			Else
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me._binder
				Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = Me._factory.Syntax
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = Me._factory.Syntax
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(arguments)
				Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
				boundExpression = binder.BindInvocationExpression(syntax, syntaxNode, TypeCharacter.None, exprFactoryMethodGroup, boundExpressions, strs, Me.Diagnostics, Nothing, False, False, False, Nothing, False)
			End If
			Return boundExpression
		End Function

		Private Function ConvertShortCircuitedBooleanOperator(ByVal node As BoundBinaryOperator) As BoundExpression
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundExpression Microsoft.CodeAnalysis.VisualBasic.ExpressionLambdaRewriter::ConvertShortCircuitedBooleanOperator(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundExpression ConvertShortCircuitedBooleanOperator(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
			' 
			' File d'attente vide.
			'    à System.ThrowHelper.ThrowInvalidOperationException(ExceptionResource resource)
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.(ICollection`1 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 525
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 445
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 363
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 307
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 86
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Function ConvertUnderlyingToNullable(ByVal operand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal nullableType As TypeSymbol, ByVal isChecked As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (isChecked AndAlso Not Me.IsIntegralType(nullableType)) Then
				isChecked = False
			End If
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Me._factory.SpecialMember(SpecialMember.System_Nullable_T__op_Implicit_FromT), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			If (methodSymbol Is Nothing) Then
				boundExpression = Me.Convert(operand, nullableType, isChecked)
			Else
				Dim substitutedNamedType As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedNamedType = DirectCast(nullableType, Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedNamedType)
				boundExpression = Me.Convert(operand, nullableType, DirectCast(substitutedNamedType.GetMemberForDefinition(methodSymbol), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol), isChecked)
			End If
			Return boundExpression
		End Function

		Private Function ConvertUserDefinedLikeOrConcate(ByVal node As BoundUserDefinedBinaryOperator) As BoundExpression
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundExpression Microsoft.CodeAnalysis.VisualBasic.ExpressionLambdaRewriter::ConvertUserDefinedLikeOrConcate(Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundExpression ConvertUserDefinedLikeOrConcate(Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator)
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

		Private Function CreateBuiltInConversion(ByVal typeFrom As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal typeTo As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal rewrittenOperand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal isChecked As Boolean, ByVal isExplicit As Boolean, ByVal semantics As ExpressionLambdaRewriter.ConversionSemantics, Optional ByVal specialConversionForNullable As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim nullable As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim flag As Boolean = typeFrom.IsNullableType()
			Dim flag1 As Boolean = typeTo.IsNullableType()
			Dim nullableUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeTo.GetNullableUnderlyingTypeOrSelf()
			Dim enumUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = nullableUnderlyingTypeOrSelf.GetEnumUnderlyingTypeOrSelf()
			Dim nullableUnderlyingTypeOrSelf1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeFrom.GetNullableUnderlyingTypeOrSelf()
			Dim enumUnderlyingTypeOrSelf1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = nullableUnderlyingTypeOrSelf1.GetEnumUnderlyingTypeOrSelf()
			If (flag OrElse flag1) Then
				If (Not flag OrElse flag1 OrElse typeTo.IsObjectType()) Then
					If (Not flag1 OrElse flag OrElse typeFrom.IsObjectType()) Then
						GoTo Label1
					End If
					If (Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(nullableUnderlyingTypeOrSelf1, nullableUnderlyingTypeOrSelf, TypeCompareKind.ConsiderEverything)) Then
						Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = nullableUnderlyingTypeOrSelf
						rewrittenOperand = Me.CreateBuiltInConversion(typeFrom, typeSymbol1, rewrittenOperand, isChecked, isExplicit, ExpressionLambdaRewriter.ConversionSemantics.[Default], True)
						nullable = Me.CreateBuiltInConversion(typeSymbol1, typeTo, rewrittenOperand, isChecked, isExplicit, ExpressionLambdaRewriter.ConversionSemantics.[Default], True)
						Return nullable
					ElseIf (Not specialConversionForNullable) Then
						nullable = Me.Convert(rewrittenOperand, typeTo, If(Not isChecked, False, Me.IsIntegralType(nullableUnderlyingTypeOrSelf)))
						Return nullable
					Else
						nullable = Me.ConvertUnderlyingToNullable(rewrittenOperand, typeTo, isChecked)
						Return nullable
					End If
				ElseIf (Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(nullableUnderlyingTypeOrSelf1, nullableUnderlyingTypeOrSelf, TypeCompareKind.ConsiderEverything)) Then
					Dim typeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = nullableUnderlyingTypeOrSelf1
					rewrittenOperand = Me.CreateBuiltInConversion(typeFrom, typeSymbol2, rewrittenOperand, isChecked, isExplicit, ExpressionLambdaRewriter.ConversionSemantics.[Default], True)
					nullable = Me.CreateBuiltInConversion(typeSymbol2, typeTo, rewrittenOperand, isChecked, isExplicit, ExpressionLambdaRewriter.ConversionSemantics.[Default], True)
					Return nullable
				ElseIf (Not specialConversionForNullable) Then
					nullable = Me.Convert(rewrittenOperand, typeTo, If(Not isChecked, False, Me.IsIntegralType(nullableUnderlyingTypeOrSelf)))
					Return nullable
				Else
					nullable = Me.ConvertNullableToUnderlying(rewrittenOperand, typeFrom, isChecked)
					Return nullable
				End If
			End If
		Label1:
			Dim conversionHelperMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (semantics = ExpressionLambdaRewriter.ConversionSemantics.[Default]) Then
				conversionHelperMethod = Me.GetConversionHelperMethod(enumUnderlyingTypeOrSelf1.SpecialType, enumUnderlyingTypeOrSelf.SpecialType)
			End If
			If (conversionHelperMethod IsNot Nothing) Then
				Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = conversionHelperMethod.Parameters(0).Type
				If (flag) Then
					type = Me._factory.NullableOf(type)
				End If
				Dim returnType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = conversionHelperMethod.ReturnType
				If (flag1) Then
					returnType = Me._factory.NullableOf(returnType)
				End If
				nullable = Me.ConvertIfNeeded(Me.Convert(Me.ConvertIfNeeded(rewrittenOperand, typeFrom, type, If(Not isChecked, False, Me.IsIntegralType(type))), returnType, conversionHelperMethod, If(Not isChecked, False, Me.IsIntegralType(returnType))), returnType, typeTo, If(Not isChecked, False, Me.IsIntegralType(enumUnderlyingTypeOrSelf)))
			ElseIf (enumUnderlyingTypeOrSelf1.IsObjectType() AndAlso enumUnderlyingTypeOrSelf.IsTypeParameter()) Then
				Dim conversionSemantic As ExpressionLambdaRewriter.ConversionSemantics = semantics
				If (conversionSemantic = ExpressionLambdaRewriter.ConversionSemantics.[DirectCast]) Then
					nullable = Me.Convert(rewrittenOperand, typeTo, False)
				ElseIf (conversionSemantic = ExpressionLambdaRewriter.ConversionSemantics.[TryCast]) Then
					nullable = Me.CreateTypeAs(rewrittenOperand, typeTo)
				Else
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToGenericParameter_T_Object, False)
					nullable = Me.[Call](Me._factory.Null(), methodSymbol.Construct(New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol() { typeTo }), New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { rewrittenOperand })
				End If
			ElseIf (enumUnderlyingTypeOrSelf1.IsTypeParameter()) Then
				If (semantics <> ExpressionLambdaRewriter.ConversionSemantics.[TryCast]) Then
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.Convert(rewrittenOperand, Me.ObjectType, False)
					nullable = Me.ConvertIfNeeded(boundExpression, Me._factory.SpecialType(SpecialType.System_Object), typeTo, False)
				Else
					nullable = Me.CreateTypeAs(If(typeTo.SpecialType = SpecialType.System_Object, rewrittenOperand, Me.Convert(rewrittenOperand, Me.ObjectType, False)), typeTo)
				End If
			ElseIf (enumUnderlyingTypeOrSelf.IsStringType() AndAlso enumUnderlyingTypeOrSelf1.IsCharSZArray()) Then
				nullable = Me.[New](SpecialMember.System_String__CtorSZArrayChar, rewrittenOperand)
			ElseIf (enumUnderlyingTypeOrSelf1.IsReferenceType AndAlso enumUnderlyingTypeOrSelf.IsCharSZArray()) Then
				If (Not enumUnderlyingTypeOrSelf1.IsStringType()) Then
					symbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)(WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharArrayRankOneObject, False)
					typeSymbol = Me._factory.SpecialType(SpecialType.System_Object)
				Else
					symbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)(WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharArrayRankOneString, False)
					typeSymbol = Me._factory.SpecialType(SpecialType.System_String)
				End If
				nullable = Me.Convert(Me.ConvertIfNeeded(rewrittenOperand, typeFrom, typeSymbol, False), typeTo, DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol), False)
			ElseIf (enumUnderlyingTypeOrSelf1.IsBooleanType() AndAlso enumUnderlyingTypeOrSelf.IsNumericType()) Then
				Dim signedVersionOfNumericType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.GetSignedVersionOfNumericType(enumUnderlyingTypeOrSelf)
				If (signedVersionOfNumericType.SpecialType = SpecialType.System_SByte) Then
					signedVersionOfNumericType = Me._factory.SpecialType(SpecialType.System_Int32)
				End If
				If (isChecked AndAlso CObj(signedVersionOfNumericType) <> CObj(enumUnderlyingTypeOrSelf)) Then
					isChecked = False
				End If
				If (flag AndAlso CObj(signedVersionOfNumericType) = CObj(enumUnderlyingTypeOrSelf)) Then
					signedVersionOfNumericType = Me._factory.NullableOf(signedVersionOfNumericType)
				End If
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.Convert(rewrittenOperand, signedVersionOfNumericType, If(Not isChecked, False, Me.IsIntegralType(signedVersionOfNumericType)))
				nullable = Me.ConvertIfNeeded(Me.Negate(boundExpression1), signedVersionOfNumericType, typeTo, If(Not isChecked, False, Me.IsIntegralType(signedVersionOfNumericType)))
			ElseIf (Not isExplicit OrElse Not enumUnderlyingTypeOrSelf.IsFloatingType()) Then
				nullable = If(semantics <> ExpressionLambdaRewriter.ConversionSemantics.[TryCast], Me.ConvertIfNeeded(rewrittenOperand, typeFrom, typeTo, If(Not isChecked, False, Me.IsIntegralType(enumUnderlyingTypeOrSelf))), Me.CreateTypeAsIfNeeded(rewrittenOperand, typeFrom, typeTo))
			Else
				nullable = Me.Convert(rewrittenOperand, typeTo, If(Not isChecked, False, Me.IsIntegralType(enumUnderlyingTypeOrSelf)))
			End If
			Return nullable
		End Function

		Private Function CreateCoalesceLambdaParameter(ByVal paramSymbol As ParameterSymbol) As BoundExpression
			Return Me._factory.Parameter(paramSymbol).MakeRValue()
		End Function

		Private Function CreateCoalesceLambdaParameterSymbol(ByVal paramType As TypeSymbol) As ParameterSymbol
			Return Me._factory.SynthesizedParameter(paramType, "CoalesceLHS", Nothing, 0)
		End Function

		Private Function CreateLiteralExpression(ByVal node As BoundExpression) As BoundExpression
			Return Me.CreateLiteralExpression(node, node.Type)
		End Function

		Private Function CreateLiteralExpression(ByVal node As BoundExpression, ByVal type As TypeSymbol) As BoundExpression
			Return Me.ConvertRuntimeHelperToExpressionTree("Constant", New BoundExpression() { Me._factory.Convert(Me.ObjectType, node, False), Me._factory.[Typeof](type) })
		End Function

		Private Function CreateTypeAs(ByVal expr As BoundExpression, ByVal type As TypeSymbol) As BoundExpression
			Return Me.ConvertRuntimeHelperToExpressionTree("TypeAs", New BoundExpression() { expr, Me._factory.[Typeof](type) })
		End Function

		Private Function CreateTypeAsIfNeeded(ByVal operand As BoundExpression, ByVal oldType As TypeSymbol, ByVal newType As TypeSymbol) As BoundExpression
			If (TypeSymbol.Equals(oldType, newType, TypeCompareKind.ConsiderEverything)) Then
				Return operand
			End If
			Return Me.CreateTypeAs(operand, newType)
		End Function

		Private Function CreateUserDefinedConversion(ByVal node As BoundUserDefinedConversion, ByVal resultType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal isLifted As Boolean, ByVal isChecked As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim outConversionOpt As BoundConversion
			Dim arguments As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim flag As Boolean
			Dim [call] As BoundCall = node.[Call]
			Dim method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = [call].Method
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = [call].Type
			If (isLifted) Then
				Dim operand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Operand
				If (operand.Type.IsNullableType() <> resultType.IsNullableType()) Then
					arguments = [call].Arguments
					boundExpression1 = Me.Visit(arguments(0))
					boundExpression3 = boundExpression1
					typeSymbol = type
					methodSymbol = method
					flag = If(Not isChecked, False, Me.IsIntegralType(type))
					boundExpression2 = Me.Convert(boundExpression3, typeSymbol, methodSymbol, flag)
					outConversionOpt = node.OutConversionOpt
					boundExpression = If(outConversionOpt Is Nothing, boundExpression2, Me.CreateBuiltInConversion(type, resultType, boundExpression2, outConversionOpt.Checked, outConversionOpt.ExplicitCastInCode, ExpressionLambdaRewriter.ConversionSemantics.[Default], False))
					Return boundExpression
				End If
				boundExpression = Me.Convert(Me.Visit(operand), resultType, method, If(Not isChecked, False, Me.IsIntegralType(resultType)))
				Return boundExpression
			End If
			arguments = [call].Arguments
			boundExpression1 = Me.Visit(arguments(0))
			boundExpression3 = boundExpression1
			typeSymbol = type
			methodSymbol = method
			flag = If(Not isChecked, False, Me.IsIntegralType(type))
			boundExpression2 = Me.Convert(boundExpression3, typeSymbol, methodSymbol, flag)
			outConversionOpt = node.OutConversionOpt
			boundExpression = If(outConversionOpt Is Nothing, boundExpression2, Me.CreateBuiltInConversion(type, resultType, boundExpression2, outConversionOpt.Checked, outConversionOpt.ExplicitCastInCode, ExpressionLambdaRewriter.ConversionSemantics.[Default], False))
			Return boundExpression
		End Function

		Private Function CreateUserDefinedNullableToUnderlyingConversion(ByVal expression As BoundExpression, ByVal nullableType As TypeSymbol, ByVal isChecked As Boolean) As BoundExpression
			Dim boundConversion As BoundExpression
			Dim nullableUnderlyingType As TypeSymbol = nullableType.GetNullableUnderlyingType()
			Dim memberForDefinition As MethodSymbol = DirectCast(Me._factory.SpecialMember(SpecialMember.System_Nullable_T__op_Explicit_ToT), MethodSymbol)
			If (memberForDefinition IsNot Nothing) Then
				memberForDefinition = DirectCast(DirectCast(nullableType, SubstitutedNamedType).GetMemberForDefinition(memberForDefinition), MethodSymbol)
				Dim syntax As SyntaxNode = expression.Syntax
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(expression)
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				boundConversion = New Microsoft.CodeAnalysis.VisualBasic.BoundConversion(syntax, New BoundUserDefinedConversion(syntax, New BoundCall(syntax, memberForDefinition, Nothing, Nothing, boundExpressions, Nothing, nullableUnderlyingType, True, False, bitVector), 0, nullableType, False), ConversionKind.[Narrowing] Or ConversionKind.UserDefined, isChecked, False, Nothing, nullableUnderlyingType, False)
			Else
				boundConversion = Me._factory.Convert(nullableUnderlyingType, expression, isChecked)
			End If
			Return boundConversion
		End Function

		Private Function [Default](ByVal type As TypeSymbol) As BoundExpression
			Return Me.ConvertRuntimeHelperToExpressionTree("Default", New BoundExpression() { Me._factory.[Typeof](type) })
		End Function

		Private Function GenerateCastsForBinaryAndUnaryOperator(ByVal loweredOperand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal isNullable As Boolean, ByVal notNullableType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal checked As Boolean, ByVal needToCastBackToByteOrSByte As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			If (notNullableType.IsEnumType()) Then
				Dim enumUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = notNullableType.GetEnumUnderlyingTypeOrSelf()
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = loweredOperand
				If (isNullable) Then
					typeSymbol = Me._factory.NullableOf(enumUnderlyingTypeOrSelf)
				Else
					typeSymbol = enumUnderlyingTypeOrSelf
				End If
				loweredOperand = Me.Convert(boundExpression, typeSymbol, False)
			End If
			If (needToCastBackToByteOrSByte) Then
				loweredOperand = Me.Convert(loweredOperand, If(isNullable, Me._factory.NullableOf(Me.Int32Type), Me.Int32Type), checked)
			End If
			Return loweredOperand
		End Function

		Private Function GenerateDiagnosticAndReturnDummyExpression(ByVal code As ERRID, ByVal node As BoundNode, ByVal ParamArray args As Object()) As BoundExpression
			Me.Diagnostics.Add(New VBDiagnostic(ErrorFactory.ErrorInfo(code, args), node.Syntax.GetLocation(), False))
			Return Me.VisitInternal(Me._factory.Literal("Diagnostics Generated"))
		End Function

		Private Shared Function GetBinaryOperatorMethodName(ByVal opKind As BinaryOperatorKind, ByVal isChecked As Boolean) As String
			' 
			' Current member / type: System.String Microsoft.CodeAnalysis.VisualBasic.ExpressionLambdaRewriter::GetBinaryOperatorMethodName(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind,System.Boolean)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.String GetBinaryOperatorMethodName(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind,System.Boolean)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 84
			'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 73
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 27
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 198
			'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 79
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
			'    à ..Visit[,]( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 286
			'    à ..Visit( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 317
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 337
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 44
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Function GetConversionHelperMethod(ByVal stFrom As Microsoft.CodeAnalysis.SpecialType, ByVal stTo As Microsoft.CodeAnalysis.SpecialType) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Math__PowDoubleDouble Or Microsoft.CodeAnalysis.WellKnownMember.System_Array__get_Length Or Microsoft.CodeAnalysis.WellKnownMember.System_Array__Empty Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanDecimal Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanInt32 Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanUInt32 Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanInt64 Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanUInt64 Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanSingle Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanDouble Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToSByteDecimal Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToSByteDouble Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToSByteSingle Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToByteDecimal Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToByteDouble Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToByteSingle Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt16Decimal Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt16Double Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt16Single Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt16Decimal Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt16Double Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt16Single Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt32Decimal Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt32Double Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt32Single Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt32Decimal Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt32Double Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt32Single Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt64Decimal Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt64Double Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt64Single Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt64Decimal Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt64Double Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt64Single Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToSingleDecimal Or Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToDoubleDecimal Or Microsoft.CodeAnalysis.WellKnownMember.System_CLSCompliantAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_FlagsAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Guid__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Type__GetTypeFromCLSID Or Microsoft.CodeAnalysis.WellKnownMember.System_Type__GetTypeFromHandle Or Microsoft.CodeAnalysis.WellKnownMember.System_Type__Missing Or Microsoft.CodeAnalysis.WellKnownMember.System_Type__op_Equality Or Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_AssemblyKeyFileAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_AssemblyKeyNameAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_MethodBase__GetMethodFromHandle Or Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_MethodBase__GetMethodFromHandle2 Or Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_MethodInfo__CreateDelegate Or Microsoft.CodeAnalysis.WellKnownMember.System_Delegate__CreateDelegate Or Microsoft.CodeAnalysis.WellKnownMember.System_Delegate__CreateDelegate4 Or Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_FieldInfo__GetFieldFromHandle Or Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_FieldInfo__GetFieldFromHandle2 Or Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_Missing__Value Or Microsoft.CodeAnalysis.WellKnownMember.System_IEquatable_T__Equals Or Microsoft.CodeAnalysis.WellKnownMember.System_Collections_Generic_IEqualityComparer_T__Equals Or Microsoft.CodeAnalysis.WellKnownMember.System_Collections_Generic_EqualityComparer_T__Equals Or Microsoft.CodeAnalysis.WellKnownMember.System_Collections_Generic_EqualityComparer_T__GetHashCode Or Microsoft.CodeAnalysis.WellKnownMember.System_Collections_Generic_EqualityComparer_T__get_Default Or Microsoft.CodeAnalysis.WellKnownMember.System_AttributeUsageAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_AttributeUsageAttribute__AllowMultiple Or Microsoft.CodeAnalysis.WellKnownMember.System_AttributeUsageAttribute__Inherited Or Microsoft.CodeAnalysis.WellKnownMember.System_ParamArrayAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_STAThreadAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_DefaultMemberAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Diagnostics_Debugger__Break Or Microsoft.CodeAnalysis.WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__Type Or Microsoft.CodeAnalysis.WellKnownMember.System_Diagnostics_DebuggerNonUserCodeAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Diagnostics_DebuggerHiddenAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Diagnostics_DebuggerBrowsableAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Diagnostics_DebuggerStepThroughAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Diagnostics_DebuggableAttribute__ctorDebuggingModes Or Microsoft.CodeAnalysis.WellKnownMember.System_Diagnostics_DebuggableAttribute_DebuggingModes__Default Or Microsoft.CodeAnalysis.WellKnownMember.System_Diagnostics_DebuggableAttribute_DebuggingModes__DisableOptimizations Or Microsoft.CodeAnalysis.WellKnownMember.System_Diagnostics_DebuggableAttribute_DebuggingModes__EnableEditAndContinue Or Microsoft.CodeAnalysis.WellKnownMember.System_Diagnostics_DebuggableAttribute_DebuggingModes__IgnoreSymbolStoreSequencePoints Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_UnknownWrapper__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_DispatchWrapper__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_ClassInterfaceAttribute__ctorClassInterfaceType Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_CoClassAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_ComAwareEventInfo__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_ComAwareEventInfo__AddEventHandler Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_ComAwareEventInfo__RemoveEventHandler Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_ComEventInterfaceAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_ComSourceInterfacesAttribute__ctorString Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_ComVisibleAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_DispIdAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_GuidAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_InterfaceTypeAttribute__ctorComInterfaceType Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_InterfaceTypeAttribute__ctorInt16 Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_Marshal__GetTypeFromCLSID Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_TypeIdentifierAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_TypeIdentifierAttribute__ctorStringString Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_BestFitMappingAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_DefaultParameterValueAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_LCIDConversionAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_UnmanagedFunctionPointerAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__AddEventHandler Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__GetOrCreateEventRegistrationTokenTable Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__InvocationList Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__RemoveEventHandler Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__AddEventHandler_T Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__RemoveAllEventHandlers Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__RemoveEventHandler_T Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_DateTimeConstantAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_DecimalConstantAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_DecimalConstantAttribute__ctorByteByteInt32Int32Int32 Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_ExtensionAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AccessedThroughPropertyAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_CompilationRelaxationsAttribute__ctorInt32 Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__WrapNonExceptionThrows Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_UnsafeValueTypeAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_FixedBufferAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_DynamicAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_DynamicAttribute__ctorTransformFlags Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_CallSite_T__Create Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_CallSite_T__Target Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_RuntimeHelpers__GetObjectValueObject Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_RuntimeHelpers__InitializeArrayArrayRuntimeFieldHandle Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_RuntimeHelpers__get_OffsetToStringData Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_RuntimeHelpers__GetSubArray_T Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_ExceptionServices_ExceptionDispatchInfo__Capture Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_ExceptionServices_ExceptionDispatchInfo__Throw Or Microsoft.CodeAnalysis.WellKnownMember.System_Security_UnverifiableCodeAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Security_Permissions_SecurityAction__RequestMinimum Or Microsoft.CodeAnalysis.WellKnownMember.System_Security_Permissions_SecurityPermissionAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Security_Permissions_SecurityPermissionAttribute__SkipVerification Or Microsoft.CodeAnalysis.WellKnownMember.System_Activator__CreateInstance Or Microsoft.CodeAnalysis.WellKnownMember.System_Activator__CreateInstance_T Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Interlocked__CompareExchange Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Interlocked__CompareExchange_T Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Monitor__Enter Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Monitor__Enter2 Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Monitor__Exit Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Thread__CurrentThread Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Thread__ManagedThreadId Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_CSharp_RuntimeBinder_Binder__BinaryOperation Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_CSharp_RuntimeBinder_Binder__Convert Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_CSharp_RuntimeBinder_Binder__GetIndex Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_CSharp_RuntimeBinder_Binder__GetMember Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_CSharp_RuntimeBinder_Binder__Invoke Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_CSharp_RuntimeBinder_Binder__InvokeConstructor Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_CSharp_RuntimeBinder_Binder__InvokeMember Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_CSharp_RuntimeBinder_Binder__IsEvent Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_CSharp_RuntimeBinder_Binder__SetIndex Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_CSharp_RuntimeBinder_Binder__SetMember Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_CSharp_RuntimeBinder_Binder__UnaryOperation Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_CSharp_RuntimeBinder_CSharpArgumentInfo__Create Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToBooleanString Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSByteString Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToByteString Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToShortString Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUShortString Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToIntegerString Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUIntegerString Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToLongString Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToULongString Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSingleString Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDoubleString Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalString Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDateString Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharString Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharArrayRankOneString Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringInt32 Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringByte Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringUInt32 Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringInt64 Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringUInt64 Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringSingle Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringDouble Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringDecimal Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringDateTime Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringChar Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToBooleanObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSByteObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToByteObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToShortObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUShortObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToIntegerObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUIntegerObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToLongObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToULongObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSingleObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDoubleObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDateObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharArrayRankOneObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToGenericParameter_T_Object Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ChangeType Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__PlusObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__NegateObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__NotObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__AndObjectObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__OrObjectObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__XorObjectObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__AddObjectObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__SubtractObjectObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__MultiplyObjectObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__DivideObjectObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ExponentObjectObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ModObjectObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__IntDivideObjectObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__LeftShiftObjectObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__RightShiftObjectObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConcatenateObjectObjectObject Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareObjectEqualObjectObjectBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareObjectNotEqualObjectObjectBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareObjectLessObjectObjectBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareObjectLessEqualObjectObjectBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareObjectGreaterEqualObjectObjectBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareObjectGreaterObjectObjectBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConditionalCompareObjectEqualObjectObjectBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConditionalCompareObjectNotEqualObjectObjectBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConditionalCompareObjectLessObjectObjectBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConditionalCompareObjectLessEqualObjectObjectBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConditionalCompareObjectGreaterEqualObjectObjectBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConditionalCompareObjectGreaterObjectObjectBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareStringStringStringBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_EmbeddedOperators__CompareStringStringStringBoolean Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateCall Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateGet Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateSet Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateSetComplex Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateIndexGet Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateIndexSet Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateIndexSetComplex Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_StandardModuleAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_StaticLocalInitFlag__ctor Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_StaticLocalInitFlag__State Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_StringType__MidStmtStr Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_IncompleteInitialization__ctor Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Embedded__ctor Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Utils__CopyArray Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_LikeOperator__LikeStringStringStringCompareMethod Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_LikeOperator__LikeObjectObjectObjectCompareMethod Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__CreateProjectError Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__SetProjectError Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__SetProjectError_Int32 Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__ClearProjectError Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__EndApp Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_ObjectFlowControl_ForLoopControl__ForLoopInitObj Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_ObjectFlowControl_ForLoopControl__ForNextCheckObj Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_ObjectFlowControl__CheckForSyncLockOnValueType Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Versioned__CallByName Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Versioned__IsNumeric Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Versioned__SystemTypeName Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Versioned__TypeName Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Versioned__VbTypeName Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Information__IsNumeric Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Information__SystemTypeName Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Information__TypeName Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Information__VbTypeName Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Interaction__CallByName Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_MoveNext Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_SetStateMachine Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__Create Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetException Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetResult Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__AwaitOnCompleted Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__AwaitUnsafeOnCompleted Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__Start_T Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetStateMachine Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Create Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetException Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetResult Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__AwaitOnCompleted Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__AwaitUnsafeOnCompleted Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Start_T Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetStateMachine Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Task Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Create Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetException Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetResult Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__AwaitOnCompleted Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__AwaitUnsafeOnCompleted Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Start_T Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetStateMachine Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Task Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncStateMachineAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_IteratorStateMachineAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Strings__AscCharInt32 Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Strings__AscStringInt32 Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Strings__AscWCharInt32 Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Strings__AscWStringInt32 Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Strings__ChrInt32Char Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Strings__ChrWInt32Char Or Microsoft.CodeAnalysis.WellKnownMember.System_Xml_Linq_XElement__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Xml_Linq_XElement__ctor2 Or Microsoft.CodeAnalysis.WellKnownMember.System_Xml_Linq_XNamespace__Get Or Microsoft.CodeAnalysis.WellKnownMember.System_Windows_Forms_Application__RunForm Or Microsoft.CodeAnalysis.WellKnownMember.System_Environment__CurrentManagedThreadId Or Microsoft.CodeAnalysis.WellKnownMember.System_ComponentModel_EditorBrowsableAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_GCLatencyMode__SustainedLowLatency Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T1__Item1 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T2__Item1 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T2__Item2 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T3__Item1 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T3__Item2 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T3__Item3 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T4__Item1 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T4__Item2 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T4__Item3 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T4__Item4 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T5__Item1 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T5__Item2 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T5__Item3 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T5__Item4 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T5__Item5 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T6__Item1 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T6__Item2 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T6__Item3 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T6__Item4 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T6__Item5 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T6__Item6 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T7__Item1 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T7__Item2 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T7__Item3 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T7__Item4 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T7__Item5 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T7__Item6 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T7__Item7 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_TRest__Item1 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_TRest__Item2 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_TRest__Item3 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_TRest__Item4 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_TRest__Item5 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_TRest__Item6 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_TRest__Item7 Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_TRest__Rest Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T1__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T2__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T3__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T4__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T5__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T6__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_T7__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_ValueTuple_TRest__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_TupleElementNamesAttribute__ctorTransformNames Or Microsoft.CodeAnalysis.WellKnownMember.System_String__Format_IFormatProvider Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_CodeAnalysis_Runtime_Instrumentation__CreatePayloadForMethodsSpanningSingleFile Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_CodeAnalysis_Runtime_Instrumentation__CreatePayloadForMethodsSpanningMultipleFiles Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_NullableAttribute__ctorByte Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_NullableAttribute__ctorTransformFlags Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_NullableContextAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_NullablePublicOnlyAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_ReferenceAssemblyAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_IsReadOnlyAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_IsByRefLikeAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_ObsoleteAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Span_T__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Span_T__get_Item Or Microsoft.CodeAnalysis.WellKnownMember.System_Span_T__get_Length Or Microsoft.CodeAnalysis.WellKnownMember.System_ReadOnlySpan_T__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_ReadOnlySpan_T__get_Item Or Microsoft.CodeAnalysis.WellKnownMember.System_ReadOnlySpan_T__get_Length Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_IsUnmanagedAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Conversion__FixSingle Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Conversion__FixDouble Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Conversion__IntSingle Or Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Conversion__IntDouble Or Microsoft.CodeAnalysis.WellKnownMember.System_Math__CeilingDouble Or Microsoft.CodeAnalysis.WellKnownMember.System_Math__FloorDouble Or Microsoft.CodeAnalysis.WellKnownMember.System_Math__TruncateDouble Or Microsoft.CodeAnalysis.WellKnownMember.System_Index__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Index__GetOffset Or Microsoft.CodeAnalysis.WellKnownMember.System_Range__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Range__StartAt Or Microsoft.CodeAnalysis.WellKnownMember.System_Range__EndAt Or Microsoft.CodeAnalysis.WellKnownMember.System_Range__get_All Or Microsoft.CodeAnalysis.WellKnownMember.System_Range__get_Start Or Microsoft.CodeAnalysis.WellKnownMember.System_Range__get_End Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncIteratorStateMachineAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_IAsyncDisposable__DisposeAsync Or Microsoft.CodeAnalysis.WellKnownMember.System_Collections_Generic_IAsyncEnumerable_T__GetAsyncEnumerator Or Microsoft.CodeAnalysis.WellKnownMember.System_Collections_Generic_IAsyncEnumerator_T__MoveNextAsync Or Microsoft.CodeAnalysis.WellKnownMember.System_Collections_Generic_IAsyncEnumerator_T__get_Current Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetResult Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetStatus Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__OnCompleted Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__Reset Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__SetException Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__SetResult Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__get_Version Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource_T__GetResult Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource_T__GetStatus Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource_T__OnCompleted Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource__GetResult Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource__GetStatus Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource__OnCompleted Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_ValueTask_T__ctorSourceAndToken Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_ValueTask_T__ctorValue Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_Tasks_ValueTask__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__Create Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__Complete Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__AwaitOnCompleted Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__AwaitUnsafeOnCompleted Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__MoveNext_T Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_ITuple__get_Item Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_ITuple__get_Length Or Microsoft.CodeAnalysis.WellKnownMember.System_InvalidOperationException__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_SwitchExpressionException__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_SwitchExpressionException__ctorObject Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_CancellationToken__Equals Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_CancellationTokenSource__CreateLinkedTokenSource Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_CancellationTokenSource__Token Or Microsoft.CodeAnalysis.WellKnownMember.System_Threading_CancellationTokenSource__Dispose Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_NativeIntegerAttribute__ctor Or Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_NativeIntegerAttribute__ctorTransformFlags Or Microsoft.CodeAnalysis.WellKnownMember.System_Text_StringBuilder__AppendString Or Microsoft.CodeAnalysis.WellKnownMember.System_Text_StringBuilder__AppendObject Or Microsoft.CodeAnalysis.WellKnownMember.System_Text_StringBuilder__ctor Or Microsoft.CodeAnalysis.WellKnownMember.Count
			Dim specialMember As Microsoft.CodeAnalysis.SpecialMember = Microsoft.CodeAnalysis.SpecialMember.System_String__ConcatStringString Or Microsoft.CodeAnalysis.SpecialMember.System_String__ConcatStringStringString Or Microsoft.CodeAnalysis.SpecialMember.System_String__ConcatStringStringStringString Or Microsoft.CodeAnalysis.SpecialMember.System_String__ConcatStringArray Or Microsoft.CodeAnalysis.SpecialMember.System_String__ConcatObject Or Microsoft.CodeAnalysis.SpecialMember.System_String__ConcatObjectObject Or Microsoft.CodeAnalysis.SpecialMember.System_String__ConcatObjectObjectObject Or Microsoft.CodeAnalysis.SpecialMember.System_String__ConcatObjectArray Or Microsoft.CodeAnalysis.SpecialMember.System_String__op_Equality Or Microsoft.CodeAnalysis.SpecialMember.System_String__op_Inequality Or Microsoft.CodeAnalysis.SpecialMember.System_String__Length Or Microsoft.CodeAnalysis.SpecialMember.System_String__Chars Or Microsoft.CodeAnalysis.SpecialMember.System_String__Format Or Microsoft.CodeAnalysis.SpecialMember.System_String__Substring Or Microsoft.CodeAnalysis.SpecialMember.System_Double__IsNaN Or Microsoft.CodeAnalysis.SpecialMember.System_Single__IsNaN Or Microsoft.CodeAnalysis.SpecialMember.System_Delegate__Combine Or Microsoft.CodeAnalysis.SpecialMember.System_Delegate__Remove Or Microsoft.CodeAnalysis.SpecialMember.System_Delegate__op_Equality Or Microsoft.CodeAnalysis.SpecialMember.System_Delegate__op_Inequality Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__Zero Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__MinusOne Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__One Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__CtorInt32 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__CtorUInt32 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__CtorInt64 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__CtorUInt64 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__CtorSingle Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__CtorDouble Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__CtorInt32Int32Int32BooleanByte Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Addition Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Subtraction Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Multiply Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Division Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Modulus Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_UnaryNegation Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Increment Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Decrement Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__NegateDecimal Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__RemainderDecimalDecimal Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__AddDecimalDecimal Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__SubtractDecimalDecimal Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__MultiplyDecimalDecimal Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__DivideDecimalDecimal Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__ModuloDecimalDecimal Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__CompareDecimalDecimal Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Equality Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Inequality Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_GreaterThan Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_GreaterThanOrEqual Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_LessThan Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_LessThanOrEqual Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromByte Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromChar Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromInt16 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromInt32 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromInt64 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromSByte Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromUInt16 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromUInt32 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromUInt64 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToByte Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToUInt16 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToSByte Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToInt16 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToSingle Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToDouble Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToChar Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToUInt64 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToInt32 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToUInt32 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToInt64 Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_FromDouble Or Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_FromSingle Or Microsoft.CodeAnalysis.SpecialMember.System_DateTime__MinValue Or Microsoft.CodeAnalysis.SpecialMember.System_DateTime__CtorInt64 Or Microsoft.CodeAnalysis.SpecialMember.System_DateTime__CompareDateTimeDateTime Or Microsoft.CodeAnalysis.SpecialMember.System_DateTime__op_Equality Or Microsoft.CodeAnalysis.SpecialMember.System_DateTime__op_Inequality Or Microsoft.CodeAnalysis.SpecialMember.System_DateTime__op_GreaterThan Or Microsoft.CodeAnalysis.SpecialMember.System_DateTime__op_GreaterThanOrEqual Or Microsoft.CodeAnalysis.SpecialMember.System_DateTime__op_LessThan Or Microsoft.CodeAnalysis.SpecialMember.System_DateTime__op_LessThanOrEqual Or Microsoft.CodeAnalysis.SpecialMember.System_Collections_IEnumerable__GetEnumerator Or Microsoft.CodeAnalysis.SpecialMember.System_Collections_IEnumerator__Current Or Microsoft.CodeAnalysis.SpecialMember.System_Collections_IEnumerator__get_Current Or Microsoft.CodeAnalysis.SpecialMember.System_Collections_IEnumerator__MoveNext Or Microsoft.CodeAnalysis.SpecialMember.System_Collections_IEnumerator__Reset Or Microsoft.CodeAnalysis.SpecialMember.System_Collections_Generic_IEnumerable_T__GetEnumerator Or Microsoft.CodeAnalysis.SpecialMember.System_Collections_Generic_IEnumerator_T__Current Or Microsoft.CodeAnalysis.SpecialMember.System_Collections_Generic_IEnumerator_T__get_Current Or Microsoft.CodeAnalysis.SpecialMember.System_IDisposable__Dispose Or Microsoft.CodeAnalysis.SpecialMember.System_Array__Length Or Microsoft.CodeAnalysis.SpecialMember.System_Array__LongLength Or Microsoft.CodeAnalysis.SpecialMember.System_Array__GetLowerBound Or Microsoft.CodeAnalysis.SpecialMember.System_Array__GetUpperBound Or Microsoft.CodeAnalysis.SpecialMember.System_Object__GetHashCode Or Microsoft.CodeAnalysis.SpecialMember.System_Object__Equals Or Microsoft.CodeAnalysis.SpecialMember.System_Object__EqualsObjectObject Or Microsoft.CodeAnalysis.SpecialMember.System_Object__ToString Or Microsoft.CodeAnalysis.SpecialMember.System_Object__ReferenceEquals Or Microsoft.CodeAnalysis.SpecialMember.System_IntPtr__op_Explicit_ToPointer Or Microsoft.CodeAnalysis.SpecialMember.System_IntPtr__op_Explicit_ToInt32 Or Microsoft.CodeAnalysis.SpecialMember.System_IntPtr__op_Explicit_ToInt64 Or Microsoft.CodeAnalysis.SpecialMember.System_IntPtr__op_Explicit_FromPointer Or Microsoft.CodeAnalysis.SpecialMember.System_IntPtr__op_Explicit_FromInt32 Or Microsoft.CodeAnalysis.SpecialMember.System_IntPtr__op_Explicit_FromInt64 Or Microsoft.CodeAnalysis.SpecialMember.System_UIntPtr__op_Explicit_ToPointer Or Microsoft.CodeAnalysis.SpecialMember.System_UIntPtr__op_Explicit_ToUInt32 Or Microsoft.CodeAnalysis.SpecialMember.System_UIntPtr__op_Explicit_ToUInt64 Or Microsoft.CodeAnalysis.SpecialMember.System_UIntPtr__op_Explicit_FromPointer Or Microsoft.CodeAnalysis.SpecialMember.System_UIntPtr__op_Explicit_FromUInt32 Or Microsoft.CodeAnalysis.SpecialMember.System_UIntPtr__op_Explicit_FromUInt64 Or Microsoft.CodeAnalysis.SpecialMember.System_Nullable_T_GetValueOrDefault Or Microsoft.CodeAnalysis.SpecialMember.System_Nullable_T_get_Value Or Microsoft.CodeAnalysis.SpecialMember.System_Nullable_T_get_HasValue Or Microsoft.CodeAnalysis.SpecialMember.System_Nullable_T__ctor Or Microsoft.CodeAnalysis.SpecialMember.System_Nullable_T__op_Implicit_FromT Or Microsoft.CodeAnalysis.SpecialMember.System_Nullable_T__op_Explicit_ToT Or Microsoft.CodeAnalysis.SpecialMember.System_Runtime_CompilerServices_RuntimeFeature__DefaultImplementationsOfInterfaces Or Microsoft.CodeAnalysis.SpecialMember.System_Runtime_CompilerServices_RuntimeFeature__UnmanagedSignatureCallingConvention Or Microsoft.CodeAnalysis.SpecialMember.System_Runtime_CompilerServices_RuntimeFeature__CovariantReturnsOfClasses Or Microsoft.CodeAnalysis.SpecialMember.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute__ctor Or Microsoft.CodeAnalysis.SpecialMember.Count
			Select Case stTo
				Case Microsoft.CodeAnalysis.SpecialType.System_Boolean
					Select Case stFrom
						Case Microsoft.CodeAnalysis.SpecialType.System_Object
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToBooleanObject
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Enum
						Case Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate
						Case Microsoft.CodeAnalysis.SpecialType.System_Delegate
						Case Microsoft.CodeAnalysis.SpecialType.System_ValueType
						Case Microsoft.CodeAnalysis.SpecialType.System_Void
						Case Microsoft.CodeAnalysis.SpecialType.System_Boolean
						Case Microsoft.CodeAnalysis.SpecialType.System_Char
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_SByte
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanInt32
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Byte
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanInt32
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Int16
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanInt32
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt16
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanInt32
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Int32
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanInt32
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt32
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanUInt32
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Int64
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanInt64
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt64
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanUInt64
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanDecimal
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Single
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanSingle
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Double
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanDouble
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_String
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToBooleanString
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Else
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
					End Select

				Case Microsoft.CodeAnalysis.SpecialType.System_Char
					Dim specialType As Microsoft.CodeAnalysis.SpecialType = stFrom
					If (specialType = Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharObject
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					Else
						If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_String) Then
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						End If
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharString
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					End If
				Case Microsoft.CodeAnalysis.SpecialType.System_SByte
					Dim specialType1 As Microsoft.CodeAnalysis.SpecialType = stFrom
					If (specialType1 = Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSByteObject
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					Else
						Select Case specialType1
							Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
								specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToSByte
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Single
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToSByteSingle
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Double
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToSByteDouble
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_String
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSByteString
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Else
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
						End Select
					End If

				Case Microsoft.CodeAnalysis.SpecialType.System_Byte
					Dim specialType2 As Microsoft.CodeAnalysis.SpecialType = stFrom
					If (specialType2 = Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToByteObject
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					Else
						Select Case specialType2
							Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
								specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToByte
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Single
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToByteSingle
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Double
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToByteDouble
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_String
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToByteString
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Else
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
						End Select
					End If

				Case Microsoft.CodeAnalysis.SpecialType.System_Int16
					Dim specialType3 As Microsoft.CodeAnalysis.SpecialType = stFrom
					If (specialType3 = Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToShortObject
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					Else
						Select Case specialType3
							Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
								specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToInt16
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Single
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt16Single
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Double
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt16Double
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_String
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToShortString
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Else
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
						End Select
					End If

				Case Microsoft.CodeAnalysis.SpecialType.System_UInt16
					Dim specialType4 As Microsoft.CodeAnalysis.SpecialType = stFrom
					If (specialType4 = Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUShortObject
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					Else
						Select Case specialType4
							Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
								specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToUInt16
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Single
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt16Single
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Double
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt16Double
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_String
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUShortString
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Else
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
						End Select
					End If

				Case Microsoft.CodeAnalysis.SpecialType.System_Int32
					Dim specialType5 As Microsoft.CodeAnalysis.SpecialType = stFrom
					If (specialType5 = Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToIntegerObject
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					Else
						Select Case specialType5
							Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
								specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToInt32
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Single
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt32Single
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Double
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt32Double
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_String
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToIntegerString
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Else
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
						End Select
					End If

				Case Microsoft.CodeAnalysis.SpecialType.System_UInt32
					Dim specialType6 As Microsoft.CodeAnalysis.SpecialType = stFrom
					If (specialType6 = Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUIntegerObject
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					Else
						Select Case specialType6
							Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
								specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToUInt32
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Single
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt32Single
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Double
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt32Double
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_String
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUIntegerString
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Else
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
						End Select
					End If

				Case Microsoft.CodeAnalysis.SpecialType.System_Int64
					Dim specialType7 As Microsoft.CodeAnalysis.SpecialType = stFrom
					If (specialType7 = Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToLongObject
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					Else
						Select Case specialType7
							Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
								specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToInt64
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Single
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt64Single
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Double
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt64Double
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_String
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToLongString
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Else
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
						End Select
					End If

				Case Microsoft.CodeAnalysis.SpecialType.System_UInt64
					Dim specialType8 As Microsoft.CodeAnalysis.SpecialType = stFrom
					If (specialType8 = Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToULongObject
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					Else
						Select Case specialType8
							Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
								specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToUInt64
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Single
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt64Single
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_Double
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt64Double
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Microsoft.CodeAnalysis.SpecialType.System_String
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToULongString
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Case Else
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
						End Select
					End If

				Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
					Select Case stFrom
						Case Microsoft.CodeAnalysis.SpecialType.System_Object
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalObject
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Enum
						Case Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate
						Case Microsoft.CodeAnalysis.SpecialType.System_Delegate
						Case Microsoft.CodeAnalysis.SpecialType.System_ValueType
						Case Microsoft.CodeAnalysis.SpecialType.System_Void
						Case Microsoft.CodeAnalysis.SpecialType.System_Char
						Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Boolean
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalBoolean
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_SByte
							specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromInt32
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Byte
							specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromInt32
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Int16
							specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromInt32
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt16
							specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromInt32
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Int32
							specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromInt32
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt32
							specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromUInt32
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Int64
							specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromInt64
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt64
							specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Implicit_FromUInt64
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Single
							specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_FromSingle
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Double
							specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_FromDouble
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_String
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalString
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Else
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
					End Select

				Case Microsoft.CodeAnalysis.SpecialType.System_Single
					Dim specialType9 As Microsoft.CodeAnalysis.SpecialType = stFrom
					If (specialType9 = Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSingleObject
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					ElseIf (specialType9 = Microsoft.CodeAnalysis.SpecialType.System_Decimal) Then
						specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToSingle
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					ElseIf (specialType9 = Microsoft.CodeAnalysis.SpecialType.System_String) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSingleString
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					Else
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					End If
				Case Microsoft.CodeAnalysis.SpecialType.System_Double
					Dim specialType10 As Microsoft.CodeAnalysis.SpecialType = stFrom
					If (specialType10 = Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDoubleObject
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					ElseIf (specialType10 = Microsoft.CodeAnalysis.SpecialType.System_Decimal) Then
						specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Explicit_ToDouble
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					ElseIf (specialType10 = Microsoft.CodeAnalysis.SpecialType.System_String) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDoubleString
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					Else
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					End If
				Case Microsoft.CodeAnalysis.SpecialType.System_String
					Dim specialType11 As Microsoft.CodeAnalysis.SpecialType = stFrom
					Select Case specialType11
						Case Microsoft.CodeAnalysis.SpecialType.System_Object
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringObject
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Enum
						Case Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate
						Case Microsoft.CodeAnalysis.SpecialType.System_Delegate
						Case Microsoft.CodeAnalysis.SpecialType.System_ValueType
						Case Microsoft.CodeAnalysis.SpecialType.System_Void
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Boolean
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringBoolean
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Char
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringChar
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_SByte
						Case Microsoft.CodeAnalysis.SpecialType.System_Int16
						Case Microsoft.CodeAnalysis.SpecialType.System_Int32
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringInt32
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Byte
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringByte
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt16
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt32
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringUInt32
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Int64
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringInt64
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_UInt64
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringUInt64
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringDecimal
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Single
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringSingle
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Microsoft.CodeAnalysis.SpecialType.System_Double
							wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringDouble
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						Case Else
							If (specialType11 = Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
								wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringDateTime
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							Else
								If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
									methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
								ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
									methodSymbol = Nothing
								Else
									methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								End If
								Return methodSymbol
							End If
					End Select

				Case Microsoft.CodeAnalysis.SpecialType.System_IntPtr
				Case Microsoft.CodeAnalysis.SpecialType.System_UIntPtr
				Case Microsoft.CodeAnalysis.SpecialType.System_Array
				Case Microsoft.CodeAnalysis.SpecialType.System_Collections_IEnumerable
				Case Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IEnumerable_T
				Case Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IList_T
				Case Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_ICollection_T
				Case Microsoft.CodeAnalysis.SpecialType.System_Collections_IEnumerator
				Case Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IEnumerator_T
				Case Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IReadOnlyList_T
				Case Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IReadOnlyCollection_T
				Case Microsoft.CodeAnalysis.SpecialType.System_Nullable_T
					If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
						methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
					ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
						methodSymbol = Nothing
					Else
						methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
					End If
					Return methodSymbol
				Case Microsoft.CodeAnalysis.SpecialType.System_DateTime
					Dim specialType12 As Microsoft.CodeAnalysis.SpecialType = stFrom
					If (specialType12 = Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDateObject
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					Else
						If (specialType12 <> Microsoft.CodeAnalysis.SpecialType.System_String) Then
							If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
								methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
							ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
								methodSymbol = Nothing
							Else
								methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							End If
							Return methodSymbol
						End If
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDateString
						If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
							methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
						ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
							methodSymbol = Nothing
						Else
							methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						End If
						Return methodSymbol
					End If
				Case Else
					If (wellKnownMember >= Microsoft.CodeAnalysis.WellKnownMember.System_Math__RoundDouble) Then
						methodSymbol = Me._factory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(wellKnownMember, False)
					ElseIf (specialMember < Microsoft.CodeAnalysis.SpecialMember.System_String__CtorSZArrayChar) Then
						methodSymbol = Nothing
					Else
						methodSymbol = DirectCast(Me._factory.SpecialMember(specialMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
					End If
					Return methodSymbol
			End Select
		End Function

		Private Function GetExprFactoryMethodGroup(ByVal methodName As String, ByVal typeArgs As ImmutableArray(Of TypeSymbol)) As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup
			Dim boundMethodGroup As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup = Nothing
			Dim instance As LookupResult = LookupResult.GetInstance()
			Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me._binder.GetNewCompoundUseSiteInfo(Me.Diagnostics)
			Me._binder.LookupMember(instance, Me._expressionType, methodName, 0, LookupOptions.AllMethodsOfAnyArity Or LookupOptions.IgnoreExtensionMethods, newCompoundUseSiteInfo)
			Me.Diagnostics.Add(Me._factory.Syntax, newCompoundUseSiteInfo)
			If (instance.IsGood) Then
				Dim item As Symbol = instance.Symbols(0)
				If (instance.Symbols(0).Kind = SymbolKind.Method) Then
					boundMethodGroup = New Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup(Me._factory.Syntax, Me._factory.TypeArguments(typeArgs), instance.Symbols.ToDowncastedImmutable(Of MethodSymbol)(), instance.Kind, Nothing, QualificationKind.QualifiedViaTypeName, False)
				End If
			End If
			If (boundMethodGroup Is Nothing) Then
				Me.Diagnostics.Add(If(instance.HasDiagnostic, instance.Diagnostic, ErrorFactory.ErrorInfo(ERRID.ERR_NameNotMember2, New [Object]() { methodName, Me._expressionType })), Me._factory.Syntax.GetLocation())
			End If
			instance.Free()
			Return boundMethodGroup
		End Function

		Private Function GetHelperForDateTimeBinaryOperation(ByVal opKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind) As MethodSymbol
			Dim specialMember As Microsoft.CodeAnalysis.SpecialMember
			opKind = opKind And Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.OpMask
			Dim binaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind = opKind
			Select Case binaryOperatorKind
				Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Equals
				Label0:
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_DateTime__op_Equality
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.NotEquals
				Label1:
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_DateTime__op_Inequality
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThanOrEqual
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_DateTime__op_LessThanOrEqual
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThanOrEqual
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_DateTime__op_GreaterThanOrEqual
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThan
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_DateTime__op_LessThan
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThan
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_DateTime__op_GreaterThan
					Exit Select
				Case Else
					If (binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Is]) Then
						GoTo Label0
					End If
					If (binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[IsNot]) Then
						GoTo Label1
					End If
					Throw ExceptionUtilities.UnexpectedValue(opKind)
			End Select
			Return DirectCast(Me._factory.SpecialMember(specialMember), MethodSymbol)
		End Function

		Private Function GetHelperForDecimalBinaryOperation(ByVal opKind As BinaryOperatorKind) As MethodSymbol
			Dim specialMember As Microsoft.CodeAnalysis.SpecialMember
			opKind = opKind And BinaryOperatorKind.OpMask
			Select Case opKind
				Case BinaryOperatorKind.Add
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__AddDecimalDecimal
					Exit Select
				Case BinaryOperatorKind.Concatenate
				Case BinaryOperatorKind.[Like]
				Case BinaryOperatorKind.Power
				Case BinaryOperatorKind.IntegerDivide
				Case BinaryOperatorKind.LeftShift
				Case BinaryOperatorKind.RightShift
				Case BinaryOperatorKind.[Xor]
				Case BinaryOperatorKind.[Or]
				Case BinaryOperatorKind.[OrElse]
				Case BinaryOperatorKind.[And]
				Case BinaryOperatorKind.[AndAlso]
					Throw ExceptionUtilities.UnexpectedValue(opKind)
				Case BinaryOperatorKind.Equals
				Case BinaryOperatorKind.[Is]
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Equality
					Exit Select
				Case BinaryOperatorKind.NotEquals
				Case BinaryOperatorKind.[IsNot]
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_Inequality
					Exit Select
				Case BinaryOperatorKind.LessThanOrEqual
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_LessThanOrEqual
					Exit Select
				Case BinaryOperatorKind.GreaterThanOrEqual
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_GreaterThanOrEqual
					Exit Select
				Case BinaryOperatorKind.LessThan
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_LessThan
					Exit Select
				Case BinaryOperatorKind.GreaterThan
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__op_GreaterThan
					Exit Select
				Case BinaryOperatorKind.Subtract
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__SubtractDecimalDecimal
					Exit Select
				Case BinaryOperatorKind.Multiply
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__MultiplyDecimalDecimal
					Exit Select
				Case BinaryOperatorKind.Divide
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__DivideDecimalDecimal
					Exit Select
				Case BinaryOperatorKind.Modulo
					specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__ModuloDecimalDecimal
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(opKind)
			End Select
			Return DirectCast(Me._factory.SpecialMember(specialMember), MethodSymbol)
		End Function

		Private Function GetHelperForDecimalUnaryOperation(ByVal opKind As UnaryOperatorKind) As MethodSymbol
			opKind = opKind And UnaryOperatorKind.OpMask
			If (CInt(opKind) - CInt(UnaryOperatorKind.Minus) > CInt(UnaryOperatorKind.Plus)) Then
				Throw ExceptionUtilities.UnexpectedValue(opKind)
			End If
			Return DirectCast(Me._factory.SpecialMember(SpecialMember.System_Decimal__NegateDecimal), MethodSymbol)
		End Function

		Private Function GetHelperForObjectBinaryOperation(ByVal opKind As BinaryOperatorKind) As MethodSymbol
			Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember
			opKind = opKind And BinaryOperatorKind.OpMask
			Select Case opKind
				Case BinaryOperatorKind.Add
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__AddObjectObjectObject
					Exit Select
				Case BinaryOperatorKind.Concatenate
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConcatenateObjectObjectObject
					Exit Select
				Case BinaryOperatorKind.[Like]
				Case BinaryOperatorKind.Equals
				Case BinaryOperatorKind.NotEquals
				Case BinaryOperatorKind.LessThanOrEqual
				Case BinaryOperatorKind.GreaterThanOrEqual
				Case BinaryOperatorKind.LessThan
				Case BinaryOperatorKind.GreaterThan
				Case BinaryOperatorKind.[OrElse]
					Throw ExceptionUtilities.UnexpectedValue(opKind)
				Case BinaryOperatorKind.Subtract
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__SubtractObjectObjectObject
					Exit Select
				Case BinaryOperatorKind.Multiply
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__MultiplyObjectObjectObject
					Exit Select
				Case BinaryOperatorKind.Power
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ExponentObjectObjectObject
					Exit Select
				Case BinaryOperatorKind.Divide
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__DivideObjectObjectObject
					Exit Select
				Case BinaryOperatorKind.Modulo
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ModObjectObjectObject
					Exit Select
				Case BinaryOperatorKind.IntegerDivide
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__IntDivideObjectObjectObject
					Exit Select
				Case BinaryOperatorKind.LeftShift
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__LeftShiftObjectObjectObject
					Exit Select
				Case BinaryOperatorKind.RightShift
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__RightShiftObjectObjectObject
					Exit Select
				Case BinaryOperatorKind.[Xor]
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__XorObjectObjectObject
					Exit Select
				Case BinaryOperatorKind.[Or]
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__OrObjectObjectObject
					Exit Select
				Case BinaryOperatorKind.[And]
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__AndObjectObjectObject
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(opKind)
			End Select
			Return Me._factory.WellKnownMember(Of MethodSymbol)(wellKnownMember, False)
		End Function

		Private Function GetHelperForObjectUnaryOperation(ByVal opKind As UnaryOperatorKind) As MethodSymbol
			Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember
			opKind = opKind And UnaryOperatorKind.OpMask
			Select Case opKind
				Case UnaryOperatorKind.Plus
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__PlusObjectObject
					Exit Select
				Case UnaryOperatorKind.Minus
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__NegateObjectObject
					Exit Select
				Case UnaryOperatorKind.[Not]
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__NotObjectObject
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(opKind)
			End Select
			Return Me._factory.WellKnownMember(Of MethodSymbol)(wellKnownMember, False)
		End Function

		Private Function GetSignedVersionOfNumericType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Select Case type.SpecialType
				Case SpecialType.System_Byte
					typeSymbol = Me._factory.SpecialType(SpecialType.System_SByte)
					Exit Select
				Case SpecialType.System_Int16
				Case SpecialType.System_Int32
				Case SpecialType.System_Int64
				Label0:
					typeSymbol = type
					Exit Select
				Case SpecialType.System_UInt16
					typeSymbol = Me._factory.SpecialType(SpecialType.System_Int16)
					Exit Select
				Case SpecialType.System_UInt32
					typeSymbol = Me._factory.SpecialType(SpecialType.System_Int32)
					Exit Select
				Case SpecialType.System_UInt64
					typeSymbol = Me._factory.SpecialType(SpecialType.System_Int64)
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return typeSymbol
		End Function

		Private Shared Function GetUnaryOperatorMethodName(ByVal opKind As UnaryOperatorKind, ByVal isChecked As Boolean) As String
			' 
			' Current member / type: System.String Microsoft.CodeAnalysis.VisualBasic.ExpressionLambdaRewriter::GetUnaryOperatorMethodName(Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind,System.Boolean)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.String GetUnaryOperatorMethodName(Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind,System.Boolean)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 84
			'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 73
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 27
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 198
			'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 79
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
			'    à ..Visit[,]( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 286
			'    à ..Visit( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 317
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 337
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 44
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Function GetUnderlyingType(ByVal type As TypeSymbol) As TypeSymbol
			Return type.GetNullableUnderlyingTypeOrSelf().GetEnumUnderlyingTypeOrSelf()
		End Function

		Private Function InitWithParameterlessValueTypeConstructor(ByVal type As TypeSymbol) As BoundExpression
			Return Me.ConvertRuntimeHelperToExpressionTree("New", New BoundExpression() { Me._factory.[Typeof](type) })
		End Function

		Private Function IsIntegralType(ByVal type As TypeSymbol) As Boolean
			Return Me.GetUnderlyingType(type).IsIntegralType()
		End Function

		Private Function MaskShiftCountOperand(ByVal node As BoundBinaryOperator, ByVal resultType As TypeSymbol, ByVal isChecked As Boolean) As BoundExpression
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundExpression Microsoft.CodeAnalysis.VisualBasic.ExpressionLambdaRewriter::MaskShiftCountOperand(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Boolean)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundExpression MaskShiftCountOperand(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Boolean)
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

		Private Function MaskShiftCountOperand(ByVal loweredOperand As BoundExpression, ByVal shiftedType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal shiftMask As Integer, ByVal shiftConst As ConstantValue, ByVal isChecked As Boolean) As BoundExpression
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			If (shiftConst Is Nothing OrElse CULng(shiftConst.UInt32Value) > CLng(shiftMask)) Then
				Dim expressionTree As BoundExpression = Me.ConvertRuntimeHelperToExpressionTree("Constant", New BoundExpression() { Me._factory.Convert(Me.ObjectType, Me._factory.Literal(shiftMask), False), Me._factory.[Typeof](Me.Int32Type) })
				Dim flag As Boolean = shiftedType.IsNullableType()
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = shiftedType.GetNullableUnderlyingTypeOrSelf().SpecialType
				If (flag) Then
					typeSymbol = Me._factory.NullableOf(Me.Int32Type)
				Else
					typeSymbol = Nothing
				End If
				Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeSymbol
				If (flag) Then
					expressionTree = Me.Convert(expressionTree, typeSymbol1, isChecked)
				End If
				loweredOperand = Me.ConvertRuntimeHelperToExpressionTree("And", New BoundExpression() { loweredOperand, expressionTree })
			End If
			Return loweredOperand
		End Function

		Private Function Negate(ByVal expr As BoundExpression) As BoundExpression
			Return Me.ConvertRuntimeHelperToExpressionTree("Negate", New BoundExpression() { expr })
		End Function

		Private Function [New](ByVal helper As SpecialMember, ByVal argument As BoundExpression) As BoundExpression
			Return Me.ConvertRuntimeHelperToExpressionTree("New", New BoundExpression() { Me._factory.ConstructorInfo(helper), argument })
		End Function

		Private Function ReplaceArgWithParameterInUserDefinedConversion(ByVal conversion As BoundConversion, ByVal toType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal parameter As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal isChecked As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim operand As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedConversion = DirectCast(conversion.Operand, Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedConversion)
			Dim [call] As BoundCall = operand.[Call]
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = [call].Type
			Dim method As MethodSymbol = [call].Method
			Dim outConversionOpt As BoundConversion = operand.OutConversionOpt
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = method.Parameters(0).Type
			Dim type1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = parameter.Type
			Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me._binder.GetNewCompoundUseSiteInfo(Me.Diagnostics)
			Dim keyValuePair As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.ConversionKind, MethodSymbol) = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyConversion(type1, typeSymbol, newCompoundUseSiteInfo)
			Dim key As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = keyValuePair.Key
			Me.Diagnostics.Add(conversion, newCompoundUseSiteInfo)
			Dim flag As Boolean = Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(key)
			If (flag) Then
				parameter = Me._factory.Convert(typeSymbol, parameter, isChecked)
			End If
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(parameter)
			Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
			[call] = [call].Update(method, Nothing, Nothing, boundExpressions, bitVector, Nothing, False, True, type)
			If (outConversionOpt IsNot Nothing) Then
				outConversionOpt = outConversionOpt.Update([call], outConversionOpt.ConversionKind, outConversionOpt.Checked, outConversionOpt.ExplicitCastInCode, outConversionOpt.ConstantValueOpt, outConversionOpt.ExtendedInfoOpt, outConversionOpt.Type)
			End If
			Dim num As Byte = CByte((If(outConversionOpt IsNot Nothing, 2, 0) + If(flag, 1, 0)))
			Dim boundUserDefinedConversion As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedConversion = operand
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = outConversionOpt
			If (boundExpression Is Nothing) Then
				boundExpression = [call]
			End If
			operand = boundUserDefinedConversion.Update(boundExpression, num, type1)
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = conversion.ConversionKind And (Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToIntegerOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflowMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToQueryLambdaBodyMismatch Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToArrayLiteralElementConversion Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Widening] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Identity Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Reference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningReference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingReference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Array Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningArray Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingArray Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Value Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningValue Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingValue Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[String] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Boolean] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingBoolean Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNothingLiteral Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesNarrowingFromNumericConstant Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Lambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWidening Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningDropReturnOrArgs Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningToNonLambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNarrowing Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelInvalid Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.VarianceConversionAmbiguity Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.MightSucceedAtRuntime Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.AnonymousDelegate Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NeedAStub Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.ConvertedToExpressionTree Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.UserDefined Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingDueToContraVarianceInDelegate Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InterpolatedString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Tuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTuple)
			If (type1.IsNullableType() AndAlso Not method.Parameters(0).Type.IsNullableType() AndAlso toType.IsNullableType() AndAlso Not method.ReturnType.IsNullableType()) Then
				conversionKind = conversionKind Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Nullable
			End If
			Return conversion.Update(operand, conversionKind, conversion.Checked, conversion.ExplicitCastInCode, conversion.ConstantValueOpt, conversion.ExtendedInfoOpt, toType)
		End Function

		Friend Shared Function RewriteLambda(ByVal node As BoundLambda, ByVal currentMethod As MethodSymbol, ByVal delegateType As NamedTypeSymbol, ByVal compilationState As TypeCompilationState, ByVal typeMap As TypeSubstitution, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal rewrittenNodes As HashSet(Of BoundNode), ByVal recursionDepth As Integer) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = (New ExpressionLambdaRewriter(currentMethod, compilationState, typeMap, node.LambdaSymbol.ContainingBinder, node.Syntax, recursionDepth, diagnostics)).VisitLambdaInternal(node, delegateType)
			If (Not boundExpression.HasErrors) Then
				boundExpression = LocalRewriter.RewriteExpressionTree(boundExpression, currentMethod, compilationState, Nothing, diagnostics, rewrittenNodes, recursionDepth)
			End If
			Return boundExpression
		End Function

		Private Function RewriteUserDefinedOperator(ByVal node As BoundUserDefinedUnaryOperator) As BoundExpression
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundExpression Microsoft.CodeAnalysis.VisualBasic.ExpressionLambdaRewriter::RewriteUserDefinedOperator(Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedUnaryOperator)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundExpression RewriteUserDefinedOperator(Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedUnaryOperator)
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

		Private Function TranslateLambdaBody(ByVal block As Microsoft.CodeAnalysis.VisualBasic.BoundBlock) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim item As BoundStatement = block.Statements(0)
			While True
				Dim kind As BoundKind = item.Kind
				If (kind = BoundKind.ReturnStatement) Then
					Dim expressionOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(item, BoundReturnStatement).ExpressionOpt
					If (expressionOpt Is Nothing) Then
						Exit While
					End If
					boundExpression = Me.Visit(expressionOpt)
					Return boundExpression
				ElseIf (kind = BoundKind.Block) Then
					Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
					If (Not boundBlock.Locals.IsEmpty OrElse boundBlock.Statements.Length <> 1) Then
						Exit While
					End If
					item = boundBlock.Statements(0)
				ElseIf (kind = BoundKind.ExpressionStatement) Then
					boundExpression = Me.Visit(DirectCast(item, BoundExpressionStatement).Expression)
					Return boundExpression
				Else
					Exit While
				End If
			End While
			boundExpression = Me.GenerateDiagnosticAndReturnDummyExpression(ERRID.ERR_StatementLambdaInExpressionTree, block, New [Object](-1) {})
			Return boundExpression
		End Function

		Private Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (node IsNot Nothing) Then
				Dim syntax As SyntaxNode = Me._factory.Syntax
				Me._factory.Syntax = node.Syntax
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitInternal(node)
				Me._factory.Syntax = syntax
				boundExpression = Me._factory.Convert(Me._expressionType, boundExpression1, False)
			Else
				boundExpression = Nothing
			End If
			Return boundExpression
		End Function

		Private Function VisitArrayAccess(ByVal node As BoundArrayAccess) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim expressionTree As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.Visit(node.Expression)
			If (node.Indices.Length <> 1) Then
				expressionTree = Me.ConvertRuntimeHelperToExpressionTree("ArrayIndex", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, Me.BuildIndices(node.Indices) })
			Else
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.Visit(node.Indices(0))
				expressionTree = Me.ConvertRuntimeHelperToExpressionTree("ArrayIndex", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, boundExpression1 })
			End If
			Return expressionTree
		End Function

		Private Function VisitArrayCreation(ByVal node As BoundArrayCreation) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim type As ArrayTypeSymbol = DirectCast(node.Type, ArrayTypeSymbol)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me._factory.[Typeof](type.ElementType)
			Dim initializerOpt As BoundArrayInitialization = node.InitializerOpt
			boundExpression = If(initializerOpt Is Nothing OrElse initializerOpt.Initializers.IsEmpty, Me.ConvertRuntimeHelperToExpressionTree("NewArrayBounds", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression1, Me.ConvertArgumentsIntoArray(node.Bounds) }), Me.ConvertRuntimeHelperToExpressionTree("NewArrayInit", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression1, Me.ConvertArgumentsIntoArray(node.InitializerOpt.Initializers) }))
			Return boundExpression
		End Function

		Private Function VisitArrayLength(ByVal node As BoundArrayLength) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim type As TypeSymbol = node.Type
			boundExpression = If(type.SpecialType <> SpecialType.System_Int64, Me.ConvertRuntimeHelperToExpressionTree("ArrayLength", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { Me.Visit(node.Expression) }), Me.VisitCall(New BoundCall(node.Syntax, DirectCast(Me._factory.SpecialMember(SpecialMember.System_Array__LongLength), PropertySymbol).GetMethod, Nothing, node.Expression, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, Nothing, False, True, type, False)))
			Return boundExpression
		End Function

		Private Function VisitBadExpression(ByVal node As BoundBadExpression) As BoundExpression
			Return node
		End Function

		Private Function VisitBinaryConditionalExpression(ByVal node As BoundBinaryConditionalExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim expressionTree As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim testExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.TestExpression
			Dim convertedTestExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ConvertedTestExpression
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = node.Type
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = testExpression.Type
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.Visit(testExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.Visit(node.ElseExpression)
			If (convertedTestExpression Is Nothing OrElse type.IsSameTypeIgnoringAll(typeSymbol) OrElse typeSymbol.IsNullableType() AndAlso type.IsSameTypeIgnoringAll(typeSymbol.GetNullableUnderlyingType())) Then
				expressionTree = Me.ConvertRuntimeHelperToExpressionTree("Coalesce", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, boundExpression1 })
			Else
				If (convertedTestExpression.Kind <> BoundKind.Conversion) Then
					Throw ExceptionUtilities.UnexpectedValue(convertedTestExpression.Kind)
				End If
				Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = DirectCast(convertedTestExpression, Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
				Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = Me.CreateCoalesceLambdaParameterSymbol(typeSymbol)
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.BuildLambdaBodyForCoalesce(boundConversion, type, parameterSymbol, boundConversion.Checked)
				Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.BuildLambdaForCoalesceCall(type, parameterSymbol, boundExpression2)
				expressionTree = Me.ConvertRuntimeHelperToExpressionTree("Coalesce", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, boundExpression1, boundExpression3 })
			End If
			Return expressionTree
		End Function

		Private Function VisitBinaryOperator(ByVal node As BoundBinaryOperator) As BoundExpression
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundExpression Microsoft.CodeAnalysis.VisualBasic.ExpressionLambdaRewriter::VisitBinaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundExpression VisitBinaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 84
			'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 73
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 27
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 198
			'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 79
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
			'    à ..Visit[,]( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 286
			'    à ..Visit( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 317
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 337
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 44
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Function VisitCall(ByVal node As BoundCall) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim method As MethodSymbol = node.Method
			Dim receiverOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ReceiverOpt
			If (receiverOpt IsNot Nothing) Then
				receiverOpt = If(receiverOpt.Kind <> BoundKind.MyBaseReference, Me.Visit(receiverOpt), Me.CreateLiteralExpression(receiverOpt, method.ContainingType))
			End If
			boundExpression = If(method.MethodKind <> MethodKind.DelegateInvoke, Me.ConvertRuntimeHelperToExpressionTree("Call", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { If(method.IsShared, Me._factory.Null(Me._expressionType), receiverOpt), Me._factory.MethodInfo(method), Me.ConvertArgumentsIntoArray(node.Arguments) }), Me.ConvertRuntimeHelperToExpressionTree("Invoke", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { receiverOpt, Me.ConvertArgumentsIntoArray(node.Arguments) }))
			Return boundExpression
		End Function

		Private Function VisitCollectionInitializer(ByVal initializer As BoundCollectionInitializerExpression) As BoundExpression
			Dim initializers As ImmutableArray(Of BoundExpression) = initializer.Initializers
			Dim length As Integer = initializers.Length
			Dim boundExpressionArray(length - 1 + 1 - 1) As BoundExpression
			Dim num As Integer = length - 1
			Dim num1 As Integer = 0
			Do
				Dim item As BoundCall = DirectCast(initializers(num1), BoundCall)
				Dim boundExpressionArray1 As BoundExpression() = boundExpressionArray
				Dim num2 As Integer = num1
				Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = Me._factory
				Dim elementInitType As NamedTypeSymbol = Me.ElementInitType
				Dim boundExpressionArray2() As BoundExpression = { Me._factory.MethodInfo(item.Method), Nothing }
				boundExpressionArray2(1) = Me.ConvertArgumentsIntoArray(If(Not item.Method.IsShared OrElse Not item.Method.IsExtensionMethod, item.Arguments, item.Arguments.RemoveAt(0)))
				boundExpressionArray1(num2) = syntheticBoundNodeFactory.Convert(elementInitType, Me.ConvertRuntimeHelperToExpressionTree("ElementInit", boundExpressionArray2), False)
				num1 = num1 + 1
			Loop While num1 <= num
			Return Me._factory.Array(Me.ElementInitType, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundExpressionArray))
		End Function

		Private Function VisitConversion(ByVal node As BoundConversion) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			boundExpression = If(Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(node.ConversionKind) OrElse node.Type.IsFloatingType(), Me.ConvertExpression(node.Operand, node.ConversionKind, node.Operand.Type, node.Type, node.Checked, node.ExplicitCastInCode, ExpressionLambdaRewriter.ConversionSemantics.[Default]), Me.VisitInternal(node.Operand))
			Return boundExpression
		End Function

		Private Function VisitDelegateCreationExpression(ByVal node As BoundDelegateCreationExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim type As NamedTypeSymbol = DirectCast(node.Type, NamedTypeSymbol)
			Dim method As MethodSymbol = node.Method
			Dim receiverOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ReceiverOpt
			If (Not node.Method.IsShared) Then
				If (receiverOpt.Kind = BoundKind.MyBaseReference) Then
					receiverOpt = New BoundMeReference(receiverOpt.Syntax, method.ContainingType)
				ElseIf (receiverOpt.IsLValue) Then
					receiverOpt = receiverOpt.MakeRValue()
				End If
				If (Not receiverOpt.Type.IsObjectType()) Then
					receiverOpt = Me._factory.Convert(Me.ObjectType, receiverOpt, False)
				End If
			Else
				receiverOpt = Me._factory.Convert(Me.ObjectType, Me._factory.Null(), False)
			End If
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me._factory.MethodInfo(If(method.CallsiteReducedFromMethod, method))
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)()
			Dim wellKnownTypeMember As Symbol = Binder.GetWellKnownTypeMember(Me._factory.Compilation, WellKnownMember.System_Reflection_MethodInfo__CreateDelegate, useSiteInfo)
			If (Not (CObj(wellKnownTypeMember) <> CObj(Nothing) And useSiteInfo.DiagnosticInfo Is Nothing)) Then
				wellKnownTypeMember = Me._factory.WellKnownMember(Of MethodSymbol)(WellKnownMember.System_Delegate__CreateDelegate4, False)
				If (wellKnownTypeMember IsNot Nothing) Then
					boundExpression1 = Me._factory.[Call](Me._factory.Null(Me.ObjectType), DirectCast(wellKnownTypeMember, MethodSymbol), New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { Me._factory.[Typeof](type), receiverOpt, boundExpression2, Me._factory.Literal(False) })
					boundExpression = Me.Convert(Me.Visit(boundExpression1), type, False)
					Return boundExpression
				End If
				boundExpression = node
				Return boundExpression
			Else
				Me.Diagnostics.AddDependencies(useSiteInfo)
				boundExpression1 = Me._factory.[Call](boundExpression2, DirectCast(wellKnownTypeMember, MethodSymbol), New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { Me._factory.[Typeof](type), receiverOpt })
			End If
			boundExpression = Me.Convert(Me.Visit(boundExpression1), type, False)
			Return boundExpression
		End Function

		Private Function VisitDirectCast(ByVal node As BoundDirectCast) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			boundExpression = If(Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(node.ConversionKind), Me.ConvertExpression(node.Operand, node.ConversionKind, node.Operand.Type, node.Type, False, True, ExpressionLambdaRewriter.ConversionSemantics.[DirectCast]), Me.VisitInternal(node.Operand))
			Return boundExpression
		End Function

		Private Function VisitExpressionWithoutStackGuard(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim kind As BoundKind = node.Kind
			If (kind > BoundKind.ObjectCreationExpression) Then
				If (kind > BoundKind.Sequence) Then
					Select Case kind
						Case BoundKind.Literal
						Case BoundKind.MeReference
						Case BoundKind.MyClassReference
						Case BoundKind.Local
							boundExpression = Me.CreateLiteralExpression(node)
							Return boundExpression
						Case BoundKind.ValueTypeMeReference
						Case BoundKind.PreviousSubmissionReference
						Case BoundKind.HostObjectMemberReference
						Case BoundKind.PseudoVariable
							Throw ExceptionUtilities.UnexpectedValue(node.Kind)
						Case BoundKind.MyBaseReference
							Throw ExceptionUtilities.UnexpectedValue(node.Kind)
						Case BoundKind.Parameter
							boundExpression = Me.VisitParameter(DirectCast(node, BoundParameter))
							Exit Select
						Case Else
							If (kind = BoundKind.Lambda) Then
								boundExpression = Me.VisitLambda(DirectCast(node, BoundLambda))
								Exit Select
							Else
								Throw ExceptionUtilities.UnexpectedValue(node.Kind)
							End If
					End Select
				Else
					Select Case kind
						Case BoundKind.NewT
							boundExpression = Me.VisitNewT(DirectCast(node, BoundNewT))
							Exit Select
						Case BoundKind.DelegateCreationExpression
							boundExpression = Me.VisitDelegateCreationExpression(DirectCast(node, BoundDelegateCreationExpression))
							Exit Select
						Case BoundKind.ArrayCreation
							boundExpression = Me.VisitArrayCreation(DirectCast(node, BoundArrayCreation))
							Exit Select
						Case BoundKind.ArrayLiteral
						Case BoundKind.ArrayInitialization
							Throw ExceptionUtilities.UnexpectedValue(node.Kind)
						Case BoundKind.FieldAccess
							Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess)
							If (Not boundFieldAccess.FieldSymbol.IsCapturedFrame) Then
								boundExpression = Me.VisitFieldAccess(boundFieldAccess)
								Exit Select
							Else
								boundExpression = Me.CreateLiteralExpression(node)
								Exit Select
							End If
						Case BoundKind.PropertyAccess
							boundExpression = Me.VisitPropertyAccess(DirectCast(node, BoundPropertyAccess))
							Exit Select
						Case Else
							If (kind = BoundKind.Sequence) Then
								boundExpression = Me.VisitSequence(DirectCast(node, BoundSequence))
								Exit Select
							Else
								Throw ExceptionUtilities.UnexpectedValue(node.Kind)
							End If
					End Select
				End If
			ElseIf (kind <= BoundKind.[TypeOf]) Then
				Select Case kind
					Case BoundKind.BadExpression
						boundExpression = Me.VisitBadExpression(DirectCast(node, BoundBadExpression))
						Exit Select
					Case BoundKind.BadStatement
					Case BoundKind.Parenthesized
					Case BoundKind.BadVariable
						Throw ExceptionUtilities.UnexpectedValue(node.Kind)
					Case BoundKind.ArrayAccess
						boundExpression = Me.VisitArrayAccess(DirectCast(node, BoundArrayAccess))
						Exit Select
					Case BoundKind.ArrayLength
						boundExpression = Me.VisitArrayLength(DirectCast(node, BoundArrayLength))
						Exit Select
					Case BoundKind.[GetType]
					Case BoundKind.FieldInfo
					Case BoundKind.MethodInfo
						boundExpression = Me.CreateLiteralExpression(node)
						Return boundExpression
					Case Else
						Select Case kind
							Case BoundKind.UnaryOperator
								boundExpression = Me.VisitUnaryOperator(DirectCast(node, BoundUnaryOperator))

							Case BoundKind.UserDefinedUnaryOperator
								boundExpression = Me.VisitUserDefinedUnaryOperator(DirectCast(node, BoundUserDefinedUnaryOperator))

							Case BoundKind.NullableIsTrueOperator
								boundExpression = Me.VisitNullableIsTrueOperator(DirectCast(node, BoundNullableIsTrueOperator))

							Case BoundKind.BinaryOperator
								boundExpression = Me.VisitBinaryOperator(DirectCast(node, BoundBinaryOperator))

							Case BoundKind.UserDefinedBinaryOperator
								boundExpression = Me.VisitUserDefinedBinaryOperator(DirectCast(node, BoundUserDefinedBinaryOperator))

							Case BoundKind.UserDefinedShortCircuitingOperator
								boundExpression = Me.VisitUserDefinedShortCircuitingOperator(DirectCast(node, BoundUserDefinedShortCircuitingOperator))

							Case BoundKind.CompoundAssignmentTargetPlaceholder
							Case BoundKind.AssignmentOperator
							Case BoundKind.ReferenceAssignment
							Case BoundKind.AddressOfOperator
							Case BoundKind.RelaxationLambda
							Case BoundKind.ConvertedTupleElements
							Case BoundKind.UserDefinedConversion
								Throw ExceptionUtilities.UnexpectedValue(node.Kind)
							Case BoundKind.TernaryConditionalExpression
								boundExpression = Me.VisitTernaryConditionalExpression(DirectCast(node, BoundTernaryConditionalExpression))

							Case BoundKind.BinaryConditionalExpression
								boundExpression = Me.VisitBinaryConditionalExpression(DirectCast(node, BoundBinaryConditionalExpression))

							Case BoundKind.Conversion
								boundExpression = Me.VisitConversion(DirectCast(node, BoundConversion))

							Case BoundKind.[DirectCast]
								boundExpression = Me.VisitDirectCast(DirectCast(node, BoundDirectCast))

							Case BoundKind.[TryCast]
								boundExpression = Me.VisitTryCast(DirectCast(node, BoundTryCast))

							Case BoundKind.[TypeOf]
								boundExpression = Me.VisitTypeOf(DirectCast(node, BoundTypeOf))

							Case Else
								Throw ExceptionUtilities.UnexpectedValue(node.Kind)
						End Select

				End Select
			ElseIf (kind = BoundKind.[Call]) Then
				boundExpression = Me.VisitCall(DirectCast(node, BoundCall))
			Else
				If (kind <> BoundKind.ObjectCreationExpression) Then
					Throw ExceptionUtilities.UnexpectedValue(node.Kind)
				End If
				boundExpression = Me.VisitObjectCreationExpression(DirectCast(node, BoundObjectCreationExpression))
			End If
			Return boundExpression
		End Function

		Private Function VisitExpressionWithStackGuard(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Try
				boundExpression = Me.VisitExpressionWithoutStackGuard(node)
			Catch insufficientExecutionStackException As System.InsufficientExecutionStackException
				ProjectData.SetProjectError(insufficientExecutionStackException)
				Throw New BoundTreeVisitor.CancelledByStackGuardException(insufficientExecutionStackException, node)
			End Try
			Return boundExpression
		End Function

		Private Function VisitFieldAccess(ByVal node As BoundFieldAccess) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim receiverOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ReceiverOpt
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = node.FieldSymbol
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
			If (Not fieldSymbol.IsShared) Then
				boundExpression = If(receiverOpt.Kind <> BoundKind.MyBaseReference, Me.Visit(receiverOpt), Me.CreateLiteralExpression(receiverOpt.MakeRValue(), fieldSymbol.ContainingType))
			Else
				boundExpression = Me._factory.Null()
			End If
			Return Me.ConvertRuntimeHelperToExpressionTree("Field", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, Me._factory.FieldInfo(fieldSymbol) })
		End Function

		Private Function VisitInternal(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Me._recursionDepth = Me._recursionDepth + 1
			If (Me._recursionDepth <= 1) Then
				boundExpression = Me.VisitExpressionWithStackGuard(node)
			Else
				StackGuard.EnsureSufficientExecutionStack(Me._recursionDepth)
				boundExpression = Me.VisitExpressionWithoutStackGuard(node)
			End If
			Me._recursionDepth = Me._recursionDepth - 1
			Return boundExpression
		End Function

		Private Function VisitLambda(ByVal node As BoundLambda) As BoundExpression
			Throw ExceptionUtilities.Unreachable
		End Function

		Private Function VisitLambdaInternal(ByVal node As BoundLambda, ByVal delegateType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._factory.WellKnownType(WellKnownType.System_Linq_Expressions_ParameterExpression)
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).GetInstance()
			Dim boundExpressions As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
			Dim instance1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
			Dim parameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = node.LambdaSymbol.Parameters
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol).Enumerator = parameters.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = enumerator.Current
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Me._factory.SynthesizedLocal(namedTypeSymbol, SynthesizedLocalKind.LoweringTemp, Nothing)
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me._factory.Local(localSymbol, False)
				Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me._factory.Local(localSymbol, True)
				instance.Add(localSymbol)
				instance1.Add(boundLocal)
				Me._parameterMap(current) = boundLocal
				Dim expressionTree As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.ConvertRuntimeHelperToExpressionTree("Parameter", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { Me._factory.[Typeof](current.Type.InternalSubstituteTypeParameters(Me._typeMap).Type), Me._factory.Literal(current.Name) })
				If (expressionTree.HasErrors) Then
					Continue While
				End If
				boundExpressions.Add(Me._factory.AssignmentExpression(boundLocal1, expressionTree))
			End While
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.TranslateLambdaBody(node.Body)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me._factory.Sequence(instance.ToImmutableAndFree(), boundExpressions.ToImmutableAndFree(), Me.ConvertRuntimeHelperToExpressionTree("Lambda", ImmutableArray.Create(Of TypeSymbol)(delegateType), New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, Me._factory.Array(namedTypeSymbol, instance1.ToImmutableAndFree()) }))
			parameters = node.LambdaSymbol.Parameters
			Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol).Enumerator = parameters.GetEnumerator()
			While enumerator1.MoveNext()
				Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = enumerator1.Current
				Me._parameterMap.Remove(parameterSymbol)
			End While
			Return boundExpression1
		End Function

		Private Function VisitNewT(ByVal node As BoundNewT) As BoundExpression
			Return Me.VisitObjectCreationContinued(Me.ConvertRuntimeHelperToExpressionTree("New", New BoundExpression() { Me._factory.[Typeof](node.Type) }), node.InitializerOpt)
		End Function

		Private Function VisitNullableIsTrueOperator(ByVal node As BoundNullableIsTrueOperator) As BoundExpression
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundExpression Microsoft.CodeAnalysis.VisualBasic.ExpressionLambdaRewriter::VisitNullableIsTrueOperator(Microsoft.CodeAnalysis.VisualBasic.BoundNullableIsTrueOperator)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundExpression VisitNullableIsTrueOperator(Microsoft.CodeAnalysis.VisualBasic.BoundNullableIsTrueOperator)
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

		Private Function VisitObjectCreationContinued(ByVal creation As BoundExpression, ByVal initializerOpt As BoundExpression) As BoundExpression
			Dim expressionTree As BoundExpression
			If (initializerOpt IsNot Nothing) Then
				Dim kind As BoundKind = initializerOpt.Kind
				If (kind = BoundKind.ObjectInitializerExpression) Then
					expressionTree = Me.ConvertRuntimeHelperToExpressionTree("MemberInit", New BoundExpression() { creation, Me.VisitObjectInitializer(DirectCast(initializerOpt, BoundObjectInitializerExpression)) })
				Else
					If (kind <> BoundKind.CollectionInitializerExpression) Then
						Throw ExceptionUtilities.UnexpectedValue(initializerOpt.Kind)
					End If
					expressionTree = Me.ConvertRuntimeHelperToExpressionTree("ListInit", New BoundExpression() { creation, Me.VisitCollectionInitializer(DirectCast(initializerOpt, BoundCollectionInitializerExpression)) })
				End If
			Else
				expressionTree = creation
			End If
			Return expressionTree
		End Function

		Private Function VisitObjectCreationExpression(ByVal node As BoundObjectCreationExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitObjectCreationExpressionInternal(node)
			Return Me.VisitObjectCreationContinued(boundExpression, node.InitializerOpt)
		End Function

		Private Function VisitObjectCreationExpressionInternal(ByVal node As BoundObjectCreationExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim expressionTree As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (node.ConstantValueOpt IsNot Nothing) Then
				expressionTree = Me.CreateLiteralExpression(node)
			ElseIf (node.ConstructorOpt Is Nothing OrElse node.Arguments.Length = 0 AndAlso Not node.Type.IsStructureType() OrElse node.ConstructorOpt.IsDefaultValueTypeConstructor()) Then
				expressionTree = Me.ConvertRuntimeHelperToExpressionTree("New", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { Me._factory.[Typeof](node.Type) })
			Else
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me._factory.ConstructorInfo(node.ConstructorOpt)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.ConvertArgumentsIntoArray(node.Arguments)
				If (Not node.Type.IsAnonymousType OrElse node.Arguments.Length = 0) Then
					expressionTree = Me.ConvertRuntimeHelperToExpressionTree("New", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, boundExpression1 })
				Else
					Dim properties As ImmutableArray(Of AnonymousTypeManager.AnonymousTypePropertyPublicSymbol) = DirectCast(node.Type, AnonymousTypeManager.AnonymousTypePublicSymbol).Properties
					Dim boundExpressionArray(properties.Length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
					Dim length As Integer = properties.Length - 1
					Dim num As Integer = 0
					Do
						boundExpressionArray(num) = Me._factory.Convert(Me.MemberInfoType, Me._factory.MethodInfo(properties(num).GetMethod), False)
						num = num + 1
					Loop While num <= length
					expressionTree = Me.ConvertRuntimeHelperToExpressionTree("New", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, boundExpression1, Me._factory.Array(Me.MemberInfoType, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpressionArray)) })
				End If
			End If
			Return expressionTree
		End Function

		Private Function VisitObjectInitializer(ByVal initializer As BoundObjectInitializerExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim initializers As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = initializer.Initializers
			Dim length As Integer = initializers.Length
			Dim boundExpressionArray(length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim num As Integer = length - 1
			Dim num1 As Integer = 0
			Do
				Dim item As BoundAssignmentOperator = DirectCast(initializers(num1), BoundAssignmentOperator)
				Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = item.Left
				Dim fieldSymbol As Symbol = Nothing
				Dim kind As BoundKind = left.Kind
				If (kind = BoundKind.FieldAccess) Then
					fieldSymbol = DirectCast(item.Left, BoundFieldAccess).FieldSymbol
				Else
					If (kind <> BoundKind.PropertyAccess) Then
						Throw ExceptionUtilities.UnexpectedValue(left.Kind)
					End If
					fieldSymbol = DirectCast(item.Left, BoundPropertyAccess).PropertySymbol
				End If
				Dim right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = item.Right
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = If(fieldSymbol.Kind = SymbolKind.Field, Me._factory.FieldInfo(DirectCast(fieldSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)), Me._factory.MethodInfo(DirectCast(fieldSymbol, PropertySymbol).SetMethod))
				boundExpressionArray(num1) = Me._factory.Convert(Me.MemberBindingType, Me.ConvertRuntimeHelperToExpressionTree("Bind", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, Me.Visit(right) }), False)
				num1 = num1 + 1
			Loop While num1 <= num
			Return Me._factory.Array(Me.MemberBindingType, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpressionArray))
		End Function

		Private Function VisitParameter(ByVal node As BoundParameter) As BoundExpression
			Return Me._parameterMap(node.ParameterSymbol)
		End Function

		Private Function VisitPropertyAccess(ByVal node As BoundPropertyAccess) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim receiverOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ReceiverOpt
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = node.PropertySymbol
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
			boundExpression = If(Not propertySymbol.IsShared, Me.Visit(receiverOpt), Me._factory.Null())
			Dim mostDerivedGetMethod As MethodSymbol = propertySymbol.GetMostDerivedGetMethod()
			Return Me.ConvertRuntimeHelperToExpressionTree("Property", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, Me._factory.MethodInfo(mostDerivedGetMethod) })
		End Function

		Private Function VisitSequence(ByVal node As BoundSequence) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim locals As ImmutableArray(Of LocalSymbol) = node.Locals
			Dim sideEffects As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = node.SideEffects
			Dim valueOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ValueOpt
			boundExpression = If(Not locals.IsEmpty OrElse Not sideEffects.IsEmpty OrElse valueOpt Is Nothing, Me.GenerateDiagnosticAndReturnDummyExpression(ERRID.ERR_ExpressionTreeNotSupported, node, New [Object](-1) {}), Me.VisitInternal(valueOpt))
			Return boundExpression
		End Function

		Private Function VisitTernaryConditionalExpression(ByVal node As BoundTernaryConditionalExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.Visit(node.Condition)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.Visit(node.WhenTrue)
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.Visit(node.WhenFalse)
			Return Me.ConvertRuntimeHelperToExpressionTree("Condition", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, boundExpression1, boundExpression2 })
		End Function

		Private Function VisitTryCast(ByVal node As BoundTryCast) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			boundExpression = If(Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(node.ConversionKind), Me.ConvertExpression(node.Operand, node.ConversionKind, node.Operand.Type, node.Type, False, True, ExpressionLambdaRewriter.ConversionSemantics.[TryCast]), Me.VisitInternal(node.Operand))
			Return boundExpression
		End Function

		Private Function VisitTypeOf(ByVal node As BoundTypeOf) As BoundExpression
			Return Me.ConvertRuntimeHelperToExpressionTree("TypeIs", New BoundExpression() { Me.Visit(node.Operand), Me._factory.[Typeof](node.TargetType) })
		End Function

		Private Function VisitUnaryOperator(ByVal node As BoundUnaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim expressionTree As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim operand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Operand
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = operand.Type
			Dim nullableUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type.GetNullableUnderlyingTypeOrSelf()
			Dim enumUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = nullableUnderlyingTypeOrSelf.GetEnumUnderlyingTypeOrSelf()
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = enumUnderlyingTypeOrSelf.SpecialType
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.Visit(operand)
			Dim operatorKind As UnaryOperatorKind = node.OperatorKind And UnaryOperatorKind.OpMask
			Dim flag As Boolean = If(Not node.Checked, False, enumUnderlyingTypeOrSelf.IsIntegralType())
			Dim str As String = Nothing
			Select Case operatorKind
				Case UnaryOperatorKind.Plus
					If (type.IsReferenceType) Then
						Dim helperForObjectUnaryOperation As MethodSymbol = Me.GetHelperForObjectUnaryOperation(operatorKind)
						expressionTree = If(helperForObjectUnaryOperation Is Nothing, boundExpression, Me.ConvertRuntimeHelperToExpressionTree("UnaryPlus", New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, Me._factory.MethodInfo(helperForObjectUnaryOperation) }))
						Return expressionTree
					Else
						expressionTree = boundExpression
						Return expressionTree
					End If
				Case UnaryOperatorKind.Minus
					str = If(flag, "NegateChecked", "Negate")
					Exit Select
				Case UnaryOperatorKind.[Not]
					str = "Not"
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(operatorKind)
			End Select
			Dim helperForDecimalUnaryOperation As MethodSymbol = Nothing
			If (type.IsReferenceType) Then
				helperForDecimalUnaryOperation = Me.GetHelperForObjectUnaryOperation(operatorKind)
			ElseIf (enumUnderlyingTypeOrSelf.IsDecimalType()) Then
				helperForDecimalUnaryOperation = Me.GetHelperForDecimalUnaryOperation(operatorKind)
			End If
			If (helperForDecimalUnaryOperation Is Nothing) Then
				Dim flag1 As Boolean = If(specialType = Microsoft.CodeAnalysis.SpecialType.System_Byte, True, specialType = Microsoft.CodeAnalysis.SpecialType.System_SByte)
				Dim flag2 As Boolean = type.IsNullableType()
				boundExpression = Me.GenerateCastsForBinaryAndUnaryOperator(boundExpression, flag2, nullableUnderlyingTypeOrSelf, If(Not flag, False, Me.IsIntegralType(enumUnderlyingTypeOrSelf)), flag1)
				Dim expressionTree1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.ConvertRuntimeHelperToExpressionTree(str, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression })
				If (flag1) Then
					Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = expressionTree1
					If (flag2) Then
						typeSymbol = Me._factory.NullableOf(enumUnderlyingTypeOrSelf)
					Else
						typeSymbol = enumUnderlyingTypeOrSelf
					End If
					expressionTree1 = Me.Convert(boundExpression1, typeSymbol, flag)
				End If
				If (nullableUnderlyingTypeOrSelf.IsEnumType()) Then
					expressionTree1 = Me.Convert(expressionTree1, type, False)
				End If
				expressionTree = expressionTree1
			Else
				expressionTree = Me.ConvertRuntimeHelperToExpressionTree(str, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression, Me._factory.MethodInfo(helperForDecimalUnaryOperation) })
			End If
			Return expressionTree
		End Function

		Private Function VisitUserDefinedBinaryOperator(ByVal node As BoundUserDefinedBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim operatorKind As BinaryOperatorKind = node.OperatorKind And BinaryOperatorKind.OpMask
			Dim flag As Boolean = CInt((node.OperatorKind And BinaryOperatorKind.Lifted)) <> 0
			Dim flag1 As Boolean = If(Not node.Checked, False, Me.IsIntegralType(node.[Call].Method.ReturnType))
			If (CInt(operatorKind) - CInt(BinaryOperatorKind.Concatenate) <= CInt(BinaryOperatorKind.Add)) Then
				boundExpression = Me.ConvertUserDefinedLikeOrConcate(node)
			Else
				boundExpression = If(CInt(operatorKind) - CInt(BinaryOperatorKind.Equals) <= CInt(BinaryOperatorKind.NotEquals) OrElse CInt(operatorKind) - CInt(BinaryOperatorKind.[Is]) <= CInt(BinaryOperatorKind.Add), Me.ConvertRuntimeHelperToExpressionTree(ExpressionLambdaRewriter.GetBinaryOperatorMethodName(operatorKind, flag1), New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { Me.Visit(node.Left), Me.Visit(node.Right), Me._factory.Literal(flag), Me._factory.MethodInfo(node.[Call].Method) }), Me.ConvertRuntimeHelperToExpressionTree(ExpressionLambdaRewriter.GetBinaryOperatorMethodName(operatorKind, flag1), New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { Me.Visit(node.Left), Me.Visit(node.Right), Me._factory.MethodInfo(node.[Call].Method) }))
			End If
			Return boundExpression
		End Function

		Private Function VisitUserDefinedShortCircuitingOperator(ByVal node As BoundUserDefinedShortCircuitingOperator) As BoundExpression
			Dim bitwiseOperator As BoundUserDefinedBinaryOperator = node.BitwiseOperator
			Return Me.ConvertRuntimeHelperToExpressionTree(ExpressionLambdaRewriter.GetBinaryOperatorMethodName(If((bitwiseOperator.OperatorKind And BinaryOperatorKind.OpMask) = BinaryOperatorKind.[And], BinaryOperatorKind.[AndAlso], BinaryOperatorKind.[OrElse]), False), New BoundExpression() { Me.Visit(bitwiseOperator.Left), Me.Visit(bitwiseOperator.Right), Me._factory.MethodInfo(bitwiseOperator.[Call].Method) })
		End Function

		Private Function VisitUserDefinedUnaryOperator(ByVal node As BoundUserDefinedUnaryOperator) As BoundExpression
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundExpression Microsoft.CodeAnalysis.VisualBasic.ExpressionLambdaRewriter::VisitUserDefinedUnaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedUnaryOperator)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundExpression VisitUserDefinedUnaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedUnaryOperator)
			' 
			' File d'attente vide.
			'    à System.ThrowHelper.ThrowInvalidOperationException(ExceptionResource resource)
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.(ICollection`1 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 525
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 445
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 363
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 307
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 86
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Enum ConversionSemantics
			[Default]
			[DirectCast]
			[TryCast]
		End Enum
	End Class
End Namespace