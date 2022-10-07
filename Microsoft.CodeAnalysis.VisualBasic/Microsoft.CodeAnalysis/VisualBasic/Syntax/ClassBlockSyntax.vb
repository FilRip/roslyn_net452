Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ClassBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
		Friend _classStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax

		Friend _endClassStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows ReadOnly Property Begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax
			Get
				Return Me.ClassStatement
			End Get
		End Property

		Public Overrides ReadOnly Property BlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Get
				Return Me.ClassStatement
			End Get
		End Property

		Public ReadOnly Property ClassStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax)(Me._classStatement)
			End Get
		End Property

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows ReadOnly Property [End] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return Me.EndClassStatement
			End Get
		End Property

		Public Overrides ReadOnly Property EndBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return Me.EndClassStatement
			End Get
		End Property

		Public ReadOnly Property EndClassStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endClassStatement, 4)
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

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal classStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax, ByVal [inherits] As SyntaxNode, ByVal [implements] As SyntaxNode, ByVal members As SyntaxNode, ByVal endClassStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax(kind, errors, annotations, DirectCast(classStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax), If([inherits] IsNot Nothing, [inherits].Green, Nothing), If([implements] IsNot Nothing, [implements].Green, Nothing), If(members IsNot Nothing, members.Green, Nothing), DirectCast(endClassStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitClassBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitClassBlock(Me)
		End Sub

		Public Shadows Function AddImplements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax
			Return Me.WithImplements(Me.[Implements].AddRange(items))
		End Function

		Friend Overrides Function AddImplementsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.AddImplements(items)
		End Function

		Public Shadows Function AddInherits(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax
			Return Me.WithInherits(Me.[Inherits].AddRange(items))
		End Function

		Friend Overrides Function AddInheritsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.AddInherits(items)
		End Function

		Public Shadows Function AddMembers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax
			Return Me.WithMembers(Me.Members.AddRange(items))
		End Function

		Friend Overrides Function AddMembersCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.AddMembers(items)
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._classStatement
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
					syntaxNode = Me._endClassStatement
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
			Dim classStatement As SyntaxNode
			Select Case i
				Case 0
					classStatement = Me.ClassStatement
					Exit Select
				Case 1
					classStatement = MyBase.GetRed(Me._inherits, 1)
					Exit Select
				Case 2
					classStatement = MyBase.GetRed(Me._implements, 2)
					Exit Select
				Case 3
					classStatement = MyBase.GetRed(Me._members, 3)
					Exit Select
				Case 4
					classStatement = Me.EndClassStatement
					Exit Select
				Case Else
					classStatement = Nothing
					Exit Select
			End Select
			Return classStatement
		End Function

		Public Function Update(ByVal classStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax, ByVal [inherits] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax), ByVal [implements] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax), ByVal members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal endClassStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax
			Dim classBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax
			If (classStatement <> Me.ClassStatement OrElse [inherits] <> Me.[Inherits] OrElse [implements] <> Me.[Implements] OrElse members <> Me.Members OrElse endClassStatement <> Me.EndClassStatement) Then
				Dim classBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ClassBlock(classStatement, [inherits], [implements], members, endClassStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				classBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, classBlockSyntax1, classBlockSyntax1.WithAnnotations(annotations))
			Else
				classBlockSyntax = Me
			End If
			Return classBlockSyntax
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Function WithBegin(ByVal begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax
			Return Me.WithClassStatement(begin)
		End Function

		Public Overrides Function WithBlockStatement(ByVal blockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.WithClassStatement(DirectCast(blockStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax))
		End Function

		Public Function WithClassStatement(ByVal classStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax
			Return Me.Update(classStatement, Me.[Inherits], Me.[Implements], Me.Members, Me.EndClassStatement)
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows Function WithEnd(ByVal [end] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax
			Return Me.WithEndClassStatement([end])
		End Function

		Public Overrides Function WithEndBlockStatement(ByVal endBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.WithEndClassStatement(endBlockStatement)
		End Function

		Public Function WithEndClassStatement(ByVal endClassStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax
			Return Me.Update(Me.ClassStatement, Me.[Inherits], Me.[Implements], Me.Members, endClassStatement)
		End Function

		Public Shadows Function WithImplements(ByVal [implements] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax
			Return Me.Update(Me.ClassStatement, Me.[Inherits], [implements], Me.Members, Me.EndClassStatement)
		End Function

		Friend Overrides Function WithImplementsCore(ByVal [implements] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.WithImplements([implements])
		End Function

		Public Shadows Function WithInherits(ByVal [inherits] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax
			Return Me.Update(Me.ClassStatement, [inherits], Me.[Implements], Me.Members, Me.EndClassStatement)
		End Function

		Friend Overrides Function WithInheritsCore(ByVal [inherits] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.WithInherits([inherits])
		End Function

		Public Shadows Function WithMembers(ByVal members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax
			Return Me.Update(Me.ClassStatement, Me.[Inherits], Me.[Implements], members, Me.EndClassStatement)
		End Function

		Friend Overrides Function WithMembersCore(ByVal members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax
			Return Me.WithMembers(members)
		End Function
	End Class
End Namespace