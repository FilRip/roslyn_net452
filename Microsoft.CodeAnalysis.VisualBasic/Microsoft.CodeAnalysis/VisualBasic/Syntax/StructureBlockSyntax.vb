Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class StructureBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
		Friend _structureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax

		Friend _endStructureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows ReadOnly Property Begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax
			Get
				Return Me.StructureStatement
			End Get
		End Property

		Public Overrides ReadOnly Property BlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Get
				Return Me.StructureStatement
			End Get
		End Property

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows ReadOnly Property [End] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return Me.EndStructureStatement
			End Get
		End Property

		Public Overrides ReadOnly Property EndBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return Me.EndStructureStatement
			End Get
		End Property

		Public ReadOnly Property EndStructureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endStructureStatement, 4)
			End Get
		End Property

		Public Shadows ReadOnly Property [Implements] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax)(MyBase.GetRed(Me._implements, 2))
			End Get
		End Property

		Public Shadows ReadOnly Property [Inherits] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax)(MyBase.GetRed(Me._inherits, 1))
			End Get
		End Property

		Public Shadows ReadOnly Property Members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._members, 3))
			End Get
		End Property

		Public ReadOnly Property StructureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax)(Me._structureStatement)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal structureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax, ByVal [inherits] As SyntaxNode, ByVal [implements] As SyntaxNode, ByVal members As SyntaxNode, ByVal endStructureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax(kind, errors, annotations, DirectCast(structureStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax), If([inherits] IsNot Nothing, [inherits].Green, Nothing), If([implements] IsNot Nothing, [implements].Green, Nothing), If(members IsNot Nothing, members.Green, Nothing), DirectCast(endStructureStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitStructureBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitStructureBlock(Me)
		End Sub

		Public Shadows Function AddImplements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax
			Return Me.WithImplements(Me.[Implements].AddRange(items))
		End Function

		Friend Overrides Function AddImplementsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.AddImplements(items)
		End Function

		Public Shadows Function AddInherits(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax
			Return Me.WithInherits(Me.[Inherits].AddRange(items))
		End Function

		Friend Overrides Function AddInheritsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.AddInherits(items)
		End Function

		Public Shadows Function AddMembers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax
			Return Me.WithMembers(Me.Members.AddRange(items))
		End Function

		Friend Overrides Function AddMembersCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.AddMembers(items)
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._structureStatement
					Exit Select
				Case 1
					syntaxNode = Me._inherits
					Exit Select
				Case 2
					syntaxNode = Me._implements
					Exit Select
				Case 3
					syntaxNode = Me._members
					Exit Select
				Case 4
					syntaxNode = Me._endStructureStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetImplementsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax)
			Return Me.[Implements]
		End Function

		Friend Overrides Function GetInheritsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax)
			Return Me.[Inherits]
		End Function

		Friend Overrides Function GetMembersCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Return Me.Members
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim structureStatement As SyntaxNode
			Select Case i
				Case 0
					structureStatement = Me.StructureStatement
					Exit Select
				Case 1
					structureStatement = MyBase.GetRed(Me._inherits, 1)
					Exit Select
				Case 2
					structureStatement = MyBase.GetRed(Me._implements, 2)
					Exit Select
				Case 3
					structureStatement = MyBase.GetRed(Me._members, 3)
					Exit Select
				Case 4
					structureStatement = Me.EndStructureStatement
					Exit Select
				Case Else
					structureStatement = Nothing
					Exit Select
			End Select
			Return structureStatement
		End Function

		Public Function Update(ByVal structureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax, ByVal [inherits] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax), ByVal [implements] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax), ByVal members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal endStructureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax
			Dim structureBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax
			If (structureStatement <> Me.StructureStatement OrElse [inherits] <> Me.[Inherits] OrElse [implements] <> Me.[Implements] OrElse members <> Me.Members OrElse endStructureStatement <> Me.EndStructureStatement) Then
				Dim structureBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.StructureBlock(structureStatement, [inherits], [implements], members, endStructureStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				structureBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, structureBlockSyntax1, structureBlockSyntax1.WithAnnotations(annotations))
			Else
				structureBlockSyntax = Me
			End If
			Return structureBlockSyntax
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Function WithBegin(ByVal begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax
			Return Me.WithStructureStatement(begin)
		End Function

		Public Overrides Function WithBlockStatement(ByVal blockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.WithStructureStatement(DirectCast(blockStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax))
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows Function WithEnd(ByVal [end] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax
			Return Me.WithEndStructureStatement([end])
		End Function

		Public Overrides Function WithEndBlockStatement(ByVal endBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.WithEndStructureStatement(endBlockStatement)
		End Function

		Public Function WithEndStructureStatement(ByVal endStructureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax
			Return Me.Update(Me.StructureStatement, Me.[Inherits], Me.[Implements], Me.Members, endStructureStatement)
		End Function

		Public Shadows Function WithImplements(ByVal [implements] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax
			Return Me.Update(Me.StructureStatement, Me.[Inherits], [implements], Me.Members, Me.EndStructureStatement)
		End Function

		Friend Overrides Function WithImplementsCore(ByVal [implements] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.WithImplements([implements])
		End Function

		Public Shadows Function WithInherits(ByVal [inherits] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax
			Return Me.Update(Me.StructureStatement, [inherits], Me.[Implements], Me.Members, Me.EndStructureStatement)
		End Function

		Friend Overrides Function WithInheritsCore(ByVal [inherits] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.WithInherits([inherits])
		End Function

		Public Shadows Function WithMembers(ByVal members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax
			Return Me.Update(Me.StructureStatement, Me.[Inherits], Me.[Implements], members, Me.EndStructureStatement)
		End Function

		Friend Overrides Function WithMembersCore(ByVal members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.WithMembers(members)
		End Function

		Public Function WithStructureStatement(ByVal structureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax
			Return Me.Update(structureStatement, Me.[Inherits], Me.[Implements], Me.Members, Me.EndStructureStatement)
		End Function
	End Class
End Namespace