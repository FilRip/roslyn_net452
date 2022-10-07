Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class CompilationUnitSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Implements ICompilationUnitSyntax
		Friend _options As SyntaxNode

		Friend _imports As SyntaxNode

		Friend _attributes As SyntaxNode

		Friend _members As SyntaxNode

		Public ReadOnly Property Attributes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributesStatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributesStatementSyntax)(MyBase.GetRed(Me._attributes, 2))
			End Get
		End Property

		Public ReadOnly Property EndOfFileToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax)._endOfFileToken, Me.GetChildPosition(4), MyBase.GetChildIndex(4))
			End Get
		End Property

		ReadOnly Property ICompilationUnitSyntax_EndOfFileToken As Microsoft.CodeAnalysis.SyntaxToken Implements ICompilationUnitSyntax.EndOfFileToken
			Get
				Return Me.EndOfFileToken
			End Get
		End Property

		Public ReadOnly Property [Imports] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax)(MyBase.GetRed(Me._imports, 1))
			End Get
		End Property

		Public ReadOnly Property Members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._members, 3))
			End Get
		End Property

		Public ReadOnly Property Options As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax)(MyBase.GetRedAtZero(Me._options))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal options As SyntaxNode, ByVal [imports] As SyntaxNode, ByVal attributes As SyntaxNode, ByVal members As SyntaxNode, ByVal endOfFileToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax(kind, errors, annotations, If(options IsNot Nothing, options.Green, Nothing), If([imports] IsNot Nothing, [imports].Green, Nothing), If(attributes IsNot Nothing, attributes.Green, Nothing), If(members IsNot Nothing, members.Green, Nothing), endOfFileToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitCompilationUnit(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitCompilationUnit(Me)
		End Sub

		Public Function AddAttributes(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributesStatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax
			Return Me.WithAttributes(Me.Attributes.AddRange(items))
		End Function

		Public Function AddImports(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax
			Return Me.WithImports(Me.[Imports].AddRange(items))
		End Function

		Public Function AddMembers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax
			Return Me.WithMembers(Me.Members.AddRange(items))
		End Function

		Public Function AddOptions(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax
			Return Me.WithOptions(Me.Options.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._options
					Exit Select
				Case 1
					syntaxNode = Me._imports
					Exit Select
				Case 2
					syntaxNode = Me._attributes
					Exit Select
				Case 3
					syntaxNode = Me._members
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim redAtZero As SyntaxNode
			Select Case i
				Case 0
					redAtZero = MyBase.GetRedAtZero(Me._options)
					Exit Select
				Case 1
					redAtZero = MyBase.GetRed(Me._imports, 1)
					Exit Select
				Case 2
					redAtZero = MyBase.GetRed(Me._attributes, 2)
					Exit Select
				Case 3
					redAtZero = MyBase.GetRed(Me._members, 3)
					Exit Select
				Case Else
					redAtZero = Nothing
					Exit Select
			End Select
			Return redAtZero
		End Function

		Public Function GetReferenceDirectives() As IList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ReferenceDirectiveTriviaSyntax)
			Return Me.GetReferenceDirectives(Nothing)
		End Function

		Friend Function GetReferenceDirectives(ByVal filter As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ReferenceDirectiveTriviaSyntax, Boolean)) As IList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ReferenceDirectiveTriviaSyntax)
			Return MyBase.GetFirstToken(True, False, False, False).GetDirectives(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ReferenceDirectiveTriviaSyntax)(filter)
		End Function

		Public Function Update(ByVal options As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax), ByVal [imports] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax), ByVal attributes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributesStatementSyntax), ByVal members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal endOfFileToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax
			Dim compilationUnitSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax
			If (options <> Me.Options OrElse [imports] <> Me.[Imports] OrElse attributes <> Me.Attributes OrElse members <> Me.Members OrElse endOfFileToken <> Me.EndOfFileToken) Then
				Dim compilationUnitSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CompilationUnit(options, [imports], attributes, members, endOfFileToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				compilationUnitSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, compilationUnitSyntax1, compilationUnitSyntax1.WithAnnotations(annotations))
			Else
				compilationUnitSyntax = Me
			End If
			Return compilationUnitSyntax
		End Function

		Public Function WithAttributes(ByVal attributes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributesStatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax
			Return Me.Update(Me.Options, Me.[Imports], attributes, Me.Members, Me.EndOfFileToken)
		End Function

		Public Function WithEndOfFileToken(ByVal endOfFileToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax
			Return Me.Update(Me.Options, Me.[Imports], Me.Attributes, Me.Members, endOfFileToken)
		End Function

		Public Function WithImports(ByVal [imports] As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax
			Return Me.Update(Me.Options, [imports], Me.Attributes, Me.Members, Me.EndOfFileToken)
		End Function

		Public Function WithMembers(ByVal members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax
			Return Me.Update(Me.Options, Me.[Imports], Me.Attributes, members, Me.EndOfFileToken)
		End Function

		Public Function WithOptions(ByVal options As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax
			Return Me.Update(options, Me.[Imports], Me.Attributes, Me.Members, Me.EndOfFileToken)
		End Function
	End Class
End Namespace