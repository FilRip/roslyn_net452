Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class SynthesizedLambdaMethod
		Inherits SynthesizedMethod
		Implements ISynthesizedMethodBodyImplementationSymbol
		Private ReadOnly _lambda As LambdaSymbol

		Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

		Private ReadOnly _locations As ImmutableArray(Of Location)

		Private ReadOnly _typeParameters As ImmutableArray(Of TypeParameterSymbol)

		Private ReadOnly _typeMap As TypeSubstitution

		Private ReadOnly _topLevelMethod As MethodSymbol

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return Me._typeParameters.Length
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				If (Not TypeOf MyBase.ContainingType Is LambdaFrame) Then
					Return Accessibility.[Private]
				End If
				Return Accessibility.Internal
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return Me._lambda.GenerateDebugInfoImpl
			End Get
		End Property

		Public ReadOnly Property HasMethodBodyDependency As Boolean Implements ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsAsync As Boolean
			Get
				Return Me._lambda.IsAsync
			End Get
		End Property

		Public Overrides ReadOnly Property IsIterator As Boolean
			Get
				Return Me._lambda.IsIterator
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsVararg As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._locations
			End Get
		End Property

		Public ReadOnly Property Method As IMethodSymbolInternal Implements ISynthesizedMethodBodyImplementationSymbol.Method
			Get
				Return Me._topLevelMethod
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._parameters
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me._lambda.ReturnType.InternalSubstituteTypeParameters(Me.TypeMap).Type
			End Get
		End Property

		Public ReadOnly Property TopLevelMethod As MethodSymbol
			Get
				Return Me._topLevelMethod
			End Get
		End Property

		Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
			Get
				Dim empty As ImmutableArray(Of TypeSymbol)
				If (Me.Arity <= 0) Then
					empty = ImmutableArray(Of TypeSymbol).Empty
				Else
					empty = StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(Me.TypeParameters)
				End If
				Return empty
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeMap As TypeSubstitution
			Get
				Return Me._typeMap
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return Me._typeParameters
			End Get
		End Property

		Friend Sub New(ByVal containingType As InstanceTypeSymbol, ByVal closureKind As Microsoft.CodeAnalysis.VisualBasic.ClosureKind, ByVal topLevelMethod As MethodSymbol, ByVal topLevelMethodId As DebugId, ByVal lambdaNode As BoundLambda, ByVal lambdaId As DebugId, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			MyBase.New(lambdaNode.Syntax, containingType, SynthesizedLambdaMethod.MakeName(topLevelMethodId, closureKind, lambdaNode.LambdaSymbol.SynthesizedKind, lambdaId), False)
			Me._lambda = lambdaNode.LambdaSymbol
			Me._locations = ImmutableArray.Create(Of Location)(lambdaNode.Syntax.GetLocation())
			If (topLevelMethod.IsGenericMethod) Then
				Dim lambdaFrame As Microsoft.CodeAnalysis.VisualBasic.LambdaFrame = TryCast(containingType, Microsoft.CodeAnalysis.VisualBasic.LambdaFrame)
				If (lambdaFrame Is Nothing) Then
					Me._typeParameters = SynthesizedClonedTypeParameterSymbol.MakeTypeParameters(topLevelMethod.TypeParameters, Me, Microsoft.CodeAnalysis.VisualBasic.LambdaFrame.CreateTypeParameter)
					Me._typeMap = TypeSubstitution.Create(topLevelMethod, topLevelMethod.TypeParameters, Me.TypeArguments, False)
				Else
					Me._typeParameters = ImmutableArray(Of TypeParameterSymbol).Empty
					Me._typeMap = lambdaFrame.TypeMap
				End If
			Else
				Me._typeMap = Nothing
				Me._typeParameters = ImmutableArray(Of TypeParameterSymbol).Empty
			End If
			Dim instance As ArrayBuilder(Of ParameterSymbol) = ArrayBuilder(Of ParameterSymbol).GetInstance()
			Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = Me._lambda.Parameters.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As ParameterSymbol = enumerator.Current
				instance.Add(SynthesizedMethod.WithNewContainerAndType(Me, current.Type.InternalSubstituteTypeParameters(Me.TypeMap).Type, current))
			End While
			Me._parameters = instance.ToImmutableAndFree()
			Me._topLevelMethod = topLevelMethod
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			If (Not Me.GenerateDebugInfoImpl) Then
				Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.SynthesizeDebuggerHiddenAttribute())
			End If
			If (Me.IsAsync OrElse Me.IsIterator) Then
				Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.SynthesizeStateMachineAttribute(Me, compilationState))
				If (Me.IsAsync) Then
					Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.SynthesizeOptionalDebuggerStepThroughAttribute())
				End If
			End If
		End Sub

		Friend Function AsMember(ByVal constructedFrame As NamedTypeSymbol) As MethodSymbol
			Dim memberForDefinition As MethodSymbol
			If (CObj(constructedFrame) <> CObj(MyBase.ContainingType)) Then
				memberForDefinition = DirectCast(DirectCast(constructedFrame, SubstitutedNamedType).GetMemberForDefinition(Me), MethodSymbol)
			Else
				memberForDefinition = Me
			End If
			Return memberForDefinition
		End Function

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Return Me._topLevelMethod.CalculateLocalSyntaxOffset(localPosition, localTree)
		End Function

		Friend Overrides Function IsMetadataNewSlot(Optional ByVal ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
			Return False
		End Function

		Private Shared Function MakeName(ByVal topLevelMethodId As DebugId, ByVal closureKind As Microsoft.CodeAnalysis.VisualBasic.ClosureKind, ByVal lambdaKind As SynthesizedLambdaKind, ByVal lambdaId As DebugId) As String
			Return GeneratedNames.MakeLambdaMethodName(If(closureKind = Microsoft.CodeAnalysis.VisualBasic.ClosureKind.General, -1, topLevelMethodId.Ordinal), topLevelMethodId.Generation, lambdaId.Ordinal, lambdaId.Generation, lambdaKind)
		End Function
	End Class
End Namespace