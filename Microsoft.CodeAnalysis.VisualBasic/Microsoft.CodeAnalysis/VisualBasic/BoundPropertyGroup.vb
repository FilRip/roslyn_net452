Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundPropertyGroup
		Inherits BoundMethodOrPropertyGroup
		Private ReadOnly _Properties As ImmutableArray(Of PropertySymbol)

		Private ReadOnly _ResultKind As LookupResultKind

		Public ReadOnly Property Properties As ImmutableArray(Of PropertySymbol)
			Get
				Return Me._Properties
			End Get
		End Property

		Public Overrides ReadOnly Property ResultKind As LookupResultKind
			Get
				Return Me._ResultKind
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal properties As ImmutableArray(Of PropertySymbol), ByVal resultKind As LookupResultKind, ByVal receiverOpt As BoundExpression, ByVal qualificationKind As Microsoft.CodeAnalysis.VisualBasic.QualificationKind, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.PropertyGroup, syntax, receiverOpt, qualificationKind, If(hasErrors, True, receiverOpt.NonNullAndHasErrors()))
			Me._Properties = properties
			Me._ResultKind = resultKind
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitPropertyGroup(Me)
		End Function

		Public Function Update(ByVal properties As ImmutableArray(Of PropertySymbol), ByVal resultKind As LookupResultKind, ByVal receiverOpt As BoundExpression, ByVal qualificationKind As Microsoft.CodeAnalysis.VisualBasic.QualificationKind) As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyGroup
			Dim boundPropertyGroup As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyGroup
			If (properties <> Me.Properties OrElse resultKind <> Me.ResultKind OrElse receiverOpt <> MyBase.ReceiverOpt OrElse qualificationKind <> MyBase.QualificationKind) Then
				Dim boundPropertyGroup1 As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyGroup = New Microsoft.CodeAnalysis.VisualBasic.BoundPropertyGroup(MyBase.Syntax, properties, resultKind, receiverOpt, qualificationKind, MyBase.HasErrors)
				boundPropertyGroup1.CopyAttributes(Me)
				boundPropertyGroup = boundPropertyGroup1
			Else
				boundPropertyGroup = Me
			End If
			Return boundPropertyGroup
		End Function
	End Class
End Namespace