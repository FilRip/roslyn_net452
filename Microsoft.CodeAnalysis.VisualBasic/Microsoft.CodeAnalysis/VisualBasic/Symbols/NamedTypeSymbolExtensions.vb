Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module NamedTypeSymbolExtensions
		<Extension>
		Friend Function AllowsExtensionMethods(ByVal container As NamedTypeSymbol) As Boolean
			If (container.TypeKind = TypeKind.[Module]) Then
				Return True
			End If
			Return container.IsScriptClass
		End Function

		<Extension>
		Public Function AsUnboundGenericType(ByVal this As NamedTypeSymbol) As NamedTypeSymbol
			Return UnboundGenericType.Create(this)
		End Function

		<Extension>
		Friend Function FindFieldOrProperty(ByVal container As NamedTypeSymbol, ByVal symbolName As String, ByVal nameSpan As TextSpan, ByVal tree As SyntaxTree) As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = container.GetMembers(symbolName).GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator.Current
					If (current.Kind = SymbolKind.Field OrElse current.Kind = SymbolKind.[Property]) Then
						Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.Location).Enumerator = current.Locations.GetEnumerator()
						While enumerator1.MoveNext()
							Dim location As Microsoft.CodeAnalysis.Location = enumerator1.Current
							If (Not location.IsInSource OrElse location.SourceTree <> tree OrElse Not (location.SourceSpan = nameSpan)) Then
								Continue While
							End If
							symbol = current
							Return symbol
						End While
					End If
				Else
					symbol = Nothing
					Exit While
				End If
			End While
			Return symbol
		End Function

		<Extension>
		Friend Function FindMember(ByVal container As NamedTypeSymbol, ByVal symbolName As String, ByVal kind As SymbolKind, ByVal nameSpan As TextSpan, ByVal tree As SyntaxTree) As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = container.GetMembers(symbolName).GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator.Current
					If (current.Kind = kind) Then
						Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.Location).Enumerator = current.Locations.GetEnumerator()
						While enumerator1.MoveNext()
							Dim location As Microsoft.CodeAnalysis.Location = enumerator1.Current
							If (Not location.IsInSource OrElse location.SourceTree <> tree OrElse Not (location.SourceSpan = nameSpan)) Then
								Continue While
							End If
							symbol = current
							Return symbol
						End While
						If (kind = SymbolKind.Method) Then
							Dim partialImplementationPart As MethodSymbol = DirectCast(current, MethodSymbol).PartialImplementationPart
							If (partialImplementationPart IsNot Nothing) Then
								Dim enumerator2 As ImmutableArray(Of Microsoft.CodeAnalysis.Location).Enumerator = partialImplementationPart.Locations.GetEnumerator()
								While enumerator2.MoveNext()
									Dim current1 As Microsoft.CodeAnalysis.Location = enumerator2.Current
									If (Not current1.IsInSource OrElse current1.SourceTree <> tree OrElse Not (current1.SourceSpan = nameSpan)) Then
										Continue While
									End If
									symbol = partialImplementationPart
									Return symbol
								End While
							End If
						End If
					End If
				Else
					symbol = Nothing
					Exit While
				End If
			End While
			Return symbol
		End Function

		<Extension>
		Friend Function HasVariance(ByVal this As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			Dim containingType As NamedTypeSymbol = this
			While True
				If (Not containingType.TypeParameters.HaveVariance()) Then
					containingType = containingType.ContainingType
					If (containingType Is Nothing) Then
						flag = False
						Exit While
					End If
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		<Extension>
		Friend Function HaveVariance(ByVal this As ImmutableArray(Of TypeParameterSymbol)) As Boolean
			Dim flag As Boolean
			Dim enumerator As ImmutableArray(Of TypeParameterSymbol).Enumerator = this.GetEnumerator()
			While True
				If (Not enumerator.MoveNext()) Then
					flag = False
					Exit While
				ElseIf (CShort(enumerator.Current.Variance) - CShort(VarianceKind.Out) <= CShort(VarianceKind.Out)) Then
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		<Extension>
		Friend Function IsOrInGenericType(ByVal toCheck As NamedTypeSymbol) As Boolean
			If (toCheck Is Nothing) Then
				Return False
			End If
			Return toCheck.IsGenericType
		End Function
	End Module
End Namespace