Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundCall
		Inherits BoundExpression
		Private ReadOnly _Method As MethodSymbol

		Private ReadOnly _MethodGroupOpt As BoundMethodGroup

		Private ReadOnly _ReceiverOpt As BoundExpression

		Private ReadOnly _Arguments As ImmutableArray(Of BoundExpression)

		Private ReadOnly _DefaultArguments As BitVector

		Private ReadOnly _ConstantValueOpt As ConstantValue

		Private ReadOnly _IsLValue As Boolean

		Private ReadOnly _SuppressObjectClone As Boolean

		Public ReadOnly Property Arguments As ImmutableArray(Of BoundExpression)
			Get
				Return Me._Arguments
			End Get
		End Property

		Public Overrides ReadOnly Property ConstantValueOpt As ConstantValue
			Get
				Return Me._ConstantValueOpt
			End Get
		End Property

		Public ReadOnly Property DefaultArguments As BitVector
			Get
				Return Me._DefaultArguments
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.Method
			End Get
		End Property

		Public Overrides ReadOnly Property IsLValue As Boolean
			Get
				Return Me._IsLValue
			End Get
		End Property

		Public ReadOnly Property Method As MethodSymbol
			Get
				Return Me._Method
			End Get
		End Property

		Public ReadOnly Property MethodGroupOpt As BoundMethodGroup
			Get
				Return Me._MethodGroupOpt
			End Get
		End Property

		Public ReadOnly Property ReceiverOpt As BoundExpression
			Get
				Return Me._ReceiverOpt
			End Get
		End Property

		Public Overrides ReadOnly Property ResultKind As LookupResultKind
			Get
				Dim lookupResultKind As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind
				lookupResultKind = If(Me.MethodGroupOpt Is Nothing, MyBase.ResultKind, Me.MethodGroupOpt.ResultKind)
				Return lookupResultKind
			End Get
		End Property

		Public ReadOnly Property SuppressObjectClone As Boolean
			Get
				Return Me._SuppressObjectClone
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal method As MethodSymbol, ByVal methodGroupOpt As BoundMethodGroup, ByVal receiverOpt As BoundExpression, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal constantValueOpt As ConstantValue, ByVal type As TypeSymbol, Optional ByVal suppressObjectClone As Boolean = False, Optional ByVal hasErrors As Boolean = False, Optional ByVal defaultArguments As BitVector = Nothing)
			MyClass.New(syntax, method, methodGroupOpt, receiverOpt, arguments, defaultArguments, constantValueOpt, method.ReturnsByRef, suppressObjectClone, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal method As MethodSymbol, ByVal methodGroupOpt As BoundMethodGroup, ByVal receiverOpt As BoundExpression, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal constantValueOpt As ConstantValue, ByVal isLValue As Boolean, ByVal suppressObjectClone As Boolean, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, method, methodGroupOpt, receiverOpt, arguments, BitVector.Null, constantValueOpt, isLValue, suppressObjectClone, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal method As MethodSymbol, ByVal methodGroupOpt As BoundMethodGroup, ByVal receiverOpt As BoundExpression, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal defaultArguments As BitVector, ByVal constantValueOpt As ConstantValue, ByVal isLValue As Boolean, ByVal suppressObjectClone As Boolean, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.[Call], syntax, type, If(hasErrors OrElse methodGroupOpt.NonNullAndHasErrors() OrElse receiverOpt.NonNullAndHasErrors(), True, arguments.NonNullAndHasErrors()))
			Me._Method = method
			Me._MethodGroupOpt = methodGroupOpt
			Me._ReceiverOpt = receiverOpt
			Me._Arguments = arguments
			Me._DefaultArguments = defaultArguments
			Me._ConstantValueOpt = constantValueOpt
			Me._IsLValue = isLValue
			Me._SuppressObjectClone = suppressObjectClone
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitCall(Me)
		End Function

		Public Shadows Function MakeRValue() As Microsoft.CodeAnalysis.VisualBasic.BoundCall
			Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall
			boundCall = If(Not Me._IsLValue, Me, Me.Update(Me.Method, Me.MethodGroupOpt, Me.ReceiverOpt, Me.Arguments, Me.DefaultArguments, Me.ConstantValueOpt, False, Me.SuppressObjectClone, MyBase.Type))
			Return boundCall
		End Function

		Protected Overrides Function MakeRValueImpl() As BoundExpression
			Return Me.MakeRValue()
		End Function

		Public Function Update(ByVal method As MethodSymbol, ByVal methodGroupOpt As BoundMethodGroup, ByVal receiverOpt As BoundExpression, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal defaultArguments As BitVector, ByVal constantValueOpt As ConstantValue, ByVal isLValue As Boolean, ByVal suppressObjectClone As Boolean, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundCall
			Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall
			If (CObj(method) <> CObj(Me.Method) OrElse methodGroupOpt <> Me.MethodGroupOpt OrElse receiverOpt <> Me.ReceiverOpt OrElse arguments <> Me.Arguments OrElse defaultArguments <> Me.DefaultArguments OrElse CObj(constantValueOpt) <> CObj(Me.ConstantValueOpt) OrElse isLValue <> Me.IsLValue OrElse suppressObjectClone <> Me.SuppressObjectClone OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundCall1 As Microsoft.CodeAnalysis.VisualBasic.BoundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(MyBase.Syntax, method, methodGroupOpt, receiverOpt, arguments, defaultArguments, constantValueOpt, isLValue, suppressObjectClone, type, MyBase.HasErrors)
				boundCall1.CopyAttributes(Me)
				boundCall = boundCall1
			Else
				boundCall = Me
			End If
			Return boundCall
		End Function
	End Class
End Namespace