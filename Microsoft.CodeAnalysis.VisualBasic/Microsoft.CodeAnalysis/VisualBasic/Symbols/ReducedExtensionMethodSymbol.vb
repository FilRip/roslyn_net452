Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Reflection
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class ReducedExtensionMethodSymbol
		Inherits MethodSymbol
		Private ReadOnly _receiverType As TypeSymbol

		Private ReadOnly _curriedFromMethod As MethodSymbol

		Private ReadOnly _fixedTypeParameters As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeSymbol))

		Private ReadOnly _proximity As Integer

		Private ReadOnly _curryTypeSubstitution As TypeSubstitution

		Private ReadOnly _curriedTypeParameters As ImmutableArray(Of ReducedExtensionMethodSymbol.ReducedTypeParameterSymbol)

		Private _lazyReturnType As TypeSymbol

		Private _lazyParameters As ImmutableArray(Of ReducedExtensionMethodSymbol.ReducedParameterSymbol)

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return Me._curriedTypeParameters.Length
			End Get
		End Property

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return Me._curriedFromMethod.CallingConvention
			End Get
		End Property

		Friend Overrides ReadOnly Property CallsiteReducedFromMethod As MethodSymbol
			Get
				Dim constructedNotSpecializedGenericMethod As MethodSymbol
				If (Me._curryTypeSubstitution Is Nothing) Then
					constructedNotSpecializedGenericMethod = Me._curriedFromMethod
				ElseIf (Me._curriedFromMethod.Arity <> Me.Arity) Then
					Dim value(Me._curriedFromMethod.Arity - 1 + 1 - 1) As TypeSymbol
					Dim enumerator As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeSymbol)).Enumerator = Me._fixedTypeParameters.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of TypeParameterSymbol, TypeSymbol) = enumerator.Current
						value(current.Key.Ordinal) = current.Value
					End While
					Dim enumerator1 As ImmutableArray(Of ReducedExtensionMethodSymbol.ReducedTypeParameterSymbol).Enumerator = Me._curriedTypeParameters.GetEnumerator()
					While enumerator1.MoveNext()
						Dim reducedTypeParameterSymbol As ReducedExtensionMethodSymbol.ReducedTypeParameterSymbol = enumerator1.Current
						value(reducedTypeParameterSymbol.ReducedFrom.Ordinal) = reducedTypeParameterSymbol
					End While
					constructedNotSpecializedGenericMethod = New SubstitutedMethodSymbol.ConstructedNotSpecializedGenericMethod(Me._curryTypeSubstitution, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(value))
				Else
					constructedNotSpecializedGenericMethod = New SubstitutedMethodSymbol.ConstructedNotSpecializedGenericMethod(Me._curryTypeSubstitution, Me.TypeArguments)
				End If
				Return constructedNotSpecializedGenericMethod
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._curriedFromMethod.ContainingSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Me._curriedFromMethod.ContainingType
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me._curriedFromMethod.DeclaredAccessibility
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._curriedFromMethod.DeclaringSyntaxReferences
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				Return ImmutableArray(Of MethodSymbol).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property FixedTypeParameters As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeSymbol))
			Get
				Return Me._fixedTypeParameters
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return Me._curriedFromMethod.GenerateDebugInfo
			End Get
		End Property

		Friend Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Return Me._curriedFromMethod.HasDeclarativeSecurity
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me._curriedFromMethod.HasSpecialName
			End Get
		End Property

		Friend Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
			Get
				Return Me._curriedFromMethod.ImplementationAttributes
			End Get
		End Property

		Public Overrides ReadOnly Property IsAsync As Boolean
			Get
				Return Me._curriedFromMethod.IsAsync
			End Get
		End Property

		Public Overrides ReadOnly Property IsExtensionMethod As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsExternalMethod As Boolean
			Get
				Return Me._curriedFromMethod.IsExternalMethod
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._curriedFromMethod.IsImplicitlyDeclared
			End Get
		End Property

		Public Overrides ReadOnly Property IsInitOnly As Boolean
			Get
				Return Me._curriedFromMethod.IsInitOnly
			End Get
		End Property

		Public Overrides ReadOnly Property IsIterator As Boolean
			Get
				Return Me._curriedFromMethod.IsIterator
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMethodKindBasedOnSyntax As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return Me._curriedFromMethod.IsSub
			End Get
		End Property

		Public Overrides ReadOnly Property IsVararg As Boolean
			Get
				Return Me._curriedFromMethod.IsVararg
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._curriedFromMethod.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MayBeReducibleExtensionMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._curriedFromMethod.MetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Microsoft.CodeAnalysis.MethodKind.ReducedExtension
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._curriedFromMethod.Name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Me._curriedFromMethod.ObsoleteAttributeData
			End Get
		End Property

		Public Overrides ReadOnly Property OverriddenMethod As MethodSymbol
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property ParameterCount As Integer
			Get
				Return Me._curriedFromMethod.ParameterCount - 1
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				If (Me._lazyParameters.IsDefault) Then
					Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = Me._curriedFromMethod.Parameters
					If (parameterSymbols.Length <> 1) Then
						Dim reducedParameterSymbol(parameterSymbols.Length - 2 + 1 - 1) As ReducedExtensionMethodSymbol.ReducedParameterSymbol
						Dim length As Integer = parameterSymbols.Length - 1
						Dim num As Integer = 1
						Do
							reducedParameterSymbol(num - 1) = New ReducedExtensionMethodSymbol.ReducedParameterSymbol(Me, parameterSymbols(num))
							num = num + 1
						Loop While num <= length
						Dim reducedParameterSymbols As ImmutableArray(Of ReducedExtensionMethodSymbol.ReducedParameterSymbol) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of ReducedExtensionMethodSymbol.ReducedParameterSymbol)(reducedParameterSymbol)
						Dim reducedParameterSymbols1 As ImmutableArray(Of ReducedExtensionMethodSymbol.ReducedParameterSymbol) = New ImmutableArray(Of ReducedExtensionMethodSymbol.ReducedParameterSymbol)()
						ImmutableInterlocked.InterlockedCompareExchange(Of ReducedExtensionMethodSymbol.ReducedParameterSymbol)(Me._lazyParameters, reducedParameterSymbols, reducedParameterSymbols1)
					Else
						Me._lazyParameters = ImmutableArray(Of ReducedExtensionMethodSymbol.ReducedParameterSymbol).Empty
					End If
				End If
				Return StaticCast(Of ParameterSymbol).From(Of ReducedExtensionMethodSymbol.ReducedParameterSymbol)(Me._lazyParameters)
			End Get
		End Property

		Friend Overrides ReadOnly Property Proximity As Integer
			Get
				Return Me._proximity
			End Get
		End Property

		Public Overrides ReadOnly Property ReceiverType As TypeSymbol
			Get
				Return Me._receiverType
			End Get
		End Property

		Public Overrides ReadOnly Property ReducedFrom As MethodSymbol
			Get
				Return Me._curriedFromMethod
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._curriedFromMethod.RefCustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return Me._curriedFromMethod.ReturnsByRef
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				If (Me._lazyReturnType Is Nothing) Then
					Dim type As TypeSymbol = Me._curriedFromMethod.ReturnType
					If (Me._curryTypeSubstitution IsNot Nothing) Then
						type = type.InternalSubstituteTypeParameters(Me._curryTypeSubstitution).Type
					End If
					Interlocked.CompareExchange(Of TypeSymbol)(Me._lazyReturnType, type, Nothing)
				End If
				Return Me._lazyReturnType
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._curriedFromMethod.ReturnTypeCustomModifiers
			End Get
		End Property

		Friend Overrides ReadOnly Property ReturnTypeMarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Me._curriedFromMethod.ReturnTypeMarshallingInformation
			End Get
		End Property

		Friend Overrides ReadOnly Property ShadowsExplicitly As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property Syntax As SyntaxNode
			Get
				Return Me._curriedFromMethod.Syntax
			End Get
		End Property

		Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
			Get
				Return StaticCast(Of TypeSymbol).From(Of ReducedExtensionMethodSymbol.ReducedTypeParameterSymbol)(Me._curriedTypeParameters)
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return StaticCast(Of TypeParameterSymbol).From(Of ReducedExtensionMethodSymbol.ReducedTypeParameterSymbol)(Me._curriedTypeParameters)
			End Get
		End Property

		Private Sub New(ByVal receiverType As TypeSymbol, ByVal curriedFromMethod As MethodSymbol, ByVal fixedTypeParameters As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeSymbol)), ByVal proximity As Integer)
			MyBase.New()
			Me._curriedFromMethod = curriedFromMethod
			Me._receiverType = receiverType
			Me._fixedTypeParameters = fixedTypeParameters
			Me._proximity = proximity
			If (Me._curriedFromMethod.Arity = 0) Then
				Me._curryTypeSubstitution = Nothing
				Me._curriedTypeParameters = ImmutableArray(Of ReducedExtensionMethodSymbol.ReducedTypeParameterSymbol).Empty
				Return
			End If
			Dim reducedTypeParameterSymbolArray As ReducedExtensionMethodSymbol.ReducedTypeParameterSymbol() = Nothing
			If (fixedTypeParameters.Length < curriedFromMethod.Arity) Then
				ReDim reducedTypeParameterSymbolArray(curriedFromMethod.Arity - fixedTypeParameters.Length - 1 + 1 - 1)
			End If
			Dim value(curriedFromMethod.Arity - 1 + 1 - 1) As TypeSymbol
			Dim length As Integer = fixedTypeParameters.Length - 1
			Dim num As Integer = 0
			Do
				Dim item As KeyValuePair(Of TypeParameterSymbol, TypeSymbol) = fixedTypeParameters(num)
				value(item.Key.Ordinal) = item.Value
				num = num + 1
			Loop While num <= length
			If (reducedTypeParameterSymbolArray IsNot Nothing) Then
				Dim num1 As Integer = 0
				Dim length1 As Integer = CInt(value.Length) - 1
				num = 0
				Do
					If (value(num) Is Nothing) Then
						Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = curriedFromMethod.TypeParameters
						Dim reducedTypeParameterSymbol As ReducedExtensionMethodSymbol.ReducedTypeParameterSymbol = New ReducedExtensionMethodSymbol.ReducedTypeParameterSymbol(Me, typeParameters(num), num1)
						reducedTypeParameterSymbolArray(num1) = reducedTypeParameterSymbol
						value(num) = reducedTypeParameterSymbol
						num1 = num1 + 1
						If (num1 = CInt(reducedTypeParameterSymbolArray.Length)) Then
							Exit Do
						End If
					End If
					num = num + 1
				Loop While num <= length1
				Me._curriedTypeParameters = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of ReducedExtensionMethodSymbol.ReducedTypeParameterSymbol)(reducedTypeParameterSymbolArray)
			Else
				Me._curriedTypeParameters = ImmutableArray(Of ReducedExtensionMethodSymbol.ReducedTypeParameterSymbol).Empty
			End If
			Me._curryTypeSubstitution = TypeSubstitution.Create(curriedFromMethod, curriedFromMethod.TypeParameters, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(value), False)
		End Sub

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Shared Function Create(ByVal instanceType As TypeSymbol, ByVal possiblyExtensionMethod As MethodSymbol, ByVal proximity As Integer, ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As MethodSymbol
			Dim reducedExtensionMethodSymbol As MethodSymbol
			Dim enumerator As HashSet(Of TypeParameterSymbol).Enumerator = New HashSet(Of TypeParameterSymbol).Enumerator()
			If (Not possiblyExtensionMethod.IsDefinition OrElse Not possiblyExtensionMethod.MayBeReducibleExtensionMethod OrElse possiblyExtensionMethod.MethodKind = Microsoft.CodeAnalysis.MethodKind.ReducedExtension) Then
				reducedExtensionMethodSymbol = Nothing
			ElseIf (possiblyExtensionMethod.ParameterCount <> 0) Then
				Dim type As TypeSymbol = possiblyExtensionMethod.Parameters(0).Type
				Dim typeParameterSymbols As HashSet(Of TypeParameterSymbol) = New HashSet(Of TypeParameterSymbol)()
				type.CollectReferencedTypeParameters(typeParameterSymbols)
				Dim immutableAndFree As ImmutableArray(Of TypeParameterSymbol) = New ImmutableArray(Of TypeParameterSymbol)()
				Dim typeSymbols As ImmutableArray(Of TypeSymbol) = New ImmutableArray(Of TypeSymbol)()
				Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = If(useSiteInfo.AccumulatesDependencies, New CompoundUseSiteInfo(Of AssemblySymbol)(useSiteInfo.AssemblyBeingBuilt), CompoundUseSiteInfo(Of AssemblySymbol).DiscardedDependencies)
				If (typeParameterSymbols.Count > 0) Then
					Dim instance As ArrayBuilder(Of Integer) = ArrayBuilder(Of Integer).GetInstance(possiblyExtensionMethod.ParameterCount, -1)
					instance(0) = 0
					Dim typeSymbols1 As ImmutableArray(Of TypeSymbol) = New ImmutableArray(Of TypeSymbol)()
					Dim inferenceLevel As TypeArgumentInference.InferenceLevel = TypeArgumentInference.InferenceLevel.None
					Dim flag As Boolean = False
					Dim flag1 As Boolean = False
					Dim inferenceErrorReason As InferenceErrorReasons = InferenceErrorReasons.Other
					Dim bitVector As Microsoft.CodeAnalysis.BitVector = Microsoft.CodeAnalysis.BitVector.Create(possiblyExtensionMethod.Arity)
					Try
						enumerator = typeParameterSymbols.GetEnumerator()
						While enumerator.MoveNext()
							bitVector(enumerator.Current.Ordinal) = True
						End While
					Finally
						DirectCast(enumerator, IDisposable).Dispose()
					End Try
					Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = If(compoundUseSiteInfo.AccumulatesDependencies, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance(False, True), Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded)
					Dim dummy As VisualBasicSyntaxTree = VisualBasicSyntaxTree.Dummy
					Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
					Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(New BoundRValuePlaceholder(dummy.GetRoot(cancellationToken), instanceType))
					Dim bitVector1 As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
					Dim syntaxNodeOrTokens As ImmutableArray(Of SyntaxNodeOrToken) = New ImmutableArray(Of SyntaxNodeOrToken)()
					Dim boundExpressions1 As HashSet(Of BoundExpression) = Nothing
					Dim flag2 As Boolean = TypeArgumentInference.Infer(possiblyExtensionMethod, boundExpressions, instance, Nothing, Nothing, Nothing, typeSymbols1, inferenceLevel, flag, flag1, inferenceErrorReason, bitVector1, syntaxNodeOrTokens, boundExpressions1, compoundUseSiteInfo, bindingDiagnosticBag, bitVector)
					instance.Free()
					If (Not flag2 OrElse Not compoundUseSiteInfo.Diagnostics.IsNullOrEmpty()) Then
						bindingDiagnosticBag.Free()
						reducedExtensionMethodSymbol = Nothing
						Return reducedExtensionMethodSymbol
					Else
						compoundUseSiteInfo.AddDependencies(bindingDiagnosticBag.DependenciesBag)
						bindingDiagnosticBag.Free()
						Dim count As Integer = typeParameterSymbols.Count
						Dim instance1 As ArrayBuilder(Of TypeParameterSymbol) = ArrayBuilder(Of TypeParameterSymbol).GetInstance(count)
						Dim instance2 As ArrayBuilder(Of TypeSymbol) = ArrayBuilder(Of TypeSymbol).GetInstance(count)
						Dim arity As Integer = possiblyExtensionMethod.Arity - 1
						Dim num As Integer = 0
						Do
							If (bitVector(num)) Then
								instance1.Add(possiblyExtensionMethod.TypeParameters(num))
								instance2.Add(typeSymbols1(num))
								If (instance1.Count = count) Then
									Exit Do
								End If
							End If
							num = num + 1
						Loop While num <= arity
						immutableAndFree = instance1.ToImmutableAndFree()
						typeSymbols = instance2.ToImmutableAndFree()
						Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(possiblyExtensionMethod, immutableAndFree, typeSymbols, False)
						If (typeSubstitution IsNot Nothing) Then
							Dim typeParameterDiagnosticInfos As ArrayBuilder(Of TypeParameterDiagnosticInfo) = ArrayBuilder(Of TypeParameterDiagnosticInfo).GetInstance()
							Dim typeParameterDiagnosticInfos1 As ArrayBuilder(Of TypeParameterDiagnosticInfo) = Nothing
							If (Not possiblyExtensionMethod.CheckConstraints(typeSubstitution, immutableAndFree, typeSymbols, typeParameterDiagnosticInfos, typeParameterDiagnosticInfos1, New CompoundUseSiteInfo(Of AssemblySymbol)(compoundUseSiteInfo))) Then
								typeParameterDiagnosticInfos.Free()
								reducedExtensionMethodSymbol = Nothing
								Return reducedExtensionMethodSymbol
							End If
							If (typeParameterDiagnosticInfos1 IsNot Nothing) Then
								typeParameterDiagnosticInfos.AddRange(typeParameterDiagnosticInfos1)
							End If
							Dim enumerator1 As ArrayBuilder(Of TypeParameterDiagnosticInfo).Enumerator = typeParameterDiagnosticInfos.GetEnumerator()
							While enumerator1.MoveNext()
								compoundUseSiteInfo.AddDependencies(enumerator1.Current.UseSiteInfo)
							End While
							typeParameterDiagnosticInfos.Free()
							type = type.InternalSubstituteTypeParameters(typeSubstitution).Type
						End If
					End If
				End If
				If (Not OverloadResolution.DoesReceiverMatchInstance(instanceType, type, compoundUseSiteInfo) OrElse Not compoundUseSiteInfo.Diagnostics.IsNullOrEmpty()) Then
					reducedExtensionMethodSymbol = Nothing
				ElseIf (Not possiblyExtensionMethod.IsExtensionMethod OrElse possiblyExtensionMethod.MethodKind = Microsoft.CodeAnalysis.MethodKind.ReducedExtension) Then
					reducedExtensionMethodSymbol = Nothing
				Else
					Dim empty As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeSymbol)) = ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeSymbol)).Empty
					If (Not immutableAndFree.IsDefault) Then
						Dim keyValuePairs As ArrayBuilder(Of KeyValuePair(Of TypeParameterSymbol, TypeSymbol)) = ArrayBuilder(Of KeyValuePair(Of TypeParameterSymbol, TypeSymbol)).GetInstance(immutableAndFree.Length)
						Dim length As Integer = immutableAndFree.Length - 1
						Dim num1 As Integer = 0
						Do
							keyValuePairs.Add(New KeyValuePair(Of TypeParameterSymbol, TypeSymbol)(immutableAndFree(num1), typeSymbols(num1)))
							num1 = num1 + 1
						Loop While num1 <= length
						empty = keyValuePairs.ToImmutableAndFree()
					End If
					useSiteInfo.AddDependencies(compoundUseSiteInfo)
					reducedExtensionMethodSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.ReducedExtensionMethodSymbol(type, possiblyExtensionMethod, empty, proximity)
				End If
			Else
				reducedExtensionMethodSymbol = Nothing
			End If
			Return reducedExtensionMethodSymbol
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Dim flag As Boolean
			If (obj <> Me) Then
				Dim reducedExtensionMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ReducedExtensionMethodSymbol = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.ReducedExtensionMethodSymbol)
				flag = If(reducedExtensionMethodSymbol Is Nothing OrElse Not reducedExtensionMethodSymbol._curriedFromMethod.Equals(Me._curriedFromMethod), False, reducedExtensionMethodSymbol._receiverType.Equals(Me._receiverType))
			Else
				flag = True
			End If
			Return flag
		End Function

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Return Me._curriedFromMethod.GetAppliedConditionalSymbols()
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me._curriedFromMethod.GetAttributes()
		End Function

		Public Overrides Function GetDllImportData() As DllImportData
			Return Me._curriedFromMethod.GetDllImportData()
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._curriedFromMethod.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(Me._receiverType.GetHashCode(), Me._curriedFromMethod.GetHashCode())
		End Function

		Public Overrides Function GetReturnTypeAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me._curriedFromMethod.GetReturnTypeAttributes()
		End Function

		Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Return Me._curriedFromMethod.GetSecurityInformation()
		End Function

		Public Overrides Function GetTypeInferredDuringReduction(ByVal reducedFromTypeParameter As TypeParameterSymbol) As TypeSymbol
			Dim value As TypeSymbol
			If (reducedFromTypeParameter Is Nothing) Then
				Throw New ArgumentNullException()
			End If
			If (reducedFromTypeParameter.ContainingSymbol <> Me._curriedFromMethod) Then
				Throw New ArgumentException()
			End If
			Dim enumerator As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeSymbol)).Enumerator = Me._fixedTypeParameters.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As KeyValuePair(Of TypeParameterSymbol, TypeSymbol) = enumerator.Current
					If (TypeSymbol.Equals(current.Key, reducedFromTypeParameter, TypeCompareKind.ConsiderEverything)) Then
						value = current.Value
						Exit While
					End If
				Else
					value = Nothing
					Exit While
				End If
			End While
			Return value
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Return Me._curriedFromMethod.GetUseSiteInfo()
		End Function

		Friend Overrides Function IsMetadataNewSlot(Optional ByVal ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
			Return False
		End Function

		Private Class ReducedParameterSymbol
			Inherits ReducedParameterSymbolBase
			Private ReadOnly _curriedMethod As ReducedExtensionMethodSymbol

			Private _lazyType As TypeSymbol

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me._curriedMethod
				End Get
			End Property

			Public Overrides ReadOnly Property Type As TypeSymbol
				Get
					If (Me._lazyType Is Nothing) Then
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.m_CurriedFromParameter.Type
						If (Me._curriedMethod._curryTypeSubstitution IsNot Nothing) Then
							typeSymbol = typeSymbol.InternalSubstituteTypeParameters(Me._curriedMethod._curryTypeSubstitution).Type
						End If
						Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)(Me._lazyType, typeSymbol, Nothing)
					End If
					Return Me._lazyType
				End Get
			End Property

			Public Sub New(ByVal curriedMethod As ReducedExtensionMethodSymbol, ByVal curriedFromParameter As ParameterSymbol)
				MyBase.New(curriedFromParameter)
				Me._curriedMethod = curriedMethod
			End Sub
		End Class

		Private NotInheritable Class ReducedTypeParameterSymbol
			Inherits TypeParameterSymbol
			Private ReadOnly _curriedMethod As ReducedExtensionMethodSymbol

			Private ReadOnly _curriedFromTypeParameter As TypeParameterSymbol

			Private ReadOnly _ordinal As Integer

			Friend Overrides ReadOnly Property ConstraintTypesNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
				Get
					Dim typeSymbols As ImmutableArray(Of TypeSymbol) = Me._curriedFromTypeParameter.ConstraintTypesNoUseSiteDiagnostics
					Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Me._curriedMethod._curryTypeSubstitution
					If (typeSubstitution IsNot Nothing) Then
						typeSymbols = TypeParameterSymbol.InternalSubstituteTypeParametersDistinct(typeSubstitution, typeSymbols)
					End If
					Return typeSymbols
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me._curriedMethod
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
				Get
					Return Me._curriedFromTypeParameter.DeclaringSyntaxReferences
				End Get
			End Property

			Public Overrides ReadOnly Property HasConstructorConstraint As Boolean
				Get
					Return Me._curriedFromTypeParameter.HasConstructorConstraint
				End Get
			End Property

			Public Overrides ReadOnly Property HasReferenceTypeConstraint As Boolean
				Get
					Return Me._curriedFromTypeParameter.HasReferenceTypeConstraint
				End Get
			End Property

			Public Overrides ReadOnly Property HasValueTypeConstraint As Boolean
				Get
					Return Me._curriedFromTypeParameter.HasValueTypeConstraint
				End Get
			End Property

			Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
				Get
					Return Me._curriedFromTypeParameter.IsImplicitlyDeclared
				End Get
			End Property

			Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
				Get
					Return Me._curriedFromTypeParameter.Locations
				End Get
			End Property

			Public Overrides ReadOnly Property MetadataName As String
				Get
					Return Me._curriedFromTypeParameter.MetadataName
				End Get
			End Property

			Public Overrides ReadOnly Property Name As String
				Get
					Return Me._curriedFromTypeParameter.Name
				End Get
			End Property

			Public Overrides ReadOnly Property Ordinal As Integer
				Get
					Return Me._ordinal
				End Get
			End Property

			Public Overrides ReadOnly Property ReducedFrom As TypeParameterSymbol
				Get
					Return Me._curriedFromTypeParameter
				End Get
			End Property

			Public Overrides ReadOnly Property TypeParameterKind As TypeParameterKind
				Get
					Return TypeParameterKind.Method
				End Get
			End Property

			Public Overrides ReadOnly Property Variance As VarianceKind
				Get
					Return Me._curriedFromTypeParameter.Variance
				End Get
			End Property

			Public Sub New(ByVal curriedMethod As ReducedExtensionMethodSymbol, ByVal curriedFromTypeParameter As TypeParameterSymbol, ByVal ordinal As Integer)
				MyBase.New()
				Me._curriedMethod = curriedMethod
				Me._curriedFromTypeParameter = curriedFromTypeParameter
				Me._ordinal = ordinal
			End Sub

			Friend Overrides Sub EnsureAllConstraintsAreResolved()
				Me._curriedFromTypeParameter.EnsureAllConstraintsAreResolved()
			End Sub

			Public Overrides Function Equals(ByVal other As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
				Return Me.Equals(TryCast(other, ReducedExtensionMethodSymbol.ReducedTypeParameterSymbol), comparison)
			End Function

			Public Function Equals(ByVal other As ReducedExtensionMethodSymbol.ReducedTypeParameterSymbol, ByVal comparison As TypeCompareKind) As Boolean
				Dim flag As Boolean
				If (CObj(Me) <> CObj(other)) Then
					flag = If(other Is Nothing OrElse Me._ordinal <> other._ordinal, False, Me.ContainingSymbol.Equals(other.ContainingSymbol, comparison))
				Else
					flag = True
				End If
				Return flag
			End Function

			Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
				Return Me._curriedFromTypeParameter.GetAttributes()
			End Function

			Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
				Return Me._curriedFromTypeParameter.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
			End Function

			Public Overrides Function GetHashCode() As Integer
				Dim num As Integer = Me._ordinal
				Return Hash.Combine(num.GetHashCode(), Me.ContainingSymbol.GetHashCode())
			End Function

			Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
				Return Me._curriedFromTypeParameter.GetUseSiteInfo()
			End Function
		End Class
	End Class
End Namespace