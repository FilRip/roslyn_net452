Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Linq
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class UnboundLambda
		Inherits BoundExpression
		Private ReadOnly _Binder As Microsoft.CodeAnalysis.VisualBasic.Binder

		Private ReadOnly _Flags As SourceMemberFlags

		Private ReadOnly _Parameters As ImmutableArray(Of ParameterSymbol)

		Private ReadOnly _ReturnType As TypeSymbol

		Private ReadOnly _BindingCache As UnboundLambda.UnboundLambdaBindingCache

		Public ReadOnly Property Binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Get
				Return Me._Binder
			End Get
		End Property

		Public ReadOnly Property BindingCache As UnboundLambda.UnboundLambdaBindingCache
			Get
				Return Me._BindingCache
			End Get
		End Property

		Public ReadOnly Property Flags As SourceMemberFlags
			Get
				Return Me._Flags
			End Get
		End Property

		Public ReadOnly Property InferredAnonymousDelegate As KeyValuePair(Of NamedTypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol))
			Get
				Dim anonymousDelegate As Tuple(Of NamedTypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol)) = Me._BindingCache.AnonymousDelegate
				If (anonymousDelegate Is Nothing) Then
					Dim keyValuePair As KeyValuePair(Of NamedTypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol)) = Me._Binder.InferAnonymousDelegateForLambda(Me)
					Interlocked.CompareExchange(Of Tuple(Of NamedTypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol)))(Me._BindingCache.AnonymousDelegate, New Tuple(Of NamedTypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol))(keyValuePair.Key, keyValuePair.Value), Nothing)
					anonymousDelegate = Me._BindingCache.AnonymousDelegate
				End If
				Return New KeyValuePair(Of NamedTypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol))(anonymousDelegate.Item1, anonymousDelegate.Item2)
			End Get
		End Property

		Public ReadOnly Property IsFunctionLambda As Boolean
			Get
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = MyBase.Syntax.Kind()
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineFunctionLambdaExpression) Then
					Return True
				End If
				Return syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineFunctionLambdaExpression
			End Get
		End Property

		Public ReadOnly Property IsSingleLine As Boolean
			Get
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = MyBase.Syntax.Kind()
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineFunctionLambdaExpression) Then
					Return True
				End If
				Return syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression
			End Get
		End Property

		Public ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._Parameters
			End Get
		End Property

		Public ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me._ReturnType
			End Get
		End Property

		Public ReadOnly Property WithDependencies As Boolean
			Get
				Return Me._BindingCache.WithDependencies
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal flags As SourceMemberFlags, ByVal parameters As ImmutableArray(Of ParameterSymbol), ByVal returnType As TypeSymbol, ByVal bindingCache As UnboundLambda.UnboundLambdaBindingCache, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.UnboundLambda, syntax, Nothing, hasErrors)
			Me._Binder = binder
			Me._Flags = flags
			Me._Parameters = parameters
			Me._ReturnType = returnType
			Me._BindingCache = bindingCache
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal flags As SourceMemberFlags, ByVal parameters As ImmutableArray(Of ParameterSymbol), ByVal returnType As TypeSymbol, ByVal bindingCache As UnboundLambda.UnboundLambdaBindingCache)
			MyBase.New(BoundKind.UnboundLambda, syntax, Nothing)
			Me._Binder = binder
			Me._Flags = flags
			Me._Parameters = parameters
			Me._ReturnType = returnType
			Me._BindingCache = bindingCache
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitUnboundLambda(Me)
		End Function

		Public Function Bind(ByVal target As UnboundLambda.TargetSignature) As BoundLambda
			Return Me._BindingCache.BoundLambdas.GetOrAdd(target, New Func(Of UnboundLambda.TargetSignature, BoundLambda)(AddressOf Me.DoBind))
		End Function

		Public Function BindForErrorRecovery() As BoundLambda
			Return Me._Binder.BindLambdaForErrorRecovery(Me)
		End Function

		Private Function DoBind(ByVal target As UnboundLambda.TargetSignature) As BoundLambda
			Return Me._Binder.BindUnboundLambda(Me, target)
		End Function

		Private Function DoInferFunctionLambdaReturnType(ByVal target As UnboundLambda.TargetSignature) As KeyValuePair(Of TypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol))
			Return Me._Binder.InferFunctionLambdaReturnType(Me, target)
		End Function

		Public Function GetBoundLambda(ByVal target As UnboundLambda.TargetSignature) As Microsoft.CodeAnalysis.VisualBasic.BoundLambda
			Dim boundLambda As Microsoft.CodeAnalysis.VisualBasic.BoundLambda
			Dim boundLambda1 As Microsoft.CodeAnalysis.VisualBasic.BoundLambda = Nothing
			If (Not Me._BindingCache.BoundLambdas.TryGetValue(target, boundLambda1)) Then
				boundLambda = Nothing
			Else
				boundLambda = boundLambda1
			End If
			Return boundLambda
		End Function

		Private Function GetSingletonBoundLambda() As Microsoft.CodeAnalysis.VisualBasic.BoundLambda
			Dim boundLambda As Microsoft.CodeAnalysis.VisualBasic.BoundLambda
			Dim boundLambda1 As Microsoft.CodeAnalysis.VisualBasic.BoundLambda = Me._BindingCache.BoundLambdas.Values.FirstOrDefault()
			If (Me._BindingCache.BoundLambdas.Count <> 1) Then
				boundLambda = Nothing
			Else
				boundLambda = boundLambda1
			End If
			Return boundLambda
		End Function

		Public Function InferReturnType(ByVal target As UnboundLambda.TargetSignature) As KeyValuePair(Of TypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol))
			Dim orAdd As KeyValuePair(Of TypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol))
			If (Me.ReturnType Is Nothing) Then
				orAdd = Me._BindingCache.InferredReturnType.GetOrAdd(target, New Func(Of UnboundLambda.TargetSignature, KeyValuePair(Of TypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol)))(AddressOf Me.DoInferFunctionLambdaReturnType))
			Else
				Dim keyValuePair As KeyValuePair(Of TypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol)) = New KeyValuePair(Of TypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol))(If(Not Me.IsFunctionLambda OrElse Not Me.ReturnType.IsVoidType(), Me.ReturnType, LambdaSymbol.ReturnTypeVoidReplacement), New ImmutableBindingDiagnostic(Of AssemblySymbol)())
				orAdd = Me._BindingCache.InferredReturnType.GetOrAdd(target, keyValuePair)
			End If
			Return orAdd
		End Function

		Public Function IsInferredDelegateForThisLambda(ByVal delegateType As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			Dim anonymousDelegate As Tuple(Of NamedTypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol)) = Me._BindingCache.AnonymousDelegate
			flag = If(anonymousDelegate IsNot Nothing, CObj(delegateType) = CObj(anonymousDelegate.Item1), False)
			Return flag
		End Function

		Public Function Update(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal flags As SourceMemberFlags, ByVal parameters As ImmutableArray(Of ParameterSymbol), ByVal returnType As TypeSymbol, ByVal bindingCache As Microsoft.CodeAnalysis.VisualBasic.UnboundLambda.UnboundLambdaBindingCache) As Microsoft.CodeAnalysis.VisualBasic.UnboundLambda
			Dim unboundLambda As Microsoft.CodeAnalysis.VisualBasic.UnboundLambda
			If (binder <> Me.Binder OrElse flags <> Me.Flags OrElse parameters <> Me.Parameters OrElse CObj(returnType) <> CObj(Me.ReturnType) OrElse bindingCache <> Me.BindingCache) Then
				Dim unboundLambda1 As Microsoft.CodeAnalysis.VisualBasic.UnboundLambda = New Microsoft.CodeAnalysis.VisualBasic.UnboundLambda(MyBase.Syntax, binder, flags, parameters, returnType, bindingCache, MyBase.HasErrors)
				unboundLambda1.CopyAttributes(Me)
				unboundLambda = unboundLambda1
			Else
				unboundLambda = Me
			End If
			Return unboundLambda
		End Function

		Friend Class TargetSignature
			Public ReadOnly ParameterTypes As ImmutableArray(Of TypeSymbol)

			Public ReadOnly ReturnType As TypeSymbol

			Public ReadOnly ReturnsByRef As Boolean

			Public ReadOnly ParameterIsByRef As BitVector

			Public Sub New(ByVal parameterTypes As ImmutableArray(Of TypeSymbol), ByVal parameterIsByRef As BitVector, ByVal returnType As TypeSymbol, ByVal returnsByRef As Boolean)
				MyBase.New()
				Me.ParameterTypes = parameterTypes
				Me.ParameterIsByRef = parameterIsByRef
				Me.ReturnType = returnType
				Me.ReturnsByRef = returnsByRef
			End Sub

			Public Sub New(ByVal params As ImmutableArray(Of ParameterSymbol), ByVal returnType As TypeSymbol, ByVal returnsByRef As Boolean)
				MyBase.New()
				Dim empty As BitVector = BitVector.Empty
				If (params.Length <> 0) Then
					Dim type(params.Length - 1 + 1 - 1) As TypeSymbol
					Dim length As Integer = params.Length - 1
					Dim num As Integer = 0
					Do
						type(num) = params(num).Type
						If (params(num).IsByRef) Then
							empty(num) = True
						End If
						num = num + 1
					Loop While num <= length
					Me.ParameterTypes = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(type)
				Else
					Me.ParameterTypes = ImmutableArray(Of TypeSymbol).Empty
				End If
				Me.ParameterIsByRef = empty
				Me.ReturnType = returnType
				Me.ReturnsByRef = returnsByRef
			End Sub

			Public Sub New(ByVal method As MethodSymbol)
				MyClass.New(method.Parameters, method.ReturnType, method.ReturnsByRef)
			End Sub

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				Dim flag As Boolean
				If (obj <> Me) Then
					Dim targetSignature As UnboundLambda.TargetSignature = TryCast(obj, UnboundLambda.TargetSignature)
					If (targetSignature Is Nothing OrElse targetSignature.ParameterTypes.Length <> Me.ParameterTypes.Length) Then
						flag = False
					Else
						Dim length As Integer = Me.ParameterTypes.Length - 1
						Dim num As Integer = 0
						While num <= length
							If (Not TypeSymbol.Equals(Me.ParameterTypes(num), targetSignature.ParameterTypes(num), TypeCompareKind.ConsiderEverything) OrElse Me.ParameterIsByRef(num) <> targetSignature.ParameterIsByRef(num)) Then
								flag = False
								Return flag
							Else
								num = num + 1
							End If
						End While
						flag = If(Me.ReturnsByRef <> targetSignature.ReturnsByRef, False, TypeSymbol.Equals(Me.ReturnType, targetSignature.ReturnType, TypeCompareKind.ConsiderEverything))
					End If
				Else
					flag = True
				End If
				Return flag
			End Function

			Public Overrides Function GetHashCode() As Integer
				Dim num As Integer = 0
				Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = Me.ParameterTypes.GetEnumerator()
				While enumerator.MoveNext()
					num = Hash.Combine(Of TypeSymbol)(enumerator.Current, num)
				End While
				num = Hash.Combine(Of TypeSymbol)(Me.ReturnType, num)
				Return num
			End Function
		End Class

		Public Class UnboundLambdaBindingCache
			Public ReadOnly WithDependencies As Boolean

			Public AnonymousDelegate As Tuple(Of NamedTypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol))

			Public ReadOnly InferredReturnType As ConcurrentDictionary(Of UnboundLambda.TargetSignature, KeyValuePair(Of TypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol)))

			Public ReadOnly BoundLambdas As ConcurrentDictionary(Of UnboundLambda.TargetSignature, BoundLambda)

			Public ErrorRecoverySignature As UnboundLambda.TargetSignature

			Public Sub New(ByVal withDependencies As Boolean)
				MyBase.New()
				Me.InferredReturnType = New ConcurrentDictionary(Of UnboundLambda.TargetSignature, KeyValuePair(Of TypeSymbol, ImmutableBindingDiagnostic(Of AssemblySymbol)))()
				Me.BoundLambdas = New ConcurrentDictionary(Of UnboundLambda.TargetSignature, BoundLambda)()
				Me.WithDependencies = withDependencies
			End Sub
		End Class
	End Class
End Namespace