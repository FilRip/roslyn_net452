Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class UseTwiceRewriter
		Private Sub New()
			MyBase.New()
		End Sub

		Private Shared Function CaptureInATemp(ByVal containingMember As Symbol, ByVal value As BoundExpression, ByVal type As TypeSymbol, ByVal temporaries As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal), ByRef referToTemp As BoundLocal) As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator
			Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(containingMember, type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
			temporaries.Add(synthesizedLocal)
			referToTemp = New BoundLocal(value.Syntax, synthesizedLocal, type)
			referToTemp.SetWasCompilerGenerated()
			Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = (New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(value.Syntax, referToTemp, value, True, type, False)).MakeCompilerGenerated()
			referToTemp = referToTemp.MakeRValue()
			Return boundAssignmentOperator
		End Function

		Private Shared Function CaptureInATemp(ByVal containingMember As Symbol, ByVal value As BoundExpression, ByVal temporaries As ArrayBuilder(Of SynthesizedLocal), ByRef referToTemp As BoundLocal) As BoundAssignmentOperator
			Return UseTwiceRewriter.CaptureInATemp(containingMember, value, value.Type, temporaries, referToTemp)
		End Function

		Private Shared Function IsInvariantArray(ByVal type As TypeSymbol) As Boolean
			Dim nullable As Nullable(Of Boolean)
			Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = TryCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
			If (arrayTypeSymbol IsNot Nothing) Then
				nullable = New Nullable(Of Boolean)(arrayTypeSymbol.ElementType.IsNotInheritable())
			Else
				nullable = Nothing
			End If
			Return nullable.GetValueOrDefault()
		End Function

		Public Shared Function UseTwice(ByVal containingMember As Symbol, ByVal value As BoundExpression, ByVal temporaries As ArrayBuilder(Of SynthesizedLocal)) As UseTwiceRewriter.Result
			Dim result As UseTwiceRewriter.Result
			Dim kind As BoundKind = value.Kind
			If (kind <= BoundKind.LateInvocation) Then
				If (kind = BoundKind.LateMemberAccess) Then
					result = UseTwiceRewriter.UseTwiceLateMember(containingMember, DirectCast(value, BoundLateMemberAccess), temporaries)
				Else
					If (kind <> BoundKind.LateInvocation) Then
						result = UseTwiceRewriter.UseTwiceExpression(containingMember, value, temporaries)
						Return result
					End If
					result = UseTwiceRewriter.UseTwiceLateInvocation(containingMember, DirectCast(value, BoundLateInvocation), temporaries)
				End If
			ElseIf (kind = BoundKind.PropertyAccess) Then
				result = UseTwiceRewriter.UseTwicePropertyAccess(containingMember, DirectCast(value, BoundPropertyAccess), temporaries)
			Else
				If (kind <> BoundKind.XmlMemberAccess) Then
					result = UseTwiceRewriter.UseTwiceExpression(containingMember, value, temporaries)
					Return result
				End If
				Dim boundXmlMemberAccess As Microsoft.CodeAnalysis.VisualBasic.BoundXmlMemberAccess = DirectCast(value, Microsoft.CodeAnalysis.VisualBasic.BoundXmlMemberAccess)
				Dim result1 As UseTwiceRewriter.Result = UseTwiceRewriter.UseTwice(containingMember, boundXmlMemberAccess.MemberAccess, temporaries)
				result = New UseTwiceRewriter.Result(boundXmlMemberAccess.Update(result1.First), boundXmlMemberAccess.Update(result1.Second))
			End If
			Return result
		End Function

		Private Shared Function UseTwiceArrayAccess(ByVal containingMember As Symbol, ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess, ByVal arg As ArrayBuilder(Of SynthesizedLocal)) As UseTwiceRewriter.Result
			Dim result As UseTwiceRewriter.Result
			If (Not UseTwiceRewriter.IsInvariantArray(node.Expression.Type)) Then
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
				Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = UseTwiceRewriter.CaptureInATemp(containingMember, node.Expression, arg, boundLocal)
				Dim length As Integer = node.Indices.Length
				Dim first(length - 1 + 1 - 1) As BoundExpression
				Dim second(length - 1 + 1 - 1) As BoundExpression
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					Dim indices As ImmutableArray(Of BoundExpression) = node.Indices
					Dim result1 As UseTwiceRewriter.Result = UseTwiceRewriter.UseTwiceRValue(containingMember, indices(num1), arg)
					first(num1) = result1.First
					second(num1) = result1.Second
					num1 = num1 + 1
				Loop While num1 <= num
				Dim boundArrayAccess As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess = node.Update(boundLocal, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(second), node.IsLValue, node.Type)
				Dim boundArrayAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess = node.Update(boundAssignmentOperator, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(first), node.IsLValue, node.Type)
				result = New UseTwiceRewriter.Result(boundArrayAccess1, boundArrayAccess)
			Else
				result = UseTwiceRewriter.UseTwiceLValue(containingMember, node, arg)
			End If
			Return result
		End Function

		Private Shared Function UseTwiceCall(ByVal containingMember As Symbol, ByVal node As BoundCall, ByVal arg As ArrayBuilder(Of SynthesizedLocal)) As UseTwiceRewriter.Result
			Return UseTwiceRewriter.UseTwiceLValue(containingMember, node, arg)
		End Function

		Private Shared Function UseTwiceExpression(ByVal containingMember As Symbol, ByVal value As BoundExpression, ByVal temporaries As ArrayBuilder(Of SynthesizedLocal)) As UseTwiceRewriter.Result
			Dim result As UseTwiceRewriter.Result
			Dim kind As BoundKind
			If (value.IsLValue) Then
				kind = value.Kind
				If (kind <= BoundKind.ArrayAccess) Then
					If (kind = BoundKind.WithLValueExpressionPlaceholder) Then
						result = New UseTwiceRewriter.Result(value, value)
						Return result
					End If
					If (kind <> BoundKind.ArrayAccess) Then
						GoTo Label4
					End If
					result = UseTwiceRewriter.UseTwiceArrayAccess(containingMember, DirectCast(value, BoundArrayAccess), temporaries)
					Return result
				ElseIf (kind = BoundKind.[Call]) Then
					result = UseTwiceRewriter.UseTwiceCall(containingMember, DirectCast(value, BoundCall), temporaries)
					Return result
				Else
					If (kind <> BoundKind.FieldAccess) Then
						GoTo Label2
					End If
					result = UseTwiceRewriter.UseTwiceFieldAccess(containingMember, DirectCast(value, BoundFieldAccess), temporaries)
					Return result
				End If
			Label4:
				result = UseTwiceRewriter.UseTwiceRValue(containingMember, value, temporaries)
			Else
				result = UseTwiceRewriter.UseTwiceRValue(containingMember, value, temporaries)
			End If
			Return result
		Label2:
			If (CByte(kind) - CByte(BoundKind.Local) <= CByte(BoundKind.LValueToRValueWrapper)) Then
				result = New UseTwiceRewriter.Result(value, value)
				Return result
			End If
			GoTo Label4
		End Function

		Private Shared Function UseTwiceFieldAccess(ByVal containingMember As Symbol, ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess, ByVal arg As ArrayBuilder(Of SynthesizedLocal)) As UseTwiceRewriter.Result
			Dim result As UseTwiceRewriter.Result
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = node.FieldSymbol
			If (Not fieldSymbol.IsShared OrElse node.ReceiverOpt Is Nothing) Then
				result = If(node.ReceiverOpt IsNot Nothing, UseTwiceRewriter.UseTwiceLValue(containingMember, node, arg), New UseTwiceRewriter.Result(node, node))
			Else
				Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = node.Update(Nothing, fieldSymbol, node.IsLValue, node.SuppressVirtualCalls, Nothing, node.Type)
				result = New UseTwiceRewriter.Result(node, boundFieldAccess)
			End If
			Return result
		End Function

		Private Shared Function UseTwiceLateBoundReceiver(ByVal containingMember As Symbol, ByVal receiverOpt As BoundExpression, ByVal temporaries As ArrayBuilder(Of SynthesizedLocal)) As UseTwiceRewriter.Result
			Dim result As UseTwiceRewriter.Result
			If (receiverOpt Is Nothing) Then
				result = New UseTwiceRewriter.Result(Nothing, Nothing)
			ElseIf (receiverOpt.IsLValue AndAlso receiverOpt.Type.IsReferenceType) Then
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
				Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = UseTwiceRewriter.CaptureInATemp(containingMember, receiverOpt.MakeRValue(), temporaries, boundLocal)
				boundLocal = boundLocal.Update(boundLocal.LocalSymbol, True, boundLocal.Type)
				result = New UseTwiceRewriter.Result(New BoundSequence(boundAssignmentOperator.Syntax, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundExpression)(boundAssignmentOperator), boundLocal, boundLocal.Type, False), boundLocal)
			ElseIf (receiverOpt.IsLValue OrElse receiverOpt.Type.IsReferenceType OrElse receiverOpt.Type.IsValueType) Then
				result = UseTwiceRewriter.UseTwiceExpression(containingMember, receiverOpt, temporaries)
			Else
				Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
				Dim boundAssignmentOperator1 As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = UseTwiceRewriter.CaptureInATemp(containingMember, receiverOpt.MakeRValue(), temporaries, boundLocal1)
				boundLocal1 = boundLocal1.Update(boundLocal1.LocalSymbol, True, boundLocal1.Type)
				result = New UseTwiceRewriter.Result(New BoundSequence(boundAssignmentOperator1.Syntax, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundExpression)(boundAssignmentOperator1), boundLocal1, boundLocal1.Type, False), boundLocal1)
			End If
			Return result
		End Function

		Private Shared Function UseTwiceLateInvocation(ByVal containingMember As Symbol, ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation, ByVal arg As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal)) As UseTwiceRewriter.Result
			Dim result As UseTwiceRewriter.Result
			Dim empty As ImmutableArray(Of BoundExpression)
			Dim boundExpressions As ImmutableArray(Of BoundExpression)
			result = If(node.Member.Kind <> BoundKind.LateMemberAccess, UseTwiceRewriter.UseTwiceLateBoundReceiver(containingMember, node.Member, arg), UseTwiceRewriter.UseTwiceLateMember(containingMember, DirectCast(node.Member, BoundLateMemberAccess), arg))
			If (Not node.ArgumentsOpt.IsEmpty) Then
				Dim length As Integer = node.ArgumentsOpt.Length
				Dim boundLateBoundArgumentSupportingAssignmentWithCapture(length - 1 + 1 - 1) As BoundExpression
				Dim boundLocal(length - 1 + 1 - 1) As BoundExpression
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					Dim item As BoundExpression = node.ArgumentsOpt(num1)
					If (item.IsSupportingAssignment()) Then
						Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(containingMember, item.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
						arg.Add(synthesizedLocal)
						boundLateBoundArgumentSupportingAssignmentWithCapture(num1) = New Microsoft.CodeAnalysis.VisualBasic.BoundLateBoundArgumentSupportingAssignmentWithCapture(item.Syntax, item, synthesizedLocal, item.Type, False)
						boundLocal(num1) = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(item.Syntax, synthesizedLocal, False, synthesizedLocal.Type)
					Else
						UseTwiceRewriter.UseTwiceRegularArgument(containingMember, item, arg, boundLateBoundArgumentSupportingAssignmentWithCapture(num1), boundLocal(num1))
					End If
					num1 = num1 + 1
				Loop While num1 <= num
				empty = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundLateBoundArgumentSupportingAssignmentWithCapture)
				boundExpressions = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundLocal)
			Else
				empty = ImmutableArray(Of BoundExpression).Empty
				boundExpressions = ImmutableArray(Of BoundExpression).Empty
			End If
			Dim boundLateInvocation As Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation = node.Update(result.First, empty, node.ArgumentNamesOpt, node.AccessKind, node.MethodOrPropertyGroupOpt, node.Type)
			Dim boundLateInvocation1 As Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation = node.Update(result.Second, boundExpressions, node.ArgumentNamesOpt, node.AccessKind, node.MethodOrPropertyGroupOpt, node.Type)
			Return New UseTwiceRewriter.Result(boundLateInvocation, boundLateInvocation1)
		End Function

		Private Shared Function UseTwiceLateMember(ByVal containingMember As Symbol, ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess, ByVal arg As ArrayBuilder(Of SynthesizedLocal)) As UseTwiceRewriter.Result
			Dim result As UseTwiceRewriter.Result = UseTwiceRewriter.UseTwiceLateBoundReceiver(containingMember, node.ReceiverOpt, arg)
			Dim boundLateMemberAccess As Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess = node.Update(node.NameOpt, node.ContainerTypeOpt, result.First, node.TypeArgumentsOpt, node.AccessKind, node.Type)
			Dim boundLateMemberAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess = node.Update(node.NameOpt, node.ContainerTypeOpt, result.Second, node.TypeArgumentsOpt, node.AccessKind, node.Type)
			Return New UseTwiceRewriter.Result(boundLateMemberAccess, boundLateMemberAccess1)
		End Function

		Private Shared Function UseTwiceLValue(ByVal containingMember As Symbol, ByVal lvalue As BoundExpression, ByVal temporaries As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal)) As UseTwiceRewriter.Result
			Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(containingMember, lvalue.Type, SynthesizedLocalKind.LoweringTemp, Nothing, True)
			Dim boundReferenceAssignment As Microsoft.CodeAnalysis.VisualBasic.BoundReferenceAssignment = (New Microsoft.CodeAnalysis.VisualBasic.BoundReferenceAssignment(lvalue.Syntax, (New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(lvalue.Syntax, synthesizedLocal, synthesizedLocal.Type)).MakeCompilerGenerated(), lvalue, True, lvalue.Type, False)).MakeCompilerGenerated()
			temporaries.Add(synthesizedLocal)
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = (New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(lvalue.Syntax, synthesizedLocal, True, lvalue.Type)).MakeCompilerGenerated()
			Return New UseTwiceRewriter.Result(boundReferenceAssignment, boundLocal)
		End Function

		Private Shared Sub UseTwiceParamArrayArgument(ByVal containingMember As Symbol, ByVal boundArray As BoundArrayCreation, ByVal arg As ArrayBuilder(Of SynthesizedLocal), ByRef first As BoundExpression, ByRef second As BoundExpression)
			Dim initializerOpt As BoundArrayInitialization = boundArray.InitializerOpt
			Dim length As Integer = initializerOpt.Initializers.Length
			Dim boundExpressionArray(length - 1 + 1 - 1) As BoundExpression
			Dim boundExpressionArray1(length - 1 + 1 - 1) As BoundExpression
			Dim num As Integer = length - 1
			Dim num1 As Integer = 0
			Do
				Dim initializers As ImmutableArray(Of BoundExpression) = initializerOpt.Initializers
				UseTwiceRewriter.UseTwiceRegularArgument(containingMember, initializers(num1), arg, boundExpressionArray(num1), boundExpressionArray1(num1))
				num1 = num1 + 1
			Loop While num1 <= num
			first = boundArray.Update(boundArray.IsParamArrayArgument, boundArray.Bounds, initializerOpt.Update(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundExpressionArray), initializerOpt.Type), Nothing, ConversionKind.DelegateRelaxationLevelNone, boundArray.Type)
			second = boundArray.Update(boundArray.IsParamArrayArgument, boundArray.Bounds, initializerOpt.Update(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundExpressionArray1), initializerOpt.Type), Nothing, ConversionKind.DelegateRelaxationLevelNone, boundArray.Type)
		End Sub

		Private Shared Function UseTwicePropertyAccess(ByVal containingMember As Symbol, ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess, ByVal arg As ArrayBuilder(Of SynthesizedLocal)) As UseTwiceRewriter.Result
			Dim result As UseTwiceRewriter.Result
			Dim empty As ImmutableArray(Of BoundExpression)
			Dim boundExpressions As ImmutableArray(Of BoundExpression)
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = node.PropertySymbol
			Dim receiverOpt As BoundExpression = node.ReceiverOpt
			If (receiverOpt Is Nothing) Then
				result = New UseTwiceRewriter.Result(Nothing, Nothing)
			ElseIf (node.PropertySymbol.IsShared) Then
				result = New UseTwiceRewriter.Result(receiverOpt, Nothing)
			ElseIf (receiverOpt.IsLValue AndAlso receiverOpt.Type.IsReferenceType AndAlso Not receiverOpt.Type.IsTypeParameter()) Then
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
				result = New UseTwiceRewriter.Result(UseTwiceRewriter.CaptureInATemp(containingMember, receiverOpt.MakeRValue(), arg, boundLocal), boundLocal)
			ElseIf (receiverOpt.IsLValue OrElse receiverOpt.Type.IsReferenceType OrElse receiverOpt.Type.IsValueType) Then
				result = UseTwiceRewriter.UseTwiceExpression(containingMember, receiverOpt, arg)
			Else
				Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
				Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = UseTwiceRewriter.CaptureInATemp(containingMember, receiverOpt.MakeRValue(), arg, boundLocal1)
				boundLocal1 = boundLocal1.Update(boundLocal1.LocalSymbol, True, boundLocal1.Type)
				result = New UseTwiceRewriter.Result(New BoundSequence(boundAssignmentOperator.Syntax, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundExpression)(boundAssignmentOperator), boundLocal1, boundLocal1.Type, False), boundLocal1)
			End If
			If (Not node.Arguments.IsEmpty) Then
				Dim length As Integer = node.Arguments.Length
				Dim boundExpressionArray(length - 1 + 1 - 1) As BoundExpression
				Dim boundExpressionArray1(length - 1 + 1 - 1) As BoundExpression
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					Dim item As BoundExpression = node.Arguments(num1)
					If (item.Kind <> BoundKind.ArrayCreation OrElse Not DirectCast(item, BoundArrayCreation).IsParamArrayArgument) Then
						UseTwiceRewriter.UseTwiceRegularArgument(containingMember, item, arg, boundExpressionArray(num1), boundExpressionArray1(num1))
					Else
						UseTwiceRewriter.UseTwiceParamArrayArgument(containingMember, DirectCast(item, BoundArrayCreation), arg, boundExpressionArray(num1), boundExpressionArray1(num1))
					End If
					num1 = num1 + 1
				Loop While num1 <= num
				empty = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundExpressionArray)
				boundExpressions = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundExpressionArray1)
			Else
				empty = ImmutableArray(Of BoundExpression).Empty
				boundExpressions = ImmutableArray(Of BoundExpression).Empty
			End If
			Dim boundPropertyAccess As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess = node.Update(propertySymbol, node.PropertyGroupOpt, node.AccessKind, node.IsWriteable, node.IsLValue, result.First, empty, node.DefaultArguments, node.Type)
			Dim boundPropertyAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess = node.Update(propertySymbol, node.PropertyGroupOpt, node.AccessKind, node.IsWriteable, node.IsLValue, result.Second, boundExpressions, node.DefaultArguments, node.Type)
			Return New UseTwiceRewriter.Result(boundPropertyAccess, boundPropertyAccess1)
		End Function

		Private Shared Sub UseTwiceRegularArgument(ByVal containingMember As Symbol, ByVal boundArgument As BoundExpression, ByVal arg As ArrayBuilder(Of SynthesizedLocal), ByRef first As BoundExpression, ByRef second As BoundExpression)
			Dim result As UseTwiceRewriter.Result = UseTwiceRewriter.UseTwiceRValue(containingMember, boundArgument, arg)
			first = result.First
			second = result.Second
		End Sub

		Private Shared Function UseTwiceRValue(ByVal containingMember As Symbol, ByVal value As BoundExpression, ByVal arg As ArrayBuilder(Of SynthesizedLocal)) As UseTwiceRewriter.Result
			Dim result As UseTwiceRewriter.Result
			Dim kind As BoundKind = value.Kind
			If (kind = BoundKind.BadVariable OrElse kind = BoundKind.MeReference OrElse kind = BoundKind.MyBaseReference OrElse kind = BoundKind.MyClassReference OrElse kind = BoundKind.Literal) Then
				result = New UseTwiceRewriter.Result(value, value)
			Else
				If (Not value.IsValue() OrElse value.Type Is Nothing OrElse value.Type.IsVoidType()) Then
					Throw ExceptionUtilities.Unreachable
				End If
				Dim constantValueOpt As ConstantValue = value.ConstantValueOpt
				If (constantValueOpt Is Nothing) Then
					Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
					Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = UseTwiceRewriter.CaptureInATemp(containingMember, value, arg, boundLocal)
					result = New UseTwiceRewriter.Result(boundAssignmentOperator, boundLocal)
				Else
					Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(value.Syntax, constantValueOpt, value.Type)
					boundLiteral.SetWasCompilerGenerated()
					result = New UseTwiceRewriter.Result(value, boundLiteral)
				End If
			End If
			Return result
		End Function

		Public Structure Result
			Public ReadOnly First As BoundExpression

			Public ReadOnly Second As BoundExpression

			Public Sub New(ByVal first As BoundExpression, ByVal second As BoundExpression)
				Me = New UseTwiceRewriter.Result() With
				{
					.First = first,
					.Second = second
				}
			End Sub
		End Structure
	End Class
End Namespace