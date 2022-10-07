Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class SynthesizedSubmissionFields
		Private ReadOnly _declaringSubmissionClass As NamedTypeSymbol

		Private ReadOnly _compilation As VisualBasicCompilation

		Private _hostObjectField As FieldSymbol

		Private _previousSubmissionFieldMap As Dictionary(Of ImplicitNamedTypeSymbol, FieldSymbol)

		Friend ReadOnly Property Count As Integer
			Get
				If (Me._previousSubmissionFieldMap Is Nothing) Then
					Return 0
				End If
				Return Me._previousSubmissionFieldMap.Count
			End Get
		End Property

		Friend ReadOnly Property FieldSymbols As IEnumerable(Of FieldSymbol)
			Get
				If (Me._previousSubmissionFieldMap IsNot Nothing) Then
					Return Me._previousSubmissionFieldMap.Values
				End If
				Return Array.Empty(Of FieldSymbol)()
			End Get
		End Property

		Public Sub New(ByVal compilation As VisualBasicCompilation, ByVal submissionClass As NamedTypeSymbol)
			MyBase.New()
			Me._declaringSubmissionClass = submissionClass
			Me._compilation = compilation
		End Sub

		Friend Sub AddToType(ByVal containingType As NamedTypeSymbol, ByVal moduleBeingBuilt As PEModuleBuilder)
			Dim enumerator As IEnumerator(Of FieldSymbol) = Nothing
			Using enumerator
				enumerator = Me.FieldSymbols.GetEnumerator()
				While enumerator.MoveNext()
					moduleBeingBuilt.AddSynthesizedDefinition(containingType, enumerator.Current.GetCciAdapter())
				End While
			End Using
			Dim hostObjectField As FieldSymbol = Me.GetHostObjectField()
			If (hostObjectField IsNot Nothing) Then
				moduleBeingBuilt.AddSynthesizedDefinition(containingType, hostObjectField.GetCciAdapter())
			End If
		End Sub

		Friend Function GetHostObjectField() As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol
			If (Me._hostObjectField Is Nothing) Then
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
				If (typeSymbol Is Nothing OrElse typeSymbol.Kind = SymbolKind.ErrorType) Then
					fieldSymbol = Nothing
				Else
					Me._hostObjectField = New SynthesizedFieldSymbol(Me._declaringSubmissionClass, Me._declaringSubmissionClass, typeSymbol, "<host-object>", Accessibility.[Private], True, False, False)
					fieldSymbol = Me._hostObjectField
				End If
			Else
				fieldSymbol = Me._hostObjectField
			End If
			Return fieldSymbol
		End Function

		Friend Function GetOrMakeField(ByVal previousSubmissionType As ImplicitNamedTypeSymbol) As FieldSymbol
			If (Me._previousSubmissionFieldMap Is Nothing) Then
				Me._previousSubmissionFieldMap = New Dictionary(Of ImplicitNamedTypeSymbol, FieldSymbol)()
			End If
			Dim synthesizedFieldSymbol As FieldSymbol = Nothing
			If (Not Me._previousSubmissionFieldMap.TryGetValue(previousSubmissionType, synthesizedFieldSymbol)) Then
				synthesizedFieldSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedFieldSymbol(Me._declaringSubmissionClass, Me._declaringSubmissionClass, previousSubmissionType, [String].Concat("<", previousSubmissionType.Name, ">"), Accessibility.[Private], True, False, False)
				Me._previousSubmissionFieldMap.Add(previousSubmissionType, synthesizedFieldSymbol)
			End If
			Return synthesizedFieldSymbol
		End Function
	End Class
End Namespace