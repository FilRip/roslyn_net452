Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module BoundExpressionExtensions
		<Conditional("DEBUG")>
		<Extension>
		Public Sub AssertRValue(ByVal node As BoundExpression)
			Dim hasErrors As Boolean = node.HasErrors
		End Sub

		<Extension>
		Public Function GetAccessKind(ByVal node As BoundExpression) As PropertyAccessKind
			Dim accessKind As PropertyAccessKind
			Dim kind As BoundKind = node.Kind
			If (kind = BoundKind.PropertyAccess) Then
				accessKind = DirectCast(node, BoundPropertyAccess).AccessKind
			Else
				If (kind <> BoundKind.XmlMemberAccess) Then
					Throw ExceptionUtilities.UnexpectedValue(node.Kind)
				End If
				accessKind = DirectCast(node, BoundXmlMemberAccess).MemberAccess.GetAccessKind()
			End If
			Return accessKind
		End Function

		<Extension>
		Public Sub GetExpressionSymbols(ByVal methodGroup As BoundMethodGroup, ByVal symbols As ArrayBuilder(Of Symbol))
			Dim length As Integer = 0
			If (methodGroup.TypeArgumentsOpt IsNot Nothing) Then
				length = methodGroup.TypeArgumentsOpt.Arguments.Length
			End If
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).Enumerator = methodGroup.Methods.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = enumerator.Current
				If (length <> 0) Then
					If (length <> current.Arity) Then
						Continue While
					End If
					symbols.Add(current.Construct(methodGroup.TypeArgumentsOpt.Arguments))
				Else
					symbols.Add(current)
				End If
			End While
			Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
			Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).Enumerator = methodGroup.AdditionalExtensionMethods(discarded).GetEnumerator()
			While enumerator1.MoveNext()
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = enumerator1.Current
				If (length <> 0) Then
					If (length <> methodSymbol.Arity) Then
						Continue While
					End If
					symbols.Add(methodSymbol.Construct(methodGroup.TypeArgumentsOpt.Arguments))
				Else
					symbols.Add(methodSymbol)
				End If
			End While
		End Sub

		<Extension>
		Public Sub GetExpressionSymbols(ByVal node As BoundExpression, ByVal symbols As ArrayBuilder(Of Symbol))
			Dim kind As BoundKind = node.Kind
			If (kind > BoundKind.MethodGroup) Then
				If (kind = BoundKind.PropertyGroup) Then
					symbols.AddRange(Of PropertySymbol)(DirectCast(node, BoundPropertyGroup).Properties)
					Return
				End If
				Select Case kind
					Case BoundKind.QuerySource
						DirectCast(node, BoundQuerySource).Expression.GetExpressionSymbols(symbols)
						Return
					Case BoundKind.ToQueryableCollectionConversion
						DirectCast(node, BoundToQueryableCollectionConversion).ConversionCall.GetExpressionSymbols(symbols)
						Return
					Case BoundKind.QueryableSource
						DirectCast(node, BoundQueryableSource).Source.GetExpressionSymbols(symbols)
						Return
					Case BoundKind.QueryClause
						DirectCast(node, BoundQueryClause).UnderlyingExpression.GetExpressionSymbols(symbols)
						Return
					Case BoundKind.Ordering
						DirectCast(node, BoundOrdering).UnderlyingExpression.GetExpressionSymbols(symbols)
						Return
					Case BoundKind.AggregateClause
						DirectCast(node, BoundAggregateClause).UnderlyingExpression.GetExpressionSymbols(symbols)
						Return
				End Select
			Else
				If (kind = BoundKind.BadExpression) Then
					symbols.AddRange(DirectCast(node, BoundBadExpression).Symbols)
					Return
				End If
				If (kind = BoundKind.MethodGroup) Then
					DirectCast(node, BoundMethodGroup).GetExpressionSymbols(symbols)
					Return
				End If
			End If
			Dim expressionSymbol As Symbol = node.ExpressionSymbol
			If (expressionSymbol IsNot Nothing) Then
				If (expressionSymbol.Kind = SymbolKind.[Namespace] AndAlso CInt(DirectCast(expressionSymbol, NamespaceSymbol).NamespaceKind) = 0) Then
					symbols.AddRange(Of NamespaceSymbol)(DirectCast(expressionSymbol, NamespaceSymbol).ConstituentNamespaces)
					Return
				End If
				symbols.Add(expressionSymbol)
			End If
		End Sub

		<Extension>
		Public Function GetIntegerConstantValue(ByVal expression As BoundExpression) As Nullable(Of Integer)
			Dim nullable As Nullable(Of Integer)
			If (Not expression.HasErrors AndAlso expression.IsConstant) Then
				Select Case expression.Type.SpecialType
					Case SpecialType.System_Int16
						nullable = New Nullable(Of Integer)(expression.ConstantValueOpt.Int16Value)
						Return nullable
					Case SpecialType.System_Int32
						nullable = New Nullable(Of Integer)(expression.ConstantValueOpt.Int32Value)
						Return nullable
					Case SpecialType.System_Int64
						If (expression.ConstantValueOpt.Int64Value > CLng(2147483647) OrElse expression.ConstantValueOpt.Int64Value < CLng(-2147483648)) Then
							nullable = Nothing
							Return nullable
						Else
							nullable = New Nullable(Of Integer)(CInt(expression.ConstantValueOpt.Int64Value))
							Return nullable
						End If
				End Select
			End If
			nullable = Nothing
			Return nullable
		End Function

		<Extension>
		Public Function GetLateBoundAccessKind(ByVal node As BoundExpression) As LateBoundAccessKind
			Dim accessKind As LateBoundAccessKind
			Dim kind As BoundKind = node.Kind
			If (kind = BoundKind.LateMemberAccess) Then
				accessKind = DirectCast(node, BoundLateMemberAccess).AccessKind
			Else
				If (kind <> BoundKind.LateInvocation) Then
					Throw ExceptionUtilities.UnexpectedValue(node.Kind)
				End If
				accessKind = DirectCast(node, BoundLateInvocation).AccessKind
			End If
			Return accessKind
		End Function

		<Extension>
		Public Function GetMostEnclosedParenthesizedExpression(ByVal expression As BoundExpression) As BoundExpression
			While expression.Kind = BoundKind.Parenthesized
				expression = DirectCast(expression, BoundParenthesized).Expression
			End While
			Return expression
		End Function

		<Extension>
		Public Function GetPropertyOrXmlProperty(ByVal node As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol
			Dim kind As BoundKind = node.Kind
			If (kind = BoundKind.PropertyAccess) Then
				propertySymbol = DirectCast(node, BoundPropertyAccess).PropertySymbol
			ElseIf (kind <> BoundKind.XmlMemberAccess) Then
				propertySymbol = Nothing
			Else
				propertySymbol = DirectCast(node, BoundXmlMemberAccess).MemberAccess.GetPropertyOrXmlProperty()
			End If
			Return propertySymbol
		End Function

		<Extension>
		Public Function GetTypeOfAssignmentTarget(ByVal node As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			typeSymbol = If(node.Kind <> BoundKind.PropertyAccess, node.Type, DirectCast(node, BoundPropertyAccess).PropertySymbol.GetTypeFromSetMethod())
			Return typeSymbol
		End Function

		<Extension>
		Public Function HasExpressionSymbols(ByVal node As BoundExpression) As Boolean
			Dim length As Boolean
			Dim kind As BoundKind = node.Kind
			If (kind <= BoundKind.PropertyGroup) Then
				If (kind > BoundKind.TypeExpression) Then
					If (kind = BoundKind.NamespaceExpression OrElse kind = BoundKind.Conversion OrElse CByte(kind) - CByte(BoundKind.MethodGroup) <= CByte(BoundKind.OmittedArgument)) Then
						length = True
						Return length
					End If
					length = False
					Return length
				Else
					If (kind <> BoundKind.BadExpression) Then
						If (kind = BoundKind.TypeExpression) Then
							length = True
							Return length
						End If
						length = False
						Return length
					End If
					length = DirectCast(node, BoundBadExpression).Symbols.Length > 0
					Return length
				End If
			ElseIf (kind <= BoundKind.ObjectCreationExpression) Then
				If (kind = BoundKind.[Call] OrElse kind = BoundKind.ObjectCreationExpression) Then
					length = True
					Return length
				End If
				length = False
				Return length
			ElseIf (CByte(kind) - CByte(BoundKind.FieldAccess) > CByte(BoundKind.LValueToRValueWrapper) AndAlso kind <> BoundKind.Local AndAlso kind <> BoundKind.RangeVariable) Then
				length = False
				Return length
			End If
			length = True
			Return length
		End Function

		<Extension>
		Public Function IsDefaultValue(ByVal node As BoundExpression) As Boolean
			Dim flag As Boolean
			Dim constantValueOpt As ConstantValue = node.ConstantValueOpt
			If (constantValueOpt Is Nothing OrElse Not constantValueOpt.IsDefaultValue) Then
				Dim kind As BoundKind = node.Kind
				If (kind > BoundKind.[DirectCast]) Then
					If (kind = BoundKind.[TryCast]) Then
						constantValueOpt = DirectCast(node, BoundTryCast).Operand.ConstantValueOpt
						flag = If(constantValueOpt Is Nothing, False, constantValueOpt.IsNothing)
						Return flag
					Else
						If (kind <> BoundKind.ObjectCreationExpression) Then
							GoTo Label2
						End If
						Dim constructorOpt As MethodSymbol = DirectCast(node, BoundObjectCreationExpression).ConstructorOpt
						flag = If(constructorOpt Is Nothing, True, constructorOpt.IsDefaultValueTypeConstructor())
						Return flag
					End If
				ElseIf (kind = BoundKind.Conversion) Then
					constantValueOpt = DirectCast(node, BoundConversion).Operand.ConstantValueOpt
					flag = If(constantValueOpt Is Nothing, False, constantValueOpt.IsNothing)
					Return flag
				ElseIf (kind = BoundKind.[DirectCast]) Then
					If (Not node.Type.IsTypeParameter() AndAlso node.Type.IsValueType) Then
						GoTo Label2
					End If
					constantValueOpt = DirectCast(node, BoundDirectCast).Operand.ConstantValueOpt
					flag = If(constantValueOpt Is Nothing, False, constantValueOpt.IsNothing)
					Return flag
				End If
			Label2:
				flag = False
			Else
				flag = True
			End If
			Return flag
		End Function

		<Extension>
		Public Function IsDefaultValueConstant(ByVal expr As BoundExpression) As Boolean
			Dim constantValueOpt As ConstantValue = expr.ConstantValueOpt
			If (constantValueOpt Is Nothing) Then
				Return False
			End If
			Return constantValueOpt.IsDefaultValue
		End Function

		<Extension>
		Public Function IsFalseConstant(ByVal expr As BoundExpression) As Boolean
			Return CObj(expr.ConstantValueOpt) = CObj(ConstantValue.[False])
		End Function

		<Extension>
		Public Function IsInstanceReference(ByVal node As BoundExpression) As Boolean
			If (node.IsMeReference() OrElse node.IsMyBaseReference()) Then
				Return True
			End If
			Return node.IsMyClassReference()
		End Function

		<Extension>
		Public Function IsIntegerZeroLiteral(ByVal node As BoundExpression) As Boolean
			While node.Kind = BoundKind.Parenthesized
				node = DirectCast(node, BoundParenthesized).Expression
			End While
			If (node.Kind <> BoundKind.Literal) Then
				Return False
			End If
			Return DirectCast(node, BoundLiteral).IsIntegerZeroLiteral()
		End Function

		<Extension>
		Public Function IsIntegerZeroLiteral(ByVal node As BoundLiteral) As Boolean
			Dim flag As Boolean
			flag = If(node.Value.Discriminator <> ConstantValueTypeDiscriminator.Int32 OrElse node.Type.SpecialType <> SpecialType.System_Int32, False, node.Value.Int32Value = 0)
			Return flag
		End Function

		<Extension>
		Public Function IsLateBound(ByVal node As BoundExpression) As Boolean
			Dim flag As Boolean
			flag = If(CByte(node.Kind) - CByte(BoundKind.LateMemberAccess) > CByte(BoundKind.OmittedArgument), False, True)
			Return flag
		End Function

		<Extension>
		Public Function IsMeReference(ByVal node As BoundExpression) As Boolean
			Return node.Kind = BoundKind.MeReference
		End Function

		<Extension>
		Public Function IsMyBaseReference(ByVal node As BoundExpression) As Boolean
			Return node.Kind = BoundKind.MyBaseReference
		End Function

		<Extension>
		Public Function IsMyClassReference(ByVal node As BoundExpression) As Boolean
			Return node.Kind = BoundKind.MyClassReference
		End Function

		<Extension>
		Public Function IsNegativeIntegerConstant(ByVal expression As BoundExpression) As Boolean
			Dim nullable As Nullable(Of Boolean)
			Dim nullable1 As Nullable(Of Boolean)
			Dim integerConstantValue As Nullable(Of Integer) = expression.GetIntegerConstantValue()
			If (integerConstantValue.HasValue) Then
				nullable1 = New Nullable(Of Boolean)(integerConstantValue.GetValueOrDefault() < 0)
			Else
				nullable = Nothing
				nullable1 = nullable
			End If
			nullable = nullable1
			Return If(Not nullable.GetValueOrDefault(), False, True)
		End Function

		<Extension>
		Public Function IsNothingLiteral(ByVal node As BoundExpression) As Boolean
			Dim flag As Boolean
			Dim type As TypeSymbol = node.Type
			If (type Is Nothing OrElse type.SpecialType = SpecialType.System_Object) Then
				Dim constantValueOpt As ConstantValue = node.ConstantValueOpt
				If (constantValueOpt Is Nothing OrElse Not constantValueOpt.IsNothing) Then
					flag = False
					Return flag
				End If
				flag = True
				Return flag
			End If
			flag = False
			Return flag
		End Function

		<Extension>
		Public Function IsNothingLiteral(ByVal node As BoundLiteral) As Boolean
			Dim flag As Boolean
			flag = If(Not node.Value.IsNothing, False, node.Type Is Nothing)
			Return flag
		End Function

		<Extension>
		Public Function IsPropertyOrXmlPropertyAccess(ByVal node As BoundExpression) As Boolean
			Dim flag As Boolean
			Dim kind As BoundKind = node.Kind
			If (kind = BoundKind.PropertyAccess) Then
				flag = True
			Else
				flag = If(kind <> BoundKind.XmlMemberAccess, False, DirectCast(node, BoundXmlMemberAccess).MemberAccess.IsPropertyOrXmlPropertyAccess())
			End If
			Return flag
		End Function

		<Extension>
		Public Function IsPropertyReturnsByRef(ByVal node As BoundExpression) As Boolean
			If (node.Kind <> BoundKind.PropertyAccess) Then
				Return False
			End If
			Return DirectCast(node, BoundPropertyAccess).PropertySymbol.ReturnsByRef
		End Function

		<Extension>
		Public Function IsPropertySupportingAssignment(ByVal node As BoundExpression) As Boolean
			Dim flag As Boolean
			Dim kind As BoundKind = node.Kind
			If (kind = BoundKind.PropertyAccess) Then
				Dim boundPropertyAccess As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess)
				flag = If(boundPropertyAccess.AccessKind <> PropertyAccessKind.[Get], boundPropertyAccess.IsWriteable, False)
			Else
				flag = If(kind <> BoundKind.XmlMemberAccess, False, DirectCast(node, BoundXmlMemberAccess).MemberAccess.IsPropertySupportingAssignment())
			End If
			Return flag
		End Function

		<Extension>
		Public Function IsStrictNothingLiteral(ByVal node As BoundExpression) As Boolean
			Dim flag As Boolean
			Dim kind As BoundKind
			If (node.IsNothingLiteral()) Then
				While True
					kind = node.Kind
					If (kind = BoundKind.Parenthesized) Then
						node = DirectCast(node, BoundParenthesized).Expression
					Else
						If (kind <> BoundKind.Conversion) Then
							GoTo Label0
						End If
						Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
						Dim constantValueOpt As ConstantValue = boundConversion.ConstantValueOpt
						If (boundConversion.ExplicitCastInCode OrElse constantValueOpt Is Nothing OrElse Not constantValueOpt.IsNothing) Then
							Exit While
						End If
						node = boundConversion.Operand
					End If
				End While
				flag = False
			Else
				flag = False
			End If
			Return flag
		Label0:
			If (kind <> BoundKind.Literal) Then
				flag = False
				Return flag
			Else
				flag = DirectCast(node, BoundLiteral).IsNothingLiteral()
				Return flag
			End If
		End Function

		<Extension>
		Public Function IsSupportingAssignment(ByVal node As BoundExpression) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean
			If (node Is Nothing) Then
				flag = False
			ElseIf (Not node.IsLValue) Then
				Dim kind As BoundKind = node.Kind
				If (kind = BoundKind.LateMemberAccess) Then
					Dim boundLateMemberAccess As Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess)
					flag = If(boundLateMemberAccess.AccessKind = LateBoundAccessKind.[Get], False, boundLateMemberAccess.AccessKind <> LateBoundAccessKind.[Call])
				ElseIf (kind = BoundKind.LateInvocation) Then
					Dim boundLateInvocation As Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation)
					If (boundLateInvocation.AccessKind = LateBoundAccessKind.Unknown) Then
						Dim methodOrPropertyGroupOpt As BoundMethodOrPropertyGroup = boundLateInvocation.MethodOrPropertyGroupOpt
						If (methodOrPropertyGroupOpt Is Nothing OrElse methodOrPropertyGroupOpt.Kind <> BoundKind.MethodGroup) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					End If
				Label1:
					flag1 = If(boundLateInvocation.AccessKind = LateBoundAccessKind.[Get], False, boundLateInvocation.AccessKind <> LateBoundAccessKind.[Call])
					flag = flag1
				Else
					flag = If(kind = BoundKind.LateBoundArgumentSupportingAssignmentWithCapture, True, node.IsPropertySupportingAssignment())
				End If
			Else
				flag = True
			End If
			Return flag
		End Function

		<Extension>
		Public Function IsTrueConstant(ByVal expr As BoundExpression) As Boolean
			Return CObj(expr.ConstantValueOpt) = CObj(ConstantValue.[True])
		End Function

		<Extension>
		Public Function IsValue(ByVal node As BoundExpression) As Boolean
			Dim type As Boolean
			Dim kind As BoundKind = node.Kind
			If (kind <= BoundKind.TypeExpression) Then
				If (kind > BoundKind.BadExpression) Then
					If (kind <> BoundKind.Parenthesized) Then
						If (kind = BoundKind.TypeExpression) Then
							type = False
							Return type
						End If
						type = True
						Return type
					End If
					type = DirectCast(node, BoundParenthesized).Expression.IsValue()
					Return type
				Else
					If (kind = BoundKind.TypeArguments) Then
						type = False
						Return type
					End If
					If (kind <> BoundKind.BadExpression) Then
						type = True
						Return type
					End If
					type = CObj(node.Type) <> CObj(Nothing)
					Return type
				End If
			ElseIf (kind > BoundKind.PropertyGroup) Then
				If (kind = BoundKind.ArrayInitialization OrElse kind = BoundKind.EventAccess OrElse kind = BoundKind.Label) Then
					type = False
					Return type
				End If
				type = True
				Return type
			Else
				If (kind = BoundKind.NamespaceExpression OrElse CByte(kind) - CByte(BoundKind.MethodGroup) <= CByte(BoundKind.OmittedArgument)) Then
					type = False
					Return type
				End If
				type = True
				Return type
			End If
			type = False
			Return type
		End Function

		<Extension>
		Public Function SetAccessKind(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal newAccessKind As PropertyAccessKind) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim kind As BoundKind = node.Kind
			If (kind = BoundKind.PropertyAccess) Then
				boundExpression = DirectCast(node, BoundPropertyAccess).SetAccessKind(newAccessKind)
			Else
				If (kind <> BoundKind.XmlMemberAccess) Then
					Throw ExceptionUtilities.UnexpectedValue(node.Kind)
				End If
				boundExpression = DirectCast(node, BoundXmlMemberAccess).SetAccessKind(newAccessKind)
			End If
			Return boundExpression
		End Function

		<Extension>
		Public Function SetAccessKind(ByVal node As BoundXmlMemberAccess, ByVal newAccessKind As PropertyAccessKind) As BoundXmlMemberAccess
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.MemberAccess.SetAccessKind(newAccessKind)
			Return node.Update(boundExpression)
		End Function

		<Extension>
		Public Function SetGetSetAccessKindIfAppropriate(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim kind As BoundKind = node.Kind
			If (kind <= BoundKind.LateInvocation) Then
				If (kind = BoundKind.LateMemberAccess) Then
					boundExpression = DirectCast(node, BoundLateMemberAccess).SetAccessKind(LateBoundAccessKind.[Get] Or LateBoundAccessKind.[Set])
				Else
					If (kind <> BoundKind.LateInvocation) Then
						boundExpression = node
						Return boundExpression
					End If
					boundExpression = DirectCast(node, BoundLateInvocation).SetAccessKind(LateBoundAccessKind.[Get] Or LateBoundAccessKind.[Set])
				End If
			ElseIf (kind = BoundKind.PropertyAccess) Then
				Dim boundPropertyAccess As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess)
				boundExpression = boundPropertyAccess.SetAccessKind(If(boundPropertyAccess.PropertySymbol.ReturnsByRef, PropertyAccessKind.[Get], PropertyAccessKind.[Get] Or PropertyAccessKind.[Set]))
			Else
				If (kind <> BoundKind.XmlMemberAccess) Then
					boundExpression = node
					Return boundExpression
				End If
				boundExpression = DirectCast(node, BoundXmlMemberAccess).SetAccessKind(PropertyAccessKind.[Get] Or PropertyAccessKind.[Set])
			End If
			Return boundExpression
		End Function

		<Extension>
		Public Function SetLateBoundAccessKind(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal newAccessKind As LateBoundAccessKind) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim kind As BoundKind = node.Kind
			If (kind = BoundKind.LateMemberAccess) Then
				boundExpression = DirectCast(node, BoundLateMemberAccess).SetAccessKind(newAccessKind)
			Else
				If (kind <> BoundKind.LateInvocation) Then
					Throw ExceptionUtilities.UnexpectedValue(node.Kind)
				End If
				boundExpression = DirectCast(node, BoundLateInvocation).SetAccessKind(newAccessKind)
			End If
			Return boundExpression
		End Function

		<Extension>
		Public Function ToStatement(ByVal node As BoundExpression) As BoundExpressionStatement
			Return New BoundExpressionStatement(node.Syntax, node, node.HasErrors)
		End Function

		<Extension>
		Friend Function TypeArguments(ByVal this As BoundMethodOrPropertyGroup) As BoundTypeArguments
			Dim typeArgumentsOpt As BoundTypeArguments
			Dim boundMethodGroup As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup = TryCast(this, Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup)
			If (boundMethodGroup Is Nothing) Then
				typeArgumentsOpt = Nothing
			Else
				typeArgumentsOpt = boundMethodGroup.TypeArgumentsOpt
			End If
			Return typeArgumentsOpt
		End Function

		<Extension>
		Public Function Update(ByVal node As BoundXmlMemberAccess, ByVal memberAccess As BoundExpression) As BoundXmlMemberAccess
			Return node.Update(memberAccess, memberAccess.Type)
		End Function
	End Module
End Namespace