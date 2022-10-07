Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundMethodOrPropertyGroup
		Inherits BoundExpression
		Private ReadOnly _ReceiverOpt As BoundExpression

		Private ReadOnly _QualificationKind As Microsoft.CodeAnalysis.VisualBasic.QualificationKind

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Dim empty As ImmutableArray(Of BoundNode)
				If (Me.ReceiverOpt Is Nothing) Then
					empty = ImmutableArray(Of BoundNode).Empty
				Else
					empty = ImmutableArray.Create(Of BoundNode)(Me.ReceiverOpt)
				End If
				Return empty
			End Get
		End Property

		Friend ReadOnly Property ContainerOfFirstInGroup As TypeSymbol
			Get
				Dim containingType As TypeSymbol
				Dim kind As BoundKind = MyBase.Kind
				If (kind = BoundKind.MethodGroup) Then
					containingType = DirectCast(Me, BoundMethodGroup).Methods(0).ContainingType
				Else
					If (kind <> BoundKind.PropertyGroup) Then
						Throw ExceptionUtilities.UnexpectedValue(MyBase.Kind)
					End If
					containingType = DirectCast(Me, BoundPropertyGroup).Properties(0).ContainingType
				End If
				Return containingType
			End Get
		End Property

		Friend ReadOnly Property MemberName As String
			Get
				Dim name As String
				Dim kind As BoundKind = MyBase.Kind
				If (kind = BoundKind.MethodGroup) Then
					name = DirectCast(Me, BoundMethodGroup).Methods(0).Name
				Else
					If (kind <> BoundKind.PropertyGroup) Then
						Throw ExceptionUtilities.UnexpectedValue(MyBase.Kind)
					End If
					name = DirectCast(Me, BoundPropertyGroup).Properties(0).Name
				End If
				Return name
			End Get
		End Property

		Public ReadOnly Property QualificationKind As Microsoft.CodeAnalysis.VisualBasic.QualificationKind
			Get
				Return Me._QualificationKind
			End Get
		End Property

		Public ReadOnly Property ReceiverOpt As BoundExpression
			Get
				Return Me._ReceiverOpt
			End Get
		End Property

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal receiverOpt As BoundExpression, ByVal qualificationKind As Microsoft.CodeAnalysis.VisualBasic.QualificationKind, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(kind, syntax, Nothing, hasErrors)
			Me._ReceiverOpt = receiverOpt
			Me._QualificationKind = qualificationKind
		End Sub
	End Class
End Namespace