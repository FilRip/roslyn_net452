Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SourceNonPropertyAccessorMethodSymbol
		Inherits SourceMethodSymbol
		Private _lazyParameters As ImmutableArray(Of ParameterSymbol)

		Private _lazyReturnType As TypeSymbol

		Private _lazyOverriddenMethods As OverriddenMembersResult(Of MethodSymbol)

		Friend NotOverridable Overrides ReadOnly Property OverriddenMembers As OverriddenMembersResult(Of MethodSymbol)
			Get
				Me.EnsureSignature()
				Return Me._lazyOverriddenMethods
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ParameterCount As Integer
			Get
				Dim length As Integer
				Dim declarationSyntax As MethodBaseSyntax
				Dim parameterList As ParameterListSyntax
				Dim num As Integer
				If (Me._lazyParameters.IsDefault) Then
					declarationSyntax = MyBase.DeclarationSyntax
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = declarationSyntax.Kind()
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						parameterList = DirectCast(declarationSyntax, MethodStatementSyntax).ParameterList
					Else
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement) Then
							GoTo Label1
						End If
						length = MyBase.ParameterCount
						Return length
					End If
				Label2:
					num = If(parameterList Is Nothing, 0, parameterList.Parameters.Count)
					length = num
				Else
					length = Me._lazyParameters.Length
				End If
				Return length
			Label1:
				parameterList = DirectCast(declarationSyntax, SubNewStatementSyntax).ParameterList
				GoTo Label2
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Me.EnsureSignature()
				Return Me._lazyParameters
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Me.EnsureSignature()
				Return Me._lazyReturnType
			End Get
		End Property

		Protected Sub New(ByVal containingType As NamedTypeSymbol, ByVal flags As SourceMemberFlags, ByVal syntaxRef As SyntaxReference, Optional ByVal locations As ImmutableArray(Of Location) = Nothing)
			MyBase.New(containingType, flags, syntaxRef, locations)
		End Sub

		Private Function CreateBinderForMethodDeclaration(ByVal sourceModule As SourceModuleSymbol) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForMethodDeclaration(sourceModule, MyBase.SyntaxTree, Me)
			Return New LocationSpecificBinder(BindingLocation.MethodSignature, Me, binder)
		End Function

		Private Sub EnsureSignature()
			Dim empty As OverriddenMembersResult(Of MethodSymbol)
			Dim typeParameterSymbols As ImmutableArray(Of TypeParameterSymbol)
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim typeWithModifier As TypeWithModifiers
			If (Me._lazyParameters.IsDefault) Then
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
				Dim containingSourceModule As SourceModuleSymbol = MyBase.ContainingSourceModule
				Dim parameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = Me.GetParameters(containingSourceModule, instance)
				Dim syntaxNodeOrToken As Microsoft.CodeAnalysis.SyntaxNodeOrToken = New Microsoft.CodeAnalysis.SyntaxNodeOrToken()
				Dim returnType As TypeSymbol = Me.GetReturnType(containingSourceModule, syntaxNodeOrToken, instance)
				If (Not MyBase.IsOverrides OrElse Not OverrideHidingHelper.CanOverrideOrHide(Me)) Then
					empty = OverriddenMembersResult(Of MethodSymbol).Empty
				Else
					If (Me.Arity <= 0) Then
						typeParameterSymbols = ImmutableArray(Of TypeParameterSymbol).Empty
						typeSubstitution = Nothing
					Else
						typeParameterSymbols = IndexedTypeParameterSymbol.Take(Me.Arity)
						typeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(Me, Me.TypeParameters, StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(typeParameterSymbols), False)
					End If
					Dim parameterSymbols As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol).GetInstance(parameters.Length)
					Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol).Enumerator = parameters.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = enumerator.Current
						typeWithModifier = current.Type.InternalSubstituteTypeParameters(typeSubstitution)
						parameterSymbols.Add(New SignatureOnlyParameterSymbol(typeWithModifier.AsTypeSymbolOnly(), ImmutableArray(Of CustomModifier).Empty, ImmutableArray(Of CustomModifier).Empty, Nothing, False, current.IsByRef, False, current.IsOptional))
					End While
					Dim name As String = Me.Name
					Dim mContainingType As NamedTypeSymbol = Me.m_containingType
					Dim methodKind As Microsoft.CodeAnalysis.MethodKind = Me.MethodKind
					Dim callingConvention As Microsoft.Cci.CallingConvention = MyBase.CallingConvention
					Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = parameterSymbols.ToImmutableAndFree()
					typeWithModifier = returnType.InternalSubstituteTypeParameters(typeSubstitution)
					empty = OverrideHidingHelper(Of MethodSymbol).MakeOverriddenMembers(New SignatureOnlyMethodSymbol(name, mContainingType, methodKind, callingConvention, typeParameterSymbols, immutableAndFree, False, typeWithModifier.AsTypeSymbolOnly(), ImmutableArray(Of CustomModifier).Empty, ImmutableArray(Of CustomModifier).Empty, ImmutableArray(Of MethodSymbol).Empty, True))
				End If
				Dim overriddenMember As MethodSymbol = empty.OverriddenMember
				If (overriddenMember IsNot Nothing) Then
					CustomModifierUtils.CopyMethodCustomModifiers(overriddenMember, MyBase.TypeArguments, returnType, parameters)
				End If
				Interlocked.CompareExchange(Of OverriddenMembersResult(Of MethodSymbol))(Me._lazyOverriddenMethods, empty, Nothing)
				Interlocked.CompareExchange(Of TypeSymbol)(Me._lazyReturnType, returnType, Nothing)
				returnType = Me._lazyReturnType
				Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol).Enumerator = parameters.GetEnumerator()
				While enumerator1.MoveNext()
					Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = enumerator1.Current
					If (parameterSymbol.Locations.Length <= 0) Then
						Continue While
					End If
					Dim type As TypeSymbol = parameterSymbol.Type
					Dim locations As ImmutableArray(Of Location) = parameterSymbol.Locations
					type.CheckAllConstraints(locations(0), instance, New CompoundUseSiteInfo(Of AssemblySymbol)(instance, containingSourceModule.ContainingAssembly))
				End While
				If (Not syntaxNodeOrToken.IsKind(SyntaxKind.None)) Then
					Dim typeParameterDiagnosticInfos As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterDiagnosticInfo) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterDiagnosticInfo).GetInstance()
					Dim typeParameterDiagnosticInfos1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterDiagnosticInfo) = Nothing
					returnType.CheckAllConstraints(typeParameterDiagnosticInfos, typeParameterDiagnosticInfos1, New CompoundUseSiteInfo(Of AssemblySymbol)(instance, containingSourceModule.ContainingAssembly))
					If (typeParameterDiagnosticInfos1 IsNot Nothing) Then
						typeParameterDiagnosticInfos.AddRange(typeParameterDiagnosticInfos1)
					End If
					Dim enumerator2 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterDiagnosticInfo).Enumerator = typeParameterDiagnosticInfos.GetEnumerator()
					While enumerator2.MoveNext()
						Dim typeParameterDiagnosticInfo As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterDiagnosticInfo = enumerator2.Current
						instance.Add(typeParameterDiagnosticInfo.UseSiteInfo, syntaxNodeOrToken.GetLocation())
					End While
					typeParameterDiagnosticInfos.Free()
				End If
				containingSourceModule.AtomicStoreArrayAndDiagnostics(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)(Me._lazyParameters, parameters, instance)
				instance.Free()
			End If
		End Sub

		Private Shared Function GetNameToken(ByVal methodStatement As MethodBaseSyntax) As SyntaxToken
			Dim identifier As SyntaxToken
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = methodStatement.Kind()
			If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
				identifier = DirectCast(methodStatement, MethodStatementSyntax).Identifier
			ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
				identifier = DirectCast(methodStatement, DeclareStatementSyntax).Identifier
			Else
				If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement) Then
					Throw ExceptionUtilities.UnexpectedValue(methodStatement.Kind())
				End If
				identifier = DirectCast(methodStatement, OperatorStatementSyntax).OperatorToken
			End If
			Return identifier
		End Function

		Protected Overridable Function GetParameters(ByVal sourceModule As SourceModuleSymbol, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of ParameterSymbol)
			Dim parameterList As ParameterListSyntax
			Dim declarationSyntax As MethodBaseSyntax = MyBase.DeclarationSyntax
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.CreateBinderForMethodDeclaration(sourceModule)
			Select Case declarationSyntax.Kind()
				Case SyntaxKind.SubStatement
				Case SyntaxKind.FunctionStatement
					parameterList = DirectCast(declarationSyntax, MethodStatementSyntax).ParameterList
					Exit Select
				Case SyntaxKind.SubNewStatement
					parameterList = DirectCast(declarationSyntax, SubNewStatementSyntax).ParameterList
					Exit Select
				Case SyntaxKind.DeclareSubStatement
				Case SyntaxKind.DeclareFunctionStatement
					parameterList = DirectCast(declarationSyntax, DeclareStatementSyntax).ParameterList
					Exit Select
				Case SyntaxKind.DelegateSubStatement
				Case SyntaxKind.DelegateFunctionStatement
				Case 100
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement
				Case SyntaxKind.EventStatement
					Throw ExceptionUtilities.UnexpectedValue(declarationSyntax.Kind())
				Case SyntaxKind.OperatorStatement
					parameterList = DirectCast(declarationSyntax, OperatorStatementSyntax).ParameterList
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(declarationSyntax.Kind())
			End Select
			Return binder.DecodeParameterList(Me, False, Me.m_flags, parameterList, diagBag)
		End Function

		Private Function GetReturnType(ByVal sourceModule As SourceModuleSymbol, ByRef errorLocation As SyntaxNodeOrToken, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim specialType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.CreateBinderForMethodDeclaration(sourceModule)
			Dim methodKind As Microsoft.CodeAnalysis.MethodKind = Me.MethodKind
			Select Case methodKind
				Case Microsoft.CodeAnalysis.MethodKind.Constructor
				Case Microsoft.CodeAnalysis.MethodKind.EventRaise
				Case Microsoft.CodeAnalysis.MethodKind.EventRemove
				Label1:
					specialType = binder.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Void, Me.Syntax, diagBag)
					Exit Select
				Case Microsoft.CodeAnalysis.MethodKind.Conversion
				Case Microsoft.CodeAnalysis.MethodKind.DelegateInvoke
				Case Microsoft.CodeAnalysis.MethodKind.Destructor
				Label0:
					Dim declarationSyntax As MethodBaseSyntax = MyBase.DeclarationSyntax
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = declarationSyntax.Kind()
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement) Then
						Microsoft.CodeAnalysis.VisualBasic.Binder.DisallowTypeCharacter(SourceNonPropertyAccessorMethodSymbol.GetNameToken(declarationSyntax), diagBag, ERRID.ERR_TypeCharOnSub)
						typeSymbol = binder.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Void, Me.Syntax, diagBag)
						errorLocation = declarationSyntax.DeclarationKeyword
					Else
						Dim getErrorInfoERRStrictDisallowsImplicitProc As Func(Of DiagnosticInfo) = Nothing
						If (binder.OptionStrict = OptionStrict.[On]) Then
							getErrorInfoERRStrictDisallowsImplicitProc = ErrorFactory.GetErrorInfo_ERR_StrictDisallowsImplicitProc
						ElseIf (binder.OptionStrict = OptionStrict.Custom) Then
							getErrorInfoERRStrictDisallowsImplicitProc = If(Me.MethodKind <> Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator, ErrorFactory.GetErrorInfo_WRN_ObjectAssumed1_WRN_MissingAsClauseinFunction, ErrorFactory.GetErrorInfo_WRN_ObjectAssumed1_WRN_MissingAsClauseinOperator)
						End If
						Dim asClauseInternal As AsClauseSyntax = declarationSyntax.AsClauseInternal
						typeSymbol = binder.DecodeIdentifierType(SourceNonPropertyAccessorMethodSymbol.GetNameToken(declarationSyntax), asClauseInternal, getErrorInfoERRStrictDisallowsImplicitProc, diagBag)
						If (asClauseInternal Is Nothing) Then
							errorLocation = declarationSyntax.DeclarationKeyword
						Else
							errorLocation = asClauseInternal.Type()
						End If
					End If
					If (Not typeSymbol.IsErrorType()) Then
						AccessCheck.VerifyAccessExposureForMemberType(Me, errorLocation, typeSymbol, diagBag, False)
						Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
						If (typeSymbol.IsRestrictedArrayType(typeSymbol1)) Then
							Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_RestrictedType1, New [Object]() { typeSymbol1 })
						End If
						If (Not MyBase.IsAsync OrElse Not MyBase.IsIterator) Then
							If (Not Me.IsSub) Then
								If (MyBase.IsAsync) Then
									Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
									If (Not typeSymbol.OriginalDefinition.Equals(declaringCompilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task_T)) AndAlso Not typeSymbol.Equals(declaringCompilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task))) Then
										Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_BadAsyncReturn)
									End If
								End If
								If (MyBase.IsIterator) Then
									Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeSymbol.OriginalDefinition
									If (originalDefinition.SpecialType <> Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IEnumerable_T AndAlso originalDefinition.SpecialType <> Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IEnumerator_T AndAlso typeSymbol.SpecialType <> Microsoft.CodeAnalysis.SpecialType.System_Collections_IEnumerable AndAlso typeSymbol.SpecialType <> Microsoft.CodeAnalysis.SpecialType.System_Collections_IEnumerator) Then
										Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_BadIteratorReturn)
									End If
								End If
							ElseIf (MyBase.IsIterator) Then
								Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_BadIteratorReturn)
							End If
						End If
					End If
					specialType = typeSymbol
					Exit Select
				Case Microsoft.CodeAnalysis.MethodKind.EventAdd
					specialType = If(DirectCast(Me.AssociatedSymbol, EventSymbol).IsWindowsRuntimeEvent, binder.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken, Me.Syntax, diagBag), binder.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Void, Me.Syntax, diagBag))
					Exit Select
				Case Else
					If (CInt(methodKind) - CInt(Microsoft.CodeAnalysis.MethodKind.PropertyGet) <= CInt(Microsoft.CodeAnalysis.MethodKind.Constructor)) Then
						Throw ExceptionUtilities.Unreachable
					End If
					If (methodKind <> Microsoft.CodeAnalysis.MethodKind.StaticConstructor) Then
						GoTo Label0
					Else
						GoTo Label1
					End If
			End Select
			Return specialType
		End Function
	End Class
End Namespace