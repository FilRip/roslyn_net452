Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module ConstraintsHelper
		Private ReadOnly s_checkConstraintsSingleTypeFunc As Func(Of TypeSymbol, ConstraintsHelper.CheckConstraintsDiagnosticsBuilders, Boolean)

		Sub New()
			ConstraintsHelper.s_checkConstraintsSingleTypeFunc = New Func(Of TypeSymbol, ConstraintsHelper.CheckConstraintsDiagnosticsBuilders, Boolean)(AddressOf ConstraintsHelper.CheckConstraintsSingleType)
		End Sub

		Private Function AppendUseSiteInfo(ByVal useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), ByVal typeParameter As TypeParameterSymbol, <InAttribute> <Out> ByRef useSiteDiagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo)) As Boolean
			Dim flag As Boolean
			Dim enumerator As IEnumerator(Of DiagnosticInfo) = Nothing
			Dim flag1 As Boolean = If(Not useSiteInfo.AccumulatesDiagnostics, False, Not useSiteInfo.Diagnostics.IsNullOrEmpty())
			If (flag1 OrElse useSiteInfo.AccumulatesDependencies AndAlso Not useSiteInfo.Dependencies.IsNullOrEmpty()) Then
				If (useSiteDiagnosticsBuilder Is Nothing) Then
					useSiteDiagnosticsBuilder = New ArrayBuilder(Of TypeParameterDiagnosticInfo)()
				End If
				If (flag1) Then
					Using enumerator
						enumerator = useSiteInfo.Diagnostics.GetEnumerator()
						While enumerator.MoveNext()
							useSiteDiagnosticsBuilder.Add(New TypeParameterDiagnosticInfo(typeParameter, enumerator.Current))
						End While
					End Using
					flag = True
				Else
					If (useSiteInfo.AccumulatesDependencies AndAlso Not useSiteInfo.Dependencies.IsNullOrEmpty()) Then
						useSiteDiagnosticsBuilder.Add(New TypeParameterDiagnosticInfo(typeParameter, If(useSiteInfo.Dependencies.Count = 1, New UseSiteInfo(Of AssemblySymbol)(useSiteInfo.Dependencies.[Single]()), New UseSiteInfo(Of AssemblySymbol)(useSiteInfo.Dependencies.ToImmutableHashSet()))))
					End If
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		<Extension>
		Public Sub CheckAllConstraints(ByVal type As TypeSymbol, ByVal loc As Location, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal template As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim instance As ArrayBuilder(Of TypeParameterDiagnosticInfo) = ArrayBuilder(Of TypeParameterDiagnosticInfo).GetInstance()
			Dim typeParameterDiagnosticInfos As ArrayBuilder(Of TypeParameterDiagnosticInfo) = Nothing
			type.CheckAllConstraints(instance, typeParameterDiagnosticInfos, template)
			If (typeParameterDiagnosticInfos IsNot Nothing) Then
				instance.AddRange(typeParameterDiagnosticInfos)
			End If
			Dim enumerator As ArrayBuilder(Of TypeParameterDiagnosticInfo).Enumerator = instance.GetEnumerator()
			While enumerator.MoveNext()
				diagnostics.Add(enumerator.Current.UseSiteInfo, loc)
			End While
			instance.Free()
		End Sub

		<Extension>
		Public Sub CheckAllConstraints(ByVal type As TypeSymbol, ByVal diagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo), <InAttribute> <Out> ByRef useSiteDiagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo), ByVal template As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim checkConstraintsDiagnosticsBuilder As ConstraintsHelper.CheckConstraintsDiagnosticsBuilders = New ConstraintsHelper.CheckConstraintsDiagnosticsBuilders() With
			{
				.diagnosticsBuilder = diagnosticsBuilder,
				.useSiteDiagnosticsBuilder = useSiteDiagnosticsBuilder,
				.template = template
			}
			type.VisitType(Of ConstraintsHelper.CheckConstraintsDiagnosticsBuilders)(ConstraintsHelper.s_checkConstraintsSingleTypeFunc, checkConstraintsDiagnosticsBuilder)
			useSiteDiagnosticsBuilder = checkConstraintsDiagnosticsBuilder.useSiteDiagnosticsBuilder
		End Sub

		<Extension>
		Public Sub CheckConstraints(ByVal tuple As TupleTypeSymbol, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal elementLocations As ImmutableArray(Of Microsoft.CodeAnalysis.Location), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal template As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim location As Microsoft.CodeAnalysis.Location
			Dim tupleUnderlyingType As NamedTypeSymbol = tuple.TupleUnderlyingType
			If (ConstraintsHelper.RequiresChecking(tupleUnderlyingType) AndAlso Not syntaxNode.HasErrors) Then
				Dim instance As ArrayBuilder(Of TypeParameterDiagnosticInfo) = ArrayBuilder(Of TypeParameterDiagnosticInfo).GetInstance()
				Dim namedTypeSymbols As ArrayBuilder(Of NamedTypeSymbol) = ArrayBuilder(Of NamedTypeSymbol).GetInstance()
				TupleTypeSymbol.GetUnderlyingTypeChain(tupleUnderlyingType, namedTypeSymbols)
				Dim num As Integer = 0
				Dim enumerator As ArrayBuilder(Of NamedTypeSymbol).Enumerator = namedTypeSymbols.GetEnumerator()
				While enumerator.MoveNext()
					Dim typeParameterDiagnosticInfos As ArrayBuilder(Of TypeParameterDiagnosticInfo) = Nothing
					ConstraintsHelper.CheckTypeConstraints(enumerator.Current, instance, typeParameterDiagnosticInfos, template)
					If (typeParameterDiagnosticInfos IsNot Nothing) Then
						instance.AddRange(typeParameterDiagnosticInfos)
					End If
					Dim enumerator1 As ArrayBuilder(Of TypeParameterDiagnosticInfo).Enumerator = instance.GetEnumerator()
					While enumerator1.MoveNext()
						Dim current As TypeParameterDiagnosticInfo = enumerator1.Current
						Dim ordinal As Integer = current.TypeParameter.Ordinal
						location = If(ordinal = 7, syntaxNode.Location, elementLocations(ordinal + num))
						diagnostics.Add(current.UseSiteInfo, location)
					End While
					instance.Clear()
					num += 7
				End While
				namedTypeSymbols.Free()
				instance.Free()
			End If
		End Sub

		<Extension>
		Public Function CheckConstraints(ByVal type As NamedTypeSymbol, ByVal diagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo), <InAttribute> <Out> ByRef useSiteDiagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo), ByVal template As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			type = DirectCast(type.GetTupleUnderlyingTypeOrSelf(), NamedTypeSymbol)
			flag = If(ConstraintsHelper.RequiresChecking(type), ConstraintsHelper.CheckTypeConstraints(type, diagnosticsBuilder, useSiteDiagnosticsBuilder, template), True)
			Return flag
		End Function

		<Extension>
		Public Function CheckConstraints(ByVal method As MethodSymbol, ByVal diagnosticLocation As Location, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal template As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			If (ConstraintsHelper.RequiresChecking(method)) Then
				Dim instance As ArrayBuilder(Of TypeParameterDiagnosticInfo) = ArrayBuilder(Of TypeParameterDiagnosticInfo).GetInstance()
				Dim typeParameterDiagnosticInfos As ArrayBuilder(Of TypeParameterDiagnosticInfo) = Nothing
				Dim flag1 As Boolean = ConstraintsHelper.CheckMethodConstraints(method, instance, typeParameterDiagnosticInfos, template)
				If (typeParameterDiagnosticInfos IsNot Nothing) Then
					instance.AddRange(typeParameterDiagnosticInfos)
				End If
				Dim enumerator As ArrayBuilder(Of TypeParameterDiagnosticInfo).Enumerator = instance.GetEnumerator()
				While enumerator.MoveNext()
					diagnostics.Add(enumerator.Current.UseSiteInfo, diagnosticLocation)
				End While
				instance.Free()
				flag = flag1
			Else
				flag = True
			End If
			Return flag
		End Function

		<Extension>
		Public Function CheckConstraints(ByVal method As MethodSymbol, ByVal diagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo), <InAttribute> <Out> ByRef useSiteDiagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo), ByVal template As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			flag = If(ConstraintsHelper.RequiresChecking(method), ConstraintsHelper.CheckMethodConstraints(method, diagnosticsBuilder, useSiteDiagnosticsBuilder, template), True)
			Return flag
		End Function

		<Extension>
		Public Function CheckConstraints(ByVal constructedSymbol As Symbol, ByVal substitution As TypeSubstitution, ByVal typeParameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol), ByVal typeArguments As ImmutableArray(Of TypeSymbol), ByVal diagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo), <InAttribute> <Out> ByRef useSiteDiagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo), ByVal template As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean = True
			Dim length As Integer = typeParameters.Length - 1
			Dim num As Integer = 0
			Do
				Dim item As TypeSymbol = typeArguments(num)
				Dim typeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = typeParameters(num)
				Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(template)
				If (Not ConstraintsHelper.CheckConstraints(constructedSymbol, substitution, typeParameterSymbol, item, diagnosticsBuilder, compoundUseSiteInfo)) Then
					flag = False
				End If
				If (ConstraintsHelper.AppendUseSiteInfo(compoundUseSiteInfo, typeParameterSymbol, useSiteDiagnosticsBuilder)) Then
					flag = False
				End If
				num = num + 1
			Loop While num <= length
			Return flag
		End Function

		Public Function CheckConstraints(ByVal constructedSymbol As Symbol, ByVal substitution As TypeSubstitution, ByVal typeParameter As TypeParameterSymbol, ByVal typeArgument As TypeSymbol, ByVal diagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			If (Not typeArgument.IsErrorType()) Then
				Dim flag1 As Boolean = True
				If (typeArgument.IsRestrictedType()) Then
					If (diagnosticsBuilder IsNot Nothing) Then
						diagnosticsBuilder.Add(New TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_RestrictedType1, New [Object]() { typeArgument })))
					End If
					flag1 = False
				End If
				If (typeParameter.HasConstructorConstraint AndAlso Not ConstraintsHelper.SatisfiesConstructorConstraint(typeParameter, typeArgument, diagnosticsBuilder)) Then
					flag1 = False
				End If
				If (typeParameter.HasReferenceTypeConstraint AndAlso Not ConstraintsHelper.SatisfiesReferenceTypeConstraint(typeParameter, typeArgument, diagnosticsBuilder)) Then
					flag1 = False
				End If
				If (typeParameter.HasValueTypeConstraint AndAlso Not ConstraintsHelper.SatisfiesValueTypeConstraint(constructedSymbol, typeParameter, typeArgument, diagnosticsBuilder, useSiteInfo)) Then
					flag1 = False
				End If
				Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = typeParameter.ConstraintTypesWithDefinitionUseSiteDiagnostics(useSiteInfo).GetEnumerator()
				While enumerator.MoveNext()
					Dim type As TypeSymbol = enumerator.Current.InternalSubstituteTypeParameters(substitution).Type
					If (ConstraintsHelper.SatisfiesTypeConstraint(typeArgument, type, useSiteInfo)) Then
						Continue While
					End If
					If (diagnosticsBuilder IsNot Nothing) Then
						diagnosticsBuilder.Add(New TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_GenericConstraintNotSatisfied2, New [Object]() { typeArgument, type })))
					End If
					flag1 = False
				End While
				flag = flag1
			Else
				flag = True
			End If
			Return flag
		End Function

		<Extension>
		Public Function CheckConstraintsForNonTuple(ByVal type As NamedTypeSymbol, ByVal typeArgumentsSyntax As SeparatedSyntaxList(Of TypeSyntax), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal template As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			If (ConstraintsHelper.RequiresChecking(type)) Then
				Dim instance As ArrayBuilder(Of TypeParameterDiagnosticInfo) = ArrayBuilder(Of TypeParameterDiagnosticInfo).GetInstance()
				Dim typeParameterDiagnosticInfos As ArrayBuilder(Of TypeParameterDiagnosticInfo) = Nothing
				Dim flag1 As Boolean = ConstraintsHelper.CheckTypeConstraints(type, instance, typeParameterDiagnosticInfos, template)
				If (typeParameterDiagnosticInfos IsNot Nothing) Then
					instance.AddRange(typeParameterDiagnosticInfos)
				End If
				Dim enumerator As ArrayBuilder(Of TypeParameterDiagnosticInfo).Enumerator = instance.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As TypeParameterDiagnosticInfo = enumerator.Current
					Dim location As Microsoft.CodeAnalysis.Location = typeArgumentsSyntax(current.TypeParameter.Ordinal).GetLocation()
					diagnostics.Add(current.UseSiteInfo, location)
				End While
				instance.Free()
				flag = flag1
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Function CheckConstraintsSingleType(ByVal type As TypeSymbol, ByVal diagnostics As ConstraintsHelper.CheckConstraintsDiagnosticsBuilders) As Boolean
			If (type.Kind = SymbolKind.NamedType) Then
				DirectCast(type, NamedTypeSymbol).CheckConstraints(diagnostics.diagnosticsBuilder, diagnostics.useSiteDiagnosticsBuilder, diagnostics.template)
			End If
			Return False
		End Function

		Private Function CheckMethodConstraints(ByVal method As MethodSymbol, ByVal diagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo), <InAttribute> <Out> ByRef useSiteDiagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo), ByVal template As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = DirectCast(method, SubstitutedMethodSymbol).TypeSubstitution
			Return method.CheckConstraints(typeSubstitution, method.OriginalDefinition.TypeParameters, method.TypeArguments, diagnosticsBuilder, useSiteDiagnosticsBuilder, template)
		End Function

		Private Function CheckTypeConstraints(ByVal type As NamedTypeSymbol, ByVal diagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo), <InAttribute> <Out> ByRef useSiteDiagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo), ByVal template As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = type.TypeSubstitution
			Return type.CheckConstraints(typeSubstitution, type.OriginalDefinition.TypeParameters, type.TypeArgumentsNoUseSiteDiagnostics, diagnosticsBuilder, useSiteDiagnosticsBuilder, template)
		End Function

		Private Function ContainsTypeConstraint(ByVal constraints As ArrayBuilder(Of TypeParameterConstraint), ByVal constraintType As TypeSymbol) As Boolean
			Dim flag As Boolean
			Dim enumerator As ArrayBuilder(Of TypeParameterConstraint).Enumerator = constraints.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim typeConstraint As TypeSymbol = enumerator.Current.TypeConstraint
					If (typeConstraint IsNot Nothing AndAlso constraintType.IsSameTypeIgnoringAll(typeConstraint)) Then
						flag = True
						Exit While
					End If
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		<Extension>
		Private Sub GetAllConstraints(ByVal typeParameter As TypeParameterSymbol, ByVal constraintsBuilder As ArrayBuilder(Of ConstraintsHelper.TypeParameterAndConstraint), ByVal fromConstraintOpt As Nullable(Of TypeParameterConstraint))
			Dim instance As ArrayBuilder(Of TypeParameterConstraint) = ArrayBuilder(Of TypeParameterConstraint).GetInstance()
			typeParameter.GetConstraints(instance)
			Dim enumerator As ArrayBuilder(Of TypeParameterConstraint).Enumerator = instance.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As TypeParameterConstraint = enumerator.Current
				Dim typeConstraint As TypeSymbol = current.TypeConstraint
				If (typeConstraint IsNot Nothing) Then
					Dim typeKind As Microsoft.CodeAnalysis.TypeKind = typeConstraint.TypeKind
					If (typeKind = Microsoft.CodeAnalysis.TypeKind.[Error]) Then
						Continue While
					End If
					If (typeKind = Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
						DirectCast(typeConstraint, TypeParameterSymbol).GetAllConstraints(constraintsBuilder, New Nullable(Of TypeParameterConstraint)(If(fromConstraintOpt.HasValue, fromConstraintOpt.Value, current)))
						Continue While
					End If
				End If
				constraintsBuilder.Add(If(fromConstraintOpt.HasValue, New ConstraintsHelper.TypeParameterAndConstraint(DirectCast(fromConstraintOpt.Value.TypeConstraint, TypeParameterSymbol), current.AtLocation(fromConstraintOpt.Value.LocationOpt), False), New ConstraintsHelper.TypeParameterAndConstraint(typeParameter, current, False)))
			End While
			instance.Free()
		End Sub

		<Extension>
		Public Function GetClassConstraint(ByVal typeParameter As TypeParameterSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim nonInterfaceConstraint As TypeSymbol = typeParameter.GetNonInterfaceConstraint(useSiteInfo)
			If (nonInterfaceConstraint IsNot Nothing) Then
				Dim typeKind As Microsoft.CodeAnalysis.TypeKind = nonInterfaceConstraint.TypeKind
				namedTypeSymbol = If(typeKind = Microsoft.CodeAnalysis.TypeKind.Array OrElse typeKind = Microsoft.CodeAnalysis.TypeKind.[Enum] OrElse typeKind = Microsoft.CodeAnalysis.TypeKind.Struct, nonInterfaceConstraint.BaseTypeWithDefinitionUseSiteDiagnostics(useSiteInfo), DirectCast(nonInterfaceConstraint, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol))
			Else
				namedTypeSymbol = Nothing
			End If
			Return namedTypeSymbol
		End Function

		Private Function GetConstraintCycleInfo(ByVal cycle As ConsList(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol)) As CompoundDiagnosticInfo
			Dim enumerator As ConsList(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol).Enumerator = New ConsList(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol).Enumerator()
			Dim typeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = Nothing
			Dim instance As ArrayBuilder(Of DiagnosticInfo) = ArrayBuilder(Of DiagnosticInfo).GetInstance()
			instance.Add(Nothing)
			Try
				enumerator = cycle.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = enumerator.Current
					If (typeParameterSymbol IsNot Nothing) Then
						instance.Add(ErrorFactory.ErrorInfo(ERRID.ERR_ConstraintCycleLink2, New [Object]() { current, typeParameterSymbol }))
					End If
					typeParameterSymbol = current
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			instance(0) = ErrorFactory.ErrorInfo(ERRID.ERR_ConstraintCycleLink2, New [Object]() { cycle.Head, typeParameterSymbol })
			Dim arrayAndFree As DiagnosticInfo() = instance.ToArrayAndFree()
			Array.Reverse(arrayAndFree)
			Return New CompoundDiagnosticInfo(arrayAndFree)
		End Function

		<Extension>
		Public Function GetNonInterfaceConstraint(ByVal typeParameter As TypeParameterSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol).Enumerator = typeParameter.ConstraintTypesWithDefinitionUseSiteDiagnostics(useSiteInfo).GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = enumerator.Current
				Dim nonInterfaceConstraint As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
				Dim kind As SymbolKind = current.Kind
				If (kind = SymbolKind.ErrorType) Then
					Continue While
				End If
				If (kind = SymbolKind.TypeParameter) Then
					nonInterfaceConstraint = DirectCast(current, TypeParameterSymbol).GetNonInterfaceConstraint(useSiteInfo)
				ElseIf (Not current.IsInterfaceType()) Then
					nonInterfaceConstraint = current
				End If
				If (typeSymbol IsNot Nothing) Then
					If (nonInterfaceConstraint Is Nothing OrElse Not typeSymbol.IsClassType() OrElse Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsDerivedFrom(nonInterfaceConstraint, typeSymbol, useSiteInfo)) Then
						Continue While
					End If
					typeSymbol = nonInterfaceConstraint
				Else
					typeSymbol = nonInterfaceConstraint
				End If
			End While
			Return typeSymbol
		End Function

		Private Function HasConflict(ByVal constraint1 As TypeParameterConstraint, ByVal constraint2 As TypeParameterConstraint, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			Dim typeConstraint As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = constraint1.TypeConstraint
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = constraint2.TypeConstraint
			If (typeConstraint IsNot Nothing AndAlso typeConstraint.IsInterfaceType()) Then
				flag = False
			ElseIf (typeSymbol Is Nothing OrElse Not typeSymbol.IsInterfaceType()) Then
				If (Not constraint1.IsValueTypeConstraint) Then
					If (Not constraint2.IsValueTypeConstraint OrElse Not ConstraintsHelper.HasValueTypeConstraintConflict(constraint1, useSiteInfo)) Then
						GoTo Label1
					End If
					flag = True
					Return flag
				Else
					If (Not ConstraintsHelper.HasValueTypeConstraintConflict(constraint2, useSiteInfo)) Then
						GoTo Label1
					End If
					flag = True
					Return flag
				End If
			Label1:
				If (Not constraint1.IsReferenceTypeConstraint) Then
					If (Not constraint2.IsReferenceTypeConstraint OrElse Not ConstraintsHelper.HasReferenceTypeConstraintConflict(constraint1)) Then
						GoTo Label2
					End If
					flag = True
					Return flag
				Else
					If (Not ConstraintsHelper.HasReferenceTypeConstraintConflict(constraint2)) Then
						GoTo Label2
					End If
					flag = True
					Return flag
				End If
			Label2:
				flag = If(typeConstraint Is Nothing OrElse typeSymbol Is Nothing OrElse ConstraintsHelper.SatisfiesTypeConstraint(typeConstraint, typeSymbol, useSiteInfo) OrElse ConstraintsHelper.SatisfiesTypeConstraint(typeSymbol, typeConstraint, useSiteInfo), False, True)
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Function HasPublicParameterlessConstructor(ByVal type As NamedTypeSymbol) As Boolean
			Dim declaredAccessibility As Boolean
			type = type.OriginalDefinition
			Dim sourceNamedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol = TryCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceNamedTypeSymbol)
			If (sourceNamedTypeSymbol Is Nothing OrElse sourceNamedTypeSymbol.MembersHaveBeenCreated) Then
				Dim enumerator As ImmutableArray(Of MethodSymbol).Enumerator = type.InstanceConstructors.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As MethodSymbol = enumerator.Current
					If (current.ParameterCount <> 0) Then
						Continue While
					End If
					declaredAccessibility = current.DeclaredAccessibility = Accessibility.[Public]
					Return declaredAccessibility
				End While
				declaredAccessibility = False
			Else
				declaredAccessibility = sourceNamedTypeSymbol.InferFromSyntaxIfClassWillHavePublicParameterlessConstructor()
			End If
			Return declaredAccessibility
		End Function

		Private Function HasReferenceTypeConstraintConflict(ByVal constraint As TypeParameterConstraint) As Boolean
			Dim flag As Boolean
			Dim typeConstraint As TypeSymbol = constraint.TypeConstraint
			If (typeConstraint IsNot Nothing) Then
				flag = If(Not ConstraintsHelper.SatisfiesReferenceTypeConstraint(Nothing, typeConstraint, Nothing), True, False)
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function HasValueTypeConstraintConflict(ByVal constraint As TypeParameterConstraint, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			Dim typeConstraint As TypeSymbol = constraint.TypeConstraint
			If (typeConstraint Is Nothing) Then
				flag = False
			ElseIf (Not ConstraintsHelper.SatisfiesValueTypeConstraint(Nothing, Nothing, typeConstraint, Nothing, useSiteInfo)) Then
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = typeConstraint.SpecialType
				flag = If(specialType = Microsoft.CodeAnalysis.SpecialType.System_Object OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_ValueType, False, True)
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function IsNullableTypeOrTypeParameter(ByVal type As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			If (type.TypeKind <> TypeKind.TypeParameter) Then
				flag = type.IsNullableType()
			Else
				Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = DirectCast(type, TypeParameterSymbol).ConstraintTypesWithDefinitionUseSiteDiagnostics(useSiteInfo).GetEnumerator()
				While enumerator.MoveNext()
					If (Not ConstraintsHelper.IsNullableTypeOrTypeParameter(enumerator.Current, useSiteInfo)) Then
						Continue While
					End If
					flag = True
					Return flag
				End While
				flag = False
			End If
			Return flag
		End Function

		<Extension>
		Public Function RemoveDirectConstraintConflicts(ByVal typeParameter As TypeParameterSymbol, ByVal constraints As ImmutableArray(Of TypeParameterConstraint), ByVal inProgress As ConsList(Of TypeParameterSymbol), ByVal reportConflicts As DirectConstraintConflictKind, ByVal diagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo)) As ImmutableArray(Of TypeParameterConstraint)
			' 
			' Current member / type: System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterConstraint> Microsoft.CodeAnalysis.VisualBasic.Symbols.ConstraintsHelper::RemoveDirectConstraintConflicts(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol,System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterConstraint>,Roslyn.Utilities.ConsList`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol>,Microsoft.CodeAnalysis.VisualBasic.Symbols.DirectConstraintConflictKind,Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterDiagnosticInfo>)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterConstraint> RemoveDirectConstraintConflicts(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol,System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterConstraint>,Roslyn.Utilities.ConsList<Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol>,Microsoft.CodeAnalysis.VisualBasic.Symbols.DirectConstraintConflictKind,Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder<Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterDiagnosticInfo>)
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

		<Extension>
		Public Sub ReportIndirectConstraintConflicts(ByVal typeParameter As SourceTypeParameterSymbol, ByVal diagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo), <InAttribute> <Out> ByRef useSiteDiagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo))
			Dim instance As ArrayBuilder(Of ConstraintsHelper.TypeParameterAndConstraint) = ArrayBuilder(Of ConstraintsHelper.TypeParameterAndConstraint).GetInstance()
			typeParameter.GetAllConstraints(instance, Nothing)
			Dim count As Integer = instance.Count
			Dim num As Integer = count - 1
			Dim num1 As Integer = 0
			Do
				Dim item As ConstraintsHelper.TypeParameterAndConstraint = instance(num1)
				If (Not item.IsBad) Then
					Dim num2 As Integer = count - 1
					For i As Integer = num1 + 1 To num2
						Dim typeParameterAndConstraint As ConstraintsHelper.TypeParameterAndConstraint = instance(i)
						If (Not typeParameterAndConstraint.IsBad) Then
							Dim flag As Boolean = False
							Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(typeParameter.ContainingAssembly)
							If (item.TypeParameter = typeParameter AndAlso typeParameterAndConstraint.TypeParameter = typeParameter) Then
								If (ConstraintsHelper.HasConflict(item.Constraint, typeParameterAndConstraint.Constraint, compoundUseSiteInfo)) Then
									diagnosticsBuilder.Add(New TypeParameterDiagnosticInfo(typeParameter, typeParameterAndConstraint.Constraint, ErrorFactory.ErrorInfo(ERRID.ERR_ConflictingDirectConstraints3, New [Object]() { typeParameterAndConstraint.Constraint.ToDisplayFormat(), item.Constraint.ToDisplayFormat(), typeParameter })))
									flag = True
								End If
							ElseIf (CObj(item.TypeParameter) <> CObj(typeParameterAndConstraint.TypeParameter) AndAlso ConstraintsHelper.HasConflict(item.Constraint, typeParameterAndConstraint.Constraint, compoundUseSiteInfo)) Then
								If (item.TypeParameter = typeParameter) Then
									diagnosticsBuilder.Add(New TypeParameterDiagnosticInfo(typeParameter, item.Constraint, ErrorFactory.ErrorInfo(ERRID.ERR_ConstraintClashDirectIndirect3, New [Object]() { item.Constraint.ToDisplayFormat(), typeParameterAndConstraint.Constraint.ToDisplayFormat(), typeParameterAndConstraint.TypeParameter })))
									flag = True
								ElseIf (typeParameterAndConstraint.TypeParameter <> typeParameter) Then
									diagnosticsBuilder.Add(New TypeParameterDiagnosticInfo(typeParameter, typeParameterAndConstraint.Constraint, ErrorFactory.ErrorInfo(ERRID.ERR_ConstraintClashIndirectIndirect4, New [Object]() { typeParameterAndConstraint.Constraint.ToDisplayFormat(), typeParameterAndConstraint.TypeParameter, item.Constraint.ToDisplayFormat(), item.TypeParameter })))
									flag = True
								Else
									diagnosticsBuilder.Add(New TypeParameterDiagnosticInfo(typeParameter, item.Constraint, ErrorFactory.ErrorInfo(ERRID.ERR_ConstraintClashIndirectDirect3, New [Object]() { item.Constraint.ToDisplayFormat(), item.TypeParameter, typeParameterAndConstraint.Constraint.ToDisplayFormat() })))
									flag = True
								End If
							End If
							If (ConstraintsHelper.AppendUseSiteInfo(compoundUseSiteInfo, typeParameter, useSiteDiagnosticsBuilder)) Then
								flag = True
							End If
							If (flag) Then
								instance(i) = typeParameterAndConstraint.ToBad()
							End If
						End If
					Next

				End If
				num1 = num1 + 1
			Loop While num1 <= num
			instance.Free()
		End Sub

		Private Function RequiresChecking(ByVal type As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			If (type.Arity <> 0) Then
				flag = If(CObj(type.OriginalDefinition) <> CObj(type), True, False)
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function RequiresChecking(ByVal method As MethodSymbol) As Boolean
			Dim flag As Boolean
			If (method.IsGenericMethod) Then
				flag = If(CObj(method.OriginalDefinition) <> CObj(method), True, False)
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function SatisfiesConstructorConstraint(ByVal typeParameter As TypeParameterSymbol, ByVal typeArgument As TypeSymbol, ByVal diagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo)) As Boolean
			Dim flag As Boolean
			Dim typeKind As Microsoft.CodeAnalysis.TypeKind = typeArgument.TypeKind
			If (typeKind = Microsoft.CodeAnalysis.TypeKind.[Enum] OrElse typeKind = Microsoft.CodeAnalysis.TypeKind.Struct) Then
				flag = True
			ElseIf (typeKind <> Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
				If (typeArgument.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Class]) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(typeArgument, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					If (Not ConstraintsHelper.HasPublicParameterlessConstructor(namedTypeSymbol)) Then
						GoTo Label1
					End If
					If (Not namedTypeSymbol.IsMustInherit) Then
						flag = True
						Return flag
					Else
						If (diagnosticsBuilder IsNot Nothing) Then
							diagnosticsBuilder.Add(New TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_MustInheritForNewConstraint2, New [Object]() { typeArgument, typeParameter })))
						End If
						flag = False
						Return flag
					End If
				End If
			Label1:
				If (diagnosticsBuilder IsNot Nothing) Then
					diagnosticsBuilder.Add(New TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_NoSuitableNewForNewConstraint2, New [Object]() { typeArgument, typeParameter })))
				End If
				flag = False
			ElseIf (DirectCast(typeArgument, TypeParameterSymbol).HasConstructorConstraint OrElse typeArgument.IsValueType) Then
				flag = True
			Else
				If (diagnosticsBuilder IsNot Nothing) Then
					diagnosticsBuilder.Add(New TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_BadGenericParamForNewConstraint2, New [Object]() { typeArgument, typeParameter })))
				End If
				flag = False
			End If
			Return flag
		End Function

		Private Function SatisfiesReferenceTypeConstraint(ByVal typeParameter As TypeParameterSymbol, ByVal typeArgument As TypeSymbol, ByVal diagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo)) As Boolean
			Dim flag As Boolean
			If (typeArgument.IsReferenceType) Then
				flag = True
			Else
				If (diagnosticsBuilder IsNot Nothing) Then
					diagnosticsBuilder.Add(New TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_BadTypeArgForRefConstraint2, New [Object]() { typeArgument, typeParameter })))
				End If
				flag = False
			End If
			Return flag
		End Function

		Private Function SatisfiesTypeConstraint(ByVal typeArgument As TypeSymbol, ByVal constraintType As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			If (Not constraintType.IsErrorType()) Then
				flag = Microsoft.CodeAnalysis.VisualBasic.Conversions.HasWideningDirectCastConversionButNotEnumTypeConversion(typeArgument, constraintType, useSiteInfo)
			Else
				constraintType.AddUseSiteInfo(useSiteInfo)
				flag = False
			End If
			Return flag
		End Function

		Private Function SatisfiesValueTypeConstraint(ByVal constructedSymbol As Symbol, ByVal typeParameter As TypeParameterSymbol, ByVal typeArgument As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal diagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			If (Not typeArgument.IsValueType) Then
				If (diagnosticsBuilder IsNot Nothing) Then
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = TryCast(constructedSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
					If (typeSymbol Is Nothing OrElse Not typeSymbol.IsNullableType()) Then
						diagnosticsBuilder.Add(New TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_BadTypeArgForStructConstraint2, New [Object]() { typeArgument, typeParameter })))
					Else
						diagnosticsBuilder.Add(New TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_BadTypeArgForStructConstraintNull, New [Object]() { typeArgument })))
					End If
				End If
				flag = False
			ElseIf (Not ConstraintsHelper.IsNullableTypeOrTypeParameter(typeArgument, useSiteInfo)) Then
				flag = True
			Else
				If (diagnosticsBuilder IsNot Nothing) Then
					diagnosticsBuilder.Add(New TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_NullableDisallowedForStructConstr1, New [Object]() { typeParameter })))
				End If
				flag = False
			End If
			Return flag
		End Function

		Private Class CheckConstraintsDiagnosticsBuilders
			Public diagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo)

			Public useSiteDiagnosticsBuilder As ArrayBuilder(Of TypeParameterDiagnosticInfo)

			Public template As CompoundUseSiteInfo(Of AssemblySymbol)

			Public Sub New()
				MyBase.New()
			End Sub
		End Class

		Private Enum DirectTypeConstraintKind
			None
			ReferenceTypeConstraint
			ValueTypeConstraint
			ExplicitType
		End Enum

		Private Structure TypeParameterAndConstraint
			Public ReadOnly TypeParameter As TypeParameterSymbol

			Public ReadOnly Constraint As TypeParameterConstraint

			Public ReadOnly IsBad As Boolean

			Public Sub New(ByVal typeParameter As TypeParameterSymbol, ByVal constraint As TypeParameterConstraint, Optional ByVal isBad As Boolean = False)
				Me = New ConstraintsHelper.TypeParameterAndConstraint() With
				{
					.TypeParameter = typeParameter,
					.Constraint = constraint,
					.IsBad = isBad
				}
			End Sub

			Public Function ToBad() As ConstraintsHelper.TypeParameterAndConstraint
				Return New ConstraintsHelper.TypeParameterAndConstraint(Me.TypeParameter, Me.Constraint, True)
			End Function

			Public Overrides Function ToString() As String
				Dim str As String = [String].Format("{0} : {1}", Me.TypeParameter, Me.Constraint)
				If (Me.IsBad) Then
					str = [String].Concat(str, " (bad)")
				End If
				Return str
			End Function
		End Structure
	End Module
End Namespace