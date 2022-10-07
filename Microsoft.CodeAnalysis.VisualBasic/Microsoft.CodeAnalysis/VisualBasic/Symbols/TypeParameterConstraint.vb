Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Structure TypeParameterConstraint
		Public ReadOnly Kind As TypeParameterConstraintKind

		Public ReadOnly TypeConstraint As TypeSymbol

		Public ReadOnly LocationOpt As Location

		Public ReadOnly Property IsConstructorConstraint As Boolean
			Get
				Return Me.Kind = TypeParameterConstraintKind.Constructor
			End Get
		End Property

		Public ReadOnly Property IsReferenceTypeConstraint As Boolean
			Get
				Return Me.Kind = TypeParameterConstraintKind.ReferenceType
			End Get
		End Property

		Public ReadOnly Property IsValueTypeConstraint As Boolean
			Get
				Return Me.Kind = TypeParameterConstraintKind.ValueType
			End Get
		End Property

		Public Sub New(ByVal kind As TypeParameterConstraintKind, ByVal loc As Location)
			MyClass.New(kind, Nothing, loc)
		End Sub

		Public Sub New(ByVal type As TypeSymbol, ByVal loc As Location)
			MyClass.New(TypeParameterConstraintKind.None, type, loc)
		End Sub

		Private Sub New(ByVal kind As TypeParameterConstraintKind, ByVal type As TypeSymbol, ByVal loc As Location)
			Me = New TypeParameterConstraint() With
			{
				.Kind = kind,
				.TypeConstraint = type,
				.LocationOpt = loc
			}
		End Sub

		Public Function AtLocation(ByVal loc As Location) As TypeParameterConstraint
			Return New TypeParameterConstraint(Me.Kind, Me.TypeConstraint, loc)
		End Function

		Public Function ToDisplayFormat() As Object
			Dim text As Object
			If (Me.TypeConstraint Is Nothing) Then
				text = SyntaxFacts.GetText(TypeParameterConstraint.ToSyntaxKind(Me.Kind))
			Else
				text = CustomSymbolDisplayFormatter.ErrorNameWithKind(Me.TypeConstraint)
			End If
			Return text
		End Function

		Public Overrides Function ToString() As String
			Return Me.ToDisplayFormat().ToString()
		End Function

		Private Shared Function ToSyntaxKind(ByVal kind As TypeParameterConstraintKind) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Select Case kind
				Case TypeParameterConstraintKind.ReferenceType
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword
					Exit Select
				Case TypeParameterConstraintKind.ValueType
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword
					Exit Select
				Case TypeParameterConstraintKind.ReferenceType Or TypeParameterConstraintKind.ValueType
					Throw ExceptionUtilities.UnexpectedValue(kind)
				Case TypeParameterConstraintKind.Constructor
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewKeyword
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(kind)
			End Select
			Return syntaxKind
		End Function
	End Structure
End Namespace