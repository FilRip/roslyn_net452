Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundDimStatement
		Inherits BoundStatement
		Implements IBoundLocalDeclarations
		Private ReadOnly _LocalDeclarations As ImmutableArray(Of BoundLocalDeclarationBase)

		Private ReadOnly _InitializerOpt As BoundExpression

		ReadOnly Property IBoundLocalDeclarations_Declarations As ImmutableArray(Of BoundLocalDeclarationBase) Implements IBoundLocalDeclarations.Declarations
			Get
				Return Me.LocalDeclarations
			End Get
		End Property

		Public ReadOnly Property InitializerOpt As BoundExpression
			Get
				Return Me._InitializerOpt
			End Get
		End Property

		Public ReadOnly Property LocalDeclarations As ImmutableArray(Of BoundLocalDeclarationBase)
			Get
				Return Me._LocalDeclarations
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal localDeclarations As ImmutableArray(Of BoundLocalDeclarationBase), ByVal initializerOpt As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.DimStatement, syntax, If(hasErrors OrElse localDeclarations.NonNullAndHasErrors(), True, initializerOpt.NonNullAndHasErrors()))
			Me._LocalDeclarations = localDeclarations
			Me._InitializerOpt = initializerOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitDimStatement(Me)
		End Function

		Public Function Update(ByVal localDeclarations As ImmutableArray(Of BoundLocalDeclarationBase), ByVal initializerOpt As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundDimStatement
			Dim boundDimStatement As Microsoft.CodeAnalysis.VisualBasic.BoundDimStatement
			If (localDeclarations <> Me.LocalDeclarations OrElse initializerOpt <> Me.InitializerOpt) Then
				Dim boundDimStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundDimStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundDimStatement(MyBase.Syntax, localDeclarations, initializerOpt, MyBase.HasErrors)
				boundDimStatement1.CopyAttributes(Me)
				boundDimStatement = boundDimStatement1
			Else
				boundDimStatement = Me
			End If
			Return boundDimStatement
		End Function
	End Class
End Namespace