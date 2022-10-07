Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module BaseTypeAnalysis
		Private Function GetBaseTypeReferenceDetails(ByVal chain As ConsList(Of BaseTypeAnalysis.DependencyDesc)) As DiagnosticInfo
			Return BaseTypeAnalysis.GetInheritanceOrDependenceDetails(chain, ERRID.ERR_BaseTypeReferences2)
		End Function

		Private Function GetDependenceChain(ByVal visited As HashSet(Of Symbol), ByVal root As SourceNamedTypeSymbol, ByVal current As TypeSymbol) As ConsList(Of BaseTypeAnalysis.DependencyDesc)
			Dim empty As ConsList(Of BaseTypeAnalysis.DependencyDesc)
			If (current Is Nothing OrElse current.Kind = SymbolKind.ErrorType) Then
				empty = Nothing
			Else
				Dim originalDefinition As TypeSymbol = current.OriginalDefinition
				If (root = originalDefinition) Then
					empty = ConsList(Of BaseTypeAnalysis.DependencyDesc).Empty
				ElseIf (visited.Add(current)) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(originalDefinition, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					If (namedTypeSymbol IsNot Nothing) Then
						Dim dependenceChain As ConsList(Of BaseTypeAnalysis.DependencyDesc) = Nothing
						dependenceChain = BaseTypeAnalysis.GetDependenceChain(visited, root, namedTypeSymbol.ContainingType)
						If (dependenceChain IsNot Nothing) Then
							empty = dependenceChain.Prepend(New BaseTypeAnalysis.DependencyDesc(BaseTypeAnalysis.DependencyKind.Containment, current))
						ElseIf (root.DetectTypeCircularity_ShouldStepIntoType(namedTypeSymbol)) Then
							If (namedTypeSymbol.TypeKind = TypeKind.[Class]) Then
								dependenceChain = BaseTypeAnalysis.GetDependenceChain(visited, root, namedTypeSymbol.GetBestKnownBaseType())
								If (dependenceChain Is Nothing) Then
									GoTo Label1
								End If
								empty = dependenceChain.Prepend(New BaseTypeAnalysis.DependencyDesc(BaseTypeAnalysis.DependencyKind.Inheritance, current))
								Return empty
							End If
						Label1:
							If (namedTypeSymbol.IsInterface) Then
								Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Enumerator = namedTypeSymbol.GetBestKnownInterfacesNoUseSiteDiagnostics().GetEnumerator()
								While enumerator.MoveNext()
									dependenceChain = BaseTypeAnalysis.GetDependenceChain(visited, root, enumerator.Current)
									If (dependenceChain Is Nothing) Then
										Continue While
									End If
									empty = dependenceChain.Prepend(New BaseTypeAnalysis.DependencyDesc(BaseTypeAnalysis.DependencyKind.Inheritance, current))
									Return empty
								End While
							End If
							empty = Nothing
						Else
							empty = Nothing
						End If
					Else
						empty = Nothing
					End If
				Else
					empty = Nothing
				End If
			End If
			Return empty
		End Function

		Friend Function GetDependenceDiagnosticForBase(ByVal this As SourceNamedTypeSymbol, ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim flag As Boolean = False
			Dim inheritsBeingResolvedOpt As ConsList(Of TypeSymbol) = basesBeingResolved.InheritsBeingResolvedOpt
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = this
			Dim dependencyDescs As ConsList(Of BaseTypeAnalysis.DependencyDesc) = ConsList(Of BaseTypeAnalysis.DependencyDesc).Empty.Prepend(New BaseTypeAnalysis.DependencyDesc(BaseTypeAnalysis.DependencyKind.Inheritance, this))
			Dim num As Integer = 1
			While True
				If (inheritsBeingResolvedOpt.Any()) Then
					Dim head As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(inheritsBeingResolvedOpt.Head, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					If (head = this) Then
						diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_IllegalBaseTypeReferences3, New [Object]() { this.GetKindText(), this, BaseTypeAnalysis.GetBaseTypeReferenceDetails(dependencyDescs) })
						Exit While
					ElseIf (this.DetectTypeCircularity_ShouldStepIntoType(head)) Then
						If (If(namedTypeSymbol Is Nothing, False, head.ContainingSymbol = namedTypeSymbol)) Then
							flag = True
						End If
						dependencyDescs = dependencyDescs.Prepend(New BaseTypeAnalysis.DependencyDesc(If(flag, BaseTypeAnalysis.DependencyKind.Containment, BaseTypeAnalysis.DependencyKind.Inheritance), head))
						num = num + 1
						namedTypeSymbol = head
						inheritsBeingResolvedOpt = inheritsBeingResolvedOpt.Tail
					Else
						diagnosticInfo = Nothing
						Exit While
					End If
				Else
					diagnosticInfo = Nothing
					Exit While
				End If
			End While
			Return diagnosticInfo
		End Function

		Friend Function GetDependenceDiagnosticForBase(ByVal this As SourceNamedTypeSymbol, ByVal base As TypeSymbol) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim enumerator As ConsList(Of BaseTypeAnalysis.DependencyDesc).Enumerator = New ConsList(Of BaseTypeAnalysis.DependencyDesc).Enumerator()
			Dim dependenceChain As ConsList(Of BaseTypeAnalysis.DependencyDesc) = BaseTypeAnalysis.GetDependenceChain(New HashSet(Of Symbol)(), DirectCast(this.OriginalDefinition, SourceNamedTypeSymbol), base)
			If (dependenceChain IsNot Nothing) Then
				dependenceChain = dependenceChain.Prepend(New BaseTypeAnalysis.DependencyDesc(BaseTypeAnalysis.DependencyKind.Inheritance, this))
				Dim num As Integer = 0
				Dim flag As Boolean = False
				Try
					enumerator = dependenceChain.GetEnumerator()
					While enumerator.MoveNext()
						If (enumerator.Current.kind = BaseTypeAnalysis.DependencyKind.Containment) Then
							flag = True
						End If
						num = num + 1
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
				If (flag) Then
					diagnosticInfo = If(num <= 2, ErrorFactory.ErrorInfo(ERRID.ERR_NestedBase2, New [Object]() { this.GetKindText(), this }), ErrorFactory.ErrorInfo(ERRID.ERR_CircularBaseDependencies4, New [Object]() { this.GetKindText(), this, BaseTypeAnalysis.GetInheritanceDetails(dependenceChain) }))
				Else
					diagnosticInfo = If(this.TypeKind <> TypeKind.[Class], ErrorFactory.ErrorInfo(ERRID.ERR_InterfaceCycle1, New [Object]() { this, BaseTypeAnalysis.GetInheritanceDetails(dependenceChain) }), ErrorFactory.ErrorInfo(ERRID.ERR_InheritanceCycle1, New [Object]() { this, BaseTypeAnalysis.GetInheritanceDetails(dependenceChain) }))
				End If
			Else
				diagnosticInfo = Nothing
			End If
			Return diagnosticInfo
		End Function

		Friend Function GetDependencyDiagnosticsForImportedBaseInterface(ByVal this As NamedTypeSymbol, ByVal base As NamedTypeSymbol) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			base = base.OriginalDefinition
			If (base IsNot Nothing) Then
				Dim typeSymbols As HashSet(Of TypeSymbol) = New HashSet(Of TypeSymbol)()
				typeSymbols.Add(base)
				If (Not BaseTypeAnalysis.HasCycles(typeSymbols, New HashSet(Of TypeSymbol)(), base)) Then
					diagnosticInfo = Nothing
				Else
					diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_InheritanceCycleInImportedType1, New [Object]() { this })
				End If
			Else
				diagnosticInfo = Nothing
			End If
			Return diagnosticInfo
		End Function

		Friend Function GetDependencyDiagnosticsForImportedClass(ByVal this As NamedTypeSymbol) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim originalDefinition As NamedTypeSymbol = this.OriginalDefinition
			If (originalDefinition IsNot Nothing) Then
				Dim basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
				Dim declaredBase As NamedTypeSymbol = this.GetDeclaredBase(basesBeingResolved)
				While declaredBase IsNot Nothing
					declaredBase = declaredBase.OriginalDefinition
					If (CObj(originalDefinition) <> CObj(declaredBase)) Then
						basesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
						declaredBase = declaredBase.GetDeclaredBase(basesBeingResolved)
						If (declaredBase Is Nothing) Then
							Exit While
						End If
						declaredBase = declaredBase.OriginalDefinition
						If (CObj(originalDefinition) <> CObj(declaredBase)) Then
							basesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
							declaredBase = declaredBase.GetDeclaredBase(basesBeingResolved)
							basesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
							originalDefinition = originalDefinition.GetDeclaredBase(basesBeingResolved).OriginalDefinition
						Else
							diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_InheritanceCycleInImportedType1, New [Object]() { this })
							Return diagnosticInfo
						End If
					Else
						diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_InheritanceCycleInImportedType1, New [Object]() { this })
						Return diagnosticInfo
					End If
				End While
				diagnosticInfo = Nothing
			Else
				diagnosticInfo = Nothing
			End If
			Return diagnosticInfo
		End Function

		Private Function GetInheritanceDetails(ByVal chain As ConsList(Of BaseTypeAnalysis.DependencyDesc)) As DiagnosticInfo
			Return BaseTypeAnalysis.GetInheritanceOrDependenceDetails(chain, ERRID.ERR_InheritsFrom2)
		End Function

		Private Function GetInheritanceOrDependenceDetails(ByVal chain As ConsList(Of BaseTypeAnalysis.DependencyDesc), ByVal inheritsOrDepends As Microsoft.CodeAnalysis.VisualBasic.ERRID) As DiagnosticInfo
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Dim enumerator As ConsList(Of BaseTypeAnalysis.DependencyDesc).Enumerator = New ConsList(Of BaseTypeAnalysis.DependencyDesc).Enumerator()
			Dim instance As ArrayBuilder(Of DiagnosticInfo) = ArrayBuilder(Of DiagnosticInfo).GetInstance()
			Dim head As BaseTypeAnalysis.DependencyDesc = chain.Head
			Try
				enumerator = chain.Tail.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As BaseTypeAnalysis.DependencyDesc = enumerator.Current
					eRRID = If(head.kind <> BaseTypeAnalysis.DependencyKind.Containment, inheritsOrDepends, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IsNestedIn2)
					instance.Add(ErrorFactory.ErrorInfo(eRRID, New [Object]() { head.dependent, current.dependent }))
					head = current
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			eRRID = If(head.kind <> BaseTypeAnalysis.DependencyKind.Containment, inheritsOrDepends, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IsNestedIn2)
			instance.Add(ErrorFactory.ErrorInfo(eRRID, New [Object]() { head.dependent, chain.Head.dependent }))
			Return New CompoundDiagnosticInfo(instance.ToArrayAndFree())
		End Function

		Private Function HasCycles(ByVal derived As HashSet(Of TypeSymbol), ByVal verified As HashSet(Of TypeSymbol), ByVal [interface] As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			Dim declaredInterfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol) = [interface].GetDeclaredInterfacesNoUseSiteDiagnostics(New BasesBeingResolved())
			If (Not declaredInterfacesNoUseSiteDiagnostics.IsEmpty) Then
				Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = declaredInterfacesNoUseSiteDiagnostics.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As NamedTypeSymbol = enumerator.Current
					current = current.OriginalDefinition
					If (current Is Nothing OrElse verified.Contains(current)) Then
						Continue While
					End If
					If (derived.Add(current)) Then
						If (Not BaseTypeAnalysis.HasCycles(derived, verified, current)) Then
							Continue While
						End If
						flag = True
						Return flag
					Else
						flag = True
						Return flag
					End If
				End While
			End If
			verified.Add([interface])
			derived.Remove([interface])
			flag = False
			Return flag
		End Function

		Private Structure DependencyDesc
			Public ReadOnly kind As BaseTypeAnalysis.DependencyKind

			Public ReadOnly dependent As TypeSymbol

			Friend Sub New(ByVal kind As BaseTypeAnalysis.DependencyKind, ByVal dependent As TypeSymbol)
				Me = New BaseTypeAnalysis.DependencyDesc() With
				{
					.kind = kind,
					.dependent = dependent
				}
			End Sub
		End Structure

		Private Enum DependencyKind
			Inheritance
			Containment
		End Enum
	End Module
End Namespace