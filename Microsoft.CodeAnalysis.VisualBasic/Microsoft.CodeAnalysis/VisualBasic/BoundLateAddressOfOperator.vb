Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundLateAddressOfOperator
		Inherits BoundExpression
		Private ReadOnly _Binder As Microsoft.CodeAnalysis.VisualBasic.Binder

		Private ReadOnly _MemberAccess As BoundLateMemberAccess

		Public ReadOnly Property Binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Get
				Return Me._Binder
			End Get
		End Property

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.MemberAccess)
			End Get
		End Property

		Public ReadOnly Property MemberAccess As BoundLateMemberAccess
			Get
				Return Me._MemberAccess
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal memberAccess As BoundLateMemberAccess, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.LateAddressOfOperator, syntax, type, If(hasErrors, True, memberAccess.NonNullAndHasErrors()))
			Me._Binder = binder
			Me._MemberAccess = memberAccess
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitLateAddressOfOperator(Me)
		End Function

		Public Function Update(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal memberAccess As BoundLateMemberAccess, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundLateAddressOfOperator
			Dim boundLateAddressOfOperator As Microsoft.CodeAnalysis.VisualBasic.BoundLateAddressOfOperator
			If (binder <> Me.Binder OrElse memberAccess <> Me.MemberAccess OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundLateAddressOfOperator1 As Microsoft.CodeAnalysis.VisualBasic.BoundLateAddressOfOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundLateAddressOfOperator(MyBase.Syntax, binder, memberAccess, type, MyBase.HasErrors)
				boundLateAddressOfOperator1.CopyAttributes(Me)
				boundLateAddressOfOperator = boundLateAddressOfOperator1
			Else
				boundLateAddressOfOperator = Me
			End If
			Return boundLateAddressOfOperator
		End Function
	End Class
End Namespace