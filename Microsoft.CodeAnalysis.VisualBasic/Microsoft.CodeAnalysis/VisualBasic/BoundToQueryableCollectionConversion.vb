Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundToQueryableCollectionConversion
		Inherits BoundQueryPart
		Private ReadOnly _ConversionCall As BoundCall

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.ConversionCall)
			End Get
		End Property

		Public ReadOnly Property ConversionCall As BoundCall
			Get
				Return Me._ConversionCall
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.ConversionCall.ExpressionSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ResultKind As LookupResultKind
			Get
				Return Me.ConversionCall.ResultKind
			End Get
		End Property

		Public Sub New(ByVal [call] As BoundCall)
			MyClass.New([call].Syntax, [call], [call].Type, False)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal conversionCall As BoundCall, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ToQueryableCollectionConversion, syntax, type, If(hasErrors, True, conversionCall.NonNullAndHasErrors()))
			Me._ConversionCall = conversionCall
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitToQueryableCollectionConversion(Me)
		End Function

		Public Function Update(ByVal conversionCall As BoundCall, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundToQueryableCollectionConversion
			Dim boundToQueryableCollectionConversion As Microsoft.CodeAnalysis.VisualBasic.BoundToQueryableCollectionConversion
			If (conversionCall <> Me.ConversionCall OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundToQueryableCollectionConversion1 As Microsoft.CodeAnalysis.VisualBasic.BoundToQueryableCollectionConversion = New Microsoft.CodeAnalysis.VisualBasic.BoundToQueryableCollectionConversion(MyBase.Syntax, conversionCall, type, MyBase.HasErrors)
				boundToQueryableCollectionConversion1.CopyAttributes(Me)
				boundToQueryableCollectionConversion = boundToQueryableCollectionConversion1
			Else
				boundToQueryableCollectionConversion = Me
			End If
			Return boundToQueryableCollectionConversion
		End Function
	End Class
End Namespace