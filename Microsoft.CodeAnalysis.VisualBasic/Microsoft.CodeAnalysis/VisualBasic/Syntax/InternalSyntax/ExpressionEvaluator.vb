Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Reflection
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Structure ExpressionEvaluator
		Private ReadOnly _symbols As ImmutableDictionary(Of String, CConst)

		Private ReadOnly Shared s_dominantType As Byte(,)

		Shared Sub New()
			ExpressionEvaluator.s_dominantType = New Byte(,) { { 10, 1, 11, 12, 13, 14, 15, 16, 18, 19, 17, 1, 1, 1, 1, 1 }, { 1, 9, 11, 1, 13, 1, 15, 1, 18, 19, 17, 1, 1, 1, 1, 1 }, { 11, 11, 11, 1, 13, 1, 15, 1, 18, 19, 17, 1, 1, 1, 1, 1 }, { 12, 1, 1, 12, 13, 14, 15, 16, 18, 19, 17, 1, 1, 1, 1, 1 }, { 13, 13, 13, 13, 13, 1, 15, 1, 18, 19, 17, 1, 1, 1, 1, 1 }, { 14, 1, 1, 14, 1, 14, 15, 16, 18, 19, 17, 1, 1, 1, 1, 1 }, { 15, 15, 15, 15, 15, 15, 15, 1, 18, 19, 17, 1, 1, 1, 1, 1 }, { 16, 1, 1, 16, 1, 16, 1, 16, 18, 19, 17, 1, 1, 1, 1, 1 }, { 18, 18, 18, 18, 18, 18, 18, 18, 18, 19, 18, 1, 1, 1, 1, 1 }, { 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 1, 1, 1, 1, 1 }, { 17, 17, 17, 17, 17, 17, 17, 17, 18, 19, 17, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 33, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 8, 1, 20, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 7, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20, 1, 20, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } }
		End Sub

		Private Sub New(ByVal symbols As ImmutableDictionary(Of String, CConst))
			Me = New ExpressionEvaluator() With
			{
				._symbols = symbols
			}
		End Sub

		Private Shared Function AsTypeCharacter(ByVal specialType As Microsoft.CodeAnalysis.SpecialType) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter
			Dim typeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter
			Select Case specialType
				Case Microsoft.CodeAnalysis.SpecialType.System_Int32
					typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Integer]
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_UInt32
				Case Microsoft.CodeAnalysis.SpecialType.System_UInt64
				Label0:
					typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.None
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Int64
					typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Long]
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
					typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Decimal]
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Single
					typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Single]
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Double
					typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Double]
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_String
					typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[String]
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return typeCharacter
		End Function

		Private Shared Function Convert(ByVal value As CConst, ByVal toSpecialType As Microsoft.CodeAnalysis.SpecialType, ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As CConst
			Dim obj As CConst
			If (Not value.IsBad) Then
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = value.SpecialType
				If (specialType = toSpecialType) Then
					obj = value
				ElseIf (Not toSpecialType.IsNumericType()) Then
					Dim specialType1 As Microsoft.CodeAnalysis.SpecialType = toSpecialType
					If (specialType1 <= Microsoft.CodeAnalysis.SpecialType.System_Boolean) Then
						If (specialType1 = Microsoft.CodeAnalysis.SpecialType.System_Object) Then
							obj = ExpressionEvaluator.ConvertToObject(value, expr)
							Return obj
						Else
							If (specialType1 <> Microsoft.CodeAnalysis.SpecialType.System_Boolean) Then
								GoTo Label3
							End If
							obj = ExpressionEvaluator.ConvertToBool(value, expr)
							Return obj
						End If
					ElseIf (specialType1 = Microsoft.CodeAnalysis.SpecialType.System_Char) Then
						obj = ExpressionEvaluator.ConvertToChar(value, expr)
						Return obj
					ElseIf (specialType1 = Microsoft.CodeAnalysis.SpecialType.System_String) Then
						obj = ExpressionEvaluator.ConvertToString(value, expr)
						Return obj
					Else
						If (specialType1 <> Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
							GoTo Label3
						End If
						obj = ExpressionEvaluator.ConvertToDate(value, expr)
						Return obj
					End If
				Label3:
					Dim displayName() As [Object] = { specialType.GetDisplayName(), toSpecialType.GetDisplayName() }
					obj = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_CannotConvertValue2, expr, displayName)
				Else
					obj = ExpressionEvaluator.ConvertToNumeric(value, toSpecialType, expr)
				End If
			Else
				obj = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
			End If
			Return obj
		End Function

		Private Shared Function ConvertNumericToNumeric(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst, ByVal toSpecialType As SpecialType, ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Try
				cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.CreateChecked(RuntimeHelpers.GetObjectValue(Convert.ChangeType(RuntimeHelpers.GetObjectValue(value.ValueAsObject), toSpecialType.ToRuntimeType(), CultureInfo.InvariantCulture)))
			Catch overflowException As System.OverflowException
				ProjectData.SetProjectError(overflowException)
				cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_ExpressionOverflow1, expr, New [Object]() { toSpecialType.GetDisplayName() })
				ProjectData.ClearProjectError()
			End Try
			Return cConst
		End Function

		Private Shared Function ConvertToBool(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst, ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			If (Not value.IsBad) Then
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = value.SpecialType
				If (specialType = Microsoft.CodeAnalysis.SpecialType.System_Boolean) Then
					cConst = DirectCast(value, CConst(Of Boolean))
				ElseIf (Not specialType.IsNumericType()) Then
					If (specialType > Microsoft.CodeAnalysis.SpecialType.System_Char) Then
						If (specialType = Microsoft.CodeAnalysis.SpecialType.System_String) Then
							cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstConversion2, expr, New [Object]() { Microsoft.CodeAnalysis.SpecialType.System_String.GetDisplayName(), Microsoft.CodeAnalysis.SpecialType.System_Boolean.GetDisplayName() })
							Return cConst
						Else
							If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
								GoTo Label3
							End If
							cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, New [Object]() { Microsoft.CodeAnalysis.SpecialType.System_DateTime.GetDisplayName(), Microsoft.CodeAnalysis.SpecialType.System_Boolean.GetDisplayName() })
							Return cConst
						End If
					ElseIf (specialType <> Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_Char) Then
							GoTo Label3
						End If
						cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, New [Object]() { Microsoft.CodeAnalysis.SpecialType.System_Char.GetDisplayName(), Microsoft.CodeAnalysis.SpecialType.System_Boolean.GetDisplayName() })
						Return cConst
					ElseIf (value.ValueAsObject IsNot Nothing) Then
						cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
						Return cConst
					Else
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(False)
						Return cConst
					End If
				Label3:
					Dim displayName() As [Object] = { specialType, Microsoft.CodeAnalysis.SpecialType.System_Boolean.GetDisplayName() }
					cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, displayName)
				Else
					cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Microsoft.VisualBasic.CompilerServices.Conversions.ToBoolean(value.ValueAsObject))
				End If
			Else
				cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
			End If
			Return cConst
		End Function

		Private Shared Function ConvertToChar(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst, ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			If (Not value.IsBad) Then
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = value.SpecialType
				If (specialType = Microsoft.CodeAnalysis.SpecialType.System_Char) Then
					cConst = DirectCast(value, CConst(Of Char))
				ElseIf (Not specialType.IsIntegralType()) Then
					If (specialType > Microsoft.CodeAnalysis.SpecialType.System_Boolean) Then
						If (specialType = Microsoft.CodeAnalysis.SpecialType.System_String) Then
							Dim cConst1 As CConst(Of String) = DirectCast(value, CConst(Of String))
							cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(If(cConst1.Value Is Nothing, Strings.ChrW(0), Microsoft.VisualBasic.CompilerServices.Conversions.ToChar(cConst1.Value)))
							Return cConst
						Else
							If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
								GoTo Label3
							End If
							cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, New [Object]() { specialType, Microsoft.CodeAnalysis.SpecialType.System_Char.GetDisplayName() })
							Return cConst
						End If
					ElseIf (specialType <> Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_Boolean) Then
							GoTo Label3
						End If
						cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, New [Object]() { specialType, Microsoft.CodeAnalysis.SpecialType.System_Char.GetDisplayName() })
						Return cConst
					ElseIf (value.ValueAsObject IsNot Nothing) Then
						cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
						Return cConst
					Else
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Strings.ChrW(0))
						Return cConst
					End If
				Label3:
					Dim displayName() As [Object] = { specialType, Microsoft.CodeAnalysis.SpecialType.System_Char.GetDisplayName() }
					cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, displayName)
				Else
					cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_IntegralToCharTypeMismatch1, expr, New [Object]() { specialType.GetDisplayName() })
				End If
			Else
				cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
			End If
			Return cConst
		End Function

		Private Shared Function ConvertToDate(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst, ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			If (Not value.IsBad) Then
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = value.SpecialType
				If (specialType = Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
					cConst = DirectCast(value, CConst(Of DateTime))
				ElseIf (Not specialType.IsIntegralType()) Then
					If (specialType > Microsoft.CodeAnalysis.SpecialType.System_Boolean) Then
						If (specialType = Microsoft.CodeAnalysis.SpecialType.System_Char) Then
							cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, New [Object]() { specialType, Microsoft.CodeAnalysis.SpecialType.System_DateTime.GetDisplayName() })
							Return cConst
						Else
							If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_String) Then
								GoTo Label3
							End If
							cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
							Return cConst
						End If
					ElseIf (specialType <> Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_Boolean) Then
							GoTo Label3
						End If
						cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, New [Object]() { specialType, Microsoft.CodeAnalysis.SpecialType.System_DateTime.GetDisplayName() })
						Return cConst
					ElseIf (value.ValueAsObject IsNot Nothing) Then
						cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
						Return cConst
					Else
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(DateTime.MinValue)
						Return cConst
					End If
				Label3:
					Dim displayName() As [Object] = { specialType, Microsoft.CodeAnalysis.SpecialType.System_DateTime.GetDisplayName() }
					cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, displayName)
				Else
					cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, New [Object]() { specialType, Microsoft.CodeAnalysis.SpecialType.System_DateTime.GetDisplayName() })
				End If
			Else
				cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
			End If
			Return cConst
		End Function

		Private Shared Function ConvertToNumeric(ByVal value As CConst, ByVal toSpecialType As Microsoft.CodeAnalysis.SpecialType, ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As CConst
			Dim numeric As CConst
			If (Not value.IsBad) Then
				If (ExpressionEvaluator.IsNothing(value)) Then
					value = CConst.Create(0)
				End If
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = value.SpecialType
				If (specialType = toSpecialType) Then
					numeric = value
				ElseIf (Not specialType.IsNumericType()) Then
					If (specialType <= Microsoft.CodeAnalysis.SpecialType.System_Boolean) Then
						If (specialType = Microsoft.CodeAnalysis.SpecialType.System_Object) Then
							numeric = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
							Return numeric
						Else
							If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_Boolean) Then
								GoTo Label3
							End If
							Dim num As Long = CLng((-DirectCast(value, CConst(Of Boolean)).Value))
							If (toSpecialType.IsUnsignedIntegralType()) Then
								Dim flag As Boolean = False
								num = CompileTimeCalculations.NarrowIntegralResult(num, Microsoft.CodeAnalysis.SpecialType.System_Int64, toSpecialType, flag)
							End If
							numeric = CConst.CreateChecked(RuntimeHelpers.GetObjectValue(Convert.ChangeType(num, toSpecialType.ToRuntimeType(), CultureInfo.InvariantCulture)))
							Return numeric
						End If
					ElseIf (specialType = Microsoft.CodeAnalysis.SpecialType.System_Char) Then
						numeric = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, New [Object]() { Microsoft.CodeAnalysis.SpecialType.System_Char.GetDisplayName(), toSpecialType.GetDisplayName() })
						Return numeric
					ElseIf (specialType = Microsoft.CodeAnalysis.SpecialType.System_String) Then
						numeric = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstConversion2, expr, New [Object]() { Microsoft.CodeAnalysis.SpecialType.System_String.GetDisplayName(), toSpecialType.GetDisplayName() })
						Return numeric
					Else
						If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
							GoTo Label3
						End If
						numeric = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, New [Object]() { Microsoft.CodeAnalysis.SpecialType.System_DateTime.GetDisplayName(), toSpecialType.GetDisplayName() })
						Return numeric
					End If
				Label3:
					Dim displayName() As [Object] = { specialType.GetDisplayName(), toSpecialType.GetDisplayName() }
					numeric = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, displayName)
				Else
					numeric = ExpressionEvaluator.ConvertNumericToNumeric(value, toSpecialType, expr)
				End If
			Else
				numeric = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
			End If
			Return numeric
		End Function

		Private Shared Function ConvertToObject(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst, ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			If (Not value.IsBad) Then
				cConst = If(Not ExpressionEvaluator.IsNothing(value), ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstConversion2, expr, New [Object]() { value.SpecialType.GetDisplayName(), SpecialType.System_Object.GetDisplayName() }), ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr))
			Else
				cConst = value
			End If
			Return cConst
		End Function

		Private Shared Function ConvertToString(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst, ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			If (Not value.IsBad) Then
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = value.SpecialType
				If (specialType = Microsoft.CodeAnalysis.SpecialType.System_String) Then
					cConst = DirectCast(value, CConst(Of String))
				ElseIf (Not specialType.IsIntegralType()) Then
					If (specialType > Microsoft.CodeAnalysis.SpecialType.System_Boolean) Then
						If (specialType = Microsoft.CodeAnalysis.SpecialType.System_Char) Then
							cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(DirectCast(value, CConst(Of Char)).Value))
							Return cConst
						Else
							If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
								GoTo Label3
							End If
							cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
							Return cConst
						End If
					ElseIf (specialType <> Microsoft.CodeAnalysis.SpecialType.System_Object) Then
						If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_Boolean) Then
							GoTo Label3
						End If
						cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
						Return cConst
					ElseIf (value.ValueAsObject IsNot Nothing) Then
						cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
						Return cConst
					Else
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Nothing)
						Return cConst
					End If
				Label3:
					Dim displayName() As [Object] = { specialType, Microsoft.CodeAnalysis.SpecialType.System_String.GetDisplayName() }
					cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, displayName)
				Else
					cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
				End If
			Else
				cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
			End If
			Return cConst
		End Function

		Private Function EvaluateBinaryExpression(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = Me.EvaluateExpressionInternal(expr.Left)
			Dim numeric As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = Me.EvaluateExpressionInternal(expr.Right)
			Dim kind As SyntaxKind = expr.Kind
			If (cConst1.IsBad OrElse numeric.IsBad) Then
				cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_BadCCExpression, expr)
			Else
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = Microsoft.CodeAnalysis.SpecialType.None
				If (ExpressionEvaluator.IsNothing(cConst1) OrElse ExpressionEvaluator.IsNothing(numeric)) Then
					If (ExpressionEvaluator.IsNothing(cConst1) AndAlso ExpressionEvaluator.IsNothing(numeric)) Then
						Select Case kind
							Case SyntaxKind.AddExpression
							Case SyntaxKind.SubtractExpression
							Case SyntaxKind.MultiplyExpression
							Case SyntaxKind.DivideExpression
							Case SyntaxKind.IntegerDivideExpression
							Case SyntaxKind.ExponentiateExpression
							Case SyntaxKind.LeftShiftExpression
							Case SyntaxKind.RightShiftExpression
							Case SyntaxKind.ModuloExpression
							Case SyntaxKind.EqualsExpression
							Case SyntaxKind.NotEqualsExpression
							Case SyntaxKind.LessThanExpression
							Case SyntaxKind.LessThanOrEqualExpression
							Case SyntaxKind.GreaterThanOrEqualExpression
							Case SyntaxKind.GreaterThanExpression
							Case SyntaxKind.IsExpression
							Case SyntaxKind.IsNotExpression
								numeric = ExpressionEvaluator.ConvertToNumeric(numeric, Microsoft.CodeAnalysis.SpecialType.System_Int32, expr.Right)
								Exit Select
							Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.RaiseEventStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.NothingLiteralExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.DirectCastExpression
							Case SyntaxKind.List Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.OptionStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.InheritsStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.TrueLiteralExpression Or SyntaxKind.NothingLiteralExpression Or SyntaxKind.ParenthesizedExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.DirectCastExpression Or SyntaxKind.TryCastExpression
								Throw ExceptionUtilities.UnexpectedValue(expr)
							Case SyntaxKind.ConcatenateExpression
							Case SyntaxKind.LikeExpression
								numeric = ExpressionEvaluator.ConvertToString(numeric, expr.Right)
								Exit Select
							Case SyntaxKind.OrExpression
							Case SyntaxKind.ExclusiveOrExpression
							Case SyntaxKind.AndExpression
								numeric = ExpressionEvaluator.ConvertToNumeric(numeric, Microsoft.CodeAnalysis.SpecialType.System_Int32, expr.Right)
								Exit Select
							Case SyntaxKind.OrElseExpression
							Case SyntaxKind.AndAlsoExpression
								numeric = ExpressionEvaluator.ConvertToBool(numeric, expr.Right)
								Exit Select
							Case Else
								Throw ExceptionUtilities.UnexpectedValue(expr)
						End Select
					End If
					If (ExpressionEvaluator.IsNothing(cConst1)) Then
						specialType = numeric.SpecialType
						If (CUShort(kind) - CUShort(SyntaxKind.LeftShiftExpression) <= CUShort(SyntaxKind.List)) Then
							specialType = Microsoft.CodeAnalysis.SpecialType.System_Int32
						ElseIf (kind = SyntaxKind.ConcatenateExpression OrElse kind = SyntaxKind.LikeExpression) Then
							specialType = Microsoft.CodeAnalysis.SpecialType.System_String
						End If
						cConst1 = ExpressionEvaluator.Convert(cConst1, specialType, expr.Left)
					ElseIf (ExpressionEvaluator.IsNothing(numeric)) Then
						specialType = cConst1.SpecialType
						If (kind = SyntaxKind.ConcatenateExpression OrElse kind = SyntaxKind.LikeExpression) Then
							specialType = Microsoft.CodeAnalysis.SpecialType.System_String
						End If
						numeric = ExpressionEvaluator.Convert(numeric, specialType, expr.Right)
					End If
				End If
				Dim specialType1 As Microsoft.CodeAnalysis.SpecialType = OperatorResolution.LookupInOperatorTables(kind, cConst1.SpecialType, numeric.SpecialType)
				If (specialType1 <> Microsoft.CodeAnalysis.SpecialType.None) Then
					specialType = specialType1
					cConst1 = ExpressionEvaluator.Convert(cConst1, specialType, expr.Left)
					If (Not cConst1.IsBad) Then
						numeric = ExpressionEvaluator.Convert(numeric, specialType, expr.Right)
						If (Not numeric.IsBad) Then
							If (kind <> SyntaxKind.AddExpression) Then
								If (CUShort(kind) - CUShort(SyntaxKind.EqualsExpression) <= CUShort(SyntaxKind.EndIfStatement)) Then
									specialType1 = Microsoft.CodeAnalysis.SpecialType.System_Boolean
								End If
							ElseIf (specialType1 = Microsoft.CodeAnalysis.SpecialType.System_String) Then
								kind = SyntaxKind.ConcatenateExpression
							End If
							cConst = ExpressionEvaluator.PerformCompileTimeBinaryOperation(kind, specialType1, cConst1, numeric, expr)
						Else
							cConst = numeric
						End If
					Else
						cConst = cConst1
					End If
				Else
					cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_BadTypeInCCExpression, expr)
				End If
			End If
			Return cConst
		End Function

		Private Function EvaluateBinaryIfExpression(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = Me.EvaluateExpressionInternal(expr.FirstExpression)
			Dim objectValue As Object = RuntimeHelpers.GetObjectValue(cConst1.ValueAsObject)
			If (objectValue Is Nothing) Then
				cConst = Me.EvaluateExpressionInternal(expr.SecondExpression)
			ElseIf (Not objectValue.[GetType]().GetTypeInfo().IsValueType) Then
				cConst = cConst1
			Else
				cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
			End If
			Return cConst
		End Function

		Public Shared Function EvaluateCondition(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, Optional ByVal symbols As ImmutableDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst) = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim badCConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			If (Not expr.ContainsDiagnostics) Then
				Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = ExpressionEvaluator.EvaluateExpression(expr, symbols)
				badCConst = If(Not cConst.IsBad, ExpressionEvaluator.ConvertToBool(cConst, expr), cConst)
			Else
				badCConst = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadCConst(ERRID.ERR_None)
			End If
			Return badCConst
		End Function

		Private Function EvaluateCTypeExpression(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = Me.EvaluateExpressionInternal(expr.Expression)
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax = TryCast(expr.Type, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax)
			If (type IsNot Nothing) Then
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = ExpressionEvaluator.GetSpecialType(type)
				cConst = ExpressionEvaluator.Convert(cConst1, specialType, expr)
			Else
				cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_BadTypeInCCExpression, expr.Type)
			End If
			Return cConst
		End Function

		Private Function EvaluateDirectCastExpression(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = Me.EvaluateExpressionInternal(expr.Expression)
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax = TryCast(expr.Type, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax)
			If (type IsNot Nothing) Then
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = ExpressionEvaluator.GetSpecialType(type)
				If (cConst1.SpecialType = Microsoft.CodeAnalysis.SpecialType.System_Object OrElse cConst1.SpecialType = Microsoft.CodeAnalysis.SpecialType.System_String) Then
					cConst = ExpressionEvaluator.Convert(cConst1, specialType, expr)
				ElseIf (cConst1.SpecialType <> specialType) Then
					cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr.Type, New [Object]() { cConst1.SpecialType, specialType })
				ElseIf (specialType = Microsoft.CodeAnalysis.SpecialType.System_Double OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_Single) Then
					cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_IdentityDirectCastForFloat, expr.Type)
				Else
					cConst = ExpressionEvaluator.Convert(cConst1, specialType, expr).WithError(ERRID.WRN_ObsoleteIdentityDirectCastForValueType)
				End If
			Else
				cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_BadTypeInCCExpression, expr.Type)
			End If
			Return cConst
		End Function

		Public Shared Function EvaluateExpression(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, Optional ByVal symbols As ImmutableDictionary(Of String, CConst) = Nothing) As CConst
			Dim badCConst As CConst
			If (Not expr.ContainsDiagnostics) Then
				badCConst = (New ExpressionEvaluator(symbols)).EvaluateExpressionInternal(expr)
			Else
				badCConst = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadCConst(ERRID.ERR_None)
			End If
			Return badCConst
		End Function

		Private Function EvaluateExpressionInternal(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim kind As SyntaxKind = expr.Kind
			Select Case kind
				Case SyntaxKind.CharacterLiteralExpression
				Case SyntaxKind.TrueLiteralExpression
				Case SyntaxKind.FalseLiteralExpression
				Case SyntaxKind.NumericLiteralExpression
				Case SyntaxKind.DateLiteralExpression
				Case SyntaxKind.StringLiteralExpression
				Case SyntaxKind.NothingLiteralExpression
					cConst = ExpressionEvaluator.EvaluateLiteralExpression(DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax))
					Exit Select
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.TrueLiteralExpression Or SyntaxKind.DateLiteralExpression
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.AddHandlerStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.FalseLiteralExpression Or SyntaxKind.DateLiteralExpression
				Case SyntaxKind.MeExpression
				Case SyntaxKind.MyBaseExpression
				Case SyntaxKind.MyClassExpression
				Case SyntaxKind.GetTypeExpression
				Case SyntaxKind.TypeOfIsExpression
				Case SyntaxKind.TypeOfIsNotExpression
				Case 288
				Case 289
				Case SyntaxKind.GetXmlNamespaceExpression
				Case SyntaxKind.SimpleMemberAccessExpression
				Case SyntaxKind.DictionaryAccessExpression
				Case SyntaxKind.XmlElementAccessExpression
				Case SyntaxKind.XmlDescendantAccessExpression
				Case SyntaxKind.XmlAttributeAccessExpression
				Case SyntaxKind.InvocationExpression
				Case SyntaxKind.ObjectCreationExpression
				Case SyntaxKind.AnonymousObjectCreationExpression
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.ReDimPreserveStatement Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.SimpleMemberAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.AnonymousObjectCreationExpression
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.MidExpression Or SyntaxKind.RaiseEventStatement Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.InvocationExpression
				Case SyntaxKind.ArrayCreationExpression
				Case SyntaxKind.CollectionInitializer
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.RaiseEventStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.NothingLiteralExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.DirectCastExpression
				Case SyntaxKind.List Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.OptionStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.InheritsStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.TrueLiteralExpression Or SyntaxKind.NothingLiteralExpression Or SyntaxKind.ParenthesizedExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.DirectCastExpression Or SyntaxKind.TryCastExpression
				Case SyntaxKind.IsExpression
				Case SyntaxKind.IsNotExpression
				Case SyntaxKind.LikeExpression
				Case SyntaxKind.AddressOfExpression
				Label0:
					cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_BadCCExpression, expr)
					Exit Select
				Case SyntaxKind.ParenthesizedExpression
					cConst = Me.EvaluateParenthesizedExpression(DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax))
					Exit Select
				Case SyntaxKind.CTypeExpression
					cConst = Me.EvaluateCTypeExpression(DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax))
					Exit Select
				Case SyntaxKind.DirectCastExpression
					cConst = Me.EvaluateDirectCastExpression(DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax))
					Exit Select
				Case SyntaxKind.TryCastExpression
					cConst = Me.EvaluateTryCastExpression(DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax))
					Exit Select
				Case SyntaxKind.PredefinedCastExpression
					cConst = Me.EvaluatePredefinedCastExpression(DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedCastExpressionSyntax))
					Exit Select
				Case SyntaxKind.AddExpression
				Case SyntaxKind.SubtractExpression
				Case SyntaxKind.MultiplyExpression
				Case SyntaxKind.DivideExpression
				Case SyntaxKind.IntegerDivideExpression
				Case SyntaxKind.ExponentiateExpression
				Case SyntaxKind.LeftShiftExpression
				Case SyntaxKind.RightShiftExpression
				Case SyntaxKind.ConcatenateExpression
				Case SyntaxKind.ModuloExpression
				Case SyntaxKind.EqualsExpression
				Case SyntaxKind.NotEqualsExpression
				Case SyntaxKind.LessThanExpression
				Case SyntaxKind.LessThanOrEqualExpression
				Case SyntaxKind.GreaterThanOrEqualExpression
				Case SyntaxKind.GreaterThanExpression
				Case SyntaxKind.OrExpression
				Case SyntaxKind.ExclusiveOrExpression
				Case SyntaxKind.AndExpression
				Case SyntaxKind.OrElseExpression
				Case SyntaxKind.AndAlsoExpression
					cConst = Me.EvaluateBinaryExpression(DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax))
					Exit Select
				Case SyntaxKind.UnaryPlusExpression
				Case SyntaxKind.UnaryMinusExpression
				Case SyntaxKind.NotExpression
					cConst = Me.EvaluateUnaryExpression(DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax))
					Exit Select
				Case SyntaxKind.BinaryConditionalExpression
					cConst = Me.EvaluateBinaryIfExpression(DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax))
					Exit Select
				Case SyntaxKind.TernaryConditionalExpression
					cConst = Me.EvaluateTernaryIfExpression(DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax))
					Exit Select
				Case Else
					If (kind = SyntaxKind.IdentifierName) Then
						cConst = Me.EvaluateIdentifierNameExpression(DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax))
						Exit Select
					Else
						GoTo Label0
					End If
			End Select
			Return cConst
		End Function

		Private Function EvaluateIdentifierNameExpression(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			If (Me._symbols IsNot Nothing) Then
				Dim identifier As IdentifierTokenSyntax = expr.Identifier
				Dim cConst1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = Nothing
				If (Not Me._symbols.TryGetValue(identifier.IdentifierText, cConst1)) Then
					cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.CreateNothing()
				ElseIf (Not cConst1.IsBad) Then
					Dim typeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter = identifier.TypeCharacter
					If (typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.None OrElse typeCharacter = ExpressionEvaluator.AsTypeCharacter(cConst1.SpecialType)) Then
						cConst = cConst1
					Else
						cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypecharNoMatch2, expr, New [Object]() { ExpressionEvaluator.GetDisplayString(typeCharacter), cConst1.SpecialType.GetDisplayName() })
					End If
				Else
					cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_None, expr)
				End If
			Else
				cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.CreateNothing()
			End If
			Return cConst
		End Function

		Private Shared Function EvaluateLiteralExpression(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = expr.Token
			If (Not expr.ContainsDiagnostics) Then
				Dim kind As SyntaxKind = token.Kind
				If (kind <= SyntaxKind.NothingKeyword) Then
					If (kind = SyntaxKind.FalseKeyword) Then
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(False)
						Return cConst
					Else
						If (kind <> SyntaxKind.NothingKeyword) Then
							Throw ExceptionUtilities.UnexpectedValue(token.Kind)
						End If
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.CreateNothing()
						Return cConst
					End If
				ElseIf (kind = SyntaxKind.TrueKeyword) Then
					cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(True)
					Return cConst
				Else
					Select Case kind
						Case SyntaxKind.IntegerLiteralToken
							cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.CreateChecked(RuntimeHelpers.GetObjectValue(DirectCast(token, IntegerLiteralTokenSyntax).ObjectValue))
							Return cConst
						Case SyntaxKind.FloatingLiteralToken
							cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.CreateChecked(RuntimeHelpers.GetObjectValue(DirectCast(token, FloatingLiteralTokenSyntax).ObjectValue))
							Return cConst
						Case SyntaxKind.DecimalLiteralToken
							cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(DirectCast(token, DecimalLiteralTokenSyntax).Value)
							Return cConst
						Case SyntaxKind.DateLiteralToken
							cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(DirectCast(token, DateLiteralTokenSyntax).Value)
							Return cConst
						Case SyntaxKind.StringLiteralToken
							cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(DirectCast(token, StringLiteralTokenSyntax).Value)
							Return cConst
						Case SyntaxKind.CharacterLiteralToken
							cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(DirectCast(token, CharacterLiteralTokenSyntax).Value)
							Return cConst
					End Select
				End If
				Throw ExceptionUtilities.UnexpectedValue(token.Kind)
			Else
				cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_BadCCExpression, expr)
			End If
			Return cConst
		End Function

		Private Function EvaluateParenthesizedExpression(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax) As CConst
			Return Me.EvaluateExpressionInternal(expr.Expression)
		End Function

		Private Function EvaluatePredefinedCastExpression(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedCastExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim obj As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim specialType As Microsoft.CodeAnalysis.SpecialType
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = Me.EvaluateExpressionInternal(expr.Expression)
			Select Case expr.Keyword.Kind
				Case SyntaxKind.CBoolKeyword
					specialType = Microsoft.CodeAnalysis.SpecialType.System_Boolean
					Exit Select
				Case SyntaxKind.CByteKeyword
					specialType = Microsoft.CodeAnalysis.SpecialType.System_Byte
					Exit Select
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.RedimClause Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlComment Or SyntaxKind.XmlCDataSection Or SyntaxKind.ArrayType Or SyntaxKind.PredefinedType Or SyntaxKind.AndKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CBoolKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RemoveHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.ReDimPreserveStatement Or SyntaxKind.RedimClause Or SyntaxKind.EraseStatement Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.SimpleMemberAccessExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlElementAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.XmlAttributeAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.ArrayCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.CTypeExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlPrefix Or SyntaxKind.XmlComment Or SyntaxKind.XmlProcessingInstruction Or SyntaxKind.XmlCDataSection Or SyntaxKind.XmlEmbeddedExpression Or SyntaxKind.ArrayType Or SyntaxKind.NullableType Or SyntaxKind.PredefinedType Or SyntaxKind.IdentifierName Or SyntaxKind.AndKeyword Or SyntaxKind.AndAlsoKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.BooleanKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByteKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CallKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CatchKeyword Or SyntaxKind.CBoolKeyword Or SyntaxKind.CByteKeyword
				Case SyntaxKind.CharKeyword
				Case SyntaxKind.ClassKeyword
				Case SyntaxKind.ConstKeyword
				Case SyntaxKind.ReferenceKeyword
				Case SyntaxKind.ContinueKeyword
				Case SyntaxKind.CTypeKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.LessThanOrEqualExpression Or SyntaxKind.GreaterThanOrEqualExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.CTypeKeyword Or SyntaxKind.CUIntKeyword Or SyntaxKind.CULngKeyword
				Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.MidExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlPrefixName Or SyntaxKind.CTypeKeyword
					Throw ExceptionUtilities.UnexpectedValue(expr.Keyword.Kind)
				Case SyntaxKind.CCharKeyword
					specialType = Microsoft.CodeAnalysis.SpecialType.System_Char
					Exit Select
				Case SyntaxKind.CDateKeyword
					specialType = Microsoft.CodeAnalysis.SpecialType.System_DateTime
					Exit Select
				Case SyntaxKind.CDecKeyword
					specialType = Microsoft.CodeAnalysis.SpecialType.System_Decimal
					Exit Select
				Case SyntaxKind.CDblKeyword
					specialType = Microsoft.CodeAnalysis.SpecialType.System_Double
					Exit Select
				Case SyntaxKind.CIntKeyword
					specialType = Microsoft.CodeAnalysis.SpecialType.System_Int32
					Exit Select
				Case SyntaxKind.CLngKeyword
					specialType = Microsoft.CodeAnalysis.SpecialType.System_Int64
					Exit Select
				Case SyntaxKind.CObjKeyword
					obj = ExpressionEvaluator.ConvertToObject(cConst, expr)
					Return obj
				Case SyntaxKind.CSByteKeyword
					specialType = Microsoft.CodeAnalysis.SpecialType.System_SByte
					Exit Select
				Case SyntaxKind.CShortKeyword
					specialType = Microsoft.CodeAnalysis.SpecialType.System_Int16
					Exit Select
				Case SyntaxKind.CSngKeyword
					specialType = Microsoft.CodeAnalysis.SpecialType.System_Single
					Exit Select
				Case SyntaxKind.CStrKeyword
					specialType = Microsoft.CodeAnalysis.SpecialType.System_String
					Exit Select
				Case SyntaxKind.CUIntKeyword
					specialType = Microsoft.CodeAnalysis.SpecialType.System_UInt32
					Exit Select
				Case SyntaxKind.CULngKeyword
					specialType = Microsoft.CodeAnalysis.SpecialType.System_UInt64
					Exit Select
				Case SyntaxKind.CUShortKeyword
					specialType = Microsoft.CodeAnalysis.SpecialType.System_UInt16
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(expr.Keyword.Kind)
			End Select
			obj = ExpressionEvaluator.Convert(cConst, specialType, expr)
			Return obj
		End Function

		Private Function EvaluateTernaryIfExpression(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = Me.EvaluateExpressionInternal(expr.Condition)
			If (Not cConst1.IsBad) Then
				Dim flag As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = ExpressionEvaluator.ConvertToBool(cConst1, expr)
				If (Not flag.IsBad) Then
					Dim cConst2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = Me.EvaluateExpressionInternal(expr.WhenTrue)
					Dim cConst3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = Me.EvaluateExpressionInternal(expr.WhenFalse)
					If (Not cConst2.IsBad AndAlso Not cConst3.IsBad) Then
						If (ExpressionEvaluator.IsNothing(cConst2)) Then
							If (Not ExpressionEvaluator.IsNothing(cConst3) AndAlso cConst3.SpecialType <> SpecialType.System_Object) Then
								cConst2 = ExpressionEvaluator.Convert(cConst2, cConst3.SpecialType, expr.WhenTrue)
							End If
						ElseIf (Not ExpressionEvaluator.IsNothing(cConst3)) Then
							Dim sDominantType As SpecialType = CSByte(ExpressionEvaluator.s_dominantType(ExpressionEvaluator.TypeCodeToDominantTypeIndex(cConst2.SpecialType), ExpressionEvaluator.TypeCodeToDominantTypeIndex(cConst3.SpecialType)))
							If (sDominantType <> cConst2.SpecialType) Then
								cConst2 = ExpressionEvaluator.Convert(cConst2, sDominantType, expr.WhenTrue)
							End If
							If (sDominantType <> cConst3.SpecialType) Then
								cConst3 = ExpressionEvaluator.Convert(cConst3, sDominantType, expr.WhenFalse)
							End If
						ElseIf (cConst2.SpecialType <> SpecialType.System_Object) Then
							cConst3 = ExpressionEvaluator.Convert(cConst3, cConst2.SpecialType, expr.WhenFalse)
						End If
					End If
					If (cConst2.IsBad) Then
						cConst = cConst2
					ElseIf (Not cConst3.IsBad) Then
						cConst = If(DirectCast(flag, CConst(Of Boolean)).Value, cConst2, cConst3)
					Else
						cConst = cConst3
					End If
				Else
					cConst = flag
				End If
			Else
				cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr)
			End If
			Return cConst
		End Function

		Private Function EvaluateTryCastExpression(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = Me.EvaluateExpressionInternal(expr.Expression)
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax = TryCast(expr.Type, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax)
			If (type IsNot Nothing) Then
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = ExpressionEvaluator.GetSpecialType(type)
				If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_Object AndAlso specialType <> Microsoft.CodeAnalysis.SpecialType.System_String) Then
					cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TryCastOfValueType1, expr.Type)
				ElseIf (cConst1.SpecialType = Microsoft.CodeAnalysis.SpecialType.System_Object OrElse cConst1.SpecialType = Microsoft.CodeAnalysis.SpecialType.System_String) Then
					cConst = ExpressionEvaluator.Convert(cConst1, specialType, expr)
				ElseIf (cConst1.SpecialType <> specialType) Then
					cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_TypeMismatch2, expr.Type, New [Object]() { cConst1.SpecialType.GetDisplayName(), specialType.GetDisplayName() })
				Else
					cConst = If(specialType = Microsoft.CodeAnalysis.SpecialType.System_Double OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_Single, ExpressionEvaluator.ReportSemanticError(ERRID.ERR_IdentityDirectCastForFloat, expr.Type), ExpressionEvaluator.ReportSemanticError(ERRID.WRN_ObsoleteIdentityDirectCastForValueType, expr.Type))
				End If
			Else
				cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_BadTypeInCCExpression, expr.Type)
			End If
			Return cConst
		End Function

		Private Function EvaluateUnaryExpression(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = Me.EvaluateExpressionInternal(expr.Operand)
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = cConst1.SpecialType
			If (specialType = Microsoft.CodeAnalysis.SpecialType.None) Then
				cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_BadCCExpression, expr)
			ElseIf (specialType = Microsoft.CodeAnalysis.SpecialType.System_String OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_Object AndAlso Not ExpressionEvaluator.IsNothing(cConst1) OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_Char OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
				cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_UnaryOperand2, expr, New [Object]() { expr.OperatorToken.ValueText, specialType.GetDisplayName() })
			Else
				Try
					Select Case expr.Kind
						Case SyntaxKind.UnaryPlusExpression
							If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_Boolean) Then
								cConst = cConst1
								Return cConst
							Else
								cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(CShort((-DirectCast(cConst1, CConst(Of Boolean)).Value)))
								Return cConst
							End If
						Case SyntaxKind.UnaryMinusExpression
							If (Not ExpressionEvaluator.IsNothing(cConst1)) Then
								Select Case specialType
									Case Microsoft.CodeAnalysis.SpecialType.System_Boolean
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(CShort((-CShort((-DirectCast(cConst1, CConst(Of Boolean)).Value)))))
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Char
										Throw ExceptionUtilities.UnexpectedValue(specialType)
									Case Microsoft.CodeAnalysis.SpecialType.System_SByte
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(CSByte((-DirectCast(cConst1, CConst(Of SByte)).Value)))
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Byte
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(CShort((-DirectCast(cConst1, CConst(Of Byte)).Value)))
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Int16
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(CShort((-DirectCast(cConst1, CConst(Of Short)).Value)))
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_UInt16
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(CInt((-DirectCast(cConst1, CConst(Of UShort)).Value)))
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Int32
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(-DirectCast(cConst1, CConst(Of Integer)).Value)
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_UInt32
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(CLng((-CULng(DirectCast(cConst1, CConst(Of UInteger)).Value))))
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Int64
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(-DirectCast(cConst1, CConst(Of Long)).Value)
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_UInt64
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create([Decimal].Negate(New [Decimal](DirectCast(cConst1, CConst(Of ULong)).Value)))
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create([Decimal].Negate(DirectCast(cConst1, CConst(Of [Decimal])).Value))
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Single
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(-DirectCast(cConst1, CConst(Of Single)).Value)
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Double
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(-DirectCast(cConst1, CConst(Of Double)).Value)
										Return cConst
									Case Else
										Throw ExceptionUtilities.UnexpectedValue(specialType)
								End Select
							Else
								cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(0)
								Return cConst
							End If

						Case SyntaxKind.NotExpression
							If (Not ExpressionEvaluator.IsNothing(cConst1)) Then
								Select Case specialType
									Case Microsoft.CodeAnalysis.SpecialType.System_Boolean
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Not DirectCast(cConst1, CConst(Of Boolean)).Value)
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Char
										Throw ExceptionUtilities.UnexpectedValue(specialType)
									Case Microsoft.CodeAnalysis.SpecialType.System_SByte
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Not DirectCast(cConst1, CConst(Of SByte)).Value)
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Byte
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(CByte((Not DirectCast(cConst1, CConst(Of Byte)).Value)))
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Int16
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Not DirectCast(cConst1, CConst(Of Short)).Value)
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_UInt16
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(CUShort((Not DirectCast(cConst1, CConst(Of UShort)).Value)))
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Int32
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Not DirectCast(cConst1, CConst(Of Integer)).Value)
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_UInt32
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Not DirectCast(cConst1, CConst(Of UInteger)).Value)
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Int64
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Not DirectCast(cConst1, CConst(Of Long)).Value)
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_UInt64
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Not DirectCast(cConst1, CConst(Of ULong)).Value)
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Not Convert.ToInt64(DirectCast(cConst1, CConst(Of [Decimal])).Value))
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Single
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Not CLng(Math.Round(CDbl(DirectCast(cConst1, CConst(Of Single)).Value))))
										Return cConst
									Case Microsoft.CodeAnalysis.SpecialType.System_Double
										cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Not CLng(Math.Round(DirectCast(cConst1, CConst(Of Double)).Value)))
										Return cConst
									Case Else
										Throw ExceptionUtilities.UnexpectedValue(specialType)
								End Select
							Else
								cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(-1)
								Return cConst
							End If

					End Select
				Catch overflowException As System.OverflowException
					ProjectData.SetProjectError(overflowException)
					cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_ExpressionOverflow1, expr)
					ProjectData.ClearProjectError()
					Return cConst
				End Try
				Throw ExceptionUtilities.UnexpectedValue(expr)
			End If
			Return cConst
		End Function

		Private Shared Function GetDisplayString(ByVal typeChar As TypeCharacter) As String
			Dim str As String
			Select Case typeChar
				Case TypeCharacter.[Integer]
					str = "%"
					Exit Select
				Case TypeCharacter.[Long]
					str = "&"
					Exit Select
				Case TypeCharacter.[Decimal]
					str = "@"
					Exit Select
				Case TypeCharacter.[Single]
					str = "!"
					Exit Select
				Case TypeCharacter.[Double]
					str = "#"
					Exit Select
				Case TypeCharacter.[String]
					str = "$"
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(typeChar)
			End Select
			Return str
		End Function

		Private Shared Function GetSpecialType(ByVal predefinedType As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax) As Microsoft.CodeAnalysis.SpecialType
			Dim specialType As Microsoft.CodeAnalysis.SpecialType
			Dim kind As SyntaxKind = predefinedType.Keyword.Kind
			If (kind > SyntaxKind.IntegerKeyword) Then
				If (kind <= SyntaxKind.ShortKeyword) Then
					If (kind <= SyntaxKind.ObjectKeyword) Then
						If (kind <> SyntaxKind.LongKeyword) Then
							GoTo Label4
						End If
						specialType = Microsoft.CodeAnalysis.SpecialType.System_Int64
						Return specialType
					ElseIf (kind = SyntaxKind.SByteKeyword) Then
						specialType = Microsoft.CodeAnalysis.SpecialType.System_SByte
						Return specialType
					Else
						If (kind <> SyntaxKind.ShortKeyword) Then
							Throw ExceptionUtilities.UnexpectedValue(kind)
						End If
						specialType = Microsoft.CodeAnalysis.SpecialType.System_Int16
						Return specialType
					End If
				ElseIf (kind > SyntaxKind.StringKeyword) Then
					Select Case kind
						Case SyntaxKind.UIntegerKeyword
							specialType = Microsoft.CodeAnalysis.SpecialType.System_UInt32
							Return specialType
						Case SyntaxKind.ULongKeyword
							specialType = Microsoft.CodeAnalysis.SpecialType.System_UInt64
							Return specialType
						Case SyntaxKind.UShortKeyword
							specialType = Microsoft.CodeAnalysis.SpecialType.System_UInt16
							Return specialType
						Case Else
							If (kind = SyntaxKind.VariantKeyword) Then
								Exit Select
							End If
							Throw ExceptionUtilities.UnexpectedValue(kind)
					End Select
				ElseIf (kind = SyntaxKind.SingleKeyword) Then
					specialType = Microsoft.CodeAnalysis.SpecialType.System_Single
					Return specialType
				Else
					If (kind <> SyntaxKind.StringKeyword) Then
						Throw ExceptionUtilities.UnexpectedValue(kind)
					End If
					specialType = Microsoft.CodeAnalysis.SpecialType.System_String
					Return specialType
				End If
			Label8:
				specialType = Microsoft.CodeAnalysis.SpecialType.System_Object
			ElseIf (kind <= SyntaxKind.CharKeyword) Then
				If (kind = SyntaxKind.BooleanKeyword) Then
					specialType = Microsoft.CodeAnalysis.SpecialType.System_Boolean
				ElseIf (kind = SyntaxKind.ByteKeyword) Then
					specialType = Microsoft.CodeAnalysis.SpecialType.System_Byte
				Else
					If (kind <> SyntaxKind.CharKeyword) Then
						Throw ExceptionUtilities.UnexpectedValue(kind)
					End If
					specialType = Microsoft.CodeAnalysis.SpecialType.System_Char
				End If
			ElseIf (kind <= SyntaxKind.DecimalKeyword) Then
				If (kind = SyntaxKind.DateKeyword) Then
					specialType = Microsoft.CodeAnalysis.SpecialType.System_DateTime
				Else
					If (kind <> SyntaxKind.DecimalKeyword) Then
						Throw ExceptionUtilities.UnexpectedValue(kind)
					End If
					specialType = Microsoft.CodeAnalysis.SpecialType.System_Decimal
				End If
			ElseIf (kind = SyntaxKind.DoubleKeyword) Then
				specialType = Microsoft.CodeAnalysis.SpecialType.System_Double
			Else
				If (kind <> SyntaxKind.IntegerKeyword) Then
					Throw ExceptionUtilities.UnexpectedValue(kind)
				End If
				specialType = Microsoft.CodeAnalysis.SpecialType.System_Int32
			End If
			Return specialType
		Label4:
			If (kind = SyntaxKind.ObjectKeyword) Then
				GoTo Label8
			End If
			Throw ExceptionUtilities.UnexpectedValue(kind)
		End Function

		Private Shared Function IsNothing(ByVal val As CConst) As Boolean
			If (val.SpecialType <> SpecialType.System_Object) Then
				Return False
			End If
			Return CObj(val.ValueAsObject) = CObj(Nothing)
		End Function

		Private Shared Function PerformCompileTimeBinaryOperation(ByVal opcode As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal resultType As SpecialType, ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst, ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim num As [Decimal] = New Decimal()
			If (left.SpecialType.IsIntegralType() OrElse left.SpecialType = SpecialType.System_Char OrElse left.SpecialType = SpecialType.System_DateTime) Then
				Dim num1 As Long = TypeHelpers.UncheckedCLng(left)
				Dim shiftSizeMask As Long = TypeHelpers.UncheckedCLng(right)
				If (resultType <> SpecialType.System_Boolean) Then
					Dim num2 As Long = CLng(0)
					Dim flag As Boolean = False
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = opcode
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddExpression
							num2 = CompileTimeCalculations.NarrowIntegralResult(num1 + shiftSizeMask, left.SpecialType, resultType, flag)
							If (resultType.IsUnsignedIntegralType()) Then
								If (CompileTimeCalculations.UncheckedCULng(num2) >= CompileTimeCalculations.UncheckedCULng(num1)) Then
									Exit Select
								End If
								flag = True
								Exit Select
							Else
								If ((shiftSizeMask <= CLng(0) OrElse num2 >= num1) AndAlso (shiftSizeMask >= CLng(0) OrElse num2 <= num1)) Then
									Exit Select
								End If
								flag = True
								Exit Select
							End If
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubtractExpression
							num2 = CompileTimeCalculations.NarrowIntegralResult(num1 - shiftSizeMask, left.SpecialType, resultType, flag)
							If (resultType.IsUnsignedIntegralType()) Then
								If (CompileTimeCalculations.UncheckedCULng(num2) <= CompileTimeCalculations.UncheckedCULng(num1)) Then
									Exit Select
								End If
								flag = True
								Exit Select
							Else
								If ((shiftSizeMask <= CLng(0) OrElse num2 <= num1) AndAlso (shiftSizeMask >= CLng(0) OrElse num2 >= num1)) Then
									Exit Select
								End If
								flag = True
								Exit Select
							End If
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiplyExpression
							num2 = CompileTimeCalculations.Multiply(num1, shiftSizeMask, left.SpecialType, resultType, flag)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DivideExpression
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DirectCastExpression
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParenthesizedExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DirectCastExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastExpression
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExponentiateExpression
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConcatenateExpression
							Throw ExceptionUtilities.UnexpectedValue(opcode)
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerDivideExpression
							If (shiftSizeMask <> 0) Then
								num2 = CompileTimeCalculations.NarrowIntegralResult(If(resultType.IsUnsignedIntegralType(), CompileTimeCalculations.UncheckedCLng(CompileTimeCalculations.UncheckedCULng(num1) / CompileTimeCalculations.UncheckedCULng(shiftSizeMask)), CompileTimeCalculations.UncheckedIntegralDiv(num1, shiftSizeMask)), left.SpecialType, resultType, flag)
								If (resultType.IsUnsignedIntegralType() OrElse num1 <> -9223372036854775808L OrElse shiftSizeMask <> CLng(-1)) Then
									Exit Select
								End If
								flag = True
								Exit Select
							Else
								cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_ZeroDivide, expr)
								Return cConst
							End If
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LeftShiftExpression
							shiftSizeMask = shiftSizeMask And CLng(left.SpecialType.GetShiftSizeMask())
							num2 = num1 << (CInt(shiftSizeMask) And 63)
							Dim flag1 As Boolean = False
							num2 = CompileTimeCalculations.NarrowIntegralResult(num2, left.SpecialType, resultType, flag1)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftExpression
							shiftSizeMask = shiftSizeMask And CLng(left.SpecialType.GetShiftSizeMask())
							If (Not resultType.IsUnsignedIntegralType()) Then
								num2 = num1 >> (CInt(shiftSizeMask) And 63)
								Exit Select
							Else
								num2 = CompileTimeCalculations.UncheckedCLng(CompileTimeCalculations.UncheckedCULng(num1) >> (CInt(shiftSizeMask) And 63))
								Exit Select
							End If
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuloExpression
							If (shiftSizeMask = 0) Then
								cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_ZeroDivide, expr)
								Return cConst
							ElseIf (resultType.IsUnsignedIntegralType()) Then
								num2 = CompileTimeCalculations.UncheckedCLng(CompileTimeCalculations.UncheckedCULng(num1) Mod CompileTimeCalculations.UncheckedCULng(shiftSizeMask))
								Exit Select
							ElseIf (shiftSizeMask = CLng(-1)) Then
								num2 = CLng(0)
								Exit Select
							Else
								num2 = num1 Mod shiftSizeMask
								Exit Select
							End If
						Case Else
							Select Case syntaxKind
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrExpression
									num2 = num1 Or shiftSizeMask

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclusiveOrExpression
									num2 = num1 Xor shiftSizeMask

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndExpression
									num2 = num1 And shiftSizeMask

								Case Else
									Throw ExceptionUtilities.UnexpectedValue(opcode)
							End Select

					End Select
					If (Not flag) Then
						cConst = ExpressionEvaluator.Convert(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(num2), resultType, expr)
					Else
						cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_ExpressionOverflow1, expr, New [Object]() { resultType.GetDisplayName() })
					End If
				Else
					Dim flag2 As Boolean = False
					Select Case opcode
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsExpression
							flag2 = If(left.SpecialType.IsUnsignedIntegralType(), CompileTimeCalculations.UncheckedCULng(num1) = CompileTimeCalculations.UncheckedCULng(shiftSizeMask), num1 = shiftSizeMask)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression
							flag2 = If(left.SpecialType.IsUnsignedIntegralType(), CompileTimeCalculations.UncheckedCULng(num1) <> CompileTimeCalculations.UncheckedCULng(shiftSizeMask), num1 <> shiftSizeMask)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExpression
							flag2 = If(left.SpecialType.IsUnsignedIntegralType(), CompileTimeCalculations.UncheckedCULng(num1) < CompileTimeCalculations.UncheckedCULng(shiftSizeMask), num1 < shiftSizeMask)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanOrEqualExpression
							flag2 = If(left.SpecialType.IsUnsignedIntegralType(), CompileTimeCalculations.UncheckedCULng(num1) <= CompileTimeCalculations.UncheckedCULng(shiftSizeMask), num1 <= shiftSizeMask)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanOrEqualExpression
							flag2 = If(left.SpecialType.IsUnsignedIntegralType(), CompileTimeCalculations.UncheckedCULng(num1) >= CompileTimeCalculations.UncheckedCULng(shiftSizeMask), num1 >= shiftSizeMask)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanExpression
							flag2 = If(left.SpecialType.IsUnsignedIntegralType(), CompileTimeCalculations.UncheckedCULng(num1) > CompileTimeCalculations.UncheckedCULng(shiftSizeMask), num1 > shiftSizeMask)
							Exit Select
						Case Else
							Throw ExceptionUtilities.UnexpectedValue(opcode)
					End Select
					cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(flag2)
				End If
			ElseIf (left.SpecialType.IsFloatingType()) Then
				Dim num3 As Double = Microsoft.VisualBasic.CompilerServices.Conversions.ToDouble(left.ValueAsObject)
				Dim num4 As Double = Microsoft.VisualBasic.CompilerServices.Conversions.ToDouble(right.ValueAsObject)
				If (resultType <> SpecialType.System_Boolean) Then
					Dim num5 As Double = 0
					Dim flag3 As Boolean = False
					Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = opcode
					Select Case syntaxKind1
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddExpression
							num5 = num3 + num4
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubtractExpression
							num5 = num3 - num4
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiplyExpression
							num5 = num3 * num4
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DivideExpression
							num5 = num3 / num4
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerDivideExpression
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DirectCastExpression
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParenthesizedExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DirectCastExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastExpression
							Throw ExceptionUtilities.UnexpectedValue(opcode)
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExponentiateExpression
							If (Not [Double].IsInfinity(num4)) Then
								If ([Double].IsNaN(num4)) Then
									num5 = NaN
									Exit Select
								End If
							ElseIf (num3 = 1) Then
								num5 = num3
								Exit Select
							ElseIf (num3 = -1) Then
								num5 = NaN
								Exit Select
							End If
							num5 = Math.Pow(num3, num4)
							Exit Select
						Case Else
							If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuloExpression) Then
								num5 = Math.IEEERemainder(num3, num4)
								Exit Select
							Else
								Throw ExceptionUtilities.UnexpectedValue(opcode)
							End If
					End Select
					num5 = CompileTimeCalculations.NarrowFloatingResult(num5, resultType, flag3)
					cConst = ExpressionEvaluator.Convert(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(num5), resultType, expr)
				Else
					Dim flag4 As Boolean = False
					Select Case opcode
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsExpression
							flag4 = num3 = num4
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression
							flag4 = num3 <> num4
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExpression
							flag4 = num3 < num4
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanOrEqualExpression
							flag4 = num3 <= num4
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanOrEqualExpression
							flag4 = num3 >= num4
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanExpression
							flag4 = num3 > num4
							Exit Select
						Case Else
							Throw ExceptionUtilities.UnexpectedValue(opcode)
					End Select
					cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(If(flag4, True, False))
				End If
			ElseIf (left.SpecialType = SpecialType.System_Decimal) Then
				Dim num6 As [Decimal] = Microsoft.VisualBasic.CompilerServices.Conversions.ToDecimal(left.ValueAsObject)
				Dim num7 As [Decimal] = Microsoft.VisualBasic.CompilerServices.Conversions.ToDecimal(right.ValueAsObject)
				If (resultType <> SpecialType.System_Boolean) Then
					Dim flag5 As Boolean = False
					Dim syntaxKind2 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = opcode
					Select Case syntaxKind2
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddExpression
							flag5 = TypeHelpers.VarDecAdd(num6, num7, num)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubtractExpression
							flag5 = TypeHelpers.VarDecSub(num6, num7, num)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiplyExpression
							flag5 = TypeHelpers.VarDecMul(num6, num7, num)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DivideExpression
							If ([Decimal].Compare(num7, [Decimal].Zero) <> 0) Then
								flag5 = TypeHelpers.VarDecDiv(num6, num7, num)
								Exit Select
							Else
								cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_ZeroDivide, expr)
								Return cConst
							End If
						Case Else
							If (syntaxKind2 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuloExpression) Then
								Throw ExceptionUtilities.UnexpectedValue(opcode)
							End If
							If ([Decimal].Compare(num7, [Decimal].Zero) <> 0) Then
								flag5 = TypeHelpers.VarDecDiv(num6, num7, num)
								If (flag5) Then
									Exit Select
								End If
								num = [Decimal].Truncate(num)
								flag5 = TypeHelpers.VarDecMul(num, num7, num)
								If (flag5) Then
									Exit Select
								End If
								flag5 = TypeHelpers.VarDecSub(num6, num, num)
								Exit Select
							Else
								cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_ZeroDivide, expr)
								Return cConst
							End If
					End Select
					If (Not flag5) Then
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(num)
					Else
						cConst = ExpressionEvaluator.ReportSemanticError(ERRID.ERR_ExpressionOverflow1, expr, New [Object]() { resultType.GetDisplayName() })
					End If
				Else
					Dim flag6 As Boolean = False
					Dim num8 As Integer = num6.CompareTo(num7)
					Select Case opcode
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsExpression
							flag6 = num8 = 0
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression
							flag6 = num8 <> 0
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExpression
							flag6 = num8 < 0
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanOrEqualExpression
							flag6 = num8 <= 0
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanOrEqualExpression
							flag6 = num8 >= 0
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanExpression
							flag6 = num8 > 0
							Exit Select
						Case Else
							Throw ExceptionUtilities.UnexpectedValue(opcode)
					End Select
					cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(flag6)
				End If
			ElseIf (left.SpecialType <> SpecialType.System_String) Then
				If (left.SpecialType <> SpecialType.System_Boolean) Then
					Throw ExceptionUtilities.Unreachable
				End If
				Dim flag7 As Boolean = Microsoft.VisualBasic.CompilerServices.Conversions.ToBoolean(left.ValueAsObject)
				Dim flag8 As Boolean = Microsoft.VisualBasic.CompilerServices.Conversions.ToBoolean(right.ValueAsObject)
				Dim flag9 As Boolean = False
				Select Case opcode
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsExpression
						flag9 = flag7 = flag8
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression
						flag9 = flag7 <> flag8
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExpression
						flag9 = If(Not flag7, False, Not flag8)
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanOrEqualExpression
						flag9 = If(flag7, True, Not flag8)
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanOrEqualExpression
						flag9 = If(Not flag7, True, flag8)
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanExpression
						flag9 = If(flag7, False, flag8)
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsExpression
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsNotExpression
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeExpression
						Throw ExceptionUtilities.UnexpectedValue(opcode)
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrExpression
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseExpression
						flag9 = flag7 Or flag8
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclusiveOrExpression
						flag9 = flag7 Xor flag8
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndExpression
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndAlsoExpression
						flag9 = flag7 And flag8
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(opcode)
				End Select
				cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(flag9)
			Else
				Dim str As String = If(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(left.ValueAsObject), "")
				Dim str1 As String = If(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(right.ValueAsObject), "")
				Dim syntaxKind3 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = opcode
				If (syntaxKind3 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConcatenateExpression) Then
					cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create([String].Concat(str, str1))
				Else
					If (CUShort(syntaxKind3) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsExpression) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement)) Then
						Throw ExceptionUtilities.UnexpectedValue(opcode)
					End If
					Dim flag10 As Boolean = False
					Dim num9 As Integer = StringComparer.Ordinal.Compare(str, str1)
					Select Case opcode
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsExpression
							flag10 = num9 = 0
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression
							flag10 = num9 <> 0
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExpression
							flag10 = num9 < 0
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanOrEqualExpression
							flag10 = num9 <= 0
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanOrEqualExpression
							flag10 = num9 >= 0
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanExpression
							flag10 = num9 > 0
							Exit Select
					End Select
					cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(flag10)
				End If
			End If
			Return cConst
		End Function

		Private Shared Function ReportSemanticError(ByVal id As ERRID, ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As BadCConst
			Return ExpressionEvaluator.ReportSemanticError(id, node, Array.Empty(Of Object)())
		End Function

		Private Shared Function ReportSemanticError(ByVal id As ERRID, ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal ParamArray args As Object()) As BadCConst
			Return New BadCConst(id, args)
		End Function

		Private Shared Function TypeCodeToDominantTypeIndex(ByVal specialType As Microsoft.CodeAnalysis.SpecialType) As Integer
			Dim num As Integer
			Dim specialType1 As Microsoft.CodeAnalysis.SpecialType = specialType
			Select Case specialType1
				Case Microsoft.CodeAnalysis.SpecialType.System_Object
					num = 15
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Enum
				Case Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate
				Case Microsoft.CodeAnalysis.SpecialType.System_Delegate
				Case Microsoft.CodeAnalysis.SpecialType.System_ValueType
				Case Microsoft.CodeAnalysis.SpecialType.System_Void
					Throw ExceptionUtilities.UnexpectedValue(specialType)
				Case Microsoft.CodeAnalysis.SpecialType.System_Boolean
					num = 13
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Char
					num = 12
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_SByte
					num = 1
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Byte
					num = 0
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Int16
					num = 2
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_UInt16
					num = 3
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Int32
					num = 4
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_UInt32
					num = 5
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Int64
					num = 6
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_UInt64
					num = 7
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
					num = 10
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Single
					num = 8
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Double
					num = 9
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_String
					num = 14
					Exit Select
				Case Else
					If (specialType1 = Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
						num = 11
						Exit Select
					Else
						Throw ExceptionUtilities.UnexpectedValue(specialType)
					End If
			End Select
			Return num
		End Function
	End Structure
End Namespace