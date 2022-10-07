Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SourceLabelSymbol
		Inherits LabelSymbol
		Private ReadOnly _labelName As SyntaxToken

		Private ReadOnly _containingMethod As MethodSymbol

		Private ReadOnly _binder As Binder

		Public Overrides ReadOnly Property ContainingMethod As MethodSymbol
			Get
				Return Me._containingMethod
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingMethod
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray.Create(Of SyntaxReference)(Me._labelName.Parent.GetReference())
			End Get
		End Property

		Friend Overrides ReadOnly Property LabelName As SyntaxToken
			Get
				Return Me._labelName
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray.Create(Of Location)(Me._labelName.GetLocation())
			End Get
		End Property

		Public Sub New(ByVal labelNameToken As SyntaxToken, ByVal containingMethod As MethodSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder)
			MyBase.New(labelNameToken.ValueText)
			Me._labelName = labelNameToken
			Me._containingMethod = containingMethod
			Me._binder = binder
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Dim flag As Boolean
			If (obj <> Me) Then
				Dim sourceLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceLabelSymbol = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceLabelSymbol)
				flag = If(sourceLabelSymbol Is Nothing OrElse Not sourceLabelSymbol._labelName.Equals(Me._labelName), False, [Object].Equals(sourceLabelSymbol._containingMethod, Me._containingMethod))
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Me._labelName.GetHashCode()
		End Function
	End Class
End Namespace