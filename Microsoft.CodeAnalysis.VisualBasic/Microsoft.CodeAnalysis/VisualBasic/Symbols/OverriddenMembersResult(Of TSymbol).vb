Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class OverriddenMembersResult(Of TSymbol As Symbol)
		Public ReadOnly Shared Empty As OverriddenMembersResult(Of TSymbol)

		Private ReadOnly _overriddenMembers As ImmutableArray(Of TSymbol)

		Private ReadOnly _inexactOverriddenMembers As ImmutableArray(Of TSymbol)

		Private ReadOnly _inaccessibleMembers As ImmutableArray(Of TSymbol)

		Public ReadOnly Property InaccessibleMembers As ImmutableArray(Of TSymbol)
			Get
				Return Me._inaccessibleMembers
			End Get
		End Property

		Public ReadOnly Property InexactOverriddenMembers As ImmutableArray(Of TSymbol)
			Get
				Return Me._inexactOverriddenMembers
			End Get
		End Property

		Public ReadOnly Property OverriddenMember As TSymbol
			Get
				Dim tSymbol As TSymbol
				Dim enumerator As ImmutableArray(Of TSymbol).Enumerator = Me.OverriddenMembers.GetEnumerator()
				While True
					If (enumerator.MoveNext()) Then
						Dim current As TSymbol = enumerator.Current
						If (current.IsMustOverride OrElse current.IsOverridable OrElse current.IsOverrides) Then
							tSymbol = current
							Exit While
						End If
					Else
						tSymbol = Nothing
						Exit While
					End If
				End While
				Return tSymbol
			End Get
		End Property

		Public ReadOnly Property OverriddenMembers As ImmutableArray(Of TSymbol)
			Get
				Return Me._overriddenMembers
			End Get
		End Property

		Shared Sub New()
			OverriddenMembersResult(Of TSymbol).Empty = New OverriddenMembersResult(Of TSymbol)(ImmutableArray(Of TSymbol).Empty, ImmutableArray(Of TSymbol).Empty, ImmutableArray(Of TSymbol).Empty)
		End Sub

		Private Sub New(ByVal overriddenMembers As ImmutableArray(Of TSymbol), ByVal inexactOverriddenMembers As ImmutableArray(Of TSymbol), ByVal inaccessibleMembers As ImmutableArray(Of TSymbol))
			MyBase.New()
			Me._overriddenMembers = overriddenMembers
			Me._inexactOverriddenMembers = inexactOverriddenMembers
			Me._inaccessibleMembers = inaccessibleMembers
		End Sub

		Public Shared Function Create(ByVal overriddenMembers As ImmutableArray(Of TSymbol), ByVal inexactOverriddenMembers As ImmutableArray(Of TSymbol), ByVal inaccessibleMembers As ImmutableArray(Of TSymbol)) As OverriddenMembersResult(Of TSymbol)
			Dim overriddenMembersResult As OverriddenMembersResult(Of TSymbol)
			overriddenMembersResult = If(Not overriddenMembers.IsEmpty OrElse Not inexactOverriddenMembers.IsEmpty OrElse Not inaccessibleMembers.IsEmpty, New OverriddenMembersResult(Of TSymbol)(overriddenMembers, inexactOverriddenMembers, inaccessibleMembers), OverriddenMembersResult(Of TSymbol).Empty)
			Return overriddenMembersResult
		End Function

		Public Shared Function GetOverriddenMember(ByVal substitutedOverridingMember As TSymbol, ByVal overriddenByDefinitionMember As TSymbol) As TSymbol
			Dim tSymbol As TSymbol
			If (overriddenByDefinitionMember Is Nothing) Then
				tSymbol = Nothing
			Else
				Dim containingType As NamedTypeSymbol = overriddenByDefinitionMember.ContainingType
				Dim originalDefinition As NamedTypeSymbol = containingType.OriginalDefinition
				Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = substitutedOverridingMember.ContainingType.BaseTypeNoUseSiteDiagnostics
				While baseTypeNoUseSiteDiagnostics IsNot Nothing
					If (Not TypeSymbol.Equals(baseTypeNoUseSiteDiagnostics.OriginalDefinition, originalDefinition, TypeCompareKind.ConsiderEverything)) Then
						baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics
					ElseIf (Not TypeSymbol.Equals(baseTypeNoUseSiteDiagnostics, containingType, TypeCompareKind.ConsiderEverything)) Then
						tSymbol = DirectCast(overriddenByDefinitionMember.OriginalDefinition.AsMember(baseTypeNoUseSiteDiagnostics), TSymbol)
						Return tSymbol
					Else
						tSymbol = overriddenByDefinitionMember
						Return tSymbol
					End If
				End While
				Throw ExceptionUtilities.Unreachable
			End If
			Return tSymbol
		End Function
	End Class
End Namespace