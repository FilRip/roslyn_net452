Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class PEPropertyOrEventHelpers
		Public Sub New()
			MyBase.New()
		End Sub

		Friend Shared Function GetDeclaredAccessibilityFromAccessors(ByVal accessor1 As MethodSymbol, ByVal accessor2 As MethodSymbol) As Microsoft.CodeAnalysis.Accessibility
			Dim declaredAccessibility As Microsoft.CodeAnalysis.Accessibility
			Dim num As Integer
			If (accessor1 Is Nothing) Then
				declaredAccessibility = If(accessor2 Is Nothing, Microsoft.CodeAnalysis.Accessibility.NotApplicable, accessor2.DeclaredAccessibility)
			ElseIf (accessor2 IsNot Nothing) Then
				Dim accessibility As Microsoft.CodeAnalysis.Accessibility = accessor1.DeclaredAccessibility
				Dim declaredAccessibility1 As Microsoft.CodeAnalysis.Accessibility = accessor2.DeclaredAccessibility
				num = CInt(If(accessibility > declaredAccessibility1, declaredAccessibility1, accessibility))
				Dim accessibility1 As Microsoft.CodeAnalysis.Accessibility = If(accessibility > declaredAccessibility1, accessibility, declaredAccessibility1)
				declaredAccessibility = If(num <> 3 OrElse accessibility1 <> Microsoft.CodeAnalysis.Accessibility.Internal, accessibility1, Microsoft.CodeAnalysis.Accessibility.ProtectedOrInternal)
			Else
				declaredAccessibility = accessor1.DeclaredAccessibility
			End If
			Return declaredAccessibility
		End Function

		Friend Shared Function GetEventsForExplicitlyImplementedAccessor(ByVal accessor As MethodSymbol) As ISet(Of EventSymbol)
			Return PEPropertyOrEventHelpers.GetSymbolsForExplicitlyImplementedAccessor(Of EventSymbol)(accessor)
		End Function

		Friend Shared Function GetPropertiesForExplicitlyImplementedAccessor(ByVal accessor As MethodSymbol) As ISet(Of PropertySymbol)
			Return PEPropertyOrEventHelpers.GetSymbolsForExplicitlyImplementedAccessor(Of PropertySymbol)(accessor)
		End Function

		Private Shared Function GetSymbolsForExplicitlyImplementedAccessor(Of T As Symbol)(ByVal accessor As MethodSymbol) As ISet(Of T)
			Dim ts As ISet(Of T)
			If (accessor IsNot Nothing) Then
				Dim explicitInterfaceImplementations As ImmutableArray(Of MethodSymbol) = accessor.ExplicitInterfaceImplementations
				If (explicitInterfaceImplementations.Length <> 0) Then
					Dim ts1 As HashSet(Of T) = New HashSet(Of T)()
					Dim enumerator As ImmutableArray(Of MethodSymbol).Enumerator = explicitInterfaceImplementations.GetEnumerator()
					While enumerator.MoveNext()
						Dim associatedSymbol As T = DirectCast(TryCast(enumerator.Current.AssociatedSymbol, T), T)
						If (associatedSymbol Is Nothing) Then
							Continue While
						End If
						ts1.Add(associatedSymbol)
					End While
					ts = ts1
				Else
					ts = SpecializedCollections.EmptySet(Of T)()
				End If
			Else
				ts = SpecializedCollections.EmptySet(Of T)()
			End If
			Return ts
		End Function
	End Class
End Namespace