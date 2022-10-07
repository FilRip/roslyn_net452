Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class AnonymousObjectCreationExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NewExpressionSyntax
		Friend ReadOnly _initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax
			Get
				Return Me._initializer
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal newKeyword As KeywordSyntax, ByVal attributeLists As GreenNode, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax)
			MyBase.New(kind, newKeyword, attributeLists)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(initializer)
			Me._initializer = initializer
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal newKeyword As KeywordSyntax, ByVal attributeLists As GreenNode, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, newKeyword, attributeLists)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(initializer)
			Me._initializer = initializer
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal newKeyword As KeywordSyntax, ByVal attributeLists As GreenNode, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax)
			MyBase.New(kind, errors, annotations, newKeyword, attributeLists)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(initializer)
			Me._initializer = initializer
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim objectMemberInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax)
			If (objectMemberInitializerSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(objectMemberInitializerSyntax)
				Me._initializer = objectMemberInitializerSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitAnonymousObjectCreationExpression(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._newKeyword
					Exit Select
				Case 1
					greenNode = Me._attributeLists
					Exit Select
				Case 2
					greenNode = Me._initializer
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._newKeyword, Me._attributeLists, Me._initializer)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._newKeyword, Me._attributeLists, Me._initializer)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._initializer, IObjectWritable))
		End Sub
	End Class
End Namespace