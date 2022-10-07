Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SourceTypeParameterSymbol
		Inherits SubstitutableTypeParameterSymbol
		Private ReadOnly _ordinal As Integer

		Private ReadOnly _name As String

		Private _lazyConstraints As ImmutableArray(Of TypeParameterConstraint)

		Private _lazyConstraintTypes As ImmutableArray(Of TypeSymbol)

		Friend Overrides ReadOnly Property ConstraintTypesNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
			Get
				Me.EnsureAllConstraintsAreResolved()
				Return Me._lazyConstraintTypes
			End Get
		End Property

		Protected MustOverride ReadOnly Property ContainerTypeParameters As ImmutableArray(Of TypeParameterSymbol)

		Public Overrides ReadOnly Property HasConstructorConstraint As Boolean
			Get
				Dim flag As Boolean
				Me.EnsureAllConstraintsAreResolved()
				Dim enumerator As ImmutableArray(Of TypeParameterConstraint).Enumerator = Me._lazyConstraints.GetEnumerator()
				While True
					If (Not enumerator.MoveNext()) Then
						flag = False
						Exit While
					ElseIf (enumerator.Current.IsConstructorConstraint) Then
						flag = True
						Exit While
					End If
				End While
				Return flag
			End Get
		End Property

		Public Overrides ReadOnly Property HasReferenceTypeConstraint As Boolean
			Get
				Dim flag As Boolean
				Me.EnsureAllConstraintsAreResolved()
				Dim enumerator As ImmutableArray(Of TypeParameterConstraint).Enumerator = Me._lazyConstraints.GetEnumerator()
				While True
					If (Not enumerator.MoveNext()) Then
						flag = False
						Exit While
					ElseIf (enumerator.Current.IsReferenceTypeConstraint) Then
						flag = True
						Exit While
					End If
				End While
				Return flag
			End Get
		End Property

		Public Overrides ReadOnly Property HasValueTypeConstraint As Boolean
			Get
				Dim flag As Boolean
				Me.EnsureAllConstraintsAreResolved()
				Dim enumerator As ImmutableArray(Of TypeParameterConstraint).Enumerator = Me._lazyConstraints.GetEnumerator()
				While True
					If (Not enumerator.MoveNext()) Then
						flag = False
						Exit While
					ElseIf (enumerator.Current.IsValueTypeConstraint) Then
						flag = True
						Exit While
					End If
				End While
				Return flag
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me.ContainingSymbol.IsImplicitlyDeclared
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Public Overrides ReadOnly Property Ordinal As Integer
			Get
				Return Me._ordinal
			End Get
		End Property

		Protected Sub New(ByVal ordinal As Integer, ByVal name As String)
			MyBase.New()
			Me._ordinal = ordinal
			Me._name = name
		End Sub

		Private Sub CheckConstraintTypeConstraints(ByVal constraints As ImmutableArray(Of TypeParameterConstraint), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim containingAssembly As AssemblySymbol = Me.ContainingAssembly
			Dim enumerator As ImmutableArray(Of TypeParameterConstraint).Enumerator = constraints.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As TypeParameterConstraint = enumerator.Current
				Dim typeConstraint As TypeSymbol = current.TypeConstraint
				If (typeConstraint Is Nothing) Then
					Continue While
				End If
				Dim locationOpt As Location = current.LocationOpt
				Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(diagnostics, containingAssembly)
				typeConstraint.AddUseSiteInfo(compoundUseSiteInfo)
				If (diagnostics.Add(locationOpt, compoundUseSiteInfo)) Then
					Continue While
				End If
				typeConstraint.CheckAllConstraints(locationOpt, diagnostics, New CompoundUseSiteInfo(Of AssemblySymbol)(diagnostics, containingAssembly))
			End While
		End Sub

		Friend Overrides Sub EnsureAllConstraintsAreResolved()
			If (Me._lazyConstraintTypes.IsDefault) Then
				TypeParameterSymbol.EnsureAllConstraintsAreResolved(Me.ContainerTypeParameters)
			End If
		End Sub

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return ImmutableArray(Of VisualBasicAttributeData).Empty
		End Function

		Friend Overrides Function GetConstraints() As ImmutableArray(Of TypeParameterConstraint)
			Me.EnsureAllConstraintsAreResolved()
			Return Me._lazyConstraints
		End Function

		Protected MustOverride Function GetDeclaredConstraints(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of TypeParameterConstraint)

		Private Shared Function GetLocation(ByVal diagnostic As TypeParameterDiagnosticInfo) As Location
			Dim item As Location
			Dim locationOpt As Location = diagnostic.Constraint.LocationOpt
			If (locationOpt Is Nothing) Then
				Dim locations As ImmutableArray(Of Location) = diagnostic.TypeParameter.Locations
				If (locations.Length <= 0) Then
					item = Nothing
				Else
					item = locations(0)
				End If
			Else
				item = locationOpt
			End If
			Return item
		End Function

		Protected Shared Function GetSymbolLocation(ByVal syntaxRef As SyntaxReference) As Location
			Dim syntax As SyntaxNode = syntaxRef.GetSyntax(New CancellationToken())
			Return syntaxRef.SyntaxTree.GetLocation(DirectCast(syntax, TypeParameterSyntax).Identifier.Span)
		End Function

		Protected MustOverride Function ReportRedundantConstraints() As Boolean

		Friend Overrides Sub ResolveConstraints(ByVal inProgress As ConsList(Of TypeParameterSymbol))
			If (Me._lazyConstraintTypes.IsDefault) Then
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
				Dim declaredConstraints As ImmutableArray(Of TypeParameterConstraint) = Me.GetDeclaredConstraints(instance)
				Dim directConstraintConflictKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.DirectConstraintConflictKind = 1 Or If(Me.ReportRedundantConstraints(), 2, 0)
				Dim typeParameterDiagnosticInfos As ArrayBuilder(Of TypeParameterDiagnosticInfo) = ArrayBuilder(Of TypeParameterDiagnosticInfo).GetInstance()
				declaredConstraints = Me.RemoveDirectConstraintConflicts(declaredConstraints, inProgress.Prepend(Me), directConstraintConflictKind, typeParameterDiagnosticInfos)
				ImmutableInterlocked.InterlockedInitialize(Of TypeParameterConstraint)(Me._lazyConstraints, declaredConstraints)
				If (ImmutableInterlocked.InterlockedInitialize(Of TypeSymbol)(Me._lazyConstraintTypes, TypeParameterSymbol.GetConstraintTypesOnly(declaredConstraints))) Then
					Dim typeParameterDiagnosticInfos1 As ArrayBuilder(Of TypeParameterDiagnosticInfo) = Nothing
					Me.ReportIndirectConstraintConflicts(typeParameterDiagnosticInfos, typeParameterDiagnosticInfos1)
					If (typeParameterDiagnosticInfos1 IsNot Nothing) Then
						typeParameterDiagnosticInfos.AddRange(typeParameterDiagnosticInfos1)
					End If
					Dim enumerator As ArrayBuilder(Of TypeParameterDiagnosticInfo).Enumerator = typeParameterDiagnosticInfos.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As TypeParameterDiagnosticInfo = enumerator.Current
						Dim location As Microsoft.CodeAnalysis.Location = SourceTypeParameterSymbol.GetLocation(current)
						instance.Add(current.UseSiteInfo, location)
					End While
					Me.CheckConstraintTypeConstraints(declaredConstraints, instance)
					DirectCast(Me.ContainingModule, SourceModuleSymbol).AddDeclarationDiagnostics(instance)
				End If
				typeParameterDiagnosticInfos.Free()
				instance.Free()
			End If
		End Sub
	End Class
End Namespace