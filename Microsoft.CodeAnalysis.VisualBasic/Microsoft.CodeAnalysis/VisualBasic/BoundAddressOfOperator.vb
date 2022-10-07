Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundAddressOfOperator
		Inherits BoundExpression
		Private ReadOnly _delegateResolutionResultCache As ConcurrentDictionary(Of TypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Binder.DelegateResolutionResult)

		Private ReadOnly _Binder As Microsoft.CodeAnalysis.VisualBasic.Binder

		Private ReadOnly _WithDependencies As Boolean

		Private ReadOnly _MethodGroup As BoundMethodGroup

		Public ReadOnly Property Binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Get
				Return Me._Binder
			End Get
		End Property

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.MethodGroup)
			End Get
		End Property

		Public ReadOnly Property MethodGroup As BoundMethodGroup
			Get
				Return Me._MethodGroup
			End Get
		End Property

		Public ReadOnly Property WithDependencies As Boolean
			Get
				Return Me._WithDependencies
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal withDependencies As Boolean, ByVal methodGroup As BoundMethodGroup, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.AddressOfOperator, syntax, Nothing, If(hasErrors, True, methodGroup.NonNullAndHasErrors()))
			Me._delegateResolutionResultCache = New ConcurrentDictionary(Of TypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Binder.DelegateResolutionResult)()
			Me._Binder = binder
			Me._WithDependencies = withDependencies
			Me._MethodGroup = methodGroup
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitAddressOfOperator(Me)
		End Function

		Friend Function GetConversionClassification(ByVal targetType As TypeSymbol) As ConversionKind
			Dim delegateResolutionResult As Microsoft.CodeAnalysis.VisualBasic.Binder.DelegateResolutionResult = New Microsoft.CodeAnalysis.VisualBasic.Binder.DelegateResolutionResult()
			If (Not Me._delegateResolutionResultCache.TryGetValue(targetType, delegateResolutionResult)) Then
				delegateResolutionResult = Microsoft.CodeAnalysis.VisualBasic.Binder.InterpretDelegateBinding(Me, targetType, False)
				Me._delegateResolutionResultCache.TryAdd(targetType, delegateResolutionResult)
			End If
			Return delegateResolutionResult.DelegateConversions
		End Function

		Friend Function GetDelegateResolutionResult(ByVal targetType As TypeSymbol, ByRef delegateResolutionResult As Microsoft.CodeAnalysis.VisualBasic.Binder.DelegateResolutionResult) As Boolean
			Return Me._delegateResolutionResultCache.TryGetValue(targetType, delegateResolutionResult)
		End Function

		Public Function Update(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal withDependencies As Boolean, ByVal methodGroup As BoundMethodGroup) As Microsoft.CodeAnalysis.VisualBasic.BoundAddressOfOperator
			Dim boundAddressOfOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAddressOfOperator
			If (binder <> Me.Binder OrElse withDependencies <> Me.WithDependencies OrElse methodGroup <> Me.MethodGroup) Then
				Dim boundAddressOfOperator1 As Microsoft.CodeAnalysis.VisualBasic.BoundAddressOfOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundAddressOfOperator(MyBase.Syntax, binder, withDependencies, methodGroup, MyBase.HasErrors)
				boundAddressOfOperator1.CopyAttributes(Me)
				boundAddressOfOperator = boundAddressOfOperator1
			Else
				boundAddressOfOperator = Me
			End If
			Return boundAddressOfOperator
		End Function
	End Class
End Namespace