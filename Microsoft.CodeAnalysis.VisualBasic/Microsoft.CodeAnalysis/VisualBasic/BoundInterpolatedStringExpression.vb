Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundInterpolatedStringExpression
		Inherits BoundExpression
		Private ReadOnly _Contents As ImmutableArray(Of BoundNode)

		Private ReadOnly _Binder As Microsoft.CodeAnalysis.VisualBasic.Binder

		Public ReadOnly Property Binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Get
				Return Me._Binder
			End Get
		End Property

		Public ReadOnly Property Contents As ImmutableArray(Of BoundNode)
			Get
				Return Me._Contents
			End Get
		End Property

		Public ReadOnly Property HasInterpolations As Boolean
			Get
				Dim flag As Boolean
				Dim enumerator As ImmutableArray(Of BoundNode).Enumerator = Me.Contents.GetEnumerator()
				While True
					If (Not enumerator.MoveNext()) Then
						flag = False
						Exit While
					ElseIf (enumerator.Current.Kind = BoundKind.Interpolation) Then
						flag = True
						Exit While
					End If
				End While
				Return flag
			End Get
		End Property

		Public ReadOnly Property IsEmpty As Boolean
			Get
				Return Me.Contents.Length = 0
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal contents As ImmutableArray(Of BoundNode), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.InterpolatedStringExpression, syntax, type, If(hasErrors, True, contents.NonNullAndHasErrors()))
			Me._Contents = contents
			Me._Binder = binder
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitInterpolatedStringExpression(Me)
		End Function

		Public Function Update(ByVal contents As ImmutableArray(Of BoundNode), ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundInterpolatedStringExpression
			Dim boundInterpolatedStringExpression As Microsoft.CodeAnalysis.VisualBasic.BoundInterpolatedStringExpression
			If (contents <> Me.Contents OrElse binder <> Me.Binder OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundInterpolatedStringExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundInterpolatedStringExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundInterpolatedStringExpression(MyBase.Syntax, contents, binder, type, MyBase.HasErrors)
				boundInterpolatedStringExpression1.CopyAttributes(Me)
				boundInterpolatedStringExpression = boundInterpolatedStringExpression1
			Else
				boundInterpolatedStringExpression = Me
			End If
			Return boundInterpolatedStringExpression
		End Function
	End Class
End Namespace