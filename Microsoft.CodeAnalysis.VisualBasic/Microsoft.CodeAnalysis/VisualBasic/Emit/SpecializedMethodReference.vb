Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend Class SpecializedMethodReference
		Inherits MethodReference
		Implements ISpecializedMethodReference
		Public Overrides ReadOnly Property AsSpecializedMethodReference As ISpecializedMethodReference
			Get
				Return Me
			End Get
		End Property

		ReadOnly Property ISpecializedMethodReferenceUnspecializedVersion As IMethodReference Implements ISpecializedMethodReference.UnspecializedVersion
			Get
				Return Me.m_UnderlyingMethod.OriginalDefinition.GetCciAdapter()
			End Get
		End Property

		Public Sub New(ByVal underlyingMethod As MethodSymbol)
			MyBase.New(underlyingMethod)
		End Sub

		Public Overrides Sub Dispatch(ByVal visitor As MetadataVisitor)
			visitor.Visit(Me)
		End Sub
	End Class
End Namespace